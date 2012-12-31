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
        public void Run_3D_CreateCrossSection_Case1158() // Case1158: 1.9.3.1_createCrossSection
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    CrossSectionService crossSectionSvc = new CrossSectionService();

                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cp_Create = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cp_Create);

                    XMLResult rt_Create = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rt_Create.IsErrorOccured)
                    {
                        cp_Create.Result = TestResult.Fail;
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns error", rt_Create.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rt_Create.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create.SingleResult == null || rt_Create.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cp_Create.Result = TestResult.Fail;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rt_Create.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Create.Result = TestResult.Pass;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rt_Create.SingleResult);
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_DeleteCrossSection_Case1159() // Case 1159: 1.9.3.2_deleteCrossSection
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cp_Create = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cp_Create);

                    XMLResult rt_Create = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rt_Create.IsErrorOccured)
                    {
                        cp_Create.Result = TestResult.Fail;
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns error", rt_Create.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rt_Create.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create.SingleResult == null || rt_Create.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cp_Create.Result = TestResult.Fail;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rt_Create.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Create.Result = TestResult.Pass;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rt_Create.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 2: Call CrossSectionService.deleteCrossSection to delete the created crosssection
                    string p_crossectionUID = rt_Create.SingleResult;
                    CheckPoint cp_Delete = new CheckPoint("Delete", "Step 2: Call CrossSectionService.deleteCrossSection to delete the created crosssection");
                    r.CheckPoints.Add(cp_Delete);

                    XMLResult rt_Delete = crossSectionSvc.deleteCrossSection(p_crossectionUID);

                    if (rt_Delete.IsErrorOccured)
                    {
                        cp_Delete.Result = TestResult.Fail;
                        cp_Delete.Outputs.AddParameter("Delete", "Delete the created crosssection returns error", rt_Delete.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        //Check the "internal_id" in return is present
                        bool isActuallyDeleted = true;
                        // To add the check

                        if (!isActuallyDeleted)
                        {
                            cp_Delete.Result = TestResult.Fail;
                            cp_Delete.Outputs.AddParameter("Delete", "The crosssection is not corroctly deleted", rt_Delete.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Delete.Result = TestResult.Pass;
                            cp_Delete.Outputs.AddParameter("Delete", "Delete the created crosssection returns succeess", rt_Delete.Message);
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_ImportCSModel_GetVolumeInfo_Case1220()  //Case 1220: 1.9.1_WorkFlow_N2_Import CS Model_GetVolumeInfo
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

                ImportService importSvc = new ImportService();
                XMLParameter pCreatePatient = new XMLParameter("patient");
                XMLParameter pImportImage = new XMLParameter("import");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pCreatePatient.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "import")
                    {
                        pImportImage.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }

                XMLParameter epGetVolumeInfo = new XMLParameter();

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getVolumeInfo")
                    {
                        epGetVolumeInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                }

                try
                {
                    PatientService patientSvc = new PatientService();
                    if (isCreatePatient)
                    {
                        CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                        r.CheckPoints.Add(pCreate);

                        XMLResult result = patientSvc.createPatient(pCreatePatient);
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

                    CheckPoint cpImport = new CheckPoint("Import Image", "Test import");
                    r.CheckPoints.Add(cpImport);

                    string filePath = string.Empty;
                    string archivePath = string.Empty;
                    bool overrideExisted = false;
                    for (int c = 0; c < pImportImage.Length; c++)
                    {
                        if (pImportImage.GetParameterName(c) == "path")
                            filePath = pImportImage.GetParameterValue(c);
                        if (pImportImage.GetParameterName(c) == "archivePath")
                            archivePath = pImportImage.GetParameterValue(c);
                        if (pImportImage.GetParameterName(c) == "move")
                        {
                            if (pImportImage.GetParameterValue(c) == "true")
                                overrideExisted = true;
                        }
                    }

                    XMLResult rtlImport = importSvc.importObject(patientUID, "", filePath, archivePath, overrideExisted, "false");

                    /**********************************************
                        import return sample:
                        - <trophy type="result" version="1.0">
                          <status code="0" message="ok" /> 
                        - <volume>
                          <parameter key="internal_id" value="5412d81a-c096-4b0d-ab93-524266af21b6" /> 
                          </volume>
                          </trophy>
                     * **********************************************/

                    if (rtlImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("Import volume fail", "Import", rtlImport.Message);
                        cpImport.Outputs.AddParameter("Import volume returns error code", "Import", rtlImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains image id and ps id
                        string volumeID = null;
                        volumeID = rtlImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (volumeID == null || volumeID == string.Empty)
                        {
                            cpImport.Result = TestResult.Fail;
                            cpImport.Outputs.AddParameter("Import volume returns wrong volume internal id", "Import", rtlImport.ResultContent);
                        }
                        else
                        {
                            cpImport.Result = TestResult.Pass;
                            cpImport.Outputs.AddParameter("import volume returns OK", "Import", rtlImport.ResultContent);
                        }

                        #region Call getVolumeInfo to check expected key and value
                        VolumeService volumeSvc = new VolumeService();

                        XMLParameter pGetVolumeInfo = new XMLParameter("volume");
                        pGetVolumeInfo.AddParameter("internal_id", volumeID);

                        CheckPoint cpGetVolumeInfo = new CheckPoint("GetVolumeInfo", "Call GetVolumeInfo to check the return value");
                        r.CheckPoints.Add(cpGetVolumeInfo);

                        XMLResult rtGetVolumeInfo = volumeSvc.getVolumeInfo(pGetVolumeInfo);
                        if (rtGetVolumeInfo.IsErrorOccured)
                        {
                            cpGetVolumeInfo.Result = TestResult.Fail;
                            cpGetVolumeInfo.Outputs.AddParameter("Get Volume Info return error", "GetVolumeInfo", rtGetVolumeInfo.ResultContent);
                        }
                        else
                        {
                            cpGetVolumeInfo.Outputs.AddParameter("Get Volume Info return success", "GetVolumeInfo", rtGetVolumeInfo.Message);

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
                            foreach (XMLParameterNode node in epGetVolumeInfo.Parameters)
                            {
                                if (!rtGetVolumeInfo.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                                {
                                    cpGetVolumeInfo.Result = TestResult.Fail;
                                    cpGetVolumeInfo.Outputs.AddParameter("Get Volume Info returns wrong key-value info for: " + node.ParameterName, "GetVolumeInfo", rtGetVolumeInfo.ResultContent);
                                    continue;
                                }
                                else
                                {
                                    cpGetVolumeInfo.Outputs.AddParameter("Get Volume Info returns correct key-value info for: " + node.ParameterName, "GetVolumeInfo", "ok");
                                }
                            }

                            if (cpGetVolumeInfo.Result != TestResult.Fail)
                            {
                                cpGetVolumeInfo.Result = TestResult.Pass;
                                cpGetVolumeInfo.Outputs.AddParameter("Get Volume Info returns all correct info", "GetVolumeInfo", rtGetVolumeInfo.ResultContent);
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
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }
            Output();
        }

        public void Run_3D_CreateVolume_GetVolumeInfo_Customdata_Case1364() //Case 1364: 1.9.1_WorkFlow_CreateVolume_GetVolumeInfo_customdata
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "WorkFlow CreateVolume_GetVolume_CustomData");

                try
                {
                    VolumeService vs = new VolumeService();
                    string volumeID = null;
                    XMLParameterCollection cInputData = new XMLParameterCollection();
                    XMLParameter cSeriesData = new XMLParameter("series");
                    XMLParameter cVolumeData = new XMLParameter("volume");
                    XMLParameter cSlicePathData = new XMLParameter("slices_path_list");
                    string studyUID = string.Empty;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        switch (ids.InputParameters.GetParameter(i).Step)
                        {
                            case "na":
                                studyUID = ids.InputParameters.GetParameter(i).Value;
                                break;
                            case "series":
                                cSeriesData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                break;
                            case "volume":
                                cVolumeData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                break;
                            case "slices_path_list":
                                cSlicePathData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                break;

                        }
                    }

                    string ep_3dscanmode = null;
                    string ep_3dscanobject = null;
                    string ep_3dfunctionalmode = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "get")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "3dscanmode":
                                    ep_3dscanmode = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "3dscanobject":
                                    ep_3dscanobject = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "3dfunctionalmode":
                                    ep_3dfunctionalmode = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    CheckPoint pCreate = new CheckPoint("Create", "WorkFlow CreateVolume_GetVolume_CustomData");
                    r.CheckPoints.Add(pCreate);

                    cInputData.Add(cSeriesData);
                    cInputData.Add(cVolumeData);
                    cInputData.Add(cSlicePathData);
                    XMLResult rslCreate = vs.createVolume(studyUID, cInputData);

                    if (!rslCreate.IsErrorOccured)
                    {
                        pCreate.Result = TestResult.Pass;
                        volumeID = rslCreate.SingleResult;
                        pCreate.Outputs.AddParameter("Create", "CreateVolume_GetVolume_CustomData Success", volumeID);
                    }
                    else
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("Create", "CreateVolume_GetVolume_CustomData Fail", rslCreate.ResultContent);
                        SaveRound(r);
                        continue;
                    }

                    CheckPoint pGet = new CheckPoint("Get", "WorkFlow CreateVolume_GetVolume_CustomData");
                    r.CheckPoints.Add(pGet);

                    XMLParameter filter = new XMLParameter("volume");
                    filter.AddParameter("internal_id", volumeID);
                    filter.AddParameter("path_type", "all");

                    string str1 = string.Empty;
                    string str2 = string.Empty;
                    string str3 = string.Empty;
                    XMLResult rslget = vs.getVolumeInfo(filter);
                    foreach (XMLParameter p in rslget.MultiResults)
                    {
                        if (p.Name == "volume")
                        {
                            str1 = p.GetParameterValue("3dscanmode");
                            str2 = p.GetParameterValue("3dscanobject");
                            str3 = p.GetParameterValue("3dfunctionalmode");
                        }
                    }

                    if (!rslget.IsErrorOccured && str1 == ep_3dscanmode && str2 == ep_3dscanobject && str3 == ep_3dfunctionalmode)
                    {
                        pGet.Result = TestResult.Pass;
                        pGet.Outputs.AddParameter("Get", "CreateVolume_GetVolume_CustomData Success", rslget.ResultContent);
                        vs.deleteVolume(volumeID);
                    }
                    else
                    {
                        pGet.Result = TestResult.Fail;
                        pGet.Outputs.AddParameter("Get", "CreateVolume_GetVolume_CustomData Fail", rslget.ResultContent);
                    }

                    vs.deleteVolume(volumeID);

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message: ", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        //public void Run_3D_CreateVolume_GetVolumeInfo_DeleteVolume_Case1368() //Case 1368: 1.9.1_Create_Get_Delete_Volume_Normal
        //{
        //    int runCount = 0;
        //    foreach (InputDataSet ids in this.Input.DataSets)
        //    {
        //        runCount++;
        //        Round r = this.NewRound(runCount.ToString(), "3D Unit Create_Get_Delete_Volume");
        //        CheckPoint pCreate = new CheckPoint("3D Unit", "3D Unit Create_Get_Delete_Volume");
        //        r.CheckPoints.Add(pCreate);

        //        VolumeService vs = new VolumeService();

        //        XMLParameterCollection cInputData = new XMLParameterCollection();
        //        XMLParameter cSeriesData = new XMLParameter("series");
        //        XMLParameter cVolumeData = new XMLParameter("volume");
        //        XMLParameter cSlicePathData = new XMLParameter("slices_path_list");
        //        string studyUID = string.Empty;
        //        for (int i = 0; i < ids.InputParameters.Count; i++)
        //        {
        //            switch (ids.InputParameters.GetParameter(i).Step)
        //            {
        //                case "na":
        //                    studyUID = ids.InputParameters.GetParameter(i).Value;
        //                    break;
        //                case "series":
        //                    cSeriesData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
        //                    break;
        //                case "volume":
        //                    cVolumeData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
        //                    break;
        //                case "slices_path_list":
        //                    cSlicePathData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
        //                    break;

        //            }
        //        }
        //        cInputData.Add(cSeriesData);
        //        cInputData.Add(cVolumeData);
        //        cInputData.Add(cSlicePathData);
        //        string test = cInputData.GenerateXML();
        //        XMLResult rslCreate = vs.createVolume(studyUID, cInputData);
        //        string volumeID = rslCreate.SingleResult;
        //        XMLParameter filter = new XMLParameter("volume");
        //        filter.AddParameter("internal_id", volumeID);
        //        filter.AddParameter("path_type", "all");

        //        XMLResult rslget = vs.getVolumeInfo(filter);
        //        XMLResult rsldel = vs.deleteVolume(volumeID);

        //        if (!rslCreate.IsErrorOccured && !rslget.IsErrorOccured && !rsldel.IsErrorOccured)
        //        {
        //            pCreate.Result = TestResult.Pass;
        //            pCreate.Outputs.AddParameter("3D Unit", "CRU Volume Success", volumeID);
        //        }
        //        else
        //        {
        //            pCreate.Result = TestResult.Fail;
        //            pCreate.Outputs.AddParameter("3D Unit", "CreateVolume", "Create:" + rslCreate.Message + " Get:" + rslget.Message + " delete:" + rsldel.Message);
        //        }

        //        SaveRound(r);
        //        Output();

        //    }

        //}

        
        public void Run_3D_GetCrossSectionInfo_Case1477()  //Case 1477: 1.9.3.5_getCrossSectionInfo
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    //Expected values
                    XMLParameter getCrossSectionInfoReturnParam = new XMLParameter("crosssection");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCrossSectionInfo")
                        {
                            // Add the key to check and the expected value
                            getCrossSectionInfoReturnParam.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cp_Create = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cp_Create);

                    XMLResult rt_Create = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rt_Create.IsErrorOccured)
                    {
                        cp_Create.Result = TestResult.Fail;
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns error", rt_Create.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rt_Create.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create.SingleResult == null || rt_Create.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cp_Create.Result = TestResult.Fail;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rt_Create.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Create.Result = TestResult.Pass;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rt_Create.SingleResult);
                        }
                    }
                    #endregion

                    //System.Threading.Thread.Sleep(3000);  // Add for defect EK_HI00167289, remove this after it's fixed.

                    #region Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info
                    CheckPoint cpGetCrossSectionInfo = new CheckPoint("GetCrossSectionInfo", "Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info");
                    r.CheckPoints.Add(cpGetCrossSectionInfo);

                    string p_crossSectionUID = rt_Create.SingleResult;
                    XMLResult rtGetCrossSectionInfo = crossSectionSvc.getCrossSectionInfo(p_crossSectionUID);

                    if (rtGetCrossSectionInfo.IsErrorOccured)
                    {
                        cpGetCrossSectionInfo.Result = TestResult.Fail;
                        cpGetCrossSectionInfo.Outputs.AddParameter("GetCrossSectionInfo", "GetCrossSectionInfo returns error", rtGetCrossSectionInfo.Message);

                        goto CLEANUP;
                    }
                    else // Check the return value details
                    {
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in getCrossSectionInfoReturnParam.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rtGetCrossSectionInfo.MultiResults[0].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName.ToLower() == rtGetCrossSectionInfo.MultiResults[0].Parameters[i].ParameterName.ToLower())
                                {
                                    isKeyShow = true;
                                    isValueEqual = string.Equals(psNode.ParameterValue, rtGetCrossSectionInfo.MultiResults[0].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cpGetCrossSectionInfo.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    cpGetCrossSectionInfo.Outputs.AddParameter("GetCrossSectionInfo", "Check the Cross Section info in getCrossSectionInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rtGetCrossSectionInfo.MultiResults[0].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    cpGetCrossSectionInfo.Outputs.AddParameter("GetCrossSectionInfo", "Check the Cross Section info in getCrossSectionInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cpGetCrossSectionInfo.Result = TestResult.Pass;
                            cpGetCrossSectionInfo.Outputs.AddParameter("GetCrossSectionInfo", "Check the Cross Section info in getCrossSectionInfo return", "The return values in getCrossSectionInfo all match the expected");
                        }
                    }
                    #endregion

                CLEANUP:
                    #region Step 3: Clean up, delete the crossSection
                    crossSectionSvc.deleteCrossSection(p_crossSectionUID);
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_GetCrossSectionCompleteInfo_SlicesPath_Case1480()  //1480: 1.9.3.9_getCrossSectionCompleteInfo_slices_path
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    //Expected values
                    XMLParameter crossSectionCompleteInfo_Slices_Path = new XMLParameter("slices_path_list");

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCrossSectionCompleteInfo_slices_path_list")
                        {
                            crossSectionCompleteInfo_Slices_Path.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cpCreate = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cpCreate);

                    XMLResult rtCreate = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rtCreate.IsErrorOccured)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns error", rtCreate.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rtCreate.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rtCreate.SingleResult == null || rtCreate.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rtCreate.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rtCreate.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info
                    CheckPoint cpGetCrossSectionCompleteInfo = new CheckPoint("GetCrossSectionInfo", "Step 2: Call CrossSectionService.GetCrossSectionCompleteInfo to get the crosssection complete info");
                    r.CheckPoints.Add(cpGetCrossSectionCompleteInfo);

                    string pCrossSectionUID = rtCreate.SingleResult;

                    XMLResult rtGetCrossSectionCompleteInfo = crossSectionSvc.getCrossSectionCompleteInfo(pCrossSectionUID);

                    if (rtGetCrossSectionCompleteInfo.IsErrorOccured)
                    {
                        cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;
                        cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "GetCrossSectionCompleteInfo returns error", rtGetCrossSectionCompleteInfo.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else // Check the return value details
                    {
                        #region Check the value in getCrossSectionCompleteInfo return
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in crossSectionCompleteInfo_Slices_Path.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rtGetCrossSectionCompleteInfo.MultiResults[1].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName.ToLower() == rtGetCrossSectionCompleteInfo.MultiResults[1].Parameters[i].ParameterName.ToLower())
                                {
                                    isKeyShow = true;
                                    // As the slice_path is danamically generated and hard to check, here we check the path contain expected value ".dcm" and the file existed 
                                    isValueEqual = rtGetCrossSectionCompleteInfo.MultiResults[1].Parameters[i].ParameterValue.Contains(psNode.ParameterValue) && System.IO.File.Exists(rtGetCrossSectionCompleteInfo.MultiResults[1].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rtGetCrossSectionCompleteInfo.MultiResults[1].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cpGetCrossSectionCompleteInfo.Result = TestResult.Pass;
                            cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return values in getCrossSectionCompleteInfo all match the expected");
                        }
                        #endregion

                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_GetCrossSectionCompleteInfo_SlicesPSXmlGeneralList_Case1481() //Case 1481: 1.9.3.11_getCrossSectionCompleteInfo_slices_ps_xml_general_list
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    //Expected values
                    XMLParameter crossSectionCompleteInfo_slices_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCrossSectionCompleteInfo_slices_ps_xml_general_list")
                        {
                            crossSectionCompleteInfo_slices_ps_xml_general_list.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cpCreate = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cpCreate);

                    XMLResult rtCreate = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rtCreate.IsErrorOccured)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns error", rtCreate.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rtCreate.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rtCreate.SingleResult == null || rtCreate.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rtCreate.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rtCreate.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info
                    CheckPoint cpGetCrossSectionCompleteInfo = new CheckPoint("GetCrossSectionInfo", "Step 2: Call CrossSectionService.GetCrossSectionCompleteInfo to get the crosssection complete info");
                    r.CheckPoints.Add(cpGetCrossSectionCompleteInfo);

                    string pCrossSectionUID = rtCreate.SingleResult;

                    XMLResult rtGetCrossSectionCompleteInfo = crossSectionSvc.getCrossSectionCompleteInfo(pCrossSectionUID);

                    if (rtGetCrossSectionCompleteInfo.IsErrorOccured)
                    {
                        cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;
                        cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "GetCrossSectionCompleteInfo returns error", rtGetCrossSectionCompleteInfo.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else // Check the return value details
                    {
                        #region Check the value in getCrossSectionCompleteInfo return
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        XMLParameter slices_ps_xml_general_list = null;

                        for (int i = 0; i < rtGetCrossSectionCompleteInfo.MultiResults.Count; i++)
                        {
                            if (rtGetCrossSectionCompleteInfo.MultiResults[i].Name == "slices_ps_xml_general_list")
                            {
                                slices_ps_xml_general_list = rtGetCrossSectionCompleteInfo.MultiResults[i];
                                break;
                            }
                        }

                        foreach (XMLParameterNode psNode in crossSectionCompleteInfo_slices_ps_xml_general_list.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < slices_ps_xml_general_list.Parameters.Count; i++)
                            {
                                if (psNode.ParameterName.ToLower() == slices_ps_xml_general_list.Parameters[i].ParameterName.ToLower())
                                {
                                    isKeyShow = true;
                                    isValueEqual = psNode.ParameterValue.Equals(slices_ps_xml_general_list.Parameters[i].ParameterValue);
                                    if (isValueEqual)  // Here can't end current for loop to search node as there are the same Key name
                                    {
                                        break;
                                    }
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + slices_ps_xml_general_list.Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cpGetCrossSectionCompleteInfo.Result = TestResult.Pass;
                            cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return values in getCrossSectionCompleteInfo all match the expected");
                        }
                        #endregion

                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_GetCrossSectionCompleteInfo_SlicesPSXmlProcessingList_Case1482() //Case 1482: 1.9.3.12_getCrossSectionCompleteInfo_slices_ps_xml_processing_list
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    //Expected values
                    XMLParameter crossSectionCompleteInfo_slices_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCrossSectionCompleteInfo_slices_ps_xml_processing_list")
                        {
                            crossSectionCompleteInfo_slices_ps_xml_processing_list.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cpCreate = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cpCreate);

                    XMLResult rtCreate = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rtCreate.IsErrorOccured)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns error", rtCreate.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rtCreate.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rtCreate.SingleResult == null || rtCreate.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rtCreate.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rtCreate.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info
                    CheckPoint cpGetCrossSectionCompleteInfo = new CheckPoint("GetCrossSectionInfo", "Step 2: Call CrossSectionService.GetCrossSectionCompleteInfo to get the crosssection complete info");
                    r.CheckPoints.Add(cpGetCrossSectionCompleteInfo);

                    string pCrossSectionUID = rtCreate.SingleResult;

                    XMLResult rtGetCrossSectionCompleteInfo = crossSectionSvc.getCrossSectionCompleteInfo(pCrossSectionUID);

                    if (rtGetCrossSectionCompleteInfo.IsErrorOccured)
                    {
                        cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;
                        cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "GetCrossSectionCompleteInfo returns error", rtGetCrossSectionCompleteInfo.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else // Check the return value details
                    {
                        #region Check the value in getCrossSectionCompleteInfo return
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        XMLParameter slices_ps_xml_processing_list = null;

                        for (int i = 0; i < rtGetCrossSectionCompleteInfo.MultiResults.Count; i++)
                        {
                            if (rtGetCrossSectionCompleteInfo.MultiResults[i].Name == "slices_ps_xml_processing_list")
                            {
                                slices_ps_xml_processing_list = rtGetCrossSectionCompleteInfo.MultiResults[i];
                                break;
                            }
                        }

                        foreach (XMLParameterNode psNode in crossSectionCompleteInfo_slices_ps_xml_processing_list.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < slices_ps_xml_processing_list.Parameters.Count; i++)
                            {
                                if (psNode.ParameterName.ToLower() == slices_ps_xml_processing_list.Parameters[i].ParameterName.ToLower())
                                {
                                    isKeyShow = true;
                                    isValueEqual = psNode.ParameterValue.Equals(slices_ps_xml_processing_list.Parameters[i].ParameterValue);
                                    if (isValueEqual)  // Here can't end current for loop to search node as there are the same Key name
                                    {
                                        break;
                                    }
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + slices_ps_xml_processing_list.Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cpGetCrossSectionCompleteInfo.Result = TestResult.Pass;
                            cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return values in getCrossSectionCompleteInfo all match the expected");
                        }
                        #endregion

                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_GetCrossSectionCompleteInfo_SlicesPSXmlAnnotationList_Case1483() //Case 1483: 1.9.3.10_getCrossSectionCompleteInfo_slices_ps_xml_annotation_list
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    //Expected values
                    XMLParameter crossSectionCompleteInfo_slices_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCrossSectionCompleteInfo_slices_ps_xml_annotation_list")
                        {
                            crossSectionCompleteInfo_slices_ps_xml_annotation_list.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cpCreate = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cpCreate);

                    XMLResult rtCreate = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rtCreate.IsErrorOccured)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns error", rtCreate.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rtCreate.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rtCreate.SingleResult == null || rtCreate.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rtCreate.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rtCreate.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info
                    CheckPoint cpGetCrossSectionCompleteInfo = new CheckPoint("GetCrossSectionInfo", "Step 2: Call CrossSectionService.GetCrossSectionCompleteInfo to get the crosssection complete info");
                    r.CheckPoints.Add(cpGetCrossSectionCompleteInfo);

                    string pCrossSectionUID = rtCreate.SingleResult;

                    XMLResult rtGetCrossSectionCompleteInfo = crossSectionSvc.getCrossSectionCompleteInfo(pCrossSectionUID);

                    if (rtGetCrossSectionCompleteInfo.IsErrorOccured)
                    {
                        cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;
                        cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "GetCrossSectionCompleteInfo returns error", rtGetCrossSectionCompleteInfo.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else // Check the return value details
                    {
                        #region Check the value in getCrossSectionCompleteInfo return
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        XMLParameter slices_ps_xml_annotation_list = null;

                        for (int i = 0; i < rtGetCrossSectionCompleteInfo.MultiResults.Count; i++)
                        {
                            if (rtGetCrossSectionCompleteInfo.MultiResults[i].Name == "slices_ps_xml_annotation_list")
                            {
                                slices_ps_xml_annotation_list = rtGetCrossSectionCompleteInfo.MultiResults[i];
                                break;
                            }
                        }

                        foreach (XMLParameterNode psNode in crossSectionCompleteInfo_slices_ps_xml_annotation_list.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < slices_ps_xml_annotation_list.Parameters.Count; i++)
                            {
                                if (psNode.ParameterName.ToLower() == slices_ps_xml_annotation_list.Parameters[i].ParameterName.ToLower())
                                {
                                    isKeyShow = true;
                                    isValueEqual = psNode.ParameterValue.Equals(slices_ps_xml_annotation_list.Parameters[i].ParameterValue);
                                    if (isValueEqual)  // Here can't end current for loop to search node as there are the same Key name
                                    {
                                        break;
                                    }
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + slices_ps_xml_annotation_list.Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cpGetCrossSectionCompleteInfo.Result = TestResult.Pass;
                            cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return values in getCrossSectionCompleteInfo all match the expected");
                        }
                        #endregion

                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_GetCrossSectionCompleteInfo_BasicCrosssectionInfo_Case1484()  //Case 1484: 1.9.3.8_getCrossSectionCompleteInfo_basic crosssection info
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crosssection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);

                    //Expected values
                    XMLParameter crossSectionCompleteInfo_crosssection = new XMLParameter("crosssection");

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCrossSectionCompleteInfo_crosssection")
                        {
                            crossSectionCompleteInfo_crosssection.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cpCreate = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cpCreate);

                    XMLResult rtCreate = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rtCreate.IsErrorOccured)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns error", rtCreate.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rtCreate.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rtCreate.SingleResult == null || rtCreate.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rtCreate.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rtCreate.SingleResult);
                        }
                    }
                    #endregion

                    System.Threading.Thread.Sleep(3000);

                    #region Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info
                    CheckPoint cpGetCrossSectionCompleteInfo = new CheckPoint("GetCrossSectionInfo", "Step 2: Call CrossSectionService.GetCrossSectionInfo to get the crosssection info");
                    r.CheckPoints.Add(cpGetCrossSectionCompleteInfo);

                    string pCrossSectionUID = rtCreate.SingleResult;

                    //pCrossSectionUID = "18e8acb3-1a3e-48c1-8682-7b7eac9a2208";

                    XMLResult rtGetCrossSectionCompleteInfo = crossSectionSvc.getCrossSectionCompleteInfo(pCrossSectionUID);

                    if (rtGetCrossSectionCompleteInfo.IsErrorOccured)
                    {
                        cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;
                        cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "GetCrossSectionCompleteInfo returns error", rtGetCrossSectionCompleteInfo.Message);

                        goto CLEANUP;
                    }
                    else // Check the return value details
                    {
                        #region Check the value in getCrossSectionCompleteInfo return
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in crossSectionCompleteInfo_crosssection.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rtGetCrossSectionCompleteInfo.MultiResults[0].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName.ToLower() == rtGetCrossSectionCompleteInfo.MultiResults[0].Parameters[i].ParameterName.ToLower())
                                {
                                    isKeyShow = true;
                                    isValueEqual = string.Equals(psNode.ParameterValue, rtGetCrossSectionCompleteInfo.MultiResults[0].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cpGetCrossSectionCompleteInfo.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rtGetCrossSectionCompleteInfo.MultiResults[0].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cpGetCrossSectionCompleteInfo.Result = TestResult.Pass;
                            cpGetCrossSectionCompleteInfo.Outputs.AddParameter("GetCrossSectionCompleteInfo", "Check the Cross Section info in getCrossSectionCompleteInfo return", "The return values in getCrossSectionCompleteInfo all match the expected");
                        }
                        #endregion
                    }
                    #endregion

                CLEANUP:
                    #region Step 3: Clean up, delete the crossSection
                    crossSectionSvc.deleteCrossSection(pCrossSectionUID);
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_CreateCrossSection_DicomInfo_Case1500() //Case 1500: 1.9.3.1_createCrossSection_DicomInfo
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {

                    //Input parameters
                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSection = new XMLParameterCollection();
                    XMLParameter crosssection = new XMLParameter("crosssection");
                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");
                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");
                    XMLParameter slicesDicomInfoList = new XMLParameter("slices_dicom_info_list");


                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crossSection")
                        {
                            crosssection.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_dicom_info_list")
                        {
                            slicesDicomInfoList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }
                    p_createCrossSection.Add(crosssection);
                    p_createCrossSection.Add(slicesPathList);
                    p_createCrossSection.Add(slicesPSAnnotationList);
                    p_createCrossSection.Add(slicesPSGeneralList);
                    p_createCrossSection.Add(slicesPSProcessingList);
                    p_createCrossSection.Add(slicesThumbnailList);
                    p_createCrossSection.Add(slicesDicomInfoList);

                    CrossSectionService crossSectionSvc = new CrossSectionService();

                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cp_Create = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cp_Create);

                    XMLResult rt_Create = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSection);

                    if (rt_Create.IsErrorOccured)
                    {
                        cp_Create.Result = TestResult.Fail;
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns error", rt_Create.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns succeess", rt_Create.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create.SingleResult == null || rt_Create.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cp_Create.Result = TestResult.Fail;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns wrong internal_id: ", rt_Create.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Create.Result = TestResult.Pass;
                            cp_Create.Outputs.AddParameter("Create", "Create a new crosssection returns correct internal_id: ", rt_Create.SingleResult);
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();

        }

        public void Run_3D_deleteCrossSection_deleteoneparent_Case1501() //Case 1501: 1.9.3.1_deleteCrossSection_localizer_has_three_parent_and_delete_one_parent
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "delete_one_cs_shared_localizer");
                CheckPoint pCreate = new CheckPoint("Workflow", "delete_one_cs_shared_localizer");
                r.CheckPoints.Add(pCreate);

                CrossSectionService cs = new CrossSectionService();

                XMLParameterCollection csInputData1 = new XMLParameterCollection();
                XMLParameterCollection csInputData2 = new XMLParameterCollection();
                XMLParameterCollection csInputData3 = new XMLParameterCollection();

                XMLParameter cs1 = new XMLParameter("crosssection");
                XMLParameter cs2 = new XMLParameter("crosssection");
                XMLParameter cs3 = new XMLParameter("crosssection");

                XMLParameter cs1_slicepath = new XMLParameter("slices_path_list");
                XMLParameter cs2_slicepath = new XMLParameter("slices_path_list");
                XMLParameter cs3_slicepath = new XMLParameter("slices_path_list");

                XMLParameter cs1_slices_dicom_info_list = new XMLParameter("slices_dicom_info_list");
                XMLParameter cs2_slices_dicom_info_list = new XMLParameter("slices_dicom_info_list");
                XMLParameter cs3_slices_dicom_info_list = new XMLParameter("slices_dicom_info_list");

                XMLParameter cs1_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");
                XMLParameter cs2_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");
                XMLParameter cs3_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");

                XMLParameter cs1_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");
                XMLParameter cs2_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");
                XMLParameter cs3_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");

                XMLParameter cs1_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");
                XMLParameter cs2_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");
                XMLParameter cs3_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");

                string volume1 = string.Empty;
                string volume2 = string.Empty;
                string volume3 = string.Empty;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "volume1":
                            volume1 = ids.InputParameters.GetParameter(i).Value;
                            break;
                        case "volume2":
                            volume2 = ids.InputParameters.GetParameter(i).Value;
                            break;
                        case "volume3":
                            volume3 = ids.InputParameters.GetParameter(i).Value;
                            break;
                        case "createcs_crosssection":
                            cs1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_dicom_info_list":
                            cs1_slices_dicom_info_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_slices_dicom_info_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_slices_dicom_info_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_ps_xml_annotation_list":
                            cs1_ps_xml_annotation_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_ps_xml_annotation_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_ps_xml_annotation_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_ps_xml_general_list":
                            cs1_ps_xml_general_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_ps_xml_general_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_ps_xml_general_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_ps_xml_processing_list":
                            cs1_ps_xml_processing_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_ps_xml_processing_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_ps_xml_processing_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs1_crosssection":
                            cs1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs2_crosssection":
                            cs2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs3_crosssection":
                            cs3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs1_slices_path_list":
                            cs1_slicepath.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs2_slices_path_list":
                            cs2_slicepath.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs3_slices_path_list":
                            cs3_slicepath.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;

                    }
                }
                csInputData1.Add(cs1);
                csInputData1.Add(cs1_slicepath);
                csInputData1.Add(cs1_slices_dicom_info_list);
                csInputData1.Add(cs1_ps_xml_annotation_list);
                csInputData1.Add(cs1_ps_xml_general_list);
                csInputData1.Add(cs1_ps_xml_processing_list);

                csInputData2.Add(cs2);
                csInputData2.Add(cs2_slicepath);
                csInputData2.Add(cs2_slices_dicom_info_list);
                csInputData2.Add(cs2_ps_xml_annotation_list);
                csInputData2.Add(cs2_ps_xml_general_list);
                csInputData2.Add(cs2_ps_xml_processing_list);

                csInputData3.Add(cs3);
                csInputData3.Add(cs3_slicepath);
                csInputData3.Add(cs3_slices_dicom_info_list);
                csInputData3.Add(cs3_ps_xml_annotation_list);
                csInputData3.Add(cs3_ps_xml_general_list);
                csInputData3.Add(cs3_ps_xml_processing_list);
                string test = csInputData1.GenerateXML();
                XMLResult rslCreate1 = cs.createCrossSection(volume1, csInputData1);
                XMLResult rslCreate2 = cs.createCrossSection(volume2, csInputData2);
                XMLResult rslCreate3 = cs.createCrossSection(volume3, csInputData3);

                string csid1 = rslCreate1.SingleResult;
                string csid2 = rslCreate2.SingleResult;
                string csid3 = rslCreate3.SingleResult;

                XMLResult rslDelete = cs.deleteCrossSection(csid1);

                XMLResult rslList = cs.listImagesOfCrossSection(csid2);
                string first_localizer = rslList.SingleResult;

                if (!rslCreate1.IsErrorOccured && !rslCreate2.IsErrorOccured && !rslCreate3.IsErrorOccured && !rslDelete.IsErrorOccured && first_localizer != string.Empty)
                {

                    pCreate.Result = TestResult.Pass;
                    pCreate.Outputs.AddParameter("Workflow", "delete one crosssection shared localizer", "OK");

                }
                else
                {
                    pCreate.Result = TestResult.Fail;
                    string msg = rslCreate1.Message + "," + rslCreate2.Message + "," + rslCreate3.Message + rslDelete.Message + ",localizer" + first_localizer;
                    pCreate.Outputs.AddParameter("Workflow", "delete one crosssection shared localizer", msg);

                }

                cs.deleteCrossSection(csid2);
                cs.deleteCrossSection(csid3);

                SaveRound(r);
                Output();

            }

        }

        public void Run_3D_deleteCrossSection_deleteallparents_Case1502() //Case 1502: 1.9.3.1_deleteCrossSection_localizer_has_three_parents_and_delete_all_parents
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "delete_all_cs_shared_localizer");
                CheckPoint pCreate = new CheckPoint("Workflow", "delete_all_cs_shared_localizer");
                r.CheckPoints.Add(pCreate);

                CrossSectionService cs = new CrossSectionService();

                XMLParameterCollection csInputData1 = new XMLParameterCollection();
                XMLParameterCollection csInputData2 = new XMLParameterCollection();
                XMLParameterCollection csInputData3 = new XMLParameterCollection();

                XMLParameter cs1 = new XMLParameter("crosssection");
                XMLParameter cs2 = new XMLParameter("crosssection");
                XMLParameter cs3 = new XMLParameter("crosssection");

                XMLParameter cs1_slicepath = new XMLParameter("slices_path_list");
                XMLParameter cs2_slicepath = new XMLParameter("slices_path_list");
                XMLParameter cs3_slicepath = new XMLParameter("slices_path_list");

                XMLParameter cs1_slices_dicom_info_list = new XMLParameter("slices_dicom_info_list");
                XMLParameter cs2_slices_dicom_info_list = new XMLParameter("slices_dicom_info_list");
                XMLParameter cs3_slices_dicom_info_list = new XMLParameter("slices_dicom_info_list");

                XMLParameter cs1_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");
                XMLParameter cs2_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");
                XMLParameter cs3_ps_xml_annotation_list = new XMLParameter("slices_ps_xml_annotation_list");

                XMLParameter cs1_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");
                XMLParameter cs2_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");
                XMLParameter cs3_ps_xml_general_list = new XMLParameter("slices_ps_xml_general_list");

                XMLParameter cs1_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");
                XMLParameter cs2_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");
                XMLParameter cs3_ps_xml_processing_list = new XMLParameter("slices_ps_xml_processing_list");

                string volume1 = string.Empty;
                string volume2 = string.Empty;
                string volume3 = string.Empty;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "volume1":
                            volume1 = ids.InputParameters.GetParameter(i).Value;
                            break;

                        case "createcs_crosssection":
                            cs1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_dicom_info_list":
                            cs1_slices_dicom_info_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_slices_dicom_info_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_slices_dicom_info_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_ps_xml_annotation_list":
                            cs1_ps_xml_annotation_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_ps_xml_annotation_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_ps_xml_annotation_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_ps_xml_general_list":
                            cs1_ps_xml_general_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_ps_xml_general_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_ps_xml_general_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs_slices_ps_xml_processing_list":
                            cs1_ps_xml_processing_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs2_ps_xml_processing_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            cs3_ps_xml_processing_list.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs1_crosssection":
                            cs1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs2_crosssection":
                            cs2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs3_crosssection":
                            cs3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs1_slices_path_list":
                            cs1_slicepath.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs2_slices_path_list":
                            cs2_slicepath.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;
                        case "createcs3_slices_path_list":
                            cs3_slicepath.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                            break;

                    }
                }
                csInputData1.Add(cs1);
                csInputData1.Add(cs1_slicepath);
                csInputData1.Add(cs1_slices_dicom_info_list);
                csInputData1.Add(cs1_ps_xml_annotation_list);
                csInputData1.Add(cs1_ps_xml_general_list);
                csInputData1.Add(cs1_ps_xml_processing_list);

                csInputData2.Add(cs2);
                csInputData2.Add(cs2_slicepath);
                csInputData2.Add(cs2_slices_dicom_info_list);
                csInputData2.Add(cs2_ps_xml_annotation_list);
                csInputData2.Add(cs2_ps_xml_general_list);
                csInputData2.Add(cs2_ps_xml_processing_list);

                csInputData3.Add(cs3);
                csInputData3.Add(cs3_slicepath);
                csInputData3.Add(cs3_slices_dicom_info_list);
                csInputData3.Add(cs3_ps_xml_annotation_list);
                csInputData3.Add(cs3_ps_xml_general_list);
                csInputData3.Add(cs3_ps_xml_processing_list);
                string test = csInputData1.GenerateXML();
                XMLResult rslCreate1 = cs.createCrossSection(volume1, csInputData1);
                XMLResult rslCreate2 = cs.createCrossSection(volume1, csInputData2);
                XMLResult rslCreate3 = cs.createCrossSection(volume1, csInputData3);

                string csid1 = rslCreate1.SingleResult;
                string csid2 = rslCreate2.SingleResult;
                string csid3 = rslCreate3.SingleResult;

                XMLResult rslList = cs.listImagesOfCrossSection(csid1);
                string first_localizer = rslList.SingleResult;

                XMLResult rslDelete1 = cs.deleteCrossSection(csid1);
                XMLResult rslDelete2 = cs.deleteCrossSection(csid2);
                XMLResult rslDelete3 = cs.deleteCrossSection(csid3);

                ImageService imgsvr = new ImageService();
                XMLParameter cInputImage = new XMLParameter("image");
                cInputImage.AddParameter("internal_id", first_localizer);

                XMLResult rslget = imgsvr.getImageInfo(cInputImage);



                if (!rslCreate1.IsErrorOccured && !rslCreate2.IsErrorOccured && !rslCreate3.IsErrorOccured && !rslDelete1.IsErrorOccured && !rslDelete2.IsErrorOccured && !rslDelete3.IsErrorOccured && rslget.Code == 1006)
                {

                    pCreate.Result = TestResult.Pass;
                    pCreate.Outputs.AddParameter("Workflow", "delete all crosssection shared localizer", "OK");

                }
                else
                {
                    pCreate.Result = TestResult.Fail;
                    string msg = "Create:" + rslCreate1.Message + "," + rslCreate2.Message + "," + rslCreate3.Message + ".Delete:" + rslDelete1.Message + rslDelete2.Message + rslDelete3.Message + ". GetImageCode:" + rslget.Code;
                    pCreate.Outputs.AddParameter("Workflow", "delete all crosssection shared localizer", msg);

                }

                SaveRound(r);
                Output();

            }

        }

        public void Run_3D_createCrossSection_localizeruid_exist_Case1503() //Case 1503: 1.9.3.1_createCrossSection_localizer_dicom_series_instance_uid_exist
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    //Input parameters
                    string crossectionUID_FirstTime = string.Empty;
                    string crossectionUID_SecondTime = string.Empty;

                    string ep_first_localizer_dicom_series_instance_uid = string.Empty;
                    string ep_second_localizer_dicom_series_instance_uid = string.Empty;

                    string p_volumeUID = null;
                    XMLParameterCollection p_createCrossSectionFirstTime = new XMLParameterCollection();
                    XMLParameterCollection p_createCrossSectionSecondTime = new XMLParameterCollection();

                    XMLParameter crosssectionFirstTime = new XMLParameter("crosssection");
                    XMLParameter crosssectionSecondTime = new XMLParameter("crosssection");

                    XMLParameter slicesPathList = new XMLParameter("slices_path_list");
                    XMLParameter slicesPSAnnotationList = new XMLParameter("slices_ps_xml_annotation_list");
                    XMLParameter slicesPSGeneralList = new XMLParameter("slices_ps_xml_general_list");
                    XMLParameter slicesPSProcessingList = new XMLParameter("slices_ps_xml_processing_list");

                    XMLParameter slicesThumbnailList = new XMLParameter("slices_ps_thumbnail_path_list");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "create_volumeUID")
                        {
                            p_volumeUID = ids.InputParameters.GetParameter(i).Value;
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_crossSection")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "slices_dicom_series_instance_uid")
                            {
                                crosssectionFirstTime.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value + "_firsttime", false);
                                crosssectionSecondTime.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value + "_secondtime", false);
                            }
                            else
                            {
                                crosssectionFirstTime.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                crosssectionSecondTime.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                                if (ids.InputParameters.GetParameter(i).Key == "first_localizer_dicom_series_instance_uid")
                                {
                                    ep_first_localizer_dicom_series_instance_uid = ids.InputParameters.GetParameter(i).Value; // Record the first_localizer_dicom_series_instance_uid value to check later
                                }
                                else if (ids.InputParameters.GetParameter(i).Key == "second_localizer_dicom_series_instance_uid")
                                {
                                    ep_second_localizer_dicom_series_instance_uid = ids.InputParameters.GetParameter(i).Value; // Record the second_localizer_dicom_series_instance_uid value to check later
                                }
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_path_list")
                        {
                            slicesPathList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_annotation_list")
                        {
                            slicesPSAnnotationList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_general_list")
                        {
                            slicesPSGeneralList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_xml_processing_list")
                        {
                            slicesPSProcessingList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "create_slices_ps_thumbnail_path_list")
                        {
                            slicesThumbnailList.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value, false);
                        }
                    }

                    p_createCrossSectionFirstTime.Add(crosssectionFirstTime);
                    p_createCrossSectionFirstTime.Add(slicesPathList);
                    p_createCrossSectionFirstTime.Add(slicesPSAnnotationList);
                    p_createCrossSectionFirstTime.Add(slicesPSGeneralList);
                    p_createCrossSectionFirstTime.Add(slicesPSProcessingList);
                    p_createCrossSectionFirstTime.Add(slicesThumbnailList);


                    p_createCrossSectionSecondTime.Add(crosssectionSecondTime);
                    p_createCrossSectionSecondTime.Add(slicesPathList);
                    p_createCrossSectionSecondTime.Add(slicesPSAnnotationList);
                    p_createCrossSectionSecondTime.Add(slicesPSGeneralList);
                    p_createCrossSectionSecondTime.Add(slicesPSProcessingList);
                    p_createCrossSectionSecondTime.Add(slicesThumbnailList);

                    //Output parameter


                    CrossSectionService crossSectionSvc = new CrossSectionService();
                    #region Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection
                    CheckPoint cp_Create_FirstTime = new CheckPoint("Create", "Step 1: Call CrossSectionService.CreateCrossSection to create a new crosssection");
                    r.CheckPoints.Add(cp_Create_FirstTime);

                    XMLResult rt_Create_FirstTime = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSectionFirstTime);

                    if (rt_Create_FirstTime.IsErrorOccured)
                    {
                        cp_Create_FirstTime.Result = TestResult.Fail;
                        cp_Create_FirstTime.Outputs.AddParameter("Create a new crosssection returns error", "Create", rt_Create_FirstTime.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_Create_FirstTime.Outputs.AddParameter("Create  a new crosssection returns succeess", "Create", rt_Create_FirstTime.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create_FirstTime.SingleResult == null || rt_Create_FirstTime.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cp_Create_FirstTime.Result = TestResult.Fail;
                            cp_Create_FirstTime.Outputs.AddParameter("Create  a new crosssection returns wrong internal_id: ", "Create", rt_Create_FirstTime.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Create_FirstTime.Result = TestResult.Pass;
                            cp_Create_FirstTime.Outputs.AddParameter("Create  a new crosssection returns correct internal_id: ", "Create", rt_Create_FirstTime.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 2: Create for the second time to resue the localizer
                    CheckPoint cp_Create_SecondTime = new CheckPoint("Create", "Step 2: Call CrossSectionService.CreateCrossSection agian to create new crosssection, given the same localizer uid"); // Create for the second time to resue the localizer
                    r.CheckPoints.Add(cp_Create_SecondTime);
                    XMLResult rt_Create_SecondTime = crossSectionSvc.createCrossSection(p_volumeUID, p_createCrossSectionSecondTime);

                    if (rt_Create_SecondTime.IsErrorOccured)
                    {
                        cp_Create_SecondTime.Result = TestResult.Fail;
                        cp_Create_SecondTime.Outputs.AddParameter("Create  a new crosssection returns error", "Create", rt_Create_SecondTime.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_Create_SecondTime.Outputs.AddParameter("Create  a new crosssection returns succeess", "Create", rt_Create_SecondTime.Message);

                        //Check the "internal_id" in return is present
                        bool isInternalIDCorrect = true;

                        if (rt_Create_SecondTime.SingleResult == null || rt_Create_SecondTime.SingleResult == String.Empty)
                        {
                            isInternalIDCorrect = false;
                        }

                        if (!isInternalIDCorrect)
                        {
                            cp_Create_SecondTime.Result = TestResult.Fail;
                            cp_Create_SecondTime.Outputs.AddParameter("Create  a new crosssection returns wrong internal_id: ", "Create", rt_Create_SecondTime.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Create_SecondTime.Result = TestResult.Pass;
                            cp_Create_SecondTime.Outputs.AddParameter("Create a new crosssection returns correct internal_id: ", "Create", rt_Create_SecondTime.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 3: Check the crossSection info is correct
                    CheckPoint cp_CheckCrossSectionCompleteInfo = new CheckPoint("Create", "Step 3-1: Call CrossSectionService.getCrossSectionCompleteInfo to check the localizer info");
                    r.CheckPoints.Add(cp_CheckCrossSectionCompleteInfo);

                    crossectionUID_FirstTime = rt_Create_FirstTime.SingleResult;
                    crossectionUID_SecondTime = rt_Create_SecondTime.SingleResult;

                    // call getCrossSectionCompleteInfo
                    XMLResult rt_getCrossSectionCompleteInfoForFirstCreate = crossSectionSvc.getCrossSectionCompleteInfo(crossectionUID_FirstTime);
                    XMLResult rt_getCrossSectionCompleteInfoForSecondCreate = crossSectionSvc.getCrossSectionCompleteInfo(crossectionUID_SecondTime);

                    string first_localizer_dicom_series = "<parameter key=\"first_localizer_dicom_series_instance_uid\" value=\"" + ep_first_localizer_dicom_series_instance_uid + "\" />";   //<parameter key="first_localizer_dicom_series_instance_uid" value="%loc1_dicom_series_instance_uid%" />
                    string second_localizer_dicom_series = "<parameter key=\"second_localizer_dicom_series_instance_uid\" value=\"" + ep_second_localizer_dicom_series_instance_uid + "\" />"; //<parameter key="second_localizer_dicom_series_instance_uid" value="%loc2_dicom_series_instance_uid%" />
                    if (rt_getCrossSectionCompleteInfoForFirstCreate.ResultContent.Contains(first_localizer_dicom_series) && rt_getCrossSectionCompleteInfoForFirstCreate.ResultContent.Contains(second_localizer_dicom_series) && rt_getCrossSectionCompleteInfoForSecondCreate.ResultContent.Contains(first_localizer_dicom_series) && rt_getCrossSectionCompleteInfoForSecondCreate.ResultContent.Contains(second_localizer_dicom_series))
                    {
                        cp_CheckCrossSectionCompleteInfo.Outputs.AddParameter("The localizer info in GetCrossSectionCompleteInfo is correct", "GetCrossSectionCompleteInfo", "");
                        cp_CheckCrossSectionCompleteInfo.Result = TestResult.Pass;
                    }
                    else
                    {
                        cp_CheckCrossSectionCompleteInfo.Outputs.AddParameter("The localizer info in GetCrossSectionCompleteInfo is wrong", "GetCrossSectionCompleteInfo", "");
                        cp_CheckCrossSectionCompleteInfo.Result = TestResult.Fail;
                    }
                    cp_CheckCrossSectionCompleteInfo.Outputs.AddParameter("The GetCrossSectionCompleteInfo return for the first CrossSection is: ", "GetCrossSectionCompleteInfo", rt_getCrossSectionCompleteInfoForFirstCreate.ResultContent);
                    cp_CheckCrossSectionCompleteInfo.Outputs.AddParameter("The GetCrossSectionCompleteInfo return for the second CrossSection is: ", "GetCrossSectionCompleteInfo", rt_getCrossSectionCompleteInfoForSecondCreate.ResultContent);

                    // Call listImagesOfCrossSection
                    CheckPoint cp_CheckListImagesOfCrossSection = new CheckPoint("Create", "Step 3-2: Call CrossSectionService.listImagesOfCrossSection to check the localizer info");
                    r.CheckPoints.Add(cp_CheckListImagesOfCrossSection);

                    XMLResult rt_listImagesOfCrossSectionForFirstCreate = crossSectionSvc.listImagesOfCrossSection(crossectionUID_FirstTime);
                    XMLResult rt_listImagesOfCrossSectionForSecondCreate = crossSectionSvc.listImagesOfCrossSection(crossectionUID_SecondTime);

                    /**************************************
                     retrun sample:
                         <trophy type="result" version="1.0">
                            <status code="%error_code%" message="%error_message%" />
                            <images_list>
                                <parameter key="first_localizer_internal_id" value="%image_uid%" />
                                <parameter key="second_localizer_internal_id" value="%image_uid%" />
                                <parameter key="slice_internal_id" value="%image_uid%" />
                                ...
                            </images_list>
                         </trophy> 
                     * *************************************/

                    string first_localizer_internal_id_FirstCreate = string.Empty;
                    string second_localizer_internal_id_FirstCreate = string.Empty;
                    string first_localizer_internal_id_SecondCreate = string.Empty;
                    string second_localizer_internal_id_SecondCreate = string.Empty;

                    foreach (XMLParameterNode node in rt_listImagesOfCrossSectionForFirstCreate.MultiResults[0].Parameters)
                    {
                        switch (node.ParameterName)
                        {
                            case "first_localizer_internal_id":
                                first_localizer_internal_id_FirstCreate = node.ParameterValue;
                                break;
                            case "second_localizer_internal_id":
                                second_localizer_internal_id_FirstCreate = node.ParameterValue;
                                break;
                            default:
                                break;
                        }
                    }
                    foreach (XMLParameterNode node in rt_listImagesOfCrossSectionForSecondCreate.MultiResults[0].Parameters)
                    {
                        switch (node.ParameterName)
                        {
                            case "first_localizer_internal_id":
                                first_localizer_internal_id_SecondCreate = node.ParameterValue;
                                break;
                            case "second_localizer_internal_id":
                                second_localizer_internal_id_SecondCreate = node.ParameterValue;
                                break;
                            default:
                                break;
                        }
                    }
                    if (first_localizer_internal_id_FirstCreate == first_localizer_internal_id_SecondCreate && second_localizer_internal_id_FirstCreate == second_localizer_internal_id_SecondCreate)
                    {
                        cp_CheckListImagesOfCrossSection.Outputs.AddParameter("The localizer info in listImagesOfCrossSection is correct", "listImagesOfCrossSection", "");
                        cp_CheckListImagesOfCrossSection.Result = TestResult.Pass;
                    }
                    else
                    {
                        cp_CheckListImagesOfCrossSection.Outputs.AddParameter("The localizer info in listImagesOfCrossSection is wrong", "listImagesOfCrossSection", "");
                        cp_CheckListImagesOfCrossSection.Result = TestResult.Fail;
                    }
                    cp_CheckListImagesOfCrossSection.Outputs.AddParameter("The localizer info in listImagesOfCrossSection for the first crossSection is: ", "listImagesOfCrossSection", rt_listImagesOfCrossSectionForFirstCreate.ResultContent);
                    cp_CheckListImagesOfCrossSection.Outputs.AddParameter("The localizer info in listImagesOfCrossSection for the second crossSection is: ", "listImagesOfCrossSection", rt_listImagesOfCrossSectionForSecondCreate.ResultContent);

                    #endregion

                    #region Step 4: Call CrossSectionService.deleteCrossSection to delete the created crosssection
                    System.Collections.Generic.List<string> crossSectionUIDList = new System.Collections.Generic.List<string>();
                    crossSectionUIDList.Add(crossectionUID_FirstTime);
                    crossSectionUIDList.Add(crossectionUID_SecondTime);

                    foreach (string p_crossectionUID in crossSectionUIDList)
                    {
                        CheckPoint cp_Delete = new CheckPoint("Delete", "Step 4: Call CrossSectionService.deleteCrossSection to delete the created crosssection");
                        r.CheckPoints.Add(cp_Delete);

                        XMLResult rt_Delete = crossSectionSvc.deleteCrossSection(p_crossectionUID);

                        if (rt_Delete.IsErrorOccured)
                        {
                            cp_Delete.Result = TestResult.Fail;
                            cp_Delete.Outputs.AddParameter("Delete the created crosssection returns error", "Delete", rt_Delete.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            cp_Delete.Result = TestResult.Pass;
                            cp_Delete.Outputs.AddParameter("Delete the created crosssection returns succeess. ID is: ", "Delete", p_crossectionUID);
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_3D_SetNameFor3DObjects_Case1627() //Case 1627: 1.1.12.01_Set Name for 3d objects
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Set 3D Object Name");
                CheckPoint pset = new CheckPoint("Set 3DObject Name", " Check for 3d object set name");
                r.CheckPoints.Add(pset);

                string threedid = ids.InputParameters.GetParameter("threedid" + runCount.ToString()).Value;
                string testname = ids.InputParameters.GetParameter("testname" + runCount.ToString()).Value;

                XMLParameter cInputSet = new XMLParameter("volume");
                cInputSet.AddParameter("name", testname);

                XMLParameter cInputGet = new XMLParameter("volume");
                cInputGet.AddParameter("internal_id", threedid);
                cInputGet.AddParameter("path_type", "all");

                try
                {

                    VolumeService vs = new VolumeService();

                    XMLResult rslset = vs.setVolume(threedid, cInputSet);
                    if (rslset.IsErrorOccured)
                    {
                        pset.Result = TestResult.Fail;
                        pset.Outputs.AddParameter("set volume", "fail", rslset.Message);
                        SaveRound(r);
                        continue;
                    }

                    XMLResult rslget = vs.getVolumeInfo(cInputGet);

                    if (rslget.IsErrorOccured)
                    {

                        pset.Result = TestResult.Fail;
                        pset.Outputs.AddParameter("get volume", "fail", rslget.Message);
                        SaveRound(r);
                        continue;
                    }

                    string getname = string.Empty;
                    foreach (XMLParameterNode x in rslget.MultiResults[2].Parameters)
                    {
                        if (x.ParameterName == "name")
                        {
                            getname = x.ParameterValue;
                        }
                    }

                    if (getname != testname)
                    {
                        pset.Result = TestResult.Fail;
                        pset.Outputs.AddParameter("set volume", "fail", "Get Value is not equal to set Value");

                        SaveRound(r);
                        continue;
                    }

                    pset.Result = TestResult.Pass;
                    pset.Outputs.AddParameter("set volume", "Success", "OK");
                    SaveRound(r);
                }
                catch (Exception e)
                {
                    pset.Result = TestResult.Fail;
                    pset.Outputs.AddParameter("set volume", "Exception caught", e.Message);

                    SaveRound(r);
                    continue;
                }

            }
            Output();
        }
    }
}