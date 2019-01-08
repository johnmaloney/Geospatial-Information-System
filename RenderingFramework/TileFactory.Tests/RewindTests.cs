using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;
using TileFactory.Utility;
using TileFactory.Utility.Obsolete;

namespace TileFactory.Tests
{
    [TestClass]
    public class RewindTests
    {
        [TestMethod]
        public void process_array_through_rewinder_expect_SOMETHING_UNKNOWN()
        {
            var feature = new Feature(GeometryType.LineString)
            {
                Geometry = new (double X, double Y, double Z)[][]
                {
                    new(double X, double Y, double Z)[]
                    {
                        (X:0.20502623888888888d, Y:0.3749348317775085d, Z:1d),
                        (X:0.21652329444444446d, Y:0.3749172753574649d, Z:0.00008042533215241402d),
                        (X:0.21654933333333332d, Y:0.38925412744493454d, Z:0.00033782393398675166d),
                        (X:0.1970966027777778d, Y:0.38923325611415305d, Z:0.00022927873054644834d),
                        (X:0.19708312222222218d, Y:0.37492359875955333d, Z:0.000048328257273705895d),
                        (X:0.20502623888888888d, Y:0.3749348317775085d, Z:1d)
                    }
                }
            };

            var rewinder = new Rewind(feature, true);
            Assert.AreEqual(0d, rewinder.Area);
        }
    }
} 