using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Diagram
{
    public class BpmnFlowNodeRef
    {
        public string Name { get; set; }
        public BpmnFlowNodeRef()
        {

        }
        public BpmnFlowNodeRef(string Name)
        {   
               this.Name = Name; 
        }
    }
}