
function Shift_ShowMealGrid() {
    ShowLoader();
    var ErrorMessage = "";
    var SelectedMeal = [];  
    var SelectedMealTitle= [];
    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);

    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList");
    var IsSatellite = $("#ShiftChangeDay").data("kendoDropDownList") == undefined ? false : true;
    var ErrorMessage = "";

    if ($("#StartDateReservationFood").val() > $("#EndDateReservationFood").val())
        ErrorMessage += "تاریخ شروع باید کوچکتر از تاریخ پایان باشد.\n";
    if ($("#StartDateReservationFood").val() == "" || $("#EndDateReservationFood").val() == "")
        ErrorMessage += "تاریخ نباید خالی باشد.\n";
    if (AccountNumberElement.value() == 0)
        ErrorMessage += "شماره حساب انتخاب نشده است \n";
    if (PersonRestaurant.value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است \n";

    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && FormInputValue[Index] == true) {
            var MealReservationFoodID = FormInputName[Index].replace("MealReservationFood_", "");
            SelectedMeal.push(MealReservationFoodID); 
            SelectedMealTitle.push($("#MealTitleReservationFood_" + MealReservationFoodID).text());
        } 
    }

    if (SelectedMeal.length == 0)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";
      
     
    if (ErrorMessage != "") {
        HideLoader();
        popupNotification.show(ErrorMessage.replace('\n', '<br/>'), "error"); 
    }
    else {
        DisableShiftReservationFoodForm(false);
        $("#MealReservationForm").load("/Food/ShiftMeal", {
                RecordID: $("#RecordID").val(),
                FromDate: $("#StartDateReservationFood").val(),
                ToDate: $("#EndDateReservationFood").val(),
                Restaurant: PersonRestaurant.value(),
                SelectedMeal: SelectedMeal.join(','),
                SelectedMealTitle: SelectedMealTitle.join(','),
                IsSatellite: IsSatellite,
                SearchOfReserveID:$("#SearchOfReserveID").data("kendoTextBox").value()
            }, function () {
                HideLoader();
            })
        }
}

function DisableShiftReservationFoodForm(IsEnable) {
    $("#StartDateReservationFood").prop('disabled', !IsEnable);
    $("#StartDateReservationFoodButton").prop('disabled', !IsEnable);
    $("#EndDateReservationFood").prop('disabled', !IsEnable);
    $("#EndDateReservationFoodButton").prop('disabled', !IsEnable);
    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").enable(IsEnable);
    $("#PersonRestaurant").data("kendoDropDownList").enable(IsEnable);   
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").enable(IsEnable);
    $("#ShowMealGrid").data("kendoButton").enable(IsEnable);

    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);
    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1) {
            $("#" + FormInputName[Index]).data("kendoSwitch").enable(IsEnable);
        }
    }
} 

function NewShiftReservationFood() {
    $("#RecordID").val(0);
    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").value(0);
    $("#PersonRestaurant").data("kendoDropDownList").value(0);   
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").value("");
    $("#SearchOfReserveID").data("kendoTextBox").value("");

    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);
    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1) {
            if ($("#" + FormInputName[Index])[0].parentElement.className.indexOf("k-switch-on") > -1) {

                $("#" + FormInputName[Index])[0].checked = false;
                $($("#" + FormInputName[Index])[0].parentElement).removeClass("k-switch-on");
                $($("#" + FormInputName[Index])[0].parentElement).addClass("k-switch-off");
            }
        }
    }
    $("#MealReservationForm").empty();
    $("#PesronGrid").data('kendoGrid').dataSource.data([]);
    DisableShiftReservationFoodForm(true);
}

