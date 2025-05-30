using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public partial class PublicSetting
    {
        public string CompanyName { get; set; }
        public string AppPersianName { get; set; }
        public string AppEnglishName { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public string E_Maile { get; set; }
        public string EMaileUserName { get; set; }
        public string EMailePassWord { get; set; }
        public string EMaileServer { get; set; }
        public string EMailePort { get; set; }
        public bool EnableSsl { get; set; }

        public string BrandSlogan { get; set; }
        public string AppLogo { get; set; }
        public string MainColor { get; set; }
        public string IconColor { get; set; }
        public string TitleColor { get; set; }
        public string IntroduceOthers { get; set; }
        public string Laws { get; set; }
        public string UserPrivacy { get; set; }
        public string FileSavingPath { get; set; }
        public bool SaveFileValueInDataBase { get; set; }
        public string LoginBackgroundImage { get; set; }
        public string HeaderLoginBackgroundImage { get; set; }
        public string HomeBackgroundImage { get; set; }
        public string GeneralMessage { get; set; }
        public string CSS { get; set; }
        public string JS { get; set; }
        public long RelatedWebService { get; set; }
        public long ChangePasswordDays { get; set; }
        public string SupportLink { get; set; }
        public int DataRecoveryMinutes { get; set; }

        public PublicSetting(string CompanyName, string AppPersianName, string AppEnglishName, string BrandSlogan, string AppLogo, string MainColor,
            string IconColor, string TitleColor, string IntroduceOthers, string Laws, string UserPrivacy, string FileSavingPath, string LoginBackgroundImage,
            string HeaderLoginBackgroundImage, string HomeBackgroundImage)
        {
            this.CompanyName = CompanyName;
            this.AppPersianName = AppPersianName;
            this.AppEnglishName = AppEnglishName;
            this.BrandSlogan = BrandSlogan;
            this.AppLogo = AppLogo;
            this.MainColor = MainColor;
            this.IconColor = IconColor;
            this.TitleColor = TitleColor;
            this.IntroduceOthers = IntroduceOthers;
            this.Laws = Laws;
            this.UserPrivacy = UserPrivacy;
            this.FileSavingPath = FileSavingPath;
            this.LoginBackgroundImage = LoginBackgroundImage;
            this.HeaderLoginBackgroundImage = HeaderLoginBackgroundImage;
            this.HomeBackgroundImage = HomeBackgroundImage;
        }
        public PublicSetting()
        {
            this.EnableSsl = true;
            RelatedWebService = 0;
            ChangePasswordDays = 30;
            SupportLink = "";
            DataRecoveryMinutes = 5;
        }
        public PublicSetting(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(PublicSetting));
            var FieldInfo = serializer.Deserialize(stringReader) as PublicSetting;
            this.CompanyName = FieldInfo.CompanyName;
            this.AppPersianName = FieldInfo.AppPersianName;
            this.AppEnglishName = FieldInfo.AppEnglishName;
            this.BrandSlogan = FieldInfo.BrandSlogan;
            this.AppLogo = FieldInfo.AppLogo;
            this.MainColor = FieldInfo.MainColor;
            this.IconColor = FieldInfo.IconColor;
            this.TitleColor = FieldInfo.TitleColor;
            this.IntroduceOthers = FieldInfo.IntroduceOthers;
            this.Laws = FieldInfo.Laws;
            this.UserPrivacy = FieldInfo.UserPrivacy;
            this.FileSavingPath = FieldInfo.FileSavingPath;
            this.SaveFileValueInDataBase = false;
            this.LoginBackgroundImage = FieldInfo.LoginBackgroundImage;
            this.HeaderLoginBackgroundImage = FieldInfo.HeaderLoginBackgroundImage;
            this.HomeBackgroundImage = FieldInfo.HomeBackgroundImage;
            this.GeneralMessage = FieldInfo.GeneralMessage;
            this.CSS = FieldInfo.CSS;
            this.JS = FieldInfo.JS;
            this.WebSite = FieldInfo.WebSite;
            this.PhoneNumber = FieldInfo.PhoneNumber;
            this.E_Maile = FieldInfo.E_Maile;
            this.EMaileUserName = FieldInfo.EMaileUserName;
            this.EMailePassWord = FieldInfo.EMailePassWord;
            this.EMailePort = FieldInfo.EMailePort;
            this.EMaileServer = FieldInfo.EMaileServer;
            this.EnableSsl = FieldInfo.EnableSsl;
            this.RelatedWebService = FieldInfo.RelatedWebService;
            this.ChangePasswordDays = FieldInfo.ChangePasswordDays;
            this.SupportLink = FieldInfo.SupportLink;
            this.DataRecoveryMinutes = FieldInfo.DataRecoveryMinutes;
        }
    }
}