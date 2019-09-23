using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;
using TileFactory.Layers;
using TileFactory.Models;
using TileFactory.Transforms;
using TileFactory.Utility;

namespace TileFactory
{
    public class Generator
    {
        #region Fields

        private readonly LayerTileCacheAccessor cacheAccessor;
        private readonly ITileContext tileContext;
        private readonly Transform transform;
        public readonly ILayerInitializationService tileInitService;

        #endregion

        #region Properties
        #endregion

        #region Methods

        public Generator(ITileContext context, 
            LayerTileCacheAccessor cacheAccessor, 
            ILayerInitializationService initService)
        {
            this.cacheAccessor = cacheAccessor;
            this.tileContext = context;
            this.transform = new Transform(context.Extent, context.Buffer);
            this.tileInitService = initService;

            if (tileContext == null)
                throw new NotSupportedException("The TileContext must have a value.");
        }

        public async Task<ITile> GenerateTile(int zoomLevel = 0, double x = 0, double y = 0)
        {
            if (tileContext.TileFeatures == null)
            {
                tileContext.TileFeatures = await tileInitService.InitializeLayer(tileContext.Identifier);

                // This is only called at the beginning //
                SplitTile(tileContext.TileFeatures.ToArray(),
                    zoom: 0, x: 0, y: 0, currentZoom: null, currentX: null, currentY: null);
            }

            // x must be the reduced value from the squared version of the zoom //
            // verify that the caller used this algorithm: //
            var zoomSqr = 1 << zoomLevel;
            var xDenom = ((x % zoomSqr) + zoomSqr) % zoomSqr; //
            if (xDenom != x)   
                throw new NotSupportedException($"The value for X (current value:{x}) was not properly calculated.");
            
            var tile = cacheAccessor.GetRawTile(tileContext.Identifier, zoomLevel, x, y);
            if (tile != null)
                return tile;

            var z0 = zoomLevel;
            var x0 = x;
            var y0 = y;
            ITile parent = null;

            while (parent == null && z0 > 0)
            {
                z0--;
                x0 = Math.Floor(x0 / 2);
                y0 = Math.Floor(y0 / 2);

                parent = cacheAccessor.GetRawTile(tileContext.Identifier, z0, x0, y0);
            }

            if (parent == null || parent.Source == null) return null;

            if (transform.IsClippedSquare(parent, tileContext.Extent, tileContext.Buffer))
                return parent;

            var solid = SplitTile(parent.Source.ToArray(), z0, (int)x0, (int)y0, zoomLevel, (int)x, (int)y);

            ITile currentTile = null;
            if (solid.HasValue)
            {
                var m = 1 << (zoomLevel - solid);
                currentTile = cacheAccessor.GetRawTile(tileContext.Identifier,
                    solid.Value, (int)Math.Floor((double)(x / m)), (int)Math.Floor((double)(y / m)));
            }
            else
                currentTile = cacheAccessor.GetRawTile(tileContext.Identifier, zoomLevel, x, y);

            return currentTile;
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
                var tileTolerance = zoom == 
                    tileContext.MaxZoom ? 0 : tileContext.Tolerance / (zoomSqr * tileContext.Extent);
                
                ITile currentTile = cacheAccessor.GetRawTile(tileContext.Identifier, zoom, x, y);
                // If the tile is null then this is a creation of a tile //
                if (currentTile == null)
                {
                    currentTile = CreateTile(features, zoomSqr, x, y, tileTolerance, zoom != tileContext.MaxZoom);
                    cacheAccessor.StoreRawTile(tileContext.Identifier, zoom, x, y, currentTile);
                }

                // TO-DO: figure out if this can be moved to the interals of currentTile //
                currentTile.Source = features;

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
                if (!tileContext.SolidChildren && transform.IsClippedSquare(currentTile, tileContext.Extent, tileContext.Buffer))
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
                    topLeft = clipper.Clip(left, scale: zoomSqr, k1: y - k1, k2: y + k3, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                    bottomLeft = clipper.Clip(left, scale: zoomSqr, k1: y + k2, k2: y + k4, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                }

                if (right != null && right.Length > 0)
                {
                    topRight = clipper.Clip(right, scale: zoomSqr, k1: y - k1, k2: y + k3, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                    bottomRight = clipper.Clip(right, scale: zoomSqr, k1: y + k2, k2: y + k4, axis: Axis.Y, minAll: currentTile.Min.Y, maxAll: currentTile.Max.Y);
                }

                if (features != null && features.Length > 0)
                {
                    tileStack.Push(y * 2);
                    tileStack.Push(x * 2);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(topLeft);

                    tileStack.Push(y * 2 + 1);
                    tileStack.Push(x * 2);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(bottomLeft);

                    tileStack.Push(y * 2);
                    tileStack.Push(x * 2 + 1);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(topRight);

                    tileStack.Push(y * 2 + 1);
                    tileStack.Push(x * 2 + 1);
                    tileStack.Push(zoom + 1);
                    tileStack.Push(bottomRight);
                }
            }


            return solidZoom;
        }

        internal ITile CreateTile(IEnumerable<IGeometryItem> features, int zoomSquared, int x, int y, double tileTolerance, bool shouldSimplify)
        {
            var tile = new GeoTile
            {
                ZoomSquared = zoomSquared,
                NumberOfPoints = 0,
                NumberOfSimplifiedPoints = 0,
                NumberOfFeatures = 0,
                X = x,
                Y = y,
                Features = new List<IGeometryItem>(),
                Source = new List<IGeometryItem>()
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
                    
                    if (min.X < tile.Min.X || tile.Min.X == 0) tileMinimum.X = min.X;
                    if (min.Y < tile.Min.Y || tile.Min.Y == 0) tileMinimum.Y = min.Y;

                    if (max.X > tile.Max.X || tile.Max.X == 0) tileMaximum.X = max.X;
                    if (max.Y > tile.Max.Y || tile.Max.Y == 0) tileMaximum.Y = max.Y;

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

            if (geometryFeature.Type == GeometryType.Point)
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
            else
            {
                // This occurs in a multi-line string scenario //
                tile.Features.Add(geometryFeature);
            }
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

            for (int i = 0; i < length; j = i++)
            {
                p1 = ring[i];
                p2 = ring[j];
                sum += (p2.X - p1.X) * (p1.Y + p2.Y);
            }
            return sum;
        }

        #endregion
    }
}
