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
        public void Run_Import_Zip_Case512() //Case 512: 1.2.3_ImportZip_Normal
        {
            int runCount = 0;
            string patientUID = string.Empty;
            bool isCreatePatient = false;
            bool isDeletePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                isDeletePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Import image");
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                ImportService ims = new ImportService();
                PresentationStateService pss = new PresentationStateService();

                XMLParameter pa = new XMLParameter("patient");
                XMLParameter ia = new XMLParameter("import");
                XMLParameter ObjFilter = new XMLParameter("filter");
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
                    if (ids.InputParameters.GetParameter(i).Step == "delete")
                    {
                        isDeletePatient = true;
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
                if (rslImport.IsErrorOccured)
                {
                    pImport.Result = TestResult.Fail;
                    pImport.Outputs.AddParameter("import", "Import image fail", rslImport.Message);
                    pImport.Outputs.AddParameter("import", "Import image error code", rslImport.Code.ToString());
                }
                else
                {
                    pImport.Result = TestResult.Pass;
                    pImport.Outputs.AddParameter("import", "Import image OK", rslImport.Message);

                    ObjFilter.AddParameter("patient_internal_id", patientUID);
                    ObjFilter.AddParameter("type", "presentationstate");
                    ObjFilter.AddParameter("current", "true");
                    XMLResult listObjcetRsl = ps.listObjects(ObjFilter);

                    CheckPoint pListObject = new CheckPoint("List Image", "Test list object");
                    r.CheckPoints.Add(pListObject);

                    if (listObjcetRsl.IsErrorOccured)
                    {
                        pListObject.Result = TestResult.Fail;
                        pListObject.Outputs.AddParameter("Erron in List Object", "List Object", "List Patient Presentation State Error");
                    }
                    else
                    {
                        pListObject.Outputs.AddParameter("List Object return content", "List Object", listObjcetRsl.ResultContent);
                        if (listObjcetRsl.ArrayResult.Length == 3)
                        {
                            bool isMatchXML = true;
                            for (int index = 0; index < listObjcetRsl.ArrayResult.Parameters.Count; index++)
                            {
                                XMLResult getPSRsl = pss.getPresentationState(listObjcetRsl.ArrayResult.Parameters[index].ParameterValue);
                                if (getPSRsl.ArrayResult.Parameters.Count != 4)
                                {
                                    isMatchXML = false;
                                    pListObject.Outputs.AddParameter("Get PS return content", "Get PS Description", getPSRsl.ResultContent);
                                    break;
                                }
                            }
                            if (isMatchXML)
                            {
                                pListObject.Result = TestResult.Pass;
                            }
                            else
                            {
                                pListObject.Result = TestResult.Fail;
                                pListObject.Outputs.AddParameter("Get PS description error", "Get PS Description", "Not return all xml and image id");
                            }
                        }
                        else
                        {
                            pListObject.Result = TestResult.Fail;
                            pListObject.Outputs.AddParameter("The PS count not match", "Get All PS Description", "Not three PS be retrieve.");
                        }
                    }
                }
                if (isDeletePatient)
                {
                    XMLResult rslDelete = ps.deletePatient(patientUID);
                    if (!rslDelete.IsErrorOccured)
                    {
                        pImport.Outputs.AddParameter("Delete created patient", "Delete Patient", rslDelete.ResultContent);
                    }
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Import_ArchivedImage_Case721()  // Case 721: 1.3.10_ImportImage_N02_Archived image
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
                XMLParameter epGetImageDescription = new XMLParameter();

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo")
                    {
                        epGetImagInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                    else if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription")
                    {
                        epGetImageDescription.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

                        #region Call getImageDescription to check expected key and value
                        CheckPoint cpGetImageDescription = new CheckPoint("GetImageDescription", "Call GetImageDescription to check the info in return value");
                        r.CheckPoints.Add(cpGetImageDescription);

                        XMLResult rtGetImageDescription = imageSvc.getImageDescription(imageID);
                        if (rtGetImageDescription.IsErrorOccured)
                        {
                            cpGetImageDescription.Result = TestResult.Fail;
                            cpGetImageDescription.Outputs.AddParameter("Get Image Description returns error", "getImageDescription", rtGetImageDescription.ResultContent);
                        }
                        else
                        {
                            cpGetImageDescription.Outputs.AddParameter("Get Image Description return success", "getImageDescription", rtGetImageDescription.Message);

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
                            foreach (XMLParameterNode node in epGetImageDescription.Parameters)
                            {
                                if (!rtGetImageDescription.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                                {
                                    cpGetImageDescription.Result = TestResult.Fail;
                                    cpGetImageDescription.Outputs.AddParameter("Get Image Description returns wrong key-value info for: " + node.ParameterName, "GetImageDescription", rtGetImageDescription.ResultContent);
                                    continue;
                                }
                                else
                                {
                                    cpGetImageDescription.Outputs.AddParameter("Get Image Description returns correct key-value info for: " + node.ParameterName, "GetImageDescription", "ok");
                                }
                            }

                            if (cpGetImageDescription.Result != TestResult.Fail)
                            {
                                cpGetImageDescription.Result = TestResult.Pass;
                                cpGetImageDescription.Outputs.AddParameter("Get Image Description returns all correct info", "GetImageDescription", rtGetImageDescription.ResultContent);
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

        public void Run_Import_TestNotification_Case981() //Case 981: 1.3.10_ImportImage_N01_WithNotification
        {
            int runCount = 0;
            string patientUID = string.Empty;
            bool isCreatePatient = false;
            bool isDeletePatient = false;
            string stopMessageType = "teststop";
            string stopMessageContent = "teststop";
            //LaunchNotification();

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
                isDeletePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Import image");
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                ImportService ims = new ImportService();
                ApplicationService ass = new ApplicationService();
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
                    if (ids.InputParameters.GetParameter(i).Step == "delete")
                    {
                        isDeletePatient = true;
                    }
                }
                if (isCreatePatient)
                {
                    XMLResult result = ps.createPatient(pa);
                    if (!result.IsErrorOccured)
                    {
                        patientUID = result.SingleResult;
                        pCreate.Result = TestResult.Pass;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                    }
                }
                System.Threading.Thread.Sleep(500);

                CheckPoint pImport = new CheckPoint("Import Image", "Test import");
                r.CheckPoints.Add(pImport);

                string filePath = string.Empty;
                string archivePath = string.Empty;
                for (int c = 0; c < ia.Length; c++)
                {
                    if (ia.GetParameterName(c) == "path")
                        filePath = ia.GetParameterValue(c);
                    if (ia.GetParameterName(c) == "archivepath")
                        archivePath = ia.GetParameterValue(c);
                }

                XMLResult rslImport = ims.importObject(patientUID, "", filePath, archivePath, true, "false");
                if (rslImport.IsErrorOccured)
                {
                    pImport.Result = TestResult.Fail;
                    pImport.Outputs.AddParameter("import", "Import image fail", rslImport.Message);
                    pImport.Outputs.AddParameter("import", "Import image error code", rslImport.Code.ToString());
                }
                else
                {
                    pImport.Result = TestResult.Pass;
                    pImport.Outputs.AddParameter("import", "Import image OK", rslImport.Message);
                }

                System.Threading.Thread.Sleep(500);
                ass.sendGenericNotification(stopMessageType, stopMessageContent);
                System.Threading.Thread.Sleep(500);

                int getn = rn.getNotificationNumber();

                CheckPoint pNotification = new CheckPoint("Receive Notification", "import message");
                r.CheckPoints.Add(pNotification);

                if (getn == 3)
                {
                    pNotification.Result = TestResult.Pass;
                    foreach (string mmm in rn.getNotificationContent())
                        pNotification.Outputs.AddParameter("Notification message", "Content:", mmm);
                }
                else
                {
                    pNotification.Result = TestResult.Fail;
                    pNotification.Outputs.AddParameter("Message", "Content:", "miss message on receive");
                }
                if (isDeletePatient)
                {
                    XMLResult rslDelete = ps.deletePatient(patientUID);
                    if (!rslDelete.IsErrorOccured)
                    {
                        pNotification.Outputs.AddParameter("Delete created patient", "Delete Patient", rslDelete.ResultContent);
                    }
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Import_TW5Tiff_Case1048() // Case 1048: 1.3.10_ImportImage_N05_TW5Tiff_FMS
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

                XMLParameter epImport = new XMLParameter();

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "import")
                    {
                        epImport.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
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
                    /************************************************
                    import return sample:
                        - <trophy type="result" version="1.0">
                          <status code="0" message="ok" /> 
                        - <fms>
                          <parameter key="internal_id" value="79d96c5c-1ec5-4408-8709-622c0bd4a951" /> 
                          </fms>
                        - <image>
                          <parameter key="internal_id" value="1547aca3-7d98-4282-bd4c-e4e9547e126b" /> 
                          </image>
                        - <presentationstate>
                          <parameter key="internal_id" value="f0c4ed26-e41f-45dc-9f03-41e982d70b23" /> 
                          </presentationstate>
                        - <image>
                          <parameter key="internal_id" value="14831be6-ca69-417c-957f-c7f3a493cc70" /> 
                          </image>
                        - <presentationstate>
                          <parameter key="internal_id" value="a640fe70-7ad9-45c4-b68c-1f0ba2c503f1" /> 
                          </presentationstate>
                        - <image>
                          <parameter key="internal_id" value="d8b65e88-a9c7-4e7b-baac-05da4982f6be" /> 
                          </image>
                        - <presentationstate>
                          <parameter key="internal_id" value="18834173-1ce8-4d6e-9ce7-daf1745761e0" /> 
                          </presentationstate>
                        - <image>
                          <parameter key="internal_id" value="6608a41f-de96-4d0b-8391-6f16cf9fb812" /> 
                          </image>
                        - <presentationstate>
                          <parameter key="internal_id" value="c1b10673-a4ba-4ee9-824c-0431926b3f27" /> 
                          </presentationstate>
                        - <other>
                          <parameter key="total" value="4" /> 
                          <parameter key="success" value="4" /> 
                          </other>
                          </trophy>
                     * **********************************************/

                    if (rslImport.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;
                        pImport.Outputs.AddParameter("Import image fail", "Import", rslImport.Message);
                        pImport.Outputs.AddParameter("Import image returns error code", "Import", rslImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains image id and ps id
                        string fmsID = null;
                        fmsID = rslImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (fmsID == null || fmsID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("Import image returns wrong FMS internal id", "Import", rslImport.ResultContent);
                        }

                        #region Call getImageInfo to check expected key and value

                        //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
                        foreach (XMLParameterNode node in epImport.Parameters)
                        {
                            if (!rslImport.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                            {
                                pImport.Result = TestResult.Fail;
                                pImport.Outputs.AddParameter("Import image returns wrong key-value info for: " + node.ParameterName, "Import", rslImport.ResultContent);
                                continue;
                            }
                            else
                            {
                                pImport.Outputs.AddParameter("Import image returns correct key-value info for: " + node.ParameterName, "Import", "ok");
                            }
                        }

                        if (pImport.Result != TestResult.Fail)
                        {
                            pImport.Result = TestResult.Pass;
                            pImport.Outputs.AddParameter("Import image Info returns all correct info", "Import", rslImport.ResultContent);
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

        public void Run_Import_3DVolume_Case1162()  // Case 1162: 1.3.10_Import3DVolume_N1
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

                    XMLResult rslImport = importSvc.importObject(patientUID, "", filePath, archivePath, overrideExisted, "false");

                    /**********************************************
                        import return sample:
                        - <trophy type="result" version="1.0">
                          <status code="0" message="ok" /> 
                        - <volume>
                          <parameter key="internal_id" value="ab142266-6785-4762-a52b-6f76227df0f2" /> 
                          </volume>
                        - <analysis3d>
                          <parameter key="internal_id" value="89095004-33b0-4cb7-85d4-df283906f625" /> 
                          </analysis3d>
                          </trophy>
                     * **********************************************/

                    if (rslImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("Import volume fail", "Import", rslImport.Message);
                        cpImport.Outputs.AddParameter("Import volume returns error code", "Import", rslImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains image id and ps id
                        string volumeID = null;
                        volumeID = rslImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (volumeID == null || volumeID == string.Empty)
                        {
                            cpImport.Result = TestResult.Fail;
                            cpImport.Outputs.AddParameter("Import volume returns wrong volume internal id", "Import", rslImport.ResultContent);
                        }

                        string analysis3DID = null;
                        analysis3DID = rslImport.MultiResults[1].GetParameterValueByName("internal_id"); // ps internal_id
                        if (analysis3DID == null || analysis3DID == string.Empty)
                        {
                            cpImport.Result = TestResult.Fail;
                            cpImport.Outputs.AddParameter("import volume returns wrong analysis3D internal id", "Import", rslImport.ResultContent);
                        }
                        else
                        {
                            cpImport.Result = TestResult.Pass;
                            cpImport.Outputs.AddParameter("import volume returns OK", "Import", rslImport.ResultContent);
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
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }
            Output();
        }

        public void Run_Import_8100Slice_Case1203()   // Case 1203: 1.3.10_ImportImage_N07_Import single 8100 dicom thin layer slice
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

                string epGetImageInfoImageType = null;
                string epGetImageInfoAnatomicRegion = null;
                string epGetImageInfoAnatomicRegionModifiers = null;
                string epGetPSInfoImageType = null;
                string epGetPSInfoAnatomicRegion = null;
                string epGetPSInfoAnatomicRegionModifiers = null;

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo")
                    {
                        switch (ids.ExpectedValues.GetParameter(i).Key)
                        {
                            case "image_type":
                                epGetImageInfoImageType = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_anatomic_region":
                                epGetImageInfoAnatomicRegion = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_anatomic_region_modifiers":
                                epGetImageInfoAnatomicRegionModifiers = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationStateInfo")
                    {
                        switch (ids.ExpectedValues.GetParameter(i).Key)
                        {
                            case "image_type":
                                epGetPSInfoImageType = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_anatomic_region":
                                epGetPSInfoAnatomicRegion = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_anatomic_region_modifiers":
                                epGetPSInfoAnatomicRegionModifiers = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            default:
                                break;
                        }
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

                        #region Call getImageInfo to check image_type and anatomic_region
                        ImageService imageSvc = new ImageService();
                        PresentationStateService psSvc = new PresentationStateService();

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

                            // Check the image_type, anatomic_region values in return are correct
                            if (!rtGetImageInfo.ResultContent.Contains("parameter key=\"image_type\" value=\"" + epGetImageInfoImageType + "\""))  //parameter key="image_type" value="PANOV6"
                            {
                                cpGetImageInfo.Result = TestResult.Fail;
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info return wrong image_type info", "getImageInfo", rtGetImageInfo.ResultContent);
                            }
                            else if (!rtGetImageInfo.ResultContent.Contains("parameter key=\"dcm_anatomic_region\" value=\"" + epGetImageInfoAnatomicRegion + "\""))  // parameter key="dcm_anatomic_region" value="Jaw region"
                            {
                                cpGetImageInfo.Result = TestResult.Fail;
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info return wrong dcm_anatomic_region info", "getImageInfo", rtGetImageInfo.ResultContent);
                            }
                            else if (!rtGetImageInfo.ResultContent.Contains("parameter key=\"dcm_anatomic_region_modifiers\" value=\"" + epGetImageInfoAnatomicRegionModifiers + "\""))  // parameter key="dcm_anatomic_region_modifiers" value="Molar 1"
                            {
                                cpGetImageInfo.Result = TestResult.Fail;
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info return wrong dcm_anatomic_region_modifiers info", "getImageInfo", rtGetImageInfo.ResultContent);
                            }
                            else
                            {
                                cpGetImageInfo.Result = TestResult.Pass;
                                cpGetImageInfo.Outputs.AddParameter("Get Image Info return correct info", "getImageInfo", rtGetImageInfo.ResultContent);
                            }
                        }

                        XMLParameterCollection pGetPSInfo = new XMLParameterCollection();
                        XMLParameter psList = new XMLParameter("presentationstate");
                        psList.AddParameter("internal_id", psID);
                        pGetPSInfo.Add(psList);

                        CheckPoint cpGetPSInfo = new CheckPoint("GetPSInfo", "Call GetPSInfo to check the dcm_anatomic_region value");
                        r.CheckPoints.Add(cpGetPSInfo);

                        XMLResult rtGetPSInfo = psSvc.getPresentationStateInfo(pGetPSInfo);
                        if (rtGetPSInfo.IsErrorOccured)
                        {
                            cpGetPSInfo.Result = TestResult.Fail;
                            cpGetPSInfo.Outputs.AddParameter("Get PS Info return error", "GetPSInfo", rtGetPSInfo.ResultContent);
                        }
                        else
                        {
                            cpGetPSInfo.Outputs.AddParameter("Get PS Info return success", "GetPSInfo", rtGetPSInfo.Message);

                            // Check the image_type, anatomic_region values in return are correct
                            if (!rtGetPSInfo.ResultContent.Contains("parameter key=\"image_type\" value=\"" + epGetPSInfoImageType + "\"")) //parameter key="image_type" value="PANOV6"
                            {
                                cpGetPSInfo.Result = TestResult.Fail;
                                cpGetPSInfo.Outputs.AddParameter("Get PS Info return wrong image_type info", "GetPSInfo", rtGetPSInfo.ResultContent);
                            }
                            else if (!rtGetPSInfo.ResultContent.Contains("parameter key=\"dcm_anatomic_region\" value=\"" + epGetPSInfoAnatomicRegion + "\""))  // parameter key="dcm_anatomic_region" value="Jaw region"
                            {
                                cpGetPSInfo.Result = TestResult.Fail;
                                cpGetPSInfo.Outputs.AddParameter("Get PS Info return wrong dcm_anatomic_region info", "GetPSInfo", rtGetPSInfo.ResultContent);
                            }
                            else if (!rtGetPSInfo.ResultContent.Contains("parameter key=\"dcm_anatomic_region_modifiers\" value=\"" + epGetPSInfoAnatomicRegionModifiers + "\""))  // parameter key="dcm_anatomic_region_modifiers" value="Molar 1"
                            {
                                cpGetPSInfo.Result = TestResult.Fail;
                                cpGetPSInfo.Outputs.AddParameter("Get PS Info return wrong dcm_anatomic_region_modifiers info", "GetPSInfo", rtGetPSInfo.ResultContent);
                            }
                            else
                            {
                                cpGetPSInfo.Result = TestResult.Pass;
                                cpGetPSInfo.Outputs.AddParameter("Get PS Info return correct info", "GetPSInfo", rtGetPSInfo.ResultContent);
                            }
                        }

                        CheckPoint cpGetPS = new CheckPoint("GetPS", "Call GetPS to check the ps xml info");
                        r.CheckPoints.Add(cpGetPS);

                        XMLResult rtGetPS = psSvc.getPresentationState(psID);
                        if (rtGetPS.IsErrorOccured)
                        {
                            cpGetPS.Result = TestResult.Fail;
                            cpGetPS.Outputs.AddParameter("Get PS return error", "GetPS", rtGetPS.ResultContent);
                        }
                        else
                        {
                            cpGetPS.Outputs.AddParameter("Get PS return success", "GetPS", rtGetPS.Message);

                            if (rtGetPS.ResultContent.Contains("parameter key=\"general.xml\"") && rtGetPS.ResultContent.Contains("parameter key=\"processing.xml\"") && rtGetPS.ResultContent.Contains("parameter key=\"annotation.xml\"")) // Should return empty ps info for this case as there is not
                            {
                                cpGetPS.Result = TestResult.Pass;
                                cpGetPS.Outputs.AddParameter("Get PS return correct info", "GetPS", rtGetPS.ResultContent);
                            }
                            else
                            {
                                cpGetPS.Result = TestResult.Fail;
                                cpGetPS.Outputs.AddParameter("Get PS return wrong content", "GetPS", rtGetPS.ResultContent);
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

        public void Run_Import_Pano_Case1485() //Case 1485: 1.2.3_ImportImage_Pano
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_Ceph_Case1486() //Case 1486: 1.2.3_ImportImage_Ceph
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_PNG_Case1487() //Case 1487: 1.2.3_ImportImage_PNG
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_TIF_Case1488() //Case 1488: 1.2.3_ImportImage_TIF
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_SC_Case1489() //Case 1489: 1.2.3_ImportImage_SC
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_CRceph_Case1490() //Case 1490: 1.2.3_ImportImage_CRceph
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_CRpano_Case1491() //Case 1491: 1.2.3_ImportImage_CRpano
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_RVG_Case1492() //Case 1492: 1.2.3_ImportImage_RVG
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_STV_Case1493() //Case 1493: 1.2.3_ImportImage_STV
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_VL_Case1494() //Case 1494: 1.2.3_ImportImage_VL
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_DX_Case1495() //Case 1495: 1.2.3_ImportImage_DX
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_FMS_Case1496() // Case 1496: 1.2.3_ImportImage_FMS
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
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);

                PatientService ps = new PatientService();
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

                string ep_nodeNum = string.Empty;
                string ep_totalSubImageNum = string.Empty;
                string ep_successImageNum = string.Empty;
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "import")
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "node")
                        {
                            ep_nodeNum = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "total")
                        {
                            ep_totalSubImageNum = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "success")
                        {
                            ep_successImageNum = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }
                }

                XMLParameter epGetFMSInfo = new XMLParameter();
                //XMLParameter epGetFMSDescription = new XMLParameter();

                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getFMSInfo")
                    {
                        epGetFMSInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                    //else if (ids.ExpectedValues.GetParameter(i).Step == "getFMSDescription")
                    //{
                    //    epGetFMSDescription.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    //}
                }

                try
                {
                    if (isCreatePatient)
                    {
                        XMLResult result = ps.createPatient(pa);
                        if (!result.IsErrorOccured)
                        {
                            patientUID = result.SingleResult;
                            pCreate.Outputs.AddParameter("Create patient UID", "Create Patient", patientUID);
                            pCreate.Result = TestResult.Pass;
                        }
                        else
                        {
                            pCreate.Outputs.AddParameter("Create patient UID", "Create Patient Fail", result.ResultContent);
                            pCreate.Result = TestResult.Fail;
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

                    /*********************** import FMS return sample:
                       <trophy type="result" version="1.0">
                          <status code="0" message="ok" /> 
                        - <fms>
                          <parameter key="internal_id" value="637e9a0d-73dc-447e-bf7d-607b6a088468" /> 
                          </fms>
                        - <image>
                          <parameter key="internal_id" value="c8e90079-1f6f-4d6c-92d2-4cd91e63c3bc" /> 
                          </image>
                        - <presentationstate>
                          <parameter key="internal_id" value="2ff59bd5-d1c3-4a3b-a514-9871abded236" /> 
                          </presentationstate>
                        - <image>
                          <parameter key="internal_id" value="9057d7c5-8e1a-4aee-95ce-a4a9e98dc42e" /> 
                          </image>
                        - <presentationstate>
                          <parameter key="internal_id" value="99179d8d-9855-4bb4-b1c0-c47c7e364d95" /> 
                          </presentationstate>
                          </trophy>
                     * **********************/

                    if (rslImport.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;
                        pImport.Outputs.AddParameter("Import FMS fail", "Import", rslImport.Message);
                        pImport.Outputs.AddParameter("Import FMS returns error code", "Import", rslImport.Code.ToString());
                    }
                    else
                    {
                        if (rslImport.MultiResults.Count.ToString() != ep_nodeNum)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("Import FMS returns wrong node", "Import", rslImport.ResultContent);
                        }
                        else
                        {
                            pImport.Result = TestResult.Pass;

                            for (int i = 0; i < rslImport.MultiResults.Count - 1; i++)
                            {
                                string internalID = rslImport.MultiResults[i].GetParameterValueByName("internal_id");
                                if (internalID == null || internalID == string.Empty)
                                {
                                    pImport.Result = TestResult.Fail;
                                    pImport.Outputs.AddParameter("Import image returns wrong FMS, image or PS info", "Import", rslImport.ResultContent);
                                    break;
                                }
                            }

                            if (pImport.Result == TestResult.Pass)
                            {
                                string totalSubImages = rslImport.MultiResults[rslImport.MultiResults.Count - 1].GetParameterValueByName("total");
                                string successSubImages = rslImport.MultiResults[rslImport.MultiResults.Count - 1].GetParameterValueByName("success");

                                if (totalSubImages != ep_totalSubImageNum || successSubImages != ep_successImageNum)
                                {
                                    pImport.Result = TestResult.Fail;
                                    pImport.Outputs.AddParameter("Import FMS returns wrong info for [other] when import sub images", "Import", rslImport.ResultContent);
                                }
                                else
                                {
                                    pImport.Result = TestResult.Pass;
                                    pImport.Outputs.AddParameter("import FMS returns OK", "Import", rslImport.ResultContent);
                                }
                            }
                        }
                    }

                    #region Call GetFMSInfo to check the return value
                    FMSService fmsSvc = new FMSService();

                    string fmsID = string.Empty; // Try to get from the import return
                    fmsID = rslImport.MultiResults[0].GetParameterValueByName("internal_id");

                    CheckPoint cpGetFMSInfo = new CheckPoint("GetFMSInfo", "Call GetFMSInfo to check the return value");
                    r.CheckPoints.Add(cpGetFMSInfo);

                    XMLResult rtGetFMSInfo = fmsSvc.getFMSInfo(fmsID);
                    if (rtGetFMSInfo.IsErrorOccured)
                    {
                        cpGetFMSInfo.Result = TestResult.Fail;
                        cpGetFMSInfo.Outputs.AddParameter("Get FMS Info returns error", "getFMSInfo", rtGetFMSInfo.ResultContent);
                    }
                    else
                    {
                        cpGetFMSInfo.Outputs.AddParameter("Get FMS Info returns success", "getFMSInfo", rtGetFMSInfo.Message);

                        //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
                        foreach (XMLParameterNode node in epGetFMSInfo.Parameters)
                        {
                            if (!rtGetFMSInfo.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                            {
                                cpGetFMSInfo.Result = TestResult.Fail;
                                cpGetFMSInfo.Outputs.AddParameter("Get FMS Info returns wrong key-value info for: " + node.ParameterName, "GetFMSInfo", rtGetFMSInfo.ResultContent);
                                continue;
                            }
                            else
                            {
                                cpGetFMSInfo.Outputs.AddParameter("Get FMS Info returns correct key-value info for: " + node.ParameterName, "GetFMSInfo", "ok");
                            }
                        }

                        if (cpGetFMSInfo.Result != TestResult.Fail)
                        {
                            cpGetFMSInfo.Result = TestResult.Pass;
                            cpGetFMSInfo.Outputs.AddParameter("Get FMS Info return all correct info", "GetFMSInfo", rtGetFMSInfo.ResultContent);
                        }
                    }
                    #endregion


                    if (isDeletePatient)
                    {
                        CheckPoint cpDelete = new CheckPoint("Delete Patient", "Test delete patient");
                        r.CheckPoints.Add(cpDelete);
                        XMLResult rltDelete = ps.deletePatient(patientUID);
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

        public void Run_Import_SimpleInstance_Case1497() // Case 1497: 1.2.3_ImportImage_SimpleInstance
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

                string instanceID = null;

                PatientService ps = new PatientService();
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

                XMLParameter epGetInstanceInfo = new XMLParameter();
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getInstanceInfo")
                    {
                        epGetInstanceInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                }

                try
                {
                    if (isCreatePatient)
                    {
                        CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                        r.CheckPoints.Add(pCreate);

                        XMLResult result = ps.createPatient(pa);
                        if (!result.IsErrorOccured)
                        {
                            patientUID = result.SingleResult;
                            pCreate.Result = TestResult.Pass;
                            pCreate.Outputs.AddParameter("Create patient UID", "Create Patient", patientUID);
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

                    /*****************************************
                     *  import simple instance return sample:
                     * <trophy type="result" version="1.0">
                     * <status code="0" message="ok" />
                     * <instance><parameter key="internal_id" value="1f678090-2c54-4128-8d13-833645ac7483" /></instance>
                     * </trophy>
                     * ****************************************/

                    if (rslImport.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;
                        pImport.Outputs.AddParameter("Import simple instance fail", "Import", rslImport.Message);
                        pImport.Outputs.AddParameter("Import simple instance returns error code", "Import", rslImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains instance id
                        instanceID = rslImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (instanceID == null || instanceID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("Import simple instance returns wrong instance internal id", "Import", rslImport.ResultContent);
                        }
                        else
                        {
                            pImport.Result = TestResult.Pass;
                            pImport.Outputs.AddParameter("import simple instance returns OK", "Import", rslImport.ResultContent);
                        }
                    }

                    #region Call getInstanceInfo to check expected key and value
                    SimpleInstanceService simpleInstanceSvc = new SimpleInstanceService();

                    CheckPoint cpGetInstanceInfo = new CheckPoint("GetInstanceInfo", "Call GetInstanceInfo to check the return value");
                    r.CheckPoints.Add(cpGetInstanceInfo);

                    XMLResult rtGetInstanceInfo = simpleInstanceSvc.getInstanceInfo(instanceID);
                    if (rtGetInstanceInfo.IsErrorOccured)
                    {
                        cpGetInstanceInfo.Result = TestResult.Fail;
                        cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info return error", "getInstanceInfo", rtGetInstanceInfo.ResultContent);
                    }
                    else
                    {
                        cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info return success", "getInstanceInfo", rtGetInstanceInfo.Message);

                        //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
                        foreach (XMLParameterNode node in epGetInstanceInfo.Parameters)
                        {
                            if (!rtGetInstanceInfo.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                            {
                                cpGetInstanceInfo.Result = TestResult.Fail;
                                cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info returns wrong key-value info for: " + node.ParameterName, "GetInstanceInfo", rtGetInstanceInfo.ResultContent);
                                continue;
                            }
                            else
                            {
                                cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info returns correct key-value info for: " + node.ParameterName, "GetInstanceInfo", "ok");
                            }
                        }

                        if (cpGetInstanceInfo.Result != TestResult.Fail)
                        {
                            cpGetInstanceInfo.Result = TestResult.Pass;
                            cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info return all correct info", "GetInstanceInfo", rtGetInstanceInfo.ResultContent);
                        }
                    }
                    #endregion

                    if (isDeletePatient)
                    {
                        CheckPoint cpDelete = new CheckPoint("Delete Patient", "Test delete patient");
                        r.CheckPoints.Add(cpDelete);
                        XMLResult rltDelete = ps.deletePatient(patientUID);
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

        public void Run_Import_SimpleInstance_LargeFile_Case1658() // Case 1658: 1.2.3_ImportImage_SimpleInstance_large file
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

                string instanceID = null;

                PatientService ps = new PatientService();
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

                XMLParameter epGetInstanceInfo = new XMLParameter();
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "getInstanceInfo")
                    {
                        epGetInstanceInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                    }
                }

                try
                {
                    if (isCreatePatient)
                    {
                        CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                        r.CheckPoints.Add(pCreate);

                        XMLResult result = ps.createPatient(pa);
                        if (!result.IsErrorOccured)
                        {
                            patientUID = result.SingleResult;
                            pCreate.Result = TestResult.Pass;
                            pCreate.Outputs.AddParameter("Create patient UID", "Create Patient", patientUID);
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

                    /*****************************************
                     *  import simple instance return sample:
                     * <trophy type="result" version="1.0">
                     * <status code="0" message="ok" />
                     * <instance><parameter key="internal_id" value="1f678090-2c54-4128-8d13-833645ac7483" /></instance>
                     * </trophy>
                     * ****************************************/

                    if (rslImport.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;
                        pImport.Outputs.AddParameter("Import simple instance fail", "Import", rslImport.Message);
                        pImport.Outputs.AddParameter("Import simple instance returns error code", "Import", rslImport.Code.ToString());
                    }
                    else
                    {
                        // Check the return contiains instance id
                        instanceID = rslImport.MultiResults[0].GetParameterValueByName("internal_id"); // image internal_id
                        if (instanceID == null || instanceID == string.Empty)
                        {
                            pImport.Result = TestResult.Fail;
                            pImport.Outputs.AddParameter("Import simple instance returns wrong instance internal id", "Import", rslImport.ResultContent);
                        }
                        else
                        {
                            pImport.Result = TestResult.Pass;
                            pImport.Outputs.AddParameter("import simple instance returns OK", "Import", rslImport.ResultContent);
                        }
                    }

                    #region Call getInstanceInfo to check expected key and value
                    SimpleInstanceService simpleInstanceSvc = new SimpleInstanceService();

                    CheckPoint cpGetInstanceInfo = new CheckPoint("GetInstanceInfo", "Call GetInstanceInfo to check the return value");
                    r.CheckPoints.Add(cpGetInstanceInfo);

                    XMLResult rtGetInstanceInfo = simpleInstanceSvc.getInstanceInfo(instanceID);
                    if (rtGetInstanceInfo.IsErrorOccured)
                    {
                        cpGetInstanceInfo.Result = TestResult.Fail;
                        cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info return error", "getInstanceInfo", rtGetInstanceInfo.ResultContent);
                    }
                    else
                    {
                        cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info return success", "getInstanceInfo", rtGetInstanceInfo.Message);

                        //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
                        foreach (XMLParameterNode node in epGetInstanceInfo.Parameters)
                        {
                            if (!rtGetInstanceInfo.ResultContent.Contains("parameter key=\"" + node.ParameterName + "\" value=\"" + node.ParameterValue + "\""))
                            {
                                cpGetInstanceInfo.Result = TestResult.Fail;
                                cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info returns wrong key-value info for: " + node.ParameterName, "GetInstanceInfo", rtGetInstanceInfo.ResultContent);
                                continue;
                            }
                            else
                            {
                                cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info returns correct key-value info for: " + node.ParameterName, "GetInstanceInfo", "ok");
                            }
                        }

                        if (cpGetInstanceInfo.Result != TestResult.Fail)
                        {
                            cpGetInstanceInfo.Result = TestResult.Pass;
                            cpGetInstanceInfo.Outputs.AddParameter("Get Instance Info return all correct info", "GetInstanceInfo", rtGetInstanceInfo.ResultContent);
                        }
                    }
                    #endregion

                    System.Threading.Thread.Sleep(10000);

                    if (isDeletePatient)
                    {
                        CheckPoint cpDelete = new CheckPoint("Delete Patient", "Test delete patient");
                        r.CheckPoints.Add(cpDelete);
                        XMLResult rltDelete = ps.deletePatient(patientUID);
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

        public void Run_Import_DXFP_Case1498() //Case 1498: 1.2.3_ImportImage_DXFP
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

                            //Check the return info contain the expected key and value: parameter key="XXX" value="XXX"
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

        public void Run_Import_ArchivedFMS_Case1570() //Case 1570: 1.1.06.02_Import an archived FMS_Normal_contains shortcut of archived image(French FE Tool)
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Import FMS1");
                CheckPoint pImportFMS = new CheckPoint("Import FMS1", "Import FMS using conversiontool");
                r.CheckPoints.Add(pImportFMS);
                string patientdir = ids.InputParameters.GetParameter("patientdir").Value;
                string patient_id = ids.InputParameters.GetParameter("patient_id").Value;
                string dpms_id = ids.InputParameters.GetParameter("dpms_id").Value;

                //call conversiontool to import archivedFMS
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + "\\ConversionTool version 1.0.4.0\\conversiontool.exe";
                p.StartInfo.Arguments = "-P\"" + patientdir + "\"";
                p.Start();
                System.Threading.Thread.Sleep(20000);
                p.Kill();

                string puid = null;
                PatientService ps = new PatientService();
                XMLParameter cInputQuery = new XMLParameter("filter");
                cInputQuery.AddParameter("dpms_id", dpms_id);
                cInputQuery.AddParameter("patient_id", patient_id);
                XMLResult rslQuery = ps.queryPatients(cInputQuery);
                if (rslQuery.IsErrorOccured)
                {
                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "Query Fail:", rslQuery.Message);
                    SaveRound(r);
                    goto CLEANUP;
                }
                
                puid = rslQuery.SingleResult;
                //trigger the post import of FMS
                NewPatientService nps = new NewPatientService();
                PatientListObjectsRequestType request = new PatientListObjectsRequestType();
                request.type = PatientListObjectsType.all;
                request.currentSpecified = true;
                request.current = true;
                request.patientInternalId = puid;

                nps.listObjects(request);

                //wait the post import completed
                System.Threading.Thread.Sleep(10000);
                PatientListObjectsResponseType response = nps.listObjects(request);


                if (!nps.LastReturnXMLValidateResult.isValid)
                {
                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "List Fail:", "Response is not complied with XML Schema");
                    SaveRound(r);
                    goto CLEANUP;
                }

                if (response.status.code != 0)
                {

                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "List Fail:", response.status.message);
                    SaveRound(r);
                    goto CLEANUP;
                }
                if (response.fmss == null)
                {
                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "Import Fail:", "Expected 3 fms, actual none.");
                    SaveRound(r);
                    goto CLEANUP;
                }
                else if (response.fmss.Length != 3)
                {
                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "Import Fail:", "Expected 3 fms, actual:" + response.fmss.Length.ToString());
                    SaveRound(r);
                    goto CLEANUP;
                }

                if (response.presentationStates == null)
                {
                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "Import Fail:", "Expected 21 PS, actual none.");
                    SaveRound(r);
                    goto CLEANUP;
                }
                else if (response.presentationStates.Length != 21)
                {
                    pImportFMS.Result = TestResult.Fail;
                    pImportFMS.Outputs.AddParameter("archiveFMS Import", "Import Fail:", "Expected 21 PS, actual:" + response.presentationStates.Length.ToString());
                    SaveRound(r);
                    goto CLEANUP;
                }

                foreach (PresentationStateType pstate in response.presentationStates)
                {
                    if (pstate.image.tags != "archived" || pstate.image.archivePath == string.Empty)
                    {
                        pImportFMS.Result = TestResult.Fail;
                        pImportFMS.Outputs.AddParameter("archiveFMS Import", "Check Fail:", "archive tag or path is not correct");
                        SaveRound(r);
                        goto CLEANUP;
                    }
                }

                pImportFMS.Result = TestResult.Pass;
                pImportFMS.Outputs.AddParameter("archiveFMS Import", "Check Success:", "OK");
                SaveRound(r);

            CLEANUP:
                if (!string.IsNullOrEmpty(puid))
                {
                    ps.deletePatient(patient_id); //delete the patient
                }
            }
            Output();
        }

        public void Run_Import_ContextWithArchiveImage_Case1572() // Case1572: 1.1.06.11_Import Context_Normal_contains an archived image
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                string patientdir = string.Empty;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                CheckPoint pImportAnalysis = new CheckPoint("Import Context", "Import Context using conversiontool");
                r.CheckPoints.Add(pImportAnalysis);

                PatientService pa = new PatientService();
                XMLParameter query = new XMLParameter("filter");
                //XMLParameter psa = new XMLParameter("presentationstate");

                //XMLParameterCollection psaCollection = new XMLParameterCollection();

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "patient")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patientdir")
                            patientdir = ids.InputParameters.GetParameter(i).Value;
                        else
                            query.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + "\\ConversionTool version 1.0.4.0\\conversiontool.exe";
                p.StartInfo.Arguments = "-P\"" + patientdir + "\"";
                p.Start();
                System.Threading.Thread.Sleep(20000);
                p.Kill();

                XMLResult rslQuery = pa.queryPatients(query);
                string puid = null;

                if (rslQuery.IsErrorOccured)
                {
                    pImportAnalysis.Result = TestResult.Fail;
                    pImportAnalysis.Outputs.AddParameter("archiveFMS Import", "Query Fail:", rslQuery.Message);
                    goto CLEANUP;
                }
                else
                {
                    puid = rslQuery.SingleResult;
                }
                
                NewPatientService nps = new NewPatientService();
                PatientListObjectsRequestType request = new PatientListObjectsRequestType();
                request.type = PatientListObjectsType.all;
                request.currentSpecified = true;
                request.current = true;
                request.patientInternalId = puid;

                nps.listObjects(request);

                System.Threading.Thread.Sleep(10000);
                PatientListObjectsResponseType response = nps.listObjects(request);

                if (!nps.LastReturnXMLValidateResult.isValid)
                {
                    pImportAnalysis.Result = TestResult.Fail;
                    pImportAnalysis.Outputs.AddParameter("archiveContext Import", "List Fail:", "Response is not complied with XML Schema");
                    goto CLEANUP;
                }

                if (response.status.code != 0)
                {
                    pImportAnalysis.Result = TestResult.Fail;
                    pImportAnalysis.Outputs.AddParameter("archiveContext Import", "List Fail:", response.status.message);
                    goto CLEANUP;
                }

                if (ids.ExpectedValues.GetParameter("currentps").Value != null)
                {
                    if (response.presentationStates.Length.ToString() != ids.ExpectedValues.GetParameter("currentps").Value)
                    {
                        pImportAnalysis.Result = TestResult.Fail;
                        pImportAnalysis.Outputs.AddParameter("archiveContext Import", "Import Fail:", "Expected" + ids.ExpectedValues.GetParameter("currentps").Value + "PS, actual:" + response.presentationStates.Length.ToString());
                        goto CLEANUP;
                    }
                }

                request.current = false; // below to check non-current PS and analysis
                response = nps.listObjects(request);
                if (!nps.LastReturnXMLValidateResult.isValid)
                {
                    pImportAnalysis.Result = TestResult.Fail;
                    pImportAnalysis.Outputs.AddParameter("archiveContext Import", "List Fail:", "Response is not complied with XML Schema");
                    goto CLEANUP;
                }

                if (response.status.code != 0)
                {
                    pImportAnalysis.Result = TestResult.Fail;
                    pImportAnalysis.Outputs.AddParameter("archiveContext Import", "List Fail:", response.status.message);
                    goto CLEANUP;
                }

                if (ids.ExpectedValues.GetParameter("noncurrentps").Value != null)
                {
                    if (response.presentationStates == null || response.presentationStates.Length.ToString() != ids.ExpectedValues.GetParameter("noncurrentps").Value)
                    {
                        pImportAnalysis.Result = TestResult.Fail;
                        pImportAnalysis.Outputs.AddParameter("archiveContext Import", "Import Fail:", "Expected" + ids.ExpectedValues.GetParameter("noncurrentps").Value + "NON Current PS, actual response:" + nps.LastReturnXML);
                        goto CLEANUP;
                    }
                }

                if (ids.ExpectedValues.GetParameter("noncurrentanalysis").Value != null)
                {
                    if (response.analysiss == null || response.analysiss.Length.ToString() != ids.ExpectedValues.GetParameter("noncurrentanalysis").Value)
                    {
                        pImportAnalysis.Result = TestResult.Fail;
                        pImportAnalysis.Outputs.AddParameter("archiveContext Import", "Import Fail:", "Expected" + ids.ExpectedValues.GetParameter("noncurrentanalysis").Value + "NON Current Analysis, actual response:" + nps.LastReturnXML);
                        goto CLEANUP;
                    }
                }

                pImportAnalysis.Result = TestResult.Pass;
                pImportAnalysis.Outputs.AddParameter("archiveContext Import", "success import the context", "OK");
            
            CLEANUP:
                if (!string.IsNullOrEmpty(puid))
                {
                    pa.deletePatient(puid);
                }

                SaveRound(r);
            }

            Output();
        }

        public void Run_Import_ImportDir_MainDirectory_Case1598() //Case 1598: 1.1.06.21_ImportDir_MainDirectory_N01
        {

            int runCount = 0;
            NotificationSim.ReceiveNotification rn = new ReceiveNotification();
            System.Collections.Generic.List<string> topics = new System.Collections.Generic.List<string>();
            topics.Add("topic.postImportCompleted");
            string stopTopic = "teststop";
            string stopContent = "teststop";
            rn.startListon(topics, stopTopic, stopContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "ImportMainDir");
                CheckPoint pimp = new CheckPoint("ImportDir", "Import Additional Dir");
                r.CheckPoints.Add(pimp);

                //create patient for import
                PatientService ps = new PatientService();
                XMLParameter cInputPatient = new XMLParameter("patient");
                cInputPatient.AddParameter("first_name", "test");
                cInputPatient.AddParameter("last_name", "importmaindir");
                XMLResult rslCreate = ps.createPatient(cInputPatient);

                if (rslCreate.IsErrorOccured)
                {
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("Import Main Dir", "Create Patient Fail", rslCreate.Message);
                    SaveRound(r);
                    continue;
                }
                string patientid = rslCreate.SingleResult;
                try
                {
                    string kdis6dir = ids.InputParameters.GetParameter("kdis6_dir").Value;


                    int retrynum = int.Parse(ids.InputParameters.GetParameter("retrynum").Value);
                    string flag = "import";
                    ImportService imps = new ImportService();
                    XMLResult rslimport = imps.importDir(patientid, kdis6dir, flag);

                    if (rslimport.Code != 800)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "Import Fail", rslimport.Message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    int n = 0;
                    while (rn.getRecievedNumber() == 0)
                    {
                        n++;
                        if (n < retrynum)
                        {
                            System.Threading.Thread.Sleep(5000);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (n == retrynum)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "Notification Fail", "No notification recieved after retry " + n.ToString() + " times.");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    ApplicationService app = new ApplicationService();
                    app.sendGenericNotification(stopTopic, stopContent);
                    System.Threading.Thread.Sleep(1000);
                    System.Collections.Generic.List<string> mmm = rn.getNotificationContent();
                    System.Collections.Generic.List<string> imageids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("Image", mmm[0]);
                    System.Collections.Generic.List<string> fmsids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("FMS", mmm[0]);
                    System.Collections.Generic.List<string> analysisids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("Analysis", mmm[0]);

                    int expectedimgcount = int.Parse(ids.ExpectedValues.GetParameter("imgnum").Value);
                    int expectedfmscount = int.Parse(ids.ExpectedValues.GetParameter("fmsnum").Value);
                    int expectedanalysiscount = int.Parse(ids.ExpectedValues.GetParameter("analysisnum").Value);

                    //check counts
                    if (imageids.Count != expectedimgcount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "Image Count is not correct", "Actual:" + imageids.Count.ToString() + ",Expected:" + expectedimgcount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (fmsids.Count != expectedfmscount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "FMS Count is not correct", "Actual:" + fmsids.Count.ToString() + ",Expected:" + expectedfmscount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (analysisids.Count != expectedanalysiscount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "Analysis Count is not correct", "Actual:" + analysisids.Count.ToString() + ",Expected:" + expectedanalysiscount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //get image info
                    ImageService imgs = new ImageService();
                    foreach (string imgid in imageids)
                    {
                        XMLParameter cInputImage = new XMLParameter("image");
                        cInputImage.AddParameter("internal_id", imgid);
                        XMLResult rslget = imgs.getImageInfo(cInputImage);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "GetImage Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }
                    //get FMS info
                    FMSService fmss = new FMSService();
                    foreach (string fmsid in fmsids)
                    {
                        XMLResult rslget = fmss.getFMSInfo(fmsid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "GetFMS Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        rslget = fmss.getFMSDescription(fmsid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "GetFMS Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }
                    //get Analysis info
                    AnalysisService anas = new AnalysisService();
                    foreach (string analysisid in analysisids)
                    {
                        XMLResult rslget = anas.getAnalysisInfo(analysisid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "GetAnalysis Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        rslget = anas.getAnalysisDescription(analysisid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "GetAnalysis Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }

                    //check list object
                    NewPatientService nps = new NewPatientService();
                    PatientListObjectsRequestType request = new PatientListObjectsRequestType();
                    request.currentSpecified = true;
                    request.current = true;
                    request.patientInternalId = patientid;
                    request.type = PatientListObjectsType.all;
                    PatientListObjectsResponseType response = nps.listObjects(request);
                    //to refresh code 800 to 0
                    response = nps.listObjects(request);

                    if (!nps.LastReturnXMLValidateResult.isValid)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail", "Response is not complied with schema");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.status.code != 0)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail,message: ", response.status.message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    int expectedcurrentps = int.Parse(ids.ExpectedValues.GetParameter("curpsnum").Value);
                    int expectedcurrentfms = int.Parse(ids.ExpectedValues.GetParameter("curfmsnum").Value);
                    int expectedcurrentanalysis = int.Parse(ids.ExpectedValues.GetParameter("curanalysisnum").Value);
                    if (response.presentationStates.Length != expectedcurrentps)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail,current image count not correct: ", "actural:" + response.presentationStates.Length.ToString() + " expect:" + expectedcurrentps.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }
                    //get PS
                    PresentationStateService pss = new PresentationStateService();
                    foreach (PresentationStateType pst in response.presentationStates)
                    {
                        XMLResult rslget = pss.getPresentationState(pst.uid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "get PS fail: ", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }

                    }

                    if (response.analysiss != null)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail,current analysis count not correct: ", "Expect no analysis will be returned");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.fmss.Length != expectedcurrentfms)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail,current fms count not correct: ", "actural:" + response.fmss.Length.ToString() + " expect:" + expectedcurrentfms.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //list noncurrent objects

                    request.currentSpecified = true;
                    request.current = false;
                    request.patientInternalId = patientid;
                    request.type = PatientListObjectsType.all;

                    response = nps.listObjects(request);

                    int expectednoncurrentps = int.Parse(ids.ExpectedValues.GetParameter("noncurpsnum").Value);
                    int expectednoncurrentfms = int.Parse(ids.ExpectedValues.GetParameter("noncurfmsnum").Value);
                    int expectednoncurrentanalysis = int.Parse(ids.ExpectedValues.GetParameter("noncuranalysisnum").Value);
                    if (response.presentationStates.Length != expectednoncurrentps)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail, non current ps count not correct: ", "actual:" + response.presentationStates.Length.ToString() + " expect:" + expectednoncurrentanalysis.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }
                    //get PS
                    foreach (PresentationStateType pst in response.presentationStates)
                    {
                        XMLResult rslget = pss.getPresentationState(pst.uid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Main Dir", "get PS fail: ", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }

                    }

                    //check 3d object

                    if (response.volumes.Length != int.Parse(ids.ExpectedValues.GetParameter("expect3dnum").Value))
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "Import 3d objects fail: ", "Count is " + response.volumes.Length.ToString() + ",expect:" + ids.ExpectedValues.GetParameter("expect3dnum").Value);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }


                    if (response.analysiss.Length != expectednoncurrentanalysis)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail,non current analysis count not correct: ", "actual:" + response.analysiss.Length.ToString() + " expect:" + expectednoncurrentanalysis.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.fmss.Length != expectednoncurrentfms)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Main Dir", "list object fail,non current fms count not correct: ", "actual:" + response.fmss.Length.ToString() + " expect:" + expectednoncurrentfms.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //Finally the test success
                    pimp.Result = TestResult.Pass;
                    pimp.Outputs.AddParameter("Import Main Dir", "Success", "OK");
                    SaveRound(r);
                    ps.deletePatient(patientid);
                }
                catch (Exception e)
                {
                    ps.deletePatient(patientid);
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("Import Main Dir", "Fail", "Exception caught,message:" + e.Message);
                    SaveRound(r);
                }

            }
        error:
            Output();
        }

        public void Run_Import_ImportDir_MoveCopyOption_Case1599() //Case 1599: 1.1.06.22_ImportDir_MoveCopyOption_N02
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {

                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Move Copy Check for importdir");
                CheckPoint pimp = new CheckPoint("ImportDirMoveCopy", "Move Copy Check for importdir");
                r.CheckPoints.Add(pimp);

                string moveflag = ids.InputParameters.GetParameter("move" + runCount.ToString()).Value;
                string flag = ids.InputParameters.GetParameter("flag" + runCount.ToString()).Value;

                string kdis6_path = ids.InputParameters.GetParameter("kdis6_path" + runCount.ToString()).Value;

                PatientService ps = new PatientService();
                XMLParameter cInputPatient = new XMLParameter("patient");
                cInputPatient.AddParameter("first_name", "test");
                cInputPatient.AddParameter("last_name", "importdir_move");
                XMLResult rslcreate = ps.createPatient(cInputPatient);

                if (rslcreate.IsErrorOccured)
                {
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("create patient", "fail", rslcreate.Message);
                    SaveRound(r);
                    continue;
                }

                string patientid = rslcreate.SingleResult;

                try
                {

                    ImportService imps = new ImportService();

                    Utility.SetCSDMConfig(CSDMConfigSection.remote, "moveWhenPostImport", moveflag);
                    Utility.RestartCSDM(20000);

                    XMLResult rslimp = imps.importDir(patientid, kdis6_path, flag);

                    if (rslimp.Code != 800)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("import dir", "fail", rslimp.Message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    System.Threading.Thread.Sleep(2000);

                    int expectfilenum = int.Parse(ids.ExpectedValues.GetParameter("expectfilenum" + runCount.ToString()).Value);
                    int filenum = Utility.getFileNumOfDirectory(kdis6_path);

                    if (filenum != expectfilenum)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("import dir", "fail", "File Number not correct");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }


                    pimp.Result = TestResult.Pass;
                    pimp.Outputs.AddParameter("import dir_move", "success", "OK");
                    SaveRound(r);
                    ps.deletePatient(patientid);
                }
                catch (Exception e)
                {
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("import dir_move", "exception caught", e.Message);
                    SaveRound(r);
                    ps.deletePatient(patientid);
                    continue;
                }

            }

            Output();

        }

        public void Run_Import_ImportDir_AddionalDirectory_Case1600() //Case 1600: 1.1.06.23_ImportDir_AddionalDirectory_N03
        {

            int runCount = 0;
            NotificationSim.ReceiveNotification rn = new ReceiveNotification();
            System.Collections.Generic.List<string> topics = new System.Collections.Generic.List<string>();
            topics.Add("topic.postImportCompleted");
            string stopTopic = "teststop";
            string stopContent = "teststop";
            rn.startListon(topics, stopTopic, stopContent);

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "ImportMainDir");
                CheckPoint pimp = new CheckPoint("ImportDir", "Import Additional Dir");
                r.CheckPoints.Add(pimp);

                //create patient for import
                PatientService ps = new PatientService();
                XMLParameter cInputPatient = new XMLParameter("patient");
                cInputPatient.AddParameter("first_name", "test");
                cInputPatient.AddParameter("last_name", "importmaindir");
                XMLResult rslCreate = ps.createPatient(cInputPatient);

                if (rslCreate.IsErrorOccured)
                {
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("Import Additional Dir", "Create Patient Fail", rslCreate.Message);
                    SaveRound(r);
                    continue;
                }
                string patientid = rslCreate.SingleResult;
                try
                {
                    string kdis6dir = ids.InputParameters.GetParameter("kdis6_dir").Value;


                    int retrynum = int.Parse(ids.InputParameters.GetParameter("retrynum").Value);
                    string flag = "reference";
                    ImportService imps = new ImportService();
                    XMLResult rslimport = imps.importDir(patientid, kdis6dir, flag);

                    if (rslimport.Code != 800)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "Import Fail", rslimport.Message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    int n = 0;
                    while (rn.getRecievedNumber() == 0)
                    {
                        n++;
                        if (n < retrynum)
                        {
                            System.Threading.Thread.Sleep(5000);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (n == retrynum)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "Notification Fail", "No notification recieved after retry " + n.ToString() + " times.");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    ApplicationService app = new ApplicationService();
                    app.sendGenericNotification(stopTopic, stopContent);
                    System.Threading.Thread.Sleep(1000);
                    System.Collections.Generic.List<string> mmm = rn.getNotificationContent();
                    System.Collections.Generic.List<string> imageids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("Image", mmm[0]);
                    System.Collections.Generic.List<string> fmsids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("FMS", mmm[0]);
                    System.Collections.Generic.List<string> analysisids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("Analysis", mmm[0]);

                    int expectedimgcount = int.Parse(ids.ExpectedValues.GetParameter("imgnum").Value);
                    int expectedfmscount = int.Parse(ids.ExpectedValues.GetParameter("fmsnum").Value);
                    int expectedanalysiscount = int.Parse(ids.ExpectedValues.GetParameter("analysisnum").Value);
                    int expectsimplecount = int.Parse(ids.ExpectedValues.GetParameter("simplenum").Value);

                    //check counts
                    if (imageids.Count != expectedimgcount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "Image Count is not correct", "Actual:" + imageids.Count.ToString() + ",Expected:" + expectedimgcount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (fmsids.Count != expectedfmscount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "FMS Count is not correct", "Actual:" + fmsids.Count.ToString() + ",Expected:" + expectedfmscount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (analysisids.Count != expectedanalysiscount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "Analysis Count is not correct", "Actual:" + analysisids.Count.ToString() + ",Expected:" + expectedanalysiscount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //get image info
                    ImageService imgs = new ImageService();
                    foreach (string imgid in imageids)
                    {
                        XMLParameter cInputImage = new XMLParameter("image");
                        cInputImage.AddParameter("internal_id", imgid);
                        XMLResult rslget = imgs.getImageInfo(cInputImage);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "GetImage Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }
                    //get FMS info
                    FMSService fmss = new FMSService();
                    foreach (string fmsid in fmsids)
                    {
                        XMLResult rslget = fmss.getFMSInfo(fmsid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "GetFMS Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        rslget = fmss.getFMSDescription(fmsid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "GetFMS Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }
                    //get Analysis info
                    AnalysisService anas = new AnalysisService();
                    foreach (string analysisid in analysisids)
                    {
                        XMLResult rslget = anas.getAnalysisInfo(analysisid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "GetAnalysis Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        rslget = anas.getAnalysisDescription(analysisid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "GetAnalysis Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }

                    //check list object
                    NewPatientService nps = new NewPatientService();
                    PatientListObjectsRequestType request = new PatientListObjectsRequestType();
                    request.currentSpecified = true;
                    request.current = true;
                    request.patientInternalId = patientid;
                    request.type = PatientListObjectsType.all;
                    PatientListObjectsResponseType response = nps.listObjects(request);
                    //to refresh code 800 to 0
                    response = nps.listObjects(request);

                    if (!nps.LastReturnXMLValidateResult.isValid)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail", "Response is not complied with schema");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.status.code != 0)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,message: ", response.status.message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    int expectedcurrentps = int.Parse(ids.ExpectedValues.GetParameter("curpsnum").Value);
                    int expectedcurrentfms = int.Parse(ids.ExpectedValues.GetParameter("curfmsnum").Value);
                    int expectedcurrentanalysis = int.Parse(ids.ExpectedValues.GetParameter("curanalysisnum").Value);
                    if (response.presentationStates.Length != expectedcurrentps)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,current image count not correct: ", "actural:" + response.presentationStates.Length.ToString() + " expect:" + expectedcurrentps.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }
                    //get PS
                    PresentationStateService pss = new PresentationStateService();
                    foreach (PresentationStateType pst in response.presentationStates)
                    {
                        XMLResult rslget = pss.getPresentationState(pst.uid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "get PS fail: ", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        //System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex(@kdis6dir + ".*$");

                        if (pst.image.tags != "archived" || pst.image.archivePath == null)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "Archive tag or path not correct: ", "tag:" + pst.image.tags + ", or archivepath is null.");
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }

                    }

                    if (response.analysiss != null)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,current analysis count not correct: ", "Expect no analysis will be returned");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.fmss.Length != expectedcurrentfms)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,current fms count not correct: ", "actural:" + response.fmss.Length.ToString() + " expect:" + expectedcurrentfms.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.simpleInstances.Length == 0)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,simple instance count not correct: ", "actual:" + response.simpleInstances.Length.ToString() + " expect:" + expectsimplecount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    foreach (SimpleInstanceType st in response.simpleInstances)
                    {
                        if (st.tags != "archived" || st.tags == null)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,simple instance archive path or tag not correct: ", "Fail");
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }

                    //list noncurrent objects

                    request.currentSpecified = true;
                    request.current = false;
                    request.patientInternalId = patientid;
                    request.type = PatientListObjectsType.all;

                    response = nps.listObjects(request);

                    int expectednoncurrentps = int.Parse(ids.ExpectedValues.GetParameter("noncurpsnum").Value);
                    int expectednoncurrentfms = int.Parse(ids.ExpectedValues.GetParameter("noncurfmsnum").Value);
                    int expectednoncurrentanalysis = int.Parse(ids.ExpectedValues.GetParameter("noncuranalysisnum").Value);

                    if (response.presentationStates.Length != expectednoncurrentps)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail, non current ps count not correct: ", "actual:" + response.presentationStates.Length.ToString() + " expect:" + expectednoncurrentanalysis.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }
                    //get PS
                    foreach (PresentationStateType pst in response.presentationStates)
                    {
                        XMLResult rslget = pss.getPresentationState(pst.uid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "get PS fail: ", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }

                    }

                    if (response.analysiss.Length != expectednoncurrentanalysis)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,non current analysis count not correct: ", "actual:" + response.analysiss.Length.ToString() + " expect:" + expectednoncurrentanalysis.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.fmss.Length != expectednoncurrentfms)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,non current fms count not correct: ", "actual:" + response.fmss.Length.ToString() + " expect:" + expectednoncurrentfms.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //check simple instance occurrence
                    if (response.simpleInstances.Length == 0)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,simple instance count not correct: ", "actual:" + response.simpleInstances.Length.ToString() + " expect:" + expectsimplecount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    foreach (SimpleInstanceType st in response.simpleInstances)
                    {
                        if (st.tags != "archived" || st.tags == null)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Additional Dir", "list object fail,simple instance archive path or tag not correct: ", "Fail");
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }

                    //Finally the test success
                    pimp.Result = TestResult.Pass;
                    pimp.Outputs.AddParameter("Import Additional Dir", "Success", "OK");
                    SaveRound(r);
                    ps.deletePatient(patientid);
                }
                catch (Exception e)
                {
                    ps.deletePatient(patientid);
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("Import Additional Dir", "Fail", "Exception caught,message:" + e.Message);
                    SaveRound(r);
                }

            }
        error:
            Output();
        }

        public void Run_Import_ImportDir_Exception_Case1605() //Case 1605: 1.1.06.27_ImportDir_Missing images, not integrated FMS and Context_E04
        {

            int runCount = 0;
            System.Collections.Generic.List<string> topics = new System.Collections.Generic.List<string>();
            topics.Add("topic.postImportCompleted");
            string stopTopic = "teststop";
            string stopContent = "teststop";


            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                NotificationSim.ReceiveNotification rn = new ReceiveNotification();
                rn.startListon(topics, stopTopic, stopContent);
                Round r = this.NewRound(runCount.ToString(), "ImportMissingDir");
                CheckPoint pimp = new CheckPoint("ImportMissingDir", "Import Missing Dir");
                r.CheckPoints.Add(pimp);

                //create patient for import
                PatientService ps = new PatientService();
                XMLParameter cInputPatient = new XMLParameter("patient");
                cInputPatient.AddParameter("first_name", "test");
                cInputPatient.AddParameter("last_name", "importmaindir");
                XMLResult rslCreate = ps.createPatient(cInputPatient);

                if (rslCreate.IsErrorOccured)
                {
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("Import Missing Dir", "Create Patient Fail", rslCreate.Message);
                    SaveRound(r);
                    continue;
                }
                string patientid = rslCreate.SingleResult;
                try
                {
                    string kdis6dir = ids.InputParameters.GetParameter("kdis6_dir" + runCount.ToString()).Value;


                    int retrynum = int.Parse(ids.InputParameters.GetParameter("retrynum" + runCount.ToString()).Value);
                    string flag = ids.InputParameters.GetParameter("flag" + runCount.ToString()).Value;
                    ImportService imps = new ImportService();
                    XMLResult rslimport = imps.importDir(patientid, kdis6dir, flag);

                    if (rslimport.Code != 800)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "Import Fail", rslimport.Message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    int n = 0;
                    while (rn.getRecievedNumber() == 0)
                    {
                        n++;
                        if (n < retrynum)
                        {
                            System.Threading.Thread.Sleep(5000);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (n == retrynum)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "Notification Fail", "No notification recieved after retry " + n.ToString() + " times.");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    ApplicationService app = new ApplicationService();
                    app.sendGenericNotification(stopTopic, stopContent);
                    System.Threading.Thread.Sleep(1000);
                    System.Collections.Generic.List<string> mmm = rn.getNotificationContent();
                    System.Collections.Generic.List<string> imageids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("Image", mmm[0]);
                    System.Collections.Generic.List<string> fmsids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("FMS", mmm[0]);
                    System.Collections.Generic.List<string> analysisids = PAS.AutoTest.TestUtility.Utility.parsePostImportResult("Analysis", mmm[0]);

                    int expectedimgcount = int.Parse(ids.ExpectedValues.GetParameter("imgnum" + runCount.ToString()).Value);
                    int expectedfmscount = int.Parse(ids.ExpectedValues.GetParameter("fmsnum" + runCount.ToString()).Value);
                    int expectedanalysiscount = int.Parse(ids.ExpectedValues.GetParameter("analysisnum" + runCount.ToString()).Value);
                    int expectedcurrentps = int.Parse(ids.ExpectedValues.GetParameter("curpsnum" + runCount.ToString()).Value);
                    int expectedcurrentfms = int.Parse(ids.ExpectedValues.GetParameter("curfmsnum" + runCount.ToString()).Value);
                    int expectedcurrentanalysis = int.Parse(ids.ExpectedValues.GetParameter("curanalysisnum" + runCount.ToString()).Value);
                    int expectednoncurrentps = int.Parse(ids.ExpectedValues.GetParameter("noncurpsnum" + runCount.ToString()).Value);
                    int expectednoncurrentfms = int.Parse(ids.ExpectedValues.GetParameter("noncurfmsnum" + runCount.ToString()).Value);
                    int expectednoncurrentanalysis = int.Parse(ids.ExpectedValues.GetParameter("noncuranalysisnum" + runCount.ToString()).Value);



                    //check counts
                    if (imageids.Count != expectedimgcount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "Image Count is not correct", "Actual:" + imageids.Count.ToString() + ",Expected:" + expectedimgcount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (fmsids.Count != expectedfmscount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "FMS Count is not correct", "Actual:" + fmsids.Count.ToString() + ",Expected:" + expectedfmscount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (analysisids.Count != expectedanalysiscount)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "Analysis Count is not correct", "Actual:" + analysisids.Count.ToString() + ",Expected:" + expectedanalysiscount.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //get image info
                    ImageService imgs = new ImageService();
                    foreach (string imgid in imageids)
                    {
                        XMLParameter cInputImage = new XMLParameter("image");
                        cInputImage.AddParameter("internal_id", imgid);
                        XMLResult rslget = imgs.getImageInfo(cInputImage);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "GetImage Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }
                    //get FMS info
                    FMSService fmss = new FMSService();
                    foreach (string fmsid in fmsids)
                    {
                        XMLResult rslget = fmss.getFMSInfo(fmsid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "GetFMS Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        rslget = fmss.getFMSDescription(fmsid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "GetFMS Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }
                    //get Analysis info
                    AnalysisService anas = new AnalysisService();
                    foreach (string analysisid in analysisids)
                    {
                        XMLResult rslget = anas.getAnalysisInfo(analysisid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "GetAnalysis Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        rslget = anas.getAnalysisDescription(analysisid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "GetAnalysis Fail", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                    }

                    //check list object
                    NewPatientService nps = new NewPatientService();
                    PatientListObjectsRequestType request = new PatientListObjectsRequestType();
                    request.currentSpecified = true;
                    request.current = true;
                    request.patientInternalId = patientid;
                    request.type = PatientListObjectsType.all;
                    PatientListObjectsResponseType response = nps.listObjects(request);
                    //to refresh code 800 to 0
                    response = nps.listObjects(request);

                    if (!nps.LastReturnXMLValidateResult.isValid)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail", "Response is not complied with schema");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.status.code != 0)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail,message: ", response.status.message);
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }


                    if (response.presentationStates.Length != expectedcurrentps)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail,current image count not correct: ", "actural:" + response.presentationStates.Length.ToString() + " expect:" + expectedcurrentps.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }
                    //get PS
                    PresentationStateService pss = new PresentationStateService();
                    foreach (PresentationStateType pst in response.presentationStates)
                    {
                        XMLResult rslget = pss.getPresentationState(pst.uid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "get PS fail: ", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }

                    }

                    if (response.analysiss != null)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail,current analysis count not correct: ", "Expect no analysis will be returned");
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.fmss.Length != expectedcurrentfms)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail,current fms count not correct: ", "actural:" + response.fmss.Length.ToString() + " expect:" + expectedcurrentfms.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    //list noncurrent objects

                    request.currentSpecified = true;
                    request.current = false;
                    request.patientInternalId = patientid;
                    request.type = PatientListObjectsType.all;

                    response = nps.listObjects(request);



                    if (response.presentationStates.Length != expectednoncurrentps)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail, non current ps count not correct: ", "actual:" + response.presentationStates.Length.ToString() + " expect:" + expectednoncurrentanalysis.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }
                    //get PS
                    foreach (PresentationStateType pst in response.presentationStates)
                    {
                        XMLResult rslget = pss.getPresentationState(pst.uid);
                        if (rslget.IsErrorOccured)
                        {
                            pimp.Result = TestResult.Fail;
                            pimp.Outputs.AddParameter("Import Missing Dir", "get PS fail: ", rslget.Message);
                            SaveRound(r);
                            ps.deletePatient(patientid);
                            goto error;
                        }
                        if (flag == "reference")
                        {
                            if (pst.image.tags != "archived" || pst.image.archivePath == null)
                            {
                                pimp.Result = TestResult.Fail;
                                pimp.Outputs.AddParameter("Import Additional Dir", "Archive tag or path not correct: ", "tag:" + pst.image.tags + ", or archivepath is null.");
                                SaveRound(r);
                                ps.deletePatient(patientid);
                                goto error;
                            }
                        }

                    }

                    if (response.analysiss.Length != expectednoncurrentanalysis)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail,non current analysis count not correct: ", "actual:" + response.analysiss.Length.ToString() + " expect:" + expectednoncurrentanalysis.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }

                    if (response.fmss.Length != expectednoncurrentfms)
                    {
                        pimp.Result = TestResult.Fail;
                        pimp.Outputs.AddParameter("Import Missing Dir", "list object fail,non current fms count not correct: ", "actual:" + response.fmss.Length.ToString() + " expect:" + expectednoncurrentfms.ToString());
                        SaveRound(r);
                        ps.deletePatient(patientid);
                        continue;
                    }


                    //Finally the test success
                    pimp.Result = TestResult.Pass;
                    pimp.Outputs.AddParameter("Import Missing Dir", "Success", "OK");
                    SaveRound(r);
                    ps.deletePatient(patientid);
                }
                catch (Exception e)
                {
                    ps.deletePatient(patientid);
                    pimp.Result = TestResult.Fail;
                    pimp.Outputs.AddParameter("Import Missing Dir", "Fail", "Exception caught,message:" + e.Message);
                    SaveRound(r);
                }

            }
        error:
            Output();
        }


    }
}