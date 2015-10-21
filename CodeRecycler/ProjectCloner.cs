using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeRecycler
{
  static class ProjectCloner
  {
    /// <summary> Clones a List of Projects into the <c>destinationFolder</c>. 
    ///    <para> File is cloned from the first <c>projectsToRecycle</c> for each if there is more than 1 source.</para></summary>
    /// <exception cref="ArgumentNullException">  Thrown when one or more required arguments are null or empty. </exception>
    /// <param name="projectsToRecycle">  The projects to recycle. </param>
    /// <param name="destinationFolder">  Pathname of the destination folder. Empty string throws <c>ArgumentNullException</c></param>
    internal static void Clone(List<ProjectToRecycle> projectsToRecycle, string destinationFolder )
    {
      if (projectsToRecycle == null) { throw new ArgumentNullException(nameof(projectsToRecycle)); }
      if (string.IsNullOrEmpty(destinationFolder)) { throw new ArgumentNullException(nameof(destinationFolder)); }

      HashSet<string> destinationProjects = new HashSet<string>(projectsToRecycle.Select(p => p.DestinationProjectName));
      Log.WriteLine("Recycling "+ destinationProjects.Count + " Projects to " + destinationFolder);

      foreach (string destinationProject in destinationProjects)
      {
        string destinationProjPath = Path.Combine(destinationFolder, destinationProject);
        if (File.Exists(destinationProjPath))
        {
          bool overwriteExisting = YesOrNo.Ask(destinationProjPath + Environment.NewLine + " already exists!" + Environment.NewLine + "Overwrite it ?");
          if (!overwriteExisting) continue;
        }

        List<string> sources = projectsToRecycle.Where(d => d.DestinationProjectName == destinationProject).Select(s => s.SourceProject).ToList();
        if (sources.Count!=1)
        {
          string message = destinationProject + "has " + sources.Count + " source Projects." + Environment.NewLine;
          for (int index = 0; index < sources.Count; index++)
          {
            string source = sources[index];
            message += (index + 1).ToString() + ". " + source + Environment.NewLine;
          }
          message += "Continue or skip " + destinationProject + " or Cancel Everything?";

          bool? carryOn = YesOrNo.OrCancel(message);
          if (carryOn == null)
          {
            Log.WriteLine("User aborted All Recycling. "+ message);
            return; // bail out of everything
          }   
          if (carryOn == false)
          {
            Log.WriteLine("User skipped one Recycled Project. "+ message);
            continue; // skip just this Destination Project
          } 
        }

        if (sources.Any())
        {
          Log.WriteLine("Recycling to :" + destinationProjPath );
          File.Copy(sources[0], destinationProjPath, overwrite: true);
          DestinationProjXml destinationProjXml = new DestinationProjXml(destinationProjPath);
          destinationProjXml.ClearOldRecycledCodeLinks();
          destinationProjXml.ClearExistingCodeExceptLinked();

          foreach (string source in sources)
          {
            if (File.Exists(source))
            {
              string relativeSource = PathMaker.MakeRelativePath(destinationProjPath, source);
              destinationProjXml.StartPlaceHolder.Value += Environment.NewLine + 
                Settings.SourcePlaceholderLowerCase + " " + relativeSource;
              Log.WriteLine("added Source: " + relativeSource);
            }
            else
            {
              Log.WriteLine("Bad Source in: " + destinationProjPath + Environment.NewLine + 
                            "       Source: " + source + Environment.NewLine);
            }
          }

          destinationProjXml.DestProjXdoc.Save(destinationProjXml.DestProjAbsolutePath);
          Log.WriteLine("saved: " + destinationProjXml.DestProjAbsolutePath);

          Recycler.Recycle(new []{destinationProjPath});
        }
      }
    }


    internal static void Clone(ProjectToRecycle projectsToRecycle, string destinationFolder)
    {
      Clone(new List<ProjectToRecycle> {projectsToRecycle }, destinationFolder);
    }

    internal static void Clone(string projectToRecycle, string destinationFolder)
    {
      Clone(new List<ProjectToRecycle> { new ProjectToRecycle {SourceProject = projectToRecycle} }, destinationFolder);
    }
  }

}
