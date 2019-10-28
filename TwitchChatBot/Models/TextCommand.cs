using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchChatBot.Models
{
    public class TextCommand
    {
        public TextCommand(string commandName, string commandText, string channel)
        {
            CommandName = commandName;
            CommandText = commandText;
            ChannelName = channel;
        }

        public string CommandName { get; set; }
        public string CommandText { get; set; }
        public string ChannelName { get; set; }
    }
}
