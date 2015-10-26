using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CodeLinker
{
  /// <summary> Removes previously Linked Code. </summary>
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
    internal string    OldLinkedXml;


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
      { Linker.Crash(e, "Crash: DestProjXml CTOR loading destination XML from " + DestProjAbsolutePath); }

      if (RootXelement == null)
      { Linker.Crash("Crash: No MSBuild Namespace in " + DestProjAbsolutePath); }

      StartPlaceHolder = FindCommentOrCrashIfDuplicatesFound(Settings.StartPlaceholderComment);
      EndPlaceHolder   = FindCommentOrCrashIfDuplicatesFound(Settings.EndPlaceholderComment);

      if (StartPlaceHolder == null && RootXelement != null)
        {
          XElement lastItemGroup = ItemGroups?.Last();
          lastItemGroup?.AddAfterSelf(new XComment( Settings.EndPlaceholderComment   ));
          lastItemGroup?.AddAfterSelf(new XComment( Settings.StartPlaceholderComment ));
          StartPlaceHolder = FindCommentOrCrashIfDuplicatesFound(Settings.StartPlaceholderComment);
          EndPlaceHolder   = FindCommentOrCrashIfDuplicatesFound(Settings.EndPlaceholderComment);
        }

      OldLinkedXml = ReadLinkedXml();
    }


    internal void ClearOldLinkedCode()
    {
      Log.WriteLine("Housekeeping old Linked Code.");
      if (RootXelement != null)
      {
        if (StartPlaceHolder != null && EndPlaceHolder != null)
        {
          try
          {
            string oldLinkedXml = "<root>" + OldLinkedXml + "</root>";

            List<XElement> keepers = new List<XElement>(); 

            if (oldLinkedXml.Contains("ItemGroup"))
            {
              XElement keeperElements = XElement.Parse(oldLinkedXml); // http://stackoverflow.com/a/11644640/492

              foreach (XElement descendant in keeperElements.DescendantsAndSelf().Where(e => e.Attribute("Include") != null))
              {
                XAttribute xAttribute = descendant.Attribute("Include");
                if (xAttribute != null && !xAttribute.Value.StartsWith("..")) // keep stray code that is not a link. VS may have added it here.
                {
                  keepers.Add(descendant);
                  Log.WriteLine("Keeping: " + descendant.ToString().Replace("xmlns=\"" + Settings.MSBuild.ToString() + "\"", ""));
                }
              }

              if (keepers.Any())
              {
                XElement newItemGroup = new XElement(Settings.MSBuild + "ItemGroup");
                foreach (XElement keeper in keepers) { newItemGroup.Add(keeper); }
                EndPlaceHolder.AddAfterSelf(newItemGroup); // move the keepers out of the LinkCodez zone.
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
          catch (Exception e) { Linker.Crash(e, "Bad Proj No ItemGroups: " + DestProjAbsolutePath); }
        }
        Log.WriteLine("ok.");
      }
    }

    /// <summary> Reads XML of between the Linked zone placeholders. Does not add a <c>&lt;Root&gt;</c> element </summary>
    internal string ReadLinkedXml()
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


    private XComment FindCommentOrCrashIfDuplicatesFound(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in DestProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

      if (placeholders.Count > 1)
        Linker.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);

      return placeholders.FirstOrDefault();
    }


    internal void ClearAllExistingCodeExceptExplicitlyLinked()
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

    internal void ClearStartPlaceholderContent()
    {
      StartPlaceHolder.Value = Settings.StartPlaceholderComment;
      Log.WriteLine("Reset Includes & Excludes");
    }

    internal void AddExclusion(string exclusion)
    {
      StartPlaceHolder.Value += Environment.NewLine + Settings.ExcludePlaceholderLowerCase + " " + exclusion;
      Log.WriteLine("Excluded: " + exclusion);
    }

    internal void AddInclusion(string inclusion)
    {
      StartPlaceHolder.Value += Environment.NewLine + Settings.IncludePlaceholderLowerCase + " " + inclusion;
      Log.WriteLine("Included: " + inclusion);
    }

    internal void AddSource(string source)
    {
      StartPlaceHolder.Value += Environment.NewLine + Settings.SourcePlaceholderLowerCase + " " + source;
      Log.WriteLine("Added Source: " + source);
    }

    internal void Save()
    {
      DestProjXdoc.Save(DestProjAbsolutePath); 
      Log.WriteLine("Saved: " + DestProjAbsolutePath);
    }
  }
}
