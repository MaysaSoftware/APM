


////function PersonRestaurant_Change(e) {
////    var MealValue = e.sender.element[0].id.replace("PersonRestaurant_", "");
////    FillersonFood(MealValue);
////}

////function SaveReservationFood(e) {
////    var myform = document.getElementById("add");
////    var fd = new FormData(myform); 
////    $.ajax({
////        url: "/Food/SavePersonReservationFood/",
////        data: fd,
////        cache: false,
////        processData: false,
////        contentType: false,
////        type: 'POST',
////        success: function (data) { 
////            if (data.ErrorMessage == "") { 
////                var wnd = $("#EditorForm1113").data("kendoWindow");
////                wnd.close();
////            }
////            else {
////                $('.MessageDalert .modal-header').empty();
////                $('.MessageDalert .modal-body').empty();
////                $('.MessageDalert .modal-header').append("خطا");
////                $('.MessageDalert .modal-body').append(data.ErrorMessage.replace(/\n/g, '<br/>'));
////                $('.MessageDalert .modal-content').addClass("ErrorMessageDalert");
////                $('.MessageDalert').modal('show');
////            } 
////        }
////    }); 
////}

////function ReloadPersonFood(e) {
////    var MealValue = e.id.replace("ReloadPersonFood_", "");
////    FillersonFood(MealValue);
////}

////function FillersonFood(MealValue) {
////    var Element = $("#PersonRestaurant_" + MealValue).data("kendoDropDownList");
////    var PersonFood = $("#PersonFood_" + MealValue).data("kendoDropDownList");
////    PersonFood.dataSource.read({ FromDate: $("#StartDateReservationFood").val(), ToDate: $("#EndDateReservationFood").val(), Meal: MealValue, Restaurant: Element.value() });
////    if (Element.value() > 0 && $("#StartDateReservationFood").val() == $("#EndDateReservationFood").val()) {
////        PersonFood.dataSource.read({ FromDate: $("#StartDateReservationFood").val(), ToDate: $("#EndDateReservationFood").val(), Meal: MealValue, Restaurant: Element.value() });
////    }
////    else {
////        PersonFood.dataSource.read();
////    }
////}


////function RequestType_Change(e) {
////    ClearForm();
////    var RequestType = $("#ReservationFoodRequestType").data("kendoDropDownList");
////    $(".ReservationFood").hide();
////    switch (RequestType.value()) {
////        case "1": {
////            $(".MealReservationFood").show();
////            $(".CountReservationFood input").each(function (index, Item) {
////                if (Item.id != "") {
////                    if (Item.id.indexOf("CountFood_") > -1) {
////                        var Element = $("#" + Item.id).data("kendoNumericTextBox");
////                        Element.value(1);
////                        Element.enable(false);
////                        Element.max(1);
////                    }
////                } 
////            })
////            break;
////        }
////        case "2": {
////            $(".MealReservationFood").show();
////            $(".Letter").show();
////            $(".CountReservationFood input").each(function (index, Item) {
////                if (Item.id != "") {
////                    if (Item.id.indexOf("CountFood_") > -1) {
////                        var Element = $("#" + Item.id).data("kendoNumericTextBox");
////                        Element.max(10000);
////                        Element.enable(true);
////                    }
////                }
////            })
////            break;
////        }
////        case "3": {
////            $(".MealReservationFood").show();
////            $(".AccountNumber").show();
////            $(".CountReservationFood input").each(function (index, Item) {
////                if (Item.id != "") {
////                    if (Item.id.indexOf("CountFood_") > -1) {
////                        var Element = $("#" + Item.id).data("kendoNumericTextBox");
////                        Element.value(1);
////                        Element.enable(false);
////                        Element.max(1);
////                    }
////                }
////            })
////            break;
////        }
////    }
////}

////function ClearForm() {
////    $("#OfficePersonGrid").data('kendoGrid').dataSource.data([]);
////    $("#OfficeDepartmentAccountNumber").data("kendoDropDownList").value(0);
////    $("#VehiclesOfficeDepartmentAccountNumber").data("kendoDropDownList").value(0);
////    $("#DriverOfficeDepartmentAccountNumber").data("kendoDropDownList").value(0); 
////    $("#NumberLetter").data("kendoTextBox").value("");
////    $("#SubjectLetter").data("kendoTextBox").value("");
////    $("#OfficeDepartmentAccountNumberMessage").data("kendoTextBox").value("");


////    $(".CountReservationFood input").each(function (index, Item) {
////        if (Item.id != "") {
////            if (Item.id.indexOf("CountFood_") > -1) {
////                var MealValue = Item.id.replace("CountFood_","");
////                $("#" + Item.id).data("kendoNumericTextBox").value(0);
////                $("#PersonRestaurant_" + MealValue).data("kendoDropDownList").value(0);
////                $("#PersonFood_" + MealValue).data("kendoDropDownList").value(0); 
////            }
////        }
////    })
////}

