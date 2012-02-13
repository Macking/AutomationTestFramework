using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class PresentationStateService:PASBase 
    {
        public PresentationStateService()
        {
            this.InitialService("PresentationState");
        }

        public XMLResult exportPresentationState(string presentationStateInternalID, string exportFormat, string exportType)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("exportPresentationState", new object[] { presentationStateInternalID, exportFormat, exportType }));
            return this.lastResult;
        }

        public XMLResult deletePresentationState(string presentationStateInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("deletePresentationState", new object[] { presentationStateInternalID }));
            return this.lastResult;
        }

        public XMLResult createPresentationState(string imageInternalID, XMLParameter presentationState)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createPresentationState", new object[] { imageInternalID, presentationState.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getPresentationState(string presentationStateInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getPresentationState", new object[] { presentationStateInternalID }));
            return this.lastResult;
        }

      public XMLResult getPresentationStateInfo(XMLParameterCollection presentationStateUidList)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getPresentationStateInfo", new object[] { presentationStateUidList.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult setPresentationState(XMLParameter presentationStateInfo,string presentationStateInternalID)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("setPresentationState", new object[] { presentationStateInfo.GenerateXML(), presentationStateInternalID }));
            return this.lastResult;
        }

        public XMLResult setPresentationStateInfo(XMLParameter indexInfo,string presentationStateInternalID)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("setPresentationStateInfo", new object[] { indexInfo.GenerateXML(), presentationStateInternalID }));
            return this.lastResult;
        }
    }
}
