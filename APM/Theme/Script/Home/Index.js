
var MainRightMenuTree;
var MainTabstrip;
var MainDrawer;
var popupNotification;
var RoleTypePermissionForm;
var AttachmentWindow;
var ReportViewerWin;
var SysSettingWin;
var DetailsGridWin;
var SysSettingDetail;
var SysSettingDialog;
var AlarmSysSettingDialog;
var ProcessReferralWin;
var UserCalendarWin;
var UserCalendarEditWin;
var APMCoreSettingWin;
var GridPrintWin;
var SearchFormWin;
var DataTableWin;
var GridCellColumnPicTempWin;
var UploadTableAttachmentWin;
var UserCalendarDialog;
var UploadPublickKeyDialog;
var ProductUpdateFromTaxOrganizationDialog;
var UploadTableAttachmentDialog;
var DeleteCalendarDialog;
var UploadFileForOCRDialog;
var MainGridList = [];
var KeepShowCalender = false;
var SignalRService = null;
var baseUrl = $("#BaseUrl").data("baseurl");

window.onload = function () {
    setTimerOn();
    LoadInformationEntryFormBadge(); 
    setTimeout(function () {
        if ($("#HomeErrorMessage").val() != "" && $("#HomeErrorMessage").val() != undefined) {
            $("#UserPhotoSide").data("kendoTooltip").show($("#tooltip-template"));
            ChangePasswordButton();
            $('.MessageDalert .modal-header').empty();
            $('.MessageDalert .modal-body').empty();
            $('.MessageDalert .modal-header').append("اخظار");
            $('.MessageDalert .modal-body').append($("#HomeErrorMessage").val().replaceAll("_", " "));
            $('.MessageDalert .modal-content').addClass("ErrorMessageDalert");
            $('.MessageDalert').modal('show');
        }
    });

}
//window.addEventListener('click', function (e) {
//    if (document.getElementById('UserCalendar').contains(e.target) || document.getElementById('calendar-div').contains(e.target) || document.getElementById('CalendarForm').contains(e.target) || KeepShowCalender) {
//        ShowCalenderGrid();
//        KeepShowCalender = false;
//    }
//    else {
//        $("#calendar-div").css("display", "none");
//    }
//});


function GetUserNotification() {
    $.ajax({
        type: 'POST',
        url: "/Home/GetUserNotification",
        data: {},
        dataType: 'json',
        success: function (data) {
            if (data > 0) {
                $("#UserNotificationBadge").addClass("k-badge-error");
            }
            else {

            }
        }
    });
}

function ConfirmClose() {
    $.ajax({
        url: "/Signin/Logout",
        data: {},
        type: "POST",
        success: function (Result) {

        },
        error: function (result) {

        }
    })
}


function ShowCalenderGrid() {
    KeepShowCalender = true;
    $("#calendar-div").css("display", "flex");
    $("#calendar-grid .k-grid-content").css("height", "273px");

    $(".Holiday").kendoBadge({
        themeColor: "warning",
        shape: "rectangle"
    });
}

var intervalLoader = null; 

