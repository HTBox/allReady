/// <reference path="../../tools/typings/tsd.d.ts" />
System.register([], function (exports_1) {
    var AllReady;
    return {
        setters: [],
        execute: function () {
            (function (AllReady) {
                (function (Controls) {
                    var Thermometer = (function () {
                        function Thermometer(container) {
                            var _this = this;
                            this.height = 200;
                            this.width = 60;
                            this.maxValue = parseInt(container.attr("data-max-value"));
                            this.currentValue = parseInt(container.attr("data-current-value"));
                            var svg = d3.select(container[0]).append("svg")
                                .attr("width", this.width * 3)
                                .attr("height", this.height * 1.50)
                                .attr("class", "thermometer");
                            var defs = svg.append("defs");
                            var filter = defs.append("filter")
                                .attr("id", "f1")
                                .attr("x", "0")
                                .attr("y", "0")
                                .attr("width", "200%")
                                .attr("height", "200%");
                            filter.append("feOffset")
                                .attr("result", "offOut")
                                .attr("in", "SourceGraphic")
                                .attr("dx", "10")
                                .attr("dy", "10");
                            filter.append("feColorMatrix")
                                .attr("result", "matrixOut")
                                .attr("in", "offOut")
                                .attr("type", "matrix")
                                .attr("values", "0.2 0 0 0 0 0 0.2 0 0 0 0 0 0.2 0 0 0 0 0 1 0");
                            filter.append("feGaussianBlur")
                                .attr("result", "blurOut")
                                .attr("in", "matrixOut")
                                .attr("stdDeviation", "10");
                            filter.append("feBlend")
                                .attr("in", "SourceGraphic")
                                .attr("in2", "blurOut")
                                .attr("mode", "normal");
                            svg.selectAll(".bulb-outline")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("circle")
                                .attr("class", "thermometer-bulb-outline")
                                .attr("cy", this.height + this.width / 3)
                                .attr("cx", (this.width / 4) * 3)
                                .attr("r", this.width / 3)
                                .style("fill-opacity", 0)
                                .style("stroke", "black");
                            svg.selectAll(".bar-shadow")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("rect")
                                .attr("height", this.height)
                                .attr("y", 0)
                                .attr("x", this.width / 2)
                                .attr("width", this.width / 2)
                                .attr("class", "thermometer-liquid")
                                .attr("filter", "url(#f1)");
                            svg.selectAll(".bar-shadow")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("rect")
                                .attr("height", this.height)
                                .attr("y", 0)
                                .attr("x", this.width / 2)
                                .attr("width", this.width / 2)
                                .attr("fill", "white")
                                .attr("filter", "url(#f1)");
                            svg.selectAll(".bar")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("rect")
                                .attr("height", 0)
                                .attr("y", this.height)
                                .attr("x", this.width / 2)
                                .attr("width", this.width / 2)
                                .attr("class", "thermometer-liquid")
                                .transition()
                                .duration(1750)
                                .attr("height", function (d) { return _this.height * (d.currentValue / d.maxValue); })
                                .attr("y", function (d) { return _this.height - (_this.height * (d.currentValue / d.maxValue)); });
                            svg.selectAll(".outline")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("rect")
                                .attr("height", this.height)
                                .attr("width", this.width / 2)
                                .attr("x", this.width / 2)
                                .style("fill-opacity", 0)
                                .style("stroke", "black");
                            svg.selectAll(".bulb")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("circle")
                                .attr("cy", this.height + this.width / 3)
                                .attr("cx", (this.width / 4) * 3)
                                .attr("r", this.width / 3)
                                .attr("filter", "url(#f1)")
                                .attr("class", "thermometer-liquid");
                            svg.selectAll(".bulb-stem")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("rect")
                                .attr("y", this.height - 2)
                                .attr("x", (this.width / 2))
                                .attr("width", (this.width / 2))
                                .attr("height", (this.width / 4))
                                .attr("class", "thermometer-liquid");
                            svg.selectAll(".value")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("text")
                                .attr("text-anchor", "start")
                                .attr("x", this.width + 15)
                                .attr("y", function (d) { return _this.height - (_this.height * (d.currentValue / d.maxValue)); })
                                .transition()
                                .delay(1750)
                                .text(function (d) { return (_this.currentValue + "/" + _this.maxValue); });
                            svg.selectAll(".left-edge")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("line")
                                .attr("class", "thermometer-left-edge")
                                .attr("x1", this.width / 2)
                                .attr("x2", this.width / 2)
                                .attr("y1", 0)
                                .attr("y2", this.height + 5);
                            svg.selectAll(".right-edge")
                                .data([{ maxValue: this.maxValue, currentValue: this.currentValue }])
                                .enter()
                                .append("line")
                                .attr("class", "thermometer-right-edge")
                                .attr("x1", this.width)
                                .attr("x2", this.width)
                                .attr("y1", 0)
                                .attr("y2", this.height + 5);
                            var yScale = d3.scale.linear().domain([0, this.maxValue]).range([0, this.height]);
                            var yAxis = d3.svg.axis().scale(yScale)
                                .orient("left")
                                .ticks(6)
                                .tickSize(12, 5);
                            svg.append("g")
                                .attr("class", "thermometer-ticks")
                                .attr("transform", "translate(" + this.width + ",0)")
                                .call(yAxis);
                        }
                        return Thermometer;
                    })();
                    Controls.Thermometer = Thermometer;
                })(Controls = AllReady.Controls || (AllReady.Controls = {}));
            })(AllReady = AllReady || (AllReady = {}));
            exports_1("AllReady", AllReady);
        }
    }
});