using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PAS.AutoTest.ScriptRunner.Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PAS.AutoTest.ScriptRunner.ScriptRunner sr = new ScriptRunner();
            sr.ExecuteScript(this.richTextBox1.Text,@"D:\test.iod");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CaseRunner.Runner r = new CaseRunner.Runner();
            r.Run();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PAS.AutoTest.ScriptRunner.ScriptRunner sr = new ScriptRunner();
            sr.Run(@"D:\test.cod",@"D:\test.iod", 1000);
        }
    }
}