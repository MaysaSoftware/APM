var today = new Date();
var tomorrow = new Date();
var threeDays = new Date();

var yesterday = new Date();
var lastWeek = new Date();
var lastMonth = new Date();
var lastYear = new Date();
var nextWeek = new Date();

tomorrow.setDate(new Date().getDate() + 1);
threeDays.setDate(new Date().getDate() + 3);
yesterday.setDate(new Date().getDate() - 1);
lastWeek.setDate(new Date().getDate() - 7);
lastMonth.setDate(new Date().getDate() - 30);
lastYear.setDate(new Date().getDate() - 365);
nextWeek.setDate(new Date().getDate() + 7);

function loading() {
    const l = $("#loading");
    if (l.hasClass('hide'))
        l.removeClass('hide');
    else
        l.addClass('hide');
}
var toastr_option1 =  {
    "closeButton": false, // true/false
    "debug": false, // true/false
    "newestOnTop": false, // true/false
    "progressBar": true, // true/false
    "positionClass": "md-toast-top-left", // md-toast-top-right / md-toast-top-left / md-toast-bottom-right /

    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300", // in milliseconds
    "hideDuration": "1000", // in milliseconds
    "timeOut": "4000", // in milliseconds
    "extendedTimeOut": "4000", // in milliseconds
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
var toastr_options = {
    "closeButton": true, // true/false
    "debug": false, // true/false
    "newestOnTop": false, // true/false
    "progressBar": true, // true/false
    "positionClass": "md-toast-top-left", // md-toast-top-right / md-toast-top-left / md-toast-bottom-right /

    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300", // in milliseconds
    "hideDuration": "1000", // in milliseconds
    "timeOut": "200000", // in milliseconds
    "extendedTimeOut": "20000", // in milliseconds
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
$(document).ajaxStart(function () {
    $("#loading").removeClass('hide');
});
$(document).ajaxStop(function () {
    // $("#loading").addClass('hide');
    setTimeout(function () {
        $("#loading").addClass('hide');
    },
        300);
});
$(document).ajaxComplete(function () {

    $('[data-toggle="popover"]').popover({
        trigger: 'hover',
        container: 'body',
    });
    $('[data-popover="popover"]').popover({
        trigger: 'hover',
        container: 'body',
    });

});
$(document).submit(function () {
    $("#loading").removeClass('hide');
});
function removeComma(input) {
    return input.replace(new RegExp(',', 'g'), "");
}
function autoComma(numberInput) {
    numberInput += '';
    numberInput = numberInput.replace(',', ''); numberInput = numberInput.replace(',', ''); numberInput = numberInput.replace(',', '');
    numberInput = numberInput.replace(',', ''); numberInput = numberInput.replace(',', ''); numberInput = numberInput.replace(',', '');
    var
        x = numberInput.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    const rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1))
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    return x1 + x2;
}
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function validateSelect(e) {
    //console.log('vs');
    //console.log(e);
    var result = true;
    $('.needs-validation .select-wrapper.mdb-select.required').find('input.select-dropdown').each(function () {
       // console.log(this);
        if ($(this).val() === "" || $(this).val() === "انتخاب نمایید") {
            //$(this).focus();
            $(this).addClass('is-invalid').removeClass('is-valid');
            e.preventDefault();
            result = false;
        } else {
            $(this).removeClass('is-invalid').addClass('is-valid');
            // result= true;
        }
    });
    return result;
}
function validate(e) {
    //console.log('vs');
    //console.log(e);
    var result = true;
    $('.needs-validation .form-control.required').each(function () {
       // console.log(this);
        if ($(this).val() === "" || $(this).val() === "انتخاب نمایید") {
            //$(this).focus();
            $(this).addClass('is-invalid').removeClass('is-valid');
            e.preventDefault();
            result = false;
        } else {
            $(this).removeClass('is-invalid').addClass('is-valid');
            // result= true;
        }
    });
    $('.needs-validation .select-wrapper.mdb-select.required').find('input.select-dropdown').each(function () {
        //console.log(this);
        if ($(this).val() === "" || $(this).val() === "انتخاب نمایید") {
            //$(this).focus();
            $(this).addClass('is-invalid').removeClass('is-valid');
            e.preventDefault();
            result = false;
        } else {
            $(this).removeClass('is-invalid').addClass('is-valid');
            // result= true;
        }
    });
    return result;
}
function FoodTitle(id) {
    var title = "";
    $.get("/Welfare/Food/Name/" + id,
        function (data) {
            title = data;
        });
    return title;
}
function UserAccess(item, value, selectId, update = true, empty = true, selectedValue = -100, des = false, all=false) {

    var select = $(`#${selectId}`);
    var doSelect = true;

    $.getJSON(`/${item}/Items/?id=${value}&all=${all}`,
        function (data) {


            select.empty();
            if (data.length > 1) {
                if (empty)
                    select.append($('<option/>',
                        {
                            'value': '',
                            'text': ' ',
                            'selected': 'selected'
                        }));
            }
            if (des === true) {
                for (let i = 0; i < data.length; i++) {

                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title,
                            'data-secondary-text': data[i].Des
                        }));
                }
            } else {
                for (let i = 0; i < data.length; i++) {

                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title
                        }));
                }
            }
            if (data.length === 1) {
                select.removeAttr('multiple');
                select.val(data[0].Id);
                doSelect = false;
                select.trigger("change");
            }
            else if (selectedValue !== -100) {
                select.val(selectedValue);
                doSelect = false;
            }
        })
        .then(
            function () {
                if (update)
                    switch (item) {
                        case 'Company':
                            $('#Subsidiary').empty();
                            $('#Subsidiary').materialSelect({ destroy: true });
                            $('#Subsidiary').materialSelect({});
                            $('#Management').empty();
                            $('#Management').materialSelect({ destroy: true });
                            $('#Management').materialSelect({});
                            $('#Office').empty();
                            $('#Office').materialSelect({ destroy: true });
                            $('#Office').materialSelect({});
                            $('#OfficeDepartment').empty();
                            $('#OfficeDepartment').materialSelect({ destroy: true });
                            $('#OfficeDepartment').materialSelect({});
                            break;
                        case 'Subsidiary':
                            $('#Management').empty();
                            $('#Management').materialSelect({ destroy: true });
                            $('#Management').materialSelect({});
                            $('#Office').empty();
                            $('#Office').materialSelect({ destroy: true });
                            $('#Office').materialSelect({});
                            $('#OfficeDepartment').empty();
                            $('#OfficeDepartment').materialSelect({ destroy: true });
                            $('#OfficeDepartment').materialSelect({});
                        case 'Management':
                            $('#Office').materialSelect({ destroy: true });
                            $('#Office').empty();
                            $('#Office').materialSelect({});
                            $('#OfficeDepartment').materialSelect({ destroy: true });
                            $('#OfficeDepartment').empty();
                            $('#OfficeDepartment').materialSelect({});
                        case 'Office':
                            $('#OfficeDepartment').materialSelect({ destroy: true });
                            $('#OfficeDepartment').empty();
                            $('#OfficeDepartment').materialSelect({});
                        default:
                            break;
                    }
                // Material Select
                select.materialSelect({ destroy: true });
                select.materialSelect({});
                if (doSelect) {
                    $(`[data-activates="select-options-${selectId}"]`).val("")
                        //.prop('placeholder', 'انتخاب نمایید')
                        ;
                } else {
                    select.trigger("change");

                }


            }
        );
};
function UserAccess2(item, value, selectId, update = true, empty = true, selectedValue = -100, all = false) {

    var select = $(`#${selectId}`);
    var doSelect = true;

    $.getJSON(`/${item}/Items/?id=${value}&all=${all}`,
        function (data) {


            select.empty();
            if (data.length > 0) {
                if (empty)
                    select.append($('<option/>',
                        {
                            'value': '',
                            'text': ' ',
                            'selected': 'selected'
                        }));
            }
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
            if (data.length === 1) {
                select.removeAttr('multiple');
                select.val(data[0].Id);
                doSelect = false;
            }
            else if (selectedValue !== -100) {
                select.val(selectedValue);
                doSelect = false;
            }
        })
        .then(
            function () {
                if (update)
                    switch (item) {
                        case 'Company':
                            $('#Subsidiary').empty();
                            $('#Management').empty();
                            $('#Office').empty();
                            $('#OfficeDepartment').empty();
                            break;
                        case 'Subsidiary':
                            $('#Management').empty();
                            $('#Office').empty();
                            $('#OfficeDepartment').empty();
                        case 'Management':
                            $('#Office').empty();
                            $('#OfficeDepartment').empty();
                        case 'Office':
                            $('#OfficeDepartment').empty();
                        default:
                            break;
                    }
                if (doSelect) {
                    $(`[data-activates="select-options-${selectId}"]`).val("")
                        //.prop('placeholder', 'انتخاب نمایید')
                        ;
                } else {
                    select.trigger("change");

                }


            }
        );
};

