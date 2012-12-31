using System;
using System.Collections.Generic;
using System.Text;
using PAS.AutoTest.TestData;
using PAS.AutoTest.PasATCore;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using PAS.AutoTest.TestUtility;

namespace PAS.AutoTest.TestCase
{
    public partial class Runner
    {
        private OutputData mOutput = new OutputData();
        private string mLastOutput = "LastOutput.xml";
        private string mInputDataPath = string.Empty;
        private InputData Input = null;

        public Runner(string InputPath)
        {
            if (InputPath == string.Empty)
                Input = new InputData();
            else
                Input = new InputData(InputPath);
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
