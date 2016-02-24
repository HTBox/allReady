///<reference path="../lib/knockout/dist/knockout.js" />

ko.subscribable.fn.withValidator = function(validatorFn) {
    var property = this;

    property.validators = property.validators || [];
    property.validators.push(validatorFn);

    // only need to initialize things once, even if multiple validators for the property
    if (property.validators.length === 1) {
        property.isValid = ko.observable(true);
        property.validationMessage = ko.observable("");
        //this should be before the subscription so that it fires before the validation
        property.notifyChangeFromInitialValue();
        property.subscribe(validate, property);

        // this allows for a 'manual' call to force a validation without a property change
        // primarily used to make an inital call to validate upon entry
        property.validate = validate;
    };
    return property;

    function validate() {
        var property = this;
        console.log("In 'withValidator'");
        for (var i = 0, length = property.validators.length; i < length; i++) {
            var validatorFn = property.validators[i];
            validationResult = validatorFn(property);
            if (!validationResult.isValid) {
                if (typeof property.hasChangedFromInitial === "function" && !property.hasChangedFromInitial()) {
                    validationResult.validationMessage = "";
                }
                break;
            }
        }
        property.isValid(validationResult.isValid);
        property.validationMessage(validationResult.validationMessage || "");

        console.log("property.isValid = " + property.isValid());
        console.log("property.validationMessage = " + property.validationMessage());
        console.log("****************************************************************************");
    };
};

ko.subscribable.fn.notifyChangeFromInitialValue = function() {
    var self = this;
    self.hasChangedFromInitial = ko.observable(false);
    self.subscribe(function() {
        var subscription = self.subscribe(function () {
            self.hasChangedFromInitial(true);
            // only need to have this subscription fire once
            subscription.dispose();
            console.log("Change from initial value")
        });
    });
};
ko.subscribable.fn.isRequired = function(customValidationMessage) {
    var property = this;

    return property.withValidator(function(property) {
        logDebugMessages1("isRequired", property);
        var isValid = property() ? true : false;
        var validationMessage = !isValid ? customValidationMessage || "'Email is required'" : "";
        logDebugMessages2("isRequired", property, isValid, validationMessage);
        return { isValid: isValid, validationMessage: validationMessage };
    });
};

ko.subscribable.fn.validateEmail = function(customValidationMessage) {
    var property = this;

    return property.withValidator(function(property) {
        logDebugMessages1("validateEmail", property);
        var regEx = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        var isValid = regEx.test(property());
        var validationMessage = !isValid ? customValidationMessage || "'Email is invalid'" : "";
        logDebugMessages2("validateEmail", property, isValid, validationMessage);
        return { isValid: isValid, validationMessage: validationMessage };
    });
};

function logDebugMessages1(validator, property) {
    console.log("================================================================");
    console.log("    In " + validator + " validator");
    console.log("    property = " + property());
}

function logDebugMessages2(validator, property, validationResult, validationMessage) {
    console.log("    validationResult = " + validationResult);
    console.log("    validationMessage = " + validationMessage);
    console.log("    hasChangedFromInitial = " + property.hasChangedFromInitial());
    console.log("================================================================");
}

ko.observableArray.fn.uniqueValidator = function(property) {
    return this.withValidator(function(obsArray) {
        //every value is unique if the length of the original array is equal to the length of a distinct array
        var isValid = obsArray().length === obsArray()
            .reduce(function(prev, curr) {
                var val = ko.unwrap(property ? curr[property] : curr);
                if (prev.indexOf(val) === -1) prev.push(val);
                return prev;
            }, []).length;
        return { isValid: isValid };
    });
};

//
// This is a "poor man's" validation routine that provides specific, limited capabilities (currently only supports required, email, phoneNumber validations).
// If more a more general & extensive validation capability is needed, consider using Knockout-Validation plugin at https://github.com/Knockout-Contrib/Knockout-Validation
//
ko.extenders.validate = function(target, options) {
    options = options || {
        isRequired: false,
        overrideMessage: ""
    };
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();
    target.showErrors = ko.observable();
    target.isShowValidationMessage = ko.computed(function() {
        //console.log("target = " + target());
        //console.log("hasError = " + target.hasError());
        //console.log("showErrors = " + target.showErrors());
        return target.hasError() && target.showErrors();
    });

    target.validate = function(newValue, isShowErrors) {
        isShowErrors = typeof isShowErrors !== "undefined" ? isShowErrors : true;
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
    };

    // do initial validation for purpose of determining if the form is valid, but do not show validation messages since the 
    // field has not yet been visited
    target.validate(target(), false);

    target.subscribe(target.validate);

    return target;
};