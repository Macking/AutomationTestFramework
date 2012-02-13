using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
    public class ApplicationService:PASBase 
    {
        public ApplicationService()
        {
            this.InitialService("Application");
        }

        public XMLResult createApplication(string application, XMLParameter config)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createApplication", new object[] { application, config.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult setConfiguration(string appliInstanceID, XMLParameter config)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setConfiguration", new object[] { appliInstanceID, config.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult closeApplication(string appliInstanceID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("closeApplication", new object[] { appliInstanceID }));
            return this.lastResult;
        }

        public XMLResult getConfiguration(string appliInstanceID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getConfiguration", new object[] { appliInstanceID }));
            return this.lastResult;
        }

        public XMLResult listApplications()
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listApplications", new object[] { }));
            return this.lastResult;
        }

        public XMLResult minimizeApplication(string appliInstanceID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("minimizeApplication", new object[] { appliInstanceID }));
            return this.lastResult;
        }

        public XMLResult openObjects(XMLParameter objectInternalIDs)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("openObjects", new object[] { objectInternalIDs.GenerateXML ()}));
            return this.lastResult;
        }

        public XMLResult restoreApplication(string appliInstanceID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("restoreApplication", new object[] { appliInstanceID }));
            return this.lastResult;
        }

        public XMLResult registerApplication(string application, string host, int port, bool portSpecified)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("registerApplication", new object[] { application, host, port, portSpecified }));
            return this.lastResult;
        }

        public XMLResult setState(string application, string state)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setState", new object[] { application, state }));
            return this.lastResult;
        }

        public XMLResult getClientPASVersion()
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getClientPASVersion", new object[] { }));
            return this.lastResult;
        }

        public XMLResult getServerPASVersion()
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getServerPASVersion", new object[] { }));
            return this.lastResult;
        }

        public XMLResult sendGenericNotification(string messageType, string message)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("sendGenericNotification", new object[] { messageType, message}));
          return this.lastResult;
        }
    }
}
