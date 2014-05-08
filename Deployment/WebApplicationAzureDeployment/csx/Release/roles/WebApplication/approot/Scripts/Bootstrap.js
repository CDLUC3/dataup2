var dataupConfigurationKey = "dataupConfigurationKey";
var configuration;
var genericErrorMessage = "Unknown error while doing the current operation";

// Retreives the Config Settings for the application
function getConfigSettings() {
    var dataupConfiguration = sessionStorage.getItem(dataupConfigurationKey);
    if (dataupConfiguration == "undefined" || dataupConfiguration == null) {
        showBusy();
        $.ajax({
            url: "/Home/GetConfiguration",
            xhrfields: { withCredentials: false },
            type: "GET",
            contentType: "application/json",
            async: false
        })
        .done(function (data) {
            configuration = data;
            sessionStorage.setItem(dataupConfigurationKey, JSON.stringify(configuration));
            return configuration;
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            showErrorMessage(jqXHR.status, genericErrorMessage);
        })
        .always(function (jqXHR, textStatus, errorThrown) {
           hideBusy();
        });
    }
    else {
        configuration = $.parseJSON(dataupConfiguration);
        return configuration;  
    }
}

function loadHomeView() {
    var stateId = getParameterByName("stateId"),
        fileListActionState = undefined;

    if (stateId.length > 0) {
        fileListActionState = getState(stateId) || {};
        fileListActionState.repositoryCredentials = { RefreshToken: getParameterByName("refreshToken"), AccessToken: getParameterByName("accessToken"), TokenExpiresOn: getParameterByName("tokenExpiresOn") };
        deleteState(stateId);
    }

    dataonBoardingData.showBusy(); 

    $.ajax({
        url: "/Home/Home",
        xhrfields: { withCredentials: false },
        type: "GET",
        contentType: "application/html",
        dataType: 'html'
    })
    .done(function (data) {

        dataonBoardingData.hideBusy();
        $('#body').html(data);

        var authTicket = getCookie("x-api-jwt");
        if (authTicket == null || authTicket.length <= 0) {
            var landingPage = getSignoutURL();
            document.location = landingPage;
            return;
        }

        initializeFileListWidget(authTicket, fileListActionState);
        initializeFileUploadWidget(authTicket);
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        dataonBoardingData.hideBusy();
        showErrorMessage(jqXHR.status, genericErrorMessage);
    })
};

function initializeFileListWidget(authTicket, fileListActionState) {
    var fileList = $('#fileListContainer').filelist({
        jwtAuthenticationToken: authTicket,
        configuration: configuration,
        state: fileListActionState,
        propagateMessages: true,
        fileSelected: function (event, data) {
            alert(data.status);
        },
        getCredentials: function (event, data) {
            dataonBoardingData.hideBusy();
            getCredentialEventHandler(data);
        },
        postFile: function (event, data) {
            postFileHandler(data.fileId, data.repositoryId, data.repositoryName);
        },
        operationStart: function (event, operation) {
            resetMessageBar();
            dataonBoardingData.showBusy();
        },
        operationCompleted: function (event, data) {
            resetMessageBar();
            if (data.actionName == "deleteFile") {
                initializeFileUploadWidget(authTicket);
            }

            dataonBoardingData.hideBusy();
        },
        operationFailed: function (event, error) {
            dataonBoardingData.hideBusy();
            showErrorMessage(error.status, error.message);
        }
    });
}
function initializeFileUploadWidget(authTicket, existingFiles) {

    $('#fileUpload').fileupload({
        jwtAuthenticationToken: authTicket,
        configuration: configuration,
        existingFilesList : existingFiles || [],
        propagateMessages: true,
        uploadStart: function (event, data) {
            dataonBoardingData.showBusy();
        },
        uploadSuccessful: function (event, data) {
            resetMessageBar();
            var fileListContainer = $('#fileListContainer').filelist({ filterValue: 1 });
            initializeFileUploadWidget(authTicket);
        },
        uploadFailed: function (event, data) {
            dataonBoardingData.hideBusy();
            showErrorMessage(data.status, data.message);
        },
        uploadAborted: function (event, data) {
            dataonBoardingData.hideBusy();
        }
    });
}

