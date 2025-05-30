using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public static class Notices
    {
        public partial class Email
        {
            public string Address { get; set; }
            public string UserName { get; set; }
            public string PassWord { get; set; }
            public string ServerName { get; set; }
            public string Port { get; set; }
            public Email()
            { 
                this.Address = ""; 
                this.UserName = ""; 
                this.PassWord = ""; 
                this.ServerName = ""; 
                this.Port = ""; 
            }

            public Email(string Address, string UserName, string PassWord, string ServerName, string Port)
            {
                this.Address = Address;
                this.UserName = UserName;
                this.PassWord = PassWord;
                this.ServerName = ServerName;
                this.Port = Port;
            }
            public Email(CoreObject _CoreObject)
            {
                string ValueXml = _CoreObject.Value.ToString();
                var stringReader = new System.IO.StringReader(ValueXml);
                var serializer = new XmlSerializer(typeof(Email));
                var EmailInfo = serializer.Deserialize(stringReader) as Email;
                this.Address = EmailInfo.Address; 
                this.UserName = EmailInfo.UserName; 
                this.PassWord = EmailInfo.PassWord; 
                this.ServerName = EmailInfo.ServerName; 
                this.Port = EmailInfo.Port; 
            }

        }
        public partial class SMS
        {
            public string Number { get; set; }
            public string UserName { get; set; }
            public string PassWord { get; set; }
            public CoreDefine.SmsServerType ServerName { get; set; }
            public SMS()
            { 
                this.Number = ""; 
                this.UserName = ""; 
                this.PassWord = ""; 
                this.ServerName = CoreDefine.SmsServerType.None; 
            }

            public SMS(string Number, string UserName, string PassWord, CoreDefine.SmsServerType ServerName)
            {
                this.Number = Number;
                this.UserName = UserName;
                this.PassWord = PassWord;
                this.ServerName = ServerName;
            }
            public SMS(CoreObject _CoreObject)
            {
                string ValueXml = _CoreObject.Value.ToString();
                if (ValueXml == "")
                {
                    SMS _Sms= new SMS();
                    this.Number = _Sms.Number;
                    this.UserName = _Sms.UserName;
                    this.PassWord = _Sms.PassWord;
                    this.ServerName = _Sms.ServerName;
                }
                else
                {
                    var stringReader = new System.IO.StringReader(ValueXml);
                    var serializer = new XmlSerializer(typeof(SMS));
                    var EmailInfo = serializer.Deserialize(stringReader) as SMS;
                    this.Number = EmailInfo.Number;
                    this.UserName = EmailInfo.UserName;
                    this.PassWord = EmailInfo.PassWord;
                    this.ServerName = EmailInfo.ServerName;
                }
            }

        }
    }
}