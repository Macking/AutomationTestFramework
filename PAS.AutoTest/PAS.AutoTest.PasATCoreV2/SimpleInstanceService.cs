using PAS.AutoTest.TestUtility;
using PAS.AutoTest.TestData;


namespace PAS.AutoTest.PasATCoreV2
{
    public partial class SimpleInstanceServiceV2 : ServiceBase
    {
        public SimpleInstanceServiceV2()
        {
            this.InitialService("SimpleInstanceV2");
        }

        private XMLValidateResult ValidateReturnXML()
        {
            return Utility.ValidateXMLPerXSD(base.lastReturnXML, @"XSDFiles\SimpleInstanceService.xsd");
        }

        public SimpleCreateSimpleInstanceResponseType createSimpleInstance(SimpleCreateSimpleInstanceRequestType request)
        {
            object svcReturn = this.InvokeMethod("createSimpleInstance", new object[] { GenarateRequestXML(request) });

            base.LastReturnXMLValidateResult = ValidateReturnXML(); //Verify the XML is valid or not per xsd file

            return (SimpleCreateSimpleInstanceResponseType)this.DeserializeXMLToClass(svcReturn, typeof(SimpleCreateSimpleInstanceResponseType));
        }

        public SimpleDeleteSimpleInstanceResponseType deleteSimpleInstance(string instanceID)
        {
            object svcReturn = this.InvokeMethod("deleteSimpleInstance", new object[] { instanceID });

            base.LastReturnXMLValidateResult = ValidateReturnXML(); //Verify the XML is valid or not per xsd file

            return (SimpleDeleteSimpleInstanceResponseType)this.DeserializeXMLToClass(svcReturn, typeof(SimpleDeleteSimpleInstanceResponseType));
        }

        public SimpleGetSimpleInstanceResponseType getSimpleInstance(string instanceID)
        {
            object svcReturn = this.InvokeMethod("getSimpleInstance", new object[] { instanceID });

            base.LastReturnXMLValidateResult = ValidateReturnXML(); //Verify the XML is valid or not per xsd file

            return (SimpleGetSimpleInstanceResponseType)this.DeserializeXMLToClass(svcReturn, typeof(SimpleGetSimpleInstanceResponseType));
        }

        public SimpleGetSimpleInstanceInfoResponseType getSimpleInstanceInfo(string instanceID)
        {
            object svcReturn = this.InvokeMethod("getSimpleInstanceInfo", new object[] { instanceID });

            base.LastReturnXMLValidateResult = ValidateReturnXML(); //Verify the XML is valid or not per xsd file

            return (SimpleGetSimpleInstanceInfoResponseType)this.DeserializeXMLToClass(svcReturn, typeof(SimpleGetSimpleInstanceInfoResponseType));
        }

        public SimpleSetSimpleInstanceInfoResponseType setSimpleInstanceInfo(SimpleSetSimpleInstanceInfoRequestType request)
        {
            object svcReturn = this.InvokeMethod("setSimpleInstanceInfo", new object[] { GenarateRequestXML(request) });

            base.LastReturnXMLValidateResult = ValidateReturnXML(); //Verify the XML is valid or not per xsd file

            return (SimpleSetSimpleInstanceInfoResponseType)this.DeserializeXMLToClass(svcReturn, typeof(SimpleSetSimpleInstanceInfoResponseType));
        }
    }

