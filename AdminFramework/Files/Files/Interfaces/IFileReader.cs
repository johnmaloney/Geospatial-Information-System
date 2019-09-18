using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Interfaces
{
    public interface IFileReader
    {
        Task<IEnumerable<IFileMetadata>> ListAll();
        Task CreateDirectory(string directoryName);
        Task AddFile(IFile file);
    }
}
