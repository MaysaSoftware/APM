using APM.Models;
using APM.Models.Database;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APM.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        public ActionResult Index(string ProcessID)
        {
            return View();
        }

        public ActionResult ProcessReferral(long StepID,long _ParentID=0)
        {
            CoreObject NewStepCore = CoreObject.Find(StepID);
            ProcessStep NewStep = new ProcessStep(NewStepCore);
            CoreObject ReferralInformationEntryForm = CoreObject.Find(0,CoreDefine.Entities.فرم_ورود_اطلاعات, Desktop.StepProcessInformationEntryForm);
            bool IsEditForm = NewStep.RecordType == Tools.GetStepRecordType("ویرایش") ? true : false;

            if (NewStep.InformationEntryFormID>0)
                if (Desktop.DataInformationEntryForm[NewStep.InformationEntryFormID.ToString()] == null)
                    Desktop.StartupSetting(NewStep.InformationEntryFormID.ToString());

            if (Desktop.DataInformationEntryForm[ReferralInformationEntryForm.CoreObjectID.ToString()]==null)
                Desktop.StartupSetting(ReferralInformationEntryForm.CoreObjectID.ToString());
            Record RecordData = new Record();

            if (IsEditForm)
            {
                long TableID= Desktop.DataInformationEntryForm[NewStep.InformationEntryFormID.ToString()].RelatedTable;
                CoreObject TableCore = CoreObject.Find(TableID);
                RecordData = new Record(Referral.DBData,TableCore.FullName,_ParentID, TableCore.CoreObjectID);
            }

            List<object> FieldParameterList = new List<object>(); 

            foreach (Field Item in Desktop.DataFields[NewStep.InformationEntryFormID.ToString()])
            {
                PermissionField _PermissionField = new PermissionField(Item.CoreObjectID, Referral.UserAccount.Permition);
                bool IsEnable = _PermissionField.CanUpdate && Item.IsEditAble && !Item.IsVirtual;
                bool IsHide = !_PermissionField.CanView;
                FieldParameterList.Add(new object[]{Item.Folder, Item.FieldName,Item.FieldType,new{
                                    FieldName = Item.FieldName,
                                    InputType = Item.FieldType,
                                    IsReadonly = !IsEnable,
                                    IsRequired = Item.IsRequired,
                                    NullValue = "نامشخص",
                                    FalseValue = "خیر",
                                    TrueValue = "بله",
                                    IsInCellEditMode = false,
                                    FieldTitle = Item.Title(),
                                    DigitsAfterDecimal = Item.DigitsAfterDecimal,
                                    RelatedField = Item.RelatedField,
                                    IsGridField = true,
                                    IsLeftWrite = Item.IsLeftWrite,
                                    RelatedTable = Item.RelatedTable,
                                    FieldValue=RecordData.Field( Item.FieldName,null)
                                } });
            }

            ViewData["DataKey"] = NewStep.InformationEntryFormID.ToString();
            ViewData["_ParentID"] = _ParentID.ToString();
            ViewData["RecordData"] = RecordData;
            ViewData["FieldParameterList"] = FieldParameterList;
            ViewData["ReferralProcessDataKey"] = ReferralInformationEntryForm.CoreObjectID.ToString();
            ViewData["FolderDataKey"] = Desktop.ToFolderNames(ReferralInformationEntryForm.CoreObjectID.ToString());
            ViewData["ReferralProcessActionData"] = "function() {return {_DataKey: '" + ReferralInformationEntryForm.CoreObjectID.ToString() + "', _ParentID:"+ _ParentID + ",ProcessStep:"+ NewStepCore.CoreObjectID.ToString() + " } }";
            Desktop.SessionEditorGrid[ReferralInformationEntryForm.CoreObjectID.ToString(), _ParentID.ToString()] =null ;
            return View();
        }


        public JsonResult SaveProcessReferral(string[] ReferralInputName,object[] ReferralInputValue,string[] FormInputName,object[] FormInputValue)
        {
            long InsertFormID = 0;
            long TableID = 0;
            string FormDataKey = "0";
            CoreObject ProccessTable = CoreObject.Find(Referral.MasterDatabaseID, CoreDefine.Entities.جدول, "فرآیند");

            if (FormInputName !=null)
            {
                FormDataKey = FormInputName.Where(item => item.StartsWith("شناسه")).ElementAt(0).Split('_').ElementAt(1);
                TableID= Desktop.DataInformationEntryForm[FormDataKey].RelatedTable;

                string _RecordID = FormInputValue[Array.IndexOf(FormInputName, "شناسه_" + FormDataKey)].ToString();


                for (int Index = 0; Index < FormInputName.Length; Index++)
                    FormInputName[Index] = FormInputName[Index].Replace("_" + FormDataKey, "");

                if (_RecordID != "0")
                {
                    Desktop.Update(TableID, long.Parse(_RecordID), FormInputName, FormInputValue);
                    InsertFormID = long.Parse(_RecordID);
                }
                else
                {
                    InsertFormID = Desktop.Create(TableID.ToString(), FormInputName, FormInputValue);
                    Desktop.SaveSubInformationEntryForm(FormDataKey, InsertFormID, 0);

                }
            }

            string ReferralDataKey = ReferralInputName.Where(item => item.StartsWith("شناسه")).ElementAt(0).Split('_').ElementAt(1);
            Array.Resize(ref ReferralInputName, ReferralInputName.Length+6);
            ReferralInputName[ReferralInputName.Length - 6] = "ساعت_ثبت";
            ReferralInputName[ReferralInputName.Length - 5] = "فرآیند";
            ReferralInputName[ReferralInputName.Length - 4] = "مرحله_فرآیند";
            ReferralInputName[ReferralInputName.Length - 3] = "فرم_جدولی";
            ReferralInputName[ReferralInputName.Length - 2] = "رکورد";
            ReferralInputName[ReferralInputName.Length - 1] = "جدول";

            Array.Resize(ref ReferralInputValue, ReferralInputValue.Length + 6);
            ReferralInputValue[ReferralInputValue.Length - 6] = CDateTime.GetNowTime();
            ReferralInputValue[ReferralInputValue.Length - 5] = Session["ProcessID"];
            ReferralInputValue[ReferralInputValue.Length - 4] = Session["ProcessStep"];
            ReferralInputValue[ReferralInputValue.Length - 3] = FormDataKey;
            ReferralInputValue[ReferralInputValue.Length - 2] = InsertFormID;
            ReferralInputValue[ReferralInputValue.Length - 1] = TableID;
             
            for (int Index = 0; Index < ReferralInputName.Length; Index++)
                ReferralInputName[Index] = ReferralInputName[Index].Replace("_" + ReferralDataKey, "");

            long RecordID = Desktop.Create(Desktop.DataInformationEntryForm[ReferralDataKey].RelatedTable.ToString(), ReferralInputName, ReferralInputValue);
            CoreObject TableCore = CoreObject.Find(Desktop.DataInformationEntryForm[ReferralDataKey].RelatedTable);

            CoreObject StepCore = CoreObject.Find((long)Session["ProcessStep"]);
            ProcessStep step = new ProcessStep(StepCore);
            Referral.DBData.UpdateRow((long)Session["ProcessID"], ProccessTable.CoreObjectID,ProccessTable.FullName , new string[] { "آخرین_اقدام", "درصد_پیشرفت" }, new object[] { Session["ProcessStep"],step.ProgressPercent });
            Desktop.SaveSubInformationEntryForm(ReferralDataKey, RecordID,0);


             
            //string DeclareParentID = Referral.DBData.DefineVariablesQuery("مراحل_فرآیند", RecordID); 
            //foreach (CoreObject Events in CoreObject.FindChilds((long) Session["ProcessStep"],CoreDefine.Entities.رویداد_مرحله_فرآیند))
            //{
            //    ProcessStepEvent StepEvent = new ProcessStepEvent(Events);
            //    Referral.DBData.Execute(DeclareParentID+ Tools.CheckQuery(StepEvent.Command));
            //}

            return Json(RecordID);
        }
    }
}