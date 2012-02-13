using System;
using System.Collections.Generic;
using System.Text;

using PAS.AutoTest.PasATCore;
using KDIS7ATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class OpenObjectWithCSINotOpen:TestCaseTemplate
    {
        #region Constructor

        public OpenObjectWithCSINotOpen()
        {
            this.mRepeat = 1000;
            this.mFunctionName = "OpenObjectWithCSINotOpen";
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            string openObjInfo = string.Empty;
            string psId = string.Empty;
            KDIS7ImageWindow iw = new KDIS7ImageWindow();
            KDIS7ATAssistant ka = new KDIS7ATAssistant();

            ApplicationService appService = new ApplicationService();

            try
            {
                string PatientId = CommonLib.CreatePatient();
                ImportService import = new ImportService();
                XMLResult importResult = import.importObject(PatientId, string.Empty, @"c:\PASPerformance\001.png", null, true, string.Empty);
                psId = importResult.MultiResults[1].GetParameterValueByIndex(0);

                XMLParameter pa = new XMLParameter("two_dimension_viewer");
                pa.AddParameter("internal_id", psId);

                openObjInfo = pa.GenerateXML();
            }
            catch (Exception ex)
            {
                LogRecordType lr = new LogRecordType();
                lr.FunctionName = this.mFunctionName;
                lr.Lable = this.mLabel;
                string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                lr.Message = ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
                lr.Passed = false;

                Log.AddRecord(lr);

                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed, true);
                this.RiseTestCaseCompleteEvent();

                this.mExecuted = this.mRepeat;
                this.mFailed = this.mRepeat;

                return;
            }

            for (int i = 1; i <= this.mRepeat; i++)
            {
                LogRecordType lr = new LogRecordType();
                lr.Lable = this.mLabel;
                lr.FunctionName = this.mFunctionName;

                try
                {
                    XMLResult result = new XMLResult(appService.InvokeMethod("openObjects", new object[] { openObjInfo }));

                    bool imageOpened = iw.ItemByID(psId).WaitProperty("ShownOnScreen", true, 30);

                    System.Threading.Thread.Sleep(3000);

                    ka.CleanApp("Default");

                    lr.ResponseTime = appService.ResponseTime;
                    lr.Passed = (!(result.IsErrorOccured) && imageOpened);

                    if (!lr.Passed)
                    {
                        if (result.IsErrorOccured)
                            lr.Message = "Call openObjects returns error: " + result.Message;
                        else
                            lr.Message = "Failed to open CSI.";

                        this.mFailed++;
                    }
                    else
                    {
                        lr.Message = result.SingleResult;
                        this.mExectuedTime += lr.ResponseTime;
                    }
                }
                catch (Exception ex)
                {
                    this.mFailed++;
                    lr.Passed = false;
                    string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                    lr.Message = "Call openObjects throws exception: " + ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
                }

                this.mExecuted = i;
                Log.AddRecord(lr);

                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed);

            }

            this.RiseTestCaseCompleteEvent();
        }

        #endregion
    }
}
