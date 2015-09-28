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
      if (!args.Any())
      {
        Log.WriteLine("ERROR: No Args given so Help Text Displayed.");
        Help.Write();
        Finish();
      }

      string firstArg = args[0];
      
      if (firstArg == "/?")
      {
        Help.Write();
        Log.WriteLine("User asked For Help. Hope I helped.");
        Finish();
      }

      /* todo: 
      parse source CSPROJ ItemGroups
      parse source CSPROJ items in ItemGroups
       - Include = 
       - None = 
      calculate new paths for destination CSPROJ
      Build XML to be inserted into destination CSPROJ
      
      write items 
          Possible YAGNI: 
          parse destination CSPROJ Cloned Code to check for changes
          Add a comment to destination CSPROJ about the rewrite (meh - Git history will track this, as will the log)
          ONLY if anything changed so source control doesn't get too many checkins
          - test first because Git shouldn't try to check in code that has not actually changed.
      Log it. Source control can be used to check the actual changes so just log that it actually changed.
      */

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