function SaveShiftReservationFood() {
    ShowLoader();
    var PesronGrid = $("#PesronGrid").data().kendoGrid.dataSource.view();
    var PesronGridJSON = JSON.stringify(PesronGrid);

    var MealFoodGrid = $("#MealFoodGrid").data().kendoGrid.dataSource.view();
    var MealFoodGridJson= JSON.stringify(MealFoodGrid);

    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList"); 
    var ShiftChangeDay = $("#ShiftChangeDay").data("kendoDropDownList");
    var ErrorMessage = ""; 
     
    if (AccountNumberElement.value() == 0)
        ErrorMessage += "شماره حساب وارد نشده است \n";
     
    if (PersonRestaurant.value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است \n";


    var SelectedMeal = []; 
    var FormInputName = [];
    var FormInputValue = [];

    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);

    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && FormInputValue[Index] == true) {
            SelectedMeal.push(FormInputName[Index].replace("MealReservationFood_", ""));
        } 
    }

    if (SelectedMeal.length == 0 )
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";
     
    if (ErrorMessage != "") {
        HideLoader();
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error"); 
    }
    else
    {
        $.ajax({
            url: "/Food/SaveShiftReservationFood",
            data: {
                'RecordID': $("#RecordID").val(),
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#EndDateReservationFood").val(),
                'RequestType': ShiftChangeDay == undefined ? 5 : 6,
                'ShiftChangeDay': ShiftChangeDay == undefined ? 0 : ShiftChangeDay.value(),
                'Restaurant': PersonRestaurant.value(),
                'OfficeDepartmentAccountNumber': AccountNumberElement.value(),
                'Message': $("#OfficeDepartmentAccountNumberMessage").val(), 
                'SearchOfReserveID': $("#SearchOfReserveID").data("kendoTextBox").value(),
                'GridJSON': MealFoodGridJson,
                'BedGridJSON': PesronGridJSON,
                'SelectedMeal': SelectedMeal.join(','),
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result.RecordID > 0) {
                    popupNotification.show("شماره رزرو : " + result.RecordID + "  <br/>" + result.Message.replace(/\n/g, '<br/>'), "success"); 
                    NewShiftReservationFood();
                }
                else {
                    popupNotification.show("ذخیره سازی با شکست مواجه شد" + '<br/>' + result.Message.replace(/\n/g, '<br/>'), "error"); 
                }
            },
            error: function (result) {
                HideLoader();
            }
        })
    }
}

function PersonShiftOnKeyDown(e) {
    ShowLoader(); 
    var FormInputName = [];
    var FormInputValue = [];
    var SelectedMeal = [];
    var ErrorMessage = "";

    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);

    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && FormInputValue[Index] == true) {
            var MealReservationFoodID = FormInputName[Index].replace("MealReservationFood_", "");
            SelectedMeal.push(MealReservationFoodID);
        }
    }

    if (SelectedMeal.length == 0)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";
      
    var FromDate = $("#StartDateReservationFood").val();
    var ToDate = $("#EndDateReservationFood").val();

    if (ErrorMessage == "") {
        $.ajax({
            url: "/Food/GetPersonInfo",
            data: {
                'SearchValue': e.value,
                'RequestType':5,
                'FromDate': FromDate,
                'ToDate': ToDate,
                'Restaurant': $("#PersonRestaurant").data("kendoDropDownList").value(),
                'RecordID': $("#RecordID").val(),
                'SelectedMeal': SelectedMeal.join(','),
                'BedReservationFood': false
            },
            type: "POST",
            success: function (result) {
                var elements = jQuery.parseJSON(result.Data);
                if (elements.length > 0) {
                    var BedGrid = $("#PesronGrid").data("kendoGrid");
                    var OfficePersonGridData = BedGrid.dataSource.view();
                    var RowCounter = OfficePersonGridData.length;

                    for (var index = 0; index < elements.length; index++) {
                        var flag = true;
                        for (var x = OfficePersonGridData.length - 1; x >= 0; x--) {
                            if (OfficePersonGridData[x].شناسه == elements[index].شناسه)
                                flag = false;
                        }
                        if (flag) {
                            var datasource = BedGrid.dataSource;
                            datasource.add({
                                شناسه_درخواست_رزرو_غذا: 0,
                                ردیف: RowCounter + 1,
                                شناسه: elements[index].شناسه,
                                نام: elements[index].نام,
                                شماره_ملی: elements[index].کد_ملی,
                                شماره_پرسنلی: elements[index].شماره_پرسنلی,
                                نوع_استخدام: elements[index].نوع_استخدام,
                                تخت :0
                            });
                            RowCounter = RowCounter + 1;
                        }
                        else {

                        }

                    }

                }
                else {
                    popupNotification.show("پرسنلی با مشخصات فوق یافت نشد", "error");
                }

                if (result.Error != "") {
                    popupNotification.show(result.Error, "error");
                }

                HideLoader();
            },
            error: function (result) { 
                HideLoader();
            }
        })
    }
    else {
        HideLoader();
        popupNotification.show(ErrorMessage.replace("\n","<br/>"), "error");
    }
     
}


