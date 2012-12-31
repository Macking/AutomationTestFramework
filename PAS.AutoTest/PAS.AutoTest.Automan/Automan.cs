using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.Threading;
using System.IO;
using System.Diagnostics;
using PAS.AutoTest.PasATCore;
using PAS.AutoTest.PasATCoreV2;
using PAS.AutoTest.TestUtility;
using PAS.AutoTest.TestCase;
using PAS.AutoTest.TestData;
using System.Text.RegularExpressions;

namespace PAS.AutoTest.Automan
{
    public partial class Automan : Form
    {
        private ArrayList caseListToRun = null;
        private int caseNum = 0;
        private int runnedCaseNum = 0;
        private int passedCaseNum = 0;
        private int failedCaseNum = 0;
        private int noRunCaseNum = 0;
        private bool isRunStopRaised = false;
        private List<Thread> caseRunThreadList = null;
        private event EventHandler OnAllCasesFinished;
        private bool isPrepareDBExited = true;
        private bool isPreparePatientDirectoryExited = true;
        //private bool isPrepareTestDataExited = true;
        private ArrayList ignoredCases = new ArrayList();

        public Automan()
        {
            InitializeComponent();
            InitializeUI();
            IntilizeIgnoredCaseList();
        }

        private void IntilizeIgnoredCaseList()
        {
            ignoredCases.Add("Case505"); // For the reason: 3D case, only can be run with 3D graph card
            ignoredCases.Add("Case1598"); // For the reason: Import DIR. Failed with error: "No notification recieved after retry 5 times"
            ignoredCases.Add("Case1599"); // For the reason: Import DIR. This case will make CSDM restart. Failed with error "File Number not correct"
            ignoredCases.Add("Case1567"); // For the reason: version case, not ready
            ignoredCases.Add("Case1664"); // For the reason: new case, not ready
            ignoredCases.Add("Case1518"); // For the reason: need run with simulator, can not run two 3D viewer at the same time
            ignoredCases.Add("Case1540"); // For the reason: need run with simulator
            ignoredCases.Add("Case1541"); // For the reason: need run with simulator
            ignoredCases.Add("Case1659"); // For the reason: new case, not ready
            ignoredCases.Add("Case1660"); // For the reason: new case, not ready
            ignoredCases.Add("Case1670"); // For the reason: new case, not ready
            ignoredCases.Add("Case1671"); // For the reason: new case, not ready
            ignoredCases.Add("Case1672"); // For the reason: new case, not ready
        }

        private void InitializeUI()
        {
            if (TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.common, "installationMode").ToLower() != "server")
            {
                this.prepareDBBtn.Enabled = false;
            }

            this.stopBtn.Enabled = false;

            GenerateCaseListNode();
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            this.runBtn.Text = "Running";
            this.runBtn.Enabled = false;
            this.prepareDataBtn.Enabled = false;
            this.prepareDBBtn.Enabled = false;
            this.stopBtn.Enabled = true;

            isRunStopRaised = false;

            GenerateCaseListToRun(this.treeView_Cases);
            caseNum = caseListToRun.Count;

            ResetCaseRunSumInfo();

            OnAllCasesFinished += new EventHandler(OnAllCasesFinishedExecutor);

            ExecuteCases();
        }

        private void ExecuteCases()
        {
            caseRunThreadList = new List<Thread>();

            foreach (object tc in caseListToRun)
            {
                if (isRunStopRaised)
                {
                    break;
                }

                Thread runThread = new Thread(new ParameterizedThreadStart(RunSingleCase));
                caseRunThreadList.Add(runThread);
                runThread.Start(tc);
            }
        }

