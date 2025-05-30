function OfficeRemoveTab(e) {
    var tabstrip = $("#OfficePersonTab").data("kendoTabStrip");
    var tab = $(e.currentTarget).closest("li");
    var OrginalDate = tab[0].id.replace("Tab", "");
    var Date = OrginalDate.substring(0, 4) + "/" + OrginalDate.substring(4, 6) + "/" + OrginalDate.substring(6, 8);

    if ($("#RecordID").val() > 0 && $("#OfficePersonGrid" + OrginalDate).data("kendoGrid").dataSource.view().length > 0) {
        ShowLoader();

        $.ajax({
            url: "/Food/RemoveLetterMealFoodWithDate",
            data: {
                'RecordID': $("#RecordID").val(),
                'Date': Date
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result == "0") {
                    tabstrip.remove(tab);
                    tabstrip.select(0);
                }
                else if (result != "") {
                    popupNotification.show(result, "error");
                    $("#OfficePersonGrid" + OrginalDate).data("kendoGrid").dataSource.read();
                }
                else {
                    popupNotification.show("ذخیره سازی با موفقیت انجام شد", "success");
                    tabstrip.remove(tab);
                    tabstrip.select(0);
                }
            },
            error: function (result) {
                HideLoader();
            }
        })
    }
    else {
        tabstrip.remove(tab);
        tabstrip.select(0);
    }
}

function addEventListenerChangeDateReservationFood(e) {
    document.getElementById(e).addEventListener("jdp:change", function (e) { CheckDateReservationFood(e) });
    document.getElementById(e).addEventListener("change", function (e) { CheckDateReservationFood(e) });
}

function CheckDateReservationFood(e) {
    if (e.target.value < $("#NowDateReservationFood").val()) {
        //$(e.target).val($("#NowDateReservationFood").val());
    }
    else {
        var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList");
        if (PersonRestaurant != undefined)
            PersonRestaurant.dataSource.read({
                FromDate: $("#StartDateReservationFood").val(),
                ToDate: $("#EndDateReservationFood").val()
            });
    }

    if($("#StartDateReservationFood").val() != $("#EndDateReservationFood").val())
        $("#MainDivWeekDay").show();
    else
        $("#MainDivWeekDay").hide(); 
}

