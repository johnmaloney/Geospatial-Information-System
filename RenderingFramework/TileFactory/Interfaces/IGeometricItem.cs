using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface IGeometryItem
    {
        GeometryType Type { get; }
        /// <summary>
        /// Represents the structure of a feature (e.g. Point, Line, Polygon) 
        /// in the case of a point the array will only be one level (i.e. Geometry[0][0])
        /// in all other cases the array will have multi levels.
        /// </summary>
        (double X, double Y, double Z)[][] Geometry { get; set; }
        (double X, double Y, double Z) MinGeometry { get; }
        (double X, double Y, double Z) MaxGeometry { get; }
    }

    public interface IGeoData
    {
        string Id { get; set; }

        IDictionary<string, object> Tags { get; set; }
    }

}
