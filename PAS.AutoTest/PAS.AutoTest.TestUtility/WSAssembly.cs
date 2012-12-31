using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using Microsoft.CSharp;
using System.Xml;

namespace PAS.AutoTest.TestUtility
{
    public class GetWSAssembly
    {
        public static List<WSDL> mServicePaths = new List<WSDL>();


        /// <summary>
        /// get an Assembly according to wsdl path.
        /// </summary>
        /// <param name="wsdl">wsdl path</param>
        /// <param name="nameSpace">namespace</param>
        /// <returns>return Assembly</returns>
        public static Assembly GetWebServiceAssembly(string wsdl, string nameSpace)
        {
            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                System.IO.Stream webStream = webClient.OpenRead(wsdl);

                ServiceDescription serviceDescription = ServiceDescription.Read(webStream);
                ServiceDescriptionImporter serviceDescroptImporter = new ServiceDescriptionImporter();

                serviceDescroptImporter.AddServiceDescription(serviceDescription, "", "");
                System.CodeDom.CodeNamespace codeNameSpace = new System.CodeDom.CodeNamespace(nameSpace);
                System.CodeDom.CodeCompileUnit codeCompileUnit = new System.CodeDom.CodeCompileUnit();
                codeCompileUnit.Namespaces.Add(codeNameSpace);
                serviceDescroptImporter.Import(codeNameSpace, codeCompileUnit);

                System.CodeDom.Compiler.CodeDomProvider codeDom = new Microsoft.CSharp.CSharpCodeProvider();
                System.CodeDom.Compiler.CompilerParameters codeParameters = new System.CodeDom.Compiler.CompilerParameters();
                codeParameters.GenerateExecutable = false;
                codeParameters.GenerateInMemory = true;

                codeParameters.ReferencedAssemblies.Add("System.dll");
                codeParameters.ReferencedAssemblies.Add("System.XML.dll");
                codeParameters.ReferencedAssemblies.Add("System.Web.Services.dll");
                codeParameters.ReferencedAssemblies.Add("System.Data.dll");

                System.CodeDom.Compiler.CompilerResults compilerResults = codeDom.CompileAssemblyFromDom(codeParameters, codeCompileUnit);

                return compilerResults.CompiledAssembly;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetWsdl(string name)
        {
            foreach (WSDL w in GetWSAssembly.mServicePaths)
            {
                if (w.Name == name)
                    return w.Path;
            }

            //if not exist in buffer.

            XmlDocument doc = new XmlDocument();
            doc.Load(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + @"\Config.xml");

            XmlNodeList ws = doc.SelectNodes("Config/ServiceWsdl/Wsdl");

            foreach (XmlNode w in ws)
            {
                if (w.Attributes["Name"].Value.Trim().ToUpper() == name.Trim().ToUpper())
                {
                    WSDL wsdl = new WSDL();
                    wsdl.Name = name;
                    wsdl.Path = w.InnerText.Trim();
                    GetWSAssembly.mServicePaths.Add(wsdl);

                    return wsdl.Path;
                }
            }

            //if not find in config.xml
            return string.Empty;
        }

    }

    public class WSDL
    {
        public string Path = string.Empty;
        public string Name = string.Empty;
    }
}
