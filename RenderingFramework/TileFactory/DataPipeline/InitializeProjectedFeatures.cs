using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline
{
    public class InitializeProjectedFeatures : APipe, IPipe
    {
        private readonly TileRetrieverService generator;

        public InitializeProjectedFeatures(TileRetrieverService generator)
        {
            this.generator = generator;
        }

        public override async Task Process(IPipeContext context)
        {
            // Want to take a tile context with projected features and add them //
            // to the existing cache system, this will only require the Generator //
            // class 

            if (!(context is ITileContext tileContext))
                throw new NotSupportedException("The IPipeline Context was not convertible to ITileContext.");

            if (tileContext.TileFeatures == null)
                throw new NotSupportedException("The tile context had not features in it.");
            

            await generator.InitializeTile(tileContext);

            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }
    }
}
