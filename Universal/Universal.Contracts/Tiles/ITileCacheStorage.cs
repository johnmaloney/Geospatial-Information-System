using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface ITileCacheStorage<TTile>
    {
        TTile GetBy(int id);
        void StoreBy(int id, TTile tile);
        void Clear();
    }
}
