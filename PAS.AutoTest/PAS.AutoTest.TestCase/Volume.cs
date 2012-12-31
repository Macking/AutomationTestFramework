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
        public void Run_WorkFlow_3Dv_openVolume_with_default_a3d_Case1659()  // Case 1559: 1.9.1_WorkFlow_3Dv_openVolume_with_default_a3d
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string volumeid = "";
                    string criteriaOfAnalysis = "";
                    XMLParameter xmlVolumeInfoFilter = new XMLParameter("volume");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "VolumeInfo")
                        {
                            xmlVolumeInfoFilter.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                volumeid = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        if (ids.InputParameters.GetParameter(i).Step == "Analyses3D")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "criteria")
                            {
                                criteriaOfAnalysis = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step 1: Get Volume Info
                    CheckPoint cpGetVolumeInfo = new CheckPoint("Get Volume Info", "Call getVolumeInfo");
                    r.CheckPoints.Add(cpGetVolumeInfo);

                    VolumeService volumesrv = new VolumeService();
                    XMLResult rtVolumeGet = volumesrv.getVolumeInfo(xmlVolumeInfoFilter);

                    if (rtVolumeGet.IsErrorOccured)
                    {
                        cpGetVolumeInfo.Result = TestResult.Fail;
                        cpGetVolumeInfo.Outputs.AddParameter("Get volume info returns error", "Get volume info", rtVolumeGet.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpGetVolumeInfo.Result = TestResult.Pass;
                        cpGetVolumeInfo.Outputs.AddParameter("Get volume info returns success", "Get volume info", rtVolumeGet.ResultContent);
                    }
                    #endregion

                    #region Step 1-1: create analysis3d for volume
                    Analysis3DService a3dsrv = new Analysis3DService();
                    XMLParameter a3dpara = new XMLParameter("analysis3d");
                    a3dpara.AddParameter("analysis3D_xml", "Only for list analysis3d");
                    a3dpara.AddParameter("current", "true");
                    a3dsrv.createAnalysis3D(volumeid, a3dpara);
                    #endregion

                    #region Step 2: List Volume Analysis3D
                    CheckPoint cpGetAnalysis3d = new CheckPoint("Get Volume Of Analysis3D", "Call listAnalyses3DOfVolume");
                    r.CheckPoints.Add(cpGetAnalysis3d);

                    XMLResult rtAnalysis3d = volumesrv.listAnalyses3DOfVolume(volumeid, criteriaOfAnalysis);

                    if (rtAnalysis3d.IsErrorOccured)
                    {
                        cpGetAnalysis3d.Result = TestResult.Fail;
                        cpGetAnalysis3d.Outputs.AddParameter("Get volume info returns error", "Get volume info", rtAnalysis3d.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpGetAnalysis3d.Result = TestResult.Pass;
                        cpGetAnalysis3d.Outputs.AddParameter("Get volume info returns success", "Get volume info", rtAnalysis3d.ResultContent);
                    }
                    #endregion

                    #region Step 3: Get Anaylysis3D's Images
                    CheckPoint cpGetAnalysis3dImages = new CheckPoint("List Images of Analysis3d", "Call listImagesOfAnalysis3D");
                    r.CheckPoints.Add(cpGetAnalysis3dImages);

                    ParseXMLContent pc = new ParseXMLContent(rtAnalysis3d.ResultContent);
                    pc.getValueFromPath("trophy/analysis3d");
                    string analysis3duids = pc.getValueByKey("internal_id");
                    if (analysis3duids != "")
                    {

                        string analysis3duid = "";
                        do
                        {
                            int count = 1;
                            int indexPath = analysis3duids.IndexOf(";");
                            if (indexPath != -1)
                            {
                                analysis3duid = analysis3duids.Substring(0, indexPath);
                                analysis3duids = analysis3duids.Substring(indexPath + 1);
                            }
                            XMLResult imagesfroma3d = a3dsrv.listImagesOfAnalysis3D(analysis3duid);
                            if (imagesfroma3d.IsErrorOccured)
                            {
                                cpGetAnalysis3dImages.Result = TestResult.Fail;
                                cpGetAnalysis3dImages.Outputs.AddParameter("List Images of Analysis3d", "Call listImagesOfAnalysis3D", imagesfroma3d.Message);
                                break;
                            }
                            else
                            {
                                cpGetAnalysis3dImages.Result = TestResult.Pass;
                                cpGetAnalysis3dImages.Outputs.AddParameter("List Images of Analysis3d " + count + " :", "Call listImagesOfAnalysis3D", imagesfroma3d.ResultContent);
                            }
                            count++;
                        } while (analysis3duids != "");


                    }
                    else
                    {
                        cpGetAnalysis3dImages.Result = TestResult.Fail;
                        cpGetAnalysis3dImages.Outputs.AddParameter("Get images from Analysis3D", "Get images of Analysis3D", "No Analysis3D id return by listAnalyses3DOfVolume");
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_WorkFlow_3Dv_createVolume_with_created_and_set_a3d_Case1660()  //  //case 1.9.1_WorkFlow_3Dv_createVolume_with_created_and_set_a3d
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize

                    string volumeID = "";
                    //for create volume
                    XMLParameter cSeriesData = new XMLParameter("series");
                    XMLParameter cVolumeData = new XMLParameter("volume");
                    XMLParameter cSlicePathData = new XMLParameter("slices_path_list");
                    //for create study
                    XMLParameter studyPara = new XMLParameter("request");
                    XMLParameter xmlAnalysisInfo = new XMLParameter("analysis3d");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        switch (ids.InputParameters.GetParameter(i).Step)
                        {
                            case "series":
                                cSeriesData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                break;
                            case "volume":
                                cVolumeData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                break;
                            case "slices_path_list":
                                cSlicePathData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                break;
                            case "study":
                                studyPara.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "seta3d":
                                xmlAnalysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                        }
                    }
                    #endregion

                    #region Step 0: Create Study
                    string studyUID = string.Empty;
                    StudyService stusrv = new StudyService();
                    XMLResult stuRsl = stusrv.createStudy(studyPara);
                    if (stuRsl.IsErrorOccured)
                    {
                        continue;
                    }
                    else
                    {
                        studyUID = stuRsl.SingleResult;
                    }

                    #endregion

                    #region Step 1: Create Volume
                    CheckPoint cpCreateVolume = new CheckPoint("Create Volume Info", "Call createVolume");
                    r.CheckPoints.Add(cpCreateVolume);
                    XMLParameterCollection cInputData = new XMLParameterCollection();


                    cInputData.Add(cSeriesData);
                    cInputData.Add(cVolumeData);
                    cInputData.Add(cSlicePathData);
                    VolumeService volumesrv = new VolumeService();
                    XMLResult rslCreate = volumesrv.createVolume(studyUID, cInputData);
                    if (rslCreate.IsErrorOccured)
                    {
                        cpCreateVolume.Result = TestResult.Fail;
                        cpCreateVolume.Outputs.AddParameter("Create Volume Info", "Call createVolume", rslCreate.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpCreateVolume.Result = TestResult.Pass;
                        cpCreateVolume.Outputs.AddParameter("Create Volume Info", "Call createVolume", rslCreate.ResultContent);
                        volumeID = rslCreate.SingleResult;
                    }
                    #endregion

                    #region Step 2: Create Analysis3D
                    CheckPoint cpCreateA3d = new CheckPoint("Create Analysis3D", "Call createAnalysis3D");
                    r.CheckPoints.Add(cpCreateA3d);
                    Analysis3DService a3dsrv = new Analysis3DService();
                    XMLParameter a3dpara = new XMLParameter("analysis3d");
                    a3dpara.AddParameter("analysis3D_xml", "analysis3D_xml");
                    XMLResult a3dcreateRsl = a3dsrv.createAnalysis3D(volumeID, a3dpara);
                    string a3duid = "";
                    if (a3dcreateRsl.IsErrorOccured)
                    {
                        cpCreateA3d.Result = TestResult.Fail;
                        cpCreateA3d.Outputs.AddParameter("Create Analysis3D", "Call createAnalysis3D", a3dcreateRsl.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpCreateA3d.Result = TestResult.Pass;
                        cpCreateA3d.Outputs.AddParameter("Create Analysis3D", "Call createAnalysis3D", a3dcreateRsl.ResultContent);
                        a3duid = a3dcreateRsl.SingleResult;
                    }

                    #endregion

                    #region Step 3: Set Analysis3D
                    CheckPoint cpSetA3d = new CheckPoint("Set Analysis3D", "Call setAnalysis3DInfo");
                    r.CheckPoints.Add(cpSetA3d);
                    XMLResult seta3dRsl = a3dsrv.setAnalysis3DInfo(a3duid, xmlAnalysisInfo);
                    if (seta3dRsl.IsErrorOccured)
                    {
                        cpSetA3d.Result = TestResult.Fail;
                        cpSetA3d.Outputs.AddParameter("Set Analysis3D", "Call setAnalysis3DInfo", seta3dRsl.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        XMLResult geta3dRsl = a3dsrv.getAnalysis3DInfo(a3duid);

                        int matchCount = 0;

                        for (int j = 0; j < geta3dRsl.MultiResults[0].Length; j++)
                        {
                            if (geta3dRsl.MultiResults[0].Parameters[j].ParameterName == "name"
                                && geta3dRsl.MultiResults[0].Parameters[j].ParameterValue == "modified analysis 3d")
                            {
                                matchCount++;
                            }
                            if (geta3dRsl.MultiResults[0].Parameters[j].ParameterName == "comments"
                                && geta3dRsl.MultiResults[0].Parameters[j].ParameterValue == "set with some modification")
                            {
                                matchCount++;
                            }
                            if (geta3dRsl.MultiResults[0].Parameters[j].ParameterName == "current"
                                && geta3dRsl.MultiResults[0].Parameters[j].ParameterValue == "true")
                            {
                                matchCount++;
                            }
                            if (geta3dRsl.MultiResults[0].Parameters[j].ParameterName == "object_creation_date"
                                && geta3dRsl.MultiResults[0].Parameters[j].ParameterValue == "2012-12-05T15:25:00+08:00")
                            {
                                matchCount++;
                            }

                        }

                        if (matchCount == 4)
                        {
                            cpSetA3d.Result = TestResult.Pass;
                            cpSetA3d.Outputs.AddParameter("Set Analysis3D", "Call setAnalysis3DInfo", seta3dRsl.ResultContent);
                        }
                        else
                        {
                            cpSetA3d.Result = TestResult.Fail;
                            cpSetA3d.Outputs.AddParameter("Set Analysis3D", "Call setAnalysis3DInfo", "The set value is not equal with get");
                        }


                    }

                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_WorkFlow_3Dv_createCross_link_a3d_Case1686()  //  //case 1.9.3_WorkFlow_3Dv_createCross_link_a3d
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize

                    string volumeID = "";
                    //for create volume
                    XMLParameter cSeriesData = new XMLParameter("series");
                    XMLParameter cVolumeData = new XMLParameter("volume");
                    XMLParameter cSlicePathData = new XMLParameter("slices_path_list");
                    //for create study
                    XMLParameter studyPara = new XMLParameter("request");
                    XMLParameter xmlAnalysisInfo = new XMLParameter("analysis3d");
                    //for create cross section
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");
                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        switch (ids.InputParameters.GetParameter(i).Step)
                        {
                            case "series":
                                cSeriesData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "volume":
                                cVolumeData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "slices_path_list":
                                cSlicePathData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "study":
                                studyPara.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "seta3d":
                                xmlAnalysisInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "create_crossSection":
                                crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "create_slices_path_list":
                                slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "create_slices_ps_xml_annotation_list":
                                slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "create_slices_ps_xml_general_list":
                                slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "create_slices_ps_xml_processing_list":
                                slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "create_slices_ps_thumbnail_path_list":
                                slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                        }
                    }
                    #endregion

                    #region Step 0: Create Study
                    string studyUID = string.Empty;
                    StudyService stusrv = new StudyService();
                    XMLResult stuRsl = stusrv.createStudy(studyPara);
                    if (stuRsl.IsErrorOccured)
                    {
                        continue;
                    }
                    else
                    {
                        studyUID = stuRsl.SingleResult;
                    }

                    #endregion

                    #region Step 1: Create Volume
                    CheckPoint cpCreateVolume = new CheckPoint("Create Volume Info", "Call createVolume");
                    r.CheckPoints.Add(cpCreateVolume);
                    XMLParameterCollection cInputData = new XMLParameterCollection();


                    cInputData.Add(cSeriesData);
                    cInputData.Add(cVolumeData);
                    cInputData.Add(cSlicePathData);
                    VolumeService volumesrv = new VolumeService();
                    XMLResult rslCreate = volumesrv.createVolume(studyUID, cInputData);
                    if (rslCreate.IsErrorOccured)
                    {
                        cpCreateVolume.Result = TestResult.Fail;
                        cpCreateVolume.Outputs.AddParameter("Create Volume Info", "Call createVolume", rslCreate.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpCreateVolume.Result = TestResult.Pass;
                        cpCreateVolume.Outputs.AddParameter("Create Volume Info", "Call createVolume", rslCreate.ResultContent);
                        volumeID = rslCreate.SingleResult;
                    }
                    #endregion

                    #region Step 2: Create Analysis3D
                    CheckPoint cpCreateA3d = new CheckPoint("Create Analysis3D", "Call createAnalysis3D");
                    r.CheckPoints.Add(cpCreateA3d);
                    Analysis3DService a3dsrv = new Analysis3DService();
                    XMLParameter a3dpara = new XMLParameter("analysis3d");
                    a3dpara.AddParameter("analysis3D_xml", "analysis3D_xml");
                    XMLResult a3dcreateRsl = a3dsrv.createAnalysis3D(volumeID, a3dpara);
                    string a3duid = "";
                    if (a3dcreateRsl.IsErrorOccured)
                    {
                        cpCreateA3d.Result = TestResult.Fail;
                        cpCreateA3d.Outputs.AddParameter("Create Analysis3D", "Call createAnalysis3D", a3dcreateRsl.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpCreateA3d.Result = TestResult.Pass;
                        cpCreateA3d.Outputs.AddParameter("Create Analysis3D", "Call createAnalysis3D", a3dcreateRsl.ResultContent);
                        a3duid = a3dcreateRsl.SingleResult;
                    }

                    #endregion

                    #region Step 3: Create Cross Section
                    CheckPoint cpCreateCSS = new CheckPoint("Create Analysis3D", "Call createAnalysis3D");
                    r.CheckPoints.Add(cpCreateCSS);


                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    XMLResult rt_Create = crossSectionSvc.createCrossSection(volumeID, p_createCrossSection);

                    if (rt_Create.IsErrorOccured)
                    {
                        cpCreateCSS.Result = TestResult.Fail;
                        cpCreateCSS.Outputs.AddParameter("Create", "Create a new crosssection returns error", rt_Create.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpCreateCSS.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rt_Create.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create.SingleResult == null || rt_Create.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cpCreateCSS.Result = TestResult.Fail;
                            cpCreateCSS.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rt_Create.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cpCreateCSS.Result = TestResult.Pass;
                            cpCreateCSS.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rt_Create.SingleResult);

                            string crosssectionuid = rt_Create.SingleResult;

                            CheckPoint cplinkcstoa3d = new CheckPoint("link CrossSection To Analysis3D", "Call linkCrossSectionToAnalysis3D");
                            r.CheckPoints.Add(cplinkcstoa3d);

                            //link cs to a3d
                            XMLResult linkRsl = a3dsrv.linkCrossSectionToAnalysis3D(a3duid, crosssectionuid);
                            if (linkRsl.IsErrorOccured)
                            {
                                cplinkcstoa3d.Result = TestResult.Fail;
                                cplinkcstoa3d.Outputs.AddParameter("Link Fail", "Link Cross Section to Analysis3D", "Return ERROR");
                            }
                            else
                            {
                                XMLResult getcsuidfroma3d = a3dsrv.listCrossSectionsOfAnalysis3D(a3duid);
                                if (!getcsuidfroma3d.IsErrorOccured)
                                {
                                    if (crosssectionuid == getcsuidfroma3d.SingleResult)
                                    {
                                        cplinkcstoa3d.Result = TestResult.Pass;
                                        cplinkcstoa3d.Outputs.AddParameter("Link Success", "Link Cross Section to Analysis3D", "Return Cross Section is matched with link");
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_WorkFlow_3Dv_createA3D_Case1688()  //  //case 1.9.2_WorkFlow_3Dv_createA3D
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize

                    string volumeID = "";
                    string criteriaOfAnalysis = "";
                    //for create volume
                    XMLParameter cSeriesData = new XMLParameter("series");
                    XMLParameter cVolumeData = new XMLParameter("volume");
                    XMLParameter cSlicePathData = new XMLParameter("slices_path_list");
                    //for create study
                    XMLParameter studyPara = new XMLParameter("request");
                    XMLParameter xmlAnalysisInfo = new XMLParameter("analysis3d");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        switch (ids.InputParameters.GetParameter(i).Step)
                        {
                            case "series":
                                cSeriesData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "volume":
                                cVolumeData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "slices_path_list":
                                cSlicePathData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "study":
                                studyPara.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "Analyses3D")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "criteria")
                            {
                                criteriaOfAnalysis = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step 0: Create Study
                    string studyUID = string.Empty;
                    StudyService stusrv = new StudyService();
                    XMLResult stuRsl = stusrv.createStudy(studyPara);
                    if (stuRsl.IsErrorOccured)
                    {
                        continue;
                    }
                    else
                    {
                        studyUID = stuRsl.SingleResult;
                    }

                    #endregion

                    #region Step 1: Create Volume
                    CheckPoint cpCreateVolume = new CheckPoint("Create Volume Info", "Call createVolume");
                    r.CheckPoints.Add(cpCreateVolume);
                    XMLParameterCollection cInputData = new XMLParameterCollection();

                    cInputData.Add(cSeriesData);
                    cInputData.Add(cVolumeData);
                    cInputData.Add(cSlicePathData);
                    VolumeService volumesrv = new VolumeService();
                    XMLResult rslCreate = volumesrv.createVolume(studyUID, cInputData);
                    if (rslCreate.IsErrorOccured)
                    {
                        cpCreateVolume.Result = TestResult.Fail;
                        cpCreateVolume.Outputs.AddParameter("Create Volume Info", "Call createVolume", rslCreate.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpCreateVolume.Result = TestResult.Pass;
                        cpCreateVolume.Outputs.AddParameter("Create Volume Info", "Call createVolume", rslCreate.ResultContent);
                        volumeID = rslCreate.SingleResult;
                    }
                    #endregion

                    #region Step 2: Create Analysis3D
                    CheckPoint cpCreateA3d = new CheckPoint("Create Analysis3D", "Call createAnalysis3D");
                    r.CheckPoints.Add(cpCreateA3d);
                    Analysis3DService a3dsrv = new Analysis3DService();
                    XMLParameter a3dpara = new XMLParameter("analysis3d");
                    a3dpara.AddParameter("analysis3D_xml", "analysis3D_xml");
                    XMLResult a3dcreateRsl = a3dsrv.createAnalysis3D(volumeID, a3dpara);
                    string a3duid = "";
                    if (a3dcreateRsl.IsErrorOccured)
                    {
                        cpCreateA3d.Result = TestResult.Fail;
                        cpCreateA3d.Outputs.AddParameter("Create Analysis3D", "Call createAnalysis3D", a3dcreateRsl.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpCreateA3d.Result = TestResult.Pass;
                        cpCreateA3d.Outputs.AddParameter("Create Analysis3D", "Call createAnalysis3D", a3dcreateRsl.ResultContent);
                        a3duid = a3dcreateRsl.SingleResult;
                    }

                    #endregion

                    #region Step 3: listAnalyses3DOfVolume
                    CheckPoint cpGetA3dofVolume = new CheckPoint("Get Analysis3D from volume", "Call listAnalyses3DOfVolume");
                    r.CheckPoints.Add(cpGetA3dofVolume);

                    XMLResult geta3dfromvolume = volumesrv.listAnalyses3DOfVolume(volumeID, criteriaOfAnalysis);

                    if (geta3dfromvolume.IsErrorOccured)
                    {
                        cpGetA3dofVolume.Result = TestResult.Fail;
                        cpGetA3dofVolume.Outputs.AddParameter("Get Analysis3D from volume", "Call listAnalyses3DOfVolume", geta3dfromvolume.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        if (a3duid == geta3dfromvolume.SingleResult)
                        {
                            cpGetA3dofVolume.Result = TestResult.Pass;
                            cpGetA3dofVolume.Outputs.AddParameter("Get Analysis3D from volume", "Call listAnalyses3DOfVolume", geta3dfromvolume.ResultContent);
                        }
                    }

                    #endregion
                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_WorkFlow_3Dv_openA3D_Case1693()  //  //case 1.9.2_WorkFlow_3Dv_openA3D
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize

                    string patientID = "";
                    string volumeID = "";
                    string analysis3dID = "";

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "study")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            {
                                patientID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "volume")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                volumeID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }

                        if (ids.InputParameters.GetParameter(i).Step == "analysis3d")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                analysis3dID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step 1: get Analysis3d
                    CheckPoint cpGetAnalysis3d = new CheckPoint("Get Analysis3d Info", "Call getAnalysis3DInfo");
                    r.CheckPoints.Add(cpGetAnalysis3d);
                    Analysis3DService a3dsrv = new Analysis3DService();

                    XMLResult getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    if (getA3dRsl.IsErrorOccured)
                    {
                        cpGetAnalysis3d.Result = TestResult.Fail;
                        cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info", "Call getAnalysis3DInfo", getA3dRsl.Message);
                        SaveRound(r);
                        break;
                    }
                    cpGetAnalysis3d.Result = TestResult.Pass;
                    cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info", "Call getAnalysis3DInfo", getA3dRsl.ResultContent);

                    string A3dResultString = getA3dRsl.ResultContent;

                    #endregion

                    #region Step 1: set Analysis3d
                    CheckPoint cpSetAnalysis3d = new CheckPoint("Set Analysis3d Info", "Call setAnalysis3DInfo");
                    r.CheckPoints.Add(cpSetAnalysis3d);

                    XMLParameter a3dpara = new XMLParameter("analysis3d");
                    a3dpara.AddParameter("name", "New Analysis");
                    a3dpara.AddParameter("comments", "Comments for New Analysis");
                    a3dpara.AddParameter("current", "false");

                    XMLResult seta3dRsl = a3dsrv.setAnalysis3DInfo(analysis3dID, a3dpara);
                    if (seta3dRsl.IsErrorOccured)
                    {
                        cpSetAnalysis3d.Result = TestResult.Fail;
                        cpSetAnalysis3d.Outputs.AddParameter("Set Analysis3d Info", "Call setAnalysis3DInfo", seta3dRsl.Message);
                        SaveRound(r);
                        break;
                    }

                    getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    if (!getA3dRsl.IsErrorOccured)
                    {
                        int successCount = 0;
                        for (int i = 0; i < getA3dRsl.MultiResults[0].Parameters.Count; i++)
                        {
                            if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "name"
                                && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "New Analysis")
                            {
                                successCount++;
                            }
                            if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "comments"
                                && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "Comments for New Analysis")
                            {
                                successCount++;
                            }
                            if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "current"
                                && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "false")
                            {
                                successCount++;
                            }
                        }
                        if (successCount == 3)
                        {
                            cpSetAnalysis3d.Result = TestResult.Pass;
                            cpSetAnalysis3d.Outputs.AddParameter("get Analysis3d Info after Set", "Call setAnalysis3DInfo", getA3dRsl.ResultContent);
                        }
                        else
                        {
                            cpSetAnalysis3d.Result = TestResult.Fail;
                            cpSetAnalysis3d.Outputs.AddParameter("get Analysis3d Info after Set", "Call setAnalysis3DInfo", "The Set value not match");
                        }
                    }

                    #endregion

                    #region Step 2: set Analysis3d to current

                    CheckPoint cpSetAnalysis3dCurrent = new CheckPoint("Set Analysis3d to Current", "Call setAnalysis3DToCurrent");
                    r.CheckPoints.Add(cpSetAnalysis3dCurrent);

                    XMLResult seta3dToCurrentRsl = a3dsrv.setAnalysis3DToCurrent(analysis3dID);

                    bool getCurrent = false;
                    getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    for (int i = 0; i < getA3dRsl.MultiResults[0].Parameters.Count; i++)
                    {
                        if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "current"
                            && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "true")
                        {
                            getCurrent = true;
                            cpSetAnalysis3dCurrent.Result = TestResult.Pass;
                            cpSetAnalysis3dCurrent.Outputs.AddParameter("get Analysis3d Info after setAnalysis3DToCurrent", "Call setAnalysis3DToCurrent", "The Set value is OK");
                            break;
                        }
                    }
                    if (!getCurrent)
                    {
                        cpSetAnalysis3dCurrent.Result = TestResult.Fail;
                        cpSetAnalysis3dCurrent.Outputs.AddParameter("get Analysis3d Info after setAnalysis3DToCurrent", "Call setAnalysis3DToCurrent", "setAnalysis3DToCurrent is not work");
                    }

                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_WorkFlow_3Dv_updateA3D_Case1695()  //  //case 1.9.2_WorkFlow_3Dv_updateA3D
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string patientID = "";
                    string volumeID = "";
                    string analysis3dID = "";

                    #region Step 0: prepare data
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "volume")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                volumeID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step 1: get listAnalyses3DOfVolume
                    CheckPoint cpGetVolumeInfo = new CheckPoint("Get Analysis3D of Volume", "Call listAnalyses3DOfVolume");
                    r.CheckPoints.Add(cpGetVolumeInfo);
                    VolumeService volsrv = new VolumeService();

                    XMLResult getA3dOfVolRsl = volsrv.listAnalyses3DOfVolume(volumeID, "all");
                    if (getA3dOfVolRsl.IsErrorOccured)
                    {
                        cpGetVolumeInfo.Result = TestResult.Fail;
                        cpGetVolumeInfo.Outputs.AddParameter("Get Analysis3D of Volume", "Call listAnalyses3DOfVolume", getA3dOfVolRsl.Message);
                        SaveRound(r);
                        break;
                    }
                    else
                    {
                        cpGetVolumeInfo.Result = TestResult.Pass;
                        cpGetVolumeInfo.Outputs.AddParameter("Get Analysis3D of Volume", "Call listAnalyses3DOfVolume", getA3dOfVolRsl.ResultContent);
                    }
                    #endregion


                    #region Step 2: get Analysis3d
                    CheckPoint cpGetAnalysis3d = new CheckPoint("Get Analysis3d Info", "Call getAnalysis3DInfo");
                    r.CheckPoints.Add(cpGetAnalysis3d);
                    Analysis3DService a3dsrv = new Analysis3DService();
                    int matchItem = 0;
                    analysis3dID = getA3dOfVolRsl.MultiResults[0].Parameters[0].ParameterValue;
                    XMLResult getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    if (getA3dRsl.IsErrorOccured)
                    {
                        cpGetAnalysis3d.Result = TestResult.Fail;
                        cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info", "Call getAnalysis3DInfo", getA3dRsl.Message);
                        SaveRound(r);
                        break;
                    }
                    for (int i = 0; i < getA3dRsl.MultiResults[0].Parameters.Count; i++)
                    {
                        if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "current"
                            && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "true")
                        {
                            matchItem++;
                        }
                    }

                    cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info 1:", "Call getAnalysis3DInfo", getA3dRsl.ResultContent);

                    analysis3dID = getA3dOfVolRsl.MultiResults[0].Parameters[1].ParameterValue;

                    getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    if (getA3dRsl.IsErrorOccured)
                    {
                        cpGetAnalysis3d.Result = TestResult.Fail;
                        cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info", "Call getAnalysis3DInfo", getA3dRsl.Message);
                        SaveRound(r);
                        break;
                    }
                    for (int i = 0; i < getA3dRsl.MultiResults[0].Parameters.Count; i++)
                    {
                        if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "current"
                            && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "false")
                        {
                            matchItem++;
                        }
                    }
                    cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info 2:", "Call getAnalysis3DInfo", getA3dRsl.ResultContent);

                    if (matchItem == 2)
                    {
                        cpGetAnalysis3d.Result = TestResult.Pass;
                        cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info", "Call getAnalysis3DInfo", "All current parameter is OK");
                    }
                    else
                    {
                        cpGetAnalysis3d.Result = TestResult.Fail;
                        cpGetAnalysis3d.Outputs.AddParameter("Get Analysis3d Info", "Call getAnalysis3DInfo", "Not all current parameter is OK");
                    }
                    #endregion

                    /*
                    #region Step 1: set Analysis3d
                    CheckPoint cpSetAnalysis3d = new CheckPoint("Set Analysis3d Info", "Call setAnalysis3DInfo");
                    r.CheckPoints.Add(cpSetAnalysis3d);

                    XMLParameter a3dpara = new XMLParameter("analysis3d");
                    a3dpara.AddParameter("name", "New Analysis");
                    a3dpara.AddParameter("comments", "Comments for New Analysis");
                    a3dpara.AddParameter("current", "false");

                    XMLResult seta3dRsl = a3dsrv.setAnalysis3DInfo(analysis3dID, a3dpara);
                    if (seta3dRsl.IsErrorOccured)
                    {
                        cpSetAnalysis3d.Result = TestResult.Fail;
                        cpSetAnalysis3d.Outputs.AddParameter("Set Analysis3d Info", "Call setAnalysis3DInfo", seta3dRsl.Message);
                        SaveRound(r);
                        break;
                    }

                    getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    if (!getA3dRsl.IsErrorOccured)
                    {
                        int successCount = 0;
                        for (int i = 0; i < getA3dRsl.MultiResults[0].Parameters.Count; i++)
                        {
                            if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "name"
                                && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "New Analysis")
                            {
                                successCount++;
                            }
                            if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "comments"
                                && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "Comments for New Analysis")
                            {
                                successCount++;
                            }
                            if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "current"
                                && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "false")
                            {
                                successCount++;
                            }
                        }
                        if (successCount == 3)
                        {
                            cpSetAnalysis3d.Result = TestResult.Pass;
                            cpSetAnalysis3d.Outputs.AddParameter("get Analysis3d Info after Set", "Call setAnalysis3DInfo", getA3dRsl.ResultContent);
                        }
                        else
                        {
                            cpSetAnalysis3d.Result = TestResult.Fail;
                            cpSetAnalysis3d.Outputs.AddParameter("get Analysis3d Info after Set", "Call setAnalysis3DInfo", "The Set value not match");
                        }
                    }

                    #endregion

                    #region Step 2: set Analysis3d to current

                    CheckPoint cpSetAnalysis3dCurrent = new CheckPoint("Set Analysis3d to Current", "Call setAnalysis3DToCurrent");
                    r.CheckPoints.Add(cpSetAnalysis3dCurrent);

                    XMLResult seta3dToCurrentRsl = a3dsrv.setAnalysis3DToCurrent(analysis3dID);

                    bool getCurrent = false;
                    getA3dRsl = a3dsrv.getAnalysis3DInfo(analysis3dID);
                    for (int i = 0; i < getA3dRsl.MultiResults[0].Parameters.Count; i++)
                    {
                        if (getA3dRsl.MultiResults[0].Parameters[i].ParameterName == "current"
                            && getA3dRsl.MultiResults[0].Parameters[i].ParameterValue == "true")
                        {
                            getCurrent = true;
                            cpSetAnalysis3dCurrent.Result = TestResult.Pass;
                            cpSetAnalysis3dCurrent.Outputs.AddParameter("get Analysis3d Info after setAnalysis3DToCurrent", "Call setAnalysis3DToCurrent", "The Set value is OK");
                            break;
                        }
                    }
                    if (!getCurrent)
                    {
                        cpSetAnalysis3dCurrent.Result = TestResult.Fail;
                        cpSetAnalysis3dCurrent.Outputs.AddParameter("get Analysis3d Info after setAnalysis3DToCurrent", "Call setAnalysis3DToCurrent", "setAnalysis3DToCurrent is not work");
                    }

                    #endregion
                    */
                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

    }       
}