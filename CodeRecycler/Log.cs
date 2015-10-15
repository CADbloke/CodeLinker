using System;
using System.Collections.Generic;
using System.IO;

namespace CodeRecycler
{
  internal static class Log
  {
    internal static string logFile = AppDomain.CurrentDomain.BaseDirectory + "\\CodeRecyclerLog.txt";

    static Log()
    {
      using (StreamWriter sw = File.AppendText(logFile))
      {
        sw.WriteLine("==========================");
        sw.WriteLine("Code Recycler Log: " + DateTime.Now);
        sw.WriteLine("--------------------------");
      }
    }


    internal static void WriteLine(List<string> lines)
    {
      using (StreamWriter sw = File.AppendText(logFile))
      {
        foreach (string line in lines) { sw.WriteLine(line); }
      }
    }

    internal static void WriteLine(string line = "")
    {
      WriteLine(new List<string> {line});
    }



    internal static void WriteLine(Exception e)
    {
      WriteLine(e.ToString());
      WriteLine(e.InnerException?.ToString());
      Console.WriteLine(e.ToString());
    }


    // http://stackoverflow.com/a/18709110/492
    private static readonly Destructor Finalise = new Destructor();
    private sealed class Destructor
    {
      ~Destructor() // One time only destructor.
      {
        using (StreamWriter logger = File.AppendText(logFile))
        {
          logger.WriteLine();
          logger.WriteLine("Finished at " + DateTime.Now);
          logger.WriteLine("More Info & Source at " + Help.SourceCodeUrl);
          logger.WriteLine("Closing Log. kthxbai.");
          logger.WriteLine("==========================");
          logger.WriteLine();
        }
        
      }
    }


  }
}
