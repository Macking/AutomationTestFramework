using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace AutotestIntsys
{
  public class progArgs
  {
    private string configFileField;
    private string testFolderField;
    private string targetMachineField;
    private string otherParametersField;

    public string configFile
    {
      get { return this.configFileField; }
      set { this.configFileField = value; }
    }

    public string testFolder
    {
      get { return this.testFolderField; }
      set { this.testFolderField = value; }
    }

    public string targetMachine
    {
      get { return this.targetMachineField; }
      set { this.targetMachineField = value; }
    }

    public string otherParameters
    {
      get { return this.otherParametersField; }
      set { this.otherParametersField = value; }
    }

    public void ShowHelp()
    {
      Console.WriteLine("Please enter the correct arguments!");
      Console.WriteLine("Usage:");
      Console.WriteLine("AutotestIntSys /f:ConfigFile /m:TestMachine /t:TestSetFolder");
      Console.WriteLine("/f:	Configuration File Name");
      Console.WriteLine("/m:	Test Machine Name");
      Console.WriteLine("/t:	TestSet Folder Path");
    }
  }

  public static class TestUtility
  {
    //Common function to generate Class from XML
    public static bool LoadConfigFile(ref TestConfigFile tcFile, string XMLfile)
    {
      try
      {
        XmlSerializer xs = new XmlSerializer(typeof(TestConfigFile));
        FileStream stream = new FileStream(XMLfile, FileMode.Open, FileAccess.Read);
        TestConfigFile tmpConfig = (TestConfigFile)xs.Deserialize(stream);
        stream.Close();
        tcFile = tmpConfig;
        return true;
      }
      catch (Exception e)
      {
        Debug.Print("AutoIntSys:---Load Config File Error---");
        Debug.Print("AutoIntSys:" + e.Message);        
      }
      return false;
    }
    
    /*
     * From test, the C# code can't use these method to download/upload files from QC
    //these codes need be verified
    private static string GetAttachmentServerPath(Test curTest, string AttachmentFileName,ref string LongFileName)
    {
      //string savePath = @"C:\temp";
      AttachmentFactory AFact = curTest.Attachments as AttachmentFactory;
      List AttachList = AFact.NewList("select * from cros_ref");
      foreach (Attachment attach in AttachList)
      {
        if (attach.get_Name(1) == AttachmentFileName)
        {
          LongFileName = attach.DirectLink; //attach.get_Name(0);
          
         // attach.Load(true, ref savePath);
          return attach.ServerFileName;
        }
      }
      return "";
    }

    private static string GetAttachmentFromTestObject(Test curTest, string AttachmentFileName, string OutputPath)
    {
      string LongFileName = ""; // = OutputPath + "\\" + AttachmentFileName;
      string myPath = GetAttachmentServerPath(curTest,AttachmentFileName,ref LongFileName);
      if (myPath == "")
        return "";
      if (OutputPath.LastIndexOf("\\") != OutputPath.Length - 1)
        OutputPath += "\\";
      ExtendedStorage exStorage = curTest.ExtendedStorage as ExtendedStorage;
      exStorage.ServerPath = myPath;
      Console.WriteLine("Server Path:{0}", exStorage.ServerPath);
      Console.WriteLine("Out Path:{0}", exStorage.ClientPath);
      exStorage.ClientPath = OutputPath;

      exStorage.Load("-r *.*", true);
      return LongFileName;
    }

    public static bool GetAttachmentFromTest(TDConnectionClass td, string TestCaseName, string AttachmentFileName, string OutputPath)
    {
      TestFactory TFact = td.TestFactory as TestFactory;
      List caseList = TFact.NewList("select * from test where TS_NAME = '" + TestCaseName + "'");
      foreach (Test t in caseList)
      {
        //bool isT = false;
        //List w = new List();
        //ExtendedStorage es = t.ExtendedStorage as ExtendedStorage;
        //es.ClientPath = "C:\\temp\\";
        //es.Load(AttachmentFileName, true);
        //string down = es.LoadEx("", true, out w, out isT);
        GetAttachmentFromTestObject(t, AttachmentFileName, OutputPath);
      }
      return true;
    }
    //Download functions should be polished
    * 
    */

    public static DateTime GetCurrentTime()
    {
      DateTime dt = DateTime.Now;
      return dt;
    }

    public static void WriteTestLog(string LogName, string ContentType, TextWriter LogContent)
    {
      Debug.Print("AutoIntSys:=========Test log start=========");
      try
      {
        using (StreamWriter w = File.AppendText(LogName))
        {
          w.Write("=========Test Log Start : ");
          w.WriteLine("{0} {1}=========", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
          w.WriteLine("  {0}  :", ContentType);
          w.WriteLine(" {0}", LogContent);
          w.Write("=========Test Log Stop : ");
          w.WriteLine("{0} {1}=========", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
          w.Flush();
          // Close the writer and underlying file.
          w.Close();
        }
        Debug.Print("AutoIntSys:" + LogContent);
        Debug.Print("AutoIntSys:=========Test log end=========");        
      }
      catch (Exception e)
      {
        Debug.Print("AutoIntSys:Test log write error!");
        Debug.Print("AutoIntSys:" + e);
      }
    }

    public static void WriteTestLog(string LogName, string ContentType, string LogContent)
    {
      Debug.Print("AutoIntSys:=========Test log start=========");      
      try
      {
        using (StreamWriter w = File.AppendText(LogName))
        {
          w.Write("=========Test Log Start : ");
          w.WriteLine("{0} {1}=========", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
          w.WriteLine("  {0}  :", ContentType);
          w.WriteLine(" {0}", LogContent);
          w.Write("=========Test Log Stop : ");
          w.WriteLine("{0} {1}=========", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
          w.Flush();
          // Close the writer and underlying file.
          w.Close();
        }
        Debug.Print("AutoIntSys:" + LogContent);
        Debug.Print("AutoIntSys:=========Test log end========="); 
      }
      catch (Exception e)
      {
        Debug.Print("AutoIntSys:Test log write error!");
        Debug.Print("AutoIntSys:" + e);
      }
    }

    public static void WriteTestLog(string LogName, string LogContent)
    {
      try
      {
        using (StreamWriter w = File.AppendText(LogName))
        {
          w.WriteLine("Passed / Total:");
          w.WriteLine("{0}", LogContent);
          w.Flush();
          w.Close();
        }
      }
      catch (Exception e)
      {
        Debug.Print("AutoIntSys:Test log write error!");
        Debug.Print("AutoIntSys:" + e);
      }
    }

    public static string GetCurrentRunDir()
    {
      string path = Process.GetCurrentProcess().MainModule.FileName;
      int p = path.LastIndexOf("\\");
      path = path.Substring(0, p + 1);
      return path;
    }

    public static bool KillSpecifyProcess(string ProcessName)
    {
      Process[] previusProcess = Process.GetProcessesByName(ProcessName);
      if (previusProcess.Length > 0)
      {
        foreach (Process p in previusProcess)
        { p.Kill(); }
      }
      Process[] lastProcess = Process.GetProcessesByName(ProcessName);
      if (lastProcess.Length == 0)
        return true;
      else
        return false;
    }

  }
}