$(document).ready(function () {
    MainRightMenuTree = $("#MainRightMenuTree").data("kendoTreeView");
    popupNotification = $("#popupNotification").data("kendoNotification");
    RoleTypePermissionForm = $("#RoleTypePermission").data("kendoWindow");
    UserCalendarWin = $("#UserCalendarWin").data("kendoWindow");
    UserCalendarEditWin = $("#UserCalendarEditWin").data("kendoWindow");
    GridPrintWin = $("#GridPrintWin").data("kendoWindow");
    AttachmentWindow = $("#MasterAttachmentForm").data("kendoWindow");
    SysSettingWin = $("#SysSettingWin").data("kendoWindow");
    SysSettingDetail = $("#SysSettingDetail").data("kendoWindow");
    ProcessReferralWin = $("#ProcessReferralWin").data("kendoWindow");
    ReportViewerWin = $("#ReportViewerWin").data("kendoWindow");
    DetailsGridWin = $("#DetailsGridWin").data("kendoWindow");
    DataTableWin = $("#DataTableWin").data("kendoWindow");
    SearchFormWin = $("#SearchFormWin").data("kendoWindow");
    GridCellColumnPicTempWin = $("#GridCellColumnPicTempWin").data("kendoWindow");
    APMCoreSettingWin = $("#APMCoreSettingWin").data("kendoWindow");
    UploadTableAttachmentWin = $("#UploadTableAttachmentWin").data("kendoWindow");
    SysSettingDialog = $("#SysSettingDialog").data("kendoDialog");
    UserCalendarDialog = $("#UserCalendarDialog").data("kendoDialog");
    DeleteCalendarDialog = $("#DeleteCalendarDialog").data("kendoDialog");
    AlarmSysSettingDialog = $("#AlarmSysSettingDialog").data("kendoDialog");
    UploadPublickKeyDialog = $("#UploadPublickKeyDialog").data("kendoDialog");
    ProductUpdateFromTaxOrganizationDialog = $("#ProductUpdateFromTaxOrganizationDialog").data("kendoDialog");
    UploadTableAttachmentDialog = $("#UploadTableAttachmentDialog").data("kendoDialog");
    UploadFileForOCRDialog = $("#UploadFileForOCRDialog").data("kendoDialog");


    $('.aboutPage').click(function (e) {
        $("div.k-drawer-items").find(".k-state-selected").removeClass("k-state-selected");

        var aboutTitle = $(".aboutPage").text();
        $("div.appTitle").text(aboutTitle);
        $("#drawer-content>main").load("/Home/About");
        document.title = aboutTitle + " - AdminDashboard";
    });

    
    $('#ToggleDrawer').click(function (e) {
        var splitter = $("#MasterLayout").data("kendoSplitter");
        var Side = $("#RightLayout");
        var Status = Side.width() > 0 ? "collapse" : "expand"; 
        setTimeout(function () {
            if (Status == "collapse") {
                splitter.options.panes[1].size = "0px";
                splitter.size("#RightLayout", "0px");
            }
            else {
                splitter.options.panes[1].size = "240px";
                splitter.size("#RightLayout", "240px");
            }
        });


    });


    //$('.k-filter-apply').click(function (e) {
    //    var ISDetailGridForm = e.currentTarget.parentElement.id.indexOf("DetailFilter") > -1 ? true : false;
    //    var ReplaceWord = ISDetailGridForm ? "DetailFilter" : "Filter";
    //    var FormArraye = e.currentTarget.parentElement.id.split('_');
    //    var DataKey = FormArraye[1];
    //    var ParentID = FormArraye[2];
    //    var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[1] : "");

    //    $("#DivFilter_" + DataKey + "_" + ParentID).css("display", "none");
    //    $("#" + GridName).css("height", "100%");
    //})

    //document.title = $("div.appTitle").text();

    $("#appbar").kendoPopover({
        showOn: "click",
        filter: ".k-i-notification",
        width: "400px",
        height: "392px",
        position: "bottom",
        header: kendo.template($("#notification-header").html()),
        body: kendo.template($("#notification-body").html()),
        show: function (e) {
            $("#NotificationMainDiv").load("/Home/NotificationMainDiv", {}, function () {

            });
            $("#NotificationHeaderDiv").load("/Home/NotificationHeaderDiv", {}, function () {
                    $("#new-count").kendoBadge({
                        themeColor: "warning",
                        shape: "rectangle"
                    });
            });


            //$(".new-count").kendoBadge({
            //    themeColor: "warning",
            //    shape: "rectangle"
            //});

            //$('.badge-missed').kendoBadge({
            //    themeColor: 'warning',
            //    shape: 'circle',
            //    size: 'small'
            //});
             
            //$("#mark-as-read").kendoButton({
            //    click: function () {
            //        $(".k-badge-dot").remove();
            //        $('.badge-missed').remove();
            //        $("#new-count").text("0 New");
            //        $(".notification-item").css("background-color", "rgba(66, 66, 66, 0.04)")
            //    }
            //});
        }
    });  



    if ($(window).width() <= 980) {
        setTimeout(function () {
            var splitter = $("#MasterLayout").data("kendoSplitter");
            var Side = $("#RightLayout");  
            splitter.options.panes[1].size = "0px";
            splitter.size("#RightLayout", "0px");  
        });
    };

});

