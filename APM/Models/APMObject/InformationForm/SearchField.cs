using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject.InformationForm
{

    public class SearchField
    {
        public long RelatedField { get; set; }
        public string DefaultOperator { get; set; }
        public string DefaultValue { get; set; }
        public bool ShowField { get; set; }
        public bool ShowOperator { get; set; }

        public SearchField()
        {
            DefaultOperator = "مساوی";
            DefaultValue = string.Empty;
            RelatedField = 0;
            ShowField = true;
            ShowOperator = false;
        }
        public SearchField(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(SearchField));
            var FormInfo = serializer.Deserialize(stringReader) as SearchField;
            RelatedField = FormInfo.RelatedField;
            DefaultOperator = FormInfo.DefaultOperator;
            DefaultValue = FormInfo.DefaultValue;
            ShowField = FormInfo.ShowField;
            ShowOperator = FormInfo.ShowOperator;
        }
    }

}