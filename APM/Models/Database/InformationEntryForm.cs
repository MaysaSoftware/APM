using APM.Models.Tools;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using static APM.Models.Tools.CoreDefine;

namespace APM.Models.Database
{
    public class InformationEntryForm
    {
        public string InformationEntryFormName { get; set; }
        public string Query { get; set; }
        public string ConditionQuery { get; set; }
        public string OrderQuery { get; set; }
        public long RelatedTable { get; set; }
        public long ExternalField { get; set; } 
        public long CoreObjectID { get; set; }
        public long ParentID { get; set; }
        public long ShowRecordCountDefault { get; set; }
        public bool IsDefault { get; set; }
        public bool IsShowID { get; set; }
        public bool ShowSelectedColumn { get; set; }
        public bool SearchWithOnkeyDown { get; set; }
        public long SearchWithOnkeyDownCoreId { get; set; }
        public string Icon { get; set; }
        public string BadgeTitle { get; set; }
        public string BadgeQuery { get; set; }
        public string BadgeColor { get; set; }
        public string BadgeTextColor { get; set; }
        public string CSS { get; set; }
        public bool ShowAttachment { get; set; }
        public bool ShowDetailAttachment { get; set; }
        public bool ShowInParentForm { get; set; }
        public bool ShowInMenueTreeList { get; set; }
        public bool Groupable { get; set; }
        public bool Pageable { get; set; }
        public bool Aggregatesable { get; set; }
        public bool ShowAttachmentColumn { get; set; } 
        public long AttachmentColumnName { get; set; }
        public string TitleSaveButton { get; set; }
        public int HieghtAttachment { get; set; }
        public GridEditMode GridEditMode { get; set; }
        public bool ShowChartInformationEntryForm { get; set; }
        public bool IsCloseFormAffterSave { get; set; }

        public string ChartID { get; set; }
        public string ChartName { get; set; }
        public string ChartTitle { get; set; }
        public string ChartGroup { get; set; }
        public string ChartAvatar { get; set; }
        public string ChartParentID { get; set; }
        public long RowColorColumnName { get; set; }
        public string RowColorOperator { get; set; }
        public string RowColorColumnValue { get; set; }
        public string RowColorSelectedColor { get; set; } 

        public bool ShowLineNumber { get; set; }
        public bool ShowClearFormWhithOutFixItemButton { get; set; }
        public string FixItemClearForm { get; set; }
        public string ClearFormWhithOutFixItemButtonName { get; set; }
        public int RowCoutnInPage { get; set; }
        public bool SaveParentSubjectSaveChild { get; set; }
        public string FormComment { get; set; }
        public bool ShowAttachementButton { get; set; } 
        public bool CheckValidChildGrid { get; set; } 
        public bool ShowNewButton { get; set; } 
        public int Height { get; set; }
        public string DefualtColumnShowInGrid { get; set; }
        public string EditorFormJson { get; set; }
        public long GroupableField { get; set; }
        public string NewPageUrl { get; set; }
        public string UpdatePageUrl { get; set; }
        public string NewButtonVisibleConditionQuery { get; set; }
        public string GroupByQuery { get; set; }
        public bool SaveAtOnce { get; set; }
        public string CheckFieldDuplicateRecords { get; set; }
        public string ColumnWidth { get; set; }
        public string AttachmentColumnWidth { get; set; }
        public string AnalystDescription { get; set; }
        public bool ShowNewButtonInToolbar { get; set; }
        public bool ShowUpdateButtonInToolbar { get; set; }
        public bool ShowDeleteButtonInToolbar { get; set; }
        public bool ShowAttachmentButtonInToolbar { get; set; }
        public bool ShowViewButtonInToolbar { get; set; }
        public OpenFormType OpenFormType { get; set; }

