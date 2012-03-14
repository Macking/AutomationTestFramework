using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AutoTestComponent
{
    [Serializable]
    public class TestConfigFile
    {
        //Test Environment parameters will be put on XML file
        //Here is the class to paser these parameters

        #region QCConnect
        //Connect Quality Center Parameters
        private QCConnectConfig QCConnectField;
        [XmlElementAttribute(IsNullable = false)]
        public QCConnectConfig QCConnect
        {
            get { return this.QCConnectField; }
            set { this.QCConnectField = value; }
        }
        #endregion

        #region CaseFilter
        //Filter for list test cases
        private CaseFilterConfig CaseFilterField;
        [XmlElementAttribute(IsNullable = false)]
        public CaseFilterConfig CaseFilter
        {
            get { return this.CaseFilterField; }
            set { this.CaseFilterField = value; }
        }
        #endregion

        #region RunParameter
        //Running Paramters
        private RunParameterConfig RunParameterField;
        [XmlElementAttribute(IsNullable = false)]
        public RunParameterConfig RunParameter
        {
            get { return this.RunParameterField; }
            set { this.RunParameterField = value; }
        }
        #endregion

        #region TestLog
        //Test log parameters
        private TestlogConfig TestLogField;
        [XmlElementAttribute(IsNullable = false)]
        public TestlogConfig TestLog
        {
            get { return this.TestLogField; }
            set { this.TestLogField = value; }
        }
        #endregion

        #region SubClass
        [System.SerializableAttribute()]
        public class QCConnectConfig
        {
            private string ServerAddrField;
            [XmlElementAttribute(IsNullable = false)]
            public string ServerAddr
            {
                get { return this.ServerAddrField; }
                set { this.ServerAddrField = value; }
            }

            private string DomainField;
            [XmlElementAttribute(IsNullable = false)]
            public string Domain
            {
                get { return this.DomainField; }
                set { this.DomainField = value; }
            }

            private string ProjectField;
            [XmlElementAttribute(IsNullable = false)]
            public string Project
            {
                get { return this.ProjectField; }
                set { this.ProjectField = value; }
            }

            private string LoginNameField;
            [XmlElementAttribute(IsNullable = false)]
            public string LoginName
            {
                get { return this.LoginNameField; }
                set { this.LoginNameField = value; }
            }

            private string PasswordField;
            [XmlElementAttribute(IsNullable = false)]
            //public string Password
            //{
            //    get { return this.PasswordField; }
            //    set { this.PasswordField = value; }
            //}
            //Use DES to Encrypt and Decrypt string
            public string Password
            {
                get { return DesEncDec.Decrypt(this.PasswordField); }
                set { this.PasswordField = DesEncDec.Encrypt(value); }
            }
        }

        [System.SerializableAttribute()]
        public class RunParameterConfig
        {
            private string TestMachineField;
            [XmlElementAttribute(IsNullable = false)]
            public string TestMachine
            {
                get { return this.TestMachineField; }
                set { this.TestMachineField = value; }
            }

            private bool UsingQTPField;
            public bool UsingQTP
            {
                get { return this.UsingQTPField; }
                set { this.UsingQTPField = value; }
            }

            private string CustomerTestProField;
            [XmlElementAttribute(IsNullable = false)]
            public string CustomerTestPro
            {
                get { return this.CustomerTestProField; }
                set { this.CustomerTestProField = value; }
            }

            private string[] DownloadFileField;
            [XmlElementAttribute(IsNullable = false)]
            public string[] DownloadFile
            {
                get { return this.DownloadFileField; }
                set { this.DownloadFileField = value; }
            }

            private string NumOfPassedField;
            [XmlElementAttribute(IsNullable = false)]
            public string NumOfPassed
            {
                get { return this.NumOfPassedField; }
                set { this.NumOfPassedField = value; }
            }

            private string NumOfTotalField;
            [XmlElementAttribute(IsNullable = false)]
            public string NumOfTotal
            {
                get { return this.NumOfTotalField; }
                set { this.NumOfTotalField = value; }
            }
        }

        //define the filter's name and value
        [System.SerializableAttribute()]
        public class AutorunFilerConfig
        {
            private string FieldNameField;
            [XmlElementAttribute(IsNullable = true)]
            public string FieldName
            {
                get { return this.FieldNameField; }
                set { this.FieldNameField = value; }
            }

            private string FieldValueField;
            [XmlElementAttribute(IsNullable = true)]
            public string FieldValue
            {
                get { return this.FieldValueField; }
                set { this.FieldValueField = value; }
            }
        }

        //Filter Config can be fiter many segments
        [System.SerializableAttribute()]
        public class CaseFilterConfig
        {
            private string TestSetNameField;
            [XmlElementAttribute(IsNullable = false)]
            public string TestSetName
            {
                get { return this.TestSetNameField; }
                set { this.TestSetNameField = value; }
            }

            private AutorunFilerConfig[] AutorunFilterField;
            [XmlElementAttribute(IsNullable = false)]
            public AutorunFilerConfig[] AutorunFilter
            {
                get { return this.AutorunFilterField; }
                set { this.AutorunFilterField = value; }
            }
        }

        [System.SerializableAttribute()]
        public class TestlogConfig
        {
            private bool TestlogEnableField;
            [XmlIgnoreAttribute()]
            public bool TestlogEnable
            {
                get { return this.TestlogEnableField; }
                set { this.TestlogEnableField = value; }
            }

            private string TestlogPathField;
            [XmlElementAttribute(IsNullable = true)]
            public string TestlogPath
            {
                get { return this.TestlogPathField; }
                set { this.TestlogPathField = value; }
            }
        }
        #endregion

        /*
        public TestConfigFile()
        {
            //this.TestLog.TestlogEnable = false;
            QCConnectField = new QCConnectConfig();
            CaseFilterField = new CaseFilterConfig();
            RunParameterField = new RunParameterConfig();
            TestLogField = new TestlogConfig();
        }

        public bool Load(string XMLfile)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestConfigFile));
                FileStream stream = new FileStream(XMLfile, FileMode.Open, FileAccess.Read);
                TestConfigFile tc = new TestConfigFile();
                tc = (TestConfigFile)xs.Deserialize(stream);
                stream.Close();
                
                return true;
            }
            catch (Exception e)
            {
                Debug.Print("AutoIntSys:---Load Config File Error---");
                Debug.Print("AutoIntSys:" + e.Message);
            }
            return false;
        }
        */
    }
    /*
    class Utility
    {
        private TestConfigFile tc = new TestConfigFile();
        public TestConfigFile TestConfigString
        {
            set { this.tc = value; }
            get { return this.tc; }
        }

        public bool Load(string XMLfile)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestConfigFile));
                FileStream stream = new FileStream(XMLfile, FileMode.Open, FileAccess.Read);
                tc = (TestConfigFile)xs.Deserialize(stream);
                stream.Close();
                return true;
            }
            catch { }
            return false;
        }

        public bool Save(string XMLFile)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestConfigFile));
                TextWriter writer = new StreamWriter(XMLFile);
                xs.Serialize(writer, tc);
                writer.Close();
                return true;
            }
            catch { }
            return false;
        }
    }*/
}
