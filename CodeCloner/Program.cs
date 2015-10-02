using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeCloner
{
  class Program
  {
    static List<DestinationCsProjParser> cloners = new List<DestinationCsProjParser>();
    static void Main(string[] args)
    {
        System.Diagnostics.Debugger.Launch(); // to find teh bugs
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

      

      if (!string.IsNullOrEmpty(argsList[0]))
      {
        if (argsList[0].ToLower().EndsWith(".csproj"))
        {
          if (argsCount > 1 && args[1].ToLower().EndsWith(".csproj"))
          {
            Log.WriteLine("Queueing Code Clone from: " + argsList[0] + " to " + argsList[1]);
            cloners.Add(new DestinationCsProjParser(sourceCsProj: argsList[0], destCsProj: argsList[1]));
          }
          else
          {
            Log.WriteLine("Queueing Code Clone to: " + argsList[0] + ". Source TBA.");
            cloners.Add(new DestinationCsProjParser(destCsProj: argsList[0]));
          }
        }

        else
        {
          string destinationsDirectory = argsList[0];
          if (!PathMaker.IsAbsolutePath(destinationsDirectory))
            destinationsDirectory = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, destinationsDirectory);

          if (Directory.Exists(destinationsDirectory))
          try
          {
            List<string> destCsprojFiles = new List<string>();

            bool subDirectories = args.Contains("/s");
            SearchOption includeSubs = subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            destCsprojFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.csproj", includeSubs));

            foreach (string destCsprojFile in destCsprojFiles)
            {
              Log.WriteLine("Queueing Code Clone to: " + destCsprojFile + ". Source TBA.");
              cloners.Add(new DestinationCsProjParser(destCsprojFile));
            }
          }
            catch (Exception e) { Crash(e, "Queueing Code Clone didn't work. Bad file name?"); }
        }


        if (!cloners.Any()) 
        {
          string errorMessage = "I got nuthin. Your Args made no sense to me." + Environment.NewLine;
          foreach (string arg in args) { errorMessage += arg + Environment.NewLine; }
          Crash(errorMessage);
        }
      }

      Finish();
    }



    internal static void Finish(string message = "")
    {
      message += Environment.NewLine + "Finished cloning " + cloners.Count + " Project"; // writes to VS window
      if (cloners.Count != 1) { message += "s"; }
      message += "." + Environment.NewLine;

      foreach (DestinationCsProjParser cloner in cloners)
      {
        message += "from " + String.Join(",", cloner.SourceCsProjList);
        message += " to " + cloner.DestCsProjAbsolutePath + Environment.NewLine;
      }
      Console.WriteLine(message);
      Environment.Exit(0);
    }

    
    internal static void Crash(Exception e, string crashedAt = "")
    {
      Log.WriteLine("");
      Log.WriteLine("I crashed at "+crashedAt+". Whups.");
      Log.WriteLine(e.ToString());
      Log.WriteLine(e.InnerException?.ToString());
      Log.WriteLine(e.StackTrace);
      Console.WriteLine(e.ToString());
      Environment.Exit(1);
    }

    public static void Crash(string errorMessage)
    {
      Log.WriteLine(errorMessage);
      Console.WriteLine(errorMessage);
      Environment.Exit(1);
    }
  }
}

/* todo: <ApplicationIcon>Resources\CADbloke favicon.ico</ApplicationIcon>
this may be a case of using a delmited list of strings to choose which XML elements need to come across
in this case "PropertyGroup, ApplicationIcon"
"Reference,HintPath" <== if relative but this may actually break more than it fixes because it depends on build settings - perhaps remove all the hint paths so it generates a real error if it needs Nugetting. Report in the Log file that you did this.

  */
