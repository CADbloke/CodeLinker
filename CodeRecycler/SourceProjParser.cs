using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace CodeRecycler
{
  internal class SourceProjParser
  {
    /// <summary> Gets the full pathname of the source create structure project file. </summary>
    internal string SourceProjPath { get; }

    internal List<XElement> ItemGroups { get; }


    internal SourceProjParser(string sourceProjAbsolutePath)
    {
      SourceProjPath = sourceProjAbsolutePath;
      if (!File.Exists(sourceProjAbsolutePath))
        Recycler.Crash("ERROR: " + sourceProjAbsolutePath + "  does not exist.");

      if (!sourceProjAbsolutePath.IsaCsOrVbProjFile())
        Recycler.Crash("ERROR: " + sourceProjAbsolutePath + "  is not a C# or VB Proj.");

      try
      {
        XDocument ProjXml = XDocument.Load(sourceProjAbsolutePath);
        ItemGroups = new List<XElement>();

        XElement xElement = ProjXml.Element(Settings.MSBuild + "Project");
        if (xElement != null)
          ItemGroups.AddRange(xElement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));

        if (ItemGroups.Count == 0)
          Log.WriteLine("Curious: " + SourceProjPath + " contains no ItemGroups. No Codez?");
      }
      catch (Exception e)
      {
        Recycler.Crash(e, "source Proj: " + sourceProjAbsolutePath);
      }
    }
  }
}