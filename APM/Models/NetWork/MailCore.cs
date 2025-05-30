using APM.Models.Database;
using Microsoft.Extensions.Hosting;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace APM.Models.NetWork
{
    public class MailCore
    {

        public string EMail { get; set; }
        public string EMailUserName { get; set; }
        public string EMailPassword { get; set; }
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

        public void SendMailMessage(List<string> EmailAccount,List<MailAttachmnet> AttachementList,ref string ErrorMessage)
        {  
            using (MailMessage message = new MailMessage())
            {
                foreach (MailAttachmnet MailAttachmnet in AttachementList)
                {
                    MemoryStream memory = new MemoryStream(MailAttachmnet.AttachmentMemory.ToArray());
                    message.Attachments.Add(new System.Net.Mail.Attachment(memory, MailAttachmnet.AttachmentName));
                }

                foreach (string EmailAccountItem in EmailAccount)
                    message.To.Add(EmailAccountItem);

                message.From = new MailAddress(EMail);
                message.Subject = Title;
                message.Body = BodyMessage;
                message.IsBodyHtml = true;
                try
                {
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = EMailServer;
                        smtp.EnableSsl = true;
                        NetworkCredential network = new NetworkCredential((string.IsNullOrEmpty(EMailUserName) ? null : EMailUserName), (string.IsNullOrEmpty(EMailPassword) ? null : EMailPassword));
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = network;
                        smtp.Port = (EMailPort == "" ? 0 : int.Parse(EMailPort));
                        smtp.Send(message);

                        foreach (string EmailAccountItem in EmailAccount)
                            Referral.DBData.Execute("Insert into ایمیل (آدرس_ایمیل, عنوان, شرح, تاریخ, ساعت, ایمیل_ارسال_کننده, هاست, پورت, نام_کاربری, رمز_عبور, فعال_کردن_SSL, وضعیت_ارسال, کاربر_ارسال_کننده)" +
                                " values(N'" + EmailAccountItem + "'," +
                                "N'" + Title + "'," +
                                "N'" + BodyMessage + "'," +
                                "N'" + Models.Tools.CDateTime.GetNowshamsiDate() + "'," +
                                "N'" + Tools.CDateTime.GetNowTime() + "'," +
                                "N'" + EMail + "'," +
                                "N'" + EMailServer + "'," +
                                "N'" + EMailPort + "'," +
                                "N'" + EMailUserName + "'," +
                                "N'" + EMailPassword + "'," +
                                (EnableSsl ? "1" : "0") + "," +
                                "1," +
                                Referral.UserAccount.UsersID.ToString() + ")");
                    }

                }
                catch (Exception ex)
                {

                }
            } 
        }


        public void SyncSendMail(string DeclareQuery,string DataKey,string RecordID,string TableID,string[] ColumnNames,object[] _Values)
        {
            string Query = "Select ایمیل From کاربر left join  نقش_کاربر on کاربر.نقش_کاربر = نقش_کاربر.شناسه  where 1<>1 ";

            foreach (string Item in ReceivingUsers)
                if (!string.IsNullOrEmpty(Item))
                    Query += "OR  کاربر.شناسه = " + Item.Split('_')[1] + " \n";

            if (InsertingUser)
                Query += "OR  کاربر.شناسه = " + Referral.UserAccount.UsersID.ToString() + " \n";

            foreach (string Item in ReceivingRole)
                if (!string.IsNullOrEmpty(Item))
                    Query += "OR  نقش_کاربر.شناسه = " + Item.Split('_')[1] + " \n";

            Query += "\nand isnull(ایمیل,N'')<>N''";

            List<string> ReceivingUsersEmail = Referral.DBData.SelectColumn(Query).OfType<string>().ToList();

            if (!string.IsNullOrEmpty(ReceivingQuery))
                ReceivingUsersEmail.AddRange(Referral.DBData.SelectColumn(DeclareQuery + "\n" + Tools.Tools.CheckQuery(ReceivingQuery)).OfType<string>().ToList());

            ReceivingUsersEmail = ReceivingUsersEmail.Distinct().ToList();
            ReceivingUsersEmail.RemoveAll(x => string.IsNullOrEmpty(x));

            if (ReceivingUsersEmail.Count > 0)
            {

                List<MailAttachmnet> MemoryStreamsList = new List<MailAttachmnet>();
                if (SendReport.Length > 0)
                {
                    foreach (string Item in SendReport)
                    {
                        if (!string.IsNullOrEmpty(Item))
                        {
                            Report report = new Report(CoreObject.Find(long.Parse(Item.Split('_')[1])));
                            string[] ParameterName = new string[0];
                            string[] ParameterValue = new string[0];

                            foreach (ReportParameter reportParameter in Desktop.DataReport[DataKey])
                            {
                                Array.Resize(ref ParameterName, ParameterName.Length + 1);
                                Array.Resize(ref ParameterValue, ParameterValue.Length + 1);
                                ParameterName[ParameterName.Length - 1] = reportParameter.FullName;
                                ParameterValue[ParameterName.Length - 1] = RecordID;
                            }

                            StiReport BuildedReport = report.Build(report.CoreObjectID, ParameterName, ParameterValue);


                            BuildedReport.Compile();
                            BuildedReport.Render();
                            MemoryStream memory = new MemoryStream();
                            StiPdfExportService pdfService = new StiPdfExportService();
                            pdfService.ExportPdf(BuildedReport, memory);
                            MemoryStreamsList.Add(new MailAttachmnet() { AttachmentMemory = memory, AttachmentName =Tools.Tools.UnSafeTitle(report.FullName) + ".pdf" });

                        }
                    }
                }

                if (SendAttachmentFile)
                {
                    string PDestinationPath = Models.Attachment.MapFileSavingAttachmentPath + TableID + "/" +RecordID;
                    DirectoryInfo directoryInfo = new DirectoryInfo(PDestinationPath);
                    if(directoryInfo.Exists)
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        string FileText = File.ReadAllText(file.FullName);
                        byte[] fileData = Convert.FromBase64String(FileText);
                        MemoryStream memory = new MemoryStream(fileData);
                        MemoryStreamsList.Add(new MailAttachmnet() { AttachmentMemory = memory, AttachmentName = file.Name });
                    }
                }
                  
                for (int Index = 0; Index < ColumnNames.Length; Index++)
                {
                    if (Title.IndexOf("@" + ColumnNames[Index] + " ") > -1)
                        Title = Title.Replace("@" + ColumnNames[Index] + " ", _Values[Index].ToString());

                    if (BodyMessage.IndexOf("@" + ColumnNames[Index] + " ") > -1)
                        BodyMessage = BodyMessage.Replace("@" + ColumnNames[Index] + " ", _Values[Index].ToString());

                }

                if (!string.IsNullOrEmpty(TitleQuery))
                    Title += " " + Referral.DBData.SelectField(DeclareQuery + "\n" + Tools.Tools.CheckQuery(TitleQuery)).ToString();

                if (!string.IsNullOrEmpty(BodyMessageQuery))
                    BodyMessage += " " + Referral.DBData.SelectField(DeclareQuery + "\n" + Tools.Tools.CheckQuery(BodyMessageQuery)).ToString();

 

                if (UsePublickEmail)
                {
                    this.EMail = Referral.PublicSetting.E_Maile;
                    this.EMailUserName = Referral.PublicSetting.EMaileUserName;
                    this.EMailPassword = Referral.PublicSetting.EMailePassWord;
                    this.EMailServer = Referral.PublicSetting.EMaileServer;
                    this.EMailPort = Referral.PublicSetting.EMailePort;
                    this.EnableSsl = Referral.PublicSetting.EnableSsl;
                }

                string ErrorMessage = "";
                SendMailMessage(ReceivingUsersEmail, MemoryStreamsList,ref ErrorMessage); 
            }
        }
 
 
    }

    public class MailAttachmnet
    {
        public MemoryStream AttachmentMemory { get; set; }
        public string AttachmentName { get; set; }
    }
}