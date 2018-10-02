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

    public interface IGeometryItem : IGeospatialItem
    {
        /// <summary>
        /// Represents the structure of a feature (e.g. Point, Line, Polygon) 
        /// in the case of a point the array will only be one level (i.e. Geometry[0][0])
        /// in all other cases the array will have multi levels.
        /// </summary>
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
