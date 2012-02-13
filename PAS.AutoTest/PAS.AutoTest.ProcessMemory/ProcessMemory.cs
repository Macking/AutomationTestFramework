using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.ProcessMemory
{
  public class ProcessMemory
  {
    //static List<Dictionary<string, string>> mUsage = new List<Dictionary<string,string>>();
    private MemMap memUsagepList;
    public class MemMap
    {
      public string derby = string.Empty;
      public string jetty = string.Empty;
      public string deamon = string.Empty;

      public void setType(string type)
      {
        switch (type)
        {
          case "client":            
              derby = "ClientDerby";
              jetty = "ClientJetty";
              deamon = "ClientDeamon";
              break;
          case "server":            
              derby = "ServerDerby";
              jetty = "ServerJetty";
              deamon = "ServerDeamon";
              break;
          default:
            break;
        }
      }
    }

    public ProcessMemory()
    {
      //mUsage = new List<Dictionary<string, string>>();
      memUsagepList = new MemMap();
    }

    /*
    public string getMemoryUsedByID(string processId)
    {
      Connection wmiConnection = new Connection();
      Win32_Process p = new Win32_Process(wmiConnection);
      IList<Dictionary<string,string>> mUsed = p.GetPropertyValues();
      //IDictionary<string, string> mUsed = p.GetPropertyValues();
      string memUse = string.Empty;
      foreach (Dictionary<string,string> processInfo in mUsed)
      {
        if (processInfo.ContainsValue(processId))
        {
          processInfo.TryGetValue("PageFileUsage", out memUse);
        }
        if (memUse != null && memUse != string.Empty)
          return memUse;
      }
      return null;

    }

    public string getMemoryUsedByName(string processName, string parentProcessName)
    {
      return null;
    }
    */

    public ProcessAndUsage getMemoryUsed(string mode)
    {
      ProcessAndUsage pau = new ProcessAndUsage();
      Connection wmiConnection;
      if (mode == "sa")
      {
        //get local machine memory usage information.
        wmiConnection = new Connection();
        memUsagepList.setType("server");
        Dictionary<string, string> memoryUsed = getProcessMemoryUsage(wmiConnection);
        string value = string.Empty;
        memoryUsed.TryGetValue(memUsagepList.derby, out value);
        pau.ServerDerby = value;
        memoryUsed.TryGetValue(memUsagepList.jetty, out value);
        pau.ServerJetty = value;
        memoryUsed.TryGetValue(memUsagepList.deamon, out value);
        pau.ServerDeamon = value;
      }
      else
      {
        //get local machine memory usage information.
        wmiConnection = new Connection();
        memUsagepList.setType("client");
        Dictionary<string, string> ClientMemoryUsed = getProcessMemoryUsage(wmiConnection);
        string value = string.Empty;
        ClientMemoryUsed.TryGetValue(memUsagepList.jetty, out value);
        pau.ClientJetty = value;
        ClientMemoryUsed.TryGetValue(memUsagepList.deamon, out value);
        pau.ClientDeamon = value;
        
        //get remote machine memory usage information.
        wmiConnection = new Connection("CsDataManager10", "CsDataManager!0", "cnshw76k3w1x", "cnshw76k3w1x");
        memUsagepList.setType("server");
        Dictionary<string, string> ServerMemoryUsed = getProcessMemoryUsage(wmiConnection);
        ServerMemoryUsed.TryGetValue(memUsagepList.derby, out value);
        pau.ServerDerby = value;
        ServerMemoryUsed.TryGetValue(memUsagepList.jetty, out value);
        pau.ServerJetty = value;
        ServerMemoryUsed.TryGetValue(memUsagepList.deamon, out value);
        pau.ServerDeamon = value;
      }
      //mUsage.Add(memoryUsed);
      return pau;
    }

    private Dictionary<string, string> getProcessMemoryUsage(Connection wmiConnection)
    {
      Win32_Process p = new Win32_Process(wmiConnection);
      IList<Dictionary<string, string>> mUsed = p.GetPropertyValues();
      //List<string> memoryUsed = new List<string>();
      Dictionary<string, string> memoryUsed = new Dictionary<string, string>();
      string memUse = string.Empty;
      string parentPID = string.Empty;
      foreach (Dictionary<string, string> processInfo in mUsed)
      {
        if (processInfo.ContainsValue("javaw.exe"))
        {
          processInfo.TryGetValue("ParentProcessId", out parentPID);
          if (parentPID != null && parentPID != string.Empty)
          {
            string parentPName = string.Empty;
            string pid = string.Empty;
            bool isCSDMProcess = false;
            foreach (Dictionary<string, string> processInfo1 in mUsed)
            {
              processInfo1.TryGetValue("Caption", out parentPName);
              processInfo1.TryGetValue("ProcessId", out pid);
              if (parentPName == "DM.exe" && pid == parentPID)
              {
                isCSDMProcess = true;
                break;
              }
            }
            if (isCSDMProcess)
              processInfo.TryGetValue("PageFileUsage", out memUse);
            if (memUse != string.Empty && isCSDMProcess)
            {
              string strCommandLine = string.Empty;
              processInfo.TryGetValue("CommandLine", out strCommandLine);
              if (strCommandLine.Contains("derbyrun.jar"))
                memoryUsed.Add(memUsagepList.derby, memUse);
              if (strCommandLine.Contains("pas_server") || strCommandLine.Contains("pas_client"))
                memoryUsed.Add(memUsagepList.jetty, memUse);
            }
          }
        }

        if (processInfo.ContainsValue("CSAcqDmn.exe"))
        {
          processInfo.TryGetValue("PageFileUsage", out memUse);
          if (memUse != string.Empty)
          {
            memoryUsed.Add(memUsagepList.deamon, memUse);
          }
        }
      }
      return memoryUsed;
    }
  }

  public class ProcessAndUsage
  {
    private string ClientJettyField = string.Empty;
    private string ClientDeamonField = string.Empty;
    private string ServerDerbyField = string.Empty;
    private string ServerJettyField = string.Empty;
    private string ServerDeamonField = string.Empty;

    public string ClientJetty
    {
      get { return this.ClientJettyField; }
      set { this.ClientJettyField = value; }
    }

    public string ClientDeamon
    {
      get { return this.ClientDeamonField; }
      set { this.ClientDeamonField = value; }
    }

    public string ServerDerby
    {
      get { return this.ServerDerbyField; }
      set { this.ServerDerbyField = value; }
    }

    public string ServerJetty
    {
      get { return this.ServerJettyField; }
      set { this.ServerJettyField = value; }
    }

    public string ServerDeamon
    {
      get { return this.ServerDeamonField; }
      set { this.ServerDeamonField = value; }
    }
  }
}
