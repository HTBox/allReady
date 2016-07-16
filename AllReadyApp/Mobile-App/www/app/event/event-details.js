"use strict";

angular
    .module("event-details", ["Backend"])
    .config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
        .state("event-details", {
            url: "/events/:id",
            templateUrl: "app/event/event-details.tpl.html",
            controller: "EventDetailsController",
            resolve: {
                eventDetails: function (Backend, $stateParams) {
                    return Backend.getEvent($stateParams.id);
                }
            }
        });
    })
    .controller("EventDetailsController", ["$scope", "$location", "eventDetails", function ($scope, $location, eventDetails) {
        $scope.data = eventDetails;
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
