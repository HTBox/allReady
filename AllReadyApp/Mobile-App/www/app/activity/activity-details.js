"use strict";

angular
    .module("activity-details", ["Backend"])
    .config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
        .state("activity-details", {
            url: "/activities/:id",
            templateUrl: "app/activity/activity-details.tpl.html",
            controller: "ActivityDetailsController",
            resolve: {
                activityDetails: function (Backend, $stateParams) {
                    return Backend.getActivity($stateParams.id);
                }
            }
        });
    })
    .controller("ActivityDetailsController", ["$scope", "$location", "activityDetails", function ($scope, $location, activityDetails) {
        $scope.data = activityDetails;
        $scope.doCheckin = function () {
            $location.url("/checkin");
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
