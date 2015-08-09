///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, campaigns) {

    function CampaignsViewModel(campaigns) {
        this.campaigns = ko.observableArray(campaigns).filterBeforeDate("EndDate").textFilter(["Name", "Description"]);
    }

    ko.applyBindings(new CampaignsViewModel(campaigns));
})(ko, $, modelCampaigns);