using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Diagram
{
    public class BpmnTask
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public CoreDefine.ProcessStepActionType processStepActionType { get; set; }
        public List< BpmnIncoming> bpmnIncomingsList { get; set; } 
        public List< BpmnOutgoing> bpmnOutgoingsList { get; set; }


        public BpmnTask()
        {
        }

        public BpmnTask(string XmlStr,string TaskType="")
        {
            ID = String.Empty;
            Name = String.Empty;
            bpmnIncomingsList = new List<BpmnIncoming>() ;
            bpmnOutgoingsList = new List<BpmnOutgoing>() ;
            
            switch (TaskType)
            {
                case "task": processStepActionType = CoreDefine.ProcessStepActionType.عملیات; break;
                case "sendTask": processStepActionType = CoreDefine.ProcessStepActionType.SendTask; break;
                case "receiveTask": processStepActionType = CoreDefine.ProcessStepActionType.ReceiveTask; break;
                case "scriptTask": processStepActionType = CoreDefine.ProcessStepActionType.ScriptTask; break;
                case "userTask": processStepActionType = CoreDefine.ProcessStepActionType.UserTask; break;
                case "serviceTask": processStepActionType = CoreDefine.ProcessStepActionType.ServiceTask; break;
                case "businessRuleTask": processStepActionType = CoreDefine.ProcessStepActionType.BusinessRuleTask; break;
                case "manualTask": processStepActionType = CoreDefine.ProcessStepActionType.ManualTask; break; 
            }

            if (XmlStr.IndexOf("</bpmn:"+ TaskType + ">") > -1)
                XmlStr = XmlStr.Substring(0, XmlStr.IndexOf("</bpmn:"+ TaskType + ">"));

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
            if (XmlStr.IndexOf("<bpmn:incoming") > 0)
            {
                string[] BpmnObjectArr = XmlStr.Split(new[] { "<bpmn:incoming>" }, StringSplitOptions.None);

                foreach (string StrItem in BpmnObjectArr)
                    if (StrItem.IndexOf("bpmn:incoming") > -1) 
                        bpmnIncomingsList.Add(new BpmnIncoming(StrItem));  

            }
            if (XmlStr.IndexOf("<bpmn:outgoing") > 0)
            {
                string[] BpmnObjectArr = XmlStr.Split(new[] { "<bpmn:outgoing>" }, StringSplitOptions.None);

                foreach (string StrItem in BpmnObjectArr)
                    if (StrItem.IndexOf("bpmn:outgoing") > -1)
                        bpmnOutgoingsList.Add(new BpmnOutgoing(StrItem)); 
            }

        }
    }
}