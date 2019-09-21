using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Universal.Contracts.Messaging
{
    public interface IQueueObserverClient
    {
        void RegisterForNotificationOf<TMessage>(Func<IMessage, Task> messageHandler);
        Task Close();
    }
}
