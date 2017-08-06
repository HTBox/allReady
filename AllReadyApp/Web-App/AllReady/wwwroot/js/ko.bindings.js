// knockout binding for jquery.maskedinput plugin
ko.bindingHandlers.masked = {
    init: function (element, valueAccessor) {
        var value = valueAccessor(),
            mask = ko.utils.unwrapObservable(value);
        $(element).mask(mask, { autoclear: false });
    }
};

// knockout binding for bootstrap accordion
ko.bindingHandlers.accordion = {
    init: function(elem, value, allBindings) {
        var options = ko.utils.unwrapObservable(value()) || {},
            toggleClass = "[data-toggle-accordion]",
            contentClass = ".collapse",
            openItem = ko.utils.unwrapObservable(options.openItem) || false,
            itemClass = "." + (ko.utils.unwrapObservable(options.item) || "panel-group"),
            accordionDirectionIconClass = "." + (ko.utils.unwrapObservable(options.itemIconDirection) || "accordion-icon-direction"),
            items = $(elem).find(contentClass);

        initializeAccordion();

        // if the array is dynamic, the accordion should be re-initialized
        var list = (options.listSource) ? options.listSource : allBindings.get("foreach");
        if (ko.isObservable(list)) {
            list.subscribe(function() {
                initializeAccordion();
            });
        }

        $(elem).on("click", toggleClass, function (event) {
            $(elem).find(contentClass).collapse("hide");
            $(this).closest(itemClass).find(contentClass).collapse("show");
        });

        $(elem).on("show.bs.collapse", function (event) {
            toggleAccordionItemIconDirection(event);
        });

        $(elem).on("hide.bs.collapse", function (event) {
            toggleAccordionItemIconDirection(event);
        });

        // if initial open item specified, expand it
        if (openItem) {
            items.eq(openItem).collapse("show");
        };

        function initializeAccordion() {
            // activate all items
            $(elem).find(contentClass).collapse({ parent: elem, toggle: false });
        }

        function toggleAccordionItemIconDirection(event) {
            var $currentPanel = $(event.target).closest(itemClass);
            var $accordionDirectionIcon = $currentPanel.find(accordionDirectionIconClass);

            $accordionDirectionIcon.toggleClass("fa-caret-down fa-caret-up");
        }
    }
};

// knockout binding for datepicker date
ko.bindingHandlers.dateTimePicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().dateTimePickerOptions || {};
        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                if (!event.date) {
                    value(null);
                }
                else if (event.date instanceof Date) {
                    value(event.date);
                }
                else {
                    value(event.date.toDate());
                }
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("DateTimePicker");
            if (picker) {
                picker.destroy();
            }
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        // This is for input data binding only.
    }
};