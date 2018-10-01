using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface IProjectionProcessor
    {
        double ProjectedX { get; }
        double ProjectedY { get; }
    }
}
