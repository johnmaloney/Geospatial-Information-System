using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Serialization
{
    public class ProtoBufSerializationFactory 
    {
        public object BuildFrom(ITransformedTile tile, ITileContext context)
        {
            var pbfTile = new TileFactory.Serialization.Tile();
            var pbfLayer = new Serialization.Tile.Types.Layer();
            pbfLayer.Extent = (uint)context.Extent;
            pbfLayer.Version = 2;

            foreach (var feature in tile.TransformedFeatures)
            {
                pbfLayer.Features.Add(new Serialization.Tile.Types.Feature() { });
            }
            pbfTile.Layers.Add(pbfLayer);
            return pbfTile;
        }
    }
}