//خواندن لیست ادارات یک کاربر
//ورودی ای دی سلکت 
function Offices(selectId, selectIdOfDepartmentId) {

    var select = $(`#${selectId}`);
    var doSelect = true;
    var department = '';
    select.materialSelect({ destroy: true });
    $.getJSON("/Office/Items2/",
        function (data) {
            if (data.length === 1) {
                doSelect = false;
                department = JSON.stringify(data[0].Department);
            }
            select.empty();
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title,
                        'department': JSON.stringify(data[i].Department)
                    }));
            }

        })
        .then(
            function () {

                // Material Select
                select.materialSelect({
                });
                if (doSelect)
                    $(`[data-activates="select-options-${selectId}"]`).val("").prop('placeholder', 'انتخاب نمایید');
                else {
                    OfficesDepartments(selectIdOfDepartmentId, department);
                }
            }
        );

}

function OfficesDepartments(selectId, value) {
    //console.log(value);
    var select = $(`#${selectId}`);
    var doSelect = true;
    select.materialSelect({ destroy: true });
    var data = JSON.parse(value);
    // console.log(data);
    if (data.length === 1)
        doSelect = false;
    select.empty();
    for (let i = 0; i < data.length; i++) {
        select.append($('<option/>',
            {
                'value': data[i].Id,
                'text': data[i].Title
            }));
    }

    // Material Select
    select.materialSelect({
    });
    //console.log(doSelect);
    if (doSelect)
        $(`[data-activates="select-options-${selectId}"]`).val("").prop('placeholder', 'انتخاب نمایید');
}


