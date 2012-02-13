using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
  public class RadiologService : PASBase
  {
    public RadiologService()
    {
      this.InitialService("Radiolog");
    }

    public XMLResult createRadioLogEntry(XMLParameter radioLogEntryInfoXml)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("createRadioLogEntry", new object[] { radioLogEntryInfoXml.GenerateXML() }));
      return this.lastResult;
    }

    public XMLResult getRadioLogEntry(string radioLogEntryInternalId)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("getRadioLogEntry", new object[] { radioLogEntryInternalId }));
      return this.lastResult;
    }

    public XMLResult setRadioLogEntry(XMLParameter radioLogEntryInfoXml, string radioLogEntryInternalId)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("setRadioLogEntry", new object[] { radioLogEntryInfoXml.GenerateXML(), radioLogEntryInternalId }));
      return this.lastResult;
    }

    public XMLResult deleteRadioLogEntry(string radioLogEntryInternalId)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("deleteRadioLogEntry", new object[] { radioLogEntryInternalId }));
      return this.lastResult;
    }

    public XMLResult initFindRadioLog(XMLParameter filter)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("initFindRadioLog", new object[] { filter.GenerateXML() }));
      return this.lastResult;
    }

    public XMLResult execFindRadioLog(int count, string sessionId)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("execFindRadioLog", new object[] { count, sessionId }));
      return this.lastResult;
    }

    public XMLResult exportRadioLog(XMLParameter filter)
    {
      this.lastResult = new XMLResult(this.InvokeMethod("exportRadioLog", new object[] { filter.GenerateXML() }));
      return this.lastResult;
    }
  }
}
