using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Files
{
    public interface IFileReader
    {
        Task<IEnumerable<IFileMetadata>> ListAll();
        Task CreateDirectory(string directoryName);
        Task AddFile(IFile file);
    }
}
