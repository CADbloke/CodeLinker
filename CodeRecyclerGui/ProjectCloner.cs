using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRecycler;

namespace CodeRecyclerGui
{
  static class ProjectCloner
  {
    /// <summary> Clones a List of Projects into the <c>destinationFolder</c>. </summary>
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
        string destinationProjectPath = Path.Combine(destinationFolder, destinationProject);
        if (File.Exists(destinationProjectPath))
        {
          bool overwriteExisting = YesOrNo.Ask(destinationProjectPath + Environment.NewLine + " already exists!" + Environment.NewLine + "Overwrite it ?");
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
            return;
          }   // bail out of everything
          if (carryOn == false)
          {
            Log.WriteLine("User skipped one Recycled Project. "+ message);
            continue;
          } // skip this Destination Project
        }
        if (sources.Any() && File.Exists(sources[0]))
        {
          Log.WriteLine("Recycling to :" + destinationProjectPath );
          File.Copy(sources[0], destinationProjectPath, overwrite: true);
          DestinationProjectXml destinationProjectXml = new DestinationProjectXml(destinationProjectPath);
          destinationProjectXml.ClearOldRecycledCodeLinks();
          destinationProjectXml.ClearExistingCodeExceptLinked();

          foreach (string source in sources)
          {
            string relativeSource = PathMaker.MakeRelativePath(destinationProjectPath, source);
            destinationProjectXml.StartPlaceHolder.Value += Environment.NewLine + Settings.SourcePlaceholderLowerCase + " " + relativeSource;
            Log.WriteLine("added Source: " + relativeSource);
          }
          destinationProjectXml.DestProjXdoc.Save(destinationProjectXml.DestProjAbsolutePath);
          Log.WriteLine("saved: " + destinationProjectXml.DestProjAbsolutePath);

          Recycler.Recycle(new []{destinationProjectPath});
        }
      }
    }
  }
}
