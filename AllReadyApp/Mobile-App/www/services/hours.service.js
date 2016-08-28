"use strict";

angular
    .module("Backend") //TODO get a better name for factory and module
    .factory("HoursService", ["$http", "$q", "CacheManager", function ($http, $q, CacheManager) {
        var rootKey = "allReadyHours";
        var svc = {};

        var protocol = "http://";
        var domainUrl = "localhost:48408"; // TODO: Update when the site is deployed for real
        var baseUrl = protocol + domainUrl + "/";

        function getRoot() {
            var stringifiedValue = window.localStorage.getItem(rootKey);
            return stringifiedValue ? JSON.parse(stringifiedValue) : null;
        }

        function saveRoot(rootObject) {
            window.localStorage.setItem(rootKey, JSON.stringify(rootObject));
        }

        svc.setTimeIn = function () {
            rootKey = 'TimeIn';

            var d = moment(Date.now()).utc().format('MM/DD/YYYY hh:mm')
            saveRoot(d);
            return d;
        };

        svc.setTimeOut = function () {
            rootKey = 'TimeOut';
            var d = moment(Date.now()).utc().format('MM/DD/YYYY hh:mm')
            saveRoot(d);
            return  d;
        };

        return svc;

}]);