using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Models;
using TileFactory.Utility;

namespace TileFactory
{
    public class Retriever
    {
        #region Fields

        private readonly ITileCacheStorage tileCache;
        private readonly ITileContext tileContext;
        private readonly ITileTransform tileTransform;

        #endregion

        #region Properties

        public List<(double X, double Y, double Z)> Coordinates{ get; private set; }

        #endregion

        #region Methods

        public Retriever(ITileCacheStorage tileCache, ITileContext context)
        {
            this.tileCache = tileCache;
            this.tileContext = context;
            this.Coordinates = new List<(double X, double Y, double Z)>();

            // This is only called at the beginning //
            var initialTile = SplitTile(tileContext, zoom:0, x:0, y:0, currentX:null, currentY:null, currentZoom:null);
            tileCache.StoreBy(0, initialTile);
        }

        public ITile GetTile(IGeometryItem[] features, int zoomLevel=0, int x=0, int y=0)
        {
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
        internal int SplitTile(ITileContext context, int zoom, int x, int y, int? currentX, int? currentY, int? currentZoom)
        {
            var features = context.TileFeatures.ToArray();
            int solidZoom = 0;

            var tileStack = new Stack<object>();
            tileStack.Push(features);
            tileStack.Push(x);
            tileStack.Push(y);
            tileStack.Push(zoom);
            
            while (tileStack.Count > 0)
            {
                zoom = (int)tileStack.Pop();
                y = (int)tileStack.Pop();
                x = (int)tileStack.Pop();
                features = (Feature[])tileStack.Pop();


                var zoomSqr = 1 << zoom;
                var id = ToIdentifier(zoom, x, y);
                var tileTolerance = zoom == context.MaxZoom ? 0 : context.Tolerance / (zoomSqr * context.Extent);

                ITile currentTile = tileCache.GetBy(id);
                // If the tile is null then this is a creation of a tile //
                if (currentTile == null)
                {
                    currentTile = CreateTile(context.TileFeatures, zoomSqr, x, y, tileTolerance, zoom != context.MaxZoom);
                    tileCache.StoreBy(id, currentTile);
                    Coordinates.Add((X: x, Y: y, Z: zoom));
                }

                currentTile.Source = currentTile.Features;

                // If this is the first-pass Tiling //
                if (!currentZoom.HasValue)
                {
                    // Short circuit and return //
                    if (zoom == context.MaxZoomIndex || currentTile.NumberOfPoints <= context.MaxAllowablePoints)
                        continue;
                }
                else // Drill down to a specific Tile //
                {
                    // stop tiling if we've reached base zoom or our target tile zoom //
                    if (zoom == context.MaxZoom || zoom == currentZoom)
                        continue;

                    // STOP tiling if it's not an ancestor of the target tile //
                    var m = 1 << (currentZoom - zoom);
                    double dividendX = currentX.Value / m.Value;
                    double dividendY = currentY.Value / m.Value;
                    if (x != Math.Floor(dividendX) || y != Math.Floor(dividendY))
                        continue;
                }

                // stop tiling if the tile is a solid clipped square //
                if (!context.SolidChildren && IsClippedSquare(currentTile, context.Extent, context.Buffer))
                {
                    // Remember the current zoom if we are drilling down //
                    if (currentZoom.HasValue)
                        solidZoom = zoom;
                    continue;
                }

                // If we slice further down no need to keep source geometry //
                currentTile.Source = null;

                // Values used for Clipping //
                var k1 = 0.5 * (context.Buffer / context.Extent);
                var k2 = 0.5 - k1;
                var k3 = 0.5 + k1;
                var k4 = 1 + k1;

                var clipper = new Clipper();
                var left = clipper.Clip(features, scale: zoomSqr, k1: x - k1, k2: x + k3, axis: Axis.X, minAll: currentTile.Min.X, maxAll: currentTile.Max.X);
                var right = clipper.Clip(features, scale: zoomSqr, k1: x - k2, k2: x + k4, axis: Axis.X, minAll: currentTile.Min.X, maxAll: currentTile.Max.X);

                // set up the four sections of tiles //
                var topLeft = new IGeometryItem[0];
                var bottomLeft = new IGeometryItem[0];
                var topRight = new IGeometryItem[0];
                var bottomRight = new IGeometryItem[0]; 

                if (left.Length > 0)
                {
                    topLeft =    clipper.Clip(left, scale: zoomSqr, k1: y - k1, k2: y + k3, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                    bottomLeft = clipper.Clip(left, scale: zoomSqr, k1: y + k2, k2: y + k4, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                }

                if(right.Length > 0)
                {
                    topRight =    clipper.Clip(right, scale: zoomSqr, k1: y - k1, k2: y + k3, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                    bottomRight = clipper.Clip(right, scale: zoomSqr, k1: y + k2, k2: y + k4, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                }

                if (features.Length > 0)
                {
                    tileStack.Push(topLeft);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(x * 2);
                    tileStack.Push(y * 2);
                
                    tileStack.Push(bottomLeft);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(x * 2);
                    tileStack.Push(y * 2 + 1);
                    
                    tileStack.Push(topRight);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(x * 2 + 1);
                    tileStack.Push(y * 2);
                    
                    tileStack.Push(bottomRight);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(x * 2 + 1);
                    tileStack.Push(y * 2 + 1);
                }
            }


            return zoom;
        }

        internal ITile CreateTile(IEnumerable<Feature> features, int zoomSquared, int x, int y, double tileTolerance, bool shouldSimplify)
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
                AddFeature(tile, feature, tileTolerance, shouldSimplify);
            }

            return tile;
        }

        internal void AddFeature(ITile tile, Feature feature, double tileTolerance, bool shouldSimplify)
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
                        && (feature.Type == GeometryType.LineString | feature.Type == GeometryType.MultiLineString 
                            && feature.Distance[i] < tileTolerance)
                        || (feature.Type == GeometryType.Polygon | feature.Type == GeometryType.MultiPolygon 
                            && feature.Area[i] < sqTolerance))
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
                        
                        if (!shouldSimplify || (point.Z) > (sqTolerance))
                        {
                            simplifiedRing.Add(point);
                            tile.NumberOfSimplifiedPoints++;
                        }
                        tile.NumberOfPoints++;
                    }

                    if (feature.Type == GeometryType.Polygon | feature.Type == GeometryType.MultiPolygon)
                        Rewind(simplifiedRing, feature.IsOuter);

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

        internal int ToIdentifier(int zoom, int x, int y)
        {
            return ((1 << zoom) * y + x) * 32 + zoom;
        }

        internal void Rewind(List<(double X, double Y, double Z)> ring, bool clockwise = true)
        {
            var area = SignedArea(ring);
            if (area < 0 == clockwise)
                ring.Reverse();
        }

        internal double SignedArea(List<(double X, double Y, double Z)> ring)
        {
            double sum = 0;
            var length = ring.Count;
            var j = length - 1;
            (double X, double Y, double Z) p1, p2;

            for (int i = 0; i < length; j= i++)
            {
                p1 = ring[i];
                p2 = ring[j];
                sum += (p2.X - p1.X) * (p1.Y + p2.Y);
            }
            return sum;
        }

        internal bool IsClippedSquare(ITile tile, double extent, double buffer)
        {
            var features = tile.Features;
            if (features.Count != 1)
                return false;

            var feature = features.Single();
            if (feature.Type != GeometryType.MultiLineString || feature.Geometry.Length > 1)
                return false;

            var length = feature.Geometry[0].Length;
            if (length != 5)
                return false;

            for (int i = 0; i < length; i++)
            {
                var point = tran
            }
        }

        #endregion
    }
}
