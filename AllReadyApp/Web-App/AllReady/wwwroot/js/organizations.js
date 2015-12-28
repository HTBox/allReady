///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, organizations) {
    function Orgainzation(item) {
        for (var prop in item) {
            this[prop] = item[prop];
        }
        //this.displayDate = function () {
        //    var start = this.StartDate.split('T')[0];
        //    var end = this.EndDate.split('T')[0];
        //    return start + ' : ' + end;
        //}
        return this;
    }

    function OrganizationsViewModel(organizations) {
        var list = organizations.map(function (item) { return new Orgainzation(item); })

        this.organizations = ko.observableArray(organizations).textFilter(["Name", "Description"]);
        this.total = organizations.length;
    }

    ko.applyBindings(new OrganizationViewModel(organizations));
})(ko, $, modelOrganizations);