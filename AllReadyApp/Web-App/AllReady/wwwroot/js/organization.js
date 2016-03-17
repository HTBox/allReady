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
            return start + ' - ' + end;
        }
        return this;
    }
    function OrganizationViewModel(campaigns) {
        var list = campaigns.map(function (item) { return new Campaign(item); })

        this.campaigns = ko.observableArray(list).filterBeforeDate("EndDate").textFilter(["Name", "Description"]);
        this.total = campaigns.length;
    }

    ko.applyBindings(new OrganizationViewModel(campaigns));
})(ko, $, modelCampaigns);