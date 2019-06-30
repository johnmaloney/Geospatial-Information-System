using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TileFactory;
using TileFactory.Commands;

namespace TileFactory.Tests
{
    [TestClass]
    /// These tests are a direct representation of the Mapbox Tile specification.
    /// Specification: https://github.com/mapbox/vector-tile-spec/tree/master/2.1#vector-tile-specification
    public class CommandDecodingTests
    {
        [TestMethod]
        [Description("CommandData initialization test")]
        public void with_generated_number_encode_then_decode_expect_proper_command_and_count()
        {
            // Attempting to replicate this structure in the following CommandData //
            // Open -> Move -> Move -> Move -> Move -> Close //
            // Encoded = [15 34 9]
            // Using this formula (ID & 0x7) | (COUNT << 3)
            // Command = Open then ID = 1 and the repeat count (COUNT) = 1 ==> 9  //
            // Command = Open then ID = 2 and the repeat count (COUNT) = 4 ==> 34 //
            // Command = Open then ID = 7 and the repeat count (COUNT) = 1 ==> 15 //

            var openCmd = new CommandData(9);
            Assert.AreEqual(CommandType.MoveTo, openCmd.Command);
            Assert.AreEqual(1, openCmd.CommandTotal);
        }

        [TestMethod]
        [Description("https://github.com/mapbox/vector-tile-spec/tree/master/2.1#4351-example-point")]
        public void command_with_one_point_decode_expect_proper_parsing()
        {
            //Encoded as: [ 9 50 34 ]
            //  | |  `> Decoded: ((34 >> 1) ^ (-(34 & 1))) = +17
            //  | `> Decoded: ((50 >> 1) ^ (-(50 & 1))) = +25
            //  | ===== relative MoveTo(+25, +17) == create point(25,17)
            //  `> [00001 001] = command id 1 (MoveTo), command count 1

            var commandFactory = new DecodingFactory(new uint[] { 9, 50, 34 });
            var commands = commandFactory.Process();

            Assert.AreEqual(1, commands.Count());
            Assert.AreEqual(CommandType.MoveTo, commands.Single().Command);
            Assert.AreEqual(25, commands.Single()[0].Value);
            Assert.AreEqual(17, commands.Single()[1].Value);
        }

        [TestMethod]
        [Description("https://github.com/mapbox/vector-tile-spec/tree/master/2.1#4352-example-multi-point")]
        public void using_multiple_commands_and_params_expect_full_decoding()
        {
            //Encoded as: [ 17 10 14 3 9 ]
            //   |  |  | | `> Decoded: ((9 >> 1) ^ (-(9 & 1))) = -5
            //   |  |  | `> Decoded: ((3 >> 1) ^ (-(3 & 1))) = -2
            //   |  |  | === relative MoveTo(-2, -5) == create point(3,2)
            //   |  |  `> Decoded: ((34 >> 1) ^ (-(34 & 1))) = +7
            //   |  `> Decoded: ((50 >> 1) ^ (-(50 & 1))) = +5
            //   | ===== relative MoveTo(+5, +7) == create point(5,7)
            //   `> [00010 001] = command id 1 (MoveTo), command count 2
            var commandFactory = new DecodingFactory(new uint[] { 17, 10, 14, 3, 9 });
            var commands = commandFactory.Process();

            Assert.AreEqual(1, commands.Count());
            Assert.AreEqual(CommandType.MoveTo, commands.Single().Command);
            Assert.AreEqual(5, commands.Single()[0].Value);
            Assert.AreEqual(7, commands.Single()[1].Value);
            Assert.AreEqual(-2, commands.Single()[2].Value);
            Assert.AreEqual(-5, commands.Single()[3].Value);
        }

