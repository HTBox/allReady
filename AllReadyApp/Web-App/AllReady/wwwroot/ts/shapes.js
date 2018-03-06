System.register([], function (exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var Shapes;
    return {
        setters: [],
        execute: function () {
            (function (Shapes) {
                var Rectangle = /** @class */ (function () {
                    function Rectangle(height, width) {
                        this.height = height;
                        this.width = width;
                        this.area = width * height;
                    }
                    Rectangle.prototype.getArea = function () {
                        return this.area;
                    };
                    return Rectangle;
                }());
                Shapes.Rectangle = Rectangle;
            })(Shapes || (Shapes = {}));
            exports_1("Shapes", Shapes);
        }
    };
});
//# sourceMappingURL=shapes.js.map