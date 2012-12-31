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
        public void Run_Image_CreateImage_Normal_Case44() //Case 44: 1.3.6_CreateImage_Normal
        {
            int runCount = 0;
            string patientUID = null;
            bool isCreatePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create image");

                PatientService ps = new PatientService();
                ImageService ims = new ImageService();
                XMLParameter pa = new XMLParameter("patient");
                XMLParameter ia = new XMLParameter("image");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "image")
                    {
                        ia.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                }
                if (isCreatePatient)
                {
                    CheckPoint pCreate = new CheckPoint("Create patient", "Test create");
                    r.CheckPoints.Add(pCreate);

                    XMLResult result = ps.createPatient(pa);
                    if (!result.IsErrorOccured)
                    {
                        patientUID = result.SingleResult;
                        pCreate.Outputs.AddParameter("create", "Create patient UID", patientUID);
                        pCreate.Result = TestResult.Pass;
                    }
                    else
                    {
                        pCreate.Outputs.AddParameter("create", "Create patient returns error: ", result.ResultContent);
                        pCreate.Result = TestResult.Fail;
                        goto CLEANUP;
                    }
                }

                CheckPoint pImage = new CheckPoint("Image create", "Test image");
                r.CheckPoints.Add(pImage);
                if (patientUID != null)
                {
                    ia.AddParameter("patient_internal_id", patientUID);
                    XMLResult imageRsl = ims.createImage(ia);
                    if (imageRsl.IsErrorOccured)
                    {
                        pImage.Result = TestResult.Fail;
                        pImage.Outputs.AddParameter("create", "Create image error", imageRsl.Message);
                    }
                    else
                    {
                        pImage.Result = TestResult.Pass;
                        pImage.Outputs.AddParameter("create", "Create image Id", imageRsl.SingleResult);
                    }
                }

            CLEANUP:
                if (isCreatePatient && !string.IsNullOrEmpty(patientUID))
                {
                    ps.deletePatient(patientUID);
                }
                SaveRound(r);
            }

            Output();
        }

        public void Run_Image_GetImageDescription_Normal_Case45() //Case 45: 1.3.6_GetImageDescription_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string imageID = null;

                    // Input parameter
                    XMLParameter pGetImageDescription = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getImageDescription")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                imageID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    // Output value
                    XMLParameter epGetImageDescription_image = new XMLParameter("image");
                    XMLParameter epGetImageDescription_series = new XMLParameter("series");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription_image")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path")
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                epGetImageDescription_image.AddParameter("path", path);
                            }
                            else
                            {
                                epGetImageDescription_image.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription_series")
                        {
                            epGetImageDescription_series.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    #region Step 1: Call ImageService.getImageDescription to get the image description
                    CheckPoint cpGetImageDescription = new CheckPoint("getImageDescription", "Call ImageService.getImageDescription to get the image description");
                    r.CheckPoints.Add(cpGetImageDescription);

                    ImageService imageService = new ImageService();
                    XMLResult rtGetImageDescription = imageService.getImageDescription(imageID);

                    if (rtGetImageDescription.IsErrorOccured)
                    {
                        cpGetImageDescription.Result = TestResult.Fail;
                        cpGetImageDescription.Outputs.AddParameter("getImageDescription", "Call ImageService.getImageDescription to get the image description", rtGetImageDescription.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cpGetImageDescription.Result = TestResult.Pass;
                        cpGetImageDescription.Outputs.AddParameter("getImageDescription", "Call ImageService.getImageDescription to get the image description", rtGetImageDescription.Message);
                    }
                    #endregion


                    #region Step 2: Check the values in ImageService.getImageDescription return are correct

                    bool isValueEqual = true;
                    bool isKeyShow = true;

                    #region Check image info
                    CheckPoint cpImageInfo = new CheckPoint("Image Info", "Check the values in ImageService.getImageDescription return");
                    r.CheckPoints.Add(cpImageInfo);
                    foreach (XMLParameterNode psNode in epGetImageDescription_image.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < rtGetImageDescription.MultiResults[0].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == rtGetImageDescription.MultiResults[0].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, rtGetImageDescription.MultiResults[0].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            cpImageInfo.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                cpImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageDescription return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rtGetImageDescription.MultiResults[0].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                cpImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageDescription return", "The return value in getImageDescription does not contain the node: " + psNode.ParameterName);
                            }

                            break; // Break current foreach loop, not compare the follwing nodes
                        }
                    }

                    if (isValueEqual) // Need this to judge it comes from break or normal flow which has complete all node comparation
                    {
                        cpImageInfo.Result = TestResult.Pass;
                        cpImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageDescription return", "The return values in getImageDescription all match the expected");
                    }
                    #endregion

                    #region Check series info
                    CheckPoint cpSeriesInfo = new CheckPoint("Series Info", "Check the values in ImageService.getImageDescription return");
                    r.CheckPoints.Add(cpSeriesInfo);
                    foreach (XMLParameterNode psNode in epGetImageDescription_series.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < rtGetImageDescription.MultiResults[1].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == rtGetImageDescription.MultiResults[1].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, rtGetImageDescription.MultiResults[1].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            cpSeriesInfo.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                cpSeriesInfo.Outputs.AddParameter("Series Info", "Check the values in ImageService.getImageDescription return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rtGetImageDescription.MultiResults[1].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                cpSeriesInfo.Outputs.AddParameter("Series Info", "Check the values in ImageService.getImageDescription return", "The return value in getImageDescription does not contain the node: " + psNode.ParameterName);
                            }
                            break; // Break current foreach loop, not compare the follwing nodes
                        }

                    }

                    if (isValueEqual)
                    {
                        cpSeriesInfo.Result = TestResult.Pass;
                        cpSeriesInfo.Outputs.AddParameter("Series Info", "Check the values in ImageService.getImageDescription return", "The return values in getImageDescription all match the expected");
                    }
                    #endregion

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

        public void Run_Image_DeleteImage_Normal_Case55() //Case 55: 1.3.6_DeleteImage_Normal
        {
            int runCount = 0;
            string patientUID = string.Empty;
            string imageUID = string.Empty;
            bool isCreatePatient = false;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                isCreatePatient = false;
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Create image");
                CheckPoint pCreate = new CheckPoint("Create patient", "Test create");
                r.CheckPoints.Add(pCreate);
                PatientService ps = new PatientService();
                ImageService ims = new ImageService();
                XMLParameter pa = new XMLParameter("patient");
                XMLParameter ia = new XMLParameter("image");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "create")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "image")
                    {
                        ia.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
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
                    else
                    {
                        pCreate.Result = TestResult.Pass;
                        pCreate.Outputs.AddParameter("create", "Create patient return error: ", result.ResultContent);
                    }
                }

                CheckPoint cpCreateImage = new CheckPoint("Image create", "Test image");
                r.CheckPoints.Add(cpCreateImage);
                if (patientUID != null)
                {
                    ia.AddParameter("patient_internal_id", patientUID);
                    XMLResult imageRsl = ims.createImage(ia);
                    if (imageRsl.IsErrorOccured)
                    {
                        cpCreateImage.Result = TestResult.Fail;
                        cpCreateImage.Outputs.AddParameter("create", "Create image error", imageRsl.Message);
                    }
                    else
                    {
                        cpCreateImage.Result = TestResult.Pass;
                        cpCreateImage.Outputs.AddParameter("create", "Create image Id", imageUID);
                        imageUID = imageRsl.SingleResult;
                    }
                }
                if (imageUID != null)
                {
                    CheckPoint cpDeleteImage = new CheckPoint("Image delete", "Test image");
                    r.CheckPoints.Add(cpDeleteImage);

                    XMLParameter deletePreference = new XMLParameter("preferences");
                    deletePreference.AddParameter("completedFlag", "true");
                    XMLResult delImageRsl = ims.deleteImage(imageUID, deletePreference);
                    if (delImageRsl.IsErrorOccured)
                    {
                        cpDeleteImage.Result = TestResult.Fail;
                        cpDeleteImage.Outputs.AddParameter("delete", "delete image error", delImageRsl.Message);
                    }
                    else
                    {
                        cpDeleteImage.Result = TestResult.Pass;
                        cpDeleteImage.Outputs.AddParameter("delete", "delete image success", delImageRsl.Message);
                    }
                }

                if (!string.IsNullOrEmpty(patientUID))
                {
                    ps.deletePatient(patientUID);
                }

                SaveRound(r);
            }

            Output();
        }

        public void Run_Image_GetInfo_1ID_Case57() //Case 57: 1.3.6_GetImageInfo_N01_1 valid ID
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    // Input parameter
                    XMLParameter getImageInfoParam = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getImageInfo")
                        {
                            getImageInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    // Output value
                    XMLParameter getImageInfoReturnValue = new XMLParameter("image");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturnValue.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturnValue.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                    }

                    #region Step 1: Call ImageService.getImageInfo to get the image info
                    CheckPoint pGetImageInfo = new CheckPoint("getImageInfo", "Call ImageService.getImageInfo to get the image info");
                    r.CheckPoints.Add(pGetImageInfo);

                    ImageService imageService = new ImageService();
                    XMLResult getImageInfoResult = imageService.getImageInfo(getImageInfoParam);

                    if (getImageInfoResult.IsErrorOccured)
                    {
                        pGetImageInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info returns error.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        pGetImageInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info succeeds.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);
                    }
                    #endregion

                    #region Step 2: Check the values in ImageService.getImageInfo return are correct
                    CheckPoint pImageInfo = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return");
                    r.CheckPoints.Add(pImageInfo);

                    bool isValueEqual = false;
                    bool isKeyShow = false;

                    foreach (XMLParameterNode psNode in getImageInfoReturnValue.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[0].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[0].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[0].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[0].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the follwing nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The return values in getImageInfo all match the expected. Get:" + getImageInfoResult.ResultContent);
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

        public void Run_Image_GetInfo_2ID_Case1171() //Case 1171: 1.3.6_GetImageInfo_N02_2 valid ID
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    // Input parameter
                    XMLParameter getImageInfoParam = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getImageInfo")
                        {
                            getImageInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    // Output value
                    XMLParameter getImageInfoReturn_1 = new XMLParameter("image");
                    XMLParameter getImageInfoReturn_2 = new XMLParameter("image");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_1")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturn_1.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturn_1.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_2")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturn_2.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturn_2.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                    }

                    #region Step 1: Call ImageService.getImageInfo to get the image info
                    CheckPoint pGetImageInfo = new CheckPoint("getImageInfo", "Call ImageService.getImageInfo to get the image info");
                    r.CheckPoints.Add(pGetImageInfo);

                    ImageService imageService = new ImageService();
                    XMLResult getImageInfoResult = imageService.getImageInfo(getImageInfoParam);

                    if (getImageInfoResult.IsErrorOccured)
                    {
                        pGetImageInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info returns error.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        pGetImageInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info succeeds.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);
                    }
                    #endregion

                    bool isValueEqual = false;
                    bool isKeyShow = false;

                    #region Step 2: Check the values in ImageService.getImageInfo return for the 1st image are correct
                    CheckPoint pImageInfo_1 = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return");
                    r.CheckPoints.Add(pImageInfo_1);

                    // Check the return for the 1st image
                    foreach (XMLParameterNode psNode in getImageInfoReturn_1.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[0].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[0].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[0].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo_1.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo_1.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[0].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo_1.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the following nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo_1.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo_1.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 1st image", "The return values in getImageInfo all match the expected");
                    }
                    #endregion

                    #region Step 3: Check the values in ImageService.getImageInfo return for the 2nd image are correct
                    CheckPoint pImageInfo_2 = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image");
                    r.CheckPoints.Add(pImageInfo_2);

                    foreach (XMLParameterNode psNode in getImageInfoReturn_2.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[1].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[1].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[1].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo_2.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo_2.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[1].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo_2.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the following nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo_2.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo_2.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image", "The return values in getImageInfo all match the expected");
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

        public void Run_Image_GetInfo_4ID_Case1172() //Case 1172: 1.3.6_GetImageInfo_N03_4 valid ID
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    // Input parameter
                    XMLParameter getImageInfoParam = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        getImageInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }

                    // Output value
                    XMLParameter getImageInfoReturn_1 = new XMLParameter("image");
                    XMLParameter getImageInfoReturn_2 = new XMLParameter("image");
                    XMLParameter getImageInfoReturn_3 = new XMLParameter("image");
                    XMLParameter getImageInfoReturn_4 = new XMLParameter("image");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_1")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturn_1.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturn_1.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_2")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturn_2.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturn_2.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_3")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturn_3.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturn_3.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_4")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturn_4.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturn_4.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                    }

                    #region Step 1: Call ImageService.getImageInfo to get the image info
                    CheckPoint pGetImageInfo = new CheckPoint("getImageInfo", "Call ImageService.getImageInfo to get the image info");
                    r.CheckPoints.Add(pGetImageInfo);

                    ImageService imageService = new ImageService();
                    XMLResult getImageInfoResult = imageService.getImageInfo(getImageInfoParam);

                    if (getImageInfoResult.IsErrorOccured)
                    {
                        pGetImageInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info returns error.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        pGetImageInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info succeeds.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);
                    }
                    #endregion

                    bool isValueEqual = false;
                    bool isKeyShow = false;

                    #region Step 2: Check the values in ImageService.getImageInfo return for the 1st image are correct
                    CheckPoint pImageInfo_1 = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return for the 1st image");
                    r.CheckPoints.Add(pImageInfo_1);

                    // Check the return for the 1st image
                    foreach (XMLParameterNode psNode in getImageInfoReturn_1.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[0].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[0].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[0].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo_1.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo_1.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[0].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo_1.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the following nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo_1.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo_1.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 1st image", "The return values in getImageInfo all match the expected");
                    }
                    #endregion

                    #region Step 3: Check the values in ImageService.getImageInfo return for the 2nd image are correct
                    CheckPoint pImageInfo_2 = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image");
                    r.CheckPoints.Add(pImageInfo_2);

                    foreach (XMLParameterNode psNode in getImageInfoReturn_2.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[1].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[1].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[1].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo_2.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo_2.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[1].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo_2.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the following nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo_2.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo_2.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 2nd image", "The return values in getImageInfo all match the expected");
                    }
                    #endregion

                    #region Step 4: Check the values in ImageService.getImageInfo return for the 3rd image are correct
                    CheckPoint pImageInfo_3 = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return for the 3rd image");
                    r.CheckPoints.Add(pImageInfo_3);

                    foreach (XMLParameterNode psNode in getImageInfoReturn_3.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[2].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[2].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[2].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo_3.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo_3.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 3rd image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[2].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo_3.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 3rd image", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the following nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo_3.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo_3.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 3rd image", "The return values in getImageInfo all match the expected");
                    }
                    #endregion

                    #region Step 5: Check the values in ImageService.getImageInfo return for the 4th image are correct
                    CheckPoint pImageInfo_4 = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return for the 4th image");
                    r.CheckPoints.Add(pImageInfo_4);

                    foreach (XMLParameterNode psNode in getImageInfoReturn_4.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getImageInfoResult.MultiResults[3].Parameters.Count; i++)
                        {
                            if (psNode.ParameterName == getImageInfoResult.MultiResults[3].Parameters[i].ParameterName)
                            {
                                isKeyShow = true;
                                isValueEqual = string.Equals(psNode.ParameterValue, getImageInfoResult.MultiResults[3].Parameters[i].ParameterValue);
                                break; // End current for loop to search node
                            }
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pImageInfo_4.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                pImageInfo_4.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 4th image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getImageInfoResult.MultiResults[3].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                pImageInfo_4.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 4th image", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the following nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pImageInfo_4.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                        pImageInfo_4.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return for the 4th image", "The return values in getImageInfo all match the expected");
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

        public void Run_Image_GetCephTracing_Normal_Case61() //Case 61: 1.3.6_GetCephTracing_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string imageInternalID = null;

                    // Input parameter
                    XMLParameter getCephTracingParam = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getCephTracing")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "imageInternalID")
                            {
                                imageInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    //Output value
                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = null;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCephTracing")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key.ToLower().Equals("state"))
                            {
                                if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                {
                                    ep_isReturnOK = true;
                                }
                                else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                {
                                    ep_isReturnOK = false;
                                }
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key.ToLower().Equals("returnvalue"))
                            {
                                ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }

                    #region Step 1: Call ImageService.getCephTracing to get the Ceph tracing info
                    CheckPoint pGetCephTracing = new CheckPoint("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info");
                    r.CheckPoints.Add(pGetCephTracing);

                    ImageService imageService = new ImageService();
                    XMLResult getCephTracingResult = imageService.getCephTracing(imageInternalID);

                    if (ep_isReturnOK) // Expect the call returns ok
                    {
                        if (getCephTracingResult.IsErrorOccured)
                        {
                            pGetCephTracing.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info returns error.");
                            pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info", getCephTracingResult.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            // Check the return value
                            if (String.Equals(getCephTracingResult.SingleResult.Replace("\n", "").Replace("\r", ""), ep_ReturnValue.Replace("\n", "").Replace("\r", "")))
                            {
                                pGetCephTracing.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info succeeds.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info", getCephTracingResult.Message);
                            }
                            else
                            {
                                pGetCephTracing.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info return value is not correct.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info return value is not correct", "Expect: " + ep_ReturnValue + "\n\n\n Actually Get: " + getCephTracingResult.SingleResult);
                            }
                        }
                    }
                    else // Expect the call returns error
                    {
                        if (getCephTracingResult.IsErrorOccured)
                        {
                            // Check the return value
                            if (getCephTracingResult.Message.Contains(ep_ReturnValue))
                            {
                                pGetCephTracing.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info returns error as expected.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info returns error as expected", getCephTracingResult.Message);
                            }
                            else
                            {
                                pGetCephTracing.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected", "Expect: " + ep_ReturnValue + "; Actually Get: " + getCephTracingResult.Message);
                            }
                        }
                        else
                        {
                            pGetCephTracing.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected.");
                            pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected", getCephTracingResult.Message);

                            SaveRound(r);
                            break; // There is error, end test case
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

        public void Run_Image_CreateImage_Exception_Case99() //Case 99: 1.3.6_CreateImage_Exception
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
                Round r = this.NewRound(runCount.ToString(), "Create image");

                PatientService patientSvc = new PatientService();
                ImageService imageSvc = new ImageService();

                XMLParameter pCreatePatient = new XMLParameter("patient");
                XMLParameter pCreateImage = new XMLParameter("image");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "createPatient")
                    {
                        pCreatePatient.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        isCreatePatient = true;
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "createImage")
                    {
                        pCreateImage.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }
                    if (ids.InputParameters.GetParameter(i).Step == "deletePatient")
                    {
                        isDeletePatient = ids.InputParameters.GetParameter(i).Value.ToLower().Equals("true");
                    }
                }

                string epErrorCode = string.Empty;
                string epErrorMessage = string.Empty;
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "createImage")
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                        {
                            epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                        {
                            epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }
                }

                try
                {
                    #region Create Patient
                    if (isCreatePatient)
                    {
                        CheckPoint cpCreatePatient = new CheckPoint("Create Patient", "Test create patient");
                        r.CheckPoints.Add(cpCreatePatient);
                        XMLResult rtCreatePatient = patientSvc.createPatient(pCreatePatient);
                        if (rtCreatePatient.IsErrorOccured)
                        {
                            cpCreatePatient.Result = TestResult.Fail;
                            cpCreatePatient.Outputs.AddParameter("create patient returns error", "Create Patient", rtCreatePatient.ResultContent);
                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            patientUID = rtCreatePatient.SingleResult;
                            cpCreatePatient.Result = TestResult.Pass;
                            cpCreatePatient.Outputs.AddParameter("create patient returns success", "Create Patient", rtCreatePatient.ResultContent);
                        }
                    }
                    #endregion

                    #region Create Image
                    CheckPoint cpCreateImage = new CheckPoint("Create Image", "Test create image");
                    r.CheckPoints.Add(cpCreateImage);
                    if (patientUID != null)
                    {
                        pCreateImage.AddParameter("patient_internal_id", patientUID);
                        XMLResult rtCreateImage = imageSvc.createImage(pCreateImage);
                        if (rtCreateImage.IsErrorOccured)
                        {
                            if (rtCreateImage.Code.ToString().Equals(epErrorCode) && rtCreateImage.Message.Contains(epErrorMessage))
                            {
                                cpCreateImage.Result = TestResult.Pass;
                                cpCreateImage.Outputs.AddParameter("Create image returns correct error message as expected", "Create Image", rtCreateImage.ResultContent);
                            }
                            else
                            {
                                cpCreateImage.Result = TestResult.Fail;
                                cpCreateImage.Outputs.AddParameter("Create image doesn't return correct error info as expected", "Create Image", rtCreateImage.ResultContent);
                            }
                        }
                        else
                        {
                            cpCreateImage.Result = TestResult.Fail;
                            cpCreateImage.Outputs.AddParameter("Create image doesn't return error as expected", "Create Image", rtCreateImage.ResultContent);
                        }
                    }
                    #endregion


                    #region Delete patient
                    if (isDeletePatient)
                    {
                        CheckPoint cpDeletePatient = new CheckPoint("DeletePatient", "Delete the created patient");
                        r.CheckPoints.Add(cpDeletePatient);

                        XMLResult rtDeletePatient = patientSvc.deletePatient(patientUID);

                        if (rtDeletePatient.IsErrorOccured)
                        {
                            cpDeletePatient.Result = TestResult.Fail;
                            cpDeletePatient.Outputs.AddParameter("delete patient returns error", "Delete Patient", rtDeletePatient.ResultContent);
                            SaveRound(r);
                            break;
                        }
                        else
                        {
                            cpDeletePatient.Result = TestResult.Pass;
                            cpDeletePatient.Outputs.AddParameter("delete patient returns success", "Delete Patient", rtDeletePatient.ResultContent);
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

        public void Run_Image_Delete_Exception_Case101()  //Case 101: 1.3.6_DeleteImage_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                string imageUID = null;
                Round r = this.NewRound(runCount.ToString(), "Delete image");

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Key == "imageID")
                    {
                        imageUID = ids.InputParameters.GetParameter(i).Value;
                    }
                }

                string epErrorCode = string.Empty;
                string epErrorMessage = string.Empty;
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "delete")
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                        {
                            epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                        {
                            epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }
                }
                try
                {
                    ImageService imageSvc = new ImageService();

                    CheckPoint cpDeleteImage = new CheckPoint("Delete Image", "Test delete image exception cases");
                    r.CheckPoints.Add(cpDeleteImage);

                    XMLParameter deletePreference = new XMLParameter("preferences");
                    deletePreference.AddParameter("completedFlag", "true");
                    XMLResult rtDeleteImage = imageSvc.deleteImage(imageUID, deletePreference);
                    if (rtDeleteImage.IsErrorOccured)
                    {
                        if (rtDeleteImage.Code.ToString().Equals(epErrorCode) && rtDeleteImage.Message.Contains(epErrorMessage))
                        {
                            cpDeleteImage.Result = TestResult.Pass;
                            cpDeleteImage.Outputs.AddParameter("Delete image returns correct error message as expected", "Delete Image", rtDeleteImage.ResultContent);
                        }
                        else
                        {
                            cpDeleteImage.Result = TestResult.Fail;
                            cpDeleteImage.Outputs.AddParameter("Delete image doesn't return correct error info as expected", "Delete Image", rtDeleteImage.ResultContent);
                        }
                    }
                    else
                    {
                        cpDeleteImage.Result = TestResult.Pass;
                        cpDeleteImage.Outputs.AddParameter("Delete image doesn't return error as expected", "Delete", rtDeleteImage.ResultContent);
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

        public void Run_Image_GetCephTracing_Exception_Case102() //Case 102: 1.3.6_GetCephTracing_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string imageInternalID = null;

                    // Input parameter
                    XMLParameter getCephTracingParam = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getCephTracing")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "imageInternalID")
                            {
                                imageInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    //Output value
                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = null;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCephTracing")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key.ToLower().Equals("state"))
                            {
                                if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                {
                                    ep_isReturnOK = true;
                                }
                                else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                {
                                    ep_isReturnOK = false;
                                }
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key.ToLower().Equals("returnvalue"))
                            {
                                ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }

                    #region Step 1: Call ImageService.getCephTracing to get the Ceph tracing info
                    CheckPoint pGetCephTracing = new CheckPoint("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info");
                    r.CheckPoints.Add(pGetCephTracing);

                    ImageService imageService = new ImageService();
                    XMLResult getCephTracingResult = imageService.getCephTracing(imageInternalID);

                    if (ep_isReturnOK) // Expect the call returns ok
                    {
                        if (getCephTracingResult.IsErrorOccured)
                        {
                            pGetCephTracing.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info returns error.");
                            pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info", getCephTracingResult.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                        else
                        {
                            // Check the return value
                            if (String.Equals(getCephTracingResult.SingleResult.Replace("\n", "").Replace("\r", ""), ep_ReturnValue.Replace("\n", "").Replace("\r", "")))
                            {
                                pGetCephTracing.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info succeeds.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info", getCephTracingResult.Message);
                            }
                            else
                            {
                                pGetCephTracing.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info return value is not correct.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info return value is not correct", "Expect: " + ep_ReturnValue + "\n\n\n Actually Get: " + getCephTracingResult.SingleResult);
                            }
                        }
                    }
                    else // Expect the call returns error
                    {
                        if (getCephTracingResult.IsErrorOccured)
                        {
                            // Check the return value
                            if (getCephTracingResult.Message.Contains(ep_ReturnValue))
                            {
                                pGetCephTracing.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info returns error as expected.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info returns error as expected", getCephTracingResult.Message);
                            }
                            else
                            {
                                pGetCephTracing.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected.");
                                pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected", "Expect: " + ep_ReturnValue + "; Actually Get: " + getCephTracingResult.Message);
                            }
                        }
                        else
                        {
                            pGetCephTracing.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected.");
                            pGetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info not returns error as expected", getCephTracingResult.Message);

                            SaveRound(r);
                            break; // There is error, end test case
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

        public void Run_Image_GetImageDescription_Exception_Case103() //Case 103: 1.3.6_GetImageDescription_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string imageID = null;

                    // Input parameter
                    XMLParameter pGetImageDescription = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getImageDescription")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                imageID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    // Output value
                    string epErrorCode = string.Empty;
                    string epErrorMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                        {
                            epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                        {
                            epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }

                    #region Step 1: Call ImageService.getImageDescription to get the image description
                    CheckPoint cpGetImageDescription = new CheckPoint("getImageDescription", "Call ImageService.getImageDescription to get the image description");
                    r.CheckPoints.Add(cpGetImageDescription);

                    ImageService imageService = new ImageService();
                    XMLResult rtGetImageDescription = imageService.getImageDescription(imageID);

                    if (rtGetImageDescription.IsErrorOccured)
                    {
                        if (rtGetImageDescription.Code.ToString() == epErrorCode && rtGetImageDescription.Message.Contains(epErrorMessage))
                        {
                            cpGetImageDescription.Result = TestResult.Pass;
                            cpGetImageDescription.Outputs.AddParameter("getImageDescription", "Call ImageService.getImageDescription returns error as expected", rtGetImageDescription.Message);
                        }
                        else
                        {
                            cpGetImageDescription.Result = TestResult.Fail;
                            cpGetImageDescription.Outputs.AddParameter("getImageDescription", "Call ImageService.getImageDescription doesn't returns the expected error", "Error Code: " + rtGetImageDescription.Code + "; Error Message: " + rtGetImageDescription.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                    }
                    else
                    {
                        cpGetImageDescription.Result = TestResult.Fail;
                        cpGetImageDescription.Outputs.AddParameter("getImageDescription", "Call ImageService.getImageDescription doesn't returns the expected error", rtGetImageDescription.Message);

                        SaveRound(r);
                        break; // There is error, end test case
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

        public void Run_Image_ListPresentationStates_Normal_Case46() //Case 46: 1.3.6_ListPresentationStates_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string imageInternalID = null;
                    ImageService imageService = new ImageService();

                    #region Initialize parameter
                    // Input parameter
                    XMLParameter p_listPSFilter = new XMLParameter("filter");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "listPresentationState")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "imageInternalID")
                            {
                                imageInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "filter")
                            {
                                p_listPSFilter.AddParameter("current", ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }

                    //Output value
                    bool ep_isReturnOK = true;
                    string ep_returnMessage = string.Empty;
                    XMLParameter ep_listPresentationState = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "listPresentationState")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "returnState")
                            {
                                if (ids.ExpectedValues.GetParameter(i).Value.ToLower() == "pass")
                                {
                                    ep_isReturnOK = true;
                                }
                                else if (ids.ExpectedValues.GetParameter(i).Value.ToLower() == "fail")
                                {
                                    ep_isReturnOK = false;
                                }
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "returnMessage")
                            {
                                ep_returnMessage = ids.ExpectedValues.GetParameter(i).Value;
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "internal_id")
                            {
                                ep_listPresentationState.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                    }
                    #endregion

                    #region Step 1: Call ImageService.listPresentationState to list presentationState
                    CheckPoint cp_listPresentationState = new CheckPoint("listPresentationState", "Call ImageService.listPresentationState to list presentationState");
                    r.CheckPoints.Add(cp_listPresentationState);

                    XMLResult rt_listPresentationState = imageService.listPresentationState(p_listPSFilter, imageInternalID);

                    if (ep_isReturnOK) // Expect the call returns ok
                    {
                        if (rt_listPresentationState.IsErrorOccured)
                        {
                            cp_listPresentationState.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState returns error.");
                            cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState", rt_listPresentationState.Message);

                            SaveRound(r);
                            continue; // There is error, end current round and test case
                        }
                        else
                        {
                            // Check the return value
                            bool isPSCorrect = false;
                            int i = 0;
                            for (i = 0; i < ep_listPresentationState.Parameters.Count; i++)
                            {
                                isPSCorrect = rt_listPresentationState.ResultContent.Contains(ep_listPresentationState.Parameters[i].ParameterValue);
                                if (!isPSCorrect)
                                {
                                    break; // stop compare and keep the index to report which one is not shown
                                }
                            }

                            if (isPSCorrect)
                            {
                                cp_listPresentationState.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState succeeds and the PS info is correct.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState", rt_listPresentationState.Message);
                            }
                            else
                            {
                                cp_listPresentationState.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState return value is not correct.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState return value is not correct", "Expect PS: " + ep_listPresentationState.Parameters[i].ParameterValue + " to show in the return. Actually Get: " + rt_listPresentationState.ResultContent);
                            }
                        }
                    }
                    else // Expect the call returns error
                    {
                        if (rt_listPresentationState.IsErrorOccured)
                        {
                            // Check the return value
                            if (rt_listPresentationState.Message.Contains(ep_returnMessage))
                            {
                                cp_listPresentationState.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState returns error as expected.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState returns error as expected", rt_listPresentationState.Message);
                            }
                            else
                            {
                                cp_listPresentationState.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState not returns error as expected.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState not returns error as expected", "Expect: " + ep_returnMessage + "; Actually Get: " + rt_listPresentationState.Message);
                            }
                        }
                        else
                        {
                            cp_listPresentationState.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState not returns error as expected.");
                            cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState not returns error as expected", rt_listPresentationState.Message);

                            SaveRound(r);
                            continue; // There is error, end test case
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

        public void Run_Image_ListPresentationStates_Exception_Case104() //Case 104: 1.3.6_ListPresentationStates_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string imageInternalID = null;
                    ImageService imageService = new ImageService();

                    #region Initialize parameter
                    // Input parameter
                    XMLParameter p_listPSFilter = new XMLParameter("filter");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "listPresentationState")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "imageInternalID")
                            {
                                imageInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "filter")
                            {
                                p_listPSFilter.AddParameter("current", ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }

                    //Output value
                    bool ep_isReturnOK = true;
                    string ep_returnMessage = string.Empty;
                    XMLParameter ep_listPresentationState = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "listPresentationState")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "returnState")
                            {
                                if (ids.ExpectedValues.GetParameter(i).Value.ToLower() == "pass")
                                {
                                    ep_isReturnOK = true;
                                }
                                else if (ids.ExpectedValues.GetParameter(i).Value.ToLower() == "fail")
                                {
                                    ep_isReturnOK = false;
                                }
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "returnMessage")
                            {
                                ep_returnMessage = ids.ExpectedValues.GetParameter(i).Value;
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "internal_id")
                            {
                                ep_listPresentationState.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                    }
                    #endregion

                    #region Step 1: Call ImageService.listPresentationState to list presentationState
                    CheckPoint cp_listPresentationState = new CheckPoint("listPresentationState", "Call ImageService.listPresentationState to list presentationState");
                    r.CheckPoints.Add(cp_listPresentationState);

                    XMLResult rt_listPresentationState = imageService.listPresentationState(p_listPSFilter, imageInternalID);

                    if (ep_isReturnOK) // Expect the call returns ok
                    {
                        if (rt_listPresentationState.IsErrorOccured)
                        {
                            cp_listPresentationState.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState returns error.");
                            cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState", rt_listPresentationState.Message);

                            SaveRound(r);
                            continue; // There is error, end current round and test case
                        }
                        else
                        {
                            // Check the return value
                            bool isPSCorrect = false;
                            int i = 0;
                            for (i = 0; i < ep_listPresentationState.Parameters.Count; i++)
                            {
                                isPSCorrect = rt_listPresentationState.ResultContent.Contains(ep_listPresentationState.Parameters[i].ParameterValue);
                                if (!isPSCorrect)
                                {
                                    break; // stop compare and keep the index to report which one is not shown
                                }
                            }

                            if (isPSCorrect)
                            {
                                cp_listPresentationState.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState succeeds and the PS info is correct.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState", rt_listPresentationState.Message);
                            }
                            else
                            {
                                cp_listPresentationState.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState return value is not correct.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState return value is not correct", "Expect PS: " + ep_listPresentationState.Parameters[i].ParameterValue + " to show in the return. Actually Get: " + rt_listPresentationState.ResultContent);
                            }
                        }
                    }
                    else // Expect the call returns error
                    {
                        if (rt_listPresentationState.IsErrorOccured)
                        {
                            // Check the return value
                            if (rt_listPresentationState.Message.Contains(ep_returnMessage))
                            {
                                cp_listPresentationState.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState returns error as expected.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState returns error as expected", rt_listPresentationState.Message);
                            }
                            else
                            {
                                cp_listPresentationState.Result = TestResult.Fail;

                                System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState not returns error as expected.");
                                cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState not returns error as expected", "Expect: " + ep_returnMessage + "; Actually Get: " + rt_listPresentationState.Message);
                            }
                        }
                        else
                        {
                            cp_listPresentationState.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.listPresentationState to list presentationState not returns error as expected.");
                            cp_listPresentationState.Outputs.AddParameter("listPresentationState", "Call ImageService.listPresentationState to list presentationState not returns error as expected", rt_listPresentationState.Message);

                            SaveRound(r);
                            continue; // There is error, end test case
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

        public void Run_Image_SetCephTracing_Normal_Case111() //Case 111: 1.3.6_SetCephTracing_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test Image Service: createImage -> getImageInfo & getImageDescription -> setImageInfo ->  getImageInfo & getImageDescription -> deleteImage");

                try
                {
                    string imageinternalUid = null;
                    ImageService imageService = new ImageService();

                    #region Parameter Initialize
                    //Input parameter
                    XMLParameter p_CreateImage = new XMLParameter("image");
                    string p_SetCephTracing = null;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createImage")
                        {
                            p_CreateImage.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                        if (ids.InputParameters.GetParameter(i).Step == "setCephTracing" && ids.InputParameters.GetParameter(i).Key == "cephTracing")
                        {
                            p_SetCephTracing = ids.InputParameters.GetParameter(i).Value;
                        }
                    }

                    //Output value
                    string ep_GetCephTraingReturn = null;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getCephTracing" && ids.ExpectedValues.GetParameter(i).Key == "cephTracing")
                        {
                            ep_GetCephTraingReturn = ids.ExpectedValues.GetParameter(i).Value;
                        }

                    }
                    #endregion

                    #region Step 1: Call ImageService.createImage to create a new image
                    CheckPoint cp_CreateImage = new CheckPoint("Image create", "Call imageService.createImage to create a new image");
                    r.CheckPoints.Add(cp_CreateImage);

                    XMLResult rt_CreateImage = imageService.createImage(p_CreateImage);
                    if (rt_CreateImage.IsErrorOccured)
                    {
                        cp_CreateImage.Result = TestResult.Fail;
                        cp_CreateImage.Outputs.AddParameter("create", "Create image returns error", rt_CreateImage.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_CreateImage.Result = TestResult.Pass;
                        cp_CreateImage.Outputs.AddParameter("create", "Create image returns success", rt_CreateImage.Message);
                        imageinternalUid = rt_CreateImage.SingleResult;
                    }
                    #endregion

                    #region Step 2: Call ImageService.setCephTracing to set the Ceph Tracing
                    CheckPoint cp_SetCephTracing = new CheckPoint("Set cephTracing", "Call imageService.setCephTracing to set the cephTracing");
                    r.CheckPoints.Add(cp_SetCephTracing);

                    XMLResult rt_SetCephTracing = imageService.setCephTracing(p_SetCephTracing, imageinternalUid);
                    if (rt_SetCephTracing.IsErrorOccured)
                    {
                        cp_SetCephTracing.Result = TestResult.Fail;
                        cp_SetCephTracing.Outputs.AddParameter("setCephTracing", "Call imageService.setCephTracing returns error", rt_SetCephTracing.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_SetCephTracing.Result = TestResult.Pass;
                        cp_SetCephTracing.Outputs.AddParameter("setCephTracing", "Call imageService.setCephTracing returns success", rt_SetCephTracing.Message);
                    }
                    #endregion

                    #region Step 3: Call ImageService.getCephTracing to get the Ceph tracing info
                    CheckPoint cp_GetCephTracing = new CheckPoint("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info");
                    r.CheckPoints.Add(cp_GetCephTracing);

                    XMLResult getCephTracingResult = imageService.getCephTracing(imageinternalUid);

                    if (getCephTracingResult.IsErrorOccured)
                    {
                        cp_GetCephTracing.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info returns error.");
                        cp_GetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info", getCephTracingResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        // Check the return value
                        if (String.Equals(getCephTracingResult.SingleResult.Replace("\n", "").Replace("\r", ""), ep_GetCephTraingReturn.Replace("\n", "").Replace("\r", "")))
                        {
                            cp_GetCephTracing.Result = TestResult.Pass;

                            System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info succeeds.");
                            cp_GetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info", getCephTracingResult.Message);
                        }
                        else
                        {
                            cp_GetCephTracing.Result = TestResult.Fail;

                            System.Diagnostics.Debug.Print("Call ImageService.getCephTracing to get the Ceph tracing info return value is not correct.");
                            cp_GetCephTracing.Outputs.AddParameter("getCephTracing", "Call ImageService.getCephTracing to get the Ceph tracing info return value is not correct", "Expect: " + ep_GetCephTraingReturn + "\n\n\n Actually Get: " + getCephTracingResult.SingleResult);
                        }
                    }
                    #endregion

                    #region Step 4: Call ImageService.deleteImage to delete the image
                    CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                    r.CheckPoints.Add(cp_DeleteImage);

                    XMLResult rt_DeleteImage = imageService.deleteImage(imageinternalUid, new XMLParameter("preferences"));
                    if (rt_DeleteImage.IsErrorOccured)
                    {
                        cp_DeleteImage.Result = TestResult.Fail;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_DeleteImage.Result = TestResult.Pass;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_Image_GetImageInfo_Exception_Case198() //Case 198: 1.3.6_GetImageInfo_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    // Input parameter
                    XMLParameter getImageInfoParam = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                        {
                            getImageInfoParam.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    // Output value
                    string epErrorCode = string.Empty;
                    string epErrorMessage = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                        {
                            epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                        {
                            epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }

                    #region Step 1: Call ImageService.getImageInfo to get the image info
                    CheckPoint pGetImageInfo = new CheckPoint("getImageInfo", "Call ImageService.getImageInfo to get the image info");
                    r.CheckPoints.Add(pGetImageInfo);

                    ImageService imageService = new ImageService();
                    XMLResult getImageInfoResult = imageService.getImageInfo(getImageInfoParam);

                    if (getImageInfoResult.IsErrorOccured)
                    {
                        if (getImageInfoResult.Code.ToString() == epErrorCode && getImageInfoResult.Message.Contains(epErrorMessage))
                        {
                            pGetImageInfo.Result = TestResult.Pass;
                            pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo returns error as expected", "Return Code: " + getImageInfoResult.Code + "; Return message: " + getImageInfoResult.Message);
                        }
                        else
                        {
                            pGetImageInfo.Result = TestResult.Fail;
                            pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo doesnot return error as expected", "Return Code: " + getImageInfoResult.Code + "; Return message: " + getImageInfoResult.Message);

                            SaveRound(r);
                            break; // There is error, end test case
                        }
                    }
                    else
                    {
                        pGetImageInfo.Result = TestResult.Fail;
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo doesnot return error", "Return Code: " + getImageInfoResult.Code + "; Return message: " + getImageInfoResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
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

        public void Run_Image_Import_GetInfo_Case1624() //Case 1624: 1.3.6_WorkFlow_N05_Import DICOM Image_Check DICOM Info
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    // Input parameter
                    string patientID = null;
                    string objectFileFullPath = null;
                    string imageID = null;
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
                    }

                    // Output value
                    XMLParameter getImageInfoReturnValue = new XMLParameter("image");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "path") // to handle different path, such as in different OS or user changes the install dr
                            {
                                string path = Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory") + ids.ExpectedValues.GetParameter(i).Value;
                                getImageInfoReturnValue.AddParameter(ids.ExpectedValues.GetParameter(i).Key, path);
                            }
                            else
                            {
                                getImageInfoReturnValue.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                            }
                        }
                    }

                    #region Step: mport the image
                    ImportService importSvc = new ImportService();
                    XMLResult rtImport = importSvc.CallImportAndCheck(r, patientID, objectFileFullPath, null);
                    if (rtImport.IsErrorOccured)
                    {
                        continue;
                    }
                    else
                    {
                        imageID = rtImport.SingleResult;
                    }
                    #endregion

                    #region Step 1: Call ImageService.getImageInfo to get the image info
                    CheckPoint pGetImageInfo = new CheckPoint("getImageInfo", "Call ImageService.getImageInfo to get the image info");
                    r.CheckPoints.Add(pGetImageInfo);

                    ImageService imageService = new ImageService();
                    XMLParameter getImageInfoParam = new XMLParameter("image");
                    getImageInfoParam.AddParameter("internal_id", imageID);
                    XMLResult getImageInfoResult = imageService.getImageInfo(getImageInfoParam);

                    if (getImageInfoResult.IsErrorOccured)
                    {
                        pGetImageInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info returns error.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        pGetImageInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call ImageService.getImageInfo to get the image info succeeds.");
                        pGetImageInfo.Outputs.AddParameter("getImageInfo", "Call ImageService.getImageInfo to get the image info", getImageInfoResult.Message);
                    }
                    #endregion

                    #region Step 2: Check the values in ImageService.getImageInfo return are correct
                    CheckPoint pImageInfo = new CheckPoint("ImageInfo", "Check the values in ImageService.getImageInfo return");
                    r.CheckPoints.Add(pImageInfo);

                    string dicom_info = null;
                    for (int i = 0; i < getImageInfoResult.DicomArrayResult.Parameters.Count; i++)
                    {
                        if (getImageInfoResult.DicomArrayResult.Parameters[i].ParameterName == "dicom_info")
                        {
                            dicom_info = getImageInfoResult.DicomArrayResult.Parameters[i].ParameterValue;
                            break; // End current for loop to search node
                        }
                    }

                    foreach (XMLParameterNode psNode in getImageInfoReturnValue.Parameters)
                    {
                        if (!dicom_info.Contains("<parameter key=\"" + psNode.ParameterName + "\" value=\"" + psNode.ParameterValue + "\" />"))   //example: <parameter key="dcm_modality" value="IO" />
                        {
                            pImageInfo.Result = TestResult.Fail;
                            pImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + dicom_info);
                        }
                    }

                    if (pImageInfo.Result != TestResult.Fail)
                    {
                        pImageInfo.Result = TestResult.Pass;
                        pImageInfo.Outputs.AddParameter("ImageInfo", "Check the values in ImageService.getImageInfo return", "The return values in getImageInfo all match the expected. Get:" + getImageInfoResult.ResultContent);
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

        public void Run_Image_WorkFlow_Case1178() // Case 1178: 1.3.6_WorkFlow_N04_createImage_getImageInfo_getImageDescription_setImageInfo_getImageInfo_getImageDescription_deleteImage
        {
            int runCount = 0;
            string imageinternalUid = string.Empty;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test Image Service: createImage -> getImageInfo & getImageDescription -> setImageInfo ->  getImageInfo & getImageDescription -> deleteImage");

                try
                {
                    ImageService imageService = new ImageService();

                    #region Parameter Initialize
                    XMLParameter p_CreateImage = new XMLParameter("image");
                    XMLParameter p_SetImageInfo = new XMLParameter("image");
                    //XMLParameter p_SetImage_series = new XMLParameter("series");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createImage_image")
                        {
                            p_CreateImage.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                        if (ids.InputParameters.GetParameter(i).Step == "setImageInfo")
                        {
                            p_SetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                        //if (ids.InputParameters.GetParameter(i).Step == "setImageInfo_series")
                        //{
                        //    p_SetImage_series.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        //}
                    }

                    XMLParameter ep_getImageInfoAfterCreate = new XMLParameter("image");
                    XMLParameter ep_getImageDescriptionAfterCreate = new XMLParameter("image");
                    //XMLParameter ep_getImageDescriptionAfterCreate_series = new XMLParameter("series");
                    XMLParameter ep_getImageInfoAfterSet = new XMLParameter("image");
                    XMLParameter ep_getImageDescriptionAfterSet = new XMLParameter("image");
                    //XMLParameter ep_getImageDescriptionAfterSet_series = new XMLParameter("series");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_AfterCreate")
                        {
                            ep_getImageInfoAfterCreate.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription_AfterCreate")
                        {
                            ep_getImageDescriptionAfterCreate.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                        //else if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription_AfterCreate_series")
                        //{
                        //    ep_getImageDescriptionAfterCreate_series.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        //}
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getImageInfo_AfterSet")
                        {
                            ep_getImageInfoAfterSet.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription_AfterSet")
                        {
                            ep_getImageDescriptionAfterSet.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                        //else if (ids.ExpectedValues.GetParameter(i).Step == "getImageDescription_AfterSet_series")
                        //{
                        //    ep_getImageDescriptionAfterSet_series.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        //}
                    }
                    #endregion

                    #region Step 1: Call ImageService.createImage to create a new image
                    CheckPoint cp_CreateImage = new CheckPoint("Image create", "Call imageService.createImage to create a new image");
                    r.CheckPoints.Add(cp_CreateImage);

                    XMLResult rt_CreateImage = imageService.createImage(p_CreateImage);
                    if (rt_CreateImage.IsErrorOccured)
                    {
                        cp_CreateImage.Result = TestResult.Fail;
                        cp_CreateImage.Outputs.AddParameter("create", "Create image returns error", rt_CreateImage.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_CreateImage.Result = TestResult.Pass;
                        cp_CreateImage.Outputs.AddParameter("create", "Create image returns success", rt_CreateImage.Message);
                        imageinternalUid = rt_CreateImage.SingleResult;
                    }
                    #endregion

                    #region Step 2: Call imageService.getImageInfo to check the return value
                    CheckPoint cp_GetImageInfoAfterCreate = new CheckPoint("Get Image Info", "Call imageService.GetImageInfo to get Image Info after the image is created");
                    r.CheckPoints.Add(cp_GetImageInfoAfterCreate);

                    XMLParameter p_GetImageInfoAfterCreate = new XMLParameter("image");
                    p_GetImageInfoAfterCreate.AddParameter("internal_id", imageinternalUid);

                    XMLResult rt_GetImageInfoAfterCreate = imageService.getImageInfo(p_GetImageInfoAfterCreate);
                    if (rt_GetImageInfoAfterCreate.IsErrorOccured)
                    {
                        cp_GetImageInfoAfterCreate.Result = TestResult.Fail;
                        cp_GetImageInfoAfterCreate.Outputs.AddParameter("getImageInfo", "Get image info after create returns error", rt_GetImageInfoAfterCreate.Message);

                        goto CLEANUP;
                    }
                    else
                    {
                        //Check the value
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in ep_getImageInfoAfterCreate.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rt_GetImageInfoAfterCreate.MultiResults[0].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName == rt_GetImageInfoAfterCreate.MultiResults[0].Parameters[i].ParameterName)
                                {
                                    isKeyShow = true;
                                    isValueEqual = string.Equals(psNode.ParameterValue, rt_GetImageInfoAfterCreate.MultiResults[0].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cp_GetImageInfoAfterCreate.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                    cp_GetImageInfoAfterCreate.Outputs.AddParameter("ImageInfo", "Check the values in getImageInfo return after create image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetImageInfoAfterCreate.MultiResults[0].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                    cp_GetImageInfoAfterCreate.Outputs.AddParameter("ImageInfo", "Check the values in getImageInfo return after create image", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cp_GetImageInfoAfterCreate.Result = TestResult.Pass;

                            System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                            cp_GetImageInfoAfterCreate.Outputs.AddParameter("getImageInfo", "Get image info after create returns success", rt_GetImageInfoAfterCreate.Message);
                            cp_GetImageInfoAfterCreate.Outputs.AddParameter("ImageInfo", "Check the values in getImageInfo return after create", "The return values in getImageInfo all match the expected");
                        }
                    }
                    #endregion

                    #region Step 3: Call ImageService.getImageDescription to check the return value
                    CheckPoint cp_GetImageDescriptionAfterCreate = new CheckPoint("Get Image Description", "Call Call imageService.GetImageDescription to get Image description after the image is created");
                    r.CheckPoints.Add(cp_GetImageDescriptionAfterCreate);

                    XMLResult rt_GetImageDescriptionAfterCreate = imageService.getImageDescription(imageinternalUid);
                    if (rt_GetImageDescriptionAfterCreate.IsErrorOccured)
                    {
                        cp_GetImageDescriptionAfterCreate.Result = TestResult.Fail;
                        cp_GetImageDescriptionAfterCreate.Outputs.AddParameter("getImageDescription", "Get image description after create returns error", rt_GetImageDescriptionAfterCreate.Message);

                        goto CLEANUP;
                    }
                    else
                    {
                        //Check the value
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in ep_getImageDescriptionAfterCreate.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rt_GetImageDescriptionAfterCreate.MultiResults[0].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName == rt_GetImageDescriptionAfterCreate.MultiResults[0].Parameters[i].ParameterName)
                                {
                                    isKeyShow = true;
                                    isValueEqual = string.Equals(psNode.ParameterValue, rt_GetImageDescriptionAfterCreate.MultiResults[0].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cp_GetImageDescriptionAfterCreate.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageDescription does not match the expected.");
                                    cp_GetImageDescriptionAfterCreate.Outputs.AddParameter("ImageDescription", "Check the values in getImageDescription return after create image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetImageDescriptionAfterCreate.MultiResults[0].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageDescription does not contain the node: " + psNode.ParameterName);
                                    cp_GetImageDescriptionAfterCreate.Outputs.AddParameter("ImageDescription", "Check the values in getImageDescription return after create image", "The return value in getImageDescription does not contain the node: " + psNode.ParameterName);
                                }
                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cp_GetImageDescriptionAfterCreate.Result = TestResult.Pass;

                            System.Diagnostics.Debug.Print("The return values in getImageDescription all match the expected.");
                            cp_GetImageDescriptionAfterCreate.Outputs.AddParameter("getImageDescription", "Get image Description after create returns success", rt_GetImageDescriptionAfterCreate.Message);
                            cp_GetImageDescriptionAfterCreate.Outputs.AddParameter("ImageDescription", "Check the values in getImageDescription return after create", "The return values in getImageDescription all match the expected");
                        }
                    }
                    #endregion

                    #region Step 4: Call imageService.setImageInfo to set image info
                    CheckPoint cp_SetImageInfoAfterCreate = new CheckPoint("Set Image Info", "Call  imageService.SetImageInfo to set Image Info after the image is created");
                    r.CheckPoints.Add(cp_SetImageInfoAfterCreate);

                    XMLResult rt_SetImageInfoAfterCreate = imageService.setImageInfo(p_SetImageInfo, imageinternalUid);
                    if (rt_SetImageInfoAfterCreate.IsErrorOccured)
                    {
                        cp_SetImageInfoAfterCreate.Result = TestResult.Fail;
                        cp_SetImageInfoAfterCreate.Outputs.AddParameter("setImageInfo", "Set image info after create returns error", rt_SetImageInfoAfterCreate.Message);

                        goto CLEANUP;
                    }
                    else
                    {
                        cp_SetImageInfoAfterCreate.Result = TestResult.Pass;
                        cp_SetImageInfoAfterCreate.Outputs.AddParameter("setImageInfo", "Set image info after create returns success", rt_SetImageInfoAfterCreate.Message);
                    }
                    #endregion

                    #region Step 5: Call imageService.getImageInfo to check the return value
                    CheckPoint cp_GetImageInfoAfterSet = new CheckPoint("Get Image Info", "Call imageService.GetImageInfo to get Image Info after set the image info");
                    r.CheckPoints.Add(cp_GetImageInfoAfterSet);

                    XMLParameter p_GetImageInfoAfterSet = new XMLParameter("image");
                    p_GetImageInfoAfterSet.AddParameter("internal_id", imageinternalUid);

                    XMLResult rt_GetImageInfoAfterSet = imageService.getImageInfo(p_GetImageInfoAfterSet);
                    if (rt_GetImageInfoAfterSet.IsErrorOccured)
                    {
                        cp_GetImageInfoAfterSet.Result = TestResult.Fail;
                        cp_GetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Get image info after set returns error", rt_GetImageInfoAfterSet.Message);

                        goto CLEANUP;
                    }
                    else
                    {
                        //Check the value
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in ep_getImageInfoAfterSet.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rt_GetImageInfoAfterSet.MultiResults[0].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName == rt_GetImageInfoAfterSet.MultiResults[0].Parameters[i].ParameterName)
                                {
                                    isKeyShow = true;
                                    isValueEqual = string.Equals(psNode.ParameterValue, rt_GetImageInfoAfterSet.MultiResults[0].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cp_GetImageInfoAfterSet.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageInfo does not match the expected.");
                                    cp_GetImageInfoAfterSet.Outputs.AddParameter("ImageInfo", "Check the values in getImageInfo return after set image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetImageInfoAfterSet.MultiResults[0].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                    cp_GetImageInfoAfterSet.Outputs.AddParameter("ImageInfo", "Check the values in getImageInfo return after set image", "The return value in getImageInfo does not contain the node: " + psNode.ParameterName);
                                }

                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cp_GetImageInfoAfterSet.Result = TestResult.Pass;

                            System.Diagnostics.Debug.Print("The return values in getImageInfo all match the expected.");
                            cp_GetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Get image info after set returns success", rt_GetImageInfoAfterSet.Message);
                            cp_GetImageInfoAfterSet.Outputs.AddParameter("ImageInfo", "Check the values in getImageInfo return after set", "The return values in getImageInfo all match the expected");
                        }
                    }
                    #endregion

                    #region Step 6: Call ImageService.getImageDescription to check the return value
                    CheckPoint cp_GetImageDescriptionAfterSet = new CheckPoint("Get Image Description", "Call Call imageService.GetImageDescription to get Image description after the image is setd");
                    r.CheckPoints.Add(cp_GetImageDescriptionAfterSet);

                    XMLResult rt_GetImageDescriptionAfterSet = imageService.getImageDescription(imageinternalUid);
                    if (rt_GetImageDescriptionAfterSet.IsErrorOccured)
                    {
                        cp_GetImageDescriptionAfterSet.Result = TestResult.Fail;
                        cp_GetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Get image description after set returns error", rt_GetImageDescriptionAfterSet.Message);

                        goto CLEANUP;
                    }
                    else
                    {
                        //Check the value
                        bool isValueEqual = false;
                        bool isKeyShow = false;

                        foreach (XMLParameterNode psNode in ep_getImageDescriptionAfterSet.Parameters)
                        {
                            isValueEqual = false;
                            isKeyShow = false;

                            int i = 0;
                            for (i = 0; i < rt_GetImageDescriptionAfterSet.MultiResults[0].Parameters.Count; i++)
                            {
                                if (psNode.ParameterName == rt_GetImageDescriptionAfterSet.MultiResults[0].Parameters[i].ParameterName)
                                {
                                    isKeyShow = true;
                                    isValueEqual = string.Equals(psNode.ParameterValue, rt_GetImageDescriptionAfterSet.MultiResults[0].Parameters[i].ParameterValue);
                                    break; // End current for loop to search node
                                }
                            }

                            if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                            {
                                cp_GetImageDescriptionAfterSet.Result = TestResult.Fail;

                                if (isKeyShow)
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageDescription does not match the expected.");
                                    cp_GetImageDescriptionAfterSet.Outputs.AddParameter("ImageDescription", "Check the values in getImageDescription return after set image", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetImageDescriptionAfterSet.MultiResults[0].Parameters[i].ParameterValue);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Print("The return value in getImageDescription does not contain the node: " + psNode.ParameterName);
                                    cp_GetImageDescriptionAfterSet.Outputs.AddParameter("ImageDescription", "Check the values in getImageDescription return after set image", "The return value in getImageDescription does not contain the node: " + psNode.ParameterName);
                                }
                                break; // End current foreach loop, not compare the follwing nodes
                            }
                        }

                        if (isValueEqual)
                        {
                            cp_GetImageDescriptionAfterSet.Result = TestResult.Pass;

                            System.Diagnostics.Debug.Print("The return values in getImageDescription all match the expected.");
                            cp_GetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Get image Description after set returns success", rt_GetImageDescriptionAfterSet.Message);
                            cp_GetImageDescriptionAfterSet.Outputs.AddParameter("ImageDescription", "Check the values in getImageDescription return after set", "The return values in getImageDescription all match the expected");
                        }
                    }
                    #endregion

                CLEANUP:
                    #region Step 7: Call ImageService.deleteImage to delete the image
                    CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                    r.CheckPoints.Add(cp_DeleteImage);

                    XMLResult rt_DeleteImage = imageService.deleteImage(imageinternalUid, new XMLParameter("preferences"));
                    if (rt_DeleteImage.IsErrorOccured)
                    {
                        cp_DeleteImage.Result = TestResult.Fail;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        cp_DeleteImage.Result = TestResult.Pass;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_Image_SetInfo_Error_Case1564()  // Case 1564: added for calling setImageInfo to save archived image, SetImageInfo_ArchivedImage_Error_WrongArchivePath
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string patientInternalId = string.Empty;
                    string objectFileFullPath = string.Empty;
                    string archivedPath = string.Empty;

                    XMLParameter pSetImageInfo = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "importImage")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            {
                                patientInternalId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivedPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo")
                        {
                            pSetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    #region Step 1: Call ImportService.importObject to import a image
                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;
                    ImportService importService = new ImportService();

                    CheckPoint cpImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import an archived  image");
                    r.CheckPoints.Add(cpImportImage);

                    XMLResult rtImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, archivedPath, true, "false");
                    if (rtImportImage.IsErrorOccured)
                    {
                        cpImportImage.Result = TestResult.Fail;
                        cpImportImage.Outputs.AddParameter("Import image returns error", "Import image", rtImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportImage.Result = TestResult.Pass;
                        cpImportImage.Outputs.AddParameter("Import image returns success", "Import image", rtImportImage.Message);

                        imageInternalID = rtImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rtImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    ImageService imageService = new ImageService();

                    #region Step 2-1: Check the getImageDescription return is correct after import image
                    CheckPoint cpGetImageDescriptionAfterImport = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after import image");
                    r.CheckPoints.Add(cpGetImageDescriptionAfterImport);

                    XMLResult rtGetImageDescriptionAfterImport = imageService.getImageDescription(imageInternalID);
                    string imagePathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("path");
                    string archivedPathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("archive_path");
                    string archiveTagAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport) && archivedPathAfterImport == archivedPath && string.IsNullOrEmpty(archiveTagAfterImport))
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Pass;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after import image");
                    }
                    else
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Fail;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after import image. Actually get: " + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    #endregion

                    #region Step 2-2: Check the info is correct in getImageInfo return after import image
                    CheckPoint cpGetImageInfoAfterImport = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not import the image");
                    r.CheckPoints.Add(cpGetImageInfoAfterImport);

                    XMLParameter pGetImageInfoAfterImport = new XMLParameter("image");
                    pGetImageInfoAfterImport.AddParameter("internal_id", imageInternalID);
                    XMLResult rtGetImageInfo = imageService.getImageInfo(pGetImageInfoAfterImport);

                    imagePathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("tags");

                    if (imagePathAfterImport == string.Empty) // && archivedPathAfterImport == archivedPath && archiveTagAfterImport == "archived", will be supported in new service
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Pass;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after import image");
                    }
                    else
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Fail;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after import image. Actually get: " + rtGetImageInfo.ResultContent);
                    }
                    #endregion

                    #region Step 3-1: Call imageService.SetImageInfo to save the archived image but the archive path is wrong
                    CheckPoint cpSetImageInfowithError = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image");
                    r.CheckPoints.Add(cpSetImageInfowithError);

                    XMLResult rtSetImageInfowithError = imageService.setImageInfo(pSetImageInfo, imageInternalID);

                    if (!rtSetImageInfowithError.IsErrorOccured)
                    {
                        cpSetImageInfowithError.Result = TestResult.Fail;
                        cpSetImageInfowithError.Outputs.AddParameter("SetImageInfo returns success unexpectly", "SetImageInfo", rtSetImageInfowithError.Message);
                    }
                    else
                    {
                        cpSetImageInfowithError.Result = TestResult.Pass;
                        cpSetImageInfowithError.Outputs.AddParameter("SetImageInfo returns error as expected", "SetImageInfo", rtSetImageInfowithError.Message);

                        // Below to check the info is correct after setImageInfo. Check points includes:
                        //1. The Image ID and PS ID are not changed
                        //2. GetImageDescription return correct image path and archive path, correct archive flag
                        //3. GetImageInfo return correct image path, correct archive flag
                        //4. Use the image path to check the image file is in server DB

                        #region Step 4: Check the ps is not change
                        CheckPoint cpPSID = new CheckPoint("Check PS ID", "Check the PS ID is not changed after call setImageInfo");
                        r.CheckPoints.Add(cpPSID);

                        XMLParameter pListPS = new XMLParameter("filter");
                        pListPS.AddParameter("current", "true");
                        string currentPSIDAfterSet = imageService.listPresentationState(pListPS, imageInternalID).SingleResult; // may need change

                        if (currentPSIDAfterSet == currentPSID)
                        {
                            cpPSID.Result = TestResult.Pass;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is not changed");
                        }
                        else
                        {
                            cpPSID.Result = TestResult.Fail;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is changed. Expect: " + currentPSID + "; Actually new PS ID: " + currentPSIDAfterSet);
                        }
                        #endregion

                        string imagePathAfterSet = string.Empty;
                        string archivePathAfterSet = string.Empty;
                        string archiveTagAfterSet = string.Empty;

                        #region Step 5: Check the getImageDescription return is correct after set image
                        CheckPoint cpGetImageDescriptionAfterSet = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageDescriptionAfterSet);

                        XMLResult rtGetImageDescriptionAfterSet = imageService.getImageDescription(imageInternalID);
                        imagePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("tags");

                        if (string.IsNullOrEmpty(imagePathAfterSet) && archivePathAfterSet == archivedPath && string.IsNullOrEmpty(archiveTagAfterSet))
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Pass;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image");
                        }
                        else
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Fail;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image. Actually get: " + rtGetImageDescriptionAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 6: Check the getImageinfo return is correct after set image
                        CheckPoint cpGetImageInfoAfterSet = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageInfoAfterSet);

                        XMLParameter pGetImageInfoAfterSet = new XMLParameter("image");
                        pGetImageInfoAfterSet.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoAfterSet = imageService.getImageInfo(pGetImageInfoAfterSet);

                        imagePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("tags");

                        if (string.IsNullOrEmpty(imagePathAfterSet)) //&& archivePathAfterSet == archivedPath && archiveTagAfterSet =="archived", will be supported in new service
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Pass;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image");
                        }
                        else
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Fail;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image. Actually get: " + rtGetImageInfoAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 7: Check the file NOT exist in server DB
                        CheckPoint cpImageInDB = new CheckPoint("Check file in DB", "Check the file exist in server DB after call setImageInfo");
                        r.CheckPoints.Add(cpImageInDB);

                        if (Utility.IsImageExistInServerDB(imagePathAfterSet))
                        {
                            cpImageInDB.Result = TestResult.Fail;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File still exist");
                        }
                        else
                        {
                            cpImageInDB.Result = TestResult.Pass;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File NOT exist as expected");
                        }
                        #endregion

                        #region Step 8: Check the original archived image is not deleted
                        CheckPoint cpOriginalArchivedImage = new CheckPoint("Check original archived file", "Check the original archived image is not deleted after call setImageInfo");
                        r.CheckPoints.Add(cpOriginalArchivedImage);

                        if (System.IO.File.Exists(archivedPath))
                        {
                            cpOriginalArchivedImage.Result = TestResult.Pass;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File exist");
                        }
                        else
                        {
                            cpOriginalArchivedImage.Result = TestResult.Fail;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File NOT exist");
                        }
                        #endregion
                    }
                    #endregion

                    #region Step 9: Call ImageService.deleteImage to delete the created image
                    CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                    r.CheckPoints.Add(cp_DeleteImage);

                    XMLResult rt_DeleteImage = imageService.deleteImage(imageInternalID, new XMLParameter("preferences"));
                    if (rt_DeleteImage.IsErrorOccured)
                    {
                        cp_DeleteImage.Result = TestResult.Fail;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);
                    }
                    else
                    {
                        cp_DeleteImage.Result = TestResult.Pass;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_Image_SetInfo_WorkFlow_Case1562()  // Case 1562: 1.1.04.05 SetImage_WorkFlow_02_Call setImage when archive path is NOT accessible and then call again when its  accessible
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string patientInternalId = string.Empty;
                    string objectFileFullPath = string.Empty;
                    string archivedPath = string.Empty;

                    XMLParameter pSetImageInfo = new XMLParameter("image");
                    XMLParameter pSetImageInfoWithError = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "importImage")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            {
                                patientInternalId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivedPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo_error")
                        {
                            pSetImageInfoWithError.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo_correct")
                        {
                            pSetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    #region Step 1: Call ImportService.importObject to import a image
                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;
                    ImportService importService = new ImportService();

                    CheckPoint cpImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import an archived  image");
                    r.CheckPoints.Add(cpImportImage);

                    XMLResult rtImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, archivedPath, true, "false");
                    if (rtImportImage.IsErrorOccured)
                    {
                        cpImportImage.Result = TestResult.Fail;
                        cpImportImage.Outputs.AddParameter("Import image returns error", "Import image", rtImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportImage.Result = TestResult.Pass;
                        cpImportImage.Outputs.AddParameter("Import image returns success", "Import image", rtImportImage.Message);

                        imageInternalID = rtImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rtImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    ImageService imageService = new ImageService();

                    #region Step 2-1: Check the getImageDescription return is correct after import image
                    CheckPoint cpGetImageDescriptionAfterImport = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after import image");
                    r.CheckPoints.Add(cpGetImageDescriptionAfterImport);

                    XMLResult rtGetImageDescriptionAfterImport = imageService.getImageDescription(imageInternalID);
                    string imagePathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("path");
                    string archivedPathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("archive_path");
                    string archiveTagAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("tags");

                    if (imagePathAfterImport == string.Empty && archivedPathAfterImport == archivedPath && string.IsNullOrEmpty(archiveTagAfterImport))
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Pass;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after import image");
                    }
                    else
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Fail;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after import image. Actually get: " + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    #endregion

                    #region Step 2-2: Check the info is correct in getImageInfo return after import image
                    CheckPoint cpGetImageInfoAfterImport = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not import the image");
                    r.CheckPoints.Add(cpGetImageInfoAfterImport);

                    XMLParameter pGetImageInfoAfterImport = new XMLParameter("image");
                    pGetImageInfoAfterImport.AddParameter("internal_id", imageInternalID);
                    XMLResult rtGetImageInfo = imageService.getImageInfo(pGetImageInfoAfterImport);

                    imagePathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport) && string.IsNullOrEmpty(archivedPathAfterImport) && string.IsNullOrEmpty(archiveTagAfterImport))  // archivedPathAfterImport == archivedPath, will be supported in new service
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Pass;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after import image");
                    }
                    else
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Fail;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after import image. Actually get: " + rtGetImageInfo.ResultContent);
                    }
                    #endregion

                    #region Step 3-1: Call imageService.SetImageInfo to save the archived image but the archive path is wrong
                    CheckPoint cpSetImageInfowithError = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image, with wrong path");
                    r.CheckPoints.Add(cpSetImageInfowithError);

                    XMLResult rtSetImageInfowithError = imageService.setImageInfo(pSetImageInfoWithError, imageInternalID);

                    if (rtSetImageInfowithError.IsErrorOccured)
                    {
                        cpSetImageInfowithError.Result = TestResult.Pass;
                        cpSetImageInfowithError.Outputs.AddParameter("SetImageInfo returns error as expected", "SetImageInfo", rtSetImageInfowithError.Message);
                    }
                    else
                    {
                        cpSetImageInfowithError.Result = TestResult.Fail;
                        cpSetImageInfowithError.Outputs.AddParameter("SetImageInfo returns success unexpectly", "SetImageInfo", rtSetImageInfowithError.Message);
                    }
                    #endregion

                    System.Threading.Thread.Sleep(1000);

                    #region Step 3-2: Call ImageService.SetImageInfo to save the archived image as normal image again
                    CheckPoint cpSetImageInfo = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image again, with correct path");
                    r.CheckPoints.Add(cpSetImageInfo);

                    XMLResult rtSetImageInfo = imageService.setImageInfo(pSetImageInfo, imageInternalID);

                    if (rtSetImageInfo.IsErrorOccured)
                    {
                        cpSetImageInfo.Result = TestResult.Fail;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns error", "SetImageInfo", rtSetImageInfo.Message);
                    }
                    else
                    {
                        cpSetImageInfo.Result = TestResult.Pass;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns success", "SetImageInfo", rtSetImageInfo.Message);

                        // Wait the file is transferred to server DB
                        System.Threading.Thread.Sleep(3000);

                        // Below to check the info is correct after setImageInfo. Check points includes:
                        //1. The Image ID and PS ID are not changed
                        //2. GetImageDescription return correct image path and archive path, correct archive flag
                        //3. GetImageInfo return correct image path, correct archive flag
                        //4. Use the image path to check the image file is in server DB
                        //5. The original archived image is not deleted

                        #region Step 4: Check the ps is set, not newly created after set image
                        CheckPoint cpPSID = new CheckPoint("Check PS ID", "Check the PS ID is not changed after call setImageInfo");
                        r.CheckPoints.Add(cpPSID);

                        XMLParameter pListPS = new XMLParameter("filter");
                        pListPS.AddParameter("current", "true");
                        string currentPSIDAfterSet = imageService.listPresentationState(pListPS, imageInternalID).SingleResult; // may need change

                        if (currentPSIDAfterSet == currentPSID)
                        {
                            cpPSID.Result = TestResult.Pass;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is not changed");
                        }
                        else
                        {
                            cpPSID.Result = TestResult.Fail;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is changed. Expect: " + currentPSID + "; Actually new PS ID: " + currentPSIDAfterSet);
                        }
                        #endregion

                        string imagePathAfterSet = string.Empty;
                        string archivePathAfterSet = string.Empty;
                        string archiveTagAfterSet = string.Empty;

                        #region Step 5: Check the getImageDescription return is correct after set image
                        CheckPoint cpGetImageDescriptionAfterSet = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageDescriptionAfterSet);

                        XMLResult rtGetImageDescriptionAfterSet = imageService.getImageDescription(imageInternalID);
                        imagePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archiveTagAfterSet)) // imagePathAfterSet sample: C:/Documents and Settings/All Users/Application Data/TW/PAS/pas_data/patient/03/8af0a7e63310cc65013310d46d0e0003/1956bc28-ca5e-4720-a857-d4de18fc1479/02962f27-e4be-4d59-b112-9663a2f2572b.dcm"
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Pass;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image");
                        }
                        else
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Fail;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image. Actually get: " + rtGetImageDescriptionAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 6: Check the getImageinfo return is correct after set image
                        CheckPoint cpGetImageInfoAfterSet = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageInfoAfterSet);

                        XMLParameter pGetImageInfoAfterSet = new XMLParameter("image");
                        pGetImageInfoAfterSet.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoAfterSet = imageService.getImageInfo(pGetImageInfoAfterSet);

                        imagePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archiveTagAfterSet))
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Pass;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image");
                        }
                        else
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Fail;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image. Actually get: " + rtGetImageInfoAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 7: Check the file exist in server DB
                        CheckPoint cpImageInDB = new CheckPoint("Check file in DB", "Check the file exist in server DB after call setImageInfo");
                        r.CheckPoints.Add(cpImageInDB);

                        if (Utility.IsImageExistInServerDB(imagePathAfterSet))
                        {
                            cpImageInDB.Result = TestResult.Pass;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File exist");
                        }
                        else
                        {
                            cpImageInDB.Result = TestResult.Fail;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File NOT exist");
                        }
                        #endregion

                        #region Step 8: Check the original archived image is not deleted
                        CheckPoint cpOriginalArchivedImage = new CheckPoint("Check original archived file", "Check the original archived image is not deleted after call setImageInfo");
                        r.CheckPoints.Add(cpOriginalArchivedImage);

                        if (System.IO.File.Exists(archivedPath))
                        {
                            cpOriginalArchivedImage.Result = TestResult.Pass;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File exist");
                        }
                        else
                        {
                            cpOriginalArchivedImage.Result = TestResult.Fail;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File NOT exist");
                        }
                        #endregion
                    }
                    #endregion

                    #region Step 9: Call ImageService.deleteImage to delete the created image
                    CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                    r.CheckPoints.Add(cp_DeleteImage);

                    XMLResult rt_DeleteImage = imageService.deleteImage(imageInternalID, new XMLParameter("preferences"));
                    if (rt_DeleteImage.IsErrorOccured)
                    {
                        cp_DeleteImage.Result = TestResult.Fail;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);
                    }
                    else
                    {
                        cp_DeleteImage.Result = TestResult.Pass;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_Image_SetInfo_WorkFlow_Case1561()  // Case 1561: 1.1.04.04 SetImage_WorkFlow_01_Call setImage to archived image and then call deleteImage to delete
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string patientInternalId = string.Empty;
                    string objectFileFullPath = string.Empty;
                    string archivedPath = string.Empty;

                    XMLParameter pSetImageInfo = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "importImage")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            {
                                patientInternalId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivedPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo")
                        {
                            pSetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    #region Step 1: Call ImportService.importObject to import a image
                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;
                    ImportService importService = new ImportService();

                    CheckPoint cpImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import an archived  image");
                    r.CheckPoints.Add(cpImportImage);

                    XMLResult rtImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, archivedPath, true, "false");
                    if (rtImportImage.IsErrorOccured)
                    {
                        cpImportImage.Result = TestResult.Fail;
                        cpImportImage.Outputs.AddParameter("Import image returns error", "Import image", rtImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportImage.Result = TestResult.Pass;
                        cpImportImage.Outputs.AddParameter("Import image returns success", "Import image", rtImportImage.Message);

                        imageInternalID = rtImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rtImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    ImageService imageService = new ImageService();

                    #region Step 2-1: Check the getImageDescription return is correct after import image
                    CheckPoint cpGetImageDescriptionAfterImport = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after import image");
                    r.CheckPoints.Add(cpGetImageDescriptionAfterImport);

                    XMLResult rtGetImageDescriptionAfterImport = imageService.getImageDescription(imageInternalID);
                    string imagePathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("path");
                    string archivedPathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("archive_path");
                    string archiveFlagAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport) && archivedPathAfterImport == archivedPath && string.IsNullOrEmpty(archiveFlagAfterImport))
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Pass;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after import image");
                    }
                    else
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Fail;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after import image. Actually get: " + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    #endregion

                    #region Step 2-2: Check the info is correct in getImageInfo return after import image
                    CheckPoint cpGetImageInfoAfterImport = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not import the image");
                    r.CheckPoints.Add(cpGetImageInfoAfterImport);

                    XMLParameter pGetImageInfoAfterImport = new XMLParameter("image");
                    pGetImageInfoAfterImport.AddParameter("internal_id", imageInternalID);
                    XMLResult rtGetImageInfo = imageService.getImageInfo(pGetImageInfoAfterImport);

                    imagePathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("archive_path");
                    archiveFlagAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport))  //&& archiveFlagAfterImport == "archived" && archivedPathAfterImport == archivedPath, will be supported in new service
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Pass;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after import image");
                    }
                    else
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Fail;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after import image. Actually get: " + rtGetImageInfo.ResultContent);
                    }
                    #endregion

                    #region Step 3: Call ImageService.SetImageInfo to save the archived image as normal image
                    CheckPoint cpSetImageInfo = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image");
                    r.CheckPoints.Add(cpSetImageInfo);

                    XMLResult rtSetImageInfo = imageService.setImageInfo(pSetImageInfo, imageInternalID);

                    string imagePathAfterSet = string.Empty;
                    string archivePathAfterSet = string.Empty;
                    string archiveTagAfterSet = string.Empty;

                    if (rtSetImageInfo.IsErrorOccured)
                    {
                        cpSetImageInfo.Result = TestResult.Fail;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns error", "SetImageInfo", rtSetImageInfo.Message);
                    }
                    else
                    {
                        cpSetImageInfo.Result = TestResult.Pass;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns success", "SetImageInfo", rtSetImageInfo.Message);

                        // Wait the file is transferred to server DB
                        System.Threading.Thread.Sleep(3000);

                        // Below to check the info is correct after setImageInfo. Check points includes:
                        //1. The Image ID and PS ID are not changed
                        //2. GetImageDescription return correct image path and archive path, correct archive flag
                        //3. GetImageInfo return correct image path, correct archive flag
                        //4. Use the image path to check the image file is in server DB
                        //5. The original archived image is not deleted

                        #region Step 4: Check the ps is set, not newly created after set image
                        CheckPoint cpPSID = new CheckPoint("Check PS ID", "Check the PS ID is not changed after call setImageInfo");
                        r.CheckPoints.Add(cpPSID);

                        XMLParameter pListPS = new XMLParameter("filter");
                        pListPS.AddParameter("current", "true");
                        string currentPSIDAfterSet = imageService.listPresentationState(pListPS, imageInternalID).SingleResult; // may need change

                        if (currentPSIDAfterSet == currentPSID)
                        {
                            cpPSID.Result = TestResult.Pass;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is not changed");
                        }
                        else
                        {
                            cpPSID.Result = TestResult.Fail;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is changed. Expect: " + currentPSID + "; Actually new PS ID: " + currentPSIDAfterSet);
                        }
                        #endregion

                        #region Step 5: Check the getImageDescription return is correct after set image
                        CheckPoint cpGetImageDescriptionAfterSet = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageDescriptionAfterSet);

                        XMLResult rtGetImageDescriptionAfterSet = imageService.getImageDescription(imageInternalID);
                        imagePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archiveTagAfterSet)) // imagePathAfterSet sample: C:/Documents and Settings/All Users/Application Data/TW/PAS/pas_data/patient/03/8af0a7e63310cc65013310d46d0e0003/1956bc28-ca5e-4720-a857-d4de18fc1479/02962f27-e4be-4d59-b112-9663a2f2572b.dcm"
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Pass;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image");
                        }
                        else
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Fail;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image. Actually get: " + rtGetImageDescriptionAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 6: Check the getImageinfo return is correct after set image
                        CheckPoint cpGetImageInfoAfterSet = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageInfoAfterSet);

                        XMLParameter pGetImageInfoAfterSet = new XMLParameter("image");
                        pGetImageInfoAfterSet.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoAfterSet = imageService.getImageInfo(pGetImageInfoAfterSet);

                        imagePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archiveTagAfterSet))
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Pass;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image");
                        }
                        else
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Fail;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image. Actually get: " + rtGetImageInfoAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 7: Check the file exist in server DB
                        CheckPoint cpImageInDB = new CheckPoint("Check file in DB", "Check the file exist in server DB after call setImageInfo");
                        r.CheckPoints.Add(cpImageInDB);

                        if (Utility.IsImageExistInServerDB(imagePathAfterSet))
                        {
                            cpImageInDB.Result = TestResult.Pass;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File exist");
                        }
                        else
                        {
                            cpImageInDB.Result = TestResult.Fail;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File NOT exist");
                        }
                        #endregion

                        #region Step 8: Check the original archived image is not deleted
                        CheckPoint cpOriginalArchivedImage = new CheckPoint("Check original archived file", "Check the original archived image is not deleted after call setImageInfo");
                        r.CheckPoints.Add(cpOriginalArchivedImage);

                        if (System.IO.File.Exists(archivedPath))
                        {
                            cpOriginalArchivedImage.Result = TestResult.Pass;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File exist");
                        }
                        else
                        {
                            cpOriginalArchivedImage.Result = TestResult.Fail;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File NOT exist");
                        }
                        #endregion
                    }
                    #endregion

                    #region Step 9: Call ImageService.deleteImage to delete the created image
                    CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                    r.CheckPoints.Add(cp_DeleteImage);

                    XMLResult rt_DeleteImage = imageService.deleteImage(imageInternalID, new XMLParameter("preferences"));
                    if (rt_DeleteImage.IsErrorOccured)
                    {
                        cp_DeleteImage.Result = TestResult.Fail;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);
                    }
                    else
                    {
                        cp_DeleteImage.Result = TestResult.Pass;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
                    }
                    #endregion

                    #region Step : Check the image is not exist
                    CheckPoint cpIsImageDeleted = new CheckPoint("Delete Image", "Check the image is really deleted");
                    r.CheckPoints.Add(cpIsImageDeleted);

                    XMLResult rtGetImageDescriptionAfterDelete = imageService.getImageDescription(imageInternalID);

                    if (rtGetImageDescriptionAfterDelete.Message.Contains("image not found"))
                    {
                        cpIsImageDeleted.Result = TestResult.Pass;
                        cpIsImageDeleted.Outputs.AddParameter("Delete Image", "Check the Image is deleted or not", "The image is really deleted");
                    }
                    else
                    {
                        cpIsImageDeleted.Result = TestResult.Fail;
                        cpIsImageDeleted.Outputs.AddParameter("Delete Image", "Check the Image is deleted or not", "The image is not successfully deleted. GetImageDescription returns:" + rtGetImageDescriptionAfterDelete.ResultContent);
                    }
                    #endregion

                    // Note: 2012-11-21, per the design change from Sprint 7, the file should not be deleted immediately when delete the instance. Will add a job to handle the file delete seperately when CSDM restarts
                    #region Step 10: Check the image is NOT deleted from server patient folder.
                    CheckPoint cpIsImageInDBAfterDelete = new CheckPoint("Delete Image", "Check the image file is NOT deleted from server Patient folder");
                    r.CheckPoints.Add(cpIsImageInDBAfterDelete);

                    if (Utility.IsImageExistInServerDB(imagePathAfterSet)) // File exit in server Patient folder
                    {
                        cpIsImageInDBAfterDelete.Result = TestResult.Pass;
                        cpIsImageInDBAfterDelete.Outputs.AddParameter("Delete Image", "Check the image file is deleted or not", "The image file is NOT deleted from server file system");
                    }
                    else
                    {
                        cpIsImageInDBAfterDelete.Result = TestResult.Fail;
                        cpIsImageInDBAfterDelete.Outputs.AddParameter("Delete Image", "Check the image file is deleted or not", "The image file is wrongly deleted from server file system");
                    }
                    #endregion


                    #region Step 11: Check the original archive image is not deleted
                    CheckPoint cpIsOriginalImageExistAfterDelete = new CheckPoint("Delete Image", "Check the original archive image file is deleted or not.");
                    r.CheckPoints.Add(cpIsOriginalImageExistAfterDelete);

                    if (System.IO.File.Exists(archivedPath))
                    {
                        cpIsOriginalImageExistAfterDelete.Result = TestResult.Pass;
                        cpIsOriginalImageExistAfterDelete.Outputs.AddParameter("Delete Image", "Check the original archive image file is deleted or not", "The original archive image file  is not deleted as expected");
                    }
                    else
                    {
                        cpIsOriginalImageExistAfterDelete.Result = TestResult.Fail;
                        cpIsOriginalImageExistAfterDelete.Outputs.AddParameter("Delete Image", "Check the original archive image file is deleted or not", "The original archive image file  is deleted wrongly");
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

        public void Run_Image_SetInfo_ImageInFMS_Case1560()  // Case 1560:  1.1.04.03 SetImage_N03_Call setImage for archived image in FMS
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string patientInternalId = string.Empty;
                    string fmsFullPath = string.Empty;
                    string objectFileFullPath = string.Empty;
                    string archivedPath = string.Empty;

                    XMLParameter pSetImageInfo = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "importFMS")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patient_internal_id")
                            {
                                patientInternalId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                fmsFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "importImage")
                        {

                            if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivedPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo")
                        {
                            pSetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    ImportService importService = new ImportService();
                    FMSService fmsSvc = new FMSService();
                    ImageService imageService = new ImageService();


                    #region Step 1-1: Call ImportService.importObject to import FMS
                    string fmsID = string.Empty;

                    CheckPoint cpImportFMS = new CheckPoint("Import Image", "Call ImportService.importObject to import a FMS");
                    r.CheckPoints.Add(cpImportFMS);

                    XMLResult rtImportFMS = importService.importObject(patientInternalId, null, fmsFullPath, null, true, "false");
                    if (rtImportFMS.IsErrorOccured)
                    {
                        cpImportFMS.Result = TestResult.Fail;
                        cpImportFMS.Outputs.AddParameter("Import image returns error", "Import image", rtImportFMS.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportFMS.Result = TestResult.Pass;
                        cpImportFMS.Outputs.AddParameter("Import image returns success", "Import image", rtImportFMS.Message);

                        foreach (XMLParameter p in rtImportFMS.MultiResults)
                        {
                            if (p.Name == "fms")
                            {
                                fmsID = p.Parameters[0].ParameterValue;
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Step 1-2: Call ImportService.importObject to import a image
                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;

                    CheckPoint cpImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import an archived  image");
                    r.CheckPoints.Add(cpImportImage);

                    XMLResult rtImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, archivedPath, true, "false");
                    if (rtImportImage.IsErrorOccured)
                    {
                        cpImportImage.Result = TestResult.Fail;
                        cpImportImage.Outputs.AddParameter("Import image returns error", "Import image", rtImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportImage.Result = TestResult.Pass;
                        cpImportImage.Outputs.AddParameter("Import image returns success", "Import image", rtImportImage.Message);

                        imageInternalID = rtImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rtImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    #region Step 1-3: Add image into FMS
                    CheckPoint cpSetFMS = new CheckPoint("Set FMS", " Call setFMSDescription to add the archived image into it");
                    r.CheckPoints.Add(cpImportFMS);


                    XMLParameterCollection pSetFMSDescription = new XMLParameterCollection();

                    XMLParameter pFMSDes = new XMLParameter("fms");
                    pFMSDes.AddParameter("xml_description", "");

                    XMLParameter pFMSPS = new XMLParameter("presentationstate");
                    foreach (XMLParameter p in rtImportFMS.MultiResults) //Add original PS
                    {
                        if (p.Name == "presentationstate")
                        {
                            pFMSPS.AddParameter("internal_id", p.Parameters[0].ParameterValue);
                        }
                    }
                    pFMSPS.AddParameter("internal_id", currentPSID);

                    pSetFMSDescription.Add(pFMSDes);
                    pSetFMSDescription.Add(pFMSPS);

                    XMLResult rtSetFMS = fmsSvc.setFMSDescription(pSetFMSDescription, fmsID);

                    if (rtSetFMS.IsErrorOccured)
                    {
                        cpSetFMS.Result = TestResult.Fail;
                        cpSetFMS.Outputs.AddParameter("Set FMS", "setFMSDescription return error: ", rtSetFMS.ResultContent);
                    }
                    else
                    {
                        cpSetFMS.Result = TestResult.Pass;
                    }
                    #endregion

                    #region Step 2-1: Check the getImageDescription return is correct after import image
                    CheckPoint cpGetImageDescriptionAfterImport = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after import image");
                    r.CheckPoints.Add(cpGetImageDescriptionAfterImport);

                    XMLResult rtGetImageDescriptionAfterImport = imageService.getImageDescription(imageInternalID);
                    string imagePathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("path");
                    string archivedPathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("archive_path");
                    string archiveTagAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("tags");

                    if (imagePathAfterImport == string.Empty && archivedPathAfterImport == archivedPath && string.IsNullOrEmpty(archiveTagAfterImport))
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Pass;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after import image");
                    }
                    else
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Fail;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after import image. Actually get: " + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    #endregion

                    #region Step 2-2: Check the info is correct in getImageInfo return after import image
                    CheckPoint cpGetImageInfoAfterImport = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not import the image");
                    r.CheckPoints.Add(cpGetImageInfoAfterImport);

                    XMLParameter pGetImageInfoAfterImport = new XMLParameter("image");
                    pGetImageInfoAfterImport.AddParameter("internal_id", imageInternalID);
                    XMLResult rtGetImageInfo = imageService.getImageInfo(pGetImageInfoAfterImport);

                    imagePathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport)) //&& archivedPathAfterImport == archivedPath && archiveTagAfterImport == "archived", will be supported in new service
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Pass;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after import image");
                    }
                    else
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Fail;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after import image. Actually get: " + rtGetImageInfo.ResultContent);
                    }
                    #endregion

                    #region Step 3: Call ImageService.SetImageInfo to save the archived image as normal image
                    CheckPoint cpSetImageInfo = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image");
                    r.CheckPoints.Add(cpSetImageInfo);

                    XMLResult rtSetImageInfo = imageService.setImageInfo(pSetImageInfo, imageInternalID);

                    if (rtSetImageInfo.IsErrorOccured)
                    {
                        cpSetImageInfo.Result = TestResult.Fail;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns error", "SetImageInfo", rtSetImageInfo.Message);
                    }
                    else
                    {
                        cpSetImageInfo.Result = TestResult.Pass;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns success", "SetImageInfo", rtSetImageInfo.Message);

                        // Wait the file is transferred to server DB
                        System.Threading.Thread.Sleep(3000);

                        // Below to check the info is correct after setImageInfo. Check points includes:
                        //1. The Image ID and PS ID are not changed
                        //2. GetImageDescription return correct image path and archive path, correct archive flag
                        //3. GetImageInfo return correct image path, correct archive flag
                        //4. Use the image path to check the image file is in server DB
                        //5. The original archived image is not deleted

                        //#region Step 3: Check the image is set, not newly created
                        ////PatientService patientSvc = new PatientService();
                        ////patientSvc.listObjects();
                        ////NewPatientService newPatientSvc = new NewPatientService();
                        ////newPatientSvc.listObjects();
                        //#endregion

                        #region Step 4: Check the ps is set, not newly created after set image
                        CheckPoint cpPSID = new CheckPoint("Check PS ID", "Check the PS ID is not changed after call setImageInfo");
                        r.CheckPoints.Add(cpPSID);

                        XMLParameter pListPS = new XMLParameter("filter");
                        pListPS.AddParameter("current", "true");
                        string currentPSIDAfterSet = imageService.listPresentationState(pListPS, imageInternalID).SingleResult; // may need change

                        if (currentPSIDAfterSet == currentPSID)
                        {
                            cpPSID.Result = TestResult.Pass;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is not changed");
                        }
                        else
                        {
                            cpPSID.Result = TestResult.Fail;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is changed. Expect: " + currentPSID + "; Actually new PS ID: " + currentPSIDAfterSet);
                        }
                        #endregion

                        string imagePathAfterSet = string.Empty;
                        string archivePathAfterSet = string.Empty;
                        string archiveTagAfterSet = string.Empty;

                        #region Step 5: Check the getImageDescription return is correct after set image
                        CheckPoint cpGetImageDescriptionAfterSet = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageDescriptionAfterSet);

                        XMLResult rtGetImageDescriptionAfterSet = imageService.getImageDescription(imageInternalID);
                        imagePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archiveTagAfterSet)) // imagePathAfterSet sampe: C:/Documents and Settings/All Users/Application Data/TW/PAS/pas_data/patient/03/8af0a7e63310cc65013310d46d0e0003/1956bc28-ca5e-4720-a857-d4de18fc1479/02962f27-e4be-4d59-b112-9663a2f2572b.dcm"
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Pass;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image");
                        }
                        else
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Fail;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image. Actually get: " + rtGetImageDescriptionAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 6: Check the getImageinfo return is correct after set image
                        CheckPoint cpGetImageInfoAfterSet = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageInfoAfterSet);

                        XMLParameter pGetImageInfoAfterSet = new XMLParameter("image");
                        pGetImageInfoAfterSet.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoAfterSet = imageService.getImageInfo(pGetImageInfoAfterSet);

                        imagePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("archive_path");
                        archiveTagAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archiveTagAfterSet))
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Pass;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image");
                        }
                        else
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Fail;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image. Actually get: " + rtGetImageInfoAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 7: Check the file exist in server DB
                        CheckPoint cpImageInDB = new CheckPoint("Check file in DB", "Check the file exist in server DB after call setImageInfo");
                        r.CheckPoints.Add(cpImageInDB);

                        if (Utility.IsImageExistInServerDB(imagePathAfterSet))
                        {
                            cpImageInDB.Result = TestResult.Pass;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File exist");
                        }
                        else
                        {
                            cpImageInDB.Result = TestResult.Fail;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File NOT exist");
                        }
                        #endregion

                        #region Step 8: Check the original archived image is not deleted
                        CheckPoint cpOriginalArchivedImage = new CheckPoint("Check original archived file", "Check the original archived image is not deleted after call setImageInfo");
                        r.CheckPoints.Add(cpOriginalArchivedImage);

                        if (System.IO.File.Exists(archivedPath))
                        {
                            cpOriginalArchivedImage.Result = TestResult.Pass;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File exist");
                        }
                        else
                        {
                            cpOriginalArchivedImage.Result = TestResult.Fail;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File NOT exist");
                        }
                        #endregion
                    }
                    #endregion


                    #region Step 9: Call ImageService.deleteImage to delete the created image
                    CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                    r.CheckPoints.Add(cp_DeleteImage);

                    XMLResult rt_DeleteImage = imageService.deleteImage(imageInternalID, new XMLParameter("preferences"));
                    if (rt_DeleteImage.IsErrorOccured)
                    {
                        cp_DeleteImage.Result = TestResult.Fail;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);
                    }
                    else
                    {
                        cp_DeleteImage.Result = TestResult.Pass;
                        cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
                    }
                    #endregion

                    #region Step 10: Call FMSService.deleteFMS to delete the FMS
                    CheckPoint cp_DeleteFMS = new CheckPoint("Delete FMS", "Call fmsService.deleteFMS to delete the FMS");
                    r.CheckPoints.Add(cp_DeleteFMS);

                    XMLResult rt_DeleteFMS = fmsSvc.deleteFMS("true", fmsID);
                    if (rt_DeleteFMS.IsErrorOccured)
                    {
                        cp_DeleteFMS.Result = TestResult.Fail;
                        cp_DeleteFMS.Outputs.AddParameter("delete fms", "Delete fms returns error", rt_DeleteFMS.Message);
                    }
                    else
                    {
                        cp_DeleteFMS.Result = TestResult.Pass;
                        cp_DeleteFMS.Outputs.AddParameter("delete fms", "Delete fms returns success", rt_DeleteFMS.Message);
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

        public void Run_Image_SetInfo_LargeImage_Case1559()  // Case 1559: 1.1.04.02 SetImage_N02_Call setImage for very large archived image
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string patientInternalId = string.Empty;
                    string objectFileFullPath = string.Empty;
                    string archivedPath = string.Empty;

                    XMLParameter pSetImageInfo = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "importImage")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivedPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo")
                        {
                            pSetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }
                    #endregion


                    #region Step 0: Create a patient for import
                    patientInternalId = PatientService.Utility_CreatePatientForSpecificCase("case1559");

                    if (string.IsNullOrEmpty(patientInternalId))
                    {
                        goto CLEANUP;
                    }
                    #endregion

                    #region Step 1: Call ImportService.importObject to import a image
                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;
                    ImportService importService = new ImportService();

                    CheckPoint cpImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import an archived  image");
                    r.CheckPoints.Add(cpImportImage);

                    XMLResult rtImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, archivedPath, true, "false");
                    if (rtImportImage.IsErrorOccured)
                    {
                        cpImportImage.Result = TestResult.Fail;
                        cpImportImage.Outputs.AddParameter("Import image returns error", "Import image", rtImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportImage.Result = TestResult.Pass;
                        cpImportImage.Outputs.AddParameter("Import image returns success", "Import image", rtImportImage.Message);

                        imageInternalID = rtImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rtImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    string imagePathAfterImport = string.Empty;
                    string archivedPathAfterImport = string.Empty;
                    string archiveTagAfterImport = string.Empty;
                    ImageService imageSvc = new ImageService();

                    #region Step 2-1: Check the getImageDescription return is correct after import image
                    CheckPoint cpGetImageDescriptionAfterImport = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after import image");
                    r.CheckPoints.Add(cpGetImageDescriptionAfterImport);

                    XMLResult rtGetImageDescriptionAfterImport = imageSvc.getImageDescription(imageInternalID);
                    imagePathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport) && archivedPathAfterImport.Equals(archivedPath) && string.IsNullOrEmpty(archiveTagAfterImport)) //getImageDescription should return not tags info
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Pass;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after import image" + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    else
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Fail;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after import image. Actually get: " + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    #endregion

                    #region Step 2-2: Check the info is correct in getImageInfo return after import image
                    CheckPoint cpGetImageInfoAfterImport = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not import the image");
                    r.CheckPoints.Add(cpGetImageInfoAfterImport);

                    XMLParameter pGetImageInfoAfterImport = new XMLParameter("image");
                    pGetImageInfoAfterImport.AddParameter("internal_id", imageInternalID);
                    XMLResult rtGetImageInfo = imageSvc.getImageInfo(pGetImageInfoAfterImport);

                    imagePathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport)) // && archivedPath.Equals(archivedPathAfterImport) && archiveTagAfterImport == "archived", will be supported in new service
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Pass;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after import image." + rtGetImageInfo.ResultContent);
                    }
                    else
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Fail;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after import image. Actually get: " + rtGetImageInfo.ResultContent);
                    }
                    #endregion

                    #region Step 3: Call ImageService.SetImageInfo to save the archived image as normal image
                    CheckPoint cpSetImageInfo = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image");
                    r.CheckPoints.Add(cpSetImageInfo);

                    XMLResult rtSetImageInfo = imageSvc.setImageInfo(pSetImageInfo, imageInternalID);

                    if (rtSetImageInfo.IsErrorOccured)
                    {
                        cpSetImageInfo.Result = TestResult.Fail;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns error", "SetImageInfo", rtSetImageInfo.Message);
                    }
                    else
                    {
                        cpSetImageInfo.Result = TestResult.Pass;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns success", "SetImageInfo", rtSetImageInfo.Message);
                    #endregion

                        #region Step 4-1: Check the getImageDescription return is correct after set image and image is transfering
                        CheckPoint cpGetImageDescriptionWhenTransfering = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct after set image and image is transfering");
                        r.CheckPoints.Add(cpGetImageDescriptionWhenTransfering);

                        XMLResult rtGetImageDescriptionWhenTransfering = imageSvc.getImageDescription(imageInternalID);
                        string imagePathWhenTransfering = rtGetImageDescriptionWhenTransfering.MultiResults[0].GetParameterValueByName("path");
                        string archivedPathWhenTransfering = rtGetImageDescriptionWhenTransfering.MultiResults[0].GetParameterValueByName("archive_path");
                        string archivedTagWhenTransfering = rtGetImageDescriptionWhenTransfering.MultiResults[0].GetParameterValueByName("tags");

                        if (imagePathWhenTransfering.Contains(Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory")) && string.IsNullOrEmpty(archivedPathWhenTransfering) && string.IsNullOrEmpty(archivedTagWhenTransfering))
                        {
                            cpGetImageDescriptionWhenTransfering.Result = TestResult.Pass;
                            cpGetImageDescriptionWhenTransfering.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image and image is transfering");
                        }
                        else
                        {
                            cpGetImageDescriptionWhenTransfering.Result = TestResult.Fail;
                            cpGetImageDescriptionWhenTransfering.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image and image is transfering. Actually get: " + rtGetImageDescriptionWhenTransfering.ResultContent);
                        }
                        #endregion

                        #region Step 4-2: Check the info is correct in getImageInfo return after set image and image is transfering
                        CheckPoint cpGetImageInfoWhenTransfering = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct after set image and image is transfering");
                        r.CheckPoints.Add(cpGetImageInfoWhenTransfering);

                        XMLParameter pGetImageInfoWhenTransfering = new XMLParameter("image");
                        pGetImageInfoWhenTransfering.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoWhenTransfering = imageSvc.getImageInfo(pGetImageInfoWhenTransfering);

                        imagePathWhenTransfering = rtGetImageInfoWhenTransfering.DicomArrayResult.GetParameterValueByName("path");
                        archivedPathWhenTransfering = rtGetImageInfoWhenTransfering.DicomArrayResult.GetParameterValueByName("archive_path");
                        archivedTagWhenTransfering = rtGetImageInfoWhenTransfering.DicomArrayResult.GetParameterValueByName("tags");

                        if (imagePathWhenTransfering.Contains(Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory")) && string.IsNullOrEmpty(archivedPathWhenTransfering) && string.IsNullOrEmpty(archivedTagWhenTransfering))
                        {
                            cpGetImageInfoWhenTransfering.Result = TestResult.Pass;
                            cpGetImageInfoWhenTransfering.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image and image is transfering");
                        }
                        else
                        {
                            cpGetImageInfoWhenTransfering.Result = TestResult.Fail;
                            cpGetImageInfoWhenTransfering.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image and image is transfering. Actually get: " + rtGetImageInfoWhenTransfering.ResultContent);
                        }
                        #endregion

                        // Wait the file is transferred to server DB
                        System.Threading.Thread.Sleep(20000);

                        // Below to check the info is correct after setImageInfo. Check points includes:
                        //1. The Image ID and PS ID are not changed
                        //2. GetImageDescription return correct image path and archive path, correct archive flag
                        //3. GetImageInfo return correct image path, correct archive flag
                        //4. Use the image path to check the image file is in server DB
                        //5. The original archived image is not deleted

                        //#region Step 3: Check the image is set, not newly created
                        ////PatientService patientSvc = new PatientService();
                        ////patientSvc.listObjects();
                        ////NewPatientService newPatientSvc = new NewPatientService();
                        ////newPatientSvc.listObjects();
                        //#endregion

                        #region Step 4: Check the ps is set, not newly created after set image
                        CheckPoint cpPSID = new CheckPoint("Check PS ID", "Check the PS ID is not changed after call setImageInfo");
                        r.CheckPoints.Add(cpPSID);

                        XMLParameter pListPS = new XMLParameter("filter");
                        pListPS.AddParameter("current", "true");
                        string currentPSIDAfterSet = imageSvc.listPresentationState(pListPS, imageInternalID).SingleResult; // may need change

                        if (currentPSIDAfterSet == currentPSID)
                        {
                            cpPSID.Result = TestResult.Pass;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is not changed");
                        }
                        else
                        {
                            cpPSID.Result = TestResult.Fail;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is changed. Expect: " + currentPSID + "; Actually new PS ID: " + currentPSIDAfterSet);
                        }
                        #endregion

                        string imagePathAfterSet = string.Empty;
                        string archivePathAfterSet = string.Empty;
                        string archivedTagAfterSet = string.Empty;

                        #region Step 5: Check the getImageDescription return is correct after set image
                        CheckPoint cpGetImageDescriptionAfterSet = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageDescriptionAfterSet);

                        XMLResult rtGetImageDescriptionAfterSet = imageSvc.getImageDescription(imageInternalID);
                        imagePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("archive_path");
                        archivedTagAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && archivePathAfterSet == string.Empty && string.IsNullOrEmpty(archivedTagAfterSet)) // imagePathAfterSet sampe: C:/Documents and Settings/All Users/Application Data/TW/PAS/pas_data/patient/03/8af0a7e63310cc65013310d46d0e0003/1956bc28-ca5e-4720-a857-d4de18fc1479/02962f27-e4be-4d59-b112-9663a2f2572b.dcm"
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Pass;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image");
                        }
                        else
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Fail;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image. Actually get: " + rtGetImageDescriptionAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 6: Check the getImageinfo return is correct after set image
                        CheckPoint cpGetImageInfoAfterSet = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageInfoAfterSet);

                        XMLParameter pGetImageInfoAfterSet = new XMLParameter("image");
                        pGetImageInfoAfterSet.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoAfterSet = imageSvc.getImageInfo(pGetImageInfoAfterSet);

                        imagePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("archive_path");
                        archivedTagAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archivedTagAfterSet))
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Pass;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image");
                        }
                        else
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Fail;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image. Actually get: " + rtGetImageInfoAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 7: Check the file exist in server DB
                        CheckPoint cpImageInDB = new CheckPoint("Check file in DB", "Check the file exist in server DB after call setImageInfo");
                        r.CheckPoints.Add(cpImageInDB);

                        if (Utility.IsImageExistInServerDB(imagePathAfterSet))
                        {
                            cpImageInDB.Result = TestResult.Pass;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File exist");
                        }
                        else
                        {
                            cpImageInDB.Result = TestResult.Fail;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File NOT exist");
                        }
                        #endregion

                        #region Step 8: Check the original archived image is not deleted
                        CheckPoint cpOriginalArchivedImage = new CheckPoint("Check original archived file", "Check the original archived image is not deleted after call setImageInfo");
                        r.CheckPoints.Add(cpOriginalArchivedImage);

                        if (System.IO.File.Exists(archivedPath))
                        {
                            cpOriginalArchivedImage.Result = TestResult.Pass;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File exist");
                        }
                        else
                        {
                            cpOriginalArchivedImage.Result = TestResult.Fail;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File NOT exist");
                        }
                        #endregion
                    }

                    #region Step 9: Call ImageService.deleteImage to delete the created image
                    if (!string.IsNullOrEmpty(imageInternalID))
                    {
                        CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                        r.CheckPoints.Add(cp_DeleteImage);

                        XMLResult rt_DeleteImage = imageSvc.deleteImage(imageInternalID, new XMLParameter("preferences"));
                        if (rt_DeleteImage.IsErrorOccured)
                        {
                            cp_DeleteImage.Result = TestResult.Fail;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);
                        }
                        else
                        {
                            cp_DeleteImage.Result = TestResult.Pass;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
                        }
                    }
                    #endregion

                CLEANUP:
                    #region Step 10: Delete the patient used in this case
                    if (!string.IsNullOrEmpty(patientInternalId))
                    {
                        CheckPoint cp_DeletePatient = new CheckPoint("Step 10: Delete Patient", "Call patientService.deletePatient to delete the patient");
                        r.CheckPoints.Add(cp_DeletePatient);

                        XMLResult rt_DeletePatient = PatientService.Utility_DeletePatientForSpecificCase(patientInternalId);
                        if (rt_DeletePatient.IsErrorOccured)
                        {
                            cp_DeletePatient.Result = TestResult.Fail;
                            cp_DeletePatient.Outputs.AddParameter("delete patient", "Delete patient returns error", rt_DeletePatient.Message);
                        }
                        else
                        {
                            cp_DeletePatient.Result = TestResult.Pass;
                            cp_DeletePatient.Outputs.AddParameter("delete patient", "Delete patient returns success", rt_DeletePatient.Message);
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

        public void Run_Image_SetInfo_Normal_Case1558()  // Case 1558:  1.1.04.01 SetImage_N01_Call setImage when the original archived image file is accessible
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    string patientInternalId = string.Empty;
                    string objectFileFullPath = string.Empty;
                    string archivePath = string.Empty;

                    XMLParameter pSetImageInfo = new XMLParameter("image");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "importImage")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "archivePath")
                            {
                                archivePath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setImageInfo")
                        {
                            pSetImageInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    #region Step 0: Create a patient for import
                    patientInternalId = PatientService.Utility_CreatePatientForSpecificCase("case1558");

                    if (string.IsNullOrEmpty(patientInternalId))
                    {
                        goto CLEANUP;
                    }
                    #endregion

                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;
                    ImportService importService = new ImportService();

                    #region Step 1: Call ImportService.importObject to import a image
                    CheckPoint cpImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import an archived  image");
                    r.CheckPoints.Add(cpImportImage);

                    XMLResult rtImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, archivePath, true, "false");
                    if (rtImportImage.IsErrorOccured)
                    {
                        cpImportImage.Result = TestResult.Fail;
                        cpImportImage.Outputs.AddParameter("Import image returns error", "Import image", rtImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cpImportImage.Result = TestResult.Pass;
                        cpImportImage.Outputs.AddParameter("Import image returns success", "Import image", rtImportImage.Message);

                        imageInternalID = rtImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rtImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    string imagePathAfterImport = string.Empty;
                    string archivedPathAfterImport = string.Empty;
                    string archiveTagAfterImport = string.Empty;
                    ImageService imageSvc = new ImageService();

                    #region Step 2-1: Check the getImageDescription return is correct after import image
                    CheckPoint cpGetImageDescriptionAfterImport = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after import image");
                    r.CheckPoints.Add(cpGetImageDescriptionAfterImport);

                    XMLResult rtGetImageDescriptionAfterImport = imageSvc.getImageDescription(imageInternalID);
                    imagePathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageDescriptionAfterImport.MultiResults[0].GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport) && archivedPathAfterImport.Equals(archivePath) && string.IsNullOrEmpty(archiveTagAfterImport)) //getImageDescription should return not tags info
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Pass;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after import image" + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    else
                    {
                        cpGetImageDescriptionAfterImport.Result = TestResult.Fail;
                        cpGetImageDescriptionAfterImport.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after import image. Actually get: " + rtGetImageDescriptionAfterImport.ResultContent);
                    }
                    #endregion

                    #region Step 2-2: Check the info is correct in getImageInfo return after import image
                    CheckPoint cpGetImageInfoAfterImport = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not import the image");
                    r.CheckPoints.Add(cpGetImageInfoAfterImport);

                    XMLParameter pGetImageInfoAfterImport = new XMLParameter("image");
                    pGetImageInfoAfterImport.AddParameter("internal_id", imageInternalID);
                    XMLResult rtGetImageInfo = imageSvc.getImageInfo(pGetImageInfoAfterImport);

                    imagePathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("path");
                    archivedPathAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("archive_path");
                    archiveTagAfterImport = rtGetImageInfo.DicomArrayResult.GetParameterValueByName("tags");

                    if (string.IsNullOrEmpty(imagePathAfterImport) && string.IsNullOrEmpty(archivedPathAfterImport) && string.IsNullOrEmpty(archiveTagAfterImport)) //&& archivePath.Equals(archivedPathAfterImport) && archiveTagAfterImport == "archived", will be supported in new service
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Pass;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after import image." + rtGetImageInfo.ResultContent);
                    }
                    else
                    {
                        cpGetImageInfoAfterImport.Result = TestResult.Fail;
                        cpGetImageInfoAfterImport.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after import image. Actually get: " + rtGetImageInfo.ResultContent);
                    }
                    #endregion


                    #region Step 2-3: Check the image info in listObject retrun after import, such as path, archivePath and tags
                    CheckPoint cpListObject = new CheckPoint("listObjects", "Call PatientService.listObjects to check the archive tag");
                    r.CheckPoints.Add(cpListObject);

                    NewPatientService patientSvcV2 = new NewPatientService();
                    PatientListObjectsRequestType pListObjects = new PatientListObjectsRequestType();
                    pListObjects.current = true;
                    pListObjects.currentSpecified = true;
                    pListObjects.patientInternalId = patientInternalId;
                    pListObjects.type = PatientListObjectsType.presentationState;
                    PatientListObjectsResponseType rtListObjectAfterImport = patientSvcV2.listObjects(pListObjects);

                    if (!patientSvcV2.LastReturnXMLValidateResult.isValid)
                    {
                        cpListObject.Result = TestResult.Fail;
                        cpListObject.Outputs.AddParameter("listObjects", "Check listObjects after import", "The listObjects return XML is not valid per XSD. Returned:" + patientSvcV2.LastReturnXML);
                    }
                    else
                    {
                        bool isArchivePathCorrect = string.Equals(rtListObjectAfterImport.presentationStates[0].image.archivePath, archivePath);
                        bool isPathCorrect = string.IsNullOrEmpty(rtListObjectAfterImport.presentationStates[0].image.path);
                        bool isTagCorrect = string.Equals(rtListObjectAfterImport.presentationStates[0].image.tags, "archived");

                        if (!isArchivePathCorrect || !isTagCorrect || !isPathCorrect)
                        {
                            cpListObject.Result = TestResult.Fail;
                            cpListObject.Outputs.AddParameter("listObjects", "Check the image info in the listObject return value", "The tag or archivedPath info is wrong in the return. Actually get: " + patientSvcV2.LastReturnXML);
                        }
                        else
                        {
                            cpListObject.Result = TestResult.Pass;
                            cpListObject.Outputs.AddParameter("listObjects", "Check the image info in the listObject return value", "The tag and archivedPath info is correct in the return.");
                        }
                    }
                    #endregion


                    #region Step 3: Call ImageService.SetImageInfo to save the archived image as normal image
                    CheckPoint cpSetImageInfo = new CheckPoint("Set Image Info", "Call ImageService.SetImageInfo to save the archived image as normal image");
                    r.CheckPoints.Add(cpSetImageInfo);

                    XMLResult rtSetImageInfo = imageSvc.setImageInfo(pSetImageInfo, imageInternalID);

                    if (rtSetImageInfo.IsErrorOccured)
                    {
                        cpSetImageInfo.Result = TestResult.Fail;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns error", "SetImageInfo", rtSetImageInfo.Message);
                    }
                    else
                    {
                        cpSetImageInfo.Result = TestResult.Pass;
                        cpSetImageInfo.Outputs.AddParameter("SetImageInfo returns success", "SetImageInfo", rtSetImageInfo.Message);
                    #endregion

                        // Wait the file is transferred to server DB
                        System.Threading.Thread.Sleep(3000);

                        // Below to check the info is correct after setImageInfo. Check points includes:
                        //1. The Image ID and PS ID are not changed
                        //2. GetImageDescription return correct image path and archive path, correct archive flag
                        //3. GetImageInfo return correct image path, correct archive flag
                        //4. Use the image path to check the image file is in server DB
                        //5. The original archived image is not deleted
                        //6. The listObject return contains correct archive tag

                        #region Step 4: Check the ps is set, not newly created after set image
                        CheckPoint cpPSID = new CheckPoint("Check PS ID", "Check the PS ID is not changed after call setImageInfo");
                        r.CheckPoints.Add(cpPSID);

                        XMLParameter pListPS = new XMLParameter("filter");
                        pListPS.AddParameter("current", "true");
                        string currentPSIDAfterSet = imageSvc.listPresentationState(pListPS, imageInternalID).SingleResult;

                        if (currentPSIDAfterSet == currentPSID)
                        {
                            cpPSID.Result = TestResult.Pass;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is not changed");
                        }
                        else
                        {
                            cpPSID.Result = TestResult.Fail;
                            cpPSID.Outputs.AddParameter("listPresentationState", "Check PS ID", "The PS ID is changed. Expect: " + currentPSID + "; Actually new PS ID: " + currentPSIDAfterSet);
                        }
                        #endregion

                        string imagePathAfterSet = string.Empty;
                        string archivePathAfterSet = string.Empty;
                        string archivedTagAfterSet = string.Empty;

                        #region Step 5: Check the getImageDescription return is correct after set image
                        CheckPoint cpGetImageDescriptionAfterSet = new CheckPoint("Check getImageDescription return", "Check getImageDescription return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageDescriptionAfterSet);

                        XMLResult rtGetImageDescriptionAfterSet = imageSvc.getImageDescription(imageInternalID);
                        imagePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("archive_path");
                        archivedTagAfterSet = rtGetImageDescriptionAfterSet.MultiResults[0].GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archivedTagAfterSet)) // imagePathAfterSet sampe: C:/Documents and Settings/All Users/Application Data/TW/PAS/pas_data/patient/03/8af0a7e63310cc65013310d46d0e0003/1956bc28-ca5e-4720-a857-d4de18fc1479/02962f27-e4be-4d59-b112-9663a2f2572b.dcm"
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Pass;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is correct after set image");
                        }
                        else
                        {
                            cpGetImageDescriptionAfterSet.Result = TestResult.Fail;
                            cpGetImageDescriptionAfterSet.Outputs.AddParameter("getImageDescription", "Check getImageDescription return", "The getImageDescription return is not correct after set image. Actually get: " + rtGetImageDescriptionAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 6: Check the getImageinfo return is correct after set image
                        CheckPoint cpGetImageInfoAfterSet = new CheckPoint("Check getImageInfo return", "Check getImageInfo return is corerct or not after call setImageInfo");
                        r.CheckPoints.Add(cpGetImageInfoAfterSet);

                        XMLParameter pGetImageInfoAfterSet = new XMLParameter("image");
                        pGetImageInfoAfterSet.AddParameter("internal_id", imageInternalID);
                        XMLResult rtGetImageInfoAfterSet = imageSvc.getImageInfo(pGetImageInfoAfterSet);

                        imagePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("path");
                        archivePathAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("archive_path");
                        archivedTagAfterSet = rtGetImageInfoAfterSet.DicomArrayResult.GetParameterValueByName("tags");

                        if (imagePathAfterSet.Contains(Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory")) && imagePathAfterSet.Contains(imageInternalID) && string.IsNullOrEmpty(archivePathAfterSet) && string.IsNullOrEmpty(archivedTagAfterSet))
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Pass;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is correct after set image");
                        }
                        else
                        {
                            cpGetImageInfoAfterSet.Result = TestResult.Fail;
                            cpGetImageInfoAfterSet.Outputs.AddParameter("getImageInfo", "Check getImageInfo return", "The getImageInfo return is not correct after set image. Actually get: " + rtGetImageInfoAfterSet.ResultContent);
                        }
                        #endregion

                        #region Step 7: Check the file exist in server DB
                        CheckPoint cpImageInDB = new CheckPoint("Check file in DB", "Check the file exist in server DB after call setImageInfo");
                        r.CheckPoints.Add(cpImageInDB);

                        if (Utility.IsImageExistInServerDB(imagePathAfterSet))
                        {
                            cpImageInDB.Result = TestResult.Pass;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File exist");
                        }
                        else
                        {
                            cpImageInDB.Result = TestResult.Fail;
                            cpImageInDB.Outputs.AddParameter("Check the file exist in server DB after call setImageInfo", "Check file in DB", "File NOT exist");
                        }
                        #endregion

                        #region Step 8: Check the original archived image is not deleted
                        CheckPoint cpOriginalArchivedImage = new CheckPoint("Check original archived file", "Check the original archived image is not deleted after call setImageInfo");
                        r.CheckPoints.Add(cpOriginalArchivedImage);

                        if (System.IO.File.Exists(archivePath))
                        {
                            cpOriginalArchivedImage.Result = TestResult.Pass;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File exist");
                        }
                        else
                        {
                            cpOriginalArchivedImage.Result = TestResult.Fail;
                            cpOriginalArchivedImage.Outputs.AddParameter("Check the original archived image is not deleted after call setImageInfo", "Check original archived file", "File NOT exist");
                        }
                        #endregion

                        #region Step 9: Check the image info in listObject retrun after set, such as path, archivePath and tags
                        CheckPoint cpListObjectAfterSet = new CheckPoint("listObjects", "Call PatientService.listObjects to check the archive tag after set");
                        r.CheckPoints.Add(cpListObjectAfterSet);

                        PatientListObjectsResponseType rtListObjectAfterSet = patientSvcV2.listObjects(pListObjects);
                        if (!patientSvcV2.LastReturnXMLValidateResult.isValid)
                        {
                            cpListObjectAfterSet.Result = TestResult.Fail;
                            cpListObjectAfterSet.Outputs.AddParameter("listObjects", "Check listObjects after set", "The listObjects return XML is not valid per XSD. Returned:" + patientSvcV2.LastReturnXML);
                        }
                        else
                        {
                            //bool isPathCorrect = rtListObjectAfterSet.presentationStates[0].image.path.Contains(Utility.GetCSDMConfig(CSDMConfigSection.remote, "patientDirectory")) && rtListObjectAfterSet.presentationStates[0].image.path.Contains(imageInternalID);
                            bool isArchivePathCorrect = string.IsNullOrEmpty(rtListObjectAfterSet.presentationStates[0].image.archivePath);
                            bool isTagCorrect = string.IsNullOrEmpty(rtListObjectAfterSet.presentationStates[0].image.tags);

                            if (!isArchivePathCorrect)
                            {
                                cpListObjectAfterSet.Result = TestResult.Fail;
                                cpListObjectAfterSet.Outputs.AddParameter("listObjects", "Check the image info in the listObject return value", "The archivedPath info is wrong in the return. Actually get: " + patientSvcV2.LastReturnXML);
                            }
                            else if (!isTagCorrect)
                            {
                                cpListObjectAfterSet.Result = TestResult.Fail;
                                cpListObjectAfterSet.Outputs.AddParameter("listObjects", "Check the image info in the listObject return value", "The tag info is wrong in the return. Actually get: " + patientSvcV2.LastReturnXML);
                            }
                            //else if (!isPathCorrect)
                            //{
                            //    cpListObjectAfterSet.Result = TestResult.Fail;
                            //    cpListObjectAfterSet.Outputs.AddParameter("listObjects", "Check the image info in the listObject return value", "The path info is wrong in the return. Actually get: " + patientSvc.LastReturnXML);
                            //}
                            else
                            {
                                cpListObjectAfterSet.Result = TestResult.Pass;
                                cpListObjectAfterSet.Outputs.AddParameter("listObjects", "Check the image info in the listObject return value", "The tag and archivedPath info is correct in the return.");
                            }
                        }
                        #endregion
                    }

                    #region Step 10: Call ImageService.deleteImage to delete the created image
                    if (!string.IsNullOrEmpty(imageInternalID))
                    {
                        CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                        r.CheckPoints.Add(cp_DeleteImage);

                        XMLResult rt_DeleteImage = imageSvc.deleteImage(imageInternalID, new XMLParameter("preferences"));
                        if (rt_DeleteImage.IsErrorOccured)
                        {
                            cp_DeleteImage.Result = TestResult.Fail;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);
                        }
                        else
                        {
                            cp_DeleteImage.Result = TestResult.Pass;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
                        }
                    }
                    #endregion

                CLEANUP:
                    #region Step 11: Delete the patient used in this case
                    if (!string.IsNullOrEmpty(patientInternalId))
                    {
                        CheckPoint cp_DeletePatient = new CheckPoint("Step 11: Delete Patient", "Call patientService.deletePatient to delete the patient");
                        r.CheckPoints.Add(cp_DeletePatient);

                        XMLResult rt_DeletePatient = PatientService.Utility_DeletePatientForSpecificCase(patientInternalId);
                        if (rt_DeletePatient.IsErrorOccured)
                        {
                            cp_DeletePatient.Result = TestResult.Fail;
                            cp_DeletePatient.Outputs.AddParameter("delete patient", "Delete patient returns error", rt_DeletePatient.Message);
                        }
                        else
                        {
                            cp_DeletePatient.Result = TestResult.Pass;
                            cp_DeletePatient.Outputs.AddParameter("delete patient", "Delete patient returns success", rt_DeletePatient.Message);
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
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.ToString());
                    SaveRound(r);
                }
            }

            Output();
        }
    }
}
