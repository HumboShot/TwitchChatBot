using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchChatBot.Models
{
    public class Settings
    {
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string BotUsername { get; set; }
        public string ChatCommandIdentifier { get; set; }
        public List<string> MonitoredChannels { get; set; } = new List<string>();
        public BotConnectionDetails BotConnectionDetails { get; set; } = new BotConnectionDetails();
    }

    public class BotConnectionDetails
    {
        public string UserName { get; set; }
        public string OAuth { get; set; }
    }
}
