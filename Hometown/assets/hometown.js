require([
    "esri/Map",
    "esri/views/MapView",
    "esri/geometry/Point",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/layers/GraphicsLayer",
    "esri/Graphic",
    "esri/PopupTemplate",
    "dojo/domReady!"
], function (Map, MapView, Point, SimpleMarkerSymbol, GraphicsLayer, Graphic, PopupTemplate) {

    var latitude = 45.789;
    var longitude = -108.500;

    var point = new Point({
        latitude: latitude,
        longitude: longitude
    });

    var symbol = new SimpleMarkerSymbol({
        color: "green",
        style: "square",
        size: 12
    });

    var simpleMap = new Map({
        basemap: "streets"
    });    

    var simpleView = new MapView({
        container: "simpleHometown",
        map: simpleMap,
        zoom: 12,
        center: [longitude, latitude] // longitude, latitude
    });

    var simpleGraphic = new Graphic({
        geometry: point,
        symbol: symbol
    });

    var simpleLayer = new GraphicsLayer({
        graphics: [simpleGraphic]
    });

    simpleMap.add(simpleLayer);

    var complexMap = new Map({
        basemap: "streets"
    });  

    var complexView = new MapView({
        container: "complexHometown",
        map: complexMap,
        zoom: 12,
        center: [longitude, latitude] // longitude, latitude
    });
     
    var template = { // autocasts as new PopupTemplate()
        title: "City of Billings",
        content: "<p>Located in {state}</p>"
        + "<p>The population is {population}</p>"
        + "<p>The city was founded in {foundedYear} by {foundersName}.</p>"
        + "<p>The most odd law on the books that it is {oddityLegal}.</p>"
    };

    var complexGraphic = new Graphic({
        geometry: point,
        symbol: symbol,
        attributes: {
            "state": "Montana",
            "population": "110,323 [USCB, 2016]", 
            "oddityLegal": "illegal to keep a rat as a pet", 
            "foundedYear": "March 1882", 
            "foundersName" : "Frederick H. Billings, President of the Northern Pacific Railroad"
        }
    });

    var complexLayer = new GraphicsLayer({
        graphics: [complexGraphic],
        popupTemplate: template
    });

    complexMap.add(complexLayer);
});