function LoadInformationEntryFormBadge() {
    $(".InformationEntryFormBadge").each(function (index, item) {
        $("#" + item.id).load("/RightMenu/InformationEntryFormBadge", { "InformationEntryFormID": item.id.split('_')[1]},
            function (result) {
                if (result > 0) {
                    //result = result > 99 ? "+99" : result;
                    $("#" + item.id).text(result).css("display", "inline");
                }
                else { 
                    $("#" + item.id).text("").css("display", "none");
                }
        })
        var a = item;
    })
}

function DefaultRightMenuSelect(e) {

    var selected = MainRightMenuTree.select();
    var Node = MainRightMenuTree.dataItem(selected);
    if (Node.id != "0") {
        ShowLoader();
        if ($("#" + Node.id).length == 0) {
            if (Node.id.indexOf("InformationEntryForm") > -1) {
                var FormArr = Node.id.split('_');
                var Desktop = FormArr[0].replace("InformationEntryForm", "");

                if (FormArr[1] =="NewForm")
                    OpenEditorForm(Desktop, 0, 0, false, false, Node.text);
                else { 
                    $(".main-section").load("/Home/InformationEntryForm", {
                        'Form': Desktop,
                        'ParentID': 0,
                        'IsDetailGrid': false
                    }, function () {
                        HideLoader();
                    });
                    window.history.pushState("", "", "/Desktop/Index?Form=" + Desktop);
                }
            }
            else if (Node.id.indexOf("Process") > -1) {
                var Process = Node.id.replace("Process", "");

                $(".main-section").load("/Home/InformationEntryForm", {
                    ProcessID: Process
                }, function () {
                    HideLoader();
                });
                window.history.pushState("", "", "/Desktop/Index?ProcessID=" + Process);
            }
            else if (Node.id.indexOf("Report") > -1) {
                var Report = Node.id.replace("Report", "");
                ShowReport(Report, Node.text);
            }
            else if (Node.id.indexOf("Home") > -1) {

                HideLoader();
                $("#HomeMainDiv")[0].innerHTML = "";
                history.pushState(null, document.title, baseUrl + "Home/Index");
            }
            else if (Node.id.indexOf("Dashboard") > -1) {
                var DashboardID = Node.id.replace("Dashboard", "");
                window.history.pushState("", "", "/Dashboard/Index?DashboardID=" + DashboardID);
                $(".main-section").load("/Dashboard/Viewer", {
                    DashboardID: DashboardID
                }, function () {
                    kendo.resize($(".k-chart, .k-grid"));
                    kendo.resize($(".k-grid, .k-chart"));
                    $(document).bind("kendo:skinChange", updateTheme);
                    HideLoader();
                });
            }
        }
        else {
            var TabName = $("#" + Node.id)[0].parentNode.id;
            var index;
            for (index = 0; index < MainTabstrip.contentElements.length; index++) {
                if (MainTabstrip.contentElements[index].id == TabName) {
                    MainTabstrip.select(index);
                    break;
                }
            };
        }
    }
}



$(document).on("keyup", "#MainRightMenuTreeFilter", function (e) {

    if (e.keyCode == 13 && !e.shiftKey) {
        $("#MainRightMenuTree span.k-in .k-treeview-leaf-text span").removeClass("highlight");

        var treeView = $("#MainRightMenuTree").getKendoTreeView();

        if ($.trim($(this).val()) == '') {
            return;
        }

        var term = this.value.toUpperCase();
        var tlen = term.length;

        $('#MainRightMenuTree span.k-in .k-treeview-leaf-text').each(function (index) {
            var text = $(this).text();
            var html = '';
            var q = 0;
            while ((p = text.toUpperCase().indexOf(term, q)) >= 0) {
                html += text.substring(q, p) + '<span class="highlight">' + text.substr(p, tlen) + '</span>';
                q = p + tlen;
            }

            if (q > 0) {
                html += text.substring(q);

                var dataItem = treeView.dataItem($(this));
                $(this).text("");
                $(this).append(html)

                $(this).parentsUntil('.k-treeview').filter('.k-item').each(

                    function (index, element) {
                        $('#MainRightMenuTree').data('kendoTreeView').expand($(this));
                        $(this).data('MainRightMenuTreeFilter', term);
                    });
            }
        });

        $('#MainRightMenuTree .k-item').each(function () {
            if ($(this).data('MainRightMenuTreeFilter') != term) {
                $('#MainRightMenuTree').data('kendoTreeView').collapse($(this));
            }
        });

    }
})

 