using System;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class CreatePatient : TestCaseTemplate
    {
        #region Constructor

        public CreatePatient()
        {
            this.mRepeat = 60000;
            this.mFunctionName = "CreatePatient";
            this.mConcurrent = 5;
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            string PatientStarts = string.Empty;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            PatientStarts += Convert.ToChar(random.Next(65, 91));
            PatientStarts += Convert.ToChar(random.Next(65, 91));
            PatientStarts += Convert.ToChar(random.Next(65, 91));
            PatientStarts += Convert.ToChar(random.Next(65, 91));

            PatientService ps = new PatientService();

            XMLParameter pa = new XMLParameter("patient");
            pa.AddParameter("patient_id", "$_R");
            pa.AddParameter("dpms_id", "$_R");
            pa.AddParameter("first_name", "$_R");
            pa.AddParameter("last_name", "$_R");
            pa.AddParameter("middle_name", "$_R");
            pa.AddParameter("prefix", "Mr");
            pa.AddParameter("suffix", "X");
            pa.AddParameter("birth_date", "2010-10-22");
            pa.AddParameter("sex", "female");
            pa.AddParameter("pregnancy", "not pregnant");
            pa.AddParameter("insurance_number", "1234567");
            pa.AddParameter("address", "Test address");
            pa.AddParameter("town", "test town");
            pa.AddParameter("postal_code", "female");
            pa.AddParameter("cellular_phone", "female");
            pa.AddParameter("home_phone", "female");
            pa.AddParameter("work_phone", "female");
            pa.AddParameter("comments", "user comments");
            pa.AddParameter("email", "test@test.com");
            pa.AddParameter("photo", "");

            string PatientInfo = pa.GenerateXML();

            for (int i = 1; i <= this.mRepeat; i++)
            {
                LogRecordType lr = new LogRecordType();
                lr.Lable = this.mLabel;
                lr.FunctionName = this.mFunctionName;

                try
                {
                    XMLResult result = new XMLResult(ps.InvokeMethod("createPatient", new object[] { PatientInfo.Replace("$_R", PatientStarts + (i + 1).ToString()) }));
                    lr.ResponseTime = ps.ResponseTime;
                    lr.Passed = !(result.IsErrorOccured);

                    if (!lr.Passed)
                    {
                        lr.Message = result.Message;
                        this.mFailed++;
                    }
                    else
                    {
                        lr.Message = result.SingleResult;
                        this.mExectuedTime += lr.ResponseTime;
                    }
                }
                catch (Exception ex)
                {
                    this.mFailed++;
                    lr.Passed = false;
                    string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                    lr.Message = ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
                }

                this.Executed = i;
                Log.AddRecord(lr);

                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed);
            }

            this.RiseTestCaseCompleteEvent();
        }

        #endregion
    }
}