        public InformationEntryForm()
        {
            InformationEntryFormName = "";
            Query = "";
            ConditionQuery = "";
            OrderQuery = "";
            RelatedTable = 0;
            ExternalField = 0; 
            CoreObjectID = 0;
            ParentID = 0;
            ShowRecordCountDefault = 1000;
            IsDefault = false;
            IsShowID = false;
            BadgeTitle = "";
            BadgeQuery = "";
            BadgeColor = "#f2d10e";
            BadgeTextColor = "#000";
            Icon = "";
            CSS = "";
            TitleSaveButton = "ذخیره";
            ShowAttachment = false;
            ShowInParentForm = false;
            Groupable = true;
            Pageable = true;
            Aggregatesable = true;
            ShowInMenueTreeList = true;
            ShowDetailAttachment = true;
            HieghtAttachment = 500;
            GridEditMode = GridEditMode.PopUp;
            ShowChartInformationEntryForm = false;
            ShowAttachmentColumn = false;
            ShowSelectedColumn = false;
            IsCloseFormAffterSave = true;
            ShowLineNumber = false;
            ShowClearFormWhithOutFixItemButton = false;
            SaveParentSubjectSaveChild = false;
            ShowAttachementButton = false;
            CheckValidChildGrid = false;
            SearchWithOnkeyDown = false;
            FixItemClearForm=string.Empty;
            ClearFormWhithOutFixItemButtonName = string.Empty;
            RowCoutnInPage = 15;
            FormComment = String.Empty;
            ShowNewButton=true;
            Height = 500;
            DefualtColumnShowInGrid = "";
            EditorFormJson = string.Empty;
            GroupableField = 0;
            NewPageUrl = string.Empty;
            UpdatePageUrl = string.Empty;
            NewButtonVisibleConditionQuery = string.Empty;
            GroupByQuery = string.Empty;
            SaveAtOnce = false;
            CheckFieldDuplicateRecords = string.Empty;
            ColumnWidth = "12";
            AttachmentColumnWidth = "12";
            SearchWithOnkeyDownCoreId = 0;
            AnalystDescription= string.Empty;
            ShowNewButtonInToolbar = true;
            ShowUpdateButtonInToolbar = true;
            ShowAttachmentButtonInToolbar = true;
            ShowDeleteButtonInToolbar = true;
            ShowViewButtonInToolbar = true;
            OpenFormType = OpenFormType.Grid;
        }  
        public InformationEntryForm(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString() ;
            ValueXml = ValueXml.Replace("<RowColorColumnName />", "<RowColorColumnName>0</RowColorColumnName>");
            var stringReader = new System.IO.StringReader(ValueXml);
 
            var serializer = new XmlSerializer(typeof(InformationEntryForm));
            var FormInfo= serializer.Deserialize(stringReader) as InformationEntryForm;
            InformationEntryFormName = FormInfo.InformationEntryFormName;
            Query= FormInfo.Query.Replace("&gt;", "<"); 
            ConditionQuery= FormInfo.ConditionQuery.Replace("&gt;", "<"); 
            OrderQuery= FormInfo.OrderQuery.Replace("&gt;", "<"); 
            RelatedTable  = FormInfo.RelatedTable;
            ExternalField = FormInfo.ExternalField;
            ShowRecordCountDefault = FormInfo.ShowRecordCountDefault;
            CoreObjectID = _CoreObject.CoreObjectID;
            ParentID = _CoreObject.ParentID;
            IsDefault = _CoreObject.IsDefault;
            IsShowID = FormInfo.IsShowID; 
            BadgeQuery= FormInfo.BadgeQuery.Replace("&gt;", "<");
            BadgeColor = FormInfo.BadgeColor;
            BadgeTitle= FormInfo.BadgeTitle;
            Icon = FormInfo.Icon;
            CSS = FormInfo.CSS;
            ShowAttachment = FormInfo.ShowAttachment;
            ShowInParentForm = FormInfo.ShowInParentForm;
            BadgeTextColor = FormInfo.BadgeTextColor;
            TitleSaveButton = FormInfo.TitleSaveButton;
            ShowInMenueTreeList = FormInfo.ShowInMenueTreeList;
            GridEditMode = FormInfo.GridEditMode;
            Groupable = FormInfo.Groupable;
            Pageable = FormInfo.Pageable;
            Aggregatesable = FormInfo.Aggregatesable;
            ShowDetailAttachment = FormInfo.ShowDetailAttachment;
            HieghtAttachment = FormInfo.HieghtAttachment; 
            ChartID = FormInfo.ChartID;
            ChartName = FormInfo.ChartName;
            ChartTitle = FormInfo.ChartTitle;
            ChartGroup = FormInfo.ChartGroup;
            ChartAvatar = FormInfo.ChartAvatar;
            ChartParentID = FormInfo.ChartParentID;
            ShowChartInformationEntryForm = FormInfo.ShowChartInformationEntryForm;
            ShowAttachmentColumn = FormInfo.ShowAttachmentColumn;
            AttachmentColumnName = FormInfo.AttachmentColumnName;
            RowColorColumnName = FormInfo.RowColorColumnName;
            RowColorOperator = FormInfo.RowColorOperator;
            RowColorColumnValue = FormInfo.RowColorColumnValue;
            RowColorSelectedColor = FormInfo.RowColorSelectedColor;
            ShowSelectedColumn = FormInfo.ShowSelectedColumn;
            IsCloseFormAffterSave = FormInfo.IsCloseFormAffterSave;
            ShowLineNumber = FormInfo.ShowLineNumber;
            ShowClearFormWhithOutFixItemButton = FormInfo.ShowClearFormWhithOutFixItemButton;
            FixItemClearForm = FormInfo.FixItemClearForm;
            ClearFormWhithOutFixItemButtonName = FormInfo.ClearFormWhithOutFixItemButtonName;
            RowCoutnInPage = FormInfo.RowCoutnInPage;
            SaveParentSubjectSaveChild = FormInfo.SaveParentSubjectSaveChild;
            FormComment = FormInfo.FormComment;
            ShowAttachementButton = FormInfo.ShowAttachementButton;
            CheckValidChildGrid = FormInfo.CheckValidChildGrid;
            ShowNewButton = FormInfo.ShowNewButton;
            Height = FormInfo.Height;
            DefualtColumnShowInGrid = FormInfo.DefualtColumnShowInGrid;
            EditorFormJson = FormInfo.EditorFormJson;
            GroupableField = FormInfo.GroupableField;
            NewPageUrl = FormInfo.NewPageUrl;
            UpdatePageUrl = FormInfo.UpdatePageUrl;
            NewButtonVisibleConditionQuery = FormInfo.NewButtonVisibleConditionQuery;
            GroupByQuery = FormInfo.GroupByQuery;
            SaveAtOnce = FormInfo.SaveAtOnce;
            CheckFieldDuplicateRecords = FormInfo.CheckFieldDuplicateRecords;
            ColumnWidth = FormInfo.ColumnWidth; 
            AttachmentColumnWidth = FormInfo.AttachmentColumnWidth;
            SearchWithOnkeyDown = FormInfo.SearchWithOnkeyDown;
            SearchWithOnkeyDownCoreId = FormInfo.SearchWithOnkeyDownCoreId;
            AnalystDescription = FormInfo.AnalystDescription;
            ShowNewButtonInToolbar = FormInfo.ShowNewButtonInToolbar;
            ShowUpdateButtonInToolbar = FormInfo.ShowUpdateButtonInToolbar;
            ShowAttachmentButtonInToolbar = FormInfo.ShowAttachmentButtonInToolbar;
            ShowDeleteButtonInToolbar = FormInfo.ShowDeleteButtonInToolbar;
            ShowViewButtonInToolbar = FormInfo.ShowViewButtonInToolbar;
            OpenFormType = FormInfo.OpenFormType;
        } 

