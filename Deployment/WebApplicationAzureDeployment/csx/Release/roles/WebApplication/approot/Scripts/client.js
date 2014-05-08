var landingPage = getSignoutURL();

$(function () {

    dataonBoardingData.initialize();

    // Delete file button click event on uploaded file list
    $(document).on('click', '#delDocument', function () {
        $('#divNoFiles').hide();

        if (confirm('Are you sure to delete the file?')) {
            dataonBoardingData.showBusy();

            var info = {
                fileId: $(this).attr("fileId"),
                userId: $("#hdnUserId").val()
            };
            var id = $(this).attr("fileId");
            //var url = "/Home/DeleteFile";
            var authTicket = getCookie("x-api-jwt");

            if (authTicket == null || authTicket.length <= 0) {
                document.location = landingPage;
                return;
            }

            $.ajax({
                url: $("#hdnBaseWebFileAPIUrl").val() + "?fileId=" + info.fileId + "&userId=" + info.userId,
                contentType: "application/json",
                xhrfields: { withCredentials: false },
                headers: { Authorization: authTicket },
                type: "DELETE",
                data: JSON.stringify(info),

                success: function (data) {
                    dataonBoardingData.hideBusy();
                    if (data == true || data.Status == true) {
                        $('table#uploadFileTable tr[fileId="' + id + '"]').remove();
                        var numberOfUploadedFiles = $('#uploadFileTable tbody tr').length;
                        if (numberOfUploadedFiles < 1) {
                            $('#uploadFileTable').hide();
                            $('#divNoFiles').removeClass('hide');
                            $('#divNoFiles').show();
                        }

                        $('table#filesDeletionAlertTable tr[fileId="' + id + '"]').remove();
                        var numberOfFilesToBeDeleted = $('#filesDeletionAlertTable tbody tr').length;
                        if (numberOfFilesToBeDeleted < 1) {
                            $('#btnUploadedFiles').addClass('active');
                            $('#divUploadedFiles').show();

                            $('#btnFilesDeletionAlert').remove();
                            $('#divfilesDeletionAlert').remove();
                        }
                        else {
                            $('#btnFilesDeletionAlert a span').html(numberOfFilesToBeDeleted);
                        }
                    }
                    else {
                        alert("Error while deleting try after some time.");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    dataonBoardingData.hideBusy();
                    if (jqXHR.status == 404) {
                        var responseText = $.parseJSON(jqXHR.responseText);
                        alert(responseText.Message);
                    }
                    else {
                        alert("Error while deleting try after some time.");
                    }
                }
            }
          );

        }
        else {
            dataonBoardingData.hideBusy();
            return false;
        }
    });

    // Uploaded file list tab click event
    $(document).on('click', '#btnUploadedFiles', function () {

        $(this).addClass('active');

        if ($('#btnPostedFiles').hasClass('active')) {
            $('#btnPostedFiles').removeClass('active');
        }

        if ($('#btnFilesDeletionAlert').hasClass('active')) {
            $('#btnFilesDeletionAlert').removeClass('active');
        }

        $('#divUploadedFiles').show();
        $('#divPostedFiles').hide();
        $('#divfilesDeletionAlert').hide();
    });

    // Published file list tab click event
    $(document).on('click', '#btnPostedFiles', function () {

        $(this).addClass('active');

        if ($('#btnUploadedFiles').hasClass('active')) {
            $('#btnUploadedFiles').removeClass('active');
        }

        if ($('#btnFilesDeletionAlert').hasClass('active')) {
            $('#btnFilesDeletionAlert').removeClass('active');
        }

        $('#divUploadedFiles').hide();
        $('#divPostedFiles').show();
        $('#divfilesDeletionAlert').hide();
    });

    // Published file list tab click event
    $(document).on('click', '#btnFilesDeletionAlert', function () {

        $(this).addClass('active');

        if ($('#btnUploadedFiles').hasClass('active')) {
            $('#btnUploadedFiles').removeClass('active');
        }

        if ($('#btnPostedFiles').hasClass('active')) {
            $('#btnPostedFiles').removeClass('active');
        }

        $('#divUploadedFiles').hide();
        $('#divPostedFiles').hide();
        $('#divfilesDeletionAlert').show();
    });

    // Showing repository list popup in uploaded file list page
    $(document).on("click", "#btnPostFile", function () {

        // returns height of browser viewport

        var windowHeight = parseInt($(window).height());

        // var docHeight = parseInt($(document).height());
        //scrollTop()

        var topOffset = parseInt($(this).offset().top) - parseInt($(window).scrollTop());

        var actualOffset = parseInt($(this).offset().top);

        var popupHeight = parseInt($(this).children("ul").height());

        var marginHeight = popupHeight + 50;

        $('#ancPost').css('background-color', '#1572b9');

        $('ul#ulRepositories').hide();

        if (topOffset + marginHeight >= windowHeight) {
            //$(this).children("ul").css('left', 100);
            $(this).children("ul").css('top', actualOffset - marginHeight);
        }

        $(this).children("ul").show(100).fadeIn(300);
    });

    // Event to handle mouse click on the page
    $(document).click(function () {

        if ($('ul#ulRepositories').is(':visible')) {
            $('ul#ulRepositories').hide();
            $('#ancPost').css('background-color', '');
        }
        if ($('#changeRepositories').is(':visible')) {
            $('#changeRepositories').hide();
        }
    });

    $(document).on("keyup focus keydown blur paste", "#qcRuleName", function (event) {
        $("#errduplicateRule").hide();
    });

    $(document).on("keyup focus keydown blur paste", ".txtHeaderName", function (event) {
        $("#duplicateHeaderError").hide();
    });

    $(document).on("keyup focus keydown blur paste", ".txtRangeStart, .txtRangeEnd", function (event) {
        $("#rangeEndSmallErr").hide();
        $("#invalidRangeValues").hide();
    });

    // Close button click on credintials popup
    $(document).on("click", "#closeCredPopup", function () {
        CloseCredentialsDiv()
    });

    // Cancel button click on credintials popup
    $(document).on("click", "#cancelCredPopup", function () {
        CloseCredentialsDiv();
    });

    function CloseCredentialsDiv() {
        $("#divCredPopup").hide();
        $("#errUserReq").hide();
        $("#errAuthentication").hide();
        $("#errPwdReq").hide();
    }
    // Text change events handling to hide the error message
    $(document).on("keyup focus keydown blur paste", "#txtUserName", function (event) {
        $("#errUserReq").hide();
        $("#errAuthentication").hide();
    });

    // Text change events handling to hide the error message
    $(document).on("keyup focus keydown blur paste", "#txtPassword", function (event) {
        $("#errPwdReq").hide();
        $("#errAuthentication").hide();
    });


    //// Method to handle the moving up and down
    $(document).on("click", ".up,.down", function () {

        var row = $(this).parents("tr:first");
        var rowIndex = parseInt(row.attr('rownumber'));
        var rowCount = parseInt($("#tbodyHeaders tr").length);

        if ($(this).is(".up")) {
            if (rowIndex > 1) {
                var d = rowIndex - 2;
                $('#tbodyHeaders').find('tr').eq(rowIndex - 2).before(row);
            }
            else {
                return false;
            }
        } else {
            if (rowIndex < rowCount) {
                $('#tbodyHeaders').find('tr').eq(rowIndex).after(row);
            }
            else {
                return false;
            }
        }
        dataonBoardingData.setHeadersOrder();
    });

    // Method to add the new row to add new header data
    $("#ancAddHeader").bind("click", function () {

        //$('#tabHeaders > tbody:last').clone(true).insertAfter('#tabHeaders > tbody:last');
        var clonedRow = $('#tbodyHeaders tr:last').clone(true)
        clonedRow.find('label.error').remove()
        clonedRow.insertAfter('#tbodyHeaders tr:last');

        var lastAddedRow = $("#tbodyHeaders tr:last");
        var index = $("#tbodyHeaders tr").length - 1;
        clearAddedHeaderValues(lastAddedRow, index);
    });

    //Method to delete the header row
    $(document).on("click", "#ancDeleteHeader", function () {

        var rowCount = $("#tbodyHeaders tr").length;
        var id = $(this).attr("rowNumber");
        if (rowCount > 1) {
            $("#tbodyHeaders tr[rowNumber=" + id + "]").remove();

            dataonBoardingData.setHeadersOrder();
        }
        else {
            return false;
        }
    });

    // Deleting the quality check rule
    $(document).on("click", "#ancDelRule", function () {

        $('#divNoRules').hide();
        $("#errRuleMessage").hide();

        if (confirm('Are you sure to delete the quality check rule?')) {
            dataonBoardingData.showBusy();
            jQuery.support.cors = true;
            var id = $(this).attr("qcId");
            var url = $("#hdnBaseWebQCAPIUrl").val()
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
                      qualityCheckId: id
                  },
                success: function (json) {
                    dataonBoardingData.hideBusy();

                    if (json == true) {

                        $('table#tabQCRules tr#' + id + '').remove();
                        var rowCount = parseInt($("#hdnRuleCount").val());
                        rowCount = rowCount - 1;
                        $("#hdnRuleCount").val(rowCount);
                        if (rowCount < 1) {
                            $('#tabQCRules').hide();
                            $('#divNoRules').removeClass('hide');
                            $('#divNoRules').show();
                        }
                    }
                    else {
                        $("#errRuleMessage").text(json.Message);
                        $("#errRuleMessage").show();
                        //alert(json.Message);
                    }
                },
                error: function (x, y, z) {
                    dataonBoardingData.hideBusy();
                    $("#errRuleMessage").text("Error while deleting try after some time");
                    $("#errRuleMessage").show();

                }
            }
          );

        }
        else {
            dataonBoardingData.hideBusy();
            return false;
        }
    });

    // Add new quality check rule button click event
    $(document).on("click", "#btnAddQcRule", function () {

        var formReference = $("#addEditQCRule");

        if (dataonBoardingData.validateQCRuleData(formReference)) {

            dataonBoardingData.showBusy();
            var data = formReference.serializeArray();
            $.ajax({
                url: formReference.attr("postUrl"),
                data: data,
                type: "post",
                success: function (json) {
                    dataonBoardingData.hideBusy();
                    // sequimData.hideInfoHeader();
                    //  dataonBoardingData.persistAgreement($("#postRepositories option:selected").text(), $("#postUserName").val());

                    if (json.Status === true) {
                        // alert(json.Message);
                        //$("#filePostSuccessPopup .identifier").text(json.Message);
                        //$("#filePostSuccessPopup").show();
                        window.location.href = '/QualityCheck/Index';
                    }
                    else {
                        $("#errduplicateRule").text(json.Message);
                        $("#errduplicateRule").show();
                        //alert(json.Message);
                    }
                },
                error: function (req, timeout, errorMessage) {
                    dataonBoardingData.hideBusy();
                    $("#errduplicateRule").text("Error while saving quality check rule data. Retry.");
                    $("#errduplicateRule").show();
                }
            });
        }
        else {
            return false;
        }
    });

    // Rule name, rule description and error message validation for alphanumeric with space 
    $('.txtRangeStart, .txtRangeEnd').keyup(function (e) {
        var key = String.fromCharCode(e.which);
        var txt = $(this).val();

        var RE = /^-?\d*\.?\d*$/;
        if (RE.test(txt)) {
            return true;
        }
        else {
            $(this).val(txt.substr(0, 1) + txt.substr(1, txt.length).replace('-', ''));
        }
        e.preventDefault();
        return true;
    });


    // Header column type changed event.
    $(document).on("change", ".ddlColumnType", function () {

        var qcruleId = $(this).attr('ruleId');

        //   $('.actions').removeAttr('colspan');

        if ($(this).find(":selected").text() == 'Numaric' || $(this).find(":selected").text() == 'Numeric') {
            $(this).closest('tr').find('.range').removeClass('hide');


        } else {
            if (!$(this).closest('tr').find('.range').hasClass('hide')) {
                $(this).closest('tr').find('.range').addClass('hide');
            }
        }

    });

    // Rule name, rule description and error message validation for alphanumeric with space 
    $('#qcRuleName,#qcRuleDesc,.txtErrorMessage').keypress(function (e) {
        var key = String.fromCharCode(e.which);

        var regex = new RegExp("^[a-zA-Z0-9 ]+$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);

        var txt = $(this).val() + key;

        if (regex.test(txt)) {
            return true;
        }

        e.preventDefault();
        return false;

    });

    // Sheet list drop down click event
    $("#ulSheetList").bind("click", function () {
        $("#errorValidateSheets").hide();
        $("#errorNoSheetSelected").hide();
    });

    var fileId,
        repositoryId;

    // Download button click on published file list
    $(document).on("click", "#liDownload", function () {
        $("#errUserReqDownload").hide();
        $("#errPwdReqDownload").hide();
        $("#errDownLoad").hide();

        var baseRepId = $(this).attr('baseRepositoryId'),
            isImp = $(this).attr('isImpersonating');
        repositoryId = $(this).attr('repositoryId');
        fileId = $(this).attr('fileId');

        if (baseRepId == 1) {
            if (isImp == "True") {
                $("#fileDownloadFrame").contents().find('body').html("");
                url = "/Home/DownloadFileFromRepository?fileId=" + fileId + "&userName=" + "&password=";
                $("#fileDownloadFrame").attr("src", url);
            }
            else {
                $("#txtUserToDownLoad").val("");
                $("#pwdToDownLoad").val("");
                fileId = $(this).attr('fileId');
                $("#divCredPopup").show();
                return false;
            }
        }
        else {

            CheckUserAuthTokenStatusForDownload(fileId, repositoryId);
        }
        return false;

    });

    // Text change events handling to hide the error message
    $(document).on("keyup focus keydown blur paste", "#txtUserToDownLoad", function (event) {
        $("#errUserReqDownload").hide();
    });

    // Text change events handling to hide the error message
    $(document).on("keyup focus keydown blur paste", "#pwdToDownLoad", function (event) {
        $("#errPwdReqDownload").hide();
    });
});

