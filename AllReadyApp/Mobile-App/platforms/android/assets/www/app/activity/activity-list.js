"use strict";

angular
    .module("activity-list", ["Backend"])
    .config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
        .state("activity-list", {
            url: "/activities",
            templateUrl: "app/activity/activity-list.tpl.html",
            controller: "ActivityListController"
        });
    })
    .controller("ActivityListController", ["$scope", "$location", "Backend", function ($scope, $location, Backend) {
        Backend.getActivities().then(function (result) {
            $scope.activityList = result;
        });

        $scope.onSelect = function (n) {
            $location.url("/activities/" + n.Id);
        };

        $scope.doRefresh = function () {
            Backend.getActivities(true /* forceWebQuery */).then(function (result) {
                $scope.activityList = result;
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
