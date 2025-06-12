using APM.Models.Security;
using APM.Models.Tools;
using System;
using System.Data;
using System.IO;
using System.Web;

namespace APM.Models
{

    public static class Login
    {
        #region " Methods "

        public static void UserAuthentication(string _UserName, string _Password, ref bool _Allowed, ref string _Message)
        {
            Log.LogFunction("Login.UserAuthentication", true);
            _Allowed = false;
            _Message = "";

            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHn0s4gy0Fr5YoUZ9V00Y0igCSFQzwEqYBh/N77k4f0fWXTHW5rqeBNLkaurJDenJ9o97TyqHs9HfvINK18Uwzsc/bG01Rq+x3H3Rf+g7AY92gvWmp7VA2Uxa30Q97f61siWz2dE5kdBVcCnSFzC6awE74JzDcJMj8OuxplqB1CYcpoPcOjKy1PiATlC3UsBaLEXsok1xxtRMQ283r282tkh8XQitsxtTczAJBxijuJNfziYhci2jResWXK51ygOOEbVAxmpflujkJ8oEVHkOA/CjX6bGx05pNZ6oSIu9H8deF94MyqIwcdeirCe60GbIQByQtLimfxbIZnO35X3fs/94av0ODfELqrQEpLrpU6FNeHttvlMc5UVrT4K+8lPbqR8Hq0PFWmFrbVIYSi7tAVFMMe2D1C59NWyLu3AkrD3No7YhLVh7LV0Tttr/8FrcZ8xirBPcMZCIGrRIesrHxOsZH2V8t/t0GXCnLLAWX+TNvdNXkB8cF2y9ZXf1enI064yE5dwMs2fQ0yOUG/xornE";
            var path = System.Web.HttpContext.Current.Server.MapPath("~/Views/Shared/Report/license.key");
            Stimulsoft.Base.StiLicense.LoadFromFile(path);

            UserAccount _UserAccount = new UserAccount();
            if (_UserAccount.IsFindUser(_UserName, _Password, ref _Allowed,ref _Message))
            {
                Referral.UserAccount = _UserAccount;
                _Allowed = true;
                //Referral.DBRegistry.Insert("Login_APMRegistry", new string[] { "UserAccountID", "LoginDate", "LoginTime", "UnsuccessfulLogin", "IsActive", "IP", "PCName", "Version", "BrowserType", "BrowserVersion" }, new object[] { _UserAccount.UsersID, _UserAccount.LoginDate, _UserAccount.LoginTime, HttpContext.Current.Session["UnsuccessfulLogin"], 1, _UserAccount.IP, _UserAccount.PCName, Referral.AppVersion, _UserAccount.BrowserType, _UserAccount.BrowserVersion });
            }
            else
                if(_Message=="")
                    _Message = "نام کاربری یا رمز عبور صحیح نیست";

            Log.LogFunction("Login.UserAuthentication", false);
        }

