namespace PAS.AutoTest.Automan
{
    partial class Automan
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            ""}, -1, System.Drawing.SystemColors.InfoText, System.Drawing.Color.Empty, null);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Automan));
            this.runBtn = new System.Windows.Forms.Button();
            this.treeView_Cases = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.runResultSumListView = new System.Windows.Forms.ListView();
            this.caseNo = new System.Windows.Forms.ColumnHeader();
            this.caseName = new System.Windows.Forms.ColumnHeader();
            this.status = new System.Windows.Forms.ColumnHeader();
            this.log = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.noRunNum = new System.Windows.Forms.Label();
            this.failNum = new System.Windows.Forms.Label();
            this.passNum = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.stopBtn = new System.Windows.Forms.Button();
            this.prepareDataBtn = new System.Windows.Forms.Button();
            this.prepareDBBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.selectAll_CheckBox = new System.Windows.Forms.CheckBox();
            this.uploadToQCBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.singleCaseID = new System.Windows.Forms.TextBox();
            this.complieInTime = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // runBtn
            // 
            this.runBtn.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runBtn.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.runBtn.Location = new System.Drawing.Point(223, 37);
            this.runBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(68, 26);
            this.runBtn.TabIndex = 0;
            this.runBtn.Text = "Run";
            this.runBtn.UseVisualStyleBackColor = true;
            this.runBtn.Click += new System.EventHandler(this.RunBtn_Click);
            // 
            // treeView_Cases
            // 
            this.treeView_Cases.CheckBoxes = true;
            this.treeView_Cases.Location = new System.Drawing.Point(14, 69);
            this.treeView_Cases.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView_Cases.Name = "treeView_Cases";
            this.treeView_Cases.Size = new System.Drawing.Size(335, 544);
            this.treeView_Cases.TabIndex = 4;
            this.treeView_Cases.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_Cases_DoubleClickNode);
            this.treeView_Cases.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Cases_AfterCheckNode);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.runResultSumListView);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Location = new System.Drawing.Point(355, 5);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(759, 610);
            this.panel1.TabIndex = 5;
            // 
            // runResultSumListView
            // 
            this.runResultSumListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.runResultSumListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.caseNo,
            this.caseName,
            this.status,
            this.log});
            this.runResultSumListView.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runResultSumListView.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.runResultSumListView.FullRowSelect = true;
            this.runResultSumListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewItem1.StateImageIndex = 0;
            this.runResultSumListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.runResultSumListView.Location = new System.Drawing.Point(9, 29);
            this.runResultSumListView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.runResultSumListView.MultiSelect = false;
            this.runResultSumListView.Name = "runResultSumListView";
            this.runResultSumListView.Size = new System.Drawing.Size(745, 579);
            this.runResultSumListView.TabIndex = 1;
            this.runResultSumListView.TabStop = false;
            this.runResultSumListView.UseCompatibleStateImageBehavior = false;
            this.runResultSumListView.View = System.Windows.Forms.View.Details;
            this.runResultSumListView.DoubleClick += new System.EventHandler(this.runResultSumListView_DoubleClickItem);
            // 
            // caseNo
            // 
            this.caseNo.Text = "No.";
            this.caseNo.Width = 49;
            // 
            // caseName
            // 
            this.caseName.Text = "Case Name";
            this.caseName.Width = 338;
            // 
            // status
            // 
            this.status.Text = "Status";
            this.status.Width = 59;
            // 
            // log
            // 
            this.log.Text = "Comment";
            this.log.Width = 299;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Test Case Run Summary";
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(480, 4);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(273, 24);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.Text = "Select Cases by Status...";
            // 
            // noRunNum
            // 
            this.noRunNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noRunNum.ForeColor = System.Drawing.Color.Blue;
            this.noRunNum.Location = new System.Drawing.Point(648, 619);
            this.noRunNum.Name = "noRunNum";
            this.noRunNum.Size = new System.Drawing.Size(36, 20);
            this.noRunNum.TabIndex = 10;
            this.noRunNum.Text = "0";
            this.noRunNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // failNum
            // 
            this.failNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.failNum.ForeColor = System.Drawing.Color.Red;
            this.failNum.Location = new System.Drawing.Point(516, 619);
            this.failNum.Name = "failNum";
            this.failNum.Size = new System.Drawing.Size(36, 20);
            this.failNum.TabIndex = 9;
            this.failNum.Text = "0";
            this.failNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // passNum
            // 
            this.passNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passNum.ForeColor = System.Drawing.Color.Green;
            this.passNum.Location = new System.Drawing.Point(402, 619);
            this.passNum.Name = "passNum";
            this.passNum.Size = new System.Drawing.Size(36, 20);
            this.passNum.TabIndex = 8;
            this.passNum.Text = "0";
            this.passNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(346, 616);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(371, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Pass:              |      Fail:              |      No Run:            ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // stopBtn
            // 
            this.stopBtn.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopBtn.ForeColor = System.Drawing.Color.Maroon;
            this.stopBtn.Location = new System.Drawing.Point(297, 37);
            this.stopBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(52, 26);
            this.stopBtn.TabIndex = 7;
            this.stopBtn.Text = "Stop";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // prepareDataBtn
            // 
            this.prepareDataBtn.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prepareDataBtn.Location = new System.Drawing.Point(14, 5);
            this.prepareDataBtn.Name = "prepareDataBtn";
            this.prepareDataBtn.Size = new System.Drawing.Size(148, 25);
            this.prepareDataBtn.TabIndex = 8;
            this.prepareDataBtn.Text = "Prepare Test Data";
            this.prepareDataBtn.UseVisualStyleBackColor = true;
            this.prepareDataBtn.Click += new System.EventHandler(this.prepareDataBtn_Click);
            // 
            // prepareDBBtn
            // 
            this.prepareDBBtn.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prepareDBBtn.Location = new System.Drawing.Point(177, 5);
            this.prepareDBBtn.Name = "prepareDBBtn";
            this.prepareDBBtn.Size = new System.Drawing.Size(172, 25);
            this.prepareDBBtn.TabIndex = 9;
            this.prepareDBBtn.Text = "Prepare Database";
            this.prepareDBBtn.UseVisualStyleBackColor = true;
            this.prepareDBBtn.Click += new System.EventHandler(this.prepareDBBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(34, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Test Case List";
            // 
            // selectAll_CheckBox
            // 
            this.selectAll_CheckBox.AutoSize = true;
            this.selectAll_CheckBox.Location = new System.Drawing.Point(14, 44);
            this.selectAll_CheckBox.Name = "selectAll_CheckBox";
            this.selectAll_CheckBox.Size = new System.Drawing.Size(15, 14);
            this.selectAll_CheckBox.TabIndex = 11;
            this.selectAll_CheckBox.UseVisualStyleBackColor = true;
            this.selectAll_CheckBox.CheckedChanged += new System.EventHandler(this.selectAll_CheckBox_CheckedChanged);
            // 
            // uploadToQCBtn
            // 
            this.uploadToQCBtn.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uploadToQCBtn.Location = new System.Drawing.Point(177, 618);
            this.uploadToQCBtn.Name = "uploadToQCBtn";
            this.uploadToQCBtn.Size = new System.Drawing.Size(172, 25);
            this.uploadToQCBtn.TabIndex = 12;
            this.uploadToQCBtn.Text = "Export Case to QC";
            this.uploadToQCBtn.UseVisualStyleBackColor = true;
            this.uploadToQCBtn.Click += new System.EventHandler(this.uploadToQCBtn_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(961, 617);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 25);
            this.button1.TabIndex = 13;
            this.button1.Text = "Export Result to QC";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // singleCaseID
            // 
            this.singleCaseID.Location = new System.Drawing.Point(177, 39);
            this.singleCaseID.MaxLength = 5;
            this.singleCaseID.Name = "singleCaseID";
            this.singleCaseID.Size = new System.Drawing.Size(40, 22);
            this.singleCaseID.TabIndex = 14;
            this.singleCaseID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.singleCaseID_KeyPress);
            this.singleCaseID.LostFocus += new System.EventHandler(this.singleCaseID_LostFocus);
            // 
            // complieInTime
            // 
            this.complieInTime.AutoSize = true;
            this.complieInTime.Location = new System.Drawing.Point(12, 621);
            this.complieInTime.Name = "complieInTime";
            this.complieInTime.Size = new System.Drawing.Size(141, 20);
            this.complieInTime.TabIndex = 15;
            this.complieInTime.Text = "Compile Code in-time";
            this.complieInTime.UseVisualStyleBackColor = true;
            // 
            // Automan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 644);
            this.Controls.Add(this.complieInTime);
            this.Controls.Add(this.singleCaseID);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.passNum);
            this.Controls.Add(this.uploadToQCBtn);
            this.Controls.Add(this.selectAll_CheckBox);
            this.Controls.Add(this.noRunNum);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.failNum);
            this.Controls.Add(this.prepareDBBtn);
            this.Controls.Add(this.prepareDataBtn);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView_Cases);
            this.Controls.Add(this.runBtn);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Automan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSDM Automan";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button runBtn;
        private System.Windows.Forms.TreeView treeView_Cases;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button stopBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView runResultSumListView;
        private System.Windows.Forms.ColumnHeader caseNo;
        private System.Windows.Forms.ColumnHeader caseName;
        private System.Windows.Forms.ColumnHeader status;
        private System.Windows.Forms.ColumnHeader log;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label failNum;
        private System.Windows.Forms.Label passNum;
        private System.Windows.Forms.Label noRunNum;
        private System.Windows.Forms.Button prepareDataBtn;
        private System.Windows.Forms.Button prepareDBBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox selectAll_CheckBox;
        private System.Windows.Forms.Button uploadToQCBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox singleCaseID;
        private System.Windows.Forms.CheckBox complieInTime;
    }
}