//خواندن لیست ماشین ها
//ورودی ای دی سلکت 
function Vehicles(selectId) {

    var select = $(`#${selectId}`);
    var setEmpty = true;
    $.getJSON("/Vehicle/Items/",
        function (data) {

            select.empty();
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title,
                        'data-secondary-text': data[i].Management + " - " + data[i].Office
                    }));
            }
            //if (data.length === 1) {
            //    select.removeAttr('multiple');
            //    select.val(data[0].Id);
            //    setEmpty = false;
            //}

        })
        .then(
            function () {
                // Material Select
                select.materialSelect({ destroy: true });
                select.materialSelect({});
                if (setEmpty)
                    $(`[data-activates="select-options-${selectId}"]`).val("")
                        //.prop('placeholder', 'انتخاب نمایید')
                        ;
            }
        );

}

//لیست راننده های یک وسیله نقلیه مشخص شده
function Drivers(vehicle, selectInput) {
    var select = $(selectInput);
    $.getJSON("/Vehicle/ListDriver/",
        { id: vehicle },
        function (data) {
            select.empty();

            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].NationalCode,
                        'text': data[i].Name + " " + data[i].Family,
                        'data-secondary-text': data[i].NationalCode
                    }));
            }
        }).then(function () {
            select.materialSelect({ destroy: true });
            select.materialSelect({});
        });
}

