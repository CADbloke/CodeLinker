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
      if (IsAbsolutePath(possibleRelativePath) )  { return possibleRelativePath; }

      if (string.IsNullOrEmpty(basePath)) basePath = AppDomain.CurrentDomain.BaseDirectory;
      if (!basePath.EndsWith("\\"))       basePath += "\\";
      string properAbsolutePath = "";

      try
      {
        properAbsolutePath = Path.GetFullPath(basePath + possibleRelativePath); // http://stackoverflow.com/a/1299356/492
      }
      catch (Exception e) { Program.Crash(e); }

      bool dir  = Directory.Exists(properAbsolutePath);
      bool file = File.Exists(properAbsolutePath);

      if (!dir && !file) { Program.Crash("ERROR: Bad Path: " + properAbsolutePath); }
      return properAbsolutePath;
    }

    internal static bool IsAbsolutePath(string possibleRelativePath)
    {
      if (possibleRelativePath.StartsWith("$("))   { return true; } // starts with Environment Variable - don't break it.
      if (Directory.Exists(possibleRelativePath))  { return true; }
      if (File.Exists(possibleRelativePath))       { return true; }
      //if (Path.IsPathRooted(possibleRelativePath)) { return true; }
      return false;
    }
  }
}

