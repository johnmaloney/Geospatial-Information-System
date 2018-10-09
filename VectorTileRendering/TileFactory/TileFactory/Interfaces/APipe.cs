using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFactory.Interfaces
{
    public abstract class APipe : IPipe
    {
        #region Fields
        
        private Queue<IPipe> pipes = new Queue<IPipe>();
        private IList<IPipe> iterationPipeline = new List<IPipe>();

        #endregion

        #region Properties

        protected bool HasNextPipe
        {
            get
            {
                return this.pipes.Count > 0;
            }
        }

        public virtual IPipe NextPipe
        {
            get
            {
                if (pipes.Count > 0)
                    return pipes.Dequeue();
                else
                    return null;
            }
        }

        #endregion

        #region Methods

        public virtual IPipe ExtendWith(IPipe pipe)
        {
            this.pipes.Enqueue(pipe);
            // Support method chaining //
            return this;
        }

        public virtual IPipe IterateWith(IPipe pipe)
        {
            // This collection is used iterate an item within the parent pipeline //
            this.iterationPipeline.Add(pipe);

            return this;
        }

        public abstract Task Process(IPipeContext context);

        public async Task Iterate(IPipeContext context)
        {
            foreach(var iteration in iterationPipeline)
            {
               await  iteration.Process(context);
            }
        }

        #endregion
    }
}

