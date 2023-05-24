var red = "#FF0000";
var white = "#FFFFF";
var lightGray = "#CCC";
var lightBlue = "#add8e6";
var lightGreen = "#66FF99";
var lightOrange = "#FFDAB9";

$(document).ready(function () {

    $("#EndUnit").css("background-color", lightBlue);
    $("#EndUnit").focus();

    $('button').click(function () {
        var value = $(this).val();

        if (value == "override") {
            var EmpId = $("#EmployeeNumber").val();
            var ItemPart = $("#ItemPart").val();
            var EndUnit = $("#EndUnit").val();
            ApproveOverride(EmpId, ItemPart, EndUnit).always(function (resData) {
                if (resData != null) {
                    if (resData.status == "0") {
                        if (resData.redirecturl != "")
                            window.location = resData.redirecturl;
                    }
                    else
                    if (resData.status == "1")
                    {


                    }

                }
                else {
                    alert("Unable to logout. User is already logged out.")
                }
            });

        }
        else
        if (value == "clear") {
            $("#responseMessage").text("");
            $("#ItemPart").val("");
            $("#EndUnit").val("");
            ChangePanelBackground(0); 
        }
        else
        if (value == "logout") {
            Logout().always(function (resData) {
                if (resData != null) {
                    if (resData.status == "0")
                        alert(resData.message);
                    if(resData.redirecturl != "")
                        window.location = resData.redirecturl;
                }
                else {
                    alert("Unable to logout. User is already logged out.")
                }
            });
        }


    });


    $("#EndUnit").blur(function () {
        var value = $(this).val();
        var BgColor = 0;
        if (value != "") {
            ValidateEndUnitPartNumber(value).always(function (resData) {
                if (resData != null && resData.status != "") {
                    if (resData.status == "0") {
                        BgColor = 2;
                        $("#EndUnit").css("background-color", lightBlue);
                    }
                    else {
                        $("#EndUnit").css("background-color", white);
                        $("#ItemPart").css("background-color", lightBlue);
                        $("#ItemPart").focus();
                        BgColor = 0;
                    }
                    ChangePanelBackground(BgColor);
                }
                else {
                    alert("Invalid Data")
                }
            });

        }
    });

    $("#ItemPart").blur(function () {
        var value = $(this).val();
        var endUnit = $("#EndUnit").val();
        var employeeNumber = $("#EmployeeNumber").val();
        
        if (value != "") {
            if (endUnit == "") {
                alert("Invalid End Unit Part number")
            }
            else {
                ValidateItemPartNumber(value, endUnit, employeeNumber).always(function (resData) {
                    if (resData != null && resData.status != "") {
                        if (resData.status == "-1") {
                            alert(resData.message);
                            $("#EndUnit").css("background-color", lightBlue);
                            $("#EndUnit").val("");
                            $("#EndUnit").focus();
                        }
                        else
                        if (resData.status == "1")
                        {
                            $("#PreEndUnit").val(endUnit);
                            $("#PreItemPart").val(value);
                            $("#ItemPart").val("");
                            $("#EndUnit").val("");
                            $("#EndUnit").focus();
                            ChangePanelBackground(1);
                        }
                        else
                        {
                            $("#PreEndUnit").val(endUnit);
                            $("#PreItemPart").val(value);
                            ChangePanelBackground(2);
                            if(resData.message != "")
                                alert(resData.message);
                        }
                    }
                    else {
                        alert("Invalid Data")
                    }
                });

            }
        }
    });

});

function ChangePanelBackground(state) {
    // 0 - Default, 1 - Good, 2 - Bad, 3 - Warning
    var PanelBackgroundColor = lightGray;
    if (state == 1) {
        PanelBackgroundColor = lightGreen;
    }
    else
    if (state == 2) {
        PanelBackgroundColor = red;
    }
    else
    if (state == 3) {
        PanelBackgroundColor = lightOrange;
    }
    $("#content-panel").css("background-color", PanelBackgroundColor)
}


function ValidateEndUnitPartNumber(endUnitPartNumber) {
    if (endUnitPartNumber != "") {
        var parseData = {
            "EndUnitPartNumber": endUnitPartNumber
        };
        return $.ajax({
            type: "POST",
            url: "/Ajax1/IsEndUnitNumberValid",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(parseData),
            success: function (data) {
                return data;
            },
            error: function (data) {
                alert("Some error occoured. Please try later");
            }
        });
    }
}

function ValidateItemPartNumber(itemPart, endUnit, employeeNumber) {
    if (itemPart != "" && endUnit != "") {
        var parseData = {
            "ItemPartNumber": itemPart,
            "EndUnitPartNumber": endUnit,
            "EmployeeNumber": employeeNumber
        };
        return $.ajax({
            type: "POST",
            url: "/Ajax1/IsItemPartNumberValid",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(parseData),
            success: function (data) {
                return data;
            },
            error: function (data) {
                alert("Some error occoured. Please try later");
            }
        });
    }

}

function Logout() {
    return $.ajax({
        type: "POST",
        url: "/Ajax1/Logout",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            return data;
        },
        error: function (data) {
            alert("Some error occoured. Please try later");
        }
    });
}

function ApproveOverride(EmpId, itemPart, endUnit) {

    var parseData = {
        "EmpId": EmpId,
        "ItemPartNumber": itemPart,
        "EndUnitPartNumber": endUnit
    };
    return $.ajax({
        type: "POST",
        url: "/Ajax/ApproveOverride",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(parseData),
        success: function (data) {
            return data;
        },
        error: function (data) {
            alert("Some error occoured. Please try later");
        }
    });
}

