using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.ProcessMemory
{
  class XMLConfig
  {
    public static List<string> GetSettings(string WMIClassName)
    {
      string xmlFilePath = System.IO.Directory.GetCurrentDirectory() + "\\MemorySettings.xml";
      List<string> alPropertyNames = new List<string>();
      System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
      xmldoc.Load(xmlFilePath);
      System.Xml.XmlNode properties = xmldoc.SelectSingleNode("//" + WMIClassName);

      for (int i = 0; i < properties.ChildNodes.Count; i++)
        alPropertyNames.Add(properties.ChildNodes[i].InnerText);
      return alPropertyNames;
    }
  }
}
