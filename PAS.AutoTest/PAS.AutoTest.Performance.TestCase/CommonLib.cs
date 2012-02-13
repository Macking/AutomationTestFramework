using System;
using System.Collections.Generic;
using System.Text;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class CommonLib
    {
        public static string CreatePatient()
        {
            try
            {
                string PatientStarts = Math.Abs (Guid.NewGuid().GetHashCode()).ToString();
                PatientService ps = new PatientService();

                XMLParameter pa = new XMLParameter("patient");
                pa.AddParameter("patient_id", PatientStarts);
                pa.AddParameter("dpms_id", PatientStarts);
                pa.AddParameter("first_name", PatientStarts);
                pa.AddParameter("last_name", PatientStarts);
                pa.AddParameter("middle_name", PatientStarts);

                XMLResult result = ps.createPatient(pa);

                return result.SingleResult;
            }
            catch (Exception)
            { throw; }
        }

        public static string CreatePresentationState(string imageId)
        {
            try
            {
                PresentationStateService ps = new PresentationStateService();

                XMLParameter psInfo = new XMLParameter("presentationstate");
                psInfo.AddParameter("general.xml", "test text for general.xml", false);
                psInfo.AddParameter("processing.xml", "test text for processing.xml", false);
                psInfo.AddParameter("annotation.xml", "test text for annotation.xml", false);
                psInfo.AddParameter("current", "true");

                return ps.createPresentationState(imageId, psInfo).SingleResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
