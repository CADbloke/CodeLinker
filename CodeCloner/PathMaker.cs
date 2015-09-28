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
  }
}
