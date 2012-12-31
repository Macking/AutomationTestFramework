using System;
using System.Collections.Generic;
using System.Text;
using PAS.AutoTest.TestData;

namespace PAS.AutoTest.PasATCore
{
    public partial class ImportService : PASBase //Web Service call methods
    {
        public ImportService()
        {
            this.InitialService("Import");
        }

        public XMLResult importObject(string PatientInternalId, string StudyInternalId, string objectFileFullPath, string archivePath, bool Override, string move)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("importObject", new object[] { PatientInternalId,  StudyInternalId,  objectFileFullPath, archivePath,  Override, true, move }));
            return this.lastResult;
        }

        public XMLResult importDir(string PatientInternalId, string DirectoryPath, string Flag)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("importDir", new object[] { PatientInternalId, DirectoryPath,Flag }));
            return this.lastResult;
        }
    }

    public partial class ImportService : PASBase  //Utility methods
    {
        #region Public utility methods
        public XMLResult CallImportAndCheck(Round r, string patientInternalId, string objectFileFullPath, string archivePath)
        {
            CheckPoint cpImport = new CheckPoint("Import", "Call importSvc.ImportObjects to import a simple instance");
            r.CheckPoints.Add(cpImport);

            XMLResult rtImport = this.importObject(patientInternalId, null, objectFileFullPath, archivePath, true, "false");
            if (rtImport.IsErrorOccured)
            {
                cpImport.Result = TestResult.Fail;
                cpImport.Outputs.AddParameter("Import Simple Instance", "Return error:", rtImport.ResultContent);
            }
            else
            {
                cpImport.Result = TestResult.Pass;
                cpImport.Outputs.AddParameter("Import Simple Instance", "Return success:", rtImport.ResultContent);
            }
            return rtImport;
        }
        #endregion
    }
}
