using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace CodeCloner
{
  /// <summary> A create struct project parser. </summary>
  internal class DestinationCsProjParser
  {
    private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

    private string    destCsProjPath;
    private XDocument csProjXml;
    private XComment  startPlaceHolder;
    private XComment  endPlaceHolder;
    
    /// <summary> Source <c>CSPROJ</c>s defined in the Destination <c>CSPROJ</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceCsProjList { get; }

    /// <summary> Constructor. </summary>
    /// <param name="destCsProjAbsolutePath"> Absolute path of destination <c>CSPROJ</c>. </param>
    public DestinationCsProjParser(string destCsProjAbsolutePath)
    {
      destCsProjPath = destCsProjAbsolutePath;
      if (!File.Exists(destCsProjAbsolutePath)) Program.Crash("ERROR: " + destCsProjAbsolutePath + "  does not exist.");
      if (!destCsProjAbsolutePath.ToLower().EndsWith(".csproj"))
        Program.Crash("ERROR: " + destCsProjAbsolutePath + "  is not a CSPROJ.");

      try { using (StreamReader reader = File.OpenText(destCsProjAbsolutePath)) { csProjXml = XDocument.Load(reader); } }
      catch (Exception e) { Program.Crash(e); }

      startPlaceHolder = FindComment("CodeCloner");
      endPlaceHolder   = FindComment("EndCodeCloner");

      SourceCsProjList.Clear();

      foreach (string line in startPlaceHolder.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList())
      {
        if (!line.ToLower().Trim().StartsWith("source:")) continue;
        SourceCsProjList.Add(line.Replace("source:", "").Trim());
      }
    }


    internal void RemoveOldDestCsProjClonedCode()
    {
      if (startPlaceHolder != null && endPlaceHolder != null && startPlaceHolder.IsBefore(endPlaceHolder))
      {
        XNode startNode = startPlaceHolder;
        while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }
      }
      else Program.Crash("Error: cannot remove old Cloned Code from " + destCsProjPath);
    }
   

    private XComment FindComment(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in csProjXml.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders  = comments.Where(c => c.Value.ToLower().StartsWith(commentStartsWith)).ToList();

      if (placeholders.Count != 1)
        Program.Crash("ERROR: " +destCsProjPath+ "has " +placeholders.Count+" XML comments with" + commentStartsWith);

      return placeholders.First();
    }

  }
}
