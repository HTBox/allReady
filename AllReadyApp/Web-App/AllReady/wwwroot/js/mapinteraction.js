///<reference path="../lib/jquery/dist/jquery.js" />
///<reference path="../lib/knockout/dist/knockout.js" />

(function (ko, $, model, elementId) {
    var bingMap;
    var pinInfobox;

    function initBingMap(divIdForMap) {
        var element = document.getElementById(divIdForMap);
        bingMap = new Microsoft.Maps.Map(element, { credentials: BingMapKey });

        // Create the info box for the pushpin
        pinInfobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(0, 0), { title: 'My Pushpin', visible: true });

        function displayInfobox(e) {
            pinInfobox.setOptions({ visible: true });
        }


        function hideInfobox(e) {
            pinInfobox.setOptions({ visible: false });
        }

        // Retrieve the location of the map center 
        var center = bingMap.getCenter();

        // Add a pin to the center of the map
        var pin = new Microsoft.Maps.Pushpin(center, { text: '1' });


        // Add a handler for the pushpin click event.
        Microsoft.Maps.Events.addHandler(pin, 'click', displayInfobox);

        // Hide the info box when the map is moved.
        Microsoft.Maps.Events.addHandler(bingMap, 'viewchange', hideInfobox);




        //Add handler for the map click event.
        //Microsoft.Maps.Events.addHandler(bingMap, 'click', displayLatLong);

        function displayLatLong(e) {
            if (e.targetType == "map") {
                var point = new Microsoft.Maps.Point(e.getX(), e.getY());
                var loc = e.target.tryPixelToLocation(point);
                //document.getElementById("textBox").value = loc.latitude + ", " + loc.longitude;
            }
        }
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

    //http://dev.openlayers.org/examples/geojson-reprojected.js
    //var hybrid = new OpenLayers.Layer.Bing({
    //    key: apiKey,
    //    type: "AerialWithLabels",
    //    name: "Bing Aerial With Labels"
    //});

    //var vector = new OpenLayers.Layer.Vector("GeoJSON", {
    //    projection: "EPSG:4326",
    //    strategies: [new OpenLayers.Strategy.Fixed()],
    //    protocol: new OpenLayers.Protocol.HTTP({
    //        url: "geojson-reprojected.json",
    //        format: new OpenLayers.Format.GeoJSON()
    //    })
    //});

    //var center = new OpenLayers.LonLat(-109.6, 46.7).transform("EPSG:4326", "EPSG:900913");

    //var map = new OpenLayers.Map({
    //    div: "map",
    //    layers: [hybrid, vector],
    //    center: center,
    //    zoom: 4
    //});

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
