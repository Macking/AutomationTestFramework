using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Schema;
using PAS.AutoTest.TestUtility;

namespace PAS.AutoTest.PasATCoreV2
{
    public abstract class ServiceBase
    {
        #region Protected Members

        protected string mWSDL = string.Empty;
        protected string MainTypeName = string.Empty;
        protected Type MainType;
        protected string lastReturnXML = string.Empty;
        protected XMLValidateResult lastReturnXMLValidateResult;
        protected double mResponseTime = 0;
        protected object mServiceInstance = null;

        #endregion

        #region Properties

        public XMLValidateResult LastReturnXMLValidateResult
        {
            get { return lastReturnXMLValidateResult; }
            set { lastReturnXMLValidateResult = value; }
        }

        public string LastReturnXML
        {
            get { return lastReturnXML; }
            set { lastReturnXML = value; }
        }

        public string WSDL
        {
            get
            {
                return GetWSAssembly.GetWsdl(this.MainTypeName);
            }
        }

        #endregion

        #region Public Methods
        public object InvokeMethod(Type type, string MethodName, object[] Paras)
        {
            object t = Activator.CreateInstance(type);
            object result = type.GetMethod(MethodName).Invoke(t, Paras);

            LastReturnXML = result.ToString();

            return result;
        }

        public object InvokeMethod(string MethodName, object[] Paras)
        {
            object result = this.MainType.GetMethod(MethodName).Invoke(this.mServiceInstance, Paras);

            LastReturnXML = result.ToString();

            return result;
        }

        public void InitialService(string MainTypeName)
        {
            this.MainTypeName = MainTypeName;
            this.MainType = GetWSAssembly.GetWebServiceAssembly(this.WSDL, MainTypeName).GetTypes()[0];
            this.mServiceInstance = Activator.CreateInstance(this.MainType);
        }
        #endregion


        #region Protected Methods

        protected string GenarateRequestXML(object request)
        {
            string xmlString = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(request.GetType());

            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, request);
                xmlString = Encoding.UTF8.GetString(ms.ToArray());
            }

            // Add the xmlns:tns="http://www.carestreamhealth.com/CSI/CSDM/1/Schema"
            string nameSpace = "xmlns:tns=\"http://www.carestreamhealth.com/CSI/CSDM/1/Schema\"";
            xmlString = xmlString.Replace("xmlns:xsi", nameSpace + " xmlns:xsi");

            return xmlString;
        }

        protected Object DeserializeXMLToClass(object returnContent, Type type)
        {
            if (LastReturnXMLValidateResult.isValid == true) //If the XML is validated, then try to parse
            {
                string content = null;

                if (returnContent.GetType() == typeof(System.String))
                {
                    content = returnContent.ToString();
                }

                XmlSerializer serializer = new XmlSerializer(type);
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

                return serializer.Deserialize(stream);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
