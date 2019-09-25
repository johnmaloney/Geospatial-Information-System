

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
        
        var Esri_WorldImagery = L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Tiles &copy; Esri &mdash; Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community'
        });

        Esri_WorldImagery.addTo(map);

        var baseLayers = {
            "World Streets": Esri_WorldImagery 
        };
        
        //L.control.layers(baseLayers, { collapsed: false }).addTo(map);  
    }

};

export default Map;