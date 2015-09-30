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
  /// <summary> A create struct project parser. </summary>
  internal class DestinationCsProjParser
  {
    private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

    private string    destCsProjPath;
    private XDocument csProjXml;
    private XComment  startPlaceHolder;
    private XComment  endPlaceHolder;
    
    /// <summary> Source <c>CSPROJ</c>s defined in the Destination <c>CSPROJ</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceCsProjList { get; }

    /// <summary> Constructor. </summary>
    /// <param name="destCsProjAbsolutePath"> Absolute path of destination <c>CSPROJ</c>. </param>
    internal  DestinationCsProjParser(string destCsProj)
    {
      destCsProjPath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
    

      try { csProjXml = XDocument.Load(destCsProjPath); }
      catch (Exception e) { Program.Crash(e); }

      startPlaceHolder = FindComment("CodeCloner");
      endPlaceHolder   = FindComment("EndCodeCloner");

      SourceCsProjList.Clear();

      foreach (string line in startPlaceHolder.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList())
      {
        if (!line.ToLower().Trim().StartsWith("source:")) continue;
        SourceCsProjList.Add(line.Replace("source:", "").Trim());
      }
    }

    internal DestinationCsProjParser(string sourceCsProj, string destCsProj)
    {
      SourceCsProjList.Add(PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceCsProj));
      destCsProjPath     = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
    }


    internal void RemoveOldDestCsProjClonedCode()
    {
      if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder))
      {
        XNode startNode = startPlaceHolder;
        while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }
      }
      else Program.Crash("Error: cannot remove old Cloned Code from " + destCsProjPath);
    }
   
    /// <summary> Clones the source code from the source <c>CSPROJ</c> file to the destination <c>CSPROJ</c> file. 
    ///           Tweaks the file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void Clone()
    {
      if (string.IsNullOrEmpty(destCsProjPath)) Program.Crash("ERROR: No destCsProjFileAbsolutePath. That's a bug.");


      if (!string.IsNullOrEmpty(sourceCsProjFileAbsolutePath)) {SourceCsProjList.Add(sourceCsProjFileAbsolutePath); }
      SourceCsProjList.AddRange(destProjParser.SourceCsProjList);

      foreach (string sourceProjPath in SourceCsProjList)
      {
        SourceCsProjParser sourceProjParser = new SourceCsProjParser(sourceProjPath);
        
      }
      
     

      


      string relativePathPrefix = PathMaker.MakeRelativePath(sourceCsProjFileAbsolutePath , destCsProjFileAbsolutePath);

      StringBuilder destXml = new StringBuilder();
      IEnumerable<XElement> sourceItemGroups = sourceProjectDocument.Descendants(MSBuildNamespace + "ItemGroup");

      foreach (XElement sourceItemGroup in sourceItemGroups)
      {
        StringBuilder destItemGroup = new StringBuilder();
        foreach (XElement xElement in sourceItemGroup.Elements())
        {
          string elementName =xElement.Name.LocalName;
          if (ItemElementsToSkip.Contains(elementName.ToLower())) { continue; }

          XmlReader reader = xElement.CreateReader(); // http://stackoverflow.com/a/659264/492
          reader.MoveToContent();
          string xml = reader.ReadInnerXml();
          
          Match filePathWithQuotes  = itemFilePathWithQuotesRegex.Match(xml);  

          if (!filePathWithQuotes.Success)
          {
            Log.WriteLine("WARNING: Regex did not find a file path in this xml...");
            Log.WriteLine(xml);
            Log.WriteLine("Bad Regex, no bone for you. Related: that's a bug, maybe.");
            continue;
          }

          /* bug: actually, replacing the path is more complicated than that. 
          <Compile Include="..\CADbloke\Find    won't combine with a prefix and spit out a relative path
          maybe
          */
          xml = itemFilePathWithQuotesRegex.Replace(xml, relativePathPrefix + filePathWithQuotes.Value);

          /*
          todo: add <Link> element before the closing tag of the item Closing tag. <Tag closes with </Tag> or />
          If closing tag is /> then it will have to become a real tag if the <Link> element is embedded.
          The link element is generally the SourcePath
          No link needed if the relative path prefix is "" - ie. the projects are in the same folder.
          */

          destItemGroup.Append(xml);
        }
        destXml.Append(destItemGroup);
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
        Program.Crash("ERROR: " +destCsProjPath+ "has " +placeholders.Count+" XML comments with" + commentStartsWith);

      return placeholders.First();
    }

  }
}
