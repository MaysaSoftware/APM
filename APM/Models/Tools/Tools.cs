using APM.Models.Database;
using APM.Models.Diagram;
using APM.Models.NetWork;
using APM.Models.Security;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static APM.Models.Tools.Software;

namespace APM.Models.Tools
{
    public class Tools
    {
        public static string NewLine = "\n";

        public static object ParameterField(string FieldTitle, string FieldName, object FieldValue, string InputType = "ShortText",bool IsReadonly = false,bool IsRequired = false,string Entitiy ="",long ParentID=0, List<SelectListItem> ComboItems = null , bool IsGridField = false,bool IsLeftWrite=false,string FalseValue = "خیر",string TrueValue = "بله",bool IsInCellEditMode = false,int  DigitsAfterDecimal = 0,long RelatedField = 0,long RelatedTable = 0,long _TableID = 0)
        {
            return new { FieldTitle= FieldTitle, FieldName = FieldName, FieldValue= FieldValue, InputType = InputType, IsReadonly = IsReadonly, IsRequired = IsRequired, Entitiy= Entitiy, ParentID= ParentID.ToString(), ComboItems = ComboItems, IsGridField = IsGridField, IsLeftWrite= IsLeftWrite, FalseValue = FalseValue, TrueValue = TrueValue, IsInCellEditMode= IsInCellEditMode, DigitsAfterDecimal= DigitsAfterDecimal, RelatedField= RelatedField , RelatedTable = RelatedTable, _TableID= _TableID };
        }

        public static string N(string Text)
        {
            return "N'" + Text + "'";
        }
        public static string ToXML(object _Object)
        {
            Log.LogFunction("Tools.ToXML", true);
            var RamStream = new MemoryStream();
            var Serializer = new System.Xml.Serialization.XmlSerializer(_Object.GetType());
            var Writer = new StreamWriter(RamStream, Encoding.UTF8);
            Serializer.Serialize(Writer, _Object);
            RamStream.Position = 0L;
            var Reader = new StreamReader(RamStream, true);
            string Output = Reader.ReadToEnd();
            Reader.Close();
            return Output.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>","");
        }
        public static string[] VString(params object[] _Values)
        {
            var Output = new string[_Values.Length];
            for (int Index = 0, loopTo = _Values.Length - 1; Index <= loopTo; Index++)
            {
                if (_Values[Index] is object)
                {
                    Output[Index] = _Values[Index].ToString();
                }
                else
                {
                    Output[Index] = "";
                }
            }

            return Output;
        }
        public static int IsInArrayIndex(object _Key, object[] _Array, bool _TypeCompare = false, bool _CaseSensitive = true)
        {
            var Index = default(int);
            if (_TypeCompare)
            {
                foreach (Type Item in _Array)
                {
                    if (ReferenceEquals(_Key, Item))
                    {
                        return Index;
                    }

                    Index += 1;
                }
            }
            else
            {
                foreach (object Item in _Array)
                {
                    if(Item!=null)
                    if (_CaseSensitive)
                    {
                        if (_Key == Item)
                        {
                            return Index;
                        }
                    }
                    else if ((_Key.ToString().ToLower() ?? "") == (Item.ToString().ToLower() ?? ""))
                    {
                        return Index;
                    }

                    Index += 1;
                }
            }

            return -1;
        }

        public static string UnSafeTitle(string _Text)
        {
            return string.IsNullOrEmpty(_Text) ?"": _Text.Replace("_", " ");
        }

        public static string SafeTitle(string _Text)
        {
            try
            { 
                return _Text.Replace(" ", "_").Replace("\n", "").Replace("+", "_").Replace("-", "_").Replace("%", "_").Replace("$", "_").Replace(":", "_");
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetIPAddress()
        {
            string IPAddress = HttpContext.Current.Request.UserHostAddress;
            //string IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(IPAddress))
            //{
            //    IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            return IPAddress; 
        }

        public static string GetPCName()
        {
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            //return  Dns.GetHostEntry(context.Request.ServerVariables["REMOTE_ADDR"]).HostName;
            return "";// Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).ToString();

        }

        public static string GetBrowserType()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            return  context.Request.Browser.Type;
        }

        public static string GetBrowserVersion()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            return context.Request.Browser.Version;
        }

        public static CoreDefine.Entities GetEntity(string _EntityName)
        {
            switch (_EntityName)
            {
                case "جدول":
                    return CoreDefine.Entities.جدول; 
                case "رویداد_جدول":
                    return CoreDefine.Entities.رویداد_جدول; 
                case "دکمه_رویداد_جدول":
                    return CoreDefine.Entities.دکمه_رویداد_جدول; 
                case "فیلد":
                    return CoreDefine.Entities.فیلد;
                case "رویداد_نمایش_فیلد":
                    return CoreDefine.Entities.رویداد_نمایش_فیلد;
                case "فرآیند":
                    return CoreDefine.Entities.فرآیند;
                case "مرحله_فرآیند":
                    return CoreDefine.Entities.مرحله_فرآیند;
                case "ارجاع_مرحله_فرآیند":
                    return CoreDefine.Entities.ارجاع_مرحله_فرآیند;
                case "رویداد_مرحله_فرآیند":
                    return CoreDefine.Entities.رویداد_مرحله_فرآیند;
                case "فرم_ورود_اطلاعات":
                    return CoreDefine.Entities.فرم_ورود_اطلاعات;
                case "فرم_جستجو":
                    return CoreDefine.Entities.فرم_جستجو;
                case "گزارش":
                    return CoreDefine.Entities.گزارش;
                case "پارامتر_گزارش":
                    return CoreDefine.Entities.پارامتر_گزارش;
                case "تنظیمات_پرداخت":
                    return CoreDefine.Entities.تنظیمات_پرداخت;
                case "وب_سرویس":
                    return CoreDefine.Entities.وب_سرویس;
                case "پارامتر_وب_سرویس":
                    return CoreDefine.Entities.پارامتر_وب_سرویس;
                case "تنظیمات_عمومی":
                    return CoreDefine.Entities.تنظیمات_عمومی;
                case "تنظیمات_مدیر_سیستم":
                    return CoreDefine.Entities.تنظیمات_مدیر_سیستم;
                case "عبارت_ویژه":
                    return CoreDefine.Entities.عبارت_ویژه;
                case "ضمیمه_جدول":
                    return CoreDefine.Entities.ضمیمه_جدول;
                case "فیلد_نمایشی":
                    return CoreDefine.Entities.فیلد_نمایشی;
                case "داشبورد":
                    return CoreDefine.Entities.داشبورد;
                case "زیر_بخش_داشبورد":
                    return CoreDefine.Entities.زیر_بخش_داشبورد;
                case "فیلد_محاسباتی":
                    return CoreDefine.Entities.فیلد_محاسباتی;
                case "پایگاه_داده":
                    return CoreDefine.Entities.پایگاه_داده;
                case "تابع_جدولی":
                    return CoreDefine.Entities.تابع_جدولی;
                case "پارامتر_تابع":
                    return CoreDefine.Entities.پارامتر_تابع;
                case "منبع_گزارش":
                    return CoreDefine.Entities.منبع_گزارش;
                case "فایل_عمومی":
                    return CoreDefine.Entities.فایل_عمومی;
                case "رویداد_عمومی":
                    return CoreDefine.Entities.رویداد_عمومی;
                case "فیلد_جستجو":
                    return CoreDefine.Entities.فیلد_جستجو;
                case "BPMN_بخش_بندی":
                    return CoreDefine.Entities.BPMN_بخش_بندی;
                case "BPMN_مسیر":
                    return CoreDefine.Entities.BPMN_مسیر;
                case "ورودی_فرآیند":
                    return CoreDefine.Entities.ورودی_فرآیند;
                case "خروجی_فرآیند":
                    return CoreDefine.Entities.خروجی_فرآیند; 
                case "فرم_دکمه_جدید":
                    return CoreDefine.Entities.فرم_دکمه_جدید; 
                case "پوشه":
                    return CoreDefine.Entities.پوشه;  
                case "رنگ_سطر_جدول":
                    return CoreDefine.Entities.رنگ_سطر_جدول; 
                default:
                    return CoreDefine.Entities.خالی;
            }

        } 


