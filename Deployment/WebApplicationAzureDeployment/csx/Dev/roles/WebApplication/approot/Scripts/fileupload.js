(function ($, undefined) {
    var messages = {

        fileUploadSuccessful: "Uploaded Sucessfully",
        fileUploadFialed: "Error while uploading the file",
        fileSizeLimitExceeded: "File with size more than 100MB can't be uploaded",
        emptyFile: "Empty files can't be uploaded",
        configurationDoesNotExits: "API ConfigurationInformation does not exist",
        jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
        duplicateFileListMessage: "Following file(s) already exists",
        duplicateFilesActionMessage: "Do you want to overwrite?"
    }

    $.widget("dataup.fileupload", {
        options: {
            jwtAuthenticationToken: "",
            configuration: {},
            existingFilesList: [],
            propagateMessages: false,
            uploadStart: $.noop,
            uploadSuccessful: $.noop,
            uploadFailed: $.noop,
            uploadAborted: $.noop,
            fileUploadInitializationFailed: $.noop
        },

        _create: function () {

            var fileUploadInitializationFailed = "fileUploadInitializationFailed",
                that = this;

            if (this.options.configuration == undefined || this.options.configuration == null || this.options.configuration.length <= 0) {
                this._trigger(fileUploadInitializationFailed, null, { message: messages.configurationDoesNotExits });
                return;
            }

            if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
                this._trigger(fileUploadInitializationFailed, null, { message: messages.jwtAuthenticationTokenEmptyErrorMessage });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};
            // this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);

        },

        _init: function () {
            $(this.fileUploadHtml).remove();
            this._initializeDataBindings();
            this._initializeViewModel(ko, this.element.dataUpManager.dataContext);
            this._attachEventHandlersForDropBox();
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                    getFiles: getFiles,
                    uploadFile: uploadFile
                };

                return dataContext;

                function getFiles() {
                    return $.ajax({
                        url: buildfilesUri(),
                        type: "GET",
                        async: true,
                        contentType: "application/json",
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken }
                    });
                }

                function uploadFile(formData, fileName, contentType) {
                    if (contentType.length <= 0) {
                        var fileParts = fileName.split('.');
                        contentType = fileParts.pop();
                    }

                    return $.ajax({
                        url: buildFileUploadUrl(fileName, contentType),
                        type: "POST",
                        data: formData,
                        xhrfields: { withCredentials: false },
                        headers: { Authorization: options.jwtAuthenticationToken },
                        processData: false,
                        contentType: false
                    });
                }

                function buildfilesUri() {
                    return options.configuration.FileApiUri + "/1";
                }

                function buildFileUploadUrl(fileName, contentType) {
                    var fileExtension = (/[^.]+$/.exec(fileName));
                    return options.configuration.BlobApiUri.concat("?filename=" + fileName + "&fileextension=" + fileExtension + "&contenttype=" + contentType);
                }

            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            dataContext.fileDetail = fileDetail;
            dataContext.file = file;

            function fileDetail(file) {
                var self = this;
                self.FileName = file.name;
                self.FileSize = file.size / 1024 / 1024;
                self.Status = file.Status;
            }

            function file(data) {
                var self = this;
                self.name = data.Name;
                self.status = data.Status;
            }


        },

        _initializeViewModel: function (ko, dataContext) {
            var that = this;
            this.element.dataUpManager.viewModel = (function (ko, dataContext, options) {
                var self = this;
                self.fileModel = {
                    UploadingFiles: [],
                    UploadingFileDetails: [],
                };

                self.existingFilesList = options.existingFilesList;

                // event handler for change event of input "file"
                self.startUploading = function (data, event) {
                    var files = event.target.files;
                    jQuery.support.cors = true;
                    uploadFiles(files);
                    //$(this).replaceWith('<input type="file"  multiple="" data-bind = " event: { change: startUploading }" />');
                };

                // event handler for dragenter
                self.dragEnter = function (event) {
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                    event.preventDefault();
                    return false;
                };

                // event handler for dragover
                self.dragOver = function (event) {
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                    event.preventDefault();
                    return false;

                };

                // event handler for dragexit
                self.dragExit = function (event) {
                    if (event.stopPropagation) {
                        event.stopPropagation();
                    }
                    event.preventDefault();
                    return false;
                };

                // event handler for drop.
                self.drop = function (event) {
                    event.stopPropagation();
                    event.preventDefault();
                    var files = event.dataTransfer.files;
                    if (files) {
                        uploadFiles(files);
                    }

                    return false;
                };

                self.errorMessage = ko.observable("");
                self.informationMessage = ko.observable("");

                self.selectFileHandler = function (data, event) {

                    // $(event.target).find("#fileInput")[0].click();

                    var elem = document.getElementById("fileInput");
                    if (typeof elem.click == "function") {
                        elem.click.apply(elem);
                    };
                };

                // called from the drop and startUploading file event handlers
                function uploadFiles(files) {
                    uploadStart();
                    if (files) {
                        if (files != undefined && files.length > 0) {
                            $.each(files, function () {
                                pushFileToArrays(this);
                            });
                        }

                        if (checkFileSize(fileModel.UploadingFileDetails)) {
                            validateandUploadFiles(fileModel.UploadingFiles, fileModel.UploadingFileDetails);
                        }
                    }
                }

                // add the file to array
                function pushFileToArrays(file) {
                    self.fileModel.UploadingFiles.push(file);
                    self.fileModel.UploadingFileDetails.push(new dataContext.fileDetail(file));
                }

                // gets the existing files which will be used to check if the file already exists
                function getExistingFileList() {
                    var def = $.Deferred();
                    if (self.existingFilesList.length <= 0) {
                        dataContext.getFiles()
                                   .success(function (result) {
                                       $.each(result, function (index, file) {
                                           self.existingFilesList.push(new dataContext.file(file));
                                       });
                                   })
                                   .complete(function () {
                                       def.resolve(options.existingFilesList);
                                   });
                    } else {
                        def.resolve(self.existingFilesList);
                    }

                    return def;
                }

                function checkFileSize(filedetails) {
                    var isFileSizeOutOfRange = false;

                    $.each(filedetails, function () {
                        if (this.FileSize <= 0) {
                            isFileSizeOutOfRange = true;
                            alert(messages.emptyFile);
                            return false;
                        }
                        if (this.FileSize > 100) {
                            isFileSizeOutOfRange = true;
                            alert(messages.fileSizeLimitExceeded);
                            return false;
                        }
                    });

                    if (isFileSizeOutOfRange) {
                        clearArrays();
                        uploadAborted();
                        return false;
                    }
                    else {
                        return true;
                    }
                }

                // validates the file and uploads the file
                function validateandUploadFiles(uploadingfiles, filedetails) {
                    getExistingFileList().done(function () {
                        var duplicatefiles = [];
                        $.each(filedetails, function () {
                            if (checkIfFileExists(this.FileName)) {
                                if (duplicatefiles.indexOf(this.FileName.toLowerCase()) < 0) {
                                    duplicatefiles.push(this.FileName);
                                }
                            }
                        });
                        if (duplicatefiles != undefined && duplicatefiles.length > 0) {
                            var AlertMessage = messages.duplicateFileListMessage + "\n";
                            $.each(duplicatefiles, function () {
                                AlertMessage += this + "\n";
                            });
                            AlertMessage += "\n" + messages.duplicateFilesActionMessage;
                            if (confirm(AlertMessage)) {
                                upload(uploadingfiles);
                            }
                            else {
                                clearArrays();
                                uploadAborted();
                            }
                        }
                        else {
                            upload(uploadingfiles);
                        }
                    });
                }

                // loops through the files and uploads them
                function upload(files) {
                    var uploadSuccessful = true;
                    waiter = $.Deferred(),
                    len = 0;

                    waiter.done(function () {
                        clearArrays();
                        if (uploadSuccessful) {
                            uploadCompleted();
                        }
                    });

                    waiter.progress(function () {
                        if (--len === 0) {
                            waiter.resolve();
                        }
                    });

                    if (files.length > 0) {
                        len = files.length;
                        $.each(files, function (file) {
                            var formdata = new FormData(),
                                fileName = this.name;
                            formdata.append(this.name, this);
                            dataContext.uploadFile(formdata, this.name, this.type)
                            .fail(function (jqXHR, textStatus, errorThrown) {
                                uploadSuccessful = false;
                                uploadFailed(jqXHR);
                                waiter.notify();
                            })
                            .done(function () {
                                // add the file to existing files list. which can be used to check for the duplicates
                                var existingFile = getFileByName(fileName)

                                // if the file does not already exist in the list then add it to the list
                                if (existingFile == undefined || existingFile == null) {
                                    self.existingFilesList.push({ name: fileName, status: "Uploaded" });
                                }
                                waiter.notify()
                            });
                        });
                    }
                }

                // helper method to search through the existing files list;
                function getFileByName(name) {
                    return $.grep(self.existingFilesList, function (file) {
                        return file.name == name && file.status == "Uploaded";
                    })[0];
                }
                function uploadStart() {
                    that._trigger("uploadStart", null, null);
                }
                function uploadCompleted() {
                    that._trigger("uploadSuccessful", null, { message: messages.fileUploadSuccessful });
                    if (!that.options.propagateMessages) {
                        that.element.dataUpManager.viewModel.errorMessage("");
                        that.element.dataUpManager.viewModel.informationMessage(messages.fileUploadSuccessful);
                    }
                };

                function uploadFailed(jqXHR) {
                    var error = "";
                    if (jqXHR.responseText != undefined && jqXHR.responseText.length > 0) {
                        error = JSON.parse(jqXHR.responseText).Message;
                    }

                    if (error.length == 0) {
                        error = messages.fileUploadFialed;
                    }

                   
                    var statusCode = jqXHR.status;

                    // below check is a workaround for IE10 Issue. The code changes status to 401 when 404 
                    var version = getInternetExplorerVersion();
                    if ((statusCode == 404 || statusCode == 0) && version >= 10.0) {
                        statusCode = 401
                    }

                    that._trigger("uploadFailed", jqXHR, { message: error, status: statusCode });
                    if (!that.options.propagateMessages) {
                        that.element.dataUpManager.viewModel.informationMessage("");
                        that.element.dataUpManager.viewModel.errorMessage(error);
                    }
                }

                function uploadAborted() {
                    that._trigger("uploadAborted", null, null);
                }

                function checkIfFileExists(fileName) {
                    var fileAlreadyExists = false,
                        existingFile = $.grep(self.existingFilesList, function (file) {
                            return (file.name.toLowerCase() == fileName.toLowerCase() && (file.status == "Uploaded" || file.status == "Error"));
                        })[0];

                    if (existingFile != undefined) {
                        fileAlreadyExists = true;
                    }

                    return fileAlreadyExists;
                }

                function clearArrays() {
                    fileModel.UploadingFiles = [];
                    fileModel.UploadingFileDetails = [];
                    $(that.fileUploadHtml).remove();
                    that._initializeDataBindings();
                    ko.applyBindings(that.element.dataUpManager.viewModel, that.fileUploadHtml[0]);
                    that._attachEventHandlersForDropBox();
                }

                return {
                    errorMessage: self.errorMessage,
                    informationMessage: self.informationMessage,
                    startUploading: self.startUploading,
                    dragEnter: self.dragEnter,
                    dragOver: self.dragOver,
                    dragExit: self.dragExit,
                    drop: self.drop,
                    selectFileHandler: self.selectFileHandler
                };

            })(ko, dataContext, this.options);

            ko.applyBindings(this.element.dataUpManager.viewModel, this.fileUploadHtml[0]);
        },

        _initializeDataBindings: function () {
            this.fileUploadHtml = $('<div class="dragndrop" id="dragDropBox"><div class="select-file"><label><a class="button" role="button" title="Select File">Add a New File<input type="file" id="fileInput" multiple data-bind = " event: { change: startUploading }" /></a></label></div>\
                <div class="or">OR</div>\
                <div class="drag-drop-text">Drag &amp; Drop Files Here</div><div class="drag-drop-icon"><img src="/Content/images/i-drag-drop.png"/></div></div>').appendTo(this.element);
        },

        _attachEventHandlersForDropBox: function () {
            var that = this,
                dropBox = document.getElementById("dragDropBox");
            dropBox.addEventListener("dragenter", function (evt) {
                that.element.dataUpManager.viewModel.dragEnter(evt);
            }, false);
            dropBox.addEventListener("dragexit", function (evt) {
                that.element.dataUpManager.viewModel.dragExit(evt);
            }, false);
            dropBox.addEventListener("dragover", function (evt) {
                that.element.dataUpManager.viewModel.dragOver(evt);
            }, false);
            dropBox.addEventListener("drop", function (evt) {
                that.element.dataUpManager.viewModel.drop(evt);
            }, false);
        },

        _destroy: function () {
            this.fileUploadHtml.remove();
        }
    });

})(jQuery);
