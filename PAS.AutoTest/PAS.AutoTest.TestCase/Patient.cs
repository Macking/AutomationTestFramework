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
        public void Run_Patient_CreatePatient_Normal_Case33() //Case 33: 1.3.1_CreatePatient_Normal
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create patient");

                CheckPoint pCreate = new CheckPoint("Create Patient", "Test description");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                XMLParameter pa = new XMLParameter("patient");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }
                XMLResult result = ps.createPatient(pa);
                if (result.IsErrorOccured)
                {
                    pCreate.Result = TestResult.Fail;
                    pCreate.Outputs.AddParameter("Create error:", "Create Patient", result.Message);
                }
                else
                {
                    pCreate.Outputs.AddParameter("Patient ID:", "Create Patient", result.SingleResult);

                    XMLResult getPatient = ps.getPatient(result.SingleResult);
                    if (getPatient.IsErrorOccured)
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("Get created patient fail", "Get Patient", getPatient.ResultContent);
                    }
                    else
                    {
                        pCreate.Outputs.AddParameter("Get created patient OK", "Get Patient", getPatient.ResultContent);
                        XMLResult delPatient = ps.deletePatient(result.SingleResult);
                        if (delPatient.IsErrorOccured)
                        {
                            pCreate.Result = TestResult.Fail;
                            pCreate.Outputs.AddParameter("Delete created patient fail", "Delete Patient", delPatient.ResultContent);
                        }
                        else
                        {
                            pCreate.Outputs.AddParameter("Delete created patient OK", "Delete Patient", delPatient.ResultContent);
                            pCreate.Result = TestResult.Pass;
                        }
                    }
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Patient_SetPatient_Normal_Case34() //Case 34: 1.3.1_SetPatient_Normal
        {
            int runCount = 0;
            int matchItem = 1;
            string patientUID = string.Empty;
            bool isCreatePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "set patient");
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();

                XMLParameter cpatientData = new XMLParameter("patient");
                XMLParameter spatientData = new XMLParameter("patient");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        cpatientData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "set")
                    {
                        spatientData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        matchItem++;
                    }
                }
                if (isCreatePatient)
                {
                    XMLResult rslCreate = ps.createPatient(cpatientData);
                    if (!rslCreate.IsErrorOccured)
                    {
                        patientUID = rslCreate.SingleResult;
                        pCreate.Result = TestResult.Pass;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                        //System.Diagnostics.Debug.Print("PAS: Create patient UID:{0}",patientUID);
                    }
                    else
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("create", "Create patient ERR", rslCreate.Message);
                        //System.Diagnostics.Debug.Print("PAS: Create patient ERR:{0}",rslCreate.Message);
                    }
                }

                CheckPoint pSet = new CheckPoint("Set Patient", "Test set");
                r.CheckPoints.Add(pSet);

                XMLResult rslSet = ps.setPatient(spatientData, patientUID);
                System.Threading.Thread.Sleep(500);

                XMLResult rslGet = ps.getPatient(patientUID);

                if (rslSet.IsErrorOccured || rslGet.IsErrorOccured)
                {
                    pSet.Result = TestResult.Fail;
                    pSet.Outputs.AddParameter("set", "Set patient ERR", patientUID);
                }

                for (int j = 0; j < rslGet.ArrayResult.Length; j++)
                {
                    for (int k = 0; k < spatientData.Length; k++)
                    {
                        if (rslGet.ArrayResult.GetParameterName(j) == spatientData.GetParameterName(k)
                          && rslGet.ArrayResult.GetParameterValue(j) == spatientData.GetParameterValue(k))
                            matchItem--;
                    }
                }
                //System.Diagnostics.Debug.Print("PAS: Get patient match:{0}",matchItem - 1);
                if (matchItem == 1)
                {
                    pSet.Outputs.AddParameter("set", "Set patient OK", patientUID);
                }
                else
                {
                    pSet.Result = TestResult.Fail;
                    pSet.Outputs.AddParameter("set", "Set patient ERR", rslSet.Message);
                }

                XMLResult delPat = ps.deletePatient(patientUID);
                if (!delPat.IsErrorOccured)
                {
                    pSet.Outputs.AddParameter("Delete created patient OK", "Delete Patient", delPat.Message);
                    pSet.Result = TestResult.Pass;
                }

                SaveRound(r);
            }
            Output();
        }

        public void Run_Patient_DeletePatient_Normal_Case38() //Case 38: 1.3.1_DeletePatient_Normal
        {
            int runCount = 0;
            string patientUID = string.Empty;
            bool isCreatePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "delete patient");
                CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();

                XMLParameter cpatientData = new XMLParameter("patient");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        cpatientData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                }
                if (isCreatePatient)
                {
                    XMLResult rslCreate = ps.createPatient(cpatientData);
                    if (!rslCreate.IsErrorOccured)
                    {
                        patientUID = rslCreate.SingleResult;
                        pCreate.Result = TestResult.Pass;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                        XMLResult rslGet = ps.getPatient(patientUID);
                        if (!rslGet.IsErrorOccured && rslGet.Code == 0)
                        {
                            CheckPoint pDel = new CheckPoint("Delete Patient", "Test delete");
                            r.CheckPoints.Add(pDel);
                            XMLResult rslDel = ps.deletePatient(patientUID);
                            System.Threading.Thread.Sleep(2000);

                            if (!rslDel.IsErrorOccured)
                            {
                                rslGet = ps.getPatient(patientUID);
                                if (rslGet.IsErrorOccured && rslGet.Code == 1020)
                                {
                                    pDel.Outputs.AddParameter("Delete", "Delete patient success", patientUID);
                                    pDel.Outputs.AddParameter("Get", "Get patient ERR", rslGet.Message);
                                    pDel.Result = TestResult.Pass;
                                }
                            }
                            else
                            {
                                pDel.Result = TestResult.Fail;
                            }
                        }

                    }
                    else
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("create", "Create patient ERR", rslCreate.Message);
                    }
                }

                SaveRound(r);
            }
            Output();
        }

        public void Run_Patient_CreatePatient_Exception_Case62() //Case 62: 1.3.1_CreatePatient_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create patient");

                CheckPoint pCreate = new CheckPoint("Create Patient", "Test description");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                XMLParameter pa = new XMLParameter("patient");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }
                XMLResult result = ps.createPatient(pa);
                if (result.IsErrorOccured)
                {
                    pCreate.Result = TestResult.Pass;
                    pCreate.Outputs.AddParameter("That's ok to create patient error:", "Create Patient", result.Message);
                }
                else
                {
                    pCreate.Result = TestResult.Fail;
                    pCreate.Outputs.AddParameter("Expect create patient error, but it is succusss", "Create Patient", result.Message);
                }
                SaveRound(r);
            }


            Output();
        }

        public void Run_Patient_CreatePatient_Normal_Case139() //Case 139: 1.2.1_CreatePatient_Normal
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create patient");

                CheckPoint pCreate = new CheckPoint("Create Patient", "Test description");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                XMLParameter pa = new XMLParameter("patient");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }
                XMLResult result = ps.createPatient(pa);
                if (result.IsErrorOccured)
                {
                    pCreate.Result = TestResult.Fail;
                    pCreate.Outputs.AddParameter("Create error:", "Create Patient", result.Message);
                }
                else
                {

                    pCreate.Outputs.AddParameter("Patient ID:", "Create Patient", result.SingleResult);

                    XMLResult getPatient = ps.getPatient(result.SingleResult);
                    if (getPatient.IsErrorOccured)
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("Get created patient fail", "Get Patient", getPatient.ResultContent);
                    }
                    else
                    {
                        pCreate.Outputs.AddParameter("Get created patient OK", "Get Patient", getPatient.ResultContent);
                        XMLResult delPatient = ps.deletePatient(result.SingleResult);
                        if (delPatient.IsErrorOccured)
                        {
                            pCreate.Result = TestResult.Fail;
                            pCreate.Outputs.AddParameter("Delete created patient fail", "Delete Patient", delPatient.ResultContent);
                        }
                        else
                        {
                            pCreate.Outputs.AddParameter("Delete created patient OK", "Delete Patient", delPatient.ResultContent);
                            pCreate.Result = TestResult.Pass;
                        }
                    }
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_Patient_GetPatient_CumulativeDose_Case976() //Case 976: 1.3.1_GetPatient_N2_getCumulativeDose
        {
            int runCount = 0;
            string patientUID = string.Empty;
            string dose_value = string.Empty;
            bool isCreatePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Cumulative Dose");

                PatientService ps = new PatientService();
                ImageService igs = new ImageService();

                XMLParameter cpatientData = new XMLParameter("patient");
                XMLParameter imageData = new XMLParameter("image");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        cpatientData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "Image")
                    {
                        imageData.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        if (ids.InputParameters.GetParameter(i).Key == "area_dose_product")
                            dose_value = ids.InputParameters.GetParameter(i).Value;
                    }
                }
                if (isCreatePatient)
                {
                    CheckPoint pCreate = new CheckPoint("Create Patient", "Test create");
                    r.CheckPoints.Add(pCreate);

                    XMLResult rslCreate = ps.createPatient(cpatientData);
                    if (!rslCreate.IsErrorOccured)
                    {
                        patientUID = rslCreate.SingleResult;
                        pCreate.Result = TestResult.Pass;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                    }
                    else
                    {
                        pCreate.Result = TestResult.Fail;
                        pCreate.Outputs.AddParameter("create", "Create patient ERR", rslCreate.Message);
                        goto CLEANUP;
                    }
                }

                if (patientUID != null)
                {
                    imageData.AddParameter("patient_internal_id", patientUID);
                    CheckPoint imageSet = new CheckPoint("Create image", "Create image");
                    r.CheckPoints.Add(imageSet);
                    XMLResult rslCreateImage = igs.createImage(imageData);
                    if (rslCreateImage.IsErrorOccured)
                    {
                        imageSet.Outputs.AddParameter("Create", "Create Image Fail", rslCreateImage.Message);
                        imageSet.Result = TestResult.Fail;
                    }
                    else
                    {
                        imageSet.Outputs.AddParameter("Create", "Create Image UID", rslCreateImage.SingleResult);
                        imageSet.Result = TestResult.Pass;

                        CheckPoint pGet = new CheckPoint("Get Patient", "Get Cumulative Dose");
                        r.CheckPoints.Add(pGet);

                        XMLResult rslGet = ps.getPatient(patientUID);
                        if (!rslGet.IsErrorOccured && rslGet.Code == 0)
                        {
                            for (int j = 0; j < rslGet.ArrayResult.Parameters.Count; j++)
                            {
                                if (rslGet.ArrayResult.Parameters[j].ParameterName == "cumulative_dose")
                                {
                                    if (double.Parse(rslGet.ArrayResult.Parameters[j].ParameterValue) == double.Parse(dose_value))
                                    {
                                        pGet.Outputs.AddParameter("Get", "Get patient OK", rslGet.ResultContent);
                                        pGet.Result = TestResult.Pass;
                                    }
                                    else
                                    {
                                        pGet.Outputs.AddParameter("Get", "Get wrong dose_value", "Expect: " + dose_value + "; actually get: " + rslGet.ArrayResult.Parameters[j].ParameterValue);
                                        pGet.Result = TestResult.Fail;
                                        goto CLEANUP;
                                    }
                                }
                            }
                        }
                        else
                        {
                            pGet.Outputs.AddParameter("Get", "Get patient Error", rslGet.ResultContent);
                            pGet.Result = TestResult.Fail;
                        }
                    }
                }

            CLEANUP:
                SaveRound(r);
                if (isCreatePatient && !string.IsNullOrEmpty(patientUID))
                {
                    ps.deletePatient(patientUID);
                }
            }
            Output();
        }

        public void Run_Patient_ListObjects_Case1527() // Case 1527: 1.1.03.03_List Objects - N1 - ALL
        {
            int runCount = 0;

            NewPatientService nps = new NewPatientService();
            PatientListObjectsRequestType ploReq = new PatientListObjectsRequestType();
            //ploReq.type = PatientListObjectsType.all
            PatientListObjectsResponseType ploRes = new PatientListObjectsResponseType();

            //ploReq.filter = ploFilter;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "ListObject");
                CheckPoint pLo = new CheckPoint("List Object", "Test create");
                r.CheckPoints.Add(pLo);

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "filter")
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "patientUid")
                        { ploReq.patientInternalId = ids.InputParameters.GetParameter(i).Value; }

                        if (ids.InputParameters.GetParameter(i).Key == "type")
                        {
                            switch (ids.InputParameters.GetParameter(i).Value)
                            {
                                case "all":
                                    ploReq.type = PatientListObjectsType.all;
                                    break;
                                case "fms":
                                    ploReq.type = PatientListObjectsType.fms;
                                    break;
                                case "presentationstate":
                                    ploReq.type = PatientListObjectsType.presentationState;
                                    break;
                                case "analysis":
                                    ploReq.type = PatientListObjectsType.analysis;
                                    break;
                                case "other":
                                    ploReq.type = PatientListObjectsType.other;
                                    break;
                                case "video":
                                    ploReq.type = PatientListObjectsType.video;
                                    break;
                                case "colorimage":
                                    ploReq.type = PatientListObjectsType.colorImage;
                                    break;
                                case "gallery":
                                    ploReq.type = PatientListObjectsType.gallery;
                                    break;
                                case "volume":
                                    ploReq.type = PatientListObjectsType.volume;
                                    break;
                                case "analysis3d":
                                    ploReq.type = PatientListObjectsType.analysis3D;
                                    break;
                            }
                        }

                        if (ids.InputParameters.GetParameter(i).Key == "current" && ids.InputParameters.GetParameter(i).Value == "true")
                            ploReq.current = true;
                        if (ids.InputParameters.GetParameter(i).Key == "current" && ids.InputParameters.GetParameter(i).Value == "false")
                            ploReq.current = false;
                        ploReq.currentSpecified = true;
                    }
                }
                ploRes = nps.listObjects(ploReq);
                //nps.listObjects(ploReq);
                if (ploRes.status.code == 0)
                {
                    pLo.Result = TestResult.Pass;
                    pLo.Outputs.AddParameter("List Object Return Pass", "List Object", "The web service return is OK!");

                }
                else
                {
                    pLo.Result = TestResult.Fail;
                    pLo.Outputs.AddParameter("List Object Return Fail", "List Object", "The web service return is ERROR!");
                }
                string expectString = string.Empty;
                int matchItem = 0;
                for (int j = 0; j < ids.ExpectedValues.Count; j++)
                {
                    if (ids.ExpectedValues.GetParameter(j).Step == "presentationstate")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.presentationStates != null && ploRes.presentationStates.Length > 0)
                        {
                            foreach (PresentationStateType ps in ploRes.presentationStates)
                            {
                                if (ps.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }

                    }
                    if (ids.ExpectedValues.GetParameter(j).Step == "fms")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.fmss != null && ploRes.fmss.Length > 0)
                        {
                            foreach (FmsType fms in ploRes.fmss)
                            {
                                if (fms.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }
                    }
                    if (ids.ExpectedValues.GetParameter(j).Step == "analysis")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.analysiss != null && ploRes.analysiss.Length > 0)
                        {
                            foreach (AnalysisType als in ploRes.analysiss)
                            {
                                if (als.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }
                    }
                    if (ids.ExpectedValues.GetParameter(j).Step == "analysis3d")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.analysis3Ds != null && ploRes.analysis3Ds.Length > 0)
                        {
                            foreach (Analysis3DType als in ploRes.analysis3Ds)
                            {
                                if (als.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }
                    }
                    if (ids.ExpectedValues.GetParameter(j).Step == "volume")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.volumes != null && ploRes.volumes.Length > 0)
                        {
                            foreach (VolumeType vls in ploRes.volumes)
                            {
                                if (vls.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }
                    }
                    if (ids.ExpectedValues.GetParameter(j).Step == "simpleinstance")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.simpleInstances != null && ploRes.simpleInstances.Length > 0)
                        {
                            foreach (SimpleInstanceType sis in ploRes.simpleInstances)
                            {
                                if (sis.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }
                    }
                    if (ids.ExpectedValues.GetParameter(j).Step == "colorimage")
                    {
                        expectString = ids.ExpectedValues.GetParameter(j).Value;
                        if (ploRes.images != null && ploRes.images.Length > 0)
                        {
                            foreach (ImageType img in ploRes.images)
                            {
                                if (img.uid != expectString)
                                    continue;
                                else
                                {
                                    matchItem++;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (ids.ExpectedValues.Count == matchItem)
                {
                    pLo.Outputs.AddParameter("List Object Return as expect", "List Object", "All return is matched!");
                }
                else
                {
                    pLo.Result = TestResult.Fail;
                    pLo.Outputs.AddParameter("List Object Return Result unexpect", "List Object", "The web service return is NOT MATCH EXPECT!");
                }

                SaveRound(r);

            }
            Output();
        }

        public void Run_Patient_ListObjects_LogiconTagForImage_Case1643() // Case 1643: 1.1.03.12_List Objects - N10 - tag for image with logicon report
        {
            int runCount = 0;

            PatientService oldPatientSvc = new PatientService();
            ImportService ims = new ImportService();
            NewPatientService patientSvc = new NewPatientService();
            PatientListObjectsRequestType pListObjects = new PatientListObjectsRequestType();
            PatientListObjectsResponseType rtListObjects = new PatientListObjectsResponseType();
            PatientListObjectsResponseType rtListObjectsAfterProcess = new PatientListObjectsResponseType();

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Initialize test data
                    XMLParameter pCreatePatient = new XMLParameter("patient");
                    string patientID = string.Empty;
                    string filePath = string.Empty;
                    string archivePath = string.Empty;
                    string imageID = string.Empty;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createPatient")
                        {
                            pCreatePatient.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "filePath")
                            {
                                filePath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivePath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string tagBeforeProcess = null;
                    string tagAfterProcess = null;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "listObjectsBeforeProcess")
                        {
                            tagBeforeProcess = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "listObjectsAfterProcess")
                        {
                            tagAfterProcess = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }
                    #endregion

                    #region Step: Create a patient
                    CheckPoint cpCreate = new CheckPoint("Create Patient", "Create Patient");
                    r.CheckPoints.Add(cpCreate);

                    XMLResult rtCreatePatient = oldPatientSvc.createPatient(pCreatePatient);
                    if (rtCreatePatient.IsErrorOccured)
                    {
                        cpCreate.Result = TestResult.Fail;
                        cpCreate.Outputs.AddParameter("Create returns error:", "Create Patient", rtCreatePatient.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        patientID = rtCreatePatient.SingleResult;
                        pListObjects.patientInternalId = patientID;
                        pListObjects.current = true;
                        pListObjects.currentSpecified = true;
                        cpCreate.Outputs.AddParameter("Patient ID:", "Create Patient", rtCreatePatient.SingleResult);
                        cpCreate.Result = TestResult.Pass;
                    }
                    #endregion

                    #region Step: Prepare a RVG image
                    CheckPoint cpImport = new CheckPoint("Import Image", "Test import");
                    r.CheckPoints.Add(cpImport);

                    XMLResult rtlImport = ims.importObject(patientID, "", filePath, archivePath, false, "false");
                    if (rtlImport.IsErrorOccured)
                    {
                        cpImport.Result = TestResult.Fail;
                        cpImport.Outputs.AddParameter("import", "Import image fail", rtlImport.ResultContent);
                    }
                    else
                    {
                        cpImport.Result = TestResult.Pass;
                        cpImport.Outputs.AddParameter("import", "Import image OK", rtlImport.ResultContent);
                        imageID = rtlImport.SingleResult;
                    }
                    #endregion

                    #region Step: Check the tag before do logicon processing
                    CheckPoint cpListObjects = new CheckPoint("List Object", "List Object before process");
                    r.CheckPoints.Add(cpListObjects);
                    rtListObjects = patientSvc.listObjects(pListObjects);
                    if (patientSvc.LastReturnXMLValidateResult.isValid && rtListObjects.status.code == 0)
                    {
                        cpListObjects.Result = TestResult.Pass;
                        cpListObjects.Outputs.AddParameter("List Object Return Pass", "List Object", "The web service return is OK!: " + patientSvc.LastReturnXML);
                    }
                    else
                    {
                        cpListObjects.Result = TestResult.Fail;
                        cpListObjects.Outputs.AddParameter("List Object Return Fail", "List Object", "The web service return is ERROR! : " + patientSvc.LastReturnXML);
                        goto CLEANUP;
                    }

                    CheckPoint cpLogiconTag = new CheckPoint("Tag", "Check the logicon tag before processing");
                    if (tagBeforeProcess == rtListObjects.presentationStates[0].image.tags)
                    {
                        cpLogiconTag.Outputs.AddParameter("Check Tag", "List Object", "List Object return tag as expect: " + patientSvc.LastReturnXML);
                        cpLogiconTag.Result = TestResult.Pass;
                    }
                    else
                    {
                        cpLogiconTag.Outputs.AddParameter("List Object Return Result unexpect", "List Object", "List Object return unexpected tag!" + patientSvc.LastReturnXML);
                        cpLogiconTag.Result = TestResult.Fail;
                    }
                    #endregion

                    #region Step: Do logicon processing
                    GenericInstanceService genericInstanceSvc = new GenericInstanceService();
                    genericInstanceSvc.DoLogiconProcess(r, imageID);
                    #endregion

                    #region Step: Check the tag after do logicon processing
                    CheckPoint cpListObjectsAfterProcess = new CheckPoint("List Object", "List Object after process");
                    r.CheckPoints.Add(cpListObjectsAfterProcess);
                    rtListObjectsAfterProcess = patientSvc.listObjects(pListObjects);
                    if (patientSvc.LastReturnXMLValidateResult.isValid && rtListObjectsAfterProcess.status.code == 0)
                    {
                        cpListObjectsAfterProcess.Result = TestResult.Pass;
                        cpListObjectsAfterProcess.Outputs.AddParameter("List Object Return Pass", "List Object", "The web service return is OK!");
                    }
                    else
                    {
                        cpListObjectsAfterProcess.Result = TestResult.Fail;
                        cpListObjectsAfterProcess.Outputs.AddParameter("List Object Return Fail", "List Object", "The web service return is ERROR! : " + patientSvc.LastReturnXML);
                        goto CLEANUP;
                    }

                    CheckPoint cpLogiconTagAfterProcess = new CheckPoint("Tag", "Check the logicon tag before processing");
                    if (tagAfterProcess == rtListObjectsAfterProcess.presentationStates[0].image.tags)
                    {
                        cpListObjectsAfterProcess.Outputs.AddParameter("Check Tag", "List Object", "List Object return tag as expect: " + patientSvc.LastReturnXML);
                        cpListObjectsAfterProcess.Result = TestResult.Pass;
                    }
                    else
                    {
                        cpListObjectsAfterProcess.Outputs.AddParameter("List Object Return Result unexpect", "List Object", "List Object return unexpected tag!" + patientSvc.LastReturnXML);
                        cpListObjectsAfterProcess.Result = TestResult.Fail;
                    }
                    #endregion

                    #region Delete patient
                CLEANUP:
                    CheckPoint cpDelete = new CheckPoint("Delete Patient", "Delete Patient");
                    r.CheckPoints.Add(cpDelete);

                    XMLResult rtDeletePatient = oldPatientSvc.deletePatient(patientID);
                    if (rtDeletePatient.IsErrorOccured)
                    {
                        cpDelete.Result = TestResult.Fail;
                        cpDelete.Outputs.AddParameter("Delete returns error:", "Delete Patient", rtDeletePatient.Message);
                    }
                    else
                    {
                        cpDelete.Outputs.AddParameter("Delete return success:", "Delete Patient", rtDeletePatient.Message);
                        cpDelete.Result = TestResult.Pass;
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

        public void Run_Patient_QueryPatient_Case1654() // Case 1654: 1.3.1_QueryPatients_N6_grouped by keyword in global search
        {
            int runCount = 0;
            List<string> needDelete = new List<string>();


            // step 1. create listed patients.         
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                bool bSkip = false; // to skip query InputDataSet
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "queryPatient")
                        bSkip = true;
                }
                if (bSkip)
                    continue;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create patient");

                CheckPoint pCreate = new CheckPoint("Create Patient", "Test description");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                XMLParameter pa = new XMLParameter("patient");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "createPatient")
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }
                XMLResult result = ps.createPatient(pa);
                if (result.IsErrorOccured)
                {
                    pCreate.Result = TestResult.Fail;
                    pCreate.Outputs.AddParameter("Create error:", "Create Patient", result.Message);
                }
                else
                {
                    pCreate.Result = TestResult.Pass;
                    pCreate.Outputs.AddParameter("Patient ID:", "Create Patient", result.SingleResult);
                    needDelete.Add(result.SingleResult);
                }
                SaveRound(r);
            }
            // step 2. query.
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                bool bSkip = false; // to skip create patient InputDataSet
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "createPatient")
                        bSkip = true;
                }
                if (bSkip)
                    continue;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Query patient");

                CheckPoint pQuery = new CheckPoint("Query Patient", "Test description");
                r.CheckPoints.Add(pQuery);
                PatientService ps = new PatientService();
                XMLParameter query = new XMLParameter("filter");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "queryPatient")
                        query.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                }


                XMLResult queryPatient = ps.queryPatients(query); // should send global_search
                if (queryPatient.IsErrorOccured)
                {
                    pQuery.Result = TestResult.Fail;
                    pQuery.Outputs.AddParameter("Query created patient fail", "Query Patient", queryPatient.ResultContent);
                }
                else
                {
                    pQuery.Outputs.AddParameter("Query created patient OK", "Query Patient", queryPatient.ResultContent);
                    // Here should add the checker for XMLResult
                    //
                    int iTotalExpectedResult = 0;
                    foreach (Parameter p in ids.ExpectedValues.Parameters)
                    {
                        int iStep = 0;
                        if (System.Int32.TryParse(p.Step, out iStep))
                        {
                            iTotalExpectedResult = iStep > iTotalExpectedResult ? iStep : iTotalExpectedResult;
                        }
                    }

                    System.Xml.XmlDocument xmlResult = new System.Xml.XmlDocument();
                    xmlResult.LoadXml(queryPatient.ResultContent);
                    int iNodelist = 0;
                    System.Xml.XmlNodeList patientList = xmlResult.SelectNodes("trophy/patient");
                    foreach (System.Xml.XmlNode patient in patientList)
                    {
                        if (iNodelist < iTotalExpectedResult)
                        {
                            // find the pAttr Key, should check whether the key attribute value is in expected result.
                            foreach (Parameter pExpected in ids.ExpectedValues.Parameters)
                            {
                                int iStep = 0;
                                if (System.Int32.TryParse(pExpected.Step, out iStep))
                                {
                                    if (iStep == (iNodelist + 1))
                                    {
                                        bool bChecked = false;
                                        // if iNodelist is less than expected value, should check whether this is the expected value ordered by global_search
                                        System.Xml.XmlNodeList parameterList = patient.SelectNodes("parameter");
                                        foreach (System.Xml.XmlNode parameter in parameterList)
                                        {
                                            if (parameter.Attributes["key"].Value == pExpected.Key)
                                            {
                                                if (parameter.Attributes["value"].Value == pExpected.Value)
                                                {
                                                    string step = "row (" + iStep + ")" + "key (" + pExpected.Key + ") check successful";
                                                    string key = "query patient";
                                                    string value = "value (" + pExpected.Value + ")";
                                                    pQuery.Outputs.AddParameter(step, key, value);
                                                    pQuery.Result = TestResult.Pass;
                                                    bChecked = true;
                                                }
                                                else
                                                {
                                                    string step = "row (" + iStep + ")" + "key (" + pExpected.Key + ") check fail";
                                                    string key = "query patient";
                                                    string value = "value (" + pExpected.Value + ")";
                                                    pQuery.Outputs.AddParameter(step, key, value);
                                                    pQuery.Result = TestResult.Fail;
                                                    bChecked = true;
                                                }
                                                break; // find the expected key value
                                            }
                                        }
                                        if (!bChecked) // the expected value is not find in expected row
                                        {
                                            string step = "row (" + iStep + ")" + "key (" + pExpected.Key + ") check missed";
                                            string key = "query patient";
                                            string value = "value (" + pExpected.Value + ")";
                                            pQuery.Outputs.AddParameter(step, key, value);
                                            pQuery.Result = TestResult.Fail;
                                            bChecked = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // here , iNodelist is greater than expected value, so this record is more than expected.
                            string step = "row (" + (iNodelist + 1) + ") is not expected";
                            string key = "query patient";
                            string value = patient.InnerXml;
                            pQuery.Outputs.AddParameter(step, key, value);
                            pQuery.Result = TestResult.Fail;
                        }
                        iNodelist++;
                    }
                    if (patientList.Count == 0)
                    {
                        string step = "no any query results";
                        string key = "query patient";
                        string value = queryPatient.ResultContent;
                        pQuery.Outputs.AddParameter(step, key, value);
                        pQuery.Result = TestResult.Fail;
                    }
                }
                SaveRound(r);
            }
            // step 3. delete listed patients.
            foreach (string delpa in needDelete)
            {
                PatientService ps = new PatientService();
                XMLParameter query = new XMLParameter("patient");
                XMLResult delPatient = ps.deletePatient(delpa);
                if (delPatient.IsErrorOccured)
                {
                    Round r = this.NewRound(runCount.ToString(), "Delete patient");
                    CheckPoint pDelete = new CheckPoint("Delete Patient", "Test description");
                    r.CheckPoints.Add(pDelete);
                    pDelete.Result = TestResult.Fail;
                    pDelete.Outputs.AddParameter("Delete created patient fail", "Delete Patient", delPatient.ResultContent);
                    SaveRound(r);
                }
            }

            Output();
        }
    }
}