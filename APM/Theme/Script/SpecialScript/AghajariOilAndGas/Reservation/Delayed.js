function NewDelayedReservationFood() {
    $("#PersonGrid").data('kendoGrid').dataSource.data([]); 
    $("#StartDateReservationFood").val($("#NowDateReservationFood").val());
    $("#EndDateReservationFood").val($("#NowDateReservationFood").val()); 
    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").value(0); 
    $("#PersonRestaurant").data("kendoDropDownList").value(0);
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").value("");  
}

function SaveDelayedReservationFood() {
    ShowLoader();
    var ErrorMessage = "";
    var SelectedGrid = $("#PersonGrid").data().kendoGrid.dataSource.view();
    if (SelectedGrid.length == 0)
        ErrorMessage += "نفرات انتخاب نشده است \n";

    var GridJSON = JSON.stringify(SelectedGrid);

    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var Restaurant = $("#PersonRestaurant").data("kendoDropDownList");
    //if ($("#StartDateReservationFood").val() < $("#NowDateReservationFood").val())
    //    ErrorMessage += "تاریخ شروع باید بزرگتر یا مساوی تاریخ امروز باشد\n";
    //if ($("#EndDateReservationFood").val() < $("#NowDateReservationFood").val())
    //    ErrorMessage += "تاریخ پایان باید بزرگتر یا مساوی تاریخ امروز باشد \n";


    if (Restaurant.value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است \n";

    if (AccountNumberElement.value() == 0)
        ErrorMessage += "شماره حساب وارد نشده است \n"; 

    if (ErrorMessage != "") {
        HideLoader(); 
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error");
    }
    else {

        $.ajax({
            url: "/Food/CheckReserve",
            data: {
                RecordID: $("#RecordID").val(),
                FromDate: $("#StartDateReservationFood").val(),
                ToDate: $("#EndDateReservationFood").val(),
                Restaurant: Restaurant.value(),
                SelectedMeal: "2",
                SelectedMealTitle: "ناهار",
                SelectedDey: "",
                IsReserveBed: false
            },
            type: "POST",
            success: function (result) {
                if (result == "") {

                    $.ajax({
                        url: "/Food/SaveDelayedReservationFood",
                        data: {
                            "RecordID": $("#RecordID").val(),
                            'FromDate': $("#StartDateReservationFood").val(),
                            'ToDate': $("#EndDateReservationFood").val(),
                            'RequestType': 4,
                            'OfficeDepartmentAccountNumber': AccountNumberElement.value(),
                            'Message': $("#OfficeDepartmentAccountNumberMessage").val(),
                            'Restaurant': Restaurant.value(),
                            'GridJSON': GridJSON
                        },
                        type: 'POST',
                        success: function (result) {
                            HideLoader();
                            if (result.RecordID > 0) {
                                popupNotification.show("شماره رزرو : " + result.RecordID + "  <br/>" + result.Message.replace(/\n/g, '<br/>'), "info");
                                NewDelayedReservationFood();
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
                else {
                    HideLoader();
                    popupNotification.show(result, "error");
                }
            },
            error: function (result) {

            }
        })


    }
}


function PersonDelayedOnKeyDown(e) {
    ShowLoader();
    $.ajax({
        url: "/Food/GetDelayedPersonInfo",
        data: { 'SearchValue': e.value },
        type: "POST",
        success: function (result) {
            var elements = jQuery.parseJSON(result);
            if (elements.length > 0) {
                var PersonGrid = $("#PersonGrid").data("kendoGrid");
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
                            شناسه_درخواست_رزرو_غذا:0,
                            ردیف: RowCounter + 1,
                            نام: elements[index].نام, 
                            نام_خانوادگی: elements[index].نام_خانوادگی,
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
            HideLoader();
        },
        error: function (result) {

        }
    })
}

function OpenDelayedPersonOfGrid() {
    var wnd = $("#SearchDelayedPersonWin").data("kendoWindow");
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 30,
        height: newHeight - 50
    });

    wnd.center();
    wnd.open();
}


function AddDelayedPersonToGrid(e) {
    var grid = $("#DelayedSearchPersonGrid").data("kendoGrid");
    var dataItem = grid.dataSource.get(e);

    $.ajax({
        url: "/Food/CheckPersonInfoToReserve",
        data: {
            'RecordID': $("#RecordID").val(),
            'Personel': dataItem.id,
            'FromDate': $("#StartDateReservationFood").val(),
            'ToDate': $("#EndDateReservationFood").val(),
            'Restaurant': $("#PersonRestaurant").data("kendoDropDownList").value(),
            'SelectedMeal': "2",
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

                var PersonGrid = $("#PersonGrid").data("kendoGrid");
                var PersonGridData = PersonGrid.dataSource.view();
                var RowCounter = PersonGridData.length;

                var flag = true;
                for (var x = PersonGridData.length - 1; x >= 0; x--) {
                    if ( PersonGridData[x].شناسه == dataItem.شناسه)
                        flag = false;
                }
                if (flag) { 
                    var datasource = PersonGrid.dataSource;
                    datasource.add({
                        شناسه: dataItem.شناسه,
                        شناسه_درخواست_رزرو_غذا:0,
                        ردیف: RowCounter + 1,
                        نام: dataItem.نام,  
                        نام_خانوادگی: dataItem.نام_خانوادگی,
                        شماره_ملی: dataItem.کد_ملی,
                        شماره_پرسنلی: dataItem.شماره_پرسنلی, 
                        نوع_استخدام: dataItem.نوع_استخدام, 
 
                    });
                    RowCounter = RowCounter + 1;
                }
                else {

                }
            }
            else
            {
                popupNotification.show(result.replace(/\n/g, '<br/>'), "error");
            }
 
        },
        error: function (result) {

        }
    }) 
}

function DelayedPersonGridSearchInputKeyDown(e) {
    if (e.key == "Enter") {
        var Id = e.currentTarget.id;
        GridFilterChange("DelayedSearchPersonGrid", Id);
    }
}



function ReloadSearchDelayedPersonGrid() {
    var grid = $("#DelayedSearchPersonGrid").data("kendoGrid");
    grid.dataSource.read({ IsReload: true });

}

function RemoveDelayedPerson(e) {
    if ($("#RecordID").val() > 0 && e.model.شناسه_درخواست_رزرو_غذا > 0) {
        ShowLoader();

        $.ajax({
            type: 'POST',
            url: "/Food/RemoveDelayedPerson",
            data: {
                "RecordID": $("#RecordID").val(),
                "Person": e.model.شناسه,
            },
            dataType: 'json',
            success: function (data) {
                if (data == "") {
                    HideLoader();
                    popupNotification.show("عملیات حذف با موفقیت انجام شد", "success");
                }
                else {
                    popupNotification.show(data, "error");
                    HideLoader();
                    var PersonGrid = $("#" + e.sender.element[0].id).data("kendoGrid");
                    PersonGrid.dataSource.read();
                }
            },
            error: function (result) {
                popupNotification.show(result.responseText, "error");
            }
        });
    }
}

function SearchOfReserveIDOnKeyDown(e) {

    ShowLoader();
    $.ajax({
        url: "/Food/GetDelayedPersonInfoBySearchOfReserveID",
        data: { 'SearchValue': e.value },
        type: "POST",
        success: function (result) {
            var elements = jQuery.parseJSON(result.PersonelData);
            if (elements.length > 0) {
                var PersonGrid = $("#PersonGrid").data("kendoGrid");
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
                            نام: elements[index].نام,
                            نام_خانوادگی: elements[index].نام_خانوادگی,
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
            HideLoader();
        },
        error: function (result) {

        }
    })
}















