///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
///<reference path="../lib/moment/moment.js" />

(function (ko, $) {

    var getEvents = $.getJSON("/api/activity");

    function EventsViewModel() {
        this.Events = ko.observableArray([]).filterBeforeDate("EndDateTime").textFilter(["Title","Description"]);
        this.loading = ko.observable(true);
    }

    var viewModel = new EventsViewModel();
    ko.applyBindings(viewModel);

    getEvents.done(function (Events) {
        viewModel.loading(false);
        Events.sort(function (a1, a2) {
            return moment(a1.StartDateTime).toDate().valueOf() - moment(a2.StartDateTime).toDate().valueOf();
        });
        viewModel.Events(Events);
    });
})(ko, $);