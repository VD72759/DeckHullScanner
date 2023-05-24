$(document).ready(function () {

    $("input[custAttr=numberonly]").keypress(function (e) {
        if (e.which != 13 && e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57))
            return false;
    });

});
