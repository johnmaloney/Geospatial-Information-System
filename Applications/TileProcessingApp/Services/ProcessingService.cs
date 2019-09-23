using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileProcessingApp.Observers;
using Universal.Contracts.Messaging;
using Universal.Contracts.Logging;
using TileProcessingApp.Models;

namespace TileProcessingApp.Services
{
    public class ProcessingService
    {
        #region Fields
        
        private readonly IQueueObserverClient queueObserver;
        private readonly IQueueMessengerClient queueMesssenger;
        private readonly ITopicObserverClient topicObserver;
        private readonly ITopicMessengerClient messenger;
        private readonly ILogger logger;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public ProcessingService(
            IQueueObserverClient queueObserver,
            IQueueMessengerClient queueMesssenger,
            ITopicObserverClient topicObserver,
            ITopicMessengerClient TopicMessenger, 
            ILogger logger)
        {
            this.queueObserver = queueObserver;
            this.queueMesssenger = queueMesssenger;
            this.topicObserver = topicObserver;
            this.messenger = TopicMessenger;
            this.logger = logger;
        }

        public void RegisterNotificationHandlers(MessageRepository messages)
        {
            var executor = new ProjectedDataObserver(messenger, messages);
            queueObserver.RegisterForNotificationOf<GeneralCommand>(executor.CommandReceiver);
        }
        
        #endregion
    }
}
