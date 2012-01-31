using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoTestComponent
{
    public class TestConfigUtility
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
    }
}
