System.register([], function (exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var HTBox;
    return {
        setters: [],
        execute: function () {
            (function (HTBox) {
                var maps;
                (function (maps) {
                    var geoMaps = Microsoft.Maps;
                    var BingMapKey = "ArclGe51CDYUCjW2tcjeuCSxXDg0z4_NFRCmVMpnpMY0qGUvPBazQIp9AjDgm2kv";
                    var Location = /** @class */ (function () {
                        function Location(config) {
                            for (var p in config) {
                                if (config.hasOwnProperty(p)) {
                                    this[p] = config[p];
                                }
                            }
                        }
                        return Location;
                    }());
                    maps.Location = Location;
                    var MapRender = /** @class */ (function () {
                        function MapRender(divIdForMap) {
                            this.bingMap = new geoMaps.Map(document.getElementById(divIdForMap), {
                                credentials: BingMapKey
                            });
                        }
                        ;
                        MapRender.prototype.getMyLocation = function (callback) {
                            navigator.geolocation.getCurrentPosition(callback);
                        };
                        MapRender.prototype.drawLocations = function (locations) {
                            var bingMap = this.bingMap;
                            var center = bingMap.getCenter();
                            var pushpin = new geoMaps.Pushpin(center, null);
                            locations.forEach(function (loc) {
                                pushpin.setLocation(new geoMaps.Location(loc.latitude, loc.longitude));
                            });
                            bingMap.entities.push(pushpin);
                            bingMap.setView({
                                zoom: 10,
                                center: center
                            });
                        };
                        MapRender.prototype.getGeoCoordinates = function (address1, address2, city, state, postalCode, country, callbackFunction) {
                            var lookupAddress = "";
                            lookupAddress = lookupAddress + (address1 != undefined || address1 != null ? " " + address1 : null);
                            lookupAddress = lookupAddress + (address2 != undefined || address2 != null ? " " + address2 : null);
                            lookupAddress = lookupAddress + (city != undefined || city != null ? " " + city : null);
                            lookupAddress = lookupAddress + (state != undefined || state != null ? " " + state : null);
                            lookupAddress = lookupAddress + (postalCode != undefined || postalCode != null ? " " + postalCode : null);
                            lookupAddress = lookupAddress + (country != undefined || country != null ? " " + country : null);
                            var geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations?query=" +
                                encodeURIComponent(lookupAddress) +
                                "&key=" + BingMapKey;
                            $.ajax({
                                url: geocodeRequest,
                                dataType: "jsonp",
                                jsonp: "jsonp",
                                success: function (result) {
                                    var geoCoordinates = {
                                        latitude: result.resourceSets[0].resources[0].geocodePoints[0].coordinates[0],
                                        longitude: result.resourceSets[0].resources[0].geocodePoints[0].coordinates[1]
                                    };
                                    callbackFunction(geoCoordinates);
                                },
                                error: function (e) {
                                    alert(e.statusText);
                                }
                            });
                        };
                        return MapRender;
                    }());
                    maps.MapRender = MapRender;
                })(maps = HTBox.maps || (HTBox.maps = {}));
            })(HTBox || (HTBox = {}));
            exports_1("HTBox", HTBox);
        }
    };
});
//# sourceMappingURL=mappingTools.js.map