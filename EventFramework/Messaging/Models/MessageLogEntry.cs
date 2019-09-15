using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Logging;

namespace Messaging.Models
{
    public class MessageLogEntry : ILogEntry
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public double Id { get; set; }

        public string MessageBody { get; set; }
    }
}
