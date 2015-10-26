using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeLinker
{
  internal class Settings
  {
    // make sure all these strings are lower case.
    internal static List<string> ItemElementsToSkip = new List<string> {"reference", "projectreference", "bootstrapperpackage", "import"};
    internal static List<string> ItemElementsDoNotBreakLink = new List<string> {"folder", "projectreference"};
    internal static List<string> ItemElementsDoNotMakeRelativePath = new List<string> {"folder"};
    internal static List<string> ItemElementsRescueFromLinkZone = new List<string> {"Include", "Exclude"}; // preserve the case here
    internal static XNamespace MSBuild = "http://schemas.microsoft.com/developer/msbuild/2003";
    internal static string StartPlaceholderComment   = "CodeLinker";
    internal static string EndPlaceholderComment     = "EndCodeLinker";
    internal static string SourcePlaceholderLowerCase  = "source:";
    internal static string ExcludePlaceholderLowerCase = "exclude:";
    internal static string IncludePlaceholderLowerCase = "include:";

    internal static string SourceCodeUrl = "https://github.com/CADbloke/CodeLinker";
  }
}