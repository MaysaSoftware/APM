function MainGridBeforeEdit(e) {
    FocusFirstItem();
}

function FocusFirstItem() {
    setTimeout(DelayedFocusFirstItem, 1000);
}

function DelayedFocusFirstItem() {
    var inputs = $(".FormItemInput").find("input:enabled, textarea:enabled");
    var Isvisble = false;
    var Counter = 0;
    if (inputs.length > 0) {
        while (!Isvisble) {
            if ($(inputs[Counter]).is(":visible")) {
                if ($(document.activeElement).attr('class').indexOf('GridIncell') > -1) {

                }
                else {
                    var dropDown = $(inputs[Counter]).data("kendoDropDownList");
                    if (dropDown != undefined) {
                        dropDown.focus();
                    } else if ($(inputs[Counter]).hasClass("DatePickers")) {

                    } else {
                        $(inputs[Counter]).focusTextToEnd();
                    }
                } 
                Isvisble = true;
                break;
            }
            else {
                if (Counter == inputs.length) {
                    Isvisble = true;
                    break;
                } 
                Counter++;
            }
        }
    }
}


$(document).on("keydown", ".FormItemInput *", function (e) {

    if (e.keyCode == 13 && !e.shiftKey) {
        if (e.target.localName == "textarea") {
            $("#" + e.target.id).val(e.target.value + "\n");
            e.preventDefault();
            return false;
        } 
        $(e.target.parentElement).removeClass("RequiredField");

    }

}); 


var SearchFieldID = "";
var IsSearching = false;
$(document).on("keydown", ".SearchFieldKeydown *", function (e) {

    if (e.keyCode == 13 && !e.shiftKey) {
        if (!IsSearching && e.currentTarget.id != SearchFieldID) {
            setTimeout(function () { 
                FillGridBySearchField(e.currentTarget.id);
                SearchFieldID = e.currentTarget.id;
                IsSearching = true;
            });
        }
        else {
            SearchFieldID = "";
            IsSearching = false;
        }
    }

});


$(document).on("keyup", ".Numeric", function (e) {

    if (e.keyCode == 13 && !e.shiftKey) { 
        e.preventDefault();
        if (e.currentTarget.parentElement.parentElement.id.indexOf("MainGrid") == -1 && e.currentTarget.parentElement.parentElement.id!="") {
            var inputs = $(".FormItemInput:visible").find("input:not([type='hidden']):enabled, textarea:enabled");
            var nextInput = inputs.get(inputs.index(this) + 1);
            if (!e.ctrlKey) {
                var dropDown = $(nextInput).data("kendoDropDownList");

                if (dropDown != undefined) {
                    dropDown.focus();
                } else {
                    $(nextInput).focusTextToEnd();
                }
            }
        }
    }

}); 


$(document).on("keyup", ".FormItemInput *", function (e) { 

    if (e.keyCode == 13 && !e.shiftKey) {

        var IndexElement = e.target;
        var IsStart = true;
        while (IndexElement.id.indexOf("EditorFormDiv") == -1 && IndexElement.id.indexOf("PopupEditorWindow") == -1 && IndexElement.parentElement != null) {
            IndexElement = IndexElement.parentElement;
        }

        if (IndexElement.parentElement != null) {

            if (e.target.className.indexOf("NationalCode") > -1) {
                var NationalCodeError = CheckNationalCode(e.target.value);
                $("#DivNationalCode_" + e.target.id)[0].innerHTML = "";
                if (NationalCodeError != "") {
                    $(e.target.parentElement).addClass("RequiredField");
                }
                else {
                    $(e.target.parentElement).removeClass("RequiredField");
                    $("#DivNationalCode_" + e.target.id)[0].innerHTML = GetCityOfNationalCode(e.target.value);
                }
            }

            var inputs = $(".FormItemInput:visible").find("input:not([type='hidden']):enabled, textarea:enabled");

            for (var i = 0; i < inputs.length; i++) {

                var inputid = inputs[i].id,
                    targetid = e.target.id;

                if (inputid == "") inputid = inputs[i].name;
                if (targetid == "") targetid = e.target.name;

                if (targetid == inputid || e.target.innerHTML.indexOf('id="' + inputs[i].id + '"') > -1) {

                    if (e.ctrlKey && i > 0) {
                        var dropDown = $(inputs[i - 1]).data("kendoDropDownList");

                        if (dropDown != undefined) {
                            dropDown.focus();
                        } else {
                            $(inputs[i - 1]).focusTextToEnd();
                        }
                    }

                    if (!e.ctrlKey && i < (inputs.length - 1)) {

                        var NextElement = inputs[i + 1];
                        while (NextElement.id.indexOf("EditorFormDiv") == -1 && NextElement.id.indexOf("PopupEditorWindow") == -1) {
                            NextElement = NextElement.parentElement;
                        }

                        if (NextElement.id == IndexElement.id) {

                            var Element;
                            if ($(inputs[i + 1]).data("kendoDropDownList") != undefined) {
                                Element = $(inputs[i + 1]).data("kendoDropDownList");
                                Element.focus();
                                return false;
                            }
                            else if ($(inputs[i + 1]).data("kendoComboBox") != undefined) {
                                if ((i + 2) <= (inputs.length - 1)) {
                                    if ($(inputs[i + 2]).data("kendoDropDownList") != undefined) {
                                        Element = $(inputs[i + 2]).data("kendoDropDownList");
                                        Element.focus();
                                    }
                                    else if ($(inputs[i + 2]).data("kendoComboBox") != undefined) {
                                        Element = $(inputs[i + 2]).data("kendoComboBox");
                                        Element.focus();
                                    }
                                    else
                                        $(inputs[i + 2]).focus();

                                    return false;
                                }
                            }
                            else if ($(inputs[i + 1]).data("kendoSwitch") != undefined) {
                                //$('[type = checkbox]: first').focus();
                                //$('input[name^=' + inputs[i + 1].id + ' ]')[0].focus();
                                //Element = $(inputs[i + 1]).data("kendoSwitch");
                                //Element.element[0].focus();
                                //Element.element[0].select();
                                return false;
                            }
                            else {

                                $(inputs[i + 1]).focusTextToEnd();
                            }

                        }

                        if (inputs[i].className.indexOf("MildayDatePickers") > -1) {
                            MiladiDatepicker.hide();
                        }
                        else if (inputs[i].className.indexOf("DatePickers") > -1) {
                            jalaliDatepicker.hide();
                        }
                    }
                }
            }
        }
    } 
});

(function ($) {
    $.fn.focusTextToEnd = function () {
        this.focus();
        var $thisVal = this.val();
        this.val('').val($thisVal);
        return this;
    }

    $.fn.hasId = function () {
        return this.attr('id') != '';
    };
}(jQuery));

function IsLastElement(FormName) {
    var focused = document.activeElement;
    var inputs = $(".FormItemInput:visible", "#" + FormName).find("input:not([type='hidden']):enabled, textarea:enabled");
    var LastIndex = inputs.length - 1;
    if ($(inputs[LastIndex]) == focused)
        return true;
    else {
        //var nextInput = inputs.get(inputs.index(focused) + 1);

        //    var dropDown = $(nextInput).data("kendoDropDownList");

        //    if (dropDown != undefined) {
        //        dropDown.focus();
        //    }
        //    else
        //    {
        //        $(nextInput).focusTextToEnd();
        //    }
        return false;
    }
}

