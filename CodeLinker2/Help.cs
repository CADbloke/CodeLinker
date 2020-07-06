// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;

namespace CodeLinker
{
    internal static class Help
    {
        public static void Write()
        {
            Log.WriteToConsole = false;

            WriteLine("Code Linker v2 - because v1 confused even me and I wrote it");
            WriteLine("Links Source code in CSPROJ and/or VBPROJ files to outside projects");
            WriteLine("Does NOT Convert C# / VB between CSPROJ and VBPROJ files.");
            WriteLine("Don't cross the streams.");
            WriteLine("Anywhere this mentions CSPROJ you can also use VBPROJ.");
            WriteLine();
            WriteLine("Usages...");
            WriteLine("CodeLinker.exe /?  this help file", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("CodeLinker.exe [Destination.csproj]", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine("Destination.csproj   Path to the existing Destination project.");
            WriteLine();
            WriteLine(" - Wrap paths with spaces in double quotes.");
            WriteLine(" - Paths can (probably should!) be relative.");
            WriteLine(" - You need to have the source project(s) in the placeholder.");
            WriteLine();
            WriteLine();
            WriteLine("CodeLinker puts your linked code between these XML comment placeholders in the Destination CSPROJ file ...", ConsoleColor.White, ConsoleColor.DarkBlue);
            WriteLine();
            WriteLine();
            WriteLine("<!-- CodeLinker", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is NOT optional. You can have multiples of these", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== optional - a partial match will exclude it", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("Include: PathTo\\FileToBeIncluded.cs     <== optional but if used it ONLY includes matches", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("DestinationProjectFolderPrefix: Folder\\For\\Linked\\Codez   <== optional folder to nest linked code in", ConsoleColor.Yellow);
            WriteLine("-->", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("<!-- EndCodeLinker -->", ConsoleColor.Yellow);
            WriteLine();
            WriteLine();
            WriteLine("If your destination project doesn't have the placeholders then it soon will, I will add them for you on the first run.");
            WriteLine();
            WriteLine("To populate the information, edit your project file in a text editor. Git is your friend.");
            WriteLine();
            WriteLine(" - You may specify multiple Source: projects. No wildcards.");
            WriteLine(" - just the File name if it's in the same folder, or relative or absolute path.");
            WriteLine();
            WriteLine(" - You may specify multiple Exclude: &/or Include: items. They all apply to all Sources");
            WriteLine(" - In/Exclusions are a simple VB LIKE String wildcard match, same as file system wildcard matches so * and ? work");
            WriteLine(" - Protip -- Folder\\OtherFolder\\* is a valid wildcard");
            WriteLine(" - Exclusions override all Inclusions.");
            WriteLine(" - If you specify no Inclusions then everything is an Inclusion.");
            WriteLine(" - If you do specify any Inclusions then ONLY they are Included.");
            WriteLine(" - Multiple Source: or Exclude: or Include: are ok - they must be on separate lines.");
            WriteLine(" - Source: order matters, CodeLinker will not add a link to a file path that already exists in the Destination project.");
            WriteLine(" - In/Exclude: order doesn't matter.");
            WriteLine();
            WriteLine("   The folder structures of the Source(s) are preserved. If the source file is nested, the link will be nested");
            WriteLine();
            WriteLine(" - Every run of ProjectLinker will re-Link the source project(s) into their space between the XML comment placeholders.");
            WriteLine(" - so ALL code links inside the placeholders are refreshed every time. OK?");
            WriteLine();
            WriteLine("to automate this linking process ...");
            WriteLine("add the CodeLinker.exe to your project can call it in our post-build commands with something like...");
            WriteLine();
            WriteLine("CodeLinker.exe \"Path\\to\\Your.csproj\"", ConsoleColor.Yellow);
            WriteLine();
            WriteLine("If your destination project has any changes Visual Studio will ask you to reload it. This is normal, don't panic");
            WriteLine();
            WriteLine("There is a log file called *CodeLinkerLog.txt* saved in the same folder as the executable. "
                    + "If you use this as a *pre/post-build process* The Visual Studio output window will have some summary information "
                    + "but the details will be in the log file. Good luck finding anything in the VS output window anyway ");
            WriteLine();
            WriteLine("More Info & Source at " + Settings.SourceCodeUrl, ConsoleColor.Green);
            WriteLine("Code Linker v2 by CADbloke", ConsoleColor.Cyan);
            WriteLine();
            WriteLine("any key to get on ith your Life");
            Log.WriteToConsole = true;
            Console.Read();
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