        private void GenerateCaseListNode()
        {
            this.treeView_Cases.BeginUpdate();
            this.treeView_Cases.Nodes.Add("Acquisition");
            this.treeView_Cases.Nodes.Add("Analysis");
            this.treeView_Cases.Nodes.Add("Application");
            this.treeView_Cases.Nodes.Add("FMS");
            this.treeView_Cases.Nodes.Add("Image");
            this.treeView_Cases.Nodes.Add("Import");
            this.treeView_Cases.Nodes.Add("GenericInstance");
            this.treeView_Cases.Nodes.Add("Patient");
            this.treeView_Cases.Nodes.Add("PS");
            this.treeView_Cases.Nodes.Add("RadioLogicalLog");
            this.treeView_Cases.Nodes.Add("SimpleInstance");
            this.treeView_Cases.Nodes.Add("WorkFlow");
            this.treeView_Cases.Nodes.Add("3D");
            this.treeView_Cases.Nodes.Add("Other");

            ArrayList methods = Utility.GenerateMethodList();
            foreach (string method in methods)
            {
                int index = treeView_Cases.Nodes.Count - 1; // by default, set to add into Other node

                for (int i = 0; i < treeView_Cases.Nodes.Count; i++)
                {
                    if (method.Contains("Run_" + treeView_Cases.Nodes[i].Text))
                    {
                        index = i;
                        break;
                    }
                }
                this.treeView_Cases.Nodes[index].Nodes.Add(method);
            }

            this.treeView_Cases.EndUpdate();
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            isRunStopRaised = true;

            try
            {
                foreach (Thread runThread in caseRunThreadList)
                {
                    if (runThread.IsAlive)
                    {
                        runThread.Abort();
                    }
                }
            }
            catch (Exception)
            {
                //eat it
            }

            OnAllCasesFinished(this, new EventArgs());
        }

        private void ResetCaseRunSumInfo()
        {
            runnedCaseNum = 0;
            passedCaseNum = 0;
            failedCaseNum = 0;
            noRunCaseNum = caseNum;
            passNum.Text = passedCaseNum.ToString();
            failNum.Text = failedCaseNum.ToString();
            noRunNum.Text = noRunCaseNum.ToString();
            runResultSumListView.Items.Clear();
        }

        private void RunSingleCase(object tc)
        {
            //Todo: need consider for acq case, only one process can start at the same time
            int count = 0;
            while (tc.ToString().ToLower().Contains("acq") && TestUtility.Utility.IsAcquisitionInProgress() && count < 3)
            {
                count++;
                Thread.Sleep(6000);
            }

            lock (this)
            {
                string methodName = tc.ToString();
                string scriptFileName = "TestData\\" + methodName + ".cod";
                string dataFileName = "TestData\\" + methodName + ".iod";
                try
                {
                    if (complieInTime.Checked) //Excete case by compile the code in time and then run
                    {
                        PAS.AutoTest.ScriptRunner.ScriptRunner sr = new PAS.AutoTest.ScriptRunner.ScriptRunner();
                        PAS.AutoTest.ScriptRunner.ExecuteResult executeResult;
                        if (File.Exists(dataFileName))
                        {
                            executeResult = sr.Run(scriptFileName, dataFileName, 600);
                        }
                        else
                        {
                            executeResult = sr.Run(scriptFileName, string.Empty, 600);
                        }
                    }
                    else //Excete case by reflection
                    {
                        Runner r = null;
                        if (File.Exists(dataFileName))
                        {
                            r = new Runner(dataFileName);
                        }
                        else
                        {
                            r = new Runner("");
                        }

                        r.GetType().GetMethod(methodName).Invoke(r, null);
                    }

                    Thread.Sleep(20);

                    XmlDocument doc = new XmlDocument();
                    doc.Load("LastOutput.xml");
                    XmlNodeList nodes = doc.SelectNodes("TestResult/Round/CheckPoint/Result");

                    bool isPass = true;
                    foreach (XmlNode node in nodes)
                    {
                        if (!Equals(node.InnerText, TestResult.Pass.ToString()) && !Equals(node.InnerText, TestResult.Done.ToString()))
                        {
                            isPass = false;
                            break;
                        }
                    }
                    if (isPass)
                    {
                        HandlePassedCase(methodName);
                    }
                    else
                    {
                        HandleFailedCase(methodName);
                    }

                    passNum.Text = passedCaseNum.ToString();
                    failNum.Text = failedCaseNum.ToString();
                    noRunNum.Text = noRunCaseNum.ToString();
                }
                catch (Exception ex)
                {
                    HandleExceptionCase(methodName, ex.ToString());

                    passNum.Text = passedCaseNum.ToString();
                    failNum.Text = failedCaseNum.ToString();
                    noRunNum.Text = noRunCaseNum.ToString();
                    return;
                }
            }
        }

