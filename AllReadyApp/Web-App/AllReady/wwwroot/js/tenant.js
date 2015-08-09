///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, campaigns) {

    function TenantViewModel(campaigns) {
        this.campaigns = ko.observableArray(campaigns).filterBeforeDate("EndDate").textFilter(["Name","Description"]);
    }

    ko.applyBindings(new TenantViewModel(campaigns));
})(ko, $, modelCampaigns);