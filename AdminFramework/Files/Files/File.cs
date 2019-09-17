using System;
using System.Collections.Generic;
using System.Text;

namespace Files
{
    public interface IFileMetadata
    {
        string Directory { get; set; }
        string Name { get; set; }
        int ContentLength { get; set; }
        IEnumerable<IFileMetadata> Files { get; set; }
    }

    public interface IFile
    {
        string TextContents { get; set; }
        byte[] DataContents { get; set; }
    }

    public class FileMetadata 
    {
    }

    public class File 
    {

    }
}
