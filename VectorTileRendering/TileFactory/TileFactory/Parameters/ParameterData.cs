using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("TileFactory.Tests")]

namespace TileFactory
{
    /// <summary>
    /// ParameterInteger is an encoded positive or negative integer, the main benefit is that 
    /// the stored int is an unsigned number but can be decoded into a signed integer value.
    /// https://github.com/mapbox/vector-tile-spec/tree/master/2.1#432-parameter-integers
    /// </summary>
    public struct ParameterData
    {
        #region Properties

        public int Value { get; private set; }

        public int EncodedValue { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Use for the decoding of a stored / encoded value 
        /// </summary>
        /// <param name="encodedValue"></param>
        public ParameterData(uint encodedValue)
        {
            Value = (int)((encodedValue >> 1) ^ (-(encodedValue & 1)));
            EncodedValue = (int)encodedValue;
        }

        /// <summary>
        /// Use for the encoding of a raw value into a storage/encoded value
        /// </summary>
        /// <param name="rawValue"></param>
        public ParameterData(int rawValue)
        {
            Value = rawValue;
            EncodedValue = (rawValue << 1) ^ (rawValue >> 31);
        }

        #endregion
    }
}
