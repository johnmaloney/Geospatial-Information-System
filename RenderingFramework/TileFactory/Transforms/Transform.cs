﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            var transformedRings = new List<ITransformedFeature>();

            foreach (var feature in tile.Features)
            {
                (int X, int Y)[] transformedRing = null;

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
                        transformedRing = new (int X, int Y)[ring.Length];
                        for (int j = 0; j < ring.Length; j++)
                        {
                            transformedRing[j]= ProcessPoint(feature.Geometry[i][j], extent, tile.ZoomSquared, tile.X, tile.Y);
                        }
                    }
                }
                transformedRings.Add(new TransformedFeature((int)feature.Type, transformedRing));    
            }

            return new TransformedTile(transformedRings);
        }

        public (int X, int Y) ProcessPoint((double X, double Y, double Z) point, double extent, double zoomSqr, double tX, double tY)
        {
            var x = (int)Math.Round(extent * (point.X * zoomSqr - tX));
            var y = (int)Math.Round(extent * (point.Y * zoomSqr - tY));

            return (X: x, Y: y);
        }

        /// <summary>
        /// checks whether a tile is a whole-area fill after clipping; if it is, there's no sense slicing it further
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="extent"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool IsClippedSquare(ITile tile, double extent, double buffer)
        {
            var features = tile.Features;
            if (features.Count != 1)
                return false;

            var feature = features.Single();
            if (feature.Type != GeometryType.Polygon || feature.Geometry.Length > 1)
                return false;

            var length = feature.Geometry[0].Length;
            if (length != 5)
                return false;

            for (int i = 0; i < length; i++)
            {
                var point = ProcessPoint(feature.Geometry[i][0], extent, tile.ZoomSquared, tile.X, tile.Y);
                if ((point.X != -buffer && point.X != extent + buffer) ||
                    (point.Y != -buffer && point.Y != extent + buffer))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