        public static CoreDefine.Entities GetEntityFromEnglishName(string CoreEntity)
        {
            switch (CoreEntity)
            {
                case "InformationEntryForm": 
                        return CoreDefine.Entities.فرم_ورود_اطلاعات;  
                case "WebService": 
                        return CoreDefine.Entities.وب_سرویس; 
                case "SearchForm": 
                        return CoreDefine.Entities.فرم_جستجو; 
                case "SearchField": 
                        return CoreDefine.Entities.فیلد_جستجو; 
                case "PublicJob": 
                        return CoreDefine.Entities.رویداد_عمومی; 
                case "SpecialPhrase": 
                        return CoreDefine.Entities.عبارت_ویژه; 
                case "Process": 
                        return CoreDefine.Entities.فرآیند; 
                case "TableButton": 
                        return CoreDefine.Entities.دکمه_رویداد_جدول; 
                case "ProcessStep": 
                        return CoreDefine.Entities.مرحله_فرآیند; 
                case "ProcessReferral": 
                        return CoreDefine.Entities.ارجاع_مرحله_فرآیند; 
                case "ProcessStepEvent": 
                        return CoreDefine.Entities.رویداد_مرحله_فرآیند; 
                case "DisplayField": 
                        return CoreDefine.Entities.فیلد_نمایشی; 
                case "TableAttachment": 
                        return CoreDefine.Entities.ضمیمه_جدول; 
                case "TableEvent": 
                        return CoreDefine.Entities.رویداد_جدول; 
                case "DataSource": 
                        return CoreDefine.Entities.پایگاه_داده; 
                case "Dashboard": 
                        return CoreDefine.Entities.داشبورد; 
                case "SubDashboard": 
                        return CoreDefine.Entities.زیر_بخش_داشبورد; 
                case "DashboardIntegration": 
                        return CoreDefine.Entities.ادغام_داشبورد; 
                case "ReportParameter": 
                        return CoreDefine.Entities.پارامتر_گزارش; 
                case "Report": 
                        return CoreDefine.Entities.گزارش; 
                case "ComputationalField": 
                        return CoreDefine.Entities.فیلد_محاسباتی; 
                case "Field": 
                        return CoreDefine.Entities.فیلد; 
                case "ShowFieldEvent": 
                        return CoreDefine.Entities.رویداد_نمایش_فیلد; 
                case "Table": 
                        return CoreDefine.Entities.جدول; 
                case "TableFunction": 
                        return CoreDefine.Entities.تابع_جدولی; 
                case "ParameterTableFunction": 
                        return CoreDefine.Entities.پارامتر_تابع; 
                case "NewButtonForm": 
                        return CoreDefine.Entities.فرم_دکمه_جدید; 
                case "Folder": 
                        return CoreDefine.Entities.پوشه; 
                case "GridRowColor": 
                        return CoreDefine.Entities.رنگ_سطر_جدول; 
                default: 
                        return CoreDefine.Entities.خالی; 
            }
        }
        public static CoreDefine.FieldDisplayType GetFieldDisplayType(string _Inputtype)
        {
            switch(_Inputtype)
            {
                case "افقی":
                    return CoreDefine.FieldDisplayType.افقی;
            }
            return CoreDefine.FieldDisplayType.عمودی;
        }
        public static CoreDefine.InputTypes GetInputType(string _Inputtype)
        {
            switch (_Inputtype)
            {
                case "ShortText":
                    return CoreDefine.InputTypes.ShortText;
                case "LongText":
                    return CoreDefine.InputTypes.LongText;
                case "Number":
                    return CoreDefine.InputTypes.Number;
                case "TwoValues":
                    return CoreDefine.InputTypes.TwoValues;
                case "SingleSelectList":
                    return CoreDefine.InputTypes.SingleSelectList;
                case "RelatedTable":
                    return CoreDefine.InputTypes.RelatedTable;
                case "FillTextAutoComplete":
                    return CoreDefine.InputTypes.FillTextAutoComplete;
                case "RelatedTableTree":
                    return CoreDefine.InputTypes.RelatedTableTree;
                case "Image":
                    return CoreDefine.InputTypes.Image;
                case "Map":
                    return CoreDefine.InputTypes.Map;
                case "Password":
                    return CoreDefine.InputTypes.Password;
                case "Phone":
                    return CoreDefine.InputTypes.Phone;
                case "PersianDateTime":
                    return CoreDefine.InputTypes.PersianDateTime;
                case "MiladyDateTime":
                    return CoreDefine.InputTypes.MiladyDateTime;
                case "Clock":
                    return CoreDefine.InputTypes.Clock;
                case "Money":
                    return CoreDefine.InputTypes.Money;
                case "UserRole":
                    return CoreDefine.InputTypes.UserRole;
                case "Plaque":
                    return CoreDefine.InputTypes.Plaque;
                case "Icon":
                    return CoreDefine.InputTypes.Icon;
                case "ExternalField":
                    return CoreDefine.InputTypes.ExternalField;
                case "ComboBox":
                    return CoreDefine.InputTypes.ComboBox;
                case "NationalCode":
                    return CoreDefine.InputTypes.NationalCode;
                case "CoreRelatedTable":
                    return CoreDefine.InputTypes.CoreRelatedTable;
                case "NotificationType":
                    return CoreDefine.InputTypes.NotificationType;
                case "RecordType":
                    return CoreDefine.InputTypes.RecordType; 
                case "Editor":
                    return CoreDefine.InputTypes.Editor; 
                case "AttachmentUploadType":
                    return CoreDefine.InputTypes.AttachmentUploadType; 
                case "Sparkline":
                    return CoreDefine.InputTypes.Sparkline; 
                case "Color":
                    return CoreDefine.InputTypes.Color; 
                case "Query":
                    return CoreDefine.InputTypes.Query; 
                case "Attachment":
                    return CoreDefine.InputTypes.Attachment; 
                case "Rating":
                    return CoreDefine.InputTypes.Rating; 
                case "MultiSelectFromRelatedTable":
                    return CoreDefine.InputTypes.MultiSelectFromRelatedTable; 
                case "MultiSelectFromComboBox":
                    return CoreDefine.InputTypes.MultiSelectFromComboBox; 
                default:
                    return CoreDefine.InputTypes.ShortText;
            }
        }
         

        public static CoreDefine.AttachmentUploadType GetAttachmentUploadType(string UploadType)
        {
            switch (UploadType)
            {
                case "وبکم":
                    return CoreDefine.AttachmentUploadType.وبکم;
                case "اسکن":
                    return CoreDefine.AttachmentUploadType.اسکن; 
                default :
                    return CoreDefine.AttachmentUploadType.بارگذاری;
                    
            }
        }
        

        public static CoreDefine.TableButtonEventsType GetTableButtonEventsType(string ButtonEventsType)
        {
            switch (ButtonEventsType)
            {
                case "اجرای_رویداد":
                    return CoreDefine.TableButtonEventsType.اجرای_رویداد;
                case "اجرای_وب_سرویس":
                    return CoreDefine.TableButtonEventsType.اجرای_وب_سرویس; 
                case "ارسال_ایمیل":
                    return CoreDefine.TableButtonEventsType.ارسال_ایمیل; 
                case "انتقال_فایل":
                    return CoreDefine.TableButtonEventsType.انتقال_فایل; 
                case "باز_کردن_فرم":
                    return CoreDefine.TableButtonEventsType.باز_کردن_فرم; 
                case "باز_کردن_فرم_فقط_خواندنی":
                    return CoreDefine.TableButtonEventsType.باز_کردن_فرم_فقط_خواندنی; 
                case "باز_کردن_فرم_به_صورت_ویرایش":
                    return CoreDefine.TableButtonEventsType.باز_کردن_فرم_به_صورت_ویرایش; 
                case "نمایش_ضمیمه":
                    return CoreDefine.TableButtonEventsType.نمایش_ضمیمه; 
                case "تولید_کلید_عمومی_مالیاتی":
                    return CoreDefine.TableButtonEventsType.تولید_کلید_عمومی_مالیاتی ; 
                case "بروزرسانی_کالا_مالیات":
                    return CoreDefine.TableButtonEventsType.بروزرسانی_کالا_مالیات ; 
                case "ارسال_صورتحساب_به_سامانه_مودیان":
                    return CoreDefine.TableButtonEventsType.ارسال_صورتحساب_به_سامانه_مودیان; 
                case "نمایش_گزارش":
                    return CoreDefine.TableButtonEventsType.نمایش_گزارش; 
                case "پرینت_گزارش":
                    return CoreDefine.TableButtonEventsType.پرینت_گزارش; 
                default :
                    return CoreDefine.TableButtonEventsType.خالی;
                    
            }
        }

