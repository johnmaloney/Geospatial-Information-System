using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface ICacheStorage<TKey, TValue>
    {
        TValue GetBy(TKey id);
        void StoreBy(TKey id, TValue tile);
        void Clear();
    }
}
