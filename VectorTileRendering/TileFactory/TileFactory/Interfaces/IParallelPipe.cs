using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFactory.Interfaces
{
    public interface IParallelPipe : IPipe
    {
        /// <summary>
        /// Execute this pipe within the parent pipe with all the siblings.
        /// </summary>
        /// <param name="parallelPipe"></param>
        /// <returns></returns>
        IParallelPipe CombineWith(IParallelPipe parallelPipe);
    }
}
