function LetterRemoveTab(e) {
    var tabstrip = $("#MealTab").data("kendoTabStrip");
    var tab = $(e.currentTarget).closest("li"); 
    var OrginalDate = tab[0].id.replace("Tab", "");
    var Date = OrginalDate.substring(0, 4) + "/" + OrginalDate.substring(4, 6) + "/" + OrginalDate.substring(6, 8);

    if ($("#RecordID").val() > 0 && $("#MealFoodGrid" + OrginalDate).data("kendoGrid").dataSource.view().length > 0) {
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
                    $("#MealFoodGrid" + OrginalDate).data("kendoGrid").dataSource.read();
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

function ShowMealGrid() {
    ShowLoader();
    var ErrorMessage = "";
    var SelectedMeal = []; 
    var SelectedDey = [];
    var SelectedMealTitle= [];
    var SelectedMealValue= [];
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
            SelectedMealValue.push($("#FoodCountReservationFood_" + MealReservationFoodID).data("kendoNumericTextBox").value());
        }
        if (FormInputName[Index].indexOf("WeekDaysReservationFood_") > -1 && FormInputValue[Index] == true) {
            SelectedDey.push(FormInputName[Index].replace("WeekDaysReservationFood_", ""));
        }
    }

    if (SelectedMeal.length==0)
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";


    if ($("#DateLetter").val() == "")
        ErrorMessage += "فیلد تاریخ نامه خالی است \n";

    if ($("#NumberLetter").data("kendoTextBox").value() == "")
        ErrorMessage += "فیلد شماره نامه خالی است \n";     

    if ($("#SubjectLetter").data("kendoTextBox").value() == "")
        ErrorMessage += "فیلد موضوع نامه خالی است \n";     

    if (SelectedMealValue.indexOf(0)>-1)
        ErrorMessage += "فیلد تعداد " + SelectedMealTitle[SelectedMealValue.indexOf(0)]+" وارد نشده است \n";
     
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
                Restaurant: PersonRestaurant.value(),
                SelectedMeal: SelectedMeal.join(','),
                SelectedMealTitle: SelectedMealTitle.join(','),
                SelectedDey: SelectedDey.join(','),
                IsReserveBed: $("#BedReservationFood")[0].checked
            },
            type: "POST",
            success: function (result) {
                if (result == "") { 
                    DisableLetterReservationFoodForm(false);
                    $("#MealReservationForm").load("/Food/LetterMeal", {
                        RecordID: $("#RecordID").val(),
                        FromDate: $("#StartDateReservationFood").val(),
                        ToDate: $("#EndDateReservationFood").val(),
                        Restaurant: PersonRestaurant.value(),
                        SelectedMeal: SelectedMeal.join(','),
                        SelectedMealTitle: SelectedMealTitle.join(','),
                        SelectedDey: SelectedDey.join(','),
                        IsSharingFood: $("#SharingFood")[0].checked,
                        DefualtFoodCount: SelectedMealValue
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


function DisableLetterReservationFoodForm(IsEnable) {
    $("#StartDateReservationFood").prop('disabled', !IsEnable);
    $("#StartDateReservationFoodButton").prop('disabled', !IsEnable);
    $("#EndDateReservationFood").prop('disabled', !IsEnable);
    $("#EndDateReservationFoodButton").prop('disabled', !IsEnable);
    $("#DateLetter").prop('disabled', !IsEnable);
    $("#DateLetterButton").prop('disabled', !IsEnable);
    $("#NumberLetter").data("kendoTextBox").enable(IsEnable);
    $("#SubjectLetter").data("kendoTextBox").enable(IsEnable);
    $("#BedCount").data("kendoNumericTextBox").enable(IsEnable); 

    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").enable(IsEnable);
    $("#ForOffice").data("kendoDropDownList").enable(IsEnable);
    $("#PersonRestaurant").data("kendoDropDownList").enable(IsEnable); 
    $("#BedReservationFood").data("kendoSwitch").enable(IsEnable);
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").enable(IsEnable); 
    $("#ShowMealGrid").data("kendoButton").enable(IsEnable);

    var FormInputName = [];
    var FormInputValue = [];
    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);
    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1) {
            $("#" + FormInputName[Index]).data("kendoSwitch").enable(IsEnable);
        }
        if (FormInputName[Index].indexOf("FoodCountReservationFood_") > -1) {
            $("#" + FormInputName[Index]).data("kendoNumericTextBox").enable(IsEnable);
        }
    }
}


function AddMealFoodGrid(e) {
    var OrginalDate = e.id.replace("AddMealFoodGridButton", "");
    var ErrorMessage = "";
    var SharingFood = $("#SharingFood" + OrginalDate).data("kendoSwitch");
    var MealReservationFood = $("#MealReservationFood" + OrginalDate).data("kendoDropDownList");
    var FoodCountReservationFood = $("#FoodCountReservationFood" + OrginalDate).data("kendoNumericTextBox");
    var RestaurantReservationFood = $("#RestaurantReservationFood" + OrginalDate).data("kendoDropDownList");
    var MealFoodGrid = $("#MealFoodGrid" + OrginalDate).data("kendoGrid");
    var Date = OrginalDate.substring(0, 4) + "/" + OrginalDate.substring(4, 6) + "/" + OrginalDate.substring(6, 8)

    if (MealReservationFood.value() == 0)
        ErrorMessage += "وعده غذایی انتخاب نشده است\n"; 
    if (RestaurantReservationFood.value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است\n";
    if (FoodCountReservationFood.value() == 0 || FoodCountReservationFood.value() == null)
        ErrorMessage += "تعداد غذا انتخاب نشده است\n";
    if (Date < $("#NowDateReservationFood").val() && $("#RecordID").val()>0)
        ErrorMessage += "بدلیل تاریخ گذشته،امکان افزودن وجود ندارد\n";

    if (ErrorMessage != "") { 
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error");
    }
    else { 
        $.ajax({
            url: "/Food/GetPersonFood",
            data: { 
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#StartDateReservationFood").val(),
                'Meal': MealReservationFood.value(),
                'Restaurant': RestaurantReservationFood.value()
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                var elements = jQuery.parseJSON(result);
                if (elements.length > 0) {
                    if (SharingFood.element[0].checked) {
                        var FoodCount = elements.length > 1 ? Math.floor(FoodCountReservationFood.value() / 2) : FoodCountReservationFood.value();
                        var Temp = FoodCount;
                        for (var Index = 0; Index < elements.length; Index++) {
                            var RowCounter = MealFoodGrid.dataSource.view().length;
                            var datasource = MealFoodGrid.dataSource;
                            datasource.add({
                                شناسه : 0,
                                ردیف: RowCounter + 1,
                                تاریخ: Date,
                                GridMealReservationFood: MealReservationFood.value(),
                                GridMealCountReservationFood: FoodCount,
                                GridRestaurantReservationFood: RestaurantReservationFood.value(),
                                GridFoodReservationFood: elements[Index].شناسه
                            });
                            FoodCount = FoodCountReservationFood.value() - Temp; 
                            Temp += FoodCount;
                        }
                    }
                    else {
                        for (var Index = 0; Index < elements.length; Index++) {
                            var RowCounter = MealFoodGrid.dataSource.view().length;
                            var datasource = MealFoodGrid.dataSource;
                            datasource.add({
                                شناسه: 0,
                                ردیف: RowCounter + 1,
                                تاریخ: Date,
                                GridMealReservationFood: MealReservationFood.value(),
                                GridMealCountReservationFood: FoodCountReservationFood.value(),
                                GridRestaurantReservationFood: RestaurantReservationFood.value(),
                                GridFoodReservationFood: elements[Index].شناسه
                            }); 
                        }
                    }
                }
                else { 
                    popupNotification.show("وعده غذایی یافت نشد", "error");
                }
            },
            error: function (result) {
                HideLoader();
            }
        })
    }
}


function AddBedToGrid() {
    var BedGrid = $("#BedGrid").data("kendoGrid");
    var RowCounter = BedGrid.dataSource.view().length;
    var datasource = BedGrid.dataSource;
    datasource.add({
        شناسه: 0,
        پرسنل:0,
        شناسه_درخواست_غذا: 0,
        ردیف: RowCounter + 1,
        نام:"",
        نام_خانوادگی:"",
        شماره_ملی:"",
        تاریخ_تولد:"",
        شماره_تماس:"",
        آدرس:""
    });
}


function PersonLetterOnKeyDown(e) {
    ShowLoader();  
    $.ajax({
        url: "/Food/GetLetterPersonInfo",
        data: { 'SearchValue': e.value },
        type: "POST",
        success: function (result) {
            var elements = jQuery.parseJSON(result);
            if (elements.length > 0) {
                var BedGrid = $("#BedGrid").data("kendoGrid");
                var BedGridData = BedGrid.dataSource.view();
                var RowCounter = BedGridData.length;

                for (var index = 0; index < elements.length; index++) {
                    var flag = true;
                    for (var x = BedGridData.length - 1; x >= 0; x--) {
                        if((BedGridData[x].شناسه == elements[index].شناسه) || BedGridData[x].شماره_ملی == elements[index].کد_ملی )
                            flag = false;
                    }
                    if (flag) {
                        var datasource = BedGrid.dataSource;
                        datasource.add({
                            شناسه:0,
                            پرسنل: elements[index].شناسه,
                            شناسه_درخواست_غذا:0,
                            ردیف: RowCounter + 1,
                            نام: elements[index].نام,
                            نام_خانوادگی: elements[index].نام_خانوادگی,
                            شماره_ملی: elements[index].کد_ملی,
                            تاریخ_تولد: elements[index].تاریخ_تولد,
                            شماره_تماس: elements[index].شماره_تماس,
                            آدرس: elements[index].آدرس,
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

function LoadLetterReservationFood() {
    $("#MainDivWeekDay").hide();
    var BedReservationFood = $("#BedReservationFood").data("kendoSwitch");
    if (!BedReservationFood.element[0].checked) { 
        $("#MainDivBedCount").hide();
        $("#MainBedHeaderTitle").hide();
        $("#MainDivBedGrid").hide();
    }
}


function NewLetterReservationFood() {
    $("#RecordID").val(0);
    $("#BedGrid").data('kendoGrid').dataSource.data([]); 
    $("#StartDateReservationFood").val($("#NowDateReservationFood").val());
    $("#EndDateReservationFood").val($("#NowDateReservationFood").val());
    $("#DateLetter").val("");
    $("#PersonRestaurant").data("kendoDropDownList").value(0);
    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").value(0);
    $("#NumberLetter").data("kendoTextBox").value("");
    $("#SubjectLetter").data("kendoTextBox").value(""); 
    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").value("");
    $("#ForOffice").data("kendoDropDownList").value(0);
    $("#BedCount").data("kendoNumericTextBox").value(0);
    if ($("#BedReservationFood")[0].parentElement.className.indexOf("k-switch-on") > -1) {
        $("#BedReservationFood")[0].checked = false;
        $($("#BedReservationFood")[0].parentElement).removeClass("k-switch-on");
        $($("#BedReservationFood")[0].parentElement).addClass("k-switch-off");
    }
    LoadLetterReservationFood();  
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
        if (FormInputName[Index].indexOf("FoodCountReservationFood_") > -1) {
            $("#" + FormInputName[Index]).data("kendoNumericTextBox").value(0);
            $("#MainDivBedCount_" + FormInputName[Index].replace("FoodCountReservationFood_","")).hide();
        } 
    }
    $("#MealReservationForm").empty();
    DisableLetterReservationFoodForm(true);
}


function BedReservationFood_Load() {
    var Element = $("#BedReservationFood").data("kendoSwitch");
    Element.bind('change', function (e) {
        BedReservationFood_SwitchChange(e.sender.element[0]);
    });
}

function BedReservationFood_SwitchChange(Element) {
    if (Element.checked == true) {
        $("#MainDivBedCount").show();
        $("#MainBedHeaderTitle").show();
        $("#MainDivBedGrid").show();
    }
    else {
        $("#MainDivBedCount").hide();
        $("#MainBedHeaderTitle").hide();
        $("#MainDivBedGrid").hide();
    }
}

function SaveLetterReservationFood() {
    ShowLoader();
    var BedGrid = $("#BedGrid").data().kendoGrid.dataSource.view();
    var BedGridJSON = JSON.stringify(BedGrid);

    var SelectedGrid = [];
    var tabStrip = $("#MealTab").data("kendoTabStrip");

    var AccountNumberElement = $("#OfficeDepartmentAccountNumber").data("kendoDropDownList");
    var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList");
    var ForOffice = $("#ForOffice").data("kendoDropDownList");
    var BedReservationFood = $("#BedReservationFood").data("kendoSwitch");
    var BedCount = $("#BedCount").data("kendoNumericTextBox");

    var ErrorMessage = "";

    var SelectedMeal = []; 
    var SelectedMealTitle = [];
    var SelectedDey = [];
    var FormInputName = [];
    var FormInputValue = [];
    var SelectedMealValue = [];

    GetInputValue("ReservationFoodForm" + " .FormItemInput", FormInputName, FormInputValue);

    for (var Index = 0; Index < FormInputName.length; Index++) {
        if (FormInputName[Index].indexOf("MealReservationFood_") > -1 && FormInputValue[Index] == true) {
            var MealReservationFoodID = FormInputName[Index].replace("MealReservationFood_", "");
            SelectedMeal.push(MealReservationFoodID);
            SelectedMealTitle.push($("#MealTitleReservationFood_" + MealReservationFoodID).text());
            SelectedMealValue.push($("#FoodCountReservationFood_" + MealReservationFoodID).data("kendoNumericTextBox").value());
        }
        if (FormInputName[Index].indexOf("WeekDaysReservationFood_") > -1 && FormInputValue[Index] == true) {
            SelectedDey.push(FormInputName[Index].replace("WeekDaysReservationFood_", ""));
        }
    }


    $(".OfficePersonTab").each(function (index, Item) {
        var OfficePersonGrid = $("#" + Item.id.replace("Tab", "MealFoodGrid")).data().kendoGrid.dataSource.view();
        var Date = Item.id.replace("Tab", "");
        Date = Date.substring(0, 4) + "/" + Date.substring(4, 6) + "/" + Date.substring(6, 8);

        if (OfficePersonGrid.length == 0 && parseInt($("#RecordID").val())  == 0)
            ErrorMessage += "وعده غذایی برای تاریخ " + Date + " انتخاب نشده است \n";
        else
            SelectedGrid.push(JSON.stringify(OfficePersonGrid));

        //var SelectedMealFoodGrid = $("#" + Item.id.replace("Tab", "MealFoodGrid")).data("kendoGrid");
        //var SelectedRows = SelectedMealFoodGrid.table.find("tr");
        //var MealArr = [];
        //var MealCountArr = [];
        //SelectedRows.each(function (index, row) {
        //    var selectedItem = SelectedMealFoodGrid.dataItem(row);
        //    var FindIndex = MealArr.indexOf(selectedItem.GridMealReservationFood);
        //    if (FindIndex > -1)
        //        MealCountArr[FindIndex] += selectedItem.GridMealCountReservationFood;
        //    else {
        //        MealArr.push(selectedItem.GridMealReservationFood);
        //        MealCountArr.push(selectedItem.GridMealCountReservationFood);
        //    }
        //});

        //for (var index = 0; index < MealArr.length; index++) {
        //    if (MealCountArr[index] != SelectedMealValue[SelectedMeal.indexOf(MealArr[index].toString())])
        //        ErrorMessage += "در تاریخ " + Date + " وعده غذایی " + $("#MealTitleReservationFood_" + MealArr[index].toString()).text() + " با تعداد وارد شده در بالا برابر نیست \n";
        //}
    })



    if (AccountNumberElement.value() == 0)
        ErrorMessage += "شماره حساب وارد نشده است \n";

    if ($("#StartDateReservationFood").val() > $("#EndDateReservationFood").val())
        ErrorMessage += "تاریخ شروع باید کوچکتر از تاریخ پایان باشد.\n";

    if ($("#StartDateReservationFood").val() == "" || $("#EndDateReservationFood").val() == "")
        ErrorMessage += "تاریخ نباید خالی باشد.\n";

    if (PersonRestaurant.value() == 0)
        ErrorMessage += "رستوران انتخاب نشده است \n"; 

    if ($("#DateLetter").val() == "")
        ErrorMessage += "فیلد تاریخ نامه خالی است \n";

    if ($("#NumberLetter").data("kendoTextBox").value() == "")
        ErrorMessage += "فیلد شماره نامه خالی است \n";

    if ($("#SubjectLetter").data("kendoTextBox").value() == "")
        ErrorMessage += "فیلد موضوع نامه خالی است \n";

    if (BedReservationFood.element[0].checked) {
        if (BedGridJSON == "[]") {
            ErrorMessage += "اطلاعات شخصی برای رزرو تخت، وارد نشده است \n";
        }
        if (BedCount.value() != null) { 
            if (BedCount.value() != BedGrid.length) {
                ErrorMessage += "اطلاعات شخصی برای رزرو تخت، وارد نشده است \n";
            }
            if (BedCount.value() == 0)
                ErrorMessage += "تعداد تخت، وارد نشده است \n";


            var SelectedBedGrid = $("#BedGrid").data("kendoGrid");
            var SelectedRows = SelectedBedGrid.table.find("tr");
            var NationalCodeArr = [];
            SelectedRows.each(function (index, row) {
                var selectedItem = SelectedBedGrid.dataItem(row);
                if (selectedItem.شماره_ملی == "")
                    ErrorMessage += "شماره ملی در ردیف " + selectedItem.ردیف + " خالی است\n";
                else if (NationalCodeArr.indexOf(selectedItem.شماره_ملی) > -1)
                    ErrorMessage += "شماره ملی در ردیف " + selectedItem.ردیف + " تکراری است\n";
                else
                    NationalCodeArr.push(selectedItem.شماره_ملی); 
            });

        }
    }


    if (SelectedMealValue.indexOf(0) > -1)
        ErrorMessage += "فیلد تعداد " + SelectedMealTitle[SelectedMealValue.indexOf(0)] + " وارد نشده است \n";

    if ((SelectedMeal.length == 0 && BedGrid.length == 0 )|| (SelectedGrid.length == 0 && SelectedMeal.length > 0))
        ErrorMessage += "وعده غذایی انتخاب نشده است \n";

 


    if (ErrorMessage != "") {
        HideLoader(); 
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error");
    }
    else {

        $.ajax({
            url: "/Food/SaveLetterReservationFood",
            data: {
                'RecordID': $("#RecordID").val(),
                'FromDate': $("#StartDateReservationFood").val(),
                'ToDate': $("#EndDateReservationFood").val(),
                'RequestType': 2,
                'OfficeDepartmentAccountNumber': AccountNumberElement.value(),
                'DateLetter': $("#DateLetter").val(),
                'NumberLetter': $("#NumberLetter").data("kendoTextBox").value(),
                'SubjectLetter': $("#SubjectLetter").data("kendoTextBox").value(),
                'ForOffice': ForOffice.value(),
                'Message': $("#OfficeDepartmentAccountNumberMessage").val(),
                'BedCount': BedCount.value(),
                'SelectedDey': SelectedDey.join(','),
                'GridJSON': SelectedGrid,
                'BedGridJSON': BedGridJSON,
                'Restaurant': PersonRestaurant.value(),
                'SelectedMeal': SelectedMeal.join(','),
                'SelectedMealValue': SelectedMealValue.join(','),
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result.RecordID > 0) { 
                    popupNotification.show("شماره رزرو : " + result.RecordID + "  <br/>" + result.Message.replace(/\n/g, '<br/>'), "info");
                    NewLetterReservationFood();
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


function RemoveLetterBed(e) {
    if ($("#RecordID").val() > 0 && e.model.شناسه > 0) {
        ShowLoader();

        $.ajax({
            type: 'POST',
            url: "/Food/RemoveLetterBed",
            data: { 
                "RecordID": e.model.شناسه
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


function LetterMealFoodGrid_CellClose(e) {
    if ($("#RecordID").val() > 0 && e.model["شناسه"] > 0)
    {
        ShowLoader();

        var ErrorMessage = "";
        var Date = e.sender.element[0].id.replace("MealFoodGrid", "");
        Date = Date.substring(0, 4) + "/" + Date.substring(4, 6) + "/" + Date.substring(6, 8)
        var SelectedMealFoodGrid = $("#" + e.sender.element[0].id).data("kendoGrid");
        var SelectedRows = SelectedMealFoodGrid.table.find("tr");
        var MealArr = [];
        var MealCountArr = [];
        SelectedRows.each(function (index, row) {
            var selectedItem = SelectedMealFoodGrid.dataItem(row);
            var FindIndex = MealArr.indexOf(selectedItem.GridMealReservationFood);
            if (FindIndex > -1)
                MealCountArr[FindIndex] += selectedItem.GridMealCountReservationFood;
            else {
                MealArr.push(selectedItem.GridMealReservationFood);
                MealCountArr.push(selectedItem.GridMealCountReservationFood);
            }
        });

        for (var index = 0; index < MealArr.length; index++) {
            var MealTitleReservationFood = $("#MealTitleReservationFood_" + MealArr[index].toString()).text();
            var FoodCountReservationFood = $("#FoodCountReservationFood_" + MealArr[index].toString()).data("kendoNumericTextBox").value();
          
            //if (MealCountArr[index] != FoodCountReservationFood)
            //    ErrorMessage += "در تاریخ " + Date + " وعده غذایی " + MealTitleReservationFood+ " با تعداد وارد شده در بالا برابر نیست \n";
        }

        if (ErrorMessage == "") {

            $.ajax({
                url: "/Food/UpdateLetterMealFood",
                data: {
                    'RecordID': e.model["شناسه"],
                    'CountReservationFood': e.model["GridMealCountReservationFood"]
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
        else {
            HideLoader();
            popupNotification.show(ErrorMessage, "error");
        }

    }
}

function LetterMealFoodGrid_Remove(e) { 
    if ($("#RecordID").val() > 0 && e.model["شناسه"] > 0) {
        ShowLoader();

        $.ajax({
            url: "/Food/RemoveLetterMealFood",
            data: {
                'RecordID': e.model["شناسه"]
            },
            type: 'POST',
            success: function (result) {
                HideLoader();
                if (result == "0") {

                }
                else if (result != "") { 
                    popupNotification.show(result, "error");
                    $("#" + e.sender.element[0].id).data("kendoGrid").dataSource.read();
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
