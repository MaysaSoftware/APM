$(document).on("keyup", ".FormItemInput *", function (e) {

    if (e.keyCode == 32) {
        if ($("#" + e.currentTarget.id).hasClass("DatePickers")) {
            var Item = e.currentTarget;

            $.ajax({
                url: "/Home/GetShamsiDate",
                data: {},
                type: "POST",
                success: function (Result) {
                    Item.value = Result;
                    CheckDate(Item);
                    jalaliDatepicker.hide();
                },
                error: function (result) {

                }
            })
        }
        else if ($("#" + e.currentTarget.id).hasClass("MildayDatePickers")) {
            var Item = e.currentTarget;

            $.ajax({
                url: "/Home/GetMiladiDate",
                data: {},
                type: "POST",
                success: function (Result) {
                    Item.value = Result;
                    CheckMiladiDate(Item);
                    MiladiDatepicker.hide();
                },
                error: function (result) {

                }
            })
        }
    }
    else if (e.keyCode == 13) {

        if ($("#" + e.currentTarget.id).hasClass("DatePickers"))
            CheckDate(e.currentTarget);
        else if ($("#" + e.currentTarget.id).hasClass("MildayDatePickers"))
            CheckMiladiDate(e.currentTarget);
        else if ($("#" + e.currentTarget.id).hasClass("PlaqueInput"))
            Plaqueonkeydown(e.currentTarget);
        else if ($("#" + e.currentTarget.id).hasClass("TimeInput"))
            CheckTime(e.currentTarget);
    }
    else if ($("#" + e.currentTarget.id).hasClass("PlaqueInput"))
        Plaqueonkeydown(e.currentTarget);
})

function DatePickers_onfocusout(e) {
    //var $focused = $(':focus');
    //var focused = document.activeElement;
    //if ($("#" + e.id).hasClass("MildayDatePickers"))
    //    CheckMiladiDate(e);
    //else if ($("#" + e.id).hasClass("DatePickers"))
    //    jalaliDatepicker.hide();
    //CheckDate(e);
}


function MousedownPassword(item) {
    $("#" + item.id).attr("type", "text");
}

function MouseupPassword(item) {
    $("#" + item.id).attr("type", "password");
}


function AddListenerCalendarWithOnKeyDown(item) {
    document.getElementById(item.id).addEventListener("jdp:change", function (e) { SelectCalendarDate(e) });
    document.getElementById(item.id).addEventListener("change", function (e) { OnKeyDownElement(e) });
}

function AddListenerCalendar(item) {
    if (item.id != undefined) {
        document.getElementById(item.id).addEventListener("jdp:change", function (e) { SelectCalendarDate(e) });
    }

}

function AddListenerMiladiCalendar(item) {
    if (item.id != undefined)
        document.getElementById(item.id).addEventListener("mdp:change", function (e) { SelectMiladiCalendarDate(e) });
}

function ConvertInputToMiladyDatePicker(Item) {
    $("#" + Item.id)[0].datepicker({
        changeMonth: true,
        changeYear: true
    });
}

function SelectMiladiCalendarDate(e) {
    CheckMiladiDate(e.target);
}

function SelectCalendarDate(e) {
    CheckDate(e.target);
}


function CalenderClick(e) {
    jalaliDatepicker.show(e);
}

function MiladiCalenderClick(e) {
    MiladiDatepicker.show(e);
}

function CheckDate(e) {
    var DateResult = CheckValidDate(e.value);
    if (DateResult == false) {
        popupNotification.show("فرمت تاریخ صحیح نمی باشد", "error");
        e.value = "";
        e.focus();
    }
    else if (DateResult == undefined) {
        e.value = "";
    }
    else {
        if (e.id.indexOf("شمسی") > -1) {
            var MiladiName = e.id.replace("شمسی", "میلادی")
            var obj = $("#" + MiladiName)
            if (obj != undefined) {
                var date = DateResult.split("/");
                var MiladiDate = new JalaliDate();
                MiladiDate = MiladiDate.jalaliToGregorian(date[0], date[1], date[2]);
                var ConvertValue = MiladiDate[0] + "/" + (MiladiDate[1] < 10 ? "0" + MiladiDate[1] : + MiladiDate[1]) + "/" + (MiladiDate[2] < 10 ? "0" + MiladiDate[2] : + MiladiDate[2]);
                obj[0].value = ConvertValue;
                obj[0].focus();
            }
        }
        if (e.parentElement.parentElement.className != undefined) {
            if (e.parentElement.parentElement.className.indexOf("GridIncell_") > -1) {
                var FormArr = e.parentElement.parentElement.className.split('_');
                var DataKey = FormArr[1];
                var ParentID = FormArr[2];
                var RecordID = FormArr[3];
                var grid = (ParentID == 0 ? $("#MainGrid" + DataKey).data("kendoGrid") : $("#DetailMainGrid" + DataKey + "_" + ParentID).data("kendoGrid"));
                if (grid != undefined) {
                    if (grid.options.editable.mode != "popup") {

                    }
                } 
            }
        }
        e.value = DateResult;
        jalaliDatepicker.hide();
    }
}


function CheckMiladiDate(e) {
    if (e.value != undefined) {

        var DateResult = CheckValidMiladiDate(e.value);
        if (DateResult == false) {
            popupNotification.show("فرمت تاریخ صحیح نمی باشد", "error");
            e.value = "";
            e.focus();
        }
        else if (DateResult == undefined) {
            e.value = "";
        }
        else {
            var ElementName = e.id.replace("میلادی", "شمسی")
            var Element = $("#" + ElementName)
            if (Element != undefined) {
                var date = DateResult.split("/");
                var _JalaliDate = new JalaliDate();
                var ConvertDate = _JalaliDate.gregorianToJalali(date[0], date[1], date[2]);
                var ConvertValue = ConvertDate[0] + "/" + (ConvertDate[1] < 10 ? "0" + ConvertDate[1] : + ConvertDate[1]) + "/" + (ConvertDate[2] < 10 ? "0" + ConvertDate[2] : + ConvertDate[2]);
                Element[0].value = ConvertValue;
            }
            e.value = DateResult;
            MiladiDatepicker.hide();
        }
    }
}

function CheckValidDate(DateText) {

    if (DateText != undefined) {
        if (DateText.length > 0) {
            var date = DateText.split("/");
            if (date.length == 1) {
                if (DateText.length == 6) {
                    var Year = DateText.substring(0, 2)
                    var Month = DateText.substring(2, 4);
                    var Day = DateText.substring(4, 6);
                    if (Month < 1 || Month > 12 || Day < 1 || Day > 31 || (Month > 6 && Day > 30)) {
                        return false;
                    }
                    else {
                        var DateNow = new Date();
                        if (DateNow.getFullYear() + "/" + (DateNow.getMonth() + 1) + "/" + DateNow.getDate() >= "2021/3/21") {
                            Year = "14" + Year;
                        }
                        else {
                            Year = "13" + Year;
                        }
                        return Year + "/" + Month + "/" + Day;
                    }
                }
                else if (DateText.length == 8) {
                    var Year = DateText.substring(0, 4)
                    var Month = DateText.substring(4, 6);
                    var Day = DateText.substring(6, 8);
                    if (Month < 1 || Month > 12 || Day < 1 || Day > 31 || (Month > 6 && Day > 30)) {
                        return false;
                    }
                    else {
                        return Year + "/" + Month + "/" + Day;
                    }
                }
                else {
                    return false;
                }
            }
            else if (date.length == 3) {
                var Year = date[0];
                var Month = date[1];
                var Day = date[2];
                if (Month < 0 || Month > 12 || Day < 0 || Day > 31 || (Month > 6 && Day > 30) || Year.length > 4) {
                    return false;
                }
                else {
                    if (Month < 10 && Month.length == 1) {
                        Month = "0" + Month;
                    }
                    if (Day < 10 && Day.length == 1) {
                        Day = "0" + Day;
                    }
                    if (Year.length == 2) {
                        var DateNow = new Date();
                        if (DateNow.getFullYear() + "/" + (DateNow.getMonth() + 1) + "/" + DateNow.getDate() >= "2020/3/21") {
                            Year = "14" + Year;
                        }
                        else {
                            Year = "13" + Year;
                        }
                    }
                    return Year + "/" + Month + "/" + Day;
                }

            }
            else {
                return false;
            }
        }
    }
}

