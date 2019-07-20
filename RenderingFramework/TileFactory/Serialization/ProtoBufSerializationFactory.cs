using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TileFactory.Commands;
using TileFactory.Interfaces;

namespace TileFactory.Serialization
{
    public class ProtoBufSerializationFactory 
    {
        #region Fields

        private readonly EncodingFactory encodingFactory;
        private TileFactory.Serialization.Tile vectorTile;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public ProtoBufSerializationFactory()
        {
            encodingFactory = new EncodingFactory();
        }

        public bool BuildFrom(ITransformedTile tile, ITileContext context)
        {
            var pbfTile = new TileFactory.Serialization.Tile();
            var pbfLayer = new Serialization.Tile.Types.Layer();
            pbfLayer.Name = "Layer1";
            pbfLayer.Extent = (uint)context.Extent;
            pbfLayer.Version = 2;

            foreach (var feature in tile.TransformedFeatures)
            {
                var layerFeature = new Serialization.Tile.Types.Feature()
                {
                    Id = (ulong)new Random(0).Next(), 
                    Type = Tile.Types.GeomType.Polygon, 
                };

                layerFeature.Geometry.AddRange(encodingFactory.BuildEncodedGeometry(feature));
                
                pbfLayer.Features.Add(layerFeature);
            }
            pbfTile.Layers.Add(pbfLayer);
            vectorTile = pbfTile;

            return true;
        }

        public string SerializeTile()
        {
            using (var output = File.Create(@"C:\\temp\tile.pbf"))
            {
                vectorTile.WriteTo(output);
            }

            return string.Empty;                
        }

        #endregion
    }
}
