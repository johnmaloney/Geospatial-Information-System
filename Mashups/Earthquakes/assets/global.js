require([
    "esri/Map",
    "esri/views/SceneView",
    "esri/Camera",
    "esri/layers/FeatureLayer",
    "esri/renderers/ClassBreaksRenderer",
    "esri/symbols/PolygonSymbol3D",
    "esri/symbols/FillSymbol3DLayer",
    "esri/symbols/SimpleLineSymbol",
    "esri/widgets/Legend",
    "dojo/domReady!"
], function (Map, SceneView, Camera, FeatureLayer, ClassBreaksRenderer, PolygonSymbol3D, FillSymbol3DLayer, SimpleLineSymbol, Legend) {

    var map = new Map({
        basemap: "dark-gray",
        ground: "world-elevation"
    });

    var view = new SceneView({
        container: "viewDiv",
        map: map,
        camera: new Camera({
            position: [-74.5, 36.6, 229000],
            heading: 328,
            tilt: 64
        })
    });
    
    var tileLyr = new FeatureLayer({
        url: "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Earthquakes/Since_1970/MapServer/0"
    });

    map.add(tileLyr);

    //var countyRenderer = new ClassBreaksRenderer({
    //    field: "POP1990",
    //    normalizationField: "SQ_MILES",
    //    legendOptions: {
    //        title: "Pop per Sq Mile"
    //    }
    //});

    //var addClass = function (min, max, clr, lbl, renderer) {
    //    renderer.addClassBreakInfo({
    //        minValue: min,
    //        maxValue: max,
    //        symbol: new PolygonSymbol3D({
    //            symbolLayers: [new FillSymbol3DLayer({
    //                material: { color: clr },
    //                outline: new SimpleLineSymbol({
    //                    style: "dash"
    //                })
    //            })]
    //        }),
    //        label: lbl
    //    });
    //}

    //addClass(0, 50, "#eff3ff", "under 50", countyRenderer);
    //addClass(50, 150, "#bdd7e7", "50 - 150", countyRenderer);
    //addClass(150, 250, "#6baed6", "150 - 250", countyRenderer);
    //addClass(250, 500, "#3182bd", "250 - 500", countyRenderer);
    //addClass(500, 1000, "#08519c", "over 500", countyRenderer);

    //var countyLyr = new FeatureLayer({
    //    portalItem: {
    //        id: "0875e77a2ff54dd689e169d7798d0905"
    //    },
    //    renderer: countyRenderer
    //});

    //map.add(countyLyr)

    //var legend = new Legend({
    //    view: view,
    //    layerInfos: [{
    //        layer: countyLyr,
    //        title: "Jen & Barry's World"
    //    }]
    //});

    //view.ui.add(legend, "bottom-left");



    });
    