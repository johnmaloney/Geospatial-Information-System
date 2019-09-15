using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Messaging;

namespace Universal.Tests.Messaging
{
    public class GeneralMessage : IMessage
    {
        public Guid Id { get; set; }

        public int CorrellationId { get; set; }

        public string Type { get { return typeof(GeneralMessage).AssemblyQualifiedName; } }

        public double Version { get; set; }
    }

    public class SpecificMessage : IMessage
    {
        public Guid Id { get; set; }

        public int CorrellationId { get; set; }

        public string Type { get; set; }

        public double Version { get; set; }
    }
}
