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
    private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

    internal XDocument sourceProjectDocument { get; set; }
    private string sourceCsProjFile;

    /// <summary> Constructor. </summary>
    /// <param name="sourceCsProjFilePath"> Full pathname of the source <c>CSPROJ</c> project file. </param>
    internal CsProjItemsCloner(string sourceCsProjFilePath = "Missing File Name")
    {
      if (!Path.IsPathRooted(sourceCsProjFilePath)) sourceCsProjFilePath = Path.Combine(Environment.CurrentDirectory, sourceCsProjFilePath);
      sourceCsProjFile = sourceCsProjFilePath;
      sourceProjectDocument = XDocument.Load(sourceCsProjFilePath);
    }



    /// <summary> The ItemGroup Item elements to skip. </summary>
    static List<string> ItemElementsToSkip = new List<string> {"reference","projectreference","bootstrapperpackage"};

    /// <summary> It turns out file names with no quotes are of no interest to me. </summary>
    private static Regex filePathWithQuotesRegex = new Regex(@"(?<=clude *?= *?"")(.+?)(?="")");

    /// <summary> Clones the source code from the source <c>CSPROJ</c> file to the destination <c>CSPROJ</c> file. 
    ///           Tweaks the file paths so the project can find them. Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    /// <param name="destCsProjFilePath"> Full or relative pathname of the destination <c>CSPROJ</c> file. </param>
    internal void Clone(string destCsProjFilePath)
    {
      string destCsProjFile = destCsProjFilePath;
      if (sourceProjectDocument == null)
      {
        Log.WriteLine("ERROR: Source project:" + sourceCsProjFile + " is fricken NULL.");
        return;
      }

      if (!Path.IsPathRooted(destCsProjFile)) { destCsProjFile = Path.Combine(Environment.CurrentDirectory, destCsProjFile); }

      if (!File.Exists(destCsProjFile))
      {
        Log.WriteLine("ERROR: Destination project:" + destCsProjFile + " not found.");
        Log.WriteLine("It was specified as :" + destCsProjFilePath + ".");
        Log.WriteLine("Sad Trombone.");
        return;
      }

      string relativePathPrefix = MakeRelativePath(sourceCsProjFile , destCsProjFile);

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
          
          Match filePathWithQuotes  = filePathWithQuotesRegex.Match(xml);  

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
          xml = filePathWithQuotesRegex.Replace(xml, relativePathPrefix + filePathWithQuotes.Value);

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


    // http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/340454#340454
    /// <summary> Creates a relative path from one file or folder to another. </summary>
    /// <exception cref="ArgumentNullException">  . </exception>
    /// <param name="fromPath"> Contains the directory that defines the start of the relative path. </param>
    /// <param name="toPath">   Contains the path that defines the endpoint of the relative path. </param>
    /// <returns> The relative path from the start directory to the end path or <c>toPath</c> if the paths are not
    ///           related. </returns>
    /// ### <exception cref="UriFormatException">         . </exception>
    /// ### <exception cref="InvalidOperationException">  . </exception>
    private static String MakeRelativePath(String fromPath, String toPath)
    {
      if (String.IsNullOrEmpty(fromPath)) { throw new ArgumentNullException("fromPath"); }
      if (String.IsNullOrEmpty(toPath)) { throw new ArgumentNullException("toPath"); }

      Uri fromUri = new Uri(fromPath);
      Uri toUri = new Uri(toPath);

      if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (toUri.Scheme.ToUpperInvariant() == "FILE") {
        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      }

      return relativePath;
    }
  }
}
