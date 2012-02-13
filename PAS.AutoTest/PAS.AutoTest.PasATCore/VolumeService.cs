using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class VolumeService:PASBase
    {
        public VolumeService()
        {
            this.InitialService("Volume");
        }

        public XMLResult createVolume(string studyId,XMLParameterCollection volumeInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createVolume", new object[] { studyId, volumeInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult deleteVolume(string volumeUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("deleteVolume", new object[] { volumeUID }));
            return this.lastResult;
        }

        public XMLResult getStudyUID(string volumeUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getStudyUID", new object[] { volumeUID }));
            return this.lastResult;
        }

        public XMLResult getVolumeInfo(XMLParameter filter)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getVolumeInfo", new object[] { filter.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult listAnalyses3DOfVolume(string volumeUID, string filterCriteria)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listAnalyses3DOfVolume", new object[] { volumeUID, filterCriteria }));
            return this.lastResult;
        }

        public XMLResult listChildVolumesOfVolume(string volumeUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listChildVolumesOfVolume", new object[] { volumeUID }));
            return this.lastResult;
        }

        public XMLResult listCrossSectionsOfVolume(string volumeUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listCrossSectionsOfVolume", new object[] { volumeUID }));
            return this.lastResult;
        }

        public XMLResult listImagesOfVolume(string volumeUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listImagesOfVolume", new object[] { volumeUID }));
            return this.lastResult;
        }

        public XMLResult listMeshesOfVolume(string volumeUID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listMeshesOfVolume", new object[] { volumeUID }));
            return this.lastResult;
        }

        public XMLResult setVolume(string volumeUID,XMLParameter volumeCompleteInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("setVolume", new object[] { volumeUID, volumeCompleteInfo.GenerateXML() }));
            return this.lastResult;
        }

    }
}
