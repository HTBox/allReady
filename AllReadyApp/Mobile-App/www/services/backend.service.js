"use strict";

angular
    .module("Backend") //TODO get a better name for factory and module
    .factory("Backend", ["$http", "$q", "CacheManager", "ApiEndpoint", function ($http, $q, CacheManager, ApiEndpoint) {
        var svc = {};
        //var protocol = "http://";
        //var domainUrl = "localhost:48408"; // TODO: Update when the site is deployed for real
        //var baseUrl = protocol + domainUrl + "/";
        var baseUrl = ApiEndpoint.url;

        svc.getEvents = function (forceWebQuery) {
            forceWebQuery = typeof forceWebQuery !== "undefined" ? forceWebQuery : false;

            if (!forceWebQuery) {
                var cachedList = CacheManager.getEventList();

                if (cachedList) {
                    return $q.when(cachedList);
                }
            }

            return $http.get(baseUrl + "api/event")
                .then(function (result) {
                    result.data.forEach(function (event) {
                        event.location = event.location || "Not Set";
                    });

                    CacheManager.saveEventList(result.data);
                    return result.data;
                });
        };

        svc.getEvent = function (id, forceWebQuery) {
            forceWebQuery = typeof forceWebQuery !== "undefined" ? forceWebQuery : false;

            if (!forceWebQuery) {
                var cachedEvent = CacheManager.getEvent(id);

                if (cachedEvent) {
                    return $q.when(cachedEvent);
                }
            }

            // TODO Use the real API URL
            return $http.get(baseUrl + "api/event/" + id)
                .then(function (result) {
                    CacheManager.saveEvent(result.data);
                    return result.data;
                });
        };

        svc.checkinEvent = function (checkinCode) {
            var regex = new RegExp("^https?://" + domainUrl + "/api/event/[0-9]+/checkin$");
            if (checkinCode.match(regex)) {
                return $http.put(checkinCode);
            } else {
                // This QR code is not one that we want to deal with: error out
                return $q.reject(new Error("We didn't recognize the QR code."));
            }
        };

        svc.signUpAndCheckIn = function (checkinCode) {
            var signupCode = checkinCode.replace("checkin", "signup");
            return $http.post(signupCode)
                .then(function () {
                    $http.put(checkinCode);
                });
        };

        svc.doLogin = function (username, password) {
            return $http.post(baseUrl + "api/me/login",
                {
                    "Email": username,
                    "Password": password,
                    "RememberMe": "true"
                }, {
                    headers: {
                        'Content-Type': 'application/json; charset=UTF-8'
                        //'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }
            ).then(function () {
                console.log("Logged in");
            });
        //var deferred = $q.defer();
        //var done = false;

        //var ref = cordova.InAppBrowser.open(baseUrl + "api/me", "_blank", "location=no");
        //ref.addEventListener("loadstop", function (e) {
        //    if (e.url.toLowerCase().indexOf("api/me") != -1) {
        //        if (!done) {
        //            ref.close();
        //            done = true;
        //            deferred.resolve();
        //        }
        //    }
        //});

        //ref.addEventListener("loaderror", function () {
        //    if (!done) {
        //        done = true;
        //        deferred.reject();
        //    }
        //});
        //return deferred.promise;
    };

return svc;
}]);