function RefreshAll() {
    ShowLoader();
    $.ajax({
        url: "/Home/ReloadCoreObject",
        data: {},
        type: "POST",
        success: function (Result) {
            popupNotification.show('عملیات بروز رسانی  با موفقیت انجام شد', "success");
            location.reload();
            HideLoader();
        },
        error: function (result) {

        }
    })

}

function SysSetting() {
    ShowLoader();
    if ($("#SysSettingDiv").length == 0) {
        $.ajax({
            url: "/Home/SysSetting",
            data: {},
            type: "POST",
            success: function (Result) {
                HideLoader();
                var wnd = SysSettingWin;
                wnd.content("<div id='SysSettingDiv'></div>");
                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                wnd.setOptions({
                    width: newWidth - 50,
                    height: newHeight - 50
                });

                wnd.center();
                wnd.open();
                $("#SysSettingDiv")[0].innerHTML = "";
                $("#SysSettingDiv").append(Result);
            },
            error: function (result) {

            }
        })
    }
    else {
        HideLoader();
        var wnd = SysSettingWin;
        var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        wnd.setOptions({
            width: newWidth - 50,
            height: newHeight - 50
        });

        wnd.center();
        wnd.open();
    }

}

function ChangePasswordButton() {
    var FormValue = $("#ChangePasswordInput").val().replace('/', '');
    var UserIDValue = $("#UserIDInput").val().replace('/', '');
    OpenEditorForm(FormValue, 0, UserIDValue, false, false, "تغییر کلمه عبور");
}

function ToggleDrawerClick() { 
    if ($(window).width() <= 980) { 
        var splitter = $("#MasterLayout").data("kendoSplitter");
        var Side = $("#RightLayout");
        splitter[Side.width() > 0 ? "collapse" : "expand"](Side);
    };
}

function ShowNotificationTopCenter(e) {
    if (e.sender.getNotifications().length == 1) {
        var element = e.element.parent(),
            eWidth = element.width(),
            eHeight = element.height(),
            wWidth = $(window).width(),
            wHeight = $(window).height(),
            newTop, newLeft;

        newLeft = Math.floor(wWidth / 2 - eWidth / 2);
        newTop = 10;

        e.element.parent().css({ top: newTop, left: newLeft, zIndex: 1100000 });
    }
}

function signOut() {
    ConfirmClose();
    location.href = "/Signin";
};


function onItemClickDrawer(e) {
    if (!e.item.hasClass("k-drawer-separator")) {
        var tabName = e.item.find(".k-item-text").text().split(" ")[0];
        var titleName = e.item.find(".k-item-text").text();
        e.sender.drawerContainer.find("#drawer-content > main").empty();
        $("#drawer-content>main").load("/Home/" + tabName);

        document.URL.title = titleName + " - AdminDashboard";
        $("div.appTitle").text(titleName);

        if ($("div.appTitle").text() === "Dashboard") {
            $("div.appTitle").text("داشبورد");
        }

        if ($(window).width() <= 980) {
            //var MainDrawer = e.sender;
            //MainDrawer.hide();
        }
    }
}
 

function CloseAttachmentForm(e) {
    var wnd = AttachmentWindow;
    wnd.content("<div id='AttachmentWindowDiv'></div>");
}

function ShowReport(ReportID, ReportTitle, _ParameterID, _ParameterValue, IsPrint = false) {
    if (IsPrint) {
        var wnd = ReportViewerWin;   
        wnd.setOptions({
            width: 1,
            height: 1
        });

        wnd.center(); 
        _ParameterID = _ParameterID == undefined ? 0 : _ParameterID;
        _ParameterValue = _ParameterValue == undefined ? "" : _ParameterValue;
        $("#ReportViewerDiv").load("/Report/PrintReportWithoutPriView?ReportID=" + ReportID + "&_ParameterID=" + _ParameterID + "&_ParameterValue=" + _ParameterValue, function (Result) {
            HideLoader(); 
        });
         
    }
    else { 
        var wnd = ReportViewerWin;
        wnd.title(ReportTitle);
        var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        wnd.setOptions({
            width: newWidth - 50,
            height: newHeight - 50
        });

        wnd.center();
        wnd.open();
        _ParameterID = _ParameterID == undefined ? 0 : _ParameterID;
        _ParameterValue = _ParameterValue == undefined ? "" : _ParameterValue;
        $("#ReportViewerDiv").load("/Report/Viewer?ReportID=" + ReportID + "&_ParameterID=" + _ParameterID + "&_ParameterValue=" + _ParameterValue, function () {
            HideLoader();

        });
    }
}


