var InsertInTable = 0;
var UpdateInTable = 1;
var OpenAttachmentTable = 2;
var ReloadTable = 3;
var FormMainInputID = 0;
var LastWindowID = [];
var IsEditGridName;
var IncellGridName;

function MainGridCreateRow(e) {
    var ISDetailGridForm = e.id.indexOf("DetailCreate") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailCreate" : "Create";
    var FormArraye = e.id.split('_');
    var DataKey = FormArraye[0].replace(ReplaceWord, "");
    var ParentID = FormArraye[1];

    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (ISDetailGridForm ? "_" + FormArraye[1] : "");
    CreateRowInGrid(GridName, ISDetailGridForm, DataKey, ParentID);
}

function CreateRowInGrid(GridName,ISDetailGridForm,DataKey, ParentID) {  
     
    var grid = $("#" + GridName).data("kendoGrid");
    if (grid.options.editable.mode == "popup") {
        OpenEditorForm(DataKey, ParentID, 0, ISDetailGridForm, false);
    }
    else {
        var LastGridRowCount = grid.dataSource.total();
        grid.addRow();
        IncellGridName = GridName;
        if (ParentID != "0" && LastGridRowCount != grid.dataSource.total()) {

            $.ajax({
                type: 'POST',
                url: "/Desktop/SaveFormEditor",
                data: {
                    "DataKey": DataKey,
                    "ParentID": ParentID,
                    "RecordID": 0,
                    "FormInputName": [],
                    "FormInputValue": [],
                    "ProcessID": 0,
                    "ProcessStepID": 0
                },
                dataType: 'json',
                success: function (data) {
                    HideLoader();
                    if (data.Message == "") {
                        grid.dataSource._data[0].id = data.RecordID;
                        $.map(data.FieldName, function (val, i) {
                            grid.dataSource._data[0][val] = data.Record[i];
                        });
                    }
                    else
                        popupNotification.show(data.Message, "error");
                },
                error: function (result) {
                    HideLoader();
                    popupNotification.show(result.responseText, "error");
                }
            });
        }
    }
}

function MainGridCreateRowWithNewButtonForm(e) { 
    if (e.id.indexOf("NewButtonForm") > -1) {
        var ISDetailGridForm = e.id.indexOf("DetailNewButtonForm") > -1 ? true : false;
        var FormArraye = e.id.split('_');
        var CoreObjectID = FormArraye[1],
            DataKey = FormArraye[2],
            ParentID = FormArraye[3],
            RecordID = 0,
            ProcessID = FormArraye[4],
            ProcessStepID = FormArraye[5];
        OpenEditorForm(DataKey, ParentID, RecordID, ISDetailGridForm, false, e.item.options.text, ProcessID , ProcessStepID, e.item.options.attributes.data_Item)
    }
}

function MainGridUpdateRow(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = e.id.indexOf("DetailUpdate") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailUpdate" : "Update";
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (ISDetailGridForm ? "_" + FormArraye[1] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != null) {
        var DataKey = e.id.replace(ReplaceWord, "").split('_')[0];
        var ParentID = FormArraye[1];

        $.ajax({
            type: 'POST',
            url: "/Desktop/CanEditRow",
            data: {
                "_DataKey": DataKey,
                "_ParentID": ParentID,
                "_RecordID": selectedItem.id,
            },
            dataType: 'json',
            success: function (data) {
                if (data == true)
                    OpenEditorForm(DataKey, ParentID, selectedItem.id, ISDetailGridForm, false)
                else
                    popupNotification.show("امکان ویرایش وجود ندارد", "error");
            },
            error: function (result) {
                popupNotification.show("خطایی رخ داده است", "error");
            }
        });
    }
    else {
        popupNotification.show("سطری انتخاب نشده است", "info");
    }
}

function MainGridDisplayForce(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = e.id.indexOf("DetailDisplay") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailDisplay" : "Display";
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (ISDetailGridForm ? "_" + FormArraye[1] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != null) {
        var DataKey = e.id.replace(ReplaceWord, "").split('_')[0];
        var ParentID = FormArraye[1];
        OpenEditorForm(DataKey, ParentID, selectedItem.id, ISDetailGridForm, true);

        $.ajax({
            type: 'POST',
            url: "/Home/SaveShowRecordTable",
            data: {
                "DataKey": DataKey,
                "RecordID": selectedItem.id,
            },
            dataType: 'json',
            success: function (data) {

            },
            error: function (result) {
            }
        });
    }
    else {
        popupNotification.show("سطری انتخاب نشده است", "info");
    }
}

function OpenEditorForm(DataKey, ParentID, RecordID, ISDetailGridForm, ISReadOnly, WinTitle = "", ProcessID = 0, ProcessStepID=0,Url="") {

    MiladiDatepicker.hide();
    jalaliDatepicker.hide();
    ISDetailGridForm = ISDetailGridForm == true ? true : false;
    ShowLoader();
    var FormDiv = "EditorFormDiv" + DataKey + "_" + ParentID + "_" + RecordID; 
    var wnd = $("#EditorForm" + DataKey).data("kendoWindow");

    if (wnd == undefined)
        wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");


    if (wnd == undefined)
        wnd = $("#EditorForm" + DataKey).kendoWindow();


    if (wnd.length == 0)
        wnd = $("#EditorForm").kendoWindow()

    if (wnd.length == 0)
        wnd = $("#NewEditorForm").data("kendoWindow");

    wnd.content("<div id='" + FormDiv + "' style='height:100%;overflow-x: hidden;'></div>");
    wnd.title(WinTitle.replaceAll("_", " "));

    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 30,
        height: newHeight - 50
    });

    wnd.center();
    wnd.open();

    var TextareaElement = $("textarea")

    for (let index = 0; index < TextareaElement.length; index++) {
        SetHeightTextarea(TextareaElement[index].id)
    }
    if (Url != "") {
        var UrlArr = Url.split('/');
        var FinalUrl = "";
        if (UrlArr[0] == "")
            FinalUrl = UrlArr.join("/");
        else
            FinalUrl = "/" + UrlArr.join("/");
        $("#" + FormDiv).load(FinalUrl, {}, function () {
            HideLoader();
        })
    } 
    else if ($("#MainGridNewPage_" + DataKey + "_" + ParentID).length > 0 && RecordID==0) {
        var UrlArr = $("#MainGridNewPage_" + DataKey + "_" + ParentID).val().split('/');
        var FinalUrl = "";
        if (UrlArr[0] == "")
            FinalUrl = UrlArr.join("/");
        else
            FinalUrl = "/" + UrlArr.join("/");

        $("#" + FormDiv).load(FinalUrl, {}, function () {
            HideLoader();
        })
    }
    else if ($("#MainGridUpdatePage_" + DataKey + "_" + ParentID).length > 0 && RecordID>0) {
        var UrlArr = $("#MainGridUpdatePage_" + DataKey + "_" + ParentID).val().split('/');
        var FinalUrl = "";
        if (UrlArr[0] == "")
            FinalUrl = UrlArr.join("/");
        else
            FinalUrl = "/" + UrlArr.join("/");

        $("#" + FormDiv).load(FinalUrl, { RecordID: RecordID}, function () {
            HideLoader();
        })
    }
    else {
        $("#" + FormDiv).load("/Home/EditorForm", {
            DataKey: DataKey,
            ParentID: ParentID,
            RecordID: RecordID,
            ISDetailGridForm: ISDetailGridForm,
            ISReadOnly: ISReadOnly,
            ProcessID: ProcessID,
            ProcessStepID: ProcessStepID
        },
            function () {
                HideLoader(); 
                $(".IsNowDateTime").val(GetClockTime());
                if ($("#" + FormDiv + " .ActiveLoadElement").length > 0) {
                    var ActiveLoadElementArr = $("#" + FormDiv + " .ActiveLoadElement");
                    for (var Index = 0; Index < ActiveLoadElementArr.length; Index++) {
                        if (ActiveLoadElementArr[Index].id == "") {
                            Index++;
                        }
                        ElementChange(ActiveLoadElementArr[Index], true)
                    }
                    //if ($("#" + FormDiv + " .ActiveLoadElement")[1].id == "")
                    //    ElementChange($("#" + FormDiv + " .ActiveLoadElement")[2], true)
                    //else
                    //    ElementChange($("#" + FormDiv + " .ActiveLoadElement")[1], true)
                }
                if ($("#" + FormDiv + " .ActiveOnKeyDown").length > 0) {

                    $("#" + FormDiv + " .ActiveOnKeyDown").each(function (index, Item) {
                        var Element = $("#" + Item.id);
                        if (Element.data("kendoDropDownList") != undefined) {
                            Element = Element.data("kendoDropDownList");
                            if (Element.value() != "0")
                                OnKeyDownElement(Element);
                        }
                    })
                }

                setTimeout(function () { 
                    if (RecordID == 0) { 
                        var FormInputName = [];
                        var FormInputValue = [];
                        GetInputValue(FormDiv, FormInputName, FormInputValue);
                        for (let index = 0; index < FormInputName.length; index++) {  
                            Element = $("#" + FormInputName[index]); 
                            if (Element.length > 0) {
                                if (Element[0].className.indexOf("ImageUpload_") > -1) {
                                    if ($(".Logo_HeaderLayout").length > 0) {
                                        $("#" + Element[0].className)[0].src = $(".Logo_HeaderLayout")[0].src;
                                        $("." + Element[0].className).val("");
                                    } 
                                } 
                            }  
                        }
                    } 
                },1000);
            }
        );
    }
}

function SetHeightTextarea(fieldId) {
    if (fieldId != "" && fieldId != "TextPreview") {
        if (document.getElementById(fieldId).scrollHeight == 0)
            document.getElementById(fieldId).style.height = '36px';
        else
            document.getElementById(fieldId).style.height = document.getElementById(fieldId).scrollHeight + 'px';
    }
}

function CloseEditorForm(e) {

    MiladiDatepicker.hide();
    jalaliDatepicker.hide();
    var IsDetail = e.sender.element[0].id.indexOf("Detail") > -1 ? true : false;
    var ReplaceWord = IsDetail ? "DetailEditorForm" : "EditorForm";
    var wnd = $("#" + e.sender.element[0].id).data("kendoWindow");
    wnd.content("");
    var grid = $("#" + (IsDetail ? "Detail" : "") + "MainGrid" + e.sender.element[0].id.replace(ReplaceWord, "")).data("kendoGrid"); 
    if (grid != undefined) {
        grid.dataSource.read({ IsReload: true });
        if (grid.select().length > 0)
            grid.clearSelection();
    }
}

function CancelFormEditor(e) {
    MiladiDatepicker.hide();
    jalaliDatepicker.hide();
    var IsDetail = e.sender.element[0].id.indexOf("Detail") > -1 ? true : false;
    var FormArr = e.sender.element[0].id.split('_');
    var ProcessID = FormArr[3];
    var ReplaceWord = IsDetail ? "DetailCancelButton" : "CancelButton";
    var wnd = $("#" + e.sender.element[0].id.replace(ReplaceWord, "EditorForm").split('_')[0]).data("kendoWindow");

    if (wnd == undefined)
        wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");
    if (wnd == undefined)
        wnd = $("#NewEditorForm" ).data("kendoWindow");
    wnd.content("");
    wnd.close();
    if ($("." + ProcessID).val() != undefined) {
        var grid = $("#" + $("." + ProcessID).val()).data("kendoGrid");
        if (grid != undefined) {
            grid.dataSource.read({ IsReload: true });
            if (grid.select().length > 0)
                grid.clearSelection();
        }
    } 
}

function MainGridSaveRow(e) {
    var ISDetailGridForm = e.id.indexOf("DetailSaveRow") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailSaveRow" : "SaveRow";
    var FormArraye = e.id.split('_');
    var DataKey = FormArraye[0].replace(ReplaceWord, "");
    var ParentID = FormArraye[1];

    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (ISDetailGridForm ? "_" + FormArraye[1] : "");
    var grid = $("#" + GridName).data("kendoGrid");

}

