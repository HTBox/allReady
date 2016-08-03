///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, campaigns) {
    function Campaign(item) {
        for (var prop in item) {
            this[prop] = item[prop];
        }
        this.displayDate = function () {
            var start = moment(this.startDate).utcOffset(this.startDate).format("dddd, MMMM Do YYYY");
            var end = moment(this.endDate).utcOffset(this.endDate).format("dddd, MMMM Do YYYY");
            return start + ' - ' + end;
        }
        return this;
    }
    function OrganizationViewModel(campaigns) {
        var list = campaigns.map(function(item) { return new Campaign(item); });

        this.campaigns = ko.observableArray(list).filterBeforeDate("endDate").textFilter(["name", "description"]);
        debugger;
        this.total = campaigns.length;
    }

    ko.applyBindings(new OrganizationViewModel(campaigns));
})(ko, $, modelCampaigns);