(function ($, ko) {
    var taskAdminViewModel = {
        expand: function (which, data, event) {
            var that = this;
            var target = $(event.currentTarget).next();
            target.toggle();
            var chevron = (target.is(":visible")) ? "fa fa-chevron-up" : "fa fa-chevron-down";
            $(event.currentTarget).children("span").removeClass().addClass(chevron);
        }
    };

    ko.applyBindings(taskAdminViewModel);
})($, ko);