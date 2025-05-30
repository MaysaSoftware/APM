using APM.Models.Database;
using APM.Models.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace APM.Models.Tools
{
    public class UserAccount
    {
        public int UsersID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleTypeID { get; set; }
        public string RoleName { get; set; }
        public string Permition { get; set; }
        public string LoginDate { get; set; }
        public string LoginTime { get; set; }
        public string IP { get; set; }
        public string PCName { get; set; }
        public string BrowserType { get; set; }
        public string BrowserVersion { get; set; }
        public string UserPhoto { get; set; }
        public string UserPhone { get; set; }
        public string UserEmail { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string NationalCode { get; set; }
        public long PersonnelID { get; set; }
        public long PersonnelUnitID { get; set; }
        public long PersonnelPostID { get; set; }
        public long PersonneLaudienceSideID { get; set; }
        public string LastDateChangePassword { get; set; }
        public string DefaultLink { get; set; }
        public int ReferralCount { get; set; }
        public long SuperiorPostId { get; set; }
        public UserAccount()
        {
        }
         
        public bool IsFindUser(string UserName, string Password,ref bool ISAllow, ref string _Message)
        {

            this.UserName = UserName;
            this.Password = Password;
            //string a = Security.Hash.sha256ToText(this.Password);
            //string b = Security.Hash.sha256ToText(this.Password);
            //if(a==b)
            //{

            //}

            Log.LogFunction("UserAccount.IsFindUser", true);

            if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password))
                _Message = "نام کاربری یا رمز عبور خالی است";

            string Query = "SELECT کاربر.سمت_سازمانی, لینک_صفحه_پیشفرض, نقش_کاربر.عنوان,دسترسی,  کاربر.شناسه,نام_و_نام_خانوادگی,تاریخ_آخرین_تغییر_رمز_عبور,کاربر.آدرس,شماره_موبایل,تاریخ_ثبت,ساعت_ثبت,نقش_کاربر,کد_کاربر,کاربر.کد_ملی,کد_اطلاعیه,ایمیل,رمز_عبور,فعال,مدیر_سیستم" +
                           "\n,(SELECT count(1) FROM ارجاع_مراحل_فرآیند  where دریافت_کننده = کاربر.شناسه and مشاهده_شده = 0  ) as تعداد_ارجاع_مراحل_فرآیند ,[کاربر].[پرسنل] as پرسنل " +
                    "\nfrom کاربر inner join نقش_کاربر on کاربر.نقش_کاربر = نقش_کاربر.شناسه " +
                    "\nwhere  فعال=1 and  ( شماره_موبایل= N'" + this.UserName + "' or ایمیل = N'" + this.UserName + "' or نام_کاربری = N'" + this.UserName + "') And رمز_عبور = N'" + this.Password + "'";

            //"where  فعال=1 and  ( شماره_موبایل= N'" + this.UserName + "' or ایمیل = N'" + this.UserName + "' or نام_کاربری = N'" + this.UserName + "') And رمز_عبور = N'" + this.Password + "'";
            //"where  فعال=1 and  ( شماره_موبایل= N'" + this.UserName + "' or ایمیل = N'" + this.UserName + "' ) And رمز_عبور = N'" + this.Password + "'";


            if (Referral.DBData == null)
            {
                Login.ConnectToDB();
                _Message = "عدم ارتباط با پایگاه داده";
            }
            try
            {
                DataTable dataTable = Referral.DBData.SelectDataTable(Query);
                if (dataTable is null)
                {
                    Log.Error("UserAccount.IsFindUser", "هیچ کاربری یافت نشد نام کاربری : "+this.UserName +" کلمه عبور : "+this.Password);
                    _Message = "هیچ کاربری یافت نشد";
                    return false;
                }

                if (dataTable.Rows.Count == 0)
                {
                    //string DecodePassword = Security.Hash.EncryptAes(this.Password);
                    //Query = "SELECT نقش_کاربر.عنوان,دسترسی,  کاربر.شناسه,نام_و_نام_خانوادگی,تاریخ_آخرین_تغییر_رمز_عبور,کاربر.آدرس,شماره_موبایل,تاریخ_ثبت,ساعت_ثبت,نقش_کاربر,کد_کاربر,کاربر.کد_ملی,کد_اطلاعیه,موجودی_کیف_پول,پذیرش_قوانین,ایمیل,رمز_عبور,فعال,مدیر_سیستم" +
                    //    " from کاربر inner join نقش_کاربر on کاربر.نقش_کاربر = نقش_کاربر.شناسه " +
                    //"where  فعال=1 and  ( شماره_موبایل= N'" + this.UserName + "' or ایمیل = N'" + this.UserName + "' or نام_کاربری = N'" + this.UserName + "') ";
                    //dataTable = Referral.DBData.SelectDataTable(Query);

                    if (dataTable.Rows.Count == 0)
                    { 
                        _Message = "کاربری با مشخصات فوق یافت نشد";
                        return false;
                    }
                    //else
                    //{
                    //    ////foreach (DataRow row in dataTable.Rows)
                    //    ////{
                    //    ////    if (Security.Hash.DecryptAes(row["رمز_عبور"].ToString()) == this.Password)
                    //    ////    {

                    //    ////    }
                    //    ////}
                    //}
                    //this.Password = DecodePassword;
                }

                this.UsersID = dataTable.Rows[0]["شناسه"] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[0]["شناسه"].ToString());
                if (UsersID == 0)
                {
                    _Message = "کاربر یافت نشد";
                    return false;
                }
                else
                {
                    this.RoleTypeID = Convert.ToInt32(dataTable.Rows[0]["نقش_کاربر"].ToString());
                    this.FullName = dataTable.Rows[0]["نام_و_نام_خانوادگی"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["نام_و_نام_خانوادگی"];
                    this.RoleName = dataTable.Rows[0]["عنوان"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["عنوان"];
                    this.Permition = dataTable.Rows[0]["دسترسی"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["دسترسی"];
                    this.LoginDate = CDateTime.GetNowshamsiDate();
                    Referral.ShamsiDate = CDateTime.GetNowshamsiDate();
                    Referral.MiladyDate = CDateTime.GetNowMiladyDate();
                    this.LoginTime = CDateTime.GetNowTime();
                    this.UserPhone = dataTable.Rows[0]["شماره_موبایل"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["شماره_موبایل"];
                    this.UserEmail = dataTable.Rows[0]["ایمیل"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["ایمیل"];
                    this.IsActive = dataTable.Rows[0]["فعال"] is DBNull ? false : (bool)dataTable.Rows[0]["فعال"];
                    this.IsAdmin = dataTable.Rows[0]["مدیر_سیستم"] is DBNull ? false : (bool)dataTable.Rows[0]["مدیر_سیستم"];
                    this.IP = Tools.GetIPAddress();
                    this.PCName = Tools.GetPCName();
                    this.BrowserType = Tools.GetBrowserType();
                    this.BrowserVersion = Tools.GetBrowserVersion();
                    this.NationalCode = dataTable.Rows[0]["کد_ملی"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["کد_ملی"];
                    this.DefaultLink = dataTable.Columns.IndexOf("لینک_صفحه_پیشفرض")>-1?( dataTable.Rows[0]["لینک_صفحه_پیشفرض"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["لینک_صفحه_پیشفرض"]):string.Empty;
                    this.PersonnelPostID = dataTable.Rows[0]["سمت_سازمانی"] is DBNull ? 0 : long.Parse(dataTable.Rows[0]["سمت_سازمانی"].ToString());
                    if (dataTable.Columns.IndexOf("تعداد_ارجاع_مراحل_فرآیند") > -1)
                        this.ReferralCount = dataTable.Rows[0]["تعداد_ارجاع_مراحل_فرآیند"] is DBNull ? 0 : int.Parse(dataTable.Rows[0]["تعداد_ارجاع_مراحل_فرآیند"].ToString()); 
                    this.LastDateChangePassword = dataTable.Columns.IndexOf("تاریخ_آخرین_تغییر_رمز_عبور") > -1 ? (dataTable.Rows[0]["تاریخ_آخرین_تغییر_رمز_عبور"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["تاریخ_آخرین_تغییر_رمز_عبور"]):string.Empty;
                    //this.SuperiorPostId = dataTable.Rows[0]["سمت_مافوق"] is DBNull ? 0 : (long)dataTable.Rows[0]["سمت_مافوق"];

                    if (dataTable.Rows[0]["کد_ملی"] is DBNull)
                    {

                    }
                    else
                    {
                        if(dataTable.Columns.IndexOf("پرسنل") > -1)
                        {
                            this.PersonnelID = dataTable.Rows[0]["پرسنل"] is DBNull ? 0 : long.Parse(dataTable.Rows[0]["پرسنل"].ToString());
                            //Query = "Select top 1 پرسنل.شناسه as شناسه_پرسنل ,پرسنل.واحد_سازمانی,پرسنل.سمت_سازمانی From پرسنل where پرسنل.کد_ملی = " + (dataTable.Rows[0]["کد_ملی"] is DBNull ? string.Empty : (string)dataTable.Rows[0]["کد_ملی"]);
                            if(PersonnelID > 0)
                            {
                                Query = "Select top 1 پرسنل.شناسه as شناسه_پرسنل ,پرسنل.واحد_سازمانی,پرسنل.سمت_سازمانی From پرسنل where پرسنل.شناسه = " + (dataTable.Rows[0]["پرسنل"] is DBNull ? "" : dataTable.Rows[0]["پرسنل"].ToString());
                                dataTable = Referral.DBData.SelectDataTable(Query);
                                if (dataTable != null)
                                {
                                    if (dataTable.Rows.Count > 0)
                                    {
                                        this.PersonnelID = dataTable.Rows[0]["شناسه_پرسنل"] is DBNull ? 0 : long.Parse(dataTable.Rows[0]["شناسه_پرسنل"].ToString());
                                        this.PersonnelUnitID = dataTable.Rows[0]["واحد_سازمانی"] is DBNull ? 0 : long.Parse(dataTable.Rows[0]["واحد_سازمانی"].ToString());
                                    }
                                }

                            }

                        }

                    }
                        return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("UserAccount.IsFindUser", ex.Message);
                return false;
            }
        }

        public int GetUserNotification()
        { 
            ReferralCount = int.Parse(Referral.DBData.SelectField("SELECT count(1) FROM ارجاع_مراحل_فرآیند  where دریافت_کننده = " + this.UsersID + " and مشاهده_شده = 0  ").ToString());
            return ReferralCount;
        }
    }
}