using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;
using AutoTestInterface;
using TDAPIOLELib;

namespace CustomerTestPro
{
  public class PASTestProcedure : IRunTest
  {
    public bool Run()
    {
      return false;
    }

    public bool Run(TDConnectionClass tdConn, TestSet RunSet, string configPath)
    {
      try
      {
      //Console.WriteLine("Test run from Interface of IRunTest!");
      Debug.Print("PAS: Test run from Interface of IRunTest!");
      TestConfigFile tConfig = new TestConfigFile();
      TestUtility.LoadConfigFile(ref tConfig, configPath);


      // invoke the test execution
      TSTestFactory TSTestFact = RunSet.TSTestFactory as TSTestFactory;
      List runList = new List();
      runList = TSTestFact.NewList("") as List;
      foreach (TSTest instance in runList)
      {
        //generate the run first      
        RunFactory runFact = instance.RunFactory as RunFactory;
        DateTime now = DateTime.Now;
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
        foreach (string downfile in tConfig.RunParameter.DownloadFile)
        {
          attachments.Add(taa.DownloadAttachment(tdConn, "TEST", caseID, downfile, @"C:\CSAutoTest\Temp"));
        }

        //When finish the test, record the summary in instance of testset
        string instanceID = instance.ID as string;
        string scriptFilename = null;
        string dataFilename = null;
        for (int i = 0; i < attachments.Count; i++)
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
            Debug.Print("PAS: Script file name: {0}", scriptFilename);
            Debug.Print("PAS: Data file name: {0}", dataFilename);
          if (dataFilename != null)
            { er = sr.Run(scriptFilename, dataFilename, 1200); }
          else
            { er = sr.Run(scriptFilename, string.Empty, 1200); }
            switch (er.Result)
          {
              case PAS.AutoTest.TestData.TestResult.Done:
                info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Passed");
                break;
            case PAS.AutoTest.TestData.TestResult.Pass:
              info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Passed");
              break;
            case PAS.AutoTest.TestData.TestResult.Fail:
              info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Failed");
              break;
              case PAS.AutoTest.TestData.TestResult.Incomplete:
              info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "Not Completed");
              break;
                //default:
              case PAS.AutoTest.TestData.TestResult.Warning:
                info.SetTestRunStatus(tdConn, instanceRun.ID.ToString(), "N/A");
                break;

          }
            if (er.Result != PAS.AutoTest.TestData.TestResult.Incomplete)
            {
          info.SetTestInstanceSummary(tdConn, instanceID, tConfig.RunParameter.NumOfPassed, er.Output.Summary.Passed.ToString());
          info.SetTestInstanceSummary(tdConn, instanceID, tConfig.RunParameter.NumOfTotal, er.Output.Summary.TotalRun.ToString());
          
          string fileLog = ".log";
          fileLog = "TEST_" + caseID + "_" + now.ToShortDateString() + "_" + now.ToShortTimeString() + ".log";
          fileLog = ConvertChar(fileLog);
          er.Output.ConvertToXml(".\\Temp\\" + fileLog);
          bool uploadSuccess = false;
          uploadSuccess = taa.UploadAttachment(tdConn, "RUN", instanceRun.ID.ToString(), fileLog, GetCurrentRunDir() + "\\Temp\\");
          if (uploadSuccess)
          {
            Debug.Print("PAS: Upload test log sucess!");
          }
          else
          {
            Debug.Print("PAS: Upload test log fail!");
          }
        }
      }
        }
      }
      catch
      {
        Debug.Print("PAS: Run test case error!");
      }

      return true;
    }

    public bool RunFinished()
    {
      //Console.WriteLine("IRunTest run finished!");
      Debug.Print("PAS: Test end from Interface of IRunTest!");
      return true;
    }

    private string ConvertChar(string str)
    {
      str = str.Replace("/", "_");
      str = str.Replace("-", "_");
      str = str.Replace(":", "_");
      str = str.Replace(" ", "_");      
      Debug.Print("PAS: Logname, " + str);
      return str;
    }

    private string GetCurrentRunDir()
    {
      string path = Process.GetCurrentProcess().MainModule.FileName;
      int p = path.LastIndexOf("\\");
      path = path.Substring(0, p + 1);
      return path;
    }
  }
}
