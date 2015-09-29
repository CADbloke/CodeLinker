using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCloner
{
  static class Help
  {
    internal static string SourceCodeUrl = "https://github.com/CADbloke/CodeCloner";
    public static void Write()
    {
      Console.WriteLine("Clones Source code between CSPROJ files");
      Console.WriteLine("Usages...");
      Console.WriteLine("CODECLONER /? - This help text.");
      Console.WriteLine("CODECLONER [Folder] [/s] [diff]");
      Console.WriteLine("CODECLONER [Source.csproj] [Destination.csproj] [diff]");
      Console.WriteLine();
      Console.WriteLine("Folder   Clones the source(s) into all CSPROJ files in the folder");
      Console.WriteLine("/s       Also iterates all subfolders. You just forgot this, right?");
      Console.WriteLine();
      Console.WriteLine("Source.csproj        Path to the CSPROJ with the source to be cloned.");
      Console.WriteLine("Destination.csproj   Path to ... duh.");
      Console.WriteLine("diff                 Opens a diff Session with diffs of the cloned CSPROJs.");
      Console.WriteLine();
      Console.WriteLine("Supported diff apps are Beyond Compare & KDiff.");
      Console.WriteLine("Wrap paths with spaces in double quotes.");
      Console.WriteLine("Paths can (probably should!) be relative.");
      Console.WriteLine("Source.csproj is optional.");
      Console.WriteLine("If you specify one CSPROJ file it is the destination,");
      Console.WriteLine("in which case it needs to have the source project in the placeholder...");
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine("The Destination CSPROJ file needs this XML comment placeholder...");
      Console.WriteLine();
      Console.WriteLine("<!-- CodeCloner");
      Console.WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is optional");
      Console.WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
      Console.WriteLine("-->");
      Console.WriteLine();
      Console.WriteLine("<!-- EndCodeCloner -->");
      Console.WriteLine();
      Console.WriteLine("You may specify multiple Source: projects. No wildcards.");
      Console.WriteLine("If you don't specify a source in the placeholder it better be here.");
      Console.WriteLine("You may specify multiple Exclude: items. Can be a wildcard, file or path.");
      Console.WriteLine("If you specify multiple items then they must be on separate lines.");
      Console.WriteLine("Every Code Clone will re-clone the source CSPROJ");
      Console.WriteLine("into the space between the XML comment placeholders.");
      Console.WriteLine("ALL code inside these placeholders is refreshed every time. OK?");
      Console.WriteLine();
      Console.WriteLine("More Info & Source at " + SourceCodeUrl);
      Console.WriteLine("Code Cloner by CADbloke");
      
    }
  }
}
