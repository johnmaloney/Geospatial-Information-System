using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Messaging
{
    public interface ITopicMessengerClient
    {
        Task Send(IMessage message);
    }
}
