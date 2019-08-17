using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Messaging
{
    public interface IQueueMessengerClient
    {
        Task Send(IMessage message);
    }
}
