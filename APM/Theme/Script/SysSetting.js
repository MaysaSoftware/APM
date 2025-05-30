var SysSettingType = "";
var NodeParentID = 0;
var NodeParentName = "";
var NodeFolder = "";
var NodeTitle = "";
var SelectedSysSettingMenu = "";
var OrderIndex = 0;
var SysSettingTreeView;
var SelectedNode;
var SysSettingID = 0;
var Node;
var NodeIsFolder = false;
var IsNewFolder = false;
var PublicSettingEntity = ["PublicSetting", "AdminSetting"];
var PublicSettingPersianName = ["عمومی", "تنظیمات مدیر سیستم"];

var Copy_CoreID = 0;
var Copy_CoreName = "";
var Copy_CoreEntity = "";
var Copy_CoreFolder = "";
var Copy_ParentId = 0;

var sourceNode;
var SourceType;
var SourceName;
var SourceID;
var SourceFolder;
var ParentSourceType;
var ParentSourceName ;
var ParentSourceID ;
var ParentSourceFolder ;
var SourceIndex;

$(document).on("keyup", "#SysSettingTreeFilter", function (e) {

    if (e.keyCode == 13 && !e.shiftKey) {
        $("#SysSettingTree span.k-in .k-treeview-leaf-text span").removeClass("highlight");

        var treeView = $("#SysSettingTree").getKendoTreeView();

        if ($.trim($(this).val()) == '') {
            return;
        }

        var term = this.value.toUpperCase();
        var tlen = term.length;

        $('#SysSettingTree span.k-in .k-treeview-leaf-text').each(function (index) {
            var text = $(this).text();
            var html = '';
            var q = 0;
            while ((p = text.toUpperCase().indexOf(term, q)) >= 0) {
                html += text.substring(q, p) + '<span class="highlight">' + text.substr(p, tlen) + '</span>';
                q = p + tlen;
            }

            if (q > 0) {
                html += text.substring(q);

                var dataItem = treeView.dataItem($(this));
                $(this).text("");
                $(this).append(html)

                $(this).parentsUntil('.k-treeview').filter('.k-item').each(

                    function (index, element) {
                        $('#SysSettingTree').data('kendoTreeView').expand($(this));
                        $(this).data('SysSettingTreeFilter', term);
                    });
            }
        });

        $('#SysSettingTree .k-item').each(function () {
            if ($(this).data('SysSettingTreeFilter') != term) {
                $('#SysSettingTree').data('kendoTreeView').collapse($(this));
            }
        });

    }
})

function SysSettingTree_DragStart(e) {
    var a = e;
    sourceNode = $("#SysSettingTree").getKendoTreeView().dataItem($(e.sourceNode).closest(".k-item"));
    ConfigNode(sourceNode);
    SourceType = SysSettingType;
    SourceName = NodeParentName;
    SourceID = $.isNumeric(sourceNode.id) ? sourceNode.id : NodeParentID;
    GetFolder(sourceNode);
    SourceFolder = NodeFolder;

    var ParentSourceNode = $("#SysSettingTree").getKendoTreeView().dataItem($(e.sourceNode.parentNode).closest(".k-item"));
    ConfigNode(ParentSourceNode);
    ParentSourceType = SysSettingType;
    ParentSourceName = NodeParentName;
    ParentSourceID = NodeParentID;
    ParentSourceFolder = NodeFolder;
    SourceIndex = FindNodeIndex(ParentSourceNode, sourceNode.text)
}

function SysSettingTree_DragEnd(e) {
    var a = e;
    var DestinationNode = $("#SysSettingTree").getKendoTreeView().dataItem($(e.sourceNode).closest(".k-item"));
    ConfigNode(DestinationNode);
    var DestinationType = SysSettingType;
    var DestinationName = NodeParentName;
    var DestinationID = NodeParentID;
    GetFolder(DestinationNode);
    var DestinationFolder = NodeFolder;



    var ParentDestinationNode = $("#SysSettingTree").getKendoTreeView().dataItem($(e.sourceNode.parentNode).closest(".k-item"));
    ConfigNode(ParentDestinationNode);
    var ParentDestinationType = SysSettingType;
    var ParentDestinationName = NodeParentName;
    var ParentDestinationID = NodeParentID;
    var ParentDestinationFolder = NodeFolder;
    var DestinationIndex = FindNodeIndex(ParentDestinationNode, DestinationNode.text)

    if (e.dropPosition == "after")
        DestinationIndex += 1;
    else (e.dropPosition == "before")
    DestinationIndex += 1;

    if (ParentDestinationID != ParentSourceID || SourceType != DestinationType || (SourceFolder == DestinationFolder && SourceType == DestinationType && e.dropPosition == "over")) {
        return;
    }
    else {
        if (sourceNode.id.indexOf("Folder") > -1) {
            $.ajax({
                type: 'POST',
                url: "/SysSetting/DragDroupFolderItem",
                data: {
                    ParentID: ParentSourceID,
                    Entity: SourceName,
                    SourceFolder: SourceFolder,
                    DestinationFolder: DestinationFolder,
                    Position: e.dropPosition
                },
                dataType: 'json',
                success: function (data) {
                    //var wnd = SysSettingDetail;
                    //wnd.close();
                    //ReloadSysSetting();
                }
            });
        }
        else {
            $.ajax({
                type: 'POST',
                url: "/SysSetting/DragDroupItem",
                data: {
                    ParentID: ParentSourceID,
                    Entity: SourceName,
                    Folder: DestinationFolder,
                    CoreID: SourceID,
                    DestinationIndex: DestinationIndex,
                    DropPosition:e.dropPosition
                },
                dataType: 'json',
                success: function (data) {
                }
            });
        }
    }
}

function ExpandAllNodes(TreeName) {
    var treeView = $("#" + TreeName).getKendoTreeView();
    treeView.expand(".k-item");
}

function CollapseAllNodes(TreeName) {
    var treeView = $("#" + TreeName).getKendoTreeView();
    treeView.collapse(".k-item");
}



