using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Utility
{
    /// <summary>
    /// Transforms the coordinates of each feature in the given tile from
    /// mercator-projected space into (extent x extent) tile space.
    /// </summary>
    public class Transform
    {
        #region Fields

        private readonly double buffer;
        private readonly double extent;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public Transform(double extent, double buffer)
        {
            this.extent = extent;
            this.buffer = buffer;
        }

        public ITile ProcessTile(ITile tile)
        {
            if (tile.Transformed)
                return tile;

            foreach(var feature in tile.Features)
            {
                if (feature.Type == GeometryType.MultiPoint)
                {
                    for (int i = 0; i < feature.Geometry.Length; i++)
                    {
                        feature.Geometry[i][0] = ProcessPoint(feature.Geometry[i][0], extent, tile.ZoomSquared, tile.X, tile.Y);
                    }
                }
                else
                {
                    for (int i = 0; i < feature.Geometry.Length; i++)
                    {
                        var ring = feature.Geometry[i];
                        for (int j = 0; j < ring.Length; j++)
                        {
                            ring[j]= ProcessPoint(feature.Geometry[i][j], extent, tile.ZoomSquared, tile.X, tile.Y);
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }

        public (double X, double Y, double Z) ProcessPoint((double X, double Y, double Z) point, double extent, double zoomSqr, double tX, double tY)
        {
            var x = Math.Round(extent * (point.X * zoomSqr - tX));
            var y = Math.Round(extent * (point.Y * zoomSqr - tY));

            return (X: x, Y: y, Z: point.Z);
        }

        #endregion
    }
}
