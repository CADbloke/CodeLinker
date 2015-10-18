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
    private  List<XElement> itemGroups;


    internal ProjectStripper(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      try { destProjXdoc = XDocument.Load(DestProjAbsolutePath); }
      catch (Exception e)
      { Program.Crash(e, "DestinationProjParser CTOR (1 param) loading destination XML from " + DestProjAbsolutePath); }

      startPlaceHolder = FindComment(Settings.StartPlaceholderComment);
      endPlaceHolder   = FindComment(Settings.EndPlaceholderComment);
    }


    internal void Strip()
    {
      XElement xElement = destProjXdoc.Element(Settings.MSBuild + "Project");

      if (xElement != null)
      {
        try
        {
          if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder)) // previously recycled
          {
            XNode startNode = startPlaceHolder;
            while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }

            Log.WriteLine("Removed old Recycled Code from " + DestProjAbsolutePath);
          }

          itemGroups = new List<XElement>();

          itemGroups.AddRange(xElement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));

          if (itemGroups.Count == 0)
            Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");
        }
        catch (Exception e) { Program.Crash(e, "Bad Proj No ItemGroups: " + DestProjDirectory); }


        foreach (XElement itemGroup in itemGroups)
        {
          itemGroup.Elements().Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
                                          (i.Attribute("Include") != null || i.Attribute("Exclude") != null)).Remove();

          if (itemGroup.IsEmpty) itemGroup.Remove();
        }

        if (startPlaceHolder == null)
        {
          XElement lastItemGroup =xElement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements).Last();
          lastItemGroup.AddAfterSelf(new XComment(Settings.EndPlaceholderComment));

          lastItemGroup.AddAfterSelf(new XComment(
                                       Settings.StartPlaceholderComment     + Environment.NewLine +
                                       Settings.SourcePlaceholderLowerCase  + "Source Project(s) here, one per line"     + Environment.NewLine +
                                       Settings.ExcludePlaceholderLowerCase + "Optional Exclusion(s) here, one per line" + Environment.NewLine ));
        }

        destProjXdoc.Save(DestProjAbsolutePath);
      }
    }


    private XComment FindComment(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in destProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

      if (placeholders.Count > 1)
        Program.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);

      return placeholders.FirstOrDefault();
    }
  }
}
