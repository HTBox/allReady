// knockout binding for jquery.maskedinput plugin
ko.bindingHandlers.masked = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var mask = allBindingsAccessor().mask || {};
        var placeholder = allBindingsAccessor().placeholder;
        if (placeholder) {
            $(element).mask(mask, { placeholder: placeholder, autoclear: false });
        } else {
            $(element).mask(mask, { autoclear: false });
        }
        ko.utils.registerEventHandler(element, "blur", function () {
            var observable = valueAccessor();
            observable($(element).val());
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).val(value);
    }
};