function UserSystemSetting() {

}

function ConfigProgressBar(ID,MaxValue,MinValue,Value) {
    var Progress = $("#" + ID).data("kendoProgressBar");
    if (Progress != undefined) {
        Progress.value(Value);
        Progress.options.max = MaxValue;
        Progress.options.min = MinValue; 
    }
}


function ReduceFileSize() {

    var wsImpl = window.WebSocket || window.MozWebSocket;
    window.ws = new wsImpl('ws://localhost:8182/');
    ws.onmessage = function (e) {
        if (typeof e.data === "string") {
            //IF Received Data is String
        }
        else if (e.data instanceof ArrayBuffer) {
            //IF Received Data is ArrayBuffer
        }
        else if (e.data instanceof Blob) {

            ScanCounter++;

            var f = e.data;
            var reader = new FileReader();
            reader.onload = function (e) {
                var a = e.target.result;
            }
            reader.readAsDataURL(f);
        }
    };
    ws.onopen = function () {
        //Do whatever u want when connected succesfully
        ws.send("1101");
    };
    ws.onclose = function () {
        $('.ScannerDalert').modal('show');
    };
}
 


function OCR_UploadFile(event) {
    UploadFileForOCRDialog.open();
};



function loadOCRFile(e) {
    if (!e.files || e.files.length != 1) return;

    var formData = new FormData();
    formData.append("files", e.files[0].rawFile);
    $.ajax({
        url: "/Attachment/UploadOCRFile",
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) {
            $("#TextPreview").data("kendoTextArea").value(data);
            var wnd = $("#TextPreviewWindow").data("kendoWindow");
            var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
            var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

            wnd.setOptions({
                width: newWidth - 50,
                height: newHeight - 50
            });

            wnd.center().open(); 
        },
        error: function (e) {
            console.log(e);
        }
    });
}




