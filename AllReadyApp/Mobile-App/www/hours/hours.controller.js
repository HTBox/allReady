"use strict";

angular
    .module("allReady.controllers")
    .controller("HoursController", ["$scope", "HoursService", function ($scope, HoursService) {
        $scope.started = false;
        $scope.times = [];

        $scope.toggle = toggle;
        $scope.setTimeIn = setTimeIn;
        $scope.setTimeOut = setTimeOut;

        function toggle() {
          if ($scope.started) {
            setTimeOut();
            $scope.started = false;
            return;
          }

          setTimeIn();
          $scope.started = true;
        }

        function setTimeIn() {
          $scope.times.unshift({time: 'Time in: ' + HoursService.setTimeIn()});
        }

        function setTimeOut() {
          $scope.times.unshift({time: 'Time out: ' + HoursService.setTimeOut()});
        }
    }]);
