using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Models;

namespace TileFactory
{
    public class Retriever
    {
        #region Fields

        private ConcurrentDictionary<int, object> tiles = new ConcurrentDictionary<int, object>();
        private readonly ITileContext tileContext;

        #endregion

        #region Properties



        #endregion

        #region Methods
        public object GetTile(ITileContext tileContext, int zoomLevel, int x, int y)
        {
            SplitTile(tileContext, zoomLevel, x, y);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// GeoJSONVT.prototype.splitTile
        /// </remarks>
        /// <param name="features"></param>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal object SplitTile(ITileContext context, int zoom, int x, int y)
        {
            var zoomSqr = 1 << zoom;
            var id = toIdentifier(zoom, x, y);

            tiles.TryGetValue(id,out object currentTile);
            var tileTolerance = zoom == context.MaxZoom ? 0 : context.Tolerance / (zoomSqr * context.Extent);

            // If the tile is null then this is a creation of a tile //
            if (currentTile == null)
            {
                currentTile = createTile(context.TileFeatures, zoomSqr, x, y, tileTolerance, zoom != context.MaxZoom);
            }

            return null;
        }

        internal ITile createTile(IEnumerable<Feature> features, int zoomSquared, int x, int y, double tileTolerance, bool shouldSimplify)
        {
            var tile = new DynamicTile
            {
                NumberOfPoints = 0,
                NumberOfSimplifiedPoints = 0,
                NumberOfFeatures = 0,
                X = x,
                Y = y,
                Features = new List<Feature>()
            };

            foreach(var feature in features)
            {
                tile.NumberOfFeatures++;
                addFeature(tile, feature, tileTolerance, shouldSimplify);
            }

            return tile;
        }

        private void addFeature(ITile tile, Feature feature, double tileTolerance, bool shouldSimplify)
        {
            var simplified = new List<(double X, double Y, double Z)[]>();
            var sqTolerance = (tileTolerance * tileTolerance);

            if (feature.Type == GeometryType.MultiPoint)
            {
                for (int i = 0; i < feature.Geometry.Length; i++)
                {
                    simplified.Add(feature.Geometry[i]);
                    tile.NumberOfPoints++;
                    tile.NumberOfSimplifiedPoints++;
                }
            }
            else
            {
                // simplify and transform projected coordinates for tile geometry //
                for (int i = 0; i < feature.Geometry.Length; i++)
                {                    
                    // filter out tiny polylines & polygons //
                    if (shouldSimplify 
                        && (feature.Type == GeometryType.LineString && feature.Distance < tileTolerance)
                        || (feature.Type == GeometryType.MultiLineString && feature.Area < sqTolerance))
                    {
                        tile.NumberOfPoints += feature.Geometry.Length;
                        continue;
                    }

                    var simplifiedRing = new List<(double X, double Y, double Z)>();
                    var ring = feature.Geometry[i];

                    // Work through the Ring of points //
                    for (int j = 0; j < ring.Length; j++)
                    {
                        var point = ring[j];
                        
                        if (!shouldSimplify || point.Z > sqTolerance)
                        {
                            simplifiedRing.Add(point);
                            tile.NumberOfSimplifiedPoints++;
                        }
                        tile.NumberOfPoints++;
                    }

                    if (feature.Type == GeometryType.MultiLineString)
                        throw new NotImplementedException("MultLineString Type is not implemented yet.");//rewind(simplifiedRing, feature.IsOuter);

                    simplified.Add(simplifiedRing.ToArray());
                }
            }

            if (simplified.Count > 0)
            {
                var tileFeature = new Feature(feature.Type)
                {
                    Geometry = simplified.ToArray(),
                    Tags = feature.Tags
                };

                if (!string.IsNullOrEmpty(feature.Id))
                    tileFeature.Id = feature.Id;

                tile.Features.Add(tileFeature);
            }
        }

        private int toIdentifier(int zoom, int x, int y)
        {
            return ((1 << zoom) * y + x) * 32 + zoom;
        }
        #endregion
    }
}
