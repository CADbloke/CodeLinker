using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace CodeCloner
{
  /// <summary> Destination <c>CSPROJ</c> Parser and Cloner. </summary>
  internal class DestinationCsProjParser
  {
    private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
    static List<string> ItemElementsToSkip = new List<string> {"reference","projectreference","bootstrapperpackage"};
    static List<string> ItemElementsDoNotLink = new List<string> {"folder"};

    /// <summary> Absolute pathname of the destination <c>CSPROJ</c> including file name. </summary>
    internal  string DestCsProjAbsolutePath { get; }
    /// <summary> Absolute Directory of the destination <c>CSPROJ</c>. NO file name. </summary>
    internal  string DestCsProjDirectory { get; }
    private XDocument destCsProjXdoc;
    private XComment  startPlaceHolder;
    private XComment  endPlaceHolder;
    
    /// <summary> Source <c>CSPROJ</c>s defined in the Destination <c>CSPROJ</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceCsProjList { get; }

    /// <summary> Source <c>CSPROJ</c> is specified in the destination <c>CSPROJ</c> XML comment placeholder. </summary>
    /// <param name="destCsProj"> Absolute path of destination <c>CSPROJ</c>. </param>
    internal  DestinationCsProjParser(string destCsProj)
    {
      DestCsProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
      DestCsProjDirectory = Path.GetDirectoryName(DestCsProjAbsolutePath);
    

      try { destCsProjXdoc = XDocument.Load(DestCsProjAbsolutePath); }
      catch (Exception e) { Program.Crash(e); }

      startPlaceHolder = FindCommentOrCrash("CodeCloner");
      endPlaceHolder   = FindCommentOrCrash("EndCodeCloner");

      SourceCsProjList.Clear();

      foreach (string line in startPlaceHolder.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList())
      {
        if (!line.ToLower().Trim().StartsWith("source:")) continue;
        SourceCsProjList.Add(Path.Combine(DestCsProjAbsolutePath, line.Replace("source:", "").Trim()));
      }
    }

    /// <summary> Source <c>CSPROJ</c> and destination <c>CSPROJ</c> specified here. </summary>
    /// <param name="sourceCsProj"> Absolute or Relative path of Source <c>CSPROJ</c>. </param>
    /// <param name="destCsProj"> Absolute or Relative path of Destination <c>CSPROJ</c>. </param>
    internal DestinationCsProjParser(string sourceCsProj, string destCsProj)
    {
      SourceCsProjList.Add(PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceCsProj));
      DestCsProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
      DestCsProjDirectory = Path.GetDirectoryName(DestCsProjAbsolutePath);
    }


    internal void RemoveOldDestCsProjClonedCode()
    {
      if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder))
      {
        XNode startNode = startPlaceHolder;
        while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }
      }
      else Program.Crash("Error: cannot remove old Cloned Code from " + DestCsProjAbsolutePath);
    }
   



    /// <summary> Clones the source code from the source <c>CSPROJ</c> file to the destination <c>CSPROJ</c> file. 
    ///           Tweaks the file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void CloneCode()
    {
      if (string.IsNullOrEmpty(DestCsProjAbsolutePath)) Program.Crash("ERROR: No destCsProjFileAbsolutePath. That's a bug.");
      if (!destCsProjXdoc.Root.Elements().Any()) Program.Crash("ERROR: No Destination CSPROJ file at " + DestCsProjAbsolutePath);

      RemoveOldDestCsProjClonedCode();
      XElement destinationInsertAfterHere = (XElement)startPlaceHolder.NextNode;

      foreach (string sourcePath in SourceCsProjList)
      {
        
        string sourceProjAbsolutePath = (PathMaker.IsAbsolutePath(sourcePath)) ? sourcePath : Path.Combine(DestCsProjDirectory, sourcePath);
        string sourceProjAbsoluteDirectory = Path.GetDirectoryName(sourceProjAbsolutePath);

        SourceCsProjParser sourceProjParser = new SourceCsProjParser(sourceProjAbsolutePath);

        XElement commentAboutSourceProject =  XElement.Parse("<!-- Cloned from "+ sourcePath + "-->");
        destinationInsertAfterHere.AddAfterSelf(commentAboutSourceProject);
        destinationInsertAfterHere = commentAboutSourceProject;

        string destRelativePathPrefix = PathMaker.MakeRelativePath(sourceProjAbsoluteDirectory, DestCsProjDirectory);

        foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
        {
          XElement destItemGroup = new XElement(sourceItemGroup);

          foreach (XElement item in destItemGroup.Elements())
          {
            string elementName = item.Name.LocalName;
            if (ItemElementsToSkip.Contains(elementName.ToLower())) continue;

            XAttribute attrib = item.Attribute("Include") ?? item.Attribute("Exclude");

            if (attrib != null)
            {
              string originalPath = attrib.Value;
              if (!PathMaker.IsAbsolutePath(originalPath)) attrib.Value = destRelativePathPrefix + originalPath;

              if (!item.Elements("Link").Any() && !ItemElementsDoNotLink.Contains(elementName.ToLower()))
                item.Add(XElement.Parse("<Link>" + originalPath + "</Link>"));
            }
          }
          destinationInsertAfterHere.AddAfterSelf(destItemGroup);
          destinationInsertAfterHere = destItemGroup;
        }
        XElement closingCommentAboutSourceProject =  XElement.Parse("<!-- End Clone from "+ sourcePath + "-->");
        destinationInsertAfterHere.AddAfterSelf(closingCommentAboutSourceProject);
        destinationInsertAfterHere = closingCommentAboutSourceProject;
      }
    }



    private XComment FindCommentOrCrash(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in destCsProjXdoc.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders  = comments.Where(c => c.Value.ToLower().StartsWith(commentStartsWith)).ToList();

      if (placeholders.Count != 1)
        Program.Crash("ERROR: " +DestCsProjAbsolutePath+ "has " +placeholders.Count+" XML comments with" + commentStartsWith);

      return placeholders.First();
    }

  }
}
