require([
    "esri/Map",
    "esri/views/MapView",
    "esri/layers/FeatureLayer",
    "esri/widgets/Search",
    "esri/widgets/Legend",
    "YearSelection",
    "dojo/on",
    "dojo/dom",
    "dojo/domReady!"
], function (Map, MapView, FeatureLayer, Search, Legend, YearSelection, on, dom) {

    var map = new Map({
        basemap: "dark-gray",
        ground: "world-elevation"
    });

    var view = new MapView({
        container: "viewDiv",
        map: map,
        zoom: 2
    });

    var popupInfo = {
        title: "Name: {Magnitude}",
        content: "Name: {Name} </br> Magnitude: {Magnitude}"
    }

    var layer = new FeatureLayer({
        url: "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Earthquakes/Since_1970/MapServer/0",
        popupTemplate: popupInfo
    });

    map.add(layer);

    var legend = new Legend({
        view: view,
        layerInfos: [
            {
                layer: layer,
                title: "Earthquakes"
            }]
    });

    view.ui.add(legend, "bottom-left");

    YearSelection.attach(view, map, layer);
});
