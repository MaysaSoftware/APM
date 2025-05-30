var ChartName = [];
var ChartTime = [];

function onTileResize(e) {
    if (e.container) {
        kendo.resize(e.container, true);
    }
}
 

function updateTheme() {
    var charts = $(".k-chart");
    $.each(charts, function (i, elem) {
        var theme = kendoTheme;
        if (/(default-|bootstrap-|material-|classic-|fluent-)/.test(kendoTheme)) {
            theme = "sass";
        }
        $(elem).getKendoChart().setOptions({ theme: theme });
    });
}

function FilterSubDashboard(e) { 
    if (e.style.display === "none" || e.style.display === "") {
        e.style.display = "flex";
    } else {
        e.style.display = "none";
    }
} 


function ReloadDashboard(id) {

    ShowLoader();  

    //var DataKey = FormArr[1]; 
    var SearchFieldItem = [];
    var SearchFieldOperator = [];
    var SearchFieldValue = [];
    var FormInputName = [];
    var FormInputValue = [];

    GetInputValue("DashboardHeaderLayout", FormInputName, FormInputValue);
     
    $("#DashboardHeaderLayout .SearchFieldItem:visible").each(function (index, Item) {
        if (Item.children[0].id != "" && Item.children[0].id.indexOf("SearchFieldButton") == -1 && Item.children[0].id.indexOf("SwitchFieldButton") == -1) {
            SearchFieldItem.push(Item.children[0].id);
            SearchFieldOperator.push($(Item.children[1].firstElementChild.firstElementChild).val());
            SearchFieldValue.push($("#DashboardHeaderLayout #" + Item.children[1].firstElementChild.firstElementChild.name.replace("_input", "").replace("SearchFieldOperator_", "SearchField_")).val());
        }
    });


    window.history.pushState("", "", "/Dashboard/Index?DashboardID=" + id + "&&FromDate=" + $("#Dashboard_از_تاریخ")[0].value + "&&ToDate=" + $("#Dashboard_تا_تاریخ")[0].value);
    $(".main-section").load("/Dashboard/Viewer", {
        DashboardID: id ,
        FromDate: $("#Dashboard_از_تاریخ")[0].value,
        ToDate: $("#Dashboard_تا_تاریخ")[0].value,
        SearchFieldItem: SearchFieldItem,
        SearchFieldOperator: SearchFieldOperator,
        SearchFieldValue: SearchFieldValue
    }, function () {
        kendo.resize($(".k-chart, .k-grid"));
        kendo.resize($(".k-grid, .k-chart"));
        $(document).bind("kendo:skinChange", updateTheme);
        HideLoader();
    });
}



function ReloadPieSubDashboard(id, ChartName) {
    //var CoockieValue = getCookie();
    var ChartType = $("#" + ChartName.id.replace("Chart", "ChartTypes")).val();
    switch (ChartType) {
        case "جدول": {
            var grid = $("#" + ChartName.id).data("kendoGrid");
            if (grid != undefined)
                grid.dataSource.read({
                    "FromDate": $("#TemplateSub_FromDate_" + id)[0].value,
                    "ToDate": $("#TemplateSub_ToDate_" + id)[0].value,
                    IsReload: true
                });
            break;
        }
        default: {
            $.ajax({
                url: "/Dashboard/ReloadPieSubDashboard",
                data: {
                    DashboardID: id,
                    FromDate: $("#TemplateSub_FromDate_" + id)[0].value,
                    ToDate: $("#TemplateSub_ToDate_" + id)[0].value
                },
                type: "POST",
                success: function (Result) {
                    var chart = $("#" + ChartName.id).data("kendoChart");
                    chart.options.series[0].data = Result;
                    chart.refresh();
                },
                error: function (result) {

                }
            })
            break;
        }
    }
}


