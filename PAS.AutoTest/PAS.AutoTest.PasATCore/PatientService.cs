using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
    public class PatientService : PASBase
    {
        public PatientService()
        {
            this.InitialService("Patient");
        }

        public XMLResult createPatient(XMLParameter patientInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createPatient", new object[] { patientInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult initFindPatient(XMLParameter filter)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("initFindPatient", new object[] { filter.GenerateXML() }));
            return lastResult;
        }

        public XMLResult findPatient(XMLParameter filter)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("findPatient", new object[] { filter.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getPatient(string patientInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getPatient", new object[] { patientInternalID }));
            return this.lastResult;
        }

        public XMLResult listObjects(XMLParameter filter)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listObjects", new object[] { filter.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult setPatient(XMLParameter patientInfo, string patientInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setPatient", new object[] { patientInfo.GenerateXML(), patientInternalID }));
            return this.lastResult;
        }

        public XMLResult deletePatient(string patientInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("deletePatient", new object[] { patientInternalID }));
            return this.lastResult;
        }

        public XMLResult execFindPatient(string sessionUid, int count, bool countSpecified)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("execFindPatient", new object[] { sessionUid, count, countSpecified }));
            return this.lastResult;
        }

        public XMLResult queryPatients(XMLParameter filter)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("queryPatients", new object[] { filter.GenerateXML() }));
            return this.lastResult;
        }
    }
}
