using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCloner
{
  class Program
  {
    static void Main(string[] args)
    {
      if (!args.Any()) { args = new[] {"/?"}; }

      string firstArg = args[0];

      if (firstArg == "/?") { Help.Write(); Finish();       }

      switch (firstArg) {


        case "/?":
        default:
          break;
      }
      Finish();
    }

    private static void Finish()
    {
      Console.WriteLine();
      Console.WriteLine("Finished. Enter key to Exit.");
      Console.ReadLine();
      Environment.Exit(0);
    }
  }
}