function ReloadBarSubDashboard(id, ChartName) {
    var ChartType = $("#" + ChartName.id.replace("Chart", "ChartTypes")).val();
    switch (ChartType) {
        case "جدول": {
            var grid = $("#" + ChartName.id).data("kendoGrid");
            if (grid != undefined)
                grid.dataSource.read({
                    "FromDate": $("#TemplateSub_FromDate_" + id)[0].value,
                    "ToDate": $("#TemplateSub_ToDate_" + id)[0].value,
                    IsReload: true
                });
            break;
        }
        default: {
            $.ajax({
                url: "/Dashboard/ReloadBarSubDashboard",
                data: {
                    DashboardID: id,
                    FromDate: $("#TemplateSub_FromDate_" + id)[0].value,
                    ToDate: $("#TemplateSub_ToDate_" + id)[0].value
                },
                type: "POST",
                success: function (Result) {
                    var chart = $("#" + ChartName.id).data("kendoChart");
                    chart.options.series[0].data = Result.SeriesBar;
                    chart.options.categoryAxis.categories = Result.Categories;
                    chart.refresh();
                },
                error: function (result) {

                }
            })
            break;
        }
    }

    $("#" + ChartName.parentElement.parentElement.id + " .MainDivDashboardDate").css("display", "none");
}


function onloadDashboard(Item) {
    var Id = Item.sender.element[0].id;
    var chartArr = Id.split("___");
    if (ChartName.indexOf(Id) == -1) {
        var Time = $("#" + Id.replace("___Chart", "___ReloadTime")).val();
        if (Time > 0) { 
            setInterval(function () {
                ReloadPieSubDashboard(chartArr[chartArr.length - 1].replace("Chart", ""), Item.sender.element[0]);
            }, Time * 60000);
            ChartName.push(Id);
        }
    } 
}


function SetReloadTime(Item) {
    var Id = Item.id;
    var chartArr = Id.split("___");
    var isSetChart = false;
    var Time = $("#" + Id.replace("___Chart", "___ReloadTime")).val();
    if (ChartName.indexOf(Id) == -1) {
        isSetChart = true
    }
    else {
        if (ChartTime[ChartName.indexOf(Id)] != Time)
            isSetChart = true;
    }
    if (isSetChart == true) {

        if (Time > 0) {
            setInterval(function () {
                ReloadPieSubDashboard(chartArr[chartArr.length - 1].replace("Chart", ""), Item);
            }, Time * 60000);
            ChartName.push(Id);
            ChartTime.push(Time);

            setCookie("Reload" + Id, Time, 1000);
            $("#" + Item.parentElement.parentElement.id + " .MainDivDashboardDate").css("display", "none");
            
        }
    }
}




function PieChartClick(e) { 

    var ID = e.sender.element[0].id;
    var chartArr = ID.split("___");
    var SwitchChecked = $("#" + ID.replace("___Chart", "___Switch"))[0].checked;
    var SubDashboardID = e.sender.element[0].id.split("___")[2].replace("Chart", "");
    var DashboardID = chartArr[chartArr.length - 2];
    var WincontentName = "PopupDashboardWindowDiv" + DashboardID;
    var Title = e.sender.options.title.text == undefined ? e.series.name : e.sender.options.title.text;

    if (Title == undefined)
        Title = "";

    var wnd = $("#PopupDashboardWindow" + DashboardID).data("kendoWindow");
    wnd.title(Title+ ' - ' + e.category);
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });

    wnd.content("<div id='" + WincontentName+"' style = 'height: 100%;'></div>")
    wnd.center();
    wnd.open();


    if (SwitchChecked) {
        $("#" + WincontentName).load("/Dashboard/ShowDetailDashboard", {
            SubDashboardID: SubDashboardID,
            Category: e.category,
            FromDate: $("#TemplateSub_FromDate_" + SubDashboardID)[0].value,
            ToDate: $("#TemplateSub_ToDate_" + SubDashboardID)[0].value
        }, function () { 
            HideLoader();
        }); 
    }
    else {
        $("#" + WincontentName).load("/Dashboard/SubViewer", {
            SubDashboardID: SubDashboardID,
            Category: e.category,
            FromDate: $("#TemplateSub_FromDate_" + SubDashboardID)[0].value,
            ToDate: $("#TemplateSub_ToDate_" + SubDashboardID)[0].value
        }, function () {
            HideLoader();
        });
    }

}

