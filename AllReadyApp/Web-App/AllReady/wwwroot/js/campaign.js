///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, activities) {

    function CampaignViewModel(activities) {
        this.activities = ko.observableArray(activities).filterBeforeDate("EndDateTimeUtc").textFilter(["Title","Description"]);
    }

    ko.applyBindings(new CampaignViewModel(activities));
})(ko, $, modelActivities);