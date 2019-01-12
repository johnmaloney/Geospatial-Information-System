using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Logging
{
    public interface ILogger
    {
        Task<bool> Log(ILogEntry entry);
    }
}