function CheckValidTime(TimeText) {
    if (TimeText.length > 0) {
        var Time = TimeText.split(":");
        if (Time.length > 0) {
            if (TimeText.length == 4 && Time.length == 1) {
                var Hour = TimeText.substring(0, 2)
                var Min = TimeText.substring(2, 4)
                if (Hour >= 0 && Hour <= 23) {
                    if (Hour.length == 1) {
                        Hour = "0" + Hour
                    }
                }
                else
                    return false;
                if (Min >= 0 && Min <= 59) {
                    if (Min.length == 1) {
                        Min = "0" + Hour
                    }
                }
                else
                    return false;
                return Hour + ":" + Min + ":" + "00";
            }
            else if (Time.length == 1) {
                var Hour = Time[0];
                if (Hour >= 0 && Hour <= 23) {
                    if (Hour.length == 1) {
                        return "0" + Hour + ":00:00"
                    }
                    else {
                        return Hour + ":00:00"
                    }
                }
                else {
                    return false;
                }
            }
            else if (Time.length == 2) {
                var Hour = Time[0];
                var Minutes = Time[1];
                if ((Hour >= 0 && Hour <= 23) && (Minutes >= 0 && Minutes <= 59)) {
                    if (Minutes < 10 && Minutes.length == 1) {
                        Minutes = "0" + Minutes;
                    }
                    if (Hour < 10 && Hour.length == 1) {
                        Hour = "0" + Hour;
                    }
                    return Hour + ":" + Minutes + ":00";
                }
                else {
                    return false;
                }
            }
            else if (Time.length == 3) {
                var Hour = Time[0];
                var Minutes = Time[1];
                var Seconds = Time[2];
                if ((Hour >= 0 && Hour <= 23) && (Minutes >= 0 && Minutes <= 59) && (Seconds >= 0 && Seconds <= 59)) {
                    if (Seconds < 10 && Seconds.length == 1) {
                        Seconds = "0" + Seconds;
                    }
                    if (Minutes < 10 && Minutes.length == 1) {
                        Minutes = "0" + Minutes;
                    }

                    if (Hour < 10 && Hour.length == 1) {
                        Hour = "0" + Hour;
                    }
                    return Hour + ":" + Minutes + ":" + Seconds;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
}

function CheckTime(e) {
    var TimeResult = CheckValidTime(e.value);
    if (TimeResult == false) {
        popupNotification.show('"فرمت ساعت صحیح نمی باشد"', "error");
        e.value = "";
        e.focus();
        return false;
    }
    else if (TimeResult == undefined) {
        e.value = "";
    }
    else {
        e.value = TimeResult;
    }
}



function CheckValidMiladiDate(DateText) {

    if (DateText != undefined) {

        if (DateText.length > 0) {
            var date = "";
            if (DateText.indexOf("/") > -1)
                date = DateText.split("/");
            else if (DateText.indexOf("-") > -1)
                date = DateText.split("-");

            if (date.length == 0) {
                if (DateText.length == 6) {
                    var Year = DateText.substring(0, 2)
                    var Month = DateText.substring(2, 4);
                    var Day = DateText.substring(4, 6);
                    if (Month < 1 || Month > 12 || Day < 1 || Day > 31 || (Month > 6 && Day > 30)) {
                        return false;
                    }
                    else {
                        var DateNow = new Date();
                        if (DateNow.getFullYear() + "/" + (DateNow.getMonth() + 1) + "/" + DateNow.getDate() >= "2021/3/21") {
                            Year = "20" + Year;
                        }
                        else {
                            Year = "13" + Year;
                        }
                        return Year + "/" + Month + "/" + Day;
                    }
                }
                else {
                    return false;
                }
            }
            else if (date.length == 3) {
                var Year = date[0];
                var Month = date[1];
                var Day = date[2];
                if (Month < 0 || Month > 12 || Day < 0 || Day > 31 || (Month == 2 && Day > 28) || ((Month == 4 || Month == 6 || Month == 9 || Month == 11) && Day > 30) || Year.length > 4) {
                    return false;
                }
                else {
                    if (Month < 10 && Month.length == 1) {
                        Month = "0" + Month;
                    }
                    if (Day < 10 && Day.length == 1) {
                        Day = "0" + Day;
                    }
                    if (Year.length == 2) {
                        var DateNow = new Date();
                        if (DateNow.getFullYear() + "/" + (DateNow.getMonth() + 1) + "/" + DateNow.getDate() >= "2020/3/21") {
                            Year = "20" + Year;
                        }
                        else {
                            Year = "13" + Year;
                        }
                    }
                    return Year + "/" + Month + "/" + Day;
                }

            }
            else {
                return false;
            }
        }
    }
}


function Plaqueonkeydown(e) {
    if (e.value != undefined) {
        var InputValue = e.value.replace(/ /g, "");
        InputValue = InputValue.replace(/ایران/g, "");
        var CharArraye = Object.assign([], InputValue);
        var Result = "";
        $.each(CharArraye, function (index, value) {
            if (value != String.fromCharCode(8206))
                Result += value;
        });

        InputValue = Result;

        if (event.keyCode == 13) {
            //$("#MachinModel").focus();
        }
        else if (CheckNumricEvent(event.keyCode) == true || CheckAlpha(event.keyCode) == true) {
            if (InputValue.length == 1 && CheckNumricEvent(event.keyCode) == true && $.isNumeric(InputValue.substring(0, 1)) == true) {
                e.value = e.value;
            }
            else if (InputValue.length == 2 && $.isNumeric(InputValue.substring(0, 2))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ';
            }
            else if (InputValue.length == 3 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ';
            }
            else if (InputValue.length == 4 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3)) && $.isNumeric(InputValue.substring(3, 4))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 4);
            }
            else if (InputValue.length == 5 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3)) && $.isNumeric(InputValue.substring(3, 5))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 5);
            }
            else if (InputValue.length == 6 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3)) && $.isNumeric(InputValue.substring(3, 6))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ';
            }
            else if (InputValue.length == 7 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3)) && $.isNumeric(InputValue.substring(3, 6)) && $.isNumeric(InputValue.substring(6, 7))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ' + InputValue.substring(6, 7);
            }
            else if (InputValue.length == 7 && isNaN(event.key) && !isNaN(InputValue.substring(0, 2)) && !isNaN(InputValue.substring(2, 5)) && !isNaN(InputValue.substring(5, 7))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(5, 7);
            }
            else {

            }

        }
        else if (event.keyCode == 8) {
            if (InputValue.length == 3 && $.isNumeric(InputValue.substring(0, 1)) && $.isNumeric(InputValue.substring(1, 2))) {
                e.value = InputValue.substring(0, 2);
            }
            else if (InputValue.length == 4 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 4);
            }
            else if (InputValue.length == 5 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3)) && $.isNumeric(InputValue.substring(3, 4))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 5);
            }
            else if (InputValue.length == 6 && $.isNumeric(InputValue.substring(0, 2)) && !$.isNumeric(InputValue.substring(2, 3)) && $.isNumeric(InputValue.substring(3, 5))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6);
            }
            else if (InputValue.length == 7 && $.isNumeric(InputValue.substring(0, 1)) && $.isNumeric(InputValue.substring(1, 2)) && $.isNumeric(InputValue.substring(2, 3)) == false && $.isNumeric(InputValue.substring(3, 4)) && $.isNumeric(InputValue.substring(4, 5)) && $.isNumeric(InputValue.substring(5, 6))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ' + InputValue.substring(6, 7);
            }
            else if (InputValue.length == 8 && $.isNumeric(InputValue.substring(0, 1)) && $.isNumeric(InputValue.substring(1, 2)) && $.isNumeric(InputValue.substring(2, 3)) == false && $.isNumeric(InputValue.substring(3, 4)) && $.isNumeric(InputValue.substring(4, 5)) && $.isNumeric(InputValue.substring(5, 6)) && $.isNumeric(InputValue.substring(6, 7))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ' + InputValue.substring(6, 8);
            }
            else if (InputValue.length == 9 && $.isNumeric(InputValue.substring(0, 1)) && $.isNumeric(InputValue.substring(1, 2)) && $.isNumeric(InputValue.substring(2, 3)) == false && $.isNumeric(InputValue.substring(3, 4)) && $.isNumeric(InputValue.substring(4, 5)) && $.isNumeric(InputValue.substring(5, 6)) && $.isNumeric(InputValue.substring(6, 7)) && $.isNumeric(InputValue.substring(7, 8))) {
                e.value = InputValue.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ' + InputValue.substring(6, 8);
            }
            else {

            }
        }
        PlaqueStyle(e)
    }
}


