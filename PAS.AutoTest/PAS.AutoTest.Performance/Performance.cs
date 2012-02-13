using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection; 

using PAS.AutoTest.PasATCore;
using PAS.AutoTest .Performance.TestCase ;

namespace PAS.AutoTest.Performance
{
    public partial class Performance : Form
    {
        public Performance()
        {
            InitializeComponent();
        }

        private string mTestCaseTypeBaseName = "TestCaseTemplate";
        private List<ITestCase> mTestCase = new List<ITestCase>();
        private object mSyncObj = new object();

        private int mCurrentCaseRepeat = 0;
        private int mCurrentCaseExecuted = 0;
        private int mCurrentCaseFailed = 0;
        private double mCurrentCaseExecutedTime = 0;
        private int mTestCaseCount = 0;
        private List<Thread> mWorkingThread = new List<Thread>();
        private Thread mUIThread = null;
        private Thread mRunAllThread = null;

        private delegate void UpdateUIHandler(ITestCase tc);
        private delegate void SelectTestCaseHandler(int index);

        private void SelectTestCase(int index)
        {
            if(index>=0 && index<this.cbTestCases .Items .Count )
                this.cbTestCases.SelectedIndex = index;
        }

        private void RunCaseSync(ITestCase cp)
        {
            this.mCurrentCaseExecuted = 0;
            this.mCurrentCaseFailed = 0;
            this.mCurrentCaseExecutedTime = 0;

            PAS.AutoTest.Performance.TestCase.Log.StartUpdateSync(); // start to log data.
            cp.Label = this.tbLabel.Text.Trim();
            cp.Reset();

            if (cp != null)
            {
                for (int i = 0; i < cp.Concurrent; i++)
                {
                    ITestCase tc = Activator.CreateInstance(cp.GetType()) as ITestCase;
                    tc.Label = cp.Label;
                    tc.Repeat = cp.Repeat;
                    tc.ExecutedTime = cp.ExecutedTime;
                    tc.FunctionName = cp.FunctionName;
                    tc.Executed = cp.Executed;
                    tc.Repeat = cp.Repeat;
                    tc.SingleCallCompleteEvent +=new SingleCallCompleteEventHandler(tc_SingleCallCompleteEvent);
                    tc.TestCaseCompleteEvent += new TestCaseCompleteEventhandler(tc_TestCaseCompleteEvent);

                    Thread t = new Thread(new ParameterizedThreadStart(this.RunCase));
                    t.Priority = ThreadPriority.Highest;
                    this.mWorkingThread.Add(t);

                    t.Start(tc);
                }

                if (this.mUIThread !=null && this.mUIThread.ThreadState == ThreadState.Running)
                    this.mUIThread.Abort();

                this.mUIThread = new Thread(this.UpdateUISync);
                this.mUIThread.Start(cp);

            }
        }

        private void tc_TestCaseCompleteEvent(object sender, EventArgs e)
        {
            ITestCase tc = sender as ITestCase;
            tc.EndTestCase();
            Thread.CurrentThread.Abort();
        }

