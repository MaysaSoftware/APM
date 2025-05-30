using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Diagram
{
    public static class BPMN
    {
        public const string InitiXML = @"<?xml version=""1.0"" encoding=""UTF-8""?>" +
        @"<bpmn:definitions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" " +
        @"xmlns:bpmn=""http://www.omg.org/spec/BPMN/20100524/MODEL"" " +
        @"xmlns:bpmndi=""http://www.omg.org/spec/BPMN/20100524/DI"" " +
        @"xmlns:dc=""http://www.omg.org/spec/DD/20100524/DC"" " +
        @"targetNamespace=""http://bpmn.io/schema/bpmn"" " +
        @"id=""Definitions_1"">" +
        @"<bpmn:process id=""Process_1"" isExecutable=""false"">" +
        @"<bpmn:startEvent id=""StartEvent_1""/>" +
        "</bpmn:process>" +
        @"<bpmndi:BPMNDiagram id=""BPMNDiagram_1"">" +
        @"<bpmndi:BPMNPlane id=""BPMNPlane_1"" bpmnElement=""Process_1"">" +
        @"<bpmndi:BPMNShape id=""_BPMNShape_StartEvent_2"" bpmnElement=""StartEvent_1"">" +
        @"<dc:Bounds height=""36.0"" width=""36.0"" x=""173.0"" y=""102.0""/>" +
        "</bpmndi:BPMNShape>" +
        "</bpmndi:BPMNPlane>" +
        "</bpmndi:BPMNDiagram>" +
        "</bpmn:definitions>";

//        "<?xml version="1.0" encoding="UTF-8"?>
//<bpmn:definitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:bpmn="http://www.omg.org/spec/BPMN/20100524/MODEL" xmlns:bpmndi="http://www.omg.org/spec/BPMN/20100524/DI" xmlns:dc="http://www.omg.org/spec/DD/20100524/DC" id="Definitions_1" targetNamespace="http://bpmn.io/schema/bpmn">
//  <bpmn:collaboration id = "Collaboration_1512g8t" >
//    < bpmn:participant id = "Participant_1et5noe" name="درخواست خرید" processRef="Process_1" />
//  </bpmn:collaboration>
//  <bpmn:process id = "Process_1" isExecutable="false">
//    <bpmn:startEvent id = "StartEvent_1" />
//  </ bpmn:process>
//  <bpmndi:BPMNDiagram id = "BPMNDiagram_1" >
//    < bpmndi:BPMNPlane id = "BPMNPlane_1" bpmnElement="Collaboration_1512g8t">
//      <bpmndi:BPMNShape id = "Participant_1et5noe_di" bpmnElement="Participant_1et5noe" isHorizontal="true">
//        <dc:Bounds x = "272" y="118" width="600" height="250" />
//        <bpmndi:BPMNLabel />
//      </bpmndi:BPMNShape>
//      <bpmndi:BPMNShape id = "_BPMNShape_StartEvent_2" bpmnElement="StartEvent_1">
//        <dc:Bounds x = "342" y="212" width="36" height="36" />
//      </bpmndi:BPMNShape>
//    </bpmndi:BPMNPlane>
//  </bpmndi:BPMNDiagram>
//</bpmn:definitions>
//"
    }
}