//خواندن لیست شماره حساب ها
//ورودی ای دی سلکت 
function AccountNumbers(selectId, selectedValue =0) {

    var select = $(`#${selectId}`);
    var setEmpty = true;
    $.getJSON("/OfficeDepartmentAccountNumbers/Items/",
        function (data) {

            select.empty();
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].AccountNumber,
                        'data-secondary-text': data[i].Title
                    }));
            }
            if (data.length === 1) {
                select.removeAttr('multiple');
                select.val(data[0].Id);
                setEmpty = false;
            }
            if (selectedValue !== 0) {
                select.val(selectedValue);
                setEmpty = false;
                OfficeTitle(selectedValue);
            }

        })
        .then(
            function () {
                // Material Select
                select.materialSelect({ destroy: true });
                select.materialSelect({});
                if (setEmpty)
                    $(`[data-activates="select-options-${selectId}"]`).val("");
            }
        );

}
function Contracts(selectId, selectedValue = 0) {

    var select = $(`#${selectId}`);
    var setEmpty = true;
    $.getJSON("/Welfare/Contract/Items/",
            function (data) {

                select.empty();
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title,
                            'data-secondary-text': data[i].Number
                        }));
                }
                if (data.length === 1) {
                    select.removeAttr('multiple');
                    select.val(data[0].Id);
                    setEmpty = false;
                }
                if (selectedValue !== 0) {
                    select.val(selectedValue);
                    setEmpty = false;
                    OfficeTitle(selectedValue);
                }

            })
        .then(
            function () {
                // Material Select
                select.materialSelect({ destroy: true });
                select.materialSelect({});
                if (setEmpty)
                    $(`[data-activates="select-options-${selectId}"]`).val("");
            }
        );

}
//خواندن مسیر
//ورودی ای دی سلکت و ای دی واحد اداره
function Routes(selectId, selectedValue) {

    var select = $(`#${selectId}`);
    $.getJSON("/Travel/Route/Items/",
        function (data) {
            select.materialSelect({ destroy: true });
            select.empty();
            if (selectedValue === undefined) select.append('<option value="" disabled selected></option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title,

                    }));
            }
            if (selectedValue !== "") {
                $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
            }
        })
        .then(
            function () {

                // Material Select
                select.materialSelect({
                });
                if (selectedValue !== undefined) {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                } else
                    $(`[data-activates="select-options-${selectId}"]`).val("");//.prop('placeholder', 'انتخاب نمایید');



            }

        );

}

//خواندن نوع وسیله نقلیه
//ورودی ای دی سلکت و ای دی واحد اداره
function VehicleTypes(selectId, selectedValue) {

    var select = $(`#${selectId}`);
    $.getJSON("/Travel/VehicleType/Items/",
        function (data) {
            select.materialSelect({ destroy: true });
            select.empty();
            if (selectedValue === undefined) select.append('<option value="" disabled selected></option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title,

                    }));
            }
            if (selectedValue !== "") {
                $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
            }
        })
        .then(
            function () {

                // Material Select
                select.materialSelect({
                });
                if (selectedValue !== undefined) {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                } else
                    $(`[data-activates="select-options-${selectId}"]`).val("");//.prop('placeholder', 'انتخاب نمایید');

            }

        );

}
//خواندن اسامی شهرها
//ورودی ای دی سلکت و ای دی مقدار انتخاب شده
function City(selectId, selectedValue) {

    var select = $(`#${selectId}`);
    $.getJSON("/City/Items/",
        function (data) {
            select.materialSelect({ destroy: true });
            select.empty();
            if (selectedValue === undefined) select.append('<option value=""  selected></option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title,

                    }));
            }
            if (selectedValue !== "") {
                $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
            }
        })
        .then(
            function () {

                // Material Select
                select.materialSelect({
                });
                if (selectedValue !== undefined) {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                } else
                    $(`[data-activates="select-options-${selectId}"]`).val("");//.prop('placeholder', 'انتخاب نمایید');

            }

        );

}
//خواندن سفرها
//ورودی تاریخ و نوع وسیله
function Trips(selectId, date, vehicleType) {
    //var str = "yasin";
    //console.log(str.includes("foodLunch") ? true : false);
    var select = $(`#${selectId}`);

    $.getJSON("/Travel/Trip/JsonList/",
        { date: date, vehicleType: vehicleType },
        function (data) {
            // select.materialSelect({ destroy: true });
            select.empty();
            select.append('<option value="" selected></option>');
            for (let i = 0; i < data.length; i++) {
                var items = '' + data[i].Items;
               // console.log(items);
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Origin + ' به ' + data[i].Destination + ' - ' + data[i].Time,
                        //'data-secondary-text': data[i].Title,
                        'data-Time': data[i].Time,
                        'data-origin': data[i].Origin,
                        'data-destination': data[i].Destination,
                        'data-destination-id': data[i].DestinationId,
                        'data-origin-id': data[i].OriginId,
                        'data-route': data[i].Route,
                        'data-foodLunch': items.includes("foodLunch")? true: false,
                        'data-foodBreakfast': items.includes("foodBreakfast")? true: false,
                        'data-foodDinner': items.includes("foodDinner")? true: false,
              
                    }));
            }
        })
        .then(
            function () {

                // Material Select
                select.materialSelect({
                });

                $(`[data-activates="select-options-${selectId}"]`).val("").prop('placeholder', 'انتخاب نمایید');

            }
        );

}

