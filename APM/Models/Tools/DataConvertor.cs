using APM.Models.Database;
using APM.Models.APMObject;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace APM.Models.Tools
{
    public class DataConvertor
    {

        public static List<ReportParameter> InformationEntryFormReport(long _FormID)
        {
            List<ReportParameter> Output = new List<ReportParameter>();
            long TableID = _FormID;
            CoreObject Form = CoreObject.Find(_FormID);
            switch (Form.Entity)
            {
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                {
                    InformationEntryForm informationEntryForm = new InformationEntryForm(Form);
                    TableID = informationEntryForm.RelatedTable;
                    break;
                }
            }

            List<CoreObject> ReportParameterList = CoreObject.FindChilds(CoreDefine.Entities.پارامتر_گزارش);
            foreach (CoreObject ParameterCore in ReportParameterList)
            {
                ReportParameter Parameter = new ReportParameter(ParameterCore);
                PermissionBase permissionBase = new PermissionBase(ParameterCore.ParentID, Referral.UserAccount.Permition);
                if (Parameter.RelatedTable==TableID && permissionBase.IsAllow)
                    Output.Add(Parameter);
            }

            return Output;
        } 

        public static List<Field> InformationEntryFormReady(InformationEntryForm _InformationEntryForm ,bool JsutRebuildField=false)
        {
            List<Field> Output = null;
            if(!JsutRebuildField)
            {
                List<CoreObject> FieldCore = CoreObject.FindChilds(_InformationEntryForm.CoreObjectID, CoreDefine.Entities.فیلد);
                if(FieldCore.Count>0)
                {
                    Output=new List<Field>();
                    foreach (CoreObject FieldCoreItem in FieldCore)
                    {
                        Output.Add(new Field(FieldCoreItem));
                    }
                    //FillSelectList(Output);
                    return Output;
                }
            }

            if (_InformationEntryForm != null)
            {
                CoreObject TableObject = CoreObject.Find(_InformationEntryForm.RelatedTable);
                CoreObject ExternalFieldObject = CoreObject.Find(_InformationEntryForm.ExternalField);
                Table Table = new Table(TableObject);
                string IdentityField = Table.IDField().FieldName; 
                string DeclareQuery=string.Empty; 
                string Query = _InformationEntryForm.Query.Trim() == "" ? DefaultQueryForTable(_InformationEntryForm.RelatedTable, _InformationEntryForm.ShowRecordCountDefault) :"Select Top 0\n"+  _InformationEntryForm.Query;
                Query = Tools.ConvertToSQLQuery(Query);
                 
                if(_InformationEntryForm.ExternalField > 0 && _InformationEntryForm.ParentID > 0)
                {
                    if (_InformationEntryForm.Query == "")
                        Query += " WHERE " + TableObject.FullName + "." + ExternalFieldObject.FullName + "=@" + IdentityField;
                    else
                    {
                        int LastIndexOf_FROM = Query.ToUpper().LastIndexOf("FROM");
                        string SubStrQuery = Query.Substring(LastIndexOf_FROM, (Query.Length - 1- LastIndexOf_FROM));
                        if (SubStrQuery.ToUpper().IndexOf("WHERE") == -1)
                            Query += " WHERE " + TableObject.FullName + "." + ExternalFieldObject.FullName + "=@" + IdentityField;
                    }
                }
                else
                {
                    Query += string.IsNullOrEmpty(_InformationEntryForm.ConditionQuery)?"": "\nWhere " + Tools.ConvertToSQLQuery(_InformationEntryForm.ConditionQuery);
                    Query += string.IsNullOrEmpty(_InformationEntryForm.GroupByQuery) ?"": "\nGroup By " + Tools.ConvertToSQLQuery(_InformationEntryForm.GroupByQuery);
                    Query += string.IsNullOrEmpty(_InformationEntryForm.OrderQuery) ?"": "\nOrder By " + Tools.ConvertToSQLQuery(_InformationEntryForm.OrderQuery);
                }
                 
                if(IdentityField != "")
                {
                    List<CoreObject> FieldList = CoreObject.FindChilds(_InformationEntryForm.RelatedTable, CoreDefine.Entities.فیلد);
                    foreach(CoreObject FieldCore in FieldList)
                    {
                        Field Field = new Field(FieldCore);
                        switch(Field.FieldType)
                        {
                            case CoreDefine.InputTypes.ComboBox:
                            case CoreDefine.InputTypes.NationalCode:
                            case CoreDefine.InputTypes.ShortText:
                                { 
                                    DeclareQuery += "Declare @" + Field.FieldName + " AS NVARCHAR(400)=N''" + Tools.NewLine;
                                    break;
                                }
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                                { 
                                    DeclareQuery += "Declare @" + Field.FieldName + " AS  Bigint = 0" + Tools.NewLine;
                                    break;
                                } 
                            case CoreDefine.InputTypes.SingleSelectList:
                                { 
                                    DeclareQuery += "Declare @" + Field.FieldName + " AS NVARCHAR(400)= N''" + Tools.NewLine;
                                    break;
                                }

                        }
                    }
                }

                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(_InformationEntryForm.RelatedTable).ParentID));
                DataTable DataTable = SelectDataTable(DataSourceInfo, DeclareQuery+Tools.NewLine+ Query, Table.FullName);
                Output = GetTranslatedFields(_InformationEntryForm.CoreObjectID, DataTable, JsutRebuildField);

                if(!JsutRebuildField)
                {
                    Desktop.CachedTable[_InformationEntryForm.CoreObjectID.ToString()] = DataTable;
                    //FillSelectList(Output); 
                }
            }

            return Output;
        }
        
        public static List<Field> TableFormReady(long _Form)
        {
            List<Field> Output = null;

            if (_Form != 0)
            {
                CoreObject TableObject = CoreObject.Find(_Form);
                string Query = DefaultQueryForTable(TableObject.CoreObjectID, 1); 
                Table Table = new Table(TableObject);

                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
                DataTable DataTable = SelectDataTable(DataSourceInfo, Query, Table.FullName);

                Output = GetTranslatedFields(_Form, DataTable);
                FillSelectList(Output);
            }

            return Output;
        }

        public static List<Field> SearchFormReady(long _Form)
        {
            List<Field> Output = null;

            if (_Form != 0)
            {
                SearchForm SearchForm =new SearchForm( CoreObject.Find(_Form)); 
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(SearchForm.RelatedTable).ParentID));
                List<CoreObject> ParentFieldList = new List<CoreObject>();
                string[] FormInputName = (string[])HttpContext.Current.Session["SearchFormButtonClick_FormInputName"];
                string[] FormInputValue = (string[])HttpContext.Current.Session["SearchFormButtonClick_FormInputValue"];
                string Query = String.Empty;
                CoreObject Form = CoreObject.Find(_Form);
                CoreObject ParentTableCore = CoreObject.Find(Form.ParentID);
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
                                        Query += " Bigint = " + (FormInputValue[FindIndex] == "" ? "0" : FormInputValue[FindIndex]);
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
                Query += "\n"+SearchForm.Query;
                string Where = SearchForm.ConditionQuery!=""? Tools.ConvertToSQLQuery(SearchForm.ConditionQuery):" 1=1 ";
                Query += Where.IndexOf("WHERE") > -1 ? Where : " WHERE " +  Where; 
                Query += SearchForm.CommonConditionQuery == "" ? "" : " And " + Where;
                Query += SearchForm.GroupByQuery != "" ? "\nGroup By" + Tools.ConvertToSQLQuery(SearchForm.GroupByQuery)  : "" ;
                DataTable DataTable = SelectDataTable(DataSourceInfo, Query);
                Desktop.CachedTable[_Form.ToString()] = DataTable;
                long RegisterCounter = RegisterCount(CoreObject.Find(SearchForm.RelatedTable).FullName, DataSourceInfo.ServerName, DataSourceInfo.DataBase);
                Desktop.RegisterdTableID[SearchForm.RelatedTable.ToString()] = RegisterCounter;
                Output = GetTranslatedFields(_Form, DataTable);
                FillSelectList(Output);
            }

            return Output;
        }

        public static string DefaultQueryForTable(long TableID,long RowCount)
        {
            CoreObject TableObject = CoreObject.Find(TableID);
            Table Table = new Table(TableObject);
            string Query = "SELECT TOP "+ RowCount + Tools.NewLine;

            List<CoreObject> AttachmentListCore = CoreObject.FindChilds(TableID, CoreDefine.Entities.ضمیمه_جدول);
            List<CoreObject> FieldObject = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد);

            foreach (CoreObject Field in AttachmentListCore)
            {
                TableAttachment tableAttachment = new TableAttachment(Field);
                if(tableAttachment.ShowDefault)
                Query += "{} AS [" + Tools.SafeTitle(Field.FullName.Replace("-", "_").Replace("/", "_")) + "]\n,";
            }

            foreach (CoreObject Field in FieldObject)
            {
                Query += "[" + Table.FullName + "]." + "[" + Field.FullName + "] AS [" + Tools.SafeTitle(Field.FullName.Replace("-", "_").Replace("/", "_")) + "]\n,";
            }

            List<CoreObject> ComputationalFieldObject = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد_محاسباتی);
            foreach (CoreObject Field in ComputationalFieldObject)
            {
                ComputationalField computationalField = new ComputationalField(Field);
                if (computationalField.FieldType == CoreDefine.InputTypes.Image)
                    Query += "{} as [" + Field.FullName + "]\n,";
                else
                Query += "(" + computationalField.Query.Replace("@", Table.FullName+".") + ") AS [" + Field.FullName + "]\n,";
            }

            Query = Query.Substring(0, Query.Length - 1);
            Query += " FROM [" + Table.TABLESCHEMA + "].["+ Table.FullName + "]";

            return Query;
        }

        public static DataTable SelectDataTable(DataSourceInfo DataSourceInfo, string Query,string TableName="")
        { 
            DataTable DataTable = new DataTable();
            switch(DataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        Query = Tools.CheckQuery(Query);

                        if(Referral.DBData.ConnectionData.Source== DataSourceInfo.ServerName && Referral.DBData.ConnectionData.DataBase == DataSourceInfo.DataBase)
                            DataTable = Referral.DBData.SelectDataTable(Query);
                        else
                        {
                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            DataTable = DataBase.SelectDataTable(Query); 
                        }
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        Query = Tools.CheckQuery(Query);

                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                        DataTable = DataBase.SelectDataTable(Query);
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        Query = Tools.CheckAccessQuery(Query);

                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                        DataTable = DataBase.SelectDataTable(Query);
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;

                            application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                            IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);

                            IWorksheet worksheet = workbook.Worksheets[TableName.Replace("$", "").Replace("'", "")];

                            DataTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                        }
                        break;
                    }
            }
            return DataTable;
        }

        public static List<Field> GetTranslatedFields(long _FormID, DataTable FormData, bool HasPermission = false)
        {
            CoreObject FormCore = CoreObject.Find(_FormID);
            List<Field> Output = new List<Field>();

            if(FormData==null)
                return Output;

            string[] ColumnNames = FormData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            List<CoreObject> TableFields = null;
            List<CoreObject> TableComputationalField = null;
            List<CoreObject> TableAttachmentField = null;
            bool DataKey_ShowWithOutPermissionConfig = Desktop.DataKey_ShowWithOutPermissionConfig[_FormID.ToString()];

            switch (FormCore.Entity)
            {
                case CoreDefine.Entities.جدول:
                {
                    TableFields = new List<CoreObject>(CoreObject.FindChilds(_FormID, CoreDefine.Entities.فیلد));
                    TableComputationalField = new List<CoreObject>(CoreObject.FindChilds(_FormID, CoreDefine.Entities.فیلد_محاسباتی));
                    TableAttachmentField = new List<CoreObject>(CoreObject.FindChilds(_FormID, CoreDefine.Entities.ضمیمه_جدول));
                        break;
                }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                {
                    InformationEntryForm informationEntryForm = new InformationEntryForm(FormCore);
                    TableFields = new List<CoreObject>(CoreObject.FindChilds(informationEntryForm.RelatedTable, CoreDefine.Entities.فیلد)); 
                    TableComputationalField = new List<CoreObject>(CoreObject.FindChilds(informationEntryForm.RelatedTable, CoreDefine.Entities.فیلد_محاسباتی));
                    TableAttachmentField = new List<CoreObject>(CoreObject.FindChilds(informationEntryForm.RelatedTable, CoreDefine.Entities.ضمیمه_جدول));
                    break;
                }
                case CoreDefine.Entities.فرم_جستجو:
                    {
                        SearchForm searchForm = new SearchForm(FormCore);
                        if (Desktop.DataTable[searchForm.RelatedTable.ToString()] == null)
                            Desktop.StartupSetting(searchForm.RelatedTable.ToString());
                        else
                        { 
                            TableFields = new List<CoreObject>(CoreObject.FindChilds(searchForm.RelatedTable, CoreDefine.Entities.فیلد));
                            TableComputationalField = new List<CoreObject>(CoreObject.FindChilds(searchForm.RelatedTable, CoreDefine.Entities.فیلد_محاسباتی));
                            TableAttachmentField = new List<CoreObject>(CoreObject.FindChilds(searchForm.RelatedTable, CoreDefine.Entities.ضمیمه_جدول));
                        }
                        break;
                    }
            }

            foreach (string _ColumnName in ColumnNames)
            {
                Field _Field = new Field();
                if (TableFields!=null)
                { 
                    int FieldIndex = TableFields.FindIndex(x => x.FullName.ToUpper() == _ColumnName.ToUpper());

                    if (FieldIndex > -1)
                        _Field = new Field(TableFields[FieldIndex]);
                    else
                    {
                         FieldIndex = TableComputationalField.FindIndex(x => x.FullName.ToUpper() == _ColumnName.ToUpper());
                        if (FieldIndex > -1)
                        {
                            ComputationalField computationalField = new ComputationalField(TableComputationalField[FieldIndex]);
                            _Field.CoreObjectID = TableComputationalField[FieldIndex].CoreObjectID;
                            _Field.IsVirtual = true;
                            _Field.ShowInForm = computationalField.ShowInForm;
                            _Field.FieldType = computationalField.FieldType;
                            _Field.IsDefaultView=computationalField.IsDefaultView;
                            _Field.IsWide = computationalField.IsWide;
                            _Field.SpecialValue = computationalField.Query;
                            _Field.DigitsAfterDecimal = computationalField.DigitsAfterDecimal;
                            _Field.MaxValue = computationalField.MaxValue;
                            _Field.MinValue = computationalField.MinValue;
                            _Field.DisplayName = computationalField.DisplayName;
                            _Field.FieldComment = computationalField.FieldComment;
                        }
                        else
                        {
                            FieldIndex = TableAttachmentField.FindIndex(x => x.FullName.ToUpper() == _ColumnName.ToUpper()); 
                            if(FieldIndex > -1)
                            { 
                                TableAttachment tableAttachment= new TableAttachment(TableAttachmentField[FieldIndex]);
                                _Field.CoreObjectID = TableAttachmentField[FieldIndex].CoreObjectID;
                                _Field.FieldType = CoreDefine.InputTypes.Image;
                                _Field.IsVirtual = false;
                                _Field.IsTableAttachemnt=true;
                                _Field.IsDefaultView = tableAttachment.ShowDefault;
                                _Field.ColumnWidth = tableAttachment.ColumnWidth;
                            }
                            else
                            {
                                if (FormData.Columns[_ColumnName].DataType.Name == "Double")
                                    _Field.FieldType = CoreDefine.InputTypes.Number;
                                else if (FormData.Columns[_ColumnName].DataType.Name == "Byte[]")
                                {
                                    _Field.FieldType = CoreDefine.InputTypes.Image;
                                }
                                _Field.IsVirtual = true;

                            }
                        }
                        _Field.FieldName = _ColumnName;
                    }
                }
                else
                { 
                    _Field.FieldName = _ColumnName;
                }


                PermissionBase _PermissionField = new PermissionBase(_Field.CoreObjectID, Referral.UserAccount.Permition);
                if (_PermissionField.IsAllow || HasPermission  || DataKey_ShowWithOutPermissionConfig || _Field.IsVirtual)
                    Output.Add(_Field);
            }              
            return Output;
        }

        public static SelectList ToSelectList(DataTable table)
        {
            List<SelectListItem> Output = new List<SelectListItem>();

            Output.Add(new SelectListItem() { Value = "0", Text = " ", Selected = true });
            if(table!=null)
            foreach (DataRow row in table.Rows)
            {
                Output.Add(new SelectListItem()
                {
                    Value = row[0].ToString(),
                    Text = row[1].ToString()
                });
            }

            return new SelectList(Output, "Value", "Text", 0);
        }
        public static SelectList ToSelectTextAutoCompleteList(DataTable table)
        {
            List<SelectListItem> Output = new List<SelectListItem>();

            foreach (DataRow row in table.Rows)
            {
                Output.Add(new SelectListItem()
                {
                    Value = row[0].ToString(),
                    Text = row[0].ToString()
                });
            }

            return new SelectList(Output, "Value", "Text", 0);
        }


        public static List<Field> FillTableDataFields(long TableID)
        {
            List<CoreObject> TableFieldObject = new List<CoreObject>(CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد));
            List<CoreObject> TableComputationalField = new List<CoreObject>(CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد_محاسباتی));
            List<Field> TableFields = new List<Field>();
            foreach (CoreObject ItemField in TableFieldObject)
            {
                TableFields.Add(new Field(ItemField));
            }
            foreach (CoreObject ItemField in TableComputationalField)
            {
                ComputationalField computationalField = new ComputationalField(ItemField);
                Field _Field = new Field();
                _Field.CoreObjectID = ItemField.CoreObjectID;
                _Field.IsVirtual = true;
                _Field.FieldType = computationalField.FieldType;
                _Field.FieldName = ItemField.FullName;
                _Field.ShowInForm = computationalField.ShowInForm;
            }
            return TableFields;
        }
        public static SelectList ToSelectList(string[] values)
        {
            List<SelectListItem> Output = new List<SelectListItem>();

            Output.Add(new SelectListItem() { Value = "", Text = "", Selected = true });

            foreach (string Item in values)
            {
                Output.Add(new SelectListItem()
                {
                    Value = Item,
                    Text = Item
                });
            }

            return new SelectList(Output, "Value", "Text", "");
        }

        public static void FillCoreList(string Entitiy, string ParentID)
        {
            string CounterCacheName = "CachedCounter"+Entitiy + ParentID;
            int Counter = RegisterCount("CoreObject",Referral.DBData.ConnectionData.Source,Referral.DBData.ConnectionData.DataBase);

            if (HttpContext.Current.Session["CachedListOfCoreList"+Entitiy+ParentID] == null || ((HttpContext.Current.Session[CounterCacheName] == null ? 0 : (int)HttpContext.Current.Session[CounterCacheName]) != Counter))
            { 
                string Query = "SELECT CoreObjectID,replace(FullName,N'_',N' ') FROM CoreObject where Entity=N'"+ Entitiy + "' And ParentID = "+ParentID;
                DataTable DataOutput = Referral.DBData.SelectDataTable(Query);
                SelectList ListData = ToSelectList(DataOutput);
                HttpContext.Current.Session["CachedListOfCoreList" + Entitiy + ParentID] = ListData;
                HttpContext.Current.Session[CounterCacheName] = Counter;
            }
        }
 
        public static void FillAutoCompleteList(List<Field> Fields)
        {
            foreach (Field Item in Fields)
            { 
                if(Item !=null)
                {
                    DataTable DataOutput = new DataTable();
                    CoreObject FieldObject = CoreObject.Find(Item.CoreObjectID);
                    CoreObject TableObject = CoreObject.Find(FieldObject.ParentID);
                    if(TableObject.Entity== CoreDefine.Entities.فرم_ورود_اطلاعات)
                    {
                        InformationEntryForm informationEntryForm = new InformationEntryForm(TableObject); 
                        TableObject = CoreObject.Find(informationEntryForm.RelatedTable);
                    }
                    DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
                    string Query = "Select " + Item.FieldName + " FROM " + TableObject.FullName + " WHERE ISNULL(" + Item.FieldName + ",N'') <> N'' GROUP BY " + Item.FieldName;
                    switch (DataSourceInfo.DataSourceType)
                    {
                        case CoreDefine.DataSourceType.SQLSERVER:
                            {
                                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                DataOutput = DataBase.SelectDataTable(Query);
                                break;
                            }
                        case CoreDefine.DataSourceType.MySql:
                            {
                                MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                DataOutput = DataBase.SelectDataTable(Query);
                                break;
                            }
                        case CoreDefine.DataSourceType.ACCESS:
                            {
                                AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                DataOutput = DataBase.SelectDataTable(Query);
                                break;
                            }
                        case CoreDefine.DataSourceType.EXCEL:
                            {
                                using (ExcelEngine excelEngine = new ExcelEngine())
                                {
                                    IApplication application = excelEngine.Excel;
                                    application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;
                                    IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                    IWorksheet worksheet = workbook.Worksheets[TableObject.FullName.Replace("$", "").Replace("'", "")];
                                    DataOutput = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                                    DataOutput = DataOutput.Select(Tools.CheckExcelQuery(Item.ViewCommand)).CopyToDataTable();
                                }
                                break;
                            }
                    }
                    SelectList ListData = ToSelectTextAutoCompleteList(DataOutput);
                    HttpContext.Current.Session[Item.SystemName()] = ListData;
                }
 
            }
        } 
        public static void FillSelectList(List<Field> Fields)
        {
            try
            {
                foreach (Field Item in Fields)
                {
                    if (Item != null)
                    {
                        switch(Item.FieldType)
                        {
                            case CoreDefine.InputTypes.MultiSelectFromRelatedTable:
                            case CoreDefine.InputTypes.RelatedTable:
                                {
                                    if (Item.RelatedTable == 0 && Item.ViewCommand != "")
                                    {
                                        string DataCacheName = "CachedListOf" + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((Item.ViewCommand + Item.SpecialValue)));
                                        string CounterCacheName = "CachedCounter";
                                        int Counter = RegisterCount();

                                        if ((HttpContext.Current.Session[DataCacheName] == null) || ((HttpContext.Current.Session[CounterCacheName] == null ? 0 : (int)HttpContext.Current.Session[CounterCacheName]) != Counter))
                                        {
                                            DataTable DataOutput = Referral.DBData.SelectDataTable(Item.ViewCommand);
                                            SelectList ListData = ToSelectList(DataOutput);
                                            HttpContext.Current.Session[Item.SystemName()] = ListData;
                                            HttpContext.Current.Session["HasImage" + Item.SystemName()] = false;
                                            HttpContext.Current.Session[DataCacheName] = ListData;
                                            HttpContext.Current.Session[CounterCacheName] = Counter;
                                        }
                                        else
                                        {
                                            HttpContext.Current.Session[Item.SystemName()] = HttpContext.Current.Session[DataCacheName];
                                        }
                                    }
                                    else
                                    {

                                        DataTable DataOutput = new DataTable();
                                        CoreObject TableObject = CoreObject.Find(Item.RelatedTable);
                                        DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));

                                        string DataCacheName = Item.DataCacheName();
                                        string CounterCacheName = "CachedCounter" + "_" + Item.RelatedTable + "_" + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((Item.ViewCommand + Item.SpecialValue)));
                                        int Counter = RegisterCount(TableObject.FullName, DataSourceInfo.ServerName, DataSourceInfo.DataBase);

                                        switch (DataSourceInfo.DataSourceType)
                                        {
                                            case CoreDefine.DataSourceType.SQLSERVER:
                                                {
                                                    if (DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                                                    {
                                                        if ((HttpContext.Current.Session[DataCacheName] == null) || ((HttpContext.Current.Session[CounterCacheName] == null ? 0 : (int)HttpContext.Current.Session[CounterCacheName]) != Counter))
                                                        {
                                                            DataOutput = Referral.DBData.SelectDataTable(Tools.CheckQuery(GetRelatedTableQuery(Item)));
                                                            SelectList ListData = ToSelectList(DataOutput);
                                                            HttpContext.Current.Session[Item.SystemName()] = ListData;
                                                            HttpContext.Current.Session[DataCacheName] = ListData;
                                                            HttpContext.Current.Session[CounterCacheName] = Counter;
                                                        }
                                                        else
                                                        {
                                                            HttpContext.Current.Session[Item.SystemName()] = HttpContext.Current.Session[DataCacheName];
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                                        DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(GetRelatedTableQuery(Item)));

                                                        SelectList ListData = ToSelectList(DataOutput);
                                                        HttpContext.Current.Session[Item.SystemName()] = ListData;
                                                        HttpContext.Current.Session[DataCacheName] = ListData;
                                                        HttpContext.Current.Session[CounterCacheName] = Counter;
                                                    }
                                                    break;
                                                }
                                            case CoreDefine.DataSourceType.MySql:
                                                {
                                                    MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                                    DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(GetRelatedTableQuery(Item)));
                                                    break;
                                                }
                                            case CoreDefine.DataSourceType.ACCESS:
                                                {
                                                    AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                                    DataOutput = DataBase.SelectDataTable(Tools.CheckAccessQuery(GetRelatedTableQuery(Item)));
                                                    break;
                                                }
                                            case CoreDefine.DataSourceType.EXCEL:
                                                {
                                                    using (ExcelEngine excelEngine = new ExcelEngine())
                                                    {
                                                        IApplication application = excelEngine.Excel;
                                                        application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;
                                                        IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                                        IWorksheet worksheet = workbook.Worksheets[TableObject.FullName.Replace("$", "").Replace("'", "")];
                                                        DataOutput = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                                                        DataOutput = DataOutput.Select(Tools.CheckExcelQuery(Item.ViewCommand)).CopyToDataTable();
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            case CoreDefine.InputTypes.MultiSelectFromComboBox:
                            case CoreDefine.InputTypes.ComboBox:
                                {
                                    HttpContext.Current.Session[Item.SystemName()] = ToSelectList(Item.ComboValues());
                                    break;
                                }
                        } 
                    }

                }
            }
            catch (Exception ex)
            {

            }
        } 
        public static SelectList FillSelectList(Field Item)
        { 
            if (Item != null)
            {
                switch(Item.FieldType)
                {
                    case CoreDefine.InputTypes.MultiSelectFromRelatedTable:
                    case CoreDefine.InputTypes.RelatedTable:
                        {
                            if (Item.RelatedTable == 0 && Item.ViewCommand != "")
                            { 
                                DataTable DataOutput = Referral.DBData.SelectDataTable(Item.ViewCommand);
                                return ToSelectList(DataOutput);  
                            }
                            else
                            {

                                DataTable DataOutput = new DataTable();
                                CoreObject TableObject = CoreObject.Find(Item.RelatedTable);
                                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID)); 

                                switch (DataSourceInfo.DataSourceType)
                                {
                                    case CoreDefine.DataSourceType.SQLSERVER:
                                        {
                                            if (DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                                            { 
                                                DataOutput = Referral.DBData.SelectDataTable(Tools.CheckQuery(GetRelatedTableQuery(Item)));
                                                return ToSelectList(DataOutput);  
                                            }
                                            else
                                            {
                                                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                                DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(GetRelatedTableQuery(Item))); 
                                                return ToSelectList(DataOutput); 
                                            } 
                                        }
                                    case CoreDefine.DataSourceType.MySql:
                                        {
                                            MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                            DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(GetRelatedTableQuery(Item)));
                                            break;
                                        }
                                    case CoreDefine.DataSourceType.ACCESS:
                                        {
                                            AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                            DataOutput = DataBase.SelectDataTable(Tools.CheckAccessQuery(GetRelatedTableQuery(Item)));
                                            break;
                                        }
                                    case CoreDefine.DataSourceType.EXCEL:
                                        {
                                            using (ExcelEngine excelEngine = new ExcelEngine())
                                            {
                                                IApplication application = excelEngine.Excel;
                                                application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;
                                                IWorkbook workbook = application.Workbooks.Open(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                                IWorksheet worksheet = workbook.Worksheets[TableObject.FullName.Replace("$", "").Replace("'", "")];
                                                DataOutput = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                                                DataOutput = DataOutput.Select(Tools.CheckExcelQuery(Item.ViewCommand)).CopyToDataTable();
                                            }
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case CoreDefine.InputTypes.MultiSelectFromComboBox:
                    case CoreDefine.InputTypes.ComboBox:
                        {
                            return ToSelectList(Item.ComboValues()); 
                        }
                } 
            }
            DataTable dataTable = null;
            return ToSelectList(dataTable);
        } 

        public static string GetRelatedTableQuery(Field Field)
        { 
            List<CoreObject> DefaultFieldName = CoreObject.DefaultChild(Field.RelatedTable);
            Table Table = new Table(CoreObject.Find(Field.RelatedTable));

            string TableIDField = Table.IDField().FieldName;
            string Query = "Select " + Table.FullName + "." + TableIDField + ", ";
            foreach (CoreObject CoreFieldItem in DefaultFieldName)
            {
                Field FieldItem = new Field(CoreFieldItem);
                if(FieldItem.FieldType== CoreDefine.InputTypes.RelatedTable) 
                    Query += CheckExternalField(FieldItem, Table.FullName) + "+N' '+ ";
                else if (FieldItem.FieldName != "عکس") 
                    Query += "IsNull(CAST("+ Table.FullName + "." + FieldItem.FieldName + " as nvarchar(400)),N'')+N' '+ "; 
            }

            Query = Query.Replace("+N' - '+ +N' - '+", "+N' - '+");
            Query = Query.Substring(0, Query.Length - 7);
             
            Query += " From " + Table.TABLESCHEMA+"."+ Table.FullName;

            if (!string.IsNullOrEmpty(Field.ViewCommand))
                if(Field.ViewCommand.Replace(" ","").ToUpper().IndexOf("WHERE")>-1 || Field.ViewCommand.Replace(" ", "").IndexOf("در.صورتی.که") >-1)
                    Query += " " + Field.ViewCommand;
                else
                    Query += " WHERE " + Field.ViewCommand;

            return Query;
        }
        public static string GetRelatedTableQueryForDashboard(Field Field)
        { 
            List<CoreObject> DefaultFieldName = CoreObject.DefaultChild(Field.RelatedTable);
            CoreObject FieldTableCore = CoreObject.Find(CoreObject.Find(Field.CoreObjectID).ParentID);
            Table Table = new Table(CoreObject.Find(Field.RelatedTable));
            string TableIDField = Table.IDField().FieldName;
            string Query = "(Select ";
            foreach (CoreObject CoreFieldItem in DefaultFieldName)
            {
                Field FieldItem = new Field(CoreFieldItem);
                if(FieldItem.FieldType== CoreDefine.InputTypes.RelatedTable) 
                    Query += CheckExternalField(FieldItem, Table.FullName) + "+N' '+ ";
                else if (FieldItem.FieldName != "عکس") 
                    Query += "CAST([" + Table.FullName +"].["+ FieldItem.FieldName + "] as nvarchar(400))+N' '+ "; 
            }

            Query = Query.Replace("+N' - '+ +N' - '+", "+N' - '+");
            Query = Query.Substring(0, Query.Length - 7);
             
            Query += " From " + Table.FullName;

            Query += " Where [" + Table.FullName+"].["+ TableIDField+"] = ["+ FieldTableCore.FullName+"].["+ Field.FieldName+"]";

            return Query+")";
        } 
        public static void FillRelatedTableReportParameter(ReportParameter Parameter)
        {  
            if(Parameter.RelatedTable ==0 && Parameter.ViewCommand!="")
            {
                string DataCacheName = "CachedListOf" + Convert.ToBase64String(Encoding.Unicode.GetBytes((Parameter.ViewCommand + Parameter.FullName)));
                string CounterCacheName = "CachedCounter";
                int Counter = RegisterCount();

                if ((HttpContext.Current.Session[DataCacheName] == null) || ((HttpContext.Current.Session[CounterCacheName] == null ? 0 : (int)HttpContext.Current.Session[CounterCacheName]) != Counter))
                {
                    DataTable DataOutput = Referral.DBData.SelectDataTable(Parameter.ViewCommand);
                    SelectList ListData = ToSelectList(DataOutput);
                    HttpContext.Current.Session[Parameter.SystemName()] = ListData;
                    HttpContext.Current.Session["HasImage" + Parameter.SystemName()] = false;
                    HttpContext.Current.Session[DataCacheName] = ListData;
                    HttpContext.Current.Session[CounterCacheName] = Counter;

                }
                else
                {
                    HttpContext.Current.Session[Parameter.SystemName()] = HttpContext.Current.Session[DataCacheName];
                }
            }
            else
            {
                CoreObject RelatedTableInfo = CoreObject.Find(Parameter.RelatedTable);

                string DataCacheName = "CachedListOf" + RelatedTableInfo.FullName + Convert.ToBase64String(Encoding.Unicode.GetBytes((Parameter.ViewCommand + Parameter.FullName)));
                string CounterCacheName = "CachedCounter" + RelatedTableInfo.FullName;

                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(RelatedTableInfo.ParentID)); 
                int Counter = RegisterCount(RelatedTableInfo.FullName,DataSourceInfo.ServerName,DataSourceInfo.DataBase);

                if ((HttpContext.Current.Session[DataCacheName] == null) || ((HttpContext.Current.Session[CounterCacheName] == null ? 0 : (int)HttpContext.Current.Session[CounterCacheName]) != Counter))
                {
                    bool HasImage = false;
                    string TableName = RelatedTableInfo.FullName;
                    List<CoreObject> DefaultFieldName = CoreObject.DefaultChild(Parameter.RelatedTable);
                    string Query = "Select " + TableName + ".شناسه, ";

                    foreach (CoreObject CoreFieldItem in DefaultFieldName)
                    {
                        Field FieldItem = new Field(CoreFieldItem);

                        if (FieldItem.FieldName != "عکس")
                            Query += CheckExternalField(FieldItem, TableName) + "+N' - '+ ";
                        else
                            HasImage = true;
                    }
                    Query = Query.Replace("+N' - '+ +N' - '+", "+N' - '+");
                    Query = Query.Substring(0, Query.Length - 9);

                    if (HasImage)
                        Query += "," + TableName + ".عکس";

                    Query += " From " + Parameter.ExTable().FullName;

                    if (!string.IsNullOrEmpty(Parameter.ViewCommand))
                        Query += " WHERE " + Parameter.ViewCommand;


                    DataTable DataOutput = SelectDataTable(DataSourceInfo, Query, RelatedTableInfo.FullName) ;
                    SelectList ListData = ToSelectList(DataOutput);
                    HttpContext.Current.Session[Parameter.SystemName()] = ListData;
                    HttpContext.Current.Session["HasImage" + Parameter.SystemName()] = HasImage;
                    HttpContext.Current.Session[DataCacheName] = ListData;
                    HttpContext.Current.Session[CounterCacheName] = Counter;

                }
                else
                {
                    HttpContext.Current.Session[Parameter.SystemName()] = HttpContext.Current.Session[DataCacheName];
                }
            }
                         
        } 
 
        public static string CheckExternalField(Field _Field,string TableName)
        { 
            string ResultValue = "";
            if (_Field.FieldType == CoreDefine.InputTypes.RelatedTable)
            {
                List<CoreObject> DefaultFieldName = CoreObject.DefaultChild(_Field.RelatedTable);
                CoreObject ChildTableName = CoreObject.Find(_Field.RelatedTable);
                Table table=new Table(ChildTableName);
                foreach (CoreObject CoreFieldItem in DefaultFieldName)
                {
                    Field FieldItem = new Field(CoreFieldItem);
                    if (DefaultFieldName.Count > 1)
                        ResultValue += "+N' - '+";
                    if (FieldItem.FieldName != "عکس")
                        ResultValue += "isnull((Select " + CheckExternalField(FieldItem, ChildTableName.FullName) + " From " + table.TABLESCHEMA+"."+ ChildTableName.FullName + " Where " + ChildTableName.FullName + ".شناسه = "+ TableName+"." + _Field.FieldName + "),N'')";

                }
            }
            else if (_Field.FieldType == CoreDefine.InputTypes.Number)
                ResultValue = " Cast( " + TableName + "." + _Field.FieldName + " as nvarchar(4000)) ";
            else
            ResultValue = "isnull( Cast( " +TableName + "." + _Field.FieldName + " as nvarchar(4000)),N'') ";
            return ResultValue;
        }

        public static void ViewFormat(ref DataTable _DataTable, List<Field> _Fields)
        {
            for (int i = 0; i < _Fields.Count; i++)
            {
                switch (_Fields[i].FieldType)
                {

                    case CoreDefine.InputTypes.RelatedTable:
                        if(_DataTable != null)
                        for (int j = 0; j < _DataTable.Rows.Count; j++)
                        {
                            if (_DataTable.Rows[j][i] == DBNull.Value)
                                _DataTable.Rows[j][i] = 0;
                        }
                        break;

                    case CoreDefine.InputTypes.SingleSelectList:
                        if (_DataTable != null)
                        for (int j = 0; j < _DataTable.Rows.Count; j++)
                        {
                            if (_DataTable.Rows[j][i] == DBNull.Value)
                                _DataTable.Rows[j][i] = "";
                        }
                        break; 

                    default:
                        break;

                }
            }
        }

        public static int RegisterCount()
        { 
            return (int)Referral.DBRegistry.SelectField("Select(Select COUNT(1) From Delete_APMRegistry) + (Select COUNT(1) From Update_APMRegistry) + (Select COUNT(1) From Insert_APMRegistry)", null);
        }
        public static int RegisterCount(string TableName,string ServerName,string DatabaseName)
        { 
            try
            {
                return (int)Referral.DBRegistry.SelectField("Select(Select COUNT(1) From Delete_APMRegistry Where TableName = " + Tools.N(TableName) + " And ServerName=" + Tools.N(ServerName) + " And DatabaseName=" + Tools.N(DatabaseName) + " ) + (Select COUNT(1) From Update_APMRegistry Where TableName = " + Tools.N(TableName) + "  And ServerName=" + Tools.N(ServerName) + " And DatabaseName=" + Tools.N(DatabaseName) + " ) + (Select COUNT(1) From Insert_APMRegistry Where TableName = " + Tools.N(TableName) + " And ServerName=" + Tools.N(ServerName) + " And DatabaseName=" + Tools.N(DatabaseName) + ")", null);
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public static string ArabicToPersion(string _value)
        {
            return _value.Replace('ك', 'ک').Replace('ي', 'ی');
        }
        public static string NationalPlaque(string _value)
        {
            string _char ="128";
            _value = ArabicToPersion(_value);
            if (_value.Length == 8)
                _value = _value.Substring(0, 2) + _char[0] + _value.Substring(2, 1) + _char[0] + _value.Substring(3, 3) + _char[0] + _value.Substring(6, 2);

            return _value;
        }

        private const int Keysize = 256; 
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
 
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        { 
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText); 
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray(); 
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray(); 
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }


        public class ForeingKeyDataPack
        {
            public string DataKey;
            public string FieldName;
            public int Index;
            public string GridName;
            public List<JavaSelectListItem> Values;

            public ForeingKeyDataPack(string _DataKey, string _FieldName, int _Index, string _GridName, SelectList _Values)
            {
                DataKey = _DataKey;
                FieldName = _FieldName;
                Index = _Index;
                GridName = _GridName;

                List<JavaSelectListItem> Items = new List<JavaSelectListItem>();
                foreach (SelectListItem Item in _Values)
                {
                    Items.Add(new JavaSelectListItem(Item.Text, Item.Value));
                }

                Values = Items;
            }
        }


        public class JavaSelectListItem
        {
            public string text;
            public string value;

            public JavaSelectListItem(string _text, string _value)
            {
                text = _text;
                value = _value;
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
                        if(ColumnsNameData!="")
                        {
                            int idx = ColumnsNameData.IndexOf(":");
                            if(idx>-1)
                            {
                                string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                                if (!ColumnsName.Contains(ColumnsNameString))
                                    ColumnsName.Add(ColumnsNameString);
                            }
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
                        if(rowData!="")
                        { 
                            int index = rowData.IndexOf(":");
                            if(index>-1)
                            {
                                string RowColumns = rowData.Substring(0, index - 1).Replace("\"", "");
                                string RowDataString = rowData.Substring(index + 1).Replace("\"", "");
                                row[RowColumns] = RowDataString;
                            }
                        }
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


        public static SelectList FillSelectListWithQuery(string SpecialWordFullName, string DeclareQuery="")
        {
            List<SelectListItem> Output = new List<SelectListItem>();
            Output.Add(new SelectListItem() { Value = "0", Text = " ", Selected = true });
             
            if(SpecialWordFullName!=null)
            {
                CoreObject SpecialWordCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, SpecialWordFullName);
                SpecialPhrase SpecialPhrase = new SpecialPhrase(SpecialWordCore);
                DeclareQuery = DeclareQuery == null ? "" : DeclareQuery; 
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, SpecialWordFullName);
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                SelectList ListData = null;
                if (SpecialPhrase.DataSourceID == Referral.MasterDatabaseID)
                    ListData = ToSelectList(Referral.DBData.SelectDataTable(Tools.CheckQuery(DeclareQuery + "\n" + specialPhrase.Query)));
                else
                {
                    DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(SpecialPhrase.DataSourceID));
                    SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                    ListData = ToSelectList(DataBase.SelectDataTable(Tools.CheckQuery(DeclareQuery + "\n" + specialPhrase.Query)));
                }
                return ListData;
            }  
            return (SelectList)new SelectList(Output, "Value", "Text", 0);
        }



    }
}