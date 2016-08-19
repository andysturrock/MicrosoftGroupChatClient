using System;
using System.Diagnostics;

namespace MicrosoftGroupChatClient
{
    public class ConsoleEventLog : EventLog
    {
        public new void WriteEntry(string message)
        {
            Console.Out.WriteLine(message);
        }
    }
}
