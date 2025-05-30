using APM.Models;
using APM.Models.Database;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class RoleTypePermissionController : Controller
    {
        // GET: RoleTypePermission
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RoleTypePermission(int RoleTypeID)
        {
            string RoleTypeName="", Permission="", DefaultRoleTypeUrl = "";
            if (RoleTypeID > 0)
            {
                string Query = "SELECT  شناسه , عنوان, دسترسی ,لینک_صفحه_پیشفرض FROM نقش_کاربر where شناسه =" + RoleTypeID;
                Record _Record = new Record(Referral.DBData, Query);
                RoleTypeName = (string)_Record.Field("عنوان", "");
                Permission = (string)_Record.Field("دسترسی", "");
                DefaultRoleTypeUrl = (string)_Record.Field("لینک_صفحه_پیشفرض", "");
            }
            ViewData["RoleTypeName"] = RoleTypeName;
            ViewData["Permission"] = Permission;
            ViewData["DefaultRoleTypeUrl"] = DefaultRoleTypeUrl;
            return PartialView();
        }

        public JsonResult SavePermision(int RoleTypeID, string Title,string DefaultRoleTypeUrl, string Permission)
        {
            CoreObject UserRole=CoreObject.Find(CoreDefine.Entities.جدول, "نقش_کاربر");

            if (RoleTypeID == 0)
            {
                int ID = Referral.DBData.Insert(UserRole.FullName, new string[] { "عنوان", "دسترسی" , "لینک_صفحه_پیشفرض" }, new object[] { Title, Permission, DefaultRoleTypeUrl });
            }
            else
            {
                bool result= Referral.DBData.UpdateRow(RoleTypeID, UserRole.CoreObjectID, UserRole.FullName, new string[] { "عنوان", "دسترسی", "لینک_صفحه_پیشفرض" }, new object[] { Title, Permission , DefaultRoleTypeUrl });
            }
            return Json(1);
        }

    }
}