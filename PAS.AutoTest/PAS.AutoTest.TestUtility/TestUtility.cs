using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Net;
using System.Collections;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.TestUtility
{
    public struct XMLValidateResult
    {
        public bool isValid;
        public string message;
    }

    public enum CSDMConfigSection
    {
        remote,
        proxy,
        local,
        common
    }

    public static partial class Utility
    {
        #region OS and Process operation
        /// <summary>
        /// Gets the OS version.
        /// </summary>
        /// <returns></returns>
        public static string GetOSVersion()
        {
            System.OperatingSystem os = Environment.OSVersion;
            System.Version v = os.Version;

            if (v.Major == 5 && v.Minor == 1) return "WinXP";
            if (v.Major == 6 && v.Minor != 0) return "Win7";

            return "Unknown OS";
        }

        /// <summary>
        /// Checks the process existed.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <returns></returns>
        public static bool CheckProcessExist(string processName)
        {
            return !CheckProcess(processName, 0);
        }

        /// <summary>
        /// Checks the process.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns></returns>
        public static bool CheckProcess(string processName, int expectedCount)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            return (processes.Length == expectedCount);
        }

        /// <summary>
        /// Kills the process.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        public static void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process p in processes)
            {
                p.Kill();
                Thread.Sleep(2000);
            }
        }

        #endregion

        #region CSDM config file operation
        /// <summary>
        /// Gets the CSDM config.
        /// </summary>
        /// <param name="section">The section. Possible value can be: remote, proxy, local, common</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetCSDMConfig(CSDMConfigSection section, string key)
        {
            string value = string.Empty;
            string configFilePath = string.Empty;

            configFilePath = GetCSDMConfigFilePath();

            //Open the Config file and get the specific config value
            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(configFilePath);

            XmlNodeList configNodes = configDoc.SelectNodes("trophy/" + section.ToString() + "/property");

            foreach (XmlNode node in configNodes)
            {
                if (node.Attributes["key"].Value.Trim().ToUpper() == key.Trim().ToUpper())
                {
                    value = node.Attributes["value"].Value;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the CSDM config file path.
        /// </summary>
        /// <returns></returns>
        public static string GetCSDMConfigFilePath()
        {
            string CSDMConfigFilePath = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
            if (GetOSVersion() == "WinXP") 
                CSDMConfigFilePath = CSDMConfigFilePath + "\\Application Data\\TW\\PAS\\csdm.config.xml";
            if (GetOSVersion() == "Win7") 
                CSDMConfigFilePath = CSDMConfigFilePath + "\\TW\\PAS\\csdm.config.xml";
            return CSDMConfigFilePath;
        }

        /// <summary>
        /// Sets the CSDM config.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetCSDMConfig(CSDMConfigSection section, string key, string value)
        {
            XmlDocument configDoc = new XmlDocument();
            configDoc.Load(GetCSDMConfigFilePath());
            XmlNodeList configNodes = configDoc.SelectNodes("trophy/" + section.ToString() + "/property");
            foreach (XmlNode node in configNodes)
            {
                if (node.Attributes["key"].Value.Trim().ToUpper() == key.Trim().ToUpper())
                {
                    node.Attributes["value"].Value = value;
                }
            }
            configDoc.Save(GetCSDMConfigFilePath());

        }
        #endregion

        #region CSDM operation
        /// <summary>
        /// Starts the CSDM.
        /// </summary>
        public static void StartCSDM()
        {
            string cmdPath = null;

            cmdPath = Environment.GetEnvironmentVariable("CommonProgramFiles(x86)");

            if (cmdPath == null)
            {
                cmdPath = Environment.GetEnvironmentVariable("CommonProgramFiles");
            }

            cmdPath = cmdPath + "\\Trophy\\PAS\\CSDataMgr.exe";

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = cmdPath;
            startInfo.Arguments = "/start";
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForInputIdle();

            int count = 0;
            do
            {
                Thread.Sleep(5000);
                count++;
            } while (!IsCSDMWebServiceAlive() && count < 12);
        }

        /// <summary>
        /// Stops the CSDM.
        /// </summary>
        public static void StopCSDM()
        {
            string cmdPath = null;

            cmdPath = Environment.GetEnvironmentVariable("CommonProgramFiles(x86)");

            if (cmdPath == null)
            {
                cmdPath = Environment.GetEnvironmentVariable("CommonProgramFiles");
            }

            cmdPath = cmdPath + "\\Trophy\\PAS\\CSDataMgr.exe";

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = cmdPath;
            startInfo.Arguments = "/stop";
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();

            int count = 0;
            do
            {
                Thread.Sleep(3000);
                count++;
            } while (IsCSDMWebServiceAlive() && count < 10);
        }

        /// <summary>
        /// Restarts the CSDM.
        /// </summary>
        /// <param name="waitTime">The wait time.</param>
        public static void RestartCSDM(int waitTime)
        {
            StopCSDM();
            StartCSDM();
        }


        /// <summary>
        /// Exits the controller.
        /// </summary>
        public static void ExitController()
        {
            string cmdPath = null;

            cmdPath = Environment.GetEnvironmentVariable("CommonProgramFiles(x86)");

            if (cmdPath == null)
            {
                cmdPath = Environment.GetEnvironmentVariable("CommonProgramFiles");
            }

            cmdPath = cmdPath + "\\Trophy\\PAS\\CSDataMgr.exe";

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = cmdPath;
            startInfo.Arguments = "/quit";
            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();

            Thread.Sleep(3000);
        }
        /// <summary>
        /// Updates the CSDM DB.
        /// </summary>
        /// <returns></returns>
        public static int UpdateCSDMDB()
        {
            string cmdPath = string.Empty;

            cmdPath = GetCSDMConfig(CSDMConfigSection.proxy, "webAppsPath").Replace("webapps", ""); // Get the parent path, such as: C:/Program Files (x86)/Carestream/CS Data Manager/PAS/ 
            cmdPath = cmdPath + "\\bin\\upgradeDB.bat";

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = cmdPath;

            string dbPath = GetCSDMConfig(CSDMConfigSection.remote, "databaseDirectory") + "\\pas";
            string backupPath = GetCSDMConfig(CSDMConfigSection.remote, "patientDirectory").Replace("patient", "dbbackup");
            startInfo.Arguments = "\"" + dbPath + "\" \"" + backupPath + "\"";

            p.StartInfo = startInfo;
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }

        /// <summary>
        /// Determines whether [is CSDM web service alive].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is CSDM web service alive]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCSDMWebServiceAlive()
        {
            try
            {
                string port = GetCSDMConfig(CSDMConfigSection.local, "httpPort");
                string uri = "http://localhost:8080/pas/services/VersionService?wsdl".Replace("8080", port);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                request.UseDefaultCredentials = true;
                request.Method = "GET";
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (WebException e)
            {
                System.Diagnostics.Trace.Write(e.Message);
                return false;
            }
        }
        #endregion

        #region File and Directory operation
        /// <summary>
        /// Gets the file creation time.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string GetFileCreationTime(string filePath)
        {
            string creationTime = null;
            try
            {
                // Example: 2012-07-11T18:59:12+08:00
                creationTime = XmlConvert.ToString(System.IO.File.GetCreationTime(filePath), "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
            }
            catch (Exception ex)
            {
                creationTime = ex.Message;
            }
            return creationTime;
        }

        /// <summary>
        /// Gets the file modified time.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string GetFileModifiedTime(string filePath)
        {
            string modifiedTime = null;
            try
            {
                // Example: 2012-07-11T18:59:12+08:00
                modifiedTime = XmlConvert.ToString(System.IO.File.GetLastWriteTime(filePath), "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
            }
            catch (Exception ex)
            {
                modifiedTime = ex.Message;
            }
            return modifiedTime;
        }

        /// <summary>
        /// Determines whether [is time equal now] [the specified time]. The inaccuracy is set to 300s
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        ///   <c>true</c> if [is time equals now] [the specified time]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTimeEqualNow(string time)
        {
            DateTime dt = DateTime.Parse(time);
            TimeSpan timeDiff = DateTime.Now.Subtract(dt);

            if (timeDiff.TotalSeconds <= 300 && timeDiff.TotalSeconds >= -300)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is image exist in server DB] [the specified image path].
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <returns>
        ///   <c>true</c> if [is image exist in server DB] [the specified image path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImageExistInServerDB(string imageLocalPath)
        {
            bool isExist = false;
            WebResponse response = null;

            try
            {
                string fileURL = GetImageURL(imageLocalPath);
                WebRequest request = WebRequest.Create(fileURL);
                response = request.GetResponse();
                isExist = response == null ? false : true;
            }
            catch (Exception)
            {
                isExist = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return isExist;
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <returns></returns>
        private static string GetImageURL(string imagePath)
        {
            string fileURL = string.Empty;

            if (imagePath.Contains("patient")) // Basic validation of the path is correct
            {
                //imagePath example: C:/ProgramData/TW/PAS/pas_data/patient/1/8afa203131467f230131468106e30001/8afa203131467f2301314682ef3b01a1/8afa203131467f2301314682ef3b01a2.dcm
                //fileURL example: http://localhost:8080/pas_data/patient/1/8afa203131467f230131468106e30001/8afa203131467f2301314682ef3b01a1/8afa203131467f2301314682ef3b01a2.dcm
                string host = GetCSDMConfig(CSDMConfigSection.remote, "host");
                string httpPort = GetCSDMConfig(CSDMConfigSection.remote, "httpPort");
                fileURL = "http://" + host + ":" + httpPort + "/pas_data/" + imagePath.Substring(imagePath.IndexOf("patient"));
            }

            return fileURL;
        }

        /// <summary>
        /// Determines whether [is file existed] [the specified file path].
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        ///   <c>true</c> if [is file existed] [the specified file path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool isFileExisted(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return fi.Exists;
        }

        /// <summary>
        /// Gets the file num of directory.
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns></returns>
        public static int getFileNumOfDirectory(string dirPath)
        {
            DirectoryInfo di = new DirectoryInfo(dirPath);
            return di.GetFiles().Length;
        }

        /// <summary>
        /// Determines whether [is directory empty] [the specified dir path].
        /// </summary>
        /// <param name="dirPath">The dir path.</param>
        /// <returns>
        ///   <c>true</c> if [is directory empty] [the specified dir path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool isDirectoryEmpty(string dirPath)
        {
            DirectoryInfo di = new DirectoryInfo(dirPath);
            if (di.GetFiles().Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private extern static IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr PostMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDlgCtrlID(IntPtr HWND);

        [DllImport("csdminterfacetest.dll")]
        public extern static int DICOMDIR_test();

        [DllImport("csdminterfacetest.dll")]
        public extern static int version_test();

        [DllImport("csdminterfacetest.dll")]
        public extern static int listObject_test();

        [DllImport("csdminterfacetest.dll")]
        private static extern int test();
        public static int TestInterface()
        {
            return test();
        }

        public static bool AcqPanoImage(int timeOut)
        {
            return ClickAcqButton("IDC_BUTTON_ETATPRET", timeOut);
        }

        public static bool AcqCephImage(int timeOut)
        {
            return ClickAcqButton("IDC_BUTTON_ETATPRET", timeOut);
        }

        public static bool AcqCRImage(int timeOut)
        {
            return ClickAcqButton("DSCAN", timeOut);
        }

        public static bool AcqTWAINImage(int timeOut)
        {
            return ClickAcqButton("SCAN", timeOut);
        }

        public static bool Acq3DVolume(int timeOut)
        {
            return ClickAcqButton("IDC_BUTTON_ETATPRET", timeOut);
        }

        private static bool ClickAcqButton(string btnName, int timeOut)
        {
            bool result = false;

            int attemp = 0;
            IntPtr desktop = GetDesktopWindow();
            IntPtr child = FindWindowEx(desktop, IntPtr.Zero, null, null);
            IntPtr c = IntPtr.Zero;

            while (child != IntPtr.Zero)
            {
                c = FindWindowEx(child, IntPtr.Zero, null, btnName);

                if (c != IntPtr.Zero)
                {
                    PostMessage(c, 0x0201, 0, 0);
                    PostMessage(c, 0x0202, 0, 0);

                    while (c != IntPtr.Zero && attemp != timeOut)
                    {
                        System.Threading.Thread.Sleep(1000);
                        c = FindWindowEx(child, IntPtr.Zero, null, btnName);
                        attemp++;
                    }

                    if (c == IntPtr.Zero)
                        result = true;

                    break;
                }
                child = FindWindowEx(desktop, child, null, null);
            }

            return result;
        }

        public static bool AcqFMS(int waitTime, int timeOut)
        {
            bool result = false;

            int attemp = 0;
            IntPtr desktop = GetDesktopWindow();
            IntPtr acqPanel = FindWindowEx(desktop, IntPtr.Zero, null, null);
            IntPtr c = IntPtr.Zero;

            while (acqPanel != IntPtr.Zero)
            {
                c = FindWindowEx(acqPanel, IntPtr.Zero, null, "FMSPanelWnd");

                if (c != IntPtr.Zero)
                {
                    IntPtr btnClose = FindWindowEx(acqPanel, IntPtr.Zero, null, null);

                    while (btnClose != IntPtr.Zero)
                    {
                        int id = (int)(GetDlgCtrlID(btnClose));

                        if (id == 6054)
                        {
                            System.Threading.Thread.Sleep(waitTime * 1000);
                            PostMessage(btnClose, 0x0201, 0, 0);
                            PostMessage(btnClose, 0x0202, 0, 0);
                            break;
                        }

                        btnClose = FindWindowEx(acqPanel, btnClose, null, null);
                    }

                    while (c != IntPtr.Zero && attemp != timeOut)
                    {
                        System.Threading.Thread.Sleep(1000);
                        c = FindWindowEx(acqPanel, IntPtr.Zero, null, "wxCellContainerFMS");
                        attemp++;
                    }

                    if (c == IntPtr.Zero)
                        result = true;

                    break;
                }

                acqPanel = FindWindowEx(desktop, acqPanel, null, null);
            }

            return result;
        }

        public static void AcquireCephOne()
        {
            AcqCephImage(50);
        }

        public static void AcquirePanoOne()
        {
            AcqPanoImage(300);
        }

        public static void AcquireFMSOne()
        {
            AcqFMS(40, 300);
        }

        #region Other
        /// <summary>
        /// Validates the XML per XSD.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <param name="xsdFile">The XSD file.</param>
        /// <returns></returns>
        public static XMLValidateResult ValidateXMLPerXSD(string xmlString, string xsdFile)
        {
            xmlString = xmlString.Replace("<tns:path xsi:nil=\"true\"/>", "");
            xmlString = xmlString.Replace("<tns:seriesInstanceUid xsi:nil=\"true\"/>", "");
            xmlString = xmlString.Replace("<tns:seriesDate xsi:nil=\"true\"/>", "");
            //xmlString = xmlString.Replace("<tns:tags/>", "<tns:tags>noarchived</tns:tags>");

            XMLValidateResult validateResult = new XMLValidateResult();
            validateResult.isValid = true;
            validateResult.message = string.Empty;


            XmlReaderSettings readerSettings = new XmlReaderSettings();

            readerSettings.Schemas.Add("http://www.carestreamhealth.com/CSI/CSDM/1/Schema", xsdFile);  // Note: if xsd namespace changes, need change this accordingly

            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationEventHandler += new ValidationEventHandler(delegate(object sender, ValidationEventArgs e)
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    validateResult.isValid = false;
                }

                validateResult.message = e.Message;
            });

            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));

            mStream.Position = 0;

            XmlReader xmlReader = XmlReader.Create(mStream, readerSettings);

            while (xmlReader.Read())
            {
                if (validateResult.isValid == false) // there is node not valid, stop the check
                {
                    break;
                }
            }

            xmlReader.Close();

            return validateResult;
        }

        /// <summary>
        /// Get parse type(Image, FMS, Analysis, simpleInstance.etc, message of post import, and return the id list of imported object according to the type
        /// </summary>
        /// <param name="Type">The type your want to return</param>
        /// <returns>Idlist of the imported object </string></returns>
        public static List<string> parsePostImportResult(string parseType, string postImportInputString)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"<trophy type=.*$");
            System.Text.RegularExpressions.Match m = reg.Match(postImportInputString);
            System.Xml.XmlDocument importres = new System.Xml.XmlDocument();
            importres.LoadXml(m.Value);
            System.Xml.XmlNodeList objs = importres.SelectNodes("//item[@type='" + parseType + "' and @errorCode=0]");
            System.Collections.Generic.List<string> idList = new System.Collections.Generic.List<string>();
            foreach (System.Xml.XmlNode obj in objs)
            {
                foreach (System.Xml.XmlAttribute attr in obj.Attributes)
                {
                    if (attr.Name == "instanceInternalID")
                    {
                        idList.Add(attr.Value);
                    }
                }
            }
            return idList;
        }

        /// <summary>
        /// Determines whether [is acquisition in progress].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is acquisition in progress]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAcquisitionInProgress()
        {
            string mutexId = string.Format("{{{0}}}", "41B81342-4F7E-4859-80CD-212D2B1141A2");
            
            try
            {
                using (Mutex mutex = Mutex.OpenExisting(mutexId))
                {
                    if (mutex != null)
                    {
                        //Console.WriteLine("Runnning");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return false;
            }
            catch (Exception)
            {
                //throw;
                return false;
            }
        }
        #endregion
    }



    public class ProcessingXMLInfo
    {
        private string rotate180Enabled;

        public string Rotate180Enabled
        {
            get { return rotate180Enabled; }
            set { rotate180Enabled = value; }
        }
        private string rotate90REnabled;

        public string Rotate90REnabled
        {
            get { return rotate90REnabled; }
            set { rotate90REnabled = value; }
        }
        private string brightnessEnabled;

        public string BrightnessEnabled
        {
            get { return brightnessEnabled; }
            set { brightnessEnabled = value; }
        }
        private string brightnessValue;

        public string BrightnessValue
        {
            get { return brightnessValue; }
            set { brightnessValue = value; }
        }
        private string contrastEnabled;

        public string ContrastEnabled
        {
            get { return contrastEnabled; }
            set { contrastEnabled = value; }
        }
        private string contrastValue;

        public string ContrastValue
        {
            get { return contrastValue; }
            set { contrastValue = value; }
        }
    }

    public class ProcessingXMLParser
    {
        private string processingXML;
        public ProcessingXMLParser(string processingXML)
        {
            this.processingXML = processingXML;
        }

        public ProcessingXMLInfo parse()
        {
            ProcessingXMLInfo pinfo = new ProcessingXMLInfo();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.processingXML);
            XmlNodeList algos = doc.SelectNodes("//Algo");
            foreach (XmlNode algo in algos)
            {
                switch (algo.Attributes["id"].Value)
                {
                    case "10009":
                        pinfo.Rotate180Enabled = algo.Attributes["enabled"].Value;
                        break;
                    case "10010":
                        pinfo.Rotate90REnabled = algo.Attributes["enabled"].Value;
                        break;
                    case "20045":
                        pinfo.BrightnessEnabled = algo.Attributes["enabled"].Value;
                        pinfo.BrightnessValue = algo.Attributes["value"].Value;
                        break;
                    case "20046":
                        pinfo.ContrastEnabled = algo.Attributes["enabled"].Value;
                        pinfo.ContrastValue = algo.Attributes["value"].Value;
                        break;
                }
            }
            return pinfo;
        }
    }

    public static class base64tools
    {
        //Encode a common string to base64 text
        public static string EncodeString2base(string NeedEncodeStr)
        {
            byte[] data = System.Text.UnicodeEncoding.UTF8.GetBytes(NeedEncodeStr);
            Base64Encoder myEncoder = new Base64Encoder(data);
            StringBuilder sb = new StringBuilder();
            sb.Append(myEncoder.GetEncoded());
            return sb.ToString();
        }

        //Decode a base64 string to common string
        public static string DecodeString4base(string EncodedString)
        {
            char[] data = EncodedString.ToCharArray();
            Base64Decoder myDecoder = new Base64Decoder(data);
            StringBuilder sb = new StringBuilder();
            byte[] temp = myDecoder.GetDecoded();
            sb.Append(System.Text.UTF8Encoding.UTF8.GetChars(temp));
            return sb.ToString();
        }

    }

    public class ParseMessageContent
    {
        private Dictionary<string, string> MessageContent = new Dictionary<string,string>();

        //Constructor for class, need provide the command message string
        public ParseMessageContent(string msgCon)
        {
            string submsgCon = msgCon;
            while (submsgCon.Length != 0)
            {
                string key = string.Empty;
                string value = string.Empty;
                int indexKey = submsgCon.IndexOf("=");
                if(indexKey!=-1)
                    key = submsgCon.Substring(0, indexKey);                    
                else
                    break;

                int indexValue = submsgCon.IndexOf(" ");
                if(indexValue!=-1)
                {
                    value = submsgCon.Substring(indexKey + 1, indexValue - indexKey - 1);
                }
                else
                {
                    value = submsgCon.Substring(indexKey + 1, submsgCon.Length - indexKey - 1);
                    if (key != string.Empty && value != string.Empty)
                        MessageContent.Add(key, value);
                    break;
                }
                if(key != string.Empty && value != string.Empty)
                    MessageContent.Add(key, value);

                submsgCon = submsgCon.Substring(indexValue + 1, submsgCon.Length - indexValue - 1);               
            }
        }

        //Get the value by provided key in the command message, if no value or no key, return null
        public string getValueFromKey(string keyName)
        {
            string sss = string.Empty;
            try
            {
                sss = MessageContent[keyName];
            }
            catch (KeyNotFoundException ex)
            {
                sss = null;
            }            
            return sss;
        }
    }

    public class ParseXMLContent
    {
        private string processXML = string.Empty;
        List<KeyValuePair<string, string>> AttributeContent = new List<KeyValuePair<string, string>>();        
        List<KeyValuePair<string, string>> SubNodeContent = new List<KeyValuePair<string, string>>();

        //constructor for XML content
        public ParseXMLContent(string XMLContent)
        {
            processXML = XMLContent;
        }

        //constructor for XML file
        public ParseXMLContent(string filePath, string type)
        {
            bool dicomfileexist = Utility.isFileExisted(filePath);
            if (dicomfileexist)
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(filePath);
                processXML = xd.InnerXml;
            }
        }

        //obsolete
        public bool getValueFromPath(string path)
        {
            string pathValue = string.Empty;
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(processXML);
            XmlNodeList nodes = xd.SelectNodes(path);
            foreach (XmlNode node in nodes)
            {                
                for (int i=0;i<node.Attributes.Count;i++)
                {
                    KeyValuePair<string, string> kvpAtt = new KeyValuePair<string, string>(node.Attributes[i].Name, node.Attributes[i].Value);
                    AttributeContent.Add(kvpAtt);
                    //AttributeContent.Add(node.Attributes[i].Name, node.Attributes[i].Value);
                }
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode subnode = node.ChildNodes[i];
                    KeyValuePair<string, string> kvpSub = new KeyValuePair<string, string>(subnode.Attributes[0].Value, subnode.Attributes[1].Value);
                    SubNodeContent.Add(kvpSub);
                }              
            }
            return true;
        }

        //get specified content from SDK out, especially FMS
        public string getStringFromPath(string path)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(processXML);
            XmlNodeList nodes = xd.SelectNodes(path);
            string imageName = "";
            foreach (XmlNode node in nodes)
            {
                if (node.InnerText == "")
                {
                    continue;
                }
                else
                {
                    imageName += node.InnerText + ";";
                }                
            }
            return imageName;
        }

        //get specified value from trophy xml
        public string getStringWithPathAndType(string path, string speciaValueForIndex, string getValueByKey)
        {            
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(processXML);           
            path += "/parameter[@type='" + speciaValueForIndex + "']";
            XmlNodeList nodes = xd.SelectNodes(path);
            List<string> allRecords = new List<string>();
            string recordString = "";
            foreach (XmlNode node in nodes)
            {                
                for (int i = 0; i < node.Attributes.Count; i++ )
                {
                    if (node.Attributes[i].Name == getValueByKey)
                    {
                            recordString += node.Attributes[i].Value + ";";
                    }
                }
             
            }
            return recordString;
        }

        //get specified value from result xml
        public string getResultWithKey(string path, string keyv)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(processXML);
            path += "/parameter[@key='" + keyv + "']";
            XmlNodeList nodes = xd.SelectNodes(path);
            List<string> allRecords = new List<string>();
            string recordString = "";
            foreach (XmlNode node in nodes)
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].Name == "value")
                    {
                        recordString += node.Attributes[i].Value + ";";
                    }
                }

            }
            return recordString;
        }

        //get specified value from mapping file xml
        public string getWithPathAndTypeFromMappingFile(string path, string speciaValueForIndex, string getValueByKey)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(processXML);
            path += "/instance[@type='" + speciaValueForIndex + "']";
            XmlNodeList nodes = xd.SelectNodes(path);
            List<string> allRecords = new List<string>();
            string recordString = "";
            foreach (XmlNode node in nodes)
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].Name == getValueByKey)
                    {
                        recordString += node.Attributes[i].Value + ";";
                    }
                }

            }
            return recordString;
        }

        //obsolete
        public string getValueByKey(string Key)
        {
            string sss = string.Empty;
            try
            {
                if (AttributeContent.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in AttributeContent)
                    {
                        if (kvp.Key == Key)
                        {
                            sss += kvp.Value + ";";
                        }
                    }
                    //sss = AttributeContent[Key];
                }
                if (SubNodeContent.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kvp in SubNodeContent)
                    {
                        if (kvp.Key == Key)
                        {
                            sss += kvp.Value + ";";
                        }
                    }
                }
                
            }
            catch (KeyNotFoundException ex)
            {
                sss = null;
            }
            return sss;
        }
    }

    public class Start2DSimulator
    {
        public Start2DSimulator(int port)
        {
            Thread th = new Thread(startSocket);
            th.Start(port);
        }

        private void startSocket(object port)
        {
            TwoDSim.simulator si = new TwoDSim.simulator((int)port);
            si.StartSimulater("0,OK");
            si.StopSimulator(1000);
        }
    }


}