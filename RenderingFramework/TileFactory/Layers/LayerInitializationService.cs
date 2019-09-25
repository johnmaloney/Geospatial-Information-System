using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TileFactory.Interfaces;
using Universal.Contracts.Layers;
using Universal.Contracts.Models;
using Universal.Contracts.Serial;
using Universal.Contracts.Tiles;

namespace TileFactory
{
    /// <summary>
    /// This class will build up the information that is required to return tiles to the 
    /// client. It uses the FileProvider
    /// </summary>
    public class LayerInitializationFileService : ILayerInitializationService
    {
        #region Fields

        private readonly IFileProvider fileProvider;
        private readonly string serverIP;
        private List<LayerInformationModel> models = new List<LayerInformationModel>();
        private readonly Dictionary<string, string> files = new Dictionary<string, string>();
        private object fileLock = new object();

        #endregion

        #region Properties

        public IEnumerable<LayerInformationModel> Models { get { return models; } }

        #endregion

        #region Methods

        public LayerInitializationFileService(IFileProvider fileProvider, string serverIP = "")
        {
            this.fileProvider = fileProvider;
            this.serverIP = serverIP;
            foreach (var item in this.fileProvider.GetDirectoryContents("/"))
            {
                var name = Path.GetFileNameWithoutExtension(item.PhysicalPath);
                var layerInformation = new LayerInformationModel()
                {
                    Identifier = Guid.NewGuid(),
                    Name = name,
                    Path = item.PhysicalPath,
                    Properties = new Property[]
                    {
                        new Property { Name = "FileExtension", Value = Path.GetExtension(item.PhysicalPath), ValueType = typeof(string) },
                        new Property { Name = "TileAccessTemplate", Value = serverIP + "/v1/tiles/"+ name +"/{z}/{x}/{y}.vector.pbf?access_token={token}"}
                    }
                };

                models.Add(layerInformation);

                files.Add(item.Name, item.PhysicalPath);
            }
        }

        /// <summary>
        /// Look into the file system and see what tiles with the given identifier
        /// can be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IGeometryItem>> InitializeLayer(string name)
        {
            if (!Models.Any(m => m.Name.ToLower() == name.ToLower()))
                throw new NotSupportedException($"The Layer named {name} was not found in the Models collection");

            var model = Models.First(m => m.Name.ToLower() == name.ToLower());

            return GetFeatures(model.GetPropertyValueAs<string>("FileExtension"), await GetText(model.Path));
        }

        /// <summary>
        /// Look into the file system and see what tiles with the given identifier
        /// can be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IGeometryItem>> InitializeLayer(Guid identifier)
        {
            if (!Models.Any(m => m.Identifier == identifier))
                throw new NotSupportedException($"The Layer with Id: {identifier} was not found in the Models collection");

            var model = Models.First(m => m.Identifier == identifier);

            return GetFeatures(model.GetPropertyValueAs<string>("FileExtension"), await GetText(model.Path));
        }

        public void AddLayer(LayerInformationModel model)
        {
            var properties = new List<Property>();
            properties.AddRange(model.Properties);

            if (!model.Properties.Any(p => p.Name.ToLower() == "tileaccesstemplate"))
            {
                properties.Add(
                    new Property
                    {
                        Name = "TileAccessTemplate",
                        Value = serverIP + "/v1/tiles/" + model.Name + "/{z}/{x}/{y}.vector.pbf?access_token={token}"
                    });
            }
            if (string.IsNullOrEmpty(model.Path) && !model.Properties.Any(p=> p.Name.ToLower() == "fileExtension"))
            {
                properties.Add(
                    new Property
                    {
                        Name = "FileExtension",
                        Value = Path.GetExtension(model.Path),
                        ValueType = typeof(string)
                    });
            }
            model.Properties = properties.ToArray();
             
            models.Add(model);
        }

        private IEnumerable<IGeometryItem> GetFeatures(string extension, string text)
        {
            switch (extension)
            {
                case ".json":
                    {
                        return text.FromJsonInto<List<Feature>>();
                    }
                default:
                    break;
            }

            return null;
        }

        private async Task<string> GetText(string filePath)
        {
            string fileText = string.Empty;
            return await File.ReadAllTextAsync(filePath);
        }

        #endregion
    }
}
