using System;
using System.Collections.Generic;
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


    internal CsProjItemsCloner(string sourceCsProjFilePath = "Missing File Name")
    {
      sourceCsProjFile = sourceCsProjFilePath;
      sourceProjectDocument = XDocument.Load(sourceCsProjFilePath);
    }


    static List<string> ItemElementsToSkip = new List<string> {"reference","projectreference","bootstrapperpackage"};
    internal string Clone(string destCsProjFilePath)
    {
      if (sourceProjectDocument == null)
      {
        Log.WriteLine("ERROR: Source project:" + sourceCsProjFile + " is NULL.");
      }
      StringBuilder newXml = new StringBuilder();
      IEnumerable<XElement> sourceItemGroups = sourceProjectDocument.Descendants(MSBuildNamespace + "ItemGroup");

      foreach (XElement sourceItemGroup in sourceItemGroups)
      {
        StringBuilder newItemGroup = new StringBuilder();
        foreach (XElement xElement in sourceItemGroup.Elements())
        {
          string elementName =xElement.Name.LocalName;
          if (ItemElementsToSkip.Contains(elementName.ToLower())) { continue; }

          XmlReader reader = xElement.CreateReader(); // http://stackoverflow.com/a/659264/492
          reader.MoveToContent();
          string xml = reader.ReadInnerXml();
          
          Match filePathWithQuotes  = Regex.Match(xml, @"(?<=clude *?= *?"")(.+?)(?="")");  // (?<=clude *?= *?")(.+?)(?=")
          // Match filePathWithoutQuotes = Regex.Match(xml, @"(?<!Link>|<DependentUpon>|<LastGenOutput>)(?<=>)(.+\.[A-Za-z]+?)(?=<)"); // never matches

          if (!filePathWithQuotes.Success)
          {
            Log.WriteLine("WARNING: Regex did not find a file path in this xml...");
            Log.WriteLine(xml);
            Log.WriteLine("Bad Regex, no bone for you. Related: that's a bug, maybe.");
            continue;
          }


          newItemGroup.Append(xml);
        }
        newXml.Append(newItemGroup);
      }

      return newXml.ToString();
    }


    private string newRelativeURL(string source, string destination) { return "ohai"; }
  }
}
