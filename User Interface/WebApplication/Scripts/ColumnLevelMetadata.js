(function ($, undefined) {
    columnLevelMetadataMessages = {
        helpText: "If your file has tabular data, you can define the columns in the table. \
        This information can be useful for querying data, creating views on data, merging with other tables \
        and exposing data over web API.",
        fileApiUriEmptyErrorMessage: "fileApiUri cannot be empty.",
        jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
        fileIdInvalidErrorMessage: "fileId should be a number and should be greater than zero.",
        addItem: "Add Field",
        entityName: "Table Name <span style='color: Red'>*</span>",
        entityDescription: "Table Description <span style='color: Red'>*</span>",
        name: "Field Name <span style='color: Red'>*</span>",
        description: "Field Description <span style='color: Red'>*</span>",
        type: "Data Type <span style='color: Red'>*</span>",
        units: "Units",
        "delete": "",
        emptyMetadataMessage: "Click on <b>+Add Field </b>button to continue.",
        numeric: "Numeric",
        save: "Save",
        loadMetadataFromFile: "Load Metadata from File"
    };

    $.widget("dataup.columnLevelMetadata", {
        options: {
            fileApiUri: "",
            jwtAuthenticationToken: "",
            fileId: 0,
            initializationFailed: $.noop,
            validationFailed: $.noop,
            loadBeginning: $.noop,
            loadCompleted: $.noop,
            saveBeginning: $.noop,
            saveSucceeded: $.noop,
            saveCompleted: $.noop
        },

        validate: function () {
            columnLevelMetadataHtmlForm = $(this.columnLevelMetadataHtml[0]);
            var validator = columnLevelMetadataHtmlForm.validate();
            if (columnLevelMetadataHtmlForm.valid()) {
                return true;
            }

            validator.showErrors();
            this._trigger("validationFailed");
            return false;
        },

        hasUnsavedChanges: function () {
            return this.element.dataUpManager.dataContext.hasUnsavedChanges;
        },

        _create: function () {
            if (this.options.fileApiUri == undefined || this.options.fileApiUri == null || $.trim(this.options.fileApiUri) == "") {
                this._trigger("initializationFailed", null, { message: columnLevelMetadataMessages.fileApiUriEmptyErrorMessage });
                return;
            }

            if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
                this._trigger("initializationFailed", null, { message: columnLevelMetadataMessages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            if (this.options.fileId == undefined || this.options.fileId == null || this.options.fileId <= 0) {
                this._trigger("initializationFailed", null, { message: columnLevelMetadataMessages.fileIdInvalidErrorMessage });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};

            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this);
        },

        _initializeDataBindings: function () {
            this.columnLevelMetadataHtml = $('<form>\
                <p data-bind="text: columnLevelMetadataMessages.helpText"></p>\
                <div class="container" data-bind="visible: errorMessage() != null">\
                    <div class="validate">\
                        <span data-bind="text: errorMessage()"></span>\
                    </div>\
                </div>\
                <div data-bind="visible: errorMessage() == null">\
                    <div>\
                        <ul>\
                            <li>\
                                <div>\
                                    <input type="button" class="button l2 save" data-bind="value: columnLevelMetadataMessages.loadMetadataFromFile, click: loadMetadata, attr: { title: columnLevelMetadataMessages.loadMetadataFromFile }" />\
                                </div>\
                            </li>\
                            <li>\
                                <div class="optionLinks">\
                                    <ul>\
                                        <li>\
                                            <a class="button add l2 right" data-bind="html: columnLevelMetadataMessages.addItem, click: addMetadata, attr: { title: columnLevelMetadataMessages.addItem }"></a>\
                                        </li>\
                                    </ul>\
                                </div>\
                            </li>\
                            <li>\
                                <div class="endStripe" data-bind="visible: metadataList().length > 10">\
                                    <input type="button" class="button l2 save" data-bind="value: columnLevelMetadataMessages.save, click: saveMetadata, attr: { title: columnLevelMetadataMessages.save }" />\
                                </div>\
                            </li>\
                        </ul>\
                    </div>\
                    <div class="list noTopPad" style="clear: right; float: none; overflow-x: auto;">\
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">\
                            <thead>\
                                <tr>\
                                    <th data-bind="html: columnLevelMetadataMessages.entityName, attr: { title: columnLevelMetadataMessages.entityName }"></th>\
                                    <th data-bind="html: columnLevelMetadataMessages.entityDescription, attr: { title: columnLevelMetadataMessages.entityDescription }"></th>\
                                    <th data-bind="html: columnLevelMetadataMessages.name, attr: { title: columnLevelMetadataMessages.name }"></th>\
                                    <th data-bind="html: columnLevelMetadataMessages.description, attr: { title: columnLevelMetadataMessages.description }"></th>\
                                    <th data-bind="html: columnLevelMetadataMessages.type, attr: { title: columnLevelMetadataMessages.type }"></th>\
                                    <th data-bind="html: columnLevelMetadataMessages.units, attr: { title: columnLevelMetadataMessages.units }"></th>\
                                    <th data-bind="html: columnLevelMetadataMessages.delete, attr: { title: columnLevelMetadataMessages.delete }"></th>\
                                </tr>\
                                <tr data-bind="visible: metadataList().length == 0">\
                                    <td colspan="7">\
                                        <div data-bind="html: columnLevelMetadataMessages.emptyMetadataMessage"></div>\
                                    </td>\
                                </tr>\
                            </thead>\
                            <tbody class="paramtrTableContainer" data-bind="foreach: metadataList">\
                                <tr>\
                                    <td>\
                                        <div>\
                                            <select data-bind="options: parent.sheetList, optionsValue: \'name\', optionsText: \'name\', value: selectedEntityName, uniqueName: true"></select>\
                                        </div>\
                                    </td>\
                                    <td>\
                                        <input required type="text" data-bind="value: entityDescription, uniqueName: true" class="entityDescription" style="width: 208px;" />\
                                    </td>\
                                    <td>\
                                        <input required type="text" data-bind="value: name, uniqueName: true" />\
                                    </td>\
                                    <td>\
                                        <input required type="text" data-bind="value: description, uniqueName: true" />\
                                    </td>\
                                    <td>\
                                        <div>\
                                            <select data-bind="options: parent.typeList, optionsValue: \'id\', optionsText: \'name\', value: selectedTypeId, uniqueName: true"></select>\
                                        </div>\
                                    </td>\
                                    <td>\
                                        <div>\
                                            <select data-bind="options: parent.unitList, optionsValue: \'id\', optionsText: \'name\', value: selectedUnitId, optionsCaption: \'\', enable: selectedTypeId() == 3, css: { valid: selectedTypeId() == 3, disable: selectedTypeId() != 3 }, uniqueName: true"></select>\
                                            <input required type="text" class="unitsText disable" readonly="true" style="display: none;" data-bind="value: selectedUnitId, enable: selectedTypeId() == 3, uniqueName: true" />\
                                        </div>\
                                    </td>\
                                    <td class="actions">\
                                        <ul>\
                                            <li class="delete">\
                                                <a href="#" data-bind="click: parent.deleteMetadata, attr: { title: columnLevelMetadataMessages.delete }"></a>\
                                            </li>\
                                        </ul>\
                                    </td>\
                                </tr>\
                            </tbody>\
                        </table>\
                    </div>\
                    <div class="endStripe" data-bind="visible: metadataList().length > 0">\
                        <input type="button" class="button l2 save" data-bind="value: columnLevelMetadataMessages.save, click: saveMetadata, attr: { title: columnLevelMetadataMessages.save }" />\
                    </div>\
                </div>\
            </form>').appendTo(this.element);

            this.element.addClass('widgetContainer');
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                    getColumnLevelMetadata: getColumnLevelMetadata,
                    saveColumnLevelMetadata: saveColumnLevelMetadata,
                    getColumnLevelMetadataFromFile: getColumnLevelMetadataFromFile
                };

                return dataContext;

                function getColumnLevelMetadata() {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/columnlevelmetadata",
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }

                function saveColumnLevelMetadata(metadataList) {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/savecolumnlevelmetadata",
                        type: "POST",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken },
                        datatype: "json",
                        data: JSON.stringify(metadataList)
                    });
                }

                function getColumnLevelMetadataFromFile() {
                    return $.ajax({
                        url: options.fileApiUri + options.fileId + "/getcolumnlevelmetadatafromfile",
                        type: "GET",
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            dataContext.sheet = sheet;
            dataContext.type = type;
            dataContext.unit = unit;
            dataContext.metadata = metadata;
            dataContext.hasUnsavedChanges = false;

            function sheet(data) {
                var self = this;
                self.name = data.SheetName;
            }

            function type(data) {
                var self = this;
                self.id = data.FileColumnTypeId;
                self.name = data.Name;
            }

            function unit(data) {
                var self = this;
                self.id = data.FileColumnUnitId;
                self.name = data.Name;
            }

            function metadata(data, parent) {
                var self = this;
                self.id = 0;
                self.selectedEntityName = ko.observable(parent.sheetList[0].name);
                self.entityDescription = ko.observable("");
                self.name = ko.observable("");
                self.description = ko.observable("");
                self.selectedTypeId = ko.observable();
                self.selectedUnitId = ko.observable();
                self.parent = parent;

                if (data == null) {
                    var textTypes = $.grep(parent.typeList, function (t, i) {
                        return t.name == "Text"
                    });

                    if (textTypes.length > 0) {
                        self.selectedTypeId = ko.observable(textTypes[0].id);
                    }
                }
                else {
                    self.id = data.Id;
                    self.selectedEntityName(data.SelectedEntityName);
                    self.entityDescription(data.EntityDescription);
                    self.name(data.Name);
                    self.description(data.Description);
                    self.selectedTypeId(data.SelectedTypeId);
                    self.selectedUnitId(data.SelectedUnitId);
                }
            }
        },

        _initializeViewModel: function (ko, widget) {
            this.element.dataUpManager.viewModel = (function (ko, widget) {
                var dataContext = widget.element.dataUpManager.dataContext;
                var options = widget.options;
                var columnLevelMetadataHtml = widget.columnLevelMetadataHtml[0];
                var columnLevelMetadataHtmlForm = $(columnLevelMetadataHtml);
                columnLevelMetadataHtmlForm.validate({ ignore: [] });

                var sheetList = [];
                var typeList = [];
                var unitList = [];
                var metadataList = ko.observableArray([]);
                var errorMessage = ko.observable(null);

                function addMetadata() {
                    metadataList.push(new dataContext.metadata(null, viewModel));
                }

                function deleteMetadata(metadata) {
                    metadataList.remove(metadata);
                }

                function saveMetadata() {
                    if (!widget.validate()) {
                        return;
                    }

                    widget._trigger("saveBeginning");
                    saveMetadataList = [];
                    $.map(metadataList(), function (clm, index) {
                        saveMetadataList.push({
                            Id: clm.id,
                            SelectedEntityName: clm.selectedEntityName(),
                            EntityDescription: clm.entityDescription(),
                            Name: clm.name(),
                            Description: clm.description(),
                            SelectedTypeId: clm.selectedTypeId(),
                            SelectedUnitId: clm.selectedTypeId() == 3 ? clm.selectedUnitId() : null
                        });
                    });

                    dataContext.saveColumnLevelMetadata(saveMetadataList)
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
                        errorMessage(httpError.Message);
                    })
                    .complete(function (jqXHR, textStatus) {
                        widget._trigger("saveCompleted");
                    });
                }

                function loadMetadata()
                {
                    if (metadataList().length > 0) {
                        var alertMessage = "\n" + "You have Field Metadata table populated with values. By reloading metadata from the file any unsaved changes would be lost. Do you wish to continue?";
                        if (confirm(alertMessage) == false) {
                            return;
                        }
                        else {
                            dataContext.hasUnsavedChanges = true;
                        }
                    }

                    
                    metadataList([]);
                    typeList = [];
                    unitList = [];
                    sheetList = []

                    dataContext.getColumnLevelMetadataFromFile()
                    .success(function (data) {
                        $.each(data.SheetList || [], function (index, s) {
                            sheetList.push(new dataContext.sheet(s));
                        });

                        $.each(data.TypeList || [], function (index, t) {
                            typeList.push(new dataContext.type(t));
                        });

                        $.each(data.UnitList || [], function (index, u) {
                            unitList.push(new dataContext.unit(u));
                        });

                        $.each(data.MetadataList || [], function (index, m) {
                            metadataList.push(new dataContext.metadata(m, viewModel));
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
                        $.each(viewModel.metadataList(), function (index, m) {
                            m.selectedEntityName.subscribe(fieldEditedHandler);
                            m.entityDescription.subscribe(fieldEditedHandler);
                            m.name.subscribe(fieldEditedHandler);
                            m.description.subscribe(fieldEditedHandler);
                            m.selectedTypeId.subscribe(fieldEditedHandler);
                            m.selectedUnitId.subscribe(fieldEditedHandler);
                        });
                    });
                    
                }

                var viewModel = {
                    sheetList: sheetList,
                    typeList: typeList,
                    unitList: unitList,
                    metadataList: metadataList,
                    errorMessage: errorMessage,
                    addMetadata: addMetadata,
                    deleteMetadata: deleteMetadata,
                    saveMetadata: saveMetadata,
                    loadMetadata: loadMetadata
                };

                ko.applyBindings(viewModel, columnLevelMetadataHtml);

                function fieldEditedHandler() {
                    dataContext.hasUnsavedChanges = true;
                }

                function getColumnLevelMetadata() {
                    widget._trigger("loadBeginning");

                    dataContext.getColumnLevelMetadata()
                    .success(function (data) {
                        $.each(data.SheetList || [], function (index, s) {
                            sheetList.push(new dataContext.sheet(s));
                        });

                        $.each(data.TypeList || [], function (index, t) {
                            typeList.push(new dataContext.type(t));
                        });

                        $.each(data.UnitList || [], function (index, u) {
                            unitList.push(new dataContext.unit(u));
                        });

                        $.each(data.MetadataList || [], function (index, m) {
                            metadataList.push(new dataContext.metadata(m, viewModel));
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
                        $.each(viewModel.metadataList(), function (index, m) {
                            m.selectedEntityName.subscribe(fieldEditedHandler);
                            m.entityDescription.subscribe(fieldEditedHandler);
                            m.name.subscribe(fieldEditedHandler);
                            m.description.subscribe(fieldEditedHandler);
                            m.selectedTypeId.subscribe(fieldEditedHandler);
                            m.selectedUnitId.subscribe(fieldEditedHandler);
                        });

                        widget._trigger("loadCompleted");
                    });
                }

                function initializeViewModel() {
                    getColumnLevelMetadata();
                }

                initializeViewModel();

                return viewModel;
            })(ko, widget);
        },

        _destroy: function () {
            this.columnLevelMetadataHtml.remove();
        }
    });
})(jQuery);