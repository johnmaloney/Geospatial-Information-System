using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Tiles
{
    public interface IGeoData
    {
        string Id { get; set; }

        IDictionary<string, object> Tags { get; set; }
    }
}
