using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileProcessingApp.Models;
using Universal.Contracts.Messaging;

namespace TileProcessingApp.Observers
{
    public class ProjectedDataObserver
    {
        #region Fields

        private readonly ITopicMessengerClient messenger;
        private readonly MessageRepository messageRepository;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ProjectedDataObserver(ITopicMessengerClient messenger, MessageRepository messageRepository)
        {
            this.messenger = messenger;
            this.messageRepository = messageRepository;
        }

        internal async Task CommandReceiver(IMessage message)
        {
            if (message is GeneralCommand gCommand)
            {
                // This will be the function that receives a command from the event framework //
                // and processes the command into a valid data conversion //
                var topic = new TopicMessage { Message = $"Tile Processing request started for {gCommand.Command}, for ID:{gCommand.Id.ToString()}" };
                messageRepository.AddMessage(topic);
                await messenger.Send(topic);
            }
        }

        #endregion
    }
}
