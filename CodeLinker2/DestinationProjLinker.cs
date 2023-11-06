// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace CodeLinker
{
    /// <summary> Destination <c> Proj </c> Parser and Linker. </summary>
    internal class DestinationProjLinker
    {
        private readonly DestinationProjXml destProjXml;

        /// <summary> Absolute pathname of the destination <c> Proj </c> including file name. </summary>
        internal string DestProjAbsolutePath { get; }

        /// <summary> Absolute Directory of the destination <c> Proj </c>. NO file name. </summary>
        internal string DestProjDirectory { get; }

        /// <summary> Source <c> Proj </c>s defined in the Destination <c> Proj </c> placeholder.
        ///     Can be zero, can be lots. </summary>
        internal List<ProjectLinkSettings> SourceProjList { get; } = new List<ProjectLinkSettings>();

        /// <summary> Source <c> Proj </c> is specified in the destination <c> Proj </c> XML comment placeholder. </summary>
        /// <param name="destProj"> Absolute path of destination <c> Proj </c>. </param>
        internal DestinationProjLinker(string destProj)
        {
            DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
            DestProjDirectory    = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

            if (string.IsNullOrEmpty(DestProjAbsolutePath))
                App.Crash("ERROR: No destProjFileAbsolutePath. That's a bug.");

            try
            {
                destProjXml = new DestinationProjXml(DestProjAbsolutePath);

                if (destProjXml.RootXelement == null || !destProjXml.RootXelement.Elements().Any())
                    App.Crash($"ERROR: Bad Destination Proj file at {DestProjAbsolutePath}. Root elemental null?: {destProjXml?.RootXelement == null}"
                            + $", Root elements count: {destProjXml.RootXelement?.Elements()?.Count()}");
            }
            catch (Exception e)
            {
                App.Crash(e, "DestinationProjLinker CTOR (1 param) loading destination XML from " + DestProjAbsolutePath);
            }

            ProjectLinkSettings projectLinkSettings = new ProjectLinkSettings();


            foreach (string line in destProjXml.StartPlaceHolder.Value.Split(new[] { "\r\n", "\n", Environment.NewLine }, StringSplitOptions.None).ToList())
            {
                try
                {
                    if (line.ToLower().Trim().StartsWith(Settings.SourcePlaceholderLowerCase, StringComparison.Ordinal))
                    {
                        string sourceInXml = line.Replace(Settings.SourcePlaceholderLowerCase, "").Replace(Settings.SourcePlaceholder, "").Replace("-->", "").Replace("\"", "").Trim();

                        string absoluteSource = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(DestProjDirectory, sourceInXml);
                        projectLinkSettings                           = new ProjectLinkSettings();
                        projectLinkSettings.SourceProjectAbsolutePath = absoluteSource;
                        SourceProjList.Add(projectLinkSettings);
                    }

                    else if (line.ToLower().Trim().StartsWith(Settings.ExcludePlaceholderLowerCase, StringComparison.Ordinal))
                        projectLinkSettings.ExclusionsList.Add(line.ToLower().Replace(Settings.ExcludePlaceholderLowerCase, "").Trim());

                    else if (line.ToLower().Trim().StartsWith(Settings.IncludePlaceholderLowerCase, StringComparison.Ordinal))
                        projectLinkSettings.InclusionsList.Add(line.ToLower().Replace(Settings.IncludePlaceholderLowerCase, "").Trim());

                    else if (line.ToLower().Trim().StartsWith(Settings.DestProjectFolderPrefixLowerCase, StringComparison.Ordinal))
                        projectLinkSettings.DestProjectFolderPrefix = line.Substring(line.IndexOf(':')+1).Trim().Trim('\\') + '\\';
                }
                catch (Exception e)
                {
                    App.Crash(e, $"broke parsing the Code Linker placeholder at: {line}");
                }
            }


            foreach (ProjectLinkSettings linker in SourceProjList)
            {
                if (!string.IsNullOrWhiteSpace(linker.DestProjectFolderPrefix))
                    Log.WriteLine($"Linked code will be in project folder: {linker.DestProjectFolderPrefix}");

                foreach (string exclusion in linker.ExclusionsList)
                    Log.WriteLine("exclusion: " + exclusion, ConsoleColor.DarkYellow);

                foreach (string inclusion in linker.InclusionsList)
                    Log.WriteLine("inclusion: " + inclusion, ConsoleColor.White, ConsoleColor.DarkGreen);

                Log.WriteLine();

                if (!linker.InclusionsList.Any())
                    linker.InclusionsList.Add("*"); // default wildcard match will include everything.}
            }
        }

        /// <summary> Links the source code from the source <c> sourceProj </c> file to the destination <c> destProj </c> file.
        ///     <para> Tweaks relative file paths so the project can find them. </para>
        ///     Adds a <c> &lt;Link&gt; </c> for the destination project Solution Explorer. </summary>
        internal void LinkCode()
        {
            string oldXml = destProjXml.ReadLinkedXml();
            destProjXml.ClearOldLinkedCode();

            var totalCodezLinked = 0;

            if (!SourceProjList?.Any() ?? true)
            {
                Log.WriteLine("[Warning] No Source Projects found. done.", ConsoleColor.Cyan);
                return;
            }

            foreach (ProjectLinkSettings sourceProject in SourceProjList)
            {

                string sourcePath              = sourceProject.SourceProjectAbsolutePath;
                HashSet<string> exclusionsListLowerCase = sourceProject.ExclusionsList;
                HashSet<string> inclusionsListLowerCase = sourceProject.InclusionsList;
                string destProjectFolderPrefix = sourceProject.DestProjectFolderPrefix;

                var codezLinked = 0;

                try
                {
                    string sourceProjAbsolutePath = PathMaker.IsAbsolutePath(sourcePath)
                                                        ? sourcePath
                                                        : Path.Combine(DestProjDirectory, sourcePath);

                    string sourceProjDirectory = Path.GetDirectoryName(sourceProjAbsolutePath);

                    string destDirectoryForRelativePath = DestProjDirectory.EndsWith("\\", StringComparison.Ordinal)
                                                              ? DestProjDirectory
                                                              : DestProjDirectory + "\\";

                    string linkRelativeSource = PathMaker.MakeRelativePath(destDirectoryForRelativePath, sourceProjAbsolutePath);

                    var sourceProjParser = new SourceProjLoader(sourceProjAbsolutePath);

                    destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("Linked from " + linkRelativeSource));
                    Log.WriteLine("Recycling from: " + sourceProjAbsolutePath + Environment.NewLine +
                                  "            to: " + DestProjAbsolutePath + Environment.NewLine, ConsoleColor.Cyan);

                    List<string> alreadyIncluded = (from sourceItemsGroup in destProjXml.ItemGroups
                                                    from sourceItems in sourceItemsGroup.Elements()
                                                    where !Settings.ItemElementsToSkip?.Contains(sourceItems.Name.LocalName.ToLower()) ?? false
                                                    select sourceItems.Attribute("Include") ?? sourceItems.Attribute("Exclude")
                                                    into attribute
                                                    where attribute != null
                                                    select attribute.Value.ToLower()).ToList();


                    foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
                    {
                        var newLinkedItemGroup = new XElement(Settings.MSBuild + "ItemGroup");

                        foreach (XElement sourceItem in sourceItemGroup.Elements())
                        {
                            string sourceElementName = sourceItem.Name.LocalName;

                            if (Settings.ItemElementsToSkip.Contains(sourceElementName.ToLower()))
                                continue;

                            XAttribute attrib = sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude");

                            if (attrib == null)
                                continue; // these are not the droids

                            string originalSourcePath = attrib.Value;
                            string trimmedOriginalSourcePath = originalSourcePath.Trim();

                            if (alreadyIncluded.Contains(trimmedOriginalSourcePath, StringComparer.OrdinalIgnoreCase))
                            {
                                Log.WriteLine("Skipped: " + originalSourcePath + Environment.NewLine +
                                              "    from: " + sourceProjAbsolutePath + Environment.NewLine +
                                              "because there is a file with the same path in the destination project." + Environment.NewLine, ConsoleColor.Gray);
                                continue;
                            }

                            List<string> include = inclusionsListLowerCase.Where(i => Operators.LikeString(trimmedOriginalSourcePath, i, CompareMethod.Text)).ToList();
                            //                                                               OW my eyes!

                            string sourceFileName = Path.GetFileName(trimmedOriginalSourcePath); // wildcards blow up Path.GetFullPath()
                            string originalFolder = Path.GetDirectoryName(originalSourcePath);
                            List<string> exclusions = exclusionsListLowerCase.Where(x => x != null
                                                                             && (
                                                                                    Operators.LikeString(sourceFileName    .ToLower(), "*" + x, CompareMethod.Text)
                                                                               ||   Operators.LikeString(originalSourcePath.ToLower(), "*" + x, CompareMethod.Text)
                                                                                 ) ).ToList();
                            if (exclusions.Any())
                            {
                                Log.WriteLine("Excluded: "                    + originalSourcePath     + Environment.NewLine +
                                              "    from: "                    + sourceProjAbsolutePath + Environment.NewLine +
                                              "because you said to Exclude: " + exclusions.FirstOrDefault(), ConsoleColor.DarkYellow);
                                continue;
                            }

                            if (!inclusionsListLowerCase.Any() || include.Any()) // empty inclusions list means include everything unless explicity excluded
                            {
                                if (!PathMaker.IsAbsolutePath(originalSourcePath))
                                {
                                    string sourcePathFromDestination = "";

                                    try
                                    {
                                        string sourceAbsolutePath = Path.GetFullPath(sourceProjDirectory?.TrimEnd('\\') + "\\" + originalFolder?.TrimEnd('\\')) + "\\" + sourceFileName;


                                        sourcePathFromDestination = (PathMaker.MakeRelativePath(DestProjDirectory.Trim('\\') + "\\", sourceAbsolutePath))?.Replace(@"\\", @"\");

                                        /*
                                        Log.WriteLine($"dest proj directory ....   {DestProjDirectory}");
                                        Log.WriteLine($"original source path....   {originalSourcePath}");
                                        Log.WriteLine($"original folder ........   {originalFolder}");
                                        Log.WriteLine($"source  proj directory..   {sourceProjDirectory}");
                                        Log.WriteLine($"sourceFileName .........   {sourceFileName}");
                                        Log.WriteLine($"sourceAbsolutePath .....   {sourceAbsolutePath}");
                                        Log.WriteLine($"source path ........       {sourcePathFromDestination}");
                                        */

                                    }

                                    catch (ArgumentException badArg)
                                    {
                                        // Log.WriteException(badArg); 
                                        if (badArg.Message.Contains("Illegal characters in path"))
                                        {
                                            if (Regex.IsMatch(originalSourcePath, @"^[a-zA-Z]:\\")) // is already an absolute path
                                                sourcePathFromDestination = originalSourcePath;

                                            else if (Regex.IsMatch(originalSourcePath, @"\*\*")) // it is **\*.* or something  awkward like that.
                                            {
                                                string relativeFolderPathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory?.TrimEnd('\\') + "\\", sourceProjDirectory);
                                                sourcePathFromDestination = (relativeFolderPathFromDestination?.TrimEnd('\\') + "\\" + originalSourcePath)?.Replace(@"\\", @"\");
                                                var linkElement = new XElement(Settings.MSBuild + "Link", destProjectFolderPrefix + @"%(RecursiveDir)%(Filename)%(Extension)");
                                                sourceItem.Add(linkElement);
                                            } // wtf: I bet that's a bug
                                            else
                                            {
                                                try
                                                {
                                                    string originalFolderrr = originalSourcePath.Substring(0, originalSourcePath.LastIndexOf("\\", StringComparison.Ordinal));
                                                    string relativeFolderPathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory?.TrimEnd('\\') + "\\", originalFolderrr);
                                                    sourcePathFromDestination = (relativeFolderPathFromDestination + originalSourcePath.Substring(originalSourcePath.LastIndexOf("\\", StringComparison.Ordinal)))
                                                    ?.Replace(@"\\", @"\");
                                                }
                                                catch (Exception e)
                                                {
                                                    App.Crash(e, "FAILed Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                string originalFolderr = originalSourcePath.Substring(0, originalSourcePath.LastIndexOf("\\"));
                                                string relativeFolderPathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", originalFolderr);
                                                sourcePathFromDestination = (relativeFolderPathFromDestination + originalSourcePath.Substring(originalSourcePath.LastIndexOf("\\")))
                                                ?.Replace(@"\\", @"\");
                                            }
                                            catch (Exception e)
                                            {
                                                App.Crash(e, "FAILed Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        App.Crash(e, "FAILed Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                                    }

                                    if (!Settings.ItemElementsDoNotMakeRelativePath.Contains(sourceElementName.ToLower()))
                                    {
                                        attrib.Value = sourcePathFromDestination;
                                        destProjXml.Keepers.RemoveAll(k => k.Attribute("Include").Value.Contains(sourcePathFromDestination));
                                    }
                                }

                                XElement[] links = sourceItem.Descendants(Settings.MSBuild + "Link").ToArray();

                                // Folders, mostly
                                if (!(links.Any() || Settings.ItemElementsDoNotBreakLink.Contains(sourceElementName.ToLower())))
                                {
                                    var linkElement = new XElement(Settings.MSBuild + "Link", destProjectFolderPrefix + originalSourcePath);
                                    sourceItem.Add(linkElement);
                                    Log.WriteLine($"linking {originalSourcePath}", ConsoleColor.DarkGreen, ConsoleColor.Black);
                                }

                                newLinkedItemGroup.Add(sourceItem);
                                codezLinked++;
                                alreadyIncluded.Add(originalSourcePath);
                            }
                            else
                            {
                                Log.WriteLine("Excluded: " + originalSourcePath + Environment.NewLine +
                                              "    from: " + sourceProjAbsolutePath + Environment.NewLine +
                                              "because it did not match anything on the Include: list " + Environment.NewLine, ConsoleColor.Gray, ConsoleColor.DarkRed);
                            }
                        }

                        if (newLinkedItemGroup.HasElements)
                            destProjXml.EndPlaceHolder.AddBeforeSelf(newLinkedItemGroup);
                    }

                    destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("End Link from " + linkRelativeSource + Environment.NewLine +
                                                                          "Linked " + codezLinked + " codez."));
                    totalCodezLinked += codezLinked;
                    // so we don't link things twice...
                    destProjXml.Keepers.RemoveAll(k => k.Attribute("Include").Value.Contains(sourceProjDirectory));
                    

                    // copy Source project Resources so *.resx files don't break. Warning: Last one wins so weird race condition lives here
                    string sourceResources = sourceProjDirectory + "\\Resources";

                    if (Directory.Exists(sourceResources))
                    {
                        string destResourcesPath = DestProjDirectory + "\\Resources";
                        Log.WriteLine($"Copying all Resource Files from {sourceResources} to {destResourcesPath}.", ConsoleColor.Green);

                        if (!Directory.Exists(destResourcesPath))
                            Directory.CreateDirectory(destResourcesPath);

                        foreach (string sourceFile in Directory.GetFiles(sourceResources))
                        {
                            string excluded = exclusionsListLowerCase.FirstOrDefault(e => e == sourceFile.ToLower() || e == sourceFile);

                            if (excluded != null)
                            {
                                Log.WriteLine($"Excluded:          {sourceFile}{Environment.NewLine}" + 
                                              $"from: Resources in Destination Project{Environment.NewLine}"
                                            + $"because you said to Exclude:{excluded} ", ConsoleColor.White, ConsoleColor.DarkRed);
                            }

                            string destFile = destResourcesPath + "\\" + Path.GetFileName(sourceFile);
                            if (File.Exists(destFile))
                            {
                                long sourceSize = new FileInfo(sourceFile).Length;
                                long destSize = new FileInfo(destFile).Length;
                                if (sourceSize != destSize) // ie. it is probably a different file.
                                {
                                    Log.WriteLine($"[Warning] Overwriting {destFile}", ConsoleColor.Red);
                                    Log.WriteLine($"Source: {sourceSize} bytes, Dest: {destSize} Bytes.", ConsoleColor.Yellow);
                                }
                            }

                            File.Copy(sourceFile, Path.Combine(destResourcesPath, Path.GetFileName(sourceFile)), true);
                            Log.WriteLine($"Copied {sourceFile}", ConsoleColor.Green);
                        }

                        Log.WriteLine($"Copied all Files from {sourceResources} to {destResourcesPath}." + Environment.NewLine, ConsoleColor.Green);
                    }
                }
                catch (Exception e)
                {
                    App.Crash(e, "Recycling " + sourcePath + " to " + DestProjAbsolutePath);
                }
            } // end foreach source project


            destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("End of Linked Code" /*+ DateTime.Now.ToString("U") */+ Environment.NewLine +
                                                                  "See CodeLinkerLog.txt for details. CodeLinker by " + Settings.SourceCodeUrl ));

            bool hasChanged = oldXml != destProjXml.ReadLinkedXml();

            List<string> keeperExclusions = new List<string>();

            foreach (ProjectLinkSettings projectLinkSettingse in SourceProjList)
                keeperExclusions.AddRange(projectLinkSettingse.ExclusionsList);

            destProjXml.PreserveKeepersAndReport(keeperExclusions);

            if (hasChanged)
            {
                destProjXml.Save();
                Log.WriteLine("Linked " + totalCodezLinked + " codez from " + SourceProjList.Count + " source Project(s).", ConsoleColor.Green);
            }
            else
                Log.WriteLine("No changes, didn't save.", ConsoleColor.Cyan, ConsoleColor.DarkBlue);

            Log.WriteLine("-------------------------------------------------------", ConsoleColor.DarkGray);
            Log.WriteLine();
        }
    }
}
