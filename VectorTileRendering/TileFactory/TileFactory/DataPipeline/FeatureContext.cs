using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline
{
    internal class IterativeFeatureContext : IPipeContext
    {
        public Feature Feature { get; private set; }

        public double Buffer { get; private set; }

        public IterativeFeatureContext(Feature feature, double buffer)
        {
            this.Feature = feature;
            this.Buffer = buffer;
        }
    }
}
