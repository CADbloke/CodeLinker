#Code Cloner 
##clones Source code between CSPROJ files

**Wait, no it doesn't.** Code Cloner creates links in the destination `.csproj` project file to the code files in the source `.csproj`, automating the process of adding an existing file as a link into the destination project. The files are added as relative paths if that's how they start out. If they were originally absolute paths then that is preserved.   
 

It is meant to be used as a pre-build process, add the command line(s) with args to your source project's `Build Events` tab in the `Project Properties` window, in the `Pre-build event command line:` text box. I add it to the source so it updates its targets whenever you build it. The targets don't have to be loaded in Visual Studio, actually life is easier if they aren't because VS will want to reload them if/when they change.   

There is a log file called *CodeClonerLog.txt* saved in the same folder as the executable. The Visual Stduo output window will have some summary information but the details will be in the log file. 

###Usages...

`CODECLONER /?`  
`CODECLONER [Folder] [/s]`  
`CODECLONER [Source.csproj] [Destination.csproj]`  
  
`/?` &nbsp;&nbsp;&nbsp;     Help!  
`Folder` &nbsp;&nbsp;&nbsp; Clones the source(s) into all CSPROJ files in the folder  
`/s` &nbsp;&nbsp;&nbsp;     Also iterates all subfolders. You just forgot this, right?

`Source.csproj`      &nbsp;&nbsp;&nbsp;  Path to the CSPROJ with the source to be cloned.  
`Destination.csproj` &nbsp;&nbsp;&nbsp;  Path to ... duh.

- Wrap paths with spaces in double quotes. 
- Paths can (probably should!) be relative.  
- Relative paths on the command line are relative to the executable
- Relative paths in the destination `.csproj` are relative the that `.csproj`  
- `Source.csproj` is optional on the command line , if you specify one CSPROJ file it is the destination, in which case it needs to have the source project in the placeholder.


####The Destination CSPROJ file needs this XML comment placeholder...


    <!-- CodeCloner
    Source: PathTo\\NameOfProject.csproj <== this is optional
    Exclude: PathTo\\FileToBeExcluded.cs <== this is optional
    -->
    
    <!-- EndCodeCloner -->


##iFAQa
**i**n**F**requently **A**sked **Q**uestion **a**nswers  

- You can add source to the destination project, just don't put it between the placeholders or it will be removed in the next clone.  
- You will probably have to build your solution twice if a project is changed by a code-clinging process, mostly because of parallel builds and all that, the project was probably building while it was also beign cloned. 
- You may specify multiple Source: projects. I don't know wildcards so you will have to list them all. Let me know how that goes because I haven't tried it as of writing this. It doesn't check for duplicated things.  
- If you don't specify a source in the placeholder it better be in the command line call.  
- `Exclude:` file or path. No wildcards here either. It is a simple `String.Contains()` filter. The log will tell you what was excluded.
- If you specify multiple `Exclude:` items then they must be on separate lines. I haven't tested this yet, either so all feedback is welcome 
- Every Code Clone will re-clone the source CSPROJ into the space between the XML comment placeholders.  
- ALL code inside these placeholders is refreshed every time. OK?
- I usually drop the executable in the solution root folder (it makes working out relative paths easier) and check it in to Git but ignore the *CodeClonerLog.txt* log file.
- The log file is continuous - delete it to reset it or you can mess with it in a text editor. 
- **Why?** I write a lot of AutoCAD plugins. They also need to run in IntelliCAD, NanoCAD, BricsCAD and a lot of versions of AutoCAD. The build settings, .NET framework version and project references are very different between the platforms but they (generally) share most of source code. **Write once, build many**. I tried other approaches but they were [horribly](http://www.theswamp.org/index.php?topic=41868.msg497509#msg497509 "HORRIBLY") [complicated](http://www.theswamp.org/index.php?topic=49039.msg541752#msg541752 "COMPLICATED") and Nuget broke most of them. Ok, I broke them with Nuget. This Solution (see what I did there?) means you can separate the build and code concerns. I also like being able to step through the debugger and see what the code is doing in a specific build. 
- Shared Projects don't have the source code right in front of you. ok, if they do then I got it wrong. Tell me how.
- This is a v1 - use it at your own risk. It may eat your kittens. Idon't think it will but I've been wrong before. See the Refund Ploicy below for more details.

####This is the bare-bones placeholder with no options specified...

    <!-- CodeCloner -->
    
    <!-- EndCodeCloner -->

####This is a ramdom command line example I just made up...

    codecloner \SourceProject.csproj \Destination1\Destination1.csproj
    codecloner \SourceProject.csproj \Destination2\Destination2.csproj
    codecloner \Destination3\Destination3.csproj
    
`Destination3.csproj` will need to have a source specified in the XML comment placeholder or it will crash out. The log will let you know. Maybe.

More Info & Source at [https://github.com/CADbloke/CodeCloner](https://github.com/CADbloke/CodeCloner "Code Cloner @ GitHub"). I'm also happy to hear of any issues &/or suggestions (Pull Requests are even better!). 

####Refund policy
Get your credit card pic retweeted at [https://twitter.com/needadebitcard](https://twitter.com/needadebitcard "DO NOT DO THIS !!") and I will refund you twice the price you paid for this. For further dtails see the [Extended Refund Policy](http://www.seobook.com/freetards "This is not you, right?")   
####Code Cloner by CADbloke