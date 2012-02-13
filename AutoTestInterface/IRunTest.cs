using System;
using System.Collections.Generic;
using System.Text;
using TDAPIOLELib;

namespace AutoTestInterface
{
  public interface IRunTest
  {
    bool Run();
    bool Run(TDConnectionClass tdConn, TestSet RunSet, string configPath);
    bool RunFinished();
  }
}
