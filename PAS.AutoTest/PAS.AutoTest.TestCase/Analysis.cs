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
        public void Run_Analysis_Acq_CreateAnalysis_Normal_Case47() //Case 47: 1.3.12_CreateAnalysis_Normal
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                string PatientID = string.Empty;
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                string retPsID = string.Empty;
                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                    }
                }
                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", rslCreateAnalysis.SingleResult);
                }
                ass.deleteAnalysis(rslCreateAnalysis.SingleResult);
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_SetAnalysisDescription_Case48() //Case 48: 1.3.12_SetAnalysisDescription_Normal
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter analysisInfo = new XMLParameter("analysis");
                string PatientID = string.Empty;
                int setInfoCount = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "AnalysisInfo")
                    {
                        analysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setInfoCount++;
                    }
                }
                string retPsID = string.Empty;
                string AnalysisUID = string.Empty;

                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");
                XMLParameter anaUidsSet = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                        if (acqTime == 0)
                        {
                            anaUidsSet.AddParameter("internal_id", retPsID);
                        }
                    }
                }

                System.Threading.Thread.Sleep(20000);

                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    AnalysisUID = rslCreateAnalysis.SingleResult;
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", AnalysisUID);
                }

                anaUidsXML.RemoveAt(0);
                anaUidsXML.Add(anaUidsSet);
                XMLResult rslsetAnaDesc = ass.setAnalysisDescription(AnalysisUID, analysisContent, false, anaUidsXML);

                if (rslsetAnaDesc.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Set Analysis Desc Failed", "Analysis Service", rslsetAnaDesc.Message);
                }
                else
                {
                    XMLResult rslGetAnaDesc = ass.getAnalysisDescription(AnalysisUID);

                    XMLResult rslListAnalysisObject = ass.listAnalysisObjects(AnalysisUID);

                    if (rslGetAnaDesc.IsErrorOccured || rslListAnalysisObject.IsErrorOccured)
                    {
                        pAnalysis.Result = TestResult.Fail;
                        pAnalysis.Outputs.AddParameter("Get Analysis Desc Failed", "Analysis Service", rslGetAnaDesc.Message);
                    }
                    else
                    {
                        //for (int co = 0; co < rslListAnalysisObject.MultiResults.Count; co++)
                        //{
                        //    if (rslListAnalysisObject.MultiResults[co].Name == "presentationstate" &&
                        //      rslListAnalysisObject.MultiResults[co].Parameters[0].ParameterValue == anaUidsSet.Parameters[0].ParameterValue)
                        //    {
                        //        pAnalysis.Result = TestResult.Pass;
                        //        pAnalysis.Outputs.AddParameter("Set Analysis Desc successfully", "Analysis Service", "The get presentation information match with setting");
                        //    }
                        //}

                        pAnalysis.Result = TestResult.Pass;

                    }
                }

                ass.deleteAnalysis(AnalysisUID);
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_GetAnalysisDescription_Case49() //Case 49: 1.3.12_GetAnalysisDescription_Normal
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter analysisInfo = new XMLParameter("analysis");
                string PatientID = string.Empty;
                int setInfoCount = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "AnalysisInfo")
                    {
                        analysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setInfoCount++;
                    }
                }
                string retPsID = string.Empty;
                string AnalysisUID = string.Empty;

                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");
                XMLParameter anaUidsSet = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                        if (acqTime == 0)
                        {
                            anaUidsSet.AddParameter("internal_id", retPsID);
                        }
                    }
                }
                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    AnalysisUID = rslCreateAnalysis.SingleResult;
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", AnalysisUID);
                }

                anaUidsXML.RemoveAt(0);
                anaUidsXML.Add(anaUidsSet);
                XMLResult rslsetAnaDesc = ass.setAnalysisDescription(AnalysisUID, analysisContent, false, anaUidsXML);

                if (rslsetAnaDesc.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Set Analysis Desc Failed", "Analysis Service", rslsetAnaDesc.Message);
                }
                else
                {
                    XMLResult rslGetAnaDesc = ass.getAnalysisDescription(AnalysisUID);

                    XMLResult rslListAnalysisObject = ass.listAnalysisObjects(AnalysisUID);

                    if (rslGetAnaDesc.IsErrorOccured || rslListAnalysisObject.IsErrorOccured)
                    {
                        pAnalysis.Result = TestResult.Fail;
                        pAnalysis.Outputs.AddParameter("Get Analysis Desc Failed", "Analysis Service", rslGetAnaDesc.Message);
                    }
                    else
                    {
                        for (int co = 0; co < rslListAnalysisObject.MultiResults.Count; co++)
                        {
                            if (rslListAnalysisObject.MultiResults[co].Name == "presentationstate" &&
                              rslListAnalysisObject.MultiResults[co].Parameters[0].ParameterValue == anaUidsSet.Parameters[0].ParameterValue)
                            {
                                pAnalysis.Result = TestResult.Pass;
                                pAnalysis.Outputs.AddParameter("Set Analysis Desc successfully", "Analysis Service", "The get presentation information match with setting");
                            }
                        }
                    }
                }

                ass.deleteAnalysis(AnalysisUID);
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_SetAnalysisInfo_Case50() //Case 50: 1.3.12_SetAnalysisInfo_Normal
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter analysisInfo = new XMLParameter("analysis");
                string PatientID = string.Empty;
                int setInfoCount = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "AnalysisInfo")
                    {
                        analysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setInfoCount++;
                    }
                }
                string retPsID = string.Empty;
                string AnalysisUID = string.Empty;

                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                    }
                }
                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    AnalysisUID = rslCreateAnalysis.SingleResult;
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", AnalysisUID);
                }

                XMLResult rslsetAnaInfo = ass.setAnalysisInfo(AnalysisUID, analysisInfo);
                if (rslsetAnaInfo.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Set Analysis Info", "Analysis Service", rslsetAnaInfo.Message);
                }
                else
                {
                    XMLResult rslGetAnaInfo = ass.getAnalysisInfo(AnalysisUID);
                    if (rslGetAnaInfo.IsErrorOccured)
                    {
                        pAnalysis.Result = TestResult.Fail;
                        pAnalysis.Outputs.AddParameter("Get Analysis Info", "Analysis Service", rslGetAnaInfo.Message);
                    }
                    else
                    {

                        for (int getCount = 0; getCount < rslGetAnaInfo.MultiResults[0].Parameters.Count; getCount++)
                        {
                            for (int k = 0; k < analysisInfo.Parameters.Count; k++)
                            {
                                if (rslGetAnaInfo.MultiResults[0].Parameters[getCount].ParameterName == analysisInfo.Parameters[k].ParameterName
                                  && rslGetAnaInfo.MultiResults[0].Parameters[getCount].ParameterValue == analysisInfo.Parameters[k].ParameterValue)
                                {
                                    setInfoCount--;
                                }
                            }

                        }
                        if (setInfoCount == 0)
                        {
                            pAnalysis.Result = TestResult.Pass;
                            pAnalysis.Outputs.AddParameter("Get Analysis Info Match the setting value", "Analysis Service", "Set successfully");
                        }

                    }

                }


                ass.deleteAnalysis(AnalysisUID);
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_GetAnalysisInfo_Case51() //Case 51:  1.3.12_GetAnalysisInfo_Normal
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter analysisInfo = new XMLParameter("analysis");
                string PatientID = string.Empty;
                int setInfoCount = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "AnalysisInfo")
                    {
                        analysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setInfoCount++;
                    }
                }
                string retPsID = string.Empty;
                string AnalysisUID = string.Empty;

                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                    }
                }
                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    AnalysisUID = rslCreateAnalysis.SingleResult;
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", AnalysisUID);
                }

                XMLResult rslsetAnaInfo = ass.setAnalysisInfo(AnalysisUID, analysisInfo);
                if (rslsetAnaInfo.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Set Analysis Info", "Analysis Service", rslsetAnaInfo.Message);
                }
                else
                {
                    XMLResult rslGetAnaInfo = ass.getAnalysisInfo(AnalysisUID);
                    if (rslGetAnaInfo.IsErrorOccured)
                    {
                        pAnalysis.Result = TestResult.Fail;
                        pAnalysis.Outputs.AddParameter("Get Analysis Info", "Analysis Service", rslGetAnaInfo.Message);
                    }
                    else
                    {

                        for (int getCount = 0; getCount < rslGetAnaInfo.MultiResults[0].Parameters.Count; getCount++)
                        {
                            for (int k = 0; k < analysisInfo.Parameters.Count; k++)
                            {
                                if (rslGetAnaInfo.MultiResults[0].Parameters[getCount].ParameterName == analysisInfo.Parameters[k].ParameterName
                                  && rslGetAnaInfo.MultiResults[0].Parameters[getCount].ParameterValue == analysisInfo.Parameters[k].ParameterValue)
                                {
                                    setInfoCount--;
                                }
                            }

                        }
                        if (setInfoCount == 0)
                        {
                            pAnalysis.Result = TestResult.Pass;
                            pAnalysis.Outputs.AddParameter("Get Analysis Info Match the setting value", "Analysis Service", "Set successfully");
                        }

                    }

                }


                ass.deleteAnalysis(AnalysisUID);
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_ListAnalysisObjects_Case54() //Case 54: 1.3.12_ListAnalysisObjects_Normal
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter analysisInfo = new XMLParameter("analysis");
                string PatientID = string.Empty;
                int setInfoCount = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "AnalysisInfo")
                    {
                        analysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        setInfoCount++;
                    }
                }
                string retPsID = string.Empty;
                string AnalysisUID = string.Empty;

                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");
                XMLParameter anaUidsSet = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                        if (acqTime == 0)
                        {
                            anaUidsSet.AddParameter("internal_id", retPsID);
                        }
                    }
                }
                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    AnalysisUID = rslCreateAnalysis.SingleResult;
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", AnalysisUID);
                }

                anaUidsXML.RemoveAt(0);
                anaUidsXML.Add(anaUidsSet);
                XMLResult rslsetAnaDesc = ass.setAnalysisDescription(AnalysisUID, analysisContent, false, anaUidsXML);

                if (rslsetAnaDesc.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Set Analysis Desc Failed", "Analysis Service", rslsetAnaDesc.Message);
                }
                else
                {
                    XMLResult rslGetAnaDesc = ass.getAnalysisDescription(AnalysisUID);

                    XMLResult rslListAnalysisObject = ass.listAnalysisObjects(AnalysisUID);

                    if (rslGetAnaDesc.IsErrorOccured || rslListAnalysisObject.IsErrorOccured)
                    {
                        pAnalysis.Result = TestResult.Fail;
                        pAnalysis.Outputs.AddParameter("Get Analysis Desc Failed", "Analysis Service", rslGetAnaDesc.Message);
                    }
                    else
                    {
                        for (int co = 0; co < rslListAnalysisObject.MultiResults.Count; co++)
                        {
                            if (rslListAnalysisObject.MultiResults[co].Name == "presentationstate" &&
                              rslListAnalysisObject.MultiResults[co].Parameters[0].ParameterValue == anaUidsSet.Parameters[0].ParameterValue)
                            {
                                pAnalysis.Result = TestResult.Pass;
                                pAnalysis.Outputs.AddParameter("Set Analysis Desc successfully", "Analysis Service", "The get presentation information match with setting");
                            }
                        }
                    }
                }

                ass.deleteAnalysis(AnalysisUID);
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_DeleteAnalysiswithPS_Case1099() //Case1099: 1.3.12_DeleteAnalysis_N3_Delete Analysis with PS
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">          <trophyheader>        <accesslog>            <creation date=\"2011-10-25\" time=\"10:54:17\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/25\" time=\"10:54:18\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"Test Analysis\" uid=\"\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"1112222\" comments=\"\" date=\"2011-10-25\" time=\"10:54:17\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"938\" y=\"201\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"image\" x=\"349\" y=\"299\" w=\"266\" h=\"355\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID0\" showstate=\"selected\" IdV0=\"76b3ec9d-1374-4a13-9950-6cf3f5ebc1e6\" />        </Frame>        <Frame index=\"4\" type=\"image\" x=\"818\" y=\"299\" w=\"266\" h=\"354\" Zorder=\"\">            <PanelProp paneltype=\"image\" id=\"@@PSID1\" showstate=\"deselected\" IdV0=\"eb87d761-accf-4531-b4fc-9ee9861f15fd\" />        </Frame>    </page></trophy>";

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");

                CheckPoint pAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pAnalysis);

                XMLParameter acq = new XMLParameter("acq_info");
                string PatientID = string.Empty;
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                string retPsID = string.Empty;
                string repStr = "@@PSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("presentationstate");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    retPsID = AcquisitionService.AcquireImage(acq);
                    if (retPsID != string.Empty)
                    {
                        analysisContent = analysisContent.Replace(repStr + acqTime, retPsID);
                        anaUids.AddParameter("internal_id", retPsID);
                    }
                }
                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Create Analysis Failed", "Analysis Service", rslCreateAnalysis.Message);
                    break;
                }
                else
                {
                    pAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", rslCreateAnalysis.SingleResult);
                }
                XMLResult rsldelAnalysis = ass.deleteAnalysis(rslCreateAnalysis.SingleResult);
                if (rsldelAnalysis.IsErrorOccured)
                {
                    pAnalysis.Result = TestResult.Fail;
                    pAnalysis.Outputs.AddParameter("Delete Analysis Failed", "Analysis Service", rslCreateAnalysis.Message);
                }
                else
                {
                    pAnalysis.Result = TestResult.Pass;
                    pAnalysis.Outputs.AddParameter("Delete Analysis Success", "Analysis Service", rslCreateAnalysis.SingleResult);
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Analysis_Acq_DeleteAnalysiswithFMS_Case962() //Case962: 1.3.12_DeleteAnalysis_N2_Delete Analysis with FMS
        {
            int runCount = 0;
            string analysisContent = "<trophy type=\"analysis\" version=\"1.0\">    <trophyheader>        <accesslog>            <creation date=\"2011/10/26\" time=\"14:33:59\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />            <modification date=\"2011/10/26\" time=\"14:33:59\" applicationname=\"kdis2dviewer\" applicationversion=\"7.0\" />        </accesslog>    </trophyheader>    <templateheader>        <object subtype=\"analysis\" instance=\"true\" name=\"2011/10/26 14:33:59\" uid=\"a680942f-ae84-4a18-8dab-cd766bda64da\" />        <comments />        <icon filepath=\"analysisicon.jpg\" />        <icon>            <ObjectData>base64imagedata</ObjectData>        </icon>        <icon id=\"thumbnailid\" />        <AnalysisProp name=\"2011/10/26 14:33:59\" comments=\"\" date=\"2011/10/26\" time=\"14:33:59\" arrangementmode=\"0\" />    </templateheader>    <page index=\"0\" Dx=\"1280\" Dy=\"1024\" backgroundColour=\"RGB(0,0,0)\">        <Frame index=\"1\" type=\"floating panel\" x=\"914\" y=\"142\" w=\"200\" h=\"239\" Zorder=\"\">            <PanelProp paneltype=\"control panel xray\" id=\"control panel xray\" showstate=\"maximized\" />        </Frame>        <Frame index=\"2\" type=\"floating panel\" x=\"914\" y=\"142\" w=\"200\" h=\"240\" Zorder=\"\">            <PanelProp paneltype=\"control panel color\" id=\"control panel color\" showstate=\"maximized\" />        </Frame>        <Frame index=\"3\" type=\"fms\" x=\"697\" y=\"255\" w=\"457\" h=\"323\" Zorder=\"\">            <PanelProp paneltype=\"fms\" id=\"@@FMSID0\" showstate=\"selected\" IdV0=\"@@FMSID0\" />        </Frame>        <Frame index=\"4\" type=\"fms\" x=\"229\" y=\"255\" w=\"457\" h=\"323\" Zorder=\"\">            <PanelProp paneltype=\"fms\" id=\"@@FMSID1\" showstate=\"deselected\" IdV0=\"@@FMSID1\" />        </Frame>    </page></trophy>";
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                Round r = this.NewRound(runCount.ToString(), "Acquisition for Analysis");



                XMLParameter acq = new XMLParameter("acq_info");
                string PatientID = string.Empty;
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "acquire")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                        {
                            PatientID = ids.InputParameters.GetParameter(i).Value;
                        }
                        acq.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                string retFmsID = string.Empty;
                string repStr = "@@FMSID";
                XMLParameterCollection anaUidsXML = new XMLParameterCollection();
                XMLParameter anaUids = new XMLParameter("fms");

                for (int acqTime = 0; acqTime < 2; acqTime++)
                {
                    CheckPoint pAcquire = new CheckPoint("Acquire FMS", "Acquire FMS");
                    r.CheckPoints.Add(pAcquire);
                    System.Diagnostics.Debug.Print("Acquire start");

                    retFmsID = AcquisitionService.AcquireFMS(acq);

                    if (!string.IsNullOrEmpty(retFmsID))
                    {
                        pAcquire.Result = TestResult.Pass;
                        pAcquire.Outputs.AddParameter("Acquire FMS", "Acq return correct.", "FMS ID :" + retFmsID);
                        analysisContent = analysisContent.Replace(repStr + acqTime, retFmsID);
                        anaUids.AddParameter("internal_id", retFmsID);
                    }
                    else
                    {
                        pAcquire.Result = TestResult.Fail;
                        pAcquire.Outputs.AddParameter("Acquire FMS", "Acq return error.", "FMS ID :" + retFmsID);
                        goto CLEANUP;
                    }
                }

                CheckPoint pCreateAnalysis = new CheckPoint("Create Analysis", "Create Analysis");
                r.CheckPoints.Add(pCreateAnalysis);

                anaUidsXML.Add(anaUids);
                AnalysisService ass = new AnalysisService();

                XMLResult rslCreateAnalysis = ass.createAnalysis(analysisContent, false, true, PatientID, @"D:\Test\DICOM_Imag_Lib\ImportImage\thumb.PNG", anaUidsXML);

                if (rslCreateAnalysis.IsErrorOccured)
                {
                    pCreateAnalysis.Result = TestResult.Fail;
                    pCreateAnalysis.Outputs.AddParameter("Create Analysis Failed", "Analysis Service", rslCreateAnalysis.ResultContent);
                    break;
                }
                else
                {
                    pCreateAnalysis.Outputs.AddParameter("Create Analysis ID:", "Analysis Service", rslCreateAnalysis.ResultContent);
                }
                XMLResult rsldelAnalysis = ass.deleteAnalysis(rslCreateAnalysis.SingleResult);
                if (rsldelAnalysis.IsErrorOccured)
                {
                    pCreateAnalysis.Result = TestResult.Fail;
                    pCreateAnalysis.Outputs.AddParameter("Delete Analysis Failed", "Analysis Service", rslCreateAnalysis.ResultContent);
                }
                else
                {
                    pCreateAnalysis.Result = TestResult.Pass;
                    pCreateAnalysis.Outputs.AddParameter("Delete Analysis Success", "Analysis Service", rslCreateAnalysis.ResultContent);
                }

            CLEANUP:
                SaveRound(r);
            }
            Output();
        }

       
    }
}