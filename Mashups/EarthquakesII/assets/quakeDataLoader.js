define("QuakeDataLoader", [
    'dojo/_base/declare',
    'dgrid/Grid',
    'dgrid/Selection',
    "dojo/on",
    "dojo/dom",
    "dojo/domReady!"
], function (declare, Grid, Selection, on, dom) {

    var self = this;

    self.CustomGrid = declare([Grid, Selection]);

    var attach = function (rootView) {
        self.rootView = rootView;
    };

    var loadData = function (features) {

        dojo.empty("grid");

        // Now, create an instance of our custom grid which
        // have the features we added!
        var grid = new self.CustomGrid({
            columns: {
                Name: 'Name',
                Magnitude: 'Magnitude'
            },
            // for Selection; only select a single row at a time
            selectionMode: 'single'
        }, 'grid');
        grid.renderArray(features);

        grid.on('dgrid-select', function (event) {

            // Report the item from the selected row to the console.
            //console.log('Row selected: ', event.rows[0].data);

            var result = event.rows[0].data;

            rootView.popup.open({
                features: [result.graphic],
                location: result.graphic.geometry //{ latitude: result.Latitude, longitude: result.Longitude }
            });
        });
    }

    return {
        attach : attach,
        load : loadData
    };
});