        private void GenerateCaseListToRun(TreeView tree)
        {
            caseListToRun = new ArrayList(); // reset the case list
            TreeNodeCollection nodes = tree.Nodes;
            foreach (TreeNode n in nodes)
            {
                foreach (TreeNode tn in n.Nodes)
                {
                    if (tn.Checked)
                    {
                        bool shouldIgnore = false;
                        foreach (object var in ignoredCases)
                        {
                            shouldIgnore = tn.Text.EndsWith(var.ToString());  // tn.Text example: Run_Import_ImportDir_MainDirectory_Case1598
                            if (shouldIgnore)
                            {
                                break; // end the loop to check
                            }
                        }

                        if (shouldIgnore)
                        {
                            continue; // ignore this case and go on to next
                        }
                        else
                        {
                            caseListToRun.Add(tn.Text);
                        }
                    }
                }
            }
            noRunCaseNum = caseListToRun.Count;
            noRunNum.Text = noRunCaseNum.ToString();

            //string caseList = null;
            //foreach (object var in caseListToRun)
            //{
            //    caseList += var.ToString();
            //    caseList += "\r\n";
            //}
            //File.AppendAllText("C:\\TestCase.txt", caseList);

        }


        //todo: optimize below two methods to handle the result
        private void HandlePassedCase(string methodName)
        {
            runnedCaseNum++;
            noRunCaseNum--;
            passedCaseNum++;

            File.Copy("LastOutput.xml", "TestResult\\" + methodName + ".xml", true);
            File.Copy("LastOutput.xml", "TestResult\\" + "Pass_" + methodName + ".xml", true);

            //todo: need consider the cross-thread issue
            runResultSumListView.BeginUpdate();
            ListViewItem item = new ListViewItem(runnedCaseNum.ToString());
            item.SubItems.Add(methodName);
            item.SubItems.Add("Pass");
            item.SubItems.Add("");
            runResultSumListView.Items.Add(item);
            runResultSumListView.EndUpdate();

            if (runnedCaseNum == caseNum)
            {
                OnAllCasesFinished(this, new EventArgs());
            }
        }

        private void HandleFailedCase(string methodName)
        {
            runnedCaseNum++;
            noRunCaseNum--;
            failedCaseNum++;

            File.Copy("LastOutput.xml", "TestResult\\" + methodName + ".xml", true);
            File.Copy("LastOutput.xml", "TestResult\\" + "Fail_" + methodName + ".xml", true);

            runResultSumListView.BeginUpdate();
            ListViewItem item = new ListViewItem(runnedCaseNum.ToString());
            item.SubItems.Add(methodName);
            item.SubItems.Add("Fail");
            item.SubItems.Add("");
            runResultSumListView.Items.Add(item);
            runResultSumListView.EndUpdate();

            if (runnedCaseNum == caseNum)
            {
                OnAllCasesFinished(this, new EventArgs());
            }
        }

        private void HandleExceptionCase(string methodName, string message)
        {
            runnedCaseNum++;
            noRunCaseNum--;
            failedCaseNum++;

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("TestResult");
            root.SetAttribute("Exception", message);
            doc.AppendChild(root);

            if (System.IO.File.Exists("LastOutput.xml"))
            {
                System.IO.File.Delete("LastOutput.xml");
            }
            using (FileStream fs = new FileStream("LastOutput.xml", FileMode.CreateNew, FileAccess.Write))
            {
                doc.Save(fs);
            }

            File.Copy("LastOutput.xml", "TestResult\\" + methodName + ".xml", true);
            File.Copy("LastOutput.xml", "TestResult\\" + "Fail_" + methodName + ".xml", true);

            runResultSumListView.BeginUpdate();
            ListViewItem item = new ListViewItem(runnedCaseNum.ToString());
            item.SubItems.Add(methodName);
            item.SubItems.Add("Fail");
            item.SubItems.Add("Exception thrown when run case");
            runResultSumListView.Items.Add(item);
            runResultSumListView.EndUpdate();

            if (runnedCaseNum == caseNum)
            {
                OnAllCasesFinished(this, new EventArgs());
            }
        }

