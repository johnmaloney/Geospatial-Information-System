using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory
{
    public class Feature : IGeometryItem
    {
        #region Fields

        private (double X, double Y, double Z)[][] geometry;
        private double[] totalAreaCalculations;
        private double[] totalDistanceCalculations;

        #endregion

        #region Properties

        public string Id { get; set; }

        public GeometryType Type { get; private set; }

        public (double X, double Y, double Z)[][] Geometry
        {
            get { return geometry; }
            set
            {
                geometry = value;
                calculateBoundingBox(value);
            }
        }

        public Dictionary<string, object> Tags { get; set; }

        public (double X, double Y, double Z) MaxGeometry { get; private set; }

        public (double X, double Y, double Z) MinGeometry { get; private set; }

        public double[] Area
        {
            get
            {
                if (totalAreaCalculations==null || totalDistanceCalculations == null)
                {
                    calculateAreaAndDistance();
                }
                return totalAreaCalculations;
            }
        }

        public double[] Distance
        {
            get
            {
                if (totalAreaCalculations == null || totalDistanceCalculations == null)
                {
                    calculateAreaAndDistance();
                }
                return totalDistanceCalculations;
            }
        }

        public bool IsOuter { get { return true;} }

        #endregion

        #region Methods

        public Feature(GeometryType type)
        {
            this.Type = type;
        }

        private void calculateBoundingBox((double X, double Y, double Z)[][] value)
        {
            this.MinGeometry = (X: double.PositiveInfinity, Y: double.PositiveInfinity, Z: 0d);
            this.MaxGeometry = (X: double.NegativeInfinity, Y: double.NegativeInfinity, Z: 0d);

            if (this.Type == GeometryType.Point || this.Type == GeometryType.MultiPoint)
            {
                calculateRingBoundingBox(MinGeometry, MaxGeometry, value[0]);
            }
            else
            {
                for (int i = 0; i < geometry.Length; i++)
                {
                    calculateRingBoundingBox(MinGeometry, MaxGeometry, geometry[i]);
                }
            }
        }

        private void calculateRingBoundingBox(
            (double X, double Y, double Z) minGeometry, 
            (double X, double Y, double Z) maxGeometry, 
            (double X, double Y, double Z)[] points)
        {
            var min = (X: double.PositiveInfinity, Y: double.PositiveInfinity, Z: 0d);
            var max = (X: double.NegativeInfinity, Y: double.NegativeInfinity, Z: 0d);
            for (int i = 0; i < points.Length; i++)
            {
                var current = points[i];
                min.X = Math.Min(current.X, min.X);
                max.X = Math.Max(current.X, max.X);

                min.Y = Math.Min(current.Y, min.Y);
                max.Y = Math.Max(current.Y, max.Y);
            }

            MinGeometry = min;
            MaxGeometry = max;
        }

        /// <summary>
        /// Calculate the area and the length of the polygon.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        private void calculateAreaAndDistance()
        {
            if (this.geometry.Length == 0)
                throw new NotSupportedException("The Geometry of the feature must have value to calculate Area and Distance");

            if (totalAreaCalculations != null && totalDistanceCalculations != null)
                return;

            totalAreaCalculations = new double[this.geometry.Length];
            totalDistanceCalculations = new double[this.geometry.Length];

            for (int i = 0; i < this.geometry.Length; i++)
            {
                double area = 0, distance = 0;

                for (int j = 0; j < geometry[i].Length - 1; j++)
                {
                    var a = geometry[i][j];
                    var b = geometry[i][j + 1];

                    area += a.X * b.Y - b.X * a.Y;

                    // Use the Manhattan distance instead of the Euclidian one to avoid //
                    // the expensive square root computation //
                    distance += Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
                }

                totalAreaCalculations[i] = Math.Abs(area / 2);
                totalDistanceCalculations[i] = distance;
            }
        }

        #endregion
    }

    public struct GeometricItem
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double Area { get; private set; }
        public double Distance { get; private set; }
    }
}
