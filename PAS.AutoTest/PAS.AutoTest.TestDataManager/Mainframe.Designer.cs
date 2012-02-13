namespace PAS.AutoTest.TestData.DataManager
{
    partial class Mainframe
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
            Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet1 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.UltraTreeNodeColumn ultraTreeNodeColumn1 = new Infragistics.Win.UltraWinTree.UltraTreeNodeColumn();
            Infragistics.Win.UltraWinTree.UltraTreeNodeColumn ultraTreeNodeColumn2 = new Infragistics.Win.UltraWinTree.UltraTreeNodeColumn();
            Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet2 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
            Infragistics.Win.UltraWinTree.UltraTreeNodeColumn ultraTreeNodeColumn3 = new Infragistics.Win.UltraWinTree.UltraTreeNodeColumn();
            Infragistics.Win.UltraWinTree.UltraTreeColumnSet ultraTreeColumnSet3 = new Infragistics.Win.UltraWinTree.UltraTreeColumnSet();
            Infragistics.Win.UltraWinTree.UltraTreeNodeColumn ultraTreeNodeColumn4 = new Infragistics.Win.UltraWinTree.UltraTreeNodeColumn();
            Infragistics.Win.UltraWinTree.UltraTreeNodeColumn ultraTreeNodeColumn5 = new Infragistics.Win.UltraWinTree.UltraTreeNodeColumn();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.InputTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ds = new System.Data.DataSet();
            this.dtDataSet = new System.Data.DataTable();
            this.cKey = new System.Data.DataColumn();
            this.cDes = new System.Data.DataColumn();
            this.InputParameters = new System.Data.DataTable();
            this.cInputDataSetKey = new System.Data.DataColumn();
            this.cInputKey = new System.Data.DataColumn();
            this.cInputName = new System.Data.DataColumn();
            this.ExpectedResult = new System.Data.DataTable();
            this.cExpectedDataSet = new System.Data.DataColumn();
            this.cExpectedKey = new System.Data.DataColumn();
            this.cExpectedName = new System.Data.DataColumn();
            this.Step = new System.Data.DataTable();
            this.cInputSetKey = new System.Data.DataColumn();
            this.cStepKey = new System.Data.DataColumn();
            this.cStepName = new System.Data.DataColumn();
            this.Parameter = new System.Data.DataTable();
            this.cParaStepKey = new System.Data.DataColumn();
            this.cParaKey = new System.Data.DataColumn();
            this.cParaValue = new System.Data.DataColumn();
            this.btnImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.InputTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpectedResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Step)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Parameter)).BeginInit();
            this.SuspendLayout();
            // 
            // InputTree
            // 
            this.InputTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.InputTree.ColumnSettings.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeColumnSet1.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeColumnSet1.AllowCellSizing = Infragistics.Win.UltraWinTree.LayoutSizing.Vertical;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraTreeColumnSet1.CellAppearance = appearance1;
            ultraTreeColumnSet1.ColumnAutoSizeMode = Infragistics.Win.UltraWinTree.ColumnAutoSizeMode.AllNodesWithDescendants;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraTreeColumnSet1.ColumnHeaderAppearance = appearance2;
            ultraTreeNodeColumn1.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeNodeColumn1.Key = "cDataSetKey";
            ultraTreeNodeColumn1.Text = "DataSet Key";
            ultraTreeNodeColumn2.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeNodeColumn2.Key = "cDataSetDescription";
            ultraTreeNodeColumn2.Text = "Description";
            ultraTreeColumnSet1.Columns.Add(ultraTreeNodeColumn1);
            ultraTreeColumnSet1.Columns.Add(ultraTreeNodeColumn2);
            ultraTreeColumnSet1.HeaderStyle = Infragistics.Win.HeaderStyle.Standard;
            ultraTreeColumnSet1.Key = "DataSet";
            ultraTreeColumnSet1.TipStyleCell = Infragistics.Win.UltraWinTree.TipStyleCell.Show;
            ultraTreeColumnSet2.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeNodeColumn3.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeNodeColumn3.Key = "cStep";
            ultraTreeNodeColumn3.Text = "Step Name";
            ultraTreeColumnSet2.Columns.Add(ultraTreeNodeColumn3);
            ultraTreeColumnSet2.HeaderStyle = Infragistics.Win.HeaderStyle.XPThemed;
            ultraTreeColumnSet2.Key = "Step";
            ultraTreeColumnSet2.LabelPosition = Infragistics.Win.UltraWinTree.NodeLayoutLabelPosition.Left;
            ultraTreeColumnSet2.LabelStyle = Infragistics.Win.UltraWinTree.NodeLayoutLabelStyle.WithCellData;
            ultraTreeColumnSet3.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeColumnSet3.CellWrapText = Infragistics.Win.DefaultableBoolean.True;
            ultraTreeNodeColumn4.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeNodeColumn4.Key = "cKey";
            ultraTreeNodeColumn4.Text = "Key";
            ultraTreeNodeColumn5.AllowCellEdit = Infragistics.Win.UltraWinTree.AllowCellEdit.Full;
            ultraTreeNodeColumn5.Key = "cValue";
            ultraTreeNodeColumn5.Text = "Value";
            ultraTreeColumnSet3.Columns.Add(ultraTreeNodeColumn4);
            ultraTreeColumnSet3.Columns.Add(ultraTreeNodeColumn5);
            ultraTreeColumnSet3.Key = "Parameter";
            ultraTreeColumnSet3.LabelPosition = Infragistics.Win.UltraWinTree.NodeLayoutLabelPosition.None;
            ultraTreeColumnSet3.LabelStyle = Infragistics.Win.UltraWinTree.NodeLayoutLabelStyle.WithCellData;
            this.InputTree.ColumnSettings.ColumnSets.Add(ultraTreeColumnSet1);
            this.InputTree.ColumnSettings.ColumnSets.Add(ultraTreeColumnSet2);
            this.InputTree.ColumnSettings.ColumnSets.Add(ultraTreeColumnSet3);
            this.InputTree.Location = new System.Drawing.Point(-1, 0);
            this.InputTree.Name = "InputTree";
            _override1.CellClickAction = Infragistics.Win.UltraWinTree.CellClickAction.EditCell;
            this.InputTree.Override = _override1;
            this.InputTree.Size = new System.Drawing.Size(339, 310);
            this.InputTree.TabIndex = 0;
            // 
            // ds
            // 
            this.ds.DataSetName = "NewDataSet";
            this.ds.Relations.AddRange(new System.Data.DataRelation[] {
            new System.Data.DataRelation("DataSetToExpected", "DataSet", "ExpectedResult", new string[] {
                        "Key"}, new string[] {
                        "DataSetKey"}, false),
            new System.Data.DataRelation("StepToPara", "Step", "Parameter", new string[] {
                        "StepKey"}, new string[] {
                        "StepKey"}, false),
            new System.Data.DataRelation("DataSetToInput", "DataSet", "InputParameters", new string[] {
                        "Key"}, new string[] {
                        "DataSetKey"}, false),
            new System.Data.DataRelation("ExpectedToStep", "ExpectedResult", "Step", new string[] {
                        "ExpectedKey"}, new string[] {
                        "InputSetKey"}, false),
            new System.Data.DataRelation("InputToStep", "InputParameters", "Step", new string[] {
                        "InputKey"}, new string[] {
                        "InputSetKey"}, false)});
            this.ds.Tables.AddRange(new System.Data.DataTable[] {
            this.dtDataSet,
            this.InputParameters,
            this.ExpectedResult,
            this.Step,
            this.Parameter});
            // 
            // dtDataSet
            // 
            this.dtDataSet.Columns.AddRange(new System.Data.DataColumn[] {
            this.cKey,
            this.cDes});
            this.dtDataSet.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "Key"}, false)});
            this.dtDataSet.TableName = "DataSet";
            // 
            // cKey
            // 
            this.cKey.ColumnName = "Key";
            // 
            // cDes
            // 
            this.cDes.ColumnName = "Description";
            // 
            // InputParameters
            // 
            this.InputParameters.Columns.AddRange(new System.Data.DataColumn[] {
            this.cInputDataSetKey,
            this.cInputKey,
            this.cInputName});
            this.InputParameters.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("DataSetToInput", "DataSet", new string[] {
                        "Key"}, new string[] {
                        "DataSetKey"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade),
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "InputKey"}, false)});
            this.InputParameters.TableName = "InputParameters";
            // 
            // cInputDataSetKey
            // 
            this.cInputDataSetKey.ColumnName = "DataSetKey";
            // 
            // cInputKey
            // 
            this.cInputKey.ColumnName = "InputKey";
            // 
            // cInputName
            // 
            this.cInputName.ColumnName = "InputName";
            // 
            // ExpectedResult
            // 
            this.ExpectedResult.Columns.AddRange(new System.Data.DataColumn[] {
            this.cExpectedDataSet,
            this.cExpectedKey,
            this.cExpectedName});
            this.ExpectedResult.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("DataSetToExpected", "DataSet", new string[] {
                        "Key"}, new string[] {
                        "DataSetKey"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade),
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "ExpectedKey"}, false)});
            this.ExpectedResult.TableName = "ExpectedResult";
            // 
            // cExpectedDataSet
            // 
            this.cExpectedDataSet.ColumnName = "DataSetKey";
            // 
            // cExpectedKey
            // 
            this.cExpectedKey.ColumnName = "ExpectedKey";
            // 
            // cExpectedName
            // 
            this.cExpectedName.ColumnName = "ExpectedName";
            // 
            // Step
            // 
            this.Step.Columns.AddRange(new System.Data.DataColumn[] {
            this.cInputSetKey,
            this.cStepKey,
            this.cStepName});
            this.Step.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("InputToStep", "InputParameters", new string[] {
                        "InputKey"}, new string[] {
                        "InputSetKey"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade),
            new System.Data.ForeignKeyConstraint("ExpectedToStep", "ExpectedResult", new string[] {
                        "ExpectedKey"}, new string[] {
                        "InputSetKey"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade),
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "StepKey"}, false)});
            this.Step.TableName = "Step";
            // 
            // cInputSetKey
            // 
            this.cInputSetKey.ColumnName = "InputSetKey";
            // 
            // cStepKey
            // 
            this.cStepKey.ColumnName = "StepKey";
            // 
            // cStepName
            // 
            this.cStepName.ColumnName = "StepName";
            // 
            // Parameter
            // 
            this.Parameter.Columns.AddRange(new System.Data.DataColumn[] {
            this.cParaStepKey,
            this.cParaKey,
            this.cParaValue});
            this.Parameter.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("StepToPara", "Step", new string[] {
                        "StepKey"}, new string[] {
                        "StepKey"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.Parameter.TableName = "Parameter";
            // 
            // cParaStepKey
            // 
            this.cParaStepKey.ColumnName = "StepKey";
            // 
            // cParaKey
            // 
            this.cParaKey.ColumnName = "ParaKey";
            // 
            // cParaValue
            // 
            this.cParaValue.ColumnName = "ParaValue";
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(364, 243);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // Mainframe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 308);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.InputTree);
            this.Name = "Mainframe";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.InputTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ExpectedResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Step)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Parameter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree InputTree;
        private System.Data.DataSet ds;
        private System.Data.DataTable dtDataSet;
        private System.Data.DataColumn cKey;
        private System.Data.DataColumn cDes;
        private System.Data.DataTable InputParameters;
        private System.Data.DataColumn cInputDataSetKey;
        private System.Data.DataTable ExpectedResult;
        private System.Data.DataColumn cExpectedDataSet;
        private System.Data.DataTable Step;
        private System.Data.DataColumn cInputSetKey;
        private System.Data.DataColumn cInputKey;
        private System.Data.DataColumn cExpectedKey;
        private System.Data.DataColumn cStepKey;
        private System.Data.DataColumn cStepName;
        private System.Data.DataTable Parameter;
        private System.Data.DataColumn cParaStepKey;
        private System.Data.DataColumn cParaKey;
        private System.Data.DataColumn cParaValue;
        private System.Windows.Forms.Button btnImport;
        private System.Data.DataColumn cInputName;
        private System.Data.DataColumn cExpectedName;
    }
}

