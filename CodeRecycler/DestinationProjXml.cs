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


    internal DestinationProjXml(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory    = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      try
      {
        DestProjXdoc = XDocument.Load(DestProjAbsolutePath);
        RootXelement = DestProjXdoc.Element(Settings.MSBuild + "Project");
      }
      catch (Exception e)
      { Recycler.Crash(e, "Crash: DestProjXml CTOR loading destination XML from " + DestProjAbsolutePath); }

      if (RootXelement == null)
      { Recycler.Crash("Crash: No MSBuild Namespace in " + DestProjAbsolutePath);         
      }

      StartPlaceHolder = FindCommentOrCrash(Settings.StartPlaceholderComment);
      EndPlaceHolder   = FindCommentOrCrash(Settings.EndPlaceholderComment);

      if (StartPlaceHolder == null && RootXelement != null)
        {
          XElement lastItemGroup =RootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements).Last();
          lastItemGroup.AddAfterSelf(new XComment(Settings.EndPlaceholderComment));
          lastItemGroup.AddAfterSelf(new XComment( Settings.StartPlaceholderComment ));
          StartPlaceHolder = FindCommentOrCrash(Settings.StartPlaceholderComment);
          EndPlaceHolder   = FindCommentOrCrash(Settings.EndPlaceholderComment);
        }
    }


    internal void ClearOldRecycledCodeLinks() // todo: Keep a copy of the old Recycle. Use that to generate Keepers.
    {
      if (RootXelement != null)
      {
        if (StartPlaceHolder != null && EndPlaceHolder != null)
        {
          try
          {
            StringBuilder oldXmlBuilder = new StringBuilder();
            XNode startNode = StartPlaceHolder;
            while (startNode.NextNode != EndPlaceHolder)
            {
              oldXmlBuilder.Append(startNode.NextNode.ToString());
              startNode.NextNode.Remove();
            }
            string oldXml = oldXmlBuilder.ToString();

            List<XElement> keepers = new List<XElement>(); 

            if (oldXml.Contains("ItemGroup"))
            {
              XElement keeperElements = XElement.Parse( "<root>" + oldXml +"</root>" ); // http://stackoverflow.com/a/11644640/492

              foreach (XElement descendant in keeperElements.Elements().Where(e => e.Attribute("Include") != null))
              {
                if (!descendant.Attribute("Include").Value.StartsWith("..")) { keepers.Add(descendant); }
              }

              if (keepers.Any())
              {
                XElement newItemGroup = new XElement(Settings.MSBuild + "ItemGroup");
                foreach (XElement keeper in keepers) { newItemGroup.Add(keeper); }
                EndPlaceHolder.AddAfterSelf(newItemGroup);
              }
            }

            Log.WriteLine("Removed old Recycled Code from " + DestProjAbsolutePath);

            ItemGroups = new List<XElement>();

            ItemGroups.AddRange(RootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));

            if (ItemGroups.Count == 0) Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");
          }
          catch (Exception e) { Recycler.Crash(e, "Bad Proj No ItemGroups: " + DestProjAbsolutePath); }
        }


        if (ItemGroups != null)
        {
          foreach (XElement itemGroup in ItemGroups)
          {
            itemGroup.Elements().Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
                                            (i.Attribute("Include") != null) && 
                                             i.Attribute("Link")    != null &&
                                             i.Attribute("Include")?.Value.Replace("..\\", "") != i.Attribute("Link")?.Value ).Remove();

            if (itemGroup.IsEmpty) itemGroup.Remove();
          }
        }
        // Save(); NO! only save the project DELIBERATELY if there are changes or it reloads in Visual Studio
      }
    }

    /// <summary> Reads XML of between the Recycled zone placeholders. </summary>
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

            if (itemGroup.IsEmpty) itemGroup.Remove();
          }
        }
      }
    }


    internal void Save() { DestProjXdoc.Save(DestProjAbsolutePath); }
  }
}
