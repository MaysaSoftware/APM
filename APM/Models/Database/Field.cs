using APM.Models.APMObject;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using static APM.Models.Tools.CoreDefine;

namespace APM.Models.Database
{
    public class Field
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public string FieldNature { get; set; }
        public CoreDefine.InputTypes FieldType { get; set; }
        public CoreDefine.FieldDisplayType FieldDisplayType { get; set; }
        public bool IsEditAble { get; set; }
        public bool IsDefaultView { get; set; }
        public bool IsRequired { get; set; }
        public long RelatedTable { get; set; }
        public string ViewCommand { get; set; }
        public string SpecialValue { get; set; }
        public string DefaultValue { get; set; }
        public bool IsDefaultViewRadioBox { get; set; }
        public bool IsLeftWrite { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsTableAttachemnt { get; set; }
        public bool IsWide { get; set; }
        public bool ActiveOnKeyDown { get; set; }
        public bool IsExclusive { get; set; }
        public bool ShowInForm { get; set; }
        public string Folder { get; set; }
        public long CoreObjectID { get; set; }
        public int DigitsAfterDecimal { get; set; }
        public long RelatedField { get; set; }
        public long RelatedFieldCommand { get; set; }
        public string Class { get; set; } 
        public bool ClearAfterChange { get; set; }
        public string FieldComment { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public string AutoFillQuery { get; set; }
        public string TextColor { get; set; }
        public int SearchAutoCompleteCount { get; set; }
        public string GridTextColor { get; set; }
        public string RelatedLink { get; set; }
        public string ColumnWidth { get; set; }
        public string AnalystDescription { get; set; }    
        public bool SaveAndNewForm { get; set; }    

        public Field()
        {
            this.FieldName = string.Empty;
            this.DisplayName = string.Empty;
            this.RelatedLink = string.Empty;
            this.FieldNature = "Nvarchar(400)";
            this.FieldType = CoreDefine.InputTypes.ShortText;
            this.IsEditAble = false;
            this.IsDefaultView = false;
            this.IsRequired = false;
            this.RelatedTable = 0;
            this.ViewCommand = string.Empty;
            this.SpecialValue = string.Empty;
            this.DefaultValue = string.Empty;
            this.FieldComment = string.Empty;
            this.IsDefaultViewRadioBox = false;
            this.IsLeftWrite = false;
            this.IsIdentity = false;
            this.IsVirtual = false;
            this.Folder = "عمومی";
            this.CoreObjectID = 0;
            this.DigitsAfterDecimal = 0;
            this.RelatedField = 0;
            this.RelatedFieldCommand = 0;
            this.Class = string.Empty; 
            this.AutoFillQuery = string.Empty;
            this.IsWide = false;
            this.ClearAfterChange = false;
            this.IsExclusive = false;
            this.ActiveOnKeyDown = false;
            this.ShowInForm = true;
            this.IsTableAttachemnt = false;
            this.MinValue = 0;
            this.MaxValue = 0;
            this.TextColor = "#000";
            SearchAutoCompleteCount = 4;
            GridTextColor = "GridBlackText";
            ColumnWidth = "2";
            FieldDisplayType = CoreDefine.FieldDisplayType.عمودی;
            AnalystDescription= string.Empty;
            SaveAndNewForm = false;
        }
        public Field(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            if (ValueXml.IndexOf("<RelatedFieldCommand />") > -1)
                ValueXml = ValueXml.Replace("<RelatedFieldCommand />", "<RelatedFieldCommand>0</RelatedFieldCommand>");
            else if (ValueXml.IndexOf("<RelatedFieldCommand") > -1)
            {
                string SubStr = ValueXml.Substring(ValueXml.IndexOf("<RelatedFieldCommand") + ("<RelatedFieldCommand>").Length, (ValueXml.IndexOf("</RelatedFieldCommand") - ValueXml.IndexOf("<RelatedFieldCommand") - ("</RelatedFieldCommand").Length));
                int Num = 0;
                if (!int.TryParse(SubStr, out Num))
                ValueXml = ValueXml.Replace("<RelatedFieldCommand>"+ SubStr+ "</RelatedFieldCommand>", "<RelatedFieldCommand>0</RelatedFieldCommand>");
            }
            if (ValueXml.IndexOf("<RelatedField />") > -1)
                ValueXml = ValueXml.Replace("<RelatedField />", "<RelatedField>0</RelatedField>");

            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(Field));
            var FieldInfo = serializer.Deserialize(stringReader) as Field;
            this.FieldName = _CoreObject.FullName;
            this.DisplayName = FieldInfo.DisplayName;
            this.FieldNature = FieldInfo.FieldNature;
            this.FieldType = FieldInfo.FieldType;
            this.IsEditAble = FieldInfo.IsEditAble;
            this.IsDefaultView = FieldInfo.IsDefaultView;
            this.IsRequired = FieldInfo.IsRequired;
            this.RelatedTable = FieldInfo.RelatedTable;
            this.ViewCommand = FieldInfo.ViewCommand;
            this.SpecialValue = FieldInfo.SpecialValue;
            this.DefaultValue = FieldInfo.DefaultValue;
            this.IsDefaultViewRadioBox = FieldInfo.IsDefaultViewRadioBox;
            this.IsLeftWrite = FieldInfo.IsLeftWrite;
            this.IsIdentity = FieldInfo.IsIdentity;
            this.IsVirtual = FieldInfo.IsVirtual;
            this.Folder = _CoreObject.Folder;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.DigitsAfterDecimal = FieldInfo.DigitsAfterDecimal;
            this.RelatedField = FieldInfo.RelatedField;
            this.RelatedFieldCommand = FieldInfo.RelatedFieldCommand;
            this.IsWide = FieldInfo.IsWide;
            this.IsExclusive = FieldInfo.IsExclusive;
            this.MinValue = FieldInfo.MinValue;
            this.MaxValue = FieldInfo.MaxValue;
            this.ActiveOnKeyDown = FieldInfo.ActiveOnKeyDown;
            this.Class = FieldInfo.Class;
            this.ClearAfterChange = FieldInfo.ClearAfterChange;
            this.FieldComment = FieldInfo.FieldComment;
            this.ShowInForm = FieldInfo.ShowInForm;
            this.AutoFillQuery = FieldInfo.AutoFillQuery;
            this.TextColor = FieldInfo.TextColor;
            this.SearchAutoCompleteCount = FieldInfo.SearchAutoCompleteCount;
            this.GridTextColor = FieldInfo.GridTextColor;
            this.RelatedLink = FieldInfo.RelatedLink;
            this.ColumnWidth = FieldInfo.ColumnWidth;
            this.FieldDisplayType = FieldInfo.FieldDisplayType;
            this.AnalystDescription = FieldInfo.AnalystDescription;
            this.SaveAndNewForm = FieldInfo.SaveAndNewForm;
        }
        public Field(string _FieldName, string DisplayName, string _FieldNature = "Nvarchar(400)", CoreDefine.InputTypes _FieldType = CoreDefine.InputTypes.LongText, string _Folder = "عمومی", bool _IsEditAble = false, bool _IsDefaultView = true, bool _IsRequired = false, string _ViewCommand = "", string _SpecialValue = "", string _DefaultValue = "", bool _IsDefaultViewRadioBox = false, bool _IsLeftWrite = false, long RelatedTable = 0, bool IsWide = false, float MinValue = float.MinValue, float MaxValue = float.MaxValue, bool IsExclusive = false, bool IsIdentity = false, bool IsVirtual = false, long CoreObjectID = 0, bool ShowInForm = true)
        {
            this.FieldName = _FieldName;
            this.DisplayName = DisplayName;
            this.FieldNature = _FieldNature;
            this.FieldType = _FieldType;
            this.IsEditAble = _IsEditAble;
            this.IsDefaultView = _IsDefaultView;
            this.IsRequired = _IsRequired;
            this.ViewCommand = _ViewCommand;
            this.SpecialValue = _SpecialValue;
            this.DefaultValue = _DefaultValue;
            this.IsDefaultViewRadioBox = _IsDefaultViewRadioBox;
            this.IsLeftWrite = _IsLeftWrite;
            this.Folder = _Folder;
            this.RelatedTable = RelatedTable;
            this.IsWide = IsWide;
            this.MaxValue = MaxValue;
            this.MinValue = MinValue;
            this.IsExclusive = IsExclusive;
            this.IsIdentity = IsIdentity;
            this.IsVirtual = IsVirtual;
            this.CoreObjectID = CoreObjectID;
            this.ShowInForm = ShowInForm;
        }

        public string DataCacheName()
        {
            return "CachedListOf" + "_" + this.RelatedTable + "_" + Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((this.ViewCommand + this.SpecialValue)));
        }
        public string[] ComboValues()
        {
            if (this.FieldType == CoreDefine.InputTypes.TwoValues)
            {
                return !string.IsNullOrEmpty(this.SpecialValue) && this.SpecialValue.Split('،').Length > 1 ? this.SpecialValue.Split('،') : new string[] { "خیر", "بله" };
            }
            else if (this.FieldType == CoreDefine.InputTypes.ComboBox || this.FieldType == CoreDefine.InputTypes.MultiSelectFromComboBox)
            {
                return !string.IsNullOrEmpty(this.SpecialValue) ? this.SpecialValue.Split('،') : new string[] { };
            }
            else
                return new string[] { "خیر", "بله" };
        }

