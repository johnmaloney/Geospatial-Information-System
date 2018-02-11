require([
    "esri/Map",
    "esri/views/MapView",
    "esri/layers/FeatureLayer",
    "esri/layers/GraphicsLayer",
    "esri/Graphic",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/tasks/support/Query",
    "esri/widgets/Search",
    "dojo/on",
    "dojo/dom",
    "dojo/domReady!"
], function (Map, MapView, FeatureLayer, GraphicsLayer, Graphic, SimpleMarkerSymbol, Query, Search, on, dom) {

    var yearsSlider = dom.byId("years");
    var query = "YYYYMMDD like "   

    var map = new Map({
        basemap: "dark-gray",
        ground: "world-elevation"
    });

    var view = new MapView({
        container: "viewDiv",
        map: map,
        zoom : 3
    });

    view.ui.add("infoDiv", "top-right");
    
    var layer = new FeatureLayer({
        url: "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Earthquakes/Since_1970/MapServer/0"
    });
    
    map.add(layer);

    var quakesQuery = new Query({
        where: query,
        returnGeometry: true,
        outFields: ["Magnitude"]
    });

    var queryingFeatures = function () {

        quakesQuery.where = query + "'" + yearsSlider.value + "%'"; 

        return layer.queryFeatures(quakesQuery);
    };

    var displayingFeatures = function (results) {
        var quakeFeatures = results.features.map(function (graphic) {

            var magnitude = graphic.attributes.Magnitude;

            if (magnitude < 7) {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "green",
                    size: "10px"
                });
            }
            else if (magnitude >= 7 && magnitude <= 8) {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "yellow",
                    size: "20px"
                });
            }
            else if (magnitude > 8 && magnitude < 9) {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "orange",
                    size: "30px"
                });
            }
            else {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "red",
                    size: "50px"
                });
            }

            return graphic;
        });

        map.removeAll();

        var resultsLayer = new GraphicsLayer();

        resultsLayer.addMany(quakeFeatures);

        map.add(resultsLayer);
    }

    on(yearsSlider, "input", function () {

        dom.byId("year-value").innerText = yearsSlider.value;

        layer.then(queryingFeatures).then(displayingFeatures); 
    });
});
    