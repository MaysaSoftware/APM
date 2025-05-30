var handler;
var MaxSessionSecond = 1800;
var TimerInfoBadgeSecond = 1;
var SessionSecond = MaxSessionSecond;

 
function SessionTimerOut() {
    window.location.href = "/Error/Expire";
}

function setTimerOn() {
    handler = setInterval(function () {
        $("#TimerCounter").html(fancyTimeFormat(SessionSecond));

        $("#TimeCounter").html(GetClockTime());
        $(".ClockTimeCounter").text(GetClockTime());
        SessionSecond--;

        $("#TimerInformationEntryFormBadgeCounter").html(TimerInfoBadgeSecond);
        TimerInfoBadgeSecond++; 

        if (SessionSecond == 0) {
            SessionTimerOut();
        }

        if ((TimerInfoBadgeSecond % 60) == 0) {
            if ($("#DataRecoveryMinutesInput").length > 0) {
                var DataRecoveryMinutesVal = $("#DataRecoveryMinutesInput").val();
                if (DataRecoveryMinutesVal != "0") {
                    if (DataRecoveryMinutesVal == (TimerInfoBadgeSecond / 60)) {
                        GetUserNotification();
                        LoadInformationEntryFormBadge();
                        TimerInfoBadgeSecond = 1;
                    }
                }
            }
        }

    }, 1000);
}

$(document).click(function () {
    SessionSecond = MaxSessionSecond;
});

function NumWith(string, length) {
    return (new Array(length + 1).join('0') + string).slice(-length);
}

function fancyTimeFormat(time) {
    var mins = ~~((time % 3600) / 60);
    var secs = time % 60;

    var ret = NumWith(mins, 2) + ":" + NumWith(secs, 2);

    return ret;
}

function GetClockTime() {
    var NowDate = new Date();
    var dateString = '';

    var Hour = NowDate.getHours();
    var Min = NowDate.getMinutes();
    var Sec = NowDate.getSeconds();

    if (Hour < 10) Hour = '0' + Hour;
    if (Min < 10) Min = '0' + Min;
    if (Sec < 10) Sec = '0' + Sec;

    dateString = Hour + ':' + Min + ':' + Sec;
    return dateString;
}