//خواندن لیست غذاها
//ورودی سلکت مورد نظر
function Food(selectInput) {

    var select = $(`${selectInput}`);

    $.getJSON("/Welfare/Food/Items",
        function (data) {
            select.empty();
            select.append('<option value="" disabled selected class="d-none"></option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }

        })
        .then(
            function () {
                select.materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({
                });

            }
        );

}

//خواندن لیست وعده های غذایی
//ورودی سلکت مورد نظر
function Meals(selectInput, all = false, bed = true) {
    var select = $(`${selectInput}`);

    $.getJSON("/Welfare/Meal/Items",
        { all: all, bed : bed },
        function (data) {
            select.empty();
            select.append('<option value="" disabled selected class="d-none">انتخاب نمایید</option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
        })
        .then(
            function () {
                select.materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({
                });

            }
        );
}
//خواندن لیست وعده های غذایی
//ورودی دیو مورد نظر
function MealsInput(divInput, prefix, all = false) {
    var div = $(`${divInput}`);

    $.getJSON("/Welfare/Meal/Items",
        { all: all },
        function (data) {
            
            var t = "";
            for (let i = 0; i < data.length; i++) {
                t += `
                        <div class="col">
                            <div class="md-form md-outline my-2 ">
                                <input type="number" class="form-control text-center" data-val="true" data-val-required=" " id="${prefix}-${data[i].Id}" name="${prefix}-${data[i].Id}" value="0" min="0" max="10000">
                                <label for="${prefix}-${data[i].Id}" class="m-active">${data[i].Title}</label>
                            </div>
                        </div>`;
            }
            
            div.html(t);
        })
        ;
}

function MealsInputShift(divInput, prefix, all = false) {
    var div = $(`${divInput}`);

    $.getJSON("/Welfare/Meal/Items",
        { all: all,shift:true },
        function (data) {

            var t = "";
            for (let i = 0; i < data.length; i++) {
                t += `
                        <div class="col">
                            <div class="md-form md-outline my-2 ">
                                <input type="number" class="form-control text-center" data-val="true" data-val-required=" " id="${prefix}-${data[i].Id}" name="${prefix}-${data[i].Id}" value="0" min="0" max="10000">
                                <label for="${prefix}-${data[i].Id}" class="m-active">${data[i].Title}</label>
                            </div>
                        </div>`;
            }
            t += `<div class="col">
                            <div class="md-form md-outline my-2 ">
                                <input type="number" class="form-control text-center afterDinner" data-val="true" data-val-required=" " id="${prefix}-5" name="${prefix}-5" value="0" min="0" max="10000">
                                <label for="${prefix}-5" class="m-active">پس شام</label>
                            </div>
                        </div>`;
            div.html(t);
        })
        ;
}
//خواندن لیست وعده های غذایی
//ورودی div مورد نظر
function MealsSwitch(div, all = false) {
    $.ajax({
        url: "/Welfare/Meal/ItemsSwitch",
        data: { all: all },
        type: 'GET',
        success: function (data) {
            $(`${div}`).html(data);
        },
        error: function (data) {
            toastr["error"](`هنگام انجام درخواست شما  خطایی رخ داده است.<br /><small>${data}</small>`);
        }
    }).then(function () {
        //$("#loading").addClass('hide');
    });


}
//خواندن لیست رستوران های دسترسی دار یک کاربر
//ورودی سلکت مورد نظر
function Restaurants(selectInput) {

    var select = $(`${selectInput}`);
    $.getJSON("/Welfare/Restaurant/Items",
        function (data) {
            select.empty();
            select.append('<option value="" disabled selected>انتخاب نمایید</option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
        })
        .then(
            function () {
                $(`${selectInput}`).materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({});

            }
        );
}

function Questions(selectInput, survey) {
   // console.log(survey);
    var select = $(`${selectInput}`);
    $.getJSON("/Welfare/Survey/QuestionItems/" + survey,
            function (data) {
                select.empty();
                select.append('<option value="" disabled selected>انتخاب نمایید</option>');
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title
                        }));
                }
            })
        .then(
            function () {
                $(`${selectInput}`).materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({});

            }
        );
}
function Surveys(selectInput) {

    var select = $(`${selectInput}`);
    $.getJSON("/Welfare/Survey/SurveyItems/" ,
            function (data) {
                select.empty();
                select.append('<option value="" disabled selected>انتخاب نمایید</option>');
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title
                        }));
                }
            })
        .then(
            function () {
                $(`${selectInput}`).materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({});

            }
        );
}
function RestaurantsContract(selectInput, contract) {

    var select = $(`${selectInput}`);
    $.getJSON("/Welfare/Restaurant/Contract/" + contract,
        function (data) {
            select.empty();
            select.append('<option value="" disabled selected></option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
        })
        .then(
            function () {
                $(`${selectInput}`).materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({});

            }
        );
}

function PriceSerisContract(selectInput, contract) {

    var select = $(`${selectInput}`);
    $.getJSON("/Welfare/Contract/PriceSeris/" + contract,
            function (data) {
                select.empty();
                select.append('<option value="" disabled selected></option>');
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title
                        }));
                }
            })
        .then(
            function () {
                $(`${selectInput}`).materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({});

            }
        );
}
//خواندن لیست رستوران های دسترسی دار یک کاربر
//ورودی سلکت مورد نظر
function FoodReservationStatus(selectInput) {

    var select = $(`${selectInput}`);
    $.getJSON("/Welfare/FoodReservationStatus/Items",
        function (data) {
            select.empty();
            select.append('<option value="" disabled selected></option>');
            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
        })
        .then(
            function () {
                select.materialSelect({ destroy: true });
                select.materialSelect({});

            }
        );
}


