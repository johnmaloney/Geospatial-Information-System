using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Messaging
{
    public interface IMessage
    {
        Guid Id { get; }
        int CorrellationId { get; }
        string Type { get; }
        double Version { get; }
    }

    public interface ICommandData
    { 
        string DataType { get; }
        object Data { get; }
        double Version { get; }  
        T DataAs<T>();
    }
}
