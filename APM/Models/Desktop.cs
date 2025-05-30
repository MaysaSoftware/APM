using Newtonsoft.Json.Linq;
using APM.Models.Database;
using APM.Models.Tools;
using APM.Models.APMObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Syncfusion.XlsIO;
using Kendo.Mvc.UI;
using APM.Models.NetWork;
using System.Threading.Tasks;
using System.Reflection;

namespace APM.Models
{
    public static class Desktop
    {
        public static DataTank.DataInformationEntryForm DataInformationEntryForm = new DataTank.DataInformationEntryForm();
        public static DataTank.DataTableForm DataTableForm = new DataTank.DataTableForm();
        public static DataTank.DataFields DataFields = new DataTank.DataFields();
        public static DataTank.DataSearchForm DataSearchForm = new DataTank.DataSearchForm();
        public static DataTank.DataReport DataReport = new DataTank.DataReport();
        public static DataTank.TableDataFields TableDataFields = new DataTank.TableDataFields();
        public static DataTank.DataTable DataTable = new DataTank.DataTable();
        public static DataTank.DataShowColumn DataShowColumn = new DataTank.DataShowColumn();
        public static DataTank.CachedDataTable CachedTable = new DataTank.CachedDataTable();
        public static DataTank.CachedRegisteryID RegisterdTableID = new DataTank.CachedRegisteryID();
        public static DataTank.CachedMasterProcessID MasterProcessID = new DataTank.CachedMasterProcessID(); 
        public static DataTank.SessionEditorGrid SessionEditorGrid = new DataTank.SessionEditorGrid();
        public static DataTank.MasterDataKey_ShowWithOutPermissionConfig DataKey_ShowWithOutPermissionConfig = new DataTank.MasterDataKey_ShowWithOutPermissionConfig();

        public static string[] RegistryColumnName = { "RegistryID", "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "Value", "IP", "ServerName", "DatabaseName", "PCName", "Version", "Source", "BrowserType", "BrowserVersion" };
        private static string[] RegistryColumnTitle = { "شناسه", "تاریخ", "ساعت", "کاربر", "جدول", "شناسه رکورد", "مقدار", "IP", "سرور", "پایگاه داده", "نوع دستگاه", "نسخه برنامه", "نوع برنامه", "نوع مرورگر", "نسخه مرورگر" };

        public static string[] LoginRegistryColumnName = { "RegistryID", "LoginDate", "LoginTime", "LogOffDate", "LogOffTime", "UserAccountID", "UnsuccessfulLogin", "IsActive", "IP", "PCName", "Version", "Source", "BrowserType", "BrowserVersion" };
        private static string[] LoginRegistryColumnTitle = { "شناسه", "تاریخ ورود", "ساعت ورود", "تاریخ خروج", "ساعت خروج", "کاربر", "تعداد تلاش ورود", "وضعیت فعال", "IP", "نوع دستگاه", "نسخه برنامه", "نوع برنامه", "نوع مرورگر", "نسخه مرورگر" };
        public static string MasterRowID
        {
            get
            {
                return (string)HttpContext.Current.Session["MasterRowID"];
            }
            set
            {
                HttpContext.Current.Session["MasterRowID"] = value;
            }
        }

        public static string StepProcessInformationEntryForm
        {
            get
            {
                return "مراحل_فرآیند";
            }
        }

        public static void StartupSetting(string _MasterDataKey,bool ShowWithOutPermissionConfig=false)
        {
            CoreObject Form = CoreObject.Find(long.Parse(_MasterDataKey));
            CoreObject Table = new CoreObject();
            DataSourceInfo DataSourceInfo = new DataSourceInfo();
            DataKey_ShowWithOutPermissionConfig[_MasterDataKey] = ShowWithOutPermissionConfig;

            switch (Form.Entity)
            {
                case CoreDefine.Entities.جدول:
                    {
                        Table = Form;
                        DataTableForm[_MasterDataKey] = new Table(Form);
                        break;
                    }
                case CoreDefine.Entities.فرم_جستجو:
                    {
                        SearchForm EntryForm = new SearchForm(Form);
                        Table = CoreObject.Find(EntryForm.RelatedTable);

                        if (DataSearchForm[_MasterDataKey] == null || ShowWithOutPermissionConfig) 
                            DataSearchForm[_MasterDataKey] = EntryForm;
                        break;
                    }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        InformationEntryForm EntryForm = new InformationEntryForm(Form);
                        Table = CoreObject.Find(EntryForm.RelatedTable);

                        if (DataInformationEntryForm[_MasterDataKey] == null || ShowWithOutPermissionConfig)
                            DataInformationEntryForm[_MasterDataKey] = (EntryForm);

                        break;
                    }

            }

            DataSourceInfo = new DataSourceInfo(CoreObject.Find(Table.ParentID));
            Referral.DBRegistry.Insert("View_APMRegistry", new string[] { "UserAccountID", "RegistryDate", "RegistryTime", "TableName", "CoreObjectID", "IP", "ServerName", "DatabaseName", "PCName", "Version", "Source", "BrowserType", "BrowserVersion" }, new object[] { Referral.UserAccount.UsersID, CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Table.FullName, Table.CoreObjectID, Referral.UserAccount.IP, DataSourceInfo.ServerName, DataSourceInfo.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, "Web", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });
        }

        public static void StartupSettingTableDataFields(long _TableID)
        {
            HttpContext.Current.Session["TableDataFields" + _TableID] = DataConvertor.FillTableDataFields(_TableID);
        }

        public static void Setting(this Kendo.Mvc.UI.Fluent.DataSourceModelDescriptorFactory<dynamic> _Model, string _DataKey)
        {

            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            CoreObject EntryFormCore = CoreObject.Find(long.Parse(_DataKey));
            Field IDField = new Field();
            if (EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات || EntryFormCore.Entity == CoreDefine.Entities.جدول)
            {
                IDField = DataTable[_DataKey].IDField();
                _Model.Id(IDField.FieldName);
                _Model.Field("_ShowError", typeof(string)).Editable(false);
                _Model.Field("_ShowWarning", typeof(string)).Editable(false);

            }
            else if (EntryFormCore.Entity == CoreDefine.Entities.فرم_جستجو)
            {
                SearchForm searchForm = new SearchForm(EntryFormCore);
                Table table = new Table(CoreObject.Find(searchForm.RelatedTable));
                IDField = table.IDField();
                _Model.Id(IDField.FieldName);
                _Model.Field("_ShowError", typeof(string)).Editable(false);
            }
            else
            {
                _Model.Id("شناسه");
            }


            foreach (Field Item in DataFields[_DataKey])
            { 
                switch (Item.FieldType)
                {
                    case CoreDefine.InputTypes.Number:
                        _Model.Field(Item.FieldName, typeof(long)).Editable(Item.IsEditAble);
                        break;

                    case CoreDefine.InputTypes.Sparkline:
                        _Model.Field(Item.FieldName, typeof(long)).Editable(Item.IsEditAble);
                        break;

                    case CoreDefine.InputTypes.TwoValues:
                        _Model.Field(Item.FieldName, typeof(bool?)).Editable(Item.IsEditAble);
                        break;

                    case CoreDefine.InputTypes.SingleSelectList:
                        _Model.Field(Item.FieldName, typeof(string)).Editable(Item.IsEditAble).DefaultValue("");
                        break;

                    case CoreDefine.InputTypes.RelatedTable:
                        _Model.Field(Item.FieldName, typeof(long)).Editable(Item.IsEditAble);
                        break;

                    case CoreDefine.InputTypes.ComboBox:
                        _Model.Field(Item.FieldName, typeof(string)).Editable(Item.IsEditAble);
                        break;

                    case CoreDefine.InputTypes.Money:
                        _Model.Field(Item.FieldName, typeof(long)).Editable(Item.IsEditAble);
                        break;

                    case CoreDefine.InputTypes.Image:
                        _Model.Field(Item.FieldName, typeof(byte[])).Editable(Item.IsEditAble);
                        break;

                    default:
                        _Model.Field(Item.FieldName, typeof(string)).Editable(Item.IsEditAble);
                        break;
                }
            }

        }

        public static void Setting(this Kendo.Mvc.UI.Fluent.DataSourceGroupDescriptorFactory<dynamic> _Group, string _DataKey)
        {
            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            foreach (Field Item in DataFields[_DataKey])
            {
                if (Item.FieldType == CoreDefine.InputTypes.Number)
                { 
                    switch (Item.FieldType)
                    {
                        case CoreDefine.InputTypes.Money:
                        case CoreDefine.InputTypes.RelatedTable:
                        case CoreDefine.InputTypes.Number: 
                        case CoreDefine.InputTypes.Sparkline:
                            _Group.Add(Item.FieldName, typeof(long));
                            break;

                        case CoreDefine.InputTypes.TwoValues:
                            _Group.Add(Item.FieldName, typeof(bool)); 
                            break;
                             
                        case CoreDefine.InputTypes.Image:
                            _Group.Add(Item.FieldName, typeof(byte[])); 
                            break;

                        default:
                            _Group.Add(Item.FieldName, typeof(string)); 
                            break;
                    } 
                }
            }
        }
        public static void Setting(this Kendo.Mvc.UI.Fluent.DataSourceAggregateDescriptorFactory<dynamic> _Aggregate, string _DataKey)
        {
            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);


            foreach (Field Item in DataFields[_DataKey])
            {
                if (Item.FieldType == CoreDefine.InputTypes.Number )
                {
                    _Aggregate.Add(Item.FieldName, typeof(long)).Max().Min().Count();
                }
            }
        }

        public static void Setting(this Kendo.Mvc.UI.Fluent.GridColumnFactory<dynamic> _Columns, string _DataKey, bool IsDetailGrid = false, long ParentID = -1, long RecordID = -1, bool _SelectMode = false,long ProcessID=0,long ProcessStepID=0)
        {
            long ExternalField = 0;
            bool Aggregatesable = true;
            long TableID = long.Parse(_DataKey);
            CoreObject EntryFormCore = CoreObject.Find(long.Parse(_DataKey));
            GridEditMode GridEditMode = GridEditMode.PopUp; 
            List<DisplayField> StartEntryFormDisplayField = new List<DisplayField>();
            List<DisplayField> EndEntryFormDisplayField = new List<DisplayField>();
            _SelectMode = EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات ? DataInformationEntryForm[_DataKey].ShowSelectedColumn : _SelectMode;
            bool ShowLineNumber = EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات ? DataInformationEntryForm[_DataKey].ShowLineNumber : false; 
            bool DataKey_ShowWithOutPermissionConfig = Desktop.DataKey_ShowWithOutPermissionConfig[_DataKey];
            if (_SelectMode)
            {
                _Columns.Select().Hidden(!_SelectMode).Width(50).Column.Title = "انتخاب";
            }
            if (ShowLineNumber)
            {
                _Columns.Template(t => { }).ClientTemplate("<span class='RowNumber'></span>").Title("ردیف");
            }


            CoreObject TableCore = CoreObject.Find(TableID);
            string[] DefualtColumnShow = EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات ? DataInformationEntryForm[_DataKey].DefualtColumnShowInGrid.Replace("،", ",").Replace(" ", "").Replace("," + TableCore.FullName + ".", ",").Split(',') : new string[] { };

            _Columns
               .Bound("_ShowError")
               .Title("خطا")
               .ClientTemplate("<div id='error_#=data.id#' style='font-size: 12px;'></div>")
               .Hidden(true);

            _Columns
               .Bound("_ShowWarning")
               .Title("هشدار")
               .ClientTemplate("<div id='warning_#=data.id#' style='font-size: 12px;'></div>")
               .Hidden(true);


            if (EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                if (EntryFormCore.ParentID != 0 && RecordID == 0)
                    ExternalField = DataInformationEntryForm[_DataKey].ExternalField;
                TableID = DataInformationEntryForm[_DataKey].RelatedTable;
                GridEditMode = DataInformationEntryForm[_DataKey].GridEditMode;
                foreach (CoreObject coreObject in CoreObject.FindChilds(long.Parse(_DataKey), CoreDefine.Entities.فیلد_نمایشی))
                {
                    DisplayField displayField = new DisplayField(coreObject);
                    if (displayField.ShowInStart)
                        StartEntryFormDisplayField.Add(displayField);
                    else
                        EndEntryFormDisplayField.Add(displayField);
                }
                Aggregatesable = DataInformationEntryForm[_DataKey].Aggregatesable;

                if (DataInformationEntryForm[_DataKey].ShowAttachmentColumn)
                {
                    string RECORDID = DataInformationEntryForm[_DataKey].RelatedTable.ToString();
                    if (DataInformationEntryForm[_DataKey].AttachmentColumnName > 0)
                    {
                        Field field = new Field(CoreObject.Find(DataInformationEntryForm[_DataKey].AttachmentColumnName));
                        RECORDID = field.RelatedTable > 0 ? field.RelatedTable.ToString() : RECORDID;
                    }
                    string AttachmentColumnName = DataInformationEntryForm[_DataKey].AttachmentColumnName == 0 ? "id" : CoreObject.Find(DataInformationEntryForm[_DataKey].AttachmentColumnName).FullName;
                    string clientTemplate = "<div id='Attachment_" + RECORDID + "_#=data." + AttachmentColumnName + "#'  class='GridAttachment' onclick='ShowGridAttachmentColumn(this)'><span class='Columnhover'></span></div>";
                    _Columns
                        .Bound("ستون_ضمیمه")
                        .Title("<span class='k-icon k-i-attachment'></span>")
                        .ClientTemplate(clientTemplate)
                        .Width(10)
                        .Editable("function(){return false;}")
                        .Hidden(false);

                }

                DataKey_ShowWithOutPermissionConfig = CoreObject.FindChilds(EntryFormCore.CoreObjectID, CoreDefine.Entities.فیلد).Count > 0 ? true : false;
            }
            else if (EntryFormCore.Entity == CoreDefine.Entities.فرم_جستجو)
            {
                SearchForm searchForm = new SearchForm(EntryFormCore);
                TableID = searchForm.RelatedTable;
                Aggregatesable = false;
                _SelectMode = true;

            }

            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            Field IDField = new Field();
            if (EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات || EntryFormCore.Entity == CoreDefine.Entities.جدول)
                IDField = DataTable[_DataKey].IDField();

            List<CoreObject> DisplayFieldCore = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد_نمایشی);
            foreach (CoreObject coreObject in DisplayFieldCore)
            {
                DisplayField displayField = new DisplayField(coreObject);
                if (!string.IsNullOrEmpty(displayField.Template))
                {
                    _Columns
                       .Bound(coreObject.FullName)
                       .Title(Tools.Tools.UnSafeTitle(coreObject.FullName))
                       .ClientTemplate(displayField.Template)
                       .Hidden(false);
                }
            }


            if (StartEntryFormDisplayField.Count > 0)
            { 
                foreach (DisplayField displayField in StartEntryFormDisplayField)
                {
                    string ClientTemplate=String.Empty;
                    switch(displayField.TableButtonEventsType)
                    {
                        case CoreDefine.TableButtonEventsType.خالی: ClientTemplate = "#=" + displayField.Template + "#"; break;
                        case CoreDefine.TableButtonEventsType.نمایش_گزارش:
                            {
                                Report report = new Report(CoreObject.Find(displayField.ReportID)); 
                                ClientTemplate = "<div class='GridAttachment' onclick='ShowReport("+ displayField.CoreObjectID + ",\"گزارش\", "+ displayField.ParameterReportID + ", #=data.id#,false)'><span  class='Columnhover " + report.Icon + "' Title ='"+ Tools.Tools.UnSafeTitle(displayField.FullName) + "'></span></div>";
                                break;
                            }
                        case CoreDefine.TableButtonEventsType.پرینت_گزارش:
                            {
                                Report report = new Report(CoreObject.Find(displayField.ReportID)); 
                                ClientTemplate = "<div class='GridAttachment' onclick='ShowReport("+ displayField.CoreObjectID + ",null, "+ displayField.ParameterReportID + ", #=data.id#,true)'><span class='Columnhover " + report.Icon + "'  Title ='" + Tools.Tools.UnSafeTitle(displayField.FullName) + "'></span></div>";
                                break;
                            }
                        default:
                            {
                                string ButtonName = _DataKey + "_" + ParentID + "_" + ProcessID + "_" + ProcessStepID;
                                string ID ="";
                                switch (displayField.TableButtonEventsType)
                                {
                                    case CoreDefine.TableButtonEventsType.باز_کردن_فرم:
                                        { 
                                            ID += (IsDetailGrid ? "Detail" : "") + "1_";
                                            if(displayField.RelatedField>0)
                                            {
                                                ButtonName= _DataKey + "_#=data." + (CoreObject.Find(displayField.RelatedField).FullName) + "#_" + ProcessID + "_" + ProcessStepID;
                                            }

                                        }
                                        break;
                                    case CoreDefine.TableButtonEventsType.باز_کردن_فرم_فقط_خواندنی:
                                        ID += (IsDetailGrid ? "Detail" : "") + "2_";
                                        break;
                                    case CoreDefine.TableButtonEventsType.نمایش_ضمیمه:
                                        ID += (IsDetailGrid ? "Detail" : "") + "3_";
                                        break;
                                    case CoreDefine.TableButtonEventsType.باز_کردن_فرم_به_صورت_ویرایش:
                                        ID += (IsDetailGrid ? "Detail" : "") + "4_";
                                        break;
                                    case CoreDefine.TableButtonEventsType.تولید_کلید_عمومی_مالیاتی:
                                        ID += (IsDetailGrid ? "Detail" : "") + "5_";
                                        break;
                                    case CoreDefine.TableButtonEventsType.بروزرسانی_کالا_مالیات:
                                        ID += (IsDetailGrid ? "Detail" : "") + "6_";
                                        break;
                                    case CoreDefine.TableButtonEventsType.ارسال_صورتحساب_به_سامانه_مودیان:
                                        ID += (IsDetailGrid ? "Detail" : "") + "7_";
                                        break;
                                    case CoreDefine.TableButtonEventsType.باز_کردن_فرم_با_لینک:
                                        ID += (IsDetailGrid ? "Detail" : "") + "8_";
                                        break;
                                    default:
                                        ID += (IsDetailGrid ? "Detail" : "") + "0_";
                                        break;
                                }
                                ID=(IsDetailGrid ? "Detail" : "") + "TableButton_" + displayField.CoreObjectID + "_" + ButtonName + "_" + ID;
                                ID += displayField.RelatedForm + "_" + (displayField.ExecutionConditionQuery != "" ? "1" : "0") + "_#=data.id#" + "_" + (CoreObject.Find(displayField.RelatedField).FullName); 
                                ClientTemplate = "<div id='"+ID+"' class='GridAttachment' onclick='TableButtonClick(this)'><span class='Columnhover " + displayField.Icon + "'  Title ='" + Tools.Tools.UnSafeTitle(displayField.FullName) + "'></span></div>";
                                
                                break;
                            }
                    } 
                    _Columns
                    .Bound("")
                    .Title(displayField.ShowTitle?(String.IsNullOrEmpty(displayField.DisplayName)? Tools.Tools.UnSafeTitle(displayField.FullName): displayField.DisplayName):"")
                    .ClientTemplate(ClientTemplate)
                    .Width(10)
                    .Editable("function(){return false;}")
                    .Hidden(false);
                }
            }