//لیست غذاهای یک رستوارن در تاریخ و وعده مشخص شده
function listFood(restaurant, meal, date, selectInput) {
    //console.log(meal);
    //console.log(date);
    var select = $(selectInput);
    $.getJSON("/reservation/food/RestaurantFood",
        { restaurant: restaurant, meal: meal, date: date },
        function (data) {
            select.materialSelect({ destroy: true });
            select.empty();

            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
        }).then(
            function () {
                // Material Select
                select.materialSelect({
                });
            }
        );
}

//لیست غذاهای یک رستوران در تاریخ و وعده مشخص شده
function Foods(restaurant, meal, date, selectInput) {
    //console.log(restaurant, meal, date);
    var select = $(selectInput);
    $.getJSON("/reservation/food/RestaurantFood",
        { restaurant: restaurant, meal: meal, date: date },
        function (data) {

            for (let i = 0; i < data.length; i++) {
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title
                    }));
            }
        }).then(function () {
            select.append('<option value="" style="color:red">عدم رزرو غذا</option>');
        });
}

//لیست تخت
function Beds(room,status, selectInput) {
    var select = $(selectInput);
    $.getJSON("/Welfare/Bed/Items/",
        { id: room, status: status },
        function (data) {
            select.empty();

            for (let i = 0; i < data.length; i++) {
                
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Number
                    }));
            }
            
        }).then(function () {
        
    });
}

function IsChecked(checkedId) {
    if ($(`#${checkedId}`).is(':checked')) return true;
    else return false;
}

function OfficeTitle(id) {
    $.getJSON("/OfficeDepartmentAccountNumbers/JsonOffice/",
        { id: id },
        function(data) {
            $("#OfficeTitle").html(data.Office);
            $("#OfficeDepartmentTitle").html(data.OfficeDepartment);
        });
};
$("#AccountNumber").on('change',
    function (e) {
        OfficeTitle(e.target.value);
    }
   
    );




