using APM.Models;
using APM.Models.Database;
using APM.Models.Diagram;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers.Diagram
{
    public class BPMNController : Controller
    {
        // GET: BPMN
        private static DataTank.SysSetting SysSetting = new DataTank.SysSetting();
        public ActionResult Index(long ProcessID)
        {
            ProcessType process = new ProcessType(CoreObject.Find(ProcessID));
            //ProcessType process = new ProcessType(CoreObject.Find(10373));
            ViewData["ProcessModelXml"] = (process.ProcessModelXml == "" ? BPMN.InitiXML : process.ProcessModelXml);
            ViewData["ProcessType"] = process;
            return View();
        }

        public JsonResult Save(string BpmnXml,long ProcessID)
        {
            BpmnXml = BpmnXml.Replace("*$>*", ">").Replace("*<*", "<");

            string[] participantArr = BpmnXml.Substring(BpmnXml.IndexOf("<bpmn:participant"), BpmnXml.IndexOf("</bpmn:collaboration>")- BpmnXml.IndexOf("<bpmn:participant")).Split(new[] { "<bpmn:participant" }, StringSplitOptions.None);
            string[] processArr = BpmnXml.Substring(BpmnXml.IndexOf("</bpmn:collaboration>"), BpmnXml.Length- BpmnXml.IndexOf("</bpmn:collaboration>")).Split(new[] { "<bpmn:process" }, StringSplitOptions.None);
            List<BpmnParticipant> BpmnParticipantList=new List<BpmnParticipant>();
            List<BpmnProcess> BpmnProcessList = new List<BpmnProcess>();

            foreach (string StrItem in participantArr) 
                if(StrItem!="")
                BpmnParticipantList.Add(new BpmnParticipant(StrItem));  

            foreach (string StrItem in processArr)
                if(StrItem.IndexOf("id=\"Process")>-1)
                BpmnProcessList.Add(new BpmnProcess(StrItem));

            List<CoreObject> BpmnParticipantCoreObjectList = CoreObject.FindChilds(ProcessID, CoreDefine.Entities.BPMN_بخش_بندی);
            int OrderIndex = BpmnParticipantCoreObjectList.Count;
            string CoreValue = "";

            foreach (CoreObject BpmnParticipantCore in BpmnParticipantCoreObjectList)
            { 
                BpmnParticipant bpmnParticipantItem = new BpmnParticipant(BpmnParticipantCore);
                int FindIndex = BpmnParticipantList.FindIndex(x => x.ID == bpmnParticipantItem.ID);
                int CoreIndex = 0;
                if (FindIndex>-1)
                { 
                    if (bpmnParticipantItem.Name != BpmnParticipantList[FindIndex].Name)
                    {
                        bpmnParticipantItem.Name = BpmnParticipantList[FindIndex].Name; 
                        CoreValue = Tools.ToXML(bpmnParticipantItem);
                        Referral.DBCore.UpdateRow(BpmnParticipantCore.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { CoreValue });
                        CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == BpmnParticipantCore.CoreObjectID);
                        Referral.CoreObjects[CoreIndex].Value = CoreValue;
                    } 
                    BpmnParticipantList.RemoveAt(BpmnParticipantList.FindIndex(x => x.ID == bpmnParticipantItem.ID));

                    BpmnProcess bpmnProcess = BpmnProcessList.Find(x => x.ID == bpmnParticipantItem.ProcessRef); 
                    List<CoreObject> BpmnLaneCoreObjectList = CoreObject.FindChilds(BpmnParticipantCore.CoreObjectID, CoreDefine.Entities.BPMN_مسیر);

                    foreach (CoreObject BpmnLaneCore in BpmnLaneCoreObjectList)
                    {
                        BpmnLane BpmnLaneItem = new BpmnLane(BpmnLaneCore);
                        FindIndex = bpmnProcess.BpmnLaneList.FindIndex(x => x.ID == BpmnLaneItem.ID);
                        if (FindIndex > -1)
                        {
                            bool IsNotEqual = false;
                            foreach(BpmnFlowNodeRef FlowNodeRef in bpmnProcess.BpmnLaneList[FindIndex].BpmnFlowNodeRefList)
                            {
                                if(BpmnLaneItem.BpmnFlowNodeRefList.FindIndex(x=>x.Name==FlowNodeRef.Name)==-1)
                                    IsNotEqual = true;
                            }
                            if (BpmnLaneItem.Name != bpmnProcess.BpmnLaneList[FindIndex].Name || IsNotEqual || BpmnLaneItem.Name != BpmnLaneCore.FullName)
                            {
                                BpmnLaneItem.Name = bpmnProcess.BpmnLaneList[FindIndex].Name;
                                BpmnLaneItem.BpmnFlowNodeRefList = bpmnProcess.BpmnLaneList[FindIndex].BpmnFlowNodeRefList;
                                CoreValue = Tools.ToXML(BpmnLaneItem);
                                Referral.DBCore.UpdateRow(BpmnLaneCore.CoreObjectID, 0, "CoreObject", new string[] { "FullName", "Value" }, new object[] { BpmnLaneItem.Name, CoreValue });
                                CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == BpmnLaneCore.CoreObjectID);
                                Referral.CoreObjects[CoreIndex].Value = CoreValue;
                                Referral.CoreObjects[CoreIndex].FullName = BpmnLaneItem.Name;
                                IsNotEqual = false;
                            } 
                            bpmnProcess.BpmnLaneList.Remove(bpmnProcess.BpmnLaneList[FindIndex]);

                            foreach (BpmnFlowNodeRef bpmnFlowNodeRefItem in BpmnLaneItem.BpmnFlowNodeRefList)
                            {
                                List<BpmnIncoming> BpmnIncomingList = new List<BpmnIncoming>();
                                List<BpmnOutgoing> BpmnOutgoingList = new List<BpmnOutgoing>();
                                List<BpmnSequenceFlow> BpmnIncomingListCore = new List<BpmnSequenceFlow>();
                                List<BpmnSequenceFlow> BpmnOutgoingListCore = new List<BpmnSequenceFlow>();
                                ProcessStep processStep = new ProcessStep();
                                CoreDefine.ProcessStepActionType processStepActionType = new CoreDefine.ProcessStepActionType();
                                string ProcessName=string.Empty;
                                CoreObject processStepCore = CoreObject.Find(BpmnLaneCore.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند, Tools.SafeTitle(bpmnFlowNodeRefItem.Name));
                                if (processStepCore.CoreObjectID > 0)
                                {
                                    processStep = new ProcessStep(processStepCore); 
                                } 

                                bool IsFind = false;


                                foreach (BpmnTask bpmnTaskItem in bpmnProcess.BpmnTaskList)
                                {
                                    if (bpmnFlowNodeRefItem.Name == bpmnTaskItem.ID)
                                    {
                                        processStepActionType =bpmnTaskItem.processStepActionType;
                                        BpmnIncomingList = bpmnTaskItem.bpmnIncomingsList;
                                        BpmnOutgoingList = bpmnTaskItem.bpmnOutgoingsList;
                                        processStep.ID = bpmnTaskItem.ID;
                                        ProcessName = bpmnTaskItem.Name;
                                        IsFind = true;
                                        break;
                                    }
                                }

                                if (!IsFind)
                                    foreach (BpmnStartEvent bpmnStartEventItem in bpmnProcess.BpmnStartEventList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnStartEventItem.ID)
                                        {
                                            processStepActionType = CoreDefine.ProcessStepActionType.شروع;
                                            BpmnIncomingList = bpmnStartEventItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnStartEventItem.bpmnOutcomingsList;
                                            processStep.ID = bpmnStartEventItem.ID;
                                            ProcessName = bpmnStartEventItem.Name;
                                            IsFind = true;
                                            break;
                                        }
                                    }

                                if (!IsFind)
                                    foreach (BpmnEndEvent bpmnEndEventItem in bpmnProcess.BpmnEndEventList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnEndEventItem.ID)
                                        {
                                            processStepActionType = CoreDefine.ProcessStepActionType.پایان;
                                            BpmnIncomingList = bpmnEndEventItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnEndEventItem.bpmnOutcomingsList;
                                            processStep.ID = bpmnEndEventItem.ID;
                                            ProcessName = bpmnEndEventItem.Name;
                                            IsFind = true;
                                            break;
                                        }
                                    }


                                if (!IsFind)
                                    foreach (BpmnExclusiveGateway bpmnExclusiveGatewayItem in bpmnProcess.BpmnExclusiveGatewayList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnExclusiveGatewayItem.ID)
                                        {
                                            processStepActionType = CoreDefine.ProcessStepActionType.شرط;
                                            BpmnIncomingList = bpmnExclusiveGatewayItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnExclusiveGatewayItem.bpmnOutgoingList;
                                            processStep.ID = bpmnExclusiveGatewayItem.ID;
                                            ProcessName = bpmnExclusiveGatewayItem.Name;
                                            IsFind = true;
                                            break;
                                        }
                                    }

                                if(processStep.CoreObjectID>0)
                                {
                                    if(processStep.ActionType!= processStepActionType || ProcessName != processStep.Name)
                                    {
                                        processStep.ActionType = processStepActionType;
                                        processStep.Name = ProcessName;
                                        CoreValue = Tools.ToXML(processStep);
                                        Referral.DBCore.UpdateRow(processStep.CoreObjectID,0, "CoreObject", new string[] { "Value" }, new object[] { CoreValue });
                                        CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == processStep.CoreObjectID);
                                        Referral.CoreObjects[CoreIndex].Value = CoreValue;
                                    }

                                    foreach(CoreObject BpmnSequenceFlowCore in CoreObject.FindChilds(processStep.CoreObjectID, CoreDefine.Entities.ورودی_فرآیند))
                                        BpmnIncomingListCore.Add(new BpmnSequenceFlow(BpmnSequenceFlowCore));

                                    foreach(CoreObject BpmnSequenceFlowCore in CoreObject.FindChilds(processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند))
                                        BpmnOutgoingListCore.Add(new BpmnSequenceFlow(BpmnSequenceFlowCore));

                                    foreach (BpmnIncoming bpmnIncomingItem in BpmnIncomingList)
                                    {
                                        int FindIndexBpmnIncoming = BpmnIncomingListCore.FindIndex(x => x.ID == bpmnIncomingItem.Name);
                                        if(FindIndexBpmnIncoming > -1)
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlowCore = BpmnIncomingListCore[FindIndexBpmnIncoming];
                                            BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnIncomingItem.Name);

                                            if(bpmnSequenceFlowCore.SourceRef!= bpmnSequenceFlow.SourceRef ||
                                                bpmnSequenceFlowCore.TargetRef != bpmnSequenceFlow.TargetRef ||
                                                bpmnSequenceFlowCore.Name != bpmnSequenceFlow.Name)
                                            {
                                                bpmnSequenceFlowCore.SourceRef = bpmnSequenceFlow.SourceRef;
                                                bpmnSequenceFlowCore.TargetRef = bpmnSequenceFlow.TargetRef;
                                                bpmnSequenceFlowCore.Name = bpmnSequenceFlow.Name;
                                                CoreValue = Tools.ToXML(bpmnSequenceFlowCore);
                                                Referral.DBCore.UpdateRow(bpmnSequenceFlowCore.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { CoreValue });
                                                CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == bpmnSequenceFlowCore.CoreObjectID);
                                                Referral.CoreObjects[CoreIndex].Value = CoreValue;
                                            } 

                                            BpmnIncomingListCore.RemoveAt(FindIndexBpmnIncoming);
                                        }
                                        else
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnIncomingItem.Name);

                                            CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                            long bpmnIncomingID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { processStep.CoreObjectID, CoreDefine.Entities.ورودی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                            if (bpmnIncomingID > 0)
                                                Referral.CoreObjects.Add(new CoreObject(bpmnIncomingID, processStep.CoreObjectID, CoreDefine.Entities.ورودی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));
                                        } 
                                    }

                                    foreach (BpmnSequenceFlow bpmnSequenceFlowItem in BpmnIncomingListCore)
                                    { 
                                        Referral.DBCore.Delete("CoreObject", bpmnSequenceFlowItem.CoreObjectID);
                                        Referral.CoreObjects.RemoveAll(x=>x.CoreObjectID == bpmnSequenceFlowItem.CoreObjectID);
                                    }

                                    foreach (BpmnOutgoing bpmnOutgoingItem in BpmnOutgoingList)
                                    {
                                        int FindIndexBpmnIncoming = BpmnOutgoingListCore.FindIndex(x => x.ID == bpmnOutgoingItem.Name);
                                        if (FindIndexBpmnIncoming > -1)
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlowCore = BpmnOutgoingListCore[FindIndexBpmnIncoming];
                                            BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnOutgoingItem.Name);

                                            if (bpmnSequenceFlowCore.SourceRef != bpmnSequenceFlow.SourceRef ||
                                                bpmnSequenceFlowCore.TargetRef != bpmnSequenceFlow.TargetRef ||
                                                bpmnSequenceFlowCore.Name != bpmnSequenceFlow.Name)
                                            {
                                                bpmnSequenceFlowCore.SourceRef = bpmnSequenceFlow.SourceRef;
                                                bpmnSequenceFlowCore.TargetRef = bpmnSequenceFlow.TargetRef;
                                                bpmnSequenceFlowCore.Name = bpmnSequenceFlow.Name;
                                                CoreValue = Tools.ToXML(bpmnSequenceFlowCore);
                                                Referral.DBCore.UpdateRow(bpmnSequenceFlowCore.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { CoreValue });
                                                CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == bpmnSequenceFlowCore.CoreObjectID);
                                                Referral.CoreObjects[CoreIndex].Value = CoreValue;
                                            }

                                            BpmnOutgoingListCore.RemoveAt(FindIndexBpmnIncoming);
                                        }
                                        else
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnOutgoingItem.Name);

                                            CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                            long bpmnIncomingID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                            if (bpmnIncomingID > 0)
                                                Referral.CoreObjects.Add(new CoreObject(bpmnIncomingID, processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));
                                        }
                                    }

                                    foreach (BpmnSequenceFlow bpmnSequenceFlowItem in BpmnOutgoingListCore)
                                    {
                                        Referral.DBCore.Delete("CoreObject", bpmnSequenceFlowItem.CoreObjectID);
                                        Referral.CoreObjects.RemoveAll(x => x.CoreObjectID == bpmnSequenceFlowItem.CoreObjectID);
                                    }

                                }
                                else
                                {
                                    processStep.ActionType = processStepActionType;
                                    CoreValue = Tools.ToXML(processStep);
                                    long ProcessStepID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { BpmnLaneItem.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند.ToString(), Tools.SafeTitle(bpmnFlowNodeRefItem.Name), 0, 0, CoreValue });
                                    if (ProcessStepID > 0)
                                    {
                                        Referral.CoreObjects.Add(new CoreObject(ProcessStepID, BpmnLaneItem.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند, "", Tools.SafeTitle(bpmnFlowNodeRefItem.Name), 0, false, CoreValue));
                                        foreach (BpmnIncoming bpmnIncomingItem in BpmnIncomingList)
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnIncomingItem.Name);
                                            CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                            long bpmnIncomingID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { ProcessStepID, CoreDefine.Entities.ورودی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                            if (bpmnIncomingID > 0)
                                                Referral.CoreObjects.Add(new CoreObject(bpmnIncomingID, ProcessStepID, CoreDefine.Entities.ورودی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));

                                        }
                                        foreach (BpmnOutgoing bpmnOutgoingItem in BpmnOutgoingList)
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnOutgoingItem.Name);
                                            CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                            long bpmnID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { ProcessStepID, CoreDefine.Entities.خروجی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                            if (bpmnID > 0)
                                                Referral.CoreObjects.Add(new CoreObject(bpmnID, ProcessStepID, CoreDefine.Entities.خروجی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));

                                        }

                                    }

                                } 

                            } 

                        }
                        else
                        {
                            Referral.DBCore.Delete("CoreObject", BpmnLaneCore.CoreObjectID);
                            Referral.CoreObjects.Remove(BpmnLaneCore);
                        }
                    }

                    foreach(BpmnLane bpmnLaneItem in bpmnProcess.BpmnLaneList)
                    {
                        CoreValue = Tools.ToXML(bpmnLaneItem);
                        long BpmnLaneID = Referral.DBCore.Insert("CoreObject"
                                    , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                    , new object[] { BpmnParticipantCore.CoreObjectID, CoreDefine.Entities.BPMN_مسیر.ToString(), Tools.SafeTitle(bpmnLaneItem.Name), 0, 0, CoreValue });

                        if (BpmnLaneID > 0)
                        {
                            Referral.CoreObjects.Add(new CoreObject(BpmnLaneID, BpmnParticipantCore.CoreObjectID, CoreDefine.Entities.BPMN_مسیر, "", Tools.SafeTitle(bpmnLaneItem.Name), 0, false, CoreValue));
                            foreach(BpmnFlowNodeRef bpmnFlowNodeRefItem in bpmnLaneItem.BpmnFlowNodeRefList)
                            {
                                ProcessStep processStep = new ProcessStep();
                                bool IsFind=false; 

                                List<BpmnIncoming> BpmnIncomingList = new List<BpmnIncoming>();
                                List<BpmnOutgoing> BpmnOutgoingList = new List<BpmnOutgoing>(); 

                                foreach (BpmnTask bpmnTaskItem in bpmnProcess.BpmnTaskList)
                                {
                                    if(bpmnFlowNodeRefItem.Name==bpmnTaskItem.ID)
                                    {
                                        processStep.ActionType = CoreDefine.ProcessStepActionType.عملیات;
                                        BpmnIncomingList = bpmnTaskItem.bpmnIncomingsList;
                                        BpmnOutgoingList = bpmnTaskItem.bpmnOutgoingsList; 
                                        IsFind =true;
                                        break;
                                    }
                                }

                                if (!IsFind)
                                    foreach (BpmnStartEvent bpmnStartEventItem in bpmnProcess.BpmnStartEventList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnStartEventItem.ID)
                                        {
                                            processStep.ActionType = CoreDefine.ProcessStepActionType.شروع;
                                            BpmnIncomingList = bpmnStartEventItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnStartEventItem.bpmnOutcomingsList;
                                            IsFind = true;
                                            break;
                                        }
                                    }

                                if (!IsFind)
                                    foreach (BpmnEndEvent bpmnEndEventItem in bpmnProcess.BpmnEndEventList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnEndEventItem.ID)
                                        {
                                            processStep.ActionType = CoreDefine.ProcessStepActionType.پایان;
                                            BpmnIncomingList = bpmnEndEventItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnEndEventItem.bpmnOutcomingsList;
                                            IsFind = true;
                                            break;
                                        }
                                    }
                                    

                                if (!IsFind)
                                    foreach (BpmnExclusiveGateway bpmnExclusiveGatewayItem in bpmnProcess.BpmnExclusiveGatewayList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnExclusiveGatewayItem.ID)
                                        {
                                            processStep.ActionType = CoreDefine.ProcessStepActionType.شرط;
                                            BpmnIncomingList = bpmnExclusiveGatewayItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnExclusiveGatewayItem.bpmnOutgoingList;
                                            IsFind = true;
                                            break;
                                        }
                                    }


                                CoreValue = Tools.ToXML(processStep);
                                long ProcessStepID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { BpmnLaneID, CoreDefine.Entities.مرحله_فرآیند.ToString(), Tools.SafeTitle(bpmnFlowNodeRefItem.Name), 0, 0, CoreValue });
                                if(ProcessStepID>0)
                                { 
                                    Referral.CoreObjects.Add(new CoreObject(ProcessStepID, BpmnLaneID, CoreDefine.Entities.مرحله_فرآیند, "", Tools.SafeTitle(bpmnFlowNodeRefItem.Name), 0, false, CoreValue));
                                    foreach(BpmnIncoming bpmnIncomingItem in BpmnIncomingList)
                                    {
                                        BpmnSequenceFlow bpmnSequenceFlow= bpmnProcess.BpmnSequenceFlowList.Find(x=>x.ID==bpmnIncomingItem.Name);
                                        CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                        long bpmnIncomingID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { ProcessStepID, CoreDefine.Entities.ورودی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                        if(bpmnIncomingID>0)
                                        Referral.CoreObjects.Add(new CoreObject(bpmnIncomingID, ProcessStepID, CoreDefine.Entities.ورودی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));

                                    }
                                    foreach(BpmnOutgoing bpmnOutgoingItem in BpmnOutgoingList)
                                    {
                                        BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnOutgoingItem.Name);
                                        CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                        long bpmnID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { ProcessStepID, CoreDefine.Entities.خروجی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                        if (bpmnID > 0)
                                            Referral.CoreObjects.Add(new CoreObject(bpmnID, ProcessStepID, CoreDefine.Entities.خروجی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));

                                    }

                                }

                            }
                        }
                    } 
                }
                else
                {
                    Referral.DBCore.Delete("CoreObject", BpmnParticipantCore.CoreObjectID);
                    Referral.CoreObjects.Remove(BpmnParticipantCore);
                }
            }

            foreach(BpmnParticipant bpmnParticipantItem in BpmnParticipantList)
            {
                CoreValue = Tools.ToXML(bpmnParticipantItem);
                long BpmnParticipantID = Referral.DBCore.Insert("CoreObject"
                          , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                          , new object[] { ProcessID, CoreDefine.Entities.BPMN_بخش_بندی.ToString(), Tools.SafeTitle(bpmnParticipantItem.Name), OrderIndex++, 0, CoreValue });

                if(BpmnParticipantID>0)
                {
                    Referral.CoreObjects.Add(new CoreObject(BpmnParticipantID, ProcessID, CoreDefine.Entities.BPMN_بخش_بندی, "", Tools.SafeTitle(bpmnParticipantItem.Name), OrderIndex, false, CoreValue));

                    BpmnProcess bpmnProcess = BpmnProcessList.Find(x => x.ID == bpmnParticipantItem.ProcessRef);  
                    foreach (BpmnLane bpmnLaneItem in bpmnProcess.BpmnLaneList)
                    {
                        CoreValue = Tools.ToXML(bpmnLaneItem);
                        long BpmnLaneID = Referral.DBCore.Insert("CoreObject"
                                    , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                    , new object[] { BpmnParticipantID, CoreDefine.Entities.BPMN_مسیر.ToString(), Tools.SafeTitle(bpmnLaneItem.Name), 0, 0, CoreValue });

                        if (BpmnLaneID > 0)
                        {
                            Referral.CoreObjects.Add(new CoreObject(BpmnLaneID, BpmnParticipantID, CoreDefine.Entities.BPMN_مسیر, "", Tools.SafeTitle(bpmnLaneItem.Name), 0, false, CoreValue));
                            foreach (BpmnFlowNodeRef bpmnFlowNodeRefItem in bpmnLaneItem.BpmnFlowNodeRefList)
                            {
                                ProcessStep processStep = new ProcessStep(); 
                                bool IsFind = false;

                                List<BpmnIncoming> BpmnIncomingList = new List<BpmnIncoming>();
                                List<BpmnOutgoing> BpmnOutgoingList = new List<BpmnOutgoing>();

                                foreach (BpmnTask bpmnTaskItem in bpmnProcess.BpmnTaskList)
                                {
                                    if (bpmnFlowNodeRefItem.Name == bpmnTaskItem.ID)
                                    {
                                        processStep.ActionType = bpmnTaskItem.processStepActionType;
                                        BpmnIncomingList = bpmnTaskItem.bpmnIncomingsList;
                                        BpmnOutgoingList = bpmnTaskItem.bpmnOutgoingsList;
                                        processStep.ID = bpmnTaskItem.ID;
                                        processStep.Name = bpmnTaskItem.Name;
                                        IsFind = true;
                                        break;
                                    }
                                }

                                if (!IsFind)
                                    foreach (BpmnStartEvent bpmnStartEventItem in bpmnProcess.BpmnStartEventList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnStartEventItem.ID)
                                        {
                                            processStep.ActionType = CoreDefine.ProcessStepActionType.شروع;
                                            BpmnIncomingList = bpmnStartEventItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnStartEventItem.bpmnOutcomingsList;
                                            processStep.ID = bpmnStartEventItem.ID;
                                            processStep.Name = bpmnStartEventItem.Name;
                                            IsFind = true;
                                            break;
                                        }
                                    }

                                if (!IsFind)
                                    foreach (BpmnEndEvent bpmnEndEventItem in bpmnProcess.BpmnEndEventList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnEndEventItem.ID)
                                        {
                                            processStep.ActionType = CoreDefine.ProcessStepActionType.پایان;
                                            BpmnIncomingList = bpmnEndEventItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnEndEventItem.bpmnOutcomingsList;
                                            processStep.ID = bpmnEndEventItem.ID;
                                            processStep.Name = bpmnEndEventItem.Name;
                                            IsFind = true;
                                            break;
                                        }
                                    }


                                if (!IsFind)
                                    foreach (BpmnExclusiveGateway bpmnExclusiveGatewayItem in bpmnProcess.BpmnExclusiveGatewayList)
                                    {
                                        if (bpmnFlowNodeRefItem.Name == bpmnExclusiveGatewayItem.ID)
                                        {
                                            processStep.ActionType = CoreDefine.ProcessStepActionType.شرط;
                                            BpmnIncomingList = bpmnExclusiveGatewayItem.bpmnIncomingsList;
                                            BpmnOutgoingList = bpmnExclusiveGatewayItem.bpmnOutgoingList;
                                            processStep.ID = bpmnExclusiveGatewayItem.ID;
                                            processStep.Name = bpmnExclusiveGatewayItem.Name;
                                            IsFind = true;
                                            break;
                                        }
                                    }


                                CoreValue = Tools.ToXML(processStep);
                                long ProcessStepID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { BpmnLaneID, CoreDefine.Entities.مرحله_فرآیند.ToString(), Tools.SafeTitle(bpmnFlowNodeRefItem.Name), 0, 0, CoreValue });
                                if (ProcessStepID > 0)
                                {
                                    Referral.CoreObjects.Add(new CoreObject(ProcessStepID, BpmnLaneID, CoreDefine.Entities.مرحله_فرآیند, "", Tools.SafeTitle(bpmnFlowNodeRefItem.Name), 0, false, CoreValue));
                                    foreach (BpmnIncoming bpmnIncomingItem in BpmnIncomingList)
                                    {
                                        BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnIncomingItem.Name);
                                        CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                        long bpmnIncomingID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { ProcessStepID, CoreDefine.Entities.ورودی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                        if (bpmnIncomingID > 0)
                                            Referral.CoreObjects.Add(new CoreObject(bpmnIncomingID, ProcessStepID, CoreDefine.Entities.ورودی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));

                                    }
                                    foreach (BpmnOutgoing bpmnOutgoingItem in BpmnOutgoingList)
                                    {
                                        BpmnSequenceFlow bpmnSequenceFlow = bpmnProcess.BpmnSequenceFlowList.Find(x => x.ID == bpmnOutgoingItem.Name);
                                        CoreValue = Tools.ToXML(bpmnSequenceFlow);
                                        long bpmnID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { ProcessStepID, CoreDefine.Entities.خروجی_فرآیند.ToString(), Tools.SafeTitle(bpmnSequenceFlow.ID), 0, 0, CoreValue });
                                        if (bpmnID > 0)
                                            Referral.CoreObjects.Add(new CoreObject(bpmnID, ProcessStepID, CoreDefine.Entities.خروجی_فرآیند, "", Tools.SafeTitle(bpmnSequenceFlow.ID), 0, false, CoreValue));

                                    }

                                }

                            }
                        }
                    }
                }

            }

            ProcessType process = new ProcessType(CoreObject.Find(ProcessID));
            process.ProcessModelXml = BpmnXml;
            string[] VersionArr = process.Version.Split('.');
            if (int.Parse(VersionArr[1]) == 100)
            {
                VersionArr[0] = (int.Parse(VersionArr[0]) + 1).ToString();
                VersionArr[1] = "0";
            }
            else
                VersionArr[1] = (int.Parse(VersionArr[1]) + 1).ToString();

            process.Version = VersionArr[0]+"."+ VersionArr[1];

            string Value = Tools.ToXML(process);

            if (Referral.DBCore.UpdateRow(ProcessID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value }))
            {
                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == ProcessID);
                Referral.CoreObjects[CoreIndex].Value = Value;
            }
            return Json(new{Version= process.Version });
        }

        public ActionResult ShowDetail(long ProcessID,string BpmnType,string BpmnID, string BpmnName)
        {
            //ProcessID = 10373;
            //BpmnType = "bpmn:Lane";
            //BpmnID = "Lane_0exmgtb";

            List<CoreObject> CoreObjectsList = CoreObject.FindChilds(ProcessID);
            CoreObject ResultCoreObject=new CoreObject();
            CoreDefine.Entities ChildEntities = new CoreDefine.Entities();

            switch (BpmnType)
            {
                case "bpmn:Lane":
                    {
                        foreach (CoreObject coreObject in CoreObjectsList)
                        {
                            List<CoreObject> LaneCoreList = CoreObject.FindChilds(coreObject.CoreObjectID);
                            bool IsFind = false;
                            foreach (CoreObject LaneCoreItem in LaneCoreList)
                            {
                                BpmnLane bpmnLane = new BpmnLane(LaneCoreItem);
                                if (bpmnLane.ID == BpmnID) 
                                    return Redirect("/SysSetting/LoadSysSettingDetail?SysSettingType=BpmnLane&ID=" + LaneCoreItem.CoreObjectID + "&ShowInNewWindow=true"); 

                            } 
                        }
                        break;
                    }
                default:
                    {
                        foreach (CoreObject coreObject in CoreObjectsList)
                        {  
                            List<CoreObject> LaneCoreList = CoreObject.FindChilds(coreObject.CoreObjectID);
                            foreach (CoreObject LaneCoreItem in LaneCoreList)
                            {
                                List<CoreObject> ProcessStepList = CoreObject.FindChilds(LaneCoreItem.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند);
                                foreach (CoreObject ProcessStepCoreItem in ProcessStepList)
                                    if (ProcessStepCoreItem.FullName == BpmnID)
                                        return Redirect("/SysSetting/LoadSysSettingDetail?SysSettingType=ProcessStep&ID=" + ProcessStepCoreItem.CoreObjectID + "&ShowInNewWindow=true");
                                    else
                                    {
                                        List<CoreObject> BpmnIncomingList = CoreObject.FindChilds(ProcessStepCoreItem.CoreObjectID, CoreDefine.Entities.ورودی_فرآیند);
                                        foreach(CoreObject BpmnIncomingCore in BpmnIncomingList)
                                        {
                                            if (BpmnIncomingCore.FullName == BpmnID)
                                                return Redirect("/SysSetting/LoadSysSettingDetail?SysSettingType=BpmnIncoming&ID=" + BpmnIncomingCore.CoreObjectID + "&ShowInNewWindow=true");
                                        }

                                        List<CoreObject> BpmnOutingList = CoreObject.FindChilds(ProcessStepCoreItem.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند);
                                        foreach(CoreObject BpmnIncomingCore in BpmnOutingList)
                                        {
                                            if (BpmnIncomingCore.FullName == BpmnID)
                                                return Redirect("/SysSetting/LoadSysSettingDetail?SysSettingType=BpmnOutgoin&ID=" + BpmnIncomingCore.CoreObjectID + "&ShowInNewWindow=true");
                                        }
                                    }
                            } 
                        }
                        break;

                    }
            }
 
            ViewData["BpmnEntities"] = ChildEntities;
            ViewData["BpmnCoreObject"] = ResultCoreObject;
            return View();
        }
    }
}