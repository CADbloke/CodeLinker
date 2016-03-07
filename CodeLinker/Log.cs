// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;
using System.Collections.Generic;
using System.IO;

namespace CodeLinker
{
    internal static class Log
    {
        internal static string logFile = AppDomain.CurrentDomain.BaseDirectory + "\\CodeLinkerLog.txt";
        internal static bool WriteToConsole = false;

        // http://stackoverflow.com/a/18709110/492
        private static readonly Destructor Finalise = new Destructor();

        static Log()
        {
            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.WriteLine();
                sw.WriteLine("==========================");
                sw.WriteLine("Code Linker Log: " + DateTime.Now);
                sw.WriteLine("--------------------------");
            }
        }

        internal static void WriteLine(List<string> lines)
        {
            using (StreamWriter sw = File.AppendText(logFile))
            {
                foreach (string line in lines)
                {
                    sw.WriteLine(line);
                    if (WriteToConsole)
                        Console.WriteLine(line);
                }
            }
        }

        internal static void WriteLine(string line = "")
        {
            WriteLine(lines: new List<string> { line });
        }

        internal static void WriteException(Exception e)
        {
            WriteLine(e.ToString());
            WriteLine(e.InnerException?.ToString());
            Console.WriteLine(e.ToString());
            if (WriteToConsole)
                Console.WriteLine(e.InnerException?.ToString());
        }


        private sealed class Destructor
        {
            ~Destructor() // One time only destructor.
            {
                using (StreamWriter logger = File.AppendText(logFile))
                {
                    logger.WriteLine();
                    logger.WriteLine("Finished at " + DateTime.Now);
                    logger.WriteLine("More Info & Source at " + Settings.SourceCodeUrl);
                    logger.WriteLine("Closing Log. kthxbai.");
                    logger.WriteLine("==========================");
                    logger.WriteLine();
                }
            }
        }
    }
}
