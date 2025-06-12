using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace APM.Models.Security
{
    public static class Log
    {

        public static string FuncionLogPath {
            get { 
                return System.Web.HttpContext.Current.Server.MapPath("~/SysLog/Function"); 
            }
        }
        public static string ErrorLogPath {
            get { 
                return System.Web.HttpContext.Current.Server.MapPath("~/SysLog/Error"); 
            }
        }
        public static bool LogFunction(string FunctionName,bool IsStarted=true)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FuncionLogPath + "\\" + Tools.CDateTime.GetNowshamsiDate().Replace("/", "-") + ".txt", true))
                {
                    writer.WriteLine((IsStarted ? "Start" : "End") + " " + FunctionName + " " + Tools.CDateTime.GetNowshamsiDate().Replace("/", "-") + " " + Tools.CDateTime.GetNowTime().Replace(":", "-"));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;   
            }
        }

        public static bool Error(string FunctionName,string ErrorMessage)
        {
            try
            { 
                using (StreamWriter writer = new StreamWriter(ErrorLogPath + "\\" +  Tools.CDateTime.GetNowshamsiDate().Replace("/", "-") + ".txt", true))
                { 
                    writer.WriteLine("Function Name : "+FunctionName + "               " + Tools.CDateTime.GetNowshamsiDate().Replace("/", "-") + " " + Tools.CDateTime.GetNowTime().Replace(":", "-"));
                    writer.WriteLine(ErrorMessage);
                    writer.WriteLine("\n");
                } 
                return true;
            }
            catch (Exception ex)
            {
                return false;   
            }
        }
    }
}