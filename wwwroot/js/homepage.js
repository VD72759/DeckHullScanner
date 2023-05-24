$(document).ready(function () {

    $("input[custAttr=numberonly]").keypress(function (e) {
        if (e.which != 13 && e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57))
            return false;
    });

    $('button').click(function () {
        var value = $(this).val();
        var EmpId = $("#EmpId").val();
        var EmpType = $("#EmpType").val();
        if (value != "") {
            if (value == "temp" || value == "full") {
                $("#EmpType").val(value);
                $(".empTypeBtn").removeClass("btn-selected");
                $("#" + value + "Btn").addClass("btn-selected");
            }
            else
                if (value == "clr") {
                    EmpId = "";
                }
                else
                    if (value == "del") {
                        EmpId = EmpId.slice(0, -1)
                    }
                    else
                        if (value == "enter") {
                            if (EmpType == "" && EmpId == "") {
                                alert("Please enter valid data")
                            }
                            else
                                if (EmpType == "") {
                                    alert("Please select employee type (FULL/TEMP)")
                                }
                                else
                                    if (EmpId == "") {
                                        alert("Please enter employee id")
                                    }
                                    else {
                                        if (validateEmpId()) {
                                            isEmployeeIdExists(EmpId, EmpType).always(function (resData) {
                                                if (resData != null && resData.status == "1" && resData.redirecturl != "") {
                                                    window.location = resData.redirecturl;
                                                }
                                                else {
                                                    alert("Invalid Data")
                                                }
                                            });
                                        }
                                        else {
                                            alert("Invalid employee id");
                                            EmpId = "";
                                        }
                                    }
                        }
                        else {
                            EmpId += value;
                        }
            $("#EmpId").val(EmpId);
        }

    });
});


function validateEmpId() {


    return true;
}

function isEmployeeIdExists(EmpId, EmpType) {
    var urlEmployeeExists = $("#CheckEmployeeExists").val();
    if (EmpId != "" && EmpType != "") {
        var parseData = {
            "EmpId": EmpId,
            "EmpType": EmpType
        };

        return $.ajax({
            type: "POST",
            url: urlEmployeeExists,
            contentType: "application/json",
            data: JSON.stringify(parseData),
            success: function (data) {
                return data;
            },
            error: function (data) {
                alert("Some error occoured. Please try later");
            }
        });
    }
    else {
        alert("Insufficient Employee data. Kindly enter Employee Id & Type");
    }
    return null;
}


