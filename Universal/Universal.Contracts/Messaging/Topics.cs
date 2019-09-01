using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Messaging
{
    public struct Topics
    {
        public const string GeneralInfo = "general-info";
        public const string GeneralEvent = "general-event";
    }

    public struct Queues
    {
        public const string GeneralCommand = "general-command";
    }
}
