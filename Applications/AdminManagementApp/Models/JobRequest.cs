using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminManagementApp.Models
{
    public class JobRequest
    {
        public string SessionId { get; set; }
        public string Message { get; set; }
        public string JobType { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