        private void OnAllCasesFinishedExecutor(object sender, EventArgs e) //Update the run button after all the case run threads have finished
        {
            this.prepareDataBtn.Enabled = true;
            this.prepareDBBtn.Enabled = true;
            this.runBtn.Enabled = true;
            this.runBtn.Text = "Run";
            this.singleCaseID.Text = string.Empty;
            this.stopBtn.Enabled = false;
        }

        private void treeView_Cases_AfterCheckNode(object sender, TreeViewEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            tree.AfterCheck -= new TreeViewEventHandler(this.treeView_Cases_AfterCheckNode);

            if (e.Node.Checked)
            {
                e.Node.ExpandAll();
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = true;
                }
            }
            else
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = false;
                }
                e.Node.Collapse();
            }

            if (e.Node.Checked == false && e.Node.Parent != null) //如果当前节点的父节点下的所有子节点都没有选中，去掉父节点的选中   
            {
                bool found = false; //父节点的子节点中至少有一个节点被选中，则found = true   
                foreach (TreeNode tn in e.Node.Parent.Nodes)
                {
                    if (tn.Checked == true)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false) //没有找到任何被选中的子节点   
                {
                    e.Node.Parent.Checked = false;
                }
            }

            tree.AfterCheck += new TreeViewEventHandler(this.treeView_Cases_AfterCheckNode);
        }

        private void treeView_Cases_DoubleClickNode(object sender, TreeNodeMouseClickEventArgs e)
        {
            string caseName = e.Node.Text;

            if (caseName.ToLower().Contains("run_")) // use the "Run_" to identify it's case node
            {
                string caseDataFile = "TestData\\" + caseName + ".iod";
                string caseCodeFile = "TestData\\" + caseName + ".cod";

                if (File.Exists(caseCodeFile))
                {
                    ProcessStartInfo i = new ProcessStartInfo("cmd.exe", "/c " + caseCodeFile);
                    i.CreateNoWindow = true;
                    i.WindowStyle = ProcessWindowStyle.Hidden;
                    System.Diagnostics.Process.Start(i);
                }
                else
                {
                    MessageBox.Show("Whoops, not find the test case code file: " + Application.StartupPath + "\\" + caseCodeFile);
                }

                if (File.Exists(caseDataFile))
                {
                    ProcessStartInfo i = new ProcessStartInfo("cmd.exe", "/c " + caseDataFile);
                    i.CreateNoWindow = true;
                    i.WindowStyle = ProcessWindowStyle.Hidden;
                    System.Diagnostics.Process.Start(i);
                }
                else
                {
                    MessageBox.Show("Whoops, not find the test case data file: " + Application.StartupPath + "\\" + caseDataFile);
                }
            }
        }

        private void runResultSumListView_DoubleClickItem(object sender, EventArgs e)
        {
            if (runResultSumListView.SelectedItems.Count > 0)
            {
                string caseName = runResultSumListView.SelectedItems[0].SubItems[1].Text; //Get the case name and then find the result xml
                string resultFilePath = "TestResult\\" + caseName + ".xml";
                if (File.Exists(resultFilePath))
                {
                    System.Diagnostics.Process.Start(resultFilePath);
                }
                else
                {
                    MessageBox.Show("Whoops, not find the result log file: " + Application.StartupPath + "\\" + resultFilePath);
                }
            }
        }

        private void prepareDataBtn_Click(object sender, EventArgs e)
        {
            prepareDataBtn.Text = "Preparing...";
            prepareDataBtn.Enabled = false;
            runBtn.Enabled = false;

            //isPrepareTestDataExited = false;

            ProcessStartInfo start = new ProcessStartInfo(@"FastCopy\FastCopy.exe");
            //need use '\' style as the limit of FastCopy tool
            string dataBakPath = @"D:\Test\Image_Lib_Backup";
            string dataPath = @"D:\Test\DICOM_Imag_Lib";
            start.Arguments = "/cmd=diff /error_stop=FALSE /force_close=TRUE  \"" + dataBakPath + "\" /to=\"" + dataPath + "\"";
            start.CreateNoWindow = true;
            start.UseShellExecute = false;

            Process p = Process.Start(start);
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(prepareDataProcess_Exited);
        }

        private void prepareDataProcess_Exited(object sender, EventArgs e)
        {
            Process p = (Process)sender;
            if (p.ExitCode == 0) //succeed
            {
                prepareDataBtn.ForeColor = SystemColors.HotTrack;
            }
            else // There is error when do copy
            {
                prepareDataBtn.ForeColor = Color.Maroon;
            }
            p.Close();

            //isPrepareTestDataExited = true;

            prepareDataBtn.Text = "Prepare Test Data";
            prepareDataBtn.Enabled = true;
            if (prepareDBBtn.Enabled)
            {
                runBtn.Enabled = true;
            }
        }

        private void prepareDBBtn_Click(object sender, EventArgs e)
        {
            prepareDBBtn.Text = "Preparing...";
            prepareDBBtn.Enabled = false;
            runBtn.Enabled = false;

            isPrepareDBExited = false;
            isPreparePatientDirectoryExited = false;

            //Stop the CSDM server
            TestUtility.Utility.StopCSDM();

            //Exit the Controller to let Derby stop. Add for Sprint 8 and later
            TestUtility.Utility.ExitController();
            System.Threading.Thread.Sleep(5000);

            //Prepare the DB
            string dbBakPath = @"D:\Test\DB\database"; //need use '\' style as the limit of FastCopy tool
            string dbPath = TestUtility.Utility.GetCSDMConfig(CSDMConfigSection.remote, "databaseDirectory");

            //Remove the old DB folder
            if (Directory.Exists(dbPath))
            {
                Directory.Delete(dbPath, true);
            }

            ProcessStartInfo startForDB = new ProcessStartInfo(@"FastCopy\FastCopy.exe");
            dbPath = dbPath.Replace('/', '\\'); //need use '\' style as the limit of FastCopy tool
            startForDB.Arguments = "/cmd=force_copy /error_stop=FALSE /force_close=TRUE  \"" + dbBakPath + "\" /to=\"" + dbPath + "\"";
            startForDB.CreateNoWindow = true;
            startForDB.UseShellExecute = false;

            Process pForDB = Process.Start(startForDB);
            pForDB.EnableRaisingEvents = true;
            pForDB.Exited += new EventHandler(prepareDBProcess_Exited);

            //Prepare the patient folder
            ProcessStartInfo startForPatientDirectory = new ProcessStartInfo(@"FastCopy\FastCopy.exe");
            string patientDirectoryBak = @"D:\Test\DB\patient"; //need use '\' style as the limit of FastCopy tool
            string patientDirectory = TestUtility.Utility.GetCSDMConfig(CSDMConfigSection.remote, "patientDirectory");
            patientDirectory = patientDirectory.Replace('/', '\\'); //need use '\' style as the limit of FastCopy tool
            startForPatientDirectory.Arguments = "/cmd=diff /error_stop=FALSE /force_close=TRUE  \"" + patientDirectoryBak + "\" /to=\"" + patientDirectory + "\"";
            startForPatientDirectory.CreateNoWindow = true;
            startForPatientDirectory.UseShellExecute = false;

            Process pForPatientDirectory = Process.Start(startForPatientDirectory);
            pForPatientDirectory.EnableRaisingEvents = true;
            pForPatientDirectory.Exited += new EventHandler(preparePatientDirectoryExited);
        }

        private void prepareDBProcess_Exited(object sender, EventArgs e)
        {
            //Upgrade the DB
            int returnCode = TestUtility.Utility.UpdateCSDMDB();

            Process p = sender as Process;
            if (p.ExitCode == 0 && returnCode == 0) //succeed
            {
                prepareDBBtn.ForeColor = SystemColors.HotTrack;
            }
            else // There is error when do copy
            {
                prepareDBBtn.ForeColor = Color.Maroon;
            }
            p.Close();

            isPrepareDBExited = true;

            //Start the CSDM server
            TestUtility.Utility.StartCSDM();

            if (isPreparePatientDirectoryExited)
            {
                prepareDBBtn.Text = "Prepare Database";
                prepareDBBtn.Enabled = true;
                if (prepareDataBtn.Enabled)
                {
                    runBtn.Enabled = true;
                }
            }
        }

        private void preparePatientDirectoryExited(object sender, EventArgs e)
        {
            Process p = sender as Process;
            p.Close();

            isPreparePatientDirectoryExited = true;

            if (isPrepareDBExited)
            {
                prepareDBBtn.Text = "Prepare Database";
                prepareDBBtn.Enabled = true;
                if (prepareDataBtn.Enabled)
                {
                    runBtn.Enabled = true;
                }
            }
        }

        private void selectAll_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.selectAll_CheckBox.CheckState == CheckState.Checked)
            {
                for (int i = 0; i < treeView_Cases.Nodes.Count; i++)
                {
                    treeView_Cases.Nodes[i].Checked = true;
                }
                treeView_Cases.Nodes[0].EnsureVisible(); //make the scrollbar show to the top
            }
            else if (this.selectAll_CheckBox.CheckState == CheckState.Unchecked)
            {
                for (int i = 0; i < treeView_Cases.Nodes.Count; i++)
                {
                    treeView_Cases.Nodes[i].Checked = false;
                }
            }
        }

        private void uploadToQCBtn_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;

            DialogResult dr = MessageBox.Show("Be careful!! This will remove the selected case's all attachments firstly and then upload new ones!!!", "Upload Cases To QC", messButton);
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            //Change btn status

            // Identify which cases need be uploaded
            GenerateCaseListToRun(this.treeView_Cases);

            //Todo: Generate the cod file for each case

            // Upload the case files as attachment
            foreach (object tc in caseListToRun)
            {
                string caseName = tc.ToString();
                try
                {
                    bool removeSuccess = TestUtility.Utility.QC_RemoveAttachementFromCase(caseName);
                    bool upLoadSuccess = TestUtility.Utility.QC_UploadAttachementToCase(caseName);

                    //Todo: Add a log to record each case upload status
                    if (removeSuccess && upLoadSuccess)
                    {
                        // add success log
                    }
                    else
                    {
                        // add fail log
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // add fail log
                }

            }

            // Update btn status
        }

        private void singleCaseID_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if ("1234567890".IndexOf(e.KeyChar) == -1 && (int)e.KeyChar != 8 && (int)e.KeyChar != 1 && (int)e.KeyChar != 3 && (int)e.KeyChar != 22 && (int)e.KeyChar != 24) // Allow: number, backspace, ctrl+a (1), ctrl+c (3), ctrl+v (22), ctrl+x (24)
            {
                e.Handled = true;
            }
        }

        private void singleCaseID_LostFocus(object sender, System.EventArgs e)
        {
            this.selectAll_CheckBox.Checked = false;

            //Make other nodes unselected and select the target case code and 
            TreeNode targetNode = null;

            foreach (TreeNode n in treeView_Cases.Nodes)
            {
                foreach (TreeNode tn in n.Nodes)
                {
                    if (!tn.Text.ToLower().EndsWith("case" + singleCaseID.Text))
                    {
                        tn.Checked = false;
                        tn.Parent.Collapse();
                    }
                    else
                    {
                        targetNode = tn;
                    }
                }
            }

            if (targetNode != null)
            {
                targetNode.Checked = true;
                targetNode.Parent.Expand();
            }
        }
    }
}