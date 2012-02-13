using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
  public class FMSService : PASBase
  {
    public FMSService()
    {
      this.InitialService("FMS");
    }

    public XMLResult createFMS(XMLParameter fmsInfo)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("createFMS", new object[] { fmsInfo.GenerateXML() }));
      return this.lastResult;
    }

    public XMLResult deleteFMS(string delImageFlag, string fmsInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("deleteFMS", new object[] { delImageFlag, fmsInternalID }));
      return this.lastResult;
    }

    public XMLResult findFMS(string presentationStateInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("findFMS", new object[] { presentationStateInternalID }));
      return this.lastResult;
    }

    public XMLResult getFMSDescription(string fmsInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("getFMSDescription", new object[] { fmsInternalID }));
      return this.lastResult;
    }

    public XMLResult getFMSInfo(string fmsInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("getFMSInfo", new object[] { fmsInternalID }));
      return this.lastResult;
    }

    public XMLResult listFMSPresentationState(string fmsInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("listFMSPresentationState", new object[] { fmsInternalID }));
      return this.lastResult;
    }

    public XMLResult setFMSDescription(XMLParameterCollection fmsDescription, string fmsInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("setFMSDescription", new object[] { fmsDescription.GenerateXML(), fmsInternalID }));
      return this.lastResult;
    }

    public XMLResult setFMSInfo(XMLParameter fmsInfo, string fmsInternalID)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("setFMSInfo", new object[] { fmsInfo.GenerateXML(), fmsInternalID }));
      return this.lastResult;
    }
  }
}
