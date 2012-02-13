using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.ProcessMemory
{
  class Win32_Process : IWMI
  {
    Connection WMIConnection;

    public Win32_Process(Connection WMIConnection)
    {
      this.WMIConnection = WMIConnection;
    }
    //public IList<string> GetPropertyValues()
    public IList<Dictionary<string,string>> GetPropertyValues()
    {
      string className = System.Text.RegularExpressions.Regex.Match(
                            this.GetType().ToString(), "Win32_.*").Value;

      return WMIReader.GetPropertyValues(WMIConnection,
                                         "SELECT * FROM " + className,
                                         className);
    }
  }
}
