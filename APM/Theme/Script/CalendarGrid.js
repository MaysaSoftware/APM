function CalendarGridCreateRow(e) {
    var wnd = UserCalendarWin; 
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    }); 
    wnd.center();
    wnd.open();
} 

function CalendarGridReload() {
    var grid = $("#calendar-grid").data("kendoGrid");
    if (grid != undefined)
        grid.dataSource.read();
}

function CalendarGridExcel(e) {
    var grid = $("#calendar-grid").data("kendoGrid");
    grid.saveAsExcel();
}

function CalendarCancelButton() {
    var wnd = UserCalendarWin; 
    wnd.close();
}

function ClearCalendarForm() {
    $(':input', '#CalendarForm')
        .not(':button, :submit, :reset, :hidden')
        .val('') ;
}

function CalendarSaveButton() {
    if ($("#C-FromDate").val() == "" || $("#C-ToDate").val() == "" || $("#C-Title").val() == "") {
        popupNotification.show('فیلد های ستاره دار را پر نمایید', "error");
    }
    else {
        var CalendarRepeatButton = $("#CalendarRepeatButton").data("kendoButtonGroup");
        var AlarmTypeCombobox = $("#AlarmType").data("kendoComboBox")
        if (CalendarRepeatButton.selectedIndices[0] == 0) {

            $.ajax({
                url: "/UserCalendar/Save",
                data: {
                    FromDate: $("#C-FromDate").val(),
                    ToDate: $("#C-ToDate").val(),
                    FromTime: $("#C-FromTime").val(),
                    ToTime: $("#C-ToTime").val(),
                    Title: $("#C-Title").val(),
                    Description: $("#C-Description").val(),
                    AlarmType: AlarmTypeCombobox.value()
                },
                type: "POST",
                success: function (Result) {
                    SuccessSaveCalendar();
                },
                error: function (result) {

                }
            })
        }
        else if (CalendarRepeatButton.selectedIndices[0] == 1) {
            var DailyCount = $("#DailyCount").data("kendoNumericTextBox"); 

            $.ajax({
                url: "/UserCalendar/SaveDaily",
                data: {
                    FromDate: $("#C-FromDate").val(),
                    ToDate: $("#C-ToDate").val(),
                    FromTime: $("#C-FromTime").val(),
                    ToTime: $("#C-ToTime").val(),
                    Title: $("#C-Title").val(),
                    Description: $("#C-Description").val(),
                    AlarmType: AlarmTypeCombobox.value(),
                    DailyCount: DailyCount.value()
                },
                type: "POST",
                success: function (Result) {
                    SuccessSaveCalendar();
                },
                error: function (result) {

                }
            })
        }
        else if (CalendarRepeatButton.selectedIndices[0] == 2) {
            var WeeklyCount = $("#WeeklyCount").data("kendoNumericTextBox");
            var CalendarRepeatWeeklyButton = $("#CalendarRepeatWeeklyButton").data("kendoButtonGroup");
            
            $.ajax({
                url: "/UserCalendar/SaveWeekly",
                data: {
                    FromDate: $("#C-FromDate").val(),
                    ToDate: $("#C-ToDate").val(),
                    FromTime: $("#C-FromTime").val(),
                    ToTime: $("#C-ToTime").val(),
                    Title: $("#C-Title").val(),
                    Description: $("#C-Description").val(),
                    AlarmType: AlarmTypeCombobox.value(),
                    WeeklyCount: WeeklyCount.value(),
                    WeeklyDays: CalendarRepeatWeeklyButton.selectedIndices.join(',')
                },
                type: "POST",
                success: function (Result) {
                    SuccessSaveCalendar();
                },
                error: function (result) {

                }
            })
        }
        else if (CalendarRepeatButton.selectedIndices[0] == 3) {
            var MonthlyCount = $("#MonthlyCount").data("kendoNumericTextBox");
            var DayOfMonth = $("#M-DayOfMonth").data("kendoNumericTextBox");
            var WeekOfMonth = $("#C-R-WeekOfMonthButton").data("kendoButtonGroup");
            var DaysOfWeekOfMonth = $("#C-R-DaysOfWeekOfMonthButton").data("kendoButtonGroup");
            var CheckedWeekOfMonth = $("#CheckedWeekOfMonth").data("kendoSwitch");
            
            $.ajax({
                url: "/UserCalendar/SaveMonthly",
                data: {
                    FromDate: $("#C-FromDate").val(),
                    ToDate: $("#C-ToDate").val(),
                    FromTime: $("#C-FromTime").val(),
                    ToTime: $("#C-ToTime").val(),
                    Title: $("#C-Title").val(),
                    Description: $("#C-Description").val(),
                    AlarmType: AlarmTypeCombobox.value(),
                    MonthlyCount: MonthlyCount.value(),
                    DayOfMonth: DayOfMonth.value(),
                    CheckedWeekOfMonth: CheckedWeekOfMonth.element[0].checked,
                    WeekOfMonth: WeekOfMonth.selectedIndices.join(','),
                    DaysOfWeekOfMonth: DaysOfWeekOfMonth.selectedIndices.join(',')
                },
                type: "POST",
                success: function (Result) {
                    SuccessSaveCalendar();
                },
                error: function (result) {

                }
            })
        }
        else if (CalendarRepeatButton.selectedIndices[0] == 4) {
            var YearlCount = $("#YearlCount").data("kendoNumericTextBox");
            var DayOfMonthOfYear = $("#DayOfMonthOfYear").data("kendoNumericTextBox");
            var MonthOfYear = $("#C-R-MonthOfYearButton").data("kendoButtonGroup");
            var WeekOfMonthOfYear = $("#C-R-WeekOfMonthOfYearButton").data("kendoButtonGroup");
            var DayOfWeekOfMonthOfYear = $("#C-R-DayOfWeekOfMonthOfYearButton").data("kendoButtonGroup");
            var CheckedMonthOfYear = $("#CheckedMonthOfYear").data("kendoSwitch");
            
            $.ajax({
                url: "/UserCalendar/SaveYearly",
                data: {
                    FromDate: $("#C-FromDate").val(),
                    ToDate: $("#C-ToDate").val(),
                    FromTime: $("#C-FromTime").val(),
                    ToTime: $("#C-ToTime").val(),
                    Title: $("#C-Title").val(),
                    Description: $("#C-Description").val(),
                    AlarmType: AlarmTypeCombobox.value(),
                    YearlCount: YearlCount.value(),
                    DayOfMonthOfYear: DayOfMonthOfYear.value(),
                    CheckedMonthOfYear: CheckedMonthOfYear.element[0].checked,
                    MonthOfYear: MonthOfYear.selectedIndices.join(','),
                    WeekOfMonthOfYear: WeekOfMonthOfYear.selectedIndices.join(','),
                    DayOfWeekOfMonthOfYear: DayOfWeekOfMonthOfYear.selectedIndices.join(',')
                },
                type: "POST",
                success: function (Result) {
                    SuccessSaveCalendar();
                },
                error: function (result) {

                }
            })
        }
        
    }
}

