using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFactory.Interfaces
{
    public abstract class APipe : IPipe
    {
        private Queue<IPipe> pipes = new Queue<IPipe>();

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

        public virtual IPipe ExtendWith(IPipe pipe)
        {
            this.pipes.Enqueue(pipe);
            // Support method chaining //
            return this;
        }

        public abstract Task Process(IPipeContext context);
    }
}

