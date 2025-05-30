using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json.Linq;
using APM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using APM.Models.Database;
using APM.Models.Tools;
using System;
using System.Data;

namespace APM.Controllers
{
    public class OrgChartController: Controller
    {
        // GET: OrgChart
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read(string _DataKey,int _ParentId)
        { 
            var source = new List<OrgChartView>();

            string TableName = CoreObject.Find(Desktop.DataInformationEntryForm[_DataKey].RelatedTable).FullName;

            DataSourceInfo DataSourceInfo = new DataSourceInfo(CoreObject.Find(CoreObject.Find(Desktop.DataInformationEntryForm[_DataKey].RelatedTable).ParentID));
            long RegisterCounter = DataConvertor.RegisterCount(TableName, DataSourceInfo.ServerName,DataSourceInfo.DataBase);

            if (Session["OrgChart" + _DataKey] == null || Desktop.CachedTable[_DataKey] is null || _ParentId != 0 || Desktop.RegisterdTableID[_DataKey] != RegisterCounter)
            {
                DataTable OrgData = Desktop.Read(_DataKey, "", _ParentId, (int)Desktop.DataInformationEntryForm[_DataKey].ShowRecordCountDefault);
                foreach (DataRow Row in OrgData.Rows)
                {
                    byte[] FileByte = Desktop.DataInformationEntryForm[_DataKey].ChartAvatar == "" ? new byte[0] : (byte[])Row[Desktop.DataInformationEntryForm[_DataKey].ChartAvatar];
                    OrgChartView Parameter = new OrgChartView();

                    Parameter.Id = (long)Row[Desktop.DataInformationEntryForm[_DataKey].ChartID];
                    Parameter.Name = (string)Row[Desktop.DataInformationEntryForm[_DataKey].ChartName];
                    Parameter.Title = Desktop.DataInformationEntryForm[_DataKey].ChartTitle == "" ? "" : (string)Row[Desktop.DataInformationEntryForm[_DataKey].ChartTitle];
                    Parameter.Group = Desktop.DataInformationEntryForm[_DataKey].ChartGroup == "" ? "" : (string)Row[Desktop.DataInformationEntryForm[_DataKey].ChartGroup];
                    Parameter.Expanded = true;
                    Parameter.ParentId = (long)Row[Desktop.DataInformationEntryForm[_DataKey].ChartParentID];
                    Parameter.Avatar = Field.FormatImage(FileByte.Length == 0 ? Referral.PublicSetting.AppLogo : Convert.ToBase64String((byte[])FileByte));
                    source.Add(Parameter);

                }
                Desktop.CachedTable[_DataKey] = OrgData;
                Desktop.RegisterdTableID[_DataKey] = RegisterCounter;
                Session["OrgChart" + _DataKey] = source;
            }
            else
                source = (List<OrgChartView>)Session["OrgChart" + _DataKey];
            return Json(source);
        }

        public JsonResult DownloadChart(long DataKey,long ParentId,string FormatType)
        {
            CoreObject coreObject = CoreObject.Find(DataKey);
            Referral.DBData.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "IP", "Version", "Source", "BrowserType", "BrowserVersion" }
                                                         , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID,coreObject.CoreObjectID, coreObject.Entity.ToString(), coreObject.FullName, FormatType, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });
            return Json(coreObject.FullName);
        }
    }
}