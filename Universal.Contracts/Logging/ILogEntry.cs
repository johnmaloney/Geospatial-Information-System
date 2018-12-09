using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Logging
{
    public interface ILogEntry
    {
        string Type { get; }

        string Title { get; }
    }
}