        public List<InformationEntryForm> Childs(CoreDefine.Entities Entities)
        {
            List<CoreObject> ObjectList= new List<CoreObject>( CoreObject.FindChilds(CoreObjectID, Entities));
            List<InformationEntryForm> InformationList = new List<InformationEntryForm>();
            foreach(CoreObject Item in ObjectList)
            { 
                InformationList.Add(new InformationEntryForm(CoreObject.Find(Item.CoreObjectID)));
            }
            return InformationList;
        }

        public string UnsafeFullName()
        {
            return Tools.Tools.UnSafeTitle(this.InformationEntryFormName);
        }
    }
    public class SearchForm
    {
        public long RelatedTable { get; set; }
        public string FullName { get; set; }
        public string Query { get; set; }
        public string GroupByQuery { get; set; }
        public string ConditionQuery { get; set; }
        public string CommonConditionQuery { get; set; }
        public long CoreObjectID { get; set; }
        public string SelectedColumns { get; set; }
        public string Icon { get; set; } 
        public bool ShowIcon { get; set; } 
        public bool ShowText { get; set; } 
        public bool CleareGridAfterSearch { get; set; }
        public string SearchConditionQuery { get; set; }
        public string SearchAlarmQuery { get; set; }

        public SearchForm()
        {
            FullName = string.Empty;
            ConditionQuery = string.Empty;
            Query = string.Empty;
            CommonConditionQuery = string.Empty;
            SelectedColumns = string.Empty;
            Icon = string.Empty;
            RelatedTable = 0; 
            CoreObjectID = 0;
            ShowIcon=false;
            ShowText=false;
            CleareGridAfterSearch=true;
            SearchConditionQuery = string.Empty;
            SearchAlarmQuery = string.Empty;
            GroupByQuery = string.Empty;
        }  
        public SearchForm(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString() ;
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(SearchForm));
            var FormInfo= serializer.Deserialize(stringReader) as SearchForm;
            Query= FormInfo.Query.Replace("&gt;", "<"); 
            RelatedTable  = FormInfo.RelatedTable; 
            FullName = _CoreObject.FullName; 
            CoreObjectID = _CoreObject.CoreObjectID; 
            SelectedColumns = FormInfo.SelectedColumns; 
            Icon = FormInfo.Icon;
            ShowIcon = FormInfo.ShowIcon;
            ShowText = FormInfo.ShowText;
            ConditionQuery = FormInfo.ConditionQuery;
            CommonConditionQuery = FormInfo.CommonConditionQuery;
            CleareGridAfterSearch = FormInfo.CleareGridAfterSearch;
            SearchConditionQuery = FormInfo.SearchConditionQuery;
            SearchAlarmQuery = FormInfo.SearchAlarmQuery;
            GroupByQuery = FormInfo.GroupByQuery; 
        }  
    }  
}