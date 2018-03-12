/// calling leaflet here loads the file which in turn assigns the global "L" variable 
define(["crimeDataRepository"],
    function (crimeDataRepository) {

        var crimeMap = L.map('mapid').setView([33.99, -118.27], 10);

        L.tileLayer('https://api.tiles.mapbox.com/v4/{id}/{z}/{x}/{y}.png?access_token=pk.eyJ1IjoibWFsb25leTEiLCJhIjoieHNjcmRxRSJ9.iiwjqAeZG4XVz4IoZBoECA', {
                attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery � <a href="http://mapbox.com">Mapbox</a>',
                maxZoom: 18,
                id: 'mapbox.satellite',
                accessToken: 'pk.eyJ1IjoibWFsb25leTEiLCJhIjoieHNjcmRxRSJ9.iiwjqAeZG4XVz4IoZBoECA'
            })
            .addTo(crimeMap);

        crimeDataRepository.loadData(crimeMap);
        // FeatureGroup is to store editable layers
        var drawnItems = new L.FeatureGroup().addTo(crimeMap);

        var drawControl = new L.Control.Draw({
            draw: {
                polygon: false,
                marker: false
            },
            edit: {
                featureGroup: drawnItems,
                edit: false
            }
        });

        crimeMap.addControl(drawControl);

        crimeMap.on(L.Draw.Event.CREATED, function (event) {

            drawnItems.clearLayers();

            var layer = event.layer;
            
            drawnItems.addLayer(layer);

            crimeDataRepository.checkForContains(layer);
        });

        var popup = L.popup();

        function onMapClick(e) {
            popup
                .setLatLng(e.latlng)
                .setContent("You clicked the map at " + e.latlng.toString())
                .openOn(crimeMap);
        }

        crimeMap.on('click', onMapClick);
    }
);


