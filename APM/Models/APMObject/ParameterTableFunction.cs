using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject
{

    public class ParameterTableFunction
    {
        public string FullName { get; set; }
        public string ParameterDataType { get; set; }
        public long CoreObjectID { get; set; }
        public ParameterTableFunction()
        {
            this.ParameterDataType = "Bigint";
        }

        public ParameterTableFunction(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ParameterTableFunction));
            var TableInfo = serializer.Deserialize(stringReader) as ParameterTableFunction;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ParameterDataType = TableInfo.ParameterDataType;
        }
    }
}