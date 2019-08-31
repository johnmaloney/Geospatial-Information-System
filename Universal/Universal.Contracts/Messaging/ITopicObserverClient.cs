using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Messaging
{
    public interface ITopicObserverClient
    {
        void RegisterForNotificationOf<TMessage>(Func<IMessage, Task> messageHandler);
    }
}
