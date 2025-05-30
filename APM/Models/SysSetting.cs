using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.NetWork;
using APM.Models.Tools;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace APM.Models
{
    public class SysSetting
    {
        public static void CreateSQLDatabase(DataSourceInfo DataSourceInfo, ref string Error,ref string DataBaseName,ref string FilePath )
        { 
            if (DataSourceInfo.CheckConnected())
                Error = "پایگاه داده قبلاً ایجاد شده بود";
            else
            {
                if (string.IsNullOrEmpty(DataSourceInfo.UserName) || string.IsNullOrEmpty(DataSourceInfo.Password) || string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.ServerName))
                    Error = "فیلد های خالی را پر نمایید";

                DataSourceInfo.FilePath = string.IsNullOrEmpty(DataSourceInfo.FilePath) ? @"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA" : DataSourceInfo.FilePath;
                DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                if (!Attachment.CheckExistsDirectory(DataSourceInfo.FilePath))
                    Error = "مسیر انتخاب شده نادرست می باشد";
                else
                { 
                    //if (DataSourceInfo.CheckConnected())
                    //{
                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, "master", DataSourceInfo.Password, DataSourceInfo.UserName, Models.Tools.SQLDataBase.SQLVersions.SQL2008);
                        if (!DataBase.CreateDataBase(DataSourceInfo.FilePath, DataSourceInfo.DataBase))
                            Error = "ایجاد پایگاه داده با شکست مواجه شد";
                    //}
                    //else
                    //    Error = "برقراری ارتباط با پایگاه داده با شکست مواجه شد";

                }

            }

            FilePath = DataSourceInfo.FilePath;
            DataBaseName = DataSourceInfo.DataBase;
        }

        public static void CreateExcelDatabase(DataSourceInfo DataSourceInfo, ref string Error, ref string DataBaseName, ref string FilePath)
        {

            if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
            else
            {
                DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                DataSourceInfo.DataBase +=Tools.Tools.GetExcelFormat(DataSourceInfo.DataBase);

                if (Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                    Error = "فایل قبلا ایجاد شده بود";
                else
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;

                        IWorkbook workbook = application.Workbooks.Create(1);
                        IWorksheet worksheet = workbook.Worksheets[0];

                        try
                        {
                            workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                        }
                        catch (Exception ex)
                        {
                            ConnectToShareFolderFile connectToShareFolderFile = new ConnectToShareFolderFile(DataSourceInfo.FilePath, DataSourceInfo.UserName, DataSourceInfo.Password);
                            if (!connectToShareFolderFile.CheckConnected())
                                Error = "عدم برقراری ارتباط با مسیر وارد شده";
                        }
                    }
                }
            }


            FilePath = DataSourceInfo.FilePath;
            DataBaseName = DataSourceInfo.DataBase;
        }

        public static void CreateAccessDatabase(DataSourceInfo DataSourceInfo, ref string Error, ref string DataBaseName, ref string FilePath)
        {
            if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
            else
            {
                DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                DataSourceInfo.DataBase +=Tools.Tools.GetAccessFormat(DataSourceInfo.DataBase);

                if (Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                    Error = "فایل قبلا ایجاد شده بود";
                else
                {
                    string SourceFile = System.Web.HttpContext.Current.Server.MapPath("~/Dependencies/Database.accdb");
                    if (Models.Attachment.CopyFile(new FileInfo(SourceFile), new FileInfo(DataSourceInfo.FilePath)))
                        Models.Attachment.RenameFile(new FileInfo(DataSourceInfo.FilePath + "Database.accdb"), new FileInfo(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase));
                }
            }

            FilePath = DataSourceInfo.FilePath;
            DataBaseName = DataSourceInfo.DataBase;
        }

        public static void  RebuildDataBase(long BaseCoreID, ref string Error ,ref string AlarmRebuilding)
        {

            List<CoreObject> ChieldList = CoreObject.FindChilds(BaseCoreID);
            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(BaseCoreID));

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
                        DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                        DataSourceInfo.DataBase += Tools.Tools.GetExcelFormat(DataSourceInfo.DataBase);
                        if (!Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                            Error = "فایل یافت نشد";

                        break;
                    }
                case CoreDefine.DataSourceType.ACCESS:
                    {
                        if (string.IsNullOrEmpty(DataSourceInfo.DataBase) || string.IsNullOrEmpty(DataSourceInfo.FilePath))
                            Error = "فیلد های نام پایگاه داده و مسیر را وارد نمایید";
                        DataSourceInfo.FilePath += DataSourceInfo.FilePath.Substring(DataSourceInfo.FilePath.Length - 1, 1) == @"\" ? "" : @"\";
                        DataSourceInfo.DataBase += Tools.Tools.GetAccessFormat(DataSourceInfo.DataBase);
                        if (!Models.Attachment.CheckExistsFile(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase))
                            Error = "فایل یافت نشد";

                        break;
                    }
            }

            if (Error == "")
                if (ChieldList.Count > 0)
                {
                    switch (DataSourceInfo.DataSourceType)
                    {
                        case CoreDefine.DataSourceType.SQLSERVER:
                            {
                                List<CoreObject> TableList = CoreObject.FindChilds(BaseCoreID, CoreDefine.Entities.جدول);
                                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, Models.Tools.SQLDataBase.SQLVersions.SQL2008);
                                foreach (CoreObject Table in TableList)
                                {
                                    string Query = "CREATE TABLE dbo." + Table.FullName + "(" + Tools.Tools.NewLine;
                                    string IdentityField = "";

                                    List<CoreObject> FieldList = CoreObject.FindChilds(Table.CoreObjectID, CoreDefine.Entities.فیلد);

                                    foreach (CoreObject FieldItem in FieldList)
                                    {
                                        Field Field = new Field(FieldItem);
                                        if (!Field.IsIdentity)
                                            Query += FieldItem.FullName + " " + Field.FieldNature + " NULL ," + Tools.Tools.NewLine;
                                        else
                                        {
                                            Query += FieldItem.FullName + " " + Field.FieldNature + " IDENTITY(1,1) NOT NULL ," + Tools.Tools.NewLine;
                                            IdentityField = FieldItem.FullName;
                                        }
                                    }
                                    Query += "PRIMARY KEY (" + IdentityField + "))";
                                    DataBase.Execute(Query);

                                }
                                break;
                            }
                        case CoreDefine.DataSourceType.EXCEL:
                            {
                                char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                                List<CoreObject> TableList = CoreObject.FindChilds(BaseCoreID, CoreDefine.Entities.جدول);

                                using (ExcelEngine excelEngine = new ExcelEngine())
                                {
                                    IApplication application = excelEngine.Excel;

                                    application.DefaultVersion = DataSourceInfo.DataBase.EndsWith(".xlsx") ? ExcelVersion.Xlsx : ExcelVersion.Excel97to2003;
                                    IWorkbook workbook = application.Workbooks.Create(TableList.Count);
                                    int TableCounter = 0;
                                    string[] WorkSheetName = new string[TableList.Count];

                                    for (int i = 0; i < TableList.Count - 1; i++)
                                    {
                                        IWorksheet worksheet = workbook.Worksheets[i];
                                        WorkSheetName[i] = worksheet.Name;
                                    }

                                    TableCounter = 0;
                                    foreach (CoreObject TableItem in TableList)
                                    {
                                        string CharTemp = "";
                                        int CharCounter = 0;
                                        int CharCounter2 = 0;

                                        IWorksheet worksheet = workbook.Worksheets[TableCounter];
                                        if (Array.IndexOf(WorkSheetName, TableItem.FullName) == -1)
                                            worksheet.Name = TableItem.FullName.Replace("$", "").Replace("'", "");

                                        List<CoreObject> FildList = CoreObject.FindChilds(TableItem.CoreObjectID, CoreDefine.Entities.فیلد);
                                        foreach (CoreObject Fild in FildList)
                                        {
                                            try
                                            {
                                                worksheet.Range[Chars[CharCounter].ToString() + CharTemp + "1"].Text = Fild.FullName;
                                                CharCounter++;
                                                if (CharCounter == Chars.Length)
                                                {
                                                    CharTemp += Chars[CharCounter2].ToString();
                                                    CharCounter2++;
                                                    CharCounter = 0;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                break;
                                            }

                                        }
                                        TableCounter++;
                                    }

                                    workbook.SaveAs(DataSourceInfo.FilePath + "\\" + DataSourceInfo.DataBase);
                                }
                                break;
                            }
                        case CoreDefine.DataSourceType.ACCESS:
                            {
                                List<CoreObject> TableList = CoreObject.FindChilds(BaseCoreID, CoreDefine.Entities.جدول);
                                AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                foreach (CoreObject TableItem in TableList)
                                {
                                    List<CoreObject> FieldList = CoreObject.FindChilds(TableItem.CoreObjectID, CoreDefine.Entities.فیلد);
                                    string[] FieldName = new string[FieldList.Count];
                                    string[] FieldType = new string[FieldList.Count];
                                    int Counter = 0;

                                    foreach (CoreObject FieldItem in FieldList)
                                    {
                                        Field field = new Field(FieldItem);
                                        FieldName[Counter] = FieldItem.FullName;

                                        if (field.IsIdentity)
                                            FieldType[Counter] = "AUTOINCREMENT";
                                        else
                                            switch (field.FieldNature)
                                            {
                                                case "Bigint":
                                                    {
                                                        FieldType[Counter] = "long";
                                                        break;
                                                    }
                                                case "Binary(800)":
                                                    {
                                                        FieldType[Counter] = "Binary";
                                                        break;
                                                    }
                                                case "Binary(MAX)":
                                                    {
                                                        FieldType[Counter] = "Binary";
                                                        break;
                                                    }
                                                case "Float":
                                                    {
                                                        FieldType[Counter] = "Float";
                                                        break;
                                                    }
                                                case "Bit":
                                                    {
                                                        FieldType[Counter] = "Bit";
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        FieldType[Counter] = "text";
                                                        break;
                                                    }
                                            }
                                        Counter++;
                                    }

                                    DataBase.CreateTableWithoutRegitry(TableItem.FullName, FieldName, FieldType);
                                }
                                break;
                            }
                    }
                }
                else
                {
                    string Value = "";
                    long ID = 0;
                    int OrderIndex = 0;
                    switch (DataSourceInfo.DataSourceType)
                    {
                        case CoreDefine.DataSourceType.SQLSERVER:
                            {
                                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, Models.Tools.SQLDataBase.SQLVersions.SQL2008);
                                DataTable TableList = DataBase.GetAllTableName();
                                foreach (DataRow TableRow in TableList.Rows)
                                {
                                    string TABLESCHEMA = TableRow[0].ToString();
                                    string TableName = TableRow[1].ToString();

                                    Value = Tools.Tools.ToXML(new Table() { TABLESCHEMA = TABLESCHEMA });
                                    ID = Referral.DBCore.Insert("CoreObject"
                                        , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                        , new object[] { BaseCoreID, CoreDefine.Entities.جدول.ToString(), "", TableName, OrderIndex, 0, Value });

                                    Referral.CoreObjects.Add(new CoreObject(ID, BaseCoreID, CoreDefine.Entities.جدول, "", TableName, OrderIndex, false, Value));

                                    string IdentityField = DataBase.GetIdentityTable(TableName);
                                    string FieldValue = "";

                                    if (IdentityField == "")
                                    {
                                        if (DataBase.CreateIdentityField(Tools.Tools.SafeTitle(TableName)))
                                        {
                                            FieldValue = Tools.Tools.ToXML(new Field("شناسه", "شناسه", "Bigint", CoreDefine.InputTypes.Number, "عمومی", false, false, false, "", "", "", false, true, 0, false, 0, float.MaxValue, false, true));
                                            int FieldID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", "شناسه", 1, 1, FieldValue });

                                            Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", "شناسه", 1, true, FieldValue));
                                            IdentityField = "شناسه";

                                        }
                                        else
                                        {
                                            AlarmRebuilding += TableName + " \n";
                                            bool IsFindColumn = false;
                                            IdentityField = DataBase.SelectField("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 AND TABLE_NAME = '" + TableName + "' ").ToString();
                                            if (IdentityField == "")
                                            {
                                                string Query = "SELECT COLUMN_NAME  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'";
                                                object[] ColumnS = DataBase.SelectColumn(Query);
                                                foreach (object ColumnItem in ColumnS)
                                                {
                                                    if (ColumnItem.ToString().ToLower() == "id")
                                                    {
                                                        IdentityField = ColumnItem.ToString();
                                                        IsFindColumn = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                                IsFindColumn = true;

                                            if (IsFindColumn)
                                            {
                                                FieldValue = Tools.Tools.ToXML(new Field(IdentityField, IdentityField, "Bigint", CoreDefine.InputTypes.Number, "عمومی", false, false, false, "", "", "", false, true, 0, false, 0, float.MaxValue, false, true));
                                                int FieldID = Referral.DBCore.Insert("CoreObject"
                                                    , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                    , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", IdentityField, 1, 1, FieldValue });

                                                Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", IdentityField, 1, true, FieldValue));

                                            }
                                        }
                                    }
                                    else
                                    {
                                        FieldValue = Tools.Tools.ToXML(new Field(IdentityField, "شناسه", "Bigint", CoreDefine.InputTypes.Number, "عمومی", false, false, false, "", "", "", false, true, 0, false, 0, float.MaxValue, false, true));
                                        int FieldID = Referral.DBCore.Insert("CoreObject"
                                            , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                            , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", IdentityField, 1, 1, FieldValue });

                                        Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", IdentityField, 1, true, FieldValue));
                                    }

                                    DataTable ColumnData = DataBase.GetAllColumn(TableName);
                                    foreach (DataRow Row in ColumnData.Rows)
                                    {
                                        string COLUMN_NAME = Row["COLUMN_NAME"].ToString();

                                        if (COLUMN_NAME != IdentityField)
                                        {

                                            string FieldNature = "";
                                            CoreDefine.InputTypes FieldType = CoreDefine.InputTypes.LongText;
                                            bool ISRequired = Row["IS_NULLABLE"].ToString() == "YES" ? false : true;
                                            int CHARACTER_MAXIMUM_LENGTH = Row["CHARACTER_MAXIMUM_LENGTH"].ToString() == "" ? 0 : Convert.ToInt32(Row["CHARACTER_MAXIMUM_LENGTH"].ToString());

                                            switch (Row["DATA_TYPE"].ToString().ToUpper())
                                            {
                                                case "TIME":
                                                    {
                                                        FieldNature = "Nvarchar(400)";
                                                        FieldType = CoreDefine.InputTypes.Clock;
                                                        break;
                                                    }
                                                case "NVARCHAR":
                                                    {
                                                        if (CHARACTER_MAXIMUM_LENGTH == -1)
                                                            FieldNature = "Nvarchar(MAX)";
                                                        else
                                                            FieldNature = "Nvarchar(400)";
                                                        FieldType = CoreDefine.InputTypes.ShortText;
                                                        break;
                                                    }
                                                case "DATE":
                                                case "DATETIME2":
                                                case "DATETIME":
                                                    {
                                                        FieldNature = "Nvarchar(400)";
                                                        FieldType = CoreDefine.InputTypes.PersianDateTime;
                                                        break;
                                                    }
                                                case "SMALLINT":
                                                case "TINYINT":
                                                case "BIGINT":
                                                    {
                                                        FieldNature = "Bigint";
                                                        FieldType = CoreDefine.InputTypes.Number;
                                                        break;
                                                    }
                                                case "DECIMAL":
                                                case "FLOAT":
                                                    {
                                                        FieldNature = "Float";
                                                        FieldType = CoreDefine.InputTypes.Number;
                                                        break;
                                                    }
                                                case "BIT":
                                                    {
                                                        FieldNature = "Bit";
                                                        FieldType = CoreDefine.InputTypes.TwoValues;
                                                        break;
                                                    }
                                                case "BINARY":
                                                    {
                                                        if (CHARACTER_MAXIMUM_LENGTH == -1)
                                                            FieldNature = "Binary(MAX)";
                                                        else
                                                            FieldNature = "Binary(800)";
                                                        FieldType = CoreDefine.InputTypes.LongText;
                                                        break;
                                                    }
                                                case "NCHAR":
                                                case "CHAR":
                                                case "VARCHAR":
                                                    {
                                                        if (CHARACTER_MAXIMUM_LENGTH == -1)
                                                            FieldNature = "Nvarchar(MAX)";
                                                        else
                                                            FieldNature = "Nvarchar(400)";

                                                        if (CHARACTER_MAXIMUM_LENGTH < 401)
                                                            FieldType = CoreDefine.InputTypes.ShortText;
                                                        else
                                                            FieldType = CoreDefine.InputTypes.LongText;
                                                        break;
                                                    }
                                                case "TEXT":
                                                    {
                                                        FieldNature = "Nvarchar(MAX)";
                                                        FieldType = CoreDefine.InputTypes.LongText;
                                                        break;
                                                    }
                                                case "VARBINARY":
                                                    {
                                                        if (CHARACTER_MAXIMUM_LENGTH == -1)
                                                            FieldNature = "Binary(MAX)";
                                                        else
                                                            FieldNature = "Binary(800)";

                                                        FieldType = CoreDefine.InputTypes.LongText;
                                                        break;
                                                    }
                                                case "INT":
                                                    {
                                                        FieldNature = "Bigint";
                                                        FieldType = CoreDefine.InputTypes.Number;
                                                        break;
                                                    }
                                                case "XML":
                                                    {
                                                        FieldNature = "xml";
                                                        FieldType = CoreDefine.InputTypes.xml;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        break;
                                                    }
                                            }

                                            FieldValue = Tools.Tools.ToXML(new Field(COLUMN_NAME, "", FieldNature, FieldType, "عمومی", true, true, ISRequired, "", "", "", false, false, 0, false));
                                            int FieldID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", COLUMN_NAME, Convert.ToInt32(Row["ORDINAL_POSITION"].ToString()), 0, FieldValue });

                                            Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", COLUMN_NAME, Convert.ToInt32(Row["ORDINAL_POSITION"].ToString()), false, FieldValue));
                                        }
                                    }

                                }
                                AlarmRebuilding = "این جداول را بررسی بفرمایید : \n" + AlarmRebuilding;
                                DataTable FronkeyData = DataBase.GetAllFrankeyColumn();
                                foreach (DataRow row in FronkeyData.Rows)
                                {
                                    string FK_Table = row[0].ToString();
                                    string FK_Column = row[1].ToString();
                                    string PK_Table = row[2].ToString();
                                    string PK_Column = row[3].ToString();
                                    string Constraint_Name = row[4].ToString();

                                    CoreObject TableCore = CoreObject.Find(BaseCoreID, CoreDefine.Entities.جدول, FK_Table);
                                    if (TableCore.CoreObjectID > 0)
                                    {
                                        CoreObject FielCore = CoreObject.Find(TableCore.CoreObjectID, CoreDefine.Entities.فیلد, FK_Column);
                                        if (FielCore.CoreObjectID > 0)
                                        {
                                            Field field = new Field(FielCore);
                                            field.FieldType = CoreDefine.InputTypes.RelatedTable;
                                            field.RelatedTable = CoreObject.Find(BaseCoreID, CoreDefine.Entities.جدول, PK_Table).CoreObjectID;
                                            if (field.FieldNature != "Bigint")
                                                field.FieldNature = "Bigint";
                                            string FieldValue = Tools.Tools.ToXML(field);
                                            if (Referral.DBCore.UpdateRow(FielCore.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { FieldValue }))
                                            {
                                                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == FielCore.CoreObjectID);
                                                Referral.CoreObjects[CoreIndex].Value = FieldValue;
                                            }

                                        }
                                    }
                                }

                                break;
                            }
                        case CoreDefine.DataSourceType.EXCEL:
                            {
                                ExcelDatabase DataBase = new ExcelDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase);
                                DataTable SchemaTable = DataBase.GetSchemaTable();

                                foreach (DataRow Row in SchemaTable.Rows)
                                {
                                    if (!Row["TABLE_NAME"].ToString().EndsWith("_xlnm#Print_Area"))
                                    {
                                        DataTable ColumnTable = DataBase.SelectDataTable("SELECT * FROM [" + Row["TABLE_NAME"].ToString() + "]");

                                        if (ColumnTable.Rows.Count > 0)
                                        {
                                            Value = Tools.Tools.ToXML(new Table());
                                            string TableName = Row["TABLE_NAME"].ToString().Substring(0, Row["TABLE_NAME"].ToString().Length - 1);

                                            ID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { BaseCoreID, CoreDefine.Entities.جدول.ToString(), "", TableName, OrderIndex, 0, Value });

                                            Referral.CoreObjects.Add(new CoreObject(ID, BaseCoreID, CoreDefine.Entities.جدول, "", TableName, OrderIndex, false, Value));

                                            int OrderCounter = 0;
                                            foreach (DataColumn col in ColumnTable.Columns)
                                            {
                                                string FieldValue = Tools.Tools.ToXML(new Field(col.ColumnName, "", "Nvarchar(400)", CoreDefine.InputTypes.Number, "عمومی", true, true, false, "", "", "", false, true, 0, false));
                                                int FieldID = Referral.DBCore.Insert("CoreObject"
                                                    , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                    , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", col.ColumnName, OrderCounter, 0, FieldValue });

                                                Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", col.ColumnName, OrderCounter, false, FieldValue));
                                                OrderCounter++;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case CoreDefine.DataSourceType.ACCESS:
                            {
                                AccessDatabase DataBase = new AccessDatabase(DataSourceInfo.FilePath, DataSourceInfo.DataBase, DataSourceInfo.Password);
                                DataTable SchemaTable = DataBase.GetSchemaTable();

                                foreach (DataRow Row in SchemaTable.Rows)
                                {
                                    if (!Row["TABLE_NAME"].ToString().StartsWith("MSys") && !Row["TABLE_NAME"].ToString().EndsWith(" Extended") && !Row["TABLE_NAME"].ToString().EndsWith(" Count"))
                                    {
                                        DataTable ColumnTable = DataBase.SelectDataTable("SELECT * FROM [" + Row["TABLE_NAME"].ToString() + "]");

                                        if (ColumnTable.Rows.Count > 0)
                                        {
                                            Value = Tools.Tools.ToXML(new Table());
                                            string TableName = Row["TABLE_NAME"].ToString();

                                            ID = Referral.DBCore.Insert("CoreObject"
                                                , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                , new object[] { BaseCoreID, CoreDefine.Entities.جدول.ToString(), "", TableName, OrderIndex, 0, Value });

                                            Referral.CoreObjects.Add(new CoreObject(ID, BaseCoreID, CoreDefine.Entities.جدول, "", TableName, OrderIndex, false, Value));

                                            int OrderCounter = 0;
                                            foreach (DataColumn col in ColumnTable.Columns)
                                            {
                                                string FieldType = "";
                                                CoreDefine.InputTypes FieldNature = CoreDefine.InputTypes.LongText;
                                                switch (col.DataType.Name.ToString())
                                                {
                                                    case "Int32":
                                                        {
                                                            FieldType = "Bigint";
                                                            FieldNature = CoreDefine.InputTypes.Number;
                                                            break;
                                                        }
                                                    case "String":
                                                        {
                                                            FieldType = "Nvarchar(400)";
                                                            FieldNature = CoreDefine.InputTypes.ShortText;
                                                            break;
                                                        }
                                                    case "DateTime":
                                                        {
                                                            FieldType = "Nvarchar(400)";
                                                            FieldNature = CoreDefine.InputTypes.ShortText;
                                                            break;
                                                        }
                                                    case "Boolean":
                                                        {
                                                            FieldType = "Bit";
                                                            FieldNature = CoreDefine.InputTypes.TwoValues;
                                                            break;
                                                        }
                                                }

                                                string FieldValue = Tools.Tools.ToXML(new Field(col.ColumnName, "", FieldType, FieldNature, "عمومی", true, true, false, "", "", "", false, true, 0, false, 0, int.MaxValue));
                                                int FieldID = Referral.DBCore.Insert("CoreObject"
                                                    , new string[] { "ParentID", "Entity", "Folder", "FullName", "OrderIndex", "IsDefault", "Value" }
                                                    , new object[] { ID, CoreDefine.Entities.فیلد.ToString(), "عمومی", col.ColumnName, OrderCounter, 0, FieldValue });

                                                Referral.CoreObjects.Add(new CoreObject(FieldID, ID, CoreDefine.Entities.فیلد, "عمومی", col.ColumnName, OrderCounter, false, FieldValue));
                                                OrderCounter++;
                                            }
                                        }

                                    }
                                }
                                break;
                            }
                    }
                }
        }

        public static void RebuildTableFunction(long BaseCoreID, ref string Error, ref string AlarmRebuilding)
        {
            CoreObject TableFunctionCore = CoreObject.Find(BaseCoreID);
            TableFunction tableFunction = new TableFunction(TableFunctionCore);
            DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(TableFunctionCore.ParentID));
            string Query= string.Empty;
            if (int.Parse(Desktop.SelectField(dataSourceInfo, "SELECT count(1)  FROM sys.sql_modules m INNER JOIN sys.objects o ON m.object_id = o.object_id WHERE o.name = N'" + TableFunctionCore.FullName + "'").ToString()) == 0)
                Query += "CREATE";
            else
                Query += "ALTER";
            Query += " FUNCTION  dbo." + TableFunctionCore.FullName + "(";
            List<CoreObject> CoreList = CoreObject.FindChilds(TableFunctionCore.CoreObjectID);
            foreach (CoreObject coreObject in CoreList)
            {
                ParameterTableFunction parameterTableFunction = new ParameterTableFunction(coreObject);
                Query += "@" + parameterTableFunction.FullName + " " + parameterTableFunction.ParameterDataType.ToString() + ",\n";
            }
            Query=Query.Substring(0, Query.Length - 2)+"\n";
            Query +=  ")RETURNS " + tableFunction.ReturnDataType + " AS \nBEGIN \n" + tableFunction.Query + "\n END";
            Desktop.ExecuteQuery(dataSourceInfo, Query);
        }
    }
}