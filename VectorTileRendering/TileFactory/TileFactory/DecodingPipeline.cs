using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory
{
    /// <summary>
    /// Should take the encoded value for a 
    /// </summary>
    public class CommandFactory
    {
        #region Fields

        private int structureIndex = 0;
        private uint[] commandDataSource;

        #endregion

        #region Properties
        
        #endregion

        #region Methods


        /// <summary>
        /// Given an array of unsigned integers
        /// </summary>
        /// <param name="data"></param>
        public CommandFactory(uint[] data)
        {
            this.commandDataSource = data;
        }

        public IReadOnlyList<CommandData> Process()
        {
            var parsedItems = new List<CommandData>();

            do
            {
                var processedCommand = ProcessCommandStructure();
                parsedItems.Add(processedCommand);

            } while (structureIndex < commandDataSource.Length);


            return parsedItems;
        }

        private CommandData ProcessCommandStructure()
        {
            var commandData = new CommandData(commandDataSource[structureIndex]);
            Increment();
            for (int p = 0; p < commandData.ParameterCount; p++)
            {
                var parameter = new ParameterData(commandDataSource[structureIndex]);
                commandData[p] = parameter;
                Increment();
            }

            return commandData;
        }

        private int Increment()
        {
            return this.structureIndex += 1;
        }

        #endregion

    }
}