    public partial class SimpleInstanceServiceV2 : ServiceBase
    {
        #region Public utility methods
        /// <summary>
        /// Calls the create and check.
        /// </summary>
        /// <param name="r">The round r.</param>
        /// <param name="pCreate">The create parameter.</param>
        /// <returns>The instance ID</returns>
        public string CallCreateAndCheck(Round r, SimpleCreateSimpleInstanceRequestType pCreate)
        {
            string instanceUID = null;

            CheckPoint cpCreate = new CheckPoint("Create", "Call create to create a normal simple instance");
            r.CheckPoints.Add(cpCreate);

            SimpleCreateSimpleInstanceResponseType rtCreate = this.createSimpleInstance(pCreate);
            if (!this.LastReturnXMLValidateResult.isValid)
            {
                cpCreate.Result = TestResult.Fail;
                cpCreate.Outputs.AddParameter("Create Simple Instance", "XML Validation Result", "createSimpleInstance response is not complied with schema. Error:" + this.LastReturnXMLValidateResult.message);
                cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML:", this.LastReturnXML);
                return null;
            }
            else
            {
                if (rtCreate.status.code == 0 && rtCreate.status.message == "ok")
                {
                    cpCreate.Result = TestResult.Pass;
                    cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML correct:", this.LastReturnXML);
                }
                else
                {
                    cpCreate.Result = TestResult.Fail;
                    cpCreate.Outputs.AddParameter("Create Simple Instance", "Return XML wrong:", this.LastReturnXML);
                }

                instanceUID = rtCreate.simpleInstance.uid;
                return instanceUID;
            }
        }

        public string CallGetAndCheck(Round r, string instanceUID, SimpleGetSimpleInstanceResponseType epGet)
        {
            CheckPoint cpGetAfterImport = new CheckPoint("Get Simple Instance", "Get Simple Instance");
            r.CheckPoints.Add(cpGetAfterImport);

            SimpleGetSimpleInstanceResponseType rtGetAfterImport = this.getSimpleInstance(instanceUID);
            if (!this.LastReturnXMLValidateResult.isValid)
            {
                cpGetAfterImport.Result = TestResult.Fail;
                cpGetAfterImport.Outputs.AddParameter("Get Simple Instance", "Invalid XML", "GetSimpleInstance response is not complied with schema.");
                return this.LastReturnXML;
            }
            else
            {
                cpGetAfterImport.Result = TestResult.Pass; // Set to pass at first, then check the detail value
                cpGetAfterImport.Outputs.AddParameter("Get Simple Instance", "Return XML", "getSimpleInstance returns result. " + this.LastReturnXML);
            }

            if (rtGetAfterImport.status.code != 0 || rtGetAfterImport.status.message != "ok")
            {
                cpGetAfterImport.Result = TestResult.Fail;
                cpGetAfterImport.Outputs.AddParameter("Get Simple Instance", "Check the getSimpleInstance response", "getSimpleInstance doesn't return expected result.  Code:" + rtGetAfterImport.status.code.ToString() + "; Message:" + rtGetAfterImport.status.message);
                return this.LastReturnXML;
            }

            CheckGetResult(rtGetAfterImport, epGet, cpGetAfterImport);

            return this.LastReturnXML;
        }

        public string CallGetInfoAndCheck(Round r, string instanceUID, SimpleGetSimpleInstanceInfoResponseType epGetInfo)
        {
            CheckPoint cpGetInfoAfterImport = new CheckPoint("Get Simple Instance Info", "Get Simple Instance Info");
            r.CheckPoints.Add(cpGetInfoAfterImport);

            SimpleGetSimpleInstanceInfoResponseType rtGetInfoAfterImport = this.getSimpleInstanceInfo(instanceUID);
            if (!this.LastReturnXMLValidateResult.isValid)
            {
                cpGetInfoAfterImport.Result = TestResult.Fail;
                cpGetInfoAfterImport.Outputs.AddParameter("Get Simple Instance Info", "Invalid XML", "GetSimpleInstanceInfo response is not complied with schema.");
                return this.LastReturnXML;
            }
            else
            {
                cpGetInfoAfterImport.Result = TestResult.Pass; //Set to pass at first, then check the detail value
                cpGetInfoAfterImport.Outputs.AddParameter("Get Simple Instance Info", "Return XML", "GetSimpleInstanceInfo returns result. " + this.LastReturnXML);
            }

            if (rtGetInfoAfterImport.status.code != 0 || rtGetInfoAfterImport.status.message != "ok")
            {
                cpGetInfoAfterImport.Result = TestResult.Fail;
                cpGetInfoAfterImport.Outputs.AddParameter("Get Simple Instance Info", "Check the GetSimpleInstanceInfo response", "GetSimpleInstanceInfo doesn't return expected result.  Code:" + rtGetInfoAfterImport.status.code.ToString() + "; Message:" + rtGetInfoAfterImport.status.message);
                return this.LastReturnXML;
            }

            CheckGetInfoResult(rtGetInfoAfterImport, epGetInfo, cpGetInfoAfterImport);

            return this.LastReturnXML;
        }

