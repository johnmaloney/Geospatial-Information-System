﻿using System;
using System.Collections.Generic;
using System.Text;
using TileFactory.Interfaces;

namespace TileFactory.Models
{
    /// <summary>
    ///
    /// </summary>
    public class DynamicTile : ITile
    {
        #region Fields



        #endregion

        #region Properties

        public IList<Feature> Features { get; set; }

        public IList<Feature> Source { get; set; }

        public int NumberOfPoints { get; set; }

        public int NumberOfSimplifiedPoints { get; set; }

        public int NumberOfFeatures { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool Transformed { get; set; }

        public (int X, int Y) Min { get; set; }

        public (int X, int Y) Max { get; set; }

        #endregion

        #region Methods



        #endregion
    }
}
