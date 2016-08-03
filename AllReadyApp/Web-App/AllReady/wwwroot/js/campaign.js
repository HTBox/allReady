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


(function (ko, $, events) {
    function Event(item) {
        for (var prop in item) {
            this[prop] = item[prop];
        }
        this.name = this.title;
        this.displayDate = function () {
            var start = moment(this.startDateTime).utcOffset(this.startDateTime).format("dddd, MMMM Do YYYY");
            var end = moment(this.endDateTime).utcOffset(this.endDateTime).format("dddd, MMMM Do YYYY");
            return start + ' - ' + end;
        };
        
        return this;
    }
    function CampaignViewModel(events) {
        var list = events.map(function (item) { return new Event(item); })

        this.events = ko.observableArray(list).filterBeforeDate("endDateTime").textFilter(["title", "description"]);
        this.resources = ko.observableArray([]);

        this.total = list.length;

    }

    ko.applyBindings(new CampaignViewModel(events));
})(ko, $, modelEvents);
