using System;
using System.Collections.Generic;
using System.Text;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class ImportObject:TestCaseTemplate 
    {
        #region Constructor

        public ImportObject()
        {
            this.mRepeat = 10000;
            this.mFunctionName = "ImportObject";
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            //create a new patient.
            string newPatientId = CommonLib.CreatePatient();

            ImportService importService = new ImportService();

            for (int i = 1; i <= this.mRepeat; i++)
            {
                LogRecordType lr = new LogRecordType();
                lr.FunctionName = this.mFunctionName;
                lr.Lable = this.mLabel;

                try
                {
                    XMLResult result = importService.importObject(newPatientId, string.Empty, @"C:\PASPerformance\001.png", null,true, string.Empty);
                    lr.ResponseTime = importService.ResponseTime;
                    lr.Passed = !(result.IsErrorOccured);

                    if (!lr.Passed)
                    {
                        this.mFailed++;
                        lr.Message = result.Message;
                    }
                    else
                    {
                        this.mExectuedTime += importService.ResponseTime;
                        lr.Message = result.Message;
                    }
                }
                catch (Exception ex)
                {
                    string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                    lr.Message = ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
                    lr.Passed = false;
                    this.mFailed ++;
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
