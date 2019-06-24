using System;
using System.Collections.Generic;
using System.Text;

namespace TileFactory.Utility
{
    public static class Identifier
    {
        public static int ToId(int zoom, int x, int y)
        {
            return (((1 << zoom) * y + x) * 32) + zoom;
        }
    }
}
