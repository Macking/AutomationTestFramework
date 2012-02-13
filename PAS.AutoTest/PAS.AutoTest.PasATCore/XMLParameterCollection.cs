using System;
using System.Collections.Generic;
using System.Text;
using System.Xml; 

namespace PAS.AutoTest.PasATCore
{
    public class XMLParameterCollection:List<XMLParameter>
    {
        #region Public Methods

        /// <summary>
        /// get a XMLParameter by its name.
        /// </summary>
        /// <param name="name">XMLParameter name.</param>
        /// <returns>XMLParameter</returns>
        public XMLParameter GetParameterByName(string name)
        {
            foreach (XMLParameter p in this)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }

            return null;
        }

        /// <summary>
        /// Generate string xml.
        /// </summary>
        /// <returns></returns>
        public string GenerateXML()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("trophy");
            root.SetAttribute("type", "request");
            root.SetAttribute("version", "1.0");

            //XmlElement root = doc.CreateElement("CDATA");
            foreach (XMLParameter pa in this)
            {
                XmlElement paraName = doc.CreateElement(pa.Name );
                root.AppendChild(paraName);

                foreach (XMLParameterNode n in pa.Parameters)
                {
                    XmlElement p = doc.CreateElement("parameter");
                    p.SetAttribute("key", n.ParameterName);

                    if (n.IsEmptyInnerText)
                        p.InnerText = n.ParameterValue;
                    else
                        p.SetAttribute("value", n.ParameterValue);

                    paraName.AppendChild(p);
                }

                doc.AppendChild(root);
            }

            return doc.OuterXml;
        }

        #endregion
    }
}
