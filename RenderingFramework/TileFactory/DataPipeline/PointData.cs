using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using Universal.Contracts.Tiles;

namespace TileFactory.DataPipeline
{
    public struct PointData : IGeospatialItem
    {
        public GeometryType Type { get; private set; }

        public double? Altitude { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public PointData(GeometryType type, double latitude, double longitude, double altitude)
        {
            Type = type;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
    }

}
