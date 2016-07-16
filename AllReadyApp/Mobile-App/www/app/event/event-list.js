"use strict";

angular
    .module("event-list", ["Backend"])
    .config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
        .state("event-list", {
            url: "/events",
            templateUrl: "app/event/event-list.tpl.html",
            controller: "EventListController"
        });
    })
    .controller("EventListController", ["$scope", "$location", "Backend", function ($scope, $location, Backend) {
        Backend.getEvents().then(function (result) {
            $scope.eventList = result;
        });

        $scope.onSelect = function (n) {
            $location.url("/events/" + n.Id);
        };

        $scope.doRefresh = function () {
            Backend.getEvents(true /* forceWebQuery */).then(function (result) {
                $scope.eventList = result;
                $scope.$broadcast("scroll.refreshComplete");
            });
        };

        $scope.doCheckin = function () {
            $location.url("/checkin");
        };
    }]).filter("dateFilter", function ($filter) {
        return function (input) {
            if (input == null) { return ""; }
            var day = moment(input);
            return day.fromNow();
        };
    });
