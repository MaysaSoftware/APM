function GridToggleDrawerClick(e) {
    var ISDetailGridForm = e.id.indexOf("DetailGridGroupableFieldToggleDrawer") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailGridGroupableFieldToggleDrawer_" : "GridGroupableFieldToggleDrawer_";
    var FormArraye = e.id.split('_');
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];

    var RightMainGridGroupMenuTree = (ISDetailGridForm ? "Detail" : "") + "MainGridGroupRightLayout_" + DataKey + "_" + ParentID;
    var LeftMainGridGroupMenuTree = (ISDetailGridForm ? "Detail" : "") + "MainGridGroupLeftLayout_" + DataKey + "_" + ParentID;


    var RightSide = $("#" + RightMainGridGroupMenuTree);
    var LeftSide = $("#" + LeftMainGridGroupMenuTree);
    if (RightSide.width() > 0) {
        RightSide.css("width", "0%");
        LeftSide.css("width", "100%");
    }
    else { 
        RightSide.css("width", "20%");
        LeftSide.css("width", "80%");
    }
     
}

function SelectedMainGridGroupMenuTree(e) {
    var ItemID = e.sender.element[0].id;
    var ISDetailGridForm = ItemID.indexOf("DetailMainGridGroupMenuTree") > -1 ? true : false; 
    var FormArraye = ItemID.split('_');
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];
    var GridName = (ISDetailGridForm ? "Detail" : "") + "MainGrid" + DataKey + (ISDetailGridForm ? +"_" + ParentID : "");  
    var grid = $("#" + GridName).data("kendoGrid");
    var InputValue = $("#" + ItemID.replace("MainGridGroupMenuTree", "InputMainGridGroupMenuTree")).val();
    var MainGridGroupMenuTree = $("#" + ItemID).data("kendoTreeView");
    var selected = MainGridGroupMenuTree.select();
    var Node = MainGridGroupMenuTree.dataItem(selected);
    var NodeId = Node.id.replace("GroupableField_", "");
    var filter = { logic: "and", filters: [] };
    var keywordfilter = { logic: 'or', filters: [] };
    if (NodeId != "All") {  
        var columns = grid.columns;
        var keyword = NodeId;
        columns.forEach(function (x) {
            if (x.field == InputValue) {
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
                } else if (type == 'boolean') { 
                        keywordfilter.filters.push({
                            field: x.field,
                            operator: 'eq',
                            value: (keyword == 0 ? false:true )
                        }); 
                }
            } 
        }); 

    }
    filter.filters.push(keywordfilter);
    grid.dataSource.filter(filter);
}