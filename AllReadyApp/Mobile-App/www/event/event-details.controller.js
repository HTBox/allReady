"use strict";

angular
    .module("allReady.controllers")
   
    .controller("EventDetailsController", ["$scope", "$state", "eventDetails", function ($scope, $state, eventDetails) {
        $scope.data = eventDetails;
        $scope.doCheckin = function () {
            $state.go("app.checkin");
        };
        $scope.avatarUrl = function () {
            return "img/missing.png";
        };
    }])
    .filter("dateFilter", function ($filter) {
        return function (input) {
            if (input == null) { return "[Missing date]"; }

            var day = moment(input);
            return day.fromNow();
        };
    })
    .filter("timeFilter", function ($filter) {
        return function (input) {
            if (input == null) { return "[missing time]"; }

            var time = moment(input);
            return time.format("hA");
        };
    });
