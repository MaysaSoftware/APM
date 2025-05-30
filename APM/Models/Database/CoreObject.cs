using APM.Models.Security;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace APM.Models.Database
{
    public class CoreObject
    {
        public long CoreObjectID { get; set; }
        public long ParentID { get; set; }
        public CoreDefine.Entities Entity { get; set; }
        public string Folder { get; set; }
        public string FullName { get; set; }
        public long OrderIndex { get; set; }
        public bool IsDefault { get; set; }
        public object Value { get; set; }

        public CoreObject()
        {
            this.CoreObjectID = 0;
            this.ParentID = 0;  
            this.OrderIndex = 0;
            this.Folder=String.Empty;
            this.FullName = String.Empty;
            this.IsDefault = false;
        }

        public CoreObject(long CoreObjectID, long ParentID, CoreDefine.Entities Entity, string Folder, string FullName, long OrderIndex, bool IsDefault,object Value)
        {
            this.CoreObjectID = CoreObjectID;
            this.ParentID = ParentID;
            this.Entity = Entity;
            this.Folder = Folder;
            this.FullName = FullName;
            this.OrderIndex = OrderIndex;
            this.IsDefault = IsDefault;
            this.Value=Value;
        }
        public static List<CoreObject> FindChilds(long ParentID,CoreDefine.Entities _Entity)
        {
            Log.LogFunction("CoreObject.FindChilds", true);
            List < CoreObject > children = new List < CoreObject >(Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == _Entity));
            return children;
        }
        public static List<CoreObject> FindChilds(string _FullName)
        {
            Log.LogFunction("CoreObject.FindChilds", true);
            List < CoreObject > children = new List < CoreObject >(Referral.CoreObjects.Where(item => item.FullName==_FullName));
            return children;
        }
        public static List<CoreObject> FindChilds(long ParentID,CoreDefine.Entities _Entity, string _FullName)
        {
            Log.LogFunction("CoreObject.FindChilds", true);
            List < CoreObject > children = new List < CoreObject >(Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == _Entity && item.FullName==_FullName));
            return children;
        }

        public static List<CoreObject> FindChildsInFolder(long ParentID,CoreDefine.Entities _Entity, string _Folder)
        {
            Log.LogFunction("CoreObject.FindChildsInFolder", true);
            List < CoreObject > children = new List < CoreObject >(Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.Entity == _Entity && ( item.Folder== _Folder || item.Folder.StartsWith(_Folder+"/")) ));
            return children;
        }

        public static CoreObject FindFloder(long ParentID, CoreDefine.Entities _Entity, string _Folder)
        {
            Log.LogFunction("CoreObject.FindFloder", true); 
            switch(_Entity)
            {
                case CoreDefine.Entities.فرم_ورود_اطلاعات :_Folder = "InformationEntryForm_" + _Folder;break;
                case CoreDefine.Entities.فیلد :_Folder = "Field_" + _Folder;break;
                case CoreDefine.Entities.گزارش :_Folder = "Report_" + _Folder;break; 
            }
            List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.ParentID == ParentID && item.FullName == _Folder.Replace(" ", "_").Replace("/", "_") && item.Entity == CoreDefine.Entities.پوشه));
            if(children.Count > 0)    
            return children[0];
            return new CoreObject();
        }

        public static List<CoreObject> FindChilds(long ParentID)
        {
            Log.LogFunction("CoreObject.FindChilds", true);
            List < CoreObject > children = new List < CoreObject >(Referral.CoreObjects.Where(item => item.ParentID == ParentID));
            return children;
        }

        public static List<CoreObject> FindChilds(CoreDefine.Entities _Entity)
        {
            Log.LogFunction("CoreObject.FindChilds", true);
            List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.Entity == _Entity));
            return children;
        }
        public static CoreObject Find(CoreDefine.Entities _Entity, string _FullName)
        {
            Log.LogFunction("CoreObject.Find", true);
            List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.FullName == _FullName && item.Entity == _Entity));
            if(children.Count > 0)
                return children[0];
            return new CoreObject();  
        }
        public static CoreObject Find(long _CoreObjectID)
        {
            Log.LogFunction("CoreObject.Find", true);
            if (_CoreObjectID==0) 
                return new CoreObject();
            else
            { 
                List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.CoreObjectID == _CoreObjectID));
                if (children.Count > 0)
                    return children[0];
            }
            return new CoreObject();  
        }


        public static CoreObject GetSourceReport(long ParentID)
        {
            Log.LogFunction("CoreObject.GetSourceReport", true);
            int CoreIndex = Referral.CoreObjects.FindIndex(x => x.ParentID == ParentID && x.Entity==CoreDefine.Entities.منبع_گزارش);
            Log.LogFunction("Software.ThreadCoreReload", false);
            CoreObject ReportCore=new CoreObject();
            if(CoreIndex<0)
            {
                DataTable CoreData = Referral.DBCore.SelectDataTable("SELECT  CoreObjectID, ParentID, Entity, Folder, FullName, OrderIndex, IsDefault, value FROM  CoreObject where Entity = N'منبع_گزارش'  and ParentID=" + ParentID.ToString());
                ReportCore = new CoreObject(
                       Convert.ToInt64(CoreData.Rows[0]["CoreObjectID"].ToString()),
                       Convert.ToInt64(CoreData.Rows[0]["ParentID"].ToString()),
                       Tools.Tools.GetEntity(CoreData.Rows[0]["Entity"].ToString()),
                       CoreData.Rows[0]["Folder"].ToString(),
                       CoreData.Rows[0]["FullName"].ToString(),
                       Convert.ToInt64(CoreData.Rows[0]["OrderIndex"].ToString() == "" ? "0" : CoreData.Rows[0]["OrderIndex"].ToString()),
                       (bool)CoreData.Rows[0]["IsDefault"],
                       (object)CoreData.Rows[0]["Value"]
                        );
                Referral.CoreObjects.Add(ReportCore);

            }
            else
            {
                ReportCore = Referral.CoreObjects[CoreIndex];
            }
            return ReportCore; 
        }

        public static CoreObject Find(long _ParentID, CoreDefine.Entities _Entity, string _FullName)
        {
            Log.LogFunction("CoreObject.Find", true); 
            List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.ParentID == _ParentID && item.Entity == _Entity && item.FullName == _FullName));
            if (children.Count > 0)
                return children[0];
            return new CoreObject(); 
        }

        public static List<CoreObject> DefaultChild(long _ParentID)
        {
            Log.LogFunction("CoreObject.Find", true);
            List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.ParentID == _ParentID && item.IsDefault == true));
            return (children);
        }

        public string Title()
        {
            return Tools.Tools.UnSafeTitle(this.FullName);
        }
        public PermissionBase Permission(string _RoleType)
        {
            return new PermissionBase(this.CoreObjectID, _RoleType);
        }
        
    }


}