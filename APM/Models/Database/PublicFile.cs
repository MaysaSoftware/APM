using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public class PublicFile
    {
        public string FullName { get; set; } 
        public long CoreObjectID { get; set; } 
        public PublicFile()
        { 
            CoreObjectID = 0;
            FullName = string.Empty; 
        }
        public PublicFile(string FullName, string Title, long FileSize)
        {
            this.FullName = FullName; 
        }

        public PublicFile(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(PublicFile));
            var TableInfo = serializer.Deserialize(stringReader) as PublicFile;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID; 
        }

    }
}