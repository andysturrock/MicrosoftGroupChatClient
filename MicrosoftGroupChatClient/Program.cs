using System;
using System.ComponentModel;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MicrosoftGroupChatClient
{
    public class Program
    {
        private readonly Uri _userSipUri;
        private readonly CancellationToken _cancellationToken;
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
            _cancellationToken = _cancellationTokenSource.Token;

            //_Horrible hack to prevent parent class constructor being called.
            // Needed because EventLog ctor checks security as only Administrator can write to event log.
            _eventLog = (ConsoleEventLog)FormatterServices.GetUninitializedObject(typeof(ConsoleEventLog));
            _eventLog.WriteEntry("Test");

            _userSipUri = new Uri(ConfigurationManager.AppSettings["UserSipUri"]);
            var ocsServer = ConfigurationManager.AppSettings["OcsServer"];
            var ocsUsername = ConfigurationManager.AppSettings["OcsUsername"];
            var password = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\hubot-msgc", "password", "default");
            Console.Out.WriteLine("Type your password");
            string ocsPassword = null;
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                Console.Out.Write("*");
                ocsPassword += key.KeyChar;
            }
            Console.Out.WriteLine();
            var lookupServerUri = new Uri(ConfigurationManager.AppSettings["LookupServerUri"]);
            var chatRoomName = ConfigurationManager.AppSettings["ChatRoomName"];

            _groupChat = new GroupChat(_eventLog, _userSipUri, ocsServer, ocsUsername, ocsPassword, lookupServerUri,
                chatRoomName);
        }

        public void StartUp()
        {
            try
            {
                _groupChat.TextMessageReceived += GroupChatTextMessageReceived;
                _groupChat.Connect();
                _groupChat.Send("Connected to GroupChat.");
            }
            catch (Exception exception)
            {
                _eventLog.WriteEntry("Exception connecting to GroupChat: " + exception.Message);
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
