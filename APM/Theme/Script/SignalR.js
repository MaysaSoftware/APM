////////$(document).ready(function () {
////////    var jQsignalR = $.noConflict(true);
////////    jQsignalR.connection.hub.url = "http://localhost:8086/signalr";
////////    SignalRService = jQsignalR.connection.scanServiceHub;

////////    if (SignalRService != undefined) {
////////        SignalRService.client.addMessage = function (name, message) {
////////            if (name == 'Status' && message == 'ScanningComplete') {
////////                console.log("success");
////////            }
////////        };

////////        jQsignalR.connection.hub.start().done(function () {
////////        });
////////    }
////////});
