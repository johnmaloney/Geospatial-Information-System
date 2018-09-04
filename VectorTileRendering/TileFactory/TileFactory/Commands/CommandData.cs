using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("TileFactory.Tests.dll")]

namespace TileFactory
{
    /// <summary>
    /// CommandData is the representation of an encoded value within an int32. 
    /// The values of <see cref="Command"/> (e.g. Open, Move, Close) and the repeating amount / count of that 
    /// Command are encoded using this process:
    /// CommandData = (id & 0x7) | (count &lt;&lt; 3) 
    /// Commands are decoded using this process:
    /// CommandData = ((encodedCommand >> 1) ^ (-(encodedCommand & 1)))
    /// </summary>
    public struct CommandData
    {
        #region Fields

        /// <summary>
        /// Specification 4.3.2 Parameter Integers, the value stored per CommandType
        /// is the amount of required parameters per command.
        /// </summary>
        private static Dictionary<CommandType, int> parametersPerCommand = new Dictionary<CommandType, int>
        {
            { CommandType.MoveTo, 2 },
            { CommandType.LineTo, 2 },
            { CommandType.ClosePath, 0 }
        };

        private readonly ParameterData[] parameterData;

        #endregion

        #region Properties

        public CommandType Command { get; set; }

        public int Count { get; private set; }

        /// <summary>
        /// 4.3.2. Parameter Integers
        /// Commands requiring parameters are followed by a ParameterInteger for each parameter required by 
        /// that command.The number of ParameterIntegers that follow a CommandInteger is equal to the parameter 
        /// count of a command multiplied by the command count of the CommandInteger. For example, 
        /// a CommandInteger with a MoveTo command with a command count of 3 will be followed 
        /// by 6 ParameterIntegers.
        /// </summary>
        public int ParameterCount { get { return parametersPerCommand[Command] * this.Count; } }

        public ParameterData this[int indexer]
        {
            get
            { return parameterData[indexer]; }
            set
            {
                parameterData[indexer] = value;
            }
        }

        public int EncodedValue { get { return ((int)Command & 0x7) | (Count << 3); } }

        #endregion

        #region Methods

        /// <summary>
        /// Use for the decoding of a stored Command
        /// </summary>
        /// <param name="encodedCommand"></param>
        public CommandData(uint encodedCommand)
        {
            Command = (CommandType)(encodedCommand & 0x7);
            Count = (int)encodedCommand >> 3;

            parameterData = new ParameterData[parametersPerCommand[Command] * this.Count];
        }

        /// <summary>
        /// Use for the creation of an encoded Command from a raw value
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterCount"></param>
        public CommandData(CommandType command, int commandCount = 0)
        {
            Command = command;
            Count = commandCount;

            parameterData = new ParameterData[parametersPerCommand[Command] * this.Count];
        }

        #endregion
    }
}