            if ((GridEditMode == GridEditMode.PopUp))
            {
                foreach (Field Item in DataFields[_DataKey])
                { 
                    bool IsHide = !new PermissionField(Item.CoreObjectID, Referral.UserAccount.Permition).CanView;
                    IsHide = ProcessID > 0 ? false : IsHide;
                    IsHide = Item.IsTableAttachemnt ? !new PermissionBase(Item.CoreObjectID, Referral.UserAccount.Permition).IsAllow : IsHide;
                    if (!IsHide)
                        IsHide = (Item.CoreObjectID == ExternalField) || (!Item.IsDefaultView && !(EntryFormCore.FullName == "نقش کاربر"));
                    else if (Item.IsVirtual)
                        IsHide = false;


                    if (EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                        IsHide = Item.FieldName == IDField.FieldName ? !DataInformationEntryForm[_DataKey].IsShowID : IsHide;
                    else
                        IsHide = false;

                    IsHide = DataKey_ShowWithOutPermissionConfig && Item.IsDefaultView ? false : IsHide;
                    if (DefualtColumnShow.Length > 1)
                        IsHide = Array.IndexOf(DefualtColumnShow, Item.FieldName) != -1 ? false : true;

                    Dictionary<string, object> Attrs = new Dictionary<string, object>();
                    string ClassItem = Item.IsRequired ? " IsRequired " : "";
                    ClassItem += Item.IsLeftWrite ? " LTRColumn " : "";
                    ClassItem += " " + Item.GridTextColor + " ";
                    Attrs.Add("data_folder", Item.Folder);
                    Attrs.Add("data_name", Item.FieldName);
                    Attrs.Add("data-Item", _DataKey + "_");
                    Attrs.Add("class", ClassItem);
                    string ColumnTitle = Item.Title() + (Item.FieldComment != "" ? "</br><span class='ColumnFieldComment' >" + Item.FieldComment +"</span>": ""); 
                    switch (Item.FieldType)
                    {
                        case CoreDefine.InputTypes.MultiSelectFromComboBox:
                        case CoreDefine.InputTypes.MultiSelectFromRelatedTable:
                        case CoreDefine.InputTypes.RelatedTable:
                            _Columns
                                //.ForeignKey(Item.FieldName, (SelectList)HttpContext.Current.Session[Item.SystemName()])
                                .ForeignKey(Item.FieldName, DataConvertor.FillSelectList(Item ))
                                .Title(ColumnTitle)
                                .HtmlAttributes(Attrs)
                                //.ClientGroupHeaderTemplate(Item.FieldName+":  #=data.aggregates.تعداد.sum#")
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.ComboBox:
                            _Columns
                                //.ForeignKey(Item.FieldName, (SelectList)HttpContext.Current.Session[Item.SystemName()])
                                .ForeignKey(Item.FieldName, DataConvertor.FillSelectList(Item))
                                .Title(ColumnTitle)
                                .HtmlAttributes(Attrs)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.Number:
                            if (IDField.FieldName == Item.FieldName || !Aggregatesable)
                            {
                                _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Format("{0:n"+ Item.DigitsAfterDecimal.ToString() + "}") 
                                .Hidden(IsHide);
                            }
                            else
                            {
                                _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(new { @class = "LTRColumn", data_folder = Item.Folder })
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Format("{0:n" + Item.DigitsAfterDecimal.ToString() + "}")
                                .ClientFooterTemplate("<div style='width:100%;font-size:11px'><span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>کمترین: #= min #</span> <span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>بیشترین: #= max #</span></div>" +
                                                        "<div style='width:100%;font-size:11px'><span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>مجموع: <span id='SUM_" + Item.FieldName + "' style='font-size: 10px;'>0</span></span><span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>میانگین: <span id='AVG_" + Item.FieldName + "'  style='font-size: 10px;'>0</span></span></div>")
                                .Hidden(IsHide);
                            }

                            break;

                        case CoreDefine.InputTypes.PersianDateTime:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(new { @class = "LTRColumn", data_folder = Item.Folder })
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.MiladyDateTime:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(new { @class = "LTRColumn", data_folder = Item.Folder })
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.TwoValues:
                            _Columns
                                .Bound(Item.FieldName)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .HtmlAttributes(new { data_folder = Item.Folder })
                                  .ClientTemplate(
                                ("# if (" + Item.FieldName + " == true) { #" +
                                    "<span class='k-switch k-switch-md k-rounded-full k-switch-on' role='switch' tabindex='0' aria-checked='true' style=''><input id='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' name='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' type='checkbox' value='true' data-role='switch' style='display: none;' checked='checked'><span class='k-switch-track k-rounded-full'><span class='k-switch-label-on'>" + Item.ComboValues()[1] + "</span><span class='k-switch-label-off'>" + Item.ComboValues()[0] + "</span></span><span class='k-switch-thumb-wrap'><span class='k-switch-thumb k-rounded-full'></span></span></span>" +
                                "# } else {#" +
                                    "<span class='k-switch k-switch-md k-rounded-full k-switch-off' role='switch' tabindex='0' aria-checked='false' style=''><input id='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' name='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' type='checkbox' value='false' data-role='switch' style='display: none;'><span class='k-switch-track k-rounded-full'><span class='k-switch-label-on'>" + Item.ComboValues()[1] + "</span><span class='k-switch-label-off'>" + Item.ComboValues()[0] + "</span></span><span class='k-switch-thumb-wrap'><span class='k-switch-thumb k-rounded-full'></span></span></span>" +
                                    "#" +
                                "} #")
                                )
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.Sparkline:
                            _Columns
                                .Bound(Item.FieldName)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .HtmlAttributes(new { data_folder = Item.Folder })
                                .ClientTemplate("<div id='Chart_#=data.id#_#=" + Item.FieldName + "#_#=" + Item.MinValue + "#_#=" + Item.MaxValue + "#' class= 'GridSparklineChart' />")
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.Rating:
                            _Columns
                                .Bound(Item.FieldName)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .HtmlAttributes(new { data_folder = Item.Folder }) 
                                .ClientTemplate("<div id='Chart_#=data.id#_#=" + Item.FieldName + "#_#=" + Item.MinValue + "#_#=" + Item.MaxValue + "#' class= 'GridSparklineChart' />")
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.Image:
                            {
                                if (Item.IsVirtual)
                                {
                                    CoreObject FieldCore = CoreObject.Find(Item.CoreObjectID);
                                    string Query = String.Empty;
                                    if (FieldCore.Entity == CoreDefine.Entities.فیلد_محاسباتی)
                                        Query = Tools.Tools.ConvertToSQLQuery(Item.SpecialValue);
                                    else
                                    {
                                        ComputationalField computationalField = new ComputationalField();
                                        Query = Tools.Tools.ConvertToSQLQuery(computationalField.Query);
                                    }
                                    if (Query.ToUpper().IndexOf("COREOBJECTATTACHMENT") > -1 && Item.IsDefaultView)
                                    {
                                        Query = Query.Replace("=", " = ");
                                        Query = Query.Replace(")", " ) ");
                                        Query = Query.Replace("(", " ( ");

                                        while (Query.IndexOf("  ") > -1)
                                            Query = Query.Replace("  ", " ");

                                        string[] QueryArrUpper = Query.ToUpper().Split(' ');
                                        string[] QueryArr = Query.Split(' ');
                                        int FindIndex = Array.IndexOf(QueryArrUpper, "INNERID");
                                        string InnerId = QueryArr[FindIndex + 2].Replace("@", "");

                                        FindIndex = Array.IndexOf(QueryArrUpper, "RECORDID");
                                        string RECORDID = QueryArr[FindIndex + 2].Replace("@", "");

                                        FindIndex = Array.IndexOf(QueryArrUpper, "FULLNAME");
                                        string FullName = QueryArr[FindIndex + 2].Replace("N'", "").Replace("'", "");

                                        string clientTemplate = "<div id='Image_" + RECORDID + "_#=" + InnerId + "#_" + RECORDID + "_1_0_" + FullName + "'  class='GridImage'><img src=''  class='Grid_Image' /></div>";
                                        _Columns
                                            .Bound(Item.FieldName)
                                            .Title(ColumnTitle)
                                            .HtmlAttributes(new { data_folder = Item.Folder })
                                            .ClientTemplate(clientTemplate)
                                            .Hidden(IsHide);
                                    }
                                }
                                else
                                {
                                    if (Item.IsTableAttachemnt)
                                    {
                                        string clientTemplate = "<div id='Image_" + TableID + "_#=data.id#_" + TableID + "_1_" + Item.CoreObjectID + "_" + Item.FieldName + "'  class='GridImage'><img src=''  class='Grid_Image' /></div>";
                                        _Columns
                                            .Bound(Item.FieldName)
                                            .HtmlAttributes(new { data_folder = Item.Folder })
                                            .Title(Tools.Tools.UnSafeTitle(Item.FieldName))
                                            .ClientTemplate(clientTemplate)
                                            .Hidden(IsHide);
                                    }
                                    else
                                    {
                                        string clientTemplate = "<div id='Image_#=data.id#_" + Item.FieldName + "'  class='GridImage'><img src=''  class='Grid_Image' /></div>";
                                        _Columns
                                            .Bound(Item.FieldName)
                                            .HtmlAttributes(new { data_folder = Item.Folder })
                                            .Title(Tools.Tools.UnSafeTitle(Item.FieldName))
                                            .ClientTemplate(clientTemplate)
                                            .Hidden(IsHide);
                                    }
                                }

                                break;
                            }

                        case CoreDefine.InputTypes.Plaque:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(new { @class = "LTRColumn", data_folder = Item.Folder })
                                .Title(ColumnTitle)
                                .ClientTemplate(@" <span tabindex='-1' class='k-input-button k-icon-button k-button-md '>
                                               <span class='k-icon k-button-icon PlaqueIcon'></span>
                                                </span>#:" + Item.FieldName + "#</div></div>")
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.UserRole:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(true);
                            break;

                        case CoreDefine.InputTypes.Password:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .ClientTemplate("<span>******</span>")
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(true);
                            break;

                        default:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .Hidden(IsHide);
                            break;
                    }
                }

            }
            else
            {
                foreach (Field Item in DataFields[_DataKey])
                {
                    bool IsHide = !new PermissionField(Item.CoreObjectID, Referral.UserAccount.Permition).CanView;
                    if (!IsHide)
                        IsHide = (Item.CoreObjectID == ExternalField) || (!Item.IsDefaultView && !(EntryFormCore.FullName == "نقش کاربر"));
                    else if (Item.IsVirtual)
                        IsHide = false;

                    IsHide = DataKey_ShowWithOutPermissionConfig ? false : IsHide;
                    if (EntryFormCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                        IsHide = Item.FieldName == IDField.FieldName ? !DataInformationEntryForm[_DataKey].IsShowID : IsHide;
                    else
                        IsHide = false;

                    if (DefualtColumnShow[0] != "")
                        IsHide = Array.IndexOf(DefualtColumnShow, Item.FieldName) != -1 ? false : true;

                    Dictionary<string, object> Attrs = new Dictionary<string, object>();
                    string ClassItem = Item.IsRequired ? " IsRequired " : "";
                    ClassItem += Item.IsLeftWrite ? " LTRColumn " : "";
                    ClassItem += Item.ActiveOnKeyDown ? " ActiveOnKeyDown " : "";
                    ClassItem += " " + Item.GridTextColor + " ";
                      

                    Attrs.Add("data_folder", Item.Folder);
                    Attrs.Add("data_name", Item.FieldName);
                    Attrs.Add("data-Item", _DataKey + "_");
                    Attrs.Add("class", ClassItem);

                    string ColumnTitle = Item.Title() + (Item.FieldComment != "" ? "</br><span class='ColumnFieldComment' >" + Item.FieldComment + "</span>" : "");
                    List<SelectListItem> ComboItems = new List<SelectListItem>();
                    if (Item.FieldType == CoreDefine.InputTypes.ComboBox)
                    {
                        string[] ValueItem = Item.SpecialValue.Split('،');
                        foreach (string Value in ValueItem)
                        {
                            ComboItems.Add(new SelectListItem() { Text = Value, Value = Value });
                        }
                    }


                    string FieldName = Item.FieldName;// + "_" + _DataKey;
                    object Parameter = new
                    {
                        FieldName = FieldName,
                        InputType = Item.FieldType,
                        IsReadonly = !Item.IsEditAble,
                        IsRequired = Item.IsRequired,
                        NullValue = "نامشخص",
                        FalseValue = Item.ComboValues()[0],
                        TrueValue = Item.ComboValues()[1],
                        IsInCellEditMode = true,
                        FieldTitle = Item.Title(),
                        DigitsAfterDecimal = Item.DigitsAfterDecimal,
                        RelatedField = Item.RelatedField,
                        IsGridField = true,
                        IsLeftWrite = Item.IsLeftWrite,
                        RelatedTable = Item.RelatedTable,
                        FieldValue = Tools.Tools.GetDefaultValue(Item.DefaultValue),
                        _TableID = "0",
                        MaxValue = Item.MaxValue,
                        MinValue = Item.MinValue,
                        ComboItems = ComboItems,
                        IsExclusive = Item.IsExclusive,
                        ActiveOnKeyDown = Item.ActiveOnKeyDown,
                        CoreObjectID = Item.CoreObjectID,
                        SearchAutoCompleteCount = Item.SearchAutoCompleteCount,
                        FieldClass = "GridIncell_"+_DataKey+"_"+ParentID+"_"+RecordID
                    };

                    switch (Item.FieldType)
                    {
                        case CoreDefine.InputTypes.MultiSelectFromRelatedTable:
                        case CoreDefine.InputTypes.MultiSelectFromComboBox:
                        case CoreDefine.InputTypes.RelatedTable:
                            _Columns
                                //.ForeignKey(Item.FieldName, (SelectList)HttpContext.Current.Session[Item.SystemName()])
                                .ForeignKey(Item.FieldName, DataConvertor.FillSelectList(Item)) 
                                .Title(ColumnTitle)
                                .HtmlAttributes(Attrs)
                                .Hidden(IsHide)
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth)) 
                                .EditorTemplateName("TForeignKey")
                                .EditorViewData(Parameter);
                            break;

                        case CoreDefine.InputTypes.ComboBox:
                            _Columns
                                //.ForeignKey(Item.FieldName, (SelectList)HttpContext.Current.Session[Item.SystemName()])
                                .ForeignKey(Item.FieldName, DataConvertor.FillSelectList(Item))
                                .Title(ColumnTitle)
                                .HtmlAttributes(Attrs)
                                .Hidden(IsHide)
                                .EditorTemplateName("ComboBox")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            break;

                        case CoreDefine.InputTypes.Number:
                            if (IDField.FieldName == Item.FieldName || !Aggregatesable)
                            {
                                _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                //.ClientFooterTemplate("<div style='width:100%'><span style='width: 50%;display: inline-block;float: right;'>کمترین: #= min #</span> <span style='width: 50%;display: inline-block;float: right;'>بیشترین: #= max #</span></div>")
                                .Hidden(IsHide)
                                .EditorTemplateName("TableNumber")
                                .Format("{0:n" + Item.DigitsAfterDecimal.ToString() + "}")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            }
                            else
                            {
                                _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .ClientFooterTemplate("<div style='width:100%;font-size:11px'><span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>کمترین: #= min #</span> <span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>بیشترین: #= max #</span></div>" +
                                                      "<div style='width:100%;font-size:11px'><span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>مجموع: <span id='SUM_" + Item.FieldName + "'  style='font-size: 10px;' >0</span></span><span style='width: 50%;display: inline-block;float: right;font-size: 10px;'>میانگین: <span id='AVG_" + Item.FieldName + "'  style='font-size: 10px;' >0</span></span></div>")
                                .Hidden(IsHide)
                                .EditorTemplateName("TableNumber")
                                .Format("{0:n" + Item.DigitsAfterDecimal.ToString() + "}")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            }

                            break;

                        case CoreDefine.InputTypes.Rating:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Hidden(IsHide)
                                .EditorTemplateName("Rating")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            break;

                        case CoreDefine.InputTypes.PersianDateTime:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Hidden(IsHide)
                                .EditorTemplateName("TableShamsiDate")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            break;

                        case CoreDefine.InputTypes.MiladyDateTime:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Hidden(IsHide)
                                .EditorTemplateName("TableMiladiDate")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            break;

                        case CoreDefine.InputTypes.TwoValues:
                            _Columns
                                .Bound(Item.FieldName)
                                .Title(ColumnTitle)
                                .HtmlAttributes(new { data_folder = Item.Folder })
                                  .ClientTemplate(
                                ("# if (" + Item.FieldName + " == true) { #" +
                                    "<span class='k-switch k-switch-md k-rounded-full k-switch-on' role='switch' tabindex='0' aria-checked='true' style=''><input id='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' name='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' type='checkbox' value='true' data-role='switch' style='display: none;' checked='checked'><span class='k-switch-track k-rounded-full'><span class='k-switch-label-on'>" + Item.ComboValues()[1] + "</span><span class='k-switch-label-off'>" + Item.ComboValues()[0] + "</span></span><span class='k-switch-thumb-wrap'><span class='k-switch-thumb k-rounded-full'></span></span></span>" +
                                "# } else {#" +
                                    "<span class='k-switch k-switch-md k-rounded-full k-switch-off' role='switch' tabindex='0' aria-checked='false' style=''><input id='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' name='Switch_#=data.id#_#=" + Item.FieldName + "#_" + Item.FieldName + "' type='checkbox' value='false' data-role='switch' style='display: none;'><span class='k-switch-track k-rounded-full'><span class='k-switch-label-on'>" + Item.ComboValues()[1] + "</span><span class='k-switch-label-off'>" + Item.ComboValues()[0] + "</span></span><span class='k-switch-thumb-wrap'><span class='k-switch-thumb k-rounded-full'></span></span></span>" +
                                    "#" +
                                "} #")
                                )
                                .Hidden(IsHide)
                                .EditorTemplateName("Boolean")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            break;
                             
                        case CoreDefine.InputTypes.Sparkline:
                            _Columns
                                .Bound(Item.FieldName)
                                .Title(ColumnTitle)
                                .Width(GetColumnWidth(Item.ColumnWidth))
                                .HtmlAttributes(new { data_folder = Item.Folder })
                                .ClientTemplate("<div id='Chart_#=data.id#_#=" + Item.FieldName + "#_#=" + Item.MinValue + "#_#=" + Item.MaxValue + "#' class= 'GridSparklineChart' />")
                                .Hidden(IsHide);
                            break;
                        case CoreDefine.InputTypes.Image:
                            {
                                if (Item.IsVirtual)
                                {
                                    CoreObject FieldCore = CoreObject.Find(Item.CoreObjectID);
                                    string Query = String.Empty;

                                    if (FieldCore.Entity== CoreDefine.Entities.فیلد)
                                        Query= Tools.Tools.ConvertToSQLQuery(Item.SpecialValue);
                                    else
                                    {
                                        ComputationalField computationalField = new ComputationalField(FieldCore);
                                        Query = Tools.Tools.ConvertToSQLQuery(computationalField.Query); 
                                    }

                                    if (Query.ToUpper().IndexOf("COREOBJECTATTACHMENT") > -1)
                                    {
                                        Query = Query.Replace("=", " = ");
                                        Query = Query.Replace(")", " ) ");
                                        Query = Query.Replace("(", " ( ");

                                        while (Query.IndexOf("  ") > -1)
                                            Query = Query.Replace("  ", " ");

                                        string[] QueryArrUpper = Query.ToUpper().Split(' ');
                                        string[] QueryArr = Query.Split(' ');
                                        int FindIndex = Array.IndexOf(QueryArrUpper, "INNERID");
                                        string InnerId = QueryArr[FindIndex + 2].Replace("@", "");

                                        FindIndex = Array.IndexOf(QueryArrUpper, "RECORDID");
                                        string RECORDID = QueryArr[FindIndex + 2].Replace("@", "");

                                        FindIndex = Array.IndexOf(QueryArrUpper, "FULLNAME");
                                        string FullName = QueryArr[FindIndex + 2].Replace("N'", "").Replace("'", "");

                                        string clientTemplate = "<div id='Image_" + RECORDID + "_#=" + InnerId + "#_" + RECORDID + "_1_0_" + FullName + "'  class='GridImage'><img src=''  class='Grid_Image' /></div>";
                                        _Columns
                                            .Bound(Item.FieldName)
                                            .Title(ColumnTitle)
                                            .HtmlAttributes(new { data_folder = Item.Folder })
                                            .ClientTemplate(clientTemplate)
                                            .Hidden(IsHide);
                                    } 
                                }
                                else
                                {
                                    if (Item.IsTableAttachemnt && !IsDetailGrid)
                                    {
                                        TableAttachment attachment = new TableAttachment(CoreObject.Find(Item.CoreObjectID));
                                        string clientTemplate = "<div id='Image_" + TableID + "_#=data.id#_" + _DataKey + "_0_" + Item.CoreObjectID + "_" + Item.FieldName + "'  class='GridImage' >" +
                                            "<img src=''  class='Grid_Image' />" +
                                            "<button data-Item='" + _DataKey + "_" + ParentID + "_" + TableID + "_#=data.id#_" + Item.CoreObjectID + "_" + attachment.MaxFileSize + "_" + String.Join(",", attachment.AllowedExtensions) + "_" + Item.FieldName + "' class='k-button k-button-md T-G-k-button CameraAtt GridCellColumnCammeraButton' onclick='GridCameraAtt_Click(this)' id='GridCameraButton_" + TableID + "_#=data.id#_" + _DataKey + "_0_" + Item.CoreObjectID + "_" + Item.FieldName + "' ><span class='k-icon fa fa-camera-web'></span></button>" +
                                            "</div>";
                                        _Columns
                                            .Bound(Item.FieldName)
                                            .HtmlAttributes(new { data_folder = Item.Folder })
                                            .Title(ColumnTitle)
                                            .ClientTemplate(clientTemplate)
                                            .Hidden(IsHide);
                                    }
                                    else
                                    {
                                        //string clientTemplate = "<div id='Image_#=data.id#_" + Item.FieldName + "'  class='GridImage'><img src=''  class='Grid_Image' /></div>";
                                        //_Columns
                                        //    .Bound(Item.FieldName)
                                        //    .HtmlAttributes(new { data_folder = Item.Folder })
                                        //    .Title(Tools.Tools.UnSafeTitle(Item.FieldName))
                                        //    .ClientTemplate(clientTemplate)
                                        //    .Hidden(IsHide);

                                        _Columns
                                            .Bound(Item.FieldName)
                                            .HtmlAttributes(new { data_folder = Item.Folder })
                                            .Title(ColumnTitle)
                                            .Hidden(IsHide);
                                    }
                                }


                                break;
                            }

                        case CoreDefine.InputTypes.Plaque:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                //.ClientTemplate(@" <span tabindex='-1' class='k-input-button k-icon-button k-button-md '>
                                //               <span class='k-icon k-button-icon PlaqueIcon'></span>
                                //                //</span>#:" + Item.FieldName + "#</div></div>")
                                .Hidden(IsHide);
                            break;

                        case CoreDefine.InputTypes.Password:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Hidden(IsHide)
                                .EditorTemplateName("Password")
                                .EditorViewData(Parameter);
                            break;

                        case CoreDefine.InputTypes.UserRole:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Hidden(true);
                            break;


                        case CoreDefine.InputTypes.FillTextAutoComplete:
                            {
                                _Columns
                                 .Bound(Item.FieldName)
                                 .HtmlAttributes(Attrs)
                                 .Title(ColumnTitle)
                                 .Hidden(IsHide)
                                 .EditorTemplateName("FillTextAutoComplete")
                                 .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                 .EditorViewData(Parameter);
                                break;
                            }
                        default:
                            _Columns
                                .Bound(Item.FieldName)
                                .HtmlAttributes(Attrs)
                                .Title(ColumnTitle)
                                .Hidden(IsHide)
                                .EditorTemplateName("String")
                                .MinResizableWidth(GetColumnWidth(Item.ColumnWidth))
                                .EditorViewData(Parameter);
                            break;
                    }

                }

            }

            if (EndEntryFormDisplayField.Count > 0)
            { 
                foreach (DisplayField displayField in EndEntryFormDisplayField)
                {
                    _Columns
                    .Bound("")
                    .Title(Tools.Tools.UnSafeTitle(displayField.FullName))
                    .ClientTemplate("#=" + displayField.Template + "#")
                    .Editable("function(){return false;}")
                    .Hidden(false);
                }
            }
            if (!IsDetailGrid && GridEditMode != GridEditMode.PopUp)
            {
                PermissionInformationEntryForm Formpermission = new PermissionInformationEntryForm(long.Parse(_DataKey), Referral.UserAccount.Permition);
                if (GridEditMode == GridEditMode.InLine && Formpermission.CanUpdate)
                    _Columns.Command(command => command.Edit().Text("ویرایش")).Width(100);
                if (Formpermission.CanDelete)
                    _Columns.Command(command => command.Destroy().Text("حذف")).Width(100);
            }

            if (ProcessStepID == 0 && ProcessID > 0)
            {
                _Columns
                    .Bound("آخرین_مرحله_اجرا_شده_فرآیند") 
                    .Title("آخرین مرحله اجرا شده فرآیند")
                    .Hidden(true);

                _Columns
                    .Bound("عنوان_آخرین_مرحله_اجرا_شده_فرآیند") 
                    .Title("عنوان آخرین مرحله")
                    .Hidden(false);

                _Columns
                    .Bound("تاریخ_و_ساعت_آخرین_مرحله_اجرا_شده_فرآیند") 
                    .Title("تاریخ و ساعت آخرین مرحله")
                    .Hidden(false);

                _Columns
                    .Bound("مرحله_بعد_فرآیند") 
                    .Title("مرحله بعد فرآیند")
                    .Hidden(true);

                _Columns
                    .Bound("عنوان_مرحله_بعد_فرآیند") 
                    .Title("عنوان مرحله بعد فرآیند")
                    .Hidden(false);
            }
        }
        
