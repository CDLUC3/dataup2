(function ($, undefined) {
    fileLevelMetadataMessages = {
        helpText: "Some repositories may require additional metadata  \
        to be persisted along with the data. Following section(s) \
        cover required and optional fields that need to be filled in \
        before submitting the file to the selected repository.",
        fileApiUriEmptyErrorMessage: "fileApiUri cannot be empty.",
        jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
        fileIdInvalidErrorMessage: "fileId should be a number and should be greater than zero.",
        repositoryIdInvalidErrorMessage: "repositoryId should be a number and should be greater than zero.",
        fileLevelMetadata: "File Level Metadata",
        unsavedChangesWhileReloading: "You have unsaved changes in File Level Metadata. Do you want to continue?",
        save: "Save"
    };

    $.widget("dataup.fileLevelMetadata", {
        options: {
            fileApiUri: "",
            jwtAuthenticationToken: "",
            fileId: 0,
            repositoryId: 0,
            initializationFailed: $.noop,
            validationFailed: $.noop,
            loadBeginning: $.noop,
            loadCompleted: $.noop,
            saveBeginning: $.noop,
            saveSucceeded: $.noop,
            saveCompleted: $.noop
        },

        validate: function () {
            fileLevelMetadataHtmlForm = $(this.fileLevelMetadataHtml[0]);
            var validator = fileLevelMetadataHtmlForm.validate();
            if (fileLevelMetadataHtmlForm.valid()) {
                return true;
            }

            validator.showErrors();
            this._trigger("validationFailed");
            return false;
        },

        hasUnsavedChanges: function () {
            return this.element.dataUpManager.dataContext.hasUnsavedChanges;
        },

        reload: function () {
            var widget = this;
            var dataContext = widget.element.dataUpManager.dataContext;
            if (dataContext.hasUnsavedChanges) {
                if (!window.confirm(fileLevelMetadataMessages.unsavedChangesWhileReloading)) {
                    return false;
                }
            }

            var viewModel = widget.element.dataUpManager.viewModel
            widget._trigger("loadBeginning");

            viewModel.fileLevelMetadataList.removeAll();
            dataContext.getFileLevelMetadata()
            .success(function (data) {
                viewModel.errorMessage(null);
                dataContext.hasUnsavedChanges = false;
                $.each(data || [], function (index, flm) {
                    var fileLevelMetadata = new dataContext.fileLevelMetadata(flm)
                    viewModel.fileLevelMetadataList.push(fileLevelMetadata);
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                if (checkStatusAndRedirectToLandingPage(jqXHR.status)) {
                    return;
                }
                var httpError = $.parseJSON(jqXHR.responseText);
                viewModel.errorMessage(httpError.Message);
            })
            .complete(function (jqXHR, textStatus) {
                $.each(viewModel.fileLevelMetadataList(), function (index, flm) {
                    flm.fieldValue.subscribe(function () {
                        dataContext.hasUnsavedChanges = true;
                    });
                });

                widget._trigger("loadCompleted");
            });

            return true;
        },

        _create: function () {
            if (this.options.fileApiUri == undefined || this.options.fileApiUri == null || $.trim(this.options.fileApiUri) == "") {
                this._trigger("initializationFailed", null, { message: fileLevelMetadataMessages.fileApiUriEmptyErrorMessage });
                return;
            }

            if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
                this._trigger("initializationFailed", null, { message: fileLevelMetadataMessages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            if (this.options.fileId == undefined || this.options.fileId == null || this.options.fileId <= 0) {
                this._trigger("initializationFailed", null, { message: fileLevelMetadataMessages.fileIdInvalidErrorMessage });
                return;
            }

            if (this.options.repositoryId == undefined || this.options.repositoryId == null || this.options.repositoryId <= 0) {
                this._trigger("initializationFailed", null, { message: fileLevelMetadataMessages.repositoryIdInvalidErrorMessage });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};

            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this);
        },

        _initializeDataBindings: function () {
            this.fileLevelMetadataHtml = $('<form>\
                    <p data-bind="visible: fileLevelMetadataList().length > 0, text: fileLevelMetadataMessages.helpText"></p>\
                    <div class="container" data-bind="visible: errorMessage() != null">\
                        <div class="validate">\
                            <span data-bind="text: errorMessage()"></span>\
                        </div>\
                    </div>\
                    <div class="endStripe" data-bind="visible: fileLevelMetadataList().length > 10">\
                        <input type="button" class="button l2 save" data-bind="value: fileLevelMetadataMessages.save, click: save, attr: { title: fileLevelMetadataMessages.save }" />\
                    </div>\
                    <div class="list" data-bind="visible: fileLevelMetadataList().length > 0">\
                        <table cellpadding="0" cellspacing="0" border="0" width="100%">\
                            <tbody data-bind="foreach: fileLevelMetadataList">\
                                <tr>\
                                    <td>\
                                        <label data-bind="html: fieldName"></label>\
                                    </td>\
                                    <td>\
                                        <input type="text" data-bind="value: fieldValue, uniqueName: true, attr: { required: isRequired, number: metaDataTypeId == 2, email: metaDataTypeId == 3, validPhone: metaDataTypeId == 5, validDate: metaDataTypeId == 6 }, attrIf: { range: rangeValues, _if: metaDataTypeId == 4 }" />\
                                        <a class="tTipQ" data-bind="attr: { title: description }"></a>\
                                        <span class="notification" data-bind="visible: datatype == 4, text: \'Values between \' + rangeValues"></span>\
                                    </td>\
                                </tr>\
                            </tbody>\
                        </table>\
                    </div>\
                    <div class="endStripe" data-bind="visible: fileLevelMetadataList().length > 0">\
                        <input type="button" class="button l2 save" data-bind="value: fileLevelMetadataMessages.save, click: save, attr: { title: fileLevelMetadataMessages.save }" />\
                    </div>\
                </form>').appendTo(this.element);

            this.element.addClass('widgetContainer');
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                    getFileLevelMetadata: getFileLevelMetadata,
                    saveFileLevelMetadata: saveFileLevelMetadata
                };

                return dataContext;

                function getFileLevelMetadata() {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/filelevelmetadata?repositoryId=" + options.repositoryId,
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }

                function saveFileLevelMetadata(saveFileLevelMetadataList) {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/savefilelevelmetadata?repositoryId=" + options.repositoryId,
                        type: "POST",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken },
                        datatype: "json",
                        data: JSON.stringify(saveFileLevelMetadataList)
                    });
                }
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            dataContext.fileLevelMetadata = fileLevelMetadata;
            dataContext.hasUnsavedChanges = false;

            function fileLevelMetadata(data) {
                var self = this;
                self.fileMetadataFieldId = data.FileMetadataFieldId;
                self.repositoryMetadataFieldId = data.RepositoryMetadataFieldId;
                self.repositoryMetadataId = data.RepositoryMetadataId;
                self.metaDataTypeId = data.MetaDataTypeId;
                self.fieldName = data.FieldName + (data.IsRequired == true ? '<span class="mandatory">*</span>' : '');
                self.description = data.Description;
                self.datatype = data.Datatype;
                self.isRequired = data.IsRequired;
                self.rangeValues = data.RangeValues != null ? data.RangeValues : [0, 0];
                self.fieldValue = ko.observable(data.FieldValue);
            }
        },

        _initializeViewModel: function (ko, widget) {
            this.element.dataUpManager.viewModel = (function (ko, widget) {
                var dataContext = widget.element.dataUpManager.dataContext;
                var options = widget.options;
                var fileLevelMetadataHtml = widget.fileLevelMetadataHtml[0];
                var fileLevelMetadataHtmlForm = $(fileLevelMetadataHtml);
                fileLevelMetadataHtmlForm.validate({ ignore: [] });

                var fileLevelMetadataList = ko.observableArray();
                var errorMessage = ko.observable(null);

                function save() {
                    if (!widget.validate()) {
                        return;
                    }

                    widget._trigger("saveBeginning");

                    saveFileLevelMetadataList = [];
                    $.map(fileLevelMetadataList(), function (flm, index) {
                        saveFileLevelMetadataList.push({ FileMetadataFieldId: flm.fileMetadataFieldId, RepositoryMetadataFieldId: flm.repositoryMetadataFieldId, FieldValue: flm.fieldValue() });
                    });

                    dataContext.saveFileLevelMetadata(saveFileLevelMetadataList)
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
                    fileLevelMetadataList: fileLevelMetadataList,
                    errorMessage: errorMessage,
                    save: save
                };

                ko.applyBindings(viewModel, fileLevelMetadataHtml);

                function getFileLevelMetadata() {
                    widget._trigger("loadBeginning");

                    dataContext.getFileLevelMetadata()
                    .success(function (data) {
                        $.each(data || [], function (index, flm) {
                            var fileLevelMetadata = new dataContext.fileLevelMetadata(flm)
                            fileLevelMetadataList.push(fileLevelMetadata);
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
                        $.each(viewModel.fileLevelMetadataList(), function (index, flm) {
                            flm.fieldValue.subscribe(function () {
                                dataContext.hasUnsavedChanges = true;
                            });
                        });

                        widget._trigger("loadCompleted");
                    });
                }

                function initializeViewModel() {
                    getFileLevelMetadata();
                }

                initializeViewModel();

                return viewModel;
            })(ko, widget);
        },

        _destroy: function () {
            this.fileLevelMetadataHtml.remove();
        }
    });
})(jQuery);