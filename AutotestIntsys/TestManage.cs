using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using TDAPIOLELib;
using log4net;

namespace AutotestIntsys
{
  class TestManage
  {
    private TDConnectionClass tdConn;
    private TestConfigFile configFile;
    private bool createTFSucess;
    private bool createTSSucess;
    private string tsFolderName;
    private ILog AutoLog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    //Constructor should provide the QC Connection Handle and TestConfigFile as parameters
    public TestManage(TDConnectionClass td, TestConfigFile tf)
    {
      tdConn = td;
      configFile = tf;
      tsFolderName = "";
      createTFSucess = false;
      createTSSucess = false;
    }

    //Given the test target machine name for calling RunTestSets
    public bool RunTestSets(string testMachine,string configPath)
    {
      try
      {
        TestSetTreeManager TestSetTreeMana = tdConn.TestSetTreeManager as TestSetTreeManager;
        TestSetFolder TestSetFolderPath = TestSetTreeMana.get_NodeByPath("Root\\AUTORUN\\" + tsFolderName) as TestSetFolder;
        TestSetFactory TestSetFact = TestSetFolderPath.TestSetFactory as TestSetFactory;
        TDFilter TestSetFilter = TestSetFact.Filter as TDFilter;
        List TestSetList = TestSetFilter.NewList() as List;
        foreach (TestSet ts in TestSetList)
        {
          // need give the testmachine name and notice the whether use QTP,
          // These value from command input
          
          if (testMachine != null && testMachine != "")
          {
            AutoLog.Info("[bool RunTestSets]Begin to run test set: " + ts.Name);
            //Console.Out.WriteLine("==========[bool RunTestSets]Begin to run test set: " + ts.Name);
            RunCurrentSet(ts, testMachine, configFile.RunParameter.UsingQTP, configPath);
            AutoLog.Info("[bool RunTestSets]End to run test set: " + ts.Name);
            //Console.Out.WriteLine("==========[bool RunTestSets]End run test set: " + ts.Name);
          }
          else
          {
            RunCurrentSet(ts, configFile.RunParameter.TestMachine, configFile.RunParameter.UsingQTP,configPath);
          }
        }
        
        return true;
      }
      catch (Exception e)
      {
        AutoLog.Info("AutoIntSys: Run test sets exception" + e.Message);
        //Console.Out.WriteLine("AutoIntSys: Run test sets exception" + e.Message);
        //Debug.Print("AutoIntSys:" + e);
        //Debug.Print("AutoIntSys:Run Test Sets Error!");
        return false;
      }
    }

    //Given the special catalog for record the test sets and running logs
    public bool CreateTestSets(string FolderName)
    {
      try
      {
        //TDConnectionClass TDConnHandle = qc.getTDConn();
        // string rootPath = "Root\\AUTORUN\\";
        tsFolderName = FolderName;
        List<string> TestSetNames = GetAllTestSetNames(configFile.CaseFilter.TestSetName);
        if (TestSetNames != null)
        {
          TestSetFolder tsFold = CreateTestSetFolder(FolderName);          
          if (tsFold != null && createTFSucess)
          {
            TestSetFolder CreateTS = CreateTestSetNames(TestSetNames, tsFold);
          }
          if (createTSSucess)
          {
            AutoLog.Info("[bool CreateTestSets]AutoIntSys: Create Test sets success");
            //Console.Out.WriteLine("==========[bool CreateTestSets]AutoIntSys: Create Test sets success");
            return true;
          }
        }
        return false;
      }
      catch (Exception e)
      {
        AutoLog.Info("AutoIntSys: Exception with " + e.Message);
        AutoLog.Info("AutoIntSys: Create Test Sets Error!");
        //Debug.Print("AutoIntSys:" + e);
        //Debug.Print("AutoIntSys:Create Test Sets Error!");
        //Console.Out.WriteLine("Create Test Sets Error!");
        return false;
      }
    }
    
