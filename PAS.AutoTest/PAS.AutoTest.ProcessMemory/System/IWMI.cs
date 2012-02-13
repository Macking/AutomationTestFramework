using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.ProcessMemory
{
  interface IWMI
  {
    //IList<string> GetPropertyValues();
    IList<Dictionary<string,string>> GetPropertyValues();
    //IDictionary<string,string> GetPropertyValues();
  }
}
