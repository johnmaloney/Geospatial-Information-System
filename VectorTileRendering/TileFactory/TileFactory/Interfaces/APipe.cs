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
        private Queue<IPipe> iterativePipes = new Queue<IPipe>();

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

        protected bool HasNextIterativePipe
        {
            get
            {
                return this.iterativePipes.Count > 0;
            }
        }

        public virtual IPipe NextIterative
        {
            get
            {
                if (iterativePipes.Count > 0)
                    return iterativePipes.Dequeue();
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
            this.iterativePipes.Enqueue(pipe);
            return this;
        }

        public abstract Task Process(IPipeContext context);

        #endregion
    }
}

