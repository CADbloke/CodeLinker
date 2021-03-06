﻿// Code Linker originally by @CADbloke (Ewen Wallace) 2015
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
        private static string _linkPrefix = "";
        internal static string LinkPrefix
        {
            get
            {
                return _linkPrefix;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _linkPrefix = string.Empty;
                else
                    _linkPrefix = value.EndsWith("\\", StringComparison.Ordinal)
                                                ? value
                                                : value + "\\";
            }
        }
        
        private readonly DestinationProjXml destProjXml;

        /// <summary> Source <c> Proj </c> is specified in the destination <c> Proj </c> XML comment placeholder. </summary>
        /// <param name="destProj"> Absolute path of destination <c> Proj </c>. </param>
        internal DestinationProjLinker(string destProj)
        {
            DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
            DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

            if (string.IsNullOrEmpty(DestProjAbsolutePath))
                App.Crash("ERROR: No destProjFileAbsolutePath. That's a bug.");

            try
            {
                destProjXml = new DestinationProjXml(DestProjAbsolutePath);
            }
            catch (Exception e)
            {
                App.Crash(e, "DestinationProjLinker CTOR (1 param) loading destination XML from " + DestProjAbsolutePath);
            }

            if (destProjXml.RootXelement == null || !destProjXml.RootXelement.Elements().Any())
                App.Crash("ERROR: Bad Destination Proj file at " + DestProjAbsolutePath);

            SourceProjList = new List<string>();
            ExclusionsList = new List<string>();
            InclusionsList = new List<string>();

            foreach (string line in
                    destProjXml.StartPlaceHolder.Value.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None).ToList())
            {
                if (line.ToLower().Trim().StartsWith(Settings.SourcePlaceholderLowerCase))
                {
                    string sourceInXml = line.ToLower().Replace(Settings.SourcePlaceholderLowerCase, "").Replace("-->", "").Trim();
                    string absoluteSource = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(DestProjDirectory, sourceInXml);
                    SourceProjList.Add(absoluteSource);
                }

                if (line.ToLower().Trim().StartsWith(Settings.ExcludePlaceholderLowerCase))
                    ExclusionsList.Add(line.ToLower().Replace(Settings.ExcludePlaceholderLowerCase, "").Trim().ToLower());

                if (line.ToLower().Trim().StartsWith(Settings.IncludePlaceholderLowerCase))
                    InclusionsList.Add(line.ToLower().Replace(Settings.IncludePlaceholderLowerCase, "").Trim().ToLower());
            }

            foreach (string exclusion in ExclusionsList)
                Log.WriteLine("exclusion: "+ exclusion , ConsoleColor.Gray, ConsoleColor.DarkRed);

            foreach (string inclusion in InclusionsList)
                Log.WriteLine("inclusion: " + inclusion, ConsoleColor.White, ConsoleColor.DarkGreen);

            if (InclusionsList == null || !InclusionsList.Any())
                InclusionsList.Add("*"); // default wildcard match will include everything.
        }

        /// <summary> <c> sourceProj </c> here overrides any sources specified in the <c> destProj </c> </summary>
        /// <param name="sourceProj"> Absolute or Relative path of Source <c> Proj </c>. </param>
        /// <param name="destProj"> Absolute or Relative path of Destination <c> Proj </c>. </param>
        internal DestinationProjLinker(string sourceProj, string destProj) : this(destProj)
        {
            SourceProjList = new List<string>
                {
                    PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceProj)
                };
        }

        /// <summary> Absolute pathname of the destination <c> Proj </c> including file name. </summary>
        internal string DestProjAbsolutePath { get; }

        /// <summary> Absolute Directory of the destination <c> Proj </c>. NO file name. </summary>
        internal string DestProjDirectory { get; }

        /// <summary> Source <c> Proj </c>s defined in the Destination <c> Proj </c> placeholder.
        ///     Can be zero, can be lots. </summary>
        internal List<string> SourceProjList { get; }

        /// <summary> Code Files to be excluded from the Link. </summary>
        internal List<string> ExclusionsList { get; }

        /// <summary> Code Files to be Included in the recycle. </summary>
        internal List<string> InclusionsList { get; }

        /// <summary> Links the source code from the source <c> sourceProj </c> file to the destination <c> destProj </c> file.
        ///     <para> Tweaks relative file paths so the project can find them. </para>
        ///     Adds a <c> &lt;Link&gt; </c> for the destination project Solution Explorer. </summary>
        internal void LinkCode()
        {
            string oldXml = destProjXml.ReadLinkedXml();
            destProjXml.ClearOldLinkedCode();

            var totalCodezLinked = 0;

            List<string> alreadyIncluded = (from sourceItemGroup in destProjXml.ItemGroups
                                            from sourceItem in sourceItemGroup.Elements()
                                            where !Settings.ItemElementsToSkip.Contains(sourceItem.Name.LocalName.ToLower())
                                            select sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude")
                                            into attrib
                                            where attrib != null
                                            select attrib.Value.ToLower()).ToList();

            if (alreadyIncluded.Count > 1)
            {
                Log.WriteLine("These are already include so will not be linked...", ConsoleColor.White, ConsoleColor.DarkGray);
                Log.WriteLine(alreadyIncluded, ConsoleColor.Gray);
            }

            foreach (string sourcePath in SourceProjList)
            {
                var codezLinked = 0;

                try
                {
                    string sourceProjAbsolutePath = PathMaker.IsAbsolutePath(sourcePath)
                                                        ? sourcePath
                                                        : Path.Combine(DestProjDirectory, sourcePath);

                    string sourceProjDirectory = Path.GetDirectoryName(sourceProjAbsolutePath);

                    string destDirectoryForRelativePath = DestProjDirectory.EndsWith("\\")
                                                              ? DestProjDirectory
                                                              : DestProjDirectory + "\\";

                    string linkRelativeSource = PathMaker.MakeRelativePath(destDirectoryForRelativePath, sourceProjAbsolutePath);

                    var sourceProjParser = new SourceProjParser(sourceProjAbsolutePath);

                    destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("Linked from " + linkRelativeSource));
                    Log.WriteLine("Recycling from: " + sourceProjAbsolutePath + Environment.NewLine +
                                  "            to: " + DestProjAbsolutePath + Environment.NewLine, ConsoleColor.Cyan);


                    foreach (XElement sourceItemGroup in sourceProjParser.ItemGroups)
                    {
                        var newLinkedItemGroup = new XElement(Settings.MSBuild + "ItemGroup");

                        foreach (XElement sourceItem in sourceItemGroup.Elements())
                        {
                            string sourceElementName = sourceItem.Name.LocalName;

                            if (Settings.ItemElementsToSkip.Contains(sourceElementName.ToLower()))
                                continue;

                            XAttribute attrib = sourceItem.Attribute("Include") ?? sourceItem.Attribute("Exclude");

                            if (attrib != null)
                            {
                                string originalSourcePath = attrib.Value;
                                string trimmedOriginalSourcePath = originalSourcePath.Trim().ToLower();

                                

                                if (alreadyIncluded.IndexOf(trimmedOriginalSourcePath) > -1)
                                {
                                    Log.WriteLine("Skipped: " + originalSourcePath + Environment.NewLine +
                                                  "    from: " + sourceProjAbsolutePath + Environment.NewLine +
                                                  "because there is a file with the same path in the destination project." + Environment.NewLine, ConsoleColor.Gray);
                                    continue;
                                }

                                List<string> include =
                                    InclusionsList.Where(i => Operators.LikeString(trimmedOriginalSourcePath, i, CompareMethod.Text)).ToList();
                                // OW my eyes!

                                if (!InclusionsList.Any() || include.Any())
                                {
                                    if (!PathMaker.IsAbsolutePath(originalSourcePath))
                                    {
                                        var sourcePathFromDestination = "";
                                        try
                                        {
                                            string sourceFileName = Path.GetFileName(originalSourcePath); // wildcards blow up Path.GetFullPath()
                                            string originalFolder = originalSourcePath;
                                            
                                            List<string> excludedFiles =
                                                ExclusionsList.Where(x => x.Contains(sourceFileName.ToLower())).ToList();
                                            // ExclusionsList.Where(x => Operators.LikeString(trimmedOriginalSourcePath, x, CompareMethod.Text)).ToList();
                                            //  OW my eyes!

                                            var excludedPaths = ExclusionsList.Where(x => x.Contains(originalSourcePath.ToLower())).ToArray();
                                            if (excludedPaths?.Any() ?? false)
                                                excludedFiles.AddRange(excludedPaths);

                                            if (excludedFiles.Any())
                                            {
                                                Log.WriteLine("Excluded: " + originalSourcePath + Environment.NewLine +
                                                              "    from: " + sourceProjAbsolutePath + Environment.NewLine +
                                                              "because you said to Exclude: " + excludedFiles.FirstOrDefault(), ConsoleColor.White, ConsoleColor.DarkRed);
                                                continue;
                                            }

                                            if (!string.IsNullOrEmpty(sourceFileName))
                                                originalFolder = originalSourcePath.Replace(sourceFileName, "");

                                            string sourceAbsolutePath = Path.GetFullPath(sourceProjDirectory + "\\" + originalFolder) + sourceFileName;

                                            sourcePathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", sourceAbsolutePath);
                                        }

                                        catch (ArgumentException badArg)
                                        {
                                            if (badArg.Message.Contains("Illegal characters in path")) 
                                            {
                                                if (Regex.IsMatch(originalSourcePath, @"^[a-zA-Z]:\\")) // is already an absolute path
                                                    sourcePathFromDestination = originalSourcePath;

                                                else if (Regex.IsMatch(originalSourcePath, @"\*\*")) // it is **\*.* or something  awkward like that.
                                                {
                                                    string relativeFolderPathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", sourceProjDirectory);
                                                    sourcePathFromDestination = relativeFolderPathFromDestination + "\\" + originalSourcePath;
                                                    var linkElement = new XElement(Settings.MSBuild + "Link", @"%(RecursiveDir)%(Filename)%(Extension)");
                                                    sourceItem.Add(linkElement);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        string originalFolder = originalSourcePath.Substring(0, originalSourcePath.LastIndexOf("\\"));
                                                        string relativeFolderPathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", originalFolder);
                                                        sourcePathFromDestination = relativeFolderPathFromDestination + originalSourcePath.Substring(originalSourcePath.LastIndexOf("\\"));
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        App.Crash(e, "Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    string originalFolder = originalSourcePath.Substring(0, originalSourcePath.LastIndexOf("\\"));
                                                    string relativeFolderPathFromDestination = PathMaker.MakeRelativePath(DestProjDirectory + "\\", originalFolder);
                                                    sourcePathFromDestination = relativeFolderPathFromDestination + originalSourcePath.Substring(originalSourcePath.LastIndexOf("\\"));
                                                }
                                                catch (Exception e)
                                                {
                                                    App.Crash(e, "Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            App.Crash(e, "Recycling. GetFullPath: " + sourceProjDirectory + "\\" + originalSourcePath);
                                        }

                                        

                                        if (!Settings.ItemElementsDoNotMakeRelativePath.Contains(sourceElementName.ToLower()))
                                            attrib.Value = sourcePathFromDestination;
                                    }

                                    XElement[] links = sourceItem.Descendants(Settings.MSBuild + "Link").ToArray();

                                                                        // Folders, mostly
                                    if (!(links.Any() || Settings.ItemElementsDoNotBreakLink.Contains(sourceElementName.ToLower())))
                                    {
                                        var linkElement = new XElement(Settings.MSBuild + "Link", _linkPrefix +originalSourcePath);
                                        sourceItem.Add(linkElement);
                                    }

                                    foreach (XElement link in links)
                                        link.Value = _linkPrefix + link.Value;

                                    var dependentOn = sourceItem.Descendants(Settings.MSBuild + "DependentUpon").ToArray();

                                    /*foreach (XElement dependent in dependentOn)
                                    {
                                        dependent.Value = _linkPrefix + dependent.Value;
                                        sourceItem.Add(dependent);
                                    }*/

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
                        Log.WriteLine($"Copying all Files from {sourceResources} to {destResourcesPath}.", ConsoleColor.Green);

                        if (!Directory.Exists(destResourcesPath))
                            Directory.CreateDirectory(destResourcesPath);

                        foreach (string sourceFile in Directory.GetFiles(sourceResources))
                        {
                            string destFile = destResourcesPath + "\\" + Path.GetFileName(sourceFile);
                            if (File.Exists(destFile))
                            {
                                long sourceSize = new FileInfo(sourceFile).Length;
                                long destSize = new FileInfo(destFile).Length;
                                if (sourceSize != destSize) // ie. it is probably a different file.
                                {
                                    Log.WriteLine($"WARNING: Overwriting {destFile}", ConsoleColor.Red);
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


            destProjXml.EndPlaceHolder.AddBeforeSelf(new XComment("End of Linked Code" + Environment.NewLine +
                                                                  "See CodeLinkerLog.txt for details. CodeLinker by " + Settings.SourceCodeUrl +
                                                                  " "));

            if (oldXml != destProjXml.ReadLinkedXml())
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
