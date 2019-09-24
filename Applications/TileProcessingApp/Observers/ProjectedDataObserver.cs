using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TileFactory.DataPipeline;
using TileFactory.DataPipeline.GeoJson;
using TileFactory.Utility;
using TileProcessingApp.Models;
using Universal.Contracts.Files;
using Universal.Contracts.Messaging;
using Universal.Contracts.Logging;

namespace TileProcessingApp.Observers
{
    public class ProjectedDataObserver
    {
        #region Fields

        private readonly ITopicMessengerClient messenger;
        private readonly MessageRepository messageRepository;
        private readonly IFileRepository fileRepository;
        private readonly ILogger logger;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ProjectedDataObserver(ITopicMessengerClient messenger, 
            MessageRepository messageRepository, IFileRepository fileRepository,
            ILogger logger)
        {
            this.messenger = messenger;
            this.messageRepository = messageRepository;
            this.fileRepository = fileRepository;
            this.logger = logger;
        }

        internal async Task CommandReceiver(IMessage message)
        {
            if (message is GeneralCommand gCommand)
            {
                // This will be the function that receives a command from the event framework //
                // and processes the command into a valid data conversion //
                var topic = new TopicMessage { Message = 
                    $"Tile Processing request started for {gCommand.Command}, for ID:{gCommand.Id.ToString()}" };
                messageRepository.AddMessage(topic);
                await messenger.Send(topic);

                try
                {
                    var fileName = gCommand.CommandDataCollection.FirstOrDefault(cd => cd.DataType == "filename")?.Data?.ToString();
                    var uploadedFile = await fileRepository.Get(gCommand.Id.ToString(), fileName);

                    string converted = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    var context = new GeoJsonContext(uploadedFile.TextContents)
                    {
                        MaxZoom = 14,
                        Buffer = 64,
                        Extent = 4096,
                        Tolerance = 3
                    };

                    var pipeline = new DetermineCollectionsTypePipeline()
                        .ExtendWith(new ParseGeoJsonToFeatures()
                            .IterateWith(new ProjectGeoJSONToGeometric(
                                (geoItem) => new WebMercatorProcessor(geoItem)))
                            .ExtendWith(new GeometricSimplification()));

                    await pipeline.Process(context);
                }
                catch (Exception ex)
                {
                    await logger.Log(new MessageLogEntry
                    {
                        Type = LogType.Error.ToString(),
                        Title = $"Processing the command {gCommand.Command} failed while building the projected data. Error Message : {ex.Message}",
                        Id = gCommand.CorrellationId, 
                        MessageBody = ex.StackTrace
                    });
                }

            }
        }

        #endregion
    }
}
