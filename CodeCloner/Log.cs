using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCloner
{
  internal static class Log
  {
    private static string logFile = AppDomain.CurrentDomain.BaseDirectory + "\\CodeClonerLog.txt";


    static Log()
    {
      using (StreamWriter sw = File.AppendText(logFile))
      {
        sw.WriteLine("==========================");
        sw.WriteLine("Code Cloner Log: " + DateTime.Now);
        sw.WriteLine("--------------------------");
      }
    }


    internal static void WriteLine(List<string> lines)
    { using (StreamWriter sw = File.AppendText(logFile))
    {
      foreach (string line in lines)
      {
        sw.WriteLine(line);
      }
    }
    }

    internal static void WriteLine(string line)
    {
      WriteLine(new List<string> {line});
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
