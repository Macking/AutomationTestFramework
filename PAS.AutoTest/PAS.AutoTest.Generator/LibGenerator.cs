using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Diagnostics;
using System.Xml;

namespace PAS.AutoTest.Generator
{
    public static partial class PasATCoreV2Generator
    {
        private static void GeneratePasATCoreV2Lib()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe"; //give a default value

            string projectFullPath = @"..\..\PAS.AutoTest\PAS.AutoTest.PasATCoreV2\PAS.AutoTest.PasATCoreV2.csproj";

            XmlDocument doc = new XmlDocument();
            doc.Load(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + @"\GeneratorConfig.xml");

            XmlNodeList infoList = doc.SelectNodes("Config/Configuration/LibGenerator");

            foreach (XmlNode info in infoList)
            {
                if (info.Attributes["Name"].Value.Trim().ToLower() == "toolfullpath")
                {
                    proc.StartInfo.FileName = info.InnerText;
                }
                else if (info.Attributes["Name"].Value.Trim().ToLower() == "project")
                {
                    projectFullPath = info.InnerText;
                }
            }

            //TODO: It's better to add the generated class files into the project danamically, while, manually add them as linked file is also a choice

            proc.StartInfo.Arguments = projectFullPath + " /t:Rebuild" + " /p:Configuration=Debug";
            proc.Start();
            proc.WaitForExit(60000);
        }

    }
}
