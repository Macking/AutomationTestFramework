using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using TDAPIOLELib;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using AutotestIntsys;
using System.Text.RegularExpressions;

namespace PAS.AutoTest.TestUtility
{
    public static partial class Utility
    {
        public static bool QC_UploadAttachementToCase(string caseName)
        {
            string caseID = string.Empty;

            MatchCollection vMatchs = Regex.Matches(caseName, @"(\d+)");
            if (vMatchs.Count >= 1)
            {
                caseID = vMatchs[vMatchs.Count - 1].Value; //get the last one, which should be the case ID
            }
            else
            {
                //error happens, no case ID found
                return false;
            }

            TestConfigFile tConfig = new TestConfigFile();
            bool success = false;
            success = LoadConfigFile(ref tConfig, "TestUtilityConfig.xml");
            if (success)
            {
                AutotestIntsys.QCOTAClass qcOnline = new AutotestIntsys.QCOTAClass(tConfig);
                if (qcOnline.Connect())
                {
                    QCOperation.QCAttachment taa = new QCOperation.QCAttachment();

                    bool upSuccess;
                    string uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\TestData\\";

                    string iodFile = caseName + ".iod";
                    if (File.Exists(uploadPath + iodFile))
                    {
                        upSuccess = (bool)taa.UploadAttachment(qcOnline.getTDConn(), "TEST", caseID, iodFile, uploadPath);
                        if (!success)
                        {
                            //error when upload
                            return false;
                        }
                    }

                    string codFile = caseName + ".cod";
                    if (File.Exists(uploadPath + codFile))
                    {
                        upSuccess = (bool)taa.UploadAttachment(qcOnline.getTDConn(), "TEST", caseID, codFile, uploadPath);
                        if (!success)
                        {
                            //error when upload
                            return false;
                        }
                    }

                    string xodFile = caseName + ".xod";
                    if (File.Exists(uploadPath + xodFile))
                    {
                        upSuccess = (bool)taa.UploadAttachment(qcOnline.getTDConn(), "TEST", caseID, xodFile, uploadPath);
                        if (!success)
                        {
                            //error when upload
                            return false;
                        }
                    }

                    qcOnline.DisConnect();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void QC_DownloadAttachementFromCase(string caseID)
        {
            TestConfigFile tConfig = new TestConfigFile();
            bool success = false;
            success = LoadConfigFile(ref tConfig, "TestUtilityConfig.xml");
            if (success)
            {
                AutotestIntsys.QCOTAClass qcOnline = new AutotestIntsys.QCOTAClass(tConfig);
                if (qcOnline.Connect())
                {
                    QCOperation.QCAttachment taa = new QCOperation.QCAttachment();

                    ArrayList attachments = new ArrayList();
                    attachments = (ArrayList)taa.DownloadAttachment(qcOnline.getTDConn(), "Test", "1406", "Query_1406.xml", "C:\\temp");
                }
                qcOnline.DisConnect();
            }

        }

        public static bool QC_RemoveAttachementFromCase(string caseName)
        {
            string caseID = string.Empty;
            MatchCollection vMatchs = Regex.Matches(caseName, @"(\d+)");
            if (vMatchs.Count >= 1)
            {
                caseID = vMatchs[vMatchs.Count - 1].Value; //get the last one, which should be the case ID
            }
            else
            {
                //error happens, no case ID found
                return false;
            }

            TestConfigFile tConfig = new TestConfigFile();
            bool success = false;
            bool isRemoveSuccess = false;

            success = LoadConfigFile(ref tConfig, "TestUtilityConfig.xml");
            if (success)
            {
                AutotestIntsys.QCOTAClass qcOnline = new AutotestIntsys.QCOTAClass(tConfig);

                if (qcOnline.Connect())
                {
                    QCOperation.QCAttachment taa = new QCOperation.QCAttachment();
                    isRemoveSuccess = (bool)taa.RemoveAttachment(qcOnline.getTDConn(), "TEST", caseID);
                    qcOnline.DisConnect();
                }

                return isRemoveSuccess;
            }
            else
            {
                return false;
            }
        }

        public static void QC_CreateCaseRunResult(string folderName)
        {
        }

        private static bool LoadConfigFile(ref TestConfigFile tcFile, string XMLfile)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestConfigFile));
                FileStream stream = new FileStream(XMLfile, FileMode.Open, FileAccess.Read);
                TestConfigFile tmpConfig = (TestConfigFile)xs.Deserialize(stream);
                stream.Close();
                tcFile = tmpConfig;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("---Load Config File Error---");
                Console.WriteLine("{0}", e.Message);
            }
            return false;
        }
    }

}