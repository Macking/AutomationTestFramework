using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class ImportService : PASBase
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
    }
}
