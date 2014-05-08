function validatePostPage() {
    var metadataWidget = $('#divMetadata').data('dataup-metadata');
    var hasUnsavedChanges = false;
    if (metadataWidget != undefined) {
        var isValidMetaData = metadataWidget.validate();
        if (!isValidMetaData) {
            return false;
        }

        hasUnsavedChanges = metadataWidget.hasUnsavedChanges();
    }

    if (!hasUnsavedChanges) {
        var citationWidget = $('#divCitation').data('dataup-citation');
        if (citationWidget != undefined) {
            hasUnsavedChanges = citationWidget.hasUnsavedChanges();
        }
    }

    if (hasUnsavedChanges) {
        if (!window.confirm("You have unsaved changes. Do you want to continue posting?")) {
            return false;
        }
    }

    return true;
}

function postFile(viewModel) {
    $("#divCredPopup").hide();
    dataonBoardingData.showBusy();

    var model = {
        FileId: viewModel.fileId,
        RepositoryId: viewModel.selectedRepository().id,
        AuthToken: {
            AccessToken: viewModel.accessToken,
            RefreshToken: viewModel.refreshToken,
            TokenExpiresOn: viewModel.tokenExpiresOn
        },
        UserName: viewModel.userName(),
        Password: viewModel.password(),
    };

    $.ajax({
        url: configuration.PublishApiUri,
        contentType: "application/json",
        xhrfields: { withCredentials: false },
        headers: { Authorization: viewModel.jwtAuthenticationToken },
        type: "POST",
        data: JSON.stringify(model),
        success: function (json) {
            viewModel.errorMessage(null);
            viewModel.goToHomePage();
        },
        error: function (req, timeout, errorMessage) {
            var errorText;
            if (checkStatusAndRedirectToLandingPage(req.status)) {
                return;
            }

            if (req.responseText.length > 0) {
                errorText = JSON.parse(req.responseText).Message;
            }
            else {
                errorText = errorMessage;
            }

            var metadataWidget = $('#divMetadata').data('dataup-metadata');
            if (metadataWidget != undefined) {
                var hasUnsavedChanges = metadataWidget.hasUnsavedChanges();
                var requiredFieldMetadataErrorMessage = "Required Field Metadata is not provided for the field";
                if (hasUnsavedChanges && errorText.substring(0, requiredFieldMetadataErrorMessage.length) == requiredFieldMetadataErrorMessage) {
                    errorText = "Please save the file level metadata changes before posting.";
                    viewModel.selectVisibleTab(viewModel.messages.metadata);
                    metadataWidget.showFileLevelMetadataTab(metadataWidget);
                }
            }

            viewModel.errorMessage(errorText);
            dataonBoardingData.hideBusy();
        }
    });
}

function repository(data) {
    var self = this;
    self.id = data.Id;
    self.name = data.Name;
    self.baseRepositoryId = data.BaseRepositoryId;
    self.isImpersonating = data.IsImpersonating;
    self.userAgreement = ko.observable(data.UserAgreement);
}

