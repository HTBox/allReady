"use strict";

angular
    .module("allReadyApp", ["ionic", "event-list", "event-details", "login", "checkin"])
    .config(function ($urlRouterProvider, $ionicConfigProvider) {
        $ionicConfigProvider.backButton.previousTitleText(false).text("");

        $urlRouterProvider.otherwise("/login");
        // Other individual routes are defined in respective controllers
    })
    .controller("MainPageCtrl", ["$scope",
    function ($scope) {
        $scope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
            // TODO Show loading indicator on page
        });

        $scope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
            // TODO Stop showing loading indicator when state change successds
        });

        $scope.$on("$stateChangeError", function (event, toState, toParams, fromState, fromParams, error) {
            // TODO Show error message when state change stops
        });
    }]);