function ShowPersonGridReservationFood() {
    ShowLoader();
    var ErrorMessage = "";
    var SelectedMeal = []; 
    var SelectedDey = [];
    var SelectedMealTitle= [];
    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);

    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList"); 
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
        if (FormInputName[Index].indexOf("WeekDaysReservationFood_") > -1 && FormInputValue[Index] == true) {
            SelectedDey.push(FormInputName[Index].replace("WeekDaysReservationFood_", ""));
        }
    }

    if (SelectedMeal.length == 0 && $("#BedReservationFood")[0].checked == false)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";

    //if ($("#StartDateReservationFood").val() != $("#EndDateReservationFood").val()) { 
    //      SelectedMeal = [];
    //      SelectedMealTitle = [];
    //}
     
    if (ErrorMessage != "") {
        HideLoader();
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "info"); 
    }
    else
    {

        $.ajax({
            url: "/Food/CheckReserve",
            data: {
                RecordID: $("#RecordID").val(),
                FromDate: $("#StartDateReservationFood").val(),
                ToDate: $("#EndDateReservationFood").val(),
                Restaurant: PersonRestaurant.value(),
                SelectedMeal: SelectedMeal.join(','),
                SelectedMealTitle: SelectedMealTitle.join(','),
                SelectedDey: SelectedDey.join(','),
                IsReserveBed: $("#BedReservationFood")[0].checked
            },
            type: "POST",
            success: function (result) {
                if (result == "") {
                    DisableReservationFoodForm(false);
                    $("#PersonReservationFoodForm").load("/Food/PersonOffice", {
                        RecordID: $("#RecordID").val(),
                        FromDate: $("#StartDateReservationFood").val(),
                        ToDate: $("#EndDateReservationFood").val(),
                        Restaurant: PersonRestaurant.value(),
                        SelectedMeal: SelectedMeal.join(','),
                        SelectedMealTitle: SelectedMealTitle.join(','),
                        SelectedDey: SelectedDey.join(','),
                        IsReserveBed: $("#BedReservationFood")[0].checked
                    }, function () {
                        HideLoader();
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

function DisableReservationFoodForm(IsEnable) {
    $("#StartDateReservationFood").prop('disabled', !IsEnable);
    $("#StartDateReservationFoodButton").prop('disabled', !IsEnable);
    $("#EndDateReservationFood").prop('disabled', !IsEnable);
    $("#EndDateReservationFoodButton").prop('disabled', !IsEnable);
    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").enable(IsEnable);
    $("#PersonRestaurant").data("kendoDropDownList").enable(IsEnable);
    $("#VehiclesOfficeDepartmentAccountNumber").data("kendoDropDownList").enable(IsEnable);
    $("#DriverOfficeDepartmentAccountNumber").data("kendoDropDownList").enable(IsEnable); 
    $("#BedReservationFood").data("kendoSwitch").enable(IsEnable);
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").enable(IsEnable); 
    $("#ShowPersonGridReservationFood").data("kendoButton").enable(IsEnable);

    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);
    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1) {
            $("#" + FormInputName[Index]).data("kendoSwitch").enable(IsEnable);
        }
    }
}

function ClearReservationFoodForm() {
    $("#RecordID").val(0);
    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").value(0);
    $("#PersonRestaurant").data("kendoDropDownList").value(0);
    $("#VehiclesOfficeDepartmentAccountNumber").data("kendoDropDownList").value(0);
    $("#DriverOfficeDepartmentAccountNumber").data("kendoDropDownList").value(0); 
    if ($("#BedReservationFood")[0].parentElement.className.indexOf("k-switch-on") > -1) {
        $("#BedReservationFood")[0].checked = false;
        $($("#BedReservationFood")[0].parentElement).removeClass("k-switch-on");
        $($("#BedReservationFood")[0].parentElement).addClass("k-switch-off");
    }
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").value(""); 

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
}

function VehiclesOfficeDepartmentAccountNumber_Change(e) {
    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var VehiclesElement = $("#VehiclesOfficeDepartmentAccountNumber").data("kendoDropDownList");
    var DriverElement = $("#DriverOfficeDepartmentAccountNumber").data("kendoDropDownList");
    if (AccountNumberElement.value() > 0 && VehiclesElement.value() > 0) {
        DriverElement.dataSource.read({ Vehicle: VehiclesElement.value(), OfficeDepartmentAccountNumber: AccountNumberElement.value() });
    }
    else
        DriverElement.dataSource.read();
}

function MealReservationFood_Load(Value) {
    var Element = $("#MealReservationFood_" + Value).data("kendoSwitch");
    Element.bind('change', function (e) {
        MealReservationFood_SwitchChange(e.sender.element[0]);
    });

    if ($("#MealReservationFood_" + Value)[0].checked == true) {
        if ($("#MainDivBedCount_" + Value).length > 0) {
            $("#MainDivBedCount_" + Value).show();
        }
    }
    else {
        if ($("#MainDivBedCount_" + Value).length > 0) {
            $("#MainDivBedCount_" + Value).hide();
        }
    }
}

function MealReservationFood_SwitchChange(Element) {
    var MealValue = Element.id.replace("MealReservationFood_", "");
    if (Element.checked == true) {
        if (Element.classList[0].indexOf("SuperiorMeal_") > -1) {
            var SuperiorMealVal = Element.classList[0].replace("SuperiorMeal_", "");
            if (SuperiorMealVal != "0") {
                var ElementItem = $("#MealReservationFood_" + SuperiorMealVal).data("kendoSwitch");
                if (ElementItem != undefined)
                    if (ElementItem.element[0].parentElement.className.indexOf("k-switch-on") > -1) {
                        $("#MealReservationFood_" + SuperiorMealVal)[0].checked = false;
                        $(ElementItem.element[0].parentElement).removeClass("k-switch-on");
                        $(ElementItem.element[0].parentElement).addClass("k-switch-off");
                    }
            }
        }
        if ($("#MainDivBedCount_" + MealValue).length > 0) {
            $("#MainDivBedCount_" + MealValue).show();
        }
    }
    else { 
        if ($("#MainDivBedCount_" + MealValue).length> 0) {
            $("#MainDivBedCount_" + MealValue).hide();
        }
    }
}

function NewOfficeReservationFood() {
    DisableReservationFoodForm(true);
    ClearReservationFoodForm();
    $("#PersonReservationFoodForm").empty();
}

function PersonOfficeOnKeyDown(e) {
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

    if (SelectedMeal.length == 0 && $("#BedReservationFood")[0].checked == false)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";

    var BedReservationFood = $("#BedReservationFood").data("kendoSwitch");
    var SearchInputId = e.sender.element[0].id;
    var FromDate = $("#StartDateReservationFood").val();
    var ToDate = $("#EndDateReservationFood").val();
    if (SearchInputId.replace("PersonOfficeDepartmentAccountNumber", "") != "") {
        var Date = SearchInputId.replace("PersonOfficeDepartmentAccountNumber", "") ;
        Date = Date.substring(0, 4) + "/" + Date.substring(4, 6) + "/" + Date.substring(6, 8);

        FromDate = Date;
        ToDate = Date;
    }

    $.ajax({
        url: "/Food/GetPersonInfo",
        data: {
            'SearchValue': e.value,
            'FromDate': FromDate,
            'ToDate': ToDate,
            'Restaurant': $("#PersonRestaurant").data("kendoDropDownList").value(),
            'RecordID': $("#RecordID").val(),
            'SelectedMeal': SelectedMeal.join(','),
            'BedReservationFood': $("#BedReservationFood")[0].checked
        },
        type: "POST",
        success: function (result) {
            var elements = jQuery.parseJSON(result.Data); 
            if (elements.length > 0) {

                var tabStrip = $("#OfficePersonTab").data("kendoTabStrip");  
                $(".OfficePersonTab").each(function (index, Item) {
                    if (Item.id.replace("Tab", "") == SearchInputId.replace("PersonOfficeDepartmentAccountNumber", "") || SearchInputId == "PersonOfficeDepartmentAccountNumber") {
                        var OfficePersonGrid = $("#" + Item.id.replace("Tab", "OfficePersonGrid")).data("kendoGrid");
                        var OfficePersonGridData = OfficePersonGrid.dataSource.view();
                        var RowCounter = OfficePersonGridData.length;

                        for (var index = 0; index < elements.length; index++) {
                            var flag = true;
                            for (var x = OfficePersonGridData.length - 1; x >= 0; x--) {
                                if (OfficePersonGridData[x].شناسه == elements[index].شناسه)
                                    flag = false;
                            }
                            if (flag) {
                                var OrginalDate = Item.id.replace("Tab", "");
                                var Date = Item.id.replace("Tab", "");
                                Date = Date.substring(0, 4) + "/" + Date.substring(4, 6) + "/" + Date.substring(6, 8)
                                var datasource = OfficePersonGrid.dataSource;
                                datasource.add({
                                    ردیف: RowCounter + 1,
                                    شناسه_درخواست_غذا: 0,
                                    تاریخ: Date,
                                    شناسه: elements[index].شناسه,
                                    نام_پرسنل: elements[index].نام,
                                    شماره_پرسنلی: elements[index].شماره_پرسنلی,
                                    شماره_ملی: elements[index].کد_ملی, 
                                    GridMealReservationFood_1: ($("#" + OrginalDate + "MealDefualtFood_1").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_1").val()),
                                    GridMealReservationFood_2: ($("#" + OrginalDate + "MealDefualtFood_2").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_2").val()),
                                    GridMealReservationFood_3: ($("#" + OrginalDate + "MealDefualtFood_3").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_3").val()),
                                    GridMealReservationFood_4: ($("#" + OrginalDate + "MealDefualtFood_4").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_4").val()),
                                    GridMealReservationFood_5: ($("#" + OrginalDate + "MealDefualtFood_5").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_5").val()),
                                    GridMealReservationFood_6: ($("#" + OrginalDate + "MealDefualtFood_6").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_6").val()),
                                    GridMealReservationFood_7: ($("#" + OrginalDate + "MealDefualtFood_7").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_7").val()),
                                    GridMealReservationFood_8: ($("#" + OrginalDate + "MealDefualtFood_8").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_8").val()),
                                    GridMealReservationFood_9: ($("#" + OrginalDate + "MealDefualtFood_9").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_9").val()),
                                    GridMealReservationFood_10: ($("#" + OrginalDate + "MealDefualtFood_10").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_10").val()),
                                    GridMealReservationFood_11: ($("#" + OrginalDate + "MealDefualtFood_11").length == 0 ? 0 : $("#" + OrginalDate + "MealDefualtFood_11").val()),
                                    تخت: BedReservationFood.element[0].checked
                                });
                                RowCounter = RowCounter + 1;
                            }
                            else {

                            }

                        }
                    }

                }) 

            }
            else {
                popupNotification.show("پرسنلی با مشخصات فوق یافت نشد", "info"); 
            }

            if (result.Error != "") {
                popupNotification.show(result.Error.replace('\n', '<br/>'), "error");
            }

            HideLoader();
        },
        error: function (result) { 
            HideLoader();
        }
    }) 
}

function OpenPersonOfGrid() { 
    var wnd = $("#SearchPersonWin").data("kendoWindow");   
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 30,
        height: newHeight - 50
    });

    wnd.center();
    wnd.open();
}

function AddPersonToGrid(e) {
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

    if (SelectedMeal.length == 0 && $("#BedReservationFood")[0].checked == false)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";

    if ($("#PersonRestaurant").data("kendoDropDownList").value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است";
     
    var BedReservationFood = $("#BedReservationFood").data("kendoSwitch");
    var grid = $("#SearchPersonGrid").data("kendoGrid");      
    var dataItem = grid.dataSource.get(e);
    if (ErrorMessage == "") {
        $.ajax({
            url: "/Food/CheckPersonInfoToReserve",
            data: {
                'RecordID': $("#RecordID").val(),
                'Personel': dataItem.id,
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#EndDateReservationFood").val(),
                'Restaurant': $("#PersonRestaurant").data("kendoDropDownList").value(),
                'SelectedMeal': SelectedMeal.join(','),
                'BedReservationFood': $("#BedReservationFood")[0].checked 
            },
            type: "POST",
            success: function (result) {
                if (result == "") { 
                    var oldConfirm = window.confirm;
                    window.confirm = function () { return true; };
                    var row = grid.tbody.find("tr[data-uid='" + dataItem.uid + "']");
                    grid.removeRow(row);
                    window.confirm = oldConfirm;
                 
                    var tabStrip = $("#OfficePersonTab").data("kendoTabStrip");
                    $(".OfficePersonTab").each(function (index, Item) {
                        var OfficePersonGrid = $("#" + Item.id.replace("Tab", "OfficePersonGrid")).data("kendoGrid");
                        var OfficePersonGridData = OfficePersonGrid.dataSource.view();
                        var RowCounter = OfficePersonGridData.length;

                        var flag = true;
                        for (var x = OfficePersonGridData.length - 1; x >= 0; x--) {
                            if (OfficePersonGridData[x].شناسه == dataItem.شناسه)
                                flag = false;
                        }
                        if (flag) {
                            var OrginalDate = Item.id.replace("Tab", "");
                            var Date = Item.id.replace("Tab", "");
                            Date = Date.substring(0, 4) + "/" + Date.substring(4, 6) + "/" + Date.substring(6, 8)
                            var datasource = OfficePersonGrid.dataSource;
                            datasource.add({
                                ردیف: RowCounter + 1,
                                شناسه_درخواست_غذا:0,
                                تاریخ: Date,
                                شناسه: dataItem.شناسه,
                                نام_پرسنل: dataItem.نام,
                                شماره_پرسنلی: dataItem.شماره_پرسنلی,
                                شماره_ملی: dataItem.کد_ملی,
                                GridMealReservationFood_1: ($("#" + OrginalDate + "MealDefualtFood_1").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_1").val()),
                                GridMealReservationFood_2: ($("#" + OrginalDate + "MealDefualtFood_2").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_2").val()),
                                GridMealReservationFood_3: ($("#" + OrginalDate + "MealDefualtFood_3").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_3").val()),
                                GridMealReservationFood_4: ($("#" + OrginalDate + "MealDefualtFood_4").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_4").val()),
                                GridMealReservationFood_5: ($("#" + OrginalDate + "MealDefualtFood_5").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_5").val()),
                                GridMealReservationFood_6: ($("#" + OrginalDate + "MealDefualtFood_6").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_6").val()),
                                GridMealReservationFood_7: ($("#" + OrginalDate + "MealDefualtFood_7").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_7").val()),
                                GridMealReservationFood_8: ($("#" + OrginalDate + "MealDefualtFood_8").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_8").val()),
                                GridMealReservationFood_9: ($("#" + OrginalDate + "MealDefualtFood_9").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_9").val()),
                                GridMealReservationFood_10: ($("#" + OrginalDate + "MealDefualtFood_10").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_10").val()),
                                GridMealReservationFood_11: ($("#" + OrginalDate + "MealDefualtFood_11").length == 0 ? 0 : $("#" + OrginalDate +"MealDefualtFood_11").val()),
                                تخت: BedReservationFood.element[0].checked
                            });
                            RowCounter = RowCounter + 1;
                        }
                        else {

                        }

                    })
                }
                else
                {
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

function SaveOfficeReservationFood() {
    ShowLoader();
    var SelectedGrid=[];
     
    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var Restaurant = $("#PersonRestaurant").data("kendoDropDownList");
    var VehiclesElement = $("#VehiclesOfficeDepartmentAccountNumber").data("kendoDropDownList");
    var DriverElement = $("#DriverOfficeDepartmentAccountNumber").data("kendoDropDownList"); 
    var ErrorMessage = "";
    //if ($("#StartDateReservationFood").val() < $("#NowDateReservationFood").val())
    //    ErrorMessage += "تاریخ شروع باید بزرگتر یا مساوی تاریخ امروز باشد\n";
    //if ($("#EndDateReservationFood").val() < $("#NowDateReservationFood").val())
    //    ErrorMessage += "تاریخ پایان باید بزرگتر یا مساوی تاریخ امروز باشد \n";
     

    if (AccountNumberElement.value() == 0)
        ErrorMessage += "شماره حساب وارد نشده است \n"; 

    var SelectedMeal = [];
    var SelectedDey = [];
    var FormInputName = [];
    var FormInputValue = [];

    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);

    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && FormInputValue[Index] == true) { 
            SelectedMeal.push(FormInputName[Index].replace("MealReservationFood_", ""));
        }

        if (FormInputName[Index].indexOf("WeekDaysReservationFood_") > -1 && FormInputValue[Index] == true) { 
            SelectedDey.push(FormInputName[Index].replace("WeekDaysReservationFood_", ""));
        }
    }
    if (SelectedMeal.length == 0 && $("#BedReservationFood")[0].checked == false)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";


    var tabStrip = $("#OfficePersonTab").data("kendoTabStrip");
    $(".OfficePersonTab").each(function (index, Item) {
        var OfficePersonGrid = $("#" + Item.id.replace("Tab", "OfficePersonGrid")).data().kendoGrid.dataSource.view();

        var OrginalDate = Item.id.replace("Tab", "");
        var Date = OrginalDate.substring(0, 4) + "/" + OrginalDate.substring(4, 6) + "/" + OrginalDate.substring(6, 8);
        if (OfficePersonGrid.length == 0)
            ErrorMessage += "نفرات برای تاریخ " + Date + " انتخاب نشده است \n";
        else {

            var Grid = $("#" + Item.id.replace("Tab", "OfficePersonGrid")).data().kendoGrid ;
            var SelectedRows = Grid.table.find("tr") ;

            if (SelectedRows.length > 0) {
                SelectedRows.each(function (index, row) {
                    var selectedItem = Grid.dataItem(row);
                    var HasData = false;
                    for (var Index = 0; Index < Grid.columns.length; Index++) {
                        if (Grid.columns[Index].field != undefined)
                            if (Grid.columns[Index].field.indexOf("GridMealReservationFood_") > -1) 
                                if (selectedItem[Grid.columns[Index].field] > 0) {
                                    HasData = true;
                                    break;
                                } 
                    }
                    if (!HasData && SelectedMeal.length >0)
                        ErrorMessage += "در تاریخ " + Date+" غذایی برای ردیف " + selectedItem.ردیف + " انتخاب نشده است \n";
                })
            }

            SelectedGrid.push(JSON.stringify(OfficePersonGrid));
        }
    })

    if (ErrorMessage != "") {
        HideLoader();
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error"); 
    }
    else {

        $.ajax({
            url: "/Food/SaveOfficeReservationFood",
            data: {
                'RecordID': $("#RecordID").val() ,
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#EndDateReservationFood").val(),
                'RequestType': 3,
                'OfficeDepartmentAccountNumber': AccountNumberElement.value(), 
                'Message': $("#OfficeDepartmentAccountNumberMessage").val(),
                'Restaurant': Restaurant.value(),
                'Vehicle': VehiclesElement.value(),
                'Driver': DriverElement.value(), 
                'SelectedMeal': SelectedMeal.join(','),
                'SelectedDey': SelectedDey.join(','),
                'GridJSON': SelectedGrid
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result.RecordID > 0) {
                    popupNotification.show("شماره رزرو : " + result.RecordID + "  <br/>" + result.Message.replace(/\n/g, '<br/>'), "info"); 
                    NewOfficeReservationFood();
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

function ReloadSearchPersonGrid() {
    var grid = $("#SearchPersonGrid").data("kendoGrid");
    grid.dataSource.read({ IsReload: true });

}

function PersonGridSearchInputKeyDown(e) {
    if (e.key == "Enter") {
        var Id = e.currentTarget.id; 
        GridFilterChange("SearchPersonGrid", Id);
    }
}

function OfficePersonGridChange(e) { 
    if ($("#RecordID").val() > 0 && e.model["شناسه_درخواست_غذا"]>0) {
        $.ajax({
            url: "/Food/UpdateOfficeReservationFood",
            data: {
                'ParentID': $("#RecordID").val(),
                'FromDate': e.model['تاریخ'],
                'Person': e.model['شناسه'],
                'Meal': e.sender.columns[e.sender._lastCellIndex].field,
                'MealValue': e.model[e.sender.columns[e.sender._lastCellIndex].field]
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result == "0") {

                }
                else if (result != "")
                {
                    popupNotification.show(result, "error"); 
                }
                else
                {
                    popupNotification.show("ذخیره سازی با موفقیت انجام شد" , "success"); 
                }
            },
            error: function (result) {
                HideLoader();
            }
        })
    }
}

function RemoveRowOfficePersonGrid(e) {
    if ($("#RecordID").val() > 0 && e.model.شناسه_درخواست_غذا >0) {
        ShowLoader();

        $.ajax({
            type: 'POST',
            url: "/Food/RemoveRowOffice",
            data: {
                "RecordID": $("#RecordID").val(),
                "Person": e.model.شناسه,
                "Date": e.model.تاریخ,
                "Bed": e.model.تخت
            },
            dataType: 'json',
            success: function (data) {
                if (data == "") {
                    HideLoader();
                    popupNotification.show("عملیات حذف با موفقیت انجام شد", "success");
                    if (e.model.تخت == true) {
                        var tabStrip = $("#OfficePersonTab").data("kendoTabStrip");
                        $(".OfficePersonTab").each(function (index, Item) {
                            var OfficePersonGrid = $("#" + Item.id.replace("Tab", "OfficePersonGrid")).data("kendoGrid");
                            OfficePersonGrid.dataSource.read();

                            //var oldConfirm = window.confirm;
                            //window.confirm = function () { return true; };
                            //var dataItem = OfficePersonGrid.dataSource.get(e.model.شناسه);
                            //if (dataItem != undefined) { 
                            //    var row = OfficePersonGrid.tbody.find("tr[data-uid='" + dataItem.uid + "']");
                            //    OfficePersonGrid.removeRow(row);
                            //    window.confirm = oldConfirm;
                            //}

                        })
                    } 
                }
                else {
                    popupNotification.show(data, "error");
                    HideLoader();
                    var OfficePersonGrid = $("#" + e.sender.element[0].id).data("kendoGrid");
                    OfficePersonGrid.dataSource.read();
                }
            },
            error: function (result) {
                popupNotification.show(result.responseText, "error");
            }
        });
    }
}