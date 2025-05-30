using APM.Models;
using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class ManagementController : Controller
    {
        // GET: Management
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CreatePublickKey()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Attachment" + "/PublickKey/" + Referral.UserAccount.UsersID.ToString() + "/");
            string[] PathArr = Session["TableButtonEventsType"].ToString().Split('_');
            CoreObject AttCore = CoreObject.Find(long.Parse(PathArr[0]));

            bool Result = false;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }
        public ActionResult ProductUpdateFromTaxOrganization()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath("~/Attachment" + "/UploadProductUpdateFromTaxOrganization/" + Referral.UserAccount.UsersID.ToString() + "/");

            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            { 
                string[] TxtLine = System.IO.File.ReadAllLines(file.FullName.Replace(file.Extension, ".txt"));
                DataTable DataInfo = Referral.DBData.SelectDataTable("Select شماره_کالا from کالاهای_امور_مالیاتی");
                foreach (string Item in TxtLine)
                {
                    string[] ProductInfo = Item.Split(',');

                    DataRow[] foundRows = DataInfo.Select("شماره_کالا = '" + ProductInfo[0] + "'");
                    if (foundRows.Length == 0 && ProductInfo[0] != "ID")
                    {
                        if (ProductInfo[5] == "")
                            Referral.DBData.Execute("Insert into کالاهای_امور_مالیاتی(شماره_کالا, عنوان, تاریخ_تولید, تاریخ_انقضا, ماهیت_استفاده, نوع_مالیات, مالیات_بر_ارزش_افزوده, اهداف_مالیات, گروه_کالا) Values(N'" + ProductInfo[0] + "',N'" + ProductInfo[9] + "',N'" + ProductInfo[3].Replace("-", "/") + "',N'" + ProductInfo[2].Replace("-", "/") + "',0,N'" + ProductInfo[6] + "'," + ProductInfo[7] + ",N'" + ProductInfo[8] + "',N'" + ProductInfo[1] + "')");
                        else
                        {

                        }
                    }
                    break;
                }

                System.IO.File.Delete(file.FullName);
            }
            bool Result = false;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadProductUpdateFromTaxOrganization(IEnumerable<HttpPostedFileBase> files)
        {
            bool Result = false;
            if (files != null)
            {
                var file = files.ElementAt(0);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Attachment" + "/UploadProductUpdateFromTaxOrganization/" + Referral.UserAccount.UsersID.ToString() + "/");

                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (FileInfo FileItem in dir.GetFiles())
                    {
                        System.IO.File.Delete(FileItem.FullName);
                    }
                    FileInfo file1 = new FileInfo(Path.Combine(path, file.FileName));

                    var binaryReader = new BinaryReader(file.InputStream);
                    byte[] fileData = binaryReader.ReadBytes(file.ContentLength);

                string utfString = Encoding.UTF8.GetString(fileData, 0, fileData.Length);
                //".txt"
                System.IO.File.WriteAllText(path + "/" + file.FileName, utfString);
                Result = true;
            }
            else
                Result = true;
            return Content(Result ? "" : "خطا در ذخیره سازی");
        }

    }
}