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
        this.Name = this.Title;
        this.displayDate = function () {
            var start = moment(this.StartDateTime).utcOffset(this.StartDateTime).format("dddd, MMMM Do YYYY");
            var end = moment(this.EndDateTime).utcOffset(this.EndDateTime).format("dddd, MMMM Do YYYY");
            return start + ' - ' + end;
        };
        
        return this;
    }
    function CampaignViewModel(events) {
        var list = events.map(function (item) { return new Event(item); })

        this.events = ko.observableArray(list).filterBeforeDate("EndDateTime").textFilter(["Title", "Description"]);
        this.resources = ko.observableArray([]);

        this.total = list.length;

    }

    ko.applyBindings(new CampaignViewModel(events));
})(ko, $, modelEvents);
