namespace PAS.AutoTest.Performance
{
    partial class ResultView
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
            Infragistics.UltraChart.Resources.Appearance.PaintElement paintElement3 = new Infragistics.UltraChart.Resources.Appearance.PaintElement();
            Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect3 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.UltraChart.Resources.Appearance.PaintElement paintElement4 = new Infragistics.UltraChart.Resources.Appearance.PaintElement();
            Infragistics.UltraChart.Resources.Appearance.GradientEffect gradientEffect4 = new Infragistics.UltraChart.Resources.Appearance.GradientEffect();
            this.cbLabels = new System.Windows.Forms.ComboBox();
            this.btnLoadLabels = new System.Windows.Forms.Button();
            this.cbTestCases = new System.Windows.Forms.ComboBox();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.dsResult = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.ctResponse = new Infragistics.Win.UltraWinChart.UltraChart();
            this.gdSummary = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsSummary = new System.Data.DataSet();
            this.Data = new System.Data.DataTable();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn9 = new System.Data.DataColumn();
            this.dataColumn10 = new System.Data.DataColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnError = new System.Windows.Forms.Button();
            this.ucMiddle = new Infragistics.Win.UltraWinChart.UltraChart();
            this.dataTable2 = new System.Data.DataTable();
            this.dataColumn8 = new System.Data.DataColumn();
            this.dataColumn12 = new System.Data.DataColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dsResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctResponse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Data)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucMiddle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).BeginInit();
            this.SuspendLayout();
            // 
            // cbLabels
            // 
            this.cbLabels.FormattingEnabled = true;
            this.cbLabels.Location = new System.Drawing.Point(125, 34);
            this.cbLabels.Name = "cbLabels";
            this.cbLabels.Size = new System.Drawing.Size(202, 21);
            this.cbLabels.TabIndex = 0;
            this.cbLabels.SelectedIndexChanged += new System.EventHandler(this.cbLabels_SelectedIndexChanged);
            // 
            // btnLoadLabels
            // 
            this.btnLoadLabels.Location = new System.Drawing.Point(346, 32);
            this.btnLoadLabels.Name = "btnLoadLabels";
            this.btnLoadLabels.Size = new System.Drawing.Size(102, 23);
            this.btnLoadLabels.TabIndex = 1;
            this.btnLoadLabels.Text = "Load all Data";
            this.btnLoadLabels.UseVisualStyleBackColor = true;
            this.btnLoadLabels.Click += new System.EventHandler(this.btnLoadLabels_Click);
            // 
            // cbTestCases
            // 
            this.cbTestCases.FormattingEnabled = true;
            this.cbTestCases.Location = new System.Drawing.Point(125, 72);
            this.cbTestCases.Name = "cbTestCases";
            this.cbTestCases.Size = new System.Drawing.Size(202, 21);
            this.cbTestCases.TabIndex = 2;
            // 
            // btnLoadData
            // 
            this.btnLoadData.Location = new System.Drawing.Point(346, 72);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(127, 23);
            this.btnLoadData.TabIndex = 3;
            this.btnLoadData.Text = "Load Test Case Data";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // dsResult
            // 
            this.dsResult.DataSetName = "NewDataSet";
            this.dsResult.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1,
            this.dataTable2});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2});
            this.dataTable1.TableName = "Data";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Time";
            this.dataColumn1.ColumnName = "Time";
            this.dataColumn1.DataType = typeof(System.DateTime);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Response";
            this.dataColumn2.ColumnName = "Response";
            this.dataColumn2.DataType = typeof(int);
            // 
            // ctResponse
            // 
            this.ctResponse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ctResponse.Axis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            paintElement3.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.None;
            paintElement3.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            this.ctResponse.Axis.PE = paintElement3;
            this.ctResponse.Axis.X.Extent = 39;
            this.ctResponse.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.X.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ctResponse.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.ctResponse.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ctResponse.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ctResponse.Axis.X.Labels.SeriesLabels.FormatString = "";
            this.ctResponse.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.X.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ctResponse.Axis.X.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.X.LineThickness = 1;
            this.ctResponse.Axis.X.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ctResponse.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.X.MajorGridLines.Visible = false;
            this.ctResponse.Axis.X.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ctResponse.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.X.MinorGridLines.Visible = false;
            this.ctResponse.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ctResponse.Axis.X.Visible = true;
            this.ctResponse.Axis.X2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.X2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ctResponse.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ctResponse.Axis.X2.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.ctResponse.Axis.X2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ctResponse.Axis.X2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.X2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ctResponse.Axis.X2.Labels.SeriesLabels.FormatString = "";
            this.ctResponse.Axis.X2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ctResponse.Axis.X2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.X2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ctResponse.Axis.X2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.X2.Labels.Visible = false;
            this.ctResponse.Axis.X2.LineThickness = 1;
            this.ctResponse.Axis.X2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ctResponse.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.X2.MajorGridLines.Visible = true;
            this.ctResponse.Axis.X2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.X2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ctResponse.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.X2.MinorGridLines.Visible = false;
            this.ctResponse.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ctResponse.Axis.X2.Visible = false;
            this.ctResponse.Axis.Y.Extent = 46;
            this.ctResponse.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ctResponse.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ctResponse.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ctResponse.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ctResponse.Axis.Y.Labels.SeriesLabels.FormatString = "";
            this.ctResponse.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ctResponse.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Y.LineThickness = 1;
            this.ctResponse.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ctResponse.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Y.MajorGridLines.Visible = true;
            this.ctResponse.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ctResponse.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Y.MinorGridLines.Visible = false;
            this.ctResponse.Axis.Y.TickmarkInterval = 100;
            this.ctResponse.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ctResponse.Axis.Y.Visible = true;
            this.ctResponse.Axis.Y2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Y2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ctResponse.Axis.Y2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ctResponse.Axis.Y2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Y2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.FormatString = "";
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Y2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Y2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Y2.Labels.Visible = false;
            this.ctResponse.Axis.Y2.LineThickness = 1;
            this.ctResponse.Axis.Y2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Y2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ctResponse.Axis.Y2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Y2.MajorGridLines.Visible = true;
            this.ctResponse.Axis.Y2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Y2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ctResponse.Axis.Y2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Y2.MinorGridLines.Visible = false;
            this.ctResponse.Axis.Y2.TickmarkInterval = 100;
            this.ctResponse.Axis.Y2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ctResponse.Axis.Y2.Visible = false;
            this.ctResponse.Axis.Z.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Z.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ctResponse.Axis.Z.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.Z.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.ctResponse.Axis.Z.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Z.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Z.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Z.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ctResponse.Axis.Z.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.Z.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Z.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Z.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Z.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Z.Labels.Visible = false;
            this.ctResponse.Axis.Z.LineThickness = 1;
            this.ctResponse.Axis.Z.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Z.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ctResponse.Axis.Z.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Z.MajorGridLines.Visible = true;
            this.ctResponse.Axis.Z.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Z.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ctResponse.Axis.Z.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Z.MinorGridLines.Visible = false;
            this.ctResponse.Axis.Z.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ctResponse.Axis.Z.Visible = false;
            this.ctResponse.Axis.Z2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Z2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ctResponse.Axis.Z2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.Z2.Labels.ItemFormatString = "";
            this.ctResponse.Axis.Z2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Z2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Z2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ctResponse.Axis.Z2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ctResponse.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ctResponse.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ctResponse.Axis.Z2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ctResponse.Axis.Z2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Z2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ctResponse.Axis.Z2.Labels.Visible = false;
            this.ctResponse.Axis.Z2.LineThickness = 1;
            this.ctResponse.Axis.Z2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Z2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ctResponse.Axis.Z2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Z2.MajorGridLines.Visible = true;
            this.ctResponse.Axis.Z2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ctResponse.Axis.Z2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ctResponse.Axis.Z2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ctResponse.Axis.Z2.MinorGridLines.Visible = false;
            this.ctResponse.Axis.Z2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ctResponse.Axis.Z2.Visible = false;
            this.ctResponse.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.StackLineChart;
            this.ctResponse.ColorModel.AlphaLevel = ((byte)(150));
            this.ctResponse.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomRandom;
            this.ctResponse.Data.SwapRowsAndColumns = true;
            this.ctResponse.Effects.Effects.Add(gradientEffect3);
            this.ctResponse.Location = new System.Drawing.Point(27, 225);
            this.ctResponse.Name = "ctResponse";
            this.ctResponse.Size = new System.Drawing.Size(687, 219);
            this.ctResponse.TabIndex = 4;
            this.ctResponse.Tooltips.HighlightFillColor = System.Drawing.Color.DimGray;
            this.ctResponse.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;
            this.ctResponse.Visible = false;
            // 
            // gdSummary
            // 
            this.gdSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gdSummary.DisplayLayout.Appearance = appearance13;
            this.gdSummary.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gdSummary.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance14.BorderColor = System.Drawing.SystemColors.Window;
            this.gdSummary.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gdSummary.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.gdSummary.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gdSummary.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.gdSummary.DisplayLayout.MaxColScrollRegions = 1;
            this.gdSummary.DisplayLayout.MaxRowScrollRegions = 1;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            appearance17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gdSummary.DisplayLayout.Override.ActiveCellAppearance = appearance17;
            appearance18.BackColor = System.Drawing.SystemColors.Highlight;
            appearance18.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gdSummary.DisplayLayout.Override.ActiveRowAppearance = appearance18;
            this.gdSummary.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gdSummary.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            this.gdSummary.DisplayLayout.Override.CardAreaAppearance = appearance19;
            appearance20.BorderColor = System.Drawing.Color.Silver;
            appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gdSummary.DisplayLayout.Override.CellAppearance = appearance20;
            this.gdSummary.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gdSummary.DisplayLayout.Override.CellPadding = 0;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance21.BorderColor = System.Drawing.SystemColors.Window;
            this.gdSummary.DisplayLayout.Override.GroupByRowAppearance = appearance21;
            appearance22.TextHAlignAsString = "Left";
            this.gdSummary.DisplayLayout.Override.HeaderAppearance = appearance22;
            this.gdSummary.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gdSummary.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.BorderColor = System.Drawing.Color.Silver;
            this.gdSummary.DisplayLayout.Override.RowAppearance = appearance23;
            this.gdSummary.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance24.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gdSummary.DisplayLayout.Override.TemplateAddRowAppearance = appearance24;
            this.gdSummary.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gdSummary.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gdSummary.Location = new System.Drawing.Point(27, 121);
            this.gdSummary.Name = "gdSummary";
            this.gdSummary.Size = new System.Drawing.Size(687, 80);
            this.gdSummary.TabIndex = 6;
            this.gdSummary.Text = "ultraGrid1";
            // 
            // dsSummary
            // 
            this.dsSummary.DataSetName = "NewDataSet";
            this.dsSummary.Tables.AddRange(new System.Data.DataTable[] {
            this.Data});
            // 
            // Data
            // 
            this.Data.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn9,
            this.dataColumn10});
            this.Data.TableName = "Data";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Test Case";
            this.dataColumn3.ColumnName = "TestCase";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Average Response (ms)";
            this.dataColumn4.ColumnName = "Average";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Minimum (ms)";
            this.dataColumn5.ColumnName = "Min";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Maximal (ms)";
            this.dataColumn6.ColumnName = "Max";
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "Repeat";
            // 
            // dataColumn9
            // 
            this.dataColumn9.ColumnName = "Error";
            // 
            // dataColumn10
            // 
            this.dataColumn10.ColumnName = "Error Rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Test Cases";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Labels";
            // 
            // btnError
            // 
            this.btnError.Location = new System.Drawing.Point(494, 72);
            this.btnError.Name = "btnError";
            this.btnError.Size = new System.Drawing.Size(102, 23);
            this.btnError.TabIndex = 10;
            this.btnError.Text = "Load Failed Calls";
            this.btnError.UseVisualStyleBackColor = true;
            this.btnError.Click += new System.EventHandler(this.btnError_Click);
            // 
            // ucMiddle
            // 
            this.ucMiddle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ucMiddle.Axis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            paintElement4.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.None;
            paintElement4.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(248)))), ((int)(((byte)(220)))));
            this.ucMiddle.Axis.PE = paintElement4;
            this.ucMiddle.Axis.X.Extent = 39;
            this.ucMiddle.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.X.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Axis.X.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.ucMiddle.Axis.X.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ucMiddle.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.X.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Axis.X.Labels.SeriesLabels.FormatString = "";
            this.ucMiddle.Axis.X.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.X.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.X.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ucMiddle.Axis.X.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.X.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.X.LineThickness = 1;
            this.ucMiddle.Axis.X.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.X.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ucMiddle.Axis.X.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.X.MajorGridLines.Visible = false;
            this.ucMiddle.Axis.X.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.X.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ucMiddle.Axis.X.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.X.MinorGridLines.Visible = false;
            this.ucMiddle.Axis.X.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ucMiddle.Axis.X.Visible = true;
            this.ucMiddle.Axis.X2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.X2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ucMiddle.Axis.X2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ucMiddle.Axis.X2.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.ucMiddle.Axis.X2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.X2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.FormatString = "";
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
            this.ucMiddle.Axis.X2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.X2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.X2.Labels.Visible = false;
            this.ucMiddle.Axis.X2.LineThickness = 1;
            this.ucMiddle.Axis.X2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.X2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ucMiddle.Axis.X2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.X2.MajorGridLines.Visible = true;
            this.ucMiddle.Axis.X2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.X2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ucMiddle.Axis.X2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.X2.MinorGridLines.Visible = false;
            this.ucMiddle.Axis.X2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ucMiddle.Axis.X2.Visible = false;
            this.ucMiddle.Axis.Y.Extent = 46;
            this.ucMiddle.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Y.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Axis.Y.Labels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ucMiddle.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ucMiddle.Axis.Y.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Y.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.FormatString = "";
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Far;
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Y.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Y.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Y.LineThickness = 1;
            this.ucMiddle.Axis.Y.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Y.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ucMiddle.Axis.Y.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Y.MajorGridLines.Visible = true;
            this.ucMiddle.Axis.Y.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Y.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ucMiddle.Axis.Y.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Y.MinorGridLines.Visible = false;
            this.ucMiddle.Axis.Y.TickmarkInterval = 100;
            this.ucMiddle.Axis.Y.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ucMiddle.Axis.Y.Visible = true;
            this.ucMiddle.Axis.Y2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Y2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ucMiddle.Axis.Y2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:00.##>";
            this.ucMiddle.Axis.Y2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Y2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.FormatString = "";
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Y2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Y2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Y2.Labels.Visible = false;
            this.ucMiddle.Axis.Y2.LineThickness = 1;
            this.ucMiddle.Axis.Y2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Y2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ucMiddle.Axis.Y2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Y2.MajorGridLines.Visible = true;
            this.ucMiddle.Axis.Y2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Y2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ucMiddle.Axis.Y2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Y2.MinorGridLines.Visible = false;
            this.ucMiddle.Axis.Y2.TickmarkInterval = 100;
            this.ucMiddle.Axis.Y2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ucMiddle.Axis.Y2.Visible = false;
            this.ucMiddle.Axis.Z.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Z.Labels.FontColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Axis.Z.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.Z.Labels.ItemFormatString = "<ITEM_LABEL>";
            this.ucMiddle.Axis.Z.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Z.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Z.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Z.Labels.SeriesLabels.FontColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Axis.Z.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.Z.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Z.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Z.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Z.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Z.Labels.Visible = false;
            this.ucMiddle.Axis.Z.LineThickness = 1;
            this.ucMiddle.Axis.Z.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Z.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ucMiddle.Axis.Z.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Z.MajorGridLines.Visible = true;
            this.ucMiddle.Axis.Z.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Z.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ucMiddle.Axis.Z.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Z.MinorGridLines.Visible = false;
            this.ucMiddle.Axis.Z.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ucMiddle.Axis.Z.Visible = false;
            this.ucMiddle.Axis.Z2.Labels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Z2.Labels.FontColor = System.Drawing.Color.Gray;
            this.ucMiddle.Axis.Z2.Labels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.Z2.Labels.ItemFormatString = "";
            this.ucMiddle.Axis.Z2.Labels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Z2.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Z2.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 7F);
            this.ucMiddle.Axis.Z2.Labels.SeriesLabels.FontColor = System.Drawing.Color.Gray;
            this.ucMiddle.Axis.Z2.Labels.SeriesLabels.HorizontalAlign = System.Drawing.StringAlignment.Near;
            this.ucMiddle.Axis.Z2.Labels.SeriesLabels.Layout.Behavior = Infragistics.UltraChart.Shared.Styles.AxisLabelLayoutBehaviors.Auto;
            this.ucMiddle.Axis.Z2.Labels.SeriesLabels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
            this.ucMiddle.Axis.Z2.Labels.SeriesLabels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Z2.Labels.VerticalAlign = System.Drawing.StringAlignment.Center;
            this.ucMiddle.Axis.Z2.Labels.Visible = false;
            this.ucMiddle.Axis.Z2.LineThickness = 1;
            this.ucMiddle.Axis.Z2.MajorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Z2.MajorGridLines.Color = System.Drawing.Color.Gainsboro;
            this.ucMiddle.Axis.Z2.MajorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Z2.MajorGridLines.Visible = true;
            this.ucMiddle.Axis.Z2.MinorGridLines.AlphaLevel = ((byte)(255));
            this.ucMiddle.Axis.Z2.MinorGridLines.Color = System.Drawing.Color.LightGray;
            this.ucMiddle.Axis.Z2.MinorGridLines.DrawStyle = Infragistics.UltraChart.Shared.Styles.LineDrawStyle.Dot;
            this.ucMiddle.Axis.Z2.MinorGridLines.Visible = false;
            this.ucMiddle.Axis.Z2.TickmarkStyle = Infragistics.UltraChart.Shared.Styles.AxisTickStyle.Smart;
            this.ucMiddle.Axis.Z2.Visible = false;
            this.ucMiddle.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.StackLineChart;
            this.ucMiddle.ColorModel.AlphaLevel = ((byte)(150));
            this.ucMiddle.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomRandom;
            this.ucMiddle.Data.SwapRowsAndColumns = true;
            this.ucMiddle.Effects.Effects.Add(gradientEffect4);
            this.ucMiddle.Location = new System.Drawing.Point(27, 472);
            this.ucMiddle.Name = "ucMiddle";
            this.ucMiddle.Size = new System.Drawing.Size(687, 219);
            this.ucMiddle.TabIndex = 13;
            this.ucMiddle.Tooltips.HighlightFillColor = System.Drawing.Color.DimGray;
            this.ucMiddle.Tooltips.HighlightOutlineColor = System.Drawing.Color.DarkGray;
            this.ucMiddle.Visible = false;
            // 
            // dataTable2
            // 
            this.dataTable2.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn8,
            this.dataColumn12});
            this.dataTable2.TableName = "Middle";
            // 
            // dataColumn8
            // 
            this.dataColumn8.Caption = "Time";
            this.dataColumn8.ColumnName = "Time";
            this.dataColumn8.DataType = typeof(System.DateTime);
            // 
            // dataColumn12
            // 
            this.dataColumn12.ColumnName = "Response";
            this.dataColumn12.DataType = typeof(int);
            // 
            // ResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 713);
            this.Controls.Add(this.btnError);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ctResponse);
            this.Controls.Add(this.gdSummary);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbTestCases);
            this.Controls.Add(this.ucMiddle);
            this.Controls.Add(this.btnLoadLabels);
            this.Controls.Add(this.cbLabels);
            this.Controls.Add(this.btnLoadData);
            this.Name = "ResultView";
            this.Text = "ResultView";
            ((System.ComponentModel.ISupportInitialize)(this.dsResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctResponse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Data)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucMiddle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbLabels;
        private System.Windows.Forms.Button btnLoadLabels;
        private System.Windows.Forms.ComboBox cbTestCases;
        private System.Windows.Forms.Button btnLoadData;
        private System.Data.DataSet dsResult;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private Infragistics.Win.UltraWinChart.UltraChart ctResponse;
        private Infragistics.Win.UltraWinGrid.UltraGrid gdSummary;
        private System.Data.DataSet dsSummary;
        private System.Data.DataTable Data;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Data.DataColumn dataColumn9;
        private System.Data.DataColumn dataColumn10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnError;
        private Infragistics.Win.UltraWinChart.UltraChart ucMiddle;
        private System.Data.DataTable dataTable2;
        private System.Data.DataColumn dataColumn8;
        private System.Data.DataColumn dataColumn12;
    }
}