using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PAS.AutoTest.TestData
{
    /// <summary>
    /// represent a checkpoint object.
    /// </summary>
    public class CheckPoint
    {
        #region Private Members

        //private ParameterCollection mExpected = new ParameterCollection("ExpectedValues");
        //private ParameterCollection mInputs = new ParameterCollection("InputParameters");
        private ParameterCollection mOutputs = new ParameterCollection("Outputs");
        private string mKey = string.Empty;
        private string mDes = string.Empty;
        private TestResult mResult = new TestResult();
        private string mMessage = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// empty constructor
        /// </summary>
        public CheckPoint() { }

        /// <summary>
        /// initialize with key and des.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        public CheckPoint(string key, string description)
        {
            this.mKey = key;
            this.mDes = description;
        }

        /// <summary>
        /// initialize with xml element.
        /// </summary>
        /// <param name="cp"></param>
        public CheckPoint(XmlNode cp)
        {
            this.mKey = cp.Attributes["Key"].Value;
            this.mDes = cp.Attributes["Des"].Value ;

           // this.mInputs = new ParameterCollection(cp.SelectSingleNode("InputParameters"));
            //this.mExpected = new ParameterCollection(cp.SelectSingleNode("ExpectedValues"));
            this.mOutputs = new ParameterCollection(cp.SelectSingleNode("Outputs"));
            this.mMessage = cp.SelectSingleNode("Message").InnerText;
            this.mResult = (TestResult)Enum.Parse(typeof(TestResult), cp.SelectSingleNode("Result").InnerText);
        }

        #endregion

        #region Properties

        /// <summary>
        /// checkpoint key
        /// </summary>
        public string Key
        {
            get { return this.mKey; }
            set { this.mKey = value; }
        }

        /// <summary>
        /// checkpoint description.
        /// </summary>
        public string Description
        {
            get { return this.mDes; }
            set { this.mDes = value; }
        }

        ///// <summary>
        ///// inputs parameters.
        ///// </summary>
        //public ParameterCollection Inputs
        //{
        //    get { return this.mInputs; }
        //    set { this.mInputs = value; }
        //}

        /// <summary>
        /// checkpoint output values.
        /// </summary>
        public ParameterCollection Outputs
        {
            get { return this.mOutputs; }
            set { this.mOutputs = value; }
        }

        ///// <summary>
        ///// expected values.
        ///// </summary>
        //public ParameterCollection ExpectedValues
        //{
        //    get { return this.mExpected; }
        //    set { this.mExpected = value; }
        //}

        /// <summary>
        /// Gets or sets checkpoint result.
        /// </summary>
        public TestResult Result
        {
            get { return this.mResult; }
            set { this.mResult = value; }
        }

        #endregion

        #region Public Methods

        public XmlElement ConvertToXml(XmlDocument doc)
        {
            XmlElement cp = doc.CreateElement("CheckPoint");

            cp.SetAttribute("Key", this.mKey);
            cp.SetAttribute("Des", this.mDes);

            //cp.AppendChild(this.mInputs.ConvertToXml(doc));
            //cp.AppendChild(this.mExpected.ConvertToXml(doc));
            cp.AppendChild(this.mOutputs.ConvertToXml(doc));

            XmlElement result = doc.CreateElement("Result");
            result.InnerText = this.mResult.ToString();
            cp.AppendChild(result);

            XmlElement msg = doc.CreateElement("Message");
            msg.InnerText = this.mMessage;
            cp.AppendChild(msg);

            return cp;
        }

        public string ConvertToXml()
        {
            return this.ConvertToXml(new XmlDocument()).OuterXml;
        }

        #endregion

    }

    /// <summary>
    /// Checkpoint result codes.
    /// </summary>
    public enum TestResult
    {
        Pass,
        Fail,
        Done,
        Incomplete,
        Warning
    }

}
