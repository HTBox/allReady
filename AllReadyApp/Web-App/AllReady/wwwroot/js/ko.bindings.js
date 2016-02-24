// knockout binding for jquery.maskedinput plugin
ko.bindingHandlers.masked = {
    init: function (element, valueAccessor) {
        var value = valueAccessor(),
            mask = ko.utils.unwrapObservable(value);
            
        $(element).mask(mask, { autoclear: false });
    }
};