function CloseHeaderDashboard(Element, e) {
    ToggleFullScreenSubDashboard(Element, e);
    var elem = document.getElementById(Element.parentElement.parentElement.id);
    $("#" + elem.id + " .Dashboard_HeaderToolbar").css("display", "");
    $("#" + elem.id + " .Dashboard_HeaderLayout").css("display", "none");
}

function ReportExcelDashboard(Element, e) { 
    var ChartType = Element.value;
    switch (ChartType) {
        case "جدول": {
            var grid = $("#" + Element.id.replace("ChartTypes", "Chart")).data("kendoGrid");
            grid.saveAsExcel();
            break;
        }
    }
}

function ReportPDFDashboard(Element, e) {
    var ChartType = Element.value;
    switch (ChartType) {
        case "جدول": {
            var grid = $("#" + Element.id.replace("ChartTypes", "Chart")).data("kendoGrid");
            //setCookie("GridDataSource", grid.dataSource);
            //localStorage.setItem("GridColumns", grid.columns);
            localStorage.setItem('GridDataSource', JSON.stringify(grid.dataSource))
            localStorage.setItem('GridColumns', JSON.stringify(grid.columns))

            var win = window.open("/Print?_DataKey=" + 0 + "&&_ParentID=" + 0 + "&&_Request=" + grid.dataSource, '_blank'); 
            if (win) {
                //Browser has allowed it to be opened
                win.focus();
                win.onload = function () {

                    //var tasks = getCookie("GridDataSource"); 
                    //var data = JSON.parse(localStorage.getItem('data'));
                };
                //alert("message 1 maybe too soon\n" + popup.onload);
                //win.onload = function () { alert("message two"); };
                //alert("message 2 maybe too late\n" + popup.onload);
            } else {
                //Browser has blocked it
                alert('Please allow popups for this website');
            }
            break;
        }
    }
}

function AlignColumnTextDashboard(Element, e) { 

    var ChartType = Element.value;
    switch (ChartType) {
        case "جدول": {
            var alignResult = "right";

            switch (e.title) {
                case "راست چین":
                    {
                        alignResult = "right";
                        break;
                    }
                case "چپ چین":
                    {
                        alignResult = "left";
                        break;
                    }
                case "وسط چین":
                    {
                        alignResult = "center";
                        break;
                    }
            }
            $("#" + Element.id.replace("ChartTypes", "Chart") + " thead th").css("text-align", alignResult);
            $("#" + Element.id.replace("ChartTypes", "Chart") + " tbody td").css("text-align", alignResult);
            break;
        }
    }
}

function ShowDashboard(Element, e) {
    ToggleFullScreenSubDashboard(Element, e);
    var elem = document.getElementById(Element.parentElement.parentElement.id);   
    $("#" + elem.id + " .Dashboard_HeaderToolbar").css("display", "none");
    $("#" + elem.id + " .Dashboard_HeaderLayout").css("display", "flex"); 
    $("#" + elem.id + " .k-filter-row").css("display", "none");
    $("#" + elem.id + " .MainDivDashboardDate").css("display", "none");
    $("#TimerCounter").html("-100");
}

