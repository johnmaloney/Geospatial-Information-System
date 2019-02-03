using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface ITile
    {
        IList<Feature> Features { get; }

        IList<Feature> Source { get; set; }

        int NumberOfPoints { get; set; }

        int NumberOfSimplifiedPoints { get; set; }

        int NumberOfFeatures { get; set; }

        int X { get; }

        int Y { get; }

        bool Transformed { get; }

        (int X, int Y) Min { get; }

        (int X, int Y) Max { get; }
    }
}
