"use strict";

angular
    .module("checkin", ["Backend"])
    .config(function ($stateProvider, $urlRouterProvider) {
        $stateProvider
        .state("checkin", {
            url: "/checkin",
            templateUrl: "app/checkin/checkin.tpl.html",
            controller: "CheckinCtrl"
        });
    })
    .controller("CheckinCtrl", ["$scope", "Backend", function ($scope, Backend) {
        var genericErrorMessage = "There was a problem checking you in to the activity. Please make sure you have internet access and try again.";
        $scope.state = 0;
        $scope.errorMessage = "";

        $scope.checkin = function () {
            cordova.plugins.barcodeScanner.scan(function (result) {
                Backend.checkinActivity(result.text)
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
