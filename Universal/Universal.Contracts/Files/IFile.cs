using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Files
{
    public interface IFile : IFileMetadata
    {
        string TextContents { get; set; }
        byte[] DataContents { get; set; }
    }
}