    /*
      //try
      //{
      //  ////get list of column name
      //  //List fieldList = TDConnHandle.get_Fields("TEST");      
      //  //int a = fieldList.Count;
      //  //foreach (TDField aField in fieldList)
      //  //{
      //  //  Console.WriteLine("Field name:{0}", aField.Name);          
      //  //}

      //  //TestSetTreeManager TestSetTreeMana = TDConnHandle.TestSetTreeManager as TestSetTreeManager;
      //  //TestSetFolder TestSetFolderPath = TestSetTreeMana.get_NodeByPath(rootPath) as TestSetFolder;
      //  ////Add a new folder under the rootPath
      //  //SysTreeNode CurTestSetFolder = TestSetFolderPath.AddNode(TestSetFolderName) as SysTreeNode;
      //  //CurTestSetFolder.Post();

      //  ////The new test set will be added under the newly test folder
      //  //TestSetFolder TSFolder = TestSetTreeMana.get_NodeById(CurTestSetFolder.NodeID) as TestSetFolder;
      //  //TestSetFactory TSFact = TSFolder.TestSetFactory as TestSetFactory;

      //  //TestFactory TestFact = TDConnHandle.TestFactory as TestFactory;
      //  //TDFilter TFilter = TestFact.Filter as TDFilter;
      //  ////All match item will be filtered by the provided field and value
      //  //foreach (TestConfigFile.AutorunFilerConfig filter in TConfig.CaseFilter.AutorunFilter)
      //  //{
      //  //  TFilter[filter.FieldName] = filter.FieldValue;
      //  //}

      //  ////All matched test cases will be storaged in TList temporarily        
      //  //List TList = TFilter.NewList();

      //  ////Add test set name
      //  //TestSet TestSetName; 
      //  //TSTestFactory TSTestFact;

      //  //TestSetName = TSFact.AddItem("test set 1") as TestSet;
      //  //TestSetName.Post();
      //  //TSTestFact = TestSetName.TSTestFactory as TSTestFactory;

      //  //foreach (Test t in TList)
      //  //{

      //  //  TSTestFact.AddItem(t.ID);
      //  //}
      //  //All test cases will be saved into the specified folder's testSet
      //  return true;
      //}
      //catch (Exception e)
      //{
      //  Console.WriteLine("Create Test Cases Error!");
      //  Console.WriteLine("{0}", e.Message);
      //  return false;
      //}
       * */


    //According to the search conditions defined in XML, generate a list of TestSet names
    private List<string> GetAllTestSetNames(string TestSetNameField)
    {
      // Using SQL and ICommand to get the specified filed values, and store them to a List
      try
      {
        ICommand com = tdConn.Command as ICommand;
        com.CommandText = "select distinct " + TestSetNameField + " from test";
        IRecordset recList = com.Execute() as IRecordset;
        // Console.WriteLine("Get record count: {0}", recList.RecordCount);

        recList.First();
        object specColumnValue;
        List<string> TestSetNameList = new List<string>();

        while (!recList.EOR)
        {
          specColumnValue = recList[TestSetNameField];
          if (specColumnValue != null)
          {
            TestSetNameList.Insert(0, specColumnValue.ToString());
          }
          recList.Next();
        }
        return TestSetNameList;
      }
      catch (Exception e)
      {
        AutoLog.Info("AutoIntSys: Exception with " + e.Message);
        AutoLog.Info("AutoIntSys: Fail to get Specified Field Value from TEST Table!");
        //Debug.Print("AutoIntSys:" + e);
        //Debug.Print("AutoIntSys:Fail to get Specified Field Value from TEST Table!");
        return null;
      }
    }

    private static bool CheckFolderIsExist(TestSetFolder TSFolder, string FolderName)
    {
      try
      {
        SysTreeNode nodeExist = TSFolder.FindChildNode(FolderName) as SysTreeNode;
        if (nodeExist != null)
          return true;
        else
          return false;
      }
      catch
      {
        return false;
      }
    }

