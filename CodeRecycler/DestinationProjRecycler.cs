using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;


namespace CodeRecycler
{
  /// <summary> Destination <c>Proj</c> Parser and Recycler. </summary>
  internal class DestinationProjRecycler
  {
    /// <summary> Absolute pathname of the destination <c>Proj</c> including file name. </summary>
    internal string DestProjAbsolutePath { get; }

    /// <summary> Absolute Directory of the destination <c>Proj</c>. NO file name. </summary>
    internal string DestProjDirectory { get; }

    private DestinationProjectXml destProjXml;

    /// <summary> Source <c>Proj</c>s defined in the Destination <c>Proj</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceProjList { get; }

    /// <summary> Code Files to be excluded from the recycle. </summary>
    internal List<string> ExclusionsList { get; }


    /// <summary> Source <c>Proj</c> is specified in the destination <c>Proj</c> XML comment placeholder. </summary>
    /// <param name="destProj"> Absolute path of destination <c>Proj</c>. </param>
    internal DestinationProjRecycler(string destProj)
    {
      DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
      DestProjDirectory    = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

      if (string.IsNullOrEmpty(DestProjAbsolutePath))
        Recycler.Crash("ERROR: No destProjFileAbsolutePath. That's a bug.");

      try { destProjXml = new DestinationProjectXml(DestProjAbsolutePath); }
      catch (Exception e)
      {
        Recycler.Crash(e, "DestinationProjRecycler CTOR (1 param) loading destination XML from " + DestProjAbsolutePath);
      }

      if (destProjXml.RootXelement == null || !destProjXml.RootXelement.Elements().Any())
        Recycler.Crash("ERROR: Bad Destination Proj file at " + DestProjAbsolutePath);

      SourceProjList = new List<string>();
      ExclusionsList = new List<string>();

      foreach (string line in destProjXml.StartPlaceHolder.Value.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None).ToList())
      {
        if (line.ToLower().Trim().StartsWith(Settings.SourcePlaceholderLowerCase))
        {
          string sourceInXml = line.ToLower().Replace(Settings.SourcePlaceholderLowerCase, "").Replace("-->", "").Trim();
          string absoluteSource = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(DestProjDirectory, sourceInXml);
          SourceProjList.Add(absoluteSource);
        }

        if (line.ToLower().Trim().StartsWith(Settings.ExcludePlaceholderLowerCase))
        { ExclusionsList.Add(line.ToLower().Replace(Settings.ExcludePlaceholderLowerCase, "").Trim().ToLower()); }
      }
    }


