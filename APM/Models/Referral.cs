using APM.Models.Database;
using APM.Models.Tools;
using APM.Models.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static APM.Models.Tools.Software;

namespace APM.Models
{
    public static class Referral
    {
        public static SQLDataBase DBData { get; set; }
        public static SQLDataBase DBRegistry { get; set; }
        public static SQLDataBase DBAttachment { get; set; }
        public static SQLDataBase DBCore { get; set; }
        public static SQLDataBase DBMsdb { get; set; }
        public static string AppVersion { get; set; }
        public static Customers CustomerName { get; set; }  
        public static List<CoreObject> CoreObjects { get; set; } =new List<CoreObject>(); 
        public static UserAccount UserAccount 
        { 
            get 
            {
                return (UserAccount)HttpContext.Current.Session["DataInfoUserAccount"];
            } 
            set 
            {
                HttpContext.Current.Session["DataInfoUserAccount"] = value;
            } 
        }
        public static PublicSetting PublicSetting { 
            get
            {
                return (PublicSetting)System.Runtime.Caching.MemoryCache.Default["Referral.PublicSetting"];
            }
            set
            {
                System.Runtime.Caching.MemoryCache.Default["Referral.PublicSetting"] = value;
            } 
        }
        public static AdminSetting AdminSetting { get; set; }
        public static int Browser_Width { get; set; }
        public static int Browser_Height { get; set; }
        public static int MasterPopupEditor_Width { get; set; }
        public static int MasterPopupEditor_Height { get; set; }
        public static long MasterDatabaseID { get; set; }
        public static string MiladyDate { get; set; }
        public static string ShamsiDate { get; set; }
        public static List<string> FontList { get { return Tools.Tools.BFontNames(); } } 

    } 


}