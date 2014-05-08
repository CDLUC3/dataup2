(function ($, undefined) {
    qualityCheckMessages = {
        helpText: "You can validate your data against pre-configured rules. \
                   Select the rule, data on which the rule should be applied \
                   and click validate to see the results. \
                   The report will show you all the detected anamolies.",
        qualityCheckApiUriEmptyErrorMessage: "qualityCheckApiUri cannot be empty.",
        jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
        fileIdInvalidErrorMessage: "fileId should be a number and should be greater than zero.",
        noQualityCheckRulesFound: "No quality check rules exists to validate.",
        errNoSheetSelected: "No sheet is selected.",
        errValidateSheets: "Error while validating the sheet(s).",
        ruleName: "Rule Name",
        sheetName: "Sheet Name",
        selectSheets: "Select Sheets",
        status: "Status",
        errorDescription: "Error Description",
        passed: "Passed",
        failed: "Failed",
        validate: "Validate"
    };

    $.widget("dataup.qualityCheck", {
        options: {
            qualityCheckApiUri: "",
            jwtAuthenticationToken: "",
            fileId: 0,
            qualityCheckInitializationFailed: $.noop,
            loadBeginning: $.noop,
            loadCompleted: $.noop,
            validationBeginning: $.noop,
            validationCompleted: $.noop
        },

        _create: function () {
            var qualityCheckInitializationFailed = "qualityCheckInitializationFailed";
            if (this.options.qualityCheckApiUri == undefined || this.options.qualityCheckApiUri == null || $.trim(this.options.qualityCheckApiUri) == "") {
                this._trigger(qualityCheckInitializationFailed, null, { message: qualityCheckMessages.qualityCheckApiUriEmptyErrorMessage });
                return;
            }

            if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
                this._trigger(qualityCheckInitializationFailed, null, { message: qualityCheckMessages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            if (this.options.fileId == undefined || this.options.fileId == null || this.options.fileId <= 0) {
                this._trigger(qualityCheckInitializationFailed, null, { message: qualityCheckMessages.fileIdInvalidErrorMessage });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};

            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this);
        },

        _initializeDataBindings: function () {
            this.qualityCheckHtml =
            $('<div>\
                    <p data-bind="text: qualityCheckMessages.helpText"></p>\
                    <div class="container" data-bind="visible: errorMessage() != null">\
                        <div class="validate">\
                            <span data-bind="text: errorMessage()"></span>\
                        </div>\
                    </div>\
                    <div class="container" data-bind="visible: errorMessage() == null">\
                        <div class="validate">\
                            <table cellpadding="0" cellspacing="0" border="0" width="400">\
                                <tr data-bind="visible: !isLoading() && columnRules().length == 0">\
                                    <td>\
                                        <div style="width: 500px" class="list">\
                                            <label style="width: 200px" data-bind="text: qualityCheckMessages.noQualityCheckRulesFound"></label>\
                                        </div>\
                                    </td>\
                                </tr>\
                                <tr data-bind="visible: isLoading() || columnRules().length > 0">\
                                    <td width="200">\
                                        <label data-bind="text: qualityCheckMessages.ruleName"></label>\
                                        <select data-bind="options: columnRules, optionsValue: \'ruleId\', optionsText: \'ruleName\', value: selectedColumnRule"></select>\
                                    </td>\
                                    <td width="200">\
                                        <label data-bind="text: qualityCheckMessages.sheetName"></label>\
                                        <ul class="dropDown">\
                                            <li>\
                                                <span data-bind="text: qualityCheckMessages.selectSheets"></span>\
                                                <ul data-bind="foreach: fileSheets">\
                                                    <li>\
                                                        <input type="checkbox" data-bind="checked: isSelected, attr: { id: sheetId }" />\
                                                        <label data-bind="text: (sheetName.length > 25 ? sheetName.substring(0, 22) + \'...\' : sheetName), attr: { for: sheetId, title: sheetName }"></label>\
                                                    </li>\
                                                </ul>\
                                            </li>\
                                        </ul>\
                                    </td>\
                                    <td valign="bottom" class="vBottom">\
                                        <input type="button" class="button l2 done right validate" data-bind="value: qualityCheckMessages.validate, click: validate, attr: { title: qualityCheckMessages.validate }" />\
                                    </td>\
                                </tr>\
                            </table>\
                            <div>\
                                <span style="color: red; display: none;" data-bind="text: qualityCheckMessages.errNoSheetSelected, visible: isErrorNoSheetSelected"></span>\
                                <span style="color: red; display: none;" data-bind="text: qualityCheckMessages.errValidateSheets, visible: isErrValidateSheets"></span>\
                            </div>\
                            <div class="list" data-bind="visible: qualityCheckResults().length > 0">\
                                <div class="list">\
                                    <table cellpadding="0" cellspacing="0" border="0" width="100%">\
                                        <thead>\
                                            <tr>\
                                                <th width="150" data-bind="text: qualityCheckMessages.sheetName, attr: { title: qualityCheckMessages.sheetName }"></th>\
                                                <th width="40" data-bind="text: qualityCheckMessages.status, attr: { title: qualityCheckMessages.status }"></th>\
                                                <th data-bind="text: qualityCheckMessages.errorDescription, attr: { title: qualityCheckMessages.errorDescription }"></th>\
                                            </tr>\
                                        </thead>\
                                        <tbody data-bind="foreach: qualityCheckResults">\
                                            <tr>\
                                                <td data-bind="text: sheetName"></td>\
                                                <td class="status">\
                                                    <span data-bind="css: { passed: errors().length == 0 }, text: errors().length == 0 ? qualityCheckMessages.passed : qualityCheckMessages.failed, attr: { title: errors().length == 0 ? qualityCheckMessages.passed : qualityCheckMessages.failed }"></span>\
                                                </td>\
                                                <td class="details" data-bind="foreach: errors">\
                                                    <span data-bind="text: ($data.length > 53 ? $data.substring(0, 50) + \'...\' : $data), attr: { title: $data }"></span>\
                                                    <br />\
                                                </td>\
                                            </tr>\
                                        </tbody>\
                                    </table>\
                                </div>\
                            </div>\
                        </div>\
                    </div>\
               </div>').appendTo(this.element);

            this.element.addClass('widgetContainer');
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                    getQualityCheckRulesAndFileSheets: getQualityCheckRulesAndFileSheets,
                    getQualityCheckIssues: getQualityCheckIssues
                };

                return dataContext;

                function getQualityCheckRulesAndFileSheets() {
                    return $.ajax({
                        url: options.qualityCheckApiUri + "?fileId=" + options.fileId,
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }

                function getQualityCheckIssues(fileId, qualityCheckId, sheetIds) {
                    return $.ajax({
                        url: options.qualityCheckApiUri + "?fileId=" + fileId + "&qualityCheckId=" + qualityCheckId + "&sheetIds=" + sheetIds,
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            dataContext.columnRule = columnRule;
            dataContext.fileSheet = fileSheet;
            dataContext.qualityCheckResult = qualityCheckResult;

            function columnRule(data) {
                var self = this;
                self.ruleId = data.QualityCheckData.QualityCheckId;
                self.ruleName = data.QualityCheckData.Name;
            }

            function fileSheet(data) {
                var self = this;
                self.sheetId = data.SheetId;
                self.sheetName = data.SheetName;
                self.isSelected = ko.observable(true);
            }

            function qualityCheckResult(data) {
                var self = this;
                self.sheetId = data.SheetId;
                self.sheetName = data.SheetName;
                self.errors = ko.observableArray(data.Errors);
            }
        },

        _initializeViewModel: function (ko, widget) {
            this.element.dataUpManager.viewModel = (function (ko, widget) {
                var dataContext = widget.element.dataUpManager.dataContext;
                var options = widget.options;
                var qualityCheckHtml = widget.qualityCheckHtml[0];

                var isLoading = ko.observable(true);
                var errorMessage = ko.observable(null);
                var columnRules = ko.observableArray();
                var selectedColumnRule = ko.observable();
                var fileSheets = ko.observableArray();
                var isErrorNoSheetSelected = ko.observable(false);
                var isErrValidateSheets = ko.observable(false);
                var qualityCheckResults = ko.observableArray();

                function validate() {
                    qualityCheckResults.removeAll();
                    isErrorNoSheetSelected(false);
                    isErrValidateSheets(false);
                    var selectedSheetIds = [];
                    $.each(fileSheets(), function (index, sheet) {
                        if (sheet.isSelected() && sheet.sheetId != -1) {
                            selectedSheetIds.push(sheet.sheetId);
                        }
                    });

                    if (selectedSheetIds.length == 0) {
                        isErrorNoSheetSelected(true);
                        return;
                    }

                    widget._trigger("validationBeginning");

                    dataContext.getQualityCheckIssues(options.fileId, selectedColumnRule(), selectedSheetIds)
                    .success(function (data) {
                        errorMessage(null);
                        $.each(data || [], function (index, qcr) {
                            var qualityCheckResult = new dataContext.qualityCheckResult(qcr)
                            qualityCheckResults.push(qualityCheckResult);
                        });
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                            return;
                        }
                        var httpError = $.parseJSON(jqXHR.responseText);
                        errorMessage(httpError.Message);
                        isErrValidateSheets(true);
                    })
                    .complete(function (jqXHR, textStatus) {
                        widget._trigger("validationCompleted");
                    });
                }

                var viewModel = {
                    isLoading: isLoading,
                    errorMessage: errorMessage,
                    columnRules: columnRules,
                    selectedColumnRule: selectedColumnRule,
                    fileSheets: fileSheets,
                    isErrorNoSheetSelected: isErrorNoSheetSelected,
                    isErrValidateSheets: isErrValidateSheets,
                    qualityCheckResults: qualityCheckResults,
                    validate: validate
                };

                ko.applyBindings(viewModel, qualityCheckHtml);

                function getQualityCheckRulesAndFileSheets() {
                    widget._trigger("loadBeginning");

                    dataContext.getQualityCheckRulesAndFileSheets()
                    .success(function (data) {
                        isLoading(false);
                        $.each(data.ColumnRules || [], function (index, cr) {
                            var columnRule = new dataContext.columnRule(cr);
                            columnRules.push(columnRule);
                        });

                        $.each(data.FileSheets || [], function (index, fs) {
                            var fileSheet = new dataContext.fileSheet(fs)
                            fileSheets.push(fileSheet);
                        });

                        if (columnRules().length > 0) {
                            selectedColumnRule(columnRules()[0]);
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
                    getQualityCheckRulesAndFileSheets();
                }

                initializeViewModel();

                return viewModel;
            })(ko, widget);
        },

        _destroy: function () {
            this.qualityCheckHtml.remove();
        }
    });
})(jQuery);