        [TestMethod]
        [Description("https://github.com/mapbox/vector-tile-spec/tree/master/2.1#4353-example-linestring")]
        public void using_two_command_types_in_encoding_expect_multiple_commands_decoded()
        {
            //Encoded as: [ 9 4 4 18 0 16 16 0 ]
            //  |      |      ==== relative LineTo(+8, +0) == Line to Point(10, 10)
            //  |      | ==== relative LineTo(+0, +8) == Line to Point(2, 10)
            //  |      `> [00010 010] = command id 2 (LineTo), command count 2
            //  | === relative MoveTo(+2, +2)
            //  `> [00001 001] = command id 1 (MoveTo), command count 1
            var commandFactory = new DecodingFactory(new uint[] { 9, 4, 4, 18, 0, 16, 16, 0 });
            var commands = commandFactory.Process();

            Assert.AreEqual(2, commands.Count());
            var moveTo = commands.First();
            Assert.AreEqual(CommandType.MoveTo, moveTo.Command);
            Assert.AreEqual(2, moveTo[0].Value);
            Assert.AreEqual(2, moveTo[1].Value);

            // Second Parsed Command //
            var lineTo = commands.Skip(1).First();
            Assert.AreEqual(CommandType.LineTo, lineTo.Command);

            Assert.AreEqual(0, lineTo[0].Value);
            Assert.AreEqual(8, lineTo[1].Value);
            Assert.AreEqual(8, lineTo[2].Value);
            Assert.AreEqual(0, lineTo[3].Value);
        }

        [TestMethod]
        [Description("https://github.com/mapbox/vector-tile-spec/tree/master/2.1#4354-example-multi-linestring")]
        public void using_lineto_with_moveto_inbetween_expect_multiple_commands_decoded()
        {
            //Encoded as: [ 9 4 4 18 0 16 16 0 9 17 17 10 4 8 ]
            //  |      |           |        | === relative LineTo(+2, +4) == Line to Point(3,5)
            //  |      |           |        `> [00001 010] = command id 2 (LineTo), command count 1
            //  |      |           | ===== relative MoveTo(-9, -9) == Start new line at(1,1)
            //  |      |           `> [00001 001] = command id 1 (MoveTo), command count 1
            //  |      |      ==== relative LineTo(+8, +0) == Line to Point(10, 10)
            //  |      | ==== relative LineTo(+0, +8) == Line to Point(2, 10)
            //  |      `> [00010 010] = command id 2 (LineTo), command count 2
            //  | === relative MoveTo(+2, +2)
            //  `> [00001 001] = command id 1 (MoveTo), command count 1
            var commandFactory = new DecodingFactory(new uint[] { 9, 4, 4, 18, 0, 16, 16, 0, 9, 17, 17, 10, 4, 8 });
            var commands = commandFactory.Process();

            Assert.AreEqual(4, commands.Count());
            var moveTo = commands.First();
            Assert.AreEqual(CommandType.MoveTo, moveTo.Command);
            Assert.AreEqual(2, moveTo[0].Value);
            Assert.AreEqual(2, moveTo[1].Value);

            // Second Parsed Command //
            var lineTo = commands.Skip(1).First();
            Assert.AreEqual(CommandType.LineTo, lineTo.Command);
            Assert.AreEqual(0, lineTo[0].Value);
            Assert.AreEqual(8, lineTo[1].Value);
            Assert.AreEqual(8, lineTo[2].Value);
            Assert.AreEqual(0, lineTo[3].Value);

            var moveToB = commands.Skip(2).First();
            Assert.AreEqual(CommandType.MoveTo, moveTo.Command);
            Assert.AreEqual(-9, moveToB[0].Value);
            Assert.AreEqual(-9, moveToB[1].Value);

            // Second Parsed Command //
            var lineToB = commands.Skip(3).First();
            Assert.AreEqual(CommandType.LineTo, lineTo.Command);
            Assert.AreEqual(2, lineToB[0].Value);
            Assert.AreEqual(4, lineToB[1].Value);
        }

