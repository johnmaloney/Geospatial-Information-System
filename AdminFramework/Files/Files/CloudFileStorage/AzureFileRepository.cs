using Files.CloudFileStorage;
using Files.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Files
{
    public class AzureFileRepository : IFileRepository
    {
        #region Fields

        private readonly AzureFileReader fileReader;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public AzureFileRepository(AzureFileReader fileReader)
        {
            this.fileReader = fileReader;
        }
        
        public async Task<IEnumerable<IFileMetadata>> GetDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public async Task<IFile> Get(string directoryPath, string fileName)
        {
            return null;
        }

        public async Task Add(IFile file)
        {
            // If only the directory is defined then create the DIR //
            if (string.IsNullOrEmpty(file.Name) && !string.IsNullOrEmpty(file.Directory))
            {
                await fileReader.CreateDirectory(file.Directory);
            }
            else
            {
                await fileReader.AddFile(file);
            }
        }
        
        #endregion
    }
}
