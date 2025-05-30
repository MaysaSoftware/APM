function toggleDrawer() {
    if ($("#ReportRightLayout").is(":visible")) { 
        $("#ReportRightLayout").css("display", "none");
        $("#ReportLeftLayout").css("flex", "0 0 100%");
        $("#ReportLeftLayout").css("max-width", "100%");
    }
    else {
        $("#ReportRightLayout").css("display", "block");
        $("#ReportLeftLayout").css("flex", "0 0 75%");
        $("#ReportLeftLayout").css("max-width", "75%");
    }
}


function RunReport() { 
    var FormInputName = [];
    var FormInputValue = [];

    GetInputValue("ReportRightLayout .FormItemInput", FormInputName, FormInputValue);
    for (var Index = 0; Index < FormInputValue.length; Index++) {
        FormInputValue[Index] = FormInputValue[Index].toString().replaceAll(",", "*").replaceAll(" ", "__");
    }
    $("#ReportLeftLayout").load("/Report/ReloadReport?_ParameterName=" + FormInputName  + "&_ParameterValue=" + FormInputValue );
}

function CancelReport() {
    var wnd = ReportViewerWin;
    wnd.close();
}

var oldPageSize = 0;

function OpenReportGrid(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = FormArraye[0].indexOf("Detail") > -1 ? true : false; 
    var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + FormArraye[1]+ (FormArraye[2] > 0 && ISDetailGridForm ? "_" + FormArraye[2] : "");

    if (e.id.indexOf("Excel")>-1) { 
        var grid = $("#" + GridName).data("kendoGrid");
        grid.saveAsExcel();
    }
    else if (e.id.indexOf("PrintColumns")>-1) {
        var grid = $("#" + GridName).data("kendoGrid");  
        //var wnd = GridPrintWin;
        //wnd.title("");
        //var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        //var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        //wnd.setOptions({
        //    width: newWidth - 50,
        //    height: newHeight - 50
        //});

        //wnd.center();
        //wnd.open();

        //$("#GridPrintDiv").kendoGrid({
        //    dataSource: grid.dataSource,
        //    pageable: false,
        //    columns: grid.columns
        //});


        //var inboxGrid = $('#GridPrintDiv').data("kendoGrid");
        //inboxGrid.dataSource.pageSize(inboxGrid.dataSource.total());
        //inboxGrid.refresh();
        //$('[class*="k-pager-nav"]').hide();
        //$("#GridPrintDiv")[0].innerHTML = grid;
        //PopupMax("/Print/Print?_DataKey=" + FormArraye[1] + "&&_ParentID=" + FormArraye[2]+"&&_Request=" + grid.dataSource, SysSettingDetail.title());

        //var wnd = ReportViewerWin;
        //wnd.title(ReportTitle);

        var win = window.open("/Print?_DataKey=" + FormArraye[1] + "&&_ParentID=" + FormArraye[2] + "&&_Request=" + grid.dataSource, '_blank');
        //win.addEventListener('load', function () {
        //    alert('hiiiii');
        //    $("#ReportPrintMainDiv")
        //}, false);
        if (win) {
            //Browser has allowed it to be opened
            win.focus();
            win.onload = function () { alert("message one"); };
            alert("message 1 maybe too soon\n" + popup.onload);
            win.onload = function () { alert("message two"); };
            alert("message 2 maybe too late\n" + popup.onload);
        } else {
            //Browser has blocked it
            alert('Please allow popups for this website');
        }
         
    }
    else if (e.id.indexOf("Pdf") > -1)
    {
        var grid = $("#" + GridName).data("kendoGrid");


        var Filter = [];
        if (grid.dataSource._filter != undefined) {
            for (var index = 0; index < grid.dataSource._filter.filters.length; index++) {
                Filter.push(JSON.stringify(grid.dataSource._filter.filters[index]));
            }
        }

        var Columns = [];
        for (var index = 0; index < grid.columns.length; index++)
            if (grid.columns[index].hidden != true)
                Columns.push(grid.columns[index].field)

        //grid.saveAsPDF();
        //grid.dataSource.view()
        window.location = "/Desktop/ExportPDF?Datakey=" + FormArraye[1] + "&Filter=" + Filter + "&Columns=" + Columns;
        //$.ajax({
        //    url: "/Desktop/ExportPDF",
        //    data: {
        //        //_ParameterName: FormInputName,
        //        //_ParameterValue: FormInputValue
        //    },
        //    type: "POST",
        //    success: function (Result) {
        //        //$("#ReportLeftLayout")[0].innerHTML = "";
        //        //$("#ReportLeftLayout").append(Result);
        //    },
        //    error: function (result) {

        //    }
        //})
    }
    else if (FormArraye.length >= 3) {
        var ISDetailGridForm = e.id.indexOf("DetailParameter_") > -1 ? true : false;
        var ISProcessForm = e.id.indexOf("ProcessTypeParameter_") > -1 ? true : false;
        var ReplaceWord = ISDetailGridForm ? "DetailParameter" : "Parameter";
        ReplaceWord = ISProcessForm ? "ProcessTypeParameter" : ReplaceWord;
        var GridName = ISDetailGridForm ? (FormArraye[0].replace(ReplaceWord, "DetailMainGrid") + FormArraye[1]+"_"+ FormArraye[2]) :( FormArraye[0].replace(ReplaceWord, "MainGrid") + FormArraye[1]);
        var grid = $("#" + GridName).data("kendoGrid");
        var selectedItem = grid != undefined ? grid.dataItem(grid.select()) : null;
        if (selectedItem != null) {


            if (ISProcessForm) {

                var wnd = ReportViewerWin;
                wnd.title("");
                var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

                wnd.setOptions({
                    width: newWidth - 50,
                    height: newHeight - 50
                });

                wnd.center();
                wnd.open();

                $.ajax({
                    url: "/Report/ReloadReportWithProcess",
                    data: {
                        _ProcessID: FormArraye[3],
                        _RecordID: selectedItem.id
                    },
                    type: "POST",
                    success: function (Result) {
                        $("#ReportViewerDiv")[0].innerHTML = "";
                        $("#ReportViewerDiv").append(Result);
                    },
                    error: function (result) {

                    }
                })
            }
            else {
                ShowReport(FormArraye[5], e.item.options.text, FormArraye[6], selectedItem.id);
            }

        }
        else {
            if (FormArraye[7]=="1")
                popupNotification.show("سطری انتخاب نشده است", "info");
            else 
                ShowReport(FormArraye[5], e.item.options.text, FormArraye[6],0);
        }
    }
}


function ExportImage() {
    kendo.drawing.drawDOM($(".content-wrapper"))
        .then(function (group) {
            return kendo.drawing.exportImage(group);
        })
        .done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: "HR-Dashboard.png",
                proxyURL: "/Home/Pdf_Export_Save"
                    });
        });
}




function GridPrintWinClose(e) {
    $("#GridPrintDiv")[0].innerHTML = "";
}

function onLoadPrintReport() { 
    $("#ReportPrintMainDiv").kendoGrid({
        dataSource: JSON.parse(localStorage.getItem('GridDataSource')) ,
        columnMenu: {
            filterable: false
        },
        height: 680,
        editable: "popup",
        pageable: false,
        sortable: false,
        navigatable: false,
        resizable: false,
        reorderable: false,
        groupable: false,
        filterable: false,   
        columns: JSON.parse(localStorage.getItem('GridColumns')) ,
    });  
}