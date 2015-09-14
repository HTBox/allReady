///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, campaigns) {
    function CampaignsViewModel(campaigns) {
        this.searchTerm = ko.observable();
        this.campaignsNearZip = ko.observableArray();
        this.loadingDone = ko.observable(false);
        this.handleEnter = function (data, event) {
            if (event.keyCode === 13) {
                this.searchCampaigns();
            }
            return true;
        };
        this.searchCampaigns = function () {
            var self = this;
            var term = this.searchTerm();
            if (term) {
                $.get('/api/campaign/search/?zip=' + term + '&miles=10')
                .done(function (data) {
                    if (data) {
                        self.campaignsNearZip(data);
                    }
                    self.loadingDone(true);
                });
            }
        };
        this.campaigns = ko.observableArray(campaigns).filterBeforeDate("EndDate").textFilter(["Name", "Description"]);
    }
    ko.applyBindings(new CampaignsViewModel(campaigns));
})(ko, $, modelCampaigns);
