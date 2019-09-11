using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Messaging;

namespace Universal.Tests.Messaging
{
    public class MockCommandData : ICommandData
    {
        public string DataType { get; set; }

        public object Data { get; set; }

        public double Version { get { return 1.0; } }

        public T DataAs<T>()
        {
            if (Data == null)
                return default(T);

            if (typeof(T) == Type.GetType(DataType))
                return (T)Data;

            throw new InvalidCastException($"The type T:{typeof(T)} is not compatible with the DataType:{DataType}");
        }
    }
}
