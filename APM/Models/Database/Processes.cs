using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public partial class ProcessStep
    {
        public  long CoreObjectID { get; set; }
        public  string FullName { get; set; }
        public  string ID { get; set; }
        public  string Name { get; set; } 
        public  long InformationEntryFormID { get; set; }
        public  CoreDefine.ProcessStepActionType ActionType { get; set; }
        public  CoreDefine.ProcessStepRecordType RecordType { get; set; }
        public int ProgressPercent { get; set; }
        public string Comment { get; set; }
        public string ResponseDeadline { get; set; }
        public CoreDefine.NotificationType NotificationType { get; set; }
        public string EMail { get; set; }
        public string EMailUserName { get; set; }
        public string EMailPassWord { get; set; }
        public string EMailServer { get; set; }
        public string EMailPort { get; set; }
        public string[] ReceivingUsers { get; set; }
        public string[] ReceivingRole { get; set; }
        public bool InsertingUser { get; set; }
        public string ReceivingQuery { get; set; }
        public bool SendAttachmentFile { get; set; }
        public bool EnableSsl { get; set; }
        public bool UsePublickEmail { get; set; }
        public string[] SendReport { get; set; }
        public string Title { get; set; }
        public string TitleQuery { get; set; }
        public string BodyMessage { get; set; }
        public string BodyMessageQuery { get; set; }

        public ProcessStep()
        { 
            this.InformationEntryFormID = 0;
            this.ActionType = CoreDefine.ProcessStepActionType.خالی;
            this.RecordType = CoreDefine.ProcessStepRecordType.خالی;
            this.ProgressPercent = 0;
        } 

        public ProcessStep(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ProcessStep));
            var ProcessStepInfo = serializer.Deserialize(stringReader) as ProcessStep;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.FullName = _CoreObject.FullName;  
            this.InformationEntryFormID = ProcessStepInfo.InformationEntryFormID;
            this.ActionType= ProcessStepInfo.ActionType;
            this.RecordType = ProcessStepInfo.RecordType;
            this.ProgressPercent = ProcessStepInfo.ProgressPercent;
            this.Name = ProcessStepInfo.Name;
            this.ID = ProcessStepInfo.ID;
            this.Comment = ProcessStepInfo.Comment;
            this.ResponseDeadline = ProcessStepInfo.ResponseDeadline;
            this.NotificationType = ProcessStepInfo.NotificationType; 
            this.EMail = ProcessStepInfo.EMail;
            this.EMailUserName = ProcessStepInfo.EMailUserName;
            this.EMailPassWord = ProcessStepInfo.EMailPassWord;
            this.EMailServer = ProcessStepInfo.EMailServer;
            this.EMailPort = ProcessStepInfo.EMailPort;
            this.ReceivingUsers = ProcessStepInfo.ReceivingUsers;
            this.ReceivingRole = ProcessStepInfo.ReceivingRole;
            this.ReceivingQuery = ProcessStepInfo.ReceivingQuery;
            this.InsertingUser = ProcessStepInfo.InsertingUser;
            this.SendAttachmentFile = ProcessStepInfo.SendAttachmentFile;
            this.SendReport = ProcessStepInfo.SendReport;
            this.Title = ProcessStepInfo.Title;
            this.TitleQuery = ProcessStepInfo.TitleQuery;
            this.BodyMessage = ProcessStepInfo.BodyMessage;
            this.BodyMessageQuery = ProcessStepInfo.BodyMessageQuery;
            this.EnableSsl = ProcessStepInfo.EnableSsl;
            this.UsePublickEmail = ProcessStepInfo.UsePublickEmail;
        }
    }
    
    public partial class ProcessReferral
    {
       public  long Personnel { get; set; }
       public  long OrganizationLevel { get; set; }
       public  long ReferralType { get; set; }
       public string Comment { get; set; }
       public string ResponseDeadline { get; set; }
       public string Query { get; set; }
       public CoreDefine.NotificationType NotificationType { get; set; }

        public ProcessReferral()
        {
            this.Personnel = 0;
            this.OrganizationLevel = 0;
            this.ReferralType = 0;
            this.Comment = "";
            this.ResponseDeadline = "";
            this.Query = "";
            this.NotificationType = CoreDefine.NotificationType.سیستمی;
        } 

        public ProcessReferral(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ProcessReferral));
            var ProcessReferralInfo = serializer.Deserialize(stringReader) as ProcessReferral;
            this.Personnel = ProcessReferralInfo.Personnel;
            this.OrganizationLevel = ProcessReferralInfo.OrganizationLevel;
            this.ReferralType = ProcessReferralInfo.ReferralType;
            this.Comment = ProcessReferralInfo.Comment;
            this.ResponseDeadline = ProcessReferralInfo.ResponseDeadline;
            this.NotificationType = ProcessReferralInfo.NotificationType;
            this.Query = ProcessReferralInfo.Query;
        }
    }

    public partial class ProcessType
    { 
        public string Icon { get; set; }  
        public string ProcessName { get; set; }  
        public string Version { get; set; }  
        public long ProcessID { get; set; }
        public long ReportID { get; set; }
        public long ParameterReportID { get; set; } 
        public ProcessModel ProcessModel { get; set; }
        public string ProcessModelXml { get; set; }

        public long InformationEntryFormID { get; set; }

        public ProcessType()
        {
            this.Icon = "";
            this.ReportID = 0;
            this.ProcessID = 0;
            this.ParameterReportID = 0;
            this.ProcessModel = new ProcessModel();
            this.ProcessModelXml = "";
            this.ProcessName = "";
            this.Version = "1.0";
            InformationEntryFormID = 0;
        }
        public ProcessType(string Icon,long ReportID,long ParameterReportID, ProcessModel ProcessModel,string ProcessModelXml)
        {
            this.Icon = Icon;
            this.ReportID = ReportID;
            this.ParameterReportID = ParameterReportID;
            this.ProcessModel = ProcessModel;
            this.ProcessModelXml = ProcessModelXml;
        }

        public ProcessType(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ProcessType));
            var ProcessTypeInfo = serializer.Deserialize(stringReader) as ProcessType; 
            this.ProcessID = _CoreObject.CoreObjectID;
            this.ProcessName = _CoreObject.FullName;
            this.Icon = ProcessTypeInfo.Icon;
            this.Version = ProcessTypeInfo.Version;
            this.ReportID = ProcessTypeInfo.ReportID;
            this.ParameterReportID = ProcessTypeInfo.ParameterReportID;
            this.ProcessModel = ProcessTypeInfo.ProcessModel;
            this.ProcessModelXml = ProcessTypeInfo.ProcessModelXml;
            this.InformationEntryFormID = ProcessTypeInfo.InformationEntryFormID;
        }
    }
    
    public partial class ProcessModel
    { 
        public List<ProcessReferral> ProcessReferral { get; set; }   

        public ProcessModel()
        {
            this.ProcessReferral = new List<ProcessReferral>(); 
        }
        public ProcessModel(List<ProcessReferral> ProcessReferral)
        {
            this.ProcessReferral = ProcessReferral; 
        }

        public ProcessModel(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ProcessModel));
            var ProcessModelInfo = serializer.Deserialize(stringReader) as ProcessModel; 
            this.ProcessReferral = ProcessModelInfo.ProcessReferral; 
        }
    }
    
    public partial class ProcessStepEvent
    { 
        public string Command { get; set; }  

        public ProcessStepEvent()
        {
            this.Command = "";
        }
        public ProcessStepEvent(string Command)
        {
            this.Command = Command;
        }

        public ProcessStepEvent(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ProcessStepEvent));
            var ProcessStepEventInfo = serializer.Deserialize(stringReader) as ProcessStepEvent; 
            this.Command = ProcessStepEventInfo.Command;
        }
    }

}