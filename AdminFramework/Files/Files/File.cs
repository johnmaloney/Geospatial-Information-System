using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Files;

namespace Files
{
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