function OpenShiftPersonOfGrid() {
    var wnd = $("#SearchShiftPersonWin").data("kendoWindow");   
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 30,
        height: newHeight - 50
    });

    wnd.center();
    wnd.open();
}


function AddShiftPersonToGrid(e) {
    var FormInputName = [];
    var FormInputValue = [];
    var SelectedMeal = [];
    var ErrorMessage = "";

    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);
    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && FormInputValue[Index] == true) {
            var MealReservationFoodID = FormInputName[Index].replace("MealReservationFood_", "");
            SelectedMeal.push(MealReservationFoodID);
        }
    }

    if (SelectedMeal.length == 0)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";

    if ($("#PersonRestaurant").data("kendoDropDownList").value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است";

    if (ErrorMessage == "") {
        var grid = $("#SearchPersonGrid").data("kendoGrid");
        var dataItem = grid.dataSource.get(e);

        $.ajax({
            url: "/Food/CheckPersonInfoToReserve",
            data: {
                'RecordID': $("#RecordID").val(),
                'Personel': dataItem.id,
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#EndDateReservationFood").val(),
                'Restaurant': $("#PersonRestaurant").data("kendoDropDownList").value(),
                'SelectedMeal': SelectedMeal.join(','),
                'BedReservationFood': false
            },
            type: "POST",
            success: function (result) {
                if (result == "") {
                    var oldConfirm = window.confirm;
                    window.confirm = function () { return true; };
                    var row = grid.tbody.find("tr[data-uid='" + dataItem.uid + "']");
                    grid.removeRow(row);
                    window.confirm = oldConfirm;

                    var OfficePersonGrid = $("#PesronGrid").data("kendoGrid");
                    var OfficePersonGridData = OfficePersonGrid.dataSource.view();
                    var RowCounter = OfficePersonGridData.length;

                    var flag = true;
                    for (var x = OfficePersonGridData.length - 1; x >= 0; x--) {
                        if (OfficePersonGridData[x].شناسه == dataItem.شناسه)
                            flag = false;
                    }
                    if (flag) {
                        var datasource = OfficePersonGrid.dataSource;
                        datasource.add({
                            شناسه_درخواست_رزرو_غذا:0,
                            ردیف: RowCounter + 1,
                            شناسه: dataItem.شناسه,
                            نام: dataItem.نام,
                            شماره_پرسنلی: dataItem.شماره_پرسنلی,
                            شماره_ملی: dataItem.کد_ملی,
                            نوع_استخدام: dataItem.نوع_استخدام,
                            تخت: 0
                        });
                        RowCounter = RowCounter + 1;
                    }
                }
                else {
                    popupNotification.show(result.replace('\n', '<br/>'), "error");
                }

            },
            error: function (result) {

            }
        })
    }
    else {
        popupNotification.show(ErrorMessage.replace('\n', '<br/>'), "error");
    }
}


