using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace TileProcessingApp.Observers
{
    public class ProjectedDataObserver
    {
        #region Fields

        private readonly ITopicMessengerClient messenger;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ProjectedDataObserver(ITopicMessengerClient messenger)
        {
            this.messenger = messenger;
        }

        internal async Task CommandReceiver(IMessage message)
        {
            if (message is GeneralCommand gCommand)
            {
                // This will be the function that receives a command from the event framework //
                // and processes the command into a valid data conversion //
                await messenger.Send(new TopicMessage { Message = $"Tile Processing request started for {gCommand.Command}, for ID:{gCommand.Id.ToString()}" });
            }
        }

        #endregion
    }
}
