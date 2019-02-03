﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Tests.Mocks
{
    public class MockTileCacheStorage : ITileCacheStorage
    {
        #region Fields

        private ConcurrentDictionary<int, ITile> tiles = new ConcurrentDictionary<int, ITile>();

        #endregion

        #region Properties



        #endregion

        #region Methods

        public MockTileCacheStorage()
        {

        }

        public ITile GetBy(int id)
        {
            if (tiles.TryGetValue(id, out ITile tile))
                return tile;

            throw new KeyNotFoundException($"Tile with ID: {id} could not be retrieved.");
        }

        public void StoreBy(int id, ITile tile)
        {
            tiles.AddOrUpdate(id, tile, (currentId, currentTile) => currentTile = tile );
        }

        #endregion
    }
}