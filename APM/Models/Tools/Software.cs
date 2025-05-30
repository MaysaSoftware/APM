using APM.Models.Database;
using APM.Models.Security;
using APM.Models.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;

namespace APM.Models.Tools
{
    public static class Software
    {
        public enum Customers
        {
            MaroonPetrochemical = 0,
            NoorTrading = 1,
            MaysaSoftware = 2,
            SadaretBank = 3,
            SetareTalaeiKhaligFars = 4,
            ArvandPetrochemical = 5,
            PayaAccountant = 6,
            AmirkabirPetrochemical = 7,
            PersianGulfGoldenStar = 8,
            AreyanSanaat = 9,
            Petrofan = 10,
            YektaPardazeshPars = 11,
            Farabintav = 12,
            TondgoyanPetrochemical = 13,
            ParkElmoFanavariKhozestan=14,
            ShoraAhvaz = 15,
            Petzone= 16,
            ArvandFreeZone= 17,
            AghajariOilAndGas = 18,
            CloudLand = 19,
            RaminPower=20,
            SalmanFarsiPetrochemical=21,
            IranNasr=22,
            ShoshtarPetrochemical = 23,
            khuareeo = 24,
        }


        public static void GetPublicSeeting()
        {
            Log.LogFunction("Software.GetPublicSeeting", true);
            
            DataTable CoreData = Referral.DBCore.SelectDataTable("Delete CoreObject where Value is null" +
                                                                 "\ndelete CoreObject  where ( Entity =N'تنظیمات_مدیر_سیستم' or  Entity =N'تنظیمات_عمومی') and CoreObjectID>2" + 
                                                                 "\nSelect * from CoreObject where Entity=N'تنظیمات_عمومی' or Entity=N'" + CoreDefine.Entities.تنظیمات_مدیر_سیستم + "' " +
                                                                 "order by OrderIndex");


            if (Referral.CoreObjects != null)
                if(Referral.CoreObjects.Count>0)
                Referral.CoreObjects.Clear();

            foreach (DataRow Row in CoreData.Rows)
            {
                Referral.CoreObjects.Add(new CoreObject(
                   Convert.ToInt64(Row["CoreObjectID"].ToString()),
                   Convert.ToInt64(Row["ParentID"].ToString()),
                   Tools.GetEntity(Row["Entity"].ToString()),
                   Row["Folder"].ToString(),
                   Row["FullName"].ToString(),
                   Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                   (bool)Row["IsDefault"],
                   (object)Row["Value"]
                    ));
            }


            List<CoreObject> AdminSettingCore = Referral.CoreObjects.Where(item => item.Entity == CoreDefine.Entities.تنظیمات_مدیر_سیستم).ToList();
            long ID = 0;
            if (AdminSettingCore.Count() == 0)
            {
                ID = Referral.DBCore.Insert("CoreObject", new string[] { "ParentID", "Entity", "FullName" }, new object[] { 0, CoreDefine.Entities.تنظیمات_مدیر_سیستم, "تنظیمات_مدیر_سیستم" });
                AdminSetting adminSetting = new AdminSetting();
                adminSetting.ShowUserRegistryInLoginForm = true;
                adminSetting.ShowDataSourceListInSettingForm = true;
                adminSetting.ShowSpecialPhraseListInSettingForm = true;
                adminSetting.ShowInformationEntryFormListInSettingForm = true;
                adminSetting.ShowProcessListInSettingForm = true;
                adminSetting.ShowReportListInSettingForm = true;
                adminSetting.ShowDashboardListInSettingForm = true;
                adminSetting.ShowPublicFileListInSettingForm = true;
                adminSetting.ShowConnectWebsiteListInSettingForm = true;
                adminSetting.ShowSMSSettingInSettingForm = true;
                adminSetting.ShowEmailSettingInSettingForm = true;
                adminSetting.ShowPaymentSettingInSettingForm = true;
                adminSetting.ShowPublicSettingInSettingForm = true;
                adminSetting.ShowUserCalendar = true;

                Referral.CoreObjects.Add(new CoreObject(ID, 0, CoreDefine.Entities.تنظیمات_مدیر_سیستم, "", CoreDefine.Entities.تنظیمات_مدیر_سیستم.ToString(), 0, false, Tools.ToXML(adminSetting)));

                Referral.AdminSetting = adminSetting;
            }
            else
            {
                Referral.AdminSetting = new AdminSetting(AdminSettingCore[0]);
            }

            List<CoreObject> _PublicSetting = CoreObject.FindChilds(CoreDefine.Entities.تنظیمات_عمومی);
            Referral.PublicSetting = new PublicSetting(_PublicSetting[0]);

            if (Referral.UserAccount != null)
            {
                bool _Allowed = false;
                string _Message = "";
                UserAccount _UserAccount = new UserAccount();
                _UserAccount.IsFindUser(Referral.UserAccount.UserName, Referral.UserAccount.Password, ref _Allowed,ref _Message);
                Referral.UserAccount = _UserAccount;
            }


            Log.LogFunction("Software.GetPublicSeeting", false);
        }

