using APM.Models;
using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.Diagram;
using APM.Models.Security;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Xml.Serialization;

namespace APM.Controllers
{
    //[RequireHttps]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            
            if(!UserAuthorization.CanUserVisit()) return Redirect(UserAuthorization.ExclusionURL());
            string _Message = "";
            if (Referral.UserAccount.LastDateChangePassword == "")
                _Message = "کلمه عبور شما منقضی شده است لطفا کلمه عبور خود را تغییر دهید.";
            else if (CDateTime.GetPersianDaysDiffDate(Referral.UserAccount.LastDateChangePassword, CDateTime.GetNowshamsiDate()) > Referral.PublicSetting.ChangePasswordDays)
                _Message = "کلمه عبور شما منقضی شده است لطفا کلمه عبور خود را تغییر دهید.";

            if (_Message != "")
            {
                string LastDateChangePassword = Referral.DBData.SelectField("Select تاریخ_آخرین_تغییر_رمز_عبور from کاربر where شناسه = " + Referral.UserAccount.UsersID.ToString()).ToString();
                if (LastDateChangePassword != "")
                {
                    Referral.UserAccount.LastDateChangePassword = LastDateChangePassword;
                    if (CDateTime.GetPersianDaysDiffDate(Referral.UserAccount.LastDateChangePassword, CDateTime.GetNowshamsiDate()) > Referral.PublicSetting.ChangePasswordDays)
                        _Message = "کلمه عبور شما منقضی شده است لطفا کلمه عبور خود را تغییر دهید.";
                    else
                        _Message = "";
                }
            }
            ViewData["_Message"] = _Message;
            return View();
        } 

        public JsonResult ReloadCoreObject()
        {
            Software.CoreReload();
            return Json(true);
        }

        public ActionResult ShowRegistryTable(int RegistryTable,string TableName,long TableID,long RowID)
        {
            Log.LogFunction("HomeController.ShowRegistryTable");
            CoreDefine.RegistryTable registryTable = CoreDefine.RegistryTable.Delete;
            switch (RegistryTable)
            {
                case 1:registryTable = CoreDefine.RegistryTable.Insert;break;
                case 2:registryTable = CoreDefine.RegistryTable.Update;break;
                case 3:registryTable = CoreDefine.RegistryTable.Download;break;
                case 4:registryTable = CoreDefine.RegistryTable.View;break;
                case 5: registryTable = CoreDefine.RegistryTable.Login; break;
            }    
            CoreObject core = CoreObject.Find(TableID);
            long CoreId = 0;
            if(core.Entity==CoreDefine.Entities.جدول)
            {
                CoreId = core.CoreObjectID;
                TableName = core.FullName;
            }
            else if (core.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm form = new InformationEntryForm(core);
                CoreId = form.RelatedTable; 
                CoreObject Tablecore = CoreObject.Find(form.RelatedTable);
                TableName = Tablecore.FullName;
            }
            List<Folder> FolderList = new List<Folder>();
            FolderList.Add(new Folder());
            ViewData["RegistryTableFolder"] = FolderList;
            ViewData["FieldParameterList"] = TableID == 0 ? new List<TemplateField>() : Desktop.RegistryTableParameter(CoreId);

            List<Folder> FolderList2 = new List<Folder>();
            FolderList2.Add(new Folder() { FullName= "جستجو" }); 
            ViewData["SearchRegistryTableFolder"] = FolderList2;
            ViewData["SearchFieldParameterList"] = Desktop.SearchRegistryTableParameter(CoreId);
            ViewData["ParentID"] = "0";
            ViewData["DataKey"] = CoreId.ToString();

            ViewData["RegistryTable"] = registryTable;
            ViewData["TableName"] = TableName;
            ViewData["RowID"] = RowID;
            Log.LogFunction("HomeController.ShowRegistryTable",false);
            return View("~/Views/Desktop/RegistryTable.cshtml");
        }

        public JsonResult SaveShowRecordTable(long DataKey, long RecordID)
        {
            //Log.LogFunction("HomeController.SaveShowRecordTable");
            CoreObject core = CoreObject.Find(DataKey);
            if (core.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm form = new InformationEntryForm(core);
                core = CoreObject.Find(form.RelatedTable);
            }

            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(core.ParentID));
            //Referral.DBRegistry.Insert("View_APMRegistry", new string[] { "UserAccountID", "RegistryDate", "RegistryTime", "TableName", "CoreObjectID", "IP", "ServerName", "DatabaseName", "PCName", "Version", "Source", "BrowserType", "BrowserVersion", "RecordID" }, new object[] { Referral.UserAccount.UsersID, CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), core.FullName, core.CoreObjectID, Referral.UserAccount.IP, DataSourceInfo.ServerName, DataSourceInfo.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, "Web", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion , RecordID });
            //Log.LogFunction("HomeController.SaveShowRecordTable",false);
            return Json("");
        }

        public JsonResult RestoreDeletedRecord(long RecordId)
        {
            Table table = new Table();
            DataSourceInfo DataSourceInfo = new DataSourceInfo();
            CoreObject TableCore = CoreObject.Find(Convert.ToInt32(Session["RegistryTableDataKey"].ToString()));
            bool IsRestoreAttchment = false;
            bool Result = false;
            
            if (Session["RegistryTableDataKey"].ToString() == "0")
            {
                Record record =new Record(Referral.DBRegistry,"Select * from Delete_APMRegistry where DeleteRegistryID = " + RecordId.ToString());
                if (record.Field("DatabaseName").ToString().IndexOf("Attachment") >-1)
                { 
                    string ValueXml = record.Field("Value").ToString();
                    var stringReader = new System.IO.StringReader(ValueXml);
                    var serializer = new XmlSerializer(typeof(RecordData));
                    var AttachmentRecordData = serializer.Deserialize(stringReader) as RecordData;
                    string URL= AttachmentRecordData.Items.Find(x => x.Name == "URL").Value.ToString();
                    string FileName = AttachmentRecordData.Items.Find(x => x.Name == "FullName").Value.ToString() + "." + AttachmentRecordData.Items.Find(x => x.Name == "Extension").Value.ToString();
                    APM.Models.Attachment.CopyFile(new FileInfo(URL.Replace("CoreObjectAttachment", "DeleteAttachment")+"\\"+FileName), new FileInfo(URL));
                    APM.Models.Attachment.DeleteFile(URL.Replace("CoreObjectAttachment", "DeleteAttachment") + "\\" + FileName);
                    byte[] fileData = new byte[0];

                    if (AttachmentRecordData.Items.Find(x => x.Name == "Value").Value.ToString()== "True")
                    {
                        string FileText = System.IO.File.ReadAllText(URL + "\\" + FileName);
                        fileData = Convert.FromBase64String(FileText);  
                    }

                    byte[] ThumbnailImageBytes = Models.Attachment.CreateThumbnailImage(URL + "\\" + FileName);
                    Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "URL", "Value", "Thumbnail" }
                                            , new object[] { AttachmentRecordData.Items.Find(x => x.Name == "RecordID").Value ,
                                                             AttachmentRecordData.Items.Find(x => x.Name == "InnerID").Value , "",
                                                             AttachmentRecordData.Items.Find(x => x.Name == "FullName").Value,
                                                             AttachmentRecordData.Items.Find(x => x.Name == "Extension").Value,
                                                             AttachmentRecordData.Items.Find(x => x.Name == "Size").Value,
                                                             AttachmentRecordData.Items.Find(x => x.Name == "URL").Value,fileData , ThumbnailImageBytes });
                    IsRestoreAttchment = true;
                    Result=true;
                }
                else
                { 
                    List<CoreObject> DataSourceCore = CoreObject.FindChilds(0, CoreDefine.Entities.پایگاه_داده);
                    foreach (CoreObject core in DataSourceCore)
                    {
                        DataSourceInfo = new DataSourceInfo(core);
                        if (DataSourceInfo.DataBase == record.Field("DatabaseName").ToString())
                        {
                            TableCore = CoreObject.Find(core.CoreObjectID, CoreDefine.Entities.جدول, record.Field("TableName").ToString());
                            table = new Table(TableCore);
                        }
                    }
                }
            }
            else
                DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));

            if(!IsRestoreAttchment)
            {
                table = new Table(TableCore);
                DataTable dataTable = (DataTable)Session["RegistryTable"];
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["RegistryID"].ToString() == RecordId.ToString())
                    {
                        var stringReader = new System.IO.StringReader(row["Value"].ToString());
                        var serializer = new XmlSerializer(typeof(RecordData));
                        var TableInfo = serializer.Deserialize(stringReader) as RecordData;

                        string TableValues = "@P0,";

                        string FieldID = table.IDField().FieldName;
                        string Query = "SET IDENTITY_INSERT ["+ table.TABLESCHEMA+ "].[" + row["TableName"].ToString() + "] ON \n" +
                            "INSERT [" + table.TABLESCHEMA + "]." + row["TableName"].ToString() + " (" + FieldID + ",";


                        List<FieldData> FieldDataItems = TableInfo.Items;
                        string[] _ColumnName = new string[FieldDataItems.Count + 1];
                        object[] _ColumnValues = new object[FieldDataItems.Count + 1];
                        _ColumnName[0] = FieldID;
                        _ColumnValues[0] = row["RecordID"];

                        for (int Index = 0; Index < FieldDataItems.Count; Index++)
                        {
                            if (Index > 0)
                            {
                                Query += ",";
                                TableValues += ",";
                            }
                            Query += FieldDataItems[Index].Name;
                            TableValues += "@P" + (Index + 1);
                            _ColumnName[Index + 1] = FieldDataItems[Index].Name;
                        }
                        Query += ") Values(" + TableValues + ")\n  SET IDENTITY_INSERT [" + table.TABLESCHEMA + "].[" + row["TableName"].ToString() + "] OFF ";
                        SqlDataReader TheReader;

                        SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                        DataBase.Open();
                        var TheCommand = new SqlCommand(Query, DataBase.ConnectionData.DBConnection);
                        TheCommand.CommandType = CommandType.Text;
                        TheCommand.Parameters.AddWithValue("@P0", row["RecordID"].ToString());
                        for (int Index = 0; Index < FieldDataItems.Count; Index++)
                        {
                            object ParameterValue;
                            if (FieldDataItems[Index].Value != null)
                                ParameterValue = FieldDataItems[Index].Value is DBNull ? "" : FieldDataItems[Index].Value;
                            else
                                ParameterValue = "";

                            TheCommand.Parameters.AddWithValue("@P" + (Index + 1), ParameterValue);
                            _ColumnValues[Index + 1] = ParameterValue;
                        }
                        try
                        {
                            TheCommand.ExecuteReader();
                            Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_بازیافت, TableCore, long.Parse(row["RecordID"].ToString()), _ColumnName, _ColumnValues, "", TableCore.CoreObjectID.ToString());
                            DataBase.Close();
                            Referral.DBData.Execute("Delete Delete_APMRegistry Where DeleteRegistryID=" + row["RegistryID"].ToString());
                            Result = true;
                        }
                        catch (Exception ex)
                        {
                            Result = false;
                        }

                        break;
                    }
                }

            }

            return Json(Result);
        }

        public ActionResult WindowResize(int _Width, int _Height)
        {
            Referral.Browser_Width = _Width;
            Referral.Browser_Height = _Height;
            Referral.MasterPopupEditor_Width = _Width - 100;
            Referral.MasterPopupEditor_Height = _Height - 200;
            return null;
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        } 

        public ActionResult Attachment(string DataKey,string ParentID, string RecordID)
        {
            string Url = "~/Views/Attachment/Index.cshtml";
            string viewStr = "";
            CoreObject _Object =CoreObject.Find(long.Parse(DataKey)); 

            string TableID = _Object.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات? new InformationEntryForm(_Object).RelatedTable.ToString():DataKey;
            
            ViewData["DataKey"] = DataKey;
            ViewData["RecordID"] = TableID;
            ViewData["InnerID"] = RecordID;
            ViewData["ParentID"] = ParentID;

            viewStr = RenderRazorViewToString(Url, new object());
            return Content(viewStr);
        }
        
        public ActionResult InformationEntryForm(string Form="", string ParentID="0", long ProcessID=0, bool IsDetailGrid = false, bool ShowWithOutPermissionConfig=false)
        { 
            if(ProcessID>0 && ParentID!="0" && Form!="")
            {
                ViewData["DataKey"] = Form;
                ViewData["ProcessID"] = ProcessID;
                ViewData["ParentID"] = ParentID;
                return View("~/Views/Desktop/ProcessStepDetail.cshtml");
            }


            if (Referral.CoreObjects.Count == 0) 
                Software.CoreReload(); 
            if (ProcessID>0)
            {
                ProcessType processType = new ProcessType(CoreObject.Find(ProcessID));
                Form = processType.InformationEntryFormID.ToString();
                ShowWithOutPermissionConfig = true;
            }

            Desktop.StartupSetting(Form, ShowWithOutPermissionConfig);
            Session["MasterDataKey"] = Form;
            ViewData["MasterDataKey"] = Form;
            ViewData["MasterParentID"] = ParentID;
            ViewData["MasterProcessID"] = ProcessID;
            ViewData["RecordID"] = "0";
            ViewData["ProcessStepID"] = 0;
            ViewData["IsDetailGrid"] = IsDetailGrid;
             
            Session["SearchinformationEntryFormQuery" + Form + "_" + ParentID] = ""; 
            Session["SearchinformationEntryFormGridFilter" + Form + "_" + ParentID] = "";
            return View("~/Views/Desktop/InformationEntryForm.cshtml");
        }

        public ActionResult SearchFormButtonClick(string Form, string[] FormInputName, string[] FormInputValue)
        {
            string Url = "~/Views/Desktop/SearchForm.cshtml";
            string viewStr = "";
            Session["SearchFormButtonClick_FormInputName"] = FormInputName;
            Session["SearchFormButtonClick_FormInputValue"] = FormInputValue;

            Desktop.StartupSetting(Form); 
            ViewData["DataKey"] = Form; 

            viewStr = RenderRazorViewToString(Url, new object());
            return Content(viewStr);
        }

        //[OutputCache(Duration = 60 * 5, Location = OutputCacheLocation.Client, NoStore = true, VaryByParam = "*")]
        //[OutputCache(Duration = 60 * 5, VaryByParam = "*")]
        public ActionResult EditorForm(string DataKey,long ParentID,long RecordID,bool ISDetailGridForm=false,bool ISReadOnly=true,long ProcessID=0,long ProcessStepID=0)
        {   
            string InCellGrid = "";
            CoreObject Form = CoreObject.Find(long.Parse(DataKey));

            if (Desktop.DataKey_ShowWithOutPermissionConfig[DataKey])
                Session["NewEditorForm" + DataKey] = null;

            switch (Form.Entity)
            {
                case CoreDefine.Entities.جدول:
                {
                    if (Desktop.DataTableForm[DataKey] == null)
                        Desktop.StartupSetting(DataKey);
                    break;
                }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                { 
                    if (Desktop.DataInformationEntryForm[DataKey] == null)
                        Desktop.StartupSetting(DataKey);


                        foreach (InformationEntryForm EntryForm in Desktop.DataInformationEntryForm[DataKey].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                        {
                            if(EntryForm.ShowInParentForm || ProcessID>0)
                            {

                                if (Desktop.DataFields[EntryForm.CoreObjectID.ToString()] == null)
                                {
                                    Desktop.StartupSetting(EntryForm.CoreObjectID.ToString());
                                    foreach (InformationEntryForm Item in Desktop.DataInformationEntryForm[EntryForm.CoreObjectID.ToString()].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                    {
                                        if (Desktop.DataFields[Item.CoreObjectID.ToString()] == null)
                                            Desktop.StartupSetting(Item.CoreObjectID.ToString());
                                    }
                                }
                                else
                                {
                                    foreach (InformationEntryForm Item in Desktop.DataInformationEntryForm[EntryForm.CoreObjectID.ToString()].Childs(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                    {
                                        if (Desktop.DataFields[Item.CoreObjectID.ToString()] == null)
                                            Desktop.StartupSetting(Item.CoreObjectID.ToString());
                                    }
                                }

                                if (Desktop.CachedTable[EntryForm.CoreObjectID.ToString()] != null)
                                {
                                    DataTable datatable = Desktop.CachedTable[EntryForm.CoreObjectID.ToString()];
                                    datatable.Rows.Clear();
                                    Desktop.CachedTable[EntryForm.CoreObjectID.ToString()] = Desktop.CachedTable[EntryForm.CoreObjectID.ToString()];
                                }
                                if (EntryForm.GridEditMode != Kendo.Mvc.UI.GridEditMode.PopUp)
                                    InCellGrid += "MainGrid" + EntryForm.CoreObjectID + ",";

                                if(Desktop.SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()]!=null)
                                { 
                                    DataTable Table = (DataTable)Desktop.SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()];
                                    Table.Rows.Clear();
                                    Desktop.SessionEditorGrid[EntryForm.CoreObjectID.ToString(), ParentID.ToString()] = Table;
                                } 
                                APM.Models.Attachment.DeleteDirectory(APM.Models.Attachment.MapTemporaryFilePath + EntryForm.CoreObjectID.ToString());
                            }

                        } 
                        APM.Models.Attachment.DeleteDirectory(APM.Models.Attachment.MapTemporaryFilePath + Form.CoreObjectID.ToString());
                        break;
                }
            }

            if (RecordID == 0 && ParentID == 0 && Session["NewEditorForm" + DataKey] != null && !ISDetailGridForm)
                return Content(Session["NewEditorForm" + DataKey].ToString());


            string Url = "~/Views/Shared/InputForm/EditorForm.cshtml";
            string viewStr = "";

            ViewData["ProcessID"] = ProcessID;
            ViewData["ProcessStepID"] = ProcessStepID;
            ViewData["DataKey"] = DataKey;
            ViewData["ParentID"] = ParentID.ToString();
            ViewData["RecordID"] = RecordID.ToString();
            ViewData["FolderDataKey"] = Desktop.ToFolderNames(DataKey);
            ViewData["ISDetailGridForm"] = ISDetailGridForm;
            ViewData["ISReadOnly"] = ISReadOnly;
            ViewData["InCellGrid"] = InCellGrid;
            if (RecordID == 0)
            {
                ViewData["FieldParameterList"] = Desktop.TempeletFieldEntryForm(DataKey, ParentID, RecordID,false, ISReadOnly);
                Desktop.BeforLoadEditorform(DataKey, ParentID);
            }
            else
            {
                CoreObject EntryFormCore = CoreObject.Find(long.Parse(DataKey));
                if(EntryFormCore.ParentID != 0 && ParentID==0 && Desktop.SessionEditorGrid[DataKey, ParentID.ToString()]!=null) 
                    ViewData["FieldParameterList"] = Desktop.TempeletFieldEntryForm(DataKey, ParentID, RecordID,true); 
                else
                   ViewData["FieldParameterList"] = Desktop.TempeletFieldEntryForm(DataKey, ParentID, RecordID,false, ISReadOnly);
            }
            viewStr = RenderRazorViewToString(Url, new object());

            if (RecordID == 0 && !ISDetailGridForm)
                Session["NewEditorForm" + DataKey] = viewStr;
            return Content(viewStr);
            //return View("~/Views/Shared/InputForm/EditorForm.cshtml");
        }
 
        public JsonResult ProcessReferral(long ProcessID, long StepID,long _RecordID, bool IsReadonly)
        {

            CoreObject NewStepCore = CoreObject.Find(StepID);
            ProcessStep NewStep = new ProcessStep(NewStepCore);
            BpmnLane bpmnLane = new BpmnLane(CoreObject.Find(NewStepCore.ParentID));
            bool IsFindUser=false; 

            string Query = "Select کاربر.شناسه   From کاربر left join  نقش_کاربر on کاربر.نقش_کاربر = نقش_کاربر.شناسه  where 1<>1 ";

            foreach (string Item in bpmnLane.Personnel)
                if (!string.IsNullOrEmpty(Item))
                {
                    string[] Temp = Item.Split('[');
                    long UserID = long.Parse(Temp[Temp.Length - 1].Replace("]", ""));
                    if (UserID < 1)
                    {
                        switch (UserID)
                        {
                            case -1: { Query += "OR کاربر.شناسه in (select [مراحل_فرآیند].[اجرا_کننده] from [مراحل_فرآیند] where [مراحل_فرآیند].[سطر] = " + _RecordID + " AND [مراحل_فرآیند].[فرآیند] = " + ProcessID + "  AND  ([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت])=(select MIN(A.[تاریخ_ثبت]+A.[ساعت_ثبت]) from [مراحل_فرآیند] AS A where A.[سطر] = " + _RecordID + " AND A.[فرآیند] = " + ProcessID + "  ) )"; break; }
                            case 0: { Query += "OR  1=1"; break; }
                        }
                    }
                    else
                        Query += "OR  کاربر.شناسه = " + Temp[Temp.Length - 1].Replace("]", "") + " \n";
                }


            foreach (string Item in bpmnLane.OrganizationLevel)
                if (!string.IsNullOrEmpty(Item))
                {
                    string[] Temp = Item.Split('[');
                    long RoleID = long.Parse(Temp[Temp.Length - 1].Replace("]", ""));
                    if (RoleID < 1)
                    {
                        switch (RoleID)
                        {
                            case -1:{  
                                    Query += "\nOR کاربر.سمت_سازمانی in (SELECT سمت_سازمانی_دریافت_کننده   FROM ارجاع_مراحل_فرآیند  where فرآیند = " + ProcessID + "   and رکورد = "+ _RecordID + "  and مرحله_فرآیند= (  SELECT TOP 1 مرحله_فرآیند  FROM  مراحل_فرآیند  where مراحل_فرآیند.فرآیند = "+ ProcessID + "   and مراحل_فرآیند.سطر = "+ _RecordID + " And مراحل_فرآیند.مرحله_بعد_فرآیند ="+StepID+ "  order by مراحل_فرآیند.شناسه desc)  group by سمت_سازمانی_دریافت_کننده)\n";
                                    break;
                                }
                            case 0: { Query += "OR  1=1"; break; }
                        }
                    }
                    else
                        Query += "OR  کاربر.سمت_سازمانی = " + RoleID.ToString() + " \n";

                }

            string UpdateReffral = "Update ارجاع_مراحل_فرآیند set مشاهده_شده = 1 , تاریخ_مشاهده =N'" + CDateTime.GetNowshamsiDate() + "',ساعت_مشاهده =N'" + CDateTime.GetNowTime() + "' where دریافت_کننده = " + Referral.UserAccount.UsersID.ToString() + " and isnull(تاریخ_مشاهده,N'') =N'' and  فرآیند = " + ProcessID + "   and رکورد = " + _RecordID;
            DataTable UserData = Referral.DBData.SelectDataTable(Query);
            foreach (DataRow Row in UserData.Rows)
                if(Row[0].ToString()==Referral.UserAccount.UsersID.ToString())
                {
                    IsFindUser=true;
                    Referral.DBData.Execute(UpdateReffral);
                    break;
                } 

            if (!string.IsNullOrEmpty(bpmnLane.Query) && !IsFindUser)
            {
                string[] ColumnNames = new string[0];
                object[] _Values = new object[0];
                InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(NewStep.InformationEntryFormID));
                CoreObject TableCore =CoreObject.Find(informationEntryForm.RelatedTable);
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCore.ParentID));
                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                string DeclareQuery = DataBase.DefineVariablesQuery(TableCore.FullName, _RecordID, ref ColumnNames, ref _Values);

                UserData = Referral.DBData.SelectDataTable(DeclareQuery + "\n" + Tools.CheckQuery(bpmnLane.Query)); 
                foreach (DataRow Row in UserData.Rows) 
                    if (Row[0].ToString() == Referral.UserAccount.UsersID.ToString())
                    {
                        IsFindUser = true;
                        Referral.DBData.Execute(UpdateReffral);
                        break;
                    } 
            }
 
            return Json(new { HasPermision = IsFindUser, InformationFormID= NewStep.InformationEntryFormID, ParentID=0, IsReadOnly= (NewStep.RecordType == Tools.GetStepRecordType("ویرایش") ? false : (_RecordID>0?false: true)), Title=NewStep.Name });
        }

        public ActionResult SysSetting()
        {
            Software.CoreReload();
            string Url = "~/Views/SysSetting/Index.cshtml";
            string viewStr = RenderRazorViewToString(Url, new object());
            return Content(viewStr);
        }


        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult GetShamsiDate()
        {
            return Json(CDateTime.GetNowshamsiDate());
        }
        public JsonResult GetMiladiDate()
        {
            return Json(CDateTime.GetNowMiladyDate());
        } 

        public ActionResult NotificationMainDiv()
        {
            return View("~/Views/Home/NotificationMainDiv.cshtml");
        }
        public ActionResult NotificationHeaderDiv()
        {
            return View("~/Views/Home/NotificationHeaderDiv.cshtml");
        }

        public JsonResult ViewReferral(string ReferralID)
        {
            Referral.UserAccount.ReferralCount--;

            return Json( Referral.DBData.Execute("Update ارجاع_مراحل_فرآیند set مشاهده_شده = 1 , تاریخ_مشاهده =N'"+CDateTime.GetNowshamsiDate()+ "',ساعت_مشاهده =N'"+CDateTime.GetNowTime()+"' where شناسه = "+ ReferralID));
        }

        public JsonResult GetUserNotification()
        {
            return Json(Referral.UserAccount.GetUserNotification());
        } 

    }
}