using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
    public class FileService : PASBase
    {
        public FileService()
        {
            this.InitialService("FileService");
        }

        public XMLResult addFileURL(XMLParameter fileURL)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("addFileURL", new object[] { fileURL.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult downloadFile(XMLParameter fileURL)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("downloadFile", new object[] { fileURL.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult listFileURL(string imageUid)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listFileURL", new object[] { imageUid }));
            return this.lastResult;
        }

        public XMLResult listLocalFiles(string localPASUid)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listLocalFiles", new object[] { localPASUid }));
            return this.lastResult;
        }

        public XMLResult registerLocalPAS(XMLParameter localPasInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("registerLocalPAS", new object[] { localPasInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult removeFileURL(string fileUrlUid)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("removeFileURL", new object[] { fileUrlUid }));
            return this.lastResult;
        }

        public XMLResult testUpload(string localPasId)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("testUpload", new object[] { localPasId }));
            return this.lastResult;
        }

        public XMLResult uploadFile(XMLParameter fileURL)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("uploadFile", new object[] { fileURL.GenerateXML() }));
            return this.lastResult;
        }
    }
}
