(function(ko, $, modelData) {

    function SelectRequestsViewModel(requests, map) {
        var self = this;

        self._map = map;

        self._pinClicked = function(pin, request, e) {
            request.selected(!request.selected());
        };

        var requestsWithPins = requests.map(function (request) {
            var pushpin = null;
            request.selected = ko.observable(request.IsSelected);
            if (requestHasValidCoordinates(request)) {
                var location = new Microsoft.Maps.Location(request.Latitude, request.Longitude);
                pushpin = new Microsoft.Maps.Pushpin(location, {
                    title: request.Name,
                    subtitle: request.Address
                });
                setPushpinState(pushpin, request.IsSelected);
                Microsoft.Maps.Events.addHandler(pushpin, "click", function(e) {
                    self._pinClicked(pushpin, request, e);
                });
                map.entities.push(pushpin);
                request.selected.subscribe(function(newValue) {
                    setPushpinState(pushpin, newValue);
                });
            }
            request.pushpin = pushpin;
            return request;
        });

        self.requests = ko.observableArray(requestsWithPins);

        function setPushpinState(pushpin, selected) {
            if (selected) {
                pushpin.setOptions({ color: '#d58033' });
            } else {
                pushpin.setOptions({ color: '#337ab7' });
            }
        }
    }

    function initializeMap() {
        setBingMapHeight();
        var map = createBingMap("bingMap");
        setMapCenterAndZoom(map, getLocationsFromModelRequests());

        var viewModel = new SelectRequestsViewModel(modelData.requests, map);
        ko.applyBindings(viewModel, document.getElementById("selectRequestsList"));
    }

    function setBingMapHeight() {
        $("#bingMap").height(($(window).height() * 0.6));
    }

    function requestHasValidCoordinates(request) {
        return request.Latitude !== 0 && request.Longitude !== 0;
    }

    function getLocationsFromModelRequests() {
        return modelData.requests.filter(requestHasValidCoordinates).map(function(request) {
            return { latitude: request.Latitude, longitude: request.Longitude };
        });
    }

    window.initializeMap = initializeMap;
})(ko, jQuery, modelData);
