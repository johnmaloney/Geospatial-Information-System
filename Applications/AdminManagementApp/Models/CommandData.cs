using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Models
{
    public class CommandData : ICommandData
    {
        public string DataType { get; set; }

        public object Data { get; set; }

        public double Version { get { return 1.1; } }

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
