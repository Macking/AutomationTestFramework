using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class Analysis3DService : PASBase
    {
        public Analysis3DService()
        {
            this.InitialService("Analysis3D");
        }

        public XMLResult createAnalysis3D(string p01_instanceUID, XMLParameter p02_xmlAnalysis3DInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createAnalysis3D", new object[] { p01_instanceUID, p02_xmlAnalysis3DInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult deleteAnalysis3D(string analysis3DUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("deleteAnalysis3D", new object[] { analysis3DUID }));
            return this.lastResult;
        }

        public XMLResult getAnalysis3DInfo(string analysis3DUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getAnalysis3DInfo", new object[] { analysis3DUID }));
            return this.lastResult;
        }

        public XMLResult listImagesOfAnalysis3D(string analysis3DUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listImagesOfAnalysis3D", new object[] { analysis3DUID }));
            return this.lastResult;
        }

        public XMLResult setAnalysis3DInfo(string analysis3DUID, XMLParameter p02_xmlAnalysis3DInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setAnalysis3DInfo", new object[] { analysis3DUID, p02_xmlAnalysis3DInfo .GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult linkCrossSectionToAnalysis3D(string analysis3DUID, string crossSectionUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("linkCrossSectionToAnalysis3D", new object[] { analysis3DUID, crossSectionUID }));
            return this.lastResult;
        }

        public XMLResult listCrossSectionsOfAnalysis3D(string analysis3DUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listCrossSectionsOfAnalysis3D", new object[] { analysis3DUID }));
            return this.lastResult;
        }

        public XMLResult setAnalysis3DToCurrent(string analysis3DUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setAnalysis3DToCurrent", new object[] { analysis3DUID }));
            return this.lastResult;
        }
    }
}