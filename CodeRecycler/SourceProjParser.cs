using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace CodeRecycler
{
  internal class SourceProjParser
  {
    private static XNamespace MSBuild = "http://schemas.microsoft.com/developer/msbuild/2003";

    /// <summary> Gets the full pathname of the source create structure project file. </summary>
    internal string SourceProjPath { get; }

    internal List<XElement> ItemGroups { get; }


    internal SourceProjParser(string sourceProjAbsolutePath)
    {
      SourceProjPath = sourceProjAbsolutePath;
      if (!File.Exists(sourceProjAbsolutePath)) { Program.Crash("ERROR: " + sourceProjAbsolutePath + "  does not exist."); }
      if (!sourceProjAbsolutePath.IsaCsOrVbProjFile())
        Program.Crash("ERROR: " + sourceProjAbsolutePath + "  is not a C# or VB Proj.");

      try
      {
        XDocument ProjXml = XDocument.Load(sourceProjAbsolutePath);
        ItemGroups = new List<XElement>();

        //IEnumerable<XElement> itemGroups = from element in ProjXml.Root.Elements().DescendantsAndSelf()
        //                                   where element.Name.LocalName == "Itemgroup" // .Attribute("name").Value
        //                                   select element;

        XElement xElement = ProjXml
          .Element(MSBuild + "Project");
        if (xElement != null)
        {
            IEnumerable<XElement> itemGroups = xElement
              .Elements(MSBuild + "ItemGroup")
              .Select(elements => elements);

            ItemGroups.AddRange(itemGroups);
          }

        if (ItemGroups.Count == 0) { Log.WriteLine("Curious: " + SourceProjPath + " contains no ItemGroups. No Codez?"); }
      }
      catch (Exception e) { Program.Crash(e, "source Proj: "+ sourceProjAbsolutePath); }
    }
  }
}