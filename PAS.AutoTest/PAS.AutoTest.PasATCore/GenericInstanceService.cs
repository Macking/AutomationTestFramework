using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PAS.AutoTest.TestData;

namespace PAS.AutoTest.PasATCore
{
    public partial class GenericInstanceService : PASBase
    {
        public GenericInstanceService()
        {
            this.InitialService("GenericInstance");
        }

        public XMLResult linkInstance(string parentInstanceUid, string childInstanceUid)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("linkInstance", new object[] { parentInstanceUid, childInstanceUid }));
            return this.lastResult;
        }

        public XMLResult linkInstance(XMLParameter linkInformation)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("linkInstance", new object[] { linkInformation.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult listInstances(string parentInstanceUid, string childInstanceType)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listInstances", new object[] { parentInstanceUid, childInstanceType }));
            return this.lastResult;
        }

        public XMLResult listInstances(XMLParameter listInfomation)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listInstances", new object[] { listInfomation.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult unlinkInstance(string parentInstanceUid, string childInstanceUid)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("unlinkInstance", new object[] { parentInstanceUid, childInstanceUid }));
            return this.lastResult;
        }

        public XMLResult moveInstance(string instanceUid, string targetedPatientUid)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("moveInstance", new object[] { instanceUid, targetedPatientUid }));
            return this.lastResult;
        }
    }

    public partial class GenericInstanceService : PASBase
    {
        #region Public utility methods
        public void DoLogiconProcess(Round r, string imageID)
        {
            CheckPoint cpCreateLogiconReport = new CheckPoint("Create Logicon Report", "Create Logicon Report");
            r.CheckPoints.Add(cpCreateLogiconReport);

            // Step 1: Get the patient Info
            ImageService imageSvc = new ImageService();
            XMLParameter pGetImageInfo = new XMLParameter("image");
            pGetImageInfo.AddParameter("internal_id", imageID);
            string rt1 = imageSvc.getImageInfo(pGetImageInfo).ResultContent;

            if (!rt1.Contains("status code=\"0\""))
            {
                cpCreateLogiconReport.Result = TestResult.Fail;
                return;
            }

            string patientID = null;
            rt1 = rt1.Replace("<dicom_info>", "");
            rt1 = rt1.Replace("</dicom_info>", "");
            XMLResult xmlRt = new XMLResult(rt1);

            for (int j = 0; j < xmlRt.ArrayResult.Parameters.Count; j++)
            {
                if (xmlRt.ArrayResult.Parameters[j].ParameterName == "patient_internal_id")
                {
                    patientID = xmlRt.ArrayResult.Parameters[j].ParameterValue;
                    break;
                }
            }

            if (patientID == null)
            {
                cpCreateLogiconReport.Result = TestResult.Fail;
                return;
            }

            // Step 2: Create a simple instance with type "logicon_report"
            SimpleInstanceService simpleInstanceSvc = new SimpleInstanceService();

            if (!System.IO.File.Exists(@"C:\LogiconReport.txt"))
            {
                try
                {
                    using (System.IO.FileStream fs = System.IO.File.Create(@"C:\LogiconReport.txt"))
                    {
                    }
                }
                catch (Exception)
                {
                    cpCreateLogiconReport.Result = TestResult.Fail;
                    return;
                }
            }

            XMLParameter pCreate = new XMLParameter("instance");
            pCreate.AddParameter("patient_internal_id", patientID);
            pCreate.AddParameter("path", @"C:\LogiconReport.txt");
            pCreate.AddParameter("instance_type", "logicon_report");
            string rt2 = simpleInstanceSvc.createSimpleInstance(pCreate).ResultContent;
            if (!rt2.Contains("status code=\"0\""))
            {
                cpCreateLogiconReport.Result = TestResult.Fail;
                return;
            }

            string simpelInstanceID = null;
            xmlRt = new XMLResult(rt2);

            for (int j = 0; j < xmlRt.ArrayResult.Parameters.Count; j++)
            {
                if (xmlRt.ArrayResult.Parameters[j].ParameterName == "internal_id")
                {
                    simpelInstanceID = xmlRt.ArrayResult.Parameters[j].ParameterValue;
                    break;
                }
            }

            if (simpelInstanceID == null)
            {
                cpCreateLogiconReport.Result = TestResult.Fail;
                return;
            }

            // Step3: Link the "LogiconReport" simple instance to the image as child
            GenericInstanceService genericInstanceSvc = new GenericInstanceService();

            XMLParameter pLink = new XMLParameter("link_information");
            pLink.AddParameter("parent_instance_internal_id", imageID);
            pLink.AddParameter("child_instance_internal_id", simpelInstanceID);
            pLink.AddParameter("is_flat_link", "false");

            string rt3 = genericInstanceSvc.linkInstance(pLink).ResultContent;
            if (!rt3.Contains("status code=\"0\""))
            {
                cpCreateLogiconReport.Result = TestResult.Fail;
                return;
            }
            else
            {
                cpCreateLogiconReport.Result = TestResult.Pass;
                return;
            }
        }
        #endregion
    }
}
