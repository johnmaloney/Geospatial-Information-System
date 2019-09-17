using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Contracts.Files
{
    interface IFIleRepository
    {
        Task<File> Get(string directoryPath, string fileName);
        Task Add(File file);
    }
}
