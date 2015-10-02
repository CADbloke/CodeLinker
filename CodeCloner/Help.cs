using System;

namespace CodeCloner
{
  static class Help
  {
    internal static string SourceCodeUrl = "https://github.com/CADbloke/CodeCloner";
    public static void Write()
    {
      WriteLine("Clones Source code between CSPROJ files");
      WriteLine("Usages...");
      WriteLine("CODECLONER /? - This help text.");
      WriteLine("CODECLONER [Folder] [/s]");
      WriteLine("CODECLONER [Source.csproj] [Destination.csproj]");
      WriteLine();
      WriteLine("Folder   Clones the source(s) into all CSPROJ files in the folder");
      WriteLine("/s       Also iterates all subfolders. You just forgot this, right?");
      WriteLine();
      WriteLine("Source.csproj        Path to the CSPROJ with the source to be cloned.");
      WriteLine("Destination.csproj   Path to ... duh.");
      WriteLine();
      WriteLine("Wrap paths with spaces in double quotes.");
      WriteLine("Paths can (probably should!) be relative.");
      WriteLine("Source.csproj is optional.");
      WriteLine("If you specify one CSPROJ file it is the destination,");
      WriteLine("in which case it needs to have the source project in the placeholder...");
      WriteLine();
      WriteLine();
      WriteLine("The Destination CSPROJ file needs this XML comment placeholder...");
      WriteLine();
      WriteLine("<!-- CodeCloner");
      WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is optional");
      WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
      WriteLine("-->");
      WriteLine();
      WriteLine("<!-- EndCodeCloner -->");
      WriteLine();
      WriteLine("You may specify multiple Source: projects. No wildcards.");
      WriteLine("If you don't specify a source in the placeholder it better be here.");
      WriteLine("You may specify multiple Exclude: items, file or path. No wildcards.");
      WriteLine("If you specify multiple items then they must be on separate lines.");
      WriteLine("Every Code Clone will re-clone the source CSPROJ");
      WriteLine("into the space between the XML comment placeholders.");
      WriteLine("ALL code inside these placeholders is refreshed every time. OK?");
      WriteLine();
      WriteLine("More Info & Source at " + SourceCodeUrl);
      WriteLine("Code Cloner by CADbloke");
      WriteLine();
    }


    private static void WriteLine(string line = "")
    {
      Console.WriteLine(line);
      Log.WriteLine(line);
    }
  }
}
