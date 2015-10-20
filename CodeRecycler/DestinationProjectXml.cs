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
  internal class DestinationProjectXml
  {
    /// <summary> Absolute pathname of the destination <c>Proj</c> including file name. </summary>
    internal string DestProjAbsolutePath { get; }

    /// <summary> Absolute Directory of the destination <c>Proj</c>. NO file name. </summary>
    internal string DestProjDirectory { get; }


    internal XDocument DestProjXdoc;
    internal XComment  StartPlaceHolder;
    internal XComment  EndPlaceHolder;
    private  List<XElement> itemGroups;
    private  XElement rootXelement;


    internal DestinationProjectXml(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";
      

      try
      {
        DestProjXdoc = XDocument.Load(DestProjAbsolutePath);
        rootXelement = DestProjXdoc.Element(Settings.MSBuild + "Project");
      }
      catch (Exception e)
      { Recycler.Crash(e, "DestinationProjParser CTOR (1 param) loading destination XML from " + DestProjAbsolutePath); }

      StartPlaceHolder = FindComment(Settings.StartPlaceholderComment);
      EndPlaceHolder   = FindComment(Settings.EndPlaceholderComment);
    }


    internal void ClearOldRecycledCodeLinks()
    {
      if (rootXelement != null)
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

            List<XElement> keepers = new List<XElement>(); // todo: Keep a copy of the old Recycle. Use that to generate Keepers.

            if (oldXml.Contains("ItemGroup"))
            {
              XElement keeperElements = XElement.Parse(oldXml);

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

            itemGroups = new List<XElement>();

            itemGroups.AddRange(rootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));

            if (itemGroups.Count == 0) Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");
          }
          catch (Exception e) { Recycler.Crash(e, "Bad Proj No ItemGroups: " + DestProjAbsolutePath); }
        }


        if (itemGroups != null)
        {
          foreach (XElement itemGroup in itemGroups)
          {
            itemGroup.Elements().Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
                                            (i.Attribute("Include") != null) && 
                                            i.Attribute("Link")    != null &&
                                            i.Attribute("Include")?.Value.Replace("..\\", "") != i.Attribute("Link")?.Value ).Remove();

            if (itemGroup.IsEmpty) itemGroup.Remove();
          }
        }

        if (StartPlaceHolder == null)
        {
          XElement lastItemGroup =rootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements).Last();
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


    internal void ClearCodeIncludesExceptLinked()
    {
      if (rootXelement != null)
      {
        itemGroups = new List<XElement>();
        itemGroups.AddRange(rootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));
        if (itemGroups.Count == 0) Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");

        if (itemGroups != null)
        {
          foreach (XElement itemGroup in itemGroups)
          {
            itemGroup.Elements().Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) && 
                                      (i.Attribute("Include") != null) && 
                                       i.Attribute("Link")    == null).Remove();

            if (itemGroup.IsEmpty) itemGroup.Remove();
          }
        }
      }
      

      DestProjXdoc.Save(DestProjAbsolutePath);
    }
  }
}
