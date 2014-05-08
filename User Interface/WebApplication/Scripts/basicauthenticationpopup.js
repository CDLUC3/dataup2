(function ($, undefined) {
    var messages = {
        
    },
    constants = {
    };

    $.widget("dataup.basicauthenticationpopup", {
        options: {
            actionName: "",
            credentialsEntered : $.noop,
            basicAuthenticationPopupFailed: $.noop
        },

        _create: function () {
            var basicAuthenticationPopupFailed = "basicAuthenticationPopupFailed";
            this.element.dataUpManager = this.element.dataUpManager || {};
            this._initializeDataBindings();
            this._initializeDataContext();
            this._initializeDataModel(ko, this.element.dataUpManager.dataContext);
            this._initializeViewModel(ko, this.element.dataUpManager.dataContext);
        },
        _init: function () {
            this.element.dataUpManager.viewModel.userName("");
            this.element.dataUpManager.viewModel.password("");

        },
        _initializeDataContext: function () {
            this.element.dataUpManager.dataContext = (function (options) {
                var dataContext = {
                };

                return dataContext;
            })(this.options)
        },

        _initializeDataModel: function (ko, dataContext) {
        },

        _initializeViewModel: function (ko, dataContext) {
            var that = this;

            this.element.dataUpManager.viewModel = (function (ko, dataContext, options) {
                that.actionName = options.actionName;
                that.userName = ko.observable(""),
                that.password = ko.observable("")
                that.download = function (data, event) {
                    var form = $('#merrittCredentials');
                    var validator = form.validate();
                    if (!form.valid()) {
                        validator.showErrors();
                        return false;
                    }


                    if (data.isValid()) {
                        that._trigger("credentialsEntered", null, { userName: data.userName(), password: data.password() });
                    }
                };
                that.isValid = ko.computed(function () {
                    if (that.userName().length == 0 || that.password().length == 0) {
                        return false;
                    } else {
                        return true;
                    }
                });

                return {
                    actionName : ko.observable(that.actionName),
                    download: that.download,
                    userName: that.userName,
                    password: that.password,
                    isValid : that.isValid
                };

            })(ko, dataContext, this.options);

            ko.applyBindings(this.element.dataUpManager.viewModel, this.popupHtml[0]);
        },

        _initializeDataBindings: function () {

            this.popupHtml = $('<div class="basicAuthenticationPopup"><div id="basicCred" class="wrapper">\
                    <form id="merrittCredentials" >\
                        <a id="closeCredPopup" href="javascript:void(0)" class="close" title="Close">X</a>\
                        <div class="container">\
                        <div class="userCredential">\
                        <div class="row">\
                            <label for="userName" class = "labels">UserName</label>\
                            <input type="text" id="userName" data-bind="value:userName" name="userName" required />\
                        </div>\
                        <div class="row">\
                            <label for="password" class = "labels">Password</label>\
                            <input type="password" id="password" data-bind="value:password" name="password" required/>\
                        </div>\
                        </div>\
                    <div class="endStripe">\
                         <input id="submit" type="submit" class="button l1 download" data-bind="click:download, value:actionName, title:actionName"/>\
                        <a id="cancelCredPopup" href="javascript:void(0)" class="button l1 cancel" title="Cancel">Cancel</a>\
                    </div>\
                </div>\
            </form>\
            </div></div>').appendTo(this.element)
        },

        _destroy: function () {
            this.popupHtml.remove();
        }
    });
})(jQuery);
