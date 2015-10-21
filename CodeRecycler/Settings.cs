using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeRecycler
{
  internal class Settings
  {
    internal static List<string> ItemElementsToSkip = new List<string> {"reference", "projectreference", "bootstrapperpackage", "import"};
    internal static List<string> ItemElementsDoNotBreakLink = new List<string> {"folder", "projectreference"};
    internal static List<string> ItemElementsDoNotMakeRelativePath = new List<string> {"folder"};
    internal static XNamespace MSBuild = "http://schemas.microsoft.com/developer/msbuild/2003";
    internal static string StartPlaceholderComment   = "CodeRecycler";
    internal static string EndPlaceholderComment     = "EndCodeRecycler";
    internal static string SourcePlaceholderLowerCase  = "source:";
    internal static string ExcludePlaceholderLowerCase = "exclude:";
  }
}