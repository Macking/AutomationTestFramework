using System;
using System.Collections.Generic;
using System.Text;
using PAS.AutoTest.TestUtility;

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

        #region Utility
        public static string AcquireImage(XMLParameter acq)
        {
            string psid = string.Empty;
            AcquisitionService acqs = new AcquisitionService();
            XMLResult rslAcqRVG = acqs.startAcquisition(acq);

            System.Threading.Thread.Sleep(1000);

            if (!rslAcqRVG.IsErrorOccured)
            {
                XMLResult getAcqRVG = new XMLResult();
                do
                {
                    System.Threading.Thread.Sleep(3000);
                    getAcqRVG = acqs.getAcquisitionResult(rslAcqRVG.SingleResult);
                    if (!getAcqRVG.IsErrorOccured)
                    {

                        // 2012-11-27/19006723: Change as the getAcquisitionResult return XML changes in Sprint 7

                        //int i = 0;
                        //for (i = 0; i < getAcqRVG.MultiResults[0].Parameters.Count; i++)
                        //{
                        //    if (getAcqRVG.MultiResults[0].Parameters[i].ParameterName == "presentation_state_internal_id")
                        //    {
                        //        psid = getAcqRVG.MultiResults[0].Parameters[i].ParameterValue;
                        //        break;
                        //    }
                        //}
                        //break;

                        PAS.AutoTest.TestUtility.ParseXMLContent parser = new PAS.AutoTest.TestUtility.ParseXMLContent(getAcqRVG.ResultContent); // To parse the return XML

                        string imageID = parser.getStringWithPathAndType("trophy/object_info", "image", "value");
                        string[] imageIDs = imageID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        int imageCount = imageIDs.Length;

                        string psID = parser.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                        string[] psIDs = psID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                        if (psIDs.Length >= 1)
                        {
                            psid = psIDs[0];
                            break;
                        }
                    }
                    if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                        continue;
                    if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                    {
                        break;
                    }
                } while (true);
            }
            return psid;
        }

        public static string AcquireFMS(XMLParameter acq)
        {
            string rt_fmsID = string.Empty;
            AcquisitionService acqs = new AcquisitionService();

            XMLResult rslAcqRVG = acqs.startAcquisition(acq);
            System.Threading.Thread.Sleep(3000);
            Utility.AcqFMS(40, 300);

            int DORVGcount = 0;
            XMLResult getAcqRVG = new XMLResult();
            do
            {
                System.Threading.Thread.Sleep(3000);
                System.Diagnostics.Debug.Print("get acquire in do");
                DORVGcount++;
                getAcqRVG = acqs.getAcquisitionResult(rslAcqRVG.SingleResult);
                if (!getAcqRVG.IsErrorOccured)
                {
                    ParseXMLContent parser = new ParseXMLContent(getAcqRVG.ResultContent); // To parse the return XML

                    string fmsID = parser.getStringWithPathAndType("trophy/object_info", "fms", "value");
                    string[] fmsIDs = fmsID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    int fmsCount = fmsIDs.Length;

                    string imageID = parser.getStringWithPathAndType("trophy/object_info", "image", "value");
                    string[] imageIDs = imageID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    int imageCount = imageIDs.Length;

                    string psID = parser.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                    string[] psIDs = psID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    int psCount = psIDs.Length;

                    if (fmsCount == 1 && imageCount >= 1 && psCount >= 1 && imageCount == psCount)
                    {
                        rt_fmsID = fmsIDs[0];
                    }
                    else
                    {
                        rt_fmsID = "";
                    }
                    break;
                }
                if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                {
                    continue;
                }
                if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                {
                    rt_fmsID = string.Empty;
                }
                System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                if (DORVGcount > 60)
                {
                    rt_fmsID = string.Empty;
                    break;
                }
            } while (true);

            return rt_fmsID;
        }

        #endregion
    }
}
