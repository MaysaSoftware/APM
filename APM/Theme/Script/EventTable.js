function OpenEventGrid(e) {
    var FormArraye = e.id.split('_');
    var ISDetailGridForm = FormArraye[0].indexOf("Detail") > -1 ? true : false;
    var GridName = (ISDetailGridForm ? "DetailMainGrid" : "MainGrid") + FormArraye[1] + (FormArraye[2] > 0 && ISDetailGridForm ? "_" + FormArraye[2] : "");
    var grid = $("#" + GridName).data("kendoGrid");
    if (e.id.indexOf("EventEditRecord_") > -1) {
        var selectedItem = grid.dataItem(grid.select());
        if (selectedItem != null) {
            ShowRegistryTable(2,"", "رویداد ویرایش", FormArraye[1], selectedItem.id)
        }
        else
            popupNotification.show("رکوردی انتخاب نشده است", "error"); 
    }
    else if (e.id.indexOf("EventEditAll_") > -1) {
        ShowRegistryTable(2, "", "رویداد ویرایش", FormArraye[1], 0)
    }
    else if (e.id.indexOf("EventInsertRecord_") > -1) {
        var selectedItem = grid.dataItem(grid.select());
        if (selectedItem != null) {
            ShowRegistryTable(1, "", "رویداد درج", FormArraye[1], selectedItem.id)
        }
        else
            popupNotification.show("رکوردی انتخاب نشده است", "error");
    }
    else if (e.id.indexOf("EventInsertAll_") > -1) {
        ShowRegistryTable(1, "", "رویداد درج", FormArraye[1], 0)
    }
    else if (e.id.indexOf("EventDeleteAll_") > -1) {
        ShowRegistryTable(0, "", "رویداد حذف", FormArraye[1], 0)
    }
    else if (e.id.indexOf("EventDownloadRecord_") > -1) {
        var selectedItem = grid.dataItem(grid.select());
        if (selectedItem != null) {
            ShowRegistryTable(3, "", "رویداد بارگیری", FormArraye[1], selectedItem.id)
        }
        else
            popupNotification.show("رکوردی انتخاب نشده است", "error");
    }
    else if (e.id.indexOf("EventDownloadTable_") > -1) {
        ShowRegistryTable(3, "", "رویداد بارگیری", FormArraye[1], 0)
    }
    else if (e.id.indexOf("EventViewTable_") > -1) {
        ShowRegistryTable(4, "", "رویداد نمایش فرم", FormArraye[1], 0)
    }
    else if (e.id.indexOf("EventViewRecordTable_") > -1) {
        var selectedItem = grid.dataItem(grid.select());
        if (selectedItem != null) {
            ShowRegistryTable(4, "", "رویداد نمایش فرم", FormArraye[1], selectedItem.id)
        }
        else
            popupNotification.show("رکوردی انتخاب نشده است", "error"); 
    }
}