        public static CoreDefine.DataSourceType GetDataSourceType(string DataSourceType)
        {
            switch (DataSourceType)
            {
                case "EXCEL":
                    return CoreDefine.DataSourceType.EXCEL;
                case "ACCESS":
                    return CoreDefine.DataSourceType.ACCESS; 
                case "MySql":
                    return CoreDefine.DataSourceType.MySql; 
                default :
                    return CoreDefine.DataSourceType.SQLSERVER;
                    
            }
        }
        public static CoreDefine.ProcessStepRecordType GetStepRecordType(string StepRecordType)
        {
            switch (StepRecordType)
            {
                case "جدید":
                    return CoreDefine.ProcessStepRecordType.جدید;
                case "ویرایش":
                    return CoreDefine.ProcessStepRecordType.ویرایش; 
                default :
                    return CoreDefine.ProcessStepRecordType.خالی;
                    
            }
        }
        public static CoreDefine.ProcessStepActionType GetStepActionType(string StepActionType)
        {
            switch (StepActionType)
            {
                case "شروع":
                    return CoreDefine.ProcessStepActionType.شروع;
                case "عملیات":
                    return CoreDefine.ProcessStepActionType.عملیات;
                case "شرط":
                    return CoreDefine.ProcessStepActionType.شرط;
                case "پایان":
                    return CoreDefine.ProcessStepActionType.پایان;
                default :
                    return CoreDefine.ProcessStepActionType.خالی;
                    
            }
        }
        public static CoreDefine.NotificationType GetNotificationType(string StepActionType)
        {
            switch (StepActionType)
            { 
                case "پیامک":
                    return CoreDefine.NotificationType.پیامک;
                case "ایمیل":
                    return CoreDefine.NotificationType.ایمیل; 
                default :
                    return CoreDefine.NotificationType.سیستمی;
                    
            }
        }
        
        public static CoreDefine.SmsServerType GetSmsServerType(string SmsServerType)
        {
            switch (SmsServerType)
            { 
                case "_0098sms":
                    return CoreDefine.SmsServerType._0098sms;
                case "FrazSms":
                    return CoreDefine.SmsServerType.FrazSms; 
                default :
                    return CoreDefine.SmsServerType.None;
                    
            }
        }
        
