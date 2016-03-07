// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CodeLinker
{
    /// <summary> Removes previously Linked Code. </summary>
    internal class DestinationProjXml
    {
        internal XDocument DestProjXdoc;
        internal XComment EndPlaceHolder;
        internal List<XElement> ItemGroups;
        internal List<XElement> Keepers; // These belong outside the Link Zone
        internal string OldLinkedXml; // To see if this has changed and needs to be resaved
        internal XElement RootXelement;
        internal XComment StartPlaceHolder;

        internal DestinationProjXml(string destProj)
        {
            DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProj);
            DestProjDirectory = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

            try
            {
                DestProjXdoc = XDocument.Load(DestProjAbsolutePath);
                RootXelement = DestProjXdoc.Element(Settings.MSBuild + "Project");
                ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
            }
            catch (Exception e)
            {
                App.Crash(e, "Crash: DestProjXml CTOR loading destination XML from " + DestProjAbsolutePath);
            }

            if (RootXelement == null)
                App.Crash("Crash: No MSBuild Namespace in " + DestProjAbsolutePath);

            StartPlaceHolder = FindCommentOrCrashIfDuplicatesFound(Settings.StartPlaceholderComment);
            EndPlaceHolder = FindCommentOrCrashIfDuplicatesFound(Settings.EndPlaceholderComment);

            if (StartPlaceHolder == null && RootXelement != null)
            {
                XElement lastItemGroup = ItemGroups?.Last();
                lastItemGroup?.AddAfterSelf(new XComment(Settings.EndPlaceholderComment));
                lastItemGroup?.AddAfterSelf(new XComment(Settings.StartPlaceholderComment));
                StartPlaceHolder = FindCommentOrCrashIfDuplicatesFound(Settings.StartPlaceholderComment);
                EndPlaceHolder = FindCommentOrCrashIfDuplicatesFound(Settings.EndPlaceholderComment);
            }

            OldLinkedXml = ReadLinkedXml();
            Keepers = new List<XElement>();
        }

        /// <summary> Absolute pathname of the destination <c> Proj </c> including file name. </summary>
        internal string DestProjAbsolutePath { get; }

        /// <summary> Absolute Directory of the destination <c> Proj </c>. NO file name. </summary>
        internal string DestProjDirectory { get; }

        internal void ClearOldLinkedCode()
        {
            Log.WriteLine("Housekeeping old Linked Code.");
            if (RootXelement != null)
            {
                if (StartPlaceHolder != null && EndPlaceHolder != null)
                {
                    try
                    {
                        string oldLinkedXml = "<root>" + OldLinkedXml + "</root>";

                        if (oldLinkedXml.Contains("ItemGroup"))
                        {
                            XElement keeperElements = XElement.Parse(oldLinkedXml); // http://stackoverflow.com/a/11644640/492

                            foreach (XElement descendant in keeperElements.DescendantsAndSelf().Where(e => e.Attribute("Include") != null))
                            {
                                XAttribute xAttribute = descendant.Attribute("Include");
                                if (xAttribute != null && !xAttribute.Value.StartsWith(".."))
                                    Keepers.Add(descendant); // keep stray code that is not a relative link. VS *may* have added it here.
                            }
                        }

                        if (StartPlaceHolder != null && EndPlaceHolder != null && StartPlaceHolder.IsBefore(EndPlaceHolder))
                        {
                            XNode startNode = StartPlaceHolder;
                            while (startNode.NextNode != EndPlaceHolder)
                            {
                                startNode.NextNode.Remove();
                            }
                        }

                        foreach (XElement itemGroup in ItemGroups)
                        {
                            if (itemGroup.IsEmpty || (!itemGroup.Descendants().Any() && string.IsNullOrEmpty(itemGroup.Value)))
                                itemGroup.Remove();
                        }

                        ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
                    }
                    catch (Exception e)
                    {
                        App.Crash(e, "Bad Proj No ItemGroups: " + DestProjAbsolutePath);
                    }
                }
                Log.WriteLine("ok.");
            }
        }

        /// <summary> Reads XML of between the Linked zone placeholders. Does not add a <c> &lt;Root&gt; </c> element </summary>
        internal string ReadLinkedXml()
        {
            var oldXmlBuilder = new StringBuilder();
            if (StartPlaceHolder != null && EndPlaceHolder != null && StartPlaceHolder.IsBefore(EndPlaceHolder))
            {
                XNode startNode = StartPlaceHolder;
                while (startNode.NextNode != EndPlaceHolder)
                {
                    oldXmlBuilder.Append(startNode.NextNode);
                    startNode = startNode.NextNode;
                }
                return oldXmlBuilder.ToString();
            }
            return string.Empty;
        }

        private XComment FindCommentOrCrashIfDuplicatesFound(string commentStartsWith)
        {
            IEnumerable<XComment> comments = from node in DestProjXdoc.Elements().DescendantNodesAndSelf()
                                             where node.NodeType == XmlNodeType.Comment
                                             select node as XComment;

            List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

            if (placeholders.Count > 1)
                App.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);

            return placeholders.FirstOrDefault();
        }

        internal void ClearAllExistingCodeExceptExplicitlyLinked()
        {
            if (RootXelement != null)
            {
                ItemGroups = new List<XElement>();
                ItemGroups.AddRange(RootXelement.Elements(Settings.MSBuild + "ItemGroup").Select(elements => elements));
                if (ItemGroups.Count == 0)
                    Log.WriteLine("Curious: " + DestProjAbsolutePath + " contains no ItemGroups. No Codez?");

                if (ItemGroups != null)
                {
                    foreach (XElement itemGroup in ItemGroups)
                    {
                        itemGroup.Elements()
                                 .Where(i => !Settings.ItemElementsToSkip.Contains(i.Name.LocalName.ToLower()) &&
                                             (i.Attribute("Include") != null) && i.Attribute("Link") == null)
                                 .Remove();

                        if (itemGroup.IsEmpty || (!itemGroup.Descendants().Any() && string.IsNullOrEmpty(itemGroup.Value)))
                            itemGroup.Remove();
                    }
                    ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
                    Log.WriteLine("Removed old Code from: " + DestProjAbsolutePath);
                }
            }
        }

        internal void ClearStartPlaceholderContent()
        {
            StartPlaceHolder.Value = Settings.StartPlaceholderComment;
            Log.WriteLine("Reset Includes & Excludes");
        }

        internal void AddExclusion(string exclusion)
        {
            StartPlaceHolder.Value += Environment.NewLine + Settings.ExcludePlaceholderLowerCase + " " + exclusion;
            Log.WriteLine("Excluded: " + exclusion);
        }

        internal void AddInclusion(string inclusion)
        {
            StartPlaceHolder.Value += Environment.NewLine + Settings.IncludePlaceholderLowerCase + " " + inclusion;
            Log.WriteLine("Included: " + inclusion);
        }

        internal void AddSource(string source)
        {
            StartPlaceHolder.Value += Environment.NewLine + Settings.SourcePlaceholderLowerCase + " " + source;
            Log.WriteLine("Added Source: " + source);
        }

        /// <summary> Saves the Project and also preserves any <c> Keepers </c> - code that needs rescuing from the Link Zone. </summary>
        internal void Save()
        {
            if (Keepers.Any())
            {
                var newItemGroup = new XElement(Settings.MSBuild + "ItemGroup");
                foreach (XElement keeper in Keepers)
                {
                    newItemGroup.Add(keeper);
                    Log.WriteLine("Rescued: " + keeper.ToString().Replace("xmlns=\"" + Settings.MSBuild + "\"", ""));
                }
                EndPlaceHolder.AddAfterSelf(newItemGroup); // move the keepers out of the Link zone.
            }

            DestProjXdoc.Save(DestProjAbsolutePath);
            Log.WriteLine("Saved: " + DestProjAbsolutePath);
        }
    }
}
