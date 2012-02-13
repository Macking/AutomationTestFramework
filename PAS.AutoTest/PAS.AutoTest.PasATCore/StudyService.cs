using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class StudyService:PASBase
    {
        public StudyService()
        {
            this.InitialService("Study");
        }

        public XMLResult createStudy(XMLParameter studyInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createStudy", new object[] { studyInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getStudy(string studyInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getStudy", new object[] { studyInternalID }));
            return this.lastResult;
        }

        public XMLResult setStudy(string studyInternalID,XMLParameter studyInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setStudy", new object[] {studyInternalID, studyInfo.GenerateXML() }));
            return this.lastResult;
        }
    }
}
