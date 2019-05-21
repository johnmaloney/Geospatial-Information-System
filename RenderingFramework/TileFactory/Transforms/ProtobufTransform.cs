using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Serialization;

namespace TileFactory.Transforms
{
    public interface ITransform<T>
    {
        T ProcessTile(ITile tile);
        (uint X, uint Y) ProcessPoint((double X, double Y, double Z) point, double extent, double zoomSqr, double tX, double tY);
    }

    public class ProtobufTransform : ITransform<Tile>
    {
        #region Fields

        private readonly Transform transformComposition;

        #endregion

        #region Properties
        #endregion

        #region Methods

        public ProtobufTransform(double extent, double buffer)
        {
            this.transformComposition = new Transform(extent, buffer);
        }

        public Tile ProcessTile(ITile tile)
        {
            var geometry = transformComposition.ProcessTile(tile);

            var feature = new TileFactory.Serialization.Tile.Types.Feature();
            feature.Geometry.AddRange(geometry.TransformedFeatures.SelectMany(item=> { return new uint[] { item.X, item.Y }; }));

            var layer = new TileFactory.Serialization.Tile.Types.Layer();
            layer.Features.Add(feature);

            var protoTile = new Tile();
            protoTile.Layers.Add(layer);
            return protoTile;            
        }

        public (uint X, uint Y) ProcessPoint((double X, double Y, double Z) point, double extent, double zoomSqr, double tX, double tY)
        {
            var x = (uint)Math.Round(extent * (point.X * zoomSqr - tX));
            var y = (uint)Math.Round(extent * (point.Y * zoomSqr - tY));

            return (X: x, Y: y);
        }

        #endregion
    }
}
