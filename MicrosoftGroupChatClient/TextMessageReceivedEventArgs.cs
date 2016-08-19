using System;

namespace MicrosoftGroupChatClient
{
    public class TextMessageReceivedEventArgs : EventArgs
    {
        public TextMessageReceivedEventArgs(TextMessage textMessage)
        {
            TextMessage = textMessage;
        }

        public TextMessage TextMessage { get; set; }
    }
}
