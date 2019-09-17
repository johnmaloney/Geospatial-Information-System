using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Files.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<IFileMetadata>> GetDirectory(string directory);
        Task<IFile> Get(string directoryPath, string fileName);
        Task Add(IFile file); 
    }
}
