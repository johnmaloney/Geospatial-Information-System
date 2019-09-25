using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Utility;
using Universal.Contracts.Tiles;

namespace TileFactory.Layers
{
    /// <summary>
    /// This class allows for the generation of cache items based on a layer request
    /// It allows a single location for retrieving caches of raw and transformed tiles.
    /// </summary>
    public class LayerTileCacheAccessor
    {
        private readonly Func<ITileCacheStorage<ITransformedTile>> transformedGenerator;
        private readonly Func<ITileCacheStorage<ITile>> rawCacheGenerator;

        private ConcurrentDictionary<string, ITileCacheStorage<ITransformedTile>> transformedCaches = 
            new ConcurrentDictionary<string, ITileCacheStorage<ITransformedTile>>();

        private ConcurrentDictionary<string, ITileCacheStorage<ITile>> rawCaches =
            new ConcurrentDictionary<string, ITileCacheStorage<ITile>>();

        private readonly object transformedCacheLock = new object();
        private readonly object rawCacheLock = new object();


        public LayerTileCacheAccessor(
            Func<ITileCacheStorage<ITransformedTile>> transformedGenerator, 
            Func<ITileCacheStorage<ITile>> rawCacheGenerator)
        {
            this.transformedGenerator = transformedGenerator;
            this.rawCacheGenerator = rawCacheGenerator;
        }

        public ITransformedTile GetTransformedTile(string layerId, int zoom, double x, double y)
        {
            if (!transformedCaches.ContainsKey(layerId))
            {
                lock(transformedCacheLock)
                {
                    var newCache = transformedGenerator();
                    transformedCaches.TryAdd(layerId, newCache);
                }
            }

            if (transformedCaches.TryGetValue(layerId, out ITileCacheStorage<ITransformedTile> cache))
            {
                var id = Identifier.ToId(zoom, (int)x, (int)y);
                return cache.GetBy(id);
            }
            throw new NotSupportedException($"The cache for {layerId} at (zoom/x/y): {zoom}/{x}/{y} was not found.");
        }

        public ITile GetRawTile(string layerId, int zoom, double x, double y)
        {
            if (!rawCaches.ContainsKey(layerId))
            {
                lock (rawCacheLock)
                {
                    var newCache = rawCacheGenerator();
                    rawCaches.TryAdd(layerId, newCache);
                }
            }

            if (rawCaches.TryGetValue(layerId, out ITileCacheStorage<ITile> cache))
            {
                var id = Identifier.ToId(zoom, (int)x, (int)y);
                return cache.GetBy(id);
            }
            throw new NotSupportedException($"The cache for {layerId} at (zoom/x/y): {zoom}/{x}/{y} was not found.");
        }

        public void StoreTransformedTile(string layerId, int zoom, double x, double y, ITransformedTile transformedTile)
        {
            if (!transformedCaches.ContainsKey(layerId))
            {
                lock (transformedCacheLock)
                {
                    var newCache = transformedGenerator();
                    transformedCaches.TryAdd(layerId, newCache);
                }
            }

            if (transformedCaches.TryGetValue(layerId, out ITileCacheStorage<ITransformedTile> cache))
            {
                var id = Identifier.ToId(zoom, (int)x, (int)y);
                cache.StoreBy(id, transformedTile);
            }
            else
                throw new NotSupportedException($"The cache for {layerId} at (zoom/x/y): {zoom}/{x}/{y} could not be added to.");
        }

        public void StoreRawTile(string layerId, int zoom, double x, double y, ITile tile)
        {
            if (!rawCaches.ContainsKey(layerId))
            {
                lock (rawCacheLock)
                {
                    var newCache = rawCacheGenerator();
                    rawCaches.TryAdd(layerId, newCache);
                }
            }

            if (rawCaches.TryGetValue(layerId, out ITileCacheStorage<ITile> cache))
            {
                var id = Identifier.ToId(zoom, (int)x, (int)y);
                cache.StoreBy(id, tile);
            }
            else
                throw new NotSupportedException($"The cache for {layerId} at (zoom/x/y): {zoom}/{x}/{y} could not be added to.");
        }
    }
}
