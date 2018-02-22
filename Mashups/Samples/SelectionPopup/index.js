require([
    "esri/Map",
    "esri/views/MapView",
    "esri/layers/FeatureLayer",
    "esri/Graphic",
    "esri/symbols/SimpleFillSymbol",
    "esri/layers/GraphicsLayer",
    "esri/tasks/support/Query",
    "dojo/domReady!"
], function (Map, MapView, FeatureLayer, Graphic, SimpleFillSymbol, GraphicsLayer, Query) {

    var whereClause = "NO_FARMS87 > 500";

    var map = new Map({
        basemap: "dark-gray"
    });

    var view = new MapView({
        container: "viewDiv",
        map: map,
        zoom: 7,
        center: [-78, 41]
    });

    var listNode = document.getElementById("list_counties");

    var popupInfo = {
        title: "{NAME} County",
        content: "<b> Farms: {NO_FARMS87} </b>"
    }

    var counties = new FeatureLayer({
        portalItem: {
            id: "0875e77a2ff54dd689e169d7798d0905"
        },
        popupTemplate: popupInfo
    });

    var resultsLayer = new GraphicsLayer();

    map.addMany([counties, resultsLayer]);

    var farmQuery = new Query({
        where: whereClause,
        returnGeometry: true,
        outFields: ["NAME", "NO_FARMS87"]
    });

    counties.then(function () {
        return counties.queryFeatures(farmQuery);
    }).then(displayResults);

    var graphics = [];

    function displayResults(results) {
        var fragment = document.createDocumentFragment();

        results.features.forEach(function (county, index) {
            county.symbol = new SimpleFillSymbol({
                color: "yellow"
            });

            graphics.push(county);

            var attributes = county.attributes;
            var name = attributes.NAME + " (" +
                attributes.NO_FARMS87 + ")";

            var li = document.createElement("li");
            li.classList.add("panel-result");
            li.tabIndex = 0;
            li.setAttribute("data-result-id", index);
            li.textContent = name;

            fragment.appendChild(li);
        });

        listNode.innerHTML = "";
        listNode.appendChild(fragment);

        resultsLayer.addMany(graphics);
    }

    listNode.addEventListener("click", onListClickHandler);

    function onListClickHandler(event) {
        var target = event.target;
        var resultId = target.getAttribute("data-result-id");

        var result = resultId && graphics && graphics[parseInt(resultId,
            10)];

        if (result) {
            view.popup.open({
                features: [result],
                location: result.geometry.centroid
            });
        }
    }
});