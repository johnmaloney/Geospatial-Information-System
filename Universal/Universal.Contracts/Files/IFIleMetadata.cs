using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Files
{
    public interface IFileMetadata
    {
        string Directory { get; set; }
        string Name { get; set; }
        int ContentLength { get; set; }
        IList<IFileMetadata> Files { get; set; }
    }
}
