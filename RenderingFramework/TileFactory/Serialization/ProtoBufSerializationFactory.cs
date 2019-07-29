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
                    Type = convertToGeomType(feature.GeometryType)
                };

                layerFeature.Geometry.AddRange(encodingFactory.BuildEncodedGeometry(feature));
                
                pbfLayer.Features.Add(layerFeature);
            }
            pbfTile.Layers.Add(pbfLayer);
            vectorTile = pbfTile;

            return true;
        }

        private Tile.Types.GeomType convertToGeomType(int geometryTypeValue)
        {
            var geometryType = (GeometryType)geometryTypeValue;
            switch (geometryType)
            {
                case GeometryType.Unknown:
                    break;
                case GeometryType.Point:
                    return Tile.Types.GeomType.Point;
                case GeometryType.LineString:
                    break;
                case GeometryType.Polygon:
                    return Tile.Types.GeomType.Polygon;
                case GeometryType.MultiPoint:
                    break;
                case GeometryType.MultiLineString:
                    break;
                case GeometryType.MultiPolygon:
                    break;
                case GeometryType.GeometryCollection:
                    break;
                case GeometryType.Feature:
                    break;
                case GeometryType.FeatureCollection:
                    break;
                default:
                    break;
            }

            throw new NotSupportedException($"The Geometry type of {geometryType} cannot be converted to a Protobuf GeomType.");
        }

        public Stream SerializeTile()
        {
            var output = new MemoryStream();
            vectorTile.WriteTo(output);
            output.Position = 0;
            return output;               
        }

        #endregion
    }
}
