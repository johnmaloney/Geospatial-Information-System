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
using System.Text;
using TileFactory;
using Universal.Contracts.Layers;
using Universal.Contracts.Models;
using Universal.Contracts.Tiles;

namespace TileProcessingApp.Observers
{
    public class ProjectedDataObserver
    {
        #region Fields

        private readonly ITopicMessengerClient messenger;
        private readonly MessageRepository messageRepository;
        private readonly IFileRepository fileRepository;
        private readonly TileRetrieverService tileService;
        private readonly ILayerInitializationService layerService;
        private readonly ILogger logger;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ProjectedDataObserver(ITopicMessengerClient messenger, 
            MessageRepository messageRepository, IFileRepository fileRepository,
            TileRetrieverService tileService,
            ILayerInitializationService layerService,
            ILogger logger)
        {
            this.messenger = messenger;
            this.messageRepository = messageRepository;
            this.fileRepository = fileRepository;
            this.tileService = tileService;
            this.layerService = layerService;
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

                    string converted = uploadedFile.GetDataContentsAsString(Encoding.UTF8);
                    string uniqueId = GenerateUniqueId(gCommand.Id);
                    var context = new GeoJsonContext(uploadedFile.TextContents)
                    {
                        Identifier = fileName + $"_{uniqueId}",
                        MaxZoom = 14,
                        Buffer = 64,
                        Extent = 4096,
                        Tolerance = 3
                    };

                    var pipeline = new DetermineCollectionsTypePipeline()
                        .ExtendWith(new ParseGeoJsonToFeatures()
                            .IterateWith(new ProjectGeoJSONToGeometric(
                                (geoItem) => new WebMercatorProcessor(geoItem)))
                            .ExtendWith(new GeometricSimplification())
                        .ExtendWith(new InitializeProjectedFeatures(tileService))); 

                    await pipeline.Process(context);

                    var layerModel = new LayerInformationModel
                    {
                        Identifier = gCommand.Id,
                        Name = context.Identifier, 
                        Properties = new Property[]
                        {
                            new Property { Name = "features", Value = context.TileFeatures, ValueType = typeof(List<IGeometryItem>) }
                        }
                    };
                    layerService.AddLayer(layerModel);

                    var topicFinished = new TopicMessage
                    {
                        Message = $"Tile Processing request FINISHED for {gCommand.Command}, for ID:{gCommand.Id.ToString()}"
                    };
                    messageRepository.AddMessage(topicFinished);
                    await messenger.Send(topicFinished);
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

        private string GenerateUniqueId(Guid messageId)
        {
            if (messageId != Guid.Empty)
                return messageId.ToString().Substring(0, 6);
            else
                return DateTime.Now.Ticks.ToString();
        }

        #endregion
    }
}
