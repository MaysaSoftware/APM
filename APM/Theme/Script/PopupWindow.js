function PopupMax(url, title) {
    var params = [
        'scrollbars=yes',
        'resizable=yes',
        'top=' + 50,
        'left=' + 50,
        'height=' + (screen.height - 200),
        'width=' + (screen.width - 100)
    ].join(',');

    var randomnumber = Math.floor((Math.random() * 100) + 1);
    var newWindow = window.open(url, "_blank", params);

    if (window.focus) {
        newWindow.focus();
    }

    return newWindow;
}

function OpenWindow(url, title) {
    var params = [
        'scrollbars=yes',
        'resizable=yes',
        'top=' + 50,
        'left=' + 50,
        'height=' + (screen.height - 200),
        'width=' + (screen.width - 100)
    ].join(',');

    var randomnumber = Math.floor((Math.random() * 100) + 1);
    var newWindow = window.open(url);

    if (window.focus) {
        newWindow.focus();
    }

    return newWindow;
}

function PopupMin(url, title, w, h) {
    var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : window.screenX;
    var dualScreenTop = window.screenTop != undefined ? window.screenTop : window.screenY;

    var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    var left = ((width / 2) - (w / 2)) + dualScreenLeft;
    var top = ((height / 2) - (h / 2)) + dualScreenTop;
    var newWindow = window.open(url, title, 'scrollbars=yes,resizable=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left + ',directories=no,titlebar=no,toolbar=no,location=yes,status=no,menubar=no');

    // Puts focus on the newWindow
    if (window.focus) {
        newWindow.focus();
    }

    return newWindow;
}