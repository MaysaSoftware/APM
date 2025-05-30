var UrlPath;
var FileName;
var FileExtension;
var FileManagerElementID;

function ConfigFileManager(ManagerName) {
    var id = ManagerName.length > 0 ? ManagerName[0].id : ManagerName.id
    var filemanager = $("#" + id).getKendoFileManager();

    filemanager.executeCommand({ command: "TogglePaneCommand", options: { type: "preview" } });
    if (filemanager.toolbar.fileManagerDetailsToggle != undefined)
        filemanager.toolbar.fileManagerDetailsToggle.switchInstance.toggle();

}


function onOpenfilemanager(e) {
    if (e.entry.extension.toLowerCase() == ".png" || e.entry.extension.toLowerCase() == ".jpg" || e.entry.extension.toLowerCase() == ".jpeg") {
        UrlPath = e.entry.path;
        FileName = e.entry.name;
        FileExtension = e.entry.extension

        $.ajax({
            url: "/Attachment/GetFileByte",
            data: { '_Path': e.entry.path },
            type: "POST",
            success: function (result) {
                if (result.length > 0) {
                    var imageEditor = $("#ImagePreview").getKendoImageEditor();
                    imageEditor.drawImage('data:image/jpeg;base64,' + result).done(function (image) {
                        imageEditor.drawCanvas(image);
                        imageEditor._initUndoRedoStack();
                        imageEditor._toggleTools();
                    }).fail(function (args) {
                        alert("خطا در ایجاد عکس!");
                    });

                    var wnd = $("#ImagePreviewWindow").data("kendoWindow");
                    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                    wnd.setOptions({
                        width: newWidth - 50,
                        height: newHeight - 50
                    });

                    wnd.title(e.entry.name).center().open();
                }
            },
            error: function (result) {

            }
        })
    }
    else if (e.entry.extension == ".txt") {
        var wnd = $("#TextPreviewWindow").data("kendoWindow");
        var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        wnd.setOptions({
            width: newWidth - 50,
            height: newHeight - 50
        });

        wnd.title(e.entry.name).center().open(); 
    }
    //else if (e.entry.extension == ".pdf") {

    //    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;
    //    $("#pdfViewer").kendoPDFViewer({
    //        pdfjsProcessing: {
    //            file: e.entry.path
    //        },
    //        width: "100%",
    //        height: newHeight
    //    });

    //    $("#PdfViewerWindow").data("kendoWindow").title(e.entry.name).center().open();
    //}
}


function onSelectfilemanager(e) {

    if (e.entries.length == 1) {
        if (e.entries[0].extension.toLowerCase() == ".png" || e.entries[0].extension.toLowerCase() == ".jpg" || e.entries[0].extension.toLowerCase() == ".jpeg" || e.entries[0].extension.toLowerCase() == ".pdf") {
            var Path = e.entries[0].path;
            $.ajax({
                url: "/Attachment/GetFileByte",
                data: { '_Path': Path },
                type: "POST",
                success: function (result) {
                    FileManagerElementID = e.sender.element[0].id;
                    if (result.length > 0) {
                        if (result.indexOf("<!DOCTYPE html>") == -1) {
                            $("#" + e.sender.element[0].id + " .k-filemanager-preview .k-file-info .k-file-preview").empty();
                            $("#" + e.sender.element[0].id + " .k-filemanager-preview .k-file-info .k-file-preview").append('<img id="DetailImagePreview" src="data:image/jpeg;base64,' + result + '" alt="نمایش عکس" width="96" height="96" /> ');
                        }
                    }
                },
                error: function (result) {

                }
            })
        }
        else if (e.entries[0].extension.toLowerCase() == ".txt"){
            var Path = e.entries[0].path;
            $.ajax({
                url: "/Attachment/GetTextFile",
                data: { '_Path': Path },
                type: "POST",
                success: function (result) {
                    FileManagerElementID = e.sender.element[0].id;
                    if (result.length > 0) {
                        if (result.indexOf("<!DOCTYPE html>") == -1) {
                            $("#" + e.sender.element[0].id + " .k-filemanager-preview .k-file-info .k-file-preview").empty();
                            $("#" + e.sender.element[0].id + " .k-filemanager-preview .k-file-info .k-file-preview").append('<textarea id="text_' + e.sender.element[0].id + '" rows="10" style="width:96;max-height:96;font-size:9px;">' + result + '</textarea>');
                            $("#TextPreview").data("kendoTextArea").value(result);
                        }
                    }
                },
                error: function (result) {

                }
            })
        }
    }
}


