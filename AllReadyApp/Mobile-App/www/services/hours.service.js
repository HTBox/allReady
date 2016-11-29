"use strict";

angular
    .module("Backend") //TODO get a better name for factory and module
    .factory("HoursService", [function () {
        var rootKey = "allReadyHours";

        return {
          setTimeIn: setTimeIn,
          setTimeOut: setTimeOut
        };

        function getRoot() {
            var stringifiedValue = window.localStorage.getItem(rootKey);
            return stringifiedValue ? JSON.parse(stringifiedValue) : null;
        }

        function saveRoot(rootObject) {
            window.localStorage.setItem(rootKey, JSON.stringify(rootObject));
        }

        function setTimeIn() {
            rootKey = 'TimeIn';

            var date = moment(Date.now()).utc().format('MM/DD/YYYY hh:mm');
            saveRoot(date);
            return date;
        }

        function setTimeOut() {
            rootKey = 'TimeOut';
            var date = moment(Date.now()).utc().format('MM/DD/YYYY hh:mm');
            saveRoot(date);
            return  date;
        }
}]);
