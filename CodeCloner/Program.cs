using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

      // https://msdn.microsoft.com/en-us/library/ms404278(v=vs.110).aspx  Common I/O Tasks
      // 
      /* todo: 
      Build list of Source / Dest CSPROJ - this may only be one depending on args syntax used.
      foreach Source/Dest CSPROJ ...
      parse source CSPROJ ItemGroups 
      parse source CSPROJ items in ItemGroups
       - Include = 
       - None = 
        - any existing code that doesn't have a link attribute may be in error. Flag it.
      calculate new paths for destination CSPROJ
      Build XML to be inserted into destination CSPROJ
      
      write items 
          Possible YAGNI: 
          parse destination CSPROJ Cloned Code to check for changes
          Add a comment to destination CSPROJ about the rewrite (meh - Git history will track this, as will the log)
          ONLY if anything changed so source control doesn't get too many checkins
          - test first because Git shouldn't try to check in code that has not actually changed.
      Log it. Source control can be used to check the actual changes so just log that it actually changed.
      diff: http://www.scootersoftware.com/v4help/index.html?scripting_reference.html load "session name" - save session in repo.
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
      Console.WriteLine("Finished. Enter key to Exit."); // todo: delete this line when Logging is good.
      Console.ReadLine(); // todo: delete this line when Logging is good so VS runs without stopping.
      Environment.Exit(0);
    }
  }
}
