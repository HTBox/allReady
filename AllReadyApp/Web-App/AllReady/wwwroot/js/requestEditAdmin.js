(function ($) {
    var map = null,
        pushpin = null;

    function setLocation(latitude, longitude) {
        var location = new Microsoft.Maps.Location(latitude, longitude);

        if (!pushpin) {
            pushpin = new Microsoft.Maps.Pushpin(location);
            pushpin.setOptions({ color: '#d58033' });
            map.entities.push(pushpin);
        }

        pushpin.setLocation(location);
        $("#Latitude").val(latitude);
        $("#Longitude").val(longitude);
    }

    function initializeMap() {
        var lat = +$("#Latitude").val() || 0,
            lon = +$("#Longitude").val() || 0,
            address = getAddress();

        setBingMapHeight();
        map = createBingMap("bingMap");

        Microsoft.Maps.Events.addHandler(map, 'click', function(e) {
            setLocation(e.location.latitude, e.location.longitude);
        });

        if (lat !== 0 || lon !== 0) {
            setLocation(lat, lon);
            setMapCenterAndZoom(map, [new Microsoft.Maps.Location(lat, lon)], 15);
        } else if (!isAddressEmpty(address)) {
            lookupAddress(address);
        } else {
            setMapCenterAndZoom(map, []);
        }

        $("#Address, #City, #State, #Zip").on("change", function () {
            var address = getAddress();
            if (!isAddressEmpty(address)) {
                lookupAddress(address);
            }
        });
    }

    function lookupAddress(address) {
        getGeoCoordinates(address.address1, address.address2, address.city, address.state, address
            .postalCode, address.country, function(l) {
                setMapCenterAndZoom(map, [l], 15);
                setLocation(l.latitude, l.longitude);
            });
    }

    function getAddress() {
        return {
            address1: $("#Address").val(),
            city: $("#City").val(),
            state: $("#State").val(),
            postalCode: $("#Zip").val()
        };
    }

    function isAddressEmpty(address) {
        if (!address) return true;
        return !(address.address1 && (address.city || address.postalCode));
    }

    function setBingMapHeight() {
        $("#bingMap").height(($(window).height() * 0.6));
    }

    // Called when the map is ready
    window.initializeMap = initializeMap;
})(jQuery);
