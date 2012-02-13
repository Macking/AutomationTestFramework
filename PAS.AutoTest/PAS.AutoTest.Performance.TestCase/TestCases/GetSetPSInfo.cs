using System;
using System.Collections.Generic;
using System.Text;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class GetSetPSInfo: TestCaseTemplate 
    {
        #region Constructor

        public GetSetPSInfo()
        {
            this.mRepeat = 100000;
            this.mFunctionName = "GetSetPSInfo";
            this.mConcurrent = 5;
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            string newPSId = string.Empty;
            string setPSInfoString = string.Empty;
            string getPSInfoString = string.Empty;
            PresentationStateService psService = new PresentationStateService();

            try
            {
                //create a new patient and import a new image to get a new ps id.
                string newPatientId = CommonLib.CreatePatient();
                ImportService import = new ImportService();
                XMLResult r = import.importObject(newPatientId, string.Empty, @"c:\PASPerformance\001.png", null, true, string.Empty);
                newPSId = r.MultiResults[1].GetParameterValueByIndex(0);
                XMLParameter psId = new XMLParameter("presentationstate");
                psId.AddParameter("internal_id", newPSId);
                getPSInfoString = psId.GenerateXML();
                XMLParameterCollection c = new XMLParameterCollection();
                c.Add(psId);
                setPSInfoString = psService.getPresentationStateInfo(c).ArrayResult.GenerateXML();     
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

            double userCaseResponse=0;

            for (int i = 1; i <= this.mRepeat; i++)
            {
                LogRecordType lr = new LogRecordType();
                lr.Lable = this.mLabel;
                lr.FunctionName = this.mFunctionName;

                try
                {
                    //get ps info
                    XMLResult getResult=new XMLResult (psService.InvokeMethod ("getPresentationStateInfo", new object []{getPSInfoString }));

                    //if get presentation infor failed.
                    if (getResult.IsErrorOccured)
                    {
                        lr.Message = "Get PS info faild. Message: " + getResult.Message;
                        lr.ResponseTime = psService.ResponseTime;
                        lr.Passed = false;
                        Log.AddRecord(lr);
                        this.mFailed++;
                        continue;
                    }
                    else  //if get ps info successed 
                    {
                        userCaseResponse = psService.ResponseTime;
                    }

 
                    //set ps info
                    XMLResult setResult = new XMLResult(psService.InvokeMethod("setPresentationStateInfo", new object[] { setPSInfoString, newPSId}));
                    
                    //if set ps info failed
                    if (setResult.IsErrorOccured)
                    {
                        lr.Message = "Set PS info failed. Message: " + setResult.Message;
                        lr.ResponseTime = psService.ResponseTime;
                        lr.Passed = false;
                        Log.AddRecord(lr);
                        this.mFailed++;
                        continue;
                    }
                    else  //if set ps info successed.
                    {
                        userCaseResponse += psService.ResponseTime;
                    }

                    lr.ResponseTime = userCaseResponse;
                    this.ExecutedTime += userCaseResponse;
                }
                catch (Exception ex)
                {
                    this.mFailed++;
                    lr.Passed = false;
                    string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                    lr.Message = ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
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
