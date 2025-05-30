using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using static APM.Models.Tools.CoreDefine;

namespace APM.Models.APMObject
{
    public class WebService
    {
        public string URL { get; set; }
        public string ExportType { get; set; }
        public long CoreObjectID { get; set; }

        public WebService()
        {

        }
        public WebService(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(WebService));
            var WebServiceInfo = serializer.Deserialize(stringReader) as WebService; 
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.URL = WebServiceInfo.URL;
            this.ExportType = WebServiceInfo.ExportType; 
        }
    }
}