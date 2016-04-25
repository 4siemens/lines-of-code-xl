var fill = d3.scale.category20();

var layout = d3.layout.cloud()
    .size([width, height]);


d3.select("#wcloud")
    .append("svg")
    .attr("width", width)
    .attr("height", height);

//d3.select("#wcloud").on("mouseleave", mouseleave);

var fontSize;

function draw(words) {

    d3.select("#wcloud")
        .select("svg")
        .select("g")
        .remove();
            

    d3.select("#wcloud")
        .select("svg")
        .append("g")
        .attr("transform", "translate(" + layout.size()[0] / 2 + "," + layout.size()[1] / 2 + ")")
        .selectAll("text")
        .data(words)
        .enter().append("text")
        .style("font-size", function(d) {
            return d.size + "px";
        })
        .style("font-family", "Impact")
        .style("fill", function(d, i) {
            return fill(i);
        })
        .attr("text-anchor", "middle")
        .attr("transform", function(d) {
            return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
        })
        .text(function (d) { return d.text; });
}

var fontSize;

function getPath(d) {
    if (!d.parent) return "";
    return getPath(d.parent) + ";" + d.name;
}

function click2(d) {


    d3.json("api/words/" + getPath(d), function (words) {

        fontSize = d3.scale.linear()
         .domain([words[words.length - 1].size, words[0].size])
         .rangeRound([10, 200]);

        layout
            .words(words)
            .padding(5)
            .rotate(function() { return 0; })
            .font("Impact")
            .fontSize(function (dd) { return fontSize(dd.size); })
            .on("end", draw);

        layout.start();
    });
};