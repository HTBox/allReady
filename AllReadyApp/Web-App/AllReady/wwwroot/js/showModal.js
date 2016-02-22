///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

var HTBox = HTBox || {};
(function() {
    "use strict";
    //
    // This is a wrapper around the Bootstrap modal dialog that encapsulates the Bootstrap modal with a Knockout view model,
    // providing a clear separation of concerns between the view model and UI.
    //
    var showModal = function (options) {
        var self = this;
        if (typeof options === "undefined") throw new Error("An options argument is required.");
        if (typeof options.viewModel !== "object") throw new Error("options.viewModel is required.");
        if (typeof options.onClose !== "function") throw new Error("options.onClose is required.");
        if (!options.modalId) throw new Error("options.modalId is required.");

        var viewModel = options.viewModel;
        var modalId = options.modalId;
        var $modalElement = $("#" + modalId);

        ko.cleanNode($modalElement[0]);
        ko.applyBindings(viewModel, $modalElement[0]);
        showTwitterBootstrapModal($modalElement);

        var hide = function () {
            this.$modalElement.modal("hide");
        }

        var show = function() {
            this.$modalElement.modal("show");
        }

        return {
            $modalElement: $modalElement,
            hide: hide,
            show: show,
            onClose: options.onClose
        }
    };

    function showTwitterBootstrapModal($modalelement) {
        $modalelement.modal({
            // Clicking the backdrop, or pressing Escape, shouldn't automatically close the modal by default.
            // The view model should remain in control of when to close.
            backdrop: "static",
            keyboard: false
        });
    };
    
    HTBox.showModal = showModal;
}());