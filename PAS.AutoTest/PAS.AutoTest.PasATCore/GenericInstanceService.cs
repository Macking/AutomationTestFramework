using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
  public class GenericInstanceService : PASBase
  {
    public GenericInstanceService()
    {
      this.InitialService("GenericInstance");
    }

    public XMLResult linkInstance(string parentInstanceUid, string childInstanceUid)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("linkInstance", new object[] { parentInstanceUid, childInstanceUid }));
      return this.lastResult;
    }

    public XMLResult listInstances(string parentInstanceUid, string childInstanceType)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("listInstances", new object[] { parentInstanceUid, childInstanceType }));
      return this.lastResult;
    }

    public XMLResult unlinkInstance(string parentInstanceUid, string childInstanceUid)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("unlinkInstance", new object[] { parentInstanceUid, childInstanceUid }));
      return this.lastResult;
    }
  }
}
