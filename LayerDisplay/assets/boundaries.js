
require([
    "esri/WebMap",
    "esri/views/MapView",
    "dojo/domReady!"
],
    function (
        WebMap, MapView
    ) {
        /************************************************************
        * Creates a new WebMap instance. A WebMap must reference
        * a PortalItem ID that represents a WebMap saved to
        * arcgis.com or an on-premise portal.
        ************************************************************/

        var webmap = new WebMap({
            portalItem: {
                id: "94d18a202c1443de99d6d49b4f4fcba8"
            }
        });

        /************************************************************
        * Set the WebMap instance to the map property in a MapView.
        ************************************************************/
        var view = new MapView({
            map: webmap,
            container: "viewDiv"
        });

    });