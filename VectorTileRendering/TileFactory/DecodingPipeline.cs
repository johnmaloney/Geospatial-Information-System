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

            // Commands are structured to contain the Type and instances of that CommandType i.e. MoveTo //
            // as dictated from the CommandData's CommandTotal value e.g. CommandTotal = 100 neans //
            // the command is to occur 100 times before the next group of commands is defined.
            // The first data value will generate a CommandData object, which will fill in the //
            // parameters of that individual command until a new command type is discovered. //
            do
            {
                var currentCommand = new CommandData(this.commandDataSource[structureIndex]);
                var processedCommand = ProcessCommandsParameters(currentCommand);
                parsedItems.Add(processedCommand);

            } while (structureIndex < commandDataSource.Length);

            return parsedItems;
        }

        private CommandData ProcessCommandsParameters(CommandData command)
        {
            Increment();
            for (int p = 0; p < command.ParameterCount; p++)
            {
                var parameter = new ParameterData(commandDataSource[structureIndex]);
                command[p] = parameter;
                Increment();
            }

            return command;
        }

        private int Increment()
        {
            return this.structureIndex += 1;
        }

        #endregion
    }
}
