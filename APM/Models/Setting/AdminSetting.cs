using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Setting
{

    public partial class AdminSetting
    {
        public bool ShowUserRegistryInLoginForm { get; set; }
        public bool ShowDataSourceListInSettingForm { get; set; }
        public bool ShowSpecialPhraseListInSettingForm { get; set; }
        public bool ShowInformationEntryFormListInSettingForm { get; set; }
        public bool ShowProcessListInSettingForm { get; set; }
        public bool ShowReportListInSettingForm { get; set; }
        public bool ShowDashboardListInSettingForm { get; set; }
        public bool ShowPublicFileListInSettingForm { get; set; }
        public bool ShowConnectWebsiteListInSettingForm { get; set; }
        public bool ShowSMSSettingInSettingForm { get; set; }
        public bool ShowEmailSettingInSettingForm { get; set; }
        public bool ShowPaymentSettingInSettingForm { get; set; }
        public bool ShowPublicSettingInSettingForm { get; set; }
        public bool ShowUserCalendar { get; set; } 
        public bool ShowInfoInLogin { get; set; } 
        public bool ShowAllRights { get; set; }

        public bool ShowEditingRestrictions { get; set; }
        public bool ShowCanUpdateOnlyUserRegistry { get; set; }
        public bool ShowCanUpdateOneDey { get; set; }
        public bool ShowCanUpdateThreeDey { get; set; }
        public bool ShowCanUpdateOneWeek { get; set; }

        public bool PermissionShowImportExportInForm { get; set; }
        public bool ShowImportExportInAllForm { get; set; }
        public string[] AllowFormShowImportExport { get; set; }

        public bool PermissionShowCommentInForm { get; set; }
        public bool ShowCommentInAllForm { get; set; }
        public string[] AllowFormShowComment { get; set; }

        public bool PermissionShowEmailInForm { get; set; }
        public bool ShowEmailInAllForm { get; set; }
        public string[] AllowFormShowEmail { get; set; }
        public AdminSetting()
        {
            ShowUserRegistryInLoginForm = false;
            ShowDataSourceListInSettingForm = false;
            ShowSpecialPhraseListInSettingForm = false;
            ShowInformationEntryFormListInSettingForm = false;
            ShowProcessListInSettingForm = false;
            ShowReportListInSettingForm = false;
            ShowDashboardListInSettingForm = false;
            ShowPublicFileListInSettingForm = false;
            ShowConnectWebsiteListInSettingForm = false;
            ShowSMSSettingInSettingForm = false;
            ShowEmailSettingInSettingForm = true;
            ShowPaymentSettingInSettingForm = true;
            ShowPublicSettingInSettingForm = true;
            ShowUserCalendar = true;
            ShowInfoInLogin = false;
            ShowAllRights = false;

            ShowEditingRestrictions = false;
            ShowCanUpdateOnlyUserRegistry = false;
            ShowCanUpdateOneDey = false;
            ShowCanUpdateThreeDey = false;
            ShowCanUpdateOneWeek = false;

            PermissionShowImportExportInForm = false;
            ShowImportExportInAllForm = false;
            AllowFormShowImportExport = new string[] { };

            PermissionShowCommentInForm = false;
            ShowCommentInAllForm = false;
            AllowFormShowComment = new string[] { };

            PermissionShowEmailInForm = false;
            ShowEmailInAllForm = false;
            AllowFormShowEmail = new string[] { };
        }
        public AdminSetting(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(AdminSetting));
            var FieldInfo = serializer.Deserialize(stringReader) as AdminSetting;
            this.ShowUserRegistryInLoginForm = FieldInfo.ShowUserRegistryInLoginForm;

            //ShowUserRegistryInLoginForm = FieldInfo.ShowUserRegistryInLoginForm;
            //ShowDataSourceListInSettingForm = FieldInfo.ShowDataSourceListInSettingForm;
            //ShowSpecialPhraseListInSettingForm = FieldInfo.ShowSpecialPhraseListInSettingForm;
            //ShowInformationEntryFormListInSettingForm = FieldInfo.ShowInformationEntryFormListInSettingForm;
            //ShowProcessListInSettingForm = FieldInfo.ShowProcessListInSettingForm;
            //ShowReportListInSettingForm = FieldInfo.ShowReportListInSettingForm;
            //ShowDashboardListInSettingForm = FieldInfo.ShowDashboardListInSettingForm;
            //ShowPublicFileListInSettingForm = FieldInfo.ShowPublicFileListInSettingForm;
            //ShowConnectWebsiteListInSettingForm = FieldInfo.ShowConnectWebsiteListInSettingForm;
            //ShowSMSSettingInSettingForm = FieldInfo.ShowSMSSettingInSettingForm;
            //ShowEmailSettingInSettingForm = FieldInfo.ShowEmailSettingInSettingForm;
            //ShowPaymentSettingInSettingForm = FieldInfo.ShowPaymentSettingInSettingForm;
            //ShowPublicSettingInSettingForm = FieldInfo.ShowPublicSettingInSettingForm;
            //ShowUserCalendar = FieldInfo.ShowUserCalendar;
            ShowInfoInLogin = FieldInfo.ShowInfoInLogin;
            ShowAllRights = FieldInfo.ShowAllRights;

            ShowEditingRestrictions = FieldInfo.ShowEditingRestrictions;
            ShowCanUpdateOnlyUserRegistry = FieldInfo.ShowCanUpdateOnlyUserRegistry;
            ShowCanUpdateOneDey = FieldInfo.ShowCanUpdateOneDey;
            ShowCanUpdateThreeDey = FieldInfo.ShowCanUpdateThreeDey;
            ShowCanUpdateOneWeek = FieldInfo.ShowCanUpdateOneWeek; 

            PermissionShowImportExportInForm = FieldInfo.PermissionShowImportExportInForm;
            ShowImportExportInAllForm = FieldInfo.ShowImportExportInAllForm;
            AllowFormShowImportExport = FieldInfo.AllowFormShowImportExport;

            PermissionShowCommentInForm = FieldInfo.PermissionShowCommentInForm;
            ShowCommentInAllForm = FieldInfo.ShowCommentInAllForm;
            AllowFormShowComment = FieldInfo.AllowFormShowComment;

            PermissionShowEmailInForm = FieldInfo.PermissionShowEmailInForm;
            ShowEmailInAllForm = FieldInfo.ShowEmailInAllForm;
            AllowFormShowEmail = FieldInfo.AllowFormShowEmail;
        }
    }
}