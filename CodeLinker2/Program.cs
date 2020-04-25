// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;


namespace CodeLinker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            App.ParseCommands(args);
            App.Finish();
        }

        // https://stackoverflow.com/a/3133249/492
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            App.Crash( $"Unhandled Crash. Sorry. :(  {e?.ExceptionObject?.ToString()}");
        }
    }
}
