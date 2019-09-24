using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Files
{
    public interface IFile : IFileMetadata
    {
        string TextContents { get; }
        byte[] DataContents { get; }

        void AddTextContent(string textFile);
        void AddDataContent(byte[] dataFile);

        string GetDataContentsAsString(Encoding encoding);
    }
}
