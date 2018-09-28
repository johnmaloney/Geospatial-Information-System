using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Utility;

namespace TileFactory.Tests
{
    [TestClass]
    public class ClipTests
    {
        [TestMethod]
        public void generate_clip_expect_proper_type_discernment()
        {
            IClipContext clip = new ClipContext(Constants.ClipType.Left, 64d/4096d);
            Assert.AreEqual(0, clip.Axis);
            Assert.AreEqual(-1.015625d, clip.K1);
            Assert.AreEqual(0.015625d, clip.K2);
            Assert.AreEqual(-1, clip.MinAll);
            Assert.AreEqual(2, clip.MaxAll);
            Assert.AreEqual(1, clip.Scale);
            Assert.AreEqual(Constants.ClipType.Left, clip.Type);
        }
    }
}
