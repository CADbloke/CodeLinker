# Code Linker 
## Reuse source code between CSPROJ or VBPROJ files

Code Linker creates links in the destination `.csproj` project file to the code files in the source `.csproj`, automating the process of [adding existing files as-a-link](https://msdn.microsoft.com/en-us/library/windows/apps/jj714082(v=vs.105).aspx) into the destination project. The files are added as relative paths (back to the original) if they are already relative links. If they were originally absolute paths then that is preserved.  If they already have a link I'll try not to break it.

The original version 1 was a fustercluck example of YAGNI. It is still in the repo to serve as a warning to others. CodeLinker2 is the one you want. Here is how that works ....

# Code Linker v2
## because v1 confused even me and I wrote it
Links Source code in CSPROJ and/or VBPROJ files to outside projects
Does NOT Convert C# / VB between CSPROJ and VBPROJ files.
Don't cross the streams.
Anywhere this mentions `CSPROJ` it also means `VBPROJ`.

### Usages...
- `CodeLinker.exe /?`  this help file
- `CodeLinker.exe` `[Destination.csproj]`
- `Destination.csproj`   Path to the existing Destination project.

- Wrap paths in your call to `CodeLinker.exe` with spaces in "double quotes".
- Paths can (probably should!) be `..\`relative.


### CodeLinker puts your linked code between these 2 XML comment placeholders in the Destination CSPROJ file ...

```xml
<!-- CodeLinker
Source: PathTo\NameOfProject.csproj     <== this is NOT optional
Exclude: PathTo\FileToBeExcluded.cs     <== optional - a partial match will exclude it. Works like wildcard in DOS
Include: PathTo\FileToBeIncluded.cs     <== optional but if used it ONLY includes matches. Works like wildcard in DOS
DestinationProjectFolderPrefix: Folder\For\Linked\Codez   <== optional folder to nest linked code in

Source: PathTo\NameOfOtherProject.csproj <== this is NOT optional ... unless you don't want more projects
Exclude: PathTo\FileToBeExcluded.cs     <== optional - a partial match will exclude it. Works like wildcard in DOS
Include: PathTo\FileToBeIncluded.cs     <== optional but if used it ONLY includes matches. Works like wildcard in DOS
DestinationProjectFolderPrefix: Folder\For\Linked\Codez   <== optional folder to nest linked code in
-->

<!-- EndCodeLinker -->
```

- If your destination project doesn't have the placeholders then it soon will, I will add them for you on the first run.
- You can wrap the `Source:` paths in quotes if you like, it doesn't matter.
- You can have multiple source projects filtered with multiple Exclusions &/or Inclusions. 
- You may specify multiple Exclude: &/or Include: items. They all apply ONLY to the Source Project immediately preceding it.
- For multiple `Source:` projects you need to re-specify any `Exclude:` or `Include:` or `DestinationProjectFolderPrefix:`, these are reset when you start a new `Source:` project group.

You could also consider manually adding something like this to your target `csproj` file ...
```xml
<PropertyGroup>
    <PostBuildEvent>"Path \\ to \\\\  CodeLinker.exe" $(ProjectPath)</PostBuildEvent>
    <RunPostBuildEvent>Always</RunPostBuildEvent>
</PropertyGroup>
```
...so it runs on every build. Why? Well, it you change the source project and add / remove / move things in the source then your target build may break until the code is re-linked and the source changes propagated to the target. If you already have a `<PostBuildEvent>` then add it to that because each new one supersedes any existing one. I use a batch file, as per below.

You can put CodeLinker in the same places as your target projects or in one place for all projects. Its location will affect the log file more than anything else, the log is always in the same folder as `CodeLinker.exe`. 

If CodeLinker runs and nothing changes it won't re-save your Target `csproj` file. If it does change the project then Visual Studio will complain loudly and want you to reload it.  This is normal, **don't panic**

### To populate the information, edit your project file in a text editor. Have no fear, Git is your friend.

- You may specify multiple Source: projects. No wildcards. Remeber to group the Include, Exclude etc together with the project
- just the File name if it's in the same folder, or relative or absolute path.
- You may specify multiple `Exclude:` &/or `Include:` items. They all apply ONLY to the immediately preceding source project
- In/Exclusions are a simple VB LIKE String wildcard match, same as file system wildcard matches so * and ? work
- Protip: `Folder\OtherFolder\*` is a valid wildcard
- `Exclusions` override **all** `Inclusions`.
- If you specify no `Inclusions` then everything is an `Inclusion`.
- If you do specify any `Inclusions` then **ONLY** they are `Included`.
- Multiple `Source:` or `Exclude:` or `Include:` are ok - they must be on separate lines.
- `Source:` project order matters, CodeLinker will not add a link to a file path that already exists in the Destination project.
- `In/Exclude:` order doesn't matter.
- the `Source:` line needs to be before any  any `Exclude:` or `Include:` or `DestinationProjectFolderPrefix:` lines for that project.

### The folder structures of the Source(s) are preserved. If the source file is nested, the link will be nested
- soooo, if you nest them inside a folder in the source project they will all be neatly inside a nested folder in the destination project
- Every run of ProjectLinker will re-Link the source project(s) into their space between the XML comment placeholders.
- so ALL code links inside the placeholders are refreshed every time. OK?
- you can use `DestinationProjectFolderPrefix:` to nest your linked project under a parent folder

### to automate this linking process ...
add the CodeLinker.exe to your project can call it in our post-build commands with something like...
```batch
CodeLinker.exe \"Path\\to\\Your.csproj\"
````

There is a log file called *CodeLinkerLog.txt* saved in the same folder as the executable. If you use this as a *pre/post-build process* The Visual Studio output window will have summary information, the details will be in the log file. Good luck finding anything in the VS output window anyway 

---------------------

## notes from the Original Fustercluck
left as an example of what happens when you think about something too much....
 
See the [Wiki](https://github.com/CADbloke/CodeLinker/wiki) for more detailed info.

... *actually, don't - it's confusing.*

There's a [page on the GUI app](https://github.com/CADbloke/CodeLinker/wiki/Using-the-GUI-App) and one for the [Command line app](https://github.com/CADbloke/CodeLinker/wiki/Command-Line). Avoid those pages, they are for the old version.

More instructions coming ~~soon~~ (*ok, who are we kidding here, this is OSS*). Your [feedback](https://github.com/CADbloke/CodeLinker/issues) would be more than welcome.
