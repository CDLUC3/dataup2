var landingPage = getSignoutURL();

$(function () {

    // Repository type change event
    $(document).on("change", "#ddlRepositoyType", function () {

        $("#credentialInputFields").hide();
        $("#urlBlock").hide();
        $("#spnReqUserName").hide();
        $("#spnReqPassword").hide();
        var baseRepId = $(this).find(":selected").val();

        var txtImpUserName = $('#txtImpUserName');
        var txtImpPassword = $('#txtImpPassword');
        var txtGetIdentifierURL = $('#txtGetIdentifierURL');
        var txtPostFileURL = $('#txtPostFileURL');

        txtImpUserName.val("");
        txtImpPassword.val("");
        txtGetIdentifierURL.val("");
        txtPostFileURL.val("");
        txtImpUserName.attr('disabled', 'disabled');
        txtImpPassword.attr('disabled', 'disabled');
        txtImpUserName.removeAttr('required');
        txtImpPassword.removeAttr('required');
        txtGetIdentifierURL.removeAttr('required');
        txtPostFileURL.removeAttr('required');
        txtImpUserName.next().remove();
        txtImpPassword.next().remove();
        txtGetIdentifierURL.next().remove();
        txtPostFileURL.next().remove();

        if (baseRepId == 1) {
            txtImpUserName.attr('required', true);
            txtImpPassword.attr('required', true);
            txtGetIdentifierURL.attr('required', true);
            txtPostFileURL.attr('required', true);

            $("#credentialInputFields").show();
            $("#urlBlock").show();

            if ($("#chkIsImpersonate").is(':checked')) {
                $("#spnReqUserName").show();
                $("#spnReqPassword").show();
                txtImpUserName.removeAttr('disabled');
                txtImpPassword.removeAttr('disabled');

            }
        }
        else if (baseRepId == 2) {
            if ($("#chkIsImpersonate").is(':checked')) {
                RedirectToWindowsLive();
            }
        }
    });

    // Is impersonate check box checked event
    $(document).on("change", "#chkIsImpersonate", function () {

        $("#spnReqUserName").hide();
        $("#spnReqPassword").hide();
        var txtImpUserName = $('#txtImpUserName');
        var txtImpPassword = $('#txtImpPassword');

        txtImpUserName.val("");
        txtImpPassword.val("");
        txtImpUserName.attr('disabled', 'disabled');
        txtImpPassword.attr('disabled', 'disabled');
        txtImpUserName.removeAttr('required');
        txtImpPassword.removeAttr('required');
        txtImpUserName.next().remove();
        txtImpPassword.next().remove();

        if ($("#ddlRepositoyType option:selected").val() == 1 && $(this).is(':checked')) {
            txtImpUserName.removeAttr('disabled');
            txtImpPassword.removeAttr('disabled');
            txtImpUserName.attr('required', true);
            txtImpPassword.attr('required', true);

            $("#spnReqUserName").show();
            $("#spnReqPassword").show();
        }
        else if ($("#ddlRepositoyType option:selected").val() == 2 && $(this).is(':checked')) {
            RedirectToWindowsLive();
        }

    });

    // Add new repository button click event
    $(document).on("click", "#btnAddRepository", function () {


        var formReference = $("#addEditRepository");
        if (validateRepositoryData(formReference)) {
            dataonBoardingData.showBusy();
            var data = formReference.serializeArray();
            data.push({ name: 'TokenExpiresOn', value: sessionStorage.getItem("tokenExpiresOn") });

            $.ajax({
                url: formReference.attr("postUrl"),
                data: data,
                type: "post",
                dataType: "json",
                success: function (json) {
                    // dataonBoardingData.hideBusy();

                    if (json.Status === "success") {
                        window.location.href = '/Repository/Index';
                    }
                    else if (json.Status == "redirect") {
                        var redirectLocation = json.Message;
                        window.location.href = redirectLocation;
                    }
                    else {
                        dataonBoardingData.hideBusy();
                        $("#errduplicateRepository").text(json.Message);
                        $("#errduplicateRepository").show();
                    }
                },
                error: function (req, timeout, errorMessage) {
                    dataonBoardingData.hideBusy();
                    $("#errduplicateRepository").text(errorMessage);
                    $("#errduplicateRepository").show();
                }
            });

            // }
        }
        else {
            return false;
        }
    });

    // Deleting the quality check rule
    $(document).on("click", "#ancDelRepository", function () {
        $('#divNoRep').hide();
        $('#divNoRepository').hide();

        $("#errRepositoryMessage").hide();

        if (confirm('Are you sure to delete the seleted repository?')) {
            dataonBoardingData.showBusy();
            jQuery.support.cors = true;
            var id = $(this).attr("repId");
            //var url = "/Repository/DeleteRepository";
            var url = $("#hdnBaseWebRepositoryAPIUrl").val()
            var authTicket = getCookie("x-api-jwt");

            if (authTicket == null || authTicket.length <= 0) {
                document.location = landingPage;
                return;
            }
            $.ajax({
                url: url + "?id=" + id,
                contentType: "application/json",
                xhrfields: { withCredentials: false },
                headers: { Authorization: authTicket },
                type: "DELETE",
                data:
                  {
                      repositoryId: id
                  },
                success: function (json) {
                    dataonBoardingData.hideBusy();

                    if (json == true || json.Status == true) {

                        $('table#tabRepositories tr#' + id + '').remove();
                        var rowCount = parseInt($("#hdnRepositoryCount").val());
                        rowCount = rowCount - 1;
                        $("#hdnRepositoryCount").val(rowCount);
                        if (rowCount < 1) {
                            $('#tabRepositories').hide();
                            $('#divNoRepository').removeClass('hide');
                            $('#divNoRepository').show();
                        }
                    }
                    else {
                        $("#errRepositoryMessage").text(json.Message);
                        $("#errRepositoryMessage").show();
                        //alert(json.Message);
                    }
                },
                error: function (x, y, z) {
                    dataonBoardingData.hideBusy();
                    $("#errRepositoryMessage").text("Error while deleting try after some time");
                    $("#errRepositoryMessage").show();
                }
            }
          );

        }
        else {
            dataonBoardingData.hideBusy();
            return false;
        }
    });
});

