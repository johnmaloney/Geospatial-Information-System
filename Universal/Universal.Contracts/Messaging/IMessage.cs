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
        int Version { get; }
    }
}
