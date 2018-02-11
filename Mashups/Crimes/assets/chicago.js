require([
    "esri/Map",
    "esri/views/SceneView",
    "esri/Camera",
    "esri/layers/FeatureLayer",
    "esri/renderers/UniqueValueRenderer",
    "esri/symbols/PointSymbol3D",
    "esri/symbols/IconSymbol3DLayer",
    "esri/widgets/Legend",
    "ClassFactory",
    "dojo/domReady!"
], function (Map, SceneView, Camera, FeatureLayer, UniqueValueRenderer, PointSymbol3D, IconSymbol3DLayer, Legend, ClassFactory) {

    var map = new Map({
        basemap: "dark-gray",
        ground: "world-elevation"
    });

    var view = new SceneView({
        container: "viewDiv",
        map: map, 
        camera: new Camera({
            position: [-87.6, 41.8, 4326],
            heading: 328,
            tilt: 50
        })
    });

    var crimeRenderer = new UniqueValueRenderer({
        field: "Description"
    });
    
    ClassFactory.addRenderer(crimeRenderer);
    ClassFactory.addClass("CYCLE, SCOOTER, BIKE W-VIN");
    ClassFactory.addClass("CYCLE, SCOOTER, BIKE NO VIN");
    ClassFactory.addClass("TRUCK, BUS, MOTOR HOME");
    ClassFactory.addClass("AUTOMOBILE");

    var template = {
        title: "Motor Vehicle Theft Type : <br />{Description}",
        content: "Case # : {Case_Number}<br />"
        + "Address : {Block}<br />"
        + "Description : {Location_Description}<br />"
        + "Location : {Location}<br />"
        + "",
    };  


    var cityLyr = new FeatureLayer({
        url: "https://services1.arcgis.com/VA8qSG2eGMxXGKnw/arcgis/rest/services/motor_vehicle_crimes/FeatureServer"
        , renderer: crimeRenderer
        , outFields: ["*"]
        , popupTemplate: template
    });

    map.add(cityLyr)
    
});