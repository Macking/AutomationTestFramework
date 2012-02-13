using System;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.PasATCore
{
    public class ImageService:PASBase 
    {
        public ImageService()
        {
            this.InitialService("Image");
        }

        public XMLResult createImage(XMLParameter imageInfo)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("createImage", new object[] { imageInfo.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getCephTracing(string imageInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getCephTracing", new object[] { imageInternalID }));
            return this.lastResult;
        }

        public XMLResult setCephTracing(string cephTracing, string imageInternalID)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("setCephTracing", new object[] { cephTracing, imageInternalID }));
            return this.lastResult;
        }

      public XMLResult deleteImage(string imageInternalID, XMLParameter preferences)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("deleteImage", new object[] { imageInternalID, preferences.GenerateXML() }));
            return this.lastResult;
        }

        public XMLResult getImageDescription(string imageInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("getImageDescription", new object[] { imageInternalID }));
            return this.lastResult;
        }

        public XMLResult listPresentationState(XMLParameter filter, string imageInternalID)
        {
            this.lastResult = new XMLResult(this.InvokeMethod("listPresentationState", new object[] { filter.GenerateXML(), imageInternalID}));
            return this.lastResult;
        }

        public XMLResult getImageInfo(XMLParameter imageInternalID)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("getImageInfo", new object[] { imageInternalID.GenerateXML() }));
          return this.lastResult;
        }

      public XMLResult setImageInfo(XMLParameter imageInfo, string imageInternalID)
        {
          this.lastResult = new XMLResult(this.InvokeMethod("setImageInfo", new object[] { imageInfo.GenerateXML(), imageInternalID }));
          return this.lastResult;
        }
    }
}
