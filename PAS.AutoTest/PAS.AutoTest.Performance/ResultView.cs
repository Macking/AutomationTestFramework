using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PAS.AutoTest.Performance.TestCase;

namespace PAS.AutoTest.Performance
{
    public partial class ResultView : Form
    {
        public ResultView()
        {
            InitializeComponent();
        }

        private void btnLoadLabels_Click(object sender, EventArgs e)
        {
            LoadLabels();
        }

        private void LoadLabels()
        {
            this.cbLabels.DataSource  = Log.LoadLabels();
        }

        private void LoadTestCaseNames(string label)
        {
            this.cbTestCases.DataSource = Log.LoadTestCaseNames(cbLabels.SelectedItem.ToString());
        }

        private void cbLabels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLabels.SelectedIndex != -1)
            {
                LoadTestCaseNames(cbLabels.SelectedItem.ToString());
            }
        }

        private void CalSummary()
        {
            if (cbTestCases.SelectedIndex != -1 && cbLabels.SelectedIndex != -1)
            {
                this.dsSummary .Clear ();
                string label = cbLabels.SelectedItem.ToString();
                string testcase = cbTestCases.SelectedItem.ToString();

                DataRow dr = dsSummary.Tables[0].NewRow();
                dr[0] = cbTestCases.SelectedItem.ToString();
                dr[1]= Log.GetAvgResponse (label , testcase ).ToString ();
                dr[3]= Log.GetMaxResponse(label, testcase ).ToString ();
                dr[2]=Log.GetMinResponse (label, testcase ).ToString ();
                dr[4]=Log.GetRepeat(label ,testcase ).ToString ();
                dr[5]= Log.GetErrors (label ,testcase ).ToString ();
                dr[6] = ((float)((float)(Log.GetErrors(label ,testcase ))*100/(float)(Log.GetRepeat (label ,testcase )))).ToString ()+"%";

                dsSummary .Tables [0].Rows.Add (dr);

                this.gdSummary .DataSource = dsSummary.Tables [0];
                this.gdSummary .DataBind ();
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (cbTestCases.SelectedIndex != -1 && cbLabels.SelectedIndex != -1)
            {
                this.dsResult.Tables[0].Clear();
                this.dsResult.Tables[1].Clear();

                DataSet ds = Log.LoadTestCaseData(cbLabels.SelectedItem.ToString(), cbTestCases.SelectedItem.ToString());

                int s = 0;
                Test t = new Test();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //average
                    s += Int32.Parse(ds.Tables[0].Rows[i][0].ToString());

                    DataRow dr2 = dsResult.Tables[1].NewRow();
                    dr2[1] = (int)(s / (i + 1));
                    dr2[0] = DateTime.Parse(ds.Tables[0].Rows[i][1].ToString());

                    dsResult.Tables[1].Rows.Add(dr2);

                    //middle
                    t.Add(Int32.Parse(ds.Tables[0].Rows[i][0].ToString()));

                    DataRow dr = dsResult.Tables[0].NewRow();
                    dr[1] = (int)(t.Average);
                    dr[0] = DateTime.Parse(ds.Tables[0].Rows[i][1].ToString());
                    dsResult.Tables[0].Rows.Add(dr);
                }

                this.ctResponse.DataSource = dsResult.Tables[0];
                this.ctResponse.DataMember = dsResult.Tables[0].TableName;
                this.ctResponse.DataBind();
                this.ctResponse.Visible = true;

                this.ucMiddle.DataSource = dsResult.Tables[1];
                this.ucMiddle.DataMember = dsResult.Tables[1].TableName;
                this.ucMiddle.DataBind();
                this.ucMiddle.Visible = true;

                CalSummary();
            }
            else
            {
                MessageBox.Show("You should select a test case first.");
            }
        }

        private void btnError_Click(object sender, EventArgs e)
        {
            if (cbTestCases.SelectedIndex != -1 && cbLabels.SelectedIndex != -1)
            {
                string label = cbLabels.SelectedItem.ToString();
                string testcase = cbTestCases.SelectedItem.ToString();

                DataSet ds = Log.GetFailedCalls(label, testcase);

                Errors er = new Errors(ds);
                er.Show();
            }
        }
    }

    public class Test
    {
        private List<double> Sum = new List<double>();

        public void Add(double value)
        {
            if (Sum.Count >= 50)
            {
                Sum.RemoveAt(0);
                Sum.Add(value);
            }
            else
                Sum.Add(value);
        }

        public double Average
        {
            get
            {
                double s = 0;

                foreach (double d in Sum)
                {
                    s += d;
                }

                return (double)(s/Sum.Count );
            }
        }
        }
}