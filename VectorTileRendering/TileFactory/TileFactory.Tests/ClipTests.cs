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
            IClipContext clip = new ClipContext(Constants.ClipType.Left, 64);
            //Assert.AreEqual()
        }
    }
}
