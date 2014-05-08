$(function () {
    var homeUrl = document.location.protocol + "//" + document.location.host + "/Home/index",
        windowsLive = "Windows Live™ ID",
        facebook = "Facebook",
        google = "Google";

    if (document.cookie.indexOf("__TimezoneOffset") < 0) {
        setTimezoneCookie();
    }

    getSupportedProviders();
    function getSupportedProviders() {

        // Get the list of Providers and attach the URLs to login buttons
        $.ajax({
            url: $("#hdnGetSupportedIdentityProvidersPath").val() + "?callback=" + encodeURIComponent(homeUrl),
            async: true,
            xhrfields: { withCredentials: false },
            type: "get",
            headers: { 'Accept': 'application/json' },
            success: function (response) {
                attachLinksToLoginButtons(response);
            }
        });
    }

    // This function is a callback for getSupportedProviders and is responsible for attaching the Login URLs to windows Live and facebook login buttons.
    function attachLinksToLoginButtons(json) {
        var identityProviders = [];


        identityProviders = JSON.parse(json);

        // Loop through the identity providers
        for (var i in identityProviders) {
            var identityProviderName = identityProviders[i].Name,
                loginUrl = identityProviders[i].LoginUrl;

            if (identityProviderName === windowsLive) {

                $(document).on("click", "#msLogin", { url: loginUrl }, function (event) {
                    document.location = event.data.url;
                });
            }
            else if (identityProviderName === facebook) {

                $(document).on("click", "#fbLogin", { url: loginUrl }, function (event) {
                    document.location = event.data.url
                });
            }
            else if (identityProviderName === google) {

                $(document).on("click", "#googleLogin", { url: loginUrl }, function (event) {
                    document.location = event.data.url
                });
            }
        }

        $("#inProgress").hide("slow");
        $("#loginButtons").show("slow");
    }

    function setTimezoneCookie() {
        var timezone_cookie = "__TimezoneOffset";
        var exdate = new Date();
        exdate.setDate(exdate.getDate() + 1);
        document.cookie = timezone_cookie + "=" + escape(new Date().getTimezoneOffset()) + ";expires=" + exdate.toGMTString() + ";path=/";
    }
});

