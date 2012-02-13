using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class AcquisitionService : PASBase
    {
        public AcquisitionService()
        {
            this.InitialService("Acquisition");
        }

        public XMLResult startAcquisition(XMLParameter acqInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("startAcquisition", new object[] { acqInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult queryLines(XMLParameter deviceIdList)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("queryLines", new object[] { deviceIdList.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult queryDevices(XMLParameter sensorTypeList)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("queryDevices", new object[] { sensorTypeList.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult setAsynAcqPatientInfo(XMLParameter asyncAcqPatientInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setAsynAcqPatientInfo", new object[] { asyncAcqPatientInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getAcquisitionResult(string acquisitionSessionID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getAcquisitionResult", new object[] { acquisitionSessionID }));
            return this.lastResult;
        }
    }
}
