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
        [Description("Encoding the Geometry of a Tile")]
        public void using_the_transformed_tile_generate_the_encoded_collection_of_data()
        {
            // the goal of this test is to take a TransformedTile, which is the coordinates of a geometry feature //
            // and turn it into a group of Commands with parameters. //
            var tile = Container.GetService<IConfigurationStrategy>().Into<TransformedTile>("transformed_tile");

            for (int i = 0; i < tile.TransformedFeatures.Count; i++)
            {
                var singleFeatureCommands = new List<CommandData>();
                var features = tile.TransformedFeatures[i];

                // The first item is a specific type //
                var firstFeature = features.Coordinates[0];

                // This is the typical opening command so it only has a Count of 1 //
                var firstCmd = new CommandData(CommandType.MoveTo, 1, 
                    new ParameterData(firstFeature.X),
                    new ParameterData(firstFeature.Y));
                singleFeatureCommands.Add(firstCmd);

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

                var midCmd = new CommandData(CommandType.LineTo, parameters.Count /2, parameters.ToArray());
                singleFeatureCommands.Add(midCmd);

                var lastFeature = features.Coordinates[features.Coordinates.Length-1];
                var closeCmd = new CommandData(CommandType.ClosePath, 1);
                singleFeatureCommands.Add(closeCmd);

                var countOfValues = singleFeatureCommands.Sum(c => c.EncodedValue.Length);
                var featureEncoded = new int[countOfValues];
                int featureCursor = 0;
                foreach(var encodedCmd in singleFeatureCommands)
                {
                    for (int k = 0; k < encodedCmd.EncodedValue.Length; k++)
                    {
                        featureEncoded[featureCursor] = encodedCmd.EncodedValue[k];
                       ++featureCursor;
                    }
                }
                Assert.IsTrue(featureEncoded.All(f => f > 0));
            }            
        }       
    }
}
