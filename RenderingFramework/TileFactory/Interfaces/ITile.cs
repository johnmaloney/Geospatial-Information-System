using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface ITile
    {
        double ZoomSquared { get; }

        IList<IGeometryItem> Features { get; }

        IList<IGeometryItem> Source { get; set; }

        int NumberOfPoints { get; set; }

        int NumberOfSimplifiedPoints { get; set; }

        int NumberOfFeatures { get; set; }

        int X { get; }

        int Y { get; }

        (double X, double Y) Min { get; }

        (double X, double Y) Max { get; }
    }

    public interface ITransformedTile
    {
        (uint X, uint Y)[] TransformedFeatures { get; set; }
    }
}
