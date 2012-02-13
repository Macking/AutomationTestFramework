using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Collections;
using System.Threading;


namespace PAS.AutoTest.Performance.TestCase
{
    public class Log
    {
        public static string mDBPath = @"C:\PasPerformance.mdb";
        private static List<LogRecordType> mRecords = new List<LogRecordType>();
        private static object mSyncObj = new object();
        private static int mUpdateFrequency = 5000;
        private static bool mIsInsertThreadWorking = false;

        public static DataSet Select(string selstr)
        {
            DataSet ds = new DataSet();

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                "Data Source =" + mDBPath;

            try
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(selstr, conn);
                da.Fill(ds);
            }
            catch (Exception)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        public static void StartUpdateSync()
        {
            if (!mIsInsertThreadWorking)
            {
                Thread t = new Thread(InsertRecordsSync);
                t.Start();

                mIsInsertThreadWorking = true;
            }
        }

        private static void InsertRecordsSync()
        {
            while (true)
            {
                InsertRecords();
                Thread.Sleep(mUpdateFrequency);
            }
        }

        private static void InsertRecords()
        {
            if (mRecords.Count == 0)
            {
                return;
            }

            lock (mSyncObj)
            {
                OleDbConnection conn = new OleDbConnection();
                conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source =" + mDBPath;

                string insertStr = string.Empty;

                try
                {
                    conn.Open();
                    OleDbCommand insertCommand = new OleDbCommand();
                    insertCommand.Connection = conn;

                    foreach (LogRecordType lr in mRecords)
                    {
                        insertStr = "INSERT INTO PerformanceLog( Label, ExecutedTime, Passed, Message, ResponseTime, FunctionName) VALUES ('" +
                        lr.Lable + "','" +
                        lr.ExecutedTime.ToString () + "'," +
                        lr.Passed + ",'" +
                        lr.Message + "'," +
                        lr.ResponseTime + ",'" +
                        lr.FunctionName + "')";

                        insertCommand.CommandText = insertStr;
                        insertCommand.ExecuteNonQuery();
                    }

                    mRecords.Clear();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }

            return;
        }

        public static void AddRecord(LogRecordType lr)
        {
            lock (mSyncObj)
            {
                mRecords.Add(lr);
            }
        }

        public static List<string> LoadLabels()
        {
            DataSet ds=Select("Select Distinct Label from PerformanceLog ");

            List<string> result = new List<string>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                result.Add(ds.Tables[0].Rows[i][0].ToString());
            }

            return result;
        }

        public static List<string> LoadTestCaseNames(string label)
        {
            List<string> result = new List<string>();
            DataSet ds = Select("Select Distinct FunctionName from PerformanceLog where Label='"+label+"'");

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                result.Add(ds.Tables[0].Rows[i][0].ToString());
            }

            return result;
        }

        public static DataSet LoadTestCaseData(string label, string functionName)
        {
            return Select("Select ResponseTime, ExecutedTime from PerformanceLog where Label='" + label + "' AND FunctionName = '" + functionName + "' AND Passed=True ORDER BY ExecutedTime ASC" );
        }

        public static int GetMaxResponse(string label, string functionName)
        {
            int result = 0;

            DataSet ds = Select("Select Max(Responsetime) from PerformanceLog where Label='" + label + "' AND FunctionName = '" + functionName + "' AND Passed=True");

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result =Int32.Parse (ds.Tables[0].Rows[0][0].ToString());
                
            }

            return result;
        }

        public static int GetMinResponse(string label, string functionName)
        {
            int result = 0;

            DataSet ds = Select("Select Min(Responsetime) from PerformanceLog where Label='" + label + "' AND FunctionName = '" + functionName + "' AND Passed=True");

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result = Int32.Parse(ds.Tables[0].Rows[0][0].ToString());

            }

            return result;
        }

        public static int GetAvgResponse(string label, string functionName)
        {
            int result = 0;

            DataSet ds = Select("Select Avg(Responsetime) from PerformanceLog where Label='" + label + "' AND FunctionName = '" + functionName + "' AND Passed=True");

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result = (int)(double.Parse(ds.Tables[0].Rows[0][0].ToString()));

            }

            return result;
        }

        public static int GetRepeat(string label, string functionName)
        {
            int result = 0;

            DataSet ds = Select("Select Count(Responsetime) from PerformanceLog where Label='" + label + "' AND FunctionName = '" + functionName + "'");

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result = (int)(double.Parse(ds.Tables[0].Rows[0][0].ToString()));
            }

            return result;
        }

        public static int GetErrors(string label, string functionName)
        {
            int result = 0;

            DataSet ds = Select("Select Count(Responsetime) from PerformanceLog where Label='" + label + "' AND FunctionName = '" + functionName + "' AND Passed=False");

            if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result = (int)(double.Parse(ds.Tables[0].Rows[0][0].ToString()));
            }

            return result;
        }

        public static DataSet GetFailedCalls(string label, string functionName)
        {
            return Select("Select Id, Message, ResponseTime from PerformanceLog where  Label='" + label + "' AND FunctionName = '" + functionName + "' AND Passed=False");

        }
    }

    public class LogRecordType
    {
        public string Lable=string.Empty;
        public DateTime ExecutedTime=System.DateTime .Now;
        public string FunctionName=string.Empty;
        public bool Passed = true;
        public string Message = string.Empty;
        public double ResponseTime = 0;
    }
}
