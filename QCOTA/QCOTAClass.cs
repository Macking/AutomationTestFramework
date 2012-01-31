using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using TDAPIOLELib;

namespace AutoTestComponent
{
    /// <summary>
    /// 
    /// </summary>
    public class QCOTAClass
    {
        private bool isConnectToQC;
        private bool isConnectProject;
        private TDConnection tdConn;
        private TestConfigFile tconf;

        public bool QCConnected
        { get { return this.isConnectToQC; } }

        public bool ProjConnected
        { get { return this.isConnectProject; } }

        public QCOTAClass()
        {
            isConnectToQC = false;
            isConnectProject = false;
            tdConn = new TDAPIOLELib.TDConnection();
            /*
            //tconf = new TestConfigFile();
            //tconf.QCConnect.ServerAddr = "http://localhost:8080/qcbin";
            //tconf.QCConnect.Domain = "Default";
            //tconf.QCConnect.Project = "Test";
            //tconf.QCConnect.LoginName = "Administrator";
            //tconf.QCConnect.Password = "";*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tf"></param>
        public QCOTAClass(TestConfigFile tf)
        {
            isConnectToQC = false;
            isConnectProject = false;
            tdConn = new TDAPIOLELib.TDConnection();
            tconf = tf;
        }

        public void Login(string address, string user, string password)
        {
            try
            {
                tdConn.InitConnectionEx(address);
                tdConn.Login(user, password);
                if (tdConn.Connected)
                    isConnectToQC = true;
            }
            catch { }
        }

        public void Logout()
        {
            try
            {
                tdConn.ReleaseConnection();
                if (!tdConn.Connected)
                    isConnectToQC = false;
            }
            catch { }
        }
        public void Connect(string domain, string project)
        {
            try
            {
                tdConn.Connect(domain, project);
                if (tdConn.ProjectConnected)
                    isConnectProject = true;
            }
            catch { }
        }

        public void Connect()
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
            }
            catch { }
        }

        public void Connect(string address, string user, string password, string domain, string project)
        {
            try
            {
                tdConn.InitConnectionEx(address);
                tdConn.Login(user, password);
                if (tdConn.Connected)
                    isConnectToQC = true;
                tdConn.Connect(domain, project);
                if (tdConn.ProjectConnected)
                    isConnectProject = true;
            }
            catch { }
        }

        public void DisConnect()
        {
            try
            {
                if (tdConn.Connected)
                {
                    if (tdConn.ProjectConnected)
                    {
                        tdConn.DisconnectProject();
                        isConnectProject = false;
                    }
                }
            }
            catch { }
        }
        public void DisConnect(bool all)
        {
            try
            {
                if (tdConn.Connected)
                {
                    if (tdConn.ProjectConnected)
                    {
                        tdConn.DisconnectProject();
                        isConnectProject = false;
                    }
                    tdConn.ReleaseConnection();
                    isConnectToQC = false;
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TDConnection getTDConn()
        {
            return this.tdConn;
        }

        public List<string> getDomainList()
        {
            try
            {
                List<string> reDomList = new List<string>();
                List domList = tdConn.DomainsList;
                foreach (string d in domList)
                {
                    reDomList.Add(d);
                }
                return reDomList;
            }
            catch { return null; }
        }

        public List<string> getProjectList(string Domain)
        {
            try
            {
                List<string> reProList = new List<string>();
                List proList = tdConn.get_ProjectsListEx(Domain);
                foreach (string d in proList)
                {
                    reProList.Add(d);
                }
                return reProList;
            }
            catch { return null; }
        }
    }
}