using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory
{
    public static class Constants
    {
        public static class ClipTypes
        {
            public const string Left = "left";
            public const string Right = "right";
        }

        public enum ClipType
        {
            Left = 1, 
            Right = 2, 
            None = 3, 
            Rejected = 4
        }
    }
}
