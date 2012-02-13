//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Net;
//using System.Net.Sockets;

//using PAS.AutoTest.PasATCore;
//using KDIS7ATCore;

//namespace PAS.AutoTest.Performance.TestCase
//{
//    public class OpenObjectDebug: TestCaseTemplate
//    {
//        #region Constructor

//        public OpenObjectDebug()
//        {
//            this.mRepeat = 1000;
//            this.mFunctionName = "OpenObjectDebug";
//            this.LoadConfig();
//        }

//        #endregion

//        #region 'Run' implement

//        public override void Run()
//        {
//            string openObjInfo = string.Empty;
//            string psId = string.Empty;
//            KDIS7ImageWindow iw = new KDIS7ImageWindow();
//            KDIS7ATAssistant ka = new KDIS7ATAssistant();
            
//            IPAddress serverIP = IPAddress.Parse("127.0.0.1");

//            ApplicationService appService = new ApplicationService();

//            try
//            {
//                string PatientId = CommonLib.CreatePatient();
//                ImportService import = new ImportService();
//                XMLResult importResult = import.importObject(PatientId, string.Empty, @"c:\PASPerformance\001.png", null,  true, string.Empty);
//                psId = importResult.MultiResults[1].GetParameterValueByIndex(0);

//                XMLParameter pa = new XMLParameter("two_dimension_viewer");
//                pa.AddParameter("internal_id", psId);

//                openObjInfo = pa.GenerateXML();
//            }
//            catch (Exception ex)
//            {
//                LogRecordType lr = new LogRecordType();
//                lr.FunctionName = this.mFunctionName;
//                lr.Lable = this.mLabel;
//                string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
//                lr.Message = ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
//                lr.Passed = false;

//                Log.AddRecord(lr);

//                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed, true);
//                this.RiseTestCaseCompleteEvent();

//                this.mExecuted = this.mRepeat;
//                this.mFailed = this.mRepeat;

//                return;
//            }

//            for (int i = 1; i <= this.mRepeat; i++)
//            {
//                LogRecordType lr = new LogRecordType();
//                lr.Lable = this.mLabel;
//                lr.FunctionName = this.mFunctionName;

//                try
//                {
//                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                    client.Connect(serverIP, 10000);

//                    bool c = client.Connected;

//                    while(c)
//                    {
//                        lr.Message += "Connected to CSI. ";
//                        client.Disconnect(false);
//                        client.Connect(serverIP, 10000);
//                        c = client.Connected;
//                    }

//                    client.Close();
                    
//                }
//                catch (Exception ex)
//                {
//                    lr.Message += ex.Message;
//                }

//                try
//                {

//                    XMLResult result = new XMLResult(appService.InvokeMethod("openObjects", new object[] { openObjInfo }));

//                    bool imageOpened = iw.ItemByID(psId).WaitProperty("ShownOnScreen", true, 30);
//                    ka.CleanApp("Default");

//                    lr.ResponseTime = appService.ResponseTime;
//                    lr.Passed = (!(result.IsErrorOccured) && imageOpened);

//                    if (!lr.Passed)
//                    {
//                        if (result.IsErrorOccured)
//                            lr.Message += result.Message;
//                        else
//                            lr.Message += "Failed to open CSI.";

//                        this.mFailed++;
//                    }
//                    else
//                    {
//                        lr.Message += result.SingleResult;
//                        this.mExectuedTime += lr.ResponseTime;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    this.mFailed++;
//                    lr.Passed = false;
//                    string innerText = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
//                    lr.Message += ex.Message + innerText == string.Empty ? string.Empty : "(" + innerText + ")";
//                }

//                this.mExecuted = i;
//                Log.AddRecord(lr);

//                this.RiseSingleCallCompleteEvent(lr.ResponseTime, lr.Passed);
//            }

//            this.RiseTestCaseCompleteEvent();
//        }

//        #endregion
//    }
//}
