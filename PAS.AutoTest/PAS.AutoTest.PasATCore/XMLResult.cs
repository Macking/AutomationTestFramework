using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
    [ClassInterfaceAttribute(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class XMLResult
    {
        #region Private Member

        private string mResultContent;
        private XmlDocument mXmlDoc=new XmlDocument ();

        #endregion

        #region Construcotors

        public XMLResult()
        { }

        public XMLResult(string Content)
        {
            this.mResultContent = Content;
            this.mXmlDoc.Load(new MemoryStream(Encoding.ASCII.GetBytes(this.mResultContent)));
        }

        public XMLResult(object Content)
        {
            //this.resultContent = Content.GetType().GetField("return").GetValue(Content).ToString ();
            //this.xmlDoc.Load(new MemoryStream(Encoding.ASCII.GetBytes (this.resultContent )) );

            if (Content.GetType() == typeof(System.String))
            {
                this.mResultContent = Content.ToString();
                this.mXmlDoc.LoadXml(Content.ToString());
            }
            else
            {
                this.mResultContent = Content.GetType().GetField("return").GetValue(Content).ToString();
                this.mXmlDoc.Load(new MemoryStream(Encoding.ASCII.GetBytes(this.mResultContent)));
            }

        }

        #endregion

        #region Properties

        public string ResultContent
        {
            get { return this.mResultContent; }
            set { mResultContent = value; }
        }

        public int Code
        {
            get
            {
                XmlNode node = this.mXmlDoc.SelectSingleNode("//status");
                return Convert.ToInt32(node.Attributes["code"].Value);
            }
        }

        /// <summary>
        /// Determine whether the function run successfully or has exception throw out.
        /// </summary>
        public bool IsErrorOccured
        {
            get
            {
                return this.Message.ToUpper().Trim() == "OK" ? false : true;
            }
        }

        /// <summary>
        /// Presents the exception message.
        /// </summary>
        public string Message
        {
            get
            {
                XmlNode node = this.mXmlDoc.SelectSingleNode("//status");
                return node.Attributes["message"].Value;
            }
        }

        public string SingleResult
        {
            get
            {
                if (this.IsErrorOccured)
                    return Message;
                else
                    return this.ArrayResult.GetParameterValue(0);
            }
        }

        public XMLParameter ArrayResult
        {
            get
            {
                try
                {
                    XMLParameter x = new XMLParameter();
                    XmlNode node = this.mXmlDoc.SelectSingleNode("//status").NextSibling;
                    x.Name = node.Name;

                    foreach (XmlNode n in node.ChildNodes)
                    {
                        if (n.Attributes.Count == 1)
                            x.AddParameter(n.Attributes["key"].Value, n.InnerText, false);
                        else
                            x.AddParameter(n.Attributes["key"].Value, n.Attributes["value"].Value);
                    }

                    return x;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public XMLParameter DicomArrayResult
        {
            get
            {
                try
                {
                    XMLParameter x = new XMLParameter();
                    XmlNode node = this.mXmlDoc.SelectSingleNode("//status").NextSibling;
                    x.Name = node.Name;

                    foreach (XmlNode n in node.ChildNodes)
                    {
                        if (n.Attributes.Count == 1 && n.ChildNodes.Count == 0)
                            x.AddParameter(n.Attributes["key"].Value, n.InnerText, false);
                        else if (n.ChildNodes.Count > 0 && n.Name == "dicom_info")
                        {
                            XmlNode subnode = node.SelectSingleNode("//dicom_info");
                            XMLParameterNode xpn = new XMLParameterNode();
                            xpn.ParameterName = n.Name;
                            xpn.ParameterValue = subnode.InnerXml;
                            x.AddParameterNode(xpn);
                        }
                        else
                            x.AddParameter(n.Attributes["key"].Value, n.Attributes["value"].Value);
                    }

                    return x;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<XMLParameter> MultiResults
        {
            get
            {
                try
                {
                    List<XMLParameter> result = new List<XMLParameter>();

                    XmlNodeList nodes = this.mXmlDoc.SelectSingleNode("trophy").ChildNodes;

                    foreach (XmlNode node in nodes)
                    {
                        if (node.Name == "status")
                            continue;

                        XMLParameter p = new XMLParameter(node.Name);

                        foreach (XmlNode n in node.ChildNodes)
                        {
                            if (n.Attributes.Count == 1)
                                p.AddParameter(n.Attributes[0].Value, n.InnerText, false);
                            else
                                p.AddParameter(n.Attributes[0].Value, n.Attributes[1].Value);
                        }

                        result.Add(p);
                    }

                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// set the content of a xmlresult
        /// </summary>
        /// <param name="Content">xml format web service result</param>
        public void SetContent(string Content)
        {
            this.mResultContent = Content;
            this.mXmlDoc.Load(new MemoryStream(Encoding.ASCII.GetBytes(this.mResultContent)));
        }

        #endregion

    }
}