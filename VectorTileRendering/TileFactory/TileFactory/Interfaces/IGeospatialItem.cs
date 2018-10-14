using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{

    public interface IGeospatialItem
    {
        GeometryType Type { get; }
        double? Altitude { get; }
        double Latitude { get; }
        double Longitude { get; }
    }
}
