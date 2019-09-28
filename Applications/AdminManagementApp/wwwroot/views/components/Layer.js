
let layers = [];

let loadLayer = async () => {

    var selector = document.getElementById("layerSelect");
    var layerName = selector.options[selector.selectedIndex].value;

    var selectedLayer = layers.find(function (element) {
        return element.identifier === layerName;
    });
    var url = selectedLayer.properties.find(function(element) {
        return element.name === "tile_access_template";
    });

    var mapboxUrl = url.value;

    var mapboxVectorTileOptions = {
        rendererFactory: L.canvas.tile,
        token: 'pk.eyJ1IjoiaXZhbnNhbmNoZXoiLCJhIjoiY2l6ZTJmd3FnMDA0dzMzbzFtaW10cXh2MSJ9.VsWCS9-EAX4_4W1K-nXnsA'
    };

    var map = mapsPlaceholder[0];

    map.eachLayer(function (l) {

        if (l._url !== "https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}") {

            map.removeLayer(l);
        }

    });

    L.vectorGrid.protobuf(mapboxUrl, mapboxVectorTileOptions).addTo(map);
};

let getLayers = async () => {
    const options = {
        method: 'GET',
        mode: 'cors',
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


let Layer = {
    render: async () => {

        // get the layers from the server //
        layers = await getLayers();

        if (layers) {
            let view =  /*html*/`
            <form id ="layersForm">
                <div class="field">
                    <label class="label">Available Layers</label>
                    <div class="control">
                        <div class="select">
                            <select id="layerSelect">
                            ${ layers.map(l =>
                `<option value=${l.identifier}>${l.name}</option>`
            )
                }
                            </select>
                        </div>
                    </div>
                </div>

                <div class="field is-grouped">
                    <div class="control">
                        <button class="button is-link is-primary">Load Layer</button>
                    </div>
                    <div class="control">                        
                        <a class="is-link" id="refreshLayers">Refresh Layers</a>
                    </div>
                </div>
            </form>`;
            return view;
        }
        else {
            return "<label>Invalid layers retrieval...</label>";
        }
        
    }
    , after_render: async () => {

        var form = document.getElementById("layersForm");
        if (form) {
            form.addEventListener("submit", function (event) {
                event.preventDefault();
                loadLayer();
            });
        }
        var refresh = document.getElementById("refreshLayers");
        if (refresh) {

            refresh.addEventListener("click", async function (event) {
                var newLayers = await getLayers();
                var select = document.getElementById("layerSelect");
                var i;
                for (i = select.options.length - 1; i >= 0; i--) {
                    select.remove(i);
                }

                newLayers.map(l => {
                    var opt = document.createElement('option');
                    opt.value = l.identifier;
                    opt.innerHTML = l.name;
                    select.appendChild(opt);
                });
            });
            
        }
    }

};

export default Layer;