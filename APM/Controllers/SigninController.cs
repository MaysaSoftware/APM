using APM.Models;
using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.NetWork;
using APM.Models.Security;
using APM.Models.Tools;
using IronOcr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc; 

namespace APM.Controllers
{
    //[RequireHttps]
    public class SigninController : Controller
    {
        // GET: Signin
        //[OutputCache(Duration = 60*60*24)]
        public ActionResult Index()
        {
            if (!UserAuthorization.CanUserVisit()) return Redirect(UserAuthorization.ExclusionURL());
            string AppVersion = "1.3.49";
            Session.Clear();
            Response.Cache.SetCacheability(HttpCacheability.Public); 
            ViewData["Version"] = AppVersion;
            Referral.AppVersion = AppVersion;
            Session["UnsuccessfulLogin"] = 1;
            if (Referral.PublicSetting is null)
            {
                if (Login.ConnectToDB())
                {
                    if (!Directory.Exists(Log.ErrorLogPath))
                    {
                        Directory.CreateDirectory(Log.ErrorLogPath);
                    }

                    if (!Directory.Exists(Log.FuncionLogPath))
                    {
                        Directory.CreateDirectory(Log.FuncionLogPath);
                    }
                }
                else
                    return View("~/Views/Error/DisconnectDB.cshtml");
            } 
            return View(); 
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Password)
        {
            bool Allowed = false;
            string Message = "";
            if(string.IsNullOrEmpty(UserName))
            { 
                ViewData["Login.Message"] = "نام کاربری را وارد نمایید";
                Session["UnsuccessfulLogin"] = ((int)Session["UnsuccessfulLogin"]) + 1;
                return View("Index");
            }
            
            if(string.IsNullOrEmpty(Password))
            { 
                ViewData["Login.Message"] = "کلمه عبور را وارد نمایید";
                Session["UnsuccessfulLogin"] = ((int)Session["UnsuccessfulLogin"]) + 1;
                return View("Index");
            }

            Login.UserAuthentication(UserName, Password, ref Allowed, ref Message);

            ViewData["Login.Message"] = Message;
            if (Allowed)
            {
                if (Referral.CoreObjects.Count < 3)
                {
                    Software.CoreReload();

                    List<CoreObject> DBList = CoreObject.FindChilds(0, CoreDefine.Entities.پایگاه_داده);
                    foreach (CoreObject DB in DBList)
                    {
                        DataSourceInfo dataSourceInfo = new DataSourceInfo(DB);
                        if (dataSourceInfo.DataBase == Referral.DBData.ConnectionData.DataBase)
                        {
                            Referral.MasterDatabaseID = DB.CoreObjectID;
                            break;
                        }
                    }
                } 

                if (string.IsNullOrEmpty(Referral.UserAccount.DefaultLink))
                    return Redirect("~/Home/Index");
                else
                {
                    string [] Link = Referral.UserAccount.DefaultLink.Split('/');
                    Link[0] = Link[2] = "";
                    string LinkStr = "~/";
                    for(int i = 3; i < Link.Length; i++)
                    {
                        LinkStr += Link[i].ToString();
                        if (i < Link.Length - 1)
                            LinkStr += "/";
                    } 
                    return Redirect(LinkStr);
                } 
            }
            else
            {
                Session["UnsuccessfulLogin"] = ((int)Session["UnsuccessfulLogin"]) + 1;
                return View("Index");
            }
        }



        public ActionResult Signup()
        {
            return PartialView();
        }
        public ActionResult ForgotPassword()
        {
            return PartialView();
        }

        public ActionResult VerifyCode()
        {
            return PartialView();
        }
        public JsonResult SendCode(string SendType, string UserName)
        {
            if(SendType== "SMS")
            {
                if (Referral.PublicSetting.RelatedWebService == 0)
                    return Json("سامانه پیامکی برای این سامانه تعریف نشده است");
                else
                {
                    if (Referral.DBData.SelectField("Select count(1) from کاربر where شماره_موبایل=N'" + UserName + "'").ToString() != "0")
                    {
                        string postData = "";
                        WebServiceRequest webServiceRequest = new WebServiceRequest() { Method = new WebServiceMethod().Post };
                        Random rnd = new Random();
                        int RandomNum = rnd.Next(100000,999999);

                        if(!Software.CheckCoreIsReload())
                            Software.JustCoreReload();

                        webServiceRequest.GenarateUrlFromWebService(Referral.PublicSetting.RelatedWebService, "", new string[] {"گیرنده","کد_رندوم"}, new object[] { UserName , RandomNum }, ref postData);
                        webServiceRequest.SendRequest(); 
                    }
                    else 
                        return Json("شماره موبایل در این سامانه تعریف نشده است");

                }    
            }
            return Json(1);
        }

        public JsonResult Logout()
        {
            if (Referral.UserAccount != null)
                Referral.DBRegistry.Execute("update Login_APMRegistry set LogOffDate=N'" + CDateTime.GetNowshamsiDate() + "',LogOffTime=N'" + CDateTime.GetNowTime() + "',IsActive=0 where UserAccountID=" + Referral.UserAccount.UsersID.ToString() + " and LoginDate=N'" + Referral.UserAccount.LoginDate + "' and LoginTime=N'" + Referral.UserAccount.LoginTime + "'");
            return Json(null);
        }
    }
}