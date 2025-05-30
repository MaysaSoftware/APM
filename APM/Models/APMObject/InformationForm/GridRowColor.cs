using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject.InformationForm
{
    public class GridRowColor
    {
        public long RowColorColumnName { get; set; }
        public string RowColorColumnFullName { get; set; }
        public string RowColorOperator { get; set; }
        public string RowColorColumnValue { get; set; }
        public string RowColorOperator2 { get; set; }
        public string RowColorColumnValue2 { get; set; }
        public string RowColorSelectedColor { get; set; }

        public GridRowColor()
        {
            RowColorColumnName = 0;
            RowColorColumnFullName = string.Empty;
            RowColorOperator =string.Empty;
            RowColorColumnValue = string.Empty; 
            RowColorOperator2 =string.Empty;
            RowColorColumnValue2 = string.Empty; 
            RowColorSelectedColor = string.Empty;
        }

        public GridRowColor(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString(); 
            var stringReader = new System.IO.StringReader(ValueXml); 
            var serializer = new XmlSerializer(typeof(GridRowColor));
            var FormInfo = serializer.Deserialize(stringReader) as GridRowColor; 
            RowColorColumnName = FormInfo.RowColorColumnName;
            RowColorColumnFullName = FormInfo.RowColorColumnFullName;
            RowColorOperator = FormInfo.RowColorOperator;
            RowColorColumnValue = FormInfo.RowColorColumnValue;
            RowColorOperator2 = FormInfo.RowColorOperator2;
            RowColorColumnValue2 = FormInfo.RowColorColumnValue2;
            RowColorSelectedColor = FormInfo.RowColorSelectedColor; 
        }

    }
}