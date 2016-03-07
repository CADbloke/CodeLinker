// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

namespace CodeLinker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App.ParseCommands(args);
            App.Finish();
        }
    }
}
