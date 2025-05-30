using APM.Models.Database;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject
{

    public class TableAttachment
    {
        public string FullName { get; set; }
        public string[] AllowedExtensions { get; set; }
        public CoreDefine.AttachmentUploadType AttachmentUploadType { get; set; }
        public CoreDefine.AttachmentUploadSize AttachmentUploadSize { get; set; }
        public long CoreObjectID { get; set; }
        public long MaxFileSize { get; set; }
        public long MinFileSize { get; set; }
        public bool IsRequired { get; set; }
        public bool ShowDefault { get; set; }
        public bool SaveInDatabase { get; set; }
        public string AutoFillQuery { get; set; }
        public string ColumnWidth { get; set; }
        public TableAttachment()
        {
            this.FullName = "";
            this.AllowedExtensions = new string[] { };
            this.AttachmentUploadType = CoreDefine.AttachmentUploadType.بارگذاری;
            this.AttachmentUploadSize = CoreDefine.AttachmentUploadSize.بزرگ;
            this.CoreObjectID = 0;
            this.MaxFileSize = long.MaxValue;
            this.MinFileSize = long.MinValue;
            this.IsRequired = false;
            this.ShowDefault = false;
            this.SaveInDatabase = false;
            this.AutoFillQuery = "";
            this.ColumnWidth = "2";
        }
        public TableAttachment(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(TableAttachment));
            var TableAttachmentInfo = serializer.Deserialize(stringReader) as TableAttachment;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.AttachmentUploadType = TableAttachmentInfo.AttachmentUploadType;
            this.AllowedExtensions = TableAttachmentInfo.AllowedExtensions;
            this.MaxFileSize = TableAttachmentInfo.MaxFileSize;
            this.MinFileSize = TableAttachmentInfo.MinFileSize;
            this.IsRequired = TableAttachmentInfo.IsRequired;
            this.ShowDefault = TableAttachmentInfo.ShowDefault;
            this.SaveInDatabase = TableAttachmentInfo.SaveInDatabase;
            this.AutoFillQuery = TableAttachmentInfo.AutoFillQuery;
            this.AttachmentUploadSize = TableAttachmentInfo.AttachmentUploadSize;
            this.ColumnWidth = TableAttachmentInfo.ColumnWidth;
        }
         
    }
}