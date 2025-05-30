function ShowLoader() {
    $(".Lodar").css("display", "block");
    var SessionSecond = 0;
    intervalLoader = setInterval(function () {
        $("#LoderTimerCounter").html(fancyTimeFormat(SessionSecond));
        SessionSecond++;
    }, 1000);
}

function HideLoader() {
    $(".Lodar").css("display", "none");
    clearInterval(intervalLoader);
}