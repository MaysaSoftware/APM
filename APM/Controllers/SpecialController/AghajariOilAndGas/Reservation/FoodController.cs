
using APM.Controllers.SpecialController.AghajariOilAndGas.Reservation;
using APM.Models;
using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.SpecialModels.AghajariOilAndGas.Reservation;
using APM.Models.Tools;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers.SpecialController.AghajariOilAndGas.Reservation
{

    public class FoodController : Controller
    {
        // GET: Food
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Person()
        {
            List<SelectListItem> mealList = new List<SelectListItem>();

            string now = CDateTime.GetNowshamsiDate();

            string query = $@"
                    SELECT [شناسه], [عنوان]
                    FROM [وعده_غذایی]
                    WHERE [تاریخ_شروع_فعالسازی] <= N'{now}'
                      AND [تاریخ_پایان_فعالسازی] >= N'{now}'";

            DataTable dt = await Referral.DBData.SelectDataTableAsync(query);

            foreach (DataRow row in dt.Rows)
            {
                mealList.Add(new SelectListItem
                {
                    Value = row[0].ToString(),
                    Text = row[1].ToString()
                });
            }

            ViewBag.Meal = mealList;
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Person.cshtml");
        }


        public ActionResult Shift()
        {
            ViewData["IsSatellite"] = false;
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Shift.cshtml");
        }
        public ActionResult Shift2()
        {
            ViewData["IsSatellite"] = true;
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Shift.cshtml");
        }
        public ActionResult ShiftMeal(long RecordID, string FromDate, string ToDate, long Restaurant, string SelectedMeal, string SelectedMealTitle, bool IsSatellite, string SearchOfReserveID)
        {
            ViewData["RecordID"] = RecordID;
            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            ViewData["Restaurant"] = Restaurant;
            ViewData["IsSatellite"] = IsSatellite;
            ViewData["SelectedMeal"] = SelectedMeal.Replace(",", "_");
            ViewData["SearchOfReserveID"] = SearchOfReserveID;

            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Shift/Meal.cshtml");
        }
        public ActionResult Office()
        {
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Office.cshtml");
        }
        public async Task<JsonResult> CheckReserve(long RecordID, string FromDate, string ToDate, long Restaurant, string SelectedMeal, string SelectedMealTitle, string SelectedDey, bool IsReserveBed)
        {
            //ViewData["RecordID"] = RecordID;
            //ViewData["FromDate"] = FromDate;
            //ViewData["ToDate"] = ToDate;
            //ViewData["Restaurant"] = Restaurant;

            string[] MealArr = SelectedMeal.Split(',');
            string[] MealTitleArr = SelectedMealTitle.Split(',');
            string[] MealSelected = new string[0];
            DataTable MealData = await Referral.DBData.SelectDataTableAsync("Select * from وعده_غذایی");
            string NowDate = CDateTime.GetNowshamsiDate();
            string NowTime = CDateTime.GetNowTime();

            CoreObject RequestReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "درخواست_رزرو_غذا");
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            CoreObject BedTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");

            List<CoreObject> TableEventList = (CoreObject.FindChilds(RequestReservationTableCore.CoreObjectID, CoreDefine.Entities.رویداد_جدول));

            //string[] CoulmnName = new string[] {"شناسه" ,"از_تاریخ", "تا_تاریخ", "نوع_درخواست_رزرو_غذا", "شماره_حساب", "برای_اداره",   "راننده", "وسیله_نقلیه", "توضیحات", "وعده_غذایی", "روزهای_هفته", "رستوران" };
            //object[] ColumnValue = new object[] { RecordID, FromDate, ToDate, RequestType, OfficeDepartmentAccountNumber, ForOffice,  Driver, Vehicle, Message, SelectedMeal, SelectedDey, Restaurant };


            string[] CoulmnName = new string[] { "شناسه", "از_تاریخ", "تا_تاریخ", "وعده_غذایی", "روزهای_هفته", "رستوران" };
            object[] ColumnValue = new object[] { RecordID, FromDate, ToDate, SelectedMeal, SelectedDey, Restaurant };


            foreach (CoreObject ParameterCore in TableEventList)
            {
                TableEvent Event = new TableEvent(ParameterCore);
                if (Event.EventType == (RecordID > 0 ? CoreDefine.TableEvents.شرط_اجرای_ویرایش.ToString() : CoreDefine.TableEvents.شرط_اجرای_درج.ToString()))
                {
                    string Query = Referral.DBData.DefineVariablesQuery(RequestReservationTableCore.FullName, RecordID, CoulmnName, ColumnValue) + "\n";
                    var Alarm = await Referral.DBData.SelectFieldAsync(Query + "\n" + Tools.CheckQuery(Event.Query));
                    if (Alarm.ToString() != "")
                        return Json(Alarm);

                }
            }
            return Json("");
        }
        public ActionResult PersonOffice(long RecordID, string FromDate, string ToDate, long Restaurant, string SelectedMeal, string SelectedMealTitle, string SelectedDey, bool IsReserveBed)
        {
            ViewData["RecordID"] = RecordID;
            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            ViewData["Restaurant"] = Restaurant;
            ViewData["SelectedMeal"] = SelectedMeal.Split(',');
            ViewData["SelectedMealTitle"] = SelectedMealTitle.Split(',');
            SelectedDey = SelectedDey.Replace("جمعه", "7").Replace("پنج_شنبه", "6").Replace("چهار_شنبه", "5").Replace("سه_شنبه", "4").Replace("دو_شنبه", "3").Replace("یک_شنبه", "2").Replace("شنبه", "1");
            ViewData["SelectedDey"] = SelectedDey.Split(',');
            ViewData["IsReserveBed"] = IsReserveBed;

            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Office/PersonOffice.cshtml");
        }
        public ActionResult Letter()
        {
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Letter.cshtml");
        }

        public ActionResult LetterMeal(long RecordID, string FromDate, string ToDate, long Restaurant, string SelectedMeal, string SelectedMealTitle, string SelectedDey, bool IsSharingFood, int[] DefualtFoodCount)
        {
            ViewData["RecordID"] = RecordID;
            ViewData["FromDate"] = FromDate;
            ViewData["ToDate"] = ToDate;
            ViewData["IsSharingFood"] = IsSharingFood;
            ViewData["DefualtFoodCount"] = DefualtFoodCount;
            ViewData["Restaurant"] = Restaurant;
            ViewData["SelectedMeal"] = SelectedMeal.Split(',');
            ViewData["SelectedMealTitle"] = SelectedMealTitle.Split(',');
            SelectedDey = SelectedDey.Replace("جمعه", "7").Replace("پنج_شنبه", "6").Replace("چهار_شنبه", "5").Replace("سه_شنبه", "4").Replace("دو_شنبه", "3").Replace("یک_شنبه", "2").Replace("شنبه", "1");
            ViewData["SelectedDey"] = SelectedDey.Split(',');
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Letter/Meal.cshtml");
        }
        public ActionResult Delayed()
        {
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Delayed.cshtml");
        }

        public async Task<ActionResult> UpdateReservation(long RecordID)
        {
            DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
            ViewData["RecordID"] = RecordID;
            //ViewData["ReserveData"] = ReserveData.Rows[0];

            switch (ReserveData.Rows[0]["نوع_درخواست_رزرو_غذا"].ToString())
            {
                case "2": return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Letter.cshtml");
                case "3": return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Office.cshtml");
                case "4": return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Delayed.cshtml");
                case "5": return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Shift.cshtml");
                case "6":
                    {
                        ViewData["IsSatellite"] = true;
                        return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Shift.cshtml");
                    }
            }
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/Permision.cshtml");
        }

        public async Task<ActionResult> UpdateChangeFood(long RecordID)
        {
            DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From نفرات_رزرو_غذا Where شناسه = " + RecordID.ToString());
            ViewData["RecordID"] = RecordID;
            return View("~/Views/SpecialForm/AghajariOilAndGas/Reservation/Food/ChangeFood.cshtml");
        }

        public async Task<ActionResult> OfficeDepartmentAccountNumber([DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("شماره_حساب_واحد_سازمانی"), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<ActionResult> GetVehiclesOfficeDepartmentAccountNumber([DataSourceRequest] DataSourceRequest _Request, string FromDate = "", string ToDate = "", int OfficeDepartmentAccountNumber = 0, int Meal = 0)
        {
            string Query = "Declare @نوع_دسترسی_کاربر as bigint =(SELECT  کاربر.نوع_دسترسی_کاربر FROM کاربر WHERE کاربر.شناسه = @شناسه_کاربر)  \n";
            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("خودرو_قابل_دسترسی", Query), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<ActionResult> GetDriverOfficeDepartmentAccountNumber([DataSourceRequest] DataSourceRequest _Request, long Vehicle = 0)
        {
            string Query = "Declare @شناسه_خودرو as bigint = " + Vehicle + " \n";
            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("راننده_خودرو", Query), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<ActionResult> GetAllOffice([DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("لیست_کل_اداره_ها"), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public async Task<ActionResult> PersonRestaurant([DataSourceRequest] DataSourceRequest _Request, string FromDate = "", string ToDate = "", int Meal = 0, int RequestType = 1)
        {
            string Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
            Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + ToDate + "' \n";
            Query += "declare @وعده_غذایی  as bigint = " + Meal + " \n";
            Query += "declare @نوع_درخواست  as bigint = " + RequestType + " \n";

            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("رستوران_کاربر", Query), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<ActionResult> ChangePersonFood([DataSourceRequest] DataSourceRequest _Request, string FromDate, long Restaurant, long Meal = 0)
        {
            string Query = "Declare @تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
            Query += "declare @رستوران  as bigint = " + Restaurant + " \n";
            Query += "declare @وعده_غذایی  as bigint = " + Meal + " \n";

            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("تغییر_غذای_کاربر", Query), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public async Task<JsonResult> SaveChangePersonFood([DataSourceRequest] DataSourceRequest _Request, long RecordID, long Restaurant, long Food)
        {
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            string[] CoulmnName = new string[] { "رستوران", "غذا" };
            object[] ColumnValue = new object[] { Restaurant, Food };

            string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
            if (Alarm != "")
                return Json(Alarm);
            else
            {
                if (Referral.DBData.UpdateRow(RecordID, PersonReservationTableCore.CoreObjectID, PersonReservationTableCore.FullName, CoulmnName, ColumnValue))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, PersonReservationTableCore, RecordID, CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                return Json("");
            }
        }

        public async Task<ActionResult> PersonFood(string FromDate, string ToDate, int? Meal, int? Restaurant, [DataSourceRequest] DataSourceRequest _Request)
        {
            string Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
            Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + ToDate + "' \n";
            Query += "declare @وعده_غذایی  as bigint = " + Meal + " \n";
            Query += "declare @مکان  as bigint = " + Restaurant + " \n";
            Query += "declare @غذا  as bigint =0 \n";
            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("غذای_کاربر", Query), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public async Task<JsonResult> GetPersonFood(string FromDate, string ToDate, int? Meal, int? Restaurant)
        {
            string Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
            Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + ToDate + "' \n";
            Query += "declare @وعده_غذایی  as bigint = " + Meal + " \n";
            Query += "declare @مکان  as bigint = " + Restaurant + " \n";
            Query += "declare @غذا  as bigint =0 \n";
            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "غذای_کاربر");
            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
            var list = JsonConvert.SerializeObject(await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query)),
                                                    Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                    });

            return Json(list, "application/json");
        }

        public async Task<JsonResult> SavePersonReservationFood(FormCollection form)
        {
            string ErrorMessage = string.Empty;
            string FromDate = form["StartDateReservationFood"];
            string ToDate = form["EndDateReservationFood"];
            bool IsTrue = (FromDate == ToDate);
            //if (string.Compare(FromDate, CDateTime.GetNowshamsiDate()) ==-1 )
            //    return Json(new { ErrorMessage = "تاریخ شروع نمی تواند کوچکتر از تاریخ امروز باشد" });
            //if(string.Compare(ToDate, CDateTime.GetNowshamsiDate()) ==-1 )
            //    return Json(new { ErrorMessage = "تاریخ شروع نمی تواند کوچکتر از تاریخ امروز باشد" });

            List<FoodReservation> FoodList = new List<FoodReservation>();

            foreach (var MealItem in form.AllKeys)
            {
                if (MealItem.StartsWith("MealReservationFood"))
                {
                    string MealValue = MealItem.Replace("MealReservationFood_", "");
                    int Restaurant = form["PersonRestaurant_" + MealValue] == null ? 0 : int.Parse(form["PersonRestaurant_" + MealValue]);
                    int Food = form["PersonFood_" + MealValue] == null ? 0 : int.Parse(form["PersonFood_" + MealValue]);
                    if (Restaurant > 0 && Food > 0)
                    {
                        string Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
                        Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + ToDate + "' \n";
                        Query += "declare @وعده_غذایی  as bigint = " + MealValue + " \n";
                        Query += "declare @مکان  as bigint = " + Restaurant + " \n";
                        Query += "declare @غذا  as bigint = " + Food + " \n";
                        CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "غذای_کاربر");
                        SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                        Query += specialPhrase.Query;
                        if ((await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query))).Rows.Count == 0)
                            return Json(new { ErrorMessage = "وعده غذایی [ " + form[MealItem] + " ] " });
                        else
                        {

                            FoodList.Add(new FoodReservation() { Meal = int.Parse(MealValue), Restaurant = Restaurant, Food = Food });

                        }
                    }
                    else if (Restaurant > 0 && !IsTrue)
                    {
                        FoodList.Add(new FoodReservation() { Meal = int.Parse(MealValue), Restaurant = Restaurant, Food = 0 });
                    }
                    //else if(Restaurant > 0 && Food==0) 
                    //    return Json(new { ErrorMessage = "غذایی برای رستوران مورد نظر انتخاب نشده است " });

                }
            }

            if (FoodList.Count == 0)
                return Json(new { ErrorMessage = "وعده غذایی انتخاب نشده است " });
            else
            {
                CoreObject TableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "رزرو_غذا");
                if (IsTrue)
                {
                    foreach (var food in FoodList)
                    {
                        Referral.DBData.Insert("رزرو_غذا"
                            , new string[] { "تاریخ_رزرو", "پرسنل_ثبت_کننده", "تاریخ_ثبت", "ساعت_ثبت", "رستوران", "نام_پرسنل_ثبت_کننده", "پرسنل", "وعده_غذایی", "غذا", "تعداد", "کاربر_ثبت_کننده" }
                            , new object[] { FromDate, Referral.UserAccount.PersonnelID, CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), food.Restaurant, Referral.UserAccount.FullName, Referral.UserAccount.PersonnelID, food.Meal, food.Food, 1, Referral.UserAccount.UsersID }
                            , TableCore.CoreObjectID);
                    }
                }
                else
                {
                    while (string.Compare(FromDate, ToDate) <= 0)
                    {
                        foreach (var food in FoodList)
                        {
                            string Query = "Declare @تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
                            Query += "declare @وعده_غذایی  as bigint = " + food.Meal + " \n";
                            Query += "declare @مکان  as bigint = " + food.Restaurant + " \n";
                            Query += "declare @غذا  as bigint = " + food.Food + " \n";
                            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "غذای_پیش_فرض_کاربر");
                            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                            Query += specialPhrase.Query;
                            var FoodValue = await Referral.DBData.SelectFieldAsync(Tools.CheckQuery(Query));
                            food.Food = FoodValue.ToString() == "" ? 0 : int.Parse(FoodValue.ToString());
                            if (food.Food > 0)
                                Referral.DBData.Insert("رزرو_غذا"
                                    , new string[] { "تاریخ_رزرو", "پرسنل_ثبت_کننده", "تاریخ_ثبت", "ساعت_ثبت", "رستوران", "نام_پرسنل_ثبت_کننده", "پرسنل", "وعده_غذایی", "غذا", "تعداد", "کاربر_ثبت_کننده" }
                                    , new object[] { FromDate, Referral.UserAccount.PersonnelID, CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), food.Restaurant, Referral.UserAccount.FullName, Referral.UserAccount.PersonnelID, food.Meal, food.Food, 1, Referral.UserAccount.UsersID }
                                    , TableCore.CoreObjectID);
                        }
                        FromDate = CDateTime.AddDay(FromDate, 1);
                    }
                }

            }
            return Json(new { ErrorMessage = "" });

        }

        public async Task<ActionResult> GetMeal([DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = Json(await DataConvertor.FillSelectListWithQueryAsync("وعده_غذایی_کاربر"), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public async Task<ActionResult> GetRestaurantOfficeDepartmentAccountNumber(string FromDate, string ToDate, int OfficeDepartmentAccountNumber, int Meal, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult;
            SelectList ListData = new SelectList(new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "هیچکدام", Selected = true } }, "Value", "Text", 0);

            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "شماره_حساب_واحد_سازمانی");
            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
            ListData = DataConvertor.ToSelectList(await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(specialPhrase.Query)));
            jsonResult = Json(ListData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<ActionResult> GetPersonOfficeDepartmentAccountNumber([DataSourceRequest] DataSourceRequest _Request, string FromDate = "", string ToDate = "", int OfficeDepartmentAccountNumber = 0, int Meal = 0)
        {
            JsonResult jsonResult;
            SelectList ListData = new SelectList(new List<SelectListItem>() { new SelectListItem() { Value = "0", Text = "هیچکدام", Selected = true } }, "Value", "Text", 0);
            if (Session["PersonOfficeDepartment"] == null)
            {
                string Query = "Declare @نوع_دسترسی_کاربر as bigint =(SELECT  کاربر.نوع_دسترسی_کاربر FROM کاربر WHERE کاربر.شناسه = @شناسه_کاربر)  \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "پرسنل_قابل_دسترسی");
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                ListData = DataConvertor.ToSelectList(await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query)));
                Session["PersonOfficeDepartment"] = ListData;
            }
            jsonResult = Json((SelectList)Session["PersonOfficeDepartment"], JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<JsonResult> GetPersonInfo(string SearchValue, int RequestType = 0, string FromDate = "", string ToDate = "", int Restaurant = 0, int Meal = 0, long RecordID = 0, string SelectedMeal = "", bool BedReservationFood = false)
        {
            string Query = "Declare @مقدار as nvarchar(400) = N'" + SearchValue + "' \n";
            Query += "Declare @نوع_دسترسی_کاربر as bigint =(SELECT  کاربر.نوع_دسترسی_کاربر FROM کاربر WHERE کاربر.شناسه = @شناسه_کاربر)  \n";
            string SpecialPhraseName = "بررسی_پرسنل_اداره";
            switch (RequestType)
            {
                case 5: { SpecialPhraseName = "بررسی_پرسنل_نوبتکار"; break; }
            }
            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, SpecialPhraseName);
            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
            DataTable PersonData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
            DataTable Data = new DataTable();
            foreach (DataColumn dataColumn in PersonData.Columns)
            {
                Data.Columns.Add(dataColumn.ColumnName, dataColumn.DataType);
            }

            string ErrorMessage = string.Empty;
            foreach (DataRow Row in PersonData.Rows)
            {
                string Message = CheckPersonInfoToReserve(long.Parse(Row["شناسه"].ToString()), FromDate, ToDate, Restaurant, Meal, RecordID, SelectedMeal, BedReservationFood).Data.ToString();
                if (Message != "")
                {
                    ErrorMessage += Message + "\n";
                }
                else
                {
                    Data.Rows.Add();
                    foreach (DataColumn dataColumn in PersonData.Columns)
                    {
                        Data.Rows[Data.Rows.Count - 1][dataColumn.ColumnName] = Row[dataColumn.ColumnName];
                    }

                }
            }
            var list = JsonConvert.SerializeObject(Data,
                                                    Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
                                                    });

            return Json(new { Data = list, Error = ErrorMessage }, "application/json");
        }
        public JsonResult GetDelayedPersonInfo(string SearchValue)
        {
            string Query = "Declare @مقدار as nvarchar(400) = N'" + SearchValue + "' \n";
            Query += "Declare @نوع_دسترسی_کاربر as bigint =(SELECT  کاربر.نوع_دسترسی_کاربر FROM کاربر WHERE کاربر.شناسه = @شناسه_کاربر)  \n";
            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "بررسی_پرسنل_روزکار");
            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
            var list = JsonConvert.SerializeObject(Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query)),
                                                    Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                    });

            return Json(list, "application/json");
        }
        public JsonResult GetDelayedPersonInfoBySearchOfReserveID(string SearchValue)
        {
            string Query = "Declare @درخواست_رزرو_غذا as nvarchar(400) = N'" + SearchValue + "' \n";
            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "جستجو_پرسنل_روزکار_براساس_شناسه_رزرو");
            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
            var list = JsonConvert.SerializeObject(Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query)),
                                                    Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                    });
            return Json(new { PersonelData = list, ReserveData = new Record(Referral.DBData, "Select * from درخواست_رزرو_غذا where شناسه = " + SearchValue) }, "application/json");
        }
        public JsonResult CheckPersonInfoToReserve(long Personel = 0, string FromDate = "", string ToDate = "", int Restaurant = 0, int Meal = 0, long RecordID = 0, string SelectedMeal = "", bool BedReservationFood = false)
        {
            string Query = string.Empty;
            SpecialPhrase specialPhrase = new SpecialPhrase();
            if (Meal > 0)
            {
                Query = "Declare @از_تاریخ as nvarchar(400) = N'" + FromDate + "' \n";
                Query += "Declare @تا_تاریخ as nvarchar(400) = N'" + ToDate + "' \n";
                Query += "Declare @رستوران as Bigint = " + Restaurant + " \n";
                Query += "Declare @وعده_غذایی as Bigint = " + Meal + " \n";
                Query += "Declare @پرسنل as Bigint = " + Personel + " \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "بررسی_پرسنل_جهت_رزرو_غذا");
                specialPhrase = new SpecialPhrase(SpecialPhraseCore);

                return Json(Referral.DBData.SelectField(Tools.CheckQuery(Query + specialPhrase.Query)));
            }
            else if (SelectedMeal != "")
            {
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "بررسی_پرسنل_جهت_رزرو_غذا");
                specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                string[] SelectedMealArr = SelectedMeal.Split(',');
                foreach (string str in SelectedMealArr)
                {
                    Query = "Declare @از_تاریخ as nvarchar(400) = N'" + FromDate + "' \n";
                    Query += "Declare @تا_تاریخ as nvarchar(400) = N'" + ToDate + "' \n";
                    Query += "Declare @رستوران as Bigint = " + Restaurant + " \n";
                    Query += "Declare @وعده_غذایی as Bigint = " + str + " \n";
                    Query += "Declare @پرسنل as Bigint = " + Personel + " \n";
                    string Message = Referral.DBData.SelectField(Tools.CheckQuery(Query + specialPhrase.Query)).ToString();
                    if (Message != "")
                        return Json(Message);
                }

            }
            else
            {
                Query = "Declare @تاریخ as nvarchar(400) = N'" + FromDate + "' \n";
                Query += "Declare @رزرو_غذا  as Bigint = " + RecordID + " \n";
                Query += "Declare @پرسنل as Bigint = " + Personel + " \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "بررسی_پرسنل_جهت_رزرو_تخت");
                specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                return Json(Referral.DBData.SelectField(Tools.CheckQuery(Query + specialPhrase.Query)));
            }
            return Json("");
        }

        public JsonResult GetLetterPersonInfo(string SearchValue)
        {
            string Query = "Declare @مقدار as nvarchar(400) = N'" + SearchValue + "' \n";
            CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "بررسی_پرسنل_نامه");
            SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
            var list = JsonConvert.SerializeObject(Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query)),
                                                    Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                    });

            return Json(list.Replace("  ", ""), "application/json");
        }

        public async Task<JsonResult> SaveOfficeReservationFood(string[] GridJSON, long RecordID = 0, string FromDate = "", string ToDate = "", int RequestType = 0, int OfficeDepartmentAccountNumber = 0, string NumberLetter = "", string SubjectLetter = "", string Message = "", int Restaurant = 0, long Vehicle = 0, long Driver = 0, long PersonFood = 0, int FoodCount = 0, long PersonID = 0, long BedID = 0, long ForOffice = 0, string SelectedMeal = "", string SelectedDey = "")
        {
            CoreObject RequestReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "درخواست_رزرو_غذا");
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            CoreObject BedTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
            bool IsEqualDate = (FromDate == ToDate);
            string ErrorMessage = "";
            RequestType = 3;
            string Alarm = string.Empty;
            long[] PersonOfBed = new long[0];
            long ParentRowID = RecordID;
            string[] OldMeal = new string[0];
            string[] NewMeal = new string[0];
            string[] CoulmnName = new string[] { "از_تاریخ", "تا_تاریخ", "نوع_درخواست_رزرو_غذا", "شماره_حساب", "برای_اداره", "موضوع_نامه", "شماره_نامه", "راننده", "وسیله_نقلیه", "توضیحات", "وعده_غذایی", "روزهای_هفته", "رستوران" };
            object[] ColumnValue = new object[] { FromDate, ToDate, RequestType, OfficeDepartmentAccountNumber, ForOffice, SubjectLetter, NumberLetter, Driver, Vehicle, Message, SelectedMeal, SelectedDey, Restaurant };
            DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
            if (RecordID > 0)
            {

                if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != SelectedMeal)
                {
                    OldMeal = ReserveData.Rows[0]["وعده_غذایی"].ToString().Split(',');
                    NewMeal = SelectedMeal.Split(',');
                    foreach (string OldMealItem in OldMeal)
                        if (Array.IndexOf(NewMeal, OldMealItem) == -1)
                        {
                            await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND وعده_غذایی = " + OldMealItem);
                        }
                }

                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });

                if (Referral.DBData.UpdateRow(ParentRowID, RequestReservationTableCore.CoreObjectID, RequestReservationTableCore.FullName, CoulmnName, ColumnValue))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());

                if (string.Compare(ReserveData.Rows[0]["از_تاریخ"].ToString(), FromDate) == -1)
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو < N'" + FromDate + "'");
                }
                if (string.Compare(ReserveData.Rows[0]["تا_تاریخ"].ToString(), ToDate) == 1)
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو > N'" + ToDate + "'");
                }
            }
            else
            {
                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });
                ParentRowID = Referral.DBData.Insert(RequestReservationTableCore.FullName, CoulmnName, ColumnValue, RequestReservationTableCore.CoreObjectID);
                if (ParentRowID > 0)
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
            }

            foreach (string PersonGrid in GridJSON)
            {
                if (PersonGrid != "[]")
                {
                    try
                    {
                        DataTable PersonData = DataConvertor.JsonStringToDataTable(PersonGrid);
                        CoulmnName = new string[] { "درخواست_رزرو_غذا", "پرسنل", "وعده_غذایی", "رستوران", "غذا", "تاریخ_رزرو", "تعداد" };
                        DataTable PersonReserve = await Referral.DBData.SelectDataTableAsync("select * FROM نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو <= N'" + FromDate + "' AND تاریخ_رزرو >= N'" + ToDate + "'");
                        if (PersonData.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in PersonData.Rows)
                            {
                                if (PersonData.Columns.IndexOf("شناسه_درخواست_غذا") > -1)
                                    if (dataRow["شناسه_درخواست_غذا"].ToString() == "0" || string.Join(",", NewMeal) != string.Join(",", OldMeal))
                                    {
                                        foreach (DataColumn dataColumn in PersonData.Columns)
                                        {
                                            if (dataColumn.ColumnName.IndexOf("GridMealReservationFood_") > -1)
                                            {
                                                if (long.Parse(dataRow[dataColumn].ToString()) > 0 && Array.IndexOf(OldMeal, dataColumn.ColumnName.Replace("GridMealReservationFood_", "")) == -1)
                                                {
                                                    ColumnValue = new object[] { ParentRowID, dataRow["شناسه"].ToString(), dataColumn.ColumnName.Replace("GridMealReservationFood_", ""), Restaurant, dataRow[dataColumn].ToString(), dataRow["تاریخ"].ToString(), 1 };
                                                    Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                                                    if (Alarm != "")
                                                        ErrorMessage += Alarm + "\n";
                                                    else
                                                    {
                                                        long RowID = Referral.DBData.Insert(PersonReservationTableCore.FullName, CoulmnName, ColumnValue, PersonReservationTableCore.CoreObjectID);
                                                        await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, PersonReservationTableCore, RowID, CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                                                    }
                                                }
                                            }
                                            else if (dataColumn.ColumnName.IndexOf("تخت") > -1)
                                            {
                                                if (dataRow[dataColumn].ToString() == "true")
                                                {
                                                    if (Array.IndexOf(PersonOfBed, long.Parse(dataRow["شناسه"].ToString())) == -1)
                                                    {
                                                        string[] BedCoulmnName = new string[] { "رزرو_غذا", "پرسنل" };
                                                        object[] BedColumnValue = new object[] { ParentRowID, dataRow["شناسه"].ToString() };
                                                        Alarm = Desktop.CheckBeforRunQuery(BedTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, BedCoulmnName, BedColumnValue);
                                                        if (Alarm != "")
                                                            ErrorMessage += Alarm + "\n";
                                                        else
                                                        {
                                                            long RowID = Referral.DBData.Insert(BedTableCore.FullName, BedCoulmnName, BedColumnValue, BedTableCore.CoreObjectID);
                                                            await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, BedTableCore, RowID, BedCoulmnName, BedColumnValue, "", BedTableCore.CoreObjectID.ToString());
                                                        }
                                                        Array.Resize(ref PersonOfBed, PersonOfBed.Length + 1);
                                                        PersonOfBed[PersonOfBed.Length - 1] = long.Parse(dataRow["شناسه"].ToString());
                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (DataColumn dataColumn in PersonData.Columns)
                                        {
                                            if (dataColumn.ColumnName.IndexOf("GridMealReservationFood_") > -1)
                                            {
                                                if (long.Parse(dataRow[dataColumn].ToString()) > 0 && Array.IndexOf(OldMeal, dataColumn.ColumnName.Replace("GridMealReservationFood_", "")) == -1)
                                                {
                                                    DataRow[] PersonRow = PersonReserve.Select(" درخواست_رزرو_غذا=" + RecordID + " And وعده_غذایی = " + dataColumn.ColumnName.Replace("GridMealReservationFood_", "") + " And تاریخ_رزرو ='" + dataRow["تاریخ"].ToString() + "' And پرسنل = " + dataRow["شناسه"].ToString());
                                                    foreach (DataRow Row in PersonRow)
                                                    {
                                                        if (Row["غذا"].ToString() != dataRow[dataColumn].ToString() || Row["رستوران"].ToString() != Restaurant.ToString())
                                                        {
                                                            ColumnValue = new object[] { ParentRowID, dataRow["شناسه"].ToString(), dataColumn.ColumnName.Replace("GridMealReservationFood_", ""), Restaurant, dataRow[dataColumn].ToString(), dataRow["تاریخ"].ToString(), 1 };
                                                            Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(Row["شناسه"].ToString()), CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                                                            if (Alarm != "")
                                                                ErrorMessage += Alarm + "\n";
                                                            else
                                                            {
                                                                if (Referral.DBData.UpdateRow(long.Parse(Row["شناسه"].ToString()), PersonReservationTableCore.CoreObjectID, PersonReservationTableCore.FullName, CoulmnName, ColumnValue))
                                                                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, PersonReservationTableCore, long.Parse(Row["شناسه"].ToString()), CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            else if (dataColumn.ColumnName.IndexOf("تخت") > -1)
                                            {
                                                if (dataRow[dataColumn].ToString() == "true")
                                                {
                                                    //if (Array.IndexOf(PersonOfBed, long.Parse(dataRow["شناسه"].ToString())) == -1)
                                                    //{
                                                    //    string[] BedCoulmnName = new string[] { "رزرو_غذا", "پرسنل" };
                                                    //    object[] BedColumnValue = new object[] { ParentRowID, dataRow["شناسه"].ToString() };
                                                    //    Alarm = Desktop.CheckBeforRunQuery(BedTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, BedCoulmnName, BedColumnValue);
                                                    //    if (Alarm != "")
                                                    //        ErrorMessage += Alarm + "\n";
                                                    //    else
                                                    //    {
                                                    //        long RowID = Referral.DBData.Insert(BedTableCore.FullName, BedCoulmnName, BedColumnValue, BedTableCore.CoreObjectID);
                                                    //        Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, BedTableCore, RowID, BedCoulmnName, BedColumnValue, "", BedTableCore.CoreObjectID.ToString());
                                                    //    }
                                                    //    Array.Resize(ref PersonOfBed, PersonOfBed.Length + 1);
                                                    //    PersonOfBed[PersonOfBed.Length - 1] = long.Parse(dataRow["شناسه"].ToString());
                                                    //}

                                                }

                                            }
                                        }
                                    }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage += ex.Message + "\n";

                    }
                }
            }

            return Json(new { Message = ErrorMessage, RecordID = ParentRowID });

        }


        public async Task<JsonResult> SaveLetterReservationFood(string[] GridJSON, long RecordID = 0, string FromDate = "", string ToDate = "", int RequestType = 0, int OfficeDepartmentAccountNumber = 0, string DateLetter = "",
                                                    string NumberLetter = "", string SubjectLetter = "", long ForOffice = 0, string Message = "", int BedCount = 0, string SelectedDey = "",
                                                    string BedGridJSON = "", string SelectedMeal = "", int Restaurant = 0, string SelectedMealValue = ""
                                                    )
        {
            CoreObject RequestReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "درخواست_رزرو_غذا");
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            CoreObject BedTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
            CoreObject PersonCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پرسنل");

            DataTable BedData = BedGridJSON == "[]" ? new DataTable() : DataConvertor.JsonStringToDataTable(BedGridJSON);
            bool IsEqualDate = (FromDate == ToDate);
            string ErrorMessage = "";
            string Alarm = string.Empty;
            long ParentRowID = RecordID;
            string[] OldMeal = new string[0];
            string[] NewMeal = new string[0];
            string[] CoulmnName = new string[] { "از_تاریخ", "تا_تاریخ", "نوع_درخواست_رزرو_غذا", "شماره_حساب", "برای_اداره", "تاریخ_نامه", "موضوع_نامه", "شماره_نامه", "توضیحات", "وضعیت_رزرو_تخت", "تعداد_رزرو_تخت", "وعده_غذایی", "روزهای_هفته", "رستوران", "تعداد_هر_وعده" };
            object[] ColumnValue = new object[] { FromDate, ToDate, RequestType, OfficeDepartmentAccountNumber, ForOffice, DateLetter, SubjectLetter, NumberLetter, Message, BedCount > 0 ? true : false, BedCount, SelectedMeal, SelectedDey, Restaurant, SelectedMealValue };
            DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
            if (RecordID > 0)
            {
                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });

                if (Referral.DBData.UpdateRow(ParentRowID, RequestReservationTableCore.CoreObjectID, RequestReservationTableCore.FullName, CoulmnName, ColumnValue))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
                else
                    return Json(new { Message = "عملیات ویرایش با شکست مواجه شد", RecordID = 0 });

                if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != SelectedMeal && ReserveData.Rows[0]["وعده_غذایی"].ToString() != "")
                {
                    OldMeal = ReserveData.Rows[0]["وعده_غذایی"].ToString().Split(',');
                    NewMeal = SelectedMeal.Split(',');
                    foreach (string OldMealItem in OldMeal)
                        if (Array.IndexOf(NewMeal, OldMealItem) == -1)
                        {
                            await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND وعده_غذایی = " + OldMealItem);
                        }
                }
                else if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != SelectedMeal && SelectedMeal == "")
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID);
                }

                if (ReserveData.Rows[0]["رستوران"].ToString() != Restaurant.ToString())
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " and رستوران =" + ReserveData.Rows[0]["رستوران"].ToString());
                }

                if (string.Compare(ReserveData.Rows[0]["از_تاریخ"].ToString(), FromDate) == -1)
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو < N'" + FromDate + "'");
                }
                if (string.Compare(ReserveData.Rows[0]["تا_تاریخ"].ToString(), ToDate) == 1)
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو > N'" + ToDate + "'");
                }
            }
            else
            {
                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });
                ParentRowID = Referral.DBData.Insert(RequestReservationTableCore.FullName, CoulmnName, ColumnValue, RequestReservationTableCore.CoreObjectID);
                await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
            }

            if (GridJSON != null)
            {
                string FinalQuery = string.Empty;
                foreach (var Grid in GridJSON)
                {
                    if (Grid != "[]")
                    {
                        DataTable MealFoodData = DataConvertor.JsonStringToDataTable(Grid);
                        foreach (DataRow dataRow in MealFoodData.Rows)
                        {
                            if (dataRow["شناسه"].ToString() == "0" || dataRow["شناسه"].ToString() == "null")
                            {
                                if (dataRow["GridMealCountReservationFood"] != null)
                                    if (int.Parse(dataRow["GridMealCountReservationFood"].ToString()) > 0)
                                        FinalQuery += "Insert into نفرات_رزرو_غذا( درخواست_رزرو_غذا, پرسنل, وعده_غذایی, رستوران, غذا, تاریخ_رزرو, تعداد)values(" + ParentRowID + ",-2," + dataRow["GridMealReservationFood"] + "," + dataRow["GridRestaurantReservationFood"] + "," + dataRow["GridFoodReservationFood"] + ",N'" + dataRow["تاریخ"] + "'," + dataRow["GridMealCountReservationFood"] + ")\n";

                            }
                            else if (long.Parse(dataRow["شناسه"].ToString()) > 0)
                                if (dataRow["GridMealCountReservationFood"] != null)
                                    if (int.Parse(dataRow["GridMealCountReservationFood"].ToString()) > 0 && string.Compare(dataRow["تاریخ"].ToString(), CDateTime.GetNowshamsiDate()) >= 0)
                                        FinalQuery += "Update نفرات_رزرو_غذا set تعداد = " + dataRow["GridMealCountReservationFood"] + " Where شناسه = " + dataRow["شناسه"].ToString() + "\n";

                        }
                    }
                }
                await Referral.DBData.ExecuteAsync(FinalQuery);
            }

            string PersonName = string.Empty;
            foreach (DataRow dataRow in BedData.Rows)
            {
                string[] BedCoulmnName = new string[] { "رزرو_غذا", "پرسنل", "کد_ملی", "نام", "نام_خانوادگی", "تاریخ_تولد", "شماره_تماس", "آدرس" };

                string PersonnelID = dataRow["پرسنل"].ToString();
                if (PersonnelID == "")
                {
                    if (dataRow["شماره_ملی"].ToString() != "")
                        PersonnelID = (await Referral.DBData.SelectFieldAsync("Select شناسه from پرسنل where پرسنل.کد_ملی = N'" + dataRow["شماره_ملی"].ToString() + "'")).ToString();

                    if (PersonnelID == "" && dataRow["شماره_ملی"].ToString() != "")
                    {
                        string[] PersonCoulmnName = new string[] { "نام", "نام_خانوادگی", "کد_ملی", "تاریخ_تولد", "شماره_تماس", "آدرس" };
                        object[] PersonColumnValue = new object[] { dataRow["نام"], dataRow["نام_خانوادگی"], dataRow["شماره_ملی"], dataRow["تاریخ_تولد"], dataRow["شماره_تماس"], dataRow["آدرس"] };
                        PersonnelID = Referral.DBData.Insert("پرسنل", PersonCoulmnName, PersonColumnValue).ToString();
                        await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, PersonCore, long.Parse(PersonnelID), PersonCoulmnName, PersonColumnValue, "", PersonCore.CoreObjectID.ToString());
                    }
                    else
                        PersonnelID = "-2";
                }
                PersonName += dataRow["نام"] + " " + dataRow["نام_خانوادگی"] + ", ";
                object[] BedColumnValue = new object[] { ParentRowID, PersonnelID, dataRow["شماره_ملی"], dataRow["نام"], dataRow["نام_خانوادگی"], dataRow["تاریخ_تولد"], dataRow["شماره_تماس"], dataRow["آدرس"] };
                if (long.Parse(dataRow["شناسه"].ToString()) > 0)
                {
                    Alarm = Desktop.CheckBeforRunQuery(BedTableCore.CoreObjectID, long.Parse(dataRow["شناسه"].ToString()), CoreDefine.TableEvents.شرط_اجرای_ویرایش, BedCoulmnName, BedColumnValue);
                    if (Alarm != "")
                        ErrorMessage += Alarm + "\n";
                    else
                    {
                        if (Referral.DBData.UpdateRow(long.Parse(dataRow["شناسه"].ToString()), BedTableCore.CoreObjectID, BedTableCore.FullName, BedCoulmnName, BedColumnValue))
                            await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, BedTableCore, long.Parse(dataRow["شناسه"].ToString()), BedCoulmnName, BedColumnValue, "", BedTableCore.CoreObjectID.ToString());
                    }
                }
                else
                {
                    Alarm = Desktop.CheckBeforRunQuery(BedTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, BedCoulmnName, BedColumnValue);
                    if (Alarm != "")
                        ErrorMessage += Alarm + "\n";
                    else
                    {
                        long RowID = Referral.DBData.Insert(BedTableCore.FullName, BedCoulmnName, BedColumnValue, BedTableCore.CoreObjectID);
                        await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, BedTableCore, RowID, BedCoulmnName, BedColumnValue, "", BedTableCore.CoreObjectID.ToString());
                    }
                }
            }

            await Referral.DBData.ExecuteAsync("Update  درخواست_رزرو_غذا set نفرات = N'" + PersonName + "' where شناسه = " + ParentRowID);
            return Json(new { Message = ErrorMessage, RecordID = ParentRowID });

        }



        public async Task<JsonResult> SaveDelayedReservationFood(long RecordID = 0, string FromDate = "", string ToDate = "", int RequestType = 0, long Restaurant = 0, int OfficeDepartmentAccountNumber = 0, long ForOffice = 0, string Message = "", string GridJSON = "")
        {
            CoreObject RequestReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "درخواست_رزرو_غذا");
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            DataTable PersonData = DataConvertor.JsonStringToDataTable(GridJSON);
            bool IsEqualDate = (FromDate == ToDate);
            string ErrorMessage = "";
            RequestType = 4;
            string[] CoulmnName = new string[] { "از_تاریخ", "تا_تاریخ", "نوع_درخواست_رزرو_غذا", "شماره_حساب", "برای_اداره", "توضیحات", "رستوران" };
            object[] ColumnValue = new object[] { FromDate, ToDate, RequestType, OfficeDepartmentAccountNumber, ForOffice, Message, Restaurant };
            string Alarm = string.Empty;
            long ParentRowID = RecordID;
            DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
            if (RecordID > 0)
            {
                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });
                if (Referral.DBData.UpdateRow(RecordID, RequestReservationTableCore.CoreObjectID, RequestReservationTableCore.FullName, CoulmnName, ColumnValue))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
                else
                    return Json(new { Message = "عملیات بروز رسانی با شکست مواجه شد", RecordID = 0 });

                if (string.Compare(ReserveData.Rows[0]["از_تاریخ"].ToString(), FromDate) == -1)
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو < N'" + FromDate + "'");
                }
                if (string.Compare(ReserveData.Rows[0]["تا_تاریخ"].ToString(), ToDate) == 1)
                {
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو > N'" + ToDate + "'");
                }
            }
            else
            {
                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });
                ParentRowID = Referral.DBData.Insert(RequestReservationTableCore.FullName, CoulmnName, ColumnValue, RequestReservationTableCore.CoreObjectID);
                await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
            }


            if (ParentRowID > 0)
            {
                DataTable DayData = await Referral.DBData.SelectDataTableAsync("Select * from لیست_تاریخ where تاریخ_شمسی>=N'" + FromDate + "' and تاریخ_شمسی <=N'" + ToDate + "' and وضعیت_روز_کاری = 1 ");
                string Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
                Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + ToDate + "' \n";
                Query += "declare @مکان  as bigint = " + Restaurant + " \n";
                Query += "SELECT   *  FROM  برنامه_غذایی_مکان   WHERE(برنامه_غذایی_مکان.تاریخ >= @از_تاریخ AND برنامه_غذایی_مکان.تاریخ <= @تا_تاریخ)  AND برنامه_غذایی_مکان.مکان = @مکان  AND برنامه_غذایی_مکان.وضعیت_پیشفرض = 1  and وعده_غذایی = 2  ";

                DataTable MealProgram = await Referral.DBData.SelectDataTableAsync(Query);
                string DefualtFood = "";
                string FinalQuery = "";

                if (RecordID == 0)
                {
                    foreach (DataRow DayRow in DayData.Rows)
                    {
                        DataRow[] row = MealProgram.Select("تاریخ = '" + DayRow["تاریخ_شمسی"].ToString() + "'");
                        DefualtFood = row[0]["غذا"].ToString();
                        if (DefualtFood != "")
                        {
                            foreach (DataRow dataRow in PersonData.Rows)
                            {
                                FinalQuery += "Insert into نفرات_رزرو_غذا( درخواست_رزرو_غذا, پرسنل, وعده_غذایی, رستوران, غذا, تاریخ_رزرو, تعداد)values(" + ParentRowID + "," + dataRow["شناسه"].ToString() + ",2," + Restaurant + "," + DefualtFood + ",N'" + DayRow["تاریخ_شمسی"].ToString() + "',1)\n";
                            }

                        }
                    }
                    if (FinalQuery != "")
                        await Referral.DBData.ExecuteAsync(FinalQuery);
                }
                else
                {
                    foreach (DataRow dataRow1 in PersonData.Rows)
                    {
                        DataTable PersonReserveDate = await Referral.DBData.SelectDataTableAsync("Select تاریخ_رزرو from نفرات_رزرو_غذا where درخواست_رزرو_غذا = " + RecordID + " and پرسنل = " + dataRow1["شناسه"].ToString() + " group by تاریخ_رزرو");
                        foreach (DataRow DayRow in DayData.Rows)
                        {
                            if (PersonReserveDate.Select("تاریخ_رزرو = '" + DayRow["تاریخ_شمسی"].ToString() + "'").Length == 0)
                            {
                                DataRow[] row = MealProgram.Select("تاریخ = '" + DayRow["تاریخ_شمسی"].ToString() + "'");
                                DefualtFood = row[0]["غذا"].ToString();
                                if (DefualtFood != "")
                                {
                                    FinalQuery += "Insert into نفرات_رزرو_غذا( درخواست_رزرو_غذا, پرسنل, وعده_غذایی, رستوران, غذا, تاریخ_رزرو, تعداد)values(" + ParentRowID + "," + dataRow1["شناسه"].ToString() + ",2," + Restaurant + "," + DefualtFood + ",N'" + DayRow["تاریخ_شمسی"].ToString() + "',1)\n";

                                }
                            }
                        }
                    }

                    if (FinalQuery != "")
                        await Referral.DBData.ExecuteAsync(FinalQuery);
                }

            }

            return Json(new { Message = ErrorMessage, RecordID = ParentRowID });
        }

        public async Task<JsonResult> SaveShiftReservationFood(long RecordID = 0, string FromDate = "", string ToDate = "", int RequestType = 0, int ShiftChangeDay = 0, long Restaurant = 0, int OfficeDepartmentAccountNumber = 0, string Message = "", string GridJSON = "", string BedGridJSON = "", string SelectedMeal = "", string SearchOfReserveID = "")
        {
            CoreObject RequestReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "درخواست_رزرو_غذا");
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            CoreObject BedTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
            CoreObject ShiftMeal = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "وعده_های_رزرو_شده_نوبتکار");

            DataTable BedData = BedGridJSON == "[]" ? new DataTable() : DataConvertor.JsonStringToDataTable(BedGridJSON);
            DataTable MealData = DataConvertor.JsonStringToDataTable(GridJSON);
            string ErrorMessage = "";
            string ShiftChangeWeekDay = "";
            switch (ShiftChangeDay)
            {
                case 1: { ShiftChangeWeekDay = "شنبه"; break; }
                case 2: { ShiftChangeWeekDay = "یک شنبه"; break; }
                case 3: { ShiftChangeWeekDay = "دو شنبه"; break; }
                case 4: { ShiftChangeWeekDay = "سه شنبه"; break; }
                case 5: { ShiftChangeWeekDay = "چهار شنبه"; break; }
                case 6: { ShiftChangeWeekDay = "پنج شنبه"; break; }
                case 7: { ShiftChangeWeekDay = "جمعه"; break; }
            }

            string[] CoulmnName = new string[] { "از_تاریخ", "تا_تاریخ", "نوع_درخواست_رزرو_غذا", "شماره_حساب", "توضیحات", "روز_تعویض_شیفت", "وعده_غذایی", "رستوران", "شماره_مجوز_قبلی" };
            object[] ColumnValue = new object[] { FromDate, ToDate, RequestType, OfficeDepartmentAccountNumber, Message, ShiftChangeWeekDay, SelectedMeal, Restaurant, SearchOfReserveID };
            string Alarm = string.Empty;
            string PesrsonName = string.Empty;
            long ParentRowID = RecordID;
            string[] OldMeal = new string[0];
            string[] NewMeal = new string[0];

            if (ShiftChangeWeekDay == "" && OfficeDepartmentAccountNumber == 6)
                return Json(new { Message = "روز تعطیلی انتخاب نشده است", RecordID = ParentRowID });

            if (SearchOfReserveID != "")
            {
                await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا where نفرات_رزرو_غذا.درخواست_رزرو_غذا = " + SearchOfReserveID + " and نفرات_رزرو_غذا.تاریخ_رزرو>=N'" + ToDate + "' AND تاریخ_رزرو <= N'" + ToDate + "' ");
            }

            DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
            DataTable PersonData = await Referral.DBData.SelectDataTableAsync("Select پرسنل from نفرات_رزرو_غذا where تاریخ_رزرو >= N'" + FromDate + "'  AND تاریخ_رزرو <= N'" + ToDate + "'  And درخواست_رزرو_غذا =" + RecordID + " And پرسنل >0  group by پرسنل");
            string FinalQuery = "";

            //if (RecordID > 0 && string.Compare(FromDate, CDateTime.GetNowshamsiDate()) == -1)
            //    FromDate = CDateTime.GetNowshamsiDate();

            if (RecordID > 0)
            {
                if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != SelectedMeal && ReserveData.Rows[0]["وعده_غذایی"].ToString() != "")
                {
                    OldMeal = ReserveData.Rows[0]["وعده_غذایی"].ToString().Split(',');
                    NewMeal = SelectedMeal.Split(',');

                    foreach (string OldMealItem in OldMeal)
                        if (Array.IndexOf(NewMeal, OldMealItem) == -1 && OldMealItem != "")
                        {
                            await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND وعده_غذایی = " + OldMealItem + " AND تاریخ_رزرو >= N'" + FromDate + "'");
                            await Referral.DBData.ExecuteAsync("Delete وعده_های_رزرو_شده_نوبتکار Where  درخواست_رزرو_غذا=" + RecordID + " AND وعده_غذایی = " + OldMealItem);
                        }
                }
                else if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != SelectedMeal && SelectedMeal == "")
                    await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو >= N'" + FromDate + "'");

                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });

                if (Referral.DBData.UpdateRow(ParentRowID, RequestReservationTableCore.CoreObjectID, RequestReservationTableCore.FullName, CoulmnName, ColumnValue))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
                else
                    return Json(new { Message = "عملیات ویرایش با شکست مواجه شد", RecordID = 0 });


                //if (string.Compare(ReserveData.Rows[0]["از_تاریخ"].ToString(), FromDate) == -1) 
                await Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو >= N'" + FromDate + "' AND تاریخ_رزرو <= N'" + ToDate + "'");
                //if (string.Compare(ReserveData.Rows[0]["تا_تاریخ"].ToString(), ToDate) == 1) 
                //    Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا Where  درخواست_رزرو_غذا=" + RecordID + " AND تاریخ_رزرو > N'" + ToDate + "'");

                foreach (DataRow dataRow in PersonData.Rows)
                {
                    bool IsFinde = false;

                    foreach (DataRow BedDataRow in BedData.Rows)
                        if (BedDataRow["شناسه"].ToString() == dataRow["پرسنل"].ToString())
                            IsFinde = true;

                    if (!IsFinde)
                        FinalQuery += "\nDelete نفرات_رزرو_غذا where تاریخ_رزرو >= N'" + FromDate + "' and تاریخ_رزرو <=N'" + ToDate + "'   And درخواست_رزرو_غذا =" + RecordID + " And پرسنل = " + dataRow["پرسنل"].ToString() + "\n";
                }
            }
            else
            {
                Alarm = Desktop.CheckBeforRunQuery(RequestReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                if (Alarm != "")
                    return Json(new { Message = Alarm, RecordID = 0 });
                ParentRowID = Referral.DBData.Insert(RequestReservationTableCore.FullName, CoulmnName, ColumnValue, RequestReservationTableCore.CoreObjectID);
                await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, RequestReservationTableCore, ParentRowID, CoulmnName, ColumnValue, "", RequestReservationTableCore.CoreObjectID.ToString());
            }


            DataTable AdminPersonData = await Referral.DBData.SelectDataTableAsync("Select * from نفرات_رزرو_غذا where تاریخ_رزرو >= N'" + FromDate + "'  and تاریخ_رزرو <=N'" + ToDate + "'   And درخواست_رزرو_غذا =" + RecordID + " And پرسنل = -2  ");
            DataTable DayData = await Referral.DBData.SelectDataTableAsync("Select * from لیست_تاریخ where تاریخ_شمسی>=N'" + FromDate + "' and تاریخ_شمسی <=N'" + ToDate + "'");
            string Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + FromDate + "' \n";
            Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + ToDate + "' \n";
            Query += "declare @مکان  as bigint = " + Restaurant + " \n";
            Query += "SELECT   *  FROM  برنامه_غذایی_مکان   WHERE(برنامه_غذایی_مکان.تاریخ >= @از_تاریخ AND برنامه_غذایی_مکان.تاریخ <= @تا_تاریخ)  AND برنامه_غذایی_مکان.مکان = @مکان  AND برنامه_غذایی_مکان.وضعیت_پیشفرض = 1 ";

            DataTable MealProgram = await Referral.DBData.SelectDataTableAsync(Query);
            string DefualtFood = "";

            foreach (DataRow DayRow in DayData.Rows)
            {
                foreach (DataRow MealRow in MealData.Rows)
                {
                    int FoodCount = 0;

                    if (DayRow["روز_هفته"].ToString() == "پنج شنبه")
                        FoodCount = int.Parse(MealRow["ThursdayReservationFood"].ToString());
                    else if (DayRow["روز_هفته"].ToString() == "جمعه" || DayRow["تعطیل"].ToString() == "True")
                        FoodCount = int.Parse(MealRow["FridayReservationFood"].ToString());
                    else
                        FoodCount = int.Parse(MealRow["SaturdayToWednesdayReservationFood"].ToString());

                    if (FoodCount > 0)
                    {
                        if (ShiftChangeWeekDay == DayRow["روز_هفته"].ToString())
                        {
                            if (DayRow["تعطیل"].ToString() == "True")
                                FoodCount = int.Parse(MealRow["ChangeShiftFridayReservationFood"].ToString());
                            else
                                FoodCount = int.Parse(MealRow["ChangeShiftSaturdayToWednesdayReservationFood"].ToString());
                        }
                        DataRow[] row = MealProgram.Select("تاریخ = '" + DayRow["تاریخ_شمسی"].ToString() + "' and وعده_غذایی = " + MealRow["GridMealReservationFood"]);
                        if (row.Length > 0)
                        {
                            DefualtFood = row[0]["غذا"].ToString();
                            if (DefualtFood != "")
                            {
                                if (RecordID > 0 && MealRow["SaturdayToWednesdayReservationFood"].ToString() != "0")
                                {

                                    DataRow[] AdminRow = AdminPersonData.Select("تاریخ_رزرو = '" + DayRow["تاریخ_شمسی"].ToString() + "' and وعده_غذایی = " + MealRow["GridMealReservationFood"]);
                                    if (AdminRow.Length == 0)
                                        FinalQuery += "Insert into نفرات_رزرو_غذا( درخواست_رزرو_غذا, پرسنل, وعده_غذایی, رستوران, غذا, تاریخ_رزرو, تعداد)values(" + ParentRowID + ",-2," + MealRow["GridMealReservationFood"] + "," + Restaurant + "," + DefualtFood + ",N'" + DayRow["تاریخ_شمسی"].ToString() + "'," + FoodCount + ")\n";
                                    else
                                    {
                                        FinalQuery += "Update نفرات_رزرو_غذا set تعداد = " + FoodCount + " Where تاریخ_رزرو =N'" + DayRow["تاریخ_شمسی"].ToString() + "'   And درخواست_رزرو_غذا =" + RecordID + " And پرسنل = -2 And  وعده_غذایی =" + MealRow["GridMealReservationFood"] + "\n";
                                        FinalQuery += "Update نفرات_رزرو_غذا set تعداد = 0 Where تاریخ_رزرو =N'" + DayRow["تاریخ_شمسی"].ToString() + "'  And درخواست_رزرو_غذا =" + RecordID + " And پرسنل > -2 And  وعده_غذایی =" + MealRow["GridMealReservationFood"] + "\n";
                                    }
                                }
                                else
                                    FinalQuery += "Insert into نفرات_رزرو_غذا( درخواست_رزرو_غذا, پرسنل, وعده_غذایی, رستوران, غذا, تاریخ_رزرو, تعداد)values(" + ParentRowID + ",-2," + MealRow["GridMealReservationFood"] + "," + Restaurant + "," + DefualtFood + ",N'" + DayRow["تاریخ_شمسی"].ToString() + "'," + FoodCount + ")\n";

                                foreach (DataRow dataRow in BedData.Rows)
                                {
                                    DataRow[] PersonRow = PersonData.Select("پرسنل =  " + dataRow["شناسه"].ToString());
                                    if (PersonRow.Length == 0)
                                        FinalQuery += "\n  Insert into نفرات_رزرو_غذا( درخواست_رزرو_غذا, پرسنل, وعده_غذایی, رستوران, غذا, تاریخ_رزرو, تعداد)values(" + ParentRowID + "," + dataRow["شناسه"] + "," + MealRow["GridMealReservationFood"] + "," + Restaurant + "," + DefualtFood + ",N'" + DayRow["تاریخ_شمسی"].ToString() + "',0)\n";
                                }
                            }
                        }
                    }
                }
            }

            if (FinalQuery != "")
            {
                await Referral.DBData.ExecuteAsync(FinalQuery);
            }


            if (RecordID == 0)
            {
                foreach (DataRow MealRow in MealData.Rows)
                {
                    if (RequestType == 5)
                    {
                        CoulmnName = new string[] { "درخواست_رزرو_غذا", "وعده_غذایی", "تعداد_غذا_روزهای_کاری", "تعداد_غذا_روز_پنجشنبه", "تعداد_غذا_روزهای_جمعه_و_تعطیل" };
                        ColumnValue = new object[] { ParentRowID, MealRow["GridMealReservationFood"], MealRow["SaturdayToWednesdayReservationFood"], MealRow["ThursdayReservationFood"], MealRow["FridayReservationFood"] };
                    }
                    else
                    {
                        CoulmnName = new string[] { "درخواست_رزرو_غذا", "وعده_غذایی", "تعداد_غذا_روزهای_کاری", "تعداد_غذا_روز_پنجشنبه", "تعداد_غذا_روزهای_جمعه_و_تعطیل", "تعداد_غذا_روزهای_کاری_تعویض_شیفت", "تعداد_غذا_جمعه_و_روز_های_تعطیل_تعویض_شیفت" };
                        ColumnValue = new object[] { ParentRowID, MealRow["GridMealReservationFood"], MealRow["SaturdayToWednesdayReservationFood"], MealRow["ThursdayReservationFood"], MealRow["FridayReservationFood"], MealRow["ChangeShiftSaturdayToWednesdayReservationFood"], MealRow["ChangeShiftFridayReservationFood"] };
                    }
                    Alarm = Desktop.CheckBeforRunQuery(ShiftMeal.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                    if (Alarm != "")
                        return Json(new { Message = Alarm, RecordID = 0 });

                    for (int Index = 0; Index < ColumnValue.Length; Index++)
                        if (ColumnValue[Index].ToString() == "null")
                            ColumnValue[Index] = 0;
                    long RowID = Referral.DBData.Insert(ShiftMeal.FullName, CoulmnName, ColumnValue, ShiftMeal.CoreObjectID);
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, ShiftMeal, RowID, CoulmnName, ColumnValue, "", ShiftMeal.CoreObjectID.ToString());

                }

            }
            else
            {

                foreach (DataRow MealRow in MealData.Rows)
                {

                    if (RequestType == 5)
                    {
                        CoulmnName = new string[] { "درخواست_رزرو_غذا", "وعده_غذایی", "تعداد_غذا_روزهای_کاری", "تعداد_غذا_روز_پنجشنبه", "تعداد_غذا_روزهای_جمعه_و_تعطیل" };
                        ColumnValue = new object[] { ParentRowID, MealRow["GridMealReservationFood"], MealRow["SaturdayToWednesdayReservationFood"], MealRow["ThursdayReservationFood"], MealRow["FridayReservationFood"] };
                    }
                    else
                    {
                        CoulmnName = new string[] { "درخواست_رزرو_غذا", "وعده_غذایی", "تعداد_غذا_روزهای_کاری", "تعداد_غذا_روز_پنجشنبه", "تعداد_غذا_روزهای_جمعه_و_تعطیل", "تعداد_غذا_روزهای_کاری_تعویض_شیفت", "تعداد_غذا_جمعه_و_روز_های_تعطیل_تعویض_شیفت" };
                        ColumnValue = new object[] { ParentRowID, MealRow["GridMealReservationFood"], MealRow["SaturdayToWednesdayReservationFood"], MealRow["ThursdayReservationFood"], MealRow["FridayReservationFood"], MealRow["ChangeShiftSaturdayToWednesdayReservationFood"], MealRow["ChangeShiftFridayReservationFood"] };
                    }
                    if (long.Parse(MealRow["شناسه"].ToString()) > 0)
                    {
                        Alarm = Desktop.CheckBeforRunQuery(ShiftMeal.CoreObjectID, long.Parse(MealRow["شناسه"].ToString()), CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                        if (Alarm != "")
                            return Json(new { Message = Alarm, RecordID = 0 });

                        for (int Index = 0; Index < ColumnValue.Length; Index++)
                            if (ColumnValue[Index].ToString() == "null")
                                ColumnValue[Index] = 0;
                        if (Referral.DBData.UpdateRow(long.Parse(MealRow["شناسه"].ToString()), ShiftMeal.CoreObjectID, ShiftMeal.FullName, CoulmnName, ColumnValue))
                            await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, ShiftMeal, long.Parse(MealRow["شناسه"].ToString()), CoulmnName, ColumnValue, "", ShiftMeal.CoreObjectID.ToString());
                    }
                    else
                    {
                        Alarm = Desktop.CheckBeforRunQuery(ShiftMeal.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                        if (Alarm != "")
                            return Json(new { Message = Alarm, RecordID = 0 });

                        for (int Index = 0; Index < ColumnValue.Length; Index++)
                            if (ColumnValue[Index].ToString() == "null")
                                ColumnValue[Index] = 0;
                        long RowID = Referral.DBData.Insert(ShiftMeal.FullName, CoulmnName, ColumnValue, ShiftMeal.CoreObjectID);
                        await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, ShiftMeal, RowID, CoulmnName, ColumnValue, "", ShiftMeal.CoreObjectID.ToString());

                    }

                }

            }

            foreach (DataRow dataRow in BedData.Rows)
            {
                try
                {
                    if (BedData.Columns.IndexOf("تخت") > -1)
                        if (dataRow["تخت"].ToString() == "true")
                        {
                            if (long.Parse(dataRow["شناسه_درخواست_رزرو_غذا"].ToString()) == 0)
                            {
                                string[] BedCoulmnName = new string[] { "رزرو_غذا", "پرسنل" };
                                object[] BedColumnValue = new object[] { ParentRowID, dataRow["شناسه"] };
                                Alarm = Desktop.CheckBeforRunQuery(BedTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, BedCoulmnName, BedColumnValue);
                                if (Alarm != "")
                                    ErrorMessage += Alarm + "\n";
                                else
                                {
                                    long RowID = Referral.DBData.Insert(BedTableCore.FullName, BedCoulmnName, BedColumnValue, BedTableCore.CoreObjectID);
                                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, BedTableCore, RowID, BedCoulmnName, BedColumnValue, "", BedTableCore.CoreObjectID.ToString());
                                }
                            }
                            else
                            {
                                await Referral.DBData.ExecuteAsync("Update پذیرش_مهمانسرا set تا_تاریخ = N'" + ToDate + "' where رزرو_غذا =" + RecordID + " and پرسنل =" + dataRow["شناسه"]);
                            }
                        }

                    PesrsonName += dataRow["نام"].ToString() + ",";
                }
                catch (Exception)
                {

                }

            }
            await Referral.DBData.ExecuteAsync("Update درخواست_رزرو_غذا set نفرات =N'" + PesrsonName + "' where شناسه = " + RecordID);
            return Json(new { Message = ErrorMessage, RecordID = ParentRowID });
        }

        public async Task<ActionResult> ReadMealLetter([DataSourceRequest] DataSourceRequest _Request, long RecordID = 0, string Date = "", long Restaurant = 0, string Meal = "", bool IsSharingFood = true, string DefualtFoodCount = "")
        {
            JsonResult jsonResult = new JsonResult();
            DataTable dataTable = new DataTable();
            string Query = String.Empty;
            if (RecordID > 0)
            {
                DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
                string[] MealId = new string[0];
                int[] DefualtFoodCounter = new int[0];
                string[] OldMealId = new string[0];
                int[] OldDefualtFoodCounter = new int[0];
                if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != Meal || ReserveData.Rows[0]["رستوران"].ToString() != Restaurant.ToString() || string.Compare(ReserveData.Rows[0]["تا_تاریخ"].ToString(), Date) == -1 || string.Compare(ReserveData.Rows[0]["از_تاریخ"].ToString(), Date) == 1 || ReserveData.Rows[0]["تعداد_هر_وعده"].ToString() != DefualtFoodCount)
                {
                    string[] OldMeal = ReserveData.Rows[0]["وعده_غذایی"].ToString().Split(',');
                    string[] NewMeal = Meal.Split(',');
                    int[] DefualtFoodCountArr = Array.ConvertAll(DefualtFoodCount.Split(','), Item => int.Parse(Item));
                    int[] OldDefualtFoodCountArr = ReserveData.Rows[0]["تعداد_هر_وعده"].ToString() == "" ? new int[0] : Array.ConvertAll(ReserveData.Rows[0]["تعداد_هر_وعده"].ToString().Split(','), Item => int.Parse(Item));
                    for (int Index = 0; Index < NewMeal.Length; Index++)
                    {
                        if (Array.IndexOf(OldMeal, NewMeal[Index]) == -1 || ReserveData.Rows[0]["رستوران"].ToString() != Restaurant.ToString() || string.Compare(ReserveData.Rows[0]["تا_تاریخ"].ToString(), Date) == -1 || string.Compare(ReserveData.Rows[0]["از_تاریخ"].ToString(), Date) == 1)
                        {
                            Array.Resize(ref MealId, MealId.Length + 1);
                            Array.Resize(ref DefualtFoodCounter, DefualtFoodCounter.Length + 1);
                            MealId[MealId.Length - 1] = NewMeal[Index];
                            DefualtFoodCounter[DefualtFoodCounter.Length - 1] = DefualtFoodCountArr[Index];
                        }
                        else if (Array.IndexOf(OldMeal, NewMeal[Index]) > -1)
                            if (OldDefualtFoodCountArr[Array.IndexOf(OldMeal, NewMeal[Index])] != DefualtFoodCountArr[Index])
                            {
                                Array.Resize(ref OldMealId, OldMealId.Length + 1);
                                Array.Resize(ref OldDefualtFoodCounter, OldDefualtFoodCounter.Length + 1);
                                OldMealId[OldMealId.Length - 1] = NewMeal[Index];
                                OldDefualtFoodCounter[OldDefualtFoodCounter.Length - 1] = DefualtFoodCountArr[Index];

                            }
                    }
                }

                Query = "Declare @درخواست_رزرو_غذا as bigint = " + RecordID + "\n" +
                "Declare @تاریخ_رزرو as Nvarchar(255)=N'" + Date + "'\n" +
                "Declare @رستوران as bigint = " + Restaurant + "\n" +
                "\ndeclare @غذا  as bigint =0 " +
                "\ndeclare @Tbl as table (شناسه bigint, ردیف int , تاریخ nvarchar(400),GridMealReservationFood int , GridRestaurantReservationFood int , GridFoodReservationFood int  , GridMealCountReservationFood int , اولویت int)" +
                "\ninsert into @Tbl";

                Query += "\nSELECT  شناسه" +
                    "\n,ROW_NUMBER() over(ORDER BY شناسه asc) as ردیف" +
                    "\n,تاریخ_رزرو as تاریخ" +
                    "\n,وعده_غذایی as GridMealReservationFood" +
                    "\n,رستوران as GridRestaurantReservationFood" +
                    "\n,غذا as GridFoodReservationFood" +
                    "\n,تعداد as GridMealCountReservationFood " +
                    "\n, 0 as اولویت" +
                    "\n  FROM NisocWelfareServiceData.dbo.نفرات_رزرو_غذا" +
                    "\nwhere درخواست_رزرو_غذا =  @درخواست_رزرو_غذا And تاریخ_رزرو =@تاریخ_رزرو";
                if (string.Compare(Date, CDateTime.GetNowshamsiDate()) > -1)
                    Query += "\nand وعده_غذایی in (SELECT value FROM STRING_SPLIT('" + Meal + "', ','))";
                Query += "\nand رستوران = @رستوران";

                if (MealId.Length > 0 && string.Compare(Date, CDateTime.GetNowshamsiDate()) > -1)
                {
                    Query += "\nunion" +
                        "\nSELECT 0," +
                        "\nROW_NUMBER() OVER(ORDER BY برنامه_غذایی_مکان.وعده_غذایی) as ردیف" +
                        "\n,@تاریخ_رزرو as تاریخ" +
                        "\n,برنامه_غذایی_مکان.وعده_غذایی as GridMealReservationFood" +
                        "\n,[برنامه_غذایی_مکان].مکان as GridRestaurantReservationFood" +
                        "\n,غذا.[شناسه] as GridFoodReservationFood" +
                        "\n,0 as GridMealCountReservationFood " +
                        "\n,برنامه_غذایی_مکان.اولویت" +
                        "\nFROM غذا inner join[برنامه_غذایی_مکان] on غذا.شناسه = برنامه_غذایی_مکان.غذا " +
                        "\ninner join[کاربر_وعده_غذایی] on[برنامه_غذایی_مکان].وعده_غذایی = [کاربر_وعده_غذایی].وعده_غذایی\n" +
                        "\nWHERE[کاربر_وعده_غذایی].کاربر = @شناسه_کاربر and[برنامه_غذایی_مکان].تاریخ >= @تاریخ_رزرو and[برنامه_غذایی_مکان].تاریخ <= @تاریخ_رزرو\n" +
                        "\nand برنامه_غذایی_مکان.وعده_غذایی in (" + string.Join(",", MealId) + ")\n" +
                        "\nand[برنامه_غذایی_مکان].مکان = @رستوران\n" +
                        "\nand(غذا.شناسه = @غذا or @غذا = 0)\n";

                    if (!IsSharingFood)
                        Query += "\nand وضعیت_پیشفرض=1 \n ";

                    Query += "\ngroup by غذا.[شناسه]\n" +
                           ",برنامه_غذایی_مکان.وعده_غذایی ,[برنامه_غذایی_مکان].مکان, برنامه_غذایی_مکان.اولویت\n";
                }
                Query += "\nselect * from @Tbl order by GridMealReservationFood , اولویت";

                dataTable = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query));
                if (MealId.Length > 0 && string.Compare(Date, CDateTime.GetNowshamsiDate()) > -1)
                {
                    for (int Index = 0; Index < MealId.Length; Index++)
                    {
                        if (IsSharingFood)
                        {
                            int MealFound = dataTable.Select("GridMealReservationFood=" + MealId[Index]).Length;
                            if (MealFound > 0)
                            {
                                int FoodCount = MealFound > 1 ? (DefualtFoodCounter[Index] / 2) : DefualtFoodCounter[Index];
                                int Temp = FoodCount;
                                foreach (DataRow Row in dataTable.Rows)
                                {
                                    if (Row["GridMealReservationFood"].ToString() == MealId[Index])
                                    {
                                        Row["GridMealCountReservationFood"] = FoodCount;
                                        FoodCount = DefualtFoodCounter[Index] - Temp;
                                        Temp += FoodCount;
                                    }
                                }
                            }

                        }
                        else
                        {
                            int MealFound = dataTable.Select("GridMealReservationFood=" + MealId[Index]).Length;
                            if (MealFound > 0)
                            {
                                foreach (DataRow Row in dataTable.Rows)
                                {
                                    if (Row["GridMealReservationFood"].ToString() == MealId[Index])
                                    {
                                        Row["GridMealCountReservationFood"] = DefualtFoodCounter[Index];
                                    }
                                }
                            }
                        }

                    }
                }
                if (OldMealId.Length > 0 && string.Compare(Date, CDateTime.GetNowshamsiDate()) > -1)
                {

                    for (int Index = 0; Index < OldMealId.Length; Index++)
                    {
                        if (IsSharingFood)
                        {
                            int MealFound = dataTable.Select("GridMealReservationFood=" + OldMealId[Index]).Length;
                            if (MealFound > 0)
                            {
                                int FoodCount = MealFound > 1 ? (OldDefualtFoodCounter[Index] / 2) : OldDefualtFoodCounter[Index];
                                int Temp = FoodCount;
                                foreach (DataRow Row in dataTable.Rows)
                                {
                                    if (Row["GridMealReservationFood"].ToString() == OldMealId[Index])
                                    {
                                        Row["GridMealCountReservationFood"] = FoodCount;
                                        FoodCount = OldDefualtFoodCounter[Index] - Temp;
                                        Temp += FoodCount;
                                    }
                                }
                            }

                        }
                        else
                        {
                            int MealFound = dataTable.Select("GridMealReservationFood=" + OldMealId[Index]).Length;
                            if (MealFound > 0)
                            {
                                foreach (DataRow Row in dataTable.Rows)
                                {
                                    if (Row["GridMealReservationFood"].ToString() == OldMealId[Index])
                                    {
                                        Row["GridMealCountReservationFood"] = OldDefualtFoodCounter[Index];
                                    }
                                }
                            }
                        }

                    }
                }

            }
            else
            {
                //if (Session["MealLetter" + Date.Replace("/", "") + Restaurant + Meal] == null)
                //{
                Query = "Declare @از_تاریخ as Nvarchar(255) = N'" + Date + "' \n";
                Query += "Declare @تا_تاریخ as Nvarchar(255) = N'" + Date + "' \n";
                Query += "declare @وعده_غذایی  as bigint = 0 \n";
                Query += "declare @مکان  as bigint = " + Restaurant + " \n";
                Query += "declare @غذا  as bigint =0 \n";
                Query += "SELECT \n" +
                    "ROW_NUMBER() OVER(ORDER BY برنامه_غذایی_مکان.وعده_غذایی) as ردیف \n" +
                    ",@از_تاریخ as تاریخ \n" +
                    ",برنامه_غذایی_مکان.وعده_غذایی as GridMealReservationFood\n" +
                    ",0 as GridMealCountReservationFood\n" +
                    ",[برنامه_غذایی_مکان].مکان as GridRestaurantReservationFood\n" +
                    ",غذا.[شناسه] as GridFoodReservationFood\n" +
                    ", برنامه_غذایی_مکان.اولویت\n" +
                    "FROM غذا inner join[برنامه_غذایی_مکان] on غذا.شناسه = برنامه_غذایی_مکان.غذا " +
                    "inner join[کاربر_وعده_غذایی] on[برنامه_غذایی_مکان].وعده_غذایی = [کاربر_وعده_غذایی].وعده_غذایی\n" +
                    "WHERE[کاربر_وعده_غذایی].کاربر = @شناسه_کاربر and[برنامه_غذایی_مکان].تاریخ >= @از_تاریخ and[برنامه_غذایی_مکان].تاریخ <= @تا_تاریخ\n" +
                    "and برنامه_غذایی_مکان.وعده_غذایی in (" + Meal + ")\n" +
                    "and[برنامه_غذایی_مکان].مکان = @مکان\n" +
                    "and(غذا.شناسه = @غذا or @غذا = 0)\n";

                if (!IsSharingFood)
                    Query += "and وضعیت_پیشفرض=1 \n ";

                Query += " group by غذا.[شناسه]\n" +
                       ",برنامه_غذایی_مکان.وعده_غذایی ,[برنامه_غذایی_مکان].مکان, برنامه_غذایی_مکان.اولویت\n" +
                       "order by برنامه_غذایی_مکان.وعده_غذایی , برنامه_غذایی_مکان.اولویت";


                Session["MealLetter" + Date.Replace("/", "") + Restaurant + Meal] = Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query));
                //}

                int[] DefualtFoodCountArr = Array.ConvertAll(DefualtFoodCount.Split(','), Item => int.Parse(Item));
                string[] MealArr = Meal.Split(',');
                //dataTable = (DataTable)Session["MealLetter" + Date.Replace("/", "") + Restaurant + Meal];
                dataTable = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query));
                if (IsSharingFood)
                {
                    for (int Index = 0; Index < MealArr.Length; Index++)
                    {
                        int MealFound = dataTable.Select("GridMealReservationFood=" + MealArr[Index]).Length;
                        if (MealFound > 0)
                        {
                            int FoodCount = MealFound > 1 ? (DefualtFoodCountArr[Index] / 2) : DefualtFoodCountArr[Index];
                            int Temp = FoodCount;
                            foreach (DataRow Row in dataTable.Rows)
                            {
                                if (Row["GridMealReservationFood"].ToString() == MealArr[Index])
                                {
                                    Row["GridMealCountReservationFood"] = FoodCount;
                                    FoodCount = DefualtFoodCountArr[Index] - Temp;
                                    Temp += FoodCount;
                                }
                            }
                        }
                    }

                }
                else
                {

                    for (int Index = 0; Index < MealArr.Length; Index++)
                    {
                        int MealFound = dataTable.Select("GridMealReservationFood=" + MealArr[Index]).Length;
                        if (MealFound > 0)
                        {
                            foreach (DataRow Row in dataTable.Rows)
                            {
                                if (Row["GridMealReservationFood"].ToString() == MealArr[Index])
                                {
                                    Row["GridMealCountReservationFood"] = DefualtFoodCountArr[Index];
                                }
                            }
                        }
                    }
                }

            }

            jsonResult = Json(dataTable.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<JsonResult> UpdateLetterMealFood(long RecordID, int CountReservationFood)
        {
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            string[] CoulmnName = new string[] { "تعداد" };
            object[] ColumnValue = new object[] { CountReservationFood };
            string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
            if (Alarm != "")
                return Json(Alarm);
            else
            {
                if (Referral.DBData.UpdateRow(RecordID, PersonReservationTableCore.CoreObjectID, PersonReservationTableCore.FullName, CoulmnName, ColumnValue))
                {
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, PersonReservationTableCore, RecordID, CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                    return Json("");
                }
                return Json("ذخیره سازی با شکست مواجه شد");
            }
        }
        public async Task<JsonResult> RemoveLetterMealFood(long RecordID)
        {
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
            if (Alarm != "")
                return Json(Alarm);
            else
            {
                if (Referral.DBData.Delete(PersonReservationTableCore.FullName, RecordID))
                {
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, RecordID, new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
                    return Json("");
                }
                return Json("ذخیره سازی با شکست مواجه شد");
            }
        }
        public async Task<JsonResult> RemoveLetterMealFoodWithDate(long RecordID, string Date)
        {
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            DataTable MealReserveData = await Referral.DBData.SelectDataTableAsync("Select شناسه From نفرات_رزرو_غذا Where درخواست_رزرو_غذا = " + RecordID + " And تاریخ_رزرو = N'" + Date + "'");
            foreach (DataRow Row in MealReserveData.Rows)
            {
                string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(Row["شناسه"].ToString()), CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
                if (Alarm != "")
                    return Json(Alarm);
                else
                {
                    if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(Row["شناسه"].ToString())))
                        await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(Row["شناسه"].ToString()), new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
                    else
                        return Json("ذخیره سازی با شکست مواجه شد");
                }
            }

            return Json("");
        }
        public ActionResult ReadPerson(string RequestType, bool IsReload, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = new JsonResult();
            if (Session[RequestType] == null || IsReload)
            {
                string specialPhraseName = "لیست_پرسنل_درخواست_غذا_از_نوع_نوبتکار";
                switch (RequestType)
                {
                    case "Office":
                        {
                            specialPhraseName = "لیست_پرسنل_درخواست_غذا_از_نوع_اداری";
                            break;
                        }
                    case "Shift":
                        {
                            specialPhraseName = "لیست_پرسنل_درخواست_غذا_از_نوع_نوبتکار";
                            break;
                        }
                    case "Delayed":
                        {
                            specialPhraseName = "لیست_پرسنل_درخواست_غذا_از_نوع_روزکار";
                            break;
                        }
                }
                string Query = "Declare @نوع_دسترسی_کاربر as bigint =(SELECT  کاربر.نوع_دسترسی_کاربر FROM کاربر WHERE کاربر.شناسه = @شناسه_کاربر)  \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, specialPhraseName);
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                Session[RequestType] = Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
            }
            jsonResult = Json(((DataTable)Session[RequestType]).ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<ActionResult> ReadMealShift(long RecordID, string Meals, string SearchOfReserveID, [DataSourceRequest] DataSourceRequest _Request)
        {
            JsonResult jsonResult = new JsonResult();
            DataTable MealData = new DataTable();

            if (RecordID == 0 && SearchOfReserveID == "")
            {
                string[] MealArr = Meals.Split('_');
                MealData.Columns.Add("شناسه", typeof(long));
                MealData.Columns.Add("GridMealReservationFood", typeof(int));
                MealData.Columns.Add("SaturdayToWednesdayReservationFood", typeof(int));
                MealData.Columns.Add("ThursdayReservationFood", typeof(int));
                MealData.Columns.Add("FridayReservationFood", typeof(int));
                MealData.Columns.Add("ChangeShiftSaturdayToWednesdayReservationFood", typeof(int));
                MealData.Columns.Add("ChangeShiftFridayReservationFood", typeof(int));
                foreach (var Meal in MealArr)
                {
                    MealData.Rows.Add(new object[] { 0, Meal, 0, 0, 0, 0, 0 });
                }
            }
            else if (RecordID == 0 && SearchOfReserveID != "")
            {
                string Query = "DECLARE @درخواست_رزرو_غذا  as Bigint = " + SearchOfReserveID + "\n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "خواندن_وعده_های_غذایی_نوبتکار");
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                MealData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
                string[] MealArr = Meals.Split('_');
                List<string> FindMealList = new List<string>();
                foreach (DataRow Row in MealData.Rows)
                {
                    Row["شناسه"] = 0;
                    if (FindMealList.FindIndex(x => x == Row["GridMealReservationFood"].ToString()) == -1)
                        FindMealList.Add(Row["GridMealReservationFood"].ToString());
                }

                foreach (string MealItem in MealArr)
                {
                    if (FindMealList.FindIndex(x => x == MealItem) == -1)
                        MealData.Rows.Add(new object[] { 0, MealItem, 0, 0, 0, 0, 0 });
                }
            }
            else
            {
                string Query = "DECLARE @درخواست_رزرو_غذا  as Bigint = " + RecordID.ToString() + "\n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "خواندن_وعده_های_غذایی_نوبتکار");
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                MealData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
                string[] MealArr = Meals.Split('_');
                List<string> FindMealList = new List<string>();
                List<string> FindReserveIDList = new List<string>();

                foreach (DataRow Row in MealData.Rows)
                {
                    if (FindMealList.FindIndex(x => x == Row["GridMealReservationFood"].ToString()) == -1)
                    {
                        FindMealList.Add(Row["GridMealReservationFood"].ToString());
                        FindReserveIDList.Add(Row["شناسه"].ToString());
                    }
                }

                foreach (string MealItem in MealArr)
                {
                    if (FindMealList.FindIndex(x => x == MealItem) == -1)
                        MealData.Rows.Add(new object[] { 0, MealItem, 0, 0, 0, 0, 0 });
                }

                foreach (string MealItem in FindMealList)
                {
                    if (Array.IndexOf(MealArr, MealItem) == -1)
                        foreach (DataRow Row in MealData.Select("شناسه = " + FindReserveIDList[FindMealList.FindIndex(x => x == MealItem)]))
                            MealData.Rows.Remove(Row);
                }
            }
            jsonResult = Json(MealData.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult ShiftMealGridChange(long ParentID, string FromDate, string ToDate, long RecordID, int Meal, string DayTitle, int DayValue, int ShiftChangeDay = 0)
        {
            string Alarm = string.Empty;
            //if (string.Compare(ToDate, CDateTime.GetNowshamsiDate()) > 0)
            //{
            //    string FinalQuery = string.Empty;
            //    if (string.Compare(FromDate, CDateTime.GetNowshamsiDate()) == -1)
            //    {
            //        FromDate = CDateTime.GetNowshamsiDate();
            //        Alarm += "بدلیل گذشت از بازه ی زمانی تعیین شده، ویرایش از تاریخ " + FromDate + " تا تاریخ :" + ToDate + " انجام شد.\n";
            //    }

            //    string MealColumnName = "";
            //    switch (DayTitle)
            //    {
            //        case "SaturdayToWednesdayReservationFood": MealColumnName = "تعداد_غذا_روزهای_کاری"; break;
            //        case "ThursdayReservationFood": MealColumnName = "تعداد_غذا_روز_پنجشنبه"; break;
            //        case "FridayReservationFood": MealColumnName = "تعداد_غذا_روزهای_جمعه_و_تعطیل"; break;
            //        case "ChangeShiftSaturdayToWednesdayReservationFood": MealColumnName = "تعداد_غذا_روزهای_کاری_تعویض_شیفت"; break;
            //        case "ChangeShiftFridayReservationFood": MealColumnName = "تعداد_غذا_جمعه_و_روز_های_تعطیل_تعویض_شیفت"; break;
            //    }

            //    string ShiftChangeWeekDay = "";
            //    switch (ShiftChangeDay)
            //    {
            //        case 1: { ShiftChangeWeekDay = "شنبه"; break; }
            //        case 2: { ShiftChangeWeekDay = "یک شنبه"; break; }
            //        case 3: { ShiftChangeWeekDay = "دو شنبه"; break; }
            //        case 4: { ShiftChangeWeekDay = "سه شنبه"; break; }
            //        case 5: { ShiftChangeWeekDay = "چهار شنبه"; break; }
            //        case 6: { ShiftChangeWeekDay = "پنج شنبه"; break; }
            //        case 7: { ShiftChangeWeekDay = "جمعه"; break; }
            //    }
            //    DataTable DayData = Referral.DBData.SelectDataTableAsync("Select * from لیست_تاریخ where تاریخ_شمسی>=N'" + FromDate + "' and تاریخ_شمسی <=N'" + ToDate + "'");
            //    foreach (DataRow DayRow in DayData.Rows)
            //    { 
            //        bool IsUpdate=false;
            //        if (DayRow["روز_هفته"].ToString() == "پنج شنبه" && DayTitle== "ThursdayReservationFood")
            //            IsUpdate=true; 
            //        else if ((DayRow["روز_هفته"].ToString() == "جمعه" || DayRow["تعطیل"].ToString() == "True") && DayTitle == "FridayReservationFood")
            //            IsUpdate = true;
            //        else if(DayTitle == "SaturdayToWednesdayReservationFood" && DayRow["وضعیت_روز_کاری"].ToString() == "True" && ShiftChangeWeekDay != DayRow["روز_هفته"].ToString())
            //            IsUpdate = true;
            //        else if(ShiftChangeWeekDay == DayRow["روز_هفته"].ToString() && DayTitle == "ChangeShiftSaturdayToWednesdayReservationFood" && DayRow["تعطیل"].ToString() != "True")
            //            IsUpdate = true;
            //        else if(ShiftChangeWeekDay == DayRow["روز_هفته"].ToString()  && DayTitle == "ChangeShiftFridayReservationFood" && DayRow["تعطیل"].ToString() == "True")
            //            IsUpdate = true; 

            //        if (IsUpdate)
            //        { 
            //            FinalQuery += "Update نفرات_رزرو_غذا set تعداد = " + DayValue + " Where تاریخ_رزرو =N'" + DayRow["تاریخ_شمسی"].ToString() + "'   And درخواست_رزرو_غذا =" + ParentID + " And پرسنل = -2 And  وعده_غذایی =" + Meal + "\n";
            //            FinalQuery += "Update نفرات_رزرو_غذا set تعداد = 0 Where تاریخ_رزرو =N'" + DayRow["تاریخ_شمسی"].ToString() + "'  And درخواست_رزرو_غذا =" + ParentID + " And پرسنل > -2 And  وعده_غذایی =" + Meal + "\n";
            //        }

            //    }

            //    string ErrorMessage = string.Empty;
            //    Referral.DBData.ExecuteAsync(FinalQuery);

            //    CoreObject ShiftMealTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "وعده_های_رزرو_شده_نوبتکار");

            //    string[] CoulmnName = new string[] { MealColumnName };
            //    object[] ColumnValue = new object[] { DayValue }; 
            //    Alarm += Desktop.CheckBeforRunQuery(ShiftMealTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue); 
            //    if (Referral.DBData.UpdateRow(RecordID, ShiftMealTableCore.CoreObjectID, ShiftMealTableCore.FullName, CoulmnName, ColumnValue))
            //    {
            //        Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, ShiftMealTableCore, RecordID, CoulmnName, ColumnValue, "", ShiftMealTableCore.CoreObjectID.ToString());
            //        return Json(Alarm);
            //    }
            //    return Json("ذخیره سازی با شکست مواجه شد"); 
            //}
            //else
            //    Alarm += "بدلیل گذشت از بازه ی زمانی امکان ویرایش وچود ندارد";
            return Json(1);
        }

        public async Task<JsonResult> RemoveShiftMealGrid(long ParentID, string FromDate, string ToDate, long RecordID, int Meal)
        {
            DataTable PersonData = await Referral.DBData.SelectDataTableAsync("Select شناسه From نفرات_رزرو_غذا Where  درخواست_رزرو_غذا =" + ParentID + "  And  وعده_غذایی =" + Meal);
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
            CoreObject ShiftMealTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "وعده_های_رزرو_شده_نوبتکار");
            string Alarm = string.Empty;
            foreach (DataRow row in PersonData.Rows)
            {
                Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(row[0].ToString()), CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
                if (Alarm != "")
                    return Json(Alarm);
                else
                {
                    if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(row[0].ToString())))
                        await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(row[0].ToString()), new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
                }
            }

            Alarm = Desktop.CheckBeforRunQuery(ShiftMealTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
            if (Alarm != "")
                return Json(Alarm);
            else
            {
                if (Referral.DBData.Delete(ShiftMealTableCore.FullName, RecordID))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, ShiftMealTableCore, RecordID, new string[] { }, new object[] { }, "", ShiftMealTableCore.CoreObjectID.ToString());
            }

            return Json("");
        }

        public async Task<ActionResult> ReadPersonShift([DataSourceRequest] DataSourceRequest _Request, long RecordID = 0)
        {
            JsonResult jsonResult = new JsonResult();
            DataTable PersonData = new DataTable();
            if (RecordID > 0)
            {
                string Query = "Declare @درخواست_رزرو_غذا as bigint = " + RecordID.ToString() + "  \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "خواندن_پرسنل_درخواست_غذا_از_نوع_نوبتکار");
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                PersonData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
            }

            jsonResult = Json(PersonData.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public JsonResult RemoveShiftPersonGrid(long ParentID, long RecordID, bool Bed, string FromDate = "", string ToDate = "")
        {
            string Alarm = string.Empty;
            //if(string.Compare(FromDate, CDateTime.GetNowshamsiDate()) == -1)
            //{ 
            //    FromDate = CDateTime.GetNowshamsiDate();
            //    //Referral.DBData.ExecuteAsync("Delete نفرات_رزرو_غذا where  درخواست_رزرو_غذا =" + ParentID + "  And پرسنل = " + RecordID + " And تاریخ_رزرو >= N'" + FromDate + "' and تاریخ_رزرو <= N'" + ToDate + "'");
            //    //Alarm = "بدلیل گذشت از بازه ی زمانی  تعیین شده، رزرو غذا از تاریخ امروز مورخ " + FromDate + " تا تاریخ " + ToDate + " برای پرسنل انتخاب شده حذف گردید و امکان بازگشت وجود ندارد \n\n";
            //}     
            //if (Bed)
            //{
            //    CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
            //    DataTable PersonData = Referral.DBData.SelectDataTableAsync("Select شناسه From پذیرش_مهمانسرا Where  رزرو_غذا = " + ParentID + " And  پرسنل = " + RecordID );

            //    foreach (DataRow row in PersonData.Rows)
            //    {
            //       Alarm += Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(row[0].ToString()), CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
            //        if (Alarm != "")
            //            return Json(Alarm);
            //        else
            //        {
            //            if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(row[0].ToString())))
            //                Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(row[0].ToString()), new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
            //        }
            //    }
            //}

            return Json(Alarm);
        }
        public async Task<ActionResult> ReadReservationFood([DataSourceRequest] DataSourceRequest _Request, long RecordID = 0, string Date = "", string SelectedMeal = "", long Restaurant = 0)
        {
            JsonResult jsonResult = new JsonResult();
            DataTable MealData = new DataTable();
            if (RecordID > 0)
            {
                DataTable ReserveData = await Referral.DBData.SelectDataTableAsync("Select * From درخواست_رزرو_غذا Where شناسه = " + RecordID.ToString());
                string[] MealId = new string[0];
                string[] FoodId = new string[0];
                if (ReserveData.Rows[0]["وعده_غذایی"].ToString() != SelectedMeal)
                {
                    string[] OldMeal = ReserveData.Rows[0]["وعده_غذایی"].ToString().Split(',');
                    string[] NewMeal = SelectedMeal.Split(',');
                    foreach (string OldMealItem in OldMeal)
                        if (Array.IndexOf(NewMeal, OldMealItem) > -1)
                            NewMeal = NewMeal.Where(val => val != OldMealItem).ToArray();
                    if (NewMeal.Length > 0)
                    {
                        string MealDefualtFoodQuery = "Declare @از_تاریخ as Nvarchar(255) = N'" + Date + "' \n";
                        MealDefualtFoodQuery += "Declare @تا_تاریخ as Nvarchar(255) = N'" + Date + "' \n";
                        MealDefualtFoodQuery += "declare @مکان  as bigint = " + Restaurant + " \n";
                        MealDefualtFoodQuery += "SELECT [برنامه_غذایی_مکان].تاریخ,برنامه_غذایی_مکان.وعده_غذایی,  غذا.[شناسه] " +
                                    "  FROM غذا inner join[برنامه_غذایی_مکان] on غذا.شناسه = برنامه_غذایی_مکان.غذا inner join[کاربر_وعده_غذایی] on[برنامه_غذایی_مکان].وعده_غذایی = [کاربر_وعده_غذایی].وعده_غذایی" +
                                    "  WHERE[کاربر_وعده_غذایی].کاربر = @شناسه_کاربر" +
                                    "  and( [برنامه_غذایی_مکان].تاریخ >= @از_تاریخ   and[برنامه_غذایی_مکان].تاریخ <= @تا_تاریخ )" +
                                    "  and برنامه_غذایی_مکان.وعده_غذایی in(" + string.Join(",", string.Join(",", NewMeal)) + ")" +
                                    "  and[برنامه_غذایی_مکان].مکان = @مکان" +
                                    "  and[برنامه_غذایی_مکان].وضعیت_پیشفرض = 1" +
                                    "  order by[برنامه_غذایی_مکان].تاریخ";

                        DataTable DefualtFoodData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(MealDefualtFoodQuery));

                        foreach (DataRow Row in DefualtFoodData.Rows)
                        {
                            Array.Resize(ref MealId, MealId.Length + 1);
                            MealId[MealId.Length - 1] = Row["وعده_غذایی"].ToString();

                            Array.Resize(ref FoodId, FoodId.Length + 1);
                            FoodId[FoodId.Length - 1] = Row["شناسه"].ToString();
                        }
                    }
                }

                if (SelectedMeal == "" && (bool)ReserveData.Rows[0]["وضعیت_رزرو_تخت"] == true)
                {
                    string Query = "Declare @درخواست_رزرو_غذا as bigint = " + RecordID + "\n" +
                     "Declare @تاریخ_رزرو as Nvarchar(255)=N'" + Date + "'\n" +
                     "Select " +
                     "@درخواست_رزرو_غذا as شناسه_درخواست_غذا" +
                     ",\nپرسنل as شناسه \n" +
                     "\n, ROW_NUMBER() OVER(ORDER BY پرسنل ASC) as ردیف" +
                     "\n,@تاریخ_رزرو as تاریخ" +
                     "\n,(select پرسنل.نام+N' '+پرسنل.نام_خانوادگی from پرسنل where پرسنل.شناسه = پذیرش_مهمانسرا.پرسنل) as نام_پرسنل" +
                     "\n,(select پرسنل.شماره_پرسنلی  from پرسنل where پرسنل.شناسه = پذیرش_مهمانسرا.پرسنل) as شماره_پرسنلی" +
                     "\n,(select پرسنل.کد_ملی  from پرسنل where پرسنل.شناسه = پذیرش_مهمانسرا.پرسنل) as شماره_ملی" +
                     "\n, 1 as تخت" +
                     "\nFrom پذیرش_مهمانسرا " +
                     "\nWhere پذیرش_مهمانسرا.رزرو_غذا = @درخواست_رزرو_غذا ";
                    MealData = await Referral.DBData.SelectDataTableAsync(Query);
                }
                else if (Restaurant != (long)ReserveData.Rows[0]["رستوران"])
                {
                    string Query = "Declare @درخواست_رزرو_غذا as bigint = " + RecordID + "\n" +
                    "Declare @تاریخ_رزرو as Nvarchar(255)=N'" + Date + "'\n" +
                    "Declare @رستوران as bigint = " + Restaurant + "\n" +
                    "Select " +
                    "@درخواست_رزرو_غذا as شناسه_درخواست_غذا" +
                    ",\nپرسنل as شناسه \n" +
                    "\n, ROW_NUMBER() OVER(ORDER BY پرسنل ASC) as ردیف" +
                    "\n,تاریخ_رزرو as تاریخ" +
                    "\n,(select پرسنل.نام+N' '+پرسنل.نام_خانوادگی from پرسنل where پرسنل.شناسه = نفرات_رزرو_غذا.پرسنل) as نام_پرسنل" +
                    "\n,(select پرسنل.شماره_پرسنلی  from پرسنل where پرسنل.شناسه = نفرات_رزرو_غذا.پرسنل) as شماره_پرسنلی" +
                    "\n,(select پرسنل.کد_ملی  from پرسنل where پرسنل.شناسه = نفرات_رزرو_غذا.پرسنل) as شماره_ملی";

                    string[] SelectedMealArr = SelectedMeal.Split(',');
                    for (int Index = 0; Index < SelectedMealArr.Length; Index++)
                    {
                        if (Array.IndexOf(MealId, SelectedMealArr[Index]) > -1)
                            Query += "\n," + FoodId[Array.IndexOf(MealId, SelectedMealArr[Index])] + " as  GridMealReservationFood_" + SelectedMealArr[Index];
                        else
                            Query += "\n,(select top 1 A.غذا from برنامه_غذایی_مکان as A where A.مکان =@رستوران and A.تاریخ =  @تاریخ_رزرو and A.وضعیت_پیشفرض = 1 and A.وعده_غذایی = " + SelectedMealArr[Index] + ") as GridMealReservationFood_" + SelectedMealArr[Index];
                    }
                    Query += "\n,(select count(1) from پذیرش_مهمانسرا where پذیرش_مهمانسرا.رزرو_غذا = نفرات_رزرو_غذا.درخواست_رزرو_غذا and پذیرش_مهمانسرا.پرسنل = نفرات_رزرو_غذا.پرسنل and تاریخ_رزرو between پذیرش_مهمانسرا.از_تاریخ and پذیرش_مهمانسرا.تا_تاریخ )  as تخت" +
                        "\nFROM  نفرات_رزرو_غذا" +
                        "\nwhere" +
                        "\nدرخواست_رزرو_غذا = @درخواست_رزرو_غذا " +
                        "\nand تاریخ_رزرو = @تاریخ_رزرو" +
                        "\ngroup by پرسنل , تاریخ_رزرو,درخواست_رزرو_غذا";

                    MealData = await Referral.DBData.SelectDataTableAsync(Query);
                }
                else
                {
                    string Query = "Declare @درخواست_رزرو_غذا as bigint = " + RecordID + "\n" +
                    "Declare @تاریخ_رزرو as Nvarchar(255)=N'" + Date + "'\n" +
                    "Select " +
                    "@درخواست_رزرو_غذا as شناسه_درخواست_غذا" +
                    ",\nپرسنل as شناسه \n" +
                    "\n, ROW_NUMBER() OVER(ORDER BY پرسنل ASC) as ردیف" +
                    "\n,تاریخ_رزرو as تاریخ" +
                    "\n,(select پرسنل.نام+N' '+پرسنل.نام_خانوادگی from پرسنل where پرسنل.شناسه = نفرات_رزرو_غذا.پرسنل) as نام_پرسنل" +
                    "\n,(select پرسنل.شماره_پرسنلی  from پرسنل where پرسنل.شناسه = نفرات_رزرو_غذا.پرسنل) as شماره_پرسنلی" +
                    "\n,(select پرسنل.کد_ملی  from پرسنل where پرسنل.شناسه = نفرات_رزرو_غذا.پرسنل) as شماره_ملی";

                    string[] SelectedMealArr = SelectedMeal.Split(',');
                    for (int Index = 0; Index < SelectedMealArr.Length; Index++)
                    {
                        if (Array.IndexOf(MealId, SelectedMealArr[Index]) > -1)
                            Query += "\n," + FoodId[Array.IndexOf(MealId, SelectedMealArr[Index])] + " as  GridMealReservationFood_" + SelectedMealArr[Index];
                        else
                            Query += "\n,(select top 1 A.غذا from نفرات_رزرو_غذا as A where A.درخواست_رزرو_غذا =نفرات_رزرو_غذا.درخواست_رزرو_غذا and A.تاریخ_رزرو =  نفرات_رزرو_غذا.تاریخ_رزرو and A.پرسنل = نفرات_رزرو_غذا.پرسنل  and A.وعده_غذایی = " + SelectedMealArr[Index] + ") as GridMealReservationFood_" + SelectedMealArr[Index];
                    }
                    Query += "\n,(select count(1) from پذیرش_مهمانسرا where پذیرش_مهمانسرا.رزرو_غذا = نفرات_رزرو_غذا.درخواست_رزرو_غذا and پذیرش_مهمانسرا.پرسنل = نفرات_رزرو_غذا.پرسنل and تاریخ_رزرو between پذیرش_مهمانسرا.از_تاریخ and پذیرش_مهمانسرا.تا_تاریخ )  as تخت" +
                        "\nFROM  نفرات_رزرو_غذا" +
                        "\nwhere" +
                        "\nدرخواست_رزرو_غذا = @درخواست_رزرو_غذا " +
                        "\nand تاریخ_رزرو = @تاریخ_رزرو" +
                        "\ngroup by پرسنل , تاریخ_رزرو,درخواست_رزرو_غذا";

                    MealData = await Referral.DBData.SelectDataTableAsync(Query);
                }

            }

            jsonResult = Json(MealData.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public async Task<JsonResult> UpdateOfficeReservationFood(long ParentID, string FromDate, long Person, string Meal, string MealValue)
        {
            string Query = string.Empty;
            string[] CoulmnName = new string[1];
            object[] ColumnValue = new object[1];
            string ErrorMessage = string.Empty;

            if (Meal.IndexOf("GridMealReservationFood") > -1)
            {
                CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
                Query = "Select شناسه From نفرات_رزرو_غذا Where تاریخ_رزرو=N'" + FromDate + "' AND پرسنل=" + Person + " And وعده_غذایی = " + Meal.Replace("GridMealReservationFood_", "");
                CoulmnName[0] = "غذا";
                ColumnValue[0] = MealValue;

                string ID = (await Referral.DBData.SelectFieldAsync(Query)).ToString();
                if (ID != "")
                {
                    string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(ID), CoreDefine.TableEvents.شرط_اجرای_ویرایش, CoulmnName, ColumnValue);
                    if (Alarm != "")
                        return Json(Alarm);
                    //else
                    //{
                    //    if (Referral.DBData.UpdateRow(long.Parse(ID), PersonReservationTableCore.CoreObjectID, PersonReservationTableCore.FullName, CoulmnName, ColumnValue))
                    //        Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_ویرایش, PersonReservationTableCore, long.Parse(ID), CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                    //}

                    return Json("");
                }
            }
            else if (Meal.IndexOf("تخت") > -1)
            {
                CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
                Query = "Select Top 1  شناسه   FROM  پذیرش_مهمانسرا  Where N'" + FromDate + "' between از_تاریخ and تا_تاریخ AND پرسنل = " + Person + " And رزرو_غذا = " + ParentID;
                string ID = (await Referral.DBData.SelectFieldAsync(Query)).ToString();
                if (ID != "")
                {
                    if (MealValue == "false")
                    {
                        string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(ID), CoreDefine.TableEvents.شرط_اجرای_حذف, CoulmnName, ColumnValue);
                        if (Alarm != "")
                            return Json(Alarm);
                        else
                        {
                            if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(ID)))
                                await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(ID), CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                        }
                    }
                }
                //else if(MealValue== "true")
                //{
                //    CoulmnName = new string[] { "رزرو_غذا", "پرسنل" };
                //    ColumnValue = new object[] { ParentID, Person };

                //    string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, 0, CoreDefine.TableEvents.شرط_اجرای_درج, CoulmnName, ColumnValue);
                //    if (Alarm != "")
                //        return Json(Alarm);
                //    else
                //    {
                //        //long RowID = Referral.DBData.Insert(PersonReservationTableCore.FullName, CoulmnName, ColumnValue, PersonReservationTableCore.CoreObjectID);
                //        //Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_درج, PersonReservationTableCore, RowID, CoulmnName, ColumnValue, "", PersonReservationTableCore.CoreObjectID.ToString());
                //    }
                //}
                return Json("");
            }


            return Json("0");
        }

        public async Task<JsonResult> RemoveRowOffice(long RecordID, long Person, string Date, bool Bed)
        {
            try
            {
                CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
                DataTable PersonData = await Referral.DBData.SelectDataTableAsync("Select شناسه From نفرات_رزرو_غذا Where درخواست_رزرو_غذا = " + RecordID + " And  پرسنل = " + Person + " And تاریخ_رزرو =N'" + Date + "'");

                foreach (DataRow row in PersonData.Rows)
                {
                    string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(row[0].ToString()), CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
                    if (Alarm != "")
                        return Json(Alarm);
                    else
                    {
                        if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(row[0].ToString())))
                            await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(row[0].ToString()), new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
                    }
                }

                if (Bed == true)
                {
                    PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
                    PersonData = await Referral.DBData.SelectDataTableAsync("Select شناسه From پذیرش_مهمانسرا Where  رزرو_غذا = " + RecordID + " And  پرسنل = " + Person + " And  N'" + Date + "' Between از_تاریخ And تا_تاریخ");

                    foreach (DataRow row in PersonData.Rows)
                    {
                        string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(row[0].ToString()), CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
                        if (Alarm != "")
                            return Json(Alarm);
                        else
                        {
                            if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(row[0].ToString())))
                                await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(row[0].ToString()), new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
                        }
                    }

                }
                return Json("");
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        public async Task<JsonResult> RemoveDelayedPerson(long RecordID, long Person)
        {
            try
            {
                CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "نفرات_رزرو_غذا");
                DataTable PersonData = await Referral.DBData.SelectDataTableAsync("Select شناسه From نفرات_رزرو_غذا Where درخواست_رزرو_غذا = " + RecordID + " And  پرسنل = " + Person);

                foreach (DataRow row in PersonData.Rows)
                {
                    string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, long.Parse(row[0].ToString()), CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
                    if (Alarm != "")
                        return Json(Alarm);
                    else
                    {
                        if (Referral.DBData.Delete(PersonReservationTableCore.FullName, long.Parse(row[0].ToString())))
                            await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, long.Parse(row[0].ToString()), new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
                    }
                }

                return Json("");
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> ReadDelayedReservationFood([DataSourceRequest] DataSourceRequest _Request, long RecordID = 0)
        {
            JsonResult jsonResult = new JsonResult();
            DataTable PersonBedData = new DataTable();
            if (RecordID > 0)
            {
                string Query = "Declare @درخواست_رزرو_غذا   as bigint =" + RecordID + " \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "خواندن_پرسنل_درخواست_غذا_از_نوع_روزکار");
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                PersonBedData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
            }

            jsonResult = Json(PersonBedData.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        public async Task<ActionResult> ReadBedReservationFood([DataSourceRequest] DataSourceRequest _Request, long RecordID = 0)
        {
            JsonResult jsonResult = new JsonResult();
            DataTable PersonBedData = new DataTable();
            if (RecordID > 0)
            {
                string Query = "Declare @رزرو_غذا as bigint =" + RecordID + " \n";
                CoreObject SpecialPhraseCore = CoreObject.Find(CoreDefine.Entities.عبارت_ویژه, "لیست_پرسنل_رزرو_تخت_در_درخواست_نامه");
                SpecialPhrase specialPhrase = new SpecialPhrase(SpecialPhraseCore);
                PersonBedData = await Referral.DBData.SelectDataTableAsync(Tools.CheckQuery(Query + specialPhrase.Query));
            }

            jsonResult = Json(PersonBedData.ToDataSourceResult(_Request));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }

        public async Task<JsonResult> RemoveLetterBed(long RecordID)
        {
            CoreObject PersonReservationTableCore = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "پذیرش_مهمانسرا");
            string Alarm = Desktop.CheckBeforRunQuery(PersonReservationTableCore.CoreObjectID, RecordID, CoreDefine.TableEvents.شرط_اجرای_حذف, new string[] { }, new object[] { });
            if (Alarm != "")
                return Json(Alarm);
            else
            {
                if (Referral.DBData.Delete(PersonReservationTableCore.FullName, RecordID))
                    await Desktop.AfterRunQuery(CoreDefine.TableEvents.دستور_بعد_از_حذف, PersonReservationTableCore, RecordID, new string[] { }, new object[] { }, "", PersonReservationTableCore.CoreObjectID.ToString());
            }
            return Json("");
        }
    }
} 