using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeLinker
{
  static class ProjectLinker
  {
    /// <summary> Copies and links a List of Projects into the <c>destinationFolder</c>. 
    ///    <para> File is Linked from the first <c>projectsToLink</c> for each if there is more than 1 source.</para></summary>
    /// <exception cref="ArgumentNullException">  Thrown when one or more required arguments are null or empty. </exception>
    /// <param name="projectsToLink">  The projects to LinkCodez. </param>
    /// <param name="destinationFolder">  Pathname of the destination folder. Empty string throws <c>ArgumentNullException</c></param>
    internal static void NewProject(List<ProjectToLink> projectsToLink, string destinationFolder )
    {
      if (projectsToLink == null) { throw new ArgumentNullException(nameof(projectsToLink)); }
      if (string.IsNullOrEmpty(destinationFolder)) { throw new ArgumentNullException(nameof(destinationFolder)); }

      HashSet<string> destinationProjects = new HashSet<string>(projectsToLink.Select(p => p.DestinationProjectName));
      Log.WriteLine("Recycling "+ destinationProjects.Count + " Projects to " + destinationFolder);

      foreach (string destinationProject in destinationProjects)
      {
        string destinationProjPath = Path.Combine(destinationFolder, destinationProject);
        if (File.Exists(destinationProjPath))
        {
          bool overwriteExisting = YesOrNo.Ask(destinationProjPath + Environment.NewLine + " already exists!" + Environment.NewLine + "Overwrite it ?");
          if (!overwriteExisting) continue;
        }

        List<string> sources = projectsToLink.Where(d => d.DestinationProjectName == destinationProject)
                                             .Select(s => s.SourceProject).ToList();
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
            Log.WriteLine("User skipped one Linked Project. "+ message);
            continue; // skip just this Destination Project
          } 
        }

        if (sources.Any())
        {
          Log.WriteLine("Recycling to :" + destinationProjPath );
          File.Copy(sources[0], destinationProjPath, overwrite: true);
          DestinationProjXml destinationProjXml = new DestinationProjXml(destinationProjPath);
          destinationProjXml.ClearOldLinkedCode();
          destinationProjXml.ClearStartPlaceholderContent();
          destinationProjXml.AddExclusion("app.config"); // tends to break a build if/when you change frameworks. Don't link it
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

          Linker.LinkCodez(new []{destinationProjPath});
        }
      }
    }


    internal static void NewProject(ProjectToLink projectsToLink, string destinationFolder)
    {
      NewProject(new List<ProjectToLink> {projectsToLink }, destinationFolder);
    }

    internal static void NewProject(string projectToLink, string destinationFolder)
    {
      NewProject(new List<ProjectToLink> { new ProjectToLink {SourceProject = projectToLink} }, destinationFolder);
    }
  }

}
