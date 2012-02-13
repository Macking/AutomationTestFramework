using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
    public struct XMLParameterNode
    {
        public bool IsEmptyInnerText;
        public string ParameterName;
        public string ParameterValue;
    }

    /// <summary>
    /// Present a xml parameter
    /// </summary>
    [ClassInterfaceAttribute(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class XMLParameter
    {
        #region Private Member

        private List<XMLParameterNode> mParas = new List<XMLParameterNode>();
        private string mName;

        #endregion

        #region Constructors

        public XMLParameter(string name)
        {
            this.mName = name;
        }

        public XMLParameter(List<XMLParameterNode> Paras, string name)
        {
            this.mParas = Paras;
        }

        public XMLParameter()
        {
        }

        #endregion

        #region Properties

        public int Length
        {
            get { return this.mParas.Count; }
        }

        public string Name
        {
            get { return this.mName; }
            set { this.mName = value; }
        }

        public List<XMLParameterNode> Parameters
        {
            get { return this.mParas; }
            set { this.mParas = value; }
        }

        #endregion

        #region Public Methods

        public void AddParameterNode(XMLParameterNode NewPara)
        {
            this.mParas.Add(NewPara);
        }

        public void AddParameter(string ParaName, string Value)
        {
            XMLParameterNode node = new XMLParameterNode();
            node.ParameterName = ParaName;
            node.ParameterValue = Value;
            node.IsEmptyInnerText = true;
            this.mParas.Add(node);
        }

        public void AddParameter(string ParaName, string Value, bool isEmptyInnertext)
        {
            XMLParameterNode node = new XMLParameterNode();
            node.ParameterName = ParaName;
            node.ParameterValue = Value;
            node.IsEmptyInnerText = isEmptyInnertext;
            this.mParas.Add(node);
        }

        public string GetParameterName(int index)
        {
            string result = string.Empty;

            for (int i = 0; i < this.mParas.Count; i++)
            {
                if (i == index)
                {
                    result = this.mParas[i].ParameterName;
                    break;
                }
            }

            return result;
        }

        public string GetParameterValue(int index)
        {
            string result = string.Empty;

            for (int i = 0; i < this.mParas.Count; i++)
            {
                if (i == index)
                {
                    result = this.mParas[i].ParameterValue;
                    break;
                }
            }

            return result;
        }

        public string GetParameterValue(string ParaName)
        {
            string result = string.Empty;

            for (int i = 0; i < this.mParas.Count; i++)
            {
                if (ParaName.Trim().ToUpper() == this.mParas[i].ParameterName.Trim().ToUpper())
                {
                    result = this.mParas[i].ParameterValue;
                    break;
                }
            }

            return result;
        }

        public string GetParameterValueByIndex(int index)
        {
            string result = string.Empty;

            for (int i = 0; i < this.mParas.Count; i++)
            {
                if (i == index)
                {
                    result = this.mParas[i].ParameterValue;
                    break;
                }
            }

            return result;
        }

        public string GetParameterValueByName(string ParaName)
        {
            string result = string.Empty;

            for (int i = 0; i < this.mParas.Count; i++)
            {
                if (ParaName.Trim().ToUpper() == this.mParas[i].ParameterName.Trim().ToUpper())
                {
                    result = this.mParas[i].ParameterValue;
                    break;
                }
            }

            return result;
        }

        public void SetParameter(string ParaName, string Value)
        {
            for (int i = 0; i < this.mParas.Count; i++)
            {
                if (ParaName.Trim().ToUpper() == this.mParas[i].ParameterName.Trim().ToUpper())
                {
                    XMLParameterNode node = new XMLParameterNode();
                    node.ParameterValue = Value;
                    node.ParameterName = ParaName;
                    this.mParas.Remove(this.mParas[i]);
                    this.mParas.Insert(i, node);
                    break;
                }
            }
        }

        public string GenerateXML()
        {
            /*if (this.Paras.Count == 0)
                return "";*/
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("trophy");
            root.SetAttribute("type", "request");
            root.SetAttribute("version", "1.0");

            //XmlElement root = doc.CreateElement("CDATA");
            XmlElement paraName = doc.CreateElement(this.mName);
            root.AppendChild(paraName);

            foreach (XMLParameterNode n in this.mParas)
            {
                XmlElement p = doc.CreateElement("parameter");
                p.SetAttribute("key", n.ParameterName);

                if (n.IsEmptyInnerText)
                    p.SetAttribute("value", n.ParameterValue);
                else
                    p.InnerText = n.ParameterValue;
                
                paraName.AppendChild(p);
            }
            doc.AppendChild(root);

            return doc.OuterXml;
        }

        #endregion

    }
}
