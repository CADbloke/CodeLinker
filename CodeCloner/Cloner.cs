using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCloner
{
  internal class Cloner
  {
    /// <summary> Absolute file path to the Source CSPROJ. </summary>
    internal string SourceCsProject { get; set; }  
     
    /// <summary> Absolute file path to the Destination CSPROJ. </summary>
    internal string DestCsProject { get; set; }

    /// <summary> True if the Cloner gets the source from the XML comment placeholder in the Destination CSPROJ. </summary>
    internal bool AutoClone { get; set; } 

    /// <summary> String containing all the <c>ItemGroup</c>s to be inserted into the Destination CSPROJ </summary>
    internal string ItemGroups { get; set; }

    


    /* 
    note: Item Types:  
    MATCH
    Compile|None|Folder|EmbeddedResource|Resource|Res|AppDesigner|Page|Content|WCFMetadataStorage|Folder
    
    KEEP
    Condition
    
    EXCLUDE
    Reference|ProjectReference|BootstrapperPackage

    Log anything that is not one of these because it's a bug - I missed it

    cope with conditional inclusions
    Cope with absolute paths, including $(EnvironmentVariables)
    */


  }
}