        public string CallSetAndCheck(Round r, SimpleSetSimpleInstanceInfoRequestType pSet)
        {
            CheckPoint cpSet = new CheckPoint("Set", "Call setSimpleInstance to change property");
            r.CheckPoints.Add(cpSet);

            SimpleSetSimpleInstanceInfoResponseType rtSet = this.setSimpleInstanceInfo(pSet);
            if (!this.LastReturnXMLValidateResult.isValid)
            {
                cpSet.Result = TestResult.Fail;
                cpSet.Outputs.AddParameter("Set Simple Instance", "Invalid XML", "setSimpleInstanceInfo response is not complied with schema.");
                return null;
            }

            if (rtSet.status.code != 0 || rtSet.status.message != "ok")
            {
                cpSet.Result = TestResult.Fail;
                cpSet.Outputs.AddParameter("Set Simple Instance", "Set Simple Instance return error", " Code: " + rtSet.status.code.ToString() + "; Message: " + rtSet.status.message);
            }
            else
            {
                cpSet.Result = TestResult.Pass;
                cpSet.Outputs.AddParameter("Set Simple Instance", "Set Simple Instance return success", " Code: " + rtSet.status.code.ToString() + "; Message: " + rtSet.status.message);
            }

            return this.LastReturnXML;
        }

        public string CallDeleteAndCheck(Round r, string instanceUID)
        {
            CheckPoint cpDelete = new CheckPoint("Delete", "Call SimpleInstanceSvc.deleteSimpleInstance to delete");
            r.CheckPoints.Add(cpDelete);

            SimpleDeleteSimpleInstanceResponseType rtDelete = this.deleteSimpleInstance(instanceUID);

            if (!this.LastReturnXMLValidateResult.isValid)
            {
                cpDelete.Outputs.AddParameter("Delete Simple Instance", "Invalid XML", "deleteSimpleInstance response is not complied with schema.");
                cpDelete.Result = TestResult.Fail;
                return null;
            }
            else
            {
                if (rtDelete.status.code == 0 && rtDelete.status.message == "ok")
                {
                    cpDelete.Outputs.AddParameter("Delete Simple Instance", "return success", "deleteSimpleInstance response is correct. Get: " + this.LastReturnXML);
                    cpDelete.Result = TestResult.Pass;
                }
                else
                {
                    cpDelete.Outputs.AddParameter("Delete Simple Instance", "return error", "deleteSimpleInstance response is wrong. Get: " + this.LastReturnXML);
                    cpDelete.Result = TestResult.Fail;
                }

                return this.LastReturnXML;
            }
        }
        #endregion


