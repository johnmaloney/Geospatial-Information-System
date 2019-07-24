using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Models
{
    public class SimpleTileCacheStorage<TTile> : ITileCacheStorage<TTile> where TTile : class
    {
        #region Fields

        private ConcurrentDictionary<int, TTile> tiles = new ConcurrentDictionary<int, TTile>();

        #endregion

        #region Properties



        #endregion

        #region Methods

        public SimpleTileCacheStorage()
        {

        }

        public TTile GetBy(int id)
        {
            if (tiles.TryGetValue(id, out TTile tile))
                return tile;

            return null;
        }

        public void StoreBy(int id, TTile tile)
        {
            //var carbon = tile.CarbonCopy(); 
            tiles.AddOrUpdate(id, tile, (currentId, currentTile) => currentTile = tile);
        }

        public void Clear()
        {
            tiles.Clear();
        }

        #endregion
    }
}
