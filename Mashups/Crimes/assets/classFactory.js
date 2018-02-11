define("ClassFactory", [
    "esri/symbols/PointSymbol3D",
    "esri/symbols/IconSymbol3DLayer"
], function (PointSymbol3D, IconSymbol3DLayer) {

    var addRenderer = function (renderer) {
        this.renderer = renderer;
    };
        
    var addClass = function (dataValue) {
        var symbolForData;
        switch (dataValue) {
            case "AUTOMOBILE": 
                this.renderer.addUniqueValueInfo({
                    value: dataValue,
                    symbol: new PointSymbol3D({
                        symbolLayers: [new IconSymbol3DLayer({
                            material: { color: "red" },
                            resource: { primitive: "circle" },
                            size: 12
                        })]
                    }),
                    label: "Theft of Automobile"
                });
                break;
            case "CYCLE, SCOOTER, BIKE W-VIN":
            case "CYCLE, SCOOTER, BIKE NO VIN":
                this.renderer.addUniqueValueInfo({
                    value: dataValue,
                    symbol: new PointSymbol3D({
                        symbolLayers: [new IconSymbol3DLayer({
                            material: { color: "green" },
                            resource: { primitive: "circle" },
                            size: 12
                        })]
                    }),
                    label: "Theft of bicycle, scooter, or motorbike"
                });
                break;
            case "TRUCK, BUS, MOTOR HOME":
                this.renderer.addUniqueValueInfo({
                    value: dataValue,
                    symbol: new PointSymbol3D({
                        symbolLayers: [new IconSymbol3DLayer({
                            material: { color: "yellow" },
                            resource: { primitive: "circle" },
                            size: 12
                        })]
                    }),
                    label: "Theft of truck, bus or motorhome"
                });
                break;
            default:
                break;
        }
    };


    return {
        addRenderer : addRenderer,
        addClass: addClass
    }; 
});