using System;
using System.Collections.Generic;
using System.Text;
using PAS.AutoTest.PasATCore;
using PAS.AutoTest.PasATCoreV2;
using PAS.AutoTest.TestData;
using PAS.AutoTest.TestUtility;

namespace PAS.AutoTest.TestCase
{
    public partial class Runner
    {
        public void Run_FMS_SetFMSIndexedInfo_Normal_Case157() //Case 157: 1.3.8_SetFMSIndexedInfo_Normal
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();


                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter FmsInfo = new XMLParameter("fms");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "FMSInfo")
                    {
                        FmsInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(22, 60);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", getAcqRVG.ResultContent);

                                string FMSInternalID = fmsIDs[0];

                                CheckPoint pFMS = new CheckPoint("Set FMS Info", "FMS Service");
                                r.CheckPoints.Add(pFMS);
                                XMLResult rslSetFmsInfo = fmss.setFMSInfo(FmsInfo, FMSInternalID);
                                if (rslSetFmsInfo.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Set FMS Result", "FMS Service", rslSetFmsInfo.ResultContent);
                                }

                                XMLResult rslGetFmsInfo = fmss.getFMSInfo(FMSInternalID);
                                if (rslGetFmsInfo.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Get FMS Info Fail", "FMS Service", rslGetFmsInfo.ResultContent);
                                }
                                else
                                {
                                    pFMS.Outputs.AddParameter("Get FMS Info Result", "FMS Service", rslGetFmsInfo.ResultContent);
                                    int matchGetItem = 0;
                                    for (int setI = 0; setI < FmsInfo.Length; setI++)
                                    {
                                        for (int getI = 0; getI < rslGetFmsInfo.MultiResults[0].Parameters.Count; getI++)
                                        {
                                            if (rslGetFmsInfo.MultiResults[0].Parameters[getI].ParameterName == FmsInfo.Parameters[setI].ParameterName
                                              && rslGetFmsInfo.MultiResults[0].Parameters[getI].ParameterValue == FmsInfo.Parameters[setI].ParameterValue)
                                                matchGetItem++;
                                        }
                                    }
                                    if (matchGetItem == FmsInfo.Length)
                                    {
                                        pFMS.Result = TestResult.Pass;
                                        pFMS.Outputs.AddParameter("Get FMS Info Result Match Setting", "FMS Service", "Success");
                                    }
                                    else
                                    {
                                        pFMS.Result = TestResult.Fail;
                                        pFMS.Outputs.AddParameter("Get FMS Info Result Not Match Setting", "FMS Service", "Failure");
                                    }

                                }

                                XMLResult delFmsRls = fmss.deleteFMS("true", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.Message);
                                    pFMS.Outputs.AddParameter("Delete FMS", "FMS Service", "Delete FMS Error");
                                    break;
                                }
                                pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.ResultContent);
                                pFMS.Outputs.AddParameter("Delete FMS and images", "FMS Service", "All images and fms has been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition return error.", getAcqRVG.ResultContent);
                            }

                            break;
                        }

                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                        {
                            continue;
                        }
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_GetFMSIndexedInfo_Normal_Case158() //Case 158: 1.3.8_GetFMSIndexedInfo_Normal
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();


                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter FmsInfo = new XMLParameter("fms");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "FMSInfo")
                    {
                        FmsInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(22, 60);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", getAcqRVG.ResultContent);

                                string FMSInternalID = fmsIDs[0];

                                CheckPoint pFMS = new CheckPoint("Set FMS Info", "FMS Service");
                                r.CheckPoints.Add(pFMS);
                                XMLResult rslSetFmsInfo = fmss.setFMSInfo(FmsInfo, FMSInternalID);
                                if (rslSetFmsInfo.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Set FMS Result", "FMS Service", rslSetFmsInfo.ResultContent);
                                }

                                XMLResult rslGetFmsInfo = fmss.getFMSInfo(FMSInternalID);
                                if (rslGetFmsInfo.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Get FMS Info Fail", "FMS Service", rslGetFmsInfo.ResultContent);
                                }
                                else
                                {
                                    pFMS.Result = TestResult.Pass;

                                    pFMS.Outputs.AddParameter("Get FMS Info Result", "FMS Service", rslGetFmsInfo.ResultContent);
                                    int matchGetItem = 0;
                                    for (int setI = 0; setI < FmsInfo.Length; setI++)
                                    {
                                        for (int getI = 0; getI < rslGetFmsInfo.MultiResults[0].Parameters.Count; getI++)
                                        {
                                            if (rslGetFmsInfo.MultiResults[0].Parameters[getI].ParameterName == FmsInfo.Parameters[setI].ParameterName
                                              && rslGetFmsInfo.MultiResults[0].Parameters[getI].ParameterValue == FmsInfo.Parameters[setI].ParameterValue)
                                                matchGetItem++;
                                        }
                                    }
                                    if (matchGetItem == FmsInfo.Length)
                                    {
                                        pFMS.Result = TestResult.Pass;
                                        pFMS.Outputs.AddParameter("Get FMS Info Result Match Setting", "FMS Service", "Success");
                                    }
                                    else
                                    {
                                        pFMS.Result = TestResult.Fail;
                                        pFMS.Outputs.AddParameter("Get FMS Info Result Not Match Setting", "FMS Service", "Failure");
                                    }

                                }

                                XMLResult delFmsRls = fmss.deleteFMS("true", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.Message);
                                    pFMS.Outputs.AddParameter("Delete FMS", "FMS Service", "Delete FMS Error");
                                    break;
                                }
                                pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.ResultContent);
                                pFMS.Outputs.AddParameter("Delete FMS and images", "FMS Service", "All images and fms has been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "FMS Service", "No Image or PS id return");
                            }
                            break;
                        }

                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_DeleteFMS_Normal_Case159() //Case 159: 1.3.8_1.3.8_DeleteFMS_Normal
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();


                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(15, 300); //can not set too long, otherwise there may be overtaken images
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                string FMSInternalID = fmsIDs[0];
                                XMLResult delFmsRls = fmss.deleteFMS("true", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Delete FMS Result", "Acquisition", delFmsRls.ResultContent);
                                    pAcquire.Outputs.AddParameter("Delete FMS", "Acquisition", "Delete FMS Error");
                                    break;
                                }
                                pAcquire.Outputs.AddParameter("Delete FMS Result", "Acquisition", delFmsRls.ResultContent);

                                XMLResult getFmsRls = fmss.getFMSDescription(FMSInternalID);
                                if (!getFmsRls.IsErrorOccured)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", getFmsRls.ResultContent);
                                    pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", "FMS does not delete");
                                    break;
                                }
                                pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", getFmsRls.ResultContent);


                                bool isImageExisted = false;
                                for (int i = 1; i <= imageIDs.Length; i++)
                                {
                                    XMLResult getImageRls = igs.getImageDescription(imageIDs[i - 1]);
                                    if (!getImageRls.IsErrorOccured)
                                    {
                                        isImageExisted = true;
                                        break;
                                    }
                                    pAcquire.Outputs.AddParameter("Get FMS Images: " + i, "Acquisition", getImageRls.ResultContent);
                                }
                                if (isImageExisted)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Get FMS Images", "Acquisition", "Images is not deleted from FMS");
                                    break;
                                }

                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("Delete FMS and images", "Acquisition", "Fms and imahes have been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", "No Image or PS id return");
                            }
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                        }
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_FindFMS_Normal_Case160() //Case 160: 1.3.8_FindFMS_Normal
        {
            int runCount = 0;
            string findPSid = string.Empty;
            string expectFmsId = string.Empty;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition");

                CheckPoint pFMS = new CheckPoint("Find PS in FMS", "Find FMS");
                r.CheckPoints.Add(pFMS);

                FMSService fmss = new FMSService();

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "presentationstate")
                    {
                        findPSid = ids.InputParameters.GetParameter(i).Value;
                    }
                }

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "fms")
                    {
                        expectFmsId = ids.ExpectedValues.GetParameter(i).Value;
                    }
                }

                XMLResult rslFindFms = fmss.findFMS(findPSid);

                if (rslFindFms.IsErrorOccured)
                {
                    pFMS.Result = TestResult.Fail;
                    pFMS.Outputs.AddParameter("Find FMS Result", "FMS Service", rslFindFms.Message);
                    pFMS.Outputs.AddParameter("Find FMS", "FMS Service", "Find FMS Return Error");
                    break;
                }
                else
                {
                    try
                    {

                        if (rslFindFms.MultiResults[0].Parameters[0].ParameterName == "internal_id"
                          && rslFindFms.MultiResults[0].Parameters[0].ParameterValue == expectFmsId)
                        {
                            pFMS.Result = TestResult.Pass;
                            pFMS.Outputs.AddParameter("Find FMS Result", "FMS Service", "Find the correct fms id");
                        }
                        else
                        {
                            pFMS.Result = TestResult.Fail;
                            pFMS.Outputs.AddParameter("Find FMS Result", "FMS Service", "Can't find the correct fms");
                        }
                    }
                    catch
                    {
                        pFMS.Result = TestResult.Fail;
                        pFMS.Outputs.AddParameter("Find FMS Result", "FMS Service", "Can't find the correct fms");
                    }
                }

                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_GetFMSDescription_Normal_Case164() //Case 164: 1.3.8_GetFMSDescription_Normal
        {

            int runCount = 0;
            string acquireType = string.Empty;
            string setFmsDescriptionValue = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameterCollection FmsDescColl = new XMLParameterCollection();
                XMLParameter FmsDesc = new XMLParameter("fms");
                XMLParameter FmsPs = new XMLParameter("presentationstate");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "FMSDesc")
                    {
                        FmsDesc.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setFmsDescriptionValue = ids.InputParameters.GetParameter(i).Value.Replace("\r", "");
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(22, 60);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                pAcquire.Result = TestResult.Pass;

                                FmsDescColl.Add(FmsDesc);
                                FmsDescColl.Add(FmsPs);

                                CheckPoint pFMS = new CheckPoint("Set FMS Description", "FMS Service");
                                r.CheckPoints.Add(pFMS);

                                string FMSInternalID = fmsIDs[0];
                                XMLResult rslSetFmsDesc = fmss.setFMSDescription(FmsDescColl, FMSInternalID);

                                if (rslSetFmsDesc.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Set FMS Result", "FMS Service", rslSetFmsDesc.ResultContent);
                                }

                                XMLResult rslGetFmsDesc = fmss.getFMSDescription(FMSInternalID);
                                if (rslGetFmsDesc.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Get FMS Description Fail", "FMS Service", rslGetFmsDesc.ResultContent);
                                }
                                else
                                {
                                    for (int rslCount = 0; rslCount < rslGetFmsDesc.MultiResults.Count; rslCount++)
                                    {
                                        if (rslGetFmsDesc.MultiResults[rslCount].Name == "fms")
                                        {
                                            if (rslGetFmsDesc.MultiResults[rslCount].Parameters[0].ParameterName == "xml_description"
                                              && rslGetFmsDesc.MultiResults[rslCount].Parameters[0].ParameterValue == setFmsDescriptionValue)
                                            {
                                                pFMS.Result = TestResult.Pass;
                                                pFMS.Outputs.AddParameter("Get FMS Description Success", "FMS Service", "Match the setting value");
                                            }
                                        }

                                        if (rslGetFmsDesc.MultiResults[rslCount].Name == "presentationstate")
                                        {
                                            for (int i = 0; i < rslGetFmsDesc.MultiResults[rslCount].Parameters.Count; i++)
                                            {
                                                if (!psID.Contains(rslGetFmsDesc.MultiResults[rslCount].Parameters[i].ParameterValue))
                                                {
                                                    pFMS.Result = TestResult.Fail;
                                                    pFMS.Outputs.AddParameter("Get FMS Description Success", "FMS Service", "Setting PSID is not match");
                                                    break;
                                                }
                                            }
                                        }

                                    }//Check the return value for fms section and ps section
                                }//Get FMS Description end

                                XMLResult delFmsRls = fmss.deleteFMS("true", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.Message);
                                    pFMS.Outputs.AddParameter("Delete FMS", "FMS Service", "Delete FMS Error");
                                    break;
                                }

                                pFMS.Result = TestResult.Pass;
                                pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.ResultContent);
                                pFMS.Outputs.AddParameter("Delete FMS and images", "FMS Service", "All images and fms has been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "FMS Service", "No Image or PS id return");
                            }
                            break;
                        }

                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_SetFMSDescription_Normal_Case165() //Case 165: 1.3.8_SetFMSDescription_Normal
        {
            int runCount = 0;
            string acquireType = string.Empty;
            string setFmsDescriptionValue = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameterCollection FmsDescColl = new XMLParameterCollection();
                XMLParameter FmsDesc = new XMLParameter("fms");
                XMLParameter FmsPs = new XMLParameter("presentationstate");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "FMSDesc")
                    {
                        FmsDesc.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setFmsDescriptionValue = ids.InputParameters.GetParameter(i).Value.Replace("\r", "");
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(22, 60);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                pAcquire.Result = TestResult.Pass;

                                FmsDescColl.Add(FmsDesc);
                                FmsDescColl.Add(FmsPs);

                                CheckPoint pFMS = new CheckPoint("Set FMS Description", "FMS Service");
                                r.CheckPoints.Add(pFMS);

                                string FMSInternalID = fmsIDs[0];
                                XMLResult rslSetFmsDesc = fmss.setFMSDescription(FmsDescColl, FMSInternalID);

                                if (rslSetFmsDesc.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Set FMS Result", "FMS Service", rslSetFmsDesc.ResultContent);
                                }

                                XMLResult rslGetFmsDesc = fmss.getFMSDescription(FMSInternalID);
                                if (rslGetFmsDesc.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Get FMS Description Fail", "FMS Service", rslGetFmsDesc.ResultContent);
                                }
                                else
                                {
                                    for (int rslCount = 0; rslCount < rslGetFmsDesc.MultiResults.Count; rslCount++)
                                    {
                                        if (rslGetFmsDesc.MultiResults[rslCount].Name == "fms")
                                        {
                                            if (rslGetFmsDesc.MultiResults[rslCount].Parameters[0].ParameterName == "xml_description"
                                              && rslGetFmsDesc.MultiResults[rslCount].Parameters[0].ParameterValue.Trim() == setFmsDescriptionValue.Trim())
                                            {
                                                pFMS.Result = TestResult.Pass;
                                                pFMS.Outputs.AddParameter("Get FMS Description Success", "FMS Service", "Match the setting value");
                                            }
                                            else
                                            {
                                                pFMS.Result = TestResult.Fail;
                                                pFMS.Outputs.AddParameter("Get FMS Description Success", "FMS Service", "Not match the setting value. Get: " + rslGetFmsDesc.ResultContent);
                                                break;
                                            }
                                        }

                                        if (rslGetFmsDesc.MultiResults[rslCount].Name == "presentationstate")
                                        {
                                            for (int i = 0; psCount < rslGetFmsDesc.MultiResults[rslCount].Parameters.Count; i++)
                                            {
                                                if (!psID.Contains(rslGetFmsDesc.MultiResults[rslCount].Parameters[i].ParameterValue))
                                                {
                                                    pFMS.Result = TestResult.Fail;
                                                    pFMS.Outputs.AddParameter("Get FMS Description Success", "FMS Service", "Setting PSID is not match");
                                                    break;
                                                }
                                            }
                                        }

                                    }//Check the return value for fms section and ps section
                                }//Get FMS Description end

                                XMLResult delFmsRls = fmss.deleteFMS("true", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pFMS.Result = TestResult.Fail;
                                    pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.Message);
                                    pFMS.Outputs.AddParameter("Delete FMS", "FMS Service", "Delete FMS Error");
                                    break;
                                }
                                pFMS.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.ResultContent);

                                pFMS.Outputs.AddParameter("Delete FMS and images", "FMS Service", "All images and fms has been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "FMS Service", "No Image or PS id return");
                            }

                            break;
                        }

                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_ListFMSPresentationState_Normal_Case230() //Case 230: 1.3.8_ListFMSPresentationState_Normal
        {
            /** Using service: AcquisitionService,FMSService
            * Using interface:
            *   AcquisitionService::startAcquisition
            *   AcquisitionService::getAcquisitionResult
            *	  FMSService::listFMSPresentationState
            *	  FMSService::deleteFMS
            **/
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();

                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(1200);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(22, 120);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                string FMSInternalID = fmsIDs[0];
                                XMLResult rslListPs = fmss.listFMSPresentationState(FMSInternalID);
                                int matchItem = 0;
                                for (int p = 0; p < rslListPs.MultiResults[0].Parameters.Count; p++)
                                {
                                    if (psID.Contains(rslListPs.MultiResults[0].Parameters[p].ParameterValue))
                                        matchItem++;
                                }
                                if (matchItem == psCount)
                                {
                                    pAcquire.Outputs.AddParameter("List FMS presentationstate", "FMS Service", "Get PS from listFMSPresentationState and match");
                                }
                                else
                                {
                                    pAcquire.Outputs.AddParameter("List FMS presentationstate", "FMS Service", "Get PS from listFMSPresentationState but not match");
                                }

                                XMLResult delFmsRls = fmss.deleteFMS("true", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.Message);
                                    pAcquire.Outputs.AddParameter("Delete FMS", "FMS Service", "Delete FMS Error");
                                    break;
                                }
                                pAcquire.Outputs.AddParameter("Delete FMS Result", "FMS Service", delFmsRls.ResultContent);

                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("Delete FMS and images", "FMS Service", "All images and fms has been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "FMS Service", "No Image or PS id return");
                            }
                            break;
                        }

                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_DeleteFMS_DeleteFMSOnly_Case1121() //Case 1121: 1.3.8_DeleteFMS_N1_Delete FMS only
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();


                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(40, 300);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                string FMSInternalID = fmsIDs[0];
                                XMLResult delFmsRls = fmss.deleteFMS("false", FMSInternalID);
                                if (delFmsRls.IsErrorOccured)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Delete FMS Result", "Acquisition", delFmsRls.ResultContent);
                                    pAcquire.Outputs.AddParameter("Delete FMS", "Acquisition", "Delete FMS Error");
                                    break;
                                }
                                pAcquire.Outputs.AddParameter("Delete FMS Result", "Acquisition", delFmsRls.ResultContent);

                                XMLResult getFmsRls = fmss.getFMSDescription(FMSInternalID);
                                if (!getFmsRls.IsErrorOccured)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", getFmsRls.ResultContent);
                                    pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", "FMS does not delete");
                                    break;
                                }
                                pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", getFmsRls.ResultContent);

                                bool isImageExisted = true;
                                for (int i = 1; i <= imageCount; i++)
                                {
                                    XMLResult getImageRls = igs.getImageDescription(imageIDs[i - 1]);
                                    if (getImageRls.IsErrorOccured)
                                    {
                                        isImageExisted = false;
                                        break;
                                    }
                                    pAcquire.Outputs.AddParameter("Get FMS Images: " + i, "Acquisition", getImageRls.ResultContent);
                                }
                                if (!isImageExisted)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Get FMS Images", "Acquisition", "Images is deleted from FMS wrongly");
                                    break;
                                }

                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("Delete FMS and images", "Acquisition", "Fms has been deleted and images are not");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", "No Image or PS id return");
                            }
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                        }
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_FMS_DeleteFMS_DeleteImageInFMS_Case1122() //Case 1122: 1.3.8_DeleteFMS_N2_Delete images from FMS
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                FMSService fmss = new FMSService();
                ImageService igs = new ImageService();
                PresentationStateService pss = new PresentationStateService();


                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition FMS");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        Utility.AcqFMS(40, 300);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
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
                                XMLParameter delImagesFlag = new XMLParameter("preferences");
                                delImagesFlag.AddParameter("completedFlag", "true");
                                bool delImageSuccess = true;
                                for (int i = 1; i <= imageCount; i++)
                                {
                                    XMLResult delImageRls = igs.deleteImage(imageIDs[i - 1], delImagesFlag);
                                    if (delImageRls.IsErrorOccured)
                                        delImageSuccess = false;
                                    pAcquire.Outputs.AddParameter("Delete FMS Image: " + i, "Acquisition", delImageRls.ResultContent);
                                }
                                if (!delImageSuccess)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Delete FMS Images", "Acquisition", "Delete FMS Images Error");
                                    break;
                                }

                                bool getImageFail = true;
                                for (int i = 1; i <= imageCount; i++)
                                {
                                    XMLResult getImageRls = igs.getImageDescription(imageIDs[i - 1]);
                                    if (!getImageRls.IsErrorOccured)
                                        getImageFail = false;
                                    pAcquire.Outputs.AddParameter("Get FMS Images: " + i, "Acquisition", getImageRls.ResultContent);
                                }
                                if (!getImageFail)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Get FMS Images", "Acquisition", "Images do not delete from FMS");
                                    break;
                                }

                                string FMSInternalID = fmsIDs[0];
                                XMLResult getFmsRls = fmss.getFMSDescription(FMSInternalID);
                                if (getFmsRls.IsErrorOccured)
                                {
                                    pAcquire.Result = TestResult.Fail;
                                    pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", getFmsRls.ResultContent);
                                    pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", "FMS has been deleted");
                                    break;
                                }
                                pAcquire.Outputs.AddParameter("Get FMS", "Acquisition", getFmsRls.ResultContent);

                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("Delete FMS and images", "Acquisition", "FMS'images have been deleted");
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", "No Image or PS id return");
                            }
                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                            continue;
                        if (getAcqRVG.Code != 0 && getAcqRVG.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", getAcqRVG.Message);
                        }
                        System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                        if (DORVGcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("Get acquire image error", "getAcquisitionResult", "Acquire great with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }
                SaveRound(r);
            }
            Output();
        }
    }
}