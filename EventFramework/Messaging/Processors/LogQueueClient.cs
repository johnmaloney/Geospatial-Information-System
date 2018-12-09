using Messaging.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Processors
{
    public class LogMessage : IMessage
    {
        public int CorrellationId { get; set; }
    }


    public class LogQueueClient : IQueueClient
    {
        #region Fields



        #endregion

        #region Properties



        #endregion

        #region Methods

        public Task Send(IMessage message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
