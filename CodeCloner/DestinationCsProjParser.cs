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

    /// <summary> Absolute pathname of the destination <c>CSPROJ</c>. </summary>
    internal  string DestCsProjPath { get; }
    private XDocument csProjXml;
    private XComment  startPlaceHolder;
    private XComment  endPlaceHolder;
    
    /// <summary> Source <c>CSPROJ</c>s defined in the Destination <c>CSPROJ</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceCsProjList { get; }

    /// <summary> Source <c>CSPROJ</c> is specified in the destination <c>CSPROJ</c> XML comment placeholder. </summary>
    /// <param name="destCsProj"> Absolute path of destination <c>CSPROJ</c>. </param>
    internal  DestinationCsProjParser(string destCsProj)
    {
      DestCsProjPath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
    

      try { csProjXml = XDocument.Load(DestCsProjPath); }
      catch (Exception e) { Program.Crash(e); }

      startPlaceHolder = FindComment("CodeCloner");
      endPlaceHolder   = FindComment("EndCodeCloner");

      SourceCsProjList.Clear();

      foreach (string line in startPlaceHolder.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList())
      {
        if (!line.ToLower().Trim().StartsWith("source:")) continue;
        SourceCsProjList.Add(Path.Combine(DestCsProjPath, line.Replace("source:", "").Trim()));
      }
    }

    /// <summary> Source <c>CSPROJ</c> and destination <c>CSPROJ</c> specified here. </summary>
    /// <param name="sourceCsProj"> Absolute or Relative path of Source <c>CSPROJ</c>. </param>
    /// <param name="destCsProj"> Absolute or Relative path of Destination <c>CSPROJ</c>. </param>
    internal DestinationCsProjParser(string sourceCsProj, string destCsProj)
    {
      SourceCsProjList.Add(PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceCsProj));
      DestCsProjPath     = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
    }


    internal void RemoveOldDestCsProjClonedCode()
    {
      if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder))
      {
        XNode startNode = startPlaceHolder;
        while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }
      }
      else Program.Crash("Error: cannot remove old Cloned Code from " + DestCsProjPath);
    }
   



    /// <summary> Clones the source code from the source <c>CSPROJ</c> file to the destination <c>CSPROJ</c> file. 
    ///           Tweaks the file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void CloneCode()
    {
      if (string.IsNullOrEmpty(DestCsProjPath)) Program.Crash("ERROR: No destCsProjFileAbsolutePath. That's a bug.");


      foreach (string sourcePath in SourceCsProjList)
      {
        string sourceProjPath = sourcePath;
        if (!PathMaker.IsAbsolutePath(sourceProjPath)) sourceProjPath = Path.Combine(DestCsProjPath, sourceProjPath);

        SourceCsProjParser sourceProjParser = new SourceCsProjParser(sourceProjPath);

        string relativePathPrefix = PathMaker.MakeRelativePath(sourceProjParser.SourceCsProjPath, DestCsProjPath);

        IEnumerable<XElement> sourceItemGroups = sourceProjParser.ItemGroups;

        foreach (XElement sourceItemGroup in sourceItemGroups)
        {
          XElement destItemGroup = new XElement(sourceItemGroup);

          foreach (XElement item in destItemGroup.Elements())
          {
            string elementName = item.Name.LocalName;
            if (ItemElementsToSkip.Contains(elementName.ToLower())) { continue; }

            XAttribute attrib = item.Attribute("Include") ?? item.Attribute("Exclude");

            if (attrib != null)
            {
              string originalPath = attrib.Value;

              if (!PathMaker.IsAbsolutePath(originalPath))
              {
                string originalAbsolutePath = PathMaker.
                  MakeAbsolutePathFromPossibleRelativePathOrDieTrying(Path.GetDirectoryName(sourceProjPath), originalPath);
                
                attrib.Value = relativePathPrefix + attrib.Value; 
              }



              /* bug: actually, replacing the path is more complicated than that. 
          <Compile Include="..\CADbloke\Find    won't combine with a prefix and spit out a relative path
          maybe
          
          xml = itemFilePathWithQuotesRegex.Replace(xml, relativePathPrefix + filePathWithQuotes.Value);

         
          todo: add <Link> element before the closing tag of the item Closing tag. <Tag closes with </Tag> or />
          If closing tag is /> then it will have to become a real tag if the <Link> element is embedded.
          The link element is generally the SourcePath
          No link needed if the relative path prefix is "" - ie. the projects are in the same folder.
          */

              destItemGroup.Append(xml);
            }
          }
          destXml.Append(destItemGroup);
        }
      }

      // return destXml.ToString();
      // todo: replace the contents in the destinaton CSPROJ file and log it.
    }
    private XComment FindComment(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in csProjXml.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders  = comments.Where(c => c.Value.ToLower().StartsWith(commentStartsWith)).ToList();

      if (placeholders.Count != 1)
        Program.Crash("ERROR: " +DestCsProjPath+ "has " +placeholders.Count+" XML comments with" + commentStartsWith);

      return placeholders.First();
    }

  }
}
