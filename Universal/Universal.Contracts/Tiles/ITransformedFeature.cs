using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface ITransformedFeature
    {
        (int X, int Y)[] Coordinates { get; }

        int GeometryType { get; }
    }
}