function DownloadCommand(e) {
    var filemanagerNS = kendo.ui.filemanager;

    filemanagerNS.commands.DownloadCommand = filemanagerNS.FileManagerCommand.extend({
        exec: function () {
            var that = this,
                filemanager = that.filemanager,
                options = that.options,
                target = options.target
            selectedFiles = filemanager.getSelected();
            var Counter;
            var TempArraye = [];
            for (Counter = 0; Counter < selectedFiles.length; Counter++) {
                TempArraye.push(selectedFiles[Counter].path);
            }
            window.location = '/Attachment/Download?_FilePath=' + TempArraye;

        }
    });

    filemanagerNS.commands.UpdateImageCommand = filemanagerNS.FileManagerCommand.extend({
        exec: function () {
            var that = this,
                filemanager = that.filemanager,
                options = that.options,
                target = options.target
            selectedFiles = filemanager.getSelected();
            if (selectedFiles.length > 0) {

                var Entry = selectedFiles[0];
                if (Entry.extension.toLowerCase() == ".png" || Entry.extension.toLowerCase() == ".jpg" || Entry.extension.toLowerCase() == ".jpeg") {
                    $.ajax({
                        url: "/Attachment/GetFileByte",
                        data: { '_Path': Entry.path },
                        type: "POST",
                        success: function (result) {
                            if (result.length > 0) {
                                UrlPath = Entry.path;
                                FileName = Entry.name;
                                FileExtension = Entry.extension;

                                Entry.name
                                Entry.extension
                                var imageEditor = $("#ImageEditor").getKendoImageEditor();
                                imageEditor.drawImage('data:image/jpeg;base64,' + result).done(function (image) {
                                    imageEditor.drawCanvas(image);
                                    imageEditor._initUndoRedoStack();
                                    imageEditor._toggleTools();
                                }).fail(function (args) {
                                    alert("خطا در ایجاد عکس!");
                                });

                                var wnd = $("#ImageEditorWindow").data("kendoWindow");
                                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                                wnd.setOptions({
                                    width: newWidth - 50,
                                    height: newHeight - 50
                                });

                                wnd.title(Entry.name).center().open();

                            }
                        },
                        error: function (result) {

                        }
                    })
                }

            }

        }
    });
}


function ScanCommand(e) {

}

function SaveImageEditor(e) {
    var imageEditor = $("#ImageEditor").getKendoImageEditor();
    var canvas = imageEditor.getCanvasElement(),
        ctx = imageEditor.getCurrent2dContext(),
        image = imageEditor.getCurrentImage();

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(image, 0, 0, canvas.width, canvas.height);
    var ImageData = canvas.toDataURL();

    $.ajax({
        url: "/Attachment/SaveImageEditor",
        data: {
            'ImageData': ImageData,
            'Url': UrlPath,
            'Name': FileName,
            'Extension': FileExtension
        },
        type: "POST",
        success: function (result) {
            var wnd = $("#ImageEditorWindow").data("kendoWindow");
            wnd.close();

            $("#" + FileManagerElementID + " .k-filemanager-preview .k-file-info .k-file-preview").empty();
            $("#" + FileManagerElementID + " .k-filemanager-preview .k-file-info .k-file-preview").append('<img id="DetailImagePreview" src="' + ImageData + '" alt="نمایش عکس" width="96" height="96" /> ');
        },
        error: function (result) {

        }
    })

}

function GetFileByte() {
    $.ajax({
        url: "/Attachment/GetFileByte",
        data: { '_Path': e.entry.path },
        type: "POST",
        success: function (result) {
            if (result.length > 0) {
                $("#imagePreview").attr('src', 'data:image/jpeg;base64,' + result)
                $("#imagePreview").attr('width', '100%')
                $("#imagePreview").attr('height', '100%')
                $("#FilePreviewWindow").data("kendoWindow").title(e.entry.name).center().open();
            }
        },
        error: function (result) {

        }
    })
}


function UploadFile(e) {
    var ID = e.sender.element[0].id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], InnerID = FileArr[4], FileCoreObjectID = FileArr[5];
    var ElementName = ID.replace("FileUpload", "");

    $.ajax({
        url: "/Attachment/GetFileByteWithName",
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
                if ($("#FileManager" + DataKey + "_" + ParentID + "_" + InnerID).length > 0) {
                    $("#FileManager" + DataKey + "_" + ParentID + "_" + InnerID).data('kendoFileManager').dataSource.read();
                }
            }
        },
        error: function (result) {

        }
    })
}


