ko.bindingHandlers.attrIf = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        var h = ko.utils.unwrapObservable(valueAccessor());
        var show = ko.utils.unwrapObservable(h._if);
        if (show) {
            ko.bindingHandlers.attr.update(element, valueAccessor, allBindingsAccessor);
        } else {
            for (var k in h) {
                if (h.hasOwnProperty(k) && k.indexOf("_") !== 0) {
                    $(element).removeAttr(k);
                }
            }
        }
    }
};

jQuery.validator.addMethod("validPhone", function (value, element) {
    var phoneReg = /^\(\d{3}\) ?\d{3}( |-)?\d{4}|^\d{3}( |-)?\d{3}( |-)?\d{4}$/;
    return this.optional(element) || phoneReg.test(value);
}, "Please specify a valid phone number in the format XXXXXXXXXX or XXX-XXX-XXXX.");

jQuery.validator.addMethod("validDate", function (value, element) {
    //yyyy-mm-dd format from between 1900-01-01 and 2099-12-31
    var dateReg = /^(19|20)\d\d([- /.])(0[1-9]|1[012])\2(0[1-9]|[12][0-9]|3[01])$/;
    return this.optional(element) || dateReg.test(value);
}, "Please specify a valid date in the YYYY-MM-DD format.");