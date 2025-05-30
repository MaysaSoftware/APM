using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Diagram
{
    public class BpmnSequenceFlow
    {
        public long CoreObjectID { get; set; }
        public string CoreObjectFullName { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string SourceRef { get; set; }
        public string TargetRef { get; set; }

        public long SourceRefCoreID { get; set; }   
        public long TargetRefCoreID { get; set; }
        public string ConditionQuery { get; set; }

        public BpmnSequenceFlow()
        {
        }
        public BpmnSequenceFlow(string XmlStr)
        {
            ID = String.Empty;
            Name = String.Empty;
            SourceRef = String.Empty;
            TargetRef = String.Empty; 

            string[] IDArr = XmlStr.Split(new[] { "id=\"" }, StringSplitOptions.None);
            if (IDArr.Length > 1)
            {
                ID = IDArr[1];
                ID = ID.Substring(0, ID.IndexOf("\""));
            }

            string[] NameArr = XmlStr.Split(new[] { "name=\"" }, StringSplitOptions.None);
            if (NameArr.Length > 1)
            {
                Name = NameArr[1];
                Name = Name.Substring(0, Name.IndexOf("\""));
            }

            string[] SourceRefArr = XmlStr.Split(new[] { "sourceRef=\"" }, StringSplitOptions.None);
            if (SourceRefArr.Length > 1)
            {
                SourceRef = SourceRefArr[1];
                SourceRef = SourceRef.Substring(0, SourceRef.IndexOf("\""));
            }
            
            string[] TargetRefArr = XmlStr.Split(new[] { "targetRef=\"" }, StringSplitOptions.None);
            if (TargetRefArr.Length > 1)
            {
                TargetRef = TargetRefArr[1];
                TargetRef = TargetRef.Substring(0, TargetRef.IndexOf("\""));
            } 
        }

        public BpmnSequenceFlow(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(BpmnSequenceFlow));
            var Info = serializer.Deserialize(stringReader) as BpmnSequenceFlow;
            this.CoreObjectFullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.SourceRef = Info.SourceRef;
            this.TargetRef = Info.TargetRef;
            this.SourceRefCoreID = Info.SourceRefCoreID;
            this.TargetRefCoreID = Info.TargetRefCoreID;
            this.ConditionQuery = Info.ConditionQuery;
        }
    }
}