function CheckAlpha(e) {
    if ((e >= 65 && e <= 90) || (e >= 219 && e <= 222) || e == 188 || e == 186) {
        return true
    }
    else
        return false
}

function CheckNumricEvent(e) {
    if ((e >= 96 && e <= 105) || (e >= 48 && e <= 57)) {
        return true
    }
    else
        return false
}

function CheckNumricValid(e) {
    if (isNaN(e.value) == true) {
        popupNotification.show("مقدار وارد شده صحیح نمی باشد", "error");
        e.value = "";
        e.focus();
    }
}

function GetInputValue(Element, FormInputName, FormInputValue) {
    //var inputs = $("#" + Element).find("input:not([type='hidden']), textarea:enabled ,.MultiSelect"); 
    var inputs = $("#" + Element).find("input:not([type='hidden']), textarea ,.MultiSelect");


    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].id != "" && inputs[i].id != undefined) {
            FormInputName.push(inputs[i].id);
            if (inputs[i].type == "checkbox")
                FormInputValue.push(inputs[i].checked);
            else if (inputs[i].type == "select-multiple") {
                var Item = $("#" + inputs[i].id).data("kendoMultiSelect");
                FormInputValue.push(Item.value().join(','));
            }
            else if ($("#" + inputs[i].id).data("kendoDropDownTree") != undefined) {
                FormInputValue.push($("#" + inputs[i].id).data("kendoDropDownTree").value())
            }
            else {
                if (inputs[i].className.indexOf("ImageUpload_") > -1) {
                    FormInputValue.push($("." + inputs[i].classList[0]).val())
                }
                else {
                    FormInputValue.push(inputs[i].value);
                }
            }
            //FormInputValue.push(inputs[i].value.replaceAll("<","\*"));
        }
        else if (inputs[i].name != "" && inputs[i].name != undefined) {
            FormInputName.push(inputs[i].name);
            FormInputValue.push(inputs[i].value);
        }
    }
}


function CheckRequiredField(FormDiv, DataKey) { 
    var RequireMessage = "";
    var RequiredField = [];
    var ReplaceWorde = "_" + DataKey;

    $("#" + FormDiv + " input,textarea,select,dropdownlist").filter('[required]:visible').each(async function (index, ElementField) {
        if (ElementField.value == "") {
            $(ElementField.parentElement).addClass("RequiredField");

            if (ElementField.id.indexOf("SearchField_") > -1) {
                var FieldArr = ElementField.id.split('_');
                ReplaceWorde = FieldArr[FieldArr.length - 5] + "_" + FieldArr[FieldArr.length - 4] + "_" + FieldArr[FieldArr.length - 3] + "_" + FieldArr[FieldArr.length - 2] + "_" + FieldArr[FieldArr.length - 1]
            }

            RequireMessage += ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ') + " پر نمایید" + "\n";
            RequiredField.push(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' '));
        }
        else
            $(ElementField.parentElement).removeClass("RequiredField");
    })


    $("#" + FormDiv + " .PasswordField").filter('input:visible').each(async function (index, ElementField) {
        if (ElementField.value != "") {
            var PasswordMessage = checkPasswordStrength(ElementField.value);
            if (PasswordMessage != "") {

                if (ElementField.id.indexOf("SearchField_") > -1) {
                    var FieldArr = ElementField.id.split('_');
                    ReplaceWorde = FieldArr[FieldArr.length - 5] + "_" + FieldArr[FieldArr.length - 4] + "_" + FieldArr[FieldArr.length - 3] + "_" + FieldArr[FieldArr.length - 2] + "_" + FieldArr[FieldArr.length - 1]
                }

                RequireMessage += ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ') + " : " + PasswordMessage + "\n";
                RequiredField.push(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' '));
            }
        }
    })


    $("#" + FormDiv + " .IsRequired").each(async function (index, ElementField) {
        if (ElementField.id != undefined && ElementField.name != undefined)
            if (((ElementField.id == "" && (ElementField.name != "" && ElementField.name != undefined)) || ElementField.id != "") && RequiredField.indexOf(ElementField.name.replace(ReplaceWorde, "").replaceAll('_', ' ').replaceAll('input', ' ')) == -1) {
                if ((ElementField.value == "" || ElementField.value == "0")) {
                    if (($(ElementField).is(":visible") == false && $(ElementField.parentElement).is(":visible") == true) || $(ElementField).is(":visible") == true) {
                        $(ElementField.parentElement).addClass("RequiredField");

                        if (ElementField.id.indexOf("SearchField_") > -1) {
                            var FieldArr = ElementField.id.split('_');
                            ReplaceWorde = FieldArr[FieldArr.length - 5] + "_" + FieldArr[FieldArr.length - 4] + "_" + FieldArr[FieldArr.length - 3] + "_" + FieldArr[FieldArr.length - 2] + "_" + FieldArr[FieldArr.length - 1]
                        }

                        if (ElementField.id == "") {
                            RequireMessage += ElementField.name.replace(ReplaceWorde, "").replaceAll('_', ' ').replaceAll('input', ' ') + " پر نمایید" + "\n";
                            RequiredField.push(ElementField.name.replace(ReplaceWorde, "").replaceAll('_', ' ').replaceAll('input', ' '));
                        }
                        else {
                            RequireMessage += ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ') + " پر نمایید" + "\n";
                            RequiredField.push(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' '));
                        }
                    }
                }
                else
                    $(ElementField.parentElement).removeClass("RequiredField");
            }
    })

    $("#" + FormDiv + " .DatePickers").each(async function (index, ElementField) {
        if (RequiredField.indexOf(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ')) == -1) {
            var NationalCodeError = CheckValidDate(ElementField.value);
            if (NationalCodeError == false) {
                $(ElementField).addClass("RequiredField");

                if (ElementField.id.indexOf("SearchField_") > -1) {
                    var FieldArr = ElementField.id.split('_');
                    ReplaceWorde = FieldArr[FieldArr.length - 5] + "_" + FieldArr[FieldArr.length - 4] + "_" + FieldArr[FieldArr.length - 3] + "_" + FieldArr[FieldArr.length - 2] + "_" + FieldArr[FieldArr.length - 1]
                }
                RequireMessage += ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ') + " : " + "فرمت تاریخ صحیح نمی باشد" + "\n";
                RequiredField.push(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' '));
            }
            else {
                if (NationalCodeError != undefined) {
                    ElementField.value = NationalCodeError;
                    $(ElementField).removeClass("RequiredField");
                }
            }
        }
    })

    //$("#" + FormDiv + " .TimeInput").each(async function (index, ElementField) {
    //    if (RequiredField.indexOf(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ')) == -1) { 
    //        var NationalCodeError = CheckValidTime(ElementField.value);
    //        if (NationalCodeError == false) {
    //            $(ElementField.parentElement).addClass("RequiredField");
    //            RequireMessage += ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ') + " : " + "فرمت ساعت صحیح نمی باشد" + "\n";
    //            RequiredField.push(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' '));
    //        }
    //        else {
    //            if (NationalCodeError != undefined) {
    //                ElementField.value = NationalCodeError;
    //                $(ElementField.parentElement).removeClass("RequiredField");
    //            }
    //        }
    //    }
    //})


    $("#" + FormDiv + " .NationalCode").filter('input:visible').each(async function (index, ElementField) {
        if (RequiredField.indexOf(ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ')) == -1) {
            var NationalCodeError = CheckNationalCode(ElementField.value);
            if (NationalCodeError != "") {
                $(ElementField.parentElement).addClass("RequiredField");
                if (ElementField.id.indexOf("SearchField_") > -1) {
                    var FieldArr = ElementField.id.split('_');
                    ReplaceWorde = FieldArr[FieldArr.length - 5] +"_" + FieldArr[FieldArr.length - 4] + "_" + FieldArr[FieldArr.length - 3] + "_" + FieldArr[FieldArr.length - 2] + "_" + FieldArr[FieldArr.length-1]
                }
                RequireMessage += ElementField.id.replace(ReplaceWorde, "").replaceAll('_', ' ') + " : " + NationalCodeError + "\n";
            }
            else
                $(ElementField.parentElement).removeClass("RequiredField");
        }
    })
    RequireMessage = RequireMessage.replace("SearchField", "");
    return RequireMessage;
}

