using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Messaging;

namespace Messaging.Models
{
    public class GeneralCommand : IMessage
    {
        public Guid Id { get; set; }

        public int CorrellationId { get; set; }

        public string Type { get; set; }

        public int Version { get; set; }

        public string Message { get; set; }
    }
}
