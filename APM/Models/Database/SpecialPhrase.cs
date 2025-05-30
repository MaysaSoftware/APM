using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public class SpecialPhrase
    {
        public string Query;
        public long CoreObjectID;
        public long DataSourceID;
        public SpecialPhrase()
        {
            this.Query = "";
            this.CoreObjectID = 0;
            this.DataSourceID = Referral.MasterDatabaseID;
        }
        public SpecialPhrase(string Query)
        {
            this.Query = Query.Replace("&gt;", "<");
            this.CoreObjectID = 0;
        }
        public SpecialPhrase(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(SpecialPhrase));
            var FormInfo = serializer.Deserialize(stringReader) as SpecialPhrase;
            this.Query = FormInfo.Query.Replace("&gt;", "<");
            this.DataSourceID = FormInfo.DataSourceID;
            this.CoreObjectID = _CoreObject.CoreObjectID;
        }
    }
}