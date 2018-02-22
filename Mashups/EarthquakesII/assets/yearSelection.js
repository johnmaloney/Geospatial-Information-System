define("YearSelection", [
    "esri/tasks/support/Query",
    "esri/Graphic",
    "esri/layers/GraphicsLayer",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/widgets/Legend",
    "QuakeDataLoader",
    "dojo/on",
    "dojo/dom",
    "dojo/domReady!"
], function (Query, Graphic, GraphicsLayer, SimpleMarkerSymbol, Legend, QuakeDataLoader, on, dom) {

    var self = this;
    var query = "YYYYMMDD like " 

    var attach = function (rootView, rootMap, rootLayer) {

        QuakeDataLoader.attach(rootView);

        self.rootView = rootView;
        self.rootLayer = rootLayer;
        self.rootMap = rootMap;

        //self.rootLayer.then(queryingFeatures).then(displayingFeatures);
    };

    var quakesQuery = new Query({
        where: query,
        returnGeometry: true,
        outFields: ["Name, Magnitude"]
    });

    var queryingFeatures = function () {

        quakesQuery.where = query + "'" + yearsSlider.value + "%'";

        return self.rootLayer.queryFeatures(quakesQuery);
    };

    var displayingFeatures = function (results) {

        var dataFeatures = [];

        var quakeFeatures = results.features.map(function (graphic) {

            var magnitude = graphic.attributes.Magnitude;
            var name = graphic.attributes.Name;

            var geometry = graphic.geometry;

            dataFeatures.push({
                Name : name,
                Magnitude : magnitude,
                graphic : graphic
            });

            if (magnitude < 7) {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "#1a9641",
                    size: "10px"
                });
            }
            else if (magnitude >= 7 && magnitude <= 8) {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "#a6d96a",
                    size: "20px"
                });
            }
            else if (magnitude > 8 && magnitude < 9) {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "#fdae61",
                    size: "30px"
                });
            }
            else {
                graphic.symbol = new SimpleMarkerSymbol({
                    color: "#d7191c",
                    size: "50px"
                });
            }
            
            return graphic;
        });

        self.rootMap.removeAll();

        var resultsLayer = new GraphicsLayer();

        resultsLayer.addMany(quakeFeatures);

        self.rootMap.add(resultsLayer);
        
        QuakeDataLoader.load(dataFeatures);
    }

    var yearsSlider = dom.byId("years");

    on(yearsSlider, "input", function () {

        dom.byId("year-value").innerText = yearsSlider.value;

        self.rootLayer.then(queryingFeatures).then(displayingFeatures);
    });

    return {
        attach: attach
    };
});