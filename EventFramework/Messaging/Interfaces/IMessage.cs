using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Interfaces
{
    public interface IMessage
    {
        int CorrellationId { get; }
    }
}
