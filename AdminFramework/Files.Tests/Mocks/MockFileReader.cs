using Files.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Tests.Mocks
{
    public class MockFileReader : IFileReader
    {
        private Dictionary<string, IFile> files = new Dictionary<string, IFile>();

        public async Task AddFile(IFile file)
        {
            if (files.ContainsKey(file.Directory))
            {
                files[file.Directory].Files.Add(file);
            }
            else
            {
                // Wrap the incoming file to create the sense of a directory //
                files.Add(file.Directory, 
                    new File
                    {
                        Directory = file.Directory,
                        Files = new List<IFileMetadata> { file }
                    });
            }
        }

        public Task CreateDirectory(string directoryName)
        {
            files.Add(directoryName, null);
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<IFileMetadata>> ListAll()
        {
            return await Task.FromResult(files.Select(kvp => kvp.Value as IFileMetadata));
        }
    }
}
