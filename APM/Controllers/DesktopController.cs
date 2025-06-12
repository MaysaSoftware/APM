using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json.Linq;
using APM.Models;
using APM.Models.Database;
using APM.Models.Tools;
using APM.Models.APMObject;
using APM.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Kendo.Mvc;
using System.Net.Http; 
using System.Text;
using APM.Models.NetWork;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System.Net;
using APM.Models.Diagram;
using System.Threading.Tasks;
using APM.Models.APMObject.InformationForm;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace APM.Controllers
{
    public class DesktopController : Controller
    {
        // GET: Desktop  
        public ActionResult Index(string Form, string ParentID = "0", long ProcessID = 0)
        { 
            if (!UserAuthorization.CanUserVisit()) return Redirect(UserAuthorization.ExclusionURL());
 
            if (ProcessID > 0)
            {
                ProcessType processType = new ProcessType(CoreObject.Find(ProcessID));
                Form = processType.InformationEntryFormID.ToString();
            }

            Desktop.StartupSetting(Form);
            Session["MasterDataKey"] = Form;
            ViewData["MasterDataKey"] = Form;
            ViewData["MasterParentID"] = ParentID;
            ViewData["MasterProcessID"] = ProcessID;
            ViewData["RecordID"] = "0";
            ViewData["ProcessStep"] = 0;
            ViewData["IsDetailGrid"] = false; 
            return View("Index");
        }

        public ActionResult ReadFilter(string _DataKey, int _ParentID, long _MasterProcessID, long _ProcessStep, long? RecordID, [DataSourceRequest] DataSourceRequest _Request, string SubDashboardID = "", string Category = "", int _ShowRowCount = 0)
        {
            JsonResult jsonResult = new JsonResult();
            string Where = _MasterProcessID == 0 ? "" : " Where نوع_فرآیند = " + _MasterProcessID;
            string TableName = "";
            CoreObject Form = CoreObject.Find(long.Parse(_DataKey));

            switch (Form.Entity)
            {
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        if (Desktop.DataInformationEntryForm[_DataKey] == null)
                            Desktop.StartupSetting(_DataKey);

                        TableName = CoreObject.Find(Desktop.DataInformationEntryForm[_DataKey].RelatedTable).FullName;

                        if (Form.ParentID != 0 && RecordID == 0 && _ParentID == 0)
                        {
                            if (Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] == null)
                            {
                                //DataTable Table = Desktop.Read(_DataKey, Where, _ParentID);
                                DataTable Table = Desktop.CachedTable[_DataKey];
                                if (_ProcessStep > 0)
                                {
                                    Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = Table;
                                    if (Desktop.DataInformationEntryForm[_DataKey].InformationEntryFormName == "ارجاع" && RecordID == 0)
                                    {
                                        List<CoreObject> ReferralStepList = CoreObject.FindChilds(_ProcessStep, CoreDefine.Entities.ارجاع_مرحله_فرآیند);
                                        int Counter = 0;
                                        foreach (CoreObject Item in ReferralStepList)
                                        {
                                            ProcessReferral ProcessReferral = new ProcessReferral(Item);
                                            DataRow Row = Table.NewRow();
                                            Row["شناسه"] = ++Counter;
                                            Row["دریافت_کننده"] = ProcessReferral.Personnel;
                                            Row["سمت_سازمانی"] = ProcessReferral.OrganizationLevel;
                                            Row["نوع_ارجاع"] = ProcessReferral.ReferralType;
                                            Row["تاریخ_مهلت_پاسخگویی"] = CDateTime.AddDay(CDateTime.GetNowshamsiDate(), Convert.ToInt16(ProcessReferral.ResponseDeadline));
                                            Row["دستور_ارجاع"] = ProcessReferral.Comment;
                                            Row["نوع_اطلاع_رسانی"] = ProcessReferral.NotificationType;
                                            Table.Rows.Add(Row);

                                        }
                                    }
                                }
                                Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = Table;
                            }
                            jsonResult = Json(((DataTable)Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()]).ToDataSourceResult(_Request));
                        }
                        else
                        {
                            if (SubDashboardID != "")
                            {
                                string Query = "declare @از_تاریخ as Nvarchar(255) = N'" + Tools.GetDefaultValue((string)Session["ChartFromDate"]) + "'\n" +
                                               "declare @تا_تاریخ  as Nvarchar(255) = N'" + Tools.GetDefaultValue((string)Session["ChartToDate"]) + "'\n";

                                SubDashboard _SubDashboard = new SubDashboard(CoreObject.Find(long.Parse(SubDashboardID)));
                                Query += "Declare @" + _SubDashboard.FullName + " as Nvarchar(400)=N'" + Category + "' \n";
                                Query += Desktop.DataInformationEntryForm[_DataKey].Query;

                                int StartIndexOfQuery = Query.ToUpper().IndexOf("ORDER BY");
                                string OrderByQuery = "";
                                if (StartIndexOfQuery != -1)
                                {
                                    OrderByQuery = Query.Substring(StartIndexOfQuery, (Query.Length - StartIndexOfQuery));
                                    Query = Query.Substring(0, StartIndexOfQuery);
                                }


                                Query += " And " + _SubDashboard.Condition + " \n" + OrderByQuery;



                                jsonResult = Json(Referral.DBData.SelectDataTable(Query).ToDataSourceResult(_Request));
                            }
                            else
                            {
                                InformationEntryForm informationEntryForm = new InformationEntryForm(Form);
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(informationEntryForm.RelatedTable).ParentID));
                                if (DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase && DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source)
                                {
                                    long RegisterCounter = DataConvertor.RegisterCount(TableName, DataSourceInfo.ServerName, DataSourceInfo.DataBase);

                                    if (Form.ParentID == 0)
                                    {
                                        if (Desktop.CachedTable[_DataKey] == null || RegisterCounter != Desktop.RegisterdTableID[informationEntryForm.RelatedTable.ToString()])
                                        {
                                            Desktop.MasterProcessID[_DataKey] = _MasterProcessID;
                                            DataTable dataTable = Desktop.Read(_DataKey, Where, _ParentID, _ShowRowCount);
                                            Desktop.CachedTable[_DataKey] = dataTable;
                                            Desktop.RegisterdTableID[informationEntryForm.RelatedTable.ToString()] = RegisterCounter;
                                            jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                        }
                                        else
                                            jsonResult = Json(Desktop.CachedTable[_DataKey].ToDataSourceResult(_Request));
                                    }
                                    else
                                    {
                                        if (Desktop.CachedTable[_DataKey + "_" + _ParentID] == null || RegisterCounter != Desktop.RegisterdTableID[informationEntryForm.RelatedTable.ToString()])
                                        {
                                            Desktop.MasterProcessID[_DataKey] = _MasterProcessID;
                                            DataTable dataTable = Desktop.Read(_DataKey, Where, _ParentID, _ShowRowCount);
                                            Desktop.CachedTable[_DataKey + "_" + _ParentID] = dataTable;
                                            Desktop.RegisterdTableID[informationEntryForm.RelatedTable.ToString()] = RegisterCounter;
                                            jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                        }
                                        else
                                            jsonResult = Json(Desktop.CachedTable[_DataKey + "_" + _ParentID].ToDataSourceResult(_Request));
                                    }

                                }
                                else
                                {
                                    Desktop.MasterProcessID[_DataKey] = _MasterProcessID;
                                    jsonResult = Json(Desktop.Read(_DataKey, Where, _ParentID, _ShowRowCount).ToDataSourceResult(_Request));

                                }
                            }

                        }

                        break;
                    }
                case CoreDefine.Entities.جدول:
                    {
                        if (Desktop.DataTableForm[_DataKey] == null)
                            Desktop.StartupSetting(_DataKey);

                        TableName = Form.FullName;
                        jsonResult = Json(Desktop.Read(_DataKey, Where, _ParentID, _ShowRowCount).ToDataSourceResult(_Request));
                        break;
                    }
                case CoreDefine.Entities.فرم_جستجو:
                    {
                        TableName = Form.FullName;
                        jsonResult = Json(Desktop.Read(_DataKey, Where, _ParentID, _ShowRowCount).ToDataSourceResult(_Request));
                        break;
                    }

            }
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult GetElementFilter(string FieldName, string DataKey, string ParentID)
        {
            string Url = "~/Views/Shared/EditorTemplates/";
            foreach (Field Item in Desktop.DataFields[DataKey])
            {
                if (Item.FieldName == FieldName)
                {
                    List<SelectListItem> ComboItems = new List<SelectListItem>();
                    if (Item.FieldType == CoreDefine.InputTypes.ComboBox)
                    {
                        string[] ValueItem = Item.SpecialValue.Split('،');
                        foreach (string Value in ValueItem)
                        {
                            ComboItems.Add(new SelectListItem() { Text = Value, Value = Value });
                        }
                    }

                    string value = "";
                    if (Item.FieldNature == "Bigint")
                        value = "";
                    else if (Item.FieldNature == "Bit")
                        value = "false";

                    Item.FieldType = FieldName.IndexOf("تاریخ") > -1 ? CoreDefine.InputTypes.PersianDateTime : Item.FieldType;

                    ViewData["DataKey"] = DataKey;
                    ViewData["FieldName"] = "Filtered_" + FieldName;
                    ViewData["InputType"] = Item.FieldType;
                    ViewData["IsReadonly"] = false;
                    ViewData["IsRequired"] = false;
                    ViewData["NullValue"] = "نامشخص";
                    ViewData["FalseValue"] = Item.ComboValues()[0];
                    ViewData["TrueValue"] = Item.ComboValues()[1];
                    ViewData["IsInCellEditMode"] = false;
                    ViewData["FieldTitle"] = Item.Title();
                    ViewData["DigitsAfterDecimal"] = Item.DigitsAfterDecimal;
                    ViewData["RelatedField"] = Item.RelatedField;
                    ViewData["IsInCellEditMode"] = false;
                    ViewData["IsGridField"] = true;
                    ViewData["IsLeftWrite"] = Item.IsLeftWrite;
                    ViewData["RelatedTable"] = Item.RelatedTable;
                    ViewData["FieldValue"] = value;
                    ViewData["_TableID"] = "0";
                    ViewData["MaxValue"] = Item.MaxValue;
                    ViewData["MinValue"] = Item.MinValue;
                    ViewData["ComboItems"] = ComboItems;
                    ViewData["IsExclusive"] = Item.IsExclusive;
                    ViewData["ActiveOnKeyDown"] = false;
                    ViewData["CoreObjectID"] = Item.CoreObjectID;
                    ViewData["ShowHideElement"] = "";
                    ViewData["FieldComment"] = ComboItems;
                    ViewData["ComboItems"] = Item.FieldComment;

                    switch (Item.FieldType)
                    {
                        case CoreDefine.InputTypes.CoreRelatedTable: Url += "TCoreObjectList"; break;

                        case CoreDefine.InputTypes.RelatedTable: Url += "TForeignKey"; break;

                        case CoreDefine.InputTypes.FillTextAutoComplete: Url += "FillTextAutoComplete"; break;

                        //case CoreDefine.InputTypes.RelatedTableTree:
                        //    @Html.Editor(FieldItem.FullName, "TForeignKeyTree", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.ActionType:
                        //    @Html.Editor(FieldItem.FullName, "ProcessActionType", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.RecordType:
                        //    @Html.Editor(FieldItem.FullName, "ProcessRecordType", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.NotificationType:
                        //    @Html.Editor(FieldItem.FullName, "NotificationType", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.AttachmentUploadType:
                        //    @Html.Editor(FieldItem.FullName, "AttachmentUploadType", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.LongText:
                        //    @Html.Editor(FieldItem.FullName, "MultilineString", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.Editor:
                        //    @Html.Editor(FieldItem.FullName, "TEditor", FieldItem.Parameter)
                        //    break;

                        case CoreDefine.InputTypes.TwoValues: Url += "Boolean"; break;

                        case CoreDefine.InputTypes.Number: Url += "TableNumber"; break;

                        //case CoreDefine.InputTypes.Image:
                        //    @Html.Editor(FieldItem.FullName, "Image", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.Query:
                        //    @Html.Editor(FieldItem.FullName, "Query", FieldItem.Parameter)
                        //    break;

                        case CoreDefine.InputTypes.MiladyDateTime: Url += "TableMiladiDate"; break;

                        case CoreDefine.InputTypes.PersianDateTime: Url += "TableShamsiDate"; break;

                        //case CoreDefine.InputTypes.Icon:
                        //    @Html.Editor(FieldItem.FullName, "Icon", FieldItem.Parameter)
                        //    break;

                        case CoreDefine.InputTypes.ComboBox: Url += "ComboBox"; break;

                        //case CoreDefine.InputTypes.Sparkline:
                        //    @Html.Editor(FieldItem.FullName, "Sparkline", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.Color:
                        //    @Html.Editor(FieldItem.FullName, "ColorPicker", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.MultiSelect:
                        //    @Html.Editor(FieldItem.FullName, "MultiSelect", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.Clock:
                        //    @Html.Editor(FieldItem.FullName, "Time", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.BPMSModel:
                        //    @Html.Editor(FieldItem.FullName, "BPMSModel", FieldItem.Parameter)
                        //    break;

                        //case CoreDefine.InputTypes.Password:
                        //    @Html.Editor(FieldItem.FullName, "Password", FieldItem.Parameter)
                        //    break;

                        case CoreDefine.InputTypes.Plaque: Url += "TablePlaque"; break;

                        default: Url += "String"; break;
                    }

                    Url += ".cshtml";
                    ViewData.Model = new object();
                    using (var sw = new StringWriter())
                    {
                        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, Url);
                        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                        viewResult.View.Render(viewContext, sw);
                        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                        return Json(sw.GetStringBuilder().ToString());
                    }
                }
            }


            //return View(Url+ ".cshtml");
            return Json("");

        }
        
        public ActionResult Read(string _DataKey, int _ParentID, long _MasterProcessID, long _ProcessStep, long? RecordID, [DataSourceRequest] DataSourceRequest _Request, string SubDashboardID = "", string Category = "", string FilterField = "", string FilterOperator = "", string FilterValue = "", string FilterMiddleOperator = "", bool IsAdvancedSearch = false, int _ShowRowCount = 0, string _Where = "",bool IsReload=false)
        {
            JsonResult jsonResult = new JsonResult();
            string TableName = "";
            CoreObject Form = CoreObject.Find(long.Parse(_DataKey));
            string GridFilter = string.Empty;
            string[] FilterFieldArr = FilterField.Split(',');
            string[] FilterOperatorArr = FilterOperator.Split(',');
            string[] FilterValueArr = FilterValue.Split(',');
            string[] FilterMiddleOperatorArr = FilterMiddleOperator.Split(',');

            switch (Form.Entity)
            {
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        if (Desktop.DataInformationEntryForm[_DataKey] == null)
                            Desktop.StartupSetting(_DataKey);

                        TableName = CoreObject.Find(Desktop.DataInformationEntryForm[_DataKey].RelatedTable).FullName;

                        if (Form.ParentID != 0 && RecordID == 0 && _ParentID == 0)
                        {
                            if (Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] == null)
                            { 
                                DataTable Table = Desktop.CachedTable[_DataKey];
                                if(Table == null)
                                    Table = Desktop.Read(_DataKey, "", _ParentID); 
                                Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = Table;
                            }
                            jsonResult = Json(((DataTable)Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()]).ToDataSourceResult(_Request));
                        }
                        else
                        {
                            if (SubDashboardID != "")
                            {
                                string Query = "declare @از_تاریخ as Nvarchar(255) = N'" + Tools.GetDefaultValue((string)Session["ChartFromDate"]) + "'\n" +
                                               "declare @تا_تاریخ  as Nvarchar(255) = N'" + Tools.GetDefaultValue((string)Session["ChartToDate"]) + "'\n";

                                SubDashboard _SubDashboard = new SubDashboard(CoreObject.Find(long.Parse(SubDashboardID)));
                                CoreObject GroupFieldCore = CoreObject.Find(_SubDashboard.GroupField);
                                CoreObject TableCore = CoreObject.Find(GroupFieldCore.ParentID);
                                Field GroupField = GroupFieldCore.CoreObjectID == 0 ? new Field() : new Field(GroupFieldCore);
                                DataTable DashboardData = new DataTable();
                                if (string.IsNullOrEmpty(_SubDashboard.CategoryAxisQuery))
                                {
                                    Query += "Select \n";
                                    if (string.IsNullOrEmpty(Desktop.DataInformationEntryForm[_DataKey].Query))
                                    {
                                        CoreObject TableRelatedCore = CoreObject.Find(Desktop.DataInformationEntryForm[_DataKey].RelatedTable);
                                        Query += "* FROM " + TableRelatedCore.FullName + "\n";
                                    }
                                    else
                                        Query+="\n"+ Desktop.DataInformationEntryForm[_DataKey].Query + "\n";
                                    Query += "Where \n";
                                    Query += Session["WhereDashboardID" + SubDashboardID.ToString()].ToString();
                                    if (_SubDashboard.DateField > 0)
                                    {
                                        CoreObject DateFieldCore = CoreObject.Find(_SubDashboard.DateField);
                                        Query += "\nAnd( " + DateFieldCore.FullName + "  between @از_تاریخ and @تا_تاریخ)";
                                    }

                                    Query += _SubDashboard.Condition != "" ? " And (" + _SubDashboard.Condition + " ) " : "";
                                }
                                else
                                    Query += _SubDashboard.CategoryAxisQuery;

                                Query = Tools.CheckQuery(Query);
                                if (Referral.MasterDatabaseID == TableCore.ParentID)
                                {
                                    DashboardData = Referral.DBData.SelectDataTable(Query);
                                }
                                else
                                {
                                    DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                                    switch (DataSourceInfo.DataSourceType)
                                    {
                                        default: break;
                                    }
                                }
                                jsonResult = Json(DashboardData.ToDataSourceResult(_Request));
                            }
                            else
                            {
                                InformationEntryForm informationEntryForm = new InformationEntryForm(Form);
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(informationEntryForm.RelatedTable).ParentID));
                                if (DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase && DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source)
                                {
                                    string CachedName = _DataKey;
                                    if (FilterField != "" && FilterFieldArr.Length > 0)
                                    {
                                        string TempQuery = Tools.ConvertToSQLQuery(Desktop.DataInformationEntryForm[_DataKey].Query).ToUpper().Trim().Replace("\n", "").Replace(" ", "");
                                        for (int index = 0; index < FilterFieldArr.Length; index++)
                                        {
                                            Field field = Desktop.DataFields[_DataKey][Desktop.DataFields[_DataKey].FindIndex(x => x.FieldName == FilterFieldArr[index])];
                                            if (field.IsVirtual)
                                                GridFilter += "(" + field.SpecialValue.Replace("@", "") + ")";
                                            else
                                            {
                                                if(Desktop.DataInformationEntryForm[_DataKey].Query !="")
                                                { 
                                                    if(TempQuery.IndexOf("."+field.FieldName+",")>-1)
                                                    {
                                                        string[] TempArr = TempQuery.Split(',');
                                                        bool IsFind = false;
                                                        foreach(string TempStr in TempArr)
                                                        {
                                                            if(TempStr.IndexOf("." + field.FieldName ) > -1)
                                                            {
                                                                GridFilter += TempStr;
                                                                IsFind= true;
                                                                break;
                                                            }
                                                        }
                                                        if(!IsFind)
                                                            GridFilter += field.FieldName;
                                                    }
                                                    else if(TempQuery.IndexOf("." + field.FieldName + "F") > -1)
                                                    { 
                                                        string[] TempArr = TempQuery.Split(',');
                                                        bool IsFind = false;
                                                        foreach (string TempStr in TempArr)
                                                        {
                                                            if (TempStr.IndexOf("." + field.FieldName + "F") > -1)
                                                            {
                                                                GridFilter += TempStr.Substring(0, TempStr.IndexOf("." + field.FieldName + "F"))+ "." + field.FieldName;
                                                                IsFind = true;
                                                                break;
                                                            }
                                                        }
                                                        if (!IsFind)
                                                            GridFilter += field.FieldName;
                                                    }
                                                    else
                                                        GridFilter += field.FieldName;

                                                }
                                                else
                                                GridFilter += field.FieldName;
                                            }

                                            GridFilter += " ";
                                            switch (field.FieldType)
                                            {
                                                case CoreDefine.InputTypes.ShortText:
                                                case CoreDefine.InputTypes.LongText:
                                                case CoreDefine.InputTypes.Phone:
                                                case CoreDefine.InputTypes.Clock:
                                                case CoreDefine.InputTypes.ComboBox:
                                                case CoreDefine.InputTypes.Editor:
                                                case CoreDefine.InputTypes.FillTextAutoComplete:
                                                case CoreDefine.InputTypes.NationalCode:
                                                case CoreDefine.InputTypes.PersianDateTime:
                                                case CoreDefine.InputTypes.MiladyDateTime:
                                                    {
                                                        switch (FilterOperatorArr[index])
                                                        {
                                                            case "نامساوی": GridFilter += "<> N'" + FilterValueArr[index] + "'"; break;
                                                            case "بزرگتر یا مساوی": GridFilter += ">= N'" + FilterValueArr[index] + "'"; break;
                                                            case "بزرگتر": GridFilter += "> N'" + FilterValueArr[index] + "'"; break;
                                                            case "کوچکتر یا مساوی": GridFilter += "<= N'" + FilterValueArr[index] + "'"; break;
                                                            case "کوچکتر": GridFilter += "< N'" + FilterValueArr[index] + "'"; break;
                                                            case "شروع با": GridFilter += "Like N'%" + FilterValueArr[index] + "'"; break;
                                                            case "شامل": GridFilter += "Like N'%" + FilterValueArr[index] + "%'"; break;
                                                            case "شامل نباشد": GridFilter += "Not Like N'%" + FilterValueArr[index] + "%'"; break;
                                                            case "پایان با": GridFilter += "Like N'" + FilterValueArr[index] + "%'"; break;
                                                            case "تهی": GridFilter += "IS Null"; break;
                                                            case "تهی نیست": GridFilter += "Is Not Null"; break;
                                                            case "خالی": GridFilter += " = N''"; break;
                                                            case "خالی نیست": GridFilter += " <> N'' "; break;
                                                            default: GridFilter += " = N'" + FilterValueArr[index] + "'"; break;
                                                        }
                                                        break;
                                                    }

                                                default:
                                                    {
                                                        switch (FilterOperatorArr[index])
                                                        {
                                                            case "نامساوی": GridFilter += "<> " + FilterValueArr[index]; break;
                                                            case "بزرگتر یا مساوی": GridFilter += ">= " + FilterValueArr[index]; break;
                                                            case "بزرگتر": GridFilter += "> " + FilterValueArr[index]; break;
                                                            case "کوچکتر یا مساوی": GridFilter += "<= " + FilterValueArr[index]; break;
                                                            case "کوچکتر": GridFilter += "< " + FilterValueArr[index]; break;
                                                            case "تهی": GridFilter += "IS Null"; break;
                                                            case "تهی نیست": GridFilter += "Is Not Null"; break;
                                                            default: GridFilter += " = " + FilterValueArr[index] + " "; break;
                                                        }
                                                        break;
                                                    };
                                            }

                                            if (FilterMiddleOperator != "" && index < FilterMiddleOperatorArr.Length)
                                                GridFilter += " " + FilterMiddleOperatorArr[index] + " ";
                                        }
                                    }
                                    if (Form.ParentID != 0)
                                        CachedName = _DataKey + "_" + _ParentID;

                                    DataTable dataTable = new DataTable();
                                    string SearchinformationEntryFormQuery = Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID] == null ? "" : Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID].ToString();
                                    string SearchinformationEntryFormGridFilter = Session["SearchinformationEntryFormGridFilter" + _DataKey + "_" + _ParentID] == null ? "" : Session["SearchinformationEntryFormGridFilter" + _DataKey + "_" + _ParentID].ToString();


                                    if (_Where != "")
                                    {
                                        Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID] = _Where;
                                        dataTable = Referral.DBData.SelectDataTable(_Where); 
                                        jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                        Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] = _Request.Page;
                                        Session["SearchinformationEntryFormGridFilter" + _DataKey + "_" + _ParentID] = "";
                                    }
                                    else if (_Where == "" && SearchinformationEntryFormQuery == "" && SearchinformationEntryFormGridFilter != "" && (Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] == null ? 1 : (int)Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID]) != _Request.Page)
                                    {
                                        Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] = _Request.Page;
                                        jsonResult = Json(Desktop.CachedTable[CachedName].ToDataSourceResult(_Request));
                                    }
                                    else if (_Where == "" && GridFilter == "" && SearchinformationEntryFormGridFilter!="")
                                    {
                                        dataTable = Desktop.Read(_DataKey, SearchinformationEntryFormGridFilter, _ParentID, _ShowRowCount); 
                                        Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID] = "";
                                        jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                        Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = dataTable;
                                        Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] = _Request.Page;  
                                    }
                                    else if (_Where == "" && _Request.Filters.Count == 0 && GridFilter != "")
                                    {
                                        dataTable = Desktop.Read(_DataKey, GridFilter, _ParentID, _ShowRowCount); 
                                        Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID] = "";
                                        jsonResult = Json(dataTable.ToDataSourceResult(_Request)); 
                                    } 
                                    else if (_Where == "" && SearchinformationEntryFormQuery != "" && !IsReload)
                                    {
                                        _Where = Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID].ToString();
                                        dataTable = Referral.DBData.SelectDataTable(_Where); 
                                        Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] = _Request.Page;
                                        Session["SearchinformationEntryFormGridFilter" + _DataKey + "_" + _ParentID] = "";
                                        jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                    }
                                    else if ((Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] == null ? 1 : (int)Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID]) != _Request.Page && _Where == "" && _Request.Filters.Count == 0 && GridFilter == "" && SearchinformationEntryFormQuery == "" && (Session["LastSearchinformationEntryDataKey"]==null?"": Session["LastSearchinformationEntryDataKey"].ToString()) ==(_DataKey + "_" + _ParentID))
                                        if(IsReload)
                                        {
                                            dataTable = Desktop.Read(_DataKey, GridFilter, _ParentID, _ShowRowCount); 
                                            Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID] = "";
                                            Session["SearchinformationEntryFormGridFilter" + _DataKey + "_" + _ParentID] = "";
                                            jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                            Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = dataTable;
                                            Session["LastSearchinformationEntryDataKey"] = _DataKey + "_" + _ParentID;
                                            Session["LastPageInformationEntry" + _DataKey + "_" + _ParentID] = _Request.Page;
                                        }
                                        else
                                        {
                                            jsonResult = Json(Desktop.CachedTable[CachedName].ToDataSourceResult(_Request));
                                        }
                                    else if (Desktop.CachedTable[CachedName] !=null && !IsReload && _Where == "" && _Request.Filters.Count == 0 && GridFilter == "" && SearchinformationEntryFormQuery == "" && (Session["LastSearchinformationEntryDataKey"]==null?"": Session["LastSearchinformationEntryDataKey"].ToString()) ==(_DataKey + "_" + _ParentID))
                                    {
                                        jsonResult = Json(Desktop.CachedTable[CachedName].ToDataSourceResult(_Request));
                                    }
                                    else if ((_Request.Filters.Count == 0 && SearchinformationEntryFormQuery == "")|| IsReload)
                                    {
                                        dataTable = Desktop.Read(_DataKey, GridFilter, _ParentID, _ShowRowCount,_MasterProcessID,_ProcessStep); 
                                        Session["SearchinformationEntryFormQuery" + _DataKey + "_" + _ParentID] = "";
                                        Session["SearchinformationEntryFormGridFilter" + _DataKey + "_" + _ParentID] = "";
                                        Session["LastSearchinformationEntryDataKey"] = _DataKey + "_" + _ParentID;
                                        jsonResult = Json(dataTable.ToDataSourceResult(_Request));
                                    }
                                    else
                                    {
                                        jsonResult = Json(Desktop.Read(_DataKey, GridFilter, _ParentID, _ShowRowCount, _MasterProcessID, _ProcessStep).ToDataSourceResult(_Request)); 
                                    } 

                                }
                                else
                                {
                                    Desktop.MasterProcessID[_DataKey] = _MasterProcessID;
                                    jsonResult = Json(Desktop.Read(_DataKey, "", _ParentID, _ShowRowCount).ToDataSourceResult(_Request));

                                }
                            }

                        }

                        break;
                    }
                case CoreDefine.Entities.جدول:
                    {
                        if (Desktop.DataTableForm[_DataKey] == null)
                            Desktop.StartupSetting(_DataKey);

                        TableName = Form.FullName; 
                        jsonResult = Json(Desktop.Read(_DataKey, "", _ParentID, _ShowRowCount).ToDataSourceResult(_Request));
                        break;
                    }
                case CoreDefine.Entities.فرم_جستجو:
                    {
                        if (FilterField != "" && FilterFieldArr.Length > 0)
                        {
                            for (int index = 0; index < FilterFieldArr.Length; index++)
                            {
                                Field field = Desktop.DataFields[_DataKey][Desktop.DataFields[_DataKey].FindIndex(x => x.FieldName == FilterFieldArr[index])];
                                if (field.IsVirtual)
                                    GridFilter += field.SpecialValue == "" ? field.FieldName : "(" + field.SpecialValue.Replace("@", "") + ")";
                                else
                                    GridFilter += field.FieldName;

                                switch (field.FieldType)
                                {
                                    case CoreDefine.InputTypes.ShortText:
                                    case CoreDefine.InputTypes.LongText:
                                    case CoreDefine.InputTypes.Phone:
                                    case CoreDefine.InputTypes.Clock:
                                    case CoreDefine.InputTypes.ComboBox:
                                    case CoreDefine.InputTypes.Editor:
                                    case CoreDefine.InputTypes.FillTextAutoComplete:
                                    case CoreDefine.InputTypes.NationalCode:
                                    case CoreDefine.InputTypes.PersianDateTime:
                                    case CoreDefine.InputTypes.MiladyDateTime:
                                        {
                                            GridFilter += " ";
                                            switch (FilterOperatorArr[index])
                                            {
                                                case "نامساوی": GridFilter += "<> N'" + FilterValueArr[index] + "'"; break;
                                                case "بزرگتر یا مساوی": GridFilter += ">= N'" + FilterValueArr[index] + "'"; break;
                                                case "بزرگتر": GridFilter += "> N'" + FilterValueArr[index] + "'"; break;
                                                case "کوچکتر یا مساوی": GridFilter += "<= N'" + FilterValueArr[index] + "'"; break;
                                                case "کوچکتر": GridFilter += "< N'" + FilterValueArr[index] + "'"; break;
                                                case "شروع با": GridFilter += "Like N'%" + FilterValueArr[index] + "'"; break;
                                                case "شامل": GridFilter += "Like N'%" + FilterValueArr[index] + "%'"; break;
                                                case "شامل نباشد": GridFilter += "Not Like N'%" + FilterValueArr[index] + "%'"; break;
                                                case "پایان با": GridFilter += "Like N'" + FilterValueArr[index] + "%'"; break;
                                                case "تهی": GridFilter += "IS Null"; break;
                                                case "تهی نیست": GridFilter += "Is Not Null"; break;
                                                case "خالی": GridFilter += " = N''"; break;
                                                case "خالی نیست": GridFilter += " <> N'' "; break;
                                                default: GridFilter += " = N'" + FilterValueArr[index] + "'"; break;
                                            }
                                            break;
                                        }

                                    default:
                                        {
                                            GridFilter += " ";
                                            switch (FilterOperatorArr[index])
                                            {
                                                case "نامساوی": GridFilter += "<> " + FilterValueArr[index]; break;
                                                case "بزرگتر یا مساوی": GridFilter += ">= " + FilterValueArr[index]; break;
                                                case "بزرگتر": GridFilter += "> " + FilterValueArr[index]; break;
                                                case "کوچکتر یا مساوی": GridFilter += "<= " + FilterValueArr[index]; break;
                                                case "کوچکتر": GridFilter += "< " + FilterValueArr[index]; break;
                                                case "تهی": GridFilter += "IS Null"; break;
                                                case "تهی نیست": GridFilter += "Is Not Null"; break;
                                                default: GridFilter += " = N'" + FilterValueArr[index] + "'"; break;
                                            }
                                            break;
                                        };
                                }

                                if (FilterMiddleOperator != "" && index < FilterMiddleOperatorArr.Length)
                                    GridFilter += " " + FilterMiddleOperatorArr[index] + " ";
                            }
                        } 

                        jsonResult = Json(Desktop.Read(_DataKey, GridFilter, _ParentID, _ShowRowCount).ToDataSourceResult(_Request)); 
                        break;
                    }

            }

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        
        public ActionResult ProcessStepRead(string _DataKey, int _ParentID, long _ProcessID, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = new JsonResult(); 
            InformationEntryForm Form = new InformationEntryForm( CoreObject.Find(long.Parse(_DataKey)));
             
            DataTable dataTable = Referral.DBData.SelectDataTable("Select ارجاع_مراحل_فرآیند.مرحله_فرآیند,ارجاع_مراحل_فرآیند.جدول,مراحل_فرآیند.سطر, ارجاع_مراحل_فرآیند.تاریخ_ثبت,ارجاع_مراحل_فرآیند.ساعت_ثبت,ارجاع_مراحل_فرآیند.نام_ثبت_کننده as اجرا_کننده, عنوان_مرحله, مرحله_بعد_فرآیند, عنوان_مرحله_بعد_فرآیند  " +
                "\n,(select  کاربر.نام_و_نام_خانوادگی +N' ['+CAST(کاربر.شناسه as nvarchar(400))+N']' from  کاربر  Where کاربر.شناسه = ارجاع_مراحل_فرآیند.دریافت_کننده ) as نفرات_دریافت_کننده " +
                "\n,(select سمت_سازمانی.عنوان  +N' ['+CAST(سمت_سازمانی.شناسه as nvarchar(400))+N']' from سمت_سازمانی   Where  سمت_سازمانی_دریافت_کننده= سمت_سازمانی.شناسه) as سمت_دریافت_کننده" +
                "\n,ارجاع_مراحل_فرآیند.تاریخ_مشاهده" +
                "\n, ارجاع_مراحل_فرآیند.ساعت_مشاهده " +
                "\n  FROM مراحل_فرآیند inner join ارجاع_مراحل_فرآیند on مراحل_فرآیند.مرحله_فرآیند =ارجاع_مراحل_فرآیند.مرحله_فرآیند and  مراحل_فرآیند.سطر = ارجاع_مراحل_فرآیند.رکورد and مراحل_فرآیند.فرآیند = ارجاع_مراحل_فرآیند.فرآیند" +
                "\nWhere "  +
                "\nسطر = " + _ParentID + " AND  مراحل_فرآیند.فرآیند  = " + _ProcessID + " AND مراحل_فرآیند.جدول = " + Form.RelatedTable + "  AND مرحله_بعد_فرآیند> 0" +
                " union" +
                "\n Select  مرحله_فرآیند, جدول, سطر, تاریخ_ثبت, ساعت_ثبت, نام_ثبت_کننده as اجرا_کننده, عنوان_مرحله, مرحله_بعد_فرآیند, عنوان_مرحله_بعد_فرآیند" +
                "\n, (select  کاربر.نام_و_نام_خانوادگی + N' [' + CAST(کاربر.شناسه as nvarchar(400)) + N']' + N'-'  from ارجاع_مراحل_فرآیند as A inner join کاربر on  A.دریافت_کننده = کاربر.شناسه Where A.رکورد = مراحل_فرآیند.سطر AND A.فرآیند = مراحل_فرآیند.فرآیند AND A.جدول = مراحل_فرآیند.جدول and A.مرحله_فرآیند = مراحل_فرآیند.مرحله_فرآیند and(A.تاریخ_ثبت + SUBSTRING(A.ساعت_ثبت, 1, 5)) >= (مراحل_فرآیند.تاریخ_ثبت + SUBSTRING(مراحل_فرآیند.ساعت_ثبت, 1, 5)) for xml path('') ) as نفرات_دریافت_کننده" +
                "\n,(select سمت_سازمانی.عنوان + N' [' + CAST(سمت_سازمانی.شناسه as nvarchar(400)) + N']' + N'-'  from ارجاع_مراحل_فرآیند as A inner join سمت_سازمانی on  A.سمت_سازمانی_دریافت_کننده = سمت_سازمانی.شناسه  Where A.رکورد = مراحل_فرآیند.سطر AND A.فرآیند = مراحل_فرآیند.فرآیند AND A.جدول = مراحل_فرآیند.جدول and A.مرحله_فرآیند = مراحل_فرآیند.مرحله_فرآیند and(A.تاریخ_ثبت + SUBSTRING(A.ساعت_ثبت, 1, 5)) >= (مراحل_فرآیند.تاریخ_ثبت + SUBSTRING(مراحل_فرآیند.ساعت_ثبت, 1, 5)) group by  سمت_سازمانی.عنوان ,سمت_سازمانی.شناسه for xml path('') ) as سمت_دریافت_کننده" +
                "\n,N''" +
                "\n,N''" +
                "\nFROM مراحل_فرآیند" +
                "\nWhere سطر = "+ _ParentID + " AND فرآیند = "+ _ProcessID + " AND جدول = " + Form.RelatedTable + "  AND مرحله_بعد_فرآیند> 0" +
                "\nand مراحل_فرآیند.شناسه not in (select A.شناسه FROM مراحل_فرآیند as A inner join ارجاع_مراحل_فرآیند on A.مرحله_فرآیند = ارجاع_مراحل_فرآیند.مرحله_فرآیند and A.سطر = ارجاع_مراحل_فرآیند.رکورد and A.فرآیند = ارجاع_مراحل_فرآیند.فرآیند" +
                "\nWhere" +
                "\nA.سطر = "+ _ParentID + " AND A.فرآیند = "+ _ProcessID + " AND A.جدول = " + Form.RelatedTable + "  AND مرحله_بعد_فرآیند> 0)");

            jsonResult = Json(dataTable.ToDataSourceResult(_Request)); 

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
       
        public ActionResult ReadRegistry(long _UserAccountID, string _FromDate, string _ToDate, string _TableName, string _RegistryTable, string _DataKey, string _RecordID, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = new JsonResult();
            string[] ColumnName = _RegistryTable == "Login" ? Desktop.LoginRegistryColumnName : Desktop.RegistryColumnName;
            if (_TableName == "" && _RecordID == "0")
            {
                _FromDate = _FromDate == "" ? CDateTime.GetNowshamsiDate() : _FromDate;
                _ToDate = _ToDate == "" ? CDateTime.GetNowshamsiDate() : _ToDate;

            }
            string Query = "Declare @FromDate as Nvarchar(255)=N'" + _FromDate + "'\n" +
                            "Declare @ToDate as Nvarchar(255)=N'" + _ToDate + "'\n" +
                            "Declare @TableName as Nvarchar(255)=N'" + _TableName + "'\n" +
                            "Declare @UserAccountID as Bigint=" + _UserAccountID + "\n" +
                            "Declare @RecordID as Bigint=" + _RecordID + "\n" +
                            "Select " + _RegistryTable + ColumnName[0] + " as RegistryID,\n";
            for (int i = 1; i < ColumnName.Length; i++)
            {
                if (ColumnName[i] == "UserAccountID")
                    Query += "(Select top 1  نام_و_نام_خانوادگی From " + Referral.DBData.ConnectionData.DataBase + ".dbo.کاربر where شناسه = UserAccountID) as UserAccountID,";
                else if (_RegistryTable == "Update" && ColumnName[i] == "Value")
                {
                    Query += "FieldName,\nPreviousValue,\nNewValue";
                    if (i < ColumnName.Length - 1)
                        Query += ",";
                }
                else if (_RegistryTable == "View" && ColumnName[i] == "Value")
                {
                }
                else
                {
                    Query += ColumnName[i];
                    if (i < ColumnName.Length - 1)
                        Query += ",";
                }

                Query += "\n";
            }
            Query += "From " + _RegistryTable + "_APMRegistry \n";
            if (_RegistryTable == "Login")
            {
                Query += "Where   " +
                         "(UserAccountID=@UserAccountID or @UserAccountID=0)\n" +
                        "  and  ((LoginDate >= @FromDate or ISNULL(@FromDate, N'') = N'')\n" +
                        "  and  (LoginDate <= @ToDate or ISNULL(@ToDate, N'') = N''))\n";
            }
            else
            {
                Query += "Where   " +
                         "(UserAccountID=@UserAccountID or @UserAccountID=0)\n" +
                         "And (RecordID=@RecordID or @RecordID=0)\n" +
                        "  and  ((RegistryDate >= @FromDate or ISNULL(@FromDate, N'') = N'')\n" +
                        "  and  (RegistryDate <= @ToDate or ISNULL(@ToDate, N'') = N''))\n" +
                        "  and  (TableName = @TableName or ISNULL(@TableName, N'') = N'')\n";
            }

            DataTable dataTable = Referral.DBRegistry.SelectDataTable(Query);
            Session["RegistryTable"] = dataTable;
            Session["RegistryTableDataKey"] = _DataKey;

            jsonResult = Json(dataTable.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
       
        public ActionResult ReadWithOutCach(string _DataKey, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = new JsonResult();
            jsonResult = Json(Desktop.Read(_DataKey, "", 0).ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult Create(string _DataKey, int _ParentID, long _MasterProcessID, long _ProcessStep, long RecordID, [DataSourceRequest] DataSourceRequest _Request, FormCollection _FormCollection)
        {
            string[] FormInputName = new string[0];
            object[] FormInputValue = new object[0];
            string IsFixedItem = "";
            Field IDField = Desktop.DataTable[_DataKey].IDField();
            RecordID = long.Parse(_FormCollection[IDField.FieldName].ToString());

            foreach (Field Item in Desktop.DataFields[_DataKey])
                if (Array.IndexOf(_FormCollection.AllKeys, Item.FieldName) > -1)
                {
                    Array.Resize(ref FormInputName, FormInputName.Length + 1);
                    Array.Resize(ref FormInputValue, FormInputValue.Length + 1);
                    FormInputName[FormInputName.Length - 1] = Item.FieldName;
                    FormInputValue[FormInputValue.Length - 1] = _FormCollection[Item.FieldName];
                }

            CalAutoFillQuery(_DataKey, _ParentID.ToString(), RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

            return Json(SaveFormEditor(_DataKey, _ParentID.ToString(), RecordID, FormInputName, FormInputValue));
        }
        public ActionResult Update(string _DataKey, int _ParentID, long _MasterProcessID, long _ProcessStep, long RecordID, [DataSourceRequest] DataSourceRequest _Request, FormCollection _FormCollection, string SubDashboardID = "", string Category = "")
        {

            string[] FormInputName = new string[0];
            object[] FormInputValue = new object[0];
            Field IDField = Desktop.DataTable[_DataKey].IDField();
            RecordID = long.Parse(_FormCollection[IDField.FieldName].ToString());

            foreach (Field Item in Desktop.DataFields[_DataKey])
                if (Array.IndexOf(_FormCollection.AllKeys, Item.FieldName) > -1)
                {
                    Array.Resize(ref FormInputName, FormInputName.Length + 1);
                    Array.Resize(ref FormInputValue, FormInputValue.Length + 1);
                    FormInputName[FormInputName.Length - 1] = Item.FieldName;
                    FormInputValue[FormInputValue.Length - 1] = _FormCollection[Item.FieldName];
                }
            return Json(SaveFormEditor(_DataKey, _ParentID.ToString(), RecordID, FormInputName, FormInputValue));
        }

        public ActionResult UpdateJson(string _DataKey, int _ParentID, string _FormCollection, [DataSourceRequest] DataSourceRequest _Request)
        {
            //string ResultCheckBeforUpdate = Desktop.CheckBeforUpdate(_DataKey, _FormCollection);
            //if (ResultCheckBeforUpdate == "")
            //{
            dynamic FormCollectionData = JObject.Parse(_FormCollection);
            bool Result = Desktop.UpdateAsync(_DataKey, _FormCollection);
            string IDName = Desktop.DataTable[_DataKey].IDField().FieldName;
            return Json(true);
            //}
            //else
            //{
            //    return Alert("ویرایش ناموفق، " + ResultCheckBeforUpdate);
            //}
        }

        public ActionResult UpdateField(string _DataKey, int _ParentID, long _RecordID, string _FieldName, object _NewValue, [DataSourceRequest] DataSourceRequest _Request)
        {
            bool Result = false;

            if (_RecordID == 0)
                Result = Desktop.Insert(_DataKey, new string[] { _FieldName }, new object[] { _NewValue }) > 0 ? true : false;
            else
                Result = Desktop.UpdateField(_DataKey, _RecordID, _FieldName, _NewValue);

            return Json(Result);
        }

        public ActionResult Destroy(string _DataKey, int _ParentID, long _MasterProcessID, long _ProcessStep, long? RecordID, [DataSourceRequest] DataSourceRequest _Request, FormCollection _FormCollection, string SubDashboardID = "", string Category = "")
        {

            CoreObject EntryForm = CoreObject.Find(long.Parse(_DataKey));
            JsonResult jsonResult = new JsonResult();

            if (EntryForm.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات && _ParentID == 0 && EntryForm.ParentID != 0)
            {
                DataTable Table = (DataTable)Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()];
                Field IDField = Desktop.DataTable[_DataKey].IDField();
                long ID = long.Parse(_FormCollection[IDField.FieldName]);
                int RowIndex = -1;

                for (int Index = 0; Index < Table.Rows.Count; Index++)
                    if ((Table.Rows[Index][IDField.FieldName].ToString() != "" ? long.Parse(Table.Rows[Index][IDField.FieldName].ToString()) : 0) == ID)
                    {
                        RowIndex = Index;
                        break;
                    }
                Table.Rows.RemoveAt(RowIndex);
                Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = Table;
                Models.Attachment.DeleteDirectory(Models.Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString() + "/" + ID);
                return Json(_Request);
            }
            Desktop.Destroy(_DataKey, _FormCollection);
            Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = Desktop.Read(_DataKey, "", _ParentID);
           jsonResult = Json(((DataTable)Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()]).ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult CheckBeforDeleteRecord(long _DataKey, long _ParentID, long _RowID, string[] FormInputName,  object[] FormInputValue)
        {
            CoreObject EntryForm = CoreObject.Find(_DataKey);
            if (EntryForm.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات && _ParentID == 0 && EntryForm.ParentID != 0)
                return Json("");


            if (EntryForm.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm informationEntryForm = new InformationEntryForm(EntryForm);
                EntryForm = CoreObject.Find(informationEntryForm.RelatedTable);
            }
             
            string Alarm = Desktop.CheckBeforRunQuery(EntryForm.CoreObjectID, _RowID, CoreDefine.TableEvents.شرط_اجرای_حذف, FormInputName, FormInputValue);
            return Json(Alarm);
        }

        public ActionResult RemoveRow(string _DataKey, int _ParentID, long _MasterProcessID, long _ProcessStep, long RecordID)
        {
            CoreObject EntryForm = CoreObject.Find(long.Parse(_DataKey));

            if (EntryForm.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات && _ParentID == 0 && EntryForm.ParentID != 0 && Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()]!=null)
            {
                DataTable Table = (DataTable)Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()];
                Field IDField = Desktop.DataTable[_DataKey].IDField();
                int RowIndex = -1;

                for (int Index = 0; Index < Table.Rows.Count; Index++)
                    if ((Table.Rows[Index][IDField.FieldName].ToString() != "" ? long.Parse(Table.Rows[Index][IDField.FieldName].ToString()) : 0) == RecordID)
                    {
                        RowIndex = Index;
                        break;
                    }

                Table.Rows.RemoveAt(RowIndex);
                Desktop.SessionEditorGrid[_DataKey, _ParentID.ToString()] = Table;
                Models.Attachment.DeleteDirectory(Models.Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString() + "/" + RecordID);
                return Json("");
            }
            Desktop.Destroy(_DataKey, RecordID);
            return Json("");
        }

        public JsonResult CanEditRow(string _DataKey, int _ParentID, long _RecordID)
        {
            CoreObject coreObject = CoreObject.Find(long.Parse(_DataKey));
            if (coreObject.Entity == CoreDefine.Entities.جدول)
                return Json(true);
            else
            {
                PermissionInformationEntryForm permissionInfo = new PermissionInformationEntryForm(long.Parse(_DataKey), Referral.UserAccount.Permition);
                InformationEntryForm informationEntryForm = new InformationEntryForm(coreObject);

                if (permissionInfo.CanUpdateOnlyUserRegistry)
                {
                    long ID = long.Parse(Referral.DBRegistry.SelectField("SElect Count(1) from Insert_APMRegistry where UserAccountID=" + Referral.UserAccount.UsersID.ToString() + " and  CoreObjectID=" + informationEntryForm.RelatedTable.ToString() + " And RecordID= " + _RecordID.ToString()).ToString());
                    if (ID == 0)
                        return Json(false);
                }
                else if (permissionInfo.CanUpdateOneDey)
                {
                    long ID = long.Parse(Referral.DBRegistry.SelectField("SElect Count(1) from Insert_APMRegistry where RegistryDate=N'" + CDateTime.GetNowshamsiDate() + "' and  CoreObjectID=" + informationEntryForm.RelatedTable.ToString() + " And RecordID= " + _RecordID.ToString()).ToString());
                    if (ID == 0)
                        return Json(false);
                }
                else if (permissionInfo.CanUpdateThreeDey)
                {
                    long ID = long.Parse(Referral.DBRegistry.SelectField("SElect Count(1) from Insert_APMRegistry where ( RegistryDate between N'" + CDateTime.GetNowshamsiDate() + "' and N'" + CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -3) + "') and  CoreObjectID=" + informationEntryForm.RelatedTable.ToString() + " And RecordID= " + _RecordID.ToString()).ToString());
                    if (ID == 0)
                        return Json(false);
                }
                else if (permissionInfo.CanUpdateOneWeek)
                {
                    long ID = long.Parse(Referral.DBRegistry.SelectField("SElect Count(1) from Insert_APMRegistry where ( RegistryDate between N'" + CDateTime.GetNowshamsiDate() + "' and N'" + CDateTime.AddDay(CDateTime.GetNowshamsiDate(), -7) + "') and  CoreObjectID=" + informationEntryForm.RelatedTable.ToString() + " And RecordID= " + _RecordID.ToString()).ToString());
                    if (ID == 0)
                        return Json(false);
                }

            }
            return Json(true);
        }

        [HttpGet]
        public FileContentResult ExportPDF(string Datakey, string Filter, string Columns)
        {
            // creating data table and adding dummy data  
            DataTable DataTable = new DataTable();
            DataTable = Desktop.CachedTable[Datakey];
            string[] Column = Columns.Split(',');

            foreach (Field field in Desktop.DataFields[Datakey])
            {
                string Query = "";
                if (Array.IndexOf(Column, field.FieldName) > -1)
                {
                    switch (field.FieldType)
                    {
                        case CoreDefine.InputTypes.RelatedTable:
                            //DataTable.Columns[field.FieldName].DataType = typeof(string);
                            string Title = field.Title();
                            //DataTable.Columns.Add(Title, typeof(string));
                            SelectList ListData = (SelectList)Session[field.SystemName()];
                            foreach (DataRow row in DataTable.Rows)
                            {
                                try
                                {
                                    row[Title] = ListData.Where(item => item.Value == row[field.FieldName].ToString()).First().Text;
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            DataTable.Columns.Remove(field.FieldName);
                            break;
                    }
                }
            }

            byte[] filecontent = exportpdf(DataTable);
            string filename = "Sample_PDF_" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".pdf";
            return File(filecontent, "pdf", filename);
        }


        private byte[] exportpdf(DataTable dtEmployee)
        {

            // creating document object  
            MemoryStream ms = new MemoryStream();
            Rectangle rec = new Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec);
            doc.SetPageSize(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();

            //Creating paragraph for header  
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Fonts/IRANSans/ttf/IRANSansWeb.ttf");
            BaseFont bfntHead = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, true);
            //Font fntHead = new Font(bfntHead, 16, 1, BaseColor.BLUE);
            Font fntHead = new Font(bfntHead, 10, Font.NORMAL, BaseColor.BLUE);
            Paragraph prgHeading = new Paragraph();
            prgHeading.Alignment = Element.ALIGN_RIGHT;
            prgHeading.Add(new Chunk("داینامیک".ToUpper(), fntHead));
            doc.Add(prgHeading);

            //Adding paragraph for report generated by  
            Paragraph prgGeneratedBY = new Paragraph();
            BaseFont btnAuthor = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, true);
            Font fntAuthor = new Font(btnAuthor, 8, 2, BaseColor.BLUE);
            prgGeneratedBY.Alignment = Element.ALIGN_RIGHT;
            //prgGeneratedBY.Add(new Chunk("Report Generated by : ASPArticles", fntAuthor));  
            //prgGeneratedBY.Add(new Chunk("\nGenerated Date : " + DateTime.Now.ToShortDateString(), fntAuthor));  
            doc.Add(prgGeneratedBY);

            //Adding a line  
            Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            doc.Add(p);

            //Adding line break  
            doc.Add(new Chunk("\n", fntHead));

            //Adding  PdfPTable  
            PdfPTable table = new PdfPTable(dtEmployee.Columns.Count);

            for (int i = 0; i < dtEmployee.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dtEmployee.Columns[i].ColumnName);
                PdfPCell cell = new PdfPCell();
                cell.Phrase = new Phrase(cellText, new Font(bfntHead, 10, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"))));
                cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#C8C8C8"));
                //cell.Phrase = new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(grdStudent.HeaderStyle.ForeColor)));  
                //cell.BackgroundColor = new BaseColor(grdStudent.HeaderStyle.BackColor);  
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingBottom = 5;
                cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                table.AddCell(cell);
            }

            //writing table Data  
            for (int i = 0; i < dtEmployee.Rows.Count; i++)
            {
                for (int j = 0; j < dtEmployee.Columns.Count; j++)
                {
                    table.AddCell(dtEmployee.Rows[i][j].ToString());
                }
            }

            //int[] widths = new int[GridView1.Columns.Count];
            //for (int x = 0; x < GridView1.Columns.Count; x++)
            //{
            //    widths[x] = (int)GridView1.Columns[x].ItemStyle.Width.Value;
            //    string cellText = Server.HtmlDecode(GridView1.HeaderRow.Cells[x].Text);
            //    PdfPCell cell = new PdfPCell(new Phrase(12, cellText, font));
            //    cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#008000"));
            //    cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            //    table.AddCell(cell);
            //}
            //table.SetWidths(widths);

            //for (int i = 0; i < GridView1.Rows.Count; i++)
            //{
            //    if (GridView1.Rows[i].RowType == DataControlRowType.DataRow)
            //    {
            //        for (int j = 0; j < GridView1.Columns.Count; j++)
            //        {
            //            string cellText = Server.HtmlDecode(GridView1.Rows[i].Cells[j].Text);
            //            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new Phrase(12, cellText, font));
            //            //Set Color of Alternating row
            //            if (i % 2 != 0)
            //            {
            //                cell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#C2D69B"));
            //            }
            //            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            //            table.AddCell(cell);
            //        }
            //    }
            //}


            doc.Add(table);
            doc.Close();

            byte[] result = ms.ToArray();
            return result;

        }


        public static ActionResult Alert(string message)
        {
            string popupName = "localpopupNotification" + DateTime.Now.Ticks.ToString();
            string script = $@"
                    var {popupName} = $('#popupNotification').data('kendoNotification');
                    {popupName}.show('{message}', 'error');
                ";
            JavaScriptResult result = new JavaScriptResult();
            result.Script = script;
            return result;
        }

        public JsonResult CalAutoFillQuery(string DataKey, string ParentID, long RecordID, string[] FormInputName, ref object[] FormInputValue, string ElementName, ref string IsFixedItem)
        {
            if(FormInputValue==null || FormInputName==null) 
                return Json(new { data = FormInputValue, IsFixedItem = IsFixedItem });

            try
            { 
                CoreObject CoreObject = CoreObject.Find(long.Parse(DataKey));
                long TableID = long.Parse(DataKey);
                string[] InputName = new string[FormInputName.Length];
                IsFixedItem = "";
                List<CoreObject> FieldObjectList = new List<CoreObject>();

                ElementName = ElementName.Replace("_" + DataKey, "");

                for (int Index = 0; Index < FormInputName.Length; Index++)
                    InputName[Index] = FormInputName[Index].Replace("_" + DataKey, "");

                if (CoreObject.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                {
                    InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject);
                    FieldObjectList = CoreObject.FindChilds(informationEntryForm.CoreObjectID, CoreDefine.Entities.فیلد);
                    TableID = informationEntryForm.RelatedTable;

                }

                if(FieldObjectList.Count==0)
                    FieldObjectList = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد);

                CoreObject TableObject = CoreObject.Find(TableID);

                string Query = "Select ";
                List<CoreObject> ComputationalFieldObject = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد_محاسباتی);
                foreach (CoreObject Field in ComputationalFieldObject)
                {
                    ComputationalField computationalField = new ComputationalField(Field);
                    if (computationalField.Query.IndexOf("@" + ElementName) > -1 || ElementName != "")
                    {
                        Query += "(" + computationalField.Query + ") AS [" + Field.FullName + "]\n,";
                    }
                    else
                        IsFixedItem += Field.FullName + ",";
                }
                 
                List<Field> FieldList = new List<Field>();
                foreach (CoreObject FieldObject in FieldObjectList)
                {
                    Field Field = new Field(FieldObject);
                    if (!string.IsNullOrEmpty(Field.AutoFillQuery))
                    {
                        if (Field.AutoFillQuery.IndexOf("@" + ElementName) > -1 || ElementName != "")
                        {
                            FieldList.Add(Field);
                            Query += "(" + Field.AutoFillQuery + ") AS [" + FieldObject.FullName + "]\n,";
                        }
                        else
                            IsFixedItem += FieldObject.FullName + ",";
                    }
                }

                List<CoreObject> AttachObjectList = CoreObject.FindChilds(TableID, CoreDefine.Entities.ضمیمه_جدول);
                foreach (CoreObject FieldObject in AttachObjectList)
                {
                    TableAttachment Field = new TableAttachment(FieldObject);
                    if (!string.IsNullOrEmpty(Field.AutoFillQuery))
                    {
                        if (Field.AutoFillQuery.IndexOf("@" + ElementName) > -1 || ElementName != "")
                        {
                            Query += "(" + Field.AutoFillQuery + ") AS [" + FieldObject.FullName + "]\n,";
                        }
                        else
                            IsFixedItem += FieldObject.FullName + ",";
                    }
                }

                Query = Query.Substring(0, Query.Length - 1);


                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));

                switch (DataSourceInfo.DataSourceType)
                {
                    case CoreDefine.DataSourceType.SQLSERVER:
                        {
                            SQLDataBase DataBase = Referral.DBData;
                            if (Referral.DBData.ConnectionData.Source != DataSourceInfo.ServerName || Referral.DBData.ConnectionData.DataBase != DataSourceInfo.DataBase)
                                DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            string DefineVariablesQuery = DataBase.DefineVariablesQuery(TableObject.FullName, RecordID, InputName, FormInputValue) + Tools.NewLine;
                            foreach (CoreObject FieldObject in AttachObjectList)
                            {
                                if (Array.IndexOf(InputName, FieldObject.FullName) > -1)
                                {
                                    string value = FormInputValue[Array.IndexOf(InputName, FieldObject.FullName)].ToString();
                                    byte[] imageBytes = new byte[0];
                                    if (value.IndexOf("data:image") > -1)
                                    {
                                        string[] ImageInfo = value.Split(',');
                                        string Data = ImageInfo[1];
                                        string Extension = ImageInfo[0].Substring(ImageInfo[0].IndexOf('/') + 1, ImageInfo[0].IndexOf(';') - ImageInfo[0].IndexOf('/') - 1);
                                        Extension = Extension.ToLower() == "octet-stream" ? "png" : Extension;
                                        bool Result = false;
                                        imageBytes = Convert.FromBase64String(Data);
                                    }

                                    StringBuilder hex = new StringBuilder(imageBytes.Length * 2);
                                    foreach (byte b in imageBytes)
                                        hex.AppendFormat("{0:x2}", b);

                                    DefineVariablesQuery += "Declare @" + FieldObject.FullName + " as varbinary(MAX)=" + (value == "" ? "null" : "0x" + hex.ToString().ToUpper()) + Tools.NewLine;
                                }
                            }
                            Query = DefineVariablesQuery + "\n" + Tools.CheckQuery(Query);
                            DataTable DataTable = DataBase.SelectDataTable(Query);
                            if (DataTable != null)
                                if (DataTable.Rows.Count > 0)
                                {
                                    foreach (CoreObject ComputationalField in ComputationalFieldObject)
                                    {
                                        int index = Array.IndexOf(InputName, ComputationalField.FullName);
                                        ComputationalField _ComputationalField = new ComputationalField(ComputationalField);
                                        if (index > -1 && DataTable.Columns.IndexOf(ComputationalField.FullName) > -1)
                                        {
                                            if (_ComputationalField.FieldType == CoreDefine.InputTypes.Image)
                                            {
                                                if (DataTable.Rows[0][ComputationalField.FullName].ToString() == "System.Byte[]")
                                                {
                                                    byte[] FileByte = (byte[])DataTable.Rows[0][ComputationalField.FullName];
                                                    FormInputValue[index] = Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte));
                                                }
                                                else
                                                    FormInputValue[index] = Field.FormatImage(Referral.PublicSetting.AppLogo);
                                            }
                                            else
                                                FormInputValue[index] = DataTable.Rows[0][ComputationalField.FullName].ToString();
                                        }
                                    }

                                    foreach (Field FieldItem in FieldList)
                                    {
                                        int index = Array.IndexOf(InputName, FieldItem.FieldName);
                                        if (index > -1 && DataTable.Columns.IndexOf(FieldItem.FieldName) > -1)
                                        {
                                            if (FieldItem.FieldType == CoreDefine.InputTypes.Image)
                                            {
                                                if (DataTable.Rows[0][FieldItem.FieldName].ToString() == "System.Byte[]")
                                                {
                                                    byte[] FileByte = (byte[])DataTable.Rows[0][FieldItem.FieldName];
                                                    FormInputValue[index] = Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte));
                                                }
                                                else
                                                    FormInputValue[index] = Field.FormatImage(Referral.PublicSetting.AppLogo);
                                            }
                                            else
                                                FormInputValue[index] = DataTable.Rows[0][FieldItem.FieldName].ToString();
                                        }
                                    }


                                    foreach (CoreObject AttachField in AttachObjectList)
                                    {
                                        int index = Array.IndexOf(InputName, AttachField.FullName);
                                        TableAttachment _ComputationalField = new TableAttachment(AttachField);

                                        if (index > -1 && DataTable.Columns.IndexOf(AttachField.FullName) > -1)
                                        {
                                            if (DataTable.Rows[0][AttachField.FullName].ToString() == "System.Byte[]")
                                            {
                                                byte[] FileByte = (byte[])DataTable.Rows[0][AttachField.FullName];
                                                FormInputValue[index] = Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte));

                                                string Extension = "png";
                                                string path = Models.Attachment.MapTemporaryFilePath + DataKey + "/0";

                                                DirectoryInfo dir = new DirectoryInfo(path);
                                                if (!Directory.Exists(dir.FullName))
                                                    Directory.CreateDirectory(dir.FullName);

                                                foreach (FileInfo FileItem in dir.GetFiles())
                                                {
                                                    if (FileItem.Name.Replace(FileItem.Extension, "") == AttachField.FullName)
                                                    {
                                                        System.IO.File.Delete(FileItem.FullName);
                                                    }
                                                }
                                                System.IO.File.WriteAllText(path + "/" + AttachField.FullName + "." + Extension, Convert.ToBase64String((byte[])FileByte));
                                            }
                                            else if (DataTable.Rows[0][AttachField.FullName].ToString().IndexOf("data:image") > -1)
                                            {
                                                string Extension = "png";
                                                string path = Models.Attachment.MapTemporaryFilePath + DataKey + "/0";

                                                DirectoryInfo dir = new DirectoryInfo(path);
                                                if (!Directory.Exists(dir.FullName))
                                                    Directory.CreateDirectory(dir.FullName);

                                                foreach (FileInfo FileItem in dir.GetFiles())
                                                {
                                                    if (FileItem.Name.Replace(FileItem.Extension, "") == AttachField.FullName)
                                                    {
                                                        System.IO.File.Delete(FileItem.FullName);
                                                    }
                                                }
                                                System.IO.File.WriteAllText(path + "/" + AttachField.FullName + "." + Extension, DataTable.Rows[0][AttachField.FullName].ToString());
                                            }
                                            else
                                                FormInputValue[index] = "";// Field.FormatImage(Referral.PublicSetting.AppLogo);
                                        }
                                    }
                                }
                            break;
                        }
                    case CoreDefine.DataSourceType.MySql:
                        {
                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                            Query = DataBase.DefineVariablesQuery(TableObject.FullName, RecordID, InputName, FormInputValue);

                            break;
                        }
                    case CoreDefine.DataSourceType.ACCESS:
                        {
                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password);
                            Query = DataBase.DefineVariablesQuery(TableObject.CoreObjectID, RecordID, InputName, FormInputValue);

                            break;
                        }
                    case CoreDefine.DataSourceType.EXCEL:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(new { data = FormInputValue, IsFixedItem = IsFixedItem });
        }
        public JsonResult OnKeyDownElement(string DataKey, string ParentID, long RecordID, string[] FormInputName, object[] FormInputValue, string ElementName)
        {
            string IsFixedItem = "";
            CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, ElementName, ref IsFixedItem);
            return Json(new { data = FormInputValue, IsFixedItem = IsFixedItem });
        }

        public JsonResult SaveFormEditor(string DataKey, string ParentID, long RecordID, string[] FormInputName, object[] FormInputValue, long SearchDataKey = 0, bool SaveChilde = true, long ProcessID=0,long ProcessStepID=0,string[] JsonGrid=null,string [] GridName=null,bool IsImport=false,bool IsCalAutoFillQuery=false)
        {
            if (Referral.CoreObjects.Count == 0)
                Software.CoreReload();
            CoreObject Form = CoreObject.Find(long.Parse(DataKey));
            string Alarm = "";
            string ErrorMessage = "";
            string AlarmMessage = "";

            if (FormInputName != null)
                for (int Index = 0; Index < FormInputName.Length; Index++)
                    FormInputName[Index] = FormInputName[Index].Replace("_" + DataKey, "");
            else
            {
               FormInputName = new string[0];
               FormInputValue=new object[0];
            }


            if (RecordID == 0)
            {
                switch (Form.Entity)
                {
                    case CoreDefine.Entities.جدول:
                        {
                            Alarm = Desktop.CheckBeforRunQuery(Form.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, FormInputName, FormInputValue);
                            if (Alarm != "")
                                return Json(new { Message = Alarm, Record = "" });

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

                                string Query = (string)Session["SearchFormButtonClick_Query"];

                                Query = "\n" + Query + "\n" + searchForm.SearchAlarmQuery;

                                if(searchForm.SearchAlarmQuery!="")
                                    Alarm = Referral.DBData.SelectField(Tools.CheckQuery(Query)).ToString();
                                if (Alarm != "")
                                    return Json(new { Message = Alarm, Record = "" });

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

                                    Alarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, FormInputName, FormInputValue);
                                    if (Alarm != "")
                                        return Json(new { Message = Alarm, Record = "" }); 

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
                                            if(ParentID=="0" && informationEntryForm.GridEditMode == GridEditMode.InCell)
                                                CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

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

                                                        FormInputValue[index] = Table.Rows[Table.Rows.Count - 1][Item.FieldName].ToString();
                                                    }
                                                }
                                                catch(Exception ex)
                                                {

                                                }
                                            }

                                            int index2 = Array.IndexOf(FormInputName, IDField.FieldName);
                                            if (index2 > -1)
                                            FormInputValue[index2] = MaxID.ToString();
                                        }
                                    }

                                    Desktop.SessionEditorGrid[DataKey, ParentID] = Table;


                                    string SourcePath = Models.Attachment.MapTemporaryFilePath + DataKey + "/0";
                                    string DestinationPath = Models.Attachment.MapTemporaryFilePath + DataKey + "/" + MaxID;
                                    if(APM.Models.Attachment.CheckExistsDirectory(DestinationPath))
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
                                        if(FindeIndex < 0)
                                        {
                                            Array.Resize(ref TFormInputName, TFormInputName.Length + 1);
                                            Array.Resize(ref TFormInputValue, TFormInputValue.Length + 1);
                                            TFormInputName[TFormInputName.Length - 1] = ExternalFieldCore.FullName;
                                            TFormInputValue[TFormInputValue.Length - 1] = ParentID;
                                        }
                                        else
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

                                CoreObject _ExternalField = CoreObject.Find(informationEntryForm.ExternalField);
                                int FindIndex = Array.IndexOf(FormInputName, _ExternalField.FullName);

                                if(FindIndex> -1)
                                        FormInputValue[FindIndex] = ParentID;
                                else
                                {
                                    if(!string.IsNullOrEmpty(ParentID) && ParentID != "0")
                                    {
                                        Array.Resize(ref FormInputValue, FormInputValue.Length+1);
                                        Array.Resize(ref FormInputName, FormInputName.Length+1);
                                        FormInputName[FormInputName.Length - 1] = _ExternalField.FullName;
                                        FormInputValue[FormInputValue.Length - 1] = ParentID;
                                    }
                                }

                                string IsFixedItem = "";
                                if(IsImport)
                                    CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

                                Alarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, FormInputName, FormInputValue, DataKey);
                                if (Alarm != "")
                                    return Json(new { Message = Alarm, Record = "" });

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

                                    if (JsonGrid != null)
                                    {
                                        for (int GridIndex=0;GridIndex<JsonGrid.Length;GridIndex++)
                                        {
                                            var Grid= JsonGrid[GridIndex];
                                            if (Grid != "[]")
                                            {
                                                string SubGridDataKey = GridName[GridIndex].Replace("DetailMainGrid","").Replace("MainGrid", ""); 
                                                DataTable SubGridData = DataConvertor.JsonStringToDataTable(Grid);
                                                string[] ColumnName = new string[SubGridData.Columns.Count-1];
                                                int ColumnCounter = 0; 
                                                TableCore = CoreObject.Find(Desktop.DataInformationEntryForm[SubGridDataKey].RelatedTable);

                                                CoreObject ExternalField = CoreObject.Find(Desktop.DataInformationEntryForm[SubGridDataKey].ExternalField);
                                                Field IDField = Desktop.DataTable[SubGridDataKey].IDField();
                                                foreach (DataColumn Column in SubGridData.Columns)
                                                    if(Column.ColumnName != IDField.FieldName)
                                                    ColumnName[ColumnCounter++] = Column.ColumnName;

                                                foreach (DataRow Row in SubGridData.Rows)
                                                {
                                                    object[] ColumnValue = new object[ColumnName.Length];
                                                    if (SubGridData.Columns.IndexOf(ExternalField.FullName) > -1)
                                                        Row[ExternalField.FullName] = ParentRowID;
                                                    else
                                                    {
                                                        if (Array.IndexOf(ColumnName,ExternalField.FullName) > -1)
                                                            ColumnValue[Array.IndexOf(ColumnName, ExternalField.FullName)] = ParentRowID;
                                                        else
                                                        { 
                                                            Array.Resize(ref ColumnValue, ColumnValue.Length + 1);
                                                            Array.Resize(ref ColumnName, ColumnName.Length + 1);
                                                            ColumnName[ColumnName.Length - 1] = ExternalField.FullName;
                                                            ColumnValue[ColumnValue.Length - 1] = ParentRowID;
                                                        }
                                                    }

                                                    try
                                                    {
                                                        for (int ColumnIndex = 0; ColumnIndex < ColumnName.Length; ColumnIndex++)
                                                        {
                                                            if (SubGridData.Columns.IndexOf(ColumnName[ColumnIndex]) > -1)
                                                            { 
                                                                if (Row[ColumnName[ColumnIndex]].ToString() != "null")
                                                                    ColumnValue[ColumnIndex] = Row[ColumnName[ColumnIndex]];
                                                                else
                                                                    switch (Row[ColumnName[ColumnIndex]].GetType().ToString())
                                                                    {
                                                                        case "Boolean":
                                                                        case "Decimal":
                                                                        case "Int64":
                                                                            {
                                                                                ColumnValue[ColumnIndex] = 0;
                                                                                break;
                                                                            }
                                                                        default:
                                                                            {
                                                                                ColumnValue[ColumnIndex] = "";
                                                                                break;
                                                                            }
                                                                    }
                                                            }
                                                        }

                                                        Alarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, ColumnName, ColumnValue);
                                                        if (Alarm == "")
                                                        {
                                                            long NewRowID = Desktop.Create(Desktop.DataInformationEntryForm[SubGridDataKey].CoreObjectID.ToString(), ColumnName, ColumnValue, Row[IDField.FieldName].ToString());
                                                            if (NewRowID > 0)
                                                            {
                                                                if (Referral.PublicSetting.FileSavingPath == "")
                                                                {

                                                                }
                                                                else
                                                                {
                                                                    string SourcePath = Models.Attachment.MapTemporaryFilePath + Desktop.DataInformationEntryForm[SubGridDataKey].CoreObjectID.ToString() + "/" + Row[IDField.FieldName];
                                                                    string DestinationPath = Models.Attachment.MapFileSavingAttachmentPath + Desktop.DataInformationEntryForm[SubGridDataKey].RelatedTable + "/" + NewRowID;
                                                                    Models.Attachment.SaveAttachment(new DirectoryInfo(SourcePath), Directory.CreateDirectory(DestinationPath), Desktop.DataInformationEntryForm[SubGridDataKey].RelatedTable, NewRowID);
                                                                }
                                                            }
                                                        }
                                                        else
                                                            ErrorMessage += Alarm + "\n";

                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                            }
                                            }

                                        }

                                    }

                                    RecordID = ParentRowID;

                                    if(ProcessID>0&& ProcessStepID>0 && !IsImport) 
                                        Desktop.SaveProcessStep(ProcessID, ProcessStepID, CoreObject.Find(informationEntryForm.RelatedTable).CoreObjectID, RecordID,long.Parse(DataKey)); 
                                }

                                if(!IsImport)
                                { 
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

                            }
                            break;
                        }
                }
            }
            else
            {
                bool IsTempGrid = false;
                if ((Form.ParentID != 0 && ParentID == "0") && Form.Entity != CoreDefine.Entities.جدول)
                {
                    List<Field> TableFields = new List<Field>();
                    DataTable Table = (DataTable)Desktop.SessionEditorGrid[DataKey, ParentID];
                    if(Table != null)
                    {
                        IsTempGrid = true;
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

                        InformationEntryForm informationEntryForm = new InformationEntryForm(Form);
                        string IsFixedItem = "";
                        if (IsCalAutoFillQuery)
                            CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

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

                                FormInputValue[index] = Table.Rows[Table.Rows.Count - 1][Item.FieldName].ToString();
                            }
                        }
                        Desktop.SessionEditorGrid[DataKey, ParentID] = Table;

                    }
                }

                if(!IsTempGrid)
                {
                    long TableID = Form.CoreObjectID; 
                    if (Form.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                    {
                        InformationEntryForm InformationEntryForm = new InformationEntryForm(Form);
                        TableID = InformationEntryForm.RelatedTable; 

                        if (InformationEntryForm.ExternalField>0)
                        { 
                            CoreObject _ExternalField = CoreObject.Find(InformationEntryForm.ExternalField);
                            int FindIndex = Array.IndexOf(FormInputName, _ExternalField.FullName);

                            if (FindIndex > -1)
                            {
                                if (FormInputValue[FindIndex].ToString() == "0" || FormInputValue[FindIndex].ToString() == "")
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
                        }
                        string IsFixedItem = "";
                        if (IsCalAutoFillQuery) 
                            CalAutoFillQuery(DataKey, ParentID, RecordID, FormInputName, ref FormInputValue, "", ref IsFixedItem);

                    }


                    Alarm = Desktop.CheckBeforRunQuery(TableID, RecordID, CoreDefine.TableEvents.شرط_اجرای_ویرایش, FormInputName, FormInputValue);
                    if (Alarm != "")
                        return Json(new { Message = Alarm, Record = "" });

                    bool result = Desktop.Update(TableID, RecordID, FormInputName, FormInputValue,DataKey);

                    //AlarmMessage = Desktop.CheckBeforRunQuery(TableID, RecordID, CoreDefine.TableEvents.هشدار_قبل_از_ویرایش, FormInputName, FormInputValue);
                    AlarmMessage += Desktop.CheckBeforRunQuery(long.Parse(DataKey), RecordID, CoreDefine.TableEvents.هشدار_قبل_از_ویرایش, FormInputName, FormInputValue);

                    if (ProcessID > 0 && ProcessStepID > 0)
                        Desktop.SaveProcessStep(ProcessID, ProcessStepID, TableID, RecordID, long.Parse(DataKey));

                    DataTable dataTable = Desktop.SelectRecord(DataKey, RecordID);
                    if (dataTable.Rows.Count > 0)
                    {
                        List<CoreObject> AttachmentCore = CoreObject.FindChilds(TableID, CoreDefine.Entities.ضمیمه_جدول);
                        for (int i = 0; i < AttachmentCore.Count; i++)
                        {
                            int Findindex = Array.IndexOf(FormInputName, AttachmentCore[i].FullName.ToString());
                            if (Findindex != -1)
                                FormInputValue[Findindex] = Attachment.GetFileByte(TableID.ToString(), RecordID.ToString(), AttachmentCore[i].FullName.ToString());
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
            }
            return Json(new { Message = Alarm, Record = FormInputValue, RecordID = RecordID ,FieldName=FormInputName, AlarmMessage = AlarmMessage });
        }

        public JsonResult SaveGridJson(string DataKey, string ParentID, string GridJSON, bool IsDisplay)
        {
            DataTable DataTable = (DataTable)Desktop.SessionEditorGrid[DataKey, ParentID];
            if (DataTable != null)
                DataTable.Rows.Clear();

            if (GridJSON != "[]" && IsDisplay)
            {
                DataTable table = DataConvertor.JsonStringToDataTable(GridJSON);
                string[] FormInputName = new string[table.Columns.Count];
                object[] FormInputValue = new object[table.Columns.Count];

                for (int i = 0; i < FormInputName.Length; i++)
                {
                    FormInputName[i] = table.Columns[i].ColumnName;
                }

                Field IDField = Desktop.DataTable[DataKey].IDField();

                long RecordID = 0;

                for (int index = 0; index < table.Rows.Count; index++)
                {
                    for (int i = 0; i < FormInputValue.Length; i++)
                    {
                        FormInputValue[i] = table.Rows[index][i];
                    }
                    RecordID = 0;// long.Parse(table.Rows[index][IDField.FieldName].ToString());
                    string Error = SaveFormEditor(DataKey, ParentID, RecordID, FormInputName, FormInputValue, 0).Data.ToString();
                    if (Error.Split(',')[0].Replace("{ Message = ", "") != "")
                        return Json(Error.Split(',')[0].Replace("{ Message = ", ""));
                }
            }

            return Json("");
        }
        public ActionResult ReadValuesWithSpecialWord(string _SpecialWordFullName,string DeclareQuery, [DataSourceRequest] DataSourceRequest _Request)
        { 

            var jsonResult = Json(DataConvertor.FillSelectListWithQuery(_SpecialWordFullName, DeclareQuery), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult ReadValuesWithQuery(string SpecialWordFullName,[DataSourceRequest] DataSourceRequest _Request)
        { 
            var jsonResult = Json(DataConvertor.FillSelectListWithQuery(SpecialWordFullName), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //[OutputCache(Duration = 60 * 5, VaryByParam = "_DataKey,_FieldName,_Request,RelatedField")]
        public ActionResult ReadExternalIDValues(string _DataKey, string _FieldName, [DataSourceRequest] DataSourceRequest _Request, long RelatedField = 0,bool IsReload=false,string TextRelatedField="")
        {
            JsonResult jsonResult;

            if (Referral.CoreObjects.Count == 0)
                Software.CoreReload(); 

            if (_DataKey == "0" && _FieldName == "SearchRegistryTable_UserAccountID")
            {
                SelectList ListData = DataConvertor.ToSelectList(Referral.DBData.SelectDataTable("Select شناسه , نام_و_نام_خانوادگی From کاربر"));
                jsonResult = Json(ListData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

            if (Desktop.DataFields[_DataKey] == null)
                Desktop.StartupSetting(_DataKey);

            CoreObject ItemObject = CoreObject.Find(long.Parse(_DataKey));
            if (_FieldName.IndexOf("SearchField_") > -1)
            {
                _FieldName = _FieldName.Replace("SearchField_", "");
                string[] FieldArr = _FieldName.Split('_');
                _FieldName = _FieldName.Replace("_" + FieldArr[FieldArr.Length - 5]+"_" + FieldArr[FieldArr.Length - 4] + "_" + FieldArr[FieldArr.Length - 3] + "_" + FieldArr[FieldArr.Length - 2] + "_" + FieldArr[FieldArr.Length - 1], "") ;
            }
            else
                _FieldName = _FieldName.Replace("_" + _DataKey, "").Replace("RegistryTable_", "");

            Field Item = Desktop.DataFields[_DataKey].Find(x => x.FieldName == _FieldName);
            if(Item== null)
            {
                if(ItemObject.Entity==CoreDefine.Entities.جدول)
                {
                    List<CoreObject> FieldCoreList = CoreObject.FindChilds(ItemObject.CoreObjectID, CoreDefine.Entities.فیلد);
                    CoreObject FieldCore= FieldCoreList.Find(x=>x.FullName == _FieldName);
                    if(FieldCore!=null)
                        Item=new Field(FieldCore);
                    else
                    {
                        List<SelectListItem> Output = new List<SelectListItem>(); 
                        Output.Add(new SelectListItem() { Value = "0", Text = " ", Selected = true }); 
                        jsonResult = Json(new SelectList(Output, "Value", "Text", 0), JsonRequestBehavior.AllowGet); 
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult; 
                    }
                }
            }
            if(RelatedField>0)
            {
                string Query = Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(Item)) + (Item.ViewCommand.Trim() != "" ? " And " : " WHERE ") + CoreObject.Find(Item.RelatedFieldCommand).FullName + " = " + RelatedField;
                SelectList ListData = DataConvertor.ToSelectList(Referral.DBData.SelectDataTable(Query));
                jsonResult = Json(ListData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            if(TextRelatedField!="")
            {
                string Query = Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(Item)) + (Item.ViewCommand.Trim() != "" ? " And " : " WHERE ") + CoreObject.Find(Item.RelatedFieldCommand).FullName + " = N'" + TextRelatedField+"'";
                SelectList ListData = DataConvertor.ToSelectList(Referral.DBData.SelectDataTable(Query));
                jsonResult = Json(ListData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            //else
            //    DataConvertor.FillSelectList(new List<Field> { Item });

            //if (Item.RelatedTable == 0 && Item.ViewCommand != "")
            //    jsonResult = Json(Session["CachedListOf" + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((Item.ViewCommand + Item.SpecialValue)))], JsonRequestBehavior.AllowGet);
            //else
            //jsonResult = Json(Session[Item.SystemName()], JsonRequestBehavior.AllowGet);
            jsonResult = Json(DataConvertor.FillSelectList(Item), JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult ReadAutoCompleteValues(string _DataKey, string _FieldName, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult;

            if (Desktop.DataFields[_DataKey] == null)
                Desktop.StartupSetting(_DataKey);

            CoreObject ItemObject = CoreObject.Find(long.Parse(_DataKey));
            _FieldName = _FieldName.Replace("_" + _DataKey, "");

            Field Item = Desktop.DataFields[_DataKey].Find(x => x.FieldName == _FieldName);
            if (Item != null)
            {
                DataConvertor.FillAutoCompleteList(new List<Field> { Item });

                if (Item.RelatedTable == 0 && Item.ViewCommand != "")
                    jsonResult = Json(Session["CachedListOf" + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((Item.ViewCommand + Item.SpecialValue)))], JsonRequestBehavior.AllowGet);
                else
                    jsonResult = Json(Session[Item.SystemName()], JsonRequestBehavior.AllowGet);

                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            return Json("");
        }

        public ActionResult ReadExternalIDValuesForTable(long _TableID, string _FieldName, [DataSourceRequest] DataSourceRequest _Request, long RelatedField = 0)
        {
            JsonResult jsonResult;

            CoreObject Core = CoreObject.Find(_TableID);


            if (Core.Entity == CoreDefine.Entities.پارامتر_گزارش)
            {
                ReportParameter Parameter = new ReportParameter(Core);

                if (RelatedField > 0)
                {
                    ReportParameter ReleatedReportParameter = new ReportParameter(CoreObject.Find(Parameter.RelatedField));
                    string Query = Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(new Field() { RelatedTable = Parameter.RelatedTable })) + (Parameter.ViewCommand.Trim() != "" ? " And " : " WHERE ") + CoreObject.Find(Parameter.RelatedFieldCommand).FullName + " = " + RelatedField;
                    SelectList ListData = DataConvertor.ToSelectList(Referral.DBData.SelectDataTable(Query));
                    jsonResult = Json(ListData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else 
                    DataConvertor.FillRelatedTableReportParameter(Parameter);
                jsonResult = Json(Session[Parameter.SystemName()], JsonRequestBehavior.AllowGet);




            }
            else
            {
                if (Desktop.TableDataFields[_TableID.ToString()] == null)
                    Desktop.StartupSettingTableDataFields(_TableID);

                Field Item = Desktop.TableDataFields[_TableID.ToString()].Find(x => x.FieldName == _FieldName);
                DataConvertor.FillSelectList(new List<Field> { Item });

                if (Item.RelatedTable == 0 && Item.ViewCommand != "")
                    jsonResult = Json(Session["CachedListOf" + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((Item.ViewCommand + Item.SpecialValue)))], JsonRequestBehavior.AllowGet);
                else
                    jsonResult = Json(Session[Item.SystemName()], JsonRequestBehavior.AllowGet);
            }


            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ReloadExternalIDValues(string _DataKey, string _FieldName, int _Index, string _GridName)
        {
            Field Item = Desktop.DataFields[_DataKey].Find(x => x.FieldName == _FieldName);
            DataConvertor.FillSelectList(new List<Field> { Item });
            DataConvertor.ForeingKeyDataPack DataPack = new DataConvertor.ForeingKeyDataPack(_DataKey, _FieldName, _Index, _GridName, (SelectList)Session[Item.SystemName()]);
            var jsonResult = Json(DataPack, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetIcon()
        {
            var jsonResult = Json(Icon.KendoIcon(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult CoreObjectList(string Entitiy, string ParentID, [DataSourceRequest] DataSourceRequest _Request)
        {
            DataConvertor.FillCoreList(Entitiy, ParentID);
            var jsonResult = Json(Session["CachedListOfCoreList" + Entitiy + ParentID], JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ProcessActionType([DataSourceRequest] DataSourceRequest _Request)
        {
            List<SelectListItem> ActionTypeList = new List<SelectListItem>() {
              new SelectListItem() {Text = "هیچکدام", Value = CoreDefine.ProcessStepActionType.خالی.ToString() },
              new SelectListItem() {Text = "شروع", Value = CoreDefine.ProcessStepActionType.شروع.ToString()},
              new SelectListItem() {Text = "عملیات", Value = CoreDefine.ProcessStepActionType.عملیات.ToString()},
              new SelectListItem() {Text = "شرط", Value = CoreDefine.ProcessStepActionType.شرط.ToString()},
              new SelectListItem() {Text = "پایان", Value = CoreDefine.ProcessStepActionType.پایان.ToString()},
            };
            var jsonResult = Json(ActionTypeList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult NotificationType([DataSourceRequest] DataSourceRequest _Request)
        {
            List<SelectListItem> NotificationTypeList = new List<SelectListItem>() {
              new SelectListItem() {Text = "سیستمی", Value = CoreDefine.NotificationType.سیستمی.ToString() },
              new SelectListItem() {Text = "پیامک", Value = CoreDefine.NotificationType.پیامک.ToString()},
              new SelectListItem() {Text = "ایمیل", Value = CoreDefine.NotificationType.ایمیل.ToString()},
            };
            var jsonResult = Json(NotificationTypeList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ProcessRecordType([DataSourceRequest] DataSourceRequest _Request)
        {
            List<SelectListItem> ProcessRecordTypeList = new List<SelectListItem>() {
              new SelectListItem() {Text = "خالی", Value = CoreDefine.ProcessStepRecordType.خالی.ToString() },
              new SelectListItem() {Text = "جدید", Value = CoreDefine.ProcessStepRecordType.جدید.ToString()},
              new SelectListItem() {Text = "ویرایش", Value = CoreDefine.ProcessStepRecordType.ویرایش.ToString()},
            };
            var jsonResult = Json(ProcessRecordTypeList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult AttachmentUploadType([DataSourceRequest] DataSourceRequest _Request)
        {
            List<SelectListItem> AttachmentUploadTypeList = new List<SelectListItem>() {
              new SelectListItem() {Text = "بارگذاری", Value = CoreDefine.AttachmentUploadType.بارگذاری.ToString() },
              new SelectListItem() {Text = "اسکن", Value = CoreDefine.AttachmentUploadType.اسکن.ToString()},
              new SelectListItem() {Text = "وبکم", Value = CoreDefine.AttachmentUploadType.وبکم.ToString()},
            };
            var jsonResult = Json(AttachmentUploadTypeList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult StartProcessStep(long ProcessID)
        {
            long InformationEntryFormID = 0;
            long NextProcessStepID = 0;
            string InformationEntryFormTitle= string.Empty;
            Tools.GetNextProcessStep(ProcessID, 0,ref NextProcessStepID, ref InformationEntryFormID, ref InformationEntryFormTitle); 
            return Json(new { NextProcessStepID= NextProcessStepID, InformationEntryFormID = InformationEntryFormID, InformationEntryFormTitle= InformationEntryFormTitle });
        }
          
        [HttpPost]
        public async Task<JsonResult> TableButtonClick(long RowID, long ButtonID, long DataKey, long ParentID)
        {
            CoreObject ButtonObject = CoreObject.Find(ButtonID);
            CoreObject TableObject = CoreObject.Find(ButtonObject.ParentID);
            TableButton TableButton = ButtonObject.Entity== CoreDefine.Entities.فیلد_نمایشی ? new TableButton(new DisplayField(ButtonObject)): new TableButton(ButtonObject);
            DataSourceInfo DataSourceInfo = new DataSourceInfo();
            string Declare = "";
            string Query = "";
            string ErrorMessage = "";
            bool RunResult = false;
            string[] ColumnNames=new string[0];
            object[] _Values=new object[0];

            if (TableObject.Entity == CoreDefine.Entities.جدول)
            {
                DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
            }
            else
            {
                InformationEntryForm informationEntryForm = new InformationEntryForm(TableObject);
                TableObject = CoreObject.Find(informationEntryForm.RelatedTable);
                DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
            }

            if (TableButton.RelatedWebService>0 || !string.IsNullOrEmpty(TableButton.ExecutionConditionQuery) || !string.IsNullOrEmpty(TableButton.Query) || !string.IsNullOrEmpty(TableButton.ReceivingQuery) || !string.IsNullOrEmpty(TableButton.TitleQuery) || !string.IsNullOrEmpty(TableButton.BodyMessageQuery))
            {
                Declare = Referral.DBData.DefineVariablesQuery(TableObject.FullName, RowID,ref ColumnNames,ref _Values);
            }

            if (TableButton.ExecutionConditionQuery != "" && TableButton.ExecutionConditionQuery != null)
                ErrorMessage = Desktop.SelectField(DataSourceInfo, Declare + Tools.NewLine + Tools.CheckQuery(TableButton.ExecutionConditionQuery)).ToString();

            if (ErrorMessage == "")
            {
                if (TableButton.TableButtonEventsType == CoreDefine.TableButtonEventsType.تولید_کلید_عمومی_مالیاتی || TableButton.TableButtonEventsType == CoreDefine.TableButtonEventsType.بروزرسانی_کالا_مالیات)
                {
                    Session["TableButtonEventsType"] = DataKey.ToString() + "_" + ParentID.ToString() + "_" + RowID.ToString();
                }
                else if (TableButton.TableButtonEventsType == CoreDefine.TableButtonEventsType.ارسال_صورتحساب_به_سامانه_مودیان)
                {
                    SQLDataBase sQLDataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);


                    Record record = new Record(sQLDataBase, "SELECT لینک_سرویس_مایسا,لینک_سرویس_سازمان_مالیات" +
                                                             "  FROM  صورتحساب inner join درخواست_گواهی_امضا_الکترونیکی on صورتحساب.درخواست_گواهی = درخواست_گواهی_امضا_الکترونیکی.شناسه" +
                                                             "  Where صورتحساب.شناسه = " + RowID);
                    string PasswordJson = System.Text.Json.JsonSerializer.Serialize(Encoding.ASCII.GetBytes(DataSourceInfo.Password));

                    using (var client = new HttpClient())
                    {
                        //string Url = record.Field("لینک_سرویس_مایسا") + "RowID=" + RowID + "&ServerName=" + DataSourceInfo.ServerName + "&DatabaseName=" + DataSourceInfo.DataBase + "&UserName=" + DataSourceInfo.UserName + "&Password=" + PasswordJson + "&Url=" + record.Field("لینک_سرویس_سازمان_مالیات").ToString();
                        string Url = "https://localhost:7256/TAXSERVICE?" + "RowID=" + RowID + "&ServerName=" + DataSourceInfo.ServerName + "&DatabaseName=" + DataSourceInfo.DataBase + "&UserName=" + DataSourceInfo.UserName + "&Password=" + PasswordJson + "&Url=HTTPS://TP.TAX.GOV.IR/REQ/API/";

                        try
                        {
                            var response = client.GetAsync(Url);
                            response.Wait();
                            var result = response.Result;
                            //string textResult = response.Content.ReadAsStringAsync(); 
                            if (result.IsSuccessStatusCode)
                            {
                                var jsonResponse = result.Content.ReadAsStringAsync().Result;
                                //readTask.Wait();
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
                else if(TableButton.TableButtonEventsType==CoreDefine.TableButtonEventsType.ارسال_ایمیل)
                {

                    MailCore mailCore = new MailCore()
                    {
                        EMail = TableButton.EMail,
                        EMailUserName = TableButton.EMailUserName,
                        EMailPassword = TableButton.EMailPassWord,
                        EMailServer = TableButton.EMailServer,
                        EMailPort = TableButton.EMailPort,
                        EnableSsl = TableButton.EnableSsl,
                        ReceivingUsers= TableButton.ReceivingUsers,
                        ReceivingRole= TableButton.ReceivingRole,
                        InsertingUser= TableButton.InsertingUser,
                        ReceivingQuery= TableButton.ReceivingQuery,
                        SendAttachmentFile= TableButton.SendAttachmentFile,
                        UsePublickEmail= TableButton.UsePublickEmail,
                        SendReport= TableButton.SendReport,
                        Title= TableButton.Title,
                        TitleQuery= TableButton.TitleQuery,
                        BodyMessage= TableButton.BodyMessage,
                        BodyMessageQuery= TableButton.BodyMessageQuery
                    };
                  mailCore.SyncSendMail(Declare, DataKey.ToString(), RowID.ToString(), TableObject.CoreObjectID.ToString(), ColumnNames,_Values);
         
                }
                else if(TableButton.TableButtonEventsType== CoreDefine.TableButtonEventsType.اجرای_وب_سرویس)
                {
                    if(TableButton.RelatedWebService>0)
                    {
                        string postData = "";                      
                        WebServiceRequest webServiceRequest = new WebServiceRequest() { Method = new WebServiceMethod().Post};  
                        webServiceRequest.GenarateUrlFromWebService(TableButton.RelatedWebService, Declare, ColumnNames, _Values,ref postData);
                        webServiceRequest.SendRequest();
                    }
                }

                if (TableButton.Query != "")
                {
                    Query = Declare + Tools.NewLine + Tools.CheckQuery(TableButton.Query);
                    RunResult = Desktop.ExecuteQuery(DataSourceInfo, Query);
                }
                else
                    RunResult = (TableButton.Query == "" && ErrorMessage == "") ? true : RunResult;

            }

            if (TableButton.IsReloadGrid && ErrorMessage=="")
            {
                Referral.DBRegistry.Insert("Update_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "CoreObjectID", "RecordID", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" }, new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableObject.FullName, TableObject.CoreObjectID, RowID, Referral.UserAccount.IP, DataSourceInfo.ServerName, DataSourceInfo.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });
            }
            return Json(new { ISReloadGrid = TableButton.IsReloadGrid, RunResult = RunResult, ErrorMessage = ErrorMessage });
        }

        public JsonResult ClearGrid(string Grids)
        {
            string[] element = Grids.Split(',');
            foreach (string Item in element)
            {
                try
                { 
                    string DataKey = "0";
                    long ParentId = 0;

                    if (Item.IndexOf("DetailMainGrid") > -1)
                    {
                        DataKey = Item.Replace("DetailMainGrid", "").Split('_')[0];
                        ParentId = long.Parse(Item.Replace("DetailMainGrid", "").Split('_')[1]);
                    }
                    else
                        DataKey = Item.Replace("MainGrid", "");
                    Desktop.BeforLoadEditorform(DataKey, ParentId);


                    DataTable Table = Desktop.CachedTable[DataKey];
                    DataTable Table2 = (DataTable)Desktop.SessionEditorGrid[DataKey, ParentId.ToString()];
                    if (Table != null)
                    {
                        Table.Rows.Clear();
                        Desktop.SessionEditorGrid[DataKey, ParentId.ToString()] = Table;
                    }
                    else if(Table2 != null)
                    {
                        Table2.Rows.Clear();
                        Desktop.SessionEditorGrid[DataKey, ParentId.ToString()] = Table2;
                    }
                }
                catch(Exception ex)
                {

                }
            }
            return Json("");
        }

        public JsonResult ClearEditorForm(string DataKey, string[] FormInputName, object[] FormInputValue)
        {

            CoreObject Form = CoreObject.Find(long.Parse(DataKey));

            if (FormInputName != null)
                for (int Index = 0; Index < FormInputName.Length; Index++)
                    FormInputName[Index] = FormInputName[Index].Replace("_" + DataKey, "");

            foreach (Field Item in Desktop.DataFields[DataKey])
            {
                int FindIndex = Array.IndexOf(FormInputName, Item.FieldName);
                if (FindIndex > -1)
                    FormInputValue[FindIndex] = Tools.GetDefaultValue(Item.DefaultValue);
            }
            return Json(FormInputValue);
        }

        public JsonResult SearchByFieldItem(string DataKey, string ParentID, long SearchDataKey, string[] SearchFieldItem, string[] SearchFieldOperator, string[] SearchFieldValue,string EditorFormDataKey, string[] FormInputName, string[] FormInputValue, string GridJSON, string GridMode, bool CleareGridAfterSearch,long ProcessID = 0 ,long ProcessStepID = 0)
        {
            string Query = "";
            string ErrorMessage =string.Empty;
            string AlarmMessage =string.Empty;

           CoreObject SearchDataKeyCore = CoreObject.Find(SearchDataKey);
            SearchForm searchForm = new SearchForm();
            if (SearchDataKeyCore.Entity == CoreDefine.Entities.فیلد_جستجو)
                searchForm = new SearchForm(CoreObject.Find(SearchDataKeyCore.ParentID));
            else
                searchForm = new SearchForm(SearchDataKeyCore);

            List<CoreObject> SearchFieldCore = CoreObject.FindChilds(searchForm.CoreObjectID, CoreDefine.Entities.فیلد_جستجو);
            CoreObject formCore = CoreObject.Find(SearchDataKeyCore.ParentID);
            CoreObject TableCore = CoreObject.Find(searchForm.RelatedTable);

            DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
            DataTable DataInfo = new DataTable();
            if(SearchFieldItem!=null)
            foreach (CoreObject Item in SearchFieldCore)
            {
                SearchField searchField = new SearchField(Item);
                Field field = new Field(CoreObject.Find(searchField.RelatedField));
                int FindIndex = Array.IndexOf(SearchFieldItem, field.FieldName);
                Query += "\nDeclare @" + Item.FullName + " AS ";
                switch (field.FieldType)
                {
                    case CoreDefine.InputTypes.Number:
                    case CoreDefine.InputTypes.RelatedTable:
                        {
                            Query += " Bigint = " + SearchFieldValue[FindIndex];
                            break;
                        }
                    case CoreDefine.InputTypes.TwoValues:
                        {
                            Query += " Bit = " + (SearchFieldValue[FindIndex].ToString().ToLower() == "true" ? "1" : "0");
                            break;
                        }
                    default:
                        {
                            Query += " Nvarchar(400) = N'" + SearchFieldValue[FindIndex] + "'";
                            break;
                        }
                }

                SearchFieldItem[FindIndex] = Item.CoreObjectID.ToString();
            }

            if (searchForm.Query == "")
            {

                Query += "\n\nSelect\n";
                if(string.IsNullOrEmpty(Desktop.DataInformationEntryForm[DataKey].Query))
                {
                    Query += " * From " + TableCore.FullName;
                }
                else
                {
                    Query += Desktop.DataInformationEntryForm[DataKey].Query;
                }
                Query = Tools.ConvertToSQLQuery(Query);
                Table table = new Table(TableCore);
                string IdentityField = table.IDField().FieldName;

                if (ProcessID > 0 && ProcessStepID == 0)
                {
                    int FindIndex = Query.LastIndexOf("FROM");
                    if (FindIndex > -1)
                    {
                        string SubQuery = Query.Substring(FindIndex, Query.Length - FindIndex);
                        Query = Query.Substring(0, FindIndex);
                        Query += ",[مراحل_فرآیند].[مرحله_فرآیند]  AS آخرین_مرحله_اجرا_شده_فرآیند";
                        Query += ",[مراحل_فرآیند].[عنوان_مرحله]  AS عنوان_آخرین_مرحله_اجرا_شده_فرآیند";
                        Query += ",[مراحل_فرآیند].[مرحله_بعد_فرآیند]  AS مرحله_بعد_فرآیند";
                        Query += ",[مراحل_فرآیند].[عنوان_مرحله_بعد_فرآیند]  AS عنوان_مرحله_بعد_فرآیند";
                        Query += ",[مراحل_فرآیند].[تاریخ_ثبت] + N' - '+[مراحل_فرآیند].[ساعت_ثبت]   AS تاریخ_و_ساعت_آخرین_مرحله_اجرا_شده_فرآیند";
                        Query = Query + "\n" + SubQuery;
                        Query += "\n LEFT JOIN مراحل_فرآیند ON [مراحل_فرآیند].[جدول] = " + TableCore.CoreObjectID + " AND [مراحل_فرآیند].[سطر] = [" + TableCore.FullName + "].[" + IdentityField + "] AND [مراحل_فرآیند].[فرآیند] = " + ProcessID.ToString() + " AND ([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت])=(SELECT Max(([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت])) FROM [مراحل_فرآیند] WHERE  [مراحل_فرآیند].[جدول] = " + TableCore.CoreObjectID + " AND [مراحل_فرآیند].[سطر] = [" + TableCore.FullName + "].[" + IdentityField + "] AND [مراحل_فرآیند].[فرآیند] = " + ProcessID.ToString() + ")";

                    }
                }
                //if (!string.IsNullOrEmpty(Desktop.DataInformationEntryForm[DataKey].Query))
                //{
                    Query +=  "\nWhere  " + (searchForm.CommonConditionQuery != "" ? Tools.ConvertToSQLQuery(searchForm.CommonConditionQuery) : "1=1");

                    if (ProcessID > 0 && ProcessStepID == 0)
                    {
                        Query += "\nAND [مراحل_فرآیند].[مرحله_فرآیند]  is not null";
                        Query += "\nAND [مراحل_فرآیند].[مرحله_بعد_فرآیند] > 0";
                        Query += "\nAND( @شناسه_کاربر IN (SELECT  مراحل_فرآیند.[اجرا_کننده] from مراحل_فرآیند where  [مراحل_فرآیند].[سطر] = [" + TableCore.FullName + "].[" + IdentityField + "] AND [مراحل_فرآیند].[فرآیند] = " + ProcessID.ToString() + ")";
                        Query += "\nOR @شناسه_کاربر IN  (SELECT [ارجاع_مراحل_فرآیند].[دریافت_کننده] FROM [ارجاع_مراحل_فرآیند] WHERE [ارجاع_مراحل_فرآیند].[فرآیند] = " + ProcessID + " AND [ارجاع_مراحل_فرآیند].[رکورد] = [" + TableCore.FullName + "].[" + IdentityField + "]))";
                    } 
                //}
            }
            else
            {
                Query += "\n\n" + Tools.ConvertToSQLQuery(searchForm.Query);
                if (SearchFieldItem == null && GridJSON == null)
                    Query += ((Tools.ConvertToSQLQuery(searchForm.ConditionQuery).ToLower().IndexOf("where") > -1) ? "\n"+ searchForm.ConditionQuery : "\nWhere\n"+ searchForm.ConditionQuery);
                else
                    Query += "\nWhere  " + (searchForm.CommonConditionQuery != "" ? Tools.ConvertToSQLQuery(searchForm.CommonConditionQuery) : "1=1");
            }

            if(SearchFieldItem!= null)
                foreach (CoreObject Item in SearchFieldCore)
                {
                    SearchField searchField = new SearchField(Item);
                    CoreObject FieldCore = CoreObject.Find(searchField.RelatedField);
                    Field field = new Field(FieldCore);
                    CoreObject FieldTableCore = CoreObject.Find(FieldCore.ParentID);
                    Query += "\n And (" + FieldTableCore.FullName+"."+ field.FieldName + " ";
                    int FindIndex = Array.IndexOf(SearchFieldItem, Item.CoreObjectID.ToString());
                    switch (field.FieldType)
                    {
                        case CoreDefine.InputTypes.TwoValues:
                            {
                                switch (SearchFieldOperator[FindIndex])
                                {
                                    case "نامساوی": Query += " <>  @" + Item.FullName + " or @" + Item.FullName + " = null "; break;
                                    case "بزرگتر یا مساوی": Query += " >= @" + Item.FullName + " or @" + Item.FullName + " = null "; break;
                                    case "بزرگتر": Query += " >  @" + Item.FullName + " or @" + Item.FullName + " = null "; break;
                                    case "کوچکتر یا مساوی": Query += " <=  @" + Item.FullName + " or @" + Item.FullName + " = null "; break;
                                    case "کوچکتر": Query += " <  @" + Item.FullName + " or @" + Item.FullName + " = null "; break;
                                    case "تهی": Query += " IS Null"; break;
                                    case "تهی نیست": Query += " Is Not Null"; break;
                                    default: Query += " = @" + Item.FullName + " or @" + Item.FullName + " = null "; break;
                                }
                                break;
                            }
                        case CoreDefine.InputTypes.ShortText:
                        case CoreDefine.InputTypes.LongText:
                        case CoreDefine.InputTypes.Phone:
                        case CoreDefine.InputTypes.Clock:
                        case CoreDefine.InputTypes.ComboBox:
                        case CoreDefine.InputTypes.Editor:
                        case CoreDefine.InputTypes.FillTextAutoComplete:
                        case CoreDefine.InputTypes.NationalCode:
                        case CoreDefine.InputTypes.PersianDateTime:
                        case CoreDefine.InputTypes.MiladyDateTime:
                            {
                                switch (SearchFieldOperator[FindIndex])
                                {
                                    case "نامساوی": Query += " <> @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                    case "بزرگتر یا مساوی": Query += ">= @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                    case "بزرگتر": Query += " > @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                    case "کوچکتر یا مساوی": Query += "<= @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                    case "کوچکتر": Query += " < @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                    case "شروع با": Query += " Like N'%'+ @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                    case "شامل": Query += " Like N'%'+@" + Item.FullName + "+'%' or @" + Item.FullName + " = N'' "; break;
                                    case "شامل نباشد": Query += " Not Like N'%'+@" + Item.FullName + "+'%' or @" + Item.FullName + " = N'' "; break;
                                    case "پایان با": Query += " Like @" + Item.FullName + "+N'%'"; break;
                                    case "تهی": Query += " IS Null"; break;
                                    case "تهی نیست": Query += " Is Not Null"; break;
                                    case "خالی": Query += " = N''"; break;
                                    case "خالی نیست": Query += " <> N'' "; break;
                                    default: Query += " = @" + Item.FullName + " or @" + Item.FullName + " = N'' "; break;
                                }
                                break;

                            }

                        default:
                            {
                                switch (SearchFieldOperator[FindIndex])
                                {
                                    case "نامساوی": Query += " <>  @" + Item.FullName + " or @" + Item.FullName + " = 0 "; break;
                                    case "بزرگتر یا مساوی": Query += " >= @" + Item.FullName + " or @" + Item.FullName + " = 0 "; break;
                                    case "بزرگتر": Query += " >  @" + Item.FullName + " or @" + Item.FullName + " = 0 "; break;
                                    case "کوچکتر یا مساوی": Query += " <=  @" + Item.FullName + " or @" + Item.FullName + " = 0 "; break;
                                    case "کوچکتر": Query += " <  @" + Item.FullName + " or @" + Item.FullName + " = 0 "; break;
                                    case "تهی": Query += " IS Null"; break;
                                    case "تهی نیست": Query += " Is Not Null"; break;
                                    default: Query += " = @" + Item.FullName + " or @" + Item.FullName + " = 0 "; break;
                                }
                                break;
                            };
                    }

                    Query += ")";

                }


            if (searchForm.Query == "")
            {
                Query += string.IsNullOrEmpty(Desktop.DataInformationEntryForm[DataKey].GroupByQuery) ? "" : "\n\nGROUP BY " + Desktop.DataInformationEntryForm[DataKey].GroupByQuery; 
                Query += string.IsNullOrEmpty(Desktop.DataInformationEntryForm[DataKey].OrderQuery) ? "" : "\n\nOrder BY " + Desktop.DataInformationEntryForm[DataKey].OrderQuery;
                Query += (ProcessID > 0 && ProcessStepID == 0)? (string.IsNullOrEmpty(Desktop.DataInformationEntryForm[DataKey].OrderQuery) ?"\nORDER BY ([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت]) desc" : " ,([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت]) desc"):"";

            }

            if (SearchFieldItem == null && GridJSON == null)
            {
                string SearchConditionQuery = string.Empty;
                CoreObject ParentTableCore = CoreObject.Find(long.Parse(EditorFormDataKey));
                List<CoreObject> ParentFieldList = new List<CoreObject>();
                if (ParentTableCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                {
                    ParentFieldList = CoreObject.FindChilds(ParentTableCore.CoreObjectID, CoreDefine.Entities.فیلد);
                    ParentTableCore = CoreObject.Find(new InformationEntryForm(ParentTableCore).RelatedTable);
                }

                if (ParentFieldList.Count == 0)
                    ParentFieldList = CoreObject.FindChilds(ParentTableCore.CoreObjectID, CoreDefine.Entities.فیلد);

                if (ParentFieldList.Count > 0)
                {

                    if (FormInputName != null)
                        for (int Index = 0; Index < FormInputName.Length; Index++)
                            FormInputName[Index] = FormInputName[Index].Replace("_" + EditorFormDataKey, "");

                    foreach (CoreObject Item in ParentFieldList)
                    {
                        Field field = new Field(Item);
                        int FindIndex = Array.IndexOf(FormInputName, field.FieldName);
                        if (FindIndex > -1)
                        {
                            SearchConditionQuery += "\nDeclare @" + ParentTableCore.FullName + "_" + Item.FullName + " AS ";
                            switch (field.FieldType)
                            {
                                case CoreDefine.InputTypes.Number:
                                case CoreDefine.InputTypes.RelatedTable:
                                    {
                                        SearchConditionQuery += " Bigint = " + (FormInputValue[FindIndex]==""?"0" : FormInputValue[FindIndex]);
                                        break;
                                    }
                                case CoreDefine.InputTypes.TwoValues:
                                    {
                                        SearchConditionQuery += " Bit = " + (FormInputValue[FindIndex] == "true" ? 1 : 0);
                                        break;
                                    }
                                default:
                                    {
                                        SearchConditionQuery += " Nvarchar(400) = N'" + FormInputValue[FindIndex] + "'";
                                        break;
                                    }
                            }
                        }
                    }

                    Query=SearchConditionQuery + "\n" + Query+"\n";
                }
            }
            else if(long.Parse(ParentID) >0)
            {
                CoreObject InformationEntryFormCore = CoreObject.Find(CoreObject.Find(long.Parse(DataKey)).ParentID);
                InformationEntryForm informationEntryForm = new InformationEntryForm(InformationEntryFormCore);
                CoreObject TableCoreInfo = CoreObject.Find(informationEntryForm.RelatedTable);
                Record record = new Record();
                if (dataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && dataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                {
                    record = new Record(Referral.DBData, TableCoreInfo.FullName, long.Parse(ParentID)); 
                }
                else
                {
                    switch (dataSourceInfo.DataSourceType)
                    {
                        case CoreDefine.DataSourceType.SQLSERVER:
                            {
                                SQLDataBase sQLDataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                record = new Record(sQLDataBase, TableCoreInfo.FullName, long.Parse(ParentID));
                                break;
                            }
                        case CoreDefine.DataSourceType.EXCEL:
                            {
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
                    }
                }
 
                Query =Referral.DBData.DefineVariablesQuery(TableCoreInfo.FullName, long.Parse(ParentID), record.Columns, record.Values, TableCoreInfo.FullName) +"\n\n"+ Query;
            }

            Query = Tools.CheckQuery(Query);
            if (formCore.ParentID != 0)
            {             
                if (dataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && dataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                {
                    DataInfo = Referral.DBData.SelectDataTable(Query);
                }
                else
                {
                    switch (dataSourceInfo.DataSourceType)
                    {
                        case CoreDefine.DataSourceType.SQLSERVER:
                            {
                                SQLDataBase sQLDataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                DataInfo = sQLDataBase.SelectDataTable(Query);
                                break;
                            }
                        case CoreDefine.DataSourceType.EXCEL:
                            {
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
                    }
                }
                     
                if (DataInfo.Rows.Count == 0 && SearchFieldItem != null)
                    ErrorMessage = "هیچ رکوردی یافت نشد";
                else
                {

                    if (!string.IsNullOrEmpty(searchForm.SearchConditionQuery) || !string.IsNullOrEmpty(searchForm.SearchAlarmQuery))
                    {
                        string SearchConditionQuery = string.Empty;
                        CoreObject ParentTableCore = CoreObject.Find(long.Parse(EditorFormDataKey));
                        List<CoreObject> ParentFieldList = new List<CoreObject>();
                        if (ParentTableCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                        {
                            ParentFieldList = CoreObject.FindChilds(ParentTableCore.CoreObjectID, CoreDefine.Entities.فیلد);
                            ParentTableCore = CoreObject.Find(new InformationEntryForm(ParentTableCore).RelatedTable);
                        }

                        if (ParentFieldList.Count == 0)
                            ParentFieldList = CoreObject.FindChilds(ParentTableCore.CoreObjectID, CoreDefine.Entities.فیلد);

                        if (ParentFieldList.Count > 0)
                        {

                            if (FormInputName != null)
                                for (int Index = 0; Index < FormInputName.Length; Index++)
                                    FormInputName[Index] = FormInputName[Index].Replace("_" + EditorFormDataKey, "");

                            foreach (CoreObject Item in ParentFieldList)
                            {
                                Field field = new Field(Item);
                                int FindIndex = Array.IndexOf(FormInputName, field.FieldName);
                                if (FindIndex > -1)
                                {
                                    SearchConditionQuery += "\nDeclare @" + ParentTableCore.FullName + "_" + Item.FullName + " AS ";
                                    switch (field.FieldType)
                                    {
                                        case CoreDefine.InputTypes.Number:
                                        case CoreDefine.InputTypes.RelatedTable:
                                            {
                                                SearchConditionQuery += " Bigint = " + FormInputValue[FindIndex];
                                                break;
                                            }
                                        case CoreDefine.InputTypes.TwoValues:
                                            {
                                                SearchConditionQuery += " Bit = " + (FormInputValue[FindIndex] == "true" ? 1 : 0);
                                                break;
                                            }
                                        default:
                                            {
                                                SearchConditionQuery += " Nvarchar(400) = N'" + FormInputValue[FindIndex] + "'";
                                                break;
                                            }
                                    }
                                }
                            }

                            foreach (DataRow Row in DataInfo.Rows)
                            {
                                string SubQuery = "";
                                foreach (DataColumn column in DataInfo.Columns)
                                {
                                    SubQuery += "\nDeclare @" + TableCore.FullName + "_" + column.ColumnName + " AS ";
                                    switch (column.DataType.Name)
                                    {
                                        case "Int64":
                                            {
                                                SubQuery += " Bigint = " + (Row[column.ColumnName].ToString()==""?"null": Row[column.ColumnName].ToString());
                                                break;
                                            }
                                        case "Decimal":
                                            {
                                                SubQuery += " Float = " + (Row[column.ColumnName].ToString()==""?"null": Row[column.ColumnName].ToString());
                                                break;
                                            }
                                        case "Boolean":
                                            {
                                                SubQuery += " Bit = " + (Row[column.ColumnName].ToString()==""?"null":((bool)Row[column.ColumnName] == true ? "1" : "0"));
                                                break;
                                            }
                                        case "Byte[]":
                                            {
                                                SubQuery += " Nvarchar(400) = null";
                                                break;
                                            }
                                        default:
                                            {
                                                SubQuery += " Nvarchar(400) = N'" + Row[column.ColumnName].ToString() + "'";
                                                break;
                                            }
                                    }
                                }

                                if(!string.IsNullOrEmpty(searchForm.SearchConditionQuery))
                                    ErrorMessage += Referral.DBData.SelectField(Tools.CheckQuery(SearchConditionQuery + "\n\n"+ SubQuery+"\n\n" + searchForm.SearchConditionQuery)).ToString();

                                if(!string.IsNullOrEmpty(searchForm.SearchAlarmQuery))
                                    AlarmMessage += Referral.DBData.SelectField(Tools.CheckQuery(SearchConditionQuery + "\n\n" + SubQuery + "\n\n" + searchForm.SearchAlarmQuery)).ToString();

                            }

                            if(DataInfo.Rows.Count==0 && !string.IsNullOrEmpty(searchForm.SearchAlarmQuery))
                                AlarmMessage += Referral.DBData.SelectField(Tools.CheckQuery(SearchConditionQuery + "\n\n" + searchForm.SearchAlarmQuery)).ToString();

                        }

                    }

                    if (ErrorMessage == "" || string.IsNullOrEmpty(ErrorMessage))
                    {
                        if (searchForm.SelectedColumns != "")
                        {
                            string[] SelectedColumn = searchForm.SelectedColumns.Split('،');
                            Field IDField = Desktop.DataTable[DataKey].IDField();
                            DataTable MainTable = (DataTable)Desktop.SessionEditorGrid[DataKey, ParentID];

                            if (!string.IsNullOrEmpty(Desktop.DataInformationEntryForm[DataKey].CheckFieldDuplicateRecords))
                            {
                                string[] CheckFieldArr = Desktop.DataInformationEntryForm[DataKey].CheckFieldDuplicateRecords.Replace(" ", "").Replace(",", "،").Split('،');
                                string[] FieldArr=new string[CheckFieldArr.Length]; 
                                for (int Index=0;Index< CheckFieldArr.Length;Index++)
                                { 
                                    foreach(string SelectedColumnItem in SelectedColumn)
                                        if(SelectedColumnItem.IndexOf("{"+CheckFieldArr[Index]+":") >-1)
                                            FieldArr[Index] = SelectedColumnItem.Replace(CheckFieldArr[Index]+":","").Replace("{","").Replace("}","");

                                }

                                foreach(DataRow row in DataInfo.Rows)
                                {
                                    string SubQuery = "";
                                    for (int Index = 0; Index < CheckFieldArr.Length; Index++)
                                    {
                                        SubQuery+= CheckFieldArr[Index]+"="+row[FieldArr[Index]];
                                    }
                                    if (MainTable.Select(SubQuery).Length > 0)
                                    {
                                        ErrorMessage = "ردیف ایجاد شده تکراری است";
                                        break;
                                    }
                                }
                            }
                            if(ErrorMessage=="")
                            {
                                int MaxID = 0;
                                if (CleareGridAfterSearch)
                                    MainTable.Rows.Clear();
                                else
                                {
                                    if (GridJSON != "[]")
                                    {
                                        DataTable GridData = DataConvertor.JsonStringToDataTable(GridJSON);
                                        foreach (DataRow GridDataRow in GridData.Rows)
                                        {
                                            if (GridDataRow[IDField.FieldName] != null && GridDataRow[IDField.FieldName].ToString() != "0")
                                            {
                                                foreach (DataRow MainTableRow in MainTable.Rows)
                                                {
                                                    if (MainTableRow[IDField.FieldName].ToString() == GridDataRow[IDField.FieldName].ToString())
                                                    {
                                                        foreach (DataColumn column in MainTable.Columns)
                                                            if (GridData.Columns.IndexOf(column.ColumnName) > -1)
                                                                if (GridDataRow[column.ColumnName].ToString() != "null")
                                                                    MainTableRow[column.ColumnName] = GridDataRow[column.ColumnName];
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MaxID = MainTable == null ? 0 : MainTable.Compute("max([" + IDField.FieldName + "])", string.Empty).ToString() == "" ? 0 : Convert.ToInt32(MainTable.Compute("max([" + IDField.FieldName + "])", string.Empty).ToString());

                                                DataRow NewRow = MainTable.NewRow();
                                                NewRow[IDField.FieldName] = ++MaxID;

                                                foreach (DataColumn column in MainTable.Columns)
                                                    if (GridData.Columns.IndexOf(column.ColumnName) > -1 && column.ColumnName!= IDField.FieldName)
                                                        if (GridDataRow[column.ColumnName].ToString() != "null")
                                                            NewRow[column.ColumnName] = GridDataRow[column.ColumnName];

                                                MainTable.Rows.Add(NewRow);
                                                Desktop.SessionEditorGrid[DataKey, ParentID] = MainTable;

                                            }
                                        }
                                    }
                                }

                                MaxID = MainTable == null ? 0 : MainTable.Compute("max([" + IDField.FieldName + "])", string.Empty).ToString() == "" ? 0 : Convert.ToInt32(MainTable.Compute("max([" + IDField.FieldName + "])", string.Empty).ToString());

                                if (DataInfo != null)
                                {
                                    foreach (DataRow row in DataInfo.Rows)
                                    {
                                        DataRow NewRow = MainTable.NewRow();
                                        NewRow[IDField.FieldName] = ++MaxID;

                                        foreach (string Item in SelectedColumn)
                                        {
                                            try
                                            {
                                                string[] ItemArr = Item.Replace("{", "").Replace("}", "").Trim().Split(':');
                                                if (row[ItemArr[1]].ToString() == "System.Byte[]")
                                                {
                                                    byte[] FileByte = (byte[])row[ItemArr[1]];
                                                    //FormInputValue[index] = Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte));

                                                    string Extension = "png";
                                                    string path = Models.Attachment.MapTemporaryFilePath + DataKey + "/" + MaxID.ToString();

                                                    DirectoryInfo dir = new DirectoryInfo(path);
                                                    if (!Directory.Exists(dir.FullName))
                                                        Directory.CreateDirectory(dir.FullName);

                                                    foreach (FileInfo FileItem in dir.GetFiles())
                                                    {
                                                        if (FileItem.Name.Replace(FileItem.Extension, "") == ItemArr[1])
                                                        {
                                                            System.IO.File.Delete(FileItem.FullName);
                                                        }
                                                    }
                                                    System.IO.File.WriteAllText(path + "/" + ItemArr[1] + "." + Extension, Convert.ToBase64String((byte[])FileByte));
                                                }
                                                else
                                                {
                                                    NewRow[ItemArr[0]] = row[ItemArr[1]];
                                                }
                                            }
                                            catch { }
                                        }
                                        MainTable.Rows.Add(NewRow);
                                        if (long.Parse(ParentID) > 0)
                                        {
                                            FormInputName =new string[] { };
                                            object[] FormInputValueObj= new object[] { };   
                                            foreach(DataColumn dataColumn in MainTable.Columns)
                                            {
                                                if(dataColumn.ColumnName != IDField.FieldName)
                                                {
                                                    Array.Resize(ref FormInputName, FormInputName.Length + 1);
                                                    Array.Resize(ref FormInputValueObj, FormInputValueObj.Length + 1);
                                                    FormInputName[FormInputName.Length - 1] = dataColumn.ColumnName;
                                                    FormInputValueObj[FormInputName.Length - 1] = NewRow[dataColumn.ColumnName].ToString(); 
                                                }
                                            }
                                            Session["SearchinformationEntryFormQuery" + DataKey + "_" + ParentID] = null;
                                            Query = "1";
                                            string Result= SaveFormEditor(DataKey, ParentID, 0, FormInputName, FormInputValueObj, 0, false).Data.ToString();
                                        }
                                    } 
                                    Desktop.SessionEditorGrid[DataKey, ParentID] = MainTable;
                                }
                            }

                        }

                        if (Desktop.DataInformationEntryForm[DataKey].ParentID == 0)
                        {
                            Desktop.CachedTable[DataKey] = DataInfo;
                        }
                    }

                }
            }



            return Json(new { Query = Query ,ErrorMessage=ErrorMessage,AlarmMessage=AlarmMessage});
        }
        public JsonResult CheckGridSelecteRowBeforSave(long RecordID, string ParentDataKey, string[] ParentColumnName, object[] ParentValuesItem, string DataKey, string[] FormInputName, object[] FormInputValue)
        {
            InformationEntryForm InformationEntryForm = new InformationEntryForm(CoreObject.Find(long.Parse(DataKey)));
            InformationEntryForm ParentInformationEntryForm = new InformationEntryForm(CoreObject.Find(long.Parse(ParentDataKey)));
            List<CoreObject> coreObjectsList = CoreObject.FindChilds(long.Parse(DataKey), CoreDefine.Entities.رویداد_جدول);
            CoreObject TableCore = CoreObject.Find(InformationEntryForm.RelatedTable);
            CoreDefine.TableEvents tableEvents = RecordID > 0 ? CoreDefine.TableEvents.شرط_اجرای_ویرایش : CoreDefine.TableEvents.شرط_اجرای_درج;


            if (tableEvents == CoreDefine.TableEvents.شرط_اجرای_ویرایش)
            {
                CoreObject _ExternalField = CoreObject.Find(InformationEntryForm.ExternalField);
                int FindIndex = Array.IndexOf(FormInputName, _ExternalField.FullName);
                if (FindIndex > -1)
                    if (FormInputValue[FindIndex] == "" || FormInputValue[FindIndex] == "0")
                    {
                        tableEvents = CoreDefine.TableEvents.شرط_اجرای_درج;
                        RecordID = 0;
                    }
            }

            CoreDefine.TableEvents WarningTableEvents = RecordID > 0 ? CoreDefine.TableEvents.هشدار_قبل_از_ویرایش : CoreDefine.TableEvents.هشدار_قبل_از_درج;
            string ErrorAlarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, RecordID, tableEvents, FormInputName, FormInputValue);
            string WarningAlarm = Desktop.CheckBeforRunQuery(TableCore.CoreObjectID, RecordID, WarningTableEvents, FormInputName, FormInputValue);
            if (ErrorAlarm != "")
                return Json(new { Error = ErrorAlarm, Warning = WarningAlarm });

            if (coreObjectsList.Count > 0)
            {
                CoreObject ParentTableCore = CoreObject.Find(ParentInformationEntryForm.RelatedTable);
                DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                string Query = "";
                foreach (Field field in Desktop.DataFields[ParentDataKey])
                {
                    int FindIndex = Array.IndexOf(ParentColumnName, field.FieldName);
                    if (FindIndex > -1)
                    {
                        Query += "\nDeclare @" + ParentTableCore.FullName + "_" + field.FieldName + " AS ";
                        switch (field.FieldType)
                        {
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                                {

                                    Query += "Bigint = " + (ParentValuesItem[FindIndex] == "" ? "0" : ParentValuesItem[FindIndex]);
                                    break;
                                }
                            case CoreDefine.InputTypes.TwoValues:
                                {
                                    Query += " Bit = " + (ParentValuesItem[FindIndex] == "" || ParentValuesItem[FindIndex].ToString() == "false" ? "0" : "1");
                                    break;
                                }
                            default:
                                {
                                    Query += "Nvarchar(400) = N'" + ParentValuesItem[FindIndex] + "'";
                                    break;
                                }
                        }

                    }
                }

                foreach (Field field in Desktop.DataFields[DataKey])
                {
                    int FindIndex = Array.IndexOf(FormInputName, field.FieldName);
                    if (FindIndex > -1)
                    {
                        Query += "\nDeclare @" + TableCore.FullName + "_" + field.FieldName + " AS ";
                        switch (field.FieldType)
                        {
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                                {

                                    Query += "Bigint = " + (FormInputValue[FindIndex] == "" ? "0" : FormInputValue[FindIndex]);
                                    break;
                                }
                            case CoreDefine.InputTypes.TwoValues:
                                {
                                    Query += " Bit = " + (FormInputValue[FindIndex] == "" || FormInputValue[FindIndex].ToString() == "false" ? "0" : "1");
                                    break;
                                }
                            default:
                                {
                                    Query += "Nvarchar(400) = N'" + FormInputValue[FindIndex] + "'";
                                    break;
                                }
                        }
                    }
                }

                foreach (CoreObject coreObject in coreObjectsList)
                {
                    TableEvent tableEvent = new TableEvent(coreObject);
                    if (tableEvent.EventType == tableEvents.ToString())
                    {
                        if (dataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase && dataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source)
                            ErrorAlarm += Referral.DBData.SelectField(Tools.CheckQuery("") + Tools.NewLine +Query + Tools.NewLine + Tools.ConvertToSQLQuery(tableEvent.Query)).ToString();
                        else
                        {
                            switch (dataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        SQLDataBase sqlData = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                        ErrorAlarm += sqlData.SelectField(Tools.CheckQuery("") + Query + Tools.NewLine + Tools.ConvertToSQLQuery(tableEvent.Query)).ToString();
                                        break;
                                    }
                                case CoreDefine.DataSourceType.MySql:
                                    {
                                        break;
                                    }
                                case CoreDefine.DataSourceType.EXCEL:
                                    {
                                        break;
                                    }
                                case CoreDefine.DataSourceType.ACCESS:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }

                foreach (CoreObject coreObject in coreObjectsList)
                {
                    TableEvent tableEvent = new TableEvent(coreObject);
                    if (tableEvent.EventType == WarningTableEvents.ToString())
                    {
                        if (dataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase && dataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source)
                            WarningAlarm += Referral.DBData.SelectField(Query + Tools.NewLine + Tools.ConvertToSQLQuery(tableEvent.Query)).ToString();
                        else
                        {
                            switch (dataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        SQLDataBase sqlData = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                        WarningAlarm += sqlData.SelectField(Query + Tools.NewLine + Tools.ConvertToSQLQuery(tableEvent.Query)).ToString();
                                        break;
                                    }
                                case CoreDefine.DataSourceType.MySql:
                                    {
                                        break;
                                    }
                                case CoreDefine.DataSourceType.EXCEL:
                                    {
                                        break;
                                    }
                                case CoreDefine.DataSourceType.ACCESS:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            return Json(new { Error = ErrorAlarm, Warning = WarningAlarm });
        }


        public JsonResult SendEmailDialog(string DataKey,long ParentID,string RecordID,bool SendAttachmentFile,string ReportSelected ,string Email,string Title,string Massage)
        {
            List<string> EmailAccount=Email.Split(',').ToList();
            string[] SendReport = ReportSelected.Split(',');
            List<MailAttachmnet> MemoryStreamsList = new List<MailAttachmnet>();
            CoreObject TableCore=CoreObject.Find(long.Parse(DataKey));
            long TableID = TableCore.CoreObjectID;

            if (TableCore.Entity==CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm EntryForm = new InformationEntryForm(TableCore);
                TableID = EntryForm.RelatedTable;
            }
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
                        MemoryStreamsList.Add(new MailAttachmnet() { AttachmentMemory = memory, AttachmentName = Tools.UnSafeTitle(report.FullName) + ".pdf" });

                    }
                }
            }

            if (SendAttachmentFile)
            {
                string PDestinationPath = Models.Attachment.MapFileSavingAttachmentPath + TableID + "/" + RecordID;
                DirectoryInfo directoryInfo = new DirectoryInfo(PDestinationPath);
                if (directoryInfo.Exists)
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        string FileText = System.IO.File.ReadAllText(file.FullName);
                        byte[] fileData = Convert.FromBase64String(FileText);
                        MemoryStream memory = new MemoryStream(fileData);
                        MemoryStreamsList.Add(new MailAttachmnet() { AttachmentMemory = memory, AttachmentName = file.Name });
                    }
            }


            MailCore mailCore = new MailCore()
            {
                EMail = Referral.PublicSetting.E_Maile,
                EMailUserName = Referral.PublicSetting.EMaileUserName,
                EMailPassword = Referral.PublicSetting.EMailePassWord,
                EMailServer = Referral.PublicSetting.EMaileServer,
                EMailPort = Referral.PublicSetting.EMailePort,
                EnableSsl = Referral.PublicSetting.EnableSsl,  
                Title = Title, 
                BodyMessage = Massage,  
            };
            string ErrorMessage = "";
            mailCore.SendMailMessage(EmailAccount, MemoryStreamsList,ref ErrorMessage);
            return Json(ErrorMessage);
        }


        public JsonResult SetSeat(long RowId, int Num,string Char,bool IsChecked)
        {
            string Query = "Update FlightPassengers set AirplaneSeat = (Select top 1 AirplaneSeat.شناسه From AirplaneSeat Where Num = " + Num.ToString() + " And RowChar =N'" + Char + "') where شناسه = " + RowId.ToString();
            if(!IsChecked)
              Query = "Update FlightPassengers set AirplaneSeat =0 where شناسه = " + RowId.ToString();
            return Json( Referral.DBData.Execute(Query));
            
        }
    }
}