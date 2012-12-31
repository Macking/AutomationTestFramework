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
        public void Run_PS_CreatePresentationState_Normal_Case7() //Case 7: 1.3.7_CreatePresentationState_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: createPresentationState");

                try
                {
                    #region Parameter initialize
                    string imageInternalID = string.Empty;
                    XMLParameter p_CreateImage = new XMLParameter("image");
                    XMLParameter p_CreatePresentationState = new XMLParameter("presentationstate");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "createImage")
                        {
                            p_CreateImage.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                        if (ids.InputParameters.GetParameter(i).Step == "createPresentationState")
                        {
                            p_CreatePresentationState.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "createPresentationState")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "returnState")
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
                            else if (ids.ExpectedValues.GetParameter(i).Key == "returnMessage")
                            {
                                ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    ImageService imageService = new ImageService();
                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call ImageService.createImage to create a new image
                    CheckPoint cp_CreateImage = new CheckPoint("Image create", "Call imageService.createImage to create a new image");
                    r.CheckPoints.Add(cp_CreateImage);

                    XMLResult rt_CreateImage = imageService.createImage(p_CreateImage);
                    if (rt_CreateImage.IsErrorOccured)
                    {
                        cp_CreateImage.Result = TestResult.Fail;
                        cp_CreateImage.Outputs.AddParameter("create", "Create image returns error", rt_CreateImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cp_CreateImage.Result = TestResult.Pass;
                        cp_CreateImage.Outputs.AddParameter("create", "Create image returns success", rt_CreateImage.Message);
                        imageInternalID = rt_CreateImage.SingleResult;
                    }
                    #endregion

                    #region Step 2: Call PresentationStateService.CreatePresentationState to create a PS for the image
                    CheckPoint cp_CreatePresentationState = new CheckPoint("Create PS", "Call PresentationStateService.CreatePresentationState to create a new ps");
                    r.CheckPoints.Add(cp_CreatePresentationState);

                    XMLResult rt_CreatePresentationState = psService.createPresentationState(imageInternalID, p_CreatePresentationState);
                    if (true == ep_isReturnOK) // Expect the call returns OK 
                    {
                        if (rt_CreatePresentationState.IsErrorOccured)
                        {
                            cp_CreatePresentationState.Result = TestResult.Fail;
                            cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error", rt_CreatePresentationState.Message);
                        }
                        else
                        {
                            cp_CreatePresentationState.Result = TestResult.Pass;
                            cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns success", rt_CreatePresentationState.Message);
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_CreatePresentationState.IsErrorOccured) // There is error
                        {
                            if (rt_CreatePresentationState.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_CreatePresentationState.Result = TestResult.Pass;
                                cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error as expected", rt_CreatePresentationState.Message);
                            }
                            else
                            {
                                cp_CreatePresentationState.Result = TestResult.Fail;
                                cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_CreatePresentationState.Message);
                            }
                        }
                        else // There is no error
                        {
                            cp_CreatePresentationState.Result = TestResult.Fail;
                            cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS not returns error as expected. ", "Actually returns: " + rt_CreatePresentationState.Message);
                        }
                    }
                    #endregion

                    #region Step 3: Call ImageService.deleteImage to delete the created image
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
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_PS_SetPresentationStateDescription_Normal_Case8() //Case 8: 1.3.7_SetPresentationStateDescription_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    bool isImportImage = false;
                    string psID = null;
                    string imageID = null;

                    //Input parameters
                    string p_Import_patientId = null;
                    string p_Import_objectFileFullPath = null;
                    XMLParameter p_setPresentationState = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            isImportImage = true;
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalId")
                            {
                                p_Import_patientId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                p_Import_objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setPresentationState")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "presentationStateInternalID")
                            {
                                psID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else
                            {
                                p_setPresentationState.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }

                    // Output value
                    bool ep_isSetPSReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    XMLParameter ep_getPresentationState = new XMLParameter();
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "setPresentationState")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isSetPSReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isSetPSReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationState")
                        {
                            ep_getPresentationState.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    ImportService importService = new ImportService();
                    PresentationStateService psService = new PresentationStateService();

                    if (isImportImage)
                    {
                        #region Step 1: Call ImportService.ImportObject to import an image and create a PS
                        CheckPoint cp_Import = new CheckPoint("Import Image", "Call ImportService.ImportObject to import an image");
                        r.CheckPoints.Add(cp_Import);

                        XMLResult rt_Import = importService.importObject(p_Import_patientId, null, p_Import_objectFileFullPath, null, true, "FALSE");
                        if (rt_Import.IsErrorOccured)
                        {
                            cp_Import.Result = TestResult.Fail;
                            cp_Import.Outputs.AddParameter("import", "Import image returns error", rt_Import.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Set
                        }
                        else
                        {
                            cp_Import.Result = TestResult.Pass;
                            cp_Import.Outputs.AddParameter("import", "Import image returns success as expected", rt_Import.Message);

                            imageID = rt_Import.MultiResults[0].Parameters[0].ParameterValue; // This one is the ImageID
                            psID = rt_Import.MultiResults[1].Parameters[0].ParameterValue; // This one is the PS ID
                        }
                        #endregion
                    }

                    #region Step 2: Call PresentationStateService.SetPresentationState to set the PS for the image
                    CheckPoint cp_SetPresentationState = new CheckPoint("Set PS", "Call PresentationStateService.SetPresentationState to set ps info");
                    r.CheckPoints.Add(cp_SetPresentationState);

                    XMLResult rt_SetPresentationState = psService.setPresentationState(p_setPresentationState, psID);

                    if (ep_isSetPSReturnOK) // Expect the call returns OK 
                    {
                        if (rt_SetPresentationState.IsErrorOccured)
                        {
                            cp_SetPresentationState.Result = TestResult.Fail;
                            cp_SetPresentationState.Outputs.AddParameter("state", "Set PresentationState returns error", rt_SetPresentationState.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Set
                        }
                        else
                        {
                            cp_SetPresentationState.Result = TestResult.Pass;
                            cp_SetPresentationState.Outputs.AddParameter("state", "Set PresentationState returns success as expected", rt_SetPresentationState.Message);
                        }

                        #region Step 3: Call PresentationStateService.GetPresentationState to get the PS for the image
                        CheckPoint cp_GetPresentationState = new CheckPoint("Get PS", "Call PresentationStateService.GetPresentationState and check the return state");
                        r.CheckPoints.Add(cp_GetPresentationState);

                        XMLResult rt_GetPresentationState = psService.getPresentationState(psID);

                        if (rt_GetPresentationState.IsErrorOccured)
                        {
                            cp_GetPresentationState.Result = TestResult.Fail;
                            cp_GetPresentationState.Outputs.AddParameter("state", "Get PS returns error", rt_GetPresentationState.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Get
                        }
                        else
                        {
                            cp_GetPresentationState.Result = TestResult.Pass;
                            cp_GetPresentationState.Outputs.AddParameter("state", "Get PS returns succeed", rt_GetPresentationState.Message);

                            // Check the return value is correct
                            #region Step 4: Check the values in PresentationStateService.getPresentationState return are correct
                            CheckPoint cp_getPSReturnValues = new CheckPoint("Get PS", "Check the values in PresentationStateService.GetPresentationState return");
                            r.CheckPoints.Add(cp_getPSReturnValues);

                            bool isValueEqual = true; // Set defult to true to deal with the case no ep_getPresentationState need check
                            bool isKeyShow = true; // Set defult to true to deal with the case no ep_getPresentationState need check

                            foreach (XMLParameterNode psNode in ep_getPresentationState.Parameters)
                            {
                                isValueEqual = false;
                                isKeyShow = false;

                                int i = 0;
                                for (i = 0; i < rt_GetPresentationState.MultiResults[0].Parameters.Count; i++)
                                {
                                    if (psNode.ParameterName == rt_GetPresentationState.MultiResults[0].Parameters[i].ParameterName)
                                    {
                                        isKeyShow = true;
                                        isValueEqual = string.Equals(psNode.ParameterValue, rt_GetPresentationState.MultiResults[0].Parameters[i].ParameterValue.Replace("\r", "").Replace("\n", ""));
                                        break; // Find the node, end current for loop to search node
                                    }
                                }

                                if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                                {
                                    cp_getPSReturnValues.Result = TestResult.Fail;

                                    if (isKeyShow)
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationState does not match the expected.");
                                        cp_getPSReturnValues.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationState return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetPresentationState.MultiResults[0].Parameters[i].ParameterValue);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationState does not contain the node: " + psNode.ParameterName);
                                        cp_getPSReturnValues.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationState return", "The return value does not contain the node: " + psNode.ParameterName);
                                    }

                                    break; // End current foreach loop, not compare the follwing nodes
                                }
                            }

                            if (isValueEqual)
                            {
                                cp_getPSReturnValues.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("The return values in getPresentationStateInfo all match the expected.");
                                cp_getPSReturnValues.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return values all match the expected");
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else // Expect the call return error
                    {
                        if (rt_SetPresentationState.IsErrorOccured) // There is error
                        {
                            if (rt_SetPresentationState.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_SetPresentationState.Result = TestResult.Pass;
                                cp_SetPresentationState.Outputs.AddParameter("state", "Set PS returns error as expected", rt_SetPresentationState.Message);
                            }
                            else
                            {
                                cp_SetPresentationState.Result = TestResult.Fail;
                                cp_SetPresentationState.Outputs.AddParameter("state", "Set PS returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_SetPresentationState.Message);

                                SaveRound(r);
                                continue; // Error happens, end current Test Get
                            }
                        }
                        else // There is no error
                        {
                            cp_SetPresentationState.Result = TestResult.Fail;
                            cp_SetPresentationState.Outputs.AddParameter("state", "Set PS not returns error as expected. ", "Actually returns: " + rt_SetPresentationState.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Get
                        }
                    }
                    #endregion

                    if (isImportImage)
                    {
                        #region Step 4: Call ImageService.deleteImage to delete the created image
                        ImageService imageService = new ImageService();
                        CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                        r.CheckPoints.Add(cp_DeleteImage);

                        XMLResult rt_DeleteImage = imageService.deleteImage(imageID, new XMLParameter("preferences"));
                        if (rt_DeleteImage.IsErrorOccured)
                        {
                            cp_DeleteImage.Result = TestResult.Fail;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);

                            SaveRound(r);
                            continue; // End current test set
                        }
                        else
                        {
                            cp_DeleteImage.Result = TestResult.Pass;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_PS_GetPresentationStateDescription_Normal_Case9() //Case 9: 1.3.7_GetPresentationStateDescription_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: getPresentationState");

                try
                {
                    #region Parameter initialize
                    //Input parameters
                    string presentationStateInternalID = null;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getPresentationState")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "presentationStateInternalID")
                            {
                                presentationStateInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    // Oupput value
                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    string ep_imageID = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationState")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                                case "image_internal_id":
                                    {
                                        ep_imageID = ids.ExpectedValues.GetParameter(i).Value; ;
                                        break;
                                    }
                                //default:
                                //    {
                                //        ep_getPresentationState.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                                //        break;
                                //    }
                            }
                        }
                    }
                    #endregion

                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call PresentationStateService.GetPresentationState to get a PS for the image
                    CheckPoint cp_GetPresentationState = new CheckPoint("Get PS", "Call PresentationStateService.GetPresentationState to get a new ps");
                    r.CheckPoints.Add(cp_GetPresentationState);

                    XMLResult rt_GetPresentationState = psService.getPresentationState(presentationStateInternalID);
                    if (true == ep_isReturnOK) // Expect the call returns OK 
                    {
                        if (rt_GetPresentationState.IsErrorOccured)
                        {
                            cp_GetPresentationState.Result = TestResult.Fail;
                            cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns error", rt_GetPresentationState.Message);
                        }
                        else
                        {
                            // Check the return value is correct
                            // 1: check the "general.xml" is present
                            // 2. check the "processing.xml" is present
                            // 3. check the "annotation.xml" is present
                            // 4. check the image id is correct
                            if (false == rt_GetPresentationState.MultiResults[0].Parameters[0].ParameterName.Equals("general.xml"))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns not contain the general.xml info", "Actually return:" + rt_GetPresentationState.ResultContent);
                            }
                            else if (false == rt_GetPresentationState.MultiResults[0].Parameters[1].ParameterName.Equals("processing.xml"))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns not contain the processing.xml info", "Actually return:" + rt_GetPresentationState.ResultContent);
                            }
                            else if (false == rt_GetPresentationState.MultiResults[0].Parameters[2].ParameterName.Equals("annotation.xml"))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns not contain the annotation.xml info", "Actually return:" + rt_GetPresentationState.ResultContent);
                            }
                            else if (false == rt_GetPresentationState.MultiResults[0].Parameters[3].ParameterValue.Equals(ep_imageID))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns wrong image ID", "Expect: " + ep_imageID + ". Actually get: " + rt_GetPresentationState.MultiResults[0].Parameters[3].ParameterValue);
                            }
                            else
                            {
                                cp_GetPresentationState.Result = TestResult.Pass;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns success, all values are correct", rt_GetPresentationState.Message);
                            }
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_GetPresentationState.IsErrorOccured) // There is error
                        {
                            if (rt_GetPresentationState.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_GetPresentationState.Result = TestResult.Pass;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns error as expected", rt_GetPresentationState.Message);
                            }
                            else
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_GetPresentationState.Message);
                            }
                        }
                        else // There is no error
                        {
                            cp_GetPresentationState.Result = TestResult.Fail;
                            cp_GetPresentationState.Outputs.AddParameter("get", "Get PS not returns error as expected. ", "Actually returns: " + rt_GetPresentationState.Message);
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

        public void Run_PS_SetPresentationStateInfo_Normal_Case10() //Case 10: 1.3.7_SetPresentationStateInfo_Normal
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    bool isImportImage = false;
                    string psID = null;
                    string imageID = null;

                    //Input parameters
                    string p_Import_patientId = null;
                    string p_Import_objectFileFullPath = null;
                    XMLParameter p_setPresentationStateInfo = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            isImportImage = true;
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalId")
                            {
                                p_Import_patientId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                p_Import_objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setPresentationStateInfo")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "presentationStateInternalID")
                            {
                                psID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else
                            {
                                p_setPresentationStateInfo.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }

                    // Output value
                    bool ep_isSetPSInfoReturnOK = true;
                    string ep_SetPSInfoReturnMessage = string.Empty;
                    XMLParameter ep_getPresentationStateInfo = new XMLParameter();
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "setPresentationStateInfo")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isSetPSInfoReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isSetPSInfoReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_SetPSInfoReturnMessage = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationStateInfo")
                        {
                            ep_getPresentationStateInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    ImportService importService = new ImportService();
                    PresentationStateService psService = new PresentationStateService();

                    if (isImportImage)
                    {
                        #region Step 1: Call ImportService.ImportObject to import an image and create a PS
                        CheckPoint cp_Import = new CheckPoint("Import Image", "Call ImportService.ImportObject to import an image");
                        r.CheckPoints.Add(cp_Import);

                        XMLResult rt_Import = importService.importObject(p_Import_patientId, null, p_Import_objectFileFullPath, null, true, "FALSE");
                        if (rt_Import.IsErrorOccured)
                        {
                            cp_Import.Result = TestResult.Fail;
                            cp_Import.Outputs.AddParameter("import", "Import image returns error", rt_Import.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Set
                        }
                        else
                        {
                            cp_Import.Result = TestResult.Pass;
                            cp_Import.Outputs.AddParameter("import", "Import image returns success as expected", rt_Import.Message);

                            imageID = rt_Import.MultiResults[0].Parameters[0].ParameterValue; // This one is the ImageID
                            psID = rt_Import.MultiResults[1].Parameters[0].ParameterValue; // This one is the PS ID
                        }
                        #endregion
                    }

                    #region Step 2: Call PresentationStateService.SetPresentationStateInfo to set the PSInfo for the image
                    CheckPoint cp_SetPresentationStateInfo = new CheckPoint("Set PSInfo", "Call PresentationStateService.SetPresentationStateInfo to set ps info");
                    r.CheckPoints.Add(cp_SetPresentationStateInfo);

                    XMLResult rt_SetPresentationStateInfo = psService.setPresentationStateInfo(p_setPresentationStateInfo, psID);
                    if (ep_isSetPSInfoReturnOK) // Expect the call return OK
                    {
                        if (rt_SetPresentationStateInfo.IsErrorOccured)
                        {
                            cp_SetPresentationStateInfo.Result = TestResult.Fail;
                            cp_SetPresentationStateInfo.Outputs.AddParameter("state", "Set PresentationStateInfo returns error", rt_SetPresentationStateInfo.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Set
                        }
                        else
                        {
                            cp_SetPresentationStateInfo.Result = TestResult.Pass;
                            cp_SetPresentationStateInfo.Outputs.AddParameter("state", "Set PresentationStateInfo returns success as expected", rt_SetPresentationStateInfo.Message);

                            #region Step 3: Call PresentationStateService.GetPresentationStateInfo to get the PSInfo for the image
                            CheckPoint cp_GetPresentationStateInfo = new CheckPoint("Get PSInfo", "Call PresentationStateService.GetPresentationStateInfo and check the return state");
                            r.CheckPoints.Add(cp_GetPresentationStateInfo);

                            XMLParameterCollection p_GetPresentationStateInfo = new XMLParameterCollection();
                            XMLParameter p_GetPSInfo_ID = new XMLParameter("presentationstate");
                            p_GetPSInfo_ID.AddParameter("internal_id", psID); // Add current psID as the input param
                            XMLParameter p_GetPSInfo_Preferences = new XMLParameter("preferences");
                            p_GetPresentationStateInfo.Add(p_GetPSInfo_ID);
                            p_GetPresentationStateInfo.Add(p_GetPSInfo_Preferences);

                            XMLResult rt_GetPresentationStateInfo = psService.getPresentationStateInfo(p_GetPresentationStateInfo);

                            if (rt_GetPresentationStateInfo.IsErrorOccured)
                            {
                                cp_GetPresentationStateInfo.Result = TestResult.Fail;
                                cp_GetPresentationStateInfo.Outputs.AddParameter("state", "Get PSInfo returns error", rt_GetPresentationStateInfo.Message);

                                SaveRound(r);
                                continue; // Error happens, end current Test Get
                            }
                            else
                            {
                                cp_GetPresentationStateInfo.Result = TestResult.Pass;
                                cp_GetPresentationStateInfo.Outputs.AddParameter("state", "Get PSInfo returns succeed", rt_GetPresentationStateInfo.Message);

                                // Check the return value is correct
                                #region Step 4: Check the values in PresentationStateService.getPresentationStateInfo return are correct
                                CheckPoint cp_getPSInfoReturnValues = new CheckPoint("Get PSInfo", "Check the values in PresentationStateService.GetPresentationStateInfo return");
                                r.CheckPoints.Add(cp_getPSInfoReturnValues);

                                // Add two check values dynamically
                                ep_getPresentationStateInfo.AddParameter("internal_id", psID);
                                ep_getPresentationStateInfo.AddParameter("image_internal_id", imageID);

                                bool isValueEqual = false;
                                bool isKeyShow = false;
                                foreach (XMLParameterNode psNode in ep_getPresentationStateInfo.Parameters)
                                {
                                    isValueEqual = false;
                                    isKeyShow = false;

                                    int i = 0;
                                    for (i = 0; i < rt_GetPresentationStateInfo.DicomArrayResult.Parameters.Count; i++)
                                    {
                                        if (psNode.ParameterName == rt_GetPresentationStateInfo.DicomArrayResult.Parameters[i].ParameterName)
                                        {
                                            isKeyShow = true;
                                            isValueEqual = string.Equals(psNode.ParameterValue, rt_GetPresentationStateInfo.DicomArrayResult.Parameters[i].ParameterValue.Replace("\r", "").Replace("\n", ""));
                                            break; // Find the node, end current for loop to search node
                                        }
                                    }

                                    if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                                    {
                                        cp_getPSInfoReturnValues.Result = TestResult.Fail;

                                        if (isKeyShow)
                                        {
                                            System.Diagnostics.Debug.Print("The return value in getPresentationStateInfo does not match the expected.");
                                            cp_getPSInfoReturnValues.Outputs.AddParameter("PSInfo Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterValue);
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.Print("The return value in getPresentationStateInfo does not contain the node: " + psNode.ParameterName);
                                            cp_getPSInfoReturnValues.Outputs.AddParameter("PSInfo Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                        }

                                        break; // End current foreach loop, not compare the follwing nodes
                                    }
                                }

                                if (isValueEqual)
                                {
                                    cp_getPSInfoReturnValues.Result = TestResult.Pass;

                                    System.Diagnostics.Debug.Print("The return values in getPresentationStateInfoInfo all match the expected.");
                                    cp_getPSInfoReturnValues.Outputs.AddParameter("PSInfo Info", "Check the values in PresentationStateService.getPresentationStateInfoInfo return", "The return values all match the expected");
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_SetPresentationStateInfo.IsErrorOccured) // There is error
                        {
                            if (rt_SetPresentationStateInfo.Message.ToLower().Contains(ep_SetPSInfoReturnMessage.ToLower()))
                            {
                                cp_SetPresentationStateInfo.Result = TestResult.Pass;
                                cp_SetPresentationStateInfo.Outputs.AddParameter("state", "Set PresentationStateInfo returns error as expected", rt_SetPresentationStateInfo.Message);
                            }
                            else
                            {
                                cp_SetPresentationStateInfo.Result = TestResult.Fail;
                                cp_SetPresentationStateInfo.Outputs.AddParameter("state", "Set PresentationStateInfo returns error message not match the expected. ", "Expect: " + ep_SetPSInfoReturnMessage + "; Actually returns: " + rt_SetPresentationStateInfo.Message);

                                SaveRound(r);
                                continue; // Error happens, end current Test Get
                            }
                        }
                        else // There is no error
                        {
                            cp_SetPresentationStateInfo.Result = TestResult.Fail;
                            cp_SetPresentationStateInfo.Outputs.AddParameter("state", "Set PS not returns error as expected. ", "Actually returns: " + rt_SetPresentationStateInfo.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Get
                        }
                    }
                    #endregion

                    if (isImportImage)
                    {
                        #region Step 5: Call ImageService.deleteImage to delete the created image
                        ImageService imageService = new ImageService();
                        CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                        r.CheckPoints.Add(cp_DeleteImage);

                        XMLResult rt_DeleteImage = imageService.deleteImage(imageID, new XMLParameter("preferences"));
                        if (rt_DeleteImage.IsErrorOccured)
                        {
                            cp_DeleteImage.Result = TestResult.Fail;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);

                            SaveRound(r);
                            continue; // End current test set
                        }
                        else
                        {
                            cp_DeleteImage.Result = TestResult.Pass;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_PS_GetPresentationStateInfo_Case11() //Case 11: 1.3.7_GetPresentationStateInfo_N01
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: getPresentationStateInfo");

                try
                {
                    #region Parameter initialize
                    //Input parameters
                    XMLParameterCollection p_getPresentationStateInfo = new XMLParameterCollection();
                    XMLParameter p_PSID = new XMLParameter("presentationstate");
                    XMLParameter p_Preferences = new XMLParameter("preferences");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getPresentationStateInfo")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                p_PSID.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "teeth_number_notation")
                            {
                                p_Preferences.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }
                    p_getPresentationStateInfo.Add(p_PSID);
                    p_getPresentationStateInfo.Add(p_Preferences);

                    // Output value
                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    string ep_imageID = string.Empty;
                    XMLParameter ep_getPresentationStateInfo = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationStateInfo")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                                default:
                                    {
                                        ep_getPresentationStateInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                                        break;
                                    }
                            }
                        }
                    }
                    #endregion

                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call PresentationStateService.GetPresentationStateInfo to get a PS for the image
                    CheckPoint cp_GetPresentationStateInfo = new CheckPoint("Get PS Info", "Call PresentationStateService.GetPresentationStateInfo to get ps info");
                    r.CheckPoints.Add(cp_GetPresentationStateInfo);

                    XMLResult rt_GetPresentationStateInfo = psService.getPresentationStateInfo(p_getPresentationStateInfo);
                    if (true == ep_isReturnOK) // Expect the call returns OK 
                    {
                        if (rt_GetPresentationStateInfo.IsErrorOccured)
                        {
                            cp_GetPresentationStateInfo.Result = TestResult.Fail;
                            cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS returns error", rt_GetPresentationStateInfo.Message);
                        }
                        else
                        {
                            cp_GetPresentationStateInfo.Result = TestResult.Pass;
                            cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS returns succeed", rt_GetPresentationStateInfo.Message);

                            // Check the return value is correct
                            #region Step 2: Check the values in PresentationStateService.getPresentationStateInfo return are correct
                            CheckPoint cp_getPSInfoReturn = new CheckPoint("Get PS Info", "Check the values in PresentationStateService.GetPresentationStateInfo return");
                            r.CheckPoints.Add(cp_getPSInfoReturn);

                            bool isValueEqual = false;
                            bool isKeyShow = false;

                            foreach (XMLParameterNode psNode in ep_getPresentationStateInfo.Parameters)
                            {
                                isValueEqual = false;
                                isKeyShow = false;

                                int i = 0;
                                for (i = 0; i < rt_GetPresentationStateInfo.MultiResults[0].Parameters.Count; i++)
                                {
                                    if (psNode.ParameterName == rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterName)
                                    {
                                        isKeyShow = true;
                                        isValueEqual = string.Equals(psNode.ParameterValue, rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterValue);
                                        break; // End current for loop to search node
                                    }
                                }

                                if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                                {
                                    cp_getPSInfoReturn.Result = TestResult.Fail;

                                    if (isKeyShow)
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationStateInfo does not match the expected.");
                                        cp_getPSInfoReturn.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterValue);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationStateInfo does not contain the node: " + psNode.ParameterName);
                                        cp_getPSInfoReturn.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                    }

                                    break; // End current foreach loop, not compare the follwing nodes
                                }
                            }

                            if (isValueEqual)
                            {
                                cp_getPSInfoReturn.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("The return values in getPresentationStateInfo all match the expected.");
                                cp_getPSInfoReturn.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return values all match the expected");
                            }
                            #endregion
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_GetPresentationStateInfo.IsErrorOccured) // There is error
                        {
                            if (rt_GetPresentationStateInfo.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_GetPresentationStateInfo.Result = TestResult.Pass;
                                cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS Info returns error as expected", rt_GetPresentationStateInfo.Message);
                            }
                            else
                            {
                                cp_GetPresentationStateInfo.Result = TestResult.Fail;
                                cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS Info returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_GetPresentationStateInfo.Message);
                            }
                        }
                        else // There is no error
                        {
                            cp_GetPresentationStateInfo.Result = TestResult.Fail;
                            cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS Info not returns error as expected. ", "Actually returns: " + rt_GetPresentationStateInfo.Message);
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

        public void Run_PS_CreatePresentationState_Exception_Case73() //Case 73: 1.3.7_CreatePresentationState_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: createPresentationState");

                try
                {
                    #region Parameter initialize
                    string imageInternalID = string.Empty;
                    XMLParameter p_CreatePresentationState = new XMLParameter("presentationstate");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "image")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "imageInternalID")
                            {
                                imageInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        if (ids.InputParameters.GetParameter(i).Step == "createPresentationState")
                        {
                            p_CreatePresentationState.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "createPresentationState")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "returnState")
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
                            else if (ids.ExpectedValues.GetParameter(i).Key == "returnMessage")
                            {
                                ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    ImageService imageService = new ImageService();
                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call PresentationStateService.CreatePresentationState to create a PS for the image
                    CheckPoint cp_CreatePresentationState = new CheckPoint("Create PS", "Call PresentationStateService.CreatePresentationState to create a new ps");
                    r.CheckPoints.Add(cp_CreatePresentationState);

                    XMLResult rt_CreatePresentationState = psService.createPresentationState(imageInternalID, p_CreatePresentationState);
                    if (true == ep_isReturnOK) // Expect the call returns OK 
                    {
                        if (rt_CreatePresentationState.IsErrorOccured)
                        {
                            cp_CreatePresentationState.Result = TestResult.Fail;
                            cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error", rt_CreatePresentationState.Message);
                        }
                        else
                        {
                            cp_CreatePresentationState.Result = TestResult.Pass;
                            cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns success", rt_CreatePresentationState.Message);
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_CreatePresentationState.IsErrorOccured) // There is error
                        {
                            if (rt_CreatePresentationState.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_CreatePresentationState.Result = TestResult.Pass;
                                cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error as expected", rt_CreatePresentationState.Message);
                            }
                            else
                            {
                                cp_CreatePresentationState.Result = TestResult.Fail;
                                cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_CreatePresentationState.Message);
                            }
                        }
                        else // There is no error
                        {
                            cp_CreatePresentationState.Result = TestResult.Fail;
                            cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS not returns error as expected. ", "Actually returns: " + rt_CreatePresentationState.Message);
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

        public void Run_PS_SetPresentationStateDescription_ExceptionCase75() //Case 75: 1.3.7_SetPresentationStateDescription_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    #region Parameter initialize
                    bool isImportImage = false;
                    string psID = null;
                    string imageID = null;

                    //Input parameters
                    string p_Import_patientId = null;
                    string p_Import_objectFileFullPath = null;
                    XMLParameter p_setPresentationState = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            isImportImage = true;
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalId")
                            {
                                p_Import_patientId = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                p_Import_objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "setPresentationState")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "presentationStateInternalID")
                            {
                                psID = ids.InputParameters.GetParameter(i).Value;
                            }
                            else
                            {
                                p_setPresentationState.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }

                    // Output value
                    bool ep_isSetPSReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    XMLParameter ep_getPresentationState = new XMLParameter();
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "setPresentationState")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isSetPSReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isSetPSReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                            }
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationState")
                        {
                            ep_getPresentationState.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }
                    #endregion

                    ImportService importService = new ImportService();
                    PresentationStateService psService = new PresentationStateService();

                    if (isImportImage)
                    {
                        #region Step 1: Call ImportService.ImportObject to import an image and create a PS
                        CheckPoint cp_Import = new CheckPoint("Import Image", "Call ImportService.ImportObject to import an image");
                        r.CheckPoints.Add(cp_Import);

                        XMLResult rt_Import = importService.importObject(p_Import_patientId, null, p_Import_objectFileFullPath, null, true, "FALSE");
                        if (rt_Import.IsErrorOccured)
                        {
                            cp_Import.Result = TestResult.Fail;
                            cp_Import.Outputs.AddParameter("import", "Import image returns error", rt_Import.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Set
                        }
                        else
                        {
                            cp_Import.Result = TestResult.Pass;
                            cp_Import.Outputs.AddParameter("import", "Import image returns success as expected", rt_Import.Message);

                            imageID = rt_Import.MultiResults[0].Parameters[0].ParameterValue; // This one is the ImageID
                            psID = rt_Import.MultiResults[1].Parameters[0].ParameterValue; // This one is the PS ID
                        }
                        #endregion
                    }

                    #region Step 2: Call PresentationStateService.SetPresentationState to set the PS for the image
                    CheckPoint cp_SetPresentationState = new CheckPoint("Set PS", "Call PresentationStateService.SetPresentationState to set ps info");
                    r.CheckPoints.Add(cp_SetPresentationState);

                    XMLResult rt_SetPresentationState = psService.setPresentationState(p_setPresentationState, psID);

                    if (ep_isSetPSReturnOK) // Expect the call returns OK 
                    {
                        if (rt_SetPresentationState.IsErrorOccured)
                        {
                            cp_SetPresentationState.Result = TestResult.Fail;
                            cp_SetPresentationState.Outputs.AddParameter("state", "Set PresentationState returns error", rt_SetPresentationState.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Set
                        }
                        else
                        {
                            cp_SetPresentationState.Result = TestResult.Pass;
                            cp_SetPresentationState.Outputs.AddParameter("state", "Set PresentationState returns success as expected", rt_SetPresentationState.Message);
                        }

                        #region Step 3: Call PresentationStateService.GetPresentationState to get the PS for the image
                        CheckPoint cp_GetPresentationState = new CheckPoint("Get PS", "Call PresentationStateService.GetPresentationState and check the return state");
                        r.CheckPoints.Add(cp_GetPresentationState);

                        XMLResult rt_GetPresentationState = psService.getPresentationState(psID);

                        if (rt_GetPresentationState.IsErrorOccured)
                        {
                            cp_GetPresentationState.Result = TestResult.Fail;
                            cp_GetPresentationState.Outputs.AddParameter("state", "Get PS returns error", rt_GetPresentationState.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Get
                        }
                        else
                        {
                            cp_GetPresentationState.Result = TestResult.Pass;
                            cp_GetPresentationState.Outputs.AddParameter("state", "Get PS returns succeed", rt_GetPresentationState.Message);

                            // Check the return value is correct
                            #region Step 4: Check the values in PresentationStateService.getPresentationState return are correct
                            CheckPoint cp_getPSReturnValues = new CheckPoint("Get PS", "Check the values in PresentationStateService.GetPresentationState return");
                            r.CheckPoints.Add(cp_getPSReturnValues);

                            bool isValueEqual = false;
                            bool isKeyShow = false;

                            foreach (XMLParameterNode psNode in ep_getPresentationState.Parameters)
                            {
                                isValueEqual = false;
                                isKeyShow = false;

                                int i = 0;
                                for (i = 0; i < rt_GetPresentationState.MultiResults[0].Parameters.Count; i++)
                                {
                                    if (psNode.ParameterName == rt_GetPresentationState.MultiResults[0].Parameters[i].ParameterName)
                                    {
                                        isKeyShow = true;
                                        isValueEqual = string.Equals(psNode.ParameterValue, rt_GetPresentationState.MultiResults[0].Parameters[i].ParameterValue.Replace("\r", "").Replace("\n", ""));
                                        break; // Find the node, end current for loop to search node
                                    }
                                }

                                if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                                {
                                    cp_getPSReturnValues.Result = TestResult.Fail;

                                    if (isKeyShow)
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationState does not match the expected.");
                                        cp_getPSReturnValues.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationState return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetPresentationState.MultiResults[0].Parameters[i].ParameterValue);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationState does not contain the node: " + psNode.ParameterName);
                                        cp_getPSReturnValues.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationState return", "The return value does not contain the node: " + psNode.ParameterName);
                                    }

                                    break; // End current foreach loop, not compare the follwing nodes
                                }
                            }

                            if (isValueEqual)
                            {
                                cp_getPSReturnValues.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("The return values in getPresentationStateInfo all match the expected.");
                                cp_getPSReturnValues.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return values all match the expected");
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else // Expect the call return error
                    {
                        if (rt_SetPresentationState.IsErrorOccured) // There is error
                        {
                            if (rt_SetPresentationState.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_SetPresentationState.Result = TestResult.Pass;
                                cp_SetPresentationState.Outputs.AddParameter("state", "Set PS returns error as expected", rt_SetPresentationState.Message);
                            }
                            else
                            {
                                cp_SetPresentationState.Result = TestResult.Fail;
                                cp_SetPresentationState.Outputs.AddParameter("state", "Set PS returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_SetPresentationState.Message);

                                SaveRound(r);
                                continue; // Error happens, end current Test Get
                            }
                        }
                        else // There is no error
                        {
                            cp_SetPresentationState.Result = TestResult.Fail;
                            cp_SetPresentationState.Outputs.AddParameter("state", "Set PS not returns error as expected. ", "Actually returns: " + rt_SetPresentationState.Message);

                            SaveRound(r);
                            continue; // Error happens, end current Test Get
                        }
                    }
                    #endregion

                    if (isImportImage)
                    {
                        #region Step 4: Call ImageService.deleteImage to delete the created image
                        ImageService imageService = new ImageService();
                        CheckPoint cp_DeleteImage = new CheckPoint("Delete Image", "Call imageService.deleteImage to delete the image");
                        r.CheckPoints.Add(cp_DeleteImage);

                        XMLResult rt_DeleteImage = imageService.deleteImage(imageID, new XMLParameter("preferences"));
                        if (rt_DeleteImage.IsErrorOccured)
                        {
                            cp_DeleteImage.Result = TestResult.Fail;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns error", rt_DeleteImage.Message);

                            SaveRound(r);
                            continue; // End current test set
                        }
                        else
                        {
                            cp_DeleteImage.Result = TestResult.Pass;
                            cp_DeleteImage.Outputs.AddParameter("delete image", "Delete image returns success", rt_DeleteImage.Message);
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

        public void Run_PS_GetPresentationStateDescription_Exception_Case76() //Case 76: 1.3.7_GetPresentationStateDescription_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: getPresentationState");

                try
                {
                    #region Parameter initialize
                    //Input parameters
                    string presentationStateInternalID = null;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getPresentationState")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "presentationStateInternalID")
                            {
                                presentationStateInternalID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    // Oupput value
                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    string ep_imageID = string.Empty;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationState")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                                case "image_internal_id":
                                    {
                                        ep_imageID = ids.ExpectedValues.GetParameter(i).Value; ;
                                        break;
                                    }
                                //default:
                                //    {
                                //        ep_getPresentationState.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                                //        break;
                                //    }
                            }
                        }
                    }
                    #endregion

                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call PresentationStateService.GetPresentationState to get a PS for the image
                    CheckPoint cp_GetPresentationState = new CheckPoint("Get PS", "Call PresentationStateService.GetPresentationState to get a new ps");
                    r.CheckPoints.Add(cp_GetPresentationState);

                    XMLResult rt_GetPresentationState = psService.getPresentationState(presentationStateInternalID);
                    if (true == ep_isReturnOK) // Expect the call returns OK 
                    {
                        if (rt_GetPresentationState.IsErrorOccured)
                        {
                            cp_GetPresentationState.Result = TestResult.Fail;
                            cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns error", rt_GetPresentationState.Message);
                        }
                        else
                        {
                            // Check the return value is correct
                            // 1: check the "general.xml" is present
                            // 2. check the "processing.xml" is present
                            // 3. check the "annotation.xml" is present
                            // 4. check the image id is correct
                            if (false == rt_GetPresentationState.MultiResults[0].Parameters[0].ParameterName.Equals("general.xml"))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns not contain the general.xml info", "Actually return:" + rt_GetPresentationState.Message);
                            }
                            else if (false == rt_GetPresentationState.MultiResults[0].Parameters[1].ParameterName.Equals("processing.xml"))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns not contain the processing.xml info", "Actually return:" + rt_GetPresentationState.Message);
                            }
                            else if (false == rt_GetPresentationState.MultiResults[0].Parameters[2].ParameterName.Equals("annotation.xml"))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns not contain the annotation.xml info", "Actually return:" + rt_GetPresentationState.Message);
                            }
                            else if (false == rt_GetPresentationState.MultiResults[0].Parameters[3].ParameterValue.Equals(ep_imageID))
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns wrong image ID", "Expect: " + ep_imageID + ". Actually get: " + rt_GetPresentationState.MultiResults[0].Parameters[3].ParameterValue);
                            }
                            else
                            {
                                cp_GetPresentationState.Result = TestResult.Pass;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns success, all values are correct", rt_GetPresentationState.Message);
                            }
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_GetPresentationState.IsErrorOccured) // There is error
                        {
                            if (rt_GetPresentationState.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_GetPresentationState.Result = TestResult.Pass;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns error as expected", rt_GetPresentationState.Message);
                            }
                            else
                            {
                                cp_GetPresentationState.Result = TestResult.Fail;
                                cp_GetPresentationState.Outputs.AddParameter("get", "Get PS returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_GetPresentationState.Message);
                            }
                        }
                        else // There is no error
                        {
                            cp_GetPresentationState.Result = TestResult.Fail;
                            cp_GetPresentationState.Outputs.AddParameter("get", "Get PS not returns error as expected. ", "Actually returns: " + rt_GetPresentationState.Message);
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

        public void Run_PS_GetPresentationStateInfo_Exception_Case78() //Case 78: 1.3.7_GetPresentationStateInfo_Exception
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: getPresentationStateInfo");

                try
                {
                    #region Parameter initialize
                    //Input parameters
                    XMLParameterCollection p_getPresentationStateInfo = new XMLParameterCollection();
                    XMLParameter p_PSID = new XMLParameter("presentationstate");
                    XMLParameter p_Preferences = new XMLParameter("preferences");
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getPresentationStateInfo")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "internal_id")
                            {
                                p_PSID.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "teeth_number_notation")
                            {
                                p_Preferences.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            }
                        }
                    }
                    p_getPresentationStateInfo.Add(p_PSID);
                    p_getPresentationStateInfo.Add(p_Preferences);

                    // Output value
                    bool ep_isReturnOK = true;
                    string ep_ReturnValue = string.Empty;
                    string ep_imageID = string.Empty;
                    XMLParameter ep_getPresentationStateInfo = new XMLParameter("presentationstate");
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getPresentationStateInfo")
                        {
                            switch (ids.ExpectedValues.GetParameter(i).Key)
                            {
                                case "returnState":
                                    {
                                        if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("pass"))
                                        {
                                            ep_isReturnOK = true;
                                        }
                                        else if (ids.ExpectedValues.GetParameter(i).Value.ToLower().Equals("fail"))
                                        {
                                            ep_isReturnOK = false;
                                        }
                                        break;
                                    }
                                case "returnMessage":
                                    {
                                        ep_ReturnValue = ids.ExpectedValues.GetParameter(i).Value;
                                        break;
                                    }
                                default:
                                    {
                                        ep_getPresentationStateInfo.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                                        break;
                                    }
                            }
                        }
                    }
                    #endregion

                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call PresentationStateService.GetPresentationStateInfo to get a PS for the image
                    CheckPoint cp_GetPresentationStateInfo = new CheckPoint("Get PS Info", "Call PresentationStateService.GetPresentationStateInfo to get ps info");
                    r.CheckPoints.Add(cp_GetPresentationStateInfo);

                    XMLResult rt_GetPresentationStateInfo = psService.getPresentationStateInfo(p_getPresentationStateInfo);
                    if (true == ep_isReturnOK) // Expect the call returns OK 
                    {
                        if (rt_GetPresentationStateInfo.IsErrorOccured)
                        {
                            cp_GetPresentationStateInfo.Result = TestResult.Fail;
                            cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS returns error", rt_GetPresentationStateInfo.Message);
                        }
                        else
                        {
                            cp_GetPresentationStateInfo.Result = TestResult.Pass;
                            cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS returns succeed", rt_GetPresentationStateInfo.Message);

                            // Check the return value is correct
                            #region Step 2: Check the values in PresentationStateService.getPresentationStateInfo return are correct
                            CheckPoint cp_getPSInfoReturn = new CheckPoint("Get PS Info", "Check the values in PresentationStateService.GetPresentationStateInfo return");
                            r.CheckPoints.Add(cp_getPSInfoReturn);

                            bool isValueEqual = false;
                            bool isKeyShow = false;

                            foreach (XMLParameterNode psNode in ep_getPresentationStateInfo.Parameters)
                            {
                                isValueEqual = false;
                                isKeyShow = false;

                                int i = 0;
                                for (i = 0; i < rt_GetPresentationStateInfo.MultiResults[0].Parameters.Count; i++)
                                {
                                    if (psNode.ParameterName == rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterName)
                                    {
                                        isKeyShow = true;
                                        isValueEqual = string.Equals(psNode.ParameterValue, rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterValue);
                                        break; // End current for loop to search node
                                    }
                                }

                                if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                                {
                                    cp_getPSInfoReturn.Result = TestResult.Fail;

                                    if (isKeyShow)
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationStateInfo does not match the expected.");
                                        cp_getPSInfoReturn.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + rt_GetPresentationStateInfo.MultiResults[0].Parameters[i].ParameterValue);
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.Print("The return value in getPresentationStateInfo does not contain the node: " + psNode.ParameterName);
                                        cp_getPSInfoReturn.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return value does not contain the node: " + psNode.ParameterName);
                                    }

                                    break; // End current foreach loop, not compare the follwing nodes
                                }
                            }

                            if (isValueEqual)
                            {
                                cp_getPSInfoReturn.Result = TestResult.Pass;

                                System.Diagnostics.Debug.Print("The return values in getPresentationStateInfo all match the expected.");
                                cp_getPSInfoReturn.Outputs.AddParameter("PS Info", "Check the values in PresentationStateService.getPresentationStateInfo return", "The return values all match the expected");
                            }
                            #endregion
                        }
                    }
                    else // Expect the call return error
                    {
                        if (rt_GetPresentationStateInfo.IsErrorOccured) // There is error
                        {
                            if (rt_GetPresentationStateInfo.Message.ToLower().Contains(ep_ReturnValue.ToLower()))
                            {
                                cp_GetPresentationStateInfo.Result = TestResult.Pass;
                                cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS Info returns error as expected", rt_GetPresentationStateInfo.Message);
                            }
                            else
                            {
                                cp_GetPresentationStateInfo.Result = TestResult.Fail;
                                cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS Info returns error message not match the expected. ", "Expect: " + ep_ReturnValue + "; Actually returns: " + rt_GetPresentationStateInfo.Message);
                            }
                        }
                        else // There is no error
                        {
                            cp_GetPresentationStateInfo.Result = TestResult.Fail;
                            cp_GetPresentationStateInfo.Outputs.AddParameter("get", "Get PS Info not returns error as expected. ", "Actually returns: " + rt_GetPresentationStateInfo.Message);
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

        public void Run_PS_GetPresentationStateDescription_PSNotExist_Case992() //Case 992: 1.3.7_GetPresentationStateDescription_N02_WhenPSNotExist
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
                //XMLParameter psa = new XMLParameter("presentationstate");

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

                    for (int findPs = 0; findPs < rslImport.MultiResults.Count; findPs++)
                    {
                        if (rslImport.MultiResults[findPs].Name == "presentationstate")
                        {
                            string psUID = rslImport.MultiResults[findPs].Parameters[0].ParameterValue;
                            XMLResult psreturn = pss.getPresentationState(psUID);

                            CheckPoint pGetPS = new CheckPoint("Get Presentation State", "PS Desc");
                            r.CheckPoints.Add(pGetPS);
                            if (psreturn.MultiResults.Count == 1 && !string.IsNullOrEmpty(psreturn.MultiResults[0].GetParameterValueByName("image_internal_id")) && string.IsNullOrEmpty(psreturn.MultiResults[0].GetParameterValueByName("general.xml")) && string.IsNullOrEmpty(psreturn.MultiResults[0].GetParameterValueByName("processing.xml")) && string.IsNullOrEmpty(psreturn.MultiResults[0].GetParameterValueByName("annotation.xml")))
                            {
                                pGetPS.Result = TestResult.Pass;
                                pGetPS.Outputs.AddParameter("get presentation state", "get presentation state as expect", psreturn.ResultContent);
                            }
                            else
                            {
                                pGetPS.Result = TestResult.Fail;
                                pGetPS.Outputs.AddParameter("get presentation state", "get presentation state not as expect", psreturn.ResultContent);
                            }
                        }
                    }
                }
                if (isDeletePatient)
                {
                    XMLResult rltDelete = ps.deletePatient(patientUID);
                    if (!rltDelete.IsErrorOccured)
                    {
                        pCreate.Outputs.AddParameter("Delete created patient", "Delete Patient", rltDelete.Message);
                    }
                }
                SaveRound(r);
            }
            Output();
        }

        public void Run_PS_Import_GetPSInfo_Case1073()   //Case 1073: 1.3.7_GetPresentationStateInfo_N06_Import image and check the DICOM info value is correct in GetPSInfo return
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string patientID = null;
                    string objectFileFullPath = null;
                    XMLParameter getPSInfoReturnParam = new XMLParameter("DICOM INFO in getPSInfo return");

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "import")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "patientInternalID")
                            {
                                patientID = ids.InputParameters.GetParameter(i).Value;
                            }
                            if (ids.InputParameters.GetParameter(i).Key == "objectFileFullPath")
                            {
                                objectFileFullPath = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "getPSInfo")
                        {
                            // Add the key to check and the expected value
                            getPSInfoReturnParam.AddParameter(ids.ExpectedValues.GetParameter(i).Key, ids.ExpectedValues.GetParameter(i).Value);
                        }
                    }

                    #region Step 1: import the image
                    CheckPoint pImport = new CheckPoint("Import", "Import the prepared DICOM image file");
                    r.CheckPoints.Add(pImport);

                    ImportService import = new ImportService();
                    XMLResult importResult = import.importObject(patientID, null, objectFileFullPath, null, true, null);

                    if (importResult.IsErrorOccured)
                    {
                        pImport.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call import service to import DICOM image file returns error.");
                        pImport.Outputs.AddParameter("import", "Import DICOM image file", importResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        pImport.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call import service to import DICOM image file succeeds.");
                        pImport.Outputs.AddParameter("import", "Import DICOM image file", importResult.Message);
                    }
                    #endregion


                    #region Step 2: call getPSInfo
                    CheckPoint pGetPSInfo = new CheckPoint("GetPSInfo", "Get the presentionstate info");
                    r.CheckPoints.Add(pGetPSInfo);

                    PresentationStateService psService = new PresentationStateService();

                    XMLParameterCollection getPSInfoParam = new XMLParameterCollection();

                    for (int i = 0; i < importResult.MultiResults.Count; i++)
                    {
                        if (importResult.MultiResults[i].Name.ToLower() == "presentationstate")
                        {
                            getPSInfoParam.Add(importResult.MultiResults[i]);
                            break; // get the PS ID, end for loop
                        }
                    }

                    XMLResult getPSResult = psService.getPresentationStateInfo(getPSInfoParam);

                    if (getPSResult.IsErrorOccured)
                    {
                        pGetPSInfo.Result = TestResult.Fail;

                        System.Diagnostics.Debug.Print("Call getPSInfo returns error.");
                        pGetPSInfo.Outputs.AddParameter("getPSInfo", "Get the presentionstate info", importResult.Message);

                        SaveRound(r);
                        break; // There is error, end test case
                    }
                    else
                    {
                        pGetPSInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("Call getPSInfo succeeds.");
                        pGetPSInfo.Outputs.AddParameter("import", "Get the presentionstate info", importResult.Message);
                    }
                    #endregion

                    #region Step 3: check the dicom info in getPSInfo return
                    CheckPoint pDICOMInfo = new CheckPoint("DICOMInfo", "Check the DICOM info in the returned presentionstate info");
                    r.CheckPoints.Add(pDICOMInfo);

                    bool isValueEqual = false;
                    bool isKeyShow = false;

                    foreach (XMLParameterNode psNode in getPSInfoReturnParam.Parameters)
                    {
                        isValueEqual = false;
                        isKeyShow = false;

                        int i = 0;
                        for (i = 0; i < getPSResult.DicomArrayResult.Parameters.Count; i++)
                        {
                            if (getPSResult.DicomArrayResult.Parameters[i].ParameterName.ToLower() == "dicom_info")
                            {
                                if (getPSResult.DicomArrayResult.Parameters[i].ParameterValue.Contains("<parameter key=\"" + psNode.ParameterName + "\" value=\"" + psNode.ParameterValue + "\" />"))   // <parameter key="dcm_modality" value="PX" />
                                {
                                    isKeyShow = true;
                                    isValueEqual = true;
                                    break;
                                }
                            }
                            //if (psNode.ParameterName == getPSResult.DicomArrayResult.Parameters[i].ParameterName)  // need change here to check the dicom info
                            //{
                            //    isKeyShow = true;
                            //    isValueEqual = string.Equals(psNode.ParameterValue, getPSResult.DicomArrayResult.Parameters[i].ParameterValue);
                            //    break; // End current for loop to search node
                            //}
                        }

                        if (!isValueEqual) // There value is not matched or not found, log fail and then end the compare progress
                        {
                            pDICOMInfo.Result = TestResult.Fail;

                            if (isKeyShow)
                            {
                                System.Diagnostics.Debug.Print("The return value in getPSInfo does not match the expected.");
                                pDICOMInfo.Outputs.AddParameter("DICOMInfo", "Check the DICOM info in the returned presentionstate info", "The value does not match the expected for node: " + psNode.ParameterName + ". Expect: " + psNode.ParameterValue + ". Actually: " + getPSResult.MultiResults[0].Parameters[i].ParameterValue);
                            }
                            else
                            {
                                System.Diagnostics.Debug.Print("The return value in getPSInfo does not contain the node: " + psNode.ParameterName);
                                pDICOMInfo.Outputs.AddParameter("DICOMInfo", "Check the DICOM info in the returned presentionstate info", "The return value in getPSInfo does not contain the node: " + psNode.ParameterName);
                            }

                            break; // End current foreach loop, not compare the follwing nodes
                        }
                    }

                    if (isValueEqual)
                    {
                        pDICOMInfo.Result = TestResult.Pass;

                        System.Diagnostics.Debug.Print("The return values in getPSInfo all match the expected.");
                        pDICOMInfo.Outputs.AddParameter("DICOMInfo", "Check the DICOM info in the returned presentionstate info", "The return values in getPSInfo all match the expected");
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

        public void Run_PS_Acq_GetPSInfo_Case1089() //Case 1089: 1.3.7_GetPresentationStateInfo_N07_Acq image and then check the DICOM info value is correct in getPSInfo return
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Acquisition");
                AcquisitionService acqs = new AcquisitionService();
                PresentationStateService pss = new PresentationStateService();

                XMLParameter acq = new XMLParameter("acq_info");
                XMLParameter psa = new XMLParameter("presentationstate");

                XMLParameterCollection psaCollection = new XMLParameterCollection();

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
                            string psUid = string.Empty;

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

                                psUid = psIDs[0];

                                psa.AddParameter("internal_id", psUid);
                                XMLParameter psPreferences = new XMLParameter("preferences");
                                psaCollection.Add(psa);
                                psaCollection.Add(psPreferences);

                                XMLResult psinforeturn = pss.getPresentationStateInfo(psaCollection);

                                CheckPoint pGetPSinfo = new CheckPoint("Get Presentation State", "PS Desc");
                                r.CheckPoints.Add(pGetPSinfo);
                                for (int findvalue = 0; findvalue < psinforeturn.DicomArrayResult.Length; findvalue++)
                                {
                                    XMLParameterNode psnode = psinforeturn.DicomArrayResult.Parameters[findvalue];
                                    if (psnode.ParameterName == "acquired" && psnode.ParameterValue == "true")
                                    {
                                        pGetPSinfo.Result = TestResult.Pass;
                                        pGetPSinfo.Outputs.AddParameter("get presentation state", "get presentation state info as expect", "The acquired node value is true for newly acq image");
                                        break;
                                    }
                                }
                                if (pGetPSinfo.Result != TestResult.Pass)
                                {
                                    pGetPSinfo.Result = TestResult.Fail;
                                    pGetPSinfo.Outputs.AddParameter("get presentation state", "get presentation state info not as expect", "The acquired node value is NOT true for newly acq image. Actually get: " + psinforeturn.ResultContent);
                                }
                            }
                            else
                            {
                                pAcquire.Result = TestResult.Fail;
                                pAcquire.Outputs.AddParameter("getAcquisitionResult", "Acquisition", "Wrong return XML: " + getAcqRVG.ResultContent);
                            }

                            break; // The test case is finished, end the "do" loop
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

        public void Run_PS_Acq_Radiolog_Case1091() //Case 1091: 1.3.7_WorkFlow_AcquireRVG+SetImageInfo+GetPresentationStateInfo_RadioLogInformation
        {
            //int runCount = this.Input.Repetition;

            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p1 = new CheckPoint("createPatient", "Setup environment of createPatient");
                CheckPoint p2 = new CheckPoint("startAcquisition", "Setup environment of acquisition");
                CheckPoint p3 = new CheckPoint("getAcquisitionResult", "Setup environment of image and PS id");
                CheckPoint p4 = new CheckPoint("setImageInfo", "Setup environment of setImageInfo");
                CheckPoint p5 = new CheckPoint("getImageInfo", "Test getImageInfo");
                CheckPoint p6 = new CheckPoint("getPresentationStateInfo", "Test getPresentationStateInfo");

                r.CheckPoints.Add(p1);
                r.CheckPoints.Add(p2);
                r.CheckPoints.Add(p3);
                r.CheckPoints.Add(p4);
                r.CheckPoints.Add(p5);
                r.CheckPoints.Add(p6);

                //create required PAS service instaces here
                PatientService pats = new PatientService();
                AcquisitionService acq = new AcquisitionService();
                ImageService img = new ImageService();
                PresentationStateService ps = new PresentationStateService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa1 = new XMLParameter("patient");
                XMLParameter pa2 = new XMLParameter("acq_info");
                XMLParameter pa3 = new XMLParameter("getAcquisitionResult");
                XMLParameter pa4 = new XMLParameter("image");
                XMLParameter pa5 = new XMLParameter("image");
                XMLParameter pt6 = new XMLParameter("presentationstate");
                XMLParameterCollection pa6 = new XMLParameterCollection();

                try
                {

                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        switch (ids.InputParameters.GetParameter(i).Step)
                        {
                            case "createPatient":
                                pa1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "acquire":
                                pa2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "setImageInfo":
                                pa4.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
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

                    //if acquireType isn't emtpy string, skip step2, here is only RVG acquisition
                    bool acqflag = true;
                    XMLResult step3_result = new XMLResult();
                    if (pa2.GetParameterValueByName("acquireType") == null || pa2.GetParameterValueByName("acquireType") == "")
                    {
                        XMLResult rslAcqRVG = acq.startAcquisition(pa2);
                        System.Threading.Thread.Sleep(3000);
                        if (rslAcqRVG.IsErrorOccured)
                        {
                            System.Diagnostics.Debug.Print("Acquire fail:");
                            p2.Result = TestResult.Fail;
                            p2.Outputs.AddParameter("acquire", "Acquire image error", rslAcqRVG.Message);
                        }
                        else
                        {
                            p2.Result = TestResult.Pass;
                            p2.Outputs.AddParameter("acquire", "Acquire session ID", rslAcqRVG.SingleResult);
                            int DORVGcount = 0;
                            do
                            {
                                System.Threading.Thread.Sleep(3000);
                                System.Diagnostics.Debug.Print("get acquire in do");
                                DORVGcount++;
                                step3_result = acq.getAcquisitionResult(rslAcqRVG.SingleResult);
                                if (!step3_result.IsErrorOccured)
                                {
                                    // move this to out of do-while loop as need check the return value is correct or not there
                                    //p3.Result = TestResult.Pass;
                                    //p3.Outputs.AddParameter("acquire", "Get acquire image", step3_result.Message);
                                    acqflag = false;
                                    break;
                                }
                                if (step3_result.Code != 501)
                                { 
                                    continue; 
                                }
                                else
                                {
                                    p3.Result = TestResult.Fail;
                                    p3.Outputs.AddParameter("acquire", "Get acquire image error", step3_result.Message);
                                    acqflag = false;
                                }
                                System.Diagnostics.Debug.Print("get acquireResult:" + DORVGcount);
                                if (DORVGcount > 60)
                                {
                                    p3.Result = TestResult.Fail;
                                    p3.Outputs.AddParameter("acquire", "Get acquire image error", "Acquire greate with 3 minutes, timeout!");
                                    acqflag = false;
                                    break;
                                }
                            } while (acqflag);
                        }
                    }


                    //Change input parameter according the output of Step3
                    // if step3 is skipped, step4,step5,setp6 input has no need parameter of internal_id

                    //2012-11-28/19006723: Chanege as ther getAcquisitionResult result change, need new way to get the image ID and PS ID
                    string imgInternalID = string.Empty;
                    string psInternalID = string.Empty;

                    ParseXMLContent parser = new ParseXMLContent(step3_result.ResultContent); // To parse the return XML

                    string imageID = parser.getStringWithPathAndType("trophy/object_info", "image", "value");
                    string[] imageIDs = imageID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    int imageCount = imageIDs.Length;

                    string psID = parser.getStringWithPathAndType("trophy/object_info", "presentation_state", "value");
                    string[] psIDs = psID.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    int psCount = psIDs.Length;

                    if (imageCount == 1 && psCount == 1)
                    {
                        p3.Result = TestResult.Pass;
                        p3.Outputs.AddParameter("acquire", "getAcquisitionResult return OK", step3_result.ResultContent);

                        imgInternalID = imageIDs[0];
                        psInternalID = psIDs[0];

                        pa5.AddParameter("internal_id", imgInternalID);
                        pt6.AddParameter("internal_id", psInternalID);
                    }
                    else
                    {
                        p3.Result = TestResult.Fail;
                        p3.Outputs.AddParameter("acquire", "getAcquisitionResult return ERROR", step3_result.ResultContent);
                        SaveRound(r);
                        continue; // Not run below steps
                    }

                    //if (step3_result.MultiResults.Count > 0)
                    //{
                    //    for (int j = 0; j < step3_result.MultiResults.Count; j++)
                    //    {
                    //        for (int i = 0; i < step3_result.MultiResults[j].Parameters.Count; i++)
                    //        {
                    //            if (step3_result.MultiResults[j].Parameters[i].ParameterName == "image_internal_id")
                    //            {
                    //                imgInternalID = step3_result.MultiResults[j].Parameters[i].ParameterValue;
                    //                pa5.AddParameter("internal_id", step3_result.MultiResults[j].Parameters[i].ParameterValue);
                    //            }
                    //            if (step3_result.MultiResults[j].Parameters[i].ParameterName == "presentation_state_internal_id")
                    //            {
                    //                pt6.AddParameter("internal_id", step3_result.MultiResults[j].Parameters[i].ParameterValue);
                    //            }
                    //        }
                    //    }
                    //}

                    //Step4 result
                    XMLResult step4_result = img.setImageInfo(pa4, imgInternalID);

                    //Log step4 service output
                    if (step4_result.IsErrorOccured)
                    {
                        p4.Result = TestResult.Fail;
                        p4.Outputs.AddParameter("setImageInfo", "Error", step4_result.ResultContent);
                        SaveRound(r);
                        continue;
                    }

                    p4.Result = TestResult.Pass;
                    p4.Outputs.AddParameter("setImageInfo", "Success", step4_result.ResultContent);

                    //Step5 result
                    XMLResult step5_result = img.getImageInfo(pa5);

                    //Log step3 service output
                    //Compare the Expected parameters of getImageInfo and output result of getImageInfo
                    //count of compare matched parameters
                    int compCount = 0;
                    //count of expected parameters on getImageInfo
                    int expectedParameterCount = 0;
                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getImageInfo")
                        { 
                            expectedParameterCount++; 
                        }
                    }

                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getImageInfo" && ids.ExpectedValues.Parameters[index].Value == step5_result.DicomArrayResult.GetParameterValueByName(ids.ExpectedValues.Parameters[index].Key))
                        { 
                            compCount++; 
                        }
                    }

                    if (step5_result.IsErrorOccured || compCount != expectedParameterCount)
                    {
                        p5.Result = TestResult.Fail;
                        p5.Outputs.AddParameter("getImageInfo", "Error", step5_result.ResultContent);
                        SaveRound(r);
                        continue;
                    }

                    p5.Result = TestResult.Pass;
                    p5.Outputs.AddParameter("getImageInfo", "Success", step5_result.ResultContent);


                    //Change input parameter according the output of Step6

                    //generate input parameter collection of step6
                    pa6.Add(pt6);
                    //Step6 result

                    XMLResult step6_result = ps.getPresentationStateInfo(pa6);

                    //Log step6 service output
                    //Compare the Expected parameters of getPresentationStateInfo and output result of getPresentationStateInfo
                    compCount = 0;
                    //count of expected parameters on getPresentationStateInfo
                    expectedParameterCount = 0;
                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getPresentationStateInfo")
                        { expectedParameterCount++; }
                    }

                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getPresentationStateInfo" && ids.ExpectedValues.Parameters[index].Value == step6_result.DicomArrayResult.GetParameterValueByName(ids.ExpectedValues.Parameters[index].Key))
                        { compCount++; }
                    }

                    if (step6_result.IsErrorOccured || compCount != expectedParameterCount)
                    {
                        p6.Result = TestResult.Fail;
                        p6.Outputs.AddParameter("getPresentationStateInfo", "Error", step6_result.ResultContent);
                    }
                    else
                    {
                        p6.Result = TestResult.Pass;
                        p6.Outputs.AddParameter("getPresentationStateInfo", "Success", step6_result.ResultContent);
                    }

                    //Save data for each round
                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint p = new CheckPoint("Excepption", "Exception thrown when case run: "+ex.ToString());
                    r.CheckPoints.Add(p);
                    SaveRound(r);
                }
            }

            //Save service log as xml file
            Output();
        }

        public void Run_PS_ImportDicom_RadioLog_Case1094() //Case 1094: 1.3.7_WorkFlow_ImportDICOMImage+GetPresentationStateInfo_RadioLogInformation
        {
            //int runCount = this.Input.Repetition;

            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p1 = new CheckPoint("createPatient", "Setup environment of createPatient");
                CheckPoint p2 = new CheckPoint("importObject", "Setup environment of importObject");
                CheckPoint p3 = new CheckPoint("getImageInfo", "Test getImageInfo");
                CheckPoint p4 = new CheckPoint("getPresentationStateInfo", "Test getPresentationStateInfo");

                r.CheckPoints.Add(p1);
                r.CheckPoints.Add(p2);
                r.CheckPoints.Add(p3);
                r.CheckPoints.Add(p4);

                //create required PAS service instaces here
                PatientService pats = new PatientService();
                ImportService ims = new ImportService();
                ImageService img = new ImageService();
                PresentationStateService ps = new PresentationStateService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa1 = new XMLParameter("patient");
                XMLParameter pa2 = new XMLParameter("importObject");
                XMLParameter pa3 = new XMLParameter("image");
                XMLParameter ps4 = new XMLParameter("presentationstate");
                XMLParameterCollection pa4 = new XMLParameterCollection();

                try
                {
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        switch (ids.InputParameters.GetParameter(i).Step)
                        {
                            case "createPatient":
                                pa1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                                break;
                            case "importObject":
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
                        p1.Outputs.AddParameter("Step 1: createPatient", "Error", step1_result.ResultContent);
                        SaveRound(r);
                        continue;
                    }

                    p1.Result = TestResult.Pass;
                    p1.Outputs.AddParameter("Step 1: createPatient", "Success", step1_result.ResultContent);


                    //Change input parameter according the output of Step1
                    pa2.AddParameter("patientInternalId", step1_result.SingleResult);

                    //Step2 result

                    //if objectFileFullPath is empty string, skip step2
                    if (pa2.GetParameterValueByName("objectFileFullPath") != null && pa2.GetParameterValueByName("objectFileFullPath") != "")
                    {
                        XMLResult step2_result = ims.importObject(pa2.GetParameterValueByName("patientInternalId"), "", pa2.GetParameterValueByName("objectFileFullPath"), "", false, "");

                        //Log step2 service output
                        if (step2_result.IsErrorOccured)
                        {
                            p2.Result = TestResult.Fail;
                            p2.Outputs.AddParameter("Step 2: importObject", "Error", step2_result.ResultContent);
                            SaveRound(r);
                            continue;
                        }

                        p2.Result = TestResult.Pass;
                        p2.Outputs.AddParameter("Step 2: importObject", "Success", step2_result.ResultContent);


                        //Change input parameter according the output of Step2
                        // if step2 is skipped, step3 input has no need parameter of internal_id

                        for (int i = 0; i < step2_result.MultiResults.Count; i++)
                        {
                            if (step2_result.MultiResults[i].Name == "image")
                            {
                                pa3.AddParameter("internal_id", step2_result.MultiResults[i].GetParameterValueByIndex(i));
                            }
                            if (step2_result.MultiResults[i].Name == "presentationstate")
                            {
                                pa4.Add(step2_result.MultiResults[i]);
                            }
                        }
                    }

                    //Step3 result
                    XMLResult step3_result = img.getImageInfo(pa3);

                    if (step3_result.IsErrorOccured)
                    {
                        p3.Result = TestResult.Fail;
                        p3.Outputs.AddParameter("Step 3: getImageInfo", "The call 'getImageInfo' returns error: ", step3_result.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        p3.Result = TestResult.Pass; // Assume test pass at first, check the details later to update this
                    }

                    //Log step3 service output
                    int expectedParameterCount = 0;
                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getImageInfo")
                        {
                            expectedParameterCount++;
                        }
                    }

                    string dicom_info_getImageInfo = step3_result.DicomArrayResult.GetParameterValueByName("dicom_info");

                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getImageInfo")
                        {
                            // The dicom_info in return does not match the expected key and value
                            if (false == dicom_info_getImageInfo.Contains("<parameter key=\"" + ids.ExpectedValues.Parameters[index].Key + "\" value=\"" + ids.ExpectedValues.Parameters[index].Value + "\" />"))
                            {
                                p3.Result = TestResult.Fail;
                                p3.Outputs.AddParameter("Step 3: getImageInfo", "Error: the expect value does not match: " + ids.ExpectedValues.Parameters[index].Key + ". Actually get dicom_Info: ", dicom_info_getImageInfo);
                                break;
                            }
                        }
                    }
                    if (p3.Result == TestResult.Pass)
                    {
                        p3.Outputs.AddParameter("Step 3: getImageInfo", "The dicom info all match the expected value", dicom_info_getImageInfo);
                    }


                    //Step4 result
                    XMLResult step4_result = ps.getPresentationStateInfo(pa4);

                    if (step4_result.IsErrorOccured)
                    {
                        p4.Result = TestResult.Fail;
                        p4.Outputs.AddParameter("Step 4: getPresentationStateInfo", "The call 'getPresentationStateInfo' returns error: ", step4_result.Message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        p4.Result = TestResult.Pass;
                    }

                    expectedParameterCount = 0;  //count of expected parameters on getPresentationStateInfo
                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getPresentationStateInfo")
                        {
                            expectedParameterCount++;
                        }
                    }

                    string dicom_info_getPSInfo = step4_result.DicomArrayResult.GetParameterValueByName("dicom_info");

                    for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                    {
                        if (ids.ExpectedValues.Parameters[index].Step == "getPresentationStateInfo")
                        {
                            if (false == dicom_info_getPSInfo.Contains("<parameter key=\"" + ids.ExpectedValues.Parameters[index].Key + "\" value=\"" + ids.ExpectedValues.Parameters[index].Value + "\" />"))
                            {
                                p4.Result = TestResult.Fail;
                                p4.Outputs.AddParameter("Step 4: getPresentationStateInfo", "Error: the expect dicom value does not match: " + ids.ExpectedValues.Parameters[index].Key + ". Actually get dicom_Info: ", dicom_info_getPSInfo);
                                break;
                            }
                        }
                    }

                    if (p4.Result == TestResult.Pass)
                    {
                        p4.Outputs.AddParameter("Step 4: getPresentationStateInfo", "Success: the expect dicom value all match the expected", dicom_info_getPSInfo);
                    }

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

            //Save service log as xml file
            Output();
        }

        public void Run_PS_CreatePS_Case1566() //Case 1566: EK_HI00159159_Create Presentation State
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test PresentationState Service: createPresentationState");

                try
                {
                    #region Parameter initialize
                    string imageInternalID = string.Empty;
                    string currentPSID = string.Empty;
                    string patientInternalId = string.Empty;
                    string objectFileFullPath = string.Empty;

                    XMLParameter p_CreatePresentationState = new XMLParameter("presentationstate");

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
                        }
                        else if (ids.InputParameters.GetParameter(i).Step == "createPresentationState")
                        {
                            p_CreatePresentationState.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                        }
                    }

                    bool ep_isCurrent = false;
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "isCurrent")
                        {
                            ep_isCurrent = ids.ExpectedValues.GetParameter(i).Value.Equals("true");
                        }
                    }
                    #endregion

                    ImageService imageService = new ImageService();
                    ImportService importService = new ImportService();
                    PresentationStateService psService = new PresentationStateService();

                    #region Step 1: Call ImportService.importObject to import a image
                    CheckPoint cp_ImportImage = new CheckPoint("Import Image", "Call ImportService.importObject to import a image");
                    r.CheckPoints.Add(cp_ImportImage);

                    XMLResult rt_ImportImage = importService.importObject(patientInternalId, null, objectFileFullPath, null, true, "false");
                    if (rt_ImportImage.IsErrorOccured)
                    {
                        cp_ImportImage.Result = TestResult.Fail;
                        cp_ImportImage.Outputs.AddParameter("Import", "Import image returns error", rt_ImportImage.Message);

                        SaveRound(r);
                        continue; // There is error when create image, end current run
                    }
                    else
                    {
                        cp_ImportImage.Result = TestResult.Pass;
                        cp_ImportImage.Outputs.AddParameter("Import", "Import image returns success", rt_ImportImage.Message);
                        imageInternalID = rt_ImportImage.MultiResults[0].Parameters[0].ParameterValue;
                        currentPSID = rt_ImportImage.MultiResults[1].Parameters[0].ParameterValue;
                    }
                    #endregion

                    #region Step 2: Call PresentationStateService.CreatePresentationState to create a PS for the image
                    string returnPSID = string.Empty;
                    CheckPoint cp_CreatePresentationState = new CheckPoint("Create PS", "Call PresentationStateService.CreatePresentationState to create a new ps");
                    r.CheckPoints.Add(cp_CreatePresentationState);

                    XMLResult rt_CreatePresentationState = psService.createPresentationState(imageInternalID, p_CreatePresentationState);
                    returnPSID = rt_CreatePresentationState.SingleResult;

                    if (rt_CreatePresentationState.IsErrorOccured)
                    {
                        cp_CreatePresentationState.Result = TestResult.Fail;
                        cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns error", rt_CreatePresentationState.Message);
                    }
                    else
                    {
                        cp_CreatePresentationState.Result = TestResult.Pass;
                        cp_CreatePresentationState.Outputs.AddParameter("create", "Create PS returns success", rt_CreatePresentationState.Message);

                        CheckPoint cp_PresentationStateID = new CheckPoint("Check PS ID", "Check the retrun PS ID in create PS return");
                        r.CheckPoints.Add(cp_PresentationStateID);

                        if (currentPSID == returnPSID) // Return the old current PS ID, defect EK_HI00159159
                        {
                            cp_PresentationStateID.Result = TestResult.Fail;
                            cp_PresentationStateID.Outputs.AddParameter("Check PS ID", "Create PS returns error", "The return ID is not new as expected after call createPS");
                        }
                        else  //Actually return a new PS ID
                        {
                            //Get new current PS ID
                            XMLParameter pListPS = new XMLParameter("filter");
                            pListPS.AddParameter("current", "true");
                            currentPSID = imageService.listPresentationState(pListPS, imageInternalID).SingleResult;

                            if (ep_isCurrent == (currentPSID == returnPSID))
                            {
                                cp_PresentationStateID.Result = TestResult.Pass;
                                cp_PresentationStateID.Outputs.AddParameter("Check PS ID", "Create PS returns error", "The PS ID is correct after call createPS");
                            }
                            else
                            {
                                cp_PresentationStateID.Result = TestResult.Fail;
                                cp_PresentationStateID.Outputs.AddParameter("Check PS ID", "Create PS returns error", "The PS ID is not correct after call createPS");
                            }
                        }
                    }
                    #endregion

                    #region Step 3: Call ImageService.deleteImage to delete the created image
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
                    cp.Outputs.AddParameter("Exception thrown", "Exception Message", ex.Message);
                    SaveRound(r);
                }
            }

            Output();
        }
    }
}