using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CodeLinker
{
  public static class App
  {
    private static List<DestinationProjLinker> linkers = new List<DestinationProjLinker>();
    internal static bool NoConfirm = false;

    public static void ParseCommands(string[] args)
    {
      linkers.Clear();
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

      bool doSubDirectories = (argsList.Contains("/s", StringComparer.CurrentCultureIgnoreCase));
      NoConfirm = (argsList.Contains("/noconfirm", StringComparer.CurrentCultureIgnoreCase));


      if (!string.IsNullOrEmpty(argsList[0]))
      {
        if (argsList[0].IsaCsOrVbProjFile())
        {
          if (argsCount > 1 && args[1].IsaCsOrVbProjFile())
          {
            Log.WriteLine("Queueing Code Link from: " + argsList[0] + " to " + argsList[1]);
            linkers.Add(new DestinationProjLinker(sourceProj: argsList[0], destProj: argsList[1]));
          } 
          else
          {
            Log.WriteLine("Queueing Code Link to: " + argsList[0] + ". Source TBA.");
            linkers.Add(new DestinationProjLinker(destProj: argsList[0]));
          }
        }

        else if (argsList[0].ToLower() == "strip")
        {
          if (argsCount > 1)
          {
            if (args[1].IsaCsOrVbProjFile())
            {
              DestinationProjXml destinationProjXml = new DestinationProjXml(args[1]);
              destinationProjXml.ClearOldLinkedCode();
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
                  destinationProjXml.ClearOldLinkedCode();
                  destinationProjXml.Save();
                }
              }
              catch (Exception e)
              {
                Crash(e, "Stripping Code from Folder: " + args[1] + " didn't work. Bad name?");
              }
              Finish("Stripped all code");
            }
          }
        }

        else if (argsList[0].ToLower() == "new")
        {
          if (argsCount > 2)
          {
            if (args[1].IsaCsOrVbProjFile())
            {
              string sourcePath = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, args[1]);
              try
              {
                ProjectMaker.NewProject(sourcePath, args[2]);
              }
              catch (Exception e)
              {
                Crash(e, "Linking " + args[1] + " to " + args[2] + " didn't work. Bad name?");
              }

              Finish("Linked " + " from " + args[1] + " to " + args[2]);
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
                    try
                    {
                      ProjectMaker.NewProject(sourcePath, args[2]);
                    }
                    catch (Exception e)
                    {
                      Crash(e, "Linking " + sourceProjFile + " to " + args[2] + " didn't work. Bad name?");
                    }

                    Log.WriteLine("Linked " + " from " + sourceProjFile + " to " + args[2]);
                  }
                  else
                  {
                    Log.WriteLine("ERROR: " + sourceProjFile + " is not a project file. Cannot Link it.");
                  }
                }
              }
              catch (Exception e)
              {
                Crash(e, "Linking Projects from Folder: " + args[1] + " didn't work. Bad name?");
              }

              Finish("Linked Projects");
            }
          }
        } // /NewProject

        else // vanilla Link command with a folder
        {
          try
          {
            List<string> destProjFiles = GetProjectsFromFolders(argsList[0], doSubDirectories);

            foreach (string destProjFile in destProjFiles)
            {
              Log.WriteLine("Queueing Code Link to: " + destProjFile + ". Source TBA.");
              linkers.Add(new DestinationProjLinker(destProjFile));
            }
          }
          catch (Exception e)
          {
            Crash(e, "Queueing Code Link didn't work. Bad file name?");
          }
        }



        if (!linkers.Any())
        {
          string errorMessage = "I got nuthin. Your Args made no sense to me." + Environment.NewLine;
          foreach (string arg in args)
          {
            errorMessage += arg + Environment.NewLine;
          }
          Crash(errorMessage);
        }


        foreach (DestinationProjLinker destinationProjLinker in linkers)
        {
          destinationProjLinker.LinkCode();
        }
      }
    }


    private static List<string> GetProjectsFromFolders(string rootFolder, bool doSubDirectories = false)
    {
      string destinationsDirectory = rootFolder;
      if (!PathMaker.IsAbsolutePath(rootFolder))
      {
        destinationsDirectory = PathMaker.MakeAbsolutePathFromPossibleRelativePathOrDieTrying(null, rootFolder);
      }

      if (Directory.Exists(destinationsDirectory))
      {
        try
        {
          List<string> destProjFiles = new List<string>();

          SearchOption includeSubs = doSubDirectories
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

          destProjFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.csproj", includeSubs));
          destProjFiles.AddRange(Directory.GetFiles(destinationsDirectory, "*.vbproj", includeSubs));

          return destProjFiles;
        }
        catch (Exception e)
        {
          Crash(e, "Getting Projects in " + rootFolder + " didn't work. Bad name?");
        }
      }

      return new List<string>(); // because complain no return path
    }


    internal static void Finish(string message = "")
    {
      message += "Finished recycling " + linkers.Count + " Project"; // writes to VS window
      if (linkers.Count != 1)
      {
        message += "s";
      }
      message += "." + Environment.NewLine;

      foreach (DestinationProjLinker linker in linkers)
      {
        message += "from :" + String.Join(",", linker.SourceProjList.ToArray());
        message += "  to :" + linker.DestProjAbsolutePath + Environment.NewLine;
      }
      Console.WriteLine(message);
      Environment.Exit(0);
    }


    internal static void Crash(Exception e, string crashedAt = "")
    {
      string message = "I crashed at " + crashedAt + ". Whups. See CodeLinkerLog.txt for details.";
      YesOrNo.Crashing(message);
      Log.WriteLine();
      Log.WriteLine(message);
      Log.WriteException(e);
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
