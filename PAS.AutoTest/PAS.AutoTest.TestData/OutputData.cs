using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using System.IO;

namespace PAS.AutoTest.TestData
{
    public class OutputData
    {
        #region Private Members

        private string mTestCaseName = string.Empty;
        private string mRunId = string.Empty;
        private DateTime mExecutedTime = System.DateTime.Now;
        private List<Round> mRounds = new List<Round>();

        #endregion

        #region Constructors

        public OutputData() { }

        public OutputData(string testcase, string runId)
        {
            this.mRunId = runId;
            this.mTestCaseName = testcase;
        }

        //public OutputData(string fileName)
        //{
        //    StreamReader sr = new StreamReader(fileName);
        //    this.Load(sr.ReadToEnd());
        //}

        public OutputData(string outputContent)
        {
            this.Load(outputContent);
        }

        #endregion

        #region Properties

        public string TestCase
        {
            get { return this.mTestCaseName; }
            set { this.mTestCaseName = value; }
        }

        /// <summary>
        /// get how many rounds in total.
        /// </summary>
        public int Repetition
        {
            get { return this.mRounds.Count; }
        }

        public string RunId
        {
            get { return this.mRunId; }
            set { this.mRunId = value; }
        }

        public DateTime ExecutedTime
        {
            get { return this.mExecutedTime; }
            set { this.mExecutedTime = value; }
        }

        public List<Round> Rounds
        {
            get { return this.mRounds; }
            set { this.mRounds = value; }
        }

        public ResultSummary Summary
        {
            get
            {
                ResultSummary s = new ResultSummary();
                foreach (Round r in this.mRounds)
                {
                    switch (r.Result)
                    {
                        case TestResult.Fail:
                            s.Failed++; break;
                        case TestResult.Pass:
                            s.Passed++; break;
                        case TestResult.Warning:
                            s.Warning++; break;
                        case TestResult.Incomplete:
                            s.Incomplete++; break;
                        case TestResult.Done:
                            s.Done++; break;
                    }
                }

                s.TotalRun = this.mRounds.Count;

                return s;
            }
        }

        public TestResult Result
        {
            get
            {
                ResultSummary s = this.Summary;
                if (s.Passed == this.mRounds.Count)
                    return TestResult.Pass;
                else
                    return TestResult.Fail;
            }
        }

        #endregion

        #region Public Methods

        public void Load(string xmlContent)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                XmlNode root = doc.SelectSingleNode("TestResult");
                this.mTestCaseName = root.Attributes["TestCase"].Value;
                this.mRunId = root.Attributes["RunId"].Value;
                this.mExecutedTime = Convert.ToDateTime(root.Attributes["Time"].Value);

                XmlNodeList rounds = root.SelectNodes("Round");
                foreach (XmlNode round in rounds)
                {
                    this.mRounds.Add(new Round(round));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

          public void ConvertFromString(string xmlContent)
          {
            try
            {
              XmlDocument doc = new XmlDocument();
              doc.LoadXml(xmlContent);

              XmlNode root = doc.SelectSingleNode("TestResult");
              this.mTestCaseName = root.Attributes["TestCase"].Value;
              this.mRunId = root.Attributes["RunId"].Value;
              this.mExecutedTime = Convert.ToDateTime(root.Attributes["Time"].Value);

              XmlNodeList rounds = root.SelectNodes("Round");
              foreach (XmlNode round in rounds)
              {
                this.mRounds.Add(new Round(round));
              }

            }
            catch (Exception)
            {
              throw;
            }
          }

        public XmlElement ConvertToXml(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("TestResult");
            root.SetAttribute("TestCase", this.mTestCaseName);
            root.SetAttribute("RunId", this.mRunId);
            root.SetAttribute("Time", this.ExecutedTime.ToLongDateString());

            foreach (Round r in this.mRounds)
            {
                root.AppendChild(r.ConvertToXml(doc));
            }

            return root;
        }

        public void ConvertToXml(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
              System.IO.File.Delete(fileName);
            }
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(this.ConvertToXml(doc));
            using (FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
              doc.Save(fs);
            }       
        }

