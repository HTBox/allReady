var geoHelper = function (positionReceivedCallback) {
    console.log('geoHelper created');

    this.getMyLocation = function () {
        navigator.geolocation.getCurrentPosition(positionReceivedCallback);
    }
}

var BingMapKey = "ArclGe51CDYUCjW2tcjeuCSxXDg0z4_NFRCmVMpnpMY0qGUvPBazQIp9AjDgm2kv";

var renderBingMap = function (divIdForMap, positionCoordsList) {
    if (positionCoordsList) {
        var bingMap = createBingMap(divIdForMap);
        var microsoftMapsLocations = createMapLocationsAndAddPushPinsToMap(bingMap, positionCoordsList);
        setMapCenterAndZoom(bingMap, microsoftMapsLocations);
    }
}

var renderRequestsMap = function(divIdForMap, requestData) {
    if (requestData) {
        var bingMap = createBingMap(divIdForMap);
        addRequestPins(bingMap, requestData);
    }
}

var getGeoCoordinates = function (address1, address2, city, state, postalCode, country, callbackFunction) {
    var lookupAddress = "";
    lookupAddress = lookupAddress + (address1 != undefined || address1 != null ? " " + address1 : null);
    lookupAddress = lookupAddress + (address2 != undefined || address2 != null ? " " + address2 : null);
    lookupAddress = lookupAddress + (city != undefined || city != null ? " " + city : null);
    lookupAddress = lookupAddress + (state != undefined || state != null ? " " + state : null);
    lookupAddress = lookupAddress + (postalCode != undefined || postalCode != null ? " " + postalCode : null);
    lookupAddress = lookupAddress + (country != undefined || country != null ? " " + country : null);
    if (lookupAddress.trim() == "") {
        return;
    }
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
}

function createBingMap(divIdForMap) {
    return new Microsoft.Maps.Map(
        document.getElementById(divIdForMap), {
        credentials: BingMapKey
    });
}

function createMapLocationsAndAddPushPinsToMap(bingMap, positionCoordsList) {
    var microsoftMapsLocations = [];
    $.each(positionCoordsList, function (index, LocationData) {
        var microsoftMapsLocation = new Microsoft.Maps.Location(LocationData.latitude, LocationData.longitude);
        microsoftMapsLocations.push(microsoftMapsLocation);
        var pushpin = new Microsoft.Maps.Pushpin();
        pushpin.setLocation(microsoftMapsLocation);
        bingMap.entities.push(pushpin);
    });
    return microsoftMapsLocations;
}

function addRequestPins(bingMap, requestData) {
    var locations = [];
    $.each(requestData, function (index, data) {
        var location = new Microsoft.Maps.Location(data.lat, data.long);
        locations.push(location);
        var order = index + 1;
        var pin = new Microsoft.Maps.Pushpin(location, { title: data.name, color: data.color, text: order.toString() });
        bingMap.entities.push(pin);
    });
    var rect = Microsoft.Maps.LocationRect.fromLocations(locations);
    bingMap.setView({ bounds: rect, padding: 80 });
}

function setMapCenterAndZoom(bingMap, microsoftMapsLocations) {
    var options = bingMap.getOptions();
    options.zoom = 10;
    if (microsoftMapsLocations.length === 1) {
        options.center = microsoftMapsLocations[0];
    } else if(microsoftMapsLocations.length > 1) {
        options.bounds = Microsoft.Maps.LocationRect.fromLocations(microsoftMapsLocations);
        options.padding = 50;
    }
    bingMap.setView(options);
}
