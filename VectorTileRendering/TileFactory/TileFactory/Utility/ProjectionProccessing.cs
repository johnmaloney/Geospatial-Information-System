using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Utility
{
    public class ProjectionProcessingFactory
    {
        #region Fields

        private Func<IGeospatialItem, IProjectionProcessor> ProjectionProcessor;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public ProjectionProcessingFactory(Func<IGeospatialItem, IProjectionProcessor> projectionProcessor)
        {
            this.ProjectionProcessor = projectionProcessor;
        }

        public void Process(IEnumerable<IGeospatialItem> unprocessedGeometryItems)
        {
        }

        public void ChangeProjectionProcessor(Func<IGeospatialItem, IProjectionProcessor> newProjectionProcessor)
        {
            ProjectionProcessor = newProjectionProcessor;
        }

        #endregion
    }
}
