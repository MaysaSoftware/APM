using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public class TableButton
    {
        public string FullName { get; set; }
        public string Icon { get; set; }
        public string Query { get; set; }
        public string ButtonColor { get; set; }
        public string ExecutionConditionQuery { get; set; }
        public CoreDefine.TableButtonEventsType TableButtonEventsType { get; set; }
        public long CoreObjectID { get; set; }
        public long RelatedForm { get; set; }
        public long RelatedField { get; set; }
        public long RelatedWebService { get; set; }
        public bool IsReloadGrid { get; set; }
        public bool IsShowText { get; set; }
        public bool IsShowIcon { get; set; } 

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
        public string URL { get; set; }


        public TableButton()
        {
            IsReloadGrid = false;
            IsShowText = false;
            IsShowIcon = true;
            RelatedForm = 0;
            RelatedField = 0;
            RelatedWebService=0;
            this.EMailServer = "smtp.gmail.com";
            this.EMailPort = "587";
            EnableSsl = true;
            UsePublickEmail = false;
            URL=String.Empty;
            ButtonColor = Referral.PublicSetting.MainColor;
        }
        public TableButton(CoreDefine.TableButtonEventsType TableButtonEventsType, string Query, string Icon, bool IsReloadGrid = false)
        {
            this.TableButtonEventsType = TableButtonEventsType;
            this.Query = Query;
            this.Icon = Icon;
        }

        public TableButton(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(TableButton));
            var ButtonInfo = serializer.Deserialize(stringReader) as TableButton; 

            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID; 
            this.Query = ButtonInfo.Query;
            this.Icon = ButtonInfo.Icon;
            this.TableButtonEventsType = ButtonInfo.TableButtonEventsType;
            this.IsReloadGrid = ButtonInfo.IsReloadGrid;
            this.RelatedForm = ButtonInfo.RelatedForm;
            this.RelatedField = ButtonInfo.RelatedField;
            this.RelatedWebService = ButtonInfo.RelatedWebService;
            this.IsShowText = ButtonInfo.IsShowText;
            this.IsShowIcon = ButtonInfo.IsShowIcon;
            this.ExecutionConditionQuery = ButtonInfo.ExecutionConditionQuery;
            this.EMail = ButtonInfo.EMail;
            this.EMailUserName = ButtonInfo.EMailUserName;
            this.EMailPassWord = ButtonInfo.EMailPassWord;
            this.EMailServer = ButtonInfo.EMailServer;
            this.EMailPort = ButtonInfo.EMailPort;
            this.ReceivingUsers = ButtonInfo.ReceivingUsers;
            this.ReceivingRole = ButtonInfo.ReceivingRole;
            this.ReceivingQuery = ButtonInfo.ReceivingQuery;
            this.InsertingUser = ButtonInfo.InsertingUser;
            this.SendAttachmentFile = ButtonInfo.SendAttachmentFile;
            this.SendReport = ButtonInfo.SendReport;
            this.Title = ButtonInfo.Title;
            this.TitleQuery = ButtonInfo.TitleQuery;
            this.BodyMessage = ButtonInfo.BodyMessage;
            this.BodyMessageQuery = ButtonInfo.BodyMessageQuery;
            this.EnableSsl = ButtonInfo.EnableSsl;
            this.UsePublickEmail = ButtonInfo.UsePublickEmail;
            this.URL = ButtonInfo.URL;
            this.ButtonColor = ButtonInfo.ButtonColor;
        }
        public TableButton(DisplayField _DisplayField)
        { 

            this.FullName = _DisplayField.FullName;
            this.CoreObjectID = _DisplayField.CoreObjectID;
     
            this.Query = _DisplayField.Query;
            this.Icon = _DisplayField.Icon;
            this.TableButtonEventsType = _DisplayField.TableButtonEventsType;
            this.IsReloadGrid = _DisplayField.IsReloadGrid;
            this.RelatedForm = _DisplayField.RelatedForm;
            this.RelatedField = _DisplayField.RelatedField;
            //this.RelatedWebService = _DisplayField.RelatedWebService;
            //this.IsShowText = _DisplayField.IsShowText;
            //this.IsShowIcon = _DisplayField.IsShowIcon;
            this.ExecutionConditionQuery = _DisplayField.ExecutionConditionQuery;
            this.EMail = _DisplayField.EMail;
            this.EMailUserName = _DisplayField.EMailUserName;
            this.EMailPassWord = _DisplayField.EMailPassWord;
            this.EMailServer = _DisplayField.EMailServer;
            this.EMailPort = _DisplayField.EMailPort;
            this.ReceivingUsers = _DisplayField.ReceivingUsers;
            this.ReceivingRole = _DisplayField.ReceivingRole;
            this.ReceivingQuery = _DisplayField.ReceivingQuery;
            this.InsertingUser = _DisplayField.InsertingUser;
            this.SendAttachmentFile = _DisplayField.SendAttachmentFile;
            this.SendReport = _DisplayField.SendReport;
            this.Title = _DisplayField.Title;
            this.TitleQuery = _DisplayField.TitleQuery;
            this.BodyMessage = _DisplayField.BodyMessage;
            this.BodyMessageQuery = _DisplayField.BodyMessageQuery;
            this.EnableSsl = _DisplayField.EnableSsl;
            this.UsePublickEmail = _DisplayField.UsePublickEmail;
            this.URL = _DisplayField.URL;
        }

    }
}