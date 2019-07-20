using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.DataPipeline;
using TileFactory.Tests.Mocks;

namespace TileFactory.Tests.Utility
{
    public class MockContextRepository
    {
        #region Fields

        private readonly ConcurrentDictionary<string, string> serialContexts;

        #endregion

        #region Properties


        #endregion

        #region Methods

        public MockContextRepository()
        {
            var mockContext = new MockTileContext()
            {
                MaxZoom = 14,
                Buffer = 64,
                Extent = 4096,
                Tolerance = 3, 
                Identifier = "colorado_outline_projected.json"
            };

            serialContexts = new ConcurrentDictionary<string, string>();
            serialContexts.TryAdd("base", mockContext.SerializeToJson()); 
        }

        public bool TryGetAs<T>(string identifier, out T instance) where T : class
        {
            if (serialContexts.TryGetValue(identifier, out string context))
            {
                instance = context.DeserializeJson<T>();
                return true;
            }

            instance = null;
            return false;
        }

        #endregion
    }
}