function SeatOnclic(Item,DataKey) {

    var grid = $("#MainGrid" + DataKey).data("kendoGrid");
    var ItemSelected = grid.select().length;
    var RowID = 0;
    var SeatArray = Item.target.id.split('_');
    if (!Item.target.checked)
        RowID = $("#" + Item.target.id).data('val').toString().replace('/','');
    else if (ItemSelected == 0) {
        popupNotification.show("سطری انتخاب نشده است", "info");
        $("#" + Item.target.id).prop("checked", false)
    }
    var selectedItem = Item.target.checked ? grid.dataItem(grid.select()) : grid.dataSource.get(RowID);
    RowID = selectedItem.id;
    if (RowID > 0) {

        $("#" + Item.target.id + "_0").text(Item.target.checked ? (SeatArray[0] + SeatArray[1] + "[" + selectedItem.CardNumber + "]") : (SeatArray[0] + SeatArray[1]));
        if (Item.target.checked) {
            grid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
            $("#" + Item.target.id).data('val', RowID);

            $("#AirPlane_TotalAcceptPassenger").text((parseInt($("#AirPlane_TotalAcceptPassenger").text()) + 1));

            switch (selectedItem.AgeType) {
                case "A": { $("#AirPlane_TotalAdult").text((parseInt($("#AirPlane_TotalAdult").text()) + 1)); break; }
                case "C": { $("#AirPlane_TotalChild").text((parseInt($("#AirPlane_TotalChild").text()) + 1)); break; }
                case "I": { $("#AirPlane_TotalInfant").text((parseInt($("#AirPlane_TotalInfant").text()) + 1)); break; }
            }

            switch (selectedItem.Gender) {
                case "F": { $("#AirPlane_TotalAcceptFemalePassenger").text((parseInt($("#AirPlane_TotalAcceptFemalePassenger").text()) + 1)); break; }
                case "M": { $("#AirPlane_TotalAcceptMalePassenger").text((parseInt($("#AirPlane_TotalAcceptMalePassenger").text()) + 1)); break; }
            }


            $("#" + Item.target.id + "_1 .Seat_PassengerName").text(selectedItem.PersianFirstName + " " + selectedItem.PersianLastName);
            $("#" + Item.target.id + "_1 .Seat_PassengerNationalID").text(selectedItem.NationalID);
            $("#" + Item.target.id + "_1 .Seat_PassengerPassportID").text(selectedItem.PassportID);
            $("#" + Item.target.id + "_1 .Seat_PassengerGender").text(selectedItem.Gender);
            $("#" + Item.target.id + "_1 .Seat_PassengerAgeType").text(selectedItem.AgeType);
        }
        else {
            grid.table.find("tr[data-uid='" + selectedItem.uid + "']").removeClass("Valid-row");
            $("#" + Item.target.id).data('val', 0);

            $("#AirPlane_TotalAcceptPassenger").text((parseInt($("#AirPlane_TotalAcceptPassenger").text()) - 1));

            switch (selectedItem.AgeType) {
                case "A": { $("#AirPlane_TotalAdult").text((parseInt($("#AirPlane_TotalAdult").text()) - 1)); break; }
                case "C": { $("#AirPlane_TotalChild").text((parseInt($("#AirPlane_TotalChild").text()) - 1)); break; }
                case "I": { $("#AirPlane_TotalInfant").text((parseInt($("#AirPlane_TotalInfant").text()) - 1)); break; }
            }

            switch (selectedItem.Gender) {
                case "F": { $("#AirPlane_TotalAcceptFemalePassenger").text((parseInt($("#AirPlane_TotalAcceptFemalePassenger").text()) - 1)); break; }
                case "M": { $("#AirPlane_TotalAcceptMalePassenger").text((parseInt($("#AirPlane_TotalAcceptMalePassenger").text()) - 1)); break; }
            }


            $("#" + Item.target.id + "_1 .Seat_PassengerName").text("");
            $("#" + Item.target.id + "_1 .Seat_PassengerNationalID").text("");
            $("#" + Item.target.id + "_1 .Seat_PassengerPassportID").text("");
            $("#" + Item.target.id + "_1 .Seat_PassengerGender").text("");
            $("#" + Item.target.id + "_1 .Seat_PassengerAgeType").text("");
        }

        $.ajax({
            type: 'POST',
            url: "/Desktop/SaveFormEditor",
            data: {
                "DataKey": DataKey,
                "ParentID": selectedItem.Flight,
                "RecordID": RowID,
                "FormInputName": ["AirplaneSeat","PassengersStatus"],
                "FormInputValue": Item.target.checked ? [SeatArray[2],2] :[0,1],
                "ProcessID": 0,
                "ProcessStepID": 0,
                'SaveChilde': false,
                "JsonGrid": [],
                'GridName': "MainGrid" + DataKey
            },
            dataType: 'json',
            success: function (data) {
                if (data.Message != "")
                    popupNotification.show(data.Message, "error");
                else {

                }
            },
            error: function (result) {
                popupNotification.show("خطایی رخ داده است", "error");
            }
        }); 
    }

 
}


function Select_MainRightMenuTree(e) {
    let Title = $(e.item).text();
    var node = $("#MainRightMenuTree").getKendoTreeView().dataItem($(e.target).closest(".k-item"));
    SysSettingTreeView = $("#MainRightMenuTree").data("kendoTreeView");

    if (node.id != "0") { 
        switch (Title) {
            case "باز کردن در تب جدید":
                var win = null;

                if (node.id.indexOf("InformationEntryForm") > -1)
                    win = window.open("/Desktop/Index?Form=" + node.id.replace("InformationEntryForm", "").split('_')[0], '_blank');
                else if (node.id.indexOf("Process") > -1)
                    win = window.open("/Desktop/Index?ProcessID=" + node.id.replace("Process", ""), '_blank');   
                else if (node.id.indexOf("Report") > -1)
                    win = window.open("/Report/Index?ReportID=" + node.id.replace("Report", "").split('_')[0], '_blank');  

                if (win) { 
                    win.focus();
                } else { 
                    alert('Please allow popups for this website');
                } 
                break;
            default: break;
        }
    }
}