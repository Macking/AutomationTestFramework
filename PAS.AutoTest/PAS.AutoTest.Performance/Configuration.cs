using System;
using System.Collections.Generic;
using System.Text;
using System.Xml; 

namespace PAS.AutoTest.Performance
{
    public class Configuration
    {
        /// <summary>
        /// test case dll path.
        /// </summary>
        public static string TestCasePath
        {
            get
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("PerfConfig.xml");

                    XmlNode node = doc.SelectSingleNode("Config/TestCasePath");

                    return node.InnerText;
                }
                catch (Exception)
                {
                    throw;
                }

            }
            set
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("PerfConfig.xml");
                    XmlNode node = doc.SelectSingleNode("Config/TestCasePath");
                    node.InnerText = value;
                    doc.Save("PerfConfig.xml");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