function ShiftMealGridChange(e) {
//    if ($("#RecordID").val() > 0 && e.model["شناسه"] > 0) {
//        var ShiftChangeDay = $("#ShiftChangeDay").data("kendoDropDownList");
//        $.ajax({
//            url: "/Food/ShiftMealGridChange",
//            data: {
//                'ParentID': $("#RecordID").val(),
//                'FromDate': $("#StartDateReservationFood").val(),
//                'ToDate': $("#EndDateReservationFood").val(),
//                'ShiftChangeDay': ShiftChangeDay == undefined ? 0 : ShiftChangeDay.value(),
//                'RecordID': e.model['شناسه'],
//                'Meal': e.model["GridMealReservationFood"],
//                'DayTitle': e.sender.columns[e.sender._lastCellIndex].field,
//                'DayValue': e.model[e.sender.columns[e.sender._lastCellIndex].field]
//            },
//            type: 'POST',
//            success: function (result) {
//                HideLoader();
//                if (result == "0") {

//                }
//                else if (result != "") {
//                    popupNotification.show(result.replace('\n', '<br/>'), "error");
//                    var Grid = $("#" + e.sender.element[0].id).data("kendoGrid");
//                    Grid.dataSource.read();
//                }
//                else {
//                    popupNotification.show("ذخیره سازی با موفقیت انجام شد", "success"); 
//                }
//            },
//            error: function (result) {
//                HideLoader();
//            }
//        })
//    }
}

function RemoveShiftMealGrid(e) {
    if ($("#RecordID").val() > 0 && e.model.شناسه > 0) {
        ShowLoader();

        $.ajax({
            type: 'POST',
            url: "/Food/RemoveShiftMealGrid",
            data: {
                "ParentID": $("#RecordID").val(),
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#EndDateReservationFood").val(),
                'RecordID': e.model['شناسه'],
                'Meal': e.model["GridMealReservationFood"],
            },
            dataType: 'json',
            success: function (data)
            {
                if (data == "")
                {
                    HideLoader();
                    popupNotification.show("عملیات حذف با موفقیت انجام شد", "success"); 
                }
                else {
                    popupNotification.show(data, "error");
                    HideLoader();
                    var Grid = $("#" + e.sender.element[0].id).data("kendoGrid");
                    Grid.dataSource.read();
                }
            },
            error: function (result) {
                popupNotification.show(result.responseText, "error");
            }
        });
    }
}

function RemoveShiftPersonGrid(e) {
    if ($("#RecordID").val() > 0 && e.model.شناسه_درخواست_رزرو_غذا > 0) {
        //ShowLoader();

        //$.ajax({
        //    type: 'POST',
        //    url: "/Food/RemoveShiftPersonGrid",
        //    data: {
        //        "ParentID": $("#RecordID").val(), 
        //        'RecordID': e.model['شناسه'], 
        //        'Bed': e.model['تخت'],
        //        'FromDate': $("#StartDateReservationFood").val(),
        //        'ToDate': $("#EndDateReservationFood").val(),
        //    },
        //    dataType: 'json',
        //    success: function (data) {
        //        if (data == "") {
        //            HideLoader();
        //            popupNotification.show("عملیات حذف با موفقیت انجام شد", "success");
        //        }
        //        else {
        //            HideLoader();
        //            popupNotification.show(data, "error");
        //            HideLoader();
        //            var Grid = $("#" + e.sender.element[0].id).data("kendoGrid");
        //            Grid.dataSource.read();
        //        }
        //    },
        //    error: function (result) {
        //        HideLoader();
        //        popupNotification.show(result.responseText, "error");
        //    }
        //});
    }
}



