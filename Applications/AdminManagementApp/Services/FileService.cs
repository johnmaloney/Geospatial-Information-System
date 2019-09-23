using AdminManagementApp.Models;
using Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.Contracts.Files;

namespace AdminManagementApp.Services
{
    public class FileService
    {
        #region Fields
        
        private readonly IFileRepository fileRepository;

        #endregion

        #region Properties



        #endregion

        #region Methods

        public FileService(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        public async Task<IEnumerable<IFileMetadata>> GetDirectory(string directory)
        {
            return await fileRepository.GetDirectory(directory);
        }

        public async Task<bool> CreateDirectoryAndFile(JobRequest request)
        {

            var directory = new File
            {
                Directory = request.SessionId
            };

            await fileRepository.Add(directory);

            if (!string.IsNullOrEmpty(request.FileName))
            {
                var file = new File
                {
                    Directory = request.SessionId,
                    Name = request.FileName,
                    TextContents = Encoding.UTF8.GetString(request.FileContent)
                };

                await fileRepository.Add(file);
            }

            return true;
        }

        #endregion
    }
}
