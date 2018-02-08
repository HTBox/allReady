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
            openItem = parseInt(ko.utils.unwrapObservable(options.openItem)),
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
        if (openItem > -1) {
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

function Daterange(begin, end, formattedDate) {
    this.begin = begin;
    this.end = end;
    this.formattedDate = formattedDate;
}

// knockout binding for datepicker date
ko.bindingHandlers.dateRangePicker = {

    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize daterangepicker with some optional options
        var options = allBindingsAccessor().daterangepickerOptions || {};
        $(element).daterangepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "apply.daterangepicker", function (event, picker) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                var daterange = new Daterange();

                if (picker.startDate && picker.endDate) {
                    var format = options.locale.format;
                    daterange.begin = picker.startDate;
                    daterange.end = picker.endDate;
                    daterange.formattedDate = picker.startDate.format(format) + ' - ' + picker.endDate.format(format);
                }
                $(element).val(daterange.formattedDate);
                value(daterange);
            }
        });

        //when a user cancels the date, update the view model
        ko.utils.registerEventHandler(element, "cancel.daterangepicker", function (event, picker) {
            $(element).val('');
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                value(new Daterange());
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data('daterangepicker');
            if (picker) {
                picker.remove();
            }
        });
    }
};
