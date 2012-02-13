using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Diagnostics;

namespace PAS.AutoTest.Performance.TestCase
{
    public class TestCaseTemplate: ITestCase 
    {
        #region Protected Members

        protected int mRepeat = 0;
        protected string mFunctionName = string.Empty;
        protected int mExecuted = 0;
        protected string mLabel = string.Empty;
        protected int mFailed = 0;
        protected double mExectuedTime = 0;
        protected int mAverage = 0;
        protected int mConcurrent = 1;
        private Thread NotificationLaunchThread;
        private Process p;

        #endregion

        #region Properties

        public int Repeat
        {
            get { return this.mRepeat; }
            set { this.mRepeat = value; }
        }

        public string FunctionName
        {
            get { return this.mFunctionName; }
            set { this.mFunctionName = value; }
        }

        public int Executed
        {
            get { return this.mExecuted; }
            set 
            {
                this.mExecuted = value;
            }
        }

        public string Label
        {
            get { return this.mLabel; }
            set { this.mLabel = value; }
        }

        public int Failed
        {
            get { return this.mFailed; }
            set { this.mFailed = value; }
        }

        public double ExecutedTime
        {
            get { return this.mExectuedTime; }
            set { this.mExectuedTime = value; }
        }

        public int Average
        {
            get 
            {
                if (this.mExecuted == 0 || this.mExectuedTime == 0)
                    return 0;
                return (int)(this.mExectuedTime / this.mExecuted); 
            }
        }

        public int Concurrent
        {
            get { return this.mConcurrent; }
            set { this.mConcurrent = value; }
        }

        #endregion

        public event SingleCallCompleteEventHandler SingleCallCompleteEvent;
        public event TestCaseCompleteEventhandler TestCaseCompleteEvent;
        public virtual void Run() { }

        public void Reset()
        {
            this.mExectuedTime = 0;
            this.mExecuted = 0;
            this.mFailed = 0;
        }

        public void LoadConfig()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("TestCaseConfig.xml");

           XmlNodeList ns= doc.SelectNodes ("Config/TestCase");

            foreach(XmlNode node in ns)
            {
                if(node.Attributes ["Name"].Value.Trim ().ToUpper ()==this.mFunctionName.Trim ().ToUpper())
                {
                    this.mRepeat=Int32.Parse (node.SelectSingleNode ("Repeat").InnerText);
                    this.Concurrent = Int32.Parse(node.SelectSingleNode("Concurrent").InnerText);
                    break;
                }
            }
        }

        public virtual void EndTestCase() { }

        public void RiseSingleCallCompleteEvent(double response, bool result)
        {
            SingleCallCompleteEvent(this, new TestCaseResultArgs(response, result));
        }

        public void RiseSingleCallCompleteEvent(double response, bool result, bool isTestCaseException)
        {
            SingleCallCompleteEvent(this, new TestCaseResultArgs(response, result, isTestCaseException));
        }

        public void RiseTestCaseCompleteEvent()
        {
            TestCaseCompleteEvent(this, null);
        }

        public void LaunchNotification()
        {
          ThreadStart entryPoint = new ThreadStart(RunCmd);
          NotificationLaunchThread = new Thread(entryPoint);
          NotificationLaunchThread.Name = "NOTIFICATION_RUNNING";
          NotificationLaunchThread.Start();
        }

        public void FinishNotification()
        {
          p.Kill();
          NotificationLaunchThread.Abort();
        }

        private void RunCmd()
        {
          p = new Process();
          p.StartInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + @"\ActiveMQClient_C#.exe";
          p.StartInfo.CreateNoWindow = true;
          p.Start();          
        }
    }

    public class TestCaseResultArgs
    {
        public double Response;
        public bool Result;
        public bool IsTestCaseException;

        public TestCaseResultArgs(double response, bool result)
        {
            this.Response = response;
            this.Result = result;
        }

        public TestCaseResultArgs(double response, bool result, bool isTestCaseException)
        {
            this.Response = response;
            this.Result = result;
            this.IsTestCaseException = isTestCaseException;
        }
    }
}