function getCookie(name) {
    var re = new RegExp(name + "=([^;]+)");
    var value = re.exec(document.cookie);
    //alert(value);
    return (value != null) ? unescape(value[1]) : null;
}

// Method to validate the repository data
function validateRepositoryData(formReference) {

    // regular expression for URL
    var urlregex = new RegExp("^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$");

    var isValid = true;

    var validator = formReference.validate();
    if (!formReference.valid()) {
        validator.showErrors();
        isValid = false;
    }

    //validate the metadata 
    var isMetaDataValid = ValidateAdminMetaData();
    var metaDataValid = (isValid && isMetaDataValid);

    return metaDataValid;
}

// TODO: Needs to be check for the cross domin issue.
function checkForRepositoryName() {

    var isValid = false;

    var repId = $("#hdnRepositoryId").val();
    var repName = $("#txtRepName").val();
    var url = $("#hdnBaseWebRepositoryAPIUrl").val();
    url = url + "?repositoryName=" + repName;

    $.ajax({
        url: url,
        type: "get",
        success: function (result) {
            dataonBoardingData.hideBusy();

            if (result == 0 || result == repId) {
                isValid = true;
            }
        },
        error: function (req, timeout, errorMessage) {
            dataonBoardingData.hideBusy();
            $("#errduplicateRepository").text(errorMessage);
            $("#errduplicateRepository").show();
        }
    });


    return isValid;
}

//------------------functions related to admin metadata-------------------
function ValidateAdminMetaData() {
    $(".metaDataMsgHolder").hide();
    var elem = $(".paramtrTableContainer tr");
    var isValid = true;
    var itemArray = new Array();
    if (elem != null) {
        // add the condition to make the metadata should be mandatory
        var elementsLength = $(elem).length;
        // check row by row and do the validation
        for (var index = 0; index < elementsLength; index++) {
            var row = $(elem).get(index);
            //for blank row this should not be validated
            if (row != null && $(row).find(".IsBlankRow").val() != "True") {
                var repField = $(row).find('.repField').val();
                var repDescription = $(row).find('.repDescription').val();
                var type = $(row).find('.typeSelectList');
                var typeValue = type.find(":selected").val();
                var rangeValue = $(row).find('.repRange').val();
                var mappingItem = $(row).find('.repMapping').val();

                //If type is range, make sure unit is there
                if (typeValue === "4" && rangeValue.length === 0) {
                    isValid = false;
                    $(row).find('.repRange').attr('required', true);
                }
                else {
                    var rangeValidationResult = ValidateRangeMetaData(rangeValue);

                    if (rangeValidationResult == "0") {
                        $(row).find('.repRange').removeAttr("required");
                    }
                    else if (rangeValidationResult == "1") {
                        isValid = false;
                        $(row).find('.repRange').attr('required', true);
                        $(".metaDataMsgHolder").show();
                        $("#metaDataMsg").text("Invalid range data, range should be of format NtoN, ex : 10to20");

                    }
                    else if (rangeValidationResult == "2") {
                        isValid = false;
                        $(row).find('.repRange').attr('required', true);
                        $(".metaDataMsgHolder").show();
                        $("#metaDataMsg").text("Invalid range data, range should be of format NtoN, ex : 10to20");
                    }
                    else if (rangeValidationResult == "3") {
                        isValid = false;
                        $(row).find('.repRange').attr('required', true);
                        $(".metaDataMsgHolder").show();
                        $("#metaDataMsg").text("Invalid range data, Start value should be less than End value");
                    }
                }

                // Check the combination of unique field and mapping
                if (repField.length > 0) {
                    var currentFieldAndMappingItem = "";
                    if (mappingItem.length > 0) {
                        currentFieldAndMappingItem = mappingItem.toLowerCase() + ":" + repField.toLowerCase();
                    }
                    else {
                        currentFieldAndMappingItem = repField.toLowerCase();
                    }
                    if (itemArray.indexOf(currentFieldAndMappingItem) < 0) {
                        itemArray.push(currentFieldAndMappingItem);
                        $(row).find('.repRange').removeAttr("required");
                        $(row).find('.repMapping').removeAttr("required");
                    }
                    else {
                        isValid = false;
                        $(".metaDataMsgHolder").show();
                        $("#metaDataMsg").text("Duplicate Field data,combination of Field and mapping should be unique.");
                    }
                }
            }
        }
    }
    return isValid;
}

