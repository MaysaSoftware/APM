using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Tools
{
    public class CoreDefine
    {
        public static string NewFolderTitle;
        public static string[] CoreObjectTableParams;

        public enum Entities
        {
            جدول = 0,
            فیلد = 1,
            رویداد_نمایش_فیلد = 2,
            فیلد_محاسباتی = 3,
            نما = 4,
            قالب = 5,
            رویداد_جدول = 6,
            گزارش = 7,
            صفحه_گزارش = 8,
            منبع_گزارش = 9,
            ستون_منبع_گزارش = 10,
            پارامتر_گزارش = 11,
            تابع_محاسباتی = 12,
            تابع_جدولی = 13,
            ستون_تابع = 14,
            پارامتر_تابع = 15,
            متغیر_تابع = 16,
            دستور_تابع = 17,
            فرم_ورود_اطلاعات = 18,
            پارامتر_فرم_جدولی = 19,
            رویداد_فرم_جدولی = 20,
            فرم_ورودی = 21,
            کاربر = 22,
            نقش_کاربر = 23,
            پیام_هشدار = 24,
            عبارت_ویژه = 25,
            فایل_عمومی = 26,
            تنظیمات_پرداخت = 27,
            تنظیمات_عمومی = 29,
            تنظیمات_مدیر_سیستم = 30,
            خالی = 31,
            فرآیند = 32,
            مرحله_فرآیند = 33,
            ارجاع_مرحله_فرآیند = 34,
            داشبورد = 35,
            رویداد_مرحله_فرآیند = 36,
            ضمیمه_جدول = 37,
            پارامتر_وب_سرویس = 38,
            وب_سرویس = 39,
            فیلد_نمایشی = 40,
            ارتباط_با_وبسایت = 41,
            زیر_بخش_داشبورد = 42,
            ادغام_داشبورد = 43,
            شاخص_فرآیند = 44,
            پایگاه_داده = 45,
            دکمه_رویداد_جدول = 46,
            فرم_جستجو = 47,
            رویداد_عمومی = 48,
            فیلد_جستجو = 49,
            BPMN_بخش_بندی = 50,
            BPMN_مسیر = 51,
            ورودی_فرآیند=52,
            خروجی_فرآیند=53,
            فرم_دکمه_جدید = 54,
            فیلد_گروهبندی= 55,
            پوشه=56,
            رنگ_سطر_جدول = 57
        }
        public enum TableEvents
        {
            شرط_اجرای_درج = 0,
            شرط_اجرای_حذف = 1,
            شرط_اجرای_ویرایش = 2,
            دستور_بعد_از_درج = 3,
            دستور_بعد_از_حذف = 4,
            دستور_بعد_از_ویرایش = 5,
            مرتب_سازی = 6,
            دستور_بعد_از_بازیافت = 7,
            هشدار_قبل_از_درج = 8,
            هشدار_قبل_از_ویرایش = 9,
            هشدار_قبل_از_حذف = 10,
        }

        public enum TableTypeEventExecution
        {
            خالی = 0,
            اجرای_کوئری = 1,
            صدور_گواهی_نامه_الکترونیکی_Src = 2,
            ارسال_ایمیل = 3,
            اجرای_وب_سرویس = 5,
            انتقال_فایل = 6,
            ارجاع = 7
        }

        public enum TableButtonEventsType
        {
            خالی = 0,
            اجرای_رویداد = 1,
            اجرای_وب_سرویس = 2,
            ارسال_ایمیل = 3,
            انتقال_فایل = 4,
            باز_کردن_فرم = 5,
            باز_کردن_فرم_فقط_خواندنی = 6,
            نمایش_ضمیمه = 7,
            باز_کردن_فرم_به_صورت_ویرایش = 8,
            تولید_کلید_عمومی_مالیاتی = 9,
            بروزرسانی_کالا_مالیات = 10,
            ارسال_صورتحساب_به_سامانه_مودیان = 11,
            باز_کردن_فرم_با_لینک = 12,
            باز_کردن_فرم_به_صورت_ویرایش_با_لینک = 13,
            نمایش_گزارش = 15 ,
            پرینت_گزارش = 16
        }

        public enum PublicJobType
        {
            خالی = 0,
            اجرای_رویداد = 1,
            ارسال_پیامک = 2,
            ارسال_ایمیل = 3,
            انتقال_فایل = 4,
            ارسال_گزارش = 5
        }
        public enum DataSourceType
        {
            SQLSERVER = 0,
            EXCEL = 1,
            ACCESS = 2,
            MySql = 3
        }
        public enum ProcessStepActionType
        {
            شروع = 0,
            عملیات = 1,
            شرط = 2,
            پایان = 3,
            خالی = 4,
            SendTask = 5,
            ReceiveTask = 6,
            ScriptTask = 7 ,
            UserTask = 8,
            ServiceTask =  9,
            BusinessRuleTask= 10,
            ManualTask= 11,
        }
         
        public enum ProcessStepRecordType
        {
            جدید = 0,
            ویرایش = 1,
            خالی = 2,
        }
        public enum ButtonType
        {
            جدید = 0,
            ویرایش = 1,
            خالی = 2,
        }
        public enum AttachmentUploadType
        {
            بارگذاری = 0,
            اسکن = 1,
            وبکم = 2,
        }
        public enum AttachmentUploadSize
        {
            کوچک = 0,
            متوسط = 1,
            بزرگ = 2,
        }
        public enum NotificationType
        {
            سیستمی = 0,
            ایمیل = 1,
            پیامک = 2
        }
        public enum SmsServerType
        {
            None = 0,
            _0098sms = 1,
            FrazSms = 2
        }

        public enum RegistryTable
        {
            Delete = 0,
            Insert = 1,
            Update = 2,
            Download = 3,
            View = 4,
            Login = 5
        }
        public enum PaymentGateWay
        {
            خالی = 0,
            زرین_پال = 1,
            شاپرک = 2
        }
        public enum FieldEvents
        {
            شرط_اجرای_ویرایش = 0,
            دستور_بعد_از_ویرایش = 1
        }


        public enum TableFormEvents
        {
            شرط_باز_شدن_فرم_زیر_مجموعه = 0
        }
        public enum CoreTypes
        {
            CoreEntity = 0,
            CoreTreeFolder = 1,
            CoreObject = 2
        }
        public enum CoreObjectColumns
        {
            CoreObjectID = 0,
            ParentID = 1,
            Entity = 2,
            Folder = 3,
            FullName = 4,
            OrderIndex = 5,
            IsDefault = 6,
            Value = 7
        }
        public enum Attributes
        {
            None = 0,
            ReadOnly = 1,
            Hidden = 2,
            AutoComplete = 3,
            Exclusive = 4
        }
        public enum InputTypes
        {
            ShortText = 0,
            LongText = 1,
            Number = 2,
            TwoValues = 3,
            SingleSelectList = 4,
            RelatedTable = 5,
            Image = 6,
            Map = 7,
            Password = 8,
            Phone = 9,
            PersianDateTime = 10,
            MiladyDateTime = 11,
            Clock = 12,
            Money = 13,
            UserRole = 14,
            Plaque = 15,
            Icon = 16,
            ExternalField = 17,
            ActionType = 18,
            ComboBox = 19,
            NationalCode = 20,
            CoreRelatedTable = 21,
            NotificationType = 22,
            RecordType = 23,
            Editor = 24,
            AttachmentUploadType = 25,
            Sparkline = 26,
            Color = 27,
            MultiSelect = 29,
            BPMSModel = 30,
            xml = 31,
            Binery = 32,
            Query = 33,
            Attachment = 34,
            FillTextAutoComplete = 35,
            RelatedTableTree = 36,
            Upload = 37,
            ViewerForm = 38,
            Rating = 39,
            CoreRelatedTableCheckbox=40,
            MultiSelectFromRelatedTable = 41,
            MultiSelectFromComboBox = 42,
        }
        public enum FieldDisplayType
        {
            عمودی = 0,
            افقی = 1
        }
        public enum DataTypes
        {
            صحیح = 0,
            اعشار = 1,
            دو_مقدار = 2,
            رشته = 3,
            متن_بلند = 4,
            جدول = 5
        }
        public enum ColumnProperties
        {
            AllowsNull = 0,
            ColumnId = 1,
            FullTextTypeColumn = 2,
            IsComputed = 3,
            IsCursorType = 4,
            IsDeterministic = 5,
            IsFulltextIndexed = 6,
            IsIdentity = 7,
            IsIdNotForRepl = 8,
            IsIndexable = 9,
            IsOutParam = 10,
            IsPrecise = 11,
            IsRowGuidCol = 12,
            IsSystemVerified = 13,
            IsXmlIndexable = 14,
            Precision = 15,
            Scale = 16,
            SystemDataAccess = 17,
            UserDataAccess = 18,
            UsesAnsiTrim = 19,
            IsSparse = 20,
            IsColumnSet = 21
        }
        public enum DefaultDates
        {
            None = 0,
            Today = 1,
            Yesterday = 2,
            ThisWeekStart = 3,
            ThisWeekEnd = 4,
            PreviousWeekStart = 5,
            PreviousWeekEnd = 6,
            ThisMonthStart = 7,
            ThisMonthEnd = 8,
            PreviousMonthStart = 9,
            PreviousMonthEnd = 10,
            ThisYearStart = 11,
            ThisYearEnd = 12,
            PreviousYearStart = 13,
            PreviousYearEnd = 14
        }
        public enum ChartTypes
        {
            میله_ای = 0,
            ستونی = 1,
            خطی = 2,
            دایره_ای = 3,
            قیفی = 4,
            جدول = 5,
            منطقه = 6,
            قوس_سنج = 7,
            تکی= 8,
        }
        public enum ChartGroupDate
        {
            هیچکدام = 0,
            سال = 1,
            ماه = 2,
            روز = 3,
        }
        public enum ChartCalculationType
        {
            هیچکدام = 0,
            تعداد = 1,
            مجموع = 2,
        }
        public enum ViewTypes
        {
            Primary = 0,
            Secondary = 1
        }
        public enum ExcelTypes
        {
            xlsx = 0,
            xls = 1
        }
        public enum AccessTypes
        {
            accdb = 0,
            mdb = 1
        }
        public enum OpenFormType
        {
            Grid=0,
            NewForm=1
        }
    }
}