

declare var modelActivities: any;

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

    function Activity(item) {
        for (var prop in item) {
            this[prop] = item[prop];
        }
        this.Name = this.Title;
        this.displayDate = function () {
            var start = this.StartDateTime.split('T')[0];
            var end = this.EndDateTime.split('T')[0];
            return start + ' : ' + end;
        }
    };

    function CampaignViewModel(activities) {
        var list = activities.map(function (item) { return new Activity(item); })

        this.activities = ko.observableArray(list).filterBeforeDate("EndDateTime").textFilter(["Title", "Description"]);
        this.resources = ko.observableArray([]);

        this.total = list.length;

    }

    ko.applyBindings(new CampaignViewModel(activities));
})(ko, $, modelActivities);
