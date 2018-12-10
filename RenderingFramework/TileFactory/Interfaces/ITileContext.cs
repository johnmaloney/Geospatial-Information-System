using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Interfaces
{
    public interface ITileContext
    {
        IEnumerable<TileFactory.Feature> TileFeatures { get; set; }
    }
}
