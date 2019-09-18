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
        IList<IFileMetadata> Files { get; set; }
    }

    public interface IFile : IFileMetadata
    {
        string TextContents { get; set; }
        byte[] DataContents { get; set; }
    }

    public class FileMetadata 
    {
    }

    public class File : IFile, IFileMetadata
    {
        #region Fields



        #endregion

        #region Properties

        public string Directory { get; set; }
        public string Name { get; set; }
        public int ContentLength { get; set; }
        public IList<IFileMetadata> Files { get; set; }
        public string TextContents { get; set; }
        public byte[] DataContents { get; set; }

        #endregion

        #region Methods



        #endregion
    }
}
