export module Shapes {
    export class Rectangle {
        area: number;
        constructor(
            public height: number,
            public width: number) {

            this.area = width * height;
        }

        getArea() {
            return this.area;
        }
    }
}