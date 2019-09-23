using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Files;

namespace AdminManagementApp.Models
{
    public class Session
    {
        public List<SessionInfo> Directories { get; set; }

        public string NewSessionId { get; set; }

        public Session(IEnumerable<IFileMetadata> directories)
        {
            Directories = new List<SessionInfo>();
            foreach (var metadata in directories)
            {
                Directories.Add(new SessionInfo(metadata));
            }
            NewSessionId = Guid.NewGuid().ToString().ToUpper();
        }
    }

    public class SessionInfo
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public SessionInfo(IFileMetadata metadata)
        {
            Key = metadata.Directory;
            if (Key.Length > 6)
            {
                Key = metadata.Directory.Substring(0, 6).ToUpper();
            }
            Value = metadata.Directory.ToUpper();
        }
    }
}
