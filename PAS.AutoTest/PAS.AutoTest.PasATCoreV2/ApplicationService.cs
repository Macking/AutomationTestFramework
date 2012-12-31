using PAS.AutoTest.TestUtility;

namespace PAS.AutoTest.PasATCoreV2
{
    public class ApplicationServiceV2:ServiceBase
    {
        public ApplicationServiceV2()
        {
            this.InitialService("ApplicationV2");
        }

        public AppOpenObjectsResponseType openObject(AppOpenObjectsRequestType request)
        {
            object svcReturn = this.InvokeMethod("openObjects", new object[] { GenarateRequestXML(request) });

            //Verify the XML is valid or not per xsd file
            this.LastReturnXMLValidateResult = Utility.ValidateXMLPerXSD(this.lastReturnXML, @"XSDFiles\ApplicationOpenObjectsResponse.xsd");

            return (AppOpenObjectsResponseType)this.DeserializeXMLToClass(svcReturn, typeof(AppOpenObjectsResponseType));
        }
    }
}
