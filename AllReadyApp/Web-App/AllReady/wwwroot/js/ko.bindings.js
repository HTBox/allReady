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
            expandIconClass = "." + (ko.utils.unwrapObservable(options.expandIcon) || "accordion-expand-icon"),
            collapseIconClass = "." + (ko.utils.unwrapObservable(options.collapseIcon) || "accordion-collapse-icon"),
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
            var $currentPanel = $(event.target).closest(itemClass);
            var $currentExpandIcon = $currentPanel.find(expandIconClass);
            var $currentCollapseIcon = $currentPanel.find(collapseIconClass);
            $currentExpandIcon.hide();
            $currentCollapseIcon.show();
        });

        $(elem).on("hide.bs.collapse", function (event) {
            var $currentPanel = $(event.target).closest(itemClass);
            var $currentExpandIcon = $currentPanel.find(expandIconClass);
            var $currentCollapseIcon = $currentPanel.find(collapseIconClass);
            $currentExpandIcon.show();
            $currentCollapseIcon.hide();
        });

        // if initial open item specified, expand it
        if (openItem) {
            items.eq(openItem).collapse("show");
        };

        function initializeAccordion() {
            // activate all items
            $(elem).find(contentClass).collapse({ parent: elem, toggle: false });

            // hide all of the 'collapse' icons, the 'expand' icons will be initially showing
            $(elem).find(collapseIconClass).hide();
        }
    }
};