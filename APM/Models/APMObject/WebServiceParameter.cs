using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject
{
    public class WebServiceParameter
    {
        public long CoreObjectID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string QueryValue { get; set; }
        public bool ConvertToJsonArr { get; set; }
        public WebServiceParameter()
        {
            ConvertToJsonArr=false;
        }
        public WebServiceParameter(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(WebServiceParameter));
            var WebServiceInfo = serializer.Deserialize(stringReader) as WebServiceParameter;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.Name = WebServiceInfo.Name;
            this.Value = WebServiceInfo.Value;
            this.QueryValue = WebServiceInfo.QueryValue;
            this.ConvertToJsonArr = WebServiceInfo.ConvertToJsonArr;
        }
    }
}