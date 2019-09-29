### Tile Rendering

This section contains the tile rendering and data projection tools. 
This is a C# port of the GeoJSON-vt Javascript library designed 
[Vladimir Agafonkin](https://blog.mapbox.com/rendering-big-geodata-on-the-fly-with-geojson-vt-4e4d2a5dd1f2).
It is designed to allow for more datasource types in the future but currently only supports GeoJSON.

This library also manages the generation of the Vector Tile and the serialization into ProtoBuf pakages.
The caching of the projected tiles and the geometric transformed tiles are stored in memory on the server, but can be
replaced with a more robust cache (e.g Redis).

An implementation of this library can be [accessed here](https://gis-processor.azurewebsites.net/api/layer).

The library has high test coverage and most behaviors can be seen from the unit tests in [this project](https://github.com/johnmaloney/Geospatial-Information-System/tree/master/RenderingFramework/TileFactory.Tests).

#### References:

[GeoJson Editor](http://geojson.io/#map=15/40.7144/-74.0073)  
[GeoJSON Vector Tiles in Javascript](https://github.com/mapbox/geojson-vt)  
[Vector Tile Specification](https://github.com/mapbox/vector-tile-spec)  
[Blog post describing the use of GeoJSON-vt](https://chriswhong.com/uncategorized/building-static-vector-tile-trees-from-geojson/)  
[Common naming scheme for Tile generation](https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#C.23)  
[Leaflet and ProtoBuf](http://leaflet.github.io/Leaflet.VectorGrid/vectorgrid-api-docs.html#vectorgrid-protobuf)  
[Google Protocol Buffers](https://developers.google.com/protocol-buffers/docs/csharptutorial)  