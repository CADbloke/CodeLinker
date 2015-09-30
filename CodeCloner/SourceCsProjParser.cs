using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;


namespace CodeCloner
{
  internal class SourceCsProjParser
  {
    private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

    /// <summary> Gets the full pathname of the source create structure project file. </summary>
    internal string SourceCsProjPath { get; }
    private XDocument csProjXml;
    internal List<XElement> ItemGroups { get; }


    internal SourceCsProjParser(string sourceCsProjAbsolutePath)
    {
      SourceCsProjPath = sourceCsProjAbsolutePath;
      if (!File.Exists(sourceCsProjAbsolutePath)) { Program.Crash("ERROR: " + sourceCsProjAbsolutePath + "  does not exist."); }
      if (!sourceCsProjAbsolutePath.ToLower().EndsWith(".csproj")) {
        Program.Crash("ERROR: " + sourceCsProjAbsolutePath + "  is not a CSPROJ.");
      }

      try
      {
        csProjXml = XDocument.Load(sourceCsProjAbsolutePath);
        ItemGroups.Clear();
        IEnumerable<XElement> itemGroups = from element in csProjXml.Root.Elements().DescendantsAndSelf()
                                           where element.Name.LocalName == "Itemgroup" // .Attribute("name").Value
                                           select element;
        ItemGroups.AddRange(itemGroups);

        if (ItemGroups.Count == 0) { Log.WriteLine("Curious: " + SourceCsProjPath + " contains no ItemGroups. No Codez?"); }
      }
      catch (Exception e) { Program.Crash(e); }
    }




    
  }
}