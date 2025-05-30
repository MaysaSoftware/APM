using APM.Models.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace APM.Models.NetWork
{
    public class TaskReferral
    {
        public string[] ReferralRecipientsUser { get; set; }
        public string[] ReferralRecipientsRole { get; set; }
        public string ReferralRecipientsQuery { get; set; }
        public int ReferralDeadlineResponse { get; set; }
        public string ReferralTitle { get; set; }
        public string ReferralTitleQuery { get; set; }
        public long ProcessID { get; set; }
        public long ProcessStepID { get; set; }
        public long InformarmationFormID { get; set; }
        public long RecordID { get; set; }
        public long TableID { get; set; }
        public long ParentID { get; set; }

        public void FinalSendTask(List<ReferralUser> ReferralUserAccount, ref string ErrorMessage)
        {
            string DeadlineDate = Tools.CDateTime.AddDay(Tools.CDateTime.GetNowshamsiDate(), ReferralDeadlineResponse);

            foreach (ReferralUser ReferralUserItem in ReferralUserAccount)
                Referral.DBData.Execute("Insert into  ارجاع_مراحل_فرآیند (" +
                    "دریافت_کننده," +
                    " مشاهده_شده," +
                    " تاریخ_مشاهده," +
                    " ساعت_مشاهده," +
                    " تاریخ_مهلت_پاسخگویی," +
                    " دستور_ارجاع," +
                    " سمت_سازمانی_دریافت_کننده," +
                    " تاریخ_ثبت," +
                    " ساعت_ثبت," +
                    " ثبت_کننده," +
                    " فرآیند," +
                    " مرحله_فرآیند," +
                    " فرم_ورود_اطلاعات," +
                    " رکورد," +
                    " جدول," +
                    " نام_ثبت_کننده," +
                    "سمت_سازمانی_ثبت_کننده," +
                    "شناسه_مافوق)" +
                    " values(" + ReferralUserItem.UserID + "," +
                    "0," +
                    "N''," +
                    "N''," +
                    "N'" + DeadlineDate + "'," +
                    "N'" + ReferralTitle+" "+ ReferralTitleQuery + "'," +
                    ReferralUserItem.PostID + "," +
                    "N'" + Tools.CDateTime.GetNowshamsiDate() + "'," +
                    "N'" + Tools.CDateTime.GetNowTime() + "'," +
                    Referral.UserAccount.UsersID + "," +
                    ProcessID+"," +
                    ProcessStepID+"," +
                    InformarmationFormID + "," +
                    RecordID + "," +
                    TableID + "," +
                    "N'"+Referral.UserAccount.FullName.ToString() + "'," +
                    Referral.UserAccount.PersonnelPostID+ "," +
                    ParentID+ ")");

            Referral.DBData.Execute("update ارجاع_مراحل_فرآیند set عنوان_سمت_سازمانی_ثبت_کننده = (select سمت_سازمانی.عنوان from سمت_سازمانی where سمت_سازمانی.شناسه = سمت_سازمانی_ثبت_کننده), " +
                "عنوان_سمت_سازمانی_دریافت_کننده = (select سمت_سازمانی.عنوان from سمت_سازمانی where سمت_سازمانی.شناسه = سمت_سازمانی_دریافت_کننده) " +
                "where(ISNULL(عنوان_سمت_سازمانی_دریافت_کننده, N'') = N'' and  سمت_سازمانی_دریافت_کننده > 0) or(ISNULL(عنوان_سمت_سازمانی_ثبت_کننده, N'') = N'' and  سمت_سازمانی_ثبت_کننده > 0)");

        }


        public void SyncSendTask(string DeclareQuery, string DataKey, string RecordID, string TableID, string[] ColumnNames, object[] _Values)
        {
            CoreObject coreObject = CoreObject.Find(long.Parse(DataKey));
            if (coreObject.Entity == Tools.CoreDefine.Entities.فرم_ورود_اطلاعات)
            {
                InformationEntryForm informationEntryForm = new InformationEntryForm(coreObject);
                if(informationEntryForm.ExternalField>0)
                {
                    CoreObject ExternalFieldCore =CoreObject.Find(informationEntryForm.ExternalField);
                    int FindIndex=Array.IndexOf(ColumnNames, ExternalFieldCore.FullName);
                    if(FindIndex>-1)
                        ParentID=long.Parse(_Values[FindIndex].ToString()) ;
                }
            }
            string Query = "Select کاربر.سمت_سازمانی, کاربر.شناسه , کاربر.نام_و_نام_خانوادگی From کاربر left join  نقش_کاربر on کاربر.نقش_کاربر = نقش_کاربر.شناسه  where 1<>1 ";

            foreach (string Item in ReferralRecipientsUser)
                if (!string.IsNullOrEmpty(Item))
                {
                    string[] Temp = Item.Split('[');
                    long UserID = long.Parse(Temp[Temp.Length - 1].Replace("]", ""));
                    if (UserID < 1)
                    {
                        switch (UserID)
                        {
                            case -1: { Query += "OR کاربر.شناسه in (select [مراحل_فرآیند].[اجرا_کننده] from [مراحل_فرآیند] where [مراحل_فرآیند].[سطر] = "+RecordID+" AND [مراحل_فرآیند].[فرآیند] = "+ProcessID+"  AND [مراحل_فرآیند].[جدول] = "+TableID+" AND ([مراحل_فرآیند].[تاریخ_ثبت]+[مراحل_فرآیند].[ساعت_ثبت])=(select MIN(A.[تاریخ_ثبت]+A.[ساعت_ثبت]) from [مراحل_فرآیند] AS A where A.[سطر] = "+RecordID+" AND A.[فرآیند] = "+ProcessID+"  AND A.[جدول] = "+TableID+"  ) )"; break; }
                            case 0: { Query += "OR  1=1"; break; }
                        }
                    }
                    else
                       Query += "OR  کاربر.شناسه = " + Temp[Temp.Length-1].Replace("]", "") + " \n";
                }
 

            foreach (string Item in ReferralRecipientsRole)
                if (!string.IsNullOrEmpty(Item))
                {
                    string[] Temp = Item.Split('[');
                    long RoleID = long.Parse(Temp[Temp.Length - 1].Replace("]", ""));
                    if (RoleID < 1)
                    { 
                        switch (RoleID)
                        {
                            case 0: { Query += "OR  1=1"; break; }
                            case -1: {
                                    string PersonnelPostID = Referral.DBData.SelectField("SELECT  سمت_مافوق   FROM  سمت_سازمانی Where شناسه = " + Referral.UserAccount.PersonnelPostID.ToString()).ToString();
                                    if (PersonnelPostID != "")
                                        Query += "\nOR سمت_سازمانی = "+ PersonnelPostID+"\n";
                                    break; }
                        }
                    }
                    else
                        Query += "OR  کاربر.سمت_سازمانی = " + RoleID.ToString() + " \n";

                }

            List<ReferralUser> ReferralUserList = new List<ReferralUser>();
            DataTable UserData= Referral.DBData.SelectDataTable(Query);
            foreach (DataRow Row in UserData.Rows)
            {
                ReferralUserList.Add(new ReferralUser() { 
                PostID = long.Parse( Row[0].ToString()),
                UserID = long.Parse( Row[1].ToString()),
                UserName = Row[2].ToString(),
                });
            }

            if (!string.IsNullOrEmpty(ReferralRecipientsQuery))
            {
                UserData = Referral.DBData.SelectDataTable(DeclareQuery + "\n" + Tools.Tools.CheckQuery(ReferralRecipientsQuery));

                foreach (DataRow Row in UserData.Rows)
                {
                    ReferralUserList.Add(new ReferralUser()
                    {
                        PostID = long.Parse(Row[0].ToString()),
                        UserID = long.Parse(Row[1].ToString()),
                        UserName = Row[2].ToString(),
                    });
                } 
            }


            ReferralUserList = ReferralUserList.Distinct().ToList();
            ReferralUserList.RemoveAll(x => x.UserID==0);

            if (ReferralUserList.Count > 0)
            {
                for (int Index = 0; Index < ColumnNames.Length; Index++)
                {
                    if (ReferralTitle.IndexOf("@" + ColumnNames[Index] + " ") > -1)
                        ReferralTitle = ReferralTitle.Replace("@" + ColumnNames[Index] + " ", _Values[Index].ToString());

                }

                if (!string.IsNullOrEmpty(ReferralTitleQuery))
                    ReferralTitleQuery += " " + Referral.DBData.SelectField(DeclareQuery + "\n" + Tools.Tools.CheckQuery(ReferralTitleQuery)).ToString();

                string ErrorMessage = "";
                FinalSendTask(ReferralUserList, ref ErrorMessage);
            }
        }



    }

    public class ReferralUser
    {
        public long PostID { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; } 
        public ReferralUser()
        {

        }

    }
}