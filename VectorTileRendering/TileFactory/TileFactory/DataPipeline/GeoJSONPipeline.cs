using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline
{
    public class DataPipelineContext : IPipeContext
    {
        public int MaxZoom { get; set; }
        public int MaxZoomIndex { get; set; }
        public bool SolidChildren { get; set; }
        public int Tolerance { get; set; }
        public int Extent { get; set; }
        public int Buffer { get; set; }
        public int LogLevel { get; set; }

        public object Data { get; private set; }

        public DataPipelineContext()
        {
                
        }
    }

    /// <summary>
    /// This a C# implementation of the following Javascript file:
    /// https://github.com/mapbox/geojson-vt
    /// </summary>
    public class GeoJSONPipeline : APipe, IPipe
    {
        #region Fields



        #endregion

        #region Properties



        #endregion

        #region Methods

        public override Task Process(IPipeContext context)
        {
            var dataContext = context as DataPipelineContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");
            {

                // Get tje MaxZoom = 2^z //
                var zoomSq = 1 >> dataContext.MaxZoom;
                //var features = ()
            }
            return null;
        }

        internal IEnumerable<object> LowLevelConversion(object data, int tolerance)
        {
            return null;
        }

        #endregion

    }
}