function postPageViewModel(data, jwtAuthenticationToken) {
    var self = this;
    self.messages = {
        back: "Back",
        change: "Change",
        changeRepository: "Change Repository",
        metadata: "Metadata",
        bestPracticesCheck: "Best Practices Check",
        qualityCheck: "Quality Check",
        saved: "Saved",
        termsAndConditionsText: "I have read the Terms and Conditions",
        cancel: "Cancel",
        post: "Post",
        unauthorizedErrorMessage: "Authentication failed for the repository.",
        userName: "User Name",
        password: "Password",
        post: "Post",
        citation: "Citation",
        close: "Close"
    };
    self.jwtAuthenticationToken = jwtAuthenticationToken;
    self.accessToken = getParameterByName("accessToken");
    self.refreshToken = getParameterByName("refreshToken");
    self.tokenExpiresOn = getParameterByName("tokenExpiresOn");
    self.errorMessage = ko.observable(null);
    self.fileId = data.FileId;
    self.fileName = data.FileName;
    self.repositoryList = ko.observableArray($.map(data.RepositoryList || [], function (r) {
        return new repository(r);
    }));
    self.selectedRepository = ko.observable();
    self.selectRepository = function (repository) {
        var result = true;
        var metadataWidget = $('#divMetadata').data('dataup-metadata');
        if (metadataWidget != undefined) {
            metadataWidget.options.repositoryId = repository.id;
            result = metadataWidget.reload();
        }

        if (result) {
            var citationWidget = $('#divCitation').data('dataup-citation');
            if (citationWidget != undefined) {
                citationWidget.options.repositoryId = repository.id;
                result = citationWidget.reload();
            }
        }

        if (result) {
            self.selectedRepository(repository);
            self.agreesUserAgreement(self.selectedRepository().userAgreement() == null || self.selectedRepository().userAgreement() == '');
        }
        $('#otherRepositories').hide();
    }

    $.each(self.repositoryList(), function (i, r) {
        if (r.id == data.RepositoryId) {
            self.selectedRepository(r);
            return;
        }
    });

    self.userName = ko.observable(data.UserName);
    self.password = ko.observable(data.Password);

    self.metadataTabTitle = ko.computed(function () {
        return self.messages.metadata + '<span class="status" style="display: none">' + '[ ' + self.messages.saved + ' ]' + '</span>'
    });

    self.citationTabTitle = ko.computed(function () {
        return self.messages.citation + '<span class="status" style="display: none">' + '[ ' + self.messages.saved + ' ]' + '</span>'
    });

    self.userAgreement = ko.computed(function () {
        return self.messages.termsAndConditionsText + '<span class="toolTip"><span class="container">' + self.selectedRepository().userAgreement() + '</span></span>';
    });
    self.agreesUserAgreement = ko.observable(self.selectedRepository().userAgreement() == null || self.selectedRepository().userAgreement() == '');

    var fileApiUri = configuration.FileApiUri + "/";
    var qualityCheckApiUri = configuration.QCApiUri + "/";
    self.visibleTab = ko.observable(self.messages.metadata);
    self.selectVisibleTab = function (tabName, vm) {
        self.hideErrorMessage();
        $(document).trigger("click");
        if (tabName == self.messages.bestPracticesCheck) {
            var bestPracticesWidget = $('#divBestPractives').data('dataup-bestPractices');
            if (bestPracticesWidget == undefined) {
                $('#divBestPractives').bestPractices({
                    fileApiUri: fileApiUri,
                    jwtAuthenticationToken: jwtAuthenticationToken,
                    fileId: self.fileId,
                    loadBeginning: function () {
                        dataonBoardingData.showBusy();
                    },
                    loadCompleted: function () {
                        dataonBoardingData.hideBusy();
                    },
                    removeSelectedBeginning: function () {
                        self.hideErrorMessage();
                        dataonBoardingData.showBusy();
                    },
                    removeSelectedCompleted: function () {
                        dataonBoardingData.hideBusy();
                    }
                });
            }
        }
        else if (tabName == self.messages.qualityCheck) {
            var qualityCheckWidget = $('#divQualityCheck').data('dataup-qualityCheck');
            if (qualityCheckWidget == undefined) {
                $('#divQualityCheck').qualityCheck({
                    qualityCheckApiUri: qualityCheckApiUri,
                    jwtAuthenticationToken: jwtAuthenticationToken,
                    fileId: self.fileId,
                    loadBeginning: function () {
                        dataonBoardingData.showBusy();
                    },
                    loadCompleted: function () {
                        dataonBoardingData.hideBusy();
                    },
                    validationBeginning: function () {
                        self.hideErrorMessage();
                        dataonBoardingData.showBusy();
                    },
                    validationCompleted: function () {
                        dataonBoardingData.hideBusy();
                    }
                });
            }
        }
        else if (tabName == self.messages.citation) {
            var citationWidget = $('#divCitation').data('dataup-citation');
            if (citationWidget == undefined) {
                $('#divCitation').citation({
                    fileApiUri: fileApiUri,
                    jwtAuthenticationToken: jwtAuthenticationToken,
                    fileId: self.fileId,
                    repositoryId: self.selectedRepository().id,
                    loadBeginning: function () {
                        dataonBoardingData.showBusy();
                    },
                    loadCompleted: function () {
                        dataonBoardingData.hideBusy();
                    },
                    saveBeginning: function () {
                        self.hideErrorMessage();
                        $('#citationTab a span').hide();
                        dataonBoardingData.showBusy();
                    },
                    saveSucceeded: function (event, result) {
                        if (result.status == true) {
                            $('#citationTab a span').show();
                        }
                    },
                    saveCompleted: function () {
                        dataonBoardingData.hideBusy();
                    }
                });
            }
        }

        self.visibleTab(tabName);
    };

    self.post = function () {
        self.hideErrorMessage();
        if (!self.agreesUserAgreement()) {
            return;
        }

        if (!validatePostPage()) {
            return;
        }

        if (self.selectedRepository().baseRepositoryId == 1 && self.selectedRepository().isImpersonating == false) {
            self.userName('');
            self.password('');
            $("#divCredPopup").show();
            return;
        }

        postFile(self);
    }

    self.postToNonImpersonatedRepository = function () {
        var nonImpersonatedForm = $('#nonImpersonatedForm');
        var validator = nonImpersonatedForm.validate();
        if (!nonImpersonatedForm.valid()) {
            validator.showErrors();
            return false;
        }

        postFile(self);
    }

    self.goToHomePage = function () {
        $('#body').removeClass('admin');
        loadHomeView();
    }

    self.hideErrorMessage = function () {
        self.errorMessage(null);
    }
}

