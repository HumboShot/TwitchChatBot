using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TwitchChatBot.Helpers
{
    public class LogHelper
    {
        private string _botUsername;

        public LogHelper()
        {

        }

        public void Write(string line)
        {
            Console.WriteLine($"{DateTime.Now} [{_botUsername}]: {line}{Environment.NewLine}");
        }

        public void WriteCommand(OnChatCommandReceivedArgs command)
        {
            Write($"New command {command.Command.ChatMessage.Message} from {command.Command.ChatMessage.Username}");
        }

        public void SetBotUsername(string username)
            => _botUsername = username;
    }
}
