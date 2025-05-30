
function OpenCommentGrid(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = FormArraye[0].indexOf("Detail") > -1 ? true : false;
    var Datakey = FormArraye[1];
    var ParentID = FormArraye[2];
    var ProcessID = FormArraye[3];
    var ProcessStepID = FormArraye[4];
    var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + FormArraye[1] + (FormArraye[2] > 0 && ISDetailGridForm ? "_" + FormArraye[2] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != null) { 
        if (e.id.indexOf("AddComment") > -1) {
            $("#Input_" + (ISDetailGridForm ? "Detail" : "") + "AddCommentDialog_" + Datakey + "_" + ParentID).data("kendoTextArea").value("");
            var AddCommentDialog = $("#" + (ISDetailGridForm ? "Detail" : "") +"AddCommentDialog_" + Datakey + "_" + ParentID).data("kendoDialog");
            AddCommentDialog.open();
        }
    }
    else {
        popupNotification.show("سطری انتخاب نشده است", "info");
    }

}

