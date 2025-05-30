using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.Tools;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Models.DesktopManagement
{
    public static class GridGroupMenuTree
    { 
        public static void GridGroupBaseMenuFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base,string DataKey)
        { 
            Field GroupableField = new Field(CoreObject.Find(Desktop.DataInformationEntryForm[@DataKey].GroupableField)); 
            SelectList ListData ;
            List<SelectListItem> ListDataItem = new List<SelectListItem>();
            string[] Color = { "#09ff00", "#ff0000", "#00d4ff" ,"#0009ff", "#6600ff", "#d100ff", "#ff004e", "#ff4c00" };
            string Query = string.Empty;
            switch (GroupableField.FieldType)
            {
                case CoreDefine.InputTypes.RelatedTable:
                    {
                        if (HttpContext.Current.Session[GroupableField.DataCacheName()] == null)
                        {
                            CoreObject TableCore = CoreObject.Find(Desktop.DataInformationEntryForm[@DataKey].RelatedTable);
                            Table Table = new Table(CoreObject.Find(GroupableField.RelatedTable));
                            List<CoreObject> DefaultFieldName = CoreObject.DefaultChild(GroupableField.RelatedTable);
                            string TableIDField = Table.IDField().FieldName;

                            Query = "Select " + GroupableField.FieldName + ",(Select ";
                            foreach (CoreObject CoreFieldItem in DefaultFieldName)
                            {
                                Field FieldItem = new Field(CoreFieldItem);
                                if (FieldItem.FieldType == CoreDefine.InputTypes.RelatedTable)
                                    Query += DataConvertor.CheckExternalField(FieldItem, Table.FullName) + "+N' '+ ";
                                else if (FieldItem.FieldName != "عکس")
                                    Query += "CAST(" + FieldItem.FieldName + " as nvarchar(400))+N' '+ ";
                            }

                            Query = Query.Replace("+N' - '+ +N' - '+", "+N' - '+");
                            Query = Query.Substring(0, Query.Length - 7);

                            Query += " From " + Table.FullName + " Where " + Table.FullName + "." + TableIDField+"=" + GroupableField.FieldName +")  From " + TableCore.FullName + " Where " + GroupableField.FieldName + " is not and "+ GroupableField.FieldName + " > 0  null Group by " + GroupableField.FieldName;

                            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                            switch (DataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        if (DataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && DataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                                            ListData = DataConvertor.ToSelectList(Referral.DBData.SelectDataTable(Query)); 
                                        else
                                        {
                                            SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                            ListData = DataConvertor.ToSelectList(DataBase.SelectDataTable(Query)); 
                                        }
                                        break;
                                    }
                                case CoreDefine.DataSourceType.MySql:
                                    {
                                        MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                        ListData = DataConvertor.ToSelectList(DataBase.SelectDataTable(Query));
                                        break;
                                    }
                                case CoreDefine.DataSourceType.ACCESS:
                                    {
                                        AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password); 
                                        ListData = DataConvertor.ToSelectList(DataBase.SelectDataTable(Query));
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
                                            DataTable DataOutput = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                                            DataOutput = DataOutput.Select(Query).CopyToDataTable();
                                        }
                                        break;
                                    }
                            } 
                        }
                        else
                        {
                            ListData=(SelectList)HttpContext.Current.Session[GroupableField.DataCacheName()];
                        }
                        break;
                    }
                case CoreDefine.InputTypes.TwoValues:
                    { 
                        ListDataItem.Add(new SelectListItem(){Value = "0",Text = GroupableField.ComboValues()[0]}); 
                        ListDataItem.Add(new SelectListItem(){Value = "1",Text = GroupableField.ComboValues()[1]}); 
                        break;
                    }
                case CoreDefine.InputTypes.SingleSelectList:
                    {
                        string[] FieldValue = GroupableField.ComboValues();
                        foreach(string FieldValueItem in FieldValue)
                        {
                            ListDataItem.Add(new SelectListItem()
                            {
                                Value = FieldValueItem,
                                Text = FieldValueItem
                            });
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
            }
            ListData = new SelectList(ListDataItem, "Value", "Text", 0);
            int ColorIndex = 0;
           _Base.Add().Text(@"<span class=""k-icon fa fa-ball-pile TreeIconColor""></span>" + "همه").Id("GroupableField_All").Selected(true).Encoded(false);
            foreach(var Item in ListData)
            {
                _Base.Add().Text(@"<span class=""k-icon fa fa-circle-small TreeIconColor"" style=""color:" + Color[ColorIndex] +@";""></span>" + Item.Text).Id("GroupableField_"+ Item.Value).Encoded(false);
                ColorIndex++;
                if (ColorIndex == Color.Length) 
                    ColorIndex = 0;
            } 
          
        }
    }
}