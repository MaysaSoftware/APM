using APM.Models.APMObject;
using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Tools
{
    public class DataTank
    {
        public class DataInformationEntryForm
        {
            public InformationEntryForm this[string DataKey]
            {
                get
                {
                    return (InformationEntryForm)HttpContext.Current.Session["DataInformationEntryForm" + DataKey];
                }
                set
                {
                    List<Field> DataFields = DataConvertor.InformationEntryFormReady(value);
                    InformationEntryForm informationEntryForm = value;
                    List<ReportParameter> DataReport = DataConvertor.InformationEntryFormReport(informationEntryForm.CoreObjectID);

                    List<int> DataShowColumns = new List<int>();
                    int Index = 0;
                    foreach (Field Item in DataFields)
                    {
                        DataShowColumns.Add(Index);
                        Index++;
                        if(Item.FieldType == CoreDefine.InputTypes.RelatedTable)
                        {   
                            HttpContext.Current.Session["TableDataFields" + Item.RelatedTable] = DataConvertor.FillTableDataFields(Item.RelatedTable);
                        }
                    } 

                    HttpContext.Current.Session["DataInformationEntryForm" + DataKey] = value;
                    HttpContext.Current.Session["DataFields" + DataKey] = DataFields;
                    HttpContext.Current.Session["DataReport" + DataKey] = DataReport;
                    HttpContext.Current.Session["DataShowColumn" + DataKey] = DataShowColumns; 
                }
            }
        }
        
        public class DataTableForm
        {
            public Table this[string DataKey]
            {
                get
                {
                    return (Table)HttpContext.Current.Session["DataInformationEntryForm" + DataKey];
                }
                set
                {
                    List<Field> DataFields = DataConvertor.TableFormReady(long.Parse(DataKey));
                    List<ReportParameter> DataReport = DataConvertor.InformationEntryFormReport(long.Parse(DataKey));
                    List<int> DataShowColumns = new List<int>();

                    int Index = 0;
                    foreach (Field Item in DataFields)
                    { 
                        DataShowColumns.Add(Index);
                        Index++;
                        if(Item.FieldType == CoreDefine.InputTypes.RelatedTable)
                            HttpContext.Current.Session["TableDataFields" + Item.RelatedTable] = DataConvertor.FillTableDataFields(Item.RelatedTable);
                    }

                    HttpContext.Current.Session["DataInformationEntryForm" + DataKey] = value;
                    HttpContext.Current.Session["DataFields" + DataKey] = DataFields;
                    HttpContext.Current.Session["TableDataFields" + DataKey] = DataFields;
                    HttpContext.Current.Session["DataReport" + DataKey] = DataReport;
                    HttpContext.Current.Session["DataShowColumn" + DataKey] = DataShowColumns;
                }
            }
        }


        public class MasterDataKey_ShowWithOutPermissionConfig
        {
            public bool this[string DataKey]
            {
                get
                {
                    return (HttpContext.Current.Session["ShowWithOutPermissionConfig" + DataKey] == null ? false : (bool)HttpContext.Current.Session["ShowWithOutPermissionConfig" + DataKey]);
                }
                set
                {
                    HttpContext.Current.Session["ShowWithOutPermissionConfig" + DataKey] = value;
                }
            }

        }

        public class SessionEditorGrid
        {
            public object this[string DataKey,string ParentID]
            {
                get
                {
                    return (object)HttpContext.Current.Session["EditorGrid_" + Referral.UserAccount.UsersID.ToString() + "_" + DataKey + "_" + ParentID];
                }
                set
                {
                    HttpContext.Current.Session["EditorGrid_" + Referral.UserAccount.UsersID.ToString() + "_" + DataKey + "_" + ParentID] = value;
                }
            }

        }

        public class DataTable
        {
            public Table this[string DataKey]
            {
                get
                {
                    CoreObject Form=CoreObject.Find(long.Parse(DataKey));
                    long TableID = long.Parse(DataKey);
                    switch (Form.Entity)
                    {
                        case CoreDefine.Entities.جدول:
                            {
                                break;
                            }
                        case CoreDefine.Entities.فرم_ورود_اطلاعات:
                            {
                                InformationEntryForm MainTableForm = (InformationEntryForm)HttpContext.Current.Session["DataInformationEntryForm" + DataKey];
                                TableID = MainTableForm.RelatedTable;
                                break;
                            }
                    } 

                    return new Table(CoreObject.Find(TableID)) ;
                }
            }

        }
        public class CachedDataTable
        {
            public System.Data.DataTable this[string DataKey]
            {
                get { return (System.Data.DataTable)HttpContext.Current.Session["CachedDataInformationEntryForm" + DataKey]; }
                set { HttpContext.Current.Session["CachedDataInformationEntryForm" + DataKey] = value; }

            }
        }
        public class CachedRegisteryID
        {
            public long this[string DataKey]
            {
                get { return (long)HttpContext.Current.Session["CachedRegisteryId" + DataKey]; }
                set { HttpContext.Current.Session["CachedRegisteryId" + DataKey] = value; }
            }

        }
        
        public class CachedMasterProcessID
        {
            public long this[string DataKey]
            {
                get { return (long)HttpContext.Current.Session["CachedMasterProcessID" + DataKey]; }
                set { HttpContext.Current.Session["CachedMasterProcessID" + DataKey] = value; }
            }

        }

        public class DataFields
        {
            public List<Field> this[string DataKey]
            {
                get
                {
                    return (List<Field>)HttpContext.Current.Session["DataFields" + DataKey];
                }
                set
                {
                    HttpContext.Current.Session["DataFields" + DataKey] = value;
                }
            }

        }

        public class DataSearchForm
        {
            public SearchForm this[string DataKey]
            {
                get
                {
                    return (SearchForm)HttpContext.Current.Session["DataInformationEntryForm" + DataKey];
                }
                set
                {
                    List<Field> DataFields = DataConvertor.SearchFormReady(long.Parse(DataKey));
                    List<int> DataShowColumns = new List<int>();

                    int Index = 0;
                    foreach (Field Item in DataFields)
                    {
                        DataShowColumns.Add(Index);
                        Index++;
                    }

                    HttpContext.Current.Session["DataInformationEntryForm" + DataKey] = value;
                    HttpContext.Current.Session["DataFields" + DataKey] = DataFields;
                    HttpContext.Current.Session["DataShowColumn" + DataKey] = DataShowColumns;
                }
            }
        }
 


        public class DataReport
        {
            public List<ReportParameter> this[string DataKey]
            {
                get
                {
                    return (List<ReportParameter>)HttpContext.Current.Session["DataReport" + DataKey];
                }
                set
                {
                    HttpContext.Current.Session["DataReport" + DataKey] = value;
                }
            }

        }

        public class SysSetting
        {
            public string SysSettingType
            {
                get
                {
                    return (string)HttpContext.Current.Session["_SysSettingType" + Referral.UserAccount.UsersID];
                }
                set
                {
                    HttpContext.Current.Session["_SysSettingType" + Referral.UserAccount.UsersID] = value;
                }
            }
            public long SysSettingID
            {
                get
                {
                    return (long)HttpContext.Current.Session["_SysSettingID" + Referral.UserAccount.UsersID];
                }
                set
                {
                    HttpContext.Current.Session["_SysSettingID" + Referral.UserAccount.UsersID] = value;
                }
            }
            public string SysSettingEntity
            {
                get
                {
                    return (string)HttpContext.Current.Session["_SysSettingEntity" + Referral.UserAccount.UsersID];
                }
                set
                {
                    HttpContext.Current.Session["_SysSettingEntity" + Referral.UserAccount.UsersID] = value;
                }
            }
            public string SysSettingName
            {
                get
                {
                    return (string)HttpContext.Current.Session["_SysSettingName" + Referral.UserAccount.UsersID];
                }
                set
                {
                    HttpContext.Current.Session["_SysSettingName" + Referral.UserAccount.UsersID] = value;
                }
            }

            public string SysDataKay
            {
                get
                {
                    return "0";
                }
                set
                {

                }
            } 

        }


        public class TableDataFields
        {
            public List<Field> this[string DataKey]
            {
                get
                {
                    return (List<Field>)HttpContext.Current.Session["TableDataFields" + DataKey];
                }
                set
                {
                    HttpContext.Current.Session["TableDataFields" + DataKey] = value;
                }
            }

        }

        public class DataShowColumn
        {
            public List<int> this[string DataKey]
            {
                get
                {
                    return (List<int>)HttpContext.Current.Session["DataShowColumn" + DataKey];
                }
                set
                {
                    HttpContext.Current.Session["DataShowColumn" + DataKey] = value;
                }
            }
        }
        public class ReportCoreObject
        {
            public ReportResource this[string DataKey=""]
            {
                get
                {
                    return (ReportResource)HttpContext.Current.Session["ReportSys"];
                }
                set
                {
                    HttpContext.Current.Session["ReportSys"] = value;
                }
            }
        }
        public class ReportId
        {
            public long this[string DataKey=""]
            {
                get
                {
                    return (long)HttpContext.Current.Session["ReportIdSys" + DataKey];
                }
                set
                {
                    HttpContext.Current.Session["ReportIdSys" + DataKey] = value;
                }
            }
        }
        public class ReportParameterName
        {
            public string[] this[string DataKey]
            {
                get
                {
                    return (string[])HttpContext.Current.Session["ReportParameterName" + Referral.UserAccount.UsersID];
                }
                set
                {
                    HttpContext.Current.Session["ReportParameterName" + Referral.UserAccount.UsersID] = value;
                }
            }
        }
        public class ReportParameterValue
        {
            public string[] this[string DataKey]
            {
                get
                {
                    return (string[])HttpContext.Current.Session["ReportParameterValue" + Referral.UserAccount.UsersID];
                }
                set
                {
                    HttpContext.Current.Session["ReportParameterValue" + Referral.UserAccount.UsersID] = value;
                }
            }
        }

    }
}