        public string ConvertToString()
        {
          XmlDocument doc = new XmlDocument();
          doc.AppendChild(this.ConvertToXml(doc));
          return doc.OuterXml;
        }

        #endregion

    }

    /// <summary>
    /// represent a test result for one test dataset..\
    /// </summary>
    public class Round
    {
        #region Private Members

        private string mKey = string.Empty;
        private string mDes = string.Empty;
        private int mTime = 0;
        private List<CheckPoint> mCheckPoints = new List<CheckPoint>();
        private InputDataSet mDataSet = new InputDataSet();
        private ResultSummary mSummary = new ResultSummary();

        #endregion

        #region Constructors

        public Round() { }

        public Round(string key, string description)
        {
            this.mDes = description;
            this.mKey = key;
        }

        public Round(XmlNode round)
        {
            this.mKey = round.Attributes["Key"].Value;
            this.mDes = round.Attributes["Des"].Value;
            this.mTime = Convert.ToInt32(round.Attributes["Time"].Value);
            this.mDataSet = new InputDataSet(round.SelectSingleNode("DataSet"));

            XmlNodeList cps = round.SelectNodes("CheckPoint");
            foreach (XmlNode cp in cps)
            {
                this.CheckPoints.Add(new CheckPoint(cp));
            }
        }

        #endregion

        #region Properties

        public string Key
        {
            get { return Key; }
            set { this.mKey = value; }
        }

        public string Description
        {
            get { return mDes; }
            set { this.mDes = value; }
        }

        public int Time
        {
            get { return mTime; }
            set { this.mTime = value; }
        }

        public InputDataSet DataSet
        {
            get { return this.mDataSet; }
            set { this.mDataSet = value; }
        }

        public List<CheckPoint> CheckPoints
        {
            get { return this.mCheckPoints; }
            set { this.mCheckPoints = value; }
        }

        public ResultSummary Summary
        {
            get
            {
                foreach (CheckPoint cp in this.mCheckPoints)
                {
                    switch (cp.Result)
                    {
                        case TestResult.Done:
                            this.mSummary.Done++; break;
                        case TestResult.Fail:
                            this.mSummary.Failed++; break;
                        case TestResult.Incomplete:
                            this.mSummary.Incomplete++; break;
                        case TestResult.Pass:
                            this.mSummary.Passed++; break;
                        case TestResult.Warning:
                            this.mSummary.Warning++; break;
                    }
                }
                return this.mSummary;
            }
        }

        public TestResult Result
        {
            get
            {
                if (this.Summary.Failed != 0)
                    return TestResult.Fail;
                else if (this.Summary.Incomplete != 0)
                    return TestResult.Incomplete;
                else if (this.Summary.Warning != 0)
                    return TestResult.Warning;
                else
                    return TestResult.Pass;
            }
        }

        #endregion

        #region Public Methods

        public XmlElement ConvertToXml(XmlDocument doc)
        {
            XmlElement round = doc.CreateElement("Round");
            round.SetAttribute("Key", this.mKey);
            round.SetAttribute("Des", this.mDes);
            round.SetAttribute("Time", this.mTime.ToString());
            round.AppendChild(this.mDataSet.ConvertToXml(doc));

            foreach (CheckPoint cp in this.mCheckPoints)
            {
                round.AppendChild(cp.ConvertToXml(doc));
            }

            return round;
        }

        public string ConvertToXml()
        {
            return this.ConvertToXml(new XmlDocument()).OuterXml;
        }

        #endregion
    }

    public class ResultSummary
    {
        public int Passed = 0;
        public int Failed = 0;
        public int Warning = 0;
        public int Done = 0;
        public int Incomplete = 0;
        public int TotalRun = 0;
    }
}
