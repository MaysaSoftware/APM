using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Diagram
{
    public class BpmnParticipant
    {
        public long CoreObjectID { get; set; }
        public string CoreObjectFullName { get; set; } 
        public  string ID { get; set; }  
        public  string Name { get; set; }
        public  string ProcessRef { get; set; }

        public BpmnParticipant()
        {

        }

        public BpmnParticipant(string XmlStr)
        { 
            ID=String.Empty;
            Name=String.Empty;
            ProcessRef=String.Empty;

            if(XmlStr !="")
            { 
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

                string[] ProcessRefArr = XmlStr.Split(new[] { "processRef=\"" }, StringSplitOptions.None);
                if (ProcessRefArr.Length > 1)
                {
                    ProcessRef = ProcessRefArr[1];
                    ProcessRef = ProcessRef.Substring(0, ProcessRef.IndexOf("\""));
                }
            }
        }

        public BpmnParticipant(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(BpmnParticipant));
            var Info = serializer.Deserialize(stringReader) as BpmnParticipant;
            this.CoreObjectFullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.ProcessRef = Info.ProcessRef;
        }
    }
}