// Helper function to get remove the duplicates from the string array
function getUniquestrArray(list) {
    var result = [];
    $.each(list, function (i, e) {
        var item = e.toLowerCase();
        if ($.inArray(item, result) == -1) result.push(item);
    });
    return result;
}


function clearAddedHeaderValues(row, index) {
    $(row).attr("rowNumber", index + 1);
    $(row).attr("id", index + 1);
    $(row).find('#ancDeleteHeader').attr("rowNumber", index + 1);
    $(row).find('.txtHeaderName').val('');
    $(row).find('.chkIsRequired').prop('checked', false);
    $(row).find('.txtErrorMessage').val('');
    $(row).find('.hdnColumnRuleId').val('0');
    $(row).find('.ddlColumnType').val('0');
    $(row).find('.hdnOrder').val(index + 1);
    $(row).find('.txtRangeStart').val('');
    $(row).find('.txtRangeEnd').val('');

    (row).find('.range').addClass('hide');

    //ddlColumnType
    //QCColumnRuleId", @class = "hdnColumnRuleId

    $(row).find('.txtHeaderName').attr("id", "LstHeaderNames" + index + "__HeaderName");
    $(row).find('.txtHeaderName').attr("name", "LstHeaderNames[" + index + "].HeaderName");

    $(row).find('.chkIsRequired').attr("id", "LstHeaderNames" + index + "__IsRequired");
    $(row).find('.chkIsRequired').attr("name", "LstHeaderNames[" + index + "].IsRequired");

    $(row).find('.txtErrorMessage').attr("id", "LstHeaderNames" + index + "__ErrorMessage");
    $(row).find('.txtErrorMessage').attr("name", "LstHeaderNames[" + index + "].ErrorMessage");

    $(row).find('.hdnColumnRuleId').attr("id", "LstHeaderNames" + index + "__QCColumnRuleId");
    $(row).find('.hdnColumnRuleId').attr("name", "LstHeaderNames[" + index + "].QCColumnRuleId");

    $(row).find('.ddlColumnType').attr("id", "LstHeaderNames" + index + "__ColumnTypeId");
    $(row).find('.ddlColumnType').attr("name", "LstHeaderNames[" + index + "].ColumnTypeId");

    $(row).find('.hdnOrder').attr("id", "LstHeaderNames" + index + "__Order");
    $(row).find('.hdnOrder').attr("name", "LstHeaderNames[" + index + "].Order");

    $(row).find('.txtRangeStart').attr("id", "LstHeaderNames" + index + "__RangeStart");
    $(row).find('.txtRangeStart').attr("name", "LstHeaderNames[" + index + "].RangeStart");

    $(row).find('.txtRangeEnd').attr("id", "LstHeaderNames" + index + "__RangeEnd");
    $(row).find('.txtRangeEnd').attr("name", "LstHeaderNames[" + index + "].RangeEnd");
}

