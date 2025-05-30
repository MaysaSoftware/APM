using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using static APM.Models.Tools.CoreDefine;

namespace APM.Models.APMObject
{
    public class TableEvent
    {
        public string TypeEventExecution { get; set; }
        public string EventType { get; set; }
        public string Condition { get; set; }
        public string Query { get; set; }
        public long RelatedTable { get; set; }
        public long RelatedWebService { get; set; }

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


        public string[] ReferralRecipientsUser { get; set; }
        public string[] ReferralRecipientsRole { get; set; }
        public string ReferralRecipientsQuery { get; set; }
        public int ReferralDeadlineResponse { get; set; }
        public string ReferralTitle { get; set; }
        public string ReferralTitleQuery { get; set; }



        public TableEvent()
        {
            this.TypeEventExecution = TableTypeEventExecution.اجرای_کوئری.ToString();
            this.EventType = string.Empty;
            this.Condition = string.Empty;
            this.Query = string.Empty;
            this.RelatedTable = 0;
            this.RelatedWebService = 0;
            this.EMailServer = "smtp.gmail.com";
            this.EMailPort = "587";
            EnableSsl =true;
            UsePublickEmail = false;
        }
        public TableEvent(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(TableEvent));
            var TableEventInfo = serializer.Deserialize(stringReader) as TableEvent;
            this.EventType = TableEventInfo.EventType;
            this.Query = TableEventInfo.Query;
            this.RelatedTable = TableEventInfo.RelatedTable;
            this.RelatedWebService = TableEventInfo.RelatedWebService;
            this.TypeEventExecution = TableEventInfo.TypeEventExecution;
            this.Condition = TableEventInfo.Condition;
            this.EMail = TableEventInfo.EMail;  
            this.EMailUserName = TableEventInfo.EMailUserName;
            this.EMailPassWord = TableEventInfo.EMailPassWord;
            this.EMailServer = TableEventInfo.EMailServer;
            this.EMailPort = TableEventInfo.EMailPort;
            this.ReceivingUsers = TableEventInfo.ReceivingUsers;
            this.ReceivingRole = TableEventInfo.ReceivingRole;
            this.ReceivingQuery = TableEventInfo.ReceivingQuery;
            this.InsertingUser = TableEventInfo.InsertingUser;
            this.SendAttachmentFile = TableEventInfo.SendAttachmentFile;
            this.SendReport = TableEventInfo.SendReport;
            this.Title = TableEventInfo.Title;
            this.TitleQuery = TableEventInfo.TitleQuery;
            this.BodyMessage = TableEventInfo.BodyMessage;
            this.BodyMessageQuery = TableEventInfo.BodyMessageQuery;
            this.EnableSsl = TableEventInfo.EnableSsl;
            this.UsePublickEmail = TableEventInfo.UsePublickEmail;
             
            this.ReferralRecipientsUser = TableEventInfo.ReferralRecipientsUser;
            this.ReferralRecipientsRole = TableEventInfo.ReferralRecipientsRole;
            this.ReferralRecipientsQuery = TableEventInfo.ReferralRecipientsQuery;
            this.ReferralDeadlineResponse = TableEventInfo.ReferralDeadlineResponse;
            this.ReferralTitle = TableEventInfo.ReferralTitle;
            this.ReferralTitleQuery = TableEventInfo.ReferralTitleQuery;
        }

    }
}