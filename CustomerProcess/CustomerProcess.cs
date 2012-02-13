using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using AutoTestInterface;

namespace CustomerProcess
{
  public class CustomerProcess : IAutoTest
  {
    private List<IRunTest> runInstances = new List<IRunTest>();

    #region IAutoTest Members

    public List<IRunTest> RunInstance
    {
      get { return runInstances; }
    }

    public int LoadRunInstance(string dllPath)
    {
      try
      {
      string[] assemblyFiles = Directory.GetFiles(dllPath, "*.dll");
        //string dllFile = string.Empty;
        Assembly assm;
      foreach (string file in assemblyFiles)
      {
          try
          {
            assm = Assembly.LoadFrom(file);
          }
          catch(Exception ap)
          {
            System.Diagnostics.Debug.Print("AutoIntSys: Can't Load dll " + file.Substring(2));
            continue;
          }
          //dllFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + file.Substring(1);
          //Assembly assm = Assembly.LoadFile(dllFile);
        foreach(Type t in assm.GetExportedTypes())
        {
          if (t.IsClass && typeof(IRunTest).IsAssignableFrom(t))
          {
            IRunTest plug = Activator.CreateInstance(t) as IRunTest;
            runInstances.Add(plug);
          }
        }
      }
        System.Diagnostics.Debug.Print("AutoIntSys: Load run instances " + runInstances.Count);
      return runInstances.Count;
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.Print("AutoIntSys: Load dll failure");
        return -1;
      }

    }

    public IRunTest GetRunName(string name)
    {
      foreach (IRunTest item in runInstances)
      {
        if (item.GetType().ToString() == name)
          return item;
      }
      return null;
    }

    #endregion
  }
}
