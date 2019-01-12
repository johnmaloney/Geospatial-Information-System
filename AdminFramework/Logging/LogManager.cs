using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Universal.Contracts.Logging;

namespace Logging
{

    public class LogManager : Universal.Contracts.Logging.ILogger
    {
        #region Fields

        private readonly Logger seriLogger;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public LogManager(string kibanaUri)
        {
            var configuration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(kibanaUri))
                {
                    AutoRegisterTemplate = true
                });

            seriLogger = configuration.CreateLogger();
        }

        public async Task<bool> Log(ILogEntry entry)
        {            
            return await Task.Factory.StartNew(()=>
            {
                seriLogger.Information($"Title: {entry.Title}, Type:{entry.Type}, Version:{entry.Id}", entry);
                return true;
            });
        }

        #endregion
    }
}
