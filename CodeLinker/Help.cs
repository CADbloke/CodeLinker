// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;

namespace CodeLinker
{
    internal static class Help
    {
        public static void Write()
        {
            WriteLine("Links Source code from CSPROJ and/or VBPROJ files");
            WriteLine("Does NOT Convert C# / VB between CSPROJ and VBPROJ files.");
            WriteLine("Don't cross the streams.");
            WriteLine("Anywhere this mentions CSPROJ you can also use VBPROJ.");
            WriteLine("Anywhere this mentions .CS you can also use .VB, or any file you need.");
            WriteLine();
            WriteLine("Usages...");
            WriteLine("CODELINKER /?");
            WriteLine("CODELINKER [Folder [/s]]");
            WriteLine("CODELINKER [[Source.csproj] Destination.csproj]");
            WriteLine("CODELINKER new [[Source.csproj] Destination Folder]");
            WriteLine("CODELINKER new [[Source Folder] Destination Folder] [/s]");
            WriteLine("CODELINKER strip [Destination Folder [/s]]");
            WriteLine("CODELINKER strip [Destination.csproj]");
            WriteLine("CODELINKER ... /noconfirm");
            WriteLine();
            WriteLine("Folder   Links the source(s) into all CSPROJ files in the folder");
            WriteLine("         This is the destination folder that has Linked projects.");
            WriteLine("         The Destination projects need to have the source in their placeholder.");
            WriteLine("Source.csproj        optional Path to the CSPROJ with the source to be Linked.");
            WriteLine("                     if only 1 CSPROJ is specified then it is the Destination.");
            WriteLine("                     This source overrides all sources in the Destination.");
            WriteLine("Destination.csproj   Path to the existing Destination project.");
            WriteLine();
            WriteLine("new      Copies the Source to the Destination path and strips the contents.");
            WriteLine("         Creates new project file(s) with a placeholder + Source.");
            WriteLine("         For Folders it copies all Projects to the same destination Folder.");
            WriteLine();
            WriteLine("Source Solution Root  (optional)");
            WriteLine("         The root of the solution containing the projects to Link.");
            WriteLine("         Default is the current directory. Current? Depends - see the Wiki.");
            WriteLine("Destination Solution Folder.");
            WriteLine("         The Folder Name for the Linked project(s).");
            WriteLine("         If only one Folder is specifed then it is the destination folder.");
            WriteLine();
            WriteLine("strip    Creates Linked projects from ALL existing CSPROJ in the folder.");
            WriteLine("         /s iterates all subfolders . Strips out ALL code. Adds a placeholder.");
            WriteLine("         usage: manually copy an existing CSPROJ(s). Strip it. Add a Source.");
            WriteLine("         This is like \"new\" but doesn't copy anything.");
            WriteLine("         Fix References, build settings etc. Build it. Rejoice.");
            WriteLine();
            WriteLine("/?       This help text.");
            WriteLine("/noconfirm   Switch. Don't ask about overwrites etc. Use in batch jobs.");
            WriteLine("             Only affects jobs when you create a new file.");
            WriteLine("/s       Switch. Iterate all subfolders. You just forgot this, right?");
            WriteLine("         You can use . for the current folder, add /s for all subfolders.");
            WriteLine("/stfu        Switch. Don't write log to the command line or Visual Studio.");
            WriteLine("             This will still write any exception summaries but not the gory details.");
            WriteLine();
            WriteLine(" - Wrap paths with spaces in double quotes.");
            WriteLine(" - Paths can (should!) be relative.");
            WriteLine(" - Source.csproj is optional,");
            WriteLine("   in which case you need to have the source project in the placeholder.");
            WriteLine("   So if you specify one CSPROJ file here it is the destination.");
            WriteLine();
            WriteLine();
            WriteLine("The Destination CSPROJ file needs this XML comment placeholder...");
            WriteLine();
            WriteLine("<!-- CodeLinker");
            WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is optional");
            WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
            WriteLine("Include: PathTo\\FileToBeIncluded.cs     <== this is optional");
            WriteLine("-->");
            WriteLine();
            WriteLine("<!-- EndCodeLinker -->");
            WriteLine();
            WriteLine(" - \"new\" and \"strip\" creates this for you.");
            WriteLine(" - You may specify multiple Source: projects. No wildcards.");
            WriteLine(" - If you don't specify a source in the command call it better be there.");
            WriteLine(" - You may specify multiple Exclude: &/or Include: items.");
            WriteLine(" - File name or path. Wildcards are OK.");
            WriteLine(" - In/Exclusions are a simple String wildcard match.");
            WriteLine(" - Exclusions override Any Inclusions.");
            WriteLine(" - If you specify no Inclusions then everything is an Inclusion.");
            WriteLine();
            WriteLine(" - Multiple Source: or Exclude: or Include: must be on separate lines.");
            WriteLine(" - Every Code ProjectLinker will re-Link the source CSPROJ");
            WriteLine("   into the space between the XML comment placeholders.");
            WriteLine(" - ALL code links inside these placeholders are refreshed every time. OK?");
            WriteLine();
            WriteLine("More Info & Source at " + Settings.SourceCodeUrl);
            WriteLine("Code Linker by CADbloke");
            WriteLine();
        }

        private static void WriteLine(string line = "")
        {
            Console.WriteLine(line);
            Log.WriteLine(line);
        }
    }
}
