using System;
using System.Collections.Generic;
using System.Text;

using PAS.AutoTest.PasATCore;

namespace PAS.AutoTest.Performance.TestCase
{
    public class SetAsynAcqPatientInfo : TestCaseTemplate
    {
        #region Constructor

        public SetAsynAcqPatientInfo()
        {
            this.mRepeat = 4000;
            this.mFunctionName = "SetAsynAcqPatientInfo";
            this.LoadConfig();
        }

        #endregion

        #region 'Run' implement

        public override void Run()
        {
            AcquisitionService acqService = new AcquisitionService();
            double timeOut = 60000; //60000ms, 60s
            
            XMLParameter acqInfo = new XMLParameter("acq_info");

            try
            {
                string PatientId = CommonLib.CreatePatient();
                acqInfo.AddParameter("patient_internal_id", PatientId);
                acqInfo.AddParameter("teeth_number", "12");
                acqInfo.AddParameter("teeth_number_notation", "american");
                acqInfo.AddParameter("output_encoding", "unicode");
                acqInfo.AddParameter("series_performing_physician_name", "performing_physician_name");
                acqInfo.AddParameter("mpps_information", "mpps_information");
                acqInfo.AddParameter("equipment_department_name", "equipment_department_name");
                acqInfo.AddParameter("equipment_institution_name", "equipment_institution_name");
                acqInfo.AddParameter("equipment_institution_address", "equipment_institution_address");
                acqInfo.AddParameter("equipment_station_name", "equipment_station_name");
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
                    XMLResult result = new XMLResult();

                    lr.ResponseTime = 0;
                    do
                    {
                        ExactTimer timer = new ExactTimer();
                        timer.Start();
                        result = acqService.setAsynAcqPatientInfo(acqInfo);
                        timer.Stop();

                        lr.ResponseTime += timer.Duration;

                        System.Threading.Thread.Sleep(100);
                    } while (result.ResultContent == null && lr.ResponseTime < timeOut); //loop while there is no return, and the time out is 60s

                    lr.Passed = !(result.IsErrorOccured);

                    if (!lr.Passed)
                    {
                        if (lr.ResponseTime >= timeOut)
                            lr.Message = "the service did not return after 60s. " + result.Message;
                        else
                            lr.Message = "the service return errors: " + result.Message;

                        this.mFailed++;
                    }
                    else // lr.Passed = true
                    {
                        lr.Message = result.Message;
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
                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed);
            }

            this.RiseTestCaseCompleteEvent();
        }

        #endregion
    }
}
