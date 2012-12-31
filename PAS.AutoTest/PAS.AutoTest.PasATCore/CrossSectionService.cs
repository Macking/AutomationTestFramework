using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class CrossSectionService : PASBase
    {
        public CrossSectionService()
        {
            this.InitialService("CrossSection");
        }

        public XMLResult createCrossSection(string volumeUID, XMLParameterCollection crossSectionInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createCrossSection", new object[] { volumeUID, crossSectionInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getCrossSectionInfo(string crossSectionUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getCrossSectionInfo", new object[] {crossSectionUID}));
            return this.lastResult;
        }

        public XMLResult getCrossSectionCompleteInfo(string crossSectionUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getCrossSectionCompleteInfo", new object[] { crossSectionUID }));
            return this.lastResult;
        }

        public XMLResult listImagesOfCrossSection(string crossSectionUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listImagesOfCrossSection", new object[] { crossSectionUID }));
            return this.lastResult;
        }

        public XMLResult deleteCrossSection(string crossSectionUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("deleteCrossSection", new object[] { crossSectionUID }));
            return this.lastResult;
        }
    }
}
