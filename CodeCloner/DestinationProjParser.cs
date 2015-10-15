using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace CodeCloner
{
  /// <summary> Destination <c>Proj</c> Parser and Cloner. </summary>
  internal class DestinationProjParser
  {
    private static List<string> ItemElementsToSkip = new List<string> {"reference", "projectreference", "bootstrapperpackage", "import"}; 
    private static List<string> ItemElementsDoNotBreakLink = new List<string> {"folder", "projectreference"};
    private static XNamespace MSBuild = "http://schemas.microsoft.com/developer/msbuild/2003";

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

    /// <summary> Code Files to be excluded from the clone. </summary>
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
      startPlaceHolder = FindCommentOrCrash("CodeCloner");
      endPlaceHolder = FindCommentOrCrash("EndCodeCloner");

      foreach (string line in startPlaceHolder.Value.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None).ToList())
      {
        if (line.ToLower().Trim().StartsWith("source:"))
        {
          string sourceInXml = line.ToLower().Replace("source:", "").Replace("-->", "").Trim();
          string absoluteSource = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(DestProjDirectory, sourceInXml);
          SourceProjList.Add(absoluteSource);
        }

        if (line.ToLower().Trim().StartsWith("exclude:")) { ExclusionsList.Add(line.Replace("exclude:", "").Trim()); }
      }

      CloneCode();
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

      startPlaceHolder = FindCommentOrCrash("CodeCloner");
      endPlaceHolder = FindCommentOrCrash("EndCodeCloner");

      CloneCode();
    }


    /// <summary> Clones the source code from the source <c>Proj</c> file to the destination <c>Proj</c> file. 
    ///           Tweaks relative file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void CloneCode()
    {
      if (string.IsNullOrEmpty(DestProjAbsolutePath)) { Program.Crash("ERROR: No destProjFileAbsolutePath. That's a bug."); }
      if (destProjXdoc.Root == null || !destProjXdoc.Root.Elements().Any()) {
        Program.Crash("ERROR: No Destination Proj file at " + DestProjAbsolutePath);
      }

      string oldXML = GetOrRemoveDestProjClonedCode(remove:true);
      int totalCodezCloned = 0;

      foreach (string sourcePath in SourceProjList)
      {
        int codezCloned = 0;

        try
        {
          string SourceProjAbsolutePath = (PathMaker.IsAbsolutePath(sourcePath))
            ? sourcePath
            : Path.Combine(DestProjDirectory, sourcePath);

          string SourceProjDirectory = Path.GetDirectoryName(SourceProjAbsolutePath);
          string destDirectoryForRelativePath = DestProjDirectory.EndsWith("\\")
            ? DestProjDirectory
            : DestProjDirectory + "\\";
          string cloneRelativeSource = PathMaker.MakeRelativePath(destDirectoryForRelativePath , SourceProjAbsolutePath);

          SourceProjParser sourceProjParser = new SourceProjParser(SourceProjAbsolutePath);

          endPlaceHolder.AddBeforeSelf(new XComment("Cloned from " + cloneRelativeSource));
          Log.WriteLine("Cloning from " + SourceProjAbsolutePath + Environment.NewLine + "to           " + DestProjAbsolutePath);


          foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
          {
            XElement destItemGroup = new XElement(MSBuild + "ItemGroup");

            foreach (XElement sourceItem in sourceItemGroup.Elements())
            {
              string elementName = sourceItem.Name.LocalName;
              if (ItemElementsToSkip.Contains(elementName.ToLower())) { continue; }

              XAttribute attrib = sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude");

              if (attrib != null)
              {
                string originalPath = attrib.Value;
                if (ExclusionsList.Any(x => originalPath.Contains(x)))
                {
                  Log.WriteLine( 
                    "Excluded :" + originalPath  +Environment.NewLine + 
                    "     from " + SourceProjAbsolutePath + Environment.NewLine + 
                    "because you said to Exclude: " + ExclusionsList.FirstOrDefault(x => originalPath.Contains(x)));
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
                    Program.Crash(e, "Cloning. GetFullPath: " + SourceProjDirectory + "\\" + originalPath);
                  }
                  string relativePathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", sourceAbsolutePath);
                  attrib.Value = relativePathFromDestination;
                }

                IEnumerable<XElement> links = sourceItem.Descendants(MSBuild + "Link");

                if (!(links.Any() || ItemElementsDoNotBreakLink.Contains(elementName.ToLower())))  // Folders, mostly
                {
                  XElement linkElement = new XElement(MSBuild + "Link", originalPath);
                  sourceItem.Add(linkElement);
                }
                destItemGroup.Add(sourceItem);
                codezCloned++;
              }
            }
            if (destItemGroup.HasElements) { endPlaceHolder.AddBeforeSelf(destItemGroup); }
          }
          endPlaceHolder.AddBeforeSelf(new XComment("End Clone from " + cloneRelativeSource+ Environment.NewLine + 
            "Cloned " + codezCloned + " codez."));
          totalCodezCloned += codezCloned;
        }
        catch (Exception e) {
          Program.Crash(e, "Cloning " + sourcePath + " to " + DestProjAbsolutePath);
        }
      }

      

      endPlaceHolder.AddBeforeSelf(new XComment("End of Cloned Code" + Environment.NewLine + 
        "See CodeClonerLog.txt for details. CodeCloner by " + Help.SourceCodeUrl + " "));

      if (oldXML != GetOrRemoveDestProjClonedCode())
      {
        destProjXdoc.Save(DestProjAbsolutePath);
        Log.WriteLine("Cloned " + totalCodezCloned + " codez from " + SourceProjList.Count + " source Project(s).");
      }
      else Log.WriteLine("No changes to save so nothing cloned.");

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


    private string GetOrRemoveDestProjClonedCode(bool remove = false)
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
      Program.Crash("Error: cannot get old Cloned Code from " + DestProjAbsolutePath);
      return "you'll never get this";
    }
  }
}
