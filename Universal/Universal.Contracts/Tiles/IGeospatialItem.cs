using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface IGeospatialItem
    {
        GeometryType Type { get; }
        double? Altitude { get; }
        double Latitude { get; }
        double Longitude { get; }
    }
}