function Select_SysSettingMenu(e) {
    let Title = $(e.item).text();
    var node = $("#SysSettingTree").getKendoTreeView().dataItem($(e.target).closest(".k-item"));
    SysSettingTreeView = $("#SysSettingTree").data("kendoTreeView");

    Node = node;
    IsNewFolder = false;

    SelectedNode = SysSettingTreeView.select();
    if (SelectedNode.length == 0) {
        SelectedNode = null;
    }

    ConfigNode(node);

    NodeIsFolder = false;

    OrderIndex = node.hasChildren == false ? OrderIndex : node.children.view().length + 1;
    SelectedSysSettingMenu = Title;

    if (node.id.indexOf("Bpmn") > -1 || node.id.indexOf("ProcessStep") > -1 || SysSettingType.indexOf("Bpmn") > -1) {
        popupNotification.show('امکان انجام عملیات روی آیتم انتخاب شده وجود ندارد', "error");
        return;
    }

    if (SysSettingType == "Field" && !(Title == "کلید اصلی" || Title == "پیشفرض" || Title == "ویرایش") && e.target.children[1].firstChild.className != undefined) {
        if (e.target.children[1].firstChild.className.indexOf("PrimaryKey") > -1) {
            popupNotification.show('امکان انجام عملیات روی کلید اصلی وجود ندارد', "error");
            return;
        } 
    }

    switch (Title) {
        case "جدید":
            OpenSysSettingDialog("", Title);
            break;
        case "ویرایش":
            if ($.isNumeric(node.id)) {
                SysSettingID = node.id;
                OpenSysSettingDetail(SysSettingID, node.text);
            }
            else if (node.id.indexOf("Folder") > -1) {
                var folderArr = node.id.split('_');
                SysSettingID = folderArr[folderArr.length - 1];
                if (SysSettingID != "0") {
                    SysSettingType = "Folder";
                    OpenSysSettingDetail(SysSettingID, node.text);
                }
                else { 
                    $.ajax({
                        type: 'POST',
                        url: "/SysSetting/SaveCoreObject",
                        data: {
                            'ParentID': NodeParentID, 
                            'Entity': 'Folder_' + SysSettingType,
                            'Folder': NodeFolder,
                            'FullName': NodeFolder.replace("/","_"),
                            'OrderIndex': OrderIndex
                        },
                        dataType: 'json',
                        success: function (result) {
                            if ($.isNumeric(result)) {
                                if (result > 0) {
                                    SysSettingType = "Folder";
                                    OpenSysSettingDetail(result, node.text);   
                                    SysSettingID = result; 
                                }
                                else {
                                    HideLoader();
                                    popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                                }
                            }
                            else {
                                HideLoader();
                                popupNotification.show(result, "error");
                            }
                        }
                    });
                }
            }
            break;
        case "حذف":
        case "حذف از هسته نرم افزار":
        case "حذف از سرور":
            if ($.isNumeric(node.id)) {
                SysSettingID = node.id;
                AlarmSysSettingDialog.title("حذف " + node.text).open();
            }
            else if (node.id.indexOf("Folder") > -1) {
                AlarmSysSettingDialog.title("حذف " + node.text).open();
            }
            break;
        case "تغییر نام":
            if ($.isNumeric(node.id)) { 
                SysSettingID = node.id;
                OpenSysSettingDialog(node.text, Title);
               
            }
            else if (node.id.indexOf("Folder") > -1) {
                SysSettingID = 0;
                NodeIsFolder = true;
                OpenSysSettingDialog(node.text, Title);
            }
            break;
        case "پیشفرض":
            {
                if ($.isNumeric(node.id)) {
                    ShowLoader();
                    SysSettingID = node.id;
                    $.ajax({
                        type: 'POST',
                        url: "/SysSetting/DefaultView",
                        data: {
                            'CoreObjectID': SysSettingID
                        },
                        dataType: 'json',
                        success: function (result) {
                            HideLoader();
                            if (result == true) {
                                popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                                var ChildrenLen = $(e.target).closest(".k-item")[0].children[0].children.length;
                                var ClassName = $(e.target).closest(".k-item")[0].children[0].children[ChildrenLen - 1].firstChild.className;
                                if (ClassName.indexOf("DefualtItem") > -1)
                                    ClassName = ClassName.replace("DefualtItem", "");
                                else
                                    ClassName += " DefualtItem";
                                $(e.target).closest(".k-item")[0].children[0].children[ChildrenLen - 1].firstChild.className = ClassName;
                            }
                            else
                                popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                        },
                        error: function (result) {
                            HideLoader();
                            popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                        }
                    });
                }
                break;
            }
        case "کلید اصلی":
            {
                if ($.isNumeric(node.id)) {
                    ShowLoader();
                    SysSettingID = node.id;

                    $.ajax({
                        type: 'POST',
                        url: "/SysSetting/SetPrimaryKey",
                        data: {
                            'CoreObjectID': SysSettingID
                        },
                        dataType: 'json',
                        success: function (result) {
                            HideLoader();
                            if (result == "") {
                                popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");

                                var ClassName = e.target.children[1].firstChild.className;
                                if (ClassName.indexOf("fa fa-key PrimaryKey") > -1)
                                    ClassName = ClassName.replace("fa fa-key PrimaryKey", "");
                                else
                                    ClassName += " fa fa-key PrimaryKey";
                                e.target.children[1].firstChild.className = ClassName;
                            }
                            else
                                popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                        },
                        error: function (result) {
                            HideLoader();
                            popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                        }
                    });
                }

                break;
            }
        case "پوشه جدید":
            IsNewFolder = true;
            NodeIsFolder = true;
            OpenSysSettingDialog("پوشه جدید");
            break;
        case "مشاهده":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "Table": {
                            ShowInformationEntryForm(node.id, "0", false, node.text, true);
                            break;
                        }
                        case "InformationEntryForm": {
                            ShowInformationEntryForm(node.id, "0", false, node.text, true);
                            break;
                        }
                        case "Report": {
                            ShowReport(node.id, node.text);
                            break;
                        }
                        case "PublicFile": {
                            ShowAttachmentForm(node.id, 0, 0);
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                else if (PublicSettingEntity.indexOf(node.id) > -1) {
                    SysSettingID = 0;
                    OpenSysSettingDetail(SysSettingID, "");
                }
                else if (node.id.indexOf("EventEditAll") > -1) {
                    ShowRegistryTable(2, "", "رویداد ویرایش", 0, 0)
                }
                else if (node.id.indexOf("EventInsertAll") > -1) {
                    ShowRegistryTable(1, "", "رویداد درج", 0, 0)
                }
                else if (node.id.indexOf("EventDeleteAll") > -1) {
                    ShowRegistryTable(0, "", "رویداد حذف", 0, 0)
                }
                else if (node.id.indexOf("EventDownloadTable") > -1) {
                    ShowRegistryTable(3, "", "رویداد بارگیری", 0, 0)
                }
                else if (node.id.indexOf("EventViewTable") > -1) {
                    ShowRegistryTable(4, "", "رویداد نمایش فرم", 0, 0)
                }
                else if (node.id.indexOf("UserLogin") > -1) {
                    ShowRegistryTable(5, "", "رویداد ورود به سیستم", 0, 0)
                }

                break;
            }
        case "خروجی اسکریپت براساس شناسه": {
                if (node.id == "APMCoreSetting") {
                    ShowAPMCoreSetting(0);
                }
                break;
        }
        case "خروجی اسکریپت براساس بازه زمانی": {
                if (node.id == "APMCoreSetting") {
                    var ScriptCoreDialog = $("#ScriptCoreDialog").data("kendoDialog");
                    ScriptCoreDialog.open();
                }
                break;
        }
        case "بروز رسانی": {
                if (node.id == "APMCoreSetting") {
                    ShowAPMCoreSetting(1);
                }
                break;
        }
        case "ایجاد ارتباط":
            {
                ShowLoader();
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "DataSource": {
                            $.ajax({
                                type: 'POST',
                                url: "/SysSetting/DataSourceCreateConnection",
                                data: {
                                    "DatabaseID": node.id
                                },
                                dataType: 'json',
                                success: function (message) {
                                    HideLoader();
                                    if (message == "") {
                                        popupNotification.show("ارتباط با موفقیت برقرار شد", "success")
                                    }
                                    else
                                        popupNotification.show(message, "error");
                                },
                                error: function (result) {
                                    HideLoader();
                                    popupNotification.show(result, "error");
                                }
                            });
                        }
                        default: break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
        case "بررسی برقراری ارتباط":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "DataSource": {
                            ShowLoader();

                            $.ajax({
                                type: 'POST',
                                url: "/SysSetting/DataSourceTestConnection",
                                data: {
                                    "DatabaseID": node.id
                                },
                                dataType: 'json',
                                success: function (message) {
                                    HideLoader();
                                    if (message == "") {
                                        popupNotification.show("ارتباط با موفقیت برقرار شد", "success");
                                    }
                                    else
                                        popupNotification.show(message, "error");
                                },
                                error: function (result) {
                                    HideLoader();
                                    popupNotification.show(result, "error");
                                }
                            });
                        }
                        default:
                            HideLoader();
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
            break; 
        case "بازسازی":
            {
                if ($.isNumeric(node.id)) {
                    if (SysSettingType == "DataSource" || SysSettingType == "TableFunction") { 
                        ShowLoader();

                        $.ajax({
                            type: 'POST',
                            url: "/SysSetting/Rebuilding",
                            data: {
                                "BaseCoreID": node.id
                            },
                            dataType: 'json',
                            success: function (Result) {
                                HideLoader();
                                if (Result.Error == "") {
                                    popupNotification.show("بازسازی با موفقیت برقرار شد", "success");
                                    popupNotification.show(Result.AlarmRebuilding.replace(/\n/g, '<br/>'), "info");  
                                    ReloadSysSetting();
                                }
                                else
                                    popupNotification.show(Result.Error, "error");
                            },
                            error: function (result) {
                                HideLoader();
                                popupNotification.show(result, "error");
                            }
                        });
                    } 
                    SysSettingID = node.id;
                }
                break;
            }
            break;
        case "بازسازی فیلد":
            {
                if($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "InformationEntryForm": {
                            ShowLoader();

                            $.ajax({
                                type: 'POST',
                                url: "/SysSetting/InformationEntryFormFieldRebuilding",
                                data: {
                                    "InformationEntryFormID": node.id
                                },
                                dataType: 'json',
                                success: function (Result) {
                                    HideLoader();
                                    if (Result.Error == "") {
                                        popupNotification.show("بازسازی با موفقیت برقرار شد", "success");
                                        ReloadSysSetting();
                                    }
                                    else
                                        popupNotification.show(Result.Error, "error");
                                },
                                error: function (result) {
                                    HideLoader();
                                    popupNotification.show(result, "error");
                                }
                            });
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
            break;
        case "کپی":
            {
                if ($.isNumeric(node.id) || node.id.indexOf("Folder") > -1) {
                    Copy_CoreID = SysSettingID;
                    Copy_ParentId = NodeParentID;
                    Copy_CoreFolder = NodeFolder;
                    Copy_CoreEntity = SysSettingType;
                    Copy_CoreName = node.text;
                }
                else {
                    Copy_CoreID = 0;
                    Copy_ParentId = 0;
                    Copy_CoreFolder = "";
                    Copy_CoreEntity = "";
                    Copy_CoreName = "";
                }
            }
            break;
        case "جایگذاری":
            {
                if (Copy_CoreEntity != "") {
                    if (Copy_CoreEntity != SysSettingType) {
                        popupNotification.show('مکان جایگذاری صحیح نمی باشد', "error");
                    }
                    else {
                        ShowLoader();
                        $.ajax({
                            type: 'POST',
                            url: "/SysSetting/CopyCoreObject",
                            data: {
                                'Copy_CoreID': Copy_CoreID,
                                'Copy_ParentId': Copy_ParentId,
                                'Copy_CoreFolder': Copy_CoreFolder,
                                'Copy_CoreEntity': Copy_CoreEntity,
                                'Copy_CoreName': Copy_CoreName,
                                'CoreID': SysSettingID,
                                'ParentId': NodeParentID,
                                'CoreFolder': NodeFolder,
                                'CoreEntity': SysSettingType,
                                'CoreName': node.text,
                            },
                            dataType: 'json',
                            success: function (result) {
                                HideLoader();
                                if (result > 0) {
                                    popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                                    ReloadSysSetting();
                                }
                                else
                                    popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                            },
                            error: function (result) {
                                HideLoader();
                                popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                            }
                        });
                    }
                }
                else {
                    popupNotification.show('آیتمی انتخاب نشده است', "error");
                }
                break;
            }
            break;
        case "بازیابی سطر حذف شده":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "Table": {
                            ShowRegistryTable(0, node.text, node.text, node.id, 0)
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
        case "رویداد ویرایش":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "Table": {
                            ShowRegistryTable(2, node.text, node.text, node.id, 0)
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
        case "رویداد درج":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "Table": {
                            ShowRegistryTable(1, node.text, node.text, node.id, 0)
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
        case "رویداد بارگیری":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "Table": {
                            ShowRegistryTable(3, node.text, node.text, node.id, 0)
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
        case "رویداد مشاهده":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "Table": {
                            ShowRegistryTable(4, node.text, node.text, node.id, 0)
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;
            }
        case "ایجاد نسخه پشتیبان":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "DataSource": {
                            window.location = '/SysSetting/CreateBackup?DatabaseID=' + node.id;
                            break;
                        }
                        default:
                            break;
                    }
                    SysSettingID = node.id;
                }
                else if (node.id == "APMCoreSetting") {
                   window.location = '/SysSetting/CreateBackupCoreDatabase'; 
                }
                break;
            }
        case "بارگذاری داده":
            { 
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "TableAttachment": { 
                            var UploadTableAttachmenwnd = UploadTableAttachmentWin;
                            UploadTableAttachmenwnd.content("<div id='UploadTableAttachmentWinDiv'></div>");
                            NodeTitle = Title;
                            $("#UploadTableAttachmentWinDiv").load("/SysSetting/GetCoreObjectTreeView",
                                {
                                    "Entity": "Field",
                                    "ParentID": NodeParentID
                                },
                                function () {
 
                                });

                            var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                            var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                            UploadTableAttachmenwnd.setOptions({
                                width: newWidth - 30,
                                height: newHeight - 50
                            });

                            UploadTableAttachmenwnd.title(Title + " " + NodeParentName + " " + node.text);
                            UploadTableAttachmenwnd.center();
                            UploadTableAttachmenwnd.open();
                            break;
                        } 
                        default: popupNotification.show('این آیتم در حال بروز رسانی می باشد', "info");
                            break;
                    }
                    SysSettingID = node.id;
                }
                break; 
            }
        case "بارگیری داده":
            {
                if ($.isNumeric(node.id)) {
                    switch (SysSettingType) {
                        case "TableAttachment": {
                            window.location = '/SysSetting/CreateBackup?DatabaseID=' + node.id;
                            break;
                        }
                        case "Table": {
                            window.location = '/SysSetting/CreateBackup?DatabaseID=' + node.id;
                            break;
                        }
                        default: popupNotification.show('این آیتم در حال بروز رسانی می باشد', "info");
                            break;
                    }
                    SysSettingID = node.id;
                }
                break;

            }            
        case "خروجی اسکریپت":
            {
                if ($.isNumeric(node.id)) {
                    window.location = '/SysSetting/CreateScriptCoreWithID?CoreID=' + node.id;
                }
                break;

            }
            
        default: break;
    }
}



function ShowInformationEntryForm(FormID, ParentID, IsProcess, Title, ShowWithOutPermissionConfig) {
    ShowLoader();

    if (ShowWithOutPermissionConfig == undefined)
        ShowWithOutPermissionConfig = false

    $.ajax({
        url: "/Home/InformationEntryForm",
        data: {
            Form: FormID,
            ParentID: ParentID,
            IsProcess: IsProcess,
            ShowWithOutPermissionConfig: ShowWithOutPermissionConfig
        },
        type: "POST",
        success: function (Result) {
            var wnd = DataTableWin;
            wnd.content(Result);
            var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
            var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

            wnd.setOptions({
                width: newWidth - 30,
                height: newHeight - 50
            });

            wnd.title(Title);
            wnd.center();
            wnd.open();
            HideLoader();
        },
        error: function (result) {
            HideLoader();
        }
    })
}


function ConfigNode(node) {
    SysSettingID = 0;
    NodeParentID = 0;
    NodeFolder = "";
    OrderIndex = 0;
    GetParentNode(node);
    if (node != undefined)
        if ($.isNumeric(node.id)) {
            SysSettingID = node.id;
        }
    GetParentNodeID(node);
}

function FindNodeIndex(ParentNode, NodeName) {
    if (ParentNode.children.view() != undefined) {
        var NodeChildren = ParentNode.children.view();
        for (var index = 0; index < NodeChildren.length; index++) {
            if (NodeChildren[index].text == NodeName) {
                return index;
            }
        }
    }
    return 0;
}

function GetParentNode(node) {
    var Parent = "";
    if (node != undefined)
        if (node.parent() != undefined) {
            if (node.id != "" && node.id != undefined && !$.isNumeric(node.id)) {
                if (node.id.indexOf("DataSource") > -1) {
                    SysSettingType = Parent = "DataSource";
                    NodeParentName = "پایگاه داده";
                }
                else if (node.id.indexOf("WebServiceParameter") > -1) {
                    SysSettingType = Parent = "WebServiceParameter";
                    NodeParentName = "پارامتر وب سرویس";
                }
                else if (node.id == "GroupField") {
                    SysSettingType = Parent = "GroupField";
                    NodeParentName = "فیلد_گروهبندی";
                }
                else if (node.id == "BpmnIncoming") {
                    SysSettingType = Parent = "BpmnIncoming";
                    NodeParentName = "ورودی فرآیند";
                }
                else if (node.id == "GridRowColor") {
                    SysSettingType = Parent = "GridRowColor";
                    NodeParentName = "رنگ سطر جدول";
                }
                else if (node.id == "BpmnOutgoin") {
                    SysSettingType = Parent = "BpmnOutgoin";
                    NodeParentName = "خروجی فرآیند";
                }
                else if (node.id == "BpmnLane") {
                    SysSettingType = Parent = "BpmnLane";
                    NodeParentName = "مسیر فرآیند";
                }
                else if (node.id == "WebService") {
                    SysSettingType = Parent = "WebService";
                    NodeParentName = "وب سرویس";
                }
                else if (node.id.indexOf("TableAttachment") > -1) {
                    SysSettingType = Parent = "TableAttachment";
                    NodeParentName = "ضمیمه";
                }
                else if (node.id.indexOf("NewButtonForm") > -1) {
                    SysSettingType = Parent = "NewButtonForm";
                    NodeParentName = "فرم دکمه جدید";
                }
                else if (node.id.indexOf("ParameterTableFunction") > -1) {
                    SysSettingType = Parent = "ParameterTableFunction";
                    NodeParentName = "پارامتر تابع";
                }
                else if (node.id.indexOf("TableFunction") > -1) {
                    SysSettingType = Parent = "TableFunction";
                    NodeParentName = "تابع";
                }
                else if (node.id.indexOf("SearchField") > -1) {
                    SysSettingType = Parent = "SearchField";
                    NodeParentName = "فیلد جستجو";
                }
                else if (node.id.indexOf("SearchForm") > -1) {
                    SysSettingType = Parent = "SearchForm";
                    NodeParentName = "فرم جستجو";
                }
                else if (node.id == "TableEvent") {
                    SysSettingType = Parent = "TableEvent";
                    NodeParentName = "رویداد جدول";
                }
                else if (node.id == "TableButton") {
                    SysSettingType = Parent = "TableButton";
                    NodeParentName = "دکمه";
                }
                else if (node.id == "Table") {
                    SysSettingType = Parent = "Table";
                    NodeParentName = "جدول";
                }
                else if (node.id == "PublicJob") {
                    SysSettingType = Parent = "PublicJob";
                    NodeParentName = "رویداد";
                }
                else if (node.id == "PublicFile") {
                    SysSettingType = Parent = "PublicFile";
                    NodeParentName = "فایل عمومی";
                }
                else if (node.id.indexOf("InformationEntryForm") > -1) {
                    SysSettingType = Parent = "InformationEntryForm";
                    NodeParentName = "فرم";
                }
                else if (node.id == "ProcessStepEvent") {
                    SysSettingType = Parent = "ProcessStepEvent";
                    NodeParentName = "رویداد مرحله";
                }
                else if (node.id == "ProcessStep") {
                    SysSettingType = Parent = "ProcessStep";
                    NodeParentName = "مرحله فرآیند";
                }
                else if (node.id == "ProcessReferral") {
                    SysSettingType = Parent = "ProcessReferral";
                    NodeParentName = "ارجاع مرحله";
                }
                else if (node.id.indexOf("Process") > -1) {
                    SysSettingType = Parent = "Process";
                    NodeParentName = "فرآیند";
                }
                else if (node.id == "Report") {
                    SysSettingType = Parent = "Report";
                    NodeParentName = "گزارش";
                }
                else if (node.id == "ReportParameter") {
                    SysSettingType = Parent = "ReportParameter";
                    NodeParentName = "پارامتر گزارش";
                }
                else if (node.id.indexOf("DisplayField") > -1) {
                    SysSettingType = Parent = "DisplayField";
                    NodeParentName = "فیلد نمایشی";
                }
                else if (node.id.indexOf("ComputationalField") > -1) {
                    SysSettingType = Parent = "ComputationalField";
                    NodeParentName = "فیلد محاسباتی";
                }
                else if (node.id.indexOf("ShowFieldEvent") > -1) {
                    SysSettingType = Parent = "ShowFieldEvent";
                    NodeParentName = "رویداد نمایش فیلد";
                }
                else if (node.id.indexOf("Field") > -1) {
                    SysSettingType = Parent = "Field";
                    NodeParentName = "فیلد";
                }
                else if (node.id.indexOf("SpecialPhrase") > -1) {
                    SysSettingType = Parent = "SpecialPhrase";
                    NodeParentName = "عبارت ویژه";
                }
                else if (node.id == "SubDashboard") {
                    SysSettingType = Parent = "SubDashboard";
                    NodeParentName = "زیر بخش داشبورد";
                }
                else if (node.id == "DashboardIntegration") {
                    SysSettingType = Parent = "DashboardIntegration";
                    NodeParentName = "ادغام داشبورد";
                }
                else if (node.id == "Dashboard") {
                    SysSettingType = Parent = "Dashboard";
                    NodeParentName = "داشبورد";
                }
                else if (PublicSettingEntity.indexOf(node.id) > -1) {
                    SysSettingType = Parent = node.id;
                    NodeParentName = PublicSettingPersianName[PublicSettingEntity.indexOf(node.id)];
                }
            }
            if (Parent == "")
                GetParentNode(node.parent())
        }
}

function GetParentNodeID(node) {
    if (node != undefined)
    if (node.parent() != undefined) {
        if (node.id != undefined) {
            if ($.isNumeric(node.id) && SysSettingID != node.id) {
                NodeParentID = node.id;
            }
            else if (node.id.indexOf("Folder") > -1) {
                NodeFolder = NodeFolder == "" ? node.text : node.text + "/" + NodeFolder;
                var NodeFolderSplit = node.id.replace(SysSettingType+"Folder","").split('_');
                if (NodeFolderSplit.length > 0) {
                    if ($.isNumeric(NodeFolderSplit[0]))
                        NodeParentID = NodeFolderSplit[0];
                }
            }
        }

        if (NodeParentID == 0)
            GetParentNodeID(node.parent())
    }
}

function GetFolder(node,IsReset) {
    var IsFind = false;
    if (node.parent() != undefined) {
        if (node.id != undefined) {
            if (node.id.indexOf("Folder") > -1) {
                if (IsReset == true) {

                }
                    
                NodeFolder = NodeFolder == "" ? node.text : NodeFolder;
                IsFind = true;
            }
        }

        if (IsFind == false)
            GetFolder(node.parent())
    }
}

function SaveSysSettingDetails() {
    ShowLoader();
    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("SysSettingForm", FormInputName, FormInputValue);

    $.ajax({
        type: 'POST',
        url: "/SysSetting/SaveSysSettingDetail",
        data: {
            "FormInputName": FormInputName,
            "FormInputValue": FormInputValue,
            "SysSettingID": $("#SysSettingID").text()
        },
        dataType: 'json',
        success: function (data) {
            if ($("#ShowInNewWindowID").length > 0)
                window.close();
            else {
                var wnd = SysSettingDetail;
                wnd.close();
                ReloadSysSetting();
                HideLoader();
            }
        },
        error: function (data) {
            HideLoader();
            popupNotification.show('ذخیره سازی با خطا مواجه شد', "error");
        }
    });
}

function ReloadSysSetting() {

    ShowLoader();
    $.ajax({
        url: "/Home/SysSetting",
        dataType: 'json',
        type: "POST",
        success: function (Result) {
            $("#MainSysSettingTreeForm")[0].innerHTML = "";
            $("#MainSysSettingTreeForm").append(Result);
            HideLoader();

        },
        error: function (result) {
            $("#MainSysSettingTreeForm")[0].innerHTML = "";
            $("#MainSysSettingTreeForm").append(result.responseText);
            HideLoader();
            if (SysSettingID > 0) {
                var treeView = $("#SysSettingTree").getKendoTreeView();
                $('#SysSettingTree li').each(function (index) {
                    var dataItem = treeView.dataItem($(this));
                    if (dataItem.id == SysSettingID) {
                        $(this).parentsUntil('.k-treeview').filter('.k-item').each(

                            function (index, element) {
                                $('#SysSettingTree').data('kendoTreeView').expand($(this));
                            });

                        var selectitem = treeView.findByUid(dataItem.uid);
                        treeView.select(selectitem);
                    }
                });
            }
        }
    });
}

function OpenMenu_SysSettingMenu(e) {
    $("#" + e.sender.element[0].id + " .k-i-arrow-60-right").addClass("k-i-arrow-60-left LeftArrowIcon");
}

function CanselSysSettingDetails() {
    if ($("#ShowInNewWindowID").length>0)
        window.close();
    else {
        var wnd = SysSettingDetail;
        wnd.close();
    }
}

function SaveDialog() {
    if ($("#SysSettingTitle")[0].value != "") {

        ShowLoader();
        if (NodeIsFolder) {
            if (IsNewFolder) {


                $.ajax({
                    type: 'POST',
                    url: "/SysSetting/SaveCoreObject",
                    data: {
                        'ParentID': NodeParentID,
                        'Entity': 'Folder_' + SysSettingType,
                        'Folder': (NodeFolder != "" ? NodeFolder + "/" : "")+$("#SysSettingTitle")[0].value,
                        'FullName': NodeFolder + "_" + $("#SysSettingTitle")[0].value,
                        'OrderIndex': OrderIndex
                    },
                    dataType: 'json',
                    success: function (result) {
                        if ($.isNumeric(result)) {
                            if (result > 0) {
                                SysSettingType = 'Folder';

                                HideLoader();
                                popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");

                                SysSettingTreeView.append({
                                    text: $("#SysSettingTitle")[0].value,
                                    id: SysSettingType + "_Folder" + $("#SysSettingTitle")[0].value.replaceAll(" ","_") + result,
                                    sprite: "k-icon k-i-folder"
                                }, SelectedNode);

                                OpenSysSettingDetail(result, $("#SysSettingTitle")[0].value)
                                SysSettingID = result;
                                //ReloadSysSetting();
                                HideLoader();
                            }
                            else {
                                HideLoader();
                                popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                            }
                        }
                        else {
                            HideLoader();
                            popupNotification.show(result, "error");
                        }
                    }
                });


            }
            //else if (Node.hasChildren == false) { 
            //    Node.set("text", $("#SysSettingTitle")[0].value);
            //    Node.set("sprite", Node.spriteCssClass);
            //}

            else {
                $.ajax({
                    type: 'POST',
                    url: "/SysSetting/RenameFolder",
                    data: {
                        'ParentID': NodeParentID,
                        'Entity': SysSettingType,
                        'OldFolder': NodeFolder,
                        'NewFolder': $("#SysSettingTitle")[0].value
                    },
                    dataType: 'json',
                    success: function (result) {
                        if (result > 0) {
                            popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                            Node.set("text", $("#SysSettingTitle")[0].value);
                            Node.set("sprite", Node.spriteCssClass);

                        }
                        else
                            popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                    }
                });
            }

            HideLoader();
        }
        else if (SysSettingID == 0) {
            $.ajax({
                type: 'POST',
                url: "/SysSetting/SaveCoreObject",
                data: {
                    'ParentID': NodeParentID,
                    'Entity': SysSettingType,
                    'Folder': NodeFolder,
                    'FullName': $("#SysSettingTitle")[0].value,
                    'OrderIndex': OrderIndex
                },
                dataType: 'json',
                success: function (result) {
                    if ($.isNumeric(result)) {
                        if (result > 0) {
                            HideLoader();
                            popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");

                            SysSettingTreeView.append({
                                text: $("#SysSettingTitle")[0].value,
                                id: result
                            }, SelectedNode);

                            OpenSysSettingDetail(result, $("#SysSettingTitle")[0].value)
                            SysSettingID = result;
                            ReloadSysSetting();
                            HideLoader();
                        }
                        else {
                            HideLoader();
                            popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                        }
                    }
                    else {
                        HideLoader();
                        popupNotification.show(result, "error");
                    }
                }
            });
        }
        else {
            $.ajax({
                type: 'POST',
                url: "/SysSetting/RenameCore",
                data: {
                    'CoreObjectID': SysSettingID,
                    'FullName': $("#SysSettingTitle")[0].value
                },
                dataType: 'json',
                success: function (result) {
                    if (result == true) {
                        HideLoader();
                        popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                        Node.set("text", $("#SysSettingTitle")[0].value);
                        Node.set("sprite", Node.spriteCssClass);
                    }
                    else {
                        HideLoader();
                        popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                    }
                }
            });
        }

    }
}

function OpenSysSettingDetail(SysSettingID, Title) {
    if (SysSettingType == "PublicFile") {
        ShowAttachmentForm(SysSettingID, 0, 0);
    }
    //else if (SysSettingType == "Process") {
    //    PopupMax("/BPMN?ProcessID=" + SysSettingID, SysSettingDetail.title());
    //}
    else {
        var wnd = SysSettingDetail;
        wnd.content("<div id='SysSettingDetailDiv'></div>");
        NodeTitle = Title;
        $("#SysSettingDetailDiv").load("/SysSetting/LoadSysSettingDetail",
            {
                "SysSettingType": SysSettingType,
                "ID" : SysSettingID
            },
            function () {
                if (SysSettingType == "Field") {
                    FieldTypeSelected($("#FieldType").val());
                    $("#FieldType").change(function () {
                        FieldTypeSelected(this.value);
                    });
                }
                else if (SysSettingType == "AdminSetting") {

                    var ShowEditingRestrictionsElement = $("#ShowEditingRestrictions").data("kendoSwitch");
                    ShowEditingRestrictions(ShowEditingRestrictionsElement.element[0].checked);
                    ShowEditingRestrictionsElement.bind('change', function (e) {
                        ShowEditingRestrictions(ShowEditingRestrictionsElement.element[0].checked);
                    });
                    
                    var PermissionShowImportExportInFormElement = $("#PermissionShowImportExportInForm").data("kendoSwitch");
                    PermissionShowImportExportInForm(PermissionShowImportExportInFormElement.element[0].checked);
                    PermissionShowImportExportInFormElement.bind('change', function (e) {
                        PermissionShowImportExportInForm(PermissionShowImportExportInFormElement.element[0].checked);
                    });

                    var PermissionShowCommentInFormElement = $("#PermissionShowCommentInForm").data("kendoSwitch");
                    PermissionShowCommentInForm(PermissionShowCommentInFormElement.element[0].checked);
                    PermissionShowCommentInFormElement.bind('change', function (e) {
                        PermissionShowCommentInForm(PermissionShowCommentInFormElement.element[0].checked);
                    }); 

                    var PermissionShowEmailInFormElement = $("#PermissionShowEmailInForm").data("kendoSwitch");
                    PermissionShowEmailInForm(PermissionShowEmailInFormElement.element[0].checked);
                    PermissionShowEmailInFormElement.bind('change', function (e) {
                        PermissionShowEmailInForm(PermissionShowEmailInFormElement.element[0].checked);
                    }); 
                }
                else if (SysSettingType == "InformationEntryForm" && NodeParentID == 0) {
                    $("#InputDiv_ExternalField").hide();
                    $("#InputDiv_Height").hide();
                    $("#InputDiv_ShowInParentForm").hide();
                    $("#InputDiv_SearchWithOnkeyDown").hide();
                    $("#InputDiv_SearchWithOnkeyDownCoreId").hide();
                }
                else if (SysSettingType == "InformationEntryForm" && NodeParentID > 0) {
                    $("#InputDiv_GroupableField").hide();
                }
                else if (SysSettingType == "TableEvent") {
                    TypeEventExecution($("#TypeEventExecution").val());
                    $("#TypeEventExecution").change(function () {
                        TypeEventExecution(this.value);
                    });

                    var Element = $("#UsePublickEmail").data("kendoSwitch");
                    UsePublickEmail(Element.element[0].checked);
                    Element.bind('change', function (e) {
                        UsePublickEmail(Element.element[0].checked); 
                    });
                     
                }
                else if (SysSettingType == "NewButtonForm") {  
                    var Element = $("#UseUrl").data("kendoSwitch");
                    UseUrlNewButtonForm(Element.element[0].checked);
                    Element.bind('change', function (e) {
                        UseUrlNewButtonForm(Element.element[0].checked);
                    });
                     
                }
        });

        var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        wnd.setOptions({
            width: newWidth - 30,
            height: newHeight - 50
        });

        wnd.title("تنظیمات " + NodeParentName + " " + Title);
        wnd.center();
        wnd.open();
    }
}

function UseUrlNewButtonForm(Checked) {

    $("#InputDiv_RelatedInformationForm").hide();
    $("#InputDiv_Url").hide();

    if (Checked == true)
        $("#InputDiv_Url").show(); 
    else
        $("#InputDiv_RelatedInformationForm").show();

}

function ShowEditingRestrictions(Checked) {

    $("#InputDiv_ShowCanUpdateOnlyUserRegistry").hide();
    $("#InputDiv_ShowCanUpdateOneDey").hide();
    $("#InputDiv_ShowCanUpdateThreeDey").hide();
    $("#InputDiv_ShowCanUpdateOneWeek").hide();

    if (Checked == true) {
        $("#InputDiv_ShowCanUpdateOnlyUserRegistry").show();
        $("#InputDiv_ShowCanUpdateOneDey").show();
        $("#InputDiv_ShowCanUpdateThreeDey").show();
        $("#InputDiv_ShowCanUpdateOneWeek").show();
    }
}


function PermissionShowCommentInForm(Checked) {

    $("#InputDiv_ShowCommentInAllForm").hide();
    $("#InputDiv_AllowFormShowComment").hide();

    if (Checked == true) {
        $("#InputDiv_ShowCommentInAllForm").show();
        $("#InputDiv_AllowFormShowComment").show();

        var ShowCommentInAllFormElement = $("#ShowCommentInAllForm").data("kendoSwitch");
        ShowCommentInAllForm(ShowCommentInAllFormElement.element[0].checked);
        ShowCommentInAllFormElement.bind('change', function (e) {
            ShowCommentInAllForm(ShowCommentInAllFormElement.element[0].checked);
        });

    }
}

function ShowCommentInAllForm(Checked) {
    $("#InputDiv_AllowFormShowComment").hide();

    if (Checked == false) {
        $("#InputDiv_AllowFormShowComment").show();
    }
}

function PermissionShowEmailInForm(Checked) {

    $("#InputDiv_ShowEmailInAllForm").hide();
    $("#InputDiv_AllowFormShowEmail").hide();

    if (Checked == true) {
        $("#InputDiv_ShowEmailInAllForm").show();
        $("#InputDiv_AllowFormShowEmail").show();

        var ShowEmailInAllFormElement = $("#ShowEmailInAllForm").data("kendoSwitch");
        ShowEmailInAllForm(ShowEmailInAllFormElement.element[0].checked);
        ShowEmailInAllFormElement.bind('change', function (e) {
            ShowEmailInAllForm(ShowEmailInAllFormElement.element[0].checked);
        });

    }
}

function ShowEmailInAllForm(Checked) {
    $("#InputDiv_AllowFormShowEmail").hide();

    if (Checked == false) {
        $("#InputDiv_AllowFormShowEmail").show();
    }
}

function PermissionShowImportExportInForm(Checked) {

    $("#InputDiv_ShowImportExportInAllForm").hide();
    $("#InputDiv_AllowFormShowImportExport").hide();

    if (Checked == true) {
        $("#InputDiv_ShowImportExportInAllForm").show();
        $("#InputDiv_AllowFormShowImportExport").show();

        var Element = $("#ShowImportExportInAllForm").data("kendoSwitch");
        ShowImportExportInAllForm(Element.element[0].checked);
        Element.bind('change', function (e) {
            ShowImportExportInAllForm(Element.element[0].checked);
        });

    }
}

function ShowImportExportInAllForm(Checked) {
    $("#InputDiv_AllowFormShowImportExport").hide();

    if (Checked == false) {
        $("#InputDiv_AllowFormShowImportExport").show();
    }
}


function UsePublickEmail(Checked) {

    $("#InputDiv_EMail").hide();
    $("#InputDiv_EMailUserName").hide();
    $("#InputDiv_EMailPassWord").hide();
    $("#InputDiv_EMailServer").hide();
    $("#InputDiv_EMailPort").hide();
    $("#InputDiv_EnableSsl").hide();

    if (Checked == false) {
        $("#InputDiv_EMail").show();
        $("#InputDiv_EMailUserName").show();
        $("#InputDiv_EMailPassWord").show();
        $("#InputDiv_EMailServer").show();
        $("#InputDiv_EMailPort").show();
        $("#InputDiv_EnableSsl").show();
    }
}

function TypeEventExecution(Value) {
    //$("#InputDiv_Condition").hide();
    $("#InputDiv_Query").hide();
    $("#MainBlockDiv_0_تنظیمات_ایمیل").hide();
    $("#MainBlockDiv_0_تنظیمات_ارجاع").hide();
    $("#InputDiv_RelatedTable").hide();
    $("#InputDiv_RelatedWebService").hide();

    switch (Value) {
        case "اجرای_کوئری": {
            $("#InputDiv_Query").show();
            $("#InputDiv_RelatedTable").show();
            break;
        }
        case "ارسال_ایمیل": {
            $("#InputDiv_Condition").show();
            $("#MainBlockDiv_0_تنظیمات_ایمیل").show();
            break;
        }
        case "اجرای_وب_سرویس": {
            $("#InputDiv_Condition").show();
            $("#InputDiv_RelatedWebService").show();
            break;
        }
        case "ارجاع": {
            $("#InputDiv_Condition").show();
            $("#MainBlockDiv_0_تنظیمات_ارجاع").show();
            break;
        }
    }
    
}


function FieldTypeSelected(Value) {

    $("#InputDiv_RelatedTable").hide();
    $("#InputDiv_ViewCommand").hide();
    $("#InputDiv_DigitsAfterDecimal").hide();
    $("#InputDiv_MaxValue").hide();
    $("#InputDiv_MinValue").hide();
    $("#InputDiv_SearchAutoCompleteCount").hide();
    $("#InputDiv_SpecialValue").hide();

    switch (Value) {
        case "FillTextAutoComplete":
            {
                $("#InputDiv_SearchAutoCompleteCount").show();
                break;
            }
        case "MultiSelectFromComboBox":
        case "TwoValues":
        case "ComboBox":
            {
                $("#InputDiv_SpecialValue").show();
                break;
            }
        case "RelatedTable":
            {
                $("#InputDiv_RelatedTable").show();
                $("#InputDiv_ViewCommand").show();
                $("#InputDiv_SearchAutoCompleteCount").show();
                break;
            }
        case "MultiSelectFromRelatedTable":
            {
                $("#InputDiv_RelatedTable").show();
                $("#InputDiv_ViewCommand").show();
                $("#InputDiv_SearchAutoCompleteCount").show();
                break;
            }
        case "Sparkline":
        case "Rating":
            {
                $("#InputDiv_MaxValue").show();
                $("#InputDiv_MinValue").show();

            }
        case "Number":
            {
                $("#InputDiv_DigitsAfterDecimal").show();
                $("#InputDiv_MaxValue").show();
                $("#InputDiv_MinValue").show();
                break;
            } 
    }
}

function OpenSysSettingDialog(Title, ActionType) {
    var TitleName = Title.replace("*", "").replace(Title.substring(Title.indexOf("("), Title.indexOf(")") + 1), "");
    if (ActionType == undefined)
        ActionType = "";
    else if (ActionType == "تغییر نام")
        ActionType = ActionType + " " + NodeParentName + " " + TitleName;
    else
        ActionType = (NodeIsFolder == true ? "پوشه" : NodeParentName + " جدید")
    SysSettingDialog.title(ActionType).open();
    $("#SysSettingTitle")[0].value = TitleName;
    $("#SysSettingTitle").focus();
    $("#SysSettingTitle").select();
}

function DeleteCore() {
    ShowLoader();
    if ($.isNumeric(Node.id)) {
        $.ajax({
            type: 'POST',
            url: "/SysSetting/DeleteCore",
            data: {
                'CoreObjectID': SysSettingID,
                'DeleteType': SelectedSysSettingMenu
            },
            dataType: 'json',
            success: function (result) {
                if (result == true) {
                    HideLoader();
                    popupNotification.show('عملیات حذف با موفقیت انجام شد', "success");
                    SysSettingTreeView.remove(SelectedNode);

                }
                else {
                    HideLoader();
                    popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                }
            }
        });
    }
    else if (Node.id.indexOf("Folder") > -1) {
        $.ajax({
            type: 'POST',
            url: "/SysSetting/DeleteFolderCore",
            data: {
                'ParentID': NodeParentID,
                'Entity': SysSettingType,
                'Folder': NodeFolder,
                'DeleteType': SelectedSysSettingMenu
            },
            dataType: 'json',
            success: function (result) {
                if (result == true) {
                    HideLoader();
                    popupNotification.show('عملیات حذف با موفقیت انجام شد', "success");
                    SysSettingTreeView.remove(SelectedNode);

                }
                else {
                    HideLoader();
                    popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
                }
            }
        });
    }


}

function DataSourceTestConnectionClick() {
    ShowLoader();
    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("SysSettingForm", FormInputName, FormInputValue);

    $.ajax({
        type: 'POST',
        url: "/SysSetting/DataSourceTestConnectionForm",
        data: {
            "FormInputName": FormInputName,
            "FormInputValue": FormInputValue
        },
        dataType: 'json',
        success: function (message) {
            HideLoader();
            if (message.Error == "") {
                popupNotification.show("ارتباط با موفقیت برقرار شد", "success");
                $("#FilePath")[0].value = message.FilePath;
            }
            else
                popupNotification.show(message.Error, "error");
        },
        error: function (result) {
            HideLoader();
            popupNotification.show(result, "error");
        }
    });

}

function DataSourceCreateConnectionClick() {
    ShowLoader();
    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("SysSettingForm", FormInputName, FormInputValue);

    $.ajax({
        type: 'POST',
        url: "/SysSetting/DataSourceCreateConnectionForm",
        data: {
            "FormInputName": FormInputName,
            "FormInputValue": FormInputValue
        },
        dataType: 'json',
        success: function (Result) {
            HideLoader();
            if (Result.Error == "") {
                popupNotification.show("ارتباط با موفقیت برقرار شد", "success")
                $("#FilePath").val(Result.FilePath);
                $("#DataBase").val(Result.Database);
            }
            else
                popupNotification.show(Result.Error, "error");
        },
        error: function (result) {
            HideLoader();
            popupNotification.show(result, "error");
        }
    });

}

function ReportDesignClick() {
    //location.href = "/Report/Designer?ReportID=" + SysSettingID;
    var wnd = SysSettingDetail;
    wnd.content("<div id='SysSettingDetailDiv'></div>");
    $("#SysSettingDetailDiv").load("/Report/Designer?ReportID=" + SysSettingID);

    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });

    wnd.title("تنظیمات " + NodeParentName + " " + NodeTitle);
    wnd.center();
    wnd.open();
}
  
function SelectFolderPath(e) {
    var filePath = $('#' + e.currentTarget.id).val();
    var theFiles = e.target.files;
    var relativePath = theFiles[0].webkitRelativePath;
    var folder = relativePath.split("/");
    alert(folder[0]);
}

function OnSelectQueryObjectTree(e) {
    var FieldName = this.element[0].id.split('_')[1];
    var DataInfo = e.node.dataset.info;
    if (DataInfo != undefined) {
        var Element = $("#PersianQuery_" + FieldName);
        var first = Element.val().substring(0, Element[0].selectionStart);
        var second = Element.val().substring(Element[0].selectionEnd, Element.val().length);
        Element.val(first + ' ' + DataInfo + ' ' + second);
        //Element.focus();
    }
}

function ConvertToSqlQuery(e) {
    var FieldName = e.sender.element[0].id.split('_')[1];
    $.ajax({
        url: "/SysSetting/ConvertToSQLQuery",
        data: {
            Query: $("#PersianQuery_" + FieldName).val()
        },
        type: "POST",
        success: function (Result) {
            $("#EnglishQuery_" + FieldName).val(Result);
            popupNotification.show('عملیات ترجمه با موفقیت انجام شد', "success");
            var tabStrip = $("#QueryObjectTabstrip_" + FieldName).data("kendoTabStrip");
            tabStrip.select(1);
        },
        error: function (result) { }
    })
}

function ShowAPMCoreSetting(ActionNum) {
    ShowLoader();
    var wnd = APMCoreSettingWin;
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });

    wnd.center();

