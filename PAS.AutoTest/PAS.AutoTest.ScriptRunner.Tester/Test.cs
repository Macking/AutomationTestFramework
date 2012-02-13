using PAS.AutoTest.TestData;
using PAS.AutoTest.PasATCore;

namespace CaseRunner
{
    public class Runner
    {
        private OutputData mOutput = new OutputData(); 
        private string mLastOutput = "LastOutput.xml";
        private string mInputDataPath = string.Empty;
        private InputData Input = null;

        public Runner()
        {
            if (mInputDataPath == string.Empty)
                Input = new InputData();
            else
                Input = new InputData(mInputDataPath);
        }

        public void Run()
        {
            Round r = this.NewRound("R1","Test round");

            

            CheckPoint p = new CheckPoint("Create Patient", "Test description");

            r.CheckPoints.Add(p);

            PatientService ps = new PatientService();

            XMLParameter pa = new XMLParameter("patient");
            pa.AddParameter(r.DataSet.InputParameters.GetParameter("dafsd").Value, "pid888");
            pa.AddParameter("dpmsid", "dpmsid888");
            pa.AddParameter("firstname", "Test");
            pa.AddParameter("lastname", "Test");
            pa.AddParameter("sex", "male");

            XMLResult result= ps.createPatient(pa);

            if (result.IsErrorOccured)
            {
                p.Result = TestResult.Fail;
                p.Outputs.AddParameter("Error", result.Message);
            }
            else
            {
                p.Result = TestResult.Pass;
                p.Outputs.AddParameter("New patient Id", result.SingleResult);
            }

            SaveRound(r);

            Output();

            System.Windows.Forms.MessageBox.Show("Done");
        }

        private void SaveRound(Round r)
        {
            this.mOutput.Rounds.Add(r);
        }

        private Round NewRound(string key, string description)
        {
            Round r = new Round(key, description);
            return r;
        }

        private void Output()
        {
            this.mOutput.ConvertToXml(this.mLastOutput);
        }
    }
}