$(document).ready(
    function () {
        // SideNav Initialization
        // SideNav Default Options
        $('.button-collapse').sideNav({
            edge: 'right', // Choose the horizontal origin
            closeOnClick: false, // Closes side-nav on &lt;a&gt; clicks, useful for Angular/Meteor
            breakpoint: 1440, // Breakpoint for button collapse
            MENU_WIDTH: 240, // Width for sidenav
            timeDurationOpen: 300, // Time duration open menu
            timeDurationClose: 200, // Time duration open menu
            timeDurationOverlayOpen: 50, // Time duration open overlay
            timeDurationOverlayClose: 200, // Time duration close overlay
            easingOpen: 'easeOutQuad', // Open animation
            easingClose: 'easeOutCubic', // Close animation
            showOverlay: true, // Display overflay
            showCloseButton: false // Append close button into siednav
        });
        // SideNav Scrollbar Initialization
        var container = document.querySelector('.custom-scrollbar');
        var ps = new PerfectScrollbar(container, {
            wheelSpeed: 2,
            wheelPropagation: true,
            minScrollbarLength: 20
        });

        new WOW().init();

        $('.timepicker').timepicker({
            timeFormat: 'HH:mm',
            //defaultTime: '7',
            dynamic: false,
            dropdown: false,
            scrollbar: false
        });

        //Active SideNav Link
        var selector = '.collapsible li';
        var url = window.location.pathname.toLowerCase();
        //console.log(url);
        var purl = $("#p-link").html();
        //console.log(purl);
        try {
            purl = purl.toLowerCase();

        } catch (e) {

        }
        $(selector).each(function () {
            const link = $(this).find('a');
            const l = link[0].pathname.toLowerCase();
           

            //link.attr('href')
            if (l === url || l === purl) {
                $(selector).removeClass('active');
                $(this).parent().parent().parent().removeClass('active').addClass('active');
                $(this).parent().parent().parent().find('a:first').removeClass('active').addClass('active');
                $(this).find('a').removeClass('active').addClass('active');
                $(this).parent().parent().parent().find('div:first').css({ "display": "block" });
            }
        });
        //End Active SideNav Link



        // Material Select Initialization
        $('.mdb-select:not(#advancedSearch .mdb-select):not(.ajax-select):not(.delay-mdb-select)').materialSelect();
        $('.mdb-select.not-required.do-select.select-wrapper .select-dropdown').val("").prop('placeholder', 'انتخاب نمایید');
        $('.mdb-select.required.do-select.select-wrapper .select-dropdown').val("").prop('placeholder', 'انتخاب نمایید');


        //$('.select-wrapper.md-form.md-outline input.select-dropdown').bind('focus blur', function () {
        //    $(this).closest('.select-outline').find('label').toggleClass('active');
        //    $(this).closest('.select-outline').find('.caret').toggleClass('active');
        //    //$(this).closest('.select-outline').find('li:first').toggleClass('disabled');
        //});


        $('.needs-validation select').on('change', e => validateSelect(e));
        $('.needs-validation').on('submit', e => validateSelect(e));


        $('#StartDate').on('change',
            function () {
                var f = $('#StartDate').val();
                var t = $('#EndDate').val();
                if (f > t) {
                    $('#eDate').MdPersianDateTimePicker('setDate', new Date(f));
                }
            });

        $('#EndDate').on('change',
            function () {
                var f = $('#StartDate').val();
                var t = $('#EndDate').val();
                if (f > t) {
                    $('#sDate').MdPersianDateTimePicker('setDate', new Date(t));
                }
            });
    });


//خواندن وضعیت کالا

function ProductStatus(selectId, category1, selectedValue) {
    //var str = "yasin";
    //console.log(str.includes("foodLunch") ? true : false);
    var select = $(`#${selectId}`);

    $.getJSON("/Store/Category2/JsonList/",
        { category1: category1 },
        function (data) {
            // select.materialSelect({ destroy: true });
            select.empty();
            if (selectedValue === undefined) {
                select.append('<option value=""  selected>&nbsp;</option>');
                //console.log(1);
            }
            for (let i = 0; i < data.length; i++) {
                var items = '' + data[i].Items;
                // console.log(items);
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title ,
                        'data-Repairable': data[i].Repairable,
                        'data-Requestable': data[i].Requestable,
                        'data-Threshold': data[i].Threshold,
                      

                    }));
            }

            if (selectedValue !== "") {
                $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                //console.log(2);
            }
        })
        .then(
            function () {

                // Material Select
                select.materialSelect({});

                if (selectedValue !== undefined) {
                    //console.log(3);
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                } else {
                   // console.log(4);
                
                    $(`[data-activates="select-options-${selectId}"]`).val("");//.prop('placeholder', 'انتخاب نمایید');
                }
            }
        );

}

