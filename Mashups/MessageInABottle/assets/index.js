require([
    "esri/Map",
    "esri/views/SceneView",
    "esri/layers/SceneLayer",
    "esri/layers/VectorTileLayer",
    "esri/symbols/PictureMarkerSymbol",
    "esri/Graphic",
    "esri/tasks/Geoprocessor",
    "MessageManager",
    "dojo/dom",
    "dojo/domReady!"
], function (Map, SceneView, SceneLayer, VectorTileLayer, PictureMarkerSymbol, Graphic, Geoprocessor, MessageManager, dom) {
    var sceneLayer;

    // Create the Map
    var map = new Map({
        basemap: "hybrid"
    });

    // Create the SceneView
    var view = new SceneView({
        container: "viewDiv",
        map: map
    });

    view
        .then(function () {
            // SceneView is now ready for display and can be used. Here we will
            // use goTo to view a particular location at a given zoom level, camera
            // heading and tilt.
            view.goTo({
                center: [-75.61, 35.95],
                zoom: 10,
                heading: 30,
                tilt: 20
            })
        })
        .otherwise(function (err) {
            // A rejected view indicates a fatal error making it unable to display,
            // this usually means that WebGL is not available, or too old.
            console.error("SceneView rejected:", err);
        });

    MessageManager.attach(view, map);

    view.on("click", MessageManager.placeBottle);

});