        [TestMethod]
        [Description("https://github.com/mapbox/vector-tile-spec/tree/master/2.1#4351-example-point")]
        public void given_a_simple_polygon_expect_proper_decoding()
        {
            //Encoded as: [ 9 6 12 18 10 12 24 44 15 ]
            //  |       |              `> [00001 111] command id 7 (ClosePath), command count 1
            //  |       |       ===== relative LineTo(+12, +22) == Line to Point(20, 34)
            //  |       | ===== relative LineTo(+5, +6) == Line to Point(8, 12)
            //  |       `> [00010 010] = command id 2 (LineTo), command count 2
            //  | ==== relative MoveTo(+3, +6)
            //  `> [00001 001] = command id 1 (MoveTo), command count 1

            var commandFactory = new DecodingFactory(new uint[] { 9, 6, 12, 18, 10, 12, 24, 44, 15 });
            var commands = commandFactory.Process();

            Assert.AreEqual(3, commands.Count());
            var first = commands.First();
            var second = commands.Skip(1).First();
            var third = commands.Skip(2).First();

            Assert.AreEqual(CommandType.MoveTo, first.Command);
            Assert.AreEqual(CommandType.LineTo, second.Command);
            Assert.AreEqual(CommandType.ClosePath, third.Command);

            Assert.AreEqual(2, first.ParameterCount);
            Assert.AreEqual(4, second.ParameterCount);
            Assert.AreEqual(0, third.ParameterCount);
            
            Assert.AreEqual(3, first[0].Value);
            Assert.AreEqual(6, first[1].Value);

            Assert.AreEqual(5, second[0].Value);
            Assert.AreEqual(6, second[1].Value);
            Assert.AreEqual(12, second[2].Value);
            Assert.AreEqual(22, second[3].Value);

            // Throws an Exception due to the close command having 0 parameters //
            //Assert.AreEqual(17, third[0].Value);
        }

        [TestMethod]
        [Description("https://github.com/mapbox/vector-tile-spec/tree/master/2.1#4356-example-multi-polygon")]
        public void given_a_complex_multipolygon_expect_proper_decoding()
        {
            //Encoded as: [ 9 0 0 26 20 0 0 20 19 0 15 9 22 2 26 18 0 0 18 17 0 15 9 4 13 26 0 8 8 0 0 7 15 ]
            //  |      |                |  |       |                |  |       |              `> [00001 111] (ClosePath)
            //  |      |                |  |       |                |  |       `> [00011 010] = (LineTo), command count 3
            //  |      |                |  |       |                |  `> [00001 001] = command id 1 (MoveTo), command count 1
            //  |      |                |  |       |                `> [00001 111] (ClosePath)
            //  |      |                |  |       `> [00011 010] = (LineTo), command count 3
            //  |      |                | `> [00001 001] = command id 1 (MoveTo), command count 1
            //  |      |                `> [00001 111] (ClosePath)
            //  |      `> [00011 010] = (LineTo), command count 3
            //  `> [00001 001] = command id 1 (MoveTo), command count 1

            var commandFactory = new DecodingFactory(new uint[] { 9,0,0,26,20,0,0,20,19,0,15,9,22,2,26,18,0,0,18,17,0,15,9,4,13,26,0,8,8,0,0,7,15 });
            var commands = commandFactory.Process();

            Assert.AreEqual(9, commands.Count());
            var first = commands.First();
            var second = commands.Skip(1).First();
            var third = commands.Skip(2).First();
            var fourth = commands.Skip(3).First();
            var fifth = commands.Skip(4).First();
            var sixth = commands.Skip(5).First();
            var seventh = commands.Skip(6).First();
            var eighth = commands.Skip(7).First();
            var ninth = commands.Skip(8).First();

            Assert.AreEqual(CommandType.MoveTo, first.Command);
            Assert.AreEqual(CommandType.LineTo, second.Command);
            Assert.AreEqual(CommandType.ClosePath, third.Command);
            Assert.AreEqual(CommandType.MoveTo, fourth.Command);
            Assert.AreEqual(CommandType.LineTo, fifth.Command);
            Assert.AreEqual(CommandType.ClosePath, sixth.Command);
            Assert.AreEqual(CommandType.MoveTo, seventh.Command);
            Assert.AreEqual(CommandType.LineTo, eighth.Command);
            Assert.AreEqual(CommandType.ClosePath, ninth.Command);

            Assert.AreEqual(2, first.ParameterCount);
            Assert.AreEqual(6, second.ParameterCount);
            Assert.AreEqual(0, third.ParameterCount);
            Assert.AreEqual(2, fourth.ParameterCount);
            Assert.AreEqual(6, fifth.ParameterCount);
            Assert.AreEqual(0, sixth.ParameterCount);
            Assert.AreEqual(2, seventh.ParameterCount);
            Assert.AreEqual(6, eighth.ParameterCount);
            Assert.AreEqual(0, ninth.ParameterCount);
        }
    }
}