//خواندن دسته بندی دو

function Category2(selectId, category1, selectedValue) {
    //alert(selectedValue);
    //var str = "yasin";
    //console.log(str.includes("foodLunch") ? true : false);
    var select = $(`#${selectId}`);

    $.getJSON("/Store/Category2/JsonList/",
        { category1: category1 },
        function (data) {
            // select.materialSelect({ destroy: true });
            select.empty();
            if (selectedValue === undefined || selectedValue === 1) {
                select.append('<option value="1"  selected>&nbsp;</option>');
                //console.log(1);
            }
            for (let i = 0; i < data.length; i++) {
                var items = '' + data[i].Items;
                // console.log(items);
                select.append($('<option/>',
                    {
                        'value': data[i].Id,
                        'text': data[i].Title ,
                        'data-Repairable': data[i].Repairable,
                        'data-Requestable': data[i].Requestable,
                        'data-Threshold': data[i].Threshold,
                      

                    }));
            }

            if (selectedValue !== "") {
                $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                //console.log(2);
            }
        })
        .then(
            function () {

                // Material Select
                select.materialSelect({});

                if (selectedValue !== undefined) {
                    //console.log(3);
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                } else {
                   // console.log(4);
                
                    $(`[data-activates="select-options-${selectId}"]`).val("");//.prop('placeholder', 'انتخاب نمایید');
                }
            }
        );

}


//خواندن دسته بندی یک

function Category1(selectId, selectedValue) {

    var select = $(`#${selectId}`);
    $.getJSON("/Store/Category1/JsonList/",
            function (data) {
                select.materialSelect({ destroy: true });
                select.empty();
                if (selectedValue === undefined) select.append('<option value=""  selected>&nbsp;</option>');
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title,

                        }));
                }
                if (selectedValue !== "") {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                }
            })
        .then(
            function () {

                // Material Select
                select.materialSelect({});
                if (selectedValue !== undefined) {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                } else
                    $(`[data-activates="select-options-${selectId}"]`).val("");//.prop('placeholder', 'انتخاب نمایید');

            }

        );

}

//خواندن لیست رستوران های دسترسی دار یک کاربر
//ورودی سلکت مورد نظر
function Stores(selectId, selectedValue, all = false, ids =-5) {
   //console.log(selectedValue);
   // console.log(all);
    var select = $(`#${selectId}`);
    var setEmpty = true;
    $.getJSON(`/Store/Stores/Items/?all=${all}&ids=${ids}`,
            function (data) {
                select.empty();
                if (selectedValue === undefined) select.append('<option value="" disabled selected></option>');
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title
                        }));
                }
                if (selectedValue !== "") {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                    select.val(selectedValue);

                    setEmpty = false;
                }
                else if (data.length === 1) {
                    //select.removeAttr('multiple');
                    select.val(data[0].Id);
                    setEmpty = false;
                }
               
            })
        .then(
            function () {
                $(`#${selectId}`).materialSelect({ destroy: true });
                // Material Select
                select.materialSelect({});
                if (selectedValue !== undefined) {
                    $(`#${selectId} option[value = "${selectedValue}"]`).attr("selected", "selected");
                }
                else if (setEmpty)
                    $(`[data-activates="select-options-${selectId}"]`).val("");
            }
        );
}

function Products(selectId) {

    var select = $(`#${selectId}`);
    var setEmpty = true;
    $.getJSON("/Store/Products/Items/",
            function (data) {

                select.empty();
                for (let i = 0; i < data.length; i++) {
                    select.append($('<option/>',
                        {
                            'value': data[i].Id,
                            'text': data[i].Title,
                            'data-secondary-text': data[i].Category1 + " - " + data[i].Category1
                        }));
                }

            })
        .then(
            function () {
                // Material Select
                select.materialSelect({ destroy: true });
                select.materialSelect({});
                if (setEmpty)
                    $(`[data-activates="select-options-${selectId}"]`).val("");
            }
        );

}