/*    if (wnd.content() == "") {*/
        wnd.content("<div Id='APMCoreSettingMainDiv'></div>");

        $("#APMCoreSettingMainDiv").load("/SysSetting/ShowAPMCoreSetting",
            {
                ActionNum: ActionNum
            },
            function () {
            wnd.open();
            HideLoader();
        });
    //}
    //else { 
    //    wnd.open();
    //    HideLoader();
    //}
}


function ReloadAPMCoreSettingGrid() {

}

function CreateAPMCoreScript() {
    if ($("#FromCoreObjectID").data("kendoNumericTextBox").value() != 0 && $("#ToCoreObjectID").data("kendoNumericTextBox").value() != 0) {
        $.ajax({
            url: "/SysSetting/CreateAPMCoreScript",
            data: {
                FromCoreObjectID: $("#FromCoreObjectID").data("kendoNumericTextBox").value(),
                ToCoreObjectID: $("#ToCoreObjectID").data("kendoNumericTextBox").value(),
                ScriptType: $("#CoreObjectScriptType")[0].checked
            },
            type: "POST",
            success: function (Result) {
                HideLoader();
                popupNotification.show('عملیات ترجمه با موفقیت انجام شد', "success");
                if ($("#CoreObjectClearScript")[0].checked)
                    $("#ResultScriptAPMCoreSetting").data("kendoTextArea").value(Result)
                else {
                    var Text = $("#ResultScriptAPMCoreSetting").data("kendoTextArea").value() + "\n\n" + Result;
                    $("#ResultScriptAPMCoreSetting").data("kendoTextArea").value(Text);

                }
            },
            error: function (result) {
                HideLoader();
                popupNotification.show("عملیات با شکست مواجه شد", "error");
            }
        })
    }
    else {
        popupNotification.show("مقادیر وارد شده صحیح نمی باشد", "error");
    }
}

