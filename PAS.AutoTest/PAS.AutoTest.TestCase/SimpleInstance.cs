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
        public void Run_SimpleInstance_CreateSimpleInstance_Case161() //Case 161: 1.3.2_CreateSimpleInstance_N1_WithAllParameters
        {
            int runCount = 0;
            string patientUID = string.Empty;
            string instanceUID = string.Empty;
            bool isCreatePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create simple instance");
                CheckPoint pCreate = new CheckPoint("Create patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                //ImageService ims = new ImageService();
                SimpleInstanceService sis = new SimpleInstanceService();
                XMLParameter pa = new XMLParameter("patient");
                XMLParameter sia = new XMLParameter("instance");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "simple")
                    {
                        sia.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }

                }
                if (isCreatePatient)
                {
                    XMLResult result = ps.createPatient(pa);
                    if (!result.IsErrorOccured)
                    {
                        patientUID = result.SingleResult;

                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                        pCreate.Result = TestResult.Pass;
                    }
                }

                CheckPoint pInstance = new CheckPoint("Simple instance create", "Test image");
                r.CheckPoints.Add(pInstance);
                if (patientUID != string.Empty)
                {
                    sia.AddParameter("patient_internal_id", patientUID);
                    XMLResult instanceRsl = sis.createSimpleInstance(sia);
                    if (instanceRsl.IsErrorOccured)
                    {

                        pInstance.Outputs.AddParameter("create", "Create SimpleInstance error", instanceRsl.Message);
                        pInstance.Result = TestResult.Fail;
                    }
                    else
                    {
                        instanceUID = instanceRsl.SingleResult;
                        pInstance.Outputs.AddParameter("create", "Create SimpleInstance Id", instanceUID);
                        pInstance.Result = TestResult.Pass;
                    }
                }

                ps.deletePatient(patientUID);

                SaveRound(r);
            }

            Output();
        }

        public void Run_SimpleInstance_WorkFlow_Case1582() // Create normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();
                    string instanceUID = null;

                    #region Input param
                    SimpleCreateSimpleInstanceRequestType pCreate = new SimpleCreateSimpleInstanceRequestType();
                    pCreate.simpleInstance = new SimpleInstanceType();
                    SimpleSetSimpleInstanceInfoRequestType pSet = new SimpleSetSimpleInstanceInfoRequestType();
                    pSet.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createSimpleInstance")
                        {
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    pCreate.simpleInstance.patientUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "filePath":
                                    pCreate.simpleInstance.filePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    pCreate.simpleInstance.originalPath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    pCreate.simpleInstance.instanceType = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    pCreate.simpleInstance.comments = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    pCreate.simpleInstance.sopInstanceUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Expected Values
                    SimpleGetSimpleInstanceResponseType epGet = new SimpleGetSimpleInstanceResponseType();
                    epGet.simpleInstance = new SimpleInstanceType();

                    SimpleGetSimpleInstanceInfoResponseType epGetInfo = new SimpleGetSimpleInstanceInfoResponseType();
                    epGetInfo.simpleInstance = new SimpleInstanceType();
                    epGetInfo.simpleInstance.filePath = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getSimpleInstance")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGet.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGet.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGet.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGet.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGet.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                //case "fileCreatedTime":  // when create, there is no such info, only for import
                                //    epGet.simpleInstance.fileCreatedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                //case "fileModifiedTime": // when create, there is no such info, only for import
                                //    epGet.simpleInstance.fileModifiedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getSimpleInstanceInfo")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetInfo.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetInfo.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetInfo.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "objectType":
                                    epGetInfo.simpleInstance.objectType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetInfo.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetInfo.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                //case "fileCreatedTime": // when create, there is no such info, only for import
                                //    epGetInfo.simpleInstance.fileCreatedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                //case "fileModifiedTime": // when create, there is no such info, only for import
                                //    epGetInfo.simpleInstance.fileModifiedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                case "fileName":
                                    epGetInfo.simpleInstance.fileName = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Step: Call create to create a normal simple instance
                    instanceUID = simpleInstanceSvc.CallCreateAndCheck(r, pCreate);
                    #endregion

                    #region Step: Call get to a normal simple instance after create
                    simpleInstanceSvc.CallGetAndCheck(r, instanceUID, epGet);
                    #endregion

                    #region Step: Call getInfo to a normal simple isntance after create
                    simpleInstanceSvc.CallGetInfoAndCheck(r, instanceUID, epGetInfo);
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_WorkFlow_Case1583() // Create archived
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    #region Input param
                    SimpleCreateSimpleInstanceRequestType pCreate = new SimpleCreateSimpleInstanceRequestType();
                    pCreate.simpleInstance = new SimpleInstanceType();
                    SimpleSetSimpleInstanceInfoRequestType pSet = new SimpleSetSimpleInstanceInfoRequestType();
                    pSet.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createSimpleInstance")
                        {
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    pCreate.simpleInstance.patientUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    pCreate.simpleInstance.archivePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    pCreate.simpleInstance.originalPath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    pCreate.simpleInstance.instanceType = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    pCreate.simpleInstance.comments = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    pCreate.simpleInstance.sopInstanceUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Expected Values
                    SimpleGetSimpleInstanceResponseType epGet = new SimpleGetSimpleInstanceResponseType();
                    epGet.simpleInstance = new SimpleInstanceType();

                    SimpleGetSimpleInstanceInfoResponseType epGetInfo = new SimpleGetSimpleInstanceInfoResponseType();
                    epGetInfo.simpleInstance = new SimpleInstanceType();
                    epGetInfo.simpleInstance.filePath = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getSimpleInstance")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGet.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    epGet.simpleInstance.archivePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGet.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGet.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGet.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGet.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                //case "fileCreatedTime": //When create, there is no file info returned as no given
                                //    epGet.simpleInstance.fileCreatedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                //case "fileModifiedTime": //When create, there is no file info returned as no given
                                //    epGet.simpleInstance.fileModifiedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getSimpleInstanceInfo")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetInfo.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    epGetInfo.simpleInstance.archivePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetInfo.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetInfo.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "objectType":
                                    epGetInfo.simpleInstance.objectType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetInfo.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetInfo.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                //case "fileCreatedTime": //When create, there is no file info returned
                                //    epGetInfo.simpleInstance.fileCreatedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                //case "fileModifiedTime": //When create, there is no file info returned
                                //    epGetInfo.simpleInstance.fileModifiedTime = ids.ExpectedValues.GetParameter(i).Value;
                                //    break;
                                case "fileName":
                                    epGetInfo.simpleInstance.fileName = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Step: Call create to create a archive simple instance
                    CheckPoint cpCreate = new CheckPoint("Create", "Call create to create a archive simple instance");
                    r.CheckPoints.Add(cpCreate);

                    SimpleCreateSimpleInstanceResponseType rtCreate = simpleInstanceSvc.createSimpleInstance(pCreate);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create Simple Instance", "XML Validation Result", "createSimpleInstance response is not complied with schema. Error:" + simpleInstanceSvc.LastReturnXMLValidateResult.message);
                        cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML:", simpleInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        if (rtCreate.status.code == 0 && rtCreate.status.message == "ok")
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML correct:", simpleInstanceSvc.LastReturnXML);
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML wrong:", simpleInstanceSvc.LastReturnXML);
                            SaveRound(r);
                            continue;
                        }
                    }
                    #endregion

                    #region Step: Call get to the archive simple instance after create
                    CheckPoint cpGetSimpleInstance = new CheckPoint("Get Simple Instance", "Get Simple Instance");
                    r.CheckPoints.Add(cpGetSimpleInstance);

                    SimpleGetSimpleInstanceResponseType rtGetSimpleInstance = simpleInstanceSvc.getSimpleInstance(rtCreate.simpleInstance.uid);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpGetSimpleInstance.Result = TestResult.Fail;
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Invalid XML", "GetSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Return XML", "getSimpleInstance returns result. " + simpleInstanceSvc.LastReturnXML);
                        cpGetSimpleInstance.Result = TestResult.Pass;
                    }

                    if (rtGetSimpleInstance.status.code != 0 || rtGetSimpleInstance.status.message != "ok")
                    {
                        cpGetSimpleInstance.Result = TestResult.Fail;
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check the getSimpleInstance response", "getSimpleInstance doesn't return expected result.  Code:" + rtGetSimpleInstance.status.code.ToString() + "; Message:" + rtGetSimpleInstance.status.message);
                        SaveRound(r);
                        continue;
                    }

                    // Check the return value   
                    if (rtGetSimpleInstance.simpleInstance.patientUid != epGet.simpleInstance.patientUid)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check patientUid", "getSimpleInstance doesn't return expected patientUid." + "Expect: " + epGet.simpleInstance.patientUid + ". Get: " + rtGetSimpleInstance.simpleInstance.patientUid);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstance.simpleInstance.filePath != null)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check filePath", "getSimpleInstance doesn't return expected filePath. Expect null" + ". Get: " + rtGetSimpleInstance.simpleInstance.filePath);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstance.simpleInstance.archivePath != epGet.simpleInstance.archivePath)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check archivePath", "getSimpleInstance doesn't return expected archivePath. Expect: " + epGetInfo.simpleInstance.archivePath + ". Get: " + rtGetSimpleInstance.simpleInstance.archivePath);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstance.simpleInstance.originalPath != epGet.simpleInstance.originalPath)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check originalPath", "getSimpleInstance doesn't return expected originalPath." + "Expect: " + epGet.simpleInstance.originalPath + ". Get: " + rtGetSimpleInstance.simpleInstance.originalPath);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstance.simpleInstance.sopInstanceUid != epGet.simpleInstance.sopInstanceUid)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check sopInstanceUid", "getSimpleInstance doesn't return expected sopInstanceUid." + "Expect: " + epGet.simpleInstance.sopInstanceUid + ". Get: " + rtGetSimpleInstance.simpleInstance.sopInstanceUid);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstance.simpleInstance.instanceType != epGet.simpleInstance.instanceType)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check instanceType", "getSimpleInstance doesn't return expected instanceType." + "Expect: " + epGet.simpleInstance.instanceType + ". Get: " + rtGetSimpleInstance.simpleInstance.instanceType);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstance.simpleInstance.comments != epGet.simpleInstance.comments)
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check comments", "getSimpleInstance doesn't return expected comments." + "Expect: " + epGet.simpleInstance.comments + ". Get: " + rtGetSimpleInstance.simpleInstance.comments);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (!Utility.IsTimeEqualNow(rtGetSimpleInstance.simpleInstance.creationDateTime))
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check creationDateTime", "getSimpleInstance doesn't return expected creationDateTime." + "Expect: " + epGet.simpleInstance.creationDateTime + ". Get: " + rtGetSimpleInstance.simpleInstance.creationDateTime);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    if (!Utility.IsTimeEqualNow(rtGetSimpleInstance.simpleInstance.lastUpdateDateTime))
                    {
                        cpGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check lastUpdateDateTime", "getSimpleInstance doesn't return expected lastUpdateDateTime." + "Expect: " + epGet.simpleInstance.lastUpdateDateTime + ". Get: " + rtGetSimpleInstance.simpleInstance.lastUpdateDateTime);
                        cpGetSimpleInstance.Result = TestResult.Fail;
                    }
                    //To do: need add back
                    //if (rtGetSimpleInstanceAfterCreate.simpleInstance.fileCreatedTime != epGetAfterCreate.simpleInstance.fileCreatedTime)
                    //{
                    //    cpGetSimpleInstanceAfterCreate.Outputs.AddParameter("Get Simple Instance", "Check fileCreatedTime", "getSimpleInstance doesn't return expected fileCreatedTime." + "Expect: " + epGetAfterCreate.simpleInstance.fileCreatedTime + ". Get: " + rtGetSimpleInstanceAfterCreate.simpleInstance.fileCreatedTime);
                    //    cpGetSimpleInstanceAfterCreate.Result = TestResult.Fail;
                    //}
                    //if (rtGetSimpleInstanceAfterCreate.simpleInstance.fileModifiedTime != epGetAfterCreate.simpleInstance.fileModifiedTime)
                    //{
                    //    cpGetSimpleInstanceAfterCreate.Outputs.AddParameter("Get Simple Instance", "Check fileModifiedTime", "getSimpleInstance doesn't return expected fileModifiedTime." + "Expect: " + epGetAfterCreate.simpleInstance.fileModifiedTime + ". Get: " + rtGetSimpleInstanceAfterCreate.simpleInstance.fileModifiedTime);
                    //    cpGetSimpleInstanceAfterCreate.Result = TestResult.Fail;
                    //}
                    #endregion

                    #region Step: Call getInfo to a normal simple isntance after create
                    CheckPoint cpGetSimpleInstanceInfo = new CheckPoint("Get Simple Instance Info", "Get Simple Instance Info");
                    r.CheckPoints.Add(cpGetSimpleInstanceInfo);

                    SimpleGetSimpleInstanceInfoResponseType rtGetSimpleInstanceInfo = simpleInstanceSvc.getSimpleInstanceInfo(rtCreate.simpleInstance.uid);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Invalid XML", "GetSimpleInstanceInfo response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpGetSimpleInstanceInfo.Result = TestResult.Pass;
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Return XML", "GetSimpleInstanceInfo returns result. " + simpleInstanceSvc.LastReturnXML);
                    }

                    if (rtGetSimpleInstanceInfo.status.code != 0 || rtGetSimpleInstanceInfo.status.message != "ok")
                    {
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check the GetSimpleInstanceInfo response", "GetSimpleInstanceInfo doesn't return expected result.  Code:" + rtGetSimpleInstanceInfo.status.code.ToString() + "; Message:" + rtGetSimpleInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }

                    // Check the return value   
                    if (rtGetSimpleInstanceInfo.simpleInstance.patientUid != epGetInfo.simpleInstance.patientUid)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check patientUid", "GetSimpleInstanceInfo doesn't return expected patientUid." + "Expect: " + epGetInfo.simpleInstance.patientUid + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.patientUid);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.filePath != null)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check filePath", "GetSimpleInstanceInfo doesn't return expected filePath." + "Expect null " + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.filePath);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.archivePath != epGetInfo.simpleInstance.archivePath)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check archivePath", "GetSimpleInstanceInfo doesn't return expected archivePath." + "Expect: " + epGetInfo.simpleInstance.archivePath + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.archivePath);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.originalPath != epGetInfo.simpleInstance.originalPath)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check originalPath", "GetSimpleInstanceInfo doesn't return expected originalPath." + "Expect: " + epGetInfo.simpleInstance.originalPath + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.originalPath);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.sopInstanceUid != epGetInfo.simpleInstance.sopInstanceUid)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check sopInstanceUid", "GetSimpleInstanceInfo doesn't return expected sopInstanceUid." + "Expect: " + epGetInfo.simpleInstance.sopInstanceUid + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.sopInstanceUid);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.instanceType != epGetInfo.simpleInstance.instanceType)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check instanceType", "GetSimpleInstanceInfo doesn't return expected instanceType." + "Expect: " + epGetInfo.simpleInstance.instanceType + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.instanceType);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.objectType != epGetInfo.simpleInstance.objectType)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check instanceType", "GetSimpleInstanceInfo doesn't return expected objectType." + "Expect: " + epGetInfo.simpleInstance.objectType + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.objectType);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.comments != epGetInfo.simpleInstance.comments)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check comments", "GetSimpleInstanceInfo doesn't return expected comments." + "Expect: " + epGetInfo.simpleInstance.comments + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.comments);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (!Utility.IsTimeEqualNow(rtGetSimpleInstanceInfo.simpleInstance.creationDateTime))
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check creationDateTime", "GetSimpleInstanceInfo doesn't return expected creationDateTime." + "Expect: " + epGetInfo.simpleInstance.creationDateTime + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.creationDateTime);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (!Utility.IsTimeEqualNow(rtGetSimpleInstanceInfo.simpleInstance.lastUpdateDateTime))
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check lastUpdateDateTime", "GetSimpleInstanceInfo doesn't return expected lastUpdateDateTime." + "Expect: " + epGetInfo.simpleInstance.lastUpdateDateTime + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.lastUpdateDateTime);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    if (rtGetSimpleInstanceInfo.simpleInstance.fileName != epGetInfo.simpleInstance.fileName)
                    {
                        cpGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check fileName", "GetSimpleInstanceInfo doesn't return expected fileName." + "Expect: " + epGetInfo.simpleInstance.fileName + ". Get: " + rtGetSimpleInstanceInfo.simpleInstance.fileName);
                        cpGetSimpleInstanceInfo.Result = TestResult.Fail;
                    }
                    //To do: need add back
                    //if (rtGetSimpleInstanceInfoAfterCreate.simpleInstance.fileCreatedTime != epGetAfterCreate.simpleInstance.fileCreatedTime)
                    //{
                    //    cpGetSimpleInstanceInfoAfterCreate.Outputs.AddParameter("Get Simple Instance Info", "Check fileCreatedTime", "GetSimpleInstanceInfo doesn't return expected fileCreatedTime." + "Expect: " + epGetAfterCreate.simpleInstance.fileCreatedTime + ". Get: " + rtGetSimpleInstanceInfoAfterCreate.simpleInstance.fileCreatedTime);
                    //    cpGetSimpleInstanceInfoAfterCreate.Result = TestResult.Fail;
                    //}
                    //if (rtGetSimpleInstanceInfoAfterCreate.simpleInstance.fileModifiedTime != epGetAfterCreate.simpleInstance.fileModifiedTime)
                    //{
                    //    cpGetSimpleInstanceInfoAfterCreate.Outputs.AddParameter("Get Simple Instance Info", "Check fileModifiedTime", "GetSimpleInstanceInfo doesn't return expected fileModifiedTime." + "Expect: " + epGetAfterCreate.simpleInstance.fileModifiedTime + ". Get: " + rtGetSimpleInstanceInfoAfterCreate.simpleInstance.fileModifiedTime);
                    //    cpGetSimpleInstanceInfoAfterCreate.Result = TestResult.Fail;
                    //}
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_WorkFlow_Case1584()  //Import normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string instanceUID = null;
                    ImportService importSvc = new ImportService();
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    #region Input param
                    string patientInternalId = null;
                    string objectFileFullPath = null;

                    SimpleSetSimpleInstanceInfoRequestType pSet = new SimpleSetSimpleInstanceInfoRequestType();
                    pSet.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import") // Import parameter
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalId")
                            {
                                patientInternalId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "set")
                        {
                            switch (ids.InputParameters.GetParameter(i).Key) // setSimpleInstance parameter
                            {
                                case "filePath":
                                    pSet.simpleInstance.filePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    pSet.simpleInstance.originalPath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    pSet.simpleInstance.instanceType = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    pSet.simpleInstance.comments = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    pSet.simpleInstance.sopInstanceUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Expected Values
                    SimpleGetSimpleInstanceResponseType epGetAfterImport = new SimpleGetSimpleInstanceResponseType();
                    epGetAfterImport.simpleInstance = new SimpleInstanceType();

                    SimpleGetSimpleInstanceInfoResponseType epGetInfoAfterImport = new SimpleGetSimpleInstanceInfoResponseType();
                    epGetInfoAfterImport.simpleInstance = new SimpleInstanceType();
                    epGetInfoAfterImport.simpleInstance.filePath = null;

                    SimpleGetSimpleInstanceResponseType epGetAfterSet = new SimpleGetSimpleInstanceResponseType();
                    epGetAfterSet.simpleInstance = new SimpleInstanceType();

                    SimpleGetSimpleInstanceInfoResponseType epGetInfoAfterSet = new SimpleGetSimpleInstanceInfoResponseType();
                    epGetInfoAfterSet.simpleInstance = new SimpleInstanceType();
                    epGetInfoAfterSet.simpleInstance.filePath = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getAfterImport")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetAfterImport.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetAfterImport.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetAfterImport.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetAfterImport.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetAfterImport.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    epGetAfterImport.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(objectFileFullPath);
                                    break;
                                case "fileModifiedTime":
                                    epGetAfterImport.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(objectFileFullPath);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getInfoAfterImport")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetInfoAfterImport.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetInfoAfterImport.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetInfoAfterImport.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "objectType":
                                    epGetInfoAfterImport.simpleInstance.objectType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetInfoAfterImport.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetInfoAfterImport.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    epGetInfoAfterImport.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(objectFileFullPath);
                                    break;
                                case "fileModifiedTime":
                                    epGetInfoAfterImport.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(objectFileFullPath);
                                    break;
                                case "fileName":
                                    epGetInfoAfterImport.simpleInstance.fileName = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getAfterSet")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetAfterSet.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetAfterSet.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetAfterSet.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetAfterSet.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetAfterSet.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetAfterSet.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetAfterSet.simpleInstance.fileCreatedTime = epGetInfoAfterImport.simpleInstance.fileCreatedTime;
                                    }
                                    break;
                                case "fileModifiedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetAfterSet.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetAfterSet.simpleInstance.fileModifiedTime = epGetInfoAfterImport.simpleInstance.fileModifiedTime;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getInfoAfterSet")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetInfoAfterSet.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetInfoAfterSet.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetInfoAfterSet.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "objectType":
                                    epGetInfoAfterSet.simpleInstance.objectType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetInfoAfterSet.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetInfoAfterSet.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileCreatedTime = epGetInfoAfterImport.simpleInstance.fileCreatedTime;
                                    }
                                    break;
                                case "fileModifiedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileModifiedTime = epGetInfoAfterImport.simpleInstance.fileModifiedTime;
                                    }
                                    break;
                                case "fileName":
                                    epGetInfoAfterSet.simpleInstance.fileName = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Step: Call import to import a normal simple instance
                    XMLResult rtImport = importSvc.CallImportAndCheck(r, patientInternalId, objectFileFullPath, null);
                    instanceUID = rtImport.SingleResult;
                    #endregion

                    #region Step: Call get to a normal simple instance after create
                    simpleInstanceSvc.CallGetAndCheck(r, instanceUID, epGetAfterImport);
                    #endregion

                    #region Step: Call getInfo to a normal simple isntance after create
                    simpleInstanceSvc.CallGetInfoAndCheck(r, instanceUID, epGetInfoAfterImport);
                    #endregion

                    #region Step: Call set to change the normal simple instance property
                    pSet.simpleInstance.uid = instanceUID;
                    simpleInstanceSvc.CallSetAndCheck(r, pSet);
                    #endregion

                    #region Step: Call get to check after set
                    simpleInstanceSvc.CallGetAndCheck(r, instanceUID, epGetAfterSet);
                    #endregion

                    #region Step: Call getInfo to check after set
                    simpleInstanceSvc.CallGetInfoAndCheck(r, instanceUID, epGetInfoAfterSet);
                    #endregion

                    #region Step: Call delete and check
                    simpleInstanceSvc.CallDeleteAndCheck(r, instanceUID);
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message + ex.StackTrace);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_WorkFlow_Case1585()  //Import archive
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string instanceUID = null;
                    ImportService importSvc = new ImportService();
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    #region Input param
                    string patientInternalId = null;
                    string objectFileFullPath = null;
                    string archivePath = null;
                    SimpleSetSimpleInstanceInfoRequestType pSet = new SimpleSetSimpleInstanceInfoRequestType();
                    pSet.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")  // Import parameter
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalId")
                            {
                                patientInternalId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivePath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "set")
                        {
                            switch (ids.InputParameters.GetParameter(i).Key) // setSimpleInstance parameter
                            {
                                case "filePath":
                                    pSet.simpleInstance.filePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    pSet.simpleInstance.originalPath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    pSet.simpleInstance.instanceType = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    pSet.simpleInstance.comments = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    pSet.simpleInstance.sopInstanceUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Expected Values
                    SimpleGetSimpleInstanceResponseType epGetAfterImport = new SimpleGetSimpleInstanceResponseType();
                    epGetAfterImport.simpleInstance = new SimpleInstanceType();

                    SimpleGetSimpleInstanceInfoResponseType epGetInfoAfterImport = new SimpleGetSimpleInstanceInfoResponseType();
                    epGetInfoAfterImport.simpleInstance = new SimpleInstanceType();
                    epGetInfoAfterImport.simpleInstance.filePath = null;

                    SimpleGetSimpleInstanceResponseType epGetAfterSet = new SimpleGetSimpleInstanceResponseType();
                    epGetAfterSet.simpleInstance = new SimpleInstanceType();

                    SimpleGetSimpleInstanceInfoResponseType epGetInfoAfterSet = new SimpleGetSimpleInstanceInfoResponseType();
                    epGetInfoAfterSet.simpleInstance = new SimpleInstanceType();
                    epGetInfoAfterSet.simpleInstance.filePath = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getAfterImport")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetAfterImport.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "filePath":
                                    epGetAfterImport.simpleInstance.filePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    epGetAfterImport.simpleInstance.archivePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetAfterImport.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetAfterImport.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetAfterImport.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetAfterImport.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    epGetAfterImport.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(objectFileFullPath);
                                    break;
                                case "fileModifiedTime":
                                    epGetAfterImport.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(objectFileFullPath);
                                    break;
                                case "tags":
                                    epGetAfterImport.simpleInstance.tags = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getInfoAfterImport")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetInfoAfterImport.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetInfoAfterImport.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    epGetInfoAfterImport.simpleInstance.archivePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetInfoAfterImport.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "objectType":
                                    epGetInfoAfterImport.simpleInstance.objectType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetInfoAfterImport.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetInfoAfterImport.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    epGetInfoAfterImport.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(objectFileFullPath); ;
                                    break;
                                case "fileModifiedTime":
                                    epGetInfoAfterImport.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(objectFileFullPath);
                                    break;
                                case "fileName":
                                    epGetInfoAfterImport.simpleInstance.fileName = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "tags":
                                    epGetInfoAfterImport.simpleInstance.tags = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getAfterSet")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetAfterSet.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "filePath":
                                    epGetAfterSet.simpleInstance.filePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    epGetAfterSet.simpleInstance.archivePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetAfterSet.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetAfterSet.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetAfterSet.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetAfterSet.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetAfterSet.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetAfterSet.simpleInstance.fileCreatedTime = epGetInfoAfterImport.simpleInstance.fileCreatedTime;
                                    }
                                    break;
                                case "fileModifiedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetAfterSet.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetAfterSet.simpleInstance.fileModifiedTime = epGetInfoAfterImport.simpleInstance.fileModifiedTime;
                                    }
                                    break;
                                case "tags":
                                    epGetAfterSet.simpleInstance.tags = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getInfoAfterSet")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    epGetInfoAfterSet.simpleInstance.patientUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    epGetInfoAfterSet.simpleInstance.archivePath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "originalPath":
                                    epGetInfoAfterSet.simpleInstance.originalPath = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "instanceType":
                                    epGetInfoAfterSet.simpleInstance.instanceType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "objectType":
                                    epGetInfoAfterSet.simpleInstance.objectType = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    epGetInfoAfterSet.simpleInstance.comments = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "sopInstanceUid":
                                    epGetInfoAfterSet.simpleInstance.sopInstanceUid = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "fileCreatedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileCreatedTime = Utility.GetFileCreationTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileCreatedTime = epGetInfoAfterImport.simpleInstance.fileCreatedTime;
                                    }
                                    break;
                                case "fileModifiedTime":
                                    if (pSet.simpleInstance.filePath != null)
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileModifiedTime = Utility.GetFileModifiedTime(pSet.simpleInstance.filePath);
                                    }
                                    else
                                    {
                                        epGetInfoAfterSet.simpleInstance.fileModifiedTime = epGetInfoAfterImport.simpleInstance.fileModifiedTime;
                                    }
                                    break;
                                case "fileName":
                                    epGetInfoAfterSet.simpleInstance.fileName = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                case "tags":
                                    epGetInfoAfterSet.simpleInstance.tags = ids.ExpectedValues.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Step: Call import to import a normal simple instance
                    XMLResult rtImport = importSvc.CallImportAndCheck(r, patientInternalId, objectFileFullPath, archivePath);
                    instanceUID = rtImport.SingleResult;
                    #endregion

                    #region Step: Call get to a normal simple instance after create
                    simpleInstanceSvc.CallGetAndCheck(r, instanceUID, epGetAfterImport);
                    #endregion

                    #region Step: Call getInfo to a normal simple isntance after create
                    simpleInstanceSvc.CallGetInfoAndCheck(r, instanceUID, epGetInfoAfterImport);
                    #endregion

                    #region Step: Call set to change the simple instance property
                    pSet.simpleInstance.uid = instanceUID;
                    simpleInstanceSvc.CallSetAndCheck(r, pSet);
                    #endregion

                    //System.Threading.Thread.Sleep(3000); //Sleep to make sure the call finished

                    #region Step: Call get to check after set
                    simpleInstanceSvc.CallGetAndCheck(r, instanceUID, epGetAfterSet);
                    #endregion

                    #region Step: Call getInfo to check after set
                    simpleInstanceSvc.CallGetInfoAndCheck(r, instanceUID, epGetInfoAfterSet);
                    #endregion


                    #region Step: Call delete and check
                    //simpleInstanceSvc.CallDeleteAndCheck(r, instanceUID);
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message + ex.StackTrace);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Create_Error_Case1589()
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    #region Input param
                    SimpleCreateSimpleInstanceRequestType pCreate = new SimpleCreateSimpleInstanceRequestType();
                    pCreate.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createSimpleInstance")
                        {
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    pCreate.simpleInstance.patientUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "filePath":
                                    pCreate.simpleInstance.filePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Expected Values
                    string epCode = null;
                    string epMessage = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "createSimpleInstance")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "code")
                            {
                                epCode = ids.ExpectedValues.GetParameter(i).Value;
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                            {
                                epMessage = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step: Call create to create a normal simple instance
                    CheckPoint cpCreate = new CheckPoint("Create", "Call create to create a normal simple instance");
                    r.CheckPoints.Add(cpCreate);

                    SimpleCreateSimpleInstanceResponseType rtCreate = simpleInstanceSvc.createSimpleInstance(pCreate);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create Simple Instance", "XML Validation Result", "createSimpleInstance response is not complied with schema. Error:" + simpleInstanceSvc.LastReturnXMLValidateResult.message);
                        cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML:", simpleInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        if (rtCreate.status.code.ToString() == epCode && rtCreate.status.message.Contains(epMessage))
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML correct:", simpleInstanceSvc.LastReturnXML);
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML wrong:", "Expect code: " + epCode + "; message: " + epMessage + ". Actually get: " + simpleInstanceSvc.LastReturnXML);
                            SaveRound(r);
                            continue;
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
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Create_Error_Case1590()
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    #region Input param
                    SimpleCreateSimpleInstanceRequestType pCreate = new SimpleCreateSimpleInstanceRequestType();
                    pCreate.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createSimpleInstance")
                        {
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "patientUid":
                                    pCreate.simpleInstance.patientUid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "filePath":
                                    pCreate.simpleInstance.filePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "archivePath":
                                    pCreate.simpleInstance.archivePath = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Expected Values
                    string epCode = null;
                    string epMessage = null;

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "createSimpleInstance")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "code")
                            {
                                epCode = ids.ExpectedValues.GetParameter(i).Value;
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                            {
                                epMessage = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step: Call create to create a normal simple instance
                    CheckPoint cpCreate = new CheckPoint("Create", "Call create to create a normal simple instance");
                    r.CheckPoints.Add(cpCreate);

                    SimpleCreateSimpleInstanceResponseType rtCreate = simpleInstanceSvc.createSimpleInstance(pCreate);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create Simple Instance", "XML Validation Result", "createSimpleInstance response is not complied with schema. Error:" + simpleInstanceSvc.LastReturnXMLValidateResult.message);
                        cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML:", simpleInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        if (rtCreate.status.code.ToString() == epCode && rtCreate.status.message.Contains(epMessage))
                        {
                            cpCreate.Result = TestResult.Pass;
                            cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML correct:", simpleInstanceSvc.LastReturnXML);
                        }
                        else
                        {
                            cpCreate.Result = TestResult.Fail;
                            cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML wrong:", "Expect code: " + epCode + "; message: " + epMessage + ". Actually get: " + simpleInstanceSvc.LastReturnXML);
                            SaveRound(r);
                            continue;
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
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Get_Error_Case1591()
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string instanceUID = null;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                instanceUID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }

                    CheckPoint pGetSimpleInstance = new CheckPoint("Get Simple Instance", "Get Simple Instance");
                    r.CheckPoints.Add(pGetSimpleInstance);

                    SimpleGetSimpleInstanceResponseType rtGetSimpleInstance = simpleInstanceSvc.getSimpleInstance(instanceUID);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        pGetSimpleInstance.Result = TestResult.Fail;
                        pGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Invalid XML", "GetSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtGetSimpleInstance.status.code.ToString() != epCode || !rtGetSimpleInstance.status.message.Contains(epMessage))
                    {
                        pGetSimpleInstance.Result = TestResult.Fail;
                        pGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check the getSimpleInstance response", "getSimpleInstance doesn't return expected result.  Code:" + rtGetSimpleInstance.status.code.ToString() + "; Message:" + rtGetSimpleInstance.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pGetSimpleInstance.Result = TestResult.Pass;
                        pGetSimpleInstance.Outputs.AddParameter("Get Simple Instance", "Check the getSimpleInstance response", "getSimpleInstance returns expected result. Code:" + rtGetSimpleInstance.status.code.ToString() + "; Message:" + rtGetSimpleInstance.status.message);
                    }

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_GetInfo_Error_Case1592()
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string instanceUID = null;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getSimpleInstanceInfo")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                instanceUID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }

                    CheckPoint pGetSimpleInstanceInfo = new CheckPoint("Get Simple Instance Info", "Get Simple Instance Info");
                    r.CheckPoints.Add(pGetSimpleInstanceInfo);

                    SimpleGetSimpleInstanceInfoResponseType rtGetSimpleInstanceInfo = simpleInstanceSvc.getSimpleInstanceInfo(instanceUID);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        pGetSimpleInstanceInfo.Result = TestResult.Fail;
                        pGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Invalid XML", "GetSimpleInstanceInfo response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtGetSimpleInstanceInfo.status.code.ToString() != epCode || !rtGetSimpleInstanceInfo.status.message.Contains(epMessage))
                    {
                        pGetSimpleInstanceInfo.Result = TestResult.Fail;
                        pGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check the getSimpleInstanceInfo response", "getSimpleInstanceInfo doesn't return expected result.  Code:" + rtGetSimpleInstanceInfo.status.code.ToString() + "; Message:" + rtGetSimpleInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pGetSimpleInstanceInfo.Result = TestResult.Pass;
                        pGetSimpleInstanceInfo.Outputs.AddParameter("Get Simple Instance Info", "Check the getSimpleInstanceInfo response", "getSimpleInstanceInfo returns expected result. Code:" + rtGetSimpleInstanceInfo.status.code.ToString() + "; Message:" + rtGetSimpleInstanceInfo.status.message);
                    }

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Delete_Error_Case1593()
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string instanceUID = null;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "deleteSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                instanceUID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }

                    CheckPoint pDeleteSimpleInstance = new CheckPoint("Delete Simple Instance Info", "Delete Simple Instance Info");
                    r.CheckPoints.Add(pDeleteSimpleInstance);

                    SimpleDeleteSimpleInstanceResponseType rtDeleteSimpleInstance = simpleInstanceSvc.deleteSimpleInstance(instanceUID);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        pDeleteSimpleInstance.Result = TestResult.Fail;
                        pDeleteSimpleInstance.Outputs.AddParameter("Delete Simple Instance", "Invalid XML", "DeleteSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtDeleteSimpleInstance.status.code.ToString() != epCode || !rtDeleteSimpleInstance.status.message.Contains(epMessage))
                    {
                        pDeleteSimpleInstance.Result = TestResult.Fail;
                        pDeleteSimpleInstance.Outputs.AddParameter("Delete Simple Instance", "Check the deleteSimpleInstance response", "deleteSimpleInstance doesn't return expected result.  Code: " + rtDeleteSimpleInstance.status.code.ToString() + "; Message: " + rtDeleteSimpleInstance.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pDeleteSimpleInstance.Result = TestResult.Pass;
                        pDeleteSimpleInstance.Outputs.AddParameter("Get Simple Instance Info", "Check the deleteSimpleInstance response", "deleteSimpleInstance returns expected result. Code: " + rtDeleteSimpleInstance.status.code.ToString() + "; Message: " + rtDeleteSimpleInstance.status.message);
                    }

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Set_Error_Case1594() // Wrong instance ID
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    SimpleSetSimpleInstanceInfoRequestType pSetSimpleInstance = new SimpleSetSimpleInstanceInfoRequestType();
                    pSetSimpleInstance.simpleInstance = new SimpleInstanceType();
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "setSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                pSetSimpleInstance.simpleInstance.uid = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }

                    CheckPoint cpSetSimpleInstance = new CheckPoint("Set Simple Instance", "Set Simple Instance");
                    r.CheckPoints.Add(cpSetSimpleInstance);

                    SimpleSetSimpleInstanceInfoResponseType rtSetSimpleInstance = simpleInstanceSvc.setSimpleInstanceInfo(pSetSimpleInstance);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpSetSimpleInstance.Result = TestResult.Fail;
                        cpSetSimpleInstance.Outputs.AddParameter("Set Simple Instance", "Invalid XML", "setSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtSetSimpleInstance.status.code.ToString() != epCode || !rtSetSimpleInstance.status.message.Contains(epMessage))
                    {
                        cpSetSimpleInstance.Result = TestResult.Fail;
                        cpSetSimpleInstance.Outputs.AddParameter("Set Simple Instance", "Check the setSimpleInstance response", "setSimpleInstance doesn't return expected result.  Code: " + rtSetSimpleInstance.status.code.ToString() + "; Message: " + rtSetSimpleInstance.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpSetSimpleInstance.Result = TestResult.Pass;
                        cpSetSimpleInstance.Outputs.AddParameter("Set Simple Instance Info", "Check the setSimpleInstance response", "setSimpleInstance returns expected result. Code: " + rtSetSimpleInstance.status.code.ToString() + "; Message: " + rtSetSimpleInstance.status.message);
                    }

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Set_Error_Case1595() // Wrong path parameter
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    ImportService importSvc = new ImportService();
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string patientId = null;
                    string objectFileFullPath = null;

                    SimpleSetSimpleInstanceInfoRequestType pSetSimpleInstanceInfo = new SimpleSetSimpleInstanceInfoRequestType();
                    pSetSimpleInstanceInfo.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientID")
                            {
                                patientId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "filePath")
                            {
                                pSetSimpleInstanceInfo.simpleInstance.filePath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                pSetSimpleInstanceInfo.simpleInstance.archivePath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }


                    #region Step: Import a normal simple instance
                    CheckPoint cpImport = new CheckPoint("Import Simple Instance", "Import to createa a normal simple instance");
                    r.CheckPoints.Add(cpImport);

                    XMLResult rtImport = importSvc.importObject(patientId, null, objectFileFullPath, null, true, "false");

                    if (rtImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns error", rtImport.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpImport.Result = TestResult.Pass;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns ok", rtImport.Message);
                    }
                    #endregion

                    #region Step: Call setSimpleInstanceInfo to change info
                    // To do: debug this if the id is correct
                    pSetSimpleInstanceInfo.simpleInstance.uid = rtImport.SingleResult;

                    CheckPoint cpSetSimpleInstanceInfo = new CheckPoint("Set Simple Instance Info", "Set Simple Instance Info");
                    r.CheckPoints.Add(cpSetSimpleInstanceInfo);

                    SimpleSetSimpleInstanceInfoResponseType rtSetSimpleInstanceInfo = simpleInstanceSvc.setSimpleInstanceInfo(pSetSimpleInstanceInfo);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Invalid XML", "setSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtSetSimpleInstanceInfo.status.code.ToString() != epCode || !rtSetSimpleInstanceInfo.status.message.Contains(epMessage))
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Check the setSimpleInstance response", "setSimpleInstance doesn't return expected result.  Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Pass;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance Info", "Check the setSimpleInstance response", "setSimpleInstance returns expected result. Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Set_Error_Case1596() // Try to change patient ID
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    ImportService importSvc = new ImportService();
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string patientID = null;
                    string objectFileFullPath = null;

                    SimpleSetSimpleInstanceInfoRequestType pSetSimpleInstanceInfo = new SimpleSetSimpleInstanceInfoRequestType();
                    pSetSimpleInstanceInfo.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientID")
                            {
                                patientID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientID")
                            {
                                pSetSimpleInstanceInfo.simpleInstance.patientUid = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }


                    #region Step: Import a normal simple instance
                    CheckPoint cpImport = new CheckPoint("Import Simple Instance", "Import to createa a normal simple instance");
                    r.CheckPoints.Add(cpImport);

                    XMLResult rtImport = importSvc.importObject(patientID, null, objectFileFullPath, null, true, "false");

                    if (rtImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns error", rtImport.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpImport.Result = TestResult.Pass;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns ok", rtImport.Message);
                    }
                    #endregion

                    #region Step: Call setSimpleInstanceInfo to change info
                    // To do: debug this if the id is correct
                    pSetSimpleInstanceInfo.simpleInstance.uid = rtImport.SingleResult;

                    CheckPoint cpSetSimpleInstanceInfo = new CheckPoint("Set Simple Instance Info", "Set Simple Instance Info");
                    r.CheckPoints.Add(cpSetSimpleInstanceInfo);

                    SimpleSetSimpleInstanceInfoResponseType rtSetSimpleInstanceInfo = simpleInstanceSvc.setSimpleInstanceInfo(pSetSimpleInstanceInfo);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Invalid XML", "setSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtSetSimpleInstanceInfo.status.code.ToString() != epCode || !rtSetSimpleInstanceInfo.status.message.Contains(epMessage))
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Check the setSimpleInstance response", "setSimpleInstance doesn't return expected result.  Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Pass;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance Info", "Check the setSimpleInstance response", "setSimpleInstance returns expected result. Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                    }
                    #endregion

                    #region Step: Check the patient Id is not changed
                    SimpleGetSimpleInstanceInfoResponseType rtGetInfo = simpleInstanceSvc.getSimpleInstanceInfo(pSetSimpleInstanceInfo.simpleInstance.uid);

                    CheckPoint cpPatientID = new CheckPoint("Check patient ID", "Check patient ID after set");
                    r.CheckPoints.Add(cpPatientID);
                    if (rtGetInfo.simpleInstance.patientUid == patientID)
                    {
                        cpPatientID.Result = TestResult.Pass;
                    }
                    else
                    {
                        cpPatientID.Result = TestResult.Fail;
                        cpPatientID.Outputs.AddParameter("Check patient ID", "Check patient ID after set", "The patient ID is not correct, expected: " + patientID + "; Actually get: " + rtGetInfo.simpleInstance.patientUid);
                        SaveRound(r);
                        continue;
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Set_Error_Case1597() // Try to set normal to archive
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    ImportService importSvc = new ImportService();
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string patientID = null;
                    string objectFileFullPath = null;

                    SimpleSetSimpleInstanceInfoRequestType pSetSimpleInstanceInfo = new SimpleSetSimpleInstanceInfoRequestType();
                    pSetSimpleInstanceInfo.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientID")
                            {
                                patientID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                pSetSimpleInstanceInfo.simpleInstance.archivePath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }


                    #region Step: Import a normal simple instance
                    CheckPoint cpImport = new CheckPoint("Import Simple Instance", "Import to create a normal simple instance");
                    r.CheckPoints.Add(cpImport);

                    XMLResult rtImport = importSvc.importObject(patientID, null, objectFileFullPath, null, true, "false");

                    if (rtImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns error", rtImport.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpImport.Result = TestResult.Pass;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns ok", rtImport.Message);
                    }
                    #endregion

                    #region Step: Call setSimpleInstanceInfo to change info
                    // To do: debug this if the id is correct
                    pSetSimpleInstanceInfo.simpleInstance.uid = rtImport.SingleResult;

                    CheckPoint cpSetSimpleInstanceInfo = new CheckPoint("Set Simple Instance Info", "Set Simple Instance Info");
                    r.CheckPoints.Add(cpSetSimpleInstanceInfo);

                    SimpleSetSimpleInstanceInfoResponseType rtSetSimpleInstanceInfo = simpleInstanceSvc.setSimpleInstanceInfo(pSetSimpleInstanceInfo);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Invalid XML", "setSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtSetSimpleInstanceInfo.status.code.ToString() != epCode || !rtSetSimpleInstanceInfo.status.message.Contains(epMessage))
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Check the setSimpleInstance response", "setSimpleInstance doesn't return expected result.  Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Pass;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance Info", "Check the setSimpleInstance response", "setSimpleInstance returns expected result. Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                    }
                    #endregion

                    #region Step: Check the instance is still normal
                    SimpleGetSimpleInstanceInfoResponseType rtGetInfo = simpleInstanceSvc.getSimpleInstanceInfo(pSetSimpleInstanceInfo.simpleInstance.uid);

                    CheckPoint cpPatientID = new CheckPoint("Check instance", "Check instance after set");
                    r.CheckPoints.Add(cpPatientID);
                    if (rtGetInfo.simpleInstance.tags != "archived" && string.IsNullOrEmpty(rtGetInfo.simpleInstance.archivePath))
                    {
                        cpPatientID.Result = TestResult.Pass;
                        cpPatientID.Outputs.AddParameter("Check instance", "Check instance after set", "The instance is still normal as expected.");
                    }
                    else
                    {
                        cpPatientID.Result = TestResult.Fail;
                        cpPatientID.Outputs.AddParameter("Check instance", "Check instance after set", "The instance info is not correct. getSimpleInstanceInfo returns: " + simpleInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_SimpleInstance_Set_Error_Case1620() // Try to change original path
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    ImportService importSvc = new ImportService();
                    SimpleInstanceServiceV2 simpleInstanceSvc = new SimpleInstanceServiceV2();

                    string patientId = null;
                    string objectFileFullPath = null;

                    SimpleSetSimpleInstanceInfoRequestType pSetSimpleInstanceInfo = new SimpleSetSimpleInstanceInfoRequestType();
                    pSetSimpleInstanceInfo.simpleInstance = new SimpleInstanceType();

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientID")
                            {
                                patientId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setSimpleInstance")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "originalPath")
                            {
                                pSetSimpleInstanceInfo.simpleInstance.originalPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "code")
                        {
                            epCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "message")
                        {
                            epMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }


                    #region Step: Import a normal simple instance
                    CheckPoint cpImport = new CheckPoint("Import Simple Instance", "Import to createa a normal simple instance");
                    r.CheckPoints.Add(cpImport);

                    XMLResult rtImport = importSvc.importObject(patientId, null, objectFileFullPath, null, true, "false");

                    if (rtImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns error", rtImport.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpImport.Result = TestResult.Pass;
                        cpImport.Outputs.AddParameter("Import", "Call import to create a normal simple instance returns ok", rtImport.Message);
                    }
                    #endregion

                    #region Step: Call setSimpleInstanceInfo to change info
                    // To do: debug this if the id is correct
                    pSetSimpleInstanceInfo.simpleInstance.uid = rtImport.SingleResult;

                    CheckPoint cpSetSimpleInstanceInfo = new CheckPoint("Set Simple Instance Info", "Set Simple Instance Info");
                    r.CheckPoints.Add(cpSetSimpleInstanceInfo);

                    SimpleSetSimpleInstanceInfoResponseType rtSetSimpleInstanceInfo = simpleInstanceSvc.setSimpleInstanceInfo(pSetSimpleInstanceInfo);
                    if (!simpleInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Invalid XML", "setSimpleInstance response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (rtSetSimpleInstanceInfo.status.code.ToString() != epCode || !rtSetSimpleInstanceInfo.status.message.Contains(epMessage))
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Fail;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance", "Check the setSimpleInstance response", "setSimpleInstance doesn't return expected result.  Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        cpSetSimpleInstanceInfo.Result = TestResult.Pass;
                        cpSetSimpleInstanceInfo.Outputs.AddParameter("Set Simple Instance Info", "Check the setSimpleInstance response", "setSimpleInstance returns expected result. Code: " + rtSetSimpleInstanceInfo.status.code.ToString() + "; Message: " + rtSetSimpleInstanceInfo.status.message);
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Result = TestResult.Fail;
                    cp.Outputs.AddParameter("Exception thrown when case runs", "Exception message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

    }
}