        public static bool ConnectToDB()
        {
            Log.LogFunction("Login.ConnectToDB", true); 
            string DBUser = "sa";
            string DBPassword = "maysasys";
            string Source = ".";
            string DataBaseName =string.Empty;
            //Referral.CustomerName = Software.Customers.RaminPower;
            Referral.CustomerName = Software.Customers.MaroonPetrochemical;
            //Referral.CustomerName = Software.Customers.khuareeo;
            //Referral.CustomerName = Software.Customers.AghajariOilAndGas;
            //Referral.CustomerName = Software.Customers.ShoshtarPetrochemical;
            //Referral.CustomerName = Software.Customers.Farabintav;
            //Referral.CustomerName = Software.Customers.CloudLand;
            //Referral.CustomerName = Software.Customers.MaysaSoftware;
            //Referral.CustomerName = Software.Customers.ShoraAhvaz;
            //Referral.CustomerName = Software.Customers.SalmanFarsiPetrochemical;
            //Referral.CustomerName = Software.Customers.ParkElmoFanavariKhozestan;
            //Referral.CustomerName = Software.Customers.IranNasr;
            //Referral.CustomerName = Software.Customers.TondgoyanPetrochemical;

            switch (Referral.CustomerName)
            {
                case Software.Customers.RaminPower:
                    {
                        DBUser = "sa";
                        DBPassword = "u+3Tx)1-,Vl$";
                        Source = "192.168.2.48";
                        DataBaseName = "OfficialRaminPower";
                        break;
                    }
                case Software.Customers.ParkElmoFanavariKhozestan:
                    {
                        Source = "185.55.224.4"; 
                        DBUser = "maysaso1_sa"; 
                        DBPassword = "1iSd2i0_7";
                        DataBaseName = "maysaso1_KhstpOfficial";
                        break;
                    }
                case Software.Customers.khuareeo:
                    {
                        Source = "185.55.224.4"; 
                        DBUser = "maysaso1_sa"; 
                        DBPassword = "1iSd2i0_7";
                        DataBaseName = "maysaso1_khuareeo";
                        break;
                    }
                case Software.Customers.IranNasr:
                    {
                        Source = "185.55.224.4"; 
                        DBUser = "maysaso1_sa"; 
                        DBPassword = "1iSd2i0_7";
                        DataBaseName = "maysaso1_Irannsr";
                        break;
                    }
                case Software.Customers.TondgoyanPetrochemical:
                    {
                        //Source = "185.55.224.4"; 
                        //DBUser = "maysaso1_sa"; 
                        //DBPassword = "1iSd2i0_7";
                        //DataBaseName = "maysaso1_STPCSalariesWagesContractors";

       

                        DataBaseName = "SalamanPCProtection";
                        break;
                    }
                case Software.Customers.CloudLand:
                    {
                        DBUser = "sa";
                        DBPassword = "1iSd2i0_7";
                        Source = "10.1.0.16";
                        DataBaseName = "CloudLand";
                        break;
                    }
                case Software.Customers.Farabintav:
                    {

                        DBUser = "farabint_sa";
                        DBPassword = "oM4g0k&71";
                        Source = "31.25.91.22";
                        DataBaseName = "farabint_Tax"; 
                        break;
                    }
                case Software.Customers.AghajariOilAndGas:
                    {
                        //Source = "172.21.21.4";
                        //////Source = "172.21.21.85";
                        //DBUser = "sa";
                        //DBPassword = "Pp@#$123654@#$";

                        //Source = "mssql.maysaservice.ir"; 
                        //DBUser = "sa";
                        //DBPassword = "Kabinet95##";

                        DataBaseName = "NisocWelfareService";


                        //Source = "172.21.52.17";
                        //DBUser = "sa";
                        //DBPassword = ":!oJqlTVej_9";
                        //DataBaseName = "NisocProtection";


                        //Source = "192.168.1.3";
                        ////Source = "192.168.1.2";
                        //DBUser = "sa";
                        //DBPassword = "Nn@#$123Ag$#@567";
                        //DataBaseName = "NisocDCS";


                        //Source = "172.21.21.87";
                        //////Source = "172.21.21.85";
                        //DBUser = "sa";
                        //DBPassword = "Mp$@gh1403";
                        //DataBaseName = "NisocPM";

                        break;
                    }
                case Software.Customers.MaroonPetrochemical:
                    {
                        //DBUser = "sa";
                        //Source = "10.1.11.71";
                        //DBPassword = "u+3Tx)1-,Vl$";

                        //Source = @"172.20.3.40\maysa";
                        //Source = @"172.20.3.40";
                        //Source = "172.20.3.52";
                        //DBPassword = "Mbs@123456";

                        DataBaseName = "MPCProtection";

                        ////Source = "185.55.224.4";
                        //DBUser = "maysaso1_sa";
                        //DBPassword = "1iSd2i0_7";
                        //DataBaseName = "maysaso1_MPCProtectionData";
                        break;
                    }
                case Software.Customers.MaysaSoftware:
                    {  
                        Source = "185.55.224.4";
                        //Source = "185.55.224.183";
                        DBUser = "maysaso1_sa";
                        //DataBaseName = "maysaso1_LetterWriting";
                        DBPassword = "1iSd2i0_7";
                        DataBaseName = "maysaso1_MSCOfficial";

                        break;
                    }
                case Software.Customers.ShoraAhvaz :
                    {  
                        Source = "185.55.224.4";
                        DBUser = "maysaso1_sa"; 
                        DBPassword = "1iSd2i0_7";

                        DataBaseName = "maysaso1_ShoraAhvaz";

                        break;
                    }
                case Software.Customers.SalmanFarsiPetrochemical:
                    {
                        Source = "192.168.1.15";
                        DBUser = "sa";
                        DBPassword = "$@lM@n14o3";

                        //DBUser = "sa";
                        //DBPassword = "1iSd2i0_7";
                        //Source = "10.1.0.16";
                        DataBaseName = "SalamanPCProtection";

                        break;
                    }
                case Software.Customers.ShoshtarPetrochemical:
                    {
                        //Source = "192.168.100.28";
                        //DBUser = "sa";
                        //DBPassword = "$h0$Ht@r1403";
                        //DataBaseName = "ShpcRepairs";
                        DataBaseName = "ShpcProductionControl";

                        break;
                    }
            }

            //string DBPassword = "u+3Tx)1-,Vl$";
            //string DataBaseName = "MPCQualityGuarantee";

            //string Source = "185.55.224.148";
            //string DBUser = "alofixne_sa";
            //string DBPassword = "l2U4i4@x";
            //string DataBaseName = "alofixne_LetterWriting";
            //Referral.CustomerName = Software.Customers.PayaAccountant;

            //string Source = "185.55.224.4";
            //string DBUser = "maysaso1_LetterWriting";
            //string DBPassword = "z6#O6h1t7";
            //string DataBaseName = "maysaso1_LetterWriting";
            //Referral.CustomerName = Software.Customers.AreyanSanaat;


            ////string DBUser = "maysaso1_MPCProtection";
            ////string DBPassword = "8h5t9a6^N";
            ////string Source = "185.55.224.4";
            ////string DataBaseName = "maysaso1_MPCProtection";

            //string DBUser = "sa";
            //string Source = "192.168.128.63";
            //string DBPassword = "u+3Tx)1-,Vl$";
            //string DataBaseName = "AmirKabirSuggest";
            //Referral.CustomerName = Software.Customers.AmirkabirPetrochemical;


            //string DataBaseName = "SadaretBankLegal";
            //Referral.CustomerName = Software.Customers.SadaretBank;


            //string DBUser = "maysaso1_sa";
            //string DBPassword = "1iSd2i0_7";
            //string Source = "185.55.224.4";
            //string DataBaseName = "maysaso1_PetzoneProtection";

            //string DBUser = "Maysa";
            //string DBPassword = "P@ssw0rde";
            //string Source = "172.16.41.161";
            //string Source = "172.16.41.249";
            //string Source = ".";

            //string DataBaseName = "MaysaPetzoneProtection";
            //Referral.CustomerName = Software.Customers.Petzone;
            //string DataBaseName = "maysaso1_welfareSevicesArvandFreeZone";
            //Referral.CustomerName = Software.Customers.ArvandFreeZone;
            //////////string Source = "185.55.224.183";
            ////////////////////string DataBaseName = "maysaso1_MPCProtection"; 
            //////////string DataBaseName = "maysaso1_MPCProtection";  



            //string DataBaseName = "maysaso1_petrofan";
            //Referral.CustomerName = Software.Customers.Petrofan;

            //string DBPassword = "N@@R1401";
            //string Source = "192.168.1.10";
            //string DataBaseName = "NoorProtection";
            //Referral.CustomerName = Software.Customers.NoorTrading;
            //string DataBaseName = "PKGSOfficial";
            //Referral.CustomerName = Software.Customers.SetareTalaeiKhaligFars;

            //string Source = "185.55.224.148";
            //string DBUser = "alofixne_sa";
            //string DBPassword = "l2U4i4@x";
            //string DataBaseName = "alofixne_AmirkabirRisc";
            //Referral.CustomerName = Software.Customers.PersianGulfGoldenStar;


            //string Source = "185.55.224.56";
            //string DBUser = "yektapa3_yektapardazeshparsOfficial";
            //string DBPassword = "S#0tfd196";
            //string DataBaseName = "yektapa3_yektapardazeshparsOfficial";
            //Referral.CustomerName = Software.Customers.YektaPardazeshPars;



            Referral.DBData = new SQLDataBase(Source, DataBaseName + "Data", DBPassword, DBUser, SQLDataBase.SQLVersions.SQL2008);
            Referral.DBRegistry = new SQLDataBase(Source, DataBaseName + "Registry", DBPassword, DBUser, SQLDataBase.SQLVersions.SQL2008);
            Referral.DBAttachment = new SQLDataBase(Source, DataBaseName + "Attachment", DBPassword, DBUser, SQLDataBase.SQLVersions.SQL2008);
            Referral.DBCore = new SQLDataBase(Source, DataBaseName + "Core", DBPassword, DBUser, SQLDataBase.SQLVersions.SQL2008);
            Referral.DBMsdb = new SQLDataBase(Source, "msdb", DBPassword, DBUser, SQLDataBase.SQLVersions.SQL2008);

            //////////////////////////////////////////SQLDataBase sQLDataBaseCoreLocal = new SQLDataBase(".", DataBaseName + "Core", "maysasys", DBUser, SQLDataBase.SQLVersions.SQL2008);
            //////////////////////////////////////////bool isrun = Referral.DBCore.UpdateRow(21985, 0, "CoreObject", new string[] { "Value" }, new object[] { sQLDataBaseCoreLocal.SelectField("SELECT Value  FROM CoreObject  where   CoreObjectID =21985") });

            ////SQLDataBase sQLDataBaseCore1 = new SQLDataBase(@"172.20.3.40\maysa", DataBaseName + "Attachment", "Mbs@123456", DBUser, SQLDataBase.SQLVersions.SQL2008);
            ////SQLDataBase sQLDataBaseCoreData = new SQLDataBase(@"172.20.3.40\maysa", DataBaseName + "Data", "Mbs@123456", DBUser, SQLDataBase.SQLVersions.SQL2008);

            if (Referral.DBCore.Open())
            {
                Referral.DBCore.Close();

                //if (Referral.PublicSetting == null)
                //    Software.GetPublicSeeting();

                ////bool _Allowed = false;
                ////UserAccount _UserAccount = new UserAccount("مایسا", "1234", ref _Allowed);

                ////Referral.UserAccount = _UserAccount;
                //////long MaxID = long.Parse(Referral.DBAttachment.SelectField("Select isnull( Max(PashaID2),0) From CoreObjectAttachment").ToString());
                ////long MaxID = 0;
                ////long PashaMaxID = long.Parse(sQLDataBaseCore1.SelectField("Select isnull( Max(CoreObjectAttachmentID),0) From MPCProtectionAttachment.dbo.CoreObjectAttachment").ToString());
                ////for (long index = MaxID; MaxID < PashaMaxID; index = +10)
                ////{
                ////    DataTable dataTable = sQLDataBaseCore1.SelectDataTable("SELECT top 10 CoreObjectAttachmentID, RecordID, InnerID, Folder, FullName, Extension, Size, Value, IsDeleted, Thumbnail, OrderColumn  FROM MPCProtectionAttachment.dbo.CoreObjectAttachment Where" +
                ////        " (RecordID=183 Or  RecordID=1251 Or  RecordID=156 Or  RecordID=223 Or  RecordID=2005 Or  RecordID=2128  " +
                ////        "Or  RecordID=215  Or  RecordID=2138   Or  RecordID=43  Or  RecordID=210  Or  RecordID=1346" +
                ////    "  Or  RecordID=1975   Or  RecordID=8   Or  RecordID=307   Or  RecordID=1201   Or  RecordID=190  Or  RecordID=2147 Or  RecordID=228 Or  RecordID=1650)   and TempID is null order by CoreObjectAttachmentID");


                ////    foreach (DataRow row in dataTable.Rows)
                ////    {
                ////        string RecordID = "";
                ////        string InnerID = row["InnerID"].ToString();
                ////        switch ((long)row["RecordID"])
                ////        {
                ////            case 183: RecordID = "11500"; break;
                ////            case 1251: RecordID = "980"; break;
                ////            case 156: RecordID = "337"; break;
                ////            case 223: RecordID = "11533"; break;
                ////            case 2005: RecordID = "21818"; break;
                ////            case 2128: RecordID = "21683"; break;
                ////            case 215: RecordID = "11521"; break;
                ////            case 2138:
                ////                {
                ////                    RecordID = "11521";
                ////                    InnerID = Referral.DBData.SelectField("SELECT  شناسه    FROM  مجوز_صدور_کارت  where  نوع_قرارداد_شغلی= 6 and شماره_مجوز = N'" + InnerID + "'").ToString();
                ////                }
                ////                break;
                ////            case 560: RecordID = "109"; break;
                ////            case 43: RecordID = "309"; break;
                ////            case 210: RecordID = "11517"; break;
                ////            case 1346: RecordID = "923"; break;
                ////            case 1975: RecordID = "21938"; break;
                ////            case 8: RecordID = "882"; break;
                ////            case 307: RecordID = "959"; break;
                ////            case 1201: RecordID = "353"; break;
                ////            case 190: RecordID = "21768"; break;
                ////            case 2147: RecordID = "21787"; break;
                ////            case 228: RecordID = "21636"; break;
                ////            case 1650:
                ////                {
                ////                    RecordID = "109";
                ////                    InnerID = sQLDataBaseCoreData.SelectField("SELECT  شخص  FROM مدارک_شخص where شناسه = " + InnerID).ToString();
                ////                }
                ////                break;
                ////        }
                ////        //string pathFile = Referral.PublicSetting.FileSavingPath + "CoreObjectAttachment\\" + RecordID + "\\" + row["InnerID"].ToString() + "\\";
                ////        string pathFile = @"\\172.20.3.40\MaysaCompany\Secret\CoreObjectAttachment\\" + RecordID + "\\" + InnerID + "\\";

                ////        if (!Directory.Exists(pathFile))
                ////        {
                ////            Directory.CreateDirectory(pathFile);
                ////        }

                ////        long MaysaID = 0;
                ////        if (!File.Exists(pathFile + "/" + row["FullName"].ToString() + "." + row["Extension"].ToString()))
                ////        {
                ////            byte[] imageBytes = (byte[])row["Value"];
                ////            string Text = Convert.ToBase64String(imageBytes);
                ////            System.IO.File.WriteAllText(pathFile + "/" + row["FullName"].ToString() + "." + row["Extension"].ToString(), Text);

                ////            byte[] ThumbnailImageBytes = Attachment.CreateThumbnailImage(pathFile + "/" + row["FullName"].ToString() + "." + row["Extension"].ToString());
                ////            if (row["FullName"].ToString() == "عکس" || row["FullName"].ToString() == "چهره" || row["FullName"].ToString() == "امضاء" || row["FullName"].ToString() == "امضا")
                ////                MaysaID = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL", "Value", "PashaID2" }
                ////                                                             , new object[] { RecordID, InnerID, row["Folder"].ToString(), row["FullName"].ToString(), row["Extension"].ToString(), imageBytes.Length, ThumbnailImageBytes, pathFile.Replace("/", "\\").Replace(@"\\", "\\").Replace(@"\\172.20.3.40", "D:"), imageBytes, row["CoreObjectAttachmentID"].ToString() });
                ////            else
                ////                MaysaID = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "Thumbnail", "URL", "PashaID2" }
                ////                                                              , new object[] { RecordID, InnerID, row["Folder"].ToString(), row["FullName"].ToString(), row["Extension"].ToString(), imageBytes.Length, ThumbnailImageBytes, pathFile.Replace("/", "\\").Replace(@"\\", "\\").Replace(@"\\172.20.3.40", "D:"), row["CoreObjectAttachmentID"].ToString() });


                ////        }
                ////        else
                ////        {
                ////            object ob = Referral.DBAttachment.SelectField("Select CoreObjectAttachmentID from CoreObjectAttachment where RecordID =" + RecordID + " and InnerID = " + InnerID + " and FullName=N'" + row["FullName"].ToString() + "'");
                ////            if (ob != null)
                ////            {
                ////                MaysaID = long.Parse(ob.ToString());
                ////                Referral.DBAttachment.UpdateRow(MaysaID, 0, "CoreObjectAttachment", new string[] { "PashaID2" }, new object[] { row["CoreObjectAttachmentID"].ToString() });
                ////            }
                ////            else
                ////            {
                ////                byte[] imageBytes = (byte[])row["Value"];
                ////                //byte[] ThumbnailImageBytes = Attachment.CreateThumbnailImage(pathFile + "/" + row["FullName"].ToString() + "." + row["Extension"].ToString());
                ////                MaysaID = Referral.DBAttachment.Insert("CoreObjectAttachment", new string[] { "RecordID", "InnerID", "Folder", "FullName", "Extension", "Size", "URL", "PashaID2" }
                ////                                                              , new object[] { RecordID, InnerID, row["Folder"].ToString(), row["FullName"].ToString(), row["Extension"].ToString(), imageBytes.Length, pathFile.Replace("/", "\\").Replace(@"\\", "\\").Replace(@"\\172.20.3.40", "D:"), row["CoreObjectAttachmentID"].ToString() });
                ////            }

                ////        }
                ////        sQLDataBaseCore1.UpdateRow(long.Parse(row["CoreObjectAttachmentID"].ToString()), 0, "CoreObjectAttachment", new string[] { "TempID" }, new object[] { MaysaID });
                ////        MaxID = long.Parse(row["CoreObjectAttachmentID"].ToString());
                ////    }
                ////}

                if (Referral.PublicSetting == null)
                    Software.GetPublicSeeting();
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}