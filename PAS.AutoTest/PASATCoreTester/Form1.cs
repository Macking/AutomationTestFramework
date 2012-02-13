using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PASATCore;
using System.Data.OleDb;
using System.Threading;
using System.Data.SQLite;



namespace PASATCoreTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*PASATCore.XMLParameter x = new XMLParameter("patient");
            x.AddParameter("id", "P008");
            x.AddParameter("firstname", "Felix");
            x.AddParameter("lastname", "Jiang");
            x.AddParameter("middlename", "J");
            x.AddParameter("birthDate", "restore");
            x.AddParameter("prefix", "Mr");
            x.AddParameter("suffix", "None");

            x.AddParameter("sex", "M");
            x.AddParameter("pregnancy", "No");

            PatientService ps = new PatientService();
            XMLResult cc= ps.CreatePatient(x);*/
            
            PASATCore.AnalysisService ass = new AnalysisService();
            XMLParameter p = new XMLParameter("presentationstate");
            p.AddParameter("id", "8afa206925fce1330125fd071bc60006");
            p.AddParameter("id", "8afa206925fce1330125fd071ca1000d");

            XMLResult cc= ass.GetDescription("8af0a5d9261b4e0c01261bb5269b003d");

            string b = "false";
            bool d = Convert.ToBoolean(b);
            int a = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int row = Convert.ToInt32(this.tbIndex.Text);
                DataSet ds = this.Select("select * from ParaT");

                if (ds.Tables.Count != 0)
                {
                    this.tbService.Text = ds.Tables[0].Rows[row][1].ToString();
                    this.tbMethod.Text = ds.Tables[0].Rows[row][2].ToString();
                    this.tbParas.Text = ds.Tables[0].Rows[row][3].ToString();
                    this.tbTemplate.Text = ds.Tables[0].Rows[row][4].ToString();
                    this.tbM.Text = ds.Tables[0].Rows[row][5].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private string DBPath = @"\\10.112.39.133\kdb\PASATDB.mdb";
        private void button3_Click(object sender, EventArgs e)
        {    
            string insertStr = "INSERT INTO ParaT( ServiceName, MethodName, ParaName, TemplateText, RequiredField) VALUES ('" +
               tbService .Text +"','"+
                tbMethod .Text  + "','" +
                tbParas .Text + "','" +
                tbTemplate .Text  + "','" +
                tbM .Text +"')";

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source =" + DBPath;

            try
            {
                conn.Open();
                OleDbCommand insertCommand = new OleDbCommand(insertStr, conn);
                insertCommand.ExecuteNonQuery();
                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private string getString(string x)
        {
            string[] matchs = { "'","/" };

            for (int i = 0; i < matchs.Length; i++)
            {
                if (x.Contains(matchs[i]))
                    x = x.Replace(matchs[i], "'" + matchs[i]);

            }

            return x;
        }

        public DataSet Select(string selstr)
        {
            DataSet ds = new DataSet();

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source =" + DBPath;

            try
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(selstr, conn);
                da.Fill(ds);
            }
            catch (Exception)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int n=Convert.ToInt32 (this.tbPatients .Text );
            DateTime dt = DateTime.Now;
            for (int i = 0; i < n; i++)
            {
                PASATCore.XMLParameter x = new XMLParameter("patient");
                x.AddParameter("id", "P008");
                x.AddParameter("firstname", "Felix");
                x.AddParameter("lastname", "Jiang");
                x.AddParameter("middlename", "J");
                x.AddParameter("birthDate", "restore");
                x.AddParameter("prefix", "Mr");
                x.AddParameter("suffix", "None");

                x.AddParameter("sex", "M");
                x.AddParameter("pregnancy", "No");

                PatientService ps = new PatientService();
                XMLResult cc = ps.CreatePatient(x);

                this.pb.Value = (int)(i * 100 / n);
                this.Update();
            }

            System.TimeSpan t = DateTime.Now - dt;
            this.label1.Text = t.Seconds.ToString();
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            XMLParameter filter = new XMLParameter("filter");
            DateTime dt = DateTime.Now;

            PatientService ps = new PatientService();
            XMLResult cc= ps.FindPatient(filter);

            System.TimeSpan t = DateTime.Now - dt;

            MessageBox.Show("Loaded " + cc.ArrayResult.Length + " patients, Cost " + (((float)t.TotalMilliseconds) / 1000).ToString() + " seconds.");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string id="8afa202226068b05012606cb6";
            int c=Convert.ToInt32 (this.tbPatients .Text );
            SQLiteConnection con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=D:\2dviewer\Axis2\database\pas.db;Pooling=true;FailIfMissing=false";
            SQLiteCommand com = new SQLiteCommand(con);
            this.pb.Value = 0;
            try
            {
                con.Open();

                for(int n=0;n<c;n++)
                {
                                com.CommandText ="insert into patient (patient_uid, patient_id, last_name, middle_name,first_name, name_prefix, name_suffix, gender, pregnance_status) VALUES('"+
                id+(1000000+n).ToString()+"',"+
                "'P001','Felix','J','Jiang','MR','None','M','false')";
                    com.ExecuteNonQuery ();
                    this.pb.Value = (int)(n * 100 / c);
                    this.Update();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            System.TimeSpan t = DateTime.Now - dt;

            MessageBox.Show("Created " + c.ToString() + " patients, Cost " + ((float)(t.TotalMilliseconds ) / 1000).ToString() + " seconds.");
            this.label1.Text = ((float)(t.Milliseconds) / 1000).ToString();
        }

        

        private void button7_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(this.tbThread.Text);
            this.pb.Value = 0;
            c.c = 0;
            DateTime dt = DateTime.Now;

            for (int i = 0; i < n; i++)
            {
                Thread thread = new Thread(new ThreadStart(GetPatient));
                thread.Start();
            }

            while (c.c < n)
            {
                this.pb.Value = (int)(c.c * 100 / n);
                this.Update();
            }

            System.TimeSpan t=DateTime .Now -dt;
            MessageBox.Show("Cost "+((float)(t.TotalMilliseconds )/1000).ToString()+" seconds");

        }

        public static Counter c = new Counter();

        public static void GetPatient()
        {
            lock (c)
            {
                XMLParameter filter = new XMLParameter("filter");
                Patient.Patient p = new PASATCoreTester.Patient.Patient();
                p.getPatient("8afa206925fce1330125fce147a10001");
                c.c++;
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            PASATCore.ApplicationService a = new ApplicationService();
            XMLResult r= a.listApplications();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string delStr = "DELETE FROM ParaT WHERE ServiceName='" +
                tbService.Text.Trim() + "' AND MethodName='" +
                tbMethod.Text + "' AND ParaName='" +
                tbParas.Text + "'";

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source =" + DBPath;

            try
            {
                conn.Open();
                OleDbCommand insertCommand = new OleDbCommand(delStr, conn);
                insertCommand.ExecuteNonQuery();
                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }

    public class Counter
    {
        public int c = 0;
    }
}