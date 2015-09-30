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
  internal class CsProjParser
  {
    private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
    private XDocument csProjXml;
    private XComment startPlaceHolder;
    private XComment endPlaceHolder;
    private string csProjPath;

    /// <summary> Source <c>CSPROJ</c>s defined in the Destination <c>CSPROJ</c> placeholder. 
    ///           Can be zero, can be lots.</summary>
    internal List<string> SourceCsProjList { get; }

    public CsProjParser(string csProjAbsolutePath)
    {
      csProjPath = csProjAbsolutePath;
      if (!File.Exists(csProjAbsolutePath)) Program.Crash("ERROR: " + csProjAbsolutePath + "  does not exist.");
      if (!csProjAbsolutePath.ToLower().EndsWith(".csproj"))
        Program.Crash("ERROR: " + csProjAbsolutePath + "  is not a CSPROJ.");

      try { using (StreamReader reader = File.OpenText(csProjAbsolutePath)) { csProjXml = XDocument.Load(reader); } }
      catch (Exception e) { Program.Crash(e); }

      startPlaceHolder = FindComment("CodeCloner");
      endPlaceHolder   = FindComment("EndCodeCloner");

      SourceCsProjList.Clear();

      foreach (string line in startPlaceHolder.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList())
      {
        if (!line.ToLower().StartsWith("source:")) continue;
        SourceCsProjList.Add(line.Replace("source:", "").Trim());
      }
    }

    internal void RemoveOldDestCsProjClonedCode()
    {
      XNode startNode = startPlaceHolder;
      while (startNode.NextNode != endPlaceHolder) { startNode.NextNode.Remove(); }
    }
   

    private XComment FindComment(string commentStartsWith)
    {
      IEnumerable<XComment> comments = from node in csProjXml.Elements().DescendantNodesAndSelf()
                                       where node.NodeType == XmlNodeType.Comment
                                       select node as XComment;

      List<XComment> placeholders  = comments.Where(c => c.Value.ToLower().StartsWith(commentStartsWith)).ToList();

      if (placeholders.Count != 1)
        Program.Crash("ERROR: " +csProjPath+ "has " +placeholders.Count+" with" + commentStartsWith);

      return placeholders.First();
    }

  }
}
