using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

namespace MicrosoftGroupChatClient
{
    public class Program
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly GroupChat _groupChat;
        private readonly ConsoleEventLog _eventLog;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var program = new Program();
            program.StartUp();
            program.Run();
            program.ShutDown();
        }

        public Program()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _eventLog = new ConsoleEventLog();

            var userSipUri = new Uri(ConfigurationManager.AppSettings["UserSipUri"]);
            var ocsServer = ConfigurationManager.AppSettings["OcsServer"];
            var ocsUsername = ConfigurationManager.AppSettings["OcsUsername"];
            var lookupServerUri = new Uri(ConfigurationManager.AppSettings["LookupServerUri"]);
            var chatRoomName = ConfigurationManager.AppSettings["ChatRoomName"];

            var useSso = bool.Parse(ConfigurationManager.AppSettings["UseSso"]);
            if (!useSso)
            {
                Console.Out.WriteLine("Type your password");
                string ocsPassword = null;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    Console.Out.Write("*");
                    ocsPassword += key.KeyChar;
                }
                Console.Out.WriteLine();
                _groupChat = new GroupChat(userSipUri, ocsServer, ocsUsername, ocsPassword, lookupServerUri,
                    chatRoomName);
            }
            else
            {
                _groupChat = new GroupChat(userSipUri, ocsServer, lookupServerUri,
                    chatRoomName);
            }
        }

        public void StartUp()
        {
            try
            {
                _groupChat.TextMessageReceived += GroupChatTextMessageReceived;
                _eventLog.WriteEntry("Connecting to GroupChat....");
                _groupChat.Connect();
                _groupChat.Send("Connected to GroupChat.");
            }
            catch (Exception exception)
            {
                _eventLog.WriteEntry("Exception connecting to GroupChat: " + exception.Message, EventLogEntryType.Error);
                throw;
            }
            _eventLog.WriteEntry("Connected to GroupChat.");
        }

        public void Run()
        {
            Console.Out.WriteLine("Type to send to GroupChat (\"Bye\" to exit)...");
            while (true)
            {
                var message = Console.ReadLine();
                if (message == "Bye")
                {
                    return;
                }
                if (string.IsNullOrEmpty(message)) continue;
                var textMessage = new TextMessage("text", 123, "andy", "test_room", message);
                var stringMessage = JsonConvert.SerializeObject(textMessage);
                Console.Out.WriteLine("Sending:" + stringMessage);
                _groupChat.Send(textMessage.Text);
            }
        }

        public void ShutDown()
        {
            try
            {
                _groupChat.Disconnect();
            }
            catch (Exception exception)
            {
                _eventLog.WriteEntry("Exception disconnecting from GroupChat: " + exception.Message);
            }

            _cancellationTokenSource.Cancel();
            _eventLog.WriteEntry("Stopped");
        }

        private void GroupChatTextMessageReceived(object sender, TextMessageReceivedEventArgs e)
        {
            _eventLog.WriteEntry($"Got: {e.TextMessage.Text}");
        }
    }
}
