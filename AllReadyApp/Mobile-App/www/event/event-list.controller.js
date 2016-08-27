"use strict";

angular
    .module("allReady.controllers")
    .controller("EventListController", ["$scope", "$location", "Backend", function ($scope, $location, Backend) {
        Backend.getEvents().then(function (result) {
            $scope.eventList = result;
        });

        $scope.onSelect = function (n) {
            $location.url("/app/eventdetails/" + n.Id);
        };

        $scope.doRefresh = function () {
            Backend.getEvents(true /* forceWebQuery */).then(function (result) {
                $scope.eventList = result;
                $scope.$broadcast("scroll.refreshComplete");
            });
        };

        $scope.doCheckin = function () {
            $location.url("/app/checkin");
        };
    }]).filter("dateFilter", function ($filter) {
        return function (input) {
            if (input == null) { return ""; }
            var day = moment(input);
            return day.fromNow();
        };
    });
