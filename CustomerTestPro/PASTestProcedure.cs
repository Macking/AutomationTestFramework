using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using AutoTestInterface;
using TDAPIOLELib;
using log4net;
using AutotestIntsys;

namespace CustomerTestPro
{
  public class PASTestProcedure : IRunTest
  {
    ILog AutoLog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public bool Run()
    {
      return false;
    }

    public bool Run(TDConnectionClass tdConn, TestSet RunSet, string configPath)
    {
      
      try
      {
        //Console.WriteLine("Test run from Interface of IRunTest!");
        AutoLog.Info("[CP Run]Test run from IRunTest: " + RunSet.Name);
        //Debug.Print("PAS: Test run from Interface of IRunTest!");
        //Console.Out.WriteLine("----------[CP Run]Test run from IRunTest: " + RunSet.Name);
        TestConfigFile tConfig = new TestConfigFile();
        bool success = false;
        //AutoLog.Info("[CP Run]Configuration file path: " + configPath);
        success = TestUtility.LoadConfigFile(ref tConfig, configPath);
        if(!success)
        {
          AutoLog.Info("[CP Run]Load Configuration file failed.");
          return true;
        }

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
          //AutoLog.Info("[CP Run]Config value of MacPath: " + tConfig.RunParameter.RemotePath);
          //AutoLog.Info("[CP Run]Config value of RunMac: " + tConfig.RunParameter.RunRemote.ToString());
          if(tConfig.RunParameter.RunRemote)
          {
            dataFilename = ReplaceStringInFile(dataFilename, tConfig.RunParameter.RemotePath);
          }

          if (scriptFilename != null)
          {
            PAS.AutoTest.ScriptRunner.ScriptRunner sr = new PAS.AutoTest.ScriptRunner.ScriptRunner();
            PAS.AutoTest.ScriptRunner.ExecuteResult er;
            //Debug.Print("PAS: Script file name: {0}", scriptFilename);
            //Debug.Print("PAS: Data file name: {0}", dataFilename);
            //Console.Out.WriteLine("----------[CP Run]Script file name: " + scriptFilename);
            AutoLog.Info("[CP Run]Script file name: " + scriptFilename);
            AutoLog.Info("[CP Run]Data file name: " + dataFilename);
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
                //Debug.Print("PAS: Upload test log sucess!");
                //Console.Out.WriteLine("----------[CP Run]Upload test log sucess!");
                AutoLog.Info("[CP Run]Upload test log success!");
              }
              else
              {
                //Debug.Print("PAS: Upload test log fail!");
                //Console.Out.WriteLine("----------[CP Run]Upload test log fail!");
                AutoLog.Info("[CP Run]Upload test log fail!");
              }
            }else
            {
              //Console.Out.WriteLine("----------[CP Run]Case run status is incomplete!");
              AutoLog.Info("[CP Run]Case run status is incomplete!");
            }
          }
        }
      }
      catch(Exception e)
      {
        //Debug.Print("PAS: Run test case error!");
        //Console.Out.WriteLine("PAS: Run test case error!");
        AutoLog.Info("[CP Run]Run test case exception: " + e.Message);
      }

      return true;
    }

    public bool RunFinished()
    {
      //Console.WriteLine("IRunTest run finished!");
      //Debug.Print("PAS: Test End from Interface of IRunTest!");
      AutoLog.Info("[CP Run]Run test case end from Interface of IRunTest!");
      return true;
    }

    private string ConvertChar(string str)
    {
      str = str.Replace("/", "_");
      str = str.Replace("-", "_");
      str = str.Replace(":", "_");
      str = str.Replace(" ", "_");
      //Debug.Print("PAS: Logname, " + str);
      AutoLog.Info("[CP Run]The log will be upload: " + str);
      return str;
    }

    private string GetCurrentRunDir()
    {
      string path = Process.GetCurrentProcess().MainModule.FileName;
      int p = path.LastIndexOf("\\");
      path = path.Substring(0, p + 1);
      return path;
    }

    private string ReplaceStringInFile(string file, string replaceString)
    {
      StringBuilder sb = new StringBuilder();
      using (StreamReader sr = new StreamReader(file))
      {
        string line;
        while ((line = sr.ReadLine()) != null) 
        {
          if (line.Contains("D:\\Test\\DICOM_Imag_Lib\\"))
          {
            line = line.Replace("D:\\Test\\DICOM_Imag_Lib\\", replaceString);
            line = line.Replace("\\", "/");
          }
          sb.AppendLine(line);
        }
      }

      string retDatafile = string.Empty;
      retDatafile = file.Remove(file.Length - 4);
      retDatafile = retDatafile + "mac.iod";
      using (StreamWriter sw = new StreamWriter(retDatafile))
      {
        sw.Write(sb.ToString());
        AutoLog.Info("[CP Run]Request data string: " + sb.ToString());
      }
      return retDatafile;
    }
  }
}
