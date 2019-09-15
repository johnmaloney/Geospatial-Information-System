using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileProcessingApp.Observers;
using Universal.Contracts.Messaging;

namespace TileProcessingApp.Services
{
    public class ProcessingService
    {
        #region Fields
        
        private readonly IQueueObserverClient queueObserver;
        private readonly IQueueMessengerClient queueMesssenger;
        private readonly ITopicObserverClient topicObserver;
        private readonly ITopicMessengerClient messenger;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public ProcessingService(
            IQueueObserverClient queueObserver,
            IQueueMessengerClient queueMesssenger,
            ITopicObserverClient topicObserver,
            ITopicMessengerClient TopicMessenger)
        {
            this.queueObserver = queueObserver;
            this.queueMesssenger = queueMesssenger;
            this.topicObserver = topicObserver;
            this.messenger = TopicMessenger;
        }

        public void RegisterNotificationHandlers()
        {
            queueObserver.RegisterForNotificationOf<GeneralCommand>(new ProjectDataObserver().CommandReceiver);
        }

        #endregion
    }
}
