///<reference path="../lib/knockout/dist/knockout.js" />

ko.observableArray.fn.withValidator = function (validatorFn) {
    this.isValid = ko.computed(function () {
        return validatorFn(this);
    }, this);
    return this;
};

ko.observableArray.fn.uniqueValidator = function (property) {
    return this.withValidator(function (obsArray) {
        //every value is unique if the length of the original array is equal to the length of a distinct array
        return obsArray().length === obsArray()
            .reduce(function(prev,curr) {
                var val = ko.unwrap(property ? curr[property] : curr);
                if (prev.indexOf(val) === -1) prev.push(val);
                return prev;
            }, []).length;
    });
};

//
// This is a "poor man's" validation routine that provides specific, limited capabilities (currently only supports required, email, phoneNumber validations).
// If more a more general & extensive validation capability is needed, consider using Knockout-Validation plugin at https://github.com/Knockout-Contrib/Knockout-Validation
//
ko.extenders.validate = function (target, options) {
    options = options || {
        isRequired: false,
        overrideMessage: ""
    };
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();
    target.showErrors = ko.observable();

    target.validate = function (newValue, isShowErrors) {
        isShowErrors = typeof isShowErrors !== 'undefined' ? isShowErrors : true;

        if (typeof options.required !== "undefined" && !newValue) {
            target.hasError(true);
            target.showErrors(isShowErrors);
            target.validationMessage(options.required || "This field is required");
            return;
        }

        if (typeof options.email !== "undefined") {
            var regEx = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            target.hasError(regEx.test(newValue) ? false : true);
            target.showErrors(isShowErrors);
            target.validationMessage(options.email || "'Email' is invalid");
            if (target.hasError) return;
        }

        if (typeof options.phoneNumber !== "undefined") {
            var digits = newValue.match(/\d/g) || "";
            target.hasError(digits.length === 10 ? false : true);
            target.showErrors(isShowErrors);
            target.validationMessage(options.phoneNumber || "'Phone Number' is invalid");
            if (target.hasError) return;
        }
    }

    // do initial validation for purpose of determining if the form is valid, but do not show error messages since the 
    // field has not yet been visited
    target.validate(target(), false);

    target.subscribe(target.validate);

    return target;
};