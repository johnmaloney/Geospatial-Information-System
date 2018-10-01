using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline
{
    internal class PointData : IPoint
    {
        public GeometryType Type { get { return GeometryType.Point; } }
        public IPosition Coordinates { get; set; }
    }

    internal class PositionData : IPosition
    {
        public double? Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
