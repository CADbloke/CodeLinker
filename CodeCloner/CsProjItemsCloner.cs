using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace CodeCloner
{
  /// <summary> Parses the Source CSPROJ and gets the string blob to paste into the Destination CSPROJ </summary>
  internal class CsProjItemsCloner
  {
    // private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

    /// <summary> Absolute pathname of the source CSPROJ. </summary>
    internal string sourceCsProjFileAbsolutePath { get; set; }
    /// <summary> Absolute pathname of the destination CSPROJ. </summary>
    internal string destCsProjFileAbsolutePath { get; set; }

    private XDocument sourceProjectDocument;
    private XDocument destProjectDocument;

    /// <summary> The ItemGroup Item elements to skip. </summary>
    static List<string> ItemElementsToSkip = new List<string> {"reference","projectreference","bootstrapperpackage"};

    /// <summary> It turns out file names with no quotes are of no interest to me. </summary>
    private static Regex itemFilePathWithQuotesRegex = new Regex(@"(?<=clude *?= *?"")(.+?)(?="")");

    /// <summary> Constructor with Destination <c>CSPROJ</c>. 
    ///           Source is either parsed from the desintation <c>CSPROJ</c> or specified later. </summary>
    /// <param name="destCsProj">  Destination <c>CSPROJ</c> - relative or absolute path. </param>
    public CsProjItemsCloner(string destCsProj)
    {
      destCsProjFileAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
    }

    /// <summary> Constructor with Source and Destination <c>CSPROJ</c>s. </summary>
    /// <param name="sourceCsProj">       Source <c>CSPROJ</c> - relative or absolute path. </param>
    /// <param name="destCsProj">  Destination <c>CSPROJ</c> - relative or absolute path. </param>
    public CsProjItemsCloner(string sourceCsProj, string destCsProj)
    {
      sourceCsProjFileAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceCsProj);
      destCsProjFileAbsolutePath   = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destCsProj);
    }


    /// <summary> Clones the source code from the source <c>CSPROJ</c> file to the destination <c>CSPROJ</c> file. 
    ///           Tweaks the file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void Clone()
    {
      if (string.IsNullOrEmpty(destCsProjFileAbsolutePath)) Program.Crash("ERROR: No destCsProjFileAbsolutePath. That's a bug.");

      DestinationCsProjParser destProjParser = new DestinationCsProjParser(destCsProjFileAbsolutePath);

      List<string> sourceProjPaths = new List<string>();

      if (!string.IsNullOrEmpty(sourceCsProjFileAbsolutePath)) {sourceProjPaths.Add(sourceCsProjFileAbsolutePath); }
      sourceProjPaths.AddRange(destProjParser.SourceCsProjList);

      foreach (string sourceProjPath in sourceProjPaths)
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


    private string newRelativeURL(string source, string destination)
    {
      if (source.StartsWith(@"$(")) { return source; /* source path starts with an Environment Variable  */ }
      if (File.Exists(source)) { return source; /* source path is an absolute path  */ }



      return "ohai";
    }



  }
}
