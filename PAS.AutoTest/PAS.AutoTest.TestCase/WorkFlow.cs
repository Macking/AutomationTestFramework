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
        public void Run_WorkFlow_CSIStartsupAndInitializes_Case978() //Case 978: 1.3.17_workflow_CSI starts up and initializes
        {
            ApplicationService applicationService = new ApplicationService();
            PatientService patientService = new PatientService();
            AcquisitionService acquisitionService = new AcquisitionService();

            XMLParameter queryPatientsParameter = new XMLParameter("filter");
            XMLParameter queryDivicesParameter = new XMLParameter("query_devices");
            XMLParameter queryLinesParameter = new XMLParameter("query_lines");
            XMLParameter setAsynAcqPatientInfoParameter = new XMLParameter("acq_info");

            CheckPoint pRegister = new CheckPoint("register", "register 2D Viewer");
            CheckPoint pQueryPatients = new CheckPoint("queryPatients", "query the patients list");
            CheckPoint pQueryDevices = new CheckPoint("queryDevices", "query installed device IDs of sensor type");
            CheckPoint pQueryLines = new CheckPoint("queryLines", "query installed line IDs of device IDs");
            CheckPoint pSetAsynAcqPatientInfo = new CheckPoint("setAsynAcqPatientInfo", "set asyn acq patient info");


            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = new Round();

                r.CheckPoints.Add(pRegister);
                r.CheckPoints.Add(pQueryPatients);
                r.CheckPoints.Add(pQueryDevices);
                r.CheckPoints.Add(pQueryLines);
                r.CheckPoints.Add(pSetAsynAcqPatientInfo);

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "query_devices")
                    {
                        queryDivicesParameter.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                #region Step 1: receive getType and then register 2D applciation to CSDM

                System.Threading.Thread simulator = new System.Threading.Thread(delegate()
                {
                    TwoDSim.simulator si = new TwoDSim.simulator(2010);

                    si.StartSimulater("0,2DViewer");
                    si.StopSimulator(60000);
                }); //Use simulator to simulate there is 2D running

                simulator.Start();
                System.Threading.Thread.Sleep(3000);

                XMLResult registerResult = applicationService.registerApplication("2DViewer", "localhost", 2010, true);

                try
                {
                    simulator.Join(3000);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (registerResult.IsErrorOccured)
                {
                    pRegister.Result = TestResult.Fail;
                    System.Diagnostics.Debug.Print("2D Viewer register fail");
                    pRegister.Outputs.AddParameter("Register", "register 2D Viewer", registerResult.Message);

                    SaveRound(r);
                    break; // There is error, end test case
                }
                else
                {
                    pRegister.Result = TestResult.Pass;
                    System.Diagnostics.Debug.Print("2D Viewer register succeed");
                    pRegister.Outputs.AddParameter("registerApplication", "Register 2D Viewer", registerResult.Message);
                }
                #endregion

                #region Step 2: call queryPatients to get the patient list
                XMLResult queryPatientsResult = patientService.queryPatients(queryPatientsParameter);

                if (queryPatientsResult.IsErrorOccured)
                {
                    pQueryPatients.Result = TestResult.Fail;
                    System.Diagnostics.Debug.Print("query the patients list fail:");
                    pQueryPatients.Outputs.AddParameter("queryPatients", "query the patients list", queryPatientsResult.Message);

                    SaveRound(r);
                    break; // There is error, end test case
                }
                else
                {
                    pQueryPatients.Result = TestResult.Pass;
                    System.Diagnostics.Debug.Print("query the patients list succeed:");
                    pQueryPatients.Outputs.AddParameter("queryPatients", "query the patients list", queryPatientsResult.Message);
                }
                #endregion

                #region Step 3: call queryDevices to query installed device IDs of sensor type
                XMLResult queryDevicesResult = acquisitionService.queryDevices(queryDivicesParameter);

                if (queryDevicesResult.IsErrorOccured)
                {
                    pQueryDevices.Result = TestResult.Fail;
                    System.Diagnostics.Debug.Print("query installed device IDs of sensor type fail:");
                    pQueryDevices.Outputs.AddParameter("queryDevices", "query installed device IDs of sensor type", queryDevicesResult.Message);

                    SaveRound(r);
                    break; // There is error, end test case
                }
                else
                {
                    pQueryDevices.Result = TestResult.Pass;
                    System.Diagnostics.Debug.Print("query installed device IDs of sensor type succeed:");
                    pQueryDevices.Outputs.AddParameter("queryDevices", "query installed device IDs of sensor type", queryDevicesResult.Message);
                }
                #endregion

                #region Step 4: call queryLines to query installed line IDs of device IDs
                // Get the line ID sub strings from queryDevicesResult, e.g: <device id="AcqCR7400.dll">
                string pattern = "<device id=\"\\S*\">";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                System.Text.RegularExpressions.MatchCollection matches = regex.Matches(queryDevicesResult.ResultContent);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    // Get the exact device ID from the result, e.g: from <device id="AcqCR7400.dll"> to AcqCR7400.dll
                    string deviceID = match.Value;
                    deviceID = deviceID.Replace("<device id=\"", "");
                    deviceID = deviceID.Replace("\">", "");

                    queryLinesParameter.AddParameter("id", deviceID);
                }

                XMLResult queryLinesResult = acquisitionService.queryLines(queryLinesParameter);

                if (queryLinesResult.IsErrorOccured)
                {
                    pQueryLines.Result = TestResult.Fail;
                    System.Diagnostics.Debug.Print("query installed line IDs of device IDs fail:");
                    pQueryLines.Outputs.AddParameter("queryLines", "query installed line IDs of device IDs", queryLinesResult.Message);

                    SaveRound(r);
                    break; // There is error, end test case
                }
                else
                {
                    pQueryLines.Result = TestResult.Pass;
                    System.Diagnostics.Debug.Print("query installed line IDs of device IDs succeed:");
                    pQueryLines.Outputs.AddParameter("queryLines", "query installed line IDs of device IDs", queryLinesResult.Message);
                }
                #endregion

                #region Step 5: setAsynAcqPatientInfo with empty patient
                setAsynAcqPatientInfoParameter.AddParameter("patient_internal_id", "");
                XMLResult setAsynAcqPatientInfoResult = acquisitionService.setAsynAcqPatientInfo(setAsynAcqPatientInfoParameter);

                if (setAsynAcqPatientInfoResult.IsErrorOccured)
                {
                    pSetAsynAcqPatientInfo.Result = TestResult.Fail;

                    System.Diagnostics.Debug.Print("Set AsynAcq PatientInfo fail:");
                    pSetAsynAcqPatientInfo.Outputs.AddParameter("setAsynAcqPatientInfo", "Set AsynAcq PatientInfo", setAsynAcqPatientInfoResult.Message);

                    SaveRound(r);
                    break; // There is error, end test case
                }
                else
                {
                    pSetAsynAcqPatientInfo.Result = TestResult.Pass;

                    System.Diagnostics.Debug.Print("Set AsynAcq PatientInfo succeed:");
                    pSetAsynAcqPatientInfo.Outputs.AddParameter("setAsynAcqPatientInfo", "Set AsynAcq PatientInfo", setAsynAcqPatientInfoResult.Message);
                }
                #endregion

                SaveRound(r);

            }

            Output();
        }

        public void Run_WorkFlow_CSIOpenImageAndClose_Case980() //Case 980: 1.3.17_workflow_CSI select a patient, open an image and then close it
        {
            // Test Case added for work flow: CSI select a patient to list all the image, click to open one of them and then save and close it.

            PatientService patientService = new PatientService();
            AcquisitionService acquisitionService = new AcquisitionService();
            PresentationStateService presentationStateService = new PresentationStateService();
            AnalysisService analysisService = new AnalysisService();

            string patientUID = null;
            XMLParameter setAsynAcqPatientInfoParam = new XMLParameter("acq_info");
            XMLParameter listObjectForPSParam = new XMLParameter("filter");
            XMLParameterCollection getPresentionStateInfoParam = new XMLParameterCollection();
            XMLParameter listObjectForVolumeParam = new XMLParameter("filter");
            XMLParameter listObjectForFMSParam = new XMLParameter("filter");
            XMLParameter listObjectForOtherParam = new XMLParameter("filter");

            XMLParameter setPresentationStateParam = new XMLParameter("presentationstate");
            XMLParameter setPresentationStateInfoParam = new XMLParameter("presentationstate");

            XMLParameter listObjectForAnalysisParam = new XMLParameter("filter");
            XMLParameter setAnalysisInfoParam = new XMLParameter("analysis");

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = new Round();

                try
                {
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getPatient")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalID")
                            {
                                patientUID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "setAsynAcqPatientInfo")
                        {
                            setAsynAcqPatientInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "listObjectForPS")
                        {
                            listObjectForPSParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "listObjectForVolume")
                        {
                            listObjectForVolumeParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "listObjectForFMS")
                        {
                            listObjectForFMSParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "listObjectForOther")
                        {
                            listObjectForOtherParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "listObjectForAnalysis")
                        {
                            listObjectForAnalysisParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "setPresentationStateInfo")
                        {
                            setPresentationStateInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "setAnalysisInfo")
                        {
                            setAnalysisInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    #region Step 1: Check the specific patient exists in Database
                    CheckPoint pGetPatient = new CheckPoint("getPatient", "Step 1: Get the specific patient info");
                    r.CheckPoints.Add(pGetPatient);

                    XMLResult getPatientResult = patientService.getPatient(patientUID);

                    if (getPatientResult.IsErrorOccured)
                    {
                        pGetPatient.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Get the specific patient info fail:");
                        pGetPatient.Outputs.AddParameter("getPatient", "get the specific patient info", getPatientResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pGetPatient.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Get the specific patient info succeed:");
                        pGetPatient.Outputs.AddParameter("getPatient", "get the specific patient info", getPatientResult.Message);
                    }
                    #endregion


                    // Step 2 - Step 7 are what to do after select a patient

                    #region Step 2: set asyn acq patient with the specific patient
                    CheckPoint pSetAsynAcqPatientInfo = new CheckPoint("setAsynAcqPatientInfo", "Step 2: call setAsynAcqPatientInfo to set asyn acq patient info");
                    r.CheckPoints.Add(pSetAsynAcqPatientInfo);

                    XMLResult setAsynAcqPatientInfoResult = acquisitionService.setAsynAcqPatientInfo(setAsynAcqPatientInfoParam);

                    if (setAsynAcqPatientInfoResult.IsErrorOccured)
                    {
                        pSetAsynAcqPatientInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Set asyn acq PatientInfo fail:");
                        pSetAsynAcqPatientInfo.Outputs.AddParameter("setAsynAcqPatientInfo", "set asyn acq patient info", setAsynAcqPatientInfoResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pSetAsynAcqPatientInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Set asyn acq PatientInfo succeed:");
                        pSetAsynAcqPatientInfo.Outputs.AddParameter("setAsynAcqPatientInfo", "set asyn acq patient info", setAsynAcqPatientInfoResult.Message);
                    }
                    #endregion

                    #region Step 3: Call listObject to get the presentation state list of the patient
                    CheckPoint pListObjectForPS = new CheckPoint("listObjectForPS", "Step 3: call listObject to get PS info");
                    r.CheckPoints.Add(pListObjectForPS);

                    XMLResult listObjectForPSResult = patientService.listObjects(listObjectForPSParam);

                    if (listObjectForPSResult.IsErrorOccured)
                    {
                        pListObjectForPS.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call listObject to get PS info fail:");
                        pListObjectForPS.Outputs.AddParameter("listObject", "listObject to get PS", listObjectForPSResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pListObjectForPS.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call listObject to get PS info succeed:");
                        pListObjectForPS.Outputs.AddParameter("listObject", "listObject to get PS", listObjectForPSResult.Message);

                    }
                    #endregion

                    #region Step 4: Call getPresentationStateInfo to get the presentationstate info according to the presentationstate internal_id

                    CheckPoint pGetPresentionStateInfo = new CheckPoint("getPresentionStateInfo", "Step 4: call getPresentationStateInfo to get presention state info");
                    r.CheckPoints.Add(pGetPresentionStateInfo);

                    foreach (XMLParameter param in listObjectForPSResult.MultiResults)
                    {
                        getPresentionStateInfoParam.Add(param);
                    }

                    XMLResult getPresentationStateInfoResult = presentationStateService.getPresentationStateInfo(getPresentionStateInfoParam);

                    if (getPresentationStateInfoResult.IsErrorOccured)
                    {
                        pGetPresentionStateInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call getPresentationStateInfo to get PS info fail:");
                        pGetPresentionStateInfo.Outputs.AddParameter("getPresentationStateInfo", "getPresentationStateInfo to get PS info", getPresentationStateInfoResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pGetPresentionStateInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call getPresentationStateInfo to get PS info succeed:");
                        pGetPresentionStateInfo.Outputs.AddParameter("getPresentationStateInfo", "getPresentationStateInfo to get PS info", getPresentationStateInfoResult.Message);

                    }
                    #endregion

                    #region Step 5: Call listObject to get the volume list of the patient
                    CheckPoint pListObjectForVolume = new CheckPoint("listObjectForVolume", "Step 5: call listObject to get volume info");
                    r.CheckPoints.Add(pListObjectForVolume);

                    XMLResult listObjectForVolumeResult = patientService.listObjects(listObjectForVolumeParam);

                    if (listObjectForVolumeResult.IsErrorOccured)
                    {
                        pListObjectForVolume.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call listObject to get volume info fail:");
                        pListObjectForVolume.Outputs.AddParameter("listObject", "listObject to get volume", listObjectForVolumeResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pListObjectForVolume.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call listObject to get volume info succeed:");
                        pListObjectForVolume.Outputs.AddParameter("listObject", "listObject to get volume", listObjectForVolumeResult.Message);

                    }
                    #endregion

                    #region Step 6: Call listObject to get the FMS list of the patient
                    CheckPoint pListObjectForFMS = new CheckPoint("listObjectForFMS", "Step 6: call listObject to get FMS info");
                    r.CheckPoints.Add(pListObjectForFMS);

                    XMLResult listObjectForFMSResult = patientService.listObjects(listObjectForFMSParam);

                    if (listObjectForFMSResult.IsErrorOccured)
                    {
                        pListObjectForFMS.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call listObject to get FMS info fail:");
                        pListObjectForFMS.Outputs.AddParameter("listObject", "listObject to get FMS", listObjectForFMSResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pListObjectForFMS.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call listObject to get FMS info succeed:");
                        pListObjectForFMS.Outputs.AddParameter("listObject", "listObject to get FMS", listObjectForFMSResult.Message);

                    }
                    #endregion

                    #region Step 7: Call listObject to get the other info of the patient
                    CheckPoint pListObjectForOther = new CheckPoint("listObjectForOther", "Step 7: call listObject to get other info");
                    r.CheckPoints.Add(pListObjectForOther);

                    XMLResult listObjectForOtherResult = patientService.listObjects(listObjectForOtherParam);

                    if (listObjectForOtherResult.IsErrorOccured)
                    {
                        pListObjectForOther.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call listObject to get other info fail:");
                        pListObjectForOther.Outputs.AddParameter("listObject", "listObject to get other", listObjectForOtherResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pListObjectForOther.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call listObject to get other info succeed:");
                        pListObjectForOther.Outputs.AddParameter("listObject", "listObject to get other", listObjectForOtherResult.Message);

                    }
                    #endregion


                    // After double click an image to open it, it will repeat step 3 to step 7
                    // When open image, it will call getPresentationState to get the presentation state, which can be used in setPresentationState step later
                    string presentationStateInternalID = null;
                    XMLParameterCollection setPresentationStateParamList = new XMLParameterCollection();

                    foreach (XMLParameterNode psNode in listObjectForPSResult.ArrayResult.Parameters)
                    {
                        presentationStateInternalID = psNode.ParameterValue;

                        XMLResult getPresentationStateResult = presentationStateService.getPresentationState(presentationStateInternalID);

                        setPresentationStateParam = getPresentationStateResult.ArrayResult;
                        setPresentationStateParamList.Add(setPresentationStateParam);
                    }



                    // Close the image, it will do step 8 to step 10
                    #region Step 8: Call setPresentationState to set presentation state, not change the presentation state value, just set it back
                    CheckPoint pSetPresentationState = new CheckPoint("setPresentationState", "Step 8: call setPresentationState to set the presentation state after close the image");
                    r.CheckPoints.Add(pSetPresentationState);

                    XMLResult setPresentationStateResult = new XMLResult();

                    foreach (XMLParameter presentationStateInfo in setPresentationStateParamList)
                    {
                        setPresentationStateResult = presentationStateService.setPresentationState(presentationStateInfo, presentationStateInternalID);

                        if (setPresentationStateResult.IsErrorOccured)
                        {
                            pSetPresentationState.Result = TestResult.Fail;
                            break;
                        }
                        else
                        {
                            pSetPresentationState.Result = TestResult.Pass;
                        }

                    }

                    if (pSetPresentationState.Result == TestResult.Fail)
                    {
                        System.Diagnostics.Debug.Print("Call setPresentationState to set presentation state fail:");
                        pSetPresentationState.Outputs.AddParameter("setPresentationState", "setPresentationState to set presentation state", setPresentationStateResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Print("Call setPresentationState to set presentation state succeed:");
                        pSetPresentationState.Outputs.AddParameter("setPresentationState", "setPresentationState to set presentation state", setPresentationStateResult.Message);
                    }
                    #endregion

                    #region Step 9: Call setPresentationStateInfo to set the presentation state info
                    CheckPoint pSetPresentationStateInfo = new CheckPoint("setPresentationStateInfo", "Step 9: call setPresentationStateInfo to set the presentation state info after close the image");
                    r.CheckPoints.Add(pSetPresentationStateInfo);

                    XMLResult setPresentationStateInfoResult = new XMLResult();

                    foreach (XMLParameter presentationStateInfoParam in getPresentationStateInfoResult.MultiResults)
                    {
                        presentationStateInternalID = presentationStateInfoParam.GetParameterValueByName("internal_id");

                        setPresentationStateInfoResult = presentationStateService.setPresentationStateInfo(setPresentationStateInfoParam, presentationStateInternalID);

                        if (setPresentationStateInfoResult.IsErrorOccured)
                        {
                            pSetPresentationStateInfo.Result = TestResult.Fail;
                            break;
                        }
                        else
                        {
                            pSetPresentationStateInfo.Result = TestResult.Pass;
                        }
                    }


                    if (pSetPresentationStateInfo.Result == TestResult.Fail)
                    {
                        System.Diagnostics.Debug.Print("Call setPresentationStateInfo to set presentation state info fail:");
                        pSetPresentationStateInfo.Outputs.AddParameter("setPresentationStateInfo", "setPresentationState to set presentation state info", setPresentationStateInfoResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Print("Call setPresentationStateInfo to set presentation state info succeed:");
                        pSetPresentationStateInfo.Outputs.AddParameter("setPresentationState", "setPresentationState to set presentation state info", setPresentationStateInfoResult.Message);
                    }
                    #endregion

                    #region Step 10: Call listObject to get the presentation state
                    CheckPoint pListObjectForPS2 = new CheckPoint("listObjectForPS", "Step 10: call listObject to get PS info after close image");
                    r.CheckPoints.Add(pListObjectForPS2);

                    XMLResult listObjectForPSResult2 = patientService.listObjects(listObjectForPSParam);

                    if (listObjectForPSResult2.IsErrorOccured)
                    {
                        pListObjectForPS.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call listObject to get PS info fail:");
                        pListObjectForPS2.Outputs.AddParameter("listObject", "listObject to get PS", listObjectForPSResult2.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pListObjectForPS2.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call listObject to get PS info succeed:");
                        pListObjectForPS2.Outputs.AddParameter("listObject", "listObject to get PS", listObjectForPSResult2.Message);

                    }
                    #endregion

                    #region Step 11: Call getPresentationStateInfo to get the presentation state info
                    CheckPoint pGetPresentionStateInfo2 = new CheckPoint("getPresentionStateInfo", "Step 11: call getPresentationStateInfo to get presention state info after close image");
                    r.CheckPoints.Add(pGetPresentionStateInfo2);

                    XMLResult getPresentationStateInfoResult2 = presentationStateService.getPresentationStateInfo(getPresentionStateInfoParam);

                    if (getPresentationStateInfoResult2.IsErrorOccured)
                    {
                        pGetPresentionStateInfo2.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call getPresentationStateInfo to get PS info fail:");
                        pGetPresentionStateInfo2.Outputs.AddParameter("getPresentationStateInfo", "getPresentationStateInfo to get PS info", getPresentationStateInfoResult2.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pGetPresentionStateInfo2.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call getPresentationStateInfo to get PS info succeed:");
                        pGetPresentionStateInfo2.Outputs.AddParameter("getPresentationStateInfo", "getPresentationStateInfo to get PS info", getPresentationStateInfoResult2.Message);

                    }
                    #endregion


                    // Close the viewer
                    #region Step 12: Call listObject to get the analysis
                    CheckPoint pListObjectForAnalysis = new CheckPoint("listObjectForAnalysis", "Step 12: call listObject to get analysis info");
                    r.CheckPoints.Add(pListObjectForAnalysis);

                    XMLResult listObjectForAnalysisResult = patientService.listObjects(listObjectForAnalysisParam);

                    if (listObjectForAnalysisResult.IsErrorOccured)
                    {
                        pListObjectForAnalysis.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call listObject to get analysis info fail:");
                        pListObjectForAnalysis.Outputs.AddParameter("listObject", "listObject to get analysis", listObjectForAnalysisResult.Message);

                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        pListObjectForAnalysis.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call listObject to get analysis info succeed:");
                        pListObjectForAnalysis.Outputs.AddParameter("listObject", "listObject to get analysis", listObjectForAnalysisResult.Message);
                    }
                    #endregion


                    string analysisId = null;
                    analysisId = listObjectForAnalysisResult.SingleResult;

                    if (analysisId == null || analysisId == string.Empty)
                    {
                        // Below Step are excuted when there is no analysis before

                        #region Step 13: createAnalysis
                        CheckPoint pCreateAnalysis = new CheckPoint("createAnalysis", "Step 13: call createAnalysis to create new analysis");
                        r.CheckPoints.Add(pCreateAnalysis);

                        XMLResult createAnalysisResult = new XMLResult();

                        XMLParameter analysisXml = new XMLParameter("analysis");
                        XMLParameterCollection uidsXml = new XMLParameterCollection();
                        XMLParameter presentationStateUids = new XMLParameter("presentationstate");
                        XMLParameter fmsUids = new XMLParameter("fms");
                        uidsXml.Add(presentationStateUids);
                        uidsXml.Add(fmsUids);

                        createAnalysisResult = analysisService.createAnalysis(analysisXml.GenerateXML().ToString(), true, true, patientUID, null, uidsXml);
                        // public XMLResult createAnalysis(string analysisXml, bool current, bool currentSpecified, string patientInternalID, string thumbnail, XMLParameterCollection uidsXml);
                        if (createAnalysisResult.IsErrorOccured)
                        {
                            System.Diagnostics.Debug.Print("Call createAnalysis to create analysis fail:");
                            pCreateAnalysis.Outputs.AddParameter("createAnalysis", "createAnalysis to create analysis", createAnalysisResult.Message);

                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Print("Call createAnalysis to create analysis succeed:");
                            pCreateAnalysis.Outputs.AddParameter("createAnalysis", "createAnalysis to create analysis", createAnalysisResult.Message);
                        }
                        #endregion

                        #region Step 14: Call AnalysisService.setAnalysisInfo to set the analysis info
                        CheckPoint pSetAnalysisInfo = new CheckPoint("setAnalysisInfo", "Step 14: call setAnalysisInfo to set the analysis info");
                        r.CheckPoints.Add(pSetAnalysisInfo);

                        XMLResult setAnalysisInfoResult = new XMLResult();

                        analysisId = createAnalysisResult.MultiResults[0].GetParameterValueByName("internal_id");

                        setAnalysisInfoResult = analysisService.setAnalysisInfo(analysisId, setAnalysisInfoParam);

                        if (setAnalysisInfoResult.IsErrorOccured)
                        {
                            System.Diagnostics.Debug.Print("Call setAnalysisInfo to set analysis info fail:");
                            pSetAnalysisInfo.Outputs.AddParameter("setAnalysisInfo", "setAnalysisInfo to set analysis info", setAnalysisInfoResult.Message);

                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Print("Call setAnalysisInfo to set analysis info succeed:");
                            pSetAnalysisInfo.Outputs.AddParameter("setAnalysisInfo", "setAnalysisInfo to set analysis info", setAnalysisInfoResult.Message);
                        }
                        #endregion
                    }

                    else
                    {
                        // Below Step 15 to Step 17 are excuted when there is analysis exist before...

                        #region Step 15: Call AnalysisService.getAnalysisInfo to get the analysis info
                        CheckPoint pGetAnalysisInfo = new CheckPoint("getAnalysisInfo", "Step 15: call getAnalysisInfo to get the analysis info");
                        r.CheckPoints.Add(pGetAnalysisInfo);

                        XMLResult getAnalysisInfoResult = new XMLResult();

                        getAnalysisInfoResult = analysisService.getAnalysisInfo(analysisId);

                        if (getAnalysisInfoResult.IsErrorOccured)
                        {
                            pGetAnalysisInfo.Result = TestResult.Fail;
                            System.Diagnostics.Debug.Print("Call getAnalysisInfo to get analysis info fail:");
                            pGetAnalysisInfo.Outputs.AddParameter("getAnalysisInfo", "getAnalysisInfo to get analysis info", getAnalysisInfoResult.Message);

                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            pGetAnalysisInfo.Result = TestResult.Pass;
                            System.Diagnostics.Debug.Print("Call getAnalysisInfo to get analysis info succeed:");
                            pGetAnalysisInfo.Outputs.AddParameter("getAnalysisInfo", "getAnalysisInfo to get analysis info", getAnalysisInfoResult.Message);
                        }
                        #endregion

                        #region Step 16: Call AnalysisService.setAnalysisDescription to set analysis description
                        CheckPoint pSetAnalysisDescription = new CheckPoint("setAnalysisDescription", "Step 16: call setAnalysisDescription to set the analysis description");
                        r.CheckPoints.Add(pSetAnalysisDescription);

                        XMLResult setAnalysisDescriptionResult = new XMLResult();

                        XMLParameter analysisXml = new XMLParameter("analysis");
                        XMLParameterCollection uidsXml = new XMLParameterCollection();
                        XMLParameter presentationStateUids = new XMLParameter("presentationstate");
                        XMLParameter fmsUids = new XMLParameter("fms");
                        uidsXml.Add(presentationStateUids);
                        uidsXml.Add(fmsUids);

                        setAnalysisDescriptionResult = analysisService.setAnalysisDescription(analysisId, analysisXml.GenerateXML(), false, uidsXml);

                        if (setAnalysisDescriptionResult.IsErrorOccured)
                        {
                            pSetAnalysisDescription.Result = TestResult.Fail;
                            System.Diagnostics.Debug.Print("Call setAnalysisDescription to set the analysis description fail:");
                            pSetAnalysisDescription.Outputs.AddParameter("setAnalysisDescription", "call set the analysis description", setAnalysisDescriptionResult.Message);

                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            pSetAnalysisDescription.Result = TestResult.Pass;
                            System.Diagnostics.Debug.Print("Call setAnalysisDescription to set the analysis description succeed:");
                            pSetAnalysisDescription.Outputs.AddParameter("setAnalysisDescription", "call set the analysis description", setAnalysisDescriptionResult.Message);
                        }
                        #endregion

                        #region Step 17: Call AnalysisService.setAnalysisInfo to set the analysis info
                        CheckPoint pSetAnalysisInfo = new CheckPoint("setAnalysisInfo", "Step 17: call setAnalysisInfo to set the analysis info");
                        r.CheckPoints.Add(pSetAnalysisInfo);

                        XMLResult setAnalysisInfoResult = new XMLResult();

                        setAnalysisInfoResult = analysisService.setAnalysisInfo(analysisId, setAnalysisInfoParam);

                        if (setAnalysisInfoResult.IsErrorOccured)
                        {
                            pSetAnalysisInfo.Result = TestResult.Fail;
                            System.Diagnostics.Debug.Print("Call setAnalysisInfo to set analysis info fail:");
                            pSetAnalysisInfo.Outputs.AddParameter("setAnalysisInfo", "setAnalysisInfo to set analysis info", setAnalysisInfoResult.Message);

                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            pSetAnalysisInfo.Result = TestResult.Pass;
                            System.Diagnostics.Debug.Print("Call setAnalysisInfo to set analysis info succeed:");
                            pSetAnalysisInfo.Outputs.AddParameter("setAnalysisInfo", "setAnalysisInfo to set analysis info", setAnalysisInfoResult.Message);
                        }
                        #endregion
                    }

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_WorkFlow_PostImportWithStompNotification_Case1016() //Case 1016: 1.3.17_workflow_PostImportWithStompNotification
        {
            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            //Generic notification to used to stop the notification receiver
            string stopMessageType = "Stop Receiving the Notification";
            string stopMessageContent = "Stop Receiving the Notification";

            // Stepup the notification receiver
            NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification();
            //NotificationSim.ReceiveNotification rn = new NotificationSim.ReceiveNotification("10.112.39.167");
            System.Collections.Generic.List<string> rms = new System.Collections.Generic.List<string>();
            rms.Add("topic.patientCreated");
            rms.Add("topic.postImportStarted");
            rms.Add("topic.postImportCompleted");
            rms.Add("topic.fmsCreated");
            rms.Add("topic.imageCreated");
            rms.Add("topic.simpleInstanceCreated");
            rms.Add("topic.presentationStateInfoModified");
            rn.startListon(rms, stopMessageType, stopMessageContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p1 = new CheckPoint("createPatient", "Setup environment of createPatient");
                CheckPoint p2 = new CheckPoint("initListObjects", "First result of listObjects for the postImport");
                CheckPoint p3 = new CheckPoint("endListObjects", "End result of listObjects for the postImport");
                CheckPoint p4 = new CheckPoint("stompNotification", "Number of received stomp notification during postImport");

                r.CheckPoints.Add(p1);
                r.CheckPoints.Add(p2);
                r.CheckPoints.Add(p3);
                r.CheckPoints.Add(p4);

                //create required PAS service instaces here
                PatientService pats = new PatientService();
                ApplicationService ass = new ApplicationService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa1 = new XMLParameter("createPatient");
                XMLParameter pa2 = new XMLParameter("listObjects");


                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "createPatient":
                            pa1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "listObjects":
                            pa2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        default:
                            Console.WriteLine("There is no valid selection when parse input parameters.");
                            break;
                    }
                }

                //If we need change parameter by specific logic, please put code here

                //Get service result
                //Step1 result

                XMLResult step1_result = pats.createPatient(pa1);

                //Log step1 service output
                if (step1_result.IsErrorOccured)
                {
                    p1.Result = TestResult.Fail;
                    p1.Outputs.AddParameter("createPatient", "Error", step1_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p1.Result = TestResult.Pass;
                p1.Outputs.AddParameter("createPatient", "Success", step1_result.ResultContent);


                //Change input parameter according the output of Step1
                pa2.AddParameter("patient_internal_id", step1_result.SingleResult);

                //Step2 result

                XMLResult step2_result = pats.listObjects(pa2);

                //Log step2 service output
                if (step2_result.Code != 800)
                {
                    p2.Result = TestResult.Fail;
                    p2.Outputs.AddParameter("initListObjects", "Error", step2_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p2.Result = TestResult.Pass;
                p2.Outputs.AddParameter("initListObjects", "Success", step2_result.ResultContent);


                //Change input parameter according the output of Step2


                //Step3 result
                XMLResult step3_result = new XMLResult();
                int countOfListObjectsLoop = 0;

                do
                {
                    System.Threading.Thread.Sleep(30000);
                    countOfListObjectsLoop++;
                    step3_result = pats.listObjects(pa2);

                    //Log step3 service output
                    if (step3_result.Code == 0)
                    {
                        p3.Result = TestResult.Pass;
                        p3.Outputs.AddParameter("endListObjects", "Success", step3_result.ResultContent);
                        break;
                    }

                    if (step3_result.IsErrorOccured && countOfListObjectsLoop > 2)
                    {
                        p3.Result = TestResult.Fail;
                        p3.Outputs.AddParameter("endListObjects", "Error", step3_result.ResultContent);
                        SaveRound(r);
                        break;
                    }

                } while (true);

                //Change input parameter according the output of Step3

                //Step4 result

                System.Threading.Thread.Sleep(500);

                ass.sendGenericNotification(stopMessageType, stopMessageContent);

                System.Threading.Thread.Sleep(1000);

                int step4_result = rn.getNotificationNumber() - 1;

                //Log step4 service output

                if (ids.ExpectedValues.GetParameter("NumOfNotification").Value == step4_result.ToString())
                {
                    p4.Result = TestResult.Pass;
                    foreach (string notification in rn.getNotificationContent())
                    {
                        p4.Outputs.AddParameter("Received notification number and content: " + rn.getNotificationContent().IndexOf(notification), "Success", notification);
                    }
                }
                else
                {
                    p4.Result = TestResult.Fail;
                    p4.Outputs.AddParameter("Received notification number", "Error", step4_result.ToString());
                    foreach (string notification in rn.getNotificationContent())
                    {
                        p4.Outputs.AddParameter("Received notification number and content: " + rn.getNotificationContent().IndexOf(notification), "Error", notification);
                    }
                }

                //Save data for each round
                SaveRound(r);
            }

            //Save service log as xml file
            Output();
        }

        public void Run_WorkFlow_ImportCeph_RadioLog_Case1043() //Case 1043: 1.3.17_workflow_ImportCeph_GetImageInfo_RadilogIsNotSaved
        {
            int runCount = 0;
            string patientUID = string.Empty;
            bool isCreatePatient = false;
            bool isDeletePatient = true;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Import image");

                PatientService patientSvc = new PatientService();
                ImportService importSvc = new ImportService();
                ImageService imageSvc = new ImageService();

                string radioLogBeforeImport = string.Empty;
                string radioLogAfterImport = string.Empty;
                string imageID = null;

                XMLParameter pa = new XMLParameter("patient");
                XMLParameter ia = new XMLParameter("import");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "import")
                    {
                        ia.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                XMLParameter epGetImagInfo = new XMLParameter();
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo")
                    {
                        epGetImagInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                }

                try
                {
                    #region Step1: Create Image
                    if (isCreatePatient)
                    {
                        CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                        r.CheckPoints.Add(pCreate);

                        XMLResult rtCreatePatient = patientSvc.createPatient(pa);
                        if (!rtCreatePatient.IsErrorOccured)
                        {
                            patientUID = rtCreatePatient.SingleResult;
                            pCreate.Outputs.AddParameter("Create patient UID", "Create Patient", patientUID);
                            pCreate.Result = TestResult.Pass;
                        }
                    }
                    #endregion

                    #region Step2: Get patient radio log info before import
                    CheckPoint cpGetPatientBeforeImport = new CheckPoint("GetPatient", "Get Patient Info before import image");
                    r.CheckPoints.Add(cpGetPatientBeforeImport);

                    XMLResult rtGetPatientBeforeImport = patientSvc.getPatient(patientUID);
                    if (rtGetPatientBeforeImport.IsErrorOccured)
                    {
                        cpGetPatientBeforeImport.Result = TestResult.Fail;
                        cpGetPatientBeforeImport.Outputs.AddParameter("Get Patient Info before import image returns error", "GetPatient", rtGetPatientBeforeImport.ResultContent);
                    }
                    else
                    {
                        cpGetPatientBeforeImport.Result = TestResult.Pass;
                        cpGetPatientBeforeImport.Outputs.AddParameter("Get Patient Info before import image returns ok", "GetPatient", rtGetPatientBeforeImport.ResultContent);

                        radioLogBeforeImport = rtGetPatientBeforeImport.MultiResults[0].GetParameterValueByName("cumulative_dose");  //get the cumulative_dose info
                    }
                    #endregion

                    #region Step3: Import Image
                    CheckPoint pImport = new CheckPoint("Import Image", "Test import");
                    r.CheckPoints.Add(pImport);

                    string filePath = string.Empty;
                    string archivePath = string.Empty;
                    bool move = false;
                    for (int c = 0; c < ia.Length; c++)
                    {
                        if (ia.GetParameterName(c) == "path")
                        {
                            filePath = ia.GetParameterValue(c);
                        }
                        else if (ia.GetParameterName(c) == "archivePath")
                        {
                            archivePath = ia.GetParameterValue(c);
                        }
                        else if (ia.GetParameterName(c) == "move")
                        {
                            if (ia.GetParameterValue(c) == "true")
                            {
                                move = true;
                            }
                        }
                    }

                    XMLResult rtlImport = importSvc.importObject(patientUID, "", filePath, archivePath, move, "false");

                    //import return sample:
                    //   <trophy type="result" version="1.0">
                    //           <status code="0" message="ok" />
                    //           <image><parameter key="internal_id" value="2bfb416b-037e-41e7-aaef-8d2bd08b1ae7" /></image>
                    //           <presentationstate><parameter key="internal_id" value="75675345-a164-4088-b6d3-1b2ae86b703b" /></presentationstate>
                    // </trophy>

                    if (rtlImport.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;
                        pImport.Outputs.AddParameter("Import image fail", "Import", rtlImport.Message);
                        pImport.Outputs.AddParameter("Import image returns error code", "Import", rtlImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains image id and ps id
                        imageID = rtlImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (imageID == null || imageID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("Import image returns wrong image internal id", "Import", rtlImport.ResultContent);
                        }

                        string psID = null;
                        psID = rtlImport.MultiResults[1].GetParameterValueByName("internal_id"); // ps internal_id
                        if (psID == null || psID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("import image returns wrong ps internal id", "Import", rtlImport.ResultContent);
                        }
                        else
                        {
                            pImport.Result = TestResult.Pass;
                            pImport.Outputs.AddParameter("import image returns OK", "Import", rtlImport.ResultContent);
                        }
                    }
                    #endregion

                    #region Step4: Call GetImageInfo to check the radio log info
                    CheckPoint cpGetImageInfo = new CheckPoint("GetImageInfo", "Get the image info of the imported image");
                    r.CheckPoints.Add(cpGetImageInfo);

                    XMLParameter pGetImageInfo = new XMLParameter("image");
                    pGetImageInfo.AddParameter("internal_id", imageID);
                    XMLResult rtGetImageInfo = imageSvc.getImageInfo(pGetImageInfo);

                    if (rtGetImageInfo.IsErrorOccured)
                    {
                        cpGetImageInfo.Result = TestResult.Fail;
                        cpGetImageInfo.Outputs.AddParameter("Get the image info of the imported image return error", "GetImageInfo", rtGetImageInfo.ResultContent);
                    }
                    else
                    {
                        cpGetImageInfo.Outputs.AddParameter("Get Image Info return success", "getImageInfo", rtGetImageInfo.Message);

                        //Check the return info contain the expected key and value: <parameter key="XXX" value="XXX" />
                        foreach (XMLParameterNode node in epGetImagInfo.Parameters)
                        {
                            if (!rtGetImageInfo.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                            {
                                cpGetImageInfo.Result = TestResult.Fail;
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info returns wrong key-value info for: " + node.ParameterName, "GetImageInfo", rtGetImageInfo.ResultContent);
                                continue;
                            }
                            else
                            {
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info returns correct key-value info for: " + node.ParameterName, "GetImageInfo", "ok");
                            }
                        }

                        if (cpGetImageInfo.Result != TestResult.Fail)
                        {
                            cpGetImageInfo.Result = TestResult.Pass;
                            cpGetImageInfo.Outputs.AddParameter("Get Image Info return all correct info", "GetImageInfo", rtGetImageInfo.ResultContent);
                        }
                    }
                    #endregion

                    #region Step5: Get patient radio log info after import
                    CheckPoint cpGetPatientAfterImport = new CheckPoint("GetPatient", "Get Patient Info after import image");
                    r.CheckPoints.Add(cpGetPatientAfterImport);

                    XMLResult rtGetPatientAfterImport = patientSvc.getPatient(patientUID);
                    if (rtGetPatientAfterImport.IsErrorOccured)
                    {
                        cpGetPatientAfterImport.Result = TestResult.Fail;
                        cpGetPatientAfterImport.Outputs.AddParameter("Get Patient Info after import image returns error", "GetPatient", rtGetPatientAfterImport.ResultContent);
                    }
                    else
                    {
                        cpGetPatientAfterImport.Result = TestResult.Pass;
                        cpGetPatientAfterImport.Outputs.AddParameter("Get Patient Info after import image returns ok", "GetPatient", rtGetPatientAfterImport.ResultContent);

                        radioLogAfterImport = rtGetPatientAfterImport.MultiResults[0].GetParameterValueByName("cumulative_dose");  //get the cumulative_dose info
                    }
                    #endregion

                    #region Step6: Check the patient radio log info not changes after import image
                    CheckPoint cpPatientRadioLogInfo = new CheckPoint("Patient Radiolog Info", "Check patient radio log info not changes aftert import image");
                    r.CheckPoints.Add(cpPatientRadioLogInfo);

                    if (radioLogAfterImport == radioLogBeforeImport)
                    {
                        cpPatientRadioLogInfo.Result = TestResult.Pass;
                        cpPatientRadioLogInfo.Outputs.AddParameter("Patient radio log info not changes aftert import image", "Patient Radiolog Info", "Before import value is: " + radioLogBeforeImport + ". After import value is: " + radioLogAfterImport);
                    }
                    else
                    {
                        cpPatientRadioLogInfo.Result = TestResult.Fail;
                        cpPatientRadioLogInfo.Outputs.AddParameter("Patient radio log info  changes aftert import image", "Patient Radiolog Info", "Before import value is: " + radioLogBeforeImport + ". After import value is: " + radioLogAfterImport);
                    }
                    #endregion

                    #region Step7: Delete Patient
                    if (isDeletePatient)
                    {
                        CheckPoint cpDelete = new CheckPoint("Delete Patient", "Test delete patient");
                        r.CheckPoints.Add(cpDelete);
                        XMLResult rltDelete = patientSvc.deletePatient(patientUID);
                        if (rltDelete.IsErrorOccured)
                        {
                            cpDelete.Outputs.AddParameter("Delete created patient returns error", "Delete Patient", rltDelete.Message);
                            cpDelete.Result = TestResult.Fail;
                        }
                        else
                        {
                            cpDelete.Outputs.AddParameter("Delete created patient returns OK", "Delete Patient", rltDelete.Message);
                            cpDelete.Result = TestResult.Pass;
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }
            Output();
        }

        public void Run_WorkFlow_ImportCS1600Image_GetImageInfo_Case1063()  //Case 1063: 1.3.17_workflow_ImportCS1600Image_GetImageInfo
        {
            int runCount = 0;
            string patientUID = string.Empty;
            bool isCreatePatient = false;
            bool isDeletePatient = true;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Import image");

                ImportService ims = new ImportService();
                XMLParameter pa = new XMLParameter("patient");
                XMLParameter ia = new XMLParameter("import");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "import")
                    {
                        ia.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                XMLParameter epGetImagInfo = new XMLParameter();

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo")
                    {
                        epGetImagInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                }

                try
                {
                    PatientService patientSvc = new PatientService();
                    if (isCreatePatient)
                    {
                        CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                        r.CheckPoints.Add(pCreate);

                        XMLResult result = patientSvc.createPatient(pa);
                        if (!result.IsErrorOccured)
                        {
                            patientUID = result.SingleResult;
                            pCreate.Result = TestResult.Pass;
                            pCreate.Outputs.AddParameter("Create patient UID", "Create Patient", patientUID);
                        }
                        else
                        {
                            pCreate.Result = TestResult.Fail;
                            pCreate.Outputs.AddParameter("Create patient returns error", "Create Patient", result.ResultContent);
                        }
                    }

                    CheckPoint pImport = new CheckPoint("Import Image", "Test import");
                    r.CheckPoints.Add(pImport);

                    string filePath = string.Empty;
                    string archivePath = string.Empty;
                    bool move = false;
                    for (int c = 0; c < ia.Length; c++)
                    {
                        if (ia.GetParameterName(c) == "path")
                            filePath = ia.GetParameterValue(c);
                        if (ia.GetParameterName(c) == "archivePath")
                            archivePath = ia.GetParameterValue(c);
                        if (ia.GetParameterName(c) == "move")
                        {
                            if (ia.GetParameterValue(c) == "true")
                                move = true;
                        }
                    }

                    XMLResult rslImport = ims.importObject(patientUID, "", filePath, archivePath, move, "false");

                    //import return sample:
                    //   <trophy type="result" version="1.0">
                    //           <status code="0" message="ok" />
                    //           <image><parameter key="internal_id" value="2bfb416b-037e-41e7-aaef-8d2bd08b1ae7" /></image>
                    //           <presentationstate><parameter key="internal_id" value="75675345-a164-4088-b6d3-1b2ae86b703b" /></presentationstate>
                    // </trophy>

                    if (rslImport.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;
                        pImport.Outputs.AddParameter("Import image fail", "Import", rslImport.Message);
                        pImport.Outputs.AddParameter("Import image returns error code", "Import", rslImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains image id and ps id
                        string imageID = null;
                        imageID = rslImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (imageID == null || imageID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("Import image returns wrong image internal id", "Import", rslImport.ResultContent);
                        }

                        string psID = null;
                        psID = rslImport.MultiResults[1].GetParameterValueByName("internal_id"); // ps internal_id
                        if (psID == null || psID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("import image returns wrong ps internal id", "Import", rslImport.ResultContent);
                        }
                        else
                        {
                            pImport.Result = TestResult.Pass;
                            pImport.Outputs.AddParameter("import image returns OK", "Import", rslImport.ResultContent);
                        }

                        #region Call getImageInfo to check expected key and value
                        ImageService imageSvc = new ImageService();

                        XMLParameter pGetImageInfo = new XMLParameter("image");
                        pGetImageInfo.AddParameter("internal_id", imageID);

                        CheckPoint cpGetImageInfo = new CheckPoint("GetImageInfo", "Call GetImageInfo to check the dcm_anatomic_region value");
                        r.CheckPoints.Add(cpGetImageInfo);

                        XMLResult rtGetImageInfo = imageSvc.getImageInfo(pGetImageInfo);
                        if (rtGetImageInfo.IsErrorOccured)
                        {
                            cpGetImageInfo.Result = TestResult.Fail;
                            cpGetImageInfo.Outputs.AddParameter("Get Image Info return error", "getImageInfo", rtGetImageInfo.ResultContent);
                        }
                        else
                        {
                            cpGetImageInfo.Outputs.AddParameter("Get Image Info return success", "getImageInfo", rtGetImageInfo.Message);

                            //Check the return info contain the expected key and value: <parameter key="XXX" value="XXX" />
                            foreach (XMLParameterNode node in epGetImagInfo.Parameters)
                            {
                                if (!rtGetImageInfo.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                                {
                                    cpGetImageInfo.Result = TestResult.Fail;
                                    cpGetImageInfo.Outputs.AddParameter("Get Image Info returns wrong key-value info for: " + node.ParameterName, "GetImageInfo", rtGetImageInfo.ResultContent);
                                    continue;
                                }
                                else
                                {
                                    cpGetImageInfo.Outputs.AddParameter("Get Image Info returns correct key-value info for: " + node.ParameterName, "GetImageInfo", "ok");
                                }
                            }

                            if (cpGetImageInfo.Result != TestResult.Fail)
                            {
                                cpGetImageInfo.Result = TestResult.Pass;
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info return all correct info", "GetImageInfo", rtGetImageInfo.ResultContent);
                            }
                        }
                        #endregion
                    }

                    if (isDeletePatient)
                    {
                        CheckPoint cpDelete = new CheckPoint("Delete Patient", "Test delete patient");
                        r.CheckPoints.Add(cpDelete);
                        XMLResult rltDelete = patientSvc.deletePatient(patientUID);
                        if (rltDelete.IsErrorOccured)
                        {
                            cpDelete.Outputs.AddParameter("Delete created patient returns error", "Delete Patient", rltDelete.Message);
                            cpDelete.Result = TestResult.Fail;
                        }
                        else
                        {
                            cpDelete.Outputs.AddParameter("Delete created patient returns OK", "Delete Patient", rltDelete.Message);
                            cpDelete.Result = TestResult.Pass;
                        }
                    }

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }
            Output();
        }
    }
}