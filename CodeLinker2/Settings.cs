// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeLinker
{
    internal class Settings
    {
        // make sure all these strings are lower case.
        internal static List<string> ItemElementsToSkip = new List<string>
            {
                "reference",
                "projectreference",
                "bootstrapperpackage",
                "import"
            };

        internal static List<string> ItemElementsDoNotBreakLink = new List<string>
            {
                "folder",
                "projectreference"
            };

        internal static List<string> ItemElementsDoNotMakeRelativePath = new List<string>
            {
                "folder"
            };

        internal static List<string> ItemElementsRescueFromLinkZone = new List<string>
            {
                "Include",
                "Exclude"
            }; // preserve the case here

        internal static XNamespace MSBuild = "http://schemas.microsoft.com/developer/msbuild/2003";
        internal static string StartPlaceholderComment          = "CodeLinker";
        internal static string EndPlaceholderComment            = "EndCodeLinker";
        internal static string SourcePlaceholderLowerCase       = "source:";
        internal static string SourcePlaceholder                = "Source:";
        internal static string ExcludePlaceholderLowerCase      = "exclude:";
        internal static string IncludePlaceholderLowerCase      = "include:";
        internal static string DestProjectFolderPrefixLowerCase = "destinationprojectfolderprefix:";
        internal static string SourceCodeUrl                    = "https://github.com/CADbloke/CodeLinker";

    }


    internal class ProjectLinkSettings
    {
        /// <summary>   The full pathname of the source project file. </summary>
        /// <value> The full pathname of the source project  file. </value>
        internal string SourceProjectAbsolutePath{ get; set; } = "";

        /// <summary>   Destination project folder prefix for THIS Source Project. </summary>
        /// <value> The destination project folder prefix. </value>
        internal string DestProjectFolderPrefix { get; set; } = "";

        /// <summary> Code Files to be excluded from the Link. </summary>
        /// <remarks> always lower case to make comparisons non-case sensitive</remarks>
        internal HashSet<string> ExclusionsList { get; set; } = new HashSet<string>()
                                                                {
                                                                    ".git\\*",
                                                                    ".git*"
                                                                };

        /// <summary> Code Files to be Included in the recycle. </summary>
        ///  <remarks> always lower case to make comparisons non-case sensitive</remarks>
        internal HashSet<string> InclusionsList { get; set; } = new HashSet<string>();
    }
}
