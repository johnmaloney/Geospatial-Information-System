using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
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

    public interface IGeometryItem
    {
        (double X, double Y, double Z)[][] Geometry { get; }
        (double X, double Y, double Z) MinGeometry { get; }
        (double X, double Y, double Z) MaxGeometry { get; }
    }

    public interface IGeospatialItem
    {
        GeometryType Type { get; }
    }

    public interface IPoint : IGeospatialItem
    {
        IPosition Coordinates { get; }
    }

    public interface IPosition
    {
        double? Altitude { get; }
        double Latitude { get; }
        double Longitude { get; }
    }
}