function ValidateRangeMetaData(rangeValue) {
    if (rangeValue != null && rangeValue.length > 0) {//if range value is not valid (ex : 10-10)show the valid message
        if (rangeValue.indexOf("to") != "-1") {
            var rangeParts = rangeValue.split('to');
            if (rangeParts.length != 2) {
                return "2"; //Invalid range string
            }
            else if (!parseFloat(rangeParts[0]) || !parseFloat(rangeParts[1])) {
                return "2"; //Invalid range string
            }
            else if (parseInt(rangeParts[0]) >= parseFloat(rangeParts[1])) {
                return "3";//range part 1 should be lesser than second part
            }
        }
        else {
            return "2"; //Invalid range string
        }

    }
    return "0"; //valid range string
}

function ClearAdminMetaDataParameterRow(row, index) {
    $(row).find('.repMapping').val('');
    $(row).find('.repField').val('');
    $(row).find('.repDescription').val('');
    $(row).find('.repRange').val('');
    $(row).find('.RepositoryMetaDataFieldId').val('');

    // Reset the admin metadata controls
    ResetAdminMetaDataControls(row, index);
}

// method to reset the controls index on delete
function ResetMetaDataColumns() {
    var elem = $(".paramtrTableContainer tr");
    var elementsLength = $(elem).length;
    for (var index = 0; index < elementsLength; index++) {
        var row = $(elem).get(index);
        if (row != null) {
            //set the values
            ResetAdminMetaDataControls(row, index);
        }
    }
}

function ResetAdminMetaDataControls(row, index) {
    $(row).find('.repMapping').attr("id", "RepositoryMetaDataFieldList_" + index + "__MappedLocation");
    $(row).find('.repMapping').attr("name", "RepositoryMetaDataFieldList[" + index + "].MappedLocation");

    $(row).find('.repField').attr("id", "RepositoryMetaDataFieldList_" + index + "__Field");
    $(row).find('.repField').attr("name", "RepositoryMetaDataFieldList[" + index + "].Field");

    $(row).find('.repDescription').attr("id", "RepositoryMetaDataFieldList_" + index + "__Description");
    $(row).find('.repDescription').attr("name", "RepositoryMetaDataFieldList[" + index + "].Description");

    $(row).find('.repIsRequired').attr("id", "RepositoryMetaDataFieldList_" + index + "__IsRequired");
    $(row).find('.repIsRequired').attr("name", "RepositoryMetaDataFieldList[" + index + "].IsRequired");

    $(row).find('.typeSelectList').attr("id", "RepositoryMetaDataFieldList_" + index + "__Types");
    $(row).find('.typeSelectList').attr("name", "RepositoryMetaDataFieldList[" + index + "].MetaDataTypeId");

    $(row).find('.typeText').attr("id", "RepositoryMetaDataFieldList_" + index + "__MetaDataTypeId");
    $(row).find('.typeText').attr("name", "RepositoryMetaDataFieldList[" + index + "].MetaDataTypeId");

    $(row).find('.repRange').attr("id", "RepositoryMetaDataFieldList_" + index + "__RangeValues");
    $(row).find('.repRange').attr("name", "RepositoryMetaDataFieldList[" + index + "].RangeValues");

    $(row).find('.RepositoryMetaDataFieldId').attr("id", "RepositoryMetaDataFieldList_" + index + "__RepositoryMetaDataFieldId");
    $(row).find('.RepositoryMetaDataFieldId').attr("name", "RepositoryMetaDataFieldList[" + index + "].RepositoryMetaDataFieldId");

    $(row).find('.metadataNodeName').attr("id", "RepositoryMetaDataFieldList_" + index + "__MetadataNodeName");
    $(row).find('.metadataNodeName').attr("name", "RepositoryMetaDataFieldList[" + index + "].MetadataNodeName");

    $(row).find('.RowType').attr("id", "RepositoryMetaDataFieldList_" + index + "__RowType");
    $(row).find('.RowType').attr("name", "RepositoryMetaDataFieldList[" + index + "].RowType");

    $(row).find('.IsBlankRow').attr("id", "RepositoryMetaDataFieldList_" + index + "__IsBlankRow");
    $(row).find('.IsBlankRow').attr("name", "RepositoryMetaDataFieldList[" + index + "].IsBlankRow");

    $(row).find('.metaDataId').attr("id", "RepositoryMetaDataFieldList_" + index + "__MetaDataId");
    $(row).find('.metaDataId').attr("name", "RepositoryMetaDataFieldList[" + index + "].MetaDataId");
}

