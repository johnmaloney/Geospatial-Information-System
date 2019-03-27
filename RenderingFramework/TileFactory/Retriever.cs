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
        private readonly Transform tileTransform;

        #endregion

        #region Properties

        public List<(double X, double Y, double Z)> Coordinates{ get; private set; }

        #endregion

        #region Methods

        public Retriever(ITileCacheStorage tileCache, ITileContext context)
        {
            this.tileCache = tileCache;
            this.tileContext = context;
            this.tileTransform = new Transform(context.Extent, context.Buffer);
            this.Coordinates = new List<(double X, double Y, double Z)>();

            // This is only called at the beginning //
            var initialTile = SplitTile(tileContext.TileFeatures.ToArray(), zoom:0, x:0, y:0, currentZoom: null, currentX:null, currentY:null);
        }

        public ITile GetTile(IGeometryItem feature, int zoomLevel=0, double x=0, double y=0)
        {
            var zoomSqr = 1 << zoomLevel;
            x = ((x % zoomSqr) + zoomSqr) % zoomSqr;

            var id = ToIdentifier(zoomLevel, (int)x, (int)y);
            var tile = tileCache.GetBy(id);
            if (tile != null)
                return tileTransform.ProcessTile(tile);

            var z0 = zoomLevel;
            var x0 = x;
            var y0 = y;
            ITile parent = null;

            while(parent == null && z0 > 0)
            {
                z0--;
                x0 = Math.Floor(x0 / 2);
                y0 = Math.Floor(y0 / 2);
                parent = tileCache.GetBy(ToIdentifier(z0, (int)x0, (int)y0));
            }

            if (parent == null || parent.Source == null) return null;

            if (IsClippedSquare(parent, tileContext.Extent, tileContext.Buffer))
                return tileTransform.ProcessTile(parent);

            var solid = SplitTile(parent.Source.ToArray(), z0, (int)x0, (int)y0, zoomLevel, (int)x, (int)y);

            if (solid.HasValue)
            {
                var m = 1 << (zoomLevel - solid);
                id = ToIdentifier(solid.Value, (int)Math.Floor((double)(x / m)), (int)Math.Floor((double)(y / m)));
            }

            var currentTile = tileCache.GetBy(id);
            return currentTile != null ? tileTransform.ProcessTile(currentTile) : null;
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
        internal int? SplitTile(IGeometryItem[] features, int zoom, int x, int y, int? currentZoom, int? currentX, int? currentY)
        {
            int? solidZoom = null;

            var tileStack = new Stack<object>();
            tileStack.Push(y);
            tileStack.Push(x);
            tileStack.Push(zoom);
            tileStack.Push(features);

            while (tileStack.Count > 0)
            {
                features = (IGeometryItem[])tileStack.Pop();
                zoom = (int)tileStack.Pop();
                x = (int)tileStack.Pop();
                y = (int)tileStack.Pop();


                var zoomSqr = 1 << zoom;
                var id = ToIdentifier(zoom, x, y);
                var tileTolerance = zoom == tileContext.MaxZoom ? 0 : tileContext.Tolerance / (zoomSqr * tileContext.Extent);

                ITile currentTile = tileCache.GetBy(id);
                // If the tile is null then this is a creation of a tile //
                if (currentTile == null)
                {
                    currentTile = CreateTile(features, zoomSqr, x, y, tileTolerance, zoom != tileContext.MaxZoom);
                    tileCache.StoreBy(id, currentTile);
                    Coordinates.Add((X: x, Y: y, Z: zoom));
                }

                currentTile.Source = currentTile.Features;

                // If this is the first-pass Tiling //
                if (!currentZoom.HasValue)
                {
                    // Short circuit and return //
                    if (zoom == tileContext.MaxZoomIndex || currentTile.NumberOfPoints <= tileContext.MaxAllowablePoints)
                        continue;
                }
                else // Drill down to a specific Tile //
                {
                    // stop tiling if we've reached base zoom or our target tile zoom //
                    if (zoom == tileContext.MaxZoom || zoom == currentZoom)
                        continue;

                    // STOP tiling if it's not an ancestor of the target tile //
                    var m = 1 << (currentZoom - zoom);
                    double dividendX = currentX.Value / m.Value;
                    double dividendY = currentY.Value / m.Value;
                    if (x != Math.Floor(dividendX) || y != Math.Floor(dividendY))
                        continue;
                }

                // stop tiling if the tile is a solid clipped square //
                if (!tileContext.SolidChildren && IsClippedSquare(currentTile, tileContext.Extent, tileContext.Buffer))
                {
                    // Remember the current zoom if we are drilling down //
                    if (currentZoom.HasValue)
                        solidZoom = zoom;
                    continue;
                }

                // If we slice further down no need to keep source geometry //
                currentTile.Source = null;

                // Values used for Clipping //
                var k1 = 0.5 * (tileContext.Buffer / tileContext.Extent);
                var k2 = 0.5 - k1;
                var k3 = 0.5 + k1;
                var k4 = 1 + k1;

                var clipper = new Clipper();
                var left = clipper.Clip(features, scale: zoomSqr, k1: x - k1, k2: x + k3, axis: Axis.X, minAll: currentTile.Min.X, maxAll: currentTile.Max.X);
                var right = clipper.Clip(features, scale: zoomSqr, k1: x + k2, k2: x + k4, axis: Axis.X, minAll: currentTile.Min.X, maxAll: currentTile.Max.X);

                // set up the four sections of tiles //
                IGeometryItem[] topLeft = null;
                IGeometryItem[] bottomLeft = null;
                IGeometryItem[] topRight = null;
                IGeometryItem[] bottomRight = null; 

                if (left != null && left.Length > 0)
                {
                    topLeft =    clipper.Clip(left, scale: zoomSqr, k1: y - k1, k2: y + k3, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                    bottomLeft = clipper.Clip(left, scale: zoomSqr, k1: y + k2, k2: y + k4, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                }

                if(right != null && right.Length > 0)
                {
                    topRight =    clipper.Clip(right, scale: zoomSqr, k1: y - k1, k2: y + k3, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                    bottomRight = clipper.Clip(right, scale: zoomSqr, k1: y + k2, k2: y + k4, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                }

                if (features != null && features.Length > 0)
                {
                    tileStack.Push(y * 2 + 1);
                    tileStack.Push(x * 2 + 1);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(bottomRight);

                    tileStack.Push(y * 2);
                    tileStack.Push(x * 2 + 1);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(topRight);

                    tileStack.Push(y * 2 + 1);
                    tileStack.Push(x * 2);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(bottomLeft);

                    tileStack.Push(y * 2);
                    tileStack.Push(x * 2);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(topLeft);
                }
            }


            return solidZoom;
        }

        internal ITile CreateTile(IEnumerable<IGeometryItem> features, int zoomSquared, int x, int y, double tileTolerance, bool shouldSimplify)
        {
            var tile = new DynamicTile
            {
                ZoomSquared = zoomSquared,
                NumberOfPoints = 0,
                NumberOfSimplifiedPoints = 0,
                NumberOfFeatures = 0,
                X = x,
                Y = y,
                Features = new List<IGeometryItem>()
            };

            if (features != null)
            {
                foreach (var feature in features)
                {
                    tile.NumberOfFeatures++;
                    AddFeature(tile, feature, tileTolerance, shouldSimplify);

                    var min = feature.MinGeometry;
                    var max = feature.MaxGeometry;

                    (double X, double Y) tileMinimum = (X: tile.Min.X, Y: tile.Min.Y);
                    (double X, double Y) tileMaximum = (X: tile.Max.X, Y: tile.Max.Y);

                    if (min.X > tile.Min.X) tileMinimum.X = min.X;
                    if (min.Y > tile.Min.Y) tileMinimum.Y = min.Y;

                    if (max.X > tile.Max.X) tileMaximum.X = max.X;
                    if (max.Y > tile.Max.Y) tileMaximum.Y = max.Y;

                    tile.Min = tileMinimum;
                    tile.Max = tileMaximum;
                }
            }
            return tile;
        }

        internal void AddFeature(ITile tile, IGeometryItem geometryFeature, double tileTolerance, bool shouldSimplify)
        {
            var simplified = new List<(double X, double Y, double Z)[]>();
            var sqTolerance = (tileTolerance * tileTolerance);

            if (geometryFeature.Type == GeometryType.MultiPoint)
            {
                for (int i = 0; i < geometryFeature.Geometry.Length; i++)
                {
                    simplified.Add(geometryFeature.Geometry[i]);
                    tile.NumberOfPoints++;
                    tile.NumberOfSimplifiedPoints++;
                }
            }
            else
            {
                Feature feature = geometryFeature as Feature;
                if (feature == null)
                    throw new NotSupportedException($"The Geometry Feature of type {geometryFeature.GetType()} is not supported.");

                // simplify and transform projected coordinates for tile geometry //
                for (int i = 0; i < geometryFeature.Geometry.Length; i++)
                {
                    // filter out tiny polylines & polygons //
                    if (shouldSimplify 
                        && (geometryFeature.Type == GeometryType.LineString | geometryFeature.Type == GeometryType.MultiLineString 
                            && feature.Distance[i] < tileTolerance)
                        || (geometryFeature.Type == GeometryType.Polygon | geometryFeature.Type == GeometryType.MultiPolygon 
                            && feature.Area[i] < sqTolerance))
                    {
                        tile.NumberOfPoints += geometryFeature.Geometry.Length;
                        continue;
                    }

                    var simplifiedRing = new List<(double X, double Y, double Z)>();
                    var ring = geometryFeature.Geometry[i];

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

                    if (geometryFeature.Type == GeometryType.Polygon | geometryFeature.Type == GeometryType.MultiPolygon)
                        Rewind(simplifiedRing, feature.IsOuter);

                    simplified.Add(simplifiedRing.ToArray());
                }
            }

            if (simplified.Count > 0)
            {
                var tileFeature = new Feature(geometryFeature.Type)
                {
                    Geometry = simplified.ToArray()
                };

                var geoData = geometryFeature as IGeoData;
                if (geoData != null && !string.IsNullOrEmpty(geoData.Id))
                {
                    tileFeature.Tags = geoData.Tags;
                    tileFeature.Id = geoData.Id;
                }

                tile.Features.Add(tileFeature);
            }
        }

        internal int ToIdentifier(int zoom, int x, int y)
        {
            return (((1 << zoom) * y + x) * 32) + zoom;
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

        /// <summary>
        /// checks whether a tile is a whole-area fill after clipping; if it is, there's no sense slicing it further
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="extent"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
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
                var point = tileTransform.ProcessPoint(feature.Geometry[i][0], extent, tile.ZoomSquared, tile.X, tile.Y);
                if ((point.X != -buffer && point.X != extent + buffer) ||
                    (point.Y != -buffer && point.Y != extent + buffer))
                    return false;
            }

            return true;
        }

        #endregion
    }
}