        public static bool CheckCoreIsReload()
        {
            return Referral.CoreObjects.Count>10?true:false;    
        }
        public static void CoreReload()
        {
            Log.LogFunction("Software.CoreReload");
            JustCoreReload();

            List<CoreObject> _PublicSetting = CoreObject.FindChilds(CoreDefine.Entities.تنظیمات_عمومی);
            if (_PublicSetting.Count == 0)
            {
                GetPublicSeeting();
                _PublicSetting = CoreObject.FindChilds(CoreDefine.Entities.تنظیمات_عمومی);
            }
            List<CoreObject> AdminSetting = CoreObject.FindChilds(CoreDefine.Entities.تنظیمات_مدیر_سیستم);
            if (AdminSetting.Count == 0)
            {
                GetPublicSeeting();
                AdminSetting = CoreObject.FindChilds(CoreDefine.Entities.تنظیمات_مدیر_سیستم);
            }
            Referral.PublicSetting = new PublicSetting(_PublicSetting[0]);
            Referral.AdminSetting = new AdminSetting(AdminSetting[0]);

            if (Referral.UserAccount != null)
            {
                bool _Allowed = false;
                string _Message = "";
                UserAccount _UserAccount = new UserAccount();
                _UserAccount.IsFindUser(Referral.UserAccount.UserName, Referral.UserAccount.Password, ref _Allowed,ref _Message);
                if (_UserAccount.UsersID > 0)
                    Referral.UserAccount = _UserAccount;
            }
            Log.LogFunction("Software.CoreReload", false);
        }

        public static void JustCoreReload()
        {
            try
            {
                DataTable CoreData = Referral.DBCore.SelectDataTable("SELECT CoreObjectID, ParentID, Entity, Folder, FullName, OrderIndex, IsDefault, value  FROM CoreObject where Entity <> N'منبع_گزارش' and  Entity <> N'تنظیمات_مدیر_سیستم'  and  Entity <> N'تنظیمات_عمومی'  order by OrderIndex");
                if (Referral.CoreObjects != null)
                {
                    if(Referral.CoreObjects.Count > 2)
                    {
                        for (int index = Referral.CoreObjects.Count-1; index>1;index--)
                        {
                            Referral.CoreObjects.RemoveAt(index);
                        }
                    }
                    //Referral.CoreObjects.Clear();
                    //GetPublicSeeting();
                }

                foreach (DataRow Row in CoreData.Rows)
                {
                    Referral.CoreObjects.Add(new CoreObject(
                                Convert.ToInt64(Row["CoreObjectID"].ToString()),
                                Convert.ToInt64(Row["ParentID"].ToString()),
                                Tools.GetEntity(Row["Entity"].ToString()),
                                Row["Folder"].ToString(),
                                Row["FullName"].ToString(),
                                Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                                (bool)Row["IsDefault"],
                                (object)Row["Value"]
                            ));
                }

                //Thread child = new Thread(() => ThreadCoreReload());
                //child.Start();

            }
            catch (Exception ex)
            {
                Log.Error("Software.JustCoreReload", ex.Message);
            }
        }


        public static void ThreadCoreReload()
        {
            Log.LogFunction("Software.ThreadCoreReload", true);
            DataTable CoreData = Referral.DBCore.SelectDataTable("SELECT  CoreObjectID, ParentID, Entity, Folder, FullName, OrderIndex, IsDefault, value FROM  CoreObject where Entity = N'منبع_گزارش'  order by OrderIndex");
            foreach (DataRow Row in CoreData.Rows)
            {
                int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == long.Parse(Row["CoreObjectID"].ToString()));
                if (CoreIndex < 0)
                {
                    Referral.CoreObjects.Add(new CoreObject(
                       Convert.ToInt64(Row["CoreObjectID"].ToString()),
                       Convert.ToInt64(Row["ParentID"].ToString()),
                       Tools.GetEntity(Row["Entity"].ToString()),
                       Row["Folder"].ToString(),
                       Row["FullName"].ToString(),
                       Convert.ToInt64(Row["OrderIndex"].ToString() == "" ? "0" : Row["OrderIndex"].ToString()),
                       (bool)Row["IsDefault"],
                       (object)Row["Value"]
                        ));
                }
                else
                    Referral.CoreObjects[CoreIndex].Value = Row["value"];
            }
            Log.LogFunction("Software.ThreadCoreReload", false);
        }
    }
}