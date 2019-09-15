using AdminManagementApp.Data;
using AdminManagementApp.Models;
using Messaging;
using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Services
{
    public class MessagingService
    {
        #region Fields

        private readonly ITopicMessengerClient topicMessenger;
        private readonly ITopicObserverClient topicObserver;
        private readonly MessageRepository repository;
        private readonly IQueueMessengerClient commandMessenger;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public MessagingService(MessageRepository repository,
            IQueueMessengerClient commandMessenger,
            ITopicMessengerClient topicMessenger,
            ITopicObserverClient topicObserver)
        {
            this.repository = repository;
            this.commandMessenger = commandMessenger;
            
            this.topicMessenger = topicMessenger;
            this.topicObserver = topicObserver;
            this.topicObserver.RegisterForNotificationOf<GeneralMessage>(MessageReceiver);

        }

        public IEnumerable<IMessage> CurrentMessages()
        {
            return repository.GetAll();
        }

        public IMessage GetMessage(Guid id)
        {
            return repository.Get(id);
        }

        public async Task<bool> Generate(JobRequest request)
        {
            IMessage message = null;
            switch (request.JobType)
            {
                case "projectData":
                    {
                        message = new GeneralCommand
                        {
                            Command = request.JobType,
                            CommandDataCollection = new List<ICommandData>
                            {
                                new CommandData { Data = request.FileName, DataType = "filename" }
                            },
                            Id = Guid.NewGuid()
                        };
                        break;
                    }
                case "generateTiles":
                    {
                        message = new GeneralCommand
                        {
                            Command = request.JobType,
                            CommandDataCollection = new List<ICommandData>
                            {
                                new CommandData { Data = request.FileName, DataType = "filename" }
                            },
                            Id = Guid.NewGuid()
                        };
                        break;
                    }
                default:
                    break;
            }

            if (message != null)
            {
                await commandMessenger.Send(message);
                return true;
            }
            return false;
        }

        public Task MessageReceiver(IMessage message)
        {
            if (message is GeneralMessage gMessage)
            {
                var existingMessage = this.repository.Get(gMessage.Id);
                if (existingMessage == null)
                {
                    this.repository.Add(gMessage);
                }
            }
            
            return Task.FromResult(true);
        }

        #endregion
    }
}
