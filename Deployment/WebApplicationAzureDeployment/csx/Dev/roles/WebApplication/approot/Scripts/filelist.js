(function ($, undefined) {
	messages = {
		operationSuccessfull: "Action completed Successfully",
		operationFailed: "Error occured while completing the current operation",
		configurationDoesNotExits: "API ConfigurationInformation does not exist",
		jwtAuthenticationTokenEmptyErrorMessage: "jwtAuthenticationToken cannot be empty.",
		deleteAlertMessage: "Are you sure you want to delete this file?",
	    NoFilesViewMessage: "You do not have any files to view. Start by uploading your file(s)."
	};

	constants = {
		Status_Posted: "Posted",
		Status_Uploaded: "Uploaded",
		Status_Error: "Error",
		Status_InQueue: "Inqueue",
		Status_Verifying: "Verifying",
		Status_None: "None",
		Filter_Uploaded: 1,
		Filter_Published: 2,
		Filter_ALL: 3,
		Sort_By_DateUploaded: 1,
		Sort_By_DatePublished: 2,
		Sort_By_FileName: 3,
		Sort_By_FileType: 4,
		Repository_Merritt: 1,
		Repository_SkyDrive: 2,
		
	};

	actions = {
		downloadFileFromRepository: "downloadFileFromRepository",
		downloadFile: "downloadFile",
		postFile: "postFile",
		deleteFile: "deleteFile",
		downloadFromBlob: "downloadFromBlob",
		showFileDetails: "showFileDetails",
		getFiles: "getFiles",
		getRepositories: "getRepositories",
		refreshFiles: "refreshFiles",
		applyState: "applyState",
		initialize: "initialize",
		checkCredentials: "checkCredentials"
	}

	triggers = {
		getCredentials: "getCredentials",
		postFile: "postFile",
		operationStart: "operationStart",
		operationCompleted: "operationCompleted",
		operationFailed: "operationFailed",
		
	};

	$.widget("dataup.filelist", {
		options: {
			jwtAuthenticationToken: "",
			configuration: {},
			state: {},
			filterValue: constants.Filter_ALL,
			sortValue: 1,
			FileListInitializationFailed: $.noop,
			fileSelected: $.noop,
			getCredentials: $.noop,
			postFile: $.noop,
			operationCompleted: $.noop,
			operationFailed: $.noop,
			fileListInitializationFailed: $.noop,
			propagateMessages: false
		},

		_create: function () {
			if (this.options.state == null) {
				this.options.state = {};
			}

			var that = this;

			var fileListInitializationFailed = "fileListInitializationFailed",
			   that = this;

			if (this.options.configuration == undefined || this.options.configuration == null || this.options.configuration.length <= 0) {
				this._trigger(fileListInitializationFailed, null, { message: messages.configurationDoesNotExits });
				return;
			}

			if (this.options.jwtAuthenticationToken == undefined || this.options.jwtAuthenticationToken == null || $.trim(this.options.jwtAuthenticationToken) == "") {
				this._trigger(fileListInitializationFailed, null, { message: messages.jwtAuthenticationTokenEmptyErrorMessage });
				return;
			}

			this.element.dataUpManager = this.element.dataUpManager || {};
		
			this._initializeDataContext();
			this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
		},

		_init: function () {
		    $(this.fileListHtml).remove();
		    this._initializeDataBindings();
		    this._initializeViewModel(ko, this.element.dataUpManager.dataContext);
		    this._attachEventsToDropdowns();
		},

		_initializeDataBindings: function () {
			var that = this;
			// binding for tile view
			function intializeTileViewBinding() {
				that.fileListHtml = $('<div class="filelist" >\
										<div class="action-bar">\
                                         <div class="title"><h2>My Files</h2></div>\
										 <div id=\"userMessagesPanel\">\
											<p  data-bind=\"text: errorMessage, visible:errorMessage().length > 0\" \>\
											<p  data-bind=\"text: informationMessage, visible:informationMessage().length > 0\" \>\
										 </div>\
										<div class="view-options">\
											<div class="view-by">\
												<ul>\
													<li><span class="current-selection" data-bind="attr:{title:selectedFilter().name}, text: selectedFilter().name"></span>\
														<ul data-bind="foreach: filterValueList">\
															<li><a href="#" data-bind="attr:{title:name}, text:name, click:filterActionHandler"></a></li>\
														</ul>\
													</li>\
												</ul>\
											</div>\
											<div class="sort-by">\
												<ul>\
													<li><span class="current-selection" data-bind="attr:{title:selectedSort().name}, text: selectedSort().name"></span>\
														<ul  data-bind="foreach: sortValueList">\
															<li><a href="#" data-bind="attr:{title:name}, text:name, click:sortActionHandler"></a></li>\
														</ul>\
													</li>\
												</ul>\
											</div>\
											<nav class="view-mode">\
												<ul>\
													<li class="side-list-view"><a href="#" title="Side Listed View" role="link">Side Listed View</a></li>\
													<li class="tile-view active"><a href="#" title="Tile View" role="link">Tile View</a></li>\
													<li class="accordion-view"><a href="#" title="Accordion View" role="link">Accordion View</a></li>\
												</ul>\
											</nav>\
										</div>\
										<nav class="action-links">\
											<ul>\
                                                <!-- ko if: selectedFile().status == \'' + constants.Status_Uploaded + '\' || selectedFile().status == \'' + constants.Status_Error + '\' -->\
												<li class="post">\
                                                        <a href="#" title="Post" role="link" class="label dropdown">Post</a>\
													    <ul data-bind="foreach: filteredRepositoryList, visible: filteredRepositoryList().length > 0 && selectedFile() != null">\
														    <li>\
                                                                <a class="repositoryList" href="#" role="link" data-bind="attr:{title:name}, text: name, click:postFileActionHandler"></a>\
                                                            </li>\
													    </ul>\
												</li>\
                                                <!-- /ko -->\
                                                <!-- ko ifnot: selectedFile().status == \'' + constants.Status_Uploaded + '\' || selectedFile().status == \'' + constants.Status_Error + '\' -->\
                                                <li class="disabledPost">\
                                                   <a href="#" title="Post" class="label">Post</a>\
                                                </li>\
                                                <!-- /ko -->\
                                                <!-- ko if: selectedFile().status != \'' + constants.Status_None + '\' -->\
                                                <li class="download">\
                                                    <a href="#" title="Download" role="link" data-bind="click:downloadFileActionHandler, visible: selectedFile() != null" class="label" >Download</a>\
                                                </li>\
                                                <!-- /ko -->\
                                                <!-- ko if: selectedFile().status == \'' + constants.Status_None + '\' -->\
                                                <li class="disabledDownload">\
                                                    <a href="#" title="Download" class="label" >Download</a>\
                                                </li>\
                                                <!-- /ko -->\
                                                <!-- ko if: selectedFile().status == \'' + constants.Status_Uploaded + '\' || selectedFile().status == \'' + constants.Status_Error + '\' -->\
                                                <li class="delete" data-bind="click:deleteFile">\
                                                    <a href="#" title="Delete" role="link" class="label">Delete</a>\
                                                </li>\
                                                <!-- /ko -->\
                                                <!-- ko ifnot: selectedFile().status == \'' + constants.Status_Uploaded + '\' || selectedFile().status == \'' + constants.Status_Error + '\' -->\
                                                <li class="disabledDelete">\
                                                    <a href="#" title="Delete" class="label">Delete</a>\
                                                </li>\
                                                <!-- /ko -->\
											</ul>\
										</nav>\
									</div>\
									<div id="fileListWrapper" class="file-list">\
										<ul>\
                                            <li class="file">\
                                                <div class="addFileTile">\
                                                    <div class="upload-placeholder" id="fileUpload"></div>\
                                                </div>\
                                            </li>\
                                            <!-- ko if: filteredFileList().length > 0 -->\
                                            <!-- ko foreach: filteredFileList -->\
											<li class = "file" data-bind="css:{uploaded:status == \'' + constants.Status_Uploaded + '\', published:status == \'' + constants.Status_Posted + '\',active: fileId() == selectedFile().fileId }, click:showDetail, attr:{fileId:fileId()}">\
												<div class="tile">\
													<div class="file-name"><span data-bind="attr:{title:name}, text:name"></span><span class="extension"  data-bind="title:extension, text: extension" ></span></div>\
													<div class="lifeline-indicator" data-bind="visible: (status == \'' + constants.Status_Uploaded + '\' || status == \'' + constants.Status_Error + '\')  && lifelineInHours != null "><span>Expires in </span><span class="hours-left" data-bind="attr:{title:lifelineInHours}, text: lifelineInHours"></span><span>hrs.</span></div>\
													<div class="repository-name" data-bind="attr:{title:repository.name}, text: repository.name, visible:status !=\'' + constants.Status_Uploaded + '\' && status !=\'' + constants.Status_Error + '\' ">\
													</div>\
													 <img src="/Content/images/tick-icon01.png" alt="Published" class="status" data-bind="visible:status==\'' + constants.Status_Posted + '\'" />\
												</div>\
												<div class="active-indicator"></div>\
											</li>\
                                            <!-- /ko -->\
                                            <!-- /ko -->\
										</ul>\
									</div>\
								</div>').appendTo(that.element);
			}

			// intialize the tile view binding
			intializeTileViewBinding();
		 },
			

		_initializeDataContext: function () {
			this.element.dataUpManager.dataContext = (function (options) {
				var dataContext = {
					getFiles: getFiles,
					getRepositoryList: getRepositoryList,
					downloadFileFromBlob: downloadFileFromBlob,
					deleteFile: deleteFile,
					checkTokenStatus: checkTokenStatus,
					downloadFileFromRepository: downloadFileFromRepository,
					updateAuthToken: updateAuthToken
				};

				return dataContext;

				function getFiles(isAsync) {
					return $.ajax({
						url: getfilesUri(),
						type: "GET",
						async: isAsync,
						contentType: "application/json",
						xhrfields: { withCredentials: false },
						headers: { Authorization: options.jwtAuthenticationToken }
					 
					});
				}

				function downloadFileFromBlob(fileId, isAsync) {
						var def = $.Deferred(),
						  xhr = new XMLHttpRequest();
						xhr.open('GET', buildFileDownloadUrl(fileId), isAsync);
						xhr.responseType = 'blob';
						xhr.setRequestHeader("Authorization", options.jwtAuthenticationToken);
						xhr.onreadystatechange = function () {
							if (xhr.readyState == 4) {
								def.resolve(xhr);
							}
						}
						xhr.send();
						return def;
				}

				function deleteFile(fileId, isAsync) {
					return $.ajax({
						url: buildDeleteFileUrl(fileId),
						type: "DELETE",
						async: isAsync,
						contentType: "application/json",
						xhrfields: { withCredentials: false },
						headers: { Authorization: options.jwtAuthenticationToken }
				   });
				}

				function getRepositoryList(isAsync) {
					return $.ajax({
						url: buildRepositoryUrl(),
						type: "GET",
						async: isAsync,
						contentType: "application/json",
						xhrfields: { withCredentials: false },
						headers: { Authorization: options.jwtAuthenticationToken }
					});
				}

				function checkTokenStatus(repositoryId, isAsync) {

				   return $.ajax({
						url: options.configuration.UserApiUri.concat("?repositoryId=", repositoryId),
						type: "get",
						async: isAsync,
						contentType: "application/json",
						xhrfields: { withCredentials: false },
						headers: { Authorization: options.jwtAuthenticationToken }
					});
				}

				function downloadFileFromRepository(fileId, repositoryCredentials, isAsync) {
					var def = $.Deferred(),
						xhr = new XMLHttpRequest();
					try {

						xhr.open('GET', options.configuration.FileApiUri.concat("/download?fileId=", fileId), isAsync);
						xhr.responseType = 'blob';
						xhr.setRequestHeader("Authorization", options.jwtAuthenticationToken);
						xhr.setRequestHeader("RepositoryCredentials", JSON.stringify(repositoryCredentials));
						xhr.onreadystatechange = function () {
							if (xhr.readyState == 4) {
								def.resolve(xhr);
							}
						}
						xhr.send();
					} catch (e) {
						def.resolve(xhr);
					}
					return def;
				}

				function updateAuthToken(authToken, isAsync) {
					return $.ajax({
						url: options.configuration.AuthTokenUri,
						type: "post",
						async: isAsync,
						contentType: "application/json",
						xhrfields: { withCredentials: false },
						headers: { Authorization: options.jwtAuthenticationToken },
						datatype: "json",
						data: JSON.stringify(authToken)
					});
				}

				function getfilesUri() {
					return options.configuration.FileApiUri + "/2";
				}

				function buildFileDownloadUrl(fileId) {

					return options.configuration.BlobApiUri.concat("?fileId=", fileId);
				}

				function buildDeleteFileUrl(fileId) {
					return options.configuration.FileApiUri.concat("?fileId=", fileId, "&userId=1");
				}

				function buildRepositoryUrl() {
					return options.configuration.RepositoryApiUri;
				}

			})(this.options)
		},

		_initializeDataModel: function (ko, dataContext) {
			dataContext.file = file;
			dataContext.selectedFile = selectedFile;
			dataContext.repository = repository;

			function file(data) {
				var self = this,
					fileName = data.Name.split('.'),
					extension = fileName.pop(),
					name = fileName.join('.');


				self.fileId = ko.observable(data.FileId);
				self.name = name;
				self.title = data.Title;
				self.createdOn = data.CreatedOn;
				self.status = data.Status;
				self.lifelineInHours = data.LifelineInHours;
				self.repositoryName = "";
				self.fileSize = data.Size;
				self.identifier = data.Identifier;
				self.publishedOn = data.PublishedOn;
				self.extension = extension;
				self.repositoryId = data.RepositoryId;
				self.contentType = data.MimeType;
				self.isSelected = ko.observable(false);
				self.repository = {};
				self.citation = data.Citation;
			}

			function selectedFile(data) {
				var self = this;
				self.name = data.name;
				self.status = data.status;
				self.fileId = data.fileId();
				self.extension = data.extension;
				self.repositoryId = data.repositoryId;
				self.contentType = data.contentType;
			}

			function repository(data) {
				var self = this;
				self.name = data.Name;
				self.isImpersonating = data.IsImpersonating;
				self.repositoryId = data.RepositoryId;
				self.allowedFileTypes = data.AllowedFileTypes;
				self.baseRepositoryId = data.BaseRepositoryId;
				self.isVisibleToAll = data.IsVisibleToAll;
				self.createdOn = data.CreatedOn;
			}
		},

		_initializeViewModel: function (ko, dataContext) {
			var that = this,
				widgetBase = $(that.element);

			this.element.dataUpManager.viewModel = (function (ko, dataContext, options) {
				var self = this;
				self.errorMessage = ko.observable("");
				self.informationMessage = ko.observable("");
				
				// model to store the file list               
				self.userFiles = [];
				self.filteredFileList = ko.observableArray();
				
				// model to store the repository list
				self.repositoryList = [];
				self.filteredRepositoryList = ko.observableArray();

				// default selected value status as "None"
				self.selectedFile = ko.observable(options.state.selectedFile || { status: constants.Status_None });
		 
				// Sort and Filter Properties
				self.filterValueList = ko.observableArray();
				self.selectedFilter = ko.observable();

				self.sortValueList = ko.observableArray();
				self.selectedSort = ko.observable();
				
				// populate the filter and sort list.
				populateFilterList();
				populateSortList();

				// get the complete list of files.
				self.getFiles = function () {
					var def = $.Deferred();
					dataContext.getFiles(true)
					.success(function (data) {
						// loop througth the files and add them to array
						self.userFiles = [];
						$.each(data || [], function (index, fileData) {
							var file = new dataContext.file(fileData);
							self.userFiles.push(file);
						});

						// get the list of repositories
						getRepositoryList().done(function () {
							// associate the repository to file which is required in case of published files.
							attachRepositoryToFile();
						})
                        .fail(function (jqXHR, status, errorThrown) {
                            reportError(actions.getRepositories, jqXHR, jqXHR.status, errorThrown);
                        })
						.always(function () {
							def.resolve();
						});
					})
					.fail(function (jqXHR, status, errorThrown) {
					    reportError(actions.getFiles, jqXHR, jqXHR.status, errorThrown);
						def.reject();
					})

					return def;
				};

				// get the list of repositories
				getRepositoryList = function () {
				 
					var def = $.Deferred();
					self.repositoryList = [];
					dataContext.getRepositoryList(true)
					.success(function (data) {
						// loop througth the repositires and add them to array
						$.each(data || [], function (index, data) {
							var repository = new dataContext.repository(data.RepositoryData);
							self.repositoryList.push(repository);
						});

						self.repositoryList.sort(sortByCreatedOnDesc);
						def.resolve();
					})
					.fail(function (jqXHR, status, errorThrown) {
						def.reject(jqXHR, status, errorThrown);
					})

					return def;
				}

				// show the detail div on selection of file
				self.showDetail = function (data, event) {
					reportStart(actions.showFileDetails);
					var selectedFile = new dataContext.selectedFile(data);
					that.element.dataUpManager.viewModel.selectedFile(selectedFile);
					showDetailPane(event.currentTarget, data);
					reportSuccess(actions.showFileDetails, "");
				}

				self.showDetailPane = function (selctedTile, data) {

					// remove any existing detail widget
					widgetBase.find('#fileDetail').remove();

					var lastTileIndex = findLastTileIntheRow(selctedTile),
						lastTile = widgetBase.find("#fileListWrapper ul li:nth-child(" + lastTileIndex + ")"),
						detailElement,
						selectedFile;

					if (lastTile.length <= 0) {
						lastTile = widgetBase.find("#fileListWrapper ul li:last-child");
					};				

					detailPaneWidth = getFileDetailPaneWidth();
					paneWidth = "100%";
					if (detailPaneWidth > 0) {
					    paneWidth = detailPaneWidth.toString() + "px";
					}

					detailElement = $('<div id="fileDetail" style="width:' + paneWidth + '"></div>').filedetail({ file: data, browserVersion: getInternetExplorerVersion(), fileDetailPaneWidth: paneWidth });
					detailElement.insertAfter(lastTile);

					// filter the repository list based on the file type
					filterRepositoryList(data.extension, options.configuration.FileTypeDelimeter);

				}

				// event handler for the download action
				self.downloadFileActionHandler = function (data, event) {
					reportStart(actions.downloadFile);
					var repository,
						status = self.selectedFile().status;

					// download the file from the blob if status = "Uploaded|Inqueue|Verifying|Error
					if (status == constants.Status_Uploaded
						|| status == constants.Status_InQueue
						|| status == constants.Status_Verifying
						|| status == constants.Status_Error
						) {

						downloadFileFromBlob(self.selectedFile().fileId);
						return;
					}

					// if status = posted get the repository of the file.
					$.each(self.repositoryList, function (index, rep) {
						if (rep.repositoryId == self.selectedFile().repositoryId) {
							repository = rep;
						}
					});
					
					// check if credentials are required to proceed further
					var isCredentialRequired = checkCredentials(repository);

					// if credentials are required then fire the trigger so client can supply the nessary information
					if (isCredentialRequired) {
						that._trigger(triggers.getCredentials,
							null,
							{
								baseRepositoryId: repository.baseRepositoryId,
								state: {
									repositoryId: repository.repositoryId,
									repositoryName: repository.name,
									selectedFile: self.selectedFile(),
									filterValue: self.selectedFilter().id,
									sortValue: self.selectedSort().id,
									actionName: actions.downloadFileFromRepository
								}
							});
						return;
					}

					// download the file from the repository if everything is perfect.
					downloadFileFromRepository(null)
				};

				// handler for the post action
				self.postFileActionHandler = function (repositoryDetails, event) {
					reportStart(actions.postFile);

					// check if credentials are required to proceed further
					if (repositoryDetails.baseRepositoryId != 1) {
						var isCredentialRequired = checkCredentials(repositoryDetails);

						// if credentials are required then fire the trigger so client can supply the nessary information
						if (isCredentialRequired) {
							that._trigger(triggers.getCredentials,
								null,
								{
									baseRepositoryId: repositoryDetails.baseRepositoryId,
									state: {
										repositoryId: repositoryDetails.repositoryId,
										repositoryName: repositoryDetails.name,
										selectedFile: self.selectedFile(),
										filterValue: self.selectedFilter().id,
										sortValue: self.selectedSort().id,
										actionName: actions.postFile
									}
								});
							return;
						}
					}
					reportSuccess(actions.postFile, "");
					// trigger the post file event.
					triggetPostFile(repositoryDetails.repositoryId, repositoryDetails.name, self.selectedFile().fileId);
				}

				// delete file action handler
				self.deleteFile = function (data, event) {

					if (!confirm(messages.deleteAlertMessage)) {
						return;
					}

					reportStart(actions.deleteFile);
					dataContext.deleteFile(data.selectedFile().fileId, false)
					 .success(function (result) {
						 that.RefreshFiles().done(function () {
							 self.selectedFile({ status: constants.Status_None });
							 reportSuccess(actions.deleteFile, "")
						 });
					 })
					 .fail(function (jqXHR, status, errorThrown) {
					     reportError(actions.deleteFile, jqXHR, jqXHR.status, errorThrown);
					 })
				}

				
				self.triggetPostFile = function (repositoryId, repositoryName, fileId) {
					that._trigger(triggers.postFile,
						null,
						{
							repositoryId: repositoryId,
							repositoryName: repositoryName,
							fileId: fileId
						});
				}
				
				downloadFileFromBlob = function (fileId) {
					var rawData;
					dataContext.downloadFileFromBlob(fileId, true).always(function (xhr) {
						if (xhr.status == 200) {
							rawData = xhr.response;
							showBrowserDownloadPopup(rawData, rawData.type);
							reportSuccess(actions.downloadFile, "");
						} else {
							getErrorFromBlob(xhr.response).always(function (error) {
								reportError(actions.downloadFile, error, xhr.status, xhr.statusText);
							});
					}
					});
				};

				self.downloadFileFromRepository = function (repositoryCredentials) {
					var rawData;
					reportStart(actions.downloadFileFromRepository);
					dataContext.downloadFileFromRepository(self.selectedFile().fileId, repositoryCredentials || {}, true).always(function (xhr) {
						if (xhr.status == 200) {
							rawData = xhr.response;
							showBrowserDownloadPopup(rawData, rawData.type);
							reportSuccess(actions.downloadFileFromRepository, "");
						} else {
							getErrorFromBlob(xhr.response).always(function (error) {
							   
								reportError(actions.downloadFileFromRepository, error, xhr.status, xhr.statusText);
							});
						}
					})
				}


				self.filterMenu = function (data, event) {
					$('ul:first', event.currentTarget).toggle();
				}

				function showBrowserDownloadPopup(fileData, contentType) {

					var resultFileStream,
						version,
						resultFileName,
						fileName = self.selectedFile().name,
						extension = self.selectedFile().extension;
				   
					if (extension == "xlsx") {
						resultFileName = fileName.concat(".", extension);
					} else {
						contentType = "application/x-zip-compressed";
						resultFileName = fileName.concat(".zip");
					}
					
					resultFileStream = new Blob([fileData], { type: contentType });

					var version = getInternetExplorerVersion();
					if (version == -1) {
					   
						// IF THE BROWSER IS NON IE
						// TBD: CODE BELOW FAILS AND NEEDS TO BE FIXED
						var downloadLink = document.createElement("a");
						downloadLink.setAttribute("href", window.webkitURL.createObjectURL(resultFileStream));
						downloadLink.setAttribute("download", resultFileName);
						var event = document.createEvent('MouseEvents');
						event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
						downloadLink.dispatchEvent(event);
					}
					else if (version >= 10.0) {
						navigator.msSaveBlob(resultFileStream, resultFileName);
					}
				}

				function filterUploadedFiles() {
					self.filteredFileList.removeAll();
					$.each(self.userFiles, function (index, file) {
						if (file.status == constants.Status_Uploaded || file.status == constants.Status_Error) {
							self.filteredFileList.push(file);
						}
					});
				}

				function filterPublishedFiles() {
					self.filteredFileList.removeAll();
					$.each(self.userFiles, function (index, file) {
						if (file.status == constants.Status_Posted) {
							self.filteredFileList.push(file);
						}
					});
				}

				function filterAll() {
					self.filteredFileList.removeAll();
					$.each(self.userFiles, function (index, file) {
						self.filteredFileList.push(file);
					});
				}

				function findLastTileIntheRow(activeTile) {
				    var tileWidth = $(activeTile).outerWidth(true),
						containerWidth = widgetBase.find('#fileListWrapper ul').outerWidth(),
						index = widgetBase.find("#fileListWrapper ul li").index(activeTile) + 1,
						maxTilesInSingleRow = Math.floor(containerWidth / tileWidth),
                        activeRow = Math.ceil(index / maxTilesInSingleRow);

				    var remainder = containerWidth % tileWidth;
				    if (remainder == 0) {
				        maxTilesInSingleRow -= maxTilesInSingleRow;
				    }

					return maxTilesInSingleRow * activeRow;
				}
                			

				function getFileDetailPaneWidth() {
				    outerWidthWithMargin = widgetBase.find("#fileListWrapper ul li").outerWidth(true);
				    outerWidthWihtoutMargin = widgetBase.find("#fileListWrapper ul li").outerWidth();
				    marginWidth = outerWidthWithMargin - outerWidthWihtoutMargin;

				    containerWidth = widgetBase.find('#fileListWrapper ul').outerWidth();
				    tileCountInARow = Math.floor(containerWidth / outerWidthWithMargin);

				    var remainder = containerWidth % outerWidthWithMargin;
				    if (remainder == 0) {
				        tileCountInARow -= tileCountInARow;
				    }

				    fileDetailPanelWidth = tileCountInARow * outerWidthWithMargin;
                    // remove the margin width of the last tile to make precice alignment
				    fileDetailPanelWidth = fileDetailPanelWidth - marginWidth;
				    return fileDetailPanelWidth;
				}

				function sortByCreatedOnDesc(left, right) {
					
					if (left.createdOn < right.createdOn) {
						return 1
					}
					else if (left.createdOn > right.createdOn) {
						return -1
					}
					else {
						return 0;
					}
				}

				function sortByPublishedOnDesc(left, right) {
					var leftPublishedOn = left.publishedOn,
						rightPublishedOn = right.publishedOn;
					if (leftPublishedOn == null && rightPublishedOn != null) {
						return 1;
					}
					if (leftPublishedOn != null && rightPublishedOn == null) {
						return -1;
					}
					else if (leftPublishedOn < rightPublishedOn) {
						return 1
					}
					else if (leftPublishedOn > rightPublishedOn) {
						return -1
					}
					else {
						return 0;
					}
				}

				function sortByName(left, right) {
					var leftName = left.name.toLowerCase(),
						rightName = right.name.toLowerCase();

					return leftName == rightName ? 0 : (leftName < rightName ? -1 : 1)
				}

				function sortByExtension(left, right) {
					var leftExtension = left.extension.toLowerCase(),
						rightExtension = right.extension.toLowerCase();
					return leftExtension == rightExtension ? 0 : (leftExtension < rightExtension ? -1 : 1)
				}

				function filterRepositoryList(fileType, fileTypeDelimiter) {
					self.filteredRepositoryList.removeAll();
					$.each(repositoryList || [], function (index, repository) {
						var isSupported = false,
							 supportedFileTypes;

						if (repository.allowedFileTypes == "undefined" || repository.allowedFileTypes == null) {
							isSupported = true;
						}
						else {

							supportedFileTypes = repository.allowedFileTypes.split(fileTypeDelimiter);
							if (supportedFileTypes.length <= 0) {
								isSupported = true;
							}
							else {
								$.each(supportedFileTypes, function (position, supportedType) {
									if (supportedType.toLowerCase() === fileType.toLowerCase()) {
										isSupported = true;
									}
								});
							}
						}

						if (isSupported == true) {
							self.filteredRepositoryList.push(repository);
						}
					});
				}

				self.filterActionHandler = function (data, event) {
					getFilterOptions(data.id);
					removeOrAddSortOptionsBasedOnFilter();
					applyFilter();
					applySort();
					self.selectedFile({ status: constants.Status_None });
				}
			 
				self.sortActionHandler = function (data, event) {
					getSortOptions(data.id);
					applyFilter();
					applySort();
					self.selectedFile({ status: constants.Status_None });
				}

				function checkCredentials(repository) {

					var needsCredentials = false,
						crendentialType;
					if (repository.isImpersonating == false) {

						if (repository.baseRepositoryId == constants.Repository_Merritt) {
							needsCredentials = true;
							return needsCredentials
						}

						if (repository.baseRepositoryId == constants.Repository_SkyDrive) {

							dataContext.checkTokenStatus(repository.repositoryId, false)
							.success(function (response) {
								if (response.RedirectionRequired) {
									needsCredentials = true;
								}
							})
							.fail(function (jqXHR, status, errorThrown) {
							    reportError(actions.checkCredentials, jqXHR, jqXHR.status, errorThrown);
							})
						}
					}

					return needsCredentials;
				}

				function removeOrAddSortOptionsBasedOnFilter() {
					var filter = self.selectedFilter().id,
						sortByPublishDate = getSelectedSort(constants.Sort_By_DatePublished);
					switch (filter) {
						case constants.Filter_Uploaded:
							// remove the sort by DatePublished
							self.sortValueList.remove(function (sort) {
								return sort.id == constants.Sort_By_DatePublished
							});

							// if sort by DatePublished was the selected sort then make the Sort By DateUploaded as the selected sort
							if (self.selectedSort().id == constants.Sort_By_DatePublished) {
								self.selectedSort(getSelectedSort(constants.Sort_By_DateUploaded));
								self.sortValueList.remove(function (sort) {
									return sort.id == constants.Sort_By_DateUploaded
								});
							}

							break;
						default:
						  
							// for any other filter check if the sort by Publish date exists if not then add the sort to the list
							// make sure not add the sort when the selected sort is already sort by DatePublished.
							if ((sortByPublishDate == undefined || sortByPublishDate.length <= 0) && (self.selectedSort().id != constants.Sort_By_DatePublished)) {
								self.sortValueList.push({
									id: constants.Sort_By_DatePublished,
									name: "Date Published"
								});
							}
					};
				}

				applyFilter = function () {
					var filter = self.selectedFilter().id;
					self.filteredFileList.removeAll();
					switch (filter) {
						case constants.Filter_Uploaded:
							filterUploadedFiles();
							break;
						case constants.Filter_Published:
							filterPublishedFiles();
							break;
						case constants.Filter_ALL:
							filterAll();
							break;
						default:
							filterUploadedFiles();
					};
				}

				applySort = function () {
					var sort = self.selectedSort().id;
					switch (sort) {
						case constants.Sort_By_DateUploaded:
							self.filteredFileList.sort(sortByCreatedOnDesc)
							break;
						case constants.Sort_By_DatePublished:
							self.filteredFileList.sort(sortByPublishedOnDesc)
							break;
						case constants.Sort_By_FileName:
							self.filteredFileList.sort(sortByName)
							break;
						case constants.Sort_By_FileType:
							self.filteredFileList.sort(sortByExtension);
							break;
						default:
							self.filteredFileList.sort(sortByCreatedOnDesc);
					}
				}
				
				function initializeViewModel() {
					reportStart(actions.initialize);
					getFiles().done(function () {
						removeOrAddSortOptionsBasedOnFilter();
						applyFilter();
						applySort();
						applyState(options.state)
						reportSuccess(actions.initialize);
					});
				}
				
				function attachRepositoryToFile() {
					$.each(userFiles, function (index, file) {
						if (file.repositoryId != null) {
							file.repository = getRepository(file.repositoryId)[0] || {};
						}
					});
				}

				function getRepository(repositoryId) {

					return $.grep(repositoryList, function (rep) {
						 return rep.repositoryId == repositoryId;
					});
				}

				function getFile(fileId) {

					return $.grep(userFiles, function (file) {
						return file.fileId() == fileId;
					});
				}
				function getSelectedFilter(filterId) {
					return $.grep(filterValueList(), function (filter) {
						return filter.id == filterId;
					})[0]
				}

				function getSelectedSort(sortId) {
					return $.grep(sortValueList(), function (sort) {
						return sort.id == sortId;
					})[0]
				}

				function getFilterOptions(selectedValue) {
					self.filterValueList.removeAll();
					self.filterValueList.push({ id: constants.Filter_Uploaded, name: "Uploaded Files" });
					self.filterValueList.push({ id: constants.Filter_Published, name: "Published Files" });
					self.filterValueList.push({ id: constants.Filter_ALL, name: "View All" });
					self.selectedFilter(getSelectedFilter(selectedValue) || {});
					self.filterValueList.remove(function (filter) {
					   return filter.id == selectedValue;
					});
				}

				function populateFilterList() {
					var selectedValue
					if (options.state != undefined && options.state != null && options.state.actionName != undefined) {
						selectedValue = options.state.filterValue;
					} else {
						selectedValue = options.filterValue;
					}

					getFilterOptions(selectedValue);
				}

				function getSortOptions(selectedValue) {
					self.sortValueList.removeAll();
					self.sortValueList.push({ id: constants.Sort_By_DateUploaded, name: "Date Uploaded" });
					self.sortValueList.push({ id: constants.Sort_By_DatePublished, name: "Date Published" });
					self.sortValueList.push({ id: constants.Sort_By_FileName, name: "File Name" });
					self.sortValueList.push({ id: constants.Sort_By_FileType, name: "File Type" });

					self.selectedSort(getSelectedSort(selectedValue) || {});

					self.sortValueList.remove(function (sort) {
						return sort.id == selectedValue;
					});

					if (self.selectedFilter().id == constants.Filter_Uploaded) {
						self.sortValueList.remove(function (sort) {
							return sort.id == constants.Sort_By_DatePublished
						});
					}
				}
				
				function populateSortList() {
				  
					var selectedValue
					if (options.state != undefined && options.state != null && options.state.actionName != undefined) {
						selectedValue = options.state.sortValue;
					} else {
						selectedValue = options.sortValue;
					}

					getSortOptions(selectedValue);
				}

				applyState = function (state) {

					if (state != undefined) {
						var selectedTile = widgetBase.find(".file[fileId=" + self.selectedFile().fileId + "]");
						if (selectedTile.length > 0) {
							var file = getFile(self.selectedFile().fileId)[0];
							showDetailPane(selectedTile, file);
						}
					}

					if (state.actionName == actions.downloadFileFromRepository) {
						self.downloadFileFromRepository(state.repositoryCredentials);
					} else if (state.actionName == actions.postFile) {
						var authToken = {
							RespositoryId: state.repositoryId,
							AccessToken: state.repositoryCredentials.AccessToken,
							RefreshToken: state.repositoryCredentials.RefreshToken,
							TokenExpiresOn: state.repositoryCredentials.TokenExpiresOn
						}

						dataContext.updateAuthToken(authToken, true)
						.success(function (result) {
							self.triggetPostFile(state.repositoryId, state.repositoryName, state.selectedFile.fileId);
						})
						.fail(function (jqXHR, status, errorThrown) {
						  
						    reportError(actions.applyState, jqXHR, jqXHR.status, errorThrown);
						})
					}
				}

				function reportStart(actionName) {
					that._trigger(triggers.operationStart, null, { actionName: actionName });
				}

				function reportSuccess(actionName, message) {
					that._trigger(triggers.operationCompleted, null, { actionName: actionName, message: message });
					if (!that.options.propagateMessages) {
						that.element.dataUpManager.viewModel.errorMessage("");
						that.element.dataUpManager.viewModel.informationMessage(message);
					}
				};

				function reportError(actionName, jqXHR, status, errorThrown) {
					var error = "";
					if (jqXHR.responseText != undefined && jqXHR.responseText != null && jqXHR.responseText.length > 0) {

						if (jqXHR.getResponseHeader('Content-Type').indexOf('application/json') > -1) {
							var errorObject = $.parseJSON(jqXHR.responseText);

							error = errorObject.Message
							if (error.length == 0) {
							    error = errorObject.message || "";
							}
						   
						} else {
							error = jqXHR.responseText;
						}

					} else if (errorThrown.length > 0) {
						error = errorThrown;
					}
				   
					if (error.length == 0) {
						error = messages.operationFailed;
					}
				  
					that._trigger(triggers.operationFailed, jqXHR, { actionName: actionName, message: error, status: status });
					if (!that.options.propagateMessages) {
						that.element.dataUpManager.viewModel.informationMessage("");
						that.element.dataUpManager.viewModel.errorMessage(error);
					}
				}

				function getErrorFromBlob(rawData) {
				    var def = $.Deferred();
				    if (rawData == undefined || rawData == null || rawData.length <= 0) {
				        def.resolve({});
				    } else {

				        var errorBlob = new Blob([rawData], { type: rawData.type }),
                            reader = new FileReader();

				        reader.onloadend = function () {
				            error = {
				                responseText: reader.result,
				                getResponseHeader: function () { return errorBlob.type }
				            };

				            def.resolve(error);
				        };

				        reader.readAsText(errorBlob);
				    }
					return def;
				};

				initializeViewModel();

				return {
					errorMessage: self.errorMessage,
					informationMessage: self.informationMessage,
					userFiles: userFiles,
					filteredFileList: filteredFileList,
					repositoryList: repositoryList,
					selectedFile: selectedFile,
					filteredRepositoryList: filteredRepositoryList,
					getFiles: getFiles,
					showDetail: showDetail,
					downloadFileActionHandler: downloadFileActionHandler,
					postFileActionHandler: postFileActionHandler,
					deleteFile: deleteFile,
					downloadFileFromRepository: downloadFileFromRepository,
					triggetPostFile: triggetPostFile,
					filterValueList: filterValueList,
					selectedFilter: selectedFilter,
					filterActionHandler: filterActionHandler,
					sortActionHandler: sortActionHandler,
					sortValueList: sortValueList,
					selectedSort: selectedSort,
					filterMenu: filterMenu
				};

			})(ko, dataContext, this.options);

			ko.applyBindings(this.element.dataUpManager.viewModel, this.fileListHtml[0]);
		},

		RefreshFiles: function () {
			var def = $.Deferred();

			this.element.dataUpManager.viewModel.getFiles().done(function () {
				applyFilter();
				applySort();
			})
			.always(function () {
				self.selectedFile({ status: constants.Status_None });
				def.resolve();
			});

			return def;
		},

		ApplyState: function (state) {
			if (state.selectedFilter != undefined || state.selctedFilter != null) {
				this.element.dataUpManager.viewModel.selctedFilter(state.selctedFilter);
				getFilterOptions(state.selctedFilter);
			}

			if (state.selectedSort != undefined || state.selectedSort != null) {
				this.element.dataUpManager.viewModel.selectedSort(state.selectedSort);
				getSortOptions(state.selectedSort);
			}
			
			if (state.selctedFile != undefined || state.selctedFile != null) {
				this.element.dataUpManager.viewModel.selctedFile(state.selctedFile);
			}

			applyFilter();
			applySort();
			applyState(state);

		},
	   
		_attachEventsToDropdowns: function () {
			var baseElement = $(this.element);
			baseElement.find(".view-by li:first").hover(function () {

			}, function () {
				$('ul:first', this).css('display', 'none');
			});

			baseElement.find(".view-by li:first").click(function () {
				$('ul:first', this).toggle();
			})

			baseElement.find(".sort-by li:first").hover(function () {

			}, function () {
				$('ul:first', this).css('display', 'none');
			});

			baseElement.find(".sort-by li:first").click(function () {
				$('ul:first', this).toggle();
			})
		},

		_destroy: function () {
			this.fileListHtml.remove();
		}
	});


	function asyncResult(result, func) {
		var waiter = $.Deferred();
		this.notify = function (asyncResult) {
			result = asyncResult;
			waiter.notify();
		}

		waiter.progress(function () {
			waiter.resolve();
		});

		waiter.done(function () {
			return func();
		});
	}

})(jQuery);
