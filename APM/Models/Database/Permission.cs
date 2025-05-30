using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.Database
{
    public class Permission
    {
    }



    public partial class PermissionBase
    {
        public long CoreObjectID;
        public bool IsAllow=false;

        public PermissionBase(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.IsAllow = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString());
                if (!IsAllow)
                    this.IsAllow = _RoleType.IndexOf("," + _CoreObjectID.ToString() + "_") > -1;
            }
        }
    }



    public partial class PermissionItem
    { 
        public bool IsAllow = false;

        public PermissionItem(string Item, string _RoleType)
        { 
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.IsAllow = Array.Exists(PermissionArraye, element => element == Item);
                if(!IsAllow)
                    this.IsAllow = _RoleType.IndexOf("," + Item + "_")>-1;
            }
        }
    }

    public class PermissionTable
    { 
        public bool IsUpdateLimitByInserterUser { get; set; }
        public bool CanDeleteAttachment { get; set; }
        public bool CanUpdateAttachment { get; set; }
        public bool CanInsertAttachment { get; set; }
        public bool CanOpenAttachment  { get; set; }
        public bool CanDownloadAttachment { get; set; }
        public bool CanDelete { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanInsert { get; set; }
        public long CoreObjectID;

        public PermissionTable()
        {
            this.IsUpdateLimitByInserterUser = false;
            this.CanDeleteAttachment = false;
            this.CanUpdateAttachment = false;
            this.CanInsertAttachment = false;
            this.CanOpenAttachment = false;
            this.CanDownloadAttachment = false;
            this.CanDelete = false;
            this.CanUpdate = false;
            this.CanInsert = false;
        }
        public PermissionTable(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanInsert = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsert");
                this.CanUpdate = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdate");
                this.CanDelete = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDelete");
                this.CanOpenAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanOpenAttachment");
                this.CanInsertAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsertAttachment");
                this.CanUpdateAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateAttachment");
                this.CanDownloadAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDownloadAttachment");
                this.CanDeleteAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDeleteAttachment");
                this.IsUpdateLimitByInserterUser = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_IsUpdateLimitByInserterUser");
            }

        }
    }

    
    public class PermissionTableAttachment
    { 
        public bool CanOpenAttachment = false; 
        public bool CanUpload = false;
        public bool CanDownload = false; 
        public long CoreObjectID;

        public PermissionTableAttachment()
        { 
            this.CanOpenAttachment = false;
            this.CanUpload = false;
            this.CanDownload = false;
        }
        public PermissionTableAttachment(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanOpenAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanOpenAttachment");
                this.CanUpload = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpload");
                this.CanDownload = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDownload");
            }

        }
    }

    public class PermissionField
    { 
        public bool CanView = false; 
        public bool CanUpdate = false; 
        public long CoreObjectID; 
        public PermissionField(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanView = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanView");
                this.CanUpdate = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdate");

            }
        }
    }

    public class PermissionComputationalField
    { 
        public bool CanView = false;  
        public long CoreObjectID; 
        public PermissionComputationalField(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanView = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanView"); 

            }
        }
    }


    public class PermissionInformationEntryForm
    {  
        public bool CanExportReport = false;
        public bool CanOpenAttachment = false;
        public bool CanDelete = false;
        public bool CanUpdate = false;
        public bool CanUpdateOnlyUserRegistry = false;
        public bool CanUpdateOneDey = false;
        public bool CanUpdateThreeDey = false;
        public bool CanUpdateOneWeek = false;
        public bool CanInsert = false; 
        public bool CanView = false; 
        public bool CanShowComputedFieldInEditForm = false; 
        public bool CanShowEventEditRecord = false; 
        public bool CanShowEventEditAll = false; 
        public bool CanShowEventInsertRecord = false; 
        public bool CanShowEventInsertAll = false; 
        public bool CanShowEventDeleteAll = false; 
        public bool CanShowEventDownloadRecord = false;
        public bool CanShowEventDownloadTable = false;
        public bool CanShowEventViewRecord = false;
        public bool CanShowEventViewTable = false;
        public bool CanImportData = false;
        public bool CanShowCountRow = false;
        public bool CanShowAutoFit = false;
        public bool CanSendEmail = false;
        public long CoreObjectID;

        public PermissionInformationEntryForm(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanInsert = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsert");
                this.CanUpdate = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdate");
                this.CanUpdateOnlyUserRegistry = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateOnlyUserRegistry");
                this.CanUpdateOneDey = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateOneDey");
                this.CanUpdateThreeDey = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateThreeDey");
                this.CanUpdateOneWeek = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateOneWeek");
                this.CanDelete = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDelete");
                this.CanView = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanView");
                this.CanOpenAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanOpenAttachment");
                this.CanExportReport = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanExportReport");
                this.CanShowComputedFieldInEditForm = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowComputedFieldInEditForm");
                this.CanShowEventEditRecord = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventEditRecord");
                this.CanShowEventEditAll = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventEditAll");
                this.CanShowEventInsertRecord = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventInsertRecord");
                this.CanShowEventInsertAll = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventInsertAll");
                this.CanShowEventDeleteAll = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventDeleteAll");
                this.CanShowEventDownloadRecord = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventDownloadRecord");
                this.CanShowEventDownloadTable = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventDownloadTable"); 
                this.CanShowEventViewRecord = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventViewRecord");
                this.CanShowEventViewTable = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowEventViewTable"); 
                this.CanImportData = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanImportData"); 
                this.CanShowCountRow = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowCountRow"); 
                this.CanShowAutoFit = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowAutoFit"); 
                this.CanSendEmail = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanSendEmail"); 
            }
        }

    }

    public class PermissionTableButton
    {
        public bool CanDeleteAttachment = false;
        public bool CanUpdateAttachment = false;
        public bool CanInsertAttachment = false;
        public bool CanOpenAttachment = false;
        public bool CanDelete = false;
        public bool CanUpdate = false;
        public bool CanInsert = false;
        public bool CanShow = false;
        public bool CanShowComputedFieldInEditForm = false;
        public long CoreObjectID;

        public PermissionTableButton(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanShow = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShow");
                this.CanInsert = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsert");
                this.CanUpdate = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdate");
                this.CanDelete = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDelete");
                this.CanOpenAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanOpenAttachment");
                this.CanInsertAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsertAttachment");
                this.CanUpdateAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateAttachment");
                this.CanDeleteAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDeleteAttachment");
                this.CanShowComputedFieldInEditForm = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowComputedFieldInEditForm");
            }
        }
    }
    
    public class PermissionProcesses
    {
        public bool CanDeleteAttachment = false;
        public bool CanUpdateAttachment = false;
        public bool CanInsertAttachment = false;
        public bool CanOpenAttachment = false;
        public bool CanDelete = false;
        public bool CanUpdate = false;
        public bool CanInsert = false;
        public bool CanShow = false;
        public bool CanShowComputedFieldInEditForm = false;
        public long CoreObjectID;

        public PermissionProcesses(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanShow = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShow");
                this.CanInsert = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsert");
                this.CanUpdate = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdate");
                this.CanDelete = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDelete");
                this.CanOpenAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanOpenAttachment");
                this.CanInsertAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanInsertAttachment");
                this.CanUpdateAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanUpdateAttachment");
                this.CanDeleteAttachment = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanDeleteAttachment");
                this.CanShowComputedFieldInEditForm = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShowComputedFieldInEditForm");
            }
        }
    }

    public class PermissionDashboard
    {
        public bool CanShow = false;
        public long CoreObjectID;
        public PermissionDashboard(long _CoreObjectID, string _RoleType)
        {
            this.CoreObjectID = _CoreObjectID;
            if (!string.IsNullOrEmpty(_RoleType))
            {
                string[] PermissionArraye = _RoleType.Split(',');
                this.CanShow = Array.Exists(PermissionArraye, element => element == _CoreObjectID.ToString() + "_CanShow"); 
            }
        }

    }

}