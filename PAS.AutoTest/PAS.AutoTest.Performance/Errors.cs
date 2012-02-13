using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PAS.AutoTest.Performance
{
    public partial class Errors : Form
    {
        private DataSet dsError = null;

        public Errors(DataSet dsError)
        {
            InitializeComponent();
            this.dsError = dsError;
        }

        private void Errors_Load(object sender, EventArgs e)
        {
            this.gdData.DataSource = this.dsError;
            this.gdData.DataBind();
        }
    }
}