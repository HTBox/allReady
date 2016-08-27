"use strict";

angular
    .module("Backend") //TODO get a better name for factory and module
    .factory("CacheManager", [function () {
        var rootKey = "allReadyEvents";
        var svc = {};

        function getRoot() {
            var stringifiedValue = window.localStorage.getItem(rootKey);
            return stringifiedValue ? JSON.parse(stringifiedValue) : null;
        }

        function saveRoot(rootObject) {
            window.localStorage.setItem(rootKey, JSON.stringify(rootObject));
        }

        svc.getEventList = function () {
            var rootValue = getRoot();

            if (!rootValue) {
                return null;
            }

            return rootValue.events || null;
        };

        svc.getEvent = function (id) {
            var rootValue = getRoot();

            if (!rootValue || !rootValue.events) {
                return null;
            }

            var event = null;

            rootValue.events.some(function (value) {
                if (value.Id === id) {
                    event = value;
                    return true;
                }

                return false;
            });

            return event;
        };

        svc.saveEventList = function (eventArray) {
            var rootValue = {
                events: eventArray
            };

            saveRoot(rootValue);
        };

        svc.saveEvent = function (event) {
            var rootValue = getRoot();

            if (rootValue && rootValue.events) {
                var idx = -1;

                rootValue.events.some(function (value, index) {
                    if (value.Id === event.id) {
                        idx = index;
                        return true;
                    }

                    return false;
                });

                if (idx !== -1) {
                    rootValue.events[idx] = event;
                } else {
                    rootValue.events.push(event);
                }
            } else {
                rootValue = {
                    events: [event]
                };
            }

            saveRoot(rootValue);
        };

        svc.clearEventCache = function () {
            if (getRoot()) {
                window.localStorage.removeItem(rootKey);
            }
        };

        return svc;
    }])
