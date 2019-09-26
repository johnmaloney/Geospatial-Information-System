using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileProcessingApp.Observers;
using Universal.Contracts.Messaging;
using Universal.Contracts.Logging;
using TileProcessingApp.Models;
using Universal.Contracts.Files;
using TileFactory;
using Universal.Contracts.Layers;

namespace TileProcessingApp.Services
{
    public class ProcessingService
    {
        #region Fields
        
        private readonly IQueueObserverClient queueObserver;
        private readonly IQueueMessengerClient queueMesssenger;
        private readonly ITopicObserverClient topicObserver;
        private readonly ITopicMessengerClient messenger;
        private readonly TileRetrieverService tileRetriever;
        private readonly ILayerInitializationService layerService;
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
            TileRetrieverService tileRetriever,
            ILayerInitializationService layerService,
            ILogger logger)
        {
            this.queueObserver = queueObserver;
            this.queueMesssenger = queueMesssenger;
            this.topicObserver = topicObserver;
            this.messenger = TopicMessenger;
            this.tileRetriever = tileRetriever;
            this.layerService = layerService;
            this.logger = logger;
        }

        public void RegisterNotificationHandlers(MessageRepository messages, IFileRepository fileRepository)
        {
            var executor = new ProjectedDataObserver(messenger, messages, fileRepository, tileRetriever, layerService, logger);
            queueObserver.RegisterForNotificationOf<GeneralCommand>(executor.CommandReceiver);
        }
        
        #endregion
    }
}
