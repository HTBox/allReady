System.register(["./shapes"], function (exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var sss, ShapeUser, rect;
    return {
        setters: [
            function (sss_1) {
                sss = sss_1;
            }
        ],
        execute: function () {
            // This works!
            (function (ShapeUser) {
                var User = /** @class */ (function () {
                    function User() {
                        this.rects = new sss.Shapes.Rectangle(10, 4);
                    }
                    User.prototype.greet = function () {
                        var x = new sss.Shapes.Rectangle(10, 4);
                        return this.rects;
                    };
                    return User;
                }());
                ShapeUser.User = User;
            })(ShapeUser || (ShapeUser = {}));
            exports_1("ShapeUser", ShapeUser);
            rect = new sss.Shapes.Rectangle(10, 4);
        }
    };
});
//# sourceMappingURL=shapelist.js.map