function ShowRegistryTable(RegisteryType, TableName, Title, TableID, RowID) {
    var wnd = SearchFormWin;
    wnd.content("");
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });

    wnd.title(Title.replaceAll('_', ' '));
    wnd.center();
    wnd.open();

    $("#SearchFormWin").load("/Home/ShowRegistryTable", {
        'RegistryTable': RegisteryType,
        'TableName': TableName.replaceAll(' ', '_'),
        'TableID': TableID,
        'RowID': RowID
    }, function () {
        HideLoader();
    });
}


function MainGridRegisteryChange(e) {
    if (e.sender.select != null) {
        var grid = $("#" + e.sender.content.context.id).data("kendoGrid");
        var selectedItem = grid.dataItem(grid.select());
        var Item = selectedItem.Value.replace("</Items>", "").split("<Items>");
        if (Item.length > 1) {
            Item = Item[1].replace("</FieldData>", "").split("<FieldData>");
            var index = 0;
            for (index = 0; index < Item.length; index++) {
                if (Item[index] != "") {
                    var Column = Item[index].replace("<Name>", "").split("</Name>");

                    var Element = $("#RegistryTable_" + Column[0]);

                    if (Column[1].indexOf("<Value />") == -1) {
                        var Value = Column[1].replace("<Value>", "").split("</Value>")[0];
                        if (Element.data("kendoAutoComplete") != undefined) {
                            Element.data("kendoAutoComplete").value(Value);
                        }
                        else if (Element.data("kendoComboBox") != undefined) {
                            Element.data("kendoComboBox").value(Value);
                        }
                        else if (Element.data("kendoDropDownList") != undefined) {
                            Element.data("kendoDropDownList").value(Value);
                        }
                        else if (Element.data("kendoDropDownTree") != undefined) {
                            Element.data("kendoDropDownTree").dataSource.filter({}).value(Value);
                        }
                        else if (Element.data("kendoEditor") != undefined) {
                            Element.data("kendoEditor").value(Value);
                        }
                        else if (Element.data("kendoNumericTextBox") != undefined) {
                            Element.data("kendoNumericTextBox").value(Value);
                        }
                        else if (Element.data("kendoSwitch") != undefined) {
                            if (Value == "True") {
                                $("#RegistryTable_" + Column[0]).parent().removeClass("k-switch-off")
                                $("#RegistryTable_" + Column[0]).parent().addClass("k-switch-on")
                            }
                            else {
                                $("#RegistryTable_" + Column[0]).parent().removeClass("k-switch-on");
                                $("#RegistryTable_" + Column[0]).parent().addClass("k-switch-off");
                            }
                        }
                        else if (Element.data("kendoTextArea") != undefined) {
                            Element.data("kendoTextArea").value(Value);
                        }
                        else if (Element.data("kendoTextBox") != undefined) {
                            Element.data("kendoTextBox").value(Value);
                        }
                    }
                }
            }
        }
    }
}

