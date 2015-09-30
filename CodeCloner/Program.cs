using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeCloner
{
  class Program
  {
    static void Main(string[] args)
    {
      System.Diagnostics.Debugger.Launch(); // note: to find teh bugs
      int argsCount = args.Count();
      if (argsCount == 0)
      {
        Log.WriteLine("ERROR: No Args given so Help Text Displayed.");
        Help.Write();
        Finish();
      }

      List<string> argsList = args.ToList();

      if (argsList[0] == "/?")
      {
        Help.Write();
        Log.WriteLine("User asked For Help. Hope I helped.");
        Finish();
      }

      List<DestinationCsProjParser> cloners = new List<DestinationCsProjParser>();

      if (!string.IsNullOrEmpty(argsList[0]))
      {
        if (argsList[0].ToLower().EndsWith(".csproj"))
        {
          if (argsCount > 1 && args[1].ToLower().EndsWith(".csproj"))
          {
            cloners.Add(new DestinationCsProjParser(sourceCsProj: argsList[0], destCsProj: argsList[1]));
            Log.WriteLine("Starting Clone from: " + argsList[0] + "to :" + argsList[1]);
          }
          else
          {
            cloners.Add(new DestinationCsProjParser(destCsProj: argsList[0]));
            Log.WriteLine("Starting Clone to :" + argsList[0] + ". Source TBA.");
          }
        }





        else if (Directory.Exists(argsList[0]))
        {
          // todo: Build a list of Cloners by finding all the CSPROJ Files in the folder.
          // todo: Paths are relative to the destination csproj, not to the executing assembly.
          List<string> csprojList = new List<string>();
        }
        else
        {
          string errorMessage = "Your Args made no sense to me." + Environment.NewLine;
          foreach (string arg in args) { errorMessage += arg + Environment.NewLine; }
          Crash(errorMessage);
        }
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


      http://referencesource.microsoft.com/#XamlBuildTask/Microsoft/Build/Tasks/Xaml/GenerateTemporaryAssemblyTask.cs <== interesting
      */


      /* 
    note: Item Types:  
    MATCH
    Compile|None|Folder|EmbeddedResource|Resource|Res|AppDesigner|Page|Content|WCFMetadataStorage|Folder
    
    KEEP
    Condition
    
    EXCLUDE
    Reference|ProjectReference|BootstrapperPackage

    Log anything that is not one of these because it's a bug - I missed it

    cope with conditional inclusions
    Cope with absolute paths, including $(EnvironmentVariables)
    */

     
      Finish();
    }



    internal static void Finish(int exitCode = 0)
    {
      Console.WriteLine("Finished. Enter key to Exit."); // todo: delete this line when Logging is good.
      Console.ReadLine(); // todo: delete this line when Logging is good so VS runs without stopping.
      Environment.Exit(exitCode);
    }

    
    internal static void Crash(Exception e)
    {
      Log.WriteLine(e.ToString());
      Console.WriteLine(e.ToString());
      Finish(1);
    }

    public static void Crash(string errorMessage)
    {
      Log.WriteLine(errorMessage);
      Console.WriteLine(errorMessage);
      Finish(1);
    }
  }
}
