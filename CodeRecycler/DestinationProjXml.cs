using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CodeRecycler
{
  /// <summary> Removes previously Recycled Code. </summary>
  internal class DestinationProjXml
  {
    /// <summary> Absolute pathname of the destination <c>Proj</c> including file name. </summary>
    internal string DestProjAbsolutePath { get; }

    /// <summary> Absolute Directory of the destination <c>Proj</c>. NO file name. </summary>
    internal string DestProjDirectory { get; }


    internal XDocument DestProjXdoc;
    internal XComment  StartPlaceHolder;
    internal XComment  EndPlaceHolder;
    internal List<XElement> ItemGroups;
    internal XElement  RootXelement;
    internal string    OldXml;


    internal DestinationProjXml(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory    = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      try
      {
        DestProjXdoc = XDocument.Load(DestProjAbsolutePath);
        RootXelement = DestProjXdoc.Element(Settings.MSBuild + "Project");
        ItemGroups   = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
      }
      catch (Exception e)
      { Recycler.Crash(e, "Crash: DestProjXml CTOR loading destination XML from " + DestProjAbsolutePath); }

      if (RootXelement == null)
      { Recycler.Crash("Crash: No MSBuild Namespace in " + DestProjAbsolutePath); }

      StartPlaceHolder = FindCommentOrCrash(Settings.StartPlaceholderComment);
      EndPlaceHolder   = FindCommentOrCrash(Settings.EndPlaceholderComment);

      if (StartPlaceHolder == null && RootXelement != null)
        {
          XElement lastItemGroup = ItemGroups?.Last();
          lastItemGroup?.AddAfterSelf(new XComment(Settings.EndPlaceholderComment));
          lastItemGroup?.AddAfterSelf(new XComment( Settings.StartPlaceholderComment ));
          StartPlaceHolder = FindCommentOrCrash(Settings.StartPlaceholderComment);
          EndPlaceHolder   = FindCommentOrCrash(Settings.EndPlaceholderComment);
        }

      OldXml = ReadRecycledXml();
    }


    internal void ClearOldRecycledCodeLinks() // todo: Keep a copy of the old Recycle. Use that to generate Keepers.
    {
      Log.WriteLine("Housekeeping old Recycled Code from " + DestProjAbsolutePath);
      if (RootXelement != null)
      {
        if (StartPlaceHolder != null && EndPlaceHolder != null)
        {
          try
          {
            string oldXml = "<root>" + OldXml + "</root>";

            List<XElement> keepers = new List<XElement>(); 

            if (oldXml.Contains("ItemGroup"))
            {
              XElement keeperElements = XElement.Parse(oldXml); // http://stackoverflow.com/a/11644640/492

              Log.WriteLine(keeperElements.DescendantsAndSelf().Count().ToString());

              foreach (XElement descendant in keeperElements.DescendantsAndSelf().Where(e => e.Attribute("Include") != null))
              {
                XAttribute xAttribute = descendant.Attribute("Include");
                if (xAttribute != null && !xAttribute.Value.StartsWith(".."))
                {
                  keepers.Add(descendant);
                  Log.WriteLine("Keeping: " + descendant.ToString().Replace("xmlns=\"" + Settings.MSBuild.ToString() + "\"", ""));
                }
              }

              if (keepers.Any())
              {
                XElement newItemGroup = new XElement(Settings.MSBuild + "ItemGroup");
                foreach (XElement keeper in keepers) { newItemGroup.Add(keeper); }
                EndPlaceHolder.AddAfterSelf(newItemGroup); // move the keepers out of the Recycle zone.
              }
            }



            if (StartPlaceHolder != null && EndPlaceHolder != null && StartPlaceHolder.IsBefore(EndPlaceHolder))
            {
              XNode startNode = StartPlaceHolder;
              while (startNode.NextNode != EndPlaceHolder)
              {
                startNode.NextNode.Remove();
              }
            }

            foreach (XElement itemGroup in ItemGroups)
              if (itemGroup.IsEmpty || (!itemGroup.Descendants().Any() && string.IsNullOrEmpty(itemGroup.Value))) itemGroup.Remove();

            ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
          }
          catch (Exception e) { Recycler.Crash(e, "Bad Proj No ItemGroups: " + DestProjAbsolutePath); }
        }


        
        // Save(); NO! only save the project DELIBERATELY if there are changes or it reloads in Visual Studio
      }
    }

    /// <summary> Reads XML of between the Recycled zone placeholders. Does not add a <c>&lt;Root&gt;</c> element </summary>
    internal string ReadRecycledXml()
    {
      StringBuilder oldXmlBuilder = new StringBuilder();
      if (StartPlaceHolder != null && EndPlaceHolder != null && StartPlaceHolder.IsBefore(EndPlaceHolder))
      {
        XNode startNode = StartPlaceHolder;
        while (startNode.NextNode != EndPlaceHolder)
        {
          oldXmlBuilder.Append(startNode.NextNode.ToString());
          startNode = startNode.NextNode;
        }
        return oldXmlBuilder.ToString();
      }
      return string.Empty;
    }


    private XComment FindCommentOrCrash(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in DestProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

      if (placeholders.Count > 1)
        Recycler.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);

      return placeholders.FirstOrDefault();
    }


    internal void ClearExistingCodeExceptLinked()
    {
      if (RootXelement != null)
      {
        ItemGroups = new List<XElement>();
        ItemGroups.AddRange(RootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));
        if (ItemGroups.Count == 0) Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");

        if (ItemGroups != null)
        {
          foreach (XElement itemGroup in ItemGroups)
          {
            itemGroup.Elements().Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
                                      (i.Attribute("Include") != null) && 
                                       i.Attribute("Link")    == null).Remove();

            if (itemGroup.IsEmpty || (!itemGroup.Descendants().Any() && string.IsNullOrEmpty(itemGroup.Value))) itemGroup.Remove();
          }
          ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
          Log.WriteLine("Removed old Code from: " + DestProjAbsolutePath);
        }
      }
    }


    internal void Save() { DestProjXdoc.Save(DestProjAbsolutePath); }
  }
}
