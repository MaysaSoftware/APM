using APM.Models.APMObject;
using APM.Models.Security;
using APM.Models.Tools;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public partial class Report 
    { 
        public string FullName { get; set; }
        public string Icon { get; set; }
        public long CoreObjectID { get; set; }
        public long DataSourceID { get; set; }
        public bool ShowInMainMenu { get; set; }
        public bool ShowParameterInload { get; set; }
        public bool SelectedRow { get; set; }
        public string QueryAffterPrint { get; set; }
        public string QueryBeforRun { get; set; }
        public string PrinterName { get; set; }
        public int PrintCopy { get; set; }
        public string QueryPrintCopy { get; set; }
        public bool UseDefualtIconColor { get; set; }
        public string IconColor { get; set; }



        public Report()
        {
           this.FullName = "";
           this.CoreObjectID = 0;
           this.Icon = "";
           this.DataSourceID = 0;
            ShowInMainMenu = true;
            ShowParameterInload = true;
            SelectedRow = true;
            QueryAffterPrint=string.Empty;
            QueryBeforRun = string.Empty;
            PrintCopy = 1;
            PrinterName=String.Empty;
            QueryPrintCopy = String.Empty;
            this.UseDefualtIconColor = true;
            this.IconColor = Referral.PublicSetting.IconColor;
        } 
         
        public Report(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(Report));
            var ReportInfo = serializer.Deserialize(stringReader) as Report;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.Icon = ReportInfo.Icon;
            this.DataSourceID = ReportInfo.DataSourceID;
            this.ShowInMainMenu = ReportInfo.ShowInMainMenu;
            this.ShowParameterInload = ReportInfo.ShowParameterInload;
            this.SelectedRow = ReportInfo.SelectedRow;
            this.QueryAffterPrint = ReportInfo.QueryAffterPrint;
            this.QueryBeforRun = ReportInfo.QueryBeforRun;
            this.PrintCopy = ReportInfo.PrintCopy;
            this.PrinterName = ReportInfo.PrinterName;
            this.QueryPrintCopy = ReportInfo.QueryPrintCopy;
            this.IconColor = ReportInfo.IconColor;
            this.UseDefualtIconColor = ReportInfo.UseDefualtIconColor;
        }


        public StiReport Build(long ReportID, string[] ParameterName,string[] ParameterValue)
        {
            StiReport _StiReport = new StiReport();
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport(ReportID));
            Report Report = new Report(CoreObject.Find(_Report.ParentID));

            try
            {
                if (_Report.Value != null)
                    _StiReport.Load(_Report.Value);
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(_Report.ValueJS);
                    _StiReport.Load(byteArray);
                }

                string ConnectionString = string.Empty;
                if (Report.DataSourceID != 0)
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
                    ConnectionString = dataSourceInfo.ConnectionString;
                }
                else
                    ConnectionString = Referral.DBData.ConnectionData.ConnectionString;

                _StiReport.Dictionary.Databases[0] = new StiSqlDatabase("ارتباط", ConnectionString);

                if (_StiReport.Dictionary.Variables.Count > 0)
                {
                    List<CoreObject> ParameterList = CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش);
                    foreach (StiVariable Varible in _StiReport.Dictionary.Variables)
                    {
                        CoreObject ParameterCore=new CoreObject();
                        foreach (CoreObject ParameterItem in ParameterList)
                        {
                            if(ParameterItem.FullName == Tools.Tools.SafeTitle(Varible.Name))
                            {
                                ParameterCore = ParameterItem;
                                break;
                            }
                        } 
                        ReportParameter _ReportParameter = new ReportParameter(ParameterCore);
                        if (ParameterName !=null)
                        {
                            int FindeIndex = Array.IndexOf(ParameterName, _ReportParameter.FullName);
                            _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value = ParameterValue[FindeIndex];
                        }
                        else
                        {
                            if (_StiReport.Dictionary.Variables[_ReportParameter.FullName]!=null)
                                _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value = Tools.Tools.GetDefaultValue(_ReportParameter.Value.ToString());
                        }
                    }
                }

                List<string> FontNames =Tools.Tools.BFontNames();
                foreach (string FontItem in FontNames)
                {
                    string Font = FontItem.Replace(" ", "") + ".ttf";
                    byte[] FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Fonts/BFonts/" + Font));
                    StiResource Resource = new StiResource(
                        FontItem, Font, false, StiResourceType.FontTtf, FileByte, false);
                    _StiReport.Dictionary.Resources.Add(Resource);

                }

                List<string> IranSansFontNames = Tools.Tools.IranSansFontNames();
                foreach (string FontItem in IranSansFontNames)
                {
                    byte[] FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Fonts/IRANSans/ttf/" + FontItem + ".ttf"));
                    StiResource Resource = new StiResource(
                         FontItem.Replace("Web_", " "), FontItem + ".ttf", false, StiResourceType.FontTtf, FileByte, false);
                    _StiReport.Dictionary.Resources.Add(Resource);
                }
                _StiReport.Compile();
                _StiReport.Render(false);

            }
            catch (Exception ex)
            {
                Log.Error("Report.GetStiViewerReport", "\n" + ex.Message);
            }
            return _StiReport;
        }

    }
    public partial class ReportParameter
    {  
        public string FullName { get; set; } 
        public string ViewCommand { get; set; } 
        public string Value { get; set; }
        public string SpecialValue { get; set; }
        public long CoreObjectID { get; set; }
        public long RelatedTable { get; set; }
        public long RelatedField { get; set; }
        public long RelatedFieldCommand { get; set; }
        public int DigitsAfterDecimal { get; set; }
        public CoreDefine.InputTypes InputTypes { get; set; }   
        public bool IsLeftWrite { get; set; }
        public bool ActiveOnKeyDown { get; set; }
        public bool IsEditAble { get; set; }

        public ReportParameter()
        {
            this.Value = null; 
            this.FullName = string.Empty;
            this.ViewCommand = string.Empty;
            this.SpecialValue = string.Empty;
            this.CoreObjectID = 0;
            this.RelatedTable = 0;
            this.RelatedField = 0;
            this.RelatedFieldCommand = 0;
            this.InputTypes= new CoreDefine.InputTypes();
            this.DigitsAfterDecimal = 0;
            this.IsLeftWrite = false;
            this.ActiveOnKeyDown = false;
            this.IsEditAble = true;
        } 
        
        public ReportParameter(string FullName , CoreDefine.InputTypes InputTypes, string Value,long RelatedTable,string ViewCommand,int DigitsAfterDecimal,bool IsLeftWrite)
        { 
            this.FullName = FullName;
            this.Value = Value;
            this.InputTypes = InputTypes;
            this.RelatedTable = RelatedTable;
            this.ViewCommand = ViewCommand;
            this.DigitsAfterDecimal = DigitsAfterDecimal;
            this.IsLeftWrite = IsLeftWrite;
        }

        public ReportParameter(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ReportParameter));
            var ReportParameterInfo = serializer.Deserialize(stringReader) as ReportParameter;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.Value = ReportParameterInfo.Value;
            this.InputTypes = ReportParameterInfo.InputTypes;
            this.RelatedTable = ReportParameterInfo.RelatedTable;
            this.ViewCommand = ReportParameterInfo.ViewCommand;
            this.DigitsAfterDecimal = ReportParameterInfo.DigitsAfterDecimal;
            this.IsLeftWrite = ReportParameterInfo.IsLeftWrite;
            this.RelatedField = ReportParameterInfo.RelatedField;
            this.RelatedFieldCommand = ReportParameterInfo.RelatedFieldCommand;
            this.ActiveOnKeyDown = ReportParameterInfo.ActiveOnKeyDown;
            this.IsEditAble = ReportParameterInfo.IsEditAble;
            this.SpecialValue = ReportParameterInfo.SpecialValue;
        }

        public string SystemName()
        {
            return Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((this.FullName + this.ViewCommand + this.RelatedTable)));
        }

        public Table ExTable()
        {
            return new Table(CoreObject.Find(RelatedTable));
        }
    }
    public partial class ReportResource
    {

        public byte[] Value { get; set; }
        public string ValueJS { get; set; } 
        public string FullName { get; set; }
        public string Icon { get; set; }
        public long CoreObjectID { get; set; }
        public long DataSourceID { get; set; }
        public long ParentID { get; set; }
        public ReportResource()
        {
            this.Value = null;  
            this.ValueJS = null;  
            this.CoreObjectID = 0; 
            this.DataSourceID = 0; 
            this.ParentID = 0;
        }

        public ReportResource(string ValueJS)
        { 
            this.ValueJS = ValueJS; 
        }


        public ReportResource(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            ValueXml = ValueXml.Replace("<Report xmlns", "<ReportResource xmlns");
            ValueXml = ValueXml.Replace("</Report>", "</ReportResource>");
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ReportResource));
            var ReportParameterInfo = serializer.Deserialize(stringReader) as ReportResource; 
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.ParentID = _CoreObject.ParentID;
            this.FullName = _CoreObject.FullName;
            this.Value = ReportParameterInfo.Value; 
            this.ValueJS = ReportParameterInfo.ValueJS; 
            this.Icon = ReportParameterInfo.Icon; 
            this.DataSourceID = ReportParameterInfo.DataSourceID; 
        } 
    }

    
}