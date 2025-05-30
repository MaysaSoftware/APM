using Ionic.Zip;
using Kendo.Mvc.UI;
using APM.Models;
using APM.Models.Tools;
using APM.Models.APMObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using APM.Models.Database;
using System.Drawing;
using System.Text;
using APM.Models.Security;
using System.Data;
using Aspose.Pdf.Devices;
using Aspose.Pdf;
using IronOcr;

namespace APM.Controllers
{  
    public class AttachmentController : Controller
    {
        // GET: Attachment
        public ActionResult Index(string RecordID, string InnerID)
        {
            ViewData["AttachmentAction"] = "{_RecordID:" + RecordID + ",_InnerID:" + InnerID + "}";
            //ViewData["MasterDataKey"] = Form; 
            //ViewData["MasterDataKeyPermission"] = new PermissionInformationEntryForm(long.Parse(Form), Referral.UserAccount.Permition);
            return View();
        }

        private readonly FileContentBrowser directoryBrowser;
        //
        // GET: /FileManager/
        private const string contentFolderRoot = "~/Content/";
        private const string prettyName = "Folders/";
        private static readonly string[] foldersToCopy = new[] { "~/Content/shared/filemanager" };

        public AttachmentController()
        {
            directoryBrowser = new FileContentBrowser();
        }


        public string ContentPath
        {
            get
            {
                return CreateUserFolder(null);
            }
        }

        public string Filter
        {
            get
            {
                return "*.*";
            }
        }

        public static string MapFilePath { get; set; }

