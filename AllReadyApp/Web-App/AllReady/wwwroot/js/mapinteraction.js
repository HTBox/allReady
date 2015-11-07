///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, model, elementId) {
    var bingMap;

    function initBingMap(divIdForMap) {
        var element = document.getElementById(divIdForMap);
        bingMap = new Microsoft.Maps.Map(element, { credentials: BingMapKey });
    };

    function drawLocation(locData) {
        var pushpin = new Microsoft.Maps.Pushpin(bingMap.getCenter(), null);
        pushpin.setLocation(new Microsoft.Maps.Location(locData.latitude,locData.longitude));

        bingMap.entities.push(pushpin);
        bingMap.setView({
            zoom: 10, 
            center: new Microsoft.Maps.Location(locData.latitude, locData.longitude)
        });
    }

    /*
     * 
     */

    var data = [
        {
            name: 'Chicago Midway International Airport',
            latitude: 41.78678,
            longitude: -87.75219,
        },
        {
            name: 'Miami International Airport',
            latitude: 25.7932,
            longitude: -80.2906,
        },
        {
            name: 'Los Angeles International Airport',
            latitude: 33.9425,
            longitude: -118.408,
        },
    ];

    $(document).ready(function () {
        initBingMap(elementId);
        data.forEach(drawLocation);

        var geo = new geoHelper(function (position) {
            // TODO: replace with your own functionality

            var myPosition = {
                name: 'my loc',
                latitude: position.coords.latitude,
                longitude: position.coords.longitude,
            };

            drawLocation(myPosition);
        });

        geo.getMyLocation();
    });

})(ko, $, modelCampaign, 'bingMap');
