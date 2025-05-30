using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.APMObject
{
    public class Folder
    {
        public string FullName { get; set; }
        public long CoreObjectID { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public bool IsExpand { get; set; }
        public int IconSize { get; set; }
        public Folder()
        {
            this.FullName = String.Empty;             
            this.CoreObjectID = 0;
            this.Icon = "k-icon k-i-folder";
            this.IsExpand =false;
            this.IconColor = Referral.PublicSetting.IconColor;
            this.IconSize = 12;
        }
        public Folder(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(Folder));
            var Info = serializer.Deserialize(stringReader) as Folder;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.Icon = Info.Icon; 
            this.IconColor = Info.IconColor; 
            this.IsExpand = Info.IsExpand; 
            this.IconSize = Info.IconSize; 
        }
    }
}