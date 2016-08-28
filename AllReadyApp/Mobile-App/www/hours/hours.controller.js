"use strict";

angular
    .module("allReady.controllers")
  
    .controller("HoursController", ["$scope", "HoursService", function ($scope, HoursService) {
        $scope.setTimeIn = function () {
            var s = document.getElementById('signin')
            var h = HoursService.setTimeIn();
            s.innerHTML = h;
        }
        $scope.setTimeOut = function () {
            var s = document.getElementById('signout')
            var h = HoursService.setTimeOut();
            
            s.innerHTML = h
            
        }
    }]);