function GeneratePassword(Item) {
    var Data = "a-z,A-Z,0-9,#";
    var dataSet = Data.split(',');
    var possible = '';
    if ($.inArray('a-z', dataSet) >= 0) {
        possible += 'abcdefghijklmnopqrstuvwxyz';
    }
    if ($.inArray('A-Z', dataSet) >= 0) {
        possible += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    }
    if ($.inArray('0-9', dataSet) >= 0) {
        possible += '0123456789';
    }
    if ($.inArray('#', dataSet) >= 0) {
        possible += '![]{}()%&*$#^~@|';
    }
    var text = '';
    for (var i = 0; i < 12; i++) {
        text += possible.charAt(Math.floor(Math.random() * possible.length));
    }
    Item.value = text;
    $("#" + Item.id).attr("type", "text");
}



function checkPasswordStrength(Password) {
    var number = /([0-9])/;
    var alphabets = /([a-zA-Z])/;
    var special_characters = /([~,!,@,#,$,%,^,&,*,-,_,+,=,?])/;
    var password = Password.trim();
    if (password.length < 6) {
        return "کلمه عبور بسیار ضعیف است. کلمه عبور باید حداقل 6 کارکتر داشته باشد";
    } else {
        if (password.match(number) == null)
            return "مقدار وارد شده حداقل باید یک عدد داشته باشد";
        else if (password.match(alphabets) == null)
            return "مقدار وارد شده حداقل باید یک حرف کوچک و بزرگ انگلیسی داشته باشد";
        else if (password.match(special_characters) == null)
            return "مقدار وارد شده حداقل باید یک کارکتر خاص مانند ~,!,@,#,$,%,^,&,*,-,_,+,=,? داشته باشد";
        else if (password.match(number) && password.match(alphabets) && password.match(special_characters)) {
            return "";
        }
    }
}

function CheckNationalCode(nationalCode) {
    if (nationalCode.length == 0)
        return "";
    if (nationalCode.length != 10)
        return "طول کد ملی باید ده کاراکتر باشد";

    if (!isNumeric(nationalCode))
        return "کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید";

    var allDigitEqual = ["0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999"];


    if (jQuery.inArray(nationalCode, allDigitEqual) !== -1) return "لطفا کد ملی را صحیح وارد نمایید";

    var chArray = nationalCode.split('');
    var num0 = chArray[0] * 10;
    var num2 = chArray[1] * 9;
    var num3 = chArray[2] * 8;
    var num4 = chArray[3] * 7;
    var num5 = chArray[4] * 6;
    var num6 = chArray[5] * 5;
    var num7 = chArray[6] * 4;
    var num8 = chArray[7] * 3;
    var num9 = chArray[8] * 2;
    var a = chArray[9];

    var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
    var c = b % 11;

    return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a))) ? "" : "کد ملی وارد شده صحیح نمی باشد";
}

