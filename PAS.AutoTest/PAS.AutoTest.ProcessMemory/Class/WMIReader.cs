using System;
using System.Management;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.ProcessMemory
{
  class WMIReader
  {
        //public static IList<string> GetPropertyValues(Connection WMIConnection,
        //                                              string SelectQuery,
        //                                              string className)
        public static IList<Dictionary<string,string>> GetPropertyValues(Connection WMIConnection,
                                              string SelectQuery,
                                              string className)
        {
            ManagementScope connectionScope = WMIConnection.GetConnectionScope;
            //List<string> alProperties = new List<string>();
            SelectQuery msQuery = new SelectQuery(SelectQuery);
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);
            //Dictionary<string, string> paM = new Dictionary<string, string>();
            List<Dictionary<string, string>> allContent = new List<Dictionary<string, string>>();
            try
            {
                foreach (ManagementObject item in searchProcedure.Get())
                {
                    Dictionary<string, string> paM = new Dictionary<string, string>();
                    foreach (string property in XMLConfig.GetSettings(className))
                    {
                        try
                        {
                          paM.Add(property, item[property].ToString());
                          //alProperties.Add(property + ": " + item[property].ToString()); 
                        }
                        catch (SystemException) { /* ignore error */ }
                    }
                    allContent.Add(paM);                    
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }
            
            //return alProperties;
            return allContent;
        }
  }
}
