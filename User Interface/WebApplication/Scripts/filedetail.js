(function ($, undefined) {
    var messages = {
        fileDataDoesNotExist: "File Data does not exist",
    },
    constants = {
        megaByteSize: 1048576,
        kiloByteSize: 1024,
        megaByteUnitName: "MB",
        kiloByteUnitName: "KB",
        bytesUnitName: "Bytes"
    };

    $.widget("dataup.filedetail", {
        options: {
            file: {},
            propagateMessages: false,
            browserVersion : 10.0,
            fileDetailInitializationFailed: $.noop
        },

        _create: function () {

            var fileDetailInitializationFailed = "fileDetailInitializationFailed",
                that = this;

            if (this.options.file == undefined || this.options.file == null) {
                this._trigger(fileDetailInitializationFailed, null, { message: messages.fileDataDoesNotExist });
                return;
            }

            this.element.dataUpManager = this.element.dataUpManager || {};
            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this.element.dataUpManager.dataContext);
        },

        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                };

                return dataContext;
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
            var that = this;
            dataContext.fileModel = fileModel;
            function fileModel(file) {
              
              
                var createdOn = new Date(file.createdOn),
                    publishedOn = new Date(file.publishedOn),
                    timeZoneOffsetInMinutes = new Date().getTimezoneOffset(),
                    self = this;

                if (that.options.browserVersion != -1) {
                    createdOn.setMinutes(createdOn.getMinutes() - (timeZoneOffsetInMinutes));
                    publishedOn.setMinutes(publishedOn.getMinutes() - (timeZoneOffsetInMinutes));
                }

                self.fileId = file.fileId;
                self.name = file.name;
                self.createdOnDate = createdOn.getMonth() + 1 + "/" + createdOn.getDate() + "/" + createdOn.getFullYear();
                self.createdOnTime = createdOn.getHours() + ":" + createdOn.getMinutes() + ":" + createdOn.getSeconds();
                self.publishedOnDate = publishedOn.getMonth() + 1 + "/" + publishedOn.getDate() + "/" + publishedOn.getFullYear();
                self.publishedOnTime = publishedOn.getHours() + ":" + publishedOn.getMinutes() + ":" + publishedOn.getSeconds();
                self.status = file.status;
                self.lifelineInHours = file.lifelineInHours;
                self.repositoryName = file.repository.name || null;
                self.fileSize = formatFileSize(file.fileSize);
                self.identifier = file.identifier;
                self.extension = file.extension;
                self.baseRepositoryId = file.repository.baseRepositoryId || 0;
                self.citation = file.citation;
            }

            function formatFileSize(bytes) {

                if (bytes > constants.megaByteSize) {
                    return (bytes / constants.megaByteSize).toFixed(2).concat(" ", constants.megaByteUnitName);
                }
                else if (bytes > constants.kiloByteSize) {
                    return (bytes / constants.kiloByteSize).toFixed(2).concat(" ", constants.kiloByteUnitName);
                }
                else {
                    return bytes.toString().concat(" ", constants.bytesUnitName);
                }
            }
        },

        _initializeViewModel: function (ko, dataContext) {
            var that = this;

            this.element.dataUpManager.viewModel = (function (ko, dataContext, options) {
                that.fileModel = new dataContext.fileModel(options.file);

                return {
                    fileModel: that.fileModel,
                    respositoryModel: that.respositoryModel
                };

            })(ko, dataContext, this.options);

            ko.applyBindings(this.element.dataUpManager.viewModel, this.detailHtml[0]);
        },

        _initializeDataBindings: function () {

            this.detailHtml = $('<div class="details">\
                            	    <table cellpadding="0" cellspacing="0" border="0" width="100%">\
                                	    <tbody>\
                                    	<tr>\
                                        	<td width="50%">\
                                            	<label>File Name</label>\
                                                <div class="file-name"><span data-bind="title:fileModel.name, text: fileModel.name"></span></div>\
                                            </td>\
                                            <td width="50%" data-bind = "visible:fileModel.repositoryName != null">\
                                            	<label>Repository Name</label>\
                                                <div class="repository-name" data-bind="title:fileModel.repositoryName, text: fileModel.repositoryName , css:{merritt:fileModel.baseRepositoryId==1, skydrive:fileModel.baseRepositoryId==2}"></div>\
                                            </td>\
                                        </tr>\
                                        <tr>\
                                        	<td>\
                                            	<label>File Extension</label>\
                                                <div class="file-extension" data-bind="title:fileModel.extension, text: fileModel.extension"></div>\
                                            </td>\
                                            <td data-bind = "visible:fileModel.status==\'Posted\'">\
                                            	<label>Identifier</label>\
                                                <div class="identifier" data-bind="title:fileModel.identifier, text: fileModel.identifier"></div>\
                                            </td>\
                                        </tr>\
                                        <tr>\
                                        	<td>\
                                            	<label>File Size</label>\
                                                <div class="file-size" data-bind="title:fileModel.fileSize, text: fileModel.fileSize"></div>\
                                            </td>\
                                            <td data-bind = "visible: fileModel.baseRepositoryId == 1 && fileModel.status==\'Posted\'">\
                                            	<label>Citation</label>\
                                                <div class="citation" data-bind="html: fileModel.citation"></div>\
                                            </td>\
                                        </tr>\
                                        <tr>\
                                        	<td data-bind = "visible:fileModel.status==\'Posted\'">\
                                            	<label>Published Date</label>\
                                                <div class="published-date">\
                                                	<time class="date" data-bind="title:fileModel.publishedOnDate, text: fileModel.publishedOnDate"></time>\
                                                    <time class="time" data-bind="title:fileModel.publishedOnTime, text: fileModel.publishedOnTime"></time>\
                                                </div>\
                                            </td>\
                                            <td data-bind = "visible:fileModel.status !=\'Posted\'">\
                                            	<label>Uploaded Date</label>\
                                                <div class="published-date">\
                                                	<time class="date" data-bind="title:fileModel.createdOnDate, text: fileModel.createdOnDate"></time>\
                                                    <time class="time" data-bind="title:fileModel.createdOnTime, text: fileModel.createdOnTime"></time>\
                                                </div>\
                                            </td>\
                                            <td>\
                                            	<label>Status</label>\
                                                <div class="citation" data-bind = "text:fileModel.status, title:fileModel.status"></div>\
                                            </td>\
                                        </tr>\
                                    </tbody>\
                                </table>\
                            </div>').appendTo(this.element);
        },

        _destroy: function () {
            this.detailHtml.remove();
        }
    });
})(jQuery);
