using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCloner
{
  static class Help
  {
    static string SourceUrl = "https://github.com/CADbloke/CodeCloner";
    public static void Write()
    {
      Console.WriteLine("Clones Source code between CSPROJ files");
      Console.WriteLine("Usage...");
      Console.WriteLine("CODECLONER /? - This help text.");
      Console.WriteLine("CODECLONER [Folder] [/s] [diff] [log]");
      Console.WriteLine("CODECLONER [Source.csproj] [Destination.csproj] [diff] [log]");
      Console.WriteLine("");
      Console.WriteLine("Folder   Clones the source into all CSPROJ files in the folder");
      Console.WriteLine("/s       Also iterates all subfolders");
      Console.WriteLine();
      Console.WriteLine("Source.csproj        Whole Path to the CSPROJ with the source to be cloned");
      Console.WriteLine("Destination.csproj   Whole path to ... duh");
      Console.WriteLine("Wrap paths with spaces in double quotes. Paths can (should!) be relative.");
      Console.WriteLine("");
      Console.WriteLine("diff     Opens a BeyondCompare Session with diffs of the cloned CSPROJs");
      Console.WriteLine("log      Append a log to CodeClonerLog.txt");
      Console.WriteLine("");
      Console.WriteLine("If you have specified the Source & Destination CSPROJ files");
      Console.WriteLine("the Destination CSPROJ file is looking for this placeholder...");
      Console.WriteLine("");
      Console.WriteLine("<!-- CodeCloner -->");
      Console.WriteLine("");
      Console.WriteLine("<!-- EndCodeCloner -->");
      Console.WriteLine("");
      Console.WriteLine("For Whole Folder Operations use this format for the placeholder...");
      Console.WriteLine("");
      Console.WriteLine("<!-- CodeClonerSource: PathTo\\NameOfProject.csproj");
      Console.WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
      Console.WriteLine("-->");
      Console.WriteLine("");
      Console.WriteLine("<!-- EndCodeCloner -->");
      Console.WriteLine("");
      Console.WriteLine("Every clone will re-clone the source CSPROJ");
      Console.WriteLine("into the space between the XML comment placeholders.");
      Console.WriteLine("All code inside these placeholders is refreshed. OK?");
      Console.WriteLine("");
      Console.WriteLine("More Info & Source at " + SourceUrl);
      Console.WriteLine("Code Cloner by CADbloke");
    }
  }
}
