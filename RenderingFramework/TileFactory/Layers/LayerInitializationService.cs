using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TileFactory.Interfaces;
using TileFactory.Layers;
using TileFactory.Models;
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
        private readonly ICacheStorage<Guid, IEnumerable<IGeometryItem>> featureCache =new SimpleFeaturesCache();
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
                        new Property { Name = LayerProperties.FileExt, Value = Path.GetExtension(item.PhysicalPath), ValueType = typeof(string) },
                        new Property { Name = LayerProperties.TileAccessTemplate, Value = serverIP + "/v1/tiles/"+ name +"/{z}/{x}/{y}.vector.pbf?access_token={token}"}
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

            if (model.Properties.Any(p => p.Name.ToLower() == LayerProperties.FileExt && p.Value != null && !string.IsNullOrEmpty(p.Value.ToString())))
            {
                return GetFeatures(model.GetPropertyValueAs<string>(LayerProperties.FileExt), await GetText(model.Path));
            }
            else if (model.Properties.Any(p => p.Name == LayerProperties.Features))
            {
                // this is loading from memory //
                var features = model.Properties.First(p => p.Name == LayerProperties.Features);
                return featureCache.GetBy(model.Identifier);
            }
            else
                throw new NotSupportedException($"The features of the requested layer with name: {name}, could not be found.");
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
                       
            if (model.Properties.Any(p => p.Name.ToLower() == LayerProperties.FileExt && p.Value != null && !string.IsNullOrEmpty(p.Value.ToString())))
            {
                return GetFeatures(model.GetPropertyValueAs<string>(LayerProperties.FileExt), await GetText(model.Path));
            }
            else if (model.Properties.Any(p => p.Name == LayerProperties.Features))
            {
                // this is loading from memory //
                var features = model.Properties.First(p => p.Name == LayerProperties.Features);
                return featureCache.GetBy(model.Identifier);
            }
            else
                throw new NotSupportedException($"The features of the requested layer with Guid: {identifier}, could not be found.");
        }

        public void AddLayer(LayerInformationModel model)
        {
            var properties = new List<Property>();
            
            var tileTemplate = new Property
            {
                Name = LayerProperties.TileAccessTemplate,
                Value = serverIP + "/v1/tiles/" + model.Name + "/{z}/{x}/{y}.vector.pbf?access_token={token}"
            };

            var fileExtension = new Property
            {
                Name = LayerProperties.FileExt,
                Value = !string.IsNullOrEmpty(model.Path) ? Path.GetExtension(model.Path) : string.Empty,
                ValueType = typeof(string)
            };
            
            if (model.Properties != null)
            {
                properties.AddRange(model.Properties);

                if (!model.Properties.Any(p => p.Name.ToLower() == LayerProperties.TileAccessTemplate))
                {
                    properties.Add(tileTemplate);
                }
                if (!model.Properties.Any(p => p.Name.ToLower() == LayerProperties.FileExt))
                {
                    properties.Add(fileExtension);
                }
                if (model.Properties.Any(p => p.Name.ToLower() == LayerProperties.Features))
                {
                    var featuresProperty = model.Properties.First(p => p.Name.ToLower() == LayerProperties.Features);
                    try
                    {
                        var features = (IEnumerable<IGeometryItem>)featuresProperty.Value;
                        featureCache.StoreBy(model.Identifier, features);
                        featuresProperty.Value = features.Count();
                    }
                    catch (Exception ex)
                    {
                        featuresProperty.Value = $"An error occurred while attempting to access/overwrite the features. {ex.Message} : {ex.StackTrace}";
                    }
                    properties.Remove(properties.First(p => p.Name.ToLower() == LayerProperties.Features));
                    properties.Add(featuresProperty);
                }
            }  
            else
            {
                properties.Add(tileTemplate);
                properties.Add(fileExtension);
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
