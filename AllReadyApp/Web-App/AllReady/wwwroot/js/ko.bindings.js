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
            accordionExpandIconClass = "." + (ko.utils.unwrapObservable(options.expandIcon) || "accordion-icon-direction"),
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
            var $accordionExpandIconClass = $currentPanel.find(accordionExpandIconClass);

            $accordionExpandIconClass.toggleClass("fa-caret-down fa-caret-up");
        }
    }
};