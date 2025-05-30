using APM.Models.Database;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using static APM.Models.Tools.CoreDefine;

namespace APM.Models.APMObject
{
    public class Table
    {
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Comment { get; set; }
        public string TABLESCHEMA { get; set; }
        public long CoreObjectID { get; set; }
        public int ShowRecordCountDefault { get; set; } 
        public string AnalystDescription { get; set; }
        public Table()
        {
            this.ShowRecordCountDefault = 1000;
            this.Comment = "";
            this.TABLESCHEMA = "dbo";
            this.AnalystDescription = string.Empty;
        }
        public Table(string DisplayName, int ShowRecordCountDefault)
        {
            this.DisplayName = DisplayName;
            this.ShowRecordCountDefault = ShowRecordCountDefault;
        }

        public Table(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(Table));
            var TableInfo = serializer.Deserialize(stringReader) as Table;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.DisplayName = TableInfo.DisplayName;
            this.ShowRecordCountDefault = TableInfo.ShowRecordCountDefault;
            this.Comment = TableInfo.Comment;
            this.TABLESCHEMA = TableInfo.TABLESCHEMA;
            this.AnalystDescription = TableInfo.AnalystDescription;
        }

        public Field IDField()
        {
            List<CoreObject> FieldList = CoreObject.FindChilds(CoreObjectID, CoreDefine.Entities.فیلد);
            Field PrimeryKey = new Field();
            foreach (CoreObject FieldObject in FieldList)
            {
                Field Field = new Field(FieldObject);
                if (Field.IsIdentity)
                {
                    PrimeryKey = Field;
                    break;
                }
            }
            return PrimeryKey;
        }
    }

}