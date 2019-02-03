using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public enum Axis : int
    {
        X = 0,
        Y = 1
    }

    /// <summary>
    /// Maps the Geo Types. The relationship between these types and 
    /// the leaflet types is not one to one:
    /// Point & Multipoint = 1
    /// LineString & MultiLineString = 2
    /// Polygon & Multipolygon = 3
    /// 
    /// </summary>
    public enum GeometryType
    {
        Point = 0,
        MultiPoint = 1,
        LineString = 2,
        MultiLineString = 3,
        Polygon = 4,
        MultiPolygon = 5,
        GeometryCollection = 6,
        Feature = 7,
        FeatureCollection = 8
    }
}
