using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace AdminManagementApp.Models
{
    public class JobRequest
    {
        public string SessionId { get; set; }
        public string Message { get; set; }
        public string JobType { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }

        private Guid SessionIdentifier
        {
            get
            {
                if (Guid.TryParse(SessionId, out Guid sessionId))
                {
                    return sessionId;
                }
                return Guid.Empty;
            }
        }

        public IMessage GenerateMessage()
        {
            IMessage message = null;
            switch (JobType)
            {
                case "projectData":
                    {
                        message = new GeneralCommand
                        {
                            Id = SessionIdentifier,
                            Command = JobType,
                            CommandDataCollection = new List<ICommandData>
                            {
                                new CommandData { Data = FileName, DataType = "filename" }
                            }
                        };
                        break;
                    }
                case "generateTiles":
                    {
                        message = new GeneralCommand
                        {
                            Id = SessionIdentifier,
                            Command = JobType,
                            CommandDataCollection = new List<ICommandData>
                            {
                                new CommandData { Data = FileName, DataType = "filename" }
                            }
                        };
                        break;
                    }
                default:
                    break;
            }
            return message;
        }
    }
}
