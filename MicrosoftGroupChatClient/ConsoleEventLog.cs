using System;
using System.Diagnostics;

namespace MicrosoftGroupChatClient
{
    public class ConsoleEventLog
    {
        public new void WriteEntry(string message)
        {
            WriteEntry(message, EventLogEntryType.Information);
        }

        internal void WriteEntry(string message, EventLogEntryType eventLogEntryType)
        {
            switch (eventLogEntryType)
            {
                case EventLogEntryType.Error:
                    Console.Error.WriteLine(message);
                    break;
                default:
                    Console.Out.WriteLine(message);
                    break;
            }
        }
    }
}
