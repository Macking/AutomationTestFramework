using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PAS.AutoTest.TestData;

namespace PAS.AutoTest.TestData.Testser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputData input = new InputData();
            input.Description = "test des for input";

            InputDataSet ds = new InputDataSet();
            ds.InputParameters.AddParameter("key1", "v1");
            ds.InputParameters.AddParameter("key2", "v2");
            ds.InputParameters.AddParameter("key3", "v3");

            ds.ExpectedValues.AddParameter("expected1", "v4");
            ds.ExpectedValues.AddParameter("expected2", "v5");
            ds.Key = "ds key1";
            ds.Description = "des des1";
            input.AddInputDataSet(ds);

            InputDataSet ds2 = new InputDataSet();
            ds2.InputParameters.AddParameter("key1", "v1");
            ds2.InputParameters.AddParameter("key2", "v2");
            ds2.InputParameters.AddParameter("key3", "v3");

            ds2.ExpectedValues.AddParameter("expected1", "v4");
            ds2.ExpectedValues.AddParameter("expected2", "v5");
            ds2.Key = "ds key2";
            ds2.Description = "des des2";
            input.AddInputDataSet(ds2);

            MessageBox.Show(input.Export("c:/test.xml").ToString());

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InputData input = new InputData("c:/test.xml");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OutputData output = new OutputData("Test Test case name", "221");
            Round r1 = new Round("R1", "test des for 1");
            CheckPoint cp = new CheckPoint("cp1", "des for cp1");
            cp.Inputs.AddParameter("input para1", "v1");
            cp.Inputs.AddParameter("input para2", "v2");
            cp.Inputs.AddParameter("input para3", "v3");
            cp.Inputs.AddParameter("input para4", "v4");

            cp.Outpus.AddParameter("output1", "v5");
            cp.Outpus.AddParameter("output2", "v6");
            cp.Outpus.AddParameter("output3", "v7");

            cp.ExpectedValues.AddParameter("E1", "v8");
            cp.ExpectedValues.AddParameter("E2", "v9");
            cp.ExpectedValues.AddParameter("E3", "v10");
            cp.Result = TestResult.Pass;

            r1.CheckPoints.Add(cp);

            CheckPoint cp2 = new CheckPoint("cp1", "des for cp1");
            cp2.Inputs.AddParameter("input para1", "v1");
            cp2.Inputs.AddParameter("input para2", "v2");
            cp2.Inputs.AddParameter("input para3", "v3");
            cp2.Inputs.AddParameter("input para4", "v4");

            cp2.Outpus.AddParameter("output1", "v5");
            cp2.Outpus.AddParameter("output2", "v6");
            cp2.Outpus.AddParameter("output3", "v7");

            cp2.ExpectedValues.AddParameter("E1", "v8");
            cp2.ExpectedValues.AddParameter("E2", "v9");
            cp2.ExpectedValues.AddParameter("E3", "v10");
            cp2.Result = TestResult.Fail;
            r1.CheckPoints.Add(cp);

            output.Rounds.Add(r1);
            output.ConvertToXml("C:/tss.xml");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OutputData output = new OutputData("c:/tss.xml");

        }
    }
}