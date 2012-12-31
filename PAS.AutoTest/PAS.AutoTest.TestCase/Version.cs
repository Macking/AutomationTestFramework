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
        public void Run_Version_GetVersion_Case1567()
        {
            Round r = this.NewRound("Round 1", "GetVersion-Interface");
            CheckPoint pLo = new CheckPoint("GetVersion-Interface", "Version service");
            r.CheckPoints.Add(pLo);

            try
            {
                int a = Utility.TestInterface();
                if (a == 1)
                {
                    pLo.Result = TestResult.Pass;
                    pLo.Outputs.AddParameter("GetVersion-Interface", "Version service", "Interface test pass");
                }
                else
                {
                    pLo.Result = TestResult.Fail;
                    pLo.Outputs.AddParameter("GetVersion-Interface", "Version service", "Interface test fail");
                }
            }
            catch
            {
                pLo.Result = TestResult.Fail;
                pLo.Outputs.AddParameter("GetVersion-Interface", "Version service", "Interface test fail");
            }
            SaveRound(r);
            Output();
        }
    }
}