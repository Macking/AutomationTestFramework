using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
  public class SimpleInstanceService : PASBase
  {
    public SimpleInstanceService()
    {
      this.InitialService("SimpleInstance");
    }

    public XMLResult createSimpleInstance(XMLParameter instanceInfo)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("createSimpleInstance", new object[] { instanceInfo.GenerateXML() }));
      return this.lastResult;
    }

    public XMLResult getSimpleInstance(string instanceInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("getSimpleInstance", new object[] { instanceInternalID }));
      return this.lastResult;
    }

      public XMLResult getInstanceInfo(string instanceInternalID)
      {
          this.lastResult = new XMLResult(this.InvokeMethod("getInstanceInfo", new object[] { instanceInternalID }));
          return this.lastResult;
      }

    public XMLResult deleteSimpleInstance(string instanceInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("deleteSimpleInstance", new object[] { instanceInternalID }));
      return this.lastResult;
    }
  }
}
