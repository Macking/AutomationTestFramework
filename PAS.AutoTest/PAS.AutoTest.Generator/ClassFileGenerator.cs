using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;

namespace PAS.AutoTest.Generator
{
    public static partial class PasATCoreV2Generator
    {
        private static void GenarateClassFile() // Maybe can return the generated file list
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "xsd.exe"; 
            string codenamespace = "PAS.AutoTest.PasATCoreV2";
            string outputDirectory = @"GeneratedClassFiles";

            List<string> xsdFileList = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + @"\GeneratorConfig.xml");

            XmlNodeList infoList = doc.SelectNodes("Config/Configuration/ClassGenerator");

            foreach (XmlNode info in infoList)
            {
                if (info.Attributes["Name"].Value.Trim().ToLower() == "toolfullpath")
                {
                    proc.StartInfo.FileName = info.InnerText;
                }
                else if (info.Attributes["Name"].Value.Trim().ToLower() == "xsdfile")
                {
                    xsdFileList.Add(info.InnerText);
                }
            }

            foreach (string xsdFile in xsdFileList)
            {
                proc.StartInfo.Arguments = "/c " + "\"" + xsdFile + "\"" + " /out:" + "\"" + outputDirectory + "\"" + " /namespace:" + codenamespace;
                proc.Start();
                proc.WaitForExit(10000);
            }
        }
    }
}
