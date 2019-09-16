using Messaging;
using Messaging.Models;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using Universal.Contracts.Messaging;

namespace MessagingConsole
{
    class Program
    {

        const string publisherConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Publisher;SharedAccessKey=knJ9TZyB9kf8kdv/cCcTW4b9/sPCTP5tcX2G9zU1QUE=";
        const string subscriberConnectionString = "Endpoint=sb://aetosmessaging.servicebus.windows.net/;SharedAccessKeyName=Subscriber;SharedAccessKey=AADc5dQr/zv+4s6lbDlaKrdDMq6h38VBKksFHOBPWZY=";
        private static IQueueObserverClient qObserver;
        private static IQueueMessengerClient qMessenger;

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
                                qMessenger.Send(new GeneralCommand
                                {
                                        Command = "Cosole Test Command",
                                        Id = Guid.NewGuid(),
                                        CommandDataCollection = new List<CommandData>()
                                        {
                                            new CommandData { Data = true, DataType = typeof(bool).ToString()}
                                        }
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
