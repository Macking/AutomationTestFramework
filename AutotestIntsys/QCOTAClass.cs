using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using TDAPIOLELib;
using log4net;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace AutotestIntsys
{
    /// <summary>
    /// 
    /// </summary>
    public class QCOTAClass
    {
        private bool isConnectToQC;
        private bool isConnectProject;
        private TDConnectionClass tdConn;
        private TestConfigFile tconf;
        private ILog AutoLog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool QCConnected
        { get { return this.isConnectToQC; } }

        public bool ProjConnected
        { get { return this.isConnectProject; } }

        public QCOTAClass()
        {
            //AutoLog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            isConnectToQC = false;
            isConnectProject = false;
            tdConn = new TDAPIOLELib.TDConnectionClass();
            tconf = new TestConfigFile();
            tconf.QCConnect.ServerAddr = "http://localhost:8080/qcbin";
            tconf.QCConnect.Domain = "Default";
            tconf.QCConnect.Project = "Test";
            tconf.QCConnect.LoginName = "Administrator";
            tconf.QCConnect.Password = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tf"></param>
        public QCOTAClass(TestConfigFile tf)
        {
            isConnectToQC = false;
            isConnectProject = false;
            tdConn = new TDAPIOLELib.TDConnectionClass();
            tconf = tf;
        }

        public bool Connect()
        {
            try
            {
                tdConn.InitConnectionEx(tconf.QCConnect.ServerAddr);
                tdConn.Login(tconf.QCConnect.LoginName, tconf.QCConnect.Password);
                if (tdConn.Connected)
                    isConnectToQC = true;
                tdConn.Connect(tconf.QCConnect.Domain, tconf.QCConnect.Project);
                if (tdConn.ProjectConnected)
                    isConnectProject = true;
                if (tdConn.Connected && tdConn.ProjectConnected)
                {
                    AutoLog.Info("AutoIntSys: Success to connect to QC.");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                AutoLog.Info("AutoIntSys: Failed to connect to QC.");
                //Debug.Print("AutoIntSys:Failed to connect to QC.");
                if (e.HelpLink == null && e.Message != null)
                {
                    AutoLog.Info("AutoIntSys: ---QC Connect Message---");
                    AutoLog.Info("AutoIntSys: " + e.Message);
                    //Debug.Print("AutoIntSys:---QC Connect Message---");
                    //Debug.Print("AutoIntSys:" + e.Message);
                }
                if (e.HelpLink != null)
                {
                    Debug.Print("AutoIntSys:---QC Connect HelpLink---");
                    string errMsg = e.HelpLink;
                    string[] printMsg = errMsg.Split(':');
                    errMsg = printMsg[1].Trim();
                    errMsg = errMsg.Remove(errMsg.Length - 11);
                    Debug.Print("AutoIntSys:" + errMsg.Trim());
                }
                return false;
            }
        }

        public bool DisConnect()
        {
            try
            {
                if (tdConn.Connected)
                {
                    if (tdConn.ProjectConnected)
                    {
                        tdConn.DisconnectProject();
                        tdConn.ReleaseConnection();
                    }
                    else
                    { tdConn.ReleaseConnection(); }
                    AutoLog.Info("AutoIntSys: Success to disconnect from QC.");
                    return true;
                }
                return false;
            }
            catch
            {
                AutoLog.Info("AutoIntSys: Failed to disconnect from QC.");
                //Debug.Print("AutoIntSys:Failed to disconnect from QC.");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TDConnectionClass getTDConn()
        {
            return this.tdConn;
        }
    }
}
