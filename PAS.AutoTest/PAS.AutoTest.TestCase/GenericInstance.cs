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
        public void Run_GenericInstance_SetGetTag_Case1579()  // Case 1579: 1.1.05.04_Set and Get Tags_Normal, added for check the set and get function to Generic Instance tags
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    GenericIntanceServiceV2 genericInstanceSvc = new GenericIntanceServiceV2();

                    string instanceUID = string.Empty;
                    GenericSetGenericInstanceInfoRequestType setRequest = new GenericSetGenericInstanceInfoRequestType();
                    setRequest.instance = new InstanceType();
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "setInstanceInfo")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                setRequest.instance.uid = ids.InputParameters.GetParameter(i).Value;
                            }
                            else if (ids.InputParameters.GetParameter(i).Key == "tags")
                            {
                                setRequest.instance.tags = ids.InputParameters.GetParameter(i).Value; ;
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

                    CheckPoint pSetTags = new CheckPoint("Set Tags", "Set Tags");
                    r.CheckPoints.Add(pSetTags);

                    GenericSetGenericInstanceInfoResponseType setResponse = genericInstanceSvc.setGenericInstanceInfo(setRequest);
                    if (!genericInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        pSetTags.Result = TestResult.Fail;
                        pSetTags.Outputs.AddParameter("Set Tags", "Invalid XML", "Set response is not complied with schema.");
                        SaveRound(r);
                        continue;
                    }

                    if (setResponse.status.code.ToString() != epCode || !setResponse.status.message.Contains(epMessage))
                    {
                        pSetTags.Result = TestResult.Fail;
                        pSetTags.Outputs.AddParameter("Set Tags", "Check the set response", "Set doesn't return expected result.  Code:" + setResponse.status.code.ToString() + "; Message:" + setResponse.status.message);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pSetTags.Result = TestResult.Pass;
                        pSetTags.Outputs.AddParameter("Set Tags", "Check the set response", "Set returns expected result. Code:" + setResponse.status.code.ToString() + "; Message:" + setResponse.status.message);
                    }

                    CheckPoint pGetTags = new CheckPoint("Get Tags", "Get Tags");
                    r.CheckPoints.Add(pGetTags);

                    instanceUID = setRequest.instance.uid;
                    GenericGetGenericInstanceInfoResponseType getResponse = genericInstanceSvc.getGenericInstanceInfo(instanceUID);
                    if (!genericInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        pGetTags.Result = TestResult.Fail;
                        pGetTags.Outputs.AddParameter("Get Tags", "Invalid XML", "Get response is not complied with schema. Actually get: " + genericInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }

                    if (getResponse.status.code != 0)
                    {
                        pGetTags.Result = TestResult.Fail;
                        pGetTags.Outputs.AddParameter("Get Tags", "Get Fail", "Get Fail.Code:" + getResponse.status.code.ToString() + " Message:" + getResponse.status.message);
                        SaveRound(r);
                        continue;
                    }

                    if (setRequest.instance.tags != getResponse.instance.tags)
                    {
                        pGetTags.Result = TestResult.Fail;
                        pGetTags.Outputs.AddParameter("Get Tags", "Get Fail", "Get value is not equal to set value." + "Expect: " + setRequest.instance.tags + "; Actually get: " + getResponse.instance.tags);
                        SaveRound(r);
                        continue;
                    }
                    else
                    {
                        pGetTags.Result = TestResult.Pass;
                        pGetTags.Outputs.AddParameter("Get Tags", "Get Success", genericInstanceSvc.LastReturnXML);
                    }

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

        public void Run_GenericInstance_InvalidInstanceId_Case1568()
        {
            int runCount = 0;
            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "SetGetCommentsV2");
                GenericIntanceServiceV2 gisv2 = new GenericIntanceServiceV2();

                GenericSetGenericInstanceInfoRequestType setRequest = new GenericSetGenericInstanceInfoRequestType();
                setRequest.instance = new InstanceType();
                string instanceid = ids.InputParameters.GetParameter("instanceid").Value;
                int expectcode = int.Parse(ids.ExpectedValues.GetParameter("expectcode").Value);
                setRequest.instance.uid = instanceid;
                setRequest.instance.comments = "Test";

                GenericGetGenericInstanceInfoResponseType getResp = gisv2.getGenericInstanceInfo(instanceid);

                CheckPoint pComments = new CheckPoint("GetSet Comments", "GetSet Comments Invalid Instance id");
                r.CheckPoints.Add(pComments);
                if (getResp.status.code != 1003)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Get Fail", "Code not expected, actual:" + getResp.status.code.ToString() + ", expect:1003");
                    SaveRound(r);
                    continue;
                }

                if (!gisv2.LastReturnXMLValidateResult.isValid)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Get Set Fail", "Response is not complied with Schema");
                    SaveRound(r);
                    continue;
                }


                GenericSetGenericInstanceInfoResponseType setResp = gisv2.setGenericInstanceInfo(setRequest);
                if (setResp.status.code != 1003)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Set Fail", "Code not expected, actual:" + setResp.status.code.ToString() + ", expect:1003");
                    SaveRound(r);
                    continue;
                }

                if (!gisv2.LastReturnXMLValidateResult.isValid)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Get Set Fail", "Response is not complied with Schema");
                    SaveRound(r);
                    continue;
                }

                pComments.Result = TestResult.Pass;
                pComments.Outputs.AddParameter("GetSet Comments Invalid instance id", "Success", "OK");
                SaveRound(r);

            }

            Output();

        }

        public void Run_GenericInstance_SetGetComment_Case1551()
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "SetGetCommentsV2");
                GenericIntanceServiceV2 gisv2 = new GenericIntanceServiceV2();

                GenericSetGenericInstanceInfoRequestType setRequest = new GenericSetGenericInstanceInfoRequestType();
                setRequest.instance = new InstanceType();
                string getInstanceid = string.Empty;
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    switch (ids.InputParameters.GetParameter(i).Step)
                    {
                        case "setInstanceInfo":
                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "instanceid":
                                    setRequest.instance.uid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                                case "comments":
                                    setRequest.instance.comments = ids.InputParameters.GetParameter(i).Value;
                                    break;
                            }
                            break;
                        case "getInstanceInfo":

                            switch (ids.InputParameters.GetParameter(i).Key)
                            {
                                case "instanceid":
                                    getInstanceid = ids.InputParameters.GetParameter(i).Value;
                                    break;
                            }
                            break;
                    }
                }

                CheckPoint pComments = new CheckPoint("GetSet Comments", "GetSet Comments");
                r.CheckPoints.Add(pComments);

                GenericSetGenericInstanceInfoResponseType setResponse = gisv2.setGenericInstanceInfo(setRequest);
                if (!gisv2.LastReturnXMLValidateResult.isValid)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Invalid XML", "Set response is not complied with schema.");
                    SaveRound(r);
                    continue;
                }

                if (setResponse.status.code != 0)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Set Fail", "Set Fail.Code:" + setResponse.status.code.ToString() + " Message:" + setResponse.status.message);
                    SaveRound(r);
                    continue;
                }
                GenericGetGenericInstanceInfoResponseType getResponse = gisv2.getGenericInstanceInfo(getInstanceid);
                if (!gisv2.LastReturnXMLValidateResult.isValid)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Invalid XML", "Get response is not complied with schema.");
                    SaveRound(r);
                    continue;
                }
                if (getResponse.status.code != 0)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "Get Fail", "Get Fail.Code:" + getResponse.status.code.ToString() + " Message:" + getResponse.status.message);
                    SaveRound(r);
                    continue;
                }
                if (setRequest.instance.comments != getResponse.instance.comments)
                {
                    pComments.Result = TestResult.Fail;
                    pComments.Outputs.AddParameter("GetSet Comments", "GetSet Fail", "Set value is not equal to get value");
                    SaveRound(r);
                    continue;
                }
                else
                {
                    pComments.Result = TestResult.Pass;
                    pComments.Outputs.AddParameter("GetSet Comments", "GetSet Success", "OK");
                    //cleanup the comments value
                    setRequest.instance.comments = "Initialized Comments";
                    gisv2.setGenericInstanceInfo(setRequest);
                    SaveRound(r);
                }

            }
            Output();

        }
        
        public void Run_GenericInstance_GetInfoForDICOM_Case1628()  // Case 1628: 1.1.05.05_Call getGenericInstanceInfo to check the Dicom_Info for dicom instance
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), ids.Description);

                try
                {
                    string instanceUID = string.Empty;
                    for (int i = 0; i < ids.InputParameters.Count; i++)
                    {
                        if (ids.InputParameters.GetParameter(i).Step == "getInstanceInfo")
                        {
                            if (ids.InputParameters.GetParameter(i).Key == "instanceID")
                            {
                                instanceUID = ids.InputParameters.GetParameter(i).Value;
                            }
                        }
                    }

                    string epCode = string.Empty;
                    string epMessage = string.Empty;
                    GenericGetGenericInstanceInfoResponseType epGetGenericInstanceInfo = new GenericGetGenericInstanceInfoResponseType();
                    epGetGenericInstanceInfo.instance = new InstanceType();
                    epGetGenericInstanceInfo.instance.dicomInfo = new DicomInfoType();
                    for (int i = 0; i < ids.ExpectedValues.Count; i++)
                    {
                        switch (ids.ExpectedValues.GetParameter(i).Key)
                        {
                            case "code":
                                epCode = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "message":
                                epMessage = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_modality":
                                epGetGenericInstanceInfo.instance.dicomInfo.modality = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_acquisitionDate":
                                epGetGenericInstanceInfo.instance.dicomInfo.acquisitionDate = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_acquisitionTime":
                                epGetGenericInstanceInfo.instance.dicomInfo.acquisitionTime = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_patientFirstName":
                                epGetGenericInstanceInfo.instance.dicomInfo.patientFirstName = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_patientLastName":
                                epGetGenericInstanceInfo.instance.dicomInfo.patientLastName = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_patientId":
                                epGetGenericInstanceInfo.instance.dicomInfo.patientId = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_patientBirthDate":
                                epGetGenericInstanceInfo.instance.dicomInfo.patientBirthDate = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_patientAge":
                                epGetGenericInstanceInfo.instance.dicomInfo.patientAge = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_studyDate":
                                epGetGenericInstanceInfo.instance.dicomInfo.studyDate = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_kvp":
                                epGetGenericInstanceInfo.instance.dicomInfo.kvp = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_xrayTubeCurrent":
                                epGetGenericInstanceInfo.instance.dicomInfo.xrayTubeCurrent = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_exposureTime":
                                epGetGenericInstanceInfo.instance.dicomInfo.exposureTime = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_sopClassUid":
                                epGetGenericInstanceInfo.instance.dicomInfo.sopClassUid = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_instanceNumber":
                                epGetGenericInstanceInfo.instance.dicomInfo.instanceNumber = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_manufacturerModelName":
                                epGetGenericInstanceInfo.instance.dicomInfo.manufacturerModelName = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_imagerPixelSpacing":
                                epGetGenericInstanceInfo.instance.dicomInfo.imagerPixelSpacing = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_transferSyntax":
                                epGetGenericInstanceInfo.instance.dicomInfo.transferSyntax = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            case "dcm_anatomicRegion":
                                epGetGenericInstanceInfo.instance.dicomInfo.anatomicRegion = ids.ExpectedValues.GetParameter(i).Value;
                                break;
                            default:
                                break;
                        }
                    }

                    GenericIntanceServiceV2 genericInstanceSvc = new GenericIntanceServiceV2();

                    CheckPoint cpGetGenericInstanceInfo = new CheckPoint("getGenericInstanceInfo", "getGenericInstanceInfo");
                    r.CheckPoints.Add(cpGetGenericInstanceInfo);

                    GenericGetGenericInstanceInfoResponseType rtGetGenericInstanceInfo = genericInstanceSvc.getGenericInstanceInfo(instanceUID);
                    if (!genericInstanceSvc.LastReturnXMLValidateResult.isValid)
                    {
                        cpGetGenericInstanceInfo.Result = TestResult.Fail;
                        cpGetGenericInstanceInfo.Outputs.AddParameter("getGenericInstanceInfo", "Invalid XML", "GetGenericInstanceInfo response is not complied with schema. Actually get: " + genericInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }

                    if (rtGetGenericInstanceInfo.status.code != 0)
                    {
                        cpGetGenericInstanceInfo.Result = TestResult.Fail;
                        cpGetGenericInstanceInfo.Outputs.AddParameter("getGenericInstanceInfo", "Get Fail", "Get Fail.Code:" + rtGetGenericInstanceInfo.status.code.ToString() + " Message:" + rtGetGenericInstanceInfo.status.message);
                        SaveRound(r);
                        continue;
                    }

                    CompareObjects compareObjects = new CompareObjects();
                    if (compareObjects.Compare(epGetGenericInstanceInfo.instance.dicomInfo, rtGetGenericInstanceInfo.instance.dicomInfo))
                    {
                        cpGetGenericInstanceInfo.Result = TestResult.Pass;
                        cpGetGenericInstanceInfo.Outputs.AddParameter("getGenericInstanceInfo", "Check the DICOM Info", "The return value is equal to expected value" + genericInstanceSvc.LastReturnXML);
                    }
                    else
                    {
                        cpGetGenericInstanceInfo.Result = TestResult.Fail;
                        cpGetGenericInstanceInfo.Outputs.AddParameter("getGenericInstanceInfo", "Check the DICOM Info", "Fail. The return values is not equal to expected value." + compareObjects.DifferencesString + ". Actually get: " + genericInstanceSvc.LastReturnXML);
                        SaveRound(r);
                        continue;
                    }

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

        public void Run_GenericInstance_ListInstances_Case1657() // Case 1657: 1.1.05.06_Call listInstances to volume to list child and slibling
        {
            int runCount = 0;

            foreach (InputDataSet ids in this.Input.DataSets)
            {
                runCount++;
                Round r = this.NewRound(runCount.ToString(), "List Instances");

                CheckPoint listCheck = new CheckPoint("List Instances", "Test description");
                r.DataSet = ids;
                r.CheckPoints.Add(listCheck);

                GenericInstanceService instances = new GenericInstanceService();
                XMLParameter pa = new XMLParameter("filter");
                for (int i = 0; i < ids.InputParameters.Count; i++)
                {
                    if (ids.InputParameters.GetParameter(i).Step == "listInstances")
                    {
                        pa.AddParameter(ids.InputParameters.GetParameter(i).Key, ids.InputParameters.GetParameter(i).Value);
                    }

                }
                XMLResult result = instances.listInstances(pa);
                if (result.IsErrorOccured)
                {
                    listCheck.Result = TestResult.Fail;
                    listCheck.Outputs.AddParameter("listInstances fail", "list Instances", result.Message);
                }
                else
                {
                    listCheck.Outputs.AddParameter("listInstances OK", "list Instances", result.ResultContent);
                    System.Xml.XmlDocument xmlResult = new System.Xml.XmlDocument();
                    xmlResult.LoadXml(result.ResultContent);

                    System.Xml.XmlNodeList instanceList = xmlResult.SelectNodes("trophy/instance/parameter");
                    listCheck.Result = TestResult.Pass;

                    List<int> findedGroup = new List<int>();
                    int nodeIndex = 0;
                    foreach (System.Xml.XmlNode instance in instanceList)
                    {
                        int matchStep = 0;
                        nodeIndex++;
                        foreach (Parameter pExpected in ids.ExpectedValues.Parameters)
                        {
                            int iExpIdx = 0;
                            if (System.Int32.TryParse(pExpected.Step, out iExpIdx))
                            {
                                if (pExpected.Key == "value"
                                    && pExpected.Value == instance.Attributes["value"].Value
                                     )
                                    matchStep = iExpIdx;
                            }
                        }
                        if (matchStep > 0)
                        {
                            bool matchKey = false;
                            bool matchParentID = false;
                            foreach (Parameter pExpected in ids.ExpectedValues.Parameters)
                            {
                                int iExpIdx = 0;
                                if (System.Int32.TryParse(pExpected.Step, out iExpIdx))
                                {
                                    if (pExpected.Key == "key"
                                       && pExpected.Value == instance.Attributes["key"].Value && iExpIdx == matchStep
                                        )
                                        matchKey = true;
                                    else if (pExpected.Key == "parent_id"
                                      && pExpected.Value == instance.Attributes["parent_id"].Value && iExpIdx == matchStep
                                       )
                                        matchParentID = true;
                                }
                            }
                            if (matchKey && matchParentID)
                            {
                            }
                            else
                            {
                                listCheck.Result = TestResult.Fail;
                                string key = nodeIndex + " key or parent id unmatched";
                                listCheck.Outputs.AddParameter("listInstances", key, instance.OuterXml);
                            }
                            findedGroup.Add(matchStep);
                        }
                        else
                        {
                            listCheck.Result = TestResult.Fail;
                            string key = nodeIndex + " fail to find the instance in expected results";
                            listCheck.Outputs.AddParameter("listInstances", key, instance.InnerXml);
                        }
                    }
                    foreach (Parameter pExpected in ids.ExpectedValues.Parameters)
                    {
                        int iExpIdx = 0;
                        if (System.Int32.TryParse(pExpected.Step, out iExpIdx))
                        {
                            if (pExpected.Key == "value")
                            {
                                bool bfinded = false;
                                foreach (int matchStep in findedGroup)
                                {
                                    if (iExpIdx == matchStep)
                                    {
                                        bfinded = true;
                                        break;
                                    }
                                }
                                if (bfinded)
                                {
                                }
                                else
                                {
                                    listCheck.Result = TestResult.Fail;
                                    string key = iExpIdx + " fail to find the instance in expected results";
                                    listCheck.Outputs.AddParameter("listInstances", key, pExpected.Value);
                                }
                            }
                        }
                    }
                }
                SaveRound(r);
            }
            Output();
        }
    }
}