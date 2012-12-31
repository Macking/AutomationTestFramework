using System;
using System.Collections.Generic;
using System.Text;
using PAS.AutoTest.TestUtility;
using PAS.AutoTest.TestData;

namespace PAS.AutoTest.PasATCoreV2
{
    public partial class GenericIntanceServiceV2 : ServiceBase
    {
        public GenericIntanceServiceV2()
        {
            this.InitialService("GenericInstanceV2");
        }

        public GenericGetGenericInstanceInfoResponseType getGenericInstanceInfo(string uid)
        {
            object svcReturn = this.InvokeMethod("getGenericInstanceInfo", new object[] { uid });

            //Verify the XML is valid or not per xsd file
            this.LastReturnXMLValidateResult = Utility.ValidateXMLPerXSD(this.lastReturnXML, @"XSDFiles\GenericInstanceService.xsd");

            return (GenericGetGenericInstanceInfoResponseType)this.DeserializeXMLToClass(svcReturn, typeof(GenericGetGenericInstanceInfoResponseType));
        }

        public GenericSetGenericInstanceInfoResponseType setGenericInstanceInfo(GenericSetGenericInstanceInfoRequestType request)
        {
            object svcRequest = GenarateRequestXML(request);
            object svcReturn = this.InvokeMethod("setGenericInstanceInfo", new object[] { svcRequest });

            //Verify the XML is valid or not per xsd file
            this.LastReturnXMLValidateResult = Utility.ValidateXMLPerXSD(this.lastReturnXML, @"XSDFiles\GenericInstanceService.xsd");

            return (GenericSetGenericInstanceInfoResponseType)this.DeserializeXMLToClass(svcReturn, typeof(GenericSetGenericInstanceInfoResponseType));
        }
    }

    public partial class GenericIntanceServiceV2 : ServiceBase
    {
        #region Private method
        private void CheckGetInfoDICOMResult(GenericGetGenericInstanceInfoResponseType rtGetInfo, GenericGetGenericInstanceInfoResponseType epGetInfo, CheckPoint cpGetInfo)
        {
            CompareObjects compareObjects = new CompareObjects();

            if (compareObjects.Compare(epGetInfo, rtGetInfo))
            {
                cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check value", "GetGenericInstanceInfo returns all expected values.");
                cpGetInfo.Result = TestResult.Pass;
            }
            else
            {
                cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check value", "GetGenericInstanceInfo doesn't return expected values." + compareObjects.DifferencesString);
                cpGetInfo.Result = TestResult.Fail;
            }

            //if (rtGetInfo.instance.dicomInfo.acquisitionDate != epGetInfo.instance.dicomInfo.acquisitionDate)
            //{
            //    cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check acquisitionDate in DICOM", "GetGenericInstanceInfo doesn't return expected acquisitionDate." + "Expect: " + epGetInfo.instance.dicomInfo.acquisitionDate + ". Get: " + rtGetInfo.instance.dicomInfo.acquisitionDate);
            //    cpGetInfo.Result = TestResult.Fail;
            //}
            //if (rtGetInfo.instance.dicomInfo.acquisitionTime != epGetInfo.instance.dicomInfo.acquisitionTime)
            //{
            //    cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check acquisitionTime in DICOM", "GetGenericInstanceInfo doesn't return expected acquisitionTime." + "Expect: " + epGetInfo.instance.dicomInfo.acquisitionTime + ". Get: " + rtGetInfo.instance.dicomInfo.acquisitionTime);
            //    cpGetInfo.Result = TestResult.Fail;
            //}
            //if (rtGetInfo.instance.dicomInfo.anatomicRegion != epGetInfo.instance.dicomInfo.anatomicRegion)
            //{
            //    cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check anatomicRegion in DICOM", "GetGenericInstanceInfo doesn't return expected anatomicRegion." + "Expect: " + epGetInfo.instance.dicomInfo.anatomicRegion + ". Get: " + rtGetInfo.instance.dicomInfo.anatomicRegion);
            //    cpGetInfo.Result = TestResult.Fail;
            //}
            //if (rtGetInfo.instance.dicomInfo.anatomicRegionModifier != epGetInfo.instance.dicomInfo.anatomicRegionModifier)
            //{
            //    cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check anatomicRegionModifier in DICOM", "GetGenericInstanceInfo doesn't return expected anatomicRegionModifier." + "Expect: " + epGetInfo.instance.dicomInfo.anatomicRegionModifier + ". Get: " + rtGetInfo.instance.dicomInfo.anatomicRegionModifier);
            //    cpGetInfo.Result = TestResult.Fail;
            //}

            //if (rtGetInfo.instance.dicomInfo.patientId != epGetInfo.instance.dicomInfo.patientId)
            //{
            //    cpGetInfo.Outputs.AddParameter("GetGenericInstanceInfo", "Check patientId in DICOM", "GetGenericInstanceInfo doesn't return expected patientId." + "Expect: " + epGetInfo.instance.dicomInfo.patientId + ". Get: " + rtGetInfo.instance.dicomInfo.patientId);
            //    cpGetInfo.Result = TestResult.Fail;
            //}
        }
        #endregion
    }
}
