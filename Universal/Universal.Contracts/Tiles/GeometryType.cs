using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    /// <summary>
    /// Maps the Geo Types. The relationship between these types and 
    /// the leaflet types is not one to one:
    /// Point & Multipoint = 1
    /// LineString & MultiLineString = 2
    /// Polygon & Multipolygon = 3
    /// 
    /// </summary>
    public enum GeometryType : int
    {
        // These first four (i.e. Unknown, Point, Linestring, Polygon) //
        // follow the Vector Tile Spec ProtoBuf object//
        // https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto
        Unknown = 0,
        Point = 1,
        LineString = 2,
        Polygon = 3,

        MultiPoint = 4,
        MultiLineString = 5,
        MultiPolygon = 6,
        GeometryCollection = 7,
        Feature = 8,
        FeatureCollection = 9
    }
}