        public static void ProcessStepSetting(this Kendo.Mvc.UI.Fluent.GridColumnFactory<dynamic> _Columns)
        {  
                _Columns
                    .Bound("شناسه") 
                    .Title("شناسه")
                    .Hidden(true);

                _Columns
                    .Bound("سطر") 
                    .Title("سطر")
                    .Hidden(true);

                _Columns
                    .Bound("اجرا_کننده") 
                    .Title("اجرا کننده")
                    .Hidden(false);


                _Columns
                    .Bound("تاریخ_ثبت")
                    .Title("تاریخ ثبت")
                    .Hidden(false);

                _Columns
                    .Bound("ساعت_ثبت")
                    .Title("ساعت ثبت")
                    .Hidden(false);

                _Columns
                    .Bound("مرحله_فرآیند") 
                    .Title("مرحله فرآیند")
                    .Hidden(true);

                _Columns
                    .Bound("عنوان_مرحله") 
                    .Title("عنوان مرحله")
                    .Hidden(false);

                _Columns
                    .Bound("مرحله_بعد_فرآیند") 
                    .Title("مرحله بعد فرآیند")
                    .Hidden(false);

                _Columns
                    .Bound("عنوان_مرحله_بعد_فرآیند") 
                    .Title("عنوان مرحله بعد فرآیند")
                    .Hidden(false); 

                _Columns
                    .Bound("نفرات_دریافت_کننده") 
                    .Title("نفرات دریافت کننده")
                    .Hidden(false); 

                _Columns
                    .Bound("سمت_دریافت_کننده") 
                    .Title("سمت دریافت کننده")
                    .Hidden(false); 

                _Columns
                    .Bound("تاریخ_مشاهده") 
                    .Title("تاریخ مشاهده")
                    .Hidden(false); 

                _Columns
                    .Bound("ساعت_مشاهده") 
                    .Title("ساعت مشاهده")
                    .Hidden(false); 
        }

