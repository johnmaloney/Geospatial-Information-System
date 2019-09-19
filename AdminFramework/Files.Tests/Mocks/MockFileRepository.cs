using Universal.Contracts.Files;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Files.Tests.Mocks
{
    public class MockFileRepository : IFileRepository
    {
        private readonly IFileReader fileReader;

        public MockFileRepository(IFileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        public async Task Add(IFile file)
        {
            await fileReader.AddFile(file);
        }

        public async Task<IFile> Get(string directoryPath, string fileName)
        {
            var allFiles = await fileReader.ListAll();
            var dirFiles = allFiles.FirstOrDefault(f => f.Directory == directoryPath).Files;
            return dirFiles.FirstOrDefault(f => f.Name == fileName) as IFile;
        }

        public async Task<IEnumerable<IFileMetadata>> GetDirectory(string directory)
        {
            var files = await fileReader.ListAll();
            return files.Where(f => f.Directory == directory);
        }
    }
}