    //Add the special folder under the ROOT\AUTORUN
    //If the folder has already exist, then return the folder handle
    private TestSetFolder CreateTestSetFolder(string FolderName)
    {
      string rootPath = "Root\\AUTORUN\\";
      try
      {
        TestSetTreeManager TestSetTreeMana = tdConn.TestSetTreeManager as TestSetTreeManager;
        TestSetFolder TestSetFolderPath = TestSetTreeMana.get_NodeByPath(rootPath) as TestSetFolder;
        //Add a new folder under the rootPath

        if (CheckFolderIsExist(TestSetFolderPath, FolderName))
        {
          AutoLog.Info("AutoIntSys: Test folder has already in QC!");
          //Debug.Print("AutoIntSys: Test folder has already in QC!");
          SysTreeNode getTestSetFolder = TestSetFolderPath.FindChildNode(FolderName) as SysTreeNode;
          TestSetFolder TSExistFolder = TestSetTreeMana.get_NodeById(getTestSetFolder.NodeID) as TestSetFolder;
          
          createTFSucess = false;
          return TSExistFolder;
        }

        SysTreeNode CurTestSetFolder = TestSetFolderPath.AddNode(FolderName) as SysTreeNode;
        CurTestSetFolder.Post();

        //The new test set will be added under the newly test folder
        TestSetFolder TSFolder = TestSetTreeMana.get_NodeById(CurTestSetFolder.NodeID) as TestSetFolder;
        createTFSucess = true;
        return TSFolder;
      }
      catch (Exception e)
      {
        AutoLog.Info("AutoIntSys: Exception with " + e.Message);
        AutoLog.Info("AutoIntSys: Create Test Sets Error!");
        //Debug.Print("AutoIntSys:Create Test Sets Error!");
        //Debug.Print("AutoIntSys:" + e.Message);
        return null;
      }
    }

    private string GenerateSQLText(string[] columnNames)
    {
      int lenNames = columnNames.GetLength(0);
      string strSQLText;
      strSQLText = "select ";
      foreach (string col in columnNames)
      {
        strSQLText += col;
        lenNames -= 1;
        if (lenNames != 0)
        {
          strSQLText += ",";
        }
      }

      strSQLText += " from test";
      if (configFile.CaseFilter.AutorunFilter != null)
      {
        strSQLText += " where ";
        int filterNumber = configFile.CaseFilter.AutorunFilter.GetLength(0);
        foreach (TestConfigFile.AutorunFilerConfig filter in configFile.CaseFilter.AutorunFilter)
        {
          string searchValue = filter.FieldValue.Trim();
          if (searchValue.Contains(">") || searchValue.Contains("<"))
            strSQLText += filter.FieldName + filter.FieldValue;
          else
            strSQLText += filter.FieldName + " = '" + filter.FieldValue + "'";
          filterNumber -= 1;
          if (filterNumber != 0)
          {
            strSQLText += " AND ";
          }
        }

      }

      return strSQLText;
    }

    private TestSetFolder CreateTestSetNames(List<string> TestSetNames, TestSetFolder TSFolder)
    {
      List<ItemValue> tcList = new List<ItemValue>();
      #region Generate TestSet Name
      //Get all match test cases and store it to a List
      try
      {
        ICommand com = tdConn.Command as ICommand;
        string[] columnName = { "TS_TEST_ID", configFile.CaseFilter.TestSetName };

        string sqlCommand = GenerateSQLText(columnName);
        com.CommandText = sqlCommand;
        
        IRecordset recList = com.Execute() as IRecordset;        

        recList.First();
        for (int num = 0; num < recList.RecordCount; num++)
        {
          ItemValue tc = new ItemValue();
          tc.Test_ID = recList["TS_TEST_ID"].ToString();
          if (recList[configFile.CaseFilter.TestSetName] != null)
            tc.Test_Custom = recList[configFile.CaseFilter.TestSetName].ToString();
          tcList.Insert(0, tc);
          recList.Next();
        }
      }
      catch (Exception e)
      {
        AutoLog.Info("AutoIntSys: Exception with " + e.Message);
        AutoLog.Info("AutoIntSys: Fail to get Specified Field Value from TEST Table!");
        //Debug.Print("AutoIntSys:Fail to get Specified Field Value from TEST Table!");
        //Debug.Print("AutoIntSys:" + e);
        return null;
      }
      #endregion

      #region Generate TestSet
      try
      {
        TestSetFactory TSFact = TSFolder.TestSetFactory as TestSetFactory;
        TestFactory TestFact = tdConn.TestFactory as TestFactory;
        foreach (string TSName in TestSetNames)
        {
          TestSet TestSetName;
          TSTestFactory TSTestFact;

          TestSetName = TSFact.AddItem(TSName) as TestSet;
          TestSetName.Post();
          TSTestFact = TestSetName.TSTestFactory as TSTestFactory;

          foreach (ItemValue t in tcList)
          {
            if (t.Test_Custom == TSName)
              TSTestFact.AddItem(t.Test_ID);
          }        
        }
        
        createTSSucess = true;
        return TSFolder;
      }
      catch (Exception e)
      {
        AutoLog.Info("AutoIntSys: Exception with " + e.Message);
        AutoLog.Info("AutoIntSys: Create Test Sets Error!");
        //Debug.Print("AutoIntSys:Create Test Sets Error!");
        //Debug.Print("AutoIntSys:" + e.Message);
        return null;
      }
      #endregion

    }

