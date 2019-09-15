using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Messaging;

namespace Messaging.Models
{
    public class TopicMessage : IMessage
    {
        public Guid Id { get; set; }

        public int CorrellationId { get; set; }

        public string Type { get { return typeof(TopicMessage).AssemblyQualifiedName; } }

        public double Version { get { return 1.0; } }

        public string Message { get; set; }
    }
}
