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
        public void Run_Application_OpenObject_MultipleInstanceId_Case1518() //Case 1518: 1.1.01.08_OpenObjects_N08_MultipleInstanceId
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "OpenObjectV2");

                ApplicationServiceV2 app = new ApplicationServiceV2();
                AppOpenObjectsRequestType appRequest = new AppOpenObjectsRequestType();
                appRequest.instanceIDList = new string[512];
                appRequest.parameters = new ParameterType[512];
                int pnum = 0;
                int idnum = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {

                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "NA":
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "instanceid":
                                    appRequest.instanceIDList[idnum] = ids.InputParameters.GetParameter(i).Value;
                                    idnum++;
                                    break;
                                case "appType":
                                    switch (ids.InputParameters.GetParameter(i).Value)
                                    {
                                        case "TwoDViewer":
                                            appRequest.application = ApplicationType.TwoDViewer;
                                            break;
                                        case "ThreeDViewer":
                                            appRequest.application = ApplicationType.ThreeDViewer;
                                            break;
                                        case "Cosmetic":
                                            appRequest.application = ApplicationType.Cosmetic;
                                            break;
                                        case "Logicon":
                                            appRequest.application = ApplicationType.Logicon;
                                            break;
                                        case "CSRestore":
                                            appRequest.application = ApplicationType.CSRestore;
                                            break;
                                        case "CSModel":
                                            appRequest.application = ApplicationType.CSModel;
                                            break;
                                        default:
                                            throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                                    }
                                    break;
                            }
                            break;
                        case "parameters":
                            ParameterType p = new ParameterType();
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "configuration":
                                    p.key = KeyType.configuration;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "helpLanguage":
                                    p.key = KeyType.helpLanguage;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "language":
                                    p.key = KeyType.language;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                            }
                            appRequest.parameters[pnum] = p;
                            pnum++;
                            break;
                    }


                }
                CheckPoint pOpen = new CheckPoint("Open ObjectV2", "Open Object for " + appRequest.application + " muli-instnaces");
                r.CheckPoints.Add(pOpen);

                AppOpenObjectsResponseType response = app.openObject(appRequest);

                if (!app.LastReturnXMLValidateResult.isValid)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Invalid format", "Response is not complied with XML Schema");
                    SaveRound(r);
                    continue;
                }

                System.Threading.Thread.Sleep(2000);
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo("c:\\csautotest");
                System.Collections.ArrayList arguments = new System.Collections.ArrayList();
                foreach (System.IO.FileInfo f in d.GetFiles("openObject*.log"))
                {
                    System.IO.StreamReader sr = f.OpenText();
                    arguments.Add(sr.ReadLine());
                    sr.Close();
                    f.Delete();
                }

                int expect = int.Parse(ids.ExpectedValues.GetParameter("pnum").Value);
                if (arguments.Count != expect)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Fail", "Expert " + expect.ToString() + " processes open, actural:" + arguments.Count.ToString());
                    SaveRound(r);
                    continue;

                }
                else
                {
                    pOpen.Result = TestResult.Pass;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Success", "OK");
                }

                SaveRound(r);

            }
            Output();
        }

        public void Run_Application_OpenObject_CmdLine_withAppType_Case1540() //Case 1540: 1.1.01.15_OpenObject_CmdLineNormal_withAppType
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "OpenObjectV2");

                ApplicationServiceV2 app = new ApplicationServiceV2();
                AppOpenObjectsRequestType appRequest = new AppOpenObjectsRequestType();
                appRequest.instanceIDList = new string[512];
                appRequest.parameters = new ParameterType[512];
                int pnum = 0;
                int idnum = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {

                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "NA":
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "instanceid":
                                    appRequest.instanceIDList[idnum] = ids.InputParameters.GetParameter(i).Value;
                                    idnum++;
                                    break;
                                case "appType":
                                    switch (ids.InputParameters.GetParameter(i).Value)
                                    {
                                        case "TwoDViewer":
                                            appRequest.application = ApplicationType.TwoDViewer;
                                            break;
                                        case "ThreeDViewer":
                                            appRequest.application = ApplicationType.ThreeDViewer;
                                            break;
                                        case "Cosmetic":
                                            appRequest.application = ApplicationType.Cosmetic;
                                            break;
                                        case "Logicon":
                                            appRequest.application = ApplicationType.Logicon;
                                            break;
                                        case "CSRestore":
                                            appRequest.application = ApplicationType.CSRestore;
                                            break;
                                        case "CSModel":
                                            appRequest.application = ApplicationType.CSModel;
                                            break;

                                        default:
                                            throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                                    }
                                    break;
                            }
                            break;
                        case "parameters":
                            ParameterType p = new ParameterType();
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "configuration":
                                    p.key = KeyType.configuration;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "helpLanguage":
                                    p.key = KeyType.helpLanguage;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "language":
                                    p.key = KeyType.language;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                            }
                            appRequest.parameters[pnum] = p;
                            pnum++;
                            break;
                    }


                }
                CheckPoint pOpen = new CheckPoint("Open ObjectV2", "Open Object for commandline");
                r.CheckPoints.Add(pOpen);

                AppOpenObjectsResponseType response = app.openObject(appRequest);

                if (!app.LastReturnXMLValidateResult.isValid)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Invalid format", "Response is not complied with XML Schema");
                    SaveRound(r);
                    continue;
                }


                if (response.status.code != 0)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Fail", "Open " + appRequest.application.ToString() + " Fail," + response.status.message);
                    SaveRound(r);
                    continue;
                }

                System.Threading.Thread.Sleep(2000);
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
                System.Collections.ArrayList arguments = new System.Collections.ArrayList();
                foreach (System.IO.FileInfo f in d.GetFiles("openObject*.log"))
                {
                    System.IO.StreamReader sr = f.OpenText();
                    arguments.Add(sr.ReadLine());
                    sr.Close();
                    f.Delete();
                }

                if (arguments.Count != 1)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Fail", "Open " + appRequest.application.ToString() + " Fail,File not found or needs to clean log file openObject*.log and run the test again");
                    SaveRound(r);
                    continue;
                }


                if (arguments[0].ToString() == (ids.ExpectedValues.GetParameter("arguments").Value))
                {
                    pOpen.Result = TestResult.Pass;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Success", "Open " + appRequest.application.ToString() + " OK");

                }
                else
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Arguments not expected", "Open " + appRequest.application.ToString() + " Fail." + "Actural Arguments:" + arguments[0].ToString());

                }
                SaveRound(r);

            }
            Output();

        }

        public void Run_Application_OpenObject_CmdLine_noappType_Case1541() //Case 1541: 1.1.01.16_OpenObject_CmdLineNormal_with no appType
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "OpenObjectV2");


                object request = "<?xml version=\"1.0\"?>" +
                    "<tns:appOpenObjectsRequest xmlns:tns=\"http://www.carestreamhealth.com/CSI/CSDM/1/Schema\"" +
                    " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.carestreamhealth.com/CSI/CSDM/1/Schema" +
                    "ApplicationOpenObjectsRequest.xsd \">" +
                    "<tns:instanceIDList>" +
                    "<tns:instanceID>" + ids.InputParameters.GetParameter("instanceid").Value + "</tns:instanceID>" +
                    "</tns:instanceIDList>" +
                    "<parameters/>" +
                    "</tns:appOpenObjectsRequest>";

                ApplicationServiceV2 app = new ApplicationServiceV2();
                CheckPoint pOpen = new CheckPoint("Open ObjectV2", "Open Object for commandline no apptype");
                r.CheckPoints.Add(pOpen);

                string response = (string)app.InvokeMethod("openObjects", new object[] { request });
                if (!response.Contains("ok"))
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("openObjectV2", "Open Fail", response);
                    SaveRound(r);
                    continue;
                }

                System.Threading.Thread.Sleep(2000);
                System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
                System.Collections.ArrayList arguments = new System.Collections.ArrayList();
                foreach (System.IO.FileInfo f in d.GetFiles("openObject*.log"))
                {
                    System.IO.StreamReader sr = f.OpenText();
                    arguments.Add(sr.ReadLine());
                    sr.Close();
                    f.Delete();
                }

                if (arguments.Count != 1)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Fail", "Fail,File not found or needs to clean log file in c:\\csautotest and run the test again");
                    SaveRound(r);
                    continue;
                }


                if (arguments[0].ToString() == (ids.ExpectedValues.GetParameter("arguments").Value))
                {
                    pOpen.Result = TestResult.Pass;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Success", "Open OK");

                }
                else
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Arguments not expected", "Open Fail." + "Actural Arguments:" + arguments[0].ToString());

                }
                SaveRound(r);

            }
            Output();
        }

        public void Run_Application_OpenObjects_E01_InvalidInstanceId_Case1519() //Case 1519: 1.1.01.09_OpenObjects_E01_InvalidInstanceId
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "OpenObjectV2");

                ApplicationServiceV2 app = new ApplicationServiceV2();
                AppOpenObjectsRequestType appRequest = new AppOpenObjectsRequestType();
                appRequest.instanceIDList = new string[512];
                appRequest.parameters = new ParameterType[512];
                int pnum = 0;
                int idnum = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {

                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "NA":
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "instanceid":
                                    appRequest.instanceIDList[idnum] = ids.InputParameters.GetParameter(i).Value;
                                    idnum++;
                                    break;
                                case "appType":
                                    switch (ids.InputParameters.GetParameter(i).Value)
                                    {
                                        case "TwoDViewer":
                                            appRequest.application = ApplicationType.TwoDViewer;
                                            break;
                                        case "ThreeDViewer":
                                            appRequest.application = ApplicationType.ThreeDViewer;
                                            break;
                                        case "Cosmetic":
                                            appRequest.application = ApplicationType.Cosmetic;
                                            break;
                                        case "Logicon":
                                            appRequest.application = ApplicationType.Logicon;
                                            break;
                                        case "CSRestore":
                                            appRequest.application = ApplicationType.CSRestore;
                                            break;
                                        case "CSModel":
                                            appRequest.application = ApplicationType.CSModel;
                                            break;
                                        case null:
                                            break;
                                        case "":
                                            break;
                                        default:
                                            throw new Exception("Not a valid app type");
                                    }
                                    break;
                            }
                            break;
                        case "parameters":
                            ParameterType p = new ParameterType();
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "configuration":
                                    p.key = KeyType.configuration;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "helpLanguage":
                                    p.key = KeyType.helpLanguage;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "language":
                                    p.key = KeyType.language;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                            }
                            appRequest.parameters[pnum] = p;
                            pnum++;
                            break;
                    }


                }
                CheckPoint pOpen = new CheckPoint("Open ObjectV2", "Open Object for " + appRequest.application + " invalid instanceid");
                r.CheckPoints.Add(pOpen);

                AppOpenObjectsResponseType response = app.openObject(appRequest);

                if (!app.LastReturnXMLValidateResult.isValid)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Invalid format", "Response is not complied with XML Schema");
                    SaveRound(r);
                    continue;
                }

                if (response.status.code != int.Parse(ids.ExpectedValues.GetParameter("errcode").Value))
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Invalid instanceid", "Expected Code 315.Actual:" + response.status.code.ToString() + response.status.message);
                    SaveRound(r);
                    continue;
                }
                else
                {
                    pOpen.Result = TestResult.Pass;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Invalid instanceid", "OK");
                    SaveRound(r);
                }

            }
            Output();
        }

        public void Run_Application_OpenObjects_E02_TypeMismatch_Case1520() //Case 1520: 1.1.01.10_OpenObjects_E02_TypeMismatch
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "OpenObjectV2");

                ApplicationServiceV2 app = new ApplicationServiceV2();
                AppOpenObjectsRequestType appRequest = new AppOpenObjectsRequestType();
                appRequest.instanceIDList = new string[512];
                appRequest.parameters = new ParameterType[512];
                int pnum = 0;
                int idnum = 0;

                for (int i = 0; i < ids.InputParameters.Count; i++)
                {

                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "NA":
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "instanceid":
                                    appRequest.instanceIDList[idnum] = ids.InputParameters.GetParameter(i).Value;
                                    idnum++;
                                    break;
                                case "appType":
                                    switch (ids.InputParameters.GetParameter(i).Value)
                                    {
                                        case "TwoDViewer":
                                            appRequest.application = ApplicationType.TwoDViewer;
                                            break;
                                        case "ThreeDViewer":
                                            appRequest.application = ApplicationType.ThreeDViewer;
                                            break;
                                        case "Cosmetic":
                                            appRequest.application = ApplicationType.Cosmetic;
                                            break;
                                        case "Logicon":
                                            appRequest.application = ApplicationType.Logicon;
                                            break;
                                        case "CSRestore":
                                            appRequest.application = ApplicationType.CSRestore;
                                            break;
                                        case "CSModel":
                                            appRequest.application = ApplicationType.CSModel;
                                            break;
                                        default:
                                            throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                                    }
                                    break;
                            }
                            break;
                        case "parameters":
                            ParameterType p = new ParameterType();
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "configuration":
                                    p.key = KeyType.configuration;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "helpLanguage":
                                    p.key = KeyType.helpLanguage;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "language":
                                    p.key = KeyType.language;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                default:
                                    throw new Exception("No a valid enum value:" + ids.InputParameters.GetParameter(i).Key);

                            }
                            appRequest.parameters[pnum] = p;
                            pnum++;
                            break;
                    }


                }
                CheckPoint pOpen = new CheckPoint("Open ObjectV2", "Open Object for " + appRequest.application + " type mismatch");
                r.CheckPoints.Add(pOpen);

                AppOpenObjectsResponseType response = app.openObject(appRequest);

                if (!app.LastReturnXMLValidateResult.isValid)
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Invalid format", "Response is not complied with XML Schema");
                    SaveRound(r);
                    continue;
                }

                if (response.status.code != int.Parse(ids.ExpectedValues.GetParameter("errcode").Value))
                {
                    pOpen.Result = TestResult.Fail;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Type Mismatch", "Expected Code 301.Actual:" + response.status.code.ToString() + response.status.message);
                    SaveRound(r);
                    continue;
                }
                else
                {
                    pOpen.Result = TestResult.Pass;
                    pOpen.Outputs.AddParameter("Open ObjectV2", "Type Mismatch", "OK");
                }

                SaveRound(r);

            }
            Output();
        }

        public void Run_Application_OpenObjectWith2D_Normal_Case1525()  // Case 1525: 1.1.01.13_Call OpenObject to open objects with 2D Viewer
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), "Open Object with 2D Viewer");
                r.Key = ids.Key;
                r.Description = ids.Description;

                ApplicationServiceV2 applicationSvc = new ApplicationServiceV2();
                AppOpenObjectsRequestType pOpenObject = new AppOpenObjectsRequestType();

                System.Collections.Generic.List<string> objectIDList = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<ParameterType> parameterList = new System.Collections.Generic.List<ParameterType>();

                #region Test data: to construct the request parameter
                try
                {
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "openObject")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "application")
                            {
                                pOpenObject.application = (ApplicationType)Enum.Parse(typeof(ApplicationType), ids.InputParameters.GetParameter(i).Value);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                objectIDList.Add(ids.InputParameters.GetParameter(i).Value);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "configuration")
                            {
                                ParameterType p = new ParameterType();
                                p.key = KeyType.configuration;
                                p.value = ids.InputParameters.GetParameter(i).Value;
                                parameterList.Add(p);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "language")
                            {
                                ParameterType p = new ParameterType();
                                p.key = KeyType.language;
                                p.value = ids.InputParameters.GetParameter(i).Value;
                                parameterList.Add(p);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "helpLanguage")
                            {
                                ParameterType p = new ParameterType();
                                p.key = KeyType.helpLanguage;
                                p.value = ids.InputParameters.GetParameter(i).Value;
                                parameterList.Add(p);
                            }
                        }
                    }
                    pOpenObject.instanceIDList = objectIDList.ToArray();
                    pOpenObject.parameters = parameterList.ToArray();
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown: ", "Initialize request parameter", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
                #endregion

                #region Test data: to construct the expected values
                string epErrorCode = string.Empty;
                string epErrorMessage = string.Empty;
                string processName = "imaging";
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "openObject")
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                        {
                            epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                        {
                            epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "processName")
                        {
                            processName = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }
                }
                #endregion

                try
                {
                    #region Step1 : Call ApplicationService.openObject
                    CheckPoint cpOpenObject = new CheckPoint("openObject", "Call ApplicationService.openObject");
                    r.CheckPoints.Add(cpOpenObject);

                    //Send web service request
                    AppOpenObjectsResponseType rtOpenObjects = applicationSvc.openObject(pOpenObject);

                    //Wait 2D to start
                    System.Threading.Thread.Sleep(20000);

                    if (applicationSvc.LastReturnXMLValidateResult.isValid)
                    {
                        if (rtOpenObjects.status.code.ToString() == epErrorCode && rtOpenObjects.status.message.Contains(epErrorMessage))
                        {
                            cpOpenObject.Result = TestResult.Pass;
                            cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns correct: " + applicationSvc.LastReturnXML);
                        }
                        else
                        {
                            cpOpenObject.Result = TestResult.Fail;
                            cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns error: " + applicationSvc.LastReturnXML);
                        }
                    }
                    else
                    {
                        cpOpenObject.Result = TestResult.Fail;
                        cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject return invalid XML string: " + applicationSvc.LastReturnXML);
                    }
                    #endregion

                    #region Step 2: Check the program instance is lanched or not
                    CheckPoint cpProcess = new CheckPoint("openObject", "Check the 2D Viewer process after call openObject");
                    r.CheckPoints.Add(cpProcess);

                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
                    if (processes.Length == 1)
                    {
                        cpProcess.Result = TestResult.Pass;
                        cpProcess.Outputs.AddParameter("OpenObject", "Check 2D Viewer process", "The 2D viewer is running well after call openObject");

                        processes[0].Kill();
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        cpProcess.Result = TestResult.Fail;
                        cpProcess.Outputs.AddParameter("OpenObject", "Check 2D Viewer process", "The 2D viewer is not running well after call openObject. The process number is: " + processes.Length);

                        foreach (System.Diagnostics.Process p in processes)
                        {
                            p.Kill();
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Case Run", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }

            }
            Output();
        }

        public void Run_Application_OpenObjectWith2D_Exception_Case1526() //Case 1526: 1.1.01.14_Call OpenObject to open objects with 2D Viewer_Exception
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), "Open Object with 2D Viewer");
                r.Key = ids.Key;
                r.Description = ids.Description;

                ApplicationServiceV2 applicationSvc = new ApplicationServiceV2();
                AppOpenObjectsRequestType pOpenObject = new AppOpenObjectsRequestType();

                System.Collections.Generic.List<string> objectIDList = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<ParameterType> parameterList = new System.Collections.Generic.List<ParameterType>();

                #region Test data: to construct the request parameter
                try
                {
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "openObject")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "application")
                            {
                                pOpenObject.application = (ApplicationType)Enum.Parse(typeof(ApplicationType), ids.InputParameters.GetParameter(i).Value);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                objectIDList.Add(ids.InputParameters.GetParameter(i).Value);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "configuration")
                            {
                                ParameterType p = new ParameterType();
                                p.key = KeyType.configuration;
                                p.value = ids.InputParameters.GetParameter(i).Value;
                                parameterList.Add(p);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "language")
                            {
                                ParameterType p = new ParameterType();
                                p.key = KeyType.language;
                                p.value = ids.InputParameters.GetParameter(i).Value;
                                parameterList.Add(p);
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "helpLanguage")
                            {
                                ParameterType p = new ParameterType();
                                p.key = KeyType.helpLanguage;
                                p.value = ids.InputParameters.GetParameter(i).Value;
                                parameterList.Add(p);
                            }
                        }
                    }
                    pOpenObject.instanceIDList = objectIDList.ToArray();
                    pOpenObject.parameters = parameterList.ToArray();
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown: ", "Initialize request parameter", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
                #endregion

                #region Test data: to construct the expected values
                string epErrorCode = string.Empty;
                string epErrorMessage = string.Empty;
                string processName = "imaging";
                for (int i = 0; i < ids.ExpectedValues.Count; i++)
                {
                    if (ids.ExpectedValues.GetParameter(i).Step == "openObject")
                    {
                        if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                        {
                            epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                        {
                            epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                        }
                        else if (ids.ExpectedValues.GetParameter(i).Key == "processName")
                        {
                            processName = ids.ExpectedValues.GetParameter(i).Value;
                        }
                    }
                }
                #endregion

                try
                {
                    #region Step1 : Call ApplicationService.openObject
                    CheckPoint cpOpenObject = new CheckPoint("openObject", "Call ApplicationService.openObject");
                    r.CheckPoints.Add(cpOpenObject);

                    //Send web service request
                    AppOpenObjectsResponseType rtOpenObjects = applicationSvc.openObject(pOpenObject);

                    if (applicationSvc.LastReturnXMLValidateResult.isValid)
                    {
                        if (rtOpenObjects.status.code.ToString() == epErrorCode && rtOpenObjects.status.message.Contains(epErrorMessage))
                        {
                            cpOpenObject.Result = TestResult.Pass;
                            cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns expected value: " + applicationSvc.LastReturnXML);
                        }
                        else
                        {
                            cpOpenObject.Result = TestResult.Fail;
                            cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns not expected value: " + applicationSvc.LastReturnXML);
                        }
                    }
                    else
                    {
                        cpOpenObject.Result = TestResult.Fail;
                        cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject return invalid XML string: " + applicationSvc.LastReturnXML);
                    }
                    #endregion

                    #region Step 2: Check the 2D is lanched or not
                    CheckPoint cpProcess = new CheckPoint("openObject", "Check the 2D Viewer process after call openObject");
                    r.CheckPoints.Add(cpProcess);

                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        cpProcess.Result = TestResult.Pass;
                        cpProcess.Outputs.AddParameter("OpenObject", "Check 2D Viewer process", "The 2D viewer is really not running as expected  after call openObject");
                    }
                    else
                    {
                        cpProcess.Result = TestResult.Fail;
                        cpProcess.Outputs.AddParameter("OpenObject", "Check 2D Viewer process", "The 2D viewer is running after call openObject. The process number is: " + processes.Length);

                        foreach (System.Diagnostics.Process p in processes)
                        {
                            p.Kill();
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    #endregion

                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Case Run", ex.Message);
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        public void Run_Application_Open3DAnalysis_Normal_Case1629()  // Case 1629: 1.1.01.17_Call OpenObject to open 3D analysis_Normal
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;

                Round r = this.NewRound(runCount.ToString(), "Open Object with 3D Viewer");
                r.Key = ids.Key;
                r.Description = ids.Description;

                try
                {
                    bool isCreate = false;
                    Analysis3DService analysis3DSvc = new Analysis3DService();
                    ApplicationServiceV2 applicationSvc = new ApplicationServiceV2();
                    AppOpenObjectsRequestType pOpenObject = new AppOpenObjectsRequestType();

                    string p01_instanceUID = null;
                    XMLParameter p02_xmlAnalysis3DInfo = new XMLParameter("analysis3d");
                    p02_xmlAnalysis3DInfo.AddParameter("name", "name");
                    p02_xmlAnalysis3DInfo.AddParameter("analysis3D_xml", "analysis3D_xml");
                    p02_xmlAnalysis3DInfo.AddParameter("current", "true");

                    System.Collections.Generic.List<string> objectIDList = new System.Collections.Generic.List<string>();
                    System.Collections.Generic.List<ParameterType> parameterList = new System.Collections.Generic.List<ParameterType>();

                    #region Test data: to construct the request parameter
                    try
                    {
                        for (int i = 0; i < ids.InputParameters.Count; i++)
                        {
                            if (ids.InputParameters.GetParameter(i).Step == "createAnalysis3D")
                            {
                                isCreate = true;
                                if (ids.InputParameters.GetParameter(i).Key == "p01_instanceUID")
                                {
                                    p01_instanceUID = ids.InputParameters.GetParameter(i).Value;
                                }
                            }
                            else if (ids.InputParameters.GetParameter(i).Step == "openObject")
                            {
                                if (ids.InputParameters.GetParameter(i).Key == "application")
                                {
                                    pOpenObject.application = (ApplicationType)Enum.Parse(typeof(ApplicationType), ids.InputParameters.GetParameter(i).Value);
                                }
                                else if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                                {
                                    objectIDList.Add(ids.InputParameters.GetParameter(i).Value);
                                }
                                else if (ids.InputParameters.GetParameter(i).Key == "configuration")
                                {
                                    ParameterType p = new ParameterType();
                                    p.key = KeyType.configuration;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    parameterList.Add(p);
                                }
                                else if (ids.InputParameters.GetParameter(i).Key == "language")
                                {
                                    ParameterType p = new ParameterType();
                                    p.key = KeyType.language;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    parameterList.Add(p);
                                }
                                else if (ids.InputParameters.GetParameter(i).Key == "helpLanguage")
                                {
                                    ParameterType p = new ParameterType();
                                    p.key = KeyType.helpLanguage;
                                    p.value = ids.InputParameters.GetParameter(i).Value;
                                    parameterList.Add(p);
                                }
                            }
                        }
                        pOpenObject.instanceIDList = objectIDList.ToArray();
                        pOpenObject.parameters = parameterList.ToArray();
                    }
                    catch (Exception ex)
                    {
                        CheckPoint cp = new CheckPoint();
                        r.CheckPoints.Add(cp);
                        cp.Outputs.AddParameter("Exception thrown: ", "Initialize request parameter", ex.Message);
                        cp.Result = TestResult.Fail;
                        SaveRound(r);
                    }
                    #endregion

                    #region Test data: to construct the expected values
                    string epErrorCode = string.Empty;
                    string epErrorMessage = string.Empty;
                    string processName = "imaging";
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        if (ids.ExpectedValues.GetParameter(i).Step == "openObject")
                        {
                            if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
                            {
                                epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
                            {
                                epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
                            }
                            else if (ids.ExpectedValues.GetParameter(i).Key == "processName")
                            {
                                processName = ids.ExpectedValues.GetParameter(i).Value;
                            }
                        }
                    }
                    #endregion

                    #region Step 1: Create Anaylisis
                    if (isCreate)
                    {
                        CheckPoint cpCreateAnayisys = new CheckPoint("createAnalysis3D", "Call Analysis3DSvc.createAnalysis3D to create 3D analysis");
                        r.CheckPoints.Add(cpCreateAnayisys);

                        XMLResult rtCreateAnaylisis = analysis3DSvc.createAnalysis3D(p01_instanceUID, p02_xmlAnalysis3DInfo);

                        if (rtCreateAnaylisis.IsErrorOccured)
                        {
                            cpCreateAnayisys.Result = TestResult.Fail;
                            cpCreateAnayisys.Outputs.AddParameter("Create Analysis", "Result", "Fail. Return Message: " + rtCreateAnaylisis.ResultContent);
                        }
                        else
                        {
                            cpCreateAnayisys.Result = TestResult.Pass;
                            cpCreateAnayisys.Outputs.AddParameter("Create Analysis", "Result", "Succeed");
                            objectIDList.Add(rtCreateAnaylisis.SingleResult);
                            pOpenObject.instanceIDList = objectIDList.ToArray();
                        }
                    }
                    #endregion

                    #region Step2 : Call ApplicationService.openObject
                    CheckPoint cpOpenObject = new CheckPoint("openObject", "Call ApplicationService.openObject");
                    r.CheckPoints.Add(cpOpenObject);

                    //Send web service request
                    AppOpenObjectsResponseType rtOpenObjects = applicationSvc.openObject(pOpenObject);

                    if (applicationSvc.LastReturnXMLValidateResult.isValid)
                    {
                        if (rtOpenObjects.status.code.ToString() == epErrorCode && rtOpenObjects.status.message.Contains(epErrorMessage))
                        {
                            cpOpenObject.Result = TestResult.Pass;
                            cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns correct: " + applicationSvc.LastReturnXML);
                        }
                        else
                        {
                            cpOpenObject.Result = TestResult.Fail;
                            cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns error: " + applicationSvc.LastReturnXML);
                        }
                    }
                    else
                    {
                        cpOpenObject.Result = TestResult.Fail;
                        cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject return invalid XML string: " + applicationSvc.LastReturnXML);
                    }
                    #endregion

                    //Wait to start
                    System.Threading.Thread.Sleep(10000);

                    #region Step 2: Check the program instance is lanched or not
                    CheckPoint cpProcess = new CheckPoint("openObject", "Check the application process after call openObject");
                    r.CheckPoints.Add(cpProcess);

                    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
                    if (processes.Length == 1)
                    {
                        cpProcess.Result = TestResult.Pass;
                        cpProcess.Outputs.AddParameter("OpenObject", "Check process", "The application is running well after call openObject");

                        processes[0].Kill();
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        // todo: 2012/11/29, 19006723: comment out this as the 3D applcation does not support open analysis yet, change back this after it's ready
                        //cpProcess.Result = TestResult.Fail;
                        cpProcess.Result = TestResult.Pass;
                        cpProcess.Outputs.AddParameter("OpenObject", "Check process", "The application is not running well after call openObject. The process number is: " + processes.Length);

                        foreach (System.Diagnostics.Process p in processes)
                        {
                            p.Kill();
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    #endregion

                    #region Step 3: delete created analysis
                    if (isCreate)
                    {
                        foreach (string id in objectIDList)
                        {
                            XMLResult rtDeleteAnalysis = analysis3DSvc.deleteAnalysis3D(id);
                        }
                    }
                    #endregion
                    SaveRound(r);
                }
                catch (Exception ex)
                {
                    CheckPoint cp = new CheckPoint();
                    r.CheckPoints.Add(cp);
                    cp.Outputs.AddParameter("Exception thrown", "Case Run", ex.Message + ex.InnerException == null ? "" : "Inner Exception: " + ex.InnerException.ToString());
                    cp.Result = TestResult.Fail;
                    SaveRound(r);
                }
            }

            Output();
        }

        //public void Run_Application_Open3DAnalysis_Exception_Case1641()  // Case 1641: 1.1.01.18_Call OpenObject to open 3D analysis_Exception
        //{
        //    int runCount = 0;
        //    foreach (InputDataSet ids in this.Input.DataSets)
        //    {
        //        runCount++;

        //        Round r = this.NewRound(runCount.ToString(), "Open Object with 3D Viewer");
        //        r.Key = ids.Key;
        //        r.Description = ids.Description;

        //        try
        //        {
        //            bool isCreate = false;
        //            Analysis3DService analysis3DSvc = new Analysis3DService();
        //            ApplicationServiceV2 applicationSvc = new ApplicationServiceV2();
        //            AppOpenObjectsRequestType pOpenObject = new AppOpenObjectsRequestType();

        //            string p01_instanceUID = null;
        //            XMLParameter p02_xmlAnalysis3DInfo = new XMLParameter("analysis3d");
        //            p02_xmlAnalysis3DInfo.AddParameter("name", "name");
        //            p02_xmlAnalysis3DInfo.AddParameter("analysis3D_xml", "analysis3D_xml");
        //            p02_xmlAnalysis3DInfo.AddParameter("current", "true");

        //            System.Collections.Generic.List<string> objectIDList = new System.Collections.Generic.List<string>();
        //            System.Collections.Generic.List<ParameterType> parameterList = new System.Collections.Generic.List<ParameterType>();

        //            #region Test data: to construct the request parameter
        //            try
        //            {
        //                for (int i = 0; i < ids.InputParameters.Count; i++)
        //                {
        //                    if (ids.InputParameters.GetParameter(i).Step == "createAnalysis3D")
        //                    {
        //                        isCreate = true;
        //                        if (ids.InputParameters.GetParameter(i).Key == "p01_instanceUID")
        //                        {
        //                            p01_instanceUID = ids.InputParameters.GetParameter(i).Value;
        //                        }
        //                    }
        //                    else if (ids.InputParameters.GetParameter(i).Step == "openObject")
        //                    {
        //                        if (ids.InputParameters.GetParameter(i).Key == "application")
        //                        {
        //                            pOpenObject.application = (ApplicationType)Enum.Parse(typeof(ApplicationType), ids.InputParameters.GetParameter(i).Value);
        //                        }
        //                        else if (ids.InputParameters.GetParameter(i).Key == "instanceID")
        //                        {
        //                            objectIDList.Add(ids.InputParameters.GetParameter(i).Value);
        //                        }
        //                        else if (ids.InputParameters.GetParameter(i).Key == "configuration")
        //                        {
        //                            ParameterType p = new ParameterType();
        //                            p.key = KeyType.configuration;
        //                            p.value = ids.InputParameters.GetParameter(i).Value;
        //                            parameterList.Add(p);
        //                        }
        //                        else if (ids.InputParameters.GetParameter(i).Key == "language")
        //                        {
        //                            ParameterType p = new ParameterType();
        //                            p.key = KeyType.language;
        //                            p.value = ids.InputParameters.GetParameter(i).Value;
        //                            parameterList.Add(p);
        //                        }
        //                        else if (ids.InputParameters.GetParameter(i).Key == "helpLanguage")
        //                        {
        //                            ParameterType p = new ParameterType();
        //                            p.key = KeyType.helpLanguage;
        //                            p.value = ids.InputParameters.GetParameter(i).Value;
        //                            parameterList.Add(p);
        //                        }
        //                    }
        //                }
        //                pOpenObject.instanceIDList = objectIDList.ToArray();
        //                pOpenObject.parameters = parameterList.ToArray();
        //            }
        //            catch (Exception ex)
        //            {
        //                CheckPoint cp = new CheckPoint();
        //                r.CheckPoints.Add(cp);
        //                cp.Outputs.AddParameter("Exception thrown: ", "Initialize request parameter", ex.Message);
        //                cp.Result = TestResult.Fail;
        //                SaveRound(r);
        //            }
        //            #endregion

        //            #region Test data: to construct the expected values
        //            string epErrorCode = string.Empty;
        //            string epErrorMessage = string.Empty;
        //            string processName = "imaging";
        //            for (int i = 0; i < ids.ExpectedValues.Count; i++)
        //            {
        //                if (ids.ExpectedValues.GetParameter(i).Step == "openObject")
        //                {
        //                    if (ids.ExpectedValues.GetParameter(i).Key == "error_code")
        //                    {
        //                        epErrorCode = ids.ExpectedValues.GetParameter(i).Value;
        //                    }
        //                    else if (ids.ExpectedValues.GetParameter(i).Key == "error_message")
        //                    {
        //                        epErrorMessage = ids.ExpectedValues.GetParameter(i).Value;
        //                    }
        //                    else if (ids.ExpectedValues.GetParameter(i).Key == "processName")
        //                    {
        //                        processName = ids.ExpectedValues.GetParameter(i).Value;
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Step 1: Create Anaylisis
        //            if (isCreate)
        //            {
        //                CheckPoint cpCreateAnayisys = new CheckPoint("createAnalysis3D", "Call Analysis3DSvc.createAnalysis3D to create 3D analysis");
        //                r.CheckPoints.Add(cpCreateAnayisys);

        //                XMLResult rtCreateAnaylisis = analysis3DSvc.createAnalysis3D(p01_instanceUID, p02_xmlAnalysis3DInfo);

        //                if (rtCreateAnaylisis.IsErrorOccured)
        //                {
        //                    cpCreateAnayisys.Result = TestResult.Fail;
        //                    cpCreateAnayisys.Outputs.AddParameter("Create Analysis", "Result", "Fail. Return Message: " + rtCreateAnaylisis.ResultContent);
        //                }
        //                else
        //                {
        //                    cpCreateAnayisys.Result = TestResult.Pass;
        //                    cpCreateAnayisys.Outputs.AddParameter("Create Analysis", "Result", "Succeed");
        //                    objectIDList.Add(rtCreateAnaylisis.SingleResult);
        //                    pOpenObject.instanceIDList = objectIDList.ToArray();
        //                }
        //            }
        //            #endregion

        //            #region Step2 : Call ApplicationService.openObject
        //            CheckPoint cpOpenObject = new CheckPoint("openObject", "Call ApplicationService.openObject");
        //            r.CheckPoints.Add(cpOpenObject);

        //            //Send web service request
        //            AppOpenObjectsResponseType rtOpenObjects = applicationSvc.openObject(pOpenObject);

        //            if (applicationSvc.LastReturnXMLValidateResult.isValid)
        //            {
        //                if (rtOpenObjects.status.code.ToString() == epErrorCode && rtOpenObjects.status.message.Contains(epErrorMessage))
        //                {
        //                    cpOpenObject.Result = TestResult.Pass;
        //                    cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns correct: " + applicationSvc.LastReturnXML);
        //                }
        //                else
        //                {
        //                    cpOpenObject.Result = TestResult.Fail;
        //                    cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject returns error: " + applicationSvc.LastReturnXML);
        //                }
        //            }
        //            else
        //            {
        //                cpOpenObject.Result = TestResult.Fail;
        //                cpOpenObject.Outputs.AddParameter("OpenObject", "Check return value", "The openObject return invalid XML string: " + applicationSvc.LastReturnXML);
        //            }
        //            #endregion

        //            //Wait to start
        //            System.Threading.Thread.Sleep(3000);

        //            #region Step 2: Check the program instance is lanched or not
        //            CheckPoint cpProcess = new CheckPoint("openObject", "Check the application process after call openObject");
        //            r.CheckPoints.Add(cpProcess);

        //            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
        //            if (processes.Length == 0)
        //            {
        //                cpProcess.Result = TestResult.Pass;
        //                cpProcess.Outputs.AddParameter("OpenObject", "Check process", "The application is not running es expected after call openObject");
        //            }
        //            else
        //            {
        //                cpProcess.Result = TestResult.Fail;
        //                cpProcess.Outputs.AddParameter("OpenObject", "Check process", "The application is running unexpectedly after call openObject. The process number is: " + processes.Length);

        //                foreach (System.Diagnostics.Process p in processes)
        //                {
        //                    p.Kill();
        //                    System.Threading.Thread.Sleep(1000);
        //                }
        //            }
        //            #endregion

        //            #region Step 3: delete created analysis
        //            if (isCreate)
        //            {
        //                foreach (string id in objectIDList)
        //                {
        //                    XMLResult rtDeleteAnalysis = analysis3DSvc.deleteAnalysis3D(id);
        //                }
        //            }
        //            #endregion
        //            SaveRound(r);
        //        }
        //        catch (Exception ex)
        //        {
        //            CheckPoint cp = new CheckPoint();
        //            r.CheckPoints.Add(cp);
        //            cp.Outputs.AddParameter("Exception thrown", "Case Run", ex.Message + ex.InnerException == null ? "" : "Inner Exception: " + ex.InnerException.ToString());
        //            cp.Result = TestResult.Fail;
        //            SaveRound(r);
        //        }
        //    }

        //    Output();
        //}

    }
}

