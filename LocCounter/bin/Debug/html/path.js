function getAncestors(node) {
    var path = [];
    var current = node;
    while (current.parent) {
        path.unshift(current);
        current = current.parent;
    }
    return path;
}

// Fade all but the current sequence, and show it in the breadcrumb trail.
function click3(d) {

    var percentage = (100 * d.value / totalSize).toPrecision(4);
    var percentageString = percentage + "%";
    if (percentage < 0.01) {
        percentageString = "< 0.01%";
    }

    var sequenceArray = getAncestors(d);
    d3.select("#path").text(sequenceArray.map(function(d) { return d.name }).join(" › "));
    d3.select("#percent").text(percentageString);

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

    d3.select("#size").text(unitify(d.data));
}

function mouseover(d) {

    var sequenceArray = getAncestors(d);
    if (!sequenceArray) return;
    if (sequenceArray.length === 0) return;
    //Fade all the segments.
    d3.selectAll("path")
        .style("opacity", 0.3);

    // Then highlight only those that are an ancestor of the current segment.
    svg.selectAll("path")
        .filter(function (node) {
            return (sequenceArray.indexOf(node) >= 0);
        })
        .style("opacity", 1);
}

// Restore everything to full opacity when moving off the visualization.
function mouseleave() {

    // Deactivate all segments during transition.
    d3.selectAll("path").on("mouseover", null);

    // Transition each segment to full opacity and then reactivate it.
    d3.selectAll("path")
        .transition()
        .duration(500)
        .style("opacity", 1)
        .each("end", function () {
            d3.select(this).on("mouseover", mouseover);
        });
}
