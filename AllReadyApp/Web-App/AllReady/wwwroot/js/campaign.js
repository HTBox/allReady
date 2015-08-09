(function (ko, $, navigator) {

    var campaignViewModel = {
        status: ko.observable(),
        searchTerm: ko.observable(),
        campaigns: ko.observableArray(),
        loadingDone: ko.observable(false),
        handleEnter: function (data, event) {
            if (event.keyCode === 13) {
                this.searchCampaigns();
            }
            return true;
        },
        searchCampaigns: function () {
            var self = this;
            var term = this.searchTerm();
            if (term !== undefined) {
                $.get('/api/campaign/search/?zip=' + term + '&miles=10')
                .done(function (data) {
                    if (data) {
                        campaignViewModel.campaigns(data.slice(0, 3));
                        campaignViewModel.loadingDone(true);
                    } else {
                        self.status('noresults');
                    }
                }).fail(function () {
                    self.status('fail');
                });
            }
        }
    };
    ko.applyBindings(campaignViewModel);
})(ko, $, window.navigator);