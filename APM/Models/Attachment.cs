using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.Tools;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace APM.Models
{
    public static class Attachment
    {

        public static string TemporaryFolder = "TemporaryAttachment/" + Referral.UserAccount.UsersID;
        public static string AttachmentFolder = "CoreObjectAttachment";
        public static string ZipAttachmentFolder = "ZipAttachment"; 
        public static string MapFilePath { get; set; }
        public static string MapFileZipPath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return System.Web.HttpContext.Current.Server.MapPath("~/Content/ZipAttachmentFolder" + "/" + ZipAttachmentFolder + "/" + MapFilePath + "/" + Referral.UserAccount.UsersID);
                else
                    return Referral.PublicSetting.FileSavingPath + "/" + ZipAttachmentFolder + "/" + MapFilePath + "/" + Referral.UserAccount.UsersID;
            }
        }

        public static string MapFileSavingPath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return MapFileSavingAttachmentPath + MapFilePath;
                else
                    return MapFileSavingAttachmentPath + MapFilePath;
            }
        }
        public static string MapTemporaryFilePath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return System.Web.HttpContext.Current.Server.MapPath("~/Content/TemporaryFolder" + "/" + TemporaryFolder + "/") ;
                else
                    return Referral.PublicSetting.FileSavingPath + "/" + TemporaryFolder + "/" ;
            }
        }

        public static string MapFileSavingAttachmentPath
        {
            get
            {
                if (Referral.PublicSetting.FileSavingPath == "")
                    return System.Web.HttpContext.Current.Server.MapPath("~/Attachment" + "/" + AttachmentFolder + "/") ;
                else
                    return Referral.PublicSetting.FileSavingPath + "/" + AttachmentFolder + "/";
            }
        }

        public static bool CheckExistsDirectory(string _Path)
        {
            try
            {   
                return Directory.Exists(_Path);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CheckExistsFile(string _Path)
        {
            try
            { 
                return File.Exists(_Path);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string GetDirectory(string _Path)
        {
            try
            {   
                return System.IO.Path.GetDirectoryName(_Path);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static bool Authorize(string path)
        {
            return path.StartsWith(path, StringComparison.OrdinalIgnoreCase);
        }
         
        public static void DeleteDirectory(string path)
        {
            if (!Authorize(path))
            {
                throw new HttpException(403, "Forbidden");
            }
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        public static bool DeleteFile(string path)
        {
            try
            { 
                if (!Authorize(path))
                {
                    throw new HttpException(403, "Forbidden");
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        } 
        public static void SaveAttachment(byte[] File, DirectoryInfo target,long RecordID,long InnerID)
        { 
                //foreach (FileInfo file in source.GetFiles())
                //{
                //    file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                //    string FileName = file.Name.IndexOf('.') > -1 ? file.Name.Substring(0, file.Name.IndexOf('.')) : file.Name;
                //    byte[] ThumbnailImageBytes =new byte[0];
                //    switch (file.Extension.ToLower())
                //    {
                //        case ".jpg":
                //        case ".img":
                //        case ".png":
                //        case ".jpeg":
                //        case ".gif":
                //            ThumbnailImageBytes = CreateThumbnailImage(target.FullName.ToString() + "/" + FileName + file.Extension);
                //            break;
                //    } 
                //    Referral.DBData.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL" }
                //                                 , new object[] { RecordID, InnerID, "", FileName, file.Extension.Replace(".", ""), file.Length, ThumbnailImageBytes, target.FullName.ToString() });
                //}

                //foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                //{
                //    DirectoryInfo nextTargetSubDir =
                //        target.CreateSubdirectory(diSourceSubDir.Name);
                //    SaveAttachment(diSourceSubDir, nextTargetSubDir, RecordID, InnerID);
                //} 
        }
        public static void SaveAttachment(DirectoryInfo source, DirectoryInfo target,long RecordID,long InnerID)
        {
            if (source.Exists)
            {
                List<CoreObject> AttachmentList = CoreObject.FindChilds(RecordID, CoreDefine.Entities.ضمیمه_جدول);
                foreach (FileInfo file in source.GetFiles())
                {
                    file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                    string FileName = file.Name.IndexOf('.') > -1 ? file.Name.Substring(0, file.Name.IndexOf('.')) : file.Name;
                    byte[] ThumbnailImageBytes =new byte[0];
                    bool SaveValue=false;
                    TableAttachment tableAttachment = new TableAttachment();
                    foreach (CoreObject attachment in AttachmentList)
                    {
                        if(attachment.FullName == FileName)
                        {
                            tableAttachment=new TableAttachment(attachment);
                            SaveValue = tableAttachment.SaveInDatabase;
                            break;
                        }
                    } 

                    if(SaveValue)
                    {
                        long AttachmentID = long.Parse(Referral.DBAttachment.SelectField("Select Isnull((SELECT CoreObjectAttachmentID FROM CoreObjectAttachment  where FullName=N'" + FileName + "' And  RecordID=" + RecordID.ToString() + " And InnerID=0 And Folder=N'" + Referral.UserAccount.UsersID.ToString() + "'),0)").ToString());
                        if (AttachmentID > 0)
                            Referral.DBAttachment.UpdateRow(AttachmentID, 0, "CoreObjectAttachment", new string[] { "InnerID", "Folder","URL" }, new object[] { InnerID ,"", target.FullName.ToString() });
                        else
                        {
                            switch (file.Extension.ToLower())
                            {
                                case ".jpg":
                                case ".img":
                                case ".png":
                                case ".jpeg":
                                case ".gif":
                                    ThumbnailImageBytes = CreateThumbnailImage(target.FullName.ToString() + "/" + FileName + file.Extension);
                                    break;
                            }

                            string FileText = File.ReadAllText(target.FullName.ToString() + "/" + FileName + file.Extension);
                            byte[] fileData = Convert.FromBase64String(FileText);
                            long id= Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL","Value" }
                                 , new object[] { RecordID, InnerID, "", FileName, file.Extension.Replace(".", ""), file.Length, ThumbnailImageBytes, target.FullName.ToString(), fileData });

                        }
                    }
                    else
                    {
                        switch (file.Extension.ToLower())
                        {
                            case ".jpg":
                            case ".img":
                            case ".png":
                            case ".jpeg":
                            case ".gif":
                                ThumbnailImageBytes = CreateThumbnailImage(target.FullName.ToString() + "/" + FileName + file.Extension);
                                break;
                        }
                        Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL" }
                             , new object[] { RecordID, InnerID, "", FileName, file.Extension.Replace(".", ""), file.Length, ThumbnailImageBytes, target.FullName.ToString() });
                    }

                }

                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);
                    SaveAttachment(diSourceSubDir, nextTargetSubDir, RecordID, InnerID);
                }
            }
        }


        public static byte[] CreateThumbnailPDF(string fileName, int width = 64, int height = 88)
        { 
            string FileText = File.ReadAllText(fileName); 
            byte[] fileData = Convert.FromBase64String(FileText);
            Stream stream = new MemoryStream(fileData); 
            Document pdfDocument = new Document(stream);
            var page = pdfDocument.Pages[1];
            using (FileStream imageStream = new FileStream(fileName.Replace(".pdf",Referral.UserAccount.UsersID.ToString()+ ".jpg"), FileMode.Create))
            {
                Resolution resolution = new Resolution(300);
                JpegDevice jpegDevice = new JpegDevice(width, height, resolution, 900);
                jpegDevice.Process(page, imageStream);
                imageStream.Close();

                FileInfo fileinfo = new FileInfo(fileName.Replace(".pdf", Referral.UserAccount.UsersID.ToString() + ".jpg"));

                byte[] _FileByte = File.ReadAllBytes(fileinfo.FullName);
                File.Delete(fileinfo.FullName);
                return _FileByte;
            }
        }
        public static byte[] CreateThumbnailImage(string fileName, int width = 64, int height = 88)
        {

            if (fileName.ToLower().EndsWith(".pdf"))
            {
                return CreateThumbnailPDF(fileName, 200, 240);
            }
            else if (fileName.ToLower().EndsWith(".jpg") || fileName.ToLower().EndsWith(".img") || fileName.ToLower().EndsWith(".png") || fileName.ToLower().EndsWith(".jpeg"))
            {
                string FileText = File.ReadAllText(fileName);
                FileInfo fileinfo = new FileInfo(fileName.Replace("CoreObjectAttachment", "ThumbnailImageAttachment").Replace("TemporaryAttachment", "ThumbnailImageAttachment"));
                if (!CheckExistsDirectory(fileinfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileinfo.DirectoryName);
                }
                byte[] fileData = Convert.FromBase64String(FileText);

                if (fileinfo.Exists)
                    DeleteFile(fileinfo.FullName);

                System.IO.File.WriteAllBytes(fileinfo.FullName, fileData);
                fileName = fileinfo.FullName;

                System.Drawing.Image image = System.Drawing.Image.FromFile(fileName.Replace("/", "\\"));
                int origWidth = image.Size.Width;
                int origHeight = image.Size.Height;
                if ((origWidth / origHeight) < 0)
                {
                    width = 200;
                    height = origWidth * (200 / origHeight);
                }
                else
                {
                    origWidth = origWidth / ((origHeight / 200) == 0 ? 1 : (origHeight / 200));
                    origHeight = 200;
                }

                System.Drawing.Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                image.Dispose();
                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(thumb, typeof(byte[]));
                DeleteFile(fileName);
                return xByte;
            }
            else
                return new byte[0];

        }

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo Destination)
        {
            if (source.Exists)
            {
                foreach (FileInfo file in source.GetFiles())
                {
                    file.CopyTo(Path.Combine(Destination.FullName, file.Name), true); 
                } 
            }
        }
        
        public static bool CopyFile(FileInfo source, FileInfo Destination)
        {
            try
            {
                if (source.Exists) 
                    source.CopyTo(Path.Combine(Destination.FullName, source.Name), true); 
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void RenameDirectory(DirectoryInfo Source, DirectoryInfo Destination)
        {
            if (Directory.Exists(Source.FullName))
            {
                Directory.Move(Source.FullName, Destination.FullName);
            }
        }
        
        public static bool RenameFile(FileInfo Source, FileInfo Destination)
        {
            try
            { 
                if (File.Exists(Source.FullName))
                    File.Move(Source.FullName, Destination.FullName); 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GetFileByte(string RecordID,string InnerID,string FileFullName,bool IsTemporary = false)
        {
            DirectoryInfo dir =new DirectoryInfo((IsTemporary? MapTemporaryFilePath: MapFileSavingAttachmentPath) + @"\" + RecordID + "\\" + InnerID);
            var physicalPath = (IsTemporary ? MapTemporaryFilePath : MapFileSavingAttachmentPath) + @"\" + RecordID+"\\"+InnerID;

            if (Directory.Exists(physicalPath))
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (file.Name.Replace(file.Extension, "") == FileFullName)
                    {
                        byte[] _FileByte = new byte[0];
                        if (file.Extension.ToLower() == ".pdf")
                        {
                            _FileByte = CreateThumbnailPDF(file.FullName, 600, 650);
                        }
                        else
                         _FileByte = CreateThumbnailImage(file.FullName, 700, 750);
                         
                        string _Result = Models.Database.Field.FormatImage(Convert.ToBase64String((byte[])_FileByte));
                        return _Result;

                    }
                }

                foreach (DirectoryInfo diSourceSubDir in dir.GetDirectories())
                {
                } 
            } 
                return Database.Field.FormatImage(Referral.PublicSetting.AppLogo);
        }



        public static string ConvertToZipFile(string[] FilesPath,long CoreObjectID=0)
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
                    }
                }
                else
                {
                    FinalFileName = FileInfoDetail.Name.Replace(FileInfoDetail.Extension, "");

                    using (ZipFile zip = new ZipFile())
                    {
                        zip.UseUnicodeAsNecessary = true;  // utf-8
                        zip.AddDirectory(FileInfoDetail.Directory.ToString());
                        zip.Comment = Referral.PublicSetting.AppEnglishName + ":" + "This zip was created at " + System.DateTime.Now.ToString("G");
                        zip.Save(FileInfoDetail.Directory.ToString() + "\\" + FinalFileName + ".zip");
                    }
                    return FileInfoDetail.Directory.ToString() + "\\" + FinalFileName + ".zip";


                    //FileManagerEntry entry = directoryBrowser.GetFile(PhysicalTargetPath);
                    //CopyEntry(MapFileZipPath, entry);

                    //FileInfoDetail.CopyTo(Path.Combine(MapFileZipPath, file.Name), true);

                    //    Referral.DBCore.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                    //  , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _RecordID, "ضمیمه", FileName, Extension, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });

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

    }
}