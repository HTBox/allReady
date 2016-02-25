///<reference path="../lib/knockout/dist/knockout.js" />

ko.subscribable.fn.withValidator = function (validatorFn, message) {
    this.__validators = this.__validators || [];
    this.__validators.push({
        fn: validatorFn,
        message: message
    });
    if (typeof this.isValid !== "function") {
        this.validationMessage = ko.observable("");
        this.isValid = ko.computed(function () {
            this.validationMessage(""); //Clear the message initially in case all validators pass
            return this.__validators.every(function (validator) {
                var isValid = validator.fn(this);
                if (!isValid && validator.message) {
                    this.validationMessage(validator.message);
                }
                return isValid;
            }, this);
        }, this);
    }
    return this;
};

ko.subscribable.fn.isRequired = function (customValidationMessage) {
    return this.withValidator(function (property) {
        return !!property();
    }, customValidationMessage || "This field is required");
};

ko.subscribable.fn.validateEmail = function (customValidationMessage) {
    return this.withValidator(function (property) {
        var regEx = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return regEx.test(property());
    }, customValidationMessage || "This field is not a well formatted email address");
};

ko.subscribable.fn.notifyChangeFromInitialValue = function () {
    this.hasChangedFromInitial = ko.observable(false);
    this.subscribe(function () {
        this.hasChangedFromInitial(true);
    }, this);
    return this;
};


ko.subscribable.fn.validatePhoneNumber = function (customValidationMessage) {
    var property = this;

    return property.withValidator(function (property) {
        var digits = property().match(/\d/g) || "";
        return digits.length === 10;
    }, customValidationMessage || "This field is not a well formatted phone number");
};

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

