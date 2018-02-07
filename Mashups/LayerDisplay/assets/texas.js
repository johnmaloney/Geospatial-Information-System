require([
    "esri/Map",
    "esri/views/SceneView",
    "esri/layers/SceneLayer",
    "esri/layers/VectorTileLayer",
    "dojo/dom",
    "dojo/domReady!"
], function (Map, SceneView, SceneLayer, VectorTileLayer, dom) {
    var sceneLayer;

    // Create the Map
    var map = new Map({
        basemap: "streets",
        ground: "world-elevation"
    });

    // Create the SceneView
    var view = new SceneView({
        container: "viewDiv",
        map: map,
        zoom: 5,
        camera: {
            position: [-95.370478,29.760878, 4326],
            tilt: 50
        }
    });
    
    /********************************************************************
       * Add a vector tile layer to the map
       *
       * The url must point to the style or the vector tile service
       *********************************************************************/
    var tileLyr = new VectorTileLayer({
        url: "https://tiles.arcgis.com/tiles/KTcxiTD9dsQw4r7Z/arcgis/rest/services/TxDOT_Vector_Tile_Basemap/VectorTileServer"
    });

    map.add(tileLyr);
    
    view.when(function () {
        // when the scene and view resolve, display the scene's
        // title in the DOM
        var title = scene.portalItem.title;
        titleDiv.innerHTML = title;
    });
});