function ToggleFullScreenSubDashboard(Element, e)
{ 
    var elem = document.getElementById(Element.parentElement.parentElement.id);  
    if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement) { 
        if (elem.requestFullscreen) {
            elem.requestFullscreen();
        } else if (elem.msRequestFullscreen) {
            elem.msRequestFullscreen();
        } else if (elem.mozRequestFullScreen) {
            elem.mozRequestFullScreen();
        } else if (elem.webkitRequestFullscreen) {
            elem.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
        }

        if (e.className.indexOf("ToggleFullScreenSubDashboard") > -1) { 
            $(e).removeClass("k-i-fullscreen");
            $(e).addClass("k-i-fullscreen-exit");
        }
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitExitFullscreen) {
            document.webkitExitFullscreen();
        }

        if (e.className.indexOf("ToggleFullScreenSubDashboard") > -1) {
            $(e).removeClass("k-i-fullscreen-exit");
            $(e).addClass("k-i-fullscreen");
        }

        $("#" + elem.id + " .Dashboard_HeaderToolbar").css("display", "");
        $("#" + elem.id + " .Dashboard_HeaderLayout").css("display", "none");
    }
}

function SubdashboardShowFilter_Change(e) { 
    var ChartType = $("#" + e.sender.element[0].id.replace("ShowFilter", "ChartTypes")).val()
    switch (ChartType) {
        case "جدول": {
            if (e.checked) {
                $("#" + e.sender.element[0].id.replace("ShowFilter", "Chart") + " .k-filter-row").css("display", "");
            }
            else {
                $("#" + e.sender.element[0].id.replace("ShowFilter", "Chart") + " .k-filter-row").css("display", "none");
            }
            break;
        }
    }

    setCookie(e.sender.element[0].id, e.checked, 1000); 
}

function LoadFirstSubDashboard(e) {
    var dashboardArr = e.id.split('_');
    var chartArr = e.id.split("___");
    var ShowFilterCoockieValue = getCookie(e.id.replace("ChartTypes", "ShowFilter"));
    var ReloadtimeCoockieValue = getCookie("Reload"+e.id.replace("ChartTypes", "Chart"));
    var ChartType = e.value;
    
    setTimeout(function () {
        switch (ChartType) {
            case "جدول": {
                if (ShowFilterCoockieValue == "true") {
                    $("#" + e.id.replace("ChartTypes", "Chart") + " .k-filter-row").css("display", "");
                }
                else if (ShowFilterCoockieValue == "false") {
                    $("#" + e.id.replace("ChartTypes", "Chart") + " .k-filter-row").css("display", "none");
                }
                var PageCount = 0;
                var PageIndex = 1;
                setInterval(function () {
                    var grid = $("#" + e.id.replace("ChartTypes", "Chart")).data("kendoGrid");
                    grid.dataSource.page();
                    if (grid != undefined) { 
                        PageCount = $("#" + e.id.replace("ChartTypes", "Chart") + " .k-dropdown-list option").length;
                        if (PageIndex >= PageCount)
                            PageIndex = 0;
                        //grid.dataSource.page(++PageIndex);
                    }
                }, 1 * 60000);
                break;
            }
        }
    });
}

function ChangeHeaderFontSizeTypes(e) {
    $("#" + e.sender.element[0].id.replace("FontSizeTypes", "Chart").replace("Header_", "") + " thead th a").css("font-size", e.sender.value());
}
function ChangeBodyFontSizeTypes(e) {
    $("#" + e.sender.element[0].id.replace("FontSizeTypes", "Chart").replace("Body_", "")+ " tbody td").css("font-size", e.sender.value());
}

function onkeydownFontWeightDashboard(e) {
    if (e.id.indexOf("HeaderFontWeigh") > -1) 
        $("#" + e.id.replace("HeaderFontWeight", "Chart").replace("Header_", "") + " thead th").css("font-weight", e.value);
    else
        $("#" + e.id.replace("BodyFontWeight", "Chart").replace("Body_", "") + " tbody td").css("font-weight", e.value);
}

function ChangeHeaderColorPicker(e) { 
    $("#" + e.sender.element[0].id.replace("HeaderColorPicker", "Chart")+ " thead tr").css("background-color", e.value);
}