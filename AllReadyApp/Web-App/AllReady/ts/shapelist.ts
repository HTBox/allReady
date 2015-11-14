import * as sss from "./shapes"

// This works!

export module ShapeUser {
    export class User {
        rects: sss.Shapes.Rectangle;
        constructor() {
            this.rects = new sss.Shapes.Rectangle(10, 4);
        }

        greet() {
            var x: sss.Shapes.Rectangle = new sss.Shapes.Rectangle(10, 4);
            return this.rects;
        }
    }
}
var rect = new sss.Shapes.Rectangle(10, 4);
document.getElementById("target").textContent = JSON.stringify(rect, undefined, 3);

