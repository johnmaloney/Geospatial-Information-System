using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Universal.Contracts.Models;
using Universal.Contracts.Tiles;

namespace Universal.Contracts.Layers
{
    public interface ILayerInitializationService
    {
        IEnumerable<LayerInformationModel> Models { get; }
        Task<IEnumerable<IGeometryItem>> InitializeLayer(string name);
        Task<IEnumerable<IGeometryItem>> InitializeLayer(Guid identifier);
        void AddLayer(LayerInformationModel model);
    }
}