postPageViewModel.fromObject = function (data, jwtAuthenticationToken) {
    return new postPageViewModel(data, jwtAuthenticationToken);
}

function loadPostPage(fileApiUri, fileId, repositoryId, jwtAuthenticationToken) {
    var viewModel;

    $.ajax({
        url: fileApiUri + fileId + "/getpostfiledetails?repositoryId=" + repositoryId,
        contentType: "application/json",
        xhrfields: { withCredentials: false },
        headers: { Authorization: jwtAuthenticationToken },
        type: "GET",
        async: false
    })
    .success(function (data) {
        viewModel = postPageViewModel.fromObject(data, jwtAuthenticationToken);
    })
    .fail(function (jqXHR, textStatus, errorThrown) {
        if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
            return;
        }
        var httpError = $.parseJSON(jqXHR.responseText);
        viewModel = postPageViewModel.fromObject(httpError, jwtAuthenticationToken);
    })
    .complete(function (jqXHR, textStatus) {
        ko.applyBindings(viewModel, $('#postDiv')[0]);
    });

    return viewModel;
}

$(function () {
    $('#body').removeClass('admin');

    var fileApiUri = configuration.FileApiUri + "/";
    var fileId = $('#fileId').val();
    var repositoryId = $('#repositoryId').val();

    var authTicket = getCookie("x-api-jwt");
    if (authTicket == null || authTicket.length <= 0) {
        var landingPage = getSignoutURL();
        document.location = landingPage;
        return;
    }

    var viewModel = loadPostPage(fileApiUri, fileId, repositoryId, authTicket);
    $("#nonImpersonatedForm").removeData("validator")
    $("#nonImpersonatedForm").removeData("unobtrusiveValidation");

    $('#divMetadata').metadata({
        fileApiUri: fileApiUri,
        jwtAuthenticationToken: authTicket,
        fileId: fileId,
        repositoryId: repositoryId,
        validationFailed: function () {
            viewModel.selectVisibleTab(viewModel.messages.metadata);
        },
        loadBeginning: function () {
            dataonBoardingData.showBusy();
        },
        loadCompleted: function () {
            dataonBoardingData.hideBusy();
        },
        saveBeginning: function () {
            viewModel.hideErrorMessage();
            $('#metadataTab a span').hide();
            dataonBoardingData.showBusy();
        },
        saveSucceeded: function (event, result) {
            if (result.status == true) {
                $('#metadataTab a span').show();
            }
        },
        saveCompleted: function () {
            dataonBoardingData.hideBusy();
        },
        tabChanged: function () {
            viewModel.hideErrorMessage();
        }
    });

    $("#showOtherRepositories").click(function () {
        viewModel.hideErrorMessage();
        $('#otherRepositories').show(100).fadeIn(300);
    });

    $(document).click(function () {
        if ($('#otherRepositories').is(':visible')) {
            $('#otherRepositories').hide();
        }
    });
});