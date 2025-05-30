using APM.Models;
using APM.Models.Database;
using APM.Models.Tools;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class ImportDataController : Controller
    {
        // GET: ImportData
        public ActionResult Index()
        {
            return View();
        }
         
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;
            List<string> ImportDataFileNameList = new List<string>();
            List<DataTable> ImportDataList = new List<DataTable>();
            string Message = "خطا در ذخیره سازی";


            if (files != null)
            {
                var file = files.ElementAt(0);

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;

                    application.DefaultVersion = file.FileName.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                    IWorkbook workbook = application.Workbooks.Open(file.InputStream);

                    for(int index=0; index< workbook.Worksheets.Count; index++)
                    {
                        IWorksheet worksheet = workbook.Worksheets[index];


                        DataTable WorksheetwData = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);

                        if (Referral.DBData.ConnectionData.DataBase == "NisocDCSData")
                        {
                            CoreObject FlightCore = CoreObject.Find(CoreDefine.Entities.جدول, "Flight");
                            CoreObject FlightPassengersCore = CoreObject.Find(CoreDefine.Entities.جدول, "FlightPassengers");
                            int StationID = 0;
                            int DestinationID = 0;
                            switch(WorksheetwData.Rows[0][0].ToString())
                            {
                                case "THR": StationID = 2; break;
                                case "AKW": StationID = 1; break;
                            }
                            switch(WorksheetwData.Rows[0][1].ToString())
                            {
                                case "THR": DestinationID = 2; break;
                                case "AKW": DestinationID = 1; break;
                            }

                            long ParentID = Desktop.Create(FlightCore.CoreObjectID.ToString(),
                                                                 new string[] { "Airplane", "Station", "Destination", "Num", "ShamsiDate" },
                                                                 new object[] { 1, StationID, DestinationID, WorksheetwData.Rows[0][2], WorksheetwData.Rows[0][3] });
                            if(ParentID > 0)
                                for(int RowIndex=2;RowIndex<WorksheetwData.Rows.Count;RowIndex++)
                                { 
                                        if(WorksheetwData.Rows[RowIndex][0].ToString()!="")
                                            Desktop.Create(FlightPassengersCore.CoreObjectID.ToString(),
                                                                    new string[] { "Flight","CardNumber","Action","AgeType","Gender","EnglishFirstName","EnglishLastName","PersianFirstName","PersianLastName","MiladiBirthDate","WithAdult","NationalID","PassportID","Mobile","Email","SSRCode","AgencyPNR","AgencyTicketNo" },
                                                                    new object[] { ParentID, WorksheetwData.Rows[RowIndex][0], WorksheetwData.Rows[RowIndex][1], WorksheetwData.Rows[RowIndex][2], WorksheetwData.Rows[RowIndex][3], WorksheetwData.Rows[0][4], WorksheetwData.Rows[RowIndex][5], WorksheetwData.Rows[RowIndex][6], WorksheetwData.Rows[RowIndex][7], WorksheetwData.Rows[RowIndex][8], WorksheetwData.Rows[RowIndex][9], WorksheetwData.Rows[RowIndex][10] , WorksheetwData.Rows[RowIndex][11], WorksheetwData.Rows[RowIndex][12], WorksheetwData.Rows[RowIndex][13], WorksheetwData.Rows[RowIndex][14], WorksheetwData.Rows[RowIndex][15], WorksheetwData.Rows[RowIndex][16] });
                                }
                            
                        }
                        ImportDataFileNameList.Add(worksheet.CodeName);
                        ImportDataList.Add(WorksheetwData); 
                    } 
                }

                if (Referral.DBData.ConnectionData.DataBase != "NisocDCSData")
                {
                    Session["ImportDataFileNameList"] = ImportDataFileNameList;
                    Session["ImportDataList"] = ImportDataList;
                    Result = true;
                } 

            }
            return Content(Result ? "" : Message);
        }


        public ActionResult LoadContentFile(string DataKey, string ParentID )
        {
            ViewData["DataKey"] = DataKey;
            ViewData["ParentID"] = ParentID;
            CoreObject ElementCore=CoreObject.Find(long.Parse(DataKey));
            long TableID= ElementCore.CoreObjectID;
            switch (ElementCore.Entity)
            {
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        TableID=Desktop.DataInformationEntryForm[DataKey].RelatedTable;
                        break;
                    }
                
            }

            List<CoreObject> FieldList=CoreObject.FindChilds(TableID,CoreDefine.Entities.فیلد); 
            List<DataTable> ImportDataList = (List<DataTable>)Session["ImportDataList"];
            List<string> ImportDataFileNameList = (List<string>)Session["ImportDataFileNameList"];
            if(ImportDataList!=null)
                for (int Index=0;Index<ImportDataList.Count;Index++)
                { 
                    DataTable DataItem = ImportDataList[Index];
                    string Message = "";
                    if(DataItem.Columns.Count>FieldList.Count)
                    {
                        Message= "تعداد ستون ها از ستون های جدول مرتبط بیشتر است";
                        break;
                    }
                    else
                    {
                        foreach(DataColumn dataColumn in DataItem.Columns)
                        {
                            bool IsFindField=false;
                            foreach(CoreObject FieldCore in FieldList)
                            {
                                if(FieldCore.FullName== dataColumn.ColumnName)
                                {
                                    IsFindField=true;
                                    break;
                                }    
                            }
                            if(!IsFindField)
                            {
                                Message = "نام ستون های موجود با ستون های جدول اصلی مطابقت ندارند";
                                break;
                            }
                        }
                    }
                    ViewData["ImportDataMessage" + Tools.SafeTitle(ImportDataFileNameList[Index])] = Message;
                }
            return View("~/Views/ImportData/LoadContentFile.cshtml");
        }

        public ActionResult Read(string FileName, [DataSourceRequest] DataSourceRequest _Request)
        { 
            JsonResult jsonResult = new JsonResult();
            List<string> ImportDataFileNameList = (List<string>)Session["ImportDataFileNameList"];
            List<DataTable> ImportDataList = (List<DataTable>)Session["ImportDataList"];
            DataTable OutPut=new DataTable();
            for(int index=0;index< ImportDataFileNameList.Count; index++)
            {
                if(ImportDataFileNameList[index] ==FileName)
                {
                    OutPut = ImportDataList[index];
                    break;
                }
            }
            jsonResult = Json(OutPut.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public ActionResult ExportTempeletFileDataButtonClick(string DataKey, string ParentID)
        {
            List<DataTable> WorkbooksDataList = new List<DataTable>();
            List<string> WorkbooksNameList = new List<string>(); 
            CoreObject ElementCore = CoreObject.Find(long.Parse(DataKey));
            string ResultFileName = "نمونه فایل ایمپورت " + Tools.UnSafeTitle(ElementCore.FullName) + ".xlsx";
            string FilePath = System.Web.HttpContext.Current.Server.MapPath("~/Attachment" + "/" + Referral.UserAccount.UsersID.ToString() + "/");
            if(!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
            else
            {
                Directory.Delete(FilePath, true);
                Directory.CreateDirectory(FilePath);

            } 
            switch (ElementCore.Entity)
            {
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    { 
                        ElementCore= CoreObject.Find(Desktop.DataInformationEntryForm[DataKey].RelatedTable);
                        break;
                    } 

            }

            DataTable FileData = new DataTable();
            List<CoreObject> FieldCoreList = CoreObject.FindChilds(ElementCore.CoreObjectID, CoreDefine.Entities.فیلد); 
            object[] FieldItemList = new object[FieldCoreList.Count];
            DataSourceInfo TableDataSourceInfo = new DataSourceInfo(CoreObject.Find(ElementCore.ParentID));
            switch (TableDataSourceInfo.DataSourceType)
            {
                case CoreDefine.DataSourceType.SQLSERVER:
                    {
                        if (TableDataSourceInfo.ServerName == Referral.DBData.ConnectionData.Source && TableDataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                        {
                            FileData = Referral.DBData.SelectDataTable("Select Top 10 * from "+ElementCore.FullName);
                        }
                        else
                        {
                            SQLDataBase DataBase = new SQLDataBase(TableDataSourceInfo.ServerName, TableDataSourceInfo.DataBase, TableDataSourceInfo.Password, TableDataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                            FileData = DataBase.SelectDataTable("Select Top 10 * from " + ElementCore.FullName);
                        }
                        break;
                    }
                case CoreDefine.DataSourceType.MySql:
                    {
                        MySqlDatabase DataBase = new MySqlDatabase(TableDataSourceInfo.ServerName, TableDataSourceInfo.DataBase, TableDataSourceInfo.Password, TableDataSourceInfo.UserName);
                        FileData = DataBase.SelectDataTable("Select Top 10 * from " + ElementCore.FullName);
                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        AccessDatabase DataBase = new AccessDatabase(TableDataSourceInfo.FilePath, TableDataSourceInfo.DataBase, TableDataSourceInfo.Password);
                        FileData = DataBase.SelectDataTable("Select Top 10 * from " + ElementCore.FullName);
                        break;
                    }
                case CoreDefine.DataSourceType.EXCEL:
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            IApplication application = excelEngine.Excel;
                            application.DefaultVersion = TableDataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;
                            IWorkbook workbook = application.Workbooks.Open(TableDataSourceInfo.FilePath + "\\" + TableDataSourceInfo.DataBase);
                            IWorksheet worksheet = workbook.Worksheets[ElementCore.FullName.Replace("$", "").Replace("'", "")];
                            FileData = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames); 
                        }
                        break;
                    }
            }

            WorkbooksDataList.Add(FileData);
            WorkbooksNameList.Add("نمونه فایل اصلی");

            for(int Index=0;Index< FieldCoreList.Count;Index++)
            { 
                Field field = new Field(FieldCoreList[Index]);
                DataTable DataOutput=new DataTable();
                string FieldType = "";
                string FieldComment = "";
                if (field.FieldNature == "Bigint")
                    FieldType = "عدد";
                else if (field.FieldNature == "Float")
                    FieldType = "عدد اعشاری";
                else if (field.FieldNature == "Bit")
                    FieldType = "دو مقدار (0 یا 1)";
                else
                    FieldType = "رشته";

                if (field.FieldType==CoreDefine.InputTypes.RelatedTable)
                {
                    CoreObject TableObject = CoreObject.Find(field.RelatedTable);
                    if (WorkbooksNameList.IndexOf(TableObject.FullName) == -1)
                    {
                        DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableObject.ParentID));
                        switch (DataSourceInfo.DataSourceType)
                        {
                            case CoreDefine.DataSourceType.SQLSERVER:
                                {
                                    if (DataSourceInfo.ServerName == Models.Referral.DBData.ConnectionData.Source && DataSourceInfo.DataBase == Models.Referral.DBData.ConnectionData.DataBase)
                                    {
                                        DataOutput = Models.Referral.DBData.SelectDataTable(Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(field)));
                                    }
                                    else
                                    {
                                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                                        DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(field)));
                                    }
                                    break;
                                }
                            case CoreDefine.DataSourceType.MySql:
                                {
                                    MySqlDatabase DataBase = new MySqlDatabase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName);
                                    DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(field)));
                                    break;
                                }
                            case CoreDefine.DataSourceType.ACCESS:
                                {
                                    AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                    DataOutput = DataBase.SelectDataTable(Tools.CheckQuery(DataConvertor.GetRelatedTableQuery(field)));
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
                                        DataOutput = DataOutput.Select(Tools.CheckExcelQuery(field.ViewCommand)).CopyToDataTable();
                                    }
                                    break;
                                }
                        }

                        if(DataOutput!=null)
                        {
                            WorkbooksDataList.Add(DataOutput);
                            WorkbooksNameList.Add(TableObject.FullName);
                        }
                    }


                    FieldComment = "از شیت " + TableObject.FullName + " انتخاب شود";
                }
                FieldItemList[Index] = field.FieldName + " : " + FieldType + " : " + FieldComment;  
            }

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Create(WorkbooksDataList.Count + 1);
                IWorksheet worksheet = workbook.Worksheets[0];
                worksheet.Name = "راهنما";
                worksheet.IsRightToLeft = true;
                worksheet.ImportArray(FieldItemList, 1,1, true);

                for (int Index=0;Index<WorkbooksDataList.Count;Index++)
                {
                    try
                    {
                        DataTable SheetData = WorkbooksDataList[Index];
                        IWorksheet RelatedWorksheet = workbook.Worksheets[Index + 1];
                        RelatedWorksheet.Name = WorkbooksNameList[Index];
                        RelatedWorksheet.IsRightToLeft = true;
                        RelatedWorksheet.ImportDataTable(SheetData, true, 1, 1);
                    }
                    catch {

                    }
                }

                try
                { 
                    workbook.SaveAs(FilePath + ResultFileName);
                    workbook.Close();
                    excelEngine.Dispose();
                }
                catch (Exception ex)
                {

                }
            }
            FileInfo file = new FileInfo(FilePath + ResultFileName);
            string contentType = MimeMapping.GetMimeMapping(file.Name);
            var readStream = System.IO.File.ReadAllBytes(FilePath + ResultFileName);
            return File(readStream, contentType );
        }


    }
}