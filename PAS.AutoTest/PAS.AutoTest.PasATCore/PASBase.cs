using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace PAS.AutoTest.PasATCore
{
    public abstract class PASBase
    {
        #region Protected Members

        protected string mWSDL = string.Empty;
        protected string MainTypeName = string.Empty;
        protected Type MainType;
        protected XMLResult lastResult = new XMLResult();
        protected double mResponseTime = 0;
        protected object mServiceInstance = null;

        //timer
        protected ExactTimer mTimer = new ExactTimer();

        #endregion

        #region Properties

        public XMLResult LastResult
        {
            get { return lastResult; }
            set { lastResult = value; }
        }

        public double ResponseTime
        {
            get { return this.mResponseTime; }
        }

        public string WSDL
        {
            //get
            //{
            //    if (this.mWSDL == string.Empty)
            //    {
            //        XmlDocument doc = new XmlDocument();
            //        doc.Load("config.xml");

            //        XmlNodeList nodes = doc.SelectNodes("Config/WSDL");

            //        foreach (XmlNode node in nodes)
            //        {
            //            if (node.Attributes["Name"].Value.ToUpper().Trim() == this.GetType().Name.ToUpper())
            //            {
            //                this.mWSDL = node.InnerText;
            //                break;
            //            }
            //        }
            //    }

            //    return this.mWSDL;
            //}

            get
            {
                return PAS.AutoTest.PasATCore.GetWSAssembly.GetWsdl(this.MainTypeName);
            }
        }

        #endregion

        #region Public Methods

        public object InvokeMethod(Type type, string MethodName, object[] Paras)
        {
            object t = Activator.CreateInstance(type);
            mTimer.Start();
            object result= type.GetMethod(MethodName).Invoke(t, Paras);
            mTimer.Start();

            this.mResponseTime = mTimer.Duration;

            return result;
        }

        public object InvokeMethod(string MethodName, object[] Paras)
        {
            //object t = Activator.CreateInstance(this.MainType);
            mTimer.Start();
            object result= this.MainType.GetMethod(MethodName).Invoke(this.mServiceInstance , Paras);
            mTimer.Stop();

            this.mResponseTime = mTimer.Duration;

            return result;
        }

        public void InitialService(string MainTypeName)
        {
            this.MainTypeName = MainTypeName;
            this.MainType = GetWSAssembly.GetWebServiceAssembly(this.WSDL, MainTypeName).GetTypes()[0] ;
            this.mServiceInstance = Activator.CreateInstance(this.MainType);
        }

        #endregion
    }
}
