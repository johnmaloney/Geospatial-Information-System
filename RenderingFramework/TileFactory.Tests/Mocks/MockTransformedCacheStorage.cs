using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Serialization;
using TileFactory.Transforms;
using Universal.Contracts.Tiles;

namespace TileFactory.Tests.Mocks
{
    public class MockTransformedCacheStorage : ITileCacheStorage<ITransformedTile>
    {
        #region Fields

        private readonly ProtobufTransform transform;
        private ConcurrentDictionary<int, ITransformedTile> tiles = new ConcurrentDictionary<int, ITransformedTile>();

        #endregion

        #region Properties



        #endregion

        #region Methods

        public MockTransformedCacheStorage()
        {
            
        }

        public ITransformedTile GetBy(int id)
        {
            if (tiles.TryGetValue(id, out ITransformedTile tile))
                return tile;

            return null;
        }

        public void StoreBy(int id, ITransformedTile tile)
        {
            tiles.AddOrUpdate(id, tile, (currentId, currentTile) => currentTile = tile);
        }

        public void Clear()
        {
            tiles.Clear();
        }

        #endregion
    }
}
