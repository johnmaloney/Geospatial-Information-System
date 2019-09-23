

let getLayers = async () => {
    const options = {
        method: 'GET',
        mode:'cors',
        headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
        }
    };
    try {
        //const response = await fetch(`https://localhost:44302/api/layer`, options);
        const response = await fetch(`https://gis-processor.azurewebsites.net/api/layer`, options);

        const json = await response.json();
        // console.log(json)
        return json;
    } catch (err) {
        console.log('Error getting documents', err);
    }
};


let Map = {
    render: async () => {
        
        let view =  /*html*/`
            <div class="section" id="mapid" style="height:500px"></div>
                `;
        return view;
    }
    , after_render: async () => {

        // get the layers from the server //
        var layers = await getLayers();

        var map = L.map('mapid');
        map.setView({ lat: 40, lng: -104 }, 10); 

        var controlLayers = [];
        layers.forEach(layer => {

            //var url = layer.properties.find(function(element) {
            //    return element.name === "TileAccessTemplate";
            //});

            //var mapboxUrl = url.value;

            //var mapboxVectorTileOptions = {
            //    rendererFactory: L.canvas.tile,
            //    token: 'pk.eyJ1IjoiaXZhbnNhbmNoZXoiLCJhIjoiY2l6ZTJmd3FnMDA0dzMzbzFtaW10cXh2MSJ9.VsWCS9-EAX4_4W1K-nXnsA'
            //};

            //var mapboxPbfLayer = L.vectorGrid.protobuf(mapboxUrl, mapboxVectorTileOptions).addTo(map);

            //var tileInfo = {};
            //tileInfo[layer.name] = mapboxPbfLayer;

            //controlLayers.push(mapboxPbfLayer);

        });   

             

        var Esri_WorldImagery = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Tiles &copy; Esri &mdash; Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community'
        });

        Esri_WorldImagery.addTo(map);

        var baseLayers = {
            "World Streets": Esri_WorldImagery 
        };

        var overlays = {
            "Tile One" : controlLayers[0]
        };

        //L.control.layers(baseLayers, { collapsed: false }).addTo(map);  
    }

};

export default Map;