        #region Private method
        private void CheckGetResult(SimpleGetSimpleInstanceResponseType rtGet, SimpleGetSimpleInstanceResponseType epGet, CheckPoint cpGet)
        {
            // add a special logic to deal with path
            if (epGet.simpleInstance.filePath != null) // Todo: need to consider normal and archive
            {
                if (rtGet.simpleInstance.filePath == null || !rtGet.simpleInstance.filePath.Contains(Utility.GetCSDMConfig(CSDMConfigSection.local, "patientDirectory")) || !rtGet.simpleInstance.filePath.Contains(epGet.simpleInstance.patientUid))
                {
                    cpGet.Outputs.AddParameter("Get Simple Instance", "Check filePath", "getSimpleInstance doesn't return expected filePath." + ". Get: " + rtGet.simpleInstance.filePath);
                    cpGet.Result = TestResult.Fail;
                    return;
                }
                else
                {
                    epGet.simpleInstance.filePath = null; // set it to null and go on to compare others
                }
            }
            if (!Utility.IsTimeEqualNow(rtGet.simpleInstance.creationDateTime))
            {
                cpGet.Outputs.AddParameter("Get Simple Instance", "Check creationDateTime", "getSimpleInstance doesn't return expected creationDateTime." + "Expect: " + epGet.simpleInstance.creationDateTime + ". Get: " + rtGet.simpleInstance.creationDateTime);
                cpGet.Result = TestResult.Fail;
                return;
            }
            else
            {
                epGet.simpleInstance.creationDateTime = null;
            }
            if (!Utility.IsTimeEqualNow(rtGet.simpleInstance.lastUpdateDateTime))
            {
                cpGet.Outputs.AddParameter("Get Simple Instance", "Check lastUpdateDateTime", "getSimpleInstance doesn't return expected lastUpdateDateTime." + "Expect: " + epGet.simpleInstance.lastUpdateDateTime + ". Get: " + rtGet.simpleInstance.lastUpdateDateTime);
                cpGet.Result = TestResult.Fail;
                return;
            }
            else
            {
                epGet.simpleInstance.lastUpdateDateTime = null;
            }

            // Compare others
            CompareObjects compareObjects = new CompareObjects();
            if (compareObjects.Compare(epGet, rtGet))
            {
                cpGet.Outputs.AddParameter("Get Simple Instance", "Check value", "GetSimpleInstance returns all expected values.");
                cpGet.Result = TestResult.Pass;
            }
            else
            {
                cpGet.Outputs.AddParameter("Get Simple Instance", "Check value", "GetSimpleInstance doesn't return expected values." + compareObjects.DifferencesString);
                cpGet.Result = TestResult.Fail;
            }
        }

        private void CheckGetInfoResult(SimpleGetSimpleInstanceInfoResponseType rtGetInfo, SimpleGetSimpleInstanceInfoResponseType epGetInfo, CheckPoint cpGetInfo)
        {
            if (rtGetInfo.simpleInstance.filePath != null) // Note: there should be no filePath in getInfo return
            {
                cpGetInfo.Outputs.AddParameter("Get Simple Instance Info", "Check filePath", "GetSimpleInstanceInfo doesn't return expected filePath." + "Expect there is no filePath in the return. " + ". Get: " + rtGetInfo.simpleInstance.filePath);
                cpGetInfo.Result = TestResult.Fail;
                return;
            }
            else
            {
                epGetInfo.simpleInstance.filePath = null;
            }

            if (!Utility.IsTimeEqualNow(rtGetInfo.simpleInstance.creationDateTime))
            {
                cpGetInfo.Outputs.AddParameter("Get Simple Instance Info", "Check creationDateTime", "GetSimpleInstanceInfo doesn't return expected creationDateTime." + "Expect: " + epGetInfo.simpleInstance.creationDateTime + ". Get: " + rtGetInfo.simpleInstance.creationDateTime);
                cpGetInfo.Result = TestResult.Fail;
                return;
            }
            else
            {
                epGetInfo.simpleInstance.creationDateTime = null;
            }

            if (!Utility.IsTimeEqualNow(rtGetInfo.simpleInstance.lastUpdateDateTime))
            {
                cpGetInfo.Outputs.AddParameter("Get Simple Instance Info", "Check lastUpdateDateTime", "GetSimpleInstanceInfo doesn't return expected lastUpdateDateTime." + "Expect: " + epGetInfo.simpleInstance.lastUpdateDateTime + ". Get: " + rtGetInfo.simpleInstance.lastUpdateDateTime);
                cpGetInfo.Result = TestResult.Fail;
                return;
            }
            else
            {
                epGetInfo.simpleInstance.lastUpdateDateTime = null;
            }

            //Compare others
            CompareObjects compareObjects = new CompareObjects();

            if (compareObjects.Compare(epGetInfo, rtGetInfo))
            {
                cpGetInfo.Outputs.AddParameter("Get Simple Instance Info", "Check value", "GetSimpleInstanceInfo returns all expected values.");
                cpGetInfo.Result = TestResult.Pass;
            }
            else
            {
                cpGetInfo.Outputs.AddParameter("Get Simple Instance Info", "Check value", "GetSimpleInstanceInfo doesn't return expected values." + compareObjects.DifferencesString);
                cpGetInfo.Result = TestResult.Fail;
            }
        }
        #endregion
    }
}
