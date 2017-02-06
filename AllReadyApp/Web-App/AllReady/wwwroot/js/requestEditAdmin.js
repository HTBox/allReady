(function ($) {
    var map = null,
        pushpin = null;

    function setLocation(latitude, longitude) {
        var location = new Microsoft.Maps.Location(latitude, longitude);

        if (latitude === 0 && longitude === 0) {
            $("#Latitude").val("");
            $("#Longitude").val("");
            if (pushpin) {
                map.entities.remove(pushpin);
                pushpin = null;
                $("#dragMessage").hide();
            }

            return null;
        }

        if (!pushpin) {
            pushpin = new Microsoft.Maps.Pushpin(location, { color: '#d58033', draggable: true });
            Microsoft.Maps.Events.addHandler(pushpin, "dragend", function (e) {
                setLocation(e.location.latitude, e.location.longitude);
            });

            map.entities.push(pushpin);
            $("#dragMessage").show();
        }

        pushpin.setLocation(location);
        $("#Latitude").val(latitude);
        $("#Longitude").val(longitude);
        return location;
    }

    function initializeMap() {
        var lat = +$("#Latitude").val() || 0,
            lon = +$("#Longitude").val() || 0,
            address = getAddress();

        setBingMapHeight();
        map = createBingMap("bingMap");

        if (lat !== 0 || lon !== 0) {
            setLocation(lat, lon);
            setMapCenterAndZoom(map, [new Microsoft.Maps.Location(lat, lon)], 15);
        } else if (!isAddressEmpty(address)) {
            lookupAddress(address);
        } else {
            setMapCenterAndZoom(map, []);
        }

        $("#Address, #City, #State, #PostalCode").on("change", function () {
            var address = getAddress();
            if (!isAddressEmpty(address)) {
                lookupAddress(address);
            }
        });
    }

    function lookupAddress(address) {
        getGeoCoordinates(address.address1, address.address2, address.city, address.state, address
            .postalCode, address.country, function(l) {
                var location = setLocation(l.latitude, l.longitude);
                if (location !== null) {
                    setMapCenterAndZoom(map, [location], 15);
                }
            });
    }

    function getAddress() {
        return {
            address1: $("#Address").val(),
            city: $("#City").val(),
            state: $("#State").val(),
            postalCode: $("#PostalCode").val()
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
