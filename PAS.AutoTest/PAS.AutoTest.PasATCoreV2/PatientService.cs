using PAS.AutoTest.TestUtility;

namespace PAS.AutoTest.PasATCoreV2
{
    public class NewPatientService : ServiceBase
    {
        public NewPatientService()
        {
            this.InitialService("PatientV2");
        }

        public PatientListObjectsResponseType listObjects(PatientListObjectsRequestType request)
        {
            // Call web service patientSvc.listObjects, return an XML string
            object svcReturn = this.InvokeMethod("listObjects", new object[] { GenarateRequestXML(request) });

            //Verify the XML is valid or not per xsd file
            this.LastReturnXMLValidateResult = Utility.ValidateXMLPerXSD(this.lastReturnXML, @"XSDFiles\PatientListObjectsResponse.xsd");

            return (PatientListObjectsResponseType)this.DeserializeXMLToClass(svcReturn, typeof(PatientListObjectsResponseType));
        }
    }
}
