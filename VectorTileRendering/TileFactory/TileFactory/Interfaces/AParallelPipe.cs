using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TileFactory.Interfaces
{
    public abstract class AParallelPipe : APipe, IParallelPipe
    {
        #region Fields

        // store the pipes that will be processed in parallel //
        private List<IParallelPipe> parallelPipes = new List<IParallelPipe>();

        // store the Task methods that will executed in parallel //
        private List<Func<IPipeContext, Task>> parallelProcesses = new List<Func<IPipeContext, Task>>();

        private int maxParallelThreads = 100;
        private bool canExecuteInParallel = true;

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Adds the pipe into a collection so that during the execution their Tasks can be used.
        /// </summary>
        /// <param name="parallelPipe"></param>
        /// <returns></returns>
        public IParallelPipe CombineWith(IParallelPipe parallelPipe)
        {
            parallelPipes.Add(parallelPipe);
            return this;
        }

        /// <summary>
        /// Processes all items in the this pipeline are executed in parallel.
        /// This can be controlled through the IEnvironment.CanExecuteInParallel property.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task ProcessAll(IPipeContext context)
        {
            if (this.canExecuteInParallel)
            {
                runThreadBatches(context);
            }
            else
            {
                foreach (var pipe in parallelPipes)
                {
                    await pipe.Process(context);
                }
            }

            // Moves to the next PIPE in the process //
            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        /// <summary>
        /// 
        /// </summary>
        internal ConcurrentDictionary<Guid, ThreadCounter> runThreadBatches(IPipeContext context, bool setThreadStats = false)
        {
            int concurrentThreads = 0;
            var maxNumberOfRetrievers = this.maxParallelThreads;
            ConcurrentDictionary<Guid, ThreadCounter> threadStats = null;

            foreach (var pipe in parallelPipes)
            {
                parallelProcesses.Add(pipe.Process);
            }

            if (setThreadStats)
            {
                threadStats = new ConcurrentDictionary<Guid, ThreadCounter>();
                concurrentThreads = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
            }

            var waitTasks = new List<Task>();
            if (parallelProcesses != null || parallelProcesses.Count != 0)
            {
                // Start a semphore to manage the max
                // parallel retiever logic.
                var throttler = new SemaphoreSlim(maxNumberOfRetrievers, maxNumberOfRetrievers);
                // using (var throttler = new System.Threading.SemaphoreSlim(maxNumberOfRetrievers))
                //{
                foreach (var task in parallelPipes)
                {

                    waitTasks.Add(Task.Run(async () =>
                    {
                        await throttler.WaitAsync();

                        Interlocked.Increment(ref concurrentThreads);

                        try
                        {
                            await task.Process(context);
                            //throttler.Release();
                        }
                        finally
                        {
                            //release the counter for the completed task
                            throttler.Release();
                            Interlocked.Decrement(ref concurrentThreads);

                            if (setThreadStats)
                            {
                                threadStats.TryAdd(Guid.NewGuid(),
                                    new ThreadCounter(System.Diagnostics.Process.GetCurrentProcess().Threads.Count,
                                    maxNumberOfRetrievers - throttler.CurrentCount
                                    ));
                            }
                        }
                    }
                    ));
                }
                // }
                // Guarantees that all processes finish before this is allowed to continue //
                Task.WaitAll(waitTasks.ToArray());
            }

            return threadStats;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal struct ThreadCounter
    {
        public int waitingThreads;
        public int runningThreads;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_waitingThread"></param>
        /// <param name="_runningThreads"></param>
        public ThreadCounter(int _waitingThread, int _runningThreads)
        {
            waitingThreads = _waitingThread;
            runningThreads = _runningThreads;
        }
    }
}
