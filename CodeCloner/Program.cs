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
            Log.WriteLine("Starting Clone Code from: " + argsList[0] + " to " + argsList[1]);
            cloners.Add(new DestinationCsProjParser(sourceCsProj: argsList[0], destCsProj: argsList[1]));
          }
          else
          {
            Log.WriteLine("Starting Clone Code to: " + argsList[0] + ". Source TBA.");
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
              Log.WriteLine("Starting Clone Code to: " + destCsprojFile + ". Source TBA.");
              cloners.Add(new DestinationCsProjParser(destCsprojFile));
            }


            // todo: Build a list of Cloners by finding all the CSPROJ Files in the folder.
            // todo: Paths are relative to the destination csproj, not to the executing assembly.
          }
            catch (Exception e) { Crash(e); }
        }


        if (!cloners.Any()) 
        {
          string errorMessage = "I got nuthin. Your Args made no sense to me." + Environment.NewLine;
          foreach (string arg in args) { errorMessage += arg + Environment.NewLine; }
          Crash(errorMessage);
        }
      }

      // todo: diff: http://www.scootersoftware.com/v4help/index.html?scripting_reference.html load "session name" - save session in repo.

      Finish();
    }



    internal static void Finish(string message = "", int exitCode = 0)
    {
      if (!string.IsNullOrEmpty(message)) Console.WriteLine(message); // todo: delete this line when Logging is good.
      Console.WriteLine("Finished. Enter key to Exit."); // todo: delete this line when Logging is good.
      Console.ReadLine(); // todo: delete this line when Logging is good so VS runs without stopping.
      Environment.Exit(exitCode);
    }

    
    internal static void Crash(Exception e)
    {
      Log.WriteLine(e.ToString());
      Log.WriteLine(e.InnerException?.ToString());
      Console.WriteLine(e.ToString());
      Finish("",1);
    }

    public static void Crash(string errorMessage)
    {
      Log.WriteLine(errorMessage);
      Console.WriteLine(errorMessage);
      Finish("",1);
    }
  }
}
