var radius = Math.min(width, height) / 1.5;

var x = d3.scale.linear()
    .range([0, 2 * Math.PI]);

var y = d3.scale.linear()
    .range([0, radius]);

var color = d3.scale.category10();

var svg = d3.select("#sunburst")
    .append("svg")
    .attr("width", width)
    .attr("height", height)
    .append("g")
    .attr("transform", "translate(" + width / 2 + "," + (height / 2 + 10) + ")");

var partition = d3.layout.partition()
    .value(function (d) { return d.data; });

var arc = d3.svg.arc()
    .startAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x))); })
    .endAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x + d.dx))); })
    .innerRadius(function (d) { return Math.max(0, y(d.y)); })
    .outerRadius(function (d) { return Math.max(0, y(d.y + d.dy)); });

var totalSize;

d3.json("flare.json", function (root) {

    totalSize = root.data;

    var g = svg.selectAll("g")
        .data(partition.nodes(root))
        .enter().append("g");

    var path = g.append("path")
        .attr("d", arc)
        .style("fill", function(d) { return color((d.children ? d : d.parent).name); })
        .on("click", function(d) {
            click2(d);
            click3(d);
            //mouseleave(d);
            path
                .transition()
                .duration(1000)
                .attrTween("d", arcTween(d));
        });
        //.on("mouseover", mouseover);;


    var units = [1000, 'k', 'M'];
    //Make 1k out of 1.000, 1M out of 1.000.000
    function unitify(n) {
        for (var i = units.length; i-- > 1;) {
            var unit = Math.pow(units[0], i);
            if (n >= unit)
                return Math.floor(n / unit) + units[i];
        }
        return n;
    }

    g.append("svg:title")
        .text(function (d) { return d.name + " " + unitify(d.data); });

    function getPath(d) {
        if (!d.parent) return "";
        return getPath(d.parent) + ";" + d.name;
    }

    click2(root);
    click3(root);
});

//d3.select(self.frameElement).style("height", height + "px");

// Interpolate the scales
function arcTween(d) {
    var xd = d3.interpolate(x.domain(), [d.x, d.x + d.dx]),
        yd = d3.interpolate(y.domain(), [d.y, 1]),
        yr = d3.interpolate(y.range(), [d.y ? 20 : 0, radius]);
    return function (dd, i) {
        return i
            ? function () { return arc(dd); }
            : function (t) {
                x.domain(xd(t));
                y.domain(yd(t)).range(yr(t));
                return arc(dd);
            };
    };
}