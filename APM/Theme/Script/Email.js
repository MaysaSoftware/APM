  
function SendEmailClick(e) {
    var ISDetailGridForm = e.id.indexOf("DetailSendEmail_") > -1 ? true : false; 
    var ReplaceWord = ISDetailGridForm ? "DetailSendEmail" : "SendEmail";
    var FormArraye = e.id.split("_");
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];
    var GridName = ISDetailGridForm ? (FormArraye[0].replace(ReplaceWord, "DetailMainGrid") + FormArraye[1] + "_" + FormArraye[2]) : (FormArraye[0].replace(ReplaceWord, "MainGrid") + FormArraye[1]);
    var grid = $("#" + GridName).data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != null) {
        CleareSendEmailItem(ISDetailGridForm,DataKey, ParentID);
        var SendEmailDialog = $("#" + (ISDetailGridForm ? "Detail" : "") + "SendEmailDialog_" + DataKey + "_" + ParentID).data("kendoWindow");
        SendEmailDialog.open();
    }
    else { 
            popupNotification.show("سطری انتخاب نشده است", "info"); 
    }
}

function CancelSendEmail(e) {
    var SendEmailDialog = $("#" + e.sender.element[0].id.replace("CancelButton","")).data("kendoWindow");
    SendEmailDialog.close();
}

function CleareSendEmailItem(ISDetailGridForm, DataKey, ParentID) {
    $("#" + (ISDetailGridForm ? "Detail" : "") + "ReciveSendEmailDialog_" + DataKey + "_" + ParentID).val("");
    $("#" + (ISDetailGridForm ? "Detail" : "") + "TitleSendEmailDialog_" + DataKey + "_" + ParentID).val("");
    $("#" + (ISDetailGridForm ? "Detail" : "") + "MessageSendEmailDialog_" + DataKey + "_" + ParentID).val("");
}

function SendEmailDialogClick(e) {
    ShowLoader();
    var ISDetailGridForm = e.sender.element[0].id.indexOf("DetailSendButtonSendEmailDialog_") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailSendButtonSendEmailDialog" : "SendButtonSendEmailDialog";
    var FormArraye = e.sender.element[0].id.split("_");
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];
    var GridName = ISDetailGridForm ? (FormArraye[0].replace(ReplaceWord, "DetailMainGrid") + FormArraye[1] + "_" + FormArraye[2]) : (FormArraye[0].replace(ReplaceWord, "MainGrid") + FormArraye[1]);
    var grid = $("#" + GridName).data("kendoGrid"); 
    var selectedItem = grid.dataItem(grid.select());

    var ErrorMessage = "";
    if ($("#" + (ISDetailGridForm ? "Detail" : "") + "ReciveSendEmailDialog_" + DataKey + "_" + ParentID).val() == "")
        ErrorMessage = "گیرنده وارد نشده است";
    else if ($("#" + (ISDetailGridForm ? "Detail" : "") + "TitleSendEmailDialog_" + DataKey + "_" + ParentID).val() == "")
        ErrorMessage = "موضوع وارد نشده است";
    else if ($("#" + (ISDetailGridForm ? "Detail" : "") + "MessageSendEmailDialog_" + DataKey + "_" + ParentID).val() == "")
        ErrorMessage = "متن وارد نشده است";

    if (ErrorMessage != "") {
        HideLoader();
        popupNotification.show(ErrorMessage, "error");
    }
    else {
        $.ajax({
            url: "/Desktop/SendEmailDialog",
            data: {
                DataKey: DataKey,
                ParentID: ParentID,
                RecordID: selectedItem.id,
                SendAttachmentFile: $("#" + (ISDetailGridForm ? "Detail" : "") + "IsSendAttachmentSendEmailDialog_" + DataKey + "_" + ParentID)[0].checked,
                ReportSelected: $("#" + (ISDetailGridForm ? "Detail" : "") + "ReportSendEmailDialog_" + DataKey + "_" + ParentID).val().join(','),
                Email: $("#" + (ISDetailGridForm ? "Detail" : "") + "ReciveSendEmailDialog_" + DataKey + "_" + ParentID).val() ,
                Title: $("#" + (ISDetailGridForm ? "Detail" : "") + "TitleSendEmailDialog_" + DataKey + "_" + ParentID).val() ,
                Massage: $("#" + (ISDetailGridForm ? "Detail" : "") + "MessageSendEmailDialog_" + DataKey + "_" + ParentID).val() ,

            },
            type: "POST",
            success: function (Result) { 
                HideLoader();
                if (Result == "") {
                    var SendEmailDialog = $("#" + e.sender.element[0].id.replace("SendButton", "")).data("kendoWindow");
                    SendEmailDialog.close();
                }
            },
            error: function (result) { 
                HideLoader();
            }
        })
    }

}