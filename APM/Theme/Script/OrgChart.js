function ORGChartDataBinding(e) {
    var ItemArr = e.id.split('_');
    $("#OrgChart_" + ItemArr[1] + "_" + ItemArr[2]).empty();

    $.ajax({
        type: 'POST',
        url: "/OrgChart/Read",
        data: {
            "_DataKey": ItemArr[1], 
            "_ParentId": ItemArr[2],
        },
        dataType: 'json',
        success: function (data) {
            var data3 = [];
            for (let index = 0; index < data.length; index++) {
                var Item = data[index];
                if (Item.ParentId == 0)
                    data3.push({ id: Item.Id, name: Item.Name, title: Item.Title, expanded: Item.Expanded, avatar: Item.Avatar });
                 else
                    data3.push({ id: Item.Id, name: Item.Name, title: Item.Title, expanded: Item.Expanded, parentId: Item.ParentId, avatar: Item.Avatar });
            }
            $("#OrgChart_" + ItemArr[1] + "_" + ItemArr[2]).kendoOrgChart({
                editable: false,
                dataSource:data3
            });

            $("#MainGrid" + ItemArr[1]).css("display", "none");

        },
        error: function (result) {
            
        }
    }); 
 
}


function OrgChartCreateRow(e) {
    var ISDetailGridForm = e.id.indexOf("DetailCreate") > -1 ? true : false;
    var ReplaceWord = ISDetailGridForm ? "DetailCreate" : "Create";
    var FormArraye = e.id.split('_');
    var DataKey = FormArraye[1];
    var ParentID = FormArraye[2];
    OpenEditorForm(DataKey, ParentID, 0, ISDetailGridForm, false)
}

function GridShowOfChart(e) {
    var ItemArr = e.id.split("_");
    $("#OrgChartDiv_" + ItemArr[1] + "_" + ItemArr[2]).css("display", "none");
    $("#MainGrid" + ItemArr[1]).css("display", "block");
}
 
function ExportPDFOrgChart(e) {

    var ItemArr = e.id.split("_");
    $.ajax({
        type: 'POST',
        url: "/OrgChart/DownloadChart",
        data: {
            "DataKey": ItemArr[1],
            "ParentId": ItemArr[2],
            "FormatType": "pdf"
        },
        dataType: 'json',
        success: function (data) {
            var ChartElement = $("#OrgChart_" + ItemArr[1] + "_" + ItemArr[2] + " .k-orgchart-container");
            var HTML_Width = ChartElement.width();
            var HTML_Height = $("#OrgChart_" + ItemArr[1] + "_" + ItemArr[2]).height();
            var top_left_margin = 15;
            var PDF_Width = HTML_Width + (top_left_margin * 2);
            var PDF_Height = (PDF_Width * 1.5) + (top_left_margin * 2);
            var canvas_image_width = HTML_Width;
            var canvas_image_height = HTML_Height;

            var totalPDFPages = Math.ceil(HTML_Height / PDF_Height) - 1;

            html2canvas(ChartElement[0]).then(function (canvas) {
                var imgData = canvas.toDataURL("image/jpeg", 1.0);
                var pdf = new jsPDF('p', 'pt', [PDF_Width, PDF_Height]);
                pdf.addImage(imgData, 'JPG', top_left_margin, top_left_margin, canvas_image_width, canvas_image_height);
                for (var i = 1; i <= totalPDFPages; i++) {
                    pdf.addPage(PDF_Width, PDF_Height);
                    pdf.addImage(imgData, 'JPG', top_left_margin, -(PDF_Height * i) + (top_left_margin * 4), canvas_image_width, canvas_image_height);
                }
                pdf.save(data+".pdf");

            });


        },
        error: function (result) {

        }
    });
}

function OrgChartRengeChange(e) {

    var ItemArr = e.id.split("_");
    var ChartElement = $("#OrgChart_" + ItemArr[1] + "_" + ItemArr[2] + " .k-orgchart-container");
    var zoomScale = Number(e.value) / 100;
    setZoom(zoomScale, ChartElement, $("#OrgChart_" + ItemArr[1] + "_" + ItemArr[2]).height(), ChartElement.width())
}
function setZoom(zoom, ChartElement, Height, Width) {

    var el = ChartElement[0];
    transformOrigin = [0, 0];
    el = el || instance.getContainer();
    var p = ["webkit", "moz", "ms", "o"],
        s = "scale(" + zoom + ")",
        oString = (transformOrigin[0] * 100) + "% " + (transformOrigin[1] * 100) + "%";

    for (var i = 0; i < p.length; i++) {
        el.style[p[i] + "Transform"] = s;
        el.style[p[i] + "TransformOrigin"] = oString;
    }

    el.style["transform"] = s;
    el.style["transformOrigin"] = oString; 


}