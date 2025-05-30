using APM.Models;
using APM.Models.Database;
using APM.Models.Diagram;
using APM.Models.Setting;
using APM.Models.NetWork;
using APM.Models.Tools;
using APM.Models.APMObject;
using Kendo.Mvc.UI;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using APM.Models.APMObject.InformationForm;
using Kendo.Mvc.Extensions;
using System.Xml.Serialization;
using System.Text;

namespace APM.Controllers
{
    public class SysSettingController : Controller
    {
        // GET: SysSetting 
        private static DataTank.SysSetting SysSetting = new DataTank.SysSetting();
        public ActionResult Index()
        {
            ViewData["ShowInNewWindow"] = true;
            return View();
        }

        public JsonResult Download(string Format)
        {
            if (SysSetting.SysSettingEntity == CoreDefine.Entities.فرآیند.ToString())
            {
                ProcessType process = new ProcessType(CoreObject.Find(SysSetting.SysSettingID));
                Session["SysSetting_ProcessModelXml"] = (process.ProcessModelXml == "" ? BPMN.InitiXML : process.ProcessModelXml);
                Session["PrintSettingName"] = Tools.UnSafeTitle(SysSetting.SysSettingName);
            }
            return Json(Referral.DBRegistry.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                                                                  , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, SysSetting.SysSettingID, SysSetting.SysSettingEntity, SysSetting.SysSettingName, Format, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion }));
        }

        public ActionResult LoadSysSettingDetail(string SysSettingType, long ID,bool ShowInNewWindow=false)
        {
            List<TemplateField> FieldParameter = new List<TemplateField>();
            List<Folder> FolderList = new List<Folder>();
            SysSetting.SysSettingType = SysSettingType;
            SysSetting.SysSettingID = ID;
            CoreObject _CoreObject = CoreObject.Find(ID);
            SysSetting.SysSettingEntity = ID == 0 ? "" : _CoreObject.Entity.ToString();
            SysSetting.SysSettingName = ID == 0 ? "" : _CoreObject.FullName;
            ViewData["FormComment"] = null;
            ViewData["SysSettingID"] = ID;

            if (FolderList.Count == 0)
                FolderList.Add(new Folder() { FullName= "عمومی" });

            switch (SysSettingType)
            {
                case "Folder":
                    {
                        Folder folder = new Folder(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", folder.Icon, CoreDefine.InputTypes.Icon.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IconColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ آیکن(پیشفرض:"+Referral.PublicSetting.IconColor+")", "IconColor", folder.IconColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IconSize", CoreDefine.InputTypes.Number, Tools.ParameterField("اندازه آیکن(پیشفرض:12)", "IconSize", folder.IconSize, CoreDefine.InputTypes.Number.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsExpand", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("گسترش یابد", "IsExpand", folder.IsExpand, CoreDefine.InputTypes.TwoValues.ToString())));
                       
                        break;
                    }
                case "DataSource":
                    {
                        DataSourceInfo DataSource = new DataSourceInfo(_CoreObject);
                        List<SelectListItem> FieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = ""},
                            new SelectListItem() {Text = "SQL SERVER", Value =CoreDefine.DataSourceType.SQLSERVER.ToString()},
                            new SelectListItem() {Text = "EXCEL", Value =CoreDefine.DataSourceType.EXCEL.ToString()},
                            new SelectListItem() {Text = "ACCESS", Value =CoreDefine.DataSourceType.ACCESS.ToString()},
                            new SelectListItem() {Text = "MY SQL", Value =CoreDefine.DataSourceType.MySql.ToString()},
                        };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "DataSourceType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع پایگاه داده", "DataSourceType", DataSource.DataSourceType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, FieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ServerName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("سرور", "ServerName", DataSource.ServerName, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "UserName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربری", "UserName", DataSource.UserName, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Password", CoreDefine.InputTypes.Password, Tools.ParameterField("کلمه عبور", "Password", DataSource.Password, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DataBase", CoreDefine.InputTypes.ShortText, Tools.ParameterField("بانک داده", "DataBase", DataSource.DataBase, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "FilePath", CoreDefine.InputTypes.ShortText, Tools.ParameterField("مسیر فایل", "FilePath", DataSource.FilePath, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        break;
                    }

                case "TableButton":
                    {
                        FolderList.Add(new Folder() { FullName = "تنظیمات ایمیل" });
                        TableButton TableButton = new TableButton(_CoreObject);

                        InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(_CoreObject.ParentID));
                        List<CoreObject> ParameterCore = CoreObject.FindChilds(CoreDefine.Entities.پارامتر_گزارش);
                        List<string> ReportList = new List<string>();
                        foreach (var item in ParameterCore)
                        {
                            ReportParameter reportParameter = new ReportParameter(item);
                            if (reportParameter.RelatedTable == informationEntryForm.RelatedTable)
                            {
                                CoreObject ReportCore = CoreObject.Find(item.ParentID);
                                ReportList.Add(Tools.UnSafeTitle(ReportCore.FullName) + "_" + ReportCore.CoreObjectID);
                            }
                        }


                        List<SelectListItem> FieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.TableButtonEventsType.خالی.ToString()},
                            new SelectListItem() {Text = "اجرای رویداد", Value =CoreDefine.TableButtonEventsType.اجرای_رویداد.ToString()},
                            new SelectListItem() {Text = "اجرای وب سرویس", Value =CoreDefine.TableButtonEventsType.اجرای_وب_سرویس.ToString()},
                            new SelectListItem() {Text = "ارسال ایمیل", Value =CoreDefine.TableButtonEventsType.ارسال_ایمیل.ToString()},
                            new SelectListItem() {Text = "انتقال فایل", Value =CoreDefine.TableButtonEventsType.انتقال_فایل.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم فقط خواندنی", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_فقط_خواندنی.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم به صورت ویرایش", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_به_صورت_ویرایش.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم.ToString()},
                            new SelectListItem() {Text = "نمایش ضمیمه", Value =CoreDefine.TableButtonEventsType.نمایش_ضمیمه.ToString()},
                            new SelectListItem() {Text = "تولید کلید عمومی", Value =CoreDefine.TableButtonEventsType.تولید_کلید_عمومی_مالیاتی.ToString()},
                            new SelectListItem() {Text = "بروز رسانی کالا و خدمت از سازمان امور مالیاتی", Value =CoreDefine.TableButtonEventsType.بروزرسانی_کالا_مالیات.ToString()},
                            new SelectListItem() {Text = "ارسال صورتحساب به سامانه مودیان", Value =CoreDefine.TableButtonEventsType.ارسال_صورتحساب_به_سامانه_مودیان.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم با لینک", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_با_لینک.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم به صورت ویرایش با لینک", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_به_صورت_ویرایش_با_لینک.ToString()},
                        };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "TableButtonEventsType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع عملیات", "TableButtonEventsType", TableButton.TableButtonEventsType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, FieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedForm", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فرم مرتبط", "RelatedForm", TableButton.RelatedForm.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedField", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فیلد مرتبط", "RelatedField", TableButton.RelatedField.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedWebService", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("وب سرویس مرتبط", "RelatedWebService", TableButton.RelatedWebService.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.وب_سرویس.ToString(), 0)));

                        FieldParameter.Add(new TemplateField(false, "عمومی", "ButtonColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ دکمه(پیشفرض:" + Referral.PublicSetting.MainColor + ")", "ButtonColor", TableButton.ButtonColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", TableButton.Icon, CoreDefine.InputTypes.Icon.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsShowIcon", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش آیکن", "IsShowIcon", TableButton.IsShowIcon, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsShowText", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش نوشته", "IsShowText", TableButton.IsShowText, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsReloadGrid", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("بروز رسانی داده", "IsReloadGrid", TableButton.IsReloadGrid, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "URL", CoreDefine.InputTypes.ShortText, Tools.ParameterField("آدرس", "URL", TableButton.URL, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "ExecutionConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط اجرای کوئری", "ExecutionConditionQuery", new string[] { TableButton.ExecutionConditionQuery, Tools.ConvertToSQLQuery(TableButton.ExecutionConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { TableButton.Query, Tools.ConvertToSQLQuery(TableButton.Query) }, CoreDefine.InputTypes.Query.ToString())));



                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "UsePublickEmail", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("استفاده از ایمیل پیشفرض", "UsePublickEmail", TableButton.UsePublickEmail.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMail", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ایمیل", "EMail", TableButton.EMail, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailUserName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربری", "EMailUserName", TableButton.EMailUserName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailPassWord", CoreDefine.InputTypes.Password, Tools.ParameterField("کلمه عبور", "EMailPassWord", TableButton.EMailPassWord, CoreDefine.InputTypes.Password.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailServer", CoreDefine.InputTypes.ShortText, Tools.ParameterField("سرور", "EMailServer", TableButton.EMailServer, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailPort", CoreDefine.InputTypes.ShortText, Tools.ParameterField("پورت", "EMailPort", TableButton.EMailPort, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EnableSsl", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("فعال کردن SSL", "EnableSsl", TableButton.EnableSsl.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ReceivingUsers", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingUsers", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "کاربران دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = TableButton.ReceivingUsers, DataList = Referral.DBData.SelectColumn("Select REPLACE(نام_و_نام_خانوادگی,N'_',N' ') +N'_'+ cast(شناسه as nvarchar(255)) from کاربر ").OfType<string>().ToList() }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ReceivingRole", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingRole", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "نقش کاربر دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = TableButton.ReceivingRole, DataList = Referral.DBData.SelectColumn("Select REPLACE(عنوان,N'_',N' ')  +N'_'+  cast(شناسه as nvarchar(255))  from نقش_کاربر ").OfType<string>().ToList() }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "InsertingUser", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("کاربر درج کننده", "InsertingUser", TableButton.InsertingUser, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "ReceivingQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری دریافت کنندگان", "ReceivingQuery", new string[] { TableButton.ReceivingQuery, Tools.ConvertToSQLQuery(TableButton.ReceivingQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "SendAttachmentFile", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ارسال پیوست", "SendAttachmentFile", TableButton.SendAttachmentFile, CoreDefine.InputTypes.TwoValues.ToString())));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "SendReport", CoreDefine.InputTypes.MultiSelect, new { FieldName = "SendReport", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "ارسال گزارش", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = TableButton.SendReport, DataList = ReportList }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "Title", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان", "Title", TableButton.Title, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "TitleQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری عنوان", "TitleQuery", new string[] { TableButton.TitleQuery, Tools.ConvertToSQLQuery(TableButton.TitleQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "BodyMessage", CoreDefine.InputTypes.ShortText, Tools.ParameterField("متن", "BodyMessage", TableButton.BodyMessage, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "BodyMessageQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری متن", "BodyMessageQuery", new string[] { TableButton.BodyMessageQuery, Tools.ConvertToSQLQuery(TableButton.BodyMessageQuery) }, CoreDefine.InputTypes.Query.ToString())));


                        break;
                    }

                case "PublicJob":
                    {
                        PublicJob TableButton = new PublicJob(_CoreObject);
                        List<SelectListItem> FieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.PublicJobType.خالی.ToString()},
                            new SelectListItem() {Text = "اجرای رویداد", Value =CoreDefine.PublicJobType.اجرای_رویداد.ToString()},
                            new SelectListItem() {Text = "ارسال پیامک", Value =CoreDefine.PublicJobType.ارسال_پیامک.ToString()},
                            new SelectListItem() {Text = "ارسال ایمیل", Value =CoreDefine.PublicJobType.ارسال_ایمیل.ToString()},
                            new SelectListItem() {Text = "انتقال فایل", Value =CoreDefine.PublicJobType.انتقال_فایل.ToString()},
                            new SelectListItem() {Text = "ارسال گزارش", Value =CoreDefine.PublicJobType.ارسال_گزارش.ToString()},
                        };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "PublicJobType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع عملیات", "PublicJobType", TableButton.PublicJobType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, FieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedDatasource", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("منبع مرتبط", "RelatedDatasource", TableButton.RelatedDatasource.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "StartDate", CoreDefine.InputTypes.PersianDateTime, Tools.ParameterField("تاریخ شروع", "StartDate", TableButton.StartDate.ToString(), CoreDefine.InputTypes.PersianDateTime.ToString(), false, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "EndDate", CoreDefine.InputTypes.PersianDateTime, Tools.ParameterField("تاریخ پایان", "EndDate", TableButton.EndDate.ToString(), CoreDefine.InputTypes.PersianDateTime.ToString(), false, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RepeatDay", CoreDefine.InputTypes.Number, Tools.ParameterField("تکرار روز", "RepeatDay", TableButton.RepeatDay, CoreDefine.InputTypes.Number.ToString(), false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "StartTime", CoreDefine.InputTypes.Clock, Tools.ParameterField("ساعت شروع", "StartTime", TableButton.StartTime, CoreDefine.InputTypes.Clock.ToString(), false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "EndTime", CoreDefine.InputTypes.Clock, Tools.ParameterField("ساعت پایان", "EndTime", TableButton.EndTime, CoreDefine.InputTypes.Clock.ToString(), false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RepeatClock", CoreDefine.InputTypes.Number, Tools.ParameterField("تکرار ساعت", "RepeatClock", TableButton.RepeatClock, CoreDefine.InputTypes.Number.ToString(), false, true)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { TableButton.Query, Tools.ConvertToSQLQuery(TableButton.Query) }, CoreDefine.InputTypes.Query.ToString())));
                        break;
                    }

                case "Table":
                    {
                        Table TableInfo = new Table(_CoreObject);

                        FieldParameter.Add(new TemplateField(true, "عمومی", "Comment", CoreDefine.InputTypes.ShortText, Tools.ParameterField("پیغام نمایشی", "Comment", TableInfo.Comment, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DisplayName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام نمایشی", "DisplayName", TableInfo.DisplayName, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "TABLESCHEMA", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربر امنیتی جدول", "TABLESCHEMA", TableInfo.TABLESCHEMA, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowRecordCountDefault", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد نمایش رکورد", "ShowRecordCountDefault", TableInfo.ShowRecordCountDefault, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "AnalystDescription", CoreDefine.InputTypes.LongText, Tools.ParameterField("توضیحات برنامه نویس/تحلیلگر", "AnalystDescription", TableInfo.AnalystDescription, CoreDefine.InputTypes.LongText.ToString())));
                        break;
                    }

                case "TableFunction":
                    {
                        TableFunction TableFunction = new TableFunction(_CoreObject);
                        List<SelectListItem> FieldNatureItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "هیچکدام", Value = ""},
                                                                      new SelectListItem() {Text = "رشته کوتاه", Value = "Nvarchar(400)"},
                                                                      new SelectListItem() {Text = "رشته طولانی", Value = "Nvarchar(MAX)"},
                                                                      new SelectListItem() {Text = "باینری کوتاه", Value = "Binary(800)"},
                                                                      new SelectListItem() {Text = "باینری طولانی", Value = "Binary(MAX)"},
                                                                      new SelectListItem() {Text = "عدد", Value ="Bigint"},
                                                                      new SelectListItem() {Text ="دو مقدار", Value =  "Bit"},
                                                                      new SelectListItem() {Text ="اعشار", Value =  "Float"},
                                                                      new SelectListItem() {Text ="xml", Value =  "xml"},
                                                                  };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "ReturnDataType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("ماهیت", "ReturnDataType", TableFunction.ReturnDataType, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, FieldNatureItems)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { TableFunction.Query, Tools.ConvertToSQLQuery(TableFunction.Query) }, CoreDefine.InputTypes.Query.ToString())));

                        break;
                    }

                case "TableEvent":
                    {
                        TableEvent EventInfo;
                        FolderList.Add(new Folder() { FullName = "تنظیمات ایمیل" });
                        FolderList.Add(new Folder() { FullName = "تنظیمات ارجاع" });

                        if (ID == 0)
                            EventInfo = new TableEvent();
                        else
                            EventInfo = new TableEvent(_CoreObject);

                        List<string> AllUserList = Tools.GetAllUserList();
                        List<string> AllPostList = Tools.GetAllPostList();

                        List<SelectListItem> TableEventItems = new List<SelectListItem>() {
                                                              new SelectListItem() {Text = "هیچکدام", Value = ""},
                                                              new SelectListItem() {Text = "هشدار قبل از درج", Value = CoreDefine.TableEvents.هشدار_قبل_از_درج.ToString()},
                                                              new SelectListItem() {Text = "شرط اجرای درج", Value = CoreDefine.TableEvents.شرط_اجرای_درج.ToString()},
                                                              new SelectListItem() {Text = "دستور بعد از درج", Value = CoreDefine.TableEvents.دستور_بعد_از_درج.ToString()},
                                                              new SelectListItem() {Text = "هشدار قبل از ویرایش", Value =CoreDefine.TableEvents.هشدار_قبل_از_ویرایش.ToString()},
                                                              new SelectListItem() {Text = "شرط اجرای ویرایش", Value =CoreDefine.TableEvents.شرط_اجرای_ویرایش.ToString()},
                                                              new SelectListItem() {Text ="دستور بعد از ویرایش", Value =  CoreDefine.TableEvents.دستور_بعد_از_ویرایش.ToString()},
                                                              new SelectListItem() {Text ="هشدار قبل از حذف", Value =  CoreDefine.TableEvents.هشدار_قبل_از_حذف.ToString()},
                                                              new SelectListItem() {Text ="شرط اجرای حذف", Value =  CoreDefine.TableEvents.شرط_اجرای_حذف.ToString()},
                                                              new SelectListItem() {Text ="دستور بعد از حذف", Value =  CoreDefine.TableEvents.دستور_بعد_از_حذف.ToString()},
                                                              new SelectListItem() {Text ="دستور بعد از بازیافت", Value =  CoreDefine.TableEvents.دستور_بعد_از_بازیافت.ToString()},
                                                          };

                        List<SelectListItem> TypeEventExecutionList = new List<SelectListItem>() {
                                                              new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.TableTypeEventExecution.خالی.ToString()},
                                                              new SelectListItem() {Text = "اجرای کوئری", Value = CoreDefine.TableTypeEventExecution.اجرای_کوئری.ToString()},
                                                              new SelectListItem() {Text = "صدور گواهی نامه الکترونیکی Src", Value = CoreDefine.TableTypeEventExecution.صدور_گواهی_نامه_الکترونیکی_Src.ToString()},
                                                              new SelectListItem() {Text = "ارسال ایمیل", Value = CoreDefine.TableTypeEventExecution.ارسال_ایمیل.ToString()},
                                                              new SelectListItem() {Text = "اجرای وب سرویس", Value = CoreDefine.TableTypeEventExecution.اجرای_وب_سرویس.ToString()},
                                                              new SelectListItem() {Text = "انتقال فایل", Value = CoreDefine.TableTypeEventExecution.انتقال_فایل.ToString()},
                                                              new SelectListItem() {Text = "ارجاع", Value = CoreDefine.TableTypeEventExecution.ارجاع.ToString()},
                                                          };

                        List<CoreObject> ParameterCore = CoreObject.FindChilds(CoreDefine.Entities.پارامتر_گزارش);
                        List<string> ReportList = new List<string>();
                        foreach (var item in ParameterCore)
                        {
                            ReportParameter reportParameter = new ReportParameter(item);
                            if (reportParameter.RelatedTable == _CoreObject.ParentID)
                            {
                                CoreObject ReportCore = CoreObject.Find(item.ParentID);
                                ReportList.Add(Tools.UnSafeTitle(ReportCore.FullName) + "_" + ReportCore.CoreObjectID);
                            }
                        }

                        FieldParameter.Add(new TemplateField(false, "عمومی", "TypeEventExecution", CoreDefine.InputTypes.ComboBox, new { FieldName = "TypeEventExecution", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع اجرای رویداد", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.TypeEventExecution, ComboItems = TypeEventExecutionList }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "EventType", CoreDefine.InputTypes.ComboBox, new { FieldName = "EventType", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع رویداد", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.EventType, ComboItems = TableEventItems }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedTable", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedTable", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "جدول مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.RelatedTable.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedWebService", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedWebService", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "وب سرویس مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.RelatedWebService.ToString(), Entitiy = CoreDefine.Entities.وب_سرویس.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Condition", CoreDefine.InputTypes.Query, new { FieldName = "Condition", InputType = CoreDefine.InputTypes.Query.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "شرط اجرای کوئری", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = new string[] { EventInfo.Condition, Tools.ConvertToSQLQuery(EventInfo.Condition) } }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, new { FieldName = "Query", InputType = CoreDefine.InputTypes.Query.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "کوئری", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = new string[] { EventInfo.Query, Tools.ConvertToSQLQuery(EventInfo.Query) } }));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "UsePublickEmail", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("استفاده از ایمیل پیشفرض", "UsePublickEmail", EventInfo.UsePublickEmail.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMail", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ایمیل", "EMail", EventInfo.EMail, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailUserName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربری", "EMailUserName", EventInfo.EMailUserName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailPassWord", CoreDefine.InputTypes.Password, Tools.ParameterField("کلمه عبور", "EMailPassWord", EventInfo.EMailPassWord, CoreDefine.InputTypes.Password.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailServer", CoreDefine.InputTypes.ShortText, Tools.ParameterField("سرور", "EMailServer", EventInfo.EMailServer, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailPort", CoreDefine.InputTypes.ShortText, Tools.ParameterField("پورت", "EMailPort", EventInfo.EMailPort, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EnableSsl", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("فعال کردن SSL", "EnableSsl", EventInfo.EnableSsl.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ReceivingUsers", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingUsers", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "کاربران دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.ReceivingUsers, DataList = AllUserList }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ReceivingRole", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingRole", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "نقش کاربر دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.ReceivingRole, DataList = AllPostList }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "InsertingUser", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("کاربر درج کننده", "InsertingUser", EventInfo.InsertingUser, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "ReceivingQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری دریافت کنندگان", "ReceivingQuery", new string[] { EventInfo.ReceivingQuery, Tools.ConvertToSQLQuery(EventInfo.ReceivingQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "SendAttachmentFile", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ارسال پیوست", "SendAttachmentFile", EventInfo.SendAttachmentFile, CoreDefine.InputTypes.TwoValues.ToString())));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "SendReport", CoreDefine.InputTypes.MultiSelect, new { FieldName = "SendReport", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "ارسال گزارش", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.SendReport, DataList = ReportList }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "Title", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان", "Title", EventInfo.Title, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "TitleQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری عنوان", "TitleQuery", new string[] { EventInfo.TitleQuery, Tools.ConvertToSQLQuery(EventInfo.TitleQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "BodyMessage", CoreDefine.InputTypes.LongText, Tools.ParameterField("متن", "BodyMessage", EventInfo.BodyMessage, CoreDefine.InputTypes.LongText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "BodyMessageQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری متن", "BodyMessageQuery", new string[] { EventInfo.BodyMessageQuery, Tools.ConvertToSQLQuery(EventInfo.BodyMessageQuery) }, CoreDefine.InputTypes.Query.ToString())));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ارجاع", "ReferralDeadlineResponse", CoreDefine.InputTypes.Number, Tools.ParameterField("مهلت پاسخگویی (روز)", "ReferralDeadlineResponse", EventInfo.ReferralDeadlineResponse.ToString(), CoreDefine.InputTypes.Number.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ارجاع", "ReferralRecipientsUser", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReferralRecipientsUser", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "کاربران دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.ReferralRecipientsUser, DataList = AllUserList }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ارجاع", "ReferralRecipientsRole", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReferralRecipientsRole", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "نقش کاربر دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EventInfo.ReferralRecipientsRole, DataList = AllPostList }));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ارجاع", "ReferralRecipientsQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری دریافت کنندگان", "ReferralRecipientsQuery", new string[] { EventInfo.ReferralRecipientsQuery, Tools.ConvertToSQLQuery(EventInfo.ReferralRecipientsQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ارجاع", "ReferralTitle", CoreDefine.InputTypes.LongText, Tools.ParameterField("دستور ارجاع", "ReferralTitle", EventInfo.ReferralTitle, CoreDefine.InputTypes.LongText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ارجاع", "ReferralTitleQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری دستور ارجاع", "ReferralTitleQuery", new string[] { EventInfo.ReferralTitleQuery, Tools.ConvertToSQLQuery(EventInfo.ReferralTitleQuery) }, CoreDefine.InputTypes.Query.ToString())));

                        break;
                    }
                case "ParameterTableFunction":
                    {
                        ParameterTableFunction TableFunction = new ParameterTableFunction(_CoreObject);
                        List<SelectListItem> FieldNatureItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "هیچکدام", Value = ""},
                                                                      new SelectListItem() {Text = "رشته کوتاه", Value = "Nvarchar(400)"},
                                                                      new SelectListItem() {Text = "رشته طولانی", Value = "Nvarchar(MAX)"},
                                                                      new SelectListItem() {Text = "باینری کوتاه", Value = "Binary(800)"},
                                                                      new SelectListItem() {Text = "باینری طولانی", Value = "Binary(MAX)"},
                                                                      new SelectListItem() {Text = "عدد", Value ="Bigint"},
                                                                      new SelectListItem() {Text ="دو مقدار", Value =  "Bit"},
                                                                      new SelectListItem() {Text ="اعشار", Value =  "Float"},
                                                                      new SelectListItem() {Text ="xml", Value =  "xml"},
                                                                  };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "ParameterDataType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("ماهیت", "ParameterDataType", TableFunction.ParameterDataType, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, FieldNatureItems)));
                        break;
                    }

                case "Process":
                    {
                        ProcessType process = new ProcessType(_CoreObject);

                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", process.Icon, CoreDefine.InputTypes.Icon.ToString(), false, true)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "InformationEntryFormID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فرم نمایشی", "InformationEntryFormID", process.InformationEntryFormID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, true, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ReportID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("گزارش", "ReportID", process.ReportID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, true, CoreDefine.Entities.گزارش.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ParameterReportID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("پارامتر", "ParameterReportID", process.ParameterReportID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پارامتر_گزارش.ToString(), process.ReportID)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "ProcessModel", CoreDefine.InputTypes.BPMSModel, Tools.ParameterField("مدل فرآیند", "ProcessModel", (process.ProcessModelXml == "" ? BPMN.InitiXML : process.ProcessModelXml), CoreDefine.InputTypes.BPMSModel.ToString())));
                        break;
                    }

                case "ProcessStepEvent":
                    {
                        ProcessStepEvent StepEvent;
                        StepEvent = new ProcessStepEvent(_CoreObject);
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Command", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Command", new string[] { StepEvent.Command, Tools.ConvertToSQLQuery(StepEvent.Command) }, CoreDefine.InputTypes.Query.ToString())));
                        break;
                    }
                case "ProcessStep":
                    {
                        ProcessStep Item = new ProcessStep(_CoreObject);
                        FolderList[0].FullName = Item.Name;
                        switch (Item.ActionType)
                        {
                            case CoreDefine.ProcessStepActionType.عملیات:
                                {
                                    FieldParameter.Add(new TemplateField(false, Item.Name, "InformationEntryFormID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("شناسه فرم جدولی", "InformationEntryFormID", Item.InformationEntryFormID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), 0)));
                                    FieldParameter.Add(new TemplateField(false, Item.Name, "RecordType", CoreDefine.InputTypes.RecordType, new { FieldName = "RecordType", InputType = CoreDefine.InputTypes.RecordType.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "وضعیت رکورد", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.RecordType }));
                                    break;
                                }
                            case CoreDefine.ProcessStepActionType.پایان:
                            case CoreDefine.ProcessStepActionType.شروع:
                                {
                                    FieldParameter.Add(new TemplateField(false, Item.Name, "InformationEntryFormID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("شناسه فرم جدولی", "InformationEntryFormID", Item.InformationEntryFormID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), 0)));
                                    FieldParameter.Add(new TemplateField(false, Item.Name, "RecordType", CoreDefine.InputTypes.RecordType, new { FieldName = "RecordType", InputType = CoreDefine.InputTypes.RecordType.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "وضعیت رکورد", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.RecordType }));
                                    break;
                                }
                            case CoreDefine.ProcessStepActionType.SendTask:
                                {
                                    FolderList[0].FullName = "تنظیمات ایمیل - " + FolderList[0].FullName;
                                    List<CoreObject> ParameterCore = CoreObject.FindChilds(CoreDefine.Entities.گزارش);
                                    List<string> ReportList = new List<string>();
                                    foreach (var item in ParameterCore)
                                    {
                                        ReportList.Add(Tools.UnSafeTitle(item.FullName) + "_" + item.CoreObjectID);
                                    }

                                    List<string> AllUserList = Tools.GetAllUserList();
                                    List<string> AllPostList = Tools.GetAllPostList();

                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "UsePublickEmail", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("استفاده از ایمیل پیشفرض", "UsePublickEmail", Item.UsePublickEmail.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "EMail", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ایمیل", "EMail", Item.EMail, CoreDefine.InputTypes.ShortText.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "EMailUserName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربری", "EMailUserName", Item.EMailUserName, CoreDefine.InputTypes.ShortText.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "EMailPassWord", CoreDefine.InputTypes.Password, Tools.ParameterField("کلمه عبور", "EMailPassWord", Item.EMailPassWord, CoreDefine.InputTypes.Password.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "EMailServer", CoreDefine.InputTypes.ShortText, Tools.ParameterField("سرور", "EMailServer", Item.EMailServer, CoreDefine.InputTypes.ShortText.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "EMailPort", CoreDefine.InputTypes.ShortText, Tools.ParameterField("پورت", "EMailPort", Item.EMailPort, CoreDefine.InputTypes.ShortText.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "EnableSsl", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("فعال کردن SSL", "EnableSsl", Item.EnableSsl.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "ReceivingUsers", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingUsers", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "کاربران دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.ReceivingUsers, DataList = AllUserList }));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "ReceivingRole", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingRole", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "نقش کاربر دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.ReceivingRole, DataList = AllPostList }));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "InsertingUser", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("کاربر درج کننده", "InsertingUser", Item.InsertingUser, CoreDefine.InputTypes.TwoValues.ToString())));
                                    FieldParameter.Add(new TemplateField(true, FolderList[0].FullName, "ReceivingQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری دریافت کنندگان", "ReceivingQuery", new string[] { Item.ReceivingQuery, Tools.ConvertToSQLQuery(Item.ReceivingQuery) }, CoreDefine.InputTypes.Query.ToString())));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "SendAttachmentFile", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ارسال پیوست", "SendAttachmentFile", Item.SendAttachmentFile, CoreDefine.InputTypes.TwoValues.ToString())));

                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "SendReport", CoreDefine.InputTypes.MultiSelect, new { FieldName = "SendReport", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "ارسال گزارش", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.SendReport, DataList = ReportList }));
                                    FieldParameter.Add(new TemplateField(false, FolderList[0].FullName, "Title", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان", "Title", Item.Title, CoreDefine.InputTypes.ShortText.ToString())));
                                    FieldParameter.Add(new TemplateField(true, FolderList[0].FullName, "TitleQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری عنوان", "TitleQuery", new string[] { Item.TitleQuery, Tools.ConvertToSQLQuery(Item.TitleQuery) }, CoreDefine.InputTypes.Query.ToString())));
                                    FieldParameter.Add(new TemplateField(true, FolderList[0].FullName, "BodyMessage", CoreDefine.InputTypes.LongText, Tools.ParameterField("متن", "BodyMessage", Item.BodyMessage, CoreDefine.InputTypes.LongText.ToString())));
                                    FieldParameter.Add(new TemplateField(true, FolderList[0].FullName, "BodyMessageQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری متن", "BodyMessageQuery", new string[] { Item.BodyMessageQuery, Tools.ConvertToSQLQuery(Item.BodyMessageQuery) }, CoreDefine.InputTypes.Query.ToString())));
                                    break;
                                }
                        }

                        //FieldParameter.Add(new TemplateField(false, "عمومی", "ProgressPercent", CoreDefine.InputTypes.Number, new { FieldName = "ProgressPercent", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "درصد پیشرفت", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.ProgressPercent.ToString() }));
                        break;
                    }
                case "BpmnLane":
                    {
                        BpmnLane bpmnLane = new BpmnLane(_CoreObject);
                        List<string> PersonelDataList = Tools.GetAllUserList();
                        List<string> OrganizationlevelDataList = Tools.GetAllPostList();
                        ViewData["FormComment"] = "اگر سمت سازمانی گیرنده ارجاع خالی باشد بصورت اتومات به نفر مافوق ارسال می گردد.";
                        FolderList[0].FullName = bpmnLane.Name;
                        FieldParameter.Add(new TemplateField(true, bpmnLane.Name, "Personnel", CoreDefine.InputTypes.MultiSelect, new { FieldName = "Personnel", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "کاربران", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = bpmnLane.Personnel, DataList = PersonelDataList }));
                        FieldParameter.Add(new TemplateField(true, bpmnLane.Name, "Organizationlevel", CoreDefine.InputTypes.MultiSelect, new { FieldName = "Organizationlevel", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "سمت سازمانی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = bpmnLane.OrganizationLevel, DataList = OrganizationlevelDataList }));
                        FieldParameter.Add(new TemplateField(true, bpmnLane.Name, "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری ارسال/دریافت ارجاع", "Query", new string[] { bpmnLane.Query, Tools.ConvertToSQLQuery(bpmnLane.Query) }, CoreDefine.InputTypes.Query.ToString())));

                        break;
                    }
                case "BpmnOutgoin":
                    {
                        BpmnSequenceFlow Item = new BpmnSequenceFlow(_CoreObject);
                        FolderList[0].FullName = Item.Name;
                        FieldParameter.Add(new TemplateField(true, Item.Name, "ConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط", "ConditionQuery", new string[] { Item.ConditionQuery, Tools.ConvertToSQLQuery(Item.ConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        break;
                    }
                case "BpmnIncoming":
                    {
                        BpmnSequenceFlow Item = new BpmnSequenceFlow(_CoreObject);
                        FolderList[0].FullName = Item.Name;
                        FieldParameter.Add(new TemplateField(true, Item.Name, "ConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط", "ConditionQuery", new string[] { Item.ConditionQuery, Tools.ConvertToSQLQuery(Item.ConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        break;
                    }
                case "Report":
                    {
                        Report Report = new Report(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", Report.Icon, CoreDefine.InputTypes.Icon.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "UseDefualtIconColor", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("استفاده از رنگ پیشفرض", "UseDefualtIconColor", Report.UseDefualtIconColor, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IconColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ آیکن", "IconColor", Report.IconColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DataSourceID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("منبع پایگاه داده", "DataSourceID", Report.DataSourceID, CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInMainMenu", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش در منوی اصلی", "ShowInMainMenu", Report.ShowInMainMenu, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowParameterInload", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش پارامترها هنگام لود", "ShowParameterInload", Report.ShowParameterInload, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SelectedRow", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("انتخاب ردیف", "SelectedRow", Report.SelectedRow, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "PrinterName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام پرینتر", "PrinterName", Report.PrinterName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "PrintCopy", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد کپی", "PrintCopy", Report.PrintCopy, CoreDefine.InputTypes.Number.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "QueryPrintCopy", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری تعداد کپی", "QueryPrintCopy", new string[] { Report.QueryPrintCopy, Tools.ConvertToSQLQuery(Report.QueryPrintCopy) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "QueryAffterPrint", CoreDefine.InputTypes.Query, Tools.ParameterField("دستور بعد از چاپ", "QueryAffterPrint", new string[] { Report.QueryAffterPrint, Tools.ConvertToSQLQuery(Report.QueryAffterPrint) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "QueryBeforRun", CoreDefine.InputTypes.Query, Tools.ParameterField("دستور قبل از اجرای گزارش", "QueryBeforRun", new string[] { Report.QueryBeforRun, Tools.ConvertToSQLQuery(Report.QueryBeforRun) }, CoreDefine.InputTypes.Query.ToString())));

                        break;
                    }

                case "ReportParameter":
                    {
                        ReportParameter Parameter = new ReportParameter(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "InputTypes", CoreDefine.InputTypes.ComboBox, new { FieldName = "InputTypes", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.InputTypes, ComboItems = Tools.FieldTypeList() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedTable", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedTable", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "جدول مرتبط", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.RelatedTable.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedField", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedField", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فیلد مرتبط", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.RelatedField.ToString(), Entitiy = CoreDefine.Entities.پارامتر_گزارش.ToString(), ParentID = _CoreObject.ParentID.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedFieldCommand", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedFieldCommand", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "شرط فیلد مرتبط", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.RelatedFieldCommand.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Value", CoreDefine.InputTypes.ShortText, new { FieldName = "Value", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "مقدار پیشفرض", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.Value }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SpecialValue", CoreDefine.InputTypes.LongText, Tools.ParameterField("مقدار ویژه", "SpecialValue", Parameter.SpecialValue, CoreDefine.InputTypes.LongText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ViewCommand", CoreDefine.InputTypes.LongText, new { FieldName = "ViewCommand", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "دستور نمایشی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.ViewCommand }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DigitsAfterDecimal", CoreDefine.InputTypes.Number, new { FieldName = "DigitsAfterDecimal", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "تعداد رقم بعد اعشار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.DigitsAfterDecimal.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsLeftWrite", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsLeftWrite", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "چپ چین", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.IsLeftWrite }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsEditAble", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsEditAble", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "قابل ویرایش", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.IsEditAble }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ActiveOnKeyDown", CoreDefine.InputTypes.TwoValues, new { FieldName = "ActiveOnKeyDown", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فعال کردن KeyDown", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Parameter.ActiveOnKeyDown }));

                        break;
                    }

                case "TableAttachment":
                    {
                        TableAttachment Attachment = new TableAttachment(_CoreObject);
                        List<string> DataList = new List<string>() { ".pdf", ".docx", ".xlsx", ".zip", ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                        List<SelectListItem> AttachmentUploadSize = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "کوچک", Value = CoreDefine.AttachmentUploadSize.کوچک.ToString()},
                                                                      //new SelectListItem() {Text = "متوسط", Value = CoreDefine.AttachmentUploadSize.متوسط.ToString()},
                                                                      new SelectListItem() {Text = "بزرگ", Value = CoreDefine.AttachmentUploadSize.بزرگ.ToString()}
                                                                  };


                        List<SelectListItem> ColumnWidthItem = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "0-5", Value = "0-5"},
                                                                      new SelectListItem() {Text = "1", Value = "1"},
                                                                      new SelectListItem() {Text = "1-25", Value = "1-25"},
                                                                      new SelectListItem() {Text = "1-5", Value = "1-5"},
                                                                      new SelectListItem() {Text = "2", Value = "2"},
                                                                      new SelectListItem() {Text = "3", Value ="3"},
                                                                      new SelectListItem() {Text = "4", Value = "4"},
                                                                      new SelectListItem() {Text = "5", Value = "5"},
                                                                      new SelectListItem() {Text = "6", Value = "6"},
                                                                      new SelectListItem() {Text = "7", Value = "7"},
                                                                      new SelectListItem() {Text = "8", Value = "8"},
                                                                      new SelectListItem() {Text = "9", Value = "9"},
                                                                      new SelectListItem() {Text = "10", Value = "10"},
                                                                      new SelectListItem() {Text = "11", Value = "11"},
                                                                      new SelectListItem() {Text = "12", Value = "12"}
                                                                  };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "AttachmentUploadType", CoreDefine.InputTypes.AttachmentUploadType, new { FieldName = "AttachmentUploadType", InputType = CoreDefine.InputTypes.AttachmentUploadType.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "نوع بارگذاری ضمیمه", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Attachment.AttachmentUploadType }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "AttachmentUploadSize", CoreDefine.InputTypes.ComboBox, new { FieldName = "AttachmentUploadSize", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "اندازه ضمیمه", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Attachment.AttachmentUploadSize, ComboItems = AttachmentUploadSize }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ColumnWidth", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("عرض فرم", "ColumnWidth", Attachment.ColumnWidth, CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ColumnWidthItem)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "AllowedExtensions", CoreDefine.InputTypes.MultiSelect, new { FieldName = "AllowedExtensions", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "پسوند مجاز", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Attachment.AllowedExtensions, DataList = DataList }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MaxFileSize", CoreDefine.InputTypes.Number, new { FieldName = "MaxFileSize", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "بیشترین سایز فایل(kb)", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Attachment.MaxFileSize.ToString(), MinValue = "1", MaxValue = long.MaxValue.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MinFileSize", CoreDefine.InputTypes.Number, new { FieldName = "MinFileSize", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "کمترین سایز فایل(kb)", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Attachment.MinFileSize.ToString(), MinValue = "1", MaxValue = long.MaxValue.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsRequired", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ضروری", "IsRequired", Attachment.IsRequired, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowDefault", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش پیشفرض", "ShowDefault", Attachment.ShowDefault, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SaveInDatabase", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ذخیره در پایگاه داده", "SaveInDatabase", Attachment.SaveInDatabase, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "AutoFillQuery", CoreDefine.InputTypes.Query, new { FieldName = "AutoFillQuery", InputType = CoreDefine.InputTypes.Query.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "دستور پر کردن خودکار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = new string[] { Attachment.AutoFillQuery, Tools.ConvertToSQLQuery(Attachment.AutoFillQuery) } }));
                        break;
                    }

                case "GridRowColor":
                    {
                        GridRowColor EntryForm = new GridRowColor(_CoreObject);
                        CoreObject ParentCore = CoreObject.Find(_CoreObject.ParentID);
                        long RelatedTable = 0;
                        switch (ParentCore.Entity)
                        {
                            case CoreDefine.Entities.فرم_ورود_اطلاعات:
                                { 
                                    InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(_CoreObject.ParentID));
                                    RelatedTable= informationEntryForm.RelatedTable;
                                    break;
                                }
                        }

                        List<SelectListItem> ColorItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "سبز", Value = "Valid-row"},
                                                                      new SelectListItem() {Text = "قرمز", Value ="Error-row"},
                                                                      new SelectListItem() {Text = "آبی", Value = "Info-row"},
                                                                      new SelectListItem() {Text = "زرد", Value = "Yellow-row"},
                                                                      new SelectListItem() {Text = "بنفش", Value = "Purple-row"},
                                                                      new SelectListItem() {Text = "صورتی", Value = "pink-row"},
                                                                      new SelectListItem() {Text = "قهوه ای", Value = "Brown-row"},
                                                                      new SelectListItem() {Text = "نارنجی", Value = "Orange-row"},
                                                                  };

                        List<SelectListItem> FieldRowColorOperatorItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = ">", Value = ">"},
                                                                      new SelectListItem() {Text = "<", Value = "<"},
                                                                      new SelectListItem() {Text = "=", Value = "="},
                                                                      new SelectListItem() {Text = "!=", Value = "!="},
                                                                      new SelectListItem() {Text = ">=", Value = ">="},
                                                                      new SelectListItem() {Text = "<=", Value = "<="},
                                                                  };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorColumnFullName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کامل ستون", "RowColorColumnFullName", EntryForm.RowColorColumnFullName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorColumnName", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("نام ستون", "RowColorColumnName", EntryForm.RowColorColumnName, CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.فیلد.ToString(), RelatedTable)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorOperator", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع عملگرا", "RowColorOperator", EntryForm.RowColorOperator, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, FieldRowColorOperatorItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorColumnValue", CoreDefine.InputTypes.ShortText, Tools.ParameterField("مقدار ستون", "RowColorColumnValue", EntryForm.RowColorColumnValue, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorOperator2", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("2نوع عملگرا", "RowColorOperator2", EntryForm.RowColorOperator2, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, FieldRowColorOperatorItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorColumnValue2", CoreDefine.InputTypes.ShortText, Tools.ParameterField("2مقدار ستون", "RowColorColumnValue2", EntryForm.RowColorColumnValue2, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowColorSelectedColor", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("رنگ سطر", "RowColorSelectedColor", EntryForm.RowColorSelectedColor, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, ColorItems)));

                        break;
                    }
                case "InformationEntryForm":
                    {
                        InformationEntryForm EntryForm = new InformationEntryForm(_CoreObject);

                        List<SelectListItem> OpenFormTypeItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "جدولی", Value = CoreDefine.OpenFormType.Grid.ToString()}, 
                                                                      new SelectListItem() {Text = "فرم جدید", Value = CoreDefine.OpenFormType.NewForm.ToString()}, 
                                                                  };
                        List<SelectListItem> FieldNatureItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "پنجره ای", Value = GridEditMode.PopUp.ToString()},
                                                                      new SelectListItem() {Text = "ردیفی", Value = GridEditMode.InLine.ToString()},
                                                                      new SelectListItem() {Text = "سلولی", Value = GridEditMode.InCell.ToString()}
                                                                  };

                        List<SelectListItem> ColorItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "سبز", Value = "Valid-row"},
                                                                      new SelectListItem() {Text = "قرمز", Value ="Error-row"},
                                                                      new SelectListItem() {Text = "آبی", Value = "Info"}
                                                                  };

                        List<SelectListItem> FieldRowColorOperatorItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = ">", Value = ">"},
                                                                      new SelectListItem() {Text = "<", Value = "<"},
                                                                      new SelectListItem() {Text = "=", Value = "="}
                                                                  };



                        List<SelectListItem> ColumnWidthItem = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "1", Value = "1"},
                                                                      new SelectListItem() {Text = "2", Value = "2"},
                                                                      new SelectListItem() {Text = "3", Value ="3"},
                                                                      new SelectListItem() {Text = "4", Value = "4"},
                                                                      new SelectListItem() {Text = "5", Value = "5"},
                                                                      new SelectListItem() {Text = "6", Value = "6"},
                                                                      new SelectListItem() {Text = "7", Value = "7"},
                                                                      new SelectListItem() {Text = "8", Value = "8"},
                                                                      new SelectListItem() {Text = "9", Value = "9"},
                                                                      new SelectListItem() {Text = "10", Value = "10"},
                                                                      new SelectListItem() {Text = "11", Value = "11"},
                                                                      new SelectListItem() {Text = "12", Value = "12"} 
                                                                  };
                         

                        FieldParameter.Add(new TemplateField(false, "عمومی", "OpenFormType", CoreDefine.InputTypes.ComboBox, new { FieldName = "OpenFormType", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع نمایش فرم", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.OpenFormType, ComboItems = OpenFormTypeItems }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "GridEditMode", CoreDefine.InputTypes.ComboBox, new { FieldName = "GridEditMode", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع درج", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.GridEditMode, ComboItems = FieldNatureItems }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedTable", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedTable", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "جدول مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.RelatedTable.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ExternalField", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "ExternalField", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فیلد مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.ExternalField.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "GroupableField", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "GroupableField", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فیلد گروهبندی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.GroupableField.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowRecordCountDefault", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد نمایش رکورد", "ShowRecordCountDefault", EntryForm.ShowRecordCountDefault, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Height", CoreDefine.InputTypes.Number, Tools.ParameterField("طول فرم", "Height", EntryForm.Height, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false))); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ColumnWidth", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("عرض فرم", "ColumnWidth", EntryForm.ColumnWidth, CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ColumnWidthItem))); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FormComment", CoreDefine.InputTypes.ShortText, Tools.ParameterField("توضیحات فرم", "FormComment", EntryForm.FormComment, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "TitleSaveButton", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان دکمه ذخیره", "TitleSaveButton", EntryForm.TitleSaveButton, CoreDefine.InputTypes.ShortText.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", EntryForm.Icon, CoreDefine.InputTypes.Icon.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsShowID", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش شناسه", "IsShowID", EntryForm.IsShowID, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SaveAtOnce", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ذخیره بصورت یکجا", "SaveAtOnce", EntryForm.SaveAtOnce, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SaveParentSubjectSaveChild", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ذخیره والد مشروط ذخیره فرزند", "SaveParentSubjectSaveChild", EntryForm.SaveParentSubjectSaveChild, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "CheckValidChildGrid", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("بررسی فرزند قبل از ذخیره", "CheckValidChildGrid", EntryForm.CheckValidChildGrid, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInParentForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش در فرم والد", "ShowInParentForm", EntryForm.ShowInParentForm, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInMenueTreeList", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش در لیست منو", "ShowInMenueTreeList", EntryForm.ShowInMenueTreeList, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Pageable", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("قابلیت صفحه بندی", "Pageable", EntryForm.Pageable, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowCoutnInPage", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد ردیف در هر صفحه", "RowCoutnInPage", EntryForm.RowCoutnInPage, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Groupable", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("قابلیت گروهبندی", "Groupable", EntryForm.Groupable, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Aggregatesable", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش زیر صفحه", "Aggregatesable", EntryForm.Aggregatesable, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowSelectedColumn", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ستون انتخاب", "ShowSelectedColumn", EntryForm.ShowSelectedColumn, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SearchWithOnkeyDown", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("جستجو با onKeyDown", "SearchWithOnkeyDown", EntryForm.SearchWithOnkeyDown, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SearchWithOnkeyDownCoreId", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "SearchWithOnkeyDownCoreId", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فرم جستجو مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.SearchWithOnkeyDownCoreId.ToString(), Entitiy = CoreDefine.Entities.فرم_جستجو.ToString(), ParentID = _CoreObject.CoreObjectID.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowLineNumber", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ستون ردیف", "ShowLineNumber", EntryForm.ShowLineNumber, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsCloseFormAffterSave", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("بستن فرم بعد از ذخیره", "IsCloseFormAffterSave", EntryForm.IsCloseFormAffterSave, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowNewButton", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش دکمه جدید در فرم", "ShowNewButton", EntryForm.ShowNewButton, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowClearFormWhithOutFixItemButton", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش دکمه پاک کردن فرم به استثناء", "ShowClearFormWhithOutFixItemButton", EntryForm.ShowClearFormWhithOutFixItemButton, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ClearFormWhithOutFixItemButtonName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان دکمه پاک کردن فرم به استثناء", "ClearFormWhithOutFixItemButtonName", EntryForm.ClearFormWhithOutFixItemButtonName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "FixItemClearForm", CoreDefine.InputTypes.ShortText, Tools.ParameterField("فیلد های استثناء پاک کردن", "FixItemClearForm", EntryForm.FixItemClearForm, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "DefualtColumnShowInGrid", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون پیشفرض نمایش گرید", "DefualtColumnShowInGrid", EntryForm.DefualtColumnShowInGrid, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "CheckFieldDuplicateRecords", CoreDefine.InputTypes.ShortText, Tools.ParameterField("فیلد های عدم تکرار در ایجاد ردیف", "CheckFieldDuplicateRecords", EntryForm.CheckFieldDuplicateRecords, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "NewPageUrl", CoreDefine.InputTypes.ShortText, Tools.ParameterField("صفحه دکمه جدید", "NewPageUrl", EntryForm.NewPageUrl, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "UpdatePageUrl", CoreDefine.InputTypes.ShortText, Tools.ParameterField("صفحه دکمه ویرایش", "UpdatePageUrl", EntryForm.UpdatePageUrl, CoreDefine.InputTypes.ShortText.ToString())));
                        
                        
                        FieldParameter.Add(new TemplateField(true, "عمومی", "EditorFormJson", CoreDefine.InputTypes.ViewerForm, Tools.ParameterField("فرم طراحی شده", "EditorFormJson", EntryForm.EditorFormJson, CoreDefine.InputTypes.ViewerForm.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "NewButtonVisibleConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری نمایش دکمه جدید", "NewButtonVisibleConditionQuery", new string[] { EntryForm.NewButtonVisibleConditionQuery, Tools.ConvertToSQLQuery(EntryForm.NewButtonVisibleConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { EntryForm.Query, Tools.ConvertToSQLQuery(EntryForm.Query) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "ConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط کوئری", "ConditionQuery", new string[] { EntryForm.ConditionQuery, Tools.ConvertToSQLQuery(EntryForm.ConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "GroupByQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("گروهبندی کوئری", "GroupByQuery", new string[] { EntryForm.GroupByQuery, Tools.ConvertToSQLQuery(EntryForm.GroupByQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "OrderQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("ترتیب کوئری", "OrderQuery", new string[] { EntryForm.OrderQuery, Tools.ConvertToSQLQuery(EntryForm.OrderQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "CSS", CoreDefine.InputTypes.LongText, Tools.ParameterField("CSS", "CSS", EntryForm.CSS, CoreDefine.InputTypes.LongText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "AnalystDescription", CoreDefine.InputTypes.LongText, Tools.ParameterField("توضیحات برنامه نویس/تحلیلگر", "AnalystDescription", EntryForm.AnalystDescription, CoreDefine.InputTypes.LongText.ToString())));

                        FolderList.Add(new Folder() { FullName = "ضمیمه" });
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "ShowAttachment", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ضمیمه در فرم", "ShowAttachment", EntryForm.ShowAttachment, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "HieghtAttachment", CoreDefine.InputTypes.Number, Tools.ParameterField("طول ضمیمه", "HieghtAttachment", EntryForm.HieghtAttachment, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "AttachmentColumnWidth", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("عرض ضمیمه", "AttachmentColumnWidth", EntryForm.AttachmentColumnWidth, CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ColumnWidthItem)));
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "ShowAttachementButton", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش دکمه ضمیمه", "ShowAttachementButton", EntryForm.ShowAttachementButton, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "ShowDetailAttachment", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش جزئیات ضمیمه در فرم", "ShowDetailAttachment", EntryForm.ShowDetailAttachment, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "ShowAttachmentColumn", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ستون ضمیمه", "ShowAttachmentColumn", EntryForm.ShowAttachmentColumn, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "ضمیمه", "AttachmentColumnName", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("ستون ضمیمه", "AttachmentColumnName", EntryForm.AttachmentColumnName, CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
 
                        FolderList.Add(new Folder() { FullName = "چارت" });
                        FieldParameter.Add(new TemplateField(false, "چارت", "ShowChartInformationEntryForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش بصورت چارت", "ShowChartInformationEntryForm", EntryForm.ShowChartInformationEntryForm, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "چارت", "ChartID", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون شناسه", "ChartID", EntryForm.ChartID, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "چارت", "ChartName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون نام", "ChartName", EntryForm.ChartName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "چارت", "ChartTitle", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون عنوان", "ChartTitle", EntryForm.ChartTitle, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "چارت", "ChartParentID", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون والد", "ChartParentID", EntryForm.ChartParentID, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "چارت", "ChartGroup", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون گروه", "ChartGroup", EntryForm.ChartGroup, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "چارت", "ChartAvatar", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ستون تصویر", "ChartAvatar", EntryForm.ChartAvatar, CoreDefine.InputTypes.ShortText.ToString())));
 
                        FolderList.Add(new Folder() { FullName = "برچسب" });
                        FieldParameter.Add(new TemplateField(false, "برچسب", "BadgeTitle", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان", "BadgeTitle", EntryForm.BadgeTitle, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "برچسب", "BadgeColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ زمینه", "BadgeColor", EntryForm.BadgeColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(false, "برچسب", "BadgeTextColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ متن", "BadgeTextColor", EntryForm.BadgeTextColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(true, "برچسب", "BadgeQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "BadgeQuery", new string[] { EntryForm.BadgeQuery, Tools.ConvertToSQLQuery(EntryForm.BadgeQuery) }, CoreDefine.InputTypes.Query.ToString())));
 
                        FolderList.Add(new Folder() { FullName = "رنگ سطر" });
                        FieldParameter.Add(new TemplateField(false, "رنگ سطر", "RowColorColumnName", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("نام ستون", "RowColorColumnName", EntryForm.RowColorColumnName, CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "رنگ سطر", "RowColorOperator", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع عملگرا", "RowColorOperator", EntryForm.RowColorOperator, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, FieldRowColorOperatorItems)));
                        FieldParameter.Add(new TemplateField(false, "رنگ سطر", "RowColorColumnValue", CoreDefine.InputTypes.ShortText, Tools.ParameterField("مقدار ستون", "RowColorColumnValue", EntryForm.RowColorColumnValue, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "رنگ سطر", "RowColorSelectedColor", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("رنگ سطر", "RowColorSelectedColor", EntryForm.RowColorSelectedColor, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, ColorItems)));


                        FolderList.Add(new Folder() { FullName = "دکمه های نوار ابزار" });
                        FieldParameter.Add(new TemplateField(false, "دکمه های نوار ابزار", "ShowNewButtonInToolbar", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دکمه جدید", "ShowNewButtonInToolbar", EntryForm.ShowNewButtonInToolbar, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "دکمه های نوار ابزار", "ShowAttachmentButtonInToolbar", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دکمه ضمیمه", "ShowAttachmentButtonInToolbar", EntryForm.ShowAttachmentButtonInToolbar, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "دکمه های نوار ابزار", "ShowUpdateButtonInToolbar", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دکمه ویرایش", "ShowUpdateButtonInToolbar", EntryForm.ShowUpdateButtonInToolbar, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "دکمه های نوار ابزار", "ShowDeleteButtonInToolbar", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دکمه حذف", "ShowDeleteButtonInToolbar", EntryForm.ShowDeleteButtonInToolbar, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "دکمه های نوار ابزار", "ShowViewButtonInToolbar", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دکمه مشاهده", "ShowViewButtonInToolbar", EntryForm.ShowViewButtonInToolbar, CoreDefine.InputTypes.TwoValues.ToString())));
                        break;
                    }

                case "NewButtonForm":
                    {
                        NewButtonForm EntryForm = new NewButtonForm(_CoreObject); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", EntryForm.Icon, CoreDefine.InputTypes.Icon.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "UseUrl", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("استفاده از لینک", "UseUrl", EntryForm.UseUrl, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedInformationForm", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فرم مرتبط", "RelatedInformationForm", EntryForm.RelatedInformationForm, CoreDefine.InputTypes.CoreRelatedTable.ToString(),false,false, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(),0))); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Url", CoreDefine.InputTypes.ShortText, Tools.ParameterField("آدرس لینک", "Url", EntryForm.Url, CoreDefine.InputTypes.ShortText.ToString())));  
 
                        break;
                    }
                    
                case "SearchForm":
                    {
                        SearchForm EntryForm = new SearchForm(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedTable", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedTable", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "جدول مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.RelatedTable.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, new { FieldName = "Icon", InputType = CoreDefine.InputTypes.Icon.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = false, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "آیکن", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.Icon }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowIcon", CoreDefine.InputTypes.TwoValues, new { FieldName = "ShowIcon", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش آیکن", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.ShowIcon }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowText", CoreDefine.InputTypes.TwoValues, new { FieldName = "ShowText", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش نام", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.ShowText }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "CleareGridAfterSearch", CoreDefine.InputTypes.TwoValues, new { FieldName = "CleareGridAfterSearch", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "خالی کردن جدول بعد از جستجو", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.CleareGridAfterSearch }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { EntryForm.Query, Tools.ConvertToSQLQuery(EntryForm.Query) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "ConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط", "ConditionQuery", new string[] { EntryForm.ConditionQuery, Tools.ConvertToSQLQuery(EntryForm.ConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "CommonConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط مشترک", "CommonConditionQuery", new string[] { EntryForm.CommonConditionQuery, Tools.ConvertToSQLQuery(EntryForm.CommonConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "GroupByQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری گروهبندی", "GroupByQuery", new string[] { EntryForm.GroupByQuery, Tools.ConvertToSQLQuery(EntryForm.GroupByQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "SelectedColumns", CoreDefine.InputTypes.LongText, Tools.ParameterField("ستون های انتخابی", "SelectedColumns", EntryForm.SelectedColumns, CoreDefine.InputTypes.LongText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "SearchConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری شرط جستجو", "SearchConditionQuery", new string[] { EntryForm.SearchConditionQuery, Tools.ConvertToSQLQuery(EntryForm.SearchConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "SearchAlarmQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری هشدار جستجو", "SearchAlarmQuery", new string[] { EntryForm.SearchAlarmQuery, Tools.ConvertToSQLQuery(EntryForm.SearchAlarmQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        break;
                    }

                case "SearchField":
                    {
                        SearchField EntryForm = new SearchField(_CoreObject);
                        List<SelectListItem> FieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "مساوی", Value = "مساوی"},
                            new SelectListItem() {Text = "نامساوی", Value ="نامساوی"},
                            new SelectListItem() {Text = "بزرگتر یا مساوی", Value ="بزرگتر یا مساوی"},
                            new SelectListItem() {Text = "بزرگتر", Value ="بزرگتر"},
                            new SelectListItem() {Text = "کوچکتر یا مساوی", Value ="کوچکتر یا مساوی"},
                            new SelectListItem() {Text = "کوچکتر", Value ="کوچکتر"},
                            new SelectListItem() {Text = "شروع با", Value ="شروع با"},
                            new SelectListItem() {Text = "شامل", Value ="شامل"},
                            new SelectListItem() {Text = "شامل نباشد", Value ="شامل نباشد"},
                            new SelectListItem() {Text = "پایان با", Value ="پایان با"},
                            new SelectListItem() {Text = "تهی", Value ="تهی"},
                            new SelectListItem() {Text = "تهی نیست", Value ="تهی نیست"},
                            new SelectListItem() {Text = "خالی", Value ="خالی"},
                            new SelectListItem() {Text = "خالی نیست", Value ="خالی نیست"},
                        };

                        //SearchForm SearchForm= new SearchForm(CoreObject.Find(_CoreObject.ParentID));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedField", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedField", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فیلد مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.RelatedField.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DefaultOperator", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("علامتگر پیشفرض", "DefaultOperator", EntryForm.DefaultOperator.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, FieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DefaultValue", CoreDefine.InputTypes.ShortText, new { FieldName = "DefaultValue", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "مقدار پیش فرض", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.DefaultValue }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowField", CoreDefine.InputTypes.TwoValues, new { FieldName = "ShowField", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش فیلد", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.ShowField }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowOperator", CoreDefine.InputTypes.TwoValues, new { FieldName = "ShowOperator", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش مقایسگر", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.ShowOperator }));

                        break;
                    }

                case "ComputationalField":
                    {
                        ComputationalField EntryForm = new ComputationalField(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FieldType", CoreDefine.InputTypes.ComboBox, new { FieldName = "FieldType", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.FieldType, ComboItems = Tools.FieldTypeList() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DisplayName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام نمایشی", "DisplayName", EntryForm.DisplayName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FieldComment", CoreDefine.InputTypes.ShortText, Tools.ParameterField("توضیحات فیلد", "FieldComment", EntryForm.FieldComment, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش در فرم", "ShowInForm", EntryForm.ShowInForm, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsDefaultView", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش پیشفرض", "IsDefaultView", EntryForm.IsDefaultView, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsWide", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش عریض", "IsWide", EntryForm.IsWide, CoreDefine.InputTypes.TwoValues.ToString())));

                        FieldParameter.Add(new TemplateField(false, "عمومی", "DigitsAfterDecimal", CoreDefine.InputTypes.Number, new { FieldName = "DigitsAfterDecimal", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "تعداد رقم بعد اعشار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = EntryForm.DigitsAfterDecimal.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MaxValue", CoreDefine.InputTypes.Number, new { FieldName = "MaxValue", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "بیشترین مقدار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = (float.MaxValue == EntryForm.MaxValue ? "0" : EntryForm.MaxValue.ToString()) }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MinValue", CoreDefine.InputTypes.Number, new { FieldName = "MinValue", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "کمترین مقدار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = (float.MinValue == EntryForm.MinValue ? "0" : EntryForm.MinValue.ToString()) }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, new { FieldName = "Query", InputType = CoreDefine.InputTypes.Query.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "کوئری", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = new string[] { EntryForm.Query, Tools.ConvertToSQLQuery(EntryForm.Query) } }));

                        break;
                    }

                case "Dashboard":
                    {
                        Dashboard DashboardForm = new Dashboard(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "InformationEntryForm", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "InformationEntryForm", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فرم ورود اطلاعات مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DashboardForm.InformationEntryForm.ToString(), Entitiy = CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowDate", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش تاریخ", "ShowDate", DashboardForm.ShowDate, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "StartDate", CoreDefine.InputTypes.ShortText, new { FieldName = "StartDate", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "تاریخ شروع", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DashboardForm.StartDate }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "EndDate", CoreDefine.InputTypes.ShortText, new { FieldName = "EndDate", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "تاریخ پایان", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DashboardForm.EndDate }));
                        break;
                    }

                case "SubDashboard":
                    {
                        CoreObject DashboardCore = CoreObject.Find(_CoreObject.ParentID);
                        SubDashboard SubDashboardForm = new SubDashboard(_CoreObject);
                        while (DashboardCore.Entity != CoreDefine.Entities.داشبورد)
                        {
                            SubDashboard SubDashboardForm2 = new SubDashboard(DashboardCore);
                            SubDashboardForm.DateField = SubDashboardForm.DateField == 0 ? SubDashboardForm2.DateField : SubDashboardForm.DateField;
                            SubDashboardForm.GroupField = SubDashboardForm.GroupField == 0 ? SubDashboardForm2.GroupField : SubDashboardForm.GroupField;

                            DashboardCore = CoreObject.Find(DashboardCore.ParentID);
                        }
                        Dashboard Dashboard = new Dashboard(DashboardCore);
                        InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(Dashboard.InformationEntryForm));

                        List<SelectListItem> ChartGroupDateFieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.ChartGroupDate.هیچکدام.ToString()},
                            new SelectListItem() {Text = "سال", Value = CoreDefine.ChartGroupDate.سال.ToString()},
                            new SelectListItem() {Text = "ماه", Value = CoreDefine.ChartGroupDate.ماه.ToString()},
                            new SelectListItem() {Text = "روز", Value = CoreDefine.ChartGroupDate.روز.ToString()},
                        };

                        List<SelectListItem> ChartCalculationTypeFieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.ChartCalculationType.هیچکدام.ToString()},
                            new SelectListItem() {Text = "تعداد", Value = CoreDefine.ChartCalculationType.تعداد.ToString()},
                            new SelectListItem() {Text = "مجموع", Value = CoreDefine.ChartCalculationType.مجموع.ToString()},
                        };

                        List<SelectListItem> ChartThemeItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "پیشفرض", Value = "default"},
                            new SelectListItem() {Text = "آبی", Value = "blueopal"},
                            new SelectListItem() {Text = "fiori", Value = "fiori"},
                            new SelectListItem() {Text = "flat", Value = "flat"},
                            new SelectListItem() {Text = "highcontrast", Value = "highcontrast"},
                            new SelectListItem() {Text = "material", Value = "material"},
                            new SelectListItem() {Text = "materialblack", Value = "materialblack"},
                            new SelectListItem() {Text = "metro", Value = "metro"},
                            new SelectListItem() {Text = "metroblack", Value = "metroblack"}, 
                            new SelectListItem() {Text = "nova", Value = "nova"},
                            new SelectListItem() {Text = "office365", Value = "office365"},
                            new SelectListItem() {Text = "silver", Value = "silver"},
                            new SelectListItem() {Text = "uniform", Value = "uniform"}
                        };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "ChartTypes", CoreDefine.InputTypes.ComboBox, new { FieldName = "ChartTypes", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع نمودار", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = SubDashboardForm.ChartTypes.ToString(), ComboItems = Tools.ChartTypeList() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Theme", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("تم", "Theme", SubDashboardForm.Theme.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ChartThemeItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", SubDashboardForm.Icon, CoreDefine.InputTypes.Icon.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "BackgroundColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ زمینه", "BackgroundColor", SubDashboardForm.BackgroundColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "TextColor", CoreDefine.InputTypes.Color, Tools.ParameterField("رنگ متن", "TextColor", SubDashboardForm.TextColor, CoreDefine.InputTypes.Color.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsWide", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsWide", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش عریض", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = SubDashboardForm.IsWide }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Title", CoreDefine.InputTypes.ShortText, new { FieldName = "Title", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "عنوان", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = SubDashboardForm.Title }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "InformationEntryForm", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فرم مرتبط", "InformationEntryForm", SubDashboardForm.InformationEntryForm.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "GroupField", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فیلد مرتبط", "GroupField", SubDashboardForm.GroupField.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DateField", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فیلد تاریخ", "DateField", SubDashboardForm.DateField.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ChartGroupDateType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("گروهبندی تاریخ", "ChartGroupDateType", SubDashboardForm.ChartGroupDateType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ChartGroupDateFieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ChartCalculationType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع محاسبه", "ChartCalculationType", SubDashboardForm.ChartCalculationType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ChartCalculationTypeFieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "CalculationField", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فیلد محاسباتی", "CalculationField", SubDashboardForm.CalculationField.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ColumnSpan", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد ستون از 5", "ColumnSpan", SubDashboardForm.ColumnSpan, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RowSpan", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد ردیف", "RowSpan", SubDashboardForm.RowSpan, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ReloadTime", CoreDefine.InputTypes.Number, Tools.ParameterField("زمان بازیابی(دقیقه)", "ReloadTime", SubDashboardForm.ReloadTime, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MaxValue", CoreDefine.InputTypes.Number, Tools.ParameterField("بیشترین مقدار", "MaxValue", SubDashboardForm.MaxValue, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MinValue", CoreDefine.InputTypes.Number, Tools.ParameterField("کمترین مقدار", "MinValue", SubDashboardForm.MinValue, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IndicatorValue", CoreDefine.InputTypes.Number, Tools.ParameterField("شاخص", "IndicatorValue", SubDashboardForm.IndicatorValue, CoreDefine.InputTypes.Number.ToString(), false, true, "", 0, null, false)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Condition", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری شرط", "Condition", new string[] { SubDashboardForm.Condition, Tools.ConvertToSQLQuery(SubDashboardForm.Condition) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "CategoryAxisQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری محور دسته", "CategoryAxisQuery", new string[] { SubDashboardForm.CategoryAxisQuery, Tools.ConvertToSQLQuery(SubDashboardForm.CategoryAxisQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "GroupByQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری گروهبندی", "GroupByQuery", new string[] { SubDashboardForm.GroupByQuery, Tools.ConvertToSQLQuery(SubDashboardForm.GroupByQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "OrderByQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری ترتیب", "OrderByQuery", new string[] { SubDashboardForm.OrderByQuery, Tools.ConvertToSQLQuery(SubDashboardForm.OrderByQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        break;
                    }

                case "DashboardIntegration":
                    {
                        DashboardIntegration DataSourceDashboardForm = new DashboardIntegration(_CoreObject);

                        List<SelectListItem> ChartCalculationTypeFieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.ChartCalculationType.هیچکدام.ToString()},
                            new SelectListItem() {Text = "تعداد", Value = CoreDefine.ChartCalculationType.تعداد.ToString()},
                            new SelectListItem() {Text = "مجموع", Value = CoreDefine.ChartCalculationType.مجموع.ToString()},
                        };

                        FieldParameter.Add(new TemplateField(false, "عمومی", "GroupField", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فیلد مرتبط", "GroupField", DataSourceDashboardForm.GroupField.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ChartCalculationType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع محاسبه", "ChartCalculationType", DataSourceDashboardForm.ChartCalculationType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, ChartCalculationTypeFieldItems)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Condition", CoreDefine.InputTypes.LongText, new { FieldName = "Condition", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "کوئری شرط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DataSourceDashboardForm.Condition }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "CategoryAxisQuery", CoreDefine.InputTypes.LongText, new { FieldName = "CategoryAxisQuery", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "کوئری محور دسته", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DataSourceDashboardForm.CategoryAxisQuery }));
                        break;
                    }

                case "SpecialPhrase":
                    {
                        SpecialPhrase EntryForm = new SpecialPhrase(_CoreObject);
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DataSourceID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("پایگاه داده مرتبط", "DataSourceID", EntryForm.DataSourceID, CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, true, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { EntryForm.Query, Tools.ConvertToSQLQuery(EntryForm.Query) }, CoreDefine.InputTypes.Query.ToString()))); 

                        break;
                    }

                case "Field":
                    {
                        Field Item = new Field(_CoreObject); 
                        List<SelectListItem> ColorItems = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "مشکی", Value = "GridBlackText"},
                                                                      new SelectListItem() {Text = "سبز", Value = "GridGreenText"},
                                                                      new SelectListItem() {Text = "قرمز", Value ="GridRedText"},
                                                                      new SelectListItem() {Text = "آبی", Value = "GridBlueText"},
                                                                      new SelectListItem() {Text = "زرد", Value = "GridYellowText"}
                                                                  };


                        List<SelectListItem> FieldDisplayType = new List<SelectListItem>(){
                                                                      new SelectListItem() {Text = "عمودی", Value = CoreDefine.FieldDisplayType.عمودی.ToString()}, 
                                                                      new SelectListItem() {Text = "افقی", Value = CoreDefine.FieldDisplayType.افقی.ToString()}, 

                        };
                        List<SelectListItem> ColumnWidthItem = new List<SelectListItem>() {
                                                                      new SelectListItem() {Text = "0-5", Value = "0-5"},
                                                                      new SelectListItem() {Text = "1", Value = "1"},
                                                                      new SelectListItem() {Text = "1-25", Value = "1-25"},
                                                                      new SelectListItem() {Text = "1-5", Value = "1-5"},
                                                                      new SelectListItem() {Text = "2", Value = "2"},
                                                                      new SelectListItem() {Text = "3", Value ="3"},
                                                                      new SelectListItem() {Text = "4", Value = "4"},
                                                                      new SelectListItem() {Text = "5", Value = "5"},
                                                                      new SelectListItem() {Text = "6", Value = "6"},
                                                                      new SelectListItem() {Text = "7", Value = "7"},
                                                                      new SelectListItem() {Text = "8", Value = "8"},
                                                                      new SelectListItem() {Text = "9", Value = "9"},
                                                                      new SelectListItem() {Text = "10", Value = "10"},
                                                                      new SelectListItem() {Text = "11", Value = "11"},
                                                                      new SelectListItem() {Text = "12", Value = "12"},
                                                                  };

                         
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FieldType", CoreDefine.InputTypes.ComboBox, new { FieldName = "FieldType", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نوع", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.FieldType.ToString(), ComboItems = Tools.FieldTypeList() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedTable", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedTable", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "جدول مرتبط", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.RelatedTable.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DisplayName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام نمایشی", "DisplayName", Item.DisplayName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FieldComment", CoreDefine.InputTypes.ShortText, Tools.ParameterField("توضیحات فیلد", "FieldComment", Item.FieldComment, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DefaultValue", CoreDefine.InputTypes.ShortText, Tools.ParameterField("مقدار پیشفرض", "DefaultValue", Item.DefaultValue, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FieldDisplayType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع نمایش فیلد", "FieldDisplayType", Item.FieldDisplayType, CoreDefine.InputTypes.ComboBox.ToString(),false,false,"",0, FieldDisplayType)));
 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsEditAble", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsEditAble", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "قابل ویرایش", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.IsEditAble }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsDefaultView", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsDefaultView", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "مشاهده پیشفرض", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.IsDefaultView }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ClearAfterChange", CoreDefine.InputTypes.TwoValues, new { FieldName = "ClearAfterChange", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "پاک کردن فرم بعد از تغییر", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.ClearAfterChange }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsRequired", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsRequired", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "ضروری", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.IsRequired }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsLeftWrite", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsLeftWrite", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "چپ چین", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.IsLeftWrite })); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ColumnWidth", CoreDefine.InputTypes.ComboBox, new { FieldName = "ColumnWidth", InputType = CoreDefine.InputTypes.ComboBox.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "عرض فیلد", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = (Item.IsWide?"12": Item.ColumnWidth), ComboItems = ColumnWidthItem }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsExclusive", CoreDefine.InputTypes.TwoValues, new { FieldName = "IsExclusive", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "منحصر به فرد", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.IsExclusive }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInForm", CoreDefine.InputTypes.TwoValues, new { FieldName = "ShowInForm", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش در فرم", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.ShowInForm }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ActiveOnKeyDown", CoreDefine.InputTypes.TwoValues, new { FieldName = "ActiveOnKeyDown", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فعال کردن KeyDown", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.ActiveOnKeyDown }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SaveAndNewForm", CoreDefine.InputTypes.TwoValues, new { FieldName = "SaveAndNewForm", InputType = CoreDefine.InputTypes.TwoValues.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, FalseValue = "خیر", TrueValue = "بله", IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "ذخیره و فرم جدید", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.SaveAndNewForm })); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SearchAutoCompleteCount", CoreDefine.InputTypes.Number, new { FieldName = "SearchAutoCompleteCount", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "تعداد کارکتر جهت جستجو", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.SearchAutoCompleteCount.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedLink", CoreDefine.InputTypes.ShortText, Tools.ParameterField("لینک مرتبط", "RelatedLink", Item.RelatedLink, CoreDefine.InputTypes.ShortText.ToString())));

                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedField", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedField", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "فیلد مرتبط", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.RelatedField.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedFieldCommand", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedFieldCommand", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "شرط فیلد مرتبط", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.RelatedFieldCommand.ToString(), Entitiy = CoreDefine.Entities.پایگاه_داده.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SpecialValue", CoreDefine.InputTypes.LongText, new { FieldName = "SpecialValue", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "مقدار ویژه", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.SpecialValue }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DigitsAfterDecimal", CoreDefine.InputTypes.Number, new { FieldName = "DigitsAfterDecimal", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "تعداد رقم بعد اعشار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.DigitsAfterDecimal.ToString() }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MaxValue", CoreDefine.InputTypes.Number, new { FieldName = "MaxValue", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "بیشترین مقدار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = (float.MaxValue == Item.MaxValue ? "0" : Item.MaxValue.ToString()) }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MinValue", CoreDefine.InputTypes.Number, new { FieldName = "MinValue", InputType = CoreDefine.InputTypes.Number.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "کمترین مقدار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = (float.MinValue == Item.MinValue ? "0" : Item.MinValue.ToString()) }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "TextColor", CoreDefine.InputTypes.Color, new { FieldName = "TextColor", InputType = CoreDefine.InputTypes.Color.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "رنگ متن", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = Item.TextColor }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "GridTextColor", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("رنگ متن گرید", "GridTextColor", Item.GridTextColor, CoreDefine.InputTypes.ComboBox.ToString(), false, false, "", 0, ColorItems))); 
                        FieldParameter.Add(new TemplateField(true, "عمومی", "ViewCommand", CoreDefine.InputTypes.Query, new { FieldName = "ViewCommand", InputType = CoreDefine.InputTypes.Query.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "دستور نمایشی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = new string[] { Item.ViewCommand, Tools.ConvertToSQLQuery(Item.ViewCommand) } }));                        
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Class", CoreDefine.InputTypes.ShortText, Tools.ParameterField("کلاس", "Class", Item.Class, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "AutoFillQuery", CoreDefine.InputTypes.Query, new { FieldName = "AutoFillQuery", InputType = CoreDefine.InputTypes.Query.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "دستور پر کردن خودکار", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = new string[] { Item.AutoFillQuery, Tools.ConvertToSQLQuery(Item.AutoFillQuery) } }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "AnalystDescription", CoreDefine.InputTypes.LongText, Tools.ParameterField("توضیحات برنامه نویس/تحلیلگر", "AnalystDescription", Item.AnalystDescription, CoreDefine.InputTypes.LongText.ToString())));
                        break;
                    }

                case "ShowFieldEvent":
                    {
                        ShowFieldEvent Item = new ShowFieldEvent(_CoreObject);

                        FieldParameter.Add(new TemplateField(false, "عمومی", "FieldValue", CoreDefine.InputTypes.ShortText, Tools.ParameterField("مقدار فیلد", "FieldValue", Item.FieldValue, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "SelectedObjectID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("انتخابی", "SelectedObjectID", Item.SelectedObjectID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, true, "خالی", 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowObject", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("وضعیت نمایش", "ShowObject", Item.ShowObject, CoreDefine.InputTypes.TwoValues.ToString(), false, true, "", 0, null, false, false, "خیر", "بله")));
                        break;
                    }

                case "DisplayField":
                    {
                        DisplayField DisplayField = new DisplayField(_CoreObject);  
                        List<SelectListItem> FieldItems = new List<SelectListItem>()
                        {
                            new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.TableButtonEventsType.خالی.ToString()},
                            new SelectListItem() {Text = "اجرای رویداد", Value =CoreDefine.TableButtonEventsType.اجرای_رویداد.ToString()},
                            new SelectListItem() {Text = "اجرای وب سرویس", Value =CoreDefine.TableButtonEventsType.اجرای_وب_سرویس.ToString()},
                            new SelectListItem() {Text = "ارسال ایمیل", Value =CoreDefine.TableButtonEventsType.ارسال_ایمیل.ToString()},
                            new SelectListItem() {Text = "انتقال فایل", Value =CoreDefine.TableButtonEventsType.انتقال_فایل.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم فقط خواندنی", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_فقط_خواندنی.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم به صورت ویرایش", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_به_صورت_ویرایش.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم.ToString()},
                            new SelectListItem() {Text = "نمایش ضمیمه", Value =CoreDefine.TableButtonEventsType.نمایش_ضمیمه.ToString()},
                            new SelectListItem() {Text = "تولید کلید عمومی", Value =CoreDefine.TableButtonEventsType.تولید_کلید_عمومی_مالیاتی.ToString()},
                            new SelectListItem() {Text = "بروز رسانی کالا و خدمت از سازمان امور مالیاتی", Value =CoreDefine.TableButtonEventsType.بروزرسانی_کالا_مالیات.ToString()},
                            new SelectListItem() {Text = "ارسال صورتحساب به سامانه مودیان", Value =CoreDefine.TableButtonEventsType.ارسال_صورتحساب_به_سامانه_مودیان.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم با لینک", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_با_لینک.ToString()},
                            new SelectListItem() {Text = "باز کردن فرم به صورت ویرایش با لینک", Value =CoreDefine.TableButtonEventsType.باز_کردن_فرم_به_صورت_ویرایش_با_لینک.ToString()},
                            new SelectListItem() {Text = "نمایش گزارش", Value =CoreDefine.TableButtonEventsType.نمایش_گزارش.ToString()},
                            new SelectListItem() {Text = "نمایش گزارش", Value =CoreDefine.TableButtonEventsType.پرینت_گزارش.ToString()},
                        };


                        InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(_CoreObject.ParentID));
                        List<CoreObject> ParameterCore = CoreObject.FindChilds(CoreDefine.Entities.پارامتر_گزارش);
                        List<string> ReportList = new List<string>();
                        foreach (var item in ParameterCore)
                        {
                            ReportParameter reportParameter = new ReportParameter(item);
                            if (reportParameter.RelatedTable == informationEntryForm.RelatedTable)
                            {
                                CoreObject ReportCore = CoreObject.Find(item.ParentID);
                                ReportList.Add(Tools.UnSafeTitle(ReportCore.FullName) + "_" + ReportCore.CoreObjectID);
                            }
                        } 

                        FieldParameter.Add(new TemplateField(false, "عمومی", "TableButtonEventsType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع عملیات", "TableButtonEventsType", DisplayField.TableButtonEventsType.ToString(), CoreDefine.InputTypes.ComboBox.ToString(), false, true, "", 0, FieldItems)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ReportID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("گزارش", "ReportID", DisplayField.ReportID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.گزارش.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ParameterReportID", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("پارامتر گزارش", "ParameterReportID", DisplayField.ParameterReportID.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.گزارش.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "DisplayName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام نمایشی", "DisplayName", DisplayField.DisplayName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInStart", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش در ابتدای ستون", "ShowInStart", DisplayField.ShowInStart, CoreDefine.InputTypes.TwoValues.ToString(), false, true, "", 0, null, false, false, "خیر", "بله")));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowTitle", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش عنوان", "ShowTitle", DisplayField.ShowTitle, CoreDefine.InputTypes.TwoValues.ToString(), false, true, "", 0, null, false, false, "خیر", "بله")));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedForm", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فرم مرتبط", "RelatedForm", DisplayField.RelatedForm.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "RelatedField", CoreDefine.InputTypes.CoreRelatedTable, Tools.ParameterField("فیلد مرتبط", "RelatedField", DisplayField.RelatedField.ToString(), CoreDefine.InputTypes.CoreRelatedTable.ToString(), false, false, CoreDefine.Entities.پایگاه_داده.ToString(), 0)));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Icon", CoreDefine.InputTypes.Icon, Tools.ParameterField("آیکن", "Icon", DisplayField.Icon, CoreDefine.InputTypes.Icon.ToString()))); 
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IsReloadGrid", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("بروز رسانی داده", "IsReloadGrid", DisplayField.IsReloadGrid, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "ExecutionConditionQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("شرط اجرای کوئری", "ExecutionConditionQuery", new string[] { DisplayField.ExecutionConditionQuery, Tools.ConvertToSQLQuery(DisplayField.ExecutionConditionQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Query", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری", "Query", new string[] { DisplayField.Query, Tools.ConvertToSQLQuery(DisplayField.Query) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Template", CoreDefine.InputTypes.LongText, new { FieldName = "Template", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "دستور", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DisplayField.Template }));



                        FolderList.Add(new Folder() { FullName = "تنظیمات ایمیل" });
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "UsePublickEmail", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("استفاده از ایمیل پیشفرض", "UsePublickEmail", DisplayField.UsePublickEmail.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMail", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ایمیل", "EMail", DisplayField.EMail, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailUserName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربری", "EMailUserName", DisplayField.EMailUserName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailPassWord", CoreDefine.InputTypes.Password, Tools.ParameterField("کلمه عبور", "EMailPassWord", DisplayField.EMailPassWord, CoreDefine.InputTypes.Password.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailServer", CoreDefine.InputTypes.ShortText, Tools.ParameterField("سرور", "EMailServer", DisplayField.EMailServer, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailPort", CoreDefine.InputTypes.ShortText, Tools.ParameterField("پورت", "EMailPort", DisplayField.EMailPort, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EnableSsl", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("فعال کردن SSL", "EnableSsl", DisplayField.EnableSsl.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ReceivingUsers", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingUsers", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "کاربران دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DisplayField.ReceivingUsers, DataList = Referral.DBData.SelectColumn("Select REPLACE(نام_و_نام_خانوادگی,N'_',N' ') +N'_'+ cast(شناسه as nvarchar(255)) from کاربر ").OfType<string>().ToList() }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ReceivingRole", CoreDefine.InputTypes.MultiSelect, new { FieldName = "ReceivingRole", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "نقش کاربر دریافت کننده", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DisplayField.ReceivingRole, DataList = Referral.DBData.SelectColumn("Select REPLACE(عنوان,N'_',N' ')  +N'_'+  cast(شناسه as nvarchar(255))  from نقش_کاربر ").OfType<string>().ToList() }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "InsertingUser", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("کاربر درج کننده", "InsertingUser", DisplayField.InsertingUser, CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "ReceivingQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری دریافت کنندگان", "ReceivingQuery", new string[] { DisplayField.ReceivingQuery, Tools.ConvertToSQLQuery(DisplayField.ReceivingQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "SendAttachmentFile", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ارسال پیوست", "SendAttachmentFile", DisplayField.SendAttachmentFile, CoreDefine.InputTypes.TwoValues.ToString())));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "SendReport", CoreDefine.InputTypes.MultiSelect, new { FieldName = "SendReport", InputType = CoreDefine.InputTypes.MultiSelect.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsLeftWrite = true, FalseValue = false, TrueValue = false, IsInCellEditMode = false, FieldTitle = "ارسال گزارش", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = DisplayField.SendReport, DataList = ReportList }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "Title", CoreDefine.InputTypes.ShortText, Tools.ParameterField("عنوان", "Title", DisplayField.Title, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "TitleQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری عنوان", "TitleQuery", new string[] { DisplayField.TitleQuery, Tools.ConvertToSQLQuery(DisplayField.TitleQuery) }, CoreDefine.InputTypes.Query.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "BodyMessage", CoreDefine.InputTypes.ShortText, Tools.ParameterField("متن", "BodyMessage", DisplayField.BodyMessage, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "BodyMessageQuery", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری متن", "BodyMessageQuery", new string[] { DisplayField.BodyMessageQuery, Tools.ConvertToSQLQuery(DisplayField.BodyMessageQuery) }, CoreDefine.InputTypes.Query.ToString())));



                        break;
                    }

                case "WebServiceParameter":
                    {
                        WebServiceParameter _WebServiceParameter;
                        if (ID == 0)
                            _WebServiceParameter = new WebServiceParameter();
                        else
                            _WebServiceParameter = new WebServiceParameter(_CoreObject);

                        FieldParameter.Add(new TemplateField(false, "عمومی", "Name", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام پارامتر", "Name", _WebServiceParameter.Name, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "Value", CoreDefine.InputTypes.ShortText, Tools.ParameterField("مقدار پارامتر", "Value", _WebServiceParameter.Value, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ConvertToJsonArr", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("تبدیل به آرایه جیسون", "ConvertToJsonArr", _WebServiceParameter.ConvertToJsonArr.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "QueryValue", CoreDefine.InputTypes.Query, Tools.ParameterField("کوئری مقدار", "QueryValue", new string[] { _WebServiceParameter.QueryValue, Tools.ConvertToSQLQuery(_WebServiceParameter.QueryValue) }, CoreDefine.InputTypes.Query.ToString())));

                        break;
                    }

                case "PaymentSetting":
                    {
                        Payment PaymentForm;
                        List<CoreObject> PaymentCore = Referral.CoreObjects.Where(item => item.Entity == CoreDefine.Entities.تنظیمات_پرداخت).ToList();
                        if (PaymentCore.Count == 0)
                        {
                            PaymentForm = new Payment();
                            string Value = Tools.ToXML(PaymentForm);
                            ID = Referral.DBCore.Insert("CoreObject"
                               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                               , new object[] { 0, CoreDefine.Entities.تنظیمات_پرداخت.ToString(), "", "تنظیمات_پرداخت", 0, 0, Value });
                            SysSetting.SysSettingID = ID;
                        }
                        else
                        {
                            PaymentForm = new Payment(PaymentCore[0]);
                            SysSetting.SysSettingID = PaymentCore[0].CoreObjectID;
                        }
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MerchantID", CoreDefine.InputTypes.ShortText, new { FieldName = "MerchantID", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "شماره اختصاصی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PaymentForm.MerchantID }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "CallBackUrl", CoreDefine.InputTypes.ShortText, new { FieldName = "CallBackUrl", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "نام کاربری", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PaymentForm.CallBackUrl }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "PaymentGatewaytype", CoreDefine.InputTypes.ShortText, new { FieldName = "PaymentGatewaytype", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "کلمه عبور", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PaymentForm.PaymentGatewaytype }));
                        break;
                    }

                case "PublicSetting":
                    {
                        PublicSetting PublicSettingForm;
                        List<CoreObject> PublicSettingCore = Referral.CoreObjects.Where(item => item.Entity == CoreDefine.Entities.تنظیمات_عمومی).ToList();
                        if (PublicSettingCore.Count == 0)
                        {
                            PublicSettingForm = new PublicSetting();
                            string Value = Tools.ToXML(PublicSettingForm);
                            ID = Referral.DBCore.Insert("CoreObject"
                               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                               , new object[] { 0, CoreDefine.Entities.تنظیمات_عمومی.ToString(), "", "تنظیمات_عمومی", 0, 0, Value });
                            SysSetting.SysSettingID = ID;
                        }
                        else
                        {
                            PublicSettingForm = new PublicSetting(PublicSettingCore[0]);
                            SysSetting.SysSettingID = PublicSettingCore[0].CoreObjectID;
                        }

                        FolderList.Add(new Folder() { FullName= "تنظیمات ایمیل" });
                        FolderList.Add(new Folder() { FullName= "تنظیمات پیامک" });
                        FolderList.Add(new Folder() { FullName= "تنظیمات امنیت" });

                        FieldParameter.Add(new TemplateField(false, "عمومی", "CompanyName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام شرکت", "CompanyName", PublicSettingForm.CompanyName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "AppPersianName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام فارسی اپلیکیشن", "AppPersianName", PublicSettingForm.AppPersianName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "AppEnglishName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام انگلیسی اپلیکیشن", "AppEnglishName", PublicSettingForm.AppEnglishName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "WebSite", CoreDefine.InputTypes.ShortText, Tools.ParameterField("وبسایت", "WebSite", PublicSettingForm.WebSite, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "PhoneNumber", CoreDefine.InputTypes.ShortText, Tools.ParameterField("شماره تماس", "PhoneNumber", PublicSettingForm.PhoneNumber, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "E_Maile", CoreDefine.InputTypes.ShortText, Tools.ParameterField("ایمیل", "E_Maile", PublicSettingForm.E_Maile, CoreDefine.InputTypes.ShortText.ToString())));

                        FieldParameter.Add(new TemplateField(false, "عمومی", "BrandSlogan", CoreDefine.InputTypes.ShortText, new { FieldName = "BrandSlogan", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "شعار برند", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.BrandSlogan }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "MainColor", CoreDefine.InputTypes.Color, new { FieldName = "MainColor", InputType = CoreDefine.InputTypes.Color.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "رنگ پس زمینه", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.MainColor }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IconColor", CoreDefine.InputTypes.Color, new { FieldName = "IconColor", InputType = CoreDefine.InputTypes.Color.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "رنگ آیکن", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.IconColor }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "TitleColor", CoreDefine.InputTypes.Color, new { FieldName = "TitleColor", InputType = CoreDefine.InputTypes.Color.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "رنگ حروف", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.TitleColor }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "FileSavingPath", CoreDefine.InputTypes.ShortText, new { FieldName = "FileSavingPath", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "مسیر ذخیره ی ضمائم", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.FileSavingPath }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "AppLogo", CoreDefine.InputTypes.Image, new { FieldName = "AppLogo", InputType = CoreDefine.InputTypes.Image.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "لوگوی شرکت", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.AppLogo }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "LoginBackgroundImage", CoreDefine.InputTypes.Image, new { FieldName = "LoginBackgroundImage", InputType = CoreDefine.InputTypes.Image.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "تصویر زمینه ورودی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.LoginBackgroundImage }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "HomeBackgroundImage", CoreDefine.InputTypes.Image, new { FieldName = "HomeBackgroundImage", InputType = CoreDefine.InputTypes.Image.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "تصویر زمینه خانه", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.HomeBackgroundImage }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "IntroduceOthers", CoreDefine.InputTypes.ShortText, new { FieldName = "IntroduceOthers", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "معرفی به دیگران", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.IntroduceOthers }));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "UserPrivacy", CoreDefine.InputTypes.LongText, new { FieldName = "UserPrivacy", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "حریم خصوصی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.UserPrivacy }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "GeneralMessage", CoreDefine.InputTypes.ShortText, new { FieldName = "GeneralMessage", InputType = CoreDefine.InputTypes.ShortText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "پیغام عمومی", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.GeneralMessage }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "Laws", CoreDefine.InputTypes.LongText, new { FieldName = "Laws", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "قوانین", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.Laws }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "CSS", CoreDefine.InputTypes.LongText, new { FieldName = "CSS", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "CSS", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.CSS }));
                        FieldParameter.Add(new TemplateField(true, "عمومی", "JS", CoreDefine.InputTypes.LongText, new { FieldName = "JS", InputType = CoreDefine.InputTypes.LongText.ToString(), IsReadonly = false, IsRequired = true, IsGridField = false, IsInCellEditMode = false, IsLeftWrite = true, FieldTitle = "JS", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.JS }));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMaileUserName", CoreDefine.InputTypes.ShortText, Tools.ParameterField("نام کاربری", "EMaileUserName", PublicSettingForm.EMaileUserName, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailePassWord", CoreDefine.InputTypes.Password, Tools.ParameterField("کلمه عبور", "EMailePassWord", PublicSettingForm.EMailePassWord, CoreDefine.InputTypes.Password.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMaileServer", CoreDefine.InputTypes.ShortText, Tools.ParameterField("سرور", "EMaileServer", PublicSettingForm.EMaileServer, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EMailePort", CoreDefine.InputTypes.ShortText, Tools.ParameterField("پورت", "EMailePort", PublicSettingForm.EMailePort, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "EnableSsl", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("فعال کردن SSL", "EnableSsl", PublicSettingForm.EnableSsl.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات پیامک", "RelatedWebService", CoreDefine.InputTypes.CoreRelatedTable, new { FieldName = "RelatedWebService", InputType = CoreDefine.InputTypes.CoreRelatedTable.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "وب سرویس مرتبط", DigitsAfterDecimal = 0, RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = PublicSettingForm.RelatedWebService.ToString(), Entitiy = CoreDefine.Entities.وب_سرویس.ToString(), ParentID = "0" }));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات امنیت", "ChangePasswordDays", CoreDefine.InputTypes.Number, Tools.ParameterField("تعداد روزهای تغییر کلمه عبور", "ChangePasswordDays", PublicSettingForm.ChangePasswordDays, CoreDefine.InputTypes.Number.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات امنیت", "SupportLink", CoreDefine.InputTypes.ShortText, Tools.ParameterField("لینک پشتیبان", "SupportLink", PublicSettingForm.SupportLink, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات امنیت", "DataRecoveryMinutes", CoreDefine.InputTypes.Number, Tools.ParameterField("زمان بازیابی داده ها(دقیقه)", "DataRecoveryMinutes", PublicSettingForm.DataRecoveryMinutes, CoreDefine.InputTypes.Number.ToString())));

                        break;
                    }

                case "AdminSetting":
                    {
                        List<CoreObject> AdminSettingCore = Referral.CoreObjects.Where(item => item.Entity == CoreDefine.Entities.تنظیمات_مدیر_سیستم).ToList();
                        AdminSetting AdminSetting = new AdminSetting(AdminSettingCore[0]);
                        SysSetting.SysSettingID = AdminSettingCore[0].CoreObjectID;

                        FolderList.Add(new Folder() { FullName= "تنظیمات سیستم" });
                        FolderList.Add(new Folder() { FullName= "تنظیمات ویرایش" });
                        FolderList.Add(new Folder() { FullName= "تنظیمات import/export" });
                        FolderList.Add(new Folder() { FullName= "تنظیمات یادداشت" });
                        FolderList.Add(new Folder() { FullName= "تنظیمات ایمیل" });

                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowUserRegistryInLoginForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ثبت نام در فرم ورود", "ShowUserRegistryInLoginForm", AdminSetting.ShowUserRegistryInLoginForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowUserCalendar", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش تقویم برنامه ریزی", "ShowUserCalendar", AdminSetting.ShowUserCalendar.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowInfoInLogin", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش اطلاعات در فرم ورود", "ShowInfoInLogin", AdminSetting.ShowInfoInLogin.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ShowAllRights", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش کلیه حقوق", "ShowAllRights", AdminSetting.ShowAllRights.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowDataSourceListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش پایگاه داده", "ShowDataSourceListInSettingForm", AdminSetting.ShowDataSourceListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowSpecialPhraseListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش عبارت ویژه", "ShowSpecialPhraseListInSettingForm", AdminSetting.ShowSpecialPhraseListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowInformationEntryFormListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش فرم ساز", "ShowInformationEntryFormListInSettingForm", AdminSetting.ShowInformationEntryFormListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowProcessListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش فرآیند ساز", "ShowProcessListInSettingForm", AdminSetting.ShowProcessListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowReportListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش گزارش ساز", "ShowReportListInSettingForm", AdminSetting.ShowReportListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowDashboardListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش داشبورد ساز", "ShowDashboardListInSettingForm", AdminSetting.ShowDashboardListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowPublicFileListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش فایل های عمومی", "ShowPublicFileListInSettingForm", AdminSetting.ShowPublicFileListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowConnectWebsiteListInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ارتباط با وب سایت", "ShowConnectWebsiteListInSettingForm", AdminSetting.ShowConnectWebsiteListInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowSMSSettingInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ارتباط با پیامک", "ShowSMSSettingInSettingForm", AdminSetting.ShowSMSSettingInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowEmailSettingInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ارتباط با ایمیل", "ShowEmailSettingInSettingForm", AdminSetting.ShowEmailSettingInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowPaymentSettingInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ارتباط با درگاه پرداخت", "ShowPaymentSettingInSettingForm", AdminSetting.ShowPaymentSettingInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات سیستم", "ShowPublicSettingInSettingForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش تنظیمات عمومی", "ShowPublicSettingInSettingForm", AdminSetting.ShowPublicSettingInSettingForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات ویرایش", "ShowEditingRestrictions", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش محدودیت های ویرایش", "ShowEditingRestrictions", AdminSetting.ShowEditingRestrictions.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ویرایش", "ShowCanUpdateOnlyUserRegistry", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ویرایش محدود به کاربر درج کننده", "ShowCanUpdateOnlyUserRegistry", AdminSetting.ShowCanUpdateOnlyUserRegistry.ToString(), CoreDefine.InputTypes.TwoValues.ToString()))); 
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ویرایش", "ShowCanUpdateOneDey", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ویرایش به مدت یک روز", "ShowCanUpdateOneDey", AdminSetting.ShowCanUpdateOneDey.ToString(), CoreDefine.InputTypes.TwoValues.ToString()))); 
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ویرایش", "ShowCanUpdateThreeDey", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ویرایش به مدت سه روز", "ShowCanUpdateThreeDey", AdminSetting.ShowCanUpdateThreeDey.ToString(), CoreDefine.InputTypes.TwoValues.ToString()))); 
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ویرایش", "ShowCanUpdateOneWeek", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("ویرایش به مدت یک هفته", "ShowCanUpdateOneWeek", AdminSetting.ShowCanUpdateOneWeek.ToString(), CoreDefine.InputTypes.TwoValues.ToString()))); 

                         
                        FieldParameter.Add(new TemplateField(false, "تنظیمات import/export", "PermissionShowImportExportInForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دسترسی نمایش import در فرم", "PermissionShowImportExportInForm", AdminSetting.PermissionShowImportExportInForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات import/export", "ShowImportExportInAllForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش import در تمام فرم ها", "ShowImportExportInAllForm", AdminSetting.ShowImportExportInAllForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات import/export", "AllowFormShowImportExport", CoreDefine.InputTypes.CoreRelatedTableCheckbox, new { FieldName = "AllowFormShowImportExport", InputType = CoreDefine.InputTypes.CoreRelatedTableCheckbox.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش import", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = string.Join(",", AdminSetting.AllowFormShowImportExport), Entitiy = CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), ParentID = "0" }));

                        FieldParameter.Add(new TemplateField(false, "تنظیمات یادداشت", "PermissionShowCommentInForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دسترسی نمایش یادداشت در فرم", "PermissionShowCommentInForm", AdminSetting.PermissionShowCommentInForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات یادداشت", "ShowCommentInAllForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش یادداشت در تمام فرم ها", "ShowCommentInAllForm", AdminSetting.ShowCommentInAllForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات یادداشت", "AllowFormShowComment", CoreDefine.InputTypes.CoreRelatedTableCheckbox, new { FieldName = "AllowFormShowComment", InputType = CoreDefine.InputTypes.CoreRelatedTableCheckbox.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش یادداشت", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = string.Join(",", AdminSetting.AllowFormShowComment), Entitiy = CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), ParentID = "0" }));
                        
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "PermissionShowEmailInForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("دسترسی نمایش ایمیل در فرم", "PermissionShowEmailInForm", AdminSetting.PermissionShowEmailInForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(false, "تنظیمات ایمیل", "ShowEmailInAllForm", CoreDefine.InputTypes.TwoValues, Tools.ParameterField("نمایش ایمیل در تمام فرم ها", "ShowEmailInAllForm", AdminSetting.ShowEmailInAllForm.ToString(), CoreDefine.InputTypes.TwoValues.ToString())));
                        FieldParameter.Add(new TemplateField(true, "تنظیمات ایمیل", "AllowFormShowEmail", CoreDefine.InputTypes.CoreRelatedTableCheckbox, new { FieldName = "AllowFormShowEmail", InputType = CoreDefine.InputTypes.CoreRelatedTableCheckbox.ToString(), IsReadonly = false, IsGridField = false, IsRequired = true, IsInCellEditMode = false, IsLeftWrite = false, FieldTitle = "نمایش ایمیل", RelatedField = 0, RelatedTable = 0, _TableID = 0, FieldValue = string.Join(",", AdminSetting.AllowFormShowEmail), Entitiy = CoreDefine.Entities.فرم_ورود_اطلاعات.ToString(), ParentID = "0" }));
                        
                        break;
                    }

                case "WebService":
                    {
                        WebService WebServiceInfo;

                        if (ID == 0)
                            WebServiceInfo = new WebService();
                        else
                            WebServiceInfo = new WebService(_CoreObject);

                        List<SelectListItem> DataTypesList = new List<SelectListItem>() {
                                                              new SelectListItem() {Text = "هیچکدام", Value = ""},
                                                              new SelectListItem() {Text = "صحیح", Value = CoreDefine.DataTypes.صحیح.ToString()},
                                                              new SelectListItem() {Text = "اعشار", Value = CoreDefine.DataTypes.اعشار.ToString()},
                                                              new SelectListItem() {Text = "دو_مقدار", Value = CoreDefine.DataTypes.دو_مقدار.ToString()},
                                                              new SelectListItem() {Text = "رشته", Value = CoreDefine.DataTypes.رشته.ToString()},
                                                              new SelectListItem() {Text = "متن_بلند", Value = CoreDefine.DataTypes.متن_بلند.ToString()},
                                                              new SelectListItem() {Text = "جدول", Value = CoreDefine.DataTypes.جدول.ToString()}
                                                          };



                        FieldParameter.Add(new TemplateField(false, "عمومی", "URL", CoreDefine.InputTypes.ShortText, Tools.ParameterField("آدرس", "URL", WebServiceInfo.URL, CoreDefine.InputTypes.ShortText.ToString())));
                        FieldParameter.Add(new TemplateField(false, "عمومی", "ExportType", CoreDefine.InputTypes.ComboBox, Tools.ParameterField("نوع خروجی", "ExportType", WebServiceInfo.ExportType, CoreDefine.InputTypes.ShortText.ToString(), false, false, "", 0, DataTypesList)));


                        break;
                    }

            }

            ViewData["FolderDataKey"] = FolderList;
            ViewData["FieldParameterList"] = FieldParameter;
            ViewData["DataKey"] = SysSetting.SysDataKay;
            ViewData["SysSettingType"] = SysSettingType;
            ViewData["ShowInNewWindow"] = ShowInNewWindow; 
            return PartialView("SysSettingDetails");
        }

        public JsonResult SaveSysSettingDetail(string[] FormInputName, string[] FormInputValue,long SysSettingID=0)
        { 
            string Value = "";
            switch (SysSetting.SysSettingType)
            {
                case "GridRowColor":
                    {
                        GridRowColor info = new GridRowColor(); 
                        info.RowColorColumnName = FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnName")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnName")]);
                        info.RowColorColumnFullName = FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnFullName")];
                        info.RowColorOperator = FormInputValue[Array.IndexOf(FormInputName, "RowColorOperator")];
                        info.RowColorColumnValue = FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnValue")];
                        info.RowColorOperator2 = FormInputValue[Array.IndexOf(FormInputName, "RowColorOperator2")];
                        info.RowColorColumnValue2 = FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnValue2")];
                        info.RowColorSelectedColor = FormInputValue[Array.IndexOf(FormInputName, "RowColorSelectedColor")];
                        Value = Tools.ToXML(info);
                        break;
                    }
                case "Folder":
                    {
                        Folder folder = new Folder();
                        folder.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")]; 
                        folder.IsExpand = FormInputValue[Array.IndexOf(FormInputName, "IsExpand")]  == "false" ? false : true;
                        folder.IconColor = FormInputValue[Array.IndexOf(FormInputName, "IconColor")];
                        folder.IconSize = FormInputValue[Array.IndexOf(FormInputName, "IconSize")] == "" ? 0 : int.Parse(FormInputValue[Array.IndexOf(FormInputName, "IconSize")]);
                        Value = Tools.ToXML(folder);
                        break;
                    }
                case "InformationEntryForm":
                    {
                        CoreObject InformationEntryFormCore = CoreObject.Find(SysSetting.SysSettingID);
                        InformationEntryForm info = new InformationEntryForm();
                        info.InformationEntryFormName = InformationEntryFormCore.FullName;
                        info.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        info.ConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ConditionQuery")];
                        info.NewButtonVisibleConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_NewButtonVisibleConditionQuery")];
                        info.GroupByQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_GroupByQuery")];
                        info.OrderQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_OrderQuery")];
                        info.BadgeQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_BadgeQuery")];
                        info.BadgeColor = FormInputValue[Array.IndexOf(FormInputName, "BadgeColor")];
                        info.BadgeTextColor = FormInputValue[Array.IndexOf(FormInputName, "BadgeTextColor")];
                        info.BadgeTitle = FormInputValue[Array.IndexOf(FormInputName, "BadgeTitle")];
                        info.CSS = FormInputValue[Array.IndexOf(FormInputName, "CSS")];
                        info.NewPageUrl = FormInputValue[Array.IndexOf(FormInputName, "NewPageUrl")];
                        info.UpdatePageUrl = FormInputValue[Array.IndexOf(FormInputName, "UpdatePageUrl")];
                        info.RelatedTable = FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")]);
                        info.ExternalField = FormInputValue[Array.IndexOf(FormInputName, "ExternalField")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ExternalField")]);
                        info.GroupableField = FormInputValue[Array.IndexOf(FormInputName, "GroupableField")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "GroupableField")]);
                        info.SearchWithOnkeyDownCoreId = FormInputValue[Array.IndexOf(FormInputName, "SearchWithOnkeyDownCoreId")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "SearchWithOnkeyDownCoreId")]);
                        info.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        info.TitleSaveButton = FormInputValue[Array.IndexOf(FormInputName, "TitleSaveButton")];
                        info.ShowRecordCountDefault = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ShowRecordCountDefault")]);
                        info.RowCoutnInPage = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "RowCoutnInPage")]);
                        info.IsShowID = FormInputValue[Array.IndexOf(FormInputName, "IsShowID")] == "false" ? false : true;
                        info.ShowAttachment = FormInputValue[Array.IndexOf(FormInputName, "ShowAttachment")] == "false" ? false : true;
                        info.ShowInParentForm = FormInputValue[Array.IndexOf(FormInputName, "ShowInParentForm")] == "false" ? false : true;
                        info.ShowInMenueTreeList = FormInputValue[Array.IndexOf(FormInputName, "ShowInMenueTreeList")] == "false" ? false : true;
                        info.Groupable = FormInputValue[Array.IndexOf(FormInputName, "Groupable")] == "false" ? false : true;
                        info.Pageable = FormInputValue[Array.IndexOf(FormInputName, "Pageable")] == "false" ? false : true;
                        info.Aggregatesable = FormInputValue[Array.IndexOf(FormInputName, "Aggregatesable")] == "false" ? false : true;
                        info.ShowDetailAttachment = FormInputValue[Array.IndexOf(FormInputName, "ShowDetailAttachment")] == "false" ? false : true;
                        info.ShowAttachmentColumn = FormInputValue[Array.IndexOf(FormInputName, "ShowAttachmentColumn")] == "false" ? false : true;
                        info.ShowChartInformationEntryForm = FormInputValue[Array.IndexOf(FormInputName, "ShowChartInformationEntryForm")] == "false" ? false : true;
                        info.ShowSelectedColumn = FormInputValue[Array.IndexOf(FormInputName, "ShowSelectedColumn")] == "false" ? false : true;
                        info.IsCloseFormAffterSave = FormInputValue[Array.IndexOf(FormInputName, "IsCloseFormAffterSave")] == "false" ? false : true;
                        info.ShowLineNumber = FormInputValue[Array.IndexOf(FormInputName, "ShowLineNumber")] == "false" ? false : true;
                        info.SaveParentSubjectSaveChild = FormInputValue[Array.IndexOf(FormInputName, "SaveParentSubjectSaveChild")] == "false" ? false : true;
                        info.ShowAttachementButton = FormInputValue[Array.IndexOf(FormInputName, "ShowAttachementButton")] == "false" ? false : true;
                        info.ShowClearFormWhithOutFixItemButton = FormInputValue[Array.IndexOf(FormInputName, "ShowClearFormWhithOutFixItemButton")] == "false" ? false : true;
                        info.CheckValidChildGrid = FormInputValue[Array.IndexOf(FormInputName, "CheckValidChildGrid")] == "false" ? false : true;
                        info.ShowNewButton = FormInputValue[Array.IndexOf(FormInputName, "ShowNewButton")] == "false" ? false : true;
                        info.FixItemClearForm = FormInputValue[Array.IndexOf(FormInputName, "FixItemClearForm")].Replace(" ", "");
                        info.ClearFormWhithOutFixItemButtonName = FormInputValue[Array.IndexOf(FormInputName, "ClearFormWhithOutFixItemButtonName")];
                        info.ChartID = FormInputValue[Array.IndexOf(FormInputName, "ChartID")];
                        info.ChartName = FormInputValue[Array.IndexOf(FormInputName, "ChartName")];
                        info.ChartTitle = FormInputValue[Array.IndexOf(FormInputName, "ChartTitle")];
                        info.ChartGroup = FormInputValue[Array.IndexOf(FormInputName, "ChartGroup")];
                        info.ChartAvatar = FormInputValue[Array.IndexOf(FormInputName, "ChartAvatar")];
                        info.ChartParentID = FormInputValue[Array.IndexOf(FormInputName, "ChartParentID")];
                        info.AttachmentColumnName = FormInputValue[Array.IndexOf(FormInputName, "AttachmentColumnName")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "AttachmentColumnName")]);
                        info.RowColorColumnName = FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnName")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnName")]);
                        info.RowColorColumnValue = FormInputValue[Array.IndexOf(FormInputName, "RowColorColumnValue")];
                        info.RowColorSelectedColor = FormInputValue[Array.IndexOf(FormInputName, "RowColorSelectedColor")];
                        info.RowColorOperator = FormInputValue[Array.IndexOf(FormInputName, "RowColorOperator")];
                        info.FormComment = FormInputValue[Array.IndexOf(FormInputName, "FormComment")];
                        info.Height = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "Height")]);
                        info.HieghtAttachment = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "HieghtAttachment")]);
                        info.DefualtColumnShowInGrid = FormInputValue[Array.IndexOf(FormInputName, "DefualtColumnShowInGrid")];
                        info.SaveAtOnce = FormInputValue[Array.IndexOf(FormInputName, "SaveAtOnce")] == "false" ? false : true;
                        info.SearchWithOnkeyDown = FormInputValue[Array.IndexOf(FormInputName, "SearchWithOnkeyDown")] == "false" ? false : true;
                        info.CheckFieldDuplicateRecords = FormInputValue[Array.IndexOf(FormInputName, "CheckFieldDuplicateRecords")];
                        info.ColumnWidth = FormInputValue[Array.IndexOf(FormInputName, "ColumnWidth")];
                        info.AttachmentColumnWidth = FormInputValue[Array.IndexOf(FormInputName, "AttachmentColumnWidth")];
                        info.AnalystDescription = FormInputValue[Array.IndexOf(FormInputName, "AnalystDescription")];
                        info.ShowNewButtonInToolbar = FormInputValue[Array.IndexOf(FormInputName, "ShowNewButtonInToolbar")] == "false" ? false : true;
                        info.ShowAttachmentButtonInToolbar = FormInputValue[Array.IndexOf(FormInputName, "ShowAttachmentButtonInToolbar")] == "false" ? false : true;
                        info.ShowDeleteButtonInToolbar = FormInputValue[Array.IndexOf(FormInputName, "ShowDeleteButtonInToolbar")] == "false" ? false : true;
                        info.ShowUpdateButtonInToolbar = FormInputValue[Array.IndexOf(FormInputName, "ShowUpdateButtonInToolbar")] == "false" ? false : true;
                        info.ShowViewButtonInToolbar = FormInputValue[Array.IndexOf(FormInputName, "ShowViewButtonInToolbar")] == "false" ? false : true;
                       

                        if (!string.IsNullOrEmpty(info.Query))
                        {
                            if(info.Query.IndexOf("،")==-1 && info.Query.IndexOf(",") == -1 && info.Query.IndexOf("از.جدول")==-1 && info.Query.IndexOf("From") == -1)
                            {
                                string[] QueryArr = info.Query.Substring(1, info.Query.Length - 2).Split(' ');
                                string TableName = CoreObject.Find(info.RelatedTable).FullName;
                                for (int Index=0;Index<QueryArr.Length;Index++)
                                {
                                    if(QueryArr[Index] != "" && QueryArr[Index] != "{}" && QueryArr[Index] != "بعنوان")
                                    {
                                        QueryArr[Index] = TableName + "." + QueryArr[Index].Trim();
                                        if (Index < QueryArr.Length - 1)
                                            QueryArr[Index] += ",\n"; 
                                    }
                                    else
                                        QueryArr[Index] += " ";
                                } 
                                info.Query= string.Join("", QueryArr) + "\n از.جدول " + TableName;
                            }
                        }
                        switch (FormInputValue[Array.IndexOf(FormInputName, "OpenFormType")])
                        {
                            case "NewForm":
                                {
                                    info.OpenFormType = CoreDefine.OpenFormType.NewForm ;
                                    break;
                                }
                           default:
                                {
                                    info.OpenFormType = CoreDefine.OpenFormType.Grid ;
                                    break;
                                }
                        }
                        switch (FormInputValue[Array.IndexOf(FormInputName, "GridEditMode")])
                        {
                            case "InCell":
                                {
                                    info.GridEditMode = GridEditMode.InCell;
                                    break;
                                }
                            case "PopUp":
                                {
                                    info.GridEditMode = GridEditMode.PopUp;
                                    break;
                                }
                            case "InLine":
                                {
                                    info.GridEditMode = GridEditMode.InLine;
                                    break;
                                }
                        }

                        Value = Tools.ToXML(info);
                        break;
                    }

                case "NewButtonForm":
                    {
                        NewButtonForm newButtonForm = new NewButtonForm();
                        newButtonForm.UseUrl = FormInputValue[Array.IndexOf(FormInputName, "UseUrl")] == "false" ? false : true;
                        newButtonForm.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        newButtonForm.Url = FormInputValue[Array.IndexOf(FormInputName, "Url")];
                        newButtonForm.RelatedInformationForm = FormInputValue[Array.IndexOf(FormInputName, "RelatedInformationForm")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedInformationForm")]);
                        Value = Tools.ToXML(newButtonForm);
                        break;
                    }
                        
                case "SearchForm":
                    {
                        SearchForm SearchForm = new SearchForm();
                        SearchForm.RelatedTable = FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")]);
                        SearchForm.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        SearchForm.ConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ConditionQuery")];
                        SearchForm.CommonConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_CommonConditionQuery")];
                        SearchForm.SearchConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_SearchConditionQuery")];
                        SearchForm.SearchAlarmQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_SearchAlarmQuery")];
                        SearchForm.GroupByQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_GroupByQuery")];
                        SearchForm.SelectedColumns = FormInputValue[Array.IndexOf(FormInputName, "SelectedColumns")];
                        SearchForm.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        SearchForm.ShowIcon = FormInputValue[Array.IndexOf(FormInputName, "ShowIcon")] == "false" ? false : true;
                        SearchForm.ShowText = FormInputValue[Array.IndexOf(FormInputName, "ShowText")] == "false" ? false : true;
                        SearchForm.CleareGridAfterSearch = FormInputValue[Array.IndexOf(FormInputName, "CleareGridAfterSearch")] == "false" ? false : true;
                        Value = Tools.ToXML(SearchForm);
                        break;
                    }

                case "SearchField":
                    {
                        SearchField SearchForm = new SearchField();
                        SearchForm.RelatedField = FormInputValue[Array.IndexOf(FormInputName, "RelatedField")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedField")]);
                        SearchForm.DefaultOperator = FormInputValue[Array.IndexOf(FormInputName, "DefaultOperator")];
                        SearchForm.DefaultValue = FormInputValue[Array.IndexOf(FormInputName, "DefaultValue")];
                        SearchForm.ShowField = FormInputValue[Array.IndexOf(FormInputName, "ShowField")] == "false" ? false : true;
                        SearchForm.ShowOperator = FormInputValue[Array.IndexOf(FormInputName, "ShowOperator")] == "false" ? false : true;
                        Value = Tools.ToXML(SearchForm);
                        break;
                    }

                case "WebServiceParameter":
                    {
                        WebServiceParameter _WebServiceParameter = new WebServiceParameter();
                        _WebServiceParameter.QueryValue = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_QueryValue")];
                        _WebServiceParameter.Name = FormInputValue[Array.IndexOf(FormInputName, "Name")];
                        _WebServiceParameter.Value = FormInputValue[Array.IndexOf(FormInputName, "Value")];
                        _WebServiceParameter.ConvertToJsonArr = FormInputValue[Array.IndexOf(FormInputName, "ConvertToJsonArr")] == "false" ? false : true;
                        Value = Tools.ToXML(_WebServiceParameter);
                        break;
                    }

                case "WebService":
                    {
                        WebService WebService = new WebService();
                        WebService.URL = FormInputValue[Array.IndexOf(FormInputName, "URL")];
                        WebService.ExportType = FormInputValue[Array.IndexOf(FormInputName, "ExportType")];

                        Value = Tools.ToXML(WebService);
                        break;
                    }

                case "PaymentSetting":
                    {
                        Payment Payment = new Payment();
                        Payment.MerchantID = FormInputValue[Array.IndexOf(FormInputName, "MerchantID")];
                        Payment.CallBackUrl = FormInputValue[Array.IndexOf(FormInputName, "CallBackUrl")];
                        Payment.PaymentGatewaytype = FormInputValue[Array.IndexOf(FormInputName, "PaymentGatewaytype")];

                        Value = Tools.ToXML(Payment);
                        break;

                    }

                case "PublicSetting":
                    {
                        PublicSetting PublicSetting = new PublicSetting();
                        PublicSetting.CompanyName = FormInputValue[Array.IndexOf(FormInputName, "CompanyName")];
                        PublicSetting.AppPersianName = FormInputValue[Array.IndexOf(FormInputName, "AppPersianName")];
                        PublicSetting.AppEnglishName = FormInputValue[Array.IndexOf(FormInputName, "AppEnglishName")];
                        PublicSetting.WebSite = FormInputValue[Array.IndexOf(FormInputName, "WebSite")];
                        PublicSetting.PhoneNumber = FormInputValue[Array.IndexOf(FormInputName, "PhoneNumber")];
                        PublicSetting.BrandSlogan = FormInputValue[Array.IndexOf(FormInputName, "BrandSlogan")];
                        PublicSetting.AppLogo = FormInputValue[Array.IndexOf(FormInputName, "AppLogo")];
                        PublicSetting.MainColor = FormInputValue[Array.IndexOf(FormInputName, "MainColor")];
                        PublicSetting.IconColor = FormInputValue[Array.IndexOf(FormInputName, "IconColor")];
                        PublicSetting.TitleColor = FormInputValue[Array.IndexOf(FormInputName, "TitleColor")];
                        PublicSetting.IntroduceOthers = FormInputValue[Array.IndexOf(FormInputName, "IntroduceOthers")];
                        PublicSetting.Laws = FormInputValue[Array.IndexOf(FormInputName, "Laws")];
                        PublicSetting.UserPrivacy = FormInputValue[Array.IndexOf(FormInputName, "UserPrivacy")];
                        PublicSetting.FileSavingPath = FormInputValue[Array.IndexOf(FormInputName, "FileSavingPath")];
                        PublicSetting.LoginBackgroundImage = FormInputValue[Array.IndexOf(FormInputName, "LoginBackgroundImage")];
                        PublicSetting.HomeBackgroundImage = FormInputValue[Array.IndexOf(FormInputName, "HomeBackgroundImage")];
                        PublicSetting.GeneralMessage = FormInputValue[Array.IndexOf(FormInputName, "GeneralMessage")];
                        PublicSetting.CSS = FormInputValue[Array.IndexOf(FormInputName, "CSS")];
                        PublicSetting.JS = FormInputValue[Array.IndexOf(FormInputName, "JS")];
                        PublicSetting.E_Maile = FormInputValue[Array.IndexOf(FormInputName, "E_Maile")];
                        PublicSetting.EMaileUserName = FormInputValue[Array.IndexOf(FormInputName, "EMaileUserName")];
                        PublicSetting.EMailePassWord = FormInputValue[Array.IndexOf(FormInputName, "EMailePassWord")];
                        PublicSetting.EMailePort = FormInputValue[Array.IndexOf(FormInputName, "EMailePort")];
                        PublicSetting.EMaileServer = FormInputValue[Array.IndexOf(FormInputName, "EMaileServer")];
                        PublicSetting.SupportLink = FormInputValue[Array.IndexOf(FormInputName, "SupportLink")];
                        PublicSetting.RelatedWebService = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedWebService")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedWebService")]);
                        PublicSetting.ChangePasswordDays = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ChangePasswordDays")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ChangePasswordDays")]);
                        PublicSetting.DataRecoveryMinutes = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "DataRecoveryMinutes")] == "" ? "5" : FormInputValue[Array.IndexOf(FormInputName, "DataRecoveryMinutes")]);

                        Value = Tools.ToXML(PublicSetting);
                        Referral.PublicSetting = PublicSetting;
                        break;
                    }

                case "AdminSetting":
                    {
                        AdminSetting adminSetting = new AdminSetting();
                        adminSetting.ShowUserRegistryInLoginForm = FormInputValue[Array.IndexOf(FormInputName, "ShowUserRegistryInLoginForm")] == "false" ? false : true;
                        adminSetting.ShowDataSourceListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowDataSourceListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowSpecialPhraseListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowSpecialPhraseListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowInformationEntryFormListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowInformationEntryFormListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowProcessListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowProcessListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowReportListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowReportListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowDashboardListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowDashboardListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowPublicFileListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowPublicFileListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowConnectWebsiteListInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowConnectWebsiteListInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowSMSSettingInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowSMSSettingInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowEmailSettingInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowEmailSettingInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowPaymentSettingInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowPaymentSettingInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowPublicSettingInSettingForm = FormInputValue[Array.IndexOf(FormInputName, "ShowPublicSettingInSettingForm")] == "false" ? false : true;
                        adminSetting.ShowUserCalendar = FormInputValue[Array.IndexOf(FormInputName, "ShowUserCalendar")] == "false" ? false : true;
                        adminSetting.ShowInfoInLogin = FormInputValue[Array.IndexOf(FormInputName, "ShowInfoInLogin")] == "false" ? false : true;
                        adminSetting.ShowAllRights = FormInputValue[Array.IndexOf(FormInputName, "ShowAllRights")] == "false" ? false : true;

                        adminSetting.ShowEditingRestrictions = FormInputValue[Array.IndexOf(FormInputName, "ShowEditingRestrictions")] == "false" ? false : true;
                        adminSetting.ShowCanUpdateOnlyUserRegistry = FormInputValue[Array.IndexOf(FormInputName, "ShowCanUpdateOnlyUserRegistry")] == "false" ? false : true;
                        adminSetting.ShowCanUpdateOneDey = FormInputValue[Array.IndexOf(FormInputName, "ShowCanUpdateOneDey")] == "false" ? false : true;
                        adminSetting.ShowCanUpdateThreeDey = FormInputValue[Array.IndexOf(FormInputName, "ShowCanUpdateThreeDey")] == "false" ? false : true;
                        adminSetting.ShowCanUpdateOneWeek = FormInputValue[Array.IndexOf(FormInputName, "ShowCanUpdateOneWeek")] == "false" ? false : true;

                        adminSetting.AllowFormShowImportExport = FormInputValue[Array.IndexOf(FormInputName, "Result_AllowFormShowImportExport")].Split(',');
                        adminSetting.PermissionShowImportExportInForm = FormInputValue[Array.IndexOf(FormInputName, "PermissionShowImportExportInForm")] == "false" ? false : true;
                        adminSetting.ShowImportExportInAllForm = FormInputValue[Array.IndexOf(FormInputName, "ShowImportExportInAllForm")] == "false" ? false : true;

                        adminSetting.AllowFormShowComment = FormInputValue[Array.IndexOf(FormInputName, "Result_AllowFormShowComment")].Split(',');
                        adminSetting.PermissionShowCommentInForm = FormInputValue[Array.IndexOf(FormInputName, "PermissionShowCommentInForm")] == "false" ? false : true;
                        adminSetting.ShowCommentInAllForm = FormInputValue[Array.IndexOf(FormInputName, "ShowCommentInAllForm")] == "false" ? false : true;
                        

                        adminSetting.AllowFormShowEmail = FormInputValue[Array.IndexOf(FormInputName, "Result_AllowFormShowEmail")].Split(',');
                        adminSetting.PermissionShowEmailInForm = FormInputValue[Array.IndexOf(FormInputName, "PermissionShowEmailInForm")] == "false" ? false : true;
                        adminSetting.ShowEmailInAllForm = FormInputValue[Array.IndexOf(FormInputName, "ShowEmailInAllForm")] == "false" ? false : true;

                        Value = Tools.ToXML(adminSetting);
                        Referral.AdminSetting= adminSetting;
                        break;
                    }

                case "SpecialPhrase":
                    {
                        SpecialPhrase SpecialPhrase = new SpecialPhrase();
                        SpecialPhrase.DataSourceID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "DataSourceID")] == "" ? Referral.MasterDatabaseID.ToString() : FormInputValue[Array.IndexOf(FormInputName, "DataSourceID")]);
                        SpecialPhrase.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];  
                        Value = Tools.ToXML(SpecialPhrase);
                        break;

                    }

                case "DisplayField":
                    {
                        DisplayField DisplayField = new DisplayField();
                        DisplayField.Template = FormInputValue[Array.IndexOf(FormInputName, "Template")].Replace("*", "<");
                        DisplayField.ReportID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ReportID")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ReportID")]);
                        DisplayField.ParameterReportID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ParameterReportID")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ParameterReportID")]);
                        DisplayField.TableButtonEventsType = Tools.GetTableButtonEventsType(FormInputValue[Array.IndexOf(FormInputName, "TableButtonEventsType")]);
                        DisplayField.DisplayName = FormInputValue[Array.IndexOf(FormInputName, "DisplayName")];
                        DisplayField.ShowInStart = FormInputValue[Array.IndexOf(FormInputName, "ShowInStart")] == "false" ? false : true;
                        DisplayField.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")] ;
                        DisplayField.ShowTitle = FormInputValue[Array.IndexOf(FormInputName, "ShowTitle")] == "false" ? false : true;
                        DisplayField.IsReloadGrid = FormInputValue[Array.IndexOf(FormInputName, "IsReloadGrid")] == "false" ? false : true;
                        DisplayField.ExecutionConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ExecutionConditionQuery")];
                        DisplayField.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        DisplayField.RelatedForm = FormInputValue[Array.IndexOf(FormInputName, "RelatedForm")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedForm")]);
                        DisplayField.RelatedField = FormInputValue[Array.IndexOf(FormInputName, "RelatedField")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedField")]);

                        DisplayField.EMail = FormInputValue[Array.IndexOf(FormInputName, "EMail")];
                        DisplayField.EMailUserName = FormInputValue[Array.IndexOf(FormInputName, "EMailUserName")];
                        DisplayField.EMailPassWord = FormInputValue[Array.IndexOf(FormInputName, "EMailPassWord")];
                        DisplayField.EMailPort = FormInputValue[Array.IndexOf(FormInputName, "EMailPort")];
                        DisplayField.EMailServer = FormInputValue[Array.IndexOf(FormInputName, "EMailServer")];

                        DisplayField.ReceivingUsers = FormInputValue[Array.IndexOf(FormInputName, "ReceivingUsers")].Split(',');
                        DisplayField.ReceivingRole = FormInputValue[Array.IndexOf(FormInputName, "ReceivingRole")].Split(',');
                        DisplayField.InsertingUser = FormInputValue[Array.IndexOf(FormInputName, "InsertingUser")] == "false" ? false : true;
                        DisplayField.ReceivingQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ReceivingQuery")];
                        DisplayField.SendAttachmentFile = FormInputValue[Array.IndexOf(FormInputName, "SendAttachmentFile")] == "false" ? false : true;
                        DisplayField.UsePublickEmail = FormInputValue[Array.IndexOf(FormInputName, "UsePublickEmail")] == "false" ? false : true;
                        DisplayField.EnableSsl = FormInputValue[Array.IndexOf(FormInputName, "EnableSsl")] == "false" ? false : true;
                        DisplayField.SendReport = FormInputValue[Array.IndexOf(FormInputName, "SendReport")].Split(',');
                        DisplayField.Title = FormInputValue[Array.IndexOf(FormInputName, "Title")];
                        DisplayField.TitleQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_TitleQuery")];
                        DisplayField.BodyMessage = FormInputValue[Array.IndexOf(FormInputName, "BodyMessage")];
                        DisplayField.BodyMessageQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_BodyMessageQuery")];
                        Value = Tools.ToXML(DisplayField);
                        break;
                    }

                case "Process":
                    {
                        ProcessType ProcessType = new ProcessType(CoreObject.Find(SysSettingID));
                        ProcessType.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        ProcessType.ReportID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ReportID")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ReportID")]);
                        ProcessType.ParameterReportID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "ParameterReportID")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ParameterReportID")]);
                        ProcessType.InformationEntryFormID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "InformationEntryFormID")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "InformationEntryFormID")]);

                        Value = Tools.ToXML(ProcessType);
                        break;
                    }
                case "BpmnIncoming":
                    {
                        BpmnSequenceFlow BpmnSequenceFlow = new BpmnSequenceFlow(CoreObject.Find(SysSettingID));
                        BpmnSequenceFlow.ConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ConditionQuery")]; 
                        Value = Tools.ToXML(BpmnSequenceFlow);
                        break;
                    }
                case "BpmnOutgoin":
                    {
                        BpmnSequenceFlow BpmnSequenceFlow = new BpmnSequenceFlow(CoreObject.Find(SysSettingID));
                        BpmnSequenceFlow.ConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ConditionQuery")]; 
                        Value = Tools.ToXML(BpmnSequenceFlow);
                        break;
                    }

                case "PublicJob":
                    {
                        CoreDefine.PublicJobType publicJobType = new CoreDefine.PublicJobType();
                        switch (FormInputValue[Array.IndexOf(FormInputName, "PublicJobType")])
                        {
                            case "اجرای_رویداد": publicJobType = CoreDefine.PublicJobType.اجرای_رویداد; break;
                            case "ارسال_پیامک": publicJobType = CoreDefine.PublicJobType.ارسال_پیامک; break;
                            case "ارسال_ایمیل": publicJobType = CoreDefine.PublicJobType.ارسال_ایمیل; break;
                            case "انتقال_فایل": publicJobType = CoreDefine.PublicJobType.انتقال_فایل; break;
                            case "ارسال_گزارش": publicJobType = CoreDefine.PublicJobType.ارسال_گزارش; break;
                            default: publicJobType = CoreDefine.PublicJobType.خالی; break;

                        }
                        PublicJob PublicJob = new PublicJob();
                        CoreObject PublicJobCore = CoreObject.Find(SysSetting.SysSettingID);
                        PublicJob.StartDate = FormInputValue[Array.IndexOf(FormInputName, "StartDate")];
                        PublicJob.EndDate = FormInputValue[Array.IndexOf(FormInputName, "EndDate")];
                        PublicJob.StartTime = FormInputValue[Array.IndexOf(FormInputName, "StartTime")];
                        PublicJob.EndTime = FormInputValue[Array.IndexOf(FormInputName, "EndTime")];
                        PublicJob.RepeatDay = FormInputValue[Array.IndexOf(FormInputName, "RepeatDay")] == "" ? 0 : int.Parse(FormInputValue[Array.IndexOf(FormInputName, "RepeatDay")]);
                        PublicJob.RepeatClock = FormInputValue[Array.IndexOf(FormInputName, "RepeatClock")] == "" ? 0 : int.Parse(FormInputValue[Array.IndexOf(FormInputName, "RepeatClock")]);
                        PublicJob.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        PublicJob.FullName = PublicJobCore.FullName;
                        PublicJob.CoreObjectID = PublicJobCore.CoreObjectID;
                        PublicJob.PublicJobType = publicJobType;
                        PublicJob.RelatedDatasource = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedDatasource")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedDatasource")]);
                        Value = Tools.ToXML(PublicJob);

                        if (PublicJob.PublicJobType == CoreDefine.PublicJobType.اجرای_رویداد)
                        {
                            DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(PublicJob.RelatedDatasource));
                            switch (dataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        string Qeury = @"DECLARE @FullName as Nvarchar(255) =N'" + PublicJobCore.FullName + "' \n " +
                                            "DECLARE @jobId BINARY(16) \n" +
                                            "EXEC  msdb.dbo.sp_add_job \n" +
                                            "       @job_name=@FullName,@enabled=1,@notify_level_eventlog=0,@notify_level_email=0, 	@notify_level_netsend=0,@notify_level_page=0,@delete_level=0,@job_id = @jobId OUTPUT \n" +
                                            "EXEC  msdb.dbo.sp_add_jobstep \n" +
                                            "@job_id=@jobId,@step_name=@FullName,@step_id=1,@cmdexec_success_code=0,@on_success_action=1,@on_success_step_id=0,@on_fail_action=2,@on_fail_step_id=0,@retry_attempts=0,@retry_interval=0,@os_run_priority=0,@subsystem=N'TSQL',@database_name=N'" + dataSourceInfo.DataBase + "',@flags=0,@command=N'" + PublicJob.Query + "' \n" +
                                            "EXEC  msdb.dbo.sp_add_jobschedule \n" +
                                            "   @job_id=@jobId,@name=@FullName,@enabled=1,@freq_type=4,@freq_interval=" + PublicJob.RepeatDay.ToString() + @",@freq_subday_type=8, @freq_subday_interval=" + PublicJob.RepeatClock.ToString() + @",@freq_relative_interval=0,@freq_recurrence_factor=0,@active_start_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.StartDate).Replace("/", "") + ",@active_end_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.EndDate).Replace("/", "") + ",@active_start_time=" + PublicJob.StartTime.Replace(":", "") + ",@active_end_time=" + PublicJob.EndTime.Replace(":", "");
                                        SQLDataBase sQLDataBase = Referral.DBMsdb;
                                        if (dataSourceInfo.ServerName != Referral.DBMsdb.ConnectionData.Source)
                                            sQLDataBase = new SQLDataBase(dataSourceInfo.ServerName, "msdb", dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                        if (sQLDataBase.Execute(Qeury))
                                        {

                                        }
                                        else
                                        {
                                            Qeury = @"DECLARE @FullName as Nvarchar(255) =N'" + PublicJobCore.FullName + "' \n " +
                                                "EXEC  dbo.sp_update_jobstep  @job_name = @FullName, @step_id = 1,  @step_name = @FullName,@subsystem = N'TSQL',@command = N'" + PublicJob.Query + "'  ; \n" +
                                                "EXEC  dbo.sp_update_schedule  \n" +
                                            "    @name=@FullName,@enabled=1,@freq_type=4,@freq_interval=" + PublicJob.RepeatDay.ToString() + @",@freq_subday_type=8, @freq_subday_interval=" + PublicJob.RepeatClock.ToString() + @",@freq_relative_interval=0,@freq_recurrence_factor=0,@active_start_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.StartDate).Replace("/", "") + ",@active_end_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.EndDate).Replace("/", "") + ",@active_start_time=" + PublicJob.StartTime.Replace(":", "") + ",@active_end_time=" + PublicJob.EndTime.Replace(":", "");
                                            sQLDataBase.Execute(Qeury);
                                        }

                                        break;
                                    }
                            }
                        }
                        break;
                    }

                case "ParameterTableFunction":
                    {
                        ParameterTableFunction ParameterTableFunction = new ParameterTableFunction();
                        ParameterTableFunction.ParameterDataType = FormInputValue[Array.IndexOf(FormInputName, "ParameterDataType")];
                        Value = Tools.ToXML(ParameterTableFunction);
                        break;
                    }

                case "Report":
                    {
                        CoreObject ParameterCore = CoreObject.Find(SysSetting.SysSettingID);
                        Report ReportParameter = new Report(ParameterCore);
                        ReportParameter.FullName = ParameterCore.FullName;
                        ReportParameter.DataSourceID = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "DataSourceID")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "DataSourceID")]);
                        ReportParameter.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        ReportParameter.ShowParameterInload = FormInputValue[Array.IndexOf(FormInputName, "ShowParameterInload")] == "false" ? false : true;
                        ReportParameter.ShowInMainMenu = FormInputValue[Array.IndexOf(FormInputName, "ShowInMainMenu")] == "false" ? false : true;
                        ReportParameter.SelectedRow = FormInputValue[Array.IndexOf(FormInputName, "SelectedRow")] == "false" ? false : true;
                        ReportParameter.UseDefualtIconColor = FormInputValue[Array.IndexOf(FormInputName, "UseDefualtIconColor")] == "false" ? false : true;
                        ReportParameter.IconColor = FormInputValue[Array.IndexOf(FormInputName, "IconColor")];
                        ReportParameter.PrinterName = FormInputValue[Array.IndexOf(FormInputName, "PrinterName")];
                        ReportParameter.PrintCopy = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "PrintCopy")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "PrintCopy")]);
                        ReportParameter.QueryPrintCopy = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_QueryPrintCopy")];
                        ReportParameter.QueryAffterPrint = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_QueryAffterPrint")];
                        ReportParameter.QueryBeforRun = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_QueryBeforRun")];
                        Value = Tools.ToXML(ReportParameter);
                        break;
                    }

                case "ReportParameter":
                    {
                        CoreObject ParameterCore = CoreObject.Find(SysSetting.SysSettingID);
                        ReportParameter ReportParameter = new ReportParameter();
                        ReportParameter.FullName = ParameterCore.FullName;
                        ReportParameter.InputTypes = Tools.GetInputType(FormInputValue[Array.IndexOf(FormInputName, "InputTypes")]);
                        ReportParameter.Value = FormInputValue[Array.IndexOf(FormInputName, "Value")];
                        ReportParameter.SpecialValue = FormInputValue[Array.IndexOf(FormInputName, "SpecialValue")];
                        ReportParameter.RelatedTable = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")]);
                        ReportParameter.RelatedField = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedField")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedField")]);
                        ReportParameter.RelatedFieldCommand = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedFieldCommand")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedFieldCommand")]);
                        ReportParameter.ViewCommand = FormInputValue[Array.IndexOf(FormInputName, "ViewCommand")];
                        ReportParameter.DigitsAfterDecimal = Convert.ToInt32(FormInputValue[Array.IndexOf(FormInputName, "DigitsAfterDecimal")]);
                        ReportParameter.IsLeftWrite = FormInputValue[Array.IndexOf(FormInputName, "IsLeftWrite")] == "false" ? false : true;
                        ReportParameter.ActiveOnKeyDown = FormInputValue[Array.IndexOf(FormInputName, "ActiveOnKeyDown")] == "false" ? false : true;
                        ReportParameter.IsEditAble = FormInputValue[Array.IndexOf(FormInputName, "IsEditAble")] == "false" ? false : true;

                        Value = Tools.ToXML(ReportParameter);
                        break;
                    }

                case "Table":
                    {
                        Table Table = new Table();
                        Table.Comment = FormInputValue[Array.IndexOf(FormInputName, "Comment")];
                        Table.TABLESCHEMA = FormInputValue[Array.IndexOf(FormInputName, "TABLESCHEMA")];
                        Table.DisplayName = FormInputValue[Array.IndexOf(FormInputName, "DisplayName")];
                        Table.ShowRecordCountDefault = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "ShowRecordCountDefault")]);
                        Table.AnalystDescription = FormInputValue[Array.IndexOf(FormInputName, "AnalystDescription")];

                        Value = Tools.ToXML(Table);
                        break;
                    }

                case "TableAttachment":
                    {
                        TableAttachment TableAttachment = new TableAttachment();


                        TableAttachment.AttachmentUploadType = Tools.GetAttachmentUploadType(FormInputValue[Array.IndexOf(FormInputName, "AttachmentUploadType")]);
                        TableAttachment.AllowedExtensions = FormInputValue[Array.IndexOf(FormInputName, "AllowedExtensions")].Split(',');
                        TableAttachment.MaxFileSize = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "MaxFileSize")]);
                        TableAttachment.MinFileSize = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "MinFileSize")]);
                        TableAttachment.ShowDefault = FormInputValue[Array.IndexOf(FormInputName, "ShowDefault")] == "false" ? false : true;
                        TableAttachment.IsRequired = FormInputValue[Array.IndexOf(FormInputName, "IsRequired")] == "false" ? false : true;
                        TableAttachment.SaveInDatabase = FormInputValue[Array.IndexOf(FormInputName, "SaveInDatabase")] == "false" ? false : true;
                        TableAttachment.AutoFillQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_AutoFillQuery")];
                        TableAttachment.ColumnWidth = FormInputValue[Array.IndexOf(FormInputName, "ColumnWidth")];
                        ;
                        switch (FormInputValue[Array.IndexOf(FormInputName, "AttachmentUploadSize")])
                        {
                            case "کوچک":
                                TableAttachment.AttachmentUploadSize = CoreDefine.AttachmentUploadSize.کوچک; break;
                            case "متوسط":
                                TableAttachment.AttachmentUploadSize = CoreDefine.AttachmentUploadSize.متوسط; break;
                            default:
                                TableAttachment.AttachmentUploadSize = CoreDefine.AttachmentUploadSize.بزرگ; break;

                        }

                        CoreObject Fieldcore = CoreObject.Find(SysSetting.SysSettingID);
                        if (!string.IsNullOrEmpty(TableAttachment.AutoFillQuery))
                        {
                            List<CoreObject> FieldObjectList = CoreObject.FindChilds(Fieldcore.ParentID, CoreDefine.Entities.فیلد);
                            foreach (CoreObject FieldObject in FieldObjectList)
                            {
                                if (TableAttachment.AutoFillQuery.IndexOf("@" + FieldObject.FullName) > -1)
                                {
                                    Field FieldItem = new Field(FieldObject);
                                    if (!FieldItem.ActiveOnKeyDown)
                                    {
                                        FieldItem.ActiveOnKeyDown = true;
                                        Value = Tools.ToXML(FieldItem);
                                        if (Referral.DBCore.UpdateRow(FieldObject.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value }))
                                        {
                                            int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == FieldObject.CoreObjectID);
                                            Referral.CoreObjects[FieldIndex].Value = Value;
                                        }
                                    }
                                }
                            }
                        }
                        Value = Tools.ToXML(TableAttachment);
                        break;
                    }

                case "TableEvent":
                    {
                        TableEvent TableEvent = new TableEvent();

                        TableEvent.TypeEventExecution = FormInputValue[Array.IndexOf(FormInputName, "TypeEventExecution")];
                        TableEvent.EventType = FormInputValue[Array.IndexOf(FormInputName, "EventType")];
                        TableEvent.Condition = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Condition")];
                        TableEvent.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        TableEvent.RelatedTable = FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")]);
                        TableEvent.RelatedWebService = FormInputValue[Array.IndexOf(FormInputName, "RelatedWebService")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedWebService")]);

                        TableEvent.EMail = FormInputValue[Array.IndexOf(FormInputName, "EMail")];
                        TableEvent.EMailUserName = FormInputValue[Array.IndexOf(FormInputName, "EMailUserName")];
                        TableEvent.EMailPassWord = FormInputValue[Array.IndexOf(FormInputName, "EMailPassWord")];
                        TableEvent.EMailPort = FormInputValue[Array.IndexOf(FormInputName, "EMailPort")];
                        TableEvent.EMailServer = FormInputValue[Array.IndexOf(FormInputName, "EMailServer")];

                        TableEvent.ReceivingUsers = FormInputValue[Array.IndexOf(FormInputName, "ReceivingUsers")].Split(',');
                        TableEvent.ReceivingRole = FormInputValue[Array.IndexOf(FormInputName, "ReceivingRole")].Split(',');
                        TableEvent.InsertingUser = FormInputValue[Array.IndexOf(FormInputName, "InsertingUser")] == "false" ? false : true;
                        TableEvent.ReceivingQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ReceivingQuery")];
                        TableEvent.SendAttachmentFile = FormInputValue[Array.IndexOf(FormInputName, "SendAttachmentFile")] == "false" ? false : true;
                        TableEvent.UsePublickEmail = FormInputValue[Array.IndexOf(FormInputName, "UsePublickEmail")] == "false" ? false : true;
                        TableEvent.EnableSsl = FormInputValue[Array.IndexOf(FormInputName, "EnableSsl")] == "false" ? false : true;
                        TableEvent.SendReport = FormInputValue[Array.IndexOf(FormInputName, "SendReport")].Split(',');
                        TableEvent.Title = FormInputValue[Array.IndexOf(FormInputName, "Title")];
                        TableEvent.TitleQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_TitleQuery")];
                        TableEvent.BodyMessage = FormInputValue[Array.IndexOf(FormInputName, "BodyMessage")];
                        TableEvent.BodyMessageQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_BodyMessageQuery")];

                        TableEvent.ReferralDeadlineResponse = FormInputValue[Array.IndexOf(FormInputName, "ReferralDeadlineResponse")] == "" ? 0 : int.Parse(FormInputValue[Array.IndexOf(FormInputName, "ReferralDeadlineResponse")]);
                        TableEvent.ReferralRecipientsUser = FormInputValue[Array.IndexOf(FormInputName, "ReferralRecipientsUser")].Split(',');
                        TableEvent.ReferralRecipientsRole = FormInputValue[Array.IndexOf(FormInputName, "ReferralRecipientsRole")].Split(',');
                        TableEvent.ReferralRecipientsQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ReferralRecipientsQuery")];
                        TableEvent.ReferralTitle = FormInputValue[Array.IndexOf(FormInputName, "ReferralTitle")];
                        TableEvent.ReferralTitleQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ReferralTitleQuery")];

                        Value = Tools.ToXML(TableEvent);
                        break;
                    }


                case "TableButton":
                    {
                        TableButton TableButton = new TableButton();
                        TableButton.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        TableButton.ButtonColor = FormInputValue[Array.IndexOf(FormInputName, "ButtonColor")];
                        TableButton.IsShowText = FormInputValue[Array.IndexOf(FormInputName, "IsShowText")] == "false" ? false : true;
                        TableButton.IsShowIcon = FormInputValue[Array.IndexOf(FormInputName, "IsShowIcon")] == "false" ? false : true;
                        TableButton.IsReloadGrid = FormInputValue[Array.IndexOf(FormInputName, "IsReloadGrid")] == "false" ? false : true;
                        TableButton.URL = FormInputValue[Array.IndexOf(FormInputName, "URL")];
                        TableButton.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        TableButton.ExecutionConditionQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ExecutionConditionQuery")];
                        TableButton.TableButtonEventsType = Tools.GetTableButtonEventsType(FormInputValue[Array.IndexOf(FormInputName, "TableButtonEventsType")]);
                        TableButton.RelatedForm = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedForm")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedForm")]);
                        TableButton.RelatedField = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedField")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedField")]);
                        TableButton.RelatedWebService = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedWebService")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RelatedWebService")]);

                        TableButton.EMail = FormInputValue[Array.IndexOf(FormInputName, "EMail")];
                        TableButton.EMailUserName = FormInputValue[Array.IndexOf(FormInputName, "EMailUserName")];
                        TableButton.EMailPassWord = FormInputValue[Array.IndexOf(FormInputName, "EMailPassWord")];
                        TableButton.EMailPort = FormInputValue[Array.IndexOf(FormInputName, "EMailPort")];
                        TableButton.EMailServer = FormInputValue[Array.IndexOf(FormInputName, "EMailServer")];

                        TableButton.ReceivingUsers = FormInputValue[Array.IndexOf(FormInputName, "ReceivingUsers")].Split(',');
                        TableButton.ReceivingRole = FormInputValue[Array.IndexOf(FormInputName, "ReceivingRole")].Split(',');
                        TableButton.InsertingUser = FormInputValue[Array.IndexOf(FormInputName, "InsertingUser")] == "false" ? false : true;
                        TableButton.ReceivingQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ReceivingQuery")];
                        TableButton.SendAttachmentFile = FormInputValue[Array.IndexOf(FormInputName, "SendAttachmentFile")] == "false" ? false : true;
                        TableButton.UsePublickEmail = FormInputValue[Array.IndexOf(FormInputName, "UsePublickEmail")] == "false" ? false : true;
                        TableButton.EnableSsl = FormInputValue[Array.IndexOf(FormInputName, "EnableSsl")] == "false" ? false : true;
                        TableButton.SendReport = FormInputValue[Array.IndexOf(FormInputName, "SendReport")].Split(',');
                        TableButton.Title = FormInputValue[Array.IndexOf(FormInputName, "Title")];
                        TableButton.TitleQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_TitleQuery")];
                        TableButton.BodyMessage = FormInputValue[Array.IndexOf(FormInputName, "BodyMessage")];
                        TableButton.BodyMessageQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_BodyMessageQuery")];

                        Value = Tools.ToXML(TableButton);
                        break;
                    }

                case "TableFunction":
                    {
                        TableFunction TableFunction = new TableFunction();
                        TableFunction.ReturnDataType = FormInputValue[Array.IndexOf(FormInputName, "ReturnDataType")];
                        TableFunction.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        Value = Tools.ToXML(TableFunction);

                        CoreObject TableFunctionCore = CoreObject.Find(SysSetting.SysSettingID);
                        List<CoreObject> ParameterFunction = CoreObject.FindChilds(TableFunctionCore.CoreObjectID, CoreDefine.Entities.پارامتر_تابع);
                        string Query = "ALTER FUNCTION " + TableFunctionCore.FullName + "(";

                        foreach (CoreObject param in ParameterFunction)
                        {
                            ParameterTableFunction parameterTableFunction = new ParameterTableFunction(param);
                            Query += "@" + param.FullName + " AS " + parameterTableFunction.ParameterDataType + ",";
                        }
                        if (ParameterFunction.Count > 0)
                            Query = Query.Substring(0, Query.Length - 1);

                        Query += ") RETURNS " + TableFunction.ReturnDataType + " AS \nBEGIN\n" + Tools.ConvertToSQLQuery(TableFunction.Query) + "\nEND";
                        DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(TableFunctionCore.ParentID));
                        Desktop.ExecuteQuery(dataSourceInfo, Query);
                        break;
                    }
                case "ShowFieldEvent":
                    {
                        ShowFieldEvent ShowFieldEvent = new ShowFieldEvent();

                        ShowFieldEvent.FieldValue = FormInputValue[Array.IndexOf(FormInputName, "FieldValue")];
                        ShowFieldEvent.ShowObject = FormInputValue[Array.IndexOf(FormInputName, "ShowObject")] == "false" ? false : true;
                        ShowFieldEvent.SelectedObjectID = FormInputValue[Array.IndexOf(FormInputName, "SelectedObjectID")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "SelectedObjectID")]);
                        Value = Tools.ToXML(ShowFieldEvent);
                        break;
                    }

                case "DataSource":
                    {
                        DataSourceInfo DataSource = new DataSourceInfo();

                        DataSource.UserName = FormInputValue[Array.IndexOf(FormInputName, "UserName")];
                        DataSource.Password = FormInputValue[Array.IndexOf(FormInputName, "Password")];
                        DataSource.DataBase = FormInputValue[Array.IndexOf(FormInputName, "DataBase")];
                        DataSource.ServerName = FormInputValue[Array.IndexOf(FormInputName, "ServerName")];
                        DataSource.FilePath = FormInputValue[Array.IndexOf(FormInputName, "FilePath")];
                        DataSource.DataSourceType = Tools.GetDataSourceType(FormInputValue[Array.IndexOf(FormInputName, "DataSourceType")]);
                        Value = Tools.ToXML(DataSource);
                        break;
                    }

                case "ComputationalField":
                    {
                        
                        ComputationalField ComputationalField = new ComputationalField();

                        ComputationalField.FieldType = Tools.GetInputType(FormInputValue[Array.IndexOf(FormInputName, "FieldType")]);
                        ComputationalField.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];
                        ComputationalField.ShowInForm = FormInputValue[Array.IndexOf(FormInputName, "ShowInForm")] == "false" ? false : true;
                        ComputationalField.IsDefaultView = FormInputValue[Array.IndexOf(FormInputName, "IsDefaultView")] == "false" ? false : true;
                        ComputationalField.IsWide = FormInputValue[Array.IndexOf(FormInputName, "IsWide")] == "false" ? false : true;
                        ComputationalField.MinValue = FormInputValue[Array.IndexOf(FormInputName, "MinValue")] == "" ? 0 : float.Parse(FormInputValue[Array.IndexOf(FormInputName, "MinValue")]);
                        ComputationalField.MaxValue = FormInputValue[Array.IndexOf(FormInputName, "MaxValue")] == "" ? 0 : float.Parse(FormInputValue[Array.IndexOf(FormInputName, "MaxValue")]);
                        ComputationalField.DigitsAfterDecimal = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "DigitsAfterDecimal")]);
                        ComputationalField.FieldComment = FormInputValue[Array.IndexOf(FormInputName, "FieldComment")];
                        ComputationalField.DisplayName = FormInputValue[Array.IndexOf(FormInputName, "DisplayName")];

                        CoreObject ComputationalFieldObject = CoreObject.Find(SysSetting.SysSettingID);
                        List<CoreObject> FieldObjectList = CoreObject.FindChilds(ComputationalFieldObject.ParentID, CoreDefine.Entities.فیلد);
                        foreach (CoreObject FieldObject in FieldObjectList)
                        {
                            if (ComputationalField.Query.IndexOf(FieldObject.FullName) > -1)
                            {
                                Field Field = new Field(FieldObject);
                                Field.ActiveOnKeyDown = true;
                                Value = Tools.ToXML(Field);
                                Referral.DBCore.UpdateRow(FieldObject.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value });
                                int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == FieldObject.CoreObjectID);
                                Referral.CoreObjects[FieldIndex].Value = Value;
                            }
                        }
                        Value = Tools.ToXML(ComputationalField);
                        break;
                    }

                case "Dashboard":
                    {
                        Dashboard Dashboard = new Dashboard();
                        Dashboard.InformationEntryForm = FormInputValue[Array.IndexOf(FormInputName, "InformationEntryForm")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "InformationEntryForm")]);
                        Dashboard.StartDate = FormInputValue[Array.IndexOf(FormInputName, "StartDate")];
                        Dashboard.EndDate = FormInputValue[Array.IndexOf(FormInputName, "EndDate")];
                        Dashboard.ShowDate = FormInputValue[Array.IndexOf(FormInputName, "ShowDate")] == "false" ? false : true;

                        Value = Tools.ToXML(Dashboard);
                        break;
                    }

                case "SubDashboard":
                    {
                        SubDashboard SubDashboard = new SubDashboard();
                        SubDashboard.ChartTypes = Tools.GetChartTypes(FormInputValue[Array.IndexOf(FormInputName, "ChartTypes")]);
                        SubDashboard.ChartCalculationType = Tools.GetChartCalculationType(FormInputValue[Array.IndexOf(FormInputName, "ChartCalculationType")]);
                        SubDashboard.ChartGroupDateType = Tools.GetChartGroupDate(FormInputValue[Array.IndexOf(FormInputName, "ChartGroupDateType")]);
                        SubDashboard.CategoryAxisQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_CategoryAxisQuery")];
                        SubDashboard.GroupByQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_GroupByQuery")];
                        SubDashboard.OrderByQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_OrderByQuery")];
                        SubDashboard.Title = FormInputValue[Array.IndexOf(FormInputName, "Title")];
                        SubDashboard.BackgroundColor = FormInputValue[Array.IndexOf(FormInputName, "BackgroundColor")];
                        SubDashboard.TextColor = FormInputValue[Array.IndexOf(FormInputName, "TextColor")];
                        SubDashboard.Icon = FormInputValue[Array.IndexOf(FormInputName, "Icon")];
                        SubDashboard.Theme = FormInputValue[Array.IndexOf(FormInputName, "Theme")];
                        SubDashboard.Condition = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Condition")]; 
                        SubDashboard.IsWide = FormInputValue[Array.IndexOf(FormInputName, "IsWide")] == "false" ? false : true;
                        SubDashboard.GroupField = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "GroupField")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "GroupField")]);
                        SubDashboard.DateField = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "DateField")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "DateField")]);
                        SubDashboard.ReloadTime = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "ReloadTime")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ReloadTime")]);
                        SubDashboard.ColumnSpan = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "ColumnSpan")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "ColumnSpan")]);
                        SubDashboard.RowSpan = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "RowSpan")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "RowSpan")]);
                        SubDashboard.MaxValue = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "MaxValue")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "MaxValue")]);
                        SubDashboard.MinValue = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "MinValue")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "MinValue")]);
                        SubDashboard.IndicatorValue = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "IndicatorValue")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "IndicatorValue")]);
                        SubDashboard.InformationEntryForm = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "InformationEntryForm")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "InformationEntryForm")]);
                        SubDashboard.CalculationField = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "CalculationField")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "CalculationField")]);

                        Value = Tools.ToXML(SubDashboard);
                        break;
                    }

                case "DashboardIntegration":
                    {
                        DashboardIntegration DashboardIntegration = new DashboardIntegration();
                        DashboardIntegration.Condition = FormInputValue[Array.IndexOf(FormInputName, "Condition")];
                        DashboardIntegration.CategoryAxisQuery = FormInputValue[Array.IndexOf(FormInputName, "CategoryAxisQuery")];
                        DashboardIntegration.GroupField = long.Parse(FormInputValue[Array.IndexOf(FormInputName, "GroupField")] == "" ? "0" : FormInputValue[Array.IndexOf(FormInputName, "GroupField")]);
                        DashboardIntegration.ChartCalculationType = Tools.GetChartCalculationType(FormInputValue[Array.IndexOf(FormInputName, "ChartCalculationType")]);

                        Value = Tools.ToXML(DashboardIntegration);
                        break;
                    }

                case "ProcessStepEvent":
                    {
                        ProcessStepEvent ProcessStepEvent = new ProcessStepEvent();
                        ProcessStepEvent.Command = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Command")];

                        Value = Tools.ToXML(ProcessStepEvent);
                        break;
                    }

                case "ProcessStep":
                    {
                        ProcessStep ProcessStep = new ProcessStep(CoreObject.Find(SysSettingID)); 
                        switch(ProcessStep.ActionType)
                        {
                            case CoreDefine.ProcessStepActionType.پایان:
                            case CoreDefine.ProcessStepActionType.شروع:
                            case CoreDefine.ProcessStepActionType.عملیات:
                                {
                                    ProcessStep.InformationEntryFormID = FormInputValue[Array.IndexOf(FormInputName, "InformationEntryFormID")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "InformationEntryFormID")]);
                                    ProcessStep.RecordType = Tools.GetStepRecordType(FormInputValue[Array.IndexOf(FormInputName, "RecordType")]);
                                    break;
                                }
                            case CoreDefine.ProcessStepActionType.SendTask:
                                {
                                    ProcessStep.EMail = FormInputValue[Array.IndexOf(FormInputName, "EMail")];
                                    ProcessStep.EMailUserName = FormInputValue[Array.IndexOf(FormInputName, "EMailUserName")];
                                    ProcessStep.EMailPassWord = FormInputValue[Array.IndexOf(FormInputName, "EMailPassWord")];
                                    ProcessStep.EMailPort = FormInputValue[Array.IndexOf(FormInputName, "EMailPort")];
                                    ProcessStep.EMailServer = FormInputValue[Array.IndexOf(FormInputName, "EMailServer")];

                                    ProcessStep.ReceivingUsers = FormInputValue[Array.IndexOf(FormInputName, "ReceivingUsers")].Split(',');
                                    ProcessStep.ReceivingRole = FormInputValue[Array.IndexOf(FormInputName, "ReceivingRole")].Split(',');
                                    ProcessStep.InsertingUser = FormInputValue[Array.IndexOf(FormInputName, "InsertingUser")] == "false" ? false : true;
                                    ProcessStep.ReceivingQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ReceivingQuery")];
                                    ProcessStep.SendAttachmentFile = FormInputValue[Array.IndexOf(FormInputName, "SendAttachmentFile")] == "false" ? false : true;
                                    ProcessStep.UsePublickEmail = FormInputValue[Array.IndexOf(FormInputName, "UsePublickEmail")] == "false" ? false : true;
                                    ProcessStep.EnableSsl = FormInputValue[Array.IndexOf(FormInputName, "EnableSsl")] == "false" ? false : true;
                                    ProcessStep.SendReport = FormInputValue[Array.IndexOf(FormInputName, "SendReport")].Split(',');
                                    ProcessStep.Title = FormInputValue[Array.IndexOf(FormInputName, "Title")];
                                    ProcessStep.TitleQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_TitleQuery")];
                                    ProcessStep.BodyMessage = FormInputValue[Array.IndexOf(FormInputName, "BodyMessage")];
                                    ProcessStep.BodyMessageQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_BodyMessageQuery")];
                                    break;
                                }
                        }

                        Value = Tools.ToXML(ProcessStep);
                        break;
                    }

                case "BpmnLane":
                    {
                        BpmnLane bpmnLane = new BpmnLane(CoreObject.Find(SysSettingID));  
                        bpmnLane.Personnel = FormInputValue[Array.IndexOf(FormInputName, "Personnel")].Split(','); 
                        bpmnLane.OrganizationLevel = FormInputValue[Array.IndexOf(FormInputName, "Organizationlevel")].Split(',');
                        bpmnLane.Query = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_Query")];  
                        Value = Tools.ToXML(bpmnLane);
                        break;
                    }

                case "Field":
                    {
                        CoreObject Fieldcore = CoreObject.Find(SysSetting.SysSettingID);
                        Field OldField = new Field(Fieldcore);

                        Field Field = new Field();
                        Field.FieldName = Fieldcore.FullName; 
                        Field.DisplayName = FormInputValue[Array.IndexOf(FormInputName, "DisplayName")];
                        Field.RelatedLink = FormInputValue[Array.IndexOf(FormInputName, "RelatedLink")];
                        Field.ColumnWidth = FormInputValue[Array.IndexOf(FormInputName, "ColumnWidth")];
                        Field.FieldType = Tools.GetInputType(FormInputValue[Array.IndexOf(FormInputName, "FieldType")]);
                        Field.FieldDisplayType = Tools.GetFieldDisplayType(FormInputValue[Array.IndexOf(FormInputName, "FieldDisplayType")]);
                        Field.Folder = Fieldcore.Folder;
                        Field.IsEditAble = FormInputValue[Array.IndexOf(FormInputName, "IsEditAble")] == "false" ? false : true;
                        Field.IsDefaultView = FormInputValue[Array.IndexOf(FormInputName, "IsDefaultView")] == "false" ? false : true;
                        Field.IsRequired = FormInputValue[Array.IndexOf(FormInputName, "IsRequired")] == "false" ? false : true;
                        Field.ShowInForm = FormInputValue[Array.IndexOf(FormInputName, "ShowInForm")] == "false" ? false : true;
                        Field.AutoFillQuery = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_AutoFillQuery")];
                        Field.ViewCommand = FormInputValue[Array.IndexOf(FormInputName, "PersianQuery_ViewCommand")];
                        Field.SpecialValue = FormInputValue[Array.IndexOf(FormInputName, "SpecialValue")];
                        Field.DefaultValue = FormInputValue[Array.IndexOf(FormInputName, "DefaultValue")];
                        Field.Class = FormInputValue[Array.IndexOf(FormInputName, "Class")];
                        Field.TextColor = FormInputValue[Array.IndexOf(FormInputName, "TextColor")];
                        Field.GridTextColor = FormInputValue[Array.IndexOf(FormInputName, "GridTextColor")];
                        Field.FieldComment = FormInputValue[Array.IndexOf(FormInputName, "FieldComment")];
                        Field.IsLeftWrite = FormInputValue[Array.IndexOf(FormInputName, "IsLeftWrite")] == "false" ? false : true;
                        Field.ClearAfterChange = FormInputValue[Array.IndexOf(FormInputName, "ClearAfterChange")] == "false" ? false : true; 
                        Field.IsExclusive = FormInputValue[Array.IndexOf(FormInputName, "IsExclusive")] == "false" ? false : true;
                        Field.ActiveOnKeyDown = FormInputValue[Array.IndexOf(FormInputName, "ActiveOnKeyDown")] == "false" ? false : true;
                        Field.SaveAndNewForm = FormInputValue[Array.IndexOf(FormInputName, "SaveAndNewForm")] == "false" ? false : true;
                        Field.RelatedField = FormInputValue[Array.IndexOf(FormInputName, "RelatedField")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedField")]);
                        Field.RelatedFieldCommand = FormInputValue[Array.IndexOf(FormInputName, "RelatedFieldCommand")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedFieldCommand")]);
                        Field.RelatedTable = FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")] == "" ? 0 : long.Parse(FormInputValue[Array.IndexOf(FormInputName, "RelatedTable")]);
                        Field.MinValue = FormInputValue[Array.IndexOf(FormInputName, "MinValue")] == "" ? 0 : float.Parse(FormInputValue[Array.IndexOf(FormInputName, "MinValue")]);
                        Field.MaxValue = FormInputValue[Array.IndexOf(FormInputName, "MaxValue")] == "" ? 0 : float.Parse(FormInputValue[Array.IndexOf(FormInputName, "MaxValue")]);
                        Field.DigitsAfterDecimal = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "DigitsAfterDecimal")]);
                        Field.SearchAutoCompleteCount = int.Parse(FormInputValue[Array.IndexOf(FormInputName, "SearchAutoCompleteCount")]);
                        Field.AnalystDescription = FormInputValue[Array.IndexOf(FormInputName, "AnalystDescription")];


                        if (Field.RelatedField>0)
                        {
                            Field RelatedField = new Field (CoreObject.Find(Field.RelatedField));
                            if(RelatedField.ActiveOnKeyDown)
                            {
                                RelatedField.ActiveOnKeyDown = true;
                                Value = Tools.ToXML(RelatedField);
                                if (Referral.DBCore.UpdateRow(Field.RelatedField, 0, "CoreObject", new string[] { "Value" }, new object[] { Value }))
                                {
                                    int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == Field.RelatedField);
                                    Referral.CoreObjects[FieldIndex].Value = Value;
                                }
                            }
                        }

                    switch (Field.FieldType)
                        {
                            case CoreDefine.InputTypes.Clock:
                            case CoreDefine.InputTypes.NationalCode:
                            case CoreDefine.InputTypes.PersianDateTime:
                            case CoreDefine.InputTypes.MiladyDateTime: 
                            case CoreDefine.InputTypes.ShortText:
                            case CoreDefine.InputTypes.FillTextAutoComplete:
                            case CoreDefine.InputTypes.ComboBox:
                            case CoreDefine.InputTypes.Password:
                            case CoreDefine.InputTypes.Phone:
                            case CoreDefine.InputTypes.Plaque:
                            case CoreDefine.InputTypes.Rating:
                            case CoreDefine.InputTypes.MultiSelectFromRelatedTable:
                            case CoreDefine.InputTypes.MultiSelectFromComboBox:
                                {
                                    Field.FieldNature = "Nvarchar(400)";
                                    break;
                                } 
                            case CoreDefine.InputTypes.LongText:
                            case CoreDefine.InputTypes.Editor:
                                { 
                                    Field.FieldNature = "Nvarchar(MAX)";
                                    break;
                                }
                            case CoreDefine.InputTypes.TwoValues:
                                { 
                                    Field.FieldNature = "Bit";
                                    break;
                                }
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                            case CoreDefine.InputTypes.Money:
                            case CoreDefine.InputTypes.CoreRelatedTable:
                            case CoreDefine.InputTypes.Sparkline:
                                { 
                                    Field.FieldNature = "Bigint";
                                    if(Field.DigitsAfterDecimal>0)
                                        Field.FieldNature = "Float"; 
                                    break;
                                }
                        }

                        if (!string.IsNullOrEmpty(Field.AutoFillQuery))
                        {
                            List<CoreObject> FieldObjectList = CoreObject.FindChilds(Fieldcore.ParentID, CoreDefine.Entities.فیلد);
                            foreach (CoreObject FieldObject in FieldObjectList)
                            {
                                if (Field.AutoFillQuery.IndexOf("@" + FieldObject.FullName) > -1)
                                {
                                    Field FieldItem = new Field(FieldObject);
                                    if (!FieldItem.ActiveOnKeyDown)
                                    {
                                        FieldItem.ActiveOnKeyDown = true;
                                        Value = Tools.ToXML(FieldItem);
                                        if (Referral.DBCore.UpdateRow(FieldObject.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value }))
                                        {
                                            int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == FieldObject.CoreObjectID);
                                            Referral.CoreObjects[FieldIndex].Value = Value;
                                        }
                                    }
                                }
                            }
                        }

                        Value = Tools.ToXML(Field);

                        CoreObject ParentCore=CoreObject.Find(Fieldcore.ParentID);
                        if(ParentCore.Entity== CoreDefine.Entities.جدول)
                            if (OldField.FieldNature != Field.FieldNature)
                        {
                            CoreObject Table = CoreObject.Find(Fieldcore.ParentID);

                            if(Table.Entity== CoreDefine.Entities.جدول)
                            {
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(Table.ParentID));

                                switch (DataSourceInfo.DataSourceType)
                                {
                                    case CoreDefine.DataSourceType.SQLSERVER:
                                        {
                                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                            if (!DataBase.Execute("ALTER TABLE " + Table.FullName + " ALTER COLUMN " + Fieldcore.FullName + " " + Field.FieldNature))
                                            {
                                                Field.FieldNature = OldField.FieldNature;
                                            }

                                            break;
                                        }
                                    case CoreDefine.DataSourceType.MySql:
                                        {
                                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                            DataBase.Execute("ALTER TABLE " + Table.FullName + " ALTER COLUMN " + Fieldcore.FullName + " " + Field.FieldNature);
                                            break;
                                        }
                                    case CoreDefine.DataSourceType.ACCESS:
                                        {
                                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                            string FieldType = "";
                                            switch (Field.FieldNature)
                                            {
                                                case "Bigint":
                                                    {
                                                        FieldType = "long";
                                                        break;
                                                    }
                                                case "Bit":
                                                    {
                                                        FieldType = "Bit";
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        FieldType = "text";
                                                        break;
                                                    }
                                            }

                                            if (DataBase.Execute("ALTER TABLE [" + Table.FullName + "] Add Column  APM_TempColumn " + FieldType))
                                                if (DataBase.Execute("UPDATE [" + Table.FullName + "] SET APM_TempColumn = [" + Fieldcore.FullName + "]"))
                                                    if (DataBase.Execute("Alter table [" + Table.FullName + "] drop column [" + Fieldcore.FullName + "]"))
                                                    {
                                                        if (DataBase.Execute("ALTER TABLE [" + Table.FullName + "] Add Column  [" + Fieldcore.FullName + "]  " + FieldType))
                                                            if (DataBase.Execute("UPDATE [" + Table.FullName + "] SET  [" + Fieldcore.FullName + "] = APM_TempColumn"))
                                                                DataBase.Execute("Alter table [" + Table.FullName + "] drop column APM_TempColumn");
                                                    }
                                                    else
                                                        DataBase.Execute("Alter table [" + Table.FullName + "] drop column [APM_TempColumn]");
                                            break;
                                        }
                                }

                            }
                        }
                    }
                    break;
            }

            if (Referral.DBCore.UpdateRow(SysSetting.SysSettingID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value }))
            {
                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == SysSetting.SysSettingID);
                Referral.CoreObjects[CoreIndex].Value = Value;
            } 
            return Json(1);
        }

        public JsonResult SaveCoreObject(long ParentID, string Entity, string Folder, string FullName, string OrderIndex)
        {
            CoreDefine.Entities EntityType = CoreDefine.Entities.جدول;
            string Value = "";

            if(Entity.IndexOf("Folder")>-1)
            { 
                Value = Tools.ToXML(new Folder());
                EntityType = CoreDefine.Entities.پوشه;
                FullName = Entity.Split('_')[1] + "_" + FullName;
            }
            else
            {
                switch (Entity)
                {
                    case "InformationEntryForm":
                        {
                            Value = Tools.ToXML(new InformationEntryForm());
                            EntityType = CoreDefine.Entities.فرم_ورود_اطلاعات;
                            break;
                        }
                    case "GridRowColor":
                        {
                            Value = Tools.ToXML(new GridRowColor());
                            EntityType = CoreDefine.Entities.رنگ_سطر_جدول;
                            break;
                        }
                    case "NewButtonForm":
                        {
                            Value = Tools.ToXML(new NewButtonForm());
                            EntityType = CoreDefine.Entities.فرم_دکمه_جدید;
                            break;
                        }
                    case "WebService":
                        {
                            Value = Tools.ToXML(new WebService());
                            EntityType = CoreDefine.Entities.وب_سرویس;
                            break;
                        }
                    case "WebServiceParameter":
                        {
                            Value = Tools.ToXML(new WebServiceParameter());
                            EntityType = CoreDefine.Entities.پارامتر_وب_سرویس;
                            break;
                        }
                    case "SearchForm":
                        {
                            Value = Tools.ToXML(new SearchForm());
                            EntityType = CoreDefine.Entities.فرم_جستجو;
                            break;
                        }
                    case "SearchField":
                        {
                            Value = Tools.ToXML(new SearchField());
                            EntityType = CoreDefine.Entities.فیلد_جستجو;
                            break;
                        }
                    case "PublicJob":
                        {
                            Value = Tools.ToXML(new PublicJob());
                            EntityType = CoreDefine.Entities.رویداد_عمومی;
                            break;
                        }
                    case "SpecialPhrase":
                        {
                            Value = Tools.ToXML(new SpecialPhrase());
                            EntityType = CoreDefine.Entities.عبارت_ویژه;
                            break;
                        }
                    case "Process":
                        {
                            Value = Tools.ToXML(new ProcessType());
                            EntityType = CoreDefine.Entities.فرآیند;
                            break;
                        }
                    case "TableButton":
                        {
                            Value = Tools.ToXML(new TableButton());
                            EntityType = CoreDefine.Entities.دکمه_رویداد_جدول;
                            break;
                        }
                    case "ProcessStep":
                        {
                            Value = Tools.ToXML(new ProcessStep());
                            EntityType = CoreDefine.Entities.مرحله_فرآیند;
                            break;
                        }
                    case "ProcessReferral":
                        {
                            Value = Tools.ToXML(new ProcessReferral());
                            EntityType = CoreDefine.Entities.ارجاع_مرحله_فرآیند;
                            break;
                        }
                    case "ProcessStepEvent":
                        {
                            EntityType = CoreDefine.Entities.رویداد_مرحله_فرآیند;
                            Value = Tools.ToXML(new ProcessStepEvent());
                            break;
                        }
                    case "DisplayField":
                        {
                            Value = Tools.ToXML(new DisplayField());
                            EntityType = CoreDefine.Entities.فیلد_نمایشی;
                            break;
                        }
                    case "TableAttachment":
                        {
                            Value = Tools.ToXML(new TableAttachment());
                            EntityType = CoreDefine.Entities.ضمیمه_جدول;
                            break;
                        }
                    case "TableEvent":
                        {
                            Value = Tools.ToXML(new TableEvent());
                            EntityType = CoreDefine.Entities.رویداد_جدول;
                            break;
                        }
                    case "DataSource":
                        {
                            Value = Tools.ToXML(new DataSourceInfo());
                            EntityType = CoreDefine.Entities.پایگاه_داده;
                            break;
                        }
                    case "Dashboard":
                        {
                            Value = Tools.ToXML(new Dashboard());
                            EntityType = CoreDefine.Entities.داشبورد;
                            break;
                        }
                    case "SubDashboard":
                        {
                            Value = Tools.ToXML(new SubDashboard());
                            EntityType = CoreDefine.Entities.زیر_بخش_داشبورد;
                            break;
                        }
                    case "DashboardIntegration":
                        {
                            Value = Tools.ToXML(new DashboardIntegration());
                            EntityType = CoreDefine.Entities.ادغام_داشبورد;
                            break;
                        }
                    case "ReportParameter":
                        {
                            Value = Tools.ToXML(new ReportParameter());
                            EntityType = CoreDefine.Entities.پارامتر_گزارش;
                            break;
                        }
                    case "Report":
                        {
                            EntityType = CoreDefine.Entities.گزارش;
                            break;
                        }
                    case "ComputationalField":
                        {
                            Value = Tools.ToXML(new ComputationalField());
                            EntityType = CoreDefine.Entities.فیلد_محاسباتی;
                            break;
                        }
                    case "Field":
                        {
                            Value = Tools.ToXML(new Field());
                            EntityType = CoreDefine.Entities.فیلد;
                            break;
                        }
                    case "ShowFieldEvent":
                        {
                            Value = Tools.ToXML(new ShowFieldEvent());
                            EntityType = CoreDefine.Entities.رویداد_نمایش_فیلد;
                            break;
                        }
                    case "Table":
                        {
                            Value = Tools.ToXML(new Table());
                            EntityType = CoreDefine.Entities.جدول;
                            break;
                        }
                    case "TableFunction":
                        {
                            Value = Tools.ToXML(new TableFunction());
                            EntityType = CoreDefine.Entities.تابع_جدولی;
                            break;
                        }
                    case "PublicFile":
                        {
                            Value = Tools.ToXML(new PublicFile());
                            EntityType = CoreDefine.Entities.فایل_عمومی;
                            break;
                        }
                    case "ParameterTableFunction":
                        {
                            Value = Tools.ToXML(new ParameterTableFunction());
                            EntityType = CoreDefine.Entities.پارامتر_تابع;
                            break;
                        }
                }

            }

            int CoreIndex = Referral.CoreObjects.FindIndex(x => (x.ParentID == ParentID && x.Entity == EntityType && x.FullName == Tools.SafeTitle(FullName)));
            if (CoreIndex > -1)
                return Json("نام تکراری می باشد");


            long ID = 0;
            Folder = Folder.Length > 1 ? (Folder.Substring(0, 1) == " " ? Folder.Substring(1, Folder.Length - 1) : Folder) : Folder;
            DataSourceInfo DataSourceInfo = new DataSourceInfo();

            switch (Entity)
            {
                case "ParameterTableFunction":
                    {
                        ID = Referral.DBCore.Insert("CoreObject"
                           , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                           , new object[] { ParentID, EntityType.ToString(), Folder, Tools.SafeTitle(FullName), OrderIndex, 0, Value });
                        CoreObject TableCore =CoreObject.Find(ParentID);
                        TableFunction tableFunction = new TableFunction(TableCore);
                        DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                        List<CoreObject> CoreList = CoreObject.FindChilds(TableCore.CoreObjectID);
                        string Query = "ALTER FUNCTION dbo." + TableCore.FullName + "(";
                        foreach (CoreObject coreObject in CoreList)
                        {
                            ParameterTableFunction parameterTableFunction = new ParameterTableFunction(coreObject);
                            Query += "@" + parameterTableFunction.FullName + " " + parameterTableFunction.ParameterDataType.ToString() + ",\n";
                        }
                        ParameterTableFunction parameterFunction = new ParameterTableFunction();
                        Query += "@" + Tools.SafeTitle(FullName) + " " + parameterFunction.ParameterDataType.ToString() + ")RETURNS "+ tableFunction.ReturnDataType + " AS BEGIN \n"+ tableFunction.Query+ "\n END";
                        Desktop.ExecuteQuery(dataSourceInfo, Query);
                        break;
                    }
                case "TableFunction":
                    {
                        ID = Referral.DBCore.Insert("CoreObject"
                           , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                           , new object[] { ParentID, CoreDefine.Entities.تابع_جدولی.ToString(), Folder, Tools.SafeTitle(FullName), OrderIndex, 0, Value });
                        DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(ParentID));
                        Desktop.ExecuteQuery(dataSourceInfo, "CREATE FUNCTION " + Tools.SafeTitle(FullName) + "()RETURNS BIGINT AS BEGIN RETURN 0 END");
                        break;
                    }
                case "Report":
                    {
                        Report CoreReport = new Report();
                        Value = Tools.ToXML(CoreReport);
                        ID = Referral.DBCore.Insert("CoreObject"
                           , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                           , new object[] { ParentID, CoreDefine.Entities.گزارش.ToString(), Folder, Tools.SafeTitle(FullName), OrderIndex, 0, Value });


                        StiReport report = new StiReport();
                        report.Dictionary.Databases.Add(new StiSqlDatabase("ارتباط", Referral.DBData.ConnectionData.ConnectionString));
                        string jstr = report.SaveToJsonString();
                        string ReportResourceValue = Tools.ToXML(new ReportResource(jstr));


                        long ChildId = Referral.DBCore.Insert("CoreObject"
                           , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                           , new object[] { ID, CoreDefine.Entities.منبع_گزارش.ToString(), Folder, "", OrderIndex, 0, ReportResourceValue });


                        Referral.CoreObjects.Add(new CoreObject(ChildId, ID, CoreDefine.Entities.منبع_گزارش, Folder, "", long.Parse(OrderIndex), false, ReportResourceValue));
                        break;
                    }

                case "Field":
                    {
                        ID = CreateField(ParentID, Folder, FullName, long.Parse(OrderIndex), Value);

                        break;
                    }
                case "Table":
                    {
                        ID = CreateCoreTable(ParentID, Folder, FullName, long.Parse(OrderIndex), Value);

                        break;
                    }
                default:
                    {
                        ID = Referral.DBCore.Insert("CoreObject"
                           , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                           , new object[] { ParentID, EntityType.ToString(), Folder, Tools.SafeTitle(FullName), OrderIndex, 0, Value });
                        break;
                    }

            }
            if (ID > 0)
                Referral.CoreObjects.Add(new CoreObject(ID, ParentID, EntityType, Folder, Tools.SafeTitle(FullName), long.Parse(OrderIndex), false, Value));

            return Json(ID);
        }

        private long CreateCoreTable(long DataSourceInfoID, string Folder, string FullName, long OrderIndex, string Value)
        {

            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(DataSourceInfoID));
            long ID = Referral.DBCore.Insert("CoreObject"
               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
               , new object[] { DataSourceInfoID, CoreDefine.Entities.جدول.ToString(), Folder, Tools.SafeTitle(FullName), OrderIndex, 0, Value });

            string FieldValue = Tools.ToXML(new Field("شناسه", "شناسه", "Bigint", CoreDefine.InputTypes.Number, "عمومی", false, false, false, "", "", "", false, true, 0, false, 0, float.MaxValue, false, true));
            int FieldID = Referral.DBCore.Insert("CoreObject"
               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
               , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", "شناسه", 1, 1, FieldValue });

            Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", "شناسه", 1, true, FieldValue));


            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                        DataBase.CreateTableWithIdentityField(Tools.SafeTitle(FullName));

                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        //DataBase.CreateTableWithIdentityField(Tools.SafeTitle(FullName));
                        break;
                    }

                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        DataBase.CreateTableWithoutRegitry(Tools.SafeTitle(FullName), new string[] { "شناسه" }, new string[] { "AUTOINCREMENT" });

                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            IWorkbook NewWoorkbook = application.Workbooks.Create(1);
                            IWorksheet NewWorksheet = NewWoorkbook.Worksheets[0];

                            IWorksheet worksheet = workbook.Worksheets.AddCopy(NewWorksheet);
                            worksheet.Name = Tools.SafeTitle(FullName);
                            worksheet.ClearData();
                            worksheet.Range["A1"].Text = "شناسه";

                            workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                        }
                        break;
                    }
            }
            return ID;
        }

        private long CreateField(long ParentID, string Folder, string FullName, long OrderIndex, string Value)
        {
            long ID = 0;
            bool IsInsert=false;


            CoreObject TableCore = CoreObject.Find(ParentID);
            if (TableCore.Entity== CoreDefine.Entities.جدول)
            { 
                Table TableInfo = new Table(TableCore);
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));

                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            IsInsert = DataBase.Execute("ALTER TABLE [" + TableInfo.TABLESCHEMA + "].[" + TableCore.FullName + "] ADD  " + Tools.SafeTitle(FullName) + " Nvarchar(400)");

                            break;
                        }
                    case CoreDefine.DataSourceType.MySql:
                        {
                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                            IsInsert = DataBase.Execute("ALTER TABLE [" + TableCore.FullName + "] ADD  " + Tools.SafeTitle(FullName) + " Nvarchar(400)");
                            break;
                        }

                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                            IsInsert = DataBase.Execute("ALTER TABLE [" + TableCore.FullName + "] Add Column  " + Tools.SafeTitle(FullName) + " text");

                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            using (ExcelEngine excelEngine = new ExcelEngine())
                            {
                                IApplication application = excelEngine.Excel;

                                application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                IWorksheet worksheet = workbook.Worksheets[TableCore.FullName];
                                worksheet.Range[1, worksheet.Columns.Count() + 1].Text = Tools.SafeTitle(FullName);

                                workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                IsInsert = true;
                            }
                            break;
                        }
                }
                if (IsInsert)
                    ID = Referral.DBCore.Insert("CoreObject"
                   , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                   , new object[] { ParentID, CoreDefine.Entities.فیلد.ToString(), Folder, Tools.SafeTitle(FullName), OrderIndex, 0, Value });
            }
 

            return ID;
        }
        public JsonResult RenameCore(long CoreObjectID, string FullName)
        {
            CoreObject _Object = CoreObject.Find(CoreObjectID);
            DataSourceInfo DataSourceInfo = new DataSourceInfo();
            bool result = false;

            switch (_Object.Entity)
            {
                case CoreDefine.Entities.فیلد:
                    CoreObject Table = CoreObject.Find(_Object.ParentID);

                    DataSourceInfo = new DataSourceInfo(CoreObject.Find(Table.ParentID));
                    switch (DataSourceInfo.DataSourceType)
                    {
                        case CoreDefine.DataSourceType.SQLSERVER:
                            {
                                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                if (DataBase.Execute("EXEC sp_rename N'" + Table.FullName + "." + _Object.FullName + "', N'" + Tools.SafeTitle(FullName) + "', 'COLUMN';"))
                                    result = true;

                                break;
                            }
                        case CoreDefine.DataSourceType.MySql:
                            {
                                MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                DataBase.Execute("EXEC sp_rename N'" + Table.FullName + "." + _Object.FullName + "', N'" + Tools.SafeTitle(FullName) + "', 'COLUMN';");
                                break;
                            }

                        case CoreDefine.DataSourceType.ACCESS:
                            {
                                AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                Field field = new Field(_Object);
                                string FieldType = "";
                                switch (field.FieldNature)
                                {
                                    case "Bigint":
                                        {
                                            FieldType = "long";
                                            break;
                                        }
                                    case "Bit":
                                        {
                                            FieldType = "Bit";
                                            break;
                                        }
                                    default:
                                        {
                                            FieldType = "text";
                                            break;
                                        }
                                }
                                if (DataBase.Execute("ALTER TABLE [" + Table.FullName + "] Add Column  " + Tools.SafeTitle(FullName) + " " + FieldType))
                                    if (DataBase.Execute("UPDATE [" + Table.FullName + "] SET " + Tools.SafeTitle(FullName) + " = [" + _Object.FullName + "]"))
                                        if (DataBase.Execute("Alter table [" + Table.FullName + "] drop column [" + _Object.FullName + "]"))
                                            result = true;
                                        else
                                            DataBase.Execute("Alter table [" + Table.FullName + "] drop column [" + Tools.SafeTitle(FullName) + "]");

                                break;
                            }
                        case CoreDefine.DataSourceType.EXCEL:
                            {
                                try
                                {
                                    using (ExcelEngine excelEngine = new ExcelEngine())
                                    {
                                        IApplication application = excelEngine.Excel;

                                        application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                        IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                        IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];
                                        foreach (IRange Column in worksheet.Columns)
                                        {
                                            if (Column.DisplayText == _Object.FullName)
                                            {
                                                Column[1, Column.Column].Text = Tools.SafeTitle(FullName);
                                                break;
                                            }
                                        }
                                        workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                    }
                                    result = true;
                                }
                                catch (Exception ex)
                                {
                                    result = false;
                                }
                                break;
                            }
                    }


                    break;
                case CoreDefine.Entities.جدول:
                    {
                        DataSourceInfo = new DataSourceInfo(CoreObject.Find(_Object.ParentID));
                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {
                                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                    if (DataBase.Execute("EXEC sp_rename N'" + _Object.FullName + "', N'" + Tools.SafeTitle(FullName) + "'"))
                                        result = true;

                                    break;
                                }
                            case CoreDefine.DataSourceType.MySql:
                                {
                                    MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                    if (DataBase.Execute("EXEC sp_rename N'" + _Object.FullName + "', N'" + Tools.SafeTitle(FullName) + "'"))
                                        result = true;
                                    break;
                                }

                            case CoreDefine.DataSourceType.ACCESS:
                                {
                                    AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);

                                    if (DataBase.Execute("SELECT * INTO " + Tools.SafeTitle(FullName) + " FROM [" + _Object.FullName + "]"))
                                    {
                                        DataBase.Execute("DROP TABLE " + _Object.FullName);
                                        result = true;
                                    }

                                    break;
                                }
                            case CoreDefine.DataSourceType.EXCEL:
                                {
                                    try
                                    {
                                        using (ExcelEngine excelEngine = new ExcelEngine())
                                        {
                                            IApplication application = excelEngine.Excel;

                                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                            IWorksheet worksheet = workbook.Worksheets[_Object.FullName.Replace("$", "").Replace("'", "")];
                                            worksheet.Name = Tools.SafeTitle(FullName);

                                            workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                        }
                                        result = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        result = false;
                                    }
                                    break;
                                }
                        }

                    }
                    break;
                case CoreDefine.Entities.رویداد_عمومی:
                    {
                        PublicJob PublicJob = new PublicJob(_Object);
                        if (PublicJob.PublicJobType == CoreDefine.PublicJobType.اجرای_رویداد)
                        {
                            DataSourceInfo = new DataSourceInfo(CoreObject.Find(PublicJob.RelatedDatasource));
                            switch (DataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        string Qeury = @"EXEC dbo.sp_update_job @job_name = N'" + _Object.FullName + @"', @new_name = N'" + Tools.SafeTitle(FullName) + "'; ";
                                        SQLDataBase sQLDataBase = Referral.DBMsdb;
                                        if (DataSourceInfo.ServerName != Referral.DBMsdb.ConnectionData.Source)
                                            sQLDataBase = new SQLDataBase(DataSourceInfo.ServerName, "msdb", DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                        if (sQLDataBase.Execute(Qeury))
                                        {
                                            result = true;
                                        }

                                        break;
                                    }
                            }
                        }
                        break;
                    }
                default:
                    result = true;
                    break;
            }

            if (result)
            {
                if (Referral.DBCore.UpdateRow(CoreObjectID, 0, "CoreObject", new string[] { "FullName" }, new object[] { Tools.SafeTitle(FullName) }))
                {
                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                    Referral.CoreObjects[CoreIndex].FullName = Tools.SafeTitle(FullName);

                }

            }

            return Json(result);
        }

        public JsonResult RenameFolder(long ParentID, string Entity, string OldFolder, string NewFolder)
        {
            CoreDefine.Entities EntityName = new CoreDefine.Entities();
            OldFolder = OldFolder.Length > 1 ? (OldFolder.Substring(0, 1) == " " ? OldFolder.Substring(1, OldFolder.Length - 1) : OldFolder) : OldFolder;
            NewFolder = NewFolder.Length > 1 ? (NewFolder.Substring(0, 1) == " " ? NewFolder.Substring(1, NewFolder.Length - 1) : NewFolder) : NewFolder;
            EntityName = Tools.GetEntityFromEnglishName(Entity);  
            List<CoreObject> ItemList = CoreObject.FindChildsInFolder(ParentID, EntityName, OldFolder);
            if (ItemList.Count > 0)
                Referral.DBCore.Execute("Update CoreObject set Folder =N'" + NewFolder + "' where ParentID=" + ParentID + " and Entity=N'" + EntityName.ToString() + "' and Folder=N'" + OldFolder + "'");
            else
            {
                ItemList = CoreObject.FindChildsInFolder(ParentID, EntityName,Tools.SafeTitle( OldFolder));
                if (ItemList.Count > 0)
                    Referral.DBCore.Execute("Update CoreObject set Folder =N'" + NewFolder + "' where ParentID=" + ParentID + " and Entity=N'" + EntityName.ToString() + "' and Folder=N'" + OldFolder + "'");
            }


            foreach (CoreObject Item in ItemList)
            {
                int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == Item.CoreObjectID);
                Referral.CoreObjects[FieldIndex].Folder = NewFolder;

                CoreObject FolderCore = CoreObject.Find(ParentID, CoreDefine.Entities.پوشه, Tools.SafeTitle(OldFolder));
                if (FolderCore.CoreObjectID > 0)
                {
                    bool result =Referral.DBCore.Execute("Update CoreObject set Folder =N'" + NewFolder + "' , FullName=N'"+Tools.SafeTitle(NewFolder) +"' where ParentID=" + ParentID + " and Entity=N'" + CoreDefine.Entities.پوشه.ToString() + "' and Folder=N'" + OldFolder + "'");
                    if (result)
                    {
                        int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == FolderCore.CoreObjectID);
                        if (CoreIndex > -1) 
                            Referral.CoreObjects[CoreIndex].Folder = NewFolder;
                    }
                }

            }

            if (ItemList.Count == 0)
            {
                List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == CoreDefine.Entities.پوشه && item.FullName.Contains(Entity + "_") && item.Folder.StartsWith(OldFolder)));
                foreach (CoreObject item in children)
                {
                    string[] FolderSplit=OldFolder.Split('/');
                    FolderSplit[FolderSplit.Length - 1] = NewFolder;
                    bool result = Referral.DBCore.Execute("Update CoreObject set Folder =N'" + string.Join("/", FolderSplit) + "'  where CoreObjectID="+ item.CoreObjectID);
                    if (result)
                    {
                        int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == item.CoreObjectID);
                        if (CoreIndex > -1)
                            Referral.CoreObjects[CoreIndex].Folder = NewFolder;
                    }

                }
            }


            return Json(1);
        }

        public JsonResult DeleteCore(long CoreObjectID, string DeleteType)
        {
            CoreObject _Object = CoreObject.Find(CoreObjectID);
            bool result = false;
            if (_Object.CoreObjectID > 0 && CoreObjectID > 0)
            {
                switch (_Object.Entity)
                {
                    case CoreDefine.Entities.فیلد:
                        {
                            CoreObject TableCore = CoreObject.Find(_Object.ParentID);
                            if(TableCore.Entity== CoreDefine.Entities.جدول)
                            {
                                Table TableInfo = new Table(TableCore); 
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                                switch (DataSourceInfo.DataSourceType)
                                {
                                    case CoreDefine.DataSourceType.SQLSERVER:
                                        {
                                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                            if (DataBase.Execute("ALTER TABLE " + TableInfo.TABLESCHEMA + "." + TableCore.FullName + " DROP COLUMN " + _Object.FullName))
                                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);

                                            break;
                                        }
                                    case CoreDefine.DataSourceType.MySql:
                                        {
                                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                            if (DataBase.Execute("ALTER TABLE dbo." + TableCore.FullName + " DROP COLUMN " + _Object.FullName))
                                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                                            break;
                                        }

                                    case CoreDefine.DataSourceType.ACCESS:
                                        {
                                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                            if (DataBase.Execute("Alter table [" + TableCore.FullName + "] drop column [" + _Object.FullName + "]"))
                                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);

                                            break;
                                        }
                                    case CoreDefine.DataSourceType.EXCEL:
                                        {
                                            using (ExcelEngine excelEngine = new ExcelEngine())
                                            {
                                                IApplication application = excelEngine.Excel;

                                                application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                                IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                                IWorksheet worksheet = workbook.Worksheets[TableCore.FullName.Replace("$", "").Replace("'", "")];

                                                foreach (IRange Column in worksheet.Columns)
                                                {
                                                    if (Column.DisplayText.Replace("\n", "_") == _Object.FullName)
                                                    {
                                                        worksheet.DeleteColumn(Column.Column);
                                                        break;
                                                    }
                                                }

                                                workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                                            }
                                            break;
                                        }
                                } 

                            }
                            else
                            {
                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                            }

                            break;
                        }
                    case CoreDefine.Entities.جدول:
                        {
                            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(_Object.ParentID));
                            switch (DataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                        if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                        {  
                                            List<CoreObject> FieldList = CoreObject.FindChilds(CoreObjectID);
                                            foreach (CoreObject Field in FieldList) 
                                                DeleteCore(Field.CoreObjectID, DeleteType);

                                            DataBase.Execute("DROP TABLE " + _Object.FullName);

                                        }

                                        result = true;

                                        break;
                                    }

                                case CoreDefine.DataSourceType.MySql:
                                    {
                                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                        if (DataBase.Execute("DROP TABLE " + _Object.FullName))
                                        {
                                            if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            {
                                                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                Referral.CoreObjects.RemoveAt(CoreIndex);

                                                List<CoreObject> FieldList = CoreObject.FindChilds(CoreObjectID);
                                                foreach (CoreObject Field in FieldList)
                                                {
                                                    if (Referral.DBCore.Delete("CoreObject", Field.CoreObjectID))
                                                    {
                                                        int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == Field.CoreObjectID);
                                                        Referral.CoreObjects.RemoveAt(FieldIndex);
                                                    }

                                                }
                                            }

                                            result = true;
                                        }
                                        break;
                                    }

                                case CoreDefine.DataSourceType.ACCESS:
                                    {
                                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                        if (DataBase.Execute("DROP TABLE [" + _Object.FullName + "]"))
                                        {
                                            if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            {
                                                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                Referral.CoreObjects.RemoveAt(CoreIndex);

                                                List<CoreObject> FieldList = CoreObject.FindChilds(CoreObjectID);
                                                foreach (CoreObject Field in FieldList)
                                                {
                                                    if (Referral.DBCore.Delete("CoreObject", Field.CoreObjectID))
                                                    {
                                                        int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == Field.CoreObjectID);
                                                        Referral.CoreObjects.RemoveAt(FieldIndex);
                                                    }

                                                }
                                            }

                                            result = true;
                                        }

                                        break;
                                    }

                                case CoreDefine.DataSourceType.EXCEL:
                                    {
                                        using (ExcelEngine excelEngine = new ExcelEngine())
                                        {
                                            IApplication application = excelEngine.Excel;

                                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                            IWorksheet worksheet = workbook.Worksheets[_Object.FullName.Replace("$", "").Replace("'", "")];
                                            worksheet.Remove();
                                            if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            {
                                                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                Referral.CoreObjects.RemoveAt(CoreIndex);

                                                List<CoreObject> FieldList = CoreObject.FindChilds(CoreObjectID);
                                                foreach (CoreObject Field in FieldList)
                                                {
                                                    if (Referral.DBCore.Delete("CoreObject", Field.CoreObjectID))
                                                    {
                                                        int FieldIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == Field.CoreObjectID);
                                                        Referral.CoreObjects.RemoveAt(FieldIndex);
                                                    }

                                                }

                                                workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                                result = true;
                                            }
                                        }
                                        break;
                                    }
                            }

                            break;
                        }

                    case CoreDefine.Entities.پایگاه_داده:
                        {
                            if (DeleteType == "حذف از هسته نرم افزار")
                            {
                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                            }
                            else
                            {
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObjectID));
                                switch (DataSourceInfo.DataSourceType)
                                {
                                    case CoreDefine.DataSourceType.SQLSERVER:
                                        {
                                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);

                                            DataBase.Execute(" DROP DATABASE " + DataSourceInfo.DataBase + " ;   ");
                                            if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            {
                                                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                Referral.CoreObjects.RemoveAt(CoreIndex);
                                            }

                                            result = true;

                                            break;
                                        }
                                    case CoreDefine.DataSourceType.MySql:
                                        {
                                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);

                                            if (DataBase.Execute("USE master ;GO DROP DATABASE " + DataSourceInfo.DataBase + " ;GO   "))
                                            {
                                                if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                                {
                                                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                    Referral.CoreObjects.RemoveAt(CoreIndex);
                                                }

                                                result = true;
                                            }

                                            break;
                                        }
                                    case CoreDefine.DataSourceType.EXCEL:
                                        {
                                            if (Models.Attachment.DeleteFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                            {
                                                if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                                {
                                                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                    Referral.CoreObjects.RemoveAt(CoreIndex);
                                                }

                                                result = true;
                                            }

                                            break;
                                        }
                                    case CoreDefine.DataSourceType.ACCESS:
                                        {
                                            if (Models.Attachment.DeleteFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                            {
                                                if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                                {
                                                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                                    Referral.CoreObjects.RemoveAt(CoreIndex);
                                                }

                                                result = true;
                                            }

                                            break;
                                        }
                                }
                            }
                            break;
                        }


                    case CoreDefine.Entities.رویداد_عمومی:
                        {
                            PublicJob PublicJob = new PublicJob(_Object);
                            if (PublicJob.PublicJobType == CoreDefine.PublicJobType.اجرای_رویداد)
                            {
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(PublicJob.RelatedDatasource));
                                switch (DataSourceInfo.DataSourceType)
                                {
                                    case CoreDefine.DataSourceType.SQLSERVER:
                                        {
                                            string Qeury = @"EXEC sp_delete_job  @job_name = N'" + _Object.FullName + "'";
                                            SQLDataBase sQLDataBase = Referral.DBMsdb;
                                            if (DataSourceInfo.ServerName != Referral.DBMsdb.ConnectionData.Source)
                                                sQLDataBase = new SQLDataBase(DataSourceInfo.ServerName, "msdb", DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                            if (sQLDataBase.Execute(Qeury))
                                            {
                                                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                                            }

                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    default:
                        {
                            result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                            if (result)
                            {
                                List<CoreObject> FieldList = CoreObject.FindChilds(CoreObjectID);
                                foreach (CoreObject Field in FieldList)
                                {
                                    DeleteCore(Field.CoreObjectID, DeleteType);
                                }
                            }
                            break;
                        }
                }
            }
            else if (_Object.CoreObjectID == 0 && CoreObjectID > 0)
                result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
            if (result)
            {
                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                if (CoreIndex > -1)
                    Referral.CoreObjects.RemoveAt(CoreIndex);
            }
            return Json(result);
        }

        public JsonResult DeleteFolderCore(long ParentID, string Entity, string Folder, string DeleteType)
        {
            bool result = true;
            CoreDefine.Entities EntityType = Tools.GetEntityFromEnglishName(Entity);
            CoreObject FolderCore = CoreObject.Find(ParentID, CoreDefine.Entities.پوشه, Tools.SafeTitle(Folder));
            if(FolderCore.CoreObjectID>0)
            {
                result = Referral.DBCore.Delete("CoreObject", FolderCore.CoreObjectID);
                if (result)
                {
                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == FolderCore.CoreObjectID);
                    if (CoreIndex > -1)
                        Referral.CoreObjects.RemoveAt(CoreIndex);
                }
            }

            foreach (CoreObject _Object in CoreObject.FindChildsInFolder(ParentID, EntityType, Folder))
               DeleteCore(_Object.CoreObjectID, DeleteType);

            List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == CoreDefine.Entities.پوشه && item.FullName.Contains(Entity + "_") && item.Folder.StartsWith(Folder)));
            foreach (CoreObject _Object in children)
                DeleteCore(_Object.CoreObjectID, DeleteType);


            return Json(result);
        }

        public JsonResult DefaultView(long CoreObjectID)
        {
            int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
            if (CoreObject.Find(CoreObjectID).IsDefault)
            {
                Referral.CoreObjects[CoreIndex].IsDefault = false;
                return Json(Referral.DBCore.UpdateRow(CoreObjectID, 0, "CoreObject", new string[] { "IsDefault" }, new object[] { false }));
            }
            else
            {
                Referral.CoreObjects[CoreIndex].IsDefault = true;
                return Json(Referral.DBCore.UpdateRow(CoreObjectID, 0, "CoreObject", new string[] { "IsDefault" }, new object[] { true }));
            }
        }

        public JsonResult SetPrimaryKey(long CoreObjectID)
        {
            CoreObject FieldObject = CoreObject.Find(CoreObjectID);
            Field Field = new Field(FieldObject);
            if (Field.FieldNature == "Bigint")
            {
                try
                {
                    CoreObject Table = CoreObject.Find(FieldObject.ParentID);
                    DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(Table.ParentID));
                    if (!Field.IsIdentity)
                    {
                        List<CoreObject> FieldList = CoreObject.FindChilds(FieldObject.ParentID, CoreDefine.Entities.فیلد);
                        bool IsFindIdentity = false;
                        foreach (CoreObject FieldItem in FieldList)
                        {
                            Field FieldCore = new Field(FieldItem);
                            if (FieldCore.IsIdentity)
                            {
                                IsFindIdentity = true;
                                break;
                            }
                        }

                        if (IsFindIdentity)
                            return Json("ابتدا کلید اصلی این جدول را غیر فعال کرده و سپس کلید اصلی را انتخاب نمایید");

                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {
                                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                    DataBase.RemovePrimeryKey(Table.FullName);
                                    DataBase.SetPrimeryKey(Table.FullName, FieldObject.FullName);
                                    break;
                                }

                        }
                        Field.IsIdentity = true;
                    }
                    else
                    {
                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {
                                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                    DataBase.RemovePrimeryKey(Table.FullName);
                                    break;
                                }

                        }
                        Field.IsIdentity = false;
                    }

                    string Value = Tools.ToXML(Field);
                    Referral.DBCore.UpdateRow(CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value });
                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                    Referral.CoreObjects[CoreIndex].Value = Value;
                }
                catch (Exception ex)
                {
                    return Json("عملیات با شکست مواجه شد");
                }
            }
            else
                return Json("ماهیت فیلد انتخاب شده عدد نمی باشد");

            return Json("");
        }

        public JsonResult DragDroupItem(long ParentID, string Entity, string Folder, long CoreID, int DestinationIndex,string DropPosition)
        {
            Folder = Folder.Length > 1 ? (Folder.Substring(0, 1) == " " ? Folder.Substring(1, Folder.Length - 1) : Folder) : Folder;
            Entity = Entity == "فرم" ? "فرم_ورود_اطلاعات" : Entity;
            List<CoreObject> ItemList = new List<CoreObject>();
            if(DropPosition== "after")
                ItemList = Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == Folder && item.OrderIndex > DestinationIndex && item.CoreObjectID != CoreID).ToList();
            else
            { 
                ItemList = Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == Folder && item.OrderIndex >= DestinationIndex && item.CoreObjectID != CoreID).ToList();
                if(ItemList.Count == 0)
                {
                    string UpdateQuery = "SET NOCOUNT ON; " +
                        "\nDECLARE @CoreObjectID bigint, @OrderIndex Bigint = 1;" +
                        "\nDECLARE vendor_cursor CURSOR FOR" +
                        "\nselect CoreObjectID" +
                        "\nfrom CoreObject" +
                        "\nwhere" +
                        "\nParentID = "+ ParentID+ " and Entity=N'"+ Tools.GetEntity(Entity) + "' and Folder=N'"+ Folder + "' and OrderIndex<="+ DestinationIndex+ " and CoreObjectID<> " + CoreID +
                        "\norder by CoreObjectID" +
                        "\nOPEN vendor_cursor" +
                        "\nFETCH NEXT FROM vendor_cursor" +
                        "\nINTO @CoreObjectID" +
                        "\nWHILE @@FETCH_STATUS = 0" +
                        "\nBEGIN" +
                        "\nupdate CoreObject set OrderIndex = @OrderIndex where CoreObjectID = @CoreObjectID" +
                        "\n set @OrderIndex = @OrderIndex + 1;" +
                        "\nFETCH NEXT FROM vendor_cursor" +
                        "\nINTO @CoreObjectID" +
                        "\nEND" +
                        "\nCLOSE vendor_cursor;" +
                        "\nDEALLOCATE vendor_cursor; ";
                    Referral.DBCore.Execute(UpdateQuery);

                    Referral.CoreObjects.RemoveAll(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == Folder && item.OrderIndex <= DestinationIndex && item.CoreObjectID != CoreID);
                    DataTable CoreData = Referral.DBCore.SelectDataTable("Select * from CoreObject where ParentID = " + ParentID + " and Entity=N'" + Tools.GetEntity(Entity) + "' and Folder=N'" + Folder + "' and OrderIndex<=" + DestinationIndex + " and CoreObjectID<> " + CoreID);
                    foreach (DataRow Row in CoreData.Rows)
                    {
                        Referral.CoreObjects.Add(new CoreObject(
                           Convert.ToInt64(Row["CoreObjectID"].ToString()),
                           Convert.ToInt64(Row["ParentID"].ToString()),
                           Tools.GetEntity(Row["Entity"].ToString()),
                           Row["Folder"].ToString(),
                           Row["FullName"].ToString(),
                           Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                           (bool)Row["IsDefault"],
                           (object)Row["Value"]
                            ));
                    }

                }
            }
            int OrderIndex = DestinationIndex + 1;
            //CoreObject x2 = CoreObject.Find(CoreID);
            Referral.DBCore.Execute("Update CoreObject Set OrderIndex =" + (DestinationIndex).ToString() + " , Folder=N'" + Folder + "' Where CoreObjectID=" + CoreID);
            foreach (CoreObject Item in ItemList)
            {
                if(Item.OrderIndex >= DestinationIndex)
                { 
                    Referral.DBCore.Execute("Update CoreObject Set OrderIndex =" + OrderIndex + " Where CoreObjectID=" + Item.CoreObjectID);
                    int ItemIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == Item.CoreObjectID);
                    Referral.CoreObjects[ItemIndex].OrderIndex = OrderIndex;
                    ++OrderIndex;
                }
            }

            int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreID);
            Referral.CoreObjects[CoreIndex].OrderIndex = DestinationIndex;
            Referral.CoreObjects[CoreIndex].Folder = Folder;
            return Json(1);
        }
        public JsonResult DragDroupFolderItem(long ParentID, string Entity, string SourceFolder, string DestinationFolder, string Position)
        {
            SourceFolder = SourceFolder.Length > 1 ? (SourceFolder.Substring(0, 1) == " " ? SourceFolder.Substring(1, SourceFolder.Length - 1) : SourceFolder) : SourceFolder;
            Entity = Entity == "فرم" ? "فرم_ورود_اطلاعات" : Entity;
            List<CoreObject> SourceItemList = new List<CoreObject>();
            List<CoreObject> DestinationList = new List<CoreObject>();

            long MaxIndexSource = 0;
            if (Position == "before")
            {
                SourceItemList = Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == SourceFolder).ToList();
                DestinationList = Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == DestinationFolder).ToList();
            }
            else
            {
                DestinationList = Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == SourceFolder).ToList();
                SourceItemList = Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == Tools.GetEntity(Entity) && item.Folder == DestinationFolder).ToList();
            }

            foreach (CoreObject SourceItem in SourceItemList)
            {
                if (SourceItem.CoreObjectID > MaxIndexSource)
                    MaxIndexSource = SourceItem.OrderIndex;
            }
            foreach (CoreObject DestinationItem in DestinationList)
            {
                ++MaxIndexSource;
                Referral.DBCore.Execute("Update CoreObject Set OrderIndex =" + MaxIndexSource + " Where CoreObjectID=" + DestinationItem.CoreObjectID);
                int ItemIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == DestinationItem.CoreObjectID);
                Referral.CoreObjects[ItemIndex].OrderIndex = MaxIndexSource;

            }
            return Json(1);
        }

        public JsonResult DataSourceTestConnectionForm(string[] FormInputName, string[] FormInputValue)
        {
            string Error = "";
            string FilePath = FormInputValue[Array.IndexOf(FormInputName, "FilePath")];
            if (SysSetting.SysSettingID > 0)
            {
                DataSourceInfo DataSourceInfo = new DataSourceInfo();

                DataSourceInfo.UserName = FormInputValue[Array.IndexOf(FormInputName, "UserName")];
                DataSourceInfo.Password = FormInputValue[Array.IndexOf(FormInputName, "Password")];
                DataSourceInfo.DataBase = FormInputValue[Array.IndexOf(FormInputName, "DataBase")];
                DataSourceInfo.ServerName = FormInputValue[Array.IndexOf(FormInputName, "ServerName")];
                DataSourceInfo.DataSourceType = Tools.GetDataSourceType(FormInputValue[Array.IndexOf(FormInputName, "DataSourceType")]);
                DataSourceInfo.FilePath = FilePath;

                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {
                            if (string.IsNullOrEmpty(DataSourceInfo.UserName) || string.IsNullOrEmpty(DataSourceInfo.Password) || string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.ServerName))
                                Error = "فیلد های خالی را پر نمایید";
                            else
                            {
                                if (!DataSourceInfo.CheckConnected())
                                    Error = "برقراری ارتباط با پایگاه داده با شکست مواجه شد";
                            }
                            break;
                        }
                    case CoreDefine.DataSourceType.MySql:
                        {
                            if (string.IsNullOrEmpty(DataSourceInfo.UserName) || string.IsNullOrEmpty(DataSourceInfo.Password) || string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.ServerName))
                                Error = "فیلد های خالی را پر نمایید";
                            else
                            {
                                if (!DataSourceInfo.CheckConnected())
                                    Error = "برقراری ارتباط با پایگاه داده با شکست مواجه شد";
                            }
                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                                Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
                            else
                            {
                                DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                                DataSourceInfo.DataBase += DataSourceInfo.DataBase.IndexOf(".xlsx") > -1 ? "" : ".xlsx";
                                if (!Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                    Error = "فایل یافت نشد";
                            }

                            break;
                        }
                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                                Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
                            else
                            {
                                DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                                DataSourceInfo.DataBase += DataSourceInfo.DataBase.IndexOf(".accdb") > -1 ? "" : ".accdb";
                                if (!Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                    Error = "فایل یافت نشد";
                            }

                            break;
                        }
                }
            }
            return Json(new { Error = Error, FilePath = FilePath });
        }
        public JsonResult DataSourceTestConnection(long DatabaseID)
        {
            string Error = "";
            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(DatabaseID));
            string FilePath = DataSourceInfo.FilePath;
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        if (string.IsNullOrEmpty(DataSourceInfo.UserName) || string.IsNullOrEmpty(DataSourceInfo.Password) || string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.ServerName))
                            Error = "فیلد های خالی را پر نمایید";
                        else
                        {
                            if (!DataSourceInfo.CheckConnected())
                                Error = "برقراری ارتباط با پایگاه داده با شکست مواجه شد";
                        }
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                            Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
                        else
                        {
                            DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                            DataSourceInfo.DataBase += Tools.GetExcelFormat(DataSourceInfo.DataBase);
                            if (!Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                Error = "فایل یافت نشد";

                        }
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                            Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
                        else
                        {
                            DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                            DataSourceInfo.DataBase += Tools.GetAccessFormat(DataSourceInfo.DataBase);
                            if (!Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                Error = "فایل یافت نشد";
                        }
                        break;
                    }
            }

            return Json(Error);
        }
        public JsonResult DataSourceCreateConnectionForm(string[] FormInputName, string[] FormInputValue)
        {
            string Error = "";
            string DataBaseName = "";
            string FilePath = "";
            DataSourceInfo DataSourceInfo = new DataSourceInfo();

            if (SysSetting.SysSettingID > 0)
            {

                DataSourceInfo.UserName = FormInputValue[Array.IndexOf(FormInputName, "UserName")];
                DataSourceInfo.Password = FormInputValue[Array.IndexOf(FormInputName, "Password")];
                DataSourceInfo.DataBase = FormInputValue[Array.IndexOf(FormInputName, "DataBase")];
                DataSourceInfo.ServerName = FormInputValue[Array.IndexOf(FormInputName, "ServerName")];
                DataSourceInfo.DataSourceType = Tools.GetDataSourceType(FormInputValue[Array.IndexOf(FormInputName, "DataSourceType")]);
                DataSourceInfo.FilePath = FormInputValue[Array.IndexOf(FormInputName, "FilePath")];

                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {
                            APM.Models.SysSetting.CreateSQLDatabase(DataSourceInfo, ref Error, ref DataBaseName, ref FilePath);
                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            APM.Models.SysSetting.CreateExcelDatabase(DataSourceInfo, ref Error, ref DataBaseName, ref FilePath);
                            break;
                        }
                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            APM.Models.SysSetting.CreateAccessDatabase(DataSourceInfo, ref Error, ref DataBaseName, ref FilePath);
                            break;
                        }
                }
            }
            return Json(new { Error = Error, FilePath = DataSourceInfo.FilePath, Database = DataSourceInfo.DataBase });
        }

        public JsonResult DataSourceCreateConnection(long DatabaseID)
        {
            string Error = "";
            string DataBaseName = "";
            string FilePath = "";

            if (DatabaseID > 0)
            {
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(DatabaseID));
                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {
                            APM.Models.SysSetting.CreateSQLDatabase(DataSourceInfo, ref Error, ref DataBaseName, ref FilePath);
                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            APM.Models.SysSetting.CreateExcelDatabase(DataSourceInfo, ref Error, ref DataBaseName, ref FilePath);
                            break;
                        }
                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            APM.Models.SysSetting.CreateAccessDatabase(DataSourceInfo, ref Error, ref DataBaseName, ref FilePath);
                            break;
                        }
                }
            }
            return Json(Error);
        }

        public JsonResult Rebuilding(long BaseCoreID)
        {
            string Error = string.Empty;
            string AlarmRebuilding = string.Empty;
            if (BaseCoreID > 0)
            {
                CoreObject coreObject = CoreObject.Find(BaseCoreID);
                switch (coreObject.Entity)
                {
                    case CoreDefine.Entities.پایگاه_داده:
                        {
                            Models.SysSetting.RebuildDataBase(BaseCoreID,ref Error,ref AlarmRebuilding);
                            break;
                        }
                        
                    case CoreDefine.Entities.تابع_جدولی:
                        {
                            Models.SysSetting.RebuildTableFunction(BaseCoreID, ref Error, ref AlarmRebuilding);
                            break;
                        }

                }
            }
            return Json(new { AlarmRebuilding = AlarmRebuilding, Error = Error });
        }

        public JsonResult CopyCoreObject(long Copy_CoreID, long Copy_ParentId, string Copy_CoreFolder, string Copy_CoreEntity, string Copy_CoreName, long CoreID, long ParentId, string CoreFolder, string CoreEntity, string CoreName)
        {
            long ID = 0;
            if(Copy_CoreName.IndexOf("(")>-1)
            {
                Copy_CoreName = Copy_CoreName.Substring(0, Copy_CoreName.IndexOf("("));
            }
            CoreDefine.Entities EntityType = Tools.GetEntityFromEnglishName(Copy_CoreEntity);

            if (Copy_CoreID > 0)
            {
                CoreObject CopyCoreObject = CoreObject.Find(Copy_CoreID);
                bool IsFind = false;
                List<CoreObject> coreChilde = CoreObject.FindChilds(Copy_ParentId, EntityType, CopyCoreObject.FullName);
                int FindcoreConter = coreChilde.Count + 1;
                string FullName = Tools.SafeTitle(CopyCoreObject.FullName + FindcoreConter.ToString());

                while (!IsFind)
                {
                    List<CoreObject> coreChildeList = CoreObject.FindChilds(Copy_ParentId, EntityType, FullName);
                    if (coreChildeList.Count > 0)
                    {
                        FullName = Tools.SafeTitle(FullName.Substring(0, CopyCoreObject.FullName.Length) + (FindcoreConter + 1).ToString());
                        FindcoreConter++;
                    }
                    else
                        IsFind = true;
                }

                List<CoreObject> CopyCoreChildeList = CoreObject.FindChilds(Copy_CoreID);
                if (EntityType == CoreDefine.Entities.گزارش)
                    if (CopyCoreChildeList.Where(item => item.Entity == CoreDefine.Entities.منبع_گزارش).Count() == 0)
                    {
                        DataTable CoreData = Referral.DBCore.SelectDataTable("SELECT  CoreObjectID, ParentID, Entity, Folder, FullName, OrderIndex, IsDefault, value FROM  CoreObject where Entity = N'منبع_گزارش' and ParentID = "+ Copy_CoreID + "  order by OrderIndex");
                        foreach (DataRow Row in CoreData.Rows)
                        {
                            int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == long.Parse(Row["CoreObjectID"].ToString()));
                            if (CoreIndex < 0)
                            {
                                CoreObject SourceCore = new CoreObject(
                                   Convert.ToInt64(Row["CoreObjectID"].ToString()),
                                   Convert.ToInt64(Row["ParentID"].ToString()),
                                   Tools.GetEntity(Row["Entity"].ToString()),
                                   Row["Folder"].ToString(),
                                   Row["FullName"].ToString(),
                                   Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                                   (bool)Row["IsDefault"],
                                   (object)Row["Value"]
                                    );
                                Referral.CoreObjects.Add(SourceCore);
                                CopyCoreChildeList.Add(SourceCore);
                            }
                            else
                                Referral.CoreObjects[CoreIndex].Value = Row["value"];
                        }
                    } 

                long OrderIndex = 0;
                switch (EntityType)
                {
                    case CoreDefine.Entities.جدول:
                        {
                            ID = CreateCoreTable(ParentId, CoreFolder, FullName, OrderIndex, CopyCoreObject.Value.ToString()); 
                            Table table = new Table(CoreObject.Find(ParentId, CoreDefine.Entities.جدول, Tools.SafeTitle(Copy_CoreName)));
                            CopyCoreChildeList.Remove(CoreObject.Find(Copy_CoreID, CoreDefine.Entities.فیلد, table.IDField().FieldName));
                            break;
                        }

                    case CoreDefine.Entities.فیلد:
                        {
                            ID = CreateField(ParentId, CoreFolder, FullName, OrderIndex, CopyCoreObject.Value.ToString());
                            break;
                        }

                    case CoreDefine.Entities.رویداد_عمومی:
                        {
                            PublicJob PublicJob = new PublicJob(CopyCoreObject);

                            if (PublicJob.PublicJobType == CoreDefine.PublicJobType.اجرای_رویداد)
                            {
                                DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(PublicJob.RelatedDatasource));
                                switch (dataSourceInfo.DataSourceType)
                                {
                                    case CoreDefine.DataSourceType.SQLSERVER:
                                        {
                                            string Qeury = @"DECLARE @FullName as Nvarchar(255) =N'" + FullName + "' \n " +
                                                "DECLARE @jobId BINARY(16) \n" +
                                                "EXEC  msdb.dbo.sp_add_job \n" +
                                                "       @job_name=@FullName,@enabled=1,@notify_level_eventlog=0,@notify_level_email=0, 	@notify_level_netsend=0,@notify_level_page=0,@delete_level=0,@job_id = @jobId OUTPUT \n" +
                                                "EXEC  msdb.dbo.sp_add_jobstep \n" +
                                                "@job_id=@jobId,@step_name=@FullName,@step_id=1,@cmdexec_success_code=0,@on_success_action=1,@on_success_step_id=0,@on_fail_action=2,@on_fail_step_id=0,@retry_attempts=0,@retry_interval=0,@os_run_priority=0,@subsystem=N'TSQL',@database_name=N'" + dataSourceInfo.DataBase + "',@flags=0,@command=N'" + PublicJob.Query + "' \n" +
                                                "EXEC  msdb.dbo.sp_add_jobschedule \n" +
                                                "   @job_id=@jobId,@name=@FullName,@enabled=1,@freq_type=4,@freq_interval=" + PublicJob.RepeatDay.ToString() + @",@freq_subday_type=8, @freq_subday_interval=" + PublicJob.RepeatClock.ToString() + @",@freq_relative_interval=0,@freq_recurrence_factor=0,@active_start_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.StartDate).Replace("/", "") + ",@active_end_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.EndDate).Replace("/", "") + ",@active_start_time=" + PublicJob.StartTime.Replace(":", "") + ",@active_end_time=" + PublicJob.EndTime.Replace(":", "");
                                            SQLDataBase sQLDataBase = Referral.DBMsdb;
                                            if (dataSourceInfo.ServerName != Referral.DBMsdb.ConnectionData.Source)
                                                sQLDataBase = new SQLDataBase(dataSourceInfo.ServerName, "msdb", dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                            if (sQLDataBase.Execute(Qeury))
                                            {

                                            }
                                            else
                                            {
                                                Qeury = @"DECLARE @FullName as Nvarchar(255) =N'" + FullName + "' \n " +
                                                    "EXEC  dbo.sp_update_jobstep  @job_name = @FullName, @step_id = 1,  @step_name = @FullName,@subsystem = N'TSQL',@command = N'" + PublicJob.Query + "'  ; \n" +
                                                    "EXEC  dbo.sp_update_schedule  \n" +
                                                "    @name=@FullName,@enabled=1,@freq_type=4,@freq_interval=" + PublicJob.RepeatDay.ToString() + @",@freq_subday_type=8, @freq_subday_interval=" + PublicJob.RepeatClock.ToString() + @",@freq_relative_interval=0,@freq_recurrence_factor=0,@active_start_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.StartDate).Replace("/", "") + ",@active_end_date=" + CDateTime.ConvertShamsiToMilady(PublicJob.EndDate).Replace("/", "") + ",@active_start_time=" + PublicJob.StartTime.Replace(":", "") + ",@active_end_time=" + PublicJob.EndTime.Replace(":", "");
                                                sQLDataBase.Execute(Qeury);
                                            }
                                            ID = Referral.DBCore.Insert("CoreObject"
                                               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                               , new object[] { ParentId, EntityType.ToString(), CoreFolder, FullName, CopyCoreObject.OrderIndex, CopyCoreObject.IsDefault, CopyCoreObject.Value.ToString() });
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    default:
                        {
                            ID = Referral.DBCore.Insert("CoreObject"
                               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                               , new object[] { ParentId, EntityType.ToString(), CoreFolder, FullName, CopyCoreObject.OrderIndex, CopyCoreObject.IsDefault, CopyCoreObject.Value.ToString() });
                            break;
                        }
                }
                if (ID > 0)
                {
                    Referral.CoreObjects.Add(new CoreObject(ID, ParentId, EntityType, CoreFolder, FullName, OrderIndex, CopyCoreObject.IsDefault, CopyCoreObject.Value.ToString()));
                    RebuildCore(CopyCoreChildeList, ID);
                }


            }
            else
            {
                bool IsFind = false;
                List<CoreObject> coreChilde = CoreObject.FindChildsInFolder(Copy_ParentId, EntityType, Copy_CoreFolder);
                int FindcoreConter = 1;
                string FolderName = Tools.SafeTitle(Copy_CoreFolder + FindcoreConter.ToString());
                while (!IsFind)
                {
                    List<CoreObject> coreChildeList = CoreObject.FindChildsInFolder(Copy_ParentId, EntityType, FolderName);
                    if (coreChildeList.Count > 0)
                    {
                        FolderName = Tools.SafeTitle(FolderName.Substring(0, Copy_CoreFolder.Length) + (FindcoreConter + 1).ToString());
                        FindcoreConter++;
                    }
                    else
                        IsFind = true;
                }

                CoreObject FolderCore = CoreObject.Find(ParentId, CoreDefine.Entities.پوشه, Tools.SafeTitle(FolderName));
                Folder NewFolder = new Folder();

                if (FolderCore.CoreObjectID > 0)
                    NewFolder = new Folder(FolderCore);

                List<CoreObject> FolderCoreList = CoreObject.FindChilds(ParentId, CoreDefine.Entities.پوشه);

                long MaxFolderIndex = 0;

                foreach (CoreObject coreObject in FolderCoreList)
                    if (coreObject.OrderIndex > MaxFolderIndex)
                        MaxFolderIndex = coreObject.OrderIndex;

                FolderName = Tools.UnSafeTitle(FolderName);

                ID = Referral.DBCore.Insert("CoreObject"
                                  , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                  , new object[] { ParentId, CoreDefine.Entities.پوشه.ToString(), FolderName,Tools.SafeTitle(FolderName), MaxFolderIndex+1, 0, Tools.ToXML(NewFolder) }); 

                if (ID > 0) 
                    Referral.CoreObjects.Add(new CoreObject(ID, ParentId, CoreDefine.Entities.پوشه, FolderName, Tools.SafeTitle(FolderName), MaxFolderIndex + 1,false, Tools.ToXML(NewFolder)));  

                foreach (CoreObject coreObject in coreChilde)
                {
                    List<CoreObject> CopyCoreChildeList = CoreObject.FindChilds(coreObject.CoreObjectID);

                    List<CoreObject> SubChilde = CoreObject.FindChilds(coreObject.ParentID, coreObject.Entity, coreObject.FullName);
                    FindcoreConter = SubChilde.Count + 1;
                    IsFind = false;
                    string FullName = coreObject.FullName + FindcoreConter.ToString();
                    while (!IsFind)
                    {
                        List<CoreObject> coreChildeList = CoreObject.FindChilds(Copy_ParentId, EntityType, FullName);
                        if (coreChildeList.Count > 0)
                        {
                            FullName = Tools.SafeTitle(FullName.Substring(0, coreObject.FullName.Length) + (FindcoreConter + 1).ToString());
                            FindcoreConter++;
                        }
                        else
                            IsFind = true;
                    }

                    switch (EntityType)
                    {
                        case CoreDefine.Entities.جدول:
                            {
                                ID = CreateCoreTable(ParentId, FolderName, FullName, coreObject.OrderIndex, coreObject.Value.ToString());
                                Table table = new Table(CoreObject.Find(ParentId, CoreDefine.Entities.جدول, coreObject.FullName));
                                CopyCoreChildeList.Remove(CoreObject.Find(coreObject.CoreObjectID, CoreDefine.Entities.فیلد, table.IDField().FieldName));
                                break;
                            }

                        case CoreDefine.Entities.فیلد:
                            {
                                ID = CreateField(ParentId, FolderName, FullName, coreObject.OrderIndex, coreObject.Value.ToString());
                                break;
                            }
                        default:
                            {
                                ID = Referral.DBCore.Insert("CoreObject"
                                   , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                   , new object[] { ParentId, EntityType.ToString(), FolderName, FullName, coreObject.OrderIndex, coreObject.IsDefault, coreObject.Value.ToString() });
                                break;
                            }
                    }
                    if (ID > 0)
                    {
                        Referral.CoreObjects.Add(new CoreObject(ID, ParentId, EntityType, FolderName, FullName, coreObject.OrderIndex, coreObject.IsDefault, coreObject.Value.ToString()));
                        RebuildCore(CopyCoreChildeList, ID);
                    }

                }


            }
            return Json(ID);
        }

        private void RebuildCore(List<CoreObject> CoreChilde, long ParentID)
        {
            long ID = 0;
            foreach (CoreObject CoreItem in CoreChilde)
            {
                switch (CoreItem.Entity)
                {
                    case CoreDefine.Entities.جدول:
                        {
                            ID = CreateCoreTable(ParentID, CoreItem.Folder, CoreItem.FullName, CoreItem.OrderIndex, CoreItem.Value.ToString());
                            break;
                        }
                    case CoreDefine.Entities.فیلد:
                        {
                            ID = CreateField(ParentID, CoreItem.Folder, CoreItem.FullName, CoreItem.OrderIndex, CoreItem.Value.ToString());
                            if(ID==0)
                            { 
                                ID = Referral.DBCore.Insert("CoreObject"
                                   , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                   , new object[] { ParentID, CoreItem.Entity.ToString(), CoreItem.Folder, CoreItem.FullName, CoreItem.OrderIndex, CoreItem.IsDefault, CoreItem.Value.ToString() });
                            }
                            break;
                        }
                    default:
                        {
                            ID = Referral.DBCore.Insert("CoreObject"
                               , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                               , new object[] { ParentID, CoreItem.Entity.ToString(), CoreItem.Folder, CoreItem.FullName, CoreItem.OrderIndex, CoreItem.IsDefault, CoreItem.Value.ToString() });
                            break;
                        }
                }
                if (ID > 0)
                {
                    Referral.CoreObjects.Add(new CoreObject(ID, ParentID, CoreItem.Entity, CoreItem.Folder, CoreItem.FullName, CoreItem.OrderIndex, CoreItem.IsDefault, CoreItem.Value.ToString()));

                    List<CoreObject> CopyCoreChildeList = CoreObject.FindChilds(CoreItem.CoreObjectID);
                    if (CopyCoreChildeList.Count > 0)
                        RebuildCore(CopyCoreChildeList, ID);

                }


            }
        }

        public JsonResult InformationEntryFormFieldRebuilding(long InformationEntryFormID)
        { 
            List<Field> NewField = DataConvertor.InformationEntryFormReady(new InformationEntryForm(CoreObject.Find(InformationEntryFormID)),true);
            List<CoreObject> OldFieldCore = CoreObject.FindChilds(InformationEntryFormID, CoreDefine.Entities.فیلد);

            int Index = 0;
            if (OldFieldCore.Count == 0)
            {
                foreach(Field NewFieldItem in NewField)
                {
                    string Value = Tools.ToXML(NewFieldItem);
                    long ID = Referral.DBCore.Insert("CoreObject"
                        , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                        , new object[] { InformationEntryFormID, CoreDefine.Entities.فیلد.ToString(), NewFieldItem.Folder, NewFieldItem.FieldName, Index, 0, Value });
                    if (ID > 0)
                    {
                        Referral.CoreObjects.Add(new CoreObject(ID, InformationEntryFormID, CoreDefine.Entities.فیلد, NewFieldItem.Folder, NewFieldItem.FieldName, Index, false, Value));
                        if(!NewFieldItem.IsVirtual)
                        { 
                            List<CoreObject> CopyCoreChildeList = CoreObject.FindChilds(NewFieldItem.CoreObjectID);
                            if (CopyCoreChildeList.Count > 0)
                                RebuildCore(CopyCoreChildeList, ID);
                        }
                    }
                }
            }
            else
            {
                foreach(CoreObject OldFieldCoreItem in OldFieldCore)
                {
                    bool IsFinde=false;
                    foreach(Field NewFieldItem in NewField)
                        if (NewFieldItem.FieldName== OldFieldCoreItem.FullName)
                        {
                            IsFinde=true;
                            break;   
                        }

                    if (!IsFinde)
                    {
                        if (Referral.DBCore.Execute("DELETE CoreObject WHERE CoreObjectID=" + OldFieldCoreItem.CoreObjectID.ToString()))
                        {
                            Referral.CoreObjects.Remove(OldFieldCoreItem);
                            OldFieldCore.Remove(OldFieldCoreItem);
                        }
                    }
                }

                foreach(Field NewFieldItem in NewField)
                {
                    bool IsFinde = false;

                    foreach(CoreObject coreObjectItem in OldFieldCore)
                    {
                        if(coreObjectItem.FullName==NewFieldItem.FieldName)
                        {
                            IsFinde = true;
                            break;
                        }
                    }
                    if(!IsFinde)
                    {
                        string Value = Tools.ToXML(NewFieldItem);
                        long ID = Referral.DBCore.Insert("CoreObject"
                            , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                            , new object[] { InformationEntryFormID, CoreDefine.Entities.فیلد.ToString(), NewFieldItem.Folder, NewFieldItem.FieldName, Index, 0, Value });
                        if (ID > 0)
                        {
                            Referral.CoreObjects.Add(new CoreObject(ID, InformationEntryFormID, CoreDefine.Entities.فیلد, NewFieldItem.Folder, NewFieldItem.FieldName, Index, false, Value));
                            List<CoreObject> CopyCoreChildeList = CoreObject.FindChilds(NewFieldItem.CoreObjectID);
                            if (CopyCoreChildeList.Count > 0)
                                RebuildCore(CopyCoreChildeList, ID);
                        }
                    }
                    Index++;
                    //foreach (CoreObject coreObject in OldFieldCore)
                    //{

                    //}

                }
            }
             
            return Json(new { Error =""});

        }

        public JsonResult ConvertToSQLQuery(string Query)
        {
            return Json(Tools.ConvertToSQLQuery(Query));
        }


        [HttpGet]
        public FileResult CreateBackup(long DatabaseID)
        {
            CoreObject coreObject = CoreObject.Find(DatabaseID);
            string FinallPath = "";
            switch (coreObject.Entity)
            {
                case CoreDefine.Entities.پایگاه_داده:
                    {
                        DataSourceInfo dataSourceInfo = new DataSourceInfo(coreObject);
                        SQLDataBase sqlDataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);

                        if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/")))
                        {
                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/"));
                        }

                        FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/") + dataSourceInfo.DataBase + "_" + CDateTime.GetNowshamsiDate().Replace("/", "") + CDateTime.GetNowTime().Replace(":", "") + ".bak";
                        sqlDataBase.Execute("BACKUP DATABASE " + dataSourceInfo.DataBase + " TO DISK = '" + FinallPath + "'; ");
                        break;
                    }
                case CoreDefine.Entities.جدول:
                    {
                        DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(coreObject.ParentID));
                        SQLDataBase sqlDataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);

                        if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/")))
                        {
                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/"));
                        }

                        FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/") + coreObject.FullName + ".xls";
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = ExcelVersion.Excel97to2003;


                            IWorkbook workbook = application.Workbooks.Create(1);
                            IWorksheet worksheet = workbook.Worksheets[0];
                            DataTable dataTable = DataConvertor.SelectDataTable(dataSourceInfo, "Select  * From " + coreObject.FullName, coreObject.FullName);
                            worksheet.ImportDataTable(dataTable, true, 1, 1, true);

                            //Creating Excel table or list object and apply style to the table
                            IListObject table = worksheet.ListObjects.Create("Employee_PersonalDetails", worksheet.UsedRange);

                            table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;

                            //Autofit the columns
                            worksheet.UsedRange.AutofitColumns();


                            try
                            {
                                workbook.SaveAs(FinallPath);
                            }
                            catch (Exception ex)
                            {

                            }


                            //IWorkbook workbook = application.Workbooks.Open(FinallPath);

                            //IWorkbook NewWoorkbook = application.Workbooks.Create(1);
                            //IWorksheet NewWorksheet = NewWoorkbook.Worksheets[0];

                            //IWorksheet worksheet = workbook.Worksheets.AddCopy(NewWorksheet);
                            //worksheet.Name = coreObject.FullName;
                            //worksheet.ClearData();
                            //worksheet.Range["A1"].Text = "شناسه";

                            //workbook.SaveAs(FinallPath);
                        }
                        break;
                    }
                case CoreDefine.Entities.ضمیمه_جدول:
                    {
                        FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/DownloadTableAttachement");
                        if (!Directory.Exists(FinallPath))
                            Directory.CreateDirectory(FinallPath);

                        CoreObject TableCore = CoreObject.Find(coreObject.ParentID);
                        Table table = new Table(TableCore);
                        string IdFild = table.IDField().FieldName;
                        string Query = "Select " + IdFild;

                        foreach (CoreObject FieldCore in CoreObject.FindChilds(TableCore.CoreObjectID, CoreDefine.Entities.فیلد))
                        {
                            if (FieldCore.IsDefault)
                                Query += "," + FieldCore.FullName;
                        }
                        Query += " From " + table.FullName;

                        DataTable dataTable = DataConvertor.SelectDataTable(new DataSourceInfo(CoreObject.Find(TableCore.ParentID)), Query, table.FullName);
                        string Path = Models.Attachment.MapFileSavingAttachmentPath;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            try
                            {
                                DirectoryInfo dir = new DirectoryInfo(Path + TableCore.CoreObjectID.ToString() + "/" + row[0].ToString());
                                if (dir.Exists)
                                {
                                    foreach (FileInfo fileInfo in dir.GetFiles())
                                    {
                                        if (fileInfo.Name.Replace(fileInfo.Extension, "") == coreObject.FullName)
                                        {
                                            string FileName = "";
                                            foreach (DataColumn column in dataTable.Columns)
                                                if (column.ColumnName != IdFild)
                                                    FileName += "_" + row[column.ColumnName];
                                            fileInfo.CopyTo(FinallPath + "\\" + FileName + fileInfo.Extension);
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        break;
                    }
            }
            FinallPath = Attachment.ConvertToZipFile(new string[] { FinallPath }, DatabaseID);

            FileInfo file = new FileInfo(FinallPath); 
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment;filename=\"" + Path.GetFileName(file.Name) + "\"");
            Response.AppendHeader("content-length", file.Length.ToString());
            Response.Buffer = false;
            Response.TransmitFile(FinallPath);  
            if(Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString())))
            Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/"), true); 
            if(Directory.Exists(file.Directory.FullName))
            Directory.Delete(file.Directory.FullName, true); 
            return null;
        }
        

        [HttpGet]
        public FileResult CreateBackupCoreDatabase()
        {
            string FinallPath = "";  
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/")))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/"));
            }

            FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/") + Referral.DBCore.ConnectionData.DataBase + "_" + CDateTime.GetNowshamsiDate().Replace("/", "") + CDateTime.GetNowTime().Replace(":", "") + ".bak";
            Referral.DBCore.Execute("BACKUP DATABASE " + Referral.DBCore.ConnectionData.DataBase + " TO DISK = '" + FinallPath + "'; ");

            FinallPath = Attachment.ConvertToZipFile(new string[] { FinallPath });

            FileInfo file = new FileInfo(FinallPath);
             
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment;filename=\"" + Path.GetFileName(file.Name) + "\"");
            Response.AppendHeader("content-length", file.Length.ToString());
            Response.Buffer = false;
            Response.TransmitFile(FinallPath);
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString())))
                Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/"), true);
            if (Directory.Exists(file.Directory.FullName))
                Directory.Delete(file.Directory.FullName, true);
            return null;

        }

        public ActionResult ShowAPMCoreSetting(int ActionNum)
        {
            ViewData["ActionNum"] = ActionNum;
            return View("~/Views/SysSetting/APMCoreSetting/Index.cshtml");
        }

        public JsonResult RunAPMCoreQyery(string Query)
        {
            return Json(Referral.DBData.Execute(Query.Replace("$**$","<")));

        }

        public ActionResult ReadAPMCoreSetting([DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = new JsonResult();
      
            if (Session["APMCoreSettingData"]==null)
            {
                Session["APMCoreSettingData"] = Referral.DBCore.SelectDataTable("SELECT CoreObjectID,ParentID,Entity,Folder,FullName,OrderIndex,IsDefault  FROM CoreObject ");
            }  
            else
            {
                DataTable CoreData = (DataTable)Session["APMCoreSettingData"];
                List<long> levels = CoreData.AsEnumerable().Select(al => al.Field<long>("CoreObjectID")).Distinct().ToList(); 
                foreach(CoreObject core in Referral.CoreObjects)
                {
                    if(levels.IndexOf(core.CoreObjectID)==-1)
                    {
                        Session["APMCoreSettingData"] = Referral.DBCore.SelectDataTable("SELECT CoreObjectID,ParentID,Entity,Folder,FullName,OrderIndex,IsDefault  FROM CoreObject ");
                        break;
                    }

                } 
            }

            jsonResult = Json(((DataTable)Session["APMCoreSettingData"]).ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public JsonResult CreateAPMCoreScript(long FromCoreObjectID, long ToCoreObjectID,bool ScriptType)
        {
            string CoreQuery = String.Empty;
            string DataQuery = "";
            IEnumerable<CoreObject> CoreObjectList = Referral.CoreObjects.Where(x => x.CoreObjectID >= FromCoreObjectID && x.CoreObjectID <= ToCoreObjectID);

            if (ScriptType)
            {
                CoreQuery = "USE [" + Referral.DBCore.ConnectionData.DataBase + "]\n";
                CoreQuery += "SET IDENTITY_INSERT[dbo].[CoreObject] ON\n";
                if(CoreObjectList.Count()==0)
                {
                    DataTable CoreData = Referral.DBCore.SelectDataTable("SELECT  CoreObjectID, ParentID, Entity, Folder, FullName, OrderIndex, IsDefault, value FROM  CoreObject where CoreObjectID >= " + FromCoreObjectID + " and CoreObjectID <="+ ToCoreObjectID + "  order by OrderIndex");
                    foreach (DataRow Row in CoreData.Rows)
                    {
                        int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == long.Parse(Row["CoreObjectID"].ToString()));
                        if (CoreIndex < 0)
                        {
                            CoreObject SourceCore = new CoreObject(
                               Convert.ToInt64(Row["CoreObjectID"].ToString()),
                               Convert.ToInt64(Row["ParentID"].ToString()),
                               Tools.GetEntity(Row["Entity"].ToString()),
                               Row["Folder"].ToString(),
                               Row["FullName"].ToString(),
                               Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                               (bool)Row["IsDefault"],
                               (object)Row["Value"]
                                );
                            Referral.CoreObjects.Add(SourceCore); 
                        }
                        else
                            Referral.CoreObjects[CoreIndex].Value = Row["value"];
                    }
                    CoreObjectList = Referral.CoreObjects.Where(x => x.CoreObjectID >= FromCoreObjectID && x.CoreObjectID <= ToCoreObjectID);
                }
                foreach (CoreObject coreItem in CoreObjectList)
                {
                    if(coreItem.Entity==CoreDefine.Entities.گزارش)
                    {
                        CoreObject ReportSourceCore = CoreObject.Find(coreItem.CoreObjectID, CoreDefine.Entities.منبع_گزارش, "");
                        if (ReportSourceCore.CoreObjectID > 0)
                        {

                            CoreQuery += "INSERT [dbo].[CoreObject] ([CoreObjectID], [ParentID], [Entity], [Folder], [FullName], [OrderIndex], [IsDefault], [Value]) VALUES (" + ReportSourceCore.CoreObjectID.ToString() + "," + ReportSourceCore.ParentID.ToString() + ",N'" + ReportSourceCore.Entity.ToString() + "',N'" + ReportSourceCore.Folder + "',N'" + ReportSourceCore.FullName + "'," + ReportSourceCore.OrderIndex + "," + (ReportSourceCore.IsDefault ? "1" : "0") + ",N'" + ReportSourceCore.Value.ToString().Replace("'", "''").Replace("&gt;", "<") + "')\n";
                            CoreQuery += "\n";
                        }
                        else
                        {
                            DataTable CoreData = Referral.DBCore.SelectDataTable("Select top 1 * from Coreobject where parentID=" + coreItem.CoreObjectID + " and entity=N'منبع_گزارش'");
                            foreach (DataRow Row in CoreData.Rows)
                            {
                                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == long.Parse(Row["CoreObjectID"].ToString()));
                                if (CoreIndex < 0)
                                {
                                    Referral.CoreObjects.Add(new CoreObject(
                                       Convert.ToInt64(Row["CoreObjectID"].ToString()),
                                       Convert.ToInt64(Row["ParentID"].ToString()),
                                       Tools.GetEntity(Row["Entity"].ToString()),
                                       Row["Folder"].ToString(),
                                       Row["FullName"].ToString(),
                                       Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                                       (bool)Row["IsDefault"],
                                       (object)Row["Value"]
                                        ));
                                }
                                else
                                    Referral.CoreObjects[CoreIndex].Value = Row["value"];

                                ReportSourceCore = CoreObject.Find(coreItem.CoreObjectID, CoreDefine.Entities.منبع_گزارش, "");
                                CoreQuery += "INSERT [dbo].[CoreObject] ([CoreObjectID], [ParentID], [Entity], [Folder], [FullName], [OrderIndex], [IsDefault], [Value]) VALUES (" + ReportSourceCore.CoreObjectID.ToString() + "," + ReportSourceCore.ParentID.ToString() + ",N'" + ReportSourceCore.Entity.ToString() + "',N'" + ReportSourceCore.Folder + "',N'" + ReportSourceCore.FullName + "'," + ReportSourceCore.OrderIndex + "," + (ReportSourceCore.IsDefault ? "1" : "0") + ",N'" + ReportSourceCore.Value.ToString().Replace("'", "''").Replace("&gt;", "<") + "')\n";
                                CoreQuery += "\n";
                            }
                        }
                    }
                    CoreQuery += "INSERT [dbo].[CoreObject] ([CoreObjectID], [ParentID], [Entity], [Folder], [FullName], [OrderIndex], [IsDefault], [Value]) VALUES (" + coreItem.CoreObjectID.ToString() + "," + coreItem.ParentID.ToString() + ",N'" + coreItem.Entity.ToString() + "',N'" + coreItem.Folder + "',N'" + coreItem.FullName + "'," + coreItem.OrderIndex + "," + (coreItem.IsDefault ? "1" : "0") + ",N'" + coreItem.Value.ToString().Replace("'","''").Replace("&gt;", "<") + "')\n";
                    CoreQuery += "\n";

                }

                foreach (CoreObject coreItem in CoreObjectList)
                {
                    switch (coreItem.Entity)
                    {
                        case CoreDefine.Entities.جدول:
                            {
                                Table TableInfo = new Table(coreItem);
                                DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(coreItem.ParentID));
                                Field IdField = TableInfo.IDField();
                                DataQuery += "CREATE TABLE [" + dataSourceInfo.DataBase + "].[" + TableInfo.TABLESCHEMA + "].[" + coreItem.FullName + "] (" + IdField.FieldName + " bigint IDENTITY(1,1) NOT NULL , PRIMARY KEY (" + IdField.FieldName + "))\n";
                                DataQuery += "\n";
                                break;
                            }
                        case CoreDefine.Entities.فیلد:
                            {
                                Field field = new Field(coreItem);
                                CoreObject ParentField = CoreObject.Find(coreItem.ParentID);
                                if (!field.IsIdentity && ParentField.Entity== CoreDefine.Entities.جدول)
                                {
                                    Table TableInfo = new Table(ParentField);
                                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(coreItem.ParentID).ParentID));
                                    DataQuery += "\nALTER TABLE  [" + dataSourceInfo.DataBase + "].[" + TableInfo.TABLESCHEMA + "].[" + TableInfo.FullName + "] ADD  " + field.FieldName + "  ";

                                    switch (field.FieldType)
                                    {
                                        case CoreDefine.InputTypes.Clock:
                                        case CoreDefine.InputTypes.NationalCode:
                                        case CoreDefine.InputTypes.PersianDateTime:
                                        case CoreDefine.InputTypes.MiladyDateTime:
                                        case CoreDefine.InputTypes.ShortText:
                                        case CoreDefine.InputTypes.FillTextAutoComplete:
                                        case CoreDefine.InputTypes.ComboBox:
                                        case CoreDefine.InputTypes.Password:
                                        case CoreDefine.InputTypes.Phone:
                                        case CoreDefine.InputTypes.Plaque:
                                        case CoreDefine.InputTypes.Rating:
                                            {
                                                DataQuery += "Nvarchar(400)";
                                                break;
                                            }
                                        case CoreDefine.InputTypes.LongText:
                                        case CoreDefine.InputTypes.Editor:
                                            {
                                                DataQuery += "Nvarchar(MAX)";
                                                break;
                                            }
                                        case CoreDefine.InputTypes.TwoValues:
                                            {
                                                DataQuery += "Bit";
                                                break;
                                            }
                                        case CoreDefine.InputTypes.Number:
                                        case CoreDefine.InputTypes.RelatedTable:
                                        case CoreDefine.InputTypes.Money:
                                        case CoreDefine.InputTypes.CoreRelatedTable:
                                        case CoreDefine.InputTypes.Sparkline:
                                            {
                                                if (field.DigitsAfterDecimal > 0)
                                                    DataQuery += "Float";
                                                else
                                                    DataQuery += "Bigint";
                                                break;
                                            }
                                    }
                                }
                                break;
                            }
                    }

                }

                CoreQuery += "SET IDENTITY_INSERT [dbo].[CoreObject] OFF\n" ;
            }
            else
            { 

                foreach (CoreObject coreItem in CoreObjectList)
                {
                    CoreQuery += "Update ["+ Referral.DBCore.ConnectionData.DataBase + "].[dbo].[CoreObject] SET [ParentID] = "+ coreItem.ParentID.ToString() + ", [Entity]=N'"+ coreItem.Entity.ToString() + "', [Folder]=N'"+ coreItem.Folder + "', [FullName]=N'" + coreItem.FullName + "', [OrderIndex]="+ coreItem.OrderIndex + ", [IsDefault]=" + (coreItem.IsDefault ? "1" : "0") + ", [Value]=N'" + coreItem.Value.ToString().Replace("'", "''").Replace("&gt;", "<") + "' \nWhere [CoreObjectID]="+ coreItem.CoreObjectID + " \n";
                    CoreQuery += "\n\n";

                }

                //foreach (CoreObject coreItem in CoreObjectList)
                //{
                //    switch (coreItem.Entity)
                //    {
                //        case CoreDefine.Entities.جدول:
                //            {
                //                Table TableInfo = new Table(coreItem);
                //                DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(coreItem.ParentID));
                //                Field IdField = TableInfo.IDField();
                //                DataQuery += "CREATE TABLE [" + dataSourceInfo.DataBase + "].[" + TableInfo.TABLESCHEMA + "].[" + coreItem.FullName + "] (" + IdField.FieldName + " bigint IDENTITY(1,1) NOT NULL , PRIMARY KEY (" + IdField.FieldName + "))\n";
                //                DataQuery += "GO\n";
                //                break;
                //            }
                //        case CoreDefine.Entities.فیلد:
                //            {
                //                Field field = new Field(coreItem);
                //                if (!field.IsIdentity)
                //                {
                //                    Table TableInfo = new Table(CoreObject.Find(coreItem.ParentID));
                //                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(coreItem.ParentID).ParentID));
                //                    DataQuery += "\nALTER TABLE  [" + dataSourceInfo.DataBase + "].[" + TableInfo.TABLESCHEMA + "].[" + TableInfo.FullName + "] ADD  " + field.FieldName + "  ";

                //                    switch (field.FieldType)
                //                    {
                //                        case CoreDefine.InputTypes.Clock:
                //                        case CoreDefine.InputTypes.NationalCode:
                //                        case CoreDefine.InputTypes.PersianDateTime:
                //                        case CoreDefine.InputTypes.MiladyDateTime:
                //                        case CoreDefine.InputTypes.ShortText:
                //                        case CoreDefine.InputTypes.FillTextAutoComplete:
                //                        case CoreDefine.InputTypes.ComboBox:
                //                        case CoreDefine.InputTypes.Password:
                //                        case CoreDefine.InputTypes.Phone:
                //                        case CoreDefine.InputTypes.Plaque:
                //                        case CoreDefine.InputTypes.Rating:
                //                            {
                //                                DataQuery += "Nvarchar(400)";
                //                                break;
                //                            }
                //                        case CoreDefine.InputTypes.LongText:
                //                        case CoreDefine.InputTypes.Editor:
                //                            {
                //                                DataQuery += "Nvarchar(MAX)";
                //                                break;
                //                            }
                //                        case CoreDefine.InputTypes.TwoValues:
                //                            {
                //                                DataQuery += "Bit";
                //                                break;
                //                            }
                //                        case CoreDefine.InputTypes.Number:
                //                        case CoreDefine.InputTypes.RelatedTable:
                //                        case CoreDefine.InputTypes.Money:
                //                        case CoreDefine.InputTypes.CoreRelatedTable:
                //                        case CoreDefine.InputTypes.Sparkline:
                //                            {
                //                                if (field.DigitsAfterDecimal > 0)
                //                                    DataQuery += "Float";
                //                                else
                //                                    DataQuery += "Bigint";
                //                                break;
                //                            }
                //                    }
                //                }
                //                break;
                //            }
                //    }

                //}

                CoreQuery += "\n";
            }



            return Json(CoreQuery+"\n\n\n--****************************************************************************************************************--\n\n\n"+DataQuery);
        }

        public ActionResult GetCoreObjectTreeView(long ParentID,string Entity)
        { 
            ViewData["ParentID"] = ParentID;
            ViewData["Entity"] = Tools.GetEntityFromEnglishName(Entity);
            return View("~/Views/SysSetting/CoreObjectTree.cshtml");
        }

        public JsonResult SaveCoreObjectTreeView(long ElementId,string SelectedCoreObject)
        {
            Session["SaveCoreObjectTreeView"] = SelectedCoreObject;
            Session["TableAttachamentCoreObject"] = ElementId;
            return Json(true);
        }

        [HttpGet]
        public FileResult CreateScriptCoreWithID(long CoreID)
        {
            string Text = CreateAPMCoreScript(CoreID, CoreID, true).Data.ToString(); 
            List<CoreObject> CoreObjects = CoreObject.FindChilds(CoreID);
            foreach (CoreObject coreObject in CoreObjects)
            {
                Text += CreateAPMCoreScript(coreObject.CoreObjectID, coreObject.CoreObjectID, true).Data.ToString();
            }
            if (Text != "")
            {
                string FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/");

                if (!Directory.Exists(FinallPath))
                {
                    Directory.CreateDirectory(FinallPath);
                }

                string FileName = Referral.DBRegistry.ConnectionData.DataBase.Replace("Registry", "") + "ScriptCore" + CDateTime.GetNowshamsiDate().Replace("/", "") + CDateTime.GetNowTime().Replace(":", "");
                System.IO.File.WriteAllText(FinallPath + "/" + FileName + ".txt", Text, Encoding.UTF8);
                FinallPath += "/" + FileName + ".txt";
                FileInfo file = new FileInfo(FinallPath);
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "application/txt";
                Response.AddHeader("content-disposition", "attachment;filename=\"" + Path.GetFileName(file.Name) + "\"");
                Response.AppendHeader("content-length", file.Length.ToString());
                Response.Buffer = false;
                Response.TransmitFile(FinallPath);
                if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString())))
                    Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/"), true);
                if (Directory.Exists(file.Directory.FullName))
                    Directory.Delete(file.Directory.FullName, true); 
            }
            return null;
        }
        [HttpGet]
        public FileResult CreateScriptCore(string ScriptCoreFromDate, string ScriptCoreFromTime, string ScriptCoreToDate, string ScriptCoreToTime)
        {
            string Query = "declare @FormDate as nvarchar(400)=N'"+ ScriptCoreFromDate + "'\n" +
                "declare @FormTime as nvarchar(400)=N'"+ ScriptCoreFromTime + "'\n" +
                "declare @ToDate as nvarchar(400)=N'"+ ScriptCoreToDate + "'\n" +
                "declare @ToTime as nvarchar(400)=N'"+ ScriptCoreToTime + "'\n" +
                "declare @tble Table(EventType  nvarchar(400),RegistryDate  nvarchar(400),RegistryTime   nvarchar(400),TableName   nvarchar(400),RecordID Bigint,Value nvarchar(Max))\n" +
                "insert into @tble\n" +
                "SELECT \n" +
                "      N'Insert'\n" +
                "      ,RegistryDate\n" +
                "      ,RegistryTime \n" +
                "      ,TableName \n" +
                "      ,RecordID\n" +
                "      , CAST(Value as nvarchar(Max))\n" +
                "  FROM  Insert_APMRegistry\n" +
                "  where\n" +
                "(  (RegistryDate+RegistryTime)>=(@FormDate+@FormTime))\n" +
                "and ((RegistryDate+RegistryTime)<=(@ToDate+@ToTime))\n" +
                "and TableName=N'CoreObject'\n" +
                "union\n" +
                "SELECT \n" +
                "      N'Update'\n" +
                "      ,RegistryDate\n" +
                "      ,RegistryTime \n" +
                "      ,TableName \n" +
                "      ,RecordID \n" +
                "      , CAST(NewValue as nvarchar(Max))\n" +
                "  FROM  Update_APMRegistry\n" +
                "  where\n" +
                "(  (RegistryDate+RegistryTime)>=(@FormDate+@FormTime))\n" +
                "and ((RegistryDate+RegistryTime)<=(@ToDate+@ToTime))\n" +
                "and TableName=N'CoreObject'\n" +
                "union\n" +
                "SELECT \n" +
                "       N'Delete' \n" +
                "      ,RegistryDate\n" +
                "      ,RegistryTime \n" +
                "      ,TableName\n" +
                "      ,RecordID \n" +
                "      , CAST(Value as nvarchar(Max))\n" +
                "  FROM  Delete_APMRegistry\n" +
                "  where\n" +
                "((RegistryDate+RegistryTime)>=(@FormDate+@FormTime))\n" +
                "and ((RegistryDate+RegistryTime)<=(@ToDate+@ToTime))\n" +
                "and TableName=N'CoreObject'\n" +
                "select * from @tble\n" +
                "order by (RegistryDate+RegistryTime)\n";

            DataTable RegistryData= Referral.DBRegistry.SelectDataTable(Query);
            string Text = string.Empty;
            List<string> EventTypeList = new List<string>();
            List<string> RecordIDList = new List<string>();
            foreach (DataRow Row in RegistryData.Rows)
            { 
                if (RecordIDList.FindIndex(x=>x== Row["RecordID"].ToString()) ==-1 || Row["EventType"].ToString()== "Delete" || Row["EventType"].ToString() == "Insert")
                {
                    Text += "\n";
                    switch (Row["EventType"].ToString())
                    {
                        case "Insert": {if(CoreObject.Find((long)Row["RecordID"]).CoreObjectID>0) Text+=CreateAPMCoreScript((long) Row["RecordID"], (long) Row["RecordID"], true).Data; break;  }
                        case "Update": { if (CoreObject.Find((long)Row["RecordID"]).CoreObjectID > 0) Text += CreateAPMCoreScript((long)Row["RecordID"], (long)Row["RecordID"], false ).Data; break; }
                        case "Delete": { Text += "\nDELETE [" + Referral.DBCore.ConnectionData.DataBase + "].[dbo].[CoreObject] WHERE CoreObjectID = "+ Row["RecordID"].ToString()+"\n";

                                var stringReader = new System.IO.StringReader(Row["Value"].ToString());
                                var serializer = new XmlSerializer(typeof(RecordData));
                                var AttachmentRecordData = serializer.Deserialize(stringReader) as RecordData;
                                CoreObject _Object = new CoreObject() { Entity = Tools.GetEntity(AttachmentRecordData.Items.Find(x => x.Name == "Entity").Value.ToString()),
                                                                            ParentID = long.Parse(AttachmentRecordData.Items.Find(x => x.Name == "ParentID").Value.ToString()),
                                                                            FullName= AttachmentRecordData.Items.Find(x => x.Name == "FullName").Value.ToString()

                                }; 

                                switch (_Object.Entity)
                                {
                                    case CoreDefine.Entities.فیلد:
                                        {
                                            CoreObject TableCore = CoreObject.Find(_Object.ParentID);
                                            if(TableCore.CoreObjectID>0)
                                            if (TableCore.Entity == CoreDefine.Entities.جدول)
                                            {
                                                Table TableInfo = new Table(TableCore);
                                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                                                switch (DataSourceInfo.DataSourceType)
                                                {
                                                    case CoreDefine.DataSourceType.SQLSERVER:
                                                        { 
                                                            Text += ("ALTER TABLE " + DataSourceInfo.DataBase+"."+TableInfo.TABLESCHEMA + "." + TableCore.FullName + " DROP COLUMN " + _Object.FullName); 
                                                            break;
                                                        }
                                                    case CoreDefine.DataSourceType.MySql:
                                                        { 
                                                            Text += "ALTER TABLE dbo." + DataSourceInfo.DataBase + "." + TableCore.FullName + " DROP COLUMN " + _Object.FullName ;
                                                            break;
                                                        }

                                                    case CoreDefine.DataSourceType.ACCESS:
                                                        { 
                                                            Text += "Alter table [" + TableCore.FullName + "] drop column [" + _Object.FullName + "]" ; 
                                                            break;
                                                        }
                                                    case CoreDefine.DataSourceType.EXCEL:
                                                        {
                                                            using (ExcelEngine excelEngine = new ExcelEngine())
                                                            {
                                                                IApplication application = excelEngine.Excel;

                                                                application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                                                IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                                                IWorksheet worksheet = workbook.Worksheets[TableCore.FullName.Replace("$", "").Replace("'", "")];

                                                                foreach (IRange Column in worksheet.Columns)
                                                                {
                                                                    if (Column.DisplayText.Replace("\n", "_") == _Object.FullName)
                                                                    {
                                                                        worksheet.DeleteColumn(Column.Column);
                                                                        break;
                                                                    }
                                                                }

                                                                workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase); 
                                                            }
                                                            break;
                                                        }
                                                } 
                                            } 
                                            break;
                                        }
                                    case CoreDefine.Entities.جدول:
                                        {
                                            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(_Object.ParentID));
                                            switch (DataSourceInfo.DataSourceType)
                                            {
                                                case CoreDefine.DataSourceType.SQLSERVER:
                                                    {
                                                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                                        Text += "DROP TABLE " + _Object.FullName;  

                                                        break;
                                                    }

                                                case CoreDefine.DataSourceType.MySql:
                                                    {  
                                                        break;
                                                    }

                                                case CoreDefine.DataSourceType.ACCESS:
                                                    { 

                                                        break;
                                                    }

                                                case CoreDefine.DataSourceType.EXCEL:
                                                    { 
                                                        break;
                                                    }
                                            }

                                            break;
                                        }

                                    case CoreDefine.Entities.پایگاه_داده:
                                        {
                                            //if (DeleteType == "حذف از هسته نرم افزار")
                                            //{
                                            //    result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                                            //}
                                            //else
                                            //{
                                            //    DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObjectID));
                                            //    switch (DataSourceInfo.DataSourceType)
                                            //    {
                                            //        case CoreDefine.DataSourceType.SQLSERVER:
                                            //            {
                                            //                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);

                                            //                Text += " DROP DATABASE " + DataSourceInfo.DataBase + " ;   ";

                                            //                break;
                                            //            }
                                            //        case CoreDefine.DataSourceType.MySql:
                                            //            {
                                            //                MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);

                                            //                if (DataBase.Execute("USE master ;GO DROP DATABASE " + DataSourceInfo.DataBase + " ;GO   "))
                                            //                {
                                            //                    if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            //                    {
                                            //                        int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                            //                        Referral.CoreObjects.RemoveAt(CoreIndex);
                                            //                    }

                                            //                    result = true;
                                            //                }

                                            //                break;
                                            //            }
                                            //        case CoreDefine.DataSourceType.EXCEL:
                                            //            {
                                            //                if (Models.Attachment.DeleteFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                            //                {
                                            //                    if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            //                    {
                                            //                        int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                            //                        Referral.CoreObjects.RemoveAt(CoreIndex);
                                            //                    }

                                            //                    result = true;
                                            //                }

                                            //                break;
                                            //            }
                                            //        case CoreDefine.DataSourceType.ACCESS:
                                            //            {
                                            //                if (Models.Attachment.DeleteFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                                            //                {
                                            //                    if (Referral.DBCore.Delete("CoreObject", CoreObjectID))
                                            //                    {
                                            //                        int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == CoreObjectID);
                                            //                        Referral.CoreObjects.RemoveAt(CoreIndex);
                                            //                    }

                                            //                    result = true;
                                            //                }

                                            //                break;
                                            //            }
                                            //    }
                                            //}
                                            break;
                                        }
                                         
                                    case CoreDefine.Entities.رویداد_عمومی:
                                        {
                                            //    PublicJob PublicJob = new PublicJob(_Object);
                                            //    if (PublicJob.PublicJobType == CoreDefine.PublicJobType.اجرای_رویداد)
                                            //    {
                                            //        DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(PublicJob.RelatedDatasource));
                                            //        switch (DataSourceInfo.DataSourceType)
                                            //        {
                                            //            case CoreDefine.DataSourceType.SQLSERVER:
                                            //                {
                                            //                    string Qeury = @"EXEC sp_delete_job  @job_name = N'" + _Object.FullName + "'";
                                            //                    SQLDataBase sQLDataBase = Referral.DBMsdb;
                                            //                    if (DataSourceInfo.ServerName != Referral.DBMsdb.ConnectionData.Source)
                                            //                        sQLDataBase = new SQLDataBase(DataSourceInfo.ServerName, "msdb", DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                            //                    if (sQLDataBase.Execute(Qeury))
                                            //                    {
                                            //                        result = Referral.DBCore.Delete("CoreObject", CoreObjectID);
                                            //                    }

                                            //                    break;
                                            //                }
                                            //        }
                                            //    }
                                            break;
                                        }
                                    default:
                                        { 
                                            break;
                                        }
                                }
                                break; }
                    }
                    RecordIDList.Add(Row["RecordID"].ToString());
                }
            }
            if(Text!="")
            {
                Text = Text.Replace("< ", "> ").Replace("<=", ">=");
                string FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/DownloadTableAttachement");

                if (!Directory.Exists(FinallPath))
                {
                    Directory.CreateDirectory(FinallPath);
                }

                string FileName = Referral.DBRegistry.ConnectionData.DataBase.Replace("Registry", "") + "ScriptCore" + CDateTime.GetNowshamsiDate().Replace("/", "") + CDateTime.GetNowTime().Replace(":", "");
                System.IO.File.WriteAllText(FinallPath + "/"+ FileName + ".txt" , Text, Encoding.UTF8);
                FinallPath += "/" + FileName + ".txt";
                FileInfo file = new FileInfo(FinallPath );
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.ContentType = "application/txt";
                Response.AddHeader("content-disposition", "attachment;filename=\"" + Path.GetFileName(file.Name) + "\"");
                Response.AppendHeader("content-length", file.Length.ToString());
                Response.Buffer = false;
                Response.TransmitFile(FinallPath);
                Directory.Delete(file.Directory.FullName, true);
            }
            return null;
        }
        
        public JsonResult CreateDownloadAPMCoreQyery(string Text)
        {
            if (Text == null)
                return Json("");

            string FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/DownloadTableAttachement");

            if (!Directory.Exists(FinallPath))
            {
                Directory.CreateDirectory(FinallPath);
            }

            string FileName = Referral.DBRegistry.ConnectionData.DataBase.Replace("Registry", "") + "ScriptCore" + CDateTime.GetNowshamsiDate().Replace("/", "") + CDateTime.GetNowTime().Replace(":", "");
            System.IO.File.WriteAllText(FinallPath + "/" + FileName + ".txt", Text.Replace("&gt;", "<"), Encoding.UTF8);
            return Json(FileName);
        }
        [HttpGet]
        public FileResult DownloadAPMCoreQyery(string FileName)
        {  
            string FinallPath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment/Backup/" + Referral.UserAccount.UsersID.ToString() + "/DownloadTableAttachement"); 
            FinallPath += "/" + FileName + ".txt";
            FileInfo file = new FileInfo(FinallPath );
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = "application/txt";
            Response.AddHeader("content-disposition", "attachment;filename=\"" + Path.GetFileName(file.Name) + "\"");
            Response.AppendHeader("content-length", file.Length.ToString());
            Response.Buffer = false;
            Response.TransmitFile(FinallPath);
            Directory.Delete(file.Directory.FullName, true); 
            return null;
        }
        

    }
}