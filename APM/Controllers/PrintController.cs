using APM.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class PrintController : Controller
    {
        // GET: Print
        public ActionResult Index(string _DataKey, string _ParentID, [DataSourceRequest] DataSourceRequest _Request)
        {
            ViewData["DataKey"] = _DataKey;
            ViewData["DataKey"] = _ParentID;
            return View("PrintReport");
        }

        public ActionResult Print(string _DataKey, string _ParentID, [DataSourceRequest] DataSourceRequest _Request)
        {
            ViewData["DataKey"] = _DataKey;
            ViewData["DataKey"] = _ParentID;
            return View("PrintReport"); 
            //var jsonResult = Json(Desktop.CachedTable[_DataKey].ToDataSourceResult(_Request));
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
        }

        public ActionResult BPMNModel()
        {
            return View();
        }

    }
}