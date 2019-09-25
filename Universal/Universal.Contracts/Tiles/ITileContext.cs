using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface ITileContext : IContext
    {
        string Identifier { get; set; }
        IEnumerable<IGeometryItem> TileFeatures { get; set; }
    }
}
