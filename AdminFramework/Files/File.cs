using System;
using System.Collections.Generic;
using System.Text;
using Universal.Contracts.Files;

namespace Files
{
    public class FileMetadata : IFileMetadata
    {
        public string Directory { get; set; }
        public string Name { get; set; }
        public int ContentLength { get; set; }
        public IList<IFileMetadata> Files { get; set; }
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
        public string TextContents { get; private set; }
        public byte[] DataContents { get; private set; }

        #endregion

        #region Methods
        
        public void AddDataContent(byte[] dataFile)
        {
            this.DataContents = dataFile;
        }

        public void AddTextContent(string textFile)
        {
            this.TextContents = textFile;
        }

        public string GetDataContentsAsString(Encoding encoding)
        {
            if (DataContents != null)
            {
                TextContents = encoding.GetString(DataContents);
                return TextContents;
            }
            else if (!string.IsNullOrEmpty(TextContents))
                return TextContents;
            else
                return string.Empty;
        }
        
        #endregion
    }
}