function ShiftPersonGridChange(e) {
    if($("#RecordID").val() > 0 && e.model["شناسه_درخواست_رزرو_غذا"] > 0)
    {
        $.ajax({
            url: "/Food/UpdateOfficeReservationFood",
            data: {
                'ParentID': $("#RecordID").val(),
                'FromDate': $("#StartDateReservationFood").val(),
                'Person': e.model['شناسه'],
                'Meal': e.sender.columns[e.sender._lastCellIndex].field,
                'MealValue': e.model[e.sender.columns[e.sender._lastCellIndex].field]
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result == "0") {

                }
                else if (result != "") {
                    popupNotification.show(result, "error"); 
                }
                else {
                    popupNotification.show("ذخیره سازی با موفقیت انجام شد", "success"); 
                }
            },
            error: function (result) {
                HideLoader();
            }
        })
    }
}



function SearchOfReserveIDShiftOnKeyDown(e) {

    ShowLoader();
    $.ajax({
        url: "/Food/GetDelayedPersonInfoBySearchOfReserveID",
        data: { 'SearchValue': e.value },
        type: "POST",
        success: function (result) {
            var elements = jQuery.parseJSON(result.PersonelData);
            if (elements.length > 0) {
                var PersonGrid = $("#PesronGrid").data("kendoGrid");
                var OfficePersonGridData = PersonGrid.dataSource.view();
                var RowCounter = OfficePersonGridData.length;

                for (var index = 0; index < elements.length; index++) {
                    var flag = true;
                    for (var x = OfficePersonGridData.length - 1; x >= 0; x--) {
                        if (OfficePersonGridData[x].شناسه == elements[index].شناسه)
                            flag = false;
                    }
                    if (flag) {
                        var datasource = PersonGrid.dataSource;
                        datasource.add({
                            شناسه: elements[index].شناسه,
                            شناسه_درخواست_رزرو_غذا: 0,
                            ردیف: RowCounter + 1,
                            نام: elements[index].نام + " " + elements[index].نام_خانوادگی,
                            شماره_ملی: elements[index].کد_ملی,
                            شماره_پرسنلی: elements[index].شماره_پرسنلی,
                            نوع_استخدام: elements[index].نوع_استخدام
                        });
                        RowCounter = RowCounter + 1;
                    }
                    else {

                    }

                }

            }
            else {
                popupNotification.show("پرسنلی با مشخصات فوق یافت نشد", "error");
            }
            if (result.ReserveData.Columns.length > 0) {
                var Columns = result.ReserveData.Columns;
                var Values = result.ReserveData.Values;
                var MealReserves = Values[Columns.indexOf("وعده_غذایی")];

                $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").value(Values[Columns.indexOf("شماره_حساب")]);
                $("#PersonRestaurant").data("kendoDropDownList").value(Values[Columns.indexOf("رستوران")]);
                $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").value(Values[Columns.indexOf("توضیحات")]);

                var FormInputName = [];
                var FormInputValue = [];
                GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);
                for (var Index = 0; Index < FormInputName.length; Index++) {
                    if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && MealReserves != "null" && MealReserves != null && MealReserves != "") {
                        if (MealReserves.split(',').indexOf(FormInputName[Index].replace("MealReservationFood_", "")) > -1) {
                            $("#" + FormInputName[Index])[0].checked = true;
                            $($("#" + FormInputName[Index])[0].parentElement).removeClass("k-switch-off");
                            $($("#" + FormInputName[Index])[0].parentElement).addClass("k-switch-on");
                        }
                        else {
                            if ($("#" + FormInputName[Index])[0].parentElement.className.indexOf("k-switch-on") > -1) {
                                $("#" + FormInputName[Index])[0].checked = false;
                                $($("#" + FormInputName[Index])[0].parentElement).removeClass("k-switch-on");
                                $($("#" + FormInputName[Index])[0].parentElement).addClass("k-switch-off");
                            }
                        }

                    }
                }
            }
            else {
                popupNotification.show("درخواستی با شماره رزرو فوق یافت نشد", "error");
            }
            HideLoader();
        },
        error: function (result) {
            HideLoader(); 
        }
    })
}