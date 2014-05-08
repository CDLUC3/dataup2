$(function () {
 
    $("#Register").bind("click", function () {
        var _this = this;
        dataonBoardingData.showBusy();
        if (!dataonBoardingData.ValidateRegister()) {
            dataonBoardingData.hideBusy();
            return false;
        }

        var str = JSON.stringify(info);

        var authTicket = getCookie("x-api-jwt");
        $.ajax({
            url: $("#hdnBaseWebUserAPIUrl").val(),
            //url: "/Authenticate/Register",
            contentType: "application/json",
            xhrfields: { withCredentials: false },
            headers: { Authorization: authTicket },
            type: "POST",
            data: JSON.stringify(info),
            success: function (result) {
                alert("You have successfully registered to the application.");
                window.location = "../Home/index";
            },
            error: function (result) {
                if (result.status == "0") {
                    window.location = "../Home/index";
                }
                else {
                    alert("Error while registering the user");

                }
                dataonBoardingData.hideBusy();
            }
        });

    });

    $("#chkIsImpersonate").bind("click", function () {

        $.ajax({
            url: "http://dev-api-dataonboarding.cloudapp.net/api/windowsLiveAuthorization",
            //url: "/Authenticate/Register",
            contentType: "application/json",
            xhrfields: { withCredentials: false },
            headers: { Authorization: authTicket },
            type: "GET",
            data: JSON.stringify(info),
            success: function (result) {
                alert("You have successfully registered to the application.");
                window.location = "../Home/index";
            },
            error: function (result) {
                if (result.status == "0") {
                    window.location = "../Home/index";
                }
                else {
                    alert("Error while registering the user");

                }
                dataonBoardingData.hideBusy();
            }
        });

    });

});