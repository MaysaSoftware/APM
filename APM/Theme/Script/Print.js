
function PagePrint() {
    window.print();
}


function PageSize(size) {
    changeCSS("/Theme/CSS/Print/Print_" + size + ".css", 18);
}

function changeCSS(cssFile, cssLinkIndex) {
    var oldlink = document.getElementsByTagName("link").item(cssLinkIndex);
    oldlink.setAttribute("href", cssFile);
}