function SearchRegistryTable() {
    var optionalData = { _UserAccountID: $("#SearchRegistryTable_UserAccountID").val(), _FromDate: $("#SearchRegistryTable_FromDate").val(), _ToDate: $("#SearchRegistryTable_ToDate").val() };
    var grid = $("#MainGridRegistery").data("kendoGrid");
    grid.dataSource.read(optionalData);
}

function RestoreDeletedRecord() {
    var grid = $("#MainGridRegistery").data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != null) {
        ShowLoader();
        $.ajax({
            url: "/Home/RestoreDeletedRecord",
            data: {
                RecordId: selectedItem.id
            },
            type: "POST",
            success: function (Result) {
                HideLoader();
                if (Result == true) {
                    popupNotification.show('عملیات ترجمه با موفقیت انجام شد', "success");
                    grid.dataSource.remove(selectedItem);
                }
                else
                    popupNotification.show("عملیات با شکست مواجه شد", "error");
            },
            error: function (result) {
                HideLoader();
                popupNotification.show("عملیات با شکست مواجه شد", "error");
            }
        })
    }
    else
        popupNotification.show("رکوردی انتخاب نشده است", "error");
}

function OpenWinEditorForm(e) {
    if (parseInt($("#SysSettingID").text())>0) { 
        PopupMax("/EditorForm?FormID=" + $("#SysSettingID").text(), "");
    } 
}

