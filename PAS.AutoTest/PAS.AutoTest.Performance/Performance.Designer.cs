namespace PAS.AutoTest.Performance
{
    partial class Performance
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.btnStart = new System.Windows.Forms.Button();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn2 = new System.Data.DataColumn();
            this.pbProgress = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.lbFunctionName = new System.Windows.Forms.Label();
            this.lbRepeat = new System.Windows.Forms.Label();
            this.lbExecuted = new System.Windows.Forms.Label();
            this.lbFailed = new System.Windows.Forms.Label();
            this.lbAverage = new System.Windows.Forms.Label();
            this.tbTestCasePath = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.cbTestCases = new System.Windows.Forms.ComboBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.cbRunAll = new System.Windows.Forms.CheckBox();
            this.tbLabel = new System.Windows.Forms.TextBox();
            this.btnViewResult = new System.Windows.Forms.Button();
            this.lbConcurrent = new System.Windows.Forms.Label();
            this.lbConcurrentLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(324, 100);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn2});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Response";
            this.dataColumn2.DataType = typeof(int);
            // 
            // pbProgress
            // 
            appearance3.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance3.FontData.BoldAsString = "True";
            appearance3.FontData.Name = "Arial";
            appearance3.ForeColor = System.Drawing.Color.Black;
            this.pbProgress.Appearance = appearance3;
            this.pbProgress.BorderStyle = Infragistics.Win.UIElementBorderStyle.Rounded4;
            appearance4.BackColor = System.Drawing.Color.Gray;
            appearance4.FontData.BoldAsString = "True";
            appearance4.FontData.Name = "Arial";
            this.pbProgress.FillAppearance = appearance4;
            this.pbProgress.Location = new System.Drawing.Point(102, 329);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(311, 23);
            this.pbProgress.Step = 1;
            this.pbProgress.TabIndex = 6;
            this.pbProgress.Text = "[Formatted]";
            this.pbProgress.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // lbFunctionName
            // 
            this.lbFunctionName.AutoSize = true;
            this.lbFunctionName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFunctionName.Location = new System.Drawing.Point(180, 153);
            this.lbFunctionName.Name = "lbFunctionName";
            this.lbFunctionName.Size = new System.Drawing.Size(0, 14);
            this.lbFunctionName.TabIndex = 7;
            // 
            // lbRepeat
            // 
            this.lbRepeat.AutoSize = true;
            this.lbRepeat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbRepeat.Location = new System.Drawing.Point(180, 182);
            this.lbRepeat.Name = "lbRepeat";
            this.lbRepeat.Size = new System.Drawing.Size(15, 14);
            this.lbRepeat.TabIndex = 9;
            this.lbRepeat.Text = "0";
            // 
            // lbExecuted
            // 
            this.lbExecuted.AutoSize = true;
            this.lbExecuted.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbExecuted.ForeColor = System.Drawing.Color.LimeGreen;
            this.lbExecuted.Location = new System.Drawing.Point(180, 212);
            this.lbExecuted.Name = "lbExecuted";
            this.lbExecuted.Size = new System.Drawing.Size(15, 14);
            this.lbExecuted.TabIndex = 10;
            this.lbExecuted.Text = "0";
            // 
            // lbFailed
            // 
            this.lbFailed.AutoSize = true;
            this.lbFailed.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFailed.ForeColor = System.Drawing.Color.Red;
            this.lbFailed.Location = new System.Drawing.Point(180, 246);
            this.lbFailed.Name = "lbFailed";
            this.lbFailed.Size = new System.Drawing.Size(15, 14);
            this.lbFailed.TabIndex = 11;
            this.lbFailed.Text = "0";
            // 
            // lbAverage
            // 
            this.lbAverage.AutoSize = true;
            this.lbAverage.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbAverage.Location = new System.Drawing.Point(181, 280);
            this.lbAverage.Name = "lbAverage";
            this.lbAverage.Size = new System.Drawing.Size(15, 14);
            this.lbAverage.TabIndex = 12;
            this.lbAverage.Text = "0";
            // 
            // tbTestCasePath
            // 
            this.tbTestCasePath.Location = new System.Drawing.Point(118, 31);
            this.tbTestCasePath.Name = "tbTestCasePath";
            this.tbTestCasePath.Size = new System.Drawing.Size(295, 20);
            this.tbTestCasePath.TabIndex = 13;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(419, 29);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 14;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cbTestCases
            // 
            this.cbTestCases.FormattingEnabled = true;
            this.cbTestCases.Location = new System.Drawing.Point(118, 64);
            this.cbTestCases.Name = "cbTestCases";
            this.cbTestCases.Size = new System.Drawing.Size(295, 21);
            this.cbTestCases.TabIndex = 15;
            this.cbTestCases.SelectedIndexChanged += new System.EventHandler(this.cbTestCases_SelectedIndexChanged);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(417, 100);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 16;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // cbRunAll
            // 
            this.cbRunAll.AutoSize = true;
            this.cbRunAll.Location = new System.Drawing.Point(422, 69);
            this.cbRunAll.Name = "cbRunAll";
            this.cbRunAll.Size = new System.Drawing.Size(59, 17);
            this.cbRunAll.TabIndex = 17;
            this.cbRunAll.Text = "Run all";
            this.cbRunAll.UseVisualStyleBackColor = true;
            this.cbRunAll.CheckedChanged += new System.EventHandler(this.cbRunAll_CheckedChanged);
            // 
            // tbLabel
            // 
            this.tbLabel.Location = new System.Drawing.Point(118, 100);
            this.tbLabel.Name = "tbLabel";
            this.tbLabel.Size = new System.Drawing.Size(187, 20);
            this.tbLabel.TabIndex = 18;
            // 
            // btnViewResult
            // 
            this.btnViewResult.Location = new System.Drawing.Point(435, 329);
            this.btnViewResult.Name = "btnViewResult";
            this.btnViewResult.Size = new System.Drawing.Size(75, 23);
            this.btnViewResult.TabIndex = 19;
            this.btnViewResult.Text = "Result";
            this.btnViewResult.UseVisualStyleBackColor = true;
            this.btnViewResult.Click += new System.EventHandler(this.btnViewResult_Click);
            // 
            // lbConcurrent
            // 
            this.lbConcurrent.AutoSize = true;
            this.lbConcurrent.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbConcurrent.Location = new System.Drawing.Point(438, 280);
            this.lbConcurrent.Name = "lbConcurrent";
            this.lbConcurrent.Size = new System.Drawing.Size(15, 14);
            this.lbConcurrent.TabIndex = 20;
            this.lbConcurrent.Text = "0";
            // 
            // lbConcurrentLabel
            // 
            this.lbConcurrentLabel.AutoSize = true;
            this.lbConcurrentLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbConcurrentLabel.Location = new System.Drawing.Point(357, 280);
            this.lbConcurrentLabel.Name = "lbConcurrentLabel";
            this.lbConcurrentLabel.Size = new System.Drawing.Size(80, 14);
            this.lbConcurrentLabel.TabIndex = 21;
            this.lbConcurrentLabel.Text = "Concurrent:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Test Case file Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Test Case";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(65, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 14);
            this.label3.TabIndex = 24;
            this.label3.Text = "Test Case Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(120, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 14);
            this.label4.TabIndex = 25;
            this.label4.Text = "Repeat:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.LimeGreen;
            this.label5.Location = new System.Drawing.Point(115, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 14);
            this.label5.TabIndex = 26;
            this.label5.Text = "Excuted:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(126, 246);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 14);
            this.label6.TabIndex = 27;
            this.label6.Text = "Failed:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(51, 280);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(124, 14);
            this.label7.TabIndex = 28;
            this.label7.Text = "Average Response:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(41, 105);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Result Label:";
            // 
            // Performance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(538, 369);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbConcurrentLabel);
            this.Controls.Add(this.lbConcurrent);
            this.Controls.Add(this.btnViewResult);
            this.Controls.Add(this.tbLabel);
            this.Controls.Add(this.cbRunAll);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.cbTestCases);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.tbTestCasePath);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lbAverage);
            this.Controls.Add(this.lbFailed);
            this.Controls.Add(this.lbExecuted);
            this.Controls.Add(this.lbRepeat);
            this.Controls.Add(this.lbFunctionName);
            this.Controls.Add(this.pbProgress);
            this.Name = "Performance";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PAS Performance";
            this.Load += new System.EventHandler(this.Performance_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Performance_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn2;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar pbProgress;
        private System.Windows.Forms.Label lbFunctionName;
        private System.Windows.Forms.Label lbRepeat;
        private System.Windows.Forms.Label lbExecuted;
        private System.Windows.Forms.Label lbFailed;
        private System.Windows.Forms.Label lbAverage;
        private System.Windows.Forms.TextBox tbTestCasePath;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox cbTestCases;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.CheckBox cbRunAll;
        private System.Windows.Forms.TextBox tbLabel;
        private System.Windows.Forms.Button btnViewResult;
        private System.Windows.Forms.Label lbConcurrent;
        private System.Windows.Forms.Label lbConcurrentLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

