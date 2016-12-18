(function(ko, $, modelData) {

    function SelectRequestsViewModel(requests, map) {
        var self = this;

        self._map = map;

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
                    pinClicked(pushpin, request, e);
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

        self.spacialMathReady = ko.observable(false);
        self.drawingToolsReady = ko.observable(false);
        self.isDrawing = ko.observable(false);
        self.canLasso = ko.computed(function() {
            return self.spacialMathReady() && self.drawingToolsReady() && !self.isDrawing();
        });

        var drawingTools = null;
        self.doLasso = function () {
            self.isDrawing(true);
            if (drawingTools === null) {
                drawingTools = new Microsoft.Maps.DrawingTools(self._map);
            }

            drawingTools.create(Microsoft.Maps.DrawingTools.ShapeType.polygon);
        };

        self.completeLasso = function() {
            if (drawingTools === null) {
                return;
            }

            drawingTools.finish(function(shape) {
                var bounds = Microsoft.Maps.SpatialMath.Geometry.bounds(shape);
                self.requests().forEach(function(request) {
                    if (request.pushpin === null) {
                        return;
                    }

                    var location = request.pushpin.getLocation();

                    // The bounds check is a slight optimization to quickly discard any points outside the
                    // bounding rect of the polygon before iterating through all of the verticies for each point.
                    if (bounds.contains(location) && pointInPolygon(shape, location)) {
                        request.selected(true);
                    }
                });

                self.isDrawing(false);
            });
        }

        self.cancelLasso = function () {
            if (drawingTools === null) {
                return;
            }

            drawingTools.finish(function() {
                self.isDrawing(false);
            });
        }

        function pinClicked(pin, request, e) {
            if (self.isDrawing()) {
                e.handled = false;
                return false;
            }

            request.selected(!request.selected());
            return true;
        }

        function setPushpinState(pushpin, selected) {
            if (selected) {
                pushpin.setOptions({ color: '#d58033' });
            } else {
                pushpin.setOptions({ color: '#337ab7' });
            }
        }

        // https://msdn.microsoft.com/en-us/library/cc451895.aspx
        function pointInPolygon(polygon, location) {
            var i;
            var points = polygon.getLocations();
            var j = points.length - 1;
            var lat = location.latitude;
            var lon = location.longitude;
            var inPoly = false;

            for (i = 0; i < points.length; i++) {
                if (points[i].longitude < lon && points[j].longitude >= lon || points[j].longitude < lon && points[i].longitude >= lon) {
                    if (points[i].latitude + (lon - points[i].longitude) / (points[j].longitude - points[i].longitude) * (points[j].latitude - points[i].latitude) < lat) {
                        inPoly = !inPoly;
                    }
                }
                j = i;
            }

            return inPoly;
        }
    }

    function initializeMap() {
        setBingMapHeight();
        var map = createBingMap("bingMap");
        setMapCenterAndZoom(map, getLocationsFromModelRequests());

        var viewModel = new SelectRequestsViewModel(modelData.requests, map);

        Microsoft.Maps.loadModule('Microsoft.Maps.SpatialMath', function() {
            viewModel.spacialMathReady(true);
        });
        Microsoft.Maps.loadModule('Microsoft.Maps.DrawingTools', function() {
            viewModel.drawingToolsReady(true);
        });

        ko.applyBindings(viewModel, document.getElementById("selectRequestsView"));
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

    // Called when the map is ready
    window.initializeMap = initializeMap;
})(ko, jQuery, modelData);
