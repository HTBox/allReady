System.register([], function(exports_1) {
    var Shapes;
    return {
        setters:[],
        execute: function() {
            (function (Shapes) {
                var Rectangle = (function () {
                    function Rectangle(height, width) {
                        this.height = height;
                        this.width = width;
                        this.area = width * height;
                    }
                    Rectangle.prototype.getArea = function () {
                        return this.area;
                    };
                    return Rectangle;
                })();
                Shapes.Rectangle = Rectangle;
            })(Shapes = Shapes || (Shapes = {}));
            exports_1("Shapes", Shapes);
        }
    }
});
//# sourceMappingURL=shapes.js.map