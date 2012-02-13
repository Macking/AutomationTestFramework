using System;
using System.Collections.Generic;
using System.Text;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class AcquireImage:TestCaseTemplate
    {
        #region Constructor

        public AcquireImage()
        {
            this.mRepeat = 4000;
            this.mFunctionName = "AcquireImage";
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            string acqInfoString = string.Empty;
            string sessionId = string.Empty;
            AcquisitionService acqService = new AcquisitionService();
            ExactTimer timer = new ExactTimer();
            int timeOut=600;

            try
            {
                LaunchNotification();
                string PatientId = CommonLib.CreatePatient();
            
                XMLParameter acqInfo = new XMLParameter("acq_info");
                acqInfo.AddParameter("device_id", "AcqImgFiles.dll");
                acqInfo.AddParameter("line_id", "308C0000");
                acqInfo.AddParameter("patient_internal_id", PatientId);
                acqInfo.AddParameter("series_performing_physician_name", "performing_physician_name");
                acqInfoString = acqInfo.GenerateXML();
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
                    int attemp = 0;
                    int errorCode = 272;  //the error code for session existed.
                    XMLResult result=new XMLResult ();

                    timer.Start();

                    while (errorCode == 272)
                    {
                        result = new XMLResult(acqService.InvokeMethod("startAcquisition", new object[] { acqInfoString }));
                        errorCode = result.Code;
                    }

                    sessionId = result.SingleResult;
                    XMLResult acqResult = acqService.getAcquisitionResult(sessionId);

                    while (acqResult.IsErrorOccured && attemp < timeOut)  //loop while the result code is 0, and the time out is 60s
                    {
                        acqResult = acqService.getAcquisitionResult(sessionId);
                        System.Threading.Thread.Sleep(100);
                        attemp++;
                    }

                    timer.Stop();

                    lr.ResponseTime = timer.Duration;
                    lr.Passed = !(acqResult.IsErrorOccured);

                    if (!lr.Passed)
                    {
                        if (attemp >= timeOut)
                            lr.Message = "the service did not return after 60s. "+acqResult.Message;
                        else
                            lr.Message = acqResult.Message;

                        this.mFailed++;
                    }
                    else
                    {
                        lr.Message = result.SingleResult +"code: "+ result.Code;
                        this.mExectuedTime += lr.ResponseTime;
                    }
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
                if(i == this.mRepeat)
                  FinishNotification();
                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed);
            }

            this.RiseTestCaseCompleteEvent();
        }

        #endregion
    }
}
