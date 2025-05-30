var viewer; 
const magicModdleDescriptor = {
    "name": "Magic",
    "prefix": "magic",
    "uri": "http://magic",
    "xml": {
        "tagAlias": "lowerCase"
    },
    "associations": [],
    "types": [
        {
            "name": "BewitchedStartEvent",
            "extends": [
                "bpmn:StartEvent"
            ],
            "properties": [
                {
                    "name": "spell",
                    "isAttr": true,
                    "type": "String"
                }
            ]
        }
    ]
};

async function OpenDiagram(ModelerName) {
    var bpmnXML = $("#BPMNModelXml").text();
    viewer = new BpmnJS({
        container: '#canvas',
        keyboard: {
            bindTo: window
        },
        //propertiesPanel: {
        //    parent: '#js-properties-panel'
        //},
        //additionalModules: [
        //    BpmnPropertiesPanelModule,
        //    BpmnPropertiesProviderModule,
        //    magicPropertiesProviderModule
        //],
        //moddleExtensions: {
        //    magic: magicModdleDescriptor
        //}
    });
    try {
        await viewer.importXML(bpmnXML);
        var canvas = viewer.get('canvas');
        var overlays = viewer.get('overlays');
        canvas.zoom('fit-viewport');     
    } catch (err) {

        console.error('could not import BPMN 2.0 diagram', err);
    }
    setTimerOn();
}


async function SaveBpmn(ProcessID) {
    ShowLoader();
    try { 
        var result = await viewer.saveXML({ format: true }); 
        $("#BPMNModelXml").text(result.xml);

        await $.ajax({
            type: 'POST',
            dataType: 'json',
            url: "/BPMN/Save",
            data: {
                "BpmnXml": result.xml.replaceAll('<', "*<*").replaceAll('>', "*$>*"),
                "ProcessID": ProcessID
            },
            success: function (result) {
                $("#BPMNVersion").text(result.Version);
                HideLoader();
            },
            error: function (result) {
                HideLoader();
            }
        })
    } catch (err) {
        console.error('نمودار BPMN 2.0 ذخیره نشد', err);
    }
}


async function ExportDiagram() { 

    try {

        var result = await viewer.saveXML({ format: true });

        alert('Diagram exported. Check the developer tools!');

        console.log('DIAGRAM', result.xml);
    } catch (err) {

        console.error('could not save BPMN 2.0 diagram', err);
    }
}

 function loadBpmnFile(event) {
    const reader = new FileReader()
     reader.onload = async event => {
         var bpmnXML = event.target.result;
         $("#BPMNModelXml").text(bpmnXML);
         $("#canvas").empty();
         viewer = new BpmnJS({
             container: '#canvas',
             keyboard: {
                 bindTo: window
             }
         });
         try {
             await viewer.importXML(bpmnXML);
             var canvas = viewer.get('canvas');
             var overlays = viewer.get('overlays');
             canvas.zoom('fit-viewport');
         } catch (err) {

             console.error('could not import BPMN 2.0 diagram', err);
         }
          
    }
    reader.onerror = error => reject(error)
    reader.readAsText(event.target.files[0]);
} 

async function ImportXml(ViewerName) {
    var xml = $("#BPMNModelXml").text();
    viewer = new BpmnJS({
        container: $('#' + ViewerName),
        height: 600
    });

    try
    {
        await viewer.importXML(xml); 
        viewer.get('canvas').zoom('fit-viewport'); 
    }
    catch (err) {

        console('error: ' + err.message);
        console.error(err);
    } 
} 

async function DownloadBPMNModel() { 
    ShowLoader();
    var downloadLink = $('#js-download-diagram');
    try {
        const { xml } = await viewer.saveXML({ format: true });
        setEncoded(downloadLink, SysSettingDetail.title() +'.bpmn', xml);

        await $.ajax({
            type: 'POST',
            dataType: 'json',
            url: "/SysSetting/Download",
            data: { "Format": "bpmn" },
            success: function (result) {
                HideLoader();
            },
            error: function (result) {
                HideLoader();
            }
        })

    }
    catch (err) {
        setEncoded(downloadLink, 'Diagram.bpmn', null);
        HideLoader();
    }

}

async function PrintBPMNModel() { 
    await $.ajax({
        type: 'POST',
        dataType: 'json',
        url: "/SysSetting/Download",
        data: { "Format": "Print"},
        success: function (result) {
            PopupMax("/Print/BPMNModel", "");
        },
        error:{ }
    })

}

function EditBPMNModel() { 
    OpenWindow("/BPMN?ProcessID=" + $("#BPMNModelXmlID").text(), SysSettingDetail.title());
}

async function DownloadSVGBPMNModel()
{
    ShowLoader(); 
    var downloadSvgLink = $('#js-download-svg');
    try
    {
        const { svg } = await viewer.saveSVG();
        setEncoded(downloadSvgLink, SysSettingDetail.title()+'.svg', svg);

        await $.ajax({
            type: 'POST',
            dataType: 'json',
            url: "/SysSetting/Download",
            data: { "Format": "SVG" },
            success: function (result) {
                HideLoader();
            },
            error: function (result) {
                HideLoader();
            }
        }) 
    }
    catch (err)
    { 
        setEncoded(downloadSvgLink, 'Diagram.svg', null);
        HideLoader();
    }
}


function DownloadImageBPMNModel() {
    ShowLoader();
    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: "/SysSetting/Download",
        data: { "Format": "PNG" },
        success: function (result) { 
            try { 
                let canvas = document.getElementById("BPMNOutput");
                let ctx = canvas.getContext("2d");
                ctx.beginPath();
                ctx.arc(95, 50, 40, 0, 2 * Math.PI);
                ctx.stroke();
                let dataURL = canvas.toDataURL('image/png');
                let url = dataURL.replace(/^data:image\/png/, 'data:application/octet-stream'); 
                let downloadLink = document.createElement('a');
                downloadLink.setAttribute('download', SysSettingDetail.title()+'.png');
                downloadLink.setAttribute('href', url);
                downloadLink.click();
            }
            catch (err){
                console.error('Error happened saving png: ', err);
            }
            HideLoader();
        },
        error: function (result) {
            HideLoader();
        }
    }) 
}

function setEncoded(link, name, data) {
    var encodedData = encodeURIComponent(data);

    if (data) {
        link.attr({
            'href': 'data:application/bpmn20-xml;charset=UTF-8,' + encodedData,
            'download': name
        });
    } 
}