    /*
      ////The filter can only filter the system fields
      //TDFilter TFilter = TestFact.Filter as TDFilter;
      ////All match item will be filtered by the provided field and value
      //foreach (TestConfigFile.AutorunFilerConfig filter in TConfig.CaseFilter.AutorunFilter)
      //{
      //  TFilter[filter.FieldName] = filter.FieldValue;
      //}
      //TFilter[TConfig.CaseFilter.TestSetName] = TSName;
      ////All matched test cases will be storaged in TList temporarily        
      //List TList = TFilter.NewList();

      ////Add test set name
    */

    private void RunCurrentSet(TestSet RunSet, string destMachine, bool isQTPRun, string configpath)
    {
      Console.Out.WriteLine("Enter the RunCurrentSet");
      if (isQTPRun)
      {
        #region Using QTP
        Console.Out.WriteLine("==========[void RunCurrentSet]Now something exception happen for QTP scheduler");
        //Console.Out.WriteLine("AutoIntSys: Begin to load QTP");
        //TSTestFactory TSTestFact = RunSet.TSTestFactory as TSTestFactory;
        //List runList = new List();
        //runList = TSTestFact.NewList("") as List;
        //if (runList.Count < 1)
        //  return;
        //TSScheduler Scheduler = RunSet.StartExecution("localhost") as TSScheduler;
        //if (destMachine == "localhost")
        //  Scheduler.RunAllLocally = true;
        //else
        //  Scheduler.TdHostName = destMachine;

        //try
        //{
        //  Debug.Print("AutoIntSys: Run Start at: " + TestUtility.GetCurrentTime());
        //  Scheduler.Run(runList);
        //}
        //catch (Exception e)
        //{
        //  Debug.Print("AutoIntSys: RUNNING Test Case Error!");
        //  Debug.Print("AutoIntSys: " + e.Message);
        //}

        //ExecutionStatus execStatus = Scheduler.ExecutionStatus as ExecutionStatus;

        //bool isRunFinished = false;
        //while (!isRunFinished)
        //{
        //  execStatus.RefreshExecStatusInfo(runList, true);
        //  isRunFinished = execStatus.Finished;
        //  System.Threading.Thread.Sleep(5000);
        //}
        //Debug.Print("AutoIntSys: Run Finish at: " + TestUtility.GetCurrentTime());
        //Console.WriteLine("Run Finish at: {0}", TestUtility.GetCurrentTime());
        #endregion
      }
      else
      {
        CustomerProcess.CustomerProcess cp = new CustomerProcess.CustomerProcess();
        if (cp.LoadRunInstance(".") > 0)
        { 
          AutoLog.Info("AutoIntSys: [void RunCurrentSet]Current Test set start run!");
          //Debug.Print("AutoIntSys: Run CP Start at: " + TestUtility.GetCurrentTime());
          //Console.Out.WriteLine("==========[void RunCurrentSet]Current Test set start: " + TestUtility.GetCurrentTime());
          AutoTestInterface.IRunTest runScheduler = cp.GetRunName(configFile.RunParameter.CustomerTestPro);
          runScheduler.Run(tdConn, RunSet, configpath);
          System.Threading.Thread.Sleep(5000);
          runScheduler.RunFinished();

          AutoLog.Info("AutoIntSys: [void RunCurrentSet]Current Test set end run!");
          //Debug.Print("AutoIntSys: Run CP Finish at: " + TestUtility.GetCurrentTime());
          //Console.Out.WriteLine("==========[void RunCurrentSet]Current Test set end: " + TestUtility.GetCurrentTime());
        }

        #region Unused
        /****

        // RunSet.ID
        // invoke the test execution
        TSTestFactory TSTestFact = RunSet.TSTestFactory as TSTestFactory;
        List runList = new List();
        runList = TSTestFact.NewList("") as List;
        foreach (TSTest instance in runList)
        {
          //generate the run first      
          RunFactory runFact = instance.RunFactory as RunFactory;
          DateTime now = TestUtility.GetCurrentTime();
          Run instanceRun = runFact.AddItem("Run_" + now.ToShortDateString() +
                                          "_" + now.ToShortTimeString()) as Run;         

          QCOperation.QCInformation info = new QCOperation.QCInformation();          
          // string runID = instanceRun.ID as string;
          //Initial the start status
          info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Not Completed");
          //Add the run steps
          //info.SetTestRunStep(tdConn, runID, "Step 1", "Passed");
          //info.SetTestRunStep(tdConn, runID, "Step 2", "Failed");
          //Update the end status
          //info.SetTestRunStatus(tdConn, runID, "Failed");

          //Download test case attachments
          string caseID = instance.TestId as string;
          ArrayList attachments = new ArrayList();
          QCOperation.QCAttachment taa = new QCOperation.QCAttachment();
          foreach (string downfile in configFile.RunParameter.DownloadFile)
          {
            attachments.Add(taa.DownloadAttachment(tdConn, "TEST", caseID, downfile, "C:\\temp"));
          }

          //When finish the test, record the summary in instance of testset
          string instanceID = instance.ID as string;
          string scriptFilename = null;
          string dataFilename = null;
          for(int i = 0; i < attachments.Count; i++)
          {
            ArrayList downList = attachments[i] as ArrayList;
            if (downList.Count > 0)
            {
              foreach (Object fileObj in downList)
              {
                string tempFilePath = fileObj as string;
                if (tempFilePath != null && tempFilePath.EndsWith("cod"))
                {
                  scriptFilename = tempFilePath;
                }
                if (tempFilePath != null && tempFilePath.EndsWith("iod"))
                {
                  dataFilename = tempFilePath;
                }
              }
            }
          }
          if (scriptFilename != null)
                {
                  PAS.AutoTest.ScriptRunner.ScriptRunner sr = new PAS.AutoTest.ScriptRunner.ScriptRunner();
            PAS.AutoTest.ScriptRunner.ExecuteResult er;
            if (dataFilename!=null)
            {  er = sr.Run(scriptFilename, dataFilename, 600); }
            else
            {  er = sr.Run(scriptFilename, string.Empty, 600); }
                  switch (er.Output.Result)
                  {
                    case PAS.AutoTest.TestData.TestResult.Pass:
                      info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Passed");
                      break;
                    case PAS.AutoTest.TestData.TestResult.Fail:
                      info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Failed");
                      break;
                    default:
                      info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Not Completed");
                      break;
                  }

                  info.SetTestInstanceSummary(tdConn, instanceID, configFile.RunParameter.NumOfPassed, er.Output.Summary.Passed.ToString());
                  info.SetTestInstanceSummary(tdConn, instanceID, configFile.RunParameter.NumOfTotal, er.Output.Summary.TotalRun.ToString());
                }
        }
         ****/
        #endregion
      
      }
    }

    class ItemValue
    {
      private string Test_IDField;
      private string Test_CustomField;

      public string Test_ID
      {
        get { return this.Test_IDField; }
        set { this.Test_IDField = value; }
      }
      public string Test_Custom
      {
        get { return this.Test_CustomField; }
        set { this.Test_CustomField = value; }
      }
    }
  }
}
