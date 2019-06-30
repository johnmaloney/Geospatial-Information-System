using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Commands
{
    public class EncodingFactory
    {
        #region Fields


        #endregion

        #region Properties



        #endregion

        #region Methods

        public EncodingFactory()
        {
        }

        public uint[] BuildEncodedGeometry(ITransformedFeature feature)
        {
            var singleFeatureCommands = new List<CommandData>();

            // The first item is a specific type //
            var firstCoordinate = feature.Coordinates[0];

            // This is the typical opening command so it only has a Count of 1 //
            var firstCmd = new CommandData(CommandType.MoveTo, 1,
                new ParameterData(firstCoordinate.X),
                new ParameterData(firstCoordinate.Y));
            singleFeatureCommands.Add(firstCmd);

            // store the parameters based on the first and last not in the count //
            var parameters = new List<ParameterData>();
            // iterate the middle of the collection this is the lineTo Commands //
            for (int j = 1; j < feature.Coordinates.Length - 1; j++)
            {
                (int X, int Y) coordinate = feature.Coordinates[j];
                // all other cases get put into a default //
                parameters.Add(new ParameterData(coordinate.X));
                parameters.Add(new ParameterData(coordinate.Y));
            }

            var midCmd = new CommandData(CommandType.LineTo, parameters.Count / 2, parameters.ToArray());
            singleFeatureCommands.Add(midCmd);

            var lastFeature = feature.Coordinates[feature.Coordinates.Length - 1];
            var closeCmd = new CommandData(CommandType.ClosePath, 1);
            singleFeatureCommands.Add(closeCmd);

            var countOfValues = singleFeatureCommands.Sum(c => c.EncodedValue.Length);
            var featureEncoded = new uint[countOfValues];
            int featureCursor = 0;
            foreach (var encodedCmd in singleFeatureCommands)
            {
                for (int k = 0; k < encodedCmd.EncodedValue.Length; k++)
                {
                    featureEncoded[featureCursor] = (uint)encodedCmd.EncodedValue[k];
                    ++featureCursor;
                }
            }
            return featureEncoded;
        }
        
        #endregion
    }
}
