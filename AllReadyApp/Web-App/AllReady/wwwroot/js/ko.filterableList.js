///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />
///<reference path="../lib/moment/moment.js" />

ko.observableArray.fn.asFilterableList = function koFilterableList(filterFn) {
    this.filtered = ko.computed(function () {
        return filterFn(this);
    }, this);
    return this;
};

ko.observableArray.fn.filterBeforeDate = function koDateFiltered(dateProperty, showOld, date) {
    var showOld = ko.observable(showOld || false);
    showOld.toggle = function () {
        showOld(!showOld());
    };
    this.showOld = showOld;
    return this.asFilterableList(function (observableArray) {
        return ko.utils.arrayFilter(observableArray(), function (item) {
            return showOld() || moment(item[dateProperty]).isAfter(moment(date || undefined));
        });
    });
};