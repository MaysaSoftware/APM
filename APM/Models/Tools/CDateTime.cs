using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace APM.Models.Tools
{
    public static class CDateTime
    {
        public static int GetPersianDaysDiffDate(string Date1, string Date2)
        {
            int year1 = Convert.ToInt16(Date1.Substring(0, 4));
            int month1 = Convert.ToInt16(Date1.Substring(5, 2));
            int day1 = Convert.ToInt16(Date1.Substring(8, 2));

            int year2 = Convert.ToInt16(Date2.Substring(0, 4));
            int month2 = Convert.ToInt16(Date2.Substring(5, 2));
            int day2 = Convert.ToInt16(Date2.Substring(8, 2));

            System.Globalization.PersianCalendar calendar = new System.Globalization.PersianCalendar();
            DateTime dt1 = calendar.ToDateTime(year1, month1, day1, 0, 0, 0, 0);
            DateTime dt2 = calendar.ToDateTime(year2, month2, day2, 0, 0, 0, 0);
            TimeSpan ts = dt2.Subtract(dt1);

            return ts.Days;
        }

        public static string GetNowshamsiDate()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate =  DateTime.Now; 

            return string.Format("{0}/{1}/{2}", pc.GetYear(thisDate), pc.GetMonth(thisDate).ToString().Length==1?"0"+ pc.GetMonth(thisDate).ToString(): pc.GetMonth(thisDate).ToString(), pc.GetDayOfMonth(thisDate).ToString().Length==1?"0"+ pc.GetDayOfMonth(thisDate).ToString(): pc.GetDayOfMonth(thisDate).ToString());
        }
        public static string StartDateCurrentMonth()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate =  DateTime.Now; 

            return string.Format("{0}/{1}/{2}", pc.GetYear(thisDate), pc.GetMonth(thisDate).ToString().Length==1?"0"+ pc.GetMonth(thisDate).ToString(): pc.GetMonth(thisDate).ToString(),"01");
        }
        public static string StartDateCurrentYear()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate =  DateTime.Now; 

            return string.Format("{0}/{1}/{2}", pc.GetYear(thisDate),"01","01");
        }
        public static string GetDayOfWeekshamsiDate()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate =  DateTime.Now; 

            return string.Format("{0}", pc.GetDayOfWeek(thisDate));
        }
        public static string GetNowMiladyDate()
        {
            DateTime thisDate =  DateTime.Now; 
            return string.Format("{0}/{1}/{2}", thisDate.Year.ToString(), thisDate.Month.ToString().Length==1?"0"+ thisDate.Month.ToString(): thisDate.Month.ToString(), thisDate.Day.ToString().Length==1?"0"+ thisDate.Day.ToString(): thisDate.Day.ToString());
        }

        public static string ConvertShamsiToMilady(string Date)
        {
            string[] DateArr = Date.Split('/');
            if (DateArr.Length != 3)
                return Date;
            else
            {
                PersianCalendar pc = new PersianCalendar();
                DateTime thisDate = new DateTime(int.Parse(DateArr[0]), int.Parse(DateArr[1]), int.Parse(DateArr[2]), pc);
                return string.Format("{0}/{1}/{2}", thisDate.Year.ToString(), thisDate.Month.ToString().Length == 1 ? "0" + thisDate.Month.ToString() : thisDate.Month.ToString(), thisDate.Day.ToString().Length == 1 ? "0" + thisDate.Day.ToString() : thisDate.Day.ToString());
            }
        }


        public static string GetNowTime()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now; 
            
            return string.Format("{0}:{1}:{2}", pc.GetHour(thisDate).ToString().Length == 1 ? "0" + pc.GetHour(thisDate).ToString() : pc.GetHour(thisDate).ToString(), pc.GetMinute(thisDate).ToString().Length == 1 ? "0" + pc.GetMinute(thisDate).ToString() : pc.GetMinute(thisDate).ToString(), pc.GetSecond(thisDate).ToString().Length == 1 ? "0" + pc.GetSecond(thisDate).ToString() : pc.GetSecond(thisDate).ToString());
        }

        public static string AddDay(string _Date, int _Day)
        {
            int Year = Convert.ToInt16(_Date.Substring(0, 4));
            int Month = Convert.ToInt16(_Date.Substring(5, 2));
            int Day = Convert.ToInt16(_Date.Substring(8, 2)); 

            PersianCalendar pc =new PersianCalendar();
            DateTime NewDate = pc.ToDateTime(Year, Month, Day, 0, 0, 0, 0);
            DateTime thisDate = pc.AddDays(NewDate, _Day);

            return string.Format("{0}/{1}/{2}", pc.GetYear(thisDate), pc.GetMonth(thisDate).ToString().Length == 1 ? "0" + pc.GetMonth(thisDate).ToString() : pc.GetMonth(thisDate).ToString(), pc.GetDayOfMonth(thisDate).ToString().Length == 1 ? "0" + pc.GetDayOfMonth(thisDate).ToString() : pc.GetDayOfMonth(thisDate).ToString());
        }

        public static int GetNumberDayOfWeek(string _Date)
        {
            int Year = Convert.ToInt16(_Date.Substring(0, 4));
            int Month = Convert.ToInt16(_Date.Substring(5, 2));
            int Day = Convert.ToInt16(_Date.Substring(8, 2));

            PersianCalendar pc =new PersianCalendar();
            DateTime NewDate = pc.ToDateTime(Year, Month, Day, 0, 0, 0, 0);
            switch(NewDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return 1;
                case DayOfWeek.Sunday:
                    return 2;
                case DayOfWeek.Monday:
                    return 3;
                case DayOfWeek.Tuesday:
                    return 4;
                case DayOfWeek.Wednesday:
                    return 5;
                case DayOfWeek.Thursday:
                    return 6;
                case DayOfWeek.Friday:
                    return 7;
                default: return 0;
            } 
        }
    }
}