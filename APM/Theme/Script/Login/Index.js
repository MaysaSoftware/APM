var SignalRService = null;

$(document).ready(function () {
    $.ajax({
        url: "/Home/WindowResize",
        data: {
            _Width: screen.width,
            _Height: screen.height
        },
        type: "POST",
        success: function (Result) { },
        error: function (result) { }
    })
});

window.onload = function () {
    $("#UserName").focus();
    $("#Password").parent().append("<div class='k-icon k-i-preview'></div>");
    $(".k-i-preview").mousedown(function () {
        $("#Password").attr("type", "text");
        $(".k-i-preview").css("color", "#ff6358");
    })
    $(".k-i-preview").mouseup(function () {
        $("#Password").attr("type", "password");
        $(".k-i-preview").css("color", "gray");
    })


    $('#ForgotPassword').click(function () {
        $("#SignInForm").css("display", "none");
        $("#ForgotPasswordForm").css("display", "flex");
    })

    $('#ForgotPasswordGoToSignINForm').click(function () {
        $("#SignInForm").css("display", "flex");
        $("#ForgotPasswordForm").css("display", "none");
    })

    $('#SignUp').click(function () {
        $("#SignInForm").css("display", "none");
        $("#SignupForm").css("display", "flex");
    })


    $('#signInPage').click(function () {
        $("#SignupForm").css("display", "none");
        $("#SignInForm").css("display", "flex");
    })


    $('#ManagementLaws').click(function () {
        $('.LawsDalert').modal('show');
    })


    $("#ForgotPassword_EmailText").data("kendoTextBox").enable(false);
    $("#ForgotPassword_PhoneText").data("kendoTextBox").enable(false);
}

$(document).on("keyup", "#ForgotPassword_PhoneText,#ForgotPassword_EmailText", function (e) {

    if (e.keyCode == 13 && !e.shiftKey) {
        SendCode();
    }
})

function Loginclick() {
    var UserName = $("#UserName").val();
    var Password = $("#Password").val();
    $($("#UserName")[0].parentElement).css("border-color", "#ced4da");
    $($("#Password")[0].parentElement).css("border-color", "#ced4da");

    if (UserName == "") {
        $("#ValidationMessage").html("نام کاربری را وارد نمایید");
        $($("#UserName")[0].parentElement).css("border-color", "red");
    }
    else if (Password == "") {
        $("#ValidationMessage").html("کلمه عبور را وارد نمایید");
        $($("#Password")[0].parentElement).css("border-color", "red");
    }
}

function SendTypeChange(e) {
    $('#ValidationMessage').html("");
    if (e.id == "SendSMS") {
        $("#ForgotPassword_PhoneText").data("kendoTextBox").enable(true);
        $("#ForgotPassword_EmailText").data("kendoTextBox").enable(false);
        $("#ForgotPassword_EmailText")[0].value = "";
    }
    else {
        $("#ForgotPassword_EmailText").data("kendoTextBox").enable(true);
        $("#ForgotPassword_PhoneText").data("kendoTextBox").enable(false);
        $("#ForgotPassword_PhoneText")[0].value = "";
    }
}

function SendCode() {
    var SendType = "";
    var SendValue = "";
    var RadioButtons = document.getElementsByName('SendType');
    for (index = 0; index < RadioButtons.length; index++) {
        if (RadioButtons[index].checked) {
            SendType = RadioButtons[index].id.replace("Send", "");
            SendValue = SendType == "SMS" ? $("#ForgotPassword_PhoneText")[0].value : $("#ForgotPassword_EmailText")[0].value;
            break;
        }
    }

    if (SendType == "SMS" && $("#ForgotPassword_PhoneText")[0].value == "") {
        $('#ValidationMessage').html("شماره تلفن خالی می باشد");
    }
    else if (SendType == "Email" && $("#ForgotPassword_EmailText")[0].value == "") {
        $('#ValidationMessage').html("آدرس ایمیل خالی می باشد");
    }
    else {
        $.ajax({
            url: "/Signin/SendCode",
            data: {
                "SendType": SendType,
                "UserName": SendValue
            },
            type: "POST",
            success: function (Message) {
                if (Message == 1) {
                    $("#VerifyCodeForm").css("display", "flex");
                    $("#ForgotPasswordForm").css("display", "none");
                }
                else {
                    $('#ValidationMessage').html(Message);
                }
            },
            error: function (result) {
                alert(result)
            }
        })
    }
}