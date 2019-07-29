using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TileFactory.DataPipeline;
using TileFactory.Tests.Mocks;
using Universal.Contracts.Serial;

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
                Identifier = "colorado_outline_projected"
            };

            var mockPointsContext = mockContext.SerializeToJson().FromJsonInto<MockTileContext>();
            mockPointsContext.Identifier = "populated_points_simple_projected";


            var mockPointsDenverOnlyContext = mockContext.SerializeToJson().FromJsonInto<MockTileContext>();
            mockPointsDenverOnlyContext.Identifier = "populated_points_denver_projected";


            var mockPointsTwoOnlyContext = mockContext.SerializeToJson().FromJsonInto<MockTileContext>();
            mockPointsTwoOnlyContext.Identifier = "populated_points_two_projected";

            serialContexts = new ConcurrentDictionary<string, string>();
            serialContexts.TryAdd("base", mockContext.SerializeToJson());
            serialContexts.TryAdd("simple_points", mockPointsContext.SerializeToJson());
            serialContexts.TryAdd("simple_points_denver", mockPointsDenverOnlyContext.SerializeToJson());
            serialContexts.TryAdd("simple_points_two", mockPointsTwoOnlyContext.SerializeToJson());

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
