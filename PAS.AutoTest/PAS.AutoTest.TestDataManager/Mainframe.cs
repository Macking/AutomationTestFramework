using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Infragistics.Win.UltraWinTree;

using PAS.AutoTest.TestData;

namespace PAS.AutoTest.TestData.DataManager
{
    public partial class Mainframe : Form
    {
        public Mainframe()
        {
            InitializeComponent();
        }

        private void InsertDataSet(InputDataSet ids)
        {
            UltraTreeNode dsNode = this.InputTree.Nodes.Add(ids.Key);

            dsNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["DataSet"];

            dsNode.Cells[0].Value = ids.Key;
            dsNode.Cells[1].Value = ids.Description;

            dsNode.Tag = 0;

            UltraTreeNode inputParameterNode = dsNode.Nodes.Add();
            inputParameterNode.Tag = 1;
            inputParameterNode.Text = "Input Parameters";

            foreach (Step s in ids.InputParameters.Steps)
            {
                InsertStep(inputParameterNode, s);
            }

            UltraTreeNode expectedResultNode = dsNode.Nodes.Add();
            expectedResultNode.Tag = 1;
            expectedResultNode.Text = "Expected Result";

            foreach (Step s in ids.ExpectedValues.Steps)
            {
                InsertStep(expectedResultNode, s);
            }
        }

        private bool InsertStep(UltraTreeNode parentNode, Step step)
        {
            if (Convert.ToInt32(parentNode.Tag) == 1)
            {
                UltraTreeNode stepNode = parentNode.Nodes.Add();
                stepNode.Tag = 2;
                stepNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["Step"];

                stepNode.Cells[0].Value = step.Name;

                foreach (Parameter p in step.Parameters)
                {
                    this.InsertParameter(stepNode, p);
                }

                return true;
            }
            else
                return false;
        }

        private bool InsertParameter(UltraTreeNode parentNode, Parameter para)
        {
            if (Convert.ToInt32(parentNode.Tag) == 2)
            {
                UltraTreeNode paraNode = parentNode.Nodes.Add();
                paraNode.Tag = 3;

                paraNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["Parameter"];

                paraNode.Cells[0].Value = para.Key;
                paraNode.Cells[1].Value = para.Value;

                return true;
            }
            else
                return false;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            InputData input = new InputData(@"c:\a.xml");

            this.InputTree.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.FreeForm ;

            foreach (InputDataSet ids in input.DataSets)
            {
                this.InsertDataSet(ids);
            }

            //this.InputTree.NodeLevelOverrides[0].ColumnSet = this.InputTree.ColumnSettings.ColumnSets["DataSet"];

            //foreach (InputDataSet ids in input.DataSets)
            //{
            //    UltraTreeNode dataset = this.InputTree.Nodes.Add(ids.Key);

            //    dataset.Cells[0].Value = ids.Key ;
            //    dataset.Cells[1].Value = ids.Description ;

            //    UltraTreeNode inputParameterNode = dataset.Nodes.Add();
            //    inputParameterNode.Text = "Input Parameters";

            //    List<Step> steps = ids.InputParameters.Steps;

            //    foreach (Step s in steps)
            //    {
            //        UltraTreeNode stepNode = inputParameterNode.Nodes.Add();
            //        stepNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["Step"];

            //        stepNode.Cells[0].Value = s.Name;

            //        foreach (Parameter p in s.Parameters)
            //        {
            //            UltraTreeNode paraNode = stepNode.Nodes.Add();

            //            paraNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["Parameter"];

            //            paraNode.Cells[0].Value = p.Key;
            //            paraNode.Cells[1].Value = p.Value;
            //        }   
            //    }

            //    UltraTreeNode expectedResultNode = dataset.Nodes.Add();
            //    expectedResultNode.Text = "Expected Result";

            //    steps = ids.ExpectedValues.Steps;

            //    foreach (Step s in steps)
            //    {
            //        UltraTreeNode stepNode = expectedResultNode.Nodes.Add();
            //        stepNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["Step"];

            //        stepNode.Cells[0].Value = s.Name;

            //        foreach (Parameter p in s.Parameters)
            //        {
            //            UltraTreeNode paraNode = stepNode.Nodes.Add();

            //            paraNode.Override.ColumnSet = this.InputTree.ColumnSettings.ColumnSets["Parameter"];

            //            paraNode.Cells[0].Value = p.Key;
            //            paraNode.Cells[1].Value = p.Value;
            //        }
            //    }
            //}

            this.InputTree.ExpandAll();
        }

    }
}