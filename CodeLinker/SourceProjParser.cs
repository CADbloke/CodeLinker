using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace CodeLinker
{
  /// <summary> Parses the <c>&lt;ItemGroup&gt;</c>s from the Source Project. Read Only. Never writes. </summary>
  internal class SourceProjParser
  {
    /// <summary> Full pathname of the source project file. </summary>
    internal string SourceProjPath { get; }

    /// <summary> Gets the <c>&lt;ItemGroup&gt;</c>s from the Source Project. </summary>
    internal List<XElement> ItemGroups { get; }


    internal SourceProjParser(string sourceProjAbsolutePath)
    {
      SourceProjPath = sourceProjAbsolutePath;
      if (!File.Exists(sourceProjAbsolutePath))
      {
        App.Crash("ERROR: " + sourceProjAbsolutePath + "  does not exist.");
      }

      if (!sourceProjAbsolutePath.IsaCsOrVbProjFile())
      {
        App.Crash("ERROR: " + sourceProjAbsolutePath + "  is not a C# or VB Proj.");
      }

      try
      {
        XDocument ProjXml = XDocument.Load(sourceProjAbsolutePath);
        ItemGroups = new List<XElement>();

        XElement xElement = ProjXml.Element(Settings.MSBuild + "Project");
        if (xElement != null)
        {
          ItemGroups.AddRange(xElement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));
        }

        if (ItemGroups.Count == 0)
        {
          Log.WriteLine("Curious: " + SourceProjPath + " contains no ItemGroups. No Codez?");
        }
      }
      catch (Exception e)
      {
        App.Crash(e, "source Proj: " + sourceProjAbsolutePath);
      }
    }
  }
}