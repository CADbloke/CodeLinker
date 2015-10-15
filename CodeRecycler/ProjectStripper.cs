using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CodeRecycler
{
  internal class ProjectStripper
  {
    /// <summary> Absolute pathname of the destination <c>Proj</c> including file name. </summary>
    internal string DestProjAbsolutePath { get; }

    /// <summary> Absolute Directory of the destination <c>Proj</c>. NO file name. </summary>
    internal string DestProjDirectory { get; }


    private XDocument destProjXdoc;
    private XComment  startPlaceHolder;
    private XComment  endPlaceHolder;

    private  List<XElement> ItemGroups;


    internal ProjectStripper(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      try { destProjXdoc = XDocument.Load(DestProjAbsolutePath); }
      catch (Exception e)
      {
        Program.Crash(e, "DestinationProjParser CTOR (1 param) loading destination XML from " + DestProjAbsolutePath);
      }

      startPlaceHolder = FindComment(DestinationProjParser.StartPlaceholderComment);
      endPlaceHolder   = FindComment(DestinationProjParser.EndPlaceholderComment);
    }


    internal void Strip()
    {
      XElement xElement = destProjXdoc.Element(DestinationProjParser.MSBuild + "Project");
      try
      {
        if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder)) // previously recycled
        {
          XNode startNode = startPlaceHolder;
          while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }

          Log.WriteLine("Removed old Recycled Code from " + DestProjAbsolutePath);
        }


        ItemGroups = new List<XElement>();

        
        if (xElement != null)
        {
          ItemGroups.AddRange(xElement.Elements(DestinationProjParser.MSBuild + "ItemGroup").Select(elements => elements));
        }

        if (ItemGroups.Count == 0) { Log.WriteLine("Curious: " + DestProjDirectory + " contains no ItemGroups. No Codez?"); }
      }
      catch (Exception e) { Program.Crash(e, "Bad Proj No ItemGroups: " + DestProjDirectory); }

      // Log.WriteLine("Strip: Parsing " + ItemGroups.Count + " ItemGroups");

      foreach (XElement itemGroup in ItemGroups)
      {
        itemGroup.Elements().Where(i => !DestinationProjParser.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
        (i.Attribute("Include") != null || i.Attribute("Exclude") != null)).Remove();

        if (itemGroup.IsEmpty) itemGroup.Remove();
      }

      // xElement?.Elements(DestinationProjParser.MSBuild + "ItemGroup")?.Where(element => element.IsEmpty)?.Remove();

      // ItemGroups.Where(i => i.IsEmpty).Remove();
      // if (ItemGroups.Count <1 ) {ItemGroups.Add(new XElement( DestinationProjParser.MSBuild + "ItemGroup")); }

      if (startPlaceHolder == null)
      {
        XElement lastItemGroup =xElement.Elements(DestinationProjParser.MSBuild + "ItemGroup").Select(elements => elements).Last();
      lastItemGroup.AddAfterSelf(new XComment(DestinationProjParser.EndPlaceholderComment));

      lastItemGroup.AddAfterSelf(new XComment(
        DestinationProjParser.StartPlaceholderComment     + Environment.NewLine +
        DestinationProjParser.SourcePlaceholderLowerCase  + "Source Project(s) here, one per line"     + Environment.NewLine +
        DestinationProjParser.ExcludePlaceholderLowerCase + "Optional Exclusion(s) here, one per line" + Environment.NewLine ));
      }

      destProjXdoc.Save(DestProjAbsolutePath);
    }


    private XComment FindComment(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in destProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

      if (placeholders.Count > 1)
      {
        Program.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);
      }

      return placeholders.FirstOrDefault();
    }
  }
}
