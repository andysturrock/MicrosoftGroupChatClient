using System;
using System.Diagnostics;

namespace MicrosoftGroupChatClient
{
    public class ConsoleEventLog
    {
        public void WriteEntry(string message)
        {
            WriteEntry(message, EventLogEntryType.Information);
        }

        public void WriteEntry(string message, EventLogEntryType eventLogEntryType)
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
