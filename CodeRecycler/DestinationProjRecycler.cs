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

    private DestinationProjXml destProjXml;

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

      try { destProjXml = new DestinationProjXml(DestProjAbsolutePath); }
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


    /// <summary> <c>sourceProj</c> here overrides any sources specified in the <c>destProj</c></summary>
    /// <param name="sourceProj"> Absolute or Relative path of Source <c>Proj</c>. </param>
    /// <param name="destProj"> Absolute or Relative path of Destination <c>Proj</c>. </param>
    internal DestinationProjRecycler(string sourceProj, string destProj) :this(destProj)
    {
      SourceProjList = new List<string> {PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceProj)};
    }


    /// <summary> Recycles the source code from the source <c>sourceProj</c> file to the destination <c>destProj</c> file.
    ///           <para> Tweaks relative file paths so the project can find them. </para>
    ///           Adds a <c>&lt;Link&gt;</c> for the destination project Solution Explorer.</summary>
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
          Log.WriteLine("Recycling from: " + sourceProjAbsolutePath + Environment.NewLine + 
                        "            to: " + DestProjAbsolutePath   + Environment.NewLine);


          foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
          {
            XElement newRecycledItemGroup = new XElement(Settings.MSBuild + "ItemGroup");

            foreach (XElement sourceItem in sourceItemGroup.Elements())
            {
              string sourceElementName = sourceItem.Name.LocalName;
              if (Settings.ItemElementsToSkip.Contains(sourceElementName.ToLower())) { continue; }

              XAttribute attrib = sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude");

              // bug: <Folder Include="..\cadblokefindreplace\  <---- WRONG

              if (attrib != null)
              {
                string originalSourcePath  = attrib.Value;
                string trimmedOriginalSourcePath = originalSourcePath.Trim().ToLower();

                IEnumerable<string> exclude = ExclusionsList 
                  .Where(x => Operators.LikeString(trimmedOriginalSourcePath, x, CompareMethod.Text)).ToList(); // OW my eyes!

                if ( exclude.Any() ) 
                {
                  Log.WriteLine( "Excluded: " + originalSourcePath     + Environment.NewLine + 
                                 "    from: " + sourceProjAbsolutePath + Environment.NewLine + 
                                 "because you said to Exclude: " + exclude.FirstOrDefault() + Environment.NewLine);
                  continue;
                }
                if (!PathMaker.IsAbsolutePath(originalSourcePath))

                {
                  string sourceAbsolutePath = "";
                  try
                  {
                    string sourceFileName = Path.GetFileName(originalSourcePath); // wildcards blow up Path.GetFullPath()
                    string originalFolder = originalSourcePath;
                    if (!string.IsNullOrEmpty(sourceFileName)) originalFolder = originalSourcePath.Replace(sourceFileName, "");
                    sourceAbsolutePath = Path.GetFullPath(sourceProjDirectory + "\\" + originalFolder) + sourceFileName;
                  }
                  catch (Exception e) {
                    Recycler.Crash(e, "Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                  }

                  string relativePathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", sourceAbsolutePath);

                  if (!Settings.ItemElementsDoNotMakeRelativePath.Contains(sourceElementName.ToLower()))
                    attrib.Value = relativePathFromDestination;
                }

                IEnumerable<XElement> links = sourceItem.Descendants(Settings.MSBuild + "Link");

                if (!(links.Any() || Settings.ItemElementsDoNotBreakLink.Contains(sourceElementName.ToLower())))  // Folders, mostly
                {
                  XElement linkElement = new XElement(Settings.MSBuild + "Link", originalSourcePath);
                  sourceItem.Add(linkElement);
                }
                newRecycledItemGroup.Add(sourceItem);
                codezRecycled++;
              }
            }
            if (newRecycledItemGroup.HasElements) { destProjXml.EndPlaceHolder.AddBeforeSelf(newRecycledItemGroup); }
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
      Log.WriteLine();
    }
  }
}