function SetCurrentRepositoryDataForDeletion() {
    var elem = $(".paramtrTableContainer tr");
    for (var index = 0; index < $(elem).length; index++) {
        var row = $(elem).get(index);
        if (row != null) {
            var deletedMetaDataField = $("#DeletedMetaDataFieldIds");
            var repositoryMetaDataFieldId = $(row).find('.RepositoryMetaDataFieldId');
            var repositoryMetaDataFieldValue = repositoryMetaDataFieldId.val();
            if (repositoryMetaDataFieldValue != 0 && repositoryMetaDataFieldValue.length > 0) {
                var currentDeletedMetaDataFieldValues = deletedMetaDataField.val();
                if (currentDeletedMetaDataFieldValues.length > 0) {
                    currentDeletedMetaDataFieldValues = currentDeletedMetaDataFieldValues + repositoryMetaDataFieldValue + ",";
                }
                else {
                    currentDeletedMetaDataFieldValues = repositoryMetaDataFieldValue + ","
                }
                deletedMetaDataField.val(currentDeletedMetaDataFieldValues);
            }
        }
    }
}

// Helper function to get retrieve the queryparam value by name
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

// onload event for the page ManageRepsoitory
// checks if the query parameter contains access token and refresh token
function ManageRepository_Onload() {

    // hide the crendential fields if its SkyDrive
    var baseRepId = $("#ddlRepositoyType").find(":selected").val();
    if (baseRepId == 2) {
        $("#credentialInputFields").hide();
    }

    if (Modernizr.localstorage) {

        var accessToken = getParameterByName("accessToken");

        // if access token exists indicates it is redirection from windows live
        if (accessToken.length > 0) {

            var data = sessionStorage.getItem("data"),
                dom = sessionStorage.getItem("dom");

            if (data != null) {
                PopulateForm(dom, data);
            };

            $("#accessToken").val(accessToken);
            $("#refreshToken").val(getParameterByName("refreshToken"));
            $("#tokenExpiresOn").val(getParameterByName("tokenExpiresOn"));
        }

    } else {

        alert("Session Storage is not Avaialable. Please use a browser that supports Session Storage");
    }
}

// Redirect the browser to API to Authenticate and get the windows live tokens
function RedirectToWindowsLive() {

    var formReference = $("#addEditRepository"),
        data = serializeFormData(),
        dom = document.getElementById("body").innerHTML,
        accessToken = $("#accessToken").val();

    sessionStorage.setItem("dom", dom);
    sessionStorage.setItem("data", data);

    // check if the access token exists or not if not then redirect the browser to get the auth tokens
    if (accessToken.length == 0) {
        // appned the url with callback param
        var liveLoginUrl = $("#hdnBaseWebApiWindowsLiveAuthPathUrl").val() + "?callBackUrl=" + document.URL;
        document.location = liveLoginUrl;
    }
}

// serializes the form data
function serializeFormData() {

    var $inputs = $('#addEditRepository :input'),
        values = [];

    //Loop through all inputs elments.
    $inputs.each(function () {
        var elem = $("#" + this.id);

        if (elem.length > 0) {
            var inputType = elem.attr("type");

            // special case for check box
            switch (inputType) {
                case "checkbox":
                default:
                    values.push({
                        id: this.id,
                        value: $(this).val()
                    });
                    break;
            }
        }
    });

    return JSON.stringify(values);
}

function PopulateForm(dom, data) {

    var data = JSON.parse(data);
    document.getElementById("body").innerHTML = dom;

    //Loop through items
    for (var i = 0; i < data.length; i++) {
        var elem = $("#" + data[i].id);
        if (elem.length > 0) {
            var inputType = elem.attr("type");
            switch (inputType) {
                case "checkbox":
                    var checkBoxValue = data[i].value
                    if (checkBoxValue.toLowerCase() === "true") {
                        elem.attr('checked', true);
                    } else {
                        elem.attr('checked', false);
                    }
                    break;
                default:
                    elem.val(data[i].value);
                    break;
            }
        }
    }
}
