// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeLinker
{
    public static class App
    {
        private static readonly List<DestinationProjLinker> linkers = new List<DestinationProjLinker>();
        internal static         bool                        NoConfirm;

        public static void ParseCommands(string[] args)
        {
            linkers.Clear();
            // System.Diagnostics.Debugger.Launch(); // to find teh bugs load this project in Visual Studio and uncomment the start of this line.
            int argsCount = args.Length;

            if (argsCount == 0)
            {
                Log.WriteLine("[Error] No Args given so Help Text Displayed.", ConsoleColor.Red);
                Log.WriteLine();
                Help.Write();
                Finish();
            }

            List<string> argsList = args.Select(a => a.Replace(@"""", "")).ToList();

            if (argsList.Contains("/?"))
            {
                Help.Write();
                Log.WriteLine("User asked For Help. Hope I helped.", ConsoleColor.Green);
                Finish();
            }


            if (!string.IsNullOrEmpty(argsList[0]))
            {
                if (argsList[0].IsaCsOrVbProjFile())
                {
                    Log.WriteLine("Queueing Code Link to: " + argsList[0] + ". Source TBA.", ConsoleColor.Cyan);
                    linkers.Add(new DestinationProjLinker(argsList[0]));
                }


                if (!linkers.Any())
                {
                    string errorMessage = "I got nuthin. Your Args made no sense to me." + Environment.NewLine;

                    foreach (string arg in args)
                        errorMessage += arg + Environment.NewLine;

                    Crash(errorMessage);
                }

                foreach (DestinationProjLinker destinationProjLinker in linkers)
                    destinationProjLinker?.LinkCode();
            }
        }


        internal static void Finish(string message = "")
        {
            message += "Finished recycling " + linkers.Count + " Project"; // writes to VS window
            if (linkers.Count != 1)
                message += "s";

            message += "." + Environment.NewLine;

            foreach (DestinationProjLinker linker in linkers)
            {
                foreach (ProjectLinkSettings projectLinkSettings in linker?.SourceProjList ?? new List<ProjectLinkSettings>())
                    message += "from :" + projectLinkSettings?.SourceProjectAbsolutePath;

                message += "  to :" + linker?.DestProjAbsolutePath + Environment.NewLine;
            }
            Log.WriteLine(message, ConsoleColor.White, ConsoleColor.DarkGray);
            Environment.Exit(0);
        }

        internal static void Crash(Exception e, string crashedAt = "")
        {
            Log.WriteToConsole = true;
            string message = "I crashed at " + crashedAt + ". Whups. See CodeLinkerLog.txt for details.";
            Log.WriteLine();
            Log.WriteLine(message, ConsoleColor.Red);
            Log.WriteException(e);
            Console.Read();
            throw e;
        }

        internal static void Crash(string errorMessage)
        {
            Log.WriteToConsole = true;
            Log.WriteLine($"[Error] {errorMessage}", ConsoleColor.Red);
            Console.WriteLine(errorMessage);
            Console.Read();
            Environment.Exit(1);
        }
    }
}
