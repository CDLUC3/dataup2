(function ($, undefined) {
    metadataMessages = {
        fileLevelMetadata: "File Metadata",
        columnLevelMetadata: "Field Metadata"
    };

    $.widget("dataup.metadata", {
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
            saveCompleted: $.noop,
            tabChanged: $.noop
        },

        validate: function () {
            var widget = this;
            var fileLevelMetadataWidget = widget.fileLevelMetadataHtml.data('dataup-fileLevelMetadata');
            if (fileLevelMetadataWidget != undefined) {
                var isValidMetaData = fileLevelMetadataWidget.validate();
                if (!isValidMetaData) {
                    widget.showFileLevelMetadataTab(widget);
                    return false;
                }
            }

            var columnLevelMetadataWidget = widget.columnLevelMetadataHtml.data('dataup-columnLevelMetadata');
            if (columnLevelMetadataWidget != undefined) {
                var isValidMetaData = columnLevelMetadataWidget.validate()
                if (!isValidMetaData) {
                    widget._showColumnLevelMetadataTab(widget);
                    return false;
                }
            }

            return true;
        },

        hasUnsavedChanges: function () {
            var widget = this;

            var fileLevelMetadataWidget = widget.fileLevelMetadataHtml.data('dataup-fileLevelMetadata');
            if (fileLevelMetadataWidget != undefined) {
                var hasUnsavedChanges = fileLevelMetadataWidget.hasUnsavedChanges();
                if (hasUnsavedChanges) {
                    return true;
                }
            }

            var columnLevelMetadataWidget = widget.columnLevelMetadataHtml.data('dataup-columnLevelMetadata');
            if (columnLevelMetadataWidget != undefined) {
                var hasUnsavedChanges = columnLevelMetadataWidget.hasUnsavedChanges();
                if (hasUnsavedChanges) {
                    return true;
                }
            }

            return false;
        },

        reload: function () {
            var widget = this;
            var fileLevelMetadataWidget = widget.fileLevelMetadataHtml.data('dataup-fileLevelMetadata');
            if (fileLevelMetadataWidget != undefined) {
                fileLevelMetadataWidget.options.repositoryId = widget.options.repositoryId;
                return fileLevelMetadataWidget.reload();
            }

            return true;
        },

        _create: function () {
            var widget = this;
            if (widget.options.fileApiUri == undefined || widget.options.fileApiUri == null || $.trim(widget.options.fileApiUri) == "") {
                widget._trigger("initializationFailed", null, { message: messages.fileApiUriEmptyErrorMessage });
                return;
            }

            if (widget.options.jwtAuthenticationToken == undefined || widget.options.jwtAuthenticationToken == null || $.trim(widget.options.jwtAuthenticationToken) == "") {
                widget._trigger("initializationFailed", null, { message: messages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            if (widget.options.fileId == undefined || widget.options.fileId == null || widget.options.fileId <= 0) {
                widget._trigger("initializationFailed", null, { message: messages.fileIdInvalidErrorMessage });
                return;
            }

            if (widget.options.repositoryId == undefined || widget.options.repositoryId == null || widget.options.repositoryId <= 0) {
                widget._trigger("initializationFailed", null, { message: messages.repositoryIdInvalidErrorMessage });
                return;
            }

            widget.element.dataUpManager = widget.element.dataUpManager || {};

            widget._initializeDataBindings(widget);
            widget._initializeViewModel(ko, widget);
            widget._initializeChildWidgets(widget);
        },

        _initializeDataBindings: function (widget) {
            widget.metadataHtml = $('<div class="container">\
                    <nav>\
                        <div class="tabNavigation">\
                            <ul>\
                            </ul>\
                        </div>\
                    </nav>\
                </div>').appendTo(widget.element);

            widget.element.addClass('widgetContainer');

            var metadataTabs = widget.metadataHtml.find('ul');

            widget.fileLevelMetadataTab = $('<li class="active"><a href="#" data-bind="html: metadataMessages.fileLevelMetadata, attr: { title: metadataMessages.fileLevelMetadata }, click: showFileLevelMetadataTab"></a></li>');
            metadataTabs.append(widget.fileLevelMetadataTab);

            widget.columnLevelMetadataTab = $('<li><a href="#" data-bind="html: metadataMessages.columnLevelMetadata, attr: { title: metadataMessages.columnLevelMetadata }, click: showColumnLevelMetadataTab"></a></li>');
            metadataTabs.append(widget.columnLevelMetadataTab);

            widget.fileLevelMetadataHtml = $('<div style="display: block"></div>');
            widget.metadataHtml.append(widget.fileLevelMetadataHtml);

            widget.columnLevelMetadataHtml = $('<div style="display: none"></div>');
            widget.metadataHtml.append(widget.columnLevelMetadataHtml);

            widget.element.append(widget.metadataHtml);
        },

        _initializeViewModel: function (ko, widget) {
            this.element.dataUpManager.viewModel = (function (ko, widget) {
                function showFileLevelMetadataTab() {
                    widget._trigger("tabChanged");
                    widget.showFileLevelMetadataTab(widget);
                }

                function showColumnLevelMetadataTab() {
                    widget._trigger("tabChanged");
                    widget._showColumnLevelMetadataTab(widget);
                }

                return {
                    showFileLevelMetadataTab: showFileLevelMetadataTab,
                    showColumnLevelMetadataTab: showColumnLevelMetadataTab
                };
            })(ko, widget);

            ko.applyBindings(this.element.dataUpManager.viewModel, this.metadataHtml[0]);
        },

        _initializeChildWidgets: function (widget) {
            widget.fileLevelMetadataHtml.fileLevelMetadata({
                fileApiUri: widget.options.fileApiUri,
                jwtAuthenticationToken: widget.options.jwtAuthenticationToken,
                fileId: widget.options.fileId,
                repositoryId: widget.options.repositoryId,
                initializationFailed: function () {
                    widget._trigger("initializationFailed");
                },
                validationFailed: function () {
                    widget._trigger("validationFailed");
                },
                loadBeginning: function () {
                    widget._trigger("loadBeginning");
                },
                loadCompleted: function () {
                    widget._trigger("loadCompleted");
                },
                saveBeginning: function () {
                    widget._trigger("saveBeginning");
                },
                saveSucceeded: function (event, result) {
                    widget._trigger("saveSucceeded", null, result);
                },
                saveCompleted: function () {
                    widget._trigger("saveCompleted");
                }
            });
        },

        showFileLevelMetadataTab: function (widget) {
            widget.fileLevelMetadataTab.addClass('active');
            widget.columnLevelMetadataTab.removeClass('active');
            widget.fileLevelMetadataHtml.show();
            widget.columnLevelMetadataHtml.hide();
        },

        _showColumnLevelMetadataTab: function (widget) {
            var columnLevelMetadataWidget = widget.columnLevelMetadataHtml.data('dataup-columnLevelMetadata');
            if (columnLevelMetadataWidget == undefined) {
                widget.columnLevelMetadataHtml.columnLevelMetadata({
                    fileApiUri: widget.options.fileApiUri,
                    jwtAuthenticationToken: widget.options.jwtAuthenticationToken,
                    fileId: widget.options.fileId,
                    initializationFailed: function () {
                        widget._trigger("initializationFailed");
                    },
                    validationFailed: function () {
                        widget._trigger("validationFailed");
                    },
                    loadBeginning: function () {
                        widget._trigger("loadBeginning");
                    },
                    loadCompleted: function () {
                        widget._trigger("loadCompleted");
                    },
                    saveBeginning: function () {
                        widget._trigger("saveBeginning");
                    },
                    saveSucceeded: function (event, result) {
                        widget._trigger("saveSucceeded", null, result);
                    },
                    saveCompleted: function () {
                        widget._trigger("saveCompleted");
                    }
                });
            }

            widget.fileLevelMetadataTab.removeClass('active');
            widget.columnLevelMetadataTab.addClass('active');
            widget.fileLevelMetadataHtml.hide();
            widget.columnLevelMetadataHtml.show();
        },

        _destroy: function () {
            this.metadataHtml.remove();
        }
    });
})(jQuery);
