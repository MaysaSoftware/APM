using Newtonsoft.Json;
using APM.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using APM.Models.Tools;
using APM.Models.Database;
using System.IO;
using Stimulsoft.Report.Components;
using APM.Models.Security;
using APM.Models.APMObject;
using System.Drawing.Printing;
using System.Management;

namespace APM.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
         
        public static DataTank.ReportCoreObject ReportCoreObject=new DataTank.ReportCoreObject(); 

        public static DataTank.ReportParameterName ParameterName = new DataTank.ReportParameterName();

        public static DataTank.ReportParameterValue ParameterValue = new DataTank.ReportParameterValue();

        public ActionResult Index(long ReportID)
        {
            try
            {
                string[] FieldName = null;
                object[] FieldValue = null; 
                ParameterName[""] = null;
                ParameterValue[""] = null; 
                Session["ReportParentID"] = ReportID;
                List<Folder> FolderList = new List<Folder>();
                FolderList.Add(new Folder());
                ViewData["ReportFolderDataKey"] = FolderList;
                ViewData["FieldParameterList"] = Desktop.TempeletReportParameter(ReportID, FieldName, FieldValue);
                ViewData["ParentID"] = "0";
                ViewData["DataKey"] = "0";

            }
            catch (Exception ex)
            {
                Log.Error("Report.Index", "\n" + ex.Message);
            }
            return View("~/Views/Report/Index.cshtml");
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            try
            {
                ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                             viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                                 ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }

            }
            catch (Exception ex)
            {
                Log.Error("Report.RenderRazorViewToString", "\n" + ex.Message);
                using (var sw = new StringWriter())
                {
                    return sw.GetStringBuilder().ToString();
                }
            }
        }


        public ActionResult Designer(long ReportID)
        {
            Session["ReportParentID"] = ReportID;
            return View("~/Views/Report/Designer.cshtml");
        }

        public ActionResult Viewer(long ReportID,long _ParameterID,string  _ParameterValue)
        {
            try
            {
                string[] FieldName = null;
                object[] FieldValue = null;

                if (_ParameterID > 0)
                {
                    CoreObject ParameterCore = CoreObject.Find(_ParameterID);
                    ReportID = ParameterCore.ParentID;
                    List<CoreObject> ParameterList = CoreObject.FindChilds(ReportID, CoreDefine.Entities.پارامتر_گزارش);
                    string[] ParameterNameArr = new string[ParameterList.Count];
                    string[] ParameterValueArr = new string[ParameterList.Count];
                    for(int Index=0;Index<ParameterList.Count;Index++)
                    {
                        ParameterNameArr[Index] = ParameterList[Index].FullName;
                        if (ParameterList[Index].FullName == ParameterCore.FullName)
                            ParameterValueArr[Index] = _ParameterValue;
                        else
                        {
                            ReportParameter reportParameter = new ReportParameter(ParameterList[Index]);

                            ParameterValueArr[Index] =Tools.GetDefaultValue(reportParameter.Value);
                        }
                    }
                    ParameterName[""] = ParameterNameArr;
                    ParameterValue[""] = ParameterValueArr;
                    FieldName = new string[] { ParameterCore.FullName };
                    FieldValue = new object[] { _ParameterValue };

                }
                else
                { 
                    ParameterName[""] = null;
                    ParameterValue[""] = null; 
                }
                Session["ReportParentID"] = ReportID;
                List<Folder> FolderList = new List<Folder>();
                FolderList.Add(new Folder());
                ViewData["ReportFolderDataKey"] = FolderList;
                ViewData["FieldParameterList"] = Desktop.TempeletReportParameter(ReportID, FieldName, FieldValue);
                ViewData["ParentID"] = "0";
                ViewData["DataKey"] = "0";

            }
            catch(Exception ex)
            { 
                Log.Error("Report.Viewer", "\n" + ex.Message);
            }
            return View("~/Views/Report/Viewer.cshtml"); 
        }

        public void PrintReportWithoutPriView(long ReportID, long _ParameterID, string _ParameterValue)
        {
            CoreObject ReportCore = CoreObject.Find(ReportID);
            DisplayField displayField = new DisplayField();
            if (ReportCore.Entity == CoreDefine.Entities.فیلد_نمایشی)
            {
                displayField = new DisplayField(ReportCore);
                ReportID = displayField.ReportID;
            }

            if (_ParameterID > 0)
            {
                CoreObject ParameterCore = CoreObject.Find(_ParameterID);
                ReportID = ParameterCore.ParentID;
                ParameterName[""] = new string[] { ParameterCore.FullName };
                ParameterValue[""] = new string[] { _ParameterValue }; 

            }
            else
            {
                ParameterName[""] = null;
                ParameterValue[""] = null;
            }
            Session["ReportParentID"] = ReportID;  

            Report report = new Report();
            StiReport _StiReport = report.Build(ReportID, ParameterName[""], ParameterValue[""]);
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport(ReportID));
            Report Report = new Report(CoreObject.Find(_Report.ParentID));

            try
            {
                string ConnectionString = string.Empty;
                if (Report.DataSourceID != 0)
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
                    ConnectionString = dataSourceInfo.ConnectionString;
                }
                else
                    ConnectionString = Referral.DBData.ConnectionData.ConnectionString;

                _StiReport.Dictionary.Databases[0] = new StiSqlDatabase("ارتباط", ConnectionString); 
                string ParameterQuery = string.Empty;

                if (_StiReport.Dictionary.Variables.Count > 0)
                {
                    List<CoreObject> ParameterList = CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش);
                    foreach (StiVariable Varible in _StiReport.Dictionary.Variables)
                    {
                        CoreObject ParameterCore = ParameterList.Where(item => item.FullName == Tools.SafeTitle(Varible.Name)).ElementAt(0);
                        ReportParameter _ReportParameter = new ReportParameter(ParameterCore);
                        ParameterQuery += "\nDECLARE @" + _ReportParameter.FullName;

                        switch (_ReportParameter.InputTypes)
                        {
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                                {
                                    ParameterQuery += " AS BIGINT=";
                                    break;
                                }
                            case CoreDefine.InputTypes.TwoValues:
                                {
                                    ParameterQuery += " AS BIT=";
                                    break;
                                }
                            default:
                                {
                                    ParameterQuery += " AS NVARCHAR(400)=";
                                    break;
                                }
                        }

                        if (ParameterName[""] != null)
                        {
                            int FindeIndex = Array.IndexOf(ParameterName[""], _ReportParameter.FullName);
                            if (FindeIndex != -1 || _StiReport.Dictionary.Variables[_ReportParameter.FullName] !=null)
                            {
                                
                                _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value = FindeIndex != -1?ParameterValue[""].ElementAt(FindeIndex):Tools.GetDefaultValue(_ReportParameter.Value);

                                switch (_ReportParameter.InputTypes)
                                {
                                    case CoreDefine.InputTypes.Number:
                                    case CoreDefine.InputTypes.RelatedTable:
                                        {
                                            ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                            break;
                                        }
                                    case CoreDefine.InputTypes.TwoValues:
                                        {
                                            ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                            break;
                                        }
                                    default:
                                        {
                                            ParameterQuery += " N'" + _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value + "'";
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            if (_StiReport.Dictionary.Variables[_ReportParameter.FullName] != null)
                            {
                                _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value = Tools.GetDefaultValue(_ReportParameter.Value.ToString());
                                if (_StiReport.Dictionary.Variables[_ReportParameter.FullName].Value != "")
                                    switch (_ReportParameter.InputTypes)
                                    {
                                        case CoreDefine.InputTypes.Number:
                                        case CoreDefine.InputTypes.RelatedTable:
                                            {
                                                ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                                break;
                                            }
                                        case CoreDefine.InputTypes.TwoValues:
                                            {
                                                ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                                break;
                                            }
                                        default:
                                            {
                                                ParameterQuery += " N'" + _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value + "'";
                                                break;
                                            }
                                    }
                                else
                                    ParameterQuery += "NULL";
                            }
                        }
                    }

                }

                if (Report.QueryBeforRun != "")
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
                    SQLDataBase dataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                    dataBase.Execute(ParameterQuery + "\n\n" + Tools.CheckQuery(Report.QueryBeforRun));
                }

                _StiReport.Compile();
                _StiReport.Render(false);
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = displayField.UseDefualReportSetting && displayField.CoreObjectID>0? displayField.PrinterName: Report.PrinterName;
                printerSettings.Copies = displayField.UseDefualReportSetting && displayField.CoreObjectID > 0 ? (short)displayField.PrintCopy : (short)Report.PrintCopy; 

                if (!string.IsNullOrEmpty(Report.QueryPrintCopy))
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
                    SQLDataBase dataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                    printerSettings.Copies = (short)int.Parse(dataBase.SelectField(ParameterQuery + "\n\n" + Tools.CheckQuery(Report.QueryPrintCopy)).ToString()); 
                }


                if(Report.PrinterName==""&& Report.PrintCopy>1)
                    _StiReport.Print(false, printerSettings.Copies);
                else if (Report.PrinterName == "" && Report.PrintCopy == 1)
                    _StiReport.Print(false);
                else
                    _StiReport.Print(false, printerSettings);

                 
                //string ipAddress = "127.0.0.1";
                //int port = 0;

                //string zplImageData = string.Empty;
                //string filePath = @"your png file path";
                //byte[] binaryData = System.IO.File.ReadAllBytes(filePath);
                //foreach (Byte b in binaryData)
                //{
                //    string hexRep = String.Format("{0:X}", b);
                //    if (hexRep.Length == 1)
                //        hexRep = "0" + hexRep;
                //    zplImageData += hexRep;
                //}
                //string zplToSend = "^XA" + "^FO50" + "50^GFA,120000,120000,100" + binaryData.Length + ",," + zplImageData + "^XZ";
                //string printImage = "^XA^FO115,50^IME:LOGO.PNG^FS^XZ";

                //try
                //{
                //    // Open connection
                //    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                //    client.Connect(ipAddress, port);

                //    // Write ZPL String to connection
                //    System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream(), Encoding.UTF8);
                //    writer.Write(zplToSend);
                //    writer.Flush();
                //    writer.Write(printImage);
                //    writer.Flush();
                //    // Close Connection
                //    writer.Close();
                //    client.Close();
                //}
                //catch (Exception ex)
                //{
                //    // Catch Exception
                //}


            }
            catch (Exception ex)
            {
                Log.Error("Report.GetStiViewerReport", "\n" + ex.Message);
            } 
        }

        public ActionResult ReloadReport(string _ParameterName,string _ParameterValue)
        { 
            string[] ParamName= _ParameterName.Split(',');
            string[] ParamValue= _ParameterValue.Split(',');
             
            for(int Index=0;Index<ParamName.Length;Index++) 
                ParamValue[Index] = ParamValue[Index].Replace("*",",").Replace("__", " "); 

            ParameterName[""] = ParamName; 
            ParameterValue[""] = ParamValue; 
            return View("~/Views/Report/LeftSide.cshtml"); 
        }
        //نیاز به اصلاح دارد باید با تابع بالایی یکسان شود
        public ActionResult ReloadReportWithProcess(long _ProcessID, string  _RecordID)
        {
            string Url = "~/Views/Report/Viewer.cshtml";
            CoreObject ProcessCore = CoreObject.Find(_ProcessID);
            ProcessType process = new ProcessType(ProcessCore);
            CoreObject ParameterCore = CoreObject.Find(process.ParameterReportID); 

             Record Rec = new  Record(Referral.DBData, "SELECT Top 1 فرم_جدولی,جدول,رکورد  FROM فرآیند inner join مراحل_فرآیند on فرآیند.شناسه=مراحل_فرآیند.فرآیند where رکورد > 0 and فرآیند.شناسه = "+ _RecordID + "  order by مراحل_فرآیند.تاریخ_ثبت desc,مراحل_فرآیند.ساعت_ثبت desc" );

            List<string> FolderList = new List<string>();
            FolderList.Add("");
            ViewData["ReportFolderDataKey"] = FolderList;
            ViewData["FieldParameterList"] = Desktop.TempeletReportParameter(process.ReportID);
            ViewData["ParentID"] = "0";
            ViewData["DataKey"] = "0";
            ParameterName[""] = new string[] {ParameterCore.FullName};
            ParameterValue[""] =new string[] {Rec.Field("رکورد", 0).ToString() };
            return Content(RenderRazorViewToString(Url, new object()));
        }

        public ActionResult StiDesigner()
        {
            StiReport report = new StiReport();
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport((long)Session["ReportParentID"])); 
            try
            {
                if (_Report.Value != null)
                    report.Load(_Report.Value);
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(_Report.ValueJS == null ? "" : _Report.ValueJS);
                    report.Load(byteArray);
                }


                List<string> FontNames = Tools.BFontNames();
                foreach (string FontItem in FontNames)
                {
                    string Font = FontItem.Replace(" ", "") + ".ttf";
                    byte[] FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Fonts/BFonts/" + Font));
                    StiResource Resource = new StiResource(
                        FontItem, Font, false, StiResourceType.FontTtf, FileByte, true);
                    report.Dictionary.Resources.Add(Resource);
                }

                List<string> IranSansFontNames = Tools.IranSansFontNames();
                foreach (string FontItem in IranSansFontNames)
                {
                    byte[] FileByte = System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath("~/Fonts/IRANSans/ttf/" + FontItem + ".ttf"));
                    StiResource Resource = new StiResource(
                         FontItem.Replace("Web_", " "), FontItem + ".ttf", false, StiResourceType.FontTtf, FileByte, true);
                    report.Dictionary.Resources.Add(Resource);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Report.StiDesigner", "\n" + ex.Message); 
            }

            return StiMvcDesigner.GetReportResult(report);
        }

        public ActionResult DesignerEvent()
        {
            return StiMvcDesigner.DesignerEventResult();
        } 

        public ActionResult SaveReport()
        {
            StiReport report = StiMvcDesigner.GetReportObject();
            string jstr = report.SaveToJsonString();
            int start = jstr.IndexOf("\"Resources\":");
            int end = jstr.IndexOf("\"Variables\":");
            string t = jstr.Substring(0, start); 
            string b= end > -1 ? jstr.Substring(end):""; 
            string Value = Tools.ToXML(new ReportResource(end > -1 ? (t + b): jstr)); 
            int saveError = 0; 
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport((long)Session["ReportParentID"])); 
            if (report.Dictionary.Variables.Count>0)
            {
                List<CoreObject> ParameterList = CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش); 
                foreach (StiVariable Varible in report.Dictionary.Variables)
                { 
                    if (ParameterList.Where(item => item.FullName == Tools.SafeTitle(Varible.Name)).ToList().Count == 0)
                    {
                        ReportParameter Parameter = new ReportParameter(Varible.Name,Tools.GetInputType("ShortText"),Varible.Value,0,"",0,false);
                        string XmlValue = Tools.ToXML(Parameter);
                        long id= Referral.DBCore.Insert("CoreObject"
                           , new string[] { "ParentID", "Entity", "FullName", "IsDefault", "Value" }
                           , new object[] { _Report.ParentID, CoreDefine.Entities.پارامتر_گزارش.ToString(), Tools.SafeTitle(Varible.Name), 0, XmlValue });

                        Referral.CoreObjects.Add(new CoreObject(id, _Report.ParentID, CoreDefine.Entities.پارامتر_گزارش, "", Tools.SafeTitle(Varible.Name), 0, false, XmlValue));

                    }
                }
            }
            else
            {
                if(CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش).Count>0)
                {
                    Referral.DBCore.Execute("Delete CoreObject where ParentID= " + _Report.ParentID + " And Entity=N'پارامتر_گزارش'");

                    int DeleteCoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == _Report.ParentID);
                    if (DeleteCoreIndex > -1)
                        Referral.CoreObjects.RemoveAt(DeleteCoreIndex);
                }
            }

            while(saveError<3)
            {
                bool Result = Referral.DBCore.UpdateRow(_Report.CoreObjectID, 0, "CoreObject", new string[] { "Value" }, new object[] { Value });
                if (Result)
                {
                    int CoreIndex = Referral.CoreObjects.FindIndex(x => x.CoreObjectID == _Report.CoreObjectID);
                    Referral.CoreObjects[CoreIndex].Value = Value;
                    saveError = 3;
                } 
                saveError++;
            }
            return StiMvcDesigner.SaveReportResult();
        }

        public ActionResult PreviewReport()
        {
            StiReport _StiReport = StiMvcDesigner.GetActionReportObject(); 
            if(Session["ReportParentID"]!=null)
            { 
               Report MaineReport=new Report( CoreObject.Find((long)Session["ReportParentID"]));
                string ParameterQuery = string.Empty;

                if (_StiReport.Dictionary.Variables.Count > 0)
                {
                    List<CoreObject> ParameterList = CoreObject.FindChilds(MaineReport.CoreObjectID, CoreDefine.Entities.پارامتر_گزارش);
                    foreach (StiVariable Varible in _StiReport.Dictionary.Variables)
                    {
                        if (ParameterList.Where(item => item.FullName == Tools.SafeTitle(Varible.Name)).Count()>0)
                        {

                            CoreObject ParameterCore = ParameterList.Where(item => item.FullName == Tools.SafeTitle(Varible.Name)).ElementAt(0);
                            ParameterQuery += "\nDECLARE @" + Varible.Name;

                            switch (Varible.Type.Name)
                            {
                                case "Int64":
                                    {
                                        ParameterQuery += " AS BIGINT = " + (Varible.Value == "" ? "null" : Varible.Value);
                                        break;
                                    }
                                case "Bool":
                                    {
                                        ParameterQuery += " AS BIT = " + (Varible.Value == "" ? "null" : Varible.Value);
                                        break;
                                    }
                                default:
                                    {
                                        ParameterQuery += " AS NVARCHAR(400) = N'" + Varible.Value + "'";
                                        break;
                                    }
                            }
                        }
                    }

                }


                if (MaineReport.QueryBeforRun!="")
                { 
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(MaineReport.DataSourceID)); 
                    SQLDataBase dataBase = new SQLDataBase(dataSourceInfo.ServerName,dataSourceInfo.DataBase,dataSourceInfo.Password,dataSourceInfo.UserName,SQLDataBase.SQLVersions.SQL2008);
                    dataBase.Execute(ParameterQuery+"\n"+Tools.CheckQuery( MaineReport.QueryBeforRun));
                }
            }
            return StiMvcDesigner.PreviewReportResult(_StiReport); 
        }
         
        public ActionResult GetStiViewerReport()
        {
            Report report = new Report();
            StiReport _StiReport = report.Build((long)Session["ReportParentID"], ParameterName[""], ParameterValue[""]);
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport((long)Session["ReportParentID"]));
            Report Report = new Report(CoreObject.Find(_Report.ParentID));

            try
            { 
                string ConnectionString = string.Empty;
                if (Report.DataSourceID != 0)
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
                    ConnectionString = dataSourceInfo.ConnectionString;
                }
                else
                    ConnectionString = Referral.DBData.ConnectionData.ConnectionString;

                _StiReport.Dictionary.Databases[0] = new StiSqlDatabase("ارتباط", ConnectionString);
                //_StiReport.Dictionary.Databases[0] = new StiSqlDatabase("اتصال به اطلاعات اصلی", ConnectionString);
                string ParameterQuery=string.Empty; 

                if (_StiReport.Dictionary.Variables.Count > 0)
                {
                    List<CoreObject> ParameterList = CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش);
                    foreach (StiVariable Varible in _StiReport.Dictionary.Variables)
                    {
                        CoreObject ParameterCore = ParameterList.Where(item => item.FullName == Tools.SafeTitle(Varible.Name)).ElementAt(0);
                        ReportParameter _ReportParameter = new ReportParameter(ParameterCore); 
                        ParameterQuery += "\nDECLARE @" + _ReportParameter.FullName;

                        switch (_ReportParameter.InputTypes)
                        {
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                                {
                                    ParameterQuery += " AS BIGINT=";
                                    break;
                                }
                            case CoreDefine.InputTypes.TwoValues:
                                {
                                    ParameterQuery += " AS BIT=";
                                    break;
                                }
                            default:
                                {
                                    ParameterQuery += " AS NVARCHAR(400)=";
                                    break;
                                }
                        }

                        if (ParameterName[""] != null)
                        {
                            int FindeIndex = Array.IndexOf(ParameterName[""], _ReportParameter.FullName);
                            if(FindeIndex != -1)
                            {
                                _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value = ParameterValue[""].ElementAt(FindeIndex);

                                switch (_ReportParameter.InputTypes)
                                {
                                    case CoreDefine.InputTypes.Number:
                                    case CoreDefine.InputTypes.RelatedTable:
                                        {
                                            ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                            break;
                                        }
                                    case CoreDefine.InputTypes.TwoValues:
                                        {
                                            ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                            break;
                                        }
                                    default:
                                        {
                                            ParameterQuery += " N'"+ _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value+"'";
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            if (_StiReport.Dictionary.Variables[_ReportParameter.FullName]!=null)
                            {
                                _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value = Tools.GetDefaultValue(_ReportParameter.Value.ToString());
                                if (_StiReport.Dictionary.Variables[_ReportParameter.FullName].Value != "")
                                    switch (_ReportParameter.InputTypes)
                                    {
                                        case CoreDefine.InputTypes.Number:
                                        case CoreDefine.InputTypes.RelatedTable:
                                            {
                                                ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                                break;
                                            }
                                        case CoreDefine.InputTypes.TwoValues:
                                            {
                                                ParameterQuery += _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value;
                                                break;
                                            }
                                        default:
                                            {
                                                ParameterQuery += " N'" + _StiReport.Dictionary.Variables[_ReportParameter.FullName].Value + "'";
                                                break;
                                            }
                                    }
                                else
                                    ParameterQuery += "NULL";
                            }
                        }
                    }

                }

                if (Report.QueryBeforRun != "")
                {
                    DataSourceInfo dataSourceInfo = new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
                    SQLDataBase dataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                    dataBase.Execute(ParameterQuery + "\n\n" + Tools.CheckQuery(Report.QueryBeforRun));
                }

                _StiReport.Compile();
                _StiReport.Render(false);
                //_StiReport.Print(false,1,1,1);

            }
            catch (Exception ex)
            {
                Log.Error("Report.GetStiViewerReport", "\n" + ex.Message);
            }
            return StiMvcViewer.GetReportResult(_StiReport);
        }

        public ActionResult ViewerEvent()
        { 
            return StiMvcViewer.ViewerEventResult();
        }

        public ActionResult PrintReport()
        {
            var report = StiMvcViewer.GetReportObject();
            var parameters = StiMvcViewer.GetRequestParams();
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport((long)Session["ReportParentID"])); 
            Report Report = new Report(CoreObject.Find(_Report.ParentID));
            DataSourceInfo dataSourceInfo = Report.DataSourceID==0? new DataSourceInfo(Referral.DBData.ConnectionData.Source, Referral.DBData.ConnectionData.DataBase, Referral.DBData.ConnectionData.DBPassword, Referral.DBData.ConnectionData.DBUser,CoreDefine.DataSourceType.SQLSERVER,"") : new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
 
            List<CoreObject> ParameterList = CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش);
            bool SaveRegistery = false;
            string Query = "";

            if (ParameterName[""] != null)
            {
                foreach (CoreObject Parameter in ParameterList)
                {
                    ReportParameter reportParameter = new ReportParameter(Parameter);
                    int FindeIndex = Array.IndexOf(ParameterName[""], Parameter.FullName);
                    if (reportParameter.InputTypes==CoreDefine.InputTypes.RelatedTable)
                    {
                        CoreObject TableCore = CoreObject.Find(reportParameter.RelatedTable);
                        Referral.DBRegistry.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "TableName", "CoreObjectID", "IP", "Version", "Source", "BrowserType", "BrowserVersion", "ServerName", "DatabaseName", "PCName" }
                                                                         , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, ParameterValue[""].ElementAt(FindeIndex), "گزارش", Report.FullName, "Print", TableCore.FullName, TableCore.CoreObjectID, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion, dataSourceInfo.ServerName, dataSourceInfo.DataBase, Referral.UserAccount.PCName });
                        SaveRegistery = true;
                    }
                    Query += "DECLARE @" + reportParameter.FullName + " AS ";
                    switch(reportParameter.InputTypes)
                    {
                        case CoreDefine.InputTypes.ShortText:
                        case CoreDefine.InputTypes.MiladyDateTime:
                        case CoreDefine.InputTypes.PersianDateTime:
                        case CoreDefine.InputTypes.NationalCode:
                        case CoreDefine.InputTypes.Plaque:
                        case CoreDefine.InputTypes.Phone:
                        case CoreDefine.InputTypes.Clock:
                        case CoreDefine.InputTypes.FillTextAutoComplete:
                        case CoreDefine.InputTypes.LongText:
                            {
                                Query += "Nvarchar(400) = N'" + ParameterValue[""].ElementAt(FindeIndex) + "'\n";
                                break;
                            }
                        case CoreDefine.InputTypes.Number:
                        case CoreDefine.InputTypes.RelatedTable:
                            {
                                Query += "BIGINT=" + ParameterValue[""].ElementAt(FindeIndex) + "\n";
                                break;
                            }
                    }
                } 
            }
            if (!SaveRegistery) 
                Referral.DBRegistry.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "TableName", "CoreObjectID", "IP", "Version", "Source", "BrowserType", "BrowserVersion", "ServerName", "DatabaseName", "PCName" }
                                                 , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, Session["ReportRecordID"], "گزارش", Report.FullName, "Print", Session["ReportTableName"], Session["ReportCoreObjectID"], Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion, dataSourceInfo.ServerName, dataSourceInfo.DataBase, Referral.UserAccount.PCName });

            if(!string.IsNullOrEmpty(Report.QueryAffterPrint))
            {
                Query += "\n" + Tools.CheckQuery( Report.QueryAffterPrint);
                if (dataSourceInfo.ServerName == dataSourceInfo.ServerName)
                {
                    Referral.DBData.Execute(Query);
                }
                else
                {
                    SQLDataBase sQLDataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                    sQLDataBase.Execute(Query);    
                }
            }

            return StiMvcViewer.PrintReportResult(report);
        }

        public ActionResult ExportReport()
        {
            var report = StiMvcViewer.GetReportObject();
            var parameters = StiMvcViewer.GetRequestParams();
            ReportResource _Report = new ReportResource(CoreObject.GetSourceReport((long)Session["ReportParentID"])); 
            Report Report = new Report(CoreObject.Find(_Report.ParentID));
            DataSourceInfo dataSourceInfo = Report.DataSourceID==0? new DataSourceInfo(CoreObject.Find(Referral.MasterDatabaseID)):new DataSourceInfo(CoreObject.Find(Report.DataSourceID));
            bool SaveRegistery = false;
            string Query = "";

            List<CoreObject> ParameterList = CoreObject.FindChilds(_Report.ParentID, CoreDefine.Entities.پارامتر_گزارش);
            if (ParameterName[""] != null)
            {
                foreach (CoreObject Parameter in ParameterList)
                {
                    ReportParameter reportParameter = new ReportParameter(Parameter);
                    int FindeIndex = Array.IndexOf(ParameterName[""], Parameter.FullName);
                    if(FindeIndex != -1)
                    { 
                        if (reportParameter.InputTypes == CoreDefine.InputTypes.RelatedTable)
                        {
                            CoreObject TableCore = CoreObject.Find(reportParameter.RelatedTable);
                            Referral.DBRegistry.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "TableName", "CoreObjectID", "IP", "Version", "Source", "BrowserType", "BrowserVersion", "ServerName", "DatabaseName", "PCName" }
                                                                             , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, ParameterValue[""].ElementAt(FindeIndex), "گزارش", Report.FullName, parameters.ExportFormat.ToString(), TableCore.FullName, TableCore.CoreObjectID, Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion, dataSourceInfo.ServerName, dataSourceInfo.DataBase, Referral.UserAccount.PCName });
                            SaveRegistery = true;
                        }

                        Query += "DECLARE @" + reportParameter.FullName + " AS ";
                        switch (reportParameter.InputTypes)
                        {
                            case CoreDefine.InputTypes.ShortText:
                            case CoreDefine.InputTypes.MiladyDateTime:
                            case CoreDefine.InputTypes.PersianDateTime:
                            case CoreDefine.InputTypes.NationalCode:
                            case CoreDefine.InputTypes.Plaque:
                            case CoreDefine.InputTypes.Phone:
                            case CoreDefine.InputTypes.Clock:
                            case CoreDefine.InputTypes.FillTextAutoComplete:
                            case CoreDefine.InputTypes.LongText:
                                {
                                    Query += "Nvarchar(400) = N'" + ParameterValue[""].ElementAt(FindeIndex) + "'\n";
                                    break;
                                }
                            case CoreDefine.InputTypes.Number:
                            case CoreDefine.InputTypes.RelatedTable:
                                {
                                    Query += "BIGINT=" + ParameterValue[""].ElementAt(FindeIndex) + "\n";
                                    break;
                                }
                        }
                    }
                }
            }
            if (!SaveRegistery)
                Referral.DBRegistry.Insert("Download_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "RecordID", "CoreEntity", "CoreName", "Format", "TableName", "CoreObjectID", "IP", "Version", "Source", "BrowserType", "BrowserVersion", "ServerName", "DatabaseName", "PCName" }
                                                 , new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, Session["ReportRecordID"], "گزارش", Report.FullName, parameters.ExportFormat.ToString(), Session["ReportTableName"], Session["ReportCoreObjectID"], Referral.UserAccount.IP, Referral.AppVersion, "WEB", Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion, dataSourceInfo.ServerName, dataSourceInfo.DataBase, Referral.UserAccount.PCName });


            if (!string.IsNullOrEmpty(Report.QueryAffterPrint))
            {
                Query += "\n" + Tools.CheckQuery(Report.QueryAffterPrint);
                if (dataSourceInfo.ServerName == dataSourceInfo.ServerName)
                {
                    Referral.DBData.Execute(Query);
                }
                else
                {
                    SQLDataBase sQLDataBase = new SQLDataBase(dataSourceInfo.ServerName, dataSourceInfo.DataBase, dataSourceInfo.Password, dataSourceInfo.UserName, SQLDataBase.SQLVersions.SQL2008);
                    sQLDataBase.Execute(Query);
                }
            }

            return StiMvcViewer.ExportReportResult(report);
        }
    }
}