        public static string GetDefaultValue(string Value)
        {
            switch (Value)
            { 
                case "@شناسه_پرسنل":
                    return Referral.UserAccount.PersonnelID.ToString();
                case "@شناسه_سمت_مخاطب":
                    return Referral.UserAccount.PersonneLaudienceSideID.ToString();
                case "@شناسه_کاربر":
                    return Referral.UserAccount.UsersID.ToString();
                case "@شناسه_نقش_کاربر":
                    return Referral.UserAccount.RoleTypeID.ToString();
                case "@شناسه_واحد_سازمانی_پرسنل":
                    return Referral.UserAccount.PersonnelUnitID.ToString();
                case "@شناسه_سمت_سازمانی_پرسنل":
                    return Referral.UserAccount.PersonnelPostID.ToString();
                case "@نقش_کاربر":
                    return Referral.UserAccount.RoleName.ToString();
                case "@نام_کاربر":
                    return Referral.UserAccount.UserName.ToString();
                case "@نام_و_نام_خانوادگی_کاربر":
                    return Referral.UserAccount.FullName.ToString();
                case "@تاریخ_امروز":
                    return CDateTime.GetNowshamsiDate();
                case "@ساعت_سیستم":
                    return CDateTime.GetNowTime();
                case "@تاریخ_پارسال":
                    {
                        string NowDate = CDateTime.GetNowshamsiDate();
                        return (int.Parse(NowDate.Substring(0, 4)) - 1).ToString() + NowDate.Substring(4, 6);
                    }
                case "@تاریخ_هفته_قبل_سیستم":
                    return CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -7);
                case "@تاریخ_ماه_قبل_سیستم":
                    return CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -30);
                case "@تاریخ_سال_قبل_سیستم":
                    return CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -365);
                case "@تاریخ_شروع_ماه_جاری":
                    return CDateTime.StartDateCurrentMonth();
                case "@تاریخ_شروع_سال_جاری":
                    return CDateTime.StartDateCurrentYear();
                default :
                    return Value;                    
            }
        }
        public static string CheckQuery(string Query)
        {
            string BeforQuery = "DECLARE @شناسه_پرسنل AS BIGINT=" + Referral.UserAccount.PersonnelID.ToString()+NewLine;
             BeforQuery += "DECLARE @شناسه_سمت_مخاطب AS BIGINT=" + Referral.UserAccount.PersonneLaudienceSideID.ToString()+NewLine;
             BeforQuery += "DECLARE @شناسه_کاربر AS BIGINT=" + Referral.UserAccount.UsersID.ToString()+NewLine;
             BeforQuery += "DECLARE @شناسه_نقش_کاربر AS BIGINT=" + Referral.UserAccount.RoleTypeID.ToString()+NewLine;
             BeforQuery += "DECLARE @شناسه_واحد_سازمانی_پرسنل AS BIGINT=" + Referral.UserAccount.PersonnelUnitID.ToString()+NewLine;  
             BeforQuery += "DECLARE @شناسه_سمت_سازمانی_پرسنل AS BIGINT=" + Referral.UserAccount.PersonnelPostID.ToString()+NewLine;   
             BeforQuery += "DECLARE @نقش_کاربری AS NVARCHAR(400)=N'" + Referral.UserAccount.RoleName.ToString()+"'"+NewLine;   
             BeforQuery += "DECLARE @نام_و_نام_خانوادگی_کاربر AS NVARCHAR(400)=N'" + Referral.UserAccount.FullName.ToString() + "'" + NewLine;   
             BeforQuery += "DECLARE @نام_کاربر AS NVARCHAR(400)=N'" + Referral.UserAccount.UserName.ToString() + "'" + NewLine;   
             BeforQuery += "DECLARE @تاریخ_امروز AS NVARCHAR(400)=N'" + CDateTime.GetNowshamsiDate() + "'" + NewLine;   
             BeforQuery += "DECLARE @ساعت_سیستم AS NVARCHAR(400)=N'" + CDateTime.GetNowTime() + "'" + NewLine;   
             BeforQuery += "DECLARE @تاریخ_هفته_قبل_سیستم AS NVARCHAR(400)=N'" + CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -7) + "'" + NewLine;   
             BeforQuery += "DECLARE @تاریخ_ماه_قبل_سیستم AS NVARCHAR(400)=N'" + CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -30) + "'" + NewLine;   
             BeforQuery += "DECLARE @تاریخ_سال_قبل_سیستم AS NVARCHAR(400)=N'" + CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -365) + "'" + NewLine;    
             BeforQuery += "DECLARE @تاریخ_شروع_ماه_جاری AS NVARCHAR(400)=N'" + CDateTime.StartDateCurrentMonth() + "'" + NewLine;    
             BeforQuery += "DECLARE @تاریخ_شروع_سال_جاری AS NVARCHAR(400)=N'" + CDateTime.StartDateCurrentYear() + "'" + NewLine;    
            return BeforQuery + NewLine + NewLine + ConvertToSQLQuery(Query);
        }

        public static string ConvertToSQLQuery(string Query)
        {
            if (string.IsNullOrEmpty(Query))
            {
                return Query;
            }
            else
            {
                char[] Chars = Query.Replace("{", "N'").Replace("}", "'").Replace("&gt;", "<").Replace("،", ",").ToCharArray();
                 
                string FinalString=string.Empty;
                string Temp=string.Empty;
                char[] Chars2 = {',', '(',')',' ','\n','\t','=','+','-','*','/','\\'};
                for (int i = 0; i < Chars.Length; i++)
                {
                    if (Array.IndexOf(Chars2, Chars[i]) > -1)
                    { 
                        FinalString += ConvertPesrsainToSQL(Temp) + Chars[i];
                        Temp=string.Empty;
                    }
                    //else if(Chars[i]== '،')
                    //{ 
                    //    FinalString += ConvertPesrsainToSQL(Temp) +",";
                    //    Temp = string.Empty;
                    //}
                    else if(Chars[i] =='{')
                    {  
                        int FindIndex = Array.IndexOf(Chars, '}', i);
                        FinalString += Temp  +"N'"+ new string(Chars, i+1, FindIndex - i-1)+"'";

                        if (i == FindIndex - 1 || (FindIndex - i - 1) ==1)
                            i = FindIndex;
                        else
                            i = FindIndex+1;
                        Temp = string.Empty;
                    }
                    else if(Chars[i]=='\'')
                    {
              
                        int FindIndex = Array.IndexOf(Chars, '\'', i+1);
                        FinalString += Temp + new string(Chars, i, FindIndex - i+1);
                        i = FindIndex;
                        Temp = string.Empty;
                        //Chars[i];
                    }
                    else
                    {
                       //int FindIndex= Array.IndexOf(Chars, ' ', i); 
                       // if(FindIndex>-1)
                       // { 
                       //     if (Array.IndexOf(Chars, '\n', i, FindIndex - i) > -1)
                       //         FindIndex = Array.IndexOf(Chars, '\n', i, FindIndex - i);
                       //     if (Array.IndexOf(Chars, '\'', i, FindIndex - i) > -1)
                       //     {
                       //         FindIndex = Array.IndexOf(Chars, '\'', i, FindIndex - i);
                       //         if(i== FindIndex)
                       //         {
                       //             FindIndex = Array.IndexOf(Chars, ' ', i);
                       //             FindIndex = Array.IndexOf(Chars, '\'', i+1, FindIndex - i-1);
                       //             if(FindIndex==-1)
                       //                 FindIndex = Array.IndexOf(Chars, ' ', i);
                       //             else 
                       //                 FindIndex++; 
                       //         }
                       //     }
                       //     if (Array.IndexOf(Chars, '(', i, FindIndex - i) > -1)
                       //         FindIndex = Array.IndexOf(Chars, '(', i, FindIndex - i);
                       //     if (Array.IndexOf(Chars, '{', i, FindIndex - i) > -1)
                       //         FindIndex = Array.IndexOf(Chars, '{', i, FindIndex - i);
                       //     if (Array.IndexOf(Chars, '،', i, FindIndex - i) > -1)
                       //         FindIndex = Array.IndexOf(Chars, '،', i, FindIndex - i); 
                       //     Temp = new string(Chars, i, FindIndex - i);
                             
                       //     i = FindIndex-1; 
                       // }
                       // else
                        Temp += Chars[i];
                    }
                }

                return FinalString+ ConvertPesrsainToSQL(Temp);
            }
        }

        public static string ConvertPesrsainToSQL(string Temp)
        {

            switch (Temp.ToUpper())
            {
                case "در.غیر.اینصورت": { Temp = "ELSE"; break; }
                case "ارتباط.مستقیم.با": { Temp = "INNER JOIN"; break; }
                case "بعنوان": { Temp = "AS"; break; }
                case "ارتباط.غیر.مستقیم.با": { Temp = "LEFT JOIN"; break; }
                case "با.توجه.به": { Temp = "ON"; break; }
                case "اگر.تهی.هست": { Temp = "ISNULL"; break; }
                case "اگر.تهی.است": { Temp = "ISNULL"; break; }
                case "دستور.ویرایش": { Temp = "UPDATE"; break; }
                case "دستور.حذف": { Temp = "DELETE"; break; }
                case "دستور.گروهبندی": { Temp = "GROUP BY"; break; }
                case "دستور.حلقه": { Temp = "WHILE"; break; }
                case "دستور.XML": { Temp = "FOR XML PATH('')"; break; }
                case "مقداردهی": { Temp = "SET"; break; }
                case "در": { Temp = "Into"; break; }
                case "،": { Temp = ","; break; }
                case "نمایش": { Temp = "SELECT"; break; }
                case "حداکثر": { Temp = "MAX"; break; }
                case "تبدیل": { Temp = "CAST"; break; }
                case "دستور.مبدل": { Temp = "CONVERT"; break; }
                case "تعریف": { Temp = "DECLARE"; break; }
                case "نوع.صحیح": { Temp = "BIGINT"; break; }
                case "نوع.رشته": { Temp = "NVARCHAR(400)"; break; }
                case "رشته.انتخاب": { Temp = "SUBSTRING"; break; }
                case "رشته.طول": { Temp = "LEN"; break; }
                case "از.جدول": { Temp = "FROM"; break; }
                case "انتخاب": { Temp = "CASE"; break; }
                case "مجموع": { Temp = "SUM"; break; }
                case "زمانیکه": { Temp = "WHEN"; break; }
                case "هست": { Temp = "IS"; break; }
                case "مخالف": { Temp = "NOT"; break; }
                case "تهی": { Temp = "NULL"; break; }
                case "پس": { Temp = "THEN"; break; }
                case "ترتیب": { Temp = "ORDER BY"; break; }
                case "صعودی": { Temp = "ASC"; break; }
                case "نزولی": { Temp = "DESC"; break; }
                case "در.صورتی.که": { Temp = "WHERE"; break; }
                case "پایان.دستورات": { Temp = "END"; break; }
                case "اگر": { Temp = "IF"; break; }
                case "تعداد.کل": { Temp = "COUNT(1)"; break; }
                case "&gt;": { Temp = "<"; break; }
                case "شروع.دستورات": { Temp = "BEGIN"; break; }
                case "{": { Temp = "N'"; break; }
                case "}": { Temp = "'"; break; }
                case "و": { Temp = "AND"; break; }
                case "یا": { Temp = "OR"; break; }
                case "سطرها.به.تعداد": { Temp = "Top"; break; }
                case "دستور.درج": { Temp = "INSERT"; break; }
                case "مقادیر": { Temp = "VALUES"; break; }
                case "دستور.خروجی": { Temp = "RETURN"; break; }
                case "دستور.اختلاف_زمان": { Temp = "DATEDIFF"; break; }
                case "دستور.افزودن_زمان": { Temp = "DATEADD"; break; }
                case "دستور.سال": { Temp = "YEAR"; break; }
                case "دستور.هفته": { Temp = "WEEK"; break; }
                case "دستور.روز": { Temp = "DAY"; break; }
                case "دستور.ساعت": { Temp = "HOUR"; break; }
                case "دستور.دقیقه": { Temp = "MINUTE"; break; }
                case "دستور.ثانیه": { Temp = "SECOND"; break; }
                case "جدول.ضمیمه.مقدار": { Temp = "Value"; break; }
                case "جدول.ضمیمه.نام": { Temp = "FullName"; break; }
                case "جدول.ضمیمه.پسوند": { Temp = "Extension"; break; }
                case "جدول.ضمیمه.اندازه": { Temp = "Size"; break; }
                case "جدول.ضمیمه.رکورد_داخلی": { Temp = "InnerID"; break; }
                case "جدول.ضمیمه.رکورد_بیرونی": { Temp = "RecordID"; break; }
                case "جدول.ضمیمه": { Temp = Referral.DBAttachment.ConnectionData.DataBase + ".dbo.CoreObjectAttachment"; break; }
                default:
                    {

                        //Temp = Temp.Replace("در.غیر.اینصورت", "ELSE");
                        //Temp = Temp.Replace("ارتباط.مستقیم.با", "INNER JOIN");
                        //Temp = Temp.Replace("ارتباط.غیر.مستقیم.با", "LEFT JOIN");
                        //Temp = Temp.Replace("با.توجه.به", "ON");
                        //Temp = Temp.Replace("اگر.تهی.هست", " ISNULL ");
                        //Temp = Temp.Replace("اگر.تهی.است", "ISNULL");
                        //Temp = Temp.Replace("دستور.ویرایش", "UPDATE");
                        //Temp = Temp.Replace("دستور.حذف", "DELETE");
                        //Temp = Temp.Replace("دستور.گروهبندی", "GROUP BY");
                        //Temp = Temp.Replace("دستور.حلقه", "WHILE");
                        //Temp = Temp.Replace("دستور.XML", "FOR XML PATH('')");
                        //Temp = Temp.Replace("مقداردهی", "SET");
                        //Temp = Temp.Replace("،", ",");
                        //Temp = Temp.Replace(" نمایش", " SELECT");
                        //Temp = Temp.Replace(" نمایش ", " SELECT ");
                        //Temp = Temp.Replace("نمایش ", "SELECT ");
                        //Temp = Temp.Replace("حداکثر", "MAX");
                        //Temp = Temp.Replace("تبدیل", "CAST");
                        //Temp = Temp.Replace("CAST_", "تبدیل_");
                        //Temp = Temp.Replace("دستور.مبدل", "CONVERT");
                        //Temp = Temp.Replace("بعنوان", "AS");
                        //Temp = Temp.Replace("تعریف ", "DECLARE ");
                        //Temp = Temp.Replace("نوع.صحیح", "BIGINT");
                        //Temp = Temp.Replace("نوع.رشته", "NVARCHAR(400)");
                        //Temp = Temp.Replace("رشته.انتخاب", "SUBSTRING");
                        //Temp = Temp.Replace("رشته.طول", "LEN");
                        //Temp = Temp.Replace("از.جدول", "FROM");
                        //Temp = Temp.Replace(" انتخاب ", " CASE ");
                        //Temp = Temp.Replace("انتخاب", "CASE");
                        //Temp = Temp.Replace("مجموع(", "SUM(");
                        //Temp = Temp.Replace("زمانیکه", "WHEN");
                        //Temp = Temp.Replace(" هست ", " IS ");
                        //Temp = Temp.Replace("مخالف", "NOT");
                        //Temp = Temp.Replace(" تهی ", " NULL ");
                        //Temp = Temp.Replace(" پس ", " THEN ");
                        //Temp = Temp.Replace("ترتیب ", "ORDER BY ");
                        //Temp = Temp.Replace("صعودی", "ASC");
                        //Temp = Temp.Replace("نزولی", "DESC");
                        //Temp = Temp.Replace("در.صورتی.که", "WHERE");
                        //Temp = Temp.Replace("پایان.دستورات", "END");
                        //Temp = Temp.Replace(" اگر ", " IF ");
                        //Temp = Temp.Replace("اگر(", "IF(");
                        //Temp = Temp.Replace("اگر (", "IF (");
                        //Temp = Temp.Replace("تعداد.کل", "COUNT(1)");
                        //Temp = Temp.Replace("&gt;", "<");
                        //Temp = Temp.Replace("شروع.دستورات", "BEGIN");
                        //Temp = Temp.Replace("{", "N'");
                        //Temp = Temp.Replace("}", "'");
                        //Temp = Temp.Replace(" و ", " AND ");
                        //Temp = Temp.Replace("\nو ", "\nAND ");
                        //Temp = Temp.Replace(" یا ", " OR ");
                        //Temp = Temp.Replace("\nیا ", "\nOR ");
                        //Temp = Temp.Replace(" سطرها.به.تعداد ", " Top ");
                        //Temp = Temp.Replace("دستور.درج ", "INSERT ");
                        //Temp = Temp.Replace(" مقادیر ", "VALUES ");
                        //Temp = Temp.Replace(" تهی", " NULL");
                        //Temp = Temp.Replace("دستور.خروجی", "RETURN");
                        //Temp = Temp.Replace("دستور.اختلاف_زمان", "DATEDIFF");
                        //Temp = Temp.Replace("دستور.افزودن_زمان", "DATEADD");
                        //Temp = Temp.Replace("دستور.سال", "YEAR");
                        //Temp = Temp.Replace("دستور.هفته", "WEEK");
                        //Temp = Temp.Replace("دستور.روز", "DAY");
                        //Temp = Temp.Replace("دستور.ساعت", "HOUR");
                        //Temp = Temp.Replace("دستور.دقیقه", "MINUTE");
                        //Temp = Temp.Replace("دستور.ثانیه", "SECOND");
                        //Temp = Temp.Replace("جدول.ضمیمه.مقدار", "Value");
                        //Temp = Temp.Replace("جدول.ضمیمه.نام", "FullName");
                        //Temp = Temp.Replace("جدول.ضمیمه.پسوند", "Extension");
                        //Temp = Temp.Replace("جدول.ضمیمه.اندازه", "Size");
                        //Temp = Temp.Replace("جدول.ضمیمه.رکورد_داخلی", "InnerID");
                        //Temp = Temp.Replace("جدول.ضمیمه.رکورد_بیرونی", "RecordID");
                        //Temp = Temp.Replace("جدول.ضمیمه", Referral.DBAttachment.ConnectionData.DataBase + ".dbo.CoreObjectAttachment");
                        break;
                    }

            }
            return Temp;
        }
        public static List<string> GetAllUserList()
        {
            List<string> AllUserList = new List<string>();
            if ( HttpContext.Current.Session["ReferralAllUserList" ]==null)
            {
                AllUserList = new List<string>() { "همه [0]", "ثبت کننده [-1]" };
                AllUserList.AddRange(Referral.DBData.SelectColumn("select نام_و_نام_خانوادگی +N' ['+cast(شناسه as nvarchar(255))+N']' from کاربر").Select(i => i.ToString()).ToList());
                HttpContext.Current.Session["ReferralAllUserList"]=AllUserList;
            }
            else
                AllUserList = (List<string>)HttpContext.Current.Session["ReferralAllUserList"]; 

            return AllUserList;
        }
        public static List<string> GetAllPostList()
        {
            List<string> AllPostList = new List<string>();

            if ( HttpContext.Current.Session["ReferralAllPostList"] ==null)
            {
                AllPostList = new List<string>() { "همه [0]", "سمت مافوق [-1]", "سمت ثبت کننده [-2]" };
                AllPostList.AddRange(Referral.DBData.SelectColumn("select عنوان +N' ['+cast(شناسه as nvarchar(255))+N']' from سمت_سازمانی").Select(i => i.ToString()).ToList());
                HttpContext.Current.Session["ReferralAllPostList"] = AllPostList;
            }
            else
                AllPostList = (List<string>)HttpContext.Current.Session["ReferralAllPostList"]; 

            return AllPostList;
        }

        public static string CheckAccessQuery(string Query)
        {
            string BeforQuery = "PARAMETERS  @شناسه_پرسنل  Number=" + Referral.UserAccount.PersonnelID.ToString()+NewLine;
             BeforQuery += ", @شناسه_سمت_مخاطب Number=" + Referral.UserAccount.PersonneLaudienceSideID.ToString()+NewLine;
             BeforQuery += ", @شناسه_کاربر Number=" + Referral.UserAccount.UsersID.ToString()+NewLine;
             BeforQuery += ", @شناسه_نقش_کاربر  Number=" + Referral.UserAccount.RoleTypeID.ToString()+NewLine;
             BeforQuery += ", @شناسه_واحد_سازمانی_پرسنل  Number=" + Referral.UserAccount.PersonnelUnitID.ToString()+NewLine;  
             BeforQuery += ", @شناسه_سمت_سازمانی_پرسنل  Number=" + Referral.UserAccount.PersonnelPostID.ToString()+NewLine;   
             BeforQuery += ", @نقش_کاربر Text = '" + Referral.UserAccount.RoleName.ToString()+"'"+NewLine;   
             BeforQuery += ", @نام_کاربر Text = '" + Referral.UserAccount.UserName.ToString() + "'" + NewLine;   
             BeforQuery += ", @تاریخ_امروز Text = '" + CDateTime.GetNowshamsiDate() + "';" + NewLine;   
            return BeforQuery+ Query;
        }
        
        public static string CheckExcelQuery(string Query)
        {
            Query = Query.Replace("@شناسه_پرسنل", Referral.UserAccount.PersonnelID.ToString());
            Query = Query.Replace("@شناسه_سمت_مخاطب", Referral.UserAccount.PersonneLaudienceSideID.ToString());
            Query = Query.Replace("@شناسه_کاربر", Referral.UserAccount.UsersID.ToString());
            Query = Query.Replace("@شناسه_نقش_کاربر", Referral.UserAccount.RoleTypeID.ToString());   
            Query = Query.Replace("@شناسه_واحد_سازمانی_پرسنل", Referral.UserAccount.PersonnelUnitID.ToString());   
            Query = Query.Replace("@شناسه_سمت_سازمانی_پرسنل", Referral.UserAccount.PersonnelPostID.ToString());      
            Query = Query.Replace("@نقش_کاربر", "N'"+Referral.UserAccount.RoleName+"'");           
            Query = Query.Replace("@نام_کاربر", "N'"+Referral.UserAccount.UserName + "'");           
            Query = Query.Replace("@تاریخ_امروز", "N'"+ CDateTime.GetNowshamsiDate() + "'");   
            return Query;
        }

        public static List<string> IranSansFontNames()
        {
            List<string> FontList = new List<string>();
            FontList.Add("IRANSansWeb(FaNum)");
            FontList.Add("IRANSansWeb(FaNum)_Black");
            FontList.Add("IRANSansWeb(FaNum)_Bold");
            FontList.Add("IRANSansWeb(FaNum)_Light");
            FontList.Add("IRANSansWeb(FaNum)_Medium");
            FontList.Add("IRANSansWeb(FaNum)_UltraLight"); 
            FontList.Add("IRANSansWeb");
            FontList.Add("IRANSansWeb_Black");
            FontList.Add("IRANSansWeb_Bold");
            FontList.Add("IRANSansWeb_Light");
            FontList.Add("IRANSansWeb_Medium");
            FontList.Add("IRANSansWeb_UltraLight"); 
            return FontList;
        }
        
        public static List<string> BFontNames()
        {
            List<string> FontList = new List<string>();
            FontList.Add("B Badr");
            //FontList.Add("B Baran");
            //FontList.Add("B Bardiya");
            //FontList.Add("B Compset");
            //FontList.Add("B Davat");
            //FontList.Add("B Elham");
            //FontList.Add("B Esfehan");
            //FontList.Add("B Fantezy");
            //FontList.Add("B Farnaz");
            //FontList.Add("B Ferdosi");
            //FontList.Add("B Hamid");
            //FontList.Add("B Helal");
            //FontList.Add("B Homa");
            //FontList.Add("B Jadid");
            //FontList.Add("B Jalal");
            FontList.Add("B Koodak");
            //FontList.Add("B Kourosh");
            //FontList.Add("B Lotus");
            //FontList.Add("B Mahsa");
            //FontList.Add("B Mehr");
            //FontList.Add("B Mitra");
            //FontList.Add("B Morvarid");
            //FontList.Add("B Narm");
            //FontList.Add("B Nasim");
            FontList.Add("B Nazanin");
            //FontList.Add("B Roya");
            //FontList.Add("B Setareh");
            //FontList.Add("B Shiraz");
            //FontList.Add("B Sina");
            //FontList.Add("B Tabassom");
            //FontList.Add("B Tehran");
            FontList.Add("B Titr");
            FontList.Add("B Titr TG E");
            FontList.Add("B Traffic");
            //FontList.Add("B Vahid");
            //FontList.Add("B Yagut");
            //FontList.Add("B Yas");
            FontList.Add("B Yekan");
            FontList.Add("B Zar");
            FontList.Add("B Ziba");
            return FontList;
        }

        public static List<SelectListItem> FieldTypeList()
        {
            List<SelectListItem> FieldTypeItems = new List<SelectListItem>() {
                                                        new SelectListItem() {Text = "هیچکدام", Value = ""},
                                                        new SelectListItem() {Text = "نوشته کوتاه", Value = CoreDefine.InputTypes.ShortText.ToString()},
                                                        new SelectListItem() {Text = "نوشته خودکار", Value = CoreDefine.InputTypes.FillTextAutoComplete.ToString()},
                                                        new SelectListItem() {Text = "نوشته طولانی", Value = CoreDefine.InputTypes.LongText.ToString()},
                                                        new SelectListItem() {Text = "ویرایشگر", Value = CoreDefine.InputTypes.Editor.ToString()},
                                                        new SelectListItem() {Text = "عدد", Value = CoreDefine.InputTypes.Number.ToString()},
                                                        new SelectListItem() {Text = "دو مقدار", Value = CoreDefine.InputTypes.TwoValues.ToString()},
                                                        new SelectListItem() {Text = "جدول مرتبط", Value = CoreDefine.InputTypes.RelatedTable.ToString()},
                                                        new SelectListItem() {Text = "چند انتخابی از جدول مرتبط", Value = CoreDefine.InputTypes.MultiSelectFromRelatedTable.ToString()},
                                                        new SelectListItem() {Text = "چند انتخابی از کمبوباکس", Value = CoreDefine.InputTypes.MultiSelectFromComboBox.ToString()},
                                                        //new SelectListItem() {Text = "جدول مرتبط درختی", Value = CoreDefine.InputTypes.RelatedTableTree.ToString()},
                                                        new SelectListItem() {Text = "انتخابی", Value = CoreDefine.InputTypes.ComboBox.ToString()},
                                                        new SelectListItem() {Text = "خط پیشرفت", Value = CoreDefine.InputTypes.Sparkline.ToString()},
                                                        new SelectListItem() {Text = "رتبه بندی", Value = CoreDefine.InputTypes.Rating.ToString()},
                                                        new SelectListItem() {Text = "عکس", Value = CoreDefine.InputTypes.Image.ToString()},
                                                        new SelectListItem() {Text = "نقشه", Value = CoreDefine.InputTypes.Map.ToString()},
                                                        new SelectListItem() {Text = "رمز", Value = CoreDefine.InputTypes.Password.ToString()},
                                                        new SelectListItem() {Text = "شماره موبایل", Value = CoreDefine.InputTypes.Phone.ToString()},
                                                        new SelectListItem() {Text = "تاریخ شمسی", Value = CoreDefine.InputTypes.PersianDateTime.ToString()},
                                                        new SelectListItem() {Text = "تاریخ میلادی", Value = CoreDefine.InputTypes.MiladyDateTime.ToString()},
                                                        new SelectListItem() {Text ="ساعت", Value = CoreDefine.InputTypes.Clock.ToString()},
                                                        new SelectListItem() {Text = "پول", Value = CoreDefine.InputTypes.Money.ToString()},
                                                        new SelectListItem() {Text = "نقش کاربر", Value = CoreDefine.InputTypes.UserRole.ToString()},
                                                        new SelectListItem() {Text = "پلاک خودرو", Value = CoreDefine.InputTypes.Plaque.ToString()},
                                                        new SelectListItem() {Text = "کد ملی", Value = CoreDefine.InputTypes.NationalCode.ToString()},
                                                        new SelectListItem() {Text = "هسته سیستم", Value = CoreDefine.InputTypes.CoreRelatedTable.ToString()}
                                                    };
            return FieldTypeItems;

        }

        
        public static List<SelectListItem> ChartTypeList()
        {
            List<SelectListItem> FieldTypeItems = new List<SelectListItem>() {
                                                              new SelectListItem() {Text = "هیچکدام", Value = ""},
                                                              new SelectListItem() {Text = "میله ای", Value = CoreDefine.ChartTypes.میله_ای.ToString()}, 
                                                              new SelectListItem() {Text = "ستونی", Value = CoreDefine.ChartTypes.ستونی.ToString()}, 
                                                              new SelectListItem() {Text = "خطی", Value = CoreDefine.ChartTypes.خطی.ToString()}, 
                                                              new SelectListItem() {Text = "دایره ای", Value = CoreDefine.ChartTypes.دایره_ای.ToString()}, 
                                                              new SelectListItem() {Text = "قیفی", Value = CoreDefine.ChartTypes.قیفی.ToString()}, 
                                                              new SelectListItem() {Text = "جدول", Value = CoreDefine.ChartTypes.جدول.ToString()}, 
                                                              new SelectListItem() {Text = "منطقه ای", Value = CoreDefine.ChartTypes.منطقه.ToString()}, 
                                                              new SelectListItem() {Text = "قوس سنج", Value = CoreDefine.ChartTypes.قوس_سنج.ToString()}, 
                                                              new SelectListItem() {Text = "تکی", Value = CoreDefine.ChartTypes.تکی.ToString()}, 
                                                          };
            return FieldTypeItems;

        }

        public static string GetExcelFormat(string FileName)
        {
            return FileName.EndsWith(".xlsx") ? "" : FileName.EndsWith(".xls") ? "" : ".xls";
        }
        public static string GetAccessFormat(string FileName)
        {
            return FileName.EndsWith(".mdb") ? "" : FileName.EndsWith(".accdb") ? "" : ".accdb";
        }
        public static CoreDefine.ChartTypes GetChartTypes(string ChartType)
        {
            switch (ChartType)
            { 
                case "ستونی":
                    return CoreDefine.ChartTypes.ستونی;
                case "خطی":
                    return CoreDefine.ChartTypes.خطی;
                case "دایره_ای":
                    return CoreDefine.ChartTypes.دایره_ای;
                case "قیفی":
                    return CoreDefine.ChartTypes.قیفی;
                case "جدول":
                    return CoreDefine.ChartTypes.جدول;
                case "منطقه":
                    return CoreDefine.ChartTypes.منطقه;
                case "قوس_سنج":
                    return CoreDefine.ChartTypes.قوس_سنج;
                case "تکی":
                    return CoreDefine.ChartTypes.تکی;
                default:
                    return CoreDefine.ChartTypes.میله_ای;
            }
        }
        public static CoreDefine.ChartCalculationType GetChartCalculationType(string ChartType)
        {
            switch (ChartType)
            {
                case "مجموع":
                    return CoreDefine.ChartCalculationType.مجموع;
                case "تعداد":
                    return CoreDefine.ChartCalculationType.تعداد; 
                default:
                    return CoreDefine.ChartCalculationType.هیچکدام;
            }
        }
        public static CoreDefine.ChartGroupDate GetChartGroupDate(string ChartType)
        {
            switch (ChartType)
            {
                case "سال":
                    return CoreDefine.ChartGroupDate.سال;
                case "ماه":
                    return CoreDefine.ChartGroupDate.ماه; 
                case "روز":
                    return CoreDefine.ChartGroupDate.روز; 
                default:
                    return CoreDefine.ChartGroupDate.هیچکدام;
            }
        }

        public static string CheckValidNationalCode(string nationalCode)
        {
            if (String.IsNullOrEmpty(nationalCode))
                return"لطفا کد ملی را صحیح وارد نمایید";

            if (nationalCode.Length != 10)
                return"طول کد ملی باید ده کاراکتر باشد";
             
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                return"کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید";
             
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return "لطفا کد ملی را صحیح وارد نمایید";

             
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)))?"":"کد ملی وارد شده صحیح نمی باشد";
        }

        public static void GetNextProcessStep(long ProcessID,long ProcessStepID, ref long NextProcessStepID, ref long InformationEntryFormID,ref string InformationEntryFormTitle, long TableID = 0, long RecordID = 0,string SourceRef = "")
        {
            bool IsFindNextActive = false;
            bool IsFinishSearch = false;
            string ActivityName = string.Empty;
            CoreObject StepCore= new CoreObject();
            CoreObject TableCore = new CoreObject();
            string[] ColumnNames = new string[0]; 
            object[] _Values = new object[0];    

            if (ProcessStepID>0)
            {
                TableCore = CoreObject.Find(TableID);
                StepCore = CoreObject.Find(ProcessStepID);
                ProcessStep processStep = new ProcessStep(StepCore);
                List<CoreObject> OutingList = CoreObject.FindChilds(processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند);
                if(processStep.ActionType== CoreDefine.ProcessStepActionType.شرط)
                {
                    bool IsFindSequenceFlow = false;
                    CheckGetWeyProcess(ProcessID, ProcessStepID, TableID, processStep, OutingList, TableCore, RecordID, ref ColumnNames, ref _Values, ref ActivityName,ref IsFindSequenceFlow);
                }
                else
                foreach(CoreObject Outing in OutingList)
                {
                    BpmnSequenceFlow bpmnSequenceFlow = new BpmnSequenceFlow(Outing);
                    ActivityName = bpmnSequenceFlow.TargetRef;
                } 
            }

            while (!IsFindNextActive)
            {
                List<CoreObject> BpmnPoolCooreList = CoreObject.FindChilds(ProcessID, CoreDefine.Entities.BPMN_بخش_بندی);
                foreach (CoreObject BpmnPoolItem in BpmnPoolCooreList)
                {
                    if (!IsFinishSearch)
                    {
                        List<CoreObject> BpmnLenCoreList = CoreObject.FindChilds(BpmnPoolItem.CoreObjectID, CoreDefine.Entities.BPMN_مسیر);
                        foreach (CoreObject BpmnLenItem in BpmnLenCoreList)
                        {
                            if (!IsFinishSearch)
                            {
                                List<CoreObject> ProcessStepCoreList = CoreObject.FindChilds(BpmnLenItem.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند);
                                foreach (CoreObject ProcessStepItem in ProcessStepCoreList)
                                {
                                    ProcessStep processStep = new ProcessStep(ProcessStepItem);
                                    NextProcessStepID = processStep.CoreObjectID; 
                                    if (processStep.ActionType == CoreDefine.ProcessStepActionType.شروع && ProcessStepID==0)
                                    {
                                        List<CoreObject> OutingList = CoreObject.FindChilds(processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند);
                                        foreach (CoreObject Outing in OutingList)
                                        {
                                            BpmnSequenceFlow bpmnSequenceFlow = new BpmnSequenceFlow(Outing);
                                            ActivityName = bpmnSequenceFlow.TargetRef;
                                        }
                                    }
                                    else
                                    {
                                        if (processStep.FullName == ActivityName && ProcessStepID != NextProcessStepID)
                                        {
                                            IsFindNextActive = true;
                                            if (processStep.InformationEntryFormID > 0)
                                            {
                                                InformationEntryFormID = processStep.InformationEntryFormID;
                                                IsFinishSearch = true;

                                                if (ProcessStepItem.ParentID != StepCore.ParentID && ProcessStepID > 0)
                                                {
                                                    DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                                                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                                    string DefineVariablesQuery = DataBase.DefineVariablesQuery(TableCore.FullName, RecordID, ref ColumnNames, ref _Values);

                                                    BpmnLane BpmnLaneItem = new BpmnLane(CoreObject.Find(ProcessStepItem.ParentID));
                                                    TaskReferral taskReferral = new TaskReferral()
                                                    {
                                                        InformarmationFormID = processStep.InformationEntryFormID,
                                                        ProcessID = ProcessID,
                                                        ProcessStepID = ProcessStepID,
                                                        RecordID = RecordID,
                                                        //ReferralDeadlineResponse = Event.ReferralDeadlineResponse,
                                                        ReferralRecipientsQuery = BpmnLaneItem.Query,
                                                        ReferralRecipientsRole = BpmnLaneItem.OrganizationLevel,
                                                        ReferralRecipientsUser = BpmnLaneItem.Personnel,
                                                        ReferralTitle = CoreObject.Find(processStep.InformationEntryFormID).Title(),
                                                        //ReferralTitleQuery = Event.ReferralTitleQuery,
                                                        TableID = TableCore.CoreObjectID
                                                    };

                                                    taskReferral.SyncSendTask(DefineVariablesQuery, processStep.InformationEntryFormID.ToString(), RecordID.ToString(), TableCore.CoreObjectID.ToString(), ColumnNames, _Values);
                                                }
                                                break;
                                            }
                                            if(processStep.ActionType== CoreDefine.ProcessStepActionType.شرط)
                                            {
                                                IsFindNextActive = false;
                                                List<CoreObject> OutingList = CoreObject.FindChilds(processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند);
                                                bool IsFindSequenceFlow = false;
                                                CheckGetWeyProcess(ProcessID, ProcessStepID, TableID, processStep, OutingList, TableCore, RecordID, ref ColumnNames, ref _Values, ref ActivityName, ref IsFindSequenceFlow);
                                            }
                                            else if(processStep.ActionType==CoreDefine.ProcessStepActionType.SendTask)
                                            {
                                                IsFindNextActive = false;
                                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                                                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                                string DefineVariablesQuery = DataBase.DefineVariablesQuery(TableCore.FullName, RecordID, ref ColumnNames, ref _Values);
                                                List<CoreObject> OutingList = CoreObject.FindChilds(processStep.CoreObjectID, CoreDefine.Entities.خروجی_فرآیند);
                                                foreach (CoreObject Outing in OutingList)
                                                {
                                                    BpmnSequenceFlow bpmnSequenceFlow = new BpmnSequenceFlow(Outing);
                                                    ActivityName = bpmnSequenceFlow.TargetRef;
                                                }

                                                switch (processStep.NotificationType)
                                                {
                                                    case CoreDefine.NotificationType.سیستمی:
                                                        {
                                                            if (ProcessStepItem.ParentID != StepCore.ParentID && ProcessStepID > 0)
                                                            {

                                                                BpmnLane BpmnLaneItem = new BpmnLane(CoreObject.Find(ProcessStepItem.ParentID));
                                                                TaskReferral taskReferral = new TaskReferral()
                                                                {
                                                                    InformarmationFormID = processStep.InformationEntryFormID,
                                                                    ProcessID = ProcessID,
                                                                    ProcessStepID = ProcessStepID,
                                                                    RecordID = RecordID,
                                                                    //ReferralDeadlineResponse = Event.ReferralDeadlineResponse,
                                                                    ReferralRecipientsQuery = BpmnLaneItem.Query,
                                                                    ReferralRecipientsRole = BpmnLaneItem.OrganizationLevel,
                                                                    ReferralRecipientsUser = BpmnLaneItem.Personnel,
                                                                    ReferralTitle = CoreObject.Find(processStep.InformationEntryFormID).Title()+"\n"+processStep.Title+":\n"+processStep.BodyMessage,
                                                                    //ReferralTitleQuery = Event.ReferralTitleQuery,
                                                                    TableID = TableCore.CoreObjectID
                                                                };

                                                                taskReferral.SyncSendTask(DefineVariablesQuery, processStep.InformationEntryFormID.ToString(), RecordID.ToString(), TableCore.CoreObjectID.ToString(), ColumnNames, _Values);
                                                            }
                                                            break;
                                                        }
                                                    case CoreDefine.NotificationType.ایمیل:
                                                        {
                                                            MailCore mailCore = new MailCore()
                                                            {
                                                                EMail = processStep.EMail,
                                                                EMailUserName = processStep.EMailUserName,
                                                                EMailPassword = processStep.EMailPassWord,
                                                                EMailServer = processStep.EMailServer,
                                                                EMailPort = processStep.EMailPort,
                                                                EnableSsl = processStep.EnableSsl,
                                                                ReceivingUsers = processStep.ReceivingUsers,
                                                                ReceivingRole = processStep.ReceivingRole,
                                                                InsertingUser = processStep.InsertingUser,
                                                                ReceivingQuery = processStep.ReceivingQuery,
                                                                SendAttachmentFile = processStep.SendAttachmentFile,
                                                                UsePublickEmail = processStep.UsePublickEmail,
                                                                SendReport = processStep.SendReport,
                                                                Title = processStep.Title,
                                                                TitleQuery = processStep.TitleQuery,
                                                                BodyMessage = processStep.BodyMessage,
                                                                BodyMessageQuery = processStep.BodyMessageQuery
                                                            };
                                                            mailCore.SyncSendMail(DefineVariablesQuery, TableID.ToString(), RecordID.ToString(), TableCore.CoreObjectID.ToString(), ColumnNames, _Values);
                                                            break;
                                                        }
                                                    case CoreDefine.NotificationType.پیامک:
                                                        {
                                                            break;
                                                        }
                                                }
                                            }
                                            else if(processStep.ActionType == CoreDefine.ProcessStepActionType.پایان)
                                            { 
                                                IsFinishSearch = true;
                                                IsFindNextActive = true;
                                                break;
                                            }
                                        }

                                    }

                                }

                            }
                            else
                                break;

                        }
                    }
                    else
                        break;
                }
            }


            if (InformationEntryFormID > 0)
                InformationEntryFormTitle = UnSafeTitle(CoreObject.Find(InformationEntryFormID).FullName);
        }


        public static void CheckGetWeyProcess(long ProcessID, long ProcessStepID, long TableID,ProcessStep processStep, List<CoreObject> OutingList, CoreObject TableCore, long RecordID,ref string[] ColumnNames,ref object[] _Values,ref string ActivityName, ref bool IsFindSequenceFlow )
        {
            if (OutingList.Count > 0)
            {
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                string DefineVariablesQuery = DataBase.DefineVariablesQuery(TableCore.FullName, RecordID, ref ColumnNames, ref _Values);
                foreach (CoreObject Outing in OutingList)
                {
                    BpmnSequenceFlow bpmnSequenceFlow = new BpmnSequenceFlow(Outing);
                    ActivityName = bpmnSequenceFlow.TargetRef;
                    if (!string.IsNullOrEmpty(bpmnSequenceFlow.ConditionQuery))
                    {
                        string Query = "\nIf(" + bpmnSequenceFlow.ConditionQuery + ") begin select 1 end else begin select 0 end"; 
                        if (DataBase.SelectField(DefineVariablesQuery + Query).ToString() == "1")
                        {
                            Desktop.SaveProcessStep(ProcessID, processStep.CoreObjectID, TableID, RecordID, 0);
                            IsFindSequenceFlow = true;
                            break;
                        }
                    }
                    else
                    {
                        CoreObject TargetCore = CoreObject.Find(CoreObject.Find(CoreObject.Find(Outing.ParentID).ParentID).ParentID);
                        List<CoreObject> TargetList = CoreObject.FindChilds(bpmnSequenceFlow.TargetRef);
                        foreach (CoreObject TargetItem in TargetList)
                        {
                            CoreObject TargetItemCore = CoreObject.Find(CoreObject.Find(TargetItem.ParentID).ParentID);
                            if (TargetItemCore.CoreObjectID == TargetCore.CoreObjectID)
                            {
                                List<CoreObject> OutingTargetItemCoreList = CoreObject.FindChilds(TargetItem.CoreObjectID, CoreDefine.Entities.ورودی_فرآیند);
                                foreach (CoreObject OutingTargetItem in OutingTargetItemCoreList)
                                {
                                    BpmnSequenceFlow bpmnSequenceFlowTargetItem = new BpmnSequenceFlow(OutingTargetItem);
                                    if (bpmnSequenceFlowTargetItem.SourceRef == bpmnSequenceFlow.SourceRef)
                                    {
                                        if (!string.IsNullOrEmpty(bpmnSequenceFlowTargetItem.ConditionQuery))
                                        {
                                            string Query = "\nIf(" + bpmnSequenceFlowTargetItem.ConditionQuery + ") begin select 1 end else begin select 0 end";
                                            if (DataBase.SelectField(DefineVariablesQuery + Query).ToString() == "1")
                                            {
                                                Desktop.SaveProcessStep(ProcessID, processStep.CoreObjectID, TableID, RecordID, 0);
                                                IsFindSequenceFlow = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }

                    if (IsFindSequenceFlow)
                        break;
                }
            } 
        }
        public static DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dataTable = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dataTable.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow row = dataTable.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int index = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, index - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(index + 1).Replace("\"", "");
                        row[RowColumns] = RowDataString;
                    }
                    catch
                    {
                        continue;
                    }
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static MethodInfo CreateFunction()
        {
            //You can pass it through parameter
            string code = @"
            using System;

            namespace APM.Models.Tools
            {                
                public class Functions
                {                
                    public static string PrintStuff(string input)
                    {
                        return input;
                    }
                }
            }";

            //Compile on runtime:
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(new CompilerParameters(), code);

            //Compiled code threw error? Print it.
            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                {
                    Console.WriteLine(error);
                }
            }

            //Return MethodInfo for future use
            Type function = results.CompiledAssembly.GetType("APM.Models.Tools.Functions");
            return function.GetMethod("PrintStuff");
        }

    }
}