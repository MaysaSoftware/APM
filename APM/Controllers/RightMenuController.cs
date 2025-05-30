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
    public class RightMenuController : Controller
    {
        // GET: RightMenu
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult InformationEntryFormBadge(long InformationEntryFormID)
        {
            InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(InformationEntryFormID));
            CoreObject DataSourceCore = CoreObject.Find(CoreObject.Find(informationEntryForm.RelatedTable).ParentID);
            object ResultBadge = Desktop.SelectField(new DataSourceInfo(DataSourceCore), Tools.CheckQuery(informationEntryForm.BadgeQuery));
            long Number = long.Parse(ResultBadge == "" ? "0" : ResultBadge.ToString());
            return Json(Number);
        }
    }
}