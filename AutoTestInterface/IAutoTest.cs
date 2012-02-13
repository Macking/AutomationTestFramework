using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTestInterface
{
  public interface IAutoTest
  {
    List<IRunTest> RunInstance { get; }
    int LoadRunInstance(string dllPath);
    IRunTest GetRunName(string name);
  }
}
