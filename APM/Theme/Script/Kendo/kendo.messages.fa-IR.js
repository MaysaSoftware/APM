(function ($, undefined) {
/* FlatColorPicker messages */

if (kendo.ui.FlatColorPicker) {
kendo.ui.FlatColorPicker.prototype.options.messages =
$.extend(true, kendo.ui.FlatColorPicker.prototype.options.messages,{
  "apply": "تایید",
  "cancel": "انصراف",
  "noColor": "بدون رنگ",
  "clearColor": "پاک کردن رنگ"
});
}

/* ColorPicker messages */

if (kendo.ui.ColorPicker) {
kendo.ui.ColorPicker.prototype.options.messages =
$.extend(true, kendo.ui.ColorPicker.prototype.options.messages,{
  "apply": "تایید",
  "cancel": "انصراف",
  "noColor": "بدون رنگ",
  "clearColor": "پاک کردن رنگ"
});
}

/* ColumnMenu messages */

if (kendo.ui.ColumnMenu) {
kendo.ui.ColumnMenu.prototype.options.messages =
$.extend(true, kendo.ui.ColumnMenu.prototype.options.messages,{
  "sortAscending": "مرتب سازی صعودی",
  "sortDescending": "مرتب سازی نزولی",
  "filter": "فیلتر",
  "column": "ستون",
  "columns": "ستون ها",
  "columnVisibility": "نمایش ستون",
  "clear": "پاک کردن",
  "cancel": "انصراف",
  "done": "انجام شد",
  "settings": "تنظیمات ویرایش ستون",
  "lock": "قفل کردن ستون",
  "unlock": "بازکردن قفل ستون",
  "stick" : "چسباندن ستون",
  "unstick": "لغو چسباندن ستون",
  "setColumnPosition": "تعیین موقعیت ستون"
});
}

/* DateRangePicker messages */

if (kendo.ui.DateRangePicker) {
kendo.ui.DateRangePicker.prototype.options.messages =
$.extend(true, kendo.ui.DateRangePicker.prototype.options.messages,{
  "startLabel": "شروع",
  "endLabel": "پایان"
});
}

/* Editor messages */

if (kendo.ui.Editor) {
kendo.ui.Editor.prototype.options.messages =
$.extend(true, kendo.ui.Editor.prototype.options.messages,{
  "bold": "درشت",
  "italic": "کج",
  "underline": "خط زیرین",
  "strikethrough": "خط زده",
  "superscript": "متن بالا",
  "subscript": "متن پایین",
  "justifyCenter": "متن مرکزی",
  "justifyLeft": "تراز متن به چپ",
  "justifyRight": "تراز متن به راست",
  "justifyFull": "تراز متن",
  "insertUnorderedList": "درج لیست مرتب نشده",
  "insertOrderedList": "درج لیست مرتب شده",
  "indent": "تورفتگی",
  "outdent": "برون رفتگی",
  "createLink": "درج لینک",
  "unlink": "حذف لینک",
  "insertImage": "درج عکس",
  "insertFile": "درج فایل",
  "insertHtml": "درج HTML",
  "viewHtml": "نمایش HTML",
  "fontName": "انتخاب فونت",
  "fontNameInherit": "(نام فونت)",
  "fontSize": "انتخاب اندازه فونت",
  "fontSizeInherit": "(اندازه)",
  "formatBlock": "قالب",
  "formatting": "قالب",
  "foreColor": "رنگ",
  "backColor": "رنگ پس زمینه",
  "style": "سبک ها",
  "emptyFolder": "پوشه خالی",
  "uploadFile": "بارگذاری",
  "overflowAnchor": "سایر ابزارها",
  "orderBy": "ترتیب بر اساس:",
  "orderBySize": "اندازه",
  "orderByName": "نام",
  "invalidFileType": "فایل انتخاب شده \"{0}\" معتبر نیست. نوع فایل قابل پشتیبانی: {1}.",
  "deleteFile": 'آیا برای حذف این فایل مطمئن هستید "{0}"؟',
  "overwriteFile": 'یک فایل با نام "{0}" هم اکنون در پوشه موجود است. آیا برای بازنشانی فایل مطمئن هستید؟',
  "directoryNotFound": "پوشه با این نام پیدا نشد.",
  "imageWebAddress": "آدرس وب",
  "imageAltText": "متن جایگزین",
  "imageWidth": "عرض (px)",
  "imageHeight": "طول (px)",
  "fileWebAddress": "آدرس وب",
  "fileTitle": "عنوان",
  "linkWebAddress": "آدرس وب",
  "linkText": "متن",
  "linkToolTip": "راهنمای یک خطی",
  "linkOpenInNewWindow": "باز کردن لینک در پنجره جدید",
  "dialogUpdate": "بروزرسانی",
  "dialogInsert": "درج",
  "dialogButtonSeparator": "یا",
  "dialogCancel": "انصراف",
  "cleanFormatting": "پاک کردن قالب بندی",
  "createTable": "ساخت جدول",
  "addColumnLeft": "افزودن ستون در سمت چپ",
  "addColumnRight": "افزودن ستون در سمت راست",
  "addRowAbove": "افزودن سطر در بالا",
  "addRowBelow": "افزودن سطر در پایین",
  "deleteRow": "حذف سطر",
  "deleteColumn": "حذف ستون",
  "dialogOk": "تایید",
  "tableWizard": "ویزارد جدول",
  "tableTab": "جدول",
  "cellTab": "سلول",
  "accessibilityTab": "دسترسی",
  "caption": "عنوان",
  "summary": "خلاصه",
  "width": "عرض",
  "height": "طول",
  "units": "واحدها",
  "cellSpacing": "فاصله سلول",
  "cellPadding": "حاشیه داخلی",
  "cellMargin": "حاشیه بیرونی",
  "alignment": "هم ترازی",
  "background": "پس زمینه",
  "cssClass": "CSS کلاس",
  "id": "شناسه",
  "border": "مرز",
  "borderStyle": "سبک مرز",
  "collapseBorders": "جمع کردن مرز",
  "wrapText": "پوشاندن متن",
  "associateCellsWithHeaders": "سربرگ وابسته",
  "alignLeft": "تراز چپ",
  "alignCenter": "تراز وسط",
  "alignRight": "تراز راست",
  "alignLeftTop": "تراز چپ بالا",
  "alignCenterTop": "تراز وسط بالا",
  "alignRightTop": "تراز راست بالا",
  "alignLeftMiddle": "تراز چپ وسط",
  "alignCenterMiddle": "تراز مرکز وسط",
  "alignRightMiddle": "تراز راست وسط",
  "alignLeftBottom": "تراز چپ پایین",
  "alignCenterBottom": "تراز وسط پایین",
  "alignRightBottom": "تراز راست پایین",
  "alignRemove": "لغو تراز",
  "columns": "ستون ها",
  "rows": "سطرها",
  "selectAllCells": "انتخاب همه سلول ها",
  "print": "چاپ",
  "headerRows": "سربرگ سطرها",
  "headerColumns": "سربرگ ستون ها",
  "tableSummaryPlaceholder": "ویژگی خلاصه با HTML5 سازگار نیست.",
  "associateNone": "هیچکدام",
  "associateScope": "با استفاده از مشخصه 'دامنه' مرتبط شوید",
  "associateIds": "با استفاده از شناسه ها مرتبط شوید",
  "copyFormat": "کپی کردن قالب",
  "applyFormat": "اعمال قالب",
  "borderNone": "هیچکدام"
});
}

/* FileBrowser messages */

if (kendo.ui.FileBrowser) {
kendo.ui.FileBrowser.prototype.options.messages =
$.extend(true, kendo.ui.FileBrowser.prototype.options.messages,{
  "uploadFile": "بارگذاری",
  "orderBy": "مرتب سازی بر اساس",
  "orderByName": "نام",
  "orderBySize": "اندازه",
  "directoryNotFound": "فولدر مورد نظر پیدا نشد",
  "emptyFolder": "فولدر خالی",
  "deleteFile": 'آیا از حذف "{0}" اطمینان دارید؟',
  "invalidFileType": "انتخاب فایل با پسوند \"{0}\" امکانپذیر نیست. پسوندهای پشیتبانی شده: {1}",
  "overwriteFile": "فایل با نام \"{0}\" در فولدر انتخابی وجود دارد. آیا می خواهید آن را بازنویسی کنید؟",
  "dropFilesHere": "فایل را به اینجا بکشید",
  "search": "جستجو"
});
}

/* Filter Part */

var FilterOperatorsBase = {
  "string": {
    "eq": "مساوی",
    "neq": "نامساوی",
    "gte": "بزرگتر یا مساوی",
    "gt": "بزرگتر",
    "lte": "کوچکتر یا مساوی",
    "lt": "کوچکتر",
    "startswith": "شروع با",
    "contains": "شامل",
    "doesnotcontain": "شامل نباشد",
    "endswith": "پایان با",
    "isnull": "تهی",
    "isnotnull": "تهی نیست",
    "isempty": "خالی",
    "isnotempty": "خالی نیست",
    "isnullorempty": "بدون مقدار",
    "isnotnullorempty": "با مقدار"
  },
  "number": {
    "eq": "مساوی",
    "neq": "نامساوی",
    "gte": "بزرگتر یا مساوی",
    "gt": "بزرگتر",
    "lte": "کوچکتر یا مساوی",
    "lt": "کوچکتر",
    "isnull": "تهی",
    "isnotnull": "تهی نیست"
  },
  "date": {
    "eq": "مساوی",
    "neq": "نامساوی",
    "gte": "بزرگتر یا مساوی",
    "gt": "بزرگتر",
    "lte": "کوچکتر یا مساوی",
    "lt": "کوچکتر",
    "isnull": "تهی",
    "isnotnull": "تهی نیست"
  },
  "enums": {
    "eq": "مساوی",
    "neq": "نامساوی",
    "isnull": "تهی",
    "isnotnull": "تهی نیست"
  },
  "boolean": {
    "eq": "مساوی",
    "neq": "نامساوی"
  }
};

/* FilterCell messages */

if (kendo.ui.FilterCell) {
kendo.ui.FilterCell.prototype.options.messages =
$.extend(true, kendo.ui.FilterCell.prototype.options.messages,{
  "isTrue": "درست باشد",
  "isFalse": "درست نباشد",
  "filter": "فیلتر",
  "clear": "پاک کردن",
  "operator": "عملگر"
});
}

/* FilterCell operators */

if (kendo.ui.FilterCell) {
kendo.ui.FilterCell.prototype.options.operators =
$.extend(true, kendo.ui.FilterCell.prototype.options.operators, FilterOperatorsBase);
}

/* FilterMenu messages */

if (kendo.ui.FilterMenu) {
kendo.ui.FilterMenu.prototype.options.messages =
$.extend(true, kendo.ui.FilterMenu.prototype.options.messages,{
  "info": "نمایش مواردی که :",
  "title": "نمایش مواردی که",
  "isTrue": "درست باشد",
  "isFalse": "درست نباشد",
  "filter": "فیلتر",
  "clear": "پاک کردن",
  "and": "و",
  "or": "یا",
  "selectValue": "-انتخاب مقدار-",
  "operator": "عملگر",
  "value": "مقدار",
  "cancel": "انصراف",
  "done": "انجام شد",
  "into": "در"
});
}

/* FilterMenu operator messages */

if (kendo.ui.FilterMenu) {
kendo.ui.FilterMenu.prototype.options.operators =
$.extend(true, kendo.ui.FilterMenu.prototype.options.operators, FilterOperatorsBase);
}

/* FilterMultiCheck messages */

if (kendo.ui.FilterMultiCheck) {
kendo.ui.FilterMultiCheck.prototype.options.messages =
$.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages,{
  "checkAll": "انتخاب همه",
  "clearAll": "پاک کردن همه",
  "clear": "پاک کردن",
  "filter": "فیلتر",
  "search": "جستجو",
  "cancel": "انصراف",
  "selectedItemsFormat": "{0} مورد انتخاب شده",
  "done": "انجام شد",
  "into": "در"
});
}

/* Filter messages */

if (kendo.ui.Filter) {
kendo.ui.Filter.prototype.options.messages =
$.extend(true, kendo.ui.Filter.prototype.options.messages,{
  "apply": "تایید",
  "and": "و",
  "or": "یا",
  "addExpression": "افزودن عبارت",
  "addGroup": "افزودن شرط",
  "close": "بستن",
  "fields": "فیلدها",
  "operators": "عملگرها"
});
}

/* Filter operators */

if (kendo.ui.Filter) {
kendo.ui.Filter.prototype.options.operators =
$.extend(true, kendo.ui.Filter.prototype.options.operators, FilterOperatorsBase);
}

/* Gantt messages */

if (kendo.ui.Gantt) {
kendo.ui.Gantt.prototype.options.messages =
$.extend(true, kendo.ui.Gantt.prototype.options.messages,{
  "actions": {
    "addChild": "اضافه کردن فرزند",
    "append": "اضافه کردن کار",
    "insertAfter": "اضافه کن زیر",
    "insertBefore": "اضافه کن بالای",
    "pdf": "گرفتن خروجی PDF"
  },
  "cancel": "انصراف",
  "deleteDependencyWindowTitle": "حذف رابطه",
  "deleteTaskWindowTitle": "حذف کار",
  "destroy": "حذف",
  "editor": {
    "assingButton": "ارجاع دادن",
    "editorTitle": "کار",
    "end": "پایان",
    "percentComplete": "پیشرفت",
    "resources": "منابع",
    "resourcesEditorTitle": "منابع",
    "resourcesHeader": "منابع",
    "resourcesEditorTitle": "منابع",
    "resourcesHeader": "منابع",
    "start": "شروع",
    "title": "عنوان",
    "unitsHeader": "واحدها"
  },
  "plannedTasks": {
    "switchText": "وظایف برنامه ریزی شده",
    "offsetTooltipAdvanced": "مهلت ملاقات زودتر",
    "offsetTooltipDelay": "تاخیر انداختن",
    "seconds": "ثانیه",
    "minutes": "دقیقه",
    "hours": "ساعت",
    "days": "روز"
  },
  "save": "ذخیره",
  "views": {
    "day": "روز",
    "end": "پایان",
    "month": "ماه",
    "start": "شروع",
    "week": "هفته",
    "year": "سال"
  }
});
}


if(kendo.ui.FileManager) {
    kendo.ui.FileManager.prototype.options.messages =
        $.extend(true, kendo.ui.FileManager.prototype.options.messages, { 
            "toolbar": {
                "createFolder": "پوشه جدید",
                "upload": "بارگذاری",
                "sortDirection": "مرتب سازی",
                "sortDirectionAsc": "مرتب سازی صعودی",
                "sortDirectionDesc": "مرتب سازی نزولی",
                "sortField": "مرتب سازی براساس",
                "nameField": "نام",
                "sizeField": "سایز فایل",
                "typeField": "نوع",
                "dateModifiedField": "تاریخ اصلاح شده",
                "dateCreatedField": "تاریخ ایجاد",
                "listView": "نمایش لیست",
                "gridView": "نمایش شبکه",
                "search": "جستجو",
                "details": "نمایش جزئیات",
                "detailsChecked": "روشن",
                "detailsUnchecked": "خاموش",
                "delete": "حذف",
                "rename": "تغییر نام"
            },
            "views": {
                "nameField": "نام",
                "sizeField": "سایز فایل",
                "typeField": "نوع",
                "dateModifiedField": "تاریخ اصلاح شده",
                "dateCreatedField": "تاریخ ایجاد",
                "items": "موارد"
            },
            "dialogs": {
                "upload": {
                    "title": "بارگذاری",
                    "clear": "پاک کردن",
                    "done": "انجام شد"
                },
                "moveConfirm": {
                    "title": "تایید",
                    "content": "<p style='text-align: center;'>آیا می خواهید انتقال دهید یا کپی کنید؟</p>",
                    "okText": "کپی",
                    "cancel": "انتقال",
                    "close": "بستن"
                },
                "deleteConfirm": {
                    "title": "Confirm",
                    "content": "<p style='text-align: center;'>آیا مطمئن هستید که می خواهید فایل(های) انتخاب شده را حذف کنید؟</p>",
                    "okText": "حذف",
                    "cancel": "انصراف",
                    "close": "بستن"
                },
                "renamePrompt": {
                    "title": "سریع",
                    "content": "<p style='text-align: center;'>نام جدیدی برای فایل وارد کنید</p>",
                    "okText": "تغییر نام",
                    "cancel": "انصراف",
                    "close": "بستن"
                }
            },
            "previewPane": {
                "noFileSelected": "هیچ فایلی انتخاب نشده است",
                "extension": "نوع",
                "size": "سایز",
                "created": "تاریخ ایجاد",
                "createdUtc": "تاریخ ایجاد UTC",
                "modified": "تاریخ اصلاح",
                "modifiedUtc": "تاریخ اصلاح UTC",
                "items": "موارد"
            }
        });
}
    
if(kendo.ui.ImageEditor) {
    kendo.ui.ImageEditor.prototype.options.messages =
        $.extend(true, kendo.ui.ImageEditor.prototype.options.messages, {
             "toolbar": {
                "open": "Open Image",
                "save": "ذخیره عکس",
                "undo": "عقب",
                "redo": "جلو",
                "crop": "بریدن",
                "resize": "تغییر اندازه",
                "zoomIn": "بزرگ نمایی",
                "zoomOut": "کوچک نمایی",
                "zoomDropdown": "گزینه های نمایش",
                "zoomActualSize": "اندازه واقعی",
                "zoomFitToScreen": "متناسب با صفحه"
            },
            "panes": {
                "crop": {
                    "title": "برش تصویر",
                    "aspectRatio": "نسبت تصویر:",
                    "aspectRatioItems": {
                        "originalRatio": "نسبت اصلی",
                        "1:1": "1:1 (مربع)",
                        "4:5": "4:5 (8:10)",
                        "5:7": "5:7",
                        "2:3": "2:3 (4:6)",
                        "16:9": "16:9"
                    },
                    "orientation": "جهت:",
                    "portrait": "پرتره",
                    "landscape": "چشم انداز"
                },
                "resize": {
                    "title": "تغییر اندازه تصویر",
                    "pixels": "پیکسل",
                    "percents": "درصد"
                }
            },
            "common": {
                "width": "عرض:",
                "height": "طول:",
                "cancel": "انصراف",
                "confirm": "تایید",
                "lockAspectRatio": "قفل نسبت ابعاد"
            } 
        });
}



/* Grid messages */

if (kendo.ui.Grid) {
kendo.ui.Grid.prototype.options.messages =
$.extend(true, kendo.ui.Grid.prototype.options.messages,{
  "commands": {
    "cancel": "انصراف",
    "canceledit": "انصراف",
    "create": "افزودن رکورد جدید",
    "destroy": "حذف",
    "edit": "ویرایش",
    "excel": "خروجی Excel",
    "pdf": "خروجی PDF",
    "save": "ذخیره",
    "select": "انتخاب",
    "update": "ذخیره"
  },
  "editable": {
    "cancelDelete": "انصراف",
    "confirmation": "آیا از حذف این ردیف مطمئنید؟",
    "confirmDelete": "حذف"
  },
  "noRecords": "اطلاعاتی وجود ندارد",
  "search": "جستجو...",
  "expandCollapseColumnHeader": "",
  "groupHeader": "برای گروه بندی : ctrl + space",
  "ungroupHeader": "برای لغو گروه بندی : ctrl + space"
});
}

/* TreeList messages */

if (kendo.ui.TreeList) {
kendo.ui.TreeList.prototype.options.messages =
$.extend(true, kendo.ui.TreeList.prototype.options.messages,{
    "noRows": "اطلاعاتی وجود ندارد",
    "loading": "در حال بارگذاری...",
    "requestFailed": "شکست در انجام درخواست.",
    "retry": "تلاش مجدد",
    "commands": {
        "edit": "ویرایش",
        "update": "بروزرسانی",
        "canceledit": "انصراف",
        "create": "افزودن رکورد جدید",
        "createchild": "افزودن رکورد زیرمجموعه",
        "destroy": "حذف",
        "excel": "خروجی Excel",
        "pdf": "خروجی PDF"
    }
});
}

/* Groupable messages */

if (kendo.ui.Groupable) {
kendo.ui.Groupable.prototype.options.messages =
$.extend(true, kendo.ui.Groupable.prototype.options.messages,{
  "empty": "ناحیه گروه بندی:"
});
}

/* NumericTextBox messages */

if (kendo.ui.NumericTextBox) {
kendo.ui.NumericTextBox.prototype.options =
$.extend(true, kendo.ui.NumericTextBox.prototype.options,{
  "upArrowText": "افزایش مقدار",
  "downArrowText": "کاهش مقدار"
});
}

/* MediaPlayer messages */

if (kendo.ui.MediaPlayer) {
kendo.ui.MediaPlayer.prototype.options.messages =
$.extend(true, kendo.ui.MediaPlayer.prototype.options.messages,{
  "pause": "مکث",
  "play": "پخش",
  "mute": "قطع صدا",
  "unmute": "باز کردن صدا",
  "quality": "کیفیت",
  "fullscreen": "تمام صفحه"
});
}

/* Pager messages */

if (kendo.ui.Pager) {
kendo.ui.Pager.prototype.options.messages =
$.extend(true, kendo.ui.Pager.prototype.options.messages,{
  "allPages": "همه",
  "display": "ردیف {0} تا {1} از {2}",
  "empty": "ردیفی برای نمایش وجود ندارد",
  "page": "صفحه",
  "of": "از {0}",
  "itemsPerPage": "ردیف در هر صفحه",
  "first": "برو به صفحه اول",
  "previous": "برو به صفحه قبل",
  "next": "برو به صفحه بعد",
  "last": "برو به صفحه آخر",
  "refresh": "بارگذاری مجدد",
    "morePages": "صفحات بیشتر", 
    "pageButtonLabel": "صفحه {0}",
    "pageSizeDropDownLabel":"اندازه صفحه کشویی"
});
    }




/* TreeListPager messages */

if (kendo.ui.TreeListPager) {
kendo.ui.TreeListPager.prototype.options.messages =
$.extend(true, kendo.ui.TreeListPager.prototype.options.messages,{
  "allPages": "همه",
  "display": "ردیف {0} تا {1} از {2}",
  "empty": "ردیفی برای نمایش وجود ندارد",
  "page": "صفحه",
  "of": "از {0}",
  "itemsPerPage": "ردیف در هر صفحه",
  "first": "برو به صفحه اول",
  "previous": "برو به صفحه قبل",
  "next": "برو به صفحه بعد",
  "last": "برو به صفحه آخر",
  "refresh": "بارگذاری مجدد",
  "morePages": "صفحات بیشتر"
});
}

/* PivotGrid messages */

if (kendo.ui.PivotGrid) {
kendo.ui.PivotGrid.prototype.options.messages =
$.extend(true, kendo.ui.PivotGrid.prototype.options.messages,{
  "measureFields": "فیلدهای داده را اینجا رها کنید",
  "columnFields": "فیلدهای ستون را اینجا رها کنید",
  "rowFields": "فیلدهای ردیف را اینجا رها کنید"
});
}

/* PivotFieldMenu messages */

if (kendo.ui.PivotFieldMenu) {
kendo.ui.PivotFieldMenu.prototype.options.messages =
$.extend(true, kendo.ui.PivotFieldMenu.prototype.options.messages,{
  "info": "نمایش مواردی که :",
  "filterFields": "فیلتر فیلدها",
  "filter": "فیلتر",
  "include": "شامل فیلدهای...",
  "title": "شامل فیلدها",
  "clear": "پاک کردن",
  "ok": "تایید",
  "cancel": "انصراف",
  "operators": {
    "contains": "شامل باشد",
    "doesnotcontain": "شامل نباشد",
    "startswith": "شروع شود با",
    "endswith": "پایان یابد با",
    "eq": "برابر باشد با",
    "neq": "برابر نباشد با"
  }
});
}

/* RecurrenceEditor messages */

if (kendo.ui.RecurrenceEditor) {
kendo.ui.RecurrenceEditor.prototype.options.messages =
$.extend(true, kendo.ui.RecurrenceEditor.prototype.options.messages,{
  "repeat": "Repeat",
  "frequencies": {
    "never": "هرگز",
    "hourly": "ساعتی",
    "daily": "روزانه",
    "weekly": "هفتگی",
    "monthly": "ماهانه",
    "yearly": "سالیانه"
  },
  "hourly": {
    "repeatEvery": "تکرار کن هر: ",
    "interval": " ساعت"
  },
  "daily": {
    "repeatEvery": "تکرار کن هر: ",
    "interval": " روز"
  },
  "weekly": {
    "interval": " هفته",
    "repeatEvery": "تکرار کن هر: ",
    "repeatOn": "تکرار کن در: "
  },
  "monthly": {
    "repeatEvery": "تکرار کن هر: ",
    "repeatOn": "تکرار کن در: ",
    "interval": "ماه ",
    "day": "روز ",
    "date": "تاریخ"
  },
  "yearly": {
    "repeatEvery": "تکرار کن هر: ",
    "repeatOn": "تکرار کن در: ",
    "interval": " سال",
    "of": " از ",
    "month": "ماه",
    "day": "روز",
    "date": "تاریخ"
  },
  "end": {
    "label": "پایان:",
    "mobileLabel": "پایان",
    "never": "هرگز",
    "after": "بعد از ",
    "occurrence": " دفعات وقوع",
    "on": "هنگام "
  },
  "offsetPositions": {
    "first": "اول",
    "second": "دوم",
    "third": "سوم",
    "fourth": "چهارم",
    "last": "آخر"
  },
  "weekdays": {
    "day": "روز",
    "weekday": "روز هفته",
    "weekend": "پایان هفته"
  }
});
}

/* MobileRecurrenceEditor messages */

if (kendo.ui.MobileRecurrenceEditor) {
    kendo.ui.MobileRecurrenceEditor.prototype.options.messages =
    $.extend(true, kendo.ui.MobileRecurrenceEditor.prototype.options.messages, kendo.ui.RecurrenceEditor.prototype.options.messages, {
      "endTitle": "پایان تکرار",
      "repeatTitle": "الگوی تکرار",
      "headerTitle": "رویداد تکرار",
      "end": {
        "patterns": {
            "never": "هرگز",
            "after": "بعد از...",
            "on": "هنگام..."
        }
      },
      "monthly": {
        "repeatBy": "تکرار توسط: ",
        "dayOfMonth": "روز ماه",
        "dayOfWeek": "روز هفته",
        "every": "هر"
      },
      "yearly": {
        "repeatBy": "تکرار توسط: ",
        "dayOfMonth": "روز ماه",
        "dayOfWeek": "روز هفته",
        "every": "هر",
        "month": "ماه",
        "day": "روز"
      }
    });
}

/* Scheduler messages */

if (kendo.ui.Scheduler) {
kendo.ui.Scheduler.prototype.options.messages =
$.extend(true, kendo.ui.Scheduler.prototype.options.messages,{
  "allDay": "تمام روز",
  "date": "تاریخ",
  "event": "رویداد",
  "time": "زمان",
  "showFullDay": "نمایش تمام روز",
  "showWorkDay": "نمایش ساعات کاری",
  "today": "امروز",
  "save": "ذخیره",
  "cancel": "انصراف",
  "destroy": "حذف",
  "resetSeries": "تنظیم مجدد سری",
  "deleteWindowTitle": "حذف رویداد",
  "ariaSlotLabel": "انتخاب از {0:t} تا {1:t}",
  "ariaEventLabel": "{0} هنگام {1:D} در {2:t}",
  "editable": {
    "confirmation": "آیا شما مطمئن هستید که میخواهید این رویداد را حذف کنید؟"
  },
  "views": {
    "day": "روز",
    "week": "هفته",
    "workWeek": "هفته کاری",
    "agenda": "دستور جلسه",
    "month": "ماه"
  },
  "recurrenceMessages": {
    "deleteWindowTitle": "مورد تکرار شونده را حذف کنید",
    "resetSeriesWindowTitle": "تنظیم مجدد سری",
    "deleteWindowOccurrence": "رخداد فعلی را حذف کنید",
    "deleteWindowSeries": "حذف سری",
    "deleteRecurringConfirmation": "آیا مطمئن هستید که می خواهید این رویداد را حذف کنید؟",
    "deleteSeriesConfirmation": "آیا مطمئن هستید که می خواهید کل مجموعه را حذف کنید؟",
    "editWindowTitle": "ویرایش مورد تکرار شونده",
    "editWindowOccurrence": "ویرایش وقوع فعلی",
    "editWindowSeries": "ویرایش سری",
    "deleteRecurring": "آیا می خواهید فقط این وقوع رویداد یا کل مجموعه را حذف کنید؟",
    "editRecurring": "آیا می خواهید فقط این وقوع رویداد یا کل مجموعه را ویرایش کنید؟"
  },
  "editor": {
    "title": "عنوان",
    "start": "شروع",
    "end": "پایان",
    "allDayEvent": "رویداد تمام روز",
    "description": "شرح",
    "repeat": "تکرار",
    "timezone": " ",
    "startTimezone": "شروع منطقه زمانی",
    "endTimezone": "پایان منطقه زمانی",
    "separateTimezones": "استفاده از مناطق زمانی جداگانه شروع و پایان",
    "timezoneEditorTitle": "مناطق زمانی",
    "timezoneEditorButton": "منطقه زمانی",
    "timezoneTitle": "مناطق زمانی",
    "noTimezone": "بدون منطقه زمانی",
    "editorTitle": "رویداد"
  },
  "search": "جستجو..."
});
}

/* Spreadsheet messages */

if (kendo.spreadsheet && kendo.spreadsheet.messages.borderPalette) {
kendo.spreadsheet.messages.borderPalette =
$.extend(true, kendo.spreadsheet.messages.borderPalette,{
  "allBorders": "همه مرزها",
  "insideBorders": "مرزهای داخلی",
  "insideHorizontalBorders": "مرزهای داخلی افقی",
  "insideVerticalBorders": "مرزهای داخلی عمودی",
  "outsideBorders": "مرزهای خارجی",
  "leftBorder": "مرز چپ",
  "topBorder": "مرز بالا",
  "rightBorder": "مرز راست",
  "bottomBorder": "مرز پایین",
  "noBorders": "بدون مرز",
  "reset": "بازنشانی رنگ",
  "customColor": "رنگ دلخواه...",
  "apply": "تایید",
  "cancel": "انصراف"
});
}

if (kendo.spreadsheet && kendo.spreadsheet.messages.dialogs) {
kendo.spreadsheet.messages.dialogs =
$.extend(true, kendo.spreadsheet.messages.dialogs,{
  "apply": "تایید",
  "save": "ذخیره",
  "cancel": "انصراف",
  "remove": "حذف",
  "retry": "تلاش مجدد",
  "revert": "برگرداندن",
  "okText": "تایید",
  "formatCellsDialog": {
    "title": "قالب بندی",
    "categories": {
      "number": "عدد",
      "currency": "پول",
      "date": "تاریخ"
      }
  },
  "fontFamilyDialog": {
    "title": "فونت"
  },
  "fontSizeDialog": {
    "title": "اندازه فونت"
  },
  "bordersDialog": {
    "title": "مرزها"
  },
  "alignmentDialog": {
    "title": "هم ترازی",
    "buttons": {
     "justtifyLeft": "تراز چپ",
     "justifyCenter": "وسط",
     "justifyRight": "تراز راست",
     "justifyFull": "تراز متن",
     "alignTop": "تراز بالا",
     "alignMiddle": "تراز مرکز",
     "alignBottom": "تراز پایین"
    }
  },
  "mergeDialog": {
    "title": "ادغام سلول ها",
    "buttons": {
      "mergeCells": "ادغام همه",
      "mergeHorizontally": "ادغام افقی",
      "mergeVertically": "ادغام عمودی",
      "unmerge": "خروج از ادغام"
    }
  },
  "freezeDialog": {
    "title": "فریز کردن پن",
    "buttons": {
      "freezePanes": "فریز کردن پن ها",
      "freezeRows": "فریز کردن ردیف ها",
      "freezeColumns": "فریز کردن ستون ها",
      "unfreeze": "خروج از فریز کردن پن ها"
    }
  },
  "confirmationDialog": {
    "text": "آیا مطمئن هستید که می خواهید این برگه را حذف کنید؟",
    "title": "حذف برگه"
  },
  "validationDialog": {
    "title": "اعتبار سنجی داده ها",
    "hintMessage": "لطفا مقدار معتبر {0} وارد کنید {1}.",
    "hintTitle": "اعتبار سنجی {0}",
    "criteria": {
      "any": "هر مقدار",
      "number": "عدد",
      "text": "متن",
      "date": "تاریخ",
      "custom": "فرمول دلخواه",
      "list": "فهرست"
    },
    "comparers": {
      "greaterThan": "بزرگتر از",
      "lessThan": "کوچکتر از",
      "between": "در بازه",
      "notBetween": "در بازه نباشد",
      "equalTo": "مساوی",
      "notEqualTo": "نامساوی",
      "greaterThanOrEqualTo": "بزرگتر یا مساوی",
      "lessThanOrEqualTo": "کوچکتر یا مساوی"
    },
    "comparerMessages": {
      "greaterThan": "بزرگتر از {0}",
      "lessThan": "کوچکتر از {0}",
      "between": "در بازه {0} و {1}",
      "notBetween": "خارج از بازه {0} و {1}",
      "equalTo": "مساوی {0}",
      "notEqualTo": "نامساوی {0}",
      "greaterThanOrEqualTo": "بزرگتر یا مساوی {0}",
      "lessThanOrEqualTo": "کوچکتر یا مساوی {0}",
      "custom": "که فرمول را برآورده می کند: {0}"
    },
    "labels": {
      "criteria": "شاخص",
      "comparer": "مقایسه گر",
      "min": "کمترین",
      "max": "بیشترین",
      "value": "مقدار",
      "start": "شروع",
      "end": "پایان",
      "onInvalidData": "هنگام داده نامعتبر",
      "rejectInput": "رد کردن ورودی",
      "showWarning": "مشاهده هشدار",
      "showHint": "مشاهده راهنما",
      "hintTitle": "عنوان راهنما",
      "hintMessage": "پیام راهنما",
      "ignoreBlank": "نادیده گرفتن فضای خالی"
    },
    "placeholders": {
      "typeTitle": "عنوان نوع",
      "typeMessage": "پیام نوع"
    }
  },
  "exportAsDialog": {
    "title": "خروجی...",
    "labels": {
      "fileName": "نام فایل",
      "saveAsType": "ذخیره بر اساس",
      "exportArea": "خروجی",
      "paperSize": "اندازه کاغذ",
      "margins": "حاشیه ها",
      "orientation": "جهت",
      "print": "چاپ",
      "guidelines": "رهنمودها",
      "center": "وسط",
      "horizontally": "افقی",
      "vertically": "عمودی"
    }
  },
  "modifyMergedDialog": {
    "errorMessage": "نمی توان بخشی از سلول ادغام شده را تغییر داد."
  },
  "useKeyboardDialog": {
    "title": "کپی و درج",
    "errorMessage": "این اقدامات را نمی توان از طریق فهرست فراخوانی کرد. لطفاً به جای آن از میانبرهای صفحه کلید استفاده کنید:",
    "labels": {
      "forCopy": "برای کپی",
      "forCut": "برای برش",
      "forPaste": "برای درج"
    }
  },
  "unsupportedSelectionDialog": {
    "errorMessage": "این عمل را نمی توان با انتخاب چندگانه انجام داد."
  },
  "insertCommentDialog": {
    "title": "درج توضیحات",
    "labels": {
      "comment": "توضیح",
      "removeComment": "حذف توضیح"
    }
  },
  "insertImageDialog": {
    "title": "درج عکس",
    "info": "یک تصویر را در اینجا بکشید یا برای انتخاب کلیک کنید",
    "typeError": "لطفاً یک تصویر JPEG ، PNG یا GIF انتخاب کنید"
  }
});
}

if (kendo.spreadsheet && kendo.spreadsheet.messages.filterMenu) {
kendo.spreadsheet.messages.filterMenu =
$.extend(true, kendo.spreadsheet.messages.filterMenu,{
  "sortAscending": "ترتیب صعودی",
  "sortDescending": "ترتیب نزولی",
  "filterByValue": "فیلتر بر اساس مقدار",
  "filterByCondition": "فیلتر به شرط",
  "apply": "تایید",
  "search": "جستجو",
  "addToCurrent": "درج در انتخاب جاری",
  "clear": "پاک کردن",
  "blanks": "(فاصله ها)",
  "operatorNone": "هیچکدام",
  "and": "و",
  "or": "یا",
  "operators": {
    "string": {
      "contains": "شامل متن",
      "doesnotcontain": "ناشامل متن",
      "startswith": "شروع متن با",
      "endswith": "پایان متن با"
    },
    "date": {
      "eq":  "تاریخ مساوی",
      "neq": "تاریخ مخالف",
      "lt":  "قبل از تاریخ",
      "gt":  "بعد از تاریخ"
    },
    "number": {
      "eq": "برابر باشد با",
      "neq": "برابر نباشد با",
      "gte": "برابر یا بزرگتر باشد از",
      "gt": "بزرگتر باشد از",
      "lte": "کمتر و یا برابر باشد با",
      "lt": "کمتر باشد از"
    }
  }
});
}

if (kendo.spreadsheet && kendo.spreadsheet.messages.colorPicker) {
kendo.spreadsheet.messages.colorPicker =
$.extend(true, kendo.spreadsheet.messages.colorPicker,{
  "reset": "بازنشانی رنگ",
  "customColor": "رنگ دلخواه...",
  "apply": "تایید",
  "cancel": "انصراف"
});
}

if (kendo.spreadsheet && kendo.spreadsheet.messages.toolbar) {
kendo.spreadsheet.messages.toolbar =
$.extend(true, kendo.spreadsheet.messages.toolbar,{
  "addColumnLeft": "افزودن ستون در سمت چپ",
  "addColumnRight": "افزودن ستون در سمت راست",
  "addRowAbove": "افزودن سطر در بالا",
  "addRowBelow": "افزودن سطر در پایین",
  "alignment": "هم ترازی",
  "alignmentButtons": {
    "justtifyLeft": "تراز چپ",
    "justifyCenter": "وسط",
    "justifyRight": "تراز راست",
    "justifyFull": "تراز متن",
    "alignTop": "تراز بالا",
    "alignMiddle": "تراز مرکز",
    "alignBottom": "تراز پایین"
  },
  "backgroundColor": "پس زمینه",
  "bold": "درشت",
  "borders": "مرزها",
  "colorPicker": {
    "reset": "بازنشانی رنگ",
    "customColor": "رنگ دلخواه..."
  },
  "copy": "کپی",
  "cut": "برش",
  "deleteColumn": "حذف ستون",
  "deleteRow": "حذف ردیف",
  "excelImport": "دریافت از Excel...",
  "filter": "فیلتر",
  "fontFamily": "فونت",
  "fontSize": "اندازه فونت",
  "format": "قالب دلخواه...",
  "formatTypes": {
    "automatic": "خودکار",
    "number": "عدد",
    "percent": "درصد",
    "financial": "مالی",
    "currency": "پول",
    "date": "تاریخ",
    "time": "ساعت",
    "dateTime": "تاریخ ساعت",
    "duration": "مدت",
    "moreFormats": "قالب های بیشتر..."
  },
  "formatDecreaseDecimal": "اعشار کمتر",
  "formatIncreaseDecimal": "اعشار بیشتر",
  "freeze": "فریز کردن پن",
  "freezeButtons": {
    "freezePanes": "فریز کردن پن ها",
    "freezeRows": "فریز کردن ردیف ها",
    "freezeColumns": "فریز کردن ستون ها",
    "unfreeze": "خروج از فریز کردن پن ها"
  },
  "insertComment": "درج توضیح",
  "insertImage": "درج عکس",
  "italic": "کج",
  "merge": "ادغام سلول ها",
  "mergeButtons": {
    "mergeCells": "ادغام همه",
    "mergeHorizontally": "ادغام افقی",
    "mergeVertically": "ادغام عمودی",
    "unmerge": "خروج از ادغام"
  },
  "open": "باز کردن...",
  "paste": "درج",
  "quickAccess": {
    "redo": "انجام دوباره",
    "undo": "ابطال"
  },
  "saveAs": "ذخیره بعنوان...",
  "sortAsc": "مرتب سازی صعودی",
  "sortDesc": "مرتب سازی نزولی",
  "sortButtons": {
    "sortSheetAsc": "ترتیب صعودی برگه",
    "sortSheetDesc": "ترتیب نزولی برگه",
    "sortRangeAsc": "ترتیب صعودی بازه",
    "sortRangeDesc": "ترتیب نزولی بازه"
  },
  "textColor": "رنگ متن",
  "textWrap": "پوشاندن متن",
  "underline": "خط زیرین",
  "validation": "اعتبارسنجی داده..."
});
}

if (kendo.spreadsheet && kendo.spreadsheet.messages.view) {
kendo.spreadsheet.messages.view =
$.extend(true, kendo.spreadsheet.messages.view,{
  "errors": {
    "shiftingNonblankCells": "به دلیل احتمال از دست رفتن داده ، نمی توان سلول ها را وارد کرد. مکان دیگری را وارد کنید یا داده ها را از انتهای صفحه کار خود حذف کنید.",
    "filterRangeContainingMerges": "نمی توان در محدوده حاوی ادغام ها فیلتر ایجاد کرد",
    "validationError": "مقداری که وارد کردید قوانین اعتبارسنجی تنظیم شده در سلول را نقض می کند."
  },
  "tabs": {
    "home": "خانه",
    "insert": "درج",
    "data": "داده"
  }
});
}

/* Slider messages */

if (kendo.ui.Slider) {
kendo.ui.Slider.prototype.options =
$.extend(true, kendo.ui.Slider.prototype.options,{
  "increaseButtonTitle": "افزایش",
  "decreaseButtonTitle": "کاهش"
});
}

/* ListBox messaages */

if (kendo.ui.ListBox) {
kendo.ui.ListBox.prototype.options.messages =
$.extend(true, kendo.ui.ListBox.prototype.options.messages,{
  "tools": {
    "remove": "حذف",
    "moveUp": "حرکت به بالا",
    "moveDown": "حرکت به پایین",
    "transferTo": "انتقال به",
    "transferFrom": "انتقال از",
    "transferAllTo": "انتقال همه به",
    "transferAllFrom": "انتقال همه از"
  }
});
}

/* TreeList messages */

if (kendo.ui.TreeList) {
kendo.ui.TreeList.prototype.options.messages =
$.extend(true, kendo.ui.TreeList.prototype.options.messages,{
  "noRows": "اطلاعاتی وجود ندارد",
  "loading": "در حال بارگذاری...",
  "requestFailed": "شکست در انجام درخواست.",
  "retry": "تلاش مجدد",
  "commands": {
      "edit": "ویرایش",
      "update": "ذخیره",
      "canceledit": "انصراف",
      "create": "درج ردیف جدید",
      "createchild": "درج گره جدید",
      "destroy": "حذف",
      "excel": "خروجی Excel",
      "pdf": "خروجی PDF"
  }
});
}

/* TreeView messages */

if (kendo.ui.TreeView) {
kendo.ui.TreeView.prototype.options.messages =
$.extend(true, kendo.ui.TreeView.prototype.options.messages,{
  "loading": "در حال بارگذاری...",
  "requestFailed": "شکست در انجام درخواست.",
  "retry": "تلاش مجدد"
});
}

/* Upload messages */

if (kendo.ui.Upload) {
kendo.ui.Upload.prototype.options.localization=
$.extend(true, kendo.ui.Upload.prototype.options.localization,{
  "select": "انتخاب فایل",
  "cancel": "انصراف",
  "retry": "تلاش مجدد",
  "remove": "حذف",
  "clearSelectedFiles": "پاک کردن",
  "uploadSelectedFiles": "بارگذاری فایل",
  "dropFilesHere": "فایل را اینجا بکشید",

  "statusUploading": "در حال بارگذاری",
  "statusUploaded": "پایان بارگذاری",
  "statusWarning": "هشدار",
  "statusFailed": "خطا در بارگذاری",
  "headerStatusUploading": "در حال بارگذاری...",
  "headerStatusPaused": "متوقف شد",
  "headerStatusUploaded": "اتمام بارگذاری",
  "uploadSuccess": "با موفقیت بارگذاری شد",
  "uploadFail": "عدم بارگذاری فایل",
  "invalidMaxFileSize": "فایل بسیار بزرگ است",
  "invalidMinFileSize": "فایل بسیار کوچک است",
  "invalidFileExtension": "نوع فایل مجاز نیست"
});
}

/* Validator messages */

if (kendo.ui.Validator) {
kendo.ui.Validator.prototype.options.messages =
$.extend(true, kendo.ui.Validator.prototype.options.messages,{
  "required": "الزامی است",
  "pattern": "الگو معتبر نیست",
  "min": "خارج از محدوده",
  "max": "خارج از محدوده",
  "step": "معتبر نیست",
  "email": "معتبر نیست",
  "url": "معتبر نیست",
  "date": "معتبر نیست",
  "dateCompare": "تاریخ غیرمجاز",
  "dropdownlistrequired": "الزامی است",
  "exclusive": "تکراری است",
  "custom": "صحیح نیست"
});
}

/* kendo.ui.progress method */
if (kendo.ui.progress) {
kendo.ui.progress.messages =
$.extend(true, kendo.ui.progress.messages, {
    loading: "در حال بارگذاری..."
});
}

/* Dialog */

if (kendo.ui.Dialog) {
kendo.ui.Dialog.prototype.options.messages =
$.extend(true, kendo.ui.Dialog.prototype.options.localization, {
  "close": "بستن"
});
}

/* Calendar */
if (kendo.ui.Calendar) {
kendo.ui.Calendar.prototype.options.messages =
$.extend(true, kendo.ui.Calendar.prototype.options.messages, {
  "weekColumnHeader": ""
});
}

/* Alert */

if (kendo.ui.Alert) {
kendo.ui.Alert.prototype.options.messages =
$.extend(true, kendo.ui.Alert.prototype.options.localization, {
  "okText": "تایید"
});
}

/* Confirm */

if (kendo.ui.Confirm) {
kendo.ui.Confirm.prototype.options.messages =
$.extend(true, kendo.ui.Confirm.prototype.options.localization, {
  "okText": "تایید",
  "cancel": "انصراف"
});
}

/* Prompt */
if (kendo.ui.Prompt) {
kendo.ui.Prompt.prototype.options.messages =
$.extend(true, kendo.ui.Prompt.prototype.options.localization, {
  "okText": "تایید",
  "cancel": "انصراف"
});
}

/* DateInput */
if (kendo.ui.DateInput) {
  kendo.ui.DateInput.prototype.options.messages =
    $.extend(true, kendo.ui.DateInput.prototype.options.messages, {
      "year": "سال",
      "month": "ماه",
      "day": "روز",
      "weekday": "روز هفته",
      "hour": "ساعت",
      "minute": "دقیقه",
      "second": "ثانیه",
      "dayperiod": "ق ظ/ب ظ"
    });
}

/* VirtualList messages */

if (kendo.ui.VirtualList) {
    kendo.ui.VirtualList.prototype.options.placeholderTemplate = "<span class='k-nodata'>در حال بارگذاری...</span>";
}

/* List messages */

if (kendo.ui.List) {
    kendo.ui.List.prototype.options.messages =
    $.extend(true, kendo.ui.List.prototype.options.messages,{
      "clear": "پاک کردن",
      "noData": "اطلاعاتی وجود ندارد"
    });
}

/* DropDownList messages */

if (kendo.ui.DropDownList) {
    kendo.ui.DropDownList.prototype.options.messages =
    $.extend(true, kendo.ui.DropDownList.prototype.options.messages, kendo.ui.List.prototype.options.messages);
}

/* ComboBox messages */

if (kendo.ui.ComboBox) {
    kendo.ui.ComboBox.prototype.options.messages =
    $.extend(true, kendo.ui.ComboBox.prototype.options.messages, kendo.ui.List.prototype.options.messages);
}

/* AutoComplete messages */

if (kendo.ui.AutoComplete) {
    kendo.ui.AutoComplete.prototype.options.messages =
    $.extend(true, kendo.ui.AutoComplete.prototype.options.messages, kendo.ui.List.prototype.options.messages);
}

/* MultiColumnComboBox messages */

if (kendo.ui.MultiColumnComboBox) {
    kendo.ui.MultiColumnComboBox.prototype.options.messages =
    $.extend(true, kendo.ui.MultiColumnComboBox.prototype.options.messages, kendo.ui.List.prototype.options.messages);
}

/* DropDownTree messages */

if (kendo.ui.DropDownTree) {
    kendo.ui.DropDownTree.prototype.options.messages =
    $.extend(true, kendo.ui.DropDownTree.prototype.options.messages,{
        "singleTag": "مورد انتخاب شده",
        "clear": "پاک کردن",
        "deleteTag": "حذف",
        "noData": "اطلاعاتی وجود ندارد"
    });
}

/* MultiSelect messages */

if (kendo.ui.MultiSelect) {
    kendo.ui.MultiSelect.prototype.options.messages =
    $.extend(true, kendo.ui.MultiSelect.prototype.options.messages,{
        "singleTag": "مورد انتخاب شده",
        "clear": "پاک کردن",
        "deleteTag": "حذف",
        "noData": "اطلاعاتی وجود ندارد"
    });
}

/* Chat messages */

if (kendo.ui.Chat) {
    kendo.ui.Chat.prototype.options.messages =
    $.extend(true, kendo.ui.Chat.prototype.options.messages,{
        "placeholder": "یک پیام بنویسید...",
        "toggleButton": "وضعیت نوارابزار",
        "sendButton": "ارسال پیام"
    });
}

/* Wizard messages */

if (kendo.ui.Wizard) {
    kendo.ui.Wizard.prototype.options.messages =
    $.extend(true, kendo.ui.Wizard.prototype.options.messages,{
        "reset": "بازنشانی",
        "previous": "قبلی",
        "next": "بعدی",
        "done": "انجام شد",
        "step": "گام",
        "of": "از"
    });
}

/* PDFViewer messages */

if (kendo.ui.PDFViewer) {
    kendo.ui.PDFViewer.prototype.options.messages =
    $.extend(true, kendo.ui.PDFViewer.prototype.options.messages, {
        defaultFileName: "سند",
        toolbar: {
            zoom: {
                zoomLevel: "سطح بزرگنمایی",
                zoomOut: "کاهش بزرگنمایی",
                zoomIn: "افزایش بزرگنمایی",
                actualWidth: "عرض واقعی",
                autoWidth: "عرض خودکار",
                fitToWidth: "تنظیم به اندازه عرض",
                fitToPage: "تنظیم نسبت به صفحه"
            },
            open: "باز کردن",
            exportAs: "خروجی",
            download: "دریافت",
            pager:  {
                first: "رفتن به صفحه اول",
                previous: "رفتن به صفحه قبل",
                next: "رفتن به صفحه بعدی",
                last: "رفتن به صفحه آخر",
                of: " از {0} ",
                page: "صفحه",
                pages: "صفحه"
            },
            print: "چاپ",
            toggleSelection: "فعال کردن انتخاب",
            togglePan: "فعال کردن به هم پیوستن",
            search: "جستجو"
        },
        errorMessages: {
            notSupported: "فقط pdf مجاز است.",
            parseError: "عدم امکان پردازش pdf.",
            notFound: "فایل یافت نشد.",
            popupBlocked: "پنجره پاپ آپ غرفعال است."
        },
        dialogs: {
            exportAsDialog: {
                title: "خروجی...",
                defaultFileName: "سند",
                pdf: "Portable Document Format (.pdf)",
                png: "Portable Network Graphics (.png)",
                svg: "Scalable Vector Graphics (.svg)",
                labels: {
                    fileName: "نام فایل",
                    saveAsType: "ذخیره بعنوان",
                    page: "صفحه"
                }
            },
            okText: "تایید",
            save: "ذخیره",
            cancel: "انصراف",
            search: {
                inputLabel: "متن جستجو",
                matchCase: "حساسیت به حروف کوچک و بزرگ",
                next: "گزینه بعدی",
                previous: "گزینه قبلی",
                close: "بستن",
                of: "از"
            }
        }
    });
}



})(window.kendo.jQuery);