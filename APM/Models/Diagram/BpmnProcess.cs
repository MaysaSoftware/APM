using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Diagram
{
    public class BpmnProcess
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<BpmnLane> BpmnLaneList { get; set; }
        public List<BpmnSequenceFlow> BpmnSequenceFlowList { get; set; }
        public List<BpmnTask> BpmnTaskList { get; set; }
        public List<BpmnStartEvent> BpmnStartEventList { get; set; }
        public List<BpmnEndEvent> BpmnEndEventList { get; set; }
        public List<BpmnExclusiveGateway> BpmnExclusiveGatewayList { get; set; }


        public BpmnProcess()
        {
        }
        public BpmnProcess (string XmlStr)
        {
            ID = String.Empty;
            Name = String.Empty;
            BpmnLaneList = new List<BpmnLane>();
            BpmnSequenceFlowList = new List<BpmnSequenceFlow>();
            BpmnTaskList = new List<BpmnTask>();
            BpmnStartEventList = new List<BpmnStartEvent>();
            BpmnEndEventList = new List<BpmnEndEvent>();
            BpmnExclusiveGatewayList = new List<BpmnExclusiveGateway>();

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

            XmlStr = XmlStr.Replace(SettingElement + ">", "");
            string BpmnLaneStr = XmlStr.Substring(XmlStr.IndexOf("<bpmn:laneSet"), XmlStr.IndexOf("</bpmn:laneSet>") - XmlStr.IndexOf("<bpmn:laneSet"));

            if (XmlStr.IndexOf("<bpmn:laneSet") > 0)
            {
                string[] BpmnObjectArr = BpmnLaneStr.Split(new[] { "<bpmn:lane " }, StringSplitOptions.None);

                foreach (string StrItem in BpmnObjectArr)
                    if (StrItem.IndexOf("id=\"Lane_") >-1)
                        BpmnLaneList.Add(new BpmnLane(StrItem));

            }

            XmlStr = XmlStr.Replace(BpmnLaneStr+ "</bpmn:laneSet>", "");

            string[] SearchTask = { "task", "sendTask", "receiveTask", "scriptTask", "userTask", "serviceTask", "businessRuleTask", "manualTask" };
           
            foreach (string TaskItem in SearchTask)
            {
                if (XmlStr.IndexOf("<bpmn:"+ TaskItem + " ") > 0)
                {
                    string[] BpmnObjectArr = XmlStr.Split(new[] { "<bpmn:"+ TaskItem + " " }, StringSplitOptions.None);
                    foreach (string StrItem in BpmnObjectArr)
                        if (StrItem.IndexOf("id=\"Activity_") > -1)
                        {
                            BpmnTaskList.Add(new BpmnTask(StrItem.Substring(0, StrItem.IndexOf("</bpmn:"+ TaskItem + ">")), TaskItem));
                            XmlStr = XmlStr.Replace("<bpmn:"+ TaskItem + " " + StrItem.Substring(0, StrItem.IndexOf("</bpmn:"+ TaskItem + ">")) + "</bpmn:"+ TaskItem + ">", "");
                        }
                } 
            } 

            if (XmlStr.IndexOf("<bpmn:startEvent ") > 0)
            {
                string[] BpmnObjectArr = XmlStr.Split(new[] { "<bpmn:startEvent " }, StringSplitOptions.None);

                foreach (string StrItem in BpmnObjectArr)
                    if (StrItem.IndexOf("id=\"StartEvent_") > -1)
                    {
                        string ReplaceText = "</bpmn:startEvent>";
                        int FindeIndex = StrItem.IndexOf(ReplaceText);
                        if(FindeIndex == -1)
                        {
                            ReplaceText = "/>";
                            FindeIndex = StrItem.IndexOf(ReplaceText);
                        } 
                        BpmnStartEventList.Add(new BpmnStartEvent(StrItem.Substring(0, FindeIndex)));
                        XmlStr = XmlStr.Replace("<bpmn:startEvent " + StrItem.Substring(0, FindeIndex) + ReplaceText, "");
                    } 
            }
            
            if (XmlStr.IndexOf("<bpmn:endEvent ") > 0)
            {
                string[] BpmnObjectArr = XmlStr.Split(new[] { "<bpmn:endEvent " }, StringSplitOptions.None); 
                foreach (string StrItem in BpmnObjectArr)
                    if (StrItem.IndexOf("id=\"Event_") > -1)
                    {
                        string ReplaceText = "</bpmn:endEvent>";
                        int FindeIndex = StrItem.IndexOf(ReplaceText);
                        if (FindeIndex == -1)
                        {
                            ReplaceText = "/>";
                            FindeIndex = StrItem.IndexOf(ReplaceText);
                        }
                        BpmnEndEventList.Add(new BpmnEndEvent(StrItem.Substring(0, FindeIndex)));
                        XmlStr = XmlStr.Replace("<bpmn:endEvent " + StrItem.Substring(0, FindeIndex) + ReplaceText, ""); 
                    } 
            }
            
            if (XmlStr.IndexOf("<bpmn:exclusiveGateway ") > 0)
            {
                string[] BpmnObjectArr = XmlStr.Split(new[] { "<bpmn:exclusiveGateway " }, StringSplitOptions.None); 
                foreach (string StrItem in BpmnObjectArr)
                    if (StrItem.IndexOf("id=\"Gateway_") > -1)
                    {
                        BpmnExclusiveGatewayList.Add(new BpmnExclusiveGateway(StrItem.Substring(0,StrItem.IndexOf("</bpmn:exclusiveGateway>"))));
                        XmlStr = XmlStr.Replace("<bpmn:exclusiveGateway " + StrItem.Substring(0, StrItem.IndexOf("</bpmn:exclusiveGateway>"))+ "</bpmn:exclusiveGateway>", "");
                    } 
            }

            if (XmlStr.IndexOf("<bpmn:sequenceFlow ") > 0)
            {
                string[] BpmnSequenceFlowArr = XmlStr.Split(new[] { "<bpmn:sequenceFlow " }, StringSplitOptions.None);

                foreach (string StrItem in BpmnSequenceFlowArr)
                    if (StrItem.IndexOf("id=\"Flow_") > -1)
                    {
                        BpmnSequenceFlowList.Add(new BpmnSequenceFlow(StrItem));
                        XmlStr = XmlStr.Replace("<bpmn:sequenceFlow " + StrItem, "");
                    }

            }
        }
    }
}