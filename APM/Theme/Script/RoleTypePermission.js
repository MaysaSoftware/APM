var RoleTypeId = 0;
var GridName;

function OpenRoleTypePermission(e) {
    RoleTypeId = 0;
    GridName = e.id.split('_')[0].replace("Create", "MainGrid");
    var grid = $("#" + GridName).data("kendoGrid");
    var wnd = RoleTypePermissionForm;
    wnd.content("<div id='RoleTypePermissionDiv'></div>");

    var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    wnd.setOptions({
        width: newWidth - 50,
        height: newHeight - 50
    });

    wnd.center();
    wnd.open();

    $("#RoleTypePermissionDiv").load("/RoleTypePermission/RoleTypePermission?RoleTypeID=0");

}

function EditRoleTypePermission(e) {
    GridName = e.id.split('_')[0].replace("Update", "MainGrid");
    var grid = $("#" + GridName).data("kendoGrid");
    var SelectItem = grid.dataItem(grid.select());
    if (SelectItem == null) {
        popupNotification.show('رکوردی انتخاب نشده است', "info");
    }
    else {
        RoleTypeId = SelectItem.id;
        var wnd = RoleTypePermissionForm;
        wnd.content("<div id='RoleTypePermissionDiv'></div>");
 
        var newWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        var newHeight = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        wnd.setOptions({
            width: newWidth - 50,
            height: newHeight - 50
        });

        wnd.center();
        wnd.open();

        $("#RoleTypePermissionDiv").load("/RoleTypePermission/RoleTypePermission?RoleTypeID=" + RoleTypeId)
    }
}


function SavePermission() {
    if ($("#RoleType")[0].value != "") {
        $.ajax({
            url: "/RoleTypePermission/SavePermision",
            data: {
                "RoleTypeID": RoleTypeId,
                "Title": $("#RoleType")[0].value,
                "DefaultRoleTypeUrl": $("#DefaultRoleTypeUrl")[0].value,
                "Permission": $("#PermissionText")[0].value
            },
            type: "POST",
            success: function (RecordID) {
                if (RecordID > 0) {
                    popupNotification.show('عملیات ذخیره سازی با موفقیت انجام شد', "success");
                    var grid = $("#" + GridName).data("kendoGrid");
                    grid.dataSource.read();
                    RoleTypePermissionForm.close();
                }
                else
                    popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
            },
            error: function (data) {
                popupNotification.show('بروز خطا هنگام انجام عملیات', "error");
            },
        });
    }
    else
        popupNotification.show('فیلد عنوان وارد نشده است', "error");
}


function checkedNodeIds(nodes, checkedNodes) {
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            checkedNodes.push(nodes[i].id);
            CheckedParentNode(nodes[i].parent(), checkedNodes)
        }

        if (nodes[i].hasChildren) {
            checkedNodeIds(nodes[i].children.view(), checkedNodes);
        }
    }
}

function CheckedParentNode(node, checkedNodes) {
    if (node.parent() != undefined) {
        if (node.id != "" && node.id != undefined)
            if (jQuery.inArray(node.id, checkedNodes) == -1)
                checkedNodes.push(node.id);
        CheckedParentNode(node.parent(), checkedNodes)
    }
}

function onCheckedPermission() {
    var checkedNodes = [],
        treeView = $("#PermissionTree").data("kendoTreeView");

    checkedNodeIds(treeView.dataSource.view(), checkedNodes);

    $("#PermissionText")[0].value = checkedNodes.join(",");

}


