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


    internal XDocument DestProjXdoc;
    internal XComment  StartPlaceHolder;
    internal XComment  EndPlaceHolder;
    private  List<XElement> itemGroups;


    internal ProjectStripper(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      try { DestProjXdoc = XDocument.Load(DestProjAbsolutePath); }
      catch (Exception e)
      { Recycler.Crash(e, "DestinationProjParser CTOR (1 param) loading destination XML from " + DestProjAbsolutePath); }

      StartPlaceHolder = FindComment(Settings.StartPlaceholderComment);
      EndPlaceHolder   = FindComment(Settings.EndPlaceholderComment);
    }


    internal void Strip()
    {
      XElement xElement = DestProjXdoc.Element(Settings.MSBuild + "Project");

      if (xElement != null)
      {
        try
        {
          if (StartPlaceHolder != null && EndPlaceHolder != null && StartPlaceHolder.IsBefore(EndPlaceHolder)) // previously recycled
          {
            XNode startNode = StartPlaceHolder;
            while (startNode.NextNode != EndPlaceHolder) { startNode.NextNode.Remove(); }

            Log.WriteLine("Removed old Recycled Code from " + DestProjAbsolutePath);
          }

          itemGroups = new List<XElement>();

          itemGroups.AddRange(xElement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));

          if (itemGroups.Count == 0)
            Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");
        }
        catch (Exception e) { Recycler.Crash(e, "Bad Proj No ItemGroups: " + DestProjAbsolutePath); }


        foreach (XElement itemGroup in itemGroups)
        {
          itemGroup.Elements().Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
                                          (i.Attribute("Include") != null) && 
                                           i.Attribute("Link")    != null &&
                                           i.Attribute("Include")?.Value.Replace("..\\", "") != i.Attribute("Link")?.Value ).Remove();

          if (itemGroup.IsEmpty) itemGroup.Remove();
        }

        if (StartPlaceHolder == null)
        {
          XElement lastItemGroup =xElement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements).Last();
          lastItemGroup.AddAfterSelf(new XComment(Settings.EndPlaceholderComment));
          lastItemGroup.AddAfterSelf(new XComment( Settings.StartPlaceholderComment ));
          StartPlaceHolder = FindComment(Settings.StartPlaceholderComment);
          EndPlaceHolder   = FindComment(Settings.EndPlaceholderComment);
        }

        DestProjXdoc.Save(DestProjAbsolutePath);
      }
    }


    private XComment FindComment(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in DestProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

      if (placeholders.Count > 1)
        Recycler.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);

      return placeholders.FirstOrDefault();
    }
  }
}