function GetCityOfNationalCode(Code) {
    if (Code != "")
        switch (Code.substring(0, 3)) {
            case "169": return "آذربایجان شرقی - آذرشهر";
            case "170": return "آذربایجان شرقی - اسکو";
            case "149": return "آذربایجان شرقی - اهر";
            case "150": return "آذربایجان شرقی - اهر";
            case "171": return "آذربایجان شرقی - بستان آباد";
            case "168": return "آذربایجان شرقی - بناب";
            case "136": return "آذربایجان شرقی - تبریز";
            case "137": return "آذربایجان شرقی - تبریز";
            case "138": return "آذربایجان شرقی - تبریز";
            case "545": return "آذربایجان شرقی - ترکمانچای";
            case "505": return "آذربایجان شرقی - جلفا";
            case "636": return "آذربایجان شرقی - چاروایماق";
            case "164": return "آذربایجان شرقی - سراب";
            case "165": return "آذربایجان شرقی - سراب";
            case "172": return "آذربایجان شرقی - شبستر";
            case "623": return "آذربایجان شرقی - صوفیان";
            case "506": return "آذربایجان شرقی - عجب شیر";
            case "519": return "آذربایجان شرقی - کلیبر";
            case "154": return "آذربایجان شرقی - مراغه";
            case "155": return "آذربایجان شرقی - مراغه";
            case "567": return "آذربایجان شرقی - ورزقان";
            case "173": return "آذربایجان شرقی - هریس";
            case "159": return "آذربایجان شرقی - هشترود";
            case "160": return "آذربایجان شرقی - هشترود";
            case "604": return "آذربایجان شرقی - هوراند";

            case "274": return "آذربایجان غربی - ارومیه";
            case "275": return "آذربایجان غربی - ارومیه";
            case "295": return "آذربایجان غربی - اشنویه";
            case "637": return "آذربایجان غربی - انزل";
            case "292": return "آذربایجان غربی - بوکان";
            case "492": return "آذربایجان غربی - پلدشت";
            case "289": return "آذربایجان غربی - پیرانشهر";
            case "677": return "آذربایجان غربی - تخت سلیمان";
            case "294": return "آذربایجان غربی - تکاب";
            case "493": return "آذربایجان غربی - چایپاره";
            case "279": return "آذربایجان غربی - خوی";
            case "280": return "آذربایجان غربی - خوی";
            case "288": return "آذربایجان غربی - سردشت";
            case "284": return "آذربایجان غربی - سلماس";
            case "285": return "آذربایجان غربی - سلماس";
            case "638": return "آذربایجان غربی - سیلوانه";
            case "291": return "آذربایجان غربی - سیه چشمه(چالدران)";
            case "640": return "آذربایجان غربی - شوط";
            case "293": return "آذربایجان غربی - شاهین دژ";
            case "675": return "آذربایجان غربی - کشاورز";
            case "282": return "آذربایجان غربی - ماکو";
            case "283": return "آذربایجان غربی - ماکو";
            case "286": return "آذربایجان غربی - مهاباد";
            case "287": return "آذربایجان غربی - مهاباد";
            case "296": return "آذربایجان غربی - میاندوآب";
            case "297": return "آذربایجان غربی - میاندوآب";
            case "290": return "آذربایجان غربی - نقده";

            case "400": return "همدان - اسدآباد";
            case "401": return "همدان - اسدآباد";
            case "404": return "همدان - بهار";
            case "405": return "همدان - بهار";
            case "397": return "همدان - تویسرکان";
            case "398": return "همدان - رزن";
            case "399": return "همدان - رزن";
            case "647": return "همدان - شراء و پیشخوار";
            case "502": return "همدان - فامنین";
            case "584": return "همدان - قلقل رود";
            case "402": return "همدان - کبودرآهنگ";
            case "403": return "همدان - کبودرآهنگ";
            case "392": return "همدان - ملایر";
            case "393": return "همدان - ملایر";
            case "395": return "همدان - نهاوند";
            case "396": return "همدان - نهاوند";
            case "386": return "همدان - همدان";

            case "503": return "یزد - ابرکوه";
            case "444": return "یزد - اردکان";
            case "551": return "یزد - اشکذر";
            case "447": return "یزد - بافق";
            case "561": return "یزد - بهاباد";
            case "445": return "یزد - تفت";
            case "718": return "یزد - دستگردان";
            case "083": return "یزد - طبس";
            case "446": return "یزد - مهریز";
            case "448": return "یزد - میبد";
            case "552": return "یزد - نیر";
            case "543": return "یزد - هرات و مروست";
            case "442": return "یزد - یزد";
            case "443": return "یزد - یزد";

            case "051": return "مرکزی - آشتیان";
            case "052": return "مرکزی - اراک";
            case "053": return "مرکزی - اراک";
            case "058": return "مرکزی - تفرش";
            case "055": return "مرکزی - خمین";
            case "617": return "مرکزی - خنداب";
            case "057": return "مرکزی - دلیجان";
            case "618": return "مرکزی - زرند مرکزی";
            case "059": return "مرکزی - ساوه";
            case "060": return "مرکزی - ساوه";
            case "061": return "مرکزی - سربند";
            case "062": return "مرکزی - سربند";
            case "544": return "مرکزی - فراهان";
            case "056": return "مرکزی - محلات";
            case "571": return "مرکزی - وفس";
            case "593": return "مرکزی - هندودر";
            case "667": return "هرمزگان - ابوموسی";

            case "348": return "هرمزگان - بستک";
            case "586": return "هرمزگان - بشاگرد";
            case "338": return "هرمزگان - بندرعباس";
            case "339": return "هرمزگان - بندرعباس";
            case "343": return "هرمزگان - بندرلنگه";
            case "344": return "هرمزگان - بندرلنگه";
            case "346": return "هرمزگان - جاسک";
            case "337": return "هرمزگان - حاجی آباد";
            case "554": return "هرمزگان - خمیر";
            case "469": return "هرمزگان - رودان";
            case "537": return "هرمزگان - فین";
            case "345": return "هرمزگان - قشم";
            case "470": return "هرمزگان - گاوبندی";
            case "341": return "هرمزگان - میناب";
            case "342": return "هرمزگان - میناب";

            case "483": return "لرستان - ازنا";
            case "484": return "لرستان - ازنا";
            case "557": return "لرستان - اشترینان";
            case "418": return "لرستان - الشتر";
            case "416": return "لرستان - الیگودرز";
            case "417": return "لرستان - الیگودرز";
            case "412": return "لرستان - بروجرد";
            case "413": return "لرستان - بروجرد";
            case "592": return "لرستان - پاپی";
            case "612": return "لرستان - چغلوندی";
            case "613": return "لرستان - چگنی";
            case "406": return "لرستان - خرم آباد";
            case "407": return "لرستان - خرم آباد";
            case "421": return "لرستان - دورود";
            case "598": return "لرستان - رومشکان";
            case "419": return "لرستان - کوهدشت";
            case "385": return "لرستان - ملاوی(پلدختر)";
            case "420": return "لرستان - نورآباد(دلفان)";
            case "528": return "لرستان - ویسیان";

            case "213": return "مازندران - آمل";
            case "214": return "مازندران - آمل";
            case "205": return "مازندران - بابل";
            case "206": return "مازندران - بابل";
            case "498": return "مازندران - بابلسر";
            case "568": return "مازندران - بندپی";
            case "711": return "مازندران - بندپی شرقی";
            case "217": return "مازندران - بهشهر";
            case "218": return "مازندران - بهشهر";
            case "221": return "مازندران - تنکابن";
            case "582": return "مازندران - جویبار";
            case "483": return "مازندران - چالوس";
            case "625": return "مازندران - چمستان";
            case "576": return "مازندران - چهاردانگه";
            case "578": return "مازندران - دودانگه";
            case "227": return "مازندران - رامسر";
            case "208": return "مازندران - ساری";
            case "209": return "مازندران - ساری";
            case "225": return "مازندران - سوادکوه";
            case "577": return "مازندران - شیرگاه";
            case "712": return "مازندران - عباس آباد";
            case "215": return "مازندران - قائمشهر";
            case "216": return "مازندران - قائمشهر";
            case "626": return "مازندران - کجور";
            case "627": return "مازندران - کلاردشت";
            case "579": return "مازندران - گلوگاه";
            case "713": return "مازندران - میاندورود";
            case "499": return "مازندران - نکاء";
            case "222": return "مازندران - نور";
            case "219": return "مازندران - نوشهر";
            case "220": return "مازندران - نوشهر";
            case "500": return "مازندران - هراز و محمودآباد";
            case "501": return "مازندران - هراز و محمودآباد";

            case "623": return "گلستان - آزادشهر";
            case "497": return "گلستان - آق قلا";
            case "223": return "گلستان - بندرترکمن";
            case "689": return "گلستان - بندرگز";
            case "487": return "گلستان - رامیان";
            case "226": return "گلستان - علی آباد";
            case "224": return "گلستان - کردکوی";
            case "486": return "گلستان - کلاله";
            case "211": return "گلستان - گرگان";
            case "212": return "گلستان - گرگان";
            case "628": return "گلستان - گمیشان";
            case "202": return "گلستان - گنبد کاووس";
            case "203": return "گلستان - گنبد کاووس";
            case "531": return "گلستان - مراوه تپه";
            case "488": return "گلستان - مینودشت";

            case "261": return "گیلان - آستارا";
            case "273": return "گیلان - آستانه";
            case "630": return "گیلان - املش";
            case "264": return "گیلان - بندرانزلی";
            case "218": return "گیلان - خمام";
            case "631": return "گیلان - رحیم آباد";
            case "258": return "گیلان - رشت";
            case "259": return "گیلان - رشت";
            case "570": return "گیلان - رضوانشهر";
            case "265": return "گیلان - رودبار";
            case "268": return "گیلان - رودسر";
            case "269": return "گیلان - رودسر";
            case "653": return "گیلان - سنگر";
            case "517": return "گیلان - سیاهکل";
            case "569": return "گیلان - شفت";
            case "267": return "گیلان - صومعه سرا";
            case "262": return "گیلان - طالش";
            case "263": return "گیلان - طالش";
            case "593": return "گیلان - عمارلو";
            case "266": return "گیلان - فومن";
            case "693": return "گیلان - کوچصفهان";
            case "271": return "گیلان - لاهیجان";
            case "272": return "گیلان - لاهیجان";
            case "694": return "گیلان - لشت نشاء";
            case "270": return "گیلان - لنگرود";
            case "516": return "گیلان - ماسال و شاندرمن";

            case "333": return "کرمانشاه - اسلام آباد";
            case "334": return "کرمانشاه - اسلام آباد";
            case "691": return "کرمانشاه - باینگان";
            case "323": return "کرمانشاه - پاوه";
            case "322": return "کرمانشاه - پاوه";
            case "595": return "کرمانشاه - ثلاث باباجانی";
            case "395": return "کرمانشاه - جوانرود";
            case "641": return "کرمانشاه - حمیل";
            case "596": return "کرمانشاه - روانسر";
            case "336": return "کرمانشاه - سرپل ذهاب";
            case "335": return "کرمانشاه - سنقر";
            case "496": return "کرمانشاه - صحنه";
            case "337": return "کرمانشاه - قصرشیرین";
            case "324": return "کرمانشاه - کرمانشاه";
            case "325": return "کرمانشاه - کرمانشاه";
            case "394": return "کرمانشاه - کرند";
            case "330": return "کرمانشاه - کنگاور";
            case "332": return "کرمانشاه - گیلانغرب";
            case "331": return "کرمانشاه - هرسین";

            case "687": return "کهکیلویه و بویراحمد - باشت";
            case "422": return "کهکیلویه و بویراحمد - بویراحمد(یاسوج)";
            case "423": return "کهکیلویه و بویراحمد - بویراحمد(یاسوج)";
            case "599": return "کهکیلویه و بویراحمد - بهمنی";
            case "600": return "کهکیلویه و بویراحمد - چاروسا";
            case "688": return "کهکیلویه و بویراحمد - دروهان";
            case "424": return "کهکیلویه و بویراحمد - کهکیلویه(دهدشت)";
            case "425": return "کهکیلویه و بویراحمد - کهکیلویه(دهدشت)";
            case "426": return "کهکیلویه و بویراحمد - گچساران(دوگنبدان)";
            case "550": return "کهکیلویه و بویراحمد - لنده";
            case "697": return "کهکیلویه و بویراحمد - مارگون";

            case "384": return "کردستان - بانه";
            case "377": return "کردستان - بیجار";
            case "378": return "کردستان - بیجار";
            case "558": return "کردستان - دهگلان";
            case "385": return "کردستان - دیواندره";
            case "646": return "کردستان - سروآباد";
            case "375": return "کردستان - سقز";
            case "376": return "کردستان - سقز";
            case "372": return "کردستان - سنندج";
            case "373": return "کردستان - سنندج";
            case "379": return "کردستان - قروه";
            case "380": return "کردستان - قروه";
            case "383": return "کردستان - کامیاران";
            case "674": return "کردستان - کرانی";
            //case "382": return "کردستان - مریوان";
            case "381": return "کردستان - مریوان";
            case "676": return "کردستان - نمشیر";

            case "722": return "کرمان - ارزونیه";
            case "542": return "کرمان - انار";
            case "313": return "کرمان - بافت";
            case "312": return "کرمان - بافت";
            case "317": return "کرمان - بردسیر";
            case "311": return "کرمان - بم";
            case "310": return "کرمان - بم";
            case "303": return "کرمان - جیرفت";
            case "302": return "کرمان - جیرفت";
            case "583": return "کرمان - رابر";
            case "321": return "کرمان - راور";
            case "382": return "کرمان - راین";
            case "304": return "کرمان - رفسنجان";
            case "305": return "کرمان - رفسنجان";
            case "536": return "کرمان - رودبار کهنوج";
            case "605": return "کرمان - ریگان";
            case "308": return "کرمان - زرند";
            case "309": return "کرمان - زرند";
            case "306": return "کرمان - سیرجان";
            case "307": return "کرمان - سیرجان";
            case "319": return "کرمان - شهداد";
            case "313": return "کرمان - شهربابک";
            case "314": return "کرمان - شهربابک";
            case "606": return "کرمان - عنبرآباد";
            case "320": return "کرمان - فهرج";
            case "698": return "کرمان - قلعه گنج";
            case "299": return "کرمان - کرمان";
            case "298": return "کرمان - کرمان";
            case "535": return "کرمان - کوهبنان";
            case "315": return "کرمان - کهنوج";
            case "316": return "کرمان - کهنوج";
            case "318": return "کرمان - گلباف";
            case "607": return "کرمان - ماهان";
            case "608": return "کرمان - منوجان";

            case "508": return "قزوین - آبیک";
            case "538": return "قزوین - آوج";
            case "728": return "قزوین - البرز";
            case "509": return "قزوین - بوئین زهرا";
            case "438": return "قزوین - تاکستان";
            case "439": return "قزوین - تاکستان";
            case "580": return "قزوین - رودبار الموت";
            case "590": return "قزوین - رودبار شهرستان";
            case "559": return "قزوین - ضیاءآباد";
            case "588": return "قزوین - طارم سفلی";
            case "431": return "قزوین - قزوین";
            case "432": return "قزوین - قزوین";

            case "037": return "قم - قم";
            case "038": return "قم - قم";
            case "702": return "قم - کهک";

            case "240": return "فارس - آباده";
            case "241": return "فارس - آباده";
            case "670": return "فارس - آباده طشک";
            case "648": return "فارس - ارسنجان";
            case "252": return "فارس - استهبان";
            case "678": return "فارس - اشکنان";
            case "253": return "فارس - اقلید";
            case "649": return "فارس - اوز";
            case "513": return "فارس - بوانات";
            case "546": return "فارس - بیضا";
            case "671": return "فارس - جویم";
            case "246": return "فارس - جهرم";
            case "247": return "فارس - جهرم";
            case "654": return "فارس - حاجی آباد(زرین دشت)";
            case "548": return "فارس - خرامه";
            case "547": return "فارس - خشت و کمارج";
            case "655": return "فارس - خفر";
            case "248": return "فارس - داراب";
            case "249": return "فارس - داراب";
            case "253": return "فارس - سپیدان";
            case "514": return "فارس - سروستان";
            case "665": return "فارس - سعادت آباد";
            case "673": return "فارس - شیبکوه";
            case "228": return "فارس - شیراز";
            case "229": return "فارس - شیراز";
            case "230": return "فارس - شیراز";
            case "679": return "فارس - فراشبند";
            case "256": return "فارس - فسا";
            case "257": return "فارس - فسا";
            case "244": return "فارس - فیروزآباد";
            case "245": return "فارس - فیروزآباد";
            case "681": return "فارس - قنقری(خرم بید)";
            case "723": return "فارس - قیروکارزین";
            case "236": return "فارس - کازرون";
            case "237": return "فارس - کازرون";
            case "683": return "فارس - کوار";
            case "656": return "فارس - کراش";
            case "250": return "فارس - لارستان";
            case "251": return "فارس - لارستان";
            case "515": return "فارس - لامرد";
            case "242": return "فارس - مرودشت";
            case "243": return "فارس - مرودشت";
            case "238": return "فارس - ممسنی";
            case "239": return "فارس - ممسنی";
            case "657": return "فارس - مهر";
            case "255": return "فارس - نی ریز";

            case "684": return "سمنان - ایوانکی";
            case "700": return "سمنان - بسطام";
            case "642": return "سمنان - بیارجمند";
            case "457": return "سمنان - دامغان";
            case "456": return "سمنان - سمنان";
            case "458": return "سمنان - شاهرود";
            case "459": return "سمنان - شاهرود";
            case "460": return "سمنان - گرمسار";
            case "530": return "سمنان - مهدیشهر";
            case "520": return "سمنان - میامی";

            case "358": return "سیستان و بلوچستان - ایرانشهر";
            case "682": return "سیستان و بلوچستان - بزمان";
            case "703": return "سیستان و بلوچستان - بمپور";
            case "364": return "سیستان و بلوچستان - چابهار";
            case "365": return "سیستان و بلوچستان - چابهار";
            case "371": return "سیستان و بلوچستان - خاش";
            case "701": return "سیستان و بلوچستان - دشتیاری";
            case "720": return "سیستان و بلوچستان - راسک";
            case "366": return "سیستان و بلوچستان - زابل";
            case "367": return "سیستان و بلوچستان - زابل";
            case "704": return "سیستان و بلوچستان - زابلی";
            case "361": return "سیستان و بلوچستان - زاهدان";
            case "362": return "سیستان و بلوچستان - زاهدان";
            case "369": return "سیستان و بلوچستان - سراوان";
            case "370": return "سیستان و بلوچستان - سراوان";
            case "635": return "سیستان و بلوچستان - سرباز";
            case "668": return "سیستان و بلوچستان - سیب و سوران";
            case "533": return "سیستان و بلوچستان - شهرکی و ناروئی(زهک)";
            case "705": return "سیستان و بلوچستان - شیب آب";
            case "699": return "سیستان و بلوچستان - فنوج";
            case "669": return "سیستان و بلوچستان - قصرقند";
            case "725": return "سیستان و بلوچستان - کنارک";
            case "597": return "سیستان و بلوچستان - لاشار(اسپکه)";
            case "611": return "سیستان و بلوچستان - میرجاوه";
            case "525": return "سیستان و بلوچستان - نیک شهر";

            case "643": return "خراسان رضوی - احمدآباد";
            case "562": return "خراسان رضوی - بجستان";
            case "572": return "خراسان رضوی - بردسکن";
            case "074": return "خراسان رضوی - تایباد";
            case "644": return "خراسان رضوی - تخت جلگه";
            case "072": return "خراسان رضوی - تربت جام";
            case "073": return "خراسان رضوی - تربت جام";
            case "069": return "خراسان رضوی - تربت حیدریه";
            case "070": return "خراسان رضوی - تربت حیدریه";
            case "521": return "خراسان رضوی - جغتای";
            case "573": return "خراسان رضوی - جوین";
            case "522": return "خراسان رضوی - چناران";
            case "724": return "خراسان رضوی - خلیل آباد";
            case "076": return "خراسان رضوی - خواف";
            case "077": return "خراسان رضوی - درگز";
            case "650": return "خراسان رضوی - رشتخوار";
            case "574": return "خراسان رضوی - زبرخان";
            case "078": return "خراسان رضوی - سبزوار";
            case "079": return "خراسان رضوی - سبزوار";
            case "081": return "خراسان رضوی - سرخس";
            case "084": return "خراسان رضوی - فریمان";
            case "651": return "خراسان رضوی - فیض آباد";
            case "086": return "خراسان رضوی - قوچان";
            case "087": return "خراسان رضوی - قوچان";
            case "089": return "خراسان رضوی - کاشمر";
            case "090": return "خراسان رضوی - کاشمر";
            case "553": return "خراسان رضوی - کلات";
            case "091": return "خراسان رضوی - گناباد";
            case "092": return "خراسان رضوی - مشهد";
            case "093": return "خراسان رضوی - مشهد";
            case "094": return "خراسان رضوی - مشهد";
            case "097": return "خراسان رضوی - مشهد منطقه2";
            case "098": return "خراسان رضوی - مشهد منطقه3";
            case "096": return "خراسان رضوی - مشهد منطقه1";
            case "105": return "خراسان رضوی - نیشابور";
            case "106": return "خراسان رضوی - نیشابور";

            case "063": return "خراسان شمالی - اسفراین";
            case "067": return "خراسان شمالی - بجنورد";
            case "068": return "خراسان شمالی - بجنورد";
            case "075": return "خراسان شمالی - جاجرم";
            case "591": return "خراسان شمالی - رازوجرکلان";
            case "082": return "خراسان شمالی - شیروان";
            case "635": return "خراسان شمالی - فاروج";
            case "524": return "خراسان شمالی - مانه و سملقان";

            case "468": return "چهارمحال و بختیاری - اردل";
            case "465": return "چهارمحال و بختیاری - بروجن";
            case "461": return "چهارمحال و بختیاری - شهرکرد";
            case "462": return "چهارمحال و بختیاری - شهرکرد";
            case "467": return "چهارمحال و بختیاری - فارسان";
            case "632": return "چهارمحال و بختیاری - فلارد";
            case "555": return "چهارمحال و بختیاری - کوهرنگ";
            case "633": return "چهارمحال و بختیاری - کیار";
            case "629": return "چهارمحال و بختیاری - گندمان";
            case "466": return "چهارمحال و بختیاری - لردگان";
            case "696": return "چهارمحال و بختیاری - میانکوه";

            case "721": return "خراسان جنوبی - بشرویه";
            case "064": return "خراسان جنوبی - بیرجند";
            case "065": return "خراسان جنوبی - بیرجند";
            case "523": return "خراسان جنوبی - درمیان";
            case "652": return "خراسان جنوبی - زیرکوه";
            case "719": return "خراسان جنوبی - سرایان";
            case "716": return "خراسان جنوبی - سربیشه";
            case "085": return "خراسان جنوبی - فردوس";
            case "088": return "خراسان جنوبی - قائنات";
            case "563": return "خراسان جنوبی - نهبندان";

            case "529": return "بوشهر - بندر دیلم";
            case "353": return "بوشهر - بندر گناوه";
            case "349": return "بوشهر - بوشهر";
            case "350": return "بوشهر - بوشهر";
            case "355": return "بوشهر - تنگستان";
            case "609": return "بوشهر - جم";
            case "351": return "بوشهر - دشتستان";
            case "352": return "بوشهر - دشتستان";
            case "354": return "بوشهر - دشتی";
            case "732": return "بوشهر - دلوار";
            case "357": return "بوشهر - دیر";
            case "532": return "بوشهر - سعد آباد";
            case "610": return "بوشهر - شبانکاره";
            case "356": return "بوشهر - کنگان";

            case "556": return "تهران - اسلامشهر";
            case "658": return "تهران - پاکدشت";
            case "001": return "تهران - تهران مرکزی";
            case "002": return "تهران - تهران مرکزی";
            case "003": return "تهران - تهران مرکزی";
            case "004": return "تهران - تهران مرکزی";
            case "005": return "تهران - تهران مرکزی";
            case "006": return "تهران - تهران مرکزی";
            case "007": return "تهران - تهران مرکزی";
            case "008": return "تهران - تهران مرکزی";
            case "011": return "تهران - تهران جنوب";
            case "020": return "تهران - تهران شرق";
            case "025": return "تهران - تهرانشمال";
            case "015": return "تهران - تهران غرب";
            case "043": return "تهران - دماوند";
            case "666": return "تهران - رباط کریم";
            case "489": return "تهران - ساوجبلاغ";
            case "044": return "تهران - شمیران";
            case "045": return "تهران - شمیران";
            case "048": return "تهران - شهرری";
            case "049": return "تهران - شهرری";
            case "490": return "تهران - شهریار";
            case "491": return "تهران - شهریار";
            case "695": return "تهران - طالقان";
            case "659": return "تهران - فیروزکوه";
            case "031": return "تهران - کرج";
            case "032": return "تهران - کرج";
            case "664": return "تهران - کهریزک";
            case "717": return "تهران - نظرآباد";
            case "041": return "تهران - ورامین";
            case "042": return "تهران - ورامین";

            case "471": return "امور خارجه - امور خارجه";
            case "472": return "امور خارجه - امور خارجه";

            case "454": return "ایلام - آبدانان";
            case "581": return "ایلام - ارکوازی(ملکشاهی)";
            case "449": return "ایلام - ایلام";
            case "450": return "ایلام - ایلام";
            case "616": return "ایلام - ایوان";
            case "534": return "ایلام - بدره";
            case "455": return "ایلام - دره شهر";
            case "451": return "ایلام - دهلران";
            case "726": return "ایلام - زرین آباد";
            case "634": return "ایلام - شیروان لومار";
            case "453": return "ایلام - شیروان و چرداول";
            case "727": return "ایلام - موسیان";
            case "452": return "ایلام - مهران";

            case "145": return "اردبیل - اردبیل";
            case "146": return "اردبیل - اردبیل";
            case "731": return "اردبیل - ارشق";
            case "690": return "اردبیل - انگوت";
            case "601": return "اردبیل - بیله سوار";
            case "504": return "اردبیل - پارس آباد";
            case "163": return "اردبیل - خلخال";
            case "714": return "اردبیل - خورش رستم";
            case "715": return "اردبیل - سرعین";
            case "566": return "اردبیل - سنجبد(کوثر)";
            case "166": return "اردبیل - مشکین شهر";
            case "167": return "اردبیل - مشکین شهر";
            case "161": return "اردبیل - مغان";
            case "162": return "اردبیل - مغان";
            case "686": return "اردبیل - نمین";
            case "603": return "اردبیل - نیر";

            case "619": return "اصفهان - آران و بیدگل";
            case "118": return "اصفهان - اردستان";
            case "127": return "اصفهان - اصفهان";
            case "128": return "اصفهان - اصفهان";
            case "129": return "اصفهان - اصفهان";
            case "620": return "اصفهان - باغ بهادران";
            case "621": return "اصفهان - بوئین و میاندشت";
            case "549": return "اصفهان - تیران و کرون";
            case "564": return "اصفهان - جرقویه";
            case "575": return "اصفهان - چادگان";
            case "113": return "اصفهان - خمینی شهر";
            case "114": return "اصفهان - خمینی شهر";
            case "122": return "اصفهان - خوانسار";
            case "540": return "اصفهان - خور و بیابانک";
            case "660": return "اصفهان - دولت آباد";
            case "120": return "اصفهان - سمیرم";
            case "512": return "اصفهان - سمیرم سفلی(دهاقان)";
            case "510": return "اصفهان - شاهین شهر";
            case "511": return "اصفهان - شاهین شهر";
            case "119": return "اصفهان - شهرضا";
            case "115": return "اصفهان - فریدن";
            case "112": return "اصفهان - فریدونشهر";
            case "110": return "اصفهان - فلاورجان";
            case "111": return "اصفهان - فلاورجان";
            case "125": return "اصفهان - کاشان";
            case "126": return "اصفهان - کاشان";
            case "565": return "اصفهان - کوهپایه";
            case "121": return "اصفهان - گلپایگان";
            case "116": return "اصفهان - لنجان(زرینشهر)";
            case "117": return "اصفهان - لنجان(زرینشهر)";
            case "541": return "اصفهان - مبارکه";
            case "622": return "اصفهان - میمه";
            case "124": return "اصفهان - نائین";
            case "108": return "اصفهان - نجف آباد";
            case "109": return "اصفهان - نجف آباد";
            case "123": return "اصفهان - نطنز";

            case "181": return "خوزستان - آبادان";
            case "527": return "خوزستان - آغاجاری";
            case "585": return "خوزستان - اروندکنار";
            case "685": return "خوزستان - امیدیه";
            case "663": return "خوزستان - اندیکا";
            case "192": return "خوزستان - اندیمشک";
            case "193": return "خوزستان - اندیمشک";
            case "174": return "خوزستان - اهواز";
            case "175": return "خوزستان - اهواز";
            case "183": return "خوزستان - ایذه";
            case "184": return "خوزستان - ایذه";
            case "481": return "خوزستان - باغ ملک";
            case "706": return "خوزستان - بندر امام خمینی";
            case "194": return "خوزستان - بندرماهشهر";
            case "195": return "خوزستان - بندرماهشهر";
            case "185": return "خوزستان - بهبهان";
            case "186": return "خوزستان - بهبهان";
            case "182": return "خوزستان - خرمشهر";
            case "199": return "خوزستان - دزفول";
            case "200": return "خوزستان - دزفول";
            case "198": return "خوزستان - دشت آزادگان";
            case "662": return "خوزستان - رامشیر";
            case "190": return "خوزستان - رامهرمز";
            case "191": return "خوزستان - رامهرمز";
            case "692": return "خوزستان - سردشت";
            case "189": return "خوزستان - شادگان";
            case "707": return "خوزستان - شاوور";
            case "526": return "خوزستان - شوش";
            case "187": return "خوزستان - شوشتر";
            case "188": return "خوزستان - شوشتر";
            case "729": return "خوزستان - گتوند";
            case "730": return "خوزستان - لالی";
            case "196": return "خوزستان - مسجدسلیمان";
            case "197": return "خوزستان - مسجدسلیمان";
            case "661": return "خوزستان - هندیجان";
            case "680": return "خوزستان - هویزه";

            default: return "";
        }
    else
        return "";
}

