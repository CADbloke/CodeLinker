using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeRecycler
{
  public static class Recycler
  {
    static List<DestinationProjParser> recyclers = new List<DestinationProjParser>();
    public static void Run(string[] args)
    {
      // System.Diagnostics.Debugger.Launch(); // to find teh bugs load this in Visual Studio and uncomment the start of this line.
      int argsCount = args.Length;
      if (argsCount == 0)
      {
        Log.WriteLine("No Args given so Help Text Displayed.");
        Log.WriteLine();
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
        if (argsList[0].IsaCsOrVbProjFile())
        {
          if (argsCount > 1 && args[1].IsaCsOrVbProjFile())
          {
            Log.WriteLine("Queueing Code Recycle from: " + argsList[0] + " to " + argsList[1]);
            recyclers.Add(new DestinationProjParser(sourceProj: argsList[0], destProj: argsList[1]));
          }
          else
          {
            Log.WriteLine("Queueing Code Recycle to: " + argsList[0] + ". Source TBA.");
            recyclers.Add(new DestinationProjParser(destProj: argsList[0]));
          }
        }

        else if (argsList[0].ToLower() == "strip")
        {
          if (argsCount > 1)
          {
            if (args[1].IsaCsOrVbProjFile())
            {
              ProjectStripper projectStripper = new ProjectStripper(args[1]);
              projectStripper.Strip();
              Finish("Stripped all code from " + args[1]);
            }

            else
            {
              try
              {
                List<string> destProjFiles = GetProjectsFromFolders(argsList[1]);

                foreach (string destProjFile in destProjFiles)
                {
                  Log.WriteLine("Stripping Code from: " + destProjFile + ". ");
                  ProjectStripper projectStripper = new ProjectStripper(destProjFile);
                  projectStripper.Strip();
                }
              }
              catch (Exception e) { Crash(e, "Stripping Code from  didn't work. Bad file name?"); }
              Finish("Stripped all code");
            }
          }
        }

        else
        {
          try
          {
            List<string> destProjFiles = GetProjectsFromFolders(argsList[0]);

            foreach (string destProjFile in destProjFiles)
            {
              Log.WriteLine("Queueing Code Recycle to: " + destProjFile + ". Source TBA.");
              recyclers.Add(new DestinationProjParser(destProjFile));
            }
          }
            catch (Exception e) { Crash(e, "Queueing Code Recycle didn't work. Bad file name?"); }
        }


        if (!recyclers.Any()) 
        {
          string errorMessage = "I got nuthin. Your Args made no sense to me." + Environment.NewLine;
          foreach (string arg in args) { errorMessage += arg + Environment.NewLine; }
          Crash(errorMessage);
        }
      }

      Finish();
    }


    private static List<string> GetProjectsFromFolders(string rootFolder, bool subDirectories = false)
    {
      string destinationsDirectory = rootFolder;
          if (!PathMaker.IsAbsolutePath(rootFolder))
            destinationsDirectory = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, rootFolder);

          if (Directory.Exists(destinationsDirectory))
          try
          {
            List<string> destProjFiles = new List<string>();

            SearchOption includeSubs = subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            destProjFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.csproj", includeSubs));
			      destProjFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.vbproj", includeSubs));

            return destProjFiles;
          }
        catch (Exception e) { Crash(e, "Queueing Code Recycle in " + rootFolder+ " didn't work. Bad file name?"); }

        return new List<string>();
        }


    internal static void Finish(string message = "")
    {
      message += "Finished recycling " + recyclers.Count + " Project"; // writes to VS window
      if (recyclers.Count != 1) { message += "s"; }
      message += "." + Environment.NewLine;

      foreach (DestinationProjParser recycler in recyclers)
      {
        message += "from " + String.Join(",", recycler.SourceProjList.ToArray());
        message += " to " + recycler.DestProjAbsolutePath + Environment.NewLine;
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
