using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchChatBot.Models
{
    public class TextCommand
    {
        public TextCommand(string commandName, string commandText)
        {
            CommandName = commandName;
            CommandText = commandText;
        }

        public string CommandName { get; set; }
        public string CommandText { get; set; }
    }
}
