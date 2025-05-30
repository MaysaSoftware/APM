
function ImportDataButtonClick(e) {
    var ISDetailGridForm = e.id.indexOf("DetailImportDataButton") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailImportDataButton" : "ImportDataButton";
    var DialogImportData = ISDetailGridForm ? "DetailDialogImportData" : "DialogImportData";
    var FormArraye = e.id.split('_');
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];
    var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[2] : "");

    var Dialog = $("#" + DialogImportData + "_" + DataKey + "_" + ParentID).data("kendoDialog");
    Dialog.open(); 
}

function OnclickCloseWinImportData(e) {
    var WindowName = e.sender.element.context.id;
    var wnd = $("#" + WindowName).data("kendoWindow"); 
    wnd.content("");
}

var IsSuccess = false;
function UploadUploadFileImportData(e) {
    IsSuccess = false;
}
function SuccessUploadFileImportData(e) {
    IsSuccess = true;
}


function CompleteUploadFileImportData(e) {
    var ID = e.sender.element[0].id; 
    var ISDetailGridForm = ID.indexOf("DetailUploadIDImportData") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailUploadIDImportData" : "UploadIDImportData";
    var DialogImportData = ISDetailGridForm ? "DetailDialogImportData" : "DialogImportData";
    var WinImportData = ISDetailGridForm ? "DetailWinImportData" : "WinImportData";

    var Dialog = $("#" + ID.replaceAll(ReplaceWord, DialogImportData)).data("kendoDialog");
    Dialog.close(); 

    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2]; 
    var FormDiv = "ImportDataFormDiv" + DataKey + "_" + ParentID;
     
    var GridName = (ISDetailGridForm ? "Detail" : "") + "MainGrid" + DataKey + (ISDetailGridForm ? "_" + ParentID : "");
    var grid = $("#" + GridName).data("kendoGrid");
    grid.dataSource.read({ IsReload: true });

    if (IsSuccess == true) { 
        var wnd = $("#" + ID.replaceAll(ReplaceWord, WinImportData)).data("kendoWindow");

        wnd.content("<div id='" + FormDiv + "' style='height:100%'></div>");
        var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        wnd.setOptions({
            width: newWidth - 50,
            height: newHeight - 50
        });

        wnd.center();
        wnd.open();


        $("#" + FormDiv).load("/ImportData/LoadContentFile", {
            DataKey: DataKey,
            ParentID: ParentID
        },
            function () {
                HideLoader();
            }
        );
    }
}



async function SaveImportDateSelectedRow(e) { 
    var FileArr = e.id.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2];

    var WinImportData =  "WinImportData";

    if ($("#" + e.id.replace("SaveButtonID_", "CheckedID_"))[0].checked == true) { 
        popupNotification.show('فایل دارای اشکال می باشد لطفا مطابق راهنما فایل را ایمپورت نمایید', "error");
    }
    else { 
        var SelectedGrid = $("#" + e.id.replace("SaveButtonID_", "ImportDataGrid_")).data("kendoGrid");

        var ColumnName = [];

        for (let index = 0; index < SelectedGrid.columns.length; index++) {
            if (SelectedGrid.columns[index].field != undefined)
                ColumnName.push(SelectedGrid.columns[index].field);
        }

        var grid = $("#MainGrid" + DataKey).data("kendoGrid");
        if (grid == undefined) {
            grid = $("#MainGrid" + DataKey + "_" + ParentID).data("kendoGrid"); 
            if (grid == undefined) {
                grid = $("#DetailMainGrid" + DataKey + "_" + ParentID).data("kendoGrid");
                WinImportData = "DetailWinImportData"  ;
            }
        }
        var datasource = grid.dataSource;
        var SelectedRows = SelectedGrid.select();
        var indexRow = 0;
        await SelectedRows.each(function (index, row) {
            var selectedItem = SelectedGrid.dataItem(row);
            var ValuesItem = [];

            for (let index = 0; index < ColumnName.length; index++) {
                ValuesItem.push(selectedItem[ColumnName[index]]);
            }

            $.ajax({
                url: "/Desktop/SaveFormEditor",
                data: {
                    'DataKey': DataKey,
                    'ParentID': ParentID,
                    'RecordID': 0,
                    'FormInputName': ColumnName,
                    'FormInputValue': ValuesItem,
                    'SearchDataKey': 0,
                    'IsImport': true,
                    'SaveChilde': false
                },
                type: "POST",
                success: function (result) {
                    indexRow = indexRow + 1;
                    document.getElementById(e.id.replace("SaveButtonID_", "ResultRunExtanded_")).innerHTML = "بررسی " + indexRow + " ردیف از " + SelectedRows.length + " ردیف ";

                    if (result.Message == "") { 
                        SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
                        SelectedGrid.clearSelection();
                        if (indexRow == SelectedRows.length) {
                            var wnd = $("#" + WinImportData+"_" + DataKey + "_" + ParentID).data("kendoWindow");
                            wnd.close();
                            datasource.read();
                        }
                    }
                    else { 
                        SelectedGrid.clearSelection();
                        SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(result.Message);
                        SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                        SelectedGrid.showColumn("_ShowError");
                        SelectedGrid.autoFitColumn("_ShowError");
                    }
                },
                error: function (result) {
                    popupNotification.show('ذخیره سازی با شکست مواجه شد', "error");
                }
            })
        });
    }
}


function ExportTempeletFileDataButtonClick(e) { 
    var ID = e.id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2];

    window.location = '/ImportData/ExportTempeletFileDataButtonClick?DataKey=' + DataKey + "&ParentID=" + ParentID;
    //$.ajax({
    //    url: "/ImportData/ExportTempeletFileDataButtonClick",
    //    data: {
    //        'DataKey': DataKey,
    //        'ParentID': ParentID
    //    },
    //    type: "GET",
    //    success: function (result) {
 
    //    },
    //    error: function (result) {
    //        popupNotification.show('ذخیره سازی با شکست مواجه شد', "error");
    //    }
    //})
}