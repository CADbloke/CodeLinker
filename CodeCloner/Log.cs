using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCloner
{
  static class Log
  {
    private static string logFile = Environment.CurrentDirectory + "CodeClonerLog.txt";


    static Log()
    {
      using (StreamWriter sw = File.AppendText(logFile))
      {
        sw.WriteLine();
        sw.WriteLine("Code Cloner Log for " + DateTime.Now);
        sw.WriteLine();
      }
    }


    static void WriteLine(List<string> lines)
    { using (StreamWriter sw = File.AppendText(logFile))
    {
      foreach (string line in lines)
      {
        sw.WriteLine(line);
      }
    }
    }

    static void WriteLine(string line)
    {
      WriteLine(new List<string> {line});
    }


    private static readonly Destructor Finalise = new Destructor();

    // http://stackoverflow.com/a/18709110/492
    private sealed class Destructor
    {
      ~Destructor() // One time only destructor.
      {
        using (StreamWriter sw = File.AppendText(logFile))
        {
          sw.WriteLine();
          sw.WriteLine("Finished at" + DateTime.Now);
          sw.WriteLine("Closing Log.");
          sw.WriteLine();
        }
        
      }
    }


  }
}
