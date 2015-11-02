///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, tenants) {
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

    function TenantViewModel(tenants) {
        var list = tenants.map(function (item) { return new Orgainzation(item); })

        this.tenants = ko.observableArray(tenants).textFilter(["Name", "Description"]);
        this.total = tenants.length;
    }

    ko.applyBindings(new TenantViewModel(tenants));
})(ko, $, modelTenants);