function loadFile(event) {
    var ImageName = event.target.id.replace("_File", "_Image");
    var FieldName = event.target.id.replace("_File", "");
    var reader = new FileReader();
    reader.readAsDataURL(event.target.files[0]);
    reader.onload = function () {
        $("#" + ImageName)[0].src = reader.result;
        $("#" + FieldName)[0].value = reader.result;
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
};


function PlaqueStyle(e) {
    var InputValue = e.value.replace(/ /g, "");
    InputValue = InputValue.replace(/ایران/g, "");
    var CharArraye = Object.assign([], InputValue);
    var Result = "";
    $.each(CharArraye, function (index, value) {
        if (value != String.fromCharCode(8206))
            Result += value;
    });

    InputValue = Result;
    if (InputValue.length >= 3 && InputValue.length <= 8) {
        switch (CharArraye[3]) {
            case "ا":
                $("#" + e.id).css("color", "white");
                $("#" + e.id).css("background-color", "#ed1c24");
                $("#" + e.id).parent().css("background-color", "#ed1c24");
                break;
            case "ث":
            case "پ":
                $("#" + e.id).css("color", "white");
                $("#" + e.id).css("background-color", "#005329");
                $("#" + e.id).parent().css("background-color", "#005329");
                break;
            case "ت":
            case "ع":
            case "ک":
                $("#" + e.id).css("color", "black");
                $("#" + e.id).css("background-color", "#f3cf09");
                $("#" + e.id).parent().css("background-color", "#f3cf09");
                break;
            default:
                $("#" + e.id).css("color", "black");
                $("#" + e.id).css("background-color", "white");
                $("#" + e.id).parent().css("background-color", "white");
        }
    }
    else {

        $("#" + e.id).css("color", "black");
        $("#" + e.id).css("background-color", "white");
        $("#" + e.id).parent().css("background-color", "white");
    }
}



function RoundNumber(value, exp) {
    if (typeof exp === 'undefined' || +exp === 0)
        return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
        return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
}


function ToEnglishNumber(strNum) {
    var pn = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
    var en = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    var an = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    var cache = strNum;
    for (var i = 0; i < 10; i++) {
        var regex_fa = new RegExp(pn[i], 'g');
        var regex_ar = new RegExp(an[i], 'g');
        cache = cache.replace(regex_fa, en[i]);
        cache = cache.replace(regex_ar, en[i]);
    }
    return cache;
}


function CopyText(element) { 
    var ElementId = element.id.replace("CopyTextTab", "");
    $("#" + ElementId).focus();
    $("#" + ElementId).select();
    document.execCommand('copy');
    popupNotification.show("کپی انجام شد", "success"); 
}

async function PastText(element) {
    var ElementId = element.id.replace("PastTextTab", "");  
    const text = await navigator.clipboard.readText();
    $("#" + ElementId).data("kendoTextArea").value(text);
    popupNotification.show("جایگذاری انجام شد", "success");

}


function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}