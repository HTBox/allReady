///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, tenants) {

    function TenantViewModel(tenants) {
        this.tenants = ko.observableArray(tenants).textFilter(["Name","Description"]);
    }

    ko.applyBindings(new TenantViewModel(tenants));
})(ko, $, modelTenants);