//call the methods that need to be initialized
function initializeBootstrap() {

    $.ajaxSetup({
        cache: false
    });

    getConfigSettings();
    if (document.location.pathname.toLowerCase() == "/home/index") {
        loadHomeView();
    }
}

function postFileHandler(fileId, repositoryId, repository) {
    dataonBoardingData.showBusy();
    var authTicket = getCookie("x-api-jwt");
    if (authTicket == null || authTicket.length <= 0) {
        var landingPage = getSignoutURL();
        document.location = landingPage;
        return;
    }

    $.ajax({
        url: "/Home/Post?fileId=" + fileId + "&repositoryId=" + repositoryId,
        xhrfields: { withCredentials: false },
        type: "GET",
        contentType: "application/json"
    })
    .done(function (data) {
        $('#body').html(data);
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        document.location = getSignoutURL();
    })
    .always(function (jqXHR, textStatus, errorThrown) {
        dataonBoardingData.hideBusy();
    });
}

function saveState(state) {
    var stateId = Math.floor(1 + Math.random());
    sessionStorage.setItem(stateId, JSON.stringify(state));
    return stateId;
}

function getState(stateId) {
    var state = sessionStorage.getItem(stateId);
    return JSON.parse(state);
}

function deleteState(stateId) {
    sessionStorage.removeItem(stateId);
}

$(function () {
    initializeBootstrap();
});

function showCredPopup(data) {
    var credPopupContainer = $(document.getElementById("divCredPopup")),
        state = data.state || {};
       
    credPopupContainer.basicauthenticationpopup({
        actionName: "Download",
        credentialsEntered: function (event, data) {
            credPopupContainer.hide();
            state.repositoryCredentials = { UserName: data.userName, Password: data.password };
            $('#fileListContainer').filelist("ApplyState", state);
        }
    });

    credPopupContainer.show();
}

function getCredentialEventHandler(data) {
    if (data.baseRepositoryId == 1) {
        if (data.state.actionName == "postFile") {
            return;
        }
        showCredPopup(data);

    } else if (data.baseRepositoryId == 2) {
        
        var stateId = saveState(data.state);
        var filePostUrl = document.location.protocol + "//" + document.location.host + document.location.pathname + "?stateId=" + stateId
        var liveLoginUrl = configuration.WindowsLiveAuthUri + "?callBackUrl=" + encodeURIComponent(filePostUrl);
        document.location = liveLoginUrl;
    }
}

function showErrorMessage(status, message) {

    // The code also checks for status=0 when the browser is IE10.0
    if (checkStatusAndRedirectToLandingPage(status)) return;

    $("#messagePannel").removeClass("success");
    $("#messagePannel").addClass("error");
    $("#messagePannel").text(message);
    $(".filelist .action-bar").css("top", "76px");
    $("#fileListWrapper").css("padding-top", "40px");
    $("#messagePannel").show();
}

function resetMessageBar() {
    $(".filelist .action-bar").css("top", "46px");
    $("#fileListWrapper").css("padding-top", "40px");
    $("#messagePannel").hide();
}

function showSuccessMessage(message) {
    $("#messagePannel").removeClass("error");
    $("#messagePannel").addClass("success");
    $("#messagePannel").text(message);
    $(".filelist .action-bar").css("top", "76px");
    $("#fileListWrapper").css("padding-top", "40px");

    $("#messagePannel").show();
}

function getSignoutURL() {
    getConfigSettings()
    return configuration.SignOutCallbackUri + "?callback=" + encodeURIComponent(document.location.protocol.concat("//", document.location.host));
}

function checkStatusAndRedirectToLandingPage(status) {
    // The code also checks for status=0 when the browser is IE10.0
    if (status == 401) {
        document.location = getSignoutURL();
        return true;
    } else if (status == 0 && getInternetExplorerVersion() >= 10.0) {
        document.location = getSignoutURL();
        return true;
    }
}

// This displays the load indicator
function showBusy() {
    $("body").addClass("busy");
    $("body").click(function (e) {
        e.preventDefault()
        e.stopImmediatePropagation();
    });
};

// This displays the load indicator
function hideBusy() {
    $("body").removeClass("busy");
    $("body").unbind();
};