using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace CodeRecycler
{
  /// <summary> Destination <c>Proj</c> Parser and Recycler. </summary>
  internal class DestinationProjParser
  {
    /// <summary> Absolute pathname of the destination <c>Proj</c> including file name. </summary>
    internal string DestProjAbsolutePath { get; }

    /// <summary> Absolute Directory of the destination <c>Proj</c>. NO file name. </summary>
    internal string DestProjDirectory { get; }

    private XDocument destProjXdoc;
    private XComment startPlaceHolder;
    private XComment endPlaceHolder;

    /// <summary> Source <c>Proj</c>s defined in the Destination <c>Proj</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceProjList { get; }

    /// <summary> Code Files to be excluded from the recycle. </summary>
    internal List<string> ExclusionsList { get; }


    /// <summary> Source <c>Proj</c> is specified in the destination <c>Proj</c> XML comment placeholder. </summary>
    /// <param name="destProj"> Absolute path of destination <c>Proj</c>. </param>
    internal DestinationProjParser(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      try { destProjXdoc = XDocument.Load(DestProjAbsolutePath); }
      catch (Exception e)
      {
        Program.Crash(e, "DestinationProjParser CTOR (1 param) loading destination XML from " + DestProjAbsolutePath);
      }


      SourceProjList = new List<string>();
      ExclusionsList = new List<string>();
      startPlaceHolder = FindCommentOrCrash(Settings.StartPlaceholderComment);
      endPlaceHolder   = FindCommentOrCrash(Settings.EndPlaceholderComment);

      foreach (string line in startPlaceHolder.Value.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None).ToList())
      {
        if (line.ToLower().Trim().StartsWith(Settings.SourcePlaceholderLowerCase))
        {
          string sourceInXml = line.ToLower().Replace(Settings.SourcePlaceholderLowerCase, "").Replace("-->", "").Trim();
          string absoluteSource = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(DestProjDirectory, sourceInXml);
          SourceProjList.Add(absoluteSource);
        }

        if (line.ToLower().Trim().StartsWith(Settings.ExcludePlaceholderLowerCase))
        { ExclusionsList.Add(line.ToLower().Replace(Settings.ExcludePlaceholderLowerCase, "").Trim().ToLower()); }
      }

      RecycleCode();
    }


    /// <summary> Source <c>Proj</c> and destination <c>Proj</c> specified here. </summary>
    /// <param name="sourceProj"> Absolute or Relative path of Source <c>Proj</c>. </param>
    /// <param name="destProj"> Absolute or Relative path of Destination <c>Proj</c>. </param>
    internal DestinationProjParser(string sourceProj, string destProj)
    {
      SourceProjList = new List<string> {PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceProj)};
      // bug: whut ?
      ExclusionsList = new List<string>();

      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath);

      try { destProjXdoc = XDocument.Load(DestProjAbsolutePath); }
      catch (Exception e)
      {
        Program.Crash(e, "DestinationProjParser CTOR (2 params) loading destination XML from " + DestProjAbsolutePath);
      }

      startPlaceHolder = FindCommentOrCrash(Settings.StartPlaceholderComment);  
      endPlaceHolder   = FindCommentOrCrash(Settings.EndPlaceholderComment);   

      RecycleCode();
    }


    /// <summary> Recycles the source code from the source <c>Proj</c> file to the destination <c>Proj</c> file. 
    ///           Tweaks relative file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void RecycleCode()
    {
      if (string.IsNullOrEmpty(DestProjAbsolutePath)) { Program.Crash("ERROR: No destProjFileAbsolutePath. That's a bug."); }
      if (destProjXdoc.Root == null || !destProjXdoc.Root.Elements().Any()) {
        Program.Crash("ERROR: No Destination Proj file at " + DestProjAbsolutePath);
      }

      string oldXML = GetOrRemoveDestProjRecycledCode(remove:true);
      int totalCodezRecycled = 0;

      foreach (string sourcePath in SourceProjList)
      {
        int codezRecycled = 0;

        try
        {
          string SourceProjAbsolutePath = (PathMaker.IsAbsolutePath(sourcePath))
            ? sourcePath
            : Path.Combine(DestProjDirectory, sourcePath);

          string SourceProjDirectory = Path.GetDirectoryName(SourceProjAbsolutePath);
          string destDirectoryForRelativePath = DestProjDirectory.EndsWith("\\")
            ? DestProjDirectory
            : DestProjDirectory + "\\";
          string recycleRelativeSource = PathMaker.MakeRelativePath(destDirectoryForRelativePath , SourceProjAbsolutePath);

          SourceProjParser sourceProjParser = new SourceProjParser(SourceProjAbsolutePath);

          endPlaceHolder.AddBeforeSelf(new XComment("Recycled from " + recycleRelativeSource));
          Log.WriteLine("Recycling from " + SourceProjAbsolutePath + Environment.NewLine + "to           " + DestProjAbsolutePath);


          foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
          {
            XElement destItemGroup = new XElement(Settings.MSBuild + "ItemGroup");

            foreach (XElement sourceItem in sourceItemGroup.Elements())
            {
              string elementName = sourceItem.Name.LocalName;
              if (Settings.ItemElementsToSkip.Contains(elementName.ToLower())) { continue; }

              XAttribute attrib = sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude");

              if (attrib != null)
              {
                string originalPath = attrib.Value;
                if (ExclusionsList.Any(x => originalPath.ToLower().Contains(x)))
                {
                  Log.WriteLine( 
                    "Excluded :" + originalPath  +Environment.NewLine + 
                    "     from " + SourceProjAbsolutePath + Environment.NewLine + 
                    "because you said to Exclude: " + ExclusionsList.FirstOrDefault(x => originalPath.ToLower().Contains(x)));
                  continue;
                }
                if (!PathMaker.IsAbsolutePath(originalPath))
                 
                {
                  string sourceAbsolutePath = "";
                  try
                  {
                    string fileName = Path.GetFileName(originalPath); // wildcards blow up Path.GetFullPath()
                    string originalFolder = originalPath;
                    if (!string.IsNullOrEmpty(fileName))  originalFolder = originalPath.Replace(fileName, "");
                    sourceAbsolutePath = Path.GetFullPath(SourceProjDirectory + "\\" + originalFolder) + fileName;
                  }
                  catch (Exception e)
                  {
                    Program.Crash(e, "Recycling. GetFullPath: " + SourceProjDirectory + "\\" + originalPath);
                  }
                  string relativePathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", sourceAbsolutePath);
                  attrib.Value = relativePathFromDestination;
                }

                IEnumerable<XElement> links = sourceItem.Descendants(Settings.MSBuild + "Link");

                if (!(links.Any() || Settings.ItemElementsDoNotBreakLink.Contains(elementName.ToLower())))  // Folders, mostly
                {
                  XElement linkElement = new XElement(Settings.MSBuild + "Link", originalPath);
                  sourceItem.Add(linkElement);
                }
                destItemGroup.Add(sourceItem);
                codezRecycled++;
              }
            }
            if (destItemGroup.HasElements) { endPlaceHolder.AddBeforeSelf(destItemGroup); }
          }
          endPlaceHolder.AddBeforeSelf(new XComment("End Recycle from " + recycleRelativeSource+ Environment.NewLine + 
            "Recycled " + codezRecycled + " codez."));
          totalCodezRecycled += codezRecycled;
        }
        catch (Exception e) {
          Program.Crash(e, "Recycling " + sourcePath + " to " + DestProjAbsolutePath);
        }
      }

      

      endPlaceHolder.AddBeforeSelf(new XComment("End of Recycled Code" + Environment.NewLine + 
        "See CodeRecyclerLog.txt for details. CodeRecycler by " + Help.SourceCodeUrl + " "));

      if (oldXML != GetOrRemoveDestProjRecycledCode())
      {
        destProjXdoc.Save(DestProjAbsolutePath);
        Log.WriteLine("Recycled " + totalCodezRecycled + " codez from " + SourceProjList.Count + " source Project(s).");
      }
      else Log.WriteLine("No changes to save so nothing recycled.");

      Log.WriteLine("----------------------------");
      Log.WriteLine("");
    }


    private XComment FindCommentOrCrash(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in destProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

      if (placeholders.Count != 1) {
        Program.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);
      }

      return placeholders.First();
    }


    private string GetOrRemoveDestProjRecycledCode(bool remove = false)
    {
      StringBuilder oldXmlBuilder = new StringBuilder();
      if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder))
      {
        XNode startNode = startPlaceHolder;
        while (startNode.NextNode != endPlaceHolder)
        {
          oldXmlBuilder.Append(startNode.NextNode.ToString());
          if (remove) startNode.NextNode.Remove();
          else startNode = startNode.NextNode;
        }
        return oldXmlBuilder.ToString();
      }
      Program.Crash("Error: cannot get old Recycled Code from " + DestProjAbsolutePath);
      return "you'll never get this";
    }
  }
}
