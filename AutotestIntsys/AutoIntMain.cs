using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace AutotestIntsys
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      progArgs argClass = new progArgs();
      try
      {
        int argsNum = args.Length;
        string argType = "";
        string argContent = "";
        foreach (string ar in args)
        {
          argType = ar.Substring(1, 1);
          argContent = ar.Substring(3);
          switch (argType)
          {
            case "f":
              if (System.IO.File.Exists(argContent))
                argClass.configFile = argContent;                
              else
              {
                string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + argContent;
                //string appPath = System.IO.Directory.GetCurrentDirectory();
                //appPath += "\\" + argContent;
                if (System.IO.File.Exists(appPath))
                { argClass.configFile = appPath; }
                else
                {
                  Debug.Print("AutoIntSys: No config file be found!");
                  return;
                }
              }
              break;
            case "t":
              argClass.testFolder = argContent;
              break;
            case "m":
              argClass.targetMachine = argContent;
              break;
          }
        }
      }
      catch (Exception e)
      {
        Debug.Print("AutoIntSys:Parse arguments error!");
        Debug.Print("AutoIntSys:" + e.Message);
      }
      if (argClass.configFile == "" || argClass.configFile == null || argClass.testFolder == "" || argClass.testFolder == null)
      {
        //Test Machine can using the default value, so if not given from command line, it can still work.
        argClass.ShowHelp();
        return;
        //Not provide the correct parameters for AutoIntSys program, so display some hint and exit
      }

      //string instanceRunName = "Run_" + TestUtility.GetCurrentTime().ToShortDateString() + "_" + TestUtility.GetCurrentTime().ToShortTimeString();

      TestConfigFile tConfig = new TestConfigFile();
      bool success = false;
      //if(argClass.configFile!=null)
      success = TestUtility.LoadConfigFile(ref tConfig, argClass.configFile);
      //else
      //success = TestUtility.LoadConfigFile(ref tConfig, "TestConfig.xml");

      if (success)
      {
        QCOTAClass qcOnline = new QCOTAClass(tConfig);
        if (qcOnline.Connect())
        {
          /*
          QCOperation.QCAttachment taa = new QCOperation.QCAttachment();
          bool upSuccess;
          //upSuccess = (bool)taa.UploadAttachment(qcOnline.getTDConn(), "RUN", "6236", "test.log", "C:\\temp\\");
          upSuccess = (bool)taa.UploadAttachment(qcOnline.getTDConn(), "TEST", "44", "test.log", "C:\\temp\\");
          ArrayList attachments = new ArrayList();
          //attachments = (ArrayList)taa.DownloadAttachment(qcOnline.getTDConn(), "TEST", "C:\\temp", "1406");
          attachments = (ArrayList)taa.DownloadAttachment(qcOnline.getTDConn(), "Test", "1406", "Query_1406.xml", "C:\\temp");
          //attachments = (ArrayList)taa.DownloadAttachment(qcOnline.getTDConn(), "Test", "1406", "*.XML", "C:\\temp");
          */
          TestManage tm = new TestManage(qcOnline.getTDConn(), tConfig);

          bool creatTS = false;
          creatTS = tm.CreateTestSets(argClass.testFolder);
          if (creatTS)
            tm.RunTestSets(argClass.targetMachine);

          qcOnline.DisConnect();
          if (tConfig.RunParameter.UsingQTP)
            TestUtility.KillSpecifyProcess("QTPro");
        }
      }
    }//End of Main

    // Using SQL and ICommand to filter test cases
    /*
    try
    {
      ICommand com = tdConn.Command as ICommand;
      com.CommandText = "select * from test";
      IRecordset rec = com.Execute() as IRecordset;
      Console.WriteLine("Get record count: {0}", rec.RecordCount);
      rec.First();
      object myvalue = rec["TS_NAME"];
      Console.WriteLine(myvalue);
    }
    catch(Exception e) 
    {
      Console.WriteLine(" {0} ", e);
      Console.WriteLine("Fail to run query");
    }
    */

    /*
     * Using attachment download DLL to download test case's attachment
     * 
    */
    /*
    AttachmentActions.TDAttachmentAction taa = new AttachmentActions.TDAttachmentAction();

    ArrayList attachments = new ArrayList();
    attachments = (ArrayList)taa.GetTestAttachment(qcOnline.getTDConn(), "1406");
    attachments = (ArrayList)taa.GetAttachment(qcOnline.getTDConn(), "RUN", "6128");

    TestUtility.GetAttachmentFromTest(qcOnline.getTDConn(), "KDDCv2_QRU_QueryPatientInformation_Normal", "Query_1406.xml", "C:\\temp");
    TestUtility.GetAttachmentFromTest(qcOnline.getTDConn(), "Test1", "test.txt", "C:\\temp\\");          
    */
    //QCFileOperation.QCAttachment a = new QCFileOperation.QCAttachment();
    //a.getTestCaseAttachment(1,2);
    //TestManage.CreateTestSets(qcOnline, tConfig, "test c2");
    //The  value of testFolder will be passed from Daily Build Process
  }//End of Class Program
}