function StartUploadFile(e) {
    var ID = e.sender.element[0].id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], FileCoreObjectID = FileArr[5];

    var InnerID = $("#EditorForm" + DataKey + "_" + ParentID + " .IsFieldID input").length == 0 ? (FileArr[4] > 0 ? FileArr[4] : 0) : $("#EditorForm" + DataKey + "_" + ParentID + " .IsFieldID input").val().replaceAll(",", "");
    var FileManagerName = "FileUpload_" + DataKey + "_" + ParentID + "_" + RecordID + "_0_" + FileCoreObjectID;
    if ($("#" + FileManagerName).length > 0) {
        var fileManager = $("#" + FileManagerName).data("kendoUpload");
        fileManager.options.async.saveUrl = fileManager.options.async.saveUrl.replace("_InnerID=0", "_InnerID=" + InnerID);
    }
}

function CompleteUploadFile(e) {
    var ID = e.sender.element[0].id;
    var FileArr = ID.split("_");
    var DataKey = FileArr[1], ParentID = FileArr[2], RecordID = FileArr[3], FileCoreObjectID = FileArr[5];
    var InnerID = $("#EditorForm" + DataKey + "_" + ParentID + " .IsFieldID input").length == 0 ? (FileArr[4] > 0 ? FileArr[4] : 0) : $("#EditorForm" + DataKey + "_" + ParentID + " .IsFieldID input").val().replaceAll(",", "");
    var ElementName = ID.replace("FileUpload", "");

    $.ajax({
        url: "/Attachment/GetFileByteWithName",
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
                if ($("#FileManager" + DataKey + "_" + ParentID + "_" + InnerID).length > 0) {
                    $("#FileManager" + DataKey + "_" + ParentID + "_" + InnerID).data('kendoFileManager').dataSource.read();
                }
            }
        },
        error: function (result) {

        }
    })
}


function ScanButton_FileManager(e) {
    var ID = e.id;


    var Element = e.target[0].parentElement;
    while (Element.id.indexOf("FileManager") == -1) {
        Element = Element.parentElement;
    }

    var FileArr = Element.id.replace("FileManager", "").split("_");
    var DataKey = FileArr[0], ParentID = FileArr[1], InnerID = FileArr[2];
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

                $.ajax({
                    url: "/Attachment/SaveScanerFile",
                    data: {
                        'ImageData': e.target.result,
                        "_InnerID": InnerID,
                        "_DataKey": DataKey,
                        "_ParentID": ParentID,
                        "_FileName": "File" + ScanCounter.toString()
                    },
                    type: "POST",
                    success: function (result) {
                        if (result != true) {
                            popupNotification.show('ذخیره سازی با خطا مواجه شد', "error");
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
        ws.send("1100");
    };
    ws.onclose = function () {
        $('.ScannerDalert').modal('show');
    };

}

function ReloadButton_FileManager(e) {

    var Element = e.target[0].parentElement;
    while (Element.id.indexOf("FileManager") == -1) {
        Element = Element.parentElement;
    }

    var fileManager = $("#" + Element.id).data("kendoFileManager");

    fileManager.refresh();
}

function loadFilePublickKey(e) {  
    if (!e.files || e.files.length != 1) return;

    var formData = new FormData();
    formData.append("files", e.files[0].rawFile);  
    $.ajax({
        url: "/Attachment/UploadPublickKey",
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) {
            console.log(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
};

function CreatePublickKey() {
    $.ajax({
        url: "/Management/CreatePublickKey",
        data: {},
        type: "POST",
        success: function (Result) {
            popupNotification.show('عملیات بروز رسانی  با موفقیت انجام شد', "success");
            UploadPublickKeyDialog.close();
        },
        error: function (result) {

        }
    })
}

function CancelCreatePublickKey() {
    UploadPublickKeyDialog.close();
}

function loadFileProductUpdateFromTaxOrganization(e) {
    if (!e.files || e.files.length != 1) return;

    var formData = new FormData();
    formData.append("files", e.files[0].rawFile);
    $.ajax({
        url: "/Management/UploadProductUpdateFromTaxOrganization",
        data: formData,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) {
            console.log(data);
        },
        error: function (e) {
            console.log(e);
        }
    });
};


function ProductUpdateFromTaxOrganization() {
    ShowLoader();
    $.ajax({
        url: "/Management/ProductUpdateFromTaxOrganization",
        data: {},
        type: "POST",
        success: function (Result) {
            HideLoader();
            popupNotification.show('عملیات بروز رسانی  با موفقیت انجام شد', "success");
            ProductUpdateFromTaxOrganizationDialog.close();
        },
        error: function (result) { 
            HideLoader();
        }
    })
}


function CancelUploadProductUpdateFromTaxOrganization() {
    ProductUpdateFromTaxOrganizationDialog.close();
}
