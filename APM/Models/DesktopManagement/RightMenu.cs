using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.Diagram;
using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.DesktopManagement
{
    public static class RightMenu
    {
        public static void RightBaseMenuFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base)
        { 
                _Base.Add().Text(@"<span class=""k-icon k-i-home TreeIconColor""></span>" + "خانه").Id("Home").Encoded(false);
                CoreObjectMenuFiller(_Base, CoreObject.FindChilds(0, CoreDefine.Entities.فرم_ورود_اطلاعات), "", "InformationEntryForm");
                CoreObjectMenuFiller(_Base, CoreObject.FindChilds(0, CoreDefine.Entities.فرآیند), "", "Process");
                if ((new PermissionItem("0_Report", Referral.UserAccount.Permition)).IsAllow)
                    _Base.Add().Text(@"<span class=""k-icon k-i-graph TreeIconColor""></span>" + "گزارش").Id("0").Encoded(false).Items(InputItems => InputItems.CoreObjectMenuFiller(CoreObject.FindChilds(0, CoreDefine.Entities.گزارش), "", "Report"));

                if ((new PermissionItem("0_Dashboard", Referral.UserAccount.Permition)).IsAllow)
                    _Base.Add().Text(@"<span class=""k-icon k-i-graph TreeIconColor""></span>" + "داشبورد").Id("0").Encoded(false).Items(InputItems => InputItems.CoreObjectMenuFiller(CoreObject.FindChilds(0, CoreDefine.Entities.داشبورد), "", "Dashboard"));

        }

        public static void CoreObjectMenuFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, List<CoreObject> _Items, string _Folder = "", string Key = "")
        {
            try
            {
                List<Folder> Folders = new List<Folder>();
                string FolderPrefix = _Folder;
                if (FolderPrefix != "") FolderPrefix += "/";

                foreach (CoreObject Item in _Items)
                {
                    if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                    {
                        InformationEntryForm informationEntryForm = new InformationEntryForm(Item);
                        if ((new PermissionItem(Item.CoreObjectID.ToString(), Referral.UserAccount.Permition)).IsAllow)
                            if (informationEntryForm.ShowInMenueTreeList)
                                if (Item.Folder.StartsWith(FolderPrefix))
                                {
                                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);
                                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                                        if (Folders.FindIndex(x => x.FullName == FolderParts[0]) == -1)
                                        { 
                                            CoreObject FolderCore = CoreObject.FindFloder(Item.ParentID, Item.Entity, FolderPrefix+ FolderParts[0]); 
                                            if(FolderCore.CoreObjectID>0)
                                            {
                                                FolderCore.FullName = FolderParts[0];
                                                Folders.Add(new Folder(FolderCore));
                                            }
                                            else
                                                Folders.Add(new Folder() { FullName = FolderParts[0] });
                                        } 
                                }
                    }
                    else
                    {
                        if (Item.Folder.StartsWith(FolderPrefix))
                        {
                            string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                            if (FolderParts.Length > 0 && FolderParts[0] != "")
                                if (Folders.FindIndex(x => x.FullName == FolderParts[0]) == -1)
                                    Folders.Add(new Folder() { FullName = FolderParts[0] });
                        }
                    }
                }


                if (_Items.Count > 0 && Folders.Count > 0)
                {
                    List<CoreObject> FolderCoreList = CoreObject.FindChilds(_Items[0].ParentID, CoreDefine.Entities.پوشه);
                    foreach (CoreObject Item in FolderCoreList)
                    {
                        Folder folder = new Folder(Item);
                        folder.FullName =Tools.Tools.UnSafeTitle(Item.FullName);
                        folder.CoreObjectID = Item.CoreObjectID;
                        if (Folders.FindIndex(x => x.FullName == Tools.Tools.UnSafeTitle(Item.FullName)) == -1)
                        {
                            Folders.Add(folder);
                        }
                        else
                        {
                            int FindIndex = Folders.FindIndex(x => x.FullName == Tools.Tools.UnSafeTitle(Item.FullName));
                            Folders[FindIndex] = folder;
                        }
                    }
                }

             


 

                foreach (Folder Folder in Folders)
                    if ((new PermissionItem(Key + Folder.FullName, Referral.UserAccount.Permition)).IsAllow)
                    { 
                        if (Folder.CoreObjectID > 0)
                        {
                            _Base.Add()
                                .Id("0")
                                .Encoded(false) 
                                .Expanded(Folder.IsExpand) 
                                .Text(@"<span class=""k-treeview-leaf-text k-icon " + Folder.Icon+ @""" style=""color:" + Folder.IconColor + "; font-size:" + Folder.IconSize+ @"px;""></span>"+Tools.Tools.UnSafeTitle(Folder.FullName))
                                .Items(items => items.CoreObjectMenuFiller(_Items, FolderPrefix + Tools.Tools.UnSafeTitle(Folder.FullName), Key));
                        }
                        else
                        {
                            _Base.Add()
                                .Id("0")
                                .Encoded(false)
                                .SpriteCssClasses("k-icon k-i-folder")
                                .Text(Tools.Tools.UnSafeTitle(Folder.FullName))
                                .Items(items => items.CoreObjectMenuFiller(_Items, FolderPrefix + Tools.Tools.UnSafeTitle(Folder.FullName), Key));
                        }
                    }
                    else
                    { 
                        List<CoreObject> FolderCoreList = CoreObject.FindChildsInFolder(0, CoreDefine.Entities.فرم_ورود_اطلاعات, Folder.FullName);
                        bool IsFind = false;
                        foreach (CoreObject Item in FolderCoreList)
                        {
                            if ((new PermissionItem(Item.CoreObjectID.ToString(), Referral.UserAccount.Permition)).IsAllow)
                            {
                                IsFind = true;
                                break;
                            }
                        }
                        if(IsFind)
                        {
                            _Base.Add()
                                .Id("0")
                                .Encoded(false)
                                .SpriteCssClasses("k-icon k-i-folder")
                                .Text(Tools.Tools.UnSafeTitle(Folder.FullName))
                                .Items(items => items.CoreObjectMenuFiller(_Items, FolderPrefix + Tools.Tools.UnSafeTitle(Folder.FullName), Key));
                        }
                    }




                foreach (CoreObject Item in _Items)
                {
                    bool IsAllow=false; 
                    IsAllow = Item.Permission(Referral.UserAccount.Permition).IsAllow;
                    if (IsAllow && Item.Folder == _Folder)
                    {
                        string icon = Icon.IconName(Item.Entity);
                        string Badge = "";

                        if (Item.Entity == CoreDefine.Entities.فرآیند)
                        {
                            ProcessType process = new ProcessType(Item);
                            if (!string.IsNullOrEmpty(process.Icon))
                                icon = process.Icon;

                            InformationEntryForm informationEntryForm = new InformationEntryForm(CoreObject.Find(process.InformationEntryFormID));
                            if (!string.IsNullOrEmpty(informationEntryForm.BadgeQuery))
                                Badge = @"<span class='InformationEntryFormBadge' id='InformationEntryFormBadge_" + informationEntryForm.CoreObjectID.ToString() + @"' style=""font-size: 12px;background-color:" + informationEntryForm.BadgeColor + @"; color:" + informationEntryForm.BadgeTextColor + @";border-radius:3px;padding: 0px 5px;margin-right: 5px;display:none;""  title=""" + informationEntryForm.BadgeTitle + @"""></span>";


                            _Base.Add()
                            .Id(Key + Item.CoreObjectID.ToString())
                            .SpriteCssClasses(icon)
                            .Text(@"<span class=""CoreObjectMenuTreeText"">" + Tools.Tools.UnSafeTitle(Item.FullName) + @"</span>" + Badge)
                            .Encoded(false);
                        }
                        else if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                        {
                            InformationEntryForm informationEntryForm = new InformationEntryForm(Item);
                            if (!string.IsNullOrEmpty(informationEntryForm.Icon))
                                icon = informationEntryForm.Icon;
                            if (!string.IsNullOrEmpty(informationEntryForm.BadgeQuery))
                               Badge = @"<span class='InformationEntryFormBadge' id='InformationEntryFormBadge_" + informationEntryForm.CoreObjectID.ToString()+ @"' style=""font-size: 12px;background-color:" + informationEntryForm.BadgeColor + @"; color:" + informationEntryForm.BadgeTextColor + @";border-radius:3px;padding: 0px 5px;margin-right: 5px;display:none;""  title=""" + informationEntryForm.BadgeTitle + @"""></span>";


                            if (informationEntryForm.ShowInMenueTreeList)
                                _Base.Add()
                                 .Id(Key + Item.CoreObjectID.ToString()+"_"+ informationEntryForm.OpenFormType.ToString())
                                 .SpriteCssClasses(icon)
                                 .Text(@"<span class=""CoreObjectMenuTreeText"">" + Tools.Tools.UnSafeTitle(Item.FullName) + @"</span>" + Badge)
                                 .Encoded(false);
                        }
                        else if (Item.Entity == CoreDefine.Entities.گزارش)
                        {
                            Report Report = new Report(Item);
                            if (Report.ShowInMainMenu)
                            {
                                if (!string.IsNullOrEmpty(Report.Icon))
                                    icon = Report.Icon;

                                _Base.Add()
                                 .Id(Key + Item.CoreObjectID.ToString())
                                 .SpriteCssClasses(icon)
                                 .Text(@"<span class=""CoreObjectMenuTreeText"">" + Tools.Tools.UnSafeTitle(Item.FullName) + @"</span>" + Badge)
                                 .Encoded(false);
                            }
                        }
                        else
                        {
                            _Base.Add()
                             .Id(Key + Item.CoreObjectID.ToString())
                             .SpriteCssClasses(icon)
                             .Text(@"<span class=""CoreObjectMenuTreeText"">" + Tools.Tools.UnSafeTitle(Item.FullName) + @"</span>" + Badge)
                             .Encoded(false);
                        }


                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}