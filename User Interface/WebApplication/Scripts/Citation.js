(function ($, undefined) {
    citationMessages = {
        helpText: "You can uniquely identify and refer to your data by generating a citation. \
                   Generated citation will be associated with your file and you will be \
                   able to view the same in file details. ",
        fileApiUriEmptyErrorMessage: "fileApiUri cannot be empty.",
        jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
        fileIdInvalidErrorMessage: "fileId should be a number and should be greater than zero.",
        repositoryIdInvalidErrorMessage: "repositoryId should be a number and should be greater than zero.",
        citationDetails: "Citation Details",
        fieldName: "Field Name",
        fieldData: "Field Data",
        publicationYear: "Publication Year",
        publicationYearTitle: "Year data are being submitted (usually the current year).",
        title: "Title",
        titleTitle: "Use a distinct title for the dataset that identifies its contents",
        version: "Version",
        versionTitle: "If this is a new submission, the version number is 1. Updated versions should be sequentially ordered.",
        publisher: "Publisher",
        publisherTitle: "List dataset authors, last name first.",
        unsavedChangesWhileReloading: "You have unsaved changes in Citation. Do you want to continue?",
        save: "Save"
    };

    $.widget("dataup.citation", {
        options: {
            fileApiUri: "",
            jwtAuthenticationToken: "",
            fileId: 0,
            repositoryId: 0,
            initializationFailed: $.noop,
            loadBeginning: $.noop,
            loadCompleted: $.noop,
            saveBeginning: $.noop,
            saveSucceeded: $.noop,
            saveCompleted: $.noop
        },

        hasUnsavedChanges: function () {
            return this.element.dataUpManager.dataContext.hasUnsavedChanges;
        },

        reload: function () {
            var widget = this;
            var dataContext = widget.element.dataUpManager.dataContext;
            if (dataContext.hasUnsavedChanges) {
                if (!window.confirm(citationMessages.unsavedChangesWhileReloading)) {
                    return false;
                }
            }

            var viewModel = widget.element.dataUpManager.viewModel
            function fieldEditedHandler() {
                widget._fieldEditedHandler(dataContext);
            }

            widget._trigger("loadBeginning");

            dataContext.getCitation()
            .success(function (data) {
                viewModel.errorMessage(null);
                viewModel.citation().publicationYear(data.PublicationYear);
                viewModel.citation().title(data.Title);
                viewModel.citation().version(data.Version);
                viewModel.citation().publisher(data.Publisher);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                    return;
                }
                var httpError = $.parseJSON(jqXHR.responseText);
                viewModel.errorMessage(httpError.Message);
            })
            .complete(function (jqXHR, textStatus) {
                widget._trigger("loadCompleted");
                dataContext.hasUnsavedChanges = false;
            });

            return true;
        },

        _create: function () {
            if (this.options.fileApiUri == undefined || this.options.fileApiUri == null || $.trim(this.options.fileApiUri) == "") {
                this._trigger("initializationFailed", null, { message: citationMessages.fileApiUriEmptyErrorMessage });
                return;
            }

            if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
                this._trigger("initializationFailed", null, { message: citationMessages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            if (this.options.fileId == undefined || this.options.fileId == null || this.options.fileId <= 0) {
                this._trigger("initializationFailed", null, { message: citationMessages.fileIdInvalidErrorMessage });
                return;
            }

            if (this.options.repositoryId == undefined || this.options.repositoryId == null || this.options.repositoryId <= 0) {
                this._trigger("initializationFailed", null, { message: citationMessages.repositoryIdInvalidErrorMessage });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};

            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this);
        },

        _initializeDataBindings: function () {
            this.citationHtml = $('<form id="citation">\
                                    <p data-bind="text: helpText()"></p>\
                                    <div class="container" data-bind="visible: errorMessage() != null">\
                                        <div class="validate">\
                                            <span data-bind="text: errorMessage()"></span>\
                                        </div>\
                                    </div>\
                                    <div class="list" data-bind="visible: errorMessage() == null">\
                                        <table cellpadding="0" cellspacing="0" border="0">\
                                            <tbody>\
                                                <tr>\
                                                    <td>\
                                                        <label data-bind="text: citationMessages.publicationYear, title: citationMessages.publicationYearTitle"></label>\
                                                    </td>\
                                                    <td>\
                                                        <input type="text" maxlength="50" data-bind="value: citation().publicationYear" />\
                                                    </td>\
                                                </tr>\
                                                <tr>\
                                                    <td>\
                                                        <label data-bind="text: citationMessages.title, title: citationMessages.titleTitle"></label>\
                                                    </td>\
                                                    <td>\
                                                        <input type="text" maxlength="500" data-bind="value: citation().title" />\
                                                    </td>\
                                                </tr>\
                                                <tr>\
                                                    <td>\
                                                        <label data-bind="text: citationMessages.version, title: citationMessages.versionTitle"></label>\
                                                    </td>\
                                                    <td>\
                                                        <input type="text" maxlength="50" data-bind="value: citation().version" />\
                                                    </td>\
                                                </tr>\
                                                <tr>\
                                                    <td>\
                                                        <label data-bind="text: citationMessages.publisher, title: citationMessages.publisherTitle"></label>\
                                                    </td>\
                                                    <td>\
                                                        <input type="text" maxlength="200" data-bind="value: citation().publisher" />\
                                                    </td>\
                                                </tr>\
                                            </tbody>\
                                        </table>\
                                    </div>\
                                    <div class="endStripe" data-bind="visible: errorMessage() == null">\
                                        <input type="button" class="button l2 save" data-bind="value: citationMessages.save, click: saveCitation, attr: { title: citationMessages.save }" />\
                                    </div>\
                                   </form>').appendTo(this.element);

            this.element.addClass('widgetContainer');
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                    getCitation: getCitation,
                    saveCitation: saveCitation
                };

                return dataContext;

                function getCitation() {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/citation?repositoryId=" + options.repositoryId,
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }

                function saveCitation(citation) {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/savecitation",
                        type: "POST",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken },
                        datatype: "json",
                        data: JSON.stringify(citation)
                    });
                }
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            dataContext.hasUnsavedChanges = false;
            dataContext.citation = citation;

            function citation(data) {
                var self = this;
                self.publicationYear = ko.observable();
                self.title = ko.observable();
                self.version = ko.observable();
                self.publisher = ko.observable();
            }
        },

        _initializeViewModel: function (ko, widget) {
            this.element.dataUpManager.viewModel = (function (ko, widget) {
                var dataContext = widget.element.dataUpManager.dataContext;
                var options = widget.options;
                var citationHtml = widget.citationHtml[0];
                var citationHtmlForm = $(citationHtml);
                citationHtmlForm.validate({ ignore: [] });

                var citation = ko.observable(new dataContext.citation());
                var errorMessage = ko.observable(null);
                var helpText = ko.observable(citationMessages.helpText);

                function saveCitation() {
                    widget._trigger("saveBeginning");

                    var citationToBeSaved = {
                        publicationYear: citation().publicationYear(),
                        title: citation().title(),
                        version: citation().version(),
                        publisher: citation().publisher()
                    };

                    dataContext.saveCitation(citationToBeSaved)
                    .success(function (data) {
                        errorMessage(null);
                        widget._trigger("saveSucceeded", null, { status: data });
                        dataContext.hasUnsavedChanges = false;
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                            return;
                        }
                        var httpError = $.parseJSON(jqXHR.responseText);
                        self.errorMessage(httpError.Message);
                    })
                    .complete(function (jqXHR, textStatus) {
                        widget._trigger("saveCompleted");
                    });
                }

                var viewModel = {
                    citation: citation,
                    errorMessage: errorMessage,
                    helpText: helpText,
                    saveCitation: saveCitation
                };

                ko.applyBindings(viewModel, citationHtml);

                function fieldEditedHandler() {
                    widget._fieldEditedHandler(dataContext);
                }

                function getCitation() {
                    widget._trigger("loadBeginning");

                    dataContext.getCitation()
                    .success(function (data) {
                        citation().publicationYear(data.PublicationYear);
                        citation().title(data.Title);
                        citation().version(data.Version);
                        citation().publisher(data.Publisher);
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                            return;
                        }
                        var httpError = $.parseJSON(jqXHR.responseText);
                        errorMessage(httpError.Message);
                        helpText("");
                    })
                    .complete(function (jqXHR, textStatus) {
                        widget._trigger("loadCompleted");
                        citation().publicationYear.subscribe(fieldEditedHandler);
                        citation().title.subscribe(fieldEditedHandler);
                        citation().version.subscribe(fieldEditedHandler);
                        citation().publisher.subscribe(fieldEditedHandler);
                    });
                }

                function initializeViewModel() {
                    getCitation();
                }

                initializeViewModel();

                return viewModel;
            })(ko, widget);
        },

        _fieldEditedHandler: function (dataContext) {
            dataContext.hasUnsavedChanges = true;
        },

        _destroy: function () {
            this.citationHtml.remove();
        }
    });
})(jQuery);
