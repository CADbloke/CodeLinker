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

        /// <summary>   These belong outside the Link Zone. Everything in this list is lower case. </summary>
        internal List<XElement> Keepers;
        internal string OldLinkedXml; // To see if this has changed and needs to be resaved
        internal XElement RootXelement;
        internal XComment StartPlaceHolder;


        /// <summary> Absolute pathname of the destination <c> Proj </c> including file name. </summary>
        internal string DestProjAbsolutePath { get; }

        /// <summary> Absolute Directory of the destination <c> Proj </c>. NO file name. </summary>
        internal string DestProjDirectory { get; }

        internal DestinationProjXml(string destProjAbsolutePath)
        {
            try
            {
                DestProjAbsolutePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destProjAbsolutePath);
                DestProjDirectory    = Path.GetDirectoryName(DestProjAbsolutePath) ?? "";

                DestProjXdoc = XDocument.Load(DestProjAbsolutePath);
                RootXelement = DestProjXdoc.Element(Settings.MSBuild + "Project");
                ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
            }
            catch (Exception e)
            {
                App.Crash(e, $"Crash: DestProjXml CTOR loading destination XML from {DestProjAbsolutePath}");
            }

            if (RootXelement == null)
                App.Crash($"Crash: No MSBuild Namespace in {DestProjAbsolutePath}");

            StartPlaceHolder = FindStartOrEndCommentOrCrashIfDuplicatesFound(Settings.StartPlaceholderComment);
            EndPlaceHolder = FindStartOrEndCommentOrCrashIfDuplicatesFound(Settings.EndPlaceholderComment);

            if (StartPlaceHolder == null && RootXelement != null)
            {
                try
                {
                    XElement lastItemGroup = ItemGroups?.Last();
                    lastItemGroup?.AddAfterSelf(new XComment(Settings.EndPlaceholderComment));
                    lastItemGroup?.AddAfterSelf(new XComment(Settings.StartPlaceholderComment));
                    StartPlaceHolder = FindStartOrEndCommentOrCrashIfDuplicatesFound(Settings.StartPlaceholderComment);
                    EndPlaceHolder   = FindStartOrEndCommentOrCrashIfDuplicatesFound(Settings.EndPlaceholderComment); 
                    Log.WriteLine("No placeholders in destination Project. Added them", ConsoleColor.Yellow);
                }
                catch (Exception e)
                {
                    App.Crash(e, $"Crash: DestProjXmladding placeholders to destination XML in {DestProjAbsolutePath}");
                }
            }

            OldLinkedXml = ReadLinkedXml();
            Keepers = new List<XElement>();
        }


        internal void ClearOldLinkedCode()
        {
            Log.WriteLine("Housekeeping old Linked Code.", ConsoleColor.White, ConsoleColor.DarkGray);
            if (RootXelement != null)
            {
                if (StartPlaceHolder != null && EndPlaceHolder != null)
                {
                    try
                    {
                        string oldXml = "<root>" + OldLinkedXml + "</root>";

                        if (oldXml.Contains("ItemGroup"))
                        {
                            XElement keeperElements = XElement.Parse(oldXml); // http://stackoverflow.com/a/11644640/492

                            foreach (XElement descendant in keeperElements.DescendantsAndSelf().Where(e => (e.Attribute("Include") ?? e.Attribute("Exclude")) != null))
                            {
                                XAttribute xAttribute = descendant.Attribute("Include") ?? descendant.Attribute("Exclude");
                                if (xAttribute != null)
                                    Keepers.Add(descendant); // keep stray code that is not a relative link. VS *may* have added it here.
                            }
                            
                            if (Keepers.Any())
                                Log.WriteLine($"Found {Keepers.Count} potential Project Items in the Linked Zone to rescue.", ConsoleColor.Cyan);
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

                            /*else if (itemGroup.IsAfter(StartPlaceHolder) && itemGroup.IsBefore(EndPlaceHolder))
                                itemGroup.Remove();*/
                            //  System.InvalidOperationException: A common ancestor is missing.
                        }

                        ItemGroups = RootXelement?.Elements(Settings.MSBuild + "ItemGroup").ToList();
                    }
                    catch (Exception e)
                    {
                        App.Crash(e, $"FAILed clearing old linked code from: {DestProjAbsolutePath}");
                    }
                }
                Log.WriteLine("finished clearing old linked XML from source project", ConsoleColor.Gray);
            }
        }

        /// <summary> Reads XML of between the Linked zone placeholders. Does not add a <c> &lt;Root&gt; </c> element </summary>
        internal string ReadLinkedXml()
        {
            var xmlBuilder = new StringBuilder();
            if (StartPlaceHolder != null && EndPlaceHolder != null && StartPlaceHolder.IsBefore(EndPlaceHolder))
            {
                XNode startNode = StartPlaceHolder;
                while (startNode.NextNode != EndPlaceHolder)
                {
                    xmlBuilder.Append(startNode.NextNode);
                    startNode = startNode.NextNode;
                }
                return xmlBuilder.ToString();
            }

            App.Crash($"Problem with placeholders in {DestProjAbsolutePath}. Has Start: {StartPlaceHolder != null}, " 
                    + $"has End: {EndPlaceHolder != null}, Start is before End: {StartPlaceHolder?.IsBefore(EndPlaceHolder)}");

            return null; // for the compiler, it never comes back from the crash
        }

        private XComment FindStartOrEndCommentOrCrashIfDuplicatesFound(string commentStartsWith)
        {
            IEnumerable<XComment> comments = from node in DestProjXdoc.Elements().DescendantNodesAndSelf()
                                             where node.NodeType == XmlNodeType.Comment
                                             select node as XComment;

            List<XComment> placeholders = comments.Where(c => c.Value.ToLower().Trim().StartsWith(commentStartsWith.ToLower())).ToList();

            if (placeholders.Count > 1)
                App.Crash("ERROR: " + DestProjAbsolutePath + " has " + placeholders.Count + " XML comments with " + commentStartsWith);

            return placeholders.FirstOrDefault();
        }

        /// <summary> Preserves any <c> Keepers </c> - code that needs rescuing from the Link Zone. </summary>
        /// <param name="exclusionsList"> </param>
        internal void PreserveKeepersAndReport(List<string> exclusionsList)
        {
            string docString = DestProjXdoc.ToString().ToLower();
            if (Keepers.Any())
            {
                var newItemGroup = new XElement(Settings.MSBuild + "ItemGroup");
                foreach (XElement keeper in Keepers)
                {
                    string keeperString = keeper.FirstAttribute.Value.ToLower();

                    if (!docString.Contains(keeperString) && (exclusionsList == null || !exclusionsList.Any(e => e?.Contains(keeperString) ?? false)))
                    {
                        newItemGroup.Add(keeper);
                        Log.WriteLine("Rescued: " + keeper.ToString().Replace("xmlns=\"" + Settings.MSBuild + "\"", ""), ConsoleColor.White, ConsoleColor.DarkBlue);
                        //Log.WriteLine("keeperString: "+ keeperString);
                    }
                    else
                        Log.WriteLine("Skipped duplicating: " + keeperString , ConsoleColor.DarkGray);
                }
                // Log.WriteLine("DocString" +docString);
                EndPlaceHolder.AddAfterSelf(newItemGroup); // move the keepers out of the Link zone.
            }
        }

        internal void Save()
        {
            DestProjXdoc.Save(DestProjAbsolutePath);
            Log.WriteLine("Saved: " + DestProjAbsolutePath, ConsoleColor.Green);
        }
    }
}
