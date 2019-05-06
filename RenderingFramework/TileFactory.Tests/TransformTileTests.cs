using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Models;
using TileFactory.Tests.Utility;
using Microsoft.Extensions.DependencyInjection;
using TileFactory.Utility;

namespace TileFactory.Tests
{
    [TestClass]
    public class TransformTileTests : ATest
    {
        [TestMethod]
        public void given_projd_tile_transform_expect_converted_points()
        {
            var tile = Container.GetService<IConfigurationStrategy>().Into<DynamicTile>("colorado_outline_tile");

            var transform = new Transform(4096, 64);
            transform.ProcessTile(tile);
        }
    }
}
