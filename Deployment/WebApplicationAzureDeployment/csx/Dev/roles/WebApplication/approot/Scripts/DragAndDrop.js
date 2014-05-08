var DragAndDrop = {

    dragEnter: function (evt) {
        if (evt.stopPropagation) {
            evt.stopPropagation();
        }
        evt.preventDefault();
    },

    dragOver: function (evt) {
        if (evt.stopPropagation) {
            evt.stopPropagation();
        }
        evt.preventDefault();
    },

    dragExit: function (evt) {
        if (evt.stopPropagation) {
            evt.stopPropagation();
        }
        evt.preventDefault();
    },

    drop: function (evt) {
        evt.stopPropagation();
        evt.preventDefault();
        var files = evt.dataTransfer.files;
        if (files) {
            UploadFile.StartUploading(files);
        }
    }
}

var UploadFile = {
    Files: {
        UploadingFiles: [],
        UploadingFileDetails: [],
        ZipFiles: [],
    },


    StartUploading: function (files) {
        if (files.length != undefined && files.length > 0) {
            $.each(files, function () {
                UploadFile.PushFileToArrays(this);
            });
        }
        else {
            UploadFile.PushFileToArrays(files);
        }

        if (UploadFile.Files.ZipFiles != undefined && UploadFile.Files.ZipFiles.length > 0) {
            UploadFile.GetZipFileDetails(UploadFile.Files.ZipFiles);
        }

        if (UploadFile.CheckFileSize(UploadFile.Files.UploadingFileDetails)) {
            UploadFile.ValidateandUploadFiles(UploadFile.Files.UploadingFiles, UploadFile.Files.UploadingFileDetails);
        }

    },

    PushFileToArrays: function (file) {
        UploadFile.Files.UploadingFiles.push(file);
        if (file.type == "application/x-zip-compressed") {
            UploadFile.Files.ZipFiles.push(file);
        }
        else {
            UploadFile.Files.UploadingFileDetails.push(new FileDetail(file.name, file.size));
        }
    },

    GetExistingFileList: function () {
        var filesList = [];
        var fileElements = $("#uploadFileTable").find("td.fileName");

        $.each(fileElements, function () {
            filesList.push(this.innerText.toLowerCase());
        });
        return filesList;
    },

    CheckFileSize: function (filedetails) {
        var filename = "";
        $.each(filedetails, function () {
            if (this.FileSize > 100) {
                filename = this.FileName;
            }
        });

        if (filename != "") {
            alert("File with size more than 100MB can't be uploaded");
            dataonBoardingData.hideBusy();
            UploadFile.ClearArrays();
            return false;
        }
        else {
            return true;
        }
    },

    GetZipFileDetails: function (zipfiles) {
        dataonBoardingData.showBusy();
        $.ajaxSetup({
            cache: false
        });
        var formdata = new FormData();

        if (zipfiles.length != undefined && zipfiles.length > 0) {
            $.each(zipfiles, function () {
                formdata.append(this.name, this);
            });
        }
        else {
            formdata.append(zipfiles.name, zipfiles);
        }

        jQuery.ajax({
            url: "GetListofFilesZipped",
            type: "POST",
            data: formdata,
            processData: false,
            contentType: false,
            async: false,
            success: function (res) {
                if (res.Error == undefined || (res.Error == "")) {
                    $.each(res.FilesList, function () {
                        UploadFile.Files.UploadingFileDetails.push(new FileDetail(this.Key, this.Value));
                    });
                }
                else {
                    dataonBoardingData.hideBusy();
                    alert(res.Error);
                }
            },
            error: function (request, status, error) {
                dataonBoardingData.hideBusy();
                alert(error);
            }
        });
    },

    ValidateandUploadFiles: function (uploadingfiles, filedetails) {
        var existingfiles = UploadFile.GetExistingFileList();
        var duplicatefiles = [];
        $.each(filedetails, function () {
            if (existingfiles.indexOf(this.FileName.toLowerCase()) >= 0) {
                if (duplicatefiles.indexOf(this.FileName.toLowerCase()) < 0) {
                    duplicatefiles.push(this.FileName);
                }
            }
        });
        if (duplicatefiles != undefined && duplicatefiles.length > 0) {
            var AlertMessage = "Following Files are Already Existing" + "\n";
            $.each(duplicatefiles, function () {
                AlertMessage += this + "\n";
            });
            AlertMessage += "\n" + "Do you want to Override ?";
            if (confirm(AlertMessage)) {
                UploadFile.Upload(uploadingfiles);
            }
            else {
                UploadFile.ClearArrays();
                dataonBoardingData.hideBusy();
            }
        }
        else {
            UploadFile.Upload(uploadingfiles);
        }
    },

    Upload: function (files) {
        dataonBoardingData.showBusy();
        $.ajaxSetup({
            cache: false
        });

        var formdata = new FormData();
        if (files.length > 0) {
            $.each(files, function () {
                formdata.append(this.name, this);
            });
        }
        else {
            formdata.append(files.name, files);
        }

        var authTicket = getCookie("x-api-jwt");

        if (authTicket == null || authTicket.length <= 0) {
            document.location = landingPage;
            return;
        }

        jQuery.ajax({
            url: "UploadFile",
            type: "POST",
            data: formdata,
            processData: false,
            contentType: false,
            success: function (res) {
                UploadFile.UploadCompleted(res);
            },
            error: function (request, status, error) {
                var response = $(request.responseText);
                if ($(request.responseText)[1] != undefined) {
                    var errorText = $(request.responseText)[1].innerText;
                    if (errorText == "SessionExpired") {
                        window.location.href = '/Authenticate/Index';
                    }
                    else {
                        UploadFile.UploadFailed(error);
                    }
                }
                else {
                    UploadFile.UploadFailed(error);
                }
            }
        });
    },

    UploadCompleted: function (data) {
        $("#divUploadFilesInnerDiv").html(data.ViewString);

        // Showing uploaded file list always
        //id = "divUploadedFiles"
        //id = "divPostedFiles" >

        $("#divUploadedFiles").show();
        $("#divPostedFiles").hide();
        $("#divfilesDeletionAlert").hide();

        if (!$("#btnUploadedFiles").hasClass("active")) {
            $("#btnUploadedFiles").addClass("active");
        }

        if ($("#btnPostedFiles").hasClass("active")) {
            $("#btnPostedFiles").removeClass("active");
        }

        if ($("#btnFilesDeletionAlert").hasClass("active")) {
            $("#btnFilesDeletionAlert").removeClass("active");
        }

        if ($("#divUploadedFiles").hasClass("hide")) {
            $("#divUploadedFiles").removeClass("hide");
        }

        if (!$("#divPostedFiles").hasClass("hide")) {
            $("#divPostedFiles").addClass("hide");
        }

        if (!$("#divfilesDeletionAlert").hasClass("hide")) {
            $("#divfilesDeletionAlert").addClass("hide");
        }

        dataonBoardingData.hideBusy();
        if ($("#divConfirmation").hasClass("visible")) {
            $("#divConfirmation").toggleClass("hide visible");
        }
        UploadFile.ClearArrays();
    },

    UploadFailed: function (message) {
        dataonBoardingData.hideBusy();
        UploadFile.ClearArrays();
    },

    ClearArrays: function () {
        UploadFile.Files.ZipFiles = [];
        UploadFile.Files.UploadingFiles = [];
        UploadFile.Files.UploadingFileDetails = [];
    },

    UploadMetaData: function (files) {
        dataonBoardingData.showBusy();
        $.ajaxSetup({
            cache: false
        });

        var isEmptyFile = false;

        var formdata = new FormData();
        if (files.length > 0) {
            $.each(files, function () {
                if (this.size > 0) {
                    formdata.append(this.name, this);
                }
                else {
                    isEmptyFile = true;
                }
            });
        }
        else {
            formdata.append(files.name, files);
        }

        if (isEmptyFile) {
            dataonBoardingData.hideBusy();
            alert("Invalid xml file");
            return false;
        }

        jQuery.ajax({
            url: "UploadMetaDataFile",
            type: "POST",
            data: formdata,
            processData: false,
            contentType: false,
            //datatype : "string",
            //contentType: "application/x-www-form-urlencoded;charset=utf-8",
            success: function (res) {

                if (res.Message == "success") {
                    $.post('GetUploadedXMLDataViewModel', {}, function (returnedList) {
                        //before setting the new xml data controls, set the current old repository data for deletion , so in case of ,this old data will be deleted
                        SetCurrentRepositoryDataForDeletion();
                        $("#managemetadata").html(returnedList);

                    });
                }
                else {
                    alert(res.Message);
                }
                dataonBoardingData.hideBusy();
            },
            error: function (request, status, error) {
                var response = $(request.responseText);
                if ($(request.responseText)[1] != undefined) {
                    var errorText = $(request.responseText)[1].innerText;
                    if (errorText == "SessionExpired") {
                        window.location.href = '/Authenticate/Index';
                    }
                    else {
                        UploadFile.UploadFailed(error);
                    }
                    dataonBoardingData.hideBusy();
                }
                else {
                    UploadFile.UploadFailed(error);
                }
            }

        });
    },
}

var FileDetail = function (filename, filesize) {
    this.FileName = filename.toLowerCase();
    this.FileSize = filesize / 1024 / 1024;
    return this;
}
