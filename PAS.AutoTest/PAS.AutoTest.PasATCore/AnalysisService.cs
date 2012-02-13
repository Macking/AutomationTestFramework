using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class AnalysisService : PASBase
    {
        public AnalysisService()
        {
            this.InitialService("Analysis");
        }

      public XMLResult createAnalysis(string analysisXml, bool current, bool currentSpecified, string patientInternalID, string thumbnail, XMLParameterCollection uidsXml)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createAnalysis", new object[] { analysisXml, current, currentSpecified, patientInternalID, thumbnail, uidsXml.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult deleteAnalysis(string analysisInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("deleteAnalysis", new object[] { analysisInternalID }));
            return this.lastResult;
        }

        public XMLResult exportAnalysis(string analysisInternalID, string exportFormat, string exportType)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("exportAnalysis", new object[] { analysisInternalID, exportFormat, exportType }));
            return this.lastResult;
        }

        public XMLResult getAnalysisDescription(string analysisInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getAnalysisDescription", new object[] { analysisInternalID }));
            return this.lastResult;
        }

        public XMLResult getAnalysisInfo(string analysisInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getAnalysisInfo", new object[] { analysisInternalID }));
            return this.lastResult;
        }

        public XMLResult listAnalysisObjects(string analysisInternalID)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("listAnalysisObjects", new object[] { analysisInternalID }));
            return this.lastResult;
        }

        public XMLResult setAnalysisDescription(string analysisInternalID, string analysisXml, bool current, XMLParameterCollection uidsXml)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setAnalysisDescription", new object[] { analysisInternalID, analysisXml, current, true, uidsXml.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult setAnalysisInfo(string analysisInternalID, XMLParameter indexedInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setAnalysisInfo", new object[] { analysisInternalID, indexedInfo.GenerateXML() }));
            return this.lastResult;
        }
    }
}