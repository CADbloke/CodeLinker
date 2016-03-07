// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

namespace CodeLinker
{
    /// <summary> A project to Link. </summary>
    internal class ProjectToLink
    {
        /// <summary> Source project.  Full Path with file extension. </summary>
        public string SourceProject { get; set; }

        /// <summary> Destination project - just the file name, no path. Extension should be the same as the Source. </summary>
        public string DestinationProjectName { get; set; }
    }
}
