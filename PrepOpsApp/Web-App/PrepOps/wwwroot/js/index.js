(function (ko, $, navigator) {

    var indexViewModel = {
        status: ko.observable(),
        searchTerm: ko.observable(),
        activities: ko.observableArray(),
        loadingDone: ko.observable(false),
        handleEnter: function(data, event) {
            if (event.keyCode === 13) {
                this.searchActivities();
            }
            return true;
        },
        searchActivities: function () {
            var self = this;
            var term = this.searchTerm();
            if (term !== undefined) {
                $.get('/api/activity/search/?zip=' + term + '&miles=10')
                .done(function (data) {
                    if (data) {
                        indexViewModel.activities(data.slice(0, 3));
                        indexViewModel.loadingDone(true);
                    } else {
                        self.status('noresults');
                    }
                }).fail(function () {
                    self.status('fail');
                });
            }
        }
    };
    ko.applyBindings(indexViewModel);
})(ko, $, window.navigator);