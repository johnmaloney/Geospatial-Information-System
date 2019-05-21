using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Transforms
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

        public ITransformedTile ProcessTile(ITile tile)
        {
            (uint X, uint Y)[] transformedRing = null;

            foreach (var feature in tile.Features)
            {
                if (feature.Type == GeometryType.MultiPoint)
                {
                    throw new NotSupportedException("This type of transform is not supported yet.");
                    //var transformedRing = new (uint X, uint Y)[feature.Geometry.Length][];
                    //for (int i = 0; i < feature.Geometry.Length; i++)
                    //{
                    //    transformedRing[i][0] = ProcessPoint(feature.Geometry[i][0], extent, tile.ZoomSquared, tile.X, tile.Y);
                    //}
                    //return transformedRing
                }
                else
                {
                    for (int i = 0; i < feature.Geometry.Length; i++)
                    {
                        var ring = feature.Geometry[i];
                        transformedRing = new (uint X, uint Y)[ring.Length];
                        for (int j = 0; j < ring.Length; j++)
                        {
                            transformedRing[j]= ProcessPoint(feature.Geometry[i][j], extent, tile.ZoomSquared, tile.X, tile.Y);
                        }
                    }
                }
            }
                       
            return new TransformedTile(transformedRing);
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