function Reload_CoreObject_Click(e) {
    var dropdowntree = $("#" + e.id).data("kendoDropDownTree");
    dropdowntree.dataSource.read();
}

function Add_CoreObject_Click(e) { 
    PopupMax("/SysSetting", "");
}


function RunAPMCoreQyeryButton() { 
    $.ajax({
        url: "/SysSetting/RunAPMCoreQyery",
        data: {
            Query: $("#QueryAPMCoreSetting").val().replaceAll('<',"$**$")
        },
        type: "POST",
        success: function (Result) {
            HideLoader();
            if (Result == true) {
                popupNotification.show('عملیات با موفقیت انجام شد', "success");
            }
            else
                popupNotification.show("عملیات با شکست مواجه شد", "error");
        },
        error: function (result) {
            HideLoader();
            popupNotification.show("عملیات با شکست مواجه شد", "error");
        }
    })
}



function onCheckedCoreObjectTreeView() {
    var checkedNodes = [],
        treeView = $("#CoreObjectTreeView").data("kendoTreeView"); 
    checkedNodeIds(treeView.dataSource.view(), checkedNodes);
    $("#CoreObjectTreeViewText")[0].value = checkedNodes.join(",").replace(",0_base", "").replaceAll("CoreObjectTree_", ""); 
    var SaveCoreObjectTreeViewButton = $("#SaveCoreObjectTreeViewButton").data("kendoButton");
    SaveCoreObjectTreeViewButton.enable($("#CoreObjectTreeViewText")[0].value == "" ? false : true); 
}


