///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, activities) {

    function CampaignViewModel(activities) {
        this.activities = ko.observableArray(activities).filterBeforeDate("EndDateTimeUtc");
    }

    ko.applyBindings(new CampaignViewModel(activities));
})(ko, $, modelActivities);