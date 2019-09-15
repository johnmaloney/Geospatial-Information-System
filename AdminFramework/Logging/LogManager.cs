using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Universal.Contracts.Logging;
using Universal.Contracts.Serial;

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
            Func<bool> task = null;
            switch (Enum.Parse(typeof(LogType),entry.Type))
            {
                case LogType.Default:
                    task = () =>
                    {
                        seriLogger.Information("Title : {Title} Body: {Body}", entry.Title, entry.SerializeToJson());
                        return true;
                    };
                    break;
                case LogType.Debug:
                    task = () =>
                    {
                        seriLogger.Debug("Title : {Title} Body: {Body}", entry.Title, entry.SerializeToJson());
                        return true;
                    };
                    break;
                case LogType.Error:
                    task = () =>
                    {
                        seriLogger.Error("Title : {Title} Body: {Body}", entry.Title, entry.SerializeToJson());
                        return true;
                    };
                    break;
                case LogType.Fatal:
                    task = () =>
                    {
                        seriLogger.Fatal("Title : {Title} Body: {Body}", entry.Title, entry.SerializeToJson());
                        return true;
                    };
                    break;
                case LogType.Information:
                    task = () =>
                    {
                        seriLogger.Information("Title : {Title} Body: {Body}", entry.Title, entry.SerializeToJson());
                        return true;
                    };
                    break;
                default:
                    task = () =>
                    {
                        seriLogger.Information("Title : {Title} Body: {Body}", entry.Title, entry.SerializeToJson());
                        return true;
                    };
                    break;
            }

            return await Task.Factory.StartNew(task);
        }

        #endregion
    }
}
