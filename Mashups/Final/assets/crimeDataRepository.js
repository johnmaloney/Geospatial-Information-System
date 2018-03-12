define("crimeDataRepository", 
    function () {

        var self = this;
        self.crimeData = [];
        self.crimeLayer;
        self.originalGeoData;

        var loadData = function (crimeMap) {

            // load the json data from the file system. //
            $.getJSON('./assets/features.json', function (response) {
                response.features.forEach(function (item, index) {
                        item.properties.geometry = item.geometry;
                        item.properties.id = item.id;
                        self.crimeData.push(item.properties);
                });

                dataLoader(self.crimeData);

                var geojsonMarkerOptions = {
                    radius: 8,
                    fillColor: "#ff7800",
                    color: "#000",
                    weight: 1,
                    opacity: 1,
                    fillOpacity: 0.8
                };

                self.crimeLayer = L.geoJSON(response, {
                    pointToLayer: function (feature, latlng) {
                        var age = feature.properties["Victim Age"];
                        var gender = feature.properties["Victim Sex"] == "M" ? "male" : "female";
                        var at = feature.properties["Premise Description"];
                        var weapon = feature.properties["Weapon Description"];

                        var message = "<b>The " + gender + " victim was " + age + " years old</b><br />"
                            + "<br />the murder occurred in/at " + at + " with " + weapon;

                        var circle = L.circleMarker(latlng, geojsonMarkerOptions);
                        circle.bindPopup(message);
                        return circle;
                    }
                });


                self.crimeLayer.addTo(crimeMap);

                self.originalGeoData = response;
            });
        };

        var dataLoader = function (crimeData) {
            $("#jsGrid").jsGrid({
                width: "100%",
                height: "400px",

                inserting: false,
                editing: false,
                sorting: true,
                paging: true,

                data: crimeData,
                rowClick: function (args) { 
                    self.crimeLayer.eachLayer(function(layer){
                        if (layer.feature.id === args.item.id){
                            layer.openPopup();                                
                        }
                    })
                    
                },
                fields: [
                    { name: "Victim Age", type: "text", width: 150 },
                    { name: "Premise Description", type: "text", width: 150 },
                    {
                        name: "Weapon Description", type: "text", width: 150,
                        itemTemplate: function (value) {
                            return value;
                        }
                    }
                ]
            });
        }; 

        var checkForContains = function (poly) {

            self.crimeData = [];

            var bounds = poly.getBounds();
            var topLat = bounds._northEast.lat;
            var topLng = bounds._northEast.lng;

            var bottomLat = bounds._southWest.lat;
            var bottomLng = bounds._southWest.lng;

            self.originalGeoData.features.forEach(function(item, index){

                var pointLng = item.geometry.coordinates[0];
                var pointLat = item.geometry.coordinates[1];

                if (pointLat < topLat && pointLng < topLng) {
                    if (pointLat > bottomLat && pointLng > bottomLng) {
                        item.properties.geometry = item.geometry;
                        item.properties.id = item.id;
                        self.crimeData.push(item.properties);
                    }
                }

                dataLoader(self.crimeData);
            });
                
        };
        
        return {
            loadData: loadData,
            checkForContains : checkForContains
        }
    }
);