        public string Title()
        {
            if (string.IsNullOrEmpty(this.DisplayName))
                return Tools.Tools.UnSafeTitle(this.FieldName);
            else
                return this.DisplayName;
        }
        public object ToFormatField(CoreDefine.InputTypes InputType, object Value)
        {
            switch (InputType)
            {
                case CoreDefine.InputTypes.ShortText:
                    return Value is null ? null : DataConvertor.ArabicToPersion(Value.ToString());

                case CoreDefine.InputTypes.LongText:
                    return Value is null ? null : DataConvertor.ArabicToPersion(Value.ToString());

                case CoreDefine.InputTypes.Plaque:
                    //string FindChar= new string((char)8206, 1);
                    //string ReplaceChar = new string((char)28, 1);
                    string Plaque = DataConvertor.ArabicToPersion(Value.ToString());
                    //Plaque = Plaque.Replace(FindChar,"");
                    //if (Plaque.Length == 8)
                    //{

                    //}
                    return Plaque;
            }
            return Value;
        }

        public static string FormatImage(string Value)
        {
            return Value.IndexOf("data:image") > -1 ? Value : "data:image/png;base64," + Value;
        }
        public static string Image(string Value)
        {
            //byte[] imageBytes = Convert.FromBase64String(Value);
            //return FormatImage(Convert.ToBase64String(imageBytes));
            return Value;
        }
        public Table ExTable()
        {
            return new Table(CoreObject.Find(RelatedTable));
        }
        public ComputationalField ComputationalField()
        {
            return new ComputationalField(CoreObject.Find(CoreObjectID));
        }

