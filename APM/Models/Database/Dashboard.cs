using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public class Dashboard
    {
        public string FullName { get; set; }
        public long CoreObjectID { get; set; }
        public long InformationEntryForm { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool ShowDate { get; set; }
        public Dashboard()
        {

        }
        public Dashboard(CoreObject _CoreObject)
        {
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(Dashboard));
            var DashboardInfo = serializer.Deserialize(stringReader) as Dashboard;
            this.InformationEntryForm = DashboardInfo.InformationEntryForm;
            this.StartDate = DashboardInfo.StartDate;
            this.EndDate = DashboardInfo.EndDate;
            this.ShowDate = DashboardInfo.ShowDate;
        }
    }

    public class SubDashboard
    {
        public string FullName { get; set; }
        public long CoreObjectID { get; set; }
        public CoreDefine.ChartTypes ChartTypes { get; set; }
        public string Title { get; set; }
        public string CategoryAxisQuery { get; set; }
        public string Condition { get; set; }
        public bool IsWide { get; set; }
        public long GroupField { get; set; }
        public long DateField { get; set; }
        public long ParentID { get; set; }
        public int ReloadTime { get; set; }
        public CoreDefine.ChartGroupDate ChartGroupDateType { get; set; }
        public CoreDefine.ChartCalculationType ChartCalculationType { get; set; }
        public int ColumnSpan { get; set; }
        public int RowSpan { get; set; }
        public long MaxValue { get; set; }
        public long IndicatorValue { get; set; }
        public long MinValue { get; set; }
        public long InformationEntryForm { get; set; }
        public long CalculationField { get; set; }
        public string Theme { get; set; }
        public string GroupByQuery { get; set; }
        public string OrderByQuery { get; set; }
        public string Icon { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public SubDashboard()
        {
            GroupField = 0;
            DateField = 0;
            ReloadTime=0;
            ChartGroupDateType = CoreDefine.ChartGroupDate.هیچکدام;
            ChartCalculationType = CoreDefine.ChartCalculationType.هیچکدام;
            ColumnSpan=2;
            RowSpan=2;
            MaxValue=0;
            IndicatorValue=0;
            MinValue=0;
            InformationEntryForm = 0;
            CalculationField = 0;
            Theme = "default";
            GroupByQuery= String.Empty;
            OrderByQuery = String.Empty;
            Icon = "k-icon k-i-chart-column-stacked100";
            BackgroundColor = "#ffffff";
            TextColor = "#000000";
        }
        public SubDashboard(CoreObject _CoreObject)
        {
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ParentID = _CoreObject.ParentID;
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(SubDashboard));
            var SubDashboardInfo = serializer.Deserialize(stringReader) as SubDashboard;
            this.ChartTypes = SubDashboardInfo.ChartTypes; 
            this.CategoryAxisQuery = SubDashboardInfo.CategoryAxisQuery; 
            this.IsWide = SubDashboardInfo.IsWide; 
            this.Title = SubDashboardInfo.Title; 
            this.Condition = SubDashboardInfo.Condition; 
            this.GroupField = SubDashboardInfo.GroupField; 
            this.DateField = SubDashboardInfo.DateField; 
            this.ReloadTime = SubDashboardInfo.ReloadTime; 
            this.ChartGroupDateType = SubDashboardInfo.ChartGroupDateType; 
            this.ChartCalculationType = SubDashboardInfo.ChartCalculationType; 
            this.ColumnSpan = SubDashboardInfo.ColumnSpan; 
            this.RowSpan = SubDashboardInfo.RowSpan; 
            this.MaxValue = SubDashboardInfo.MaxValue;
            this.MinValue = SubDashboardInfo.MinValue;  
            this.IndicatorValue = SubDashboardInfo.IndicatorValue;
            this.InformationEntryForm = SubDashboardInfo.InformationEntryForm;
            this.CalculationField = SubDashboardInfo.CalculationField;
            this.Theme = SubDashboardInfo.Theme;
            this.GroupByQuery = SubDashboardInfo.GroupByQuery;
            this.OrderByQuery = SubDashboardInfo.OrderByQuery;
            this.Icon = SubDashboardInfo.Icon;
            this.BackgroundColor = SubDashboardInfo.BackgroundColor;
            this.TextColor = SubDashboardInfo.TextColor;
        }
    }

    public class DashboardIntegration
    {
        public string FullName { get; set; }
        public long CoreObjectID { get; set; }
        public long GroupField { get; set; }
        public CoreDefine.ChartCalculationType ChartCalculationType { get; set; }
        public string CategoryAxisQuery { get; set; }
        public string Condition { get; set; } 
        public DashboardIntegration()
        {
            GroupField = 0;
            Condition =String.Empty;
            CategoryAxisQuery = String.Empty;

        }
        public DashboardIntegration(CoreObject _CoreObject)
        {
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(DashboardIntegration));
            var DataSourceDashboardInfo = serializer.Deserialize(stringReader) as DashboardIntegration; 
            this.GroupField = DataSourceDashboardInfo.GroupField;
            this.Condition = DataSourceDashboardInfo.Condition; 
            this.CategoryAxisQuery = DataSourceDashboardInfo.CategoryAxisQuery;
            this.ChartCalculationType = DataSourceDashboardInfo.ChartCalculationType;
        }
    }
}