using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class EditorFormController : Controller
    {
        // GET: EditorForm
        public ActionResult Index(long FormID)
        {
            return View();
        }
    }
}