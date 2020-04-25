﻿// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;

namespace CodeLinker
{
    internal static class Help
    {
        public static void Write()
        {
            WriteLine("Code Linker v2 - because v1 confused even me and I wrote it");
            WriteLine("Links Source code from CSPROJ and/or VBPROJ files");
            WriteLine("Does NOT Convert C# / VB between CSPROJ and VBPROJ files.");
            WriteLine("Don't cross the streams.");
            WriteLine("Anywhere this mentions CSPROJ you can also use VBPROJ.");
            WriteLine("Anywhere this mentions .CS you can also use .VB, or any file you need.");
            WriteLine();
            WriteLine("Usages...");
            WriteLine("CODELINKER /?");
            WriteLine("CODELINKER [Destination.csproj]");
            WriteLine("Destination.csproj   Path to the existing Destination project.", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine();
            WriteLine(" - Wrap paths with spaces in double quotes.");
            WriteLine(" - Paths can (should!) be relative.");
            WriteLine("   You need to have the source project in the placeholder.");
            WriteLine();
            WriteLine();
            WriteLine("The Destination CSPROJ file needs this XML comment placeholder...", ConsoleColor.White, ConsoleColor.DarkRed);
            WriteLine("If it doesn't have the placeholder then it soon will., I will add one for you");
            WriteLine();
            WriteLine();
            WriteLine("<!-- CodeLinker");
            WriteLine();
            WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is NOT optional. You have multiples of these");
            WriteLine();
            WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== optional - a partial match will exlude it");
            WriteLine();
            WriteLine("Include: PathTo\\FileToBeIncluded.cs     <== optional but it ONLY includes matches");
            WriteLine("-->");
            WriteLine();
            WriteLine("<!-- EndCodeLinker -->");
            WriteLine();
            WriteLine(" - You may specify multiple Source: projects. No wildcards."); // todo: orly ???
            WriteLine(" - You may specify multiple Exclude: &/or Include: items. The all apply to all projects");
            WriteLine(" - File name if in the same folder, or relative or absolute path. Wildcards are OK.");
            WriteLine(" - In/Exclusions are a simple VB LIKE String wildcard match.");
            WriteLine(" - Exclusions override Any Inclusions.");
            WriteLine(" - If you specify no Inclusions then everything is an Inclusion.");
            WriteLine();
            WriteLine(" - Multiple Source: or Exclude: or Include: are ok - must be on separate lines.");
            WriteLine("   The order doesn't matter as long as they are all in teh 1st XML comment.");
            WriteLine(" - Every run of ProjectLinker will re-Link the source CSPROJ");
            WriteLine("   into the space between the XML comment placeholders.");
            WriteLine(" - ALL code links inside the placeholders are refreshed every time. OK?");
            WriteLine();
            WriteLine("More Info & Source at " + Settings.SourceCodeUrl, ConsoleColor.Green);
            WriteLine("Code Linker by CADbloke", ConsoleColor.Cyan);
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