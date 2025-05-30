using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject.InformationForm
{
    public class NewButtonForm
    {
        public bool UseUrl { get; set; }    
        public string Url { get; set; }
        public long RelatedInformationForm { get; set; }
        public long CoreObjectID { get; set; }
        public string FullName { get; set; } 
        public string Icon { get; set; } 

        public NewButtonForm()
        { 
            Url = string.Empty;
            FullName = string.Empty;
            Icon = string.Empty;
            RelatedInformationForm = 0;
            CoreObjectID = 0;
            UseUrl=false;
        }
        public NewButtonForm(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(NewButtonForm));
            var Info = serializer.Deserialize(stringReader) as NewButtonForm;
            Url = Info.Url;
            FullName = _CoreObject.FullName;
            CoreObjectID = _CoreObject.CoreObjectID;
            RelatedInformationForm = Info.RelatedInformationForm;
            UseUrl = Info.UseUrl;
            Icon = Info.Icon; 
        }
    }
}