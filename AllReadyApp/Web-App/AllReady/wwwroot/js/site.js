var geoHelper = function (positionReceivedCallback) {
    console.log('geoHelper created');

    this.getMyLocation = function () {
        navigator.geolocation.getCurrentPosition(positionReceivedCallback);
    }
}

var BingMapKey = "ArclGe51CDYUCjW2tcjeuCSxXDg0z4_NFRCmVMpnpMY0qGUvPBazQIp9AjDgm2kv";

var renderBingMap = function (divIdForMap, positionCoordsList) {
    var bingMap = new Microsoft.Maps.Map(
                    document.getElementById(divIdForMap),
                    {
                        credentials: BingMapKey
                    });
    if (positionCoordsList != undefined && positionCoordsList != null) {
        $.each(positionCoordsList, function (index, LocationData) {
            var pushpin = new Microsoft.Maps.Pushpin(bingMap.getCenter(), null);
            pushpin.setLocation(new Microsoft.Maps.Location(
                LocationData.latitude,
                LocationData.longitude));
            bingMap.entities.push(pushpin);
            bingMap.setView({
                zoom: 10, center: new Microsoft.Maps.Location(LocationData.latitude, LocationData.longitude)
            });
        });
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
