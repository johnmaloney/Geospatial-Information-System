using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TileFactory;
using TileFactory.Interfaces;
using TileFactory.Tests.Mocks;
using TileFactory.Tests.Utility;
using TileFactory.Transforms;

namespace TileFactory.Tests
{
    [TestClass]
    /// These tests are a direct representation of the Mapbox Tile specification.
    /// Specification: https://github.com/mapbox/vector-tile-spec/tree/master/2.1#vector-tile-specification
    public class TileEncodingTests : ATest
    {
        [TestMethod]
        [Description("")]
        public void using_the_transformed_tile_generate_commands_expect_encoded_values()
        {
            // the goal of this test is to take a TransformedTile, which is the coordinates of a geometry feature //
            // and turn it into a group of Commands with parameters. //
            var tile = Container.GetService<IConfigurationStrategy>().Into<TransformedTile>("transformed_tile");

            for (int i = 0; i <= tile.TransformedFeatures.Count; i++)
            {
                var commands = new List<CommandData>();
                var features = tile.TransformedFeatures[i];

                // The first item is a specific type //
                var firstFeature = features.Coordinates[0];
                var firstCmd = new CommandData(CommandType.MoveTo, 1);
                firstCmd[0] = new ParameterData(firstFeature.X);
                firstCmd[1] = new ParameterData(firstFeature.Y);
                commands.Add(firstCmd);

                // store the parameters based on the first and last not in the count //
                var parameters = new List<ParameterData>();
                // iterate the middle of the collection this is the lineTo Commands //
                for (int j = 1; j < features.Coordinates.Length -1; j++)
                {
                    (int X, int Y) feature = features.Coordinates[j];
                    // all other cases get put into a default //
                    parameters.Add(new ParameterData(feature.X));
                    parameters.Add(new ParameterData(feature.Y));
                }

                var midCmd = new CommandData(CommandType.LineTo, parameters.Count /2);
                for (int k = 0; k < parameters.Count; k++)
                {
                    midCmd[k] = parameters[k];
                }
                commands.Add(midCmd);

                var lastFeature = features.Coordinates[features.Coordinates.Length-1];
                var closeCmd = new CommandData(CommandType.ClosePath, 1);
                commands.Add(closeCmd);
            }
        }       
    }
}
