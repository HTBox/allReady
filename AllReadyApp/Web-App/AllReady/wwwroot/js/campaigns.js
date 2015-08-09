///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, campaigns) {
    function CampaignsViewModel(campaigns) {
    }

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
        },
        campaigns : ko.observableArray(campaigns).filterBeforeDate("EndDate").textFilter(["Name", "Description"])
    };
    ko.applyBindings(campaignViewModel);
})(ko, $, modelCampaigns);
