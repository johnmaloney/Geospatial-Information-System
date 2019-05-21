using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Tests.Mocks
{
    public class MockRawCacheStorage : ITileCacheStorage<ITile>
    {
        #region Fields

        private ConcurrentDictionary<int, ITile> tiles = new ConcurrentDictionary<int, ITile>();

        #endregion

        #region Properties



        #endregion

        #region Methods

        public MockRawCacheStorage()
        {

        }

        public ITile GetBy(int id)
        {
            if (tiles.TryGetValue(id, out ITile tile))
                return tile;

            return null;
        }

        public void StoreBy(int id, ITile tile)
        {
            tiles.AddOrUpdate(id, tile, (currentId, currentTile) => currentTile = tile );
        }

        #endregion
    }
}
