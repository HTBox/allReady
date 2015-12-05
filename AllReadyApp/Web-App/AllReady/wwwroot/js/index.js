(function (ko, $, navigator) {

    var indexViewModel = {
        status: ko.observable(),
        activities: ko.observableArray(),
        loadingDone: ko.observable(false),
    };
    ko.applyBindings(indexViewModel);
})(ko, $, window.navigator);