        public string DeleteAttachmentFolder = "DeleteAttachment";
        public string ZipAttachmentFolder = "ZipAttachment";
        //public string Tempelet
        public string MapFileSavingPath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return Models.Attachment.MapFileSavingAttachmentPath + MapFilePath;
                else
                    return Models.Attachment.MapFileSavingAttachmentPath + MapFilePath;
            }
        }

        public string MapTemporaryFilePath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return Models.Attachment.MapTemporaryFilePath + MapFilePath;
                else
                    return Models.Attachment.MapTemporaryFilePath + MapFilePath;
            }
        }

        public string MapFileDeletingPath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return System.Web.HttpContext.Current.Server.MapPath("~/Content/DeleteAttachmentFolder" + "/" + DeleteAttachmentFolder + "/" + MapFilePath);
                else
                    return Referral.PublicSetting.FileSavingPath + "/" + DeleteAttachmentFolder + "/" + MapFilePath;
            }
        }
        public string MapFileZipPath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return System.Web.HttpContext.Current.Server.MapPath("~/Content/ZipAttachmentFolder" + "/" + ZipAttachmentFolder + "/" + MapFilePath + "/" + Referral.UserAccount.UsersID);
                else
                    return Referral.PublicSetting.FileSavingPath + "/" + ZipAttachmentFolder + "/" + MapFilePath + "/" + Referral.UserAccount.UsersID;
            }
        }

        public static long RecordID { get; set; }
        public static long InnerID { get; set; }
        public static bool IsTemporary { get; set; }


        public JsonResult DeleteTableAttachement(long _RecordID, long _InnerID, long _DataKey, long _ParentID, long FileCoreObjectID)
        {
            ConfigPath(_RecordID, _InnerID, _DataKey.ToString(), _ParentID.ToString());
            string FileName = Referral.DBCore.SelectField("Select FullName From CoreObject where CoreobjectID=" + FileCoreObjectID).ToString();
            string path = NormalizePath("");
            long AttachmentID = long.Parse(Referral.DBAttachment.SelectField("select ISNULL((SELECT CoreObjectAttachmentID FROM CoreObjectAttachment  where FullName=N'" + FileName + "' And URL = N'" + path.Replace("/", "\\").Replace(@"\\", "\\") + "'),0)").ToString());
            if (AttachmentID > 0)
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo FileItem in dir.GetFiles())
                {
                    if (FileItem.Name.Replace(FileItem.Extension, "") == FileName)
                    {
                        System.IO.File.Delete(FileItem.FullName);
                        Referral.DBAttachment.Delete("CoreObjectAttachment", AttachmentID);

                    }
                }
            }
            return Json(Field.FormatImage(Referral.PublicSetting.AppLogo));
        }

        public JsonResult SaveCamera(string ImageData, long _RecordID, long _InnerID, long _DataKey, long _ParentID, long FileCoreObjectID)
        {
            string[] ImageInfo = ImageData.Split(',');
            string Data = ImageInfo[1];
            string Extension = ImageInfo[0].Substring(ImageInfo[0].IndexOf('/') + 1, ImageInfo[0].IndexOf(';') - ImageInfo[0].IndexOf('/') - 1);
            Extension = Extension.ToLower() == "octet-stream" ? "png" : Extension;
            bool Result = false;
            byte[] imageBytes = Convert.FromBase64String(Data);
            ConfigPath(_RecordID, _InnerID, _DataKey.ToString(), _ParentID.ToString());
            CoreObject CoreAtt = CoreObject.Find(FileCoreObjectID);
            TableAttachment tableAttachment = new TableAttachment(CoreAtt);

            string path = NormalizePath("");

            DirectoryInfo dir = new DirectoryInfo(path);
            if (!Directory.Exists(dir.FullName))
                Directory.CreateDirectory(dir.FullName);

            foreach (FileInfo FileItem in dir.GetFiles())
            {
                if (FileItem.Name.Replace(FileItem.Extension, "") == CoreAtt.FullName)
                {
                    System.IO.File.Delete(FileItem.FullName);
                }
            }
            System.IO.File.WriteAllText(path + "/" + CoreAtt.FullName + "." + Extension, Data);

            if (!IsTemporary)
            {
                byte[] ThumbnailImageBytes = APM.Models.Attachment.CreateThumbnailImage(path + "/" + CoreAtt.FullName + "." + Extension);
                DataTable AttData = Referral.DBAttachment.SelectDataTable("SELECT CoreObjectAttachmentID FROM CoreObjectAttachment  where FullName=N'" + CoreAtt.FullName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + (IsTemporary ? " And Folder=" + Referral.UserAccount.UsersID.ToString() : ""));
                if (AttData.Rows.Count > 0)
                {
                    if (AttData.Rows.Count > 1)
                    {
                        Referral.DBAttachment.Execute("Delete FROM CoreObjectAttachment  where FullName=N'" + CoreAtt.FullName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + " And CoreObjectAttachmentID!=" + AttData.Rows[0][0]);
                    }

                    if (tableAttachment.SaveInDatabase)
                        Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Value", "Size", "Extension", "Thumbnail" }, new object[] { imageBytes, imageBytes.Length, Extension, ThumbnailImageBytes });
                    else
                        Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Thumbnail", "Size", "Extension" }, new object[] { ThumbnailImageBytes, imageBytes.Length, Extension });
                }
                else
                {
                    long Id = 0;
                    if (tableAttachment.SaveInDatabase)
                        Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL", "Value" }
                                                                        , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), CoreAtt.FullName, Extension, imageBytes.Length, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\"), imageBytes });
                    else
                        Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL" }
                                                                        , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), CoreAtt.FullName, Extension, imageBytes.Length, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\") });
                    Result = Id > 0 ? true : false;

                }
            }
            else
                Result = true;

            return Json(Result);
        }
        public JsonResult SaveScanerFile(string ImageData, long _InnerID, long _DataKey, long _ParentID, string _FileName)
        {
            string[] ImageInfo = ImageData.Split(',');
            string Data = ImageInfo[1];
            string Extension = ImageInfo[0].Substring(ImageInfo[0].IndexOf('/') + 1, ImageInfo[0].IndexOf(';') - ImageInfo[0].IndexOf('/') - 1);
            Extension = Extension.ToLower() == "octet-stream" ? "png" : Extension;
            bool Result = false;
            byte[] imageBytes = Convert.FromBase64String(Data);
            long _RecordID = _DataKey;
            CoreObject InformationEntryFormcoreObject = CoreObject.Find(_DataKey);
            if (InformationEntryFormcoreObject.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm informationEntryForm = new InformationEntryForm(InformationEntryFormcoreObject);
                _RecordID = informationEntryForm.RelatedTable;
            }
            ConfigPath(_RecordID, _InnerID, _DataKey.ToString(), _ParentID.ToString());

            string path = NormalizePath("");

            DirectoryInfo dir = new DirectoryInfo(path);
            if (!Directory.Exists(dir.FullName))
                Directory.CreateDirectory(dir.FullName);

            if (System.IO.File.Exists(path + "/" + _FileName + "." + Extension))
                _FileName += CDateTime.GetNowshamsiDate().Replace("/", "") + CDateTime.GetNowTime().Replace(":", "");

            System.IO.File.WriteAllText(path + "/" + _FileName + "." + Extension, Data);

            byte[] ThumbnailImageBytes = APM.Models.Attachment.CreateThumbnailImage(path + "/" + _FileName + "." + Extension);
            DataTable AttData = Referral.DBAttachment.SelectDataTable("SELECT CoreObjectAttachmentID FROM CoreObjectAttachment  where FullName=N'" + _FileName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + (IsTemporary ? " And Folder=" + Referral.UserAccount.UsersID.ToString() : ""));
            if (AttData.Rows.Count > 0)
            {
                if (AttData.Rows.Count > 1)
                {
                    Referral.DBAttachment.Execute("Delete FROM CoreObjectAttachment  where FullName=N'" + _FileName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + " And CoreObjectAttachmentID!=" + AttData.Rows[0][0]);
                }
                Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Thumbnail", "Size", "Extension" }, new object[] { ThumbnailImageBytes, imageBytes.Length, Extension });
            }
            else
            {
                long Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL" }
                                                                    , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), _FileName, Extension, imageBytes.Length, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\") });
                Result = Id > 0 ? true : false;

            }

            return Json(Result);
        }

        public ActionResult Templates_Save(long _RecordID, long _InnerID, string _DataKey, string _ParentID, string FileName, IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;
            if (files != null)
            {
                var file = files.ElementAt(0);
                ConfigPath(_RecordID, _InnerID, _DataKey, _ParentID);
                FileManagerEntry newEntry;
                string path = NormalizePath("");
                var fileName = Path.GetFileName(file.FileName);

                CoreObject AttCore = CoreObject.Find(long.Parse(_DataKey));
                TableAttachment tableAttachment = new TableAttachment();

                if (AttCore.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                {
                    InformationEntryForm informationEntryForm = new InformationEntryForm(AttCore);
                    List<CoreObject> TableAtt = CoreObject.FindChilds(informationEntryForm.RelatedTable, CoreDefine.Entities.ضمیمه_جدول, FileName);
                    if (TableAtt.Count > 0)
                    {
                        tableAttachment = new TableAttachment(TableAtt[0]);
                    }
                }
                else if (AttCore.Entity == CoreDefine.Entities.جدول)
                {
                    List<CoreObject> TableAtt = CoreObject.FindChilds(AttCore.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول, FileName);
                    if (TableAtt.Count > 0)
                    {
                        tableAttachment = new TableAttachment(TableAtt[0]);
                    }
                }

                if (AuthorizeUpload(path, file))
                {

                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (FileInfo FileItem in dir.GetFiles())
                    {
                        if (FileItem.Name.Replace(FileItem.Extension, "") == FileName)
                        {
                            System.IO.File.Delete(FileItem.FullName);
                        }
                    }
                    FileInfo file1 = new FileInfo(Path.Combine(path, fileName));
                    string FullName = FileName + file1.Extension;

                    var binaryReader = new BinaryReader(file.InputStream);
                    byte[] fileData = binaryReader.ReadBytes(file.ContentLength);
                    string Text = Convert.ToBase64String(fileData);
                    System.IO.File.WriteAllText(path + "/" + FullName, Text);

                    newEntry = directoryBrowser.GetFile(Path.Combine(path, FullName));

                    //if (!IsTemporary)
                    //{
                    byte[] ThumbnailImageBytes = Models.Attachment.CreateThumbnailImage(path + "/" + FullName);

                    DataTable AttData = Referral.DBAttachment.SelectDataTable("SELECT CoreObjectAttachmentID FROM CoreObjectAttachment  where FullName=N'" + FileName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + (IsTemporary ? " And Folder=" + Referral.UserAccount.UsersID.ToString() : ""));

                    if (AttData.Rows.Count > 0)
                    {
                        if (AttData.Rows.Count > 1)
                        {
                            Referral.DBAttachment.Execute("Delete FROM CoreObjectAttachment  where FullName=N'" + FileName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + " And CoreObjectAttachmentID!=" + AttData.Rows[0][0]);
                        }

                        if (tableAttachment.SaveInDatabase && (file1.Extension.ToLower() == ".jpg" || file1.Extension.ToLower() == ".img" || file1.Extension.ToLower() == ".png" || file1.Extension.ToLower() == ".jpeg"))
                            Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Value", "Size", "Extension", "Thumbnail" }, new object[] { fileData, file.ContentLength, newEntry.Extension.Replace(".", ""), ThumbnailImageBytes });
                        else
                            Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Thumbnail", "Size", "Extension" }, new object[] { ThumbnailImageBytes, file.ContentLength, newEntry.Extension.Replace(".", "") });
                    }
                    else
                    {
                        long Id = 0;
                        if (tableAttachment.SaveInDatabase && (file1.Extension.ToLower() == ".jpg" || file1.Extension.ToLower() == ".img" || file1.Extension.ToLower() == ".png" || file1.Extension.ToLower() == ".jpeg"))
                            Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL", "Value" }
                                                                         , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), FileName, newEntry.Extension.Replace(".", ""), file.ContentLength, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\"), fileData });
                        else
                            Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL" }
                                                                         , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), FileName, newEntry.Extension.Replace(".", ""), file.ContentLength, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\") });
                        Result = Id > 0 ? true : false;

                    }
                    //}
                    //else
                    //    Result = true;
                }
            }
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadPublickKey(string FileName, IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;
            if (files != null)
            {
                var file = files.ElementAt(0);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Attachment" + "/PublickKey/" + Referral.UserAccount.UsersID.ToString() + "/");

                if (AuthorizeUpload(path, file))
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (FileInfo FileItem in dir.GetFiles())
                    {
                        if (FileItem.Name.Replace(FileItem.Extension, "") == FileName)
                        {
                            System.IO.File.Delete(FileItem.FullName);
                        }
                    }
                    FileInfo file1 = new FileInfo(Path.Combine(path, file.FileName));
                    string FullName = FileName + file1.Extension;

                    var binaryReader = new BinaryReader(file.InputStream);
                    byte[] fileData = binaryReader.ReadBytes(file.ContentLength);
                    string Text = Convert.ToBase64String(fileData);
                    System.IO.File.WriteAllText(path + "/" + file.FileName, Text);
                    Result = true;
                }
            }
            else
                Result = true;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadOCRFile(string FileName, IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;
            if (files != null)
            {
                var file = files.ElementAt(0);
                var binaryReader = new BinaryReader(file.InputStream);
                byte[] fileData = binaryReader.ReadBytes(file.ContentLength);
                string FinalMessage = string.Empty;
                var ocr = new IronTesseract();
                OcrInput input = new OcrInput();

                ocr.Language = OcrLanguage.Persian;
                //ocr.AddSecondaryLanguage(OcrLanguage.English); 

                if (file.ContentType == "image/jpeg" || file.ContentType == "image/jpg" || file.ContentType == "image/png")
                {
                    System.Drawing.Image image;
                    try
                    {
                        MemoryStream ms = new MemoryStream(fileData, 0, fileData.Length);
                        ms.Write(fileData, 0, fileData.Length);
                        image = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
                        Bitmap bmpImage = new Bitmap(image);
                        int Split = 5;
                        for (int i = 0; i < Split; i++)
                        {
                            System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(0,
                                                        i * (image.Height / Split),
                                                        image.Width,
                                                        image.Height / Split);



                            bmpImage.Clone(cropArea, bmpImage.PixelFormat);
                            System.Drawing.Image Image2 = (System.Drawing.Image)bmpImage;
                            ImageConverter converter = new ImageConverter();
                            byte[] imageByte = (byte[])converter.ConvertTo(Image2, typeof(byte[]));
                            input = new OcrInput();
                            //input.LoadImage(imageByte);
                            var result = ocr.Read(input);
                            FinalMessage += result.Text;
                        }
                    }
                    catch { }
                }
                else if (file.ContentType == "application/pdf")
                {
                    //input.LoadPdf(fileData);
                    var result = ocr.Read(input);
                    FinalMessage += result.Text;
                }

                return Content(FinalMessage);
            }
            Result = true;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFileScriptCore(string FileName, IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;
            if (files != null)
            {
                var file = files.ElementAt(0);
                var binaryReader = new BinaryReader(file.InputStream);
                byte[] fileData = binaryReader.ReadBytes(file.ContentLength);
                var str = System.Text.Encoding.UTF8.GetString(fileData);
                return Content(str);
            }
            Result = true;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }
        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadTableAttachment(string FileName, IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;

            if (files != null && Session["SaveCoreObjectTreeView"]!=null && Session["TableAttachamentCoreObject"] != null)
            {
                string[] CoreObjectTreeViewArr = Session["SaveCoreObjectTreeView"].ToString().Split(',');
                CoreObject AttachemntCore = CoreObject.Find((long)Session["TableAttachamentCoreObject"]);
                TableAttachment tableAttachment = new TableAttachment(AttachemntCore);
                CoreObject TableCoreObject = CoreObject.Find(AttachemntCore.ParentID);
                APM.Models.APMObject.Table table = new Models.APMObject.Table(TableCoreObject);
                DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(TableCoreObject.ParentID));

                string Query = "SELECT "+table.IDField().FieldName+" From "+table.FullName+" Where ";
                foreach(string Item in CoreObjectTreeViewArr)
                {
                    Query += "Cast("+CoreObject.Find(long.Parse(Item)).FullName+" AS Nvarchar(400) ) +";
                }
                Query=Query.Substring(0,Query.Length-1);

                var file = files.ElementAt(0);
                Query += "=N'" + file.FileName.Substring(0, file.FileName.LastIndexOf(".")) + "'";
                SQLDataBase DataBase = new SQLDataBase(DataSourceInfo.ServerName, DataSourceInfo.DataBase, DataSourceInfo.Password, DataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                DataTable Data= DataBase.SelectDataTable(Query);
                if(Data != null)
                foreach(DataRow Row in Data.Rows)
                {
                    try
                    { 
                        ConfigPath(TableCoreObject.CoreObjectID, (long)Row[0], TableCoreObject.CoreObjectID.ToString(), "0");
                        FileManagerEntry newEntry;
                        string path = NormalizePath("");
                        var fileName = AttachemntCore.FullName+ file.FileName.Substring(file.FileName.LastIndexOf("."), file.FileName.Length- file.FileName.LastIndexOf("."));

                        if (AuthorizeUpload(path, file))
                        {
                            DirectoryInfo dir = new DirectoryInfo(path);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            foreach (FileInfo FileItem in dir.GetFiles())
                            {
                                if (FileItem.Name == fileName)
                                {
                                    System.IO.File.Delete(FileItem.FullName);
                                }
                            }
                            var binaryReader = new BinaryReader(file.InputStream);
                            byte[] fileData = binaryReader.ReadBytes(file.ContentLength);
                            string Text = Convert.ToBase64String(fileData);
                            System.IO.File.WriteAllText(path + "/" + fileName, Text);

                            newEntry = directoryBrowser.GetFile(Path.Combine(path, fileName));
                            byte[] ThumbnailImageBytes = Models.Attachment.CreateThumbnailImage(newEntry.Path);

                            if (!IsTemporary)
                            {
                                long _RecordID = TableCoreObject.CoreObjectID;
                                long _InnerID = (long)Row[0];
                                string Extension = file.FileName.Substring(file.FileName.LastIndexOf("."), file.FileName.Length - file.FileName.LastIndexOf("."));

                                DataTable AttData = Referral.DBAttachment.SelectDataTable("SELECT CoreObjectAttachmentID FROM CoreObjectAttachment  where FullName=N'" + AttachemntCore.FullName + "' And  RecordID=" + TableCoreObject.CoreObjectID.ToString() + " And InnerID=" + Row[0].ToString() + (IsTemporary ? " And Folder=" + Referral.UserAccount.UsersID.ToString() : ""));

                                if (AttData.Rows.Count > 0)
                                {
                                    if (AttData.Rows.Count > 1)
                                    {
                                        Referral.DBAttachment.Execute("Delete FROM CoreObjectAttachment  where FullName=N'" + AttachemntCore.FullName + "' And  RecordID=" + _RecordID + " And InnerID=" + _InnerID + " And CoreObjectAttachmentID!=" + AttData.Rows[0][0]);
                                    }

                                    if (tableAttachment.SaveInDatabase && (Extension.ToLower() == ".jpg" || Extension.ToLower() == ".img" || Extension.ToLower() == ".png" || Extension.ToLower() == ".jpeg"))
                                        Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Value", "Size", "Extension", "Thumbnail" }, new object[] { fileData, file.ContentLength, newEntry.Extension.Replace(".", ""), ThumbnailImageBytes });
                                    else
                                        Result = Referral.DBAttachment.UpdateRow(long.Parse(AttData.Rows[0][0].ToString()), 0, "CoreObjectAttachment", new string[] { "Thumbnail", "Size", "Extension" }, new object[] { ThumbnailImageBytes, file.ContentLength, newEntry.Extension.Replace(".", "") });
                                }
                                else
                                {
                                    long Id = 0;
                                    if (tableAttachment.SaveInDatabase && (Extension.ToLower() == ".jpg" || Extension.ToLower() == ".img" || Extension.ToLower() == ".png" || Extension.ToLower() == ".jpeg"))
                                        Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL", "Value" }
                                                                                        , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), AttachemntCore.FullName, newEntry.Extension.Replace(".", ""), file.ContentLength, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\"), fileData });
                                    else
                                        Id = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL" }
                                                                                        , new object[] { (IsTemporary ? _RecordID : RecordID), InnerID, (IsTemporary ? Referral.UserAccount.UsersID.ToString() : ""), AttachemntCore.FullName, newEntry.Extension.Replace(".", ""), file.ContentLength, ThumbnailImageBytes, path.Replace("/", "\\").Replace(@"\\", "\\") });
                                    Result = Id > 0 ? true : false;

                                }
                            } 
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                }

                Result = true;
            }
            else
                Result = true;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }



        private string CreateUserFolder(string _Path)
        {
            var path = string.IsNullOrEmpty(_Path) ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) : _Path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //foreach (var sourceFolder in foldersToCopy)
                //{
                //    CopyFolder(Server.MapPath(sourceFolder), path);
                //}
            }
            return path;
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(file));
                System.IO.File.Copy(file, dest);
            }

            foreach (var folder in Directory.EnumerateDirectories(source))
            {
                var dest = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }

        public bool Authorize(string path)
        {
            return CanAccess(path);
        }

        public bool CanAccess(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (IsTemporary)
                    return path.StartsWith(MapTemporaryFilePath, StringComparison.OrdinalIgnoreCase);
                else
                    return path.StartsWith(MapFileSavingPath, StringComparison.OrdinalIgnoreCase);
            }
            else
                return path.StartsWith(path, StringComparison.OrdinalIgnoreCase);
        }

        private string ToAbsolute(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }

        private string CombinePaths(string basePath, string relativePath)
        {
            return VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(basePath), relativePath);
        }


        public FileManagerEntry VirtualizePath(FileManagerEntry entry)
        {
            entry.Path = entry.Path.Replace(ContentPath, "").Replace(@"\", "/");
            return entry;
        }

        public ActionResult Create(string target, long _RecordID, long _InnerID, string _DataKey, string _ParentID, FileManagerEntry entry)
        {
            ConfigPath(_RecordID, _InnerID, _DataKey, _ParentID);
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(target)))
            {
                throw new HttpException(403, "Forbidden");
            }

            if (String.IsNullOrEmpty(entry.Path))
            {
                newEntry = CreateNewFolder(target, entry);
            }
            else
            {
                newEntry = CopyEntry(target, entry);
            }

            return Json(VirtualizePath(newEntry));
        }

        public FileManagerEntry CopyEntry(string target, FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);

            string[] FindPath = path.Split('/');
            var physicalPath = FindPath.Length == 2 ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) : path;
            var physicalTarget = EnsureUniqueName(NormalizePath(target), entry);

            FileManagerEntry newEntry;
            CreateUserFolder(target);
            if (entry.IsDirectory)
            {
                CopyDirectory(new DirectoryInfo(physicalPath), Directory.CreateDirectory(physicalTarget));
                newEntry = directoryBrowser.GetDirectory(physicalTarget);

                if (!IsTemporary)
                {
                    string OldFileName = entry.Path.Replace(MapFileSavingPath.Replace("\\", "") + "/", "");
                    string NewURL = entry.Path.Replace(OldFileName, entry.Name);
                    Referral.DBAttachment.Execute("update CoreObjectAttachment set URL=REPLACE(URL,N'" + entry.Path + "',N'" + NewURL + "') where URL like N'%" + entry.Path + "%'");
                }

            }
            else
            {
                System.IO.File.Copy(physicalPath, physicalTarget);
                newEntry = directoryBrowser.GetFile(physicalTarget);

                if (!IsTemporary)
                {
                    string OldFileName = entry.Path.Replace(MapFileSavingPath.Replace("\\", "") + "/", "");
                    string NewURL = entry.Path.Replace(OldFileName, entry.Name);
                    Referral.DBAttachment.Execute("update CoreObjectAttachment set URL=REPLACE(URL,N'" + entry.Path + "',N'" + NewURL + "') where URL like N'%" + entry.Path + "%'");
                }

            }

            return newEntry;
        }

        public void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

        public FileManagerEntry CreateNewFolder(string target, FileManagerEntry entry)
        {
            FileManagerEntry newEntry;
            var path = NormalizePath(target);
            string physicalPath = EnsureUniqueName(path, entry);

            Directory.CreateDirectory(physicalPath);

            newEntry = directoryBrowser.GetDirectory(physicalPath);

            return newEntry;
        }

        public string EnsureUniqueName(string target, FileManagerEntry entry)
        {
            var tempName = entry.Name + entry.Extension;
            int sequence = 0;
            var physicalTarget = NormalizePath(Path.Combine(target, tempName));

            if (!Authorize(physicalTarget))
            {
                throw new HttpException(403, "Forbidden");
            }

            if (entry.IsDirectory)
            {
                while (Directory.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence);
                    physicalTarget = Path.Combine(target, tempName);
                }
            }
            else
            {
                while (System.IO.File.Exists(physicalTarget))
                {
                    tempName = entry.Name + String.Format("({0})", ++sequence) + entry.Extension;
                    physicalTarget = Path.Combine(target, tempName);
                }
            }

            return physicalTarget;
        }

        public ActionResult Destroy(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);

            if (!string.IsNullOrEmpty(path))
            {
                if (entry.IsDirectory)
                {
                    DeleteDirectory(path);
                }
                else
                {
                    DeleteFile(entry);
                }
                return Json(new object[0]);
            }
            throw new HttpException(404, "File Not Found");
        }

        public void DeleteFile(FileManagerEntry entry)
        {
            if (!Authorize(entry.Path))
            {
                throw new HttpException(403, "Forbidden");
            }

            string[] FindPath = entry.Path.Split('/');
            var physicalPath = FindPath.Length == 2 ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) + @"\" + FindPath[1] : entry.Path;
            if (System.IO.File.Exists(physicalPath))
            {
                if (!IsTemporary)
                {
                    string FileExtension = entry.Extension.Replace(".", "");
                    string Query = "Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName=N'" + entry.Name + "' and Extension=N'" + FileExtension + "' And URL=N'" + physicalPath.Replace("/", "\\").Replace("\\" + entry.Name + entry.Extension, "") + "') Select @ID";
                    var _RecordID = Referral.DBAttachment.SelectField(Query);
                    if (!string.IsNullOrEmpty(_RecordID.ToString()))
                        Referral.DBAttachment.Delete("CoreObjectAttachment", Convert.ToInt32(_RecordID));
                    if (FindPath.Length == 2)
                    {
                        CopyEntry(physicalPath.Replace(MapFileSavingPath, MapFileDeletingPath).Replace("\\" + entry.Name + entry.Extension, ""), entry);
                    }
                    else
                    {
                        CopyEntry(physicalPath.Replace(Models.Attachment.AttachmentFolder, DeleteAttachmentFolder).Replace("/" + entry.Name + entry.Extension, "").Replace("\\", ""), entry);
                    }
                }

                System.IO.File.Delete(physicalPath);
            }
        }

        public void DeleteDirectory(string path)
        {
            if (!Authorize(path))
            {
                throw new HttpException(403, "Forbidden");
            }

            string[] FindPath = path.Split('/');
            var physicalPath = FindPath.Length == 2 ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) + @"\" + FindPath[1] : path;

            if (Directory.Exists(physicalPath))
            {
                if (!IsTemporary)
                {
                    string[] allfiles = Directory.GetFiles(physicalPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in allfiles)
                    {
                        FileInfo info = new FileInfo(file);
                        var _RecordID = Referral.DBAttachment.SelectField("Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName=N'" + info.Name.Replace(info.Extension, "") + "' and Extension=N'" + info.Extension.Replace(".", "") + "' And URL=N'" + info.Directory.FullName.Replace("/", "\\") + "') Select @ID");
                        if (!string.IsNullOrEmpty(_RecordID.ToString()))
                            Referral.DBAttachment.Delete("CoreObjectAttachment", Convert.ToInt32(_RecordID));
                    }
                    CopyDirectory(new DirectoryInfo(physicalPath), Directory.CreateDirectory(physicalPath.Replace(path, MapFileDeletingPath)));
                }

                Directory.Delete(physicalPath, true);
            }
        }

        public void ConfigPath(long _RecordID, long _InnerID, string _DataKey, string _ParentID)
        {
            CoreObject Entryform = CoreObject.Find(long.Parse(_DataKey));
            if (Entryform.Entity == CoreDefine.Entities.فایل_عمومی)
            {
                IsTemporary = false;
                RecordID = _RecordID;
            }
            else if ((Entryform.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات && _ParentID == "0" && Entryform.ParentID != 0) || _InnerID == 0 || long.Parse(_ParentID) < 0)
            {
                IsTemporary = true;
                RecordID = long.Parse(_DataKey);
            }
            else
            {
                IsTemporary = false;
                RecordID = _RecordID;
            }

            InnerID = _InnerID;
            MapFilePath = RecordID + "/" + InnerID;
        }
        public ActionResult Read(string target, long _RecordID, long _InnerID, string _DataKey, string _ParentID, [DataSourceRequest] DataSourceRequest _Request)
        {
            ConfigPath(_RecordID, _InnerID, _DataKey, _ParentID);
            CreateUserFolder(target);
            var path = NormalizePath(target);

            if (Authorize(path))
            {
                try
                {
                    directoryBrowser.Server = Server;

                    var result = directoryBrowser.GetFiles(path, Filter)
                        .Concat(directoryBrowser.GetDirectories(path)).Select(VirtualizePath);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (DirectoryNotFoundException)
                {
                    throw new HttpException(404, "File Not Found");
                }
            }

            throw new HttpException(403, "Forbidden");
        }
        public ActionResult Update(string target, long _RecordID, long _InnerID, string _DataKey, string _ParentID, FileManagerEntry entry)
        {
            ConfigPath(_RecordID, _InnerID, _DataKey, _ParentID);
            FileManagerEntry newEntry;

            if (!Authorize(NormalizePath(entry.Path)) && !Authorize(NormalizePath(target)))
            {
                throw new HttpException(403, "Forbidden");
            }

            newEntry = RenameEntry(entry);

            return Json(VirtualizePath(newEntry));
        }

        public FileManagerEntry RenameEntry(FileManagerEntry entry)
        {
            var path = NormalizePath(entry.Path);
            var physicalPath = entry.Path;
            var physicalTarget = EnsureUniqueName(Path.GetDirectoryName(path), entry);
            FileManagerEntry newEntry;

            if (entry.IsDirectory)
            {
                Directory.Move(physicalPath, physicalTarget);
                newEntry = directoryBrowser.GetDirectory(physicalTarget);
                if (!IsTemporary)
                {
                    string[] filepath = entry.Path.Split('/');
                    string OldFileName = filepath[filepath.Length - 1];
                    string OldURL = entry.Path.Replace("/", "\\");
                    string NewURL = entry.Path.Replace(OldFileName, entry.Name).Replace("/", "\\");
                    Referral.DBAttachment.Execute("update CoreObjectAttachment set URL=REPLACE(URL,N'" + OldURL + "',N'" + NewURL + "') where URL like N'%" + OldURL + "%'");
                }
            }
            else
            {
                var file = new FileInfo(physicalPath);
                System.IO.File.Move(file.FullName, physicalTarget);
                newEntry = directoryBrowser.GetFile(physicalTarget);

                if (!IsTemporary)
                {
                    string[] filepath = entry.Path.Split('/');
                    string OldFileName = filepath[filepath.Length - 1].Replace(entry.Extension, "");
                    string OldFileExtension = entry.Extension.Replace(".", "");
                    string Query = "Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName=N'" + OldFileName + "' and Extension=N'" + OldFileExtension + "' and URL=N'" + entry.Path.Replace("/" + OldFileName + "." + OldFileExtension, "").Replace("/", "\\") + "') Select @ID";
                    var Record = Referral.DBAttachment.SelectField(Query);
                    Referral.DBAttachment.UpdateRow(Convert.ToInt32(Record), 0, "CoreObjectAttachment", new string[] { "FullName" }, new object[] { newEntry.Name });
                }
            }

            return newEntry;
        }

        public bool AuthorizeUpload(string path, HttpPostedFileBase file)
        {
            if (!CanAccess(path))
            {
                throw new DirectoryNotFoundException(String.Format("The specified path cannot be found - {0}", path));
            }

            if (!IsValidFile(file.FileName))
            {
                throw new InvalidDataException(String.Format("The type of file is not allowed. Only {0} extensions are allowed.", Filter));
            }

            return true;
        }

        private bool IsValidFile(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var allowedExtensions = Filter.Split(',');

            return allowedExtensions.Any(e => e.Equals("*.*") || e.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (IsTemporary)
                    return MapTemporaryFilePath;
                else
                {
                    return CreateUserFolder(MapFileSavingPath);
                }
            }

            return path;
        }

        public ActionResult GetFileByte(string _Path, [DataSourceRequest] DataSourceRequest request)
        {
            string[] FindPath = _Path.Split('/');
            var physicalPath = FindPath.Length == 2 ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) + @"\" + FindPath[1] : _Path;
            byte[] FileByte = new byte[0];
            if (physicalPath.ToLower().EndsWith(".pdf"))
                FileByte = APM.Models.Attachment.CreateThumbnailPDF(physicalPath, 600, 650);
            else
            {
                string FileText = System.IO.File.ReadAllText(physicalPath);
                FileByte = Convert.FromBase64String(FileText);
            }

            JsonResult jsonResult = new JsonResult();
            jsonResult = Json(Convert.ToBase64String((byte[])FileByte));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetTextFile(string _Path, [DataSourceRequest] DataSourceRequest request)
        {
            string[] FindPath = _Path.Split('/');
            var physicalPath = FindPath.Length == 2 ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) + @"\" + FindPath[1] : _Path;
            byte[] FileByte = new byte[0];
            if (physicalPath.ToLower().EndsWith(".txt"))
            {
                string FileText = System.IO.File.ReadAllText(physicalPath);
                FileByte = Convert.FromBase64String(FileText);
            }
            string result = System.Text.Encoding.UTF8.GetString(FileByte);
            JsonResult jsonResult = new JsonResult();
            jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetFileByteWithName(long _RecordID, long _InnerID, string _DataKey, string _ParentID, long FileCoreObjectID, [DataSourceRequest] DataSourceRequest request, string FileName = "")
        {
            if (string.IsNullOrEmpty(FileName))
            {
                TableAttachment TableAttachment = new TableAttachment(CoreObject.Find(FileCoreObjectID));
                FileName = TableAttachment.FullName;
            }
            ConfigPath(_RecordID, _InnerID, _DataKey, _ParentID);
            var physicalPath = (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath);
            DirectoryInfo dir = new DirectoryInfo(physicalPath);
            byte[] FileByte = new byte[0];

            CreateUserFolder(dir.FullName);
            foreach (FileInfo FileItem in dir.GetFiles())
            {
                if (FileItem.Name.Replace(FileItem.Extension, "") == FileName)
                {
                    switch (FileItem.Extension.ToLower())
                    {
                        case ".jpg":
                        case ".img":
                        case ".png":
                        case ".jpeg":
                        case ".gif":
                            FileByte = APM.Models.Attachment.CreateThumbnailImage(FileItem.FullName, 700, 780);
                            break;

                        case ".doc":
                        case ".docx":
                            FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Theme/Image/images/Word.png"));
                            break;
                        case ".xls":
                        case ".xlsx":
                            FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Theme/Image/images/Excel.png"));
                            break;
                        case ".pdf":
                            FileByte = APM.Models.Attachment.CreateThumbnailPDF(FileItem.FullName, 400, 450);
                            break;
                        case ".zip":
                        case ".rar":
                            FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Theme/Image/images/Rar.png"));
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }

            JsonResult jsonResult = new JsonResult();
            jsonResult = Json(Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte)));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public JsonResult GetCountAttachment(string RecordID, string InnerID)
        {
            if (string.IsNullOrEmpty(RecordID) || string.IsNullOrEmpty(InnerID))
                return Json(0);
            return Json(Referral.DBAttachment.SelectField("Select Count(1) From CoreObjectAttachment Where RecordID=" + RecordID + " And InnerID=" + InnerID));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(string path, long _RecordID, long _InnerID, string _DataKey, string _ParentID, HttpPostedFileBase file)
        {
            ConfigPath(_RecordID, _InnerID, _DataKey, _ParentID);
            FileManagerEntry newEntry;
            path = NormalizePath(path);
            var fileName = Path.GetFileName(file.FileName);

            if (AuthorizeUpload(path, file))
            {
                var binaryReader = new BinaryReader(file.InputStream);
                byte[] fileData = binaryReader.ReadBytes(file.ContentLength);
                string Text = Convert.ToBase64String(fileData);
                System.IO.File.WriteAllText(path + "/" + fileName, Text);

                newEntry = directoryBrowser.GetFile(Path.Combine(path, fileName));
                byte[] ThumbnailImageBytes = Models.Attachment.CreateThumbnailImage(newEntry.Path);

                if (!IsTemporary)
                    if (Referral.PublicSetting.SaveFileValueInDataBase)
                    {
                        Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Value", "Thumbnail", "URL" }
                                                                     , new object[] { RecordID, InnerID, "", newEntry.Name, newEntry.Extension.Replace(".", ""), file.ContentLength, fileData, new byte[] { }, path.Replace("/", "\\").Replace(@"\\", "\\") });
                    }
                    else
                    {
                        Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "URL", "Thumbnail" }
                                                                     , new object[] { RecordID, InnerID, "", newEntry.Name, newEntry.Extension.Replace(".", ""), file.ContentLength, path.Replace("/", "\\").Replace(@"\\", "\\"), ThumbnailImageBytes });

                    }
                return Json(VirtualizePath(newEntry), "text/plain");
            }

            throw new HttpException(403, "Forbidden");
        }

        [HttpGet]
        public FileResult Download(string _FilePath)
        {
            string FinallPath = "";
            string[] FilesPath = _FilePath.Split(',');

            if (FilesPath.Length == 1)
            {
                if (Directory.Exists(FilesPath[0]))
                {
                    FinallPath = ConvertToZipFile(FilesPath);
                }
                else
                {
                    string[] FindPath = FilesPath[0].Split('/');
                    FinallPath = FindPath.Length == 2 ? (IsTemporary ? MapTemporaryFilePath : MapFileSavingPath) + @"\" + FindPath[1] : FilesPath[0];

                    FileInfo file2 = new FileInfo(FinallPath);
                    FileManagerEntry entry = directoryBrowser.GetFile(FinallPath);
                    if (!IsTemporary)
                    {
                        string Query = "Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName=N'" + entry.Name + "' and Extension=N'" + entry.Extension.Replace(".", "") + "') Select @ID";
                        var _RecordID = Referral.DBAttachment.SelectField(Query);

                        string FileName = file2.Name.Replace(file2.Extension, "");
                        string Extension = file2.Extension.Replace(".", "");

                        if (!string.IsNullOrEmpty(_RecordID.ToString()))
                            Referral.DBAttachment.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                          , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _RecordID, "ضمیمه", FileName, Extension, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });

                    }

                    if (!Attachment.CheckExistsDirectory(MapFileZipPath))
                        Directory.CreateDirectory(MapFileZipPath);

                    string FileText = System.IO.File.ReadAllText(entry.Path);
                    byte[] fileData = Convert.FromBase64String(FileText);
                    System.IO.File.WriteAllBytes(MapFileZipPath + "/" + entry.Name + entry.Extension, fileData);
                    FinallPath = MapFileZipPath + "/" + entry.Name + entry.Extension;
                }
            }
            else
                FinallPath = ConvertToZipFile(FilesPath);

            FileInfo file = new FileInfo(FinallPath);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = file.Name,
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            string contentType = MimeMapping.GetMimeMapping(file.Name);
            var readStream = System.IO.File.ReadAllBytes(FinallPath);
            return File(readStream, contentType);

        }



        [HttpGet]
        public FileResult DownloadFile(string DataKey, string ParentID, long RecordID, long InnerID,string FileNameDownload)
        {
            ConfigPath(RecordID , InnerID , DataKey, ParentID); 

            string FinallPath = "";
            string FilesPath =  NormalizePath("") ;


            if (Directory.Exists(FilesPath))
            {
                foreach(string FilePath in Directory.GetFiles(FilesPath))
                {
                    FileInfo fileInfo = new FileInfo(FilePath);
                    if(fileInfo.Name.Replace(fileInfo.Extension,"") == FileNameDownload)
                    {
                        FinallPath = FilePath;
                        break;
                    }

                } 
            }
  
            if(FinallPath == "") 
                return null;  

            FileInfo file2 = new FileInfo(FinallPath);
            FileManagerEntry entry = directoryBrowser.GetFile(FinallPath);
            if (!IsTemporary)
            {
                string Query = "Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName=N'" + entry.Name + "' and Extension=N'" + entry.Extension.Replace(".", "") + "') Select @ID";
                var _RecordID = Referral.DBAttachment.SelectField(Query);

                string FileName = file2.Name.Replace(file2.Extension, "");
                string Extension = file2.Extension.Replace(".", "");

                if (!string.IsNullOrEmpty(_RecordID.ToString()))
                    Referral.DBAttachment.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                    , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _RecordID, "ضمیمه", FileName, Extension, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });

            }

            if (!Attachment.CheckExistsDirectory(MapFileZipPath))
                Directory.CreateDirectory(MapFileZipPath);

            string FileText = System.IO.File.ReadAllText(entry.Path);
            byte[] fileData = Convert.FromBase64String(FileText);
            System.IO.File.WriteAllBytes(MapFileZipPath + "/" + entry.Name + entry.Extension, fileData);
            FinallPath = MapFileZipPath + "/" + entry.Name + entry.Extension;
             
    

            FileInfo file = new FileInfo(FinallPath);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = file.Name,
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            string contentType = MimeMapping.GetMimeMapping(file.Name);
            var readStream = System.IO.File.ReadAllBytes(FinallPath);
            return File(readStream, contentType);

        }


        public string ConvertToZipFile(string[] FilesPath)
        {
            string FinallPath = MapFileZipPath;
            string FinalFileName = "";

            if (Directory.Exists(MapFileZipPath))
            {
                Directory.Delete(MapFileZipPath, true);
            }

            foreach (string PhysicalTarget in FilesPath)
            {

                string[] FindPath = PhysicalTarget.Split('/');
                string PhysicalTargetPath = FindPath.Length == 2 ? MapFileSavingPath + @"\" + FindPath[1] : PhysicalTarget;
                FileInfo FileInfoDetail = new FileInfo(PhysicalTargetPath);

                if (FileInfoDetail.Attributes == System.IO.FileAttributes.Directory)
                {
                    FinalFileName = FileInfoDetail.Name;
                    FinallPath += "\\" + FileInfoDetail.Name;
                    CopyDirectory(new DirectoryInfo(FileInfoDetail.FullName), Directory.CreateDirectory(FinallPath));
                    string[] allfiles = Directory.GetFiles(FileInfoDetail.FullName, "*.*", SearchOption.AllDirectories);
                    foreach (var _file in allfiles)
                    {
                        FileInfo info = new FileInfo(_file);
                        string[] FileShortName = info.Name.Split('.');

                        string FileText = System.IO.File.ReadAllText(info.FullName);
                        byte[] fileData = Convert.FromBase64String(FileText);
                        System.IO.File.WriteAllBytes(FinallPath + "\\" + info.Name, fileData);


                        string Query = "Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName+N'.'+Extension=N'" + info.Name + "' and Extension=N'" + info.Extension.Replace(".", "") + "') Select @ID";
                        var _RecordID = Referral.DBAttachment.SelectField(Query);
                        string FileName = info.Name.Replace(info.Extension, "");
                        string Extension = info.Extension.Replace(".", "");

                        if (!string.IsNullOrEmpty(_RecordID.ToString()))
                            Referral.DBCore.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                          , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _RecordID, "ضمیمه", FileName, Extension, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });

                    }
                }
                else
                {
                    FinalFileName = FileInfoDetail.Name.Replace(FileInfoDetail.Extension, "");
                    FileManagerEntry entry = directoryBrowser.GetFile(PhysicalTargetPath);
                    CopyEntry(MapFileZipPath, entry);

                    string FileText = System.IO.File.ReadAllText(entry.Path);
                    byte[] fileData = Convert.FromBase64String(FileText);
                    System.IO.File.WriteAllBytes(MapFileZipPath + "/" + entry.Name + entry.Extension, fileData);


                    var _RecordID = Referral.DBAttachment.SelectField("Declare @ID as bigint= (select top 1 CoreObjectAttachmentID from CoreObjectAttachment where RecordID=" + RecordID + " and InnerID=" + InnerID + " and FullName=N'" + entry.Name + "' and Extension=N'" + entry.Extension.Replace(".", "") + "' ) Select @ID");

                    string FileName = entry.Name;
                    string Extension = entry.Extension.Replace(".", "");

                    if (!string.IsNullOrEmpty(_RecordID.ToString()))
                        Referral.DBCore.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                      , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _RecordID, "ضمیمه", FileName, Extension, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });

                }

            }

            using (ZipFile zip = new ZipFile())
            {
                zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(MapFileZipPath);
                zip.Comment = Referral.PublicSetting.AppEnglishName + ":" + "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.Save(MapFileZipPath + "\\" + FinalFileName + ".zip");
            }
            return MapFileZipPath + "\\" + FinalFileName + ".zip";
        }

        public JsonResult SaveImageEditor(string ImageData, string Url, string Name, string Extension)
        {
            string tem = ImageData.Substring(ImageData.IndexOf(",") + 1, ImageData.Length - ImageData.IndexOf(",") - 1);

            byte[] imageBytes = Convert.FromBase64String(tem);
            string FileUrl = Url.Replace("/", "\\");
            System.IO.File.WriteAllBytes(FileUrl, imageBytes);
            byte[] ThumbnailImageBytes = Models.Attachment.CreateThumbnailImage(FileUrl);
            long AttachmentID = long.Parse(Referral.DBAttachment.SelectField("SELECT ISNULL(CoreObjectAttachmentID,0) FROM CoreObjectAttachment  where FullName=N'" + Name + "' And URL = N'" + FileUrl.Replace("\\" + Name + Extension, "") + "'").ToString());
            Referral.DBAttachment.UpdateRow(AttachmentID, 0, "CoreObjectAttachment", new string[] { "Thumbnail" }, new object[] { ThumbnailImageBytes });
            return Json(0);
        }


    }


    public class FileContentBrowser
    {
        public IEnumerable<FileManagerEntry> GetFiles(string path, string filter)
        {
            var directory = new DirectoryInfo(path);

            var extensions = (filter ?? "*").Split(new string[] { ", ", ",", "; ", ";" }, System.StringSplitOptions.RemoveEmptyEntries);

            return extensions.SelectMany(directory.GetFiles)
                .Select(file => new FileManagerEntry
                {
                    Name = Path.GetFileNameWithoutExtension(file.Name),
                    Size = file.Length,
                    Path = file.FullName,
                    Extension = file.Extension,
                    IsDirectory = false,
                    HasDirectories = false,
                    Created = file.CreationTime,
                    CreatedUtc = file.CreationTimeUtc,
                    Modified = file.LastWriteTime,
                    ModifiedUtc = file.LastWriteTimeUtc
                });
        }

        public IEnumerable<FileManagerEntry> GetDirectories(string path)
        {
            var directory = new DirectoryInfo(path);

            return directory.GetDirectories()
                .Select(subDirectory => new FileManagerEntry
                {
                    Name = subDirectory.Name,
                    Path = subDirectory.FullName,
                    Extension = subDirectory.Extension,
                    IsDirectory = true,
                    HasDirectories = subDirectory.GetDirectories().Length > 0,
                    Created = subDirectory.CreationTime,
                    CreatedUtc = subDirectory.CreationTimeUtc,
                    Modified = subDirectory.LastWriteTime,
                    ModifiedUtc = subDirectory.LastWriteTimeUtc
                });
        }

        public FileManagerEntry GetDirectory(string path)
        {
            var directory = new DirectoryInfo(path);

            return new FileManagerEntry
            {
                Name = directory.Name,
                Path = directory.FullName,
                Extension = directory.Extension,
                IsDirectory = true,
                HasDirectories = directory.GetDirectories().Length > 0,
                Created = directory.CreationTime,
                CreatedUtc = directory.CreationTimeUtc,
                Modified = directory.LastWriteTime,
                ModifiedUtc = directory.LastWriteTimeUtc
            };
        }

        public FileManagerEntry GetFile(string path)
        {
            var file = new FileInfo(path);

            return new FileManagerEntry
            {
                Name = Path.GetFileNameWithoutExtension(file.Name),
                Path = file.FullName,
                Size = file.Length,
                Extension = file.Extension,
                IsDirectory = false,
                HasDirectories = false,
                Created = file.CreationTime,
                CreatedUtc = file.CreationTimeUtc,
                Modified = file.LastWriteTime,
                ModifiedUtc = file.LastWriteTimeUtc
            };
        }

        public HttpServerUtilityBase Server
        {
            get;
            set;
        }
    }
}
