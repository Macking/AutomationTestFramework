using System;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class GetSetPS : TestCaseTemplate
    {
        #region Constructor

        public GetSetPS()
        {
            this.mRepeat = 100000;
            this.mFunctionName = "GetSetPS";
            this.mConcurrent = 5;
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            string newPatientId = string.Empty;
            string psId = string.Empty;
            string PsInfoString = string.Empty;

            PresentationStateService ps = new PresentationStateService();

            try
            {
                //create a new patient and import a new image to get a new ps id.
                newPatientId = CommonLib.CreatePatient();
                ImportService import = new ImportService();
                XMLResult importResult = import.importObject(newPatientId, string.Empty, @"c:\PASPerformance\001.png", null,  true, string.Empty);
                psId = importResult.MultiResults[1].GetParameterValueByIndex(0);

                //Get PS info string.
                PsInfoString = ps.getPresentationState(psId).ArrayResult.GenerateXML();
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

                double userCaseResponse = 0;

                try
                {
                    //get ps
                    XMLResult getPsResult = ps.getPresentationState(psId);

                    if (getPsResult.IsErrorOccured)
                    {
                        lr.Passed = false;
                        lr.Message = getPsResult.Message;
                        lr.ResponseTime = ps.ResponseTime;
                        Log.AddRecord(lr);

                        this.mFailed++;
                        continue;
                    }
                    else
                        userCaseResponse = ps.ResponseTime;

                    //set ps
                    XMLResult setPsResult = new XMLResult(ps.InvokeMethod("setPresentationState", new object[] { PsInfoString, psId }));

                    if (setPsResult.IsErrorOccured)
                    {
                        lr.Passed = false;
                        lr.Message = setPsResult.Message;
                        lr.ResponseTime = ps.ResponseTime;
                        Log.AddRecord(lr);

                        this.mFailed++;
                        continue;
                    }
                    else
                        userCaseResponse += ps.ResponseTime;

                    //if no exception.
                    lr.Passed = true;
                    lr.ResponseTime = userCaseResponse;

                    this.mExectuedTime += userCaseResponse;
                }
                catch (Exception ex)
                {
                    lr.Passed = false;
                    string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                    lr.Message = ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";

                    this.mFailed++;
                }

                Log.AddRecord(lr);
                this.mExecuted = i;

                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed);
            }

            this.RiseTestCaseCompleteEvent();
        }
        #endregion
    }
}
