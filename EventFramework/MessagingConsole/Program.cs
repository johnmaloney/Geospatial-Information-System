using Messaging;
using Messaging.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace MessagingConsole
{
    class Program
    {

        const string publisherConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        const string subscriberConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Subscriber;SharedAccessKey=AADc5dQr/zv+4s6lbDlaKrdDMq6h38VBKksFHOBPWZY=";
        private static IQueueObserverClient qObserver;
        private static IQueueMessengerClient qMessenger;
        private static ITopicObserverClient tObserver;
        private static ITopicMessengerClient tMessenger;
        private static MessageReceiver receiver;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter commands:");

            string line;
            Console.WriteLine("Enter one or more lines of text (press CTRL+Z to exit):");
            Console.WriteLine();
            do
            {
                line = Console.ReadLine();
                if (line != null)
                {
                    switch (line.ToLower())
                    {
                        case "qoff":
                            {
                                qObserver.Close();
                                break;
                            }
                        case "q":
                            {                                
                                var serviceBus = new QueueClient(subscriberConnectionString, Queues.GeneralCommand);                                
                                qObserver = new ObserverClient(serviceBus);

                                break;
                            }
                        case "s":
                            {
                                var serviceBus = new QueueClient(publisherConnectionString, Queues.GeneralCommand);
                                qMessenger = new MessengerClient(serviceBus);
                                Task.Factory.StartNew(async () =>
                                {
                                    await qMessenger.Send(new GeneralCommand
                                    {
                                        Command = "Cosole Test Command",
                                        Id = Guid.NewGuid(),
                                        CommandDataCollection = new List<CommandData>()
                                        {
                                            new CommandData { Data = true, DataType = typeof(bool).ToString() }
                                        }
                                    });
                                });                                

                                break;
                            }
                        case "t":
                            {
                                var topicClient = new TopicClient(publisherConnectionString, "general-info");
                                tMessenger = new MessengerClient(topicClient);
                                Task.Factory.StartNew(async () =>
                                {
                                    await tMessenger.Send(new TopicMessage
                                    {
                                        Id = Guid.NewGuid(),
                                        Message = "Topic Message from a console application"
                                    });
                                });

                                //Task.Factory.StartNew(async () =>
                                //{
                                //    var sender = new MessageSender(publisherConnectionString, "general-info");
                                //    await sender.SendAsync(new Message { MessageId = Guid.NewGuid().ToString() });
                                //    await sender.CloseAsync();
                                //});

                                break;
                            }
                        case "to":
                            {
                                //receiver = new MessageReceiver(subscriberConnectionString, "general-info");
                                //receiver.RegisterMessageHandler((m, c) =>
                                //{
                                //    Console.WriteLine($"Received Topic with id : {m.CorrelationId}");
                                //    return Task.FromResult(true);
                                //},
                                //new MessageHandlerOptions((ex)=> { Console.WriteLine(ex.Exception.Message); return Task.FromResult(true); })
                                //{
                                //    MaxConcurrentCalls = 2,
                                //    AutoComplete = false
                                //});

                                var topicObserver = new SubscriptionClient(subscriberConnectionString, "general-info", "gis");
                                tObserver = new ObserverClient(topicObserver);

                                tObserver.RegisterForNotificationOf<TopicMessage>(async (m) =>
                                {
                                    if (m is TopicMessage topicMessage)
                                    {
                                        Console.WriteLine(topicMessage.Message);
                                    }
                                    await Task.FromResult(true);
                                });
                                break;
                            }
                        case "peek":
                            {
                                Task.Factory.StartNew(async () =>
                                {
                                    var receiver = new MessageReceiver(subscriberConnectionString, "general-info");
                                    bool hasMessages = true;
                                    while (hasMessages)
                                    {
                                        Message message = await receiver.PeekAsync();
                                        hasMessages = message != null;
                                        if (hasMessages)
                                            Console.WriteLine(message.CorrelationId);
                                    }
                                    await receiver.CloseAsync();
                                });
                                
                                break;
                            }
                        default:
                            break;
                    }
                }
            } while (line != null);

            

        }

    }
}
