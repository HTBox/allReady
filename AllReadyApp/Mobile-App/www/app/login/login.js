"use strict";

angular
    .module("login", ["Backend"])
    .config(function($stateProvider, $urlRouterProvider) {
        $stateProvider.state("login", {
            url: "/login",
            templateUrl: "app/login/login.tpl.html",
            controller: "LoginController"
        });
    }).controller("LoginController", ["$scope", "$location", "Backend", function($scope, $location, Backend) {
        document.addEventListener("deviceready", function() {
            Backend.doLogin().then(function() {
                $location.url("/events");
            }, function() {
                // TODO Show login error
            });
        }, false);
    }]);
