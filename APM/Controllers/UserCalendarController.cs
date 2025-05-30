using APM.Models;
using APM.Models.Database;
using APM.Models.Tools;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class UserCalendarController : Controller
    {
        // GET: UserCalendar 

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Destroy(int TaskID)
        {
            Referral.DBData.Delete( "برنامه_ریزی", TaskID);
            return Json(true);
        }

        public JsonResult SaveEdit(int TaskID,string CDate, string FromTime, string ToTime, string Title, string Description,string DescriptionResult, string AlarmType)
        {

            CoreObject UserCalenderTable = CoreObject.Find(CoreDefine.Entities.جدول, "برنامه_ریزی");
            Referral.DBData.UpdateRow(TaskID, UserCalenderTable.CoreObjectID, UserCalenderTable.FullName, new string[] { "تاریخ", "پرسنل", "عنوان", "شرح_فعالیت", "از_ساعت", "تا_ساعت", "نوع_تکرار", "نوع_هشدار" , "علت_یا_نتیجه" }, new object[] { CDate, Referral.UserAccount.PersonnelID, Title, Description, FromTime, ToTime, "", AlarmType, DescriptionResult });
 
            return Json(true);
        }

        public JsonResult SaveTaskType(long TaskID, bool TaskType, string Description)
        {
            CoreObject UserCalenderTable = CoreObject.Find(CoreDefine.Entities.جدول, "برنامه_ریزی");
            Referral.DBData.UpdateRow(TaskID, UserCalenderTable.CoreObjectID, UserCalenderTable.FullName, new string[] { "وضعیت_اقدام", "علت_یا_نتیجه" }, new object[] { (TaskType ? 1 : 0), Description });

            return Json(true);
        }

        public JsonResult Save(string FromDate, string ToDate, string FromTime, string ToTime, string Title, string Description, string AlarmType)
        {
            long ID = 0;
            if (FromDate == ToDate)
                ID = Referral.DBData.Insert("برنامه_ریزی", new string[] { "تاریخ", "پرسنل", "عنوان", "شرح_فعالیت", "از_ساعت", "تا_ساعت", "نوع_تکرار", "نوع_هشدار" }, new object[] { FromDate, Referral.UserAccount.PersonnelID, Title, Description, FromTime, ToTime, "", AlarmType });
            else
            {
                ID = Referral.DBData.Insert("برنامه_ریزی", new string[] { "تاریخ", "پرسنل", "عنوان", "شرح_فعالیت", "از_ساعت", "تا_ساعت", "نوع_تکرار", "نوع_هشدار" }, new object[] { FromDate, Referral.UserAccount.PersonnelID, Title, Description, FromTime, ToTime, "", AlarmType });
                ID = Referral.DBData.Insert("برنامه_ریزی", new string[] { "تاریخ", "پرسنل", "عنوان", "شرح_فعالیت", "از_ساعت", "تا_ساعت", "نوع_تکرار", "نوع_هشدار" }, new object[] { ToDate, Referral.UserAccount.PersonnelID, Title, Description, FromTime, ToTime, "", AlarmType });
            }
            return Json(ID);
        }
        public JsonResult SaveDaily(string FromDate, string ToDate, string FromTime, string ToTime, string Title, string Description, string AlarmType, int DailyCount)
        {
            string Query = "declare @Table as Table(شناسه BIGINT,تاریخ NVARCHAR(10)) \n" +
                "insert into @Table \n" +
                "select شناسه,تاریخ_شمسی from لیست_تاریخ where تاریخ_شمسی >= N'" + FromDate + "' and تاریخ_شمسی<= N'" + ToDate + "' \n" +
                "DECLARE @شناسه BIGINT \n" +
                "DECLARE @تاریخ VARCHAR(10)\n" +
                "DECLARE @MaxID  BIGINT = (select MAX(شناسه) from @Table)\n" +
                "DECLARE @CounterID  BIGINT = (select Min(شناسه) from @Table)\n" +
                "DECLARE TEM_CURSOR CURSOR FOR\n" +
                "select* from @Table\n" +
                "OPEN TEM_CURSOR\n" +
                "FETCH NEXT FROM TEM_CURSOR INTO @شناسه, @تاریخ\n" +
                "WHILE @@FETCH_STATUS = 0\n" +
                "BEGIN\n" +
                "if (@CounterID = @شناسه)\n" +
                "begin\n" +
                "set @CounterID = @CounterID + " + DailyCount + "\n" +
                "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(@تاریخ," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'روزانه',N'" + AlarmType + "')\n" +
                "end\n" +
                "FETCH NEXT FROM TEM_CURSOR INTO @شناسه, @تاریخ\n" +
                "END\n" +
                "CLOSE TEM_CURSOR\n" +
                "DEALLOCATE TEM_CURSOR\n";
            Referral.DBData.Execute(Query);
            Referral.DBData.Execute("Insert into Insert_APMRegistry(RegistryDate,TableName)values(N'',N'برنامه_ریزی')");
            return Json(true);
        }
        public JsonResult SaveWeekly(string FromDate, string ToDate, string FromTime, string ToTime, string Title, string Description, string AlarmType, int WeeklyCount, string WeeklyDays)
        {
            string[] DateArr = new string[0];
            string[] Days = WeeklyDays.Split(',');
            Array.Sort(Days);
            while(string.Compare(ToDate, FromDate) >0)
            {
                if(WeeklyCount > 1)
                  FromDate = CDateTime.AddDay(FromDate, WeeklyCount * 7);

                string NewDate ="" ;
                string TempDate = FromDate;
                int DaysOfWeek= CDateTime.GetNumberDayOfWeek(FromDate);
                if(string.Compare(ToDate, FromDate) != -1)
                    foreach (string Day in Days)
                    {
                        if (DaysOfWeek < (int.Parse(Day) + 1))
                            NewDate = CDateTime.AddDay(FromDate, (int.Parse(Day) + 1 - DaysOfWeek));
                        else if (DaysOfWeek == (int.Parse(Day) + 1))
                            NewDate = FromDate;
                        else if ((7 - DaysOfWeek) >= (int.Parse(Day) + 1) || ((7 - DaysOfWeek) < (int.Parse(Day) + 1)))
                            NewDate = CDateTime.AddDay(FromDate, (int.Parse(Day) + 1 + (7 - DaysOfWeek)));
                        else
                            NewDate = CDateTime.AddDay(FromDate, (int.Parse(Day) + 1));

                        if(string.Compare(ToDate, NewDate) != -1)
                        {
                            if(string.Compare(NewDate, TempDate) != -1)  
                                TempDate = NewDate;
                            if(Array.IndexOf(DateArr, NewDate) == -1)
                            {
                                Array.Resize(ref DateArr, DateArr.Length+1);
                                string Query = "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(N'" + NewDate + "'," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'هفتگی',N'" + AlarmType + "')\n";
                                Referral.DBData.Execute(Query);
                                DateArr[DateArr.Length-1] = NewDate;
                            }
                        }
                        else
                            FromDate = NewDate;
                    }
                FromDate = string.Compare(NewDate, TempDate)==1? NewDate: TempDate;

                Referral.DBData.Execute("Insert into Insert_APMRegistry(RegistryDate,TableName)values(N'',N'برنامه_ریزی')");
            }

            return Json(true);
        }
        
        public JsonResult SaveMonthly(string FromDate, string ToDate, string FromTime, string ToTime, string Title, string Description, string AlarmType, int MonthlyCount, int DayOfMonth,bool CheckedWeekOfMonth, string WeekOfMonth, string DaysOfWeekOfMonth)
        {
            string[] DateArr = new string[0];
            string[] Days = DaysOfWeekOfMonth.Split(',');
            string[] Weeks = WeekOfMonth.Split(',');
            Array.Sort(Days);
            Array.Sort(Weeks); 
            int DayOfFromDate = int.Parse(FromDate.Substring(8, 2));
            string StrDay = CheckedWeekOfMonth? FromDate.Substring(8, 2):(DayOfMonth > 9 ?  DayOfMonth.ToString() : "0" + DayOfMonth.ToString());
            string StrMonth = !CheckedWeekOfMonth && (DayOfMonth > DayOfFromDate) ? FromDate.Substring(5, 2) : "";
            string StrYear = FromDate.Substring(0, 4);
            string TempFromDate = FromDate; 

            while (string.Compare(ToDate, FromDate) >0)
            {
                if (CheckedWeekOfMonth)
                {
                    string NewDate = "";
                    if ((int.Parse(FromDate.Substring(5, 2)) + MonthlyCount) > 12)
                    {
                        StrMonth = ((int.Parse(FromDate.Substring(5, 2)) + MonthlyCount) - 12).ToString();
                        StrYear = (int.Parse(FromDate.Substring(0, 4)) + 1).ToString();
                    }
                    else
                        StrMonth = MonthlyCount == 1 ? FromDate.Substring(5, 2):(int.Parse(FromDate.Substring(5, 2)) + MonthlyCount).ToString();

                    StrMonth = (int.Parse(StrMonth) > 9 ? StrMonth : "0" + int.Parse(StrMonth).ToString());
                    FromDate = StrYear + "/" + StrMonth + "/01"; 

                    if (MonthlyCount==1)
                    {
                        while (string.Compare(ToDate, FromDate) > 0)
                        {

                            for (int Week = 0; Week < 4; Week++)
                            {
                                string EndDateWeek = "";
                                int DaysOfWeek = CDateTime.GetNumberDayOfWeek(FromDate);
                                if (DaysOfWeek >= 3)
                                {
                                    FromDate = CDateTime.AddDay(FromDate, 8 - DaysOfWeek);
                                    EndDateWeek = CDateTime.AddDay(FromDate, 6);
                                }
                                else
                                    EndDateWeek = CDateTime.AddDay(FromDate, 7 - DaysOfWeek);
                                 
                                if (Array.IndexOf(Weeks, Week.ToString()) > -1)
                                {
                                    foreach (string Day in Days)
                                    {
                                        NewDate = CDateTime.AddDay(FromDate, int.Parse(Day));
                                        if (string.Compare(NewDate, TempFromDate) > -1 && string.Compare(ToDate, NewDate) > -1)
                                        {
                                            string Query = "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(N'" + NewDate + "'," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'هفتگی',N'" + AlarmType + "')\n";
                                            Referral.DBData.Execute(Query);
                                        }
                                    }
                                }
                                FromDate = EndDateWeek;
                            }
                        }
                    }
                    else
                    {

                        while (string.Compare(ToDate, FromDate) > 0)
                        {
                            for (int Week = 0; Week < 4; Week++)
                            {
                                string EndDateWeek = "";
                                int DaysOfWeek = CDateTime.GetNumberDayOfWeek(FromDate);
                                if (DaysOfWeek >= 3)
                                {
                                    FromDate = CDateTime.AddDay(FromDate, 8 - DaysOfWeek);
                                    EndDateWeek = CDateTime.AddDay(FromDate, 6);
                                }
                                else
                                    EndDateWeek = CDateTime.AddDay(FromDate, 7 - DaysOfWeek);
                                if (Array.IndexOf(Weeks, Week.ToString()) > -1)
                                {
                                    foreach (string Day in Days)
                                    {
                                        NewDate = CDateTime.AddDay(FromDate, int.Parse(Day));
                                        if (string.Compare(NewDate, TempFromDate) > -1 && string.Compare(ToDate, NewDate) > -1)
                                        {
                                            string Query = "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(N'" + NewDate + "'," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'هفتگی',N'" + AlarmType + "')\n";
                                            Referral.DBData.Execute(Query);
                                        }
                                    }
                                }
                                FromDate = EndDateWeek;
                            }
                            if ((int.Parse(FromDate.Substring(5, 2)) + MonthlyCount) > 12)
                            {
                                StrMonth = ((int.Parse(FromDate.Substring(5, 2)) + MonthlyCount) - 12).ToString();
                                StrYear = (int.Parse(FromDate.Substring(0, 4)) + 1).ToString();
                            }
                            else
                                StrMonth =  (int.Parse(FromDate.Substring(5, 2)) + MonthlyCount).ToString();

                            StrMonth = (int.Parse(StrMonth) > 9 ? StrMonth : "0" + int.Parse(StrMonth).ToString());
                            FromDate =  StrYear + "/" + StrMonth + "/01";
                           
                        }
                    }
                }
                else
                {
                    if (string.Compare(ToDate, FromDate) != -1)
                    {

                        if (StrMonth == "")
                            if (int.Parse(FromDate.Substring(5, 2)) == 12)
                            {
                                StrYear = (int.Parse(FromDate.Substring(0, 4)) + 1).ToString();
                                StrMonth = "01";
                            }
                            else
                                StrMonth = (int.Parse(FromDate.Substring(5, 2)) + 1).ToString();


                        StrMonth = (int.Parse(StrMonth) > 9 ? StrMonth : "0" + int.Parse(StrMonth).ToString());
                        FromDate = StrYear + "/" + StrMonth + "/" + StrDay;

                        Array.Resize(ref DateArr, DateArr.Length + 1);
                        string Query = "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(N'" + FromDate + "'," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'هفتگی',N'" + AlarmType + "')\n";
                        Referral.DBData.Execute(Query);
                        DateArr[DateArr.Length - 1] = FromDate;

                        if (int.Parse(FromDate.Substring(5, 2)) == 12)
                        {
                            StrYear = (int.Parse(FromDate.Substring(0, 4)) + 1).ToString();
                            StrMonth = "01";
                        }
                        else
                        {
                            if ((int.Parse(FromDate.Substring(5, 2)) + MonthlyCount) > 12)
                            {
                                StrMonth = ((int.Parse(FromDate.Substring(5, 2)) + MonthlyCount) - 12).ToString();
                                StrYear = (int.Parse(FromDate.Substring(0, 4)) + 1).ToString();
                            }
                            else
                                StrMonth = (int.Parse(FromDate.Substring(5, 2)) + MonthlyCount).ToString();
                        }
                        StrMonth = (int.Parse(StrMonth) > 9 ? StrMonth : "0" + int.Parse(StrMonth).ToString());
                        FromDate = StrYear + "/" + StrMonth + "/" + StrDay;
                    }

                }
            } 
            Referral.DBData.Execute("Insert into Insert_APMRegistry(RegistryDate,TableName)values(N'',N'برنامه_ریزی')");

            return Json(true);
        }
        
        public JsonResult SaveYearly(string FromDate, string ToDate, string FromTime, string ToTime, string Title, string Description, string AlarmType, int YearlCount, int DayOfMonthOfYear, bool CheckedMonthOfYear, string MonthOfYear, string WeekOfMonthOfYear, string DayOfWeekOfMonthOfYear)
        {
            string[] DateArr = new string[0];
            string[] Days = DayOfWeekOfMonthOfYear.Split(',');
            string[] Weeks = WeekOfMonthOfYear.Split(',');
            string[] Months = MonthOfYear.Split(',');
            Array.Sort(Days);
            Array.Sort(Weeks);  

            int[] MonthsTemp = new int[Months.Length];

            for (int MonthCounter = 0; MonthCounter < Months.Length; MonthCounter++)
               MonthsTemp[MonthCounter] =int.Parse( Months[MonthCounter]); 

            Array.Sort(MonthsTemp);

            for (int MonthCounter = 0; MonthCounter < Months.Length; MonthCounter++)
                Months[MonthCounter]=MonthsTemp[MonthCounter].ToString()  ;

            int DayOfFromDate = int.Parse(FromDate.Substring(8, 2));
            string StrDay = CheckedMonthOfYear ? FromDate.Substring(8, 2) : (DayOfMonthOfYear > 9 ? DayOfMonthOfYear.ToString() : "0" + DayOfMonthOfYear.ToString());
            string StrMonth = !CheckedMonthOfYear && (DayOfMonthOfYear > DayOfFromDate) ? FromDate.Substring(5, 2) : "";
            string StrYear = FromDate.Substring(0, 4);
            string TempFromDate = FromDate;

            while (string.Compare(ToDate, FromDate) > 0)
            {
                if (CheckedMonthOfYear)
                { 
                        for (int Month = 1; Month <= 12; Month++)
                        {
                            if (Array.IndexOf(Months, (Month - 1).ToString()) > -1)
                            {
                                FromDate = StrYear + "/" + (Month > 9 ? Month.ToString() : "0" + Month.ToString()) + "/01";

                                if (string.Compare(FromDate, TempFromDate) > -1 && string.Compare(ToDate, FromDate) > -1)
                                {

                                    for (int Week = 0; Week < 4; Week++)
                                    {
                                        string NewDate = "";
                                        string EndDateWeek = "";
                                        int DaysOfWeek = CDateTime.GetNumberDayOfWeek(FromDate);
                                        if (DaysOfWeek >= 3)
                                        {
                                            FromDate = CDateTime.AddDay(FromDate, 8 - DaysOfWeek);
                                            EndDateWeek = CDateTime.AddDay(FromDate, 6);
                                        }
                                        else
                                            EndDateWeek = CDateTime.AddDay(FromDate, 7 - DaysOfWeek);

                                        if (Array.IndexOf(Weeks, Week.ToString()) > -1)
                                        {
                                            foreach (string Day in Days)
                                            {
                                                NewDate = CDateTime.AddDay(FromDate, int.Parse(Day));
                                                if (string.Compare(NewDate, TempFromDate) > -1 && string.Compare(ToDate, NewDate) > -1)
                                                {
                                                    string Query = "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(N'" + NewDate + "'," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'سالیانه',N'" + AlarmType + "')\n";
                                                    Referral.DBData.Execute(Query);
                                                }
                                            }
                                        }
                                        FromDate = EndDateWeek;
                                    } 
                                }
                            }
                        }
                        StrYear = (int.Parse(StrYear) + YearlCount).ToString();
                        FromDate = StrYear + "/01/01";  
                }
                else
                { 
                    while (string.Compare(ToDate, FromDate) > 0)
                    {
                        for (int Month = 1; Month <= 12; Month++)
                        {
                            if (Array.IndexOf(Months, (Month-1).ToString()) > -1)
                            {
                                FromDate = StrYear + "/" + (Month > 9 ? Month.ToString() : "0" + Month.ToString()) + "/" + StrDay;
 
                                if (string.Compare(FromDate, TempFromDate) > -1 && string.Compare(ToDate, FromDate) > -1)
                                {
                                    string Query = "insert into برنامه_ریزی (تاریخ, پرسنل, عنوان, شرح_فعالیت, از_ساعت, تا_ساعت, نوع_تکرار,نوع_هشدار)values(N'" + FromDate + "'," + Referral.UserAccount.PersonnelID + ",N'" + Title + "',N'" + Description + "',N'" + FromTime + "',N'" + ToTime + "',N'سالیانه',N'" + AlarmType + "')\n";
                                    Referral.DBData.Execute(Query);
                                }  
                            }
                        }
                        StrYear = (int.Parse(StrYear) + YearlCount).ToString();
                        FromDate = StrYear + "/01/01";
                    } 
                }
            }
            Referral.DBData.Execute("Insert into Insert_APMRegistry(RegistryDate,TableName)values(N'',N'برنامه_ریزی')");

            return Json(true);
        }

    }
}