        public string SystemName()
        {
            return Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes((this.FieldName + this.ViewCommand + this.SpecialValue)));
        }

        public static List<string> ToFolderNames(List<Field> _Fields)
        {
            List<string> FolderList = new List<string>();
            FolderList.Add("عمومی");
            return FolderList;
        }
    }
    public class TemplateField
    {
        public bool IsWide { get; set; }
        public string Folder { get; set; }
        public string FullName { get; set; }
        public CoreDefine.InputTypes InputTypes { get; set; }
        public object Parameter { get; set; }
        public string ColumnWidth { get; set; }

        public TemplateField()
        {
            this.Folder = string.Empty;
            this.IsWide = false;
            this.FullName = string.Empty;
            this.InputTypes = CoreDefine.InputTypes.ShortText;
            this.Parameter = new { };
            this.ColumnWidth = "2";
        }
        public TemplateField(bool IsWide, string Folder, string FullName, CoreDefine.InputTypes InputTypes, object Parameter,string ColumnWidth="2")
        {
            this.IsWide = IsWide;
            this.Folder = Folder;
            this.FullName = FullName;
            this.InputTypes = InputTypes;
            this.Parameter = Parameter;
            this.ColumnWidth = ColumnWidth;
        }

    }

    public class DisplayField
    {
        public TableButtonEventsType TableButtonEventsType { get; set; } 
        public long ReportID { get; set; }  
        public long ParameterReportID { get; set; }  
        public long CoreObjectID { get; set; }  
        public string Template { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public bool ShowInStart { get; set; }
        public bool UseDefualReportSetting { get; set; }
        public string PrinterName { get; set; }
        public int PrintCopy { get; set; }
        public string QueryPrintCopy { get; set; }
        public string ExecutionConditionQuery { get; set; }
        public string Query { get; set; }
        public string Icon { get; set; }
        public bool ShowTitle { get; set; }
        public long RelatedForm { get; set; }
        public long RelatedField { get; set; }
        public bool IsReloadGrid { get; set; }

        public string EMail { get; set; }
        public string EMailUserName { get; set; }
        public string EMailPassWord { get; set; }
        public string EMailServer { get; set; }
        public string EMailPort { get; set; }
        public string[] ReceivingUsers { get; set; }
        public string[] ReceivingRole { get; set; }
        public bool InsertingUser { get; set; }
        public string ReceivingQuery { get; set; }
        public bool SendAttachmentFile { get; set; }
        public bool EnableSsl { get; set; }
        public bool UsePublickEmail { get; set; }
        public string[] SendReport { get; set; }
        public string Title { get; set; }
        public string TitleQuery { get; set; }
        public string BodyMessage { get; set; }
        public string BodyMessageQuery { get; set; }
        public string URL { get; set; }

        public DisplayField()
        {
            this.Template = string.Empty; 
            this.ReportID = 0;
            this.ParameterReportID = 0;
            this.CoreObjectID = 0;
            this.TableButtonEventsType = TableButtonEventsType.خالی;
            this.ShowInStart = false;
            this.UseDefualReportSetting = true;
            this.PrinterName = string.Empty;
            this.QueryPrintCopy = string.Empty;
            this.PrintCopy = 0;
            this.ExecutionConditionQuery = string.Empty;
            this.Query = string.Empty;
            this.Icon= string.Empty;
            this.ShowTitle = true;
            this.RelatedForm = 0;
            this.RelatedField = 0;
            this.IsReloadGrid = false; 
            this.EMailServer = "smtp.gmail.com";
            this.EMailPort = "587";
            EnableSsl = true;
            UsePublickEmail = false;
            URL = string.Empty;
        }
        public DisplayField(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(DisplayField));
            var DisplayField = serializer.Deserialize(stringReader) as DisplayField;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            this.Template = DisplayField.Template;
            this.TableButtonEventsType = DisplayField.TableButtonEventsType;
            this.ReportID = DisplayField.ReportID;
            this.ParameterReportID = DisplayField.ParameterReportID;
            this.DisplayName = DisplayField.DisplayName;
            this.ShowInStart = DisplayField.ShowInStart;
            this.PrinterName = DisplayField.PrinterName;
            this.QueryPrintCopy = DisplayField.QueryPrintCopy;
            this.PrintCopy = DisplayField.PrintCopy;
            this.Icon = DisplayField.Icon;
            this.ShowTitle = DisplayField.ShowTitle;
            this.ExecutionConditionQuery = DisplayField.ExecutionConditionQuery;
            this.Query = DisplayField.Query;
            this.RelatedForm = DisplayField.RelatedForm;
            this.RelatedField = DisplayField.RelatedField;
            this.IsReloadGrid = DisplayField.IsReloadGrid;

            this.EMail = DisplayField.EMail;
            this.EMailUserName = DisplayField.EMailUserName;
            this.EMailPassWord = DisplayField.EMailPassWord;
            this.EMailServer = DisplayField.EMailServer;
            this.EMailPort = DisplayField.EMailPort;
            this.ReceivingUsers = DisplayField.ReceivingUsers;
            this.ReceivingRole = DisplayField.ReceivingRole;
            this.ReceivingQuery = DisplayField.ReceivingQuery;
            this.InsertingUser = DisplayField.InsertingUser;
            this.SendAttachmentFile = DisplayField.SendAttachmentFile;
            this.SendReport = DisplayField.SendReport;
            this.Title = DisplayField.Title;
            this.TitleQuery = DisplayField.TitleQuery;
            this.BodyMessage = DisplayField.BodyMessage;
            this.BodyMessageQuery = DisplayField.BodyMessageQuery;
            this.EnableSsl = DisplayField.EnableSsl;
            this.UsePublickEmail = DisplayField.UsePublickEmail;
            this.URL = DisplayField.URL;
        }

    }
    public class ComputationalField
    {
        public string Query { get; set; }
        public bool ShowInForm { get; set; }
        public bool IsDefaultView { get; set; }
        public bool IsWide { get; set; }
        public CoreDefine.InputTypes FieldType { get; set; }
        public int DigitsAfterDecimal { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public string DisplayName { get; set; }
        public string FieldComment { get; set; }

        public ComputationalField()
        {
            this.Query = string.Empty;
            this.ShowInForm = true;
            this.IsDefaultView = true;
            this.IsWide = false;
            this.DigitsAfterDecimal = 0;
            this.MaxValue = 0;
            this.MinValue = 0;
            this.DisplayName = string.Empty;
            this.FieldComment = string.Empty;
        }
        public ComputationalField(string Query)
        {
            this.Query = Query;
        }
        public ComputationalField(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ComputationalField));
            var DisplayField = serializer.Deserialize(stringReader) as ComputationalField;
            this.Query = DisplayField.Query;
            this.FieldType = DisplayField.FieldType;
            this.ShowInForm = DisplayField.ShowInForm;
            this.IsDefaultView = DisplayField.IsDefaultView;
            this.IsWide = DisplayField.IsWide;
            this.DigitsAfterDecimal = DisplayField.DigitsAfterDecimal;
            this.MaxValue = DisplayField.MaxValue;
            this.MinValue = DisplayField.MinValue;
            this.DisplayName = DisplayField.DisplayName;
            this.FieldComment = DisplayField.FieldComment;
        }

    }

    public class ShowFieldEvent
    {
        public string FieldValue { get; set; }
        public long SelectedObjectID { get; set; }
        public bool ShowObject { get; set; }

        public ShowFieldEvent()
        {
            this.FieldValue = string.Empty;
            this.SelectedObjectID = 0;
            this.ShowObject = false;
        }
        public ShowFieldEvent(string FieldValue, long SelectedObjectID, bool ShowObject)
        {
            this.FieldValue = FieldValue;
            this.SelectedObjectID = SelectedObjectID;
            this.ShowObject = ShowObject;
        }
        public ShowFieldEvent(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(ShowFieldEvent));
            var ShowFieldEvent = serializer.Deserialize(stringReader) as ShowFieldEvent;
            this.FieldValue = ShowFieldEvent.FieldValue;
            this.SelectedObjectID = ShowFieldEvent.SelectedObjectID;
            this.ShowObject = ShowFieldEvent.ShowObject;
        }

    }
    public class AutoCompeleteFieldEvent
    {
        public string FieldValue { get; set; }
        public long SelectedObjectID { get; set; }
        public bool ShowObject { get; set; }

        public AutoCompeleteFieldEvent()
        {
            this.FieldValue = string.Empty;
            this.SelectedObjectID = 0;
            this.ShowObject = false;
        }
        public AutoCompeleteFieldEvent(string FieldValue, long SelectedObjectID, bool ShowObject)
        {
            this.FieldValue = FieldValue;
            this.SelectedObjectID = SelectedObjectID;
            this.ShowObject = ShowObject;
        }
        public AutoCompeleteFieldEvent(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(AutoCompeleteFieldEvent));
            var ShowFieldEvent = serializer.Deserialize(stringReader) as AutoCompeleteFieldEvent;
            this.FieldValue = ShowFieldEvent.FieldValue;
            this.SelectedObjectID = ShowFieldEvent.SelectedObjectID;
            this.ShowObject = ShowFieldEvent.ShowObject;
        }

    }
}