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
            WriteLine("CODELINKER Source.csproj Destination.csproj /abs /prefix:\"In This Folder\"");
            WriteLine();
            WriteLine("Folder   Links the source(s) into all CSPROJ files in the folder", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("         This is the destination folder that has Linked projects.");
            WriteLine("         The Destination projects need to have the source in their placeholder.");
            WriteLine("Source.csproj        optional Path to the CSPROJ with the source to be Linked.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("                     if only 1 CSPROJ is specified then it is the Destination.");
            WriteLine("                     This source overrides all sources in the Destination.");
            WriteLine("Destination.csproj   Path to the existing Destination project.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine();
            WriteLine("new      Copies the Source to the Destination path and strips the contents.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("         Creates new project file(s) with a placeholder + Source.");
            WriteLine("         For Folders it copies all Projects to the same destination Folder.");
            WriteLine("/nosub   Don't create SubFolders for all new Projects.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine();
            WriteLine("Source Solution Root  (optional)", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("         The root of the solution containing the projects to Link.");
            WriteLine("         Default is the current directory. Current? Depends - see the Wiki.");
            WriteLine("Destination Solution Folder.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("         The Folder Name for the Linked project(s).");
            WriteLine("         If only one Folder is specifed then it is the destination folder.");
            WriteLine();
            WriteLine("strip    Creates Linked projects from ALL existing CSPROJ in the folder.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("         /s iterates all subfolders . Strips out ALL code. Adds a placeholder.");
            WriteLine("         usage: manually copy an existing CSPROJ(s). Strip it. Add a Source.");
            WriteLine("         This is like \"new\" but doesn't copy anything.");
            WriteLine("         Fix References, build settings etc. Build it. Rejoice.");
            WriteLine();
            WriteLine("/?       This help text.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("/noconfirm   Switch. Don't ask about overwrites etc. Use in batch jobs.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("             Only affects jobs when you create a new file.");
            WriteLine("/s       Switch. Iterate all subfolders. You just forgot this, right?", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("         You can use . for the current folder, add /s for all subfolders.");
            WriteLine("/stfu        Switch. Don't write log to the command line or Visual Studio.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("             This will still write any exception summaries but not the gory details.");
            WriteLine();
            WriteLine("/abs         Switch. Write absolute paths instead of relative paths in the Destination.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("             Default is to write relative paths.");
            WriteLine();
            WriteLine("/prefix:xxx  Add a folder prefix to all the link paths in the Destination.");
            WriteLine("             Default is no prefix. NO space after the colon. Use quotes if the path has spaces", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine();
            WriteLine(" - Wrap paths with spaces in double quotes.");
            WriteLine(" - Paths can (should!) be relative.");
            WriteLine(" - Source.csproj is optional,");
            WriteLine("   in which case you need to have the source project in the placeholder.");
            WriteLine("   So if you specify one CSPROJ file here it is the destination.");
            WriteLine();
            WriteLine();
            WriteLine("The Destination CSPROJ file needs this XML comment placeholder...", ConsoleColor.White, ConsoleColor.DarkRed);
            WriteLine("If it doesn't have the placeholder then it soon will.");
            WriteLine();
            WriteLine("<!-- CodeLinker");
            WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is optional");
            WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
            WriteLine("Ummm, the exclude thing doesn't actually work yet. Sorry.");
            WriteLine("Include: PathTo\\FileToBeIncluded.cs     <== this is optional");
            WriteLine("-->");
            WriteLine();
            WriteLine("<!-- EndCodeLinker -->");
            WriteLine();
            WriteLine(" - \"new\" and \"strip\" creates this for you.", ConsoleColor.White, ConsoleColor.DarkBlue);
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
            WriteLine("More Info & Source at " + Settings.SourceCodeUrl, ConsoleColor.Green);
            WriteLine("Code Linker by CADbloke", ConsoleColor.Cyan);
            WriteLine();
        }

        private static void WriteLine(string line = "", ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            //Console.ForegroundColor = foreground;
            //Console.BackgroundColor = background;
            
            // Console.WriteLine(line);
            Log.WriteLine(line, foreground, background);
            
        }
    }
}
