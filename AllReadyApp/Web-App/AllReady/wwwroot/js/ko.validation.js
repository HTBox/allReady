///<reference path="../lib/knockout/dist/knockout.js" />

ko.subscribable.fn.withValidator = function (validatorFn) {
    //var isValidAlready = this.isValid() || true; //Default to valid if no validator defined yet
    var isValidAlready = true; //Default to valid if no validator defined yet

    this.isValid = ko.computed(function () {
        //debugger;
        console.log("****************************************************************************");

        if (this.isCurrentlyInValidation) {
            console.log("Exiting since validation is already in progress");
            return true;
        }

        console.log("isValidAlready = " + isValidAlready);

        this.isCurrentlyInValidation = true;
        var validationResult = isValidAlready && validatorFn(this);
        this.isCurrentlyInValidation = false;

        console.log("validationResult = " + validationResult);
        console.log("****************************************************************************");

        return validationResult;
    }, this);

    var isValidAlready = this.isValid() || true; //Default to valid if no validator defined yet
    return this;
};

ko.observableArray.fn.notifyChangeFromInitialValue = function () {
    var self = this;
    self.haschangedFromInitial = ko.observable(false);
    self.subscribe(function () {
        self.haschangedFromInitial(true);
    });
}


ko.subscribable.fn.isRequired = function (validationMessage) {
    var property = this;
    initializeValidationProperties(property);

    return this.withValidator(function (property) {
        var validationResult = property() ? true : false;
        property.validationMessage(!validationResult ? validationMessage || "'Email is required'" : "");

        logDebugMessages("isRequired", property, validationResult);

        return validationResult;
    });
};

ko.subscribable.fn.validateEmail = function (validationMessage) {
    var property = this;
    initializeValidationProperties(property);
    return this.withValidator(function (property) {
        var regEx = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        var validationResult = regEx.test(property());
        property.validationMessage(!validationResult ? validationMessage || "'Email is invalid'" : "");

        logDebugMessages("validateEmail", property, validationResult);

        return validationResult;
    });
};

function initializeValidationProperties(property) {
    if (typeof property.validationMessage !== "undefined") {
        return;
    }
    property.validationMessage = ko.observable("");

    property.isShowValidationMessage = ko.computed(function () {
        logIsShowValidationMessage(property);
        return typeof property.isValid === "function" && !property.isValid();
    });
}

function logDebugMessages(validator, property, validationResult) {
    console.log("================================================================");
    console.log("In " + validator + " validator");
    console.log("property = " + property());
    console.log("validationResult = " + validationResult);
    console.log("validationMessage = " + property.validationMessage());
    console.log("isShowValidationMessage = " + property.isShowValidationMessage());
    console.log("================================================================");
}

function logIsShowValidationMessage(property) {
    console.log("++++++++++++++++++++++++++++++");
    console.log("In 'isShowValidationMessage'");
    if (typeof property.isValid === 'function') {
        console.log("isValid = " + property.isValid());
    } else {
        console.log("isValid = not yet assigned");
    }
    console.log("++++++++++++++++++++++++++++++");
}

ko.observableArray.fn.uniqueValidator = function (property) {
    return this.withValidator(function (obsArray) {
        //every value is unique if the length of the original array is equal to the length of a distinct array
        return obsArray().length === obsArray()
            .reduce(function (prev, curr) {
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
    target.isShowValidationMessage = ko.computed(function () {
        //console.log("target = " + target());
        //console.log("hasError = " + target.hasError());
        //console.log("showErrors = " + target.showErrors());
        return target.hasError() && target.showErrors();
    });

    target.validate = function (newValue, isShowErrors) {
        isShowErrors = typeof isShowErrors !== 'undefined' ? isShowErrors : true;
        //console.log("new value = " + newValue);

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

    // do initial validation for purpose of determining if the form is valid, but do not show validation messages since the 
    // field has not yet been visited
    target.validate(target(), false);

    target.subscribe(target.validate);

    return target;
};