    /// <summary> Source <c>Proj</c> and destination <c>Proj</c> specified here. 
    ///           Source here overrides any sources specified in the <c>destProj</c></summary>
    /// <param name="sourceProj"> Absolute or Relative path of Source <c>Proj</c>. </param>
    /// <param name="destProj"> Absolute or Relative path of Destination <c>Proj</c>. </param>
    internal DestinationProjRecycler(string sourceProj, string destProj) :this(destProj)
    {
      SourceProjList = new List<string> {PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceProj)};
    }


    /// <summary> Recycles the source code from the source <c>Proj</c> file to the destination <c>Proj</c> file. 
    ///           Tweaks relative file paths so the project can find them. 
    ///           Adds a <c>&lt;Link&gt;</c> so you can edit within the destination project.</summary>
    internal void RecycleCode()
    {
      string oldXml = destProjXml.ReadRecycledXml();
      destProjXml.ClearOldRecycledCodeLinks();

      int totalCodezRecycled = 0;

      foreach (string sourcePath in SourceProjList)
      {
        int codezRecycled = 0;

        try
        {
          string sourceProjAbsolutePath = (PathMaker.IsAbsolutePath(sourcePath))
            ? sourcePath
            : Path.Combine(DestProjDirectory, sourcePath);

          string sourceProjDirectory = Path.GetDirectoryName(sourceProjAbsolutePath);
          string destDirectoryForRelativePath = DestProjDirectory.EndsWith("\\")
            ? DestProjDirectory
            : DestProjDirectory + "\\";
          string recycleRelativeSource = PathMaker.MakeRelativePath(destDirectoryForRelativePath , sourceProjAbsolutePath);

          SourceProjParser sourceProjParser = new SourceProjParser(sourceProjAbsolutePath);

          destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("Recycled from " + recycleRelativeSource));
          Log.WriteLine("Recycling from " + sourceProjAbsolutePath + Environment.NewLine + 
                        "            to " + DestProjAbsolutePath);


          foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
          {
            XElement destItemGroup = new XElement(Settings.MSBuild + "ItemGroup");

            foreach (XElement sourceItem in sourceItemGroup.Elements())
            {
              string elementName = sourceItem.Name.LocalName;
              if (Settings.ItemElementsToSkip.Contains(elementName.ToLower())) { continue; }

              XAttribute attrib = sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude");

              // bug: <Folder Include="..\cadblokefindreplace\  <---- WRONG

              if (attrib != null)
              {
                string originalPath = attrib.Value;
                string trimmedOriginalPath = originalPath.Trim().ToLower();

                if ( ExclusionsList.Any(x => Operators.LikeString(trimmedOriginalPath, x, CompareMethod.Text))) // OW my eyes!
                {
                  Log.WriteLine( 
                    "Excluded: " + originalPath           + Environment.NewLine + 
                    "    from: " + sourceProjAbsolutePath + Environment.NewLine + 
                    "because you said to Exclude: " + 
                    ExclusionsList.FirstOrDefault(x => Operators.LikeString(trimmedOriginalPath, x, CompareMethod.Text)) +
                    Environment.NewLine);
                  continue;
                }
                if (!PathMaker.IsAbsolutePath(originalPath))

                {
                  string sourceAbsolutePath = "";
                  try
                  {
                    string fileName = Path.GetFileName(originalPath); // wildcards blow up Path.GetFullPath()
                    string originalFolder = originalPath;
                    if (!string.IsNullOrEmpty(fileName)) originalFolder = originalPath.Replace(fileName, "");
                    sourceAbsolutePath = Path.GetFullPath(sourceProjDirectory + "\\" + originalFolder) + fileName;
                  }
                  catch (Exception e) {
                    Recycler.Crash(e, "Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalPath);
                  }

                  string relativePathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", sourceAbsolutePath);

                  if (!Settings.ItemElementsDoNotMakeRelativePath.Contains(elementName.ToLower()))
                    attrib.Value = relativePathFromDestination;
                }

                IEnumerable<XElement> links = sourceItem.Descendants(Settings.MSBuild + "Link");

                if (!(links.Any() || Settings.ItemElementsDoNotBreakLink.Contains(elementName.ToLower())))  // Folders, mostly
                {
                  XElement linkElement = new XElement(Settings.MSBuild + "Link", originalPath);
                  sourceItem.Add(linkElement);
                }
                destItemGroup.Add(sourceItem);
                codezRecycled++;
              }
            }
            if (destItemGroup.HasElements) { destProjXml.EndPlaceHolder.AddBeforeSelf(destItemGroup); }
          }
          destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("End Recycle from " + recycleRelativeSource+ Environment.NewLine + 
            "Recycled " + codezRecycled + " codez."));
          totalCodezRecycled += codezRecycled;
        }
        catch (Exception e) {
          Recycler.Crash(e, "Recycling " + sourcePath + " to " + DestProjAbsolutePath);
        }
      }


      destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("End of Recycled Code" + Environment.NewLine + 
        "See CodeRecyclerLog.txt for details. CodeRecycler by " + Help.SourceCodeUrl + " "));

      if (oldXml != destProjXml.ReadRecycledXml())
      {
        destProjXml.Save();
        Log.WriteLine("Recycled " + totalCodezRecycled + " codez from " + SourceProjList.Count + " source Project(s).");
      }
      else Log.WriteLine("No changes to save so nothing recycled.");

      Log.WriteLine("----------------------------");
      Log.WriteLine("");
    }
  }
}
