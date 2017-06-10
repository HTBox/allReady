///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, campaigns) {
    function Campaign(item) {
        for (var prop in item) {
            this[prop] = item[prop];
        }
        this.displayDate = function () {
            var start = moment(this.StartDate).utcOffset(this.StartDate).format("dddd, MMMM Do YYYY");
            var end = moment(this.EndDate).utcOffset(this.EndDate).format("dddd, MMMM Do YYYY");
            return start + ' to ' + end;
        }
        return this;
    }

    function CampaignsViewModel(campaigns) {
        this.searchTerm = ko.observable();
        this.campaignsNearPostalCode = ko.observableArray();
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
                $.get('/api/campaign/search/?postalCode=' + term + '&miles=10')
                .done(function (data) {
                    if (data) {
                        self.campaignsNearPostalCode(data);
                    }
                    self.loadingDone(true);
                });
            }
        };

        var list = campaigns.map(function (item) { return new Campaign(item); })

        this.campaigns = ko.observableArray(list).filterBeforeDate("EndDate").textFilter(["Name", "Description"]);
        this.featuredCampaigns = campaigns.filter(function (c) { return c.Featured; }).length;
        this.total = list.length;
    }
    ko.applyBindings(new CampaignsViewModel(campaigns));

})(ko, $, modelCampaigns);