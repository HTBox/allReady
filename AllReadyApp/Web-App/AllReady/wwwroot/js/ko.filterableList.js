///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
///<reference path="../lib/moment/moment.js" />

ko.observableArray.fn.filterList = function (filterFn) {
    var inputToFilterFn = this.filtered || this;
    //If this observable array has already been filtered, append this function as a filter instead of replacing
    this.filtered = ko.computed(function () {
        return filterFn(inputToFilterFn);
    }, this);
    return this;
};

ko.observableArray.fn.filterBeforeDate = function (dateProperty, showOld, date) {
    var showOld = ko.observable(showOld || false);
    showOld.toggle = function () {
        showOld(!showOld());
    };
    this.showOld = showOld;
    return this.filterList(function (observableArray) {
        return ko.utils.arrayFilter(observableArray(), function (item) {
            return showOld() || moment(item[dateProperty]).isAfter(moment(date || undefined));
        });
    });
};

ko.observableArray.fn.textFilter = function (searchProperties, initialTerm) {
    if (!$.isArray(searchProperties)) {
        searchProperties = [searchProperties];
    }
    var searchTerm = ko.observable(initialTerm || "");
    this.searchTerm = searchTerm;
    return this.filterList(function (observableArray) {
        return ko.utils.arrayFilter(observableArray(), function (item) {
            for (var i = 0; i < searchProperties.length; i++) {
                if (typeof item[searchProperties[i]] === "string" && ~item[searchProperties[i]].toLowerCase().indexOf(searchTerm().toLowerCase())) {
                    return true;
                }
            }
            return false;
        });
    });
};