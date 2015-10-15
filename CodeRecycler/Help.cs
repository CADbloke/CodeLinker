using System;

namespace CodeRecycler
{
  static class Help
  {
    internal static string SourceCodeUrl = "https://github.com/CADbloke/CodeRecycler";
    public static void Write()
    {
      WriteLine("Recycles Source code from CSPROJ and/or VBPROJ files");
      WriteLine("Does NOT Convert between CSPROJ and VBPROJ files.");
      WriteLine("Don't cross the streams.");
      WriteLine("Anywhere this mentions CSPROJ you can also use VBPROJ.");
      WriteLine("Anywhere this mentions .CS you can also use .VB (or any file you need.");
      WriteLine();
      WriteLine("Usages...");
      WriteLine("CODERECYCLER /?");
      WriteLine("CODERECYCLER [Folder [/s]]");
      WriteLine("CODERECYCLER [Source.csproj Destination.csproj]");
      WriteLine("CODERECYCLER strip [Destination Folder [/s]]");
      WriteLine("CODERECYCLER strip [Destination.csproj]");
      WriteLine();
      WriteLine("/?       This help text.");
      WriteLine();
      WriteLine("Folder   Recycles the source(s) into all CSPROJ files in the folder");
      WriteLine("         This is the destination folder that has recycled projects.");
      WriteLine("         The Destination projects need to have the source in their placeholder.");
      WriteLine("/s       Also iterates all subfolders. You just forgot this, right?");
      WriteLine();
      WriteLine("Source.csproj        Path to the CSPROJ with the source to be recycled.");
      WriteLine("Destination.csproj   Path to the existing Destination project.");
      WriteLine();
      WriteLine("Source Solution Root  (optional)");
      WriteLine("         The root of the solution containing the projects to recycle.");
      WriteLine("         Default is the current directory.");
      WriteLine("Destination Solution Folder  (optional)");
      WriteLine("         The new Folder Name for the recycled projects. Default is \"_Builds\".");
      WriteLine("         If only one Folder is specifed then it is the destination folder.");
      WriteLine();
      WriteLine("strip    Creates new recycled projects from ALL existing CSPROJ in the folder.");
      WriteLine("         /s iterates all subfolders . Strips out ALL code. Adds a placeholder.");
      WriteLine("         usage: copy an existing CSPROJ. Strip it. Add a Source.");
      WriteLine("                Update References, build settings etc. Build it. Rejoice.");
      WriteLine();
      WriteLine(" - Wrap paths with spaces in double quotes.");
      WriteLine(" - Paths can (should!) be relative.");
      WriteLine(" - Source.csproj is optional,");
      WriteLine("   in which case you need to have the source project in the placeholder.");
      WriteLine("   So if you specify one CSPROJ file it is the destination.");
      WriteLine();
      WriteLine();
      WriteLine("The Destination CSPROJ file needs this XML comment placeholder...");
      WriteLine();
      WriteLine("<!-- CodeRecycler");
      WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is optional");
      WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
      WriteLine("-->");
      WriteLine();
      WriteLine("<!-- EndCodeRecycler -->");
      WriteLine();
      WriteLine(" - You may specify multiple Source: projects. No wildcards.");
      WriteLine(" - If you don't specify a source in the command call it better be here.");
      WriteLine(" - You may specify multiple Exclude: items, file or path. No wildcards.");
      WriteLine(" - Exclusions are a simple String.Contains() filter list.");
      WriteLine(" - If you specify multiple items then they must be on separate lines.");
      WriteLine(" - Every Code Recycle will re-recycle the source CSPROJ");
      WriteLine("   into the space between the XML comment placeholders.");
      WriteLine(" - ALL code links inside these placeholders are refreshed every time. OK?");
      WriteLine();
      WriteLine("More Info & Source at " + SourceCodeUrl);
      WriteLine("Code Recycler by CADbloke");
      WriteLine();
    }


    private static void WriteLine(string line = "")
    {
      Console.WriteLine(line);
      Log.WriteLine(line);
    }
  }
}
