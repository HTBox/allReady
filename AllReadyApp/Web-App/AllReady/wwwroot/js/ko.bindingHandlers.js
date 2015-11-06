///<reference path="../lib/knockout/dist/knockout.js" />

ko.bindingHandlers.tooltip = {
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).tooltip('destroy');
        var options = ko.unwrap(valueAccessor());
        ko.utils.objectForEach(options, function (property) {
            options[property] = ko.unwrap(options[property]);
        });
        $(element).tooltip(options);
    }
};