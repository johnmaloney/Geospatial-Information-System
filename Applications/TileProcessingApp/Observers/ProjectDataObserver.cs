using Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universal.Contracts.Messaging;

namespace TileProcessingApp.Observers
{
    public class ProjectDataObserver
    {
        #region Fields
        
        #endregion

        #region Properties

        #endregion

        #region Methods
        
        internal async Task CommandReceiver(IMessage message)
        {
            // This will be the function that receives a command from the event framework //
            // and processes the command into a valid data conversion //
        }

        #endregion
    }
}
