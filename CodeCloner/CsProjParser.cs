using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CodeCloner
{
  /// <summary> Parses the Source CSPROJ and gets the string blob to paste into the Destination CSPROJ </summary>
  internal class CsProjParser
  {
    private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

    internal XDocument projectDocument { get; set; }


    internal CsProjParser(string csProjFilePath)
    {
      try { projectDocument = XDocument.Load(csProjFilePath); }
      catch (Exception e)
      {
        Log.WriteLine(e.ToString());
        throw;
      }
    }


    static List<string> ItemElementsToSkip = new List<string> {"reference","projectreference","bootstrapperpackage"};
    internal string Parse()
    {
      StringBuilder newXml = new StringBuilder();
      IEnumerable<XElement> sourceItemGroups = projectDocument.Descendants(MSBuildNamespace + "ItemGroup");


      foreach (XElement sourceItemGroup in sourceItemGroups)
      {
        foreach (XElement xElement in sourceItemGroup.Elements())
        {
          if (ItemElementsToSkip.Contains(xElement.Name.LocalName.ToLower())) { continue; }



          XmlReader reader = xElement.CreateReader(); // http://stackoverflow.com/a/659264/492
          reader.MoveToContent();
          string xml =  reader.ReadInnerXml();
        }
      }

      return newXml.ToString();
    }
  }
}
