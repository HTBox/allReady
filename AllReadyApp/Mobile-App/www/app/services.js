"use strict";

angular
    .module("Backend", []) //TODO get a better name for factory and module
    .factory("CacheManager", [function () {
        var rootKey = "allReadyActivities";
        var svc = {};

        function getRoot() {
            var stringifiedValue = window.localStorage.getItem(rootKey);
            return stringifiedValue ? JSON.parse(stringifiedValue) : null;
        }

        function saveRoot(rootObject) {
            window.localStorage.setItem(rootKey, JSON.stringify(rootObject));
        }

        svc.getActivityList = function () {
            var rootValue = getRoot();

            if (!rootValue) {
                return null;
            }

            return rootValue.activities || null;
        };

        svc.getActivity = function (id) {
            var rootValue = getRoot();

            if (!rootValue || !rootValue.activities) {
                return null;
            }

            var activity = null;

            rootValue.activities.some(function (value) {
                if (value.Id === id) {
                    activity = value;
                    return true;
                }

                return false;
            });

            return activity;
        };

        svc.saveActivityList = function (activityArray) {
            var rootValue = {
                activities: activityArray
            };

            saveRoot(rootValue);
        };

        svc.saveActivity = function (activity) {
            var rootValue = getRoot();

            if (rootValue && rootValue.activities) {
                var idx = -1;

                rootValue.activities.some(function (value, index) {
                    if (value.Id === activity.id) {
                        idx = index;
                        return true;
                    }

                    return false;
                });

                if (idx !== -1) {
                    rootValue.activities[idx] = activity;
                } else {
                    rootValue.activities.push(activity);
                }
            } else {
                rootValue = {
                    activities: [activity]
                };
            }

            saveRoot(rootValue);
        };

        svc.clearActivityCache = function () {
            if (getRoot()) {
                window.localStorage.removeItem(rootKey);
            }
        };

        return svc;
    }])
    .factory("Backend", ["$http", "$q", "CacheManager", function ($http, $q, CacheManager) {
        var svc = {};
        var protocol = "https://";
        var domainUrl;
        var baseUrl;

        var readStringFromFileAtPath = function (pathOfFileToReadFrom) {
            var request = new XMLHttpRequest();
            request.open("GET", pathOfFileToReadFrom, false);
            request.send(null);
            var returnValue = request.responseText;

            return returnValue;
        }

        var getDomainUrl = function readDomainUrl() {
            var config = readStringFromFileAtPath(cordova.file.applicationDirectory + "config.xml");
            var parser = new DOMParser();
            var doc = parser.parseFromString(config, "application/xml");
            domainUrl = doc.getElementsByTagName("preference").item(1).nodeValue;
            baseUrl = protocol + domainUrl + "/";
        };

        getDomainUrl();

        svc.getActivities = function (forceWebQuery) {
            forceWebQuery = typeof forceWebQuery !== "undefined" ? forceWebQuery : false;

            if (!forceWebQuery) {
                var cachedList = CacheManager.getActivityList();

                if (cachedList) {
                    return $q.when(cachedList);
                }
            }

            // TODO Use the real API URL
            return $http.get(baseUrl + "api/activity")
                .then(function (result) {
                    // <TEMP> For testing purposes
                    // Limit results to 10
                    result.data.length = 10;
                    // Add a default location if needed
                    result.data.forEach(function (activity) {
                        activity.location = activity.location || "Seattle Red Cross";
                    });
                    // </TEMP>

                    CacheManager.saveActivityList(result.data);
                    return result.data;
                });
        };

        svc.getActivity = function (id, forceWebQuery) {
            forceWebQuery = typeof forceWebQuery !== "undefined" ? forceWebQuery : false;

            if (!forceWebQuery) {
                var cachedActivity = CacheManager.getActivity(id);

                if (cachedActivity) {
                    return $q.when(cachedActivity);
                }
            }

            // TODO Use the real API URL
            return $http.get(baseUrl + "api/activity/" + id)
                .then(function (result) {
                    CacheManager.saveActivity(result.data);
                    return result.data;
                });
        };

        svc.checkinActivity = function (checkinCode) {
            var regex = new RegExp("^https?://" + domainUrl + "/api/activity/[0-9]+/checkin$");
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

        svc.doLogin = function(){
            var deferred = $q.defer();
            var done = false;

            var ref = cordova.InAppBrowser.open(baseUrl + "api/me", "_blank", "location=no");
            ref.addEventListener("loadstop", function(e) {
                if (e.url === baseUrl + "api/me") {
                    if (!done) {
                        ref.close();
                        done = true;
                        deferred.resolve();
                    }
                }
            });

            ref.addEventListener("loaderror", function(){
                if (!done){
                    done = true;
                    deferred.reject();
                }
            });
            return deferred.promise;
        };

        return svc;
    }]);

