using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Diagram
{
    public class BpmnIncoming
    {
        public string Name { get; set; }
        public BpmnIncoming()
        {

        }
        public BpmnIncoming(string Name)
        {
            if (Name.IndexOf("</bpmn:incoming>") > -1)
                Name = Name.Substring(0, Name.IndexOf("</bpmn:incoming>"));
            this.Name = Name;
        }
    }
}