function SuccessSaveCalendar() {
    popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
    ClearCalendarForm();
    CalendarCancelButton();
    ShowCalenderGrid();
    CalendarGridReload();
}

function CalendarRepeatButton(e) {
    if (e.indices[0] == 0) {
        $(".CalendarDaily,.CalendarWeekly,.CalendarMonthly,.CalendarYearly").css("display", "none");
    }
    else if (e.indices[0] == 1) {
        $(".CalendarDaily").css("display", "block");
        $(".CalendarWeekly,.CalendarMonthly,.CalendarYearly").css("display", "none");
    }
    else if (e.indices[0] == 2) { 
        $(".CalendarWeekly").css("display", "block");
        $(".CalendarDaily,.CalendarMonthly,.CalendarYearly").css("display", "none");
    }
    else if (e.indices[0] == 3) { 
        $(".CalendarDaily,.CalendarWeekly,.CalendarYearly").css("display", "none");
        $(".CalendarMonthly").css("display", "block");
    }
    else if (e.indices[0] == 4) { 
        $(".CalendarDaily,.CalendarWeekly,.CalendarMonthly").css("display", "none");
        $(".CalendarYearly").css("display", "block");
    }

}
 
function CalendarCheckedWeekly(e) {
    var WeekOfMonth = $("#C-R-WeekOfMonthButton").data("kendoButtonGroup");
    var DaysOfWeekOfMonth = $("#C-R-DaysOfWeekOfMonthButton").data("kendoButtonGroup");
    var numerictextbox = $("#M-DayOfMonth").data("kendoNumericTextBox");

    if (e.checked == true)
    {
        WeekOfMonth.enable(true);
        DaysOfWeekOfMonth.enable(true); 
        numerictextbox.enable(false);
    }
    else
    {
        WeekOfMonth.enable(false); 
        DaysOfWeekOfMonth.enable(false); 
        numerictextbox.enable(true);
    } 
}

function CalendarCheckedMonthly(e) {
    var WeekOfMonthOfYear = $("#C-R-WeekOfMonthOfYearButton").data("kendoButtonGroup");
    var DayOfWeekOfMonthOfYear = $("#C-R-DayOfWeekOfMonthOfYearButton").data("kendoButtonGroup");
    var numerictextbox = $("#DayOfMonthOfYear").data("kendoNumericTextBox");

    if (e.checked == true) {
        WeekOfMonthOfYear.enable(true); 
        DayOfWeekOfMonthOfYear.enable(true); 
        numerictextbox.enable(false); 
    }
    else { 
        WeekOfMonthOfYear.enable(false); 
        DayOfWeekOfMonthOfYear.enable(false); 
        numerictextbox.enable(true); 
    } 
}

