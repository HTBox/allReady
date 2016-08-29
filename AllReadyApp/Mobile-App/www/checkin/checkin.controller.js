"use strict";

angular
    .module("allReady.controllers")
  
    .controller("CheckinController", ["$scope", "Backend", function ($scope, Backend) {
        var genericErrorMessage = "There was a problem checking you in to the event. Please make sure you have internet access and try again.";
        $scope.state = 0;
        $scope.errorMessage = "";

        $scope.checkin = function () {
            cordova.plugins.barcodeScanner.scan(function (result) {
                Backend.checkinEvent(result.text)
                    .then(function (success) {
                        if (success.data.NeedsSignup) {
                            return Backend.signUpAndCheckIn(result.text)
                                .then(function (signupSuccess) {
                                    $scope.state = 1;
                                });
                        } else {
                            $scope.state = 1;
                        }
                    })
                    .catch(function (fail) {
                        $scope.state = 2;

                        if (fail.message) {
                            $scope.errorMessage = fail.message;
                        } else {
                            $scope.errorMessage = genericErrorMessage;
                        }
                    });
            }, function (error) {
                $scope.state = 2;
                $scope.errorMessage = error;
            });
        };
    }]);
