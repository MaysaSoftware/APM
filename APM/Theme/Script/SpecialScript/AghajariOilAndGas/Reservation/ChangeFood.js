function ChangePersonRestaurant(e) {

    var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList");
    var PersonFood = $("#PersonFood").data("kendoDropDownList");

    if (PersonRestaurant.value() > 0) {
        PersonFood.dataSource.read({ Restaurant: PersonRestaurant.value() });
    }
}

function SaveChangePersonFood(e) { 
    ShowLoader();
    var PersonRestaurant = $("#PersonRestaurant").data("kendoDropDownList");
    var PersonFood = $("#PersonFood").data("kendoDropDownList");
    var ErrorMeassage = "";

    if (PersonRestaurant.value() == 0)
        ErrorMeassage += "رستوران انتخاب نشده است";

    if (PersonFood.value() == 0)
        ErrorMeassage += "غذا انتخاب نشده است";

    if (ErrorMeassage != "") { 
        HideLoader();
        popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error");
    }
    else {

        $.ajax({
            url: "/Food/SaveChangePersonFood",
            data: {
                RecordID: $("#RecordID").val(),
                Restaurant: PersonRestaurant.value(),
                Food: PersonFood.value(),
            },
            type: "POST",
            success: function (result) {
                HideLoader();
                if (result == "") { 
                    popupNotification.show("تغییرات با موفقیت ذخیره شد", "success");
                    var Element = e.sender.element[0];
                    while (Element.id.indexOf("EditorForm") == -1 && Element.parentElement != null) {
                        Element = Element.parentElement;
                    }

                    var Win = $("#" + Element.id.split('_')[0].replace("Div", "")).data("kendoWindow");
                    if (Win != undefined)
                    Win.close();
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