        private void RunAllSync()
        {
            SelectTestCaseHandler sc = new SelectTestCaseHandler(this.SelectTestCase);

            for (int i = 0; i < this.mTestCaseCount; i++)
            {
                //select the running case as the selected item in test case combo
                this.Invoke(sc, i);

                ITestCase tc = this.mTestCase[i];
                tc.Label = this.tbLabel.Text.Trim();
                tc.Reset();
                this.RunCaseSync(tc);

                //wait until all thread done.
                while (this.mCurrentCaseExecuted != this.mCurrentCaseRepeat) { }

                //stop all working thread
                for (int n = 0; n < this.mWorkingThread.Count; n++)
                {
                    mWorkingThread[n].Abort();
                }

                this.mUIThread.Abort();
                this.mWorkingThread.Clear();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.mWorkingThread.Clear();

            if (this.tbLabel.Text.Trim() != string.Empty)
            {
                if (this.cbRunAll.Checked)
                {
                    this.mRunAllThread = new Thread(this.RunAllSync);
                    this.mRunAllThread.Start();
                }
                else
                {
                    ITestCase tc = this.cbTestCases.SelectedItem as ITestCase;
                    this.RunCaseSync(tc);
                }
            }
            else
            {
                MessageBox.Show("Please enter a label.");
            }
        }

        private void UpdateUISync(object para)
        {
            ITestCase tc = para as ITestCase;
            UpdateUIHandler uph = new UpdateUIHandler(this.UpdateUI);

            while (true)
            {
                this.Invoke(uph, tc);
                Thread.Sleep(100);

                if (tc.Repeat == tc.Executed)
                {
                    this.Invoke(uph, tc);
                    break;
                }
            }
        }

        private void RunCase(object para)
        {
            ITestCase tc = para as ITestCase;
            tc.Run();
        }

        private void UpdateUI(ITestCase tc)
        {
            this.lbExecuted.Text = this.mCurrentCaseExecuted.ToString();
            this.pbProgress.Value = Math.Min((int)(this.mCurrentCaseExecuted * 100 / tc.Repeat), 100);
            this.lbFailed.Text = this.mCurrentCaseFailed.ToString();

            if (this.mCurrentCaseExecuted == 0 || this.mCurrentCaseExecutedTime == 0)
                this.lbAverage.Text = "0";
            else
                this.lbAverage.Text = ((int)(this.mCurrentCaseExecutedTime / this.mCurrentCaseExecuted)).ToString();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                Assembly ass = Assembly.LoadFile(this.tbTestCasePath.Text.Trim());
                Type[] ts = ass.GetTypes();

                foreach (Type t in ts)
                {
                    if (t.BaseType !=null && t.BaseType.Name == this.mTestCaseTypeBaseName)
                    {
                        ITestCase tc = (ITestCase)(Activator.CreateInstance(t));
                        tc.SingleCallCompleteEvent += new SingleCallCompleteEventHandler(tc_SingleCallCompleteEvent);
                        this.mTestCase.Add(tc);
                    }
                }

                this.cbTestCases.DataSource = this.mTestCase;
                this.cbTestCases.DisplayMember = "FunctionName";
                this.mTestCaseCount = this.cbTestCases.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tc_SingleCallCompleteEvent(object sender, TestCaseResultArgs e)
        {
            lock (this.mSyncObj)
            {
                this.mCurrentCaseExecuted++;

                if (e.Result)
                    this.mCurrentCaseExecutedTime += e.Response;
                else
                    this.mCurrentCaseFailed++;

                if (this.mCurrentCaseExecuted >= this.mCurrentCaseRepeat)
                {
                    this.mCurrentCaseExecuted = this.mCurrentCaseRepeat;
                    ITestCase tc = sender as ITestCase;
                    tc.EndTestCase();
                    Thread.CurrentThread.Abort();
                }
            }
        }

        private void Performance_Load(object sender, EventArgs e)
        {
            this.tbTestCasePath.Text = Configuration.TestCasePath;
        }

        private void cbTestCases_SelectedIndexChanged(object sender, EventArgs e)
        {
            ITestCase tc = this.cbTestCases.SelectedItem as ITestCase;

            if (tc != null)
            {
                this.lbFunctionName.Text = tc.FunctionName;
                this.lbRepeat.Text = tc.Repeat.ToString();
                this.lbExecuted.Text = tc.Executed.ToString();
                this.lbFailed.Text = tc.Failed.ToString();
                this.lbConcurrent.Text = tc.Concurrent.ToString();
                this.lbAverage.Text = tc.Average.ToString();
                this.mCurrentCaseRepeat = tc.Repeat;
                this.mCurrentCaseExecuted = tc.Executed;
                
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.mWorkingThread.Count; i++)
            {
                mWorkingThread[i].Abort();
            }

            this.mUIThread.Abort();

            this.mWorkingThread.Clear();
        }

        /// <summary>
        /// when close the main windows. abord all working thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Performance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.mUIThread != null)
                this.mUIThread.Abort();

            foreach (Thread t in this.mWorkingThread)
            {
                if (t != null)
                    t.Abort();
            }
        }

        private void cbRunAll_CheckedChanged(object sender, EventArgs e)
        {
            this.cbTestCases.Enabled = !this.cbRunAll.Checked;
        }

        private void btnViewResult_Click(object sender, EventArgs e)
        {
            ResultView rv = new ResultView();
            rv.Show();
        }
    }
}