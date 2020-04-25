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
        internal static bool WriteToConsole = true;

        // http://stackoverflow.com/a/18709110/492
        private static readonly Destructor Finalise = new Destructor();

        static Log()
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logFile))
                {
                    sw.WriteLine();
                    sw.WriteLine("==========================");
                    sw.WriteLine("Code Linker Log: " + DateTime.Now);
                    sw.WriteLine("--------------------------");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                throw;
            }
        }

        internal static void WriteLine(List<string> lines, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                throw;
            }

            Console.ResetColor();
        }

        internal static void WriteLine(string line = "", ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            WriteLine(lines: new List<string> { line }, foreground: foreground, background: background);
        }

        internal static void WriteException(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            
            WriteLine(e.ToString(), ConsoleColor.Yellow, ConsoleColor.DarkRed);
            WriteLine(e.InnerException?.ToString(), ConsoleColor.Yellow);
            Console.WriteLine(e.ToString());
            Console.WriteLine(e.InnerException?.ToString());
            Console.ResetColor();
        }


        private sealed class Destructor
        {
            ~Destructor() // One time only destructor.
            {
                try
                {
                    using (StreamWriter logger = File.AppendText(logFile))
                    {
                        logger.WriteLine();
                        logger.WriteLine("Finished at "           + DateTime.Now);
                        logger.WriteLine("More Info & Source at " + Settings.SourceCodeUrl);
                        logger.WriteLine("Closing Log. kthxbai.");
                        logger.WriteLine("==========================");
                        logger.WriteLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadKey();
                    throw;
                }
            }
        }
    }
}