function resetFileInput() {
    var clone = $("#addContent").clone(false, false);
    $("#addContent").replaceWith(clone);
}

//Method to set the error indicator for control
function SetErrorIndicator(cntrl, status) {
    if (status) {
        cntrl.css("border", "1px solid red");
    }
    else {
        cntrl.css("border", "1px solid");
    }
}


$(function () {
    $("#Exit").bind("click", function () {
        window.location = "../Authenticate/SignOut";
    });
});

$(function () {
    $("#Register").bind("click", function () {
        var _this = this;
        dataonBoardingData.showBusy();
        if (!dataonBoardingData.ValidateRegister()) {
            dataonBoardingData.hideBusy();
            return false;
        }

        var info = {
            FirstName: $("#txtFirstName").val(),
            MiddleName: $("#txtMiddleName").val(),
            LastName: $("#txtLastName").val(),
            Organization: $("#txtOrganization").val(),
            EmailId: $("#txtEmail").val()
        }

        var str = JSON.stringify(info);

        var authTicket = getCookie("x-api-jwt");

        if (authTicket == null || authTicket.length <= 0) {
            document.location = landingPage;
            return;
        }

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
                window.location = "/Home/index";
            },
            error: function (result) {
                if (result.status == "0") {
                    window.location = "/Home/index";
                }
                else {
                    alert("Error while registering the user");

                }
                dataonBoardingData.hideBusy();
            }
        });
    });

});

