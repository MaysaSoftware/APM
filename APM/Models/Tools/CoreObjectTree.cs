using APM.Models.APMObject;
using APM.Models.APMObject.InformationForm;
using APM.Models.Database;
using APM.Models.DesktopManagement;
using APM.Models.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace APM.Models.Tools
{
    public static class CoreObjectTree
    {



        public static void BaseCoreObjectTreeDropDownFiller(this Kendo.Mvc.UI.Fluent.DropDownTreeItemFactory _Base,long ParentID, CoreDefine.Entities Entitiy)
        {
            if (Entitiy == CoreDefine.Entities.خالی)
            {
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.پایگاه_داده)).Text(CoreDefine.Entities.پایگاه_داده.ToString().Replace("_", " ")).Items(subItems => subItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(ParentID, CoreDefine.Entities.پایگاه_داده), "", ""));
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فرم_ورود_اطلاعات)).Text(CoreDefine.Entities.فرم_ورود_اطلاعات.ToString().Replace("_", " ")).Items(subItems => subItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(ParentID, CoreDefine.Entities.فرم_ورود_اطلاعات), "", ""));
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.گزارش)).Text(CoreDefine.Entities.گزارش.ToString().Replace("_", " ")).Items(subItems => subItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(ParentID, CoreDefine.Entities.گزارش), "", ""));
            }
            else
            { 
                CoreObjectTreeDropDownFiller(_Base, CoreObject.FindChilds(ParentID, Entitiy), "", "");
            }
        }
         
        public static void BaseCoreObjectCheckTreeDropDownFiller(this Kendo.Mvc.UI.Fluent.DropDownTreeItemFactory _Base,long ParentID, CoreDefine.Entities Entitiy,string Value)
        {
            if (Entitiy == CoreDefine.Entities.خالی)
            {
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.پایگاه_داده)).Text(CoreDefine.Entities.پایگاه_داده.ToString().Replace("_", " ")).Items(subItems => subItems.CoreObjectCheckBoxTreeDropDownFiller(CoreObject.FindChilds(ParentID, CoreDefine.Entities.پایگاه_داده), Value, "", ""));
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فرم_ورود_اطلاعات)).Text(CoreDefine.Entities.فرم_ورود_اطلاعات.ToString().Replace("_", " ")).Items(subItems => subItems.CoreObjectCheckBoxTreeDropDownFiller(CoreObject.FindChilds(ParentID, CoreDefine.Entities.فرم_ورود_اطلاعات), Value, "", ""));
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.گزارش)).Text(CoreDefine.Entities.گزارش.ToString().Replace("_", " ")).Items(subItems => subItems.CoreObjectCheckBoxTreeDropDownFiller(CoreObject.FindChilds(ParentID, CoreDefine.Entities.گزارش), Value, "", ""));
            }
            else
            {
                CoreObjectCheckBoxTreeDropDownFiller(_Base, CoreObject.FindChilds(ParentID, Entitiy), Value, "", "");
            }
        }
        public static void CoreObjectCheckBoxTreeDropDownFiller(this Kendo.Mvc.UI.Fluent.DropDownTreeItemFactory _Base, List<CoreObject> _Items, string Value, string _Folder="", string Key="" )
        {
            List<Folder> Folders = new List<Folder>();
            string FolderPrefix = _Folder;
            string[] ValueArr = Value.Split(',');
            if (FolderPrefix != "") FolderPrefix += "/";   

            foreach (CoreObject Item in _Items)
                if (Item.Folder.StartsWith(FolderPrefix))
                {
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                        if (Folders.FindIndex(x => x.FullName == FolderParts[0]) == -1)
                            Folders.Add(new Folder() { FullName = FolderParts[0] });
                }

            if (_Items.Count > 0 && Folders.Count > 0)
            {
                List<CoreObject> FolderCoreList = CoreObject.FindChilds(_Items[0].ParentID, CoreDefine.Entities.پوشه);
                foreach (CoreObject Item in FolderCoreList)
                {
                    Folder folder = new Folder(Item);
                    folder.FullName = Tools.UnSafeTitle(Item.FullName);
                    folder.CoreObjectID = Item.CoreObjectID;
                    if (Folders.FindIndex(x => x.FullName == Tools.UnSafeTitle(Item.FullName)) == -1)
                    {
                        Folders.Add(folder);
                    }
                    else
                    {
                        int FindIndex = Folders.FindIndex(x => x.FullName == Tools.UnSafeTitle(Item.FullName));
                        Folders[FindIndex] = folder;
                    }
                }
            }

            foreach (Folder Folder in Folders)
                if(Folder.CoreObjectID>0)
                {
                    _Base.Add()
                        .Id( Folder.CoreObjectID.ToString())
                        .Checked(Array.IndexOf(ValueArr, Folder.CoreObjectID.ToString())>-1?true:false)
                        .Encoded(false)
                        .SpriteCssClasses(Folder.Icon)
                        .Text(Tools.UnSafeTitle(Folder.FullName))
                        .Items(items => items.CoreObjectCheckBoxTreeDropDownFiller(_Items, Value, FolderPrefix + Tools.UnSafeTitle(Folder.FullName), Key));
                }
                else
                { 
                    _Base.Add()
                        .Encoded(false)
                        .SpriteCssClasses("k-icon k-i-folder")
                        .Text(Tools.UnSafeTitle(Folder.FullName))
                        .Items(items => items.CoreObjectCheckBoxTreeDropDownFiller(_Items, Value, FolderPrefix + Tools.UnSafeTitle(Folder.FullName), Key));
                }


            foreach (CoreObject Item in _Items)
            {
                string icon = Icon.IconName(Item.Entity) + (Item.IsDefault ? " DefualtItem" : "");
                string TextExtension = "";
                string TextPrefix = ""; 

                if (Item.Entity == CoreDefine.Entities.فرآیند)
                {
                    ProcessType process = new ProcessType(Item);
                    if (!string.IsNullOrEmpty(process.Icon))
                        icon = process.Icon + (Item.IsDefault ? " DefualtItem" : "");
                }

                if (Item.Folder == _Folder)
                    _Base.Add()
                        .Id(Item.CoreObjectID.ToString())
                        .Checked(Array.IndexOf(ValueArr, Item.CoreObjectID.ToString()) > -1 ? true : false)
                        .Encoded(false)
                        .SpriteCssClasses(icon)
                        .Text(TextPrefix + Item.Title() + TextExtension) 
                        .Items(items =>
                        {  
                            if (Item.Entity == CoreDefine.Entities.پایگاه_داده)
                            {
                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.جدول))
                                .Text("جدول")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectCheckBoxTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.جدول), Value));

                            }

                            if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                .Text("فرم زیر مجموعه")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectCheckBoxTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_ورود_اطلاعات), Value)); 
                            } 

                            if (Item.Entity == CoreDefine.Entities.گزارش)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.پارامتر_گزارش))
                                .Text("پارامتر گزارش")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectCheckBoxTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.پارامتر_گزارش), Value));

                            }   

                        });
            }

        }
        public static void CoreObjectTreeDropDownFiller(this Kendo.Mvc.UI.Fluent.DropDownTreeItemFactory _Base, List<CoreObject> _Items, string _Folder = "", string Key = "")
        {
            List<Folder> Folders = new List<Folder>();
            string FolderPrefix = _Folder;
            if (FolderPrefix != "") FolderPrefix += "/";   

            foreach (CoreObject Item in _Items)
                if (Item.Folder.StartsWith(FolderPrefix))
                {
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                        if (Folders.FindIndex(x => x.FullName == FolderParts[0]) == -1)
                            Folders.Add(new Folder() { FullName = FolderParts[0] });
                }

            if (_Items.Count > 0 && Folders.Count > 0)
            {
                List<CoreObject> FolderCoreList = CoreObject.FindChilds(_Items[0].ParentID, CoreDefine.Entities.پوشه);
                foreach (CoreObject Item in FolderCoreList)
                {
                    Folder folder = new Folder(Item);
                    folder.FullName = Tools.UnSafeTitle(Item.FullName);
                    folder.CoreObjectID = Item.CoreObjectID;
                    if (Folders.FindIndex(x => x.FullName == Tools.UnSafeTitle(Item.FullName)) == -1)
                    {
                        Folders.Add(folder);
                    }
                    else
                    {
                        int FindIndex = Folders.FindIndex(x => x.FullName == Tools.UnSafeTitle(Item.FullName));
                        Folders[FindIndex] = folder;
                    }
                }
            }

            foreach (Folder Folder in Folders)
                if(Folder.CoreObjectID>0)
                {
                    _Base.Add()
                        .Id( Folder.CoreObjectID.ToString())
                        .Encoded(false)
                        .SpriteCssClasses(Folder.Icon)
                        .Text(Tools.UnSafeTitle(Folder.FullName))
                        .Items(items => items.CoreObjectTreeDropDownFiller(_Items, FolderPrefix + Tools.UnSafeTitle(Folder.FullName), Key));
                }
                else
                { 
                    _Base.Add()
                        .Encoded(false)
                        .SpriteCssClasses("k-icon k-i-folder")
                        .Text(Tools.UnSafeTitle(Folder.FullName))
                        .Items(items => items.CoreObjectTreeDropDownFiller(_Items, FolderPrefix + Tools.UnSafeTitle(Folder.FullName), Key));
                }


            foreach (CoreObject Item in _Items)
            {
                string icon = Icon.IconName(Item.Entity) + (Item.IsDefault ? " DefualtItem" : "");
                string TextExtension = "";
                string TextPrefix = ""; 

                if (Item.Entity == CoreDefine.Entities.فرآیند)
                {
                    ProcessType process = new ProcessType(Item);
                    if (!string.IsNullOrEmpty(process.Icon))
                        icon = process.Icon + (Item.IsDefault ? " DefualtItem" : "");
                }

                if (Item.Folder == _Folder)
                    _Base.Add()
                        .Id(Item.CoreObjectID.ToString()) 
                        .Encoded(false)
                        .SpriteCssClasses(icon)
                        .Text(TextPrefix + Item.Title() + TextExtension) 
                        .Items(items =>
                        {
                            if (Item.Entity == CoreDefine.Entities.جدول)
                            {
                                items.Add() 
                                .Encoded(false)
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد))
                                .Text("فیلد")
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.فیلد)));


                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد_نمایشی))
                                .Text("فیلد نمایشی")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.فیلد_نمایشی)));

                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد_محاسباتی))
                                .Text("فیلد محاسباتی")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.فیلد_محاسباتی)));

                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ضمیمه_جدول))
                                .Text("ضمیمه")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول)));
                            }

                            if (Item.Entity == CoreDefine.Entities.پایگاه_داده)
                            {
                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.جدول))
                                .Text("جدول")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.جدول)));

                            }

                            if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                .Text("فرم زیر مجموعه")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_ورود_اطلاعات)));


                                items.Add()
                                .Id("SearchForm")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فرم_جستجو))
                                .Text("فرم جستجو")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_جستجو)));

                                items.Add()
                                .Id("TableButton")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.دکمه_رویداد_جدول))
                                .Text("دکمه")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.دکمه_رویداد_جدول)));
                            }

                            if(Item.Entity==CoreDefine.Entities.فرم_جستجو)
                            {
                                items.Add()
                                .Id("SearchField")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد_جستجو))
                                .Text("فیلد جستجو")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد_جستجو)));

                            }

                            if (Item.Entity == CoreDefine.Entities.فرآیند)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.مرحله_فرآیند))
                                .Text("مرحله فرآیند")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند)));

                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.شاخص_فرآیند))
                                .Text("شاخص فرآیند")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.شاخص_فرآیند)));

                            }

                            if (Item.Entity == CoreDefine.Entities.گزارش)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.پارامتر_گزارش))
                                .Text("پارامتر گزارش")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.پارامتر_گزارش)));

                            }

                            if (Item.Entity == CoreDefine.Entities.مرحله_فرآیند)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ارجاع_مرحله_فرآیند))
                                .Text("ارجاع")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ارجاع_مرحله_فرآیند)));


                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.رویداد_مرحله_فرآیند))
                                .Text("رویداد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.رویداد_مرحله_فرآیند)));

                            }

                            if (Item.Entity == CoreDefine.Entities.داشبورد)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.زیر_بخش_داشبورد))
                                .Text("زیر بخش داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد)));


                                items.Add()
                                .Id("SearchField")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد_جستجو))
                                .Text("فیلد جستجو")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد_جستجو)));
                                 
                            }

                            if (Item.Entity == CoreDefine.Entities.زیر_بخش_داشبورد)
                            {
                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.زیر_بخش_داشبورد))
                                .Text("زیر بخش داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد)));


                                items.Add()
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ادغام_داشبورد))
                                .Text("ادغام داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeDropDownFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ادغام_داشبورد)));

                            }

                        });
            }

        }

        public static void QueryObjectSettingTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base)
        {
 
                _Base.Add().SpriteCssClasses("k-icon k-i-info-circle").Text("عبارت").Encoded(true).Items(subItems => subItems.FunctionQuerySettingTreeFiller());
                _Base.Add().SpriteCssClasses("k-icon k-i-info-circle").Text("نشانگر").Encoded(true).Items(subItems => subItems.OperatorQuerySettingTreeFiller());
                _Base.Add().SpriteCssClasses(Icon.IconName(CoreDefine.Entities.پایگاه_داده)).Text("پایگاه داده").Encoded(true).Items(subItems => subItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(0, CoreDefine.Entities.پایگاه_داده), ""));
        }

        public static void OperatorQuerySettingTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base)
        {
            List<SelectListItem> OperatorList = new List<SelectListItem>(){
                                                        new SelectListItem() {Text = "+", Value = "+"},
                                                        new SelectListItem() {Text = "-", Value = "-"},
                                                        new SelectListItem() {Text = "*", Value = "*"},
                                                        new SelectListItem() {Text = "/", Value = "/"},
                                                        new SelectListItem() {Text = "%", Value = "%"},
                                                        new SelectListItem() {Text = "&", Value = "&"},
                                                        new SelectListItem() {Text = "|", Value = "|"},
                                                        new SelectListItem() {Text = "^", Value = "^"},
                                                        new SelectListItem() {Text = "=", Value = "="},
                                                        new SelectListItem() {Text = ">", Value = ">"},
                                                        new SelectListItem() {Text = "<", Value = "<"},
                                                        new SelectListItem() {Text = ">=", Value = ">="},
                                                        new SelectListItem() {Text = "<=", Value = "<="},
                                                        new SelectListItem() {Text = "<>", Value = "<>"},
                                                        new SelectListItem() {Text = "+=", Value = "+="},
                                                        new SelectListItem() {Text = "-=", Value = "-="},
                                                        new SelectListItem() {Text = "*=", Value = "*="},
                                                        new SelectListItem() {Text = "/=", Value = "/="},
                                                        new SelectListItem() {Text = "%=", Value = "%="},
                                                        new SelectListItem() {Text = "&=", Value = "&="},
                                                        new SelectListItem() {Text = "^-=", Value = "^-="},
                                                        new SelectListItem() {Text = "|*=", Value = "|*="}
            };         
             
            foreach (SelectListItem Operator in OperatorList)
                _Base.Add()
                    .Encoded(false)
                    .HtmlAttributes(new { data_info = Operator.Value })
                    .Text(Operator.Text);

        }
        public static void FunctionQuerySettingTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base)
        { 
            List<SelectListItem> OperatorList = new List<SelectListItem>() {
                                                        new SelectListItem() {Text = "ویرایش", Value = "دستور.ویرایش"},
                                                        new SelectListItem() {Text = "درج", Value = "دستور.درج"},
                                                        new SelectListItem() {Text = "حذف", Value = "دستور.حذف"},
                                                        new SelectListItem() {Text = "نمایش", Value = "نمایش"},
                                                        new SelectListItem() {Text = "سطرها به تعداد", Value = "سطرها.به.تعداد"},
                                                        new SelectListItem() {Text = "تعداد کل", Value = "تعداد.کل"},
                                                        new SelectListItem() {Text = "در صورتی که", Value = "در.صورتی.که"},
                                                        new SelectListItem() {Text = "در غیر اینصورت", Value = "در.غیر.اینصورت"},
                                                        new SelectListItem() {Text = "شروع دستورات", Value = "شروع.دستورات"},
                                                        new SelectListItem() {Text = "پایان دستورات", Value = "پایان.دستورات"},
                                                        new SelectListItem() {Text = "ارتباط مستقیم", Value = "ارتباط.مستقیم.با"}, 
                                                        new SelectListItem() {Text = "ارتباط غیر مستقیم", Value = "ارتباط.غیر.مستقیم.با"}, 
                                                        new SelectListItem() {Text = "با توجه به", Value = "با.توجه.به"}, 
                                                        new SelectListItem() {Text = "جدول", Value = "نمایش <<نام_ستون>> \n از.جدول <<نام_جدول>> \nدر.صورتی.که <<>>"},
                                                        new SelectListItem() {Text = "شرط", Value = "انتخاب \nزمانیکه <<عبارت شرطی>> \nپس <<عبارت نمایشی>> \nدر.غیر.اینصورت <<عبارت نمایشی>> \nپایان.دستورات"},
                                                        new SelectListItem() {Text = "حداکثر", Value = "حداکثر(<<نام_ستون>>)"},
                                                        new SelectListItem() {Text = "حداقل", Value = "حداقل(<<نام_ستون>>)"},
                                                        new SelectListItem() {Text = "تعداد", Value = "تعداد(<<نام_ستون>>)"},
                                                        new SelectListItem() {Text = "مجموع", Value = "مجموع(<<نام_ستون>>)"},
                                                        new SelectListItem() {Text = "میانگین", Value = "میانگین(<<نام_ستون>>)"},
                                                        new SelectListItem() {Text = "رشته.انتخاب", Value = "رشته.انتخاب(<<نام_ستون>>،<<مقدار_شروع>>،<<مقدار_پایان>>)"},
                                                        new SelectListItem() {Text = "مقایسه.بین", Value = "<<نام_ستون>> مقایسه.بین <<عبارت_اول>> و <<عبارت_دوم>>  "},
                                                        new SelectListItem() {Text = "اگر.تهی.هست", Value = "اگر.تهی.هست(<<نام_ستون>> , <<عبارت>>)"},
                                                        new SelectListItem() {Text = "در", Value = "در"},
                                                        new SelectListItem() {Text = "مقداردهی", Value = "مقداردهی"},
                                                        new SelectListItem() {Text = "مقادیر", Value = "مقادیر"},
                                                        new SelectListItem() {Text = "از جدول", Value = "از.جدول"},
                                                        new SelectListItem() {Text = "انتخاب", Value = "انتخاب"},
                                                        new SelectListItem() {Text = "زمانیکه", Value = "زمانیکه()"},
                                                        new SelectListItem() {Text = "بعنوان", Value = "بعنوان"},
                                                        new SelectListItem() {Text = "هست", Value = "هست"},
                                                        new SelectListItem() {Text = "مخالف", Value = "مخالف"},
                                                        new SelectListItem() {Text = "تهی", Value = "تهی"},
                                                        new SelectListItem() {Text = "خروجی", Value = "دستور.خروجی"},
                                                        new SelectListItem() {Text = "اگر", Value = "اگر"},
                                                        new SelectListItem() {Text = "یا", Value = "یا"},
                                                        new SelectListItem() {Text = "و", Value = "و"},
                                                        new SelectListItem() {Text ="گروهبندی", Value = "دستور.گروهبندی"},
                                                        new SelectListItem() {Text ="حلقه", Value = "دستور.حلقه"},
                                                        new SelectListItem() {Text = "ترتیب", Value = "ترتیب"},
                                                        new SelectListItem() {Text = "صعودی", Value = "صعودی"},
                                                        new SelectListItem() {Text = "نزولی", Value = "نزولی"},
                                                        new SelectListItem() {Text = "نوع رشته", Value = "نوع.رشته"},
                                                        new SelectListItem() {Text = "نوع صحیح", Value = "نوع.صحیح"},
                                                        new SelectListItem() {Text = "شناسه پرسنل", Value = "@شناسه_پرسنل"},
                                                        new SelectListItem() {Text = "شناسه سمت مخاطب", Value = "@شناسه_سمت_مخاطب"}, 
                                                        new SelectListItem() {Text = "شناسه کاربر", Value = "@شناسه_کاربر"}, 
                                                        new SelectListItem() {Text = "شناسه نقش کاربر", Value = "@شناسه_نقش_کاربر"}, 
                                                        new SelectListItem() {Text = "شناسه واحد سازمانی پرسنل", Value = "@شناسه_واحد_سازمانی_پرسنل"}, 
                                                        new SelectListItem() {Text = "شناسه سمت سازمانی پرسنل", Value = "@شناسه_سمت_سازمانی_پرسنل"}, 
                                                        new SelectListItem() {Text = "نقش کاربر", Value = "@نقش_کاربری"}, 
                                                        new SelectListItem() {Text = "نام کاربر", Value = "@نام_کاربر"}, 
                                                        new SelectListItem() {Text = "نام و نام خانوادگی کاربر", Value = "@نام_و_نام_خانوادگی_کاربر"}, 
                                                        new SelectListItem() {Text = "تاریخ امروز", Value = "@تاریخ_امروز"}, 
                                                        new SelectListItem() {Text = "تاریخ شروع ماه جاری", Value = "@تاریخ_شروع_ماه_جاری"}, 
                                                        new SelectListItem() {Text = "تاریخ شروع سال جاری", Value = "@تاریخ_شروع_سال_جاری"}, 
                                                        new SelectListItem() {Text = "ساعت سیستم", Value = "@ساعت_سیستم"}, 
                                                        new SelectListItem() {Text = "تاریخ هفته قبل سیستم", Value = "@تاریخ_هفته_قبل_سیستم"}, 
                                                        new SelectListItem() {Text = "تاریخ ماه قبل سیستم", Value = "@تاریخ_ماه_قبل_سیستم"}, 
                                                        new SelectListItem() {Text = "تاریخ سال قبل سیستم", Value = "@تاریخ_سال_قبل_سیستم"}, 
                                                        new SelectListItem() {Text = "اختلاف دو تاریخ برحسب سال", Value = "دستور.اختلاف_زمان(دستور.سال،<<تاریخ مبداء>>،<<تاریخ مقصد<<)"},
                                                        new SelectListItem() {Text = "اختلاف دو تاریخ برحسب هفته", Value = "دستور.اختلاف_زمان(دستور.هفته،<<تاریخ مبداء>>،<<تاریخ مقصد<<)"},
                                                        new SelectListItem() {Text = "اختلاف دو تاریخ برحسب روز", Value = "دستور.اختلاف_زمان(دستور.روز،<<تاریخ مبداء>>،<<تاریخ مقصد<<)"},
                                                        new SelectListItem() {Text = "اختلاف دو تاریخ برحسب ساعت", Value = "دستور.اختلاف_زمان(دستور.ساعت،<<تاریخ مبداء+{ }+ ساعت مبداء>>،<<تاریخ مقصد +{ }+ ساعت مقصد>>)"},
                                                        new SelectListItem() {Text = "اختلاف دو تاریخ برحسب دقیقه", Value = "دستور.اختلاف_زمان(دستور.دقیقه،<<تاریخ مبداء+{ }+ ساعت مبداء>>،<<تاریخ مقصد+{ }+ ساعت مقصد>>)"},
                                                        new SelectListItem() {Text = "اختلاف دو تاریخ برحسب ثانیه", Value = "دستور.اختلاف_زمان(دستور.ثانیه،<<تاریخ مبداء+{ }+ ساعت مبداء>>،<<تاریخ مقصد+{ }+ ساعت مقصد>>)"},
                                                        new SelectListItem() {Text = "تبدیل دقیقه به ساعت", Value = "دستور.مبدل(nvarchar(8),دستور.افزودن_زمان(دستور.دقیقه, <<دقیقه>> , 0), 114)"},
                                                        new SelectListItem() {Text = "تبدیل ساعت به دقیقه", Value = "دستور.اختلاف_زمان(دستور.دقیقه،0،<<ساعت>>)"},
                                                        new SelectListItem() {Text = "جدول ضمیمه", Value = "جدول.ضمیمه"}, 
                                                        new SelectListItem() {Text = "جدول ضمیمه - مقدار", Value = "جدول.ضمیمه.مقدار"}, 
                                                        new SelectListItem() {Text = "جدول ضمیمه - نام", Value = "جدول.ضمیمه.نام"}, 
                                                        new SelectListItem() {Text = "جدول ضمیمه - پسوند", Value = "جدول.ضمیمه.پسوند"}, 
                                                        new SelectListItem() {Text = "جدول ضمیمه - اندازه", Value = "جدول.ضمیمه.اندازه"}, 
                                                        new SelectListItem() {Text = "جدول ضمیمه - رکورد داخلی", Value = "جدول.ضمیمه.رکورد_داخلی"}, 
                                                        new SelectListItem() {Text = "جدول ضمیمه - رکورد بیرونی", Value = "جدول.ضمیمه.رکورد_بیرونی"}, 
            }; 

            foreach (SelectListItem Operator in OperatorList)
                _Base.Add()
                    .Encoded(false)
                    .HtmlAttributes(new { data_info = Operator.Value })
                    .Text(Operator.Text);

        }    
        
        public static void CoreObjectQuerySettingTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, List<CoreObject> _Items, string _Folder = "",string Key="")
        {
            List<string> Folders = new List<string>();
            string FolderPrefix = _Folder;
            if (FolderPrefix != "") FolderPrefix += "/";

            foreach (CoreObject Item in _Items)
                if (Item.Folder.StartsWith(FolderPrefix))
                {
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                        if (Folders.IndexOf(FolderParts[0]) == -1)
                            Folders.Add(FolderParts[0]);
                }

            foreach (string Folder in Folders)
                _Base.Add() 
                    .Encoded(false) 
                    .SpriteCssClasses("k-icon k-i-folder")
                    .Text(Tools.UnSafeTitle(Folder))
                    .Items(items => items.CoreObjectQuerySettingTreeFiller(_Items, FolderPrefix + Folder, Key));

            foreach (CoreObject Item in _Items)
            {
                string icon = Icon.IconName(Item.Entity) + (Item.IsDefault ? " DefualtItem" : ""); 

                if (Item.Entity == CoreDefine.Entities.فرآیند)
                {
                    ProcessType process = new ProcessType(Item);
                    if (!string.IsNullOrEmpty(process.Icon))
                        icon = process.Icon + (Item.IsDefault ? " DefualtItem" : "");
                }
                string Data_Info = Item.FullName;
                string TextExtension = "";
                string TextPrefix = "<span></span>";
                switch (Item.Entity)
                {
                    case CoreDefine.Entities.فیلد_محاسباتی:
                        {
                            ComputationalField EntryForm = new ComputationalField(Item);
                            Data_Info = "\n"+EntryForm.Query;
                            break;
                        }

                    case CoreDefine.Entities.ضمیمه_جدول:
                        { 
                            Data_Info = "(نمایش سطرها.به.تعداد 1 Thumbnail \nاز.جدول CoreObjectAttachment \nدر.صورتی.که FullName=N'" + Data_Info + "' و RecordID ="+Item.ParentID+"  و InnerID=<<@نام_ستون>>) ";
                            break;
                        }
                    case CoreDefine.Entities.جدول:
                        {
                            Key = Key.Replace("PublicClass", "");
                            Table table = new Table(Item);
                            if (!string.IsNullOrEmpty(table.DisplayName))
                                TextExtension = "<span >(" + table.DisplayName + ")</span>";

                            break;
                        }
                    case CoreDefine.Entities.فیلد:
                        {
                            Field Field = new Field(Item);

                            if (Field.DisplayName != "")
                                TextExtension = "(" + Field.DisplayName + ")";

                            if (Field.FieldType == CoreDefine.InputTypes.RelatedTable && Field.RelatedTable > 0)
                            {
                                CoreObject FieldCore = CoreObject.Find(Field.RelatedTable);
                                if (Field.DisplayName != "")
                                    TextExtension = "<span class ='DataSourceInfo'>(" + Field.DisplayName + ")(" + Tools.UnSafeTitle(FieldCore.FullName) + ")</span>";
                                else
                                    TextExtension = "<span class ='DataSourceInfo'>(" + Tools.UnSafeTitle(FieldCore.FullName) + ")</span>";

                            } 

                            break;
                        }
                }

                if (Item.Folder == _Folder)
                    _Base.Add()
                        .Id(Item.CoreObjectID.ToString()) 
                        .Encoded(false)
                        .SpriteCssClasses(icon) 
                        .Text(TextPrefix + Item.Title() + TextExtension)
                        .HtmlAttributes(new { data_info = Data_Info })
                        .Items(items =>
                        {
                            if (Item.Entity == CoreDefine.Entities.جدول)
                            {
                                items.Add()
                                .Id("Field")
                                .Encoded(false)
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد))
                                .Text("فیلد")
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.فیلد), "", "Field"));
                                  
                                items.Add()
                                .Id("ComputationalField")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فیلد_محاسباتی))
                                .Text("فیلد محاسباتی")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.فیلد_محاسباتی), "", "ComputationalField")); 

                                items.Add()
                                .Id("TableAttachment")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ضمیمه_جدول))
                                .Text("ضمیمه")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds((int)Item.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول), "", "TableAttachment")); 
  
                            }

                            if (Item.Entity == CoreDefine.Entities.پایگاه_داده)
                            { 
                                items.Add()
                                .Id("Table")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.جدول))
                                .Text("جدول")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.جدول), "", "Table"));
 
                            }

                            if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                            { 
                                items.Add()
                                .Id("InformationEntryForm")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.فرم_ورود_اطلاعات))
                                .Text("فرم زیر مجموعه")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectSettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_ورود_اطلاعات), "", Key));
                                
                            }

                            if (Item.Entity == CoreDefine.Entities.فرآیند)
                            {                                 
                                items.Add()
                                .Id("ProcessStep")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.مرحله_فرآیند))
                                .Text( "مرحله فرآیند") 
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند), "", Key));                           
                                
                                items.Add()
                                .Id("ProcessIndicator")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.شاخص_فرآیند))
                                .Text("شاخص فرآیند") 
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.شاخص_فرآیند), "", Key));
 
                            }

                            if (Item.Entity == CoreDefine.Entities.گزارش)
                            {                                 
                                items.Add()
                                .Id("ReportParameter")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.پارامتر_گزارش))
                                .Text("پارامتر گزارش") 
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.پارامتر_گزارش), "", Key));
 
                            }

                            if (Item.Entity == CoreDefine.Entities.مرحله_فرآیند)
                            {                                 
                                items.Add()
                                .Id("ProcessReferral")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ارجاع_مرحله_فرآیند))
                                .Text("ارجاع") 
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ارجاع_مرحله_فرآیند), "", Key));

                              
                                items.Add()
                                .Id("ProcessStepEvent")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.رویداد_مرحله_فرآیند))
                                .Text("رویداد") 
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.رویداد_مرحله_فرآیند), "", Key));

                            }

                            if (Item.Entity == CoreDefine.Entities.داشبورد)
                            {
                                items.Add()
                                .Id("SubDashboard")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.زیر_بخش_داشبورد))
                                .Text("زیر بخش داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد), "", Key));
                            }

                            if (Item.Entity == CoreDefine.Entities.زیر_بخش_داشبورد)
                            {
                                items.Add()
                                .Id("DashboardIntegration")
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ادغام_داشبورد))
                                .Text("ادغام داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectQuerySettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ادغام_داشبورد), "", Key));

                            }

                        });
            }

        }

        public static void CoreObjectSettingTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, List<CoreObject> _Items, string _Folder = "", string Key = "", string Class = "PublicClass")
        {
            List<Folder> Folders = new List<Folder>();
            string FolderPrefix = _Folder;
            if (FolderPrefix != "") FolderPrefix += "/";

            foreach (CoreObject Item in _Items)
                if (Item.Folder.StartsWith(FolderPrefix))
                {
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                        if (Folders.FindIndex(x=>x.FullName== FolderParts[0]) == -1)
                            Folders.Add(new Folder() { FullName = FolderParts[0] });
                }

            if(Key!="" && _Items.Count>0)
            {
                List<CoreObject> children = new List<CoreObject>(Referral.CoreObjects.Where(item => item.ParentID == _Items[0].ParentID && item.Entity == CoreDefine.Entities.پوشه && item.FullName.Contains(Key+"_")&& item.Folder.StartsWith(FolderPrefix)));

                foreach (CoreObject Item in children)
                {
                    Folder folder = new Folder(Item);
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);
                    folder.FullName= FolderParts[0];
                    folder.CoreObjectID = Item.CoreObjectID;
                    if (Folders.FindIndex(x => x.FullName == folder.FullName) == -1)
                    {
                        Folders.Add(folder);
                    }
                    else
                    {
                        int FindIndex = Folders.FindIndex(x => x.FullName == folder.FullName);
                        Folders[FindIndex] = folder;
                    }
                }
            }
            foreach (Folder Folder in Folders)
                _Base.Add()
                    .Id(Key + "Folder" + Tools.SafeTitle(Folder.FullName)+"_"+Folder.CoreObjectID.ToString()) 
                    .Encoded(false)
                    .SpriteCssClasses(Folder.Icon)
                    .Text(Tools.UnSafeTitle(Folder.FullName))
                    .Items(items => items.CoreObjectSettingTreeFiller(_Items, FolderPrefix + Tools.UnSafeTitle(Folder.FullName), Key));

            foreach (CoreObject Item in _Items)
            { 
                string icon = Icon.IconName(Item.Entity) + (Item.IsDefault ? " DefualtItem" : "");
                string TextExtension = "";
                string TextPrefix = "<span></span>";
                Key = Key + " PublicClass";

                switch (Item.Entity)
                {
                    case CoreDefine.Entities.پایگاه_داده:
                        {
                            DataSourceInfo DataSourceInfo = new DataSourceInfo(Item);
                            switch (DataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        TextExtension = "<span class ='DataSourceInfo'>(" + DataSourceInfo.DataSourceType.ToString() + " - " + DataSourceInfo.DataBase + ")</span>";
                                        if (DataSourceInfo.CheckConnected())
                                            TextExtension += "<span class ='IsConnectedDB'></span>";
                                        else
                                            TextExtension += "<span class ='NotConnectedDB'></span>";
                                        break;
                                    }
                                case CoreDefine.DataSourceType.EXCEL:
                                    {
                                        TextExtension = "<span class ='DataSourceInfo'>(" + DataSourceInfo.DataSourceType.ToString() + " - " + DataSourceInfo.DataBase + ")</span>";
                                        DataSourceInfo.DataBase += Tools.GetExcelFormat(DataSourceInfo.DataBase);
                                        if (Attachment.CheckExistsFile(DataSourceInfo.FilePath + @"\" + DataSourceInfo.DataBase))
                                            TextExtension += "<span class ='IsConnectedDB'></span>";
                                        else
                                            TextExtension += "<span class ='NotConnectedDB'></span>";
                                        break;
                                    }
                                case CoreDefine.DataSourceType.ACCESS:
                                    {
                                        TextExtension = "<span class ='DataSourceInfo'>(" + DataSourceInfo.DataSourceType.ToString() + " - " + DataSourceInfo.DataBase + ")</span>";
                                        DataSourceInfo.DataBase += Tools.GetAccessFormat(DataSourceInfo.DataBase);
                                        if (Attachment.CheckExistsFile(DataSourceInfo.FilePath + @"\" + DataSourceInfo.DataBase))
                                            TextExtension += "<span class ='IsConnectedDB'></span>";
                                        else
                                            TextExtension += "<span class ='NotConnectedDB'></span>";
                                        break;
                                    }
                            }

                            Key = Key.Replace("PublicClass", "");
                            break;
                        }
                    case CoreDefine.Entities.جدول:
                        {
                            Key = Key.Replace("PublicClass", "");
                            Table table = new Table(Item);
                            if(!string.IsNullOrEmpty(table.DisplayName))
                                TextExtension = "<span >(" + table.DisplayName + ")</span>";

                            break;
                        }
                    case CoreDefine.Entities.فیلد:
                        {
                            Field Field = new Field(Item);

                            if (Field.DisplayName != "")
                                TextExtension = "(" + Field.DisplayName + ")";

                            if (Field.FieldType == CoreDefine.InputTypes.RelatedTable && Field.RelatedTable > 0)
                            {
                                CoreObject FieldCore = CoreObject.Find(Field.RelatedTable);
                                if (Field.DisplayName != "")
                                    TextExtension = "<span class ='DataSourceInfo'>("+ Field.DisplayName + ")(" + Tools.UnSafeTitle(FieldCore.FullName) + ")</span>";
                                else
                                    TextExtension = "<span class ='DataSourceInfo'>(" + Tools.UnSafeTitle(FieldCore.FullName) + ")</span>";

                            }

                            if (Field.IsIdentity)
                                TextPrefix = "<span class='fa fa-key PrimaryKey'></span>";

                            if (Field.IsRequired)
                                TextExtension += "*";
                            //if (Field.IsEditAble)
                            //    TextExtension = "<span class='k-icon k-i-pencil'></span>";

                            break;
                        }
                    case CoreDefine.Entities.مرحله_فرآیند:
                        {
                            ProcessStep ProcessStep = new ProcessStep(Item);
                            TextExtension = "<span class ='DataSourceInfo'>(" + Tools.UnSafeTitle(ProcessStep.Name) + ")</span>";
                            break;
                        }
                    case CoreDefine.Entities.خروجی_فرآیند:
                        {
                            BpmnSequenceFlow ProcessStep = new BpmnSequenceFlow(Item);
                            TextExtension = "<span class ='DataSourceInfo'>(" + Tools.UnSafeTitle(ProcessStep.Name) + ")</span>";
                            break;
                        }
                    case CoreDefine.Entities.ورودی_فرآیند:
                        {
                            BpmnSequenceFlow ProcessStep = new BpmnSequenceFlow(Item);
                            TextExtension = "<span class ='DataSourceInfo'>(" + Tools.UnSafeTitle(ProcessStep.Name) + ")</span>";
                            break;
                        }
                    case CoreDefine.Entities.رویداد_نمایش_فیلد:
                        {
                            ShowFieldEvent Field = new ShowFieldEvent(Item);
                            if (Field.ShowObject)
                                TextExtension += "<span class ='fa fa-eye'></span>";
                            else
                                TextExtension += "<span class ='fa fa-eye-slash'></span>";
                            break;
                        }
                    case CoreDefine.Entities.فرآیند:
                        {
                            ProcessType process = new ProcessType(Item);
                            if (!string.IsNullOrEmpty(process.Icon))
                                icon = process.Icon + (Item.IsDefault ? " DefualtItem" : "");
                            break; 
                        }
                    case CoreDefine.Entities.فرم_ورود_اطلاعات:
                        {
                            InformationEntryForm EntryForm = new InformationEntryForm(Item);
                            if (!string.IsNullOrEmpty(EntryForm.Icon))
                                icon = EntryForm.Icon + (Item.IsDefault ? " DefualtItem" : "");
                            break;
                        }
                    case CoreDefine.Entities.گزارش:
                        {
                            Report Report = new Report(Item);
                            if (!string.IsNullOrEmpty(Report.Icon))
                                icon = Report.Icon;
                            break;
                        }
                    case CoreDefine.Entities.فرم_دکمه_جدید:
                        {
                            NewButtonForm NewButtonForm = new NewButtonForm(Item);
                            if (!string.IsNullOrEmpty(NewButtonForm.Icon))
                                icon = NewButtonForm.Icon;
                            break;
                        }
                    case CoreDefine.Entities.فیلد_نمایشی:
                        {
                            DisplayField NewButtonForm = new DisplayField(Item);
                            if (!string.IsNullOrEmpty(NewButtonForm.Icon))
                                icon = NewButtonForm.Icon;
                            break;
                        }
                    case CoreDefine.Entities.دکمه_رویداد_جدول:
                        {
                            TableButton NewButtonForm = new TableButton(Item);
                            if (!string.IsNullOrEmpty(NewButtonForm.Icon))
                                icon = NewButtonForm.Icon;
                            break;
                        }
                    default: break;
                }


                if (Item.Folder == _Folder)
                    _Base.Add()
                        .Id(Item.CoreObjectID.ToString())
                        .HtmlAttributes(new { @class = Key })
                        .Encoded(false)
                        .SpriteCssClasses(icon)
                        .Text(TextPrefix + Item.Title() + TextExtension)
                        .Items(items =>
                        {
                            foreach (SubEntities SubEntitiesItem in GetSubEntitiesFromObject(Item.Entity))
                            {
                                items.Add()
                                .Id(SubEntitiesItem.Name)
                                .SpriteCssClasses(Icon.IconName(SubEntitiesItem.Entities))
                                .HtmlAttributes(new { @class = SubEntitiesItem.Class })
                                .Text(SubEntitiesItem.Text)
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectSettingTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, SubEntitiesItem.Entities), "", SubEntitiesItem.Class));
                            }     
                        });
            }

        }

 
        public static List<SubEntities> GetSubEntitiesFromObject(CoreDefine.Entities entities)
        {
            List<SubEntities> SubEntitiesList = new List<SubEntities>();
            switch(entities)
            {
                case CoreDefine.Entities.پایگاه_داده:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.جدول, Name = "Table", Text = "جدول", Class = "Table" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.تابع_جدولی, Name = "TableFunction", Text = "تابع", Class = "TableFunction" });
                        break;
                    }
                case CoreDefine.Entities.جدول:
                    {
                        SubEntitiesList.Add(new SubEntities(){ Entities= CoreDefine.Entities.فیلد, Name= "Field" ,Text="فیلد",Class= "Field" });
                        SubEntitiesList.Add(new SubEntities() { Entities= CoreDefine.Entities.فیلد_نمایشی, Name= "DisplayField", Text = "فیلد نمایشی",Class= "DisplayField" });
                        SubEntitiesList.Add(new SubEntities() { Entities= CoreDefine.Entities.فیلد_محاسباتی, Name= "ComputationalField", Text = "فیلد محاسباتی",Class= "ComputationalField" });
                        SubEntitiesList.Add(new SubEntities() { Entities= CoreDefine.Entities.ضمیمه_جدول, Name= "TableAttachment", Text = "ضمیمه",Class= "TableAttachment" });
                        SubEntitiesList.Add(new SubEntities() { Entities= CoreDefine.Entities.رویداد_جدول, Name= "TableEvent", Text = "رویداد",Class= "TableEvent" });
                        break;
                    }
                case CoreDefine.Entities.فیلد:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.رویداد_نمایش_فیلد, Name = "ShowFieldEvent", Text = "رویداد نمایش فیلد",Class= "ShowFieldEvent PublicClass" });
                        break;
                    }
                case CoreDefine.Entities.وب_سرویس:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.پارامتر_وب_سرویس, Name = "WebServiceParameter", Text = "پارامتر وب سرویس",Class= "WebServiceParameter" });
                        break;
                    }
                case CoreDefine.Entities.تابع_جدولی:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.پارامتر_تابع, Name = "ParameterTableFunction", Text = "پارامتر تابع",Class= "ParameterTableFunction" });
                        break;
                    }
                case CoreDefine.Entities.فرم_ورود_اطلاعات:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فرم_ورود_اطلاعات, Name = "InformationEntryForm", Text = "فرم زیر مجموعه",Class= "InformationEntryForm" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فرم_دکمه_جدید, Name = "NewButtonForm", Text = "فرم دکمه جدید", Class = "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فیلد, Name = "Field", Text = "فیلد", Class = "Field" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فیلد_نمایشی, Name = "DisplayField", Text = "ستون نمایشی",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فرم_جستجو, Name = "SearchForm", Text = "فرم جستجو",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.دکمه_رویداد_جدول, Name = "TableButton", Text = "دکمه",Class= "TableButton" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.رویداد_جدول , Name = "TableEvent", Text = "رویداد",Class= "TableEvent" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فیلد_گروهبندی, Name = "GroupField", Text = "فیلد گروهبندی", Class= "GroupField" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.رنگ_سطر_جدول, Name = "GridRowColor", Text = "رنگ سطر جدول", Class= "GridRowColor" });
                        break;
                    }
                case CoreDefine.Entities.فرم_جستجو:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فیلد_جستجو, Name = "SearchField", Text = "فیلد جستجو",Class= "SearchField" });
                        break;
                    }
                case CoreDefine.Entities.فرآیند:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.BPMN_بخش_بندی, Name = "BpmnParticipant", Text = "بخش بندی فرآیند",Class= "" });
                        break;
                    } 
                case CoreDefine.Entities.BPMN_بخش_بندی:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.BPMN_مسیر, Name = "BpmnLane", Text = "مسیر فرآیند",Class= "" });
                        break;
                    } 
                case CoreDefine.Entities.BPMN_مسیر:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.مرحله_فرآیند, Name = "ProcessStep", Text = "مرحله فرآیند",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.ارجاع_مرحله_فرآیند, Name = "ProcessReferral", Text = "ارجاع فرآیند", Class= "" });
                        break;
                    }  
                case CoreDefine.Entities.مرحله_فرآیند:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.ورودی_فرآیند, Name = "BpmnIncoming", Text = "ورودی فرآیند",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.خروجی_فرآیند, Name = "BpmnOutgoin", Text = "خروجی فرآیند",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.رویداد_مرحله_فرآیند, Name = "ProcessStepEvent", Text = "رویداد فرآیند", Class = "" });
                        break;
                    }  
                case CoreDefine.Entities.گزارش:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.پارامتر_گزارش, Name = "ReportParameter", Text = "پارامتر گزارش",Class= "" });
                        break;
                    }   
                case CoreDefine.Entities.داشبورد:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.زیر_بخش_داشبورد, Name = "SubDashboard", Text = "زیر بخش داشبورد",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.فیلد_جستجو, Name = "SearchField", Text = "فیلد جستجو", Class = "SearchField" });
                        break;
                    }   
                case CoreDefine.Entities.زیر_بخش_داشبورد:
                    {
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.زیر_بخش_داشبورد, Name = "SubDashboard", Text = "زیر بخش داشبورد",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.ادغام_داشبورد, Name = "DashboardIntegration", Text = "ادغام داشبورد",Class= "" });
                        SubEntitiesList.Add(new SubEntities() { Entities = CoreDefine.Entities.رنگ_سطر_جدول, Name = "GridRowColor", Text = "شاخص", Class = "GridRowColor" });
                        break;
                    }      
            } 
            return SubEntitiesList;

        }
        public static void CoreObjectTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, List<CoreObject> _Items, string _Folder = "", string _RoleType = "",string Key="")
        {
            List<string> Folders = new List<string>();
            string FolderPrefix = _Folder;
            if (FolderPrefix != "") FolderPrefix += "/";

            foreach (CoreObject Item in _Items)
                if (Item.Folder.StartsWith(FolderPrefix))
                {
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                        if (Folders.IndexOf(FolderParts[0]) == -1)
                            Folders.Add(FolderParts[0]);
                }

            foreach (string Folder in Folders)
                _Base.Add()
                    .Id(Key+ Folder)
                    .Encoded(false)
                    .Text(@"<span class=""k-icon k-i-folder TreeIconColor""></span> <span class=""CoreObjectMenuTreeText"">" + Tools.UnSafeTitle(Folder) + @"</span>")
                    .Items(items => items.CoreObjectTreeFiller(_Items, FolderPrefix + Folder, _RoleType, Key));

            foreach (CoreObject Item in _Items)
            { 
                string icon = Icon.IconName(Item.Entity) + (Item.IsDefault ? " DefualtItem" : "");
                string TextExtension = "";
                string TextPrefix = "<span></span>";

                if (Item.Entity == CoreDefine.Entities.فرآیند)
                {
                    ProcessType process = new ProcessType(Item);
                    if (!string.IsNullOrEmpty(process.Icon))
                        icon = @"<span class=""" + process.Icon + @" TreeIconColor""></span>";
                }

                switch (Item.Entity)
                {
                    case CoreDefine.Entities.پایگاه_داده:
                        {
                            DataSourceInfo DataSourceInfo = new DataSourceInfo(Item);
                            switch (DataSourceInfo.DataSourceType)
                            {
                                case CoreDefine.DataSourceType.SQLSERVER:
                                    {
                                        TextExtension = "<span class ='DataSourceInfo'>(" + DataSourceInfo.DataSourceType.ToString() + " - " + DataSourceInfo.DataBase + ")</span>";
                                        break;
                                    }
                                case CoreDefine.DataSourceType.EXCEL:
                                    {
                                        TextExtension = "<span class ='DataSourceInfo'>(" + DataSourceInfo.DataSourceType.ToString() + " - " + DataSourceInfo.DataBase + ")</span>";
                                        break;
                                    }
                                case CoreDefine.DataSourceType.ACCESS:
                                    {
                                        TextExtension = "<span class ='DataSourceInfo'>(" + DataSourceInfo.DataSourceType.ToString() + " - " + DataSourceInfo.DataBase + ")</span>";
                                        break;
                                    }
                            }
                            break;
                        }
                    case CoreDefine.Entities.فیلد:
                        {
                            Field Field = new Field(Item);
                            if (Field.FieldType == CoreDefine.InputTypes.RelatedTable && Field.RelatedTable > 0)
                            {
                                CoreObject FieldCore = CoreObject.Find(Field.RelatedTable);
                                TextExtension = "<span class ='DataSourceInfo'>(" + Tools.UnSafeTitle(FieldCore.FullName) + ")</span>";
                            }

                            if (Field.IsIdentity)
                                TextPrefix = "<span class='fa fa-key PrimaryKey'></span>";
                            break;
                        }
                    default: break;
                }



                if (Item.Folder == _Folder)
                    _Base.Add()
                        .Checked(!string.IsNullOrEmpty(_RoleType) ? Item.Permission(_RoleType).IsAllow : false)
                        .Id(Item.CoreObjectID.ToString())
                        .Encoded(false)
                        .SpriteCssClasses(icon)
                        .Text(TextPrefix + Item.Title() + TextExtension) 
                        .Items(items =>
                        {

                            if (Item.Entity == CoreDefine.Entities.پایگاه_داده)
                            {
                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.جدول))
                                .Text("جدول")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.جدول), "", _RoleType, Key));

                            }

                            if (Item.Entity == CoreDefine.Entities.جدول)
                            {
                                PermissionTable TableItem = new PermissionTable(Item.CoreObjectID, _RoleType);

                                items.Add()
                                .Encoded(false)
                                .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "فیلد")
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد), "", _RoleType, Key));

                                items.Add()
                                .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "فیلد نمایشی")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد_نمایشی), "", _RoleType, Key));

                                items.Add()
                                .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "فیلد محاسباتی")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد_محاسباتی), "", _RoleType, Key));

                                items.Add()
                                .Encoded(false)
                                .Text(@"<span class=""k-icon k-i-attachment TreeIconColor""></span>" + "ضمیمه جدول")
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول), "", _RoleType, Key));

                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.TableSpecialItemsFiller(TableItem));
                            }

                            if (Item.Entity == CoreDefine.Entities.ضمیمه_جدول)
                            {
                                PermissionTableAttachment FieldItem = new PermissionTableAttachment(Item.CoreObjectID, _RoleType);

                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.FieldSpecialItemsFiller(FieldItem));
                            }

                            if (Item.Entity == CoreDefine.Entities.فیلد_محاسباتی)
                            {
                                PermissionComputationalField FieldItem = new PermissionComputationalField(Item.CoreObjectID, _RoleType);

                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.FieldSpecialItemsFiller(FieldItem));
                            }

                            if (Item.Entity == CoreDefine.Entities.فیلد)
                            { 
                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.FieldSpecialItemsFiller(new PermissionField(Item.CoreObjectID, _RoleType)));
                            }

                            if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                            {
                                PermissionInformationEntryForm InformationEntryFormItem = new PermissionInformationEntryForm(Item.CoreObjectID, _RoleType);
                                List<CoreObject> SubTableForms = CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_ورود_اطلاعات);
                                List<CoreObject> ButtonTableForms = CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.دکمه_رویداد_جدول);
                                List<CoreObject> ButtonNewForms = CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_دکمه_جدید);

                                if (SubTableForms.Count > 0)
                                {
                                    items.Add()
                                    .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "فرم زیر مجموعه")
                                    .Encoded(false)
                                    .Items(FieldItems => FieldItems.CoreObjectTreeFiller(SubTableForms, "", _RoleType, Key));
                                }
                                
                                if (ButtonTableForms.Count > 0)
                                {
                                    items.Add()
                                    .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "دکمه")
                                    .Encoded(false)
                                    .Items(FieldItems => FieldItems.CoreObjectTreeFiller(ButtonTableForms, "", _RoleType, Key));
                                }
                                
                                if (ButtonNewForms.Count > 0)
                                {
                                    items.Add()
                                    .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "دکمه جدید")
                                    .Encoded(false)
                                    .Items(FieldItems => FieldItems.CoreObjectTreeFiller(ButtonNewForms, "", _RoleType, Key));
                                }

                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.InformationEntryFormSpecialItemsFiller(InformationEntryFormItem));
                            }

                            if (Item.Entity == CoreDefine.Entities.دکمه_رویداد_جدول || Item.Entity== CoreDefine.Entities.فرم_دکمه_جدید) 
                            {  
                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.TableButtonSpecialItemsFiller(new PermissionTableButton(Item.CoreObjectID, _RoleType)));
                            }
                            if (Item.Entity == CoreDefine.Entities.فرآیند)
                            { 
                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.ProcessesSpecialItemsFiller(new PermissionProcesses(Item.CoreObjectID, _RoleType)));
                            }

                            if (Item.Entity == CoreDefine.Entities.داشبورد)
                            { 
                                items.Add()
                                .Text(@"<span class=""k-icon k-i-subreport TreeIconColor""></span>" + "زیر بخش داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد), "", _RoleType, Key));
                            }
                            
                            if (Item.Entity == CoreDefine.Entities.زیر_بخش_داشبورد)
                             { 
                                items.Add() 
                                .Text(@"<span class=""k-icon k-i-subreport TreeIconColor""></span>" + "زیر بخش داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد), "", _RoleType, Key));

                                items.Add() 
                                .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ادغام_داشبورد))
                                .Text("ادغام داشبورد")
                                .Encoded(false)
                                .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ادغام_داشبورد), "", _RoleType, Key));


                                items.Add()
                                .Text("مجوز")
                                .Items(SpecialItems => SpecialItems.DashboardSpecialItemsFiller(new PermissionDashboard(Item.CoreObjectID, _RoleType)));
                            }

                        });
            }

        }

        public static void TableSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionTable _TablePermission)
        {
            _Base.Add()
            .Checked(_TablePermission.CanInsert) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-table-row-insert-above TreeIconColor""></span>" + "درج")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanInsert");

            _Base.Add()
            .Checked(_TablePermission.CanUpdate) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-table-properties TreeIconColor""></span>" + "ویرایش")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanUpdate");

            _Base.Add()
            .Checked(_TablePermission.CanDelete) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-table-row-delete TreeIconColor""></span>" + "حذف")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanDelete");

            _Base.Add()
            .Checked(_TablePermission.CanOpenAttachment) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده ضمائم")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanOpenAttachment");


            _Base.Add()
            .Checked(_TablePermission.CanInsertAttachment) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-plus TreeIconColor""></span>" + "درج ضمائم")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanInsertAttachment");

            _Base.Add()
            .Checked(_TablePermission.CanUpdateAttachment) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش ضمائم")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanUpdateAttachment");

            _Base.Add()
            .Checked(_TablePermission.CanDeleteAttachment) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-x TreeIconColor""></span>" + "حذف ضمائم")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanDeleteAttachment");

            _Base.Add()
            .Checked(_TablePermission.CanDeleteAttachment) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-download TreeIconColor""></span>" + "بارگیری ضمائم")
            .Id(_TablePermission.CoreObjectID.ToString() + "_CanDownloadAttachment"); 

            _Base.Add()
            .Checked(_TablePermission.IsUpdateLimitByInserterUser)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-user TreeIconColor""></span>" + "ویرایش محدود به کاربر درج کننده")
            .Id(_TablePermission.CoreObjectID.ToString() + "_IsUpdateLimitByInserterUser");
        }

        public static void FieldSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionTableAttachment _FieldPermission)
        {

            _Base.Add()
            .Checked(_FieldPermission.CanOpenAttachment) 
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده")
            .Encoded(false)
            .Id(_FieldPermission.CoreObjectID.ToString() + "_CanOpenAttachment");

            _Base.Add()
            .Checked(_FieldPermission.CanUpload) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-upload TreeIconColor""></span>" + "بارگذاری")
            .Id(_FieldPermission.CoreObjectID.ToString() + "_CanUpload"); 

            _Base.Add()
            .Checked(_FieldPermission.CanDownload) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-download TreeIconColor""></span>" + "بارگیری")
            .Id(_FieldPermission.CoreObjectID.ToString() + "_CanDownload"); 
             
        }
        
        public static void FieldSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionComputationalField _FieldPermission)
        {

            _Base.Add()
            .Checked(_FieldPermission.CanView) 
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده")
            .Encoded(false)
            .Id(_FieldPermission.CoreObjectID.ToString() + "_CanView");              
        }
        
        public static void FieldSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionField _FieldPermission)
        {

            _Base.Add()
            .Checked(_FieldPermission.CanView) 
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده")
            .Encoded(false)
            .Id(_FieldPermission.CoreObjectID.ToString() + "_CanView");

            _Base.Add()
            .Checked(_FieldPermission.CanUpdate) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش")
            .Id(_FieldPermission.CoreObjectID.ToString() + "_CanUpdate"); 
             
        }

        public static void InformationEntryFormSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionInformationEntryForm _InformationEntryFormPermission)
        {
            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanInsert)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-plus TreeIconColor""></span>" + "درج")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanInsert");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanUpdate)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanUpdate");

            if(Referral.AdminSetting.ShowEditingRestrictions)
            {
                if(Referral.AdminSetting.ShowCanUpdateOnlyUserRegistry)
                    _Base.Add()
                    .Checked(_InformationEntryFormPermission.CanUpdateOnlyUserRegistry)
                    .Encoded(false)
                    .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش محدود به کاربر ثبت کننده")
                    .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanUpdateOnlyUserRegistry");

                if (Referral.AdminSetting.ShowCanUpdateOneDey)
                    _Base.Add()
                    .Checked(_InformationEntryFormPermission.CanUpdateOneDey)
                    .Encoded(false)
                    .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش به مدت یک روز")
                    .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanUpdateOneDey");

                if (Referral.AdminSetting.ShowCanUpdateThreeDey)
                    _Base.Add()
                    .Checked(_InformationEntryFormPermission.CanUpdateThreeDey)
                    .Encoded(false)
                    .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش به مدت سه روز")
                    .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanUpdateThreeDey");

                if (Referral.AdminSetting.ShowCanUpdateOneWeek)
                    _Base.Add()
                    .Checked(_InformationEntryFormPermission.CanUpdateOneWeek)
                    .Encoded(false)
                    .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش به مدت یک هفته")
                    .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanUpdateOneWeek"); 
            }


            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanOpenAttachment)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده ضمائم")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanOpenAttachment");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanDelete)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-x TreeIconColor""></span>" + "حذف")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanDelete");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowCountRow)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "نمایش تعداد سطرها")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowCountRow");
            

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanView)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "نمایش")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanView");
            

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowAutoFit)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-columns TreeIconColor""></span>" + "نمایش تنظیم ستون")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowAutoFit");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowComputedFieldInEditForm) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-custom-format TreeIconColor""></span>" + "نمایش فیلدهای محاسباتی هنگام ورود اطلاعات")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowComputedFieldInEditForm");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanExportReport) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-graph TreeIconColor""></span>" + "گزارش")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanExportReport");

            if (Referral.AdminSetting.PermissionShowImportExportInForm)
                _Base.Add()
                .Checked(_InformationEntryFormPermission.CanImportData) 
                .Encoded(false)
                .Text(@"<span class=""k-icon k-i-graph TreeIconColor""></span>" + "ایمپورت اطلاعات")
                .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanImportData");

            if (Referral.AdminSetting.PermissionShowEmailInForm)
                _Base.Add()
                .Checked(_InformationEntryFormPermission.CanSendEmail) 
                .Encoded(false)
                .Text(@"<span class=""k-icon k-i-graph TreeIconColor""></span>" + "ارسال ایمیل")
                .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanSendEmail");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventEditRecord) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-pencil TreeIconColor""></span>" + "ویرایش سطر")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventEditRecord");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventEditAll) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-table-properties TreeIconColor""></span>" + "ویرایش کل")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventEditAll");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventInsertRecord) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-plus TreeIconColor""></span>" + "درج سطر")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventInsertRecord");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventInsertAll) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-table-add TreeIconColor""></span>" + "درج کل")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventInsertAll");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventDeleteAll) 
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-replace-single TreeIconColor""></span>" + "بازیابی سطر حذف شده")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventDeleteAll");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventDownloadRecord) 
            .Encoded(false)
            .Text(@"<span class=""fa fa-down-to-line TreeIconColor""></span>" + "بارگیری سطر")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventDownloadRecord");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventDownloadTable) 
            .Encoded(false)
            .Text(@"<span class=""fa fa-download TreeIconColor""></span>" + "کل بارگیری")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventDownloadTable");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventViewRecord) 
            .Encoded(false)
            .Text(@"<span class=""fa fa-eye TreeIconColor""></span>" + "مشاهده سطر")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventViewRecord");

            _Base.Add()
            .Checked(_InformationEntryFormPermission.CanShowEventViewTable) 
            .Encoded(false)
            .Text(@"<span class=""fa fa-eye TreeIconColor""></span>" + "کل مشاهده")
            .Id(_InformationEntryFormPermission.CoreObjectID.ToString() + "_CanShowEventViewTable");
        }

        public static void ProcessesSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionProcesses _ProcessesPermission)
        {
            _Base.Add()
            .Checked(_ProcessesPermission.CanShow)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده")
            .Id(_ProcessesPermission.CoreObjectID.ToString() + "_CanShow"); 
             
        }
        public static void TableButtonSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionTableButton _PermissionTableButton)
        {
            _Base.Add()
            .Checked(_PermissionTableButton.CanShow)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده")
            .Id(_PermissionTableButton.CoreObjectID.ToString() + "_CanShow");  
        }
        public static void DashboardSpecialItemsFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, PermissionDashboard _DashboardPermission)
        {
            _Base.Add()
            .Checked(_DashboardPermission.CanShow)
            .Encoded(false)
            .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "مشاهده")
            .Id(_DashboardPermission.CoreObjectID.ToString() + "_CanShow"); 
             
        }


        public static void CoreObjectTreeFiller(this Kendo.Mvc.UI.Fluent.TreeViewItemFactory _Base, List<CoreObject> _Items, string _Folder = "")
        {
            List<string> Folders = new List<string>();
            string FolderPrefix = _Folder;
            if (FolderPrefix != "") FolderPrefix += "/";

            foreach (CoreObject Item in _Items)
                if (Item.Folder.StartsWith(FolderPrefix))
                {
                    string[] FolderParts = Item.Folder.Remove(0, FolderPrefix.Length).Split(new[] { "/" }, StringSplitOptions.None);

                    if (FolderParts.Length > 0 && FolderParts[0] != "")
                        if (Folders.IndexOf(FolderParts[0]) == -1)
                            Folders.Add(FolderParts[0]);
                }

            foreach (string Folder in Folders)
                _Base.Add() 
                    .Encoded(false)
                    .Text(@"<span class=""k-icon k-i-folder TreeIconColor""></span> <span class=""CoreObjectMenuTreeText"">" + Tools.UnSafeTitle(Folder) + @"</span>")
                    .Items(items => items.CoreObjectTreeFiller(_Items, FolderPrefix + Folder));

            foreach (CoreObject Item in _Items)
            {
                string icon = Icon.IconName(Item.Entity) + (Item.IsDefault ? " DefualtItem" : "");
                if (Item.Folder == _Folder)
                    _Base.Add()
                        .Id("CoreObjectTree_" + Item.CoreObjectID.ToString())
                        .Encoded(false)
                        .SpriteCssClasses(icon)
                        .Text(Item.Title());
                        //.Items(items =>
                        //{

                        //    if (Item.Entity == CoreDefine.Entities.پایگاه_داده)
                        //    {
                        //        items.Add()
                        //        .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.جدول))
                        //        .Text("جدول")
                        //        .Encoded(false)
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.جدول), "", _RoleType, Key));

                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.جدول)
                        //    {
                        //        PermissionTable TableItem = new PermissionTable(Item.CoreObjectID, _RoleType);

                        //        items.Add()
                        //        .Encoded(false)
                        //        .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "فیلد")
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد), "", _RoleType, Key));

                        //        items.Add()
                        //        .Text(@"<span class=""k-icon k-i-eye TreeIconColor""></span>" + "فیلد نمایشی")
                        //        .Encoded(false)
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد_نمایشی), "", _RoleType, Key));

                        //        items.Add()
                        //        .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "فیلد محاسباتی")
                        //        .Encoded(false)
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فیلد_محاسباتی), "", _RoleType, Key));

                        //        items.Add()
                        //        .Encoded(false)
                        //        .Text(@"<span class=""k-icon k-i-attachment TreeIconColor""></span>" + "ضمیمه جدول")
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ضمیمه_جدول), "", _RoleType, Key));

                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.TableSpecialItemsFiller(TableItem));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.ضمیمه_جدول)
                        //    {
                        //        PermissionTableAttachment FieldItem = new PermissionTableAttachment(Item.CoreObjectID, _RoleType);

                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.FieldSpecialItemsFiller(FieldItem));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.فیلد_محاسباتی)
                        //    {
                        //        PermissionComputationalField FieldItem = new PermissionComputationalField(Item.CoreObjectID, _RoleType);

                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.FieldSpecialItemsFiller(FieldItem));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.فیلد)
                        //    {
                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.FieldSpecialItemsFiller(new PermissionField(Item.CoreObjectID, _RoleType)));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.فرم_ورود_اطلاعات)
                        //    {
                        //        PermissionInformationEntryForm InformationEntryFormItem = new PermissionInformationEntryForm(Item.CoreObjectID, _RoleType);
                        //        List<CoreObject> SubTableForms = CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_ورود_اطلاعات);

                        //        if (SubTableForms.Count > 0)
                        //        {
                        //            items.Add()
                        //            .Text(@"<span class=""k-icon k-i-table-column-insert-right TreeIconColor""></span>" + "فرم زیر مجموعه")
                        //            .Encoded(false)
                        //            .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.فرم_ورود_اطلاعات), "", _RoleType, Key));
                        //        }

                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.InformationEntryFormSpecialItemsFiller(InformationEntryFormItem));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.فرآیند)
                        //    {
                        //        PermissionProcesses ProcessesItem = new PermissionProcesses(Item.CoreObjectID, _RoleType);
                        //        List<CoreObject> SubProcessStep = CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند);

                        //        if (SubProcessStep.Count > 0)
                        //        {
                        //            items.Add()
                        //            .Text(@"<span class=""k-icon k-i-connector TreeIconColor""></span>" + "مراحل فرآیند")
                        //            .Encoded(false)
                        //            .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.مرحله_فرآیند), "", _RoleType, Key));
                        //        }

                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.ProcessesSpecialItemsFiller(ProcessesItem));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.داشبورد)
                        //    {
                        //        items.Add()
                        //        .Text(@"<span class=""k-icon k-i-subreport TreeIconColor""></span>" + "زیر بخش داشبورد")
                        //        .Encoded(false)
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد), "", _RoleType, Key));
                        //    }

                        //    if (Item.Entity == CoreDefine.Entities.زیر_بخش_داشبورد)
                        //    {
                        //        items.Add()
                        //        .Text(@"<span class=""k-icon k-i-subreport TreeIconColor""></span>" + "زیر بخش داشبورد")
                        //        .Encoded(false)
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.زیر_بخش_داشبورد), "", _RoleType, Key));

                        //        items.Add()
                        //        .SpriteCssClasses(Icon.IconName(CoreDefine.Entities.ادغام_داشبورد))
                        //        .Text("ادغام داشبورد")
                        //        .Encoded(false)
                        //        .Items(FieldItems => FieldItems.CoreObjectTreeFiller(CoreObject.FindChilds(Item.CoreObjectID, CoreDefine.Entities.ادغام_داشبورد), "", _RoleType, Key));


                        //        items.Add()
                        //        .Text("مجوز")
                        //        .Items(SpecialItems => SpecialItems.DashboardSpecialItemsFiller(new PermissionDashboard(Item.CoreObjectID, _RoleType)));
                        //    }

                        //});
            }

        }

    }
}