using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

using PAS.AutoTest.TestData;

namespace PAS.AutoTest.RemoteController
{
    public class Controller
    {
        private string mHost = "127.0.0.1";
        private int mPort = 6241;
        private TcpClient mTcpClient = new TcpClient();
        private string mValidatorAppName = "PASValidator";

        public Controller() { }

        public Controller(string host, int port) 
        {
            this.mHost = host;
            this.mPort = port;
            this.mTcpClient = new TcpClient(this.mHost, this.mPort);
        }

        /// <summary>
        /// host name or ip address.
        /// </summary>
        public string Host
        {
            get { return mHost; }
            set { this.mHost = value; }
        }

        /// <summary>
        /// port #, 6241 as default.
        /// </summary>
        public int Port
        {
            get { return this.mPort; }
            set { this.mPort = value; }
        }

        /// <summary>
        /// gets or sets app full path.
        /// </summary>
        public string AppPath
        {
            get { return "test"; }
            set { string a = value; }
        }

        public bool LaunchValidator()
        {
            //if the validator is running============================
            Process[] ps = Process.GetProcesses();

            for (int j = 0; j < ps.Length; j++)
            {
                if (ps[j].ProcessName.ToUpper().Trim ()==this.mValidatorAppName.ToUpper ().Trim ())
                {
                    return true;
                }
            }
            //=============================================

            try
            {
                Process.Start(this.AppPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public delegate void TestCaseCompleteEventHandler(object sender, TestCaseCompleteEventArgs e);
        public event TestCaseCompleteEventHandler TestCaseCompleteEvent;

        public bool RunCase(string codeFile, string dataFile, string testCaseName, string RunId)
        {
            return true;
        }
    }

    public class TestCaseCompleteEventArgs
    {
        public TestResult Result;
        public string ResultPath = string.Empty;

        public string TestCaseName = string.Empty;
        public string RunId = string.Empty;
    }
}
