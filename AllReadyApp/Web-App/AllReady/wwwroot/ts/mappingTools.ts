

export module HTBox.maps {
    var geoMaps = Microsoft.Maps;
    var BingMapKey = "ArclGe51CDYUCjW2tcjeuCSxXDg0z4_NFRCmVMpnpMY0qGUvPBazQIp9AjDgm2kv";

    export interface location {
        name: string,
        latitude: number,
        longitude: number,

    }
    export class Location implements location {
        name: string;
        latitude: number;
        longitude: number;
        constructor(config: any) {

            for (var p in config) {
                if (config.hasOwnProperty(p)) {
                    this[p] = config[p];
                }
            }
        }
    }


    export class MapRender {
        bingMap: any;
        constructor(divIdForMap: string) {
            this.bingMap = new geoMaps.Map(
                document.getElementById(divIdForMap),
                {
                    credentials: BingMapKey
                });
        };

        public getMyLocation(callback) {
            navigator.geolocation.getCurrentPosition(callback);
        }

        public zoomToMyLocation() {
            this.getMyLocation(pos => {
                var myPosition = {
                    name: 'my loc',
                    latitude: pos.coords.latitude,
                    longitude: pos.coords.longitude,
                };

                this.zoomToLocation(myPosition);
            });
        }

        public zoomToLocation(location: location) {
            var bingMap = this.bingMap;
            var zoomLocation = new geoMaps.Location(location.latitude, location.longitude));
            bingMap.setView({
                zoom: 10,
                center: zoomLocation
            });
        }

        public drawLocations(locations: Array<location>) {
            var bingMap = this.bingMap;
            var center = bingMap.getCenter();
            var pushpin = new geoMaps.Pushpin(center, null);

            locations.forEach(loc => {
                pushpin.setLocation(new geoMaps.Location(loc.latitude, loc.longitude));
            });

            bingMap.entities.push(pushpin);
            bingMap.setView({
                zoom: 10,
                center: center
            });
        }

        public drawCampaignLocations(campaigns: Array<any>) {
        }

        public drawAddress(address1, address2, city, state, postalCode, country) {
            var self = this;
            self.getGeoCoordinates(address1, address2, city, state, postalCode, country, location => {
                self.drawLocations([location]);
                self.zoomToLocation(location);
            });
        }

        getGeoCoordinates(address1, address2, city, state, postalCode, country, callbackFunction) {
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
                    var points = result.resourceSets[0].resources[0].geocodePoints[0];
                    var myLocation = {
                        name: lookupAddress,
                        latitude: points.coordinates[0],
                        longitude: points.coordinates[1],
                    };
                    callbackFunction(myLocation);
                },
                error: function (e) {
                    alert(e.statusText);
                }
            });
        }
    }
}

