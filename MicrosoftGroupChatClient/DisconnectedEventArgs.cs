using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftGroupChatClient
{
    public class DisconnectedEventArgs : EventArgs
    {
        public DisconnectedEventArgs(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; set; }
    }
}
