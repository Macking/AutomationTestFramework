using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PAS.AutoTest.TestData
{
    /// <summary>
    /// represent a single Parameter used in input class
    /// </summary>
    public class Parameter
    {

        #region Private Members

        private string mStep = string.Empty;
        private string mKey = string.Empty;
        private string mValue = string.Empty;

        #endregion

        #region Properties
        /// <summary>
        /// parameter name, can not repeat in the same dataset.
        /// </summary>
        public string Key
        {
            get { return mKey; }
            set { this.mKey = value; }
        }

        /// <summary>
        /// parameter value.
        /// </summary>
        public string Value
        {
            get { return mValue; }
            set { this.mValue = value; }
        }

        /// <summary>
        /// step name
        /// </summary>
        public string Step
        {
            get { return mStep; }
            set { this.mStep = value; }
        }

        #endregion

        #region Constructors

        public Parameter() { }

        public Parameter(string step, string key, string value)
        {
            this.mStep = step;
            this.mKey = key;
            this.mValue = value;
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                Parameter temp = obj as Parameter;

                if (temp.Key == this.Key && temp.mStep ==this.mStep)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// convert to xml element.
        /// </summary>
        /// <returns></returns>
        public XmlElement ConvertToXml(XmlDocument doc)
        {
            XmlElement p = doc.CreateElement("Parameter");
            p.SetAttribute("Key", this.mKey);
            p.SetAttribute("Step", this.mStep);
            p.InnerText = this.mValue;

            return p;
        }

        /// <summary>
        /// initialize with xml node.
        /// </summary>
        /// <param name="para"></param>
        public Parameter(XmlNode para)
        {
            this.mKey = para.Attributes["Key"].Value;
            this.mStep = para.Attributes["Step"].Value;
            this.Value = para.InnerText;
        }

        #endregion
    }

    /// <summary>
    /// present a parameters collection.
    /// </summary>
    public class ParameterCollection
    {

        #region Private Members

        private List<Parameter> mParameters = new List<Parameter>();
        private string mKey = string.Empty;

        #endregion

        #region Constructors

        public ParameterCollection(string key)
        {
            this.mKey = key;
        }

        public ParameterCollection(XmlNode paras)
        {
            this.mKey = paras.Name;

            XmlNodeList ps = paras.SelectNodes("Parameter");

            foreach (XmlNode p in ps)
            {
                Parameter para = new Parameter(p);
                this.mParameters.Add(para);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// get the key value.
        /// </summary>
        public string Key
        {
            get { return mKey; }
        }

        /// <summary>
        /// the total count of paramenters in this set.
        /// </summary>
        public int Count
        {
            get { return this.mParameters.Count; }
        }

        /// <summary>
        /// get or set parameters list.
        /// </summary>
        public List<Parameter> Parameters
        {
            get { return this.mParameters; }
            set { this.mParameters = value; }
        }

        /// <summary>
        /// steps, read only.
        /// </summary>
        public List<Step> Steps
        {
            get
            {
                List<Step> result = new List<Step>();

                foreach (Parameter p in this.mParameters)
                {
                    if(!(result .Contains(new Step (p.Step ))))
                        result.Add (new Step(p.Step ));

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].Name == p.Step)
                        {
                            result[i].Parameters.Add(p);
                            break;
                        }
                    }
                }

                return result;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// get a single parameter object according to specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Parameter GetParameter(string key)
        {
            foreach (Parameter i in this.mParameters)
            {
                if (i.Key == key)
                    return i;
            }

            //if no one matchs.
            return null;
        }

        /// <summary>
        /// get inputparameter by sequence number.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Parameter GetParameter(int index)
        {
            if (index >= 0 && index < this.mParameters.Count)
                return this.mParameters[index];
            else
                return null;
        }

        /// <summary>
        /// set the specified InputParamenter's value according to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetParameterValue(string key, string value)
        {
            foreach (Parameter i in this.mParameters)
            {
                if (i.Key == key)
                {
                    i.Value = value;
                    break;
                }
            }
        }

        /// <summary>
        /// Inset a parameter into set.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddParameter(string step, string key, string value)
        {
            Parameter input = new Parameter(step,key, value);

            if (this.mParameters.Contains(input))
                return false;
            else
            {
                this.mParameters.Add(input);
                return true;
            }
        }

        /// <summary>
        /// remove the specifed parameter from set.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveParameter(string key)
        {
            Parameter input = new Parameter();
            input.Key = key;

            if (this.mParameters.Contains(input))
            {
                this.mParameters.Remove(input);
                return true;
            }
            else
                return false;
        }

        public XmlElement ConvertToXml(XmlDocument doc)
        {
            XmlElement paras = doc.CreateElement(this.mKey);

            foreach (Parameter p in this.mParameters)
            {
                paras.AppendChild(p.ConvertToXml(doc));
            }

            return paras;
        }

        #endregion
    }

    /// <summary>
    /// Present a paramenter collection organized by the step name.
    /// </summary>
    public class Step
    {
        #region Private Members

        private string mName=string .Empty;
        private List<Parameter> mParameters = new List<Parameter>();

        #endregion

        #region Constructors

        public Step() { }

        public Step(string name)
        {
            this.mName = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// get or set the step name.
        /// </summary>
        public string Name
        {
            get { return this.mName; }
            set { this.mName = value; }
        }

        /// <summary>
        /// get or set parameters collection.
        /// </summary>
        public List<Parameter> Parameters
        {
            get{return this.mParameters;}
            set{this.mParameters =value;}
        }

        #endregion

        #region Public Methords

        public Parameter GetParameter(string key)
        {
            Parameter result = null;

            foreach (Parameter p in this.mParameters)
            {
                if (key == p.Key)
                {
                    result = p;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// if the step name is the same, then the steps are same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Step temp = obj as Step;

            if (temp.Name == this.Name)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
