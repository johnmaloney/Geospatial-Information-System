require([
    "esri/Map",
    "esri/views/MapView",

    "esri/layers/FeatureLayer",

    "dojo/domReady!"
],
    function (
        Map, MapView,
        FeatureLayer
    ) {

        var map = new Map({
            basemap: "hybrid"
        });

        var view = new MapView({
            container: "viewDiv",
            map: map,

            extent: { // autocasts as new Extent()
                xmin: -178.217598382,
                ymin: 18.921786345999976,
                xmax: -66.96927110500002,
                ymax: 71.40623554799998,
                spatialReference: 4326 
            }
        });

        /********************
         * Add feature layer
         ********************/

        // Carbon storage of trees in Warren Wilson College.
        var featureLayer = new FeatureLayer({
            url: "http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/3"
        });

        map.add(featureLayer);

    });