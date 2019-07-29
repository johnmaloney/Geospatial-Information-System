using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TileFactory.Tests.Utility
{
    [Obsolete("Replaced by the Universal.Contracts.Serial namespace.", false)]
    public static class SerializationExtensions
    {
        /// <summary>
        /// Creates a deep copy of the original.
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="itemToCopy">Object to create a copy off.</param>
        /// <returns>The copy</returns>
        public static T CarbonCopy<T>(this T itemToCopy)
        {
            System.Diagnostics.Stopwatch watch = null;
            try
            {

#if DEBUG
                watch = new System.Diagnostics.Stopwatch();
                watch.Start();
#endif
                    using (var ms = new MemoryStream())
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(ms, itemToCopy);
                        ms.Position = 0;
                        return (T)formatter.Deserialize(ms);
                    }
            }
            finally
            {
#if DEBUG
                if (watch != null)
                {
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine("SerializationExtensions.CarbonCopy " +
                                                       itemToCopy.GetType().Name + " " + watch.ElapsedMilliseconds +
                                                       "ms");
                }
#endif
            }
        }

        /// <summary>
        /// Simplification of string encoding to byte array.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
    
    }
}
