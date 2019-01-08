using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TileFactory.Interfaces;

namespace TileFactory.DataPipeline
{
    /// <summary>
    /// Simplifies the features points.
    /// </summary>
    public class GeometricSimplification : APipe, IPipe
    {
        public override async Task Process(IPipeContext context)
        {
            var dataContext = context as AContext;

            if (dataContext == null)
                throw new NotSupportedException("The pipeline context must be of type DataPipelineContext");

            if (dataContext.TileFeatures == null)
                throw new NotSupportedException("The Features of the context must have a value for the data to be processed.");

            var stack = new Stack<int>();

            foreach (var feature in dataContext.TileFeatures)
            {
                var sqTolerance = dataContext.ExtentTolerance * dataContext.ExtentTolerance;
                for (int i = 0; i < feature.Geometry.Length; i++)
                {
                    var pointsLength = feature.Geometry[i].Length;

                    var firstIndex = 0;
                    var lastIndex = pointsLength - 1;
                    var index = 0;
                    double maxSqDist, sqDist = 0d;


                    // Always retain the endpoints (1 is the max value) //
                    feature.Geometry[i][firstIndex].Z = 1;
                    feature.Geometry[i][lastIndex].Z = 1;

                    while (lastIndex > 0)
                    {
                        maxSqDist = 0;

                        for (int j = firstIndex; j < lastIndex; j++)
                        {
                            sqDist = getSqSegDistance(feature.Geometry[i][j], feature.Geometry[i][firstIndex], feature.Geometry[i][lastIndex]);

                            if (sqDist > maxSqDist)
                            {
                                index = j;
                                maxSqDist = sqDist;
                            }
                        }

                        if (maxSqDist > sqTolerance)
                        {
                            feature.Geometry[i][index].Z = maxSqDist;
                            stack.Push(firstIndex);
                            stack.Push(index);
                            firstIndex = index;
                        }
                        else
                        {
                            if (stack.TryPop(out lastIndex))
                                firstIndex = stack.Pop();
                            else // END the WHILE loop //
                                lastIndex = 0;
                        }
                    }
                }
                
            }

            while (this.HasNextPipe)
                await this.NextPipe.Process(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 
        /// Replicates this: https://github.com/mapbox/geojson-vt/blob/master/src/simplify.js
        /// </remarks>
        /// <param name="current"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        private double getSqSegDistance((double X, double Y, double Z) current, (double X, double Y, double Z) first, (double X, double Y, double Z) last)
        {
            double x = first.X;
            double y = first.Y;
            double bX = last.X;
            double bY = last.Y;
            double pX = current.X;
            double pY = current.Y;
            double dX = bX - x;
            double dY = bY - y;

            if (dX != 0 || dY != 0)
            {
                var t = ((pX - x) * dX + (pY - y) * dY) / (dX * dX + dY * dY);

                if (t > 1)
                {
                    x = bX;
                    y = bY;
                }
                else if(t > 0)
                {
                    x += dX * t;
                    y += dY * t;
                }
            }

            dX = pX - x;
            dY = pY - y;

            return dX * dX + dY * dY;
        }
    }
}
