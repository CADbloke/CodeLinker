// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;


namespace CodeLinker
{
    public static class IsaCsOrVbProjectFile
    {
        /// <summary> A string extension method checks if 'fileName' is a Visual Studio C# or VB project file by checking the file
        ///     extension. </summary>
        /// <param name="fileName"> The fileName to check. Can be a full path or just the file name. </param>
        /// <returns> true if it is a Visual Studio C# or VB project file, false if not. </returns>
        public static bool IsaCsOrVbProjFile(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            string tidiedFileName = fileName.ToLower().Trim();

            if (tidiedFileName.EndsWith(".csproj", StringComparison.Ordinal))
                return true;

            if (tidiedFileName.EndsWith(".vbproj", StringComparison.Ordinal))
                return true;

            return false;
        }
    }
}
