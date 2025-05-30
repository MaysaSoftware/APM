using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Diagram
{
    public class BpmnOutgoing
    {
        public string Name { get; set; }

        public BpmnOutgoing()
        {

        }
        public BpmnOutgoing(string Name)
        {
            if (Name.IndexOf("</bpmn:outgoing>") > -1)
                Name = Name.Substring(0, Name.IndexOf("</bpmn:outgoing>"));
            this.Name = Name;
        }
    }
}