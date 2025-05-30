using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Diagram
{
    public class BpmnLane
    {
        public long CoreObjectID { get; set; }
        public string CoreObjectFullName { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public List<BpmnFlowNodeRef> BpmnFlowNodeRefList { get; set; } 
        public string[] Personnel { get; set; }
        public string[] OrganizationLevel { get; set; }   
        public string Query { get; set; } 

        public BpmnLane()
        {

        }
        public BpmnLane(string XmlStr)
        {
            ID = String.Empty;
            Name = String.Empty;
            BpmnFlowNodeRefList = new List<BpmnFlowNodeRef>();

            string SettingElement = XmlStr.Substring(0, XmlStr.IndexOf(">"));
            string[] IDArr = SettingElement.Split(new[] { "id=\"" }, StringSplitOptions.None);
            if (IDArr.Length > 1)
            {
                ID = IDArr[1];
                ID = ID.Substring(0, ID.IndexOf("\""));
            }

            string[] NameArr = SettingElement.Split(new[] { "name=\"" }, StringSplitOptions.None);
            if (NameArr.Length > 1)
            {
                Name = NameArr[1];
                Name = Name.Substring(0, Name.IndexOf("\""));
            }

            string[] BpmnFlowNodeRefArr = XmlStr.Split(new[] { "<bpmn:flowNodeRef>" }, StringSplitOptions.None);
            foreach(string strItem in BpmnFlowNodeRefArr)
                if(strItem.IndexOf("</bpmn:flowNodeRef>") >-1)
                    BpmnFlowNodeRefList.Add(new BpmnFlowNodeRef(strItem.Substring(0,strItem.IndexOf("</bpmn:flowNodeRef>"))));

        }

        public BpmnLane(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(BpmnLane));
            var Info = serializer.Deserialize(stringReader) as BpmnLane;
            this.CoreObjectFullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ID = Info.ID;
            this.Name = Info.Name;
            this.BpmnFlowNodeRefList = Info.BpmnFlowNodeRefList;
            this.Personnel = Info.Personnel;
            this.OrganizationLevel = Info.OrganizationLevel;
            this.Query = Info.Query;
        }
    }
}