using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TileFactory;

namespace TileFactory.Tests
{
    [TestClass]
    public class CommandEncodingTests
    {
        [TestMethod]
        public void with_generated_number_encoce_then_decode_expect_proper_command_and_count()
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
            Assert.AreEqual(1, openCmd.Count);
        }

        [TestMethod]
        public void command_with_one_point_expect_proper_parsing()
        {
            //Encoded as: [ 9 50 34 ]
            //  | |  `> Decoded: ((34 >> 1) ^ (-(34 & 1))) = +17
            //  | `> Decoded: ((50 >> 1) ^ (-(50 & 1))) = +25
            //  | ===== relative MoveTo(+25, +17) == create point(25,17)
            //  `> [00001 001] = command id 1 (MoveTo), command count 1

            var commandFactory = new CommandFactory(new uint[] { 9, 50, 34 });
            var commands = commandFactory.Process();

            Assert.AreEqual(1, commands.Count());
            Assert.AreEqual(CommandType.MoveTo, commands.First().Command);
            Assert.AreEqual(25, commands.First()[0].Value);
            Assert.AreEqual(17, commands.First()[1].Value);
        }

        [TestMethod]
        public void using_array_of_commands_and_params_expect_full_decoding()
        {
        }
    }
}