var TaskType = false;
var TaskID = false;
var CalendarGrid;

function DoneCalendarClick(Item) {
    TaskType = true;
    TaskID = Item;

    CalendarGrid = $("#calendar-grid").data("kendoGrid");
    var dataItem = CalendarGrid.dataSource.get(Item);
    var row = $("#calendar-grid").data("kendoGrid").tbody.find("tr[data-uid='" + dataItem.uid + "']"); 

    CalendarGrid.select(row);
    UserCalendarDialog.title("تایید "+dataItem.عنوان);
    UserCalendarDialog.open();
}

function EditCalendarClick(Item) {

    TaskID = Item; 
    CalendarGrid = $("#calendar-grid").data("kendoGrid");
    var dataItem = CalendarGrid.dataSource.get(Item);
    var row = $("#calendar-grid").data("kendoGrid").tbody.find("tr[data-uid='" + dataItem.uid + "']");
    var AlarmTypeCombobox = $("#CE-AlarmType").data("kendoComboBox");

    CalendarGrid.select(row);

    $("#CE-Date")[0].value = dataItem.تاریخ;
    $("#CE-Title")[0].value = dataItem.عنوان;
    $("#CE-Description")[0].value = dataItem.شرح_فعالیت;
    $("#CE-FromTime")[0].value = dataItem.از_ساعت;
    $("#CE-ToTime")[0].value = dataItem.تا_ساعت;
    $("#CE-DescriptionResult")[0].value = dataItem.علت_یا_نتیجه;
    AlarmTypeCombobox.value(dataItem.نوع_هشدار);

    var wnd = UserCalendarEditWin;
    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });
    wnd.center();  
    wnd.open();
}

function CancelCalendarClick(Item) {
    TaskType = false;
    TaskID = Item; 
    SelectRow(Item);
    var dataItem = CalendarGrid.dataSource.get(Item);
    UserCalendarDialog.title("لغو " + dataItem.عنوان);
    UserCalendarDialog.open();
}

function SelectRow(Item) {
    TaskID = Item;
    CalendarGrid = $("#calendar-grid").data("kendoGrid");
    var dataItem = CalendarGrid.dataSource.get(Item);
    var row = $("#calendar-grid").data("kendoGrid").tbody.find("tr[data-uid='" + dataItem.uid + "']");

    CalendarGrid.select(row);
}

function SaveUserCalendarDialog() { 
    $.ajax({
        url: "/UserCalendar/SaveTaskType",
        data: {
            TaskID: TaskID,
            TaskType: TaskType,
            Description: $("#U-C-D-Title").val()
        },
        type: "POST",
        success: function (Result) {
            popupNotification.show('عملیات بروز رسانی  با موفقیت انجام شد', "success");
            UserCalendarDialog.close();
            $("#U-C-D-Title")[0].value = "";
            ShowCalenderGrid();
            CalendarGridReload();
        },
        error: function (result) {

        }
    })
}

function CancelUserCalendarDialog() {
    ShowCalenderGrid();
}

function CalendarGridDestroyRow(Item) {
    SelectRow(Item);
    DeleteCalendarDialog.open();
}

function CalendarEditSaveButton() {

    var AlarmTypeCombobox = $("#CE-AlarmType").data("kendoComboBox");
    $.ajax({
        url: "/UserCalendar/SaveEdit",
        data: {
            TaskID: TaskID,
            CDate: $("#CE-Date").val(),
            FromTime: $("#CE-FromTime").val(),
            ToTime: $("#CE-ToTime").val(),
            Title: $("#CE-Title").val(),
            Description: $("#CE-Description").val(),
            DescriptionResult: $("#CE-DescriptionResult").val(),
            AlarmType: AlarmTypeCombobox.value()
        },
        type: "POST",
        success: function (Result) {
            popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
            CalendarEditCancelButton();
            CalendarGridReload();
            SelectRow(TaskID);

        },
        error: function (result) {

        }
    })
}

function CalendarEditCancelButton() {
    ClearCalendarForm();
    var wnd = UserCalendarEditWin;
    wnd.close();
    ShowCalenderGrid();
}

function DeleteCalendar() {
    CalendarGrid = $("#calendar-grid").data("kendoGrid");
    var dataItem = CalendarGrid.dataSource.get(TaskID);
    var row = $("#calendar-grid").data("kendoGrid").tbody.find("tr[data-uid='" + dataItem.uid + "']");
    CalendarGrid.select(row);

    $.ajax({
        url: "/UserCalendar/Destroy",
        data: {
            TaskID: TaskID,
        },
        type: "POST",
        success: function (Result) {
            popupNotification.show('عملیات حذف  با موفقیت انجام شد', "success");
            CalendarGrid.removeRow(CalendarGrid.select());
            ShowCalenderGrid();
        },
        error: function (result) {

        }
    })
}

function CancelDeleteCalendar() { 
    ShowCalenderGrid();
}