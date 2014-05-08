(function ($, undefined) {
    bestPracticesMessages = {
        fileApiUriEmptyErrorMessage: "fileApiUri cannot be empty.",
        jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
        fileIdInvalidErrorMessage: "fileId should be a number and should be greater than zero.",
        bestPracticesHeader1: "We have checked your data file for potential issues that might make your data less understandable to others. Some of the issues are related to .csv compatibility, and others are simply best practices for data management and organization.",
        bestPracticesHeader2: "Some of the potential issues listed here must be manually altered to comply with best practices. ",
        bestPracticesHeader3: " modified files to fix errors. You can Ignore these errors and continue posting.",
        download: "Download",
        noErrorsInThisFileMessage: "There are no errors in this file.",
        removeSelected: "Remove Selected",
        potentialProblem: "Potential Problem",
        whyThisMayBeAProblem: "Why this may be a problem",
        suggestedRemedy: "Suggested Remedy",
        locations: "Location(s)",
        noErrorsFound: "No errors found.",
        previous: "Previous",
        next: "Next"
    };

    $.widget("dataup.bestPractices", {
        options: {
            fileApiUri: "",
            jwtAuthenticationToken: "",
            fileId: 0,
            bestPracticesInitializationFailed: $.noop,
            loadBeginning: $.noop,
            loadCompleted: $.noop,
            removeSelectedBeginning: $.noop,
            removeSelectedCompleted: $.noop
        },

        _create: function () {
            var bestPracticesInitializationFailed = "bestPracticesInitializationFailed";
            if (this.options.fileApiUri == undefined || this.options.fileApiUri == null || $.trim(this.options.fileApiUri) == "") {
                this._trigger(bestPracticesInitializationFailed, null, { message: bestPracticesMessages.fileApiUriEmptyErrorMessage });
                return;
            }

            if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
                this._trigger(bestPracticesInitializationFailed, null, { message: bestPracticesMessages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            if (this.options.fileId == undefined || this.options.fileId == null || this.options.fileId <= 0) {
                this._trigger(bestPracticesInitializationFailed, null, { message: bestPracticesMessages.fileIdInvalidErrorMessage });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};

            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this);
        },

        _initializeDataBindings: function () {
            this.bestPracticesHtml =
            $('<div class="bestPractices">\
                <div class="container" data-bind="visible: errorMessage() != null">\
                    <div class="validate">\
                        <span data-bind="text: errorMessage()"></span>\
                    </div>\
                </div>\
                <div class="container" data-bind="visible: errorMessage() == null">\
                    <section>\
                        <div class="container">\
                            <p data-bind="text: bestPracticesMessages.bestPracticesHeader1"></p>\
                            <div class="info">\
                                <p data-bind="html: bestPracticesHeader2"></p>\
                            </div>\
                        </div>\
                    </section>\
                    <span class="error" data-bind="visible: noErrors, text: bestPracticesMessages.noErrorsInThisFileMessage"></span>\
                    <section data-bind="visible: fileSheets().length > 0">\
                        <nav>\
                            <div class="tabNavigation">\
                                <div id="st_horizontal" class="st_horizontal">\
                                    <div class="st_tabs_container st_sliding_active">\
                                        <a id="ancPreTab" style="display: inline;" href="#prev" class="st_prev st_btn_disabled" data-bind="attr: { title: bestPracticesMessages.previous }"></a>\
                                        <div style="overflow: hidden;" class="st_slide_container">\
                                            <ul id="errorTabs" class="st_tabs" data-bind="foreach: fileSheets">\
                                                <li data-bind="css: { active: $data == $root.selectedFileSheet() }, click: $root.selectFileSheet, attr: { sheetName: sheetName, sheetId: sheetId }">\
                                                    <a data-bind="css: { active: $data == $root.selectedFileSheet() }, attr: { sheetName: sheetName, sheetId: sheetId, title: tabName }, text: tabName" href="#"></a>\
                                                </li>\
                                            </ul>\
                                        </div>\
                                        <a style="display: inline;" href="#next" class="st_next st_btn_disabled" data-bind="attr: { title: bestPracticesMessages.next }"></a>\
                                    </div>\
                                </div>\
                            </div>\
                        </nav>\
                        <div class="container">\
                            <div data-bind="visible: canDeleteErrors()" style="overflow: hidden;">\
                                <a href="#" class="button l2 cancel left disabled removeError" data-bind="text: bestPracticesMessages.removeSelected, css: { disabled: disableRemoveSelected }, click: removeSelectedErrors, attr: { title: bestPracticesMessages.removeSelected }"></a>\
                            </div>\
                            <table cellpadding="0" cellspacing="0" border="0" width="100%" class="report-pane">\
                                <thead>\
                                    <tr>\
                                        <th width="5%"></th>\
                                        <th width="28%" data-bind="html: bestPracticesMessages.potentialProblem, attr: { title: bestPracticesMessages.potentialProblem }"></th>\
                                        <th width="28%" data-bind="html: bestPracticesMessages.whyThisMayBeAProblem, attr: { title: bestPracticesMessages.whyThisMayBeAProblem }"></th>\
                                        <th width="28%" data-bind="html: bestPracticesMessages.suggestedRemedy, attr: { title: bestPracticesMessages.suggestedRemedy }"></th>\
                                        <th width="11%" data-bind="html: bestPracticesMessages.locations, attr: { title: bestPracticesMessages.locations }"></th>\
                                    </tr>\
                                </thead>\
                            </table>\
                            <div class="scroll-grid">\
                                <table id="tabErrorContainer" border="0" cellspacing="0" cellpadding="0" data-bind="foreach: fileSheets">\
                                    <tbody data-bind="attr: { id: sheetId }, visible: $data == $root.selectedFileSheet()">\
                                        <tr>\
                                            <td class="noBorder noPad">\
                                                <table border="0" cellspacing="0" cellpadding="0">\
                                                    <tbody data-bind="visible: !$root.noErrors && fileErrors().length == 0">\
                                                        <tr>\
                                                            <td data-bind="html: bestPracticesMessages.noErrorsFound"></td>\
                                                        </tr>\
                                                    </tbody>\
                                                    <tbody data-bind="visible: fileErrors().length > 0, foreach: fileErrors">\
                                                        <tr>\
                                                            <td width="5%">\
                                                                <input name="" type="checkbox" value="" data-bind="checked: isSelected, enable: canFix"></input>\
                                                                <input type="hidden" data-bind="value: errorType"></input>\
                                                            </td>\
                                                            <td width="28%" data-bind="text: title, attr: { title: title }"></td>\
                                                            <td width="28%" data-bind="text: description, attr: { title: description }"></td>\
                                                            <td width="28%" data-bind="text: recommendation, attr: { title: recommendation }"></td>\
                                                            <td width="11%" class="location" data-bind="text: errorAddress, attr: { title: errorAddress }"></td>\
                                                        </tr>\
                                                    </tbody>\
                                                </table>\
                                            </td>\
                                        </tr>\
                                    </tbody>\
                                </table>\
                            </div>\
                        </div>\
                    </section>\
                </div>\
              </div>').appendTo(this.element);

            this.element.addClass('widgetContainer');
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                    getErrors: getErrors,
                    removeErrors: removeErrors
                };

                return dataContext;

                function getErrors() {
                    return $.ajax({
                        url: getGetErrorsUri(),
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }

                function removeErrors(sheetName, errorTypes) {
                    var removeErrorsViewModel = {
                        FileId: options.fileId,
                        SheetName: sheetName,
                        ErrorTypes: errorTypes
                    };

                    return $.ajax({
                        url: getRemoveErrorsUri(),
                        type: "DELETE",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken },
                        datatype: "json",
                        data: JSON.stringify(removeErrorsViewModel)
                    });
                }

                function getGetErrorsUri() {
                    return options.fileApiUri + "/" + options.fileId + "/errors";
                }

                function getRemoveErrorsUri() {
                    return options.fileApiUri + "/" + options.fileId + "/removeerrors";
                }
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            dataContext.fileSheet = fileSheet;
            dataContext.fileError = fileError;

            function fileSheet(data) {
                var self = this;
                self.sheetId = data.SheetId;
                self.sheetName = data.SheetName;
                self.fileErrors = ko.observableArray($.map(data.FileErrors || [], function (fe) {
                    return new fileError(fe);
                }));

                self.tabName = ko.computed(function () {
                    return self.sheetName + ' (' + self.fileErrors().length + ')';
                });
            }

            function fileError(data) {
                var self = this;
                self.errorType = data.ErrorType;
                self.title = data.Title;
                self.description = data.Description;
                self.recommendation = data.Recommendation;
                self.errorAddress = data.ErrorAddress;
                self.canFix = data.CanFix;
                self.isSelected = ko.observable(false);
            }
        },

        _initializeViewModel: function (ko, widget) {
            this.element.dataUpManager.viewModel = (function (ko, widget) {
                var dataContext = widget.element.dataUpManager.dataContext;
                var options = widget.options;
                var bestPracticesHtml = widget.bestPracticesHtml[0];

                var canDeleteErrors = ko.observable(false);
                var fileSheets = ko.observableArray();
                var selectedFileSheet = ko.observable();
                var errorMessage = ko.observable(null);
                var bestPracticesHeader2 = ko.observable(bestPracticesMessages.bestPracticesHeader2 + '<a href="#" title="' + bestPracticesMessages.download + '">' + bestPracticesMessages.download + '</a>' + bestPracticesMessages.bestPracticesHeader3);

                function selectFileSheet(fileSheet) {
                    selectedFileSheet(fileSheet);
                };

                var noErrors = ko.computed(function () {
                    var noErrors = fileSheets().length > 0;
                    $.each(fileSheets(), function (index, fs) {
                        if (fs.fileErrors().length > 0) {
                            noErrors = false;
                            return;
                        }
                    });
                    return noErrors;
                });

                var disableRemoveSelected = ko.computed(function () {
                    var disableRemoveSelected = true;
                    if (selectedFileSheet() != undefined && selectedFileSheet() != null) {
                        $.each(selectedFileSheet().fileErrors(), function (index, fe) {
                            if (fe.isSelected() === true) {
                                disableRemoveSelected = false;
                                return;
                            }
                        });
                    }

                    return disableRemoveSelected;
                });

                function removeSelectedErrors() {
                    if (selectedFileSheet() == undefined || selectedFileSheet() == null) {
                        return;
                    }

                    var selectedFileErrors = [], selectedErrorTypes = [];
                    $.each(selectedFileSheet().fileErrors(), function (index, fe) {
                        if (fe.isSelected() === true) {
                            selectedFileErrors.push(fe);
                            selectedErrorTypes.push(fe.errorType);
                        }
                    });

                    if (selectedFileErrors.length == 0) {
                        return;
                    }

                    widget._trigger("removeSelectedBeginning");

                    dataContext.removeErrors(selectedFileSheet().sheetName, selectedErrorTypes)
                    .success(function (data) {
                        errorMessage(null);
                        $.each(selectedFileErrors, function (index, fe) {
                            selectedFileSheet().fileErrors.remove(fe);
                        });
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                            return;
                        }
                        var httpError = $.parseJSON(jqXHR.responseText);
                        errorMessage(httpError.Message);
                    })
                    .complete(function (jqXHR, textStatus) {
                        widget._trigger("removeSelectedCompleted");
                    });
                }

                var viewModel = {
                    canDeleteErrors: canDeleteErrors,
                    fileSheets: fileSheets,
                    selectedFileSheet: selectedFileSheet,
                    errorMessage: errorMessage,
                    bestPracticesHeader2: bestPracticesHeader2,
                    noErrors: noErrors,
                    selectFileSheet: selectFileSheet,
                    disableRemoveSelected: disableRemoveSelected,
                    removeSelectedErrors: removeSelectedErrors
                };

                ko.applyBindings(viewModel, bestPracticesHtml);

                function getErrors() {
                    widget._trigger("loadBeginning");

                    dataContext.getErrors()
                    .success(function (data) {
                        canDeleteErrors(data.CanDeleteErrors);
                        var fileDownloadUri = '/Home/DownloadFile?fileId=' + options.fileId + '&mimeType=' + data.MimeType + '&fileName=' + data.FileName;
                        bestPracticesHeader2(bestPracticesMessages.bestPracticesHeader2 + '<a href="' + fileDownloadUri + '" title="' + bestPracticesMessages.download + '">' + bestPracticesMessages.download + '</a>' + bestPracticesMessages.bestPracticesHeader3);

                        $.each(data.FileSheets || [], function (index, fs) {
                            var fileSheet = new dataContext.fileSheet(fs);
                            fileSheets.push(fileSheet);
                        });

                        if (fileSheets().length > 0) {
                            selectedFileSheet(fileSheets()[0]);
                        }
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                            return;
                        }
                        var httpError = $.parseJSON(jqXHR.responseText);

                        errorMessage(httpError.Message);
                    })
                    .complete(function (jqXHR, textStatus) {
                        widget._trigger("loadCompleted");
                    });
                }

                function initializeViewModel() {
                    getErrors();
                }

                initializeViewModel();

                return viewModel;
            })(ko, widget);
        },

        _destroy: function () {
            this.bestPracticesHtml.remove();
        }
    });
})(jQuery);