        public static void SettingRegistry(this Kendo.Mvc.UI.Fluent.DataSourceModelDescriptorFactory<dynamic> _Model, CoreDefine.RegistryTable RegistryTable)
        {
            _Model.Id(RegistryColumnName[0]);
            if (RegistryTable == CoreDefine.RegistryTable.Login)
            {
                for (int i = 1; i < LoginRegistryColumnName.Length; i++)
                    _Model.Field(LoginRegistryColumnName[i], typeof(string)).Editable(false).DefaultValue("");

            }
            else
                for (int i = 1; i < RegistryColumnName.Length; i++)
                {
                    if (RegistryTable == CoreDefine.RegistryTable.Update && RegistryColumnName[i] == "Value")
                    {
                        _Model.Field("FieldName", typeof(string)).Editable(false).DefaultValue("");
                        _Model.Field("PreviousValue", typeof(string)).Editable(false).DefaultValue("");
                        _Model.Field("NewValue", typeof(string)).Editable(false).DefaultValue("");
                    }
                    else if (RegistryTable == CoreDefine.RegistryTable.View && RegistryColumnName[i] == "Value")
                    {
                    }
                    else
                        _Model.Field(RegistryColumnName[i], typeof(string)).Editable(false).DefaultValue("");
                }
        }
        public static void SettingRegistry(this Kendo.Mvc.UI.Fluent.GridColumnFactory<dynamic> _Columns, CoreDefine.RegistryTable RegistryTable)
        {


            _Columns
                .Bound(RegistryColumnName[0])
                .Title(RegistryColumnTitle[0])
                .Hidden(true);
            if (RegistryTable == CoreDefine.RegistryTable.Login)
            {
                for (int i = 1; i < LoginRegistryColumnName.Length; i++)
                    _Columns
                    .Bound(LoginRegistryColumnName[i])
                    .Title(LoginRegistryColumnTitle[i])
                    .Hidden(LoginRegistryColumnName[i] == "Value" ? true : false);

            }
            else
                for (int i = 1; i < RegistryColumnName.Length; i++)
                {
                    if (RegistryTable == CoreDefine.RegistryTable.Update && RegistryColumnName[i] == "Value")
                    {
                        _Columns
                            .Bound("FieldName")
                            .Title("فیلد")
                            .Hidden(false);

                        _Columns
                            .Bound("PreviousValue")
                            .Title("مقدار قبلی")
                            .Hidden(false);

                        _Columns
                            .Bound("NewValue")
                            .Title("مقدار جدید")
                            .Hidden(false);
                    }
                    else if (RegistryTable == CoreDefine.RegistryTable.View && RegistryColumnName[i] == "Value")
                    {
                    }
                    else
                        _Columns
                        .Bound(RegistryColumnName[i])
                        .Title(RegistryColumnTitle[i])
                        .Hidden(RegistryColumnName[i] == "Value" ? true : false);
                }
        }
        public static DataTable SelectTable(string _DataKey, string _Where, int _ParentID, int _ShowRowCount = 0, long ProcessID = 0, long ProcessStepID = 0)
        {
            CoreObject Form = CoreObject.Find(long.Parse(_DataKey));
            DataTable Output = new DataTable();

            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            switch (Form.Entity)
            {
                case CoreDefine.Entities.فرم_جستجو:
                    {
                        SearchForm searchForm = new SearchForm(Form);
                        CoreObject TableCore = CoreObject.Find(searchForm.RelatedTable);
                        CoreObject ParentTableCore = CoreObject.Find(Form.ParentID);
                        List<CoreObject> ParentFieldList = new List<CoreObject>();
                        string[] FormInputName = (string[])HttpContext.Current.Session["SearchFormButtonClick_FormInputName"];
                        string[] FormInputValue=(string[])HttpContext.Current.Session["SearchFormButtonClick_FormInputValue"];
                        string Query = string.Empty;
                        CoreObject ParentFormCore = ParentTableCore;
                        if (ParentTableCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                        {
                            if (ParentFormCore.ParentID > 0)
                                ParentFormCore = CoreObject.Find(ParentTableCore.ParentID);
                            ParentFieldList = CoreObject.FindChilds(ParentFormCore.CoreObjectID, CoreDefine.Entities.فیلد);
                            ParentTableCore = CoreObject.Find(new InformationEntryForm(ParentFormCore).RelatedTable);
                        }

                        if (ParentFieldList.Count == 0)
                            ParentFieldList = CoreObject.FindChilds(ParentTableCore.CoreObjectID, CoreDefine.Entities.فیلد);
                        if (ParentFieldList.Count > 0)
                        {
                            if (FormInputName != null)
                                for (int Index = 0; Index < FormInputName.Length; Index++)
                                    FormInputName[Index] = FormInputName[Index].Replace("_" + ParentFormCore.CoreObjectID.ToString(), "");

                            foreach (CoreObject Item in ParentFieldList)
                            {
                                Field field = new Field(Item);
                                int FindIndex = Array.IndexOf(FormInputName, field.FieldName);
                                if (FindIndex > -1)
                                {
                                    Query += "\nDeclare @" + ParentTableCore.FullName + "_" + Item.FullName + " AS ";
                                    switch (field.FieldType)
                                    {
                                        case CoreDefine.InputTypes.Number:
                                        case CoreDefine.InputTypes.RelatedTable:
                                            {
                                                Query += " Bigint = " + (FormInputValue[FindIndex]==""?"0": FormInputValue[FindIndex]);
                                                break;
                                            }
                                        case CoreDefine.InputTypes.TwoValues:
                                            {
                                                Query += " Bit = " + (FormInputValue[FindIndex] == "true" ? 1 : 0);
                                                break;
                                            }
                                        default:
                                            {
                                                Query += " Nvarchar(400) = N'" + FormInputValue[FindIndex] + "'";
                                                break;
                                            }
                                    }
                                }
                            }
                        }

                        HttpContext.Current.Session["SearchFormButtonClick_Query"] = Query;
                         Query +="\n\n"+ searchForm.Query;

                        Query = Tools.Tools.CheckQuery(Query);
                        if (_Where != "")
                        {
                            _Where = Tools.Tools.ConvertToSQLQuery(_Where);
                            _Where += _Where.IndexOf("WHERE") > -1 ? _Where : (string.IsNullOrEmpty(_Where) ? "" : " WHERE " + _Where);
                            Query += _Where;
                        }
                        else
                        {
                            _Where = Tools.Tools.ConvertToSQLQuery(searchForm.ConditionQuery);
                            _Where = _Where.IndexOf("WHERE") > -1 ? _Where : (string.IsNullOrEmpty(_Where) ?"": " WHERE " + _Where) ;
                            Query += _Where;
                        }

                        Query += searchForm.CommonConditionQuery == "" ? "" :(_Where==""?"\nWhere 1=1\n ":"") +" And " + Tools.Tools.ConvertToSQLQuery(searchForm.CommonConditionQuery);
                        Query += searchForm.GroupByQuery == "" ? "" : " \nGroup by " + Tools.Tools.ConvertToSQLQuery(searchForm.GroupByQuery);
                        DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {

                                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                    Output = DataBase.SelectDataTable(Query);
                                    break;
                                }
                            case CoreDefine.DataSourceType.MySql:
                                {

                                    Query = Tools.Tools.CheckQuery(Query);
                                    MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                    string IdentityField = DataBase.GetIdentityTable(Form.FullName).ToString();

                                    if (IdentityField != "")
                                        Query = "Declare @" + IdentityField + " As Bigint =  " + _ParentID + Tools.Tools.NewLine + Query;

                                    Output = DataBase.SelectDataTable(Query);
                                    break;
                                }
                            case CoreDefine.DataSourceType.ACCESS:
                                {
                                    Query = Tools.Tools.CheckAccessQuery(Query);

                                    AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                    Output = DataBase.SelectDataTable(Query);
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

                                        Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                                    }
                                    break;
                                }
                        }

                        break;
                    }
                case CoreDefine.Entities.جدول:
                    {
                        Table Table = new Table(Form);
                        string TableIdentityField = Table.IDField().FieldName;

                        string Query = "Select Top ";
                        Query += _ShowRowCount == 0 ? Table.ShowRecordCountDefault.ToString() : _ShowRowCount.ToString();
                        Query += "\n"; 
                        List<CoreObject> FieldObject = CoreObject.FindChilds(Form.CoreObjectID, CoreDefine.Entities.فیلد);

                        foreach (CoreObject Field in FieldObject)
                        { 
                            Query += "[" + Table.FullName + "]." + "[" + Field.FullName + "] AS [" + Tools.Tools.SafeTitle(Field.FullName.Replace("-", "_").Replace("/", "_")) + "]\n,";
                        }

                        List<CoreObject> ComputationalFieldObject = CoreObject.FindChilds(Form.CoreObjectID, CoreDefine.Entities.فیلد_محاسباتی);
                        foreach (CoreObject Field in ComputationalFieldObject)
                        {
                            ComputationalField computationalField = new ComputationalField(Field);
                            Query += "(" + computationalField.Query.Replace("@", "") + ") AS [" + Field.FullName + "]\n,";
                        }

                        Query = Query.Substring(0, Query.Length - 1);
                        Query += " From ["+ Table.TABLESCHEMA+ "].[" + Form.FullName + "]" + "\n" + _Where;


                        DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(Form.ParentID));
                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {

                                    Query = Tools.Tools.CheckQuery(Query);
                                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);

                                    string IdentityField = DataBase.GetIdentityTable(Form.FullName).ToString();

                                    if (IdentityField != "")
                                        Query = "Declare @" + IdentityField + " As Bigint =  " + _ParentID + Tools.Tools.NewLine + Query;

                                    Output = DataBase.SelectDataTable(Query);
                                    break;
                                }
                            case CoreDefine.DataSourceType.MySql:
                                {

                                    Query = Tools.Tools.CheckQuery(Query);
                                    MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                    string IdentityField = DataBase.GetIdentityTable(Form.FullName).ToString();

                                    if (IdentityField != "")
                                        Query = "Declare @" + IdentityField + " As Bigint =  " + _ParentID + Tools.Tools.NewLine + Query;

                                    Output = DataBase.SelectDataTable(Query);
                                    break;
                                }
                            case CoreDefine.DataSourceType.ACCESS:
                                {
                                    Query = Tools.Tools.CheckAccessQuery(Query);

                                    AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                    Output = DataBase.SelectDataTable(Query);
                                    break;
                                }
                            case CoreDefine.DataSourceType.EXCEL:
                                {
                                    using (ExcelEngine excelEngine = new ExcelEngine())
                                    {
                                        IApplication application = excelEngine.Excel;

                                        application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                        IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                        IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                                        Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                                    }
                                    break;
                                }
                        }

                        break;
                    }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        InformationEntryForm _InformationEntryForm = new InformationEntryForm(Form);
                        CoreObject TableObject = CoreObject.Find(_InformationEntryForm.RelatedTable);
                        CoreObject ExternalFieldObject = CoreObject.Find(_InformationEntryForm.ExternalField);
                        Table Table = new Table(TableObject);
                        string IdentityField = Table.IDField().FieldName;
                        string DeclareQuery = "Declare @" + IdentityField + " AS  Bigint = " + _ParentID + Tools.Tools.NewLine;

                        long RowCount = _ShowRowCount == 0 ? _InformationEntryForm.ShowRecordCountDefault : _ShowRowCount;
                        string Query = _InformationEntryForm.Query.Trim() == "" ? Tools.DataConvertor.DefaultQueryForTable(_InformationEntryForm.RelatedTable, RowCount) : "Select Top " + RowCount + "\n" + _InformationEntryForm.Query;
                        Query = Tools.Tools.ConvertToSQLQuery(Query);

                        if(ProcessID>0 && ProcessStepID==0)
                        {
                            int FindIndex = Query.LastIndexOf("FROM");
                            if(FindIndex > -1)
                            {
                                string SubQuery=Query.Substring(FindIndex,Query.Length-FindIndex);
                                Query = Query.Substring(0, FindIndex);
                                Query += "\n,[مراحل_فرآیند].[مرحله_فرآیند]  AS آخرین_مرحله_اجرا_شده_فرآیند";
                                Query += "\n,[مراحل_فرآیند].[عنوان_مرحله]  AS عنوان_آخرین_مرحله_اجرا_شده_فرآیند";
                                Query += "\n,[مراحل_فرآیند].[مرحله_بعد_فرآیند]  AS مرحله_بعد_فرآیند";
                                Query += "\n,[مراحل_فرآیند].[عنوان_مرحله_بعد_فرآیند]  AS عنوان_مرحله_بعد_فرآیند";
                                Query += "\n,[مراحل_فرآیند].[تاریخ_ثبت] + N' - '+[مراحل_فرآیند].[ساعت_ثبت]   AS تاریخ_و_ساعت_آخرین_مرحله_اجرا_شده_فرآیند";
                                Query = Query + "\n" + SubQuery;
                                Query += "\n LEFT JOIN مراحل_فرآیند ON [مراحل_فرآیند].[جدول] = "+ TableObject.CoreObjectID + " AND [مراحل_فرآیند].[سطر] = [" + TableObject.FullName+"].["+ IdentityField + "] AND [مراحل_فرآیند].[فرآیند] = "+ProcessID.ToString()+ " AND ([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت])=(SELECT Max(([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت])) FROM [مراحل_فرآیند] WHERE  [مراحل_فرآیند].[جدول] = " + TableObject.CoreObjectID + " AND [مراحل_فرآیند].[سطر] = [" + TableObject.FullName + "].[" + IdentityField + "] AND [مراحل_فرآیند].[فرآیند] = " + ProcessID.ToString()+ ")";

                            }
                        }

                        if (_InformationEntryForm.ExternalField > 0 && _InformationEntryForm.ParentID > 0)
                        {
                            if (_InformationEntryForm.Query == "")
                                Query += " WHERE " + TableObject.FullName + "." + ExternalFieldObject.FullName + "=@" + IdentityField + (_Where != "" ? " And " + _Where : "") + (string.IsNullOrEmpty(_InformationEntryForm.ConditionQuery) ? "" : " And "+ Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.ConditionQuery));
                            else
                            {
                                int LastIndexOf_FROM = Query.ToUpper().LastIndexOf("FROM");
                                string SubStrQuery = Query.Substring(LastIndexOf_FROM, (Query.Length - 1 - LastIndexOf_FROM));
                                if (SubStrQuery.ToUpper().IndexOf("WHERE") == -1)
                                    Query += "\n WHERE " + TableObject.FullName + "." + ExternalFieldObject.FullName + "=@" + IdentityField + (_Where != "" ? " And " + _Where : "")+ (string.IsNullOrEmpty(_InformationEntryForm.ConditionQuery) ? "  " : " AND "+Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.ConditionQuery));
                            }

                            Query += string.IsNullOrEmpty(_InformationEntryForm.GroupByQuery) ? "" : "\n\nGroup By " + Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.GroupByQuery);
                            Query += string.IsNullOrEmpty(_InformationEntryForm.OrderQuery) ? "" : "\nOrder By " + Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.OrderQuery);
                        }
                        else
                        {
                            Query += "\nWhere \n" + (string.IsNullOrEmpty(_Where) ? (string.IsNullOrEmpty(_InformationEntryForm.ConditionQuery) ? " 1 = 1 " : Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.ConditionQuery)) : _Where);
                            if(ProcessID> 0 && ProcessStepID == 0)
                            {
                                Query += "\nAND [مراحل_فرآیند].[مرحله_فرآیند]  is not null";
                                Query += "\nAND [مراحل_فرآیند].[مرحله_بعد_فرآیند] > 0";
                                Query += "\nAND( @شناسه_کاربر IN (SELECT  مراحل_فرآیند.[اجرا_کننده] from مراحل_فرآیند where  [مراحل_فرآیند].[سطر] = [" + TableObject.FullName + "].[" + IdentityField + "] AND [مراحل_فرآیند].[فرآیند] = " + ProcessID.ToString()+")";
                                Query += "\nOR @شناسه_کاربر IN  (SELECT [ارجاع_مراحل_فرآیند].[دریافت_کننده] FROM [ارجاع_مراحل_فرآیند] WHERE [ارجاع_مراحل_فرآیند].[فرآیند] = "+ProcessID+" AND [ارجاع_مراحل_فرآیند].[رکورد] = ["+ TableObject.FullName + "].[" + IdentityField + "]))";
                                Query += "\nORDER BY ([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت]) desc";
                            }
                            else
                            { 
                                Query += string.IsNullOrEmpty(_InformationEntryForm.GroupByQuery) ? "" : "\n\nGroup By " + Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.GroupByQuery);
                                Query += string.IsNullOrEmpty(_InformationEntryForm.OrderQuery) ? "" : "\nOrder By " + Tools.Tools.ConvertToSQLQuery(_InformationEntryForm.OrderQuery);
                            }
                        }


                        if (IdentityField != "")
                        {
                            List<CoreObject> FieldList = CoreObject.FindChilds(_InformationEntryForm.RelatedTable, CoreDefine.Entities.فیلد);

                            DataTable DeclaredataTable = Referral.DBData.SelectDataTable("Select * From " + TableObject.FullName + " Where " + IdentityField + "=" + _ParentID);

                            if (DeclaredataTable != null)
                                foreach (CoreObject FieldCore in FieldList)
                                {
                                    Field Field = new Field(FieldCore);
                                    switch (Field.FieldType)
                                    {
                                        case CoreDefine.InputTypes.ShortText:
                                            {
                                                DeclareQuery += "Declare @" + Field.FieldName + " AS NVARCHAR(400)=N'";
                                                if (DeclaredataTable.Rows.Count > 0)
                                                    DeclareQuery += DeclaredataTable.Rows[0][Field.FieldName].ToString();
                                                DeclareQuery += "'" + Tools.Tools.NewLine;
                                                break;
                                            }
                                        case CoreDefine.InputTypes.RelatedTable:
                                            {
                                                DeclareQuery += "Declare @" + Field.FieldName + " AS  Bigint = ";
                                                if (DeclaredataTable.Rows.Count == 0)
                                                    DeclareQuery += "0";
                                                else
                                                    DeclareQuery += DeclaredataTable.Rows[0][Field.FieldName].ToString() == "" ? "0" : DeclaredataTable.Rows[0][Field.FieldName].ToString();

                                                DeclareQuery += Tools.Tools.NewLine;
                                                break;
                                            }
                                        case CoreDefine.InputTypes.NationalCode:
                                            {
                                                DeclareQuery += "Declare @" + Field.FieldName + " AS NVARCHAR(400)=N'";
                                                if (DeclaredataTable.Rows.Count > 0)
                                                    DeclareQuery += DeclaredataTable.Rows[0][Field.FieldName].ToString();
                                                DeclareQuery += "'" + Tools.Tools.NewLine;
                                                break;
                                            }
                                        case CoreDefine.InputTypes.Number:
                                            {
                                                if (Field.FieldName != IdentityField)
                                                {
                                                    DeclareQuery += "Declare @" + Field.FieldName + " AS  Bigint = ";
                                                    if (DeclaredataTable.Rows.Count == 0)
                                                        DeclareQuery += "0";
                                                    else
                                                        DeclareQuery += DeclaredataTable.Rows[0][Field.FieldName].ToString() == "" ? "null" : DeclaredataTable.Rows[0][Field.FieldName].ToString();

                                                    DeclareQuery += Tools.Tools.NewLine;
                                                }
                                                break;
                                            }
                                        case CoreDefine.InputTypes.ComboBox:
                                            {
                                                DeclareQuery += "Declare @" + Field.FieldName + " AS NVARCHAR(400)= N'";
                                                if (DeclaredataTable.Rows.Count > 0)
                                                    DeclareQuery += DeclaredataTable.Rows[0][Field.FieldName].ToString();
                                                DeclareQuery += "'" + Tools.Tools.NewLine;
                                                break;
                                            }
                                        case CoreDefine.InputTypes.SingleSelectList:
                                            {
                                                DeclareQuery += "Declare @" + Field.FieldName + " AS NVARCHAR(400)= N'";
                                                if (DeclaredataTable.Rows.Count > 0)
                                                    DeclareQuery += DeclaredataTable.Rows[0][Field.FieldName].ToString();
                                                DeclareQuery += "'" + Tools.Tools.NewLine;
                                                break;
                                            }

                                    }
                                }
                        }
                        DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(_InformationEntryForm.RelatedTable).ParentID));
                        Output = DataConvertor.SelectDataTable(DataSourceInfo, DeclareQuery + Tools.Tools.NewLine + Query, Table.FullName);

                        break;
                    }
            }
            return Output;

        }

        public static DataTable SelectRecord(string DataKey, long ID)
        {

            DataTable OutPut = new DataTable();
            CoreObject TableObject = CoreObject.Find(long.Parse(DataKey));

            if (TableObject.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm informationEntryForm = new InformationEntryForm(TableObject);
                TableObject = CoreObject.Find(informationEntryForm.RelatedTable);
            }

            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
            Table table = new Table(TableObject);
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        if (DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                            OutPut = Referral.DBData.SelectDataTable("Select * From " + TableObject.FullName + " Where " + table.IDField().FieldName + " = " + ID);
                        else
                        {
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            OutPut = DataBase.SelectDataTable("Select * From "+ table.TABLESCHEMA+"." + TableObject.FullName + " Where " + table.IDField().FieldName + " = " + ID);
                        }
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        //MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        //Id = DataBase.Insert(DataTable[_DataKey].CoreObjectID, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        //AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        //Id = DataBase.Insert(DataTable[_DataKey].CoreObjectID, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        break;
                    }
            }

            return OutPut;
        }
        public static DataTable Read(string _DataKey, string _Where, int _ParentID, int _ShowRowCount = 0,long ProcessID=0,long ProcessStepID=0)
        {
            DataTable Output = SelectTable(_DataKey, _Where, _ParentID, _ShowRowCount, ProcessID, ProcessStepID);
            DataConvertor.ViewFormat(ref Output, DataFields[_DataKey]);
            return Output;
        }

        public static string CheckBeforRunQuery(long TableID, long RowID, CoreDefine.TableEvents TableEvent, string[] FormInputName, object[] FormInputValue, string DataKey = "")
        {
            string Alarm = string.Empty;
            string Query = string.Empty; 
            CoreObject TableObject = CoreObject.Find(TableID); 

            switch(TableObject.Entity)
            { 
                case CoreDefine.Entities.فرم_ورود_اطلاعات: { 
                        InformationEntryForm information = new InformationEntryForm(TableObject);
                        TableObject = CoreObject.Find(information.RelatedTable); 
                        break; 
                    }
            }


            Table Table = new Table(TableObject);
            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
            List<TableEvent> TableEventList = new List<TableEvent>();
            string IDField = Table.IDField().FieldName;

            List<Field> FieldList = new List<Field>();
            List<Field> IsExclusiveFieldList = new List<Field>();

            if (DataKey != "")
            {
                FieldList = DataFields[DataKey];
            }
            else
            {
                if (TableDataFields[TableObject.CoreObjectID.ToString()] == null)
                    StartupSettingTableDataFields(TableObject.CoreObjectID);

                FieldList = TableDataFields[TableObject.CoreObjectID.ToString()];
            }

            foreach (Field Item in FieldList)
                if (!Item.IsIdentity && !Item.IsVirtual)
                    if (Item.IsExclusive)
                        IsExclusiveFieldList.Add(Item);

            foreach (CoreObject Item in CoreObject.FindChilds(TableID, CoreDefine.Entities.رویداد_جدول))
            {
                TableEvent Event = new TableEvent(Item);
                if (Event.EventType == TableEvent.ToString())
                    TableEventList.Add(Event);
            }


            if (IsExclusiveFieldList.Count > 0 || TableEventList.Count > 0)
            {
                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            Query = DataBase.DefineVariablesQuery(TableObject.FullName, RowID, FormInputName, FormInputValue);

                            foreach (TableEvent Event in TableEventList)
                            {
                                if (Event.EventType == TableEvent.ToString())
                                    Alarm = DataBase.SelectField(Query + "\n" + Tools.Tools.CheckQuery(Event.Query)).ToString();
                                if (Alarm != "")
                                    return Alarm;
                            }

                            foreach (Field Item in IsExclusiveFieldList)
                                if (!Item.IsIdentity && !Item.IsVirtual)
                                {
                                    int FindIndex = Array.IndexOf(FormInputName, Item.FieldName);
                                    if (Item.IsExclusive && FindIndex > -1)
                                    {
                                        if (FormInputValue[FindIndex].ToString() != "" && FormInputValue[FindIndex].ToString() != "0")
                                        {
                                            string MainQuery = "IF((SELECT COUNT(1) FROM " + TableObject.FullName + " WHERE " + TableObject.FullName + "." + Item.FieldName + " = @" + Item.FieldName + "  and " + TableObject.FullName + "." + IDField + "<>@" + IDField + ") > 0 ) BEGIN SELECT N'" + Tools.Tools.UnSafeTitle(Item.FieldName) + " تکراری می باشد' END ELSE BEGIN SELECT N'' END ";
                                            Alarm = DataBase.SelectField(Query + "\n" + MainQuery).ToString();
                                            if (Alarm != "")
                                                return Alarm;

                                        }
                                    }
                                }
                            break;
                        }
                    case CoreDefine.DataSourceType.MySql:
                        {
                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                            Query = DataBase.DefineVariablesQuery(TableObject.FullName, RowID, FormInputName, FormInputValue);

                            foreach (TableEvent Event in TableEventList)
                            {
                                if (Event.EventType == TableEvent.ToString())
                                    Alarm = DataBase.SelectField(Query + "\n" + Tools.Tools.CheckQuery(Event.Query)).ToString();
                                if (Alarm != "")
                                    return Alarm;
                            }

                            foreach (Field Item in IsExclusiveFieldList)
                                if (!Item.IsIdentity && !Item.IsVirtual)
                                {
                                    if (Item.IsExclusive)
                                    {
                                        string MainQuery = "IF((SELECT COUNT(1) FROM " + TableObject.FullName + " WHERE " + TableObject.FullName + "." + Item.FieldName + " = @" + Item.FieldName + "  and " + TableObject.FullName + "." + IDField + "<>@" + IDField + ") > 0 ) BEGIN SELECT N'" + Tools.Tools.UnSafeTitle(Item.FieldName) + " تکراری می باشد' END ELSE BEGIN SELECT N'' END ";
                                        Alarm = DataBase.SelectField(Query + "\n" + MainQuery).ToString();
                                        if (Alarm != "")
                                            return Alarm;
                                    }
                                }
                            break;
                        }
                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password);
                            Query = DataBase.DefineVariablesQuery(TableObject.CoreObjectID, RowID, FormInputName, FormInputValue);


                            foreach (TableEvent Event in TableEventList)
                            {
                                if (Event.EventType == TableEvent.ToString())
                                    Alarm = DataBase.SelectField(Query + "\n" + Tools.Tools.CheckQuery(Event.Query)).ToString();
                                if (Alarm != "")
                                    return Alarm;
                            }

                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            break;
                        }
                }
            }

            return Alarm;
        }

        public static async Task<bool> AfterRunQuery(CoreDefine.TableEvents TableEventType, CoreObject TableCore, long RecordID, string[] ColumnNames, object[] _Values, string FileDestinationPath, string _DataKey)
        {
            List<CoreObject> TableEventList = CoreObject.FindChilds(TableCore.CoreObjectID, CoreDefine.Entities.رویداد_جدول);
            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);

            if(!string.IsNullOrEmpty(_DataKey))
            { 
                CoreObject InformationCore = CoreObject.Find(long.Parse(_DataKey));
                if (InformationCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                    TableEventList.AddRange(CoreObject.FindChilds(InformationCore.CoreObjectID, CoreDefine.Entities.رویداد_جدول));
            }


            foreach (CoreObject ParameterCore in TableEventList)
            {
                TableEvent Event = new TableEvent(ParameterCore);
                if (Event.EventType == TableEventType.ToString())
                {
                    bool RunQuery = true;

                    if (TableEventType == CoreDefine.TableEvents.دستور_بعد_از_درج)
                        DataBase.DefineVariablesQuery(TableCore.FullName, RecordID, ref ColumnNames, ref _Values);

                    string DefineVariablesQuery = DataBase.DefineVariablesQuery(TableCore.FullName, RecordID, ColumnNames, _Values) + "\n";
                    if (!string.IsNullOrEmpty(Event.Condition))
                    {
                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {
                                    RunQuery = DataBase.SelectField(DefineVariablesQuery + Tools.Tools.CheckQuery(Event.Condition)).ToString() == "" ? true : false;
                                    break;
                                }
                        }

                    }

                    if (RunQuery)
                    {
                        if (Event.TypeEventExecution == CoreDefine.TableTypeEventExecution.صدور_گواهی_نامه_الکترونیکی_Src.ToString() && Array.IndexOf(ColumnNames, "نوع_مودی") > -1)
                        {
                            string path = System.Web.HttpContext.Current.Server.MapPath("~/OpenSSL/" + Referral.UserAccount.UsersID.ToString() + "/");
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            else
                            {
                                Directory.Delete(path, true);
                                Directory.CreateDirectory(path);
                            }

                            DirectoryInfo source = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath("~/Tools/OpenSSL"));
                            DirectoryInfo Destination = new DirectoryInfo(path);
                            foreach (FileInfo file in source.GetFiles())
                            {
                                file.CopyTo(Path.Combine(Destination.FullName, file.Name), true);
                            }

                            string FileName = "fa.cnf";

                            using (StreamWriter writer = new StreamWriter(path + "/" + FileName, true))
                            {
                                if (_Values[Array.IndexOf(ColumnNames, "نوع_مودی")].ToString().ToLower() == "true")
                                {
                                    writer.WriteLine("[req]");
                                    writer.WriteLine("prompt = no");
                                    writer.WriteLine("distinguished_name = dn");
                                    writer.WriteLine("[dn]");
                                    writer.WriteLine("CN = " + _Values[Array.IndexOf(ColumnNames, "نام_شخص_به_انگلیسی")] + " [Stamp]");
                                    writer.WriteLine("serialNumber = " + _Values[Array.IndexOf(ColumnNames, "شناسه_ملی")]);

                                    switch (_Values[Array.IndexOf(ColumnNames, "نوع_متقاضی")].ToString())
                                    {
                                        case "مستقل": { writer.WriteLine("O = Unaffiliated "); break; }
                                        case "وابسته به دولت": { writer.WriteLine("O = Governmental "); break; }
                                        case "وابسته به غیر دولت": { writer.WriteLine("O = Non-Governmental "); break; }
                                    }
                                    if (Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_3") > -1)
                                        if (_Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_3")].ToString() != "")
                                            writer.WriteLine("3.OU = " + _Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_3")]);

                                    if (Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_2") > -1)
                                        if (_Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_2")].ToString() != "")
                                            writer.WriteLine("2.OU = " + _Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_2")]);

                                    if (Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_1") > -1)
                                        if (_Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_1")].ToString() != "")
                                            writer.WriteLine("1.OU = " + _Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_1")]);

                                    writer.WriteLine("OU = " + _Values[Array.IndexOf(ColumnNames, "نام_سازمان")]);
                                    writer.WriteLine("C = IR");

                                }
                                else
                                {
                                    writer.WriteLine("[req]");
                                    writer.WriteLine("prompt = no");
                                    writer.WriteLine("distinguished_name = dn");
                                    writer.WriteLine("[dn]");
                                    writer.WriteLine("CN = " + _Values[Array.IndexOf(ColumnNames, "نام_شخص_به_انگلیسی")] + " [Stamp]");
                                    writer.WriteLine("serialNumber = " + _Values[Array.IndexOf(ColumnNames, "کد_ملی_متقاضی")]);

                                    switch (_Values[Array.IndexOf(ColumnNames, "نوع_متقاضی")].ToString())
                                    {
                                        case "مستقل": { writer.WriteLine("O = Unaffiliated "); break; }
                                        case "وابسته به دولت": { writer.WriteLine("O = Governmental "); break; }
                                        case "وابسته به غیر دولت": { writer.WriteLine("O = Non-Governmental "); break; }
                                    }

                                    if (Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_3") > -1)
                                        if (_Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_3")].ToString() != "")
                                            writer.WriteLine("3.OU = " + _Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_3")]);

                                    if (Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_2") > -1)
                                        if (_Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_2")].ToString() != "")
                                            writer.WriteLine("2.OU = " + _Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_2")]);

                                    if (Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_1") > -1)
                                        if (_Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_1")].ToString() != "")
                                            writer.WriteLine("1.OU = " + _Values[Array.IndexOf(ColumnNames, "نام_واحد_سازمانی_1")]);

                                    writer.WriteLine("OU = " + _Values[Array.IndexOf(ColumnNames, "نام_سازمان")]);
                                    writer.WriteLine("SN = " + _Values[Array.IndexOf(ColumnNames, "نام_خانوادگی_متقاضی")]);
                                    //writer.WriteLine("G = " + _Values[Array.IndexOf(ColumnNames, "نام_متقاضی")]);
                                    //writer.WriteLine("T = " + _Values[Array.IndexOf(ColumnNames, "سمت_متقاضی")]);
                                    //writer.WriteLine("T = Expert");
                                    //writer.WriteLine("S = " + _Values[Array.IndexOf(ColumnNames, "نام_استان")]);
                                    //writer.WriteLine("L = " + _Values[Array.IndexOf(ColumnNames, "نام_شهرستان")]);
                                    //writer.WriteLine("OrganizationIdentifier = " + _Values[Array.IndexOf(ColumnNames, "شناسه_ملی")]);
                                    writer.WriteLine("C = IR");
                                }
                            }

                            OpenSSL openSSL = new OpenSSL();
                            string Command = "openssl req -new -utf8 -nameopt multiline,utf8 -config " + FileName + " -newkey rsa:2048 -nodes -keyout fa.key -out fa.csr";
                            string CMDError = "";
                            openSSL.ExecuteCMD(path, Command, ref CMDError);

                            using (StreamWriter writer = new StreamWriter(path + "/CMDError.txt", true))
                            {
                                writer.WriteLine(CMDError);
                            }


                            //Command = "openssl req -in fa.csr -noout -pubkey -out publickey.pem";
                            //Command = @"openssl req -config D:\proje\.net\APM\APM\OpenSSL\1\openssl.cnf -x509 -days 365 -newkey rsa:1024 -keyout hostkey.pem -nodes -out hostcert.pem";
                            Command = "openssl req -config " + path + "openssl.cnf -in fa.csr -noout -pubkey -out publickey.pem";
                            //Command = @"openssl x509 -in D:\proje\.net\APM\APM\OpenSSL\1\fa.csr -out D:\proje\.net\APM\APM\OpenSSL\1\server.crt -req -signkey D:\proje\.net\APM\APM\OpenSSL\1\fa.key -days 365";
                            CMDError = "";
                            openSSL.ExecuteCMD(path, Command, ref CMDError);

                            using (StreamWriter writer = new StreamWriter(path + "/CMDError.txt", true))
                            {
                                writer.WriteLine(CMDError);
                            }

                            string PDestinationPath = Models.Attachment.MapFileSavingAttachmentPath + TableCore.CoreObjectID + "/" + RecordID;


                            string[] KeyArr = new string[] { "fa.csr", "fa.key", "publickey.pem" };
                            string[] KeyNameArr = new string[] { "CSR.txt", "PrivateKey.txt", "Publickey.txt" };
                            for (int Index = 0; Index < KeyArr.Length; Index++)
                            {
                                if (File.Exists(path + KeyArr[Index]))
                                {
                                    byte[] fileData = System.IO.File.ReadAllBytes(path + KeyArr[Index]);
                                    string ContentFile = System.IO.File.ReadAllText(path + KeyArr[Index]);
                                    if (KeyNameArr[Index] == "CSR.txt")
                                    {
                                        ContentFile = ContentFile.Replace("-----BEGIN CERTIFICATE REQUEST-----\r\n", "");
                                        ContentFile = ContentFile.Replace("-----END CERTIFICATE REQUEST-----", "");
                                        ContentFile = ContentFile.Substring(0, ContentFile.Length - 4);
                                        DataBase.Execute("Update درخواست_گواهی_امضا_الکترونیکی set کلید_CSR=N'" + ContentFile + "' Where شناسه = " + RecordID);

                                    }
                                    if (KeyNameArr[Index] == "PrivateKey.txt")
                                    {
                                        ContentFile = ContentFile.Replace("-----BEGIN PRIVATE KEY-----\r\n", "");
                                        ContentFile = ContentFile.Replace("-----END PRIVATE KEY-----", "");
                                        ContentFile = ContentFile.Substring(0, ContentFile.Length - 4);
                                        DataBase.Execute("Update درخواست_گواهی_امضا_الکترونیکی set کلید_خصوصی=N'" + ContentFile + "' Where شناسه = " + RecordID);
                                    }
                                    if (KeyNameArr[Index] == "Publickey.txt")
                                    {
                                        ContentFile = ContentFile.Replace("-----BEGIN PUBLIC KEY-----\r\n", "");
                                        ContentFile = ContentFile.Replace("-----END PUBLIC KEY-----", "");
                                        ContentFile = ContentFile.Substring(0, ContentFile.Length - 4);
                                        DataBase.Execute("Update درخواست_گواهی_امضا_الکترونیکی set کلید_عمومی=N'" + ContentFile + "' Where شناسه = " + RecordID);
                                    }
                                    string Text = Convert.ToBase64String(fileData);
                                    System.IO.File.WriteAllText(PDestinationPath + "/" + KeyNameArr[Index], Text);
                                    FileInfo File2 = new FileInfo(PDestinationPath + "/" + KeyNameArr[Index]);
                                    byte[] ThumbnailImageBytes = Models.Attachment.CreateThumbnailImage(File2.FullName);
                                    Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "URL", "Thumbnail" }
                                                                                    , new object[] { TableCore.CoreObjectID, RecordID, "", File2.Name, File2.Extension.Replace(".", ""), File2.Length, path.Replace("/", "\\").Replace(@"\\", "\\"), ThumbnailImageBytes });

                                }
                            }

                            Directory.Delete(path, true);
                        }
                        else if (Event.TypeEventExecution == CoreDefine.TableTypeEventExecution.ارسال_ایمیل.ToString())
                        {

                            MailCore mailCore = new MailCore()
                            {
                                EMail = Event.EMail,
                                EMailUserName = Event.EMailUserName,
                                EMailPassword = Event.EMailPassWord,
                                EMailServer = Event.EMailServer,
                                EMailPort = Event.EMailPort,
                                EnableSsl = Event.EnableSsl,
                                ReceivingUsers = Event.ReceivingUsers,
                                ReceivingRole = Event.ReceivingRole,
                                InsertingUser = Event.InsertingUser,
                                ReceivingQuery = Event.ReceivingQuery,
                                SendAttachmentFile = Event.SendAttachmentFile,
                                UsePublickEmail = Event.UsePublickEmail,
                                SendReport = Event.SendReport,
                                Title = Event.Title,
                                TitleQuery = Event.TitleQuery,
                                BodyMessage = Event.BodyMessage,
                                BodyMessageQuery = Event.BodyMessageQuery
                            };
                            mailCore.SyncSendMail(DefineVariablesQuery, _DataKey, RecordID.ToString(), TableCore.CoreObjectID.ToString(), ColumnNames, _Values);
                        }
                        else if (Event.TypeEventExecution == CoreDefine.TableTypeEventExecution.ارجاع.ToString())
                        {
                            TaskReferral taskReferral = new TaskReferral()
                            {
                                InformarmationFormID = long.Parse(_DataKey),
                                ProcessID = 0,
                                ProcessStepID = 0,
                                RecordID = RecordID,
                                ReferralDeadlineResponse = Event.ReferralDeadlineResponse,
                                ReferralRecipientsQuery = Event.ReferralRecipientsQuery,
                                ReferralRecipientsRole = Event.ReferralRecipientsRole,
                                ReferralRecipientsUser = Event.ReferralRecipientsUser,
                                ReferralTitle = Event.ReferralTitle,
                                ReferralTitleQuery = Event.ReferralTitleQuery,
                                TableID = TableCore.CoreObjectID
                            };
                            taskReferral.SyncSendTask(DefineVariablesQuery, _DataKey, RecordID.ToString(), TableCore.CoreObjectID.ToString(), ColumnNames, _Values);
                        }

                        if (Event.Query != "")
                        {
                            switch (DataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        string Query2 = DefineVariablesQuery + "\n" + Tools.Tools.CheckQuery(Event.Query);

                                        bool result = DataBase.Execute(Query2);
                                        if (Event.RelatedTable > 0 && result)
                                        {
                                            CoreObject TableName = CoreObject.Find(Event.RelatedTable);
                                            string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
                                            object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableName.FullName, Referral.UserAccount.IP, DataBase.ConnectionData.Source, DataBase.ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
                                            Referral.DBRegistry.Insert("Insert_APMRegistry", ColumnName, Values);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static bool SaveProcessStep(long ProcessID, long ProcessStepID ,long TableID,long RecordID,long InformationFormID)
        {
            try
            {
                long NextProcessStepID = 0;
                long InformationFormId = 0;
                string InformationFormTitle = string.Empty;
                if(InformationFormID>0)
                Tools.Tools.GetNextProcessStep(ProcessID, ProcessStepID, ref NextProcessStepID, ref InformationFormId, ref InformationFormTitle, TableID, RecordID);
                ProcessStep processStep = new ProcessStep(CoreObject.Find(ProcessStepID));
                ProcessStep NextProcessStep = InformationFormID > 0? new ProcessStep(CoreObject.Find(NextProcessStepID)):new ProcessStep();
                Referral.DBData.Execute("Insert into مراحل_فرآیند (فرآیند" +
                ", مرحله_فرآیند" +
                ", جدول" +
                ", سطر" +
                ", تاریخ_ثبت" +
                ", ساعت_ثبت" +
                ", اجرا_کننده" +
                ",عنوان_مرحله" +
                ",مرحله_بعد_فرآیند" +
                ",عنوان_مرحله_بعد_فرآیند" +
                ",نام_ثبت_کننده) values (" +
                ProcessID + "," +
                ProcessStepID + "," +
                TableID + "," +
                RecordID + "," +
                "N'" + CDateTime.GetNowshamsiDate() + "'," +
                "N'" + CDateTime.GetNowTime() + "'," +
                Referral.UserAccount.UsersID + "," +
                "N'" + processStep.Name + "'," +
                NextProcessStep.CoreObjectID + "," +
                "N'" + NextProcessStep.Name + "'," +
                "N'"+Referral.UserAccount.FullName+"')");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            } 
        }
        public static long Create(string _DataKey, string[] FormInputName, object[] FormInputValue,string InCellRowID="0")
        {
            List<Field> TableFields = new List<Field>();
            List<string> FieldNames = new List<string>();
            List<object> Values = new List<object>();
            long Id = 0;
            CoreObject TableObject = CoreObject.Find(long.Parse(_DataKey));
            CoreObject _ExternalField = new CoreObject();

            if (TableObject.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm informationEntryForm = new InformationEntryForm(TableObject);
                TableObject = CoreObject.Find(informationEntryForm.RelatedTable);

                if (informationEntryForm.ExternalField > 0)
                    _ExternalField = CoreObject.Find(informationEntryForm.ExternalField);

            }


            List<CoreObject> AttachmentCore = CoreObject.FindChilds(TableObject.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول);
            string[] AttachmentList = new string[AttachmentCore.Count];
            for (int i = 0; i < AttachmentCore.Count; i++)
            {
                AttachmentList[i] = AttachmentCore[i].FullName.ToString();
            }
            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            Table table = new Table(TableObject);
            string IDField = table.IDField().FieldName;
            int FindIDFieldIndex = Array.IndexOf(FormInputName, IDField);

            foreach (Field Item in DataFields[_DataKey])
                if (Item.FieldName != IDField && !Item.IsIdentity && !Item.IsVirtual && Array.IndexOf(AttachmentList, Item.FieldName) == -1)
                    TableFields.Add(Item);

            foreach (Field Item in TableFields)
            {
                int index = Array.IndexOf(FormInputName, Item.FieldName);
                if (index > -1)
                {
                    Values.Add(Item.ToFormatField(Item.FieldType, FormInputValue[index]));
                    FieldNames.Add(Item.FieldName);
                }
            }

            if (_ExternalField.FullName != "")
            {
                int FindIndex = Array.IndexOf(FormInputName, _ExternalField.FullName);
                if (FieldNames.IndexOf(_ExternalField.FullName) == -1 && FindIndex > -1)
                {
                    Values.Add(FormInputValue[FindIndex]);
                    FieldNames.Add(FormInputName[FindIndex]);
                }
            }
            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        if(Referral.DBData.ConnectionData.DBConnection.Database == DataSourceInfo.DataBase && Referral.DBData.ConnectionData.DBConnection.DataSource== DataSourceInfo.ServerName)
                            Id=Referral.DBData.Insert(DataTable[_DataKey].FullName, FieldNames.ToArray(), Values.ToArray(), DataTable[_DataKey].CoreObjectID);
                        else
                        {
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            Id = DataBase.Insert(DataTable[_DataKey].FullName, FieldNames.ToArray(), Values.ToArray(), DataTable[_DataKey].CoreObjectID); 
                        }
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        Id = DataBase.Insert(DataTable[_DataKey].CoreObjectID, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        Id = DataBase.Insert(DataTable[_DataKey].CoreObjectID, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        break;
                    }
            }

            string PSourcePath = Models.Attachment.MapTemporaryFilePath + _DataKey + "/" + (FindIDFieldIndex > -1 ? (FormInputValue[FindIDFieldIndex].ToString()==""? InCellRowID : FormInputValue[FindIDFieldIndex].ToString()) : InCellRowID);
            string PDestinationPath = Models.Attachment.MapFileSavingAttachmentPath + (TableObject.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات ? DataInformationEntryForm[_DataKey].RelatedTable.ToString() : TableObject.CoreObjectID.ToString()) + "/" + Id;
            Models.Attachment.SaveAttachment(new DirectoryInfo(PSourcePath), Directory.CreateDirectory(PDestinationPath), (TableObject.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات ? DataInformationEntryForm[_DataKey].RelatedTable : TableObject.CoreObjectID), Id);
            Models.Attachment.DeleteDirectory(PSourcePath);
            _ = AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, TableObject, Id, FieldNames.ToArray(), Values.ToArray(), PDestinationPath, _DataKey);
            return Id;
        }


        public static bool Update(long _TableID, long RowID, string[] FormInputName, object[] FormInputValue,string _Datakey="")
        {
            CoreObject TableCore = CoreObject.Find(_TableID);
            List<Field> TableFields = new List<Field>();
            List<string> FieldNames = new List<string>();
            List<object> Values = new List<object>();
            List<CoreObject> AttachmentCore = CoreObject.FindChilds(_TableID, CoreDefine.Entities.ضمیمه_جدول);
            string[] AttachmentList = new string[AttachmentCore.Count];
            for (int i = 0; i < AttachmentCore.Count; i++)
            {
                AttachmentList[i] = AttachmentCore[i].FullName.ToString();
            }

            if (TableDataFields[_TableID.ToString()] == null)
                StartupSettingTableDataFields(_TableID);

            foreach (Field Item in TableDataFields[_TableID.ToString()])
                if (!Item.IsIdentity && !Item.IsVirtual && Array.IndexOf(AttachmentList, Item.FieldName) == -1)
                    TableFields.Add(Item);

            foreach (Field Item in TableFields)
            {
                int Index = Array.IndexOf(FormInputName, Item.FieldName);
                if (Index > -1)
                {
                    //if(Item.FieldType==CoreDefine.InputTypes.Password)
                    //   Values.Add(Security.Hash.EncryptAes( FormInputValue[Index].ToString()));
                    //else
                    Values.Add(FormInputValue[Index]);
                    FieldNames.Add(Item.FieldName);
                }
                else
                {
                    Index = Array.IndexOf(FormInputName, _TableID.ToString() + Item.FieldName);
                    if (Index > -1)
                    {
                        Values.Add(FormInputValue[Index]);
                        FieldNames.Add(Item.FieldName);
                    }
                }
            }


            bool UpdateResult = false;

            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                        UpdateResult = DataBase.UpdateRow(RowID, TableCore.CoreObjectID, TableCore.FullName, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        UpdateResult = DataBase.UpdateRow(RowID, TableCore.CoreObjectID, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        UpdateResult = DataBase.UpdateRow(RowID, TableCore.CoreObjectID, FieldNames.ToArray(), Values.ToArray());
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        break;
                    }
            }

            if (UpdateResult)
                AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, TableCore, RowID, FieldNames.ToArray(), Values.ToArray(), "", _Datakey);
            return UpdateResult;

        }

        public static bool Update(string _DataKey, string _FormCollection)
        {
            bool Output = true;

            List<string> FieldNames = new List<string>();
            List<object> Values = new List<object>();
            dynamic FormCollectionData = JObject.Parse(_FormCollection);
            int RowID = GetColumnWidth(FormCollectionData[DataTable[_DataKey].IDField().FieldName].Value.ToString());

            foreach (Field Item in DataFields[_DataKey])
            {
                if (Item.IsEditAble)
                {
                    object Value = Item.ToFormatField(Item.FieldType, FormCollectionData[Item.FieldName].Value);
                    //        if (Item.FieldName == "پلاک_خودرو" && Item.ToFormatSave(Item.FieldType, _FormCollection[FieldName]).ToString().Length == 8)
                    //        {
                    //            Value = DataConvertor.NationalPlaque(Value.ToString());
                    //        }

                    FieldNames.Add(Item.FieldName);
                    Values.Add(Value);
                    //        //if (!UpdateResult)
                    //        //    Output = UpdateResult;
                }
            }

            bool UpdateResult = Referral.DBData.UpdateRow(RowID, long.Parse(_DataKey), DataTable[_DataKey].FullName, FieldNames.ToArray(), Values.ToArray());
            //Attachment.SaveAttachment(DataTable[_DataKey].CoreObjectID, RowID);
            return Output;
        }

        public static bool UpdateField(string _DataKey, long _RecordID, string _FieldName, object _NewValue)
        {
            return Referral.DBData.UpdateRow(_RecordID, long.Parse(_DataKey), DataTable[_DataKey].FullName, new string[] { _FieldName }, new object[] { _NewValue });
        }

        public static int Insert(string _DataKey, string[] _ColumnName, object[] _NewValue)
        {
            return Referral.DBData.Insert(DataTable[_DataKey].FullName, _ColumnName, _NewValue);
        }

        public static void Destroy(string _DataKey, FormCollection _FormCollection)
        {
            CoreObject Form = CoreObject.Find(long.Parse(_DataKey));
            DataSourceInfo DataSourceInfo = new DataSourceInfo();
            string TableName = Form.FullName;
            CoreObject TableObject = Form;
            Table TableInfo = new Table();  

            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            string IDField = DataTable[_DataKey].IDField().FieldName;

            int RowID = int.Parse(_FormCollection[IDField]);

            switch (Form.Entity)
            {
                case CoreDefine.Entities.جدول:
                    {
                        DataSourceInfo = new DataSourceInfo(CoreObject.Find(Form.ParentID));
                        TableInfo=new Table(TableObject);
                        break;
                    }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        InformationEntryForm _InformationEntryForm = new InformationEntryForm(Form);
                        TableObject = CoreObject.Find(_InformationEntryForm.RelatedTable);
                        DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
                        TableName = TableObject.FullName;
                        TableInfo = new Table(TableObject);
                        break;
                    }
            }

            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                        DataTable TableData = DataBase.SelectDataTable("Select * from " +TableInfo.TABLESCHEMA+"." + TableObject.FullName + " Where [" + IDField + "] = " + RowID);
                        string[] _ColumnName = new string[TableData.Columns.Count];
                        object[] _ColumnValue = new object[TableData.Columns.Count];
                        int Index = 0;
                        foreach (DataColumn dataColumn in TableData.Columns)
                        {
                            _ColumnName[Index] = dataColumn.ColumnName;
                            _ColumnValue[Index] = TableData.Rows[0][dataColumn.ColumnName];
                            Index++;
                        }
                        if (DataBase.Delete(TableName, RowID))
                            AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, TableObject, RowID, _ColumnName, _ColumnValue, "", _DataKey);
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        DataBase.Delete(Form.CoreObjectID, RowID);
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        DataBase.Delete(Form.CoreObjectID, RowID);
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        break;
                    }
            }
        }

        public static void Destroy(string _DataKey, long RowID)
        {
            CoreObject Form = CoreObject.Find(long.Parse(_DataKey));
            DataSourceInfo DataSourceInfo = new DataSourceInfo();
            string TableName = Form.FullName;

            if (DataFields[_DataKey] == null)
                StartupSetting(_DataKey);

            switch (Form.Entity)
            {
                case CoreDefine.Entities.جدول:
                    {
                        DataSourceInfo = new DataSourceInfo(CoreObject.Find(Form.ParentID));
                        break;
                    }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        InformationEntryForm _InformationEntryForm = new InformationEntryForm(Form);
                        CoreObject TableObject = CoreObject.Find(_InformationEntryForm.RelatedTable);
                        DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
                        TableName = TableObject.FullName;
                        break;
                    }
            }
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                        DataBase.Delete(TableName, RowID);
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        DataBase.Delete(Form.CoreObjectID, RowID);
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        DataBase.Delete(Form.CoreObjectID, RowID);
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        break;
                    }
            }

        }

        public static List<Folder> ToFolderNames(string _DataKey)
        {
            List<Folder> FolderList = new List<Folder>();
            List<CoreObject> FolderCoreList= CoreObject.FindChilds(long.Parse(_DataKey),CoreDefine.Entities.پوشه);
            if (HttpContext.Current.Session["FolderNamesOf" + _DataKey] == null)
            {
                if (_DataKey != "0")
                {
                    if (DataFields[_DataKey] == null)
                        StartupSetting(_DataKey);

                    foreach (Field Item in DataFields[_DataKey])
                    {
                        bool IsFind = false;
                        foreach (Folder FolderName in FolderList)
                        {
                            if (Item.Folder ==Tools.Tools.UnSafeTitle( FolderName.FullName))
                            {
                                IsFind = true;
                                break;
                            }
                        }
                        if (!IsFind)
                            if(FolderCoreList.FindIndex(x=>x.FullName==Tools.Tools.SafeTitle(Item.Folder)) >-1)
                            {
                                FolderList.Add(new Folder(FolderCoreList.Find(x => x.FullName == Tools.Tools.SafeTitle(Item.Folder))));
                            }
                            else
                            {
                                FolderList.Add(new Folder() { FullName= Item.Folder });
                            }
                    }
                }
                HttpContext.Current.Session["FolderNamesOf" + _DataKey] = FolderList;
            }
            else
                FolderList = (List<Folder>)HttpContext.Current.Session["FolderNamesOf" + _DataKey];
            return FolderList;
        }

        public static List<TemplateField> TempeletFieldEntryForm(string DataKey, long ParentID, long RecordID, bool FillWithSession = false, bool ISReadOnly = false, long ProcessID = 0, long ProcessStepID = 0)
        {
            List<TemplateField> FieldParameterList = new List<TemplateField>();
            bool DataKey_ShowWithOutPermissionConfig = Desktop.DataKey_ShowWithOutPermissionConfig[DataKey.ToString()]; 
            if (DataKey != "0")
            {
                DataTable RecordData = new DataTable();
                DataTable Data = new DataTable();
                long ExternalField = 0;
                bool isHideExternalField = false;
                CoreObject Form = CoreObject.Find(long.Parse(DataKey));
                Table Table = new Table();

                switch (Form.Entity)
                {
                    case CoreDefine.Entities.جدول:
                        {
                            Table = new Table(Form);
                            break;
                        }
                    case CoreDefine.Entities.فرم_ورود_اطلاعات:
                        {
                            Table = new Table(CoreObject.Find(DataInformationEntryForm[DataKey].RelatedTable));
                            if (RecordID == 0 && ParentID == 0)
                            {
                                if (Form.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات && Form.ParentID != 0)
                                {
                                    ExternalField = DataInformationEntryForm[DataKey].ExternalField;
                                    isHideExternalField = true;
                                }
                            }
                            if(CoreObject.FindChilds(DataInformationEntryForm[DataKey].CoreObjectID,CoreDefine.Entities.فیلد).Count>0)
                                DataKey_ShowWithOutPermissionConfig = true;
                            break;
                        }
                }


                if (RecordID > 0)
                {
                    if (FillWithSession)
                        Data = (DataTable)SessionEditorGrid[DataKey, ParentID.ToString()];
                    else
                        RecordData = SelectTable(Table.CoreObjectID.ToString(), " Where " + Table.IDField().FieldName + "=" + RecordID, 0, 1);
                }

                if (ParentID > 0)
                {
                    ExternalField = DataInformationEntryForm[DataKey].ExternalField;
                }

                string PrimeryKey = Table.IDField().FieldName;

                foreach (Field Item in DataFields[DataKey])
                {
                    PermissionField _PermissionField = new PermissionField(Item.CoreObjectID, Referral.UserAccount.Permition);

                    bool IsEnable = Form.Entity == CoreDefine.Entities.جدول ? !Item.IsVirtual : _PermissionField.CanUpdate && Item.IsEditAble && !Item.IsVirtual;
                    IsEnable= DataKey_ShowWithOutPermissionConfig && !_PermissionField.CanUpdate && Item.IsEditAble && !Item.IsVirtual?true: IsEnable;

                    bool IsHide = !_PermissionField.CanView || (isHideExternalField && Item.CoreObjectID == ExternalField);

                    //IsHide = Form.Entity == CoreDefine.Entities.جدول ? false : IsHide;
                    IsEnable = Form.Entity == CoreDefine.Entities.جدول && Item.IsIdentity ? false : IsEnable;

                    bool IsWide = Item.IsWide;
                    IsHide = Item.ShowInForm ? IsHide : !Item.ShowInForm;
                    string FieldClass = PrimeryKey == Item.FieldName ? " IsFieldID " : "";
                    IsHide = PrimeryKey == Item.FieldName ? false : IsHide;
                    if (Item.ClearAfterChange)
                        FieldClass += Item.ClearAfterChange ? " ClearAfterChange " : "";
                    FieldClass += Item.IsRequired ? " IsRequired " : "";
                    FieldClass += Item.SaveAndNewForm ? " SaveAndNewForm " : "";
                    

                    if (!IsHide || DataKey_ShowWithOutPermissionConfig)
                    {
                        object FieldValue;
                        if (FillWithSession && RecordID > 0)
                        {
                            FieldValue = Data.Select(PrimeryKey + " = " + RecordID)[0][Item.FieldName];
                        }
                        else
                        {
                            FieldValue=null;
                            if (RecordData.Columns.IndexOf(Item.FieldName) > -1 || RecordData.Columns.Count>0)
                            {
                                FieldValue = RecordID > 0 ? (Item.FieldName == PrimeryKey ? RecordID : RecordData.Rows[0][Item.FieldName]) : Tools.Tools.GetDefaultValue(Item.DefaultValue);
                                FieldValue = (RecordID == 0 && ParentID > 0 && Item.CoreObjectID == ExternalField) ? ParentID : FieldValue; 
                            }
                            else
                            {
                                if (RecordID == 0 && ParentID > 0 && Item.CoreObjectID == ExternalField)
                                    FieldValue = ParentID;
                            }
                        }
                        List<SelectListItem> ComboItems = new List<SelectListItem>();
                        if (Item.FieldType == CoreDefine.InputTypes.ComboBox)
                        {
                            string[] ValueItem = Item.SpecialValue.Split('،');
                            foreach (string Value in ValueItem)
                            {
                                ComboItems.Add(new SelectListItem() { Text = Value, Value = Value });
                            }
                        }
                        string FieldName = Item.FieldName + "_" + DataKey;
                        if (Item.FieldType == CoreDefine.InputTypes.Image)
                        {
                            if (RecordData.Rows.Count > 0)
                            {

                                if (RecordData.Columns.IndexOf(Item.FieldName) > -1)
                                {
                                    byte[] FileByte = RecordData.Rows[0][Item.FieldName].ToString() == "" ? (new byte[0]) : (byte[])RecordData.Rows[0][Item.FieldName];
                                    FieldValue = Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte)); 
                                }
                                else
                                    FieldValue = Field.FormatImage(Referral.PublicSetting.AppLogo);

                            }
                            else
                                FieldValue = Field.FormatImage(Referral.PublicSetting.AppLogo);
                        }

                        List<CoreObject> showFieldEvents = new List<CoreObject>();
                        if (Item.CoreObjectID > 0)
                        {
                            showFieldEvents = CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.رویداد_نمایش_فیلد);
                        }

                        string ShowHideElement = "";
                        if (showFieldEvents.Count > 0)
                        {
                            foreach (CoreObject core in showFieldEvents)
                            {
                                //core.FullName
                                ShowFieldEvent ShowFieldItem = new ShowFieldEvent(core);
                                CoreObject SelectedItem = CoreObject.Find(ShowFieldItem.SelectedObjectID); 
                                if (SelectedItem.CoreObjectID > 0)
                                {
                                    if (SelectedItem.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                                    {
                                        switch (Item.FieldType)
                                        {
                                            case Tools.CoreDefine.InputTypes.ComboBox:
                                                {
                                                    ShowHideElement += @"{""FieldValue"":""" + ShowFieldItem.FieldValue.Replace(" ", "_") + @""",""SelectedItem"":""Grid_" + SelectedItem.FullName + "_" + DataKey + @""",""ShowFieldItem"":" + ShowFieldItem.ShowObject.ToString().ToLower() + "};";
                                                    break;
                                                }
                                            default:
                                                {
                                                    ShowHideElement += @"{""FieldValue"":" + ShowFieldItem.FieldValue.Replace(" ", "_") + @",""SelectedItem"":""Grid_" + SelectedItem.FullName + "_" + DataKey + @""",""ShowFieldItem"":" + ShowFieldItem.ShowObject.ToString().ToLower() + "};";
                                                    break;
                                                }
                                        }
                                    }
                                    else if (SelectedItem.Entity == CoreDefine.Entities.فرم_جستجو)
                                    {
                                        ShowHideElement += @"{""FieldValue"":""" + ShowFieldItem.FieldValue.Replace(" ", "_") + @""",""SelectedItem"":""" + ((ParentID > 0 ? "Detail" : "") + "SearchForm_" + SelectedItem.CoreObjectID + "_" + SelectedItem.ParentID.ToString() + "_" + ParentID+"_"+ProcessID.ToString()+"_"+ProcessStepID.ToString()) + @""",""ShowFieldItem"":" + ShowFieldItem.ShowObject.ToString().ToLower() + "};";
                                    }
                                    else if (SelectedItem.Entity == CoreDefine.Entities.دکمه_رویداد_جدول)
                                    {
                                        ShowHideElement += @"{""FieldValue"":""" + ShowFieldItem.FieldValue.Replace(" ", "_") + @""",""SelectedItem"":""" + ((ParentID > 0 ? "Detail" : "") + "TableButton_" +SelectedItem.FullName+"_"+ SelectedItem.CoreObjectID) + @""",""ShowFieldItem"":" + ShowFieldItem.ShowObject.ToString().ToLower() + "};";
                                    }
                                    else
                                    {
                                        ShowHideElement += @"{""FieldValue"":""" + ShowFieldItem.FieldValue.Replace(" ", "_") + @""",""SelectedItem"":""" + SelectedItem.FullName + "_" + DataKey + @""",""ShowFieldItem"":" + ShowFieldItem.ShowObject.ToString().ToLower() + "};";
                                    }
                                }

                            }
                        }

                        if (Item.FieldType == CoreDefine.InputTypes.Password)
                        {
                            //if (Referral.UserAccount.LastDateChangePassword != "")
                            //    FieldValue = Security.Hash.DecryptAes(FieldValue.ToString());
                        }


                        if (ISReadOnly || (!ISReadOnly && !IsEnable))
                            FieldClass += " ISReadOnlyField";

                        if(RecordID==0 && Item.FieldType==CoreDefine.InputTypes.Clock && Item.DefaultValue.Trim()== "@ساعت_سیستم")
                            FieldClass += " IsNowDateTime";


                        FieldParameterList.Add(new TemplateField(IsWide, Item.Folder, FieldName, Item.FieldType, new
                        {
                            FieldName = FieldName,
                            InputType = Item.FieldType,
                            IsReadonly = ISReadOnly == false ? !IsEnable : ISReadOnly,
                            IsRequired = Item.IsRequired,
                            NullValue = "نامشخص",
                            FalseValue = Item.FieldType == CoreDefine.InputTypes.TwoValues ? Item.ComboValues()[0] : "",
                            TrueValue = Item.FieldType == CoreDefine.InputTypes.TwoValues ? Item.ComboValues()[1] : "",
                            IsInCellEditMode = false,
                            FieldTitle = Item.Title(),
                            DigitsAfterDecimal = Item.DigitsAfterDecimal,
                            RelatedField = Item.RelatedField>0?CoreObject.Find(Item.RelatedField).FullName:"",
                            IsGridField = true,
                            IsLeftWrite = Item.IsLeftWrite,
                            RelatedTable = Item.RelatedTable,
                            FieldValue = FieldValue,
                            _TableID = "0",
                            MaxValue = Item.MaxValue,
                            MinValue = Item.MinValue,
                            ComboItems = ComboItems,
                            IsExclusive = Item.IsExclusive,
                            ActiveOnKeyDown = Item.ActiveOnKeyDown,
                            CoreObjectID = Item.CoreObjectID,
                            ShowHideElement = ShowHideElement,
                            FieldComment = Item.FieldComment,
                            TextColor = Item.TextColor,
                            FieldClass = FieldClass,
                            ParentID = ParentID,
                            SearchAutoCompleteCount = Item.SearchAutoCompleteCount,
                            RelatedLink = Item.RelatedLink, 
                    }, Item.ColumnWidth));
                    }
                    else
                    {
                        CoreObject Attachment = CoreObject.Find(Table.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول, Item.FieldName);
                        if (Attachment.CoreObjectID > 0)
                        {
                            FieldParameterList.Add(new TemplateField(IsWide, Item.Folder, Item.FieldName, CoreDefine.InputTypes.Upload, new
                            {
                                FileName = Item.FieldName,
                                FileFullName = Item.FieldName,
                                InputType = CoreDefine.InputTypes.Upload,
                                IsReadonly = ISReadOnly,
                                IsRequired = Item.IsRequired,
                                IsInCellEditMode = false,
                                FieldTitle = Item.Title(),
                                DigitsAfterDecimal = Item.DigitsAfterDecimal,
                                RelatedField = Item.RelatedField,
                                IsGridField = true,
                                IsLeftWrite = Item.IsLeftWrite,
                                RelatedTable = Item.RelatedTable,
                                _TableID = "0",
                                MaxValue = Item.MaxValue,
                                MinValue = Item.MinValue,
                                IsExclusive = Item.IsExclusive,
                                ActiveOnKeyDown = Item.ActiveOnKeyDown,
                                CoreObjectID = Item.CoreObjectID,
                                FieldComment = Item.FieldComment,
                                TextColor = Item.TextColor,
                                FieldClass = FieldClass,
                                ParentID = ParentID,
                                InnerID = RecordID,
                                RecordID = Table.CoreObjectID,
                                TableAttachment = new TableAttachment(Attachment),
                                PermissionTableAttachment = new PermissionTableAttachment(Item.CoreObjectID, Referral.UserAccount.Permition)
                            }, Item.ColumnWidth));
                        }
                    }
                }

            }
            return FieldParameterList;
        }

        public static List<TemplateField> TempeletReportParameter(long ReportID,string[] FieldName=null,object[] FieldValue=null)
        {
            List<TemplateField> ReportParameterList = new List<TemplateField>();
            List<CoreObject> ParameterCoreList = CoreObject.FindChilds(ReportID, CoreDefine.Entities.پارامتر_گزارش);

            foreach (CoreObject Item in ParameterCoreList)
            {
                ReportParameter Parameter = new ReportParameter(Item);

                List<SelectListItem> ComboItems = new List<SelectListItem>();
                if (Parameter.InputTypes == CoreDefine.InputTypes.ComboBox)
                {
                    string[] ValueItem = Parameter.SpecialValue.Split('،');
                    foreach (string Value in ValueItem)
                    {
                        ComboItems.Add(new SelectListItem() { Text = Value, Value = Value });
                    }
                }

                string DefualtValue = Tools.Tools.GetDefaultValue(Parameter.Value);
                if (FieldName!=null)
                {
                    int FindIndex=Array.IndexOf(FieldName, Item.FullName);
                    if(FindIndex!=-1)
                        DefualtValue= FieldValue[FindIndex].ToString() ;
                }

                ReportParameterList.Add(new TemplateField(false, Item.Folder, Item.FullName, Parameter.InputTypes, new
                {
                    FieldName = Item.FullName,
                    InputType = Parameter.InputTypes,
                    IsReadonly = !Parameter.IsEditAble,
                    IsRequired = false,
                    NullValue = "نامشخص",
                    FalseValue = Parameter.InputTypes == CoreDefine.InputTypes.TwoValues ? Parameter.SpecialValue.Split('،')[0] : "",
                    TrueValue = Parameter.InputTypes == CoreDefine.InputTypes.TwoValues ? Parameter.SpecialValue.Split('،')[1] : "", 
                    IsInCellEditMode = false,
                    FieldTitle = Item.Title(),
                    DigitsAfterDecimal = Parameter.DigitsAfterDecimal, 
                    RelatedField = Parameter.RelatedField > 0 ? CoreObject.Find(Parameter.RelatedField).FullName : "",
                    IsGridField = true,
                    IsLeftWrite = Parameter.IsLeftWrite,
                    RelatedTable = Parameter.RelatedTable,
                    FieldValue =DefualtValue,
                    _TableID = Parameter.CoreObjectID.ToString(),
                    MaxValue = float.MaxValue,
                    MinValue = float.MinValue,
                    ComboItems= ComboItems,
                    ActiveOnKeyDown=Parameter.ActiveOnKeyDown
                }));
            }
            return ReportParameterList;
        }

        public static List<TemplateField> RegistryTableParameter(long TableID)
        {
            List<TemplateField> RegistryTableParameterList = new List<TemplateField>();
            List<CoreObject> ParameterCoreList = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد);
            Table table = new Table(CoreObject.Find(TableID));
            string IDField = table.IDField().FieldName;
            foreach (CoreObject Item in ParameterCoreList)
            {
                if (Item.FullName != IDField)
                {
                    Field Parameter = new Field(Item);

                    List<SelectListItem> ComboItems = new List<SelectListItem>();
                    if (Parameter.FieldType == CoreDefine.InputTypes.ComboBox)
                    {
                        string[] ValueItem = Parameter.SpecialValue.Split('،');
                        foreach (string Value in ValueItem)
                        {
                            ComboItems.Add(new SelectListItem() { Text = Value, Value = Value });
                        }
                    }

                    RegistryTableParameterList.Add(new TemplateField(false, "", Item.FullName, Parameter.FieldType, new
                    {
                        FieldName = "RegistryTable_" + Item.FullName,
                        InputType = Parameter.FieldType,
                        IsReadonly = false,
                        IsRequired = Parameter.IsRequired,
                        NullValue = "نامشخص",
                        FalseValue = Parameter.ComboValues()[0],
                        TrueValue = Parameter.ComboValues()[1],
                        IsInCellEditMode = false,
                        FieldTitle = Item.Title(),
                        DigitsAfterDecimal = Parameter.DigitsAfterDecimal,
                        RelatedField = Parameter.RelatedField,
                        IsGridField = true,
                        IsLeftWrite = Parameter.IsLeftWrite,
                        RelatedTable = Parameter.RelatedTable,
                        FieldValue = Tools.Tools.GetDefaultValue(Parameter.DefaultValue),
                        _TableID = "0",
                        MaxValue = Parameter.MaxValue,
                        MinValue = Parameter.MinValue,
                        ComboItems = ComboItems,
                        IsExclusive = Parameter.IsExclusive,
                        ActiveOnKeyDown = Parameter.ActiveOnKeyDown,
                        CoreObjectID = Parameter.CoreObjectID,
                        ShowHideElement = "",
                        FieldComment = Parameter.FieldComment
                    }));

                }
            }
            return RegistryTableParameterList;
        }

        public static List<TemplateField> SearchRegistryTableParameter(long TableID)
        {
            List<TemplateField> RegistryTableParameterList = new List<TemplateField>();
            RegistryTableParameterList.Add(new TemplateField(false, "جستجو", "SearchRegistryTable_UserAccountID", CoreDefine.InputTypes.RelatedTable, new
            {
                FieldName = "SearchRegistryTable_UserAccountID",
                InputType = CoreDefine.InputTypes.RelatedTable,
                IsReadonly = false,
                IsRequired = false,
                NullValue = "نامشخص",
                FalseValue = "خیر",
                TrueValue = "بله",
                IsInCellEditMode = false,
                FieldTitle = "کاربر",
                DigitsAfterDecimal = "0",
                RelatedField = "0",
                IsGridField = false,
                IsLeftWrite = false,
                RelatedTable = "0",
                FieldValue = "0",
                _TableID = TableID.ToString(),
                Query = "Select شناسه , نام_و_نام_خانوادگی From  "+Referral.DBData.ConnectionData.DataBase+ ".dbo.کاربر",
                MaxValue = "0",
                MinValue = "0",
                IsExclusive = false,
                CoreObjectID = 0,
                ShowHideElement = "",
                FieldComment = ""
            }));
            RegistryTableParameterList.Add(new TemplateField(false, "جستجو", "SearchRegistryTable_FromDate", CoreDefine.InputTypes.PersianDateTime, new
            {
                FieldName = "SearchRegistryTable_FromDate",
                InputType = CoreDefine.InputTypes.PersianDateTime,
                IsReadonly = false,
                IsRequired = false,
                NullValue = "نامشخص",
                FalseValue = "خیر",
                TrueValue = "بله",
                IsInCellEditMode = false,
                FieldTitle = "از تاریخ",
                DigitsAfterDecimal = "0",
                RelatedField = "0",
                IsGridField = false,
                IsLeftWrite = false,
                RelatedTable = "0",
                FieldValue = TableID == 0 ? CDateTime.GetNowshamsiDate() : "",
                _TableID = TableID.ToString(),
                Query = "",
                MaxValue = "0",
                MinValue = "0",
                IsExclusive = false,
                CoreObjectID = 0,
                ShowHideElement = "",
                FieldComment = ""
            }));
            RegistryTableParameterList.Add(new TemplateField(false, "جستجو", "SearchRegistryTable_ToDate", CoreDefine.InputTypes.PersianDateTime, new
            {
                FieldName = "SearchRegistryTable_ToDate",
                InputType = CoreDefine.InputTypes.PersianDateTime,
                IsReadonly = false,
                IsRequired = false,
                NullValue = "نامشخص",
                FalseValue = "خیر",
                TrueValue = "بله",
                IsInCellEditMode = false,
                FieldTitle = "تا تاریخ",
                DigitsAfterDecimal = "0",
                RelatedField = "0",
                IsGridField = false,
                IsLeftWrite = false,
                RelatedTable = "0",
                FieldValue = TableID == 0 ? CDateTime.GetNowshamsiDate() : "",
                _TableID = TableID.ToString(),
                Query = "",
                MaxValue = "0",
                MinValue = "0",
                IsExclusive = false,
                CoreObjectID = 0,
                ShowHideElement = "",
                FieldComment = ""
            }));

            return RegistryTableParameterList;
        }

        public static void BeforLoadEditorform(string _DataKey, long ParentID)
        {
            try
            {
                if (_DataKey != "0")
                {
                    CoreObject Form = CoreObject.Find(long.Parse(_DataKey));

                    switch (Form.Entity)
                    {
                        case CoreDefine.Entities.جدول:
                            {
                                break;
                            }
                        case CoreDefine.Entities.فرم_ورود_اطلاعات:
                            {
                                foreach (InformationEntryForm EntryForm in DataInformationEntryForm[_DataKey].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                {
                                    SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()] = null;
                                    Attachment.DeleteDirectory(Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString());

                                    if (DataInformationEntryForm[EntryForm.CoreObjectID.ToString()] == null)
                                        StartupSetting(EntryForm.CoreObjectID.ToString());

                                    foreach (InformationEntryForm Item in DataInformationEntryForm[EntryForm.CoreObjectID.ToString()].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                    {
                                        SessionEditorGrid[Item.CoreObjectID.ToString(), EntryForm.CoreObjectID.ToString()] = null;
                                        Attachment.DeleteDirectory(Attachment.MapTemporaryFilePath + Item.CoreObjectID.ToString());
                                    }
                                }
                                break;
                            }
                    }
                    Attachment.DeleteDirectory(Attachment.MapTemporaryFilePath + _DataKey + "/0");

                }
            }
            catch (Exception ex)
            {
                Security.Log.Error("BeforLoadEditorform", ex.Message);
            }
        }

        public static void SaveSubInformationEntryForm(string _DataKey, long RecordID, long ParentID)
        {
            if (RecordID > 0)
            {
                foreach (InformationEntryForm EntryForm in DataInformationEntryForm[_DataKey].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                {
                    if (SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()] != null)
                    {
                        CoreObject ExternalField = CoreObject.Find(EntryForm.ExternalField);
                        DataTable SubGridData = (DataTable)SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()];
                        string[] ColumnName = new string[SubGridData.Columns.Count];
                        int ColumnCounter = 0;
                        Field IDField = DataTable[_DataKey].IDField();

                        foreach (DataColumn Column in SubGridData.Columns)
                            ColumnName[ColumnCounter++] = Column.ColumnName;

                        foreach (DataRow Row in SubGridData.Rows)
                        {
                            Row[ExternalField.FullName] = RecordID;
                            long NewRowID = Create(EntryForm.CoreObjectID.ToString(), ColumnName, Row.ItemArray);
                            if (NewRowID > 0)
                            {
                                string SourcePath = Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString() + "/" + Row[IDField.FieldName];
                                string DestinationPath = Attachment.MapFileSavingAttachmentPath + EntryForm.RelatedTable + "/" + NewRowID;
                                Attachment.SaveAttachment(new DirectoryInfo(SourcePath), Directory.CreateDirectory(DestinationPath), EntryForm.RelatedTable, NewRowID);
                            }
                        }
                    }
                    Attachment.DeleteDirectory(Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString());
                }

                string PSourcePath = Attachment.MapTemporaryFilePath + _DataKey + "/0";
                string PDestinationPath = Attachment.MapFileSavingAttachmentPath + DataInformationEntryForm[_DataKey].RelatedTable + "/" + RecordID;
                Attachment.SaveAttachment(new DirectoryInfo(PSourcePath), Directory.CreateDirectory(PDestinationPath), DataInformationEntryForm[_DataKey].RelatedTable, RecordID);
                Attachment.DeleteDirectory(Attachment.MapTemporaryFilePath + _DataKey);
            }
        }

        public static DataTable GetNotificationData()
        {
            string Query = "SELECT  شناسه," +
                "\n مراحل_فرآیند," +
                "\n دریافت_کننده," +
                "\n مشاهده_شده," +
                "\n تاریخ_مشاهده," +
                "\n ساعت_مشاهده," +
                "\n تاریخ_مهلت_پاسخگویی," +
                "\n دستور_ارجاع," +
                "\n سمت_سازمانی_دریافت_کننده," +
                "\n تاریخ_ثبت," +
                "\n ساعت_ثبت," +
                "\n ثبت_کننده," +
                "\n فرآیند," +
                "\n مرحله_فرآیند," +
                "\n فرم_ورود_اطلاعات," +
                "\n رکورد," +
                "\n جدول," +
                "\n نام_ثبت_کننده," +
                "\n عنوان_سمت_سازمانی_دریافت_کننده," +
                "\n سمت_سازمانی_ثبت_کننده," +
                "\n عنوان_سمت_سازمانی_ثبت_کننده," +
                "\n شناسه_مافوق" +
                "\nFROM ارجاع_مراحل_فرآیند" + 
                "\n  where\n دریافت_کننده = " + Referral.UserAccount.UsersID + " \nand مشاهده_شده = 0  " +
                "\norder by\n تاریخ_ثبت desc ,\n ساعت_ثبت desc";
            DataTable NotifiData = Referral.DBData.SelectDataTable(Query);
            string PersonnelTableID = CoreObject.Find(Referral.MasterDatabaseID,CoreDefine.Entities.جدول, "کاربر").CoreObjectID.ToString();
            foreach (DataRow row in NotifiData.Rows)
            {
                if (HttpContext.Current.Session["PersonnelPhoto" + row["ثبت_کننده"]] == null)
                    HttpContext.Current.Session["PersonnelPhoto" + row["ثبت_کننده"]] = Attachment.GetFileByte(PersonnelTableID, row["ثبت_کننده"].ToString(), "چهره");
            }

            HttpContext.Current.Session["NotifiDataCount"] = NotifiData.Rows.Count;
            return NotifiData;
            //return new DataTable();
        }

        public static DataTable GetDashboardData(string StartData, string EndData, SubDashboard subDashboard, string[] SearchFieldItem = null, string[] SearchFieldOperator = null, string[] SearchFieldValue = null)
        {
            string Query = "declare @از_تاریخ as Nvarchar(255) = N'" + Tools.Tools.GetDefaultValue(StartData) + "'\n" +
                           "declare @تا_تاریخ  as Nvarchar(255) = N'" + Tools.Tools.GetDefaultValue(EndData) + "'\n";

            CoreObject Dashboardcore = CoreObject.Find(subDashboard.ParentID);
            List<CoreObject> SearchFieldList = CoreObject.FindChilds(subDashboard.ParentID, CoreDefine.Entities.فیلد_جستجو);

            string SearchQuery=string.Empty;
            if (SearchFieldItem != null)
            {

                for (int Index=0; Index<SearchFieldItem.Length; Index++)
                {
                    if (SearchFieldList.Find(x=>x.FullName== SearchFieldItem[Index]).CoreObjectID>0)
                    {

                        foreach (CoreObject FieldItem in SearchFieldList)
                        {
                            if(FieldItem.FullName== SearchFieldItem[Index])
                            { 
                                APM.Models.APMObject.InformationForm.SearchField SearchField = new APM.Models.APMObject.InformationForm.SearchField(FieldItem);
                                CoreObject FieldCore=CoreObject.Find(SearchField.RelatedField);
                                CoreObject TableCoreItem = CoreObject.Find(FieldCore.ParentID);
                                Field SearchFieldCore = new Field(FieldCore);
                                SearchQuery += "\n@"+ SearchFieldItem[Index];
                                //SearchQuery += "\n" + TableCoreItem.FullName+"."+ SearchFieldItem[Index];
                               Query += $"declare @{SearchFieldItem[Index]} as {SearchFieldCore.FieldNature} = {SearchFieldValue[Index]}\n";
                            }
                        }
                    }

                    switch (SearchFieldOperator[Index])
                    {
                        case "نامساوی": SearchQuery += "<> " ; break;
                        case "بزرگتر یا مساوی": SearchQuery += ">= "  ; break;
                        case "بزرگتر": SearchQuery += "> " ; break;
                        case "کوچکتر یا مساوی": SearchQuery += "<= "  ; break;
                        case "کوچکتر": SearchQuery += "< " ; break;
                        case "تهی": SearchQuery += "IS Null"; break;
                        case "تهی نیست": SearchQuery += "Is Not Null"; break;
                        default: SearchQuery += " = " ; break;
                    }
                    SearchQuery += SearchFieldValue[Index];

                    if (Index < SearchFieldItem.Length - 1)
                        SearchQuery += " And ";
                }
            }
            else
            {
                foreach (CoreObject FieldItem in SearchFieldList)
                { 
                    APM.Models.APMObject.InformationForm.SearchField SearchField = new APM.Models.APMObject.InformationForm.SearchField(FieldItem);
                    CoreObject FieldCore = CoreObject.Find(SearchField.RelatedField); 
                    Field SearchFieldCore = new Field(FieldCore); 
                    Query += $"declare @{FieldItem.FullName} as {SearchFieldCore.FieldNature} = {Tools.Tools.GetDefaultValue(SearchField.DefaultValue)}\n"; 
                }
            }

            HttpContext.Current.Session["GroupDashboardID" + subDashboard.CoreObjectID.ToString()] = "";
            HttpContext.Current.Session["OrderDashboardID" + subDashboard.CoreObjectID.ToString()] = "";
            HttpContext.Current.Session["WhereDashboardID" + subDashboard.CoreObjectID.ToString()] = "";

            string WhereQuery = HttpContext.Current.Session["WhereDashboardID" + subDashboard.ParentID.ToString()] == null ? "" : HttpContext.Current.Session["WhereDashboardID" + subDashboard.ParentID.ToString()].ToString();
            CoreObject GroupFieldCore = CoreObject.Find(subDashboard.GroupField);
            CoreObject TableCore = CoreObject.Find(GroupFieldCore.ParentID);
            Field GroupField = GroupFieldCore.CoreObjectID == 0 ? new Field() : new Field(GroupFieldCore);
            DataTable DashboardData = new DataTable();
            CoreObject DateFieldCore = CoreObject.Find(subDashboard.DateField);
            CoreObject TableDateFieldCore = CoreObject.Find(DateFieldCore.ParentID);

            if (string.IsNullOrEmpty(subDashboard.CategoryAxisQuery))
            {
                Query += "Select \n";
                if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.هیچکدام)
                {
                    switch (GroupField.FieldType)
                    {
                        case CoreDefine.InputTypes.TwoValues:
                            {
                                Query += "case when(" + GroupField.FieldName + "=0) then N'" + GroupField.ComboValues()[0] + "' when(" + GroupField.FieldName + "=1) then N'" + GroupField.ComboValues()[1] + "' else N'نامشخص' end ";
                                break;
                            }
                        case CoreDefine.InputTypes.RelatedTable:
                            {
                                Query += DataConvertor.GetRelatedTableQueryForDashboard(GroupField);
                                break;
                            }
                        default:
                            {
                                Query += GroupField.FieldName;
                                break;
                            }
                    }

                }
                else
                {
                    switch (subDashboard.ChartGroupDateType)
                    {
                        case CoreDefine.ChartGroupDate.سال:
                            {
                                Query += "SUBSTRING(" + DateFieldCore.FullName + ",1,4)";
                                break;
                            }
                        case CoreDefine.ChartGroupDate.ماه:
                            {
                                Query += "SUBSTRING(" + DateFieldCore.FullName + ",1,7)";
                                break;
                            }
                        case CoreDefine.ChartGroupDate.روز:
                            {
                                Query += DateFieldCore.FullName;
                                break;
                            }
                    }
                }

                switch (subDashboard.ChartCalculationType)
                {
                    case CoreDefine.ChartCalculationType.مجموع:
                        {
                            if(subDashboard.CalculationField>0)
                            {
                                CoreObject CalculationFieldCore = CoreObject.Find(subDashboard.CalculationField);
                                CoreObject CalculationFieldTableCore = CoreObject.Find(CalculationFieldCore.ParentID);
                                Query += "\n ,Sum(["+ CalculationFieldTableCore.FullName+"].[" + CalculationFieldCore.FullName + "]) ";
                            }
                            else
                              Query += "\n ,Sum(1) ";
                            break;
                        }
                    default:
                        {
                            Query += "\n ,COUNT(1) ";
                            break;
                        }
                }
                Query += "\nFrom " + TableCore.FullName + " \n";
                Query += "where 1=1 And \n";
                if (!string.IsNullOrEmpty(SearchQuery))
                    Query += SearchQuery + " And\n";
                Query += subDashboard.DateField > 0 ? DateFieldCore.FullName + "  between @از_تاریخ and @تا_تاریخ" : "";
                Query += subDashboard.Condition != "" ? " And (" + subDashboard.Condition + ")\n" : "";

                if (WhereQuery != "")
                {
                    Query += "\n And (" + WhereQuery + ")";
                }

            }
            else
            {
                Query += subDashboard.CategoryAxisQuery;
                Query += "\n\nwhere 1=1 And \n";

                if (!string.IsNullOrEmpty(SearchQuery))
                    Query += SearchQuery + " And\n";

                Query += subDashboard.DateField > 0 ? "["+ TableDateFieldCore.FullName+"].["+ DateFieldCore.FullName + "]  between @از_تاریخ and @تا_تاریخ" : "";
                Query += subDashboard.DateField > 0 && subDashboard.Condition != "" ? "\n And " : "";
                Query += subDashboard.Condition != "" ? "\n (" + subDashboard.Condition + ")\n" : "";
                if (WhereQuery != "")
                {
                    Query += "\n And (" + WhereQuery + ")";
                }
                Query=Tools.Tools.CheckQuery(Query);
            }
            
            Query += "\n\ngroup by " + GroupField.FieldName;

            if (!string.IsNullOrEmpty(subDashboard.GroupByQuery))
                Query += "," + Tools.Tools.ConvertToSQLQuery( subDashboard.GroupByQuery);


                switch (subDashboard.ChartGroupDateType)
                {
                    case CoreDefine.ChartGroupDate.سال:
                        {
                            Query += ",SUBSTRING(" + DateFieldCore.FullName + ",1,4)";
                            break;
                        }
                    case CoreDefine.ChartGroupDate.ماه:
                        {
                            Query += ",SUBSTRING(" + DateFieldCore.FullName + ",1,7)";
                            break;
                        }
                    case CoreDefine.ChartGroupDate.روز:
                        {
                            Query += "," + DateFieldCore.FullName;
                            break;
                        }
                }

                if (!string.IsNullOrEmpty(subDashboard.OrderByQuery))
                    Query += "\norder by " + Tools.Tools.ConvertToSQLQuery(subDashboard.OrderByQuery);
                else
                    Query += "\norder by " + GroupField.FieldName;


                switch (subDashboard.ChartGroupDateType)
                {
                    case CoreDefine.ChartGroupDate.سال:
                        {
                            Query += ",SUBSTRING(" + DateFieldCore.FullName + ",1,4)";
                            break;
                        }
                    case CoreDefine.ChartGroupDate.ماه:
                        {
                            Query += ",SUBSTRING(" + DateFieldCore.FullName + ",1,7)";
                            break;
                        }
                    case CoreDefine.ChartGroupDate.روز:
                        {
                            Query += "," + DateFieldCore.FullName;
                            break;
                        }
                }


            if (Referral.MasterDatabaseID == TableCore.ParentID)
            {
                DashboardData = Referral.DBData.SelectDataTable(Query);
            }
            else
            {
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {  
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            DashboardData = DataBase.SelectDataTable(Query);
                            break;
                        }
                    case CoreDefine.DataSourceType.MySql:
                        {
                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                            DashboardData = DataBase.SelectDataTable(Query);
                            break;
                        }
                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                            DashboardData = DataBase.SelectDataTable(Query);
                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            using (ExcelEngine excelEngine = new ExcelEngine())
                            {
                                IApplication application = excelEngine.Excel;

                                application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                                IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                                //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                                //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                            }
                            break;
                        } 
                    default: break;
                }
            }
            return DashboardData;
        }

        public static bool ExecuteQuery(DataSourceInfo DataSourceInfo, string Query)
        {
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        if(Referral.DBData.ConnectionData.Source== DataSourceInfo.ServerName && Referral.DBData.ConnectionData.DataBase == DataSourceInfo.DataBase)
                            return Referral.DBData.Execute(Query);
                        
                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                        return DataBase.Execute(Query);
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        return DataBase.Execute(Query);
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        return DataBase.Execute(Query);
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        return true;
                    }
                default: return false;
            }
        }
        public static object SelectField(DataSourceInfo DataSourceInfo, string Query)
        {
            switch (DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        if (DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                            return Referral.DBData.SelectField(Query);
                        else
                        {
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            return DataBase.SelectField(Query);
                        }
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        return DataBase.SelectField(Query);
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        return DataBase.SelectField(Query);
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            //IWorksheet worksheet = workbook.Worksheets[Table.FullName.Replace("$", "").Replace("'", "")];

                            //Output = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        return true;
                    }
                default: return false;
            }
        }

        public static object FinalSaveFormEditor(string DataKey, string ParentID, long RecordID, string[] FormInputName, object[] FormInputValue, long SearchDataKey = 0, bool SaveChilde = true, long ProcessID = 0, long ProcessStepID = 0)
        {

            if (Referral.CoreObjects.Count == 0)
                Software.CoreReload();
            CoreObject Form = CoreObject.Find(long.Parse(DataKey));
            string Alarm = "";

            if (FormInputName != null)
                for (int Index = 0; Index < FormInputName.Length; Index++)
                    FormInputName[Index] = FormInputName[Index].Replace("_" + DataKey, "");

            if (RecordID == 0)
            {
                switch (Form.Entity)
                {
                    case CoreDefine.Entities.جدول:
                        {
                            Alarm = Desktop.CheckBeforRunQuery(Form.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, FormInputName, FormInputValue);
                            if (Alarm != "")
                                return  new { Message = Alarm, Record = "" };

                            long ParentRowID = Desktop.Create(DataKey, FormInputName, FormInputValue);
                            if (ParentRowID > 0)
                            {
                                string PSourcePath = "";
                                string PDestinationPath = "";
                                if (Referral.PublicSetting.FileSavingPath == "")
                                {

                                }
                                else
                                {
                                    PSourcePath = Models.Attachment.MapTemporaryFilePath + DataKey + "/0";
                                    PDestinationPath = Models.Attachment.MapFileSavingAttachmentPath + DataKey + "/" + ParentRowID;
                                    Models.Attachment.SaveAttachment(new DirectoryInfo(PSourcePath), Directory.CreateDirectory(PDestinationPath), Form.CoreObjectID, ParentRowID);
                                    Models.Attachment.DeleteDirectory(Models.Attachment.MapTemporaryFilePath + DataKey);
                                }
                            }
                            DataTable dataTable = Desktop.SelectRecord(DataKey, ParentRowID);
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                int Findindex = Array.IndexOf(FormInputName, column.ColumnName);
                                if (Findindex > -1)
                                    FormInputValue[Findindex] = dataTable.Rows[0][column.ColumnName].ToString();
                            }
                            RecordID = ParentRowID;
                            break;
                        }
                    case CoreDefine.Entities.فرم_ورود_اطلاعات:
                        {
                            InformationEntryForm informationEntryForm = new InformationEntryForm(Form);

                            CoreObject TableCore = CoreObject.Find(informationEntryForm.RelatedTable);
                            SearchForm searchForm = new SearchForm();
                            string[] SelectedColumn = new string[0];
                            if (SearchDataKey > 0)
                            {
                                searchForm = new SearchForm(CoreObject.Find(SearchDataKey));
                                SelectedColumn = searchForm.SelectedColumns.Split('،');
                            }

                            if (Form.ParentID != 0 && ParentID == "0")
                            {
                                DataTable Table = (DataTable)Desktop.SessionEditorGrid[DataKey, ParentID];
                                if (Table != null)
                                {
                                    DataRow Row = Table.NewRow();
                                    Field IDField = Desktop.DataTable[DataKey].IDField();
                                    int MaxID = Table.Compute("max([" + IDField.FieldName + "])", string.Empty).ToString() == "" ? 0 : Convert.ToInt32(Table.Compute("max([" + IDField.FieldName + "])", string.Empty).ToString());
                                    Row[IDField.FieldName] = ++MaxID;


                                    //CoreObject _ExternalField = CoreObject.Find(informationEntryForm.ExternalField);
                                    //int FindIndex = Array.IndexOf(FormInputName, _ExternalField.FullName);
                                    //if (FindIndex > -1)
                                    //{ 
                                    //    FormInputValue[FindIndex] = ParentID; 
                                    //}
                                    //else
                                    //{
                                    //    if (!string.IsNullOrEmpty(ParentID) && ParentID != "0")
                                    //    {
                                    //        Array.Resize(ref FormInputValue, FormInputValue.Length + 1);
                                    //        Array.Resize(ref FormInputName, FormInputName.Length + 1);
                                    //        FormInputName[FormInputName.Length - 1] = _ExternalField.FullName;
                                    //        FormInputValue[FormInputValue.Length - 1] = ParentID;
                                    //    }
                                    //}


                                    Alarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, FormInputName, FormInputValue);
                                    if (Alarm != "")
                                        return new { Message = Alarm, Record = "" };

                                    Table.Rows.Add(Row);

                                    if (FormInputName != null)
                                    {
                                        if (SearchDataKey > 0)
                                        {
                                            foreach (string Item in SelectedColumn)
                                            {
                                                string[] ItemArr = Item.Replace("{", "").Replace("}", "").Replace(" ", "").Split(':');
                                                int index = Array.IndexOf(FormInputName, ItemArr[1]);
                                                if (index > -1)
                                                    if (Table.Columns[ItemArr[0]] != null)
                                                        if (Table.Columns[ItemArr[0]].DataType.Name == "Double")
                                                            Table.Rows[Table.Rows.Count - 1][ItemArr[0]] = FormInputValue[index].ToString() == "null" ? 0 : double.Parse(FormInputValue[index].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                                        else
                                                            Table.Rows[Table.Rows.Count - 1][ItemArr[0]] = FormInputValue[index];
                                            }
                                        }
                                        else
                                        {
                                            List<Field> TableFields = new List<Field>();

                                            if (Desktop.DataFields[DataKey] == null)
                                                Desktop.StartupSetting(DataKey);

                                            foreach (Field Item in Desktop.DataFields[DataKey])
                                                if (!Item.IsIdentity && !Item.IsVirtual)
                                                    TableFields.Add(Item);


                                            string IsFixedItem = "";
                                            //CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

                                            foreach (Field Item in TableFields)
                                            {
                                                try
                                                {
                                                    int index = Array.IndexOf(FormInputName, Item.FieldName);
                                                    if (index > -1)
                                                    {
                                                        if (Table.Columns[Item.FieldName].DataType.Name == "Double")
                                                            Table.Rows[Table.Rows.Count - 1][Item.FieldName] = (FormInputValue[index].ToString() == "null" || FormInputValue[index].ToString() == "") ? 0 : double.Parse(FormInputValue[index].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                                        else if (Table.Columns[Item.FieldName].DataType.Name == "Int64")
                                                            Table.Rows[Table.Rows.Count - 1][Item.FieldName] = (FormInputValue[index].ToString() == "null" || FormInputValue[index].ToString() == "") ? 0 : long.Parse(FormInputValue[index].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                                        else if (Table.Columns[Item.FieldName].DataType.Name == "Boolean")
                                                            Table.Rows[Table.Rows.Count - 1][Item.FieldName] = (FormInputValue[index].ToString() == "null" || FormInputValue[index].ToString() == "") ? 0 : FormInputValue[index];
                                                        else
                                                            Table.Rows[Table.Rows.Count - 1][Item.FieldName] = FormInputValue[index];
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                        }
                                    }

                                    Desktop.SessionEditorGrid[DataKey, ParentID] = Table;


                                    string SourcePath = Models.Attachment.MapTemporaryFilePath + DataKey + "/0";
                                    string DestinationPath = Models.Attachment.MapTemporaryFilePath + DataKey + "/" + MaxID;
                                    if (APM.Models.Attachment.CheckExistsDirectory(DestinationPath))
                                        APM.Models.Attachment.DeleteDirectory(DestinationPath);
                                    Models.Attachment.RenameDirectory(new DirectoryInfo(SourcePath), new DirectoryInfo(DestinationPath));
                                }

                            }
                            else
                            {

                                if (SearchDataKey > 0)
                                {
                                    int FieldCount = Desktop.DataFields[DataKey].Count;
                                    string[] TFormInputName = new string[FieldCount];
                                    object[] TFormInputValue = new object[FieldCount];

                                    for (int index = 0; index < FieldCount; index++)
                                    {
                                        TFormInputName[index] = Desktop.DataFields[DataKey][index].FieldName;
                                        TFormInputValue[index] = Desktop.DataFields[DataKey][index].DefaultValue;
                                    }

                                    if (Form.ParentID != 0)
                                    {
                                        CoreObject ExternalFieldCore = CoreObject.Find(informationEntryForm.ExternalField);
                                        int FindeIndex = Array.IndexOf(TFormInputName, ExternalFieldCore.FullName);
                                        TFormInputValue[FindeIndex] = ParentID;
                                    }

                                    foreach (string Item in SelectedColumn)
                                    {
                                        string[] ItemArr = Item.Replace("{", "").Replace("}", "").Replace(" ", "").Split(':');
                                        int index = Array.IndexOf(FormInputName, ItemArr[1]);
                                        int FindeIndex = Array.IndexOf(TFormInputName, ItemArr[0]);
                                        TFormInputValue[FindeIndex] = FormInputValue[index];
                                    }

                                    FormInputName = TFormInputName;
                                    FormInputValue = TFormInputValue;
                                }

                                //if (!string.IsNullOrEmpty(ParentID) && ParentID != "0" && !SaveChilde)
                                //{
                                CoreObject _ExternalField = CoreObject.Find(informationEntryForm.ExternalField);
                                int FindIndex = Array.IndexOf(FormInputName, _ExternalField.FullName);
                                if (FindIndex > -1)
                                {
                                    //if (FormInputValue[FindIndex] == "" || FormInputValue[FindIndex] == "null" || FormInputValue[FindIndex] == "0")
                                    //    FormInputValue[FindIndex] = ParentID;
                                    //else
                                    FormInputValue[FindIndex] = ParentID;

                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(ParentID) && ParentID != "0")
                                    {
                                        Array.Resize(ref FormInputValue, FormInputValue.Length + 1);
                                        Array.Resize(ref FormInputName, FormInputName.Length + 1);
                                        FormInputName[FormInputName.Length - 1] = _ExternalField.FullName;
                                        FormInputValue[FormInputValue.Length - 1] = ParentID;
                                    }
                                }

                                //}

                                string IsFixedItem = "";
                                //CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

                                Alarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, FormInputName, FormInputValue, DataKey);
                                if (Alarm != "")
                                    return new { Message = Alarm, Record = "" };

                                long ParentRowID = Desktop.Create(DataKey, FormInputName, FormInputValue);
                                if (ParentRowID > 0)
                                {
                                    if (SaveChilde)
                                        foreach (InformationEntryForm EntryForm in Desktop.DataInformationEntryForm[DataKey].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                        {
                                            if (Desktop.SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()] != null)
                                            {
                                                DataTable SubGridData = (DataTable)Desktop.SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()];
                                                if (SubGridData.Rows.Count > 0)
                                                {
                                                    CoreObject ExternalField = CoreObject.Find(EntryForm.ExternalField);
                                                    string[] ColumnName = new string[SubGridData.Columns.Count];
                                                    int ColumnCounter = 0;
                                                    Field IDField = Desktop.DataTable[DataKey].IDField();


                                                    TableCore = CoreObject.Find(EntryForm.RelatedTable);

                                                    foreach (DataColumn Column in SubGridData.Columns)
                                                        ColumnName[ColumnCounter++] = Column.ColumnName;

                                                    foreach (DataRow Row in SubGridData.Rows)
                                                    {
                                                        Row[ExternalField.FullName] = ParentRowID;

                                                        Alarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, ColumnName, Row.ItemArray);
                                                        if (Alarm == "")
                                                        {
                                                            long NewRowID = Desktop.Create(EntryForm.CoreObjectID.ToString(), ColumnName, Row.ItemArray);
                                                            if (NewRowID > 0)
                                                            {
                                                                if (Referral.PublicSetting.FileSavingPath == "")
                                                                {

                                                                }
                                                                else
                                                                {
                                                                    string SourcePath = Models.Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString() + "/" + Row[IDField.FieldName];
                                                                    string DestinationPath = Models.Attachment.MapFileSavingAttachmentPath + EntryForm.RelatedTable + "/" + NewRowID;
                                                                    Models.Attachment.SaveAttachment(new DirectoryInfo(SourcePath), Directory.CreateDirectory(DestinationPath), EntryForm.RelatedTable, NewRowID);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            Models.Attachment.DeleteDirectory(Models.Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString());

                                        }
                                    RecordID = ParentRowID;

                                    if (ProcessID > 0 && ProcessStepID > 0)
                                        Desktop.SaveProcessStep(ProcessID, ProcessStepID, TableCore.CoreObjectID, RecordID, long.Parse(DataKey));
                                }
                                DataTable dataTable = Desktop.SelectRecord(DataKey, ParentRowID);
                                if (dataTable.Rows.Count > 0)
                                {
                                    List<CoreObject> AttachmentCore = CoreObject.FindChilds(informationEntryForm.RelatedTable, CoreDefine.Entities.ضمیمه_جدول);
                                    for (int i = 0; i < AttachmentCore.Count; i++)
                                    {
                                        int Findindex = Array.IndexOf(FormInputName, AttachmentCore[i].FullName.ToString());
                                        if (Findindex != -1)
                                            FormInputValue[Findindex] = Attachment.GetFileByte(informationEntryForm.RelatedTable.ToString(), ParentRowID.ToString(), AttachmentCore[i].FullName.ToString());
                                    }

                                    foreach (DataColumn column in dataTable.Columns)
                                    {
                                        int Findindex = Array.IndexOf(FormInputName, column.ColumnName);
                                        if (Findindex > -1)
                                            FormInputValue[Findindex] = dataTable.Rows[0][column.ColumnName].ToString();
                                        else
                                        {
                                            Array.Resize(ref FormInputName, FormInputName.Length + 1);
                                            Array.Resize(ref FormInputValue, FormInputValue.Length + 1);
                                            FormInputName[FormInputName.Length - 1] = column.ColumnName;
                                            FormInputValue[FormInputValue.Length - 1] = dataTable.Rows[0][column.ColumnName].ToString();
                                        }
                                    }

                                }

                            }
                            break;
                        }
                }
            }
            else
            {
                if ((Form.ParentID != 0 && ParentID == "0") && Form.Entity != CoreDefine.Entities.جدول)
                {
                    List<Field> TableFields = new List<Field>();
                    DataTable Table = (DataTable)Desktop.SessionEditorGrid[DataKey, ParentID];
                    Field IDField = Desktop.DataTable[DataKey].IDField();
                    int RowIndex = 0;

                    for (int Index = 0; Index < Table.Rows.Count; Index++)
                        if (long.Parse(Table.Rows[Index][IDField.FieldName].ToString()) == RecordID)
                        {
                            RowIndex = Index;
                            break;
                        }


                    if (Desktop.DataFields[DataKey] == null)
                        Desktop.StartupSetting(DataKey);

                    foreach (Field Item in Desktop.DataFields[DataKey])
                        if (!Item.IsIdentity && !Item.IsVirtual)
                            TableFields.Add(Item);

                    foreach (Field Item in TableFields)
                    {
                        int index = Array.IndexOf(FormInputName, Item.FieldName);
                        if (index > -1)
                        {
                            if (Table.Columns[Item.FieldName].DataType.Name == "Double")
                                Table.Rows[RowIndex][Item.FieldName] = FormInputValue[index].ToString() == "null" || FormInputValue[index].ToString() == "" ? 0 : double.Parse(FormInputValue[index].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            else if (Table.Columns[Item.FieldName].DataType.Name == "Int64")
                                Table.Rows[RowIndex][Item.FieldName] = FormInputValue[index].ToString() == "null" || FormInputValue[index].ToString() == "" ? 0 : FormInputValue[index];
                            else if (Table.Columns[Item.FieldName].DataType.Name == "Boolean")
                                Table.Rows[RowIndex][Item.FieldName] = FormInputValue[index].ToString() == "" ? false : (FormInputValue[index] == "1" ? true : (FormInputValue[index].ToString().ToLower() == "true" ? true : false));
                            else
                                Table.Rows[RowIndex][Item.FieldName] = FormInputValue[index];
                        }
                    }
                    Desktop.SessionEditorGrid[DataKey, ParentID] = Table;
                }
                else
                {
                    long TableID = Form.CoreObjectID;
                    if (Form.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                    {
                        InformationEntryForm InformationEntryForm = new InformationEntryForm(Form);
                        TableID = InformationEntryForm.RelatedTable;
                    }
                    Alarm = Desktop.CheckBeforRunQuery(TableID, RecordID, CoreDefine.TableEvents.شرط_اجرای_ویرایش, FormInputName, FormInputValue);
                    if (Alarm != "")
                        return new { Message = Alarm, Record = "" };

                    bool result = Desktop.Update(TableID, RecordID, FormInputName, FormInputValue);

                    if (ProcessID > 0 && ProcessStepID > 0)
                        Desktop.SaveProcessStep(ProcessID, ProcessStepID, TableID, RecordID, long.Parse(DataKey));
                }
            }
            return new { Message = Alarm, Record = FormInputValue, RecordID = RecordID };
        }

        public static bool ShowButtonMainGridTools(CoreDefine.ButtonType _ButtonType,string _DataKey)
        {
            if(_ButtonType == CoreDefine.ButtonType.جدید)
            {
                if (!string.IsNullOrEmpty(DataInformationEntryForm[_DataKey].NewButtonVisibleConditionQuery))
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find( CoreObject.Find(DataInformationEntryForm[_DataKey].RelatedTable).ParentID));
                    object Result= SelectField(dataSourceInfo, Tools.Tools.CheckQuery( DataInformationEntryForm[_DataKey].NewButtonVisibleConditionQuery));
                    try
                    {
                        if ((int)Result == 1)
                            return true; 
                    }
                    catch (Exception ex)
                    {
                        return false;   
                    }
                }
            }

            return true;

        }

        public static int GetColumnWidth(string ColumnWidth)
        {
            ColumnWidth = ColumnWidth.Replace("-", "/");
            return int.Parse((float.Parse(ColumnWidth) * 100).ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}