function SaveFormEditor(e) {
    ShowLoader();
    if (e.sender != undefined) {
        var ElementSenderID = e.sender.element[0].id;
        var FormArraye = ElementSenderID.split('_');
        var ISDetailGridForm = ElementSenderID.indexOf("DetailSaveButton") > -1 ? true : false;
        var ReplaceWord = ISDetailGridForm ? "DetailSaveButton" : "SaveButton";
        var DataKey = FormArraye[0].replace(ReplaceWord, "");
        var ParentID = FormArraye[1];
        var RecordID = FormArraye[2];
        var ProcessID = FormArraye[3];
        var ProcessStepID = FormArraye[4];
        var CloseFormAffterSave = FormArraye[5] == 1 ? true : false;
        var SaveParentSubjectSaveChild = FormArraye[6] == 1 ? true : false;
        var SaveAtOnce = FormArraye[7] == 1 ? true : false;
        var FormInputName = [];
        var FormInputValue = [];
        var GridElements = [];
        var FileManagerName = "FileManager" + DataKey + "_" + ParentID + "_" + RecordID;
        var FormDiv = "EditorFormDiv" + DataKey + "_" + ParentID + "_" + RecordID;
        var IsCloseFormAffterSave = (ISDetailGridForm ? "Detail" : "") + "IsCloseFormAffterSave_" + DataKey + "_" + ParentID;
        var projectName = $("#PublicProjeName").val();
        FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
        var GridName = FormArraye[0] + (FormArraye[1] > 0 && ISDetailGridForm ? "_" + FormArraye[1] : "");

        if ($("#" + FormDiv).length == 0) {
            FormDiv = (ISDetailGridForm ? "Detail" : "") + "EditorForm" + DataKey;

            var wnd = $("#" + FormDiv).data("kendoWindow")
            if (wnd == undefined) {
                wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");
                FormDiv = "PopupEditorWindow" + LastWindowID[LastWindowID.length - 1];
            }
        }

        GetInputValue(FormDiv + " .FormItemInput", FormInputName, FormInputValue);
        var RequireMessage = CheckRequiredField(FormDiv, DataKey);

        if (RequireMessage == "") {
            switch (projectName) {
                case "NisocWelfareServiceData": {
                    switch (DataKey) {
                        case "74450": {
                            var MaxLunch = 0;
                            var MaxDinner = 0;
                            var CountLunch = 0;
                            var CountDinner = 0;

                            if (FormInputName.indexOf("حداکثر_تعداد_ناهار_74450") == -1)
                                RequireMessage += "\n حداکثر تعداد ناهار برای این درخواست مشاهده نشده است";
                            else {
                                MaxLunch = FormInputValue[FormInputName.indexOf("حداکثر_تعداد_ناهار_74450")];
                                if (MaxLunch == "")
                                    FormInputValue[FormInputName.indexOf("حداکثر_تعداد_ناهار_74450")] = 0;
                            }

                            if (FormInputName.indexOf("حداکثر_تعداد_شام_74450") == -1)
                                RequireMessage += "\n حداکثر تعداد شام برای این درخواست مشاهده نشده است";
                            else {
                                MaxDinner = FormInputValue[FormInputName.indexOf("حداکثر_تعداد_شام_74450")];
                                if (MaxDinner == "")
                                    FormInputValue[FormInputName.indexOf("حداکثر_تعداد_شام_74450")] = 0;
                            }

                            var Grid = $("#MainGrid74451").data().kendoGrid;
                            var SelectedRows = Grid.table.find("tr");
                            if (SelectedRows.length > 0) {
                                SelectedRows.each(function (index, row) {
                                    var selectedItem = Grid.dataItem(row);
                                    if (selectedItem["وعده_غذایی"] == 2) {
                                        CountLunch += selectedItem["تعداد"];
                                    }
                                    else if (selectedItem["وعده_غذایی"] == 3) {
                                        CountDinner += selectedItem["تعداد"];
                                    }


                                    console.log("تعداد:" + selectedItem["تعداد"])
                                    console.log("شام:" + CountDinner)
                                    console.log("ناهار:" + CountLunch)
                                })

                                if (FormInputName.indexOf("تعداد_شام_رزرو_شده_74450") > -1)
                                    FormInputValue[FormInputName.indexOf("تعداد_شام_رزرو_شده_74450")] = CountDinner;
                                else {
                                    FormInputName.push("تعداد_شام_رزرو_شده_74450");
                                    FormInputValue.push(CountDinner);
                                }

                                if (FormInputName.indexOf("تعداد_ناهار_رزرو_شده_74450") > -1)
                                    FormInputValue[FormInputName.indexOf("تعداد_ناهار_رزرو_شده_74450")] = CountLunch;
                                else {
                                    FormInputName.push("تعداد_ناهار_رزرو_شده_74450");
                                    FormInputValue.push(CountLunch);
                                }

                                if (MaxDinner != CountDinner)
                                    RequireMessage += "\nتعداد شام با مقدار حداکثر برابر نیست";

                                console.log("MaxLunch:" + MaxLunch + " \n CountLunch" + CountLunch)

                                if (parseInt(MaxLunch) > parseInt(CountLunch) && parseInt(MaxLunch) < parseInt(CountLunch))
                                    RequireMessage += "\nتعداد ناهار با مقدار حداکثر برابر نیست";
                            }
                            else
                                RequireMessage += "\nهیچ ردیفی انتخاب نشده است";

                            break;
                        }
                    }
                    break;
                }
            }
        }

        if (RequireMessage != "") {
            HideLoader();
            popupNotification.show(RequireMessage.replace(/\n/g, '<br/>'), "error");
        }
        else {

            if (SaveAtOnce && RecordID == "0") {
                var JsonGrid = [];
                var GridNameID = [];
                var ErrorMessage = "";

                if ($("#" + FormDiv + " .k-grid-display-block:visible").length > 0 && RecordID == 0) {
                    $("#" + FormDiv + " .k-grid-display-block:visible").each(async function (index, GridElement) {
                        var HasError = false;
                        var SelectedGrid = $("#" + GridElement.id).data("kendoGrid");
                        var SelectedRows;
                        var HasSelectColumn = true;

                        if (SelectedGrid.columns[0].field == "_ShowError" || (SelectedGrid.columns[0].title == "ردیف" && SelectedGrid.columns[1].field == "_ShowError")) {
                            HasSelectColumn = false;
                            SelectedRows = SelectedGrid.table.find("tr");
                        }
                        else
                            SelectedRows = SelectedGrid.select();

                        if ($("#" + GridElement.id.replace("MainGrid", "SaveParentSubjectSaveChild")).prop("checked") == true || SaveParentSubjectSaveChild == true) {
                            HideLoader();
                            if (SelectedRows.length > 0) {
                                SelectedRows.each(function (index, row) {
                                    var selectedItem = SelectedGrid.dataItem(row);

                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").find("td").each(function (index, ItemCell) {
                                        if (ItemCell.classList.value.indexOf("IsRequired") > -1 && HasError == false) {
                                            var CellName = ItemCell.outerHTML.substring(ItemCell.outerHTML.indexOf("data_name") + 11, ItemCell.outerHTML.indexOf("data-item") - 2);
                                            if (selectedItem[CellName] == "" || selectedItem[CellName] == "0" || selectedItem[CellName] == undefined) {
                                                if (HasSelectColumn)
                                                    SelectedGrid.clearSelection();
                                                SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(CellName + " خالی است ");
                                                SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                                                SelectedGrid.showColumn("_ShowError");
                                                SelectedGrid.autoFitColumn("_ShowError");
                                                HasError = true;
                                            }
                                        }
                                    })

                                });

                                if (HasError == true)
                                    ErrorMessage += 'خطاهای موجود در جدول را رفع نمایید\n';
                            }
                            else {
                                HasError = true;
                                ErrorMessage += 'هیچ رکوردی وارد نشده است\n';
                            }
                        }

                        if (!HasError) {
                            if (HasSelectColumn) {
                                var JsonText = "";
                                SelectedRows.each(function (index, row) {
                                    var selectedItem = SelectedGrid.dataItem(row);
                                    JsonText += JSON.stringify(selectedItem) + ",";
                                });

                                JsonGrid.push("[" + JsonText + "]");
                            }
                            else
                                JsonGrid.push(JSON.stringify(SelectedGrid.dataSource.view()));
                            GridNameID.push(GridElement.id);
                        }

                    })
                }

                if (ErrorMessage == "") {
                    $.ajax({
                        type: 'POST',
                        url: "/Desktop/SaveFormEditor",
                        data: {
                            "DataKey": DataKey,
                            "ParentID": ParentID,
                            "RecordID": RecordID,
                            "FormInputName": FormInputName,
                            "FormInputValue": FormInputValue,
                            "ProcessID": ProcessID,
                            "ProcessStepID": ProcessStepID,
                            'SaveChilde': false,
                            "JsonGrid": JsonGrid,
                            'GridName': GridNameID
                        },
                        dataType: 'json',
                        success: function (data) {
                            HideLoader();
                            if (data.Message == "") {
                                popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");

                                if ($("#" + IsCloseFormAffterSave).prop("checked") == false) {

                                    FillForm(data.Record, FormInputName, FormInputValue);
                                    DisableEditorForm(FormInputName, false);
                                    if (FormArraye[3] == 0) {
                                        var AttachmentButton = (ISDetailGridForm ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + DataKey + "_" + ParentID + "_" + RecordID;
                                        $("#" + ElementSenderID).prop('disabled', true);
                                        $("#" + AttachmentButton).prop('disabled', false);


                                        for (var index = 0; index < GridElements.length; index++) {
                                            var DetailName = GridElements[index].indexOf("Detail") > -1 ? "Detail" : "";
                                            var ParentID = GridElements[index].indexOf("Detail") > -1 ? "" : "0";
                                            var GridArr = GridElements[index].replace(DetailName + "MainGrid", "");
                                            $("#" + DetailName + "Create" + GridArr + "_" + ParentID).css('display', 'none');
                                            $("#" + DetailName + "Update" + GridArr + "_" + ParentID).css('display', 'none');
                                            $("#" + DetailName + "Attachment" + GridArr + "_" + ParentID).css('display', 'none');
                                            $("#" + DetailName + "Destroy" + GridArr + "_" + ParentID).css('display', 'none');
                                            var SubGrid = $("#" + GridElements[index]).data("kendoGrid");
                                            SubGrid.dataSource.read();
                                        }

                                        EnableElement(FormDiv, false);

                                        if ($("#" + FileManagerName).length > 0) {
                                            var fileManager = $("#" + FileManagerName).data("kendoFileManager");
                                            fileManager.dataSource.read({ _InnerID: data.RecordID });
                                        }

                                    }
                                    else {
                                        var wnd = $("#" + ElementSenderID.replace(ReplaceWord, "EditorForm").split('_')[0]).data("kendoWindow");
                                        if (wnd == undefined)
                                            wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");

                                        var ElementName = wnd.title().replaceAll(" ", "_");

                                        var LastDropdownlistId = LastWindowID[LastWindowID.length - 1];
                                        wnd.close();
                                        var grid = $("#" + GridName).data("kendoGrid");
                                        if (grid != undefined) {
                                            grid.dataSource.read({ IsReload: true });
                                        }
                                        else {
                                            var dropdownlist = $("#" + ElementName + LastDropdownlistId).length == 0 ? $("#" + ElementName).data("kendoDropDownList") : $("#" + ElementName + LastDropdownlistId).data("kendoDropDownList");
                                            if (dropdownlist != undefined)
                                                dropdownlist.dataSource.read();
                                        }

                                    }
                                }
                                else {
                                    var wnd = $("#" + ElementSenderID.replace(ReplaceWord, "EditorForm").split('_')[0]).data("kendoWindow");
                                    if (wnd == undefined)
                                        wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");

                                    if (wnd == undefined)
                                        wnd = $("#" + ElementSenderID.replace(ReplaceWord, "EditorForm").split('_')[0] + "_0_0").data("kendoWindow");

                                    var ElementName = wnd.title().replaceAll(" ", "_");

                                    var LastDropdownlistId = LastWindowID[LastWindowID.length - 1];
                                    wnd.close();
                                    var grid = $("#" + GridName).data("kendoGrid");
                                    if (grid != undefined) {
                                        grid.dataSource.read({ IsReload: true });
                                    }
                                    else if ($("." + ProcessID).val() != undefined) {
                                        var grid = $("#" + $("." + ProcessID).val()).data("kendoGrid");
                                        if (grid != undefined) {
                                            grid.dataSource.read({ IsReload: true });
                                            if (grid.select().length > 0)
                                                grid.clearSelection();
                                        }
                                    }
                                    else {
                                        var dropdownlist = $("#" + ElementName + LastDropdownlistId).length == 0 ? $("#" + ElementName).data("kendoDropDownList") : $("#" + ElementName + LastDropdownlistId).data("kendoDropDownList");
                                        if (dropdownlist != undefined)
                                            dropdownlist.dataSource.read();
                                    }
                                }
                            }
                            else
                                popupNotification.show(data.Message, "error");

                        },
                        error: function (result) {
                            HideLoader();
                            popupNotification.show(result.responseText, "error");
                        }
                    });
                }
                else {
                    popupNotification.show(ErrorMessage.replace(/\n/g, '<br/>'), "error");
                }
            }
            else {
                if ($("#" + FormDiv + " .k-grid-display-block:visible").length > 0 && RecordID == 0) {
                    $("#" + FormDiv + " .k-grid-display-block:visible").each(async function (index, GridElement) {
                        var SubGridDataKey = GridElement.id.indexOf("Detail") > -1 ? GridElement.id.replace("DetailMainGrid", "").split('_')[0] : GridElement.id.replace("MainGrid", "");
                        var SubGridParentID = GridElement.id.indexOf("Detail") > -1 ? GridElement.id.replace("DetailMainGrid", "").split('_')[1] : "0";
                        var HasError = false;
                        var HasWarning = false;
                        var SelectedGrid = $("#" + GridElement.id).data("kendoGrid");
                        var ColumnName = [];
                        var ParentColumnName = FormInputName;

                        for (let index = 0; index < ParentColumnName.length; index++) {
                            ParentColumnName[index] = ParentColumnName[index].replace("_" + DataKey, "");
                        }

                        for (let index = 0; index < SelectedGrid.columns.length; index++) {
                            if (SelectedGrid.columns[index].field != undefined)
                                ColumnName.push(SelectedGrid.columns[index].field);
                        }

                        var ISDetailGridForm = GridElement.id.indexOf("DetailMainGrid") > -1 ? true : false;
                        var SelectedRows;
                        var HasSelectColumn = true;


                        if (SelectedGrid.columns[0].field == "_ShowError" || (SelectedGrid.columns[0].title == "ردیف" && SelectedGrid.columns[1].field == "_ShowError")) {
                            HasSelectColumn = false;
                            SelectedRows = SelectedGrid.table.find("tr");
                        }
                        else
                            SelectedRows = SelectedGrid.select();
                        var indexRow = 0;
                        var CounterCheckedRow = 0;

                        if ($("#" + GridElement.id.replace("MainGrid", "SaveParentSubjectSaveChild")).prop("checked") == true || SaveParentSubjectSaveChild == true) {
                            HideLoader();
                            if (SelectedRows.length > 0) {
                                await SelectedRows.each(function (index, row) {
                                    var selectedItem = SelectedGrid.dataItem(row);


                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").find("td").each(function (index, ItemCell) {
                                        if (ItemCell.classList.value.indexOf("IsRequired") > -1 && HasError == false) {
                                            var CellName = ItemCell.outerHTML.substring(ItemCell.outerHTML.indexOf("data_name") + 11, ItemCell.outerHTML.indexOf("data-item") - 2);
                                            if (selectedItem[CellName] == null || selectedItem[CellName].toString().replaceAll(' ', '') == "" || selectedItem[CellName] == "0" || selectedItem[CellName] == undefined) {
                                                if (HasSelectColumn)
                                                    SelectedGrid.clearSelection();
                                                SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(CellName + " خالی است ");
                                                SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                                                SelectedGrid.showColumn("_ShowError");
                                                SelectedGrid.autoFitColumn("_ShowError");
                                                HasError = true;
                                            }
                                        }
                                    })

                                    if (HasError == false) {

                                        var ValuesItem = [];

                                        for (let index = 0; index < ColumnName.length; index++) {
                                            ValuesItem.push(selectedItem[ColumnName[index]]);
                                        }

                                        $.ajax({
                                            url: "/Desktop/CheckGridSelecteRowBeforSave",
                                            data: {
                                                'RecordID': 0,
                                                'ParentDataKey': DataKey,
                                                'ParentColumnName': ParentColumnName,
                                                'ParentValuesItem': FormInputValue,
                                                'DataKey': SubGridDataKey,
                                                'FormInputName': ColumnName,
                                                'FormInputValue': ValuesItem
                                            },
                                            type: "POST",
                                            success: function (result) {
                                                CounterCheckedRow++;

                                                if (result.Warning != "") {
                                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #warning_" + selectedItem.id).html(result.Warning);
                                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Warning-row");
                                                    SelectedGrid.showColumn("_ShowWarning");
                                                    SelectedGrid.autoFitColumn("_ShowWarning");
                                                    HasWarning = true;
                                                }

                                                if (result.Error == "") {
                                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
                                                    indexRow = indexRow + 1;
                                                    document.getElementById(GridElement.id.replace("MainGrid", "SelectedRow")).innerHTML = "بررسی " + indexRow + " ردیف صحیح از " + SelectedRows.length + " ردیف "
                                                }
                                                else {
                                                    if (HasSelectColumn)
                                                        SelectedGrid.clearSelection();
                                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(result.Error);
                                                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                                                    SelectedGrid.showColumn("_ShowError");
                                                    SelectedGrid.autoFitColumn("_ShowError");
                                                    HasError = true;
                                                }

                                                if (CounterCheckedRow == SelectedRows.length) {
                                                    HideLoader();
                                                    if (HasError) {
                                                        popupNotification.show('به علت وجود خطا در جدول ذخیره سازی با شکست مواجه شد', "error");
                                                    }
                                                    else {
                                                        SelectedGrid.hideColumn("_ShowError");
                                                        var ClientAnswer = true;
                                                        if (HasWarning) {
                                                            if (confirm("آیا هشدارهای موجود را نادیده میگیرید؟")) {
                                                                SelectedGrid.hideColumn("_ShowWarning");
                                                            }
                                                            else {
                                                                ClientAnswer == false;
                                                            }
                                                        }

                                                        if (ClientAnswer) {
                                                            GridElements.push(GridElement.id);
                                                            if (GridElements.length == $("#" + FormDiv + " .k-grid-display-block:visible").length) {
                                                                $.ajax({
                                                                    type: 'POST',
                                                                    url: "/Desktop/SaveFormEditor",
                                                                    data: {
                                                                        "DataKey": DataKey,
                                                                        "ParentID": ParentID,
                                                                        "RecordID": RecordID,
                                                                        "FormInputName": FormInputName,
                                                                        "FormInputValue": FormInputValue,
                                                                        "SaveChilde": false,
                                                                        "ProcessID": ProcessID,
                                                                        "ProcessStepID": ProcessStepID
                                                                    },
                                                                    dataType: 'json',
                                                                    success: function (data) {
                                                                        HideLoader();
                                                                        if (data.Message == "") {
                                                                            popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");

                                                                            if (FormInputName[0].indexOf("_" + DataKey) == -1)
                                                                                for (let index = 0; index < FormInputName.length; index++) {
                                                                                    FormInputName[index] = FormInputName[index] + "_" + DataKey;
                                                                                }

                                                                            FillForm(data.Record, FormInputName, FormInputValue);
                                                                            DisableEditorForm(FormInputName, false);

                                                                            var AttachmentButton = (ISDetailGridForm ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + DataKey + "_" + ParentID + "_" + RecordID;
                                                                            $("#" + e.sender.element[0].id).prop('disabled', true);
                                                                            $("#" + AttachmentButton).prop('disabled', false);


                                                                            for (var index = 0; index < GridElements.length; index++) {
                                                                                var DetailName = GridElements[index].indexOf("Detail") > -1 ? "Detail" : "";
                                                                                var ChildeParentID = GridElements[index].indexOf("Detail") > -1 ? "" : "0";
                                                                                var GridArr = GridElements[index].replace(DetailName + "MainGrid", "");
                                                                                $("#" + DetailName + "Create" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                                                $("#" + DetailName + "Update" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                                                $("#" + DetailName + "Attachment" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                                                $("#" + DetailName + "Destroy" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                                                var SubGrid = $("#" + GridElements[index]).data("kendoGrid");

                                                                                ColumnName = [];
                                                                                indexRow = 0;
                                                                                CounterCheckedRow = 0;
                                                                                for (let index = 0; index < SubGrid.columns.length; index++) {
                                                                                    if (SubGrid.columns[index].field != undefined)
                                                                                        ColumnName.push(SubGrid.columns[index].field);
                                                                                }

                                                                                HasSelectColumn = true;
                                                                                var SubGridSelectedGrid;
                                                                                if (SubGrid.columns[0].field == "_ShowError" || (SubGrid.columns[0].title == "ردیف" && SubGrid.columns[1].field == "_ShowError")) {
                                                                                    HasSelectColumn = false;
                                                                                    SubGridSelectedGrid = SubGrid.table.find("tr");
                                                                                }
                                                                                else
                                                                                    SubGridSelectedGrid = SubGrid.select();

                                                                                var HasError = 0;
                                                                                var FinalParentID = data.RecordID;

                                                                                SubGridSelectedGrid.each(function () {
                                                                                    var dataItem = SubGrid.dataItem(this);
                                                                                    var ValuesItem = [];

                                                                                    for (let index = 0; index < ColumnName.length; index++) {
                                                                                        ValuesItem.push(dataItem[ColumnName[index]]);
                                                                                    }
                                                                                    $.ajax({
                                                                                        type: 'POST',
                                                                                        url: "/Desktop/SaveFormEditor",
                                                                                        data: {
                                                                                            "DataKey": GridArr,
                                                                                            "ParentID": FinalParentID,
                                                                                            "RecordID": 0,
                                                                                            "FormInputName": ColumnName,
                                                                                            "FormInputValue": ValuesItem,
                                                                                            "SaveChilde": false
                                                                                        },
                                                                                        dataType: 'json',
                                                                                        success: function (data) {
                                                                                            CounterCheckedRow++;

                                                                                            if (HasSelectColumn)
                                                                                                SubGrid.clearSelection();

                                                                                            if (data.Message == "") {
                                                                                                SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Valid-row");
                                                                                                indexRow = indexRow + 1;
                                                                                                document.getElementById(SubGrid.element[0].id.replace("MainGrid", "SelectedRow")).innerHTML = "ذخیره " + indexRow + " ردیف از " + SubGrid.dataSource.total() + " ردیف ";
                                                                                                SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html("");
                                                                                            }
                                                                                            else {
                                                                                                SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html(data.Message);
                                                                                                SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Error-row");
                                                                                                SubGrid.showColumn("_ShowError");
                                                                                                SubGrid.autoFitColumn("_ShowError");
                                                                                                HasError++;
                                                                                            }

                                                                                            if (CounterCheckedRow == SubGrid.dataSource.total())
                                                                                                if (CloseFormAffterSave == true && HasError == 0) {
                                                                                                    var wnd = $("#" + e.sender.element[0].id.replace(ReplaceWord, "EditorForm").split('_')[0]).data("kendoWindow");
                                                                                                    if (wnd == undefined)
                                                                                                        wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");

                                                                                                    var ElementName = wnd.title().replaceAll(" ", "_");

                                                                                                    var LastDropdownlistId = LastWindowID[LastWindowID.length - 1];
                                                                                                    wnd.close();
                                                                                                    var grid = $("#" + GridName).data("kendoGrid");
                                                                                                    if (grid != undefined) {
                                                                                                        grid.dataSource.read({ IsReload: true });
                                                                                                    }
                                                                                                    else if ($("." + ProcessID).val() != undefined) {
                                                                                                        var grid = $("#" + $("." + ProcessID).val()).data("kendoGrid");
                                                                                                        if (grid != undefined) {
                                                                                                            grid.dataSource.read({ IsReload: true });
                                                                                                            if (grid.select().length > 0)
                                                                                                                grid.clearSelection();
                                                                                                        }
                                                                                                    }
                                                                                                    else {
                                                                                                        var dropdownlist = $("#" + ElementName + LastDropdownlistId).length == 0 ? $("#" + ElementName).data("kendoDropDownList") : $("#" + ElementName + LastDropdownlistId).data("kendoDropDownList");
                                                                                                        if (dropdownlist != undefined)
                                                                                                            dropdownlist.dataSource.read();
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                    SubGrid.dataSource.read({ _ParentID: FinalParentID });
                                                                                        },
                                                                                        error: function (result) {
                                                                                            popupNotification.show(result.responseText, "error");
                                                                                        }
                                                                                    });
                                                                                });

                                                                            }


                                                                            $("#" + FormDiv + " .TableAttachment- .DeleteAtt").css('display', 'none');
                                                                            $("#" + FormDiv + " .TableAttachment- .ScannerAtt").css('display', 'none');
                                                                            $("#" + FormDiv + " .TableAttachment- .CameraAtt").css('display', 'none');
                                                                            $("#" + FormDiv + " .TableAttachment- .UploadFile :input").prop('disabled', true);
                                                                        }
                                                                        else
                                                                            popupNotification.show(data.Message, "error");

                                                                    },
                                                                    error: function (result) {
                                                                        HideLoader();
                                                                        popupNotification.show(result.responseText, "error");
                                                                    }
                                                                });
                                                            }

                                                        }
                                                    }
                                                }
                                            },
                                            error: function (result) {
                                                popupNotification.show('ذخیره سازی با شکست مواجه شد', "error");
                                            }
                                        })
                                    }
                                    else {
                                        HideLoader();
                                    }
                                });
                            }
                            else {
                                popupNotification.show('هیچ رکوردی وارد نشده است', "error");
                            }
                        }
                        else {
                            GridElements.push(GridElement.id);
                            if (GridElements.length == $("#" + FormDiv + " .k-grid-display-block:visible").length) {
                                if ($("#" + GridElement.id.replace("MainGrid", "CheckValidChildGrid")).prop("checked") == true) {
                                    if (SelectedRows.length > 0) {
                                        await SelectedRows.each(function (index, row) {

                                            var selectedItem = SelectedGrid.dataItem(row);


                                            SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").find("td").each(function (index, ItemCell) {
                                                if (ItemCell.classList.value.indexOf("IsRequired") > -1 && !HasError) {
                                                    var CellName = ItemCell.outerHTML.substring(ItemCell.outerHTML.indexOf("data_name") + 11, ItemCell.outerHTML.indexOf("data-item") - 2);
                                                    if (selectedItem[CellName] == "" || selectedItem[CellName] == "0" || selectedItem[CellName] == undefined) {
                                                        if (HasSelectColumn)
                                                            SelectedGrid.clearSelection();
                                                        SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(CellName + " خالی است ");
                                                        SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                                                        SelectedGrid.showColumn("_ShowError");
                                                        SelectedGrid.autoFitColumn("_ShowError");
                                                        HasError = true;
                                                    }
                                                }
                                            })

                                            if (HasError == false) {

                                                var ValuesItem = [];

                                                for (let index = 0; index < ColumnName.length; index++) {
                                                    ValuesItem.push(selectedItem[ColumnName[index]]);
                                                }

                                                $.ajax({
                                                    url: "/Desktop/CheckGridSelecteRowBeforSave",
                                                    data: {
                                                        'RecordID': selectedItem.id,
                                                        'ParentDataKey': DataKey,
                                                        'ParentColumnName': ParentColumnName,
                                                        'ParentValuesItem': FormInputValue,
                                                        'DataKey': SubGridDataKey,
                                                        'FormInputName': ColumnName,
                                                        'FormInputValue': ValuesItem
                                                    },
                                                    type: "POST",
                                                    success: function (result) {
                                                        CounterCheckedRow++;

                                                        if (result.Warning != "") {
                                                            SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #warning_" + selectedItem.id).html(result.Warning);
                                                            SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Warning-row");
                                                            SelectedGrid.showColumn("_ShowWarning");
                                                            SelectedGrid.autoFitColumn("_ShowWarning");
                                                            HasWarning = true;
                                                        }

                                                        if (result.Error == "") {
                                                            SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
                                                            indexRow = indexRow + 1;
                                                            document.getElementById(GridElement.id.replace("MainGrid", "SelectedRow")).innerHTML = "بررسی " + indexRow + " ردیف صحیح از " + SelectedRows.length + " ردیف "
                                                        }
                                                        else {
                                                            if (HasSelectColumn)
                                                                SelectedGrid.clearSelection();
                                                            SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(result.Error);
                                                            SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                                                            SelectedGrid.showColumn("_ShowError");
                                                            SelectedGrid.autoFitColumn("_ShowError");
                                                            HasError = true;
                                                        }

                                                        if (CounterCheckedRow == SelectedRows.length) {
                                                            HideLoader();
                                                            if (HasError) {
                                                                popupNotification.show('به علت وجود خطا در جدول ذخیره سازی با شکست مواجه شد', "error");
                                                            }
                                                            else {
                                                                SelectedGrid.hideColumn("_ShowError");
                                                                var ClientAnswer = true;
                                                                if (HasWarning) {
                                                                    if (confirm("آیا هشدارهای موجود را نادیده میگیرید؟")) {
                                                                        SelectedGrid.hideColumn("_ShowWarning");
                                                                    }
                                                                    else {
                                                                        ClientAnswer == false;
                                                                    }
                                                                }

                                                                if (ClientAnswer) {
                                                                    if (GridElements.length == $("#" + FormDiv + " .k-grid-display-block:visible").length) {
                                                                        $.ajax({
                                                                            type: 'POST',
                                                                            url: "/Desktop/SaveFormEditor",
                                                                            data: {
                                                                                "DataKey": DataKey,
                                                                                "ParentID": ParentID,
                                                                                "RecordID": RecordID,
                                                                                "FormInputName": FormInputName,
                                                                                "FormInputValue": FormInputValue,
                                                                                "SaveChilde": false
                                                                            },
                                                                            dataType: 'json',
                                                                            success: function (data) {
                                                                                HideLoader();
                                                                                if (data.Message == "") {
                                                                                    popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");

                                                                                    if (FormInputName[0].indexOf("_" + DataKey) == -1)
                                                                                        for (let index = 0; index < FormInputName.length; index++) {
                                                                                            FormInputName[index] = FormInputName[index] + "_" + DataKey;
                                                                                        }

                                                                                    FillForm(data.Record, FormInputName, FormInputValue);
                                                                                    DisableEditorForm(FormInputName, false);

                                                                                    var AttachmentButton = (ISDetailGridForm ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + DataKey + "_" + ParentID + "_" + RecordID;
                                                                                    $("#" + e.sender.element[0].id).prop('disabled', true);
                                                                                    $("#" + AttachmentButton).prop('disabled', false);


                                                                                    for (var index = 0; index < GridElements.length; index++) {
                                                                                        var DetailName = GridElements[index].indexOf("Detail") > -1 ? "Detail" : "";
                                                                                        var GridArr = GridElements[index].replace(DetailName + "MainGrid", "");
                                                                                        var SubGrid = $("#" + GridElements[index]).data("kendoGrid");

                                                                                        ColumnName = [];
                                                                                        indexRow = 0;
                                                                                        CounterCheckedRow = 0;
                                                                                        for (let index = 0; index < SubGrid.columns.length; index++) {
                                                                                            if (SubGrid.columns[index].field != undefined)
                                                                                                ColumnName.push(SubGrid.columns[index].field);
                                                                                        }

                                                                                        HasSelectColumn = true;

                                                                                        if (SubGrid.columns[0].field == "_ShowError" || (SubGrid.columns[0].title == "ردیف" && SubGrid.columns[1].field == "_ShowError")) {
                                                                                            HasSelectColumn = false;
                                                                                        }

                                                                                        SubGrid.table.find("tr").each(function () {
                                                                                            var dataItem = SubGrid.dataItem(this);
                                                                                            var ValuesItem = [];

                                                                                            for (let index = 0; index < ColumnName.length; index++) {
                                                                                                ValuesItem.push(dataItem[ColumnName[index]]);
                                                                                            }

                                                                                            $.ajax({
                                                                                                type: 'POST',
                                                                                                url: "/Desktop/SaveFormEditor",
                                                                                                data: {
                                                                                                    "DataKey": GridArr,
                                                                                                    "ParentID": data.RecordID,
                                                                                                    "RecordID": 0,
                                                                                                    "FormInputName": ColumnName,
                                                                                                    "FormInputValue": ValuesItem,
                                                                                                    "SaveChilde": false
                                                                                                },
                                                                                                dataType: 'json',
                                                                                                success: function (SubData) {
                                                                                                    CounterCheckedRow++;

                                                                                                    if (HasSelectColumn)
                                                                                                        SubGrid.clearSelection();

                                                                                                    if (SubData.Message == "") {
                                                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Valid-row");
                                                                                                        indexRow = indexRow + 1;
                                                                                                        document.getElementById(SubGrid.element[0].id.replace("MainGrid", "SelectedRow")).innerHTML = "ذخیره " + indexRow + " ردیف از " + SubGrid.dataSource.total() + " ردیف ";
                                                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html("");
                                                                                                    }
                                                                                                    else {
                                                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html(SubData.Message);
                                                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Error-row");
                                                                                                    }

                                                                                                    if (CounterCheckedRow == SubGrid.dataSource.total())
                                                                                                        SubGrid.dataSource.read({ _ParentID: data.RecordID });
                                                                                                },
                                                                                                error: function (result) {
                                                                                                    popupNotification.show(result.responseText, "error");
                                                                                                }
                                                                                            });
                                                                                        });

                                                                                    }

                                                                                    EnableElement(FormDiv, false);
                                                                                }
                                                                                else
                                                                                    popupNotification.show(data.Message, "error");

                                                                            },
                                                                            error: function (result) {
                                                                                HideLoader();
                                                                                popupNotification.show(result.responseText, "error");
                                                                            }
                                                                        });
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    },
                                                    error: function (result) {
                                                        popupNotification.show('ذخیره سازی با شکست مواجه شد', "error");
                                                    }
                                                })
                                            }
                                            else {
                                                HideLoader();
                                            }
                                        });
                                    }
                                    else {
                                        $.ajax({
                                            type: 'POST',
                                            url: "/Desktop/SaveFormEditor",
                                            data: {
                                                "DataKey": DataKey,
                                                "ParentID": ParentID,
                                                "RecordID": RecordID,
                                                "FormInputName": FormInputName,
                                                "FormInputValue": FormInputValue,
                                                "SaveChilde": false,
                                                "ProcessID": ProcessID,
                                                "ProcessStepID": ProcessStepID
                                            },
                                            dataType: 'json',
                                            success: function (data) {
                                                HideLoader();
                                                if (data.Message == "") {

                                                    if (FormInputName[0].indexOf("_" + DataKey) == -1)
                                                        for (let index = 0; index < FormInputName.length; index++) {
                                                            FormInputName[index] = FormInputName[index] + "_" + DataKey;
                                                        }

                                                    FillForm(data.Record, FormInputName, FormInputValue);
                                                    DisableEditorForm(FormInputName, false);

                                                    var AttachmentButton = (ISDetailGridForm ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + DataKey + "_" + ParentID + "_" + RecordID;
                                                    $("#" + e.sender.element[0].id).prop('disabled', true);
                                                    $("#" + AttachmentButton).prop('disabled', false);


                                                    for (var index = 0; index < GridElements.length; index++) {
                                                        var DetailName = GridElements[index].indexOf("Detail") > -1 ? "Detail" : "";
                                                        var ChildeParentID = GridElements[index].indexOf("Detail") > -1 ? "" : "0";
                                                        var GridArr = GridElements[index].replace(DetailName + "MainGrid", "");
                                                        $("#" + DetailName + "Create" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                        $("#" + DetailName + "Update" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                        $("#" + DetailName + "Attachment" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                        $("#" + DetailName + "Destroy" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                        var SubGrid = $("#" + GridElements[index]).data("kendoGrid");

                                                        ColumnName = [];
                                                        indexRow = 0;
                                                        CounterCheckedRow = 0;
                                                        for (let index = 0; index < SubGrid.columns.length; index++) {
                                                            if (SubGrid.columns[index].field != undefined)
                                                                ColumnName.push(SubGrid.columns[index].field);
                                                        }

                                                        HasSelectColumn = true;
                                                        HasError = false;
                                                        var ChildeSelectedRows;
                                                        var CountRowTotal = 0;
                                                        if (SubGrid.columns[0].field == "_ShowError" || (SubGrid.columns[0].title == "ردیف" && SubGrid.columns[1].field == "_ShowError")) {
                                                            HasSelectColumn = false;
                                                            ChildeSelectedRows = SubGrid.table.find("tr");
                                                            CountRowTotal = SubGrid.dataSource.total();
                                                        }
                                                        else {
                                                            ChildeSelectedRows = SubGrid.select();
                                                            CountRowTotal = ChildeSelectedRows.length;
                                                        }

                                                        ChildeSelectedRows.each(function () {
                                                            var dataItem = SubGrid.dataItem(this);
                                                            var ValuesItem = [];

                                                            for (let index = 0; index < ColumnName.length; index++) {
                                                                ValuesItem.push(dataItem[ColumnName[index]]);
                                                            }

                                                            $.ajax({
                                                                type: 'POST',
                                                                url: "/Desktop/SaveFormEditor",
                                                                data: {
                                                                    "DataKey": GridArr,
                                                                    "ParentID": data.RecordID,
                                                                    "RecordID": 0,
                                                                    "FormInputName": ColumnName,
                                                                    "FormInputValue": ValuesItem,
                                                                    "SaveChilde": false
                                                                },
                                                                dataType: 'json',
                                                                success: function (Subdata) {
                                                                    CounterCheckedRow++;

                                                                    if (HasSelectColumn)
                                                                        SubGrid.clearSelection();

                                                                    if (Subdata.Message == "") {
                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Valid-row");
                                                                        indexRow = indexRow + 1;
                                                                        document.getElementById(SubGrid.element[0].id.replace("MainGrid", "SelectedRow")).innerHTML = "ذخیره " + indexRow + " ردیف از " + CountRowTotal + " ردیف ";
                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html("");
                                                                    }
                                                                    else {
                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html(Subdata.Message);
                                                                        SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Error-row");
                                                                        SubGrid.showColumn("_ShowError");
                                                                        SubGrid.autoFitColumn("_ShowError");
                                                                        HasError = true;
                                                                    }


                                                                    if (CounterCheckedRow == CountRowTotal && !HasError) {
                                                                        SubGrid.dataSource.read({ _ParentID: data.RecordID, IsReload: true });
                                                                    }
                                                                },
                                                                error: function (result) {
                                                                    popupNotification.show(result.responseText, "error");
                                                                }
                                                            });
                                                        });

                                                    }


                                                    $("#" + FormDiv + " .TableAttachment- .DeleteAtt").css('display', 'none');
                                                    $("#" + FormDiv + " .TableAttachment- .ScannerAtt").css('display', 'none');
                                                    $("#" + FormDiv + " .TableAttachment- .CameraAtt").css('display', 'none');
                                                    $("#" + FormDiv + " .TableAttachment- .UploadFile :input").prop('disabled', true);


                                                }
                                                else
                                                    popupNotification.show(data.Message, "error");

                                            },
                                            error: function (result) {
                                                HideLoader();
                                                popupNotification.show(result.responseText, "error");
                                            }
                                        });
                                    }
                                }
                                else {
                                    $.ajax({
                                        type: 'POST',
                                        url: "/Desktop/SaveFormEditor",
                                        data: {
                                            "DataKey": DataKey,
                                            "ParentID": ParentID,
                                            "RecordID": RecordID,
                                            "FormInputName": FormInputName,
                                            "FormInputValue": FormInputValue,
                                            "SaveChilde": false,
                                            "ProcessID": ProcessID,
                                            "ProcessStepID": ProcessStepID
                                        },
                                        dataType: 'json',
                                        success: function (data) {
                                            HideLoader();
                                            if (data.Message == "") {

                                                if (FormInputName[0].indexOf("_" + DataKey) == -1)
                                                    for (let index = 0; index < FormInputName.length; index++) {
                                                        FormInputName[index] = FormInputName[index] + "_" + DataKey;
                                                    }

                                                FillForm(data.Record, FormInputName, FormInputValue);
                                                DisableEditorForm(FormInputName, false);

                                                var AttachmentButton = (ISDetailGridForm ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + DataKey + "_" + ParentID + "_" + RecordID;
                                                $("#" + e.sender.element[0].id).prop('disabled', true);
                                                $("#" + AttachmentButton).prop('disabled', false);


                                                for (var index = 0; index < GridElements.length; index++) {
                                                    var DetailName = GridElements[index].indexOf("Detail") > -1 ? "Detail" : "";
                                                    var ChildeParentID = GridElements[index].indexOf("Detail") > -1 ? "" : "0";
                                                    var GridArr = GridElements[index].replace(DetailName + "MainGrid", "");
                                                    $("#" + DetailName + "Create" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                    $("#" + DetailName + "Update" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                    $("#" + DetailName + "Attachment" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                    $("#" + DetailName + "Destroy" + GridArr + "_" + ChildeParentID).css('display', 'none');
                                                    var SubGrid = $("#" + GridElements[index]).data("kendoGrid");

                                                    ColumnName = [];
                                                    indexRow = 0;
                                                    CounterCheckedRow = 0;
                                                    for (let index = 0; index < SubGrid.columns.length; index++) {
                                                        if (SubGrid.columns[index].field != undefined)
                                                            ColumnName.push(SubGrid.columns[index].field);
                                                    }

                                                    HasSelectColumn = true;
                                                    HasError = false;
                                                    var ChildeSelectedRows;
                                                    var CountRowTotal = 0;
                                                    if (SubGrid.columns[0].field == "_ShowError" || (SubGrid.columns[0].title == "ردیف" && SubGrid.columns[1].field == "_ShowError")) {
                                                        HasSelectColumn = false;
                                                        ChildeSelectedRows = SubGrid.table.find("tr");
                                                        CountRowTotal = SubGrid.dataSource.total();
                                                    }
                                                    else {
                                                        ChildeSelectedRows = SubGrid.select();
                                                        CountRowTotal = ChildeSelectedRows.length;
                                                    }

                                                    ChildeSelectedRows.each(function () {
                                                        var dataItem = SubGrid.dataItem(this);
                                                        var ValuesItem = [];

                                                        for (let index = 0; index < ColumnName.length; index++) {
                                                            ValuesItem.push(dataItem[ColumnName[index]]);
                                                        }

                                                        $.ajax({
                                                            type: 'POST',
                                                            url: "/Desktop/SaveFormEditor",
                                                            data: {
                                                                "DataKey": GridArr,
                                                                "ParentID": data.RecordID,
                                                                "RecordID": 0,
                                                                "FormInputName": ColumnName,
                                                                "FormInputValue": ValuesItem,
                                                                "SaveChilde": false
                                                            },
                                                            dataType: 'json',
                                                            success: function (Subdata) {
                                                                CounterCheckedRow++;

                                                                if (HasSelectColumn)
                                                                    SubGrid.clearSelection();

                                                                if (Subdata.Message == "") {
                                                                    SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Valid-row");
                                                                    indexRow = indexRow + 1;
                                                                    document.getElementById(SubGrid.element[0].id.replace("MainGrid", "SelectedRow")).innerHTML = "ذخیره " + indexRow + " ردیف از " + CountRowTotal + " ردیف ";
                                                                    SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html("");
                                                                }
                                                                else {
                                                                    SubGrid.table.find("tr[data-uid='" + dataItem.uid + "'] #error_" + dataItem.id).html(Subdata.Message);
                                                                    SubGrid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass("Error-row");
                                                                    SubGrid.showColumn("_ShowError");
                                                                    SubGrid.autoFitColumn("_ShowError");
                                                                    HasError = true;
                                                                }


                                                                if (CounterCheckedRow == CountRowTotal && !HasError) {
                                                                    SubGrid.dataSource.read({ _ParentID: data.RecordID });
                                                                }
                                                            },
                                                            error: function (result) {
                                                                popupNotification.show(result.responseText, "error");
                                                            }
                                                        });
                                                    });

                                                }
                                                EnableElement(FormDiv, false);

                                            }
                                            else
                                                popupNotification.show(data.Message, "error");

                                        },
                                        error: function (result) {
                                            HideLoader();
                                            popupNotification.show(result.responseText, "error");
                                        }
                                    });
                                }
                            }
                        }
                    })
                }
                else {
                    $.ajax({
                        type: 'POST',
                        url: "/Desktop/SaveFormEditor",
                        data: {
                            "DataKey": DataKey,
                            "ParentID": ParentID,
                            "RecordID": RecordID,
                            "FormInputName": FormInputName,
                            "FormInputValue": FormInputValue,
                            "ProcessID": ProcessID,
                            "ProcessStepID": ProcessStepID
                        },
                        dataType: 'json',
                        success: function (data) {
                            HideLoader();
                            if (data.Message == "") {
                                popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                                FormMainInputID = data.RecordID;

                                if ($("#" + IsCloseFormAffterSave).prop("checked") == false) {

                                    FillForm(data.Record, FormInputName, FormInputValue);
                                    DisableEditorForm(FormInputName, false);
                                    if (FormArraye[3] == 0) {
                                        var AttachmentButton = (ISDetailGridForm ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + DataKey + "_" + ParentID + "_" + RecordID;
                                        $("#" + ElementSenderID).prop('disabled', true);
                                        $("#" + AttachmentButton).prop('disabled', false);


                                        for (var index = 0; index < GridElements.length; index++) {
                                            var DetailName = GridElements[index].indexOf("Detail") > -1 ? "Detail" : "";
                                            var ParentID2 = GridElements[index].indexOf("Detail") > -1 ? "" : "0";
                                            var GridArr = GridElements[index].replace(DetailName + "MainGrid", "");
                                            $("#" + DetailName + "Create" + GridArr + "_" + ParentID2).css('display', 'none');
                                            $("#" + DetailName + "Update" + GridArr + "_" + ParentID2).css('display', 'none');
                                            $("#" + DetailName + "Attachment" + GridArr + "_" + ParentID2).css('display', 'none');
                                            $("#" + DetailName + "Destroy" + GridArr + "_" + ParentID2).css('display', 'none');
                                            var SubGrid = $("#" + GridElements[index]).data("kendoGrid");
                                            SubGrid.dataSource.read();
                                        }

                                        EnableElement(FormDiv, false);

                                        if ($("#" + FileManagerName).length > 0) {
                                            var fileManager = $("#" + FileManagerName).data("kendoFileManager");
                                            fileManager.dataSource.read({ _InnerID: data.RecordID });
                                        }

                                    }
                                    else {
                                        var wnd = $("#" + ElementSenderID.replace(ReplaceWord, "EditorForm").split('_')[0]).data("kendoWindow");
                                        if (wnd == undefined)
                                            wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");

                                        var ElementName = wnd.title().replaceAll(" ", "_");

                                        var LastDropdownlistId = LastWindowID[LastWindowID.length - 1];
                                        wnd.close();
                                        var grid = $("#" + GridName).data("kendoGrid");
                                        if (grid != undefined) {
                                            grid.dataSource.read({ IsReload: true });
                                        }
                                        else {
                                            var dropdownlist = $("#" + ElementName + LastDropdownlistId).length == 0 ? $("#" + ElementName).data("kendoDropDownList") : $("#" + ElementName + LastDropdownlistId).data("kendoDropDownList");
                                            if (dropdownlist != undefined)
                                                dropdownlist.dataSource.read();
                                        }

                                    }
                                }
                                else {
                                    var wnd = $("#" + ElementSenderID.replace(ReplaceWord, "EditorForm").split('_')[0]).data("kendoWindow");
                                    if (wnd == undefined)
                                        wnd = $("#PopupEditorWindow" + LastWindowID[LastWindowID.length - 1]).data("kendoWindow");

                                    var ElementName = wnd.title().replaceAll(" ", "_");

                                    var LastDropdownlistId = LastWindowID[LastWindowID.length - 1];
                                    wnd.close();
                                    var grid = $("#" + GridName).data("kendoGrid");
                                    if (grid != undefined) {
                                        grid.dataSource.read({ IsReload: true });
                                    }
                                    else if ($("." + ProcessID).val() != undefined) {
                                        var grid = $("#" + $("." + ProcessID).val()).data("kendoGrid");
                                        if (grid != undefined) {
                                            grid.dataSource.read({ IsReload: true });
                                            if (grid.select().length > 0)
                                                grid.clearSelection();
                                        }
                                    }
                                    else {
                                        var dropdownlist = $("#" + ElementName + LastDropdownlistId).length == 0 ? $("#" + ElementName).data("kendoDropDownList") : $("#" + ElementName + LastDropdownlistId).data("kendoDropDownList");
                                        if (dropdownlist != undefined)
                                            dropdownlist.dataSource.read();
                                    }
                                }
                            }
                            else
                                popupNotification.show(data.Message, "error");

                        },
                        error: function (result) {
                            HideLoader();
                            popupNotification.show(result.responseText, "error");
                        }
                    });
                }
            }
        }
    }
}
 
function EnableElement(FormName, IsEnable) {

    $("#" + FormName + " .TableAttachment- .DeleteAtt").css('display', (IsEnable ? 'block' : 'none'));
    $("#" + FormName + " .TableAttachment- .ScannerAtt").css('display', (IsEnable ? 'block' : 'none'));
    $("#" + FormName + " .TableAttachment- .CameraAtt").css('display', (IsEnable ? 'block' : 'none'));
    $("#" + FormName + " .TableAttachment- .UploadFile :input").prop('disabled', !IsEnable);


    $("#" + FormName + " .k-grid-display-block").each(function (index, GridElement) {
        var ToolbarNamw = GridElement.id.replace("MainGrid", "MainGridToolbar");
        $("#" + ToolbarNamw + " .CreateButton").css('display', (IsEnable ? 'block' : 'none'));
        $("#" + ToolbarNamw + " .UpdateButton").css('display', (IsEnable ? 'block' : 'none'));
        $("#" + ToolbarNamw + " .DestroyButton").css('display', (IsEnable ? 'block' : 'none'));
        $("#" + ToolbarNamw + " .AttachmentButton").css('display', (IsEnable ? 'block' : 'none'));
        $("#" + ToolbarNamw + " .DisplaySearchButton").css('display', (IsEnable ? 'block' : 'none'));
    })
}

function ShowReportEditorForm(e) {
    var FormArraye = e.sender.element[0].id.split('_');
    var IsDetail = e.sender.element[0].id.indexOf("Detail") > -1 ? true : false;
    var ID = $("#EditorFormDiv" + FormArraye[1] + "_" + FormArraye[2] + "_" + FormArraye[3] + " .IsFieldID input").val().replaceAll(",", "");
    if (ID == 0)
        popupNotification.show("اطلاعات هنوز ذخیره نشده است", "error");
    else
        ShowReport(FormArraye[4], e.sender.element[0].textContent, FormArraye[5], ID);
}

function MainGridDestroyRow(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = e.id.indexOf("DetailDestroy") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailDestroy" : "Destroy";

    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (FormArraye[1] > 0 && ISDetailGridForm ? "_" + FormArraye[1] : "");
    var DataKey = ISDetailGridForm ? GridName.split('_')[0].replace("DetailMainGrid", "") : GridName.replace("MainGrid", "");
    var ParentID = ISDetailGridForm ? GridName.split('_')[1] : "0";

    var grid = $("#" + GridName).data("kendoGrid");
    var ColumnName = [];
    for (let index = 0; index < grid.columns.length; index++) {
        if (grid.columns[index].field != undefined)
            ColumnName.push(grid.columns[index].field);
    }
    var Element = e.sender.element[0];
    while (Element.id.indexOf("EditorFormDiv") == -1 && Element.id.indexOf("_active_cell") == -1 && Element.parentElement != null) {
        Element = Element.parentElement;
    }

    if (grid.select().length > 0) {
        var SelectedRows = grid.select();
        SelectedRows.each(function (index, row) {
            var selectedItem = grid.dataItem(row); 
            var ValuesItem = [];

            if (Element.id !="")
                ParentID = $("#" + Element.id + " .IsFieldID input").val().replaceAll(",", "");

            for (let index = 0; index < ColumnName.length; index++) {
                ValuesItem.push(selectedItem[ColumnName[index]]);
            }

            $.ajax({
                type: 'POST',
                url: "/Desktop/CheckBeforDeleteRecord",
                data: {
                    "_DataKey": DataKey,
                    "_ParentID": ParentID,
                    "_RowID": selectedItem.id,
                    "FormInputName": ColumnName,
                    "FormInputValue": ValuesItem
                },
                dataType: 'json',
                success: function (data) {
                    if (data == "") {
                        grid.removeRow(row);
                    }
                    else {
                        popupNotification.show(data, "error");
                    }
                },
                error: function (result) {
                    popupNotification.show(result.responseText, "error");
                }
            });

        });
    }
    else {
        popupNotification.show("سطری انتخاب نشده است", "info");
    }
}

function RemoveRowGrid(e) {
    var GridName = e.sender.element[0].id;
    var IsDetailGrid = GridName.indexOf("Detail") > -1 ? true : false;
    var DataKey = IsDetailGrid ? GridName.split('_')[0].replace("DetailMainGrid", "") : GridName.replace("MainGrid", "");
    var ParentID = IsDetailGrid ? GridName.split('_')[1] : "0";
    if (e.model.id > 0) {
        if (ParentID > 0) {
            var grid = $("#" + GridName).data("kendoGrid");
            var selectedItem = e.model;
            var row = e;
            var ColumnName = [];
            for (let index = 0; index < grid.columns.length; index++) {
                if (grid.columns[index].field != undefined)
                    ColumnName.push(grid.columns[index].field);
            }
            var ValuesItem = [];

            for (let index = 0; index < ColumnName.length; index++) {
                ValuesItem.push(selectedItem[ColumnName[index]]);
            }

            $.ajax({
                type: 'POST',
                url: "/Desktop/CheckBeforDeleteRecord",
                data: {
                    "_DataKey": DataKey,
                    "_ParentID": ParentID,
                    "_RowID": selectedItem.id,
                    "FormInputName": ColumnName,
                    "FormInputValue": ValuesItem
                },
                dataType: 'json',
                success: function (data) {
                    if (data == "") {

                        $.ajax({
                            type: 'POST',
                            url: "/Desktop/RemoveRow",
                            data: {
                                "_DataKey": DataKey,
                                "_ParentID": ParentID,
                                "_MasterProcessID": "0",
                                "_ProcessStep": "0",
                                "RecordID": e.model.id
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
                                }
                            },
                            error: function (result) {
                                popupNotification.show(result.responseText, "error");
                            }
                        });
                    }
                    else {
                        popupNotification.show(data, "error");
                    }
                },
                error: function (result) {
                    popupNotification.show(result.responseText, "error");
                }
            });
        }
        else {

            $.ajax({
                type: 'POST',
                url: "/Desktop/RemoveRow",
                data: {
                    "_DataKey": DataKey,
                    "_ParentID": ParentID,
                    "_MasterProcessID": "0",
                    "_ProcessStep": "0",
                    "RecordID": e.model.id
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
                    }
                },
                error: function (result) {
                    popupNotification.show(result.responseText, "error");
                }
            });
        }
    }
}

function MainGridShowAttachment(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = e.id.indexOf("DetailAttachment") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailAttachment" : "Attachment";
    var DataKey = FormArraye[0].replace(ReplaceWord, "");
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (FormArraye[1] > 0 && ISDetailGridForm ? "_" + FormArraye[1] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());

    if (selectedItem != null) {
        var ParentID = FormArraye[1];
        $.ajax({
            url: "/Home/Attachment",
            data: {
                DataKey: DataKey,
                ParentID: ParentID,
                RecordID: selectedItem.id
            },
            type: "POST",
            success: function (Result) {

                var wnd = AttachmentWindow;
                wnd.content("<div id='AttachmentWindowDiv'></div>");
                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                wnd.setOptions({
                    width: newWidth - 50,
                    height: newHeight - 50
                });
                wnd.content(Result);
                wnd.center();
                wnd.open();

                $("#FileManager" + DataKey + "_" + ParentID + "_" + selectedItem.id).css("height", "100%");
                $("#FileManager" + DataKey + "_" + ParentID + "_" + selectedItem.id + " .k-filemanager-view").css("height", "100%");
            },
            error: function (result) {

            }
        })
    }
    else {
        popupNotification.show("سطری انتخاب نشده است", "info");
    }
}

function MainGridAutoFitForce(e) {
    var ID = e.id;
    var FormArraye = ID.split('_');
    var ISDetailGridForm = ID.indexOf("DetailAutoFit") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailAutoFit" : "AutoFit";
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (FormArraye[1] > 0 && ISDetailGridForm ? "_" + FormArraye[1] : "");
    var grid = $("#" + GridName).data("kendoGrid");

    for (var i = 0; i < grid.columns.length; i++) {
        if (!grid.columns[i].hidden) {
            grid.autoFitColumn(i);
        }
    }
}

function MainGridDataReload(e) {
    var ID = e.id;
    var FormArraye = ID.split('_');
    var ISDetailGridForm = ID.indexOf("DetailRefresh") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailRefresh" : "Refresh";
    var DataKey = FormArraye[0].replace(ReplaceWord, "");
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (FormArraye[1] > 0 && ISDetailGridForm ? "_" + FormArraye[1] : "");
    var gridInput = $("#" + GridName);
    var grid = gridInput.data("kendoGrid");
    var groupData = []; 
    grid.hideColumn("_ShowError");

    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].values !== undefined) {
            if (groupData.length === 0) groupData.push({ field: grid.columns[i].field, dir: "asc" });

            $.ajax({
                url: "/Desktop/ReloadExternalIDValues",
                data: { '_DataKey': DataKey, '_FieldName': grid.columns[i].field, '_Index': i, '_GridName': gridInput[0].id },
                type: 'POST',
                success: function (result) {
                    var grid = $("#" + result.GridName).data("kendoGrid");
                    grid.columns[result.Index].values = result.Values;
                },
                error: function (result) {
                }
            });
        }
    }
     
    grid.dataSource.group(groupData);
    grid.dataSource.read({ IsReload: true });
    setTimeout(function () {
        grid.dataSource.group([]);
        grid.dataSource.read({ IsReload: true });
    }, 1000);
}

function MainGridCellClose(e) {
    var a = e;
}

function OnCellClose(e) {
    if (e.sender.select != null) {
        var Form = e.sender.content.context.id.replace("MainGrid", "");
        if (Form > 0 && e.container != undefined) {

            if (e.container.length>0)
            {
                var ISDetailGridForm = e.sender.content.context.id.indexOf("DetailMainGrid") > -1 ? true : false;
                var ReplaceWord = ISDetailGridForm ? "DetailMainGrid" : "MainGrid";
                var FormArraye = e.sender.content.context.id.split('_');

                var Element = $("#" + e.sender.content.context.id)[0];
                while (Element.id.indexOf("EditorFormDiv") == -1 && Element.id.indexOf("_active_cell") == -1 && Element.parentElement != null) {
                    Element = Element.parentElement;
                }

                var DataKey = FormArraye[0].replace(ReplaceWord, "");
                var ParentID = ISDetailGridForm ? FormArraye[1] : ($("#" + Element.id + " .IsFieldID input").length>0? $("#" + Element.id + " .IsFieldID input").val().replaceAll(",", ""):0);

                if (e.container[0].classList.value.indexOf("ActiveOnKeyDown") > -1 || ParentID>0) {

                    var grid = $("#" + e.sender.content.context.id).data("kendoGrid");
                    var selectedItem = grid.dataItem(grid.select());
                    var FormInputName = [];
                    var FormInputValue = [];

                    if (selectedItem != null) {

                        $.map(grid.dataSource.options.fields, function (val, i) {
                            if (val.field != undefined && val.field != "_ShowError" && val.field != "_ShowWarning") {
                                FormInputName.push(val.field);
                                FormInputValue.push(selectedItem[val.field]);
                            }
                        });

                        $.ajax({
                            type: 'POST',
                            url: "/Desktop/SaveFormEditor",
                            data: {
                                "DataKey": DataKey,
                                "ParentID": ParentID,
                                "RecordID": selectedItem['شناسه'],
                                "FormInputName": FormInputName,
                                "FormInputValue": FormInputValue,
                                "ProcessID": 0,
                                "ProcessStepID": 0,
                                'SaveChilde': false,
                                "JsonGrid": [],
                                'GridName': IncellGridName,
                                "IsImport": false,
                                "IsCalAutoFillQuery": e.container[0].classList.value.indexOf("ActiveOnKeyDown") > -1 ? true : false
                            },
                            dataType: 'json',
                            success: function (data) {
                                if (data.Message != "")
                                    popupNotification.show(data.Message, "error");
                                else {

                                    if (data.AlarmMessage != "")
                                        popupNotification.show(data.AlarmMessage, "error");

                                    $.map(data.FieldName, function (FieldItem, Index) {
                                        selectedItem[FieldItem] = data.Record[Index];

                                        //var firstItem = grid.dataSource.data()[DataIndex];
                                        var firstItem = e.sender.dataSource.get(selectedItem.id);
                                        firstItem.set(FieldItem, data.Record[Index]);
                                    });

                                    selectedItem['id'] = selectedItem[e.sender.dataSource.options.schema.model.id];
                                    var gridSource = e.sender.dataSource.get(selectedItem.id);
                                    gridSource.dirty = true;
                                }
                            },
                            error: function (result) {
                                popupNotification.show("خطایی رخ داده است", "error");
                            }
                        });
                    }
                } 
            }
        }
    } 
}
function MainGridChange(e) {
    if (e.sender.select != null) {
        var Form = e.sender.content.context.id.replace("MainGrid", "");
        if (Form > 0) {

            var grid = $("#" + e.sender.content.context.id).data("kendoGrid");
            var selectedItem = grid.dataItem(grid.select());
            var ItemSelected = grid.select().length;

            document.getElementById(e.sender.content.context.id.replace("MainGrid", "SelectedRow")).innerHTML = "انتخاب " + ItemSelected + " ردیف از " + grid.dataSource.total() + " ردیف ";
            if (selectedItem != null) {
                if (($("#MainGridCommandButton_" + Form + "_wrapper").length > 0 && selectedItem.مرحله_بعد_فرآیند != undefined) || selectedItem.مرحله_بعد_فرآیند == null) {
                    $("#MainGridCommandButton_" + Form + "_wrapper").css("display", "block");
                    $("#MainGridCommandButton_" + Form + "_optionlist").empty();

                    if (selectedItem.عنوان_مرحله_بعد_فرآیند != "" && selectedItem.عنوان_مرحله_بعد_فرآیند != undefined) {
                        var Arr = $("#MainGridCommandButton_" + Form).attr("data_name").split('_');
                        var ProcessID = Arr[2];
                        $("#NextItemButtonLabel_" + Form)[0].innerText = selectedItem.عنوان_مرحله_بعد_فرآیند;
                        $("#MainGridCommandButton_" + Form + "_optionlist").append('<li class="k-menu-item k-item"><span tabindex="0" class="k-link k-menu-link" id="NextStep_' + Form + "_" + ProcessID + "_" + selectedItem.مرحله_بعد_فرآیند + '" data-overflow="auto" aria-disabled="false onclick="MainGridCommand(this)"><span class="k-menu-link-text">' + selectedItem.عنوان_مرحله_بعد_فرآیند + '</span></span></li>');

                    }
                    else
                        $("#MainGridCommandButton_" + Form + "_wrapper").css("display", "none");
                }
            }
        }
    }
    else if (e.items.length > 0) {
        if (e.action == "itemchange" && IncellGridName != undefined) {

            var ISDetailGridForm = IncellGridName.indexOf("DetailMainGrid") > -1 ? true : false;
            var ReplaceWord = ISDetailGridForm ? "DetailMainGrid" : "MainGrid";
            var FormArraye = IncellGridName.split('_');

            var Element = $("#" + IncellGridName)[0];
            while (Element.id.indexOf("EditorFormDiv") == -1 && Element.id.indexOf("_active_cell") == -1 && Element.parentElement != null) {
                Element = Element.parentElement;
            }

            var DataKey = FormArraye[0].replace(ReplaceWord, "");
            var ParentID = ISDetailGridForm ? FormArraye[1] : $("#" + Element.id + " .IsFieldID input").length==0?"0": $("#" + Element.id + " .IsFieldID input").val().replaceAll(",", "");

            //if (e.items[0].id > 0) {
            if (ParentID != "0" || (ParentID == "0" && e.items[0].id == 0)) {
                    //var grid = $("#" + IncellGridName).data("kendoGrid");
                    //var FormInputName = [];
                    //var FormInputValue = [];

                    //$.map(grid.dataSource.options.fields, function (val, i) {
                    //    if (val.field != undefined && val.field != "_ShowError" && val.field != "_ShowWarning") { 
                    //        FormInputName.push(val.field);
                    //        FormInputValue.push(e.items[0][val.field]);
                    //    }
                    //});

                    //$.ajax({
                    //    type: 'POST',
                    //    url: "/Desktop/SaveFormEditor",
                    //    data: {
                    //        "DataKey": DataKey,
                    //        "ParentID": ParentID,
                    //        "RecordID": e.items[0].id,
                    //        "FormInputName": FormInputName,
                    //        "FormInputValue": FormInputValue,
                    //        "ProcessID": 0,
                    //        "ProcessStepID": 0,
                    //        'SaveChilde': false,
                    //        "JsonGrid": [],
                    //        'GridName': IncellGridName,
                    //        "IsImport": true
                    //    },
                    //    dataType: 'json',
                    //    success: function (data) {
                    //        if (data.Message != "")
                    //            popupNotification.show(data.Message, "error");
                    //        else {
                    //            var IndexRow = 0;

                    //            $.map(grid.dataSource._data, function (Item, DataIndex) {
                    //                if (Item.id == e.items[0].id) { 
                    //                    $.map(data.FieldName, function (FieldItem, Index) {
                    //                        //e.items[0][FieldItem] = data.Record[Index];
                    //                        //grid.dataSource._data[DataIndex][FieldItem] = data.Record[Index];

                    //                        var firstItem = grid.dataSource.data()[DataIndex];
                    //                        firstItem.set(FieldItem, data.Record[Index]);
                    //                    }); 
                    //                }
                    //            });

                    //            //MainGridPreviewSetting(IncellGridName);
                    //        }
                    //    },
                    //    error: function (result) {
                    //        popupNotification.show("خطایی رخ داده است", "error");
                    //    }
                    //});
                }
            //} 
        }
    }
}

function MainGridRequestEnd(e, DataKey) {
    if (e.response != undefined) {
        if (e.response.Errors) {
            popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
        } else {
            var errIndex = -1;
            if (typeof e.response === 'string' || e.response instanceof String) {
                errIndex = e.response.indexOf("error");
            }

            if (errIndex > -1) {
                //grid.dataSource.read();
            } else {
                switch (e.type) {
                    case "create":
                        popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                        var grid = $("#" + IsEditGridName).data("kendoGrid");
                        if (grid != undefined)
                            grid.dataSource.read();
                        break;
                    case "update":
                        popupNotification.show('عملیات ویرایش با موفقیت انجام شد', "success");
                        break;
                    case "destroy":
                        popupNotification.show('عملیات حذف با موفقیت انجام شد', "success");
                        break;
                    default: HideLoader();
                        break;
                }
            }
        }
    }
}

function MainGridEdit(e) {
    FocusFirstItem();
}

function GridAutoFit(grid) {
    var counter = 0;

    for (var i = 0; i < grid.columns.length; i++) {
        if (!grid.columns[i].hidden) {
            counter += 1;
        }
    }

    if (counter > 4) {
        for (var i = 0; i < grid.columns.length; i++) {
            if (!grid.columns[i].hidden) {
                grid.autoFitColumn(i);
            }
        }
    }
}

function MainGridResize(GridName) {
    var grid = $("#" + GridName).data("kendoGrid");
    grid.resize();

    var FormID = GridName.replace("MainGrid", "");

    var newHeight = $("#" + GridName).height();
    var ExtraHeight = $(".k-grid-header").height() + $(".k-grid-pager").height() + $("#" + GridName + " tr:eq(0)").height() + $("#MainGridToolbar" + FormID).height();
    var rowHeight = $("#" + GridName + " tr:last").height();
}

var FirstTime = false;
var GridResizeStart = [];

function MainGridPreviewSetting(e) {
    var GridName;
    if (e.sender != undefined)
    { 
        if (e.sender.content != undefined) {
            GridName = e.sender.content.context.id;
            if (e.sender.content.context.classList.value.indexOf("SearchGrid") > -1) {
                var grid = this;
                grid.tbody.find("tr").dblclick(function (e) {
                    //var DataItem = grid.dataItem(this);  
                    SaveSearchWin(GridName.split('_')[1], false);
                })
            }
        }
        else
            GridName = e;
    }
    else
        GridName = e;

    var grid = $("#" + GridName).data("kendoGrid");
    if (GridName.indexOf("___Chart")==-1)
    GridAutoFit(grid);
    var ParentID = $("." + GridName).val();
    if (GridResizeStart.indexOf(GridName) > -1) {
        var a = 1;
    }
    else {
        GridResizeStart.push(GridName);
        ResizeStart(GridName);
    }


    var FormArraye = GridName.split('_');
    var ISDetailGridForm = GridName.indexOf("DetailMainGrid") > -1 ? true : false;


    var Counter = 1;
    //var grid = this;
    if (grid.dataSource._aggregate != undefined)
        if (grid.dataSource._aggregate.length > 0) {
            var gridData = grid.dataSource.view();
            if (gridData.length > 0) {
                $.each(grid.dataSource._aggregateResult, function (Item) {
                    var Element = $("#SUM_" + Item);
                    if (Element != undefined) {
                        var total = 0;
                        $.each(gridData, function (index, subItem) {
                            total += subItem[Item];
                        })
                        Element.text(total);
                        $("#AVG_" + Item).text(RoundNumber(total / gridData.length, 3));
                    }
                });
            }
        }

    grid.table.find("tr").each(function () {
        var dataItem = grid.dataItem(this);
        //var themeColor = dataItem.Discontinued ? 'success' : 'error';
        //var text = dataItem.Discontinued ? 'available' : 'not available';

        //$(this).find(".badgeTemplate").kendoBadge({
        //    themeColor: themeColor,
        //    text: text,
        //});

        //$(this).find(".rating").kendoRating({
        //    min: 1,
        //    max: 5,
        //    label: false,
        //    selection: "continuous"
        //});



        //$(this).find(".ColorRow").each(function (e) {
        //    var ItemInfoArr = this.id.split('_');
        //    var ID = this.id;
        //    var FileName = ItemInfoArr[5] == 0 ? ItemInfoArr[6] : "";
        //    $.ajax({
        //        url: "/Attachment/GetFileByteWithName",
        //        data: {
        //            '_RecordID': ItemInfoArr[1],
        //            "_InnerID": ItemInfoArr[2],
        //            "_DataKey": ItemInfoArr[3],
        //            "_ParentID": ItemInfoArr[4],
        //            "FileCoreObjectID": ItemInfoArr[5],
        //            "FileName": FileName
        //        },
        //        type: "POST",
        //        success: function (result) {
        //            if (result.length > 0) {
        //                $("#" + ID + " img").attr('src', result)
        //            }
        //        },
        //        error: function (result) {

        //        }
        //    })
        //})
        var ValueColorRow = "";
        var OperatorColorRow = "";
        var ValueColorRow2 = "";
        var OperatorColorRow2 = "";
        var ColumnNameColorRow = "";
        var RowColorSelectedColor = "";

        if (GridName.indexOf("___Chart") > -1) { 
            ColumnNameColorRow = $("#InputColumnNameColorRow_" + GridName).val();
            OperatorColorRow = $("#InputOperatorColorRow_" + GridName).val();
            ValueColorRow = $("#InputValueColorRow_" +GridName).val();
            OperatorColorRow2 = $("#InputOperatorColorRow2_" + GridName).val();
            ValueColorRow2 = $("#InputValueColorRow2_" +GridName).val();
            RowColorSelectedColor = $("#InputRowColorSelectedColor_" + GridName).val();
        }
        else { 
            if (ISDetailGridForm) {
                ColumnNameColorRow = $("#DetailInputColumnNameColorRow_" + FormArraye[0].replace("DetailMainGrid", "") + "_" + FormArraye[1]).val();
                OperatorColorRow = $("#DetailInputOperatorColorRow_" + FormArraye[0].replace("DetailMainGrid", "") + "_" + FormArraye[1]).val();
                ValueColorRow = $("#DetailInputValueColorRow_" + FormArraye[0].replace("DetailMainGrid", "") + "_" + FormArraye[1]).val();
                RowColorSelectedColor = $("#DetailInputRowColorSelectedColor_" + FormArraye[0].replace("DetailMainGrid", "") + "_" + FormArraye[1]).val();
            }
            else {
                ColumnNameColorRow = $("#InputColumnNameColorRow_" + FormArraye[0].replace("MainGrid", "") + "_" + ParentID).val();
                OperatorColorRow = $("#InputOperatorColorRow_" + FormArraye[0].replace("MainGrid", "") + "_" + ParentID).val();
                ValueColorRow = $("#InputValueColorRow_" + FormArraye[0].replace("MainGrid", "") + "_" + ParentID).val();
                RowColorSelectedColor = $("#InputRowColorSelectedColor_" + FormArraye[0].replace("MainGrid", "") + "_" + ParentID).val();
            }

        }

        if (ColumnNameColorRow != "" && ColumnNameColorRow != undefined) {
            var ColumnNameColorRowArr = ColumnNameColorRow.split(',');
            var OperatorColorRowArr = OperatorColorRow.split(',');
            var ValueColorRowArr = ValueColorRow.split(',');
            var OperatorColorRowArr2 = OperatorColorRow2.split(',');
            var ValueColorRowArr2 = ValueColorRow2.split(',');
            var RowColorSelectedColorArr = RowColorSelectedColor.split(',');

            for (var Index = 0; Index < ColumnNameColorRowArr.length-1; Index++) { 
                if (dataItem[ColumnNameColorRowArr[Index]] != undefined) {
                    var RowValue = dataItem[ColumnNameColorRowArr[Index]];
                    if (RowValue == true)
                        RowValue = 1
                    else if (RowValue == false)
                        RowValue = 0

                    if (OperatorColorRowArr2[Index] != "" && OperatorColorRowArr2[Index] != undefined) {
                        if (OperatorColorRowArr[Index] == ">") {
                            switch (OperatorColorRowArr2[Index]) {
                                case "<": {
                                    if (RowValue > ValueColorRowArr[Index] && RowValue < ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "=": {
                                    if (RowValue > ValueColorRowArr[Index] && RowValue == ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "<=": {
                                    if (RowValue > ValueColorRowArr[Index] && RowValue <= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">=": {
                                    if (RowValue > ValueColorRowArr[Index] && RowValue >= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">": {
                                    if (RowValue > ValueColorRowArr[Index] && RowValue > ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "!=": {
                                    if (RowValue > ValueColorRowArr[Index] && RowValue != ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                            }
                        }
                        else if (OperatorColorRowArr[Index] == "<") {

                            switch (OperatorColorRowArr2[Index]) {
                                case "<": {
                                    if (RowValue < ValueColorRowArr[Index] && RowValue < ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "=": {
                                    if (RowValue < ValueColorRowArr[Index] && RowValue == ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "<=": {
                                    if (RowValue < ValueColorRowArr[Index] && RowValue <= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">=": {
                                    if (RowValue < ValueColorRowArr[Index] && RowValue >= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">": {
                                    if (RowValue < ValueColorRowArr[Index] && RowValue > ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "!=": {
                                    if (RowValue < ValueColorRowArr[Index] && RowValue != ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                            }
                        }
                        else if (OperatorColorRowArr[Index] == "=") {
                            switch (OperatorColorRowArr2[Index]) {
                                case "<": {
                                    if (RowValue == ValueColorRowArr[Index] && RowValue < ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "=": {
                                    if (RowValue == ValueColorRowArr[Index] && RowValue == ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "<=": {
                                    if (RowValue == ValueColorRowArr[Index] && RowValue <= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">=": {
                                    if (RowValue == ValueColorRowArr[Index] && RowValue >= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">": {
                                    if (RowValue == ValueColorRowArr[Index] && RowValue > ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "!=": {
                                    if (RowValue == ValueColorRowArr[Index] && RowValue != ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                            }
                        }
                        else if (OperatorColorRowArr[Index] == "!=") {
                            switch (OperatorColorRowArr2[Index]) {
                                case "<": {
                                    if (RowValue != ValueColorRowArr[Index] && RowValue < ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "=": {
                                    if (RowValue != ValueColorRowArr[Index] && RowValue == ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "<=": {
                                    if (RowValue != ValueColorRowArr[Index] && RowValue <= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">=": {
                                    if (RowValue != ValueColorRowArr[Index] && RowValue >= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">": {
                                    if (RowValue != ValueColorRowArr[Index] && RowValue > ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "!=": {
                                    if (RowValue != ValueColorRowArr[Index] && RowValue != ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                            }
                        }
                        else if (OperatorColorRowArr[Index] == ">=") {
                            switch (OperatorColorRowArr2[Index]) {
                                case "<": {
                                    if (RowValue >= ValueColorRowArr[Index] && RowValue < ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "=": {
                                    if (RowValue >= ValueColorRowArr[Index] && RowValue == ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "<=": {
                                    if (RowValue >= ValueColorRowArr[Index] && RowValue <= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">=": {
                                    if (RowValue >= ValueColorRowArr[Index] && RowValue >= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">": {
                                    if (RowValue >= ValueColorRowArr[Index] && RowValue > ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "!=": {
                                    if (RowValue >= ValueColorRowArr[Index] && RowValue != ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                            }
                        }
                        else if (OperatorColorRowArr[Index] == "<=") {
                            switch (OperatorColorRowArr2[Index]) {
                                case "<": {
                                    if (RowValue <= ValueColorRowArr[Index] && RowValue < ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "=": {
                                    if (RowValue <= ValueColorRowArr[Index] && RowValue == ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "<=": {
                                    if (RowValue <= ValueColorRowArr[Index] && RowValue <= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">=": {
                                    if (RowValue <= ValueColorRowArr[Index] && RowValue >= ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case ">": {
                                    if (RowValue <= ValueColorRowArr[Index] && RowValue > ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                                case "!=": {
                                    if (RowValue <= ValueColorRowArr[Index] && RowValue != ValueColorRowArr2[Index])
                                        grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        if (OperatorColorRowArr[Index] == ">") {
                            if (RowValue > ValueColorRowArr[Index])
                                grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                        }
                        else if (OperatorColorRowArr[Index] == "<") {
                            if (RowValue < ValueColorRowArr[Index])
                                grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                        }
                        else if (OperatorColorRowArr[Index] == "=") {
                            if (RowValue == ValueColorRowArr[Index])
                                grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                        }
                        else if (OperatorColorRowArr[Index] == "<=") {
                            if (RowValue <= ValueColorRowArr[Index])
                                grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                        }
                        else if (OperatorColorRowArr[Index] == ">=") {
                            if (RowValue >= ValueColorRowArr[Index])
                                grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                        }
                        else if (OperatorColorRowArr[Index] == "!=") {
                            if (RowValue != ValueColorRowArr[Index])
                                grid.table.find("tr[data-uid='" + dataItem.uid + "']").addClass(RowColorSelectedColorArr[Index]);
                        }

                    }
                }
            }
        }

        $(this).find(".RowNumber").each(function (e) {
            this.innerHTML = Counter++;
        })

        $(this).find(".GridImage").each(function (e) {
            var ItemInfoArr = this.id.split('_');
            var ID = this.id;
            var FileName = ItemInfoArr[5] == 0 ? ItemInfoArr[6] : "";
            if (ItemInfoArr[2] != "null") {
                $.ajax({
                    url: "/Attachment/GetFileByteWithName",
                    data: {
                        '_RecordID': ItemInfoArr[1],
                        "_InnerID": ItemInfoArr[2],
                        "_DataKey": ItemInfoArr[3],
                        "_ParentID": ItemInfoArr[4],
                        "FileCoreObjectID": ItemInfoArr[5],
                        "FileName": FileName
                    },
                    type: "POST",
                    success: function (result) {
                        if (result.length > 0 && result.indexOf("MainDivision") == -1) {
                            $("#" + ID + " img").attr('src', result)
                        }
                    },
                    error: function (result) {

                    }
                })
            }
        })


        $(this).find(".GridAttachment").each(function (e) {
            var ItemInfoArr = this.id.split('_');
            var ID = this.id;
            if (ID != undefined && ID != '') {

                var FileName = ItemInfoArr[5] == 0 ? ItemInfoArr[6] : "";
                $.ajax({
                    url: "/Attachment/GetCountAttachment",
                    data: {
                        'RecordID': ItemInfoArr[1],
                        "InnerID": ItemInfoArr[2]
                    },
                    type: "POST",
                    success: function (result) {
                        if (result > 0) {
                            $("#" + ID + " span").addClass('k-icon k-i-attachment');
                        }
                    },
                    error: function (result) {

                    }
                })
            }
        })


        $(this).find(".PlaqueValue").each(function (e) {
            var ItemInfoArr = this.id.split('_');
            var ID = this.id;

        })


        $(this).find(".GridSparklineChart").each(function (e) {
            var ItemInfoArr = this.id.split('_');
            $(this).kendoSparkline({
                legend: {
                    visible: false
                },
                data: [ItemInfoArr[2]],
                type: "bar",
                chartArea: {
                    margin: 0,
                    width: 180,
                    background: "transparent"
                },
                seriesDefaults: {
                    labels: {
                        visible: true,
                        format: '{0}%',
                        background: 'none'
                    }
                },
                categoryAxis: {
                    majorGridLines: {
                        visible: false
                    },
                    majorTicks: {
                        visible: false
                    }
                },
                valueAxis: {
                    type: "numeric",
                    min: ItemInfoArr[3],
                    max: ItemInfoArr[4],
                    visible: false,
                    labels: {
                        visible: false
                    },
                    minorTicks: { visible: false },
                    majorGridLines: { visible: false }
                },
                tooltip: {
                    visible: false
                }
            })
        });

        kendo.bind($(this), dataItem);
    });
}


function ShowGridAttachmentColumn(e) {
    var ItemInfoArr = e.id.split('_');
    ShowAttachmentForm(ItemInfoArr[1], 0, ItemInfoArr[2]);
}


var ResizeTime;
var ResizeTimeout = false;
var ResizeTimeDelta = 350;


function ResizeStart(GridName) {
    ResizeTime = new Date();
    if (ResizeTimeout == false) {
        ResizeTimeout = true;
        setTimeout(ResizeEnd(GridName), ResizeTimeDelta);
    }
}

function ResizeEnd(GridName) {
    //if (new Date() - ResizeTime < ResizeTimeDelta) {
    //    setTimeout(ResizeEnd(GridName), ResizeTimeDelta);
    //} else {
    //    ResizeTimeout = false;
    MainGridResize(GridName);
    /*    }*/
}

function EditorFormSelectGroupButton(e) {
    var ElementName = e.sender.element.context.id.replace("Button", "");
    var _Value = e.indices[0] == UpdateInTable || e.indices[0] == OpenAttachmentTable ? $("#" + ElementName)[0].value : "0";
    //var _Value = e.item.id == "Edit" || e.item.id == "Attachment" ? $("#" + ElementName)[0].value : "0";
    var RelatedTable = $("#DropDownList" + ElementName)[0].value.replace("/", "");
    var TableIDValue = ElementName.split("_");
    TableIDValue = TableIDValue[TableIDValue.length - 1];

    if (e.item.id == "Reload") {
        var dropdownlist = $("#" + ElementName).data("kendoDropDownList");
        dropdownlist.dataSource.read();
    }
    else if (e.item.id == "Edit" || e.item.id == "Add") {
        LastWindowID.push(TableIDValue);
        OpenEditorForm(RelatedTable, 0, _Value, false, false, ElementName.replace(TableIDValue, ""));
    }
    else if (e.item.id == "Attachment" && _Value > 0) {
        $.ajax({
            url: "/Home/Attachment",
            data: {
                DataKey: RelatedTable,
                ParentID: 0,
                RecordID: _Value
            },
            type: "POST",
            success: function (Result) {

                var wnd = AttachmentWindow;
                wnd.content("<div id='AttachmentWindowDiv'></div>");
                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                wnd.setOptions({
                    width: newWidth - 50,
                    height: newHeight - 50
                });
                wnd.content(Result);
                wnd.center();
                wnd.open();
            },
            error: function (result) {

            }
        })
    }

}


var IsReloaded_RTB = false;
function Reload_RTB_Click(e) {
    var ElementName = e.id.replace("Reload_", "");
    var TableIDValue = ElementName.split("_");
    TableIDValue = TableIDValue[TableIDValue.length - 1];
    var Element = e;
    if ($("#RelatedField" + ElementName).length > 0) {
        while (Element.parentElement != undefined && Element.parentElement != null && Element.id.indexOf("EditorFormDiv") == -1) {
            Element = Element.parentElement;
        }
    }
    var RelatedFieldElement = $("#RelatedField" + ElementName).val() + "_" + TableIDValue;
    IsReloaded_RTB = false;
    AutoCompleteDropDownList(Element.id, RelatedFieldElement, TableIDValue);
    if (IsReloaded_RTB != true)
    { 
        var dropdownlist = $("#DropDownList" + ElementName).length == 0 ? $("#DropDownList" + ElementName.replace("_" + TableIDValue, "")).data("kendoDropDownList") : $("#" + ElementName).data("kendoDropDownList");
        if (dropdownlist != undefined)
            dropdownlist.dataSource.read({ IsReload: true });
    }
}

function Attachment_RTB_Click(e) {
    var ElementName = e.id.replace("Attachment_", "");
    var _Value = 0;
    var RelatedTable = 0;

    var TableIDValue = ElementName.split("_");
    TableIDValue = TableIDValue[TableIDValue.length - 1];

    if ($("#DropDownList" + ElementName).length == 0) {
        RelatedTable = $("#DropDownList" + ElementName.replace("_" + TableIDValue, ""))[0].value.replace("/", "");
        _Value = $("#" + ElementName.replace("_" + TableIDValue, ""))[0].value;
    }
    else {
        RelatedTable = $("#DropDownList" + ElementName)[0].value.replace("/", "");
        _Value = $("#" + ElementName)[0].value;
    }

    if (_Value > 0)
        ShowAttachmentForm(RelatedTable, 0, _Value);
}

function Edit_RTB_Click(e) {
    var ElementName = e.id.replace("Edit_", "");
    var _Value = 0;
    var RelatedTable = 0;
    var TableIDValue = ElementName.split("_");
    TableIDValue = TableIDValue[TableIDValue.length - 1];
    LastWindowID.push(TableIDValue);
    var TitleWin = "";

    if ($("#DropDownList" + ElementName).length == 0) {
        RelatedTable = $("#DropDownList" + ElementName.replace("_" + TableIDValue, ""))[0].value.replace("/", "");
        TitleWin = ElementName.replace("_" + TableIDValue, "");
        _Value = $("#" + ElementName.replace("_" + TableIDValue, ""))[0].value;
    }
    else {
        RelatedTable = $("#DropDownList" + ElementName)[0].value.replace("/", "");
        TitleWin = ElementName.replace(TableIDValue, "");
        _Value = $("#" + ElementName)[0].value;
    }

    OpenEditorForm(RelatedTable, 0, _Value, false, false, TitleWin);
}

function Add_RTB_Click(e) {

    var ElementName = e.id.replace("Add_", "");
    var TableIDValue = ElementName.split("_");
    TableIDValue = TableIDValue[TableIDValue.length - 1];
    var RelatedTable = 0;
    LastWindowID.push(TableIDValue);
    var TitleWin = "";

    if ($("#DropDownList" + ElementName).length == 0) {
        RelatedTable = $("#DropDownList" + ElementName.replace("_" + TableIDValue, ""))[0].value.replace("/", "");
        TitleWin = ElementName.replace("_" + TableIDValue, "");
    }
    else {
        RelatedTable = $("#DropDownList" + ElementName)[0].value.replace("/", "");
        TitleWin = ElementName.replace(TableIDValue, "");
    }

    OpenEditorForm(RelatedTable, 0, "0", false, false, TitleWin);
}

var IsRunCamera = [];
function PlayCamera(element) {
    var Image = $("#WebcamDiv" + element);
    Webcam.set({
        width: Image.width(),
        height: Image.height(),
        image_format: 'jpeg',
        jpeg_quality: 1800
    });
    Webcam.attach("WebcamDiv" + element);

    $("#WebcamDiv" + element + " video").css("transform", "scaleX(1) scaleY(1)");
    $("#WebcamDiv" + element + " video").css("width", "100%"); 
    $("#WebcamDiv" + element + " video").css("height", "100%");
}

function take_snapshot(ElementName) {
    Webcam.snap(function (data_uri) {
        $("#ImageUpload" + ElementName).attr('src', data_uri)
    });
}

function DeleteAtt_Click(e) {
    var ID = e.id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], InnerID = FileArr[4], FileCoreObjectID = FileArr[5];
    var ElementName = ID.replace("Delete", "");
    $.ajax({
        url: "/Attachment/DeleteTableAttachement",
        data: {
            '_RecordID': RecordID,
            "_InnerID": InnerID,
            "_DataKey": DataKey,
            "_ParentID": ParentID,
            "FileCoreObjectID": FileCoreObjectID
        },
        type: "POST",
        success: function (result) {
            if (result.length > 0) {
                $("#WebcamDiv" + ElementName).css("display", "none");
                $("#ImageUpload" + ElementName).css("display", "block");
                $("#ImageUpload" + ElementName).attr('src', result);
                var FileManager = $("#FileManager" + DataKey + "_" + ParentID + "_" + InnerID).data("kendoFileManager");
                if (FileManager != undefined)
                    FileManager.refresh();
            }
        },
        error: function (result) {

        }
    })
}


function ScannerAtt_Click(e) {

    var ID = e.id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], InnerID = FileArr[4], FileCoreObjectID = FileArr[5];
    var ElementName = ID.replace("Scanner", "ImageUpload");
    var ImageSource = $("#" + ElementName).attr('src')
    var ScanCounter = 0;

    var wsImpl = window.WebSocket || window.MozWebSocket;

    window.ws = new wsImpl('ws://localhost:8181/');
    ws.onmessage = function (e) {
        if (typeof e.data === "string") {
            //IF Received Data is String
        }
        else if (e.data instanceof ArrayBuffer) {
            //IF Received Data is ArrayBuffer
        }
        else if (e.data instanceof Blob) {

            ScanCounter++;

            var f = e.data;
            var reader = new FileReader();
            reader.onload = function (e) {
                $("#" + ElementName).attr('src', e.target.result);

                $.ajax({
                    url: "/Attachment/SaveCamera",
                    data: {
                        'ImageData': $("#" + ElementName).attr('src'),
                        '_RecordID': RecordID,
                        "_InnerID": InnerID,
                        "_DataKey": DataKey,
                        "_ParentID": ParentID,
                        "FileCoreObjectID": FileCoreObjectID
                    },
                    type: "POST",
                    success: function (result) {
                        if (result != true) {
                            popupNotification.show('ذخیره سازی با خطا مواجه شد', "error");
                            $("#" + ElementName).attr('src', ImageSource)
                        }
                    },
                    error: function (result) {

                    }
                })
            }
            reader.readAsDataURL(f);
        }
    };
    ws.onopen = function () {
        //Do whatever u want when connected succesfully
        ws.send("1100");
    };
    ws.onclose = function () {
        $('.ScannerDalert').modal('show');
    };
}


function CameraAtt_Click(e,IsColumnCell) {

    var ID = e.id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], InnerID = FileArr[4], FileCoreObjectID = FileArr[5];
    var ElementName = ID.replace("Camera", "");
    var ImageSource = $("#ImageUpload" + ElementName).attr('src')
    if (IsRunCamera.indexOf(ElementName) == -1) {
        IsRunCamera.push(ElementName);
        $("#ImageUpload" + ElementName).css("display", "none");
        $("#" + ID + " span").addClass('k-i-camera');
        $("#" + ID + " span").removeClass("fa fa-camera-web");
        $("#WebcamDiv" + ElementName).css("display", "flex");
        PlayCamera(ElementName);
    }
    else {
        IsRunCamera[IsRunCamera.indexOf(ElementName)] = "";
        $("#" + ID + " span").removeClass('k-i-camera');
        $("#" + ID + " span").addClass("fa fa-camera-web");
        $("#WebcamDiv" + ElementName).css("display", "none");
        $("#ImageUpload" + ElementName).css("display", "block");
        take_snapshot(ElementName);

        if (IsColumnCell == true) { 
            var grid = $("#" + IncellGridName).data("kendoGrid");
            if (grid != undefined) {
                var selectedItem = grid.dataItem(grid.select());
                if (selectedItem != undefined)
                    InnerID = selectedItem.id;
            }
        }
        

        $.ajax({
            url: "/Attachment/SaveCamera",
            data: {
                'ImageData': $("#ImageUpload" + ElementName).attr('src'),
                '_RecordID': RecordID,
                "_InnerID": InnerID,
                "_DataKey": DataKey,
                "_ParentID": ParentID,
                "FileCoreObjectID": FileCoreObjectID
            },
            type: "POST",
            success: function (result) {
                if (result != true) {
                    popupNotification.show('ذخیره سازی با خطا مواجه شد', "error");
                    $("#ImageUpload" + ElementName).attr('src', ImageSource)
                }
                else {
                    $(".ImageUpload" + ElementName).val($("#ImageUpload" + ElementName).attr('src'));
                    $("#ImageUpload" + ElementName)[0].src = $("#ImageUpload" + ElementName).attr('src');
                }
            },
            error: function (result) {

            }
        })

    }
}

function DownloadAtt_Click(e) {

    var ID = e.id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], InnerID = FileArr[4], FileCoreObjectID = FileArr[5];
    var FileName = $("." + e.id.replace("Download_", "ImageUpload_"))[0].id.replace("_" + DataKey, "");
    window.open(
        '/Attachment/DownloadFile?DataKey=' + DataKey + "&&ParentID=" + ParentID + "&&RecordID=" + RecordID + "&&InnerID=" + InnerID + "&&FileNameDownload=" + FileName,
        '_blank'  
    ); 
 
}


var RowPicID;
function GridCameraAtt_Click(e) {
    var IDArr = e.id.split("_"); 
    e.parentElement.parentElement.parentElement
    var CoreName = "";
    for (var Index = 5; Index < IDArr.length; Index++) {
        CoreName += IDArr[Index];
        if (Index != IDArr.length - 1)
            CoreName += "_";
    }
    var FileArr = e.dataset.item.split("_");
    var Datakey = FileArr[0];
    var ParentID = FileArr[1];
    var RecordID = FileArr[2];
    var InnerID = FileArr[3];
    var CoreObjectID = FileArr[4];

    var grid = $("#MainGrid" + Datakey).data("kendoGrid");

    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != undefined) {
        RowPicID = e.id.replace("GridCameraButton_", "Image_");
        var DeleteButton = "Delete_" + Datakey + "_" + ParentID + "_" + RecordID + "_" + InnerID + "_" + CoreObjectID;
        var ScannerButton = "Scanner_" + Datakey + "_" + ParentID + "_" + RecordID + "_" + InnerID + "_" + CoreObjectID;
        var CameraButton = "Camera_" + Datakey + "_" + ParentID + "_" + RecordID + "_" + InnerID + "_" + CoreObjectID;
        var ImageID = "ImageUpload_" + Datakey + "_" + ParentID + "_" + RecordID + "_" + InnerID + "_" + CoreObjectID;
        var WebcamDiv = "WebcamDiv_" + Datakey + "_" + ParentID + "_" + RecordID + "_" + InnerID + "_" + CoreObjectID;
        var ActionData = "_DataKey=" + Datakey + "&&_RecordID=" + RecordID + "&&_InnerID=" + InnerID + "&&_ParentID=" + ParentID + "&&FileName=" + CoreName;

        $(".MaxFileSizeG-C-C-C-B").text(FileArr[5]);
        $(".AllowedExtensionsG-C-C-C-B").text(FileArr[6]);
        $(".DeleteAttG-C-C-C-B")[0].id = DeleteButton;
        $(".ScannerAttG-C-C-C-B")[0].id = ScannerButton;
        $(".CameraAttG-C-C-C-B")[0].id = CameraButton;
        $("#InputG-C-C-C-B").addClass(ImageID);
        $(".UploadFile_imgG-C-C-C-B")[0].id = ImageID;
        $(".UploadFile_imgDivG-C-C-C-B")[0].id = WebcamDiv;
        $(".UploadFile_imgG-C-C-C-B")[0].src = $("#" + e.id.replace("GridCameraButton_", "Image_") + " img")[0].src;
        var Upload = $("#filesG-C-C-C-B").data("kendoUpload");
        Upload.options.async.saveUrl = Upload.options.async.saveUrl.replace("Length=0", ActionData);
        GridCellColumnPicTempWin.center().open();
    }
    else 
        popupNotification.show('ردیفی انتخاب نشده است', "error");
}

function CloseGridCellColumnPicTempWin(e) {
    var WinName = e.sender.element[0].id;
    $("#" + RowPicID + " img")[0].src = $(".UploadFile_imgG-C-C-C-B")[0].src;
}

function OnClickClearFormWhithOutFixItem(e) {
    var ReplaceWord = e.sender.element[0].id.indexOf("DetailFixItemClearFormButton") > -1 ? "DetailFixItemClearFormButton" : "FixItemClearFormButton";
    var FormName = e.sender.element[0].id.replace(ReplaceWord, "EditorFormDiv");
    ClearEditorForm(e.sender.element[0].id.split("_")[0].replace(ReplaceWord, ""), FormName, true);
}

function OnclickNewRecordEditorForm(e) {
    var ReplaceWord = e.sender.element[0].id.indexOf("DetailNewRecordButton") > -1 ? "DetailNewRecordButton" : "NewRecordButton";
    var FormName = e.sender.element[0].id.replace(ReplaceWord, "EditorFormDiv");
    ClearEditorForm(e.sender.element[0].id.split("_")[0].replace(ReplaceWord, ""), FormName, false);
}

function ClearEditorForm(DataKey, FormName, IsFixedItem, FixedField) {
    var FormArr = FormName.replace("EditorFormDiv", "").split("_");

    var FixedItemArr = [];
    if ($("#FixItemClearForm_" + FormArr[0] + "_" + FormArr[1]).val() != undefined)
        FixedItemArr = $("#FixItemClearForm_" + FormArr[0] + "_" + FormArr[1]).val().replaceAll('،', ',').split(',');
    if (FixedField != undefined && FixedField != "")
        FixedItemArr.push(FixedField);
    var GridElements = [];



    $("#" + FormName + " span").removeClass("RequiredField");
    $("#" + FormName + " .DivNationalCode").each(function (index, DivNationalCodeElement) {
        DivNationalCodeElement.innerHTML = "";
    })

    $("#" + FormName + " .k-grid-display-block").each(function (index, GridElement) {
        GridElements.push(GridElement.id); 
        document.getElementById(GridElement.id.replace("MainGrid", "SelectedRow")).innerHTML = "";
    })

    if (GridElements.length > 0) {
        $.ajax({
            url: "/Desktop/ClearGrid",
            data: {
                Grids: GridElements.join(','),
            },
            type: "POST",
            success: function (Result) {
                for (var Index = 0; Index < GridElements.length; Index++) {
                    var grid = $("#" + GridElements[Index]).data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                        grid.hideColumn("_ShowError");
                    }
                }
            },
            error: function (result) {

            }
        })
    }

    EnableElement(FormName, true);
    var SaveButton = (FormName.indexOf("Detail") > -1 ? "DetailSaveButton" : "SaveButton") + FormName.substring(FormName.indexOf(DataKey + "_"), FormName.length);
    var AttachmentButton = (FormName.indexOf("Detail") > -1 ? "DetailAttachmentEditorFormButton" : "AttachmentEditorFormButton") + FormName.substring(FormName.indexOf(DataKey + "_"), FormName.length);

    $(".PlaqueInput").css("background-color", "#fff");

    $(".SaveButton").each(function (Index, Item) {
        if (Item.id.indexOf(SaveButton) > -1)
            $(Item).prop('disabled', false);
    })
 

    $("#" + AttachmentButton).prop('disabled', true);
    if ($(".Logo_HeaderLayout").length > 0)
        $("#" + FormName + " .UploadFile_img").attr('src', $(".Logo_HeaderLayout")[0].src);
    else
        $("#" + FormName + " .UploadFile_img").attr('src', "");


    var FormInputName = [];
    var FormInputValue = [];
    $("textarea").prop('enable', true);
    GetInputValue(FormName + " .FormItemInput", FormInputName, FormInputValue)
    DisableEditorForm(FormInputName, true);

    $.ajax({
        url: "/Desktop/ClearEditorForm",
        data: {
            DataKey: DataKey,
            FormInputName: FormInputName,
            FormInputValue: FormInputValue
        },
        type: "POST",
        success: function (Result) {
            FillForm(Result, FormInputName, FormInputValue, IsFixedItem, FixedItemArr, DataKey);
            $("#" + FormName + " .IsFieldID input").val(0);
            if ($("#" + FormName + " .ActiveLoadElement").length > 0) {
                var ActiveLoadElementArr = $("#" + FormName + " .ActiveLoadElement");
                for (var Index = 0; Index < ActiveLoadElementArr.length; Index++) {
                    if (ActiveLoadElementArr[Index].id == "") {
                        Index++;
                    }
                    ElementChange(ActiveLoadElementArr[Index], true)
                } 
            }
        },
        error: function (result) {

        }
    })

    FocusFirstItem();
}

function DisableEditorForm(FormInputName, IsEnable) {
    for (let index = 0; index < FormInputName.length; index++) {
        Element = $("#" + FormInputName[index]);
        if (Element.hasClass("ISReadOnlyField") == false) {

            if (Element.data("kendoAutoComplete") != undefined) {
                Element.data("kendoAutoComplete").enable(IsEnable);
            }
            else if (Element.data("kendoComboBox") != undefined) {
                Element.data("kendoComboBox").enable(IsEnable);
            }
            else if (Element.data("kendoDropDownList") != undefined) {
                Element.data("kendoDropDownList").enable(IsEnable);
            }
            else if (Element.data("kendoNumericTextBox") != undefined) {
                Element.data("kendoNumericTextBox").enable(IsEnable);
            }
            else if (Element.data("kendoDropDownTree") != undefined) {
                Element.data("kendoDropDownTree").enable(IsEnable);
            }
            else if (Element.data("kendoEditor") != undefined) {
                Element.data("kendoEditor").enable(IsEnable);
            }
            else if (Element.data("kendoSwitch") != undefined) {
                Element.data("kendoSwitch").enable(IsEnable);
            }
            else if (Element.data("kendoTextArea") != undefined) {
                Element.data("kendoTextArea").enable(IsEnable);
            }
            else if (Element.data("kendoTextBox") != undefined) {
                Element.data("kendoTextBox").enable(IsEnable);
            }
            else if ($("#_Image" + FormInputName[index])[0] != undefined) {
                //$("#_Image" + FormInputName[index])[0].src = RecordValue[index];
                //$("#" + FormInputName[index])[0].value = RecordValue[index];
            }
            else {
                if (Element.hasClass("isReadonly")) {

                }
                else {
                    Element.prop('enable', IsEnable);
                    Element.prop('disabled', !IsEnable);
                }
            }

        }

    }
}



function FillForm(NewValue, FormInputName, FormInputValue, IsFixedItem = false, FixedItemArr, DataKey) {
    for (let index = 0; index < NewValue.length; index++) {
        //if (NewValue[index] != FormInputValue[index]) {
        var isFillInput = true;
        if (IsFixedItem && FixedItemArr.indexOf(FormInputName[index].replace("_" + DataKey, "")) > -1) {
            isFillInput = false;
        }
        if (isFillInput) {
            Element = $("#" + FormInputName[index]);
            if (Element.data("kendoAutoComplete") != undefined) {
                Element.data("kendoAutoComplete").value(NewValue[index]);
            }
            else if (Element.data("kendoComboBox") != undefined) {
                Element.data("kendoComboBox").value(NewValue[index]);
            }
            else if (Element.data("kendoDropDownList") != undefined) {
                Element.data("kendoDropDownList").value(NewValue[index]);
            }
            else if (Element.data("kendoNumericTextBox") != undefined) {
                Element.data("kendoNumericTextBox").value(NewValue[index] == "" ? 0 : NewValue[index]);
            }
            else if (Element.data("kendoDropDownTree") != undefined) {
                Element.data("kendoDropDownTree").dataSource.filter({}).value(NewValue[index]);
            }
            else if (Element.data("kendoEditor") != undefined) {
                Element.data("kendoEditor").value(NewValue[index]);
            }
            else if (Element.data("kendoSwitch") != undefined) {
                if (NewValue[index].toString().toLowerCase() == "true" || NewValue[index] == "1" || $("#" + FormInputName[index]).parent().find(".k-switch-label-on").text() == NewValue[index].toString()) {
                    $("#" + FormInputName[index]).parent().removeClass("k-switch-off");
                    $("#" + FormInputName[index]).parent().addClass("k-switch-on");
                    $("#" + FormInputName[index])[0].checked = true;
                }
                else {
                    $("#" + FormInputName[index]).parent().removeClass("k-switch-on");
                    $("#" + FormInputName[index]).parent().addClass("k-switch-off");
                    $("#" + FormInputName[index])[0].checked = false;
                }
            }
            else if (Element.data("kendoTextArea") != undefined) {
                Element.data("kendoTextArea").value(NewValue[index]);
            }
            else if (Element.data("kendoTextBox") != undefined) {
                Element.data("kendoTextBox").value(NewValue[index]);
            }
            else if ($("#_Image" + FormInputName[index])[0] != undefined) {
                if (NewValue[index] == "" && $(".Logo_HeaderLayout").length > 0) {
                    $("#_Image" + FormInputName[index])[0].src = $(".Logo_HeaderLayout")[0].src;
                    $("#" + FormInputName[index])[0].value = "";
                }
                else {
                    $("#_Image" + FormInputName[index])[0].src = NewValue[index];
                    $("#" + FormInputName[index])[0].value = NewValue[index];
                }

            }
            else if (Element.length > 0) {
                if (Element[0].className.indexOf("ImageUpload_") > -1) {
                    if (NewValue[index] == "" && $(".Logo_HeaderLayout").length > 0) {
                        $("#" + Element[0].className)[0].src = $(".Logo_HeaderLayout")[0].src;
                        $("." + Element[0].className).val("");
                    }
                    else {
                        $("." + Element[0].className).val(NewValue[index]);
                        $("#" + Element[0].className)[0].src = NewValue[index];
                    }
                }
                else
                    Element.val(NewValue[index]);
            }
            else
                Element.val(NewValue[index]);
        }
        //}
    }
}


function OnclickCloseEditorForm(e) {
    var WindowName = e.sender.element.context.id;
    var wnd = $("#" + WindowName).data("kendoWindow");

    if (LastWindowID.indexOf(WindowName.replace("PopupEditorWindow", "")) > -1)
        LastWindowID.pop();
    wnd.content("");
}

function MainGridOpenEntryForm(e) {
    var grid = $("#MainGrid").data("kendoGrid");
    var SelectItem = grid.dataItem(grid.select());
    if (SelectItem == null) {
        var popupNotification = $("#popupNotification").data("kendoNotification");
        popupNotification.show('رکوردی انتخاب نشده است', "info");
    }
    else {
        PopupMax('Desktop?Form=' + e.id + '&RowID=' + SelectItem.id, '');
    }
}

function MainGridCommand(e) {
    if (e.id != undefined) {
        var Arr = $("#" + e.id).attr("data_name").split('_');
        var StepID = 0;
        var Form = Arr[1];
        var ProcessID = Arr[2];
        var Title = "";
        var GridName;
        if ($.isNumeric(e.id)) {
            StepID = e.id;
            Form = e.parentElement.parentElement.id.replace("MainGridCommandButton_", "").replace("_optionlist", "");
            Title = e.textContent;
        }
        else if ($("#" + e.id + "_optionlist li").length > 0) {
            var StepArr = $("#" + e.id + "_optionlist li")[0].childNodes[0].id.split('_');
            StepID = StepArr[3];
            Form = StepArr[1];
            Title = $("#" + e.id + "_optionlist li")[0].childNodes[0].textContent;
        }
        GridName = "MainGrid" + Form;
        if (StepID > 0) {
            var grid = $("#" + GridName).data("kendoGrid");
            var selectedItem = grid.dataItem(grid.select());
            OpenProcessReferral(ProcessID, StepID, Title, selectedItem.id , false);
        }
    }
}

function DetailGridDisplayDetailProcess(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = e.id.indexOf("DetailDisplay") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailDisplay" : "Display";
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + (FormArraye[1] > 0 ? "_" + FormArraye[1] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var selectedItem = grid.dataItem(grid.select());
    if (selectedItem != null) {
        OpenProcessReferral(selectedItem.فرآیند, selectedItem.مرحله_فرآیند, "", selectedItem.id, true);
    }
    else {
        popupNotification.show("سطری انتخاب نشده است", "info");
    }
}

function OpenProcessReferral(ProcessID, StepID, Title, _RecordID, IsReadonly) {
    ShowLoader();
    $.ajax({
        url: "/Home/ProcessReferral",
        data: {
            ProcessID: ProcessID,
            StepID: StepID,
            _RecordID: _RecordID,
            IsReadonly: IsReadonly
        },
        type: "POST",
        success: function (Result) {
            if (Result.HasPermision) {
                if (Result.InformationFormID>0)
                    OpenEditorForm(Result.InformationFormID, Result.ParentID, _RecordID, false, Result.IsReadOnly, Result.Title, ProcessID, StepID);
                else
                    HideLoader();

            }
            else { 
                HideLoader();
                popupNotification.show("کاربر گرامی شما دسترسی لازم را ندارید", "info");
            }

        },
        error: function (result) {

        }
    })
}

function ShowProcessReferralNotification(e) {
    var arr = e.id.split('_');
    var ReferralID = arr[0],
        ProcessID = arr[1],
        ProcessStepID = arr[2],
        InformationFormID = arr[3],
        RecordID = arr[4],
        TableID = arr[5],
        ParentID = arr[6];


    if (ProcessID == 0) {
        if (InformationFormID > 0) { 
            if (window.location.href.indexOf("Desktop/Index?Form=" + InformationFormID) > -1)
                OpenEditorForm(InformationFormID, ParentID, RecordID, false, true);
            else {
                $(".main-section").load("/Home/InformationEntryForm", {
                    'Form': InformationFormID,
                    'ParentID': ParentID,
                    'IsDetailGrid': false
                }, function () { 
                    OpenEditorForm(InformationFormID, ParentID, RecordID, false, true);
                });
                window.history.pushState("", "", "/Desktop/Index?Form=" + InformationFormID + "&&ParentID=" + ParentID);
            }
        }
    }
    else if (InformationFormID > 0) {
            if (window.location.href.indexOf("Desktop/Index?ProcessID=" + ProcessID) > -1)
                OpenEditorForm(InformationFormID, ParentID, RecordID, false, true, "", ProcessID, ProcessStepID);
            else {
                $(".main-section").load("/Home/InformationEntryForm", {
                    ProcessID: ProcessID
                }, function () {
                    OpenEditorForm(InformationFormID, ParentID, RecordID, false, true, "", ProcessID, ProcessStepID);
                });
                window.history.pushState("", "", "/Desktop/Index?ProcessID=" + ProcessID);
            } 
        } 


    $.ajax({
        type: 'POST',
        url: "/Home/ViewReferral",
        data: {
            "ReferralID": ReferralID
        },
        dataType: 'json',
        success: function (data) {

        },
        error: function (result) {
            popupNotification.show(result.responseText, "error");
        }
    });

    $("#new-count span").text($("#new-count span").text() - 1);
    $('#' + e.id).remove();

    if ($("#new-count span").text() == "0") { 
        $("#UserNotificationBadge").removeClass("k-badge-error"); 
    }
}

function ShowDetailGrid(e) {
    var ButtonArr = e.id.split("_");
    var ParentForm = ButtonArr[1];
    var FormID = ButtonArr[2];
    var ParentID = ButtonArr[3];

    var wnd = $("#ShowSubGridWin" + ParentForm).data("kendoWindow");
    wnd.content("<div id='ShowDetailGridDiv' style='height:100%'></div>");
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });

    wnd.title("" + e.textContent);
    wnd.center();
    wnd.open();
    $("#ShowDetailGridDiv").load("/Desktop/Index?Form=" + FormID + "&ParentID=" + ParentID + "&IsProcess=false");
}

function DataGridFilterUILTRForeignKeyOpen(e) {
    if (!e.sender.popup.element.hasClass("FilterUILTRDropDownList")) {
        e.sender.popup.element.addClass("FilterUILTRDropDownList");
        e.sender.popup.element.find("input").on("input", function () {
            $(this).val($(this).val().replace(".", String.fromCharCode(8206)));
        });
    }
}

function OnSelectContextMenuGrid(e) {
    var item = e.item.id.split("_");

    switch (item[0]) {
        case "Copy":
            CopySelected(e.item.id.replace("Copy_", ""));
            break;
        case "CopyWithHeaders":
            CopySelectedWithHeaders(e.item.id.replace("CopyWithHeaders_", ""));
            break;
        case "Export":
            ExportSelected(e.item.id.replace("Export_", ""));
            break;
        case "ExportWithHeaders":
            ExportSelectedWithHeaders(e.item.id.replace("ExportWithHeaders_", ""));
            break;
        case "ExportToChart":
            ExportToChart();
            break;
        default:
            break;
    };
}

function CopySelected(GridName) {
    var grid = $("#" + GridName).data("kendoGrid");
    let selected = grid.select();

    if (selected.length === 0) {
        popupNotification.show('رکوردی انتخاب نشده است', "info");
        return;
    }

    grid.copySelectionToClipboard(false);
}

function CopySelectedWithHeaders(GridName) {
    var grid = $("#" + GridName).data("kendoGrid");
    let selected = grid.select();

    if (selected.length === 0) {
        popupNotification.show('رکوردی انتخاب نشده است', "info");
        return;
    }

    grid.copySelectionToClipboard(true);
}

function ExportSelected(GridName) {
    var grid = $("#" + GridName).data("kendoGrid");
    let selected = grid.select();

    if (selected.length === 0) {
        popupNotification.show('رکوردی انتخاب نشده است', "info");
        return;
    }
    grid.exportSelectedToExcel(false);
}

function ExportSelectedWithHeaders(GridName) {
    var grid = $("#" + GridName).data("kendoGrid");
    let selected = grid.select();

    if (selected.length === 0) {
        popupNotification.show('رکوردی انتخاب نشده است', "info");
        return;
    }

    grid.exportSelectedToExcel(true);
}

function ExportToChart(GridName) {
    var grid = $("#" + GridName).data("kendoGrid");
    var container = $('#chart-container');
    var windowInstance = $('#chart-container').data('kendoWindow');
    var currInstance = container.find('.k-chart').data('kendoChart');
    var data = grid.getSelectedData();

    if (!data.length) {
        kendo.alert('Please select cells before exporting.');
        return;
    }

    let unknownCountries = $.extend(true, [], data);

    unknownCountries.forEach(function (item, index, array) {
        if (!array[index].ShipCountry) {
            array[index].ShipCountry = "Unknown"
        }
    });

    if (windowInstance) {
        windowInstance.destroy();
    }

    if (currInstance) {
        currInstance.destroy();
    }

    let windowWidth = data.length > 5 ? data.length * 75 : 500;
    windowInstance = container.kendoWindow({ width: windowWidth }).data('kendoWindow');

    container.empty();
    var element = $('<div></div>').appendTo(container);
    windowInstance.open().center();
    element.kendoChart({
        dataSource: {
            data: unknownCountries
        },
        series: [{
            type: "column",
            field: 'Freight'
        }],
        categoryAxis: { field: "ShipCountry" }
    });
}

function OnKeyDownElement(e) {
    var FormID = 0;
    var Element = "";
    var ElementID = "";
    var GridName = "MainGrid";
    var DataKey = "";
    var ParentID = "";
    var SaveAndNewForm = false;

    if (e.sender != undefined) {
        ElementID = e.sender.element[0].id;
        FormID = e.sender.element[0].id.split('_');
        Element = e.sender.element[0].parentElement;
        var DataItem = $(e.sender.element[0]).attr("data-item");
        if (DataItem != undefined) {
            var DataArr = DataItem.split("_");
            var DataKey = DataArr[0];
            var ParentID = DataArr[1];
            GridName += DataKey;
        }
        if ($.inArray("SaveAndNewForm", e.sender.element[0].classList) > -1)
            SaveAndNewForm = true;

        if ($.inArray("TimeInput", e.sender.element[0].classList) > -1)
            CheckTime(e.sender.element[0]);
    }
    else if (e.target != undefined) {
        ElementID = e.target.id;
        FormID = e.target.id.split('_');
        Element = e.target.parentElement;
    }
    else if (e.element[0] != undefined) {
        ElementID = e.element[0].id;
        FormID = e.element[0].id.split('_');
        Element = e.element[0].parentElement;
        var DataItem = $(e.element[0]).attr("data-item");
        if (DataItem != undefined) {
            var DataArr = DataItem.split("_");
            var DataKey = DataArr[0];
            var ParentID = DataArr[1];
            GridName += DataKey;
        }
        if ($.inArray("TimeInput", e.element[0].classList) > -1)
            CheckTime(e.element[0]);
    }

    FormID = FormID[FormID.length - 1];

    while (Element.id.indexOf("EditorFormDiv" + FormID) == -1 && Element.id.indexOf("_active_cell") == -1 && Element.parentElement != null && Element.id.indexOf("ReportRightLayout") == -1 ) {
        Element = Element.parentElement;
    }
    Element.cellIndex;
    Element.rowSpan;
    if (Element.id.indexOf("_active_cell") == -1 && Element.id!="") {

        var FormArraye = Element.id.split('_');
        DataKey = FormID;
        ParentID = FormArraye[1];
        var RecordID = FormArraye[2];
        var FormDiv = Element.id == "ReportRightLayout" ? "ReportRightLayout": "EditorFormDiv" + DataKey + "_" + ParentID + "_" + RecordID;
       
        var FormInputName = [];
        var FormInputValue = [];
        GetInputValue(FormDiv + " .FormItemInput", FormInputName, FormInputValue)
        if (Element.id == "ReportRightLayout") {
            DataKey = 0;
            AutoCompleteDropDownList(FormDiv, ElementID, DataKey);
        }
        else {

            $.ajax({
                type: 'POST',
                url: "/Desktop/OnKeyDownElement",
                data: {
                    "DataKey": DataKey,
                    "ParentID": ParentID,
                    "RecordID": RecordID,
                    "FormInputName": FormInputName,
                    "FormInputValue": FormInputValue,
                    "ElementName": ElementID,
                },
                dataType: 'json',
                success: function (data) {
                    if (data != "") {
                        FillForm(data.data, FormInputName, FormInputValue, true, data.IsFixedItem.split(','), DataKey);
                        AutoCompleteDropDownList(FormDiv, ElementID, DataKey);

                        if ($("#" + Element.id + " .SearchWithOnkeyDown").length > 0 && RecordID == 0) {
                            FormInputName = [];
                            FormInputValue = [];
                            GetInputValue(Element.id + " .FormItemInput", FormInputName, FormInputValue);
                            var RequireMessage = CheckRequiredField(Element.id, DataKey);
                            if (RequireMessage == "") {
                                $("#" + Element.id + " .SearchWithOnkeyDown").each(function (index, Item) {
                                    var FormArr = Item.id.replace("Detail", "").replace("MainGrid","").split("_");
                                    $.ajax({
                                        type: 'POST',
                                        url: "/Desktop/SearchByFieldItem",
                                        data: {
                                            "DataKey": FormArr[0],
                                            "ParentID": 0,
                                            "SearchDataKey": $("#SearchWithOnkeyDownCoreId_" + FormArr[0]+"_0").val(),
                                            "SearchFieldItem": [],
                                            "SearchFieldOperator": [],
                                            "SearchFieldValue": [],
                                            "CleareGridAfterSearch": true,
                                            "EditorFormDataKey": DataKey,
                                            "FormInputName": FormInputName,
                                            "FormInputValue": FormInputValue,
                                            "GridJSON": []
                                        },
                                        dataType: 'json',
                                        success: function (data) {
                                            HideLoader();
                                            if (data.ErrorMessage != "")
                                                popupNotification.show(data.ErrorMessage, "error");
                                            else if (data.Query != "") {
                                                var grid = $("#" + Item.id).data("kendoGrid");
                                                grid.dataSource.read({ _Where: data.Query });
                                                grid.refresh();
                                                 
                                            }

                                            if (data.AlarmMessage != "")
                                                popupNotification.show(data.AlarmMessage, "info");
                                        },
                                        error: function (result) {
                                            HideLoader();
                                        }
                                    });
                                })
                            }
                        }

                        if (SaveAndNewForm == true) {
                            SaveFormEditor($("#" + FormDiv + " .SaveButton ").click()); 

                            setTimeout(function () {
                                HideLoader(); 
                                $("#" + FormDiv + " .NewButton  ").click();
                                FocusFirstItem();
                            }, 1000);
                        }
                    }
                    else
                        popupNotification.show("خطا در محاسبات", "error");

                },
                error: function (result) {
                    popupNotification.show(result.responseText, "error");
                }
            });
        }

    }

}

function AutoCompleteDropDownList(FormDiv, ElementID, DataKey) {
    $("#" + FormDiv + " .ActiveRelatedField").each(function (index, Item) {
        if (Item.id != "") {
            var RelatedFieldElement = $("#RelatedField" + Item.id).val();
            var RelatedFieldElementName = DataKey == 0 ? RelatedFieldElement : (RelatedFieldElement + "_" + DataKey);
            if (RelatedFieldElement != "" && ElementID == RelatedFieldElementName)
                if ($("#" + RelatedFieldElementName).length > 0) {
                    var RelatedFieldValue = $("#" + RelatedFieldElementName).val(); 
                    var dropdownlist = $("#" + Item.id).data("kendoDropDownList");
                    if (dropdownlist == undefined)
                        dropdownlist = $("#" + Item.id).getKendoMultiSelect();
                    if (RelatedFieldValue > 0) {
                        if (dropdownlist != undefined) {
                            IsReloaded_RTB = true;
                            dropdownlist.dataSource.read({ RelatedField: RelatedFieldValue });
                        }

                    }
                    else if (RelatedFieldValue != "") { 
                        IsReloaded_RTB = true;
                        dropdownlist.dataSource.read({ TextRelatedField: RelatedFieldValue });
                    }
                }
        }
    })
}

function ShowAttachmentForm(DataKey, ParentID, RecordID) {
    if ($.isNumeric(DataKey)) {
        $.ajax({
            url: "/Home/Attachment",
            data: {
                "DataKey": DataKey,
                "ParentID": ParentID,
                "RecordID": RecordID
            },
            type: "POST",
            success: function (Result) {

                var wnd = AttachmentWindow;
                wnd.content("<div id='AttachmentWindowDiv'></div>");
                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                wnd.setOptions({
                    width: newWidth - 50,
                    height: newHeight - 50
                });
                wnd.content(Result);
                wnd.center();
                wnd.open();

                $("#FileManager" + DataKey + "_" + ParentID + "_" + RecordID).css("height", "100%");
                $("#FileManager" + DataKey + "_" + ParentID + "_" + RecordID + " .k-filemanager-view").css("height", "100%");
            },
            error: function (result) {

            }
        })
    }
}

async function TableButtonClick(e) {
    var FormArraye = e.id.split('_');
    var FormTitle = $("#" + e.id + " .TableButtonText").text();
    var ISDetailGridForm = e.id.indexOf("DetailTableButton") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailTableButton" : "TableButton";
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + FormArraye[2] + (ISDetailGridForm ? "_" + FormArraye[3] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var SelectedRows = grid.select();
    var indexRow = -1;
    var ISReloadGrid = false;
    FormArraye[4] = FormArraye[4].replace("Detail", "");
    var CoreObjectID = FormArraye[1];
    var Actiontype = FormArraye[6];
    var RelatedForm = FormArraye[7];
    var ExecutionConditionQuery = FormArraye[8];
    var StartColumnIndex = 10; 
        if (FormArraye[9] > 0)
            SelectedRows = grid.tbody.find("tr[data-uid='" + grid.dataSource.get(FormArraye[9]).uid + "']");

    if (SelectedRows.length > 0 && Actiontype != "1") {

        ShowLoader();
        var ColumnName = "";
        for (let i = StartColumnIndex; i < FormArraye.length; i++) {
            ColumnName += FormArraye[i];
            if (i < FormArraye.length - 1)
                ColumnName += "_";
        }
        if (ColumnName == "")
            ColumnName = "id";

        await SelectedRows.each(function (index, row) {
            var selectedItem = grid.dataItem(row);

            switch (Actiontype) {
                case "2": { OpenEditorForm(RelatedForm, 0, selectedItem[ColumnName], false, true, FormTitle); break; }
                case "3": { ShowAttachmentForm(RelatedForm, 0, selectedItem[ColumnName]); break; }
                case "4": {
                    if (ExecutionConditionQuery == "1") {

                        $.ajax({
                            url: "/Desktop/TableButtonClick",
                            data: {
                                RowID: selectedItem.id,
                                ButtonID: CoreObjectID,
                                DataKey: FormArraye[2],
                                ParentID: FormArraye[3]
                            },
                            type: "POST",
                            success: function (Result) {
                                HideLoader();
                                if (Result.RunResult == false) {
                                    if (SelectedRows.length > 1) {
                                        grid.clearSelection();
                                        grid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");

                                        if (Result.ErrorMessage) {
                                            grid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(Result.ErrorMessage);
                                            grid.showColumn("_ShowError");
                                            grid.autoFitColumn("_ShowError");
                                        }

                                    }
                                    else if (SelectedRows.length == 1) {
                                        popupNotification.show(Result.ErrorMessage, "error");
                                    }
                                }
                                else {

                                    OpenEditorForm(RelatedForm, 0, selectedItem[ColumnName], false, false, FormTitle);
                                    if (SelectedRows.length == 1) {
                                        popupNotification.show("عملیات با موفقیت انجام شد", "success");
                                        indexRow = indexRow + 1;
                                    }
                                    else {
                                        grid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
                                        indexRow = indexRow + 1;
                                        grid.clearSelection();
                                    }

                                    if (Result.ISReloadGrid)
                                        ISReloadGrid = true;
                                }

                                if (indexRow == SelectedRows.length - 1) {
                                    if (Result.ISReloadGrid)
                                        grid.dataSource.read({ IsReload: true });
                                }

                            },
                            error: function (result) {
                                HideLoader();
                                if (Result.ISReloadGrid)
                                    grid.dataSource.read({ IsReload: true });
                            }
                        })
                    }
                    else { 
                        OpenEditorForm(RelatedForm, 0, selectedItem[ColumnName], false, false, FormTitle);
                    }
                    break;
                }
                case "5": { UploadPublickKeyDialog.open(); break; }
                case "6": { ProductUpdateFromTaxOrganizationDialog.open(); break; }
            }

            if (Actiontype != "4") {

                $.ajax({
                    url: "/Desktop/TableButtonClick",
                    data: {
                        RowID: selectedItem.id,
                        ButtonID: CoreObjectID,
                        DataKey: FormArraye[2],
                        ParentID: FormArraye[3]
                    },
                    type: "POST",
                    success: function (Result) {
                        HideLoader();
                        if (Result.RunResult == false) {
                            if (SelectedRows.length > 1) {
                                grid.clearSelection();
                                grid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");

                                if (Result.ErrorMessage) {
                                    grid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(Result.ErrorMessage);
                                    grid.showColumn("_ShowError");
                                    grid.autoFitColumn("_ShowError");
                                }

                            }
                            else if (SelectedRows.length == 1) {
                                popupNotification.show(Result.ErrorMessage, "error");
                            }
                        }
                        else {
                            if (SelectedRows.length == 1) {
                                popupNotification.show("عملیات با موفقیت انجام شد", "success");
                                indexRow = indexRow + 1;
                            }
                            else {
                                grid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
                                indexRow = indexRow + 1;
                                grid.clearSelection();
                            }

                            if (Result.ISReloadGrid)
                                ISReloadGrid = true;
                        }

                        if (indexRow == SelectedRows.length - 1) {
                            if (Result.ISReloadGrid)
                                grid.dataSource.read({ IsReload: true });
                        }

                    },
                    error: function (result) {
                        HideLoader();
                        if (Result.ISReloadGrid)
                            grid.dataSource.read({ IsReload: true });
                    }
                })
            }

        });

    }
    else {
        switch (Actiontype) {
            case "1": {
                if (RelatedForm == "0") {
                    CreateRowInGrid(GridName, ISDetailGridForm, FormArraye[2], FormArraye[3])
                }
                else
                    OpenEditorForm(RelatedForm, FormArraye[3], 0, false, false, FormTitle);  
                break; 
            }
            case "6": { ProductUpdateFromTaxOrganizationDialog.open(); break; }
            default: { popupNotification.show("سطری انتخاب نشده است", "info"); break; }
        }

        $.ajax({
            url: "/Desktop/TableButtonClick",
            data: {
                RowID: 0,
                ButtonID: CoreObjectID,
                DataKey: FormArraye[2],
                ParentID: FormArraye[3]
            },
            type: "POST",
            success: function (Result) {
                HideLoader();
                if (Result.ISReloadGrid)
                    grid.dataSource.read();

            },
            error: function (result) {
                HideLoader();
            }
        })


    }
}

var SearchGridName;
var SearchParentId;
var SearchGridDatakey;

function SearchFormButtonClick(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = e.id.indexOf("DetailSearchForm") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailSearchForm" : "SearchForm";
    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : FormArraye[0].replace(ReplaceWord, "MainGrid");
    var GridName = FormArraye[0] + FormArraye[2] + (ISDetailGridForm ? "_" + FormArraye[3] : "");
    SearchGridName = GridName;
    SearchParentId = FormArraye[3];
    SearchGridDatakey = FormArraye[2];
    var grid = $("#" + GridName).data("kendoGrid");
    ShowLoader();
    var ButtonID = FormArraye[1];
    var DataKey = FormArraye[2];
    var ParentID = FormArraye[3];
    var FormInputName = [];
    var FormInputValue = [];
    var RequireMessage = "";

    var EditorFormDivElement = e.sender.element[0];
    while (EditorFormDivElement.id.indexOf("EditorFormDiv") == -1) {
        EditorFormDivElement = EditorFormDivElement.parentElement;
        if (EditorFormDivElement == null)
            break;
    }


    if (EditorFormDivElement != null)
        if (EditorFormDivElement.id != undefined) {
            var EditorFormDivArr = EditorFormDivElement.id.replace("EditorFormDiv", "").split("_");
            var EditorFormDataKey = EditorFormDivArr[0];
            var EditorFormParentID = EditorFormDivArr[1];
            var EditorFormRecordeID = EditorFormDivArr[2];
            RequireMessage = CheckRequiredField(EditorFormDivElement.id, EditorFormDataKey);

            GetInputValue(EditorFormDivElement.id + " .FormItemInput", FormInputName, FormInputValue);
        }

    if (RequireMessage == "") {

        $.ajax({
            url: "/Home/SearchFormButtonClick",
            data: {
                "Form": ButtonID,
                "FormInputName": FormInputName,
                "FormInputValue": FormInputValue,
            },
            type: "POST",
            success: function (Result) {
                HideLoader();

                SearchFormWin.content(Result);
                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                SearchFormWin.setOptions({
                    width: newWidth - 50,
                    height: newHeight - 50
                });

                SearchFormWin.title("" + e.item.element[0].textContent);
                SearchFormWin.center();
                SearchFormWin.open();
            },
            error: function (result) {
                HideLoader();
            }
        })
    }
    else {
        HideLoader();
        popupNotification.show(RequireMessage.replace(/\n/g, '<br/>'), "error");
    }
}

function LoadElement(Item) {
    var Element;
    Element = $(Item).data("kendoDropDownList");
    if (Element == undefined)
        Element = $(Item).data("kendoSwitch");
    if (Element == undefined)
        Element = $(Item).data("kendoComboBox");

    Element.bind('change', function (e) {
        ElementChange(Item, false);
    });
}

function ElementChange(Item, IsFirstLoad) {
    if (Item.id != undefined && Item.id != "") {
        var IsShowHideElement = true;
        if ($("#InputDiv_" + Item.id).length != 0) {
            IsShowHideElement = $("#InputDiv_" + Item.id).css('display') == "none" ? false : true;
        }

        if (IsShowHideElement) {

            if (Item.className.indexOf("ClearAfterChange") > -1) {
                var DataKey = Item.id.split("_");
                DataKey = DataKey[DataKey.length - 1];
                if ($.isNumeric(DataKey)) {
                    var Element = Item.parentElement;
                    while (Element.id.indexOf("EditorFormDiv" + DataKey) == -1) {
                        Element = Element.parentElement;
                    }
                    var ID = $("#" + Element.id + " .IsFieldID input").length == 0 ? 0 : $("#" + Element.id + " .IsFieldID input").val().replaceAll(",", "");
                    if (ID == 0 && !IsFirstLoad)
                        ClearEditorForm(DataKey, Element.id, true, Item.id.replace("_" + DataKey, ""));
                }
            }
            var ShowHideElement = $("#ShowHideElement_" + Item.id).val().split(";");

            var ElementAccess = [];

            if (ShowHideElement.length > 0 && ShowHideElement != "/") {
                $.each(ShowHideElement, function (index, value) {
                    if (value != "") {
                        var Element = JSON.parse(value);
                        var ElementItem = $(Item).data("kendoDropDownList");
                        var ElementValue;
                        if (ElementItem == undefined) {
                            ElementItem = $(Item).data("kendoSwitch");
                            if (ElementItem == undefined) {
                                ElementItem = $(Item).data("kendoComboBox");
                                ElementValue = ElementItem.value();
                            }
                            //else if ($(".MainBlockDiv"))
                            //{

                            //}
                            else
                                ElementValue = ElementItem.check() ? 1 : 0;
                        }
                        else {
                            ElementValue = ElementItem.value();
                        }

                        if (ElementValue.toString().replaceAll(" ", "_") == Element.FieldValue && ElementAccess.indexOf(Element.SelectedItem) == -1) {
                            if (Element.ShowFieldItem == true) {

                                if (Element.SelectedItem.indexOf("TableButton_") > -1 && $("#InputDiv_" + Element.SelectedItem).length > 0) {
                                    $.each($("#InputDiv_" + Element.SelectedItem), function (index, value) {
                                        $(value.parentElement).css("display", "block")
                                    })
                                }
                                else {

                                    $("#InputDiv_" + Element.SelectedItem).css("display", "block")
                                    if (Element.SelectedItem.indexOf("Grid_") > -1) {
                                        if ($("#InputDiv_" + Element.SelectedItem).length > 0) {
                                            var DivChild = $("#InputDiv_" + Element.SelectedItem)[0].children[1] == undefined ? $("#InputDiv_" + Element.SelectedItem)[0] : $("#InputDiv_" + Element.SelectedItem)[0].children[1].children;
                                            for (let i = 0; i < DivChild.length; i++) {
                                                if (DivChild[i].id != "") {
                                                    if (DivChild[i].id.indexOf("MainGrid") > -1) {
                                                        var grid = $("#" + DivChild[i].id).data("kendoGrid");
                                                        if (grid != undefined) {
                                                            for (let i = 0; i < grid.columns.length; i++) {
                                                                if (!grid.columns[i].hidden) {
                                                                    grid.autoFitColumn(i);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (Element.SelectedItem.indexOf("SearchForm_") > -1) {
                                        $("#" + Element.SelectedItem).css("display", "block");
                                    }
                                }

                            }
                            else {
                                if (Element.SelectedItem.indexOf("SearchForm_") > -1)
                                    $("#" + Element.SelectedItem).css("display", "none")
                                else if (Element.SelectedItem.indexOf("TableButton_") > -1 && $("#InputDiv_" + Element.SelectedItem).length > 0) {
                                    $.each($("#InputDiv_" + Element.SelectedItem), function (index, value) {
                                        $(value.parentElement).css("display", "none")
                                    })
                                }
                                else
                                    $("#InputDiv_" + Element.SelectedItem).css("display", "none")

                            }

                            ElementAccess.push(Element.SelectedItem);
                        }
                        else {
                            if (Item.type == "checkbox") {
                                if (ElementAccess.indexOf(Element.SelectedItem) == -1)
                                    $("#InputDiv_" + Element.SelectedItem).css("display", "none")
                            }
                            //else {
                            //    $("#InputDiv_" + Element.SelectedItem).css("display", "block")
                            //}
                        }
                    }

                });
            }
        }
    }

}

async function SaveSelectedRow(e) {
    var Datakey = e.id.split('_')[1];
    await SaveSearchWin(Datakey, true);
}

async function SaveSearchWin(Datakey,IsClosed) { 
    var SelectedGrid = $("#MainGrid_" + Datakey).data("kendoGrid");

    var ColumnName = [];

    for (let index = 0; index < SelectedGrid.columns.length; index++) {
        if (SelectedGrid.columns[index].field != undefined)
            ColumnName.push(SelectedGrid.columns[index].field);
    }

    var ISDetailGridForm = SearchGridName.indexOf("DetailMainGrid") > -1 ? true : false;
    var grid = $("#" + SearchGridName).data("kendoGrid");
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
                'DataKey': SearchGridDatakey,
                'ParentID': SearchParentId,
                'RecordID': 0,
                'FormInputName': ColumnName,
                'FormInputValue': ValuesItem,
                'SearchDataKey': Datakey
            },
            type: "POST",
            success: function (result) {
                if (result.Message == "")
                {
                    //popupNotification.show('ذخیره سازی با موفقیت انجام شد', "success");
                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Valid-row");
                    indexRow = indexRow + 1;
                    document.getElementById("ResultRunExtanded").innerHTML = "ذخیره " + indexRow + " ردیف از " + SelectedRows.length + " ردیف "
                    SelectedGrid.clearSelection();
                    if (indexRow == SelectedRows.length && IsClosed) {
                        SearchFormWin.close();
                        datasource.read();
                    }
                    else if (!IsClosed) {
                        //var oldConfirm = window.confirm;
                        //window.confirm = function () { return true; };
                        //SelectedGrid.removeRow(row);
                        //window.confirm = oldConfirm;
                    }
                }
                else
                {
                    //popupNotification.show('ذخیره سازی با شکست مواجه شد', "error");
                    SelectedGrid.clearSelection();
                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "'] #error_" + selectedItem.id).html(result.Message);
                    SelectedGrid.table.find("tr[data-uid='" + selectedItem.uid + "']").addClass("Error-row");
                }
            },
            error: function (result) {
                popupNotification.show('ذخیره سازی با شکست مواجه شد', "error");
            }
        })
    });
}


function CloseSearchFormWin(e) {
    if (SearchGridName != undefined && SearchGridName != "") {
        var grid = $("#" + SearchGridName).data("kendoGrid");
        var datasource = grid.dataSource;
        datasource.read();
    }
}
function SearchMainGridAutoFitForce(e) {
    var grid = $("#" + e.id.replace("AutoFit", "")).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        //ConfigProgressBar(e.id.replace("AutoFit","ProgressBar_"), grid.columns.length, 0, i);
        if (!grid.columns[i].hidden) {
            grid.autoFitColumn(i);
        }
    }
}

function SearchInputKeyDown(e) {
    if (e.key == "Enter") {
        var Id = e.currentTarget.id;
        var ISDetailGridForm = Id.indexOf("DetailSearchInput") > -1 ? true : false;
        var ISSearchForm = Id.indexOf("SearchFormSearchInput") > -1 ? true : false;
        var ReplaceWord = ISDetailGridForm ? "DetailSearchInput" : (ISSearchForm ? "SearchFormSearchInput" : "SearchInput");
        var FormArraye = Id.split('_');

        FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : (ISSearchForm ? FormArraye[0].replace(ReplaceWord, "MainGrid_") : FormArraye[0].replace(ReplaceWord, "MainGrid"));
        var GridName = FormArraye[0] + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[2] : "");
        if ($("#" + GridName).data("kendoGrid") == undefined)
            GridName = FormArraye[0] + "_" + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[2] : "");
        GridFilterChange(GridName, Id);
    }
}

function SearchButton(e) {
    var Id = e.id;
    var ISDetailGridForm = Id.indexOf("DetailSearchButton") > -1 ? true : false;
    var ISSearchForm = Id.indexOf("SearchFormSearchInput") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailSearchButton" : (ISSearchForm ? "SearchFormSearchButton" : "SearchButton");
    var FormArraye = Id.split('_');

    FormArraye[0] = ISDetailGridForm ? FormArraye[0].replace(ReplaceWord, "DetailMainGrid") : (ISSearchForm ? FormArraye[0].replace(ReplaceWord, "MainGrid_") : FormArraye[0].replace(ReplaceWord, "MainGrid"));
    var GridName = FormArraye[0] + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[2] : "");
    if ($("#" + GridName).data("kendoGrid") == undefined)
        GridName = FormArraye[0] + "_" + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[2] : "");
    GridFilterChange(GridName, Id.replace("SearchButton", "SearchInput"));
}

function AdvancedSearchButtonClick(e) {
    var ISDetailGridForm = e.id.indexOf("DetailAdvancedSearch") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailAdvancedSearch" : "AdvancedSearch";
    var FormArraye = e.id.split('_');
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];
    var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + FormArraye[1] + (ISDetailGridForm ? "_" + FormArraye[1] : "");
    var FilterElement = $("#" + (ISDetailGridForm ? "Detail" : "") + "DivFilter_" + DataKey + "_" + ParentID);
    if (FilterElement.css('display') == 'none') {
        //FilterElement.show('slow');
        FilterElement.css("display", "block");
        $("#" + GridName).css("height", "70%");
    } else {
        FilterElement.hide('slow');
        FilterElement.css("display", "none");
        $("#" + GridName).css("height", "100%");
    }

}

function GridFilterChange(GridName, SearchInput) {
    var grid = $("#" + GridName).data("kendoGrid"),
        filter = { logic: "and", filters: [] };

    var keyword = $('#' + SearchInput).val();
    if (keyword.length >= 4) {
        if ($.isNumeric(keyword.substring(0, 2)) && !$.isNumeric(keyword.substring(2, 3)) && $.isNumeric(keyword.substring(3, 4))) {
            if (keyword.length == 4) {
                keyword = keyword.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(3, 4);
            }
            else if (keyword.length == 5 && $.isNumeric(keyword.substring(3, 5))) {
                keyword = keyword.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(3, 5);
            }
            else if (keyword.length == 6 && $.isNumeric(keyword.substring(3, 6))) {
                keyword = keyword.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ';
            }
            else if (keyword.length == 7 && $.isNumeric(keyword.substring(3, 7))) {
                keyword = keyword.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ' + keyword.substring(6, 7);
            }
            else if (keyword.length == 8 && $.isNumeric(keyword.substring(3, 8))) {
                keyword = keyword.substring(0, 2) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(2, 3) + ' ' + String.fromCharCode(8206) + ' ' + keyword.substring(3, 6) + ' ' + String.fromCharCode(8206) + ' ایران ' + String.fromCharCode(8206) + ' ' + keyword.substring(6, 8);
            }
        }
    }

    if (keyword != "") {
        var columns = grid.columns;
        var keywordfilter = { logic: 'or', filters: [] };

        columns.forEach(function (x) {
            if (x.field && x.field != "_ShowError" && x.field != "_ShowWarning") {
                var type = grid.dataSource.options.schema.model.fields[x.field].type;
                var editable = grid.dataSource.options.schema.model.fields[x.field].editable;
                if (x.field != grid.dataSource.options.schema.model.id && (editable == false && type == 'string')) {
                    keywordfilter.filters.push({
                        field: x.field,
                        operator: 'contains',
                        value: keyword
                    })
                }
                else if (type == 'string') {
                    keywordfilter.filters.push({
                        field: x.field,
                        operator: 'contains',
                        value: keyword
                    })
                }
                //else if (x.field == 'خودرو' && type == 'number' && (typeof x.values != 'undefined')) {
                //    var word = keyword.substring(0, 2) + String.fromCharCode(28) + keyword.substring(2, 3) + String.fromCharCode(28) + keyword.substring(3, 6) + String.fromCharCode(28) + keyword.substring(6, 8);

                //    x.values.forEach(function (value) {
                //        if (value.text != "") {
                //            if (value.text.indexOf(word) >= 0) {
                //                keywordfilter.filters.push({
                //                    field: x.field,
                //                    operator: 'eq',
                //                    value: value.value
                //                });
                //            }
                //        }
                //    });

                //}
                else if (type == 'number' && (typeof x.values != 'undefined')) {

                    x.values.forEach(function (value) {
                        if (value.text != "") {
                            if (value.text.indexOf(keyword) >= 0) {
                                keywordfilter.filters.push({
                                    field: x.field,
                                    operator: 'eq',
                                    value: value.value
                                });
                            }
                        }
                    });

                } else if (type == 'number') {
                    if (isNumeric(keyword)) {
                        keywordfilter.filters.push({
                            field: x.field,
                            operator: 'eq',
                            value: keyword
                        });
                    }
                }

            }
        });


        filter.filters.push(keywordfilter);
    }

    grid.dataSource.filter(filter);
}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function MainGridOrgChartShow(e) {
    var ItemArr = e.id.split("_");
    $("#OrgChartDiv_" + ItemArr[1] + "_" + ItemArr[2]).css("display", "block");
    $("#MainGrid" + ItemArr[1]).css("display", "none");
}

function DetailMainTreeClickItem(e) {
    var ItemArr = e.id.split('_');
    ShowLoader();


    $("#TGridDiv" + ItemArr[1] + "_" + ItemArr[3]).load("/Home/InformationEntryForm", {
        'Form': ItemArr[2],
        'ParentID': ItemArr[3],
        'IsDetailGrid': true
    }, function () {
        HideLoader();
        $("#TTreeDiv" + ItemArr[1] + "_" + ItemArr[3] + " .DetailTitle").removeClass("SelectedElement");
        $("#" + e.id + " .DetailTitle").addClass("SelectedElement");
    });
}

function ProcessStepDetailMainTreeClickItem(e) {
    var ItemArr = e.id.split('_');
    ShowLoader();


    $("#TGridDiv" + ItemArr[1] + "_" + ItemArr[3]).load("/Home/InformationEntryForm", {
        'Form': ItemArr[1],
        'ProcessID': ItemArr[2],
        'ParentID': ItemArr[3],
        'IsDetailGrid': true
    }, function () {
        HideLoader();
        $("#TTreeDiv" + ItemArr[1] + "_" + ItemArr[3] + " .DetailTitle").removeClass("SelectedElement");
        $("#" + e.id + " .DetailTitle").addClass("SelectedElement");
    });
}

function ProcessStepReload(e) {  
    var grid = $("#" + e.id.replace("ReloadButtonName", "ProcessStepGrid")).data("kendoGrid");
    grid.dataSource.read();
}

function ChangeFilter(e) {
    var a = e;
}

function CloseFilterRow(e) {
    e.parentElement.parentElement.parentElement.parentElement.remove();
}

function FilterAddExpression(e) {
    var ElementArr = e.id.split('_');
    $("#DivFilter_" + ElementArr[1] + "_" + ElementArr[2]).append($("#FilterItemTemp_" + ElementArr[1] + "_" + ElementArr[2])[0].innerHTML);
}

function FilterChange(e) {
    var Element = e.sender.element[0].id.split('_');
    var ParentID = document.activeElement.parentElement.parentElement == null ? -1 : document.activeElement.parentElement.parentElement.id;
    if (document.activeElement.parentElement.classList.value.indexOf("k-filter-field") > -1) {

        $.ajax({
            type: 'POST',
            url: "/Desktop/GetElementFilter",
            data: {
                "FieldName": document.activeElement.parentElement.innerText,
                "DataKey": Element[1],
                "ParentID": Element[2]
            },
            dataType: 'json',
            success: function (Result) {

                $("#" + ParentID + " .k-filter-toolbar-item ")[2].innerHTML = "";
                //$("#" + ParentID + " .k-filter-toolbar-item ")[2].append(Result);
                $("#" + ParentID + " .k-filter-toolbar-item ")[2].innerHTML = Result;
                popupNotification.show(Result, "info");

            },
            error: function (result) {
                popupNotification.show(result.responseText, "error");
            }
        });
    }
}

function SaveGridFilterButton(e) {
    var startSearch = true;
    if (e.id != undefined)
        if (e.id.indexOf("GridDataRow") > -1)
            startSearch = false;

    if (startSearch) {
        ShowLoader();
        var ISDetailGridForm = e.id == undefined ? (e.sender.element[0].id.indexOf("DetailSaveFilterButton") > -1 ? true : false) : (e.id.indexOf("DetailDataRowCount") > -1 ? true : false);

        var ReplaceWord = e.id == undefined ? (ISDetailGridForm ? "DetailSaveFilterButton" : "SaveFilterButton") : (ISDetailGridForm ? "DetailDataRowCount" : "DataRowCount");
        var FormArraye = e.id == undefined ? e.sender.element[0].id.split('_') : e.id.split('_');
        var DataKey = FormArraye[1];
        var ParentID = FormArraye[2];
        var Count = e.id == undefined ? $((ISDetailGridForm ? "#DetailDataRowCount" : "#DataRowCount") + '_' + DataKey + '_' + ParentID).length > 0 ? $((ISDetailGridForm ? "#DetailDataRowCount" : "#DataRowCount") + '_' + DataKey + '_' + ParentID)[0].innerText : 100000 : 1000;
        if (e.id != undefined) {
            var TempArr = FormArraye;
            Count = FormArraye[FormArraye.length - 1];
            TempArr[TempArr.length - 1] = "";
            var ID = TempArr.join("_")
            $("#" + ID.substring(0, ID.length - 1))[0].innerText = Count;
        }


        var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + DataKey + (ISDetailGridForm ? "_" + ParentID : "");
        var Query = "";
        var FilterField = $("#" + (ISDetailGridForm ? "Detail" : "") + "Filter_" + DataKey + "_" + ParentID + " .k-filter-field");
        var FilterOperator = $("#" + (ISDetailGridForm ? "Detail" : "") + "Filter_" + DataKey + "_" + ParentID + " .k-filter-operator ");
        var FilterValue = $("#" + (ISDetailGridForm ? "Detail" : "") + "Filter_" + DataKey + "_" + ParentID + " .k-filter-value input");
        var FilterPreview = $("#" + (ISDetailGridForm ? "Detail" : "") + "Filter_" + DataKey + "_" + ParentID + " .k-filter-preview")[0].innerText;
        var indexValue = 0;

        var FilterFieldArr = [];
        var FilterOperatorArr = [];
        var FilterValueArr = [];
        var FilterMiddleOperatorArr = [];

        $("#" + (ISDetailGridForm ? "Detail" : "") + "DivFilter_" + DataKey + "_" + ParentID).css("display", "block");

        for (var index = 0; index < FilterField.length; index++) {
            var SubQuery = "";
            SubQuery += FilterField[index].innerText.replaceAll(' ', '_') + ' ' + FilterOperator[index].innerText + ' ' + "''";
            FilterPreview = FilterPreview.substring(SubQuery.length + 2, FilterPreview.length);

            FilterFieldArr.push(FilterField[index].innerText.replaceAll(' ', '_'));
            FilterOperatorArr.push(FilterOperator[index].innerText);

            if (FilterValue[indexValue].type == "text") {
                FilterValueArr.push(FilterValue[indexValue].value);
                indexValue = indexValue + 1;
            }
            else if (FilterValue[indexValue].type == "hidden") {
                if (FilterValue[indexValue + 1].type == "checkbox") {
                    FilterValueArr.push(FilterValue[indexValue + 2].value == "true" ? '1' : '0');
                    indexValue = indexValue + 3;
                }
                else if (FilterValue[indexValue].id.indexOf("DropDownListFiltered") > -1) {
                    if (FilterValue[indexValue + 2] == undefined) {
                        FilterValueArr.push(FilterValue[indexValue + 1].value);
                        indexValue = indexValue + 2;
                    }
                    else {
                        if (FilterValue[indexValue + 2].id == FilterValue[indexValue].id.replace("DropDownListFiltered_", "Filtered_")) {
                            FilterValueArr.push(FilterValue[indexValue + 2].value);
                            indexValue = indexValue + 3;
                        }
                        else {
                            FilterValueArr.push(FilterValue[indexValue + 1].value);
                            indexValue = indexValue + 2;
                        }
                    }
                }
            }
            else if (FilterValue[indexValue].type == "checkbox") { 
                FilterValueArr.push(FilterValue[indexValue+1].value=="true"?1:0);
                indexValue = indexValue + 2;
            }

            if (FilterPreview.indexOf('AND') > -1 && FilterPreview.indexOf('AND') < 2) {
                FilterMiddleOperatorArr.push("AND");
                FilterPreview = FilterPreview.substring(FilterPreview.indexOf('AND') + 3, FilterPreview.length);
            }
            if (FilterPreview.indexOf('OR') > -1 && FilterPreview.indexOf('OR') < 2) {
                FilterMiddleOperatorArr.push("OR");
                FilterPreview = FilterPreview.substring(FilterPreview.indexOf('OR') + 3, FilterPreview.length);
            }
        }

        $("#" + (ISDetailGridForm ? "Detail" : "") + "DivFilter_" + DataKey + "_" + ParentID).css("display", "none");
        $("#" + GridName).css("height", "100%");

        var grid = $("#" + GridName).data("kendoGrid");
        if (grid == undefined) {
            GridName = "MainGrid_" + FormArraye[1];
            grid = $("#" + GridName).data("kendoGrid");
        }

        var optionalData;
        if (FilterFieldArr.length == 0)
            optionalData = { IsReload: true };
        else
            optionalData = { FilterField: FilterFieldArr.join(',').toString(), FilterOperator: FilterOperatorArr.join(',').toString(), FilterValue: FilterValueArr.join(',').toString(), FilterMiddleOperator: FilterMiddleOperatorArr.join(',').toString(), IsAdvancedSearch: true, _ShowRowCount: Count };

        grid.dataSource.read(optionalData);
        grid.refresh();
    }
}

function AttachmentFormEditor(e) {
    var DataKey = e.sender.element[0].id.replace("AttachmentEditorFormButton", "").split('_')[0];
    var ParentID = e.sender.element[0].id.replace("AttachmentEditorFormButton", "").split('_')[1];
    var FormName = e.sender.element[0].id.replace("AttachmentEditorFormButton", "EditorFormDiv");

    var ID = $("#" + FormName + " .IsFieldID input").val().replaceAll(",", "");
    if (ID == 0)
        popupNotification.show("اطلاعات هنوز ذخیره نشده است", "error");
    else
        ShowAttachmentForm(DataKey, ParentID, ID);
}

async function SearchFieldClick(e) {
    if (e.sender.element[0].id.indexOf("SearchFieldButton_")>-1)
    await FillGridBySearchField(e.sender.element[0].id);
}

async function FillGridBySearchField(ElementId) {
    ShowLoader();
    var IsDetail = ElementId.indexOf("Detail") > -1 ? true : false;
    var FormArr = ElementId.split('_');
    var IsClearGrid = $("#" + ElementId.replace("SearchFieldButton_", "SwitchFieldButton_"))[0].checked;  
    var ElementDiv = ElementId.replace("SearchFieldButton_", "SearchFieldcontainer_");

    if (ElementDiv.indexOf("SearchField") > -1) {
        var Element = $("#" + ElementId)[0];
        while (Element.id.indexOf("SearchFieldcontainer_") == -1) {
            Element = Element.parentElement;
        }
        ElementDiv = Element.id;
        FormArr = ElementDiv.split('_');
    }


    var DataKey = FormArr[1];
    var ParentID = FormArr[2];
    var ProcessID = FormArr[3];
    var ProcessStepID = FormArr[4];
    var SearchFieldItem = [];
    var SearchFieldOperator = [];
    var SearchFieldValue = []; 
    var FormInputName = [];
    var FormInputValue = []; 
    var RequireMessage = "";
    var EditorFormDataKey = "0";
    var EditorFormParentID = "0";
    var EditorFormRecordeID = "0";

    var GridName = (IsDetail ? "DetailMainGrid" : "MainGrid") + DataKey + (IsDetail ? "_" + ParentID : "");
    var grid = $("#" + GridName).data("kendoGrid");
    var GridJSON = (!IsClearGrid && grid.options.editable.mode == "incell" && grid.dataSource.total() > 0)? JSON.stringify(grid.dataSource.view()):"[]";


    if ($("#" + ElementDiv).length > 0) {
        var EditorFormDivElement = $("#" + ElementDiv)[0];
        while (EditorFormDivElement.id.indexOf("EditorFormDiv") == -1) {
            EditorFormDivElement = EditorFormDivElement.parentElement;
            if (EditorFormDivElement == null)
                break;
        }

        if (EditorFormDivElement != null)
            if (EditorFormDivElement.id != undefined) {
                var EditorFormDivArr = EditorFormDivElement.id.replace("EditorFormDiv","").split("_");
                EditorFormDataKey = EditorFormDivArr[0];
                EditorFormParentID = EditorFormDivArr[1];
                EditorFormRecordeID = EditorFormDivArr[2];
                RequireMessage = CheckRequiredField(EditorFormDivElement.id, EditorFormDataKey);

                GetInputValue(EditorFormDivElement.id + " .FormItemInput", FormInputName, FormInputValue);
            }
    }
     

    $("#" + ElementDiv + " .SearchFieldItem:visible").each(function (index, Item) {
        if (Item.children[0].id != "" && Item.children[0].id.indexOf("SearchFieldButton") == -1 && Item.children[0].id.indexOf("SwitchFieldButton") == -1) {
            SearchFieldItem.push(Item.children[0].id.replace(DataKey + "_" + ParentID + "_" + ProcessID + "_" + ProcessStepID, ""));
            SearchFieldOperator.push($(Item.children[1].firstElementChild.firstElementChild).val()); 
            SearchFieldValue.push(
                $("#" + ElementDiv + " #" + Item.children[1].firstElementChild.firstElementChild.name.replace("_input", "").replace("SearchFieldOperator_", "SearchField_")).attr('type') == 'checkbox' ?
                    $("#" + ElementDiv + " #" + Item.children[1].firstElementChild.firstElementChild.name.replace("_input", "").replace("SearchFieldOperator_", "SearchField_")).prop("checked") :
                $("#" + ElementDiv + " #" + Item.children[1].firstElementChild.firstElementChild.name.replace("_input", "").replace("SearchFieldOperator_", "SearchField_")).val()
            );
        }
    });

    if (RequireMessage != "") {
        HideLoader();
        popupNotification.show(RequireMessage.replace(/\n/g, '<br/>'), "error");
    }
    else { 
        $.ajax({
            type: 'POST',
            url: "/Desktop/SearchByFieldItem",
            data: {
                "DataKey": DataKey,
                "ParentID": ParentID,
                "SearchDataKey": FormArr[FormArr.length - 1],
                "SearchFieldItem": SearchFieldItem,
                "SearchFieldOperator": SearchFieldOperator,
                "SearchFieldValue": SearchFieldValue,
                "CleareGridAfterSearch": IsClearGrid,
                "EditorFormDataKey": EditorFormDataKey,
                "FormInputName": FormInputName,
                "FormInputValue": FormInputValue,
                "GridJSON": GridJSON,
                "GridMode": (IsDetail ? "Detail" : ""),
                "ProcessID": ProcessID,
                "ProcessStepID": ProcessStepID
            },
            dataType: 'json',
            success: function (data) {
                HideLoader();
                if (data.ErrorMessage != "")
                    popupNotification.show(data.ErrorMessage, "error");
                else if (data.Query != "") {
                    if (data.Query == "1")
                        data.Query = "";
                    grid.dataSource.read({ _Where: data.Query });
                    grid.refresh();

                    $("#" + ElementDiv + " .SearchFieldItem:visible").each(function (index, Item) {
                        if (Item.children[0].id != "" && Item.children[0].id.indexOf("SearchFieldButton") == -1 && Item.children[0].id.indexOf("SwitchFieldButton") == -1) {
                            if ($("#" + ElementDiv + " #" + Item.children[1].firstElementChild.firstElementChild.name.replace("_input", "").replace("SearchFieldOperator_", "SearchField_")).hasClass("SearchFieldKeydown") == true)
                                $("#" + ElementDiv + " #" + Item.children[1].firstElementChild.firstElementChild.name.replace("_input", "").replace("SearchFieldOperator_", "SearchField_")).val("");
                        }
                    });  
                }

                if (data.AlarmMessage!="")
                    popupNotification.show(data.AlarmMessage, "info");
            },
            error: function (result) {
                HideLoader();
            }
        });
    }

     
}

function OnChangeGrid(e) {
    IncellGridName = e.sender.element[0].id;
}


function CloseGridCellColumnPicTempWin(e) {
    var WinName = e.sender.element[0].id;
    $("#" + RowPicID + " img")[0].src = $(".UploadFile_imgG-C-C-C-B")[0].src;
}
