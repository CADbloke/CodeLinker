﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CodeRecycler
{
  public static class Recycler
  {
    static List<DestinationProjRecycler> recyclers = new List<DestinationProjRecycler>();
    internal static bool NoConfirm = false;
    public static void Recycle(string[] args)
    {
      recyclers.Clear();
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

      if (argsList.Contains("/?"))
      {
        Help.Write();
        Log.WriteLine("User asked For Help. Hope I helped.");
        Finish();
      }

      bool doSubDirectories = (argsList.Contains("/s",         StringComparer.CurrentCultureIgnoreCase));
      NoConfirm             = (argsList.Contains("/noconfirm", StringComparer.CurrentCultureIgnoreCase));


      if (!string.IsNullOrEmpty(argsList[0]))
      {
        if (argsList[0].IsaCsOrVbProjFile())
        {
          if (argsCount > 1 && args[1].IsaCsOrVbProjFile())
          {
            Log.WriteLine("Queueing Code Recycle from: " + argsList[0] + " to " + argsList[1]);
            recyclers.Add(new DestinationProjRecycler(sourceProj: argsList[0], destProj: argsList[1]));
          }
          else
          {
            Log.WriteLine("Queueing Code Recycle to: " + argsList[0] + ". Source TBA.");
            recyclers.Add(new DestinationProjRecycler(destProj: argsList[0]));
          }
        }

        else if (argsList[0].ToLower() == "strip")
        {
          if (argsCount >1)
          {
            if (args[1].IsaCsOrVbProjFile())
            {
              DestinationProjXml destinationProjXml = new DestinationProjXml(args[1]); 
              destinationProjXml.ClearOldRecycledCodeLinks();
              destinationProjXml.Save();
              Finish("Stripped all code from " + args[1]);
            }

            else
            {
              try
              {
                List<string> destProjFiles = GetProjectsFromFolders(argsList[1], doSubDirectories);

                foreach (string destProjFile in destProjFiles)
                {
                  Log.WriteLine("Stripping Code from: " + destProjFile + ". ");
                  DestinationProjXml destinationProjXml = new DestinationProjXml(destProjFile);
                  destinationProjXml.ClearOldRecycledCodeLinks();
                  destinationProjXml.Save();
                }
              }
              catch (Exception e) { Crash(e, "Stripping Code from Folder: "+ args[1] + " didn't work. Bad name?"); }
              Finish("Stripped all code");
            }
          }
        }

        else if (argsList[0].ToLower() == "clone")
        {
          if (argsCount >2)
          {
            if (args[1].IsaCsOrVbProjFile())
            {
              string sourcePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, args[1]);
              try { ProjectCloner.Clone(sourcePath, args[2]); }
              catch (Exception e)
              { Crash(e, "Cloning "+ args[1] +  " to " + args[2] + " didn't work. Bad name?"); }

              Finish("Cloned " + " from " + args[1] +  " to " + args[2]);
            }

            else
            { 
              try
              {
                List<string> sourceProjFiles = GetProjectsFromFolders(argsList[1], doSubDirectories);

                foreach (string sourceProjFile in sourceProjFiles)
                {
                  if (sourceProjFile.IsaCsOrVbProjFile())
                  {
                    string sourcePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, sourceProjFile);
                    try { ProjectCloner.Clone(sourcePath, args[2]); }
                    catch (Exception e)
                     { Crash( e, "Cloning " + sourceProjFile + " to " + args[2] + " didn't work. Bad name?"); }

                    Log.WriteLine("Cloned " + " from " + sourceProjFile + " to " + args[2]);
                  }
                  else
                  {
                    Log.WriteLine("ERROR: " + sourceProjFile + " is not a project file. Cannot clone it.");
                  }
                }
              }
              catch (Exception e) { Crash( e, "Cloning Projects from Folder: " + args[1] + " didn't work. Bad name?"); }

              Finish("Cloned Projects");
            }
          }
        } // /Clone

        else // vanilla RECYCLE command with a folder
        {
          try
          {
            List<string> destProjFiles = GetProjectsFromFolders(argsList[0], doSubDirectories);

            foreach (string destProjFile in destProjFiles)
            {
              Log.WriteLine("Queueing Code Recycle to: " + destProjFile + ". Source TBA.");
              recyclers.Add(new DestinationProjRecycler(destProjFile));
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


        foreach (DestinationProjRecycler destinationProjRecycler in recyclers)
        {
          destinationProjRecycler.RecycleCode();
        }
      }
    }


    private static List<string> GetProjectsFromFolders(string rootFolder, bool subDirectories = false)
    {
      string destinationsDirectory = rootFolder;
      if (!PathMaker.IsAbsolutePath(rootFolder)) {
        destinationsDirectory = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, rootFolder);
      }

      if (Directory.Exists(destinationsDirectory))
      {
        try
        {
          List<string> destProjFiles = new List<string>();

          SearchOption includeSubs = subDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
          destProjFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.csproj", includeSubs));
          destProjFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.vbproj", includeSubs));

          return destProjFiles;
        }
        catch (Exception e)
         { Crash(e, "Getting Projects in " + rootFolder + " didn't work. Bad name?"); }
      }

      return new List<string>(); // because complain no return path
    }


    internal static void Finish(string message = "")
    {
      message += "Finished recycling " + recyclers.Count + " Project"; // writes to VS window
      if (recyclers.Count != 1) { message += "s"; }
      message += "." + Environment.NewLine;

      foreach (DestinationProjRecycler recycler in recyclers)
      {
        message += "from :" + String.Join(",", recycler.SourceProjList.ToArray());
        message += " to  :" + recycler.DestProjAbsolutePath + Environment.NewLine;
      }
      Console.WriteLine(message);
      Environment.Exit(0);
    }

    
    internal static void Crash(Exception e, string crashedAt = "")
    {
      Log.WriteLine();
      Log.WriteLine("I crashed at "+ crashedAt +". Whups.");
      Log.WriteLine(e.ToString());
      Log.WriteLine(e.InnerException?.ToString());
      Log.WriteLine(e.StackTrace);
      Console.WriteLine(e.ToString());
      throw e;
    }

    internal static void Crash(string errorMessage)
    {
      Log.WriteLine(errorMessage);
      Console.WriteLine(errorMessage);
      throw new Exception("Crashed. See Log file for details");
    }
  }
}
