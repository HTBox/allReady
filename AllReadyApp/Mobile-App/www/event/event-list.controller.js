"use strict";

angular
    .module("allReady.controllers")
    .controller("EventListController", ["$scope", "$state", "Backend", function ($scope, $state, Backend) {
        Backend.getEvents().then(function (result) {
            $scope.eventList = result;
        });

        $scope.onSelect = function (n) {
            $state.go("app.eventdetails", { id: n.Id });
        };

        $scope.doRefresh = function () {
            Backend.getEvents(true /* forceWebQuery */).then(function (result) {
                $scope.eventList = result;
                $scope.$broadcast("scroll.refreshComplete");
            });
        };

        $scope.doCheckin = function () {
            $state.go("app.checkin");
        };
    }]).filter("dateFilter", function ($filter) {
        return function (input) {
            if (input == null) { return ""; }
            var day = moment(input);
            return day.fromNow();
        };
    });
