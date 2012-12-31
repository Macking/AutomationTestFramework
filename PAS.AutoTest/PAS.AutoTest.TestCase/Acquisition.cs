using System;
using System.Collections.Generic;
using System.Text;
using PAS.AutoTest.PasATCore;
using PAS.AutoTest.PasATCoreV2;
using PAS.AutoTest.TestData;
using PAS.AutoTest.TestUtility;
using NotificationSim;

namespace PAS.AutoTest.TestCase
{
    public partial class Runner
    {
        public void Run_Acquisition_RotateAdjustRetaken_Case1640()
        {

            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                ReceiveNotification rn = new ReceiveNotification();
                List<string> topics = new List<string>();
                topics.Add("topic.imageCreated");
                string stopTopic = "teststop";
                string stopContent = "teststop";
                rn.startListon(topics, stopTopic, stopContent);

                string acqType = string.Empty;
                Round r = this.NewRound(runCount.ToString(), "7600CR Acquisition");
                CheckPoint pAcq = new CheckPoint("Acquire Image", "Acquisition CR7600");
                r.CheckPoints.Add(pAcq);
                AcquisitionService acqs = new AcquisitionService();
                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acqType = ids.InputParameters.GetParameter(i).Value;
                    }
                }

                XMLResult rslAcq = acqs.startAcquisition(acq);
                if (rslAcq.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcq.Result = TestResult.Fail;
                    pAcq.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcq.Message);
                    SaveRound(r);
                    continue;
                }
                System.Threading.Thread.Sleep(15000);

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                switch (acqType)
                {
                    case "rotate":
                        p.StartInfo.FileName = "7600helperRotate.exe";
                        break;
                    case "adjust":
                        p.StartInfo.FileName = "7600helperAdjust.exe";
                        break;
                    case "retaken":
                        p.StartInfo.FileName = "7600helperRetaken.exe";
                        break;
                }
                p.Start();
                p.WaitForExit();

                int retrynum = 24;
                int n = 0;
                while (rn.getRecievedNumber() == 0)
                {
                    n++;
                    if (n < retrynum)
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                        break;
                }

                if (n == retrynum)
                {
                    pAcq.Result = TestResult.Fail;
                    pAcq.Outputs.AddParameter("Acquire 7600", "Notification Fail", @"Recieve Notification Timeout(120 seconds)");
                    SaveRound(r);
                    continue;
                }


                System.Threading.Thread.Sleep(15000);
                ApplicationService apps = new ApplicationService();
                apps.sendGenericNotification(stopTopic, stopContent);
                System.Threading.Thread.Sleep(1000);
                List<string> mmm = rn.getNotificationContent();
                ImageService imgs = new ImageService();
                if (acqType == "retaken")
                {
                    int notificationNum = mmm.Count;
                    if (notificationNum != 3)
                    {
                        pAcq.Result = TestResult.Fail;
                        pAcq.Outputs.AddParameter("Acquire 7600", "Notification number not correct", " Actual:" + notificationNum.ToString() + "Expect:3");
                        SaveRound(r);
                        continue;
                    }
                    foreach (string msg in mmm)
                    {
                        string[] token = msg.Split('=');
                        string imageid = token[token.Length - 1];
                        XMLParameter cImg = new XMLParameter("image");
                        cImg.AddParameter("internal_id", imageid);
                        XMLResult rslGetRetaken = imgs.getImageInfo(cImg);
                        if (rslGetRetaken.IsErrorOccured)
                        {
                            pAcq.Result = TestResult.Fail;
                            pAcq.Outputs.AddParameter("Acquire 7600", "Get Retaken Image Fail", rslGetRetaken.Message);
                            SaveRound(r);
                            goto error;
                        }
                    }

                    goto retaken;
                error:
                    continue;
                }

                string[] token1 = mmm[0].Split('=');
                string imageid1 = token1[token1.Length - 1];

                XMLParameter img = new XMLParameter("image");
                img.AddParameter("internal_id", imageid1);
                XMLResult rslGet = imgs.getImageInfo(img);
                if (rslGet.IsErrorOccured)
                {
                    pAcq.Result = TestResult.Fail;
                    pAcq.Outputs.AddParameter("Acquire 7600", "GetImage Fail", rslGet.Message);
                    SaveRound(r);
                    continue;
                }
                XMLParameter filter = new XMLParameter("filter");
                filter.AddParameter("current", "true");
                XMLResult rslListPS = imgs.listPresentationState(filter, imageid1);
                string psid = rslListPS.SingleResult;

                PresentationStateService pss = new PresentationStateService();
                XMLResult rslget = pss.getPresentationState(psid);
                string processingXml = rslget.MultiResults[0].Parameters[0].ParameterValue.Trim();
                ProcessingXMLParser parser = new ProcessingXMLParser(processingXml);
                ProcessingXMLInfo pinfo = parser.parse();

                if (acqType == "rotate" && pinfo.Rotate180Enabled != "true")
                {
                    pAcq.Result = TestResult.Fail;
                    pAcq.Outputs.AddParameter("Acquire 7600", "ProcessingXML not correct", "Rotate180 is " + pinfo.Rotate180Enabled);
                    SaveRound(r);
                    continue;
                }

                if (acqType == "adjust" && pinfo.BrightnessEnabled != "true" && pinfo.BrightnessValue == "0")
                {
                    pAcq.Result = TestResult.Fail;
                    pAcq.Outputs.AddParameter("Acquire 7600", "ProcessingXML not correct", "BrightnessValue is 0");
                    SaveRound(r);
                    continue;
                }
            retaken:
                pAcq.Result = TestResult.Pass;
                pAcq.Outputs.AddParameter("Acquire 7600", "Success", "OK");
                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_GetFMSinfo_Case1176()
        {
            int runCount = 0;
            string acquireType = string.Empty;

            ApplicationService app = new ApplicationService();
            NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification();
            System.Collections.Generic.List<string> topics = new System.Collections.Generic.List<string>();
            topics.Add("topic.acquisitionCompleted");
            topics.Add("topic.fmsCreated");
            topics.Add("topic.imageCreated");
            string stopTopic = "teststop";
            string stopContent = "teststop";
            rn.startListon(topics, stopTopic, stopContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");

                AcquisitionService acqs = new AcquisitionService();

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

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition CR7600");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(10000);

                switch (acquireType)
                {
                    case "CR7600":
                        System.Diagnostics.Debug.Print("CR7600 Acquire");
                        System.Diagnostics.Process p = System.Diagnostics.Process.Start("7600helper.exe");
                        p.WaitForExit();
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT CR7600 Acquire");
                        break;
                }
                System.Threading.Thread.Sleep(60000);//Wait acqpanel exit
                app.sendGenericNotification(stopTopic, stopContent);
                System.Threading.Thread.Sleep(1000);

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
                    int msgnum = rn.getNotificationNumber();
                    if (msgnum != 4)
                    {
                        pAcquire.Result = TestResult.Fail;
                        pAcquire.Outputs.AddParameter("Notification Error", "startAcquisition", "RecievedMessageNo:" + msgnum.ToString() + ",Expected:4");
                        continue;
                    }

                    System.Collections.Generic.List<string> mmm = rn.getNotificationContent();
                    string[] token = mmm[0].Split('=');
                    string imageid = token[token.Length - 1];
                    ImageService imgs = new ImageService();
                    XMLParameter cInputImgid = new XMLParameter("image");
                    cInputImgid.AddParameter("internal_id", imageid);
                    XMLResult getRsl = imgs.getImageInfo(cInputImgid);
                    if (!getRsl.IsErrorOccured)
                    {
                        pAcquire.Result = TestResult.Pass;
                        pAcquire.Outputs.AddParameter("Acquistion Success", "startAcquisition", "Message:OK");
                    }
                    else
                    {
                        pAcquire.Result = TestResult.Fail;
                        pAcquire.Outputs.AddParameter("GetImageInfo Error", "startAcquisition", "Message:" + getRsl.Message);
                        continue;
                    }

                }


                SaveRound(r);
                Output();
            }

        }

        public void Run_Acquisition_GetImageInfo_Case1173()
        {
            int runCount = 0;
            string acquireType = string.Empty;

            ApplicationService app = new ApplicationService();
            NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification();
            System.Collections.Generic.List<string> topics = new System.Collections.Generic.List<string>();
            topics.Add("topic.acquisitionCompleted");
            topics.Add("topic.imageCreated");

            string stopTopic = "teststop";
            string stopContent = "teststop";
            rn.startListon(topics, stopTopic, stopContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");

                AcquisitionService acqs = new AcquisitionService();

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

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition CR7600");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(10000);

                switch (acquireType)
                {
                    case "CR7600":
                        System.Diagnostics.Debug.Print("CR7600 Acquire");
                        System.Diagnostics.Process p = System.Diagnostics.Process.Start("7600helper.exe");
                        p.WaitForExit();
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT CR7600 Acquire");
                        break;
                }
                System.Threading.Thread.Sleep(60000);//Wait acqpanel exit
                app.sendGenericNotification(stopTopic, stopContent);
                System.Threading.Thread.Sleep(1000);

                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    pAcquire.Outputs.AddParameter("Acquire session ID", "startAcquisition", rslAcqRVG.SingleResult);
                    int msgnum = rn.getNotificationNumber();
                    if (msgnum != 3)
                    {
                        pAcquire.Result = TestResult.Fail;
                        pAcquire.Outputs.AddParameter("Notification Error", "startAcquisition", "RecievedMessageNo:" + msgnum.ToString() + ",Expected:3");
                        continue;
                    }

                    System.Collections.Generic.List<string> mmm = rn.getNotificationContent();
                    string[] token = mmm[0].Split('=');
                    string imageid = token[token.Length - 1];
                    ImageService imgs = new ImageService();
                    XMLParameter cInputImgid = new XMLParameter("image");
                    cInputImgid.AddParameter("internal_id", imageid);
                    XMLResult getRsl = imgs.getImageInfo(cInputImgid);
                    if (!getRsl.IsErrorOccured)
                    {
                        pAcquire.Result = TestResult.Pass;
                        pAcquire.Outputs.AddParameter("Acquistion Success", "startAcquisition", "Message:OK");
                    }
                    else
                    {
                        pAcquire.Result = TestResult.Fail;
                        pAcquire.Outputs.AddParameter("GetImageInfo Error", "startAcquisition", "Message:" + getRsl.Message);
                        continue;
                    }

                }


                SaveRound(r);
                Output();
            }

        }

        public void Run_Acquisition_AcquireFMS_Case1113()
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();

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
                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", getAcqRVG.ResultContent);
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition return error.", getAcqRVG.ResultContent);
                            }

                            break;
                        }
                        if (getAcqRVG.IsErrorOccured && getAcqRVG.Code != 499)
                        {
                            continue;
                        }
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

        public void Run_Acquisition_AcquireCeph_Case1110()
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();

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

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition CEPH");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "CEPH":
                        System.Diagnostics.Debug.Print("CEPH Acquire");
                        Utility.AcqCephImage(50);
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

                            string imageID = parser.getStringWithPathAndType("trophy/object_info", "image", "value");
                            string[] imageIDs = imageID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            int imageCount = imageIDs.Length;

                            string psID = parser.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            string[] psIDs = psID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            int psCount = psIDs.Length;

                            if (imageCount == 1 && psCount == 1)
                            {
                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", getAcqRVG.ResultContent);
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition return error.", getAcqRVG.ResultContent);
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

        public void Run_Acquisition_AcquirePano_Case1109()
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();

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

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition PANO");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);

                switch (acquireType)
                {
                    case "PANO":
                        System.Diagnostics.Debug.Print("PANO Acquire");
                        Utility.AcqPanoImage(50);
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

                            string imageID = parser.getStringWithPathAndType("trophy/object_info", "image", "value");
                            string[] imageIDs = imageID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            int imageCount = imageIDs.Length;

                            string psID = parser.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            string[] psIDs = psID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            int psCount = psIDs.Length;

                            if (imageCount == 1 && psCount == 1)
                            {
                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", getAcqRVG.ResultContent);
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition return error.", getAcqRVG.ResultContent);
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

        public void Run_Acquisition_AcquireRVG_Case1108()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();

                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(3000);
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

                            string imageID = parser.getStringWithPathAndType("trophy/object_info", "image", "value");
                            string[] imageIDs = imageID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            int imageCount = imageIDs.Length;

                            string psID = parser.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            string[] psIDs = psID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            int psCount = psIDs.Length;

                            if (imageCount == 1 && psCount == 1)
                            {
                                pAcquire.Result = TestResult.Pass;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", getAcqRVG.ResultContent);
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition return error.", getAcqRVG.ResultContent);
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

        public void Run_Acquisition_Notification_Case985()
        {
            int runCount = 0;
            string patientUID = string.Empty;
            bool isCreatePatient = false;

            string stopMessageType = "teststop";
            string stopMessageContent = "teststop";

            NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification();
            //NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification("10.112.39.167");
            System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
            rms.Add("topic.patientCreated");
            rms.Add("topic.acquisitionCompleted");
            rms.Add("topic.imageCreated");
            rms.Add("topic.presentationStateModified");
            rn.startListon(rms, stopMessageType, stopMessageContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquire image");
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                AcquisitionService acqs = new AcquisitionService();
                ApplicationService ass = new ApplicationService();

                XMLParameter pa = new XMLParameter("patient");
                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                if (isCreatePatient)
                {
                    XMLResult result = ps.createPatient(pa);
                    if (!result.IsErrorOccured)
                    {
                        pCreate.Result = TestResult.Pass;
                        patientUID = result.SingleResult;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                    }
                    else
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("create patient error", "Create patient", result.Message);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Test acquire");
                r.CheckPoints.Add(pAcquire);
                acq.AddParameter("patient_internal_id", patientUID);
                System.Diagnostics.Debug.Print("Acquire start");

                XMLResult rslAcq = acqs.startAcquisition(acq);
                System.Diagnostics.Debug.Print("Acquire :");
                System.Threading.Thread.Sleep(5000);

                if (rslAcq.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("acquire", "Acquire image error", rslAcq.Message);
                }
                else
                {
                    pAcquire.Outputs.AddParameter("acquire", "Acquire session ID", rslAcq.SingleResult);
                    int DOcount = 0;
                    XMLResult getAcq = new XMLResult();
                    do
                    {
                        System.Threading.Thread.Sleep(3000);
                        System.Diagnostics.Debug.Print("get acquire in do");
                        DOcount++;
                        getAcq = acqs.getAcquisitionResult(rslAcq.SingleResult);
                        if (!getAcq.IsErrorOccured)
                        {
                            pAcquire.Result = TestResult.Pass;
                            pAcquire.Outputs.AddParameter("acquire", "Get acquire image", getAcq.Message);
                            break;
                        }
                        if (getAcq.IsErrorOccured && getAcq.Code != 499)
                            continue;
                        if (getAcq.Code != 0 && getAcq.Code != 499)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("acquire", "Get acquire image error", getAcq.Message);
                            break;
                        }
                        System.Diagnostics.Debug.Print("get acquireResult:" + DOcount);
                        if (DOcount > 60)
                        {
                            pAcquire.Result = TestResult.Fail;
                            pAcquire.Outputs.AddParameter("acquire", "Get acquire image error", "Acquire greate with 3 minutes, timeout!");
                            break;
                        }
                    } while (true);
                }

                System.Threading.Thread.Sleep(500);

                ass.sendGenericNotification(stopMessageType, stopMessageContent);

                System.Threading.Thread.Sleep(500);

                int getn = rn.getNotificationNumber();

                CheckPoint pNotification = new CheckPoint("Receive Notification", "acquire message");
                r.CheckPoints.Add(pNotification);

                if (getn == 4)
                {
                    pNotification.Result = TestResult.Pass;
                    foreach (string mmm in rn.getNotificationContent())
                    {
                        pNotification.Outputs.AddParameter("Notification message", "Content:", mmm);
                    }
                }
                else
                {
                    pNotification.Result = TestResult.Fail;
                    foreach (string mmm in rn.getNotificationContent())
                    {
                        pNotification.Outputs.AddParameter("Notification message", "Content:", mmm);
                    }
                    pNotification.Outputs.AddParameter("Message", "Content:", "miss message on receive");
                }

                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_QueryLinces_Case5()
        {
            //int runCount = this.Input.Repetition;

            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p = new CheckPoint("QueryLines", "Test queryLines");

                r.CheckPoints.Add(p);

                //create required PAS service instaces here
                AcquisitionService acqs = new AcquisitionService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa = new XMLParameter("query_lines");


                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }

                //Get service result
                System.Diagnostics.Debug.Print("PAS: queryLines start!");
                XMLResult result = acqs.queryLines(pa);

                //Log service output
                if (result.IsErrorOccured)
                {
                    p.Result = TestResult.Fail;
                    p.Outputs.AddParameter("QueryLines", "Error", result.Message);
                }
                else
                {
                    p.Result = TestResult.Pass;
                    p.Outputs.AddParameter("QueryLines", "Success", result.ResultContent);
                }

                //Save data for each round
                SaveRound(r);
            }

            //Save service log as xml file
            Output();
        }

        public void Run_Acquisition_QueryDevices_Case4()
        {
            //int runCount = this.Input.Repetition;

            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p = new CheckPoint("QueryDevices", "Test queryDevices");

                r.CheckPoints.Add(p);

                //create required PAS service instaces here
                AcquisitionService acqs = new AcquisitionService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa = new XMLParameter("query_devices");


                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }

                //Get service result
                XMLResult result = acqs.queryDevices(pa);

                //Log service output
                if (result.IsErrorOccured)
                {
                    p.Result = TestResult.Fail;
                    p.Outputs.AddParameter("QueryDevices", "Error", result.Message);
                }
                else
                {
                    p.Result = TestResult.Pass;
                    p.Outputs.AddParameter("QueryDevices", "Success", result.ResultContent);
                }

                //Save data for each round
                SaveRound(r);
            }

            //Save service log as xml file
            Output();
        }

        public void Run_Acquisition_3D_Case505()
        {
            int runCount = 0;
            string patientUID = string.Empty;
            string acquireType = string.Empty;
            bool isCreatePatient = false;

            string stopMessageType = "teststop";
            string stopMessageContent = "teststop";

            NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification();
            //NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification("10.112.39.167");
            System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
            //rms.Add("topic.acquisitionCompleted");
            rms.Add("topic.volumeCreated");
            rn.startListon(rms, stopMessageType, stopMessageContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquire image");
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                AcquisitionService acqs = new AcquisitionService();
                ApplicationService ass = new ApplicationService();
                VolumeService vss = new VolumeService();

                XMLParameter pa = new XMLParameter("patient");
                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter volfilter = new XMLParameter("volume");


                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "acquireType")
                    {
                        acquireType = ids.InputParameters.GetParameter(i).Key;
                    }
                }
                if (isCreatePatient)
                {
                    XMLResult result = ps.createPatient(pa);
                    if (!result.IsErrorOccured)
                    {
                        pCreate.Result = TestResult.Pass;
                        patientUID = result.SingleResult;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                    }
                    else
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("create patient error", "Create patient", result.Message);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Test acquire");
                r.CheckPoints.Add(pAcquire);
                acq.AddParameter("patient_internal_id", patientUID);
                XMLResult rslAcq = acqs.startAcquisition(acq);
                System.Threading.Thread.Sleep(30000);

                switch (acquireType)
                {
                    case "3D":
                        System.Diagnostics.Debug.Print("3D Volume Acquire");
                        TestUtility.Utility.Acq3DVolume(100);
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                if (rslAcq.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("acquire", "Acquire image error", rslAcq.Message);
                }
                else
                {
                    System.Diagnostics.Debug.Print("Acquire succeed:");
                    pAcquire.Result = TestResult.Pass;
                }
                //else
                //{
                //    pAcquire.Outputs.AddParameter("acquire", "Acquire session ID", rslAcq.SingleResult);
                //    int DOcount = 0;
                //    XMLResult getAcq = new XMLResult();
                //    do
                //    {
                //        System.Threading.Thread.Sleep(3000);
                //        System.Diagnostics.Debug.Print("get acquire in do");
                //        DOcount++;
                //        getAcq = acqs.getAcquisitionResult(rslAcq.SingleResult);
                //        if (!getAcq.IsErrorOccured)
                //        {
                //            int getn = rn.getRecievedNumber();
                //            if (getn > 0)
                //            {
                //                ass.sendGenericNotification(stopMessageType, stopMessageContent);
                //                System.Threading.Thread.Sleep(50);
                //                System.Collections.Generic.List<string> cont = rn.getNotificationContent();

                //                foreach (string s in cont)
                //                {
                //                    string[] mess = s.Split('=');
                //                    if (mess.Length < 3)
                //                    {
                //                        continue;
                //                    }
                //                    string volumeid = mess[mess.Length - 1];
                //                    volfilter.AddParameter("internal_id", volumeid);

                //                    CheckPoint pVolume = new CheckPoint("Get Volume", "Test create");
                //                    r.CheckPoints.Add(pVolume);
                //                    XMLResult volrsl = vss.getVolumeInfo(volfilter);
                //                    if (!volrsl.IsErrorOccured)
                //                    {
                //                        pVolume.Result = TestResult.Pass;
                //                        pVolume.Outputs.AddParameter("Volume", "Get volume success", "Acquire 3D, OK!");
                //                        pVolume.Outputs.AddParameter("Volume", "Volume info", volrsl.Message);
                //                        //break;
                //                    }
                //                    else
                //                    {
                //                        pVolume.Result = TestResult.Fail;
                //                        pVolume.Outputs.AddParameter("Volume", "Get volume failed", "Acquire 3D, OK!");
                //                    }
                //                }

                //                pAcquire.Result = TestResult.Pass;
                //                pAcquire.Outputs.AddParameter("acquire", "Acquire volume success", "Acquire 3D, OK!");
                //                break;
                //            }
                //        }
                //        if (getAcq.IsErrorOccured && getAcq.Code == 499)
                //            continue;
                //        if (getAcq.Code != 0 && getAcq.Code != 499)
                //        {
                //            pAcquire.Result = TestResult.Fail;
                //            pAcquire.Outputs.AddParameter("acquire", "Get acquire image error", getAcq.Message);
                //            break;
                //        }
                //        System.Diagnostics.Debug.Print("get acquireResult:" + DOcount);
                //        if (DOcount > 250)
                //        {
                //            pAcquire.Result = TestResult.Fail;
                //            pAcquire.Outputs.AddParameter("acquire", "Get acquire image error", "Acquire greate with 3 minutes, timeout!");
                //            break;
                //        }

                //    } while (true);
                //}

                System.Threading.Thread.Sleep(1000);
                SaveRound(r);
            }

            Output();

        }
        
        public void Run_Acquisition_New_RVG_1_Case1664()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";

                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                string acq_session_id = "";
                System.Threading.Thread.Sleep(10);
                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            image_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");

                        //check the acquired image has been put into correct position.
                        target_path = pc.getValueByKey("target_path");
                        int indexPath = target_path.IndexOf(";");
                        if (indexPath != -1)
                            target_path = target_path.Substring(0, indexPath);
                        //now the Processing data file not provide correct after send command
                        bool dicomfileexist = Utility.isFileExisted(target_path);
                        if (!dicomfileexist)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                            SaveRound(r);
                            continue;
                        }

                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            target_path = target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == target_path)
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_New_FMS_2_Case1670()
        {
            int runCount = 0;
            string acquireType = string.Empty;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");

                ApplicationService apps = new ApplicationService();
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
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
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                //start stomp message listener
                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";

                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);

                switch (acquireType)
                {
                    case "FMS":
                        System.Diagnostics.Debug.Print("FMS Acquire");
                        //Utility.AcqFMS(40, 300);
                        System.Threading.Thread tx = new System.Threading.Thread(Utility.AcquireFMSOne);
                        tx.Start();
                        break;
                    default:
                        System.Diagnostics.Debug.Print("NOT FMS/PANO/CEPH/CR Acquire");
                        break;
                }

                string acq_session_id = "";
                //System.Threading.Thread.Sleep(50);
                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);

                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND ERROR", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string fms_internal_id_from_acqResult = "";
                    string fms_internal_id_from_mapfile = "";
                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check EROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }

                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);

                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslForFMS = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            fms_internal_id_from_acqResult = rslForFMS.getStringWithPathAndType("trophy/object_info", "fms", "value");
                            image_internal_id_from_acqResult = rslForFMS.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslForFMS.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 300);
                    if (timeout >= 300)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    string fmsfilepath = "";
                    string original_path = "";
                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";
                    string keep_target_path = "";
                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");
                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && type.Contains(kvp.Value))
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        original_path = pc.getValueByKey("original_path");

                        //check the acquired image has been put into correct position.
                        target_path = pc.getValueByKey("target_path");
                        keep_target_path = target_path;
                        int needCheckFile = 0;
                        int checkedFile = 0;
                        do
                        {
                            string sub_target_path = target_path;
                            int indexKey = sub_target_path.IndexOf(";");
                            if (indexKey != -1)
                            {
                                sub_target_path = sub_target_path.Substring(0, indexKey);
                                target_path = target_path.Substring(indexKey + 1);

                                //temp in here to skip the processing file check.
                                if (sub_target_path.Contains(".xml"))
                                {
                                    continue;
                                }

                                needCheckFile++;
                                bool dicomfileexist = Utility.isFileExisted(sub_target_path);
                                if (sub_target_path.Contains(".fms"))
                                    fmsfilepath = sub_target_path;


                                if (!dicomfileexist)
                                {
                                    pCheckMessage.Result = TestResult.Fail;
                                    pCheckMessage.Outputs.AddParameter("Dicom File {" + sub_target_path + "} Check ERROR", "Check Command", "The acquired image not in cache place");
                                    SaveRound(r);
                                    continue;
                                }
                                checkedFile++;
                            }
                        } while (target_path != "");

                        if (needCheckFile != checkedFile)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Files Check ERROR", "Check Command", "No all files contained in Message are exist");
                            SaveRound(r);
                            continue;
                        }

                        //get all image uid from mapping file
                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        //get all ps uid from mapping file
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                        //get FMS uid from mapping file
                        fms_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "FMS", "instance_internal_id");

                        string processdatafilecontent = pc.getWithPathAndTypeFromMappingFile("path_mapping", "ProcessingDataFile", "content");
                        int countForProcessing = 0;
                        do
                        {
                            string sub_processdatafilecontent = processdatafilecontent;
                            int indexKey3 = sub_processdatafilecontent.IndexOf(";");
                            if (indexKey3 != -1)
                            {

                                sub_processdatafilecontent = sub_processdatafilecontent.Substring(0, indexKey3);
                                processdatafilecontent = processdatafilecontent.Substring(indexKey3 + 1);
                                if (sub_processdatafilecontent != "")
                                {
                                    countForProcessing++;
                                    string processingXMLcontent = base64tools.DecodeString4base(sub_processdatafilecontent);
                                    pCheckMessage.Outputs.AddParameter("Processing Data File from SDK", "Processing Content " + countForProcessing, processingXMLcontent);
                                }
                            }
                        } while (processdatafilecontent != "");

                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (fms_internal_id_from_acqResult != fms_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The FMS id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Check Command", acq_out);

                    //Check the FMS contain the correct image that compare with mapping files
                    ParseXMLContent pc1 = new ParseXMLContent(fmsfilepath, "File");
                    string psIDInFMS = pc1.getStringFromPath("FMS_DATA/ItemData/Identifier");
                    int checkPSIDInResult = 0;
                    int psInFMSContent = 0;
                    do
                    {
                        string sub_psID = psIDInFMS;
                        int indexKey = sub_psID.IndexOf(";");
                        if (indexKey != -1)
                        {
                            psInFMSContent++;
                            sub_psID = sub_psID.Substring(0, indexKey);
                            psIDInFMS = psIDInFMS.Substring(indexKey + 1);
                        }
                        if (ps_internal_id_from_acqResult.Contains(sub_psID))
                        {
                            checkPSIDInResult++;
                        }

                    } while (psIDInFMS != "");
                    if (checkPSIDInResult == psInFMSContent)
                    {
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All IMAGEs in FMS are matched with mapping file");
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "NOT All IMAGEs in FMS are matched with mapping file");
                    }
                    //Check the FMS contain the correct image end

                    System.Threading.Thread.Sleep(5000);
                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);
                    //check getFMSInfo
                    int getn = rn.getRecievedNumber();
                    if (getn == 2)
                    {
                        FMSService fmss = new FMSService();
                        int indexKey = fms_internal_id_from_mapfile.IndexOf(";");
                        if (indexKey != -1)
                        {
                            fms_internal_id_from_mapfile = fms_internal_id_from_mapfile.Substring(0, indexKey);
                        }

                        //check get FMS description with content
                        XMLResult fmsDesRsl = fmss.getFMSDescription(fms_internal_id_from_mapfile);
                        string psids = "";
                        if (fmsDesRsl.IsErrorOccured || fmsDesRsl.Code != 0)
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get FMS Info result", "Get Return XML", fmsDesRsl.ResultContent);
                            continue;
                        }
                        else
                        {
                            ParseXMLContent pc2 = new ParseXMLContent(fmsDesRsl.ResultContent);
                            pc2.getValueFromPath("trophy/presentationstate");
                            psids = pc2.getValueByKey("internal_id");
                        }
                        int checkPSIDInFMSDes = 0;
                        int psInGetFMSDes = 0;
                        do
                        {
                            string sub_psID = psids;
                            int indKey = sub_psID.IndexOf(";");
                            if (indKey != -1)
                            {
                                psInGetFMSDes++;
                                sub_psID = sub_psID.Substring(0, indKey);
                                psids = psids.Substring(indKey + 1);
                            }
                            if (ps_internal_id_from_acqResult.Contains(sub_psID))
                            {
                                checkPSIDInFMSDes++;
                            }

                        } while (psids != "");

                        if (checkPSIDInFMSDes == psInGetFMSDes)
                        {
                            pCheckGetImage.Outputs.AddParameter("Check the presentation ids again with getAcquisitionResult", "Check the presentation ids", "ALL IDs is equal with aspect");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Check the presentation ids again with getAcquisitionResult", "Check the presentation ids", "Some IDs is not equal with aspect");
                            continue;
                        }

                        //got the first image to check
                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            keep_target_path = keep_target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && keep_target_path.Contains(getImageRsl.DicomArrayResult.Parameters[i].ParameterValue))
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }


                    pCheckMessage.Result = TestResult.Pass;
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_New_Pano_3_Case1671()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                string acquireType = string.Empty;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                System.Threading.Thread.Sleep(20);
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
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
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";
                
                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                string acq_session_id = "";

                System.Threading.Thread.Sleep(1000);
                switch (acquireType)
                {
                    case "PANO":
                        System.Diagnostics.Debug.Print("PANO Acquire");
                        //Utility.AcqPanoImage(300);
                        System.Threading.Thread tx = new System.Threading.Thread(Utility.AcquirePanoOne);
                        tx.Start();
                        break;
                    case "CEPH":
                        System.Diagnostics.Debug.Print("CEPH Acquire");
                        //Utility.AcqCephImage(50);
                        System.Threading.Thread tx1 = new System.Threading.Thread(Utility.AcquireCephOne);
                        tx1.Start();
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
                    continue;
                }
                else
                {
                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            image_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");

                        //check the acquired image has been put into correct position.
                        target_path = pc.getValueByKey("target_path");
                        int indexPath = target_path.IndexOf(";");
                        if (indexPath != -1)
                            target_path = target_path.Substring(0, indexPath);
                        //now the Processing data file not provide correct after send command
                        bool dicomfileexist = Utility.isFileExisted(target_path);
                        if (!dicomfileexist)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                            SaveRound(r);
                            continue;
                        }

                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            target_path = target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == target_path)
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();

        }

        public void Run_Acquisition_New_Ceph_4_Case1672()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                string acquireType = string.Empty;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
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
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";

                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                string acq_session_id = "";

                System.Threading.Thread.Sleep(1000);
                switch (acquireType)
                {
                    case "PANO":
                        System.Diagnostics.Debug.Print("PANO Acquire");
                        //Utility.AcqPanoImage(300);
                        System.Threading.Thread tx = new System.Threading.Thread(Utility.AcquirePanoOne);
                        tx.Start();
                        break;
                    case "CEPH":
                        System.Diagnostics.Debug.Print("CEPH Acquire");
                        //Utility.AcqCephImage(50);
                        System.Threading.Thread tx1 = new System.Threading.Thread(Utility.AcquireCephOne);
                        tx1.Start();
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
                    continue;
                }
                else
                {
                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            image_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");

                        //check the acquired image has been put into correct position.
                        target_path = pc.getValueByKey("target_path");
                        int indexPath = target_path.IndexOf(";");
                        if (indexPath != -1)
                            target_path = target_path.Substring(0, indexPath);
                        //now the Processing data file not provide correct after send command
                        bool dicomfileexist = Utility.isFileExisted(target_path);
                        if (!dicomfileexist)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                            SaveRound(r);
                            continue;
                        }

                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            target_path = target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == target_path)
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();

        }

        public void Run_Acquisition_New_CS1600_8_Case1680()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                string acquireType = string.Empty;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20); 
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
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
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";
                
                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);

                System.Threading.Thread.Sleep(1000);

                string acq_session_id = "";
                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    //string image_internal_id_from_acqResult = "";
                    //string ps_internal_id_from_acqResult = "";
                    //string image_internal_id_from_mapfile = "";
                    //string ps_internal_id_from_mapfile = "";
                    string video_internal_id_from_acqResult = "";
                    string video_internal_id_from_mapfile = "";
                    string retInstanceType = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            video_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "video", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                                retInstanceType = type;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");

                        //check the acquired image has been put into correct position.
                        target_path = pc.getValueByKey("target_path");
                        string temp_target_path = target_path;
                        int indexPath = temp_target_path.IndexOf(";");
                        if (indexPath != -1)
                            temp_target_path = temp_target_path.Substring(0, indexPath);
                        //now the Processing data file not provide correct after send command
                        bool dicomfileexist = Utility.isFileExisted(temp_target_path);
                        if (!dicomfileexist)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                            SaveRound(r);
                            continue;
                        }

                        //image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        //ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                        video_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Video", "instance_internal_id");
                    }
                    if (video_internal_id_from_acqResult != video_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        //XMLParameter getImage = new XMLParameter("image");
                        int indexImage = video_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            video_internal_id_from_acqResult = video_internal_id_from_acqResult.Substring(0, indexImage);
                        //getImage.AddParameter("internal_id", video_internal_id_from_acqResult);
                        //ImageService imagesrv = new ImageService();
                        SimpleInstanceService simplesrv = new SimpleInstanceService();
                        XMLResult getVideoRsl = simplesrv.getInstanceInfo(video_internal_id_from_acqResult);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Video Info result", "Get Return XML", getVideoRsl.ResultContent);
                        int matchValueCount = 0;
                        ParseXMLContent rslVideoResult = new ParseXMLContent(getVideoRsl.ResultContent);
                        if (target_path.Contains(rslVideoResult.getResultWithKey("trophy/instance", "file_name")))
                        {
                            matchValueCount++;
                        }
                        if ((rslVideoResult.getResultWithKey("trophy/instance", "patient_internal_id")).Contains(patient_uid + ";"))
                        {
                            matchValueCount++;
                        }
                        if ((rslVideoResult.getResultWithKey("trophy/instance", "instance_type")).Contains(retInstanceType.ToLower()))
                        {
                            matchValueCount++;
                        }
                        if ((rslVideoResult.getResultWithKey("trophy/instance", "object_type")).Contains("SimpleInstance;"))
                        {
                            matchValueCount++;
                        }
                        if (matchValueCount == 4)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_New_CS1600_9_Case1680()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                string acquireType = string.Empty;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
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
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";

                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);

                System.Threading.Thread.Sleep(1000);

                string acq_session_id = "";
                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            image_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");

                        //check the acquired image has been put into correct position.
                        target_path = pc.getValueByKey("target_path");
                        int indexPath = target_path.IndexOf(";");
                        if (indexPath != -1)
                            target_path = target_path.Substring(0, indexPath);
                        //now the Processing data file not provide correct after send command
                        bool dicomfileexist = Utility.isFileExisted(target_path);
                        if (!dicomfileexist)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                            SaveRound(r);
                            continue;
                        }

                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");

                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            target_path = target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == target_path)
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_New_CS7600_6_Case1664()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";

                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                string acq_session_id = "";
                System.Threading.Thread.Sleep(50);
                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    //wait to acquire CS7600 AG Image manually.
                    System.Threading.Thread.Sleep(2000);

                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            image_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");
                        target_path = pc.getValueByKey("target_path");
                        string process_target_path = target_path;
                        do
                        {
                            //check the acquired image has been put into correct position.
                            string sub_process_target_path = process_target_path;

                            int indexPath = sub_process_target_path.IndexOf(";");
                            if (indexPath != -1)
                            {
                                sub_process_target_path = sub_process_target_path.Substring(0, indexPath);
                                process_target_path = process_target_path.Substring(indexPath + 1);
                                //temp in here to skip the processing file check.
                                if (sub_process_target_path.Contains(".xml"))
                                {
                                    continue;
                                }
                                //now the Processing data file not provide correct after send command
                                bool dicomfileexist = Utility.isFileExisted(sub_process_target_path);
                                if (!dicomfileexist)
                                {
                                    pCheckMessage.Result = TestResult.Fail;
                                    pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                                    SaveRound(r);
                                    continue;
                                }
                            }

                        } while (process_target_path != "");


                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                        string processdatafilecontent = pc.getWithPathAndTypeFromMappingFile("path_mapping", "ProcessingDataFile", "content");
                        int countForProcessing = 0;
                        do
                        {
                            string sub_processdatafilecontent = processdatafilecontent;
                            int indexKey3 = sub_processdatafilecontent.IndexOf(";");
                            if (indexKey3 != -1)
                            {

                                sub_processdatafilecontent = sub_processdatafilecontent.Substring(0, indexKey3);
                                processdatafilecontent = processdatafilecontent.Substring(indexKey3 + 1);
                                if (sub_processdatafilecontent != "")
                                {
                                    countForProcessing++;
                                    string processingXMLcontent = base64tools.DecodeString4base(sub_processdatafilecontent);
                                    pCheckMessage.Outputs.AddParameter("Processing Data File from SDK", "Processing Content " + countForProcessing, processingXMLcontent);
                                }
                            }
                        } while (processdatafilecontent != "");
                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            target_path = target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && target_path.Contains(getImageRsl.DicomArrayResult.Parameters[i].ParameterValue))
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Acquisition_New_CS7600_7_Case1664()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                ApplicationService apps = new ApplicationService();
                //start 2D simulator and register 2D simulator to CSDM with same port number
                Start2DSimulator s2d = new Start2DSimulator(10000);
                System.Threading.Thread.Sleep(20);
                apps.registerApplication("2DViewer", "localhost", 10000, true);

                //get acquire information and expect information
                XMLParameter acq = new XMLParameter("acq_info");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                Dictionary<string, string> expectVal = new Dictionary<string, string>();
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.InputParameters.GetParameter(j).Step == "acquire")
                    {
                        expectVal.Add(ids.ExpectedValues.GetParameter(j).Key, ids.ExpectedValues.GetParameter(j).Value);
                    }
                }

                CheckPoint pAcquire = new CheckPoint("Acquire Image", "Acquisition RVG");
                r.CheckPoints.Add(pAcquire);
                System.Diagnostics.Debug.Print("Acquire start");

                string stopMessageType = "teststop";
                string stopMessageContent = "teststop";

                string hostadd = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "host");
                string portnoti = PAS.AutoTest.TestUtility.Utility.GetCSDMConfig(PAS.AutoTest.TestUtility.CSDMConfigSection.remote, "notificationPort");
                NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification(hostadd, int.Parse(portnoti));

                System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
                rms.Add("topic.acquisitionCompleted");
                rn.startListon(rms, stopMessageType, stopMessageContent);

                AcquisitionService acqs = new AcquisitionService();
                XMLResult rslAcqRVG = acqs.startAcquisition(acq);
                string acq_session_id = "";
                System.Threading.Thread.Sleep(50);
                if (rslAcqRVG.IsErrorOccured)
                {
                    System.Diagnostics.Debug.Print("Acquire fail:");
                    pAcquire.Result = TestResult.Fail;
                    pAcquire.Outputs.AddParameter("Acquire image error", "startAcquisition", rslAcqRVG.Message);
                    continue;
                }
                else
                {
                    //wait to acquire CS7600 AG Image manually.
                    System.Threading.Thread.Sleep(2000);

                    acq_session_id = rslAcqRVG.SingleResult;
                    pAcquire.Result = TestResult.Pass;
                    pAcquire.Outputs.AddParameter("Acquire image success", "startAcquisition", rslAcqRVG.ResultContent);

                    //start an advanced simulator to receive the specified command
                    int port = 10000;
                    string sRet = "";
                    TwoDSim.AdvanceSimulator asi = new TwoDSim.AdvanceSimulator(port);
                    asi.StartSimulater("0,OK;open_acquired_objects");
                    sRet = asi.StopSimulator(120000);


                    CheckPoint pCheckMessage = new CheckPoint("Check Command", "Acquisition SOCKET Message");
                    r.CheckPoints.Add(pCheckMessage);
                    if (sRet == "")
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("NO COMMAND", "Check Command", "TIMEOUT: The socket message doesn't received");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pCheckMessage.Outputs.AddParameter("open_acquired_objects COMMAND parameter", "Record Command", sRet);
                    }

                    ParseMessageContent cn = new ParseMessageContent(sRet);

                    //get patient internal id from command message
                    string patient_uid = cn.getValueFromKey("-patient_internal_id");
                    bool gotPatientId = false;
                    foreach (KeyValuePair<string, string> kvp in expectVal)
                    {
                        if (kvp.Key == "patient_internal_id" && kvp.Value == patient_uid)
                        {
                            gotPatientId = true;
                        }
                    }
                    if (!gotPatientId)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Patient ID Check ERROR", "Check Command", "The socket message content with WRONG patient ID");
                        SaveRound(r);
                        continue;
                    }

                    string fms_internal_id_from_acqResult = "";
                    string fms_internal_id_from_mapfile = "";
                    string image_internal_id_from_acqResult = "";
                    string ps_internal_id_from_acqResult = "";
                    string image_internal_id_from_mapfile = "";
                    string ps_internal_id_from_mapfile = "";

                    //get acquisition session id from command message
                    string session_uid_from_command = cn.getValueFromKey("-session_id");
                    if (acq_session_id != session_uid_from_command)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Session ID Check ERROR", "Check Command", "The session id from command not equals the id from startAcquisition!");
                        SaveRound(r);
                        continue;
                    }
                    int timeout = 0;
                    do
                    {
                        XMLResult rslgetAcqResult = acqs.getAcquisitionResult(session_uid_from_command);
                        //timeout added
                        if (!rslgetAcqResult.IsErrorOccured && rslgetAcqResult.Code == 0)
                        {
                            pCheckMessage.Outputs.AddParameter("Show the content after getAcquisitionResult", "Output getAcquisitionResult", rslgetAcqResult.ResultContent);
                            ParseXMLContent rslacqResult = new ParseXMLContent(rslgetAcqResult.ResultContent);
                            image_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "image", "value");
                            ps_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                            fms_internal_id_from_acqResult = rslacqResult.getStringWithPathAndType("trophy/object_info", "fms", "value");
                            break;
                        }
                        timeout++;
                        System.Threading.Thread.Sleep(1000);
                    } while (timeout < 120);
                    if (timeout >= 120)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Try too many times", "Check getAcquisitionResult", "TIMEOUT of getAcquisitionResult");
                        break;
                    }

                    //parser the mapping xml content from command message
                    string mapping_xml = cn.getValueFromKey("-mapping_xml");
                    string mapping_content = base64tools.DecodeString4base(mapping_xml);
                    pCheckMessage.Outputs.AddParameter("Show the decode mapping file content", "Output mappingfile", mapping_content);
                    string target_path = "";

                    if (mapping_content != null)
                    {
                        ParseXMLContent pc = new ParseXMLContent(mapping_content);
                        pc.getValueFromPath("path_mapping/instance");
                        string type = pc.getValueByKey("type");

                        int indexKey = type.IndexOf(";");
                        if (indexKey != -1)
                            type = type.Substring(0, indexKey);

                        bool gotType = false;
                        foreach (KeyValuePair<string, string> kvp in expectVal)
                        {
                            if (kvp.Key == "type" && kvp.Value == type)
                            {
                                gotType = true;
                            }
                        }
                        if (!gotType)
                        {
                            pCheckMessage.Result = TestResult.Fail;
                            pCheckMessage.Outputs.AddParameter("Type Check ERROR", "Check Command ", "The socket message content with WRONG type");
                            SaveRound(r);
                            continue;
                        }

                        string original_path = pc.getValueByKey("original_path");
                        target_path = pc.getValueByKey("target_path");
                        string process_target_path = target_path;
                        do
                        {
                            //check the acquired image has been put into correct position.
                            string sub_process_target_path = process_target_path;

                            int indexPath = sub_process_target_path.IndexOf(";");
                            if (indexPath != -1)
                            {
                                sub_process_target_path = sub_process_target_path.Substring(0, indexPath);
                                process_target_path = process_target_path.Substring(indexPath + 1);
                                //temp in here to skip the processing file check.
                                if (sub_process_target_path.Contains(".xml"))
                                {
                                    continue;
                                }
                                //now the Processing data file not provide correct after send command
                                bool dicomfileexist = Utility.isFileExisted(sub_process_target_path);
                                if (!dicomfileexist)
                                {
                                    pCheckMessage.Result = TestResult.Fail;
                                    pCheckMessage.Outputs.AddParameter("Dicom File Check ERROR", "Check Command", "The acquired image not in cache place");
                                    SaveRound(r);
                                    continue;
                                }
                            }

                        } while (process_target_path != "");


                        image_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "instance_internal_id");
                        ps_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "Image", "ps_internal_id");
                        fms_internal_id_from_mapfile = pc.getWithPathAndTypeFromMappingFile("path_mapping", "FMS", "instance_internal_id");

                        string processdatafilecontent = pc.getWithPathAndTypeFromMappingFile("path_mapping", "ProcessingDataFile", "content");
                        int countForProcessing = 0;
                        do
                        {
                            string sub_processdatafilecontent = processdatafilecontent;
                            int indexKey3 = sub_processdatafilecontent.IndexOf(";");
                            if (indexKey3 != -1)
                            {

                                sub_processdatafilecontent = sub_processdatafilecontent.Substring(0, indexKey3);
                                processdatafilecontent = processdatafilecontent.Substring(indexKey3 + 1);
                                if (sub_processdatafilecontent != "")
                                {
                                    countForProcessing++;
                                    string processingXMLcontent = base64tools.DecodeString4base(sub_processdatafilecontent);
                                    pCheckMessage.Outputs.AddParameter("Processing Data File from SDK", "Processing Content " + countForProcessing, processingXMLcontent);
                                }
                            }
                        } while (processdatafilecontent != "");
                    }
                    if (image_internal_id_from_acqResult != image_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The image id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (ps_internal_id_from_acqResult != ps_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The presentation id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }
                    if (fms_internal_id_from_acqResult != fms_internal_id_from_mapfile)
                    {
                        pCheckMessage.Result = TestResult.Fail;
                        pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "The FMS id from command not equals the id from getAcquisitionResult!");
                        SaveRound(r);
                        continue;
                    }

                    //parser the trophy xml content from command message
                    string acq_out_xml = cn.getValueFromKey("-acq_out_xml");
                    string acq_out = base64tools.DecodeString4base(acq_out_xml);
                    pCheckMessage.Outputs.AddParameter("XML From SDK", "Output acquisition SDK content", acq_out);
                    pCheckMessage.Result = TestResult.Pass;
                    pCheckMessage.Outputs.AddParameter("Message and Interface result", "Check Command", "All ID is matched");

                    CheckPoint pCheckGetImage = new CheckPoint("Check NOTIFICATION", "Acquisition complete Message");
                    r.CheckPoints.Add(pCheckGetImage);
                    apps.sendGenericNotification(stopMessageType, stopMessageContent);
                    System.Threading.Thread.Sleep(1000);

                    //check getImageInfo
                    int getn = rn.getNotificationNumber();
                    if (getn == 2)
                    {

                        XMLParameter getImage = new XMLParameter("image");
                        int indexImage = image_internal_id_from_acqResult.IndexOf(";");
                        if (indexImage != -1)
                            image_internal_id_from_acqResult = image_internal_id_from_acqResult.Substring(0, indexImage);
                        getImage.AddParameter("internal_id", image_internal_id_from_acqResult);
                        ImageService imagesrv = new ImageService();
                        XMLResult getImageRsl = imagesrv.getImageInfo(getImage);
                        //match items
                        pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Get Return XML", getImageRsl.ResultContent);
                        int matchValueCount = 0;
                        for (int i = 0; i < getImageRsl.DicomArrayResult.Length; i++)
                        {
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == image_internal_id_from_acqResult)
                            {
                                matchValueCount++;
                            }

                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "patient_internal_id"
                                && getImageRsl.DicomArrayResult.Parameters[i].ParameterValue == patient_uid)
                            {
                                matchValueCount++;
                            }
                            target_path = target_path.Replace('\\', '/');
                            if (getImageRsl.DicomArrayResult.Parameters[i].ParameterName == "path"
                                && target_path.Contains(getImageRsl.DicomArrayResult.Parameters[i].ParameterValue))
                            {
                                matchValueCount++;
                            }
                        }
                        if (matchValueCount == 3)
                        {
                            pCheckGetImage.Result = TestResult.Pass;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "All ID is matched");
                        }
                        else
                        {
                            pCheckGetImage.Result = TestResult.Fail;
                            pCheckGetImage.Outputs.AddParameter("Get Image Info result", "Check Return XML", "Some ID is not matched");
                        }

                    }
                    else
                    {
                        pCheckGetImage.Result = TestResult.Fail;
                        pCheckGetImage.Outputs.AddParameter("Notification not received", "Check Return", "No acquisition complete Message be received");
                    }

                }
                SaveRound(r);
            }
            Output();
        }
    

    }
}