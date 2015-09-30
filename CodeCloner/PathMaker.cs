using System;
using System.IO;

namespace CodeCloner
{
  internal static class PathMaker
  {
    /// <summary> Creates a relative path from one file or folder to another.
    ///   Returns <c>toPath</c> if paths are not related. </summary>
    /// <exception cref="ArgumentNullException">. </exception>
    /// <param name="fromPath"> Contains the directory that defines the start of the relative path. </param>
    /// <param name="toPath">   Contains the path that defines the endpoint of the relative path. </param>
    /// <returns> The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
    /// <exception cref="UriFormatException">. </exception>
    /// <exception cref="InvalidOperationException">. </exception>
    /// <!-- from http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/340454#340454 -->
    internal static string MakeRelativePath(string fromPath, string toPath)
    {
      if (string.IsNullOrEmpty(fromPath)) { throw new ArgumentNullException(nameof(fromPath)); }
      if (string.IsNullOrEmpty(toPath))   { throw new ArgumentNullException(nameof(toPath)); }

      Uri fromUri = new Uri(fromPath);
      Uri toUri   = new Uri(toPath);

      if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (toUri.Scheme.ToUpperInvariant() == "FILE")
      {
        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      }

      return relativePath;
    }

    /// <summary> Always returns an Absolute Path from a Path that is possibly Relative, possibly Absolute. </summary>
    /// <param name="possibleRelativePath"> a Path that is possibly relative, possibly Absolute. 
    ///                                     In any case this always returns an Absolute Path. </param>
    /// <param name="basePath">             Optional: Base Path if building an Absolute Path from a relative path.
    ///                                     Defaults to the current execution folder if <c>null</c>. </param>
    internal static string MakeAbsolutePathFromPossibleRelativePathOrDieTrying(string basePath, string possibleRelativePath)
    {
      // bug: if the source is specified in hte destination csproj placeholder then the relative path is relative to the CSPROJ, not to the current Directory
      if (Path.IsPathRooted(possibleRelativePath)) { return possibleRelativePath; }

      if (basePath == null) { basePath = Environment.CurrentDirectory; }

      string properAbsolutePath = "";

      try
      {
        string absolutePath = Path.Combine(basePath, possibleRelativePath);
        properAbsolutePath = Path.GetFullPath((new Uri(absolutePath)).LocalPath);
      }
      catch (Exception e) {
        Program.Crash(e);
      }

      if (!Directory.Exists(properAbsolutePath) && !File.Exists(properAbsolutePath)) { Program.Crash("ERROR: Cannot Build Path"); }
      return properAbsolutePath;
    }

 
  }


}

// note: Path.IsPathRooted to identify absolute paths
