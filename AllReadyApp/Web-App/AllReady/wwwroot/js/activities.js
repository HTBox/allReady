///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
///<reference path="../lib/moment/moment.js" />

(function (ko, $) {

    var getActivities = $.getJSON("/api/activity");

    function ActivitiesViewModel() {
        this.showOld = ko.observable(false);
        this.activities = ko.observableArray([]);
        this.selectedActivities = ko.computed(function () {
            var self = this;
            return ko.utils.arrayFilter(this.activities(), function (activity) {
                return self.showOld() || moment(activity.EndDateTime).isAfter(moment());
            })
        }, this);
        this.loading = ko.observable(true);
    }
    ActivitiesViewModel.prototype = {
        toggle: function (observable) {
            return function toggleObservable() {
                observable(!observable());
            }
        }
    };

    var viewModel = new ActivitiesViewModel();
    ko.applyBindings(viewModel);

    getActivities.done(function (activities) {
        viewModel.loading(false);
        viewModel.activities(activities);
    });
})(ko, $);