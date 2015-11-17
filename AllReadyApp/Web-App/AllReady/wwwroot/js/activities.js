(function (ko, $) {
    var getActivities = $.getJSON("/api/activity");
    function ActivitiesViewModel() {
        this.activities = ko.observableArray([]).filterBeforeDate("EndDateTime").textFilter(["Title", "Description"]);
        this.loading = ko.observable(true);
    }
    var viewModel = new ActivitiesViewModel();
    ko.applyBindings(viewModel);
    getActivities.done(function (activities) {
        viewModel.loading(false);
        activities.sort(function (a1, a2) {
            return moment(a1.StartDateTime).toDate().valueOf() - moment(a2.StartDateTime).toDate().valueOf();
        });
        viewModel.activities(activities);
    });
})(ko, $);