function getCookie(name) {
    var re = new RegExp(name + "=([^;]+)");
    var value = re.exec(document.cookie);
    //alert(value);
    return (value != null) ? unescape(value[1]) : null;
}


var dataonBoardingData = {

    initialize: function () {
        // if the timezone cookie not exists create one
        if (document.cookie.indexOf("__TimezoneOffset") < 0) {
            dataonBoardingData.setTimezoneCookie();
        }
    },
    constants: {
        submittedFileName: ""
    },

    setHeadersOrder: function () {
        var index = 1;
        $('#tbodyHeaders').find('tr').each(function () {
            $(this).attr("rowNumber", index);
            $(this).find('.hdnOrder').val(index);
            $(this).find('#ancDeleteHeader').attr("rowNumber", index);
            index++;
        });
    },

    setTimezoneCookie: function () {
        var timezone_cookie = "__TimezoneOffset";
        var exdate = new Date();
        exdate.setDate(exdate.getDate() + 1);
        document.cookie = timezone_cookie + "=" + escape(new Date().getTimezoneOffset()) + ";expires=" + exdate.toGMTString() + ";path=/";
    },

    ValidateRegister: function () {
        var txtFirstName = $("#txtFirstName");
        var txtLastName = $("#txtLastName");
        var txtOrganization = $("#txtOrganization");
        var txtEmailId = $("#txtEmail");
        var validationStatus = true;
        txtFirstName.attr('required', true);
        txtLastName.attr('required', true);
        txtOrganization.attr('required', true);
        txtEmailId.attr('required', true);
        $("#spanFirstName").val("");
        $("#spanLastName").val("");
        $("#spanEmail").val("");
        $("#spanOrganization").val("");

        if (txtFirstName.val().length == 0) {
            validationStatus = false;
            txtFirstName.addClass("required");
            $("#spanFirstName").text("Please enter the first name");
        }
        else {
            txtFirstName.removeClass("required");
            $("#spanFirstName").text(" ");
        }
        if (txtLastName.val().length == 0) {
            validationStatus = false;
            txtLastName.addClass("required");
            $("#spanLastName").text("Please enter the last name");
        } else {
            txtLastName.removeClass("required");
            $("#spanLastName").text(" ");
        }
        if (txtEmailId.val().length == 0) {
            validationStatus = false;
            txtOrganization.addClass("required");
            $("#spanEmail").text("Please enter the email");
        } else if (!dataonBoardingData.validateEmail(txtEmailId.val())) {
            validationStatus = false;
            txtEmailId.addClass("required");
            $("#spanEmail").text("Please enter the valid email id");
        }
        else {
            txtEmailId.removeClass("required");
            $("#spanEmail").text("");
        }
        if (txtOrganization.val().length == 0) {
            validationStatus = false;
            txtOrganization.addClass("required");
            $("#spanOrganization").text("Please enter the organization");
        } else {
            txtOrganization.removeClass("required");
            $("#spanOrganization").text("");
        }

        return validationStatus;
    },

    displayPostCred: function () {
        if ($("#hdnCredReqPost").val() == 'true') {
            $("#postCredBlock").show();
        }
        else {
            $("#postCredBlock").hide();
        }
    },
    getMetadataTypeList: function (metadataTypeID, documentID) {
        dataonBoardingData.showBusy();
        var action = "/Document/FetchMetadataTypeList";
        $.ajax(
               {
                   url: action,
                   data: { metadataTypeID: metadataTypeID, documentID: documentID },
                   type: "post",
                   success: function (returnedList) {
                       $("#divmetadatalist").html(returnedList);
                       sequimData.hideBusy();
                       // $.validator.unobtrusive.parse(document);

                   },
                   error: function (message) {
                       sequimData.hideBusy();
                   },
               });
    },
    //Email validation
    validateEmail: function (value) {

        var isEmailvalid = false;

        var emailReg = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
        if (value != undefined && value != "") {
            if (emailReg.test(value)) {
                isEmailvalid = true;
            }
        }
        return isEmailvalid;
    },

    //Phone validation
    validatePhoneNumber: function (value) {

        var isPhonevalid = false;
        // var phoneReg = /^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$/;      
        //var phoneReg = /^[0-9,+()-]+$/;
        //var phoneReg = /^(\([2-9]|[2-9])(\d{2}|\d{2}\))(-|.|\s)?\d{3}(-|.|\s)?\d{4}$/;
        var phoneReg = /^\(\d{3}\) ?\d{3}( |-)?\d{4}|^\d{3}( |-)?\d{3}( |-)?\d{4}$/;
        if (value != "") {
            if (phoneReg.test(value)) {
                isPhonevalid = true;
            }
        }
        return isPhonevalid;
    },

    //Date validation
    validateDateFormat: function (value) {
        var isvalid = false;

        //yyyy-mm-dd format from between 1900-01-01 and 2099-12-31     
        var dateReg = /^(19|20)\d\d([- /.])(0[1-9]|1[012])\2(0[1-9]|[12][0-9]|3[01])$/;
        if (value != "") {
            if (dateReg.test(value)) {
                isvalid = true;
            }
        }
        return isvalid;
    },

    //Range validation
    validateRange: function (value, minRange, maxRange) {
        var isRangeValid = true;
        var numberReg = /^-?(([1-9]\d*)|0)(.0*[0-9](0*[0-9])*)?$/;
        if (!numberReg.test(value)) {
            isRangeValid = false;
        }
        if (isRangeValid) {
            isRangeValid = (parseFloat(value) >= parseFloat(minRange) && parseFloat(value) <= parseFloat(maxRange));

        }
        return isRangeValid;
    },

    //Range validation
    validateNumeric: function (value) {
        var isNumberValid = true;
        isNumberValid = !isNaN(parseFloat(value)) && isFinite(value);
        return isNumberValid;
    },

    persistAgreement: function (repositoryName, userName) {
        window.userList.cacheAgreement.set(repositoryName, true);
        window.userList.cacheName.set(repositoryName, userName);
    },

    getIdentifier: function (reference) {
        // dataUp.showInfoHeader();
        var formReference = $(reference).parents("form");
        var action = formReference.attr("action");
        var repositoyName = $("#repositories option:selected").text();
        var whoValue = $("#citationPublisher").val();
        var whenValue = $("#citationPublicationYear").val();
        var whatValue = $("#citationTitle").val();
        var usernameValue = $("#repositoryUsername").val();
        var passwordValue = $("#repositoryPassword").val();

        var antiForgeryToken = formReference.find("[name='__RequestVerificationToken']").val();
        if (sequimData.validateRepositoryDetails()) {
            $('#loginRepositoryPopup').hide();
            dataonBoardingData.showBusy();
            $.ajax({
                url: action,
                data: { __RequestVerificationToken: antiForgeryToken, Who: whoValue, What: whatValue, When: whenValue, UserName: usernameValue, Password: passwordValue, RepositoryName: repositoyName },
                type: "post",
                success: function (returnedList) {
                    dataonBoardingData.hideBusy();
                    if (returnedList != "") {
                        var result = returnedList.split("|");
                        if (result[0] === "true") {
                            $("#txtGetIdentifier").val(result[1]);
                            $("#hiddenGetIdentifier").val(result[1]);
                            $('#loginRepositoryPopup').hide();
                            $("#getusername").val(usernameValue);
                            sequimData.persistAgreement(repositoyName, usernameValue);
                            $('#getIdentifier').attr('disabled', 'disabled');
                            $('#getIdentifierHelp').css("display", "none");
                        }
                        else {
                            $('#loginRepositoryPopup').show();
                            $('#usercredentialswrong').text('');
                            $('#usercredentialswrong').text(result[1]);
                            $('#usercredentialswrong').show();

                            //  $("#getIdentifier").attr('disabled', 'disabled');
                            $(".chkAgreement").removeAttr('checked');
                            $("#repositoryPassword").val('');
                        }
                    }
                    else {

                        $('#loginRepositoryPopup').show();
                        $('#usercredentialswrong').text(result[1]);
                        $('#usercredentialswrong').show();
                        $("#getIdentifier").attr('disabled', 'disabled');
                        $(".chkAgreement").removeAttr('checked');
                        $("#repositoryPassword").val('');
                    }
                },
                error: function (req, timeout, errorMessage) {
                    dataonBoardingData.hideBusy();
                    $('#loginRepositoryPopup').show();
                    alert(timeout + " :" + errorMessage);
                }
            });
        }
        else {
            //  $('#loginRepositoryPopup').show();
            sequimData.hideBusy();
            return false;
        }
    },

    validateRepositoryDetails: function () {
        var isValid = true;
        $("#errUserRequired").hide();
        $("#errPwdRequired").hide();

        if ($("#hdnCredRequired").val() == 'true') {

            if ($("#repositoryUsername").val() == '') {
                isValid = false;
                $("#errUserRequired").show();
            }
            if ($("#repositoryPassword").val() == '') {
                isValid = false;
                $("#errPwdRequired").show();
            }
        }
        return isValid;
    },

    validatePostRepositoryDetails: function () {
        var isValid = true;

        $("#errUserReqPost").hide();
        $("#errPwdReqPost").hide();

        if ($('.userCredential').is(':visible')) {

            if ($("#postUserName").val() == '') {
                isValid = false;
                $("#errUserReqPost").show();
            }
            if ($("#postPassword").val() == '') {
                isValid = false;
                $("#errPwdReqPost").show();
            }
        }

        return isValid;
    },

    getUserAgreement: function (element, selected) {
        var display = $('#txtAgreement');
        if (display.is(":visible")) {
            display.hide();
        } else {
            //Fetch agreement text.           
            $.post('/Repository/GetAgreement', { repositoryName: selected.text() }, function (value) {
                display.text(value);
            });
            display.show();
        }
    },

    getUserAgreementPost: function (element, selected) {
        var display = $('#txtAgreementPost');
        if (display.is(":visible")) {
            display.hide();
        } else {
            //Fetch agreement text.           
            $.post('/Repository/GetAgreement', { repositoryName: selected.text() }, function (value) {
                display.text(value);
            });
            display.show();
        }
    },

    // This displays the load indicator
    showBusy: function () {
        $("body").addClass("busy");
        $("body").click(function (e) {
            e.preventDefault()
            e.stopImmediatePropagation();
        });
    },

    // This displays the load indicator
    hideBusy: function () {
        $("body").removeClass("busy");
        $("body").unbind();
    },

    // Method to validate the quality check data
    validateQCRuleData: function (formReference) {

        $("#duplicateHeaderError").hide();
        $("#rangeEndSmallErr").hide();
        $("#invalidRangeValues").hide();
        var isValid = true;
        var headerlist = [];

        var validator = formReference.validate();
        if (!formReference.valid()) {
            validator.showErrors();
            isValid = false;
        }

        $('#tbodyHeaders').find('tr').each(function () {
            var name = $(this).find('.txtHeaderName').val();
            headerlist.push(name);
            // Range validation
            if ($(this).find('.ddlColumnType').val() == '1') {

                var rangeStart = $(this).find('.txtRangeStart').val()
                var rangeEnd = $(this).find('.txtRangeEnd').val()

                var invalidSeq = rangeStart.indexOf("-.");
                var invalidSeqOne = rangeEnd.indexOf("-.");

                if (invalidSeq >= 0 || invalidSeqOne >= 0) {
                    isValid = false;
                    $("#invalidRangeValues").show();
                }
                if (isValid) {
                    // check for the start and end validation                                     
                    if (rangeStart.trim() != '' && rangeEnd.trim() != '') {
                        var startValue = parseFloat(rangeStart.trim());
                        var endValue = parseFloat(rangeEnd.trim());
                        if (startValue > endValue) {
                            isValid = false;
                            $("#rangeEndSmallErr").show();
                        }
                    }
                }
            }

        });

        // Duplicate header names validation
        if (isValid) {
            var count = headerlist.length;
            var uniqHeaders = getUniquestrArray(headerlist);
            if (count > uniqHeaders.length) {
                isValid = false;
                $("#duplicateHeaderError").show();
            }
        }

        return isValid;
    }
};

