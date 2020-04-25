// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;

namespace CodeLinker
{
    internal static class Help
    {
        public static void Write()
        {
            WriteLine("Code Linker v2 - because v1 confused even me and I wrote it");
            WriteLine("Links Source code in CSPROJ and/or VBPROJ files to outside projects");
            WriteLine("Does NOT Convert C# / VB between CSPROJ and VBPROJ files.");
            WriteLine("Don't cross the streams.");
            WriteLine("Anywhere this mentions CSPROJ you can also use VBPROJ.");
            WriteLine();
            WriteLine("Usages...");
            WriteLine("CODELINKER /?  tis help file", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("CODELINKER [Destination.csproj]", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("Destination.csproj   Path to the existing Destination project.");
            WriteLine();
            WriteLine(" - Wrap paths with spaces in double quotes.");
            WriteLine(" - Paths can (should!) be relative.");
            WriteLine("   You need to have the source project in the placeholder.");
            WriteLine();
            WriteLine();
            WriteLine("The Destination CSPROJ file uses this XML comment placeholder...", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("If it doesn't have the placeholder then it soon will., I will add one for you on the first run");
            WriteLine();
            WriteLine();
            WriteLine("<!-- CodeLinker", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is NOT optional. You have multiples of these", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== optional - a partial match will exlude it", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("Include: PathTo\\FileToBeIncluded.cs     <== optional but it ONLY includes matches", ConsoleColor.Yellow);
            WriteLine("-->");
            WriteLine();
            WriteLine("<!-- EndCodeLinker -->");
            WriteLine();
            WriteLine();
            WriteLine(" - You may specify multiple Source: projects. No wildcards.");
            WriteLine(" - File name if in the same folder, or relative or absolute path. Wildcards are OK.");
            WriteLine();
            WriteLine(" - You may specify multiple Exclude: &/or Include: items. They all apply to all Sour es");
            WriteLine(" - In/Exclusions are a simple VB LIKE String wildcard match, same as file system wildcard matches so * and ? work");
            WriteLine(" - Protip -- Folder\\OtherFolder\\* is a valid wildcard");
            WriteLine(" - Exclusions override Any Inclusions.");
            WriteLine(" - If you specify no Inclusions then everything is an Inclusion.");
            WriteLine(" - If you do specify any Inclusions then ONLY they are Included.");
            
            WriteLine(" - Multiple Source: or Exclude: or Include: are ok - must be on separate lines.");
            WriteLine(" - Source: order matters, this will not add a link to a file path that already exists in the Destination project.");
            WriteLine(" - In/Exclude: order doesn't matter.");
            WriteLine();
            WriteLine("   The folder structures of the Source(s) are preserved. If the source file is nested, the link will be nested");
            WriteLine();
            WriteLine(" - Every run of ProjectLinker will re-Link the source project(s)");
            WriteLine("   into the space between the XML comment placeholders.");
            WriteLine(" - ALL code links inside the placeholders are refreshed every time. OK?");
            WriteLine();
            WriteLine("More Info & Source at " + Settings.SourceCodeUrl, ConsoleColor.Green);
            WriteLine("Code Linker 2 by CADbloke", ConsoleColor.Cyan);
            WriteLine();
            WriteLine("any key to continue");
            Console.ReadKey();
        }

        private static void WriteLine(string line = "", ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            
            Console.WriteLine(line);
            Log.WriteLine(line, foreground, background);
            
        }
    }
}
