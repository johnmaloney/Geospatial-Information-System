using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TileFactory;

namespace TileFactory.Tests
{
    [TestClass]
    /// These tests are a direct representation of the Mapbox Tile specification.
    /// Specification: https://github.com/mapbox/vector-tile-spec/tree/master/2.1#vector-tile-specification
    public class CommandEncodingTests
    {
        [TestMethod]
        [Description("CommandData Encoding")]
        public void given_a_move_to_command_expect_parameter_proper_construction()
        {
            var openCmd = new CommandData(CommandType.MoveTo, 1, new ParameterData(100), new ParameterData(80));
            Assert.AreEqual(openCmd.ParameterCount, 2);
        }

        [TestMethod]
        public void given_moveto_command_expect_encoded_value_to_be_array()
        {
            var openCmd = new CommandData(CommandType.MoveTo, 1, new ParameterData(100), new ParameterData(80));
            var encoded = openCmd.EncodedValue;
            Assert.AreEqual(encoded.Length, 3);
            Assert.AreEqual(encoded[0], 9);
            Assert.AreEqual(encoded[1], 200);
            Assert.AreEqual(encoded[2], 160);
        }

        [TestMethod]
        public void given_lineto_command_expect_encoded_value_to_be_array()
        {
            var openCmd = new CommandData(CommandType.LineTo, 4, 
                new ParameterData(100), new ParameterData(80), 
                new ParameterData(102), new ParameterData(78), 
                new ParameterData(104), new ParameterData(76), 
                new ParameterData(106), new ParameterData(74));
            var encoded = openCmd.EncodedValue;
            Assert.AreEqual(encoded.Length, 9);
            Assert.AreEqual(encoded[0], 34);
            Assert.AreEqual(encoded[1], 200);
            Assert.AreEqual(encoded[2], 160);
            Assert.AreEqual(encoded[3], 204);
            Assert.AreEqual(encoded[4], 156);
            Assert.AreEqual(encoded[5], 208);
            Assert.AreEqual(encoded[6], 152);
            Assert.AreEqual(encoded[7], 212);
            Assert.AreEqual(encoded[8], 148);
        }

        [TestMethod]
        public void given_closepath_command_expect_encoded_value_to_be_array()
        {
            var openCmd = new CommandData(CommandType.ClosePath, 1);
            var encoded = openCmd.EncodedValue;
            Assert.AreEqual(encoded.Length, 1);
            Assert.AreEqual(encoded[0], 15);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void given_moveto_command_invalid_amount_of_parameters_expect_exception()
        {
            var invalid = new CommandData(CommandType.MoveTo, 3, new ParameterData(100), new ParameterData(80), new ParameterData(4));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void given_lineto_command_invalid_amount_of_parameters_expect_exception()
        {
            var invalid = new CommandData(CommandType.LineTo, 3, new ParameterData(100), new ParameterData(80), new ParameterData(4));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void given_close_command_invalid_amount_of_parameters_expect_exception()
        {
            var invalid = new CommandData(CommandType.ClosePath, 3, new ParameterData(100), new ParameterData(80), new ParameterData(4));
        }
    }
}
