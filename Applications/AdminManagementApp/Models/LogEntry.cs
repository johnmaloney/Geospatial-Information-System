using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Logging;

namespace AdminManagementApp
{
    public class LogEntry : ILogEntry
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public double Id { get; set; }
    }
}
