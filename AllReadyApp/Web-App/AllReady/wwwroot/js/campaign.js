///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />


/* TODO: fix me
$(document).ready(function ()
{ 
    ko.applyBindings(new ResourcesViewModel('Test'));
});
*/
function ResourcesViewModel(category)
{
    var self = this;
    self.resources = ko.observableArray([]);

    $.ajax({
        type: "GET",
        url: '/api/resource/search/?category=' + category,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data)
        {
            self.resources(data);
        },
        error: function (error) {
            alert(error.status + error.statusText);
        }
    });
}


(function (ko, $, activities) {

    function CampaignViewModel(activities) {
        this.activities = ko.observableArray(activities).filterBeforeDate("EndDateTimeUtc").textFilter(["Title","Description"]);
    }

    ko.applyBindings(new CampaignViewModel(activities));
})(ko, $, modelActivities);
