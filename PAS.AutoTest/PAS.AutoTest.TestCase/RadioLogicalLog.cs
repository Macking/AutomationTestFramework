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
        public void Run_RadioLogicalLog_WorkFlow_N01_CreateGet_Case925() //Case 925: 1.3.3_WorkFlow_N01_CreateGet
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
                CheckPoint p3 = new CheckPoint("createRadioLogEntry", "Test createRadioLogEntry");
                CheckPoint p4 = new CheckPoint("getRadioLogEntry", "Test getRadioLogEntry");

                r.CheckPoints.Add(p1);
                r.CheckPoints.Add(p2);
                r.CheckPoints.Add(p3);
                r.CheckPoints.Add(p4);

                //create required PAS service instaces here
                PatientService pats = new PatientService();
                ImportService ims = new ImportService();
                RadiologService radios = new RadiologService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa1 = new XMLParameter("createPatient");
                XMLParameter pa2 = new XMLParameter("importObject");
                XMLParameter pa3 = new XMLParameter("createRadioLogEntry");
                XMLParameter pa4 = new XMLParameter("getRadioLogEntry");


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
                        case "createRadioLogEntry":
                            pa3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "getRadioLogEntry":
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
                pa2.AddParameter("patientInternalId", step1_result.SingleResult);
                pa3.AddParameter("patient_internal_id", step1_result.SingleResult);

                //Step2 result

                //if objectFileFullPath is empty string, skip step2
                if (pa2.GetParameterValueByName("objectFileFullPath") != null && pa2.GetParameterValueByName("objectFileFullPath") != "")
                {
                    XMLResult step2_result = ims.importObject(pa2.GetParameterValueByName("patientInternalId"), "", pa2.GetParameterValueByName("objectFileFullPath"), "", false, "");

                    //Log step2 service output
                    if (step2_result.IsErrorOccured)
                    {
                        p2.Result = TestResult.Fail;
                        p2.Outputs.AddParameter("importObject", "Error", step2_result.ResultContent);
                        SaveRound(r);
                        continue;
                    }

                    p2.Result = TestResult.Pass;
                    p2.Outputs.AddParameter("importObject", "Success", step2_result.ResultContent);


                    //Change input parameter according the output of Step2
                    // if step2 is skipped, step3 input has no need parameter of instance_internal_id
                    pa3.AddParameter("instance_internal_id", step2_result.ArrayResult.GetParameterValueByName("internal_id"));
                }
                else
                {
                    p2.Outputs.AddParameter("importObject", "Success", "This step is ignored by case, no run");
                    p2.Result = TestResult.Pass;
                }

                //Step3 result

                XMLResult step3_result = radios.createRadioLogEntry(pa3);

                //Log step3 service output
                if (step3_result.IsErrorOccured)
                {
                    p3.Result = TestResult.Fail;
                    p3.Outputs.AddParameter("createRadioLogEntry", "Error", step3_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p3.Result = TestResult.Pass;
                p3.Outputs.AddParameter("createRadioLogEntry", "Success", step3_result.ResultContent);


                //Change input parameter according the output of Step3
                pa4.AddParameter("radioLogEntryInternalId", step3_result.SingleResult);
                //Step4 result

                XMLResult step4_result = radios.getRadioLogEntry(pa4.GetParameterValueByName("radioLogEntryInternalId"));

                //Log step4 service output
                //Compare the input parameters of createRadioLogEntry and output result of getRadioLogEntry
                int compCount = 0;

                for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                {
                    if (ids.ExpectedValues.Parameters[index].Value == step4_result.ArrayResult.GetParameterValueByName(ids.ExpectedValues.Parameters[index].Key))
                    { compCount++; }
                }

                if (step4_result.IsErrorOccured || compCount != ids.ExpectedValues.Parameters.Count)
                {
                    p4.Result = TestResult.Fail;
                    p4.Outputs.AddParameter("getRadioLogEntry", "Error", step4_result.ResultContent);
                }
                else
                {
                    p4.Result = TestResult.Pass;
                    p4.Outputs.AddParameter("getRadioLogEntry", "Success", step4_result.ResultContent);
                }

                //Save data for each round
                SaveRound(r);
            }

            //Save service log as xml file
            Output();
        }

        public void Run_RadioLogicalLog_WorkFlow_N02_CreateDeleteGet_Case926() //Case 926: 1.3.3_WorkFlow_N02_CreateDeleteGet
        {
            //int runCount = this.Input.Repetition;

            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p1 = new CheckPoint("createPatient", "Setup environment of createPatient");
                CheckPoint p2 = new CheckPoint("createRadioLogEntry", "Setup environment of createRadioLogEntry");
                CheckPoint p3 = new CheckPoint("deleteRadioLogEntry", "Test deleteRadioLogEntry");
                CheckPoint p4 = new CheckPoint("getRadioLogEntry", "Test getRadioLogEntry");

                r.CheckPoints.Add(p1);
                r.CheckPoints.Add(p2);
                r.CheckPoints.Add(p3);
                r.CheckPoints.Add(p4);

                //create required PAS service instaces here
                PatientService pats = new PatientService();
                RadiologService radios = new RadiologService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa1 = new XMLParameter("createPatient");
                XMLParameter pa2 = new XMLParameter("createRadioLogEntry");
                XMLParameter pa3 = new XMLParameter("deleteRadioLogEntry");
                XMLParameter pa4 = new XMLParameter("getRadioLogEntry");


                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "createPatient":
                            pa1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "createRadioLogEntry":
                            pa2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "deleteRadioLogEntry":
                            pa3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "getRadioLogEntry":
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

                XMLResult step2_result = radios.createRadioLogEntry(pa2);

                //Log step2 service output
                if (step2_result.IsErrorOccured)
                {
                    p2.Result = TestResult.Fail;
                    p2.Outputs.AddParameter("createRadioLogEntry", "Error", step2_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p2.Result = TestResult.Pass;
                p2.Outputs.AddParameter("createRadioLogEntry", "Success", step2_result.ResultContent);


                //Change input parameter according the output of Step2
                pa3.AddParameter("radioLogEntryInternalId", step2_result.SingleResult);
                pa4.AddParameter("radioLogEntryInternalId", step2_result.SingleResult);


                //Step3 result

                XMLResult step3_result = radios.deleteRadioLogEntry(pa3.GetParameterValueByName("radioLogEntryInternalId"));

                //Log step3 service output
                if (step3_result.IsErrorOccured)
                {
                    p3.Result = TestResult.Fail;
                    p3.Outputs.AddParameter("deleteRadioLogEntry", "Error", step3_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p3.Result = TestResult.Pass;
                p3.Outputs.AddParameter("deleteRadioLogEntry", "Success", step3_result.ResultContent);


                //Change input parameter according the output of Step3

                //Step4 result

                XMLResult step4_result = radios.getRadioLogEntry(pa4.GetParameterValueByName("radioLogEntryInternalId"));

                //Log step4 service output

                if (step4_result.Code.ToString() == ids.ExpectedValues.GetParameter("code").Value)
                {
                    p4.Result = TestResult.Pass;
                    p4.Outputs.AddParameter("getRadioLogEntry", "Success", step4_result.ResultContent);
                }
                else
                {
                    p4.Result = TestResult.Fail;
                    p4.Outputs.AddParameter("getRadioLogEntry", "Error", step4_result.ResultContent);
                }

                //Save data for each round
                SaveRound(r);
            }

            //Save service log as xml file
            Output();
        }

        public void Run_RadioLogicalLog_WorkFlow_N03_CreateSetGet_Case927() //Case 927: 1.3.3_WorkFlow_N03_CreateSetGet
        {
            //int runCount = this.Input.Repetition;

            int runCount = 0; // this is the initial run count. Use this count to repeat different test data set.

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "Test round");

                //Mark the check point here to indicate what's the check point here
                CheckPoint p1 = new CheckPoint("createPatient", "Setup environment of createPatient");
                CheckPoint p2 = new CheckPoint("createRadioLogEntry", "Setup environment of createRadioLogEntry");
                CheckPoint p3 = new CheckPoint("setRadioLogEntry", "Test setRadioLogEntry");
                CheckPoint p4 = new CheckPoint("getRadioLogEntry", "Test getRadioLogEntry");

                r.CheckPoints.Add(p1);
                r.CheckPoints.Add(p2);
                r.CheckPoints.Add(p3);
                r.CheckPoints.Add(p4);

                //create required PAS service instaces here
                PatientService pats = new PatientService();
                RadiologService radios = new RadiologService();

                //create input parameters here, it may include XML path type and string type value
                XMLParameter pa1 = new XMLParameter("createPatient");
                XMLParameter pa2 = new XMLParameter("createRadioLogEntry");
                XMLParameter pa3 = new XMLParameter("setRadioLogEntry");
                XMLParameter pa4 = new XMLParameter("getRadioLogEntry");


                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "createPatient":
                            pa1.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "createRadioLogEntry":
                            pa2.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "setRadioLogEntry":
                            pa3.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                            break;
                        case "getRadioLogEntry":
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

                XMLResult step2_result = radios.createRadioLogEntry(pa2);

                //Log step2 service output
                if (step2_result.IsErrorOccured)
                {
                    p2.Result = TestResult.Fail;
                    p2.Outputs.AddParameter("createRadioLogEntry", "Error", step2_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p2.Result = TestResult.Pass;
                p2.Outputs.AddParameter("createRadioLogEntry", "Success", step2_result.ResultContent);


                //Change input parameter according the output of Step2


                //Step3 result

                XMLResult step3_result = radios.setRadioLogEntry(pa3, step2_result.SingleResult);

                //Log step3 service output
                if (step3_result.IsErrorOccured)
                {
                    p3.Result = TestResult.Fail;
                    p3.Outputs.AddParameter("setRadioLogEntry", "Error", step3_result.ResultContent);
                    SaveRound(r);
                    continue;
                }

                p3.Result = TestResult.Pass;
                p3.Outputs.AddParameter("setRadioLogEntry", "Success", step3_result.ResultContent);


                //Change input parameter according the output of Step3

                //Step4 result

                XMLResult step4_result = radios.getRadioLogEntry(step2_result.SingleResult);

                //Log step4 service output

                //Compare the Expected parameters of getRadioLogEntry and output result of getRadioLogEntry
                int compCount = 0;

                for (int index = 0; index < ids.ExpectedValues.Parameters.Count; index++)
                {
                    if (ids.ExpectedValues.Parameters[index].Value == step4_result.ArrayResult.GetParameterValueByName(ids.ExpectedValues.Parameters[index].Key))
                    { compCount++; }
                }

                if (step4_result.IsErrorOccured || compCount != ids.ExpectedValues.Parameters.Count)
                {
                    p4.Result = TestResult.Fail;
                    p4.Outputs.AddParameter("getRadioLogEntry", "Error", step4_result.ResultContent);
                }
                else
                {
                    p4.Result = TestResult.Pass;
                    p4.Outputs.AddParameter("getRadioLogEntry", "Success", step4_result.ResultContent);
                }

                //Save data for each round
                SaveRound(r);
            }

            //Save service log as xml file
            Output();
        }
    }
}