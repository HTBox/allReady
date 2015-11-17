fn = ko.observableArray.fn;
fn.withValidator = function (validatorFn) {
    this.isValid = ko.computed(function () {
        return validatorFn(this);
    }, this);
    return this;
};
fn.uniqueValidator = function (property) {
    return this.withValidator(function (obsArray) {
        //every value is unique if the length of the original array is equal to the length of a distinct array
        return obsArray().length === obsArray()
            .reduce(function (prev, curr) {
            var val = ko.unwrap(property ? curr[property] : curr);
            if (prev.indexOf(val) === -1)
                prev.push(val);
            return prev;
        }, []).length;
    });
};