function SaveCoreObjectTreeViewButton(e) {
    $.ajax({
        url: "/SysSetting/SaveCoreObjectTreeView",
        data: {
            ElementId :SysSettingID,
            SelectedCoreObject: $("#CoreObjectTreeViewText")[0].value, 
        },
        type: "POST",
        success: function (Result) { 
            if (Result == true) { 
                UploadTableAttachmentDialog.open();
            }
            else
                popupNotification.show("عملیات با شکست مواجه شد", "error");
        },
        error: function (result) {
            HideLoader();
            popupNotification.show("عملیات با شکست مواجه شد", "error");
        }
    }) 
}


function loadTableAttachmentFiles(e) {
    if (!e.files || e.files.length != 1) return;

    var formData = new FormData();
    formData.append("files", e.files[0].rawFile);
    $.ajax({
        url: "/Attachment/UploadTableAttachment",
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) {
            console.log(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}


// function that gathers IDs of checked nodes
function checkedNodeIds(nodes, checkedNodes) {
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            checkedNodes.push(nodes[i].id);
        }

        if (nodes[i].hasChildren) {
            checkedNodeIds(nodes[i].children.view(), checkedNodes);
        }
    }
}

// show checked node IDs on datasource change
function OnChangeCoreRelatedTableCheckbox(e) {
    var checkedNodes = [],
        dropDownTree = $("#" + e.sender.element[0].id).data("kendoDropDownTree"),
            message;

    checkedNodeIds(dropDownTree.dataSource.view(), checkedNodes);

    if (checkedNodes.length > 0) {
        message = checkedNodes.join(",");
    } else {
        message = "";
    }

    $("#Result_" + e.sender.element[0].id).data("kendoTextBox").value(message);
}

function CreateScriptCore() {
    var ErrorMessage = "";
    if ($("#ScriptCoreFromDate").val() == "")
        ErrorMessage += "فیلد از تاریخ را پر نمایید\n";
    if ($("#ScriptCoreFromTime").val() == "")
        ErrorMessage += "فیلد از ساعت را پر نمایید\n";
    if ($("#ScriptCoreToDate").val() == "")
        ErrorMessage += "فیلد تا تاریخ را پر نمایید\n";
    if ($("#ScriptCoreToTime").val() == "")
        ErrorMessage += "فیلد تا ساعت را پر نمایید\n";
    if (ErrorMessage=="")
        window.location = '/SysSetting/CreateScriptCore?ScriptCoreFromDate=' + $("#ScriptCoreFromDate").val() + "&ScriptCoreFromTime=" + $("#ScriptCoreFromTime").val() + "&ScriptCoreToDate=" + $("#ScriptCoreToDate").val() + "&ScriptCoreToTime=" + $("#ScriptCoreToTime").val();
    else
        popupNotification.show(ErrorMessage.replace('\n','</br>'), "error");
}


function UploadFileScriptCore(e) {
    if (!e.files || e.files.length != 1) return; 
    var formData = new FormData();
    formData.append("files", e.files[0].rawFile);
    $.ajax({
        url: "/Attachment/UploadFileScriptCore",
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) {
            $("#QueryAPMCoreSetting").data("kendoTextArea").value(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function UploadFileScriptCoreButton() { 
    var UploadFileScriptCoreDialog = $("#UploadFileScriptCoreDialog").data("kendoDialog");
    UploadFileScriptCoreDialog.open();
}

function ClearScriptCoreButton() { 
    $("#QueryAPMCoreSetting").data("kendoTextArea").value("");
}

function DownloadAPMCoreQyeryButton() { 
    if ($("#ResultScriptAPMCoreSetting").data("kendoTextArea").value() != "") {

        $.ajax({
            url: "/SysSetting/CreateDownloadAPMCoreQyery",
            data: {
                'Text': $("#ResultScriptAPMCoreSetting").data("kendoTextArea").value().replaceAll('<', '&gt;')
            },
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                if (data!="")
                window.location = '/SysSetting/DownloadAPMCoreQyery?FileName=' + data;
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
    else
        popupNotification.show("محتوای کوئری خالی است", "error");
}