function CheckUserAuthTokenStatus(fileId, repositoryId, repository) {
    var authTicket = getCookie("x-api-jwt");

    if (authTicket == null || authTicket.length <= 0) {
        document.location = landingPage;
        return;
    }

    $.ajax({
        url: $("#hdnBaseWebUserAPIUrl").val() + "?repositoryId=" + repositoryId,
        type: "get",
        contentType: "application/json",
        headers: { Authorization: authTicket },
        success: function (response) {
            var filePostUrl = "http://" + document.location.host + "/Post/Index?fileId=" + fileId + "&repositoryId=" + repositoryId + "&repository=" + repository;
            if (response.RedirectionRequired) {
                var liveLoginUrl = $("#hdnBaseWebApiWindowsLiveAuthPathUrl").val() + "?callBackUrl=" + encodeURIComponent(filePostUrl);
                document.location = liveLoginUrl;
            }
            else {

                document.location = filePostUrl;
            }
        },
        error: function (req, timeout, errorMessage) {
            alert(errorMessage);
        }
    });
}

function CheckUserAuthTokenStatusForDownload(fileId, repositoryId) {
    var downloadUrl = "/Home/DownloadFileFromSkyDriveRepository?fileId=" + fileId + "&accessToken=" + "&refreshToken=" + "&tokenExpiresOn=",
        authTicket = getCookie("x-api-jwt");

    if (authTicket == null || authTicket.length <= 0) {
        document.location = landingPage;
        return;
    }

    $.ajax({
        url: $("#hdnBaseWebUserAPIUrl").val() + "?repositoryId=" + repositoryId,
        type: "get",
        contentType: "application/json",
        headers: { Authorization: authTicket },
        success: function (response) {
            if (response.RedirectionRequired) {
                var location = document.location;
                downloadUrl = location.protocol + "//" + location.host + "/" + location.pathname;
                var liveLoginUrl = $("#hdnBaseWebApiWindowsLiveAuthPathUrl").val() + "?callBackUrl=" + encodeURIComponent(downloadUrl);
                sessionStorage.setItem("fileId", fileId);

                document.location = liveLoginUrl;
            }
            else {
                $("#fileDownloadFrame").contents().find('body').html("");
                $("#fileDownloadFrame").attr("src", downloadUrl);
            }
        },
        error: function (req, timeout, errorMessage) {
            alert(errorMessage);
        }
    });
}

function PublishedFileList_Onload() {
    var accessToken = getParameterByName("accessToken"),
        refreshToken = getParameterByName("refreshToken"),
        tokenExpiresOn = getParameterByName("tokenExpiresOn"),
        fileId = sessionStorage.getItem("fileId");

    if (accessToken.length > 0) {
        var downloadUrl = "/Home/DownloadFileFromSkyDriveRepository?fileId=" + fileId + "&accessToken=" + accessToken + "&refreshToken=" + refreshToken + "&tokenExpiresOn=" + tokenExpiresOn;
        $("#fileDownloadFrame").contents().find('body').html("");
        $("#fileDownloadFrame").attr("src", downloadUrl);
    }

}

var querystring = location.search.replace('?', '').split('&');
var queryObj = {};
for (var i = 0; i < querystring.length; i++) {
    var name = querystring[i].split('=')[0];
    var value = querystring[i].split('=')[1];
    queryObj[name] = value;
}

// Returns the version of Internet Explorer or a -1
// (indicating the use of another browser).
function getInternetExplorerVersion() {
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    else if (navigator.appName == 'Netscape') {
        var ua = navigator.userAgent;
        var re = new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }

    return rv;
}

