
function StartProcess(e) {
    ShowLoader();

    var ProcessInfo= e.id.split('_'); 
    var ProcessID = ProcessInfo[2];

    $.ajax({
        type: "Post",
        url: "/Desktop/StartProcessStep",
        data: { 'ProcessID': ProcessID },
        success: function (Result) {
            if (Result.InformationEntryFormID > 0) {
                OpenEditorForm(Result.InformationEntryFormID, 0, 0, false, false, Result.InformationEntryFormTitle, ProcessID, Result.NextProcessStepID);
            }
            else { 
                HideLoader();
                popupNotification.show('عملیات با موفقیت انجام شد', "success");
            }
        },
        error: function (data) {
            HideLoader();
            popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
        },
    });
}


function SaveProcessReferral(e)
{
    ShowLoader();
    var inputs = $("#ReferralFormInfoDiv .FormItemInput").find("input:not([type='hidden']), textarea:enabled");
    var FormInputName = [];
    var FormInputValue = [];

    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].id != "") {
            FormInputName.push(inputs[i].id);
            if (inputs[i].type == "checkbox")
                FormInputValue.push(inputs[i].checked);
            else
                FormInputValue.push(inputs[i].value);
        }
        else if (inputs[i].name != "") {
            FormInputName.push(inputs[i].name);
            FormInputValue.push(inputs[i].value);
        }
    }

    var ReferralInputs = $("#ReferralFormMailDiv .FormItemInput").find("input:not([type='hidden']), textarea:enabled");
    var ReferralInputName = [];
    var ReferralInputValue = [];

    for (var i = 0; i < ReferralInputs.length; i++) {
        if (ReferralInputs[i].id != "") {
            ReferralInputName.push(ReferralInputs[i].id);
            if (ReferralInputs[i].type == "checkbox")
                ReferralInputValue.push(ReferralInputs[i].checked);
            else
                ReferralInputValue.push(ReferralInputs[i].value);
        }
        else if (ReferralInputs[i].name != "") {
            ReferralInputName.push(ReferralInputs[i].name);
            ReferralInputValue.push(ReferralInputs[i].value);
        }
    }

     $.ajax({
            type: 'POST',
            url: "/Process/SaveProcessReferral",
             data: {
                 "ReferralInputName": ReferralInputName,
                 "ReferralInputValue": ReferralInputValue,
                 "FormInputName": FormInputName,
                 "FormInputValue": FormInputValue
             },
            dataType: 'json',
         success: function (data) {
             popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
             var wnd = ProcessReferralWin;
             wnd.content("");
             wnd.close();
             var GridName = "MainGrid" + e.sender.element[0].id.replace("SaveButton", "");
             var grid = $("#" + GridName).data("kendoGrid");
             grid.dataSource.read();
             HideLoader();
            }
     });
}


function CanselProcessReferral() {
    var wnd = ProcessReferralWin;
    wnd.close();
}


function ReferralProcessStepperOnSelect(e) {
    FocusFirstItem()
    if (e.step.options.label == "ارجاع") {
        $("#ReferralFormMailDiv").css("display", "block");
        $("#ReferralFormInfoDiv").css("display", "none");
        $("#ButtonMailDiv").css("display", "none");
    }
    else if (e.step.options.label == "ارسال") {
            $("#ReferralFormMailDiv").css("display", "none");
            $("#ReferralFormInfoDiv").css("display", "none");
            $("#ButtonMailDiv").css("display", "block");
        }
    else {
        $("#ButtonMailDiv").css("display", "none");
        $("#ReferralFormMailDiv").css("display", "none");
        $("#ReferralFormInfoDiv").css("display", "block");
    }
}
