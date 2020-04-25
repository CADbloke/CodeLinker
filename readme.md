# Code Linker 
## Reuses source code between CSPROJ and VBPROJ files

Code Linker creates links in the destination `.csproj` project file to the code files in the source `.csproj`, automating the process of [adding existing files as-a-link](https://msdn.microsoft.com/en-us/library/windows/apps/jj714082(v=vs.105).aspx) into the destination project. The files are added as relative paths (back to the original) if they are already relative links. If they were originally absolute paths then that is preserved.  If they already have a link I'll try not to break it.

The original version was a fustercluck example of YAGNI. CodeLinker2 is the one you want. Here is how that works ....

# Code Linker v2 - because v1 confused even me and I wrote it
Links Source code from CSPROJ and/or VBPROJ files
Does NOT Convert C# / VB between CSPROJ and VBPROJ files.
Don't cross the streams.
Anywhere this mentions CSPROJ you can also use VBPROJ.
Anywhere this mentions .CS you can also use .VB, or any file you need.

Usages...
CODELINKER /?
CODELINKER [Destination.csproj]
Destination.csproj   Path to the existing Destination project.", ConsoleColor.White, ConsoleColor.DarkBlue);

 - Wrap paths with spaces in double quotes.
 - Paths can (should!) be relative.
   You need to have the source project in the placeholder.


The Destination CSPROJ file needs this XML comment placeholder...", ConsoleColor.White, ConsoleColor.DarkRed);
If it doesn't have the placeholder then it soon will., I will add one for you


<!-- CodeLinker

Source: PathTo\\NameOfProject.csproj     <== this is NOT optional. You have multiples of these

Exclude: PathTo\\FileToBeExcluded.cs     <== optional - a partial match will exlude it

Include: PathTo\\FileToBeIncluded.cs     <== optional but it ONLY includes matches
-->

<!-- EndCodeLinker -->

 - You may specify multiple Source: projects. No wildcards. // todo: orly ???
 - You may specify multiple Exclude: &/or Include: items. The all apply to all projects
 - File name if in the same folder, or relative or absolute path. Wildcards are OK.
 - In/Exclusions are a simple VB LIKE String wildcard match.
 - Exclusions override Any Inclusions.
 - If you specify no Inclusions then everything is an Inclusion.

 - Multiple Source: or Exclude: or Include: are ok - must be on separate lines.
   The order doesn't matter as long as they are all in teh 1st XML comment.
 - Every run of ProjectLinker will re-Link the source CSPROJ
   into the space between the XML comment placeholders.
 - ALL code links inside the placeholders are refreshed every time. OK?

---------------------

## notes from the Original Fustercluck
left as an example of what happens when you think about something too much....
 
See the [Wiki](https://github.com/CADbloke/CodeLinker/wiki) for more detailed info.

There's a [page on the GUI app](https://github.com/CADbloke/CodeLinker/wiki/Using-the-GUI-App) and one for the [Command line app](https://github.com/CADbloke/CodeLinker/wiki/Command-Line).

More instructions coming soon. Your [feedback](https://github.com/CADbloke/CodeLinker/issues) would be more than welcome.

There is a log file called *CodeLinkerLog.txt* saved in the same folder as the executable. If you use this as a *pre/post-build process* The Visual Studio output window will have some summary information but the details will be in the log file. Good luck finding anything in the output window anyway 

[![Join the chat at https://gitter.im/CADbloke/CodeLinker](https://badges.gitter.im/CADbloke/CodeLinker.svg)](https://gitter.im/CADbloke/CodeLinker?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)