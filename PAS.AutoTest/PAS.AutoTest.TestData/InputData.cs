using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections .ObjectModel ;
using System.IO;

namespace PAS.AutoTest.TestData
{
    /// <summary>
    /// represent an input test data file.
    /// </summary>
    public class InputData
    {
        #region Private Members

        private string mDes = string.Empty;
        private List<InputDataSet> mDataSets = new List<InputDataSet>();

        #endregion

        #region Constructors

        /// <summary>
        /// empty constructor
        /// </summary>
        public InputData() { }

        /// <summary>
        /// initialize with the xml file name.
        /// </summary>
        /// <param name="fileName"></param>
        public InputData(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            this.LoadFromXml(sr.ReadToEnd());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the description for the inputdata file.
        /// </summary>
        public string Description
        {
            get { return mDes; }
            set { this.mDes = value; }
        }

        /// <summary>
        /// count of input datasets.
        /// </summary>
        public int Repetition
        {
            get { return this.mDataSets.Count; }
        }

        /// <summary>
        /// get or set the dataset array.
        /// </summary>
        public List<InputDataSet> DataSets
        {
            get { return this.mDataSets; }
            set { this.mDataSets = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// insert a new dataset.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public bool AddInputDataSet(InputDataSet dataset)
        {
            if (this.mDataSets.Contains(dataset))
                return false;
            else
            {
                this.mDataSets.Add(dataset);
                return true;
            }
        }

        /// <summary>
        /// Get dataset by key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public InputDataSet GetDataSet(string key)
        {
            for (int i = 0; i < this.mDataSets.Count; i++)
            {
                if (this.mDataSets[i].Key.ToUpper().Trim() == key.ToUpper().Trim())
                {
                    return this.mDataSets[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Get dataset by index.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public InputDataSet GetDataSet(int index)
        {
            if (index >= 0 && index < this.mDataSets.Count)
                return this.mDataSets[index];
            else
                return null;
        }

        /// <summary>
        /// remove the specified input dataset.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public bool RemoveInputDataSet(InputDataSet dataset)
        {
            if (this.mDataSets.Contains(dataset))
            {
                this.mDataSets.Remove(dataset);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// initialize with xml string.
        /// </summary>
        /// <param name="xmlContent"></param>
        public void LoadFromXml(string xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            this.mDataSets.Clear();

            try
            {
                doc.LoadXml(xmlContent);
                XmlNode root = doc.SelectSingleNode("Input");

                //set description.
                foreach (XmlAttribute a in root.Attributes)
                {
                    if (a.Name.ToUpper() == "DES")
                    {
                        this.mDes = a.Value;
                        break;
                    }
                }

                //add datasets.
                XmlNodeList datasets = root.SelectNodes("DataSet");
                foreach (XmlNode dataset in datasets)
                {
                    InputDataSet set = new InputDataSet(dataset);
                    this.AddInputDataSet(set);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Convert to xml in string format.
        /// </summary>
        /// <returns></returns>
        public string ConvertToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Input");

            root.SetAttribute("Des", this.mDes);

            //datasets
            foreach (InputDataSet dataset in this.mDataSets)
            {
                root.AppendChild(dataset.ConvertToXml(doc));
            }

            doc.AppendChild(root);
            return doc.OuterXml;
        }

        /// <summary>
        /// export to xml file, specify the full path of file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Export(string fileName)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(this.ConvertToXml());

                doc.Save(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// represent a set of parameters which used in one repetition.
    /// </summary>
    public class InputDataSet
    {
        #region Private Members

        private string mKey = string.Empty;
        private string mDes = string.Empty;
        private ParameterCollection mInputParameters = new ParameterCollection("InputParameters");
        private ParameterCollection mExpectedValues = new ParameterCollection("ExpectedValues");

        #endregion

        #region Constructors

        public InputDataSet() { }

        public InputDataSet(string key, string description)
        {
            this.mKey = key;
            this.mDes = description;
        }

        public InputDataSet(XmlNode ds)
        {
            //attributes
            foreach (XmlAttribute a in ds.Attributes)
            {
                if (a.Name.ToUpper() == "KEY")
                {
                    this.mKey = a.Value;
                }
                else if (a.Name.ToUpper() == "DES")
                {
                    this.mDes = a.Value;
                }
            }

            this.mInputParameters = new ParameterCollection(ds.SelectSingleNode("InputParameters"));
            this.mExpectedValues = new ParameterCollection(ds.SelectSingleNode("ExpectedValues"));
        }

        #endregion

        #region Properties

        /// <summary>
        /// inputs
        /// </summary>
        public ParameterCollection InputParameters
        {
            get { return this.mInputParameters; }
            set { this.mInputParameters = value; }
        }

        /// <summary>
        /// expected.
        /// </summary>
        public ParameterCollection ExpectedValues
        {
            get { return this.mExpectedValues; }
            set { this.mExpectedValues = value; }
        }

        /// <summary>
        /// name for a test dataset, can not repeat in the same input file.
        /// </summary>
        public string Key
        {
            get { return this.mKey; }
            set { this.mKey = value; }
        }

        /// <summary>
        /// description for this dataset purpose.
        /// </summary>
        public string Description
        {
            get { return this.mDes; }
            set { this.mDes = value; }
        }

        #endregion

        #region Public Methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                InputDataSet set = obj as InputDataSet;

                if (set.Key == this.Key)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public XmlElement ConvertToXml(XmlDocument doc)
        {
            XmlElement dataset = doc.CreateElement("DataSet");

            dataset.SetAttribute("Key", this.mKey);
            dataset.SetAttribute("Des", this.mDes);

            dataset.AppendChild(this.mInputParameters.ConvertToXml(doc));
            dataset.AppendChild(this.mExpectedValues.ConvertToXml(doc));

            return dataset;
        }

        public string ConvertToXml()
        {
            return this.ConvertToXml(new XmlDocument()).OuterXml;
        }

        #endregion

    }
}
