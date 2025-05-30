using APM.Models;
using APM.Models.Database;
using APM.Models.Tools;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index(long DashboardID, string FromDate, string ToDate)
        {
            ViewData["DashboardID"] = DashboardID;
            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            return View();
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
        public ActionResult Viewer(long DashboardID, string FromDate, string ToDate,string[] SearchFieldItem, string[] SearchFieldOperator,string[] SearchFieldValue)
        { 
            ViewData["DashboardID"] = DashboardID;
            ViewData["FromDate"]=FromDate;
            ViewData["ToDate"]=ToDate;  
            ViewData["SearchFieldItem"] = SearchFieldItem;  
            ViewData["SearchFieldOperator"] = SearchFieldOperator;  
            ViewData["SearchFieldValue"] = SearchFieldValue;  
            return View("~/Views/Dashboard/Viewer.cshtml");
        }
        
        public ActionResult SubViewer(long SubDashboardID, string category, string FromDate, string ToDate)
        { 
            ViewData["DashboardID"] = SubDashboardID; 
            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            string WhereQuery = "";
            string GroupQuery = "";
            string OrderQuery = "";
            CoreObject DashboardCore = CoreObject.Find(SubDashboardID);
            SubDashboard subDashboard = new SubDashboard(DashboardCore);

            while (DashboardCore.Entity != CoreDefine.Entities.داشبورد)
            {
                WhereQuery +=Session["WhereDashboardID" + DashboardCore.ParentID.ToString()]==null?"": Session["WhereDashboardID" + DashboardCore.ParentID.ToString()].ToString()+" And\n";
                DashboardCore = CoreObject.Find(DashboardCore.ParentID);
            }

            if(subDashboard.Condition!="")
            {
                WhereQuery += subDashboard.Condition + " And \n";
            }

            if (subDashboard.DateField>0)
            {
                Field field=new Field(CoreObject.Find(subDashboard.GroupField));
                Field DateField=new Field(CoreObject.Find(subDashboard.DateField));

                if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.سال)
                {
                    Session["GroupDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,4)";
                    Session["OrderDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,4)";
                    Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + "SUBSTRING(" + DateField.FieldName + ",1,4) = N'" + category + "'\n";

                }
                else if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.ماه)
                { 
                    Session["GroupDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,7)";
                    Session["OrderDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,7)";
                    Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + "SUBSTRING(" + DateField.FieldName + ",1,7) = N'" + category + "'\n";
                }
                else if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.روز)
                { 
                    Session["GroupDashboardID" + SubDashboardID.ToString()] = DateField.FieldName;
                    Session["OrderDashboardID" + SubDashboardID.ToString()] = DateField.FieldName;
                    Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + DateField.FieldName + " = N'" + category + "'\n";
                }
                else
                {

                    switch (field.FieldType)
                    {
                        case CoreDefine.InputTypes.TwoValues:
                            {
                                Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + "(case when(" + field.FieldName + "=0) then N'" + field.ComboValues()[0] + "' when(" + field.FieldName + "=1) then N'" + field.ComboValues()[1] + "' else N'نامشخص' end)  = N'" + category + "'\n";
                                break;
                            }
                        case CoreDefine.InputTypes.ComboBox:
                        case CoreDefine.InputTypes.ShortText:
                        case CoreDefine.InputTypes.LongText:
                        case CoreDefine.InputTypes.Plaque:
                        case CoreDefine.InputTypes.NationalCode:
                        case CoreDefine.InputTypes.Phone:
                        case CoreDefine.InputTypes.PersianDateTime:
                        case CoreDefine.InputTypes.MiladyDateTime:
                            {
                                Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + field.FieldName + " = N'" + category + "'\n";
                                break;
                            }
                        case CoreDefine.InputTypes.RelatedTable:
                            {
                                Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + DataConvertor.GetRelatedTableQueryForDashboard(field) + " = N'" + category + "'\n";
                                break;
                            }
                        default:
                            {

                                break;
                            }
                    }
                }

            } 
             
            return View("~/Views/Dashboard/Viewer.cshtml");
        }

 

        public JsonResult ReloadPieSubDashboard(long DashboardID,string FromDate,string ToDate)
        {
            SubDashboard subDashboard = new SubDashboard(CoreObject.Find(DashboardID));
            DataTable Data = Desktop.GetDashboardData(FromDate, ToDate, subDashboard);
            dynamic[] DynamicData = new dynamic[Data.Rows.Count];
            for (int Index = 0; Index < Data.Rows.Count; Index++)
            {
                DynamicData[Index] = new { category = Data.Rows[Index][0].ToString() == "" ? "تهی" : Data.Rows[Index][0], value = Data.Rows[Index][1] };
            }
            return Json(DynamicData);
        }
        public JsonResult ReloadBarSubDashboard(long DashboardID,string FromDate,string ToDate)
        {
            SubDashboard subDashboard = new SubDashboard(CoreObject.Find(DashboardID));
            DataTable Data = Desktop.GetDashboardData(FromDate, ToDate, subDashboard);
            double[] SeriesBar = new double[Data.Rows.Count];
            string[] Categories = new string[Data.Rows.Count];
            for (int Index = 0; Index < Data.Rows.Count; Index++)
            {
                Categories[Index] = Data.Rows[Index][0].ToString();
                SeriesBar[Index] = double.Parse(Data.Rows[Index][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            }
            return Json(new { SeriesBar= SeriesBar, Categories= Categories });
        }

        public ActionResult ShowDetailDashboard(long SubDashboardID, string Category, string FromDate,string ToDate)
        {  
            CoreObject DashboardCore = CoreObject.Find(SubDashboardID);

            string WhereQuery = "";
            string GroupQuery = "";
            string OrderQuery = "";
            while (DashboardCore.Entity != CoreDefine.Entities.داشبورد)
            {
                WhereQuery += Session["WhereDashboardID" + DashboardCore.ParentID.ToString()] == null ? "" : Session["WhereDashboardID" + DashboardCore.ParentID.ToString()].ToString() + " And\n";
                DashboardCore = CoreObject.Find(DashboardCore.ParentID); 
            }

            Dashboard Dashboard = new Dashboard(DashboardCore);

            SubDashboard subDashboard = new SubDashboard(CoreObject.Find(SubDashboardID));


            if (subDashboard.Condition != "")
            {
                WhereQuery += subDashboard.Condition + " And \n";
            }

            Session["ChartFromDate"] = FromDate;
            Session["ChartToDate"] = ToDate;

            if(Desktop.DataInformationEntryForm[Dashboard.InformationEntryForm.ToString()]==null)
               Desktop.StartupSetting(Dashboard.InformationEntryForm.ToString()); 

            Session["MasterDataKey"] = Dashboard.InformationEntryForm.ToString();
            ViewData["DataKey"] = Dashboard.InformationEntryForm.ToString();
            ViewData["ParentID"] = "0";
            ViewData["ProcessID"] = 0;
            ViewData["RecordID"] = "0";
            ViewData["ProcessStep"] = "0";
            ViewData["SubDashboardID"] = SubDashboardID.ToString();
            ViewData["Category"] = Category;


            if (subDashboard.DateField > 0)
            {
                Field field = new Field(CoreObject.Find(subDashboard.GroupField));
                Field DateField = new Field(CoreObject.Find(subDashboard.DateField));

                if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.سال)
                {
                    Session["GroupDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,4)";
                    Session["OrderDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,4)";
                    Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + "SUBSTRING(" + DateField.FieldName + ",1,4) = N'" + Category + "'\n";

                }
                else if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.ماه)
                {
                    Session["GroupDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,7)";
                    Session["OrderDashboardID" + SubDashboardID.ToString()] = "SUBSTRING(" + DateField.FieldName + ",1,7)";
                    Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + "SUBSTRING(" + DateField.FieldName + ",1,7) = N'" + Category + "'\n";
                }
                else if (subDashboard.ChartGroupDateType == CoreDefine.ChartGroupDate.روز)
                {
                    Session["GroupDashboardID" + SubDashboardID.ToString()] = DateField.FieldName;
                    Session["OrderDashboardID" + SubDashboardID.ToString()] = DateField.FieldName;
                    Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + DateField.FieldName + " = N'" + Category + "'\n";
                }
                else
                {
                    switch (field.FieldType)
                    {
                        case CoreDefine.InputTypes.TwoValues:
                            {
                                Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + "(case when(" + field.FieldName + "=0) then N'" + field.ComboValues()[0] + "' when(" + field.FieldName + "=1) then N'" + field.ComboValues()[1] + "' else N'نامشخص' end)  = N'" + Category + "'\n";
                                break;
                            }
                        case CoreDefine.InputTypes.ComboBox:
                        case CoreDefine.InputTypes.ShortText:
                        case CoreDefine.InputTypes.LongText:
                        case CoreDefine.InputTypes.Plaque:
                        case CoreDefine.InputTypes.NationalCode:
                        case CoreDefine.InputTypes.Phone:
                        case CoreDefine.InputTypes.PersianDateTime:
                        case CoreDefine.InputTypes.MiladyDateTime:
                            {
                                Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + field.FieldName + " = N'" + Category + "'\n";
                                break;
                            }
                        case CoreDefine.InputTypes.RelatedTable:
                            {
                                Session["WhereDashboardID" + SubDashboardID.ToString()] = WhereQuery + " " + DataConvertor.GetRelatedTableQueryForDashboard(field) + " = N'" + Category + "'\n";
                                break;
                            }
                        default:
                            {

                                break;
                            }
                    }
                }

            }


            return View("~/Views/Shared/InputForm/TGrid.cshtml"); 
        }


        public ActionResult Read([DataSourceRequest] DataSourceRequest _Request, string ChartName = "", string FromDate="", string ToDate="", long SubDashboardID=0, string[] SearchFieldItem = null, string[] SearchFieldOperator = null, string[] SearchFieldValue = null,bool IsReload=false)
        {
            JsonResult jsonResult = new JsonResult();
            if (IsReload)
            {
                SubDashboard subDashboard = new SubDashboard(CoreObject.Find(SubDashboardID)); 
                jsonResult = Json((Desktop.GetDashboardData(FromDate, ToDate, subDashboard, SearchFieldItem, SearchFieldOperator, SearchFieldValue)).ToDataSourceResult(_Request));
            }
            else
                jsonResult = Json(((DataTable)Session[ChartName]).ToDataSourceResult(_Request));

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}