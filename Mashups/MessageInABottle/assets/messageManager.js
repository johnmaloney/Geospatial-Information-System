define("MessageManager", [
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/layers/GraphicsLayer",
    "esri/Graphic",
    "esri/geometry/Point",
    "esri/tasks/support/FeatureSet",
    "esri/tasks/Geoprocessor",
    "esri/widgets/Spinner",
    "dojo/on",
    "dojo/dom",
    "dojo/dom-construct",
    "dojo/domReady!"
], function (PictureMarkerSymbol, SimpleLineSymbol, GraphicsLayer, Graphic, Point, FeatureSet, Geoprocessor, Spinner, on, dom, domConstruct) {

    var self = this;

    var attach = function (rootView, rootMap) {

        self.rootView = rootView;
        self.rootMap = rootMap;

        self.spinner = new Spinner({
            container: domConstruct.create("div"),
            view: self.rootView
        });

        self.messageTrackLayer = new GraphicsLayer();
        self.rootMap.add(messageTrackLayer);
    };

    var placeBottle = function (event) {

        self.spinner.show({
            location: self.rootMap.center
        });

        self.messageTrackLayer.removeAll();

        var geometry = new Point({
            longitude: event.mapPoint.longitude,
            latitude: event.mapPoint.latitude
        });
        
        var picSymbol = new PictureMarkerSymbol(
            'assets/sting.png',
            26, 26);

        var graphic = new Graphic(geometry, picSymbol);

        //map.graphics.add(graphic);
        self.messageTrackLayer.add(graphic);
                        
        var gp = new Geoprocessor({
            url: "https://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_Currents_World/GPServer/MessageInABottle",
            outSpatialReference: {
                wkid: 102100
            }
        });

        var featureSet = new FeatureSet();
        featureSet.features = [graphic];

        var params = {
            "Input_Point": featureSet,
            "Days": dojo.byId("days").value
        };

        gp.execute(params).then(displayTrack);
    };

    function displayTrack(results, messages) {

        var simpleLineSymbol = new SimpleLineSymbol(
            SimpleLineSymbol.STYLE_SOLID,
            new dojo.Color([255, 255, 0]), 3
        );

        var features = results.results[0].value.features;

        dojo.forEach(features, function (feature) {
            feature.symbol = simpleLineSymbol;
        });

        self.messageTrackLayer.addMany(features);

        self.spinner.hide();
    }

    var daySlider = dom.byId("days");

    on(daySlider, "input", function () {

        dom.byId("day-value").innerText = daySlider.value;
    });

    return {
        attach: attach, 
        placeBottle : placeBottle
    };
});