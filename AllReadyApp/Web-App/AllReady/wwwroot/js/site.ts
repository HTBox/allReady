///<reference path="../../node_modules/bingmaps/scripts/MicrosoftMaps/Microsoft.Maps.d.ts" />

var geoHelper: Function = function (positionReceivedCallback: PositionCallback): void {
    console.log("geoHelper created");

    this.getMyLocation = function (): void {
        navigator.geolocation.getCurrentPosition(positionReceivedCallback);
    };
};

var BingMapKey: string = "ArclGe51CDYUCjW2tcjeuCSxXDg0z4_NFRCmVMpnpMY0qGUvPBazQIp9AjDgm2kv";

var renderBingMap: Function = function (divIdForMap: string, positionCoordsList: any): void {
    if (positionCoordsList) {
        var bingMap: Microsoft.Maps.Map = createBingMap(divIdForMap);
        var microsoftMapsLocations: Microsoft.Maps.Location[] = createMapLocationsAndAddPushPinsToMap(bingMap, positionCoordsList);
        setMapCenterAndZoom(bingMap, microsoftMapsLocations, 10);
    }
};

var renderRequestsMap: Function = function (divIdForMap: string, requestData: any[]): void {
    if (requestData) {
        var bingMap: Microsoft.Maps.Map = createBingMap(divIdForMap);
        addRequestPins(bingMap, requestData);
    }
};

var getGeoCoordinates: Function = function (address1: string, address2: string, city: string, state: string, postalCode:string, country: string, callbackFunction: Function): void {
    var lookupAddress: string = "";
    lookupAddress = lookupAddress + (address1 !== undefined || address1 !== null ? " " + address1 : "");
    lookupAddress = lookupAddress + (address2 !== undefined || address2 !== null ? " " + address2 : "");
    lookupAddress = lookupAddress + (city !== undefined || city !== null ? ", " + city : "");
    lookupAddress = lookupAddress + (state !== undefined || state !== null ? " " + state : "");
    lookupAddress = lookupAddress + (postalCode !== undefined || postalCode !== null ? " " + postalCode : "");
    lookupAddress = lookupAddress + (country !== undefined || country !== null ? " " + country : "");
    if (lookupAddress.trim() === "") {
        return;
    }
    var geocodeRequest: string = "http://dev.virtualearth.net/REST/v1/Locations?query=" +
        encodeURIComponent(lookupAddress) +
        "&key=" + BingMapKey;

    $.ajax({
        url: geocodeRequest,
        dataType: "jsonp",
        jsonp: "jsonp",
        success: function (result: any): void {
            var geoCoordinates: any = { latitude: 0, longitude: 0 };
            if (result.resourceSets[0].estimatedTotal > 0) {
                geoCoordinates = {
                    latitude: result.resourceSets[0].resources[0].geocodePoints[0].coordinates[0],
                    longitude: result.resourceSets[0].resources[0].geocodePoints[0].coordinates[1]
                };
            }
            callbackFunction(geoCoordinates);
        },
        error: function (e: JQueryXHR): void {
            alert(e.statusText);
        }
    });
};

function createBingMap(divIdForMap: string): Microsoft.Maps.Map {
    "use strict";

    return new Microsoft.Maps.Map(
        document.getElementById(divIdForMap), {
        credentials: BingMapKey
    });
}

function createMapLocationsAndAddPushPinsToMap(bingMap: Microsoft.Maps.Map, positionCoordsList: any[]): Microsoft.Maps.Location[] {
    "use strict";

    var microsoftMapsLocations: Microsoft.Maps.Location[] = [];
    $.each(positionCoordsList, function (index: number, LocationData: any): void {
        var microsoftMapsLocation: Microsoft.Maps.Location = new Microsoft.Maps.Location(LocationData.latitude, LocationData.longitude);
        microsoftMapsLocations.push(microsoftMapsLocation);
        var pushpin: Microsoft.Maps.Pushpin = new Microsoft.Maps.Pushpin(microsoftMapsLocation);
        bingMap.entities.push(pushpin);
    });
    return microsoftMapsLocations;
}

function addRequestPins(bingMap: Microsoft.Maps.Map, requestData: any[]): void {
    "use strict";

    var locations: Microsoft.Maps.Location[] = [];
    $.each(requestData, function (index: number, data: any): void {
        var location: Microsoft.Maps.Location = new Microsoft.Maps.Location(data.lat, data.long);
        locations.push(location);
        var order: number = index + 1;
        var pin: Microsoft.Maps.Pushpin = new Microsoft.Maps.Pushpin(location, {
            title: data.name, color: data.color, text: order.toString()
        });
        bingMap.entities.push(pin);
    });
    var rect: Microsoft.Maps.LocationRect = Microsoft.Maps.LocationRect.fromLocations(locations);
    bingMap.setView({ bounds: rect, padding:80 });
}

function setMapCenterAndZoom(bingMap: Microsoft.Maps.Map, microsoftMapsLocations: Microsoft.Maps.Location[], zoom: number): void {
    "use strict";

    var options: any = bingMap.getOptions();
    options.zoom = zoom || 10;
    if (microsoftMapsLocations.length === 1) {
        options.center = microsoftMapsLocations[0];
    } else if(microsoftMapsLocations.length > 1) {
        options.bounds = Microsoft.Maps.LocationRect.fromLocations(microsoftMapsLocations);
        options.padding = 50;
    }
    bingMap.setView(options);
}
