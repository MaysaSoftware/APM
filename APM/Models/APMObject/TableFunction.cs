using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject
{

    public class TableFunction
    {
        public string FullName { get; set; }
        public string ReturnDataType { get; set; }
        public string Query { get; set; }
        public long CoreObjectID { get; set; }
        public TableFunction()
        {
            this.ReturnDataType = "Bigint";
            this.Query = "";
        }

        public TableFunction(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(TableFunction));
            var TableInfo = serializer.Deserialize(stringReader) as TableFunction;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ReturnDataType = TableInfo.ReturnDataType;
            this.Query = TableInfo.Query;
        }
    }
}